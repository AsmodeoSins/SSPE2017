using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;



namespace ControlPenales
{
    partial class ConsultaUnificadaViewModel : ValidationViewModelBase
    {
        #region constructor
        public ConsultaUnificadaViewModel() { }
        #endregion

        #region metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    Buscar();
                    break;
                case "ver":
                    if(pImprimir)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                        if (SelectedConsultaUnificada != null)
                        VerDocumento();
                    else
                        new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un documento");
                    break;
                case "limpiar_menu":
                       ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ConsultaUnificadaView();
                       ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.ConsultaUnificadaViewModel();
                    break;
                case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private async void OnLoad(ConsultaUnificadaView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }

        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    if(obj is TextBox)
                    {
                        var tb = (TextBox)obj;
                        FNombre = tb.Text;
                        Buscar();
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar busqueda", ex);
            }
        }

        private void CargarListas()
        {
            try
            {
                ConfiguraPermisos();
                LstClasificacion = new List<CLASIFICACION_DOCUMENTO>(new cClasificacionDocumento().ObtenerTodos(string.Empty));
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstClasificacion.Insert(0, new CLASIFICACION_DOCUMENTO() { ID_CLASIFICACION = -1, DESCR = "TODO" });
               
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listados", ex);
            }
        }
        #endregion

        #region Consulta Unificada
        private async void Buscar() 
        {
            try
            {
                fNombreAux = FNombre;
                fClasificacionAux = FClasificacion;
                LstConsultaUnificada = new RangeEnabledObservableCollection<CONSULTA_UNIFICADA>();
                LstConsultaUnificada.InsertRange(await SegmentarResultadoBusqueda(1));
                DataGridVacio = LstConsultaUnificada.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
               StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar.", ex);
            }
        }

        private async Task<List<CONSULTA_UNIFICADA>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<CONSULTA_UNIFICADA>>(() =>
                             new ObservableCollection<CONSULTA_UNIFICADA>(new cConsultaUnificada().ObtenerTodos(fNombreAux,fClasificacionAux,_Pag)));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar mas documentos.", ex);
                return new List<CONSULTA_UNIFICADA>();
            }
        }

        private async void VerDocumento() 
        {
            try
            {
                var view = new ReportePDFView();
                view.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                 await Task.Factory.StartNew(() =>
                    {
                        var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                        File.WriteAllBytes(fileNamepdf,SelectedConsultaUnificada.DOCUMENTO);
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            view.pdfViewer.LoadFile(fileNamepdf);
                            view.Show();
                            //pdfViewer.Visibility = Visibility.Visible;

                        }));
                    });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar documento.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CONSULTA_UNIFICADA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        pInsertar = true;
                    if (p.EDITAR == 1)
                        pEditar = true;
                    if (p.CONSULTAR == 1)
                        pConsultar = true;
                    if (p.IMPRIMIR == 1)
                        pImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
}

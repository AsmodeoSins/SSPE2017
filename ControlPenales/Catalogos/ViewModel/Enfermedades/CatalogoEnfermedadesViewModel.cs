using ControlPenales.Clases.Estatus;
using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    public partial class CatalogoEnfermedadesViewModel:ValidationViewModelBase
    {
        #region General
        public async void CatalogoEnfermedadesLoad(CatalogoEnfermedadesView Window = null)
        {
           try
           {
               LlenarTipoEnfermedades();
               BuscarEnfermedades();
               AgregarVisible = false;
               await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                   ObtenerSectores_Clasificacion(true);
               });
               setValidacionRules();
               StaticSourcesViewModel.SourceChanged = false;
           }
            catch(Exception ex)
           {
               StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la vista.", ex);
           }
        }

        private async void ClickSwitch(object parametro)
        {
            if (parametro!=null && !string.IsNullOrWhiteSpace(parametro.ToString()))
                switch (parametro.ToString())
                {
                    case "buscar":
                        BuscarEnfermedades();
                        break;
                    case "menu_agregar":
                        modo_seleccionado = MODO_OPERACION.INSERTAR;
                        AgregarVisible = true;
                        Limpiar();
                        SelectedTipoEnfermedadValue = "-1";
                        GuardarMenuEnabled = true;
                        CancelarMenuEnabled = true;
                        EditarMenuEnabled = false;
                        AgregarMenuEnabled = false;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "menu_guardar":
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios. \n\n" + base.Error);
                            return;
                        }
                        Guardar();
                        break;
                    case "menu_cancelar":
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "Existen cambios sin guardar,¿desea cancelar la operación?") != 1)
                                return;
                        }
                        AgregarVisible = false;
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        IsEnfermedadesEnabled = true;
                        Limpiar();
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "menu_editar":
                        if (SelectedEnfermedad==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccionar una enfermedad para editar");
                            return;
                        }
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                            selectedTipoEnfermedadValue = selectedEnfermedad.TIPO;
                            RaisePropertyChanged("SelectedTipoEnfermedadValue");
                            textLetraEnfermedad = selectedEnfermedad.LETRA;
                            RaisePropertyChanged("TextLetraEnfermedad");
                            textClaveEnfermedad = selectedEnfermedad.CLAVE;
                            RaisePropertyChanged("TextClaveEnfermedad");
                            textEnfermedad = selectedEnfermedad.NOMBRE;
                            RaisePropertyChanged("TextEnfermedad");
                            IsEnfermedadesEnabled = false;
                            AgregarVisible = true;
                            GuardarMenuEnabled = true;
                            CancelarMenuEnabled = true;
                            EditarMenuEnabled = false;
                            AgregarMenuEnabled = false;
                            if (selectedEnfermedad.SECTOR_CLASIFICACION!=null && selectedEnfermedad.SECTOR_CLASIFICACION.Count>0)
                            {
                                foreach(var item in selectedEnfermedad.SECTOR_CLASIFICACION)
                                {
                                    if (lstSectorClasificacion.FirstOrDefault(w => w.SECTOR_CLASIFICACION.ID_SECTOR_CLAS == item.ID_SECTOR_CLAS) != null)
                                        lstSectorClasificacion.First(w => w.SECTOR_CLASIFICACION.ID_SECTOR_CLAS == item.ID_SECTOR_CLAS).IS_CHECKED = true;
                                }
                                lstSectorClasificacion = new List<EXT_SECTOR_CLASIFICACION>(lstSectorClasificacion);
                                RaisePropertyChanged("LstSectorClasificacion");
                            }
                        });
                        
                        modo_seleccionado = MODO_OPERACION.EDICION;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "menu_salir":
                        SalirMenu();
                        break;
                }
        }

        public async static void SalirMenu()
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?");
                    if (dialogresult != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }

                var metro = Application.Current.Windows[0] as MetroWindow;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                GC.Collect();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir del módulo", ex);
            }
        }

        private async void  Guardar()
        {
            try
            {
                if (modo_seleccionado==MODO_OPERACION.INSERTAR)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la enfermedad en el catalogo", () =>
                    {
                        var _sectores_seleccionados = lstSectorClasificacion.Where(w => w.IS_CHECKED).Select(s => s.SECTOR_CLASIFICACION.ID_SECTOR_CLAS).ToList();
                        var _enfermedad = new ENFERMEDAD
                        {
                            CLAVE = textClaveEnfermedad,
                            LETRA = textLetraEnfermedad,
                            NOMBRE = textEnfermedad,
                            TIPO = selectedTipoEnfermedadValue
                        };
                        new cEnfermedad().Insertar(_enfermedad, _sectores_seleccionados);
                        return true;
                    }))
                    {
                        ListEnfermedades = new RangeEnabledObservableCollection<ENFERMEDAD>();
                        ListEnfermedades.InsertRange(await SegmentarResultadoBusqueda());
                        if (ListEnfermedades == null || ListEnfermedades.Count() == 0)
                            EmptyVisible = true;
                        else
                            EmptyVisible = false;
                        AgregarVisible = false;
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La enfermedad fue guardada en el catalogo con exito");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else if (modo_seleccionado==MODO_OPERACION.EDICION)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la enfermedad en el catalogo", () =>
                    {
                        var _sectores_seleccionados = lstSectorClasificacion.Where(w => w.IS_CHECKED).Select(s => s.SECTOR_CLASIFICACION.ID_SECTOR_CLAS).ToList();
                        var _enfermedad = new ENFERMEDAD
                        {
                            ID_ENFERMEDAD=SelectedEnfermedad.ID_ENFERMEDAD,
                            CLAVE = textClaveEnfermedad,
                            LETRA = textLetraEnfermedad,
                            NOMBRE = textEnfermedad,
                            TIPO = selectedTipoEnfermedadValue
                        };
                        new cEnfermedad().Editar(_enfermedad, _sectores_seleccionados);
                        return true;
                    }))
                    {
                        ListEnfermedades = new RangeEnabledObservableCollection<ENFERMEDAD>();
                        ListEnfermedades.InsertRange(await SegmentarResultadoBusqueda());
                        if (ListEnfermedades == null || ListEnfermedades.Count() == 0)
                            EmptyVisible = true;
                        else
                            EmptyVisible = false;
                        AgregarVisible = false;
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        IsEnfermedadesEnabled = true;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La enfermedad fue editada en el catalogo con exito");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar en el catalogo de enfermedades.", ex);
            }
        }

        private void Limpiar()
        {
            TextClaveEnfermedad = string.Empty;
            TextEnfermedad = string.Empty;
            TextLetraEnfermedad = string.Empty;
            SelectedTipoEnfermedadValue = "-1";
            foreach (var item in lstSectorClasificacion)
                item.IS_CHECKED = false;
            LstSectorClasificacion = new List<EXT_SECTOR_CLASIFICACION>(lstSectorClasificacion);
        }
        #endregion

        #region busqueda segmentada
        private async void BuscarEnfermedades()
        {
            try
            {
                ListEnfermedades = new RangeEnabledObservableCollection<ENFERMEDAD>();
                ListEnfermedades.InsertRange(await SegmentarResultadoBusqueda());
                if (ListEnfermedades == null || ListEnfermedades.Count() == 0)
                    EmptyVisible = true;
                else
                    EmptyVisible = false;
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar las enfermedades.", ex);
            }
        }

        private async Task<List<ENFERMEDAD>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ENFERMEDAD>>(() => new ObservableCollection<ENFERMEDAD>(new cEnfermedad().ObtenerTodosPorRango(TextBuscarEnfermedad, Pagina)));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de enfermedades.", ex);
                return new List<ENFERMEDAD>();
            }
            
        }
        #endregion

        #region Catalogos
        private void LlenarTipoEnfermedades()
        {
            lstTipoEnfermedad = new List<Tipos_Enfermedades>() {new Tipos_Enfermedades{
                ID="-1",
                DESCR="SELECCIONE"
            },
            new Tipos_Enfermedades{
                ID="M",
                DESCR="MEDICO"
            },
            new Tipos_Enfermedades{
                ID="D",
                DESCR="DENTAL"
            }};
            RaisePropertyChanged("LstTipoEnfermedad");
        }

        private void ObtenerSectores_Clasificacion(bool isExceptionManaged=false)
        {
            try
            {
                lstSectorClasificacion = new cSectorClasificacion().ObtenerGruposVulnerables().Select(s => new EXT_SECTOR_CLASIFICACION {
                    IS_CHECKED=false,
                    SECTOR_CLASIFICACION=s
                }).ToList();
                RaisePropertyChanged("LstSectorClasificacion");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar lso catalogos de interconsulta externa", ex);
            }
        }
        #endregion

        public class Tipos_Enfermedades
        {
            private string _id;
            public string ID
            {
                get { return _id; }
                set { _id = value; }
            }
            private string _descr;
            public string DESCR
            {
                get { return _descr; }
                set { _descr = value; }
            }
        }

        private enum MODO_OPERACION
        {
            INSERTAR=1,
            EDICION=2
        }
    }
}

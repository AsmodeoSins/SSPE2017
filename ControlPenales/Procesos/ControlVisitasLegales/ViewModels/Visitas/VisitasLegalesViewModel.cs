using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class VisitasLegalesViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region Variables
        public string Name { get; set; }
        #endregion

        #region Métodos
        void IPageViewModel.inicializa() { }

        public void ObtenerVisitas()
        {
            var lista_visitas = new List<InternoVisitaAbogado>();
            var FechaServer = Fechas.GetFechaDateServer.Date;
            var visitas = new cAduanaIngreso().ObtenerVisitasAbogados(FechaServer).ToList();
            foreach (var visita in visitas)
            {
                lista_visitas.Add(new InternoVisitaAbogado()
                {
                    ID_CENTRO = visita.ID_CENTRO,
                    ID_ANIO = visita.ID_ANIO,
                    ID_IMPUTADO = (short)visita.ID_IMPUTADO,
                    PATERNO = visita.INGRESO.IMPUTADO.PATERNO.TrimEnd(),
                    MATERNO = visita.INGRESO.IMPUTADO.MATERNO.TrimEnd(),
                    NOMBRE = visita.INGRESO.IMPUTADO.NOMBRE.TrimEnd(),
                    TIPO_VISITANTE = (short)visita.ADUANA.ID_TIPO_PERSONA,
                    NOMBRE_VISITANTE = string.Format("{1} {2} {0}", visita.ADUANA.PERSONA.NOMBRE.TrimEnd(), visita.ADUANA.PERSONA.PATERNO.TrimEnd(), visita.ADUANA.PERSONA.MATERNO.TrimEnd())
                });
            }
            ListaInternoAbogado = lista_visitas;
        }
        #endregion

        #region Métodos Eventos
        public async void CargarVisitas(VisitasLegalesView Ventana)
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                try
                {
                    this.Ventana = Ventana;
                    ObtenerVisitas();
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Hubo un error al cargar las visitas existentes", ex);
                }
            });
        }

        public async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "actualizar_menu":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        ObtenerVisitas();
                    });
                    break;
                case "huellas_menu":
                    try
                    {
                        var windowBusqueda = new BusquedaHuellaVisita();
                        windowBusqueda.DataContext = new BusquedaHuellaVisitaViewModel();
                        windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                        windowBusqueda.Closed += async (s, e) =>
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                ObtenerVisitas();
                            });
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        windowBusqueda.ShowDialog();
                    }
                    catch (Exception ex)
                    {

                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al inicializar la ventana de lectura de huellas", ex);
                    }
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }
        #endregion
    }
}

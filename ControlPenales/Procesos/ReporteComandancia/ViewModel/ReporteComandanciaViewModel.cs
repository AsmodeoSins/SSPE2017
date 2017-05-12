using ControlPenales;
using ControlPenales.BiometricoServiceReference;
using LinqKit;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace ControlPenales
{
    public partial class ReporteComandanciaViewModel : ValidationViewModelBase
    {
        #region Constructor
        public ReporteComandanciaViewModel() { }
        #endregion
        
        private async void SwitchClick(Object obj)
        {
            ReportViewerVisible = false;

            //StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
            //  reporte.Visible = false;
            //GenerarReporte();
           // await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);

            // reporte.Visible = true;
            ReportViewerVisible = true;

        }

        #region Reporte
        private void GenerarReporte()
        {
        }
          #endregion
        
        private async void OnLoad(ReporteComandanciaView Window = null)
        {
            try
            {
                Reporte = Window.Report;
                //await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);

                //ValidarFiltros();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_CAUSA_PENAL.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

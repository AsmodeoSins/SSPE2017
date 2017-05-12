using ControlPenales.Clases.ControlInternos;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReportePoblacionDelitoFederalViewModel : ValidationViewModelBase
    {
        #region [METODOS]
        private async void ClickSwitch(object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "generar":
                        if (!pConsultar)
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        ReportViewerVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
                        ReportViewerVisible = Visibility.Visible;
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void GenerarReporte()
        {
            try
            {
                var mes = Fechas.GetFechaDateServer.Month;
                //NOTA: ANIO TEMPORALMENTE FIJO SOLO PARA PRUEBAS DEBIDO A QUE NO HAY REGISTROS PARA EL ANIO 2016
                //DESCOMENTAR MAS ADELANTE
                var anio = 2010;//Fechas.GetFechaDateServer.Year;

                //var lista_internos_mes = internos_lista.Where(w => w.FEC_INGRESO_CERESO.Value.Month == mes).ToList();
                var datosReporte = new List<cReporteDatos>();
                // List<string> termsList = new List<string>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Población por delito federal"
                });
                var lst_internos_causa_penal_delito = new cCausaPenalDelito().ObtenerTodos().ToList();
                var internos_lista = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro).Where(w => w.FEC_INGRESO_CERESO.Value.Month == mes && w.FEC_INGRESO_CERESO.Value.Year == anio).ToList();
                int x = 0;
                var lst_imp = new List<cPoblacionDelitoFeferal>();

                foreach (var item in lst_internos_causa_penal_delito)
                {
                    if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "F")
                    {
                        var obj = new cPoblacionDelitoFeferal();
                        obj.DelitoFederal = item.MODALIDAD_DELITO.DELITO.DESCR;
                        obj.Sexo = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO;
                        obj.Indiciados = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" ? 1 : 0;
                        obj.Procesados = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" ? 1 : 0;
                        obj.Sentenciados = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" ? 1 : 0;

                        lst_imp.Add(obj);
                    }
                }
                var results = lst_imp.GroupBy(n => n.DelitoFederal).
                     Select(group =>
                         new
                         {
                             DelitoFederal = group.Key,
                             Sexo = group.ToList(),
                         });

                var lst_imp2 = new List<cPoblacionDelitoFeferal>();

                foreach (var row in results)
                {
                    var obj = new cPoblacionDelitoFeferal();
                    obj.DelitoFederal = row.DelitoFederal;
                    foreach (var item in row.Sexo)
                    {
                        obj.Sexo = item.Sexo == "M" ? "MASCULINO" : item.Sexo == "F" ? "FEMENINO" : string.Empty;
                        obj.Indiciados += item.Indiciados;
                        obj.Procesados += item.Procesados;
                        obj.Sentenciados += item.Sentenciados;

                    }
                    lst_imp2.Add(obj);
                }
                Reporte.LocalReport.ReportPath = "../../Reportes/rPoblacionDelitoFederal.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = lst_imp2;
                Reporte.LocalReport.DataSources.Add(rds2);
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }

        private void OnLoad(ReportePoblacionDelitoFederalView window)
        {
            try
            {
                ConfiguraPermisos();
                Reporte = window.Report;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_POBLACION_DELITO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

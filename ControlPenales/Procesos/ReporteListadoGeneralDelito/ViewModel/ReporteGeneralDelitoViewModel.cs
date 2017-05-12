using ControlPenales.BiometricoServiceReference;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteGeneralDelitoViewModel
    {
        List<cReporteListadoGeneralDelito> ListValue;
        private void OnLoad(ReporteGeneralDelitoView Window)
        {
            try
            {
                ConfiguraPermisos();
                //ListaOrdenamiento = ListaOrdenamiento ?? new ObservableCollection<Orderby>(
                //    new List<Orderby>() { 
                //    new Orderby { Text="SELECCIONE",Value= OrderByValues.Ninguno },
                //    new Orderby { Text="NÚMERO DE EXPEDIENTE",Value= OrderByValues.Numero_Expediente },
                //    new Orderby { Text="NOMBRE DEL INTERNO",Value= OrderByValues.Nombre_Interno },
                //    new Orderby { Text="FECHA DE INGRESO",Value= OrderByValues.Fecha_Ingreso },
                //    new Orderby { Text="UBICACIÓN",Value= OrderByValues.Ubicacion },
                //    new Orderby { Text="CLASIFICACIÓN JURÍDICA",Value= OrderByValues.Clasificacion_Juridica } 
                //});

                Reporte = Window.Report;
                Reporte.RenderingComplete += (s, e) =>
                {
                    ReportViewerVisible = Visibility.Visible;
                };

                //SelectedValue = OrderByValues.Ninguno;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla", ex);
            }
        }

        private async void SwitchClick(object obj)
        {
            if (!pConsultar)
            {
                ReportViewerVisible = Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
           // if (SelectedValue == OrderByValues.Ninguno)
           //     return;
            ReportViewerVisible = Visibility.Collapsed;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
        }

        #region Reporte
        private void GenerarReporte()
        {
            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "RELACIÓN DE INTERNOS",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });


                #region armar reporte
                var hoy = Fechas.GetFechaDateServer;
                var f  = new Fechas();
                var reporte = new List<cReporteListadoGeneralDelito>();
                var lista = new cIngreso().ReporteGeneralDelito(GlobalVar.gCentro, OrdenarPor, Ordenamiento);
                if (lista != null)
                {
                    foreach (var w in lista)
                    { 
                        reporte.Add(new cReporteListadoGeneralDelito(){
                         Expediente = string.Format("{0}/{1}",w.ID_ANIO,w.ID_IMPUTADO),
                         Ingreso = w.ID_INGRESO.ToString(),
                         Edad = IncluirEdad ? (short?)f.CalculaEdad(w.NACIMIENTO_FECHA,hoy) : 0,
                         NombreCompleto = string.Format("{0} {1} {2}",
                         !string.IsNullOrEmpty(w.NOMBRE) ? w.NOMBRE.Trim() : string.Empty,
                         !string.IsNullOrEmpty(w.PATERNO) ? w.PATERNO.Trim() : string.Empty,
                         !string.IsNullOrEmpty(w.MATERNO) ? w.MATERNO.Trim() : string.Empty),
                         Ubicacion = string.Format("{0}-{1}-{2}-{3}",
                                         !string.IsNullOrEmpty(w.EDIFICIO) ? w.EDIFICIO.Trim() : string.Empty,
                                         !string.IsNullOrEmpty(w.SECTOR) ? w.SECTOR.Trim() : string.Empty,
                                         !string.IsNullOrEmpty(w.CELDA) ? w.CELDA.Trim() : string.Empty,
                                         w.CAMA),
                         Fuero = string.IsNullOrEmpty(w.FUERO) ? (w.FUERO == "C" ? "COMUN" : w.FUERO == "F" ? "FEDERAL" : "DESCONOCIDO") : "DESCONOCIDO",
                         Situacion = w.CLASIFICACION_JURIDICA,
                         FechaIngreso = w.FEC_INGRESO_CERESO,
                         DelitoIngreso = w.INGRESO_DELITO,
                         Foto = IncluirFoto ? (w.FOTO != null ? w.FOTO : new Imagenes().getImagenPerson()) : null,
                         Alias = w.ALIAS,
                        });
                    }
                }
                #endregion

                #region Reporte
                Reporte.LocalReport.ReportPath = "Reportes/rListadoGeneralDelito.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                var rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = reporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                #region Parametros
                Reporte.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("MostrarEdad", IncluirEdad ? "N" : "S"));
                Reporte.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("MostrarFoto", IncluirFoto ? "N" : "S"));
                #endregion

                //cReporteListadoGeneralDelito
                #region comentado
                //ListValue = new cIngreso().GetData().Where(w => w.ID_UB_CENTRO == GlobalVar.gCentro && !Parametro.ESTATUS_ADMINISTRATIVO_INACT.Contains(w.ID_ESTATUS_ADMINISTRATIVO)).GroupBy(g => new { g.ID_ANIO, g.ID_IMPUTADO }).SelectMany(sm => sm.Where(w => w.ID_INGRESO == sm.Max(m => m.ID_INGRESO))).AsEnumerable().Select(s => new cReporteListadoGeneralDelito()
                //{
                //    ID_ANIO = s.ID_ANIO,
                //    ID_IMPUTADO = s.ID_IMPUTADO,
                //    Expediente = s.ID_ANIO + "\\" + s.ID_IMPUTADO,
                //    NombreCompleto = (s.IMPUTADO.PATERNO != null ? s.IMPUTADO.PATERNO.Trim() : string.Empty) + " " + (s.IMPUTADO.MATERNO != null ? s.IMPUTADO.MATERNO.Trim() : string.Empty) + " " + (s.IMPUTADO.NOMBRE != null ? s.IMPUTADO.NOMBRE.Trim() : string.Empty),
                //    Ingreso = s.ID_INGRESO.ToString(),
                //    Alias = getAlias(s.IMPUTADO.ALIAS),// "preguntar por alias",
                //    DelitoIngreso = s.INGRESO_DELITO != null ? s.INGRESO_DELITO.DESCR : string.Empty,
                //    Fuero = "preguntar por fuero",
                //    Situacion = s.CLASIFICACION_JURIDICA != null ? s.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty,
                //    Ubicacion = s.CAMA != null ? s.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + s.CAMA.CELDA.SECTOR.DESCR.Trim() + s.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(s.CAMA.DESCR) ? s.CAMA.ID_CAMA.ToString().Trim() : s.CAMA.ID_CAMA + " " + s.CAMA.DESCR.Trim()) : string.Empty,
                //    Foto = IncluirFoto ? s.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ? s.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : null : null,


                //    FechaIngreso = s.FEC_INGRESO_CERESO,
                //    Edad = IncluirEdad ? new Fechas().CalculaEdad(s.IMPUTADO.NACIMIENTO_FECHA) : new Nullable<short>()
                //}).ToList();

                //switch (SelectedValue)
                //{
                //    case OrderByValues.Ninguno:
                //        break;
                //    case OrderByValues.Numero_Expediente:
                //        if (Ordenamiento)
                //            rds1.Value = ListValue.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).ToList();
                //        else
                //            rds1.Value = ListValue.OrderByDescending(o => o.ID_ANIO).ThenByDescending(t => t.ID_IMPUTADO).ToList();
                //        break;
                //    case OrderByValues.Nombre_Interno:
                //        if (Ordenamiento)
                //            rds1.Value = ListValue.OrderBy(o => o.NombreCompleto).ToList();
                //        else
                //            rds1.Value = ListValue.OrderByDescending(o => o.Expediente).ToList();
                //        break;
                //    case OrderByValues.Fecha_Ingreso:
                //        if (Ordenamiento)
                //            rds1.Value = ListValue.OrderBy(o => o.FechaIngreso).ToList();
                //        else
                //            rds1.Value = ListValue.OrderByDescending(o => o.Expediente).ToList();
                //        break;
                //    case OrderByValues.Ubicacion:
                //        if (Ordenamiento)
                //            rds1.Value = ListValue.OrderBy(o => o.Ubicacion).ToList();
                //        else
                //            rds1.Value = ListValue.OrderByDescending(o => o.Expediente).ToList();
                //        break;
                //    case OrderByValues.Clasificacion_Juridica:
                //        if (Ordenamiento)
                //            rds1.Value = ListValue.OrderBy(o => o.DelitoIngreso).ToList();
                //        else
                //            rds1.Value = ListValue.OrderByDescending(o => o.Expediente).ToList();
                //        break;
                //    default:
                //        break;
                //}

                //Reporte.LocalReport.DataSources.Add(rds1);
                #endregion

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.RefreshReport();
                }));
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }

        private string getAlias(ICollection<ALIAS> collection)
        {
            var alias = string.Empty;

            foreach (var item in collection.Select(se => new { ALIAS = ((se.PATERNO != null ? se.PATERNO.Trim() : string.Empty) + " " + (se.MATERNO != null ? se.MATERNO.Trim() : string.Empty) + " " + (se.NOMBRE != null ? se.NOMBRE.Trim() : string.Empty)) }))
                alias += item.ALIAS + ",\n";

            return alias.Length != 0 ? alias.Remove(alias.Length - 2) : string.Empty;
        }

        void LimpiarReporte()
        {
            try
            {
                ReportViewerVisible = Visibility.Collapsed;
                Reporte.Reset();
                Reporte.RefreshReport();
                ReportViewerVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar reporte", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_LISTADO_GENERAL_DELITO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace ControlPenales
{
    public partial class ReporteTrasladosEstatalesViewModel:ValidationViewModelBase
    {
        private void OnLoaded(ReporteTrasladosEstatalesView window)
        {
            try
            {
                ConfiguraPermisos();
                lstTipoTraslado = new List<Tipo_Traslado> {
            new Tipo_Traslado("-1","SELECCIONE"),
            new Tipo_Traslado("T","TODOS"),
            new Tipo_Traslado("L","LOCAL"),
            new Tipo_Traslado("F", "FORANEO")
            };
                RaisePropertyChanged("LstTipoTraslado");
                SelectedTipoTrasladoValue = "-1";
                _reporte = window.Report;
                setValidaciones();
            }
            catch (Exception ex)
            {
                ReportViewerVisible = Visibility.Collapsed;
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla", ex);
            }
        }

        private async void GeneraReporte(object parametro)
        {
            if (!pConsultar)
            {
                _reporte.Visible = false;
                ReportViewerVisible = Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            if (base.HasErrors)
            {
                _reporte.Visible = false;
                ReportViewerVisible = Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                return;
            }
                
            var tipo_movimientos = isEgresoChecked ? "E" : isIngresoChecked? "I": "";
            ReportViewerVisible = Visibility.Collapsed;
            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cargando datos", GeneraReporteDatos))
               ReportViewer_Generar();
        }

        private void ReportViewer_Generar()
        {
            try
            {
                _reporte.Reset();
                _reporte.Visible = false;
                _reporte.LocalReport.ReportPath = "Reportes/rTrasladoEstatal.rdlc";
                _reporte.LocalReport.DataSources.Clear();
                Microsoft.Reporting.WinForms.ReportDataSource rsd1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rsd1.Name = "DS_DETALLE";
                rsd1.Value = ds_detalle;
                Microsoft.Reporting.WinForms.ReportDataSource rsd2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rsd2.Name = "DS_ENCABEZADO";
                rsd2.Value = ds_encabezado;
                _reporte.LocalReport.DataSources.Add(rsd1);
                _reporte.LocalReport.DataSources.Add(rsd2);
                _reporte.RefreshReport();
                _reporte.Visible = true;
                ReportViewerVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ReportViewerVisible = Visibility.Visible;
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el reporte.", ex);
            }
        }

        private bool GeneraReporteDatos()
        {
            try
            {
                var tipo_movimiento = isEgresoChecked ? "E" : isIngresoChecked ? "I" : "";
                var _otros_emisor = Parametro.ID_EMISOR_OTROS;
                System.DateTime _fechaS = Fechas.GetFechaDateServer;
                ds_detalle = new cTrasladoDetalle().ObtenerTodosPorTipo(tipo_movimiento, GlobalVar.gCentro, fechaInicio, fechaFin, selectedTipoTrasladoValue).AsEnumerable()
                    .Select(s => new EXT_REPORTE_TRASLADO_DETALLE
                    {
                        CERESO_DESTINO = tipo_movimiento == "E" ?
                            s.TRASLADO.CENTRO_DESTINO != null ?
                                s.TRASLADO.CENTRO.DESCR
                                : s.TRASLADO.ID_CENTRO_DESTINO_FORANEO != _otros_emisor ?
                                        s.TRASLADO.EMISOR.DESCR
                                        : s.TRASLADO.OTRO_CENTRO_DESTINO_FORANEO
                            : s.TRASLADO.CENTRO_ORIGEN != null ?
                                s.TRASLADO.CENTRO1.DESCR
                                : s.TRASLADO.ID_CENTRO_ORIGEN_FORANEO == _otros_emisor ?
                                    s.TRASLADO.CENTRO_ORIGEN_FORANEO
                                    : s.TRASLADO.EMISOR1.DESCR,
                        EXPEDIENTE = s.ID_ANIO.ToString() + "/" + s.ID_IMPUTADO.ToString().PadLeft(2, '0') + "-" + s.ID_INGRESO.ToString(),
                        FEC_TRASLADO = s.EGRESO_FEC.HasValue ? s.EGRESO_FEC.Value : _fechaS,//CONSIDERAR LA POSIBILIDAD DE MANEJAR TODOS LOS CAMPOS COMO STRING, ESTE CAMPO VENIA VACIO Y OCASIONA UN ERROR EN TIEMPO DE EJECUCION
                        MOTIVO_TRASLADO =  s.TRASLADO != null ? s.TRASLADO.TRASLADO_MOTIVO != null ? !string.IsNullOrEmpty(s.TRASLADO.TRASLADO_MOTIVO.DESCR) ? s.TRASLADO.TRASLADO_MOTIVO.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                        NOMBRECOMPLETO = s.INGRESO != null ? s.INGRESO.IMPUTADO != null ? ObtieneNombre(s.INGRESO.IMPUTADO) : string.Empty : string.Empty,
                        TIPO_MOVIMIENTO = tipo_movimiento,
                        UBICACION = s.INGRESO.CAMA != null ?
                                s.INGRESO.CAMA.CELDA != null ?
                                    s.INGRESO.CAMA.CELDA.SECTOR != null ?
                                        s.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO != null ?
                                            s.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + s.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() + "" + s.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim() + "-" + s.INGRESO.CAMA.ID_CAMA
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : string.Empty,
                    }).ToList();
                var logo_bc = Parametro.LOGO_ESTADO_BC;
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var rango = string.Empty;
                if (fechaInicio.HasValue)
                    if (fechaFin.HasValue)
                        rango = "Desde el día " + fechaInicio.Value.ToShortDateString() + " hasta el día" + fechaFin.Value.ToShortDateString();
                    else
                        rango = "Desde el día " + fechaInicio.Value.ToShortDateString();
                ds_encabezado = new List<EXT_REPORTE_TRASLADO_ENCABEZADO>() {new EXT_REPORTE_TRASLADO_ENCABEZADO{
                LOGO_BC=logo_bc,
                CENTRO_ORIGEN= centro  != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty,
                RANGO_FECHAS=rango,
                TITULO= "Reporte de Traslados Estatales " + tipo_movimiento == "E"? "Enviados":"Recibidos"
                } };
                return true;

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private string ObtieneNombre(IMPUTADO imp)
        {
            return string.Format("{0} {1} {2}", !string.IsNullOrEmpty(imp.NOMBRE) ? imp.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(imp.PATERNO) ? imp.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(imp.MATERNO) ? imp.MATERNO.Trim() : string.Empty);
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_TRASLADOS_ESTATALES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

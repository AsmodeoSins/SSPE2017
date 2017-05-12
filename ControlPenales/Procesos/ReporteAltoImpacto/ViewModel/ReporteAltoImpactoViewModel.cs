using ControlPenales;
using ControlPenales.BiometricoServiceReference;
using LinqKit;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Objects;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
namespace ControlPenales
{
    public partial class ReporteAltoImpactoViewModel : ValidationViewModelBase
    {
        #region Constructor
        public ReporteAltoImpactoViewModel() { }
        #endregion
        
        private async void SwitchClick(Object obj)
        {
            if (!pConsultar)
            {
                Reporte.Visible = false;
                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                return;
            }

            if (base.HasErrors)
            {
                Reporte.Visible = false;
                (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de llenar todos los campo Obligatorios.");
            }
            else
            {
                ReportViewerVisible = false;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
                ReportViewerVisible = true;
            }

        }

        #region Reporte
        private void GenerarReporte()
        {

            //Filtra todos los delitos que se realizaron durante la fecha inicio y fecha con la fecha de ingreso a cereso(INRGRESO)
            var predicatefUERO = PredicateBuilder.True<FUERO>();

            var predicate = PredicateBuilder.True<CAUSA_PENAL_DELITO>();

            if (TextFechaInicio.HasValue)
                predicate = predicate.And(w => EntityFunctions.TruncateTime(w.CAUSA_PENAL.INGRESO.FEC_INGRESO_CERESO) >= TextFechaInicio);
            if (TextFechaFin.HasValue)
                predicate = predicate.And(w => EntityFunctions.TruncateTime(w.CAUSA_PENAL.INGRESO.FEC_INGRESO_CERESO) <= TextFechaFin);

            //----------------------------------------------------------------------------------------------------------------------------------

            ///-------------------------------------------------FILTROS REPORTE ------------------------------------------------------------->
            if (SelectFuero != "SELECCIONE")
            {
                predicate = predicate.And(w => w.ID_FUERO == SelectFuero);//Obtiene todos los delitos relacionado al fuero seleccionado
            }
            //--CAUSA PENAL DELITOS --
            var ObtenerDelitos = new cCausaPenalDelito().GetData(predicate.Expand());


            ObtenerCatidaPorGrupos(ObtenerDelitos, SelectTitulo, SelectGrupoDelito);


        }


        /// <summary>
        /// Obtiene listado de delitos obteniendo la cantidad de delitos por grupos dependiendo del Fuero-Titulo-Grupo
        /// Si no selecciona ningun fuero se filtran todos los delitos  con sus titulos y grupos relacionados a todos los ingresos del rango de fechas
        /// si se filtra por un fuero 
        /// Filtra dependiendo si se selecciono algun filtro
        /// </summary>
        private void ObtenerCatidaPorGrupos(IQueryable<CAUSA_PENAL_DELITO> LstCausaPenalDelito, short? Titulo_Seleccionado = null, short? Grupo_Seleccionado = null)
        {
            try
            {
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = !string.IsNullOrEmpty(Parametro.ENCABEZADO1) ? Parametro.ENCABEZADO1.Trim() : string.Empty,
                    Encabezado2 = !string.IsNullOrEmpty(Parametro.ENCABEZADO2) ? Parametro.ENCABEZADO2.Trim() : string.Empty,
                    Encabezado3 = !string.IsNullOrEmpty(Parametro.ENCABEZADO3) ? Parametro.ENCABEZADO3.Trim() : string.Empty,

                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    // Centro = centro.DESCR.Trim().ToUpper(),
                });

                var Delito = new cDelito();
                var datosReporteAltoImpacto = new List<ReporteAltoImpacto>();

                foreach (var itemDelitos in LstCausaPenalDelito)
                {
                    if (Titulo_Seleccionado > -1)
                    {
                        //****Verifica que exista el delito de los  ingresos filtrado anteriormente por fechas  y que el titulo sea igual al que se selecciono en el combobox y se filtra tambien por grupo_delito en caso de que el usuario lo selecciono
                        var DelitosFiltrado = Grupo_Seleccionado > -1 ? Delito.ObtenerTodos().Where(w => w.ID_DELITO == itemDelitos.ID_DELITO && w.ID_TITULO == Titulo_Seleccionado && w.ID_GRUPO_DELITO == Grupo_Seleccionado).FirstOrDefault() : Delito.ObtenerTodos().Where(w => w.ID_DELITO == itemDelitos.ID_DELITO && w.ID_TITULO == Titulo_Seleccionado).FirstOrDefault();
                        if (DelitosFiltrado != null)
                        {//****si existe se guarda en la lista 
                            var AltoImactos = new ReporteAltoImpacto();
                            AltoImactos.Delito = itemDelitos.DESCR_DELITO;
                            AltoImactos.Fuero = DelitosFiltrado.DELITO_GRUPO != null ? DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO != null ? DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO.FUERO != null ? !string.IsNullOrEmpty(DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO.FUERO.DESCR) ? DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO.FUERO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty;
                            AltoImactos.Titulo = DelitosFiltrado.DELITO_GRUPO != null ? DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO != null ? !string.IsNullOrEmpty(DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO.DESCR) ? DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                            AltoImactos.GrupoDelito = DelitosFiltrado.DELITO_GRUPO != null ? !string.IsNullOrEmpty(DelitosFiltrado.DELITO_GRUPO.DESCR) ? DelitosFiltrado.DELITO_GRUPO.DESCR.Trim() : string.Empty : string.Empty;
                            AltoImactos.FechaInicio = TextFechaInicio.HasValue ? TextFechaInicio.Value.ToString("dd/MM/yyyy") : string.Empty;
                            AltoImactos.FechaFin = TextFechaFin.HasValue ? TextFechaFin.Value.ToString("dd/MM/yyyy") : string.Empty;
                            string FUERO = !string.IsNullOrEmpty(AltoImactos.Fuero) ? AltoImactos.Fuero.Trim() : "";
                            string TITULO = !string.IsNullOrEmpty(AltoImactos.Titulo) ? AltoImactos.Titulo.Trim() : "";
                            AltoImactos.TITULO_FUERO_DESCRTITULO = "REINGRESOS DEL FUERO " + FUERO + ",POR " + TITULO;
                            datosReporteAltoImpacto.Add(AltoImactos);
                        }
                    }
                    else
                    {//***cuando el usuario no selecciono ningun filtro de titulo y grupo
                        //****Se hace consulta a la tabla delito para obtener la descripciones del titulo y grupos
                        var DelitosFiltrado = Delito.ObtenerTodos().Where(w => w.ID_DELITO == itemDelitos.ID_DELITO).FirstOrDefault();
                        if (DelitosFiltrado != null)
                        {
                            var AltoImactos = new ReporteAltoImpacto();
                            AltoImactos.Delito = itemDelitos.DESCR_DELITO;
                            AltoImactos.Fuero = DelitosFiltrado.DELITO_GRUPO != null ? DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO != null ? DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO.FUERO != null ? !string.IsNullOrEmpty(DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO.FUERO.DESCR) ? DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO.FUERO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty;
                            AltoImactos.Titulo = DelitosFiltrado.DELITO_GRUPO != null ? DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO != null ? !string.IsNullOrEmpty(DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO.DESCR) ? DelitosFiltrado.DELITO_GRUPO.DELITO_TITULO.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                            AltoImactos.GrupoDelito = DelitosFiltrado.DELITO_GRUPO != null ? !string.IsNullOrEmpty(DelitosFiltrado.DELITO_GRUPO.DESCR) ? DelitosFiltrado.DELITO_GRUPO.DESCR.Trim() : string.Empty : string.Empty;
                            AltoImactos.FechaInicio = TextFechaInicio.HasValue ? TextFechaInicio.Value.ToString("dd/MM/yyyy") : string.Empty;
                            AltoImactos.FechaFin = TextFechaFin.HasValue ? TextFechaFin.Value.ToString("dd/MM/yyyy") : string.Empty;
                            string FUERO = !string.IsNullOrEmpty(AltoImactos.Fuero) ? AltoImactos.Fuero.Trim() : "";
                            string TITULO = !string.IsNullOrEmpty(AltoImactos.Titulo) ? AltoImactos.Titulo.Trim() : "";
                            AltoImactos.TITULO_FUERO_DESCRTITULO = "REINGRESOS DEL FUERO " + FUERO + ",POR " + TITULO;
                            datosReporteAltoImpacto.Add(AltoImactos);
                        }
                    }

                }

                //**Validacion si no existen registros
                //if (datosReporteAltoImpacto.Count == 0)
                //    Application.Current.Dispatcher.Invoke((Action)(delegate
                //    {
                //        Reporte.Clear();
                //        Reporte.LocalReport.DataSources.Clear();

                //    }));
                //else
                //{
                    #region Reporte
                    Reporte.LocalReport.ReportPath = "Reportes/rAltoImpacto.rdlc";
                    Reporte.LocalReport.DataSources.Clear();

                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = datosReporteAltoImpacto;
                    Reporte.LocalReport.DataSources.Add(rds1);

                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = datosReporte;
                    Reporte.LocalReport.DataSources.Add(rds2);


                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        Reporte.RefreshReport();
                    }));
                    #endregion
                //}
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void CargarListas()
        {
            try
            {
                ListFueros = new ObservableCollection<FUERO>(new cFuero().ObtenerTodos().OrderBy(o => o.DESCR));

                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListFueros.Insert(0, new FUERO() { ID_FUERO = "SELECCIONE", DESCR = "SELECCIONE" });

                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar buscar por nuc", ex);
            }
        }
        private async void OnLoad(ReporteAltoImpactoView Window = null)
        {
            try
            {
                ConfiguraPermisos();
                Reporte = Window.Report;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
                ValidacionesFiltros();

                //ValidarFiltros();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_ALTO_IMPACTO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

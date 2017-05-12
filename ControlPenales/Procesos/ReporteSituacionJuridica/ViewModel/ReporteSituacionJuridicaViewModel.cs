using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteSituacionJuridicaViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteSituacionJuridicaView Window)
        {
            try
            {
                ConfiguraPermisos();
                Ventana = Window;
                Reporte = Ventana.ReportSituacionJuridica;
                ReportViewerVisible = Visibility.Collapsed;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ObtenerEdificios();
                });
                ReportViewerVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public async void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "GenerarReporte":
                    if (!pConsultar)
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    Reporte.Reset();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                        }));
                        GenerarReporte();
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            ReportViewerVisible = Visibility.Visible;
                        }));

                    });
                    break;
            }
        }

        public void ObtenerEdificios()
        {
            Edificios = new List<EDIFICIO>();
            Edificios.Add(new EDIFICIO() { ID_CENTRO = GlobalVar.gCentro, ID_EDIFICIO = TODOS_LOS_EDIFICIOS, ID_TIPO_EDIFICIO = null, DESCR = "TODOS", ESTATUS = EDIFICIO_ACTIVO });
            SelectedEdificio = Edificios.FirstOrDefault();
            Edificios.AddRange(new cEdificio().ObtenerTodos(null, 0, GlobalVar.gCentro, EDIFICIO_ACTIVO).ToList());
        }

        public async void ObtenerSectores()
        {
            Sectores = new List<SECTOR>();
            Sectores.Add(new SECTOR() { ID_SECTOR = TODOS_LOS_SECTORES, DESCR = "TODOS" });
            SelectedSector = Sectores.FirstOrDefault();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                if (SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS)
                    Sectores.AddRange(new cSector().ObtenerTodos(null, null, GlobalVar.gCentro, SelectedEdificio.ID_EDIFICIO).ToList());
                else
                    Sectores.RemoveRange(1, Sectores.Count - 1);
            });
        }

        public string ObtenerAlias(INGRESO Ingreso)
        {
            var ListaAlias = Ingreso.IMPUTADO.ALIAS;
            var lAlias = new StringBuilder();
            foreach (var Alias in ListaAlias)
            {
                lAlias.Append(string.Format("{1} {2} {0} \n", !string.IsNullOrEmpty(Alias.NOMBRE) ? Alias.NOMBRE.TrimEnd() : string.Empty, !string.IsNullOrEmpty(Alias.PATERNO) ? Alias.PATERNO.TrimEnd() : string.Empty, !string.IsNullOrEmpty(Alias.MATERNO) ? Alias.MATERNO.TrimEnd() : string.Empty));
            }
            return ListaAlias.Count() > 0 ? lAlias.ToString() : string.Empty;
        }

        public string ObtenerApodos(INGRESO Ingreso)
        {
            var ListaApodos = Ingreso.IMPUTADO.APODO.Where(w => w.APODO1 != null);
            var lApodos = new StringBuilder();
            foreach (var Apodo in ListaApodos)
            {
                lApodos.Append(string.Format("{0} \n", Apodo.APODO1.TrimEnd()));
            }
            return ListaApodos.Count() > 0 ? lApodos.ToString() : string.Empty;
        }

        public string ObtenerAliasApodo(string alias,string apodo)
        {
            var resultado= new StringBuilder();
            if(!string.IsNullOrEmpty(alias))
            {
                resultado.Append("<b><i>Alias:</i></b> ");
                resultado.Append(alias);
            }
            if(!string.IsNullOrEmpty(apodo))
            {
                if (resultado.Length > 0)
                    resultado.AppendLine("<br/><b><i>Apodo:</i></b> ");
                else
                    resultado.Append("<b><i>Apodo:</i></b> ");
                resultado.Append(apodo);
            }
            return resultado.ToString();
        }

        public string ObtenerAliasApodo(INGRESO Ingreso)
        {
            bool alias = false,apodo = false;
            var resultado = new StringBuilder();
            if (Ingreso != null)
            if (Ingreso.IMPUTADO != null)
            if (Ingreso.IMPUTADO.ALIAS != null)
            {
                foreach (var a in Ingreso.IMPUTADO.ALIAS)
                {
                    if (alias)
                        resultado.Append(",\n");
                    else
                        resultado.Append("<b><i>Alias:</i></b> ");
                    resultado.Append(string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(a.NOMBRE) ? a.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(a.PATERNO) ? a.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(a.MATERNO) ? a.MATERNO.Trim() : string.Empty));
                    alias = true;
                }
            }
            if (Ingreso.IMPUTADO.APODO != null)
            {
                foreach (var a in Ingreso.IMPUTADO.APODO)
                {
                    if (!string.IsNullOrEmpty(a.APODO1))
                    {
                        if (apodo)
                            resultado.Append(",\n");
                        else
                            resultado.Append("\n<b><i>Apodo:</i></b> ");
                        resultado.Append(a.APODO1.Trim());
                        apodo = true;
                    }
                }
            }
            return resultado.ToString();
        }

        public void GenerarReporte()
        {
            try
            {

                var edificio = SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS ? SelectedEdificio.ID_EDIFICIO : (short?)0;
                var sector = SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS ? (SelectedSector.ID_SECTOR != TODOS_LOS_SECTORES ? SelectedSector.ID_SECTOR : (short?)0) : (short?)0;

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

                var lstSJ =  new List<cSituacionJuridicaIngreso>();
                var reporte = new cIngreso().ReporteSituacionJuridica(GlobalVar.gCentro);
                if (reporte != null)
                {
                    foreach (var w in reporte)
                    {
                        lstSJ.Add(new cSituacionJuridicaIngreso()
                        {
                            Id_Anio = (short)w.ID_ANIO,
                            Id_Imputado = w.ID_IMPUTADO,
                            Nombre = w.NOMBRE,
                            Paterno = w.PATERNO,
                            Materno = w.MATERNO,
                            Situacion_Juridica = w.CLASIFICACION_JURIDICA,
                            Alias = ObtenerAliasApodo(w.ALIAS, w.APODOS)
                        });
                        #region comentado
                        //cSituacionJuridicaIngreso()
                        //{
                        //    Id_Anio = s.ID_ANIO,
                        //    Id_Imputado = s.ID_IMPUTADO,
                        //    Nombre = !string.IsNullOrEmpty(s.IMPUTADO.NOMBRE) ? s.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                        //    Paterno = !string.IsNullOrEmpty(s.IMPUTADO.PATERNO) ? s.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                        //    Materno = !string.IsNullOrEmpty(s.IMPUTADO.MATERNO) ? s.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                        //    Situacion_Juridica = s.CLASIFICACION_JURIDICA != null ? s.CLASIFICACION_JURIDICA.DESCR.TrimEnd() : string.Empty,
                        //    Alias = ObtenerAliasApodo(s)
                        //}
                        #endregion
                    }
                }

                List<CELDA> lEstancia = new cCelda().ObtenerCeldas(
                        GlobalVar.gCentro,
                        edificio,
                        sector).
                        AsEnumerable().
                        Where(w =>
                        w.CAMA.Count > 0).
                        ToList();
                    #region comentado
                //List<cSituacionJuridicaIngreso> lSituacionJuridica = new cIngreso().
                //    ObtenerIngresosEstancias(lEstancia).
                //    AsEnumerable().
                //    Select(s => new cSituacionJuridicaIngreso()
                //    {
                //        Id_Anio = s.ID_ANIO,
                //        Id_Imputado = s.ID_IMPUTADO,
                //        Nombre = !string.IsNullOrEmpty(s.IMPUTADO.NOMBRE) ? s.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                //        Paterno = !string.IsNullOrEmpty(s.IMPUTADO.PATERNO) ? s.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                //        Materno = !string.IsNullOrEmpty(s.IMPUTADO.MATERNO) ? s.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                //        Situacion_Juridica = s.CLASIFICACION_JURIDICA != null ? s.CLASIFICACION_JURIDICA.DESCR.TrimEnd() : string.Empty,
                //        Alias = ObtenerAliasApodo(s), 
                //        //Alias = string.Format("{0}\n", ObtenerAlias(s)),
                //        //Apodos = string.Format("{0}\n", ObtenerApodos(s)),
                //        //TieneAlias = !string.IsNullOrEmpty(ObtenerAlias(s)),
                //        //TieneApodos = !string.IsNullOrEmpty(ObtenerApodos(s))

                //    }).
                //    ToList();
#endregion
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.LocalReport.ReportPath = "Reportes/rSituacionJuridica.rdlc";
                    Reporte.LocalReport.DataSources.Clear();

                    ReportDataSource ReportDataSource = new ReportDataSource();
                    ReportDataSource.Name = "DataSet1";
                    ReportDataSource.Value = lstSJ;

                    ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                    ReportDataSource_Encabezado.Name = "DataSet2";
                    ReportDataSource_Encabezado.Value = datosReporte;

                    Reporte.LocalReport.DataSources.Add(ReportDataSource);
                    Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);

                    Reporte.Refresh();
                    Reporte.RefreshReport();

                }));


            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_SITUACION_JURIDICA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

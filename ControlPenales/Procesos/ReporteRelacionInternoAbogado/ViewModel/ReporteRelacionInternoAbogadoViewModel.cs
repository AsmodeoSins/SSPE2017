using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Reporting.WinForms;
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
    public partial class ReporteRelacionInternoAbogadoViewModel : ValidationViewModelBase
    {

        private short?[] estatus_inactivo=null;
        public async void OnLoad(ReporteRelacionInternoAbogadoView Window)
        {
            try
            {
                ConfiguraPermisos();
                Ventana = Window;
                CrearNuevoExpedienteEnabled = false;
                estatus_inactivo = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));

                    Reporte = Ventana.Report;
                    TextBotonSeleccionarIngreso = "Generar Reporte";
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Visible;

                    }));
                });

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
                case "BuscarIngresos":
                    if (!pConsultar)
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        }));
                    });
                    break;
                case "buscar_seleccionar":
                    if (SelectExpediente == null)
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar un expediente.");
                    }
                    else
                    {
                        if (SelectIngreso==null)
                        {
                            var met = Application.Current.Windows[0] as MetroWindow;
                            await met.ShowMessageAsync("Validación", "Debe seleccionar un ingreso.");
                            return;
                        }
                        if (estatus_inactivo!=null && estatus_inactivo.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO))
                        {
                            var met = Application.Current.Windows[0] as MetroWindow;
                            await met.ShowMessageAsync("Validación", "El ingreso no se encuentra activo en su centro.");
                            return;
                        }
                        ReportViewerVisible = Visibility.Collapsed;
                        Reporte.Reset();
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                GenerarReporte();
                            }));

                        });
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        ReportViewerVisible = Visibility.Visible;
                    }
                    break;
                case "nueva_busqueda":
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    AnioBuscar = null;
                    FolioBuscar = null;
                    NombreBuscar = string.Empty;
                    ApellidoPaternoBuscar = string.Empty;
                    ApellidoMaternoBuscar = string.Empty;
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                    break;
                case "buscar_salir":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    ReportViewerVisible = Visibility.Visible;
                    break;
            }
        }

        private void ClickBuscarInterno(object parametro)
        {
            buscarImputadoInterno(parametro);
        }

        private async void buscarImputadoInterno(Object obj = null)
        {
            ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
            //ListExpediente = new List<IMPUTADO>();
            ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
            ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
            //ListExpediente = await SegmentarResultadoBusqueda();

            //if (ListExpediente != null)
            //{
            //    EmptyExpedienteVisible = ListExpediente.Count < 0;
            //    var ListExpedienteAux = new List<IMPUTADO>(ListExpediente);
            //    foreach (var Expediente in ListExpediente)
            //    {
            //        var ingreso_activo = Expediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
            //        if (ingreso_activo != null)
            //        {
            //            if (ingreso_activo.ID_ESTATUS_ADMINISTRATIVO != LIBERADO &&
            //            ingreso_activo.ID_ESTATUS_ADMINISTRATIVO != TRASLADADO &&
            //            ingreso_activo.ID_ESTATUS_ADMINISTRATIVO != SUJETO_A_PROCESO_EN_LIBERTAD &&
            //            ingreso_activo.ID_ESTATUS_ADMINISTRATIVO != DISCRECIONAL)
            //            {
            //                if (!ingreso_activo.ABOGADO_INGRESO.Any())
            //                {
            //                    ListExpedienteAux.Remove(Expediente);
            //                }
            //            }
            //            else
            //            {
            //                ListExpedienteAux.Remove(Expediente);
            //            }
            //        }
            //        else
            //        {
            //            ListExpedienteAux.Remove(Expediente);
            //        }


            //    }
            //    ListExpediente = ListExpedienteAux;
            //}
            //else
            //    EmptyExpedienteVisible = true;
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                return new List<IMPUTADO>();

            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
            if (result.Any())
            {
                Pagina++;
                SeguirCargando = true;
            }
            else
                SeguirCargando = false;

            return result.ToList();
        }

        #region NombreCompletoAbogado
        public string ObtenerPaternoAbogado(ABOGADO_INGRESO abogado_ingreso)
        {
            var paterno = abogado_ingreso.ABOGADO.PERSONA.PATERNO;

            return !string.IsNullOrEmpty(paterno) ? paterno.TrimEnd() : string.Empty;
        }

        public string ObtenerMaternoAbogado(ABOGADO_INGRESO abogado_ingreso)
        {
            var materno = abogado_ingreso.ABOGADO.PERSONA.MATERNO;

            return !string.IsNullOrEmpty(materno) ? materno.TrimEnd() : string.Empty;
        }

        public string ObtenerNombreAbogado(ABOGADO_INGRESO abogado_ingreso)
        {
            var nombre = abogado_ingreso.ABOGADO.PERSONA.NOMBRE;

            return !string.IsNullOrEmpty(nombre) ? nombre.TrimEnd() : string.Empty;
        }
        #endregion

        #region NombreCompletoInterno
        public string ObtenerPaterno(ABOGADO_INGRESO abogado_ingreso)
        {
            var paterno = abogado_ingreso.INGRESO.IMPUTADO.PATERNO;

            return !string.IsNullOrEmpty(paterno) ? paterno.TrimEnd() : string.Empty;
        }

        public string ObtenerMaterno(ABOGADO_INGRESO abogado_ingreso)
        {
            var materno = abogado_ingreso.INGRESO.IMPUTADO.MATERNO;

            return !string.IsNullOrEmpty(materno) ? materno.TrimEnd() : string.Empty;
        }

        public string ObtenerNombre(ABOGADO_INGRESO abogado_ingreso)
        {
            var nombre = abogado_ingreso.INGRESO.IMPUTADO.NOMBRE;

            return !string.IsNullOrEmpty(nombre) ? nombre.TrimEnd() : string.Empty;
        }
        #endregion

        public void GenerarReporte()
        {
            var datosReporte = new List<cReporteDatos>();
            var lRelaciones = new List<cInternoAbogado>();

            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
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


                lRelaciones = new cAbogadoIngreso().
                    ObtenerAbogadosAsignados(
                    GlobalVar.gCentro,
                    SelectExpediente.ID_ANIO,
                    SelectExpediente.ID_IMPUTADO,
                    SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO).
                    AsEnumerable().
                    Select(s => new cInternoAbogado()
                    {
                        Anio = s.ID_ANIO,
                        Estatus = s.ESTATUS_VISITA.DESCR,
                        esTitular = s.ID_ABOGADO_TITULO == "T",
                        Id_Abogado = s.ID_ABOGADO,
                        Id_Imputado = s.ID_IMPUTADO,
                        Materno = ObtenerMaterno(s),
                        Materno_Abogado = ObtenerMaternoAbogado(s),
                        Nombre = ObtenerNombre(s),
                        Nombre_Abogado = ObtenerNombreAbogado(s),
                        Observaciones = !string.IsNullOrEmpty(s.OBSERV) ? s.OBSERV.TrimEnd() : "SIN OBSERVACIONES",
                        Paterno = ObtenerPaterno(s),
                        Paterno_Abogado = ObtenerPaternoAbogado(s)
                    }).ToList();


                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.LocalReport.ReportPath = "Reportes/rVisitaLegal.rdlc";
                    Reporte.LocalReport.DataSources.Clear();

                    ReportDataSource ReportDataSource = new ReportDataSource();
                    ReportDataSource.Name = "DataSet1";
                    ReportDataSource.Value = lRelaciones;

                    ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                    ReportDataSource_Encabezado.Name = "DataSet2";
                    ReportDataSource_Encabezado.Value = datosReporte;

                    Reporte.LocalReport.DataSources.Add(ReportDataSource);
                    Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);

                    //Reporte.LocalReport.SetParameters(new ReportParameter("Fecha", Fechas.GetFechaDateServer.ToString()));
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;

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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_RELACION_INTERNO_ABOGADO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

        #region Cambio SelectedItem de Busqueda de Expediente
        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_expediente":
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            if (selectExpediente.INGRESO != null && selectExpediente.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = selectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            }
                            else
                            {
                                EmptyIngresoVisible = true;
                                SelectIngreso = null;
                            }
                        }
                        break;
                }
            }
        }
        #endregion
    }
}

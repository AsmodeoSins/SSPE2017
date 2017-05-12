using System;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Controlador;
using SSP.Modelo;
using SSP.Servidor;
using System.Windows.Input;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using SSP.Controlador.Catalogo.Justicia;
namespace ControlPenales
{
    partial class PlanimetriaViewModel : ValidationViewModelBase
    {
        #region Constructor
        public PlanimetriaViewModel(){}
        #endregion
        
        #region Metodos
        private void SeleccionaSector(object tView)
        {
            try
            {
                var treeView = (System.Windows.Controls.TreeView)tView;
                if (treeView.SelectedItem.GetType().BaseType.Name.Equals("SECTOR"))
                    if (treeView.SelectedItem != null)
                    {
                        SelectedSector = (SECTOR)treeView.SelectedItem;
                        EdificioSectorHeader = string.Format("{0} - {1}", SelectedSector.EDIFICIO.DESCR.Trim() ,SelectedSector.DESCR.Trim());
                        PopulateObservaciones();
                        //ConstruirListaCeldasPopUp();
                        ObtenerCamas();

                    }
            }
            catch (Exception ex) {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar el sector", ex);
            }
        }
        
        private async void btnClick(object param)
        {
            switch ((string)param)
            {
                case "addObservacion":
                    LimpiarCampos();
                    if (SelectedSector != null)
                    {
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        //ConstruirListaCeldasPopUp();
                        ObtenerCamas();
                        setValidacionesPlanimetria();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CELDAS_OBSERVACION);
                    }
                    else
                        new Dialogos().NotificacionDialog("NOTIFICACIÓN", "Favor de seleccionar un sector.");
                    break;
                case "rollbackObservacion":
                    LimpiarCampos();
                    //ConstruirListaCeldasPopUp();
                    ObtenerCamas();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CELDAS_OBSERVACION);
                    break;
                case "commitObservacion":
                    setValidacionesPlanimetria();
                    if (!PInsertar && !PEditar)//No tiene privilegios de insertar ni editar
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    if (!base.HasErrors)
                    {
                        popUpCeldaObs.OBSERVACION = popUpObservacion;
                        
                        GuardarObservaciones();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CELDAS_OBSERVACION);
                        LimpiarCampos();
                        //ConstruirListaCeldasPopUp();
                        ObtenerCamas();
                    }
                    break;
                case "editObservacion":
                    if (!PEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    popUpCeldaObs = SelectedObservacion;
                    OnPropertyChanged("popUpCeldaObs");
                    popUpObservacion = SelectedObservacion.OBSERVACION;
                    IdSectorClasificacion = SelectedObservacion.ID_SECTOR_CLAS != null ? SelectedObservacion.ID_SECTOR_CLAS : -1;
                    SelectedSectorClasificacion = LstSectorClasificacion.Where(w => w.ID_SECTOR_CLAS == IdSectorClasificacion).FirstOrDefault();
                    //PopularEditListaCeldasPopUp();
                    //ConstruirListaCeldasPopUp();
                    ObtenerCamas();
                    setValidacionesPlanimetria();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CELDAS_OBSERVACION);
                    break;
                case "delObservacion":
                    EliminarObservacion((await ConfirmarEliminarObservacion()));
                    LimpiarCampos();
                    RePopularCeldas();
                    PopulateObservaciones();
                    //ConstruirListaCeldasPopUp();
                    ObtenerCamas();
                    break;
                case "reporte_menu"://"imprimirR1"
                    if (!PImprimir)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    ImprimirReporte();
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new PlanimetriaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.PlanimetriaViewModel();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                default:
                    break;
            }
        }
        
        private async Task<bool> ConfirmarEliminarObservacion()
        {
            var metro = Application.Current.Windows[0] as MetroWindow;
            MessageDialogResult result = MessageDialogResult.Negative;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Eliminar",
                NegativeButtonText = "No Eliminar",
                AnimateShow = true,
                AnimateHide = true
            };
            if (SelectedObservacion != null)
            {
                if (SelectedObservacion.SECTOR_OBSERVACION_CELDA != null)
                    if (SelectedObservacion.SECTOR_OBSERVACION_CELDA.Count != 0)
                    {
                        result = await metro.ShowMessageAsync("Observaciones de celdas", "¿ Esta observación contiene celdas en ella, desea continuar con la eliminación ?", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        return result == MessageDialogResult.Negative ? false : true;
                    }
                    else
                    {
                        result = await metro.ShowMessageAsync("Observaciones de celdas", "¿ Desea eliminar la observación ?", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        return result == MessageDialogResult.Negative ? false : true;
                    }
                else
                {
                    result = await metro.ShowMessageAsync("Observaciones de celdas", "¿ Desea eliminar la observación ?", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                    return result == MessageDialogResult.Negative ? false : true;
                }
            }
            else
            {
                StaticSourcesViewModel.Mensaje("Observaciones de celdas", "Debe seleccionar una observación primero.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                return false;
            }
        }
        
        private void EliminarObservacion(bool continuar)
        {
            try
            {
                if (continuar)
                {
                    if (SelectedObservacion != null)
                    {
                        if (new cObservacionPlanimetria().Eliminar(SelectedObservacion.ID_SECTOR_OBS))
                        {
                            PopulateObservaciones();
                            StaticSourcesViewModel.Mensaje("Observaciones de celdas", "Observación eliminada correctamente.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        }
                    }
                    else
                        StaticSourcesViewModel.Mensaje("Observaciones de celdas", "Observación no se pudo eliminar.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar", ex);
            }
         
        }
        
        private void ConstruirListaCeldasPopUp()
        {
            //if (SelectedSector != null)
            //{
            //    if (SelectedObservacion == null)//AGREGAR
            //    {
            //        LstCeldaX = new ObservableCollection<CeldaX>();
            //     foreach (var c in SelectedSector.CELDA)
            //     {
            //             LstCeldaX.Add(
            //                 new CeldaX
            //                 {
            //                     Celda = c,
            //                     Seleccionado = false
            //                 }
            //                 );
            //     }
            //    }
            //    else//EDITAR
            //    {
            //        LstCeldaX = new ObservableCollection<CeldaX>();
            //        foreach (var c in SelectedSector.CELDA)
            //        {
            //            LstCeldaX.Add(
            //                new CeldaX
            //                {
            //                    Celda = c,
            //                    Seleccionado = SelectedObservacion.SECTOR_OBSERVACION_CELDA.Where(w => w.ID_CENTRO == c.ID_CENTRO && w.ID_EDIFICIO == c.ID_EDIFICIO && w.ID_SECTOR == c.ID_SECTOR && w.ID_CELDA == c.ID_CELDA).Count() > 0 ? true : false
            //                });
            //        }

            //    }
            //}
        }
       
        private void PopulateObservaciones()
        {
            try
            {
                LstObservacionesXSector = new ObservableCollection<SECTOR_OBSERVACION>(new SSP.Controlador.Catalogo.Justicia.cSector().Obtener(SelectedSector.ID_SECTOR, SelectedSector.ID_EDIFICIO, SelectedSector.ID_CENTRO).SECTOR_OBSERVACION.OrderBy(w => w.ID_SECTOR_CLAS));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar observaciones", ex);
            }

        }
        
        private void LimpiarCampos()
        {
            try
            {
                SelectedObservacion = null;
                popUpCeldaObs = new SECTOR_OBSERVACION();
                popUpObservacion = string.Empty;
                IdSectorClasificacion = -1;
                SelectedSectorClasificacion = LstSectorClasificacion.Where(w => w.ID_SECTOR_CLAS == IdSectorClasificacion).FirstOrDefault();
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar campos", ex);
            }

        }

        private void PopularCentros()
        {
            if (LstCentros == null)
                LstCentros = new ObservableCollection<CENTRO>(new SSP.Controlador.Catalogo.Justicia.cCentro().Obtener(4));
        }
        
        private void RePopularCeldas()
        {
            lstCeldas = new ObservableCollection<CELDA>(SelectedSector.CELDA);
        }
        
        #endregion
        
        #region Planimetria
        private void GuardarObservaciones()
        {
            try
            {
                if (SelectedSector != null)
                {
                    var obj = new SECTOR_OBSERVACION();
                    obj.ID_CENTRO = SelectedSector.ID_CENTRO;
                    obj.ID_EDIFICIO = SelectedSector.ID_EDIFICIO;
                    obj.ID_SECTOR = SelectedSector.ID_SECTOR;
                    obj.OBSERVACION = popUpObservacion;
                    obj.ID_SECTOR_CLAS = IdSectorClasificacion;

                    if (SelectedObservacion == null)//AGREGAR
                    {
                        obj.ID_SECTOR_OBS = new cObservacionPlanimetria().Agregar(obj);
                        if (obj.ID_SECTOR_OBS > 0)
                        {
                            if (GuardarCeldas(obj.ID_SECTOR_OBS))
                            {
                                PopulateObservaciones();
                                StaticSourcesViewModel.Mensaje("Observaciones de celdas", "Informaci\u00F3n registrada correctamente.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                            }
                            else
                                StaticSourcesViewModel.Mensaje("Observaciones de celdas", "Ocurrió un error al guardar la informacion", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Observaciones de celdas", "Ocurrió un error al guardar la informacion", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);

                    }
                    else//ACTUALIZAR
                    {
                        obj.ID_SECTOR_OBS = SelectedObservacion.ID_SECTOR_OBS;
                        if (new cObservacionPlanimetria().Actualizar(obj))
                        {
                            if (GuardarCeldas(obj.ID_SECTOR_OBS))
                            {
                                PopulateObservaciones();
                                StaticSourcesViewModel.Mensaje("Observaciones de celdas", "Informaci\u00F3n registrada correctamente.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                            }
                            else
                                StaticSourcesViewModel.Mensaje("Observaciones de celdas", "Ocurrió un error al guardar la informacion", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Observaciones de celdas", "Ocurrió un error al guardar la informacion", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                    }
                }
                else
                    new Dialogos().NotificacionDialog("Notificacion", "Favor de seleccionar un sector");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
            }

        }

        private bool GuardarCeldas(int Id)
        {
            try
            {
                var cel = new List<SECTOR_OBSERVACION_CELDA>();
                if (LstCeldaX != null)
                {
                    var lista = LstCeldaX.Where(w => w.Seleccionado == true);
                    if (lista != null)
                    {
                        foreach (var c in lista)
                        {
                            cel.Add(
                                new SECTOR_OBSERVACION_CELDA()
                                {
                                    ID_CENTRO = /*c.Celda.ID_CENTRO*/ c.Cama.ID_CENTRO,
                                    ID_EDIFICIO = /*c.Celda.ID_EDIFICIO*/  c.Cama.ID_EDIFICIO,
                                    ID_SECTOR = /*c.Celda.ID_SECTOR*/ c.Cama.ID_SECTOR,
                                    ID_CAMA = c.Cama.ID_CAMA,
                                    ID_CELDA = /*c.Celda.ID_CELDA*/c.Cama.ID_CELDA.ToString(),
                                    ID_SECTOR_OBS = Id
                                });
                        }
                    }
                }

                if (new cSectorClasificacionCelda().Insertar(Id, cel))
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
            }
            return false;
        }
        #endregion

        #region Camas
        private void ObtenerCamas()
        {
            LstCeldaX = new ObservableCollection<CeldaX>();

            try
            {
                if (SelectedSector != null)
                {
                    SelectedSector = new cSector().Obtener(SelectedSector.ID_SECTOR, SelectedSector.ID_EDIFICIO, SelectedSector.ID_CENTRO);
                    foreach (var celda in SelectedSector.CELDA.OrderBy(w => w.ID_CELDA))//celdas por sector
                    {
                        foreach (var cama in celda.CAMA.OrderBy(x => x.ID_CAMA))//camas por celda
                        {
                            if (SelectedObservacion == null)
                            {
                                if (cama.SECTOR_OBSERVACION_CELDA != null)//ya fue registrada antes
                                    continue;

                                else
                                {//Aun no ha sido registrada en observaciones celda. se crea para que se seleccione desde la vista
                                    LstCeldaX.Add(
                                       new CeldaX
                                       {
                                           Cama = cama,
                                           Seleccionado = false
                                       });
                                };
                            }

                            else
                            {
                                if (SelectedObservacion.SECTOR_OBSERVACION_CELDA.AsQueryable().Any() && SelectedObservacion.SECTOR_OBSERVACION_CELDA.AsQueryable().Any(d => d.ID_CAMA == cama.ID_CAMA))
                                {
                                    LstCeldaX.Add(//Se acaba de dar de alta
                                     new CeldaX
                                     {
                                         Cama = cama,
                                         Seleccionado = true
                                     });

                                    continue;
                                }

                                if (cama.SECTOR_OBSERVACION_CELDA != null)
                                {
                                    if (SelectedObservacion.SECTOR_OBSERVACION_CELDA.AsQueryable().Where(x => x.ID_SECTOR_OBS == SelectedObservacion.ID_SECTOR_OBS && x.ID_CAMA == cama.ID_CAMA).Count() > 0)
                                        LstCeldaX.Add(//Ya fue registrada , se regresa marcada para que se vuelva a crear y se borre solo si el usuario la desmarca
                                             new CeldaX
                                                 {
                                                     Cama = cama,
                                                     Seleccionado = true
                                                 });
                                }

                                else
                                {//Aun no ha sido registrada en observaciones celda. se crea para que se seleccione desde la vista
                                    LstCeldaX.Add(
                                       new CeldaX
                                       {
                                           Cama = cama,
                                           Seleccionado = false
                                       });
                                };
                            };
                        };
                    };
                };
            }

            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener las camas", ex);
            }
        }
        #endregion

        //PLANIMETRIA REPORTES
        private void ImprimirReporte() 
        {
            try
            {
                if (SelectedSector != null)
                {
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                    var reporteDatos = new List<cReporteDatos>();
                    reporteDatos.Add(new cReporteDatos()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1,
                        Encabezado2 = Parametro.ENCABEZADO2,
                        Titulo = "PLANIMETRIA",
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Centro = centro.DESCR.Trim()
                    });

                    var reporte = new List<cImputadoPlanimetria>();
                    var internos = new cIngreso().ObtenerIngresosPorSector(SelectedSector.ID_CENTRO, SelectedSector.ID_EDIFICIO, SelectedSector.ID_SECTOR);
                    if (internos != null)
                    {
                        foreach (var i in internos)
                        {
                            string paterno = i.IMPUTADO.PATERNO;
                            if (!string.IsNullOrEmpty(paterno))
                                paterno = paterno.Trim();

                            string materno = i.IMPUTADO.MATERNO;
                            if (!string.IsNullOrEmpty(materno))
                                materno = materno.Trim();

                            string nombre = i.IMPUTADO.NOMBRE;
                            if (!string.IsNullOrEmpty(nombre))
                                nombre = nombre.Trim();
                            
                            var clasificacion = string.Empty;
                            if (i.CAMA != null)
                            {
                                if (i.CAMA.SECTOR_OBSERVACION_CELDA != null)
                                {
                                    if (i.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null)
                                    {
                                        if (i.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null)
                                        {
                                            clasificacion = i.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION;
                                        }
                                    }
                                }
                            }
                            reporte.Add(new cImputadoPlanimetria()
                            { ///TODO: cambios en el modelo
                                //Clasificacion = i.CAMA.CELDA.SECTOR_OBSERVACION_CELDA.FirstOrDefault().SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION,
                                Clasificacion = clasificacion,
                                Centro = i.CENTRO.DESCR,
                                Anio = i.ID_ANIO.ToString(),
                                Imputado = i.ID_IMPUTADO.ToString(),
                                APaterno = string.Empty,
                                AMaterno = string.Empty,
                                Nombre = string.Format("{0} {1} {2}", paterno, materno, nombre),
                                Estancia = i.CAMA.CELDA.ID_CELDA,
                                Cama = i.CAMA.ID_CAMA.ToString()
                            });
                        }

                        var view = new ReportesView();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        view.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        view.Owner = PopUpsViewModels.MainWindow;
                        view.Show();

                        //ARMAMOS EL REPORTE
                        view.Report.LocalReport.ReportPath = "Reportes/rPlanimetria.rdlc";
                        view.Report.LocalReport.DataSources.Clear();

                        var generales = new List<cGeneralesPlanimetria>();
                        generales.Add(new cGeneralesPlanimetria() { Centro = "CERESO DE MEXICALI", Ubicacion = SelectedSector.DESCR });

                        Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds1.Name = "DataSet1";
                        rds1.Value = generales;
                        view.Report.LocalReport.DataSources.Add(rds1);

                        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds2.Name = "DataSet2";
                        rds2.Value = reporte;
                        view.Report.LocalReport.DataSources.Add(rds2);

                        Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds3.Name = "DataSet3";
                        rds3.Value = reporteDatos;
                        view.Report.LocalReport.DataSources.Add(rds3);

                        view.Report.RefreshReport();
                    }
                }
                else
                    new Dialogos().ConfirmacionDialogo("Notificacion", "Favor de seleccionar un sector");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }
        //LOAD
        private void PlanimetriaLoad(PlanimetriaView Window = null)
        {
            ImagenHeight = Window.ActualHeight / 1.1;

            IdSectorClasificacion = -1;
            ConfiguraPermisos();
            PopularCentros();
            PopulateClasificacionSector();
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.PLANIMETRIA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            AgregarBotonEnabled = PInsertar = true;
                        if (p.EDITAR == 1)
                            PEditar = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = true;
                        if (p.IMPRIMIR == 1)
                        { 
                            ReporteEnabled = PImprimir = true;
                            MenuReporteEnabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
    
    public class CeldaX : CELDA
    {
        public bool Seleccionado { get; set; }
        //public CELDA Celda { get; set; }
        public CAMA Cama { get; set; }
    }

}

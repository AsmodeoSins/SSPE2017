using ControlPenales.BiometricoServiceReference;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
namespace ControlPenales
{
    public partial class ReporteBrazaleteGafeteViewModel : ValidationViewModelBase
    {
        #region Métodos Eventos
        public async void OnLoad(ReporteBrazaleteGafete Window)
        {
            try
            {
                BrazaleteReportViewerVisible = Visibility.Collapsed;
                GafeteReportViewerVisible = Visibility.Collapsed;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    BusquedaAvanzadaChecked = false;
                    Ventana = Window;
                    Reporte = Ventana.ReportBrazalete;
                    ListaIngresos = new List<cInternoGafeteBrazalete>();
                    ListaIngresosSeleccionados = new List<cInternoGafeteBrazalete>();
                    ObtenerEdificios();
                    EmptyVisible = EmptySeleccionadosVisible = true;
                    var placeholder = new Imagenes().getImagenPerson();
                    FotoIngreso = placeholder;
                    FotoCentro = placeholder;
                    TextoGenerarReporte = "Generar Brazalete(s)";
                    Pagina = 1;
                });
                BrazaleteSelected = true;
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
                    if (!ListaIngresosSeleccionados.Any())
                    {
                        var mensaje = System.Windows.Application.Current.Windows[0] as MetroWindow;
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Cerrar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        BrazaleteReportViewerVisible = Visibility.Collapsed;
                        GafeteReportViewerVisible = Visibility.Collapsed;
                        await mensaje.ShowMessageAsync("Validación", "Debe seleccionar al menos un interno para generar el reporte.", MessageDialogStyle.Affirmative, mySettings);
                        BrazaleteReportViewerVisible = BrazaleteSelected ? Visibility.Visible : Visibility.Collapsed;
                        GafeteReportViewerVisible = GafeteSelected ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        Reporte.Reset();
                        if (BrazaleteSelected)
                        {
                            BrazaleteReportViewerVisible = Visibility.Collapsed;
                            GenerarReporte(enumTipoReporte.BRAZALETE);
                            BrazaleteReportViewerVisible = Visibility.Visible;
                        }
                        else
                        {
                            GafeteReportViewerVisible = Visibility.Collapsed;
                            GenerarReporte(enumTipoReporte.GAFETE);
                            GafeteReportViewerVisible = Visibility.Visible;
                        }
                    }
                    break;
                case "ObtenerIngresos":
                    try
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            BrazaleteReportViewerVisible = Visibility.Collapsed;
                            GafeteReportViewerVisible = Visibility.Collapsed;
                        }));
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            Pagina = 1;
                            ListaIngresos = new List<cInternoGafeteBrazalete>();
                            EmptyVisible = true;
                            ObtenerIngresos();
                        });
                        System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            BrazaleteReportViewerVisible = BrazaleteSelected ? Visibility.Visible : Visibility.Collapsed;
                            GafeteReportViewerVisible = GafeteSelected ? Visibility.Visible : Visibility.Collapsed;
                        }));
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }
                    break;
                case "Permitir":
                    if (!SelectedIngreso.SELECCIONADO)
                        SeleccionarTodosIngresos = false;
                    else
                        SeleccionarTodosIngresos = !ListaIngresos.Where(w => !w.SELECCIONADO).Any();
                    break;
                case "SeleccionarTodosIngresos":
                    if (ListaIngresos.Any())
                    {
                        var lista_ingresos = new List<cInternoGafeteBrazalete>(ListaIngresos);
                        foreach (var Ingreso in lista_ingresos)
                        {
                            Ingreso.SELECCIONADO = SeleccionarTodosIngresos;
                        }
                        ListaIngresos = lista_ingresos;
                    }
                    else
                    {
                        SeleccionarTodosIngresos = false;
                    }
                    break;
                case "PermitirSeleccionado":
                    if (!SelectedIngresoSeleccionado.SELECCIONADO)
                        SeleccionarTodosIngresosSeleccionados = false;
                    else
                        SeleccionarTodosIngresosSeleccionados = !ListaIngresosSeleccionados.Where(w => !w.SELECCIONADO).Any();
                    break;
                case "SeleccionarTodosIngresosSeleccionados":
                    if (ListaIngresosSeleccionados.Any())
                    {
                        var lista_ingresos_seleccionados = new List<cInternoGafeteBrazalete>(ListaIngresosSeleccionados);
                        foreach (var Ingreso in lista_ingresos_seleccionados)
                        {
                            Ingreso.SELECCIONADO = SeleccionarTodosIngresosSeleccionados;
                        }
                        ListaIngresosSeleccionados = lista_ingresos_seleccionados;
                    }
                    else
                    {
                        SeleccionarTodosIngresosSeleccionados = false;
                    }
                    break;
                case "AgregarInternos":
                    if (ListaIngresos.Any())
                    {
                        if (ListaIngresos.Where(w => w.SELECCIONADO).Any())
                        {
                            var lista_ingresos = new List<cInternoGafeteBrazalete>(ListaIngresos);
                            var lista_ingresos_seleccionados = new List<cInternoGafeteBrazalete>(
                                ListaIngresos.Where(w => w.SELECCIONADO).
                                OrderByDescending(oD => oD.ID_ANIO).
                                ThenByDescending(tD => tD.ID_IMPUTADO).
                                ToList());
                            foreach (var ingreso_seleccionado in lista_ingresos_seleccionados)
                            {
                                ingreso_seleccionado.SELECCIONADO = false;
                                lista_ingresos.Remove(ingreso_seleccionado);
                            }
                            ListaIngresos = lista_ingresos.Any() ? lista_ingresos.OrderByDescending(oD => oD.ID_ANIO).ThenByDescending(tD => tD.ID_IMPUTADO).ToList() : new List<cInternoGafeteBrazalete>();
                            EmptyVisible = !ListaIngresos.Any();
                            EmptySeleccionadosVisible = false;
                            ListaIngresosSeleccionados.AddRange(lista_ingresos_seleccionados);
                            ListaIngresosSeleccionados = ListaIngresosSeleccionados.OrderByDescending(oD => oD.ID_ANIO).ThenByDescending(tD => tD.ID_IMPUTADO).ToList();
                            SeleccionarTodosIngresos = false;
                        }
                        else
                        {
                            var mensaje = System.Windows.Application.Current.Windows[0] as MetroWindow;
                            var mySettings = new MetroDialogSettings()
                            {
                                AffirmativeButtonText = "Cerrar",
                                AnimateShow = true,
                                AnimateHide = false
                            };
                            BrazaleteReportViewerVisible = Visibility.Collapsed;
                            GafeteReportViewerVisible = Visibility.Collapsed;
                            await mensaje.ShowMessageAsync("Validación", "Debe seleccionar al menos un interno.", MessageDialogStyle.Affirmative, mySettings);
                            BrazaleteReportViewerVisible = BrazaleteSelected ? Visibility.Visible : Visibility.Collapsed;
                            GafeteReportViewerVisible = GafeteSelected ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        var mensaje = System.Windows.Application.Current.Windows[0] as MetroWindow;
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Cerrar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        BrazaleteReportViewerVisible = Visibility.Collapsed;
                        GafeteReportViewerVisible = Visibility.Collapsed;
                        await mensaje.ShowMessageAsync("Validación", "Lista de internos vacía. Realice una búsqueda e intente de nuevo.", MessageDialogStyle.Affirmative, mySettings);
                        BrazaleteReportViewerVisible = BrazaleteSelected ? Visibility.Visible : Visibility.Collapsed;
                        GafeteReportViewerVisible = GafeteSelected ? Visibility.Visible : Visibility.Collapsed;
                    }
                    break;
                case "RemoverInternos":
                    if (ListaIngresosSeleccionados.Any())
                    {
                        if (ListaIngresosSeleccionados.Where(w => w.SELECCIONADO).Any())
                        {
                            var lista_ingresos = new List<cInternoGafeteBrazalete>(ListaIngresosSeleccionados);
                            var lista_ingresos_seleccionados = new List<cInternoGafeteBrazalete>(
                                ListaIngresosSeleccionados.Where(w => w.SELECCIONADO).
                                OrderByDescending(oD => oD.ID_ANIO).
                                ThenByDescending(tD => tD.ID_IMPUTADO).
                                ToList());
                            foreach (var ingreso_seleccionado in lista_ingresos_seleccionados)
                            {
                                ingreso_seleccionado.SELECCIONADO = false;
                                lista_ingresos.Remove(ingreso_seleccionado);
                            }
                            ListaIngresosSeleccionados = lista_ingresos.Any() ? lista_ingresos.OrderByDescending(oD => oD.ID_ANIO).ThenByDescending(tD => tD.ID_IMPUTADO).ToList() : new List<cInternoGafeteBrazalete>();
                            EmptySeleccionadosVisible = !ListaIngresosSeleccionados.Any();
                            EmptyVisible = false;
                            ListaIngresos.AddRange(lista_ingresos_seleccionados);
                            ListaIngresos = ListaIngresos.OrderByDescending(oD => oD.ID_ANIO).ThenByDescending(tD => tD.ID_IMPUTADO).ToList();
                            SeleccionarTodosIngresosSeleccionados = false;
                        }
                        else
                        {
                            var mensaje = System.Windows.Application.Current.Windows[0] as MetroWindow;
                            var mySettings = new MetroDialogSettings()
                            {
                                AffirmativeButtonText = "Cerrar",
                                AnimateShow = true,
                                AnimateHide = false
                            };
                            BrazaleteReportViewerVisible = Visibility.Collapsed;
                            GafeteReportViewerVisible = Visibility.Collapsed;
                            await mensaje.ShowMessageAsync("Validación", "Debe seleccionar al menos un interno.", MessageDialogStyle.Affirmative, mySettings);
                            BrazaleteReportViewerVisible = BrazaleteSelected ? Visibility.Visible : Visibility.Collapsed;
                            GafeteReportViewerVisible = GafeteSelected ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        var mensaje = System.Windows.Application.Current.Windows[0] as MetroWindow;
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Cerrar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        BrazaleteReportViewerVisible = Visibility.Collapsed;
                        GafeteReportViewerVisible = Visibility.Collapsed;
                        await mensaje.ShowMessageAsync("Validación", "Lista de internos seleccionados vacía.", MessageDialogStyle.Affirmative, mySettings);
                        BrazaleteReportViewerVisible = BrazaleteSelected ? Visibility.Visible : Visibility.Collapsed;
                        GafeteReportViewerVisible = GafeteSelected ? Visibility.Visible : Visibility.Collapsed;
                    }
                    break;
            }
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                //if (!PConsultar)
                //{
                //    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                //    return;
                //}

                base.ClearRules();
                if (obj != null)
                {
                    var textbox = obj as System.Windows.Controls.TextBox;
                    if (textbox != null)
                        switch (textbox.Name)
                        {
                            case "AnioBuscar":
                                var Anio = 0;
                                int.TryParse(textbox.Text, out Anio);
                                AnioBuscar = Anio != 0 ? Anio : (int?)null;
                                break;
                            case "FolioBuscar":
                                var Folio = 0;
                                int.TryParse(textbox.Text, out Folio);
                                FolioBuscar = Folio != 0 ? Folio : (int?)null;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                        };
                };

                try
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        BrazaleteReportViewerVisible = Visibility.Collapsed;
                        GafeteReportViewerVisible = Visibility.Collapsed;
                    }));
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        Pagina = 1;
                        ListaIngresos = new List<cInternoGafeteBrazalete>();
                        EmptyVisible = true;
                        ObtenerIngresos();
                    });
                    System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        BrazaleteReportViewerVisible = BrazaleteSelected ? Visibility.Visible : Visibility.Collapsed;
                        GafeteReportViewerVisible = GafeteSelected ? Visibility.Visible : Visibility.Collapsed;
                    }));
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }
        #endregion

        #region Métodos
        public async void GenerarReporte(enumTipoReporte TipoReporte)
        {

            List<cReporteEdificio> lEdificio = new List<cReporteEdificio>();
            List<cReporteSector> lSector = new List<cReporteSector>();
            List<cBrazaleteGafeteInterno> internos = new List<cBrazaleteGafeteInterno>();
            try
            {

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        BrazaleteReportViewerVisible = Visibility.Collapsed;
                        GafeteReportViewerVisible = Visibility.Collapsed;
                    }));
                    var Centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                    if (SelectedEdificio.ID_EDIFICIO == TODOS_LOS_EDIFICIOS)
                    {
                        lEdificio.Add(new cReporteEdificio { Descr = "TODOS LOS EDIFICIOS", IdCentro = GlobalVar.gCentro, IdEdificio = TODOS_LOS_EDIFICIOS });
                    }
                    else
                    {
                        lEdificio.Add(new cReporteEdificio { Descr = SelectedEdificio.DESCR, IdCentro = SelectedEdificio.ID_CENTRO, IdEdificio = SelectedEdificio.ID_EDIFICIO });
                    }

                    if (SelectedEdificio.ID_EDIFICIO == TODOS_LOS_EDIFICIOS)
                    {
                        lSector.Add(new cReporteSector { Descr = "TODOS LOS SECTORES", IdCentro = GlobalVar.gCentro });
                    }
                    else
                    {
                        if (SelectedSector.ID_SECTOR == TODOS_LOS_SECTORES)
                        {
                            lSector.Add(new cReporteSector { Descr = "TODOS LOS SECTORES", IdCentro = GlobalVar.gCentro });
                        }
                        else
                        {
                            lSector.Add(new cReporteSector { Descr = SelectedSector.DESCR, IdCentro = GlobalVar.gCentro, IdEdificio = SelectedEdificio.ID_EDIFICIO, IdSector = SelectedSector.ID_SECTOR });
                        }
                    }

                    internos = new List<cBrazaleteGafeteInterno>();
                    try
                    {

                        foreach (var Ingreso in ListaIngresosSeleccionados)
                        {
                            var ingreso = new cIngreso().ObtenerUltimoIngreso(Ingreso.ID_CENTRO, Ingreso.ID_ANIO, Ingreso.ID_IMPUTADO);
                            var cama = ingreso.CAMA;
                            var foto_ingreso = ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                            var foto_centro = ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                            var ubicacion = cama != null ? string.Format("{0}-{1}-{2}",
                                !string.IsNullOrEmpty(cama.CELDA.SECTOR.EDIFICIO.DESCR) ? cama.CELDA.SECTOR.EDIFICIO.DESCR.TrimEnd() : string.Empty,
                                string.Format("{0}{1}",
                                !string.IsNullOrEmpty(cama.CELDA.SECTOR.DESCR) ? cama.CELDA.SECTOR.DESCR.TrimEnd() : string.Empty,
                                !string.IsNullOrEmpty(cama.CELDA.ID_CELDA) ? cama.CELDA.ID_CELDA.TrimStart().TrimEnd() : string.Empty),
                                cama.ID_CAMA) : "SIN UBIC.";
                            var writer = new BarcodeWriter() { Format = BarcodeFormat.CODE_39, Renderer = new BitmapRenderer() };
                            writer.Options.PureBarcode = true;
                            var bitmap = writer.Write(ingreso.IMPUTADO.NIP.TrimEnd());
                            var stream = new MemoryStream();
                            bitmap.Save(stream, ImageFormat.Bmp);
                            stream.Close();
                            var descripcion_centro = ingreso.CENTRO.DESCR.Replace("CERESO ", "").TrimStart();
                            internos.Add(new cBrazaleteGafeteInterno()
                            {
                                Centro = TipoReporte == enumTipoReporte.BRAZALETE ? ingreso.CENTRO.DESCR.TrimEnd() : descripcion_centro.TrimEnd(),
                                Expediente = string.Format("{0}/{1}", Ingreso.ID_ANIO, Ingreso.ID_IMPUTADO),
                                Foto = foto_centro != null ? foto_centro.BIOMETRICO : (foto_ingreso != null ? foto_ingreso.BIOMETRICO : new Imagenes().getImagenPerson()),
                                NIP = ingreso.IMPUTADO.NIP,
                                Nombre = string.Format("{1} {2} {0}", Ingreso.NOMBRE.TrimEnd(), Ingreso.PATERNO.TrimEnd(), Ingreso.MATERNO.TrimEnd()),
                                Ubicacion = ubicacion,
                                Codigo_Barras = stream.ToArray()
                            });
                        }
                    }
                    catch (Exception ex)
                    {

                        throw new ApplicationException(ex.Message);
                    }
                    System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        BrazaleteReportViewerVisible = BrazaleteSelected ? Visibility.Visible : Visibility.Collapsed;
                        GafeteReportViewerVisible = GafeteSelected ? Visibility.Visible : Visibility.Collapsed;
                    }));
                });

                #region Reporte
                switch (TipoReporte)
                {

                    case enumTipoReporte.BRAZALETE:

                        Reporte = Ventana.ReportBrazalete;
                        Reporte.LocalReport.ReportPath = "Reportes/rRelacionInternosBrazalete.rdlc";
                        Reporte.LocalReport.DataSources.Clear();
                        Reporte.Margin = new Padding(0, 10, 0, 0);

                        Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Brazalete = new Microsoft.Reporting.WinForms.ReportDataSource();
                        ReportDataSource_Brazalete.Name = "DataSet1";
                        ReportDataSource_Brazalete.Value = lEdificio;
                        Reporte.LocalReport.DataSources.Add(ReportDataSource_Brazalete);

                        #region Subreporte
                        Reporte.LocalReport.SubreportProcessing += (s, e) =>
                        {
                            if (e.ReportPath.Equals("srSectorInternosBrazalete", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ReportDataSource rds1 = new ReportDataSource("DataSet1", lSector);
                                e.DataSources.Add(rds1);
                            }
                            else
                            {
                                ReportDataSource rds2 = new ReportDataSource("DataSet1", internos);
                                e.DataSources.Add(rds2);
                            }
                        };
                        #endregion
                        break;
                    case enumTipoReporte.GAFETE:
                        Reporte = Ventana.ReportGafete;
                        Reporte.LocalReport.ReportPath = "Reportes/rRelacionInternosGafete.rdlc";
                        Reporte.LocalReport.DataSources.Clear();



                        Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Gafete = new Microsoft.Reporting.WinForms.ReportDataSource();
                        ReportDataSource_Gafete.Name = "DataSet1";
                        ReportDataSource_Gafete.Value = lEdificio;
                        Reporte.LocalReport.DataSources.Add(ReportDataSource_Gafete);

                        #region Subreporte
                        Reporte.LocalReport.SubreportProcessing += (s, e) =>
                        {
                            if (e.ReportPath.Equals("srSectorInternosGafete", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ReportDataSource rds1 = new ReportDataSource("DataSet1", lSector);
                                e.DataSources.Add(rds1);
                            }
                            else
                            {
                                ReportDataSource rds2 = new ReportDataSource("DataSet1", internos);
                                e.DataSources.Add(rds2);
                            }
                        };
                        #endregion
                        break;
                }
                #endregion
                Reporte.Refresh();
                Reporte.RefreshReport();

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        public void ObtenerIngresos()
        {
            short? nulo = null;

            var Ingresos = new cIngreso().ObtenerIngresosActivosFiltradosPorEdificio(GlobalVar.gCentro,
            SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS ? SelectedEdificio.ID_EDIFICIO : nulo,
            SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS ? (SelectedSector.ID_SECTOR != TODOS_LOS_SECTORES ? SelectedSector.ID_SECTOR : nulo) : nulo, Pagina,
             BusquedaAvanzadaChecked && AnioBuscar.HasValue ? AnioBuscar.Value : 0,
            BusquedaAvanzadaChecked && FolioBuscar.HasValue ? FolioBuscar.Value : 0,
            BusquedaAvanzadaChecked && !string.IsNullOrEmpty(NombreBuscar) ? NombreBuscar : string.Empty,
            BusquedaAvanzadaChecked && !string.IsNullOrEmpty(ApellidoPaternoBuscar) ? ApellidoPaternoBuscar : string.Empty,
            BusquedaAvanzadaChecked && !string.IsNullOrEmpty(ApellidoMaternoBuscar) ? ApellidoMaternoBuscar : string.Empty).ToList();
            if (Ingresos.Any())
            {
                Pagina++;
                SeguirCargandoIngresos = true;
                var lista_ingresos = new List<cInternoGafeteBrazalete>(ListaIngresos);
                byte[] fi,fc;
                INGRESO_BIOMETRICO bio;
                foreach (var Ingreso in Ingresos)
                {
                    if (Ingreso.INGRESO_BIOMETRICO != null)
                    {
                        bio = Ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                        if (bio != null)
                            fi = bio.BIOMETRICO;
                        else
                            fi = new Imagenes().getImagenPerson();
                        bio = Ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                        if (bio != null)
                            fc = bio.BIOMETRICO;
                        else
                            fc = new Imagenes().getImagenPerson();
                    }
                    else
                    {
                        fi = fc = new Imagenes().getImagenPerson();
                    }
                    
                    var interno = new cInternoGafeteBrazalete()
                        {
                            ID_CENTRO = Ingreso.ID_CENTRO,
                            ID_ANIO = Ingreso.ID_ANIO,
                            ID_IMPUTADO = Ingreso.ID_IMPUTADO,
                            ID_INGRESO = Ingreso.ID_INGRESO,
                            NOMBRE = !string.IsNullOrEmpty(Ingreso.IMPUTADO.NOMBRE) ? Ingreso.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                            PATERNO = !string.IsNullOrEmpty(Ingreso.IMPUTADO.PATERNO) ? Ingreso.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                            MATERNO = !string.IsNullOrEmpty(Ingreso.IMPUTADO.MATERNO) ? Ingreso.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                            SELECCIONADO = false,
                            FOTOINGRESO = fi,
                            FOTOCENTRO = fc,
                        };
                    if (ListaIngresosSeleccionados.Any())
                    {
                        if (!(ListaIngresosSeleccionados.Count(c =>
                            c.ID_CENTRO == interno.ID_CENTRO &&
                            c.ID_ANIO == interno.ID_ANIO &&
                            c.ID_IMPUTADO == interno.ID_IMPUTADO &&
                            c.ID_INGRESO == interno.ID_INGRESO) > 0))
                        {
                            interno.SELECCIONADO = SeleccionarTodosIngresos;
                            lista_ingresos.Add(interno);
                        }
                    }
                    else
                    {
                        interno.SELECCIONADO = SeleccionarTodosIngresos;
                        lista_ingresos.Add(interno);
                    }
                }
                ListaIngresos = lista_ingresos;
                EmptyVisible = !lista_ingresos.Any();
            }
            else
            {
                SeguirCargandoIngresos = false;
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
            System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                BrazaleteReportViewerVisible = Visibility.Collapsed;
                GafeteReportViewerVisible = Visibility.Collapsed;
            }));
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
            System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                BrazaleteReportViewerVisible = BrazaleteSelected ? Visibility.Visible : Visibility.Collapsed;
                GafeteReportViewerVisible = GafeteSelected ? Visibility.Visible : Visibility.Collapsed;
            }));
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_GAFETES_BRAZALETES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

using ControlPenales.BiometricoServiceReference;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZXing;
using ZXing.Rendering;

namespace ControlPenales
{
    public partial class ReporteCredencialBibliotecaViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteCredencialBiblioteca Window)
        {
            Ventana = Window;
            Reporte = Ventana.ReportCredencialBiblioteca;
            try
            {
                ConfiguraPermisos();
                ReportViewerVisible = Visibility.Collapsed;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    BusquedaAvanzadaChecked = false;
                    ObtenerEdificios();
                    ListaIngresos = new List<cCredencialBiblioteca>();
                    ListaIngresosSeleccionados = new List<cCredencialBiblioteca>();

                    EmptyVisible = EmptySeleccionadosVisible = true;
                    var placeholder = new Imagenes().getImagenPerson();
                    FotoIngreso = placeholder;
                    FotoCentro = placeholder;
                    Pagina = 1;
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
                    if (!ListaIngresosSeleccionados.Any())
                    {
                        var mensaje = System.Windows.Application.Current.Windows[0] as MetroWindow;
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Cerrar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        ReportViewerVisible = Visibility.Collapsed;
                        await mensaje.ShowMessageAsync("Validación", "Debe seleccionar al menos un interno para generar el reporte.", MessageDialogStyle.Affirmative, mySettings);
                        ReportViewerVisible = Visibility.Visible;
                    }
                    else
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        Reporte.Reset();
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            GenerarReporte();
                        });
                        ReportViewerVisible = Visibility.Visible;
                    }
                    break;
                case "ObtenerIngresos":
                    try
                    {
                        if (!pConsultar)
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        ReportViewerVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {

                            Pagina = 1;
                            ListaIngresos = new List<cCredencialBiblioteca>();
                            EmptyVisible = true;
                            ObtenerIngresos();

                        });
                        ReportViewerVisible = Visibility.Visible;
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }
                    break;
                case "Permitir":
                    if (!SelectedIngreso.Seleccionado)
                        SeleccionarTodosIngresos = false;
                    else
                        SeleccionarTodosIngresos = !ListaIngresos.Where(w => !w.Seleccionado).Any();
                    break;
                case "SeleccionarTodosIngresos":
                    if (ListaIngresos.Any())
                    {
                        var lista_ingresos = new List<cCredencialBiblioteca>(ListaIngresos);
                        foreach (var Ingreso in lista_ingresos)
                        {
                            Ingreso.Seleccionado = SeleccionarTodosIngresos;
                        }
                        ListaIngresos = lista_ingresos;
                    }
                    else
                    {
                        SeleccionarTodosIngresos = false;
                    }
                    break;
                case "PermitirSeleccionado":
                    if (!SelectedIngresoSeleccionado.Seleccionado)
                        SeleccionarTodosIngresosSeleccionados = false;
                    else
                        SeleccionarTodosIngresosSeleccionados = !ListaIngresosSeleccionados.Where(w => !w.Seleccionado).Any();
                    break;
                case "SeleccionarTodosIngresosSeleccionados":
                    if (ListaIngresosSeleccionados.Any())
                    {
                        var lista_ingresos_Seleccionados = new List<cCredencialBiblioteca>(ListaIngresosSeleccionados);
                        foreach (var Ingreso in lista_ingresos_Seleccionados)
                        {
                            Ingreso.Seleccionado = SeleccionarTodosIngresosSeleccionados;
                        }
                        ListaIngresosSeleccionados = lista_ingresos_Seleccionados;
                    }
                    else
                    {
                        SeleccionarTodosIngresosSeleccionados = false;
                    }
                    break;
                case "AgregarInternos":
                    if (ListaIngresos.Any())
                    {
                        if (ListaIngresos.Where(w => w.Seleccionado).Any())
                        {
                            var lista_ingresos = new List<cCredencialBiblioteca>(ListaIngresos);
                            var lista_ingresos_Seleccionados = new List<cCredencialBiblioteca>(
                                ListaIngresos.Where(w => w.Seleccionado).
                                OrderByDescending(oD => oD.Id_Anio).
                                ThenByDescending(tD => tD.Id_Imputado).
                                ToList());
                            foreach (var ingreso_Seleccionado in lista_ingresos_Seleccionados)
                            {
                                ingreso_Seleccionado.Seleccionado = false;
                                lista_ingresos.Remove(ingreso_Seleccionado);
                            }
                            ListaIngresos = lista_ingresos.Any() ? lista_ingresos.OrderByDescending(oD => oD.Id_Anio).ThenByDescending(tD => tD.Id_Imputado).ToList() : new List<cCredencialBiblioteca>();
                            EmptyVisible = !ListaIngresos.Any();
                            EmptySeleccionadosVisible = false;
                            ListaIngresosSeleccionados.AddRange(lista_ingresos_Seleccionados);
                            ListaIngresosSeleccionados = ListaIngresosSeleccionados.OrderByDescending(oD => oD.Id_Anio).ThenByDescending(tD => tD.Id_Imputado).ToList();
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
                            ReportViewerVisible = Visibility.Collapsed;
                            await mensaje.ShowMessageAsync("Validación", "Debe seleccionar al menos un interno.", MessageDialogStyle.Affirmative, mySettings);
                            ReportViewerVisible = Visibility.Visible;
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
                        ReportViewerVisible = Visibility.Collapsed;
                        await mensaje.ShowMessageAsync("Validación", "Lista de internos vacía. Realice una búsqueda e intente de nuevo.", MessageDialogStyle.Affirmative, mySettings);
                        ReportViewerVisible = Visibility.Visible;
                    }
                    break;
                case "RemoverInternos":
                    if (ListaIngresosSeleccionados.Any())
                    {
                        if (ListaIngresosSeleccionados.Where(w => w.Seleccionado).Any())
                        {
                            var lista_ingresos = new List<cCredencialBiblioteca>(ListaIngresosSeleccionados);
                            var lista_ingresos_Seleccionados = new List<cCredencialBiblioteca>(
                                ListaIngresosSeleccionados.Where(w => w.Seleccionado).
                                OrderByDescending(oD => oD.Id_Anio).
                                ThenByDescending(tD => tD.Id_Imputado).
                                ToList());
                            foreach (var ingreso_Seleccionado in lista_ingresos_Seleccionados)
                            {
                                ingreso_Seleccionado.Seleccionado = false;
                                lista_ingresos.Remove(ingreso_Seleccionado);
                            }
                            ListaIngresosSeleccionados = lista_ingresos.Any() ? lista_ingresos.OrderByDescending(oD => oD.Id_Anio).ThenByDescending(tD => tD.Id_Imputado).ToList() : new List<cCredencialBiblioteca>();
                            EmptySeleccionadosVisible = !ListaIngresosSeleccionados.Any();
                            EmptyVisible = false;
                            ListaIngresos.AddRange(lista_ingresos_Seleccionados);
                            ListaIngresos = ListaIngresos.OrderByDescending(oD => oD.Id_Anio).ThenByDescending(tD => tD.Id_Imputado).ToList();
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
                            ReportViewerVisible = Visibility.Collapsed;
                            await mensaje.ShowMessageAsync("Validación", "Debe seleccionar al menos un interno.", MessageDialogStyle.Affirmative, mySettings);
                            ReportViewerVisible = Visibility.Visible;
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
                        ReportViewerVisible = Visibility.Collapsed;
                        await mensaje.ShowMessageAsync("Validación", "Lista de internos Seleccionados vacía.", MessageDialogStyle.Affirmative, mySettings);
                        ReportViewerVisible = Visibility.Visible;
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
                    if (!pConsultar)
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    }
                    else
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {

                            Pagina = 1;
                            ListaIngresos = new List<cCredencialBiblioteca>();
                            EmptyVisible = true;
                            ObtenerIngresos();

                        });
                        ReportViewerVisible = Visibility.Visible;
                    }

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

        public byte[] ObtenerCodigoBarras(string NIP)
        {
            if (!string.IsNullOrEmpty(NIP))
            {
                var writer = new BarcodeWriter() { Format = BarcodeFormat.CODE_39, Renderer = new BitmapRenderer() };
                writer.Options.PureBarcode = true;
                var bitmap = writer.Write(NIP.TrimEnd());
                var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Close();
                return stream.ToArray();
            }
            return null;
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
                var lista_ingresos = new List<cCredencialBiblioteca>(ListaIngresos);
                foreach (var Ingreso in Ingresos)
                {
                    var foto_ingreso = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                    var foto_centro = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                    var cama = new cCama().ObtenerCama((short)Ingreso.ID_UB_CENTRO, (short)Ingreso.ID_UB_EDIFICIO, (short)Ingreso.ID_UB_SECTOR, Ingreso.ID_UB_CELDA, (short)Ingreso.ID_UB_CAMA);
                    var interno = new cCredencialBiblioteca()
                    {
                        Id_Centro = Ingreso.ID_CENTRO,
                        Id_Anio = Ingreso.ID_ANIO,
                        Id_Imputado = Ingreso.ID_IMPUTADO,
                        Id_Ingreso = Ingreso.ID_INGRESO,
                        Nombre = !string.IsNullOrEmpty(Ingreso.IMPUTADO.NOMBRE) ? Ingreso.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                        Paterno = !string.IsNullOrEmpty(Ingreso.IMPUTADO.PATERNO) ? Ingreso.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                        Materno = !string.IsNullOrEmpty(Ingreso.IMPUTADO.MATERNO) ? Ingreso.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                        CentroDescr = Ingreso.CENTRO.DESCR.Replace("CERESO ", "").TrimStart().TrimEnd(),
                        EdificioDescr = cama.CELDA.SECTOR.EDIFICIO.DESCR.TrimEnd(),
                        SectorDescr = cama.CELDA.SECTOR.DESCR.TrimEnd(),
                        Celda = cama.ID_CELDA.TrimStart().TrimEnd(),
                        Foto = foto_centro != null ? foto_centro.BIOMETRICO : (foto_ingreso != null ? foto_ingreso.BIOMETRICO : new Imagenes().getImagenPerson()),
                        CodigoBarras = ObtenerCodigoBarras(Ingreso.IMPUTADO.NIP),
                        Seleccionado = false,
                        FOTOCENTRO = foto_centro != null ? foto_centro.BIOMETRICO : new Imagenes().getImagenPerson(),
                        FOTOINGRESO = foto_ingreso != null ? foto_ingreso.BIOMETRICO : new Imagenes().getImagenPerson(),
                    };
                    if (ListaIngresosSeleccionados.Any())
                    {
                        if (!(ListaIngresosSeleccionados.Count(c =>
                            c.Id_Centro == interno.Id_Centro &&
                            c.Id_Anio == interno.Id_Anio &&
                            c.Id_Imputado == interno.Id_Imputado &&
                            c.Id_Ingreso == interno.Id_Ingreso) > 0))
                        {
                            interno.Seleccionado = SeleccionarTodosIngresos;
                            lista_ingresos.Add(interno);
                        }
                    }
                    else
                    {
                        interno.Seleccionado = SeleccionarTodosIngresos;
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

            Sectores = new List<SECTOR>();
            Sectores.Add(new SECTOR() { ID_SECTOR = TODOS_LOS_SECTORES, DESCR = "TODOS" });
            SelectedSector = Sectores.FirstOrDefault();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                }));
                if (SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS)
                    Sectores.AddRange(new cSector().ObtenerTodos(null, null, GlobalVar.gCentro, SelectedEdificio.ID_EDIFICIO).ToList());
                else
                    Sectores.RemoveRange(1, Sectores.Count - 1);
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Visible;
                }));
            });

        }

        public void GenerarReporte()
        {
            var Centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
            try
            {
                List<cReporteEdificio> lEdificio = new List<cReporteEdificio>();
                List<cReporteSector> lSector = new List<cReporteSector>();
                List<cBrazaleteGafeteInterno> internos = new List<cBrazaleteGafeteInterno>();

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
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte = Ventana.ReportCredencialBiblioteca;
                    Reporte.LocalReport.ReportPath = "Reportes/rCredencialBiblioteca.rdlc";
                    Reporte.LocalReport.DataSources.Clear();
                    Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_CredencialBiblioteca = new Microsoft.Reporting.WinForms.ReportDataSource();
                    ReportDataSource_CredencialBiblioteca.Name = "DataSet1";
                    ReportDataSource_CredencialBiblioteca.Value = ListaIngresosSeleccionados;
                    Reporte.LocalReport.DataSources.Add(ReportDataSource_CredencialBiblioteca);

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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_CREDENCIAL_BIBLIOTECA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

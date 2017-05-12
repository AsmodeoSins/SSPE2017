using ControlPenales.BiometricoServiceReference;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ControlPenales.Clases;

namespace ControlPenales
{
    public partial class ReporteFormatoIdentificacionViewModel : ValidationViewModelBase
    {

        public async void OnLoad(ReporteFormatoIdentificacion Window)
        {
            ConfiguraPermisos();
            Ventana = Window;
            Reporte = Ventana.Report;
            AnioBuscarImputado = FolioBuscarImputado = NombreBuscarImputado = PaternoBuscarImputado = MaternoBuscarImputado = string.Empty;
            try
            {

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));
                    ListaIngresos = new List<cFormatoIdentificacion>();
                    ListaIngresosSeleccionados = new List<cFormatoIdentificacion>();

                    EmptyVisible = EmptySeleccionadosVisible = true;
                    var placeholder = new Imagenes().getImagenPerson();
                    FotoIngreso = placeholder;
                    FotoCentro = placeholder;
                    Pagina = 1;
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
                        Reporte.Reset();
                        GenerarReporte();
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
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                ReportViewerVisible = Visibility.Collapsed;
                            }));

                            Pagina = 1;
                            ListaIngresos = new List<cFormatoIdentificacion>();
                            EmptyVisible = true;
                            ObtenerIngresos();
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
                        var lista_ingresos = new List<cFormatoIdentificacion>(ListaIngresos);
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
                        var lista_ingresos_Seleccionados = new List<cFormatoIdentificacion>(ListaIngresosSeleccionados);
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
                            var lista_ingresos = new List<cFormatoIdentificacion>(ListaIngresos);
                            var lista_ingresos_Seleccionados = new List<cFormatoIdentificacion>(
                                ListaIngresos.Where(w => w.Seleccionado).
                                OrderByDescending(oD => oD.Id_Anio).
                                ThenByDescending(tD => tD.Id_Imputado).
                                ToList());
                            foreach (var ingreso_Seleccionado in lista_ingresos_Seleccionados)
                            {
                                ingreso_Seleccionado.Seleccionado = false;
                                lista_ingresos.Remove(ingreso_Seleccionado);
                            }
                            ListaIngresos = lista_ingresos.Any() ? lista_ingresos.OrderByDescending(oD => oD.Id_Anio).ThenByDescending(tD => tD.Id_Imputado).ToList() : new List<cFormatoIdentificacion>();
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
                            var lista_ingresos = new List<cFormatoIdentificacion>(ListaIngresosSeleccionados);
                            var lista_ingresos_Seleccionados = new List<cFormatoIdentificacion>(
                                ListaIngresosSeleccionados.Where(w => w.Seleccionado).
                                OrderByDescending(oD => oD.Id_Anio).
                                ThenByDescending(tD => tD.Id_Imputado).
                                ToList());
                            foreach (var ingreso_Seleccionado in lista_ingresos_Seleccionados)
                            {
                                ingreso_Seleccionado.Seleccionado = false;
                                lista_ingresos.Remove(ingreso_Seleccionado);
                            }
                            ListaIngresosSeleccionados = lista_ingresos.Any() ? lista_ingresos.OrderByDescending(oD => oD.Id_Anio).ThenByDescending(tD => tD.Id_Imputado).ToList() : new List<cFormatoIdentificacion>();
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

        public void ObtenerIngresos()
        {
            var Anio = 0;
            var Folio = 0;
            int.TryParse(AnioBuscarImputado, out Anio);
            int.TryParse(FolioBuscarImputado, out Folio);

            var Ingresos = new cIngreso().ObtenerTodosReporte(GlobalVar.gCentro, Anio != 0 ? Anio : 0, Folio != 0 ? Folio : 0, !string.IsNullOrEmpty(NombreBuscarImputado) ? NombreBuscarImputado : string.Empty, !string.IsNullOrEmpty(PaternoBuscarImputado) ? PaternoBuscarImputado : string.Empty, !string.IsNullOrEmpty(MaternoBuscarImputado) ? MaternoBuscarImputado : string.Empty);
            if (Ingresos.Any())
            {
                Pagina++;
                SeguirCargandoIngresos = true;
                var lista_ingresos = new List<cFormatoIdentificacion>(ListaIngresos);
                foreach (var Ingreso in Ingresos)
                {
                    var foto_ingreso = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                    var foto_centro = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                    var interno = new cFormatoIdentificacion()
                    {
                        Id_Anio = Ingreso.ID_ANIO,
                        Id_Imputado = Ingreso.ID_IMPUTADO,
                        Id_ingreso = Ingreso.ID_INGRESO,
                        Nombre = !string.IsNullOrEmpty(Ingreso.IMPUTADO.NOMBRE) ? Ingreso.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                        Paterno = !string.IsNullOrEmpty(Ingreso.IMPUTADO.PATERNO) ? Ingreso.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                        Materno = !string.IsNullOrEmpty(Ingreso.IMPUTADO.MATERNO) ? Ingreso.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                    };
                    if (ListaIngresosSeleccionados.Any())
                    {
                        if (!(ListaIngresosSeleccionados.Count(c =>
                            c.Id_Anio == interno.Id_Anio &&
                            c.Id_Imputado == interno.Id_Imputado &&
                            c.Id_ingreso == interno.Id_ingreso) > 0))
                        {
                            lista_ingresos.Add(interno);
                        }
                    }
                    else
                    {
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

        public string ObtenerAlias(INGRESO Ingreso)
        {
            var ListaAlias = Ingreso.IMPUTADO.ALIAS;
            var lAlias = new StringBuilder();

            foreach (var Alias in ListaAlias)
            {
                lAlias.Append(string.Format("{1} {2} {0}, ", !string.IsNullOrEmpty(Alias.NOMBRE) ? Alias.NOMBRE.TrimEnd() : string.Empty, !string.IsNullOrEmpty(Alias.PATERNO) ? Alias.PATERNO.TrimEnd() : string.Empty, !string.IsNullOrEmpty(Alias.MATERNO) ? Alias.MATERNO.TrimEnd() : string.Empty));
            }
            if (ListaAlias.Count > 0)
                lAlias.Remove(lAlias.Length - 1, 1);
            return lAlias.ToString();
        }

        public string ObtenerApodos(INGRESO Ingreso)
        {
            var ListaApodos = Ingreso.IMPUTADO.APODO.Where(w => w.APODO1 != null).ToList();
            var lApodos = new StringBuilder();

            foreach (var Apodo in ListaApodos)
            {
                lApodos.Append(string.Format("{0},", Apodo.APODO1.TrimEnd()));
            }
            if (ListaApodos.Count > 0)
                lApodos.Remove(lApodos.Length - 1, 1);
            return lApodos.ToString();
        }

        public int ObtenerEdad(DateTime Fecha_Nacimiento)
        {
            return (Fecha_Nacimiento.Month < Fechas.GetFechaDateServer.Month) ?
                Fechas.GetFechaDateServer.Year - Fecha_Nacimiento.Year :
                ((Fecha_Nacimiento.Month == Fechas.GetFechaDateServer.Month &&
                Fecha_Nacimiento.Day <= Fechas.GetFechaDateServer.Day) ? (Fechas.GetFechaDateServer.Year - Fecha_Nacimiento.Year) :
                (Fechas.GetFechaDateServer.Year - Fecha_Nacimiento.Year) - 1);
        }

        public string ObtenerMediaFiliacion(enumMediaFilicacion Media_Filiacion_Tipo)
        {
            var media_filiacion = string.Empty;
            switch (Media_Filiacion_Tipo)
            {
                case enumMediaFilicacion.NARIZ_RAIZ:
                    break;
                case enumMediaFilicacion.NARIZ_DORSO:
                    break;
                case enumMediaFilicacion.NARIZ_ANCHO:
                    break;
                case enumMediaFilicacion.NARIZ_BASE:
                    break;
                case enumMediaFilicacion.NARIZ_ALTURA:
                    break;
                case enumMediaFilicacion.CARA:
                    break;
                case enumMediaFilicacion.CABELLO_CANTIDAD:
                    break;
                case enumMediaFilicacion.CABELLO_COLOR:
                    break;
                case enumMediaFilicacion.CABELLO_CALVICIE:
                    break;
                case enumMediaFilicacion.CABELLO_FORMA:
                    break;
                case enumMediaFilicacion.CEJAS_DIRECCION:
                    break;
                case enumMediaFilicacion.CEJAS_IMPLANTACION:
                    break;
                case enumMediaFilicacion.CEJAS_FORMA:
                    break;
                case enumMediaFilicacion.CEJAS_TAMANO:
                    break;
                case enumMediaFilicacion.OJOS_COLOR:
                    break;
                case enumMediaFilicacion.OJOS_FORMA:
                    break;
                case enumMediaFilicacion.OJOS_TAMANO:
                    break;
                case enumMediaFilicacion.BOCA_TAMANO:
                    break;
                case enumMediaFilicacion.BOCA_COMISURAS:
                    break;
                case enumMediaFilicacion.LABIOS_ESPESOR:
                    break;
                case enumMediaFilicacion.SANGRE_TIPO:
                    break;
                case enumMediaFilicacion.SANGRE_FACTOR_RH:
                    break;
                case enumMediaFilicacion.MENTON_TIPO:
                    break;
                case enumMediaFilicacion.MENTON_FORMA:
                    break;
                case enumMediaFilicacion.MENTON_INCLINACION:
                    break;
                case enumMediaFilicacion.FRENTE_ALTURA:
                    break;
                case enumMediaFilicacion.FRENTE_INCLINACION:
                    break;
                case enumMediaFilicacion.FRENTE_ANCHO:
                    break;
                case enumMediaFilicacion.COLOR_PIEL:
                    break;
                case enumMediaFilicacion.CABELLO_IMPLANTACION:
                    break;
                case enumMediaFilicacion.LABIOS_ALTURA_NASO_LABIAL:
                    break;
                case enumMediaFilicacion.LABIOS_PROMINENCIA:
                    break;
                case enumMediaFilicacion.OREJA_BORDE_FORMA:
                    break;
                case enumMediaFilicacion.COMPLEXION:
                    break;
                case enumMediaFilicacion.OREJA_HELIX_ORIGINAL:
                    break;
                case enumMediaFilicacion.OREJA_HELIX_SUPERIOR:
                    break;
                case enumMediaFilicacion.OREJA_HELIX_POSTERIOR:
                    break;
                case enumMediaFilicacion.OREJA_HELIX_ADHERENCIA:
                    break;
                case enumMediaFilicacion.OREJA_LOBULO_CONTORNO:
                    break;
                case enumMediaFilicacion.OREJA_LOBULO_ADHERENCIA:
                    break;
                case enumMediaFilicacion.OREJA_LOBULO_PARTICULARIDAD:
                    break;
                case enumMediaFilicacion.OREJA_LOBULO_DIMENSION:
                    break;
                default:
                    media_filiacion = "INDEFINIDO";
                    break;
            }
            return media_filiacion;
        }

        public async void GenerarReporte()
        {
            var datosReporte = new List<cReporteDatos>();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                }));
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
                    var consulta_ingreso = new cIngreso();
                    foreach (var Ingreso in ListaIngresosSeleccionados)
                    {
                        var interno = consulta_ingreso.ObtenerUltimoIngresoReporte((short)GlobalVar.gCentro, (short)Ingreso.Id_Anio, Ingreso.Id_Imputado);

                        //DATOS PERSONALES
                        Ingreso.Id_Anio = interno.ID_ANIO;
                        Ingreso.Id_Imputado = interno.ID_IMPUTADO;
                        Ingreso.Folio_Gob = (interno.ANIO_GOBIERNO.HasValue && !string.IsNullOrEmpty(interno.FOLIO_GOBIERNO)) ? string.Format("{0}/{1}", interno.ANIO_GOBIERNO.Value, interno.FOLIO_GOBIERNO) : "SIN DEFINIR";
                        Ingreso.Id_ingreso = interno.ID_INGRESO;
                        Ingreso.Fec_Ingreso = interno.FEC_INGRESO_CERESO.HasValue ? interno.FEC_INGRESO_CERESO.Value.Date : DateTime.Now;
                        Ingreso.Materno = !string.IsNullOrEmpty(interno.IMPUTADO.MATERNO) ? interno.IMPUTADO.MATERNO.TrimEnd() : string.Empty;
                        Ingreso.Paterno = !string.IsNullOrEmpty(interno.IMPUTADO.PATERNO) ? interno.IMPUTADO.PATERNO.TrimEnd() : string.Empty;
                        Ingreso.Nombre = !string.IsNullOrEmpty(interno.IMPUTADO.NOMBRE) ? interno.IMPUTADO.NOMBRE.TrimEnd() : string.Empty;
                        Ingreso.Alias = ObtenerAlias(interno);
                        Ingreso.Apodos = ObtenerApodos(interno);
                        Ingreso.Estatus = interno.ESTATUS_ADMINISTRATIVO.DESCR.TrimEnd();
                        Ingreso.Tipo_Ingreso = !string.IsNullOrEmpty(interno.TIPO_INGRESO.DESCR) ? interno.TIPO_INGRESO.DESCR.TrimEnd() : "SIN DEFINIR";
                        Ingreso.Oficio_Internacion = !string.IsNullOrEmpty(interno.DOCINTERNACION_NUM_OFICIO) ? interno.DOCINTERNACION_NUM_OFICIO.TrimEnd() : "SIN DEFINIR";
                        Ingreso.Autoridad = "SIN DEFINIR";
                        Ingreso.A_Disposicion = !string.IsNullOrEmpty(interno.A_DISPOSICION) ? interno.A_DISPOSICION.TrimEnd() : "SIN DEFINIR";

                        //FOTOGRAFIAS
                        var foto_frente = interno.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                        var perfil_derecho = interno.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_SEGUIMIENTO).FirstOrDefault();
                        var perfil_izquierdo = interno.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_IZQ_SEGUIMIENTO).FirstOrDefault();
                        Ingreso.Fotografia_Frente = foto_frente != null ? foto_frente.BIOMETRICO : new Imagenes().getImagenPerson();
                        Ingreso.Perfil_Derecho = perfil_derecho != null ? perfil_derecho.BIOMETRICO : new Imagenes().getImagenPerson();
                        Ingreso.Perfil_Izquierdo = perfil_izquierdo != null ? perfil_izquierdo.BIOMETRICO : new Imagenes().getImagenPerson();

                        //DATOS GENERALES
                        Ingreso.Padre_Nombre_Completo = string.Format("{1} {2} {0}",
                           !string.IsNullOrEmpty(interno.IMPUTADO.NOMBRE_PADRE) ? interno.IMPUTADO.NOMBRE_PADRE.TrimEnd() : string.Empty,
                           !string.IsNullOrEmpty(interno.IMPUTADO.PATERNO_PADRE) ? interno.IMPUTADO.PATERNO_PADRE.TrimEnd() : string.Empty,
                           !string.IsNullOrEmpty(interno.IMPUTADO.MATERNO_PADRE) ? interno.IMPUTADO.MATERNO_PADRE.TrimEnd() : string.Empty);
                        Ingreso.Madre_Nombre_Completo = string.Format("{1} {2} {0}",
                            !string.IsNullOrEmpty(interno.IMPUTADO.NOMBRE_MADRE) ? interno.IMPUTADO.NOMBRE_MADRE.TrimEnd() : string.Empty,
                            !string.IsNullOrEmpty(interno.IMPUTADO.PATERNO_MADRE) ? interno.IMPUTADO.PATERNO_MADRE.TrimEnd() : string.Empty,
                            !string.IsNullOrEmpty(interno.IMPUTADO.MATERNO_MADRE) ? interno.IMPUTADO.MATERNO_MADRE.TrimEnd() : string.Empty);
                        Ingreso.Origen = !string.IsNullOrEmpty(interno.IMPUTADO.NACIMIENTO_LUGAR) ? interno.IMPUTADO.NACIMIENTO_LUGAR.TrimEnd() : "SIN DEFINIR";
                        Ingreso.Domicilio = !string.IsNullOrEmpty(interno.DOMICILIO_CALLE) ? interno.DOMICILIO_CALLE.TrimEnd() : "SIN DEFINIR";
                        Ingreso.Numero_Domicilio = interno.DOMICILIO_NUM_EXT.HasValue ? interno.DOMICILIO_NUM_EXT.Value.ToString() : "SIN DEFINIR";
                        Ingreso.Colonia_Domicilio = interno.ID_COLONIA.HasValue ? interno.COLONIA.DESCR.TrimEnd() : "SIN DEFINIR";
                        Ingreso.Ciudad = interno.ID_MUNICIPIO.HasValue ? interno.MUNICIPIO.MUNICIPIO1.TrimEnd() : "SIN DEFINIR";
                        Ingreso.Fec_Nacimiento = interno.IMPUTADO.NACIMIENTO_FECHA.HasValue ? interno.IMPUTADO.NACIMIENTO_FECHA.Value : DateTime.Now;
                        Ingreso.Edad = interno.IMPUTADO.NACIMIENTO_FECHA.HasValue ? ObtenerEdad(interno.IMPUTADO.NACIMIENTO_FECHA.Value) : 0;
                        Ingreso.Sexo = !string.IsNullOrEmpty(interno.IMPUTADO.SEXO) ? interno.IMPUTADO.SEXO.TrimEnd() : "SIN DEFINIR";
                        Ingreso.Estado_Civil = interno.ESTADO_CIVIL != null ? (!string.IsNullOrEmpty(interno.ESTADO_CIVIL.DESCR) ? interno.ESTADO_CIVIL.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR";
                        Ingreso.Religion = interno.RELIGION != null ? (!string.IsNullOrEmpty(interno.RELIGION.DESCR) ? interno.RELIGION.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR";
                        Ingreso.Ocupacion = interno.OCUPACION != null ? (!string.IsNullOrEmpty(interno.OCUPACION.DESCR) ? interno.OCUPACION.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR";
                        Ingreso.Grado_Maximo_Estudios = interno.ESCOLARIDAD != null ? interno.ESCOLARIDAD.DESCR.TrimEnd() : "SIN DEFINIR";
                        Ingreso.Lugar_Nacimiento_Extranjeros = !string.IsNullOrEmpty(interno.IMPUTADO.NACIMIENTO_LUGAR) ? interno.IMPUTADO.NACIMIENTO_LUGAR.TrimEnd() : "SIN DEFINIR";

                        //MEDIA FILIACIÓN
                        Ingreso.Estatura = interno.ESTATURA.HasValue ? interno.ESTATURA.Value : 0;
                        Ingreso.Peso = interno.PESO.HasValue ? interno.PESO.Value : 0;
                        Ingreso.Complexion = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.COMPLEXION).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.COMPLEXION).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Color_Piel = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.COLOR_PIEL).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.COLOR_PIEL).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Color_Cabello = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.CABELLO_COLOR).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.CABELLO_COLOR).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Forma_Cabello = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.CABELLO_FORMA).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.CABELLO_FORMA).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Frente_Alta = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.FRENTE_ALTURA).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.FRENTE_ALTURA).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Frente_Inclinada = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.FRENTE_INCLINACION).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.FRENTE_INCLINACION).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Frente_Ancha = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.FRENTE_ANCHO).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.FRENTE_ANCHO).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Color_Ojos = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.OJOS_COLOR).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.OJOS_COLOR).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Forma_Ojos = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.OJOS_FORMA).FirstOrDefault() != null ?
                           interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.OJOS_FORMA).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Tamano_Ojos = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.OJOS_TAMANO).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.OJOS_TAMANO).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Raiz_Nariz = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.NARIZ_RAIZ).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.NARIZ_RAIZ).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Ancho_Nariz = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.NARIZ_ANCHO).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.NARIZ_ANCHO).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Tamano_Boca = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.BOCA_TAMANO).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.BOCA_TAMANO).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Comisuras_Boca = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.BOCA_COMISURAS).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.BOCA_COMISURAS).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Espesor_Labios = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.LABIOS_ESPESOR).FirstOrDefault() != null ?
                           interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.LABIOS_ESPESOR).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Altura_Labios = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.LABIOS_ALTURA_NASO_LABIAL).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.LABIOS_ALTURA_NASO_LABIAL).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Promedio_Labios = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.LABIOS_PROMINENCIA).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.LABIOS_PROMINENCIA).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Tipo_Menton = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.MENTON_TIPO).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.MENTON_TIPO).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Forma_Menton = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.MENTON_FORMA).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.MENTON_FORMA).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                        Ingreso.Inclinacion_Menton = interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.MENTON_INCLINACION).FirstOrDefault() != null ?
                            interno.IMPUTADO.IMPUTADO_FILIACION.Where(w => (enumMediaFilicacion)w.ID_MEDIA_FILIACION == enumMediaFilicacion.MENTON_INCLINACION).FirstOrDefault().TIPO_FILIACION.DESCR.TrimEnd() : string.Empty;
                    }

                }
                catch (Exception ex)
                {

                    throw new ApplicationException(ex.Message);
                }
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte = Ventana.Report;
                    Reporte.LocalReport.ReportPath = "Reportes/rFormatoIdentificacion.rdlc";
                    Reporte.LocalReport.DataSources.Clear();
                    ReportDataSource ReportDataSource_Encabezado = new Microsoft.Reporting.WinForms.ReportDataSource();
                    ReportDataSource_Encabezado.Name = "DataSet1";
                    ReportDataSource_Encabezado.Value = datosReporte;

                    ReportDataSource ReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                    ReportDataSource.Name = "DataSet2";
                    ReportDataSource.Value = ListaIngresosSeleccionados;

                    Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                    Reporte.LocalReport.DataSources.Add(ReportDataSource);
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;
                }));
            });



        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_FORMATO_IDENTIFICACION_FICHA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

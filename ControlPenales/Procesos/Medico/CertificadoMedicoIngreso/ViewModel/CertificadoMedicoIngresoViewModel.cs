using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using ControlPenales.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlPenales
{
    partial class CertificadoMedicoIngresoViewModel : ValidationViewModelBase
    {
        public CertificadoMedicoIngresoViewModel(INGRESO select = null)
        {
            try
            {
                if (SelectIngreso != null)
                {
                    SelectExpediente = select.IMPUTADO;
                    SelectIngreso = select;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la configuración  principal", ex);
            }
        }

        private void WindowUnLoad(CertificadoMedicoIngresoView Window = null)
        {
            try
            {
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la configuración  principal", ex);
            }
        }

        private void WindowLoad(CertificadoMedicoIngresoView Window = null)
        {
            try
            {
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la configuración  principal", ex);
            }
        }

        private void listBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is ListBoxItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null) return;
            var item = AutoCompleteLB.ItemContainerGenerator.ItemFromContainer(dep);
            if (item == null) return;
            new ControlPenales.Controls.AutoCompleteTextBox().SetTextValueBySelection(item, false);
        }

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                new ControlPenales.Controls.AutoCompleteTextBox().SetTextValueBySelection(AutoCompleteLB.SelectedItem, false);
            else if (e.Key == Key.Tab)
                new ControlPenales.Controls.AutoCompleteTextBox().SetTextValueBySelection(AutoCompleteLB.SelectedItem, true);
        }

        private void EnfermedadSeleccionada(Object obj)
        {
            try
            {
                if (!(obj is ENFERMEDAD))
                {
                    var enfermedad = (ENFERMEDAD)obj;

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la enfermedad.", ex);
            }
        }

        private void SeniasFrenteLoad(SeniasFrenteView Window = null)
        {
            try
            {
                if (Window == null)
                    return;
                if (ListRadioButonsFrente != null ? ListRadioButonsFrente.Any() : false)
                    return;
                ListRadioButonsFrente = new List<RadioButton>();
                ListRadioButonsFrente = new FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridFrente"))).ToList();
                if (SelectLesion != null)
                    foreach (var item in ListRadioButonsFrente)
                        item.IsChecked = item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar datos iniciales de las lesiones.", ex);
            }
        }

        private void SeniasDorsoLoad(SeniasDorsoView Window = null)
        {
            try
            {
                if (Window == null)
                    return;
                if (ListRadioButonsDorso != null ? ListRadioButonsDorso.Any() : false)
                    return;
                ListRadioButonsDorso = new List<RadioButton>();
                ListRadioButonsDorso = new FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridDorso"))).ToList();
                if (SelectLesion != null)
                    foreach (var item in ListRadioButonsDorso)
                        item.IsChecked = item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar datos iniciales de las lesiones.", ex);
            }
        }

        private void RegionSwitch(Object obj)
        {
            try
            {
                SelectRegion = short.Parse(obj.ToString().Split('-')[0]);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la region del cuerpo.", ex);
            }
        }

        private void LesionSelected(Object obj)
        {
            try
            {
                if (!(obj is LesionesCustom)) return;
                SelectLesion = (LesionesCustom)obj;
                TextDescripcionLesion = SelectLesion.DESCR;
                SelectRegion = SelectLesion.REGION.ID_REGION;
                if (SelectLesion.REGION.LADO == "D")
                    foreach (var item in ListRadioButonsDorso)
                        if (item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false)
                        {
                            item.IsChecked = true;
                            return;
                        }
                if (SelectLesion.REGION.LADO == "F")
                    foreach (var item in ListRadioButonsFrente)
                        if (item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false)
                        {
                            item.IsChecked = true;
                            return;
                        }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la lesion.", ex);
            }
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "limpiar_lesion":
                    BotonLesionEnabled = true;
                    TextDescripcionLesion = string.Empty;
                    var radioButons = new FingerPrintScanner().FindVisualChildren<RadioButton>(((CertificadoMedicoIngresoView)((ContentControl)Application.Current.Windows[0].FindName("contentControl")).Content)).ToList();
                    foreach (var item in radioButons)
                        item.IsChecked = false;
                    SelectLesion = null;
                    break;
                case "agregar_lesion":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        if (SelectIngreso == null)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso.");
                            }));
                            return;
                        };
                        if (string.IsNullOrEmpty(TextDescripcionLesion))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Debes ingresar una descripcion de la lesión.");
                            }));
                            return;
                        }
                        if (SelectRegion == null)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar la region donde se encuentra la lesión.");
                            }));
                            return;
                        };
                        if (ListLesiones == null)
                            ListLesiones = new ObservableCollection<LesionesCustom>();
                        ListLesiones.Add(new LesionesCustom { DESCR = TextDescripcionLesion, REGION = new cAnatomiaTopografica().Obtener((int)SelectRegion) });
                    });

                    break;
                case "agendar_cita":

                    break;
                case "nueva_busqueda":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = new int?();
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_visible":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CertificadoMedicoIngresoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.CertificadoMedicoIngresoViewModel();
                    break;

                case "buscar_menu":
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                        break;
                    };
                    if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        break;
                    };
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" + SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                        break;
                    };
                    SelectedIngreso = SelectIngreso;
                    StaticSourcesViewModel.SourceChanged = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    //OnBuscarPorHuella();///Inmediatamente que se valida, continua con la siguiente validacion(busqueda por biometria), comentar en caso de pruebas omitiendo biometria
                    break;
                case "guardar_menu":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                        break;
                    }
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un ingreso valido.");
                        break;
                    }


                    break;
            }
        }

        private async void ModelEnter(Object obj)
        {
            try
            {
                NombreBuscar = TextNombreImputado;
                ApellidoPaternoBuscar = TextPaternoImputado;
                ApellidoMaternoBuscar = TextMaternoImputado;
                FolioBuscar = TextFolioImputado;
                AnioBuscar = TextAnioImputado;

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

                #region Validacion Nombre y Apellidos
                if (string.IsNullOrEmpty(TextNombreImputado))
                    NombreBuscar = string.Empty;
                else
                    NombreBuscar = TextNombreImputado;

                if (string.IsNullOrEmpty(TextPaternoImputado))
                    ApellidoPaternoBuscar = string.Empty;
                else
                    ApellidoPaternoBuscar = TextPaternoImputado;

                if (string.IsNullOrEmpty(TextMaternoImputado))
                    ApellidoMaternoBuscar = string.Empty;
                else
                    ApellidoMaternoBuscar = TextMaternoImputado;

                #endregion

                if (AnioBuscar != null && FolioBuscar != null)
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            if (ListExpediente[0].INGRESO.Count == 0)
                            {
                                SelectExpediente = null;
                                SelectIngreso = null;
                                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            }
                            if (Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(a => a.HasValue ? a.Value == ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO : false))
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                                }));
                                return;
                            }
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO.HasValue ?
                                ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro : false)
                            {
                                SelectExpediente = null;
                                SelectIngreso = null;
                                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                                }));
                                return;
                            }
                            SelectExpediente = ListExpediente[0];
                            SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            ///TODO: checar si ya existe el certificado en el ingreso seleccionado
                            GetDatosImputadoSeleccionado();
                            StaticSourcesViewModel.SourceChanged = false;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        });
                    }
                    else
                    {
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                }
                else
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)//Empty row
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }

            return;
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "AnioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    AnioBuscar = int.Parse(textbox.Text);
                                else
                                    AnioBuscar = null;
                                break;
                            case "FolioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    FolioBuscar = int.Parse(textbox.Text);
                                else
                                    FolioBuscar = null;
                                break;
                        }
                    }
                }

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;

                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }

            return;
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
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

            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new List<IMPUTADO>();
            }
        }

        private void GetDatosImputadoSeleccionado()
        {
            TextAnioImputado = SelectExpediente.ID_ANIO;
            TextFolioImputado = SelectExpediente.ID_IMPUTADO;
            TextPaternoImputado = SelectExpediente.PATERNO.Trim();
            TextMaternoImputado = SelectExpediente.MATERNO.Trim();
            TextNombreImputado = SelectExpediente.NOMBRE.Trim();
            TextEdad = SelectExpediente.NACIMIENTO_FECHA.HasValue ? new Fechas().CalculaEdad(SelectExpediente.NACIMIENTO_FECHA).ToString() : string.Empty;
            SelectSexo = SelectExpediente.SEXO;
            SelectFechaNacimiento = SelectExpediente.NACIMIENTO_FECHA.HasValue ? SelectExpediente.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty;
            TextLugarNacimiento = SelectExpediente.NACIMIENTO_LUGAR;
            //TextEscolaridad = SelectExpediente.ESCOLARIDAD != null ? SelectExpediente.ESCOLARIDAD.DESCR : string.Empty;
            TextEscolaridad = SelectIngreso.ESCOLARIDAD != null ? SelectIngreso.ESCOLARIDAD.DESCR : string.Empty;
            //TextOcupacion = SelectExpediente.OCUPACION != null ? SelectExpediente.OCUPACION.DESCR : string.Empty;
            TextOcupacion = SelectIngreso.OCUPACION != null ? SelectIngreso.OCUPACION.DESCR : string.Empty;
            TextFechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToString("dd/MM/yyyy") : string.Empty;
            TextDelito = SelectIngreso.INGRESO_DELITO != null ? SelectIngreso.INGRESO_DELITO.DESCR : string.Empty;
        }

    }
    public class LesionesCustom
    {
        public string DESCR { get; set; }
        public ANATOMIA_TOPOGRAFICA REGION { get; set; }
    }
}
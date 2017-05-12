using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    partial class CatalogoSectoresViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public CatalogoSectoresViewModel()
        {
            CatalogoHeader = "Sectores";
            HeaderAgregar = "Agregar Nuevo Sector";
            //LLENAR 
            EditarVisible = false;
            NuevoVisible = false;
            AgregarVisible = false;
            GuardarMenuEnabled = false;
            EliminarMenuEnabled = false;
            EditarMenuEnabled = false;
            CancelarMenuEnabled = false;
            AyudaMenuEnabled = true;
            SalirMenuEnabled = true;
            ExportarMenuEnabled = true;
            /*MAXLENGTH*/
            MaxLength = 14;
            SeleccionIndice = -1;
            EmptyVisible = false;
            //Listado 
            ListItems = new ObservableCollection<SECTOR>();
            //ListMunicipios = new ObservableCollection<MUNICIPIO>();
            //Obtenemos los Centros
            ListMunicipios = new ObservableCollection<MUNICIPIO>(new cMunicipio().ObtenerTodos("", 2));
            SelectedMunicipio = ListMunicipios.Where(w => w.ID_MUNICIPIO == 2).FirstOrDefault();
            ListCentros = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos("", 2, SelectedMunicipio.ID_MUNICIPIO));
            SelectedCentro = ListCentros.Where(w => w.ID_CENTRO == 4).FirstOrDefault();
            ListEdificios = new ObservableCollection<EDIFICIO>(new cEdificio().ObtenerTodos("", SelectedMunicipio.ID_MUNICIPIO, SelectedCentro.ID_CENTRO));
            SelectedEdificio = ListEdificios.Where(w => w.ID_EDIFICIO > 0).ToList().FirstOrDefault();
            this.GetSectores();
            this.setValidationRules();
        }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetSectores();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        EditarVisible = true;
                        NuevoVisible = false;
                        GuardarMenuEnabled = true;
                        AgregarMenuEnabled = false;
                        EliminarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        CancelarMenuEnabled = true;
                        AyudaMenuEnabled = true;
                        SalirMenuEnabled = true;
                        ExportarMenuEnabled = false;
                        AgregarVisible = true;
                        bandera_editar = true;
                        FocusText = true;
                        #region Obtener Valores
                        Clave = SelectedItem.ID_CENTRO;
                        Descripcion = SelectedItem.DESCR;
                        Centro = SelectedItem.ID_CENTRO;
                        Edificio = SelectedItem.ID_EDIFICIO;
                        SelectedMunicipio = ListMunicipios.Where(w => w.ID_MUNICIPIO == SelectedItem.EDIFICIO.CENTRO.ID_MUNICIPIO).FirstOrDefault();
                        SelectedCentro = ListCentros.Where(w => w.ID_CENTRO == SelectedItem.ID_CENTRO).FirstOrDefault();
                        SelectedEdificio = ListEdificios.Where(w => w.ID_EDIFICIO == SelectedItem.ID_EDIFICIO).FirstOrDefault();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).FirstOrDefault();
                        ImagenGuardarPlanimetria = new Imagenes().ConvertByteToBitmap(SelectedItem.PLANO);
                        Planimetria = SelectedItem.PLANO;
                        #endregion Obtener Valores
                    }
                    else
                    {
                        bandera_editar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una opción.");
                    }
                    break;
                case "menu_agregar":
                    EditarVisible = false;
                    NuevoVisible = true;
                    GuardarMenuEnabled = true;
                    AgregarMenuEnabled = false;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = true;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = false;
                    AgregarVisible = true;
                    bandera_editar = false;
                    FocusText = true;
                    /********************************/
                    SeleccionIndice = -1;
                    Clave = 0;
                    Descripcion = "";
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    ImagenGuardarPlanimetria = null;
                    Planimetria = null;
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Descripcion) && SelectedEstatus != null)
                    {
                        EditarVisible = false;
                        NuevoVisible = false;
                        GuardarMenuEnabled = false;
                        AgregarMenuEnabled = true;
                        EliminarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        AyudaMenuEnabled = true;
                        SalirMenuEnabled = true;
                        ExportarMenuEnabled = true;
                        AgregarVisible = false;
                        /**********************************/
                        this.GuardarSectores();
                        /**********************************/
                    }
                    else
                        FocusText = true;
                    break;
                case "menu_cancelar":
                    NuevoVisible = false;
                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    AgregarVisible = false;
                    SeleccionIndice = -1;
                    /****************************************/
                    Clave = 0;
                    Descripcion = "";
                    /****************************************/
                    break;
                case "menu_eliminar":
                    var metro = Application.Current.Windows[0] as MetroWindow;
                    if (SelectedItem != null)
                    {
                        if (ValidarEliminar())
                        {
                            var mySettings = new MetroDialogSettings()
                            {
                                AffirmativeButtonText = "Aceptar",
                                NegativeButtonText = "Cancelar",
                                AnimateShow = true,
                                AnimateHide = false
                            };
                            var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.DESCR.Trim() + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                            if (result == MessageDialogResult.Affirmative)
                            {
                                if (this.Eliminar())
                                {
                                    var dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                                    await metro.ShowMetroDialogAsync(dialog);
                                    await TaskEx.Delay(1500);
                                    await metro.HideMetroDialogAsync(dialog);
                                }
                                else
                                {
                                    mySettings = new MetroDialogSettings()
                                    {
                                        AffirmativeButtonText = "Aceptar"
                                    };
                                    await metro.ShowMessageAsync("Algo ocurrió...", "No se puede eliminar sector: Tiene dependencias.", MessageDialogStyle.Affirmative, mySettings);
                                    await TaskEx.Delay(1500);
                                }
                            }
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación", "No puede eliminar el sector, tiene información relacionada");
                        }
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opción");
                    SeleccionIndice = -1;
                    break;
                case "menu_exportar":
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    break;
                case "menu_ayuda":
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    break;
                case "menu_salir":
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetSectores();
            }
        }

        private void GetSectores()
        {
            try
            {
                cSector objSector = new cSector();
                ListItems = new ObservableCollection<SECTOR>(objSector.ObtenerTodos(Busqueda, SelectedMunicipio.ID_MUNICIPIO, SelectedCentro.ID_CENTRO, SelectedEdificio.ID_EDIFICIO));
                if (ListItems.Count > 0)
                    EmptyVisible = false;
                else
                    EmptyVisible = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GuardarSectores()
        {
            try
            {
                if (Clave > 0)
                {  //Actualizar
                    obj.Actualizar(new SECTOR()
                    {
                        DESCR = Descripcion,
                        ID_CENTRO = SelectedCentro.ID_CENTRO,
                        ID_EDIFICIO = SelectedEdificio.ID_EDIFICIO,
                        ID_SECTOR = SelectedItem.ID_SECTOR,
                        PLANO = Planimetria,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                else
                {   //Agregar
                    obj.Insertar(new SECTOR()
                    {
                        DESCR = Descripcion,
                        ID_CENTRO = SelectedCentro.ID_CENTRO,
                        ID_EDIFICIO = SelectedEdificio.ID_EDIFICIO,
                        PLANO = Planimetria,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                ImagenGuardarPlanimetria = null;
                Planimetria = null;
                //Mostrar Listado
                GetSectores();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool Eliminar()
        {
            try
            {
                if (SelectedItem != null)
                {
                    if (!obj.Eliminar(Convert.ToInt32(SelectedItem.ID_SECTOR)))
                        return false;
                    GetSectores();
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        private void ElegirImagenGuardar(object obj)
        {
            StaticSourcesViewModel.ShowLoading = Visibility.Hidden;
            if (!(obj is System.Windows.Controls.Image))
                return;

            var op = new System.Windows.Forms.OpenFileDialog();
            op.Title = "Seleccione una imagen";
            op.Filter = "Formatos Validos|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ((System.Windows.Controls.Image)obj).Source = null;
                Planimetria = null;

                if (new System.IO.FileInfo(op.FileName).Length > 5000000)
                    StaticSourcesViewModel.Mensaje("Imagen no soportada", "El archivo debe ser de menos de 5 Mb", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                else
                {
                    var rawimage = new System.Windows.Media.Imaging.BitmapImage(new Uri(op.FileName));
                    //((System.Windows.Controls.Image)obj).Source = rawimage;
                    ImagenGuardarPlanimetria = rawimage;
                    Planimetria = new Imagenes().ConvertBitmapToByte(rawimage);
                }
            }
        }

        private bool ValidarEliminar()
        {
            if (SelectedItem.CELDA != null)
            {
                if (SelectedItem.CELDA.Count > 0)
                    return false;
                if (SelectedItem.SECTOR_OBSERVACION.Count > 0)
                    return false;
            }
            return true;
        }

        //LOAD
        private async void SectoresLoad(CatalogoSectoresView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<SECTOR>();
                    CatalogoHeader = "Sectores";
                    HeaderAgregar = "Agregar Sectores";
                    //LLENAR 
                    EditarVisible = false;
                    NuevoVisible = false;
                    AgregarVisible = false;
                    GuardarMenuEnabled = false;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    /*MAXLENGTH*/
                    MaxLength = 14;
                    SeleccionIndice = -1;
                    //Obtenemos los sectores
                    this.GetSectores();
                    this.setValidationRules();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de sector.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_SECTORES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                    {
                        AgregarMenuEnabled = true;
                        DataGridEnabled = true;
                    }
                    if (p.CONSULTAR == 1)
                    {
                        BuscarHabilitado = true;
                        TextoHabilitado = true;
                        MunicipioHabilitado = true;
                        CentroHabilitado = true;
                        EdificioHabilitado = true;
                    }
                    if (p.EDITAR == 1)
                        EditarEnabled = true;
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
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
    partial class CatalogoModalidadDelitoViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        cModalidadDelito obj = new cModalidadDelito();
        cTipoDelito objTipo = new cTipoDelito();

        public CatalogoModalidadDelitoViewModel() { }

        void IPageViewModel.inicializa()
        {
            CatalogoHeader = "Modalidad Delito";
            HeaderAgregar = "Agregar Nueva Modalidad Delito";
            FiltroDisplay = "DESCR";
            ComboBoxLigado = "Tipo Delito";
            //LLENAR 
            EditarVisible = false;
            NuevoVisible = false;
            FiltroVisible = true;
            ListItems = new ObservableCollection<MODALIDAD_DELITO>();
            ListTipos = new ObservableCollection<TIPO_DELITO>(objTipo.ObtenerTodos());
            ListTipos.Insert(0, new TIPO_DELITO() { ID_TIPO_DELITO = 0, DESCR = "SELECCIONE" });
            SelectedTipo = ListTipos.Where(w => w.DESCR == "SELECCIONE").FirstOrDefault();
            AgregarVisible = false;
            GuardarMenuEnabled = false;
            AgregarMenuEnabled = true;
            EliminarMenuEnabled = false;
            EditarMenuEnabled = false;
            CancelarMenuEnabled = false;
            AyudaMenuEnabled = true;
            SalirMenuEnabled = true;
            ExportarMenuEnabled = true;
            SeleccionIndice = -1;
            this.setValidationRules();
        }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetModalidadDelito();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Modalidad Delito";
                        #region visiblePantalla
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
                        #endregion
                        bandera_editar = true;
                        FocusText = true;
                        /*****************************************/
                        Clave = SelectedItem.ID_MODALIDAD;
                        Descripcion = SelectedItem.DESCR;
                        //Tipo = SelectedItem.ID_TIPO_DELITO;
                        // SelectedTipo = ListTipos.Where(w=>w.ID_TIPO_DELITO == SelectedItem.ID_TIPO_DELITO).FirstOrDefault();
                        /*****************************************/
                    }
                    else
                    {
                        bandera_editar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una opción.");
                    }
                    break;
                case "menu_agregar":
                    HeaderAgregar = "Agregar Modalidad Delito";
                    #region visiblePantalla
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
                    #endregion
                    bandera_editar = false;
                    FocusText = true;
                    /********************************/
                    SeleccionIndice = -1;
                    Clave = 0;
                    Descripcion = "";
                    Tipo = 0;
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Descripcion))
                    {
                        #region visiblePantalla
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
                        #endregion
                        /**********************************/
                        this.Guardar();
                        /**********************************/
                    }
                    else
                        FocusText = true;
                    break;
                case "menu_cancelar":
                    #region visiblePantalla
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
                    #endregion
                    /****************************************/
                    SeleccionIndice = -1;
                    Clave = 0;
                    Descripcion = string.Empty;
                    Tipo = 0;
                    this.GetModalidadDelito();
                    /****************************************/
                    break;
                case "menu_eliminar":
                    var metro = Application.Current.Windows[0] as MetroWindow;
                    if (SelectedItem != null)
                    {
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Aceptar",
                            NegativeButtonText = "Cancelar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        var result = await metro.ShowMessageAsync("Eliminar", "¿Está seguro que desea eliminar... [ " + SelectedItem.DESCR + " ]?", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            BaseMetroDialog dialog;
                            if (this.Eliminar())
                            {
                                dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                            }
                            else
                            {
                                dialog = (BaseMetroDialog)metro.Resources["ErrorDialog"];
                            }
                            await metro.ShowMetroDialogAsync(dialog);
                            await TaskEx.Delay(1500);
                            await metro.HideMetroDialogAsync(dialog);
                        }
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opción");
                    SeleccionIndice = -1;
                    break;
                case "menu_exportar":
                    SeleccionIndice = -1;
                    break;
                case "menu_ayuda":
                    SeleccionIndice = -1;
                    break;
                case "menu_salir":
                    SeleccionIndice = -1;
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetModalidadDelito();
            }
        }

        private void GetModalidadDelito()
        {
            try
            {
                ListItems.Clear();
                // ListItems = obj.ObtenerTodos(Busqueda, SelectedTipo.ID_TIPO_DELITO);
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

        private void Guardar()
        {
            try
            {
                if (Clave > 0)
                {  //Actualizar
                    //obj.Actualizar(new MODALIDAD_DELITO
                    //{
                    //    ID_MODALIDAD = SelectedItem.ID_MODALIDAD,
                    //    DESCR = Descripcion,
                    //    ID_TIPO_DELITO = SelectedTipo.ID_TIPO_DELITO
                    //});
                }
                else
                {   //Agregar
                    //obj.Insertar(new MODALIDAD_DELITO
                    //{
                    //    ID_MODALIDAD = 0,
                    //    DESCR = Descripcion,
                    //    ID_TIPO_DELITO = SelectedTipo.ID_TIPO_DELITO
                    //});
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                Tipo = 0;
                //Mostrar Listado
                this.GetModalidadDelito();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool Eliminar()
        {
            //if (SelectedItem != null)
            //{
            //    if (!obj.Eliminar(SelectedItem.ID_MODALIDAD))
            //        return false;
            //    Clave = 0;
            //    Descripcion = string.Empty;
            //    Tipo = 0;
            //    this.GetModalidadDelito();
            //}
            return true;
        }
    }
}
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    partial class CatalogoProgramasViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        public CatalogoProgramasViewModel()
        {
            //EmptyVisible = false;
            //Listado 
            ListItems = new List<TIPO_PROGRAMA>();
            CatalogoHeader = "Tipo Programa";
            HeaderAgregar = "Agregar Tipo Programa";
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
            MaxLength = 5;
            SeleccionIndice = -1;
            //Obtenemos los programas
            this.GetProgramas();
            this.setValidationRules();
        }

        //private async void Load(object obj)
        //{
        //    CatBusqueda = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<DEPARTAMENTO>>(() => new ObservableCollection<DEPARTAMENTO>(new cDepartamentos().ObtenerTodos().OrderBy(o => o.DESCR)));
        //    CatBusqueda.Insert(0, new DEPARTAMENTO() { DESCR = "TODOS" });
        //}

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    GetProgramas();
                    break;
                case "menu_editar":
                    if (SelectedItem == null)
                        break;
                    PanelVisible = true;
                    AccionHeader = "Editar Programa";
                    CancelarMenuEnabled = true;
                    ListTipos = ListTipos ?? await StaticSourcesViewModel.CargarDatosAsync<List<DEPARTAMENTO>>(() => new List<DEPARTAMENTO>(new cDepartamentos().ObtenerTodos().OrderBy(o => o.DESCR)));
                    SelectedTipo = SelectedItem.ID_DEPARTAMENTO;
                    Nombre = SelectedItem.NOMBRE;
                    Clave = SelectedItem.ID_TIPO_PROGRAMA;
                    //Corta al campo descripcion los espacios restantes.
                    Descripcion = SelectedItem.DESCR;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                    CancelarMenuEnabled = true;
                    GuardarMenuEnabled = true;
                    break;
                case "menu_agregar":
                    PanelVisible = true;
                    AccionHeader = "Agregar Programa";
                    ListTipos = ListTipos ?? await StaticSourcesViewModel.CargarDatosAsync<List<DEPARTAMENTO>>(() => new List<DEPARTAMENTO>(new cDepartamentos().ObtenerTodos().OrderBy(o => o.DESCR)));
                    SelectedIndex = -1;
                    Nombre = string.Empty;
                    Descripcion = string.Empty;
                    SelectedTipo = -1;
                    SelectedItem = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    CancelarMenuEnabled = true;
                    GuardarMenuEnabled = true;
                    break;
                case "menu_guardar":
                    if (SelectedEstatus != null && !string.IsNullOrEmpty(Descripcion) && !string.IsNullOrEmpty(Nombre))
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
                        PanelVisible = false;
                        #endregion
                        /**********************************/
                        this.GuardarTipoPrograma();
                        /**********************************/
                    }
                    break;
                case "menu_cancelar":
                    PanelVisible = false;
                    //SelectedIndex = -1;
                    Nombre = string.Empty;
                    SelectedTipo = -1;
                    Descripcion = string.Empty;
                    SelectedItem = null;
                    CancelarMenuEnabled = false;
                    GuardarMenuEnabled = false;
                    break;
                case "menu_eliminar":
                    if (SelectedItem == null)
                        break;
                    if (await new Dialogos().ConfirmarEliminar("Confirmación de eliminación", "Esta seguro de eliminar este registro?") != 1)
                        break;
                    if (new cPrograma().Delete(new TIPO_PROGRAMA { DESCR = SelectedItem.DESCR, ID_DEPARTAMENTO = SelectedItem.ID_DEPARTAMENTO, ID_TIPO_PROGRAMA = SelectedItem.ID_TIPO_PROGRAMA, NOMBRE = SelectedItem.NOMBRE }))
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Programa", "Eliminado Exitosamente");
                        SelectedcatTipo = SelectedItem.ID_DEPARTAMENTO.Value;
                        SelectedItem = null;
                        GetProgramas();
                    }
                    break;
                case "menu_exportar":
                    break;
                case "menu_ayuda":
                    break;
                case "menu_salir":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetProgramas();
            }
        }

        private void GetProgramas()
        {
            try
            {
                ListItems.Clear();
                ListItems = new List<TIPO_PROGRAMA>( new cPrograma().ObtenerTodos(Busqueda, SelectedcatTipo != -1 ? (short?)SelectedcatTipo : null));
                if (ListItems.Count > 0)

                    EmptyVisible = false;

                else
                    EmptyVisible = true;
                //EmptyVisible = ListItems.Any() ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }


        private void GuardarTipoPrograma()
        {
            try
            {
                cPrograma programa = new cPrograma();
                if (SelectedItem != null)
                {  //Actualizar
                    SelectedItem.ID_TIPO_PROGRAMA = Clave;
                    SelectedItem.DESCR = Descripcion;
                    SelectedItem.NOMBRE = Nombre;
                    SelectedItem.ESTATUS = SelectedEstatus.CLAVE;
                    SelectedItem.ID_DEPARTAMENTO = SelectedTipo;
                    programa.Actualizar(new TIPO_PROGRAMA
                    {
                        ID_TIPO_PROGRAMA = Clave,
                        NOMBRE = Nombre,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE,
                        ID_DEPARTAMENTO = SelectedTipo
                    });
                }
                else
                {   //Agregar
                    programa.Insertar(new TIPO_PROGRAMA
                    {
                        ID_TIPO_PROGRAMA = Clave,
                        NOMBRE = Nombre,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE,
                        ID_DEPARTAMENTO = SelectedTipo
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                Nombre = string.Empty;
                SelectedTipo = -1;
                SelectedEstatus = null;
                Busqueda = string.Empty;
                //Mostrar Listado
                this.GetProgramas();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        //LOAD
        private async void ProgramaLoad(CatalogoProgramasView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    PanelVisible = false;
                    //Listado 
                    ListItems = new List<TIPO_PROGRAMA>();

                    CatalogoHeader = "Tipo Programa";
                    HeaderAgregar = "Agregar Tipo Programa";
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
                    //Obtenemos los Departamentos
                    ListTipos = new List<DEPARTAMENTO>(new cDepartamentos().ObtenerTodos(string.Empty,"S"));
                    ListTiposFiltros = new List<DEPARTAMENTO>(ListTipos);
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListTipos.Insert(0, new DEPARTAMENTO() { ID_DEPARTAMENTO = -1, DESCR = "SELECCIONE" });
                        ListTiposFiltros.Insert(0, new DEPARTAMENTO() { ID_DEPARTAMENTO = -1, DESCR = "TODO" });
                    }));
                    //Obtenemos los programas
                    GetProgramas();
                    setValidationRules();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_TIPO_PROGRAMA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
                    if (p.CONSULTAR == 1)
                    {
                        BuscarHabilitado = true;
                        DepartamentoHabilitado = true;
                        TextoHabilitado = true;
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
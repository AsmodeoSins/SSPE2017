using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlPenales.Clases.Estatus;

namespace ControlPenales
{
    partial class CatalogoActividadEjeViewModel : ValidationViewModelBase
    {

        #region Metodos
        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    if (SelectedEjeBusqueda == -1)
                        Buscar();
                    else
                        Buscar(SelectedEjeBusqueda);
                    break;
                case "menu_editar":
                    if (SelectedItem!=null)
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea empezar a editar sin guardar?");
                            if (dialogresult != 0)
                                StaticSourcesViewModel.SourceChanged = false;
                            else
                                return;
                        }
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w=>w.CLAVE==SelectedItem.ESTATUS);
                        SelectedEje = SelectedItem.ID_EJE;
                        SelectedActividad = LstActividades.FirstOrDefault(w => w.ID_TIPO_PROGRAMA == SelectedItem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == SelectedItem.ID_ACTIVIDAD);
                        GuardarMenuEnabled = true;
                        AgregarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        AgregarVisible = true;
                        Bandera_Agregar = false;
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                    {
                        Bandera_Agregar = true;
                        new Dialogos().ConfirmacionDialogo("Validación!", "Seleccione una activdad de un eje primero");
                    }
                    
                    break;
                case "menu_agregar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea empezar un nuevo registro sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                    GuardarMenuEnabled = true;
                    AgregarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    AgregarVisible = true;
                    Bandera_Agregar = true;
                    Limpiar();
                    setValidaciones();
                    break;
                case "menu_guardar":
                    if (base.HasErrors)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de llenar los campos obligatorios. " + base.Error);
                        return;
                    }
                    if (Bandera_Agregar && ListItems.Any(w=>w.ID_ACTIVIDAD==SelectedActividad.ID_ACTIVIDAD && w.ID_TIPO_PROGRAMA==SelectedActividad.ID_TIPO_PROGRAMA && 
                        w.ID_EJE==SelectedEje))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No se puede repetir la misma actividad en el eje");
                        return;
                    }
                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    AgregarVisible = false;
                    /**********************************/
                    await Guardar();
                    Buscar();
                    StaticSourcesViewModel.SourceChanged = false;
                    /**********************************/
                    break;
                case "menu_cancelar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea cancelar sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EditarMenuEnabled = false;
                    AgregarVisible = false;
                    /****************************************/
                    Limpiar();
                    StaticSourcesViewModel.SourceChanged = false;
                    /****************************************/
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

        private void Limpiar()
        {
            SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w => w.CLAVE == "-1");
            SelectedEje = -1;
            SelectedActividad = lstActividades.FirstOrDefault(w => w.ID_ACTIVIDAD == -1 && w.ID_TIPO_PROGRAMA == -1);
            StaticSourcesViewModel.SourceChanged = false;
        }
        

        private async void PageLoad(CatalogoActividadEjeView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ListItems = new ObservableCollection<ACTIVIDAD_EJE>(new cActividadEje().ObtenerTodos(null,null));
                    lstEjes = new ObservableCollection<EJE>(new cEje().ObtenerTodos(string.Empty));
                    lstEjes.Insert(0,new EJE{
                        ID_EJE=-1,
                        DESCR="SELECCIONE"
                    });
                    RaisePropertyChanged("LstEjes");
                    lstActividades=new ObservableCollection<ACTIVIDAD>(new cActividad().ObtenerTodos());
                    lstActividades.Insert(0,new ACTIVIDAD{
                        ID_ACTIVIDAD=-1,
                        ID_TIPO_PROGRAMA=-1,
                        DESCR="SELECCIONE"
                    });
                    RaisePropertyChanged("LstActividades");
                    _lista_Estatus.LISTA_ESTATUS.Insert(0,new Estatus
                    {
                        CLAVE = "-1",
                        DESCRIPCION = "SELECCIONE"
                    });
                    RaisePropertyChanged("Lista_Estatus");
                    //LLENAR 
                    GuardarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    SelectedActividad = lstActividades.FirstOrDefault(w => w.ID_ACTIVIDAD == -1 && w.ID_TIPO_PROGRAMA == -1);
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w=>w.CLAVE=="-1");
                    ConfiguraPermisos();
                });

                setValidaciones();
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información.", ex);
            }
        }
        #endregion


        private async Task Guardar()
        {
            try
            {
                var obj = new ACTIVIDAD_EJE {
                    ID_ACTIVIDAD=selectedActividad.ID_ACTIVIDAD,
                    ID_TIPO_PROGRAMA=selectedActividad.ID_TIPO_PROGRAMA,
                    ID_EJE=SelectedEje,
                    FECHA=Fechas.GetFechaDateServer,
                    ESTATUS=SelectedEstatus.CLAVE
                };
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    if (!Bandera_Agregar)
                    {
                        new cActividadEje().Actualizar(obj);
                        Bandera_Agregar = true;
                    }
                    else
                    {
                        new cActividadEje().Insertar(obj);
                    }
                });
                new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente.");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la actividad del eje.", ex);

            }
        }


        private async void Buscar(short? id_eje=null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    _listItems = new ObservableCollection<ACTIVIDAD_EJE>(new cActividadEje().ObtenerTodos(id_eje));
                    RaisePropertyChanged("ListItems");
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar las actividades de ejes.", ex);
            }
        }



        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CATALOGOACTIVIDADEJE.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
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

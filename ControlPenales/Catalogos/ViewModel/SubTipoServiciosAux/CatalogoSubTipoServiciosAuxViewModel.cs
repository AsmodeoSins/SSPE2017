using ControlPenales.Clases.Estatus;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPenales
{
    partial class CatalogoSubTipoServiciosAuxViewModel:ValidationViewModelBase
    {
        #region Metodos
        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    Buscar(SelectedTipoServAuxBusqueda != -1 ? (short?)SelectedTipoServAuxBusqueda : null);
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea empezar a editar sin guardar?");
                            if (dialogresult != 0)
                                StaticSourcesViewModel.SourceChanged = false;
                            else
                                return;
                        }
                        SelectedTipoServAux = SelectedItem.ID_TIPO_SADT.Value;
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w => w.CLAVE == SelectedItem.ESTATUS);
                        TextSubTipoServAux = SelectedItem.DESCR;
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
                        new Dialogos().ConfirmacionDialogo("Validación!", "Seleccione un tipo de servicio auxiliar primero");
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
            SelectedTipoServAux = -1;
            TextSubTipoServAux = string.Empty;
            SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w => w.CLAVE == "-1");
            StaticSourcesViewModel.SourceChanged = false;
        }


        private async void PageLoad(CatalogoSubTipoServiciosAuxView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    _listItems = new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos());
                    RaisePropertyChanged("ListItems");
                    var res = new cTipo_Serv_Aux_Diag_Trat().ObtenerTodos(string.Empty,"S");
                    lstTipoServAux = new ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT>(res);
                    lstTipoServAux.Insert(0,new TIPO_SERVICIO_AUX_DIAG_TRAT {
                        ID_TIPO_SADT=-1,
                        DESCR="SELECCIONE"
                    });
                    RaisePropertyChanged("LstTipoServAux");
                    lstTipoServAuxBusqueda = new ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT>(res);
                    lstTipoServAuxBusqueda.Insert(0, new TIPO_SERVICIO_AUX_DIAG_TRAT {
                        ID_TIPO_SADT=-1,
                        DESCR="TODOS"
                    });
                    RaisePropertyChanged("LstTipoServAuxBusqueda");
                    _lista_Estatus.LISTA_ESTATUS.Insert(0, new Estatus
                    {
                        CLAVE = "-1",
                        DESCRIPCION = "SELECCIONE"
                    });
                    RaisePropertyChanged("Lista_Estatus");
                    if (_listItems == null || _listItems.Count == 0)
                    {
                        emptyVisible = true;
                        RaisePropertyChanged("EmptyVisible");
                    }
                    //LLENAR 
                    GuardarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w => w.CLAVE == "-1");
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
                var obj = new SUBTIPO_SERVICIO_AUX_DIAG_TRAT
                {
                    ID_SUBTIPO_SADT = SelectedItem != null ? SelectedItem.ID_SUBTIPO_SADT : (short)0,
                    ID_TIPO_SADT=SelectedTipoServAux,
                    DESCR = TextSubTipoServAux,
                    ESTATUS = SelectedEstatus.CLAVE
                };
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    if (!Bandera_Agregar)
                    {
                        new cSubtipo_Serv_Aux_Diag_Trat().Actualizar(obj);
                        Bandera_Agregar = true;
                    }
                    else
                    {
                        new cSubtipo_Serv_Aux_Diag_Trat().Insertar(obj);
                    }
                });
                new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente.");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el tipo de servicio auxiliar.", ex);

            }
        }


        private async void Buscar(short? tipo_serv_aux=null )
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    _listItems = new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(tipo_serv_aux));
                    RaisePropertyChanged("ListItems");
                    if (_listItems == null || _listItems.Count == 0)
                        emptyVisible = true;
                    else
                        emptyVisible = false;
                    RaisePropertyChanged("EmptyVisible");
                });

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar los tipos de servicios auxiliares.", ex);
            }
        }



        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CATALOGOSUBTIPOSERVICIOSAUX.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

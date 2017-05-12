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
    partial class CatalogoServiciosAuxiliaresViewModel:ValidationViewModelBase
    {
        #region Metodos
        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    Buscar(SelectedTipoServAuxBusqueda != -1 ? (short?)SelectedTipoServAuxBusqueda : null, SelectedSubtipoServAuxBusqueda!=-1?(short?)SelectedSubtipoServAuxBusqueda:null);
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
                        if (LstTipoServAux != null && LstTipoServAux.Any(w => w.ID_TIPO_SADT == SelectedItem.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.ID_TIPO_SADT))
                            SelectedTipoServAux = SelectedItem.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.ID_TIPO_SADT.Value;
                        else
                            SelectedTipoServAux = -1;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                            lstSubtipoServAux = new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(SelectedTipoServAux,"S"));
                            lstSubtipoServAux.Insert(0, new SUBTIPO_SERVICIO_AUX_DIAG_TRAT {
                                ID_SUBTIPO_SADT=-1,
                                DESCR="SELECCIONE"
                            });
                            RaisePropertyChanged("LstSubtipoServAux");
                            if (lstSubtipoServAux != null && lstSubtipoServAux.Any(w => w.ID_SUBTIPO_SADT == SelectedItem.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.ID_SUBTIPO_SADT))
                                selectedSubtipoServAux = SelectedItem.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.ID_SUBTIPO_SADT;
                            else
                                selectedSubtipoServAux = -1;
                            RaisePropertyChanged("SelectedSubtipoServAux");
                        });
                        
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w => w.CLAVE == SelectedItem.ESTATUS);
                        TextServAux = SelectedItem.DESCR;
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
            SelectedSubtipoServAux = -1;
            TextServAux = string.Empty;
            SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w => w.CLAVE == "-1");
            StaticSourcesViewModel.SourceChanged = false;
        }


        private async void PageLoad(CatalogoServiciosAuxiliaresView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    _listItems = new ObservableCollection<SERVICIO_AUX_DIAG_TRAT>(new cServ_Aux_Diag_Trat().ObtenerTodos());
                    RaisePropertyChanged("ListItems");
                    var res = new cTipo_Serv_Aux_Diag_Trat().ObtenerTodos();
                    lstTipoServAux = new ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT>(res);
                    lstTipoServAux.Insert(0, new TIPO_SERVICIO_AUX_DIAG_TRAT
                    {
                        ID_TIPO_SADT = -1,
                        DESCR = "SELECCIONE"
                    });
                    RaisePropertyChanged("LstTipoServAux");
                    lstTipoServAuxBusqueda = new ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT>(res);
                    lstTipoServAuxBusqueda.Insert(0, new TIPO_SERVICIO_AUX_DIAG_TRAT
                    {
                        ID_TIPO_SADT = -1,
                        DESCR = "TODOS"
                    });
                    RaisePropertyChanged("LstTipoServAuxBusqueda");
                    var res2 = new cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(-1);
                    lstSubtipoServAux = new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(res2);
                    lstSubtipoServAux.Insert(0, new SUBTIPO_SERVICIO_AUX_DIAG_TRAT {
                        ID_SUBTIPO_SADT=-1,
                        DESCR="SELECCIONE"
                    });
                    RaisePropertyChanged("LstSubtipoServAux");
                    lstSubtipoServAuxBusqueda = new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(res2);
                    lstSubtipoServAuxBusqueda.Insert(0, new SUBTIPO_SERVICIO_AUX_DIAG_TRAT {
                        ID_SUBTIPO_SADT=-1,
                        DESCR="TODOS"
                    });
                    RaisePropertyChanged("LstSubtipoServAuxBusqueda");
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

        private async void CambioModelo(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_tipo_serv_aux":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            lstSubtipoServAux = new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(SelectedTipoServAux, "S"));
                            lstSubtipoServAux.Insert(0, new SUBTIPO_SERVICIO_AUX_DIAG_TRAT
                            {
                                ID_SUBTIPO_SADT = -1,
                                DESCR = "SELECCIONE"
                            });
                            RaisePropertyChanged("LstSubtipoServAux");
                            selectedSubtipoServAux = -1;
                            RaisePropertyChanged("SelectedSubtipoServAux");
                        });
                            
                        break;
                    case "cambio_tipo_serv_aux_busqueda":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            lstSubtipoServAuxBusqueda = new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(SelectedTipoServAuxBusqueda, "S"));
                            lstSubtipoServAuxBusqueda.Insert(0, new SUBTIPO_SERVICIO_AUX_DIAG_TRAT
                            {
                                ID_SUBTIPO_SADT = -1,
                                DESCR = "TODOS"
                            });
                            RaisePropertyChanged("LstSubtipoServAuxBusqueda");
                            selectedSubtipoServAuxBusqueda = -1;
                            RaisePropertyChanged("SelectedSubtipoServAuxBusqueda");
                        }).ContinueWith((prevTask) => {
                           
                        });

                        break;
                }
            }
        }
        #endregion


        private async Task Guardar()
        {
            try
            {
                var obj = new SERVICIO_AUX_DIAG_TRAT
                {
                    ID_SERV_AUX=SelectedItem!=null?SelectedItem.ID_SERV_AUX:(short)0,
                    ID_SUBTIPO_SADT = SelectedSubtipoServAux,
                    DESCR = TextServAux,
                    ESTATUS = SelectedEstatus.CLAVE
                };
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    if (!Bandera_Agregar)
                    {
                        new cServ_Aux_Diag_Trat().Actualizar(obj);
                        Bandera_Agregar = true;
                    }
                    else
                    {
                        new cServ_Aux_Diag_Trat().Insertar(obj);
                    }
                });
                new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente.");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el servicio auxiliar.", ex);

            }
        }


        private async void Buscar(short? tipo_serv_aux = null, short? subtipo_serv_aux=null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    _listItems = new ObservableCollection<SERVICIO_AUX_DIAG_TRAT>(new cServ_Aux_Diag_Trat().ObtenerTodos(tipo_serv_aux,subtipo_serv_aux));
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CATALOGOSERVICIOSAUXILIARES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

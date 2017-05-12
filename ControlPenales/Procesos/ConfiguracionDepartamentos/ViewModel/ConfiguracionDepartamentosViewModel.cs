using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Reporting.WinForms;
using System.Windows.Controls;
using LinqKit;
using SSP.Controlador.Catalogo.Justicia.OrganizacionInterna;
using System.Threading.Tasks;
using System.Transactions;
using ControlPenales.Clases.ConfigDepartamentos;

namespace ControlPenales
{

    partial class ConfiguracionDepartamentosViewModel : ValidationViewModelBase
    {
        cDepartamentoAcceso DepAccesoControlador = new cDepartamentoAcceso();
        cCentro CentroControlador = new cCentro();
        cUsuario UsuarioControlador = new cUsuario();
        DateTime? FechaServer = Fechas.GetFechaDateServer;

        private async void clickSwitch(object op)
        {
            switch (op.ToString())
            {
                #region Departamento Seleccionado
                case "editar_departamento":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }

                    if (SelectDep != null)
                    {
                        if (SelectDep.ID_ROL != null)
                        {
                            if (StaticSourcesViewModel.SourceChanged)
                            {
                                var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea cambiar de departamento sin guardar?");
                                if (dialogresult != 0)
                                    StaticSourcesViewModel.SourceChanged = false;
                                else
                                    return;
                            }
                            PopularCoordinadoresDepSeleccionado();
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No puede asignar un coordinador porque no hay un rol de coordinación asignado al departamento..");
                            OcultarTodoGrupoCoordinadoresDep();
                        }

                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un departamento para realizar esta accion.");
                        OcultarTodoGrupoCoordinadoresDep();
                    }
                    break;
                #endregion

                #region Coordinadores de departamento Seleccionado
                case "Asignar_Coordinador":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    Guardar_Editar = "Guardar";
                    // ------------------------------- Validacion Seleccionar un departamento  para proceder con registro-------------------------------------------------
                    if (SelectDep != null)
                    {
                        GroupCentroSeleccionado = Visibility.Visible;
                        BotonGuardarVisibilty = Visibility.Visible;
                        VisibilityBotonCancelar = Visibility.Visible;
                        LimpiarControles();
                        OperacionActivaEnable = false;//Deshabilita Grid Departamento y COORDINADOR Seleccionado
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un departamento para realizar esta accion.");
                    }

                    break;
                case "editar_Coordinador":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    Guardar_Editar = "Editar";
                    if (SelectedCoordinadoresAsignados != null)
                    {
                        LimpiarControles();
                        OperacionActivaEnable = false;

                        PopularCentroSeleccionado();

                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un coordinador para editar un registro.");
                    }
                    break;

                case "eliminar_Coordinador":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    if (SelectedCoordinadoresAsignados != null)
                    {
                        if (await new Dialogos().ConfirmarEliminar("Confirmación de eliminación", "Esta seguro de eliminar al coordinador del departamento: " + SelectDep.DESCR + "?") != 1)
                            break;
                        try
                        {


                            if (new cDepartamentoAcceso().Eliminar((new DEPARTAMENTO_ACCESO()
                            {
                                ID_DEPARTAMENTO = SelectedCoordinadoresAsignados.OBJETO_DEPARTAMENTO_ACCESO.ID_DEPARTAMENTO,
                                ID_USUARIO = SelectedCoordinadoresAsignados.OBJETO_DEPARTAMENTO_ACCESO.ID_USUARIO,
                                REGISTRO_FEC = SelectedCoordinadoresAsignados.OBJETO_DEPARTAMENTO_ACCESO.REGISTRO_FEC
                            })))
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Éxito", "Registro Eliminado");
                                PopularCoordinadoresDepSeleccionado();
                            }
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Editar", ex);
                        }
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un coordinador para eliminar un registro.");
                    }
                    
                    break;
                #endregion

              
                case "guardar":
                    string ProcesoTipo = string.Empty;
                    bool EXITO_PROCESO = false;
                    if (base.HasErrors)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de llenar todos los campo Obligatorios.");
                    }
                    else
                    {


                        if (Guardar_Editar == "Guardar")
                        {
                            if (!pInsertar)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                                break;
                            }

                            ProcesoTipo = "Guardar";
                            if (SelectedCoordinador.ID_EMPLEADO > -1)
                            {//inicio
                                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                                {
                                    try
                                    {
                                        if (new cDepartamentoAcceso().Insertar(new DEPARTAMENTO_ACCESO()
                                        {
                                            ID_DEPARTAMENTO = SelectDep.ID_DEPARTAMENTO,
                                            ID_USUARIO = UsuarioControlador.Obtener(SelectedCoordinador.ID_EMPLEADO).FirstOrDefault().ID_USUARIO.Trim(),
                                            REGISTRO_FEC = FechaServer
                                        }))
                                        {
                                            transaccion.Complete();
                                            StaticSourcesViewModel.SourceChanged = false;
                                            GroupCentroSeleccionado = Visibility.Hidden;
                                            BotonGuardarVisibilty = Visibility.Hidden;
                                            VisibilityBotonCancelar = Visibility.Hidden;
                                            OperacionActivaEnable = true;
                                            EXITO_PROCESO = true;
                                            new Dialogos().ConfirmacionDialogo("Éxito", "La información se registro correctamente");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
                                    }
                                }
                            }//fin
                        }
                        else
                        {
                            if (Guardar_Editar == "Editar")
                            {//Ijnicio
                                if (!pEditar)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                                    break;
                                }
                                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                                {

                                    try
                                    {
                                        ProcesoTipo = "Editar";

                                        if (new cDepartamentoAcceso().Eliminar(new DEPARTAMENTO_ACCESO()
                                        {
                                            ID_DEPARTAMENTO = SelectedCoordinadoresAsignados.OBJETO_DEPARTAMENTO_ACCESO.ID_DEPARTAMENTO,
                                            ID_USUARIO = SelectedCoordinadoresAsignados.OBJETO_DEPARTAMENTO_ACCESO.ID_USUARIO,
                                            REGISTRO_FEC = SelectedCoordinadoresAsignados.OBJETO_DEPARTAMENTO_ACCESO.REGISTRO_FEC
                                        }))
                                        {
                                            //------------------------------------------------Nueva Relacion------------------------------------------
                                            if (new cDepartamentoAcceso().Insertar(new DEPARTAMENTO_ACCESO()
                                            {
                                                ID_DEPARTAMENTO = SelectDep.ID_DEPARTAMENTO,
                                                ID_USUARIO = UsuarioControlador.Obtener(SelectedCoordinador.ID_EMPLEADO).FirstOrDefault().ID_USUARIO,
                                                REGISTRO_FEC = FechaServer
                                            }))
                                            {
                                                transaccion.Complete();
                                                StaticSourcesViewModel.SourceChanged = false;
                                                GroupCentroSeleccionado = Visibility.Hidden;
                                                BotonGuardarVisibilty = Visibility.Hidden;
                                                VisibilityBotonCancelar = Visibility.Hidden;
                                                OperacionActivaEnable = true;
                                                EXITO_PROCESO = true;
                                                new Dialogos().ConfirmacionDialogo("Éxito", "La información se Edito correctamente");
                                            }

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Editar", ex);
                                    }

                                }
                            }//Fin
                        }
                        //Si no ocurrio error 
                        if (EXITO_PROCESO)
                        {

                            PopularCoordinadoresDepSeleccionado();
                        }
                    }
                    break;


                case "cancelar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea cancelar sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                    GroupCentroSeleccionado = Visibility.Hidden;
                    BotonGuardarVisibilty = Visibility.Hidden;
                    VisibilityBotonCancelar = Visibility.Hidden;
                    LimpiarControles();
                    break;
                case "salir_menu":
                    //if (!Changed)
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                    PrincipalViewModel.SalirMenu();

                    break;
                case "limpiar_menu":

                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea limpiar la pantalla sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ConfiguracionDepartamentosView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ConfiguracionDepartamentosViewModel();


                    break;
                case "ayuda_menu":
                    break;
            }
        }

        private void LimpiarControles()
        {
            SelectPaisNacimiento = Parametro.PAIS;
            SelectEntidadNacimiento = -1;
            SelectMunicipioNacimiento = -1;
            IsEnablePais = true;
            IsEnableEstado = true;
            IsEnableMunicipio = true;
            IsEnableCentro = true;
            OperacionActivaEnable = true;
        }

        private void PopularCentroSeleccionado()
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
            {
              
                //CENTRO_COORDINADOR
                var CENTRO_COORDINADOR_SELECT = SelectedCoordinadoresAsignados.OBJETO_DEPARTAMENTO_ACCESO.USUARIO.EMPLEADO.CENTRO;

                SelectedPaisNacimiento = new cPaises().Obtener(int.Parse(CENTRO_COORDINADOR_SELECT.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.ID_PAIS_NAC.ToString())).FirstOrDefault();
                SelectedEntidadNacimiento = new cEntidad().Obtener(int.Parse(CENTRO_COORDINADOR_SELECT.ID_ENTIDAD.ToString())).FirstOrDefault();

                SelectPaisNacimiento = CENTRO_COORDINADOR_SELECT.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.ID_PAIS_NAC;
                SelectEntidadNacimiento = (short)CENTRO_COORDINADOR_SELECT.ID_ENTIDAD;
                SelectMunicipioNacimiento = (short)CENTRO_COORDINADOR_SELECT.ID_MUNICIPIO;


                IsEnablePais = false;
                IsEnableEstado = false;
                IsEnableMunicipio = false;
                IsEnableCentro = false;

                GroupCentroSeleccionado = System.Windows.Visibility.Visible;
                BotonGuardarVisibilty = Visibility.Visible;
                VisibilityBotonCancelar = Visibility.Visible;
            }));

            SelectCentro = (short)SelectedCoordinadoresAsignados.OBJETO_DEPARTAMENTO_ACCESO.USUARIO.EMPLEADO.CENTRO.ID_CENTRO;
            if (SelectCentro != null)
            {
                CargarCoordinadoresAsync();
            }
        }

        private void OcultarTodoGrupoCoordinadoresDep()
        {
            VisibilityCoordinadoresDepSelect = System.Windows.Visibility.Hidden;
            GroupCentroSeleccionado = System.Windows.Visibility.Hidden;
            BotonGuardarVisibilty = Visibility.Hidden;
            VisibilityBotonCancelar = Visibility.Hidden;

        }

        /// <summary>
        /// Popula Grid de de departamento coordinadores selecccionado
        /// </summary>
        private void PopularCoordinadoresDepSeleccionado()
        {
            ListaCoordinadoresAsignados = new ObservableCollection<cCOORDINADORESASIGNADOS>();
            if (pConsultar)
            {
                var ListCoorddinadoresAsignado = new List<cCOORDINADORESASIGNADOS>();
                int count = 0;
                //-----------------Obtiene la relacion con el departamneto seleccionado apara obtener a los coordinadores activos------------------
                foreach (var itemCoordinador in new cDepartamentoAcceso().ObtenerTodos(SelectDep.ID_DEPARTAMENTO, "S")
                    .Select(s => new { Obj = s, s.USUARIO.EMPLEADO.ID_CENTRO, s.USUARIO.EMPLEADO.PERSONA.NOMBRE, s.USUARIO.EMPLEADO.PERSONA.PATERNO, s.USUARIO.EMPLEADO.PERSONA.MATERNO }))
                {
                    count++;
                    var ObjCOORDINADOR = new cCOORDINADORESASIGNADOS();
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(!string.IsNullOrEmpty(itemCoordinador.PATERNO) ? itemCoordinador.PATERNO.Trim() : "");
                    sb.Append(" ");
                    sb.Append(!string.IsNullOrEmpty(itemCoordinador.MATERNO) ? itemCoordinador.MATERNO.Trim() : "");
                    sb.Append(" ");
                    sb.Append(!string.IsNullOrEmpty(itemCoordinador.NOMBRE) ? itemCoordinador.NOMBRE.Trim() : "");
                    ObjCOORDINADOR.NOMBRE_COORDINADOR = sb.ToString();
                    ObjCOORDINADOR.OBJETO_DEPARTAMENTO_ACCESO = itemCoordinador.Obj;
                    ListCoorddinadoresAsignado.Add(ObjCOORDINADOR);
                }

                //Si Encontro  o no registros habilita grid para asignar un coordinador al departamento seleccionado
                VisibilityCoordinadoresDepSelect = System.Windows.Visibility.Visible;
                if (count > 0)
                {

                    ListaCoordinadoresAsignados = new ObservableCollection<cCOORDINADORESASIGNADOS>(ListCoorddinadoresAsignado);
                }
            }


        }
        
        #region [Load]
        private async void OnLoad(ConfiguracionDepartamentosView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
                ValidacionDatosGenerales();
                StaticSourcesViewModel.SourceChanged = false;//Restea Propiedades
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }
        #endregion

        private void CargarListas()
        {
            try
            {
                ConfiguraPermisos();
                if (pConsultar)
                {
                    LstDepartamentos = new ObservableCollection<DEPARTAMENTO>(new cDepartamentos().ObtenerTodos(string.Empty, "S"));
                    ListPaisNacimiento = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerNacionalidad());
                }
                else
                {
                    LstDepartamentos = new ObservableCollection<DEPARTAMENTO>();
                    ListPaisNacimiento = new ObservableCollection<PAIS_NACIONALIDAD>();
                }
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListPaisNacimiento.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
                    OnPropertyChanged("ListPaisNacimiento");

                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listas", ex);
            }

        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CONF_DEPARTAMENTOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

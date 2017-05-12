using System;
using System.Linq;

namespace ControlPenales
{
    public partial class CatalogoEspecialistasViewModel : ValidationViewModelBase
    {
        public CatalogoEspecialistasViewModel() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    if (SelectedEspecialidadBusqueda != null)
                        Buscar();
                    break;
                case "buscar_visitante":
                    if (TextPaterno == null)
                        TextPaterno = string.Empty;
                    if (TextMaterno == null)
                        TextMaterno = string.Empty;
                    if (TextNombre == null)
                        TextNombre = string.Empty;
                    if (TextCodigo == null)
                        TextCodigo = string.Empty;
                    var pers2 = SelectPersona;
                    SelectPersonaAuxiliar = pers2;
                    BuscarPersonasSinCodigo();
                    break;
                case "nueva_busqueda_visitante":
                    TextPaterno = TextMaterno = TextNombre = string.Empty;
                    ListPersonas = ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    break;
                case "buscar_visita":
                    {
                        var pers1 = SelectPersona;
                        SelectPersonaAuxiliar = pers1;
                        TextCodigo = TextPaterno = TextMaterno = TextNombre = string.Empty;
                        ImagenPersona = new Imagenes().getImagenPerson();
                        ListPersonasAuxiliar = ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ESPECIALISTAS);
                    };
                    break;
                case "seleccionar_otros_personas":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ESPECIALISTAS);
                    ValidacionesEspecialistas();
                    ReadCampoCodigo = true;
                    EsOtroTipoCaptura = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ESPECIALISTAS);
                    break;
                case "cancelar_buscar_persona":
                    var pers = SelectPersonaAuxiliar;
                    SelectPersona = pers;
                    if (SelectPersona != null)
                        if (SelectPersona != null)
                        {
                            TextPaterno = !string.IsNullOrEmpty(SelectPersona.PATERNO) ? SelectPersona.PATERNO.Trim() : string.Empty;
                            TextMaterno = string.IsNullOrEmpty(SelectPersona.MATERNO) ? string.Empty : SelectPersona.MATERNO.Trim();
                            TextNombre = !string.IsNullOrEmpty(SelectPersona.NOMBRE) ? SelectPersona.NOMBRE.Trim() : string.Empty;
                            TextCodigo = SelectPersona.ID_PERSONA.ToString();
                        };

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ESPECIALISTAS);
                    break;
                case "seleccionar_buscar_persona":
                    if (SelectPersona == null)
                    {
                        SelectPersona = SelectPersonaAuxiliar;
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debe seleccionar a una persona.");
                        break;
                    }
                    else
                        await SeleccionarPersona(SelectPersona);
                    StaticSourcesViewModel.SourceChanged = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ESPECIALISTAS);
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

                        GuardarMenuEnabled = true;
                        AgregarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        AgregarVisible = true;
                        Bandera_Agregar = false;
                        StaticSourcesViewModel.SourceChanged = false;
                        ProcesaDatosEdicion();
                    }
                    else
                    {
                        Bandera_Agregar = true;
                        new Dialogos().ConfirmacionDialogo("Validación!", "Es necesario seleccione un especialista primero");
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

                    setValidaciones();
                    GuardarMenuEnabled = true;
                    AgregarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    AgregarVisible = true;
                    Bandera_Agregar = true;

                    MenuBuscarEnabled = true;
                    IsEnabledEspecialidad = true;
                    IsEnabledCamposBuasqueda = true;

                    Limpiar();
                    base.ClearRules();
                    break;
                case "menu_guardar":
                    setValidaciones();
                    if (base.HasErrors)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                        return;
                    }

                    if (EsOtroTipoCaptura)
                    {
                        ValidacionesEspecialistas();
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                            return;
                        }

                        await GuardaEspecialistaSinPersona();

                        EsOtroTipoCaptura = false;
                        Limpiar();
                        Buscar();
                        base.ClearRules();
                        AgregarVisible = false;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    };

                    if (SelectPersona == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debe seleccionar a una persona.");
                        break;
                    };

                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    AgregarVisible = false;
                    await Guardar();
                    Buscar();
                    StaticSourcesViewModel.SourceChanged = false;
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
                    Limpiar();
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "menu_ayuda":
                    break;
                case "menu_salir":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private async System.Threading.Tasks.Task GuardaEspecialistaSinPersona()
        {
            try
            {
                System.DateTime _fechaActual = Fechas.GetFechaDateServer;
                var NuevoEspecialista = new SSP.Servidor.ESPECIALISTA
                {
                    ESTATUS = SelectedEstatus.CLAVE,
                    ID_ESPECIALIDAD = SelectedEspecialidadEdicion.Value,
                    ID_PERSONA = null,
                    ESPECIALISTA_MATERNO = TextMaterno,
                    ESPECIALISTA_NOMBRE = TextNombre,
                    ESPECIALISTA_PATERNO = TextPaterno,
                    ID_CENTRO_UBI = GlobalVar.gCentro,
                    ID_USUARIO = GlobalVar.gUsr,
                    REGISTRO_FEC = _fechaActual
                };

                bool _Respuesta = false;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    if (!Bandera_Agregar)
                    {
                        NuevoEspecialista.ID_ESPECIALISTA = SelectedItem != null ? SelectedItem.ID_ESPECIALISTA : new short();
                        _Respuesta = new SSP.Controlador.Catalogo.Justicia.cEspecialistas().ActualizarEspecialista(NuevoEspecialista);
                        Bandera_Agregar = true;
                    }
                    else
                        _Respuesta = new SSP.Controlador.Catalogo.Justicia.cEspecialistas().GuardarEspecialista(NuevoEspecialista);
                });

                if (_Respuesta)
                    new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente.");
                else
                    new Dialogos().ConfirmacionDialogo("Error", "Surgió un error al guardar.");
            }
            catch (Exception exc)
            {
                new Dialogos().ConfirmacionDialogo("Error", string.Format("{0} {1}", "Surgió un error al guardar.", exc.Message));
            }
        }

        private async void Buscar()
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    LstEspecialistas = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALISTA>(new SSP.Controlador.Catalogo.Justicia.cEspecialistas().GetData(x => x.ID_ESPECIALIDAD == SelectedEspecialidadBusqueda));
                    RaisePropertyChanged("LstEspecialistas");
                    if (LstEspecialistas == null || LstEspecialistas.Count == 0)
                        emptyVisible = true;
                    else
                        emptyVisible = false;

                    RaisePropertyChanged("EmptyVisible");
                });

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar los especialistas.", ex);
            }
        }

        private void ProcesaDatosEdicion()
        {
            try
            {
                if (SelectedItem == null)
                    return;

                SelectPersona = SelectedItem.PERSONA;

                Lista_Estatus = new Clases.Estatus.EstatusControl();
                LstEspecialidadesCaptura = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD>(new SSP.Controlador.Catalogo.Justicia.cEspecialidades().ObtenerTodos(string.Empty, "S"));
                TextCodigo = SelectPersona != null ? SelectPersona.ID_PERSONA.ToString() : string.Empty;
                TextPaterno = SelectedItem.PERSONA != null ? !string.IsNullOrEmpty(SelectedItem.PERSONA.PATERNO) ? SelectedItem.PERSONA.PATERNO.Trim() : SelectedItem.ESPECIALISTA_PATERNO : SelectedItem.ESPECIALISTA_PATERNO;
                TextMaterno = SelectedItem.PERSONA != null ? !string.IsNullOrEmpty(SelectedItem.PERSONA.MATERNO) ? SelectedItem.PERSONA.MATERNO.Trim() : SelectedItem.ESPECIALISTA_MATERNO : SelectedItem.ESPECIALISTA_MATERNO;
                TextNombre = SelectedItem.PERSONA != null ? !string.IsNullOrEmpty(SelectedItem.PERSONA.NOMBRE) ? SelectedItem.PERSONA.NOMBRE.Trim() : SelectedItem.ESPECIALISTA_NOMBRE : SelectedItem.ESPECIALISTA_NOMBRE;
                SelectSexo = SelectedItem.PERSONA != null ? SelectedItem.PERSONA.SEXO : "S";
                FechaNacimiento = SelectedItem.PERSONA != null ? SelectedItem.PERSONA.FEC_NACIMIENTO : null;
                TextCurp = SelectedItem.PERSONA != null ? !string.IsNullOrEmpty(SelectedItem.PERSONA.CURP) ? SelectedItem.PERSONA.CURP : string.Empty : string.Empty;
                TextRfc = SelectedItem.PERSONA != null ? !string.IsNullOrEmpty(SelectedItem.PERSONA.RFC) ? SelectedItem.PERSONA.RFC : string.Empty : string.Empty;
                TextTelefono = SelectedItem.PERSONA != null ? !string.IsNullOrEmpty(SelectedItem.PERSONA.TELEFONO) ? SelectedItem.PERSONA.TELEFONO.Trim() : string.Empty : string.Empty;
                TextCorreo = SelectedItem.PERSONA != null ? !string.IsNullOrEmpty(SelectedItem.PERSONA.CORREO_ELECTRONICO) ? SelectedItem.PERSONA.CORREO_ELECTRONICO : string.Empty : string.Empty;
                TextNip = SelectedItem.PERSONA != null ? SelectedItem.PERSONA.PERSONA_NIP != null ? SelectedItem.PERSONA.PERSONA_NIP.Any() ? SelectedItem.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).Any() ?
                    SelectedItem.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().NIP : new int?() : new int?() : new int?() : new int?();
                FotoVisita = SelectedItem == null ?
                    new Imagenes().getImagenPerson() :
                        SelectedItem.PERSONA != null ? SelectedItem.PERSONA.PERSONA_BIOMETRICO.Any() ? SelectedItem.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).Any() ?
                            SelectedItem.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                            new Imagenes().getImagenPerson() : new Imagenes().getImagenPerson() : new Imagenes().getImagenPerson();
                SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                SelectedEspecialidadEdicion = SelectedItem.ID_ESPECIALIDAD;
                MenuBuscarEnabled = false;
                IsEnabledEspecialidad = false;
                IsEnabledCamposBuasqueda = false;

                if (SelectedItem.ID_PERSONA.HasValue)
                    EsOtroTipoCaptura = false;
                else
                    EsOtroTipoCaptura = true;
            }
            catch (Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar los especialistas.", exc);
            }
        }
        private void Limpiar()
        {
            TextCodigo = TextNombre = TextPaterno = TextMaterno = TextCurp = TextRfc = TextTelefono = TextCorreo = string.Empty;
            TextEdad = null;
            SelectSexo = "S";
            FotoVisita = new Imagenes().getImagenPerson();
            FechaNacimiento = null;
            SelectPersona = SelectPersonaAuxiliar = null;
            LstEspecialidadesCaptura = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD>(new SSP.Controlador.Catalogo.Justicia.cEspecialidades().ObtenerTodos(string.Empty, "S"));
            LstEspecialidadesCaptura.Insert(0, new SSP.Servidor.ESPECIALIDAD { DESCR = "SELECCIONE", ID_ESPECIALIDAD = -1 });
            RaisePropertyChanged("LstEspecialidadesCaptura");
            SelectedEspecialidadEdicion = -1;
            Lista_Estatus = new Clases.Estatus.EstatusControl();
            //Lista_Estatus.LISTA_ESTATUS.Insert(0, new Clases.Estatus.Estatus { CLAVE = "-1", DESCRIPCION = "SELECCIONE" });
            SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w => w.CLAVE == "S");
            StaticSourcesViewModel.SourceChanged = false;
        }

        private async System.Threading.Tasks.Task Guardar()
        {
            try
            {
                if (SelectPersona == null)
                    return;

                if (SelectedEspecialidadEdicion == null)
                    return;

                if (SelectedEstatus == null)
                    return;

                System.DateTime _fechaActual = Fechas.GetFechaDateServer;
                var NuevoEspecialista = new SSP.Servidor.ESPECIALISTA
                {
                    ESTATUS = SelectedEstatus.CLAVE,
                    ID_ESPECIALIDAD = SelectedEspecialidadEdicion.Value,
                    ID_PERSONA = SelectPersona.ID_PERSONA,
                    ID_USUARIO = GlobalVar.gUsr,
                    ESPECIALISTA_MATERNO = SelectPersona == null ? TextMaterno : string.Empty,
                    ESPECIALISTA_NOMBRE = SelectPersona == null ? TextNombre : string.Empty,
                    ESPECIALISTA_PATERNO = SelectPersona == null ? TextPaterno : string.Empty,
                    ID_CENTRO_UBI = GlobalVar.gCentro,
                    REGISTRO_FEC = _fechaActual
                };

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    if (!Bandera_Agregar)
                    {
                        NuevoEspecialista.ID_ESPECIALISTA = SelectedItem != null ? SelectedItem.ID_ESPECIALISTA : new short();
                        new SSP.Controlador.Catalogo.Justicia.cEspecialistas().ActualizarEspecialista(NuevoEspecialista);
                        Bandera_Agregar = true;
                    }
                    else
                        new SSP.Controlador.Catalogo.Justicia.cEspecialistas().GuardarEspecialista(NuevoEspecialista);
                });
                new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente.");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la especialidad.", ex);

            }
        }


        private async void PageLoad(CatalogoEspecialistasView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    _listItems = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD>(new SSP.Controlador.Catalogo.Justicia.cEspecialidades().ObtenerTodos(string.Empty, "S"));
                    _listItems.Insert(0, new SSP.Servidor.ESPECIALIDAD { DESCR = "SELECCIONE", ID_ESPECIALIDAD = -1 });
                    RaisePropertyChanged("ListItems");
                    if (_listItems == null || _listItems.Count == 0)
                    {
                        emptyVisible = true;
                        RaisePropertyChanged("EmptyVisible");
                    }

                    LstEspecialidadesCaptura = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD>(new SSP.Controlador.Catalogo.Justicia.cEspecialidades().ObtenerTodos(string.Empty, "S"));
                    LstEspecialidadesCaptura.Insert(0, new SSP.Servidor.ESPECIALIDAD { DESCR = "SELECCIONE", ID_ESPECIALIDAD = -1 });
                    RaisePropertyChanged("LstEspecialidadesCaptura");

                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w => w.CLAVE == "-1");

                    //LLENAR 
                    GuardarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.FirstOrDefault(w => w.CLAVE == "-1");
                    ConfiguraPermisos();
                });

                //setValidaciones();
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información.", ex);
            }
        }


        private void EspecialistaEnter(Object obj)
        {
            try
            {
                //if (!PConsultar)
                //{
                //    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                //    return;
                //}
                base.ClearRules();
                if (obj is System.Windows.Controls.TextBox)
                {
                    var textbox = obj as System.Windows.Controls.TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                TextNombre = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                TextPaterno = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                TextMaterno = textbox.Text;
                                break;
                            case "CodigoBuscar":
                                TextCodigo = textbox.Text;
                                break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(TextCodigo))
                    BuscarPersonasSinCodigo();
                else
                    BuscarPersonas();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }

        private async void BuscarPersonasSinCodigo()
        {
            try
            {
                var person = SelectPersona;
                ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                ListPersonasAuxiliar.InsertRange(ListPersonas);
                if (PopUpsViewModels.VisibleBuscarPersonasExistentes == System.Windows.Visibility.Collapsed)
                {
                    SelectPersonaAuxiliar = person;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ESPECIALISTAS);
                }
                EmptyBuscarRelacionInternoVisible = !(ListPersonas.Count > 0);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante externo.", ex);
            }
        }

        private async void BuscarPersonas()
        {
            try
            {
                var persona = SelectPersona;
                ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                SelectPersonaAuxiliar = persona;
                if (ListPersonas.Count == 1)
                {
                    #region Validaciones
                    var x = ListPersonas.FirstOrDefault();
                    EmptyBuscarRelacionInternoVisible = ListPersonas != null ? ListPersonas.Count <= 0 : false;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ESPECIALISTAS);

                    #endregion

                    await SeleccionarPersona(ListPersonas.FirstOrDefault());
                    StaticSourcesViewModel.SourceChanged = false;
                };
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private async System.Threading.Tasks.Task<System.Collections.Generic.List<SSP.Servidor.PERSONA>> SegmentarPersonasBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(TextPaterno) && string.IsNullOrEmpty(TextMaterno) && string.IsNullOrEmpty(TextNombre) && string.IsNullOrEmpty(TextCodigo))
                    return new System.Collections.Generic.List<SSP.Servidor.PERSONA>();
                Pagina = _Pag;
                //var result = await StaticSourcesViewModel.CargarDatosAsync<System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                //        new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONA>(new SSP.Controlador.Catalogo.Justicia.cPersona().ObtenerTodos(TextNombre, TextPaterno, TextMaterno, !string.IsNullOrEmpty(TextCodigo) ? int.Parse(TextCodigo) : new int?(), _Pag)
                //            .OrderByDescending(o => o.NOMBRE != null)));

                //Pagina = result.Any() ? Pagina++ : Pagina;
                //SeguirCargandoPersonas = result.Any();
                //return result.ToList();

                var _Empleados = await StaticSourcesViewModel.CargarDatosAsync<System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONA>>(() =>//CONSULTA TODOS PARA AISLARLOS EN BASE A LOS FILTROS ESTATICOS
                        new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONA>(new SSP.Controlador.Catalogo.Justicia.cPersona().ObtenerXNombreYNIP(TextNombre, TextPaterno, TextMaterno, string.IsNullOrEmpty(TextCodigo) ? new Nullable<int>() : int.Parse(TextCodigo))));

                var result = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONA>();
                if (_Empleados != null && _Empleados.Any())
                {
                    foreach (var item in _Empleados)
                    {
                        if (item.EMPLEADO != null)
                            if (item.EMPLEADO.USUARIO != null)
                                if (item.EMPLEADO.USUARIO != null && item.EMPLEADO.USUARIO.Any())
                                    if (item.EMPLEADO.USUARIO.Any(x => x.ESTATUS == "S" && x.USUARIO_ROL.Any(y => y.ID_CENTRO == GlobalVar.gCentro)))
                                    {
                                        result.Add(item);
                                        continue;
                                    }

                        if (item.PERSONA_EXTERNO != null)
                        {
                            result.Add(item);
                            continue;
                        }
                    }
                };

                Pagina = result.Any() ? Pagina + 1 : Pagina;
                SeguirCargandoPersonas = result.Any();
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new System.Collections.Generic.List<SSP.Servidor.PERSONA>();
            }
        }

        private async System.Threading.Tasks.Task SeleccionarPersona(SSP.Servidor.PERSONA persona)
        {
            try
            {
                var tipoP = SelectTipoPersona.HasValue ? SelectTipoPersona.Value : new int?();
                if (tipoP > 0)
                {
                    SelectTipoPersona = tipoP;
                    SelectPersona = persona;
                    CodigoEnabled = NombreReadOnly = false;
                    GeneralEnabled = ValidarEnabled = DiscapacitadoEnabled = true;

                    await StaticSourcesViewModel.CargarDatosMetodoAsync(async () =>
                    {
                        SelectPersona = persona;
                        await GetDatosPersonaSeleccionada(EntradaEnabled);
                        StaticSourcesViewModel.SourceChanged = false;
                    });

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ESPECIALISTAS);
                }
                else
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no cuenta con un registro correcto.");
                    return;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }


        private void SelectTipoVisita(object s, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (s is System.Windows.Controls.Button)
                {
                    switch (((System.Windows.Controls.Button)s).CommandParameter.ToString())
                    {
                        case "visita_familiar":
                            //LockerEnabled = false;
                            SelectTipoPersona = int.Parse(Parametro.ID_TIPO_PERSONA_VISITA);
                            break;
                        case "visita_empleado":
                            //LockerEnabled = true;
                            SelectTipoPersona = int.Parse(Parametro.ID_TIPO_PERSONA_EMPLEADO);
                            break;
                        case "visita_externa":
                            //LockerEnabled = true;
                            SelectTipoPersona = int.Parse(Parametro.ID_TIPO_PERSONA_EXTERNA);
                            break;
                        case "visita_legal":
                            //LockerEnabled = true;
                            SelectTipoPersona = int.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                            break;
                    }
                    SeleccionarTipoVisitaAduana.Close();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar el tipo de visita.", ex);
            }
        }


        private async System.Threading.Tasks.Task GetDatosPersonaSeleccionada(bool hideAll = false)
        {
            try
            {
                if (SelectPersona != null)
                {
                    TextCodigo = SelectPersona.ID_PERSONA.ToString();

                    if (SelectPersona != null)
                    {
                        TextPaterno = !string.IsNullOrEmpty(SelectPersona.PATERNO) ? SelectPersona.PATERNO.Trim() : string.Empty;
                        TextMaterno = !string.IsNullOrEmpty(SelectPersona.MATERNO) ? SelectPersona.MATERNO.Trim() : string.Empty;
                        TextNombre = !string.IsNullOrEmpty(SelectPersona.NOMBRE) ? SelectPersona.NOMBRE.Trim() : string.Empty;
                        SelectSexo = !string.IsNullOrEmpty(SelectPersona.SEXO) ? SelectPersona.SEXO : string.Empty;
                        FechaNacimiento = SelectPersona.FEC_NACIMIENTO.HasValue ? SelectPersona.FEC_NACIMIENTO.Value : new System.DateTime?();
                        TextCurp = !string.IsNullOrEmpty(SelectPersona.CURP) ? SelectPersona.CURP.Trim() : string.Empty;
                        TextRfc = !string.IsNullOrEmpty(SelectPersona.RFC) ? SelectPersona.RFC.Trim() : string.Empty;
                        TextTelefono = !string.IsNullOrEmpty(SelectPersona.TELEFONO) ? SelectPersona.TELEFONO.Trim() : string.Empty;
                        TextCorreo = !string.IsNullOrEmpty(SelectPersona.CORREO_ELECTRONICO) ? SelectPersona.CORREO_ELECTRONICO.Trim() : string.Empty;
                        TextNip = SelectPersona.PERSONA_NIP != null ? SelectPersona.PERSONA_NIP.Any() ? SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).Any() ?
                            SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().NIP : new int?() : new int?() : new int?();
                        FotoVisita = SelectPersona == null ?
                            new Imagenes().getImagenPerson() :
                                SelectPersona.PERSONA_BIOMETRICO.Any() ? SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).Any() ?
                                    SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                    new Imagenes().getImagenPerson() : new Imagenes().getImagenPerson();
                    };

                    LstEspecialidadesCaptura = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD>();

                    //prepara la lista con las especialidades en las que puede entrar
                    var _especialidadesDisponibles = new SSP.Controlador.Catalogo.Justicia.cEspecialidades().ObtenerTodos(string.Empty, "S");
                    var _especialidadesActuales = new SSP.Controlador.Catalogo.Justicia.cEspecialistas().GetData(x => x.ID_PERSONA == SelectPersona.ID_PERSONA);
                    if (_especialidadesDisponibles.Any())
                        foreach (var item in _especialidadesDisponibles)
                        {
                            if (_especialidadesActuales.Any(x => x.ID_ESPECIALIDAD == item.ID_ESPECIALIDAD))
                                continue; // ya existe dentro de las especialidades que tiene

                            System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                                 {
                                     LstEspecialidadesCaptura.Add(item);
                                 }));
                        };

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                     {
                         LstEspecialidadesCaptura.Insert(0, new SSP.Servidor.ESPECIALIDAD { DESCR = "SELECCIONE", ID_ESPECIALIDAD = -1 });
                     }));

                    RaisePropertyChanged("LstEspecialidadesCaptura");

                    SelectedEspecialidadEdicion = -1;
                    await System.Threading.Tasks.TaskEx.Delay(100);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante externo.", ex);
            }
        }

        private void ClickEnter(Object obj)
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
                            case "NombreBuscar":
                                TextNombre = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                TextPaterno = textbox.Text;
                                break;
                            case "PaternoBuscar":
                                TextPaterno = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                TextMaterno = textbox.Text;
                                break;
                            case "MaternoBuscar":
                                TextMaterno = textbox.Text;
                                break;
                            case "CodigoBuscar":
                                TextCodigo = textbox.Text;
                                break;
                        };
                };

                BuscarPersonasSinCodigo();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.CATALOGO_DE_ESPECIALISTAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
                    if (p.EDITAR == 1)
                        EditarEnabled = EditarMenuEnabled = true;
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
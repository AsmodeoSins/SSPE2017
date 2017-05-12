using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    public partial class AtencionSolicitudInterconsultaViewModel:ValidationViewModelBase
    {
        #region Constructor
        public AtencionSolicitudInterconsultaViewModel()
        {

        }

        public AtencionSolicitudInterconsultaViewModel(CANALIZACION canalizacion)
        {
            selectedCanalizacion = canalizacion;
        }

        #endregion

        #region Generales

        private async void SolicitudInterconsultaOnLoading(SolicitudInterconsultaView window)
        {
            estatus_administrativos_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
               
                CargarExtEspecialidadTipoInterconsulta(true);
                CargarExtEspecialidadInterconsultaNivelPrioridad(true);
                CargarEspecialidades(true);
                CargarTipoInterconsulta(true);
                CargarInterconsultaAtencionTipo(true);
                CargarInterconsultaNivelPrioridad(true);
                selectedInterconsultaTipo = -1;
                RaisePropertyChanged("SelectedInterconsultaTipo");
                selectedInterconsultaAtencionTipo = -1;
                RaisePropertyChanged("SelectedInterconsultaAtencionTipo");
                selectedNvlPrioridad = -1;
                RaisePropertyChanged("SelectedNvlPrioridad");
                //SERVICIOS AUXILIARES
                CargarTipo_Servicios_Auxiliares(true);
                CargarSubTipo_Servicios_Auxiliares(-1, true);
                CargarServicioAuxiliar(-1, -1, true);
                selectedTipoServAux = -1;
                RaisePropertyChanged("SelectedTipoServAux");
                selectedSubtipoServAux = -1;
                RaisePropertyChanged("SelectedSubtipoServAux");
                if (selectedCanalizacion != null)
                {
                    CargarDatosNotaMedica(selectedCanalizacion.NOTA_MEDICA, true);
                    CargarDatosCanalizacion(selectedCanalizacion, true);
                    isInterconsultaEnabled = true;
                    RaisePropertyChanged("IsInterconsultaEnabled");
                    IsCanalizacionVisible = Visibility.Visible;
                    MenuGuardarEnabled = true;
                    CargarDatosInterconsultaSolicitud(selectedCanalizacion, true);
                }
                    
            });
            ConfiguraPermisos();
        }

        private async void ClickSwitch(object parametro)
        {
            if (parametro!=null && !string.IsNullOrWhiteSpace(parametro.ToString()))
                switch (parametro.ToString())
                {
                    case "agregar_menu":
                        base.ClearRules();
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                            CargarAtencionTipoCanalBuscar(true);
                            BuscarCanalizacion(null,null, null, string.Empty, string.Empty, string.Empty, null, null, true);
                            selectedAtencion_TipoCanalBuscarValue = -1;
                            RaisePropertyChanged("SelectedAtencion_TipoCanalBuscarValue");
                        });
                        PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.BUSCAR_CANALIZACIONES);
                    break;
                    case "filtro_canalizacion":
                        if (!IsFechaIniBusquedaCanalValida)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La fecha de inicio tiene que ser menor a la fecha fin!");
                            return;
                        }
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        BuscarCanalizacion(selectedAtencion_TipoCanalBuscarValue == -1 ? null : (short?)selectedAtencion_TipoCanalBuscarValue, anioBuscarCanal, folioBuscarCanal, nombreBuscarCanal, apellidoPaternoBuscarCanal,
                            apellidoMaternoBuscarCanal,fechaInicialBuscarCanal,fechaFinalBuscarCanal,false);
                    });
                    break;
                    case "cancelar_buscar_canalizacion":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_CANALIZACIONES);
                        break;
                    case "seleccionar_canalizacion":
                        if (SelectedCanalizacionBusqueda==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione una canalización");
                            return;
                        }
                        modoVistaModelo = enumModo.INSERCION;
                        Limpiar();
                        GridsSolicitudesEnabled = true;
                        selectedHojaReferenciaMedicaMem = null;
                        SelectedExtEspecialidad = null;
                        SelectedServAuxCanalizacion = null;
                        SelectedCanalizacionInterconsulta = null;
                        IsCanalizacionVisible = Visibility.Visible;
                        FechaMinima = FechaServer;
                        selectedCanalizacion = SelectedCanalizacionBusqueda;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                            CargarDatosNotaMedica(selectedCanalizacion.NOTA_MEDICA,true);
                            CargarDatosCanalizacion(selectedCanalizacion, true);
                            CargarDatosInterconsultaSolicitud(selectedCanalizacion, true);
                        });
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_CANALIZACIONES);
                        IsModoInsercion = true;
                        IsInterconsultaEnabled = true;
                        if (_permisos_agregar)
                            MenuGuardarEnabled = true;
                        else
                            MenuGuardarEnabled = false;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                        
                    case "guardar_menu":
                        if (IsReferenciaVisible == Visibility.Collapsed && IsSolicitudInternaVisible==Visibility.Collapsed)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar una hoja de referencia médica o una solicitud de interconsulta interna");
                            return;
                        }
                        if (HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios. \n\n" + base.Error);
                            return;
                        }
                        await Guardar();
                        MenuAgregarEnabled = true;
                        GridsSolicitudesEnabled = true;
                        break;
                    case "buscar_menu":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            CargarAtencionTipoBuscar(true);
                            CargarTipoInterconsultaBuscar(true);
                            BuscarSolicitudesInterconsulta(null, null, null, null, null, null, null, null, null, false);
                            selectedAtencion_TipoBuscarValue = -1;
                            RaisePropertyChanged("SelectedAtencion_TipoBuscarValue");
                            selectedInter_TipoBuscarValue = -1;
                            RaisePropertyChanged("SelectedInter_TipoBuscarValue");
                        });
                         
                        LimpiarBusqueda();
                        PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.BUSCAR_INTERCONSULTA_SOLICITUD);
                        break;
                    case "cancelar_buscar_interconsulta":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERCONSULTA_SOLICITUD);
                        break;
                    case "filtro_interconsultas":
                        if (!IsFechaIniBusquedaSolValida)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La fecha de inicio tiene que ser menor a la fecha fin!");
                            return;
                        }
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarSolicitudesInterconsulta(selectedAtencion_TipoBuscarValue == -1 ? (short?)null : selectedAtencion_TipoBuscarValue, anioBuscarInter, folioBuscarInter,
                                nombreBuscarInter, apellidoPaternoBuscarInter, apellidoMaternoBuscarInter, selectedInter_TipoBuscarValue == -1 ? (short?)null : selectedInter_TipoBuscarValue,
                                fechaInicialBuscarInter, fechaFinalBuscarInter, true);
                        });
                        break;
                    case "seleccionar_interconsulta":
                        if (SelectedInterconsultaBusqueda==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debe de seleccionar una solicitud de interconsulta!");
                            return;
                        }
                        if (SelectedInterconsultaBusqueda.ID_INIVEL==(short)enumNivelPrioridad_SolicitudInterconsulta.ORDINARIA &&
                            !new cUsuarioRol().ObtenerTodos(StaticSourcesViewModel.UsuarioLogin.Username).Any(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_ROL == (short)enumRolesCoordinadoresAreasTecnicas.COORDINADOR_MEDICO))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Solamente el coordinador del área médica puede editar una solicitud de interconsulta ordinaria");
                            return;
                        }
                        modoVistaModelo = enumModo.REAGENDA;
                        selectedInterconsulta_solicitud = SelectedInterconsultaBusqueda;
                        if (selectedInterconsulta_solicitud.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA)
                            selectedHojaReferenciaMedicaMem = selectedInterconsulta_solicitud.HOJA_REFERENCIA_MEDICA.FirstOrDefault();
                            
                        FechaMinima = null;
                        try
                        {
                            await CargarInterconsultaSolicitud();
                        }
                        catch(Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de interconsulta", ex);
                        }
                        
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERCONSULTA_SOLICITUD);
                        IsCanalizacionVisible = Visibility.Collapsed;
                        IsInterconsultaEnabled = true;
                        IsModoInsercion = false;
                        if (_permisos_editar)
                            if (selectedInterconsulta_solicitud.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA)
                                MenuGuardarEnabled = true;
                            else
                                MenuGuardarEnabled = false;
                        else
                            MenuGuardarEnabled = false;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                       
                    case "limpiar_menu":
                        if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea limpiar la pantalla?") != 1)
                            return;
                    }
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new AtencionSolicitudInterconsultaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new AtencionSolicitudInterconsultaViewModel();
                        break;
                    case "salir_menu":
                        SalirMenu();
                        break;
                    case "menu_eliminar":
                        if (selectedInterconsulta_solicitud == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una solicitud de interconsulta");
                            return;
                        }
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "Existen cambios sin guardar,¿desea cancelar la solicitud?") != 1)
                                return;
                        }
                        else
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "¿Esta seguro de cancelar la solicitud de interconsulta?") != 1)
                                return;
                        }
                        if (!new cUsuarioRol().ObtenerTodos(StaticSourcesViewModel.UsuarioLogin.Username).Any(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_ROL == (short)enumRolesCoordinadoresAreasTecnicas.COORDINADOR_MEDICO))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Solamente el coordinador médico puede realizar una cancelación");
                            return;
                        }
                        modoVistaModelo = enumModo.CANCELAR;
                        await Guardar();
                        break;
                    //case "agregar_especialidad_tratamiento":
                    //    if (selectedEspecialidad==-1)
                    //    {
                    //        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una especialidad");
                    //        return;
                    //    }
                    //    if (lstExtEspecialidad!=null && lstExtEspecialidad.Any(a=>a.ID==selectedEspecialidad))
                    //    {
                    //        new Dialogos().ConfirmacionDialogo("Validación", "La especialidad ya fue agregada al plan de tratamiento");
                    //        return;
                    //    }

                    //    if (lstExtEspecialidad==null)
                    //    {
                    //        lstExtEspecialidad = new ObservableCollection<EXT_ESPECIALIDAD>();
                    //    }

                    //    lstExtEspecialidad.Add(new EXT_ESPECIALIDAD
                    //        {
                    //            ID = selectedEspecialidad,
                    //            Descr = lstEspecialidad.First(w => w.ID_ESPECIALIDAD == selectedEspecialidad).DESCR,
                    //            LstInterconsulta_Tipo = new List<INTERCONSULTA_TIPO>(lstExtEspecialidadInterconsulta_Tipo),
                    //            SelectedInterconsulta_Tipo = -1,
                    //            LstNivel_Prioridad=new List<INTERCONSULTA_NIVEL_PRIORIDAD>(lstExtEspecialidadNvlPrioridades),
                    //            SelectedNivel_Prioridad=-1,
                    //            IsProgramado=false
                    //        });
                    //    RaisePropertyChanged("LstExtEspecialidad");
                    //    break;
                    case "agregar_interconsulta":
                        SelectedExtEspecialidad = null;
                        if (SelectedServAuxCanalizacion == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una servicio auxiliar de diagnostico y tratamiento asignado a una canalizacion");
                            return;
                        }
                        if (selectedServAuxCanalizacion.IsProgramado)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El servicio auxiliar de diagnostico y tratamiento ya fue asignado a una interconsulta");
                            return;
                        }
                        if (SelectedServAuxCanalizacion.SelectedInterconsulta_Tipo==-1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar el tipo de interconsulta para el servicio auxiliar de diagnostico y tratamiento");
                            return;
                        }
                        if (SelectedServAuxCanalizacion.SelectedNivel_Prioridad == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar el nivel de prioridad para el servicio auxiliar de diagnostico y tratamiento");
                            return;
                        }
                        GridsSolicitudesEnabled = false;
                        if (SelectedServAuxCanalizacion.SelectedInterconsulta_Tipo==(short)enumInterconsulta_Tipo.EXTERNA)
                        {
                            IsSolicitudInternaVisible = Visibility.Collapsed;
                            tipoServicioInterconsultaSeleccionado = enumTipoServicioInterconsulta.SERV_AUXILIAR;
                            if (selectedCanalizacion.INTERCONSULTA_SOLICITUD!=null && selectedCanalizacion.INTERCONSULTA_SOLICITUD.Count>0)
                            {
                                lstHojaReferenciaMedicaMem = new ObservableCollection<HOJA_REFERENCIA_MEDICA>(new cHoja_Referencia_Medica().ObtenerTodosporCanalizacion(selectedCanalizacion.ID_ATENCION_MEDICA,"P",selectedServAuxCanalizacion.SelectedNivel_Prioridad,(short)enumTipoServicioInterconsulta.SERV_AUXILIAR));
                                RaisePropertyChanged("LstHojaReferenciaMedicaMem");
                            }
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_INTERCONSULTA_MEMORIA);
                        }
                        else if (SelectedServAuxCanalizacion.SelectedInterconsulta_Tipo==(short)enumInterconsulta_Tipo.INTERNA)
                        {
                            selectedHojaReferenciaMedicaMem = null;
                            LimpiarInterconsutaInterna();
                            id_atencion_tipo = (short)enumInterconsultaAtencionTipo.ESTUDIO_DE_GABINETE;
                            isfechacitarequerida = false;
                            IsSolicitudInternaVisible = Visibility.Visible;
                            IsReferenciaVisible = Visibility.Collapsed;
                            CargarCatalogosInterconsultaInterna(true);
                            clearValidacionesInterconsultaExterna();
                            setValidacionesRefMedica();
                        }
                        break;

                    case "agregar_interconsulta_especialidad":
                        SelectedServAuxCanalizacion = null;
                       
                        if (selectedExtEspecialidad == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una especialidad asignada a una canalizacion");
                            return;
                        }
                        if (selectedExtEspecialidad.IsProgramado)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La especialidad ya fue asignada a una interconsulta");
                            return;
                        }
                        if (selectedExtEspecialidad.SelectedInterconsulta_Tipo == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar el tipo de interconsulta para la especialidad");
                            return;
                        }
                        if (selectedExtEspecialidad.SelectedNivel_Prioridad == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar el nivel de prioridad para la especialidad");
                            return;
                        }
                        GridsSolicitudesEnabled = false;
                        if (selectedExtEspecialidad.SelectedInterconsulta_Tipo == (short)enumInterconsulta_Tipo.EXTERNA)
                        {
                            IsSolicitudInternaVisible = Visibility.Collapsed;
                            tipoServicioInterconsultaSeleccionado = enumTipoServicioInterconsulta.ESPECIALIDAD;
                            if (selectedCanalizacion.INTERCONSULTA_SOLICITUD != null && selectedCanalizacion.INTERCONSULTA_SOLICITUD.Count > 0)
                            {
                                lstHojaReferenciaMedicaMem = new ObservableCollection<HOJA_REFERENCIA_MEDICA>(new cHoja_Referencia_Medica().ObtenerTodosporCanalizacion(selectedCanalizacion.ID_ATENCION_MEDICA, "P", selectedExtEspecialidad.SelectedNivel_Prioridad, (short)enumTipoServicioInterconsulta.ESPECIALIDAD));
                                RaisePropertyChanged("LstHojaReferenciaMedicaMem");
                            }
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_INTERCONSULTA_MEMORIA);
                        }
                        else if (SelectedExtEspecialidad.SelectedInterconsulta_Tipo==(short)enumInterconsulta_Tipo.INTERNA)
                        {
                            LimpiarInterconsutaInterna();
                            id_atencion_tipo = selectedExtEspecialidad.ID_INTERAT;
                            isfechacitarequerida = false;
                            IsSolicitudInternaVisible = Visibility.Visible;
                            IsReferenciaVisible = Visibility.Collapsed;
                            CargarCatalogosInterconsultaInterna(true);
                            clearValidacionesInterconsultaExterna();
                            setValidacionesRefMedica();
                        }
                        break;
                    case "agregar_interconsulta_mem":
                        selectedHojaReferenciaMedicaMem = null;
                        LimpiarHojaRefMedica();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERCONSULTA_MEMORIA);
                        if (tipoServicioInterconsultaSeleccionado == enumTipoServicioInterconsulta.SERV_AUXILIAR)
                            id_atencion_tipo = (short)enumInterconsultaAtencionTipo.ESTUDIO_DE_GABINETE;
                        else
                            id_atencion_tipo = selectedExtEspecialidad.ID_INTERAT;
                        isfechacitarequerida = false;
                        IsFechaCitaValid = true;
                        IsOtroHospitalSelected = Visibility.Collapsed;
                        IsReferenciaVisible = Visibility.Visible;
                        IsSolicitudInternaVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            CargarHospitales(true);
                            CargarTipoCitas(true);
                            selectedRefMedHospital = -1;
                            RaisePropertyChanged("SelectedRefMedHospital");
                            selectedRefMedTipoCitaValue = -1;
                            RaisePropertyChanged("SelectedRefMedTipoCitaValue");
                            if (tipoServicioInterconsultaSeleccionado == enumTipoServicioInterconsulta.SERV_AUXILIAR)
                            {
                                if (lstSerAuxHojaRef == null)
                                    lstSerAuxHojaRef = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>();
                                lstSerAuxHojaRef.Add(new EXT_SERV_AUX_DIAGNOSTICO()
                                {
                                    ID_SERV_AUX = selectedServAuxCanalizacion.ID_SERV_AUX,
                                    DESCR = selectedServAuxCanalizacion.DESCR,
                                    SUBTIPO_DESCR = selectedServAuxCanalizacion.SUBTIPO_DESCR,

                                });
                                lstSerAuxHojaRef = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(LstSerAuxHojaRef);
                                RaisePropertyChanged("LstSerAuxHojaRef");
                                isServicioAuxiliarHojaRefVisible = Visibility.Visible;
                                RaisePropertyChanged("IsServicioAuxiliarHojaRefVisible");
                                isEspecialidadHojaRefVisible = Visibility.Collapsed;
                                RaisePropertyChanged("IsEspecialidadHojaRefVisible");
                            }
                            else
                            {
                                hRefEspecialidad= selectedExtEspecialidad.Descr;
                                RaisePropertyChanged("HRefEspecialidad");
                                isServicioAuxiliarHojaRefVisible = Visibility.Collapsed;
                                RaisePropertyChanged("IsServicioAuxiliarHojaRefVisible");
                                isEspecialidadHojaRefVisible = Visibility.Visible;
                                RaisePropertyChanged("IsEspecialidadHojaRefVisible");
                            }
                        
                        });
                            
                        
                        clearValidacionesRefMedica();
                        setValidacionesInterconsultaExterna();
                        break;
                    case "cancelar_buscar_interconsulta_mem":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERCONSULTA_MEMORIA);
                        GridsSolicitudesEnabled = true;
                        break;
                    case "seleccionar_interconsulta_mem":
                        if (SelectedHojaReferenciaMedicaMemBuscar==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una hoja de referencia medica");
                            return;
                        }
                        LimpiarHojaRefMedica();
                        id_atencion_tipo = (short)enumInterconsultaAtencionTipo.ESTUDIO_DE_GABINETE;
                        selectedHojaReferenciaMedicaMem = selectedHojaReferenciaMedicaMemBuscar;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERCONSULTA_MEMORIA);
                        isfechacitarequerida = false;
                        IsFechaCitaValid = true;
                        IsOtroHospitalSelected = Visibility.Collapsed;
                        IsReferenciaVisible = Visibility.Visible;
                        IsSolicitudInternaVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            CargarHospitales(true);
                            CargarTipoCitas(true);
                            selectedRefMedHospital = selectedHojaReferenciaMedicaMem.ID_HOSPITAL.Value;
                            RaisePropertyChanged("SelectedRefMedHospital");
                            if (selectedRefMedHospital == otro_hospital)
                            {
                                isOtroHospitalSelected = Visibility.Visible;
                                RaisePropertyChanged("IsOtroHospitalSelected");
                                textRefMedOtroHospital = selectedHojaReferenciaMedicaMem.HOSPITAL_OTRO;
                                RaisePropertyChanged("textRefMedOtroHospital");
                            }
                            else
                            {
                                isOtroHospitalSelected = Visibility.Collapsed;
                                RaisePropertyChanged("IsOtroHospitalSelected");
                            }
                            textRefMedExpHGT = selectedHojaReferenciaMedicaMem.EXP_HGT;
                            RaisePropertyChanged("TextRefMedExpHGT");
                            selectedRefMedTipoCitaValue = selectedHojaReferenciaMedicaMem.ID_TIPO_CITA.Value;
                            RaisePropertyChanged("SelectedRefMedTipoCitaValue");
                            if (tipoServicioInterconsultaSeleccionado == enumTipoServicioInterconsulta.SERV_AUXILIAR)
                            {
                                lstSerAuxHojaRef = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.SERVICIO_AUX_INTERCONSULTA.Select(s=>new EXT_SERV_AUX_DIAGNOSTICO{
                                    ID_SERV_AUX=s.ID_SERV_AUX,
                                    DESCR=s.SERVICIO_AUX_DIAG_TRAT.DESCR,
                                    SUBTIPO_DESCR=s.SERVICIO_AUX_DIAG_TRAT.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR
                                    }));

                                lstSerAuxHojaRef.Add(new EXT_SERV_AUX_DIAGNOSTICO()
                                {
                                    ID_SERV_AUX = selectedServAuxCanalizacion.ID_SERV_AUX,
                                    DESCR = selectedServAuxCanalizacion.DESCR,
                                    SUBTIPO_DESCR = selectedServAuxCanalizacion.SUBTIPO_DESCR,

                                });
                                lstSerAuxHojaRef = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(lstSerAuxHojaRef);
                                RaisePropertyChanged("LstSerAuxHojaRef");
                                isServicioAuxiliarHojaRefVisible = Visibility.Visible;
                                RaisePropertyChanged("IsServicioAuxiliarHojaRefVisible");
                            }
                            
                            if (selectedHojaReferenciaMedicaMem.FECHA_CITA.HasValue)
                            {
                                fechaMinima = selectedHojaReferenciaMedicaMem.FECHA_CITA.Value.AddDays(-1); //ES NECESARIO RESTAR UN MINUTO PARA QUE EL CONTROL MUESTRE LA FECHA
                                RaisePropertyChanged("FechaMinima");
                            }
                            fechaCita = selectedHojaReferenciaMedicaMem.FECHA_CITA;
                            RaisePropertyChanged("FechaCita");
                            if (SelectedRefMedTipoCitaValue == (short)enumCita_Tipo.PRIMERA)
                            {
                                isfechacitarequerida = false;
                                if (!FechaCita.HasValue || (FechaCita.HasValue && FechaCita >= FechaServer))
                                    IsFechaCitaValid = true;
                                else
                                    IsFechaCitaValid = false;
                            }
                            else
                            {
                                isfechacitarequerida = true;
                                if (!FechaCita.HasValue || (FechaCita.HasValue && FechaCita < FechaServer))
                                    IsFechaCitaValid = false;
                            }
                            textRefMedMotivo = selectedHojaReferenciaMedicaMem.MOTIVO;
                            RaisePropertyChanged("TextRefMedMotivo");
                            textRefMedObservaciones = selectedHojaReferenciaMedicaMem.OBSERV;
                            RaisePropertyChanged("TextRefMedObservaciones");
                        
                        });
                            
                        
                        clearValidacionesRefMedica();
                        setValidacionesInterconsultaExterna();
                        break;
                    case "cerrar_hoja_referencia_medica":
                        if (SelectedCanalizacionInterconsulta==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una solicitud de interconsulta");
                            return;
                        }
                        if (SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.ESTATUS!="P")
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Esta operación no es valida para este registro, favor de seleccionar una solicitud de interconsulta abierta");
                            return;
                        }
                        INTERCONSULTA_SOLICITUD _interconsulta_solicitud = new INTERCONSULTA_SOLICITUD {
                            ESTATUS=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().FECHA_CITA.HasValue?"S":"E",
                            ID_CENTRO_UBI=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.ID_CENTRO_UBI,
                            ID_ESPECIALIDAD=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.ID_ESPECIALIDAD,
                            ID_INIVEL=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.ID_INIVEL,
                            ID_INTER=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.ID_INTER,
                            ID_INTERAT=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.ID_INTERAT,
                            ID_INTERSOL=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.ID_INTERSOL,
                            ID_NOTA_MEDICA=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.ID_NOTA_MEDICA,
                            ID_USUARIO=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.ID_USUARIO,
                            OFICIO_EXC=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.OFICIO_EXC,
                            REGISTRO_FEC=SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.REGISTRO_FEC
                        };
                         if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando solicitud", () =>
                             {
                                 new cInterconsulta_Solicitud().Actualizar(_interconsulta_solicitud, Fechas.GetFechaDateServer, false);
                                 selectedCanalizacion = new cCanalizacion().ObtenerCanalizaciones(selectedCanalizacion.ID_ATENCION_MEDICA,selectedCanalizacion.ID_CENTRO_UBI).FirstOrDefault();
                                 CargarDatosCanalizacion(selectedCanalizacion, true);
                                 CargarDatosInterconsultaSolicitud(selectedCanalizacion, true);
                                 return true;
                             }))
                         {
                             new Dialogos().ConfirmacionDialogo("EXITO!", "La solicitud de interconsulta fue modifica con exito");
                             StaticSourcesViewModel.SourceChanged = false;
                         }
                         break;
                    case "agregar_fecha_cita_hoja_referencia":
                        if (SelectedCanalizacionInterconsulta==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una solicitud de interconsulta");
                            return;
                        }
                        if (SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.ESTATUS!="E")
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Esta operación no es valida para este registro, favor de seleccionar una solicitud de interconsulta sin fecha de cita");
                            return;
                        }
                        LimpiarHojaRefMedica();
                        modoVistaModelo = enumModo.REAGENDA;
                        id_atencion_tipo = (short)enumInterconsultaAtencionTipo.ESTUDIO_DE_GABINETE;
                        selectedHojaReferenciaMedicaMem = SelectedCanalizacionInterconsulta.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERCONSULTA_MEMORIA);
                        isfechacitarequerida = false;
                        IsFechaCitaValid = true;
                        IsOtroHospitalSelected = Visibility.Collapsed;
                        IsReferenciaVisible = Visibility.Visible;
                        IsSolicitudInternaVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            CargarHospitales(true);
                            CargarTipoCitas(true);
                            selectedRefMedHospital = selectedHojaReferenciaMedicaMem.ID_HOSPITAL.Value;
                            RaisePropertyChanged("SelectedRefMedHospital");
                            if (selectedRefMedHospital == otro_hospital)
                            {
                                isOtroHospitalSelected = Visibility.Visible;
                                RaisePropertyChanged("IsOtroHospitalSelected");
                                textRefMedOtroHospital = selectedHojaReferenciaMedicaMem.HOSPITAL_OTRO;
                                RaisePropertyChanged("textRefMedOtroHospital");
                            }
                            else
                            {
                                isOtroHospitalSelected = Visibility.Collapsed;
                                RaisePropertyChanged("IsOtroHospitalSelected");
                            }
                            textRefMedExpHGT = selectedHojaReferenciaMedicaMem.EXP_HGT;
                            RaisePropertyChanged("TextRefMedExpHGT");
                            selectedRefMedTipoCitaValue = selectedHojaReferenciaMedicaMem.ID_TIPO_CITA.Value;
                            RaisePropertyChanged("SelectedRefMedTipoCitaValue");
                            if (selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.SERVICIO_AUX_INTERCONSULTA.Any())
                            {
                                lstSerAuxHojaRef = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.SERVICIO_AUX_INTERCONSULTA.Select(s=>new EXT_SERV_AUX_DIAGNOSTICO{
                                    ID_SERV_AUX=s.ID_SERV_AUX,
                                    DESCR=s.SERVICIO_AUX_DIAG_TRAT.DESCR,
                                    SUBTIPO_DESCR=s.SERVICIO_AUX_DIAG_TRAT.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR
                                    }));
                                RaisePropertyChanged("LstSerAuxHojaRef");
                                isServicioAuxiliarHojaRefVisible = Visibility.Visible;
                                RaisePropertyChanged("IsServicioAuxiliarHojaRefVisible");
                            }
                            else
                            {
                                hRefEspecialidad = selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ESPECIALIDAD.DESCR;
                                RaisePropertyChanged("HRefEspecialidad");
                                isServicioAuxiliarHojaRefVisible = Visibility.Collapsed;
                                RaisePropertyChanged("IsServicioAuxiliarHojaRefVisible");
                                isEspecialidadHojaRefVisible = Visibility.Visible;
                                RaisePropertyChanged("IsEspecialidadHojaRefVisible");
                            }
                            
                            if (selectedHojaReferenciaMedicaMem.FECHA_CITA.HasValue)
                            {
                                fechaMinima = selectedHojaReferenciaMedicaMem.FECHA_CITA.Value.AddDays(-1); //ES NECESARIO RESTAR UN MINUTO PARA QUE EL CONTROL MUESTRE LA FECHA
                                RaisePropertyChanged("FechaMinima");
                            }
                            fechaCita = selectedHojaReferenciaMedicaMem.FECHA_CITA;
                            RaisePropertyChanged("FechaCita");
                            if (SelectedRefMedTipoCitaValue == (short)enumCita_Tipo.PRIMERA)
                            {
                                isfechacitarequerida = false;
                                if (!FechaCita.HasValue || (FechaCita.HasValue && FechaCita >= FechaServer))
                                    IsFechaCitaValid = true;
                                else
                                    IsFechaCitaValid = false;
                            }
                            else
                            {
                                isfechacitarequerida = true;
                                if (!FechaCita.HasValue || (FechaCita.HasValue && FechaCita < FechaServer))
                                    IsFechaCitaValid = false;
                            }
                            textRefMedMotivo = selectedHojaReferenciaMedicaMem.MOTIVO;
                            RaisePropertyChanged("TextRefMedMotivo");
                            textRefMedObservaciones = selectedHojaReferenciaMedicaMem.OBSERV;
                            RaisePropertyChanged("TextRefMedObservaciones");
                        
                        });
                        clearValidacionesRefMedica();
                        setValidacionesInterconsultaExterna();
                         break;
                }
        }

        public void CargarCatalogosInterconsultaInterna(bool isExceptionManaged)
        {
            try
            {
                CargarCentros(true);
                selectedSolIntCentro = -1;
                RaisePropertyChanged("SelectedSolIntCentro");
                textSolIntMotivo = string.Empty;
                RaisePropertyChanged("TextSolIntMotivo");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos de interconsulta externa", ex);
            }
        }

        public async static void SalirMenu()
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?");
                    if (dialogresult != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }

                var metro = Application.Current.Windows[0] as MetroWindow;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                GC.Collect();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir del módulo", ex);
            }
        }


        private async Task CargarInterconsultaSolicitud()
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                selectedInterconsultaTipo = selectedInterconsulta_solicitud.ID_INTER.Value;
                RaisePropertyChanged("SelectedInterconsultaTipo");
                selectedNvlPrioridad = selectedInterconsulta_solicitud.ID_INIVEL.Value;
                RaisePropertyChanged("SelectedNvlPrioridad");
                selectedInterconsultaAtencionTipo = selectedInterconsulta_solicitud.ID_INTERAT.Value;
                RaisePropertyChanged("SelectedInterconsultaAtencionTipo");

                #region cambio_interconsulta_tipo
                if (selectedInterconsultaTipo == 1)  //Si la interconsulta es interna
                {
                    isfechacitarequerida = false;
                    IsSolicitudInternaVisible = Visibility.Visible;
                    IsReferenciaVisible = Visibility.Collapsed;
                    CargarCentros(true);
                    selectedSolIntCentro = selectedInterconsulta_solicitud.SOL_INTERCONSULTA_INTERNA.First().ID_CENTRO.Value;
                    RaisePropertyChanged("SelectedSolIntCentro");
                    textSolIntMotivo = selectedInterconsulta_solicitud.SOL_INTERCONSULTA_INTERNA.First().MOTIVO_INTERCONSULTA;
                    RaisePropertyChanged("TextSolIntMotivo");
                }
                else if (selectedInterconsultaTipo == 2) //Si es externa
                {
                    var _hoja_ref=selectedInterconsulta_solicitud.HOJA_REFERENCIA_MEDICA.First();
                    isfechacitarequerida = false;
                    IsOtroHospitalSelected = Visibility.Collapsed;
                    IsReferenciaVisible = Visibility.Visible;
                    IsSolicitudInternaVisible = Visibility.Collapsed;
                    CargarHospitales(true);
                    CargarTipoCitas(true);
                    selectedRefMedHospital = _hoja_ref.ID_HOSPITAL.Value;
                    RaisePropertyChanged("SelectedRefMedHospital");
                    if (selectedRefMedHospital == otro_hospital)
                    {
                        isOtroHospitalSelected = Visibility.Visible;
                        RaisePropertyChanged("IsOtroHospitalSelected");
                        textRefMedOtroHospital = _hoja_ref.HOSPITAL_OTRO;
                        RaisePropertyChanged("textRefMedOtroHospital");
                    }
                    else
                    {
                        isOtroHospitalSelected = Visibility.Collapsed;
                        RaisePropertyChanged("IsOtroHospitalSelected");
                    }
                    textRefMedExpHGT = _hoja_ref.EXP_HGT;
                    RaisePropertyChanged("TextRefMedExpHGT");
                    selectedRefMedTipoCitaValue = _hoja_ref.ID_TIPO_CITA.Value;
                    RaisePropertyChanged("SelectedRefMedTipoCitaValue");
                    if (_hoja_ref.FECHA_CITA.HasValue)
                    {
                        fechaMinima = _hoja_ref.FECHA_CITA.Value.AddDays(-1); //ES NECESARIO RESTAR UN MINUTO PARA QUE EL CONTROL MUESTRE LA FECHA
                        RaisePropertyChanged("FechaMinima");
                    }

                    fechaCita = _hoja_ref.FECHA_CITA;
                    RaisePropertyChanged("FechaCita");
                    
                    if (SelectedRefMedTipoCitaValue == (short)enumCita_Tipo.PRIMERA)
                    {
                        isfechacitarequerida = false;
                        if (!FechaCita.HasValue || (FechaCita.HasValue && FechaCita >= FechaServer))
                            IsFechaCitaValid = true;
                        else
                            IsFechaCitaValid = false;
                    }
                    else
                    {
                        isfechacitarequerida = true;
                        if (!FechaCita.HasValue || (FechaCita.HasValue && FechaCita < FechaServer))
                            IsFechaCitaValid = false;
                        else
                            IsFechaCitaValid = true;
                    }
                    textRefMedMotivo = selectedInterconsulta_solicitud.HOJA_REFERENCIA_MEDICA.First().MOTIVO;
                    RaisePropertyChanged("TextRefMedMotivo");
                    textRefMedObservaciones = selectedInterconsulta_solicitud.HOJA_REFERENCIA_MEDICA.First().OBSERV;
                    RaisePropertyChanged("TextRefMedObservaciones");
                    if (selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.SERVICIO_AUX_INTERCONSULTA!=null && selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.SERVICIO_AUX_INTERCONSULTA.Count>0)
                    {
                        lstSerAuxHojaRef = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.SERVICIO_AUX_INTERCONSULTA.Select(s => new EXT_SERV_AUX_DIAGNOSTICO
                        {
                            ID_SERV_AUX = s.ID_SERV_AUX,
                            DESCR = s.SERVICIO_AUX_DIAG_TRAT.DESCR,
                            SUBTIPO_DESCR = s.SERVICIO_AUX_DIAG_TRAT.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR
                        }));
                        RaisePropertyChanged("LstSerAuxHojaRef");
                        isServicioAuxiliarHojaRefVisible = Visibility.Visible;
                        RaisePropertyChanged("IsServicioAuxiliarHojaRefVisible");
                        isEspecialidadHojaRefVisible = Visibility.Collapsed;
                        RaisePropertyChanged("IsEspecialidadHojaRefVisible");
                    }
                    else
                    {
                        hRefEspecialidad = selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ESPECIALIDAD.DESCR;
                        RaisePropertyChanged("HRefEspecialidad");
                        isServicioAuxiliarHojaRefVisible = Visibility.Collapsed;
                        RaisePropertyChanged("IsServicioAuxiliarHojaRefVisible");
                        isEspecialidadHojaRefVisible = Visibility.Visible;
                        RaisePropertyChanged("IsEspecialidadHojaRefVisible");
                    }
                    clearValidacionesRefMedica();
                    setValidacionesInterconsultaExterna();

                }
                else
                {
                    IsReferenciaVisible = Visibility.Collapsed;
                    IsSolicitudInternaVisible = Visibility.Collapsed;
                    clearValidacionesInterconsultaExterna();
                    clearValidacionesRefMedica();
                }
                #endregion
                CargarDatosNotaMedica(selectedInterconsulta_solicitud.CANALIZACION.NOTA_MEDICA, true);
            });
            
        }

        private void CargarDatosInterconsultaSolicitud(CANALIZACION canalizacion, bool isExceptionManaged=false)
        {
            try
            {
                lstCanalizacionInterconsultas = new ObservableCollection<EXT_INTERCONSULTA_SOLICITUD>(canalizacion.INTERCONSULTA_SOLICITUD.Select(s => new EXT_INTERCONSULTA_SOLICITUD {
                    INTERCONSULTA_SOLICITUD=s,
                    IsCanalizacionEspecialidad=s.ID_ESPECIALIDAD.HasValue?Visibility.Visible:Visibility.Collapsed,
                    IsCanalizacionServAux=s.SERVICIO_AUX_INTERCONSULTA.Any()?Visibility.Visible:Visibility.Collapsed
                }));
                RaisePropertyChanged("LstCanalizacionInterconsultas");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la canalización", ex);
            }
        }

        private void CargarDatosCanalizacion(CANALIZACION canalizacion, bool isExceptionManaged=false)
        {
            try
            {
                lstExtEspecialidad = new ObservableCollection<EXT_ESPECIALIDAD>(canalizacion.CANALIZACION_ESPECIALIDAD.Select(s => new EXT_ESPECIALIDAD {
                    Descr=s.ESPECIALIDAD.DESCR,
                    ID=s.ID_ESPECIALIDAD,
                    ID_INTERAT=s.ID_INTERAT.Value,
                    INTERCONSULTA_ATENCION_DESCR=s.INTERCONSULTA_ATENCION_TIPO.DESCR,
                    LstInterconsulta_Tipo=lstExtEspecialidadInterconsulta_Tipo,
                    LstNivel_Prioridad=lstExtEspecialidadNvlPrioridades,
                    SelectedInterconsulta_Tipo=s.ID_INTER.HasValue?s.ID_INTER.Value:(short)-1,
                    SelectedNivel_Prioridad=s.ID_INIVEL.HasValue?s.ID_INIVEL.Value:(short)-1,
                    IsProgramado=(s.CANALIZACION.INTERCONSULTA_SOLICITUD!=null && s.CANALIZACION.INTERCONSULTA_SOLICITUD.Count>0?
                    s.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(f=>f.ID_ESPECIALIDAD==s.ID_ESPECIALIDAD && f.ID_INTER==(short)enumInterconsulta_Tipo.EXTERNA 
                        && (f.ESTATUS=="P" || f.ESTATUS=="E" || f.ESTATUS=="S" || (f.ESTATUS=="N" && f.EXCARCELACION_DESTINO.Any(a3=>a3.ID_ESTATUS!="CA")))):false)||
                    (s.CANALIZACION.INTERCONSULTA_SOLICITUD != null && s.CANALIZACION.INTERCONSULTA_SOLICITUD.Count > 0 ?
                    s.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(f =>f.ID_ESPECIALIDAD == s.ID_ESPECIALIDAD && f.ID_INTER == (short)enumInterconsulta_Tipo.INTERNA && f.ESTATUS != "C") : false)
                }));
                RaisePropertyChanged("LstExtEspecialidad");
                lstServAuxSeleccionados = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(canalizacion.CANALIZACION_SERV_AUX.Select(s => new EXT_SERV_AUX_DIAGNOSTICO
                {
                    DESCR=s.SERVICIO_AUX_DIAG_TRAT.DESCR,
                    ID_SERV_AUX=s.ID_SERV_AUX,
                    ID_SUBTIPO_SADT=s.SERVICIO_AUX_DIAG_TRAT.ID_SUBTIPO_SADT.Value,
                    ISCHECKED=true,
                    SUBTIPO_DESCR=s.SERVICIO_AUX_DIAG_TRAT.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR,
                    LstInterconsulta_Tipo=lstExtEspecialidadInterconsulta_Tipo,
                    LstNivel_Prioridad=lstExtEspecialidadNvlPrioridades,
                    SelectedInterconsulta_Tipo=s.ID_INTER.HasValue?s.ID_INTER.Value:(short)-1,
                    SelectedNivel_Prioridad=s.ID_INIVEL.HasValue?s.ID_INIVEL.Value:(short)-1,
                    IsProgramado = (s.CANALIZACION.INTERCONSULTA_SOLICITUD!=null && s.CANALIZACION.INTERCONSULTA_SOLICITUD.Count>0?
                    s.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SERVICIO_AUX_INTERCONSULTA.Any(a2 => a2.ID_SERV_AUX == s.ID_SERV_AUX) 
                        && a.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA && (a.ESTATUS == "P" || a.ESTATUS=="E" || a.ESTATUS=="S" || (a.ESTATUS=="N" && a.EXCARCELACION_DESTINO.Any(a3=>a3.ID_ESTATUS!="CA")))) : false) ||
                    (s.CANALIZACION.INTERCONSULTA_SOLICITUD!=null && s.CANALIZACION.INTERCONSULTA_SOLICITUD.Count>0?
                    s.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a=>a.SERVICIO_AUX_INTERCONSULTA.Any(a2=>a2.ID_SERV_AUX==s.ID_SERV_AUX) && a.ID_INTER==(short)enumInterconsulta_Tipo.INTERNA && a.ESTATUS!="C"):false)
                }));
                RaisePropertyChanged("LstServAuxSeleccionados");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la canalización", ex);
            }
        }
       

        private void CargarDatosNotaMedica(NOTA_MEDICA nota_medica,bool isExceptionManaged=false)
        {
            try
            {

            
            textAnioImputado = nota_medica.ATENCION_MEDICA.INGRESO.ID_ANIO.ToString();
            RaisePropertyChanged("TextAnioImputado");
            textFolioImputado = nota_medica.ATENCION_MEDICA.INGRESO.ID_IMPUTADO.ToString();
            RaisePropertyChanged("TextFolioImputado");
            textPaternoImputado = !string.IsNullOrWhiteSpace(nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO : string.Empty;
            RaisePropertyChanged("TextPaternoImputado");
            textMaternoImputado = !string.IsNullOrWhiteSpace(nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO :
                string.Empty;
            RaisePropertyChanged("TextMaternoImputado");
            textNombreImputado = nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE;
            RaisePropertyChanged("textNombreImputado");
            textSexoImputado = nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO";
            RaisePropertyChanged("TextSexoImputado");
            textEdadImputado = new Fechas().CalculaEdad(nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString();
            RaisePropertyChanged("TextEdadImputado");
            textFechaNacImputado = nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString();
            RaisePropertyChanged("TextFechaNacImputado");
            imagenIngreso = nota_medica.ATENCION_MEDICA.INGRESO.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                nota_medica.ATENCION_MEDICA.INGRESO.INGRESO_BIOMETRICO.First(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO :
                nota_medica.ATENCION_MEDICA.INGRESO.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                nota_medica.ATENCION_MEDICA.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                new Imagenes().getImagenPerson();
            RaisePropertyChanged("ImagenIngreso");
            #region signos vitales
            if (nota_medica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES!=null)
            {
                textNMFrecuenciaCardiaca = nota_medica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC;
                RaisePropertyChanged("textNMFrecuenciaCardiaca");
                textNMFrecuenciaRespira = nota_medica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA;
                RaisePropertyChanged("TextNMFrecuenciaRespira");
                textNMObservacionesSignosVitales = nota_medica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.OBSERVACIONES;
                RaisePropertyChanged("TextNMObservacionesSignosVitales");
                textNMPeso = nota_medica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO;
                RaisePropertyChanged("TextNMPeso");
                textNMTalla = nota_medica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TALLA;
                RaisePropertyChanged("TextNMTalla");
                textNMTemperatura = nota_medica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TEMPERATURA;
                RaisePropertyChanged("TextNMTemperatura");
                var _tension_arterial = !string.IsNullOrWhiteSpace(nota_medica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL) ? nota_medica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL.Split('/') : null;
                if (_tension_arterial != null)
                {
                    textNMTensionArterial1 = _tension_arterial[0];
                    RaisePropertyChanged("TextNMTensionArterial1");
                    textNMTensionArterial2 = _tension_arterial[1];
                    RaisePropertyChanged("TextNMTensionArterial2");
                }
            }
            #endregion
            #region exploracion fisica
            if ((nota_medica.ATENCION_MEDICA.ID_TIPO_ATENCION==(short)enumAtencionTipo.CONSULTA_MEDICA 
                && (nota_medica.ATENCION_MEDICA.ID_TIPO_SERVICIO==(short)enumAtencionServicio.CITA_MEDICA 
                || nota_medica.ATENCION_MEDICA.ID_TIPO_SERVICIO==(short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO 
                || nota_medica.ATENCION_MEDICA.ID_TIPO_SERVICIO==(short)enumAtencionServicio.CERTIFICADO_INTEGRIDAD_FISICA)))
            {
                exploracionFisicaVisible = Visibility.Visible;
                RaisePropertyChanged("ExploracionFisicaVisible");
                exploracionFisica = nota_medica.EXPLORACION_FISICA;
                RaisePropertyChanged("ExploracionFisica");
            }
            else
            {
                exploracionFisicaVisible = Visibility.Collapsed;
                RaisePropertyChanged("ExploracionFisicaVisible");            }
            #endregion
            #region topografia
            if (nota_medica.ATENCION_MEDICA.CERTIFICADO_MEDICO != null)
            {
                isCertificadoMedico = Visibility.Visible;
                RaisePropertyChanged("IsCertificadoMedico");
                lstLesiones = new ObservableCollection<LESION>(nota_medica.ATENCION_MEDICA.CERTIFICADO_MEDICO.LESION);
                RaisePropertyChanged("LstLesiones");
                textObservacionesConclusionesCertificado = nota_medica.ATENCION_MEDICA.CERTIFICADO_MEDICO.OBSERVACIONES;
                RaisePropertyChanged("TextObservacionesConclusionesCertificado");
                checked15DiasSanar = nota_medica.ATENCION_MEDICA.CERTIFICADO_MEDICO.TARDA_SANAR_15 == "S" ? true : false;
                RaisePropertyChanged("Checked15DiasSanar");
                checkedHospitalizacion = nota_medica.ATENCION_MEDICA.CERTIFICADO_MEDICO.AMERITA_HOSPITALIZACION == "S" ? true : false;
                RaisePropertyChanged("CheckedHospitalizacion");
                checkedPeligroVida = nota_medica.ATENCION_MEDICA.CERTIFICADO_MEDICO.PELIGRA_VIDA == "S" ? true : false;
                RaisePropertyChanged("checkedPeligroVida");
            }
            else
            {
                isCertificadoMedico = Visibility.Collapsed;
                RaisePropertyChanged("IsCertificadoMedico");
            }
            #endregion
            #region Diagnostico
            text_Pronostico_Descr =nota_medica.ID_PRONOSTICO.HasValue? nota_medica.PRONOSTICO.DESCR:string.Empty;
            RaisePropertyChanged("Text_Pronostico_Descr");
            lstEnfermedades =new ObservableCollection<NOTA_MEDICA_ENFERMEDAD>(nota_medica.NOTA_MEDICA_ENFERMEDAD);
            RaisePropertyChanged("LstEnfermedades");
            #endregion

            #region Tratamiento
            if (nota_medica.ATENCION_MEDICA.RECETA_MEDICA.Any())
            {
                receta_Medica = new ObservableCollection<RecetaMedica>(nota_medica.ATENCION_MEDICA.RECETA_MEDICA.First().RECETA_MEDICA_DETALLE.Select(s => new RecetaMedica
                {
                    CANTIDAD = s.DOSIS,
                    DURACION = s.DURACION,
                    MEDIDA = s.PRODUCTO.ID_UNIDAD_MEDIDA.Value,
                    PRODUCTO = s.PRODUCTO,
                    HORA_MANANA = s.DESAYUNO.HasValue && s.DESAYUNO.Value == 1 ? true : false,
                    HORA_NOCHE = s.CENA.HasValue && s.CENA.Value == 1 ? true : false,
                    HORA_TARDE = s.COMIDA.HasValue && s.COMIDA.Value == 1 ? true : false,
                }));
            }
            else
            {
                receta_Medica = null;
            }
            RaisePropertyChanged("Receta_Medica");
            lstDietas = new ObservableCollection<DietaMedica>(nota_medica.NOTA_MEDICA_DIETA.Select(s => new DietaMedica {
                DIETA=s.DIETA,
                ELEGIDO=true
            }));
            RaisePropertyChanged("LstDietas");
            #endregion
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la nota medica", ex);
            }
        }

        private async Task Guardar()
        {
            try
            {
                var _fecha_servidor = Fechas.GetFechaDateServer;
                
                if (modoVistaModelo==enumModo.INSERCION)
                {
                    short? _cierra_hoja_ref = null;
                    if (isSolicitudInternaVisible!=Visibility.Visible)
                    {
                        if (SelectedServAuxCanalizacion != null)
                            if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "¿Desea seguir agregando servicios auxiliares a esta hoja de referencia médica?") != 0)
                                _cierra_hoja_ref = 0;
                            else
                                _cierra_hoja_ref = 1;
                    }
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando solicitud", () =>
                    {
                        var _sol_interconsulta_interna = new List<SOL_INTERCONSULTA_INTERNA>();
                        var _hoja_ref = new List<HOJA_REFERENCIA_MEDICA>();
                        var _serv_aux_hoja_ref = new List<SERVICIO_AUX_INTERCONSULTA>();
                        var _estatus = string.Empty;
                        CANALIZACION _canalizacion = null;
                        List <CANALIZACION_SERV_AUX> _canalizacion_serv_aux = new List<CANALIZACION_SERV_AUX>();
                        if (selectedCanalizacion!=null)
                        {
                            _canalizacion = new CANALIZACION {
                                ID_ATENCION_MEDICA=selectedCanalizacion.ID_ATENCION_MEDICA,
                                ID_ESTATUS_CAN=selectedCanalizacion.ID_ESTATUS_CAN,
                                ID_FECHA=selectedCanalizacion.ID_FECHA,
                                ID_USUARIO=selectedCanalizacion.ID_USUARIO,
                                ID_CENTRO_UBI=selectedCanalizacion.ID_CENTRO_UBI
                            };
                            if (SelectedServAuxCanalizacion!=null)
                                _canalizacion_serv_aux.Add(new CANALIZACION_SERV_AUX {
                                    ID_ATENCION_MEDICA=selectedCanalizacion.ID_ATENCION_MEDICA,
                                    ID_ESTATUS="A",
                                    ID_FECHA=_fecha_servidor,
                                    ID_INIVEL=selectedServAuxCanalizacion.SelectedNivel_Prioridad,
                                    ID_INTER=selectedServAuxCanalizacion.SelectedInterconsulta_Tipo,
                                    ID_SERV_AUX=selectedServAuxCanalizacion.ID_SERV_AUX,
                                    ID_CENTRO_UBI=selectedCanalizacion.ID_CENTRO_UBI
                                });
                        }
                        if (selectedServAuxCanalizacion != null)
                            if (lstSerAuxHojaRef != null)
                                foreach (var item in lstSerAuxHojaRef)
                                    _serv_aux_hoja_ref.Add(new SERVICIO_AUX_INTERCONSULTA
                                    {
                                        ID_SERV_AUX = item.ID_SERV_AUX,
                                        REGISTRO_FEC = _fecha_servidor,
                                        ID_INTERSOL = selectedHojaReferenciaMedicaMem == null ? 0 : selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_INTERSOL,
                                        ID_CENTRO_UBI = selectedCanalizacion.ID_CENTRO_UBI
                                    });
                            else
                                _serv_aux_hoja_ref.Add(new SERVICIO_AUX_INTERCONSULTA {
                                    ID_SERV_AUX=selectedServAuxCanalizacion.ID_SERV_AUX,
                                    REGISTRO_FEC=_fecha_servidor,
                                    ID_INTERSOL=0,
                                    ID_CENTRO_UBI=selectedCanalizacion.ID_CENTRO_UBI
                                });
                       
                        if ((SelectedServAuxCanalizacion!=null && selectedServAuxCanalizacion.SelectedInterconsulta_Tipo==(short)enumInterconsulta_Tipo.EXTERNA) ||
                            (selectedExtEspecialidad!=null && selectedExtEspecialidad.SelectedInterconsulta_Tipo==(short)enumInterconsulta_Tipo.EXTERNA))
                        {
                            _hoja_ref.Add(new HOJA_REFERENCIA_MEDICA
                            {
                                ID_HOJA=selectedHojaReferenciaMedicaMem!=null?selectedHojaReferenciaMedicaMem.ID_HOJA:0,
                                EXP_HGT = textRefMedExpHGT,
                                ID_HOSPITAL = selectedRefMedHospital,
                                ID_TIPO_CITA = selectedRefMedTipoCitaValue,
                                MOTIVO = textRefMedMotivo,
                                OBSERV = textRefMedObservaciones,
                                HOSPITAL_OTRO = selectedRefMedHospital == otro_hospital ? textRefMedOtroHospital : null,
                                FECHA_CITA = FechaCita,
                                ID_INTERSOL = selectedHojaReferenciaMedicaMem == null ? 0 : selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_INTERSOL,
                                ID_CENTRO_UBI=selectedCanalizacion.ID_CENTRO_UBI
                            });
                        }
                        else if ((selectedServAuxCanalizacion!=null && selectedServAuxCanalizacion.SelectedInterconsulta_Tipo==(short)enumInterconsulta_Tipo.INTERNA) || 
                            (selectedExtEspecialidad!=null && selectedExtEspecialidad.SelectedInterconsulta_Tipo==(short)enumInterconsulta_Tipo.INTERNA))
                        {
                            _sol_interconsulta_interna.Add(new SOL_INTERCONSULTA_INTERNA {
                                ID_CENTRO = selectedSolIntCentro,
                                ID_USUARIO= GlobalVar.gUsr,
                                MOTIVO_INTERCONSULTA = textSolIntMotivo,
                                REGISTRO_FEC=_fecha_servidor,
                                ID_CENTRO_UBI=GlobalVar.gCentro
                            });
                        }
                        var _interconsulta_tipo = selectedServAuxCanalizacion != null ? selectedServAuxCanalizacion.SelectedInterconsulta_Tipo : selectedExtEspecialidad != null ? (short?)selectedExtEspecialidad.SelectedInterconsulta_Tipo : null;
                        var _interconsulta_solicitud = new INTERCONSULTA_SOLICITUD
                        {
                            ID_INTERSOL=selectedHojaReferenciaMedicaMem!=null?selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_INTERSOL:0,
                            SERVICIO_AUX_INTERCONSULTA =selectedHojaReferenciaMedicaMem==null?_serv_aux_hoja_ref:selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.SERVICIO_AUX_INTERCONSULTA,
                            ID_ESPECIALIDAD = selectedExtEspecialidad!=null?(short?)selectedExtEspecialidad.ID:null,
                            ESTATUS = _interconsulta_tipo.Value==(short)enumInterconsulta_Tipo.INTERNA?"S":_cierra_hoja_ref.HasValue && _cierra_hoja_ref!=1?"P":isfechacitarequerida ?"S":FechaCita.HasValue ? "S" : "E",
                            ID_INTER = _interconsulta_tipo,
                            ID_INTERAT = id_atencion_tipo, 
                            ID_NOTA_MEDICA = selectedCanalizacion.ID_ATENCION_MEDICA,
                            ID_USUARIO = GlobalVar.gUsr,
                            REGISTRO_FEC = _fecha_servidor,
                            HOJA_REFERENCIA_MEDICA =selectedHojaReferenciaMedicaMem==null? _hoja_ref:selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA,
                            SOL_INTERCONSULTA_INTERNA = _sol_interconsulta_interna,
                            ID_INIVEL = selectedServAuxCanalizacion!=null?selectedServAuxCanalizacion.SelectedNivel_Prioridad:selectedExtEspecialidad!=null?(short?)selectedExtEspecialidad.SelectedInterconsulta_Tipo:null,
                            ID_CENTRO_UBI=selectedCanalizacion.ID_CENTRO_UBI
                        };
                        if ((selectedHojaReferenciaMedicaMem==null && IsReferenciaVisible==Visibility.Visible)|| (IsSolicitudInternaVisible==Visibility.Visible))
                            new cInterconsulta_Solicitud().Insertar(_interconsulta_solicitud, GlobalVar.gCentro, (short)enumMensajeTipo.SOLICITUD_INTERCONSULTA_INTERNA,
                                (short)enumMensajeTipo.SOLICITUD_INTERCONSULTA_EXTERNA, _fecha_servidor);
                        else
                            new cInterconsulta_Solicitud().Actualizar(_interconsulta_solicitud,_fecha_servidor,false,_serv_aux_hoja_ref,_hoja_ref.FirstOrDefault(),null,_canalizacion,_canalizacion_serv_aux);
                        selectedCanalizacion = new cCanalizacion().ObtenerCanalizaciones(selectedCanalizacion.ID_ATENCION_MEDICA,selectedCanalizacion.ID_CENTRO_UBI).FirstOrDefault();
                        CargarDatosCanalizacion(selectedCanalizacion, true);
                        CargarDatosInterconsultaSolicitud(selectedCanalizacion, true);
                        return true;
                    }))
                    {
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La solicitud de interconsulta fue guardada con exito");
                        LimpiarHojaRefMedica();
                        LimpiarInterconsutaInterna();
                        ClearRules();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else if (modoVistaModelo==enumModo.REAGENDA)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando solicitud", () =>
                        {
                            var _hoja_ref = new HOJA_REFERENCIA_MEDICA();
                            if (isReferenciaVisible == Visibility.Visible)
                            {
                                _hoja_ref=new HOJA_REFERENCIA_MEDICA
                                {
                                    EXP_HGT = textRefMedExpHGT,
                                    ID_HOSPITAL = selectedRefMedHospital,
                                    ID_TIPO_CITA = selectedRefMedTipoCitaValue,
                                    MOTIVO = textRefMedMotivo,
                                    OBSERV = textRefMedObservaciones,
                                    HOSPITAL_OTRO = selectedRefMedHospital == otro_hospital ? textRefMedOtroHospital : null,
                                    FECHA_CITA = FechaCita,
                                    ID_HOJA=selectedHojaReferenciaMedicaMem.ID_HOJA,
                                    ID_INTERSOL = selectedHojaReferenciaMedicaMem.ID_INTERSOL,
                                    ID_CENTRO_UBI=selectedHojaReferenciaMedicaMem.ID_CENTRO_UBI
                                };

                            }
                            else
                                _hoja_ref = null;
                            var _interconsulta_solicitud = new INTERCONSULTA_SOLICITUD {
                                ESTATUS=FechaCita.HasValue?"S":selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ESTATUS,
                                ID_ESPECIALIDAD=selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_ESPECIALIDAD,
                                ID_INIVEL = selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_INIVEL,
                                ID_INTER = selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_INTER,
                                ID_INTERAT = selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_INTERAT,
                                ID_INTERSOL = selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_INTERSOL,
                                ID_NOTA_MEDICA = selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_NOTA_MEDICA,
                                ID_USUARIO = selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_USUARIO,
                                OFICIO_EXC = selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.OFICIO_EXC,
                                REGISTRO_FEC = selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.REGISTRO_FEC,
                                ID_CENTRO_UBI=selectedHojaReferenciaMedicaMem.INTERCONSULTA_SOLICITUD.ID_CENTRO_UBI
                            };
                            new cInterconsulta_Solicitud().Actualizar(_interconsulta_solicitud, _fecha_servidor, true, null, _hoja_ref, null, null, null, null,
                                (short)enumMensajeTipo.REAGENDA_SOLICITUD_INTERCONSULTA_EXTERNA);
                            return true;
                        }
                        ))
                    {
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La solicitud de interconsulta fue guardada con exito");
                        LimpiarHojaRefMedica();
                        if (IsModoInsercion)
                        {
                            selectedCanalizacion = new cCanalizacion().ObtenerCanalizaciones(selectedCanalizacion.ID_ATENCION_MEDICA, selectedCanalizacion.ID_CENTRO_UBI).FirstOrDefault();
                            CargarDatosCanalizacion(selectedCanalizacion, true);
                            CargarDatosInterconsultaSolicitud(selectedCanalizacion, true);
                        }
                        ClearRules();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    
                }
                else if (modoVistaModelo==enumModo.CANCELAR)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cancelando solicitud", () =>
                    {
                        var _canalizacion_serv_aux = new List<CANALIZACION_SERV_AUX>();
                        foreach (var item in selectedInterconsulta_solicitud.SERVICIO_AUX_INTERCONSULTA)
                        {
                            var _item_canalizacion_serv_aux = selectedInterconsulta_solicitud.CANALIZACION.CANALIZACION_SERV_AUX.FirstOrDefault(w => w.ID_SERV_AUX == item.ID_SERV_AUX);
                            if (_item_canalizacion_serv_aux!=null)
                                _canalizacion_serv_aux.Add(new CANALIZACION_SERV_AUX {
                                    ID_ATENCION_MEDICA = _item_canalizacion_serv_aux.ID_ATENCION_MEDICA,
                                    ID_CENTRO_UBI=item.ID_CENTRO_UBI,
                                    ID_ESTATUS="P",
                                    ID_FECHA = _item_canalizacion_serv_aux.ID_FECHA,
                                    ID_INIVEL = _item_canalizacion_serv_aux.ID_INIVEL,
                                    ID_INTER = _item_canalizacion_serv_aux.ID_INTER,
                                    ID_SERV_AUX=item.ID_SERV_AUX
                                });
                        }
                        var _canalizacion_especialidad = new List<CANALIZACION_ESPECIALIDAD>();
                        foreach(var item in selectedInterconsulta_solicitud.CANALIZACION.CANALIZACION_ESPECIALIDAD)
                        {
                            var _item_canalizacion_especialidad = selectedInterconsulta_solicitud.CANALIZACION.CANALIZACION_ESPECIALIDAD.FirstOrDefault(w => w.ID_ESPECIALIDAD == item.ID_ESPECIALIDAD);
                            _canalizacion_especialidad.Add(new CANALIZACION_ESPECIALIDAD {
                                ID_ATENCION_MEDICA=item.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI=item.ID_CENTRO_UBI,
                                ID_ESPECIALIDAD=item.ID_ESPECIALIDAD,
                                ID_ESTATUS="P",
                                ID_FECHA=item.ID_FECHA,
                                ID_INIVEL=item.ID_INIVEL,
                                ID_INTER=item.ID_INTER,
                                ID_INTERAT=item.ID_INTERAT
                            });
                        }
                        var _canalizacion = new CANALIZACION { 
                            ID_ATENCION_MEDICA=selectedInterconsulta_solicitud.CANALIZACION.ID_ATENCION_MEDICA,
                            ID_CENTRO_UBI=selectedInterconsulta_solicitud.CANALIZACION.ID_CENTRO_UBI,
                            ID_ESTATUS_CAN="P",
                            ID_FECHA=selectedInterconsulta_solicitud.CANALIZACION.ID_FECHA,
                            ID_USUARIO=selectedInterconsulta_solicitud.CANALIZACION.ID_USUARIO
                        };
                        var _interconsulta_solicitud = new INTERCONSULTA_SOLICITUD
                        {
                            ESTATUS = "C",
                            ID_ESPECIALIDAD = selectedInterconsulta_solicitud.ID_ESPECIALIDAD,
                            ID_INIVEL = selectedInterconsulta_solicitud.ID_INIVEL,
                            ID_INTER = selectedInterconsulta_solicitud.ID_INTER,
                            ID_INTERAT = selectedInterconsulta_solicitud.ID_INTERAT,
                            ID_INTERSOL = selectedInterconsulta_solicitud.ID_INTERSOL,
                            ID_NOTA_MEDICA = selectedInterconsulta_solicitud.ID_NOTA_MEDICA,
                            ID_USUARIO = selectedInterconsulta_solicitud.ID_USUARIO,
                            OFICIO_EXC = selectedInterconsulta_solicitud.OFICIO_EXC,
                            REGISTRO_FEC = selectedInterconsulta_solicitud.REGISTRO_FEC,
                            ID_CENTRO_UBI=selectedInterconsulta_solicitud.ID_CENTRO_UBI
                        };
                        new cInterconsulta_Solicitud().Actualizar(_interconsulta_solicitud, _fecha_servidor, false, null, null, null, _canalizacion, _canalizacion_serv_aux, _canalizacion_especialidad, null,
                            _interconsulta_solicitud.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA ? (short?)enumMensajeTipo.CANCELACION_SOLICITUD_INTERCONSULTA_EXTERNA : null, _interconsulta_solicitud.ID_INTER == (short)enumInterconsulta_Tipo.INTERNA ? (short?)enumMensajeTipo.CANCELACION_SOLICITUD_INTERCONSULTA_INTERNA : null);
                        return true;
                    }
                        ))
                    {
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La solicitud de interconsulta fue cancelada con exito");
                        Limpiar();
                        IsInterconsultaEnabled = false;
                        ClearRules();
                        StaticSourcesViewModel.SourceChanged = false;
                        selectedInterconsulta_solicitud = null;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la solicitud de interconsulta", ex);
            }
        }

        private void Limpiar()
        {
            ImagenIngreso = null;
            TextAnioImputado = string.Empty;
            TextEdadImputado = string.Empty;
            TextFechaNacImputado = string.Empty;
            TextFolioImputado = string.Empty;
            TextMaternoImputado = string.Empty;
            TextNombreImputado = string.Empty;
            TextPaternoImputado = string.Empty;
            TextRefMedExpHGT = string.Empty;
            TextRefMedMotivo = string.Empty;
            TextRefMedObservaciones = string.Empty;
            TextSexoImputado = string.Empty;
            TextSolIntMotivo = string.Empty;
            SelectedInterconsultaTipo = -1;
            SelectedInterconsultaAtencionTipo = -1;
            SelectedNvlPrioridad = -1;
            IsEspecialidadVisible = Visibility.Collapsed;
            IsReferenciaVisible = Visibility.Collapsed;
            IsSolicitudInternaVisible = Visibility.Collapsed;
            IsServAuxSeleccionadosValid = false;
            IsFechaCitaValid = false;
            isfechacitarequerida = false;
            FechaCita = null;
            #region Nota Medica
            IsCertificadoMedico = Visibility.Collapsed;
            TextNMPeso = string.Empty;
            TextNMTalla = string.Empty;
            TextNMTensionArterial1 = string.Empty;
            TextNMTensionArterial2 = string.Empty;
            textNMFrecuenciaCardiaca = string.Empty;
            TextNMFrecuenciaRespira = string.Empty;
            TextNMTemperatura = string.Empty;
            TextNMObservacionesSignosVitales = string.Empty;
            LstLesiones = null;
            Text_Pronostico_Descr = string.Empty;
            ExploracionFisica = string.Empty;
            LstEnfermedades = null;
            CheckedHospitalizacion = false;
            CheckedPeligroVida = false;
            Checked15DiasSanar = false;
            Receta_Medica = null;
            LstDietas = null;
            TextObservacionesConclusionesCertificado = string.Empty;
            #endregion
            FechaServer = Fechas.GetFechaDateServer;
            id_atencion_tipo = null;
        }

        private void LimpiarHojaRefMedica()
        {
            IsReferenciaVisible=Visibility.Collapsed;
            IsServicioAuxiliarHojaRefVisible = Visibility.Collapsed;
            LstSerAuxHojaRef = null;
            //SelectedServAuxCanalizacion = null;
            SelectedRefMedHospital = -1;
            LstHospitales = null;
            IsOtroHospitalSelected = Visibility.Collapsed;
            TextRefMedOtroHospital = string.Empty;
            TextRefMedExpHGT = string.Empty;
            LstTipoCitas = null;
            SelectedRefMedTipoCitaValue = -1;
            IsFechaCitaValid = false;
            FechaCita = null;
            TextRefMedMotivo = string.Empty;
            TextRefMedObservaciones = string.Empty;
        }

        private void LimpiarInterconsutaInterna()
        {
            selectedSolIntCentro = -1;
            RaisePropertyChanged("SelectedSolIntCentro");
            textSolIntMotivo = string.Empty;
            RaisePropertyChanged("TextSolIntMotivo");
            isSolicitudInternaVisible = Visibility.Collapsed;
            RaisePropertyChanged("IsSolicitudInternaVisible");
        }

        private void LimpiarBusqueda()
        {
            FolioBuscarInter = null;
            AnioBuscarInter = null;
            NombreBuscarInter = null;
            ApellidoPaternoBuscarInter = null;
            ApellidoMaternoBuscarInter = null;
            FechaInicialBuscarInter = null ;
            FechaFinalBuscarInter = null;
            //SelectedAtencion_TipoBuscarValue = -1;
            //SelectedInter_TipoBuscarValue = -1;
            SelectedInterconsultaBusqueda = null;
        }

        private async void  OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_interconsulta_tipo_atencion":
                    LstServAuxSeleccionados = null;
                    IsServAuxSeleccionadosValid = false;
                    var _interconsulta_atencion_tipo = lstInterconsultaAtencionTipo.FirstOrDefault(w => w.ID_INTERAT == selectedInterconsultaAtencionTipo);
                    if (_interconsulta_atencion_tipo!=null)
                    {
                        if (_interconsulta_atencion_tipo.CATALOGO_ESPECIALIDADES=="1")
                        {
                            IsEspecialidadVisible = Visibility.Visible;
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                                CargarEspecialidades(true);
                                selectedEspecialidad = -1;
                                RaisePropertyChanged("SelectedEspecialidad");
                            });
                            setValidacionesEspecialidad();
                            clearValidacionesServAux();
                        }
                        else if (_interconsulta_atencion_tipo.CATALOGO_SERVICIOS_ESP=="1")
                        {
                            IsEspecialidadVisible = Visibility.Collapsed;

                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                CargarTipo_Servicios_Auxiliares(true);
                                CargarSubTipo_Servicios_Auxiliares(-1, true);
                                CargarServicioAuxiliar(-1, -1, true);
                                selectedTipoServAux = -1;
                                RaisePropertyChanged("SelectedTipoServAux");
                                selectedSubtipoServAux = -1;
                                RaisePropertyChanged("SelectedSubtipoServAux");
                            });
                            setValidacionesServAux();
                            clearValidacionesEspecialidad();
                        }
                        else
                        {
                            IsEspecialidadVisible = Visibility.Collapsed;
                            clearValidacionesEspecialidad();
                            clearValidacionesInterconsultaExterna();
                        }
                    }
                    break;
                case "cambio_tipo_serv_aux":

                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        
                        CargarSubTipo_Servicios_Auxiliares(selectedTipoServAux, true);
                        CargarServicioAuxiliar(selectedTipoServAux, -1, true);
                        selectedSubtipoServAux = -1;
                        RaisePropertyChanged("SelectedSubtipoServAux");
                    });

                    break;
                case "cambio_subtipo_serv_aux":
  
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        CargarServicioAuxiliar(selectedTipoServAux, selectedSubtipoServAux, true);
                    });

                    break;
                case "cambio_interconsulta_tipo":

                    if (selectedInterconsultaTipo==1)  //Si la interconsulta es interna
                    {
                        isfechacitarequerida = false;
                        IsSolicitudInternaVisible = Visibility.Visible;
                        IsReferenciaVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            CargarCentros(true);
                            selectedSolIntCentro = -1;
                            RaisePropertyChanged("SelectedSolIntCentro");
                        });                       
                        clearValidacionesInterconsultaExterna();
                        setValidacionesRefMedica();
                    }
                    else if (selectedInterconsultaTipo == 2) //Si es externa
                    {
                        isfechacitarequerida = false;
                        IsFechaCitaValid = true;
                        IsOtroHospitalSelected = Visibility.Collapsed;
                        IsReferenciaVisible = Visibility.Visible;
                        IsSolicitudInternaVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            CargarHospitales(true);
                            CargarTipoCitas(true);
                            selectedRefMedHospital = -1;
                            RaisePropertyChanged("SelectedRefMedHospital");
                            selectedRefMedTipoCitaValue = -1;
                            RaisePropertyChanged("SelectedRefMedTipoCitaValue");
                        });
                        clearValidacionesRefMedica();
                        setValidacionesInterconsultaExterna();
                    }
                    else
                    {
                        IsReferenciaVisible = Visibility.Collapsed;
                        IsSolicitudInternaVisible = Visibility.Collapsed;
                        clearValidacionesInterconsultaExterna();
                        clearValidacionesRefMedica();
                    }
                    break;
                case "cambio_hospital":
                    if (selectedRefMedHospital == otro_hospital)
                    {
                        IsOtroHospitalSelected = Visibility.Visible;
                        setValidacionesOtroHospital();
                    }
                    else
                    {
                        IsOtroHospitalSelected = Visibility.Collapsed;
                        clearValidacionesOtroHospital();
                    }
                    break;
                case "cambio_tipo_citas":
                    if (SelectedRefMedTipoCitaValue==(short)enumCita_Tipo.PRIMERA)
                    {
                        isfechacitarequerida = false;
                        if (!FechaCita.HasValue || (FechaCita.HasValue && FechaCita >= FechaServer))
                            IsFechaCitaValid = true;
                        else
                            IsFechaCitaValid = false;
                    }
                    else
                    {
                        isfechacitarequerida = true;
                        if (modoVistaModelo==enumModo.INSERCION)
                        {
                            if (!FechaCita.HasValue)
                                IsFechaCitaValid = false;

                        }
                        else
                        {
                            if (!FechaCita.HasValue || (FechaCita.HasValue && FechaCita < FechaServer))
                                IsFechaCitaValid = false;
                        }                            
                    }
                    break;
                case "cambio_nivel_prioridad":
                    //SelectedInterconsultaTipo=2 Externa
                    //SelectedNvlPrioridad=2 Urgente
                    if (SelectedInterconsultaTipo == 2 && SelectedNvlPrioridad == 2)
                        FechaCita = FechaServer.AddMinutes(30);
                    break;
                case "cambio_fecha_inicio_busqueda_canal":
                    if (!fechaInicialBuscarCanal.HasValue || !FechaFinalBuscarCanal.HasValue || FechaFinalBuscarCanal >= FechaInicialBuscarCanal)
                        IsFechaIniBusquedaCanalValida = true;
                    else
                        IsFechaIniBusquedaCanalValida = false;
                    break;
                case "cambio_fecha_inicio_busqueda_sol":
                    if (!FechaInicialBuscarInter.HasValue || !FechaFinalBuscarInter.HasValue || FechaFinalBuscarInter >= FechaInicialBuscarInter)
                        IsFechaIniBusquedaSolValida = true;
                    else
                        IsFechaIniBusquedaSolValida = false;
                    break;
                case "cambio_fecha_cita":
                    if ((isfechacitarequerida && !FechaCita.HasValue) || (FechaCita.HasValue && FechaCita < FechaServer))
                        IsFechaCitaValid = false;
                    else
                        IsFechaCitaValid = true;
                    break;
                
            }
        }


        private void BuscarCanalizacion(short? tipo_atencion=null,short? anio=null, int? folio=null, string nombre="", string paterno="", string materno="", DateTime? fecha_inicio=null, DateTime? fecha_fin=null, bool isExceptionManaged=false)
        {
            try
            {
                lstCanalizacionesBusqueda = new ObservableCollection<CANALIZACION>(new cCanalizacion().ObtenerCanalizaciones("P",GlobalVar.gCentro,estatus_administrativos_inactivos,
                    tipo_atencion,anio, folio, nombre, paterno, materno, fecha_inicio, fecha_fin).OrderByDescending(o=>o.ID_FECHA));
                RaisePropertyChanged("LstCanalizacionesBusqueda");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de interconsulta", ex);
            }
        }

        #endregion

        #region catalogos
        private void CargarAtencionTipoBuscar(bool isExceptionManaged=false)
        {
            try
            {
                lstAtencion_TipoBuscar = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w=>w.ESTATUS=="S"));
                lstAtencion_TipoBuscar.Add(new ATENCION_TIPO {
                    DESCR="SELECCIONE UNO",
                    ID_TIPO_ATENCION=-1
                });
                RaisePropertyChanged("LstAtencion_TipoBuscar");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error el cargar tipos de interconsulta", ex);
            }
        }


        private void CargarExtEspecialidadTipoInterconsulta(bool isExceptionManaged = false)
        {
            try
            {
                lstExtEspecialidadInterconsulta_Tipo = new cInterconsultaTipo().ObtenerTodos("S").ToList();
                lstExtEspecialidadInterconsulta_Tipo.Insert(0, new INTERCONSULTA_TIPO
                {
                    ID_INTER = -1,
                    DESCR = "SELECCIONE UNO"
                });
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de interconsulta", ex);
            }

        }
        private void CargarTipoInterconsulta(bool isExceptionManaged = false)
        {
            try
            {
                lstInterconsultaTipo = new ObservableCollection<INTERCONSULTA_TIPO>(new cInterconsultaTipo().ObtenerTodos("S"));
                lstInterconsultaTipo.Insert(0, new INTERCONSULTA_TIPO {
                    ID_INTER=-1,
                    DESCR="SELECCIONE UNO"
                });
                RaisePropertyChanged("LstInterconsultaTipo");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de interconsulta", ex);
            }

        }

        private void CargarInterconsultaAtencionTipo(bool isExceptionManaged=false)
        {
            try
            {
                lstInterconsultaAtencionTipo = new ObservableCollection<INTERCONSULTA_ATENCION_TIPO>(new cInterconsultaAtencionTipo().ObtenerTodos("","S"));
                lstInterconsultaAtencionTipo.Insert(0, new INTERCONSULTA_ATENCION_TIPO {
                    ID_INTERAT=-1,
                    DESCR="SELECCIONE UNO"
                });
                RaisePropertyChanged("LstInterconsultaAtencionTipo");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de atención de interconsulta", ex);
            }
        }

        private void CargarExtEspecialidadInterconsultaNivelPrioridad(bool isExceptionManaged = false)
        {
            try
            {
                lstExtEspecialidadNvlPrioridades = new cNivelPrioridad().ObtenerTodos("S").ToList();
                lstExtEspecialidadNvlPrioridades.Insert(0, new INTERCONSULTA_NIVEL_PRIORIDAD
                {
                    ID_INIVEL = -1,
                    DESCR = "SELECCIONE UNO"
                });
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar nivel de prioridades", ex);
            }
        }

        private void CargarInterconsultaNivelPrioridad(bool isExceptionManaged = false)
        {
            try
            {
                lstNvlPrioridades = new ObservableCollection<INTERCONSULTA_NIVEL_PRIORIDAD>(new cNivelPrioridad().ObtenerTodos("S"));
                lstNvlPrioridades.Insert(0, new INTERCONSULTA_NIVEL_PRIORIDAD
                {
                    ID_INIVEL = -1,
                    DESCR = "SELECCIONE UNO"
                });
                RaisePropertyChanged("LstNvlPrioridades");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar nivel de prioridades", ex);
            }
        }

        private void CargarEspecialidades(bool isExceptionManaged=false)
        {
            try
            {
                lstEspecialidad = new ObservableCollection<ESPECIALIDAD>(new cEspecialidades().ObtenerTodos("", "S"));
                lstEspecialidad.Insert(0, new ESPECIALIDAD
                {
                    ID_ESPECIALIDAD = -1,
                    DESCR = "SELECCIONE UNA"
                });
                RaisePropertyChanged("LstEspecialidad");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las especialidades", ex);
            }
        }

        private void CargarTipo_Servicios_Auxiliares(bool isExceptionManaged=false)
        {
            try
            {
                lstTipoServAux = new ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT>(new cTipo_Serv_Aux_Diag_Trat().ObtenerTodos("","S"));
                lstTipoServAux.Insert(0, new TIPO_SERVICIO_AUX_DIAG_TRAT
                {
                    ID_TIPO_SADT=-1,
                    DESCR = "SELECCIONE UNA"
                });
                RaisePropertyChanged("LstTipoServAux");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de servicios auxiliares", ex);
            }
        }

        private void CargarSubTipo_Servicios_Auxiliares(short? tipo_serv_aux,bool isExceptionManaged=false)
        {
            try
            {
                lstSubtipoServAux = new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(tipo_serv_aux, "S"));
                lstSubtipoServAux.Insert(0, new SUBTIPO_SERVICIO_AUX_DIAG_TRAT
                {
                    ID_SUBTIPO_SADT = -1,
                    DESCR = "SELECCIONE UNA"
                });
                RaisePropertyChanged("LstSubtipoServAux");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", ex);
            }
        }

        private void CargarServicioAuxiliar(short tipo_serv_aux,short? subtipo_serv_aux, bool isExceptionManaged=false)
        {
            try
            {
                lstServAux = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(new cServ_Aux_Diag_Trat().ObtenerTodos(tipo_serv_aux,subtipo_serv_aux, "S").Select(s => new EXT_SERV_AUX_DIAGNOSTICO {
                    DESCR=s.DESCR,
                    ESTATUS=s.ESTATUS,
                    ID_SERV_AUX=s.ID_SERV_AUX,
                    ID_SUBTIPO_SADT=s.ID_SUBTIPO_SADT,
                    ISCHECKED=false,
                    SUBTIPO_DESCR=s.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR
                }));
                if (lstServAuxSeleccionados!=null)
                {
                    if (subtipo_serv_aux.HasValue)
                    {
                        foreach(var item in lstServAuxSeleccionados.Where(w=>w.ID_SUBTIPO_SADT==subtipo_serv_aux.Value))
                        {
                            if (item.ISCHECKED)
                            {
                                var item_catalogo=lstServAux.FirstOrDefault(w=>w.ID_SERV_AUX==item.ID_SERV_AUX);
                                if (item_catalogo != null)
                                    item_catalogo.ISCHECKED = true;
                            }
                        }
                    }
                }
                RaisePropertyChanged("LstServAux");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los servicios auxiliares", ex);
            }
        }

        private void CargarHospitales(bool isExceptionManaged=false)
        {
            try
            {
                lstHospitales = new ObservableCollection<HOSPITAL>(new cHospitales().Seleccionar(true));
                lstHospitales.Insert(0, new HOSPITAL
                {
                    ID_HOSPITAL = -1,
                    DESCR = "SELECCIONE UNO"
                });
                RaisePropertyChanged("LstHospitales");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los hospitales", ex);
            }
        }

        private void CargarTipoCitas(bool isExceptionManaged = false)
        {
            try
            {
                lstTipoCitas = new ObservableCollection<CITA_TIPO>(new cCita_Tipo().ObtenerTodos("S"));
                lstTipoCitas.Insert(0, new CITA_TIPO
                {
                    ID_TIPO_CITA = -1,
                    DESCR = "SELECCIONE UNO"
                });
                RaisePropertyChanged("LstTipoCitas");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de cita para interconsulta", ex);
            }
        }

        private void CargarCentros(bool isExceptionManaged)
        {
            try
            {
                lstCentros = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos("",0,0,"S"));
                lstCentros.Insert(0, new CENTRO
                {
                    ID_CENTRO = -1,
                    DESCR = "SELECCIONE UNO"
                });
                RaisePropertyChanged("LstCentros");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de cita para interconsulta", ex);
            }
        }

        private void CargarTipoInterconsultaBuscar(bool isExceptionManaged=false)
        {
            try
            {
                lstInterconsulta_TiposBuscar = new ObservableCollection<INTERCONSULTA_TIPO>(new cInterconsultaTipo().ObtenerTodos("S"));
                lstInterconsulta_TiposBuscar.Insert(0, new INTERCONSULTA_TIPO
                {
                    ID_INTER = -1,
                    DESCR = "SELECCIONE UNO"
                });
                RaisePropertyChanged("LstInterconsulta_TiposBuscar");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de interconsulta", ex);
            }
        }

        private void CargarAtencionTipoCanalBuscar(bool isExceptionManaged = false)
        {
            try
            {
                lstAtencion_TipoCanalBuscar = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w=>w.ESTATUS=="S"));
                lstAtencion_TipoCanalBuscar.Insert(0, new ATENCION_TIPO
                {
                    ID_TIPO_ATENCION=-1,
                    DESCR="SELECCIONE UNA"
                });
                RaisePropertyChanged("LstAtencion_TipoCanalBuscar");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar las solicitudes de interconsulta", ex);
            }
        }


        #endregion

        #region  NotaMedica 
        #region Topografia de Lesiones
        private void SeniasFrenteLoad(SI_SeniasFrenteView Window = null)
        {
            try
            {
                if (Window == null)
                    return;
                if (selectedCanalizacion == null)
                    return;
                CheckBoxFrente = new FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)Window.FindName("GridFrente"))).ToList();
                if (LstLesiones!=null)
                {
                    var _lesiones = LstLesiones.ToList();
                    foreach (var item in CheckBoxFrente)
                        foreach (var _item_lesion in _lesiones)
                            if (item.CommandParameter != null && item.CommandParameter.ToString().Split('-')[0] == _item_lesion.ID_REGION.ToString())
                            {
                                item.IsChecked = true;
                                break;
                            }
                            else
                                item.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar datos iniciales de las lesiones.", ex);
            }
        }

        private void SeniasDorsoLoad(SI_SeniasDorsoView Window = null)
        {
            try
            {
                if (Window == null)
                    return;
                if (selectedCanalizacion == null)
                    return;
                
                ListCheckBoxDorso = new FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)Window.FindName("GridDorso"))).ToList();
                if (LstLesiones != null)
                {
                    var _lesiones = LstLesiones.ToList();
                    foreach (var item in ListCheckBoxDorso)
                        foreach (var _item_lesion in _lesiones)
                            if (item.CommandParameter != null && item.CommandParameter.ToString().Split('-')[0] == _item_lesion.ID_REGION.ToString())
                            {
                                item.IsChecked = true;
                                break;
                            }
                            else
                                item.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar datos iniciales de las lesiones.", ex);
            }
        }
        #endregion
        #endregion

        #region Buscar
        private void BuscarSolicitudesInterconsulta(short? tipo_atencion=null,short?anio_imputado=null, int?folio_imputado=null,string nombre="",string paterno="", string materno="",short? tipo_interconsulta=null,
            DateTime? fecha_inicio=null,DateTime? fecha_final=null,bool isExceptionManaged=false)
        {
            try
            {
                listaInterconsultasBusqueda = new ObservableCollection<INTERCONSULTA_SOLICITUD>(new cInterconsulta_Solicitud().Buscar(GlobalVar.gCentro,new List<string> { "E", "S" }, tipo_atencion, anio_imputado, folio_imputado, 
                    nombre,paterno,materno,tipo_interconsulta,fecha_inicio,fecha_final));
                RaisePropertyChanged("ListaInterconsultasBusqueda");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar las solicitudes de interconsulta", ex);
            }
        }


        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                MenuBuscarEnabled = false;
                _permisos_editar = false;
                _permisos_agregar = false;
                permisos = new cProcesoUsuario().Obtener(enumProcesos.SOLICITUDINTERCONSULTA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Count() > 0)
                    MenuLimpiarEnabled = true;
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                    {
                        MenuAgregarEnabled = true;
                        _permisos_agregar = true;
                    }
                    if (p.CONSULTAR == 1)
                    {
                        MenuBuscarEnabled = true;
                    }
                    if(p.EDITAR==1)
                    {
                        MenuEditarEnabled = true;
                        EliminarMenuEnabled = true;
                        _permisos_editar = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion



        private enum enumCita_Tipo
        {
            PRIMERA=1,
            SUBSECUENTE=2
        }

        private enum enumModo
        {
            INSERCION=1,
            REAGENDA=2,
            CANCELAR=3
        }

        private enum enumTipoServicioInterconsulta
        {
            ESPECIALIDAD=1,
            SERV_AUXILIAR=2
        }
    }
}

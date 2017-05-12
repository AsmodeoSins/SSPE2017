using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReAgendaTrasladoVirtualViewModel:ValidationViewModelBase
    {
        #region Generales
       

        private async void ReAgendaTrasladoVirtualOnLoading(ReAgendaTrasladoVirtualView window)
        {
            estatus_administrativos_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                otro_hospital=Parametro.ID_HOSPITAL_OTROS;
                CargarCentrosBusqueda(true);
                selectedCentro_OrigenBuscarValue = -1;
                RaisePropertyChanged("SelectedCentro_OrigenBuscarValue");
            });

        }

        private async void ClickSwitch(object parametro)
        {
            try
            {
                if (parametro!=null && !string.IsNullOrWhiteSpace(parametro.ToString()))
                    switch (parametro.ToString())
                    {
                        case "buscar_menu":
                            LimpiarBuscarTV();
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                                BusquedaTrasladoVirtual(GlobalVar.gCentro, null, null, string.Empty, string.Empty, string.Empty, null, null, null, true);
                            });
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_TV_MEDICO_PENDIENTE);
                            break;
                        case "seleccionar_buscar_TV":
                           if(selectedTVBusqueda==null)
                           {
                               new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un traslado");
                               return;
                           }
                           AgregarCitaMedicaAgendaHoraI = DateTime.Now;
                           AgregarCitaMedicaAgendaHoraF = DateTime.Now;
                           StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                           {
                               BuscarIngreso(Convert.ToInt16(selectedTVBusqueda.ID_CENTRO), Convert.ToInt16(selectedTVBusqueda.ID_ANIO), selectedTVBusqueda.ID_IMPUTADO, Convert.ToInt16(selectedTVBusqueda.ID_INGRESO));
                               textAnioImputado = selectedIngreso.ID_ANIO.ToString();
                               RaisePropertyChanged("TextAnioImputado");
                               textFolioImputado = selectedIngreso.ID_IMPUTADO.ToString();
                               RaisePropertyChanged("TextFolioImputado");
                               textPaternoImputado = selectedIngreso.IMPUTADO.PATERNO;
                               RaisePropertyChanged("TextPaternoImputado");
                               textMaternoImputado = selectedIngreso.IMPUTADO.MATERNO;
                               RaisePropertyChanged("TextMaternoImputado");
                               textNombreImputado = selectedIngreso.IMPUTADO.NOMBRE;
                               RaisePropertyChanged("TextNombreImputado");
                               textSexoImputado = !string.IsNullOrWhiteSpace(selectedIngreso.IMPUTADO.SEXO) ? selectedIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO" : "";
                               RaisePropertyChanged("TextSexoImputado");
                               textEdadImputado = selectedIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? new Fechas().CalculaEdad(selectedIngreso.IMPUTADO.NACIMIENTO_FECHA.Value).ToString() : "";
                               RaisePropertyChanged("TextEdadImputado");
                               textFechaNacImputado = selectedIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? selectedIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString() : "";
                               RaisePropertyChanged("TextFechaNacImputado");
                               var foto_seguimiento = selectedIngreso.INGRESO_BIOMETRICO.FirstOrDefault(f => f.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO);
                               imagenIngreso = foto_seguimiento != null ? (foto_seguimiento.BIOMETRICO) : new Imagenes().getImagenPerson();
                               RaisePropertyChanged("ImagenIngreso");
                               CargarPendientesReagenda(true);
                           });
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_TV_MEDICO_PENDIENTE);
                            break;
                        case "cancelar_buscar_TV":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_TV_MEDICO_PENDIENTE);
                            break;
                        case "filtro_TVBuscar":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                BusquedaTrasladoVirtual(GlobalVar.gCentro,anioBuscarTV, folioBuscarTV, nombreBuscarTV, apellidoPaternoBuscarTV, apellidoMaternoBuscarTV, selectedCentro_OrigenBuscarValue!=-1?selectedCentro_OrigenBuscarValue:(short?)null, FechaInicialBuscarTV, FechaFinalBuscarTV, true);
                            });
                            break;
                        case "reagendar":
                            if (selectedPendienteTV==null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione una atencion pendiente");
                                return;
                            }
                            
                            switch(selectedPendienteTV.TIPO)
                            {
                                case "CITA_MEDICA":
                                    LimpiarReagendarCita();
                                    StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                                        CargarEmpleados(true);
                                        selectedCitaAgendaEmpleadoValue = lstCitaMedicaAgendaEmpleados.First().ID_EMPLEADO;
                                        RaisePropertyChanged("SelectedCitaAgendaEmpleadoValue");
                                        CargarAreas(true);
                                        selectedCitaMedicaArea = -1;
                                        RaisePropertyChanged("SelectedCitaMedicaArea");
                                        modo_guardar_seleccionado = MODO_REAGENDA.CITA_MEDICA;
                                    });
                                    setAgregarAgendaValidacionRules();
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.REAGENDAR_TV_CITA_MEDICA);
                                    break;
                                case "CANALIZACION":
                                    if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                        "Se va a recrear la solicitud de canalización,¿desea generarla?") != 1)
                                        return;
                                    modo_guardar_seleccionado = MODO_REAGENDA.CANALIZACION;
                                    Guardar();
                                    break;
                                case "INTERCONSULTA_SOLICITUD":
                                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                                        CargarTV_Inter_Tipos(true);
                                        CargarTV_Inter_Prioridad(true);
                                        CargarTV_InterCentros(true);
                                        CargarTV_Inter_Hospital(true);
                                        CargarTV_Inter_Cita_Tipo(true);
                                        selectedTV_Inter_TipoValue = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_INTER.Value;
                                        RaisePropertyChanged("SelectedTV_Inter_TipoValue");
                                        selectedTV_Inter_PrioridadValue = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_INIVEL.Value;
                                        RaisePropertyChanged("SelectedTV_Inter_PrioridadValue");
                                        if (selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_INTER == (short)enumInterconsulta_Tipo.INTERNA)
                                        {
                                            tv_Inter_Externa_Visible = Visibility.Collapsed;
                                            RaisePropertyChanged("TV_Inter_Externa_Visible");
                                            tv_Inter_Interna_Visible = Visibility.Visible;
                                            RaisePropertyChanged("TV_Inter_Interna_Visible");
                                            selectedTV_Inter_CentroValue = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_SOL_INTERCONSULTA_INTERNA.First().ID_CENTRO.Value;
                                            RaisePropertyChanged("SelectedTV_Inter_CentroValue");
                                            tv_Inter_Motivo = selectedPendienteTV.TV_SOL_INTERCONSULTA_INTERNA_REG.MOTIVO_INTERCONSULTA;
                                            RaisePropertyChanged("TV_Inter_Motivo");
                                            setValidacionesRefMedica();
                                        }
                                        else if (selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA)
                                        {
                                            TV_Inter_Externa_Visible = Visibility.Visible;
                                            TV_Inter_Interna_Visible = Visibility.Collapsed;
                                            selectedTV_Inter_HospitalValue = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL.Value;
                                            RaisePropertyChanged("SelectedTV_Inter_HospitalValue");
                                            if (selectedTV_Inter_HospitalValue == otro_hospital)
                                            {
                                                IsOtroHospitalSelected = Visibility.Visible;
                                                tv_Inter_Otro_Hospital = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO;
                                                RaisePropertyChanged("TV_Inter_Otro_Hospital");
                                            }
                                            selectedTV_Inter_Cita_TipoValue = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().ID_TIPO_CITA.Value;
                                            RaisePropertyChanged("SelectedTV_Inter_Cita_TipoValue");
                                            tv_Inter_ExpHGT = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().EXP_HGT;
                                            RaisePropertyChanged("TV_Inter_ExpHGT");
                                            tv_Inter_HR_Motivo = selectedPendienteTV.TV_HOJA_REFERENCIA_MEDICA_REG.MOTIVO;
                                            RaisePropertyChanged("TV_Inter_HR_Motivo");
                                            tv_Inter_HR_Observacion = selectedPendienteTV.TV_HOJA_REFERENCIA_MEDICA_REG.OBSERV;
                                            RaisePropertyChanged("TV_Inter_HR_Observacion");
                                            var _fecha_server=Fechas.GetFechaDateServer;
                                            if (selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().FECHA_CITA.HasValue && selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().FECHA_CITA.Value<_fecha_server)
                                            {
                                                fechaMinima = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().FECHA_CITA.Value;
                                                RaisePropertyChanged("FechaMinima");
                                            }
                                            else
                                            {
                                                fechaMinima = _fecha_server;
                                                RaisePropertyChanged("FechaMinima");
                                            }
                                            tv_Inter_FechaCita = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().FECHA_CITA;
                                            RaisePropertyChanged("TV_Inter_FechaCita");
                                            if (selectedTV_Inter_Cita_TipoValue == (short)enumCita_Tipo.PRIMERA)
                                            {
                                                isfechacitarequerida = false;
                                                if (!tv_Inter_FechaCita.HasValue || (tv_Inter_FechaCita.HasValue && tv_Inter_FechaCita.Value >= _fecha_server))
                                                {
                                                    isFechaCitaValid = true;
                                                    RaisePropertyChanged("IsFechaCitaValid");
                                                }
                                                else
                                                {
                                                    isFechaCitaValid = false;
                                                    RaisePropertyChanged("IsFechaCitaValid");
                                                }
                                            }
                                            else
                                            {
                                                isfechacitarequerida = true;
                                                if (!tv_Inter_FechaCita.HasValue || (tv_Inter_FechaCita.HasValue && tv_Inter_FechaCita.Value < _fecha_server))
                                                {
                                                    isFechaCitaValid = false;
                                                    RaisePropertyChanged("IsFechaCitaValid");
                                                }
                                                else
                                                {
                                                    isFechaCitaValid = true;
                                                    RaisePropertyChanged("IsFechaCitaValid");
                                                }
                                            }
                                            setValidacionesInterconsultaExterna();
                                        }
                                    });
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RECREAR_TV_INTERCONSULTA);
                                    break;
                            }
                            
                            break;
                        case "cancelar_agregar_cita_medica":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.REAGENDAR_TV_CITA_MEDICA);
                            break;
                        case "agregar_cita_medica":
                            if (HasErrors)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                                return;
                            }
                            Guardar();
                            break;
                        case "cancelar_reagenda":
                            if (selectedPendienteTV == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione una atencion pendiente");
                                return;
                            }
                            switch (selectedPendienteTV.TIPO)
                            {
                                case "CITA_MEDICA":
                                    modo_guardar_seleccionado = MODO_REAGENDA.CANCELAR_CITA_MEDICA;
                                    break;
                                case "CANALIZACION":
                                    modo_guardar_seleccionado = MODO_REAGENDA.CANCELAR_CANALIZACION;
                                    break;
                                case "INTERCONSULTA_SOLICITUD":
                                    modo_guardar_seleccionado = MODO_REAGENDA.CANCELAR_INTERCONSULTA;
                                    break;
                            }
                            Guardar();
                            break;
                        case "cancelar_atencion_canalizacion":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RECREAR_TV_INTERCONSULTA);
                            break;
                        case "guardad_atencion_canalizacion":
                            if (HasErrors)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                                return;
                            }
                            modo_guardar_seleccionado=MODO_REAGENDA.INTERCONSULTA;
                            Guardar();
                            break;
                    }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el evento click", ex);
            }
        }

        private async void Guardar()
        {
            try
            {
                var _fecha_server = Fechas.GetFechaDateServer;
                switch (modo_guardar_seleccionado)
                {
                    case MODO_REAGENDA.CITA_MEDICA:
                        if (agregarCitaMedicaAgendaHoraI.Value.Minute % 15 != 0 || agregarCitaMedicaAgendaHoraF.Value.Minute % 15 != 0)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Los intervalos de atención tienen que ser en bloques de 15 minutos");
                            return;
                        }
                        var _fecha_ini = new DateTime(agregarCitaMedicaAgendaFecha.Value.Year, agregarCitaMedicaAgendaFecha.Value.Month, agregarCitaMedicaAgendaFecha.Value.Day,
                                    agregarCitaMedicaAgendaHoraI.Value.Hour, agregarCitaMedicaAgendaHoraI.Value.Minute, agregarCitaMedicaAgendaHoraI.Value.Second);
                        var _fecha_fin = new DateTime(agregarCitaMedicaAgendaFecha.Value.Year, agregarCitaMedicaAgendaFecha.Value.Month, agregarCitaMedicaAgendaFecha.Value.Day,
                                    agregarCitaMedicaAgendaHoraF.Value.Hour, agregarCitaMedicaAgendaHoraF.Value.Minute, agregarCitaMedicaAgendaHoraF.Value.Second);
                        if (_fecha_ini>_fecha_fin)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La fecha final de la cita debe de ser mayor a la fecha inicial");
                            return;
                        }
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la cita médica", () =>{

                            var _usuario = new cUsuario().ObtenerUsuario(lstCitaMedicaAgendaEmpleados.First(w => w.ID_EMPLEADO == selectedCitaAgendaEmpleadoValue).Usuario.ID_USUARIO);
                            short _id_tipo_servicio = selectedPendienteTV.TV_CITA_MEDICA.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL;
                            if (selectedPendienteTV.TV_CITA_MEDICA.TV_PROC_ATENCION_MEDICA_PROG != null && selectedPendienteTV.TV_CITA_MEDICA.TV_PROC_ATENCION_MEDICA_PROG.Count() > 0)
                                _id_tipo_servicio = (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS;
                            var _cita_medica = new ATENCION_CITA
                            {
                                CITA_FECHA_HORA = _fecha_ini,
                                CITA_HORA_TERMINA = _fecha_fin,
                                ESTATUS = "N",
                                ID_ANIO = selectedPendienteTV.TV_CITA_MEDICA.ID_ANIO,
                                ID_AREA = selectedCitaMedicaArea,
                                ID_CENTRO = selectedPendienteTV.TV_CITA_MEDICA.ID_CENTRO,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_IMPUTADO = selectedPendienteTV.TV_CITA_MEDICA.ID_IMPUTADO,
                                ID_INGRESO = selectedPendienteTV.TV_CITA_MEDICA.ID_INGRESO,
                                ID_TIPO_ATENCION = selectedPendienteTV.TV_CITA_MEDICA.ID_TIPO_ATENCION,
                                ID_TIPO_SERVICIO = selectedPendienteTV.TV_CITA_MEDICA.ID_TIPO_SERVICIO,
                                ID_USUARIO = _usuario.ID_USUARIO,
                                ID_RESPONSABLE = _usuario.ID_PERSONA
                            };
                            var _atencion_medica = new ATENCION_MEDICA();
                            if (selectedPendienteTV.TV_CITA_MEDICA.TV_PROC_ATENCION_MEDICA_PROG != null && selectedPendienteTV.TV_CITA_MEDICA.TV_PROC_ATENCION_MEDICA_PROG.Count() > 0)
                            {
                                var _detalle = new List<PROC_MEDICO_PROG_DET>();
                                var _proc_atencion_medica_prog = new List<PROC_ATENCION_MEDICA_PROG>();
                                var _proc_atencion_medica = new List<PROC_ATENCION_MEDICA>();
                                foreach (var item in selectedPendienteTV.TV_CITA_MEDICA.TV_PROC_ATENCION_MEDICA_PROG)
                                {
                                    _detalle = new List<PROC_MEDICO_PROG_DET>();
                                    _proc_atencion_medica_prog = new List<PROC_ATENCION_MEDICA_PROG>();
                                    foreach (var prod in item.PROC_MED.PROC_MATERIAL)
                                        _detalle.Add(new PROC_MEDICO_PROG_DET
                                        {
                                            ID_CENTRO_UBI = GlobalVar.gCentro,
                                            ID_PROCMED = item.ID_PROCMED,
                                            REGISTRO_FEC = _fecha_server,
                                            ID_PRODUCTO = prod.ID_PRODUCTO
                                        });
                                    _proc_atencion_medica_prog.Add(new PROC_ATENCION_MEDICA_PROG
                                    {
                                        ESTATUS = "N",
                                        ID_CENTRO_UBI = GlobalVar.gCentro,
                                        ID_PROCMED = item.ID_PROCMED,
                                        ID_USUARIO_ASIGNADO = _usuario.ID_USUARIO,
                                        PROC_MEDICO_PROG_DET = _detalle
                                    });
                                    _proc_atencion_medica.Add(new PROC_ATENCION_MEDICA
                                    {
                                        ID_CENTRO_UBI = GlobalVar.gCentro,
                                        ID_PROCMED = item.ID_PROCMED,
                                        ID_USUARIO = _usuario.ID_USUARIO,
                                        OBSERV = item.OBSERV,
                                        PROC_ATENCION_MEDICA_PROG = _proc_atencion_medica_prog,
                                        REGISTRO_FEC = _fecha_server
                                    });
                                }

                                _atencion_medica = new ATENCION_MEDICA
                                {
                                    ID_ANIO = selectedPendienteTV.TV_CITA_MEDICA.ID_ANIO,
                                    ID_CENTRO = selectedPendienteTV.TV_CITA_MEDICA.ID_CENTRO,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    ID_IMPUTADO = selectedPendienteTV.TV_CITA_MEDICA.ID_IMPUTADO,
                                    ID_INGRESO = selectedPendienteTV.TV_CITA_MEDICA.ID_INGRESO,
                                    ID_TIPO_ATENCION = selectedPendienteTV.TV_CITA_MEDICA.ID_TIPO_ATENCION,
                                    ID_TIPO_SERVICIO = (short)enumAtencionServicio.TRASLADO_VIRTUAL,
                                    PROC_ATENCION_MEDICA = _proc_atencion_medica
                                };
                            }
                            else
                                _atencion_medica = null;
                            new cTraslado_Virtual_Medico().ReagendarTVCitaMedica(selectedPendienteTV.TV_CITA_MEDICA.ID_TV_CITA, _cita_medica, GlobalVar.gCentro, _atencion_medica);
                            selectedIngreso.TV_CITA_MEDICA.First(w => w.ID_TV_CITA == selectedPendienteTV.TV_CITA_MEDICA.ID_TV_CITA).ID_TV_MEDICO_ESTATUS = "RP";
                            return true;

                        }))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {

                                CargarPendientesReagenda(true);
                            });
                            SelectedPendienteTV = null;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.REAGENDAR_TV_CITA_MEDICA);
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La cita medica fue reagendada con exito");
                            
                        }
                        break;
                    case MODO_REAGENDA.CANCELAR_CITA_MEDICA:
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cancelando la cita médica", () =>{
                            new cTraslado_Virtual_Medico().CancelarTV_Cita_Medica(selectedPendienteTV.TV_CITA_MEDICA.ID_TV_CITA, GlobalVar.gCentro);
                            selectedIngreso.TV_CITA_MEDICA.First(w => w.ID_TV_CITA == selectedPendienteTV.TV_CITA_MEDICA.ID_TV_CITA).ID_TV_MEDICO_ESTATUS = "CA";
                            return true;
                        }))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {

                                CargarPendientesReagenda(true);
                            });
                            SelectedPendienteTV = null;
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La cita medica fue cancelada con exito");
                        }
                        break;
                    case MODO_REAGENDA.CANALIZACION:
                         if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la canalización", () =>{
                             var _canalizacion_especialidades = new List<CANALIZACION_ESPECIALIDAD>();
                             if (selectedPendienteTV.TV_CANALIZACION.TV_CANALIZACION_ESPECIALIDAD != null && selectedPendienteTV.TV_CANALIZACION.TV_CANALIZACION_ESPECIALIDAD.Count() > 0)
                             {
                                 foreach (var item in selectedPendienteTV.TV_CANALIZACION.TV_CANALIZACION_ESPECIALIDAD)
                                     _canalizacion_especialidades.Add(new CANALIZACION_ESPECIALIDAD
                                     {
                                         ID_CENTRO_UBI = GlobalVar.gCentro,
                                         ID_ESPECIALIDAD = item.ID_ESPECIALIDAD,
                                         ID_ESTATUS = "P",
                                         ID_FECHA = _fecha_server,
                                         ID_INIVEL = item.ID_INIVEL,
                                         ID_INTER = item.ID_INTER,
                                         ID_INTERAT = item.ID_INTERAT
                                     });
                             }
                             else
                                 _canalizacion_especialidades = null;
                             var _canalizacion_serv_aux = new List<CANALIZACION_SERV_AUX>();
                             if (selectedPendienteTV.TV_CANALIZACION.TV_CANALIZACION_SERV_AUX != null && selectedPendienteTV.TV_CANALIZACION.TV_CANALIZACION_SERV_AUX.Count() > 0)
                                 foreach (var item in selectedPendienteTV.TV_CANALIZACION.TV_CANALIZACION_SERV_AUX)
                                     _canalizacion_serv_aux.Add(new CANALIZACION_SERV_AUX
                                     {
                                         ID_CENTRO_UBI = GlobalVar.gCentro,
                                         ID_ESTATUS = "P",
                                         ID_FECHA = _fecha_server,
                                         ID_INIVEL = item.ID_INIVEL,
                                         ID_INTER = item.ID_INTER,
                                         ID_SERV_AUX = item.ID_SERV_AUX
                                     });
                             var _canalizacion = new CANALIZACION
                             {
                                 ID_CENTRO_UBI = GlobalVar.gCentro,
                                 ID_ESTATUS_CAN = selectedPendienteTV.TV_CANALIZACION.ID_ESTATUS_CAN,
                                 ID_FECHA = _fecha_server,
                                 ID_USUARIO = GlobalVar.gUsr,
                                 CANALIZACION_SERV_AUX = _canalizacion_serv_aux,
                                 CANALIZACION_ESPECIALIDAD = _canalizacion_especialidades
                             };
                             var _usuario = new cUsuario().ObtenerUsuario(GlobalVar.gUsr);
                             var _nota_medica = new NOTA_MEDICA
                             {
                                 ID_CENTRO_UBI = GlobalVar.gCentro,
                                 ID_RESPONSABLE = _usuario.ID_PERSONA,
                                 CANALIZACION = _canalizacion
                             };
                             var _atencion_medica = new ATENCION_MEDICA
                             {
                                 ID_ANIO = selectedPendienteTV.TV_CANALIZACION.ID_ANIO,
                                 ID_CENTRO = selectedPendienteTV.TV_CANALIZACION.ID_CENTRO,
                                 ID_CENTRO_UBI = GlobalVar.gCentro,
                                 ID_IMPUTADO = selectedPendienteTV.TV_CANALIZACION.ID_IMPUTADO,
                                 ID_INGRESO = selectedPendienteTV.TV_CANALIZACION.ID_INGRESO,
                                 ID_TIPO_ATENCION = (short)enumAtencionTipo.CONSULTA_MEDICA,
                                 ID_TIPO_SERVICIO = (short)enumAtencionServicio.TRASLADO_VIRTUAL,
                                 NOTA_MEDICA = _nota_medica
                             };
                             new cTraslado_Virtual_Medico().RecrearTVCanalizaciones(_atencion_medica, GlobalVar.gCentro,selectedPendienteTV.TV_CANALIZACION.ID_TV_CANALIZACION);
                             selectedIngreso.TV_CANALIZACION.First(w => w.ID_TV_CANALIZACION == selectedPendienteTV.TV_CANALIZACION.ID_TV_CANALIZACION).ID_TV_MEDICO_ESTATUS = "RP";
                             return true;
                         }))
                         {
                             await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                             {
                                 CargarPendientesReagenda(true);
                             });
                             SelectedPendienteTV = null;
                             new Dialogos().ConfirmacionDialogo("EXITO!", "La solicitud de canalización fue creada con exito");
                         }
                        break;
                    case MODO_REAGENDA.CANCELAR_CANALIZACION:
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cancelando la solicitud de canalización", () =>
                        {
                            new cTraslado_Virtual_Medico().CancelarTV_CANALIZACION(selectedPendienteTV.TV_CANALIZACION.ID_TV_CANALIZACION, GlobalVar.gCentro);
                            selectedIngreso.TV_CANALIZACION.First(w => w.ID_TV_CANALIZACION == selectedPendienteTV.TV_CANALIZACION.ID_TV_CANALIZACION).ID_TV_MEDICO_ESTATUS = "CA";
                            return true;
                        }))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {

                                CargarPendientesReagenda(true);
                            });
                            SelectedPendienteTV = null;
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La canalizacion fue cancelada con exito");
                        }
                        break;
                    case MODO_REAGENDA.INTERCONSULTA:
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la canalización", () =>
                        {
                            var _servicio_aux_interconsulta = new List<SERVICIO_AUX_INTERCONSULTA>();
                            if (selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_SERVICIO_AUX_INTERCONSULTA != null && selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_SERVICIO_AUX_INTERCONSULTA.Count() > 0)
                            {
                                foreach (var item in selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_SERVICIO_AUX_INTERCONSULTA)
                                    _servicio_aux_interconsulta.Add(new SERVICIO_AUX_INTERCONSULTA
                                    {
                                        ID_CENTRO_UBI = GlobalVar.gCentro,
                                        ID_SERV_AUX = item.ID_SERV_AUX,
                                        REGISTRO_FEC = _fecha_server
                                    });
                            }
                            else
                                _servicio_aux_interconsulta = null;
                            var _hoja_referencia_medica = new List<HOJA_REFERENCIA_MEDICA>();
                            if (selectedTV_Inter_TipoValue == (short)enumInterconsulta_Tipo.EXTERNA)
                            {
                                _hoja_referencia_medica.Add(new HOJA_REFERENCIA_MEDICA
                                {
                                    EXP_HGT = tv_Inter_ExpHGT,
                                    FECHA_CITA = tv_Inter_FechaCita,
                                    HOSPITAL_OTRO = tv_Inter_Otro_Hospital,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    ID_HOSPITAL = selectedTV_Inter_HospitalValue,
                                    ID_TIPO_CITA = selectedTV_Inter_Cita_TipoValue,
                                    MOTIVO = tv_Inter_HR_Motivo,
                                    OBSERV = tv_Inter_HR_Observacion
                                });
                            }
                            else
                                _hoja_referencia_medica = null;
                            var _sol_interconsulta_interna = new List<SOL_INTERCONSULTA_INTERNA>();
                            if (selectedTV_Inter_TipoValue==(short)enumInterconsulta_Tipo.INTERNA)
                            {
                                _sol_interconsulta_interna.Add(new SOL_INTERCONSULTA_INTERNA {
                                    ID_CENTRO=selectedTV_Inter_CentroValue,
                                    ID_CENTRO_UBI=GlobalVar.gCentro,
                                    ID_USUARIO=GlobalVar.gUsr,
                                    MOTIVO_INTERCONSULTA=tv_Inter_Motivo,
                                    REGISTRO_FEC=_fecha_server
                                });
                            }
                            var _interconsulta_solicitud = new List<INTERCONSULTA_SOLICITUD>();
                            _interconsulta_solicitud.Add(new INTERCONSULTA_SOLICITUD {
                                ESTATUS=_hoja_referencia_medica!=null && _hoja_referencia_medica.First().FECHA_CITA.HasValue?"S":_hoja_referencia_medica==null?"S":"E",
                                HOJA_REFERENCIA_MEDICA=_hoja_referencia_medica,
                                ID_CENTRO_UBI=GlobalVar.gCentro,
                                ID_ESPECIALIDAD=selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_ESPECIALIDAD,
                                ID_INIVEL=selectedTV_Inter_PrioridadValue,
                                ID_INTER=selectedTV_Inter_TipoValue,
                                ID_INTERAT=selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_INTERAT,
                                ID_USUARIO=GlobalVar.gUsr,
                                REGISTRO_FEC=_fecha_server,
                                SERVICIO_AUX_INTERCONSULTA=_servicio_aux_interconsulta,
                                SOL_INTERCONSULTA_INTERNA=_sol_interconsulta_interna
                            });
                            var _canalizacion_especialidades = new List<CANALIZACION_ESPECIALIDAD>();
                            if (selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_ESPECIALIDAD!=null)
                            {
                                _canalizacion_especialidades.Add(new CANALIZACION_ESPECIALIDAD {
                                    ID_CENTRO_UBI=GlobalVar.gCentro,
                                    ID_ESPECIALIDAD=selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_ESPECIALIDAD.Value,
                                    ID_ESTATUS="A",
                                    ID_FECHA=_fecha_server,
                                    ID_INIVEL=selectedTV_Inter_PrioridadValue,
                                    ID_INTER=selectedTV_Inter_TipoValue,
                                    ID_INTERAT=selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_INTERAT
                                });
                                
                            }
                            else
                                _canalizacion_especialidades = null;
                            var _canalizacion_serv_aux = new List<CANALIZACION_SERV_AUX>();
                            if (selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_SERVICIO_AUX_INTERCONSULTA != null && selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_SERVICIO_AUX_INTERCONSULTA.Count() > 0)
                                foreach (var item in selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_SERVICIO_AUX_INTERCONSULTA)
                                    _canalizacion_serv_aux.Add(new CANALIZACION_SERV_AUX
                                    {
                                        ID_CENTRO_UBI = GlobalVar.gCentro,
                                        ID_ESTATUS = "A",
                                        ID_FECHA = _fecha_server,
                                        ID_INIVEL = selectedTV_Inter_PrioridadValue,
                                        ID_INTER = selectedTV_Inter_TipoValue,
                                        ID_SERV_AUX = item.ID_SERV_AUX
                                    });
                            var _canalizacion = new CANALIZACION
                            {
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_ESTATUS_CAN = "A",
                                ID_FECHA = _fecha_server,
                                ID_USUARIO = GlobalVar.gUsr,
                                CANALIZACION_SERV_AUX = _canalizacion_serv_aux,
                                CANALIZACION_ESPECIALIDAD = _canalizacion_especialidades,
                                INTERCONSULTA_SOLICITUD=_interconsulta_solicitud
                            };
                            var _usuario = new cUsuario().ObtenerUsuario(GlobalVar.gUsr);
                            var _nota_medica = new NOTA_MEDICA
                            {
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_RESPONSABLE = _usuario.ID_PERSONA,
                                CANALIZACION = _canalizacion
                            };
                            var _atencion_medica = new ATENCION_MEDICA
                            {
                                ID_ANIO = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_ANIO,
                                ID_CENTRO = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_CENTRO,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_IMPUTADO = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_IMPUTADO,
                                ID_INGRESO = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_INGRESO,
                                ID_TIPO_ATENCION = (short)enumAtencionTipo.CONSULTA_MEDICA,
                                ID_TIPO_SERVICIO = (short)enumAtencionServicio.TRASLADO_VIRTUAL,
                                NOTA_MEDICA = _nota_medica
                            };
                            new cTraslado_Virtual_Medico().RecrearTVInterconsulta(_atencion_medica, GlobalVar.gCentro, selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_TV_INTERSOL);
                            selectedIngreso.TV_INTERCONSULTA_SOLICITUD.First(w => w.ID_TV_INTERSOL == selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_TV_INTERSOL).ID_TV_MEDICO_ESTATUS = "RP";
                            return true;
                        }))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                CargarPendientesReagenda(true);
                            });
                            SelectedPendienteTV = null;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RECREAR_TV_INTERCONSULTA);
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La solicitud de interconsulta fue creada con exito");
                        }
                        break;
                    case MODO_REAGENDA.CANCELAR_INTERCONSULTA:
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cancelando la solicitud de interconsulta", () =>
                        {
                            new cTraslado_Virtual_Medico().CancelarTV_Interconsulta(selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_TV_INTERSOL, GlobalVar.gCentro);
                            selectedIngreso.TV_INTERCONSULTA_SOLICITUD.First(w => w.ID_TV_INTERSOL == selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_TV_INTERSOL).ID_TV_MEDICO_ESTATUS = "CA";
                            return true;
                        }))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {

                                CargarPendientesReagenda(true);
                            });
                            SelectedPendienteTV = null;
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La solicitud de interconsulta fue cancelada con exito");
                        }
                        break;
                }

            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
            }
        }

        private async void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_fecha_inicio_busqueda_TV":
                    if (!FechaFinalBuscarTV.HasValue || !FechaInicialBuscarTV.HasValue || FechaFinalBuscarTV >= FechaInicialBuscarTV)
                        IsFechaIniBusquedaTVValida = true;
                    else
                        IsFechaIniBusquedaTVValida = false;
                    break;
                case "cambio_cita_medica_fecha_agregar_agenda":
                    if (AgregarCitaMedicaAgendaFecha.HasValue)
                        AgregarCitaMedicaAgendaFechaValid = true;
                    else
                        AgregarCitaMedicaAgendaFechaValid = false;
                    break;
                case "cambio_hospital":
                    if (SelectedTV_Inter_HospitalValue == otro_hospital)
                    {
                        IsOtroHospitalSelected = Visibility.Visible;
                        setValidacionHospitalOtros();
                    }
                    else
                    {
                        IsOtroHospitalSelected = Visibility.Collapsed;
                        clearValidacionHospitalOtros();
                    }
                    break;
                case "cambio_fecha_cita":
                    if (!isfechacitarequerida || TV_Inter_FechaCita.HasValue && TV_Inter_FechaCita.Value >= Fechas.GetFechaDateServer)
                        IsFechaCitaValid = true;
                    else
                        IsFechaCitaValid = false;
                    break;
                case "cambio_tv_interconsulta_tipo":
                    if (SelectedTV_Inter_TipoValue==(short)enumInterconsulta_Tipo.INTERNA)
                    {
                        tv_Inter_Externa_Visible = Visibility.Collapsed;
                        RaisePropertyChanged("TV_Inter_Externa_Visible");
                        tv_Inter_Interna_Visible = Visibility.Visible;
                        RaisePropertyChanged("TV_Inter_Interna_Visible");
                        if (selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_INTER == (short)enumInterconsulta_Tipo.INTERNA)
                        {
                            selectedTV_Inter_CentroValue = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_SOL_INTERCONSULTA_INTERNA.First().ID_CENTRO.Value;
                            RaisePropertyChanged("SelectedTV_Inter_CentroValue");
                            tv_Inter_Motivo = selectedPendienteTV.TV_SOL_INTERCONSULTA_INTERNA_REG.MOTIVO_INTERCONSULTA;
                            RaisePropertyChanged("TV_Inter_Motivo");
                        }
                        else
                        {
                            selectedTV_Inter_CentroValue = -1;
                            RaisePropertyChanged("SelectedTV_Inter_CentroValue");
                        }
                        setValidacionesRefMedica();
                    }
                    else if (SelectedTV_Inter_TipoValue == (short)enumInterconsulta_Tipo.EXTERNA)
                    {
                        tv_Inter_Externa_Visible = Visibility.Visible;
                        RaisePropertyChanged("TV_Inter_Externa_Visible");
                        tv_Inter_Interna_Visible = Visibility.Collapsed;
                        RaisePropertyChanged("TV_Inter_Interna_Visible");
                        if (selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA)
                        {
                            selectedTV_Inter_HospitalValue = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL.Value;
                            RaisePropertyChanged("SelectedTV_Inter_HospitalValue");
                            if (selectedTV_Inter_HospitalValue == otro_hospital)
                            {
                                IsOtroHospitalSelected = Visibility.Visible;
                                tv_Inter_Otro_Hospital = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO;
                                RaisePropertyChanged("TV_Inter_Otro_Hospital");
                            }
                            selectedTV_Inter_Cita_TipoValue = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().ID_TIPO_CITA.Value;
                            RaisePropertyChanged("SelectedTV_Inter_Cita_TipoValue");
                            tv_Inter_ExpHGT = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().EXP_HGT;
                            RaisePropertyChanged("TV_Inter_ExpHGT");
                            tv_Inter_HR_Motivo = selectedPendienteTV.TV_HOJA_REFERENCIA_MEDICA_REG.MOTIVO;
                            RaisePropertyChanged("TV_Inter_HR_Motivo");
                            tv_Inter_HR_Observacion = selectedPendienteTV.TV_HOJA_REFERENCIA_MEDICA_REG.OBSERV;
                            RaisePropertyChanged("TV_Inter_HR_Observacion");
                            tv_Inter_FechaCita = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().FECHA_CITA;
                            var _fecha_server = Fechas.GetFechaDateServer;
                            if (selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().FECHA_CITA.HasValue && selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().FECHA_CITA.Value < _fecha_server)
                            {
                                fechaMinima = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().FECHA_CITA.Value;
                                RaisePropertyChanged("FechaMinima");
                            }
                            else
                            {
                                fechaMinima = _fecha_server;
                                RaisePropertyChanged("FechaMinima");
                            }
                            tv_Inter_FechaCita = selectedPendienteTV.TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First().FECHA_CITA;
                            RaisePropertyChanged("TV_Inter_FechaCita");
                            if (selectedTV_Inter_Cita_TipoValue == (short)enumCita_Tipo.PRIMERA)
                            {
                                isfechacitarequerida = false;
                                if (!tv_Inter_FechaCita.HasValue || (tv_Inter_FechaCita.HasValue && tv_Inter_FechaCita.Value >= _fecha_server))
                                {
                                    isFechaCitaValid = true;
                                    RaisePropertyChanged("IsFechaCitaValid");
                                }
                                else
                                {
                                    isFechaCitaValid = false;
                                    RaisePropertyChanged("IsFechaCitaValid");
                                }
                            }
                            else
                            {
                                isfechacitarequerida = true;
                                if (!tv_Inter_FechaCita.HasValue || (tv_Inter_FechaCita.HasValue && tv_Inter_FechaCita.Value < _fecha_server))
                                {
                                    isFechaCitaValid = false;
                                    RaisePropertyChanged("IsFechaCitaValid");
                                }
                                else
                                {
                                    isFechaCitaValid = true;
                                    RaisePropertyChanged("IsFechaCitaValid");
                                }
                            }
                        }
                        else
                        {
                            selectedTV_Inter_HospitalValue = -1;
                            RaisePropertyChanged("SelectedTV_Inter_HospitalValue");

                            selectedTV_Inter_Cita_TipoValue = -1;
                            RaisePropertyChanged("SelectedTV_Inter_Cita_TipoValue");
                            tv_Inter_ExpHGT = string.Empty;
                            RaisePropertyChanged("TV_Inter_ExpHGT");
                            tv_Inter_FechaCita = null;
                            RaisePropertyChanged("TV_Inter_FechaCita");
                            if (TV_Inter_FechaCita.HasValue && TV_Inter_FechaCita.Value >= Fechas.GetFechaDateServer)
                            {
                                IsFechaCitaValid = true;
                            }
                        }
                        setValidacionesInterconsultaExterna();
                    }

                    break;
                case "cambio_tipo_citas":
                    if (selectedTV_Inter_Cita_TipoValue == (short)enumCita_Tipo.PRIMERA)
                    {
                        isfechacitarequerida = false;
                        if (!TV_Inter_FechaCita.HasValue || (TV_Inter_FechaCita.HasValue && TV_Inter_FechaCita.Value >= Fechas.GetFechaDateServer))
                            IsFechaCitaValid = true;
                        else
                            IsFechaCitaValid = false;
                    }
                    else
                    {
                        isfechacitarequerida = true;
                        if (!TV_Inter_FechaCita.HasValue || (TV_Inter_FechaCita.HasValue && TV_Inter_FechaCita.Value < Fechas.GetFechaDateServer))
                            IsFechaCitaValid = false;
                        else
                            IsFechaCitaValid = true;
                    }
                    break;
            }
        }

        private void LimpiarBuscarTV()
        {
            AnioBuscarTV = null;
            FolioBuscarTV = null;
            NombreBuscarTV = string.Empty;
            ApellidoPaternoBuscarTV = string.Empty;
            ApellidoMaternoBuscarTV = string.Empty;
            FechaFinalBuscarTV = null;
            FechaInicialBuscarTV = null;
            SelectedCentro_OrigenBuscarValue = -1;
        }

        private void LimpiarReagendarCita()
        {
            AgregarCitaMedicaAgendaFecha = null;
            AgregarCitaMedicaAgendaHoraF = null;
            AgregarCitaMedicaAgendaHoraI = null;
            AgregarCitaMedicaAgendaFechaValid = false;
        }

        public void CargarPendientesReagenda(bool isExceptionManaged=false)
        { 
            try
            {
                lstPendientesTV = new List<cPendientesTVAreaMedica>(selectedIngreso.TV_CITA_MEDICA.Where(w => w.ID_TV_MEDICO_ESTATUS == "PE").Select(s => new cPendientesTVAreaMedica
                {
                    ESTATUS = "PE",
                    TIPO = "CITA_MEDICA",
                    TV_CANALIZACION = null,
                    TV_CITA_MEDICA = s,
                    TV_INTERCONSULTA_SOLICITUD = null
                }).Union(selectedIngreso.TV_CANALIZACION.Where(w => w.ID_TV_MEDICO_ESTATUS == "PE").Select(s => new cPendientesTVAreaMedica
                {
                    ESTATUS = "PE",
                    TIPO = "CANALIZACION",
                    TV_CANALIZACION = s,
                    TV_CITA_MEDICA = null,
                    TV_INTERCONSULTA_SOLICITUD = null
                }))
                .Union(selectedIngreso.TV_INTERCONSULTA_SOLICITUD.Where(w => w.ID_TV_MEDICO_ESTATUS == "PE").Select(s => new cPendientesTVAreaMedica {
                    ESTATUS="PE",
                    TIPO="INTERCONSULTA_SOLICITUD",
                    TV_CANALIZACION=null,
                    TV_CITA_MEDICA=null,
                    TV_INTERCONSULTA_SOLICITUD=s
                })));
                RaisePropertyChanged("LstPendientesTV");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las atenciones pendientes", ex);
            }
        }
        #endregion

        #region Busqueda TV
        public void BusquedaTrasladoVirtual(short id_ub_centro, int? id_anio=null, int? id_imputado=null, string nombre="", string paterno="", string materno="" , int? centro_origen=null, DateTime? fecha_ini=null, DateTime? fecha_fin=null, bool isExceptionManaged=false)
        {
            try
            {
                listaTVBusqueda =new ObservableCollection<TRASLADO_VIRTUAL_INGRESO>(new cTraslado_Virtual_Medico().ObtenerTrasladoVirtualMedico(id_ub_centro, estatus_administrativos_inactivos, id_anio, id_imputado, nombre,paterno,materno, centro_origen, fecha_ini, fecha_fin));
                RaisePropertyChanged("ListaTVBusqueda");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los traslados con atenciones pendientes", ex);
            }
        }

        public void BuscarIngreso(short centro, short anio, int imputado, short ingreso,bool isExceptionManaged=false)
        {
            try
            {
                selectedIngreso = new cIngreso().Obtener(centro, anio, imputado, ingreso);
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener el ingreso", ex);
            }

        }
        #endregion


        #region Cargar Catalogos
        public void CargarCentrosBusqueda(bool isExceptionManaged=false)
        {
            try
            {
                lstCentro_OrigenBuscar = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos("", 0, 0, "S"));
                lstCentro_OrigenBuscar.Insert(0, new CENTRO
                {
                    ID_CENTRO = -1,
                    DESCR = "SELECCIONE UNO"
                });
                RaisePropertyChanged("LstCentro_OrigenBuscar");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los centros", ex);
            }
        }

        public void CargarEmpleados(bool isExceptionManaged = false)
        {
            try
            {
                var roles=new List<short>();
                if (selectedPendienteTV.TV_CITA_MEDICA.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA)
                    roles.Add((short)enumRolesAreasTecnicas.MEDICO);
                else
                    roles.Add((short)enumRolesAreasTecnicas.DENTISTA);
                if (selectedPendienteTV.TV_CITA_MEDICA.TV_PROC_ATENCION_MEDICA_PROG != null && selectedPendienteTV.TV_CITA_MEDICA.TV_PROC_ATENCION_MEDICA_PROG.Count() > 0)
                    roles.Add((short)enumRolesAreasTecnicas.ENFERMERO);
                lstCitaMedicaAgendaEmpleados = new ObservableCollection<cUsuarioExtendida>(new cUsuario().ObtenerTodosporAreaTecnica(GlobalVar.gCentro, (short)eAreas.AREA_MEDICA, roles).Select(s => new cUsuarioExtendida {
                    Usuario=s,
                    NOMBRE_COMPLETO = s.EMPLEADO.PERSONA.NOMBRE.Trim() + (s.EMPLEADO.PERSONA.PATERNO == null || s.EMPLEADO.PERSONA.PATERNO.Trim() == "" ? "" : " " + s.EMPLEADO.PERSONA.PATERNO.Trim()) +
                           (s.EMPLEADO.PERSONA.MATERNO == null || s.EMPLEADO.PERSONA.MATERNO.Trim() == "" ? "" : " " + s.EMPLEADO.PERSONA.MATERNO.Trim()),
                    ID_EMPLEADO = s.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO
                }));
                if (new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica((short)eAreas.AREA_MEDICA,GlobalVar.gUsr))
                {
                    var _usuario = new cUsuario().Obtener(StaticSourcesViewModel.UsuarioLogin.Username);
                    if (!lstCitaMedicaAgendaEmpleados.Any(a => a.Usuario.ID_USUARIO == _usuario.ID_USUARIO))
                        lstCitaMedicaAgendaEmpleados.Insert(0,new cUsuarioExtendida {
                            Usuario = _usuario,
                            NOMBRE_COMPLETO = _usuario.EMPLEADO.PERSONA.NOMBRE.Trim() + (string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.PATERNO) ? string.Empty : " " + _usuario.EMPLEADO.PERSONA.PATERNO.Trim()) +
                            (string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.MATERNO) ? string.Empty : " " + _usuario.EMPLEADO.PERSONA.MATERNO.Trim()),
                            ID_EMPLEADO = _usuario.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO
                        });
                }
                RaisePropertyChanged("LstCitaMedicaAgendaEmpleados");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de empleados", ex);
                else
                    throw ex;
            }
        }

        private void CargarAreas(bool isExceptionManaged=false)
        {
            try
            {
                lstCitaMedicaAreas = new ObservableCollection<AREA>(new cArea().ObtenerTodos());
                lstCitaMedicaAreas.Insert(0, new AREA
                {
                    ID_AREA = -1,
                    DESCR = "SELECCIONE"
                });
                OnPropertyChanged("LstCitaMedicaAreas");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de areas", ex);
                else
                    throw ex;
            }
        }

        private void CargarTV_InterCentros(bool isExceptionManaged=false)
        {
            try
            {
                lstTV_Inter_Centro = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos("", 0, 0, "S"));
                lstTV_Inter_Centro.Insert(0, new CENTRO
                {
                    ID_CENTRO = -1,
                    DESCR = "SELECCIONE UNO"
                });
                RaisePropertyChanged("LstTV_Inter_Centro");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de areas", ex);
                else
                    throw ex;
            }
        }

        private void CargarTV_Inter_Tipos(bool isExceptionManaged=false)
        {
            try
            {
                lst_TV_Inter_Tipo=new ObservableCollection<INTERCONSULTA_TIPO>(new cInterconsultaTipo().ObtenerTodos("S"));
                lst_TV_Inter_Tipo.Insert(0, new INTERCONSULTA_TIPO {
                    ID_INTER=-1,
                    DESCR="SELECCIONE UNO"
                });
                RaisePropertyChanged("Lst_TV_Inter_Tipo");
            }
            catch(Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...","Ocurrió un error al cargar el catalogo de tipos de interconsulta",ex);
                else
                    throw ex;
            }
        }

        private void CargarTV_Inter_Prioridad(bool isExceptionManaged=false)
        {
            try
            {
                lst_TV_Inter_Prioridad = new ObservableCollection<INTERCONSULTA_NIVEL_PRIORIDAD>(new cNivelPrioridad().ObtenerTodos("S"));
                lst_TV_Inter_Prioridad.Insert(0, new INTERCONSULTA_NIVEL_PRIORIDAD {
                    ID_INIVEL=-1,
                    DESCR="SELECCIONE UNO"
                });
                RaisePropertyChanged("Lst_TV_Inter_Prioridad");
            }
            catch(Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...","Ocurrió un error al cargar el catalogo de nivel de prioridades",ex);
                else
                    throw ex;
            }
        }

        private void CargarTV_Inter_Hospital(bool isExceptionManaged=false)
        {
            try
            {
                lstTV_Inter_Hospitales = new ObservableCollection<HOSPITAL>(new cHospitales().Seleccionar(true));
                lstTV_Inter_Hospitales.Insert(0, new HOSPITAL {
                    ID_HOSPITAL=-1,
                    DESCR="SELECCIONE UNO"
                });
                RaisePropertyChanged("LstTV_Inter_Hospitales");
            }
            catch(Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de hospitales", ex);
                else
                    throw ex;
            }
        }

        private void CargarTV_Inter_Cita_Tipo(bool isExceptionManaged=false)
        {
            try
            {
                lstTV_Inter_Cita_Tipo = new ObservableCollection<CITA_TIPO>(new cCita_Tipo().ObtenerTodos("S"));
                lstTV_Inter_Cita_Tipo.Insert(0, new CITA_TIPO {
                    ID_TIPO_CITA=-1,
                    DESCR="SELECCIONE UNO"
                });
                RaisePropertyChanged("LstTV_Inter_Cita_Tipo");
            }
            catch(Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de hospitales", ex);
                else
                    throw ex;
            }
        }
        #endregion

        private enum MODO_REAGENDA
        {
            CITA_MEDICA = 1,
            CANALIZACION = 2,
            INTERCONSULTA = 3,
            CANCELAR_CITA_MEDICA = 4,
            CANCELAR_CANALIZACION = 5,
            CANCELAR_INTERCONSULTA=6
        }
    }
}

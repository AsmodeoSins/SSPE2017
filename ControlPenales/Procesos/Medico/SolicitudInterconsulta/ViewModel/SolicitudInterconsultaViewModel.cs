using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    public partial class SolicitudInterconsultaViewModel:FingerPrintScanner
    {
        #region Constructor
        public SolicitudInterconsultaViewModel()
        {

        }

        public SolicitudInterconsultaViewModel(NOTA_MEDICA nota_medica)
        {
            selectedNotaMedica = nota_medica;
        }

        #endregion

        #region Generales

        private async void SolicitudInterconsultaOnLoading(SolicitudInterconsultaView window)
        {
            EliminarMenuEnabled = false;
            estatus_administrativos_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                if (selectedNotaMedica!=null)
                    CargarDatosNotaMedica(selectedNotaMedica,true);
                CargarExtEspecialidadTipoInterconsulta(true);
                CargarExtEspecialidadInterconsultaNivelPrioridad(true);
                CargarEspecialidades(true);
                
                CargarInterconsultaAtencionTipo(true);
                
                CargarAtencionTipo(true);
                
                RaisePropertyChanged("SelectedInterconsultaTipo");
                selectedInterconsultaTipoAtencion = -1;
                RaisePropertyChanged("SelectedInterconsultaTipoAtencion");
                
                RaisePropertyChanged("SelectedNvlPrioridad");
                selectedAtencion_TipoValue = -1;
                RaisePropertyChanged("SelectedAtencion_TipoValue");

                //SERVICIOS AUXILIARES
                CargarTipo_Servicios_Auxiliares(true);
                CargarSubTipo_Servicios_Auxiliares(-1, true);
                CargarServicioAuxiliar(-1, -1, true);
                selectedTipoServAux = -1;
                RaisePropertyChanged("SelectedTipoServAux");
                selectedSubtipoServAux = -1;
                RaisePropertyChanged("SelectedSubtipoServAux");
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
                            BuscarNotasMedicasInterconsulta(null,null, null, string.Empty, string.Empty, string.Empty, null, null, true);
                        });
                        PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.BUSCAR_NOTA_MEDICA);
                    break;
                    case "filtro_notas_medicas":
                        if (!IsFechaIniBusquedaValida)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La fecha de inicio tiene que ser menor a la fecha fin!");
                            return;
                        }
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        BuscarNotasMedicasInterconsulta(selectedAtencion_TipoValue==-1?null:(short?)selectedAtencion_TipoValue,anioBuscarNotaMedica,folioBuscarNotaMedica,nombreBuscarNotaMedica,apellidoPaternoBuscarNotaMedica,
                            apellidoMaternoBuscarNotaMed,fechaInicialNotaMed,fechaFinalBuscarNotaMed,false);
                    });
                    break;
                    case "cancelar_buscar_nota_medica":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_NOTA_MEDICA);
                        break;
                    case "seleccionar_nota_medica":
                        if (SelectedNotaMedicaBusqueda==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione una nota medica");
                            return;
                        }
                        modoVistaModelo = enumModo.INSERCION;
                        Limpiar();
                        selectedNotaMedica = SelectedNotaMedicaBusqueda;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                            CargarDatosNotaMedica(selectedNotaMedica,true);
                        });
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_NOTA_MEDICA);
                        IsInterconsultaEnabled = true;
                        if (_permisos_agregar)
                            MenuGuardarEnabled = true;
                        else
                            MenuGuardarEnabled = false;
                        StaticSourcesViewModel.SourceChanged = false;
                        setValidaciones();
                        break;
                        
                    case "guardar_menu":
                        if ((lstServAuxSeleccionados==null || lstServAuxSeleccionados.Count==0) && (lstExtEspecialidad==null || lstExtEspecialidad.Count==0))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar por lo menos una especialidad o servicio auxiliar de diagnostico y tratamiento");
                            return;
                        }
                        if (modoVistaModelo == enumModo.EDICION)
                            OnBuscarPorHuella();
                        else
                        {
                            await Guardar();
                            MenuGuardarEnabled = false;
                        }
                        //MenuAgregarEnabled = false;
                        break;
                    case "buscar_menu":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            CargarAtencionTipoCanalBuscar(true);
                            BuscarCanalizacion(null, null, null, string.Empty, string.Empty, string.Empty, null, null, true);
                            selectedAtencion_TipoCanalBuscarValue = -1;
                            RaisePropertyChanged("SelectedAtencion_TipoCanalBuscarValue");
                        });
                        LimpiarBusqueda();
                        PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.BUSCAR_CANALIZACIONES);
                        break;
                    case "cancelar_buscar_canalizacion":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_CANALIZACIONES);
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
                    case "seleccionar_canalizacion":
                        Limpiar();
                        if (selectedCanalizacionBusqueda==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debe de seleccionar una canalización!");
                            return;
                        }
                        selectedCanalizacion = SelectedCanalizacionBusqueda;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                            CargarDatosNotaMedica(selectedCanalizacion.NOTA_MEDICA,true);
                            CargarDatosCanalizacion(selectedCanalizacion, true);
                        });
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_CANALIZACIONES);
                        IsInterconsultaEnabled = true;
                        MenuAgregarEnabled = false;
                        MenuGuardarEnabled = true;
                        modoVistaModelo = enumModo.EDICION;
                        //if (_permisos_editar)
                        //    MenuGuardarEnabled = true;
                        //else
                        //    MenuGuardarEnabled = false;
                        MenuAtencionEnabled = true;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                       
                    case "limpiar_menu":
                        if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea limpiar la pantalla?") != 1)
                            return;
                    }
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new SolicitudInterconsultaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new SolicitudInterconsultaViewModel();
                        break;
                    case "salir_menu":
                        SalirMenu();
                        break;
                    case "menu_eliminar":
                        if (selectedCanalizacion == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una canalización");
                            return;
                        }
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "Existen cambios sin guardar,¿desea cancelar la canalización?") != 1)
                                return;
                        }
                        else
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "¿Esta seguro de cancelar la canalización?") != 1)
                                return;
                        }
                        //if (!new cUsuarioRol().ObtenerTodos(StaticSourcesViewModel.UsuarioLogin.Username).Any(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_ROL == (short)enumRolesCoordinadoresAreasTecnicas.COORDINADOR_MEDICO))
                        //{
                        //    new Dialogos().ConfirmacionDialogo("Validación", "Solamente el coordinador médico puede realizar una cancelación");
                        //    return;
                        //}
                        modoVistaModelo = enumModo.CANCELAR;
                        await Guardar();
                        break;
                    case "agregar_especialidad_tratamiento":
                        if (selectedInterconsultaTipoAtencion == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un tipo de especilialidad");
                            return;
                        }
                        if (selectedEspecialidad==-1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una especialidad");
                            return;
                        }
                        if (lstExtEspecialidad!=null && lstExtEspecialidad.Any(a=>a.ID==selectedEspecialidad))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La especialidad ya fue agregada al plan de tratamiento");
                            return;
                        }

                        if (lstExtEspecialidad==null)
                        {
                            lstExtEspecialidad = new ObservableCollection<EXT_ESPECIALIDAD>();
                        }
                        lstExtEspecialidad.Add(new EXT_ESPECIALIDAD
                            {
                                ISCHECKED=true,
                                ID = selectedEspecialidad,
                                Descr = lstEspecialidad.First(w => w.ID_ESPECIALIDAD == selectedEspecialidad).DESCR,
                                LstInterconsulta_Tipo = new List<INTERCONSULTA_TIPO>(lstExtEspecialidadInterconsulta_Tipo),
                                SelectedInterconsulta_Tipo = -1,
                                LstNivel_Prioridad=new List<INTERCONSULTA_NIVEL_PRIORIDAD>(lstExtEspecialidadNvlPrioridades),
                                SelectedNivel_Prioridad=-1,
                                IsProgramado=false,
                                ID_INTERAT = selectedInterconsultaTipoAtencion,
                                INTERCONSULTA_ATENCION_DESCR=lstInterconsultaTipo_Atencion.First(w => w.ID_INTERAT == selectedInterconsultaTipoAtencion).DESCR
                            });
                        RaisePropertyChanged("LstExtEspecialidad");
                        isEspecialidadesValid = true;
                        break;
                    case "atencion_menu":
                        var metro = Application.Current.Windows[0] as MetroWindow;
                        GC.Collect();
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new AtencionSolicitudInterconsultaView();
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new AtencionSolicitudInterconsultaViewModel(selectedCanalizacion);
                        break;
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


        private void CargarDatosCanalizacion(CANALIZACION canalizacion, bool isExceptionManaged = false)
        {
            try
            {
                lstExtEspecialidad = new ObservableCollection<EXT_ESPECIALIDAD>(canalizacion.CANALIZACION_ESPECIALIDAD.Select(s => new EXT_ESPECIALIDAD
                {
                    ISCHECKED = true,
                    Descr = s.ESPECIALIDAD.DESCR,
                    ID = s.ID_ESPECIALIDAD,
                    ID_INTERAT = s.ID_INTERAT.Value,
                    INTERCONSULTA_ATENCION_DESCR = s.INTERCONSULTA_ATENCION_TIPO.DESCR,
                    LstInterconsulta_Tipo = lstExtEspecialidadInterconsulta_Tipo,
                    LstNivel_Prioridad = lstExtEspecialidadNvlPrioridades,
                    SelectedInterconsulta_Tipo = s.ID_INTER.HasValue ? s.ID_INTER.Value : (short)-1,
                    SelectedNivel_Prioridad = s.ID_INIVEL.HasValue ? s.ID_INIVEL.Value : (short)-1,
                    IsProgramado = (s.CANALIZACION.INTERCONSULTA_SOLICITUD != null && s.CANALIZACION.INTERCONSULTA_SOLICITUD.Count > 0 ?
                    s.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(f => f.ID_ESPECIALIDAD == s.ID_ESPECIALIDAD && f.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA && f.ESTATUS != "C") : false) ||
                    (s.CANALIZACION.INTERCONSULTA_SOLICITUD != null && s.CANALIZACION.INTERCONSULTA_SOLICITUD.Count > 0 ?
                    s.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(f => f.ID_ESPECIALIDAD == s.ID_ESPECIALIDAD && f.ID_INTER == (short)enumInterconsulta_Tipo.INTERNA && f.ESTATUS != "C") : false)
                }));
                RaisePropertyChanged("LstExtEspecialidad");
                lstServAuxSeleccionados = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(canalizacion.CANALIZACION_SERV_AUX.Select(s => new EXT_SERV_AUX_DIAGNOSTICO
                {
                    DESCR = s.SERVICIO_AUX_DIAG_TRAT.DESCR,
                    ID_SERV_AUX = s.ID_SERV_AUX,
                    ID_SUBTIPO_SADT = s.SERVICIO_AUX_DIAG_TRAT.ID_SUBTIPO_SADT.Value,
                    ISCHECKED = true,
                    SUBTIPO_DESCR = s.SERVICIO_AUX_DIAG_TRAT.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR,
                    LstInterconsulta_Tipo = lstExtEspecialidadInterconsulta_Tipo,
                    LstNivel_Prioridad = lstExtEspecialidadNvlPrioridades,
                    SelectedInterconsulta_Tipo = s.ID_INTER.HasValue ? s.ID_INTER.Value : (short)-1,
                    SelectedNivel_Prioridad = s.ID_INIVEL.HasValue ? s.ID_INIVEL.Value : (short)-1,
                    IsProgramado = (s.CANALIZACION.INTERCONSULTA_SOLICITUD != null && s.CANALIZACION.INTERCONSULTA_SOLICITUD.Count > 0 ?
                    s.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SERVICIO_AUX_INTERCONSULTA.Any(a2 => a2.ID_SERV_AUX == s.ID_SERV_AUX) && a.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA && a.ESTATUS != "C") : false) ||
                    (s.CANALIZACION.INTERCONSULTA_SOLICITUD != null && s.CANALIZACION.INTERCONSULTA_SOLICITUD.Count > 0 ?
                    s.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SERVICIO_AUX_INTERCONSULTA.Any(a2 => a2.ID_SERV_AUX == s.ID_SERV_AUX) && a.ID_INTER == (short)enumInterconsulta_Tipo.INTERNA && a.ESTATUS != "C") : false)
                }));
                RaisePropertyChanged("LstServAuxSeleccionados");
            }
            catch (Exception ex)
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
            text_Pronostico_Descr = nota_medica.ID_PRONOSTICO.HasValue?nota_medica.PRONOSTICO.DESCR:string.Empty;
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
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando solicitud de canalización", () =>
                    {
                       
                        var _serv_aux_seleccionados = new List<CANALIZACION_SERV_AUX>();
                        if (lstServAuxSeleccionados!=null)
                            foreach (var item in lstServAuxSeleccionados)
                                _serv_aux_seleccionados.Add(new CANALIZACION_SERV_AUX {
                                    ID_ATENCION_MEDICA=selectedNotaMedica.ID_ATENCION_MEDICA,
                                    ID_ESTATUS="P",
                                    ID_FECHA=_fecha_servidor,
                                    ID_INIVEL=item.SelectedNivel_Prioridad!=-1?(short?)item.SelectedNivel_Prioridad:null,
                                    ID_INTER=item.SelectedInterconsulta_Tipo!=-1?(short?)item.SelectedInterconsulta_Tipo:null,
                                    ID_SERV_AUX=item.ID_SERV_AUX,
                                    ID_CENTRO_UBI=GlobalVar.gCentro
                                });
                        var _especialidades = new List<CANALIZACION_ESPECIALIDAD>();
                        if (lstExtEspecialidad!=null)
                            foreach (var item in lstExtEspecialidad)
                                _especialidades.Add(new CANALIZACION_ESPECIALIDAD {
                                    ID_ATENCION_MEDICA=selectedNotaMedica.ID_ATENCION_MEDICA,
                                    ID_ESPECIALIDAD=item.ID,
                                    ID_ESTATUS="P",
                                    ID_FECHA=_fecha_servidor,
                                    ID_INIVEL=item.SelectedNivel_Prioridad!=-1?(short?)item.SelectedNivel_Prioridad:null,
                                    ID_INTER=item.SelectedInterconsulta_Tipo!=-1?(short?)item.SelectedInterconsulta_Tipo:null,
                                    ID_INTERAT = item.ID_INTERAT,
                                    ID_CENTRO_UBI=GlobalVar.gCentro
                                });
                        var _canalizacion = new CANALIZACION
                        {
                            ID_ATENCION_MEDICA=selectedNotaMedica.ID_ATENCION_MEDICA,
                            ID_ESTATUS_CAN="P",
                            ID_FECHA=_fecha_servidor,
                            ID_USUARIO=GlobalVar.gUsr,
                            CANALIZACION_ESPECIALIDAD=_especialidades,
                            CANALIZACION_SERV_AUX=_serv_aux_seleccionados,
                            ID_CENTRO_UBI=GlobalVar.gCentro
                        };
                        new cCanalizacion().Insertar(_canalizacion,GlobalVar.gCentro,(short)enumMensajeTipo.SOLICITUD_CANALIZACION,_fecha_servidor);
                        selectedCanalizacion = new cCanalizacion().ObtenerCanalizaciones(_canalizacion.ID_ATENCION_MEDICA, _canalizacion.ID_CENTRO_UBI).FirstOrDefault();
                        return true;
                    }))
                    {
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La solicitud de interconsulta fue guardada con exito");
                        //Limpiar();

                        IsInterconsultaEnabled = false;
                        MenuAtencionEnabled = true;
                        ClearRules();
                        StaticSourcesViewModel.SourceChanged = false;

                    }
                }
                else if (modoVistaModelo==enumModo.EDICION)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando solicitud", () =>
                        {
                            var _serv_aux_seleccionados = new List<CANALIZACION_SERV_AUX>();
                            if (lstServAuxSeleccionados != null)
                                foreach (var item in lstServAuxSeleccionados)
                                    _serv_aux_seleccionados.Add(new CANALIZACION_SERV_AUX
                                    {
                                        ID_ATENCION_MEDICA = selectedCanalizacion.ID_ATENCION_MEDICA,
                                        ID_ESTATUS = item.IsProgramado?"A":"P",
                                        ID_FECHA = _fecha_servidor,
                                        ID_INIVEL = item.SelectedNivel_Prioridad != -1 ? (short?)item.SelectedNivel_Prioridad : null,
                                        ID_INTER = item.SelectedInterconsulta_Tipo != -1 ? (short?)item.SelectedInterconsulta_Tipo : null,
                                        ID_SERV_AUX = item.ID_SERV_AUX,
                                        ID_CENTRO_UBI = GlobalVar.gCentro
                                    });
                            var _especialidades = new List<CANALIZACION_ESPECIALIDAD>();
                            if (lstExtEspecialidad != null)
                                foreach (var item in lstExtEspecialidad)
                                    _especialidades.Add(new CANALIZACION_ESPECIALIDAD
                                    {
                                        ID_ATENCION_MEDICA = selectedCanalizacion.ID_ATENCION_MEDICA,
                                        ID_ESPECIALIDAD = item.ID,
                                        ID_ESTATUS = item.IsProgramado?"A":"P",
                                        ID_FECHA = _fecha_servidor,
                                        ID_INIVEL = item.SelectedNivel_Prioridad != -1 ? (short?)item.SelectedNivel_Prioridad : null,
                                        ID_INTER = item.SelectedInterconsulta_Tipo != -1 ? (short?)item.SelectedInterconsulta_Tipo : null,
                                        ID_INTERAT = item.ID_INTERAT,
                                        ID_CENTRO_UBI = GlobalVar.gCentro
                                    });
                            var _canalizacion = new CANALIZACION
                            {
                                ID_ATENCION_MEDICA = selectedCanalizacion.ID_ATENCION_MEDICA,
                                ID_ESTATUS_CAN = "P",
                                ID_FECHA = _fecha_servidor,
                                ID_USUARIO = GlobalVar.gUsr,
                                CANALIZACION_ESPECIALIDAD = null,
                                CANALIZACION_SERV_AUX = null,
                                ID_CENTRO_UBI = GlobalVar.gCentro
                            };
                            new cCanalizacion().Actualizar(_canalizacion, _especialidades, _serv_aux_seleccionados, GlobalVar.gCentro, _fecha_servidor,(short) enumMensajeTipo.MODIFICACION_CANALIZACION, null);
                            selectedCanalizacion = new cCanalizacion().ObtenerCanalizaciones(_canalizacion.ID_ATENCION_MEDICA, _canalizacion.ID_CENTRO_UBI).FirstOrDefault();
                            return true;
                        }
                        ))
                    {
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La solicitud de interconsulta fue guardada con exito");
                        //Limpiar();
                        MenuGuardarEnabled = false;
                        MenuAgregarEnabled = true;
                        IsInterconsultaEnabled = false;
                        ClearRules();
                        StaticSourcesViewModel.SourceChanged = false;
                        //selectedCanalizacion = null;
                    }
                    
                }
                else if (modoVistaModelo==enumModo.CANCELAR)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cancelando solicitud", () =>
                    {
                        var _canalizacion = new CANALIZACION {
                            ID_ATENCION_MEDICA=selectedCanalizacion.ID_ATENCION_MEDICA,
                            ID_ESTATUS_CAN="C",
                            ID_FECHA=_fecha_servidor,
                            ID_USUARIO=selectedCanalizacion.ID_USUARIO,
                            ID_CENTRO_UBI=selectedCanalizacion.ID_CENTRO_UBI
                        };
                        new cCanalizacion().Actualizar(_canalizacion, null, null, GlobalVar.gCentro, _fecha_servidor, null,(short)enumMensajeTipo.CANCELACION_CANALIZACION);
                        return true;
                    }
                        ))
                    {
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La canalización fue cancelada con exito");
                        Limpiar();
                        IsInterconsultaEnabled = false;
                        MenuAtencionEnabled = false;
                        ClearRules();
                        StaticSourcesViewModel.SourceChanged = false;
                        selectedCanalizacion = null;
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
            
            TextSexoImputado = string.Empty;
            
            
            SelectedInterconsultaTipoAtencion = -1;
            SelectedEspecialidad = -1;
            LstSubtipoServAux = new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new List<SUBTIPO_SERVICIO_AUX_DIAG_TRAT> {
                new SUBTIPO_SERVICIO_AUX_DIAG_TRAT{
                    DESCR="SELECCIONE",
                    ID_SUBTIPO_SADT=-1,
                    ID_TIPO_SADT=-1
                }
            });
            SelectedTipoServAux = -1;
            SelectedSubtipoServAux = -1;
            LstServAuxSeleccionados = null;
            isServAuxSeleccionadosValid = false;
            isEspecialidadesValid = false;
            LstExtEspecialidad = null;
            LstServAux = null;
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
        }

        private void LimpiarBusqueda()
        {
            AnioBuscarCanal = null;
            FolioBuscarCanal = null;
            NombreBuscarCanal = null;
            ApellidoPaternoBuscarCanal = null;
            ApellidoMaternoBuscarCanal = null;
            FechaInicialBuscarCanal = null ;
            FechaFinalBuscarCanal = null;
            SelectedAtencion_TipoCanalBuscarValue = -1;
            SelectedCanalizacionBusqueda = null;
        }

        private void OnChecked(object parametro)
        {
            var parametros=(object[])parametro;
            switch (parametros[0].ToString())
            {
                case "ServAux":
                    var item = LstServAux.FirstOrDefault(w => w.ID_SERV_AUX == Convert.ToInt16(parametros[1]));
                    if (lstServAuxSeleccionados == null)
                    {
                        lstServAuxSeleccionados = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(new List<EXT_SERV_AUX_DIAGNOSTICO> {
                        new EXT_SERV_AUX_DIAGNOSTICO{
                            DESCR=item.DESCR,
                            ESTATUS=item.ESTATUS,
                            ID_SERV_AUX=item.ID_SERV_AUX,
                            ID_SUBTIPO_SADT=item.ID_SUBTIPO_SADT,
                            ISCHECKED=item.ISCHECKED,
                            SUBTIPO_DESCR=item.SUBTIPO_DESCR,
                            IsProgramado=false,
                            LstInterconsulta_Tipo=lstExtEspecialidadInterconsulta_Tipo,
                            LstNivel_Prioridad=lstExtEspecialidadNvlPrioridades
                            }
                        });
                        OnPropertyValidateChanged("LstServAuxSeleccionados");
                        isServAuxSeleccionadosValid = true;
                    }
                    else
                    {
                        if (Convert.ToBoolean(parametros[2]))
                        {
                            lstServAuxSeleccionados.Add(new EXT_SERV_AUX_DIAGNOSTICO {
                                DESCR=item.DESCR,
                                ESTATUS=item.ESTATUS,
                                ID_SERV_AUX=item.ID_SERV_AUX,
                                ID_SUBTIPO_SADT=item.ID_SUBTIPO_SADT,
                                ISCHECKED=item.ISCHECKED,
                                SUBTIPO_DESCR=item.SUBTIPO_DESCR,
                                IsProgramado = false,
                                LstInterconsulta_Tipo = lstExtEspecialidadInterconsulta_Tipo,
                                LstNivel_Prioridad = lstExtEspecialidadNvlPrioridades

                            });
                            lstServAuxSeleccionados = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(lstServAuxSeleccionados);
                            RaisePropertyChanged("LstServAuxSeleccionados");
                            isServAuxSeleccionadosValid = true;
                        }
                        else
                        {
                            lstServAuxSeleccionados.Remove(lstServAuxSeleccionados.FirstOrDefault(w => w.ID_SERV_AUX == item.ID_SERV_AUX));
                            lstServAuxSeleccionados = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(lstServAuxSeleccionados);
                            RaisePropertyChanged("LstServAuxSeleccionados");
                            if (LstServAuxSeleccionados == null || LstServAuxSeleccionados.Count == 0)
                                isServAuxSeleccionadosValid = false;
                        }
                    }
                    break;
                case "ServAuxSolicitado":
                    if (!Convert.ToBoolean(parametros[2]))
                    {
                        var item_seleccionado = lstServAuxSeleccionados.FirstOrDefault(w => w.ID_SERV_AUX == Convert.ToInt16(parametros[1]));
                        if (lstServAux != null && lstServAux.Count > 0)
                        {
                            var item_ref = lstServAux.FirstOrDefault(w => w.ID_SERV_AUX == item_seleccionado.ID_SERV_AUX);
                            if (item_ref != null)
                            {
                                item_ref.ISCHECKED = false;
                                lstServAux = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(lstServAux);
                                OnPropertyValidateChanged("LstServAux");
                            }
                        }
                        lstServAuxSeleccionados.Remove(item_seleccionado);
                        lstServAuxSeleccionados = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(lstServAuxSeleccionados);
                        OnPropertyValidateChanged("LstServAuxSeleccionados");
                        if (LstServAuxSeleccionados==null || LstServAuxSeleccionados.Count==0)
                            isServAuxSeleccionadosValid = false;
                    }
                    break;
                case "Especialidad":
                    if  (!Convert.ToBoolean(parametros[2]))
                    {
                        var item_seleccionado = lstExtEspecialidad.FirstOrDefault(w => w.ID == Convert.ToInt16(parametros[1]));
                        LstExtEspecialidad.Remove(item_seleccionado);
                        LstExtEspecialidad = new ObservableCollection<EXT_ESPECIALIDAD>(LstExtEspecialidad);
                        if (lstExtEspecialidad == null || LstExtEspecialidad.Count > 0)
                            isEspecialidadesValid = true;
                    }
                    break;
            }
        }


        private async void  OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
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
                case "cambio_fecha_inicio_busqueda":
                    if (!FechaInicialNotaMed.HasValue || !FechaFinalBuscarNotaMed.HasValue || FechaFinalBuscarNotaMed >= FechaInicialNotaMed)
                        IsFechaIniBusquedaValida = true;
                    else
                        IsFechaIniBusquedaValida = false;
                    break;
                case "cambio_fecha_inicio_busqueda_canal":
                    if (!FechaInicialBuscarCanal.HasValue || !FechaInicialBuscarCanal.HasValue || FechaInicialBuscarCanal >= FechaInicialBuscarCanal)
                        IsFechaIniBusquedaCanalValida = true;
                    else
                        IsFechaIniBusquedaCanalValida = false;
                    break;
            }
        }


        private void BuscarNotasMedicasInterconsulta(short? tipo_atencion=null,short? anio=null, int? folio=null, string nombre="", string paterno="", string materno="", DateTime? fecha_inicio=null, DateTime? fecha_fin=null, bool isExceptionManaged=false)
        {
            try
            {
                lstNotasMedicasBusqueda = new ObservableCollection<NOTA_MEDICA>(new cNotaMedica().ObtenerNotasConInterconsulta(GlobalVar.gCentro,estatus_administrativos_inactivos,
                    tipo_atencion,anio, folio, nombre, paterno, materno, fecha_inicio, fecha_fin).OrderByDescending(w=>w.ATENCION_MEDICA.ATENCION_FEC));
                RaisePropertyChanged("LstNotasMedicasBusqueda");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las notas médicas", ex);
            }
        }

        #endregion

        #region catalogos
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

        private void CargarInterconsultaAtencionTipo(bool isExceptionManaged=false)
        {
            try
            {
                lstInterconsultaTipo_Atencion = new ObservableCollection<INTERCONSULTA_ATENCION_TIPO>(new cInterconsultaAtencionTipo().ObtenerTodos("", "S").Where(w=>w.CATALOGO_ESPECIALIDADES=="1"));
                lstInterconsultaTipo_Atencion.Insert(0, new INTERCONSULTA_ATENCION_TIPO
                {
                    ID_INTERAT=-1,
                    DESCR="SELECCIONE UNO"
                });
                RaisePropertyChanged("LstInterconsultaTipo_Atencion");
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
                    SUBTIPO_DESCR=s.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR,
                    IsProgramado=false
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
                                {
                                    item_catalogo.ISCHECKED = true;
                                    item_catalogo.IsProgramado = item.IsProgramado;
                                }
                                    
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
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los tipos de atención", ex);
            }
        }

        private void CargarAtencionTipo(bool isExceptionManaged = false)
        {
            try
            {
                lstAtencion_Tipo = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                lstAtencion_Tipo.Insert(0, new ATENCION_TIPO
                {
                    ID_TIPO_ATENCION = -1,
                    DESCR = "SELECCIONE UNA"
                });
                RaisePropertyChanged("LstAtencion_Tipo");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los tipos de atención", ex);
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
                if (selectedNotaMedica == null)
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
                if (selectedNotaMedica == null)
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
        private void BuscarCanalizacion(short? tipo_atencion = null, short? anio = null, int? folio = null, string nombre = "", string paterno = "", string materno = "", DateTime? fecha_inicio = null, DateTime? fecha_fin = null, bool isExceptionManaged = false)
        {
            try
            {
                lstCanalizacionesBusqueda = new ObservableCollection<CANALIZACION>(new cCanalizacion().ObtenerCanalizaciones("P", GlobalVar.gCentro, estatus_administrativos_inactivos,
                    tipo_atencion, anio, folio, nombre, paterno, materno, fecha_inicio, fecha_fin).OrderByDescending(w=>w.ID_FECHA));
                RaisePropertyChanged("LstCanalizacionesBusqueda");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las canalizaciones", ex);
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
                        //EliminarMenuEnabled = true;
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

        #region HUELLAS DIGITALES

        private async void OnBuscarPorHuella(string obj = "")
        {
            try
            {
                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
                await TaskEx.Delay(400);
                var nRet = -1;
                BanderaHuella = true;
                if (ParametroRequiereGuardarHuellas)
                    try
                    {
                        nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                    }
                    catch
                    {
                        BanderaHuella = false;
                    }
                else
                    BanderaHuella = false;

                WindowBusqueda = new BusquedaHuella();
                var dataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_EMPLEADO, nRet == 0, ParametroRequiereGuardarHuellas);
                WindowBusqueda.DataContext = dataContext;
                WindowBusqueda.dgHuella.Columns.Insert(WindowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Persona")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "TIPO VISITA"
                });
                dataContext.CabeceraBusqueda = string.Empty;
                dataContext.CabeceraFoto = string.Empty;
                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(WindowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }
                WindowBusqueda.Owner = PopUpsViewModels.MainWindow;
                WindowBusqueda.KeyDown -= HuellaKeyDown;
                WindowBusqueda.KeyDown += HuellaKeyDown;
                WindowBusqueda.Closed -= HuellaClose;
                WindowBusqueda.Closed += HuellaClose;
                WindowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }
        private void HuellaKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Escape) WindowBusqueda.Close();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }
        private async void HuellaClose(object s, EventArgs e)
        {
            try
            {
                if (BanderaHuella == true) CLSFPCaptureDllWrapper.CLS_Terminate();
                CodigoEnabled = NombreReadOnly = false;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                var huella = ((BusquedaHuellaViewModel)WindowBusqueda.DataContext);
                if (!huella.IsSucceed || huella.ScannerMessage == "HUELLA NO ENCONTRADA" || huella.SelectRegistro==null || huella.SelectRegistro.Persona==null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "No se valido la firma del coordinador médico!");
                    return;
                }
                var _persona = new cPersona().ObtenerPersona(huella.SelectRegistro.Persona.ID_PERSONA);
                if (!_persona.EMPLEADO.USUARIO.Any(a=>a.ESTATUS=="S" && a.USUARIO_ROL.Any(a2=>a2.ID_ROL==(short)enumRolesCoordinadoresAreasTecnicas.COORDINADOR_MEDICO && a2.ID_CENTRO==GlobalVar.gCentro)))
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "No se valido la firma del coordinador médico!");
                    return;
                }
                await Guardar();
                //Aqui se maneja lo que pasa despues de cerrar la huella
                //if ()
                //{
                //    var persn = SelectPersona;
                //    ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                //    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                //    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                //    SelectPersonaAuxiliar = persn;
                //    return;
                //}
                //if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null) return;
                //await SeleccionarPersona(huella.SelectRegistro.Persona);
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
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
            EDICION=2,
            CANCELAR=3
        }

    }
}

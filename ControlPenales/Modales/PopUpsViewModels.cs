using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    public class PopUpsViewModels
    {
        #region [Lista de PopUps]
        public enum TipoPopUp
        {
            TOMAR_FOTO_IFE_CEDULA,
            AGREGAR_VISITA_EDIFICIO,
            AGREGAR_VISITA_APELLIDO,
            AGREGAR_ESTUDIO_PERSONALIDAD,
            AGREGAR_ESTUDIO_PERSONALIDAD_DETALLE,
            INGRESAR_LISTA_TONTA,
            CERRAR_TODOS,
            APODO,
            FOTOSSENIASPARTICULAES,
            ALIAS,
            HUELLAS,
            BUSCAR_CONSULTAS_MEDICAS,
            BUSCAR_VISITA_EXISTENTE,
            BUSCAR_PERSONAS_EXISTENTES,
            BUSCAR_PERSONAS_EXTERNAS,
            AGREGAR_VISITA_EXTERNA,
            AMPLIAR_DESCRIPCION,
            AMPLIAR_DESCRIPCION_GENERICO,
            AGREGAR_DOCUMENTO,
            AGREGAR_APOYO_ECONOMICO,
            AGREGAR_NUCELO_FAMILIAR,
            PANDILLA,
            RELACION_INTERNO,
            ULTIMOS_EMPLEOS,
            DATOS_GRUPO_FAMILIAR,
            USO_DROGAS,
            FAMILIAR_DELITO,
            FAMILIAR_DROGA,
            INGRESA_ACOMPANIANTE,
            INGRESA_IMPUTADO_VISITANTE,
            BUSQUEDA,
            BUSQUEDA_EXPEDIENTE,
            PROGRAMAR_VISITA,
            SANCION_DISCIPLINARIA,
            INGRESO_ANTERIOR,
            AGREGAR_DICTAMEN,
            AGREGAR_ACTIVIDAD,
            AGREGAR_GFPV,
            AGREGAR_IAS,
            SELECCIONA_UBICACION,
            SELECCIONA_UBICACION_CELDA,
            AGREGAR_DELITO,
            AGREGAR_USO_DROGA_TS,
            AGREGAR_COPARTICIPE,
            AGREGAR_PARAMETROS,
            TOPOGRAFIA_HUMANA,
            AGREGAR_DELITO_CP,
            AGREGAR_CELDAS_OBSERVACION,
            OSCURECER_FONDO,
            DIGITALIZAR_DOCUMENTO,
            EMPALME_FECHAS,
            SELECCIONA_COLOR,
            REGISTRO_CORRESPONDENCIA,
            AGREGAR_INCIDENTE,
            AGREGAR_SANCION,
            NOTIFICACION,
            BUSCAR_DECOMISO,
            BUSCAR_OFICIALACARGO,
            BUSCAR_INTERNO,
            BUSCAR_INTERNO_CP,
            BUSCAR_VISITANTE,
            BUSCAR_EMPLEADO,
            BUSCAR_EXTERNO,
            EDITAR_FECHA,
            EDITAR_INTEGRANTES_GRUPO,
            PROXIMA_CAUSA_PENAL,
            BUSCAR_EVENTO,
            AGREGAR_EVENTO_PROGRAMA,
            AGREGAR_EVENTO_PRESIDIUM,
            AGREGAR_EVENTO_INF_TECNICA,
            AGREGAR_PROCESO,
            AGREGAR_ROL,
            AGREGAR_PROCESO_USUARIO,
            AGREGAR_USUARIO,
            IMPRESION_DECOMISO,
            VISUALIZAR_DOCUMENTOS,
            BUSCAR_TRASLADOS,
            AGREGAR_PARTICIPANTE_GRUPO,
            AGREGAR_FECHA_PARTICIPANTE,
            AGREGAR_PARTICIPANTE_COMPLEMENTARIO,
            AGENDAR_CITA,
            BUSCAR_PERSONA,
            LISTA_ASISTENCIA,
            LISTA_ACTIVIDADES_INTERNO,
            LISTA_ASISTENCIA_INTERNO_EDIFICIO,
            BUSQUEDA_INTERNOS_PROGRAMAS,
            VER_SELECCIONADOSCOMPL,
            BUSCAR_LIBERADO,
            BUSCAR_NUCS_IMPUTADO,
            BUSCAR_EXCARCELACIONES,
            AGREGAR_CANCELAR_SUSPENDER,
            BUSCAR_IMPUTADO_NUC,
            AGREGAR_DESTINO_EXCARCELACION,
            MOTIVO,
            SOLICITUD_CITA,
            EXCARCELACION_CANCELACION_MOTIVO,
            BUSCAR_DECOMISO_EVENTO,
            ESCOLARIDAD_ESTUDIO_EDUCATIVO_FUERO_COMUN,
            ACTIVIDADES_EDUCATIVAS_EDUCATIVO_FUERO_COMUN,
            ACTIVIDADES_CULTURALES_DEPORTIVAS_EDUCATIVO_FUERO_COMUN,
            ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN,
            AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN,
            AGREGAR_CONGREGACIONES_SOCIFAM_FC,
            AGREGAR_AGENDA_MEDICA,
            AGREGAR_OPINION_AREA_TECNICA,
            HISTORICO_TRATAMIENTO,
            BUSCAR_AGENDA,
            AGREGAR_INTEGRANTE_GRUPO_FAMILIAR_TRABAJO_SOCIAL_FUERO_FEDERAL,
            AGREGAR_DIAS_LABORADOS_FUERO_FEDERAL,
            AGREGAR_EDUC_CUL_FUERO_FEDERAL,
            SANCIONES_INFORME_VIGILANCIA_FUERO_FEDERAL,
            EXCARCELACIONES_CANCELAR,
            AGREGAR_DOCUMENTOS_HISTORIA_CLINICA,
            DIGITALIZACION_SIMPLE,
            INCIDENCIA_ATENCION_CITA,
            VISUALIZAR_LISTA_DOCUMENTOS,
            BUSCAR_NOTA_MEDICA,
            SELECCIONAR_REPORTE_SANCION,
            AGREGAR_IMAGENES_HISTORIA_CLINICA_DENTAL,
            FILTRO_REPORTE_CAUSAS_PENALES,
            SELECCIONAR_INTERCONSULTA_MEDICA_EXCARCELACION,
            BUSCAR_INTERCONSULTA_SOLICITUD,
            AGREGAR_DOCUMENTOS_RESULT_TRATAMIENTO,
            AGREGAR_ESPECIALISTAS,
            AGENDA_ESPECIALISTAS,
            AGREGAR_CITA_ESPECIALISTA,
            AGREGAR_PROCESO_LIBERTAD,
            AGREGAR_MEDIDA,
            AGREGAR_MEDIDA_ESTAUS,
            BUSCAR_CANALIZACIONES,
            AGREGAR_PERSONA,
            SELECCIONAR_VISITANTES,
            AGREGAR_LUGAR,
            AGREGAR_MEDIDA_PRESENTACION,
            AGREGAR_MEDIDA_DOCUMENTO,
            BUSCAR_INTERCONSULTA_MEMORIA,
            VER_MEDIDA_ESTATUS,
            VER_MEDIDA_PERSONA,
            VER_MEDIDA_PRESENTACION,
            SELECCIONAR_VISITANTES_PERMITIDOS_MULTIPLE,
            NOTAS_MEDICAS_HOSPITALIZACION,
            AGREGAR_CONCENTRADO_HOJA_LIQUIDOS,
            VER_MEDIDA_LUGARES,
            DEFINIR_FECHAS_DESARROLLO_PERSONALIDAD,
            AGREGAR_PROCESO_LIBERTAD_SEGUMIENTO,
            AGREGAR_OBSERVACIONES_MEDICAMENTOS_RECETA_MEDICA,
            BUSCAR_ESCALA_RIESGO,
            AGREGAR_ACTIVIDAD_PROGRAMA,
            AGENDA_LIBERTAD,
            CURSOS_CAPACITACION_FEDERALES,
            OFICIO_ASIGNACION_NSJP,
            OFICIO_ASIGNACION_TRADICIONAL,
            OFICIO_CONCLUSION,
            OFICIO_BAJA,
            BUSCAR_CAUSA_PENAL,
            BUSCAR_TV_MEDICO_PENDIENTE,
            REAGENDAR_TV_CITA_MEDICA,
            RECREAR_TV_INTERCONSULTA,
            CAPTURA_OFICIO_PERSONALIDAD
        }
        #endregion

        #region [Variables]
        private static object _PopUpDataContext;
        public static object PopUpDataContext
        {
            get { return _PopUpDataContext; }
        }

        private static PrincipalView _MainWindow;
        public static PrincipalView MainWindow
        {
            get { return PopUpsViewModels._MainWindow; }
            set
            {
                PopUpsViewModels._MainWindow = value;

                value.AgregarInterconsultaMedicaExcarcelacionView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarInterconsultaMedicaExcarcelacionView.Focusable = true;
                        MainWindow.AgregarInterconsultaMedicaExcarcelacionView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up seleccionar interconsulta medica en excarcelacion", ex);
                    }
                };

                value.AgregarObservacionesMedicamentosRecetaMedicaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarObservacionesMedicamentosRecetaMedicaView.Focusable = true;
                        MainWindow.AgregarObservacionesMedicamentosRecetaMedicaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar observaciones a medicamentos en la receta medica", ex);
                    }
                };

                value.CapturarNumeroOficioPersonalidadView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.CapturarNumeroOficioPersonalidadView.Focusable = true;
                        MainWindow.CapturarNumeroOficioPersonalidadView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de captura de oficio de estudios de personalidad", ex);
                    }
                };

                value.AgregarFechasInicioFinDesarrolloPersonalidadView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarFechasInicioFinDesarrolloPersonalidadView.Focusable = true;
                        MainWindow.AgregarFechasInicioFinDesarrolloPersonalidadView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up seleccionar interconsulta medica en excarcelacion", ex);
                    }
                };

                value.AgregarActivididadesProdCapacFederalesView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarActivididadesProdCapacFederalesView.Focusable = true;
                        MainWindow.AgregarActivididadesProdCapacFederalesView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de cursos de capacitacion", ex);
                    }
                };

                value.TomarFotosIfeCedulaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.TomarFotosIfeCedulaView.Focusable = true;
                        MainWindow.TomarFotosIfeCedulaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up visitante lista", ex);
                    }
                };

                value.AgregarEspecialistaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarEspecialistaView.Focusable = true;
                        MainWindow.AgregarEspecialistaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up especialistas", ex);
                    }
                };

                value.AgregarConcentradoHojaControlLiquidosView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarConcentradoHojaControlLiquidosView.Focusable = true;
                        MainWindow.AgregarConcentradoHojaControlLiquidosView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de concentrado en control de liquidos", ex);
                    }
                };
                value.AgregarSancionDisciVigilanciaFF.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarSancionDisciVigilanciaFF.Focusable = true;
                        MainWindow.AgregarSancionDisciVigilanciaFF.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up sanciones", ex);
                    }
                };

                value.SeleccionarVisitanteView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.SeleccionarVisitanteView.Focusable = true;
                        MainWindow.SeleccionarVisitanteView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up visitantes", ex);
                    }
                };

                value.AgregarArchivoResultTratamView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarArchivoResultTratamView.Focusable = true;
                        MainWindow.AgregarArchivoResultTratamView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up documentos", ex);
                    }
                };

                value.ConsultarDocumentosEstudioPersonalidadView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.ConsultarDocumentosEstudioPersonalidadView.Focusable = true;
                        MainWindow.ConsultarDocumentosEstudioPersonalidadView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up documentos", ex);
                    }
                };

                value.AgregarCitaEspecialistaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarCitaEspecialistaView.Focusable = true;
                        MainWindow.AgregarCitaEspecialistaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de citas de especialistas", ex);
                    }
                };

                value.AgregarImagenHistClinicDentalView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarImagenHistClinicDentalView.Focusable = true;
                        MainWindow.AgregarImagenHistClinicDentalView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up documentos", ex);
                    }
                };

                value.DigitalizacionSimpleView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.DigitalizacionSimpleView.Focusable = true;
                        MainWindow.DigitalizacionSimpleView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up digitalizacion", ex);
                    }
                };

                value.AgregarDocumentosHistoriaClinicaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarDocumentosHistoriaClinicaView.Focusable = true;
                        MainWindow.AgregarDocumentosHistoriaClinicaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up documentos", ex);
                    }
                };

                value.AgregarActivEducativasView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarActivEducativasView.Focusable = true;
                        MainWindow.AgregarActivEducativasView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up actividades", ex);
                    }
                };

                value.AgregarProgActivEducFFView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarProgActivEducFFView.Focusable = true;
                        MainWindow.AgregarProgActivEducFFView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up actividades", ex);
                    }
                };


                value.AgregarDiasLaboradosCapacFFView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarDiasLaboradosCapacFFView.Focusable = true;
                        MainWindow.AgregarDiasLaboradosCapacFFView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de dias", ex);
                    }
                };


                value.AgregarGrupoFamTrabSocialFFView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarGrupoFamTrabSocialFFView.Focusable = true;
                        MainWindow.AgregarGrupoFamTrabSocialFFView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up integrantes", ex);
                    }
                };

                value.AgregarAreaTecnicaOpinionView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarAreaTecnicaOpinionView.Focusable = true;
                        MainWindow.AgregarAreaTecnicaOpinionView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up opiniones por area", ex);
                    }
                };

                value.AgregarCongregSocioFamFCView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarCongregSocioFamFCView.Focusable = true;
                        MainWindow.AgregarCongregSocioFamFCView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up congregacion", ex);
                    }
                };


                value.AgregarTratamientoRecibidoPsiclogicoFueroComunView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarTratamientoRecibidoPsiclogicoFueroComunView.Focusable = true;
                        MainWindow.AgregarTratamientoRecibidoPsiclogicoFueroComunView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up programas", ex);
                    }
                };

                value.AgregarCapacitacionActividadesView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarCapacitacionActividadesView.Focusable = true;
                        MainWindow.AgregarCapacitacionActividadesView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up actividades", ex);
                    }
                };

                value.AgregarActividadCultDepEducativoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarActividadCultDepEducativoView.Focusable = true;
                        MainWindow.AgregarActividadCultDepEducativoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up actividades", ex);
                    }
                };

                value.ProgramarVisitaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.ProgramarVisitaView.Focusable = true;
                        MainWindow.ProgramarVisitaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up visitante lista", ex);
                    }
                };

                //value.AgregarUsu.IsVisibleChanged += (s, e) =>
                //{
                //    try
                //    {
                //        if (!((bool)e.NewValue))
                //            return;

                //        MainWindow.ProgramarVisitaView.Focusable = true;
                //        MainWindow.ProgramarVisitaView.Focus();
                //    }
                //    catch (Exception ex)
                //    {
                //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up visitante lista", ex);
                //    }
                //};
                value.IngresarVisitanteListaTontaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.IngresarVisitanteListaTontaView.Focusable = true;
                        MainWindow.IngresarVisitanteListaTontaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up visitante lista", ex);
                    }
                };

                value.AgregaVisitaEdificioView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregaVisitaEdificioView.Focusable = true;
                        MainWindow.AgregaVisitaEdificioView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up visita por edifico", ex);
                    }
                };

                value.AgregaVisitaApellidoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregaVisitaApellidoView.Focusable = true;
                        MainWindow.AgregaVisitaApellidoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo paso...", "Ocurrió un error al ocultar/mostrar pop up visita por apellido", ex);
                    }
                };

                value.AgregarEscolaridadEstudioEducativoFueroComunView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarEscolaridadEstudioEducativoFueroComunView.Focusable = true;
                        MainWindow.AgregarEscolaridadEstudioEducativoFueroComunView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo paso...", "Ocurrió un error al ocultar/mostrar pop up escolaridad del estudio educativo", ex);
                    }
                };

                value.ProgramarVisitaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.ProgramarVisitaView.Focusable = true;
                        MainWindow.ProgramarVisitaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up visita", ex);
                    }
                };

                value.BuscarConsultasMedicasView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.BuscarConsultasMedicasView.Focusable = true;
                        MainWindow.BuscarConsultasMedicasView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up visita", ex);
                    }
                };

                value.ApodoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.ApodoView.Focusable = true;
                        MainWindow.ApodoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up apodo", ex);
                    }
                };

                value.FotosSenasView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.FotosSenasView.Focusable = true;
                        MainWindow.FotosSenasView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up tomar foto", ex);
                    }
                };

                value.AliasView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AliasView.Focusable = true;
                        MainWindow.AliasView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up alias", ex);
                    }
                };

                value.BuscarVisitaExistente.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.BuscarVisitaExistente.Focusable = true;
                        MainWindow.BuscarVisitaExistente.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up buscar visita", ex);
                    }
                };

                value.BuscarPersonasExistentes.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.BuscarPersonasExistentes.Focusable = true;
                        MainWindow.BuscarPersonasExistentes.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up buscar persona", ex);
                    }
                };

                value.AmpliarDescripcionView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AmpliarDescripcionView.Focusable = true;
                        MainWindow.AmpliarDescripcionView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up ampliar descripción", ex);
                    }
                };

                value.AmpliarDescripcionGenericoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AmpliarDescripcionGenericoView.Focusable = true;
                        MainWindow.AmpliarDescripcionGenericoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up ampliar descripción", ex);
                    }
                };

                value.AgregaDocumentoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregaDocumentoView.Focusable = true;
                        MainWindow.AgregaDocumentoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar documento", ex);
                    }
                };

                value.AgregarApoyoEconomicoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarApoyoEconomicoView.Focusable = true;
                        MainWindow.AgregarApoyoEconomicoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar documento", ex);
                    }
                };

                //value.AgregarNuceloFamView.IsVisibleChanged += (s, e) =>
                //{
                //    try
                //    {
                //        if (!((bool)e.NewValue))
                //            return;

                //        MainWindow.AgregarNuceloFamView.Focusable = true;
                //        MainWindow.AgregarNuceloFamView.Focus();
                //    }
                //    catch (Exception ex)
                //    {
                //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar documento", ex);
                //    }
                //};
                value.AgregarEstudioPersonalidadView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarEstudioPersonalidadView.Focusable = true;
                        MainWindow.AgregarEstudioPersonalidadView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar estudio de personalidad", ex);
                    }
                };


                value.AgregarEstudioPersonalidadDetalleView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarEstudioPersonalidadDetalleView.Focusable = true;
                        MainWindow.AgregarEstudioPersonalidadDetalleView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar el detalle del estudio de personalidad", ex);
                    }
                };

                value.PandillaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.PandillaView.Focusable = true;
                        MainWindow.PandillaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up pandilla", ex);
                    }
                };

                value.RelacionView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.RelacionView.Focusable = true;
                        MainWindow.RelacionView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up relación", ex);
                    }
                };

                value.AddUsoDrogasView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AddUsoDrogasView.Focusable = true;
                        MainWindow.AddUsoDrogasView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar uso drogas", ex);
                    }
                };

                value.UltimosEmpleosView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.UltimosEmpleosView.Focusable = true;
                        MainWindow.UltimosEmpleosView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up último empleo", ex);
                    }
                };

                value.GrupoFamiliarView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.GrupoFamiliarView.Focusable = true;
                        MainWindow.GrupoFamiliarView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up grupo familiar", ex);
                    }
                };

                value.FamiliarDelitoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.FamiliarDelitoView.Focusable = true;
                        MainWindow.FamiliarDelitoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up familiar delito", ex);
                    }
                };

                value.FamiliarDrogaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.FamiliarDrogaView.Focusable = true;
                        MainWindow.FamiliarDrogaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up familiar droga", ex);
                    }
                };

                value.IngresarAcompananteView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.IngresarAcompananteView.Focusable = true;
                        MainWindow.IngresarAcompananteView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up ingresar acompañante", ex);
                    }
                };

                value.IngresosAnterioresView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.IngresosAnterioresView.Focusable = true;
                        MainWindow.IngresosAnterioresView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up ingreso anterior", ex);
                    }
                };

                value.AgregarSancionDisciplinariaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarSancionDisciplinariaView.Focusable = true;
                        MainWindow.AgregarSancionDisciplinariaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar sanciones diciplinarias", ex);
                    }
                };

                value.AgregarDictamenView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarDictamenView.Focusable = true;
                        MainWindow.AgregarDictamenView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar dictamen", ex);
                    }
                };

                value.AgregarGFPVView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarGFPVView.Focusable = true;
                        MainWindow.AgregarGFPVView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar GRUPO FAMILIAR DESDE PADRON DE VISITAS", ex);
                    }
                };

                value.AgregarActividadView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarActividadView.Focusable = true;
                        MainWindow.AgregarActividadView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar actividad", ex);
                    }
                };

                value.AgregarIASView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarIASView.Focusable = true;
                        MainWindow.AgregarIASView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar ingreso anterior sistema", ex);
                    }
                };

                value.SeleccionarUbicacionView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.SeleccionarUbicacionView.Focusable = true;
                        MainWindow.SeleccionarUbicacionView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up seleccionar ubicación", ex);
                    }
                };

                value.AgregarCoparticipeView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarCoparticipeView.Focusable = true;
                        MainWindow.AgregarCoparticipeView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar coparticipe", ex);
                    }
                };
                value.AgregarParametrosView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarParametrosView.Focusable = true;
                        MainWindow.AgregarParametrosView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar coparticipe", ex);
                    }
                };



                value.BuscarView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.BuscarView.Focusable = true;
                        MainWindow.BuscarView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up buscar", ex);
                    }
                };

                value.BuscarExpedienteView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.BuscarExpedienteView.Focusable = true;
                        MainWindow.BuscarExpedienteView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up buscar expediente.", ex);
                    }
                };

                value.IngresarImputadoVisitanteView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.IngresarImputadoVisitanteView.Focusable = true;
                        MainWindow.IngresarImputadoVisitanteView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up ingresar imputado visitante", ex);
                    }
                };

                value.BuscarPersonasExistentes.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.BuscarPersonasExistentes.Focusable = true;
                        MainWindow.BuscarPersonasExistentes.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up buscar personas existentes", ex);
                    }
                };

                value.EmpalmeFechasView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.EmpalmeFechasView.Focusable = true;
                        MainWindow.EmpalmeFechasView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up empalme fechas", ex);
                    }
                };

                value.SeleccionarColorView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.SeleccionarColorView.Focusable = true;
                        MainWindow.SeleccionarColorView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up seleccionar color", ex);
                    }
                };

                value.AgregarIncidenteView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarIncidenteView.Focusable = true;
                        MainWindow.AgregarIncidenteView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar incidente", ex);
                    }
                };

                value.DigitalizarDocumentos.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.DigitalizarDocumentos.Focusable = true;
                        MainWindow.DigitalizarDocumentos.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up digitalizar documentos", ex);
                    }
                };

                value.AgregarSancionesView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.AgregarSancionesView.Focusable = true;
                        MainWindow.AgregarSancionesView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up agregar sanciones", ex);
                    }
                };

                value.CorrespondenciaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.CorrespondenciaView.Focusable = true;
                        MainWindow.CorrespondenciaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up correspondencia", ex);
                    }
                };

                value.ErrorView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.ErrorView.Focusable = true;
                        MainWindow.ErrorView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar mensaje de error", ex);
                    }
                };

                value.EditarFechaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.EditarFechaView.Focusable = true;
                        MainWindow.EditarFechaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up visita", ex);
                    }
                };

                value.EditarIntegrantesGrupoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.EditarIntegrantesGrupoView.Focusable = true;
                        MainWindow.EditarIntegrantesGrupoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up visita", ex);
                    }
                };

                value.MostrarPDFView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.MostrarPDFView.Focusable = true;
                        MainWindow.MostrarPDFView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de visualización de documentos", ex);
                    }
                };

                value.BuscarTrasladosView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;
                        MainWindow.BuscarTrasladosView.Focusable = true;
                        MainWindow.BuscarTrasladosView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de buscar traslados", ex);
                    }
                };

                value.AgregarParticipanteGrupoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;
                        MainWindow.AgregarParticipanteGrupoView.Focusable = true;
                        MainWindow.AgregarParticipanteGrupoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de agregar participante a grupo", ex);
                    }
                };

                value.AgregarFechaParticipanteView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;
                        MainWindow.AgregarFechaParticipanteView.Focusable = true;
                        MainWindow.AgregarFechaParticipanteView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de agregar fecha a participante", ex);
                    }
                };

                value.AgregarParticipanteComplementarioView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;
                        MainWindow.AgregarParticipanteComplementarioView.Focusable = true;
                        MainWindow.AgregarParticipanteComplementarioView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de agregar participante a complementaria", ex);
                    }
                };

                value.AgregarParticipanteComplementarioView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;
                        MainWindow.AgregarParticipanteComplementarioView.Focusable = true;
                        MainWindow.AgregarParticipanteComplementarioView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de agregar participante a complementaria", ex);
                    }
                };

                value.VerSeleccionadosComplementarioView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;
                        MainWindow.VerSeleccionadosComplementarioView.Focusable = true;
                        MainWindow.VerSeleccionadosComplementarioView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de seleccionados grupo complementario", ex);
                    }
                };

                value.BuscarNUCsPorImputadoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;
                        MainWindow.BuscarNUCsPorImputadoView.Focusable = true;
                        MainWindow.BuscarNUCsPorImputadoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de listado documentos", ex);
                    }
                };

                value.BuscarExcarcelacionesView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.BuscarExcarcelacionesView.Focusable = true;
                        MainWindow.BuscarExcarcelacionesView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de busqueda de excarcelaciones", ex);
                    }
                };

                value.AgregarParticipanteSancionesView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.AgregarParticipanteSancionesView.Focusable = true;
                        MainWindow.AgregarParticipanteSancionesView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de busqueda de excarcelaciones", ex);
                    }
                };
                value.BuscarImputadosNUCView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.BuscarImputadosNUCView.Focusable = true;
                        MainWindow.BuscarImputadosNUCView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de busqueda de imputados por NUC", ex);
                    }
                };

                value.AgregarDestinosExcarcelacionView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.AgregarDestinosExcarcelacionView.Focusable = true;
                        MainWindow.AgregarDestinosExcarcelacionView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de agregar destino a excarcelación", ex);
                    }
                };

                value.MotivoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.MotivoView.Focusable = true;
                        MainWindow.MotivoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de motivo", ex);
                    }
                };
                value.AgregarExcarcelacion_Cancelar_MotivoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.AgregarExcarcelacion_Cancelar_MotivoView.Focusable = true;
                        MainWindow.AgregarExcarcelacion_Cancelar_MotivoView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de motivo de cancelación de excarcelación", ex);
                    }
                };
                value.AgregarAgendaMedicaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.AgregarAgendaMedicaView.Focusable = true;
                        MainWindow.AgregarAgendaMedicaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de agregar agenda médica", ex);
                    }
                };
                value.BuscarAgendaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.BuscarAgendaView.Focusable = true;
                        MainWindow.BuscarAgendaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de buscar agenda", ex);
                    }
                };
                value.ExcarcelacionesCancelarView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.ExcarcelacionesCancelarView.Focusable = true;
                        MainWindow.ExcarcelacionesCancelarView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de cancelar excarcelaciones", ex);
                    }
                };
                value.AgregarIncidenciaAtencionCitaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.AgregarIncidenciaAtencionCitaView.Focusable = true;
                        MainWindow.AgregarIncidenciaAtencionCitaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de agregar incidencias para atencion de citas", ex);
                    }
                };
                value.BuscarNotaMedicaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.BuscarNotaMedicaView.Focusable = true;
                        MainWindow.BuscarNotaMedicaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de agregar incidencias para atencion de citas", ex);
                    }
                };

                value.SeleccionarVisitasMultipleView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if (!((bool)e.NewValue))
                            return;

                        MainWindow.SeleccionarVisitasMultipleView.Focusable = true;
                        MainWindow.SeleccionarVisitasMultipleView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de seleccion de visitas", ex);
                    }
                };

                value.BuscarInterconsultaSolicitudView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.BuscarInterconsultaSolicitudView.Focusable = true;
                        MainWindow.BuscarInterconsultaSolicitudView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de buscar solicitudes de interconsulta", ex);
                    }
                };
                value.BuscarCanalizacionesView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.BuscarCanalizacionesView.Focusable = true;
                        MainWindow.BuscarCanalizacionesView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de buscar solicitudes de canalización", ex);
                    }
                };
                value.BuscarInterconsultaMemoriaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.BuscarInterconsultaMemoriaView.Focusable = true;
                        MainWindow.BuscarInterconsultaMemoriaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de buscar solicitudes de canalización", ex);
                    }
                };
                value.NotasMedicasHospitalizacionWindow.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.NotasMedicasHospitalizacionWindow.Focusable = true;
                        MainWindow.NotasMedicasHospitalizacionWindow.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de buscar notas medicas que requieren hospitalización", ex);
                    }
                };
                value.BuscarTVMedicoPendienteView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.BuscarTVMedicoPendienteView.Focusable = true;
                        MainWindow.BuscarTVMedicoPendienteView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de buscar citas medicas pendientes de reagendar", ex);
                    }
                };
                value.ReagendarTV_Cita_MedicaView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        if ((!(bool)e.NewValue))
                            return;
                        MainWindow.ReagendarTV_Cita_MedicaView.Focusable = true;
                        MainWindow.ReagendarTV_Cita_MedicaView.Focus();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de reagendar citas medicas pendientes por traslado", ex);
                    }
                };
                value.RecrearTVInterconsultaView.IsVisibleChanged += (s, e) =>
                    {
                        try
                        {
                            if ((!(bool)e.NewValue))
                                return;
                            MainWindow.RecrearTVInterconsultaView.Focusable = true;
                            MainWindow.RecrearTVInterconsultaView.Focus();
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ocultar/mostrar pop up de recrear atencion de canalización por traslado virtual", ex);
                        }
                    };
            }
        }

        private static bool _EnabledMenu = true;
        public static bool EnabledMenu
        {
            get { return PopUpsViewModels._EnabledMenu; }
            set
            {
                Application.Current.Dispatcher.Invoke((Action)(async delegate
                {
                    if (MainWindow != null)
                    {
                        var PopupOpen = ((System.Windows.Controls.Panel)(((Panel)(((ContentControl)(MainWindow)).Content)).Children[0])).Children.OfType<UserControl>().Where(w => w.Visibility == Visibility.Visible && w.Name.ToLower().EndsWith("view")).ToList();

                        if (!PopupOpen.Any())
                            PopUpsViewModels._EnabledMenu = value;
                        else
                            PopUpsViewModels._EnabledMenu = false;

                        if (((System.Windows.Controls.Panel)(((Panel)(((ContentControl)(MainWindow)).Content)).Children[0])).Children.OfType<UserControl>().Where(w => w.Visibility == Visibility.Visible && (w.Name.ToLower().EndsWith("oscurecerfondo") || w.Name.ToLower().EndsWith("guardandohuellas") || w.Name.ToLower().EndsWith("huellasview"))).Any())
                            PopUpsViewModels._EnabledMenu = value;
                    }
                    else
                        PopUpsViewModels._EnabledMenu = value;
                    await System.Threading.Tasks.TaskEx.Delay(1);
                    RaiseStaticPropertyChanged("EnabledMenu");
                }));
            }
        }


        private static Visibility _VisibleAgregarObservacionesMedicamentosRecetaMedicaView = Visibility.Collapsed;
        public static Visibility VisibleAgregarObservacionesMedicamentosRecetaMedicaView { get { return PopUpsViewModels._VisibleAgregarObservacionesMedicamentosRecetaMedicaView; } }

        private static Visibility _VisibleAgregarActivididadesProdCapacFederalesView = Visibility.Collapsed;
        public static Visibility VisibleAgregarActivididadesProdCapacFederalesView { get { return PopUpsViewModels._VisibleAgregarActivididadesProdCapacFederalesView; } }

        private static Visibility _VisibleSeleccionarVisitasMultipleView = Visibility.Collapsed;
        public static Visibility VisibleSeleccionarVisitasMultipleView { get { return PopUpsViewModels._VisibleSeleccionarVisitasMultipleView; } }

        private static Visibility _VisibleAgregarFechasInicioFinDesarrolloPersonalidadView = Visibility.Collapsed;
        public static Visibility VisibleAgregarFechasInicioFinDesarrolloPersonalidadView { get { return PopUpsViewModels._VisibleAgregarFechasInicioFinDesarrolloPersonalidadView; } }

        private static Visibility _VisibleconcentradoControlLiquidos = Visibility.Collapsed;
        public static Visibility VisibleconcentradoControlLiquidos { get { return PopUpsViewModels._VisibleconcentradoControlLiquidos; } }

        private static Visibility _VisibleSeleccionarVisitanteView = Visibility.Collapsed;
        public static Visibility VisibleSeleccionarVisitanteView { get { return PopUpsViewModels._VisibleSeleccionarVisitanteView; } }

        private static Visibility _VisibleEspecialidadCita = Visibility.Collapsed;
        public static Visibility VisibleEspecialidadCita { get { return PopUpsViewModels._VisibleEspecialidadCita; } }

        private static Visibility _VisibleAgregarInterconsultaMedicaExcarc = Visibility.Collapsed;
        public static Visibility VisibleAgregarInterconsultaMedicaExcarc { get { return PopUpsViewModels._VisibleAgregarInterconsultaMedicaExcarc; } }

        private static Visibility _VisibleAgregarEspecialistaView = Visibility.Collapsed;
        public static Visibility VisibleAgregarEspecialistaView { get { return PopUpsViewModels._VisibleAgregarEspecialistaView; } }

        private static Visibility _VisibleAgregarArchivoResultTratam = Visibility.Collapsed;
        public static Visibility VisibleAgregarArchivoResultTratam { get { return PopUpsViewModels._VisibleAgregarArchivoResultTratam; } }

        private static Visibility _VisibleAgregarImagenesHistoriaClinicaDentalView = Visibility.Collapsed;
        public static Visibility VisibleAgregarImagenesHistoriaClinicaDentalView { get { return PopUpsViewModels._VisibleAgregarImagenesHistoriaClinicaDentalView; } }

        private static Visibility _VisibleDigitalizacionSimpleView = Visibility.Collapsed;
        public static Visibility VisibleDigitalizacionSimpleView { get { return PopUpsViewModels._VisibleDigitalizacionSimpleView; } }

        private static Visibility _VisibleConsultarDocumentosEstudioPersonalidadView = Visibility.Collapsed;
        public static Visibility VisibleConsultarDocumentosEstudioPersonalidadView { get { return PopUpsViewModels._VisibleConsultarDocumentosEstudioPersonalidadView; } }

        private static Visibility _VisibleAgregarDocumentosHistoriaClinicaView = Visibility.Collapsed;
        public static Visibility VisibleAgregarDocumentosHistoriaClinicaView { get { return PopUpsViewModels._VisibleAgregarDocumentosHistoriaClinicaView; } }

        private static Visibility _VisibleSancionDiscFF = Visibility.Collapsed;
        public static Visibility VisibleSancionDiscFF { get { return PopUpsViewModels._VisibleSancionDiscFF; } }

        private static Visibility _VisibleAgregaEducFF = Visibility.Collapsed;
        public static Visibility VisibleAgregaEducFF { get { return PopUpsViewModels._VisibleAgregaEducFF; } }

        private static Visibility _VisibleTomarFotoIfeCedula = Visibility.Collapsed;
        public static Visibility VisibleTomarFotoIfeCedula { get { return PopUpsViewModels._VisibleTomarFotoIfeCedula; } }

        private static Visibility _VisibleAgregarDiasLab = Visibility.Collapsed;
        public static Visibility VisibleAgregarDiasLab { get { return PopUpsViewModels._VisibleAgregarDiasLab; } }

        private static Visibility _VisibleAgregarIntegranteGrupoFamFF = Visibility.Collapsed;
        public static Visibility VisibleAgregarIntegranteGrupoFamFF { get { return PopUpsViewModels._VisibleAgregarIntegranteGrupoFamFF; } }

        private static Visibility _VisibleAgregarOpinion = Visibility.Collapsed;
        public static Visibility VisibleAgregarOpinion { get { return PopUpsViewModels._VisibleAgregarOpinion; } }

        private static Visibility _VisibleAgregarCongreg = Visibility.Collapsed;
        public static Visibility VisibleAgregarCongreg { get { return PopUpsViewModels._VisibleAgregarCongreg; } }

        private static Visibility _VisibleAgregarGrupoPsicologico = Visibility.Collapsed;
        public static Visibility VisibleAgregarGrupoPsicologico { get { return PopUpsViewModels._VisibleAgregarGrupoPsicologico; } }

        private static Visibility _VisibleAgregarActividadLaboral = Visibility.Collapsed;
        public static Visibility VisibleAgregarActividadLaboral { get { return PopUpsViewModels._VisibleAgregarActividadLaboral; } }

        private static Visibility _VisibleAgregarActividCultDepEstudioEducativo = Visibility.Collapsed;
        public static Visibility VisibleAgregarActividCultDepEstudioEducativo { get { return PopUpsViewModels._VisibleAgregarActividCultDepEstudioEducativo; } }

        private static Visibility _VisibleAgregarActividadesEducativasEstudioEducativo = Visibility.Collapsed;
        public static Visibility VisibleAgregarActividadesEducativasEstudioEducativo { get { return PopUpsViewModels._VisibleAgregarActividadesEducativasEstudioEducativo; } }

        private static Visibility _VisibleAgregarEscolaridadEstudioEducativo = Visibility.Collapsed;
        public static Visibility VisibleAgregarEscolaridadEstudioEducativo { get { return PopUpsViewModels._VisibleAgregarEscolaridadEstudioEducativo; } }

        private static Visibility _VisibleAgregarVisitaEdificio = Visibility.Collapsed;
        public static Visibility VisibleAgregarVisitaEdificio { get { return PopUpsViewModels._VisibleAgregarVisitaEdificio; } }

        private static Visibility _VisibleAgregarVisitaApellido = Visibility.Collapsed;
        public static Visibility VisibleAgregarVisitaApellido { get { return PopUpsViewModels._VisibleAgregarVisitaApellido; } }

        private static Visibility _VisibleIngresarVisitanteListaTonta = Visibility.Collapsed;
        public static Visibility VisibleIngresarVisitanteListaTonta { get { return PopUpsViewModels._VisibleIngresarVisitanteListaTonta; } }

        private static Visibility _VisibleBuscarVisitaExistente = Visibility.Collapsed;
        public static Visibility VisibleBuscarVisitaExistente { get { return PopUpsViewModels._VisibleBuscarVisitaExistente; } }

        private static Visibility _VisibleApodo = Visibility.Collapsed;
        public static Visibility VisibleApodo { get { return _VisibleApodo; } }

        private static Visibility _VisibleBuscarConsultasMedicas = Visibility.Collapsed;
        public static Visibility VisibleBuscarConsultasMedicas { get { return _VisibleBuscarConsultasMedicas; } }

        private static Visibility _VisibleTomarFotoSenasParticulares = Visibility.Collapsed;
        public static Visibility VisibleTomarFotoSenasParticulares { get { return _VisibleTomarFotoSenasParticulares; } }

        private static Visibility _VisibleAlias = Visibility.Collapsed;
        public static Visibility VisibleAlias { get { return _VisibleAlias; } }

        private static Visibility _VisibleHuellas = Visibility.Collapsed;
        public static Visibility VisibleHuellas { get { return _VisibleHuellas; } }

        private static Visibility _VisibleAmpliarDescripcion = Visibility.Collapsed;
        public static Visibility VisibleAmpliarDescripcion { get { return _VisibleAmpliarDescripcion; } }

        private static Visibility _VisibleAmpliarDescripcionGenerico = Visibility.Collapsed;
        public static Visibility VisibleAmpliarDescripcionGenerico { get { return _VisibleAmpliarDescripcionGenerico; } }

        private static Visibility _VisibleAgregarDocumento = Visibility.Collapsed;
        public static Visibility VisibleAgregarDocumento { get { return _VisibleAgregarDocumento; } }

        private static Visibility _VisiblePandillaInterno = Visibility.Collapsed;
        public static Visibility VisiblePandillaInterno { get { return _VisiblePandillaInterno; } }

        private static Visibility _VisibleRelacionInterno = Visibility.Collapsed;
        public static Visibility VisibleRelacionInterno { get { return _VisibleRelacionInterno; } }

        private static Visibility _VisibleUltimosEmpleos = Visibility.Collapsed;
        public static Visibility VisibleUltimosEmpleos { get { return _VisibleUltimosEmpleos; } }

        private static Visibility _VisibleDatosGrupoFamiliar = Visibility.Collapsed;
        public static Visibility VisibleDatosGrupoFamiliar { get { return _VisibleDatosGrupoFamiliar; } }

        private static Visibility _VisibleAgregarApoyoEconomico = Visibility.Collapsed;
        public static Visibility VisibleAgregarApoyoEconomico { get { return _VisibleAgregarApoyoEconomico; } }

        private static Visibility _VisibleAgregarFrecuenciaUsoDrogasTS = Visibility.Collapsed;
        public static Visibility VisibleAgregarFrecuenciaUsoDrogasTS { get { return _VisibleAgregarFrecuenciaUsoDrogasTS; } }

        private static Visibility _VisibleNuceloFamiliar = Visibility.Collapsed;
        public static Visibility VisibleNuceloFamiliar { get { return _VisibleNuceloFamiliar; } }

        private static Visibility _VisibleUsoDrogas = Visibility.Collapsed;
        public static Visibility VisibleUsoDrogas { get { return _VisibleUsoDrogas; } }

        private static Visibility _VisibleFamiliarDelito = Visibility.Collapsed;
        public static Visibility VisibleFamiliarDelito { get { return _VisibleFamiliarDelito; } }

        private static Visibility _VisibleFamiliarDroga = Visibility.Collapsed;
        public static Visibility VisibleFamiliarDroga { get { return _VisibleFamiliarDroga; } }

        private static Visibility _VisibleIngresarAcompanante = Visibility.Collapsed;
        public static Visibility VisibleIngresarAcompanante { get { return _VisibleIngresarAcompanante; } }

        private static Visibility _VisibleIngresaImputadoVisitante = Visibility.Collapsed;
        public static Visibility VisibleIngresaImputadoVisitante { get { return _VisibleIngresaImputadoVisitante; } }

        private static Visibility _VisibleBusquedaExpediente = Visibility.Collapsed;
        public static Visibility VisibleBusquedaExpediente
        {
            get { return _VisibleBusquedaExpediente; }
            set
            {
                _VisibleBusquedaExpediente = value;
                RaiseStaticPropertyChanged("VisibleBusquedaExpediente");
            }
        }

        private static Visibility _VisibleBusquedaImputado = Visibility.Collapsed;
        public static Visibility VisibleBusquedaImputado
        {
            get { return _VisibleBusquedaImputado; }
            set
            {
                _VisibleBusquedaImputado = value;
                RaiseStaticPropertyChanged("VisibleBusquedaImputado");
            }
        }

        private static Visibility _VisibleProgramarVisita = Visibility.Collapsed;
        public static Visibility VisibleProgramarVisita { get { return _VisibleProgramarVisita; } }

        private static Visibility _VisibleIngresoAnterior = Visibility.Collapsed;
        public static Visibility VisibleIngresoAnterior { get { return _VisibleIngresoAnterior; } }

        private static Visibility _VisibleAgregarSancion = Visibility.Collapsed;
        public static Visibility VisibleAgregarSancion { get { return _VisibleAgregarSancion; } }

        private static Visibility _VisibleAgregarDictamen = Visibility.Collapsed;
        public static Visibility VisibleAgregarDictamen { get { return _VisibleAgregarDictamen; } }

        private static Visibility _VisibleAgregarGFPV = Visibility.Collapsed;
        public static Visibility VisibleAgregarGFPV { get { return _VisibleAgregarGFPV; } }

        private static Visibility _VisibleAgregarActividad = Visibility.Collapsed;
        public static Visibility VisibleAgregarActividad { get { return _VisibleAgregarActividad; } }

        private static Visibility _VisibleAgregarIAS = Visibility.Collapsed;
        public static Visibility VisibleAgregarIAS { get { return _VisibleAgregarIAS; } }

        private static Visibility _VisibleSeleccionaUbicacion = Visibility.Collapsed;
        public static Visibility VisibleSeleccionaUbicacion { get { return _VisibleSeleccionaUbicacion; } }

        private static Visibility _VisibleSeleccionaUbicacionCelda = Visibility.Collapsed;
        public static Visibility VisibleSeleccionaUbicacionCelda { get { return _VisibleSeleccionaUbicacionCelda; } }

        private static Visibility _VisibleAgregarDelito = Visibility.Collapsed;
        public static Visibility VisibleAgregarDelito { get { return _VisibleAgregarDelito; } }

        private static Visibility _VisibleAgregarCoparticipe = Visibility.Collapsed;
        public static Visibility VisibleAgregarCoparticipe { get { return _VisibleAgregarCoparticipe; } }

        private static Visibility _VisibleAgregarParametro = Visibility.Collapsed;
        public static Visibility VisibleAgregarParametro { get { return _VisibleAgregarParametro; } }

        private static Visibility _VisibleTopografiaHumana = Visibility.Collapsed;
        public static Visibility VisibleTopografiaHumana { get { return _VisibleTopografiaHumana; } }

        private static Visibility _VisibleAgregarDelitoCP = Visibility.Collapsed;
        public static Visibility VisibleAgregarDelitoCP { get { return _VisibleAgregarDelitoCP; } }

        private static Visibility _VisibleAgregarCeldasObservacion = Visibility.Collapsed;
        public static Visibility VisibleAgregarCeldasObservacion { get { return _VisibleAgregarCeldasObservacion; } }

        private static Visibility _VisibleBuscarPersonasExistentes = Visibility.Collapsed;
        public static Visibility VisibleBuscarPersonasExistentes { get { return _VisibleBuscarPersonasExistentes; } }

        private static Visibility _VisibleBuscarPersonasExternas = Visibility.Collapsed;
        public static Visibility VisibleBuscarPersonasExternas { get { return _VisibleBuscarPersonasExternas; } }

        private static Visibility _VisibleAgregarVisitaExterna = Visibility.Collapsed;
        public static Visibility VisibleAgregarVisitaExterna { get { return _VisibleAgregarVisitaExterna; } }

        private static Visibility _VisibleEmpalmeFechas = Visibility.Collapsed;
        public static Visibility VisibleEmpalmeFechas { get { return _VisibleEmpalmeFechas; } }

        private static Visibility _VisibleSeleccionarColor = Visibility.Collapsed;
        public static Visibility VisibleSeleccionarColor { get { return _VisibleSeleccionarColor; } }

        private static Visibility _VisibleAgregarIncidente = Visibility.Collapsed;
        public static Visibility VisibleAgregarIncidente { get { return _VisibleAgregarIncidente; } }

        private static Visibility _VisibleAgregarSancionIncidente = Visibility.Collapsed;
        public static Visibility VisibleAgregarSancionIncidente { get { return _VisibleAgregarSancionIncidente; } }

        private static Visibility _VisibleFondoOscuro = Visibility.Collapsed;
        public static Visibility VisibleFondoOscuro { get { return _VisibleFondoOscuro; } }

        private static Visibility _VisibleDigitalizarDocumento = Visibility.Collapsed;
        public static Visibility VisibleDigitalizarDocumento { get { return _VisibleDigitalizarDocumento; } }

        private static Visibility _VisibleRegistroCorrespondencia = Visibility.Collapsed;
        public static Visibility VisibleRegistroCorrespondencia { get { return _VisibleRegistroCorrespondencia; } }

        private static Visibility _VisibleNotificacion = Visibility.Collapsed;
        public static Visibility VisibleNotificacion { get { return _VisibleNotificacion; } }

        private static Visibility _VisibleBuscarDecomiso = Visibility.Collapsed;
        public static Visibility VisibleBuscarDecomiso { get { return _VisibleBuscarDecomiso; } }

        private static Visibility _VisibleBuscarOficialACargo = Visibility.Collapsed;
        public static Visibility VisibleBuscarOficialACargo { get { return _VisibleBuscarOficialACargo; } }

        private static Visibility _VisibleBuscarInterno = Visibility.Collapsed;
        public static Visibility VisibleBuscarInterno { get { return _VisibleBuscarInterno; } }

        private static Visibility _VisibleBuscarInternoCP = Visibility.Collapsed;
        public static Visibility VisibleBuscarInternoCP { get { return _VisibleBuscarInternoCP; } }

        private static Visibility _VisibleBuscarVisitante = Visibility.Collapsed;
        public static Visibility VisibleBuscarVisitante { get { return _VisibleBuscarVisitante; } }

        private static Visibility _VisibleBuscarEmpleado = Visibility.Collapsed;
        public static Visibility VisibleBuscarEmpleado { get { return _VisibleBuscarEmpleado; } }

        private static Visibility _VisibleBuscarExterno = Visibility.Collapsed;
        public static Visibility VisibleBuscarExterno { get { return _VisibleBuscarExterno; } }

        private static Visibility _VisibleEditarFecha = Visibility.Collapsed;
        public static Visibility VisibleEditarFecha { get { return _VisibleEditarFecha; } }

        private static Visibility _VisibleEditarIntegrantesGrupo = Visibility.Collapsed;
        public static Visibility VisibleEditarIntegrantesGrupo { get { return _VisibleEditarIntegrantesGrupo; } }

        private static Visibility _VisibleProximaCausaPenal = Visibility.Collapsed;
        public static Visibility VisibleProximaCausaPenal { get { return _VisibleProximaCausaPenal; } }

        private static Visibility _VisibleBuscarEvento = Visibility.Collapsed;
        public static Visibility VisibleBuscarEvento { get { return _VisibleBuscarEvento; } }

        private static Visibility _VisibleEventoPrograma = Visibility.Collapsed;
        public static Visibility VisibleEventoPrograma { get { return _VisibleEventoPrograma; } }

        private static Visibility _VisibleEventoPresidium = Visibility.Collapsed;
        public static Visibility VisibleEventoPresidium { get { return _VisibleEventoPresidium; } }

        private static Visibility _VisibleInfTecnica = Visibility.Collapsed;
        public static Visibility VisibleInfTecnica { get { return _VisibleInfTecnica; } }

        private static Visibility _VisibleProceso = Visibility.Collapsed;
        public static Visibility VisibleProceso { get { return _VisibleProceso; } }

        private static Visibility _VisibleRol = Visibility.Collapsed;
        public static Visibility VisibleRol { get { return _VisibleRol; } }

        private static Visibility _VisibleProcesoUsuario = Visibility.Collapsed;
        public static Visibility VisibleProcesoUsuario { get { return _VisibleProcesoUsuario; } }

        private static Visibility _VisibleAgregarUsuario = Visibility.Collapsed;
        public static Visibility VisibleAgregarUsuario { get { return _VisibleAgregarUsuario; } }

        private static Visibility _VisibleImpresionDecomiso = Visibility.Collapsed;
        public static Visibility VisibleImpresionDecomiso { get { return _VisibleImpresionDecomiso; } }

        private static Visibility _VisibleVisualizarDocumento = Visibility.Collapsed;
        public static Visibility VisibleVisualizarDocumento { get { return _VisibleVisualizarDocumento; } }

        private static Visibility _VisibleBuscarTraslados = Visibility.Collapsed;
        public static Visibility VisibleBuscarTraslados { get { return _VisibleBuscarTraslados; } }

        private static Visibility _VisibleAgregarParticipanteGrupo = Visibility.Collapsed;
        public static Visibility VisibleAgregarParticipanteGrupo { get { return _VisibleAgregarParticipanteGrupo; } }

        private static Visibility _VisibleAgregarFechaParticipante = Visibility.Collapsed;
        public static Visibility VisibleAgregarFechaParticipante { get { return _VisibleAgregarFechaParticipante; } }

        private static Visibility _VisibleAgregarParticipanteComplementario = Visibility.Collapsed;
        public static Visibility VisibleAgregarParticipanteComplementario { get { return _VisibleAgregarParticipanteComplementario; } }

        private static Visibility _VisibleAgendarCita = Visibility.Collapsed;
        public static Visibility VisibleAgendarCita { get { return _VisibleAgendarCita; } }

        private static Visibility _VisibleBuscarPersona = Visibility.Collapsed;
        public static Visibility VisibleBuscarPersona { get { return _VisibleBuscarPersona; } }

        private static Visibility _VisibleListaAsistencia = Visibility.Collapsed;
        public static Visibility VisibleListaAsistencia { get { return _VisibleListaAsistencia; } }

        private static Visibility _visibleListaActividadesInterno = Visibility.Collapsed;
        public static Visibility VisibleListaActividadesInterno { get { return _visibleListaActividadesInterno; } }

        private static Visibility _visibleListaAsistenciaInternosEdificio = Visibility.Collapsed;
        public static Visibility VisibleListaAsistenciaInternosEdificio { get { return _visibleListaAsistenciaInternosEdificio; } }

        private static Visibility _VisibleBusquedaInternosProgramas = Visibility.Collapsed;
        public static Visibility VisibleBusquedaInternosProgramas { get { return _VisibleBusquedaInternosProgramas; } }

        private static Visibility _VisibleVerSeleccionadosCompl = Visibility.Collapsed;
        public static Visibility VisibleVerSeleccionadosCompl { get { return _VisibleVerSeleccionadosCompl; } }

        private static Visibility _VisibleBuscarLiberado = Visibility.Collapsed;
        public static Visibility VisibleBuscarLiberado { get { return _VisibleBuscarLiberado; } }

        private static Visibility _VisibleBuscarNUCsPorImputado = Visibility.Collapsed;
        public static Visibility VisibleBuscarNUCsPorImputado { get { return _VisibleBuscarNUCsPorImputado; } }

        private static Visibility _VisibleBuscarExcarcelaciones = Visibility.Collapsed;
        public static Visibility VisibleBuscarExcarcelaciones { get { return _VisibleBuscarExcarcelaciones; } }

        private static Visibility _VisibleAgregarSanciones = Visibility.Collapsed;
        public static Visibility VisibleAgregarSanciones { get { return _VisibleAgregarSanciones; } }

        private static Visibility _VisibleEstudioPersonalidad = Visibility.Collapsed;
        public static Visibility VisibleEstudioPersonalidad { get { return PopUpsViewModels._VisibleEstudioPersonalidad; } }

        private static Visibility _VisibleEstudioPersonalidadDetalle = Visibility.Collapsed;
        public static Visibility VisibleEstudioPersonalidadDetalle { get { return PopUpsViewModels._VisibleEstudioPersonalidadDetalle; } }

        private static Visibility _VisibleBuscarImputadosNUC = Visibility.Collapsed;
        public static Visibility VisibleBuscarImputadosNUC { get { return _VisibleBuscarImputadosNUC; } }

        private static Visibility _VisibleAgregarDestinosExcarcelacion = Visibility.Collapsed;
        public static Visibility VisibleAgregarDestinosExcarcelacion { get { return _VisibleAgregarDestinosExcarcelacion; } }

        private static Visibility _VisibleMotivo = Visibility.Collapsed;
        public static Visibility VisibleMotivo { get { return _VisibleMotivo; } }

        private static Visibility _VisibleSolicitudCita = Visibility.Collapsed;
        public static Visibility VisibleSolicitudCita { get { return _VisibleSolicitudCita; } }

        private static Visibility _VisibleExcarcelacion_Motivo = Visibility.Collapsed;
        public static Visibility VisibleExcarcelacion_Motivo { get { return _VisibleExcarcelacion_Motivo; } }

        private static Visibility _VisibleBuscarDecomisoEvento = Visibility.Collapsed;
        public static Visibility VisibleBuscarDecomisoEvento { get { return _VisibleBuscarDecomisoEvento; } }

        private static Visibility _VisibleAgregarAgendaMedica = Visibility.Collapsed;
        public static Visibility VisibleAgregarAgendaMedica { get { return _VisibleAgregarAgendaMedica; } }

        private static Visibility _VisibleHistoricoTratamiento = Visibility.Collapsed;
        public static Visibility VisibleHistoricoTratamiento { get { return _VisibleHistoricoTratamiento; } }

        private static Visibility _VisibleBuscarAgenda = Visibility.Collapsed;
        public static Visibility VisibleBuscarAgenda { get { return _VisibleBuscarAgenda; } }

        private static Visibility _VisibleExcarcelacionesCancelar = Visibility.Collapsed;
        public static Visibility VisibleExcarcelacionesCancelar { get { return _VisibleExcarcelacionesCancelar; } }

        private static Visibility _VisibleAgregarIncidenciaAtencionCita = Visibility.Collapsed;
        public static Visibility VisibleAgregarIncidenciaAtencionCita { get { return _VisibleAgregarIncidenciaAtencionCita; } }

        private static Visibility _VisibleBuscarNotaMedica = Visibility.Collapsed;
        public static Visibility VisibleBuscarNotaMedica { get { return _VisibleBuscarNotaMedica; } }

        private static Visibility _VisibleSeleccionarReporteSancion = Visibility.Collapsed;
        public static Visibility VisibleSeleccionarReporteSancion { get { return _VisibleSeleccionarReporteSancion; } }

        private static Visibility _VisibleReporteFiltroCausaPenal = Visibility.Collapsed;
        public static Visibility VisibleReporteFiltroCausaPenal { get { return _VisibleReporteFiltroCausaPenal; } }

        private static Visibility _VisibleBuscarInterconsultaSolicitud = Visibility.Collapsed;
        public static Visibility VisibleBuscarInterconsultaSolicitud { get { return _VisibleBuscarInterconsultaSolicitud; } }

        private static Visibility _VisibleAgregarProcesoLibertad = Visibility.Collapsed;
        public static Visibility VisibleAgregarProcesoLibertad { get { return _VisibleAgregarProcesoLibertad; } }

        private static Visibility _VisibleAgregarMedida = Visibility.Collapsed;
        public static Visibility VisibleAgregarMedida { get { return _VisibleAgregarMedida; } }

        private static Visibility _VisibleAgregarMedidaEstatus = Visibility.Collapsed;
        public static Visibility VisibleAgregarMedidaEstatus { get { return _VisibleAgregarMedidaEstatus; } }

        private static Visibility _VisibleAgregarPersona = Visibility.Collapsed;
        public static Visibility VisibleAgregarPersona { get { return _VisibleAgregarPersona; } }

        private static Visibility _VisibleBuscarCanalizaciones = Visibility.Collapsed;
        public static Visibility VisibleBuscarCanalizaciones { get { return _VisibleBuscarCanalizaciones; } }

        private static Visibility _VisibleAgregarLugar = Visibility.Collapsed;
        public static Visibility VisibleAgregarLugar { get { return _VisibleAgregarLugar; } }

        private static Visibility _VisibleAgregarMedidaPresentacion = Visibility.Collapsed;
        public static Visibility VisibleAgregarMedidaPresentacion { get { return _VisibleAgregarMedidaPresentacion; } }

        private static Visibility _VisibleAgregarMedidaDocumento = Visibility.Collapsed;
        public static Visibility VisibleAgregarMedidaDocumento { get { return _VisibleAgregarMedidaDocumento; } }

        private static Visibility _VisibleBuscarInterconsultaMemoria = Visibility.Collapsed;
        public static Visibility VisibleBuscarInterconsultaMemoria { get { return _VisibleBuscarInterconsultaMemoria; } }

        private static Visibility _VisibleVerMedidaEstatus = Visibility.Collapsed;
        public static Visibility VisibleVerMedidaEstatus { get { return _VisibleVerMedidaEstatus; } }

        private static Visibility _VisibleVerMedidaPersona = Visibility.Collapsed;
        public static Visibility VisibleVerMedidaPersona { get { return _VisibleVerMedidaPersona; } }

        private static Visibility _VisibleVerMedidaPresentacion = Visibility.Collapsed;
        public static Visibility VisibleVerMedidaPresentacion { get { return _VisibleVerMedidaPresentacion; } }

        private static Visibility _VisibleNotaMedicaHospitalizacion = Visibility.Collapsed;
        public static Visibility VisibleNotaMedicaHospitalizacion { get { return PopUpsViewModels._VisibleNotaMedicaHospitalizacion; } }

        private static Visibility _VisibleVerMedidaLugares = Visibility.Collapsed;
        public static Visibility VisibleVerMedidaLugares { get { return PopUpsViewModels._VisibleVerMedidaLugares; } }

        private static Visibility _VisibleAgregarProcesoLibertadSeguimiento = Visibility.Collapsed;
        public static Visibility VisibleAgregarProcesoLibertadSeguimiento { get { return PopUpsViewModels._VisibleAgregarProcesoLibertadSeguimiento; } }

        private static Visibility _VisibleBuscarEscalaRiesgo = Visibility.Collapsed;
        public static Visibility VisibleBuscarEscalaRiesgo { get { return PopUpsViewModels._VisibleBuscarEscalaRiesgo; } }

        private static Visibility _VisibleActividadPrograma = Visibility.Collapsed;
        public static Visibility VisibleActividadPrograma { get { return PopUpsViewModels._VisibleActividadPrograma; } }

        private static Visibility _VisibleAgendaLibertad = Visibility.Collapsed;
        public static Visibility VisibleAgendaLibertad { get { return PopUpsViewModels._VisibleAgendaLibertad; } }

        private static Visibility _VisibleOficioAsignacionNSJP = Visibility.Collapsed;
        public static Visibility VisibleOficioAsignacionNSJP { get { return PopUpsViewModels._VisibleOficioAsignacionNSJP; } }

        private static Visibility _VisibleOficioAsignacionTradicional = Visibility.Collapsed;
        public static Visibility VisibleOficioAsignacionTradicional { get { return PopUpsViewModels._VisibleOficioAsignacionTradicional; } }

        private static Visibility _VisibleOficioConclusion = Visibility.Collapsed;
        public static Visibility VisibleOficioConclusion { get { return PopUpsViewModels._VisibleOficioConclusion; } }

        private static Visibility _VisibleOficioBaja = Visibility.Collapsed;
        public static Visibility VisibleOficioBaja { get { return PopUpsViewModels._VisibleOficioBaja; } }

        private static Visibility _VisibleBuscarCausaPenal = Visibility.Collapsed;
        public static Visibility VisibleBuscarCausaPenal { get { return PopUpsViewModels._VisibleBuscarCausaPenal; } }

        private static Visibility _VisibleBuscarTVMedico = Visibility.Collapsed;
        public static Visibility VisibleBuscarTVMedico { get { return PopUpsViewModels._VisibleBuscarTVMedico; } }

        private static Visibility _VisibleReagendarTV_Cita_Medica = Visibility.Collapsed;
        public static Visibility VisibleReagendarTV_Cita_Medica { get { return PopUpsViewModels._VisibleReagendarTV_Cita_Medica; } }

        private static Visibility _VisibleRecrearTVInterconsultaView = Visibility.Collapsed;
        public static Visibility VisibleRecrearTVInterconsultaView { get { return PopUpsViewModels._VisibleRecrearTVInterconsultaView; } }

        private static Visibility _VisibleCapturarNumeroOficioPersonalidadView = Visibility.Collapsed;
        public static Visibility VisibleCapturarNumeroOficioPersonalidadView { get { return PopUpsViewModels._VisibleCapturarNumeroOficioPersonalidadView; } }
        
        #endregion

        #region [Metodos]
        public static void ShowPopUp(object ViewModel, TipoPopUp PopUpToShow)
        {
            try
            {
                if (ViewModel != _PopUpDataContext)
                    _PopUpDataContext = null;

                _PopUpDataContext = ViewModel;
                RaiseStaticPropertyChanged("PopUpDataContext");

                switch (PopUpToShow)
                { 
                    case TipoPopUp.DEFINIR_FECHAS_DESARROLLO_PERSONALIDAD:
                        _VisibleAgregarFechasInicioFinDesarrolloPersonalidadView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarFechasInicioFinDesarrolloPersonalidadView");
                        break;
                    case TipoPopUp.CAPTURA_OFICIO_PERSONALIDAD:
                        _VisibleCapturarNumeroOficioPersonalidadView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleCapturarNumeroOficioPersonalidadView");
                        break;
                    case TipoPopUp.CURSOS_CAPACITACION_FEDERALES:
                        _VisibleAgregarActivididadesProdCapacFederalesView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarActivididadesProdCapacFederalesView");
                        break;
                    case TipoPopUp.AGREGAR_OBSERVACIONES_MEDICAMENTOS_RECETA_MEDICA:
                        _VisibleAgregarObservacionesMedicamentosRecetaMedicaView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarObservacionesMedicamentosRecetaMedicaView");
                        break;
                    case TipoPopUp.AGREGAR_DOCUMENTOS_RESULT_TRATAMIENTO:
                        _VisibleAgregarArchivoResultTratam = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarArchivoResultTratam");
                        break;
                    case TipoPopUp.AGREGAR_CONCENTRADO_HOJA_LIQUIDOS:
                        _VisibleconcentradoControlLiquidos = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleconcentradoControlLiquidos");
                        break;
                    case TipoPopUp.SELECCIONAR_VISITANTES_PERMITIDOS_MULTIPLE:
                        _VisibleSeleccionarVisitasMultipleView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleSeleccionarVisitasMultipleView");
                        break;
                    case TipoPopUp.SELECCIONAR_VISITANTES:
                        _VisibleSeleccionarVisitanteView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleSeleccionarVisitanteView");
                        break;
                    case TipoPopUp.AGREGAR_CITA_ESPECIALISTA:
                        _VisibleEspecialidadCita = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleEspecialidadCita");
                        break;
                    case TipoPopUp.AGREGAR_ESPECIALISTAS:
                        _VisibleAgregarEspecialistaView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarEspecialistaView");
                        break;
                    case TipoPopUp.DIGITALIZACION_SIMPLE:
                        _VisibleDigitalizacionSimpleView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleDigitalizacionSimpleView");
                        break;
                    case TipoPopUp.SELECCIONAR_INTERCONSULTA_MEDICA_EXCARCELACION:
                        _VisibleAgregarInterconsultaMedicaExcarc = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarInterconsultaMedicaExcarc");
                        break;
                    case TipoPopUp.AGREGAR_IMAGENES_HISTORIA_CLINICA_DENTAL:
                        _VisibleAgregarImagenesHistoriaClinicaDentalView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarImagenesHistoriaClinicaDentalView");
                        break;
                    case TipoPopUp.VISUALIZAR_LISTA_DOCUMENTOS:
                        _VisibleConsultarDocumentosEstudioPersonalidadView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleConsultarDocumentosEstudioPersonalidadView");
                        break;
                    case TipoPopUp.AGREGAR_DOCUMENTOS_HISTORIA_CLINICA:
                        _VisibleAgregarDocumentosHistoriaClinicaView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarDocumentosHistoriaClinicaView");
                        break;
                    case TipoPopUp.SANCIONES_INFORME_VIGILANCIA_FUERO_FEDERAL:
                        _VisibleSancionDiscFF = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleSancionDiscFF");
                        break;
                    case TipoPopUp.AGREGAR_EDUC_CUL_FUERO_FEDERAL:
                        _VisibleAgregaEducFF = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregaEducFF");
                        break;
                    case TipoPopUp.AGREGAR_DIAS_LABORADOS_FUERO_FEDERAL:
                        _VisibleAgregarDiasLab = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarDiasLab");
                        break;
                    case TipoPopUp.TOMAR_FOTO_IFE_CEDULA:
                        _VisibleTomarFotoIfeCedula = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleTomarFotoIfeCedula");
                        break;
                    case TipoPopUp.AGREGAR_INTEGRANTE_GRUPO_FAMILIAR_TRABAJO_SOCIAL_FUERO_FEDERAL:
                        _VisibleAgregarIntegranteGrupoFamFF = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarIntegranteGrupoFamFF");
                        break;
                    case TipoPopUp.AGREGAR_OPINION_AREA_TECNICA:
                        _VisibleAgregarOpinion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarOpinion");
                        break;
                    case TipoPopUp.AGREGAR_CONGREGACIONES_SOCIFAM_FC:
                        _VisibleAgregarCongreg = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarCongreg");
                        break;
                    case TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN:
                        _VisibleAgregarGrupoPsicologico = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarGrupoPsicologico");
                        break;
                    case TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN:
                        _VisibleAgregarActividadLaboral = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarActividadLaboral");
                        break;
                    case TipoPopUp.ACTIVIDADES_EDUCATIVAS_EDUCATIVO_FUERO_COMUN:
                        _VisibleAgregarActividadesEducativasEstudioEducativo = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarActividadesEducativasEstudioEducativo");
                        break;
                    case TipoPopUp.ACTIVIDADES_CULTURALES_DEPORTIVAS_EDUCATIVO_FUERO_COMUN:
                        _VisibleAgregarActividCultDepEstudioEducativo = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarActividCultDepEstudioEducativo");
                        break;
                    case TipoPopUp.ESCOLARIDAD_ESTUDIO_EDUCATIVO_FUERO_COMUN:
                        _VisibleAgregarEscolaridadEstudioEducativo = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarEscolaridadEstudioEducativo");
                        break;
                    case TipoPopUp.AGREGAR_VISITA_EDIFICIO:
                        _VisibleAgregarVisitaEdificio = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarVisitaEdificio");
                        break;

                    case TipoPopUp.AGREGAR_USO_DROGA_TS:
                        _VisibleAgregarFrecuenciaUsoDrogasTS = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarFrecuenciaUsoDrogasTS");
                        break;
                    case TipoPopUp.AGREGAR_VISITA_APELLIDO:
                        _VisibleAgregarVisitaApellido = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarVisitaApellido");
                        break;
                    case TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD:
                        _VisibleEstudioPersonalidad = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleEstudioPersonalidad");
                        break;
                    case TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD_DETALLE:
                        _VisibleEstudioPersonalidadDetalle = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleEstudioPersonalidadDetalle");
                        break;
                    case TipoPopUp.INGRESAR_LISTA_TONTA:
                        _VisibleIngresarVisitanteListaTonta = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleIngresarVisitanteListaTonta");
                        break;
                    case TipoPopUp.APODO:
                        _VisibleApodo = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleApodo");
                        break;
                    case TipoPopUp.BUSCAR_CONSULTAS_MEDICAS:
                        _VisibleBuscarConsultasMedicas = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarConsultasMedicas");
                        break;
                    case TipoPopUp.FOTOSSENIASPARTICULAES:
                        _VisibleTomarFotoSenasParticulares = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleTomarFotoSenasParticulares");
                        break;
                    case TipoPopUp.ALIAS:
                        _VisibleAlias = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAlias");
                        break;
                    case TipoPopUp.HUELLAS:
                        _VisibleHuellas = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleHuellas");
                        break;
                    case TipoPopUp.BUSCAR_VISITA_EXISTENTE:
                        _VisibleBuscarVisitaExistente = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarVisitaExistente");
                        break;
                    case TipoPopUp.BUSCAR_PERSONAS_EXTERNAS:
                        _VisibleBuscarPersonasExternas = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarPersonasExternas");
                        break;
                    case TipoPopUp.BUSCAR_PERSONAS_EXISTENTES:
                        _VisibleBuscarPersonasExistentes = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarPersonasExistentes");
                        break;
                    case TipoPopUp.AGREGAR_VISITA_EXTERNA:
                        _VisibleAgregarVisitaExterna = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarVisitaExterna");
                        break;
                    case TipoPopUp.AGREGAR_APOYO_ECONOMICO:
                        _VisibleAgregarApoyoEconomico = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarApoyoEconomico");
                        break;
                    case TipoPopUp.AGREGAR_NUCELO_FAMILIAR:
                        _VisibleNuceloFamiliar = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleNuceloFamiliar");
                        break;
                    case TipoPopUp.AMPLIAR_DESCRIPCION:
                        _VisibleAmpliarDescripcion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAmpliarDescripcion");
                        break;
                    case TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO:
                        _VisibleAmpliarDescripcionGenerico = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAmpliarDescripcionGenerico");
                        break;
                    case TipoPopUp.AGREGAR_DOCUMENTO:
                        _VisibleAgregarDocumento = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarDocumento");
                        break;
                    case TipoPopUp.PANDILLA:
                        _VisiblePandillaInterno = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisiblePandillaInterno");
                        break;
                    case TipoPopUp.RELACION_INTERNO:
                        _VisibleRelacionInterno = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleRelacionInterno");
                        break;
                    case TipoPopUp.ULTIMOS_EMPLEOS:
                        _VisibleUltimosEmpleos = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleUltimosEmpleos");
                        break;
                    case TipoPopUp.DATOS_GRUPO_FAMILIAR:
                        _VisibleDatosGrupoFamiliar = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleDatosGrupoFamiliar");
                        break;
                    case TipoPopUp.USO_DROGAS:
                        _VisibleUsoDrogas = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleUsoDrogas");
                        break;
                    case TipoPopUp.FAMILIAR_DELITO:
                        _VisibleFamiliarDelito = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleFamiliarDelito");
                        break;
                    case TipoPopUp.FAMILIAR_DROGA:
                        _VisibleFamiliarDroga = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleFamiliarDroga");
                        break;
                    case TipoPopUp.INGRESA_ACOMPANIANTE:
                        _VisibleIngresarAcompanante = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleIngresarAcompanante");
                        break;
                    case TipoPopUp.INGRESA_IMPUTADO_VISITANTE:
                        _VisibleIngresaImputadoVisitante = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleIngresaImputadoVisitante");
                        break;
                    case TipoPopUp.BUSQUEDA_EXPEDIENTE:
                        _VisibleBusquedaExpediente = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBusquedaExpediente");
                        break;
                    case TipoPopUp.BUSQUEDA:
                        VisibleBusquedaImputado = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBusquedaImputado");
                        break;
                    case TipoPopUp.PROGRAMAR_VISITA:
                        _VisibleProgramarVisita = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleProgramarVisita");
                        break;
                    case TipoPopUp.INGRESO_ANTERIOR:
                        _VisibleIngresoAnterior = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleIngresoAnterior");
                        break;
                    case TipoPopUp.SANCION_DISCIPLINARIA:
                        _VisibleAgregarSancion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarSancion");
                        break;
                    case TipoPopUp.AGREGAR_DICTAMEN:
                        _VisibleAgregarDictamen = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarDictamen");
                        break;
                    case TipoPopUp.AGREGAR_GFPV:
                        _VisibleAgregarGFPV = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarGFPV");
                        break;
                    case TipoPopUp.AGREGAR_ACTIVIDAD:
                        _VisibleAgregarActividad = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarActividad");
                        break;
                    case TipoPopUp.AGREGAR_IAS:
                        _VisibleAgregarIAS = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarIAS");
                        break;
                    case TipoPopUp.SELECCIONA_UBICACION:
                        _VisibleSeleccionaUbicacion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleSeleccionaUbicacion");
                        break;
                    case TipoPopUp.SELECCIONA_UBICACION_CELDA:
                        _VisibleSeleccionaUbicacionCelda = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleSeleccionaUbicacionCelda");
                        break;
                    case TipoPopUp.AGREGAR_DELITO:
                        _VisibleAgregarDelito = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarDelito");
                        break;
                    case TipoPopUp.AGREGAR_COPARTICIPE:
                        _VisibleAgregarCoparticipe = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarCoparticipe");
                        break;
                    case TipoPopUp.AGREGAR_PARAMETROS:
                        _VisibleAgregarParametro = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarParametro");
                        break;

                    case TipoPopUp.TOPOGRAFIA_HUMANA:
                        _VisibleTopografiaHumana = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleTopografiaHumana");
                        break;
                    case TipoPopUp.AGREGAR_DELITO_CP:
                        _VisibleAgregarDelitoCP = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarDelitoCP");
                        break;
                    case TipoPopUp.AGREGAR_CELDAS_OBSERVACION:
                        _VisibleAgregarCeldasObservacion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarCeldasObservacion");
                        break;
                    case TipoPopUp.EMPALME_FECHAS:
                        _VisibleEmpalmeFechas = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleEmpalmeFechas");
                        break;
                    case TipoPopUp.SELECCIONA_COLOR:
                        _VisibleSeleccionarColor = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleSeleccionarColor");
                        break;
                    case TipoPopUp.AGREGAR_INCIDENTE:
                        _VisibleAgregarIncidente = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarIncidente");
                        break;
                    case TipoPopUp.AGREGAR_SANCION:
                        _VisibleAgregarSancionIncidente = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarSancionIncidente");
                        break;
                    case TipoPopUp.OSCURECER_FONDO:
                        _VisibleFondoOscuro = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleFondoOscuro");
                        break;
                    case TipoPopUp.DIGITALIZAR_DOCUMENTO:
                        _VisibleDigitalizarDocumento = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleDigitalizarDocumento");
                        break;
                    case TipoPopUp.REGISTRO_CORRESPONDENCIA:
                        _VisibleRegistroCorrespondencia = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleRegistroCorrespondencia");
                        break;
                    case TipoPopUp.NOTIFICACION:
                        _VisibleNotificacion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleNotificacion");
                        break;
                    case TipoPopUp.BUSCAR_DECOMISO:
                        _VisibleBuscarDecomiso = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarDecomiso");
                        break;
                    case TipoPopUp.BUSCAR_OFICIALACARGO:
                        _VisibleBuscarOficialACargo = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarOficialACargo");
                        break;
                    case TipoPopUp.BUSCAR_INTERNO:
                        _VisibleBuscarInterno = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarInterno");
                        break;
                    case TipoPopUp.BUSCAR_INTERNO_CP:
                        _VisibleBuscarInternoCP = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarInternoCP");
                        break;
                    case TipoPopUp.BUSCAR_VISITANTE:
                        _VisibleBuscarVisitante = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarVisitante");
                        break;
                    case TipoPopUp.BUSCAR_EMPLEADO:
                        _VisibleBuscarEmpleado = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarEmpleado");
                        break;
                    case TipoPopUp.BUSCAR_EXTERNO:
                        _VisibleBuscarExterno = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarExterno");
                        break;
                    case TipoPopUp.EDITAR_FECHA:
                        _VisibleEditarFecha = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleEditarFecha");
                        break;
                    case TipoPopUp.EDITAR_INTEGRANTES_GRUPO:
                        _VisibleEditarIntegrantesGrupo = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleEditarIntegrantesGrupo");
                        break;
                    case TipoPopUp.PROXIMA_CAUSA_PENAL:
                        _VisibleProximaCausaPenal = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleProximaCausaPenal");
                        break;
                    case TipoPopUp.BUSCAR_EVENTO:
                        _VisibleBuscarEvento = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarEvento");
                        break;
                    case TipoPopUp.AGREGAR_EVENTO_PROGRAMA:
                        _VisibleEventoPrograma = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleEventoPrograma");
                        break;
                    case TipoPopUp.AGREGAR_EVENTO_PRESIDIUM:
                        _VisibleEventoPresidium = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleEventoPresidium");
                        break;
                    case TipoPopUp.AGREGAR_EVENTO_INF_TECNICA:
                        _VisibleInfTecnica = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleInfTecnica");
                        break;
                    case TipoPopUp.AGREGAR_PROCESO:
                        _VisibleProceso = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleProceso");
                        break;
                    case TipoPopUp.AGREGAR_ROL:
                        _VisibleRol = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleRol");
                        break;
                    case TipoPopUp.AGREGAR_PROCESO_USUARIO:
                        _VisibleProcesoUsuario = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleProcesoUsuario");
                        break;
                    case TipoPopUp.AGREGAR_USUARIO:
                        _VisibleAgregarUsuario = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarUsuario");
                        break;
                    case TipoPopUp.IMPRESION_DECOMISO:
                        _VisibleImpresionDecomiso = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleImpresionDecomiso");
                        break;
                    case TipoPopUp.VISUALIZAR_DOCUMENTOS:
                        _VisibleVisualizarDocumento = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleVisualizarDocumento");
                        break;
                    case TipoPopUp.BUSCAR_TRASLADOS:
                        _VisibleBuscarTraslados = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarTraslados");
                        break;
                    case TipoPopUp.AGREGAR_PARTICIPANTE_GRUPO:
                        _VisibleAgregarParticipanteGrupo = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarParticipanteGrupo");
                        break;
                    case TipoPopUp.AGREGAR_FECHA_PARTICIPANTE:
                        _VisibleAgregarFechaParticipante = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarFechaParticipante");
                        break;
                    case TipoPopUp.AGREGAR_PARTICIPANTE_COMPLEMENTARIO:
                        _VisibleAgregarParticipanteComplementario = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarParticipanteComplementario");
                        break;
                    case TipoPopUp.AGENDAR_CITA:
                        _VisibleAgendarCita = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgendarCita");
                        break;
                    case TipoPopUp.BUSCAR_PERSONA:
                        _VisibleBuscarPersona = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarPersona");
                        break;
                    case TipoPopUp.BUSQUEDA_INTERNOS_PROGRAMAS:
                        _VisibleBusquedaInternosProgramas = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBusquedaInternosProgramas");
                        break;
                    case TipoPopUp.LISTA_ASISTENCIA:
                        _VisibleListaAsistencia = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleListaAsistencia");
                        break;
                    case TipoPopUp.LISTA_ACTIVIDADES_INTERNO:
                        _visibleListaActividadesInterno = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleListaActividadesInterno");
                        break;
                    case TipoPopUp.LISTA_ASISTENCIA_INTERNO_EDIFICIO:
                        _visibleListaAsistenciaInternosEdificio = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleListaAsistenciaInternosEdificio");
                        break;
                    case TipoPopUp.VER_SELECCIONADOSCOMPL:
                        _VisibleVerSeleccionadosCompl = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleVerSeleccionadosCompl");
                        break;
                    case TipoPopUp.BUSCAR_LIBERADO:
                        _VisibleBuscarLiberado = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarLiberado");
                        break;
                    case TipoPopUp.BUSCAR_NUCS_IMPUTADO:
                        _VisibleBuscarNUCsPorImputado = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarNUCsPorImputado");
                        break;
                    case TipoPopUp.BUSCAR_EXCARCELACIONES:
                        _VisibleBuscarExcarcelaciones = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarExcarcelaciones");
                        break;
                    case TipoPopUp.AGREGAR_CANCELAR_SUSPENDER:
                        _VisibleAgregarSanciones = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarSanciones");
                        break;
                    case TipoPopUp.BUSCAR_IMPUTADO_NUC:
                        _VisibleBuscarImputadosNUC = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarImputadosNUC");
                        break;
                    case TipoPopUp.AGREGAR_DESTINO_EXCARCELACION:
                        _VisibleAgregarDestinosExcarcelacion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarDestinosExcarcelacion");
                        break;
                    case TipoPopUp.MOTIVO:
                        _VisibleMotivo = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleMotivo");
                        break;
                    case TipoPopUp.SOLICITUD_CITA:
                        _VisibleSolicitudCita = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleSolicitudCita");
                        break;
                    case TipoPopUp.EXCARCELACION_CANCELACION_MOTIVO:
                        _VisibleExcarcelacion_Motivo = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleExcarcelacion_Motivo");
                        break;
                    case TipoPopUp.BUSCAR_DECOMISO_EVENTO:
                        _VisibleBuscarDecomisoEvento = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarDecomisoEvento");
                        break;
                    case TipoPopUp.AGREGAR_AGENDA_MEDICA:
                        _VisibleAgregarAgendaMedica = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarAgendaMedica");
                        break;
                    case TipoPopUp.HISTORICO_TRATAMIENTO:
                        _VisibleHistoricoTratamiento = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleHistoricoTratamiento");
                        break;
                    case TipoPopUp.BUSCAR_AGENDA:
                        _VisibleBuscarAgenda = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarAgenda");
                        break;
                    case TipoPopUp.EXCARCELACIONES_CANCELAR:
                        _VisibleExcarcelacionesCancelar = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleExcarcelacionesCancelar");
                        break;
                    case TipoPopUp.INCIDENCIA_ATENCION_CITA:
                        _VisibleAgregarIncidenciaAtencionCita = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarIncidenciaAtencionCita");
                        break;
                    case TipoPopUp.BUSCAR_NOTA_MEDICA:
                        _VisibleBuscarNotaMedica = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarNotaMedica");
                        break;
                    case TipoPopUp.SELECCIONAR_REPORTE_SANCION:
                        _VisibleSeleccionarReporteSancion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleSeleccionarReporteSancion");
                        break;
                    case TipoPopUp.FILTRO_REPORTE_CAUSAS_PENALES:
                        _VisibleReporteFiltroCausaPenal = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleReporteFiltroCausaPenal");
                        break;
                    case TipoPopUp.BUSCAR_INTERCONSULTA_SOLICITUD:
                        _VisibleBuscarInterconsultaSolicitud = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarInterconsultaSolicitud");
                        break;
                    case TipoPopUp.AGREGAR_PROCESO_LIBERTAD:
                        _VisibleAgregarProcesoLibertad = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarProcesoLibertad");
                        break;
                    case TipoPopUp.AGREGAR_MEDIDA:
                        _VisibleAgregarMedida = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarMedida");
                        break;
                    case TipoPopUp.AGREGAR_MEDIDA_ESTAUS:
                        _VisibleAgregarMedidaEstatus = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarMedidaEstatus");
                        break;
                    case TipoPopUp.AGREGAR_PERSONA:
                        _VisibleAgregarPersona = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarPersona");
                        break;
                    case TipoPopUp.BUSCAR_CANALIZACIONES:
                        _VisibleBuscarCanalizaciones = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarCanalizaciones");
                        break;
                    case TipoPopUp.AGREGAR_LUGAR:
                        _VisibleAgregarLugar = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarLugar");
                        break;
                    case TipoPopUp.AGREGAR_MEDIDA_PRESENTACION:
                        _VisibleAgregarMedidaPresentacion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarMedidaPresentacion");
                        break;
                    case TipoPopUp.AGREGAR_MEDIDA_DOCUMENTO:
                        _VisibleAgregarMedidaDocumento = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarMedidaDocumento");
                        break;
                    case TipoPopUp.BUSCAR_INTERCONSULTA_MEMORIA:
                        _VisibleBuscarInterconsultaMemoria = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarInterconsultaMemoria");
                        break;
                    case TipoPopUp.VER_MEDIDA_ESTATUS:
                        _VisibleVerMedidaEstatus = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleVerMedidaEstatus");
                        break;
                    case TipoPopUp.VER_MEDIDA_PERSONA:
                        _VisibleVerMedidaPersona = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleVerMedidaPersona");
                        break;
                    case TipoPopUp.VER_MEDIDA_PRESENTACION:
                        _VisibleVerMedidaPresentacion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleVerMedidaPresentacion");
                        break;
                    case TipoPopUp.NOTAS_MEDICAS_HOSPITALIZACION:
                        _VisibleNotaMedicaHospitalizacion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleNotaMedicaHospitalizacion");
                        break;
                    case TipoPopUp.VER_MEDIDA_LUGARES:
                        _VisibleVerMedidaLugares = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleVerMedidaLugares");
                        break;
                    case TipoPopUp.AGREGAR_PROCESO_LIBERTAD_SEGUMIENTO:
                        _VisibleAgregarProcesoLibertadSeguimiento = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgregarProcesoLibertadSeguimiento");
                        break;
                    case TipoPopUp.BUSCAR_ESCALA_RIESGO:
                        _VisibleBuscarEscalaRiesgo = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarEscalaRiesgo");
                        break;
                    case TipoPopUp.AGREGAR_ACTIVIDAD_PROGRAMA:
                        _VisibleActividadPrograma = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleActividadPrograma");
                        break;
                    case TipoPopUp.AGENDA_LIBERTAD:
                        _VisibleAgendaLibertad = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleAgendaLibertad");
                        break;
                    case TipoPopUp.OFICIO_ASIGNACION_NSJP:
                        _VisibleOficioAsignacionNSJP = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleOficioAsignacionNSJP");
                        break;
                    case TipoPopUp.OFICIO_ASIGNACION_TRADICIONAL:
                        _VisibleOficioAsignacionTradicional = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleOficioAsignacionTradicional");
                        break;
                    case TipoPopUp.OFICIO_CONCLUSION:
                        _VisibleOficioConclusion = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleOficioConclusion");
                        break;
                    case TipoPopUp.OFICIO_BAJA:
                        _VisibleOficioBaja = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleOficioBaja");
                        break;
                    case TipoPopUp.BUSCAR_CAUSA_PENAL:
                        _VisibleBuscarCausaPenal = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarCausaPenal");
                        break;
                    case TipoPopUp.BUSCAR_TV_MEDICO_PENDIENTE:
                        _VisibleBuscarTVMedico = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBuscarTVMedico");
                        break;
                    case TipoPopUp.REAGENDAR_TV_CITA_MEDICA:
                        _VisibleReagendarTV_Cita_Medica = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleReagendarTV_Cita_Medica");
                        break;
                    case TipoPopUp.RECREAR_TV_INTERCONSULTA:
                        _VisibleRecrearTVInterconsultaView = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleRecrearTVInterconsultaView");
                        break;
                    default:
                        break;
                }

                EnabledMenu = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir popup", ex);
            }
        }

        public static void ClosePopUp(TipoPopUp PopUpToClose = TipoPopUp.CERRAR_TODOS)
        {
            try
            {
                switch (PopUpToClose)
                {
                    case TipoPopUp.DIGITALIZACION_SIMPLE:
                        _VisibleDigitalizacionSimpleView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleDigitalizacionSimpleView");
                        break;
                    case TipoPopUp.CURSOS_CAPACITACION_FEDERALES:
                        _VisibleAgregarActivididadesProdCapacFederalesView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarActivididadesProdCapacFederalesView");
                        break;
                    case TipoPopUp.AGREGAR_OBSERVACIONES_MEDICAMENTOS_RECETA_MEDICA:
                        _VisibleAgregarObservacionesMedicamentosRecetaMedicaView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarObservacionesMedicamentosRecetaMedicaView");
                        break;
                    case TipoPopUp.DEFINIR_FECHAS_DESARROLLO_PERSONALIDAD:
                        _VisibleAgregarFechasInicioFinDesarrolloPersonalidadView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarFechasInicioFinDesarrolloPersonalidadView");
                        break;
                    case TipoPopUp.AGREGAR_CONCENTRADO_HOJA_LIQUIDOS:
                        _VisibleconcentradoControlLiquidos = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleconcentradoControlLiquidos");
                        break;
                    case TipoPopUp.SELECCIONAR_VISITANTES:
                        _VisibleSeleccionarVisitanteView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSeleccionarVisitanteView");
                        break;
                    case TipoPopUp.SELECCIONAR_VISITANTES_PERMITIDOS_MULTIPLE:
                        _VisibleSeleccionarVisitasMultipleView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSeleccionarVisitasMultipleView");
                        break;
                    case TipoPopUp.AGREGAR_CITA_ESPECIALISTA:
                        _VisibleEspecialidadCita = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEspecialidadCita");
                        break;
                    case TipoPopUp.AGREGAR_ESPECIALISTAS:
                        _VisibleAgregarEspecialistaView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarEspecialistaView");
                        break;
                    case TipoPopUp.AGREGAR_DOCUMENTOS_RESULT_TRATAMIENTO:
                        _VisibleAgregarArchivoResultTratam = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarArchivoResultTratam");
                        break;
                    case TipoPopUp.SELECCIONAR_INTERCONSULTA_MEDICA_EXCARCELACION:
                        _VisibleAgregarInterconsultaMedicaExcarc = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarInterconsultaMedicaExcarc");
                        break;
                    case TipoPopUp.AGREGAR_IMAGENES_HISTORIA_CLINICA_DENTAL:
                        _VisibleAgregarImagenesHistoriaClinicaDentalView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarImagenesHistoriaClinicaDentalView");
                        break;
                    case TipoPopUp.VISUALIZAR_LISTA_DOCUMENTOS:
                        _VisibleConsultarDocumentosEstudioPersonalidadView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleConsultarDocumentosEstudioPersonalidadView");
                        break;
                    case TipoPopUp.AGREGAR_EDUC_CUL_FUERO_FEDERAL:
                        _VisibleAgregaEducFF = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregaEducFF");
                        break;
                    case TipoPopUp.AGREGAR_DOCUMENTOS_HISTORIA_CLINICA:
                        _VisibleAgregarDocumentosHistoriaClinicaView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDocumentosHistoriaClinicaView");
                        break;
                    case TipoPopUp.SANCIONES_INFORME_VIGILANCIA_FUERO_FEDERAL:
                        _VisibleSancionDiscFF = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSancionDiscFF");
                        break;
                    case TipoPopUp.AGREGAR_DIAS_LABORADOS_FUERO_FEDERAL:
                        _VisibleAgregarDiasLab = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDiasLab");
                        break;
                    case TipoPopUp.TOMAR_FOTO_IFE_CEDULA:
                        _VisibleTomarFotoIfeCedula = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleTomarFotoIfeCedula");
                        break;
                    case TipoPopUp.AGREGAR_INTEGRANTE_GRUPO_FAMILIAR_TRABAJO_SOCIAL_FUERO_FEDERAL:
                        _VisibleAgregarIntegranteGrupoFamFF = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarIntegranteGrupoFamFF");
                        break;
                    case TipoPopUp.AGREGAR_OPINION_AREA_TECNICA:
                        _VisibleAgregarOpinion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarOpinion");
                        break;
                    case TipoPopUp.AGREGAR_CONGREGACIONES_SOCIFAM_FC:
                        _VisibleAgregarCongreg = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarCongreg");
                        break;
                    case TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN:
                        _VisibleAgregarActividadLaboral = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarActividadLaboral");
                        break;
                    case TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN:
                        _VisibleAgregarGrupoPsicologico = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarGrupoPsicologico");
                        break;
                    case TipoPopUp.ESCOLARIDAD_ESTUDIO_EDUCATIVO_FUERO_COMUN:
                        _VisibleAgregarEscolaridadEstudioEducativo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarEscolaridadEstudioEducativo");
                        break;
                    case TipoPopUp.ACTIVIDADES_CULTURALES_DEPORTIVAS_EDUCATIVO_FUERO_COMUN:
                        _VisibleAgregarActividCultDepEstudioEducativo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarActividCultDepEstudioEducativo");
                        break;
                    case TipoPopUp.ACTIVIDADES_EDUCATIVAS_EDUCATIVO_FUERO_COMUN:
                        _VisibleAgregarActividadesEducativasEstudioEducativo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarActividadesEducativasEstudioEducativo");
                        break;
                    case TipoPopUp.AGREGAR_VISITA_EDIFICIO:
                        _VisibleAgregarVisitaEdificio = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarVisitaEdificio");
                        break;

                    case TipoPopUp.AGREGAR_USO_DROGA_TS:
                        _VisibleAgregarFrecuenciaUsoDrogasTS = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarFrecuenciaUsoDrogasTS");
                        break;
                    case TipoPopUp.AGREGAR_VISITA_APELLIDO:
                        _VisibleAgregarVisitaApellido = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarVisitaApellido");
                        break;
                    case TipoPopUp.INGRESAR_LISTA_TONTA:
                        _VisibleIngresarVisitanteListaTonta = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleIngresarVisitanteListaTonta");
                        break;
                    case TipoPopUp.APODO:
                        _VisibleApodo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleApodo");
                        break;
                    case TipoPopUp.BUSCAR_CONSULTAS_MEDICAS:
                        _VisibleBuscarConsultasMedicas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarConsultasMedicas");
                        break;
                    case TipoPopUp.FOTOSSENIASPARTICULAES:
                        _VisibleTomarFotoSenasParticulares = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleTomarFotoSenasParticulares");
                        break;
                    case TipoPopUp.ALIAS:
                        _VisibleAlias = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAlias");
                        break;
                    case TipoPopUp.HUELLAS:
                        _VisibleHuellas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleHuellas");
                        break;
                    case TipoPopUp.BUSCAR_VISITA_EXISTENTE:
                        _VisibleBuscarVisitaExistente = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarVisitaExistente");
                        break;
                    case TipoPopUp.BUSCAR_PERSONAS_EXTERNAS:
                        _VisibleBuscarPersonasExternas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarPersonasExternas");
                        break;
                    case TipoPopUp.BUSCAR_PERSONAS_EXISTENTES:
                        _VisibleBuscarPersonasExistentes = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarPersonasExistentes");
                        break;
                    case TipoPopUp.AGREGAR_VISITA_EXTERNA:
                        _VisibleAgregarVisitaExterna = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarVisitaExterna");
                        break;
                    case TipoPopUp.AGREGAR_APOYO_ECONOMICO:
                        _VisibleAgregarApoyoEconomico = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarApoyoEconomico");
                        break;
                    case TipoPopUp.AGREGAR_NUCELO_FAMILIAR:
                        _VisibleNuceloFamiliar = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleNuceloFamiliar");
                        break;
                    case TipoPopUp.AGREGAR_DOCUMENTO:
                        _VisibleAgregarDocumento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDocumento");
                        break;
                    case TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD:
                        _VisibleEstudioPersonalidad = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEstudioPersonalidad");
                        break;
                    case TipoPopUp.PANDILLA:
                        _VisiblePandillaInterno = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisiblePandillaInterno");
                        break;
                    case TipoPopUp.RELACION_INTERNO:
                        _VisibleRelacionInterno = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleRelacionInterno");
                        break;
                    case TipoPopUp.ULTIMOS_EMPLEOS:
                        _VisibleUltimosEmpleos = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleUltimosEmpleos");
                        break;
                    case TipoPopUp.DATOS_GRUPO_FAMILIAR:
                        _VisibleDatosGrupoFamiliar = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleDatosGrupoFamiliar");
                        break;
                    case TipoPopUp.AMPLIAR_DESCRIPCION:
                        _VisibleAmpliarDescripcion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAmpliarDescripcion");
                        break;
                    case TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO:
                        _VisibleAmpliarDescripcionGenerico = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAmpliarDescripcionGenerico");
                        break;
                    case TipoPopUp.USO_DROGAS:
                        _VisibleUsoDrogas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleUsoDrogas");
                        break;
                    case TipoPopUp.FAMILIAR_DELITO:
                        _VisibleFamiliarDelito = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleFamiliarDelito");
                        break;
                    case TipoPopUp.FAMILIAR_DROGA:
                        _VisibleFamiliarDroga = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleFamiliarDroga");
                        break;
                    case TipoPopUp.INGRESA_ACOMPANIANTE:
                        _VisibleIngresarAcompanante = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleIngresarAcompanante");
                        break;
                    case TipoPopUp.INGRESA_IMPUTADO_VISITANTE:
                        _VisibleIngresaImputadoVisitante = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleIngresaImputadoVisitante");
                        break;
                    case TipoPopUp.BUSQUEDA:
                        VisibleBusquedaImputado = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBusquedaImputado");
                        break;
                    case TipoPopUp.BUSQUEDA_EXPEDIENTE:
                        _VisibleBusquedaExpediente = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBusquedaExpediente");
                        break;
                    case TipoPopUp.PROGRAMAR_VISITA:
                        _VisibleProgramarVisita = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleProgramarVisita");
                        break;
                    case TipoPopUp.INGRESO_ANTERIOR:
                        _VisibleIngresoAnterior = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleIngresoAnterior");
                        break;
                    case TipoPopUp.SANCION_DISCIPLINARIA:
                        _VisibleAgregarSancion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarSancion");
                        break;
                    case TipoPopUp.AGREGAR_DICTAMEN:
                        _VisibleAgregarDictamen = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDictamen");
                        break;
                    case TipoPopUp.AGREGAR_GFPV:
                        _VisibleAgregarGFPV = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarGFPV");
                        break;
                    case TipoPopUp.AGREGAR_ACTIVIDAD:
                        _VisibleAgregarActividad = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarActividad");
                        break;
                    case TipoPopUp.AGREGAR_IAS:
                        _VisibleAgregarIAS = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarIAS");
                        break;
                    case TipoPopUp.SELECCIONA_UBICACION:
                        _VisibleSeleccionaUbicacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSeleccionaUbicacion");
                        break;
                    case TipoPopUp.SELECCIONA_UBICACION_CELDA:
                        _VisibleSeleccionaUbicacionCelda = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSeleccionaUbicacionCelda");
                        break;
                    case TipoPopUp.AGREGAR_DELITO:
                        _VisibleAgregarDelito = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDelito");
                        break;
                    case TipoPopUp.AGREGAR_COPARTICIPE:
                        _VisibleAgregarCoparticipe = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarCoparticipe");
                        break;
                    case TipoPopUp.AGREGAR_PARAMETROS:
                        _VisibleAgregarParametro = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarParametro");
                        break;
                    case TipoPopUp.TOPOGRAFIA_HUMANA:
                        _VisibleTopografiaHumana = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleTopografiaHumana");
                        break;
                    case TipoPopUp.AGREGAR_DELITO_CP:
                        _VisibleAgregarDelitoCP = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDelitoCP");
                        break;
                    case TipoPopUp.AGREGAR_CELDAS_OBSERVACION:
                        _VisibleAgregarCeldasObservacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarCeldasObservacion");
                        break;
                    case TipoPopUp.EMPALME_FECHAS:
                        _VisibleEmpalmeFechas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEmpalmeFechas");
                        break;
                    case TipoPopUp.SELECCIONA_COLOR:
                        _VisibleSeleccionarColor = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSeleccionarColor");
                        break;
                    case TipoPopUp.AGREGAR_INCIDENTE:
                        _VisibleAgregarIncidente = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarIncidente");
                        break;
                    case TipoPopUp.AGREGAR_SANCION:
                        _VisibleAgregarSancionIncidente = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarSancionIncidente");
                        break;
                    case TipoPopUp.OSCURECER_FONDO:
                        _VisibleFondoOscuro = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleFondoOscuro");
                        break;
                    case TipoPopUp.DIGITALIZAR_DOCUMENTO:
                        _VisibleDigitalizarDocumento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleDigitalizarDocumento");
                        break;
                    case TipoPopUp.REGISTRO_CORRESPONDENCIA:
                        _VisibleRegistroCorrespondencia = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleRegistroCorrespondencia");
                        break;
                    case TipoPopUp.NOTIFICACION:
                        _VisibleNotificacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleNotificacion");
                        break;
                    case TipoPopUp.BUSCAR_DECOMISO:
                        _VisibleBuscarDecomiso = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarDecomiso");
                        break;
                    case TipoPopUp.BUSCAR_OFICIALACARGO:
                        _VisibleBuscarOficialACargo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarOficialACargo");
                        break;
                    case TipoPopUp.BUSCAR_INTERNO:
                        _VisibleBuscarInterno = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarInterno");
                        break;

                    case TipoPopUp.BUSCAR_INTERNO_CP:
                        _VisibleBuscarInternoCP = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarInternoCP");
                        break;

                    case TipoPopUp.BUSCAR_VISITANTE:
                        _VisibleBuscarVisitante = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarVisitante");
                        break;
                    case TipoPopUp.BUSCAR_EMPLEADO:
                        _VisibleBuscarEmpleado = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarEmpleado");
                        break;
                    case TipoPopUp.BUSCAR_EXTERNO:
                        _VisibleBuscarExterno = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarExterno");
                        break;
                    case TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD_DETALLE:
                        _VisibleEstudioPersonalidadDetalle = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEstudioPersonalidadDetalle");
                        break;
                    case TipoPopUp.EDITAR_FECHA:
                        _VisibleEditarFecha = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEditarFecha");
                        break;
                    case TipoPopUp.EDITAR_INTEGRANTES_GRUPO:
                        _VisibleEditarIntegrantesGrupo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEditarIntegrantesGrupo");
                        break;
                    case TipoPopUp.PROXIMA_CAUSA_PENAL:
                        _VisibleProximaCausaPenal = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleProximaCausaPenal");
                        break;
                    case TipoPopUp.BUSCAR_EVENTO:
                        _VisibleBuscarEvento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarEvento");
                        break;
                    case TipoPopUp.AGREGAR_EVENTO_PROGRAMA:
                        _VisibleEventoPrograma = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEventoPrograma");
                        break;
                    case TipoPopUp.AGREGAR_EVENTO_PRESIDIUM:
                        _VisibleEventoPresidium = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEventoPresidium");
                        break;
                    case TipoPopUp.AGREGAR_EVENTO_INF_TECNICA:
                        _VisibleInfTecnica = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleInfTecnica");
                        break;
                    case TipoPopUp.AGREGAR_PROCESO:
                        _VisibleProceso = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleProceso");
                        break;
                    case TipoPopUp.AGREGAR_ROL:
                        _VisibleRol = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleRol");
                        break;
                    case TipoPopUp.AGREGAR_PROCESO_USUARIO:
                        _VisibleProcesoUsuario = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleProcesoUsuario");
                        break;
                    case TipoPopUp.AGREGAR_USUARIO:
                        _VisibleAgregarUsuario = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarUsuario");
                        break;
                    case TipoPopUp.IMPRESION_DECOMISO:
                        _VisibleImpresionDecomiso = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleImpresionDecomiso");
                        break;
                    case TipoPopUp.VISUALIZAR_DOCUMENTOS:
                        _VisibleVisualizarDocumento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVisualizarDocumento");
                        break;
                    case TipoPopUp.BUSCAR_TRASLADOS:
                        _VisibleBuscarTraslados = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarTraslados");
                        break;
                    case TipoPopUp.AGREGAR_PARTICIPANTE_GRUPO:
                        _VisibleAgregarParticipanteGrupo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarParticipanteGrupo");
                        break;
                    case TipoPopUp.AGREGAR_FECHA_PARTICIPANTE:
                        _VisibleAgregarFechaParticipante = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarFechaParticipante");
                        break;
                    case TipoPopUp.AGREGAR_PARTICIPANTE_COMPLEMENTARIO:
                        _VisibleAgregarParticipanteComplementario = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarParticipanteComplementario");
                        break;
                    case TipoPopUp.AGENDAR_CITA:
                        _VisibleAgendarCita = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgendarCita");
                        break;
                    case TipoPopUp.BUSCAR_PERSONA:
                        _VisibleBuscarPersona = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarPersona");
                        break;
                    case TipoPopUp.BUSQUEDA_INTERNOS_PROGRAMAS:
                        _VisibleBusquedaInternosProgramas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBusquedaInternosProgramas");
                        break;
                    case TipoPopUp.LISTA_ASISTENCIA:
                        _VisibleListaAsistencia = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleListaAsistencia");
                        break;
                    case TipoPopUp.LISTA_ACTIVIDADES_INTERNO:
                        _visibleListaActividadesInterno = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleListaActividadesInterno");
                        break;
                    case TipoPopUp.LISTA_ASISTENCIA_INTERNO_EDIFICIO:
                        _visibleListaAsistenciaInternosEdificio = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleListaAsistenciaInternosEdificio");
                        break;
                    case TipoPopUp.VER_SELECCIONADOSCOMPL:
                        _VisibleVerSeleccionadosCompl = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVerSeleccionadosCompl");
                        break;
                    case TipoPopUp.BUSCAR_LIBERADO:
                        _VisibleBuscarLiberado = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarLiberado");
                        break;
                    case TipoPopUp.BUSCAR_NUCS_IMPUTADO:
                        _VisibleBuscarNUCsPorImputado = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarNUCsPorImputado");
                        break;
                    case TipoPopUp.BUSCAR_EXCARCELACIONES:
                        _VisibleBuscarExcarcelaciones = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarExcarcelaciones");
                        break;
                    case TipoPopUp.AGREGAR_CANCELAR_SUSPENDER:
                        _VisibleAgregarSanciones = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarSanciones");
                        break;
                    case TipoPopUp.BUSCAR_IMPUTADO_NUC:
                        _VisibleBuscarImputadosNUC = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarImputadosNUC");
                        break;
                    case TipoPopUp.AGREGAR_DESTINO_EXCARCELACION:
                        _VisibleAgregarDestinosExcarcelacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDestinosExcarcelacion");
                        break;
                    case TipoPopUp.MOTIVO:
                        _VisibleMotivo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleMotivo");
                        break;
                    case TipoPopUp.SOLICITUD_CITA:
                        _VisibleSolicitudCita = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSolicitudCita");
                        break;
                    case TipoPopUp.EXCARCELACION_CANCELACION_MOTIVO:
                        _VisibleExcarcelacion_Motivo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleExcarcelacion_Motivo");
                        break;
                    case TipoPopUp.BUSCAR_DECOMISO_EVENTO:
                        _VisibleBuscarDecomisoEvento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarDecomisoEvento");
                        break;
                    case TipoPopUp.AGREGAR_AGENDA_MEDICA:
                        _VisibleAgregarAgendaMedica = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarAgendaMedica");
                        break;
                    case TipoPopUp.HISTORICO_TRATAMIENTO:
                        _VisibleHistoricoTratamiento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleHistoricoTratamiento");
                        break;
                    case TipoPopUp.BUSCAR_AGENDA:
                        _VisibleBuscarAgenda = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarAgenda");
                        break;
                    case TipoPopUp.EXCARCELACIONES_CANCELAR:
                        _VisibleExcarcelacionesCancelar = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleExcarcelacionesCancelar");
                        break;
                    case TipoPopUp.INCIDENCIA_ATENCION_CITA:
                        _VisibleAgregarIncidenciaAtencionCita = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarIncidenciaAtencionCita");
                        break;
                    case TipoPopUp.BUSCAR_NOTA_MEDICA:
                        _VisibleBuscarNotaMedica = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarNotaMedica");
                        break;
                    case TipoPopUp.SELECCIONAR_REPORTE_SANCION:
                        _VisibleSeleccionarReporteSancion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSeleccionarReporteSancion");
                        break;
                    case TipoPopUp.FILTRO_REPORTE_CAUSAS_PENALES:
                        _VisibleReporteFiltroCausaPenal = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleReporteFiltroCausaPenal");
                        break;
                    case TipoPopUp.BUSCAR_INTERCONSULTA_SOLICITUD:
                        _VisibleBuscarInterconsultaSolicitud = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarInterconsultaSolicitud");
                        break;
                    case TipoPopUp.AGREGAR_PROCESO_LIBERTAD:
                        _VisibleAgregarProcesoLibertad = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarProcesoLibertad");
                        break;
                    case TipoPopUp.AGREGAR_MEDIDA:
                        _VisibleAgregarMedida = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarMedida");
                        break;
                    case TipoPopUp.AGREGAR_MEDIDA_ESTAUS:
                        _VisibleAgregarMedidaEstatus = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarMedidaEstatus");
                        break;
                    case TipoPopUp.AGREGAR_PERSONA:
                        _VisibleAgregarPersona = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarPersona");
                        break;
                    case TipoPopUp.BUSCAR_CANALIZACIONES:
                        _VisibleBuscarCanalizaciones = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarCanalizaciones");
                        break;
                    case TipoPopUp.AGREGAR_LUGAR:
                        _VisibleAgregarLugar = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarLugar");
                        break;
                    case TipoPopUp.AGREGAR_MEDIDA_PRESENTACION:
                        _VisibleAgregarMedidaPresentacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarMedidaPresentacion");
                        break;
                    case TipoPopUp.AGREGAR_MEDIDA_DOCUMENTO:
                        _VisibleAgregarMedidaDocumento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarMedidaDocumento");
                        break;
                    case TipoPopUp.BUSCAR_INTERCONSULTA_MEMORIA:
                        _VisibleBuscarInterconsultaMemoria = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarInterconsultaMemoria");
                        break;
                    case TipoPopUp.VER_MEDIDA_ESTATUS:
                        _VisibleVerMedidaEstatus = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVerMedidaEstatus");
                        break;
                    case TipoPopUp.VER_MEDIDA_PERSONA:
                        _VisibleVerMedidaPersona = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVerMedidaPersona");
                        break;
                    case TipoPopUp.VER_MEDIDA_PRESENTACION:
                        _VisibleVerMedidaPresentacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVerMedidaPresentacion");
                        break;
                    case TipoPopUp.NOTAS_MEDICAS_HOSPITALIZACION:
                        _VisibleNotaMedicaHospitalizacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleNotaMedicaHospitalizacion");
                        break;
                    case TipoPopUp.VER_MEDIDA_LUGARES:
                        _VisibleVerMedidaLugares = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVerMedidaLugares");
                        break;
                    case TipoPopUp.AGREGAR_PROCESO_LIBERTAD_SEGUMIENTO:
                        _VisibleAgregarProcesoLibertadSeguimiento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarProcesoLibertadSeguimiento");
                        break;
                    case TipoPopUp.BUSCAR_ESCALA_RIESGO:
                        _VisibleBuscarEscalaRiesgo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarEscalaRiesgo");
                        break;
                    case TipoPopUp.AGREGAR_ACTIVIDAD_PROGRAMA:
                        _VisibleActividadPrograma = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleActividadPrograma");
                        break;
                    case TipoPopUp.AGENDA_LIBERTAD:
                        _VisibleAgendaLibertad = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgendaLibertad");
                        break;
                    case TipoPopUp.OFICIO_ASIGNACION_NSJP:
                        _VisibleOficioAsignacionNSJP = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleOficioAsignacionNSJP");
                        break;
                    case TipoPopUp.OFICIO_ASIGNACION_TRADICIONAL:
                        _VisibleOficioAsignacionTradicional = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleOficioAsignacionTradicional");
                        break;
                    case TipoPopUp.OFICIO_CONCLUSION:
                        _VisibleOficioConclusion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleOficioConclusion");
                        break;
                    case TipoPopUp.OFICIO_BAJA:
                        _VisibleOficioBaja = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleOficioBaja");
                        break;
                    case TipoPopUp.BUSCAR_CAUSA_PENAL:
                        _VisibleBuscarCausaPenal = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarCausaPenal");
                        break;
                    case TipoPopUp.BUSCAR_TV_MEDICO_PENDIENTE:
                        _VisibleBuscarTVMedico = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarTVMedico");
                        break;
                    case TipoPopUp.REAGENDAR_TV_CITA_MEDICA:
                        _VisibleReagendarTV_Cita_Medica = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleReagendarTV_Cita_Medica");
                        break;
                    case TipoPopUp.RECREAR_TV_INTERCONSULTA:
                        _VisibleRecrearTVInterconsultaView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleRecrearTVInterconsultaView");
                        break;
                    case TipoPopUp.CAPTURA_OFICIO_PERSONALIDAD:
                        _VisibleCapturarNumeroOficioPersonalidadView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleCapturarNumeroOficioPersonalidadView");
                        break;
                    case TipoPopUp.CERRAR_TODOS:
                        _PopUpDataContext = null;
                        RaiseStaticPropertyChanged("PopUpDataContext");

                        _VisibleEspecialidadCita = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEspecialidadCita");
                        _VisibleAgregarArchivoResultTratam = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarArchivoResultTratam");
                        _VisibleAgregarImagenesHistoriaClinicaDentalView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarImagenesHistoriaClinicaDentalView");
                        _VisibleAgregarInterconsultaMedicaExcarc = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarInterconsultaMedicaExcarc");
                        _VisibleConsultarDocumentosEstudioPersonalidadView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleConsultarDocumentosEstudioPersonalidadView");
                        _VisibleDigitalizacionSimpleView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleDigitalizacionSimpleView");
                        _VisibleAgregarDocumentosHistoriaClinicaView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDocumentosHistoriaClinicaView");
                        _VisibleSancionDiscFF = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSancionDiscFF");
                        _VisibleAgregaEducFF = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregaEducFF");
                        _VisibleAgregarDiasLab = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDiasLab");
                        _VisibleAgregarIntegranteGrupoFamFF = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarIntegranteGrupoFamFF");
                        _VisibleAgregarOpinion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarOpinion");
                        _VisibleAgregarCongreg = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarCongreg");
                        _VisibleAgregarGrupoPsicologico = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarGrupoPsicologico");
                        _VisibleTomarFotoIfeCedula = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleTomarFotoIfeCedula");
                        _VisibleAgregarActividadLaboral = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarActividadLaboral");
                        _VisibleAgregarActividCultDepEstudioEducativo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarActividCultDepEstudioEducativo");
                        _VisibleAgregarActividadesEducativasEstudioEducativo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarActividadesEducativasEstudioEducativo");
                        _VisibleAgregarEscolaridadEstudioEducativo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarEscolaridadEstudioEducativo");
                        _VisibleAgregarVisitaEdificio = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarVisitaEdificio");
                        _VisibleAgregarVisitaApellido = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarVisitaApellido");
                        _VisibleIngresarVisitanteListaTonta = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleIngresarVisitanteListaTonta");
                        _VisibleApodo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleApodo");
                        _VisibleTomarFotoSenasParticulares = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleTomarFotoSenasParticulares");
                        _VisibleAlias = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAlias");
                        _VisibleHuellas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleHuellas");
                        _VisibleBuscarConsultasMedicas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarConsultasMedicas");
                        _VisibleBuscarVisitaExistente = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarVisitaExistente");
                        _VisibleBuscarPersonasExistentes = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarPersonasExistentes");
                        _VisibleBuscarPersonasExternas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarPersonasExternas");
                        _VisibleAgregarVisitaExterna = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarVisitaExterna");
                        _VisibleAmpliarDescripcion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAmpliarDescripcion");
                        _VisibleAmpliarDescripcionGenerico = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAmpliarDescripcionGenerico");
                        _VisibleAgregarDocumento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDocumento");
                        _VisiblePandillaInterno = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisiblePandillaInterno");
                        _VisibleRelacionInterno = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleRelacionInterno");
                        _VisibleUltimosEmpleos = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleUltimosEmpleos");
                        _VisibleDatosGrupoFamiliar = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleDatosGrupoFamiliar");
                        _VisibleAmpliarDescripcion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAmpliarDescripcion");
                        _VisibleUsoDrogas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleUsoDrogas");
                        _VisibleAmpliarDescripcion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAmpliarDescripcion");
                        _VisibleFamiliarDelito = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleFamiliarDelito");
                        _VisibleFamiliarDroga = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleFamiliarDroga");
                        _VisibleIngresarAcompanante = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleIngresarAcompanante");
                        _VisibleIngresaImputadoVisitante = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleIngresaImputadoVisitante");
                        _VisibleBusquedaImputado = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBusquedaImputado");
                        _VisibleBusquedaExpediente = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBusquedaExpediente");
                        _VisibleProgramarVisita = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleProgramarVisita");
                        _VisibleIngresoAnterior = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleIngresoAnterior");
                        _VisibleAgregarSancion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarSancion");
                        _VisibleAgregarDictamen = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDictamen");
                        _VisibleAgregarGFPV = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarGFPV");
                        _VisibleAgregarActividad = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarActividad");
                        _VisibleAgregarDelitoCP = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDelitoCP");
                        _VisibleAgregarCeldasObservacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarCeldasObservacion");
                        _VisibleEmpalmeFechas = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEmpalmeFechas");
                        _VisibleFondoOscuro = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSeleccionarColor");
                        _VisibleSeleccionarColor = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarIncidente");
                        _VisibleAgregarIncidente = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarSancionIncidente");
                        _VisibleAgregarSancionIncidente = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleFondoOscuro");
                        _VisibleDigitalizarDocumento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleDigitalizarDocumento");
                        _VisibleRegistroCorrespondencia = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleRegistroCorrespondencia");
                        _VisibleNotificacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleNotificacion");
                        _VisibleBuscarDecomiso = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarDecomiso");
                        _VisibleBuscarInterno = Visibility.Collapsed;
                        _VisibleBuscarOficialACargo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarOficialACargo");
                        RaiseStaticPropertyChanged("VisibleBuscarInterno");
                        _VisibleBuscarVisitante = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarVisitamte");
                        _VisibleBuscarEmpleado = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarEmpleado");
                        _VisibleBuscarExterno = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarExterno");
                        _VisibleSeleccionaUbicacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSeleccionaUbicacion");
                        _VisibleSeleccionaUbicacionCelda = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSeleccionaUbicacionCelda");
                        _VisibleEditarFecha = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEditarFecha");
                        _VisibleEditarIntegrantesGrupo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEditarIntegrantesGrupo");
                        _VisibleProximaCausaPenal = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleProximaCausaPenal");
                        _VisibleBuscarEvento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarEvento");
                        _VisibleEventoPrograma = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEventoPrograma");
                        _VisibleEventoPresidium = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEventoPresidium");
                        _VisibleInfTecnica = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleInfTecnica");
                        _VisibleProceso = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleProceso");
                        _VisibleRol = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleRol");
                        _VisibleProcesoUsuario = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleProcesoUsuario");
                        _VisibleAgregarUsuario = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarUsuario");
                        _VisibleImpresionDecomiso = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleImpresionDecomiso");
                        _VisibleVisualizarDocumento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVisualizarDocumento");
                        _VisibleBuscarTraslados = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarTraslados");
                        _VisibleAgregarParticipanteGrupo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarParticipanteGrupo");
                        _VisibleAgregarFechaParticipante = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarFechaParticipante");
                        _VisibleAgregarParticipanteComplementario = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarParticipanteComplementario");
                        _VisibleAgendarCita = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgendarCita");
                        _VisibleBuscarPersona = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarPersona");
                        _VisibleBusquedaInternosProgramas = Visibility.Visible;
                        RaiseStaticPropertyChanged("VisibleBusquedaInternosProgramas");
                        _VisibleListaAsistencia = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleListaAsistencia");
                        _VisibleVerSeleccionadosCompl = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVerSeleccionadosCompl");
                        _VisibleBuscarLiberado = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarLiberado");
                        _VisibleBuscarNUCsPorImputado = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarNUCsPorImputado");
                        _VisibleBuscarExcarcelaciones = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarExcarcelaciones");
                        _VisibleAgregarSanciones = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarSanciones");
                        _VisibleEstudioPersonalidad = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEstudioPersonalidad");
                        _VisibleEstudioPersonalidadDetalle = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleEstudioPersonalidadDetalle");
                        _VisibleBuscarImputadosNUC = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarImputadosNUC");
                        _VisibleAgregarDestinosExcarcelacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarDestinosExcarcelacion");
                        _VisibleMotivo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleMotivo");
                        _VisibleSolicitudCita = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSolicitudCita");
                        _VisibleExcarcelacion_Motivo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleExcarcelacion_Motivo");
                        _VisibleBuscarDecomisoEvento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarDecomisoEvento");
                        _VisibleAgregarAgendaMedica = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarAgendaMedica");
                        _VisibleHistoricoTratamiento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleHistoricoTratamiento");
                        _VisibleBuscarAgenda = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarAgenda");
                        _VisibleExcarcelacionesCancelar = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleExcarcelacionesCancelar");
                        _VisibleAgregarIncidenciaAtencionCita = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarIncidenciaAtencionCita");
                        _VisibleBuscarInternoCP = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarInternoCP");
                        _VisibleBuscarNotaMedica = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarNotaMedica");
                        _VisibleSeleccionarReporteSancion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleSeleccionarReporteSancion");
                        _VisibleReporteFiltroCausaPenal = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleReporteFiltroCausaPenal");
                        _VisibleBuscarInterconsultaSolicitud = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarInterconsultaSolicitud");
                        _VisibleAgregarProcesoLibertad = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarProcesoLibertad");
                        _VisibleAgregarMedida = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarMedida");
                        _VisibleAgregarMedidaEstatus = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarMedidaEstatus");
                        _VisibleAgregarPersona = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarPersona");
                        _VisibleBuscarCanalizaciones = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarCanalizaciones");
                        _VisibleAgregarLugar = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarLugar");
                        _VisibleAgregarMedidaPresentacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarMedidaPresentacion");
                        _VisibleAgregarMedidaDocumento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarMedidaDocumento");
                        _VisibleBuscarInterconsultaMemoria = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarInterconsultaMemoria");
                        _VisibleVerMedidaEstatus = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVerMedidaEstatus");
                        _VisibleVerMedidaPersona = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVerMedidaPersona");
                        _VisibleVerMedidaPresentacion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVerMedidaPresentacion");
                        _VisibleVerMedidaLugares = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleVerMedidaLugares");
                        _VisibleAgregarProcesoLibertadSeguimiento = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgregarProcesoLibertadSeguimiento");
                        _VisibleBuscarEscalaRiesgo = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarEscalaRiesgo");
                        _VisibleActividadPrograma = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleActividadPrograma");
                        _VisibleAgendaLibertad = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleAgendaLibertad");
                        _VisibleOficioAsignacionNSJP = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleOficioAsignacionNSJP");
                        _VisibleOficioAsignacionTradicional = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleOficioAsignacionTradicional");
                        _VisibleOficioConclusion = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleOficioConclusion");
                        _VisibleOficioBaja = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleOficioBaja");
                        _VisibleBuscarCausaPenal = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarCausaPenal");
                        _VisibleBuscarTVMedico = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleBuscarTVMedico");
                        _VisibleReagendarTV_Cita_Medica = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleReagendarTV_Cita_Medica");
                        _VisibleRecrearTVInterconsultaView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleRecrearTVInterconsultaView");
                        _VisibleCapturarNumeroOficioPersonalidadView = Visibility.Collapsed;
                        RaiseStaticPropertyChanged("VisibleCapturarNumeroOficioPersonalidadView");
                        break;
                        break;
                }

                EnabledMenu = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar popup", ex);
            }
        }
        #endregion

        #region [Aux]
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged(string propName)
        {
            EventHandler<PropertyChangedEventArgs> handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propName));
        }
        #endregion

        #region [Lock Menu]
        private static bool _FocusBlock;
        public static bool FocusBlock
        {
            get { return _FocusBlock; }
            set
            {
                _FocusBlock = value;
                RaiseStaticPropertyChanged("FocusBlock");
            }
        }
        private static Visibility _LockMenu = Visibility.Visible;
        public static Visibility LockMenu { get { return PopUpsViewModels._LockMenu; } set { _LockMenu = value; RaiseStaticPropertyChanged("LockMenu"); } }
        private static Visibility _UnLockMenu = Visibility.Collapsed;
        public static Visibility UnLockMenu { get { return PopUpsViewModels._UnLockMenu; } set { _UnLockMenu = value; RaiseStaticPropertyChanged("UnLockMenu"); } }
        #endregion

    }
}

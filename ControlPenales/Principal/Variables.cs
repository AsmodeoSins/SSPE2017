using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class PrincipalViewModel : ValidationViewModelBase
    {
        #region Menu
        #region Proceso
        #region Registro
        private Visibility mRegistroMV = Visibility.Visible;
        public Visibility MRegistroMV
        {
            get { return mRegistroMV; }
            set { mRegistroMV = value; OnPropertyChanged("MRegistroMV"); }
        }

        private Visibility mRegistroV = Visibility.Visible;
        public Visibility MRegistroV
        {
            get { return mRegistroV; }
            set { mRegistroV = value; OnPropertyChanged("MRegistroV"); }
        }

        private Visibility mDecomisoV = Visibility.Visible;
        public Visibility MDecomisoV
        {
            get { return mDecomisoV; }
            set { mDecomisoV = value; OnPropertyChanged("MDecomisoV"); }
        }

        private Visibility mVisitaExternaV = Visibility.Visible;
        public Visibility MVisitaExternaV
        {
            get { return mVisitaExternaV; }
            set { mVisitaExternaV = value; OnPropertyChanged("MVisitaExternaV"); }
        }
        #endregion

        #region Seguimiento
        private Visibility mSeguimientoMV = Visibility.Visible;
        public Visibility MSeguimientoMV
        {
            get { return mSeguimientoMV; }
            set { mSeguimientoMV = value; OnPropertyChanged("MSeguimientoMV"); }
        }

        #region Juridico
        private Visibility mJuridicoMV = Visibility.Visible;
        public Visibility MJuridicoMV
        {
            get { return mJuridicoMV; }
            set { mJuridicoMV = value; OnPropertyChanged("MJuridicoMV"); }
        }
        private Visibility mCausaPenalV = Visibility.Visible;
        public Visibility MCausaPenalV
        {
            get { return mCausaPenalV; }
            set { mCausaPenalV = value; OnPropertyChanged("MCausaPenalV"); }
        }
        private Visibility mIdentificacionV = Visibility.Visible;
        public Visibility MIdentificacionV
        {
            get { return mIdentificacionV; }
            set { mIdentificacionV = value; OnPropertyChanged("MIdentificacionV"); }
        }
        private Visibility mEstatusAdministrativoV = Visibility.Visible;
        public Visibility MEstatusAdministrativoV
        {
            get { return mEstatusAdministrativoV; }
            set { mEstatusAdministrativoV = value; OnPropertyChanged("MEstatusAdministrativoV"); }
        }
        #endregion

        #region Administrativo
        private Visibility mAdministrativoMV = Visibility.Visible;
        public Visibility MAdministrativoMV
        {
            get { return mAdministrativoMV; }
            set { mAdministrativoMV = value; OnPropertyChanged("MAdministrativoMV"); }
        }
        private Visibility mPertenenciasV = Visibility.Visible;
        public Visibility MPertenenciasV
        {
            get { return mPertenenciasV; }
            set { mPertenenciasV = value; OnPropertyChanged("MPertenenciasV"); }
        }
        private Visibility mUbicacionEstanciaV = Visibility.Visible;
        public Visibility MUbicacionEstanciaV
        {
            get { return mUbicacionEstanciaV; }
            set { mUbicacionEstanciaV = value; OnPropertyChanged("MUbicacionEstanciaV"); }
        }
        private Visibility mIncidentesV = Visibility.Visible;
        public Visibility MIncidentesV
        {
            get { return mIncidentesV; }
            set { mIncidentesV = value; OnPropertyChanged("MIncidentesV"); }
        }
        #endregion

        #region Tecnico
        private Visibility mTecnicoMV = Visibility.Visible;
        public Visibility MTecnicoMV
        {
            get { return mTecnicoMV; }
            set { mTecnicoMV = value; OnPropertyChanged("MTecnicoMV"); }
        }

        private Visibility mEstudioPersonalidadV = Visibility.Visible;
        public Visibility MEstudioPersonalidadV
        {
            get { return mEstudioPersonalidadV; }
            set { mEstudioPersonalidadV = value; OnPropertyChanged("MEstudioPersonalidadV"); }
        }

        #region Programas de Reinsercion
        private Visibility mProgramasReinsercionMV = Visibility.Visible;
        public Visibility MProgramasReinsercionMV
        {
            get { return mProgramasReinsercionMV; }
            set { mProgramasReinsercionMV = value; OnPropertyChanged("MProgramasReinsercionMV"); }
        }

        private Visibility mCreacionGruposV = Visibility.Visible;
        public Visibility MCreacionGruposV
        {
            get { return mCreacionGruposV; }
            set { mCreacionGruposV = value; OnPropertyChanged("MCreacionGruposV"); }
        }

        private Visibility mManejoGruposV = Visibility.Visible;
        public Visibility MManejoGruposV
        {
            get { return mManejoGruposV; }
            set { mManejoGruposV = value; OnPropertyChanged("MManejoGruposV"); }
        }

        private Visibility mManejoEmpalmesV = Visibility.Visible;
        public Visibility MManejoEmpalmesV
        {
            get { return mManejoEmpalmesV; }
            set { mManejoEmpalmesV = value; OnPropertyChanged("MManejoEmpalmesV"); }
        }

        private Visibility mManejoCanceladoSuspendidoV = Visibility.Visible;
        public Visibility MManejoCanceladoSuspendidoV
        {
            get { return mManejoCanceladoSuspendidoV; }
            set { mManejoCanceladoSuspendidoV = value; OnPropertyChanged("MManejoCanceladoSuspendidoV"); }
        }

        private Visibility mControlCalificacionesV = Visibility.Visible;
        public Visibility MControlCalificacionesV
        {
            get { return mControlCalificacionesV; }
            set { mControlCalificacionesV = value; OnPropertyChanged("MControlCalificacionesV"); }
        }

        private Visibility mControlParticipacionV = Visibility.Visible;
        public Visibility MControlParticipacionV
        {
            get { return mControlParticipacionV; }
            set { mControlParticipacionV = value; OnPropertyChanged("MControlParticipacionV"); }
        }
        #endregion

        private Visibility mControlSancionesV = Visibility.Visible;
        public Visibility MControlSancionesV
        {
            get { return mControlSancionesV; }
            set { mControlSancionesV = value; OnPropertyChanged("MControlSancionesV"); }
        }

        private Visibility mEMIV = Visibility.Visible;
        public Visibility MEMIV
        {
            get { return mEMIV; }
            set { mEMIV = value; OnPropertyChanged("MEMIV"); }
        }
        #endregion
        #endregion

        #region Control y Seguridad
        private Visibility mControlSeguridadMV = Visibility.Visible;
        public Visibility MControlSeguridadMV
        {
            get { return mControlSeguridadMV; }
            set { mControlSeguridadMV = value; OnPropertyChanged("MControlSeguridadMV"); }
        }
        #region Visitas
        private Visibility mVisitasMV = Visibility.Visible;
        public Visibility MVisitasMV
        {
            get { return mVisitasMV; }
            set { mVisitasMV = value; OnPropertyChanged("MVisitasMV"); }
        }

        private Visibility mPadronVisitaV = Visibility.Visible;
        public Visibility MPadronVisitaV
        {
            get { return mPadronVisitaV; }
            set { mPadronVisitaV = value; OnPropertyChanged("MPadronVisitaV"); }
        }

        private Visibility mProgramaVisitaEdificioV = Visibility.Visible;
        public Visibility MProgramaVisitaEdificioV
        {
            get { return mProgramaVisitaEdificioV; }
            set { mProgramaVisitaEdificioV = value; OnPropertyChanged("MProgramaVisitaEdificioV"); }
        }

        private Visibility mProgramaVisitaApellidoV = Visibility.Visible;
        public Visibility MProgramaVisitaApellidoV
        {
            get { return mProgramaVisitaApellidoV; }
            set { mProgramaVisitaApellidoV = value; OnPropertyChanged("MProgramaVisitaApellidoV"); }
        }

        private Visibility mVerificarPaseV = Visibility.Visible;
        public Visibility MVerificarPaseV
        {
            get { return mVerificarPaseV; }
            set { mVerificarPaseV = value; OnPropertyChanged("MVerificarPaseV"); }
        }

        private Visibility mCancelarSuspenderCredencialesV = Visibility.Visible;
        public Visibility MCancelarSuspenderCredencialesV
        {
            get { return mCancelarSuspenderCredencialesV; }
            set { mCancelarSuspenderCredencialesV = value; OnPropertyChanged("MCancelarSuspenderCredencialesV"); }
        }
        #endregion
        private Visibility mSolicitudCitasV = Visibility.Visible;
        public Visibility MSolicitudCitasV
        {
            get { return mSolicitudCitasV; }
            set { mSolicitudCitasV = value; OnPropertyChanged("MSolicitudCitasV"); }
        }

        private Visibility mPersonalV = Visibility.Visible;
        public Visibility MPersonalV
        {
            get { return mPersonalV; }
            set { mPersonalV = value; OnPropertyChanged("MPersonalV"); }
        }
        #region Abogados
        private Visibility mAbogadosMV = Visibility.Visible;
        public Visibility MAbogadosMV
        {
            get { return mAbogadosMV; }
            set { mAbogadosMV = value; OnPropertyChanged("MAbogadosMV"); }
        }

        private Visibility mPadronAbogadosV = Visibility.Visible;
        public Visibility MPadronAbogadosV
        {
            get { return mPadronAbogadosV; }
            set { mPadronAbogadosV = value; OnPropertyChanged("MPadronAbogadosV"); }
        }

        private Visibility mPadronColaboradoresV = Visibility.Visible;
        public Visibility MPadronColaboradoresV
        {
            get { return mPadronColaboradoresV; }
            set { mPadronColaboradoresV = value; OnPropertyChanged("MPadronColaboradoresV"); }
        }
        #region Actuarios
        private Visibility mActuariosMV = Visibility.Visible;
        public Visibility MActuariosMV
        {
            get { return mActuariosMV; }
            set { mActuariosMV = value; OnPropertyChanged("MActuariosMV"); }
        }

        private Visibility mPadronActuariosV = Visibility.Visible;
        public Visibility MPadronActuariosV
        {
            get { return mPadronActuariosV; }
            set { mPadronActuariosV = value; OnPropertyChanged("MPadronActuariosV"); }
        }

        private Visibility mRequerimientoInternosV = Visibility.Visible;
        public Visibility MRequerimientoInternosV
        {
            get { return mRequerimientoInternosV; }
            set { mRequerimientoInternosV = value; OnPropertyChanged("MRequerimientoInternosV"); }
        }
        #endregion
        private Visibility mAduanaV = Visibility.Visible;
        public Visibility MAduanaV
        {
            get { return mAduanaV; }
            set { mAduanaV = value; OnPropertyChanged("MAduanaV"); }
        }
        #endregion
        #region Control Internos
        private Visibility mControlInternosMV = Visibility.Visible;
        public Visibility MControlInternosMV
        {
            get { return mControlInternosMV; }
            set { mControlInternosMV = value; OnPropertyChanged("MControlInternosMV"); }
        }

        private Visibility mControlInternosEdificioV = Visibility.Visible;
        public Visibility MControlInternosEdificioV
        {
            get { return mControlInternosEdificioV; }
            set { mControlInternosEdificioV = value; OnPropertyChanged("MControlInternosEdificioV"); }
        }

        private Visibility mControlActividadesProgramadasV = Visibility.Visible;
        public Visibility MControlActividadesProgramadasV
        {
            get { return mControlActividadesProgramadasV; }
            set { mControlActividadesProgramadasV = value; OnPropertyChanged("MControlActividadesProgramadasV"); }
        }

        private Visibility mControlActividadesNoProgramadasV = Visibility.Visible;
        public Visibility MControlActividadesNoProgramadasV
        {
            get { return mControlActividadesNoProgramadasV; }
            set { mControlActividadesNoProgramadasV = value; OnPropertyChanged("MControlActividadesNoProgramadasV"); }
        }
        #endregion
        #endregion

        #region Salida
        private Visibility mSalidaMV = Visibility.Visible;
        public Visibility MSalidaMV
        {
            get { return mSalidaMV; }
            set { mSalidaMV = value; OnPropertyChanged("MSalidaMV"); }
        }
        #region Excarcelacion
        private Visibility mExcarcelacionesMV = Visibility.Visible;
        public Visibility MExcarcelacionesMV
        {
            get { return mExcarcelacionesMV; }
            set { mExcarcelacionesMV = value; OnPropertyChanged("MExcarcelacionesMV"); }
        }
        private Visibility mExcarcelacionV = Visibility.Visible;
        public Visibility MExcarcelacionV
        {
            get { return mExcarcelacionV; }
            set { mExcarcelacionV = value; OnPropertyChanged("MExcarcelacionV"); }
        }
        private Visibility mAutorizacionExcarcelacionV = Visibility.Visible;
        public Visibility MAutorizacionExcarcelacionV
        {
            get { return mAutorizacionExcarcelacionV; }
            set { mAutorizacionExcarcelacionV = value; OnPropertyChanged("MAutorizacionExcarcelacionV"); }
        }
        #endregion
        #region Traslado
        private Visibility mTrasladoMV = Visibility.Visible;
        public Visibility MTrasladoMV
        {
            get { return mTrasladoMV; }
            set { mTrasladoMV = value; OnPropertyChanged("MTrasladoMV"); }
        }
        private Visibility mTrasladoForaneoV = Visibility.Visible;
        public Visibility MTrasladoForaneoV
        {
            get { return mTrasladoForaneoV; }
            set { mTrasladoForaneoV = value; OnPropertyChanged("MTrasladoForaneoV"); }
        }
        private Visibility mTrasladoMasivoV = Visibility.Visible;
        public Visibility MTrasladoMasivoV
        {
            get { return mTrasladoMasivoV; }
            set { mTrasladoMasivoV = value; OnPropertyChanged("MTrasladoMasivoV"); }
        }
        private Visibility mAutorizaIngresoTrasladoV = Visibility.Visible;
        public Visibility MAutorizaIngresoTrasladoV
        {
            get { return mAutorizaIngresoTrasladoV; }
            set { mAutorizaIngresoTrasladoV = value; OnPropertyChanged("MAutorizaIngresoTrasladoV"); }
        }
        #endregion
        private Visibility mLiberacionV = Visibility.Visible;
        public Visibility MLiberacionV
        {
            get { return mLiberacionV; }
            set { mLiberacionV = value; OnPropertyChanged("MLiberacionV"); }
        }
        private Visibility mLiberacionBiometriaV = Visibility.Visible;
        public Visibility MLiberacionBiometriaV
        {
            get { return mLiberacionBiometriaV; }
            set { mLiberacionBiometriaV = value; OnPropertyChanged("MLiberacionBiometriaV"); }
        }
        #endregion

        #region Eventos
        private Visibility mEventosMV = Visibility.Visible;
        public Visibility MEventosMV
        {
            get { return mEventosMV; }
            set { mEventosMV = value; OnPropertyChanged("MEventosMV"); }
        }

        private Visibility mProgramacionEventoV = Visibility.Visible;
        public Visibility MProgramacionEventoV
        {
            get { return mProgramacionEventoV; }
            set { mProgramacionEventoV = value; OnPropertyChanged("MProgramacionEventoV"); }
        }
        #endregion

        #region Citas
        private Visibility mCitasMV = Visibility.Visible;
        public Visibility MCitasMV
        {
            get { return mCitasMV; }
            set { mCitasMV = value; OnPropertyChanged("MCitasMV"); }
        }

        private Visibility mSolicitudCitaV = Visibility.Visible;
        public Visibility MSolicitudCitaV
        {
            get { return mSolicitudCitaV; }
            set { mSolicitudCitaV = value; OnPropertyChanged("MSolicitudCitaV"); }
        }

        private Visibility mAgendarCitaV = Visibility.Visible;
        public Visibility MAgendarCitaV
        {
            get { return mAgendarCitaV; }
            set { mAgendarCitaV = value; OnPropertyChanged("MAgendarCitaV"); }
        }

        private Visibility mAtenderCitaV = Visibility.Visible;
        public Visibility MAtenderCitaV
        {
            get { return mAtenderCitaV; }
            set { mAtenderCitaV = value; OnPropertyChanged("MAtenderCitaV"); }
        }
        #endregion

        #region Liberados
        private Visibility mLiberadosMV = Visibility.Visible;
        public Visibility MLiberadosMV
        {
            get { return mLiberadosMV; }
            set { mLiberadosMV = value; OnPropertyChanged("MLiberadosMV"); }
        }

        private Visibility mRegistroLiberadosV = Visibility.Visible;
        public Visibility MRegistroLiberadosV
        {
            get { return mRegistroLiberadosV; }
            set { mRegistroLiberadosV = value; OnPropertyChanged("MRegistroLiberadosV"); }
        }

        private Visibility mTrabajoSocialV = Visibility.Visible;
        public Visibility MTrabajoSocialV
        {
            get { return mTrabajoSocialV; }
            set { mTrabajoSocialV = value; OnPropertyChanged("MTrabajoSocialV"); }
        }

        private Visibility mReportePsicologicoV = Visibility.Visible;
        public Visibility MReportePsicologicoV
        {
            get { return mReportePsicologicoV; }
            set { mReportePsicologicoV = value; OnPropertyChanged("MReportePsicologicoV"); }
        }

        private Visibility mVisitaDomiciliariaV = Visibility.Visible;
        public Visibility MVisitaDomiciliariaV
        {
            get { return mVisitaDomiciliariaV; }
            set { mVisitaDomiciliariaV = value; OnPropertyChanged("MVisitaDomiciliariaV"); }
        }

        private Visibility mEMILiberadoV = Visibility.Visible;
        public Visibility MEMILiberadoV
        {
            get { return mEMILiberadoV; }
            set { mEMILiberadoV = value; OnPropertyChanged("MEMILiberadoV"); }
        }
        #endregion

        #region Estudios
        private Visibility mEstudiosMV = Visibility.Visible;
        public Visibility MEstudiosMV
        {
            get { return mEstudiosMV; }
            set { mEstudiosMV = value; OnPropertyChanged("MEstudiosMV"); }
        }

        private Visibility mEstudioSocioeconomicoV = Visibility.Visible;
        public Visibility MEstudioSocioeconomicoV
        {
            get { return mEstudioSocioeconomicoV; }
            set { mEstudioSocioeconomicoV = value; OnPropertyChanged("MEstudioSocioeconomicoV"); }
        }

        private Visibility mRealizacionListasV = Visibility.Visible;
        public Visibility MRealizacionListasV
        {
            get { return mRealizacionListasV; }
            set { mRealizacionListasV = value; OnPropertyChanged("MRealizacionListasV"); }
        }

        #endregion

        #region Area Medica
        private Visibility mAreaMedicaMV = Visibility.Visible;
        public Visibility MAreaMedicaMV
        {
            get { return mAreaMedicaMV; }
            set { mAreaMedicaMV = value; OnPropertyChanged("MAreaMedicaMV"); }
        }

        private Visibility mHistoriaClinicaV = Visibility.Visible;
        public Visibility MHistoriaClinicaV
        {
            get { return mHistoriaClinicaV; }
            set { mHistoriaClinicaV = value; OnPropertyChanged("MHistoriaClinicaV"); }
        }

        private Visibility mAgendaMedicaV = Visibility.Visible;
        public Visibility MAgendaMedicaV
        {
            get { return mAgendaMedicaV; }
            set { mAgendaMedicaV = value; OnPropertyChanged("MAgendaMedicaV"); }
        }

        private Visibility mReagendaTVV = Visibility.Visible;
        public Visibility MReagendaTVV
        {
            get { return mReagendaTVV; }
            set { mReagendaTVV = value; OnPropertyChanged("MReagendaTVV"); }
        }

        private Visibility _MReporteNotasMedicas = Visibility.Visible;
        public Visibility MReporteNotasMedicas
        {
            get { return _MReporteNotasMedicas; }
            set { _MReporteNotasMedicas = value; OnPropertyChanged("MReporteNotasMedicas"); }
        }

        private Visibility mNotaMedicaV = Visibility.Visible;
        public Visibility MNotaMedicaV
        {
            get { return mNotaMedicaV; }
            set { mNotaMedicaV = value; OnPropertyChanged("MNotaMedicaV"); }
        }

        private Visibility mNotaMedicaEspecialistaV = Visibility.Visible;
        public Visibility MNotaMedicaEspecialistaV
        {
            get { return mNotaMedicaEspecialistaV; }
            set { mNotaMedicaEspecialistaV = value; OnPropertyChanged("MNotaMedicaEspecialistaV"); }
        }

        private Visibility mNotaEvolucionV = Visibility.Visible;
        public Visibility MNotaEvolucionV
        {
            get { return mNotaEvolucionV; }
            set { mNotaEvolucionV = value; OnPropertyChanged("MNotaEvolucionV"); }
        }

        private Visibility mCertificadoTraspasoCancelacionesV = Visibility.Visible;
        public Visibility MCertificadoTraspasoCancelacionesV
        {
            get { return mCertificadoTraspasoCancelacionesV; }
            set { mCertificadoTraspasoCancelacionesV = value; OnPropertyChanged("MCertificadoTraspasoCancelacionesV"); }
        }

        private Visibility mSolicitudAtencionEstatusV = Visibility.Visible;
        public Visibility MSolicitudAtencionEstatusV
        {
            get { return mSolicitudAtencionEstatusV; }
            set { mSolicitudAtencionEstatusV = value; OnPropertyChanged("MSolicitudAtencionEstatusV"); }
        }

        private Visibility mHojaEnfermeriaV = Visibility.Visible;
        public Visibility MHojaEnfermeriaV
        {
            get { return mHojaEnfermeriaV; } 
            set { mHojaEnfermeriaV = value; OnPropertyChanged("MHojaEnfermeriaV"); }
        }

        private Visibility mhojacontrolLiquidosV = Visibility.Visible;
        public Visibility MhojacontrolLiquidosV
        {
            get { return mhojacontrolLiquidosV; }
            set { mhojacontrolLiquidosV = value; OnPropertyChanged("MhojacontrolLiquidosV"); }
        }
        #endregion

        private Visibility mConsultaUnificadaV = Visibility.Visible;
        public Visibility MConsultaUnificadaV
        {
            get { return mConsultaUnificadaV; }
            set { mConsultaUnificadaV = value; OnPropertyChanged("MConsultaUnificadaV"); }
        }

        #endregion

        #region Consulta
        #region Internos
        private Visibility mInternosMV = Visibility.Visible;
        public Visibility MInternosMV
        {
            get { return mInternosMV; }
            set { mInternosMV = value; OnPropertyChanged("MInternosMV"); }
        }

        private Visibility mFormatosMV = Visibility.Visible;
        public Visibility MFormatosMV
        {
            get { return mFormatosMV; }
            set { mFormatosMV = value; OnPropertyChanged("MFormatosMV"); }
        }

        private Visibility mEntrevistaInicialTrabajoSocialV = Visibility.Visible;
        public Visibility MEntrevistaInicialTrabajoSocialV
        {
            get { return mEntrevistaInicialTrabajoSocialV; }
            set { mEntrevistaInicialTrabajoSocialV = value; OnPropertyChanged("MEntrevistaInicialTrabajoSocialV"); }
        }
        #endregion
        private Visibility mPlanimetriaV = Visibility.Visible;
        public Visibility MPlanimetriaV
        {
            get { return mPlanimetriaV; }
            set { mPlanimetriaV = value; OnPropertyChanged("MPlanimetriaV"); }
        }
        #endregion

        #region Reportes
        #region Visita Legal
        private Visibility mVisitaLegalMV = Visibility.Visible;
        public Visibility MVisitaLegalMV
        {
            get { return mVisitaLegalMV; }
            set { mVisitaLegalMV = value; OnPropertyChanged("MVisitaLegalMV"); }
        }

        private Visibility mBitacoraRegistroAbogadoReporteV = Visibility.Visible;
        public Visibility MBitacoraRegistroAbogadoReporteV
        {
            get { return mBitacoraRegistroAbogadoReporteV; }
            set { mBitacoraRegistroAbogadoReporteV = value; OnPropertyChanged("MBitacoraRegistroAbogadoReporteV"); }
        }

        private Visibility mPadronAbogadoReporteV = Visibility.Visible;
        public Visibility MPadronAbogadoReporteV
        {
            get { return mPadronAbogadoReporteV; }
            set { mPadronAbogadoReporteV = value; OnPropertyChanged("MPadronAbogadoReporteV"); }
        }

        #region Record de Visita
        private Visibility mRecordVisitaAbogadoMV = Visibility.Visible;
        public Visibility MRecordVisitaAbogadoMV
        {
            get { return mRecordVisitaAbogadoMV; }
            set { mRecordVisitaAbogadoMV = value; OnPropertyChanged("MRecordVisitaAbogadoMV"); }
        }

        private Visibility mRecordAbogadoReporteFechaV = Visibility.Visible;
        public Visibility MRecordAbogadoReporteFechaV
        {
            get { return mRecordAbogadoReporteFechaV; }
            set { mRecordAbogadoReporteFechaV = value; OnPropertyChanged("MRecordAbogadoReporteFechaV"); }
        }

        private Visibility mRecordAbogadoReporteIngresoV = Visibility.Visible;
        public Visibility MRecordAbogadoReporteIngresoV
        {
            get { return mRecordAbogadoReporteIngresoV; }
            set { mRecordAbogadoReporteIngresoV = value; OnPropertyChanged("MRecordAbogadoReporteIngresoV"); }
        }
        #endregion
        private Visibility mEMIPendientesV = Visibility.Visible;
        public Visibility MEMIPendientesV
        {
            get { return mEMIPendientesV; }
            set { mEMIPendientesV = value; OnPropertyChanged("MEMIPendientesV"); }
        }
        #endregion
        #endregion

        #region Catalogos
        #region Personas
        private Visibility mPersonasMV = Visibility.Visible;
        public Visibility MPersonasMV
        {
            get { return mPersonasMV; }
            set { mPersonasMV = value; OnPropertyChanged("MPersonasMV"); }
        }
        #region Filiacion
        private Visibility mFiliacionMV = Visibility.Visible;
        public Visibility MFiliacionMV
        {
            get { return mFiliacionMV; }
            set { mFiliacionMV = value; OnPropertyChanged("MFiliacionMV"); }
        }

        private Visibility mMediaFiliacionV = Visibility.Visible;
        public Visibility MMediaFiliacionV
        {
            get { return mMediaFiliacionV; }
            set { mMediaFiliacionV = value; OnPropertyChanged("MMediaFiliacionV"); }
        }

        private Visibility mTipoFiliacionV = Visibility.Visible;
        public Visibility MTipoFiliacionV
        {
            get { return mTipoFiliacionV; }
            set { mTipoFiliacionV = value; OnPropertyChanged("MTipoFiliacionV"); }
        }
        #endregion
        private Visibility mEscolaridadV = Visibility.Visible;
        public Visibility MEscolaridadV
        {
            get { return mEscolaridadV; }
            set { mEscolaridadV = value; OnPropertyChanged("MEscolaridadV"); }
        }

        private Visibility mReligionV = Visibility.Visible;
        public Visibility MReligionV
        {
            get { return mReligionV; }
            set { mReligionV = value; OnPropertyChanged("MReligionV"); }
        }

        private Visibility mTipoSangreV = Visibility.Visible;
        public Visibility MTipoSangreV
        {
            get { return mTipoSangreV; }
            set { mTipoSangreV = value; OnPropertyChanged("MTipoSangreV"); }
        }

        private Visibility mOcupacionV = Visibility.Visible;
        public Visibility MOcupacionV
        {
            get { return mOcupacionV; }
            set { mOcupacionV = value; OnPropertyChanged("MOcupacionV"); }
        }

        private Visibility mEtniaV = Visibility.Visible;
        public Visibility MEtniaV
        {
            get { return mEtniaV; }
            set { mEtniaV = value; OnPropertyChanged("MEtniaV"); }
        }

        private Visibility mPandillaV = Visibility.Visible;
        public Visibility MPandillaV
        {
            get { return mPandillaV; }
            set { mPandillaV = value; OnPropertyChanged("MPandillaV"); }
        }

        private Visibility mComportamientoHomosexualV = Visibility.Visible;
        public Visibility MComportamientoHomosexualV
        {
            get { return mComportamientoHomosexualV; }
            set { mComportamientoHomosexualV = value; OnPropertyChanged("MComportamientoHomosexualV"); }
        }

        private Visibility mTatuajesV = Visibility.Visible;
        public Visibility MTatuajesV
        {
            get { return mTatuajesV; }
            set { mTatuajesV = value; OnPropertyChanged("MTatuajesV"); }
        }

        private Visibility mCicatricesV = Visibility.Visible;
        public Visibility MCicatricesV
        {
            get { return mCicatricesV; }
            set { mCicatricesV = value; OnPropertyChanged("MCicatricesV"); }
        }

        private Visibility mDefectosV = Visibility.Visible;
        public Visibility MDefectosV
        {
            get { return mDefectosV; }
            set { mDefectosV = value; OnPropertyChanged("MDefectosV"); }
        }

        private Visibility mLunaresV = Visibility.Visible;
        public Visibility MLunaresV
        {
            get { return mLunaresV; }
            set { mLunaresV = value; OnPropertyChanged("MLunaresV"); }
        }

        private Visibility mTipoDiscapacidadV = Visibility.Visible;
        public Visibility MTipoDiscapacidadV
        {
            get { return mTipoDiscapacidadV; }
            set { mTipoDiscapacidadV = value; OnPropertyChanged("MTipoDiscapacidadV"); }
        }
        #endregion

        private Visibility mTipoAbogadoV = Visibility.Visible;
        public Visibility MTipoAbogadoV
        {
            get { return mTipoAbogadoV; }
            set { mTipoAbogadoV = value; OnPropertyChanged("MTipoAbogadoV"); }
        }

        private Visibility mCatParametroV = Visibility.Visible;
        public Visibility MCatParametroV
        {
            get { return mCatParametroV; }
            set { mCatParametroV = value; OnPropertyChanged("MCatParametroV"); }
        }

        #region Institucion
        private Visibility mInstitucionMV = Visibility.Visible;
        public Visibility MInstitucionMV
        {
            get { return mInstitucionMV; }
            set { mInstitucionMV = value; OnPropertyChanged("MInstitucionMV"); }
        }

        private Visibility mPaisesV = Visibility.Visible;
        public Visibility MPaisesV
        {
            get { return mPaisesV; }
            set { mPaisesV = value; OnPropertyChanged("MPaisesV"); }
        }

        private Visibility mEstadosV = Visibility.Visible;
        public Visibility MEstadosV
        {
            get { return mEstadosV; }
            set { mEstadosV = value; OnPropertyChanged("MEstadosV"); }
        }

        private Visibility mMunicipiosV = Visibility.Visible;
        public Visibility MMunicipiosV
        {
            get { return mMunicipiosV; }
            set { mMunicipiosV = value; OnPropertyChanged("MMunicipiosV"); }
        }

        private Visibility mColoniasV = Visibility.Visible;
        public Visibility MColoniasV
        {
            get { return mColoniasV; }
            set { mColoniasV = value; OnPropertyChanged("MColoniasV"); }
        }
        #region CERESOS
        private Visibility mCeresosMV = Visibility.Visible;
        public Visibility MCeresosMV
        {
            get { return mCeresosMV; }
            set { mCeresosMV = value; OnPropertyChanged("MCeresosMV"); }
        }

        private Visibility mCentrosV = Visibility.Visible;
        public Visibility MCentrosV
        {
            get { return mCentrosV; }
            set { mCentrosV = value; OnPropertyChanged("MCentrosV"); }
        }

        private Visibility mEdificiosV = Visibility.Visible;
        public Visibility MEdificiosV
        {
            get { return mEdificiosV; }
            set { mEdificiosV = value; OnPropertyChanged("MEdificiosV"); }
        }

        private Visibility mSectoresV = Visibility.Visible;
        public Visibility MSectoresV
        {
            get { return mSectoresV; }
            set { mSectoresV = value; OnPropertyChanged("MSectoresV"); }
        }

        private Visibility mCeldasV = Visibility.Visible;
        public Visibility MCeldasV
        {
            get { return mCeldasV; }
            set { mCeldasV = value; OnPropertyChanged("MCeldasV"); }
        }

        private Visibility mCamasV = Visibility.Visible;
        public Visibility MCamasV
        {
            get { return mCamasV; }
            set { mCamasV = value; OnPropertyChanged("MCamasV"); }
        }
        #endregion
        private Visibility mAgenciasV = Visibility.Visible;
        public Visibility MAgenciasV
        {
            get { return mAgenciasV; }
            set { mAgenciasV = value; OnPropertyChanged("MAgenciasV"); }
        }

        private Visibility mJuzgadosV = Visibility.Visible;
        public Visibility MJuzgadosV
        {
            get { return mJuzgadosV; }
            set { mJuzgadosV = value; OnPropertyChanged("MJuzgadosV"); }
        }
        #endregion

        #region Organizacion Interna
        private Visibility mOrganizacionInternaMV = Visibility.Visible;
        public Visibility MOrganizacionInternaMV
        {
            get { return mOrganizacionInternaMV; }
            set { mOrganizacionInternaMV = value; OnPropertyChanged("MOrganizacionInternaMV"); }
        }

        private Visibility mDepartamentosV = Visibility.Visible;
        public Visibility MDepartamentosV
        {
            get { return mDepartamentosV; }
            set { mDepartamentosV = value; OnPropertyChanged("MDepartamentosV"); }
        }

        private Visibility mProgramaRehabilitacionV = Visibility.Visible;
        public Visibility MProgramaRehabilitacionV
        {
            get { return mProgramaRehabilitacionV; }
            set { mProgramaRehabilitacionV = value; OnPropertyChanged("MProgramaRehabilitacionV"); }
        }

        private Visibility mAreasV = Visibility.Visible;
        public Visibility MAreasV
        {
            get { return mAreasV; }
            set { mAreasV = value; OnPropertyChanged("MAreasV"); }
        }

        private Visibility mTipoActividadesV = Visibility.Visible;
        public Visibility MTipoActividadesV
        {
            get { return mTipoActividadesV; }
            set { mTipoActividadesV = value; OnPropertyChanged("MTipoActividadesV"); }
        }

        private Visibility mActividadesV = Visibility.Visible;
        public Visibility MActividadesV
        {
            get { return mActividadesV; }
            set { mActividadesV = value; OnPropertyChanged("MActividadesV"); }
        }

        private Visibility mEjesV = Visibility.Visible;
        public Visibility MEjesV
        {
            get { return mEjesV; }
            set { mEjesV = value; OnPropertyChanged("MEjesV"); }
        }
        #endregion

        #region Legal
        private Visibility mLegalMV = Visibility.Visible;
        public Visibility MLegalMV
        {
            get { return mLegalMV; }
            set { mLegalMV = value; OnPropertyChanged("MLegalMV"); }
        }

        private Visibility mDelitoV = Visibility.Visible;
        public Visibility MDelitoV
        {
            get { return mDelitoV; }
            set { mDelitoV = value; OnPropertyChanged("MDelitoV"); }
        }

        private Visibility mAutoTerminoV = Visibility.Visible;
        public Visibility MAutoTerminoV
        {
            get { return mAutoTerminoV; }
            set { mAutoTerminoV = value; OnPropertyChanged("MAutoTerminoV"); }
        }

        private Visibility mTiposRecursosV = Visibility.Visible;
        public Visibility MTiposRecursosV
        {
            get { return mTiposRecursosV; }
            set { mTiposRecursosV = value; OnPropertyChanged("MTiposRecursosV"); }
        }

        private Visibility mTipoIngresoV = Visibility.Visible;
        public Visibility MTipoIngresoV
        {
            get { return mTipoIngresoV; }
            set { mTipoIngresoV = value; OnPropertyChanged("MTipoIngresoV"); }
        }
        #endregion

        #region Estudio
        private Visibility mEstudioMV = Visibility.Visible;
        public Visibility MEstudioMV
        {
            get { return mEstudioMV; }
            set { mEstudioMV = value; OnPropertyChanged("MEstudioMV"); }
        }

        private Visibility mTipoEstudioV = Visibility.Visible;
        public Visibility MTipoEstudioV
        {
            get { return mTipoEstudioV; }
            set { mTipoEstudioV = value; OnPropertyChanged("MTipoEstudioV"); }
        }

        private Visibility mMotivoSolicitudEstudioV = Visibility.Visible;
        public Visibility MMotivoSolicitudEstudioV
        {
            get { return mMotivoSolicitudEstudioV; }
            set { mMotivoSolicitudEstudioV = value; OnPropertyChanged("MMotivoSolicitudEstudioV"); }
        }
        #endregion

        #region Visitas
        private Visibility mVisitasCMV = Visibility.Visible;
        public Visibility MVisitasCMV
        {
            get { return mVisitasCMV; }
            set { mVisitasCMV = value; OnPropertyChanged("MVisitasCMV"); }
        }

        private Visibility mTipoVisitaV = Visibility.Visible;
        public Visibility MTipoVisitaV
        {
            get { return mTipoVisitaV; }
            set { mTipoVisitaV = value; OnPropertyChanged("MTipoVisitaV"); }
        }

        private Visibility mRelacionV = Visibility.Visible;
        public Visibility MRelacionV
        {
            get { return mRelacionV; }
            set { mRelacionV = value; OnPropertyChanged("MRelacionV"); }
        }
        #endregion

        #region Decomisos
        private Visibility mDecomisosMV = Visibility.Visible;
        public Visibility MDecomisosMV
        {
            get { return mDecomisosMV; }
            set { mDecomisosMV = value; OnPropertyChanged("MDecomisosMV"); }
        }

        private Visibility mMarcasModelosV = Visibility.Visible;
        public Visibility MMarcasModelosV
        {
            get { return mMarcasModelosV; }
            set { mMarcasModelosV = value; OnPropertyChanged("MMarcasModelosV"); }
        }

        private Visibility mFabricanteModeloV = Visibility.Visible;
        public Visibility MFabricanteModeloV
        {
            get { return mFabricanteModeloV; }
            set { mFabricanteModeloV = value; OnPropertyChanged("MFabricanteModeloV"); }
        }

        private Visibility mCompaniasV = Visibility.Visible;
        public Visibility MCompaniasV
        {
            get { return mCompaniasV; }
            set { mCompaniasV = value; OnPropertyChanged("MCompaniasV"); }
        }

        private Visibility mUnidadMedidaV = Visibility.Visible;
        public Visibility MUnidadMedidaV
        {
            get { return mUnidadMedidaV; }
            set { mUnidadMedidaV = value; OnPropertyChanged("MUnidadMedidaV"); }
        }

        private Visibility mObjetoV = Visibility.Visible;
        public Visibility MObjetoV
        {
            get { return mObjetoV; }
            set { mObjetoV = value; OnPropertyChanged("MObjetoV"); }
        }

        private Visibility mGruposPolicialesV = Visibility.Visible;
        public Visibility MGruposPolicialesV
        {
            get { return mGruposPolicialesV; }
            set { mGruposPolicialesV = value; OnPropertyChanged("MGruposPolicialesV"); }
        }
        #endregion

        #region Planimetria
        private Visibility mPlanimetriaMV = Visibility.Visible;
        public Visibility MPlanimetriaMV
        {
            get { return mPlanimetriaMV; }
            set { mPlanimetriaMV = value; OnPropertyChanged("MPlanimetriaMV"); }
        }

        private Visibility mSectorClasificacionV = Visibility.Visible;
        public Visibility MSectorClasificacionV
        {
            get { return mSectorClasificacionV; }
            set { mSectorClasificacionV = value; OnPropertyChanged("MSectorClasificacionV"); }
        }
        #endregion

        #region Notificaciones
        private Visibility mNotificacionesMV = Visibility.Visible;
        public Visibility MNotificacionesMV
        {
            get { return mNotificacionesMV; }
            set { mNotificacionesMV = value; OnPropertyChanged("MNotificacionesMV"); }
        }

        private Visibility mTipoMensajeV = Visibility.Visible;
        public Visibility MTipoMensajeV
        {
            get { return mTipoMensajeV; }
            set { mTipoMensajeV = value; OnPropertyChanged("MTipoMensajeV"); }
        }
        #endregion

        #region Causa Penal
        private Visibility mCausaPenalCMV = Visibility.Visible;
        public Visibility MCausaPenalCMV
        {
            get { return mCausaPenalCMV; }
            set { mCausaPenalCMV = value; OnPropertyChanged("MCausaPenalCMV"); }
        }

        private Visibility mTipoAmparoIndirectoV = Visibility.Visible;
        public Visibility MTipoAmparoIndirectoV
        {
            get { return mTipoAmparoIndirectoV; }
            set { mTipoAmparoIndirectoV = value; OnPropertyChanged("MTipoAmparoIndirectoV"); }
        }

        private Visibility mTipoIncidenteV = Visibility.Visible;
        public Visibility MTipoIncidenteV
        {
            get { return mTipoIncidenteV; }
            set { mTipoIncidenteV = value; OnPropertyChanged("MTipoIncidenteV"); }
        }
        #endregion

        private Visibility mConsultaUnificadaCV = Visibility.Visible;
        public Visibility MConsultaUnificadaCV
        {
            get { return mConsultaUnificadaCV; }
            set { mConsultaUnificadaCV = value; OnPropertyChanged("MConsultaUnificadaCV"); }
        }

        private Visibility mYardasV = Visibility.Visible;
        public Visibility MYardasV
        {
            get { return mYardasV; }
            set { mYardasV = value; OnPropertyChanged("MYardasV"); }
        }
        #endregion

        #region Configurar
        #region Seguridad
        private Visibility mSeguridadMV = Visibility.Visible;
        public Visibility MSeguridadMV
        {
            get { return mSeguridadMV; }
            set { mSeguridadMV = value; OnPropertyChanged("MSeguridadMV"); }
        }

        private Visibility mPrivilegiosV = Visibility.Visible;
        public Visibility MPrivilegiosV
        {
            get { return mPrivilegiosV; }
            set { mPrivilegiosV = value; OnPropertyChanged("MPrivilegiosV"); }
        }

        private Visibility mActivacionCuentaV = Visibility.Visible;
        public Visibility MActivacionCuentaV
        {
            get { return mActivacionCuentaV; }
            set { mActivacionCuentaV = value; OnPropertyChanged("MActivacionCuentaV"); }
        }

        private Visibility mCambioClaveAccesoV = Visibility.Visible;
        public Visibility MCambioClaveAccesoV
        {
            get { return mCambioClaveAccesoV; }
            set { mCambioClaveAccesoV = value; OnPropertyChanged("MCambioClaveAccesoV"); }
        }

        private Visibility mEquiposAutorizadosV = Visibility.Visible;
        public Visibility MEquiposAutorizadosV
        {
            get { return mEquiposAutorizadosV; }
            set { mEquiposAutorizadosV = value; OnPropertyChanged("MEquiposAutorizadosV"); }
        }
        #endregion
        private Visibility mOpcionesGeneralesV = Visibility.Visible;
        public Visibility MOpcionesGeneralesV
        {
            get { return mOpcionesGeneralesV; }
            set { mOpcionesGeneralesV = value; OnPropertyChanged("MOpcionesGeneralesV"); }
        }

        private Visibility mEquipoAreaV = Visibility.Visible;
        public Visibility MEquipoAreaV
        {
            get { return mEquipoAreaV; }
            set { mEquipoAreaV = value; OnPropertyChanged("MEquipoAreaV"); }
        }

        private Visibility mConfigurarDepartamentosV = Visibility.Visible;
        public Visibility MConfigurarDepartamentosV
        {
            get { return mConfigurarDepartamentosV; }
            set { mConfigurarDepartamentosV = value; OnPropertyChanged("MConfigurarDepartamentosV"); }
        }

        private Visibility mConfigurarDepartamentosAreaTecnicaV = Visibility.Visible;
        public Visibility MConfigurarDepartamentosAreaTecnicaV
        {
            get { return mConfigurarDepartamentosAreaTecnicaV; }
            set { mConfigurarDepartamentosAreaTecnicaV = value; OnPropertyChanged("MConfigurarDepartamentosAreaTecnicaV"); }
        }
        #endregion

        #region Enabled
        private bool mRegistro = false;
        public bool MRegistro
        {
            get { return mRegistro; }
            set { mRegistro = value; OnPropertyChanged("MRegistro"); }
        }

        private bool mDecomiso = false;
        public bool MDecomiso
        {
            get { return mDecomiso; }
            set { mDecomiso = value; OnPropertyChanged("MDecomiso"); }
        }

        private bool mCorrespondencia = false;
        public bool MCorrespondencia
        {
            get { return mCorrespondencia; }
            set { mCorrespondencia = value; OnPropertyChanged("MCorrespondencia"); }
        }

        private bool mCausaPenal = false;
        public bool MCausaPenal
        {
            get { return mCausaPenal; }
            set { mCausaPenal = value; OnPropertyChanged("MCausaPenal"); }
        }

        private bool mIdentificacion = false;
        public bool MIdentificacion
        {
            get { return mIdentificacion; }
            set { mIdentificacion = value; OnPropertyChanged("MIdentificacion"); }
        }

        private bool mEstatusAdministrativo = false;
        public bool MEstatusAdministrativo
        {
            get { return mEstatusAdministrativo; }
            set { mEstatusAdministrativo = value; OnPropertyChanged("MEstatusAdministrativo"); }
        }

        private bool mCreacionListas = false;
        public bool MCreacionListas
        {
            get { return mCreacionListas; }
            set { mCreacionListas = value; OnPropertyChanged("MCreacionListas"); }
        }

        private bool mPertenencias = false;
        public bool MPertenencias
        {
            get { return mPertenencias; }
            set { mPertenencias = value; OnPropertyChanged("MPertenencias"); }
        }

        private bool mUbicacionEstancia = false;
        public bool MUbicacionEstancia
        {
            get { return mUbicacionEstancia; }
            set { mUbicacionEstancia = value; OnPropertyChanged("MUbicacionEstancia"); }
        }

        private bool mIncidentes = false;
        public bool MIncidentes
        {
            get { return mIncidentes; }
            set { mIncidentes = value; OnPropertyChanged("MIncidentes"); }
        }

        private bool mEMI = false;
        public bool MEMI
        {
            get { return mEMI; }
            set { mEMI = value; OnPropertyChanged("MEMI"); }
        }

        private bool mEntrevistaInicialTS = false;
        public bool MEntrevistaInicialTS
        {
            get { return mEntrevistaInicialTS; }
            set { mEntrevistaInicialTS = value; OnPropertyChanged("MEntrevistaInicialTS"); }
        }

        private bool mEstudioSocioEconomico = false;
        public bool MEstudioSocioEconomico
        {
            get { return mEstudioSocioEconomico; }
            set { mEstudioSocioEconomico = value; OnPropertyChanged("MEstudioSocioEconomico"); }
        }

        private bool mProgramacionEstudios = false;
        public bool MProgramacionEstudios
        {
            get { return mProgramacionEstudios; }
            set { mProgramacionEstudios = value; OnPropertyChanged("MProgramacionEstudios"); }
        }

        private bool mCierreEstudio = false;
        public bool MCierreEstudio
        {
            get { return mCierreEstudio; }
            set { mCierreEstudio = value; OnPropertyChanged("MCierreEstudio"); }
        }

        private bool mRealizacionEstudio = false;
        public bool MRealizacionEstudio
        {
            get { return mRealizacionEstudio; }
            set { mRealizacionEstudio = value; OnPropertyChanged("MRealizacionEstudio"); }
        }

        private bool mControlSanciones = false;
        public bool MControlSanciones
        {
            get { return mControlSanciones; }
            set { mControlSanciones = value; OnPropertyChanged("MControlSanciones"); }
        }

        private bool mCreacionGrupos = false;
        public bool MCreacionGrupos
        {
            get { return mCreacionGrupos; }
            set { mCreacionGrupos = value; OnPropertyChanged("MCreacionGrupos"); }
        }

        private bool mManejoGrupos = false;
        public bool MManejoGrupos
        {
            get { return mManejoGrupos; }
            set { mManejoGrupos = value; OnPropertyChanged("MManejoGrupos"); }
        }

        private bool mManejoEmpalmes = false;
        public bool MManejoEmpalmes
        {
            get { return mManejoEmpalmes; }
            set { mManejoEmpalmes = value; OnPropertyChanged("MManejoEmpalmes"); }
        }

        private bool mManejoCanceladosSuspendidos = false;
        public bool MManejoCanceladosSuspendidos
        {
            get { return mManejoCanceladosSuspendidos; }
            set { mManejoCanceladosSuspendidos = value; OnPropertyChanged("MManejoCanceladosSuspendidos"); }
        }

        private bool mControlCalificaciones = false;
        public bool MControlCalificaciones
        {
            get { return mControlCalificaciones; }
            set { mControlCalificaciones = value; OnPropertyChanged("MControlCalificaciones"); }
        }

        private bool mControlParticipantes = false;
        public bool MControlParticipantes
        {
            get { return mControlParticipantes; }
            set { mControlParticipantes = value; OnPropertyChanged("MControlParticipantes"); }
        }

        private bool mProgramacionEventos = false;
        public bool MProgramacionEventos
        {
            get { return mProgramacionEventos; }
            set { mProgramacionEventos = value; OnPropertyChanged("MProgramacionEventos"); }
        }

        private bool mHistoriaClinica = false;
        public bool MHistoriaClinica
        {
            get { return mHistoriaClinica; }
            set { mHistoriaClinica = value; OnPropertyChanged("MHistoriaClinica"); }
        }

        private bool mAgendaMedica = false;
        public bool MAgendaMedica
        {
            get { return mAgendaMedica; }
            set { mAgendaMedica = value; OnPropertyChanged("MAgendaMedica"); }
        }

        private bool mAgendaEnfermero = false;
        public bool MAgendaEnfermero
        {
            get { return mAgendaEnfermero; }
            set { mAgendaEnfermero = value; OnPropertyChanged("MAgendaEnfermero"); }
        }

        private bool mNotaMedica = false;
        public bool MNotaMedica
        {
            get { return mNotaMedica; }
            set { mNotaMedica = value; OnPropertyChanged("MNotaMedica"); }
        }

        private bool mNotaMedicaEspecialista = false;
        public bool MNotaMedicaEspecialista
        {
            get { return mNotaMedicaEspecialista; }
            set { mNotaMedicaEspecialista = value; OnPropertyChanged("MNotaMedicaEspecialista"); }
        }

        private bool mNotaEvolucion = false;
        public bool MNotaEvolucion
        {
            get { return mNotaEvolucion; }
            set { mNotaEvolucion = value; OnPropertyChanged("MNotaEvolucion"); }
        }

        private bool mCertificadoMedico = false;
        public bool MCertificadoMedico
        {
            get { return mCertificadoMedico; }
            set { mCertificadoMedico = value; OnPropertyChanged("MCertificadoMedico"); }
        }

        private bool mSolicitudCanalizacion = false;
        public bool MSolicitudCanalizacion
        {
            get { return mSolicitudCanalizacion; }
            set { mSolicitudCanalizacion = value; OnPropertyChanged("MSolicitudCanalizacion"); }
        }

        private bool mAtencionCanalizacion = false;
        public bool MAtencionCanalizacion
        {
            get { return mAtencionCanalizacion; }
            set { mAtencionCanalizacion = value; OnPropertyChanged("MAtencionCanalizacion"); }
        }

        private bool mResultadoServiciosAuxiliares = false;
        public bool MResultadoServiciosAuxiliares
        {
            get { return mResultadoServiciosAuxiliares; }
            set { mResultadoServiciosAuxiliares = value; OnPropertyChanged("MResultadoServiciosAuxiliares"); }
        }

        private bool mAgendaEspecialista = false;
        public bool MAgendaEspecialista
        {
            get { return mAgendaEspecialista; }
            set { mAgendaEspecialista = value; OnPropertyChanged("MAgendaEspecialista"); }
        }

        private bool mTarjetaInformativaDeceso = false;
        public bool MTarjetaInformativaDeceso
        {
            get { return mTarjetaInformativaDeceso; }
            set { mTarjetaInformativaDeceso = value; OnPropertyChanged("MTarjetaInformativaDeceso"); }
        }

        private bool mBitacoraEspecializacion = false;
        public bool MBitacoraEspecializacion
        {
            get { return mBitacoraEspecializacion; }
            set { mBitacoraEspecializacion = value; OnPropertyChanged("MBitacoraEspecializacion"); }
        }

        private bool mNotificacionTrabajoSocial = false;
        public bool MNotificacionTrabajoSocial
        {
            get { return mNotificacionTrabajoSocial; }
            set { mNotificacionTrabajoSocial = value; OnPropertyChanged("MNotificacionTrabajoSocial"); }
        }

        private bool mSolicitudCita = false;
        public bool MSolicitudCita
        {
            get { return mSolicitudCita; }
            set { mSolicitudCita = value; OnPropertyChanged("MSolicitudCita"); }
        }

        private bool mAgendarCita = false;
        public bool MAgendarCita
        {
            get { return mAgendarCita; }
            set { mAgendarCita = value; OnPropertyChanged("MAgendarCita"); }
        }

        private bool mAtenderCita = false;
        public bool MAtenderCita
        {
            get { return mAtenderCita; }
            set { mAtenderCita = value; OnPropertyChanged("MAtenderCita"); }
        }

        private bool mNotaTecnica = false;
        public bool MNotaTecnica
        {
            get { return mNotaTecnica; }
            set { mNotaTecnica = value; OnPropertyChanged("MNotaTecnica"); }
        }

        private bool mHistorialCitas = false;
        public bool MHistorialCitas
        {
            get { return mHistorialCitas; }
            set { mHistorialCitas = value; OnPropertyChanged("MHistorialCitas"); }
        }

        private bool mRegistroLiberados = false;
        public bool MRegistroLiberados
        {
            get { return mRegistroLiberados; }
            set { mRegistroLiberados = value; OnPropertyChanged("MRegistroLiberados"); }
        }

        private bool mEMILiberados = false;
        public bool MEMILiberados
        {
            get { return mEMILiberados; }
            set { mEMILiberados = value; OnPropertyChanged("MEMILiberados"); }
        }

        private bool mReportePsicologicoLiberado = false;
        public bool MReportePsicologicoLiberado
        {
            get { return mReportePsicologicoLiberado; }
            set { mReportePsicologicoLiberado = value; OnPropertyChanged("MReportePsicologicoLiberado"); }
        }

        private bool mEntrevistaInicialLiberado = false;
        public bool MEntrevistaInicialLiberado
        {
            get { return mEntrevistaInicialLiberado; }
            set { mEntrevistaInicialLiberado = value; OnPropertyChanged("MEntrevistaInicialLiberado"); }
        }

        private bool mVisitaDomiciliariaLiberado = false;
        public bool MVisitaDomiciliariaLiberado
        {
            get { return mVisitaDomiciliariaLiberado; }
            set { mVisitaDomiciliariaLiberado = value; OnPropertyChanged("MVisitaDomiciliariaLiberado"); }
        }

        private bool mSeguimientoLibertad = false;
        public bool MSeguimientoLibertad
        {
            get { return mSeguimientoLibertad; }
            set { mSeguimientoLibertad = value; OnPropertyChanged("MSeguimientoLibertad"); }
        }

        private bool mEscalaRiesgo = false;
        public bool MEscalaRiesgo
        {
            get { return mEscalaRiesgo; }
            set { mEscalaRiesgo = value; OnPropertyChanged("MEscalaRiesgo"); }
        }


        private bool mPadronVisitaFamiliar = false;
        public bool MPadronVisitaFamiliar
        {
            get { return mPadronVisitaFamiliar; }
            set { mPadronVisitaFamiliar = value; OnPropertyChanged("MPadronVisitaFamiliar"); }
        }

        private bool mVerificarPases = false;
        public bool MVerificarPases
        {
            get { return mVerificarPases; }
            set { mVerificarPases = value; OnPropertyChanged("MVerificarPases"); }
        }

        private bool mCancelarCredenciales = false;
        public bool MCancelarCredenciales
        {
            get { return mCancelarCredenciales; }
            set { mCancelarCredenciales = value; OnPropertyChanged("MCancelarCredenciales"); }
        }

        private bool mPadronAbogados = false;
        public bool MPadronAbogados
        {
            get { return mPadronAbogados; }
            set { mPadronAbogados = value; OnPropertyChanged("MPadronAbogados"); }
        }

        private bool mPadronColaboradores = false;
        public bool MPadronColaboradores
        {
            get { return mPadronColaboradores; }
            set { mPadronColaboradores = value; OnPropertyChanged("MPadronColaboradores"); }
        }

        private bool mPadronActuarios = false;
        public bool MPadronActuarios
        {
            get { return mPadronActuarios; }
            set { mPadronActuarios = value; OnPropertyChanged("MPadronActuarios"); }
        }

        private bool mRequerimientoInternos = false;
        public bool MRequerimientoInternos
        {
            get { return mRequerimientoInternos; }
            set { mRequerimientoInternos = value; OnPropertyChanged("MRequerimientoInternos"); }
        }

        private bool mPadronVisitaExterna = false;
        public bool MPadronVisitaExterna
        {
            get { return mPadronVisitaExterna; }
            set { mPadronVisitaExterna = value; OnPropertyChanged("MPadronVisitaExterna"); }
        }

        private bool mVisitasPorEdificio = false;
        public bool MVisitasPorEdificio
        {
            get { return mVisitasPorEdificio; }
            set { mVisitasPorEdificio = value; OnPropertyChanged("MVisitasPorEdificio"); }
        }

        private bool mVisitasPorApellido = false;
        public bool MVisitasPorApellido
        {
            get { return mVisitasPorApellido; }
            set { mVisitasPorApellido = value; OnPropertyChanged("MVisitasPorApellido"); }
        }

        private bool mPadronPersonal = false;
        public bool MPadronPersonal
        {
            get { return mPadronPersonal; }
            set { mPadronPersonal = value; OnPropertyChanged("MPadronPersonal"); }
        }

        private bool mAccesoAduana = false;
        public bool MAccesoAduana
        {
            get { return mAccesoAduana; }
            set { mAccesoAduana = value; OnPropertyChanged("MAccesoAduana"); }
        }

        private bool mInternosPorEdificio = false;
        public bool MInternosPorEdificio
        {
            get { return mInternosPorEdificio; }
            set { mInternosPorEdificio = value; OnPropertyChanged("MInternosPorEdificio"); }
        }

        private bool mProgramas = false;
        public bool MProgramas
        {
            get { return mProgramas; }
            set { mProgramas = value; OnPropertyChanged("MProgramas"); }
        }

        private bool mActividadesNoProgramadas = false;
        public bool MActividadesNoProgramadas
        {
            get { return mActividadesNoProgramadas; }
            set { mActividadesNoProgramadas = value; OnPropertyChanged("MActividadesNoProgramadas"); }
        }

        private bool mVisitasLegales = false;
        public bool MVisitasLegales
        {
            get { return mVisitasLegales; }
            set { mVisitasLegales = value; OnPropertyChanged("MVisitasLegales"); }
        }

        private bool mExcarcelacion = false;
        public bool MExcarcelacion
        {
            get { return mExcarcelacion; }
            set { mExcarcelacion = value; OnPropertyChanged("MExcarcelacion"); }
        }

        private bool mAutorizarExcarcelacion = false;
        public bool MAutorizarExcarcelacion
        {
            get { return mAutorizarExcarcelacion; }
            set { mAutorizarExcarcelacion = value; OnPropertyChanged("MAutorizarExcarcelacion"); }
        }

        private bool mTrasladoForaneo = false;
        public bool MTrasladoForaneo
        {
            get { return mTrasladoForaneo; }
            set { mTrasladoForaneo = value; OnPropertyChanged("MTrasladoForaneo"); }
        }

        private bool mTrasladoMasivo = false;
        public bool MTrasladoMasivo
        {
            get { return mTrasladoMasivo; }
            set { mTrasladoMasivo = value; OnPropertyChanged("MTrasladoMasivo"); }
        }

        private bool mAutorizaIngresoTraslado = false;
        public bool MAutorizaIngresoTraslado
        {
            get { return mAutorizaIngresoTraslado; }
            set { mAutorizaIngresoTraslado = value; OnPropertyChanged("MAutorizaIngresoTraslado"); }
        }

        private bool mPreBaja = false;
        public bool MPreBaja
        {
            get { return mPreBaja; }
            set { mPreBaja = value; OnPropertyChanged("MPreBaja"); }
        }

        private bool mLiberacionBiometria = false;
        public bool MLiberacionBiometria
        {
            get { return mLiberacionBiometria; }
            set { mLiberacionBiometria = value; OnPropertyChanged("MLiberacionBiometria"); }
        }

        private bool mConsultaUnificada = false;
        public bool MConsultaUnificada
        {
            get { return mConsultaUnificada; }
            set { mConsultaUnificada = value; OnPropertyChanged("MConsultaUnificada"); }
        }

        private bool mPlanimetria = false;
        public bool MPlanimetria
        {
            get { return mPlanimetria; }
            set { mPlanimetria = value; OnPropertyChanged("MPlanimetria"); }
        }

        private bool mReporteListadoGeneral = false;
        public bool MReporteListadoGeneral
        {
            get { return mReporteListadoGeneral; }
            set { mReporteListadoGeneral = value; OnPropertyChanged("MReporteListadoGeneral"); }
        }

        private bool mReporteGafetes = false;
        public bool MReporteGafetes
        {
            get { return mReporteGafetes; }
            set { mReporteGafetes = value; OnPropertyChanged("MReporteGafetes"); }
        }

        private bool mReporteCredencialBiblioteca = false;
        public bool MReporteCredencialBiblioteca
        {
            get { return mReporteCredencialBiblioteca; }
            set { mReporteCredencialBiblioteca = value; OnPropertyChanged("MReporteCredencialBiblioteca"); }
        }

        private bool mReporteImpresionLista = false;
        public bool MReporteImpresionLista
        {
            get { return mReporteImpresionLista; }
            set { mReporteImpresionLista = value; OnPropertyChanged("MReporteImpresionLista"); }
        }

        private bool mReporteSituacionJuridica = false;
        public bool MReporteSituacionJuridica
        {
            get { return mReporteSituacionJuridica; }
            set { mReporteSituacionJuridica = value; OnPropertyChanged("MReporteSituacionJuridica"); }
        }

        private bool mReporteControlInternosEdificio = false;
        public bool MReporteControlInternosEdificio
        {
            get { return mReporteControlInternosEdificio; }
            set { mReporteControlInternosEdificio = value; OnPropertyChanged("MReporteControlInternosEdificio"); }
        }

        private bool mReporteInternosPorUbicacion = false;
        public bool MReporteInternosPorUbicacion
        {
            get { return mReporteInternosPorUbicacion; }
            set { mReporteInternosPorUbicacion = value; OnPropertyChanged("MReporteInternosPorUbicacion"); }
        }

        private bool mReporteInternosReubicacion = false;
        public bool MReporteInternosReubicacion
        {
            get { return mReporteInternosReubicacion; }
            set { mReporteInternosReubicacion = value; OnPropertyChanged("MReporteInternosReubicacion"); }
        }

        private bool _MReporteNotasMedicasEnable = false;
        public bool MReporteNotasMedicasEnable
        {
            get { return _MReporteNotasMedicasEnable; }
            set { _MReporteNotasMedicasEnable = value; OnPropertyChanged("MReporteNotasMedicasEnable"); }
        }

        private bool mReportePapeletas = false;
        public bool MReportePapeletas
        {
            get { return mReportePapeletas; }
            set { mReportePapeletas = value; OnPropertyChanged("MReportePapeletas"); }
        }

        private bool mReporteAltasBajas = false;
        public bool MReporteAltasBajas
        {
            get { return mReporteAltasBajas; }
            set { mReporteAltasBajas = value; OnPropertyChanged("MReporteAltasBajas"); }
        }

        private bool mReporteTrasladosEstatales = false;
        public bool MReporteTrasladosEstatales
        {
            get { return mReporteTrasladosEstatales; }
            set { mReporteTrasladosEstatales = value; OnPropertyChanged("MReporteTrasladosEstatales"); }
        }

        private bool mReporteListadoGeneralDelito = false;
        public bool MReporteListadoGeneralDelito
        {
            get { return mReporteListadoGeneralDelito; }
            set { mReporteListadoGeneralDelito = value; OnPropertyChanged("MReporteListadoGeneralDelito"); }
        }

        private bool mReporteControlVisitantes = false;
        public bool MReporteControlVisitantes
        {
            get { return mReporteControlVisitantes; }
            set { mReporteControlVisitantes = value; OnPropertyChanged("MReporteControlVisitantes"); }
        }

        private bool mReportePoblacion = false;
        public bool MReportePoblacion
        {
            get { return mReportePoblacion; }
            set { mReportePoblacion = value; OnPropertyChanged("MReportePoblacion"); }
        }

        private bool mReportePoblacionAltasBajas = false;
        public bool MReportePoblacionAltasBajas
        {
            get { return mReportePoblacionAltasBajas; }
            set { mReportePoblacionAltasBajas = value; OnPropertyChanged("MReportePoblacionAltasBajas"); }
        }

        private bool mReportePoblacionPorDelitos = false;
        public bool MReportePoblacionPorDelitos
        {
            get { return mReportePoblacionPorDelitos; }
            set { mReportePoblacionPorDelitos = value; OnPropertyChanged("MReportePoblacionPorDelitos"); }
        }

        private bool mReportePoblacionEntidadProcedencia = false;
        public bool MReportePoblacionEntidadProcedencia
        {
            get { return mReportePoblacionEntidadProcedencia; }
            set { mReportePoblacionEntidadProcedencia = value; OnPropertyChanged("MReportePoblacionEntidadProcedencia"); }
        }

        private bool mReportePoblacionIndigena = false;
        public bool MReportePoblacionIndigena
        {
            get { return mReportePoblacionIndigena; }
            set { mReportePoblacionIndigena = value; OnPropertyChanged("MReportePoblacionIndigena"); }
        }

        private bool mReportePoblacionExtranjera = false;
        public bool MReportePoblacionExtranjera
        {
            get { return mReportePoblacionExtranjera; }
            set { mReportePoblacionExtranjera = value; OnPropertyChanged("MReportePoblacionExtranjera"); }
        }

        private bool mReporteActividades = false;
        public bool MReporteActividades
        {
            get { return mReporteActividades; }
            set { mReporteActividades = value; OnPropertyChanged("MReporteActividades"); }
        }

        private bool mReportePoblacionTerceraEdad = false;
        public bool MReportePoblacionTerceraEdad
        {
            get { return mReportePoblacionTerceraEdad; }
            set { mReportePoblacionTerceraEdad = value; OnPropertyChanged("MReportePoblacionTerceraEdad"); }
        }

        private bool mReportePoblacionTotalIngresos = false;
        public bool MReportePoblacionTotalIngresos
        {
            get { return mReportePoblacionTotalIngresos; }
            set { mReportePoblacionTotalIngresos = value; OnPropertyChanged("MReportePoblacionTotalIngresos"); }
        }

        private bool mReportePoblacionMotivosSalida = false;
        public bool MReportePoblacionMotivosSalida
        {
            get { return mReportePoblacionMotivosSalida; }
            set { mReportePoblacionMotivosSalida = value; OnPropertyChanged("MReportePoblacionMotivosSalida"); }
        }

        private bool mReportePoblacionActivaCierre = false;
        public bool MReportePoblacionActivaCierre
        {
            get { return mReportePoblacionActivaCierre; }
            set { mReportePoblacionActivaCierre = value; OnPropertyChanged("MReportePoblacionActivaCierre"); }
        }

        private bool mReportePrimeraVezFueroDelito = false;
        public bool MReportePrimeraVezFueroDelito
        {
            get { return mReportePrimeraVezFueroDelito; }
            set { mReportePrimeraVezFueroDelito = value; OnPropertyChanged("MReportePrimeraVezFueroDelito"); }
        }

        private bool mReporteSentenciadoSexoFueroDelito = false;
        public bool MReporteSentenciadoSexoFueroDelito
        {
            get { return mReporteSentenciadoSexoFueroDelito; }
            set { mReporteSentenciadoSexoFueroDelito = value; OnPropertyChanged("MReporteSentenciadoSexoFueroDelito"); }
        }

        private bool mReporteProcesadoSexoFueroDelito = false;
        public bool MReporteProcesadoSexoFueroDelito
        {
            get { return mReporteProcesadoSexoFueroDelito; }
            set { mReporteProcesadoSexoFueroDelito = value; OnPropertyChanged("MReporteProcesadoSexoFueroDelito"); }
        }

        private bool mReporteCNDH = false;
        public bool MReporteCNDH
        {
            get { return mReporteCNDH; }
            set { mReporteCNDH = value; OnPropertyChanged("MReporteCNDH"); }
        }

        private bool mReporteTiempoCompurgar = false;
        public bool MReporteTiempoCompurgar
        {
            get { return mReporteTiempoCompurgar; }
            set { mReporteTiempoCompurgar = value; OnPropertyChanged("MReporteTiempoCompurgar"); }
        }

        private bool mReporteCausaPenal = false;
        public bool MReporteCausaPenal
        {
            get { return mReporteCausaPenal; }
            set { mReporteCausaPenal = value; OnPropertyChanged("MReporteCausaPenal"); }
        }

        private bool mReporteBitacoraRegistroAbogado = false;
        public bool MReporteBitacoraRegistroAbogado
        {
            get { return mReporteBitacoraRegistroAbogado; }
            set { mReporteBitacoraRegistroAbogado = value; OnPropertyChanged("MReporteBitacoraRegistroAbogado"); }
        }

        private bool mReportePadronAbogados = false;
        public bool MReportePadronAbogados
        {
            get { return mReportePadronAbogados; }
            set { mReportePadronAbogados = value; OnPropertyChanged("MReportePadronAbogados"); }
        }

        private bool mReporteRecordVisitaFecha = false;
        public bool MReporteRecordVisitaFecha
        {
            get { return mReporteRecordVisitaFecha; }
            set { mReporteRecordVisitaFecha = value; OnPropertyChanged("MReporteRecordVisitaFecha"); }
        }

        private bool mReporteRecordVisitaIngreso = false;
        public bool MReporteRecordVisitaIngreso
        {
            get { return mReporteRecordVisitaIngreso; }
            set { mReporteRecordVisitaIngreso = value; OnPropertyChanged("MReporteRecordVisitaIngreso"); }
        }

        private bool mReporteAbogadosPoblacionAsignada = false;
        public bool MReporteAbogadosPoblacionAsignada
        {
            get { return mReporteAbogadosPoblacionAsignada; }
            set { mReporteAbogadosPoblacionAsignada = value; OnPropertyChanged("MReporteAbogadosPoblacionAsignada"); }
        }

        private bool mReporteRelacionAbogadoInterno = false;
        public bool MReporteRelacionAbogadoInterno
        {
            get { return mReporteRelacionAbogadoInterno; }
            set { mReporteRelacionAbogadoInterno = value; OnPropertyChanged("MReporteRelacionAbogadoInterno"); }
        }

        private bool mReporteKardexInterno = false;
        public bool MReporteKardexInterno
        {
            get { return mReporteKardexInterno; }
            set { mReporteKardexInterno = value; OnPropertyChanged("MReporteKardexInterno"); }
        }

        private bool mReporteInternosEnGrupos = false;
        public bool MReporteInternosEnGrupos
        {
            get { return mReporteInternosEnGrupos; }
            set { mReporteInternosEnGrupos = value; OnPropertyChanged("MReporteInternosEnGrupos"); }
        }

        private bool mReporteGruposActivos = false;
        public bool MReporteGruposActivos
        {
            get { return mReporteGruposActivos; }
            set { mReporteGruposActivos = value; OnPropertyChanged("MReporteGruposActivos"); }
        }

        private bool mReporteHorarioGrupo = false;
        public bool MReporteHorarioGrupo
        {
            get { return mReporteHorarioGrupo; }
            set { mReporteHorarioGrupo = value; OnPropertyChanged("MReporteHorarioGrupo"); }
        }

        private bool mReporteResponsableGrupo = false;
        public bool MReporteResponsableGrupo
        {
            get { return mReporteResponsableGrupo; }
            set { mReporteResponsableGrupo = value; OnPropertyChanged("MReporteResponsableGrupo"); }
        }

        private bool mReporteHorarioResponsableGrupo = false;
        public bool MReporteHorarioResponsableGrupo
        {
            get { return mReporteHorarioResponsableGrupo; }
            set { mReporteHorarioResponsableGrupo = value; OnPropertyChanged("MReporteHorarioResponsableGrupo"); }
        }

        private bool mReporteHorarioAreas = false;
        public bool MReporteHorarioAreas
        {
            get { return mReporteHorarioAreas; }
            set { mReporteHorarioAreas = value; OnPropertyChanged("MReporteHorarioAreas"); }
        }

        private bool mReporteEMIPendiente = false;
        public bool MReporteEMIPendiente
        {
            get { return mReporteEMIPendiente; }
            set { mReporteEMIPendiente = value; OnPropertyChanged("MReporteEMIPendiente"); }
        }

        private bool mReporteAsistenciaPadronEmpleado = false;
        public bool MReporteAsistenciaPadronEmpleado
        {
            get { return mReporteAsistenciaPadronEmpleado; }
            set { mReporteAsistenciaPadronEmpleado = value; OnPropertyChanged("MReporteAsistenciaPadronEmpleado"); }
        }

        private bool mReporteFormatoIdentificacion = false;
        public bool MReporteFormatoIdentificacion
        {
            get { return mReporteFormatoIdentificacion; }
            set { mReporteFormatoIdentificacion = value; OnPropertyChanged("MReporteFormatoIdentificacion"); }
        }

        private bool mReporteDecomisos = false;
        public bool MReporteDecomisos
        {
            get { return mReporteDecomisos; }
            set { mReporteDecomisos = value; OnPropertyChanged("MReporteDecomisos"); }
        }

        private bool mReporteDecomisoCustodio = false;
        public bool MReporteDecomisoCustodio
        {
            get { return mReporteDecomisoCustodio; }
            set { mReporteDecomisoCustodio = value; OnPropertyChanged("MReporteDecomisoCustodio"); }
        }

        private bool mReporteDecomisoObjeto = false;
        public bool MReporteDecomisoObjeto
        {
            get { return mReporteDecomisoObjeto; }
            set { mReporteDecomisoObjeto = value; OnPropertyChanged("MReporteDecomisoObjeto"); }
        }

        private bool mReporteAltoImpacto = false;
        public bool MReporteAltoImpacto
        {
            get { return mReporteAltoImpacto; }
            set { mReporteAltoImpacto = value; OnPropertyChanged("MReporteAltoImpacto"); }
        }

        private bool mReporteInternosPeligrosos = false;
        public bool MReporteInternosPeligrosos
        {
            get { return mReporteInternosPeligrosos; }
            set { mReporteInternosPeligrosos = value; OnPropertyChanged("MReporteInternosPeligrosos"); }
        }

        private bool mReporteBitacoraAccesoAduana = false;
        public bool MReporteBitacoraAccesoAduana
        {
            get { return mReporteBitacoraAccesoAduana; }
            set { mReporteBitacoraAccesoAduana = value; OnPropertyChanged("MReporteBitacoraAccesoAduana"); }
        }

        private bool mReporteBitacoraCorrespondencia = false;
        public bool MReporteBitacoraCorrespondencia
        {
            get { return mReporteBitacoraCorrespondencia; }
            set { mReporteBitacoraCorrespondencia = value; OnPropertyChanged("MReporteBitacoraCorrespondencia"); }
        }

        private bool mReportePadronVisitaExterna = false;
        public bool MReportePadronVisitaExterna
        {
            get { return mReportePadronVisitaExterna; }
            set { mReportePadronVisitaExterna = value; OnPropertyChanged("MReportePadronVisitaExterna"); }
        }

        private bool mReporteTotalVisitas = false;
        public bool MReporteTotalVisitas
        {
            get { return mReporteTotalVisitas; }
            set { mReporteTotalVisitas = value; OnPropertyChanged("MReporteTotalVisitas"); }
        }

        private bool mReportePoblacionInternos = false;
        public bool MReportePoblacionInternos
        {
            get { return mReportePoblacionInternos; }
            set { mReportePoblacionInternos = value; OnPropertyChanged("MReportePoblacionInternos"); }
        }

        private bool mReporteIngresosEgresos = false;
        public bool MReporteIngresosEgresos
        {
            get { return mReporteIngresosEgresos; }
            set { mReporteIngresosEgresos = value; OnPropertyChanged("MReporteIngresosEgresos"); }
        }

        private bool mReporteVisitaFamiliar = false;
        public bool MReporteVisitaFamiliar
        {
            get { return mReporteVisitaFamiliar; }
            set { mReporteVisitaFamiliar = value; OnPropertyChanged("MReporteVisitaFamiliar"); }
        }

        private bool mReporteVisitantesPorDia = false;
        public bool MReporteVisitantesPorDia
        {
            get { return mReporteVisitantesPorDia; }
            set { mReporteVisitantesPorDia = value; OnPropertyChanged("MReporteVisitantesPorDia"); }
        }

        private bool mReporteVisitantesPorDiaIntima = false;
        public bool MReporteVisitantesPorDiaIntima
        {
            get { return mReporteVisitantesPorDiaIntima; }
            set { mReporteVisitantesPorDiaIntima = value; OnPropertyChanged("MReporteVisitantesPorDiaIntima"); }
        }

        private bool mReporteBitacoraTiempos = false;
        public bool MReporteBitacoraTiempos
        {
            get { return mReporteBitacoraTiempos; }
            set { mReporteBitacoraTiempos = value; OnPropertyChanged("MReporteBitacoraTiempos"); }
        }

        private bool mReporteVisitantesTramite = false;
        public bool MReporteVisitantesTramite
        {
            get { return mReporteVisitantesTramite; }
            set { mReporteVisitantesTramite = value; OnPropertyChanged("MReporteVisitantesTramite"); }
        }

        private bool mReporteVisitantesPorInterno = false;
        public bool MReporteVisitantesPorInterno
        {
            get { return mReporteVisitantesPorInterno; }
            set { mReporteVisitantesPorInterno = value; OnPropertyChanged("MReporteVisitantesPorInterno"); }
        }

        private bool mReportePadronEmpleado = false;

        public bool MReportePadronEmpleado
        {
            get { return mReportePadronEmpleado; }
            set { mReportePadronEmpleado = value; OnPropertyChanged("MReportePadronEmpleado"); }
        }

        private bool mReporteReosPeligrosos = false;
        public bool MReporteReosPeligrosos
        {
            get { return mReporteReosPeligrosos; }
            set { mReporteReosPeligrosos = value; OnPropertyChanged("MReporteReosPeligrosos"); }
        }

        private bool mReporteVisitantesRegistradosPorInterno = false;
        public bool MReporteVisitantesRegistradosPorInterno
        {
            get { return mReporteVisitantesRegistradosPorInterno; }
            set { mReporteVisitantesRegistradosPorInterno = value; OnPropertyChanged("MReporteVisitantesRegistradosPorInterno"); }
        }

        private bool mReporteProgramasRehabilitacion = false;
        public bool MReporteProgramasRehabilitacion
        {
            get { return mReporteProgramasRehabilitacion; }
            set { mReporteProgramasRehabilitacion = value; OnPropertyChanged("MReporteProgramasRehabilitacion"); }
        }

        private bool mBitacoraHospitalizacion = false;
        public bool MBitacoraHospitalizacion
        {
            get { return mBitacoraHospitalizacion; }
            set { mBitacoraHospitalizacion = value; OnPropertyChanged("MBitacoraHospitalizacion"); }
        }

        private bool mHojaEnfermeria = false;
        public bool MHojaEnfermeria
        { 
            get { return mHojaEnfermeria; }
            set { mHojaEnfermeria = value; OnPropertyChanged("MHojaEnfermeria"); }
        }

        private bool mhojacontrolLiquidos = false;
        public bool MhojacontrolLiquidos
        {
            get { return mhojacontrolLiquidos; }
            set { mhojacontrolLiquidos = value; OnPropertyChanged("MhojacontrolLiquidos"); }
        }

        private bool mCatalogoMediaFiliacion = false;
        public bool MCatalogoMediaFiliacion
        {
            get { return mCatalogoMediaFiliacion; }
            set { mCatalogoMediaFiliacion = value; OnPropertyChanged("MCatalogoMediaFiliacion"); }
        }

        private bool mCatalogoTipoFiliacion = false;
        public bool MCatalogoTipoFiliacion
        {
            get { return mCatalogoTipoFiliacion; }
            set { mCatalogoTipoFiliacion = value; OnPropertyChanged("MCatalogoTipoFiliacion"); }
        }

        private bool mCatalogoEscolaridad = false;
        public bool MCatalogoEscolaridad
        {
            get { return mCatalogoEscolaridad; }
            set { mCatalogoEscolaridad = value; OnPropertyChanged("MCatalogoEscolaridad"); }
        }

        private bool mCatalogoReligion = false;
        public bool MCatalogoReligion
        {
            get { return mCatalogoReligion; }
            set { mCatalogoReligion = value; OnPropertyChanged("MCatalogoReligion"); }
        }

        private bool mCatalogoTipoSangre = false;
        public bool MCatalogoTipoSangre
        {
            get { return mCatalogoTipoSangre; }
            set { mCatalogoTipoSangre = value; OnPropertyChanged("MCatalogoTipoSangre"); }
        }

        private bool mCatalogoOcupacion = false;
        public bool MCatalogoOcupacion
        {
            get { return mCatalogoOcupacion; }
            set { mCatalogoOcupacion = value; OnPropertyChanged("MCatalogoOcupacion"); }
        }

        private bool mCatalogoEtnia = false;
        public bool MCatalogoEtnia
        {
            get { return mCatalogoEtnia; }
            set { mCatalogoEtnia = value; OnPropertyChanged("MCatalogoEtnia"); }
        }

        private bool mCatalogoPandillas = false;
        public bool MCatalogoPandillas
        {
            get { return mCatalogoPandillas; }
            set { mCatalogoPandillas = value; OnPropertyChanged("MCatalogoPandillas"); }
        }

        private bool mCatalogoComportamientoHomosexual = false;
        public bool MCatalogoComportamientoHomosexual
        {
            get { return mCatalogoComportamientoHomosexual; }
            set { mCatalogoComportamientoHomosexual = value; OnPropertyChanged("MCatalogoComportamientoHomosexual"); }
        }

        private bool mCatalogoTatuajes = false;
        public bool MCatalogoTatuajes
        {
            get { return mCatalogoTatuajes; }
            set { mCatalogoTatuajes = value; OnPropertyChanged("MCatalogoTatuajes"); }
        }

        private bool mCatalogoCicatricez = false;
        public bool MCatalogoCicatricez
        {
            get { return mCatalogoCicatricez; }
            set { mCatalogoCicatricez = value; OnPropertyChanged("MCatalogoCicatricez"); }
        }

        private bool mCatalogoDefectos = false;
        public bool MCatalogoDefectos
        {
            get { return mCatalogoDefectos; }
            set { mCatalogoDefectos = value; OnPropertyChanged("MCatalogoDefectos"); }
        }

        private bool mCatalogoLunares = false;
        public bool MCatalogoLunares
        {
            get { return mCatalogoLunares; }
            set { mCatalogoLunares = value; OnPropertyChanged("MCatalogoLunares"); }
        }

        private bool mCatalogoTipoDiscapacidad = false;
        public bool MCatalogoTipoDiscapacidad
        {
            get { return mCatalogoTipoDiscapacidad; }
            set { mCatalogoTipoDiscapacidad = value; OnPropertyChanged("MCatalogoTipoDiscapacidad"); }
        }

        private bool mCatalogoTipoAbogado = false;
        public bool MCatalogoTipoAbogado
        {
            get { return mCatalogoTipoAbogado; }
            set { mCatalogoTipoAbogado = value; OnPropertyChanged("MCatalogoTipoAbogado"); }
        }

        private bool mCatalogoPaises = false;
        public bool MCatalogoPaises
        {
            get { return mCatalogoPaises; }
            set { mCatalogoPaises = value; OnPropertyChanged("MCatalogoPaises"); }
        }

        private bool mCatalogoEstados = false;
        public bool MCatalogoEstados
        {
            get { return mCatalogoEstados; }
            set { mCatalogoEstados = value; OnPropertyChanged("MCatalogoEstados"); }
        }

        private bool mCatalogoMunicipios = false;
        public bool MCatalogoMunicipios
        {
            get { return mCatalogoMunicipios; }
            set { mCatalogoMunicipios = value; OnPropertyChanged("MCatalogoMunicipios"); }
        }

        private bool mCatalogoColonias = false;
        public bool MCatalogoColonias
        {
            get { return mCatalogoColonias; }
            set { mCatalogoColonias = value; OnPropertyChanged("MCatalogoColonias"); }
        }

        private bool mCatalogoCentros = false;
        public bool MCatalogoCentros
        {
            get { return mCatalogoCentros; }
            set { mCatalogoCentros = value; OnPropertyChanged("MCatalogoCentros"); }
        }

        private bool mCatalogoEdificios = false;
        public bool MCatalogoEdificios
        {
            get { return mCatalogoEdificios; }
            set { mCatalogoEdificios = value; OnPropertyChanged("MCatalogoEdificios"); }
        }

        private bool mCatalogoSectores = false;
        public bool MCatalogoSectores
        {
            get { return mCatalogoSectores; }
            set { mCatalogoSectores = value; OnPropertyChanged("MCatalogoSectores"); }
        }

        private bool mCatalogoCeldas = false;
        public bool MCatalogoCeldas
        {
            get { return mCatalogoCeldas; }
            set { mCatalogoCeldas = value; OnPropertyChanged("MCatalogoCeldas"); }
        }

        private bool mCatalogoCamas = false;
        public bool MCatalogoCamas
        {
            get { return mCatalogoCamas; }
            set { mCatalogoCamas = value; OnPropertyChanged("MCatalogoCamas"); }
        }

        private bool mCatalogoAgencias = false;
        public bool MCatalogoAgencias
        {
            get { return mCatalogoAgencias; }
            set { mCatalogoAgencias = value; OnPropertyChanged("MCatalogoAgencias"); }
        }

        private bool mCatalogoJuzgados = false;
        public bool MCatalogoJuzgados
        {
            get { return mCatalogoJuzgados; }
            set { mCatalogoJuzgados = value; OnPropertyChanged("MCatalogoJuzgados"); }
        }

        private bool mCatalogoInstitucionesMedicas = false;
        public bool MCatalogoInstitucionesMedicas
        {
            get { return mCatalogoInstitucionesMedicas; }
            set { mCatalogoInstitucionesMedicas = value; OnPropertyChanged("MCatalogoInstitucionesMedicas"); }
        }

        private bool mCatalogoDepartamentos = false;
        public bool MCatalogoDepartamentos
        {
            get { return mCatalogoDepartamentos; }
            set { mCatalogoDepartamentos = value; OnPropertyChanged("MCatalogoDepartamentos"); }
        }

        private bool mCatalogoProgramasRehabilitacion = false;
        public bool MCatalogoProgramasRehabilitacion
        {
            get { return mCatalogoProgramasRehabilitacion; }
            set { mCatalogoProgramasRehabilitacion = value; OnPropertyChanged("MCatalogoProgramasRehabilitacion"); }
        }

        private bool mCatalogoAreas = false;
        public bool MCatalogoAreas
        {
            get { return mCatalogoAreas; }
            set { mCatalogoAreas = value; OnPropertyChanged("MCatalogoAreas"); }
        }

        private bool mCatalogoTiposActividades = false;
        public bool MCatalogoTiposActividades
        {
            get { return mCatalogoTiposActividades; }
            set { mCatalogoTiposActividades = value; OnPropertyChanged("MCatalogoTiposActividades"); }
        }

        private bool mCatalogoActividades = false;
        public bool MCatalogoActividades
        {
            get { return mCatalogoActividades; }
            set { mCatalogoActividades = value; OnPropertyChanged("MCatalogoActividades"); }
        }

        private bool mCatalogoEjes = false;
        public bool MCatalogoEjes
        {
            get { return mCatalogoEjes; }
            set { mCatalogoEjes = value; OnPropertyChanged("MCatalogoEjes"); }
        }

        private bool mCatalogoActividadEje = false;
        public bool MCatalogoActividadEje
        {
            get { return mCatalogoActividadEje; }
            set { mCatalogoActividadEje = value; OnPropertyChanged("MCatalogoActividadEje"); }
        }

        private bool mCatalogoDelitos = false;
        public bool MCatalogoDelitos
        {
            get { return mCatalogoDelitos; }
            set { mCatalogoDelitos = value; OnPropertyChanged("MCatalogoDelitos"); }
        }

        private bool mCatalogoTiposRecursos = false;
        public bool MCatalogoTiposRecursos
        {
            get { return mCatalogoTiposRecursos; }
            set { mCatalogoTiposRecursos = value; OnPropertyChanged("MCatalogoTiposRecursos"); }
        }


        private bool mCatalogoTiposIngreso = false;
        public bool MCatalogoTiposIngreso
        {
            get { return mCatalogoTiposIngreso; }
            set { mCatalogoTiposIngreso = value; OnPropertyChanged("MCatalogoTiposIngreso"); }
        }


        private bool mCatalogoTiposEstudio = false;
        public bool MCatalogoTiposEstudio
        {
            get { return mCatalogoTiposEstudio; }
            set { mCatalogoTiposEstudio = value; OnPropertyChanged("MCatalogoTiposEstudio"); }
        }

        private bool mCatalogoMotivosSolicitudEstudios = false;
        public bool MCatalogoMotivosSolicitudEstudios
        {
            get { return mCatalogoMotivosSolicitudEstudios; }
            set { mCatalogoMotivosSolicitudEstudios = value; OnPropertyChanged("MCatalogoMotivosSolicitudEstudios"); }
        }

        private bool mCatalogoTiposVisita = false;
        public bool MCatalogoTiposVisita
        {
            get { return mCatalogoTiposVisita; }
            set { mCatalogoTiposVisita = value; OnPropertyChanged("MCatalogoTiposVisita"); }
        }

        private bool mCatalogoRelacion = false;
        public bool MCatalogoRelacion
        {
            get { return mCatalogoRelacion; }
            set { mCatalogoRelacion = value; OnPropertyChanged("MCatalogoRelacion"); }
        }

        private bool mCatalogoDecomisoMarcaModelo = false;
        public bool MCatalogoDecomisoMarcaModelo
        {
            get { return mCatalogoDecomisoMarcaModelo; }
            set { mCatalogoDecomisoMarcaModelo = value; OnPropertyChanged("MCatalogoDecomisoMarcaModelo"); }
        }

        private bool mCatalogoDecomisoFabricanteModelo = false;
        public bool MCatalogoDecomisoFabricanteModelo
        {
            get { return mCatalogoDecomisoFabricanteModelo; }
            set { mCatalogoDecomisoFabricanteModelo = value; OnPropertyChanged("MCatalogoDecomisoFabricanteModelo"); }
        }

        private bool mCatalogoDecomisoCompanias = false;
        public bool MCatalogoDecomisoCompanias
        {
            get { return mCatalogoDecomisoCompanias; }
            set { mCatalogoDecomisoCompanias = value; OnPropertyChanged("MCatalogoDecomisoCompanias"); }
        }

        private bool mCatalogoDecomisoUnidadMedida = false;
        public bool MCatalogoDecomisoUnidadMedida
        {
            get { return mCatalogoDecomisoUnidadMedida; }
            set { mCatalogoDecomisoUnidadMedida = value; OnPropertyChanged("MCatalogoDecomisoUnidadMedida"); }
        }

        private bool mCatalogoObjetos = false;
        public bool MCatalogoObjetos
        {
            get { return mCatalogoObjetos; }
            set { mCatalogoObjetos = value; OnPropertyChanged("MCatalogoObjetos"); }
        }

        private bool mCatalogoGruposPoliciales = false;
        public bool MCatalogoGruposPoliciales
        {
            get { return mCatalogoGruposPoliciales; }
            set { mCatalogoGruposPoliciales = value; OnPropertyChanged("MCatalogoGruposPoliciales"); }
        }

        private bool mCatalogoSectorClasificacion = false;
        public bool MCatalogoSectorClasificacion
        {
            get { return mCatalogoSectorClasificacion; }
            set { mCatalogoSectorClasificacion = value; OnPropertyChanged("MCatalogoSectorClasificacion"); }
        }

        private bool mCatalogoTipoMensaje = false;
        public bool MCatalogoTipoMensaje
        {
            get { return mCatalogoTipoMensaje; }
            set { mCatalogoTipoMensaje = value; OnPropertyChanged("MCatalogoTipoMensaje"); }
        }
        private bool mCatalogoTipoAmparoIndirecto = false;


        public bool MCatalogoTipoAmparoIndirecto
        {
            get { return mCatalogoTipoAmparoIndirecto; }
            set { mCatalogoTipoAmparoIndirecto = value; OnPropertyChanged("MCatalogoTipoAmparoIndirecto"); }
        }

        private bool mCatalogoTipoIncidente = false;
        public bool MCatalogoTipoIncidente
        {
            get { return mCatalogoTipoIncidente; }
            set { mCatalogoTipoIncidente = value; OnPropertyChanged("MCatalogoTipoIncidente"); }
        }

        private bool mCatalogoConsultaUnificada = false;
        public bool MCatalogoConsultaUnificada
        {
            get { return mCatalogoConsultaUnificada; }
            set { mCatalogoConsultaUnificada = value; OnPropertyChanged("MCatalogoConsultaUnificada"); }
        }

        private bool mCatalogoHorarioYardas = false;
        public bool MCatalogoHorarioYardas
        {
            get { return mCatalogoHorarioYardas; }
            set { mCatalogoHorarioYardas = value; OnPropertyChanged("MCatalogoHorarioYardas"); }
        }

        private bool mCatalogoCamasHospital = false;
        public bool MCatalogoCamasHospital
        {
            get { return mCatalogoCamasHospital; }
            set { mCatalogoCamasHospital = value; OnPropertyChanged("MCatalogoCamasHospital"); }
        }

        private bool mCatalogoEspecialidades = false;
        public bool MCatalogoEspecialidades
        {
            get { return mCatalogoEspecialidades; }
            set { mCatalogoEspecialidades = value; OnPropertyChanged("MCatalogoEspecialidades"); }
        }

        private bool mCatalogoEspecialistas = false;
        public bool MCatalogoEspecialistas
        {
            get { return mCatalogoEspecialistas; }
            set { mCatalogoEspecialistas = value; OnPropertyChanged("MCatalogoEspecialistas"); }
        }

        private bool mCatalogoServiciosAuxiliaresDiagnostico = false;
        public bool MCatalogoServiciosAuxiliaresDiagnostico
        {
            get { return mCatalogoServiciosAuxiliaresDiagnostico; }
            set { mCatalogoServiciosAuxiliaresDiagnostico = value; OnPropertyChanged("MCatalogoServiciosAuxiliaresDiagnostico"); }
        }

        private bool mCatalogoTipoServiciosAuxiliaresDiagnostico = false;
        public bool MCatalogoTipoServiciosAuxiliaresDiagnostico
        {
            get { return mCatalogoTipoServiciosAuxiliaresDiagnostico; }
            set { mCatalogoTipoServiciosAuxiliaresDiagnostico = value; OnPropertyChanged("MCatalogoTipoServiciosAuxiliaresDiagnostico"); }
        }

        private bool mCatalogoSubTipoServiciosAuxiliaresDiagnostico = false;
        public bool MCatalogoSubTipoServiciosAuxiliaresDiagnostico
        {
            get { return mCatalogoSubTipoServiciosAuxiliaresDiagnostico; }
            set { mCatalogoSubTipoServiciosAuxiliaresDiagnostico = value; OnPropertyChanged("MCatalogoSubTipoServiciosAuxiliaresDiagnostico"); }
        }

        private bool mCatalogoTipoAtencionInterconsulta = false;
        public bool MCatalogoTipoAtencionInterconsulta
        {
            get { return mCatalogoTipoAtencionInterconsulta; }
            set { mCatalogoTipoAtencionInterconsulta = value; OnPropertyChanged("MCatalogoTipoAtencionInterconsulta"); }
        }

        private bool mCatalogoProcedimientosMedicos = false;
        public bool MCatalogoProcedimientosMedicos
        {
            get { return mCatalogoProcedimientosMedicos; }
            set { mCatalogoProcedimientosMedicos = value; OnPropertyChanged("MCatalogoProcedimientosMedicos"); }
        }

        private bool mCatalogoProcedimientosMedicosSubTipo = false;
        public bool MCatalogoProcedimientosMedicosSubTipo
        {
            get { return mCatalogoProcedimientosMedicosSubTipo; }
            set { mCatalogoProcedimientosMedicosSubTipo = value; OnPropertyChanged("MCatalogoProcedimientosMedicosSubTipo"); }
        }

        private bool mCatalogoProcedimientosMateriales = false;
        public bool MCatalogoProcedimientosMateriales
        {
            get { return mCatalogoProcedimientosMateriales; }
            set { mCatalogoProcedimientosMateriales = value; OnPropertyChanged("MCatalogoProcedimientosMateriales"); }
        }

        private bool mProgramasLibertad = false;
        public bool MProgramasLibertad
        {
            get { return mProgramasLibertad; }
            set { mProgramasLibertad = value; OnPropertyChanged("MProgramasLibertad"); }
        }

        private bool mUnidadReceptora = false;
        public bool MUnidadReceptora
        {
            get { return mUnidadReceptora; }
            set { mUnidadReceptora = value; OnPropertyChanged("MUnidadReceptora"); }
        }

        private bool mPrivilegios = false;
        public bool MPrivilegios
        {
            get { return mPrivilegios; }
            set { mPrivilegios = value; OnPropertyChanged("MPrivilegios"); }
        }

        private bool mActivacionCuenta = false;
        public bool MActivacionCuenta
        {
            get { return mActivacionCuenta; }
            set { mActivacionCuenta = value; OnPropertyChanged("MActivacionCuenta"); }
        }

        private bool mCambioClaveAcceso = false;
        public bool MCambioClaveAcceso
        {
            get { return mCambioClaveAcceso; }
            set { mCambioClaveAcceso = value; OnPropertyChanged("MCambioClaveAcceso"); }
        }

        private bool mEquiposAutorizados = false;
        public bool MEquiposAutorizados
        {
            get { return mEquiposAutorizados; }
            set { mEquiposAutorizados = value; OnPropertyChanged("MEquiposAutorizados"); }
        }

        private bool mConfiguracionDepartamentos = false;
        public bool MConfiguracionDepartamentos
        {
            get { return mConfiguracionDepartamentos; }
            set { mConfiguracionDepartamentos = value; OnPropertyChanged("MConfiguracionDepartamentos"); }
        }

        private bool mAdministracionParametros = false;
        public bool MAdministracionParametros
        {
            get { return mAdministracionParametros; }
            set { mAdministracionParametros = value; OnPropertyChanged("MAdministracionParametros"); }
        }

        private bool mCatalogoEnfermedades = false;
        public bool MCatalogoEnfermedades
        {
            get { return mCatalogoEnfermedades; }
            set { mCatalogoEnfermedades = value; RaisePropertyChanged("MCatalogoEnfermedades"); }
        }

        private bool mCatalogoMedicamentos = false;
        public bool MCatalogoMedicamentos
        {
            get { return mCatalogoMedicamentos; }
            set { mCatalogoMedicamentos = value; RaisePropertyChanged("MCatalogoMedicamentos"); }
        }

        private bool mCatalogoMedicamento_Categorias = false;
        public bool MCatalogoMedicamento_Categorias
        {
            get { return mCatalogoMedicamento_Categorias; }
            set { mCatalogoMedicamento_Categorias = value; RaisePropertyChanged("MCatalogoMedicamento_Categorias"); }
        }

        private bool mCatalogoMedicamento_Subcategorias = false;
        public bool MCatalogoMedicamento_Subcategorias
        {
            get { return mCatalogoMedicamento_Subcategorias; }
            set { mCatalogoMedicamento_Subcategorias = value; RaisePropertyChanged("MCatalogoMedicamento_Subcategorias"); }
        }

        private bool mCatalogo_Patologicos = false;
        public bool MCatalogo_Patologicos
        {
            get { return mCatalogo_Patologicos; }
            set { mCatalogo_Patologicos = value; RaisePropertyChanged("MCatalogo_Patologicos"); }
        }

        private bool mReporteBitacoraVisitaFamiliar = false;
        public bool MReporteBitacoraVisitaFamiliar
        {
            get { return mReporteBitacoraVisitaFamiliar; }
            set { mReporteBitacoraVisitaFamiliar = value; OnPropertyChanged("MReporteBitacoraVisitaFamiliar"); }
        }

        private bool mReagendaTV = false;
        public bool MReagendaTV
        {
            get { return mReagendaTV; }
            set { mReagendaTV = value; OnPropertyChanged("MReagendaTV"); }
        }
        #endregion

        #endregion

        #region Hilos Sesion
        private System.Timers.Timer aSesionTimer;
        private int sesionT = Parametro.TIEMPO_VALIDA_SESION;
        #endregion
    }
}

using System.ComponentModel;
using SSP.Servidor;
using ControlPenales;
using System;

namespace ControlPenales
{
    partial class CausaPenalViewModel
    {
        #region INGRESO
        void SetValidacionesDatosIngreso()
        {
            try
            {
                base.ClearRules();
                if (FecRegistroI.HasValue)
                    base.AddRule(() => FecRegistroI, () => FecRegistroI != null ? true : false, "FECHA DE REGISTRO ES REQUERIDO!");
                if (FecCeresoI.HasValue)
                    base.AddRule(() => FecCeresoI, () => FecCeresoI != null ? true : false, "FECHA INGRESO AL CERESO ES REQUERIDO!");
                base.AddRule(() => TipoI, () => TipoI != -1 ? true : false, "TIPO DE INGRESO ES REQUERIDO!");
                base.AddRule(() => EstatusAdministrativoI, () => EstatusAdministrativoI != -1 ? true : false, "ESTATUS ADMINISTRATIVO ES REQUERIDO!");
                base.AddRule(() => ClasificacionI, () => !string.IsNullOrEmpty(ClasificacionI), "CLASIFICACIÓN JURIDICA ES REQUERIDA!");
                base.AddRule(() => IngresoDelito, () => IngresoDelito != -1 ? true : false, "DELITO ES REQUERIDO!");
                base.AddRule(() => NoOficioI, () => !string.IsNullOrEmpty(NoOficioI), "NUMERO DE OFICIO ES REQUERIDO!");
                base.AddRule(() => AutoridadInternaI, () => AutoridadInternaI != -1, "TIPO DE AUTORIDAD QUE INTERNA ES REQUERIDA!");
                base.AddRule(() => TipoSeguridadI, () => !string.IsNullOrEmpty(TipoSeguridadI), "TIPO DE SEGURIDAD ES REQUERIDO!");
                base.AddRule(() => QuedaDisposicionI, () => QuedaDisposicionI != -1, "TIPO DE DISPOSICIÓN ES REQUERIDO!");
                base.AddRule(() => UbicacionI, () => !string.IsNullOrEmpty(UbicacionI), "UBICACIÓN ES REQUERIDA!");
                OnPropertyChanged("FecRegistroI");
                OnPropertyChanged("FecCeresoI");
                OnPropertyChanged("TipoI");
                OnPropertyChanged("EstatusAdministrativoI");
                OnPropertyChanged("ClasificacionI");
                OnPropertyChanged("IngresoDelito");
                OnPropertyChanged("NoOficioI");
                OnPropertyChanged("AutoridadInternaI");
                OnPropertyChanged("TipoSeguridadI");
                OnPropertyChanged("QuedaDisposicionI");
                OnPropertyChanged("NoOficioI");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en datos del ingreso", ex);
            }
        }
        #endregion

        #region CAUSA PENAL
        private void SetValidacionesCausaPenal()
        {
            try
            {
                if (IndexMenu != 1)
                    return;
                base.ClearRules();
                base.AddRule(() => AnioCP, () => AnioCP != null, "AÑO ES REQUERIDO!");
                base.AddRule(() => FolioCP, () => FolioCP != null, "FOLIO ES REQUERIDO!");
                base.AddRule(() => EstatusCP, () => EstatusCP != -1, "ESTATUS DE LA CAUSA PENAL ES REQUERIDO!");
                base.AddRule(() => PaisJuzgadoCP, () => PaisJuzgadoCP != -1, "PAÍS ES REQUERIDO!");
                base.AddRule(() => EstadoJuzgadoCP, () => EstadoJuzgadoCP != -1, "ESTADO ES REQUERIDO!");
                base.AddRule(() => MunicipioJuzgadoCP, () => MunicipioJuzgadoCP != -1, "MUNICIPIO ES REQUERIDO!");
                base.AddRule(() => FueroCP, () => !string.IsNullOrEmpty(FueroCP), "FUERO ES REQUERIDO!");
                base.AddRule(() => JuzgadoCP, () => JuzgadoCP != -1, "JUZGADO ES REQUERIDO!");
                OnPropertyChanged("AnioCP");
                OnPropertyChanged("FolioCP");
                OnPropertyChanged("EstatusCP");
                OnPropertyChanged("PaisJuzgadoCP");
                OnPropertyChanged("EstadoJuzgadoCP");
                OnPropertyChanged("MunicipioJuzgadoCP");
                OnPropertyChanged("FueroCP");
                OnPropertyChanged("JuzgadoCP");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en causa penal", ex);
            }
        }
        #endregion

        #region SENTENCIA
        private void SetValidacionesSentencia()
        {
            try
            {
                if (IndexMenu != 3)
                    return;
                short tmp = 0;
                base.ClearRules();
                #region comentado
                //base.AddRule(() => FecS, () => 
                //    FecS != null ? 
                //        FecS >= SelectedIngreso.FEC_INGRESO_CERESO 
                //            : false,
                //FecS == null ? 
                //    "FECHA ES REQUERIDA!" : 
                //        FecS < SelectedIngreso.FEC_INGRESO_CERESO ?
                //            string.Format("LA FECHA DEBE SER MAYOR A LA FECHA DE INGRESO AL CERESO {0:dd/MM/yyyy}", SelectedIngreso.FEC_INGRESO_CERESO) : "FECHA ES REQUERIDA!");
                #endregion
                base.AddRule(() => FecS, () => FecS != null ,"FECHA ES REQUERIDA!");
                #region comentado
                //base.AddRule(() => FecEjecutoriaS, () =>
                //    FecEjecutoriaS != null ? 
                //        FecS != null ? 
                //            FecEjecutoriaS.Value.Date > FecS.Value.Date 
                //                : false 
                //                    : false,
                //    FecEjecutoriaS == null ? "FECHA EJECUTORIA ES REQUERIDA!" : FecS != null ? FecEjecutoriaS.Value.Date <= FecS.Value.Date ? "LA FECHA EJECUTORIA DEBE SER MAYOR A LA FECHA DE SENTENCIA" : "LA FECHA EJECUTORIA DEBE SER MAYOR A LA FECHA DE SENTENCIA" : "3");
                
                //base.AddRule(() => FecInicioCompurgacionS, () => FecInicioCompurgacionS != null, "FECHA INICIO COMPURGACIÓN ES REQUERIDA");
                //if (FecRealCompurgacionS != null)
                //    base.AddRule(() => FecRealCompurgacionS, () => FecInicioCompurgacionS != null ? FecRealCompurgacionS.Value.Date >= FecInicioCompurgacionS.Value.Date : false, "FECHA REAL COMPURGACIÓN DEBE SER MAYOR A LA FECHA DE INICIO DE COMPURGACIÓN!");
                #endregion
                short? a = 0, m = 0, d = 0;
                a = AniosS != null ? AniosS : 0;
                m = MesesS != null ? MesesS : 0;
                d = DiasS != null ? DiasS : 0;
                if (m == 0 && d == 0)
                    base.AddRule(() => AniosS, () => AniosS.HasValue ? AniosS.Value > 0 ? true : false : false, "AÑOS DE SENTENCIA ES REQUERIDO!");
                if ((a == 0  && d == 0) || m > 0)
                    base.AddRule(() => MesesS, () => MesesS.HasValue ? (MesesS > 0 && MesesS < 12) ? true : false : false, "MESES DE SENTENCIA ES REQUERIDO!");
                if ((a == 0  && m == 0) || d > 0)
                    base.AddRule(() => DiasS, () => DiasS.HasValue ? (DiasS.Value > 0 && DiasS < 31) ? true : false : false, "DÍAS DE SENTENCIA REQUERIDO!");
                base.AddRule(() => GradoAutoriaS, () => GradoAutoriaS.Value != -1, "GRADO DE AUTORÍA ES REQUERIDO!");
                base.AddRule(() => GradoParticipacionS, () => GradoParticipacionS.Value != -1, "GRADO DE PARTICIPACIÓN ES REQUERIDO!");
                OnPropertyChanged("FecS");
                OnPropertyChanged("FecEjecutoriaS");
                OnPropertyChanged("FecInicioCompurgacionS");
                OnPropertyChanged("FecRealCompurgacionS");
                OnPropertyChanged("AniosS");
                OnPropertyChanged("MesesS");
                OnPropertyChanged("DiasS");
                OnPropertyChanged("GradoAutoriaS");
                OnPropertyChanged("GradoParticipacionS");

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en sentencia", ex);
            }
        }

        void setValidacionesAgregarCausaPenalSentencia()
        {
            try
            {
                base.ClearRules();
                if (FecS.HasValue)
                    base.AddRule(() => FecS, () => FecS != null, "FECHA ES REQUERIDA!");
                if (FecEjecutoriaS.HasValue)
                    base.AddRule(() => FecEjecutoriaS, () => FecEjecutoriaS != null, "FECHA EJECUTORIA ES REQUERIDA!");
                if (FecInicioCompurgacionS.HasValue)
                    base.AddRule(() => FecInicioCompurgacionS, () => FecInicioCompurgacionS != null, "FECHA INICIO DE COMPURGACIÓN ES REQUERIDA!");

                if (MesesS == 0 && DiasS == 0)
                    base.AddRule(() => AniosS, () => AniosS.HasValue ? AniosS.Value > 0 ? true : false : false, "AÑOS DE SENTENCIA ES REQUERIDO!");
                if (AniosS == 0 && DiasS == 0)
                    base.AddRule(() => MesesS, () => MesesS.HasValue ? (MesesS > 0 && MesesS < 12) ? true : false : false, "MESES DE SENTENCIA ES REQUERIDO!");
                if (AniosS == 0 && MesesS == 0)
                    base.AddRule(() => DiasS, () => DiasS.HasValue ? (DiasS.Value > 0 && DiasS < 31) ? true : false : false, "DÍAS DE SENTENCIA REQUERIDO!");

                base.AddRule(() => GradoAutoriaS, () => GradoAutoriaS.HasValue ? GradoAutoriaS.Value != -1 : false, "GRADO DE AUTORÍA ES REQUERIDO!");
                base.AddRule(() => GradoParticipacionS, () => GradoParticipacionS.HasValue ? GradoAutoriaS.Value != -1 : false, "GRADO DE PARTICIPACIÓN ES REQUERIDO!");

                base.AddRule(() => FecRealCompurgacionS, () => FecRealCompurgacionS != null, "TIPO DE INGRESO ES REQUERIDO!");

                OnPropertyChanged("FecS");
                OnPropertyChanged("FecEjecutoriaS");
                OnPropertyChanged("FecInicioCompurgacionS");
                OnPropertyChanged("AniosS");
                OnPropertyChanged("MesesS");
                OnPropertyChanged("DiasS");
                OnPropertyChanged("GradoAutoriaS");
                OnPropertyChanged("GradoParticipacionS");
                OnPropertyChanged("FecRealCompurgacionS");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en agregar causa penal sentencia", ex);
            }
        }
        #endregion

        #region DELITOS
        void SetValidacionesAgregarDelito()
        {
            try
            {
                base.ClearRules();

                base.AddRule(() => DelitoD, () => !string.IsNullOrEmpty(DelitoD), "DELITO ES REQUERIDO!");
                base.AddRule(() => ModalidadD, () => !string.IsNullOrEmpty(ModalidadD), "MODALIDAD DEL DELITO ES REQUERIDO!");
                base.AddRule(() => TipoD, () => TipoD > 0, "TIPO DE INGRESO ES REQUERIDO!");
                OnPropertyChanged("DelitoD");
                OnPropertyChanged("ModalidadD");
                OnPropertyChanged("TipoD");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en agregar delito", ex);
            }
        }

        void LimpiarValidacionesAgregarDelito()
        {
            try
            {
                base.RemoveRule("ModalidadD");
                base.RemoveRule("TipoD");

                OnPropertyChanged("ModalidadD");
                OnPropertyChanged("TipoD");
            }

            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar las validaciones en agregar delito", ex);
            }

            return;
        }
        #endregion

        #region INGRESO
        void SetValidacionesIngresoCausaPenal()
        {
            try
            {
                base.ClearRules();

                #region AP
                // base.AddRule(() => AgenciaAP, () => AgenciaAP >= 0, "AGENCIA ES REQUERIDA!");
                //// base.AddRule(() => SelectedAgencia, () => SelectedAgencia != null && SelectedAgencia.ID_AGENCIA > 0, "AGENCIA ES REQUERIDO!");
                // base.AddRule(() => AnioAP, () => AnioAP > 0, "AÑO ES REQUERIDO!");
                // base.AddRule(() => FolioAP, () => FolioAP > 0, "FOLIO ES REQUERIDO!");
                // base.AddRule(() => FecAveriguacionAP, () => FecAveriguacionAP != null, "FECHA DE AVERIGUACION ES REQUERIDO!");
                // base.AddRule(() => FecConsignacionAP, () => FecConsignacionAP != null, "FECHA DE CONSIGNACION ES REQUERIDO!");
                #endregion

                #region CP
                base.AddRule(() => AnioCP, () => AnioCP != null, "AÑO ES REQUERIDO!");
                base.AddRule(() => FolioCP, () => FolioCP != null, "FOLIO ES REQUERIDO!");
                //base.AddRule(() => BisCP, () => !string.IsNullOrEmpty(BisCP), "BIS ES REQUERIDO!");
                //base.AddRule(() => ForaneoCP, () => !string.IsNullOrEmpty(ForaneoCP), "CAUSA PENAL FORANEA ES REQUERIDA!");
                //base.AddRule(() => TipoOrdenCP, () => TipoOrdenCP > 0, "TIPO DE ORDEN ES REQUERIDO!");
                //base.AddRule(() => SelectedPais, () => SelectedPais != null && SelectedPais.ID_PAIS_NAC > 0, "PAIS ES REQUERIDO!");
                //base.AddRule(() => SelectedEstado, () => SelectedEstado != null && SelectedEstado.ID_ENTIDAD > 0, "ESTADO ES REQUERIDO!");
                //base.AddRule(() => MunicipioJuzgadoCP, () => MunicipioJuzgadoCP > 0, "MUNICIPIO ES REQUERIDO!");
                //base.AddRule(() => JuzgadoCP, () => JuzgadoCP > 0, "JUZGADO ES REQUERIDO!");
                //base.AddRule(() => FueroCP, () => !string.IsNullOrEmpty(FueroCP), "FUERO ES REQUERIDO!");
                //base.AddRule(() => FecRadicacionCP, () => FecRadicacionCP != null, "FECHA DE RADICACION ES REQUERIDO!");
                //base.AddRule(() => AmpliacionCP, () => !string.IsNullOrEmpty(AmpliacionCP), "AMPLIACION ES REQUERIDO!");
                //base.AddRule(() => FecVencimientoTerinoCP, () => FecVencimientoTerinoCP != null, "FECHA VENCIMIENTO DE TERMINO ES REQUERIDO!");
                //base.AddRule(() => TerminoCP, () => TerminoCP > 0, "TERMINO DE LA CAUSA PENAL ES REQUERIDO!");
                base.AddRule(() => EstatusCP, () => EstatusCP != -1, "ESTATUS DE LA CAUSA PENAL ES REQUERIDO!");
                OnPropertyChanged("AnioCP");
                OnPropertyChanged("FolioCP");
                OnPropertyChanged("EstatusCP");
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en ingreso causa penal", ex);
            }
        }
        #endregion

        #region COPARTICIPE
        void setValidacionesAgregarCoparticipe()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => PaternoCoparticipe, () => !string.IsNullOrEmpty(PaternoCoparticipe), "APELLIDO PATERNO ES REQUERIDO!");
                base.AddRule(() => MaternoCoparticipe, () => !string.IsNullOrEmpty(MaternoCoparticipe), "APELLIDO MATERNO ES REQUERIDO!");
                base.AddRule(() => NombreCoparticipe, () => !string.IsNullOrEmpty(NombreCoparticipe), "NOMBRE(S) ES REQUERIDO!");
                OnPropertyChanged("PaternoCoparticipe");
                OnPropertyChanged("MaternoCoparticipe");
                OnPropertyChanged("NombreCoparticipe");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en agregar coparticipante", ex);
            }
        }
        void setValidacionesAgregarCoparticipeAlias()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ES REQUERIDO!");
                base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ES REQUERIDO!");
                base.AddRule(() => NombreAlias, () => !string.IsNullOrEmpty(NombreAlias), "NOMBRE DE INGRESO ES REQUERIDO!");
                OnPropertyChanged("PaternoAlias");
                OnPropertyChanged("MaternoAlias");
                OnPropertyChanged("NombreAlias");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en agregar coparticipe alias", ex);
            }
        }
        void setValidacionesAgregarCoparticipeApodo()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => Apodo, () => !string.IsNullOrEmpty(Apodo), "APODO ES REQUERIDO!");
                OnPropertyChanged("Apodo");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en agregar coparticipe apodo", ex);
            }
        }
        #endregion
    
        #region AMPARO DIRECTO
        private void SetValidacionesAmparoDirecto() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => ADNoOficio, () => !string.IsNullOrEmpty(ADNoOficio), "NO.OFICIO ES OBLIGATORIO!");
                //base.AddRule(() => ADFechaDocumentoValid, () => ADFechaDocumentoValid, "FECHA DEL DOCUMENTO ES OBLIGATORIA!");
                //base.AddRule(() => ADFechaNotificacionValid, () => ADFechaNotificacionValid, "FECHA DE NOTIFICACION ES OBLIGATORIA!");
                //base.AddRule(() => ADFechaSuspencionValid, () => ADFechaSuspencionValid, "FECHA DE SUSPENCION ES OBLIGATORIA!");
                base.AddRule(() => ADFechaDocumento, () => ADFechaDocumento.HasValue, "FECHA DEL DOCUMENTO ES OBLIGATORIA!");
                base.AddRule(() => ADFechaNotificacion, () => ADFechaNotificacion.HasValue, "FECHA DE NOTIFICACION ES OBLIGATORIA!");
                base.AddRule(() => ADFechaSuspencion, () => ADFechaSuspencion.HasValue, "FECHA DE SUSPENCION ES OBLIGATORIA!");
                base.AddRule(() => ADAutoridadInforma, () => ADAutoridadInforma != -1, "AUTORIDAD QUE INFORMA ES OBLIGATORIA!");
                OnPropertyChanged("ADNoOficio");
                //OnPropertyChanged("ADFechaDocumentoValid");
                //OnPropertyChanged("ADFechaNotificacionValid");
                //OnPropertyChanged("ADFechaSuspencionValid");
                OnPropertyChanged("ADFechaDocumento");
                OnPropertyChanged("ADFechaNotificacion");
                OnPropertyChanged("ADFechaSuspencion");
                OnPropertyChanged("ADAutoridadInforma");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones para agregar amparo directo", ex);
            }
        }
        #endregion

        #region AMPARO INDIRECTO
        private void SetValidacionesAmparoIndirecto()
        {
            try
            {
                base.ClearRules();
                //base.AddRule(() => AITipo, () => AITipo != -1, "TIPO DE AMPARO INDIRECTO ES OBLIGATORIO!");
                base.AddRule(() => AINoOficio, () => !string.IsNullOrEmpty(AINoOficio), "NO.OFICIO ES OBLIGATORIO!");
                //base.AddRule(() => AIFechaDocumentoValid, () => AIFechaDocumentoValid, "FECHA DEL DOCUMENTO ES OBLIGATORIA!");
                //base.AddRule(() => AIFechaNotificacionValid, () => AIFechaNotificacionValid, "FECHA DE NOTIFICACION ES OBLIGATORIA!");
                //base.AddRule(() => AIFechaSuspencionValid, () => AIFechaSuspencionValid, "FECHA DE SUSPENCION ES OBLIGATORIA!");
                base.AddRule(() => AIFechaDocumento, () => AIFechaDocumento.HasValue, "FECHA DEL DOCUMENTO ES OBLIGATORIA!");
                base.AddRule(() => AIFechaNotificacion, () => AIFechaNotificacion.HasValue, "FECHA DE NOTIFICACION ES OBLIGATORIA!");
                base.AddRule(() => AIFechaSuspencion, () => AIFechaSuspencion.HasValue, "FECHA DE SUSPENCION ES OBLIGATORIA!");
                base.AddRule(() => AIAutoridadInforma, () => AIAutoridadInforma != -1, "AUTORIDAD QUE INFORMA ES OBLIGATORIA!");
                
                OnPropertyChanged("AITipo");
                OnPropertyChanged("AINoOficio");
                //OnPropertyChanged("AIFechaDocumentoValid");
                //OnPropertyChanged("AIFechaNotificacionValid");
                //OnPropertyChanged("AIFechaSuspencionValid");
                OnPropertyChanged("AIFechaDocumento");
                OnPropertyChanged("AIFechaNotificacion");
                OnPropertyChanged("AIFechaSuspencion");
                OnPropertyChanged("AIAutoridadInforma");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones para agregar amparo indirecto", ex);
            }
        }
        #endregion

        #region INCIDENTES
        private void SetValidacionesIncidentes()
        {
            try
            {
                base.ClearRules();

                base.AddRule(() => ITipo, () => ITipo != -1, "TIPO DE INCIDENTE ES OBLIGATORIO!");
                base.AddRule(() => INoOficio, () => !string.IsNullOrEmpty(INoOficio), "NO.OFICIO ES OBLIGATORIO!");
                base.AddRule(() => IFechaDocumentoValid, () => IFechaDocumentoValid, "FECHA DEL DOCUMENTO ES OBLIGATORIA!");
                base.AddRule(() => IAutoridadInforma, () => IAutoridadInforma != -1, "AUTORIDAD QUE INFORMA ES OBLIGATORIA!");
                base.AddRule(() => IResultado, () => !string.IsNullOrEmpty(IResultado), "RESULTADO ES OBLIGATORIO!");
                if (ITipo == 3)
                {
                    if (IResultado == "M")
                    {
                        short? a = 0, m = 0, d = 0;
                        a = IModificaPenaAnios != null ? IModificaPenaAnios : 0;
                        m = IModificaPenaMeses != null ? IModificaPenaMeses : 0;
                        d = IModificaPenaDias != null ? IModificaPenaDias : 0;
                        if (m == 0 && d == 0)
                            base.AddRule(() => IModificaPenaAnios, () => IModificaPenaAnios.HasValue ? IModificaPenaAnios.Value > 0 ? true : false : false, "AÑOS ES REQUERIDO!");
                        if ((a == 0 && d == 0) || m > 0)
                            base.AddRule(() => IModificaPenaMeses, () => IModificaPenaMeses.HasValue ? (IModificaPenaMeses > 0 && IModificaPenaMeses < 12) ? true : false : false, "MESES ES REQUERIDO!");
                        if ((a == 0 && m == 0) || d > 0)
                            base.AddRule(() => IModificaPenaDias, () => IModificaPenaDias.HasValue ? (IModificaPenaDias.Value > 0 && IModificaPenaDias < 31) ? true : false : false, "DÍAS ES REQUERIDO!");
                    }
                }
                if (ITipo == 1)
                {
                    if (IResultado == "C")
                    {
                        base.AddRule(() => IDiasRemision, () => IDiasRemision.HasValue ? true : false, "DÍAS DE REMISIÓN ES REQUERIDO!");
                     
                    }
                }
                OnPropertyChanged("ITipo");
                OnPropertyChanged("INoOficio");
                OnPropertyChanged("IFechaDocumentoValid");
                OnPropertyChanged("IAutoridadInforma");
                OnPropertyChanged("IResultado");
                OnPropertyChanged("IModificaPenaAnios");
                OnPropertyChanged("IModificaPenaMeses");
                OnPropertyChanged("IModificaPenaDias");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones para agregar incidentes", ex);
            }
        }
        #endregion

        #region RECURSOS
        private void SetValidacionesRecurso()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => RTipoRecurso, () => RTipoRecurso != -3, "TIPO DE RECURSO ES REQUERIDO!");
                base.AddRule(() => RTribunal, () => RTribunal != -1, "TRIBUNAL ES REQUERIDO!");
                base.AddRule(() => RResultadoRecurso, () => !string.IsNullOrEmpty(RResultadoRecurso), "RESULTADO DEL RECURSO ES REQUERIDO!");
                base.AddRule(() => RFechaRecursoValid, () => RFechaRecursoValid, "FECHA DEL RECURSO ES REQUERIDA!");
                base.AddRule(() => RFechaResolucionValid, () => RFechaResolucionValid, "FECHA DE RESOLUCIÓN ES REQUERIDO!");
                base.AddRule(() => RFuero, () => !string.IsNullOrEmpty(RFuero), "FUERO ES REQUERIDO!");
                base.AddRule(() => RTocaPenal, () => !string.IsNullOrEmpty(RTocaPenal), "TOCA PENAL ES REQUERIDO!");
                base.AddRule(() => RNoOficio, () => !string.IsNullOrEmpty(RNoOficio), "NO.OFICIO ES REQUERIDO!");
                base.AddRule(() => RResolucion, () => !string.IsNullOrEmpty(RResolucion), "RESOLUCIÓN ES REQUERIDA!");

                if (RTipoRecurso == (short)enumRecursosTipo.APELACION_O_DETERMINACION_SEGUNDA_INSTANCIA && RResultadoRecurso == ((short)enumResultado2Instancia.MODIFICA).ToString())
                {
                    short? a = 0, m = 0, d = 0;
                    a = RAnio != null ? RAnio : 0;
                    m = RMeses != null ? RMeses : 0;
                    d = RDias != null ? RDias : 0;
                    if (m == 0 && d == 0)
                        base.AddRule(() => RAnio, () => RAnio.HasValue ? RAnio.Value > 0 ? true : false : false, "AÑOS DE SENTENCIA ES REQUERIDO!");
                    if ((a == 0 && d == 0) || m > 0)
                        base.AddRule(() => RMeses, () => RMeses.HasValue ? (RMeses > 0 && RMeses < 12) ? true : false : false, "MESES DE SENTENCIA ES REQUERIDO!");
                    if ((a == 0 && m == 0) || d > 0)
                        base.AddRule(() => RDias, () => RDias.HasValue ? (RDias.Value > 0 && RDias < 31) ? true : false : false, "DÍAS DE SENTENCIA REQUERIDO!");
                }

                OnPropertyChanged("RTipoRecurso");
                OnPropertyChanged("RTribunal");
                OnPropertyChanged("RResultadoRecurso");
                OnPropertyChanged("RFechaRecursoValid");
                OnPropertyChanged("RFechaResolucionValid");
                OnPropertyChanged("RFuero");
                OnPropertyChanged("RTocaPenal");
                OnPropertyChanged("RNoOficio");
                OnPropertyChanged("RResolucion");

                OnPropertyChanged("RAnio");
                OnPropertyChanged("RMeses");
                OnPropertyChanged("RDias");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en recurso", ex);
            }
        }
        #endregion

        #region General
        private bool InternoActivo()
        {
            if (SelectedIngreso != null)
            {
                if (SelectedIngreso.ID_ESTATUS_ADMINISTRATIVO == (short)enumEstatusAdministrativo.LIBERADO)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "El ingreso no esta activo en el centro");
                    return false; 
                }
            }
            return true;
        }
        #endregion
    }
}

using SSP.Controlador.Catalogo.Justicia.EstudioMI;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel : ValidationViewModelBase
    {
        private void GuardarFactorCrimidiagnostico()
        {
            try
            {
                var Name = "Factor Criminodiagnostico";
                if (emiActual != null)
                {
                    short def = 0;
                    var factor = new EMI_FACTOR_CRIMINODIAGNOSTICO();
                    factor.ID_EMI = emiActual.ID_EMI;
                    factor.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    if (EgocentrismoSelected != -1)
                        factor.EGOCENTRISMO = EgocentrismoSelected != null ? EgocentrismoSelected.Value : def;
                    
                    if (AgresividadSelected != -1)
                        factor.AGRESIVIDAD = AgresividadSelected != null ? AgresividadSelected.Value : def;
                    
                    if (IndiferenciaAfectivaSelected != -1)
                        factor.INDIFERENCIA_AFECTIVA = IndiferenciaAfectivaSelected != null ? IndiferenciaAfectivaSelected.Value : def;
                    
                    if (LabilidadAfectivaSelected != -1)
                        factor.LABILIDAD_AFECTIVA = LabilidadAfectivaSelected != null ? LabilidadAfectivaSelected.Value : def;
                    
                    if (AdaptacionSocialSelected != -1)
                        factor.ADAPTABILIDAD_SOCIAL = AdaptacionSocialSelected != null ? AdaptacionSocialSelected.Value : def;
                    
                    if (LiderazgoSelected != -1)
                        factor.LIDERAZGO = LiderazgoSelected != null ? LiderazgoSelected.Value : def;
                    
                    if (ToleranciaFrustracionSelected != -1)
                        factor.TOLERANCIA_A_LA_FRUSTRACION = ToleranciaFrustracionSelected != null ? ToleranciaFrustracionSelected.Value : def;
                    
                    if (ControlImpulsosSelected != -1)
                        factor.CONTROL_DE_IMPULSOS = ControlImpulsosSelected != null ? ControlImpulsosSelected.Value : def;
                    
                    if (CapacidadCriminalSelected != -1)
                        factor.CAPACIDAD_CRIMINAL = CapacidadCriminalSelected != null ? CapacidadCriminalSelected.Value : def;
                    
                    if (PronosticoIntrainstitucionalSelected != -1)
                        factor.PRONOSTICO_INTRA_INST = PronosticoIntrainstitucionalSelected != null ? PronosticoIntrainstitucionalSelected.Value : def;
                    
                    if (IndiceEstadoPeligrosoSelected != -1)
                        factor.INDICE_ESTADO_PELIGROSO = IndiceEstadoPeligrosoSelected != null ? IndiceEstadoPeligrosoSelected.Value : def;
                    
                    if (UbicacionClasificacionCriminologicaSelected != -1)
                        factor.UBICACION_CLASIF_CRIM = UbicacionClasificacionCriminologicaSelected != null ? UbicacionClasificacionCriminologicaSelected.Value : def;
                    factor.DICTAMEN = Dictamen;
                    if (emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO == null)
                    {
                        if (PInsertar)
                        {
                            var emi = new EMI() { ID_EMI = emiActual.ID_EMI, ID_EMI_CONS = emiActual.ID_EMI_CONS, ESTATUS = "C", FEC_CAPTURA = emiActual.FEC_CAPTURA};
                            if (new cEMIFactorCriminodiagnostico().Agregar(emi, factor))
                            {
                                emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO = factor;
                                Mensaje(true, Name);
                            }
                            else
                                Mensaje(false, Name);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }
                    else
                    {
                        if (PEditar)
                        {
                            if (new cEMIFactorCriminodiagnostico().Actualizar(factor))
                            {
                                emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO = factor;
                                Mensaje(true, Name);
                            }
                            else
                                Mensaje(true, Name);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar factor Criminodiagnóstico", ex);
            }
        }

        private void PopulateFactorCrimidiagnostico()
        {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual != null)
                    if (emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO != null)
                    {
                        ClasificacionCrimidiagnosticoEnabled = true;
                        short def = -1;
                        EgocentrismoSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EGOCENTRISMO != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EGOCENTRISMO : def;
                        AgresividadSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.AGRESIVIDAD != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.AGRESIVIDAD : def;
                        IndiferenciaAfectivaSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.INDIFERENCIA_AFECTIVA != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.INDIFERENCIA_AFECTIVA : def;
                        LabilidadAfectivaSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.LABILIDAD_AFECTIVA != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.LABILIDAD_AFECTIVA : def;
                        AdaptacionSocialSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.ADAPTABILIDAD_SOCIAL != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.ADAPTABILIDAD_SOCIAL : def;
                        LiderazgoSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.LIDERAZGO != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.LIDERAZGO : def;
                        ToleranciaFrustracionSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.TOLERANCIA_A_LA_FRUSTRACION != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.TOLERANCIA_A_LA_FRUSTRACION : def;
                        ControlImpulsosSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.CONTROL_DE_IMPULSOS != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.CONTROL_DE_IMPULSOS : def;
                        CapacidadCriminalSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.CAPACIDAD_CRIMINAL != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.CAPACIDAD_CRIMINAL : def;
                        PronosticoIntrainstitucionalSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.PRONOSTICO_INTRA_INST != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.PRONOSTICO_INTRA_INST : def;
                        IndiceEstadoPeligrosoSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.INDICE_ESTADO_PELIGROSO != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.INDICE_ESTADO_PELIGROSO : def;
                        UbicacionClasificacionCriminologicaSelected = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.UBICACION_CLASIF_CRIM != def ? emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.UBICACION_CLASIF_CRIM : def;
                        Dictamen = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.DICTAMEN;
                        return;
                    }
                //HISTORIAL DE INFORMACION
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO != null)
                    {
                        short def = -1;
                        EgocentrismoSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EGOCENTRISMO != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EGOCENTRISMO : def;
                        AgresividadSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.AGRESIVIDAD != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.AGRESIVIDAD : def;
                        IndiferenciaAfectivaSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.INDIFERENCIA_AFECTIVA != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.INDIFERENCIA_AFECTIVA : def;
                        LabilidadAfectivaSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.LABILIDAD_AFECTIVA != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.LABILIDAD_AFECTIVA : def;
                        AdaptacionSocialSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.ADAPTABILIDAD_SOCIAL != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.ADAPTABILIDAD_SOCIAL : def;
                        LiderazgoSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.LIDERAZGO != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.LIDERAZGO : def;
                        ToleranciaFrustracionSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.TOLERANCIA_A_LA_FRUSTRACION != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.TOLERANCIA_A_LA_FRUSTRACION : def;
                        ControlImpulsosSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.CONTROL_DE_IMPULSOS != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.CONTROL_DE_IMPULSOS : def;
                        CapacidadCriminalSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.CAPACIDAD_CRIMINAL != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.CAPACIDAD_CRIMINAL : def;
                        PronosticoIntrainstitucionalSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.PRONOSTICO_INTRA_INST != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.PRONOSTICO_INTRA_INST : def;
                        IndiceEstadoPeligrosoSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.INDICE_ESTADO_PELIGROSO != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.INDICE_ESTADO_PELIGROSO : def;
                        UbicacionClasificacionCriminologicaSelected = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.UBICACION_CLASIF_CRIM != def ? AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.UBICACION_CLASIF_CRIM : def;
                        Dictamen = AnteriorEMI.EMI_FACTOR_CRIMINODIAGNOSTICO.DICTAMEN;
                        return;
                    }

                //NUEVO REGISTRO
                EgocentrismoSelected = AgresividadSelected = IndiferenciaAfectivaSelected = LabilidadAfectivaSelected = AdaptacionSocialSelected = LiderazgoSelected = ToleranciaFrustracionSelected = ControlImpulsosSelected = CapacidadCriminalSelected = PronosticoIntrainstitucionalSelected = IndiceEstadoPeligrosoSelected = UbicacionClasificacionCriminologicaSelected = -1;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer factor criminodiagnóstico", ex);
            }
        }

        private void PopularClasificacionCriminologica()
        {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual != null)
                    if (emiActual.EMI_CLAS_CRIMINOLOGICA != null)
                    {
                        ClasCriminologicaEnabled = ClasificacionCriminologicaEnabled = true;
                        PertenenciaCrimenOrganizado = emiActual.EMI_CLAS_CRIMINOLOGICA.ID_CRIMENO != null ? emiActual.EMI_CLAS_CRIMINOLOGICA.ID_CRIMENO : -1;
                        ClasificacionCriminologica = emiActual.EMI_CLAS_CRIMINOLOGICA.ID_CLAS != null ? emiActual.EMI_CLAS_CRIMINOLOGICA.ID_CLAS : -1;
                        LstSanciones = new ObservableCollection<EMI_SANCION_DISCIPLINARIA>(new cEMISancionDisciplinaria().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS));
                        SancionesEmpty = LstSanciones.Count > 0 ? false : true;
                        return;
                    }
                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_CLAS_CRIMINOLOGICA != null)
                    {
                        PertenenciaCrimenOrganizado = AnteriorEMI.EMI_CLAS_CRIMINOLOGICA.ID_CRIMENO != null ? AnteriorEMI.EMI_CLAS_CRIMINOLOGICA.ID_CRIMENO : -1;
                        ClasificacionCriminologica = AnteriorEMI.EMI_CLAS_CRIMINOLOGICA.ID_CLAS != null ? AnteriorEMI.EMI_CLAS_CRIMINOLOGICA.ID_CLAS : -1;
                        LstSanciones = new ObservableCollection<EMI_SANCION_DISCIPLINARIA>(AnteriorEMI.EMI_SANCION_DISCIPLINARIA);
                        SancionesEmpty = LstSanciones.Count > 0 ? false : true;
                        return;
                    }

                //NUEVO REGISTRO    
                PertenenciaCrimenOrganizado = ClasificacionCriminologica = -1;
                LstSanciones = new ObservableCollection<EMI_SANCION_DISCIPLINARIA>();
                SancionesEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer clasificación criminológica", ex);
            }            
        }

        #region [DICTAMEN]
        public void LimpiarDictamen()
        {
            try
            {
                DictamenAdd = string.Empty;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DICTAMEN);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar dictamen", ex);
            }
        }
        public void AgregarDictamen()
        {
            try
            {
                Dictamen = string.Format("{0} {1}", Dictamen, DictamenAdd);
                LimpiarDictamen();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar dictamen", ex);
            }
        }
        #endregion

        #region [LOAD]
        //LOAD
        private void FactorCriminodiagnosticoLoad(FactoresCriminodiagnosticoView Window = null)
        {
            try
            {
                if (TabClasificacionCriminologicaPadreSelected)
                {
                    if (TabFactorCriminodiagnosticoSelected)//LA OPCION ESTA SELECCIONADA
                    {
                        setValidacionesCriminodiagnostico();
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar factor diagnóstico criminológico", ex);
            }
        }

        private void ClasificacionCriminologicaLoad(ClasificacionCriminologicaView Window = null)
        {
            try
            {
                if (TabClasificacionCriminologicaPadreSelected)
                {
                    if (TabClasificacionCriminologicaSelected)//LA OPCION ESTA SELECCIONADA
                    {
                        ClasificacionCrimidiagnosticoEnabled = true;
                        setValidacionesClasificacionCriminologica();
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar clasificación criminológica", ex);
            }
        }
        #endregion

        #region [UNLOAD]
        private void FactorCriminodiagnosticoUnload(FactoresCriminodiagnosticoView Window = null)
        {
            try
            {
                if (!base.HasErrors)
                    GuardarFactorCrimidiagnostico();
                else
                {
                    TabClasificacionCriminologicaPadreSelected = TabFactorCriminodiagnosticoSelected = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de factores diagnóstico criminológico", ex);
            }
        }

        private void ClasificacionCriminologicaUnload(ClasificacionCriminologicaView Window = null)
        {
            try
            {
                if (!base.HasErrors)
                    GuardarClasCriminologica();
                else
                {
                    TabClasificacionCriminologicaPadreSelected = TabClasificacionCriminologicaSelected = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de clasificación criminológica", ex);
            }
        }
        #endregion
    }
}

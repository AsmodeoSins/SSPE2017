using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
namespace ControlPenales
{
    partial class EMILiberadoViewModel : ValidationViewModelBase
    {
        private void PopulateFactores() 
        {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual != null)
                    if (emiActual.EMI_FACTORES_SOCIO_FAMILIARES != null)
                    {
                        FactoresSocioFamiliaresEnabled = FactoresEnabled = IngresoAnteriorEnabled = IngresoAnteriorMenorEnabled = true;
                        RecibeVisitaFamiliar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR;
                        Frecuencia = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_VISITA_FAMILIAR;
                        VisitaIntima = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA;
                        ApoyoEconomico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO;
                        CantidadFrecuencia = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_APOYO_ECONOMICO;
                        CantidadApoyoEconomico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_APOYO_ECONOMICO;
                        VivePadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE == null ? SelectIngreso.PADRE_FINADO : emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE;
                        ViveMadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE == null ? SelectIngreso.MADRE_FINADO : emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE;
                        PadresVivenJuntos = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.PADRES_JUNTOS;
                        FallecioPadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_PADRE;
                        FallecioMadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_MADRE;
                        MotivoSeparacion = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MOTIVOS_SEPARACION;
                        EdadInternoSeparacionPadres = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_SEPARACION;
                        TotalParejas = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.TOTAL_PAREJAS;
                        CuantasUnion = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_PAREJAS_UNION;
                        ContactoNombre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_NOMBRE;
                        ContactoParentesco = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_CONTACTO_PARENTESCO;
                        //ContactoTelefono = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO;
                        TextContactoTelefono = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.ToString();
                        Social = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL;
                        Economico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_ECONOMICO;
                        Cultural = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL;
                        Hijos = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS;
                        Registrados = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.HIJOS_REGISTRADOS;
                        CuantosMantieneRelacion = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_RELACION;
                        CuantosVisitan = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_VISITA;
                        AbandonoFamiliar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR;
                        EspecifiqueAbandonoFamiliar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR;
                        MaltratoEmocional = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL;
                        EspecifiqueMaltratoEmocional = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL;
                        MaltratoFisico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO;
                        EspecifiqueMaltratoFisico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO;
                        AbusoSexual = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL;
                        EspecifiqueAbusoSexual = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL;
                        EdadAbuso = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_ABUSO_SEXUAL;
                        EdadInicioContactoSexual = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INICIO_CONTACTO_SEXUAL;
                        HuidasHogar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.HUIDAS_HOGAR_ABANDONO_FAMILIAR;
                        return;
                    }

                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES != null)
                    {
                        ControlTab = 5;
                        RecibeVisitaFamiliar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR;
                        Frecuencia = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_VISITA_FAMILIAR;
                        VisitaIntima = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA;
                        ApoyoEconomico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO;
                        CantidadFrecuencia = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_APOYO_ECONOMICO;
                        CantidadApoyoEconomico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_APOYO_ECONOMICO;
                        VivePadre = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE == null ? SelectIngreso.PADRE_FINADO : AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE;
                        ViveMadre = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE == null ? SelectIngreso.MADRE_FINADO : AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE;
                        PadresVivenJuntos = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.PADRES_JUNTOS;
                        FallecioPadre = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_PADRE;
                        FallecioMadre = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_MADRE;
                        MotivoSeparacion = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.MOTIVOS_SEPARACION;
                        EdadInternoSeparacionPadres = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_SEPARACION;
                        TotalParejas = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.TOTAL_PAREJAS;
                        CuantasUnion = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_PAREJAS_UNION;
                        ContactoNombre = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_NOMBRE;
                        ContactoParentesco = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_CONTACTO_PARENTESCO;
                        //ContactoTelefono = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO;
                        TextContactoTelefono = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.ToString();
                        Social = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL;
                        Economico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_ECONOMICO;
                        Cultural = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL;
                        Hijos = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS;
                        Registrados = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.HIJOS_REGISTRADOS;
                        CuantosMantieneRelacion = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_RELACION;
                        CuantosVisitan = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_VISITA;
                        AbandonoFamiliar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR;
                        EspecifiqueAbandonoFamiliar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR;
                        MaltratoEmocional = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL;
                        EspecifiqueMaltratoEmocional = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL;
                        MaltratoFisico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO;
                        EspecifiqueMaltratoFisico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO;
                        AbusoSexual = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL;
                        EspecifiqueAbusoSexual = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL;
                        EdadAbuso = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_ABUSO_SEXUAL;
                        EdadInicioContactoSexual = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INICIO_CONTACTO_SEXUAL;
                        HuidasHogar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.HUIDAS_HOGAR_ABANDONO_FAMILIAR;
                        return;
                    }

                //NUEVO REGISTRO
                RecibeVisitaFamiliar = string.Empty;
                VisitaIntima = string.Empty;
                ApoyoEconomico = string.Empty;
                CantidadApoyoEconomico = string.Empty;
                VivePadre = string.Empty;
                ViveMadre = string.Empty;
                PadresVivenJuntos = string.Empty;
                MotivoSeparacion = string.Empty;
                ContactoNombre = string.Empty;
                AbandonoFamiliar = string.Empty;
                MaltratoEmocional = string.Empty;
                EspecifiqueMaltratoEmocional = string.Empty;
                MaltratoFisico = string.Empty;
                EspecifiqueMaltratoFisico = string.Empty;
                AbusoSexual = string.Empty;
                EspecifiqueAbusoSexual = string.Empty;
                Frecuencia = -1;
                CantidadFrecuencia = -1;
                ContactoParentesco = -1;
                EspecifiqueAbandonoFamiliar = -1;
                HuidasHogar = -1;
                Social = -1;
                Economico = -1;
                Cultural = -1;
                FallecioPadre = 0;
                FallecioMadre = 0;
                EdadInternoSeparacionPadres = TotalParejas = CuantasUnion = Hijos = Registrados = CuantosMantieneRelacion = CuantosVisitan = EdadAbuso = EdadInicioContactoSexual = 0;
                ContactoTelefono = new Nullable<long>();
                TextContactoTelefono = string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer factores", ex);
            }
        }

        private void PopulateDatosGrupoFamiliar()
        {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual != null)
                    if (emiActual.EMI_GRUPO_FAMILIAR != null)
                    {
                        if (emiActual.EMI_GRUPO_FAMILIAR.Count > 0)
                        {
                            GrupoFamiliarEnabled = true;
                            LstGrupoFamiliar = new ObservableCollection<EMI_GRUPO_FAMILIAR>(emiActual.EMI_GRUPO_FAMILIAR);
                            IsGrupoFamiliarEmpty = LstGrupoFamiliar.Count > 0 ? false : true;
                            return;
                        }
                    }

                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_GRUPO_FAMILIAR != null)
                    {
                        LstGrupoFamiliar = new ObservableCollection<EMI_GRUPO_FAMILIAR>(AnteriorEMI.EMI_GRUPO_FAMILIAR);
                        IsGrupoFamiliarEmpty = LstGrupoFamiliar.Count > 0 ? false : true;
                        return;
                    }

                //NUEVO REGISTRO
                LstGrupoFamiliar = new ObservableCollection<EMI_GRUPO_FAMILIAR>();
                IsGrupoFamiliarEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer datos grupo familiar", ex);
            }
        }

        private void GrupoFamiliarAntecedentes() {
            try
            {
                ControlTab = 7;
                //INFORMACION ACTUAL
                if (emiActual != null)
                    if (emiActual.EMI_ANTECEDENTE_FAM_CON_DEL != null)
                    {
                        if (emiActual.EMI_ANTECEDENTE_FAM_CON_DEL.Count > 0)
                        {
                            AntecedenteGrupoFamiliarEnabled = GrupoFamiliarEnabled = true;
                            LstFamiliarDelito = new ObservableCollection<EMI_ANTECEDENTE_FAM_CON_DEL>(emiActual.EMI_ANTECEDENTE_FAM_CON_DEL);
                            IsEmptyFamiliarDelito = LstFamiliarDelito.Count > 0 ? false : true;
                            return;
                        }
                    }
                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_ANTECEDENTE_FAM_CON_DEL != null)
                    {
                        LstFamiliarDelito = new ObservableCollection<EMI_ANTECEDENTE_FAM_CON_DEL>(AnteriorEMI.EMI_ANTECEDENTE_FAM_CON_DEL);
                        IsEmptyFamiliarDelito = LstFamiliarDelito.Count > 0 ? false : true;
                        return;
                    }
                //NUEVO REGISTRO
                LstFamiliarDelito = new ObservableCollection<EMI_ANTECEDENTE_FAM_CON_DEL>();
                IsEmptyFamiliarDelito = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en grupo familiar antecedentes", ex);
            }
        }

        private void GrupoFamiliarDroga() {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual != null)
                    if (emiActual.EMI_ANTECEDENTE_FAMILIAR_DROGA != null)
                    {
                        if (emiActual.EMI_ANTECEDENTE_FAMILIAR_DROGA.Count > 0)
                        {
                            AntecedenteGrupoFamiliarEnabled = GrupoFamiliarEnabled = true;
                            LstFamiliarDroga = new ObservableCollection<EMI_ANTECEDENTE_FAMILIAR_DROGA>(emiActual.EMI_ANTECEDENTE_FAMILIAR_DROGA);
                            IsEmptyFamiliarDroga = LstFamiliarDroga.Count > 0 ? false : true;
                            return;
                        }
                    }
                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_ANTECEDENTE_FAMILIAR_DROGA != null)
                    {
                        LstFamiliarDroga = new ObservableCollection<EMI_ANTECEDENTE_FAMILIAR_DROGA>(AnteriorEMI.EMI_ANTECEDENTE_FAMILIAR_DROGA);
                        IsEmptyFamiliarDroga = LstFamiliarDroga.Count > 0 ? false : true;
                        return;
                    }

                //NUEVO REGISTRO
                LstFamiliarDroga = new ObservableCollection<EMI_ANTECEDENTE_FAMILIAR_DROGA>();
                IsEmptyFamiliarDroga = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en grupo familiar droga", ex);
            }
        }
    }
}

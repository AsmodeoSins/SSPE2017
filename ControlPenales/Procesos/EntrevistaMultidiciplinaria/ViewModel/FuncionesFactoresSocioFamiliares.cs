using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel : ValidationViewModelBase
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
                        RecibeVisitaFamiliar = !string.IsNullOrEmpty(emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR) ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR : "";
                        Frecuencia = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_VISITA_FAMILIAR!= null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_VISITA_FAMILIAR : -1;
                        VisitaIntima = !string.IsNullOrEmpty(emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA) ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA : "";
                        ApoyoEconomico = !string.IsNullOrEmpty(emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO) ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO : "";
                        CantidadFrecuencia = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_APOYO_ECONOMICO != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_APOYO_ECONOMICO : -1;
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
                        ContactoParentesco = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_CONTACTO_PARENTESCO != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_CONTACTO_PARENTESCO : -1;
                        //ContactoTelefono = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO;
                        TextContactoTelefono = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.ToString();
                        Social = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL : -1;
                        Economico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_ECONOMICO != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_ECONOMICO : -1;
                        Cultural = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL : -1;
                        Hijos = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS;
                        Registrados = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.HIJOS_REGISTRADOS;
                        CuantosMantieneRelacion = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_RELACION;
                        CuantosVisitan = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_VISITA;
                        AbandonoFamiliar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR;
                        EspecifiqueAbandonoFamiliar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR : -1;
                        MaltratoEmocional = !string.IsNullOrEmpty(emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL) ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL : "";
                        EspecifiqueMaltratoEmocional = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL;
                        MaltratoFisico = !string.IsNullOrEmpty(emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO) ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO : "";
                        EspecifiqueMaltratoFisico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO;
                        AbusoSexual = !string.IsNullOrEmpty(emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL) ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL : "";
                        EspecifiqueAbusoSexual = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL;
                        EdadAbuso = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_ABUSO_SEXUAL;
                        EdadInicioContactoSexual = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INICIO_CONTACTO_SEXUAL;
                        HuidasHogar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.HUIDAS_HOGAR_ABANDONO_FAMILIAR != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.HUIDAS_HOGAR_ABANDONO_FAMILIAR : -1;
                        return;
                    }

                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES != null)
                    {
                        ControlTab = 5;
                        #region Comentado
                        //RecibeVisitaFamiliar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR;
                        //Frecuencia = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_VISITA_FAMILIAR;
                        //VisitaIntima = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA;
                        //ApoyoEconomico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO;
                        //CantidadFrecuencia = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_APOYO_ECONOMICO;
                        //CantidadApoyoEconomico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_APOYO_ECONOMICO;
                        //VivePadre = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE == null ? SelectIngreso.IMPUTADO.PADRE_FINADO : AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE;
                        //ViveMadre = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE == null ? SelectIngreso.IMPUTADO.MADRE_FINADO : AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE;
                        //PadresVivenJuntos = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.PADRES_JUNTOS;
                        //FallecioPadre = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_PADRE;
                        //FallecioMadre = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_MADRE;
                        //MotivoSeparacion = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.MOTIVOS_SEPARACION;
                        //EdadInternoSeparacionPadres = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_SEPARACION;
                        //TotalParejas = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.TOTAL_PAREJAS;
                        //CuantasUnion = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_PAREJAS_UNION;
                        //ContactoNombre = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_NOMBRE;
                        //ContactoParentesco = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_CONTACTO_PARENTESCO;
                        ////ContactoTelefono = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO;
                        //TextContactoTelefono = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.ToString();
                        //Social = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL;
                        //Economico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_ECONOMICO;
                        //Cultural = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL;
                        //Hijos = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS;
                        //Registrados = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.HIJOS_REGISTRADOS;
                        //CuantosMantieneRelacion = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_RELACION;
                        //CuantosVisitan = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_VISITA;
                        //AbandonoFamiliar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR;
                        //EspecifiqueAbandonoFamiliar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR;
                        //MaltratoEmocional = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL;
                        //EspecifiqueMaltratoEmocional = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL;
                        //MaltratoFisico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO;
                        //EspecifiqueMaltratoFisico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO;
                        //AbusoSexual = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL;
                        //EspecifiqueAbusoSexual = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL;
                        //EdadAbuso = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_ABUSO_SEXUAL;
                        //EdadInicioContactoSexual = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INICIO_CONTACTO_SEXUAL;
                        //HuidasHogar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.HUIDAS_HOGAR_ABANDONO_FAMILIAR;
                        #endregion

                        RecibeVisitaFamiliar = !string.IsNullOrEmpty(AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR) ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR : "";
                        Frecuencia = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_VISITA_FAMILIAR != null ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_VISITA_FAMILIAR : -1;
                        VisitaIntima = !string.IsNullOrEmpty(AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA) ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA : "";
                        ApoyoEconomico = !string.IsNullOrEmpty(AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO) ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO : "";
                        CantidadFrecuencia = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_APOYO_ECONOMICO != null ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_APOYO_ECONOMICO : -1;
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
                        ContactoParentesco = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_CONTACTO_PARENTESCO != null ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_CONTACTO_PARENTESCO : -1;
                        //ContactoTelefono = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO;
                        TextContactoTelefono = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.ToString();
                        Social = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL != null ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL : -1;
                        Economico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_ECONOMICO != null ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_ECONOMICO : -1;
                        Cultural = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL != null ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL : -1;
                        Hijos = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS;
                        Registrados = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.HIJOS_REGISTRADOS;
                        CuantosMantieneRelacion = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_RELACION;
                        CuantosVisitan = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_VISITA;
                        AbandonoFamiliar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR;
                        EspecifiqueAbandonoFamiliar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR != null ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR : -1;
                        MaltratoEmocional = !string.IsNullOrEmpty(AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL) ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL : "";
                        EspecifiqueMaltratoEmocional = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL;
                        MaltratoFisico = !string.IsNullOrEmpty(AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO) ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO : "";
                        EspecifiqueMaltratoFisico = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO;
                        AbusoSexual = !string.IsNullOrEmpty(AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL) ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL : "";
                        EspecifiqueAbusoSexual = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL;
                        EdadAbuso = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_ABUSO_SEXUAL;
                        EdadInicioContactoSexual = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INICIO_CONTACTO_SEXUAL;
                        HuidasHogar = AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.HUIDAS_HOGAR_ABANDONO_FAMILIAR != null ? AnteriorEMI.EMI_FACTORES_SOCIO_FAMILIARES.HUIDAS_HOGAR_ABANDONO_FAMILIAR : -1;
                        return;
                    }

                //NUEVO REGISTRO
                RecibeVisitaFamiliar = VisitaIntima = ApoyoEconomico = CantidadApoyoEconomico = VivePadre = ViveMadre = PadresVivenJuntos = MotivoSeparacion = ContactoNombre = AbandonoFamiliar = MaltratoEmocional = EspecifiqueMaltratoEmocional = MaltratoFisico = EspecifiqueMaltratoFisico = AbusoSexual = EspecifiqueAbusoSexual = string.Empty;
                Frecuencia = CantidadFrecuencia = ContactoParentesco = EspecifiqueAbandonoFamiliar = HuidasHogar = Social = Economico = Cultural = -1;
                FallecioPadre = FallecioMadre = EdadInternoSeparacionPadres = TotalParejas = CuantasUnion = Hijos = Registrados = CuantosMantieneRelacion = CuantosVisitan = EdadAbuso = EdadInicioContactoSexual = 0;
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

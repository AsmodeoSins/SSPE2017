using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel : ValidationViewModelBase
    {
        private void PopulateUsoDrogas()
        {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual != null)
                    if (emiActual.EMI_USO_DROGA != null)
                    {
                        if (emiActual.EMI_USO_DROGA.Count > 0)
                        {
                            ConductasParasocialesEnabled = UsoDrogaEnabled = GrupoFamiliarEnabled = AntecedenteGrupoFamiliarEnabled = true;
                            LstUsoDrogas = new ObservableCollection<EMI_USO_DROGA>(emiActual.EMI_USO_DROGA);
                            IsUsoDrogasEmpty = LstUsoDrogas.Count > 0 ? false : true;
                            return;
                        }
                    }
                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_USO_DROGA != null)
                    {
                        LstUsoDrogas = new ObservableCollection<EMI_USO_DROGA>(AnteriorEMI.EMI_USO_DROGA);
                        IsUsoDrogasEmpty = LstUsoDrogas.Count > 0 ? false : true;
                        return;
                    }
                //NUEVO REGISTRO
                LstUsoDrogas = new ObservableCollection<EMI_USO_DROGA>();
                IsUsoDrogasEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer uso drogas", ex);
            }
        }

        private void PopulateHPS()
        {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual != null)
                    if (emiActual.EMI_HPS != null)
                    {
                        ConductasParasocialesEnabled = HPSEnabled = UsoDrogaEnabled = GrupoFamiliarEnabled = AntecedenteGrupoFamiliarEnabled = true;
                        VivioCalleOrfanato = !string.IsNullOrEmpty(emiActual.EMI_HPS.VIVIO_CALLE_ORFANATO_NINO) ? emiActual.EMI_HPS.VIVIO_CALLE_ORFANATO_NINO : string.Empty;
                        #region Homosexual
                        Homosexual = !string.IsNullOrEmpty(emiActual.EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL) ? emiActual.EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL : string.Empty;
                        HomosexualEdadIncial = emiActual.EMI_HPS.EDAD_INICIAL_HOMOSEXUAL != null ? emiActual.EMI_HPS.EDAD_INICIAL_HOMOSEXUAL : 0;
                        HomosexualRol = !string.IsNullOrEmpty(emiActual.EMI_HPS.ROL_HOMOSEXUAL) ? emiActual.EMI_HPS.ROL_HOMOSEXUAL : string.Empty;
                        Id_Homo = emiActual.EMI_HPS.ID_HOMO != null ? emiActual.EMI_HPS.ID_HOMO : -1;
                        #endregion
                        #region pandilleo
                        PertenecePandilla = !string.IsNullOrEmpty(emiActual.EMI_HPS.PERTENECE_PANDILLA_ACTUAL) ? emiActual.EMI_HPS.PERTENECE_PANDILLA_ACTUAL : string.Empty;
                        PertenecioPandillaExterior = !string.IsNullOrEmpty(emiActual.EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR) ? emiActual.EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR : string.Empty;
                        PandillaExteriorEdadInicial = emiActual.EMI_HPS.EDAD_INICIAL_PANDILLAS != null ? emiActual.EMI_HPS.EDAD_INICIAL_PANDILLAS : 0;
                        PandillaExteriorMotivo = emiActual.EMI_HPS.MOTIVOS_PERTENENCIA_PANDILLAS;
                        PandillaNombre = emiActual.EMI_HPS.ID_PANDILLA != null ? emiActual.EMI_HPS.ID_PANDILLA : -1;
                        NombrePandilla = emiActual.EMI_HPS.PANDILLA;
                        #endregion
                        #region Vagancia
                        Vagancia = !string.IsNullOrEmpty(emiActual.EMI_HPS.VAGANCIA) ? emiActual.EMI_HPS.VAGANCIA : string.Empty;
                        VaganciaEdadIncial = emiActual.EMI_HPS.EDAD_INICIAL_VAGANCIA != null ? emiActual.EMI_HPS.EDAD_INICIAL_VAGANCIA : 0;
                        VaganciaMotivos = emiActual.EMI_HPS.MOTIVOS_VAGANCIA;
                        #endregion
                        #region Cicatrices
                        Cicatrices = !string.IsNullOrEmpty(emiActual.EMI_HPS.CICATRICES) ? emiActual.EMI_HPS.CICATRICES : string.Empty;
                        CicatricesEdadIncial = emiActual.EMI_HPS.EDAD_INICIO_CICATRICES != null ? emiActual.EMI_HPS.EDAD_INICIO_CICATRICES : 0;
                        CicatricesMotivo = emiActual.EMI_HPS.MOTIVO_CICATRICES;
                        CicatricesRina = string.IsNullOrEmpty(emiActual.EMI_HPS.CICATRIZ_POR_RINA) ? false : emiActual.EMI_HPS.CICATRIZ_POR_RINA == "S" ? true : false;
                        #endregion
                        #region Desercion Escolar
                        DesercionEscolar = !string.IsNullOrEmpty(emiActual.EMI_HPS.DESERCION_ESCOLAR) ? emiActual.EMI_HPS.DESERCION_ESCOLAR : string.Empty;
                        DesercionMotivo = emiActual.EMI_HPS.MOTIVO_DESERCION_ESCOLAR;
                        #endregion
                        #region Reprobación escolar
                        ReprobacionEscolar = !string.IsNullOrEmpty(emiActual.EMI_HPS.REPROBACION_ESCOLAR) ? emiActual.EMI_HPS.REPROBACION_ESCOLAR : string.Empty;
                        ReprobacionEscolarMotivo = emiActual.EMI_HPS.MOTIVO_REPROBACION_ESCOLAR;
                        ReprobacionGrado = emiActual.EMI_HPS.GRADO_REPROBACION_ESCOLAR != null ? emiActual.EMI_HPS.GRADO_REPROBACION_ESCOLAR : -1;
                        #endregion
                        #region Expulsion escolar
                        ExplusionEscolar = !string.IsNullOrEmpty(emiActual.EMI_HPS.EXPULSION_ESCOLAR) ? emiActual.EMI_HPS.EXPULSION_ESCOLAR : string.Empty;
                        ExplusionEscolarMotivo = emiActual.EMI_HPS.MOTIVO_EXPULSION_ESCOLAR;
                        ExpulsionGrado = emiActual.EMI_HPS.GRADO_EXPULSION_ESCOLAR != null ? emiActual.EMI_HPS.GRADO_EXPULSION_ESCOLAR : -1;
                        #endregion
                        #region Paga x servicio Sexual
                        ConHombres = string.IsNullOrEmpty(emiActual.EMI_HPS.PAGA_SEXUAL_HOMBRE) ? false : emiActual.EMI_HPS.PAGA_SEXUAL_HOMBRE == "S" ? true : false;
                        ConMujeres = string.IsNullOrEmpty(emiActual.EMI_HPS.PAGA_SEXUAL_MUJER) ? false : emiActual.EMI_HPS.PAGA_SEXUAL_MUJER == "S" ? true : false;
                        #endregion
                        #region se prostituia
                        SeProstituiaHombres = string.IsNullOrEmpty(emiActual.EMI_HPS.PROSTITUIA_HOMBRES) ? false : emiActual.EMI_HPS.PROSTITUIA_HOMBRES == "S" ? true : false;
                        SeProstituiaMujeres = string.IsNullOrEmpty(emiActual.EMI_HPS.PROSTITUIA_MUJERES) ? false : emiActual.EMI_HPS.PROSTITUIA_MUJERES == "S" ? true : false;
                        MotivoProstituye = emiActual.EMI_HPS.PROSTITUYE_POR != null ? emiActual.EMI_HPS.PROSTITUYE_POR : -1;
                        #endregion
                        return;
                    }

                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_HPS != null)
                    {
                        VivioCalleOrfanato = !string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.VIVIO_CALLE_ORFANATO_NINO) ? AnteriorEMI.EMI_HPS.VIVIO_CALLE_ORFANATO_NINO : string.Empty;
                        #region Homosexual
                        Homosexual = !string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL) ? AnteriorEMI.EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL : string.Empty;
                        HomosexualEdadIncial = AnteriorEMI.EMI_HPS.EDAD_INICIAL_HOMOSEXUAL != null ? AnteriorEMI.EMI_HPS.EDAD_INICIAL_HOMOSEXUAL : 0;
                        HomosexualRol = !string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.ROL_HOMOSEXUAL) ? AnteriorEMI.EMI_HPS.ROL_HOMOSEXUAL : string.Empty;
                        Id_Homo = AnteriorEMI.EMI_HPS.ID_HOMO != null ? AnteriorEMI.EMI_HPS.ID_HOMO : -1;
                        #endregion
                        #region pandilleo
                        PertenecePandilla = !string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.PERTENECE_PANDILLA_ACTUAL) ? AnteriorEMI.EMI_HPS.PERTENECE_PANDILLA_ACTUAL : string.Empty;
                        PertenecioPandillaExterior = !string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR) ? AnteriorEMI.EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR : string.Empty;
                        PandillaExteriorEdadInicial = AnteriorEMI.EMI_HPS.EDAD_INICIAL_PANDILLAS != null ? AnteriorEMI.EMI_HPS.EDAD_INICIAL_PANDILLAS : 0;
                        PandillaExteriorMotivo = AnteriorEMI.EMI_HPS.MOTIVOS_PERTENENCIA_PANDILLAS;
                        PandillaNombre = AnteriorEMI.EMI_HPS.ID_PANDILLA != null ? AnteriorEMI.EMI_HPS.ID_PANDILLA : -1;
                        NombrePandilla = AnteriorEMI.EMI_HPS.PANDILLA;
                        #endregion
                        #region Vagancia
                        Vagancia = !string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.VAGANCIA) ? AnteriorEMI.EMI_HPS.VAGANCIA : string.Empty;
                        VaganciaEdadIncial = AnteriorEMI.EMI_HPS.EDAD_INICIAL_VAGANCIA != null ? AnteriorEMI.EMI_HPS.EDAD_INICIAL_VAGANCIA : 0;
                        VaganciaMotivos = AnteriorEMI.EMI_HPS.MOTIVOS_VAGANCIA;
                        #endregion
                        #region Cicatrices
                        Cicatrices = !string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.CICATRICES) ? AnteriorEMI.EMI_HPS.CICATRICES : string.Empty;
                        CicatricesEdadIncial = AnteriorEMI.EMI_HPS.EDAD_INICIO_CICATRICES != null ? AnteriorEMI.EMI_HPS.EDAD_INICIO_CICATRICES : 0;
                        CicatricesMotivo = AnteriorEMI.EMI_HPS.MOTIVO_CICATRICES;
                        CicatricesRina = string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.CICATRIZ_POR_RINA) ? false : AnteriorEMI.EMI_HPS.CICATRIZ_POR_RINA == "S" ? true : false;
                        #endregion
                        #region Desercion Escolar
                        DesercionEscolar = !string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.DESERCION_ESCOLAR) ? AnteriorEMI.EMI_HPS.DESERCION_ESCOLAR : string.Empty;
                        DesercionMotivo = AnteriorEMI.EMI_HPS.MOTIVO_DESERCION_ESCOLAR;
                        #endregion
                        #region Reprobación escolar
                        ReprobacionEscolar = !string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.REPROBACION_ESCOLAR) ? AnteriorEMI.EMI_HPS.REPROBACION_ESCOLAR : string.Empty;
                        ReprobacionEscolarMotivo = AnteriorEMI.EMI_HPS.MOTIVO_REPROBACION_ESCOLAR;
                        ReprobacionGrado = AnteriorEMI.EMI_HPS.GRADO_REPROBACION_ESCOLAR != null ? AnteriorEMI.EMI_HPS.GRADO_REPROBACION_ESCOLAR : -1;
                        #endregion
                        #region Expulsion escolar
                        ExplusionEscolar = !string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.EXPULSION_ESCOLAR) ? AnteriorEMI.EMI_HPS.EXPULSION_ESCOLAR : string.Empty;
                        ExplusionEscolarMotivo = AnteriorEMI.EMI_HPS.MOTIVO_EXPULSION_ESCOLAR;
                        ExpulsionGrado = AnteriorEMI.EMI_HPS.GRADO_EXPULSION_ESCOLAR != null ? AnteriorEMI.EMI_HPS.GRADO_EXPULSION_ESCOLAR : -1;
                        #endregion
                        #region Paga x servicio Sexual
                        ConHombres = string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.PAGA_SEXUAL_HOMBRE) ? false : AnteriorEMI.EMI_HPS.PAGA_SEXUAL_HOMBRE == "S" ? true : false;
                        ConMujeres = string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.PAGA_SEXUAL_MUJER) ? false : AnteriorEMI.EMI_HPS.PAGA_SEXUAL_MUJER == "S" ? true : false;
                        #endregion
                        #region se prostituia
                        SeProstituiaHombres = string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.PROSTITUIA_HOMBRES) ? false : AnteriorEMI.EMI_HPS.PROSTITUIA_HOMBRES == "S" ? true : false;
                        SeProstituiaMujeres = string.IsNullOrEmpty(AnteriorEMI.EMI_HPS.PROSTITUIA_MUJERES) ? false : AnteriorEMI.EMI_HPS.PROSTITUIA_MUJERES == "S" ? true : false;
                        MotivoProstituye = AnteriorEMI.EMI_HPS.PROSTITUYE_POR != null ? AnteriorEMI.EMI_HPS.PROSTITUYE_POR : -1;
                        #endregion
                        return;
                    }

                //NUEVOPROCESO REGISTRO
                VivioCalleOrfanato = Homosexual = HomosexualRol = PertenecePandilla = PertenecioPandillaExterior = PandillaExteriorMotivo = Vagancia = VaganciaMotivos = Cicatrices = CicatricesMotivo = DesercionEscolar = DesercionMotivo = ReprobacionEscolar = ReprobacionEscolarMotivo = ExplusionEscolar = ExplusionEscolarMotivo = NombrePandilla = string.Empty;
                HomosexualEdadIncial = PandillaExteriorEdadInicial = VaganciaEdadIncial = CicatricesEdadIncial = 0;
                PandillaNombre = ReprobacionGrado = ExpulsionGrado = MotivoProstituye = Id_Homo = -1;
                CicatricesRina = ConHombres = ConMujeres = SeProstituiaHombres = SeProstituiaMujeres = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer HPS", ex);
            }
        }

        private void PopulateTatuajes()
        {
            try
            {
                //INFORMACION ACTUAL
                if (emiActual != null)
                    if (emiActual.EMI_TATUAJE != null)
                    {
                        TatuajesEnabled = SeniasParticularesEnabled = true;
                        CantidadAntesIngresoAntisocial = emiActual.EMI_TATUAJE.ANTISOCIAL_AI;
                        CantidadIntramurosAntisocial = emiActual.EMI_TATUAJE.ANTISOCIAL_I;

                        CantidadAntesIngresoErotico = emiActual.EMI_TATUAJE.EROTICO_AI;
                        CantidadIntramurosErotico = emiActual.EMI_TATUAJE.EROTICO_I;

                        CantidadAntesIngresoReligioso = emiActual.EMI_TATUAJE.RELIGIOSO_AI;
                        CantidadIntramurosReligioso = emiActual.EMI_TATUAJE.RELIGIOSO_I;

                        CantidadAntesIngresoIdentificacion = emiActual.EMI_TATUAJE.IDENTIFICACION_AI;
                        CantidadIntramurosIdentificacion = emiActual.EMI_TATUAJE.IDENTIFICACION_I;

                        CantidadAntesIngresoDecorativo = emiActual.EMI_TATUAJE.DECORATIVO_AI;
                        CantidadIntramurosDecorativo = emiActual.EMI_TATUAJE.DECORATIVO_I;

                        CantidadAntesIngresoSentimental = emiActual.EMI_TATUAJE.SENTIMENTAL_AI;
                        CantidadIntramurosSentimental = emiActual.EMI_TATUAJE.SENTIMENTAL_I;

                        TatuajesDescripcion = emiActual.EMI_TATUAJE.DESCR;
                        TatuajesTotal = emiActual.EMI_TATUAJE.TOTAL_TATUAJES;
                        return;
                    }
                //INFORMACION HISTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_TATUAJE != null)
                    {
                        CantidadAntesIngresoAntisocial = AnteriorEMI.EMI_TATUAJE.ANTISOCIAL_AI;
                        CantidadIntramurosAntisocial = AnteriorEMI.EMI_TATUAJE.ANTISOCIAL_I;

                        CantidadAntesIngresoErotico = AnteriorEMI.EMI_TATUAJE.EROTICO_AI;
                        CantidadIntramurosErotico = AnteriorEMI.EMI_TATUAJE.EROTICO_I;

                        CantidadAntesIngresoReligioso = AnteriorEMI.EMI_TATUAJE.RELIGIOSO_AI;
                        CantidadIntramurosReligioso = AnteriorEMI.EMI_TATUAJE.RELIGIOSO_I;

                        CantidadAntesIngresoIdentificacion = AnteriorEMI.EMI_TATUAJE.IDENTIFICACION_AI;
                        CantidadIntramurosIdentificacion = AnteriorEMI.EMI_TATUAJE.IDENTIFICACION_I;

                        CantidadAntesIngresoDecorativo = AnteriorEMI.EMI_TATUAJE.DECORATIVO_AI;
                        CantidadIntramurosDecorativo = AnteriorEMI.EMI_TATUAJE.DECORATIVO_I;

                        CantidadAntesIngresoSentimental = AnteriorEMI.EMI_TATUAJE.SENTIMENTAL_AI;
                        CantidadIntramurosSentimental = AnteriorEMI.EMI_TATUAJE.SENTIMENTAL_I;

                        TatuajesDescripcion = AnteriorEMI.EMI_TATUAJE.DESCR;
                        TatuajesTotal = AnteriorEMI.EMI_TATUAJE.TOTAL_TATUAJES;
                        return;
                    }
                //NUEVO REGISTRO
                CantidadAntesIngresoAntisocial = CantidadIntramurosAntisocial = CantidadAntesIngresoErotico = CantidadIntramurosErotico = CantidadAntesIngresoReligioso = CantidadIntramurosReligioso = CantidadAntesIngresoIdentificacion = CantidadIntramurosIdentificacion = CantidadAntesIngresoDecorativo = CantidadIntramurosDecorativo = CantidadAntesIngresoSentimental = CantidadIntramurosSentimental = 0;
                TatuajesDescripcion = string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer tatuajes", ex);
            }
        }

        private void PopulateEnfermedades()
        {
            try
            {
                //informacion actual
                if (emiActual != null)
                    if (emiActual.EMI_ENFERMEDAD != null)
                    {
                        EnfermedadesEnabled = ActividadesEnabled = true;
                        DescripcionPresentarlasAntecedentes = emiActual.EMI_ENFERMEDAD.DESCR_ENFERMEDAD;
                        AparienciaFisicaAlineado = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.APFISICA_ALINADO) ? emiActual.EMI_ENFERMEDAD.APFISICA_ALINADO : "";
                        AparienciaFisicaConformado = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.APFISICA_CONFORMADO) ? emiActual.EMI_ENFERMEDAD.APFISICA_CONFORMADO : "";
                        AparienciaFisicaIntegro = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.APFISICA_INTEGRO) ? emiActual.EMI_ENFERMEDAD.APFISICA_INTEGRO : "";
                        AparienciaFisicaLimpio = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.APFISICA_LIMPIO) ? emiActual.EMI_ENFERMEDAD.APFISICA_LIMPIO : "";
                        Discapacidades = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.DISCAPACIDAD) ? emiActual.EMI_ENFERMEDAD.DISCAPACIDAD : "";
                        DiscapacidadesMotivo = emiActual.EMI_ENFERMEDAD.DESCR_DISCAPACIDAD;
                        EnfermoMental = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.ENFERMO_MENTAL) ? emiActual.EMI_ENFERMEDAD.ENFERMO_MENTAL : "";
                        EnfermoMentalMotivo = emiActual.EMI_ENFERMEDAD.DESCR_ENFERMO_MENTAL;
                        VIHHepatitis = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.VIH_HEPATITIS) ? emiActual.EMI_ENFERMEDAD.VIH_HEPATITIS : "";
                        VIHHepatitisTratamientoFarmaco = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO) ? emiActual.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO : "";
                        VIHHepatitisDiagnosticoFormal = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL) ? emiActual.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL : "";
                        return;
                    }

                //INFORMACION HOSTORICA
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_ENFERMEDAD != null)
                    {
                        #region comentado
                        //DescripcionPresentarlasAntecedentes = AnteriorEMI.EMI_ENFERMEDAD.DESCR_ENFERMEDAD;
                        //AparienciaFisicaAlineado = AnteriorEMI.EMI_ENFERMEDAD.APFISICA_ALINADO;
                        //AparienciaFisicaConformado = AnteriorEMI.EMI_ENFERMEDAD.APFISICA_CONFORMADO;
                        //AparienciaFisicaIntegro = AnteriorEMI.EMI_ENFERMEDAD.APFISICA_INTEGRO;
                        //AparienciaFisicaLimpio = AnteriorEMI.EMI_ENFERMEDAD.APFISICA_LIMPIO;
                        //Discapacidades = AnteriorEMI.EMI_ENFERMEDAD.DISCAPACIDAD;
                        //DiscapacidadesMotivo = AnteriorEMI.EMI_ENFERMEDAD.DESCR_DISCAPACIDAD;
                        //EnfermoMental = AnteriorEMI.EMI_ENFERMEDAD.ENFERMO_MENTAL;
                        //EnfermoMentalMotivo = AnteriorEMI.EMI_ENFERMEDAD.DESCR_ENFERMO_MENTAL;
                        //VIHHepatitis = AnteriorEMI.EMI_ENFERMEDAD.VIH_HEPATITIS;
                        //VIHHepatitisTratamientoFarmaco = AnteriorEMI.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO;
                        //VIHHepatitisDiagnosticoFormal = AnteriorEMI.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL;
                        #endregion
                        DescripcionPresentarlasAntecedentes = AnteriorEMI.EMI_ENFERMEDAD.DESCR_ENFERMEDAD;
                        AparienciaFisicaAlineado = !string.IsNullOrEmpty(AnteriorEMI.EMI_ENFERMEDAD.APFISICA_ALINADO) ? AnteriorEMI.EMI_ENFERMEDAD.APFISICA_ALINADO : "";
                        AparienciaFisicaConformado = !string.IsNullOrEmpty(AnteriorEMI.EMI_ENFERMEDAD.APFISICA_CONFORMADO) ? AnteriorEMI.EMI_ENFERMEDAD.APFISICA_CONFORMADO : "";
                        AparienciaFisicaIntegro = !string.IsNullOrEmpty(AnteriorEMI.EMI_ENFERMEDAD.APFISICA_INTEGRO) ? AnteriorEMI.EMI_ENFERMEDAD.APFISICA_INTEGRO : "";
                        AparienciaFisicaLimpio = !string.IsNullOrEmpty(AnteriorEMI.EMI_ENFERMEDAD.APFISICA_LIMPIO) ? AnteriorEMI.EMI_ENFERMEDAD.APFISICA_LIMPIO : "";
                        Discapacidades = !string.IsNullOrEmpty(AnteriorEMI.EMI_ENFERMEDAD.DISCAPACIDAD) ? AnteriorEMI.EMI_ENFERMEDAD.DISCAPACIDAD : "";
                        DiscapacidadesMotivo = AnteriorEMI.EMI_ENFERMEDAD.DESCR_DISCAPACIDAD;
                        EnfermoMental = !string.IsNullOrEmpty(AnteriorEMI.EMI_ENFERMEDAD.ENFERMO_MENTAL) ? AnteriorEMI.EMI_ENFERMEDAD.ENFERMO_MENTAL : "";
                        EnfermoMentalMotivo = AnteriorEMI.EMI_ENFERMEDAD.DESCR_ENFERMO_MENTAL;
                        VIHHepatitis = !string.IsNullOrEmpty(AnteriorEMI.EMI_ENFERMEDAD.VIH_HEPATITIS) ? AnteriorEMI.EMI_ENFERMEDAD.VIH_HEPATITIS : "";
                        VIHHepatitisTratamientoFarmaco = !string.IsNullOrEmpty(AnteriorEMI.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO) ? AnteriorEMI.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO : "";
                        VIHHepatitisDiagnosticoFormal = !string.IsNullOrEmpty(AnteriorEMI.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL) ? AnteriorEMI.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL : "";
                        return;
                    }
                //NUEVO REGISTRO
                DescripcionPresentarlasAntecedentes = AparienciaFisicaAlineado = AparienciaFisicaConformado = AparienciaFisicaIntegro = AparienciaFisicaLimpio = Discapacidades = DiscapacidadesMotivo = EnfermoMental = EnfermoMentalMotivo = VIHHepatitisTratamientoFarmaco = VIHHepatitisDiagnosticoFormal = VIHHepatitis = string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer enfermedades", ex);
            }
        }
    }
}

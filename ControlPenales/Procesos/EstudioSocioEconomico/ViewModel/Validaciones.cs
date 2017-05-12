using System;

namespace ControlPenales
{
    partial class EstudioSocioEconomicoViewModel
    {
        void SetValidaciones() 
        {
            try
            {
                //base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso.HasValue ? SelectTipoIngreso.Value >= 0 : false, "TIPO DE INGRESO ES REQUERIDO!");

                //estas son las reglas basicas de la ventana, algunas reglas se activan y desactivan mediante eventos disparadores
                base.ClearRules();
                base.AddRule(() => GrupoFamiliarPrimario, () => !string.IsNullOrEmpty(GrupoFamiliarPrimario), "GRUPO FAMILIAR PRIMARIO ES REQUERIDO!");
                base.AddRule(() => RelacionIntroFamiliarPrimario, () => !string.IsNullOrEmpty(RelacionIntroFamiliarPrimario), "RELACION INTRO FAMILIAR PRIMARIO ES REQUERIDO!");
                base.AddRule(() => NoPersonasVivenHogar, () => NoPersonasVivenHogar.HasValue , "EL NUMERO DE PERSONAS QUE VIVEN EN EL HOGAR PRIMARIO ES REQUERIDO!");
                base.AddRule(() => NoPersonasTrabajanPrimario, () => NoPersonasTrabajanPrimario.HasValue , "EL NUMERO DE PERSONAS QUE TRABAJAN ES REQUERIDO");
                base.AddRule(() => IngresoMensualPrimario, () => IngresoMensualPrimario.HasValue ? IngresoMensualPrimario.Value >= 0 : false, "EL INGRESO MENSUAL PRIMARIO ES REQUERIDO");
                base.AddRule(() => EgresoMensualPrimario, () => EgresoMensualPrimario.HasValue ? EgresoMensualPrimario.Value >= 0 : false, "EL EGRESO MENSUAL PRIMARIO ES REQUERIDO");
                base.AddRule(() => FamiliarAntecedentePrimario, () => FamiliarAntecedentePrimario.HasValue ? FamiliarAntecedentePrimario != -1 ? true : false : false, "EL ANTECEDENTE FAMILIAR PRIMARIO ES REQUERIDO");
                base.AddRule(() => ViviendaZonaPrimario, () => !string.IsNullOrEmpty(ViviendaZonaPrimario), "LA ZONA DE LA VIVIENDA PRIMARIA ES REQUERIDO");
                base.AddRule(() => ViviendaCondicionesPrimario, () => !string.IsNullOrEmpty(ViviendaCondicionesPrimario), "LAS CONDICION DE LA VIVIENDA PRIMARIA ES REQUERIDO");
                base.AddRule(() => NivelSocioCulturalPrimario, () => !string.IsNullOrEmpty(NivelSocioCulturalPrimario), "EL NIVEL SOCIO CULTURAL PRIMARIO REQUERIDO");
                base.AddRule(() => Salario, () => Salario.HasValue , "EL SALARIO QUE PERCIBIA ES REQUERIDO!");
                base.AddRule(() => GrupoFamiliarSecundario, () => !string.IsNullOrEmpty(GrupoFamiliarSecundario), "GRUPO FAMILIAR SECUNDARIO ES REQUERIDO!");
                base.AddRule(() => RelacionIntroFamiliarSecundario, () => !string.IsNullOrEmpty(RelacionIntroFamiliarSecundario), "GRUPO FAMILIAR SECUNDARIO ES REQUERIDO!");
                base.AddRule(() => PersonasLaboranSecundario, () => PersonasLaboranSecundario.HasValue ? PersonasLaboranSecundario.Value >= 0 : false, "PERSONAS QUE LABORAN SECUNDARIO ES REQUERIDO!");
                base.AddRule(() => IngresoMensualSecundario, () => IngresoMensualSecundario.HasValue ? IngresoMensualSecundario.Value >= 0 : false, "INGRESO MENSUAL SECUNDARIO ES REQUERIDO!");
                base.AddRule(() => EgresoMensualSecundario, () => EgresoMensualSecundario.HasValue ? EgresoMensualSecundario.Value >= 0 : false, "EGRESO MENSUAL SECUNDARIO ES REQUERIDO!");
                base.AddRule(() => FamiliarAntecedenteSecundario, () => FamiliarAntecedenteSecundario.HasValue ? FamiliarAntecedenteSecundario != -1 ? true : false : false, "EGRESO MENSUAL SECUNDARIO ES REQUERIDO!");
                base.AddRule(() => ViviendaZonaSecundario, () => !string.IsNullOrEmpty(ViviendaZonaSecundario), "ZONA DE LA VIVIENDA SECUNDARIA ES REQUERIDO!");
                base.AddRule(() => ViviendaCondicionesSecundario, () => !string.IsNullOrEmpty(ViviendaCondicionesSecundario), "CONDICIONES DE VIVIENDA SECUNDARIO ES REQUERIDO!");
                base.AddRule(() => NivelSocioCulturalSecundario, () => !string.IsNullOrEmpty(NivelSocioCulturalSecundario), "NIVEL SOCIO CULTURAL SECUNDARIO ES REQUERIDO!");
                base.AddRule(() => RecibeVisita, () => RecibeVisita.HasValue ? RecibeVisita != -1 ? true : false : false, "RECIBE VISITA SECUNDARIO ES REQUERIDO!");
                base.AddRule(() => ApoyoEconomico, () => ApoyoEconomico.HasValue ? ApoyoEconomico != -1 ? true : false : false, "APOYO ECONOMICO ES REQUERIDO!");
                base.AddRule(() => FechaEstudio, () => FechaEstudio.HasValue ? FechaEstudio.Value <= Fechas.GetFechaDateServer ? true : false : false, "LA FECHA SELECCIONADA NO PUEDE SER MAYOR AL DIA ACTUAL");
                base.AddRule(() => DictamenDescripcion, () => !string.IsNullOrEmpty(DictamenDescripcion), "EL DICTAMEN ES REQUERIDO!");


                OnPropertyChanged("DictamenDescripcion");
                OnPropertyChanged("Salario");
                OnPropertyChanged("GrupoFamiliarPrimario");
                OnPropertyChanged("RelacionIntroFamiliarPrimario");
                OnPropertyChanged("NoPersonasVivenHogar");
                OnPropertyChanged("NoPersonasTrabajanPrimario");
                OnPropertyChanged("IngresoMensualPrimario");
                OnPropertyChanged("EgresoMensualPrimario");
                OnPropertyChanged("FamiliarAntecedentePrimario");
                OnPropertyChanged("ViviendaZonaPrimario");
                OnPropertyChanged("ViviendaCondicionesPrimario");
                OnPropertyChanged("NivelSocioCulturalPrimario");
                OnPropertyChanged("GrupoFamiliarSecundario");
                OnPropertyChanged("RelacionIntroFamiliarSecundario");
                OnPropertyChanged("PersonasLaboranSecundario");
                OnPropertyChanged("IngresoMensualSecundario");
                OnPropertyChanged("EgresoMensualSecundario");
                OnPropertyChanged("FamiliarAntecedenteSecundario");
                OnPropertyChanged("ViviendaZonaSecundario");
                OnPropertyChanged("ViviendaCondicionesSecundario");
                OnPropertyChanged("NivelSocioCulturalSecundario");
                OnPropertyChanged("RecibeVisita");
                OnPropertyChanged("ApoyoEconomico");
                OnPropertyChanged("FechaEstudio");
            }

            catch (Exception exc)
            {
            }
        }
    }
}

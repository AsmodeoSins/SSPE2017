using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class ReportePsicologicoViewModel
    {
        //void ValidarAlias()
        //{
        //    base.ClearRules();
        //    base.AddRule(() => NombreAlias, () => !string.IsNullOrEmpty(NombreAlias), "NOMBRE ES REQUERIDO!");
        //    base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ES REQUERIDO!");
        //    base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ES REQUERIDO!");
        //    OnPropertyChanged("NombreAlias");
        //    OnPropertyChanged("PaternoAlias");
        //    OnPropertyChanged("MaternoAlias");
        //}

        //void ValidacionGeneral()
        //{
        //    base.ClearRules();
        //    #region Datos Generales Entrevista
        //    base.AddRule(() => TextLugarEntrevista, () => !string.IsNullOrEmpty(TextLugarEntrevista), "LUGAR ENTREVISTA ES REQUERIDO!");
        //    base.AddRule(() => TextFechaEntrv, () => TextFechaEntrv!=null, "FECHA ENTREVISTA ES REQUERIDO!");
        //    base.AddRule(() => TextHoraEntrevista, () => !string.IsNullOrEmpty(TextHoraEntrevista) != null, "HORA ENTREVISTA ES REQUERIDO!");
        //    #endregion


        //    base.AddRule(() => TextDescripcionEntrv, () => !string.IsNullOrEmpty(TextDescripcionEntrv), "DESCRIPCION ES REQUERIDO!");
        //    base.AddRule(() => TextTecnicasUtilizadas, () => !string.IsNullOrEmpty(TextTecnicasUtilizadas), "TECNICAS UTILIZADAS ES REQUERIDO!");
        //    base.AddRule(() => TextExamenMental, () => !string.IsNullOrEmpty(TextExamenMental), "EXAMEN MENTAL ES REQUERIDO!");
        //    base.AddRule(() => TextPersonalidad, () => !string.IsNullOrEmpty(TextExamenMental), "PERSONALIDAD MENTAL ES REQUERIDO!");
        //    base.AddRule(() => TextNuceloFamPrimario, () => !string.IsNullOrEmpty(TextNuceloFamPrimario), "NUCLEO FAMILIAR PRIMARIO ES REQUERIDO!");
        //    base.AddRule(() => TextNuceloFamSecundario, () => !string.IsNullOrEmpty(TextNuceloFamSecundario), "NUCLEO FAMILIAR SECUNDARIO ES REQUERIDO!");

        //    base.AddRule(() => TextObsrv, () => !string.IsNullOrEmpty(TextObsrv), "OBSERVACION ES REQUERIDO!");
        //    base.AddRule(() => TextSugerencia, () => !string.IsNullOrEmpty(TextSugerencia), "SUGERENCIA ES REQUERIDA!");

        //    OnPropertyChanged("TextLugarEntrevista");
        //    OnPropertyChanged("TextFechaEntrv");
        //    OnPropertyChanged("TextHoraEntrevista");
        //    OnPropertyChanged("TextDescripcionEntrv");
        //    OnPropertyChanged("TextTecnicasUtilizadas");
        //    OnPropertyChanged("TextExamenMental");
        //    OnPropertyChanged("TextPersonalidad");
        //    OnPropertyChanged("TextLugarEntrevista");
        //    OnPropertyChanged("TextLugarEntrevista");


        //}



        void ValidacionDatosFamiliar()
        {
            base.ClearRules();
            #region Datos Generales
            
            #endregion

            base.AddRule(() => TextLugarEntrevista, () => !string.IsNullOrEmpty(TextLugarEntrevista), "LUGAR ENTREVISTA ES REQUERIDO!");
            base.AddRule(() => TextFechaEntrv, () => TextFechaEntrv != null, "FECHA ENTREVISTA ES REQUERIDO!");
            base.AddRule(() => InicioDiaDomingo, () => InicioDiaDomingo != null, "HORA ENTREVISTA ES REQUERIDO!");

            OnPropertyChanged("TextLugarEntrevista");
            OnPropertyChanged("TextFechaEntrv");
            OnPropertyChanged("InicioDiaDomingo");

            base.AddRule(() => TextNombreFamiliar, () => !string.IsNullOrEmpty(TextNombreFamiliar), "NOMBRE FAMILIAR ES REQUERIDO!");
            base.AddRule(() => SelectParentesco, () => SelectParentesco > -1, "PARENTESCO ES REQUERIDO!");
            base.AddRule(() => TextTelefonoFamiliar, () => TextTelefonoFamiliar != null ? TextTelefonoFamiliar.Length >= 14 : false, "TELEFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
            base.AddRule(() => TextCalleFamiliar, () => !string.IsNullOrEmpty(TextCalleFamiliar), "CALLE ES REQUERIDO!");
            base.AddRule(() => TextNumExteriorFamiliar, () => !string.IsNullOrEmpty(TextNumExteriorFamiliar), "NUMERO EXTERIOR ES REQUERIDO!");

            OnPropertyChanged("TextNombreFamiliar");
            OnPropertyChanged("SelectParentesco");
            OnPropertyChanged("TextTelefonoFamiliar");
            OnPropertyChanged("TextCalleFamiliar");
            OnPropertyChanged("TextNumExteriorFamiliar");

            #region Descripcion Entrevistado
            base.AddRule(() => TextDescripcionEntrv, () => !string.IsNullOrEmpty(TextDescripcionEntrv), "DECRIPCIÓN ENTREVISTADO ES REQUERIDO!");
            base.AddRule(() => TextTecnicasUtilizadas, () => !string.IsNullOrEmpty(TextTecnicasUtilizadas), "TÉCNICAS UTLIZADAS ES REQUERIDA!");
            base.AddRule(() => TextExamenMental, () => !string.IsNullOrEmpty(TextExamenMental), "EXAMEN MENTAL ES REQUERIDA!");
            base.AddRule(() => TextPersonalidad, () => !string.IsNullOrEmpty(TextPersonalidad), "PERSONALIDAD REQUERIDA!");
            base.AddRule(() => TextNuceloFamPrimario, () => !string.IsNullOrEmpty(TextNuceloFamPrimario), "NUCLEO FAMILIAR PRIMARIO  REQUERIDO!");
            base.AddRule(() => TextNuceloFamSecundario, () => !string.IsNullOrEmpty(TextNuceloFamSecundario), "NUCLEO FAMILIAR SECUNDARIO REQUERIDO!");
            base.AddRule(() => TextObsrv, () => !string.IsNullOrEmpty(TextObsrv), "OBSERVACIÓN REQUERIDO!");
            base.AddRule(() => TextSugerencia, () => !string.IsNullOrEmpty(TextSugerencia), "SUGERENCIA ES REQUERIDO!");

            OnPropertyChanged("TextDescripcionEntrv");
            OnPropertyChanged("TextTecnicasUtilizadas");
            OnPropertyChanged("TextExamenMental");
            OnPropertyChanged("TextPersonalidad");
            OnPropertyChanged("TextNuceloFamPrimario");
            OnPropertyChanged("TextNuceloFamSecundario");
            OnPropertyChanged("TextObsrv");
            OnPropertyChanged("TextSugerencia");
            #endregion
            
        }

       

      

        //void ValidarApodo()
        //{
        //    base.ClearRules();
        //    base.AddRule(() => Apodo, () => !string.IsNullOrEmpty(Apodo), "APODO ES REQUERIDO!");
        //    OnPropertyChanged("Apodo");
        //}

    }
}

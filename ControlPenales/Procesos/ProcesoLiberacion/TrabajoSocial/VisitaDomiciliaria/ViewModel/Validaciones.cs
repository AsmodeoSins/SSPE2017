using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class VisitaDomiciliariaViewModel
    {
        //void ValidacionesLiberado()
        //{
        //    base.ClearRules();
        //    base.AddRule(() => SelectSexo, () => !string.IsNullOrEmpty(SelectSexo) && (SelectSexo == "F" || SelectSexo == "M"), "SEXO ES REQUERIDO!");
        //    base.AddRule(() => SelectOcupacion, () => SelectOcupacion != null, "OCUPACION ES REQUERIDA!");
        //    base.AddRule(() => SelectReligion, () => SelectReligion != null, "RELIGION ES REQUERIDA!");

        //}



     
        void ValidacionDatosGenerales()
        {
            base.ClearRules();
            base.AddRule(() => TextMedidaCautelar, () => !string.IsNullOrEmpty(TextMedidaCautelar), "MEDIDA CAUTELAR ES REQUERIDA!");
            base.AddRule(() => TextMotivoVisita, () => !string.IsNullOrEmpty(TextMotivoVisita), "MOTIVO VISITA ES REQUERIDA!");
            base.AddRule(() => TextLugarEntrevista, () => !string.IsNullOrEmpty(TextLugarEntrevista), "LUGAR ES REQUERIDO!");
            base.AddRule(() => TextFechaEntrv, () => TextFechaEntrv != null, "FECHA ES REQUERIDA!");
            //base.AddRule(() => HoraEntrv, () => HoraEntrv!=null, "HORA ES REQUERIDA!");

            OnPropertyChanged("HoraEntrv");
           
            OnPropertyChanged("TextLugarEntrevista");
            OnPropertyChanged("TextFechaEntrv");
            OnPropertyChanged("TextMedidaCautelar");
            OnPropertyChanged("TextMotivoVisita");
        }

        void ValidacionPersonaEntrevistada()
        {
            #region DatosPersonaEntrevfistada

            base.AddRule(() => TextNombreEntrevistado, () => !string.IsNullOrEmpty(TextNombreEntrevistado), "NOMBRE  ES REQUERIDO!");
            base.AddRule(() => TextEdadEntrevistado, () => !string.IsNullOrEmpty(TextEdadEntrevistado), "EDAD ES REQUERIDO!");
            base.AddRule(() => TextCalleEntrevistado, () => !string.IsNullOrEmpty(TextCalleEntrevistado), "CALLE  ES REQUERIDA!");
           // base.AddRule(() => TextNumeroInteriorEntrevistado, () => !string.IsNullOrEmpty(TextNumeroInteriorEntrevistado), "NUMERO INTERIOR ES REQUERIDA!");
            base.AddRule(() => TextNumeroExteriorEntrevistado, () => !string.IsNullOrEmpty(TextNumeroExteriorEntrevistado), "NUMERO EXTERIOR ES REQUERIDA!");
            base.AddRule(() => TextTelefonoEntrevistado, () => TextTelefonoEntrevistado != null ? TextTelefonoEntrevistado.Length >= 14 : false, "TELÉFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
            base.AddRule(() => SelectParentesco, () => SelectParentesco > -1, "PARENTESCO ES REQUERIDO!");
            base.AddRule(() => TextTiempoConocerceEntrvistado, () => !string.IsNullOrEmpty(TextTiempoConocerceEntrvistado), "TIEMPO DE CONOCERLO ES REQUERIDO!");
            base.AddRule(() => TextRelacionSentenciadoEntrevistado, () => !string.IsNullOrEmpty(TextRelacionSentenciadoEntrevistado), "RELACIÓN CON EL SENTENCIADO  ES REQUERIDO!");
            base.AddRule(() => SelectPaisNacEntrv, () => SelectPaisNacEntrv > -1, "PAÍS ES REQUERIDO!");
            base.AddRule(() => SelectEntidadNacEntrv, () => SelectEntidadNacEntrv > -1, "ESTADO ES REQUERIDO!");
            base.AddRule(() => SelectMunicipioNacEntrv, () => SelectMunicipioNacEntrv > -1, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => TextObservaciones, () => !string.IsNullOrEmpty(TextObservaciones), "OBSERVACIÓN  ES REQUERIDO!");


           
            
            OnPropertyChanged("TextNombreEntrevistado");
            OnPropertyChanged("TextEdadEntrevistado");
            OnPropertyChanged("TextCalleEntrevistado");
           // OnPropertyChanged("TextNumeroInteriorEntrevistado");
            OnPropertyChanged("TextNumeroExteriorEntrevistado");
            OnPropertyChanged("TextTelefonoEntrevistado");
            OnPropertyChanged("SelectParentesco");
            OnPropertyChanged("TextTiempoConocerceEntrvistado");
            OnPropertyChanged("TextRelacionSentenciadoEntrevistado");
            OnPropertyChanged("SelectPaisNacEntrv");
            OnPropertyChanged("SelectEntidadNacEntrv");
            OnPropertyChanged("SelectMunicipioNacEntrv");
            OnPropertyChanged("TextObservaciones");

            

            #endregion
           

        }
        void ValidacionCroquis()
        {
            #region ValidacionCroquis
            base.AddRule(() => TextNombreCroquis, () => !string.IsNullOrEmpty(TextNombreCroquis), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => TextTelCroquis, () => TextTelCroquis != null ? TextTelCroquis.Length >= 14 : false, "TELÉFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
            base.AddRule(() => TextDireccionCroquis, () => !string.IsNullOrEmpty(TextDireccionCroquis), "DIRECCIÓN ES REQUERIDA!");

            OnPropertyChanged("TextNombreCroquis");
            OnPropertyChanged("TextTelCroquis");
            OnPropertyChanged("TextDireccionCroquis");
            #endregion

        }
     
    }
}

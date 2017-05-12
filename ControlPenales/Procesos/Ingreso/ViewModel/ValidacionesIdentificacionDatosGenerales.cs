using System.ComponentModel;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        void setValidacionesIdentificacionDatosGenerales()
        {
            //base.ClearRules();
            #region DATOS_GENERALES
            base.AddRule(() => SelectSexo, () => !string.IsNullOrEmpty(SelectSexo) && (SelectSexo == "F" || SelectSexo == "M"), "SEXO ES REQUERIDO!");
            base.AddRule(() => SelectEstadoCivil, () => SelectEstadoCivil.Value != -1 , "ESTADO CIVIL ES REQUERIDO!");
            base.AddRule(() => SelectOcupacion, () => SelectOcupacion.Value != -1, "OCUPACIÓN ES REQUERIDA!");
            base.AddRule(() => SelectEscolaridad, () => SelectEscolaridad != -1, "ESCOLARIDAD ES REQUERIDA!");
            base.AddRule(() => SelectNacionalidad, () => SelectNacionalidad != -1 , "NACIONALIDAD ES REQUERIDA!");
            base.AddRule(() => SelectReligion, () => SelectReligion.Value != -1, "RELIGIÓN ES REQUERIDA!");
            base.AddRule(() => SelectEtnia, () => SelectEtnia.Value != -1, "ETNIA ES REQUERIDA!");
            
            base.AddRule(() => SelectedIdioma, () => SelectedIdioma.HasValue ? SelectedIdioma.Value >= 1 : false, "IDIOMA ES REQUERIDO!");
            base.AddRule(() => SelectedDialecto, () => SelectedDialecto.HasValue ? SelectedDialecto.Value >= 1 : false, "DIALECTO ES REQUERIDO!");
            OnPropertyChanged("SelectSexo");
            OnPropertyChanged("SelectEstadoCivil");
            OnPropertyChanged("SelectOcupacion");
            OnPropertyChanged("SelectEscolaridad");
            OnPropertyChanged("SelectNacionalidad");
            OnPropertyChanged("SelectReligion");
            OnPropertyChanged("SelectEtnia");
            OnPropertyChanged("SelectedIdioma");
            OnPropertyChanged("SelectedDialecto");
            #endregion

            #region DOMICILIO
            base.AddRule(() => SelectPais, () => SelectPais.HasValue ? SelectPais.Value != -1 : false, "PAIS ES REQUERIDO!");
            base.AddRule(() => SelectEntidad, () => SelectEntidad.HasValue ? SelectEntidad != -1 : false, "ENTIDAD ES REQUERIDA!");
            base.AddRule(() => SelectMunicipio, () => SelectMunicipio.HasValue ? SelectMunicipio != -1 : false, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => SelectColonia, () => SelectColonia != null ? SelectColonia >= 1 : false, "COLONIA ES REQUERIDA!");
            
            
            base.AddRule(() => TextCalle, () => !string.IsNullOrEmpty(TextCalle), "CALLE ES REQUERIDA!");
            base.AddRule(() => TextNumeroExterior, () => TextNumeroExterior.HasValue ? TextNumeroExterior.Value > 0 : false, "NUMERO EXTERIOR ES REQUERIDO!");
            base.AddRule(() => AniosEstado, () => !string.IsNullOrEmpty(AniosEstado), "TIEMPO DE REFERENCIA EN EL ESTADO ES REQUERIDO!");
            base.AddRule(() => MesesEstado, () => !string.IsNullOrEmpty(MesesEstado), "TIEMPO DE REFERENCIA EN EL ESTADO ES REQUERIDO!");
            base.AddRule(() => TextTelefono, () => TextTelefono != null ? TextTelefono.Length >= 14 : false , "TELEFONO ES REQUERIDO Y DEBE CONTENER 10 DIGITOS!");
            base.AddRule(() => TextCodigoPostal, () => TextCodigoPostal.HasValue ? TextCodigoPostal.Value >= 1 : false, "CODIGO POSTAL ES REQUERIDO!");
            OnPropertyChanged("SelectPais");
            OnPropertyChanged("SelectEntidad");
            OnPropertyChanged("SelectMunicipio");
            OnPropertyChanged("SelectColonia");
            OnPropertyChanged("TextCalle");
            OnPropertyChanged("TextNumeroExterior");
            OnPropertyChanged("AniosEstado");
            OnPropertyChanged("MesesEstado");
            OnPropertyChanged("TextTelefono");
            OnPropertyChanged("TextCodigoPostal");
            #endregion

            #region NACIMIENTO
            base.AddRule(() => SelectPaisNacimiento, () => SelectPaisNacimiento.HasValue ? SelectPaisNacimiento.Value != -1 : false, "PAIS DE NACIMIENTO ES REQUERIDO!");
            base.AddRule(() => SelectEntidadNacimiento, () => SelectEntidadNacimiento.HasValue ? SelectEntidadNacimiento.Value != -1 : false, "ENTIDAD DE NACIMIENTO ES REQUERIDO!");
            base.AddRule(() => SelectMunicipioNacimiento, () => SelectMunicipioNacimiento.HasValue ? SelectMunicipioNacimiento.Value != -1 : false, "MUNICIPIO DE NACIMIENTO ES REQUERIDO!");
            base.AddRule(() => TextFechaNacimiento, () => TextFechaNacimiento.HasValue, "FECHA DE NACIMIENTO ES REQUERIDA!");
            OnPropertyChanged("SelectPaisNacimiento");
            OnPropertyChanged("SelectEntidadNacimiento");
            OnPropertyChanged("SelectMunicipioNacimiento");
            OnPropertyChanged("TextFechaNacimiento");
            #endregion

            #region PADRE
            base.AddRule(() => TextPadrePaterno, () => !string.IsNullOrEmpty(TextPadrePaterno), "APELLIDO PATERNO DEL PADRE ES REQUERIDO!");
            base.AddRule(() => TextPadreMaterno, () => !string.IsNullOrEmpty(TextPadreMaterno), "APELLIDO MATERNO DEL PADRE ES REQUERIDO!");
            base.AddRule(() => TextPadreNombre, () => !string.IsNullOrEmpty(TextPadreNombre), "NOMBRE(S) DEL PADRE ES REQUERIDA!");

            if (!MismoDomicilioPadre && !CheckPadreFinado)
            {
                #region DOMICILIO PADRE
                base.AddRule(() => SelectPaisDomicilioPadre, () => (SelectPaisDomicilioPadre.HasValue ? SelectPaisDomicilioPadre.Value != -1 : false) 
                    && CheckPadreFinado == false && MismoDomicilioPadre == false, "PAIS ES REQUERIDO!");
                base.AddRule(() => SelectEntidadDomicilioPadre, () => (SelectEntidadDomicilioPadre.HasValue ? SelectEntidadDomicilioPadre.Value != -1 : false) 
                    && CheckPadreFinado == false && MismoDomicilioPadre == false, "ENTIDAD ES REQUERIDA!");
                base.AddRule(() => SelectMunicipioDomicilioPadre, () => (SelectMunicipioDomicilioPadre.HasValue ? SelectMunicipioDomicilioPadre.Value != -1 : false) 
                    && CheckPadreFinado == false && MismoDomicilioPadre == false, "MUNICIPIO ES REQUERIDO!");
                base.AddRule(() => SelectColoniaDomicilioPadre, () => (SelectColoniaDomicilioPadre != null ? SelectColoniaDomicilioPadre != -1 : false) 
                    && CheckPadreFinado == false && MismoDomicilioPadre == false, "COLONIA ES REQUERIDA!");
                base.AddRule(() => TextCalleDomicilioPadre, () => !string.IsNullOrEmpty(TextCalleDomicilioPadre) && CheckPadreFinado == false && MismoDomicilioPadre == false, "CALLE ES REQUERIDA!");
                base.AddRule(() => TextNumeroExteriorDomicilioPadre, () => (TextNumeroExteriorDomicilioPadre.HasValue ? TextNumeroExteriorDomicilioPadre.Value > 0 : false) 
                    && CheckPadreFinado == false && MismoDomicilioPadre == false, "NUMERO EXTERIOR ES REQUERIDO!");
                //base.AddRule(() => TextNumeroInteriorDomicilioPadre, () => !string.IsNullOrEmpty(TextNumeroInteriorDomicilioPadre) && CheckPadreFinado == false && MismoDomicilioPadre == false, "NUMERO INTERIOR ES REQUERIDO!");
                base.AddRule(() => TextCodigoPostalDomicilioPadre, () => (TextCodigoPostalDomicilioPadre.HasValue ? TextCodigoPostalDomicilioPadre.Value >= 1 : false) 
                    && CheckPadreFinado == false && MismoDomicilioPadre == false, "CODIGO POSTAL ES REQUERIDO!");
                #endregion
            }
            OnPropertyChanged("TextPadrePaterno");
            OnPropertyChanged("TextPadreMaterno");
            OnPropertyChanged("TextPadreNombre");
            OnPropertyChanged("SelectPaisDomicilioPadre");
            OnPropertyChanged("SelectEntidadDomicilioPadre");
            OnPropertyChanged("SelectMunicipioDomicilioPadre");
            OnPropertyChanged("SelectColoniaDomicilioPadre");
            OnPropertyChanged("TextCalleDomicilioPadre");
            OnPropertyChanged("TextNumeroExteriorDomicilioPadre");
            OnPropertyChanged("TextCodigoPostalDomicilioPadre");
            #endregion

            #region MADRE
            base.AddRule(() => TextMadrePaterno, () => !string.IsNullOrEmpty(TextMadrePaterno), "APELLIDO PATERNO DE LA MADRE ES REQUERIDO!");
            base.AddRule(() => TextMadreMaterno, () => !string.IsNullOrEmpty(TextMadreMaterno), "APELLIDO MATERNO DE LA MADRE ES REQUERIDO!");
            base.AddRule(() => TextMadreNombre, () => !string.IsNullOrEmpty(TextMadreNombre), "NOMBRE(S) DE LA MADRE ES REQUERIDO!");

            if (!MismoDomicilioMadre && !CheckMadreFinado)
            {
                #region DOMICILIO PADRE
                base.AddRule(() => SelectPaisDomicilioMadre, () => (SelectPaisDomicilioMadre.HasValue ? SelectPaisDomicilioMadre.Value != -1 : false)
                    && CheckMadreFinado == false && MismoDomicilioMadre == false, "PAIS ES REQUERIDO!");
                base.AddRule(() => SelectEntidadDomicilioMadre, () => (SelectEntidadDomicilioMadre.HasValue ? SelectEntidadDomicilioMadre.Value != -1 : false) 
                    && CheckMadreFinado == false && MismoDomicilioMadre == false, "ENTIDAD ES REQUERIDA!");
                base.AddRule(() => SelectMunicipioDomicilioMadre, () => (SelectMunicipioDomicilioMadre.HasValue ? SelectMunicipioDomicilioMadre.Value != -1 : false) 
                    && CheckMadreFinado == false && MismoDomicilioMadre == false, "MUNICIPIO ES REQUERIDO!");
                base.AddRule(() => SelectColoniaDomicilioMadre, () => (SelectColoniaDomicilioMadre != null ? SelectColoniaDomicilioMadre != -1 : false)
                    && CheckMadreFinado == false && MismoDomicilioMadre == false, "COLONIA ES REQUERIDA!");
                base.AddRule(() => TextCalleDomicilioMadre, () => !string.IsNullOrEmpty(TextCalleDomicilioMadre) && CheckMadreFinado == false && MismoDomicilioMadre == false, "CALLE ES REQUERIDA!");
                base.AddRule(() => TextNumeroExteriorDomicilioMadre, () => (TextNumeroExteriorDomicilioMadre.HasValue ? TextNumeroExteriorDomicilioMadre.Value
                    > 0 : false) && CheckMadreFinado == false && MismoDomicilioMadre == false, "NUMERO EXTERIOR ES REQUERIDO!");
                //base.AddRule(() => TextNumeroInteriorDomicilioMadre, () => !string.IsNullOrEmpty(TextNumeroInteriorDomicilioMadre) && CheckMadreFinado == false && MismoDomicilioMadre == false, "NUMERO INTERIOR ES REQUERIDO!");
                base.AddRule(() => TextCodigoPostalDomicilioMadre, () => (TextCodigoPostalDomicilioMadre.HasValue ? TextCodigoPostalDomicilioMadre.Value >= 1 : false) 
                    && CheckMadreFinado == false && MismoDomicilioMadre == false, "CÓDIGO POSTAL ES REQUERIDO!");
                #endregion
            }
            OnPropertyChanged("TextMadrePaterno");
            OnPropertyChanged("TextMadreMaterno");
            OnPropertyChanged("TextMadreNombre");
            OnPropertyChanged("SelectPaisDomicilioMadre");
            OnPropertyChanged("SelectEntidadDomicilioMadre");
            OnPropertyChanged("SelectMunicipioDomicilioMadre");
            OnPropertyChanged("SelectColoniaDomicilioMadre");
            OnPropertyChanged("TextCalleDomicilioMadre");
            OnPropertyChanged("TextNumeroExteriorDomicilioMadre");
            OnPropertyChanged("TextCodigoPostalDomicilioMadre");
            #endregion
        }
    }
}

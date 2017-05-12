using System;
using System.ComponentModel;

namespace ControlPenales
{
    partial class JuridicoIdentificacionViewModel
    {
        void setValidacionesIdentificacionDatosGenerales()
        {
            try
            {
                base.ClearRules();

                #region DATOS_GENERALES
                base.AddRule(() => SelectSexo, () => !string.IsNullOrEmpty(SelectSexo) && (SelectSexo == "F" || SelectSexo == "M"), "SEXO ES REQUERIDO!");
                base.AddRule(() => SelectEstadoCivil, () => SelectEstadoCivil.HasValue ? SelectEstadoCivil >= 1 : false, "ESTADO CIVIL ES REQUERIDO!");
                base.AddRule(() => SelectOcupacion, () => SelectOcupacion.HasValue ? SelectOcupacion >= 1 : false, "OCUPACION ES REQUERIDA!");
                base.AddRule(() => SelectEscolaridad, () => SelectEscolaridad.HasValue ? SelectEscolaridad >= 1 : false, "ESCOLARIDAD ES REQUERIDA!");
                base.AddRule(() => SelectNacionalidad, () => SelectNacionalidad.HasValue ? SelectNacionalidad >= 1 : false, "NACIONALIDAD ES REQUERIDA!");
                base.AddRule(() => SelectReligion, () => SelectReligion.HasValue ? SelectReligion >= 1 : false, "RELIGION ES REQUERIDA!");
                base.AddRule(() => SelectEtnia, () => SelectEtnia.HasValue ? SelectEtnia >= 1 : false, "ETNIA ES REQUERIDA!");
                base.AddRule(() => TextPeso, () => !string.IsNullOrEmpty(TextPeso), "PESO ES REQUERIDO!");
                base.AddRule(() => TextEstatura, () => !string.IsNullOrEmpty(TextEstatura), "ESTATURA ES REQUERIDA!");
                base.AddRule(() => SelectedIdioma, () => SelectedIdioma.HasValue ? SelectedIdioma >= 1 : false, "IDIOMA ES REQUERIDO!");
                base.AddRule(() => SelectedDialecto, () => SelectedDialecto.HasValue ? SelectedDialecto >= 1 : false, "DIALECTO ES REQUERIDO!");
                #endregion

                #region DOMICILIO
                base.AddRule(() => SelectPais, () => SelectPais.HasValue ? SelectPais >= 1 : true, "PAÍS ES REQUERIDO!");
                base.AddRule(() => SelectEntidad, () => SelectEntidad.HasValue ? SelectEntidad >= 1 : true, "ENTIDAD ES REQUERIDA!");
                base.AddRule(() => SelectMunicipio, () => SelectMunicipio.HasValue ? SelectMunicipio >= 1 : true, "MUNICIPIO ES REQUERIDO!");
                base.AddRule(() => SelectColonia, () => SelectColonia.HasValue ? SelectColonia >= 0 : true, "COLONIA ES REQUERIDA!");
                base.AddRule(() => TextCalle, () => !string.IsNullOrEmpty(TextCalle), "CALLE ES REQUERIDA!");
                base.AddRule(() => TextNumeroExterior, () => TextNumeroExterior > 0, "NUMERO EXTERIOR ES REQUERIDO!");
                //base.AddRule(() => TextNumeroInterior, () => !string.IsNullOrEmpty(TextNumeroInterior), "NUMERO INTERIOR ES REQUERIDO!");
                base.AddRule(() => AniosEstado, () => !string.IsNullOrEmpty(AniosEstado), "TIEMPO DE REFERENCIA EN EL ESTADO ES REQUERIDO!");
                base.AddRule(() => MesesEstado, () => !string.IsNullOrEmpty(MesesEstado), "TIEMPO DE REFERENCIA EN EL ESTADO ES REQUERIDO!");
                base.AddRule(() => TextTelefono, () => TextTelefono != null ? TextTelefono.Length >= 14 : false, "TELEFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
                base.AddRule(() => TextCodigoPostal, () => TextCodigoPostal >= 1, "CÓDIGO POSTAL ES REQUERIDO!");
                //base.AddRule(() => TextDomicilioTrabajo, () => !string.IsNullOrEmpty(TextDomicilioTrabajo), "DOMICILIO DE TRABAJO ES REQUERIDO!");
                #endregion

                #region NACIMIENTO
                base.AddRule(() => SelectPaisNacimiento, () => SelectPaisNacimiento.HasValue ? SelectPaisNacimiento >= 1 : true, "PAÍS DE NACIMIENTO ES REQUERIDO!");
                base.AddRule(() => SelectEntidadNacimiento, () => SelectEntidadNacimiento.HasValue ? SelectEntidadNacimiento >= 1 : true, "ENTIDAD DE NACIMIENTO ES REQUERIDO!");
                base.AddRule(() => SelectMunicipioNacimiento, () => SelectMunicipioNacimiento.HasValue ? SelectMunicipioNacimiento >= 1 : true, "MUNICIPIO DE NACIMIENTO ES REQUERIDO!");
                //if (TextFechaNacimiento.HasValue)
                base.AddRule(() => TextFechaNacimiento, () =>TextFechaNacimiento.HasValue,"FECHA DE NACIMIENTO ES REQUERIDA");
                #endregion

                #region PADRE
                base.AddRule(() => TextPadrePaterno, () => !string.IsNullOrEmpty(TextPadrePaterno), "APELLIDO PATERNO DEL PADRE ES REQUERID!");
                base.AddRule(() => TextPadreMaterno, () => !string.IsNullOrEmpty(TextPadreMaterno), "APELLIDO MATERNO DEL PADRE ES REQUERIDO!");
                base.AddRule(() => TextPadreNombre, () => !string.IsNullOrEmpty(TextPadreNombre), "NOMBRE(S) DEL PADRE ES REQUERIDO!");
                if (!MismoDomicilioPadre && !CheckPadreFinado)
                {
                    #region DOMICILIO PADRE
                    base.AddRule(() => SelectPaisDomicilioPadre, () => (SelectPaisDomicilioPadre.HasValue ? SelectPaisDomicilioPadre >= 1 : false) && CheckPadreFinado == false && MismoDomicilioPadre == false, "PAIS ES REQUERIDO!");
                    base.AddRule(() => SelectEntidadDomicilioPadre, () => (SelectEntidadDomicilioPadre.HasValue ? SelectEntidadDomicilioPadre >= 1 : false) && CheckPadreFinado == false && MismoDomicilioPadre == false, "ENTIDAD ES REQUERIDA!");
                    base.AddRule(() => SelectMunicipioDomicilioPadre, () => (SelectMunicipioDomicilioPadre.HasValue ? SelectMunicipioDomicilioPadre >= 1 : false) && CheckPadreFinado == false && MismoDomicilioPadre == false, "MUNICIPIO ES REQUERIDO!");
                    base.AddRule(() => SelectColoniaDomicilioPadre, () => (SelectColoniaDomicilioPadre.HasValue ? SelectColoniaDomicilioPadre >= 1 : false) && CheckPadreFinado == false && MismoDomicilioPadre == false, "COLONIA ES REQUERIDA!");
                    base.AddRule(() => TextCalleDomicilioPadre, () => !string.IsNullOrEmpty(TextCalleDomicilioPadre) && CheckPadreFinado == false && MismoDomicilioPadre == false, "CALLE ES REQUERIDA!");
                    base.AddRule(() => TextNumeroExteriorDomicilioPadre, () => TextNumeroExteriorDomicilioPadre > 0 && CheckPadreFinado == false && MismoDomicilioPadre == false, "NUMERO EXTERIOR ES REQUERIDO!");
                    //base.AddRule(() => TextNumeroInteriorDomicilioPadre, () => !string.IsNullOrEmpty(TextNumeroInteriorDomicilioPadre) && CheckPadreFinado == false && MismoDomicilioPadre == false, "NUMERO INTERIOR ES REQUERIDO!");
                    base.AddRule(() => TextCodigoPostalDomicilioPadre, () => TextCodigoPostalDomicilioPadre >= 1 && CheckPadreFinado == false && MismoDomicilioPadre == false, "CÓDIGO POSTAL ES REQUERIDO!");
                    #endregion
                }
                #endregion

                #region MADRE
                base.AddRule(() => TextMadrePaterno, () => !string.IsNullOrEmpty(TextMadrePaterno), "APELLIDO PATERNO DE LA MADRE ES REQUERIDO!");
                base.AddRule(() => TextMadreMaterno, () => !string.IsNullOrEmpty(TextMadreMaterno), "APELLIDO MATERNO DE LA MADRE ES REQUERIDO!");
                base.AddRule(() => TextMadreNombre, () => !string.IsNullOrEmpty(TextMadreNombre), "NOMBRE(S) DE LA MADRE ES REQUERIDO!");

                if (!MismoDomicilioMadre && !CheckMadreFinado)
                {
                    #region DOMICILIO PADRE
                    base.AddRule(() => SelectPaisDomicilioMadre, () => (SelectPaisDomicilioMadre.HasValue ? SelectPaisDomicilioMadre >= 1 : false) && CheckMadreFinado == false && MismoDomicilioMadre == false, "PAIS ES REQUERIDO!");
                    base.AddRule(() => SelectEntidadDomicilioMadre, () => (SelectEntidadDomicilioMadre.HasValue ? SelectEntidadDomicilioMadre >= 1 : false) && CheckMadreFinado == false && MismoDomicilioMadre == false, "ENTIDAD ES REQUERIDA!");
                    base.AddRule(() => SelectMunicipioDomicilioMadre, () => (SelectMunicipioDomicilioMadre.HasValue ? SelectMunicipioDomicilioMadre >= 1 : false) && CheckMadreFinado == false && MismoDomicilioMadre == false, "MUNICIPIO ES REQUERIDO!");
                    base.AddRule(() => SelectColoniaDomicilioMadre, () => (SelectColoniaDomicilioMadre.HasValue ? SelectColoniaDomicilioMadre >= 1 : false) && CheckMadreFinado == false && MismoDomicilioMadre == false, "COLONIA ES REQUERIDA!");
                    base.AddRule(() => TextCalleDomicilioMadre, () => !string.IsNullOrEmpty(TextCalleDomicilioMadre) && CheckMadreFinado == false && MismoDomicilioMadre == false, "CALLE ES REQUERIDA!");
                    base.AddRule(() => TextNumeroExteriorDomicilioMadre, () => TextNumeroExteriorDomicilioMadre > 0 && CheckMadreFinado == false && MismoDomicilioMadre == false, "NUMERO EXTERIOR ES REQUERIDO!");
                    //base.AddRule(() => TextNumeroInteriorDomicilioMadre, () => !string.IsNullOrEmpty(TextNumeroInteriorDomicilioMadre) && CheckMadreFinado == false && MismoDomicilioPadre == false, "NUMERO INTERIOR ES REQUERIDO!");
                    base.AddRule(() => TextCodigoPostalDomicilioMadre, () => TextCodigoPostalDomicilioMadre >= 1 && CheckMadreFinado == false && MismoDomicilioMadre == false, "CÓDIGO POSTAL ES REQUERIDO!");
                    #endregion
                }
                #endregion

                OnPropertyChanged("SelectSexo");
                OnPropertyChanged("SelectEstadoCivil");
                OnPropertyChanged("SelectOcupacion");
                OnPropertyChanged("SelectEscolaridad");
                OnPropertyChanged("SelectNacionalidad");
                OnPropertyChanged("SelectReligion");
                OnPropertyChanged("SelectEtnia");
                OnPropertyChanged("TextPeso");
                OnPropertyChanged("TextEstatura");
                OnPropertyChanged("SelectedIdioma");
                OnPropertyChanged("SelectedDialecto");
                
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

                OnPropertyChanged("SelectPaisNacimiento");
                OnPropertyChanged("SelectEntidadNacimiento");
                OnPropertyChanged("SelectMunicipioNacimiento");
                OnPropertyChanged("TextFechaNacimiento");

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
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear las validaciones.", ex);
            }
        }
    }
}

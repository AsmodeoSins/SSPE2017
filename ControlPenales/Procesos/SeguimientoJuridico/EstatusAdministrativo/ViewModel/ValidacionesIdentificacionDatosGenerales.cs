using System;
using System.ComponentModel;

namespace ControlPenales
{
    partial class EstatusAdministrativoViewModel
    {
        void setValidacionesIdentificacionDatosGenerales()
        {
            try
            {
                base.ClearRules();

                #region DATOS_GENERALES
                base.AddRule(() => SelectSexo, () => !string.IsNullOrEmpty(SelectSexo) && (SelectSexo == "F" || SelectSexo == "M"), "SEXO ES REQUERIDO!");
                base.AddRule(() => SelectEstadoCivil, () => SelectEstadoCivil.HasValue ? SelectEstadoCivil >= 1 : false, "ESTADO CIVIL ES REQUERIDO!");
                base.AddRule(() => SelectOcupacion, () => SelectOcupacion.HasValue ? SelectOcupacion >= 1 : false, "OCUPACIÓN ES REQUERIDA!");
                base.AddRule(() => SelectEscolaridad, () => SelectEscolaridad.HasValue ? SelectEscolaridad >= 1 : false, "ESCOLARIDAD ES REQUERIDA!");
                base.AddRule(() => SelectNacionalidad, () => SelectNacionalidad.HasValue ? SelectNacionalidad >= 1 : false, "NACIONALIDAD ES REQUERIDA!");
                base.AddRule(() => SelectReligion, () => SelectReligion.HasValue ? SelectReligion >= 1 : false, "RELIGIÓN ES REQUERIDA!");
                base.AddRule(() => SelectEtnia, () => SelectEtnia.HasValue ? SelectEtnia >= 1 : false, "ETNIA ES REQUERIDA!");
                base.AddRule(() => TextPeso, () => !string.IsNullOrEmpty(TextPeso), "PESO ES REQUERIDO!");
                base.AddRule(() => TextEstatura, () => !string.IsNullOrEmpty(TextEstatura), "ESTATURA ES REQUERIDA!");
                base.AddRule(() => SelectedIdioma, () => SelectedIdioma.HasValue ? SelectedIdioma >= 1 : false, "IDIOMA ES REQUERIDO!");
                base.AddRule(() => SelectedDialecto, () => SelectedDialecto.HasValue ? SelectedDialecto >= 1 : false, "DIALECTO ES REQUERIDO!");
                #endregion

                #region DOMICILIO
                base.AddRule(() => SelectPais, () => SelectPais.HasValue ? SelectPais >= 1 : false, "PAÍS ES REQUERIDO!");
                base.AddRule(() => SelectEntidad, () => SelectEntidad.HasValue ? SelectEntidad >= 1 : false, "ENTIDAD ES REQUERIDA!");
                base.AddRule(() => SelectMunicipio, () => SelectMunicipio.HasValue ? SelectMunicipio >= 1 : false, "MUNICIPIO ES REQUERIDO!");
                base.AddRule(() => SelectColoniaItem, () => SelectColoniaItem != null ? SelectColoniaItem.ID_COLONIA >= 1 : false, "COLONIA ES REQUERIDA!");
                base.AddRule(() => TextCalle, () => !string.IsNullOrEmpty(TextCalle), "CALLE ES REQUERIDA!");
                base.AddRule(() => TextNumeroExterior, () => TextNumeroExterior.HasValue ? TextNumeroExterior.Value > 0 : false, "NUMERO EXTERIOR ES REQUERIDO!");
                //base.AddRule(() => TextNumeroInterior, () => !string.IsNullOrEmpty(TextNumeroInterior), "NUMERO INTERIOR ES REQUERIDO!");
                base.AddRule(() => AniosEstado, () => !string.IsNullOrEmpty(AniosEstado), "TIEMPO DE REFERENCIA EN EL ESTADO ES REQUERIDO!");
                base.AddRule(() => MesesEstado, () => !string.IsNullOrEmpty(MesesEstado), "TIEMPO DE REFERENCIA EN EL ESTADO ES REQUERIDO!");
                base.AddRule(() => TextTelefono, () => TextTelefono != null ? TextTelefono.Length >= 14 : false, "TELÉFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
                base.AddRule(() => TextCodigoPostal, () => TextCodigoPostal.HasValue ? TextCodigoPostal.Value >= 1 : false, "CÓDIGO POSTAL ES REQUERIDO!");
                //base.AddRule(() => TextDomicilioTrabajo, () => !string.IsNullOrEmpty(TextDomicilioTrabajo), "DOMICILIO DE TRABAJO ES REQUERIDO!");
                #endregion

                #region NACIMIENTO
                base.AddRule(() => SelectPaisNacimiento, () => SelectPaisNacimiento.HasValue ? SelectPaisNacimiento >= 1 : false, "PAÍS DE NACIMIENTO ES REQUERIDO!");
                base.AddRule(() => SelectEntidadNacimiento, () => SelectEntidadNacimiento.HasValue ? SelectEntidadNacimiento >= 1 : false, "ENTIDAD DE NACIMIENTO ES REQUERIDO!");
                base.AddRule(() => SelectMunicipioNacimiento, () => SelectMunicipioNacimiento.HasValue ? SelectMunicipioNacimiento >= 1 : false, "MUNICIPIO DE NACIMIENTO ES REQUERIDO!");
                //base.AddRule(() => TextFechaNacimiento, () => TextFechaNacimiento != null, "FECHA DE NACIMIENTO ES REQUERIDA!");
                base.AddRule(() => TextFechaNacimiento, () => TextFechaNacimiento.HasValue, "FECHA DE NACIMIENTO ES REQUERIDA!");
                //base.AddRule(() => TextLugarNacimientoExtranjero, () => !string.IsNullOrEmpty(TextLugarNacimientoExtranjero), "LUGAR DE NACIMIENTO DE EXTRANJERO ES REQUERIDO!");
                #endregion

                #region PADRE
                base.AddRule(() => TextPadrePaterno, () => !string.IsNullOrEmpty(TextPadrePaterno), "APELLIDO PATERNO DEL PADRE ES REQUERIDO!");
                base.AddRule(() => TextPadreMaterno, () => !string.IsNullOrEmpty(TextPadreMaterno), "APELLIDO MATERNO DEL PADRE ES REQUERIDO!");
                base.AddRule(() => TextPadreNombre, () => !string.IsNullOrEmpty(TextPadreNombre), "NOMBRE(S) DEL PADRE ES REQUERIDO!");

                if (!MismoDomicilioPadre && !CheckPadreFinado)
                {
                    #region DOMICILIO PADRE
                    base.AddRule(() => SelectPaisDomicilioPadre, () => (SelectPaisDomicilioPadre.HasValue ? SelectPaisDomicilioPadre >= 1 : false)
                        && CheckPadreFinado == false && MismoDomicilioPadre == false, "PAIS ES REQUERIDO!");
                    base.AddRule(() => SelectEntidadDomicilioPadre, () => (SelectEntidadDomicilioPadre.HasValue ? SelectEntidadDomicilioPadre >= 1 : false)
                        && CheckPadreFinado == false && MismoDomicilioPadre == false, "ENTIDAD ES REQUERIDA!");
                    base.AddRule(() => SelectMunicipioDomicilioPadre, () => (SelectMunicipioDomicilioPadre.HasValue ? SelectMunicipioDomicilioPadre >= 1 : false)
                        && CheckPadreFinado == false && MismoDomicilioPadre == false, "MUNICIPIO ES REQUERIDO!");
                    base.AddRule(() => SelectColoniaItemDomicilioPadre, () => (SelectColoniaItemDomicilioPadre != null ? SelectColoniaItemDomicilioPadre.ID_COLONIA >= 1 : false)
                        && CheckPadreFinado == false && MismoDomicilioPadre == false, "COLONIA ES REQUERIDA!");
                    base.AddRule(() => TextCalleDomicilioPadre, () => !string.IsNullOrEmpty(TextCalleDomicilioPadre)
                        && CheckPadreFinado == false && MismoDomicilioPadre == false, "CALLE ES REQUERIDA!");
                    base.AddRule(() => TextNumeroExteriorDomicilioPadre, () => TextNumeroExteriorDomicilioPadre.HasValue ? TextNumeroExteriorDomicilioPadre.Value > 0 : false
                        && CheckPadreFinado == false && MismoDomicilioPadre == false, "NUMERO EXTERIOR ES REQUERIDO!");
                    //base.AddRule(() => TextNumeroInteriorDomicilioPadre, () => !string.IsNullOrEmpty(TextNumeroInteriorDomicilioPadre) && CheckPadreFinado == false && MismoDomicilioPadre == false, "NUMERO INTERIOR ES REQUERIDO!");
                    base.AddRule(() => TextCodigoPostalDomicilioPadre, () => TextCodigoPostalDomicilioPadre.HasValue ? TextCodigoPostalDomicilioPadre.Value > 0 : false
                        && CheckPadreFinado == false && MismoDomicilioPadre == false, "CODIGO POSTAL ES REQUERIDO!");

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
                    base.AddRule(() => SelectPaisDomicilioMadre, () => (SelectPaisDomicilioMadre.HasValue ? SelectPaisDomicilioMadre >= 1 : false)
                        && CheckMadreFinado == false && MismoDomicilioPadre == false, "PAÍS ES REQUERIDO!");
                    base.AddRule(() => SelectEntidadDomicilioMadre, () => (SelectEntidadDomicilioMadre.HasValue ? SelectEntidadDomicilioMadre >= 1 : false)
                        && CheckMadreFinado == false && MismoDomicilioPadre == false, "ENTIDAD ES REQUERIDA!");
                    base.AddRule(() => SelectMunicipioDomicilioMadre, () => (SelectMunicipioDomicilioMadre.HasValue ? SelectMunicipioDomicilioMadre >= 1 : false)
                        && CheckMadreFinado == false && MismoDomicilioPadre == false, "MUNICIPIO ES REQUERIDO!");
                    base.AddRule(() => SelectColoniaItemDomicilioMadre, () => (SelectColoniaItemDomicilioMadre != null ? SelectColoniaItemDomicilioMadre.ID_COLONIA >= 1 : false)
                        && CheckMadreFinado == false && MismoDomicilioPadre == false, "COLONIA ES REQUERIDA!");
                    base.AddRule(() => TextCalleDomicilioMadre, () => !string.IsNullOrEmpty(TextCalleDomicilioMadre) && CheckMadreFinado == false && MismoDomicilioPadre == false, "CALLE ES REQUERIDA!");
                    base.AddRule(() => TextNumeroExteriorDomicilioMadre, () => TextNumeroExteriorDomicilioMadre.HasValue ? TextNumeroExteriorDomicilioMadre.Value > 0 : false
                        && CheckMadreFinado == false && MismoDomicilioPadre == false, "NUMERO EXTERIOR ES REQUERIDO!");
                    //base.AddRule(() => TextNumeroInteriorDomicilioMadre, () => !string.IsNullOrEmpty(TextNumeroInteriorDomicilioMadre) && CheckMadreFinado == false && MismoDomicilioPadre == false, "NUMERO INTERIOR ES REQUERIDO!");
                    base.AddRule(() => TextCodigoPostalDomicilioMadre, () => TextCodigoPostalDomicilioMadre.HasValue ? TextCodigoPostalDomicilioMadre.Value > 0 : false
                        && CheckMadreFinado == false && MismoDomicilioPadre == false, "CÓDIGO POSTAL ES REQUERIDO!");
                    #endregion
                }

                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear validaciones.", ex);
            }
        }
    }
}

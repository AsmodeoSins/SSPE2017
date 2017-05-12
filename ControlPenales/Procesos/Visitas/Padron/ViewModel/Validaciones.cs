using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace ControlPenales
{
    partial class PadronVisitasViewModel
    {
        void SetValidacionesHojaVisita()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => SelectSexo, () => SelectSexo != "S", "SEXO ES REQUERIDO!");
                base.AddRule(() => SelectParentesco, () => SelectParentesco > 0, "PARENTESCO ES REQUERIDO!");
                base.AddRule(() => TextCalle, () => !string.IsNullOrEmpty(TextCalle), "CALLE ES REQUERIDA!");
                base.AddRule(() => TextNombre, () => !string.IsNullOrEmpty(TextNombre), "NOMBRE ES REQUERIDO!");
                base.AddRule(() => TextEdad, () => TextEdad.HasValue ? TextEdad > 0 : false, "EDAD ES REQUERIDA!");
                base.AddRule(() => TextTelefono, () => TextTelefono != null ? TextTelefono.Length >= 14 : false, "TELÉFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
                base.AddRule(() => SelectPais, () => SelectPais.HasValue ? SelectPais >= 1 : false, "PAÍS ES REQUERIDO!");
                base.AddRule(() => TextPaterno, () => !string.IsNullOrEmpty(TextPaterno), "APELLIDO PATERNO ES REQUERIDO!");
                base.AddRule(() => TextMaterno, () => !string.IsNullOrEmpty(TextMaterno), "APELLIDO MATERNO ES REQUERIDO!");
                base.AddRule(() => SelectEntidad, () => SelectEntidad.HasValue ? SelectEntidad >= 1 : false, "ENTIDAD ES REQUERIDA!");
                base.AddRule(() => SelectColonia, () => SelectColonia.HasValue ? SelectColonia >= 1 : false, "COLONIA ES REQUERIDA!");
                base.AddRule(() => SelectMunicipio, () => SelectMunicipio.HasValue ? SelectMunicipio >= 1 : false, "MUNICIPIO ES REQUERIDO!");
                OnPropertyChanged("SelectSexo");
                OnPropertyChanged("SelectParentesco");
                OnPropertyChanged("TextCalle");
                OnPropertyChanged("TextNombre");
                OnPropertyChanged("TextEdad");
                OnPropertyChanged("TextTelefono");
                OnPropertyChanged("SelectPais");
                OnPropertyChanged("TextPaterno");
                OnPropertyChanged("TextMaterno");
                OnPropertyChanged("SelectEntidad");
                OnPropertyChanged("SelectColonia");
                OnPropertyChanged("SelectMunicipio");
                //base.AddRule(() => FechaNacimiento, () => FechaNacimiento != null, "FECHA DE NACIMIENTO ES REQUERIDO!");
                //base.AddRule(() => TextNumExt, () => TextNumExt != null && TextNumExt > 0, "NUMERO EXTERIOR ES REQUERIDO!");
                //base.AddRule(() => TextNumeroInterior, () => !string.IsNullOrEmpty(TextNumeroInterior), "NUMERO INTERIOR ES REQUERIDO!");
                //base.AddRule(() => TextCodigoPostal, () => TextCodigoPostal != null && TextCodigoPostal > 0, "CODIGO POSTAL ES REQUERIDO!");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar validaciones.", ex);
            }
        }
        void SetValidacionesGenerales()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => SelectSexo, () => SelectSexo != "S", "SEXO ES REQUERIDO!");
                base.AddRule(() => TextTelefono, () => TextTelefono != null ? TextTelefono.Length >= 14 : false, "TELÉFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
                //base.AddRule(() => TextRfc, () => string.IsNullOrEmpty(TextRfc) ? false : TextRfc.Length == 13, "RFC ES REQUERIDO!");
                base.AddRule(() => TextCurp, () => string.IsNullOrEmpty(TextCurp) ? true : TextCurp.Length == 18, "CURP ES REQUERIDO Y DEBE SER DE 18 CARACTERES!");
                if (SelectImputadoIngreso != null)
                {
                    base.AddRule(() => SelectParentesco, () => SelectParentesco > 0, "RELACIÓN ES REQUERIDA!");
                    base.AddRule(() => SelectEstatusRelacion, () => SelectEstatusRelacion > 0, "RELACIÓN ES REQUERIDA!");
                }
                base.AddRule(() => TextCalle, () => !string.IsNullOrEmpty(TextCalle), "CALLE ES REQUERIDA!");
                base.AddRule(() => TextNombre, () => !string.IsNullOrEmpty(TextNombre), "NOMBRE ES REQUERIDO!");
                //base.AddRule(() => TextPaterno, () => !string.IsNullOrEmpty(TextPaterno), "APELLIDO PATERNO ES REQUERIDO!");
                //base.AddRule(() => TextMaterno, () => !string.IsNullOrEmpty(TextMaterno), "APELLIDO MATERNO ES REQUERIDO!");
                base.AddRule(() => TextCorreo, () => new Correo().ValidarCorreo(TextCorreo), "EL FORMATO DEL CORREO NO ES VALIDO!");
                if (SelectedTab != TabsVisita.ENTREGA_CREDENCIALES)
                    base.AddRule(() => SelectTipoVisitante, () => SelectTipoVisitante.HasValue ? SelectTipoVisitante.Value > 0 : false, "TIPO VISITANTE ES REQUERIDO!");
                base.AddRule(() => SelectPais, () => SelectPais.HasValue ? SelectPais >= 1 : false, "PAÍS ES REQUERIDO!");
                base.AddRule(() => SelectEntidad, () => SelectEntidad.HasValue ? SelectEntidad >= 1 : false, "ENTIDAD ES REQUERIDA!");
                base.AddRule(() => SelectMunicipio, () => SelectMunicipio.HasValue ? SelectMunicipio >= 1 : false, "MUNICIPIO ES REQUERIDO!");
                base.AddRule(() => SelectColonia, () => SelectColonia.HasValue ? SelectColonia >= 1 : false, "COLONIA ES REQUERIDA!");
                base.AddRule(() => SelectDiscapacidad, () => DiscapacidadEnabled ? SelectDiscapacidad.HasValue ? SelectDiscapacidad.Value > 0 :
                    false : true, "TIPO DE DISCAPACIDAD ES REQUERIDO!");
                var fecha = Fechas.GetFechaDateServer;
                base.AddRule(() => FechaNacimiento, () => FechaNacimiento.HasValue ?
                    ((fecha.Year - FechaNacimiento.Value.Year) > 0 && FechaNacimiento.Value < fecha) :
                        false, "FECHA DE NACIMIENTO ES REQUERIDO!");
                OnPropertyChanged("SelectSexo");
                OnPropertyChanged("TextTelefono");
                //OnPropertyChanged("TextRfc");
                //OnPropertyChanged("TextCurp");
                OnPropertyChanged("SelectParentesco");
                OnPropertyChanged("SelectEstatusRelacion");
                OnPropertyChanged("TextCalle");
                OnPropertyChanged("TextNombre");
                //OnPropertyChanged("TextPaterno");
                //OnPropertyChanged("TextMaterno");
                OnPropertyChanged("TextCorreo");
                OnPropertyChanged("SelectTipoVisitante");
                OnPropertyChanged("SelectPais");
                OnPropertyChanged("SelectEntidad");
                OnPropertyChanged("SelectMunicipio");
                OnPropertyChanged("SelectColonia");
                OnPropertyChanged("SelectDiscapacidad");
                OnPropertyChanged("FechaNacimiento");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar validaciones.", ex);
            }
        }
    }
}
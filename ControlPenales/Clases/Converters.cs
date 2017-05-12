using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Medico.CertificadoMedico;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ControlPenales
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateConverter : IValueConverter
    {
        private const string _format = "dd-MM-yyyy";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
                return new Fechas().CalculaEdad((DateTime)value);
            return string.Empty;
        }
        /*
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString(_format);
        }
        */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime DateTimeValue;
            if (DateTime.TryParse((string)value, out DateTimeValue))
                return DateTimeValue;
            return value;
        }
    }

    public class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var Consecutivo = value as System.Windows.Controls.DataGridRow;
            if (Consecutivo == null)
                return value;

            return Consecutivo.GetIndex() + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class GetFuero : IValueConverter
    {
        private const string _format = "dd-MM-yyyy";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
                return new cFuero().ObtenerXID(value.ToString()).DESCR;

            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }

    [ValueConversion(typeof(short), typeof(bool))]
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if ((short)value == 1)
                return true;
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }

    [ValueConversion(typeof(SSP.Servidor.PERSONA), typeof(string))]
    public class GetTipoPersona : IValueConverter
    {
        private const string _format = "dd-MM-yyyy";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is SSP.Servidor.PERSONA)
                {
                    var tipo = string.Empty;
                    tipo = ((SSP.Servidor.PERSONA)value).ABOGADO != null ? ((SSP.Servidor.PERSONA)value).ABOGADO.ABOGADO_TITULO.DESCR : tipo;
                    tipo = ((SSP.Servidor.PERSONA)value).VISITANTE != null ? tipo + (string.IsNullOrEmpty(tipo) ? string.Empty : ", ") + "VISITA" : tipo;
                    //Modificacion de modelo, PENDIENTE
                    tipo = ((SSP.Servidor.PERSONA)value).PERSONA_EXTERNO != null ? tipo + (string.IsNullOrEmpty(tipo) ? string.Empty : ", ") + "EXTERNO" : tipo;
                    tipo = ((SSP.Servidor.PERSONA)value).EMPLEADO != null ? tipo + (string.IsNullOrEmpty(tipo) ? string.Empty : ", ") + "EMPLEADO" : tipo;
                    if (tipo.Trim() == string.Empty)
                    {
                        tipo = "REGISTRO INCORRECTO";
                    }
                    return tipo;
                }
            }
            catch
            {
            }
            return string.Empty;
        }
        /*
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString(_format);
        }
        */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    [ValueConversion(typeof(VISITANTE_INGRESO), typeof(string))]
    public class VisitanteForaneo : IValueConverter
    {
        private const string _format = "dd-MM-yyyy";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is VISITANTE_INGRESO)
                {
                    var tipo = string.Empty;
                    var val = (VISITANTE_INGRESO)value;
                    tipo = val.ID_TIPO_VISITANTE == Parametro.ID_TIPO_VISITANTE_FORANEO ? "EN REVISION" : val.ESTATUS_VISITA.DESCR;
                    return tipo;
                }
            }
            catch
            {
            }
            return string.Empty;
        }
        /*
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString(_format);
        }
        */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(SSP.Servidor.PERSONA), typeof(string))]
    public class GetTipoAduana : IValueConverter
    {
        private const string _format = "dd-MM-yyyy";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ADUANA)
            {
                //var aduana = ((ADUANA)value);
                var tipo = string.Empty;
                if (((ADUANA)value).ID_TIPO_PERSONA.HasValue ? ((ADUANA)value).ID_TIPO_PERSONA.Value > 0 : false)
                {
                    if (((ADUANA)value).ID_TIPO_PERSONA.Value == short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL))
                        tipo = ((ADUANA)value).PERSONA.ABOGADO != null ? ((ADUANA)value).PERSONA.ABOGADO.ABOGADO_TITULO.DESCR : string.Empty;
                    if (((ADUANA)value).ID_TIPO_PERSONA.Value == short.Parse(Parametro.ID_TIPO_PERSONA_VISITA))
                        tipo = ((ADUANA)value).ADUANA_INGRESO.Any(w => w.INTIMA == "S") ? "INTIMA" : "FAMILIAR";
                    if (((ADUANA)value).ID_TIPO_PERSONA.Value == short.Parse(Parametro.ID_TIPO_PERSONA_EXTERNA))
                        tipo = "EXTERNA";
                    if (((ADUANA)value).ID_TIPO_PERSONA.Value == short.Parse(Parametro.ID_TIPO_PERSONA_EMPLEADO))
                        tipo = "EMPLEADO";
                }
                return tipo;
            }
            if (value is BitacoraAduana)
            {
                //var aduana = ((ADUANA)value);
                var tipo = string.Empty;
                if (((BitacoraAduana)value).ADUANA.ID_TIPO_PERSONA.HasValue ? ((BitacoraAduana)value).ADUANA.ID_TIPO_PERSONA.Value > 0 : false)
                {
                    if (((BitacoraAduana)value).ADUANA.ID_TIPO_PERSONA.Value == short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL))
                        tipo = ((BitacoraAduana)value).ADUANA.PERSONA.ABOGADO != null ? ((BitacoraAduana)value).ADUANA.PERSONA.ABOGADO.ABOGADO_TITULO.DESCR : string.Empty;
                    if (((BitacoraAduana)value).ADUANA.ID_TIPO_PERSONA.Value == short.Parse(Parametro.ID_TIPO_PERSONA_VISITA))
                        tipo = ((BitacoraAduana)value).ADUANA.ADUANA_INGRESO.Any(w => w.INTIMA == "S") ? "INTIMA" : "FAMILIAR";
                    if (((BitacoraAduana)value).ADUANA.ID_TIPO_PERSONA.Value == short.Parse(Parametro.ID_TIPO_PERSONA_EXTERNA))
                        tipo = "EXTERNA";
                    if (((BitacoraAduana)value).ADUANA.ID_TIPO_PERSONA.Value == short.Parse(Parametro.ID_TIPO_PERSONA_EMPLEADO))
                        tipo = "EMPLEADO";
                }
                return tipo;
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(INTERCONSULTA_SOLICITUD), typeof(string))]
    public class GetDestinoDescripcion : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)
                return string.Empty;
            var _interconsulta_solicitud = (INTERCONSULTA_SOLICITUD)value;
            if (_interconsulta_solicitud.ID_INTER == (short)enumInterconsulta_Tipo.INTERNA)
            {
                var _sol_inter_interna = _interconsulta_solicitud.SOL_INTERCONSULTA_INTERNA.FirstOrDefault();
                if (_sol_inter_interna == null)
                    return string.Empty;
                return _sol_inter_interna.CENTRO.DESCR;
            }
            else if (_interconsulta_solicitud.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA)
            {
                var _hoja_referencia_medica = _interconsulta_solicitud.HOJA_REFERENCIA_MEDICA.FirstOrDefault();
                if (_hoja_referencia_medica == null)
                    return string.Empty;
                if (_hoja_referencia_medica.ID_HOSPITAL == Parametro.ID_HOSPITAL_OTROS)
                    return _hoja_referencia_medica.HOSPITAL_OTRO;
                else
                    return _hoja_referencia_medica.HOSPITAL.DESCR;
            }
            else
                return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(HOJA_REFERENCIA_MEDICA), typeof(string))]
    public class GetHospitalDescripcion : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)
                return string.Empty;
            var _hoja_referencia_medica = (HOJA_REFERENCIA_MEDICA)value;
            if (_hoja_referencia_medica.ID_HOSPITAL == Parametro.ID_HOSPITAL_OTROS)
                return _hoja_referencia_medica.HOSPITAL_OTRO;
            else
                return _hoja_referencia_medica.HOSPITAL.DESCR;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(INTERCONSULTA_SOLICITUD), typeof(string))]
    public class GetFechaCitaInterconsulta : IValueConverter
    {
        private const string _format = "dd-MM-yyyy";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)
                return string.Empty;
            var _interconsulta_solicitud = (INTERCONSULTA_SOLICITUD)value;
            if (_interconsulta_solicitud.ID_INTER == 1 && _interconsulta_solicitud.SOL_INTERCONSULTA_INTERNA != null && _interconsulta_solicitud.SOL_INTERCONSULTA_INTERNA.Count > 0)
                if (_interconsulta_solicitud.REGISTRO_FEC.HasValue)
                    return _interconsulta_solicitud.REGISTRO_FEC.Value.ToString(_format);
                else
                    return string.Empty;
            else if (_interconsulta_solicitud.HOJA_REFERENCIA_MEDICA != null && _interconsulta_solicitud.HOJA_REFERENCIA_MEDICA.Count > 0)
                if (_interconsulta_solicitud.HOJA_REFERENCIA_MEDICA.First().FECHA_CITA.HasValue)
                    return _interconsulta_solicitud.HOJA_REFERENCIA_MEDICA.First().FECHA_CITA.Value.ToString(_format);
                else
                    return string.Empty;
            else
                return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class Converters
    {
        public string MascaraTelefono(string telefono)
        {
            if (string.IsNullOrEmpty(telefono))
                return string.Empty;
            var fon = telefono.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            var rets = string.Empty;
            switch (fon.Length)
            {
                case 4:
                    rets = System.Text.RegularExpressions.Regex.Replace(fon, @"(\d{3})(\d{1})", "$1-$2");
                    break;
                case 5:
                    rets = System.Text.RegularExpressions.Regex.Replace(fon, @"(\d{3})(\d{2})", "$1-$2");
                    break;
                case 6:
                    rets = System.Text.RegularExpressions.Regex.Replace(fon, @"(\d{3})(\d{3})", "$1-$2");
                    break;
                case 7:
                    rets = System.Text.RegularExpressions.Regex.Replace(fon, @"(\d{3})(\d{4})", "$1-$2");
                    break;
                case 8:
                    rets = System.Text.RegularExpressions.Regex.Replace(fon, @"(\d{3})(\d{3})(\d{2})", "($1) $2-$3");
                    break;
                case 9:
                    rets = System.Text.RegularExpressions.Regex.Replace(fon, @"(\d{3})(\d{3})(\d{3})", "($1) $2-$3");
                    break;
                case 10:
                    rets = System.Text.RegularExpressions.Regex.Replace(fon, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
                    break;
                default:
                    rets = telefono;
                    break;
            }
            return rets;
        }


        public string MascaraTocaPenal(string tp)
        {
            var rets = string.Empty;
            if (tp.Length > 4)
                rets = System.Text.RegularExpressions.Regex.Replace(tp, @"(\d{4})(\d{" + (tp.Length - 4) + "})", @"$1/$2");
            else
                rets = tp;
            return rets;
        }

        public string MascaraHora(string tp)
        {
            var rets = string.Empty;
            if (tp.Length > 4)
                rets = System.Text.RegularExpressions.Regex.Replace(tp, @"(\d{2})(\d{" + (tp.Length - 2) + "})", @"$1:$2");
            else
                rets = tp;
            return rets;
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class PrioridadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (short.Parse(value.ToString()))
            {
                case 1:
                    return "ALTA";
                    break;
                case 2:
                    return "MEDIA";
                    break;
                case 3:
                    return "BAJA";
                    break;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }


    [ValueConversion(typeof(string), typeof(string))]
    public class TensionArterialConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string _valor = value.ToString();
                if (!string.IsNullOrEmpty(_valor))
                {
                    if (_valor.Contains("/"))
                        return _valor;
                    else
                        return string.Format("{0} /", _valor);
                }
                else
                    return string.Empty;
            }

            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }


    [ValueConversion(typeof(string), typeof(string))]
    public class TemperaturaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string _valor = value.ToString();
                if (!string.IsNullOrEmpty(_valor))
                {
                    if (_valor.Contains("°C"))
                        return _valor;

                    else
                        return string.Format("{0} °C", _valor);
                }
                else
                    return string.Empty;
            }

            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }


    [ValueConversion(typeof(string), typeof(string))]
    public class PulsoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string _valor = value.ToString();
                if (!string.IsNullOrEmpty(_valor))
                {
                    if (_valor.Contains("LPM"))
                        return _valor;

                    else
                        return string.Format("{0} LPM", _valor);
                }
                else
                    return string.Empty;
            }

            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }


    [ValueConversion(typeof(string), typeof(string))]
    public class AlturaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string _valor = value.ToString();
                if (!string.IsNullOrEmpty(_valor))
                {
                    if (_valor.Contains("MTS"))
                        return _valor;

                    else
                        return string.Format("{0} MTS", _valor);
                }
                else
                    return string.Empty;
            }

            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class RespConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string _valor = value.ToString();
                if (!string.IsNullOrEmpty(_valor))
                {
                    if (_valor.Contains("RPM"))
                        return _valor;

                    else
                        return string.Format("{0} RPM", _valor);
                }
                else
                    return string.Empty;
            }

            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class PesoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string _valor = value.ToString();
                if (!string.IsNullOrEmpty(_valor))
                {
                    if (_valor.Contains("KG"))
                        return _valor;

                    else
                        return string.Format("{0} KG", _valor);
                }
                else
                    return string.Empty;
            }

            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }

    [ValueConversion(typeof(IMPUTADO), typeof(string))]
    public class NombreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            if (!(value is IMPUTADO))
                return string.Empty;
            var imp = (IMPUTADO)value;
            return string.Format("{0} {1} {2}",
                !string.IsNullOrEmpty(imp.NOMBRE) ? imp.NOMBRE.Trim() : string.Empty,
                !string.IsNullOrEmpty(imp.PATERNO) ? imp.PATERNO.Trim() : string.Empty,
                !string.IsNullOrEmpty(imp.MATERNO) ? imp.MATERNO.Trim() : string.Empty);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }


    [ValueConversion(typeof(IMPUTADO), typeof(string))]
    public class UbicacionByImputadoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string _respuesta = string.Empty;

            if (value == null)
                return _respuesta;

            if (!(value is IMPUTADO))
                return _respuesta;

            var imp = (IMPUTADO)value;
            if (imp.INGRESO.Any())
            {
                var _IngresoActivo = imp.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                if (_IngresoActivo != null)
                    _respuesta = string.Format("{0}-{1}-{2}-{3}",
                        _IngresoActivo.CAMA != null ? _IngresoActivo.CAMA.CELDA != null ? _IngresoActivo.CAMA.CELDA.SECTOR != null ? _IngresoActivo.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(_IngresoActivo.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? _IngresoActivo.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                        _IngresoActivo.CAMA != null ? _IngresoActivo.CAMA.CELDA != null ? _IngresoActivo.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(_IngresoActivo.CAMA.CELDA.SECTOR.DESCR) ? _IngresoActivo.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        _IngresoActivo.CAMA != null ? _IngresoActivo.CAMA.CELDA != null ? !string.IsNullOrEmpty(_IngresoActivo.CAMA.CELDA.ID_CELDA) ? _IngresoActivo.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                        _IngresoActivo.CAMA != null ? _IngresoActivo.CAMA.ID_CAMA.ToString() : string.Empty);
            };

            return _respuesta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }


    [ValueConversion(typeof(VISITANTE_INGRESO), typeof(string))]
    public class VisitanteNombreCompleto : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            if (!(value is VISITANTE_INGRESO))
                return string.Empty;
            var imp = (VISITANTE_INGRESO)value;
            return string.Format("{0} {1} {2}",
                !string.IsNullOrEmpty(imp.VISITANTE.PERSONA.NOMBRE) ? imp.VISITANTE.PERSONA.NOMBRE.Trim() : string.Empty,
                !string.IsNullOrEmpty(imp.VISITANTE.PERSONA.PATERNO) ? imp.VISITANTE.PERSONA.PATERNO.Trim() : string.Empty,
                !string.IsNullOrEmpty(imp.VISITANTE.PERSONA.MATERNO) ? imp.VISITANTE.PERSONA.MATERNO.Trim() : string.Empty);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }


    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolToOppositeBoolConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region MarkupExtension

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }

    [ValueConversion(typeof(INGRESO), typeof(byte[]))]
    public class ImagenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ingreso = (INGRESO)value;

            if (ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                return ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
            else
                return new Imagenes().getImagenPerson();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(short), typeof(byte[]))]
    public class ProcesaImagen : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Imagenes().getImagen("imageNotFound.jpg");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(SSP.Servidor.PERSONA), typeof(byte[]))]
    public class PersonaImagenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var obj = (SSP.Servidor.PERSONA)value;

            if (obj.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                return obj.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
            else
                return new Imagenes().getImagenPerson();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class SexoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            if (value.ToString() == "M")
                return "MASCULINO";
            else
                if (value.ToString() == "F")
                    return "FEMENINO";
                else
                    return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class TipoArchivoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (value.ToString() == "0")
                return "FÍSICO";
            else
                if (value.ToString() == "1")
                    return "DIGITAL";
                else
                    return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class TipoEstatusEmiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (value.ToString() == "C")
                return "COMPLETO";
            else
                if (value.ToString() == "P")
                    return "PENDIENTE";
                else
                    return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class TrimStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is string)
            if (value == null) return string.Empty;

            return value.ToString().Trim();

            //return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }
    [ValueConversion(typeof(SSP.Servidor.PERSONA), typeof(string))]
    public class GetPersonaNombre : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is string)
            if (value == null) return string.Empty;
            if (!(value is SSP.Servidor.PERSONA)) return string.Empty;
            var person = (SSP.Servidor.PERSONA)value;
            return string.Format("{0} {1} {2}",
                !string.IsNullOrEmpty(person.NOMBRE) ? person.NOMBRE.Trim() : string.Empty,
                !string.IsNullOrEmpty(person.PATERNO) ? person.PATERNO.Trim() : string.Empty,
                !string.IsNullOrEmpty(person.MATERNO) ? person.MATERNO.Trim() : string.Empty);

            //return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }
    [ValueConversion(typeof(SSP.Servidor.PERSONA), typeof(string))]
    public class GetPersonaNombreReporteNotasMedicas : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is string)
            if (value == null) return string.Empty;
            if (!(value is ATENCION_MEDICA)) return string.Empty;
            var atMed = (ATENCION_MEDICA)value;
            if (atMed.NOTA_MEDICA != null)
            {
                var person = atMed.NOTA_MEDICA.PERSONA;
                if (person == null)
                    return string.Empty;

                return string.Format("{0} {1} {2}",
                   !string.IsNullOrEmpty(person.NOMBRE) ? person.NOMBRE.Trim() : string.Empty,
                   !string.IsNullOrEmpty(person.PATERNO) ? person.PATERNO.Trim() : string.Empty,
                   !string.IsNullOrEmpty(person.MATERNO) ? person.MATERNO.Trim() : string.Empty);
            }
            if (atMed.ATENCION_CITA.Any())
            {
                var person = atMed.ATENCION_CITA.FirstOrDefault().PERSONA;
                if (person == null)
                    return string.Empty;

                return string.Format("{0} {1} {2}",
                   !string.IsNullOrEmpty(person.NOMBRE) ? person.NOMBRE.Trim() : string.Empty,
                   !string.IsNullOrEmpty(person.PATERNO) ? person.PATERNO.Trim() : string.Empty,
                   !string.IsNullOrEmpty(person.MATERNO) ? person.MATERNO.Trim() : string.Empty);
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class GetNombrePersonaConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (string.IsNullOrEmpty(value.ToString()))
                return string.Empty;

            short _IdEspecialista = short.Parse(value.ToString());
            var _detalleEspecialista = new cEspecialistas().GetData(x => x.ID_ESPECIALISTA == _IdEspecialista).FirstOrDefault();
            if (_detalleEspecialista != null)
                if (_detalleEspecialista.ID_PERSONA.HasValue)
                    return string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(_detalleEspecialista.PERSONA.NOMBRE) ? _detalleEspecialista.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_detalleEspecialista.PERSONA.PATERNO) ? _detalleEspecialista.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_detalleEspecialista.PERSONA.MATERNO) ? _detalleEspecialista.PERSONA.MATERNO.Trim() : string.Empty);
                else
                    return string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(_detalleEspecialista.ESPECIALISTA_NOMBRE) ? _detalleEspecialista.ESPECIALISTA_NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_detalleEspecialista.ESPECIALISTA_PATERNO) ? _detalleEspecialista.ESPECIALISTA_PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_detalleEspecialista.ESPECIALISTA_MATERNO) ? _detalleEspecialista.ESPECIALISTA_MATERNO.Trim() : string.Empty);
            else
                return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }

    [ValueConversion(typeof(ATENCION_MEDICA), typeof(bool))]
    public class ConvertSignosVitales : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is string)
            if (value == null) return false;
            if (value is ATENCION_MEDICA)
            {
                return ((ATENCION_MEDICA)value).NOTA_SIGNOS_VITALES != null;
            }
            else if (value is EXCARCELACION)
            {
                var excarcelacion = ((EXCARCELACION)value);
                return excarcelacion.CERT_MEDICO_SALIDA.HasValue ? excarcelacion.ATENCION_MEDICA != null ? excarcelacion.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null : false : false;
            }
            else if (value is TRASLADO_DETALLE)
            {
                var traslado = ((TRASLADO_DETALLE)value);
                return traslado.ID_ATENCION_MEDICA.HasValue ? traslado.ATENCION_MEDICA != null ? traslado.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null : false : false;
            }
            else if (value is INGRESO)
            {
                var ingreso = ((INGRESO)value);
                //var rol = new cUsuarioRol().GetData().First(x => x.ID_USUARIO.Contains(StaticSourcesViewModel.UsuarioLogin.Username) && x.ID_ROL == (short)enumRolesAreasTecnicas.MEDICO || x.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA);
                return ingreso.ATENCION_MEDICA.Any(w => w.NOTA_SIGNOS_VITALES != null && w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO);
            }
            else if (value is ATENCION_CITA)
            {
                var atCita = ((ATENCION_CITA)value);
                if (atCita.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL)
                {
                    return new cHistoriaClinicaDental().GetData().Any(a => (a.ID_CENTRO == atCita.ID_CENTRO && a.ID_ANIO == atCita.ID_ANIO && a.ID_IMPUTADO == atCita.ID_IMPUTADO && a.ID_INGRESO == atCita.ID_INGRESO) ?
                        a.REGISTRO_FEC.HasValue ?
                            atCita.CITA_FECHA_HORA.HasValue ?
                                (a.REGISTRO_FEC.Value.Year == atCita.CITA_FECHA_HORA.Value.Year && a.REGISTRO_FEC.Value.Month == atCita.CITA_FECHA_HORA.Value.Month && a.REGISTRO_FEC.Value.Day == atCita.CITA_FECHA_HORA.Value.Day)
                            : false
                        : false
                    : false);
                }
                else if (atCita.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA)
                {
                    return new cHistoriaClinica().GetData().Any(a => (a.ID_CENTRO == atCita.ID_CENTRO && a.ID_ANIO == atCita.ID_ANIO && a.ID_IMPUTADO == atCita.ID_IMPUTADO && a.ID_INGRESO == atCita.ID_INGRESO) ?
                        a.ESTUDIO_FEC.HasValue ?
                            atCita.CITA_FECHA_HORA.HasValue ?
                                (a.ESTUDIO_FEC.Value.Year == atCita.CITA_FECHA_HORA.Value.Year && a.ESTUDIO_FEC.Value.Month == atCita.CITA_FECHA_HORA.Value.Month && a.ESTUDIO_FEC.Value.Day == atCita.CITA_FECHA_HORA.Value.Day)
                            : false
                        : false
                    : false);
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }
    [ValueConversion(typeof(ATENCION_MEDICA), typeof(bool))]
    public class ConvertConsultaMedica : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is string)
            if (value == null) return false;
            if (value is ATENCION_MEDICA)
            {
                return ((ATENCION_MEDICA)value).NOTA_SIGNOS_VITALES != null ? ((ATENCION_MEDICA)value).NOTA_MEDICA != null : false;
            }
            else if (value is EXCARCELACION)
            {
                var excarcelacion = ((EXCARCELACION)value);
                return excarcelacion.CERT_MEDICO_SALIDA.HasValue ?
                    excarcelacion.ATENCION_MEDICA != null ?
                        excarcelacion.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ?
                            excarcelacion.ATENCION_MEDICA.CERTIFICADO_MEDICO != null ?
                                excarcelacion.CERT_MEDICO_RETORNO.HasValue ?
                                    excarcelacion.ATENCION_MEDICA1 != null ?
                                        excarcelacion.ATENCION_MEDICA1.NOTA_SIGNOS_VITALES != null ?
                                            excarcelacion.ATENCION_MEDICA1.CERTIFICADO_MEDICO != null
                                        : false
                                    : false
                                : false
                            : false
                        : false
                    : false
                : false;
            }
            else if (value is TRASLADO_DETALLE)
            {
                var traslado = ((TRASLADO_DETALLE)value);
                return traslado.ID_ATENCION_MEDICA.HasValue ? traslado.ATENCION_MEDICA != null ? traslado.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null && traslado.ATENCION_MEDICA.NOTA_MEDICA != null : false : false;
            }
            else if (value is INGRESO)
            {
                var ingreso = ((INGRESO)value);
                //var rol = new cUsuarioRol().GetData().First(x => x.ID_USUARIO.Contains(StaticSourcesViewModel.UsuarioLogin.Username) && x.ID_ROL == (short)enumRolesAreasTecnicas.MEDICO || x.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA);
                return ingreso.ATENCION_MEDICA.Any(w => w.NOTA_SIGNOS_VITALES != null && w.NOTA_MEDICA != null &&
                    w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO);
            }
            else if (value is ATENCION_CITA)
            {
                var atCita = ((ATENCION_CITA)value);
                if (atCita.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL)
                {
                    return new cHistoriaClinicaDental().GetData().Any(a => (a.ID_CENTRO == atCita.ID_CENTRO && a.ID_ANIO == atCita.ID_ANIO && a.ID_IMPUTADO == atCita.ID_IMPUTADO && a.ID_INGRESO == atCita.ID_INGRESO) ?
                        a.REGISTRO_FEC.HasValue ?
                            atCita.CITA_FECHA_HORA.HasValue ?
                                (a.REGISTRO_FEC.Value.Year == atCita.CITA_FECHA_HORA.Value.Year && a.REGISTRO_FEC.Value.Month == atCita.CITA_FECHA_HORA.Value.Month && a.REGISTRO_FEC.Value.Day == atCita.CITA_FECHA_HORA.Value.Day) ?
                                    a.ESTATUS == "T"
                                : false
                            : false
                        : false
                    : false);
                }
                else if (atCita.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA)
                {
                    return new cHistoriaClinica().GetData().Any(a => (a.ID_CENTRO == atCita.ID_CENTRO && a.ID_ANIO == atCita.ID_ANIO && a.ID_IMPUTADO == atCita.ID_IMPUTADO && a.ID_INGRESO == atCita.ID_INGRESO) ?
                        a.ESTUDIO_FEC.HasValue ?
                            atCita.CITA_FECHA_HORA.HasValue ?
                                (a.ESTUDIO_FEC.Value.Year == atCita.CITA_FECHA_HORA.Value.Year && a.ESTUDIO_FEC.Value.Month == atCita.CITA_FECHA_HORA.Value.Month && a.ESTUDIO_FEC.Value.Day == atCita.CITA_FECHA_HORA.Value.Day) ?
                                    a.ESTATUS == "T"
                                : false
                            : false
                        : false
                    : false);
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }
    [ValueConversion(typeof(ABOGADO_INGRESO), typeof(string))]
    public class GetEstatusAbogadoIngreso : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is string)
            if (value == null)
                return string.Empty;
            if (value is ABOGADO_INGRESO)
            {
                var val = (ABOGADO_INGRESO)value;
                if (val.ID_ESTATUS_VISITA == 14 || val.ID_ESTATUS_VISITA == 15)
                {
                    return val.ESTATUS_VISITA.DESCR;
                }

                var doctojuez = Parametro.DOCUMENTO_ASIGNACION_JUEZ;
                var asignaInterno = Parametro.DOCUMENTO_ASIGNACION_INTERNO;
                var autorizado = Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                if (val.ABOGADO_ING_DOCTO.Any(wh => wh.ID_TIPO_VISITA == short.Parse(doctojuez[0]) &&
                    (wh.ID_TIPO_DOCUMENTO == short.Parse(doctojuez[1]) ||
                    wh.ID_TIPO_DOCUMENTO == short.Parse(asignaInterno[1]))))
                    return val.ESTATUS_VISITA.DESCR;
                else if (val.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR)
                {

                    //if (val.ABOGADO.ABOGADO2.ABOGADO_INGRESO.Any(w => w.ID_CENTRO == val.ID_CENTRO && w.ID_ANIO == val.ID_ANIO && w.ID_IMPUTADO == val.ID_IMPUTADO && w.ID_INGRESO == val.ID_INGRESO) ?
                    //val.ABOGADO.ABOGADO2.ABOGADO_INGRESO.First(w => w.ID_CENTRO == val.ID_CENTRO && w.ID_ANIO == val.ID_ANIO && w.ID_IMPUTADO == val.ID_IMPUTADO && w.ID_INGRESO == val.ID_INGRESO)
                    //.ABOGADO_ING_DOCTO.Any(wh => (wh.ID_TIPO_VISITA == short.Parse(doctojuez[0]) && (wh.ID_TIPO_DOCUMENTO == short.Parse(doctojuez[1]) ||
                    //    wh.ID_TIPO_DOCUMENTO == short.Parse(asignaInterno[1])))) : false)/* || (wh.ABOGADO_INGRESO.ID_ESTATUS_VISITA == autorizado)*/
                    //    return val.ABOGADO.ABOGADO2.ABOGADO_INGRESO.First(w => w.ID_CENTRO == val.ID_CENTRO && w.ID_ANIO == val.ID_ANIO && w.ID_IMPUTADO == val.ID_IMPUTADO && w.ID_INGRESO == val.ID_INGRESO).ESTATUS_VISITA.DESCR;
                    var titular = val.ABOGADO.ABOGADO_TITULAR1.Where(w => w.ESTATUS == "S").FirstOrDefault();
                    if (titular.ABOGADO.ABOGADO_INGRESO.Any(w => w.ID_CENTRO == val.ID_CENTRO && w.ID_ANIO == val.ID_ANIO && w.ID_IMPUTADO == val.ID_IMPUTADO && w.ID_INGRESO == val.ID_INGRESO) ?
                    titular.ABOGADO.ABOGADO_INGRESO.First(w => w.ID_CENTRO == val.ID_CENTRO && w.ID_ANIO == val.ID_ANIO && w.ID_IMPUTADO == val.ID_IMPUTADO && w.ID_INGRESO == val.ID_INGRESO)
                    .ABOGADO_ING_DOCTO.Any(wh => (wh.ID_TIPO_VISITA == short.Parse(doctojuez[0]) && (wh.ID_TIPO_DOCUMENTO == short.Parse(doctojuez[1]) ||
                        wh.ID_TIPO_DOCUMENTO == short.Parse(asignaInterno[1])))) : false)/* || (wh.ABOGADO_INGRESO.ID_ESTATUS_VISITA == autorizado)*/
                        return titular.ABOGADO.ABOGADO_INGRESO.First(w => w.ID_CENTRO == val.ID_CENTRO && w.ID_ANIO == val.ID_ANIO && w.ID_IMPUTADO == val.ID_IMPUTADO && w.ID_INGRESO == val.ID_INGRESO).ESTATUS_VISITA.DESCR;
                    else
                    {
                        if (val.ABOGADO_CAUSA_PENAL.Any(w => w.ABOGADO_CP_DOCTO.Any(wh => wh.ID_TIPO_VISITA == short.Parse(doctojuez[0]) &&
                        (wh.ID_TIPO_DOCUMENTO == short.Parse(doctojuez[1]) || wh.ID_TIPO_DOCUMENTO == short.Parse(asignaInterno[1])))))
                            return val.ESTATUS_VISITA.DESCR;
                        else
                            return val.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_SUSPENDIDO ? val.ESTATUS_VISITA.DESCR :
                               val.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_CANCELADO ? val.ESTATUS_VISITA.DESCR :
                                   "PASE";
                    }
                }
                else
                {
                    if (val.ABOGADO_CAUSA_PENAL.Any(w => w.ABOGADO_CP_DOCTO.Any(wh => wh.ID_TIPO_VISITA == short.Parse(doctojuez[0]) &&
                    (wh.ID_TIPO_DOCUMENTO == short.Parse(doctojuez[1]) || wh.ID_TIPO_DOCUMENTO == short.Parse(asignaInterno[1])))))
                        return val.ESTATUS_VISITA.DESCR;
                    else
                        return val.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_SUSPENDIDO ? val.ESTATUS_VISITA.DESCR :
                           val.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_CANCELADO ? val.ESTATUS_VISITA.DESCR :
                               "PASE";
                }
            }

            return value.ToString().Trim();

            //return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }


    [ValueConversion(typeof(ABOGADO_CAUSA_PENAL), typeof(string))]
    public class GetEstatusAbogadoCausaPenal : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is string)
            if (value == null)
                return string.Empty;
            if (value is ABOGADO_CAUSA_PENAL)
            {
                var acp = (ABOGADO_CAUSA_PENAL)value;
                if (acp.ID_ESTATUS_VISITA == 14 || acp.ID_ESTATUS_VISITA == 15)
                {
                    return acp.ESTATUS_VISITA.DESCR;
                }
                //var doctojuez = Parametro.DOCUMENTO_ASIGNACION_JUEZ;
                //var asignaInterno = Parametro.DOCUMENTO_ASIGNACION_INTERNO;
                if (acp.ABOGADO_CP_DOCTO != null)
                {
                    var r = acp.ABOGADO_CP_DOCTO.Count(w => w.ID_TIPO_DOCUMENTO == 5 || w.ID_TIPO_DOCUMENTO == 6);
                    if (r > 0)
                    {
                        return acp.ESTATUS_VISITA.DESCR;
                    }
                    else
                    {
                        return "PASE";
                    }
                }
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class GetNombrePrograma : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string respuesta = string.Empty;
            short campo;
            if (short.TryParse(value.ToString(), out campo))
            {
                var dato = new cProgramasEstudioEducativoComun().GetData(x => x.ID_PROGRAMA == campo).FirstOrDefault();
                if (dato != null)
                    respuesta = !string.IsNullOrEmpty(dato.DESCR) ? dato.DESCR.Trim() : string.Empty;
            }

            return respuesta;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }


    [ValueConversion(typeof(short), typeof(string))]
    public class GetNombreTipoPrograma : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string respuesta = string.Empty;
            short campo;
            if (short.TryParse(value.ToString(), out campo))
            {
                var dato = new cTipoPrograma().GetData(x => x.ID_TIPO_PROGRAMA == campo).FirstOrDefault();
                if (dato != null)
                    respuesta = !string.IsNullOrEmpty(dato.NOMBRE) ? dato.NOMBRE.Trim() : string.Empty;
            }

            return respuesta;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }



    [ValueConversion(typeof(INGRESO), typeof(string))]
    public class IsIngresoActivo : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is string)
            if (value == null)
                return string.Empty;
            if (value is INGRESO)
            {
                var val = (INGRESO)value;
                return Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(w => w.HasValue ? w.Value == val.ID_ESTATUS_ADMINISTRATIVO : false) ? "NO" : "SI";
            }

            return value.ToString().Trim();

            //return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }
    [ValueConversion(typeof(string), typeof(string))]
    public class ConvertTimeString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is string)
            if (value == null) return string.Empty;

            return value.ToString().Trim().Insert(2, ":");

            //return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class CatalogosEstatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return value == null ? "N" : value;
            if (value != null)
            {
                if (value.ToString() == "S")
                    return "ACTIVA";
            }
            return "INACTIVA";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }

    [ValueConversion(typeof(string), typeof(bool))]
    public class ConvertEstatusStringToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return value == null ? "N" : value;
            if (value == null) return false;
            if (value is string)
                return string.IsNullOrEmpty(value.ToString()) ? false : value.ToString() == "S";
            return false;

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "N";
            if (value is bool)
                return (bool)value ? "S" : "N";
            return "N";
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class TituloConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return value;

            string titulo = string.Empty;

            for (int i = 0; i < value.ToString().Length; i++)
            {
                if (i == 0)
                    titulo += value.ToString()[i].ToString().ToUpper();
                else titulo += value.ToString()[i].ToString().ToLower();
            }

            return titulo;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    [ValueConversion(typeof(ATENCION_CITA), typeof(string))]
    public class AtencionCitaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return value;

            var titulo = string.Empty;

            if (value is ATENCION_CITA)
            {
                var atCita = (ATENCION_CITA)value;
                titulo = atCita.ESTATUS == "S" ? "CONSULTA HECHA" : "PENDIENTE";
            }

            return titulo;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// conversion
    /// </summary>
    [ValueConversion(typeof(short), typeof(string))]
    public class DescripcionCentroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return value;

            var descripcion = string.Empty;

            if (value is short)
            {
                int Id_centro = int.Parse(value.ToString());
                var CentroObtenido = new cCentro().Obtener(Id_centro).FirstOrDefault();
                descripcion = CentroObtenido.DESCR;
            }

            return descripcion;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    [ValueConversion(typeof(decimal), typeof(string))]
    public class TipoLiquidoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            decimal campo;
            if (decimal.TryParse(value.ToString(), out campo))
            {
                string cam = campo.ToString();
                var TipoLiquido = new SSP.Controlador.Catalogo.Justicia.cLiquidoTipo().GetData(x => x.ID_LIQTIPO == cam).FirstOrDefault();
                return string.Format("{0}", TipoLiquido != null ? !string.IsNullOrEmpty(TipoLiquido.DESCR) ? TipoLiquido.DESCR.Trim() : string.Empty : string.Empty);
            };

            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class SiNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null || value == string.Empty)
                return "NO";
            else
                if ((string)value == "S")
                    return "SI";
                else
                    return "NO";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == string.Empty)
                return "N";
            else
                if (value == "SI")
                    return "S";
                else
                    return "N";
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class SiNoHojaEnfermeriaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (string.IsNullOrEmpty(value.ToString()))
                return string.Empty;
            else
                if ((string)value == "S")
                    return "SI";
                else
                    return "NO";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == string.Empty)
                return string.Empty;
            else
                if (value == "SI")
                    return "S";
                else
                    return "N";
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class FavorableNoFavorableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null || value == string.Empty)
                return string.Empty;

            if ((string)value == "F")
                return "FAVORABLE";

            if ((string)value == "D")
                return "DESFAVORABLE";

            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == string.Empty)
                return string.Empty;

            if ((string)value == "F")
                return "FAVORABLE";

            if ((string)value == "D")
                return "DESFAVORABLE";

            return string.Empty;
        }
    }

    public class MathConverter :
#if !SILVERLIGHT
 MarkupExtension,
    IMultiValueConverter,
#endif
 IValueConverter
    {
        Dictionary<string, IExpression> _storedExpressions = new Dictionary<string, IExpression>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new object[] { value }, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                decimal result = Parse(parameter.ToString()).Eval(values);
                if (targetType == typeof(decimal)) return result;
                if (targetType == typeof(string)) return result.ToString();
                if (targetType == typeof(int)) return (int)result;
                if (targetType == typeof(double)) return (double)result;
                if (targetType == typeof(long)) return (long)result;
                throw new ArgumentException(String.Format("Unsupported target type {0}", targetType.FullName));
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#if !SILVERLIGHT
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
#endif
        protected virtual void ProcessException(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        private IExpression Parse(string s)
        {
            IExpression result = null;
            if (!_storedExpressions.TryGetValue(s, out result))
            {
                result = new Parser().Parse(s);
                _storedExpressions[s] = result;
            }

            return result;
        }

        interface IExpression
        {
            decimal Eval(object[] args);
        }

        class Constant : IExpression
        {
            private decimal _value;

            public Constant(string text)
            {
                if (!decimal.TryParse(text, out _value))
                {
                    throw new ArgumentException(String.Format("'{0}' is not a valid number", text));
                }
            }

            public decimal Eval(object[] args)
            {
                return _value;
            }
        }

        class Variable : IExpression
        {
            private int _index;

            public Variable(string text)
            {
                if (!int.TryParse(text, out _index) || _index < 0)
                {
                    throw new ArgumentException(String.Format("'{0}' is not a valid parameter index", text));
                }
            }

            public Variable(int n)
            {
                _index = n;
            }

            public decimal Eval(object[] args)
            {
                if (_index >= args.Length)
                {
                    throw new ArgumentException(String.Format("MathConverter: parameter index {0} is out of range. {1} parameter(s) supplied", _index, args.Length));
                }

                return System.Convert.ToDecimal(args[_index]);
            }
        }

        class BinaryOperation : IExpression
        {
            private Func<decimal, decimal, decimal> _operation;
            private IExpression _left;
            private IExpression _right;

            public BinaryOperation(char operation, IExpression left, IExpression right)
            {
                _left = left;
                _right = right;
                switch (operation)
                {
                    case '+': _operation = (a, b) => (a + b); break;
                    case '-': _operation = (a, b) => (a - b); break;
                    case '*': _operation = (a, b) => (a * b); break;
                    case '/': _operation = (a, b) => (a / b); break;
                    default: throw new ArgumentException("Invalid operation " + operation);
                }
            }

            public decimal Eval(object[] args)
            {
                return _operation(_left.Eval(args), _right.Eval(args));
            }
        }

        class Negate : IExpression
        {
            private IExpression _param;

            public Negate(IExpression param)
            {
                _param = param;
            }

            public decimal Eval(object[] args)
            {
                return -_param.Eval(args);
            }
        }

        class Parser
        {
            private string text;
            private int pos;

            public IExpression Parse(string text)
            {
                try
                {
                    pos = 0;
                    this.text = text;
                    var result = ParseExpression();
                    RequireEndOfText();
                    return result;
                }
                catch (Exception ex)
                {
                    string msg =
                        String.Format("MathConverter: error parsing expression '{0}'. {1} at position {2}", text, ex.Message, pos);

                    throw new ArgumentException(msg, ex);
                }
            }

            private IExpression ParseExpression()
            {
                IExpression left = ParseTerm();

                while (true)
                {
                    if (pos >= text.Length) return left;

                    var c = text[pos];

                    if (c == '+' || c == '-')
                    {
                        ++pos;
                        IExpression right = ParseTerm();
                        left = new BinaryOperation(c, left, right);
                    }
                    else
                    {
                        return left;
                    }
                }
            }

            private IExpression ParseTerm()
            {
                IExpression left = ParseFactor();

                while (true)
                {
                    if (pos >= text.Length) return left;

                    var c = text[pos];

                    if (c == '*' || c == '/')
                    {
                        ++pos;
                        IExpression right = ParseFactor();
                        left = new BinaryOperation(c, left, right);
                    }
                    else
                    {
                        return left;
                    }
                }
            }

            private IExpression ParseFactor()
            {
                SkipWhiteSpace();
                if (pos >= text.Length) throw new ArgumentException("Unexpected end of text");

                var c = text[pos];

                if (c == '+')
                {
                    ++pos;
                    return ParseFactor();
                }

                if (c == '-')
                {
                    ++pos;
                    return new Negate(ParseFactor());
                }

                if (c == 'x' || c == 'a') return CreateVariable(0);
                if (c == 'y' || c == 'b') return CreateVariable(1);
                if (c == 'z' || c == 'c') return CreateVariable(2);
                if (c == 't' || c == 'd') return CreateVariable(3);

                if (c == '(')
                {
                    ++pos;
                    var expression = ParseExpression();
                    SkipWhiteSpace();
                    Require(')');
                    SkipWhiteSpace();
                    return expression;
                }

                if (c == '{')
                {
                    ++pos;
                    var end = text.IndexOf('}', pos);
                    if (end < 0) { --pos; throw new ArgumentException("Unmatched '{'"); }
                    if (end == pos) { throw new ArgumentException("Missing parameter index after '{'"); }
                    var result = new Variable(text.Substring(pos, end - pos).Trim());
                    pos = end + 1;
                    SkipWhiteSpace();
                    return result;
                }

                const string decimalRegEx = @"(\d+\.?\d*|\d*\.?\d+)";
                var match = Regex.Match(text.Substring(pos), decimalRegEx);
                if (match.Success)
                {
                    pos += match.Length;
                    SkipWhiteSpace();
                    return new Constant(match.Value);
                }
                else
                {
                    throw new ArgumentException(String.Format("Unexpeted character '{0}'", c));
                }
            }

            private IExpression CreateVariable(int n)
            {
                ++pos;
                SkipWhiteSpace();
                return new Variable(n);
            }

            private void SkipWhiteSpace()
            {
                while (pos < text.Length && Char.IsWhiteSpace((text[pos]))) ++pos;
            }

            private void Require(char c)
            {
                if (pos >= text.Length || text[pos] != c)
                {
                    throw new ArgumentException("Expected '" + c + "'");
                }

                ++pos;
            }

            private void RequireEndOfText()
            {
                if (pos != text.Length)
                {
                    throw new ArgumentException("Unexpected character '" + text[pos] + "'");
                }
            }
        }
    }

    [ValueConversion(typeof(CENTRO), typeof(string))]
    public class CentroDestinoVacio : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "FORANEO";
            else
                return ((CENTRO)value).DESCR;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

    [ValueConversion(typeof(short), typeof(string))]
    public class EstatusSolicitud : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                switch ((short)value)
                {
                    case (short)enumSolicitudCita.SOLICITADA:
                        return enumSolicitudCita.SOLICITADA;
                    case (short)enumSolicitudCita.AGENDADA:
                        return enumSolicitudCita.AGENDADA;
                    case (short)enumSolicitudCita.DESCARTADA:
                        return enumSolicitudCita.DESCARTADA;
                    case (short)enumSolicitudCita.CANCELADA:
                        return enumSolicitudCita.CANCELADA;
                }
            return string.Empty;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

    [ValueConversion(typeof(short), typeof(Visibility))]
    public class EstatusSolicitudVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((short)value == (short)enumSolicitudCita.AGENDADA)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(CAMA), typeof(string))]
    public class IngresoUbicacion : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var cama = ((CAMA)value);
                if (cama.CELDA == null) return string.Empty;
                if (cama.CELDA.SECTOR == null) return string.Empty;
                if (cama.CELDA.SECTOR.EDIFICIO == null) return string.Empty;
                return string.Format("{0}-{1}-{2}-{3}",
                                              cama.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                              cama.CELDA.SECTOR.DESCR.Trim(),
                                              cama.ID_CELDA.Trim(),
                                              cama.ID_CAMA);

            }
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(BitacoraAduana), typeof(string))]
    public class IngresoUbicacionAduana : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (!(value is BitacoraAduana)) return string.Empty;
            var aduana = ((BitacoraAduana)value);
            if (aduana.ADUANA.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_VISITA)
            {
                if (aduana.ADUANA.ADUANA_INGRESO != null ? !aduana.ADUANA.ADUANA_INGRESO.Any() : false)
                {
                    return "SOLO DEPOSITANTE";
                }
            }
            var cama = aduana.ADUANA.ADUANA_INGRESO != null ?
                aduana.ADUANA.ADUANA_INGRESO.Any() ?
                    aduana.ADUANA.ADUANA_INGRESO.FirstOrDefault().INGRESO != null ?
                        aduana.ADUANA.ADUANA_INGRESO.FirstOrDefault().INGRESO.CAMA != null ?
                            aduana.ADUANA.ADUANA_INGRESO.FirstOrDefault().INGRESO.CAMA
                        : null
                    : null
                : null
            : null;
            if (cama == null) return string.Empty;
            if (cama.CELDA == null) return string.Empty;
            if (cama.CELDA.SECTOR == null) return string.Empty;
            if (cama.CELDA.SECTOR.EDIFICIO == null) return string.Empty;
            return string.Format("{0}-{1}-{2}-{3}",
                                          cama.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                          cama.CELDA.SECTOR.DESCR.Trim(),
                                          cama.ID_CELDA.Trim(),
                                          cama.ID_CAMA);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(VISITA_AUTORIZADA), typeof(string))]
    public class NombreTelefono : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var Visita = ((VISITA_AUTORIZADA)value);
                return string.Format(" {0} ,  Teléfono: {1}", !string.IsNullOrEmpty(Visita.NOMBRE) ? Visita.NOMBRE.Trim() : string.Empty,
                    !string.IsNullOrEmpty(Visita.PATERNO) ? Visita.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(Visita.MATERNO) ? Visita.MATERNO.Trim() : string.Empty,
                    new Converters().MascaraTelefono(Visita.TELEFONO));
            }
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(CAUSA_PENAL), typeof(string))]
    public class CausaPenalFolio : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var _causa_penal = ((CAUSA_PENAL)value);
                if (_causa_penal.CP_FOLIO.HasValue && _causa_penal.CP_ANIO.HasValue)
                    return string.Format("{0}/{1}", _causa_penal.CP_ANIO, _causa_penal.CP_FOLIO.Value.ToString().PadLeft(5, '0'));
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class EstatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
                if (value.ToString().Contains("S"))
                    return "ACTIVO";
            return "INACTIVO";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    [ValueConversion(typeof(short), typeof(string))]
    public class EscolaridadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Respuesta = string.Empty;
            short Id = (short)value;
            var dato = new cEducacionGrado().GetData(x => x.ID_GRADO == Id).FirstOrDefault();
            if (dato != null)
                Respuesta = !string.IsNullOrEmpty(dato.DESCR) ? dato.DESCR.Trim() : string.Empty;

            return Respuesta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class OdontogramaDentalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Respuesta = string.Empty;
            short Id = (short)value;
            var dato = new cOdontograma().GetData(x => x.ID_POSICION == Id).FirstOrDefault();
            if (dato != null)
                Respuesta = string.Format("{0} {1}", !string.IsNullOrEmpty(dato.LADO) ? dato.LADO.Trim() : string.Empty, !string.IsNullOrEmpty(dato.LATERAL) ? dato.LATERAL.Trim() : string.Empty);

            return Respuesta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    [ValueConversion(typeof(short), typeof(string))]
    public class TipoImagenDentalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string Respuesta = string.Empty;
                short Id = short.Parse(value.ToString());
                var dato = new cTipoDocumentosHistoriaClinica().GetData(x => x.ID_DOCTO == Id).FirstOrDefault();
                if (dato != null)
                    Respuesta = dato != null ? !string.IsNullOrEmpty(dato.DESCR) ? dato.DESCR.Trim() : string.Empty : string.Empty;

                return Respuesta;
            }
            catch (Exception exc)
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class OdontogramaTipoDentalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Respuesta = string.Empty;
            short Id = (short)value;
            var dato = new cOdontogramaTipo().GetData(x => x.ID_TIPO_ODO == Id).FirstOrDefault();
            if (dato != null)
                Respuesta = !string.IsNullOrEmpty(dato.DESCR) ? dato.DESCR.Trim() : string.Empty;

            return Respuesta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    [ValueConversion(typeof(short), typeof(string))]
    public class OdontogramaSegConverter : IMultiValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string _respuesta = string.Empty;
                string _trat = string.Empty;
                short _posicionDiente = short.Parse(value[0].ToString());
                short _caraDiente = short.Parse(value[1].ToString());
                short _enfermedad = short.Parse(value[2].ToString());
                short _nomenclatura = short.Parse(value[3].ToString());

                var _detallePosicion = new cOdontograma().GetData(x => x.ID_POSICION == _caraDiente).FirstOrDefault();
                var _detalleDiente = new cOdontogramaTipo().GetData(x => x.ID_TIPO_ODO == _posicionDiente).FirstOrDefault();
                if (_enfermedad != -1)
                {
                    var _enf = new cEnfermedades().GetData(x => x.ID_ENFERMEDAD == _enfermedad).FirstOrDefault();
                    if (_enf != null)
                        _trat = !string.IsNullOrEmpty(_enf.NOMBRE) ? _enf.NOMBRE.Trim() : string.Empty;
                };

                if (_nomenclatura != -1)
                {
                    var _tratamDental = new cDentalNomenclatura().GetData(x => x.ID_NOMENCLATURA == _nomenclatura).FirstOrDefault();
                    if (_tratamDental != null)
                        _trat = !string.IsNullOrEmpty(_tratamDental.DESCR) ? _tratamDental.DESCR.Trim() : string.Empty;
                }

                _respuesta = string.Format("{0} EN {1} DEL {2} . POSICIÓN {3}",
                                _trat,
                                _detalleDiente != null ? !string.IsNullOrEmpty(_detalleDiente.DESCR) ? _detalleDiente.DESCR.Trim() : string.Empty : string.Empty,
                                _detallePosicion != null ? !string.IsNullOrEmpty(_detallePosicion.DESCR) ? _detallePosicion.DESCR.Trim() : string.Empty : string.Empty,
                    //_detallePosicion != null ? string.Format("{0} {1} ", !string.IsNullOrEmpty(_detallePosicion.LADO) ? _detallePosicion.LADO.Trim() : string.Empty, !string.IsNullOrEmpty(_detallePosicion.LATERAL) ? _detallePosicion.LATERAL.Trim() : string.Empty) : string.Empty,
                                    _detallePosicion != null ? _detallePosicion.ID_POSICION.ToString() : string.Empty);

                return _respuesta;
            }

            catch (Exception exc)
            {
                return string.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class OdontogramaDientesImagenesConverter : IMultiValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                short _enfermedad = short.Parse(value[0].ToString());
                short _tratamiento = short.Parse(value[1].ToString());
                System.Windows.Media.Imaging.BitmapImage _respuesta = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagen("imageNotFound.jpg"));
                if (_enfermedad != -1)
                {
                    var _enf = new cOdontogramaSimbologias().GetData(x => x.ID_ENFERMEDAD == _enfermedad).FirstOrDefault();
                    if (_enf != null)
                        _respuesta = new Imagenes().ConvertByteToBitmap(_enf.IMAGEN);
                };

                if (_tratamiento != -1)
                {
                    var _tratamDental = new cOdontogramaSimbologias().GetData(x => x.ID_NOMENCLATURA == _tratamiento).FirstOrDefault();
                    if (_tratamDental != null)
                        _respuesta = new Imagenes().ConvertByteToBitmap(_tratamDental.IMAGEN);
                };

                return _respuesta;
            }

            catch (Exception exc)
            {
                return string.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class EnfermedadDentalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Respuesta = string.Empty;

            if (value == null)
                return string.Empty;

            short Id = short.Parse(value.ToString());
            var dato = new cEnfermedades().GetData(x => x.ID_ENFERMEDAD == Id).FirstOrDefault();
            if (dato != null)
                Respuesta = !string.IsNullOrEmpty(dato.NOMBRE) ? dato.NOMBRE.Trim() : string.Empty;

            return Respuesta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class TratamientoEnfermedadDescripcionConverter : IMultiValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return string.Empty;
                var _respuesta = string.Empty;
                var _trat = string.Empty;
                var _posicionDiente = short.Parse(value[0].ToString());
                var _caraDiente = short.Parse(value[1].ToString());
                //var _enfermedad = short.Parse(value[2].ToString());
                var _enfermedad = value[2] != null ? short.Parse(value[2].ToString()) : (short)-1;
                var _tratamiento = value[3] != null ? short.Parse(value[3].ToString()) : (short)-1;

                var _detallePosicion = new cOdontograma().GetData(x => x.ID_POSICION == _caraDiente).FirstOrDefault();
                var _detalleDiente = new cOdontogramaTipo().GetData(x => x.ID_TIPO_ODO == _posicionDiente).FirstOrDefault();
                if (_enfermedad != -1)
                {
                    var _enf = new cEnfermedades().GetData(x => x.ID_ENFERMEDAD == _enfermedad).FirstOrDefault();
                    if (_enf != null)
                        _trat = !string.IsNullOrEmpty(_enf.NOMBRE) ? _enf.NOMBRE.Trim() : string.Empty;
                };

                if (_tratamiento != -1)
                {
                    var _tratamDental = new cTratamientoDental().GetData(x => x.ID_TRATA == _tratamiento).FirstOrDefault();
                    if (_tratamDental != null)
                        _trat = !string.IsNullOrEmpty(_tratamDental.DESCR) ? _tratamDental.DESCR.Trim() : string.Empty;
                }

                _respuesta = string.Format("{0} EN {1} DEL {2}. POSICIÓN {3}",
                                _trat,
                                _detalleDiente != null ? !string.IsNullOrEmpty(_detalleDiente.DESCR) ? _detalleDiente.DESCR.Trim() : string.Empty : string.Empty,
                                _detallePosicion != null ? !string.IsNullOrEmpty(_detallePosicion.DESCR) ? _detallePosicion.DESCR.Trim() : string.Empty : string.Empty,
                    //_detallePosicion != null ? string.Format("{0} {1} ", !string.IsNullOrEmpty(_detallePosicion.LADO) ? _detallePosicion.LADO.Trim() : string.Empty, !string.IsNullOrEmpty(_detallePosicion.LATERAL) ? _detallePosicion.LATERAL.Trim() : string.Empty) : string.Empty,
                                    _detallePosicion != null ? _detallePosicion.ID_POSICION.ToString() : string.Empty);

                return _respuesta;
            }

            catch (Exception exc)
            {
                return string.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class TratamientoEnfermedadDentalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var Respuesta = string.Empty;
            if (value == null) return Respuesta;
            if (value is ODONTOGRAMA_SEGUIMIENTO2)
            {
                var valor = (ODONTOGRAMA_SEGUIMIENTO2)value;
                Respuesta = valor.ENFERMEDAD != null ? "ENFERMEDAD" : valor.DENTAL_TRATAMIENTO != null ? "TRATAMIENTO" : string.Empty;
            }
            return Respuesta;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class NomenclaturaDentalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Respuesta = string.Empty;

            if (value == null)
                return string.Empty;

            short Id = short.Parse(value.ToString());
            var dato = new cDentalNomenclatura().GetData(x => x.ID_NOMENCLATURA == Id).FirstOrDefault();
            if (dato != null)
                Respuesta = !string.IsNullOrEmpty(dato.DESCR) ? dato.DESCR.Trim() : string.Empty;

            return Respuesta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class TratamientoDentalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Respuesta = string.Empty;

            if (value == null)
                return string.Empty;

            short Id = short.Parse(value.ToString());
            var dato = new cTratamientoDental().GetData(x => x.ID_TRATA == Id).FirstOrDefault();
            if (dato != null)
                Respuesta = !string.IsNullOrEmpty(dato.DESCR) ? dato.DESCR.Trim() : string.Empty;

            return Respuesta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class GetDiaSemana : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string[] semana = { "DOMINGO", "LUNES", "MARTES", "MIERCOLES", "JUEVES", "VIERNES", "SABADO" };
                int d = int.Parse(value.ToString());
                return semana[d];
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }


    [ValueConversion(typeof(int), typeof(string))]
    public class GetNombreMes : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string[] meses = { "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE" };
                int d = int.Parse(value.ToString());
                return meses[d];
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
        }
    }

    [ValueConversion(typeof(INGRESO), typeof(string))]
    public class EMIPendiente : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string tab = string.Empty;
            if (value != null)
            {
                var ingreso = (INGRESO)value;
                if (ingreso.EMI_INGRESO.Count > 0)
                {
                    var emi_ingreso = ingreso.EMI_INGRESO.FirstOrDefault();
                    tab = "FICHA DE IDENTIFICACIÓN";
                    if (emi_ingreso != null)
                    {
                        var emi = emi_ingreso.EMI;
                        if (emi.EMI_SITUACION_JURIDICA != null || emi.EMI_INGRESO_ANTERIOR.Count > 0)
                        {
                            tab = "SITUACIÓN JURIDICA";
                        }
                        if (emi.EMI_FACTORES_SOCIO_FAMILIARES != null || emi.EMI_GRUPO_FAMILIAR.Count > 0 || emi.EMI_ANTECEDENTE_FAM_CON_DEL.Count > 0 || emi.EMI_ANTECEDENTE_FAMILIAR_DROGA.Count > 0)
                        {
                            tab = "FACTORES SOCIO FAMILIARES";
                        }
                        if (emi.EMI_USO_DROGA.Count > 0 || emi.EMI_HPS != null || emi.EMI_TATUAJE != null || emi.EMI_ENFERMEDAD != null)
                        {
                            tab = "CONDUCTAS PARASOCIALES";
                        }
                        if (emi.EMI_CLAS_CRIMINOLOGICA != null || emi.EMI_FACTOR_CRIMINODIAGNOSTICO != null)
                        {
                            tab = "CLASIFICACIÓN CRIMINOLOGICA";
                        }
                    }
                }
            }
            return tab;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    //OBTENER EL EXPEDIENTE DEL INTERNO
    [ValueConversion(typeof(INGRESO), typeof(string))]
    public class ExpInterno : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is INGRESO)
                {
                    var i = (INGRESO)value;
                    return string.Format("{0}/{1}", i.ID_ANIO, i.ID_IMPUTADO);
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    //Obtener Atendio 
    [ValueConversion(typeof(ATENCION_RECIBIDA), typeof(string))]
    public class AtendioAtencionRecibida : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is ATENCION_RECIBIDA)
                {
                    var obj = (ATENCION_RECIBIDA)value;
                    if (obj.USUARIO != null)
                    {
                        if (obj.USUARIO.EMPLEADO != null)
                        {
                            if (obj.USUARIO.EMPLEADO.PERSONA != null)
                                return string.Format("{0} {1} {0}", !string.IsNullOrEmpty(obj.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? obj.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(obj.USUARIO.EMPLEADO.PERSONA.PATERNO) ? obj.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(obj.USUARIO.EMPLEADO.PERSONA.MATERNO) ? obj.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty);
                        }
                    }
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }



    [ValueConversion(typeof(short), typeof(string))]
    public class CertificadoMedicoNumero : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                short numero = 0;
                short.TryParse(value.ToString(), out numero);
                if (numero == 1)
                    return "SÍ";
            }
            return "NO";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class EstatusTraslado : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch (value.ToString())
                {
                    case "PR":
                        value = "PROGRAMADO";
                        break;
                    case "FI":
                        value = "FINALIZADO";
                        break;
                    case "EP":
                        value = "EN PROCESO";
                        break;
                }
                return value;
            }

            return "SIN ESTATUS";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class EstatusExcarcelacion : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch (value.ToString())
                {
                    case "AC":
                        value = "ACTIVA";
                        break;
                    case "PR":
                        value = "PROGRAMADA";
                        break;
                    case "AU":
                        value = "AUTORIZADA";
                        break;
                    case "EP":
                        value = "EN PROCESO";
                        break;
                }
                return value;
            }
            return "SIN ESTATUS";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(short), typeof(string))]
    public class TipoExcToStringConverter : IValueConverter
    {
        enum enumTipoExcarcelacion
        {
            JURIDICA = 1,
            MEDICA = 2
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                short tipo = 0;
                short.TryParse(value.ToString(), out tipo);
                switch (tipo)
                {

                    case (short)enumTipoExcarcelacion.JURIDICA:
                        value = "JURÍDICA";
                        break;
                    case (short)enumTipoExcarcelacion.MEDICA:
                        value = "MÉDICA";
                        break;
                }
                return value;
            }
            return "SIN TIPO";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

    /// <summary>
    /// Obtiene el ultimo estatus de la medida en libertad
    /// </summary>
    [ValueConversion(typeof(MEDIDA_LIBERTAD), typeof(string))]
    public class EstatusMedidaLibertad : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is MEDIDA_LIBERTAD)
                {
                    var medida = (MEDIDA_LIBERTAD)value;
                    if (medida.MEDIDA_LIBERTAD_ESTATUS != null)
                    {
                        var estatus = medida.MEDIDA_LIBERTAD_ESTATUS.OrderBy(w => w.ID_ESTATUS).FirstOrDefault();
                        if (estatus != null)
                        {
                            return estatus.MEDIDA_ESTATUS.DESCR;
                        }
                    }
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(string), typeof(Visibility))]
    public class TomarAsistenciaVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.ToString() == "S")
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

    [ValueConversion(typeof(int), typeof(string))]
    public class ConverterTelefon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return new Converters().MascaraTelefono(value.ToString());
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(PROCESO_LIBERTAD), typeof(string))]
    public class ConverterCausaPenalLiberado : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var pl = (PROCESO_LIBERTAD)value;
                if (pl != null)
                {
                    if (pl.CP_ANIO.HasValue && pl.CP_FOLIO.HasValue)
                    {
                        return string.Format("{0}/{1}", pl.CP_ANIO, pl.CP_FOLIO);
                    }
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}

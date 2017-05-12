using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlPenales
{
    partial class PadronAbogadosViewModel
    {
        public void SetValidaciones()
        {
            base.ClearRules();
            //base.AddRule(() => ImageFrontal, () => (ImageFrontal != null ? ImageFrontal.Count == 1 : false), "LA FOTO DE FRENTE ES REQUERIDA");
            //base.AddRule(() => ImagesToSave, () => (ImagesToSave != null ? ImagesToSave.Count == 4 : false), "LAS FOTOS DE LAS CREDENCIALES SON REQUERIDAS");
            base.AddRule(() => TextNombreAbogado, () => !string.IsNullOrEmpty(TextNombreAbogado), "NOMBRE ES REQUERIDO");
            //base.AddRule(() => TextMaternoAbogado, () => !string.IsNullOrEmpty(TextMaternoAbogado), "APELLIDO MATERNO ES REQUERIDO");
            base.AddRule(() => TextPaternoAbogado, () => !string.IsNullOrEmpty(TextPaternoAbogado), "APELLIDO PATERNO ES REQUERIDO");
            base.AddRule(() => SelectSexo, () => (!string.IsNullOrEmpty(SelectSexo) ? SelectSexo != "S" : false), "SEXO ES REQUERIDO");

            //if (SelectFechaNacimiento.HasValue)
            //{
                var fecha = Fechas.GetFechaDateServer;
                base.AddRule(() => SelectFechaNacimiento, () => ((fecha.Year - SelectFechaNacimiento.Year) > 0) && (SelectFechaNacimiento < fecha), "LA FECHA DE NACIMIENTO ES REQUERIDA!");
            //}
            base.AddRule(() => TextRfc, () => string.IsNullOrEmpty(TextRfc) ? false : TextRfc.Length == 13, "RFC ES REQUERIDO!");
            base.AddRule(() => TextCurp, () => string.IsNullOrEmpty(TextCurp) ? false : TextCurp.Length == 18, "CURP ES REQUERIDO!");
            base.AddRule(() => TextTelefonoFijo, () => !string.IsNullOrEmpty(TextTelefonoFijo) ? TextTelefonoFijo.Length >= 14 : false, "TELÉFONO FIJO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
            base.AddRule(() => TextTelefonoMovil, () => !string.IsNullOrEmpty(TextTelefonoMovil) ? TextTelefonoMovil.Length >= 14 : false, "TELÉFONO MÓVIL ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
            base.AddRule(() => TextCorreo, () => new Correo().ValidarCorreo(TextCorreo), "CORREO ES REQUERIDO");
            base.AddRule(() => TextIne, () => !string.IsNullOrEmpty(TextIne), "CREDENCIAL DEL INE ES REQUERIDA");
            //base.AddRule(() => TextCjf, () => !string.IsNullOrEmpty(TextCjf), "CREDENCIAL DEL CONSEJO DE JUSTICIA FEDERAL ES REQUERIDA");
            base.AddRule(() => TextCedulaCJF, () => !string.IsNullOrEmpty(TextCedulaCJF), CedulaCJF.ToUpper() + " ES REQUERIDA");
            base.AddRule(() => SelectEstatusVisita, () => SelectEstatusVisita.HasValue ? SelectEstatusVisita.Value >= 1 : false, "ESTATUS ES REQUERIDO!");
            base.AddRule(() => SelectPais, () => SelectPais.HasValue ? SelectPais >= 1 : false, "PAÍS ES REQUERIDO!");
            base.AddRule(() => SelectEntidad, () => SelectEntidad.HasValue ? SelectEntidad >= 1 : false, "ENTIDAD ES REQUERIDA!");
            base.AddRule(() => SelectMunicipio, () => SelectMunicipio.HasValue ? SelectMunicipio >= 1 : false, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => SelectColonia, () => SelectColonia.HasValue ? SelectColonia >= 1 : false, "COLONIA ES REQUERIDA!");
            base.AddRule(() => TextCalle, () => !string.IsNullOrEmpty(TextCalle), "CALLE ES REQUERIDA!");
            base.AddRule(() => TextNumExt, () => TextNumExt.HasValue ? TextNumExt.Value > 0 : false, "NUMERO EXTERIOR ES REQUERIDO!");
            base.AddRule(() => TextCodigoPostal, () => TextCodigoPostal.HasValue ? TextCodigoPostal.Value > 0 : false, "CÓDIGO POSTAL ES REQUERIDO!");
            base.AddRule(() => SelectDiscapacitado, () => DiscapacitadoEnabled && (SelectDiscapacitado == "S" || SelectDiscapacitado == "N"), "DISCAPACITADO ES REQUERIDO!");

            //if (SelectDiscapacitado == "S")
            //    base.AddRule(() => SelectTipoDiscapacidad, () => SelectTipoDiscapacidad.HasValue ? SelectTipoDiscapacidad.Value > 0 : false, "TIPO DISCAPACIDAD ES REQUERIDO!");
            //OnPropertyChanged("SelectTipoDiscapacidad");
            OnPropertyChanged("SelectDiscapacitado");
        }
    }
}

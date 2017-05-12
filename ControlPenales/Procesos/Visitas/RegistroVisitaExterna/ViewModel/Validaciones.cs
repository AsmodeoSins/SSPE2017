
namespace ControlPenales
{
    partial class RegistroVisitaExternaViewModel
    {
        void SetValidacionesAgregarNuevaVisitaExterna()
        {
            base.ClearRules();
            base.AddRule(() => TextPaternoNuevo, () => !string.IsNullOrEmpty(TextPaternoNuevo), "APELLIDO PATERNO ES REQUERIDO!");
            base.AddRule(() => TextMaternoNuevo, () => !string.IsNullOrEmpty(TextMaternoNuevo), "APELLIDO MATERNO ES REQUERIDO!");
            base.AddRule(() => TextNombreNuevo, () => !string.IsNullOrEmpty(TextNombreNuevo), "NOMBRE(S) ES REQUERIDO(S)!");

            OnPropertyChanged("TextPaternoNuevo");
            OnPropertyChanged("TextMaternoNuevo");
            OnPropertyChanged("TextNombreNuevo");

            base.AddRule(() => SelectSexoNuevo, () => !string.IsNullOrEmpty(SelectSexoNuevo), "SEXO ES REQUERIDO!");
            OnPropertyChanged("SelectSexoNuevo");
            //if (FechaNacimientoNuevo.HasValue)
            //    base.AddRule(() => FechaNacimientoNuevo, () => FechaNacimientoNuevo.HasValue ? FechaNacimientoNuevo.Value <= Fechas.GetFechaDateServer : false, "FECHA DE NACIMIENTO ES REQUERIDA");

            base.AddRule(() => FechaNacimientoNuevo, () => FechaNacimientoNuevo.HasValue, "FECHA DE NACIMIENTO ES REQUERIDA");
            OnPropertyChanged("FechaNacimientoNuevo");

            base.AddRule(() => TextTelefonoFijoNuevo, () => !string.IsNullOrEmpty(TextTelefonoFijoNuevo) ? TextTelefonoFijoNuevo.Length >= 14 : false, "TELÉFONO FIJO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
            base.AddRule(() => TextTelefonoMovilNuevo, () => !string.IsNullOrEmpty(TextTelefonoMovilNuevo) ? TextTelefonoMovilNuevo.Length >= 14 : false, "TELÉFONO MÓVIL ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
            OnPropertyChanged("TextTelefonoFijoNuevo");
            OnPropertyChanged("TextTelefonoMovilNuevo");

            base.AddRule(() => TextCorreoNuevo, () => new Correo().ValidarCorreo(TextCorreoNuevo), "CORREO ES REQUERIDO!");
            base.AddRule(() => SelectTipoVisitanteNuevo, () => SelectTipoVisitanteNuevo.HasValue ? SelectTipoVisitanteNuevo.Value > 0 : false, "TIPO VISITANTE ES REQUERIDO!");
            OnPropertyChanged("TextCorreoNuevo");
            OnPropertyChanged("SelectTipoVisitanteNuevo");

            base.AddRule(() => SelectPais, () => SelectPais.HasValue ? SelectPais.Value > 0 : false, "PAÍS ES REQUERIDO!");
            base.AddRule(() => SelectEntidad, () => SelectEntidad.HasValue ? SelectEntidad.Value > 0 : false, "ESTADO ES REQUERIDO!");
            base.AddRule(() => SelectMunicipio, () => SelectMunicipio.HasValue ? SelectMunicipio.Value > 0 : false, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => SelectColonia, () => SelectColonia.HasValue ? SelectColonia.Value > 0 : false, "COLONIA ES REQUERIDA!");
            OnPropertyChanged("SelectPais");
            OnPropertyChanged("SelectEntidad");
            OnPropertyChanged("SelectMunicipio");
            OnPropertyChanged("SelectColonia");


            base.AddRule(() => TextCalle, () => !string.IsNullOrEmpty(TextCalle), "CALLE ES REQUERIDA!");
            base.AddRule(() => TextNumExt, () => TextNumExt.HasValue ? TextNumExt.Value > 0 : false, "NUMERO EXTERIOR ES REQUERIDO!");
            base.AddRule(() => TextCodigoPostal, () => TextCodigoPostal.HasValue ? TextCodigoPostal.Value > 0 : false, "CÓDIGO POSTAL ES REQUERIDO!");
            OnPropertyChanged("TextCalle");
            OnPropertyChanged("TextNumExt");
            OnPropertyChanged("TextCodigoPostal");

            if (selectTipoVisitanteNuevo == 23)
                base.AddRule(() => SelectDiscapacitadoNuevo, () => DiscapacitadoNuevoEnabled && (SelectDiscapacitadoNuevo == "S" || SelectDiscapacitadoNuevo == "N"), "DISCAPACITADO ES REQUERIDO!");
            OnPropertyChanged("DiscapacidadNuevoEnabled");
            OnPropertyChanged("SelectDiscapacitadoNuevo");

            if (selectDiscapacitadoNuevo == "S")
                base.AddRule(() => SelectDiscapacidadNuevo, () => SelectDiscapacidadNuevo.HasValue ? SelectDiscapacidadNuevo.Value > 0 : false, "TIPO DISCAPACIDAD ES REQUERIDO!");
            OnPropertyChanged("SelectDiscapacidadNuevo");
            
            base.AddRule(() => FotoPersonaExterna, () => FotoPersonaExterna != null, "FOTO DE FRENTE ES REQUERIDA");
            base.AddRule(() => FotoFrenteCredencial, () => FotoFrenteCredencial != null, "FOTO DE CREDENCIAL FRONTAL ES REQUERIDA");
            base.AddRule(() => FotoDetrasCredencial, () => FotoDetrasCredencial != null, "FOTO DE CREDENCIAL TRASERA ES REQUERIDA");
            OnPropertyChanged("FotoPersonaExterna");
            OnPropertyChanged("FotoFrenteCredencial");
            OnPropertyChanged("FotoDetrasCredencial");
        }
    }
}

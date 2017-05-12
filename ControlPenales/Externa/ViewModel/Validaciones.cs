using System;

namespace ControlPenales
{
    partial class PrincipalVisitaExternaViewModel
    {
        void SetValidacionesAgregarNuevaVisitaExterna()
        {
            base.ClearRules();
            base.AddRule(() => TextPaternoNuevo, () => !string.IsNullOrEmpty(TextPaternoNuevo), "APELLIDO PATERNO ES REQUERIDO!");
            base.AddRule(() => TextMaternoNuevo, () => !string.IsNullOrEmpty(TextMaternoNuevo), "APELLIDO MATERNO ES REQUERIDO!");
            base.AddRule(() => TextNombreNuevo, () => !string.IsNullOrEmpty(TextNombreNuevo), "NOMBRE(S) ES REQUERIDO(S)!");

            base.AddRule(() => SelectSexoNuevo, () => SelectSexoNuevo != "S" || SelectSexoNuevo != "N", "SEXO ES REQUERIDO!");

            if (FechaNacimientoNuevo.HasValue)
                base.AddRule(() => FechaNacimientoNuevo, () => FechaNacimientoNuevo.HasValue ? FechaNacimientoNuevo.Value <= Fechas.GetFechaDateServer : false, "FECHA DE NACIMIENTO ES REQUERIDA");

            base.AddRule(() => TextTelefonoFijoNuevo, () => TextTelefonoFijoNuevo != null ? TextTelefonoFijoNuevo.Length >= 14 : false, "TELÉFONO FIJO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
            base.AddRule(() => TextTelefonoMovilNuevo, () => TextTelefonoMovilNuevo != null ? TextTelefonoMovilNuevo.Length >= 14 : false, "TELÉFONO MÓVIL ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");

            base.AddRule(() => TextCorreoNuevo, () => new Correo().ValidarCorreo(TextCorreoNuevo), "CORREO ES REQUERIDO!");
            base.AddRule(() => SelectTipoVisitanteNuevo, () => SelectTipoVisitanteNuevo.HasValue ? SelectTipoVisitanteNuevo.Value > 0 : false, "TIPO VISITANTE ES REQUERIDO!");

            base.AddRule(() => SelectPais, () => SelectPais.HasValue ? SelectPais.Value > 0 : false, "PAÍS ES REQUERIDO!");
            base.AddRule(() => SelectEntidad, () => SelectEntidad.HasValue ? SelectEntidad.Value > 0 : false, "ESTADO ES REQUERIDO!");
            base.AddRule(() => SelectMunicipio, () => SelectMunicipio.HasValue ? SelectMunicipio.Value > 0 : false, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => SelectColonia, () => SelectColonia.HasValue ? SelectColonia.Value > 0 : false, "COLONIA ES REQUERIDA!");

            base.AddRule(() => TextCalle, () => !string.IsNullOrEmpty(TextCalle), "CALLE ES REQUERIDA!");
            base.AddRule(() => TextNumExt, () => TextNumExt.HasValue ? TextNumExt.Value > 0 : false, "NUMERO EXTERIOR ES REQUERIDO!");
            base.AddRule(() => TextCodigoPostal, () => TextCodigoPostal.HasValue ? TextCodigoPostal.Value > 0 : false, "CÓDIGO POSTAL ES REQUERIDO!");

            if (selectTipoVisitanteNuevo == 23)
                base.AddRule(() => SelectDiscapacitadoNuevo, () => DiscapacitadoNuevoEnabled && (SelectDiscapacitadoNuevo == "S" || SelectDiscapacitadoNuevo == "N"), "DISCAPACITADO ES REQUERIDO!");
            OnPropertyChanged("DiscapacidadNuevoEnabled");
            OnPropertyChanged("SelectDiscapacitadoNuevo");

            if (selectDiscapacitadoNuevo == "S")
                base.AddRule(() => SelectDiscapacidadNuevo, () => SelectDiscapacidadNuevo.HasValue ? SelectDiscapacidadNuevo.Value > 0 : false, "TIPO DISCAPACIDAD ES REQUERIDO!");
            OnPropertyChanged("SelectDiscapacidadNuevo");

            /*
            base.AddRule(() => FotoPersonaExterna, () => FotoPersonaExterna != null, "FOTO DE PERSONA ES REQUERIDO!");
            base.AddRule(() => FotoFrenteCredencial, () => FotoFrenteCredencial != null, "FOTO FRONTAL DE LA CREDENCIAL ES REQUERIDO!");
            base.AddRule(() => FotoDetrasCredencial, () => FotoDetrasCredencial != null, "FOTO TRASERA DE LA CREDENCIAL ES REQUERIDO!");

            base.AddRule(() => HuellasCapturadas, () => HuellasCapturadas != null ? HuellasCapturadas.Count > 0 : false, "HUELLAS SON REQUERIDAS!");
             
            */

        }

        void SetValidacionesVisitaExterna()
        {
            base.ClearRules();
            base.AddRule(() => TextPaterno, () => !string.IsNullOrEmpty(TextPaterno), "APELLIDO PATERNO ES REQUERIDO!");
            base.AddRule(() => TextMaterno, () => !string.IsNullOrEmpty(TextMaterno), "APELLIDO MATERNO ES REQUERIDO!");
            base.AddRule(() => TextNombre, () => !string.IsNullOrEmpty(TextNombre), "NOMBRE ES REQUERIDO!");

            /*base.AddRule(() => TextHoraEntrada, () => !string.IsNullOrEmpty(TextHoraEntrada), "HORA DE ENTRADA ES REQUERIDA!");
            base.AddRule(() => TextHoraSalida, () => !string.IsNullOrEmpty(TextHoraSalida), "HORA DE SALIDA ES REQUERIDA!");*/

            base.AddRule(() => TextAsunto, () => !string.IsNullOrEmpty(TextAsunto), "ASUNTO ES REQUERIDO!");
            OnPropertyChanged("TextAsunto");
            base.AddRule(() => TextInstitucion, () => !string.IsNullOrEmpty(TextInstitucion), "INSTITUCIÓN QUE VISITA ES REQUERIDA!");
            OnPropertyChanged("TextInstitucion");
            base.AddRule(() => SelectArea, () => SelectArea.HasValue ? SelectArea.Value > 0 : false, "ÁREA ES REQUERIDA!");
            OnPropertyChanged("SelectArea");
            if (FechaRegistro.HasValue)
            {
                base.AddRule(() => FechaRegistro, () => FechaRegistro.HasValue ? FechaRegistro.Value <= Fechas.GetFechaDateServer : false, "FECHA DE REGISTRO ES REQUERIDA");
                OnPropertyChanged("FechaRegistro");
            }

            if (selectDiscapacitado == "S")
                base.AddRule(() => SelectDiscapacidad, () => SelectDiscapacidad.HasValue ? SelectDiscapacidad.Value > 0 : false, "TIPO DISCAPACIDAD ES REQUERIDO!");
            OnPropertyChanged("SelectDiscapacidadNuevo");

            base.AddRule(() => SelectPersona, () => SelectPersona != null, "DEBES SELECCIONAR O CREAR UNA NUEVA PERSONA.");
        }

    }
}

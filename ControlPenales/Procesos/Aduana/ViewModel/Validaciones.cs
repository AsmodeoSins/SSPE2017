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
    partial class RecepcionAduanaViewModel
    {
        //public void SetValidacionesGenerales()
        //{
        //    try
        //    {
        //        base.ClearRules();
        //        base.AddRule(() => TextNombre, () => !string.IsNullOrEmpty(TextNombre), string.Empty);
        //        OnPropertyChanged("TextNombre");
        //        base.AddRule(() => TextMaterno, () => !string.IsNullOrEmpty(TextMaterno), string.Empty);
        //        OnPropertyChanged("TextMaterno");
        //        base.AddRule(() => TextPaterno, () => !string.IsNullOrEmpty(TextPaterno), string.Empty);
        //        OnPropertyChanged("TextPaterno");
        //        base.AddRule(() => SelectSexo, () => (!string.IsNullOrEmpty(SelectSexo) ? SelectSexo != "S" : false), string.Empty);
        //        OnPropertyChanged("SelectSexo");
        //        var fecha = Fechas.GetFechaDateServer;
        //        base.AddRule(() => FechaNacimiento, () => ((fecha.Year - FechaNacimiento.Year) > 0) && (FechaNacimiento < fecha), string.Empty);
        //        OnPropertyChanged("FechaNacimiento");
        //        base.AddRule(() => TextRfc, () => string.IsNullOrEmpty(TextRfc) ? false : TextRfc.Length == 13, "RFC ES REQUERIDO!");
        //        OnPropertyChanged("TextRfc");
        //        base.AddRule(() => TextCurp, () => string.IsNullOrEmpty(TextCurp) ? false : TextCurp.Length == 18, "CURP ES REQUERIDO!");
        //        OnPropertyChanged("TextCurp");
        //        base.AddRule(() => TextNip, () => TextNip.HasValue ? TextNip.Value > 0 : false, "NIP ES REQUERIDO!");
        //        OnPropertyChanged("TextNip");
        //        base.AddRule(() => TextTelefono, () => !string.IsNullOrEmpty(TextTelefono), string.Empty);
        //        OnPropertyChanged("TextTelefono");
        //        base.AddRule(() => TextCorreo, () => new Correo().ValidarCorreo(TextCorreo), "CORREO ES REQUERIDO");
        //        OnPropertyChanged("TextCorreo");
        //        base.AddRule(() => TextCedulaCJF, () => !string.IsNullOrEmpty(TextCedulaCJF), string.Empty);
        //        OnPropertyChanged("TextCedulaCJF");
        //        base.AddRule(() => SelectPais, () => SelectPais.HasValue ? SelectPais >= 1 : false, string.Empty);
        //        OnPropertyChanged("SelectPais");
        //        base.AddRule(() => SelectEntidad, () => SelectEntidad.HasValue ? SelectEntidad >= 1 : false, string.Empty);
        //        OnPropertyChanged("SelectEntidad");
        //        base.AddRule(() => SelectMunicipio, () => SelectMunicipio.HasValue ? SelectMunicipio >= 1 : false, string.Empty);
        //        OnPropertyChanged("SelectMunicipio");
        //        base.AddRule(() => SelectColonia, () => SelectColonia.HasValue ? SelectColonia >= 1 : false, string.Empty);
        //        OnPropertyChanged("SelectColonia");
        //        base.AddRule(() => TextCalle, () => !string.IsNullOrEmpty(TextCalle), string.Empty);
        //        OnPropertyChanged("TextCalle");
        //        base.AddRule(() => TextNumExt, () => TextNumExt.HasValue ? TextNumExt.Value > 0 : false, string.Empty);
        //        OnPropertyChanged("TextNumExt");
        //        base.AddRule(() => TextCodigoPostal, () => TextCodigoPostal.HasValue ? TextCodigoPostal.Value > 0 : false, string.Empty);
        //        OnPropertyChanged("TextCodigoPostal");
        //        base.AddRule(() => SelectDiscapacitado, () => DiscapacitadoEnabled && (SelectDiscapacitado == "S" || SelectDiscapacitado == "N"), string.Empty);
        //        if (SelectDiscapacitado == "S")
        //            base.AddRule(() => SelectTipoDiscapacidad, () => SelectTipoDiscapacidad.HasValue ? SelectTipoDiscapacidad.Value > 0 : false, string.Empty);
        //        OnPropertyChanged("SelectTipoDiscapacidad");
        //        OnPropertyChanged("SelectDiscapacitado");
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
        //    }
        //}

        public void SetValidacionesAbogados()
        {
            //SetValidacionesGenerales();
        }

        public void SetValidacionesActuarios()
        {
            //SetValidacionesGenerales();
        }

        public void SetValidacionesColaboradores()
        {
            //SetValidacionesGenerales();
        }

        public void SetValidacionesExternos()
        {
            //SetValidacionesGenerales();
            base.ClearRules();
            base.AddRule(() => TextAsuntoExterno, () => !string.IsNullOrEmpty(TextAsuntoExterno), "ASUNTO ES REQUERIDO");
            OnPropertyChanged("TextAsuntoExterno");
            base.AddRule(() => SelectAreaExterno, () => SelectAreaExterno >= 0, "AREA A LA QUE SE DIRIGE ES REQUERIDA");
            OnPropertyChanged("SelectAreaExterno");
            base.AddRule(() => SelectPuestoExterno, () => SelectPuestoExterno != -1, "PUESTO ES REQUERIDO");
            OnPropertyChanged("SelectPuestoExterno");
        }

        public void SetValidacionesVisitaFamiliar()
        {
            base.ClearRules();
            base.AddRule(() => SelectAreaDestino, () => SelectAreaDestino != null ? SelectAreaDestino.ID_AREA > 0 : false, "AREA ES REQUERIDA");
            OnPropertyChanged("SelectAreaDestino");
        }

        private bool ValidarDiaVisita(SSP.Servidor.PERSONA p)
        {
            try
            {
                var i = 0;
                if (p.ABOGADO != null)
                {
                    i++;//1
                }
                if (p.EMPLEADO != null)
                {
                    i++;//2
                }
                //Modificacion de modelo, PENDIENTE
                //if (p.PERSONA_EXTERNO != null ? p.PERSONA_EXTERNO.Count > 0 : false)
                //{
                //    i++;//3
                //}
                if (p.PERSONA_EXTERNO != null)
                {
                    i++;//3
                }
                ///////////////////////////////////////
                if (p.VISITANTE != null)
                {
                    i++;//4
                    VisitaFamiliarEnabled = true;
                }
                if (i == 0)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no cuenta con un registro correcto.");
                    return false;
                }

                if (i == 4)//familiar
                {
                    var mv = Parametro.MODO_VISITA;
                    if (mv == (short)enumModoVisita.APELLIDO)
                    {
                        //var visita = new cVisitaApellido().ObtenerTodos
                    }
                    else
                        if (mv == (short)enumModoVisita.EDIFICIO)
                        {

                        }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar dias de visita.", ex);
            }
            return false;
        }
        private void setValidacionesEmpleado()
        {
            base.ClearRules();
            base.AddRule(() => SelectPuestoExterno, () => SelectPuestoExterno != -1, "PUESTO ES REQUERIDO");
            RaisePropertyChanged("SelectPuestoExterno");
        }
    }
}

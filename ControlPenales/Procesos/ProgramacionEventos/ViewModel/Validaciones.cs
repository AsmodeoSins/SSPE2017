using ControlPenales;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ProgramacionEventosViewModel : ValidationViewModelBase
    {
        #region Evento
        private void ValidacionEvento() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => ENombre, () => !string.IsNullOrEmpty(ENombre), "NOMBRE ES REQUERIDO!");
                base.AddRule(() => EEventoTipo, () => EEventoTipo != -1, "TIPO DE EVENTO ES REQUERIDO!");
                if (ECentro != -1)
                    base.AddRule(() => ECentro, () => ECentro != -1, "LUGAR ES REQUERIDO!");
                else
                    base.AddRule(() => ELugar, () => !string.IsNullOrEmpty(ELugar), "LUGAR EXTERNO ES REQUERIDO!");
                base.AddRule(() => EDireccion, () => !string.IsNullOrEmpty(EDireccion), "DIRECCIÓN ES REQUERIDA!");
                base.AddRule(() => EEstado, () => EEstado != -1, "ESTADO ES REQUERIDO!");
                base.AddRule(() => EMunicipio, () => EMunicipio != -1, "MUNICIPIO ES REQUERIDO!");
                base.AddRule(() => ETelefono, () => !string.IsNullOrEmpty(ETelefono), "TELÉFONO ES REQUERIDO!");
                base.AddRule(() => EDuracionHrs, () => EDuracionHrs != null, "HORAS DE DURACIÓN ES REQUERIDA!");
                if (EDuracionHrs == null || EDuracionHrs == 0)
                    base.AddRule(() => EDuracionMin, () => EDuracionMin != null, "HORAS DE DURACIÓN ES REQUERIDA!");
                base.AddRule(() => EFecha, () => EFecha != null, "FECHA ES REQUERIDA!");
                base.AddRule(() => EHoraInvitados, () => EHoraInvitados != null, "HORA INVITACION DE LOS ASISTENTES ES REQUERIDA!");
                base.AddRule(() => EHoraPresidium, () => EHoraPresidium != null, "HORA DE ARRIBO DEL PRESIDIUM ES REQUERIDA!");
                base.AddRule(() => EDependencia, () => !string.IsNullOrEmpty(EDependencia), "DEPENDENCIA\\ENTIDAD PROMOVENTE ES REQUERIDA!");
                base.AddRule(() => EPerfilInvitados, () => !string.IsNullOrEmpty(EPerfilInvitados), "PERFIL DE LOS ASISTENTES ES REQUERIDO!");
                base.AddRule(() => EObjetivo, () => !string.IsNullOrEmpty(EObjetivo), "DESCRIPCION ES REQUERIDA!");
                base.AddRule(() => EObjetivoGral, () => !string.IsNullOrEmpty(EObjetivoGral), "OBJETIVO GENERAL ES REQUERIDO!");
                base.AddRule(() => EMaestroCeremonias, () => !string.IsNullOrEmpty(EMaestroCeremonias), "MAESTRO DE CEREMONIAS ES REQUERIDO!");
                base.AddRule(() => EImpactoEvento, () => EImpactoEvento != -1, "IMPACTO DEL EVENTO ES REQUERIDO!");
                base.AddRule(() => EVestimentaSugerida, () => EVestimentaSugerida != -1, "VESTIMENTA SUGERIDA ES REQUERIDA!");
                base.AddRule(() => EEstatus, () => EEstatus != -1, "ESTATUS ES REQUERIDO!");
                base.AddRule(() => SelectResponsable, () => SelectResponsable.HasValue ? SelectResponsable.Value != -1 : false, "RESPONSABLE ES REQUERIDO!");

                //base.AddRule(() => LstEventoPrograma, () => LstEventoPrograma != null ? LstEventoPrograma.Count > 0 : false, "PROGRAMA ES REQUERIDO!");
                //OnPropertyChanged("LstEventoPrograma");

                //base.AddRule(() => LstEventoPresidium, () => LstEventoPresidium != null ? LstEventoPresidium.Count > 0 : false, "PRESIDIUM ES REQUERIDO!");
                //OnPropertyChanged("LstEventoPresidium");

                //base.AddRule(() => LstEventoInfTecnica, () => LstEventoInfTecnica != null ? LstEventoInfTecnica.Count > 0 : false, "INFORMACIÓN TÉCNICA ES REQUERIDA!");
                //OnPropertyChanged("LstEventoInfTecnica");

                //base.AddRule(() => LstIngresosSeleccionados, () => LstIngresosSeleccionados != null ? LstIngresosSeleccionados.Count > 0 : false, "INTERNOS PARTICIPANTES SON REUQERIDOS!");
                //OnPropertyChanged("LstIngresosSeleccionados");

                OnPropertyChanged("ENombre");
                OnPropertyChanged("EEventoTipo");
                OnPropertyChanged("ECentro");
                OnPropertyChanged("ELugar");
                OnPropertyChanged("EDireccion");
                OnPropertyChanged("EEstado");
                OnPropertyChanged("EMunicipio");
                OnPropertyChanged("ETelefono");
                OnPropertyChanged("EDuracionHrs");
                OnPropertyChanged("EDuracionMin");
                OnPropertyChanged("EFecha");
                OnPropertyChanged("EHoraInvitados");
                OnPropertyChanged("EHoraPresidium");
                OnPropertyChanged("EDependencia");
                OnPropertyChanged("EPerfilInvitados");
                OnPropertyChanged("EObjetivo");
                OnPropertyChanged("EObjetivoGral");
                OnPropertyChanged("EMaestroCeremonias");
                OnPropertyChanged("EImpactoEvento");
                OnPropertyChanged("EVestimentaSugerida");
                OnPropertyChanged("EEstatus");
                OnPropertyChanged("SelectResponsable");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en evento", ex);
            }
        }
        #endregion

        #region Programa
        private void ValidacionPrograma() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => EPDescripcion, () => !string.IsNullOrEmpty(EPDescripcion), "Descripcion es requerida!");
                base.AddRule(() => EPDuracion, () => !string.IsNullOrEmpty(EPDuracion), "Duracion es requerida!");
                OnPropertyChanged("EPDescripcion");
                OnPropertyChanged("EPDuracion");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en programa", ex);
            }

        }
        private bool ValidarExistenProgramas() 
        {
            if (LstEventoPrograma == null)
                return false;
            else
                if (LstEventoPrograma.Count == 0)
                    return false;
            return true;
        }
        #endregion

        #region Presidium
        private void ValidacionPresidium()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => EPNombre, () => !string.IsNullOrEmpty(EPNombre), "Nombre es requerido!");
                base.AddRule(() => EPPuesto, () => !string.IsNullOrEmpty(EPPuesto), "Puesto es requerido!");
                OnPropertyChanged("EPNombre");
                OnPropertyChanged("EPPuesto");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en presidium", ex);
            }
        }
        private bool ValidarExistenPresidium()
        {
            if (LstEventoPresidium == null)
                return false;
            else
                if (LstEventoPresidium.Count == 0)
                    return false;
            return true;
        }
        #endregion

        #region Informacion Tecnica
        private void ValidacionInfTecnica()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => ETDescripcion, () => !string.IsNullOrEmpty(ETDescripcion), "Descripción es requerida!");
                OnPropertyChanged("ETDescripcion");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en informacion tecnica", ex);
            }
        }
        private bool ValidarExistenInformacionTecnica()
        {
            if (LstEventoInfTecnica == null)
                return false;
            else
                if (LstEventoInfTecnica.Count == 0)
                    return false;
            return true;
        }
        #endregion
    }
}

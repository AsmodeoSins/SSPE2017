using System;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        void setValidacionesFichaIdentificacion()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => FechaCapturaValid, () => FechaCapturaValid, "FECHA DE CAPTURA ES REQUERIDO PARA GRABAR");
                base.AddRule(() => TiempoColonia, () => !string.IsNullOrEmpty(TiempoColonia), "TIEMPO EN COLONIA ES REQUERIDO PARA GRABAR");
                base.AddRule(() => UltimoGradoEducativoConcluido, () => UltimoGradoEducativoConcluido.HasValue ? UltimoGradoEducativoConcluido != -1 : false, "ULTIMO GRADO EDUCATIVO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => ViviaAntesDetencion, () => !string.IsNullOrEmpty(ViviaAntesDetencion), "LUGAR ANTES DE DETENCION ES REQUERIDO PARA GRABAR");
                base.AddRule(() => Parentesco, () => Parentesco.HasValue ? Parentesco != -1 : false, "PARENTESCO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => ExFuncionarioSeguridadPublica, () => ExFuncionarioSeguridadPublica.HasValue ? ExFuncionarioSeguridadPublica != -1 : false, "ES-FUNCIONARIO DE SEGURIDAD PUBLICA ES REQUERIDO PARA GRABAR");
                base.AddRule(() => CertificadoEducacion, () => CertificadoEducacion.HasValue ? CertificadoEducacion != -1 : false, "CERTIFICADO DE EDUCACION ES REQUERIDO PARA GRABAR");
                base.AddRule(() => OficiosHabilidades, () => !string.IsNullOrEmpty(OficiosHabilidades), "OFICIOS Y HABILIDADES ES REQUERIDO PARA GRABAR");
                base.AddRule(() => UltimoAnio, () => UltimoAnio != null, "ULTIMO AÑO ES REQUERIDO PARA GRABAR");/**/
                OnPropertyChanged("FechaCapturaValid");
                OnPropertyChanged("TiempoColonia");
                OnPropertyChanged("UltimoGradoEducativoConcluido");
                OnPropertyChanged("ViviaAntesDetencion");
                OnPropertyChanged("Parentesco");
                OnPropertyChanged("ExFuncionarioSeguridadPublica");
                OnPropertyChanged("CertificadoEducacion");
                OnPropertyChanged("OficiosHabilidades");
                OnPropertyChanged("UltimoAnio");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en ficha de identificación", ex);
            }
        }
        
        void setValidacionesEmpleos()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => SelectedOcupacion, () => SelectedOcupacion != null ? SelectedOcupacion.ID_OCUPACION != -1 : false, "OCUPACION ES REQUERIDO PARA GRABAR");
                base.AddRule(() => Duracion, () => !string.IsNullOrEmpty(Duracion), "DURACION ES REQUERIDO PARA GRABAR");
                base.AddRule(() => Empresa, () => !string.IsNullOrEmpty(Empresa), "EMPRESA ES REQUERIDO PARA GRABAR");
                base.AddRule(() => MotivoDesempleo, () => !string.IsNullOrEmpty(MotivoDesempleo), "MOTIVO DESEMPLEO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("SelectedOcupacion");
                OnPropertyChanged("Duracion");
                OnPropertyChanged("Empresa");
                OnPropertyChanged("MotivoDesempleo");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en empleos", ex);
            }
        }
    }
}

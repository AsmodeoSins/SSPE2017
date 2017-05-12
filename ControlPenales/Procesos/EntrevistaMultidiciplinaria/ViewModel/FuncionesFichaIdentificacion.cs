using System;
using System.Collections.ObjectModel;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel : ValidationViewModelBase
    {
        private void PopulateFichaIdentificacion()
        {
            try
            {
                if (emiActual.EMI_FICHA_IDENTIFICACION != null)
                {
                    //CONTROL TABS
                    var ficha = emiActual.EMI_FICHA_IDENTIFICACION;
                    FechaCaptura = ficha.FEC_CAPTURA;
                    TiempoColonia = ficha.TIEMPO_RESID_COL;
                    UltimoGradoEducativoConcluido = ficha.ID_GRADO_EDUCATIVO_CONCLUIDO;
                    ViviaAntesDetencion = ficha.PERSONA_CONVIVENCIA_ANTERIOR;
                    ExFuncionarioSeguridadPublica = ficha.ID_EXFUNCIONARIO_SEGPUB;
                    Parentesco = ficha.ID_PARENTESCO;
                    CertificadoEducacion = ficha.ID_CERTIFICADO_EDUCACION;
                    CeresoProcedencia = ficha.ID_CERESO_PROCEDENCIA != null ? ficha.ID_CERESO_PROCEDENCIA : -1;
                    ActaNacimiento = ficha.ACTA_NACIMIENTO == "S" ? true : false;
                    Pasaporte = ficha.PASAPORTE == "S" ? true : false;
                    LicenciaManejo = ficha.LICENCIA_MANEJO == "S" ? true : false;
                    CredencialElector = ficha.CREDENCIAL_ELECTOR == "S" ? true : false;
                    CartillaMilitar = ficha.CARTILLA_MILITAR == "S" ? true : false;
                    CertificadoEducacion = ficha.ID_CERTIFICADO_EDUCACION;
                    OficiosHabilidades = ficha.OFICIOS_HABILIDADES;
                    UltimoAnio = ficha.CAMBIOS_DOMICILIO_ULTIMO_ANO;
                    Motivo = ficha.MOTIVOS_CAMBIOS_DOMICILIO;
                    PopulateUltimosEmpleos();
                    return;
                }

                if (AnteriorEMI != null)//CARGAMOS INFORMACION HISTORICA
                {
                    if (AnteriorEMI.EMI_FICHA_IDENTIFICACION != null)
                    {
                        ControlTab = 1;
                        var ficha = AnteriorEMI.EMI_FICHA_IDENTIFICACION;
                        FechaCaptura = ficha.FEC_CAPTURA;
                        TiempoColonia = ficha.TIEMPO_RESID_COL;
                        UltimoGradoEducativoConcluido = ficha.ID_GRADO_EDUCATIVO_CONCLUIDO;
                        ViviaAntesDetencion = ficha.PERSONA_CONVIVENCIA_ANTERIOR;
                        ExFuncionarioSeguridadPublica = ficha.ID_EXFUNCIONARIO_SEGPUB;
                        Parentesco = ficha.ID_PARENTESCO;
                        CertificadoEducacion = ficha.ID_CERTIFICADO_EDUCACION;
                        CeresoProcedencia = ficha.ID_CERESO_PROCEDENCIA != null ? ficha.ID_CERESO_PROCEDENCIA : -1;
                        ActaNacimiento = ficha.ACTA_NACIMIENTO == "S" ? true : false;
                        Pasaporte = ficha.PASAPORTE == "S" ? true : false;
                        LicenciaManejo = ficha.LICENCIA_MANEJO == "S" ? true : false;
                        CredencialElector = ficha.CREDENCIAL_ELECTOR == "S" ? true : false;
                        CartillaMilitar = ficha.CARTILLA_MILITAR == "S" ? true : false;
                        CertificadoEducacion = ficha.ID_CERTIFICADO_EDUCACION;
                        OficiosHabilidades = ficha.OFICIOS_HABILIDADES;
                        UltimoAnio = ficha.CAMBIOS_DOMICILIO_ULTIMO_ANO;
                        Motivo = ficha.MOTIVOS_CAMBIOS_DOMICILIO;
                        PopulateUltimosEmpleos();
                        return;
                    }
                }

                //CARGAMOS LOS DATOS POR DEFAULT
                UltimoGradoEducativoConcluido = ExFuncionarioSeguridadPublica = Parentesco = CertificadoEducacion = -1;
                CeresoProcedencia = -1;
                UltimoAnio = 0;
                LstUltimosEmpleos = new ObservableCollection<SSP.Servidor.EMI_ULTIMOS_EMPLEOS>();
                IsEmpleosEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ficha identificación", ex);
            }
        }

        private void PopulateUltimosEmpleos() 
        {
            try
            {
                //EMI ACTUAL
                if (emiActual.EMI_ULTIMOS_EMPLEOS != null)
                {
                    if (emiActual.EMI_ULTIMOS_EMPLEOS.Count > 0)
                    {
                        LstUltimosEmpleos = new ObservableCollection<SSP.Servidor.EMI_ULTIMOS_EMPLEOS>(emiActual.EMI_ULTIMOS_EMPLEOS);
                        IsEmpleosEmpty = LstUltimosEmpleos == null ? true : LstUltimosEmpleos.Count <= 0 ? true : false;
                        return;
                    }
                }
                //EMI ANTERIOR
                if (AnteriorEMI != null)
                    if (AnteriorEMI.EMI_ULTIMOS_EMPLEOS != null)
                    {
                        LstUltimosEmpleos = new ObservableCollection<SSP.Servidor.EMI_ULTIMOS_EMPLEOS>(AnteriorEMI.EMI_ULTIMOS_EMPLEOS);
                        IsEmpleosEmpty = LstUltimosEmpleos == null ? true : LstUltimosEmpleos.Count <= 0 ? true : false;
                        return;
                    }

                //INICIALIZA VACIO
                LstUltimosEmpleos = new ObservableCollection<SSP.Servidor.EMI_ULTIMOS_EMPLEOS>();
                IsEmpleosEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ultimos empleos", ex);
            }
        }
    }
}

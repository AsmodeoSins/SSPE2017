using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        #region [Ficha de Identidad]
        private EMI_FICHA_IDENTIFICACION EMI_FICHA_IDENTIFICACION = new EMI_FICHA_IDENTIFICACION();
        private EMI_ULTIMOS_EMPLEOS EMI_ULTIMOS_EMPLEOS = new EMI_ULTIMOS_EMPLEOS();
        #region [Datos de la Ficha]
        private ObservableCollection<EDUCACION_GRADO> lstGradoEducativo;
        public ObservableCollection<EDUCACION_GRADO> LstGradoEducativo
        {
            get { return lstGradoEducativo; }
            set { lstGradoEducativo = value; OnPropertyChanged("LstGradoEducativo"); }
        }
        private ObservableCollection<EXFUNCIONARIO_SEGPUB> lstExfuncionario;
        public ObservableCollection<EXFUNCIONARIO_SEGPUB> LstExfuncionario
        {
            get { return lstExfuncionario; }
            set { lstExfuncionario = value; OnPropertyChanged("LstExfuncionario"); } 
        }
        private ObservableCollection<EDUCACION_CERTIFICADO> lstCertificadoEduacion;
        public ObservableCollection<EDUCACION_CERTIFICADO> LstCertificadoEduacion
        {
            get { return lstCertificadoEduacion; }
            set { lstCertificadoEduacion = value; OnPropertyChanged("LstCertificadoEduacion"); }
        }

        private EDUCACION_GRADO selectedGradoEducativo;
        public EDUCACION_GRADO SelectedGradoEducativo
        {
            get { return selectedGradoEducativo; }
            set { selectedGradoEducativo = value; OnPropertyChanged("SelectedGradoEducativo"); }
        }

        private TIPO_REFERENCIA selectedParentescoVivia;
        public TIPO_REFERENCIA SelectedParentescoVivia
        {
            get { return selectedParentescoVivia; }
            set { selectedParentescoVivia = value; OnPropertyChanged("SelectedParentescoVivia"); }
        }

        private EXFUNCIONARIO_SEGPUB selectedExfuncionario;
        public EXFUNCIONARIO_SEGPUB SelectedExfuncionario
        {
            get { return selectedExfuncionario; }
            set { selectedExfuncionario = value; OnPropertyChanged("SelectedExfuncionario"); }
        }

        private EMISOR selectedEmisorProcedencia;
        public EMISOR SelectedEmisorProcedencia
        {
            get { return selectedEmisorProcedencia; }
            set { selectedEmisorProcedencia = value; OnPropertyChanged("SelectedEmisorProcedencia"); }
        }

        private EDUCACION_CERTIFICADO selectedCertificadoEducacion;


        private bool fechaCapturaValid = false;
        public bool FechaCapturaValid
        {
            get { return fechaCapturaValid; }
            set { fechaCapturaValid = value; OnPropertyChanged("FechaCapturaValid"); }
        }
        
        public DateTime? FechaCaptura
        {
            get { return EMI_FICHA_IDENTIFICACION.FEC_CAPTURA; }
            set
            {
                EMI_FICHA_IDENTIFICACION.FEC_CAPTURA = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                //setValidacionesFichaIdentificacion();
                if (value.HasValue)
                    FechaCapturaValid = true;
                else
                    FechaCapturaValid = false;
                OnPropertyValidateChanged("FechaCaptura");
            }
        }
        public string TiempoColonia
        {
            get { return EMI_FICHA_IDENTIFICACION.TIEMPO_RESID_COL; }
            set
            {
                EMI_FICHA_IDENTIFICACION.TIEMPO_RESID_COL = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                OnPropertyValidateChanged("TiempoColonia");
            }
        }
        public short? UltimoGradoEducativoConcluido
        {
            get { return EMI_FICHA_IDENTIFICACION.ID_GRADO_EDUCATIVO_CONCLUIDO; }
            set
            {
                EMI_FICHA_IDENTIFICACION.ID_GRADO_EDUCATIVO_CONCLUIDO = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                OnPropertyValidateChanged("UltimoGradoEducativoConcluido");
            }
        }
        public string ViviaAntesDetencion
        {
            get { return EMI_FICHA_IDENTIFICACION.PERSONA_CONVIVENCIA_ANTERIOR; }
            set
            {
                EMI_FICHA_IDENTIFICACION.PERSONA_CONVIVENCIA_ANTERIOR = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                OnPropertyValidateChanged("ViviaAntesDetencion");
            }
        }
        public short? Parentesco
        {
            get { return EMI_FICHA_IDENTIFICACION.ID_PARENTESCO; }
            set
            {
                EMI_FICHA_IDENTIFICACION.ID_PARENTESCO = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                OnPropertyValidateChanged("Parentesco");
            }
        }

        //private ObservableCollection<EMISOR> lstEmisores;
        //public ObservableCollection<EMISOR> LstEmisores
        //{
        //    get { return lstEmisores; }
        //    set
        //    {
        //        lstEmisores = value;
        //        OnPropertyChanged("LstEmisores");
        //    }
        //}

        private ObservableCollection<TIPO_REFERENCIA> lstParentesco;
        public ObservableCollection<TIPO_REFERENCIA> LstParentesco
        {
            get { return lstParentesco; }
            set
            {
                lstParentesco = value;
                OnPropertyChanged("LstParentesco");
            }
        }

        private ObservableCollection<TIPO_REFERENCIA> lstParentesco2;
        public ObservableCollection<TIPO_REFERENCIA> LstParentesco2
        {
            get { return lstParentesco; }
            set
            {
                lstParentesco = value;
                OnPropertyChanged("LstParentesco2");
            }
        }

        public short? ExFuncionarioSeguridadPublica
        {
            get { return EMI_FICHA_IDENTIFICACION.ID_EXFUNCIONARIO_SEGPUB; }
            set
            {
                EMI_FICHA_IDENTIFICACION.ID_EXFUNCIONARIO_SEGPUB = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                OnPropertyValidateChanged("ExFuncionarioSeguridadPublica");
            }
        }
        public int? CeresoProcedencia
        {
            get { return EMI_FICHA_IDENTIFICACION.ID_CERESO_PROCEDENCIA; }
            set
            {
                EMI_FICHA_IDENTIFICACION.ID_CERESO_PROCEDENCIA = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                OnPropertyValidateChanged("CeresoProcedencia");
            }
        }
        #region [Documentacion Oficial]
        public bool ActaNacimiento
        {
            get { return EMI_FICHA_IDENTIFICACION.ACTA_NACIMIENTO == "S" ? true : false; }
            set
            {
                EMI_FICHA_IDENTIFICACION.ACTA_NACIMIENTO = value ? "S" : "N";
                OnPropertyValidateChanged("ActaNacimiento");
            }
        }
        public bool Pasaporte
        {
            get { return EMI_FICHA_IDENTIFICACION.PASAPORTE == "S" ? true : false; }
            set
            {
                EMI_FICHA_IDENTIFICACION.PASAPORTE = value ? "S" : "N";
                OnPropertyValidateChanged("Pasaporte");
            }
        }
        public bool LicenciaManejo
        {
            get { return EMI_FICHA_IDENTIFICACION.LICENCIA_MANEJO == "S" ? true : false; }
            set
            {
                EMI_FICHA_IDENTIFICACION.LICENCIA_MANEJO = value ? "S" : "N";
                OnPropertyValidateChanged("LicenciaManejo");
            }
        }
        public bool CredencialElector
        {
            get { return EMI_FICHA_IDENTIFICACION.CREDENCIAL_ELECTOR == "S" ? true : false; }
            set
            {
                EMI_FICHA_IDENTIFICACION.CREDENCIAL_ELECTOR = value ? "S" : "N";
                OnPropertyValidateChanged("CredencialElector");
            }
        }
        public bool CartillaMilitar
        {
            get { return EMI_FICHA_IDENTIFICACION.CARTILLA_MILITAR == "S" ? true : false; }
            set
            {
                EMI_FICHA_IDENTIFICACION.CARTILLA_MILITAR = value ? "S" : "N";
                OnPropertyValidateChanged("CartillaMilitar");
            }
        }
        public short? CertificadoEducacion
        {
            get { return EMI_FICHA_IDENTIFICACION.ID_CERTIFICADO_EDUCACION; }
            set
            {
                EMI_FICHA_IDENTIFICACION.ID_CERTIFICADO_EDUCACION = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                OnPropertyValidateChanged("CertificadoEducacion");
            }
        }
        public string OficiosHabilidades
        {
            get { return EMI_FICHA_IDENTIFICACION.OFICIOS_HABILIDADES; }
            set
            {
                EMI_FICHA_IDENTIFICACION.OFICIOS_HABILIDADES = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                OnPropertyValidateChanged("OficiosHabilidades");
            }
        }
        #endregion
        #region [Cambios de Domicilio]
        public short? UltimoAnio
        {
            get { return EMI_FICHA_IDENTIFICACION.CAMBIOS_DOMICILIO_ULTIMO_ANO; }
            set
            {
                EMI_FICHA_IDENTIFICACION.CAMBIOS_DOMICILIO_ULTIMO_ANO = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                OnPropertyValidateChanged("UltimoAnio");
            }
        }
        public string Motivo
        {
            get { return EMI_FICHA_IDENTIFICACION.MOTIVOS_CAMBIOS_DOMICILIO; }
            set
            {
                EMI_FICHA_IDENTIFICACION.MOTIVOS_CAMBIOS_DOMICILIO = value;
                EmpleosAnterioresEnabled = ValidarUltimosEmpleos();
                OnPropertyValidateChanged("Motivo");
            }
        }
        #endregion
        #endregion
        #region [Ultimo Empleo y 3 Anteriores]
        private bool _IsEmpleosEmpty;
        public bool IsEmpleosEmpty
        {
            get { return _IsEmpleosEmpty; }
            set
            {
                _IsEmpleosEmpty = value;
                OnPropertyChanged("IsEmpleosEmpty");
            }
        }

        private ObservableCollection<EMI_ULTIMOS_EMPLEOS> lstUltimosEmpleos;
        public ObservableCollection<EMI_ULTIMOS_EMPLEOS> LstUltimosEmpleos
        {
            get { return lstUltimosEmpleos; }
            set
            {
                lstUltimosEmpleos = value;
                OnPropertyValidateChanged("LstUltimosEmpleos");
            }
        }

        private EMI_ULTIMOS_EMPLEOS _SelectedEmpleo;
        public EMI_ULTIMOS_EMPLEOS SelectedEmpleo
        {
            get { return _SelectedEmpleo; }
            set
            {
                _SelectedEmpleo = value;
                OnPropertyChanged("SelectedEmpleo");
                if (SelectedEmpleo != null)
                {
                    var isInDB = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIUltimosEmpleos().GetData(w => w.ID_EMI == emiActual.ID_EMI && w.ID_EMI_CONS == emiActual.ID_EMI_CONS && w.ID_EMI_ULTIMOS_EMPLEOS== SelectedEmpleo.ID_EMI_ULTIMOS_EMPLEOS);
                    EliminarItemMenu = isInDB.Count() > 0 ? false : true;
                }
            }
        }

        private ObservableCollection<OCUPACION> lstOcupacion;
        public ObservableCollection<OCUPACION> LstOcupacion
        {
            get { return lstOcupacion; }
            set { lstOcupacion = value; OnPropertyChanged("LstOcupacion"); }
        }

        private OCUPACION selectedOcupacion;
        public OCUPACION SelectedOcupacion
        {
            get { return selectedOcupacion; }
            set { selectedOcupacion = value; OnPropertyChanged("SelectedOcupacion"); }
        }

        private string duracion;
        public string Duracion
        {
            get { return duracion; }
            set { duracion = value; OnPropertyChanged("Duracion"); }
        }

        private string empresa;
        public string Empresa
        {
            get { return empresa; }
            set { empresa = value; OnPropertyChanged("Empresa"); }
        }
        
        private string motivoDesempleo;
        public string MotivoDesempleo
        {
            get { return motivoDesempleo; }
            set { motivoDesempleo = value; OnPropertyChanged("MotivoDesempleo"); }
        }

        private bool empleoFormal;
        public bool EmpleoFormal
        {
            get { return empleoFormal; }
            set { empleoFormal = value; OnPropertyChanged("EmpleoFormal"); }
        }

        private bool ultimoAntesDetencion;
        public bool UltimoAntesDetencion
        {
            get { return ultimoAntesDetencion; }
            set { ultimoAntesDetencion = value; OnPropertyChanged("UltimoAntesDetencion"); }
        }
 
        private bool inestabilidadLaboral;
        public bool InestabilidadLaboral
        {
            get { return inestabilidadLaboral; }
            set { inestabilidadLaboral = value; OnPropertyChanged("InestabilidadLaboral"); }
        }

        private bool empleosAnterioresEnabled=false;
        public bool EmpleosAnterioresEnabled
        {
            get { return empleosAnterioresEnabled; }
            set { empleosAnterioresEnabled = value; OnPropertyChanged("EmpleosAnterioresEnabled"); }
        }
        
        //public short? Ocupacion
        //{
        //    get { return EMI_ULTIMOS_EMPLEOS.OCUPACION; }
        //    set
        //    {
        //        EMI_ULTIMOS_EMPLEOS.OCUPACION = value;
        //        OnPropertyChanged("Ocupacion");
        //    }
        //}
        //public string Duracion
        //{
        //    get { return EMI_ULTIMOS_EMPLEOS.DURACION; }
        //    set
        //    {
        //        EMI_ULTIMOS_EMPLEOS.DURACION = value;
        //        OnPropertyChanged("Duracion");
        //    }
        //}
        //public string Empresa
        //{
        //    get { return EMI_ULTIMOS_EMPLEOS.EMPRESA; }
        //    set
        //    {
        //        EMI_ULTIMOS_EMPLEOS.EMPRESA = value;
        //        OnPropertyChanged("Empresa");
        //    }
        //}
        //public string MotivoDesempleo
        //{
        //    get { return EMI_ULTIMOS_EMPLEOS.MOTIVO_DESEMPLEO; }
        //    set
        //    {
        //        EMI_ULTIMOS_EMPLEOS.MOTIVO_DESEMPLEO = value;
        //        OnPropertyChanged("MotivoDesempleo");
        //    }
        //}
        //public string EmpleoFinal
        //{
        //    get { return EMI_ULTIMOS_EMPLEOS.EMPLEO_FORMAL; }
        //    set
        //    {
        //        EMI_ULTIMOS_EMPLEOS.EMPLEO_FORMAL = value;
        //        OnPropertyChanged("EmpleoFinal");
        //    }
        //}
        //public string UltimoAntesDetencion
        //{
        //    get { return EMI_ULTIMOS_EMPLEOS.ULTIMO_EMPLEO_ANTES_DETENCION; }
        //    set
        //    {
        //        EMI_ULTIMOS_EMPLEOS.ULTIMO_EMPLEO_ANTES_DETENCION = value;
        //        OnPropertyChanged("UltimoAntesDetencion");
        //    }
        //}
        //public string InestabilidadLaboral
        //{
        //    get { return EMI_ULTIMOS_EMPLEOS.INESTABILIDAD_LABORAL; }
        //    set
        //    {
        //        EMI_ULTIMOS_EMPLEOS.INESTABILIDAD_LABORAL = value;
        //        OnPropertyChanged("InestabilidadLaboral");
        //    }
        //}
        #endregion
        #region [Uso Drogas]
        private bool _IsUsoDrogasEmpty;
        public bool IsUsoDrogasEmpty
        {
            get { return _IsUsoDrogasEmpty; }
            set
            {
                _IsUsoDrogasEmpty = value;
                OnPropertyChanged("IsUsoDrogasEmpty");
            }
        }
        #endregion
        #region [Visible]
        private bool agregarTrabajoVisible = false;
        public bool AgregarTrabajoVisible
        {
            get { return agregarTrabajoVisible; }
            set
            {
                agregarTrabajoVisible = value;
                OnPropertyChanged("AgregarTrabajoVisible");
            }
        }
        private bool agregarUsoDrogasVisible;
        public bool AgregarUsoDrogasVisible
        {
            get { return agregarUsoDrogasVisible; }
            set
            {
                agregarUsoDrogasVisible = value;
                OnPropertyChanged("AgregarUsoDrogasVisible");
            }
        }
        #endregion

        #region Tabs
        private bool tabFichaIdentificacion;
        public bool TabFichaIdentificacion
        {
            get { return tabFichaIdentificacion; }
            set { tabFichaIdentificacion = value; OnPropertyChanged("TabFichaIdentificacion"); }
        }
        #endregion

        #endregion
    }
}

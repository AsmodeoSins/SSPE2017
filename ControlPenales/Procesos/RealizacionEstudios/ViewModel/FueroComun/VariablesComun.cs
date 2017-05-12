using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class RealizacionEstudiosViewModel
    {
        #region Estudio Medico
        private string _EdadInterno;
        public string EdadInterno
        {
            get { return _EdadInterno; }
            set { _EdadInterno = value; OnPropertyChanged("EdadInterno"); }
        }

        private string _SexoInterno;
        public string SexoInterno
        {
            get { return _SexoInterno; }
            set { _SexoInterno = value; OnPropertyChanged("SexoInterno"); }
        }

        private string _AntecedentesHeredoFamiliares;
        public string AntecedentesHeredoFamiliares
        {
            get { return _AntecedentesHeredoFamiliares; }
            set { _AntecedentesHeredoFamiliares = value; OnPropertyChanged("AntecedentesHeredoFamiliares"); }
        }

        private string _ImpresionDiagnosticaEstudioMedicoComun;
        public string ImpresionDiagnosticaEstudioMedicoComun
        {
            get { return _ImpresionDiagnosticaEstudioMedicoComun; }
            set { _ImpresionDiagnosticaEstudioMedicoComun = value; OnPropertyChanged("ImpresionDiagnosticaEstudioMedicoComun"); }
        }

        private short? _IdDictamenMedicoComun;
        public short? IdDictamenMedicoComun
        {
            get { return _IdDictamenMedicoComun; }
            set { _IdDictamenMedicoComun = value; OnPropertyChanged("IdDictamenMedicoComun"); }
        }

        private DateTime? _FechaEstudioMedicoComun;
        public DateTime? FechaEstudioMedicoComun
        {
            get { return _FechaEstudioMedicoComun; }
            set {  _FechaEstudioMedicoComun = value; OnPropertyChanged("FechaEstudioMedicoComun"); }
        }

        private string _AntecedentesPersonalesNoPatologicos;
        public string AntecedentesPersonalesNoPatologicos
        {
            get { return _AntecedentesPersonalesNoPatologicos; }
            set { _AntecedentesPersonalesNoPatologicos = value; OnPropertyChanged("AntecedentesPersonalesNoPatologicos"); }
        }

        private string _AntedecentesConsumoToxicosEstadoActual;
        public string AntedecentesConsumoToxicosEstadoActual
        {
            get { return _AntedecentesConsumoToxicosEstadoActual; }
            set { _AntedecentesConsumoToxicosEstadoActual = value; OnPropertyChanged("AntedecentesConsumoToxicosEstadoActual"); }
        }

        private string _DescripcionTatuajesCicatricesMalformaciones;
        public string DescripcionTatuajesCicatricesMalformaciones
        {
            get { return _DescripcionTatuajesCicatricesMalformaciones; }
            set { _DescripcionTatuajesCicatricesMalformaciones = value; OnPropertyChanged("DescripcionTatuajesCicatricesMalformaciones"); }
        }

        private string _DescipcionPadecimientoActual;
        public string DescipcionPadecimientoActual
        {
            get { return _DescipcionPadecimientoActual; }
            set { _DescipcionPadecimientoActual = value; OnPropertyChanged("DescipcionPadecimientoActual"); }
        }

        #endregion

        #region Estudio Psiquiatrico
        private List<IGrouping<TIPO_PROGRAMA, GRUPO_PARTICIPANTE>> _GruposProgramas; //= new List<IGrouping<TIPO_PROGRAMA, GRUPO_PARTICIPANTE>>(); Usada para los grupos
        public List<IGrouping<TIPO_PROGRAMA, GRUPO_PARTICIPANTE>> GruposProgramas
        {
            get { return _GruposProgramas; }
            set { _GruposProgramas = value; OnPropertyChanged("GruposProgramas"); }
        }

        private string _AspectoFisico;
        public string AspectoFisico
        {
            get { return _AspectoFisico; }
            set { _AspectoFisico = value; OnPropertyChanged("AspectoFisico"); }
        }

        private string _ConductaMotora;
        public string ConductaMotora
        {
            get { return _ConductaMotora; }
            set { _ConductaMotora = value; OnPropertyChanged("ConductaMotora"); }
        }

        private string _Habla;
        public string Habla
        {
            get { return _Habla; }
            set { _Habla = value; OnPropertyChanged("Habla"); }
        }

        private string _Actitud;
        public string Actitud
        {
            get { return _Actitud; }
            set { _Actitud = value; OnPropertyChanged("Actitud"); }
        }

        private string _EstadoAnimo;
        public string EstadoAnimo
        {
            get { return _EstadoAnimo; }
            set { _EstadoAnimo = value; OnPropertyChanged("EstadoAnimo"); }
        }

        private string _ExpresionAfectiva;
        public string ExpresionAfectiva
        {
            get { return _ExpresionAfectiva; }
            set { _ExpresionAfectiva = value; OnPropertyChanged("ExpresionAfectiva"); }
        }

        private string _Adecuacion;
        public string Adecuacion
        {
            get { return _Adecuacion; }
            set { _Adecuacion = value; OnPropertyChanged("Adecuacion"); }
        }

        private string _Alucinaciones;
        public string Alucinaciones
        {
            get { return _Alucinaciones; }
            set { _Alucinaciones = value; OnPropertyChanged("Alucinaciones"); }
        }

        private string _Ilusiones;
        public string Ilusiones
        {
            get { return _Ilusiones; }
            set { _Ilusiones = value; OnPropertyChanged("Ilusiones"); }
        }

        private string _Despersonalizacion;
        public string Despersonalizacion
        {
            get { return _Despersonalizacion; }
            set { _Despersonalizacion = value; OnPropertyChanged("Despersonalizacion"); }
        }

        private string _Desrealizacion;
        public string Desrealizacion
        {
            get { return _Desrealizacion; }
            set { _Desrealizacion = value; OnPropertyChanged("Desrealizacion"); }
        }

        private string _CursoPensamiento;
        public string CursoPensamiento
        {
            get { return _CursoPensamiento; }
            set { _CursoPensamiento = value; OnPropertyChanged("CursoPensamiento"); }
        }

        private string _ContinuidadPensamiento;
        public string ContinuidadPensamiento
        {
            get { return _ContinuidadPensamiento; }
            set { _ContinuidadPensamiento = value; OnPropertyChanged("ContinuidadPensamiento"); }
        }

        private string _ContenidoPensamiento;
        public string ContenidoPensamiento
        {
            get { return _ContenidoPensamiento; }
            set { _ContenidoPensamiento = value; OnPropertyChanged("ContenidoPensamiento"); }
        }

        private string _PensamientoAbstracto;
        public string PensamientoAbstracto
        {
            get { return _PensamientoAbstracto; }
            set { _PensamientoAbstracto = value; OnPropertyChanged("PensamientoAbstracto"); }
        }

        private string _Concentracion;
        public string Concentracion
        {
            get { return _Concentracion; }
            set { _Concentracion = value; OnPropertyChanged("Concentracion"); }
        }

        private string _Orientacion;
        public string Orientacion
        {
            get { return _Orientacion; }
            set { _Orientacion = value; OnPropertyChanged("Orientacion"); }
        }

        private string _Memoria;
        public string Memoria
        {
            get { return _Memoria; }
            set { _Memoria = value; OnPropertyChanged("Memoria"); }
        }

        private string _BajaToleranciaFrustr;
        public string BajaToleranciaFrustr
        {
            get { return _BajaToleranciaFrustr; }
            set { _BajaToleranciaFrustr = value; OnPropertyChanged("BajaToleranciaFrustr"); }
        }

        private string _ExpresionDesadaptativa;
        public string ExpresionDesadaptativa
        {
            get { return _ExpresionDesadaptativa; }
            set { _ExpresionDesadaptativa = value; OnPropertyChanged("ExpresionDesadaptativa"); }
        }

        private string _Adecuada;
        public string Adecuada
        {
            get { return _Adecuada; }
            set { _Adecuada = value; OnPropertyChanged("Adecuada"); }
        }

        private string _CapacidadJuicio;
        public string CapacidadJuicio
        {
            get { return _CapacidadJuicio; }
            set { _CapacidadJuicio = value; OnPropertyChanged("CapacidadJuicio"); }
        }

        private string _Introspeccion;
        public string Introspeccion
        {
            get { return _Introspeccion; }
            set { _Introspeccion = value; OnPropertyChanged("Introspeccion"); }
        }

        private string _Fiabilidad;
        public string Fiabilidad
        {
            get { return _Fiabilidad; }
            set { _Fiabilidad = value; OnPropertyChanged("Fiabilidad"); }
        }

        private string _ImpresionDiagnosticaPsiquiatricoComun;
        public string ImpresionDiagnosticaPsiquiatricoComun
        {
            get { return _ImpresionDiagnosticaPsiquiatricoComun; }
            set { _ImpresionDiagnosticaPsiquiatricoComun = value; OnPropertyChanged("ImpresionDiagnosticaPsiquiatricoComun"); }
        }

        private short? _DictamenPsiqComun;
        public short? DictamenPsiqComun
        {
            get { return _DictamenPsiqComun; }
            set { _DictamenPsiqComun = value; OnPropertyChanged("DictamenPsiqComun"); }
        }

        private DateTime? _FecDictamenPsiqComun;
        public DateTime? FecDictamenPsiqComun
        {
            get { return _FecDictamenPsiqComun; }
            set { _FecDictamenPsiqComun = value; OnPropertyChanged("FecDictamenPsiqComun"); }
        }


        #endregion

        #region Estudio Psicologico

        #region Listas PsicologicoFuero Comun
        ObservableCollection<PFC_IV_NIVEL_INTELECTUAL> _LstNivelIntelectualComun;
        public ObservableCollection<PFC_IV_NIVEL_INTELECTUAL> LstNivelIntelectualComun
        {
            get { return _LstNivelIntelectualComun; }
            set { _LstNivelIntelectualComun = value; OnPropertyChanged("LstNivelIntelectualComun"); }
        }

        private ObservableCollection<PFC_IV_DISFUNCION> _ListDisfuncionNComun;
        public ObservableCollection<PFC_IV_DISFUNCION> ListDisfuncionNComun
        {
            get { return _ListDisfuncionNComun; }
            set { _ListDisfuncionNComun = value; OnPropertyChanged("ListDisfuncionNComun"); }
        }

        private ObservableCollection<PFC_V_CRIMINOLOGICA> _ListClasifCriminologica;
        public ObservableCollection<PFC_V_CRIMINOLOGICA> ListClasifCriminologica
        {
            get { return _ListClasifCriminologica; }
            set { _ListClasifCriminologica = value; OnPropertyChanged("ListClasifCriminologica"); }
        }



        #endregion

        private string _CondicionesGralesInterno;
        public string CondicionesGralesInterno
        {
            get { return _CondicionesGralesInterno; }
            set { _CondicionesGralesInterno = value; OnPropertyChanged("CondicionesGralesInterno"); }
        }

        private ObservableCollection<PFC_IV_PROGRAMA> lstProgramasPsicologico;
        public ObservableCollection<PFC_IV_PROGRAMA> LstProgramasPsicologico
        {
            get { return lstProgramasPsicologico; }
            set { lstProgramasPsicologico = value; OnPropertyChanged("LstProgramasPsicologico"); }
        }

        private PFC_IV_PROGRAMA _SelectedPsicologicoGrupo;
        public PFC_IV_PROGRAMA SelectedPsicologicoGrupo
        {
            get { return _SelectedPsicologicoGrupo; }
            set { _SelectedPsicologicoGrupo = value; OnPropertyChanged("SelectedPsicologicoGrupo"); }
        }

        private ObservableCollection<PFC_IV_PROGRAMA> lstProgModifConduc;
        public ObservableCollection<PFC_IV_PROGRAMA> LstProgModifConduc
        {
            get { return lstProgModifConduc; }
            set { lstProgModifConduc = value; OnPropertyChanged("LstProgModifConduc"); }
        }

        private ObservableCollection<PFC_IV_PROGRAMA> lstComplement;
        public ObservableCollection<PFC_IV_PROGRAMA> LstComplement
        {
            get { return lstComplement; }
            set { lstComplement = value; OnPropertyChanged("LstComplement"); }
        }

        private ObservableCollection<PFC_IV_PROGRAMA> lstTalleresOrient;
        public ObservableCollection<PFC_IV_PROGRAMA> LstTalleresOrient
        {
            get { return lstTalleresOrient; }
            set { lstTalleresOrient = value; OnPropertyChanged("LstTalleresOrient"); }
        }

        private string _DuracionrupoIV;
        public string DuracionrupoIV
        {
            get { return _DuracionrupoIV; }
            set { _DuracionrupoIV = value; OnPropertyChanged("DuracionrupoIV"); }
        }

        private string _ObservacionesGrupoIV;
        public string ObservacionesGrupoIV
        {
            get { return _ObservacionesGrupoIV; }
            set { _ObservacionesGrupoIV = value; OnPropertyChanged("ObservacionesGrupoIV"); }
        }

        private string _ConcluidoGrupoIV;
        public string ConcluidoGrupoIV
        {
            get { return _ConcluidoGrupoIV; }
            set { _ConcluidoGrupoIV = value; OnPropertyChanged("ConcluidoGrupoIV"); }
        }


        ObservableCollection<GRUPO_PARTICIPANTE> _Progr;
        public ObservableCollection<GRUPO_PARTICIPANTE> Progr
        {
            get { return _Progr; }
            set { _Progr = value; OnPropertyChanged("Progr"); }
        }

        private string _DescripcionPrincipalesRazgosIngreso;
        public string DescripcionPrincipalesRazgosIngreso
        {
            get { return _DescripcionPrincipalesRazgosIngreso; }
            set { _DescripcionPrincipalesRazgosIngreso = value; OnPropertyChanged("DescripcionPrincipalesRazgosIngreso"); }
        }

        private bool _IsTestLaurettaBenderChecked = false;
        public bool IsTestLaurettaBenderChecked
        {
            get { return _IsTestLaurettaBenderChecked; }
            set { _IsTestLaurettaBenderChecked = value; OnPropertyChanged("IsTestLaurettaBenderChecked"); }
        }

        private bool _IsTestMatricesProgresivasRavenChecked = false;
        public bool IsTestMatricesProgresivasRavenChecked
        {
            get { return _IsTestMatricesProgresivasRavenChecked; }
            set { _IsTestMatricesProgresivasRavenChecked = value; OnPropertyChanged("IsTestMatricesProgresivasRavenChecked"); }
        }

        private bool _IsTestCasaArbolPersonaChecked = false;
        public bool IsTestCasaArbolPersonaChecked
        {
            get { return _IsTestCasaArbolPersonaChecked; }
            set { _IsTestCasaArbolPersonaChecked = value; OnPropertyChanged("IsTestCasaArbolPersonaChecked"); }
        }

        private bool _IsEnabledInfluenciaDrogas = false;
        public bool IsEnabledInfluenciaDrogas
        {
            get { return _IsEnabledInfluenciaDrogas; }
            set { _IsEnabledInfluenciaDrogas = value; OnPropertyChanged("IsEnabledInfluenciaDrogas"); }
        }

        private string _IdReqExtraMurosPsicologicoComun;
        public string IdReqExtraMurosPsicologicoComun
        {
            get { return _IdReqExtraMurosPsicologicoComun; }
            set
            {
                _IdReqExtraMurosPsicologicoComun = value;
                if (!string.IsNullOrEmpty(value) && value == "S")
                {
                    base.RemoveRule("CualTratamientoExtraMurosPsicologicoComun");
                    IsEnabledCualTratamExtraMurosPsicologicoComun = true;
                    base.AddRule(() => CualTratamientoExtraMurosPsicologicoComun, () => !string.IsNullOrEmpty(CualTratamientoExtraMurosPsicologicoComun), "CUAL TRATAMIENTO ES REQUERIDO!");
                }

                else
                {
                    IsEnabledCualTratamExtraMurosPsicologicoComun = false;
                    base.RemoveRule("CualTratamientoExtraMurosPsicologicoComun");
                }

                OnPropertyChanged("IsEnabledCualTratamExtraMurosPsicologicoComun");
                OnPropertyChanged("CualTratamientoExtraMurosPsicologicoComun");
                OnPropertyChanged("IdReqExtraMurosPsicologicoComun");
            }
        }

        private string _CualTratamientoExtraMurosPsicologicoComun;
        public string CualTratamientoExtraMurosPsicologicoComun
        {
            get { return _CualTratamientoExtraMurosPsicologicoComun; }
            set { _CualTratamientoExtraMurosPsicologicoComun = value; OnPropertyChanged("CualTratamientoExtraMurosPsicologicoComun"); }
        }

        private bool _IsEnabledCualTratamExtraMurosPsicologicoComun = false;
        public bool IsEnabledCualTratamExtraMurosPsicologicoComun
        {
            get { return _IsEnabledCualTratamExtraMurosPsicologicoComun; }
            set { _IsEnabledCualTratamExtraMurosPsicologicoComun = value; OnPropertyChanged("IsEnabledCualTratamExtraMurosPsicologicoComun"); }
        }


        private bool _IsTestMinesotaChecked = false;
        public bool IsTestMinesotaChecked
        {
            get { return _IsTestMinesotaChecked; }
            set { _IsTestMinesotaChecked = value; OnPropertyChanged("IsTestMinesotaChecked"); }
        }

        private bool _IsTestOtrosChecked = false;
        public bool IsTestOtrosChecked
        {
            get { return _IsTestOtrosChecked; }
            set
            {
                _IsTestOtrosChecked = value;
                if (value)
                {
                    base.RemoveRule("EspecifiqueOtroTest");
                    IsEnabledOtroTest = true;
                    base.AddRule(() => EspecifiqueOtroTest, () => !string.IsNullOrEmpty(EspecifiqueOtroTest), "OTRAS PRUEBAS ES REQUERIDO!");
                }
                else
                {
                    IsEnabledOtroTest = false;
                    base.RemoveRule("EspecifiqueOtroTest");
                }

                OnPropertyChanged("IsTestOtrosChecked");
                OnPropertyChanged("IsEnabledOtroTest");
                OnPropertyChanged("EspecifiqueOtroTest");
            }
        }

        private bool _IsEnabledOtroTest = false;
        public bool IsEnabledOtroTest
        {
            get { return _IsEnabledOtroTest; }
            set { _IsEnabledOtroTest = value; OnPropertyChanged("IsEnabledOtroTest"); }
        }

        private string _EspecifiqueOtroTest;
        public string EspecifiqueOtroTest
        {
            get { return _EspecifiqueOtroTest; }
            set { _EspecifiqueOtroTest = value; OnPropertyChanged("EspecifiqueOtroTest"); }
        }

        private short? _IdNivelIntelectual;
        public short? IdNivelIntelectual
        {
            get { return _IdNivelIntelectual; }
            set { _IdNivelIntelectual = value; OnPropertyChanged("IdNivelIntelectual"); }
        }

        private short? _IdDisfuncionNeurologica;
        public short? IdDisfuncionNeurologica
        {
            get { return _IdDisfuncionNeurologica; }
            set { _IdDisfuncionNeurologica = value; OnPropertyChanged("IdDisfuncionNeurologica"); }
        }

        private string _IntegracionDinamicaPersonalidadActual;
        public string IntegracionDinamicaPersonalidadActual
        {
            get { return _IntegracionDinamicaPersonalidadActual; }
            set { _IntegracionDinamicaPersonalidadActual = value; OnPropertyChanged("IntegracionDinamicaPersonalidadActual"); }
        }

        private string _RasgosPersonalidadRelaciondosComisionDelito;
        public string RasgosPersonalidadRelaciondosComisionDelito
        {
            get { return _RasgosPersonalidadRelaciondosComisionDelito; }
            set { _RasgosPersonalidadRelaciondosComisionDelito = value; OnPropertyChanged("RasgosPersonalidadRelaciondosComisionDelito"); }
        }

        private string _SenialeProgramasDebeRemitirseInterno;
        public string SenialeProgramasDebeRemitirseInterno
        {
            get { return _SenialeProgramasDebeRemitirseInterno; }
            set { _SenialeProgramasDebeRemitirseInterno = value; OnPropertyChanged("SenialeProgramasDebeRemitirseInterno"); }
        }

        private short? _IdDictamenPsicologicoComun;
        public short? IdDictamenPsicologicoComun
        {
            get { return _IdDictamenPsicologicoComun; }
            set
            {

                _IdDictamenPsicologicoComun = value;
                if (value == (short)eDiagnosticoDictamen.DESFAVORABLE)
                {
                    base.RemoveRule("CasoNegativoEstudioPsicologicoComun");
                    IsRequeridoCasoN = true;
                    base.AddRule(() => CasoNegativoEstudioPsicologicoComun, () => !string.IsNullOrEmpty(CasoNegativoEstudioPsicologicoComun), "ESPECIFIQUE PROGRAMAS ES REQUERIDO!");
                    OnPropertyChanged("CasoNegativoEstudioPsicologicoComun");
                    OnPropertyChanged("IsRequeridoCasoN");
                }

                else
                {
                    IsRequeridoCasoN = false;
                    base.RemoveRule("CasoNegativoEstudioPsicologicoComun");
                    OnPropertyChanged("CasoNegativoEstudioPsicologicoComun");
                    OnPropertyChanged("IsRequeridoCasoN");
                }

                OnPropertyChanged("IsRequeridoCasoN");
                OnPropertyChanged("IdDictamenPsicologicoComun");
                OnPropertyChanged("CasoNegativoEstudioPsicologicoComun");
            }
        }

        private bool _IsRequeridoCasoN = false;

        public bool IsRequeridoCasoN
        {
            get { return _IsRequeridoCasoN; }
            set { _IsRequeridoCasoN = value; OnPropertyChanged("IsRequeridoCasoN"); }
        }


        private string _NombreDina;

        public string NombreDina
        {
            get { return _NombreDina; }
            set { _NombreDina = value; OnPropertyChanged("NombreDina"); }
        }

        private short _Selected { get; set; }
        private string _NombreDina2;

        public string NombreDina2
        {
            get { return _NombreDina2; }
            set { _NombreDina2 = value; OnPropertyChanged("NombreDina2"); }
        }
        private DateTime? _FechaDictamenPsicologicoComun;
        public DateTime? FechaDictamenPsicologicoComun
        {
            get { return _FechaDictamenPsicologicoComun; }
            set { _FechaDictamenPsicologicoComun = value; OnPropertyChanged("FechaDictamenPsicologicoComun"); }
        }

        private string _MotivacionDictamenPsicologicoComun;
        public string MotivacionDictamenPsicologicoComun
        {
            get { return _MotivacionDictamenPsicologicoComun; }
            set { _MotivacionDictamenPsicologicoComun = value; OnPropertyChanged("MotivacionDictamenPsicologicoComun"); }
        }

        private string _CasoNegativoEstudioPsicologicoComun;
        public string CasoNegativoEstudioPsicologicoComun
        {
            get { return _CasoNegativoEstudioPsicologicoComun; }
            set { _CasoNegativoEstudioPsicologicoComun = value; OnPropertyChanged("CasoNegativoEstudioPsicologicoComun"); }
        }


        #endregion

        #region Estudio Criminodiagnostico

        private ObservableCollection<PFC_V_CAPACIDAD> lstCapacidad;
        public ObservableCollection<PFC_V_CAPACIDAD> LstCapacidad
        {
            get { return lstCapacidad; }
            set { lstCapacidad = value; OnPropertyChanged("LstCapacidad"); }
        }

        private ObservableCollection<PFC_V_PELIGROSIDAD> lstPeligrosidad;
        public ObservableCollection<PFC_V_PELIGROSIDAD> LstPeligrosidad
        {
            get { return lstPeligrosidad; }
            set { lstPeligrosidad = value; OnPropertyChanged("LstPeligrosidad"); }
        }

        private DateTime? _FechaDictamenCrimino;
        public DateTime? FechaDictamenCrimino
        {
            get { return _FechaDictamenCrimino; }
            set { _FechaDictamenCrimino = value; OnPropertyChanged("FechaDictamenCrimino"); }
        }

        private short? _DictamenCriminod;
        public short? DictamenCriminod
        {
            get { return _DictamenCriminod; }
            set
            {
                _DictamenCriminod = value;
                if (value != null)
                {
                    if (value == 2)//desfavorable
                    {
                        EnabledMotivComun1 = false;
                        base.RemoveRule("SenialeProgramasDebeRemitirseInterno");
                        base.AddRule(() => SenialeProgramasDebeRemitirseInterno, () => !string.IsNullOrEmpty(SenialeProgramasDebeRemitirseInterno), "SENALE PROGRAMAS ES REQUERIDO!");
                        OnPropertyChanged("SenialeProgramasDebeRemitirseInterno");
                        OnPropertyChanged("EnabledMotivComun1");
                    };

                    if (value == 1)//favorable
                    {
                        EnabledMotivComun1 = true;
                        base.RemoveRule("SenialeProgramasDebeRemitirseInterno");
                        OnPropertyChanged("SenialeProgramasDebeRemitirseInterno");
                        OnPropertyChanged("EnabledMotivComun1");
                    };
                };

                OnPropertyChanged("DictamenCriminod");
                OnPropertyChanged("SenialeProgramasDebeRemitirseInterno");
                OnPropertyChanged("EnabledMotivComun1");
            }
        }

        private bool _EnabledMotivComun1 = true;
        public bool EnabledMotivComun1
        {
            get { return _EnabledMotivComun1; }
            set { _EnabledMotivComun1 = value; OnPropertyChanged("EnabledMotivComun1"); }
        }

        private string _MotivacionDictamenCriminodiagnosticoComun;
        public string MotivacionDictamenCriminodiagnosticoComun
        {
            get { return _MotivacionDictamenCriminodiagnosticoComun; }
            set { _MotivacionDictamenCriminodiagnosticoComun = value; OnPropertyChanged("MotivacionDictamenCriminodiagnosticoComun"); }
        }

        private string _ReqTratamExtramurosCriminod;
        public string ReqTratamExtramurosCriminod
        {
            get { return _ReqTratamExtramurosCriminod; }
            set
            {
                _ReqTratamExtramurosCriminod = value;
                if (!string.IsNullOrEmpty(value) && value == "S")
                {
                    base.RemoveRule("CualTratamRemitirCriminodiagnosticoComun");
                    IsEnabledCualTratam = true;
                    base.AddRule(() => CualTratamRemitirCriminodiagnosticoComun, () => !string.IsNullOrEmpty(CualTratamRemitirCriminodiagnosticoComun), "CUAL TRATAMIENTO EXTRAMUROS ES REQUERIDO!");
                }

                else
                {
                    IsEnabledCualTratam = false;
                    base.RemoveRule("CualTratamRemitirCriminodiagnosticoComun");
                }

                OnPropertyChanged("ReqTratamExtramurosCriminod");
                OnPropertyChanged("CualTratamRemitirCriminodiagnosticoComun");
                OnPropertyChanged("IsEnabledCualTratam");
            }
        }

        private bool _IsEnabledCualTratam = false;
        public bool IsEnabledCualTratam
        {
            get { return _IsEnabledCualTratam; }
            set { _IsEnabledCualTratam = value; OnPropertyChanged("IsEnabledCualTratam"); }
        }

        private string _CualTratamRemitirCriminodiagnosticoComun;
        public string CualTratamRemitirCriminodiagnosticoComun
        {
            get { return _CualTratamRemitirCriminodiagnosticoComun; }
            set { _CualTratamRemitirCriminodiagnosticoComun = value; OnPropertyChanged("CualTratamRemitirCriminodiagnosticoComun"); }
        }


        private string _IdEncontrabaBajoInfluenciaDroga;
        public string IdEncontrabaBajoInfluenciaDroga
        {
            get { return _IdEncontrabaBajoInfluenciaDroga; }
            set
            {
                _IdEncontrabaBajoInfluenciaDroga = value;
                if (!string.IsNullOrEmpty(value) && value == "S")
                    IsEnabledInfluenciaDrogas = true;
                else
                    IsEnabledInfluenciaDrogas = false;

                OnPropertyChanged("IsEnabledInfluenciaDrogas");
                OnPropertyChanged("IdEncontrabaBajoInfluenciaDroga");
            }
        }

        private bool _IsAlcoholChecked = false;
        public bool IsAlcoholChecked
        {
            get { return _IsAlcoholChecked; }
            set { _IsAlcoholChecked = value; OnPropertyChanged("IsAlcoholChecked"); }
        }

        private bool _IsDrogasIlegalesChecked = false;
        public bool IsDrogasIlegalesChecked
        {
            get { return _IsDrogasIlegalesChecked; }
            set { _IsDrogasIlegalesChecked = value; OnPropertyChanged("IsDrogasIlegalesChecked"); }
        }

        private bool _IsOtraChecked = false;
        public bool IsOtraChecked
        {
            get { return _IsOtraChecked; }
            set { _IsOtraChecked = value; OnPropertyChanged("IsOtraChecked"); }
        }

        private string _CriminoGenesisEstudioCriminoFC;
        public string CriminoGenesisEstudioCriminoFC
        {
            get { return _CriminoGenesisEstudioCriminoFC; }
            set { _CriminoGenesisEstudioCriminoFC = value; OnPropertyChanged("CriminoGenesisEstudioCriminoFC"); }
        }

        private string _AntecedentesEvolucionConductasParaSociales;
        public string AntecedentesEvolucionConductasParaSociales
        {
            get { return _AntecedentesEvolucionConductasParaSociales; }
            set { _AntecedentesEvolucionConductasParaSociales = value; OnPropertyChanged("AntecedentesEvolucionConductasParaSociales"); }
        }

        private short? _IdClasificacionCriminologica;
        public short? IdClasificacionCriminologica
        {
            get { return _IdClasificacionCriminologica; }
            set { _IdClasificacionCriminologica = value; OnPropertyChanged("IdClasificacionCriminologica"); }
        }
        private string _IntimidacionAntePenaImpuesta;
        public string IntimidacionAntePenaImpuesta
        {
            get { return _IntimidacionAntePenaImpuesta; }
            set { _IntimidacionAntePenaImpuesta = value; OnPropertyChanged("IntimidacionAntePenaImpuesta"); }
        }

        private string _PorqueIntimidacionAntePenaImpuesta;
        public string PorqueIntimidacionAntePenaImpuesta
        {
            get { return _PorqueIntimidacionAntePenaImpuesta; }
            set { _PorqueIntimidacionAntePenaImpuesta = value; OnPropertyChanged("PorqueIntimidacionAntePenaImpuesta"); }
        }

        private short? _IdCapacidadCriminologicaActual;
        public short? IdCapacidadCriminologicaActual
        {
            get { return _IdCapacidadCriminologicaActual; }
            set { _IdCapacidadCriminologicaActual = value; OnPropertyChanged("IdCapacidadCriminologicaActual"); }
        }

        private short? _IdEgocentrismo;
        public short? IdEgocentrismo
        {
            get { return _IdEgocentrismo; }
            set { _IdEgocentrismo = value; OnPropertyChanged("IdEgocentrismo"); }
        }

        private short? _IdLabilidadAfectiva;
        public short? IdLabilidadAfectiva
        {
            get { return _IdLabilidadAfectiva; }
            set { _IdLabilidadAfectiva = value; OnPropertyChanged("IdLabilidadAfectiva"); }
        }

        private short? _IdAgresividad;
        public short? IdAgresividad
        {
            get { return _IdAgresividad; }
            set { _IdAgresividad = value; OnPropertyChanged("IdAgresividad"); }
        }

        private short? _IdIndiferenciaAfectiva;
        public short? IdIndiferenciaAfectiva
        {
            get { return _IdIndiferenciaAfectiva; }
            set { _IdIndiferenciaAfectiva = value; OnPropertyChanged("IdIndiferenciaAfectiva"); }
        }

        private short? _IdAdaptabilidadSocial;
        public short? IdAdaptabilidadSocial
        {
            get { return _IdAdaptabilidadSocial; }
            set { _IdAdaptabilidadSocial = value; OnPropertyChanged("IdAdaptabilidadSocial"); }
        }

        private short? _IdIndicePeligrosidadCriminologicaActual;
        public short? IdIndicePeligrosidadCriminologicaActual
        {
            get { return _IdIndicePeligrosidadCriminologicaActual; }
            set { _IdIndicePeligrosidadCriminologicaActual = value; OnPropertyChanged("IdIndicePeligrosidadCriminologicaActual"); }
        }

        private string _ProgramasActividadesRemitirInterno;
        public string ProgramasActividadesRemitirInterno
        {
            get { return _ProgramasActividadesRemitirInterno; }
            set { _ProgramasActividadesRemitirInterno = value; OnPropertyChanged("ProgramasActividadesRemitirInterno"); }
        }

        #endregion

        #region Estudio SocioFamiliar
        private DateTime? _FechaEstudioSocioFamiliarComun;
        public DateTime? FechaEstudioSocioFamiliarComun
        {
            get { return _FechaEstudioSocioFamiliarComun; }
            set { _FechaEstudioSocioFamiliarComun = value; OnPropertyChanged("FechaEstudioSocioFamiliarComun"); }
        }

        private string _FechaYLugarNacimiento;
        public string FechaYLugarNacimiento
        {
            get { return _FechaYLugarNacimiento; }
            set { _FechaYLugarNacimiento = value; OnPropertyChanged("FechaYLugarNacimiento"); }
        }

        private string _NombreImputadoSocioComun;
        public string NombreImputadoSocioComun
        {
            get { return _NombreImputadoSocioComun; }
            set { _NombreImputadoSocioComun = value; OnPropertyChanged("NombreImputadoSocioComun"); }
        }

        private string _DomicilioAntesIngresarCentro;
        public string DomicilioAntesIngresarCentro
        {
            get { return _DomicilioAntesIngresarCentro; }
            set { _DomicilioAntesIngresarCentro = value; OnPropertyChanged("DomicilioAntesIngresarCentro"); }
        }

        private string _EstadoCivilSocioComun;
        public string EstadoCivilSocioComun
        {
            get { return _EstadoCivilSocioComun; }
            set { _EstadoCivilSocioComun = value; OnPropertyChanged("EstadoCivilSocioComun"); }
        }

        private string _Telefono;
        public string Telefono
        {
            get { return _Telefono; }
            set { _Telefono = value; OnPropertyChanged("Telefono"); }
        }

        private short? _IdDictamenSocioFamComun;
        public short? IdDictamenSocioFamComun
        {
            get { return _IdDictamenSocioFamComun; }
            set { _IdDictamenSocioFamComun = value; OnPropertyChanged("IdDictamenSocioFamComun"); }
        }

        private string _FamiliaPrimaria;
        public string FamiliaPrimaria
        {
            get { return _FamiliaPrimaria; }
            set { _FamiliaPrimaria = value; OnPropertyChanged("FamiliaPrimaria"); }
        }


        #region Padron Visitas Autorizadas
        private List<VISITA_AUTORIZADA> _ListaVisitasAut;
        public List<VISITA_AUTORIZADA> ListaVisitasAut
        {
            get { return _ListaVisitasAut; }
            set { _ListaVisitasAut = value; OnPropertyChanged("ListaVisitasAut"); }
        }

        private ObservableCollection<PFC_VI_COMUNICACION> _LstComunicaciones;
        public ObservableCollection<PFC_VI_COMUNICACION> LstComunicaciones
        {
            get { return _LstComunicaciones; }
            set { _LstComunicaciones = value; OnPropertyChanged("LstComunicaciones"); }
        }

        private PFC_VI_COMUNICACION _SelectedComunicacionComun;

        public PFC_VI_COMUNICACION SelectedComunicacionComun
        {
            get { return _SelectedComunicacionComun; }
            set { _SelectedComunicacionComun = value; OnPropertyChanged("SelectedComunicacionComun"); }
        }


        private ObservableCollection<VISITA_AUTORIZADA> _LstVisitantesPorInterno;

        public ObservableCollection<VISITA_AUTORIZADA> LstVisitantesPorInterno
        {
            get { return _LstVisitantesPorInterno; }
            set { _LstVisitantesPorInterno = value; OnPropertyChanged("LstVisitantesPorInterno"); }
        }

        private VISITA_AUTORIZADA _SelectedVisitanteInterno;
        public VISITA_AUTORIZADA SelectedVisitanteInterno
        {
            get { return _SelectedVisitanteInterno; }
            set { _SelectedVisitanteInterno = value; OnPropertyChanged("SelectedVisitanteInterno"); }
        }

        private short _ActualProcesoSeleccionVisitante { get; set; }

        private string _FrecuenciaFamiliar;
        public string FrecuenciaFamiliar
        {
            get { return _FrecuenciaFamiliar; }
            set { _FrecuenciaFamiliar = value; OnPropertyChanged("FrecuenciaFamiliar"); }
        }


        #endregion
        private string _FamiliaSecundaria;
        public string FamiliaSecundaria
        {
            get { return _FamiliaSecundaria; }
            set { _FamiliaSecundaria = value; OnPropertyChanged("FamiliaSecundaria"); }
        }

        private string _IsAdultoMayorParticipoEnProgramaEspecial;
        public string IsAdultoMayorParticipoEnProgramaEspecial
        {
            get { return _IsAdultoMayorParticipoEnProgramaEspecial; }
            set { _IsAdultoMayorParticipoEnProgramaEspecial = value; OnPropertyChanged("IsAdultoMayorParticipoEnProgramaEspecial"); }
        }

        private bool _Enabled1 = false;

        public bool Enabled1
        {
            get { return _Enabled1; }
            set { _Enabled1 = value; OnPropertyChanged("Enabled1"); }
        }
        private bool _Enabled2 = false;

        public bool Enabled2
        {
            get { return _Enabled2; }
            set { _Enabled2 = value; OnPropertyChanged("Enabled2"); }
        }
        private string _IdRecibeVisitaSocioFamComun;
        public string IdRecibeVisitaSocioFamComun
        {
            get { return _IdRecibeVisitaSocioFamComun; }
            set
            {
                _IdRecibeVisitaSocioFamComun = value;

                if (string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("EspecificarQuienVisita");
                    base.RemoveRule("FrecuenciaVisita");
                    base.RemoveRule("RazonNoRecibeVisitas");

                    OnPropertyChanged("RazonNoRecibeVisitas");
                    OnPropertyChanged("EspecificarQuienVisita");
                    OnPropertyChanged("FrecuenciaVisita");
                }
                else
                {
                    if (value == "N")
                    {
                        Enabled1 = true;
                        IsEnabledSocUno = false;
                        Enabled2 = false;
                        IsEnabledSocDos = true;
                        base.RemoveRule("RazonNoRecibeVisitas");
                        base.RemoveRule("EspecificarQuienVisita");
                        base.RemoveRule("FrecuenciaVisita");
                        base.AddRule(() => RazonNoRecibeVisitas, () => !string.IsNullOrEmpty(RazonNoRecibeVisitas), "ESPECIFIQUE RAZON NO VISITA ES REQUERIDO!");
                        OnPropertyChanged("RazonNoRecibeVisitas");
                        OnPropertyChanged("EspecificarQuienVisita");
                        OnPropertyChanged("FrecuenciaVisita");
                        OnPropertyChanged("Enabled1");
                        OnPropertyChanged("Enabled2");
                        OnPropertyChanged("IsEnabledSocUno");
                        OnPropertyChanged("IsEnabledSocDos");
                    }

                    if (value == "S")
                    {
                        IsEnabledSocDos = false;
                        Enabled1 = false;
                        Enabled2 = true;
                        IsEnabledSocUno = true;
                        base.RemoveRule("RazonNoRecibeVisitas");
                        base.RemoveRule("EspecificarQuienVisita");
                        base.RemoveRule("FrecuenciaVisita");
                        base.AddRule(() => EspecificarQuienVisita, () => !string.IsNullOrEmpty(EspecificarQuienVisita), "ESPECIFIQUE QUIEN VISITA ES REQUERIDO!");
                        base.AddRule(() => FrecuenciaVisita, () => !string.IsNullOrEmpty(FrecuenciaVisita), "ESPECIFIQUE FRECUENCIA ES REQUERIDO!");
                        OnPropertyChanged("EspecificarQuienVisita");
                        OnPropertyChanged("FrecuenciaVisita");
                        OnPropertyChanged("RazonNoRecibeVisitas");
                        OnPropertyChanged("Enabled1");
                        OnPropertyChanged("IsEnabledSocUno");
                        OnPropertyChanged("Enabled2");
                        OnPropertyChanged("IsEnabledSocDos");
                    }
                }

                OnPropertyChanged("IdRecibeVisitaSocioFamComun");
            }
        }

        private bool _IsEnabledSocUno = true;

        public bool IsEnabledSocUno
        {
            get { return _IsEnabledSocUno; }
            set
            {
                _IsEnabledSocUno = value;
                if (value)
                {
                    base.AddRule(() => EspecificarQuienVisita, () => !string.IsNullOrEmpty(EspecificarQuienVisita), "ESPECIFIQUE QUIEN VISITA ES REQUERIDO!");
                    base.AddRule(() => FrecuenciaVisita, () => !string.IsNullOrEmpty(FrecuenciaVisita), "ESPECIFIQUE FRECUENCIA ES REQUERIDO!");
                    OnPropertyChanged("EspecificarQuienVisita");
                    OnPropertyChanged("FrecuenciaVisita");
                }

                else
                {
                    base.RemoveRule("EspecificarQuienVisita");
                    base.RemoveRule("FrecuenciaVisita");
                    OnPropertyChanged("EspecificarQuienVisita");
                    OnPropertyChanged("FrecuenciaVisita");
                }

                OnPropertyChanged("IsEnabledSocUno");
            }
        }

        private bool _IsEnabledSocDos = true;
        public bool IsEnabledSocDos
        {
            get { return _IsEnabledSocDos; }
            set
            {
                _IsEnabledSocDos = value;
                if (value)
                {
                    base.RemoveRule("EspecificarQuienVisita");
                    base.RemoveRule("FrecuenciaVisita");
                    base.AddRule(() => RazonNoRecibeVisitas, () => !string.IsNullOrEmpty(RazonNoRecibeVisitas), "ESPECIFIQUE RAZON NO VISITA ES REQUERIDO!");
                    OnPropertyChanged("RazonNoRecibeVisitas");
                    OnPropertyChanged("EspecificarQuienVisita");
                    OnPropertyChanged("FrecuenciaVisita");
                }

                else
                {
                    base.RemoveRule("RazonNoRecibeVisitas");
                    OnPropertyChanged("RazonNoRecibeVisitas");
                }

                OnPropertyChanged("IsEnabledSocDos");
            }
        }

        private ObservableCollection<PFC_VI_GRUPO> _ListGruposSocioFamComun;
        public ObservableCollection<PFC_VI_GRUPO> ListGruposSocioFamComun
        {
            get { return _ListGruposSocioFamComun; }
            set { _ListGruposSocioFamComun = value; OnPropertyChanged("ListGruposSocioFamComun"); }
        }


        private ObservableCollection<PFC_VI_GRUPO> _ListFortalecimientoSocioFamComun;
        public ObservableCollection<PFC_VI_GRUPO> ListFortalecimientoSocioFamComun
        {
            get { return _ListFortalecimientoSocioFamComun; }
            set { _ListFortalecimientoSocioFamComun = value; OnPropertyChanged("ListFortalecimientoSocioFamComun"); }
        }

        private PFC_VI_GRUPO _SelectedFortGrupo;
        public PFC_VI_GRUPO SelectedFortGrupo
        {
            get { return _SelectedFortGrupo; }
            set { _SelectedFortGrupo = value; OnPropertyChanged("SelectedFortGrupo"); }
        }

        private PFC_VI_GRUPO _SelectedGrupoSocioEconomico;
        public PFC_VI_GRUPO SelectedGrupoSocioEconomico
        {
            get { return _SelectedGrupoSocioEconomico; }
            set { _SelectedGrupoSocioEconomico = value; OnPropertyChanged("SelectedGrupoSocioEconomico"); }
        }

        private string _CongregSocFC;
        public string CongregSocFC
        {
            get { return _CongregSocFC; }
            set { _CongregSocFC = value; OnPropertyChanged("CongregSocFC"); }
        }

        private string _PeriodoSocFC;
        public string PeriodoSocFC
        {
            get { return _PeriodoSocFC; }
            set { _PeriodoSocFC = value; OnPropertyChanged("PeriodoSocFC"); }
        }

        private string _ObservacionesSocFC;
        public string ObservacionesSocFC
        {
            get { return _ObservacionesSocFC; }
            set { _ObservacionesSocFC = value; OnPropertyChanged("ObservacionesSocFC"); }
        }

        private short IsCongre { get; set; }
        private bool AgregarCongregacion { get; set; }
        private bool AgregarFortNucleoFamiliar { get; set; }
        private System.Windows.Visibility _IsEnabledC = System.Windows.Visibility.Visible;
        public System.Windows.Visibility IsEnabledC
        {
            get { return _IsEnabledC; }
            set { _IsEnabledC = value; OnPropertyChanged("IsEnabledC"); }
        }

        private bool _IsPadreVisitaCheckd = false;
        public bool IsPadreVisitaCheckd
        {
            get { return _IsPadreVisitaCheckd; }
            set { _IsPadreVisitaCheckd = value; OnPropertyChanged("IsPadreVisitaCheckd"); }
        }

        private bool _IsMadreVisitaChecked = false;
        public bool IsMadreVisitaChecked
        {
            get { return _IsMadreVisitaChecked; }
            set { _IsMadreVisitaChecked = value; OnPropertyChanged("IsMadreVisitaChecked"); }
        }

        private bool _IsEsposoConcubinaChecked = false;
        public bool IsEsposoConcubinaChecked
        {
            get { return _IsEsposoConcubinaChecked; }
            set { _IsEsposoConcubinaChecked = value; OnPropertyChanged("IsEsposoConcubinaChecked"); }
        }

        private bool _IsHermanosChecked = false;
        public bool IsHermanosChecked
        {
            get { return _IsHermanosChecked; }
            set { _IsHermanosChecked = value; OnPropertyChanged("IsHermanosChecked"); }
        }

        private bool _IsHijosChecked = false;
        public bool IsHijosChecked
        {
            get { return _IsHijosChecked; }
            set { _IsHijosChecked = value; OnPropertyChanged("IsHijosChecked"); }
        }

        private bool _IsOtrosVisitaChecked = false;
        public bool IsOtrosVisitaChecked
        {
            get { return _IsOtrosVisitaChecked; }
            set { _IsOtrosVisitaChecked = value; OnPropertyChanged("IsOtrosVisitaChecked"); }
        }

        private string _EspecificarQuienVisita;
        public string EspecificarQuienVisita
        {
            get { return _EspecificarQuienVisita; }
            set { _EspecificarQuienVisita = value; OnPropertyChanged("EspecificarQuienVisita"); }
        }

        private string _FrecuenciaVisita;
        public string FrecuenciaVisita
        {
            get { return _FrecuenciaVisita; }
            set { _FrecuenciaVisita = value; OnPropertyChanged("FrecuenciaVisita"); }
        }

        private string _RazonNoRecibeVisitas;
        public string RazonNoRecibeVisitas
        {
            get { return _RazonNoRecibeVisitas; }
            set { _RazonNoRecibeVisitas = value; OnPropertyChanged("RazonNoRecibeVisitas"); }
        }

        private string _IdComunicacionViaTelChecked;
        public string IdComunicacionViaTelChecked
        {
            get { return _IdComunicacionViaTelChecked; }
            set
            {
                _IdComunicacionViaTelChecked = value;
                if (!string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("EspecifiqueViaTelefonica");
                    IsEnabledEspecifiqueComunicacionTel = true;
                    base.AddRule(() => EspecifiqueViaTelefonica, () => !string.IsNullOrEmpty(EspecifiqueViaTelefonica), "ESPECIFIQUE COMUNICACION TELEFONICA ES REQUERIDO!");
                    OnPropertyChanged("EspecifiqueViaTelefonica");
                }

                else
                {
                    IsEnabledEspecifiqueComunicacionTel = false;
                    base.RemoveRule("EspecifiqueViaTelefonica");
                    OnPropertyChanged("EspecifiqueViaTelefonica");
                }

                OnPropertyChanged("EspecifiqueViaTelefonica");
                OnPropertyChanged("IsEnabledEspecifiqueComunicacionTel");
                OnPropertyChanged("IdComunicacionViaTelChecked");
            }
        }

        private string _EspecifiqueViaTelefonica;
        public string EspecifiqueViaTelefonica
        {
            get { return _EspecifiqueViaTelefonica; }
            set { _EspecifiqueViaTelefonica = value; OnPropertyChanged("EspecifiqueViaTelefonica"); }
        }

        private string _ApoyosRecibeExterior;
        public string ApoyosRecibeExterior
        {
            get { return _ApoyosRecibeExterior; }
            set { _ApoyosRecibeExterior = value; OnPropertyChanged("ApoyosRecibeExterior"); }
        }

        private string _PlanesInternoAlSerExternado;
        public string PlanesInternoAlSerExternado
        {
            get { return _PlanesInternoAlSerExternado; }
            set { _PlanesInternoAlSerExternado = value; OnPropertyChanged("PlanesInternoAlSerExternado"); }
        }

        private string _ConQuienVivirSerExternado;
        public string ConQuienVivirSerExternado
        {
            get { return _ConQuienVivirSerExternado; }
            set { _ConQuienVivirSerExternado = value; OnPropertyChanged("ConQuienVivirSerExternado"); }
        }

        private string _IdOfertaTrabajoChecked;
        public string IdOfertaTrabajoChecked
        {
            get { return _IdOfertaTrabajoChecked; }
            set
            {
                _IdOfertaTrabajoChecked = value;
                if (!string.IsNullOrEmpty(value) && value == "S")
                {
                    IsEnabledOfertaTrabajo = true;
                    base.AddRule(() => EspecifiqueOfertaTrabajo, () => !string.IsNullOrEmpty(EspecifiqueOfertaTrabajo), "ESPECIFIQUE OFERTA DE TRABAJO ES REQUERIDO!");
                }

                else
                {
                    IsEnabledOfertaTrabajo = false;
                    base.RemoveRule("EspecifiqueOfertaTrabajo");
                }

                OnPropertyChanged("EspecifiqueOfertaTrabajo");
                OnPropertyChanged("IsEnabledOfertaTrabajo");
                OnPropertyChanged("IdOfertaTrabajoChecked");
            }
        }

        private bool _IsEnabledOfertaTrabajo = false;
        public bool IsEnabledOfertaTrabajo
        {
            get { return _IsEnabledOfertaTrabajo; }
            set { _IsEnabledOfertaTrabajo = value; OnPropertyChanged("IsEnabledOfertaTrabajo"); }
        }

        private string _IdAvalMoralChecked;
        public string IdAvalMoralChecked
        {
            get { return _IdAvalMoralChecked; }
            set
            {
                _IdAvalMoralChecked = value;
                if (!string.IsNullOrEmpty(value) && value == "S")
                {
                    IsEnabledAvalMoral = true;
                    base.AddRule(() => EspecifiqueAvalMoral, () => !string.IsNullOrEmpty(EspecifiqueAvalMoral), "ESPECIFIQUE AVAL MORAL ES REQUERIDO!");
                }

                else
                {
                    IsEnabledAvalMoral = false;
                    base.RemoveRule("EspecifiqueAvalMoral");
                }

                OnPropertyChanged("EspecifiqueAvalMoral");
                OnPropertyChanged("IdAvalMoralChecked");
            }
        }


        private bool _IsEnabledAvalMoral = false;
        public bool IsEnabledAvalMoral
        {
            get { return _IsEnabledAvalMoral; }
            set { _IsEnabledAvalMoral = value; OnPropertyChanged("IsEnabledAvalMoral"); }
        }

        private string _EspecifiqueAvalMoral;
        public string EspecifiqueAvalMoral
        {
            get { return _EspecifiqueAvalMoral; }
            set { _EspecifiqueAvalMoral = value; OnPropertyChanged("EspecifiqueAvalMoral"); }
        }

        private short? _IdDictamenSocioSocioFamComun;
        public short? IdDictamenSocioSocioFamComun
        {
            get { return _IdDictamenSocioSocioFamComun; }
            set { _IdDictamenSocioSocioFamComun = value; OnPropertyChanged("IdDictamenSocioSocioFamComun"); }
        }

        private string _MotivacionDictamenSocioEconomicoComun;
        public string MotivacionDictamenSocioEconomicoComun
        {
            get { return _MotivacionDictamenSocioEconomicoComun; }
            set { _MotivacionDictamenSocioEconomicoComun = value; OnPropertyChanged("MotivacionDictamenSocioEconomicoComun"); }
        }

        private DateTime? _FecRealizacionEstudioSocioFamiliar;
        public DateTime? FecRealizacionEstudioSocioFamiliar
        {
            get { return _FecRealizacionEstudioSocioFamiliar; }
            set { _FecRealizacionEstudioSocioFamiliar = value; OnPropertyChanged("FecRealizacionEstudioSocioFamiliar"); }
        }

        private bool _IsEnabledEspecifiqueComunicacionTel = false;
        public bool IsEnabledEspecifiqueComunicacionTel
        {
            get { return _IsEnabledEspecifiqueComunicacionTel; }
            set { _IsEnabledEspecifiqueComunicacionTel = value; OnPropertyChanged("IsEnabledEspecifiqueComunicacionTel"); }
        }

        #endregion

        #region Estudio Educativo, Cultural y deportivo
        ///agregar listas de grupos, datos del dictamen ya estan en variables genericas
        ///

        #region Datos Educativos
        private short _IdEducativo;
        public short IdEducativo
        {
            get { return _IdEducativo; }
            set { _IdEducativo = value; OnPropertyChanged("IdEducativo"); }
        }

        private string _Concluida;
        public string Concluida
        {
            get { return _Concluida; }
            set { _Concluida = value; OnPropertyChanged("Concluida"); }
        }

        private string _ObservacionesEducacion;
        public string ObservacionesEducacion
        {
            get { return _ObservacionesEducacion; }
            set { _ObservacionesEducacion = value; OnPropertyChanged("ObservacionesEducacion"); }
        }

        private ObservableCollection<EDUCACION_GRADO> lstEscolaridad;
        public ObservableCollection<EDUCACION_GRADO> LstEscolaridad
        {
            get { return lstEscolaridad; }
            set { lstEscolaridad = value; OnPropertyChanged("LstEscolaridad"); }
        }

        private ObservableCollection<PFC_VII_ESCOLARIDAD_ANTERIOR> lstEscolaridadesEducativo;
        public ObservableCollection<PFC_VII_ESCOLARIDAD_ANTERIOR> LstEscolaridadesEducativo
        {
            get { return lstEscolaridadesEducativo; }
            set { lstEscolaridadesEducativo = value; OnPropertyChanged("LstEscolaridadesEducativo"); }
        }

        private PFC_VII_ESCOLARIDAD_ANTERIOR _SelectedComunicacion;
        public PFC_VII_ESCOLARIDAD_ANTERIOR SelectedComunicacion
        {
            get { return _SelectedComunicacion; }
            set { _SelectedComunicacion = value; OnPropertyChanged("SelectedComunicacion"); }
        }

        private PFC_VII_ESCOLARIDAD_ANTERIOR _SelectedActividadEducativa;
        public PFC_VII_ESCOLARIDAD_ANTERIOR SelectedActividadEducativa
        {
            get { return _SelectedActividadEducativa; }
            set { _SelectedActividadEducativa = value; OnPropertyChanged("SelectedActividadEducativa"); }
        }

        private bool _EnabledActivCultComun = true;
        public bool EnabledActivCultComun
        {
            get { return _EnabledActivCultComun; }
            set { _EnabledActivCultComun = value; OnPropertyChanged("EnabledActivCultComun"); }
        }

        private short _IdNivelEducativoActiv;
        public short IdNivelEducativoActiv
        {
            get { return _IdNivelEducativoActiv; }
            set { _IdNivelEducativoActiv = value; OnPropertyChanged("IdNivelEducativoActiv"); }
        }

        private string _IdConcluidaActivEducativa;
        public string IdConcluidaActivEducativa
        {
            get { return _IdConcluidaActivEducativa; }
            set { _IdConcluidaActivEducativa = value; OnPropertyChanged("IdConcluidaActivEducativa"); }
        }

        private string _IdRendimientoActivEducativo;
        public string IdRendimientoActivEducativo
        {
            get { return _IdRendimientoActivEducativo; }
            set { _IdRendimientoActivEducativo = value; OnPropertyChanged("IdRendimientoActivEducativo"); }
        }

        private string _IdInteresActivEducativo;
        public string IdInteresActivEducativo
        {
            get { return _IdInteresActivEducativo; }
            set { _IdInteresActivEducativo = value; OnPropertyChanged("IdInteresActivEducativo"); }
        }

        private string _ObservacionesActivEducativa;
        public string ObservacionesActivEducativa
        {
            get { return _ObservacionesActivEducativa; }
            set { _ObservacionesActivEducativa = value; OnPropertyChanged("ObservacionesActivEducativa"); }
        }

        private short ProcesoActual { get; set; }
        private ObservableCollection<PFC_VII_ESCOLARIDAD_ANTERIOR> lstActividadesEducativas;
        public ObservableCollection<PFC_VII_ESCOLARIDAD_ANTERIOR> LstActividadesEducativas
        {
            get { return lstActividadesEducativas; }
            set { lstActividadesEducativas = value; OnPropertyChanged("LstActividadesEducativas"); }
        }

        private ObservableCollection<PFC_VII_ACTIVIDAD> lstAcitividadesCulturales;
        public ObservableCollection<PFC_VII_ACTIVIDAD> LstAcitividadesCulturales
        {
            get { return lstAcitividadesCulturales; }
            set { lstAcitividadesCulturales = value; OnPropertyChanged("LstAcitividadesCulturales"); }
        }

        private ObservableCollection<PFC_VII_ACTIVIDAD> lstActividadesDeportivas;
        public ObservableCollection<PFC_VII_ACTIVIDAD> LstActividadesDeportivas
        {
            get { return lstActividadesDeportivas; }
            set { lstActividadesDeportivas = value; OnPropertyChanged("LstActividadesDeportivas"); }
        }

        #endregion
        #region Datos Culturales y Deportivos
        private ObservableCollection<ACTIVIDAD> lstProgramasActiv;
        public ObservableCollection<ACTIVIDAD> LstProgramasActiv
        {
            get { return lstProgramasActiv; }
            set { lstProgramasActiv = value; OnPropertyChanged("LstProgramasActiv"); }
        }


        private PFC_VII_ACTIVIDAD selectedActivCultural;
        public PFC_VII_ACTIVIDAD SelectedActivCultural
        {
            get { return selectedActivCultural; }
            set { selectedActivCultural = value; OnPropertyChanged("SelectedActivCultural"); }
        }

        private PFC_VII_ACTIVIDAD selectedActividadDeportiva;

        public PFC_VII_ACTIVIDAD SelectedActividadDeportiva
        {
            get { return selectedActividadDeportiva; }
            set { selectedActividadDeportiva = value; OnPropertyChanged("SelectedActividadDeportiva"); }
        }

        private short? _SelectedPrograma;
        public short? SelectedPrograma
        {
            get { return _SelectedPrograma; }
            set { _SelectedPrograma = value; OnPropertyChanged("SelectedPrograma"); }
        }

        private string _DescripcionActividad;
        public string DescripcionActividad
        {
            get { return _DescripcionActividad; }
            set { _DescripcionActividad = value; OnPropertyChanged("DescripcionActividad"); }
        }

        private string _DescripcionDuracion;
        public string DescripcionDuracion
        {
            get { return _DescripcionDuracion; }
            set { _DescripcionDuracion = value; OnPropertyChanged("DescripcionDuracion"); }
        }

        private string _DescripcionObservacionesActiv;
        public string DescripcionObservacionesActiv
        {
            get { return _DescripcionObservacionesActiv; }
            set { _DescripcionObservacionesActiv = value; OnPropertyChanged("DescripcionObservacionesActiv"); }
        }



        #endregion

        private short? _IdDictamenEducativoComun;
        public short? IdDictamenEducativoComun
        {
            get { return _IdDictamenEducativoComun; }
            set { _IdDictamenEducativoComun = value; OnPropertyChanged("IdDictamenEducativoComun"); }
        }

        private string _MotivacionDictamenEducativoComun;
        public string MotivacionDictamenEducativoComun
        {
            get { return _MotivacionDictamenEducativoComun; }
            set { _MotivacionDictamenEducativoComun = value; OnPropertyChanged("MotivacionDictamenEducativoComun"); }
        }

        private DateTime? _FechaEstudioEducativoComun;
        public DateTime? FechaEstudioEducativoComun
        {
            get { return _FechaEstudioEducativoComun; }
            set { _FechaEstudioEducativoComun = value; OnPropertyChanged("FechaEstudioEducativoComun"); }
        }

        #endregion

        #region Estudio Sobre Capacitacion y Trabajo Penitenciario

        private ObservableCollection<ACTIVIDAD> lstCapacitacion;
        public ObservableCollection<ACTIVIDAD> LstCapacitacion
        {
            get { return lstCapacitacion; }
            set { lstCapacitacion = value; OnPropertyChanged("LstCapacitacion"); }
        }

        private short? _IdCapac;
        public short? IdCapac
        {
            get { return _IdCapac; }
            set { _IdCapac = value; OnPropertyChanged("IdCapac"); }
        }

        private PFC_VIII_CAPACITACION _SelectedCapac;
        public PFC_VIII_CAPACITACION SelectedCapac
        {
            get { return _SelectedCapac; }
            set { _SelectedCapac = value; OnPropertyChanged("SelectedCapac"); }
        }

        private ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL> lstCapacitacionLaboral;
        public ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL> LstCapacitacionLaboral
        {
            get { return lstCapacitacionLaboral; }
            set { lstCapacitacionLaboral = value; OnPropertyChanged("LstCapacitacionLaboral"); }
        }

        private PFC_VIII_ACTIVIDAD_LABORAL _SelectedCapacitacionLaboral;
        public PFC_VIII_ACTIVIDAD_LABORAL SelectedCapacitacionLaboral
        {
            get { return _SelectedCapacitacionLaboral; }
            set { _SelectedCapacitacionLaboral = value; OnPropertyChanged("SelectedCapacitacionLaboral"); }
        }

        private ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL> lstActivNoGratificadas;
        public ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL> LstActivNoGratificadas
        {
            get { return lstActivNoGratificadas; }
            set { lstActivNoGratificadas = value; OnPropertyChanged("LstActivNoGratificadas"); }
        }

        private PFC_VIII_ACTIVIDAD_LABORAL _SelectedActivNoGratificada;
        public PFC_VIII_ACTIVIDAD_LABORAL SelectedActivNoGratificada
        {
            get { return _SelectedActivNoGratificada; }
            set { _SelectedActivNoGratificada = value; OnPropertyChanged("SelectedActivNoGratificada"); }
        }

        private ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL> lstActivGratificadas;
        public ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL> LstActivGratificadas
        {
            get { return lstActivGratificadas; }
            set { lstActivGratificadas = value; OnPropertyChanged("LstActivGratificadas"); }
        }

        private PFC_VIII_ACTIVIDAD_LABORAL _SelectedActivGratificada;
        public PFC_VIII_ACTIVIDAD_LABORAL SelectedActivGratificada
        {
            get { return _SelectedActivGratificada; }
            set { _SelectedActivGratificada = value; OnPropertyChanged("SelectedActivGratificada"); }
        }

        private string _ActividadOficioAntesReclusion;
        public string ActividadOficioAntesReclusion
        {
            get { return _ActividadOficioAntesReclusion; }
            set { _ActividadOficioAntesReclusion = value; OnPropertyChanged("ActividadOficioAntesReclusion"); }
        }

        private string _IdResponsabilidad;
        public string IdResponsabilidad
        {
            get { return _IdResponsabilidad; }
            set { _IdResponsabilidad = value; OnPropertyChanged("IdResponsabilidad"); }
        }

        private string _IdCalidadTrabajo;
        public string IdCalidadTrabajo
        {
            get { return _IdCalidadTrabajo; }
            set { _IdCalidadTrabajo = value; OnPropertyChanged("IdCalidadTrabajo"); }
        }

        private string _IdPerseverancia;
        public string IdPerseverancia
        {
            get { return _IdPerseverancia; }
            set { _IdPerseverancia = value; OnPropertyChanged("IdPerseverancia"); }
        }

        private string _IdCuentaConFondoAhorro;
        public string IdCuentaConFondoAhorro
        {
            get { return _IdCuentaConFondoAhorro; }
            set { _IdCuentaConFondoAhorro = value; OnPropertyChanged("IdCuentaConFondoAhorro"); }
        }

        private short? _DiasEfectivosLaboradosEnOtrosCentros;
        public short? DiasEfectivosLaboradosEnOtrosCentros
        {
            get { return _DiasEfectivosLaboradosEnOtrosCentros; }
            set
            {
                _DiasEfectivosLaboradosEnOtrosCentros = value;
                if (value.HasValue)
                    ValidacionDiasLaborados(DiasEfectivosLaboradosEnOtrosCentros, DiasEfectivosLaboradosEnCentroActual);

                OnPropertyChanged("DiasEfectivosLaboradosTotalDiasLaborados");
                OnPropertyChanged("DiasEfectivosLaboradosEnOtrosCentros");
            }
        }

        private short? _DiasEfectivosLaboradosEnCentroActual;
        public short? DiasEfectivosLaboradosEnCentroActual
        {
            get { return _DiasEfectivosLaboradosEnCentroActual; }
            set
            {
                _DiasEfectivosLaboradosEnCentroActual = value;
                if (value.HasValue)
                    ValidacionDiasLaborados(DiasEfectivosLaboradosEnOtrosCentros, DiasEfectivosLaboradosEnCentroActual);

                OnPropertyChanged("DiasEfectivosLaboradosTotalDiasLaborados");
                OnPropertyChanged("DiasEfectivosLaboradosEnCentroActual");
            }
        }

        private short? _DiasEfectivosLaboradosTotalDiasLaborados;
        public short? DiasEfectivosLaboradosTotalDiasLaborados
        {
            get { return _DiasEfectivosLaboradosTotalDiasLaborados; }
            set { _DiasEfectivosLaboradosTotalDiasLaborados = value; OnPropertyChanged("DiasEfectivosLaboradosTotalDiasLaborados"); }
        }

        private DateTime? _PeriodoDondeRealizoActividadLaboral;
        public DateTime? PeriodoDondeRealizoActividadLaboral
        {
            get { return _PeriodoDondeRealizoActividadLaboral; }
            set { _PeriodoDondeRealizoActividadLaboral = value; OnPropertyChanged("PeriodoDondeRealizoActividadLaboral"); }
        }


        private DateTime? _FechaSeguridadCustodiaDictamen;
        public DateTime? FechaSeguridadCustodiaDictamen
        {
            get { return _FechaSeguridadCustodiaDictamen; }
            set 
            {
                _FechaSeguridadCustodiaDictamen = value;
                OnPropertyChanged("FechaSeguridadCustodiaDictamen");
            }
        }

        private string _IdDicatmenSeguridadCustodiaDict;
        public string IdDicatmenSeguridadCustodiaDict
        {
            get { return _IdDicatmenSeguridadCustodiaDict; }
            set { _IdDicatmenSeguridadCustodiaDict = value; OnPropertyChanged("IdDicatmenSeguridadCustodiaDict"); }
        }

        private string _MotivacionDictamenSeguridadCustodia;
        public string MotivacionDictamenSeguridadCustodia
        {
            get { return _MotivacionDictamenSeguridadCustodia; }
            set { _MotivacionDictamenSeguridadCustodia = value; OnPropertyChanged("MotivacionDictamenSeguridadCustodia"); }
        }


        #region Area de Actividades y Capacitacion Laboral
        private string _NombreA;
        public string NombreA
        {
            get { return _NombreA; }
            set { _NombreA = value; OnPropertyChanged("NombreA"); }
        }

        private string _NombreDinamico2;

        public string NombreDinamico2
        {
            get { return _NombreDinamico2; }
            set { _NombreDinamico2 = value; OnPropertyChanged("NombreDinamico2"); }
        }

        private System.Windows.Visibility _IsEnabledConcluyo = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility IsEnabledConcluyo
        {
            get { return _IsEnabledConcluyo; }
            set { _IsEnabledConcluyo = value; OnPropertyChanged("IsEnabledConcluyo"); }
        }

        private string _DescripcionPeriodo;
        public string DescripcionPeriodo
        {
            get { return _DescripcionPeriodo; }
            set { _DescripcionPeriodo = value; OnPropertyChanged("DescripcionPeriodo"); }
        }

        private string _ConcluyoActiv;
        public string ConcluyoActiv
        {
            get { return _ConcluyoActiv; }
            set { _ConcluyoActiv = value; OnPropertyChanged("ConcluyoActiv"); }
        }

        private string _ObservacionesActiv;
        public string ObservacionesActiv
        {
            get { return _ObservacionesActiv; }
            set { _ObservacionesActiv = value; OnPropertyChanged("ObservacionesActiv"); }
        }

        private short ListaActual { get; set; }
        #endregion

        private enum eProcesosTipoP
        {
            CAPACITACION_LABORAL = 147
        };
        #endregion

        #region Informe de area de seguridad y custodia
        private string _IdConductaObservadaCentro;
        public string IdConductaObservadaCentro
        {
            get { return _IdConductaObservadaCentro; }
            set { _IdConductaObservadaCentro = value; OnPropertyChanged("IdConductaObservadaCentro"); }
        }

        private string _IdConductaAutoridad;
        public string IdConductaAutoridad
        {
            get { return _IdConductaAutoridad; }
            set { _IdConductaAutoridad = value; OnPropertyChanged("IdConductaAutoridad"); }
        }

        private short? _IdConductaGral;
        public short? IdConductaGral
        {
            get { return _IdConductaGral; }
            set { _IdConductaGral = value; OnPropertyChanged("IdConductaGral"); }
        }

        private short? _IdRelacionCompanieros;
        public short? IdRelacionCompanieros
        {
            get { return _IdRelacionCompanieros; }
            set { _IdRelacionCompanieros = value; OnPropertyChanged("IdRelacionCompanieros"); }
        }

        private string _IdRegistraCorrectivosDisciplinarios;
        public string IdRegistraCorrectivosDisciplinarios
        {
            get { return _IdRegistraCorrectivosDisciplinarios; }
            set { _IdRegistraCorrectivosDisciplinarios = value; OnPropertyChanged("IdRegistraCorrectivosDisciplinarios"); }
        }

        private string _IdOpinionGralConductaInterno;
        public string IdOpinionGralConductaInterno
        {
            get { return _IdOpinionGralConductaInterno; }
            set { _IdOpinionGralConductaInterno = value; OnPropertyChanged("IdOpinionGralConductaInterno"); }
        }

        private ObservableCollection<SANCION> lstSanciones;
        public ObservableCollection<SANCION> LstSanciones
        {
            get { return lstSanciones; }
            set { lstSanciones = value; OnPropertyChanged("LstSanciones"); }
        }

        private ObservableCollection<PFC_IX_CORRECTIVO> lstCorrectivosSeguridad;
        public ObservableCollection<PFC_IX_CORRECTIVO> LstCorrectivosSeguridad
        {
            get { return lstCorrectivosSeguridad; }
            set { lstCorrectivosSeguridad = value; OnPropertyChanged("LstCorrectivosSeguridad"); }
        }

        private PFC_IX_CORRECTIVO _SelectedSancionComun;

        public PFC_IX_CORRECTIVO SelectedSancionComun
        {
            get { return _SelectedSancionComun; }
            set { _SelectedSancionComun = value; OnPropertyChanged("SelectedSancionComun"); }
        }

        #endregion

        #region Estudio Personalidad Padre
        private PERSONALIDAD _EstudioPersonalidad { get; set; }
        private PERSONALIDAD_FUERO_COMUN _EstudioFueroComunPadre { get; set; }
        private bool _IsNuevoEstudioP { get; set; }
        #endregion


        private string _Arterial1;
        public string Arterial1
        {
            get { return _Arterial1; }
            set
            {
                _Arterial1 = value;
                if (!string.IsNullOrEmpty(value))
                    TextPresionArterial = value + "/" + Arterial2;

                OnPropertyChanged("Arterial1");
            }
        }

        private string _Arterial2;
        public string Arterial2
        {
            get { return _Arterial2; }
            set
            {
                _Arterial2 = value;
                if (!string.IsNullOrEmpty(value))
                    TextPresionArterial = Arterial1 + "/" + value;

                OnPropertyChanged("Arterial2");
            }
        }

        private string _TextPresionArterial;
        public string TextPresionArterial
        {
            get { return _TextPresionArterial; }
            set { _TextPresionArterial = value; OnPropertyChanged("TextPresionArterial"); }
        }
    }
}
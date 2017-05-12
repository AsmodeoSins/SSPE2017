using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Linq;
using System.Collections.ObjectModel;
namespace ControlPenales
{
    partial class RealizacionEstudiosViewModel
    {
        private bool bandera { get; set; }
        private string FueroImputado { get; set; }
        #region Signos Vitales
        private string _TensionArterialGenerico;
        public string TensionArterialGenerico
        {
            get { return _TensionArterialGenerico; }
            set { _TensionArterialGenerico = value; OnPropertyChanged("TensionArterialGenerico"); }
        }

        private string _TemperaturaGenerico;
        public string TemperaturaGenerico
        {
            get { return _TemperaturaGenerico; }
            set { _TemperaturaGenerico = value; OnPropertyChanged("TemperaturaGenerico"); }
        }

        private string _PulsoGenerico;
        public string PulsoGenerico
        {
            get { return _PulsoGenerico; }
            set { _PulsoGenerico = value; OnPropertyChanged("PulsoGenerico"); }
        }

        private string _RespiracionGenerico;
        public string RespiracionGenerico
        {
            get { return _RespiracionGenerico; }
            set { _RespiracionGenerico = value; OnPropertyChanged("RespiracionGenerico"); }
        }

        private string _PesoGenerico;
        public string PesoGenerico
        {
            get { return _PesoGenerico; }
            set { _PesoGenerico = value; OnPropertyChanged("PesoGenerico"); }
        }

        private string _EstaturaGenerico;
        public string EstaturaGenerico
        {
            get { return _EstaturaGenerico; }
            set { _EstaturaGenerico = value; OnPropertyChanged("EstaturaGenerico"); }
        }

        #endregion

        private DateTime? _FechaRealizacionEstudio;
        public DateTime? FechaRealizacionEstudio
        {
            get { return _FechaRealizacionEstudio; }
            set { _FechaRealizacionEstudio = value; OnPropertyChanged("FechaRealizacionEstudio"); }
        }

        private enum eRolesDepartamentoAccesoProcesoRealizacionEstudios
        {
            COORDINADOR_MEDICO = 5,
            COORDINADOR_PSICOLOGICO = 6
        };

        private string _MotivacionDictamem;
        public string MotivacionDictamem
        {
            get { return _MotivacionDictamem; }
            set { _MotivacionDictamem = value; OnPropertyChanged("MotivacionDictamem"); }
        }

        private PERSONALIDAD _Ultimo;

        public PERSONALIDAD Ultimo
        {
            get { return _Ultimo; }
            set { _Ultimo = value; OnPropertyChanged("Ultimo"); }
        }
        private short? _IdDictamenResultado;
        public short? IdDictamenResultado
        {
            get { return _IdDictamenResultado; }
            set { _IdDictamenResultado = value; OnPropertyChanged("IdDictamenResultado"); }
        }

        private DateTime? _FechaDictamenInformeSeguridadCustodia;
        public DateTime? FechaDictamenInformeSeguridadCustodia
        {
            get { return _FechaDictamenInformeSeguridadCustodia; }
            set { _FechaDictamenInformeSeguridadCustodia = value; OnPropertyChanged("FechaDictamenInformeSeguridadCustodia"); }
        }

        private string _MotivacionDictamenInformeSeguridadCustodia;
        public string MotivacionDictamenInformeSeguridadCustodia
        {
            get { return _MotivacionDictamenInformeSeguridadCustodia; }
            set { _MotivacionDictamenInformeSeguridadCustodia = value; OnPropertyChanged("MotivacionDictamenInformeSeguridadCustodia"); }
        }

        private string _IdDictamenInformeSeguridadCustodia;
        public string IdDictamenInformeSeguridadCustodia
        {
            get { return _IdDictamenInformeSeguridadCustodia; }
            set { _IdDictamenInformeSeguridadCustodia = value; OnPropertyChanged("IdDictamenInformeSeguridadCustodia"); }
        }

        private string _ImpresionDiagnostica;
        public string ImpresionDiagnostica
        {
            get { return _ImpresionDiagnostica; }
            set { _ImpresionDiagnostica = value; OnPropertyChanged("ImpresionDiagnostica"); }
        }

        private short? _IdRequiereTratExtramuros;
        public short? IdRequiereTratExtramuros
        {
            get { return _IdRequiereTratExtramuros; }
            set { _IdRequiereTratExtramuros = value; OnPropertyChanged("IdRequiereTratExtramuros"); }
        }

        private string _NombreTratamientoRemitir;
        public string NombreTratamientoRemitir
        {
            get { return _NombreTratamientoRemitir; }
            set { _NombreTratamientoRemitir = value; OnPropertyChanged("NombreTratamientoRemitir"); }
        }


        private string _NombreImputado;
        public string NombreImputado
        {
            get { return _NombreImputado; }
            set { _NombreImputado = value; OnPropertyChanged("NombreImputado"); }
        }

        private string _AliasImputado;
        public string AliasImputado
        {
            get { return _AliasImputado; }
            set { _AliasImputado = value; OnPropertyChanged("AliasImputado"); }
        }

        private string _DelitoImputado;
        public string DelitoImputado
        {
            get { return _DelitoImputado; }
            set { _DelitoImputado = value; OnPropertyChanged("DelitoImputado"); }
        }

        private string _AntecedentesPatologicos;
        public string AntecedentesPatologicos
        {
            get { return _AntecedentesPatologicos; }
            set { _AntecedentesPatologicos = value; OnPropertyChanged("AntecedentesPatologicos"); }
        }

        private string _PadecimientoActual;
        public string PadecimientoActual
        {
            get { return _PadecimientoActual; }
            set { _PadecimientoActual = value; OnPropertyChanged("PadecimientoActual"); }
        }

        private string _EstadoCivil;
        public string EstadoCivil
        {
            get { return _EstadoCivil; }
            set { _EstadoCivil = value; OnPropertyChanged("EstadoCivil"); }
        }

        private string _ExamenMental;
        public string ExamenMental
        {
            get { return _ExamenMental; }
            set { _ExamenMental = value; OnPropertyChanged("ExamenMental"); }
        }

        private string _EspecifiqueOfertaTrabajo;
        public string EspecifiqueOfertaTrabajo
        {
            get { return _EspecifiqueOfertaTrabajo; }
            set { _EspecifiqueOfertaTrabajo = value; OnPropertyChanged("EspecifiqueOfertaTrabajo"); }
        }

        private string _NombreInterno;
        public string NombreInterno
        {
            get { return _NombreInterno; }
            set { _NombreInterno = value; OnPropertyChanged("NombreInterno"); }
        }

        private short? _IdPronosticoReincidencia;
        public short? IdPronosticoReincidencia
        {
            get { return _IdPronosticoReincidencia; }
            set { _IdPronosticoReincidencia = value; OnPropertyChanged("IdPronosticoReincidencia"); }
        }
        private string _VersionDelitoSegunInterno;
        public string VersionDelitoSegunInterno
        {
            get { return _VersionDelitoSegunInterno; }
            set { _VersionDelitoSegunInterno = value; OnPropertyChanged("VersionDelitoSegunInterno"); }
        }


        private bool _IsEnablesFueroComunPrincipal = false;
        public bool IsEnablesFueroComunPrincipal
        {
            get { return _IsEnablesFueroComunPrincipal; }
            set { _IsEnablesFueroComunPrincipal = value; OnPropertyChanged("IsEnablesFueroComunPrincipal"); }
        }

        private bool _IsEnabledFueroFederalPrincipal = false;
        public bool IsEnabledFueroFederalPrincipal
        {
            get { return _IsEnabledFueroFederalPrincipal; }
            set { _IsEnabledFueroFederalPrincipal = value; OnPropertyChanged("IsEnabledFueroFederalPrincipal"); }
        }


        public PERSONALIDAD _UltimoEstudioPersonalidadConcluido { get; set; }
        #region Enabled en base a rol
        private bool _EnabledMedicoFueroComun = false;
        public bool EnabledMedicoFueroComun
        {
            get { return _EnabledMedicoFueroComun; }
            set { _EnabledMedicoFueroComun = value; OnPropertyChanged("EnabledMedicoFueroComun"); }
        }

        private bool _EnabledPsiquiatricoFueroComun = false;
        public bool EnabledPsiquiatricoFueroComun
        {
            get { return _EnabledPsiquiatricoFueroComun; }
            set { _EnabledPsiquiatricoFueroComun = value; OnPropertyChanged("EnabledPsiquiatricoFueroComun"); }
        }

        private bool _EnabledPsicologicoFueroComun = false;
        public bool EnabledPsicologicoFueroComun
        {
            get { return _EnabledPsicologicoFueroComun; }
            set { _EnabledPsicologicoFueroComun = value; OnPropertyChanged("EnabledPsicologicoFueroComun"); }
        }

        private bool _EnabledCriminodiagnosticoFueroComun = false;
        public bool EnabledCriminodiagnosticoFueroComun
        {
            get { return _EnabledCriminodiagnosticoFueroComun; }
            set { _EnabledCriminodiagnosticoFueroComun = value; OnPropertyChanged("EnabledCriminodiagnosticoFueroComun"); }
        }

        private bool _EnabledSocioFamiliarFueroComun = false;
        public bool EnabledSocioFamiliarFueroComun
        {
            get { return _EnabledSocioFamiliarFueroComun; }
            set { _EnabledSocioFamiliarFueroComun = value; OnPropertyChanged("EnabledSocioFamiliarFueroComun"); }
        }

        private bool _EnabledEducativoFueroComun = false;
        public bool EnabledEducativoFueroComun
        {
            get { return _EnabledEducativoFueroComun; }
            set { _EnabledEducativoFueroComun = value; OnPropertyChanged("EnabledEducativoFueroComun"); }
        }

        private bool _EnabledCapacitacionFueroComun = false;
        public bool EnabledCapacitacionFueroComun
        {
            get { return _EnabledCapacitacionFueroComun; }
            set { _EnabledCapacitacionFueroComun = value; OnPropertyChanged("EnabledCapacitacionFueroComun"); }
        }

        private bool _EnabledSeguridadFueroComun = false;
        public bool EnabledSeguridadFueroComun
        {
            get { return _EnabledSeguridadFueroComun; }
            set { _EnabledSeguridadFueroComun = value; OnPropertyChanged("EnabledSeguridadFueroComun"); }
        }



        private System.Collections.Generic.List<short> lstRoles { get; set; }

        #endregion
        #region Enabled en campos
        private bool _IsEnabledDatosMedicoComun = true;
        public bool IsEnabledDatosMedicoComun
        {
            get { return _IsEnabledDatosMedicoComun; }
            set { _IsEnabledDatosMedicoComun = value; OnPropertyChanged("IsEnabledDatosMedicoComun"); }
        }

        private bool _IdEnabledDatosPsiqComun = true;
        public bool IdEnabledDatosPsiqComun
        {
            get { return _IdEnabledDatosPsiqComun; }
            set { _IdEnabledDatosPsiqComun = value; OnPropertyChanged("IdEnabledDatosPsiqComun"); }
        }

        private bool _IsEnabledDatosPsicologoComun = true;
        public bool IsEnabledDatosPsicologoComun
        {
            get { return _IsEnabledDatosPsicologoComun; }
            set { _IsEnabledDatosPsicologoComun = value; OnPropertyChanged("IsEnabledDatosPsicologoComun"); }
        }

        private bool _IsEnabledDatosCriminod = true;

        public bool IsEnabledDatosCriminod
        {
            get { return _IsEnabledDatosCriminod; }
            set { _IsEnabledDatosCriminod = value; OnPropertyChanged("IsEnabledDatosCriminod"); }
        }

        private bool _IsEnabledDatosSocioFamiliar = true;
        public bool IsEnabledDatosSocioFamiliar
        {
            get { return _IsEnabledDatosSocioFamiliar; }
            set { _IsEnabledDatosSocioFamiliar = value; OnPropertyChanged("IsEnabledDatosSocioFamiliar"); }
        }

        private bool _IsEnabledTrabajoDatos = true;
        public bool IsEnabledTrabajoDatos
        {
            get { return _IsEnabledTrabajoDatos; }
            set { _IsEnabledTrabajoDatos = value; OnPropertyChanged("IsEnabledTrabajoDatos"); }
        }


        private bool _IsEnabledEducativoD = true;
        public bool IsEnabledEducativoD
        {
            get { return _IsEnabledEducativoD; }
            set { _IsEnabledEducativoD = value; OnPropertyChanged("IsEnabledEducativoD"); }
        }

        private bool _IsEnabledSeguridadDatos = true;
        public bool IsEnabledSeguridadDatos
        {
            get { return _IsEnabledSeguridadDatos; }
            set { _IsEnabledSeguridadDatos = value; OnPropertyChanged("IsEnabledSeguridadDatos"); }
        }

        private bool _VisibleMainFederal = false;
        public bool VisibleMainFederal
        {
            get { return _VisibleMainFederal; }
            set
            {
                _VisibleMainFederal = value;
                OnPropertyChanged("VisibleMainFederal");
            }
        }

        private short _OrdenMainFede;

        public short OrdenMainFede
        {
            get { return _OrdenMainFede; }
            set { _OrdenMainFede = value; OnPropertyChanged("OrdenMainFede"); }
        }
        private short _OrdenMainComun;
        public short OrdenMainComun
        {
            get { return _OrdenMainComun; }
            set { _OrdenMainComun = value; OnPropertyChanged("OrdenMainComun"); }
        }

        private bool _IsEnabledActaConsejoT = false;
        public bool IsEnabledActaConsejoT
        {
            get { return _IsEnabledActaConsejoT; }
            set { _IsEnabledActaConsejoT = value; OnPropertyChanged("IsEnabledActaConsejoT"); }
        }
        private bool _IsEnabledMedicoFederal = false;

        public bool IsEnabledMedicoFederal
        {
            get { return _IsEnabledMedicoFederal; }
            set
            {
                _IsEnabledMedicoFederal = value;
                OnPropertyChanged("IsEnabledMedicoFederal");
            }
        }
        private bool _IsEnabledPsicoFederal = false;

        public bool IsEnabledPsicoFederal
        {
            get { return _IsEnabledPsicoFederal; }
            set
            {
                _IsEnabledPsicoFederal = value;
                OnPropertyChanged("IsEnabledPsicoFederal");
            }
        }
        private bool _IsTrabajoSocialEnabled = false;

        public bool IsTrabajoSocialEnabled
        {
            get { return _IsTrabajoSocialEnabled; }
            set
            {
                _IsTrabajoSocialEnabled = value;
                OnPropertyChanged("IsTrabajoSocialEnabled");
            }
        }
        private bool _IsEnabledActivProd = false;

        public bool IsEnabledActivProd
        {
            get { return _IsEnabledActivProd; }
            set
            {
                _IsEnabledActivProd = value;
                OnPropertyChanged("IsEnabledActivProd");
            }
        }
        private bool _IsEnabledActivEducFederal = false;

        public bool IsEnabledActivEducFederal
        {
            get { return _IsEnabledActivEducFederal; }
            set
            {
                _IsEnabledActivEducFederal = value;
                OnPropertyChanged("IsEnabledActivEducFederal");
            }
        }
        private bool _IsEnabledVigilanciaFederal = false;

        public bool IsEnabledVigilanciaFederal
        {
            get { return _IsEnabledVigilanciaFederal; }
            set
            {
                _IsEnabledVigilanciaFederal = value;
                OnPropertyChanged("IsEnabledVigilanciaFederal");
            }
        }
        private bool _IsEnabledCriminologicoFederal = false;

        public bool IsEnabledCriminologicoFederal
        {
            get { return _IsEnabledCriminologicoFederal; }
            set
            {
                _IsEnabledCriminologicoFederal = value;
                OnPropertyChanged("IsEnabledCriminologicoFederal");
            }
        }

        private short _IdVentanaFederal;

        public short IdVentanaFederal
        {
            get { return _IdVentanaFederal; }
            set { _IdVentanaFederal = value; OnPropertyChanged("IdVentanaFederal"); }
        }

        #endregion

        private string _FormatoFecha = new DateTime().ToString("yyyy");

        public string FormatoFecha
        {
            get { return _FormatoFecha; }
            set { _FormatoFecha = value; OnPropertyChanged("FormatoFecha"); }
        }

        #region Geograficos

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PAIS_NACIONALIDAD> lstPais;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PAIS_NACIONALIDAD> LstPais
        {
            get { return lstPais; }
            set { lstPais = value; OnPropertyChanged("LstPais"); }
        }


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ENTIDAD> lstEstado;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ENTIDAD> LstEstado
        {
            get { return lstEstado; }
            set { lstEstado = value; OnPropertyChanged("LstEstado"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.MUNICIPIO> lstMunicipio;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.MUNICIPIO> LstMunicipio
        {
            get { return lstMunicipio; }
            set { lstMunicipio = value; OnPropertyChanged("LstMunicipio"); }
        }

        private SSP.Servidor.MUNICIPIO _SeleMunicipio;

        public SSP.Servidor.MUNICIPIO SeleMunicipio
        {
            get { return _SeleMunicipio; }
            set
            {
                _SeleMunicipio = value;
                if (value != null)
                {
                    if (value.ID_MUNICIPIO != -1)
                        LstColonia = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.COLONIA>(value.COLONIA);
                    else
                        LstColonia = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.COLONIA>();
                }
                else
                    LstColonia = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.COLONIA>();
                LstColonia.Insert(0, new SSP.Servidor.COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                EColonia = -1;
                OnPropertyChanged("SeleMunicipio");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.COLONIA> lstColonia;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.COLONIA> LstColonia
        {
            get { return lstColonia; }
            set { lstColonia = value; OnPropertyChanged("LstColonia"); }
        }


        private short? ePais;
        public short? EPais
        {
            get { return ePais; }
            set { ePais = value; OnPropertyChanged("EPais"); }
        }

        private short? eEstado;
        public short? EEstado
        {
            get { return eEstado; }
            set { eEstado = value; OnPropertyChanged("EEstado"); }
        }

        private short? eMunicipio;
        public short? EMunicipio
        {
            get { return eMunicipio; }
            set
            {
                if (value > 0)
                    ListColonia = new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, value).OrderBy(o => o.DESCR));
                else
                    ListColonia = new ObservableCollection<COLONIA>();

                ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });

                if (ListColonia.Count == 1)
                {
                    ListColonia = new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, 1001).OrderBy(o => o.DESCR));
                    ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                    SelectColonia = 102;
                }
                else
                {
                    if (value == 1001)
                        SelectColonia = 102;
                    else
                        SelectColonia = -1;
                }

                eMunicipio = value;
                OnPropertyChanged("EMunicipio");
            }
        }

        private int? eColonia;
        public int? EColonia
        {
            get { return eColonia; }
            set { eColonia = value; OnPropertyChanged("EColonia"); }
        }

        private string eCalle;
        public string ECalle
        {
            get { return eCalle; }
            set { eCalle = value; OnPropertyChanged("ECalle"); }
        }

        #endregion

        #region Orden
        private short _OrdenPrincipal;

        public short OrdenPrincipal
        {
            get { return _OrdenPrincipal; }
            set
            {
                _OrdenPrincipal = value;
                OnPropertyChanged("OrdenPrincipal");
            }
        }


        private bool _VisibleComunMain = false;

        public bool VisibleComunMain
        {
            get { return _VisibleComunMain; }
            set { _VisibleComunMain = value; OnPropertyChanged("VisibleComunMain"); }
        }

        private short _IndexComun;
        public short IndexComun
        {
            get { return _IndexComun; }
            set { _IndexComun = value; OnPropertyChanged("IndexComun"); }
        }

        private short _IndexFederal;

        public short IndexFederal
        {
            get { return _IndexFederal; }
            set
            {
                _IndexFederal = value;
                OnPropertyChanged("IndexFederal");
            }
        }


        private short _IndexProceso;

        public short IndexProceso
        {
            get { return _IndexProceso; }
            set { _IndexProceso = value; OnPropertyChanged("IndexProceso"); }
        }
        private short _OrdenTabsComun;

        public short OrdenTabsComun
        {
            get { return _OrdenTabsComun; }
            set { _OrdenTabsComun = value; OnPropertyChanged("OrdenTabsComun"); }
        }

        private bool _VisibleComun = false;

        public bool VisibleComun
        {
            get { return _VisibleComun; }
            set { _VisibleComun = value; OnPropertyChanged("VisibleComun"); }
        }

        private bool _VisibleFederal = false;

        public bool VisibleFederal
        {
            get { return _VisibleFederal; }
            set { _VisibleFederal = value; OnPropertyChanged("VisibleFederal"); }
        }

        private bool _VisiblePrincipal = false;

        public bool VisiblePrincipal
        {
            get { return _VisiblePrincipal; }
            set { _VisiblePrincipal = value; OnPropertyChanged("VisiblePrincipal"); }
        }


        private bool _VisibleMedicoComun = false;

        public bool VisibleMedicoComun
        {
            get { return _VisibleMedicoComun; }
            set { _VisibleMedicoComun = value; OnPropertyChanged("VisibleMedicoComun"); }
        }
        private bool _VisiblePsiqComun = false;

        public bool VisiblePsiqComun
        {
            get { return _VisiblePsiqComun; }
            set { _VisiblePsiqComun = value; OnPropertyChanged("VisiblePsiqComun"); }
        }
        private bool _VisiblePsicoComun = false;

        public bool VisiblePsicoComun
        {
            get { return _VisiblePsicoComun; }
            set { _VisiblePsicoComun = value; OnPropertyChanged("VisiblePsicoComun"); }
        }
        private bool _VisibleCrimiComun = false;

        public bool VisibleCrimiComun
        {
            get { return _VisibleCrimiComun; }
            set { _VisibleCrimiComun = value; OnPropertyChanged("VisibleCrimiComun"); }
        }
        private bool _VisibleTSComun = false;

        public bool VisibleTSComun
        {
            get { return _VisibleTSComun; }
            set { _VisibleTSComun = value; OnPropertyChanged("VisibleTSComun"); }
        }
        private bool _VisibleEducComun = false;

        public bool VisibleEducComun
        {
            get { return _VisibleEducComun; }
            set { _VisibleEducComun = value; OnPropertyChanged("VisibleEducComun"); }
        }
        private bool _VisibleCapacComun = false;

        public bool VisibleCapacComun
        {
            get { return _VisibleCapacComun; }
            set { _VisibleCapacComun = value; OnPropertyChanged("VisibleCapacComun"); }
        }
        private bool _VisibleSeguridadComun = false;

        public bool VisibleSeguridadComun
        {
            get { return _VisibleSeguridadComun; }
            set { _VisibleSeguridadComun = value; OnPropertyChanged("VisibleSeguridadComun"); }
        }

        #endregion

        private DateTime _MaximaFechaRealizacionEstudios = Fechas.GetFechaDateServer;

        public DateTime MaximaFechaRealizacionEstudios
        {
            get { return _MaximaFechaRealizacionEstudios; }
            set { _MaximaFechaRealizacionEstudios = value; OnPropertyChanged("MaximaFechaRealizacionEstudios"); }
        }
    }
}
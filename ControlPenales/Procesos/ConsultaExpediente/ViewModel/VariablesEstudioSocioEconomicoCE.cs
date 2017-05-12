using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        #region Enabled en tabs
        private bool _DatosGrupoFamiliarPrimarioEnabled = true;
        public bool DatosGrupoFamiliarPrimarioEnabled
        {
            get { return _DatosGrupoFamiliarPrimarioEnabled; }
            set { _DatosGrupoFamiliarPrimarioEnabled = value; OnPropertyChanged("DatosGrupoFamiliarPrimarioEnabled"); }
        }

        private bool _TabDatosGrupoFamiliarPrimario = true;
        public bool TabDatosGrupoFamiliarPrimario
        {
            get { return _TabDatosGrupoFamiliarPrimario; }
            set { _TabDatosGrupoFamiliarPrimario = value; OnPropertyChanged("TabDatosGrupoFamiliarPrimario"); }
        }

        private enum eProcedimiento
        {
            SI = 0,
            NO = 1
        };

        private bool _DatosGrupoFamiliarSecundarioEnabled = true;
        public bool DatosGrupoFamiliarSecundarioEnabled
        {
            get { return _DatosGrupoFamiliarSecundarioEnabled; }
            set { _DatosGrupoFamiliarSecundarioEnabled = value; OnPropertyChanged("DatosGrupoFamiliarSecundarioEnabled"); }
        }

        private bool _TabDatosGrupoFamiliarSecundario = true;
        public bool TabDatosGrupoFamiliarSecundario
        {
            get { return _TabDatosGrupoFamiliarSecundario; }
            set { _TabDatosGrupoFamiliarSecundario = value; OnPropertyChanged("TabDatosGrupoFamiliarSecundario"); }
        }

        private bool _DictamenSocioEconomicoEnabled = true;
        public bool DictamenSocioEconomicoEnabled
        {
            get { return _DictamenSocioEconomicoEnabled; }
            set { _DictamenSocioEconomicoEnabled = value; OnPropertyChanged("DictamenSocioEconomicoEnabled"); }
        }

        private bool _TabDictamenSocioEconomico = true;
        public bool TabDictamenSocioEconomico
        {
            get { return _TabDictamenSocioEconomico; }
            set { _TabDictamenSocioEconomico = value; OnPropertyChanged("TabDictamenSocioEconomico"); }
        }

        #endregion

        #region Datos del Estudio
        private string _Dictamen;

        private SSP.Servidor.SOCIOECONOMICO _Estudio;
        public SSP.Servidor.SOCIOECONOMICO Estudio
        {
            get { return _Estudio; }
            set { _Estudio = value; OnPropertyChanged("Estudio"); }
        }

        public string Dictamen
        {
            get { return _Dictamen; }
            set { _Dictamen = value; OnPropertyChanged("Dictamen"); }
        }

        private System.DateTime _DictamenFecha;
        public System.DateTime DictamenFecha
        {
            get { return _DictamenFecha; }
            set { _DictamenFecha = value; OnPropertyChanged("DictamenFecha"); }
        }

        #region Grupo Primario multiple

        private System.Collections.Generic.List<SSP.Servidor.SOCIOECONOMICO_CAT> _ListCaractPrimario;
        public System.Collections.Generic.List<SSP.Servidor.SOCIOECONOMICO_CAT> ListCaractPrimario
        {
            get { return _ListCaractPrimario; }
            set
            {
                _ListCaractPrimario = value;
                OnPropertyChanged("ListCaractPrimario");
            }
        }

        private System.Collections.Generic.List<SSP.Servidor.SOCIOECONOMICO_CAT> _ListCaractSecundario;
        public System.Collections.Generic.List<SSP.Servidor.SOCIOECONOMICO_CAT> ListCaractSecundario
        {
            get { return _ListCaractSecundario; }
            set
            {
                _ListCaractSecundario = value;
                OnPropertyChanged("ListCaractSecundario");
            }
        }

        private bool _IsCartonPrimarioChecked = false;
        public bool IsCartonPrimarioChecked
        {
            get { return _IsCartonPrimarioChecked; }
            set
            {
                _IsCartonPrimarioChecked = value;
                OnPropertyChanged("IsCartonPrimarioChecked");
            }
        }

        private bool _IsAdobePrimarioChecked = false;
        public bool IsAdobePrimarioChecked
        {
            get { return _IsAdobePrimarioChecked; }
            set { _IsAdobePrimarioChecked = value; OnPropertyChanged("IsAdobePrimarioChecked"); }
        }

        private bool _IsLadrilloPrimarioChecked = false;
        public bool IsLadrilloPrimarioChecked
        {
            get { return _IsLadrilloPrimarioChecked; }
            set { _IsLadrilloPrimarioChecked = value; OnPropertyChanged("IsLadrilloPrimarioChecked"); }
        }

        private bool _IsBlockPrimarioChecked = false;
        public bool IsBlockPrimarioChecked
        {
            get { return _IsBlockPrimarioChecked; }
            set { _IsBlockPrimarioChecked = value; OnPropertyChanged("IsBlockPrimarioChecked"); }
        }

        private bool _IsMaderaPrimarioChecked = false;
        public bool IsMaderaPrimarioChecked
        {
            get { return _IsMaderaPrimarioChecked; }
            set { _IsMaderaPrimarioChecked = value; OnPropertyChanged("IsMaderaPrimarioChecked"); }
        }

        private bool _IsMaterialesOtrosPrimarioChecked = false;
        public bool IsMaterialesOtrosPrimarioChecked
        {
            get { return _IsMaterialesOtrosPrimarioChecked; }
            set { _IsMaterialesOtrosPrimarioChecked = value; OnPropertyChanged("IsMaterialesOtrosPrimarioChecked"); }
        }

        private bool _IsSalaPrimarioChecked = false;
        public bool IsSalaPrimarioChecked
        {
            get { return _IsSalaPrimarioChecked; }
            set { _IsSalaPrimarioChecked = value; OnPropertyChanged("IsSalaPrimarioChecked"); }
        }

        private bool _IsCocinaPrimarioChecked = false;
        public bool IsCocinaPrimarioChecked
        {
            get { return _IsCocinaPrimarioChecked; }
            set { _IsCocinaPrimarioChecked = value; OnPropertyChanged("IsCocinaPrimarioChecked"); }
        }

        private bool _IsComedorPrimarioChecked = false;
        public bool IsComedorPrimarioChecked
        {
            get { return _IsComedorPrimarioChecked; }
            set { _IsComedorPrimarioChecked = value; OnPropertyChanged("IsComedorPrimarioChecked"); }
        }

        private bool _IsRecamaraChecked = false;
        public bool IsRecamaraChecked
        {
            get { return _IsRecamaraChecked; }
            set { _IsRecamaraChecked = value; OnPropertyChanged("IsRecamaraChecked"); }
        }

        private bool _IsBanioChecked = false;
        public bool IsBanioChecked
        {
            get { return _IsBanioChecked; }
            set { _IsBanioChecked = value; OnPropertyChanged("IsBanioChecked"); }
        }

        private bool _IsDistribucionPrimariaOtrosChecked = false;
        public bool IsDistribucionPrimariaOtrosChecked
        {
            get { return _IsDistribucionPrimariaOtrosChecked; }
            set { _IsDistribucionPrimariaOtrosChecked = value; OnPropertyChanged("IsDistribucionPrimariaOtrosChecked"); }
        }

        private bool _IsEnergiaElectricaPrimariaChecked = false;
        public bool IsEnergiaElectricaPrimariaChecked
        {
            get { return _IsEnergiaElectricaPrimariaChecked; }
            set { _IsEnergiaElectricaPrimariaChecked = value; OnPropertyChanged("IsEnergiaElectricaPrimariaChecked"); }
        }

        private bool _IsAguaPrimariaChecked = false;
        public bool IsAguaPrimariaChecked
        {
            get { return _IsAguaPrimariaChecked; }
            set { _IsAguaPrimariaChecked = value; OnPropertyChanged("IsAguaPrimariaChecked"); }
        }

        private bool _IsDrenajePrimariaChecked = false;
        public bool IsDrenajePrimariaChecked
        {
            get { return _IsDrenajePrimariaChecked; }
            set { _IsDrenajePrimariaChecked = value; OnPropertyChanged("IsDrenajePrimariaChecked"); }
        }

        private bool _IsPavimentoPrimarioChecked = false;
        public bool IsPavimentoPrimarioChecked
        {
            get { return _IsPavimentoPrimarioChecked; }
            set { _IsPavimentoPrimarioChecked = value; OnPropertyChanged("IsPavimentoPrimarioChecked"); }
        }

        private bool _IsTelefonoPrimarioChecked = false;
        public bool IsTelefonoPrimarioChecked
        {
            get { return _IsTelefonoPrimarioChecked; }
            set { _IsTelefonoPrimarioChecked = value; OnPropertyChanged("IsTelefonoPrimarioChecked"); }
        }

        private bool _IsTVCableChecked = false;
        public bool IsTVCableChecked
        {
            get { return _IsTVCableChecked; }
            set { _IsTVCableChecked = value; OnPropertyChanged("IsTVCableChecked"); }
        }

        private bool _IsEstufaPrimarioChecked = false;
        public bool IsEstufaPrimarioChecked
        {
            get { return _IsEstufaPrimarioChecked; }
            set { _IsEstufaPrimarioChecked = value; OnPropertyChanged("IsEstufaPrimarioChecked"); }
        }

        private bool _IsRefrigeradorPrimarioChecked = false;
        public bool IsRefrigeradorPrimarioChecked
        {
            get { return _IsRefrigeradorPrimarioChecked; }
            set { _IsRefrigeradorPrimarioChecked = value; OnPropertyChanged("IsRefrigeradorPrimarioChecked"); }
        }

        private bool _IsMicroOndasPrimarioChecked = false;
        public bool IsMicroOndasPrimarioChecked
        {
            get { return _IsMicroOndasPrimarioChecked; }
            set { _IsMicroOndasPrimarioChecked = value; OnPropertyChanged("IsMicroOndasPrimarioChecked"); }
        }

        private bool _IsTVPrimarioChecked = false;
        public bool IsTVPrimarioChecked
        {
            get { return _IsTVPrimarioChecked; }
            set { _IsTVPrimarioChecked = value; OnPropertyChanged("IsTVPrimarioChecked"); }
        }

        private bool _IsLavadoraPrimarioChecked = false;
        public bool IsLavadoraPrimarioChecked
        {
            get { return _IsLavadoraPrimarioChecked; }
            set { _IsLavadoraPrimarioChecked = value; OnPropertyChanged("IsLavadoraPrimarioChecked"); }
        }

        private bool _IsSecadoraPrimarioChecked = false;
        public bool IsSecadoraPrimarioChecked
        {
            get { return _IsSecadoraPrimarioChecked; }
            set { _IsSecadoraPrimarioChecked = value; OnPropertyChanged("IsSecadoraPrimarioChecked"); }
        }

        private bool _IsComputadoraPrimarioChecked = false;
        public bool IsComputadoraPrimarioChecked
        {
            get { return _IsComputadoraPrimarioChecked; }
            set { _IsComputadoraPrimarioChecked = value; OnPropertyChanged("IsComputadoraPrimarioChecked"); }
        }

        private bool _IsOtrosElectrodomesticosPrimarioChecked = false;
        public bool IsOtrosElectrodomesticosPrimarioChecked
        {
            get { return _IsOtrosElectrodomesticosPrimarioChecked; }
            set { _IsOtrosElectrodomesticosPrimarioChecked = value; OnPropertyChanged("IsOtrosElectrodomesticosPrimarioChecked"); }
        }

        private bool _IsAutomovilPrimarioChecked = false;
        public bool IsAutomovilPrimarioChecked
        {
            get { return _IsAutomovilPrimarioChecked; }
            set { _IsAutomovilPrimarioChecked = value; OnPropertyChanged("IsAutomovilPrimarioChecked"); }
        }

        private bool _IsAutobusPrimarioChecked = false;
        public bool IsAutobusPrimarioChecked
        {
            get { return _IsAutobusPrimarioChecked; }
            set { _IsAutobusPrimarioChecked = value; OnPropertyChanged("IsAutobusPrimarioChecked"); }
        }

        private bool _IsOtrosMediosTransportePrimarioChecked = false;
        public bool IsOtrosMediosTransportePrimarioChecked
        {
            get { return _IsOtrosMediosTransportePrimarioChecked; }
            set { _IsOtrosMediosTransportePrimarioChecked = value; OnPropertyChanged("IsOtrosMediosTransportePrimarioChecked"); }
        }

        private bool _IsCartonSecundarioChecked = false;
        public bool IsCartonSecundarioChecked
        {
            get { return _IsCartonSecundarioChecked; }
            set { _IsCartonSecundarioChecked = value; OnPropertyChanged("IsCartonSecundarioChecked"); }
        }

        private bool _IsAdobeSecundarioChecked = false;
        public bool IsAdobeSecundarioChecked
        {
            get { return _IsAdobeSecundarioChecked; }
            set { _IsAdobeSecundarioChecked = value; OnPropertyChanged("IsAdobeSecundarioChecked"); }
        }

        private bool _IsLadrilloSecundarioChecked = false;
        public bool IsLadrilloSecundarioChecked
        {
            get { return _IsLadrilloSecundarioChecked; }
            set { _IsLadrilloSecundarioChecked = value; OnPropertyChanged("IsLadrilloSecundarioChecked"); }
        }

        private bool _IsBlockSecundarioChecked = false;
        public bool IsBlockSecundarioChecked
        {
            get { return _IsBlockSecundarioChecked; }
            set { _IsBlockSecundarioChecked = value; OnPropertyChanged("IsBlockSecundarioChecked"); }
        }


        private bool _IsMaderaSecundarioChecked = false;
        public bool IsMaderaSecundarioChecked
        {
            get { return _IsMaderaSecundarioChecked; }
            set { _IsMaderaSecundarioChecked = value; OnPropertyChanged("IsMaderaSecundarioChecked"); }
        }

        private bool _IsOtrosMaterialesSecundarioChecked = false;
        public bool IsOtrosMaterialesSecundarioChecked
        {
            get { return _IsOtrosMaterialesSecundarioChecked; }
            set { _IsOtrosMaterialesSecundarioChecked = value; OnPropertyChanged("IsOtrosMaterialesSecundarioChecked"); }
        }

        private bool _IsSalaSecundarioChecked = false;
        public bool IsSalaSecundarioChecked
        {
            get { return _IsSalaSecundarioChecked; }
            set { _IsSalaSecundarioChecked = value; OnPropertyChanged("IsSalaSecundarioChecked"); }
        }

        private bool _IsCocinaSecundarioChecked = false;
        public bool IsCocinaSecundarioChecked
        {
            get { return _IsCocinaSecundarioChecked; }
            set { _IsCocinaSecundarioChecked = value; OnPropertyChanged("IsCocinaSecundarioChecked"); }
        }

        private bool _IsComedorSecundarioChecked = false;
        public bool IsComedorSecundarioChecked
        {
            get { return _IsComedorSecundarioChecked; }
            set { _IsComedorSecundarioChecked = value; OnPropertyChanged("IsComedorSecundarioChecked"); }
        }

        private bool _IsRecamaraSecundarioChecked = false;
        public bool IsRecamaraSecundarioChecked
        {
            get { return _IsRecamaraSecundarioChecked; }
            set { _IsRecamaraSecundarioChecked = value; OnPropertyChanged("IsRecamaraSecundarioChecked"); }
        }

        private bool _IsBanioSecundarioChecked = false;
        public bool IsBanioSecundarioChecked
        {
            get { return _IsBanioSecundarioChecked; }
            set { _IsBanioSecundarioChecked = value; OnPropertyChanged("IsBanioSecundarioChecked"); }
        }

        private bool _IsOtrosDistribucionSecundarioChecked = false;
        public bool IsOtrosDistribucionSecundarioChecked
        {
            get { return _IsOtrosDistribucionSecundarioChecked; }
            set { _IsOtrosDistribucionSecundarioChecked = value; OnPropertyChanged("IsOtrosDistribucionSecundarioChecked"); }
        }

        private bool _IsEnergiaElectricaSecundariaChecked = false;
        public bool IsEnergiaElectricaSecundariaChecked
        {
            get { return _IsEnergiaElectricaSecundariaChecked; }
            set { _IsEnergiaElectricaSecundariaChecked = value; OnPropertyChanged("IsEnergiaElectricaSecundariaChecked"); }
        }

        private bool _IsAguaSecundarioChecked = false;
        public bool IsAguaSecundarioChecked
        {
            get { return _IsAguaSecundarioChecked; }
            set { _IsAguaSecundarioChecked = value; OnPropertyChanged("IsAguaSecundarioChecked"); }
        }

        private bool _IsDrenajeSecundarioChecked = false;
        public bool IsDrenajeSecundarioChecked
        {
            get { return _IsDrenajeSecundarioChecked; }
            set { _IsDrenajeSecundarioChecked = value; OnPropertyChanged("IsDrenajeSecundarioChecked"); }
        }

        private bool _IsPavimentoSecundarioChecked = false;
        public bool IsPavimentoSecundarioChecked
        {
            get { return _IsPavimentoSecundarioChecked; }
            set { _IsPavimentoSecundarioChecked = value; OnPropertyChanged("IsPavimentoSecundarioChecked"); }
        }

        private bool _IsTelefonoSecundarioChecked = false;
        public bool IsTelefonoSecundarioChecked
        {
            get { return _IsTelefonoSecundarioChecked; }
            set { _IsTelefonoSecundarioChecked = value; OnPropertyChanged("IsTelefonoSecundarioChecked"); }
        }

        private bool _IsTVCableSecundarioChecked = false;
        public bool IsTVCableSecundarioChecked
        {
            get { return _IsTVCableSecundarioChecked; }
            set { _IsTVCableSecundarioChecked = value; OnPropertyChanged("IsTVCableSecundarioChecked"); }
        }

        private bool _IsEstufaSecundarioChecked = false;
        public bool IsEstufaSecundarioChecked
        {
            get { return _IsEstufaSecundarioChecked; }
            set { _IsEstufaSecundarioChecked = value; OnPropertyChanged("IsEstufaSecundarioChecked"); }
        }

        private bool _IsRefrigeradorSecundarioChecked = false;
        public bool IsRefrigeradorSecundarioChecked
        {
            get { return _IsRefrigeradorSecundarioChecked; }
            set { _IsRefrigeradorSecundarioChecked = value; OnPropertyChanged("IsRefrigeradorSecundarioChecked"); }
        }

        private bool _IsMicroOndasSecundarioChecked = false;
        public bool IsMicroOndasSecundarioChecked
        {
            get { return _IsMicroOndasSecundarioChecked; }
            set { _IsMicroOndasSecundarioChecked = value; OnPropertyChanged("IsMicroOndasSecundarioChecked"); }
        }

        private bool _IsTVSecundarioChecked = false;
        public bool IsTVSecundarioChecked
        {
            get { return _IsTVSecundarioChecked; }
            set { _IsTVSecundarioChecked = value; OnPropertyChanged("IsTVSecundarioChecked"); }
        }

        private bool _IsLavadoraSecundarioChecked = false;
        public bool IsLavadoraSecundarioChecked
        {
            get { return _IsLavadoraSecundarioChecked; }
            set { _IsLavadoraSecundarioChecked = value; OnPropertyChanged("IsLavadoraSecundarioChecked"); }
        }

        private bool _IsSecadoraSecundarioChecked = false;
        public bool IsSecadoraSecundarioChecked
        {
            get { return _IsSecadoraSecundarioChecked; }
            set { _IsSecadoraSecundarioChecked = value; OnPropertyChanged("IsSecadoraSecundarioChecked"); }
        }

        private bool _IsComputadoraSecundariaChecked = false;
        public bool IsComputadoraSecundariaChecked
        {
            get { return _IsComputadoraSecundariaChecked; }
            set { _IsComputadoraSecundariaChecked = value; OnPropertyChanged("IsComputadoraSecundariaChecked"); }
        }

        private bool _IsOtrosElectrodomesticosChecked = false;
        public bool IsOtrosElectrodomesticosChecked
        {
            get { return _IsOtrosElectrodomesticosChecked; }
            set { _IsOtrosElectrodomesticosChecked = value; OnPropertyChanged("IsOtrosElectrodomesticosChecked"); }
        }

        private bool _IsAutomovilSecundarioChecked = false;
        public bool IsAutomovilSecundarioChecked
        {
            get { return _IsAutomovilSecundarioChecked; }
            set { _IsAutomovilSecundarioChecked = value; OnPropertyChanged("IsAutomovilSecundarioChecked"); }
        }

        private bool _IsAutobusSecundarioChecked = false;
        public bool IsAutobusSecundarioChecked
        {
            get { return _IsAutobusSecundarioChecked; }
            set { _IsAutobusSecundarioChecked = value; OnPropertyChanged("IsAutobusSecundarioChecked"); }
        }

        private bool _IsOtrosMediosTransporteChecked = false;
        public bool IsOtrosMediosTransporteChecked
        {
            get { return _IsOtrosMediosTransporteChecked; }
            set { _IsOtrosMediosTransporteChecked = value; OnPropertyChanged("IsOtrosMediosTransporteChecked"); }
        }


        #endregion


        #region Grupo Primario
        private string _GrupoFamiliarPrimario;
        public string GrupoFamiliarPrimario
        {
            get { return _GrupoFamiliarPrimario; }
            set { _GrupoFamiliarPrimario = value; OnPropertyChanged("GrupoFamiliarPrimario"); }
        }

        private decimal? _Salario;
        public decimal? Salario
        {
            get { return _Salario; }
            set { _Salario = value; OnPropertyChanged("Salario"); }
        }


        private string _RelacionIntroFamiliarPrimario;
        public string RelacionIntroFamiliarPrimario
        {
            get { return _RelacionIntroFamiliarPrimario; }
            set { _RelacionIntroFamiliarPrimario = value; OnPropertyChanged("RelacionIntroFamiliarPrimario"); }
        }

        private short? _NoPersonasVivenHogar;
        public short? NoPersonasVivenHogar
        {
            get { return _NoPersonasVivenHogar; }
            set { _NoPersonasVivenHogar = value; OnPropertyChanged("NoPersonasVivenHogar"); }
        }

        private short? _NoPersonasTrabajanPrimario;
        public short? NoPersonasTrabajanPrimario
        {
            get { return _NoPersonasTrabajanPrimario; }
            set { _NoPersonasTrabajanPrimario = value; OnPropertyChanged("NoPersonasTrabajanPrimario"); }
        }

        private int? _IngresoMensualPrimario;
        public int? IngresoMensualPrimario
        {
            get { return _IngresoMensualPrimario; }
            set { _IngresoMensualPrimario = value; OnPropertyChanged("IngresoMensualPrimario"); }
        }

        private int? _EgresoMensualPrimario;
        public int? EgresoMensualPrimario
        {
            get { return _EgresoMensualPrimario; }
            set { _EgresoMensualPrimario = value; OnPropertyChanged("EgresoMensualPrimario"); }
        }

        private decimal? _FamiliarAntecedentePrimario;
        public decimal? FamiliarAntecedentePrimario
        {
            get { return _FamiliarAntecedentePrimario; }
            set
            {
                _FamiliarAntecedentePrimario = value;
                OnPropertyChanged("FamiliarAntecedentePrimario");
            }
        }

        private bool _IsEnabledAntecedentePrimario = false;
        public bool IsEnabledAntecedentePrimario
        {
            get { return _IsEnabledAntecedentePrimario; }
            set { _IsEnabledAntecedentePrimario = value; OnPropertyChanged("IsEnabledAntecedentePrimario"); }
        }

        private bool _IsEnabledAntecedenteSecundario = false;

        public bool IsEnabledAntecedenteSecundario
        {
            get { return _IsEnabledAntecedenteSecundario; }
            set { _IsEnabledAntecedenteSecundario = value; OnPropertyChanged("IsEnabledAntecedenteSecundario"); }
        }


        private string _AntecedentePrimario;
        public string AntecedentePrimario
        {
            get { return _AntecedentePrimario; }
            set { _AntecedentePrimario = value; OnPropertyChanged("AntecedentePrimario"); }
        }

        private string _ViviendaZonaPrimario;
        public string ViviendaZonaPrimario
        {
            get { return _ViviendaZonaPrimario; }
            set { _ViviendaZonaPrimario = value; OnPropertyChanged("ViviendaZonaPrimario"); }
        }

        private string _ViviendaCondicionesPrimario;
        public string ViviendaCondicionesPrimario
        {
            get { return _ViviendaCondicionesPrimario; }
            set { _ViviendaCondicionesPrimario = value; OnPropertyChanged("ViviendaCondicionesPrimario"); }
        }

        private string _NivelSocioCulturalPrimario;
        public string NivelSocioCulturalPrimario
        {
            get { return _NivelSocioCulturalPrimario; }
            set { _NivelSocioCulturalPrimario = value; OnPropertyChanged("NivelSocioCulturalPrimario"); }
        }

        #endregion

        #endregion Grupo Primario
        #region Grupo Primario Caracteristicas
        private char _IdTipo;
        public char IdTipo
        {
            get { return _IdTipo; }
            set { _IdTipo = value; OnPropertyChanged("IdTipo"); }
        }

        private short _IdClave;
        public short IdClave
        {
            get { return _IdClave; }
            set { _IdClave = value; OnPropertyChanged("IdClave"); }
        }

        private System.DateTime _RegistroFecha;
        public System.DateTime RegistroFecha
        {
            get { return _RegistroFecha; }
            set { _RegistroFecha = value; OnPropertyChanged("RegistroFecha"); }
        }


        #endregion
        #region Grupo Secundario
        private string _GrupoFamiliarSecundario;
        public string GrupoFamiliarSecundario
        {
            get { return _GrupoFamiliarSecundario; }
            set { _GrupoFamiliarSecundario = value; OnPropertyChanged("GrupoFamiliarSecundario"); }
        }

        private string _RelacionIntroFamiliarSecundario;
        public string RelacionIntroFamiliarSecundario
        {
            get { return _RelacionIntroFamiliarSecundario; }
            set { _RelacionIntroFamiliarSecundario = value; OnPropertyChanged("RelacionIntroFamiliarSecundario"); }
        }

        private short? _PersonasLaboranSecundario;
        public short? PersonasLaboranSecundario
        {
            get { return _PersonasLaboranSecundario; }
            set { _PersonasLaboranSecundario = value; OnPropertyChanged("PersonasLaboranSecundario"); }
        }

        private int? _IngresoMensualSecundario;
        public int? IngresoMensualSecundario
        {
            get { return _IngresoMensualSecundario; }
            set { _IngresoMensualSecundario = value; OnPropertyChanged("IngresoMensualSecundario"); }
        }

        private int? _EgresoMensualSecundario;
        public int? EgresoMensualSecundario
        {
            get { return _EgresoMensualSecundario; }
            set { _EgresoMensualSecundario = value; OnPropertyChanged("EgresoMensualSecundario"); }
        }

        private decimal? _FamiliarAntecedenteSecundario;
        public decimal? FamiliarAntecedenteSecundario
        {
            get { return _FamiliarAntecedenteSecundario; }
            set
            {
                _FamiliarAntecedenteSecundario = value;
                OnPropertyChanged("FamiliarAntecedenteSecundario");
            }
        }

        private string _AntecedenteSecundario;
        public string AntecedenteSecundario
        {
            get { return _AntecedenteSecundario; }
            set { _AntecedenteSecundario = value; OnPropertyChanged("AntecedenteSecundario"); }
        }

        private string _ViviendaZonaSecundario;
        public string ViviendaZonaSecundario
        {
            get { return _ViviendaZonaSecundario; }
            set { _ViviendaZonaSecundario = value; OnPropertyChanged("ViviendaZonaSecundario"); }
        }

        private string _ViviendaCondicionesSecundario;
        public string ViviendaCondicionesSecundario
        {
            get { return _ViviendaCondicionesSecundario; }
            set { _ViviendaCondicionesSecundario = value; OnPropertyChanged("ViviendaCondicionesSecundario"); }
        }

        private string _NivelSocioCulturalSecundario;
        public string NivelSocioCulturalSecundario
        {
            get { return _NivelSocioCulturalSecundario; }
            set
            {
                _NivelSocioCulturalSecundario = value;
                OnPropertyChanged("NivelSocioCulturalSecundario");
            }
        }

        private bool _IsEnabledRazonNoRecibeVisita = false;
        public bool IsEnabledRazonNoRecibeVisita
        {
            get { return _IsEnabledRazonNoRecibeVisita; }
            set { _IsEnabledRazonNoRecibeVisita = value; OnPropertyChanged("IsEnabledRazonNoRecibeVisita"); }
        }


        private decimal? _RecibeVisita;
        public decimal? RecibeVisita
        {
            get { return _RecibeVisita; }
            set
            {
                _RecibeVisita = value;
                OnPropertyChanged("RecibeVisita");
            }
        }

        private string _DeQuien;
        public string DeQuien
        {
            get { return _DeQuien; }
            set { _DeQuien = value; OnPropertyChanged("DeQuien"); }
        }

        private string _Frecuencia;
        public string Frecuencia
        {
            get { return _Frecuencia; }
            set { _Frecuencia = value; OnPropertyChanged("Frecuencia"); }
        }

        private string _RazonNoRecibeVisita;
        public string RazonNoRecibeVisita
        {
            get { return _RazonNoRecibeVisita; }
            set { _RazonNoRecibeVisita = value; OnPropertyChanged("RazonNoRecibeVisita"); }
        }


        private decimal? _ApoyoEconomico;
        public decimal? ApoyoEconomico
        {
            get { return _ApoyoEconomico; }
            set
            {
                _ApoyoEconomico = value;
                OnPropertyChanged("ApoyoEconomico");
            }
        }


        private bool _IsEnabledMedioApoyo = false;
        public bool IsEnabledMedioApoyo
        {
            get { return _IsEnabledMedioApoyo; }
            set { _IsEnabledMedioApoyo = value; OnPropertyChanged("IsEnabledMedioApoyo"); }
        }

        private bool _IsGiroChecked = false;
        public bool IsGiroChecked
        {
            get { return _IsGiroChecked; }
            set { _IsGiroChecked = value; OnPropertyChanged("IsGiroChecked"); }
        }

        private bool _IsCuentaBChecked = false;
        public bool IsCuentaBChecked
        {
            get { return _IsCuentaBChecked; }
            set { _IsCuentaBChecked = value; OnPropertyChanged("IsCuentaBChecked"); }
        }

        private bool _IsDepositoChecked = false;
        public bool IsDepositoChecked
        {
            get { return _IsDepositoChecked; }
            set { _IsDepositoChecked = value; OnPropertyChanged("IsDepositoChecked"); }
        }

        private short _MedioApoyoEconomico;
        public short MedioApoyoEconomico
        {
            get { return _MedioApoyoEconomico; }
            set { _MedioApoyoEconomico = value; OnPropertyChanged("MedioApoyoEconomico"); }
        }


        private bool _IsEnabledQuien = false;
        public bool IsEnabledQuien
        {
            get { return _IsEnabledQuien; }
            set { _IsEnabledQuien = value; OnPropertyChanged("IsEnabledQuien"); }
        }

        private bool _IsEnabledMedioApoyoEconomico = false;
        public bool IsEnabledMedioApoyoEconomico
        {
            get { return _IsEnabledMedioApoyoEconomico; }
            set { _IsEnabledMedioApoyoEconomico = value; OnPropertyChanged("IsEnabledMedioApoyoEconomico"); }
        }


        #endregion

        #region Grupo Secundario Caracteristicas
        private char _IdTipoSecundario;
        public char IdTipoSecundario
        {
            get { return _IdTipoSecundario; }
            set { _IdTipoSecundario = value; OnPropertyChanged("IdTipoSecundario"); }
        }

        private short _IdClaveSecundario;
        public short IdClaveSecundario
        {
            get { return _IdClaveSecundario; }
            set { _IdClaveSecundario = value; OnPropertyChanged("IdClaveSecundario"); }
        }

        private System.DateTime _RegistroFechaSecundario;
        public System.DateTime RegistroFechaSecundario
        {
            get { return _RegistroFechaSecundario; }
            set { _RegistroFechaSecundario = value; OnPropertyChanged("RegistroFechaSecundario"); }
        }

        #endregion
        #region Dictamen
        private string _DictamenDescripcion;
        public string DictamenDescripcion
        {
            get { return _DictamenDescripcion; }
            set { _DictamenDescripcion = value; OnPropertyChanged("DictamenDescripcion"); }
        }

        private System.DateTime? _FechaEstudio;
        public System.DateTime? FechaEstudio
        {
            get { return _FechaEstudio; }
            set { _FechaEstudio = value; OnPropertyChanged("FechaEstudio"); }
        }

        #endregion
    }
}
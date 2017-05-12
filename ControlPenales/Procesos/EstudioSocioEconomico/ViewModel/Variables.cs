using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;

namespace ControlPenales
{
    partial class EstudioSocioEconomicoViewModel
    {

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set { pInsertar = value; }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set { pEditar = value; }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set { pConsultar = value; }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set { pImprimir = value; }
        }
        #endregion
        #region Menu

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        #endregion
        #region Busqueda e Imagenes de Imputado
        private string apellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return apellidoPaternoBuscar; }
            set { apellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
        }

        private string apellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return apellidoMaternoBuscar; }
            set { apellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscar"); }
        }

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set
            {
                nombreBuscar = value; OnPropertyChanged("NombreBuscar");
            }
        }

        private int? anioBuscar;
        public int? AnioBuscar
        {
            get { return anioBuscar; }
            set { anioBuscar = value; OnPropertyChanged("AnioBuscar"); }
        }

        private int? folioBuscar;
        public int? FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value; OnPropertyChanged("FolioBuscar"); }
        }

        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set
            {
                imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
            }
        }

        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set
            {
                imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
            }
        }

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }
        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        #endregion
        #region Datos Generales
        private SOCIOECONOMICO _Estudio;
        public SOCIOECONOMICO Estudio
        {
            get { return _Estudio; }
            set { _Estudio = value; OnPropertyChanged("Estudio"); }
        }

        private IMPUTADO selectedInterno;
        public IMPUTADO SelectedInterno
        {
            get { return selectedInterno; }
            set { selectedInterno = value; OnPropertyChanged("SelectedInterno"); }
        }
        private int? anioD;
        public int? AnioD
        {
            get { return anioD; }
            set { anioD = value; OnPropertyChanged("AnioD"); }
        }
        private int? folioD;
        public int? FolioD
        {
            get { return folioD; }
            set
            {
                folioD = value;
                OnPropertyChanged("FolioD");
            }
        }
        private string paternoD;
        public string PaternoD
        {
            get { return paternoD; }
            set { paternoD = value; OnPropertyChanged("PaternoD"); }
        }
        private string maternoD;
        public string MaternoD
        {
            get { return maternoD; }
            set { maternoD = value; OnPropertyChanged("MaternoD"); }
        }
        private string nombreD;
        public string NombreD
        {
            get { return nombreD; }
            set { nombreD = value; OnPropertyChanged("NombreD"); }
        }
        private int? ingresosD;
        public int? IngresosD
        {
            get { return ingresosD; }
            set { ingresosD = value; OnPropertyChanged("IngresosD"); }
        }
        private string noControlD;
        public string NoControlD
        {
            get { return noControlD; }
            set { noControlD = value; OnPropertyChanged("NoControlD"); }
        }
        private string ubicacionD;
        public string UbicacionD
        {
            get { return ubicacionD; }
            set { ubicacionD = value; OnPropertyChanged("UbicacionD"); }
        }
        private string tipoSeguridadD;
        public string TipoSeguridadD
        {
            get { return tipoSeguridadD; }
            set { tipoSeguridadD = value; OnPropertyChanged("TipoSeguridadD"); }
        }
        private DateTime? fecIngresoD;
        public DateTime? FecIngresoD
        {
            get { return fecIngresoD; }
            set { fecIngresoD = value; OnPropertyChanged("FecIngresoD"); }
        }
        private string clasificacionJuridicaD;
        public string ClasificacionJuridicaD
        {
            get { return clasificacionJuridicaD; }
            set { clasificacionJuridicaD = value; OnPropertyChanged("ClasificacionJuridicaD"); }
        }
        private string estatusD;
        public string EstatusD
        {
            get { return estatusD; }
            set { estatusD = value; OnPropertyChanged("EstatusD"); }
        }

        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set { listExpediente = value; OnPropertyChanged("ListExpediente"); }
        }

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }
        private bool emptyIngresoVisible;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private int indexMenu;
        public int IndexMenu
        {
            get { return indexMenu; }
            set { indexMenu = value; OnPropertyChanged("IndexMenu"); }
        }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (selectExpediente.INGRESO!=null && selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = selectExpediente.INGRESO.OrderBy(o => o.FEC_INGRESO_CERESO).FirstOrDefault();
                    }
                    else
                        EmptyIngresoVisible = true;

                    //OBTENEMOS FOTO DE FRENTE
                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        else
                            ImagenImputado = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenImputado = new Imagenes().getImagenPerson();
                }
                else
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    EmptyIngresoVisible = true;
                }
                OnPropertyChanged("SelectExpediente");
            }
        }
        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                {
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                    OnPropertyChanged("SelectIngreso");
                    return;
                }
                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectIngreso");
            }
        }

        private INGRESO AuxIngreso;

        private string tituloModal;
        public string TituloModal
        {
            get { return tituloModal; }
            set { tituloModal = value; OnPropertyChanged("TituloModal"); }
        }

        private string tituloAlias;
        public string TituloAlias
        {
            get { return tituloAlias; }
            set { tituloAlias = value; OnPropertyChanged("TituloAlias"); }
        }

        private string tituloApodo;
        public string TituloApodo
        {
            get { return tituloApodo; }
            set { tituloApodo = value; OnPropertyChanged("TituloApodo"); }
        }

        //NO ELIMINAR SI YA ESTAN GRABADOS EN BASE DE DATOS
        private bool eliminarVisible;
        public bool EliminarVisible
        {
            get { return eliminarVisible; }
            set { eliminarVisible = value; OnPropertyChanged("EliminarVisible"); }
        }

        private bool causaPenalDelitoEmpty = true;
        public bool CausaPenalDelitoEmpty
        {
            get { return causaPenalDelitoEmpty; }
            set { causaPenalDelitoEmpty = value; OnPropertyChanged("CausaPenalDelitoEmpty"); }
        }

        #endregion
        #region Enabled en tabs
        private bool _DatosGrupoFamiliarPrimarioEnabled = false;
        public bool DatosGrupoFamiliarPrimarioEnabled
        {
            get { return _DatosGrupoFamiliarPrimarioEnabled; }
            set { _DatosGrupoFamiliarPrimarioEnabled = value; OnPropertyChanged("DatosGrupoFamiliarPrimarioEnabled"); }
        }

        private bool _TabDatosGrupoFamiliarPrimario = false;
        public bool TabDatosGrupoFamiliarPrimario
        {
            get { return _TabDatosGrupoFamiliarPrimario; }
            set { _TabDatosGrupoFamiliarPrimario = value; OnPropertyChanged("TabDatosGrupoFamiliarPrimario"); }
        }

        private bool _DatosGrupoFamiliarSecundarioEnabled = false;
        public bool DatosGrupoFamiliarSecundarioEnabled
        {
            get { return _DatosGrupoFamiliarSecundarioEnabled; }
            set { _DatosGrupoFamiliarSecundarioEnabled = value; OnPropertyChanged("DatosGrupoFamiliarSecundarioEnabled"); }
        }

        private bool _TabDatosGrupoFamiliarSecundario = false;
        public bool TabDatosGrupoFamiliarSecundario
        {
            get { return _TabDatosGrupoFamiliarSecundario; }
            set { _TabDatosGrupoFamiliarSecundario = value; OnPropertyChanged("TabDatosGrupoFamiliarSecundario"); }
        }

        private bool _DictamenSocioEconomicoEnabled = false;
        public bool DictamenSocioEconomicoEnabled
        {
            get { return _DictamenSocioEconomicoEnabled; }
            set { _DictamenSocioEconomicoEnabled = value; OnPropertyChanged("DictamenSocioEconomicoEnabled"); }
        }

        private bool _TabDictamenSocioEconomico = false;
        public bool TabDictamenSocioEconomico
        {
            get { return _TabDictamenSocioEconomico; }
            set { _TabDictamenSocioEconomico = value; OnPropertyChanged("TabDictamenSocioEconomico"); }
        }



        #endregion
        #region Reporte
        private Visibility reportViewerVisible = Visibility.Hidden;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        #endregion
        #region Datos del Estudio
        private string _Dictamen;
        public string Dictamen
        {
            get { return _Dictamen; }
            set { _Dictamen = value; OnPropertyChanged("Dictamen"); }
        }

        private DateTime _DictamenFecha;
        public DateTime DictamenFecha
        {
            get { return _DictamenFecha; }
            set { _DictamenFecha = value; OnPropertyChanged("DictamenFecha"); }
        }


        #region Grupo Primario multiple

        private List<SOCIOECONOMICO_CAT> _ListCaractPrimario;
        public List<SOCIOECONOMICO_CAT> ListCaractPrimario
        {
            get { return _ListCaractPrimario; }
            set 
            {
                _ListCaractPrimario = value;
                OnPropertyChanged("ListCaractPrimario");
            }
        }

        private List<SOCIOECONOMICO_CAT> _ListCaractSecundario;
        public List<SOCIOECONOMICO_CAT> ListCaractSecundario
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
            set { _IsMaterialesOtrosPrimarioChecked = value; OnPropertyChanged("IsMaterialesOtrosPrimarioChecked");  }
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
            set { _IsMicroOndasPrimarioChecked = value; OnPropertyChanged("IsMicroOndasPrimarioChecked");  }
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
            set { _RelacionIntroFamiliarPrimario = value; OnPropertyChanged("RelacionIntroFamiliarPrimario");  }
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
                if (value == decimal.Zero)
                {
                    base.RemoveRule("AntecedentePrimario");
                    IsEnabledAntecedentePrimario = true;
                    base.AddRule(() => AntecedentePrimario, () => !string.IsNullOrEmpty(AntecedentePrimario), "ANTECEDENTE SECUNDARIO ES REQUERIDO!");
                    OnPropertyChanged("AntecedentePrimario");
                }
                else
                {
                    IsEnabledAntecedentePrimario = false;
                    base.RemoveRule("AntecedentePrimario");
                    OnPropertyChanged("AntecedentePrimario");
                }

                OnPropertyChanged("FamiliarAntecedentePrimario");
                OnPropertyChanged("AntecedentePrimario");
                OnPropertyChanged("IsEnabledAntecedentePrimario");
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

        private DateTime _RegistroFecha;
        public DateTime RegistroFecha
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
                if (value == decimal.Zero)
                {
                    base.RemoveRule("AntecedenteSecundario");
                    IsEnabledAntecedenteSecundario = true;
                    base.AddRule(() => AntecedenteSecundario, () => !string.IsNullOrEmpty(AntecedenteSecundario), "ANTECEDENTE PRIMARIO ES REQUERIDO!");
                    OnPropertyChanged("AntecedenteSecundario");
                }
                else
                {
                    base.RemoveRule("AntecedenteSecundario");
                    OnPropertyChanged("AntecedenteSecundario");
                    IsEnabledAntecedenteSecundario = false;
                }

                OnPropertyChanged("FamiliarAntecedenteSecundario");
                OnPropertyChanged("AntecedenteSecundario");
                OnPropertyChanged("IsEnabledAntecedenteSecundario");
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
                if (value.HasValue && value != -1)
                {
                    if (value == decimal.Zero)
                    {//si recibe visita
                        base.RemoveRule("Frecuencia");
                        base.RemoveRule("DeQuien");
                        IsEnabledRazonNoRecibeVisita = false;
                        IsEnabledQuien = true;
                        base.AddRule(() => Frecuencia, () => !string.IsNullOrEmpty(Frecuencia), "LA FRECUENCIA DE LA VISITA ES REQUERIDO!");
                        OnPropertyChanged("Frecuencia");
                        base.AddRule(() => DeQuien, () => !string.IsNullOrEmpty(DeQuien), "LA DESCRIPCION ACERCA DE QUIEN RECIBE VISITA ES REQUERIDO!");
                        OnPropertyChanged("DeQuien");

                        base.RemoveRule("RazonNoRecibeVisita");
                        OnPropertyChanged("RazonNoRecibeVisita");
                    }

                    else
                    {
                        IsEnabledRazonNoRecibeVisita = true;
                        IsEnabledQuien = false;

                        base.RemoveRule("DeQuien");
                        base.RemoveRule("Frecuencia");

                        OnPropertyChanged("Frecuencia");
                        OnPropertyChanged("DeQuien");

                        base.AddRule(() => RazonNoRecibeVisita, () => !string.IsNullOrEmpty(RazonNoRecibeVisita), "LA RAZON DEL PORQUE NO RECIBE VISITA ES REQUERIDO!");
                        OnPropertyChanged("RazonNoRecibeVisita");
                    }
                }

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
                if (value == decimal.Zero)
                    IsEnabledMedioApoyo = true;
                else
                    IsEnabledMedioApoyo = false;

                OnPropertyChanged("IsEnabledMedioApoyo");
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

        private DateTime _RegistroFechaSecundario;
        public DateTime RegistroFechaSecundario
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

        private DateTime? _FechaEstudio;
        public DateTime? FechaEstudio
        {
            get { return _FechaEstudio; }
            set { _FechaEstudio = value; OnPropertyChanged("FechaEstudio"); }
        }

        #endregion

    }
}

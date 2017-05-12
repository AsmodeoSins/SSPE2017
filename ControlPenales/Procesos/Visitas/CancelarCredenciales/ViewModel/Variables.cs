using Andora.UserControlLibrary;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlPenales
{
    partial class CancelacionVisitasViewModel
    {
        private long? TextCodigoCredencial { get; set; }
        public long? TextCodigo
        {
            get
            {
                return TextCodigoCredencial;
            }
            set
            {
                TextCodigoCredencial = value;
                OnPropertyChanged("TextCodigo");
            }
        }

        private string TextNombreCredencial { get; set; }
        public string TextNombre
        {
            get
            {
                return TextNombreCredencial;
            }
            set
            {
                TextNombreCredencial = value;
                OnPropertyChanged("TextNombre");
            }
        }

        private string TextPaternoCredencial { get; set; }
        public string TextPaterno
        {
            get
            {
                return TextPaternoCredencial;
            }
            set
            {
                TextPaternoCredencial = value;
                OnPropertyChanged("TextPaterno");
            }
        }

        private string TextMaternoCredencial { get; set; }
        public string TextMaterno
        {
            get
            {
                return TextMaternoCredencial;
            }
            set
            {
                TextMaternoCredencial = value;
                OnPropertyChanged("TextMaterno");
            }
        }

        private string SelectSexoCredencial { get; set; }
        public string SelectSexo
        {
            get
            {
                return SelectSexoCredencial;
            }
            set
            {
                SelectSexoCredencial = value;
                OnPropertyChanged("SelectSexo");
            }
        }

        private DateTime? FechaNacimientoCredencial { get; set; }
        public DateTime? FechaNacimiento
        {
            get
            {
                return FechaNacimientoCredencial;
            }
            set
            {
                FechaNacimientoCredencial = value;
                if (value.HasValue)
                {
                    var hoy = Fechas.GetFechaDateServer;
                    var edad = hoy.Year - value.Value.Year;
                    if (hoy.Month < value.Value.Month || (hoy.Month == value.Value.Month && hoy.Day < value.Value.Day))
                        edad--;
                    TextEdad = (short?)edad;
                }

                OnPropertyChanged("FechaNacimiento");
            }
        }

        private short? TextEdadCredencial { get; set; }
        public short? TextEdad
        {
            get
            {
                return TextEdadCredencial;
            }
            set
            {
                TextEdadCredencial = value;
                OnPropertyChanged("TextEdad");
            }
        }

        private ObservableCollection<TIPO_VISITANTE> listTipoVisitante;
        public ObservableCollection<TIPO_VISITANTE> ListTipoVisitante
        {
            get { return listTipoVisitante; }
            set
            {
                listTipoVisitante = value;
                OnPropertyChanged("ListTipoVisitante");
            }
        }

        private short? SelectTipoVisitanteCredencial = -1;
        public short? SelectTipoVisitante
        {
            get
            {
                return SelectTipoVisitanteCredencial;
            }
            set
            {
                SelectTipoVisitanteCredencial = value;
                OnPropertyChanged("SelectTipoVisitante");
            }
        }

        private string TextNipCredencial { get; set; }
        public string TextNip
        {
            get
            {
                return TextNipCredencial;
            }
            set
            {
                TextNipCredencial = value;
                OnPropertyChanged("TextNip");
            }
        }

        private ObservableCollection<ESTATUS_VISITA> listSituacion;
        public ObservableCollection<ESTATUS_VISITA> ListSituacion
        {
            get { return listSituacion; }
            set { listSituacion = value; OnPropertyChanged("ListSituacion"); }
        }

        private ObservableCollection<ESTATUS_VISITA> listEstatusVisita;
        public ObservableCollection<ESTATUS_VISITA> ListEstatusVisita
        {
            get { return listEstatusVisita; }
            set { listEstatusVisita = value; OnPropertyChanged("ListEstatusVisita"); }
        }
        
        private short? SelectSituacionCredencial { get; set; }

        private short? selectSituacion = -1;
        public short? SelectSituacion
        {
            get { return selectSituacion; }
            set { selectSituacion = value; OnPropertyChanged("SelectSituacion"); }
        }
        //public short? SelectSituacion  
        //{
        //    get
        //    {
        //        return SelectSituacionCredencial;
        //    }
        //    set
        //    {
        //        SelectSituacionCredencial = value;
        //        OnPropertyChanged("SelectSituacion");
        //    }
        //}

        private string TextObservacionCredencial { get; set; }
        public string TextObservacion
        {
            get
            {
                return TextObservacionCredencial;
            }
            set
            {
                TextObservacionCredencial = value;
                OnPropertyChanged("TextObservacion");
            }
        }

        private ObservableCollection<ACOMPANANTE> listAcompanantesCredencial;
        public ObservableCollection<ACOMPANANTE> ListAcompanantes
        {
            get
            {
                return listAcompanantesCredencial;
            }
            set
            {
                listAcompanantesCredencial = value;
                OnPropertyChanged("ListAcompanantes");
            }
        }

        private ACOMPANANTE selectAcompanante;
        public ACOMPANANTE SelectAcompanante
        {
            get { return selectAcompanante; }
            set
            {
                selectAcompanante = value;
                OnPropertyChanged("SelectAcompanante");
            }
        }

        private ObservableCollection<VISITANTE_INGRESO> _ListadoInternosCredencial;
        public ObservableCollection<VISITANTE_INGRESO> ListadoInternos
        {
            get
            {
                return _ListadoInternosCredencial;
            }
            set
            {
                _ListadoInternosCredencial = value;
                OnPropertyChanged("ListadoInternos");
            }
        }

        private VISITANTE_INGRESO SelectVisitanteIngresoCredencial;
        public VISITANTE_INGRESO SelectVisitanteIngreso
        {
            get
            {
                return SelectVisitanteIngresoCredencial;
            }
            set
            {
                SelectVisitanteIngresoCredencial = value;
                OnPropertyChanged("SelectVisitanteIngreso");
            }
        }

        private INGRESO SelectImputadoIngresoCredencial;
        public INGRESO SelectImputadoIngreso
        {
            get
            {
                return SelectImputadoIngresoCredencial;
            }
            set
            {
                SelectImputadoIngresoCredencial = value;
                OnPropertyChanged("SelectImputadoIngreso");
            }
        }

        private string AnioDCredencial { get; set; }
        public string AnioD
        {
            get
            {
                return AnioDCredencial;
            }
            set
            {
                AnioDCredencial = value;
                OnPropertyChanged("AnioD");
            }
        }

        private string FolioDCredencial { get; set; }
        public string FolioD
        {
            get
            {
                return FolioDCredencial;
            }
            set
            {
                FolioDCredencial = value;
                OnPropertyChanged("FolioD");
            }
        }

        private string NombreDCredencial { get; set; }
        public string NombreD
        {
            get
            {
                return NombreDCredencial;
            }
            set
            {
                NombreDCredencial = value;
                OnPropertyChanged("NombreD");
            }
        }

        private string PaternoDCredencial { get; set; }
        public string PaternoD
        {
            get
            {
                return PaternoDCredencial;
            }
            set
            {
                PaternoDCredencial = value;
                OnPropertyChanged("PaternoD");
            }
        }

        private string MaternoDCredencial { get; set; }
        public string MaternoD
        {
            get
            {
                return MaternoDCredencial;
            }
            set
            {
                MaternoDCredencial = value;
                OnPropertyChanged("MaternoD");
            }
        }

        private string IngresosDCredencial { get; set; }
        public string IngresosD
        {
            get
            {
                return IngresosDCredencial;
            }
            set
            {
                IngresosDCredencial = value;
                OnPropertyChanged("IngresosD");
            }
        }

        private string UbicacionDCredencial { get; set; }
        public string UbicacionD
        {
            get
            {
                return UbicacionDCredencial;
            }
            set
            {
                UbicacionDCredencial = value;
                OnPropertyChanged("UbicacionD");
            }
        }

        private string TipoSeguridadDCredencial { get; set; }
        public string TipoSeguridadD
        {
            get
            {
                return TipoSeguridadDCredencial;
            }
            set
            {
                TipoSeguridadDCredencial = value;
                OnPropertyChanged("TipoSeguridadD");
            }
        }

        private string FecIngresoDCredencial { get; set; }
        public string FecIngresoD
        {
            get
            {
                return FecIngresoDCredencial;
            }
            set
            {
                FecIngresoDCredencial = value;

                OnPropertyChanged("FecIngresoD");
            }
        }

        private string ClasificacionJuridicaDCredencial { get; set; }
        public string ClasificacionJuridicaD
        {
            get
            {
                return ClasificacionJuridicaDCredencial;
            }
            set
            {
                ClasificacionJuridicaDCredencial = value;
                OnPropertyChanged("ClasificacionJuridicaD");
            }
        }

        private string EstatusDCredencial { get; set; }
        public string EstatusD
        {
            get
            {
                return EstatusDCredencial;
            }
            set
            {
                EstatusDCredencial = value;
                OnPropertyChanged("EstatusD");
            }
        }

        private PERSONAVISITAAUXILIAR SelectVisitanteCredencial { get; set; }
        public PERSONAVISITAAUXILIAR SelectVisitante
        {
            get
            {
                return SelectVisitanteCredencial;
            }
            set
            {
                SelectVisitanteCredencial = value;
                if (value != null)
                {
                    if (value.OBJETO_PERSONA != null)
                    {
                        if (value.OBJETO_PERSONA.PERSONA_BIOMETRICO != null)
                        {
                            var b = value.OBJETO_PERSONA.PERSONA_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO);
                            if (b != null)
                            {
                                ImagenPersona = b.BIOMETRICO;
                            }
                            else
                                ImagenPersona = new Imagenes().getImagenPerson();
                        }
                        else
                            ImagenPersona = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenPersona = new Imagenes().getImagenPerson();

                    SeleccionarVisitaExistente = true;
                }
                else
                {
                    ImagenPersona = new Imagenes().getImagenPerson();
                    SeleccionarVisitaExistente = false;
                }

                //if (value != null && value.OBJETO_PERSONA != null)
                //    ImagenPersona = value.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                //                        value.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                //                            new Imagenes().getImagenPerson();

                //if (value != null)
                //{
                //    SeleccionarVisitaExistente = true;
                //}
                //else
                //{
                //    SeleccionarVisitaExistente = false;
                //}
                OnPropertyChanged("SelectVisitante");
            }
        }

        private short? SelectParentescoCredencial { get; set; }
        public short? SelectParentesco
        {
            get
            {
                return SelectParentescoCredencial;
            }
            set
            {
                SelectParentescoCredencial = value;
                OnPropertyChanged("SelectParentesco");
            }
        }

        private short? selectEstatusRelacionCredencial;
        public short? SelectEstatusRelacion
        {
            get
            {
                return selectEstatusRelacionCredencial;
            }
            set
            {
                selectEstatusRelacionCredencial = value;
                OnPropertyChanged("SelectEstatusRelacion");
            }
        }

        private bool _ObservacionEnabled = false;
        public bool ObservacionEnabled
        {
            get { return _ObservacionEnabled; }
            set
            {
                _ObservacionEnabled = value;
                OnPropertyChanged("ObservacionEnabled");
            }
        }

        private bool _IsDetalleInternosEnable = false;
        public bool IsDetalleInternosEnable
        {
            get { return _IsDetalleInternosEnable; }
            set
            {
                _IsDetalleInternosEnable = value;
                OnPropertyChanged("IsDetalleInternosEnable");
            }
        }

        public string TextHeaderDatosInterno = "Datos del Interno Seleccionado";

        private Visibility datosExpedienteVisible = Visibility.Collapsed;
        public Visibility DatosExpedienteVisible
        {
            get { return datosExpedienteVisible; }
            set { datosExpedienteVisible = value; OnPropertyChanged("DatosExpedienteVisible"); }
        }

        private short selectParentescoAcompanante;
        public short SelectParentescoAcompanante
        {
            get { return selectParentescoAcompanante; }
            set { selectParentescoAcompanante = value; OnPropertyChanged("SelectParentescoAcompanante"); }
        }

        private ObservableCollection<VISITANTE_INGRESO> listBuscarAcompanantes;
        public ObservableCollection<VISITANTE_INGRESO> ListBuscarAcompanantes
        {
            get { return listBuscarAcompanantes; }
            set { listBuscarAcompanantes = value; OnPropertyChanged("ListBuscarAcompanantes"); }
        }

        private ObservableCollection<PERSONAVISITAAUXILIAR> listVisitantesCredencial;
        
        //public ObservableCollection<PERSONAVISITAAUXILIAR> ListVisitantes
        //{
        //    get
        //    {
        //        return listVisitantesCredencial;
        //    }
        //    set
        //    {
        //        listVisitantesCredencial = value;
        //        OnPropertyChanged("ListVisitantes");
        //    }
        //}


        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        private RangeEnabledObservableCollection<PERSONAVISITAAUXILIAR> listVisitantes;
        public RangeEnabledObservableCollection<PERSONAVISITAAUXILIAR> ListVisitantes
        {
            get { return listVisitantes; }
            set { listVisitantes = value; OnPropertyChanged("ListVisitantes"); }
        }

        

        private bool emptyBuscarRelacionInternoVisible;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return emptyBuscarRelacionInternoVisible; }
            set { emptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }

        private bool seleccionarVisitaExistente;
        public bool SeleccionarVisitaExistente
        {
            get { return seleccionarVisitaExistente; }
            set { seleccionarVisitaExistente = value; OnPropertyChanged("SeleccionarVisitaExistente"); }
        }

        private bool _enableMotivoEstatusAcompaniante;
        public bool enableMotivoEstatusAcompaniante
        {
            get { return _enableMotivoEstatusAcompaniante; }
            set
            {
                _enableMotivoEstatusAcompaniante = value;
                OnPropertyChanged("enableMotivoEstatusAcompaniante");
            }
        }

        private bool _enableMotivoEstatusImputado;
        public bool enableMotivoEstatusImputado
        {
            get { return _enableMotivoEstatusImputado; }
            set
            {
                _enableMotivoEstatusImputado = value;
                OnPropertyChanged("enableMotivoEstatusImputado");
            }
        }

        private string _SelectEstatusAcompaniante = "S";
        public string SelectEstatusAcompaniante
        {
            get
            {
                return _SelectEstatusAcompaniante;
            }
            set
            {
                _SelectEstatusAcompaniante = value;
                OnPropertyChanged("SelectEstatusAcompaniante");
            }
        }

        private short? _SelectEstatusImputado = -1;
        public short? SelectEstatusImputado
        {
            get
            {
                return _SelectEstatusImputado;
            }
            set
            {
                _SelectEstatusImputado = value;
                OnPropertyChanged("SelectEstatusImputado");
            }
        }

        private string _TextMotivoAcompaniante;
        public string TextMotivoAcompaniante
        {
            get
            {
                return _TextMotivoAcompaniante;
            }
            set
            {
                _TextMotivoAcompaniante = value;
                OnPropertyChanged("TextMotivoAcompaniante");
            }
        }

        private string _TextMotivoImputado;
        public string TextMotivoImputado
        {
            get
            {
                return _TextMotivoImputado;
            }
            set
            {
                _TextMotivoImputado = value;
                OnPropertyChanged("TextMotivoImputado");
            }
        }

        #region menu
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }

        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        #region Privilegios
        private bool PInsertar = false;
        private bool PEditar = false;
        private bool PConsultar = false;
        private bool PImprimir = false;
        #endregion

    }
}

using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        #region Bitacora Cambios
        private bool _BitacoraCambiosAdministrativaEnabled;
        public bool BitacoraCambiosAdministrativaEnabled
        {
            get { return _BitacoraCambiosAdministrativaEnabled; }
            set { _BitacoraCambiosAdministrativaEnabled = value; OnPropertyChanged("BitacoraCambiosAdministrativaEnabled"); }
        }

        private ObservableCollection<INGRESO_UBICACION_ANT> _ListUbicaciones;
        public ObservableCollection<INGRESO_UBICACION_ANT> ListUbicaciones
        {
            get { return _ListUbicaciones; }
            set { _ListUbicaciones = value; OnPropertyChanged("ListUbicaciones"); }
        }
        #endregion

        #region Medidas Disciplinarias
        private ObservableCollection<INCIDENTE> _ListIncidentesIngreso;
        public ObservableCollection<INCIDENTE> ListIncidentesIngreso
        {
            get { return _ListIncidentesIngreso; }
            set { _ListIncidentesIngreso = value; OnPropertyChanged("ListIncidentesIngreso"); }
        }

        private INCIDENTE _SelectedIncidente;
        public INCIDENTE SelectedIncidente
        {
            get { return _SelectedIncidente; }
            set
            {
                _SelectedIncidente = value;
                if (value != null)
                {
                    if (value.SANCION.Count > 0)
                        ListSancionesIncidente = new ObservableCollection<SANCION>(value.SANCION.OrderBy(o => o.INICIA_FEC).ThenBy(t => t.TERMINA_FEC));
                    else
                        ListSancionesIncidente = new ObservableCollection<SANCION>();

                    TextMotivoIncidente = value.MOTIVO;
                    FechaRegistroIncidente = value.REGISTRO_FEC.HasValue ? value.REGISTRO_FEC.Value.ToString() : string.Empty;
                    SelectTipoIncidente = value.ID_INCIDENTE_TIPO;
                }
                else
                {
                    TextMotivoIncidente = string.Empty;
                    FechaRegistroIncidente = string.Empty;
                    SelectTipoIncidente = new Nullable<short>();
                    ListSancionesIncidente = new ObservableCollection<SANCION>();
                }
                OnPropertyChanged("SelectedIncidente");
            }
        }

        private string _TextMotivoIncidente;
        public string TextMotivoIncidente
        {
            get { return _TextMotivoIncidente; }
            set { _TextMotivoIncidente = value; OnPropertyChanged("TextMotivoIncidente"); }
        }

        private string _FechaRegistroIncidente;
        public string FechaRegistroIncidente
        {
            get { return _FechaRegistroIncidente; }
            set { _FechaRegistroIncidente = value; OnPropertyChanged("FechaRegistroIncidente"); }
        }

        private ObservableCollection<INCIDENTE_TIPO> _ListTipoIncidente;
        public ObservableCollection<INCIDENTE_TIPO> ListTipoIncidente
        {
            get { return _ListTipoIncidente; }
            set { _ListTipoIncidente = value; OnPropertyChanged("ListTipoIncidente"); }
        }

        private short? _SelectTipoIncidente;
        public short? SelectTipoIncidente
        {
            get { return _SelectTipoIncidente; }
            set { _SelectTipoIncidente = value; OnPropertyChanged("SelectTipoIncidente"); }
        }

        private ObservableCollection<SANCION> _ListSancionesIncidente;
        public ObservableCollection<SANCION> ListSancionesIncidente
        {
            get { return _ListSancionesIncidente; }
            set { _ListSancionesIncidente = value; OnPropertyChanged("ListSancionesIncidente"); }
        }
        #endregion

        #region Excarcelaciones
        private ObservableCollection<EXCARCELACION> _ListExcarcelaciones;
        public ObservableCollection<EXCARCELACION> ListExcarcelaciones
        {
            get { return _ListExcarcelaciones; }
            set { _ListExcarcelaciones = value; OnPropertyChanged("ListExcarcelaciones"); }
        }

        private EXCARCELACION _SelectExcarcelacion;
        public EXCARCELACION SelectExcarcelacion
        {
            get { return _SelectExcarcelacion; }
            set
            {
                _SelectExcarcelacion = value;
                if (value != null)
                    GetExcarcelaciones(value);
                OnPropertyChanged("SelectExcarcelacion");
            }
        }

        private string _TextFechaHoraExcarcelacion;
        public string TextFechaHoraExcarcelacion
        {
            get { return _TextFechaHoraExcarcelacion; }
            set { _TextFechaHoraExcarcelacion = value; OnPropertyChanged("TextFechaHoraExcarcelacion"); }
        }

        private string _TextTipoExcarcelacion;
        public string TextTipoExcarcelacion
        {
            get { return _TextTipoExcarcelacion; }
            set { _TextTipoExcarcelacion = value; OnPropertyChanged("TextTipoExcarcelacion"); }
        }

        private string _TextOficioExcarcelacion;
        public string TextOficioExcarcelacion
        {
            get { return _TextOficioExcarcelacion; }
            set { _TextOficioExcarcelacion = value; OnPropertyChanged("TextOficioExcarcelacion"); }
        }

        private string _TextDestinoExcarcelacion;
        public string TextDestinoExcarcelacion
        {
            get { return _TextDestinoExcarcelacion; }
            set { _TextDestinoExcarcelacion = value; OnPropertyChanged("TextDestinoExcarcelacion"); }
        }

        private string _TextFechaReingresoExcarcelacion;
        public string TextFechaReingresoExcarcelacion
        {
            get { return _TextFechaReingresoExcarcelacion; }
            set { _TextFechaReingresoExcarcelacion = value; OnPropertyChanged("TextFechaReingresoExcarcelacion"); }
        }

        private string _TextObservacionesExcarcelacion;
        public string TextObservacionesExcarcelacion
        {
            get { return _TextObservacionesExcarcelacion; }
            set { _TextObservacionesExcarcelacion = value; OnPropertyChanged("TextObservacionesExcarcelacion"); }
        }

        private string _textJuzgado;
        public string TextJuzgado
        {
            get { return _textJuzgado; }
            set { _textJuzgado = value; OnPropertyChanged("TextJuzgado"); }
        }
        #endregion

        #region Traslados
        private bool _TrasladosAdministrativaEnabled;
        public bool TrasladosAdministrativaEnabled
        {
            get { return _TrasladosAdministrativaEnabled; }
            set { _TrasladosAdministrativaEnabled = value; OnPropertyChanged("TrasladosAdministrativaEnabled"); }
        }

        private ObservableCollection<TRASLADO> _ListTraslados;
        public ObservableCollection<TRASLADO> ListTraslados
        {
            get { return _ListTraslados; }
            set { _ListTraslados = value; OnPropertyChanged("ListTraslados"); }
        }

        private TRASLADO _SelectTraslados;
        public TRASLADO SelectTraslados
        {
            get { return _SelectTraslados; }
            set
            {
                _SelectTraslados = value;
                if (value != null)
                {
                    TextOficioTraslado = value.OFICIO_AUTORIZACION;
                    TextJustificacionTraslado = value.JUSTIFICACION;
                    TextAutorizadoTraslado = !string.IsNullOrWhiteSpace(value.AUTORIZA_TRASLADO) ? value.AUTORIZA_TRASLADO : string.Empty;
                    TextFechaTraslado = value.TRASLADO_FEC.ToString();
                    SelectCentroDestinoTraslado = value.CENTRO_DESTINO;
                    SelectMotivoTraslado = value.ID_MOTIVO;
                }
                else
                {
                    TextOficioTraslado = string.Empty;
                    TextJustificacionTraslado = string.Empty;
                    TextAutorizadoTraslado = string.Empty;
                    TextFechaTraslado = string.Empty;
                    SelectCentroDestinoTraslado = new Nullable<short>();
                    SelectMotivoTraslado = new Nullable<short>();
                }
                OnPropertyChanged("SelectTraslados");
            }
        }

        private string _TextOficioTraslado;
        public string TextOficioTraslado
        {
            get { return _TextOficioTraslado; }
            set { _TextOficioTraslado = value; OnPropertyChanged("TextOficioTraslado"); }
        }

        private string _TextJustificacionTraslado;
        public string TextJustificacionTraslado
        {
            get { return _TextJustificacionTraslado; }
            set { _TextJustificacionTraslado = value; OnPropertyChanged("TextJustificacionTraslado"); }
        }

        private string _TextAutorizadoTraslado;
        public string TextAutorizadoTraslado
        {
            get { return _TextAutorizadoTraslado; }
            set { _TextAutorizadoTraslado = value; OnPropertyChanged("TextAutorizadoTraslado"); }
        }

        private string _TextFechaTraslado;
        public string TextFechaTraslado
        {
            get { return _TextFechaTraslado; }
            set { _TextFechaTraslado = value; OnPropertyChanged("TextFechaTraslado"); }
        }

        private short? _SelectCentroDestinoTraslado;
        public short? SelectCentroDestinoTraslado
        {
            get { return _SelectCentroDestinoTraslado; }
            set { _SelectCentroDestinoTraslado = value; OnPropertyChanged("SelectCentroDestinoTraslado"); }
        }

        private short? _SelectMotivoTraslado;
        public short? SelectMotivoTraslado
        {
            get { return _SelectMotivoTraslado; }
            set { _SelectMotivoTraslado = value; OnPropertyChanged("SelectMotivoTraslado"); }
        }

        private ObservableCollection<TRASLADO_MOTIVO> _ListMotivoTraslado;
        public ObservableCollection<TRASLADO_MOTIVO> ListMotivoTraslado
        {
            get { return _ListMotivoTraslado; }
            set { _ListMotivoTraslado = value; OnPropertyChanged("ListMotivoTraslado"); }
        }

        private ObservableCollection<CENTRO> _ListCentroDestino;
        public ObservableCollection<CENTRO> ListCentroDestino
        {
            get { return _ListCentroDestino; }
            set { _ListCentroDestino = value; OnPropertyChanged("ListCentroDestino"); }
        }
        #endregion
    }
}

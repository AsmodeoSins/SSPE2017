using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    partial class ReporteKardexInternoViewModel : ValidationViewModelBase
    {

        IList<EmpalmeParticipante> listaemp = new List<EmpalmeParticipante>();
        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get
            {
                return imagenIngreso;
            }
            set
            {
                imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
                OnPropertyChanged("ImagenParticipante");
            }
        }
        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get
            {
                return imagenImputado;

            }
            set
            {
                imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
                OnPropertyChanged("ImagenParticipante");
            }
        }
        public byte[] ImagenParticipante
        {
            get
            {
                return imagenIngreso.Length != new Imagenes().getImagenPerson().Length ? imagenIngreso : imagenImputado.Length != new Imagenes().getImagenPerson().Length ? imagenImputado : new Imagenes().getImagenPerson();
            }
        }

        int _MaxValueProBar = 1;
        public int MaxValueProBar
        {
            get { return _MaxValueProBar; }
            set
            {
                _MaxValueProBar = value;
                OnPropertyChanged("MaxValueProBar");
            }
        }
        string _AvanceTratamiento = "0/0";
        public string AvanceTratamiento
        {
            get { return _AvanceTratamiento; }
            set
            {
                _AvanceTratamiento = value;
                OnPropertyChanged("AvanceTratamiento");
            }
        }
        string _HorasTratamiento = "0/0";
        public string HorasTratamiento
        {
            get { return _HorasTratamiento; }
            set
            {
                _HorasTratamiento = value;
                OnPropertyChanged("HorasTratamiento");
            }
        }
        private string _TextAnio;
        public string TextAnio
        {
            get { return _TextAnio; }
            set { _TextAnio = value; OnPropertyChanged("TextAnio"); }
        }
        private string _TextFolio;
        public string TextFolio
        {
            get { return _TextFolio; }
            set { _TextFolio = value; OnPropertyChanged("TextFolio"); }
        }
        private string _TextPaternoImputado;
        public string TextPaternoImputado
        {
            get { return _TextPaternoImputado; }
            set { _TextPaternoImputado = value; OnPropertyChanged("TextPaternoImputado"); }
        }
        private string _TextMaternoImputado;
        public string TextMaternoImputado
        {
            get { return _TextMaternoImputado; }
            set { _TextMaternoImputado = value; OnPropertyChanged("TextMaternoImputado"); }
        }
        private string _TextNombreImputado;
        public string TextNombreImputado
        {
            get { return _TextNombreImputado; }
            set { _TextNombreImputado = value; OnPropertyChanged("TextNombreImputado"); }
        }
        string _TextUbicacion;
        public string TextUbicacion
        {
            get { return _TextUbicacion; }
            set
            {
                _TextUbicacion = value;
                OnPropertyChanged("TextUbicacion");
            }
        }
        string _TextPlanimetria;
        public string TextPlanimetria
        {
            get { return _TextPlanimetria; }
            set
            {
                _TextPlanimetria = value;
                OnPropertyChanged("TextPlanimetria");
            }
        }
        string _TextSentencia;
        public string TextSentencia
        {
            get { return _TextSentencia; }
            set
            {
                _TextSentencia = value;
                OnPropertyChanged("TextSentencia");
            }
        }
        string _TextSentenciaRes;
        public string TextSentenciaRes
        {
            get { return _TextSentenciaRes; }
            set
            {
                _TextSentenciaRes = value;
                OnPropertyChanged("TextSentenciaRes");
            }
        }
        string _TextEstatus;
        public string TextEstatus
        {
            get { return _TextEstatus; }
            set
            {
                _TextEstatus = value;
                OnPropertyChanged("TextEstatus");
            }
        }
        private int? _AnioBuscar;
        public int? AnioBuscar
        {
            get { return _AnioBuscar; }
            set
            {
                _AnioBuscar = value;
                OnPropertyChanged("AnioBuscar");
            }
        }
        private int? _FolioBuscar;
        public int? FolioBuscar
        {
            get { return _FolioBuscar; }
            set
            {
                _FolioBuscar = value;
                OnPropertyChanged("FolioBuscar");
            }
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
        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                if (value.Count > 0)
                {
                    SelectExpediente = value.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).FirstOrDefault();
                }
                OnPropertyChanged("ListExpediente");
            }
        }
        public INGRESO selectIngreso { get; set; }
        private INGRESO SelectIngresoAuxiliar;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (value == null)
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    OnPropertyChanged("SelectIngreso");
                    return;
                }

                if (value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();

                if (value.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                    ImagenIngreso = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectIngreso");
            }
        }
        public IMPUTADO selectExpediente { get; set; }
        private IMPUTADO SelectExpedienteAuxiliar;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (value != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (value.INGRESO!=null && value.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = value.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
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
        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        private string _PaternoD;
        public string PaternoD
        {
            get
            {
                return _PaternoD;
            }
            set
            {
                _PaternoD = value;
                OnPropertyChanged("PaternoD");
            }
        }
        private string _MaternoD;
        public string MaternoD
        {
            get
            {
                return _MaternoD;
            }
            set
            {
                _MaternoD = value;
                OnPropertyChanged("MaternoD");
            }
        }
        private string _NombreD;
        public string NombreD
        {
            get
            {
                return _NombreD;
            }
            set
            {
                _NombreD = value;
                OnPropertyChanged("NombreD");
            }
        }
        private bool emptyIngresoVisible;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }
        string _Planimetriacolor;
        public string Planimetriacolor
        {
            get { return _Planimetriacolor; }
            set
            {
                _Planimetriacolor = value;
                OnPropertyChanged("Planimetriacolor");
            }
        }
        int _CantidadActividadesAprovadas = 0;
        public int CantidadActividadesAprovadas
        {
            get { return _CantidadActividadesAprovadas; }
            set
            {
                _CantidadActividadesAprovadas = value;
                OnPropertyChanged("CantidadActividadesAprovadas");
            }
        }
        private ObservableCollection<IMPUTADO> listExpedientePadron;
        public ObservableCollection<IMPUTADO> ListExpedientePadron
        {
            get { return listExpedientePadron; }
            set { listExpedientePadron = value; }
        }
        private ObservableCollection<IMPUTADO> listExpedienteAsignacion;
        public ObservableCollection<IMPUTADO> ListExpedienteAsignacion
        {
            get { return listExpedienteAsignacion; }
            set { listExpedienteAsignacion = value; }
        }
        private string textBotonSeleccionarIngreso = "Seleccionar Ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }

        private bool selectIngresoEnabled = true;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }


        private Visibility reportViewerVisible = Visibility.Visible;
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

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}

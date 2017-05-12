using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Windows;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {
        #region [Situacion Juridica]
        private EMI_SITUACION_JURIDICA EMI_SITUACION_JURIDICA = new EMI_SITUACION_JURIDICA();

        #region [Situacion Juridica]
        //public string SentenciaAño
        //{
        //  get { return _SentenciaAño; }
        //    set
        //    {
        //        _SentenciaAño = value;
        //        OnPropertyChanged("SentenciaAño");
        //    }
        //}
        //public string SentenciaMes
        //{
        //    get { return _SentenciaMes; }
        //    set
        //    {
        //        _SentenciaMes = value;
        //        OnPropertyChanged("SentenciaMes");
        //    }
        //}
        //public string SentenciaDia
        //{
        //    get { return _SentenciaDia; }
        //    set { _SentenciaDia = value; OnPropertyChanged("SentenciaDia"); }
        //}
        //public string CompurgadoAño
        //{
        //    get { return _CompurgadoAño; }
        //    set
        //    {
        //        _CompurgadoAño = value;
        //        OnPropertyChanged("CompurgadoAño");
        //    }
        //}
        //public string CompurgadoMes
        //{
        //    get { return _CompurgadoMes; }
        //    set
        //    {
        //        _CompurgadoMes = value;
        //        OnPropertyChanged("CompurgadoMes");
        //    }
        //}
        //public string CompurgadoDia
        //{
        //    get { return _CompurgadoDia; }
        //    set
        //    {
        //        _CompurgadoDia = value;
        //        OnPropertyChanged("CompurgadoDia");
        //    }
        //}
        //public string CompurgarAño
        //{
        //    get { return _CompurgarAño; }
        //    set
        //    {
        //        _CompurgarAño = value;
        //        OnPropertyChanged("CompurgarAño");
        //    }
        //}
        //public string CompurgarMes
        //{
        //    get { return _CompurgarMes; }
        //    set
        //    {
        //        _CompurgarMes = value;
        //        OnPropertyChanged("CompurgarMes");
        //    }
        //}
        //public string CompurgarDia
        //{
        //    get { return _CompurgarDia; }
        //    set
        //    {
        //        _CompurgarDia = value;
        //        OnPropertyChanged("CompurgarDia");
        //    }
        //}
        #endregion
        #region [Delito(s)]
        //public string Delitos
        //{
        //    get { return _Delitos; }
        //    set
        //    {
        //        _Delitos = value;
        //        OnPropertyChanged("Delitos");
        //    }
        //}
        #endregion
        #region [Estudios y Traslados]
        #region [Estudios y Traslados]
        public string VersionDelito
        {
            get { return EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO; }
            set
            {
                EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO = value;
                OnPropertyChanged("VersionDelito");
            }
        }
        #region [Periodo de Libertad Entre Ingresos]
        private string menorPeriodoLibreReingreso;
        public string MenorPeriodoLibreReingreso
        {
            get { return menorPeriodoLibreReingreso; }
            set { menorPeriodoLibreReingreso = value; OnPropertyChanged("MenorPeriodoLibreReingreso"); }
        }
        private string mayorPeriodoLibreReingreso;

        public string MayorPeriodoLibreReingreso
        {
            get { return mayorPeriodoLibreReingreso; }
            set { mayorPeriodoLibreReingreso = value; OnPropertyChanged("MayorPeriodoLibreReingreso"); }
        }
        public bool PracticadoEstudios
        {
            get { return EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS == "S" ? true : false; }
            set
            {
                EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS = value == true ? "S" : "N";
                if (value)
                    IsCuandoEstudiosEnabled = true;
                else
                    IsCuandoEstudiosEnabled = false;
                OnPropertyChanged("IsCuandoEstudiosEnabled");
                if (TabSituacionJuridicaSelected && SelectIngreso != null)
                    setValidacionesEstudiosTraslado();
                OnPropertyChanged("PracticadoEstudios");
            }
        }
        public string Cuando
        {
            get { return EMI_SITUACION_JURIDICA.CUANDO_PRACT_ESTUDIOS; }
            set
            {
                EMI_SITUACION_JURIDICA.CUANDO_PRACT_ESTUDIOS = value;
                OnPropertyChanged("Cuando");

            }
        }
        public bool Traslado
        {
            get { return EMI_SITUACION_JURIDICA.DESEA_TRASLADO == "S" ? true : false; }
            set
            {
                EMI_SITUACION_JURIDICA.DESEA_TRASLADO = value == true ? "S" : "N";
                OnPropertyChanged("Traslado");
                if (value)
                    IsDeseaTrasladoEnabled = true;
                else
                    IsDeseaTrasladoEnabled = false;
                if (TabSituacionJuridicaSelected && SelectIngreso != null)
                    setValidacionesEstudiosTraslado();
                OnPropertyChanged("IsDeseaTrasladoEnabled");
            }
        }
        //public bool TrasladoNO
        //{
        //    get { return EMI_SITUACION_JURIDICA.DESEA_TRASLADO == "S" ? false : true; }
        //    set
        //    {
        //        EMI_SITUACION_JURIDICA.DESEA_TRASLADO = value == true ? "N" : "S";
        //        OnPropertyChanged("TrasladoNO");
        //    }
        //}
        public string Donde
        {
            get { return EMI_SITUACION_JURIDICA.DONDE_DESEA_TRASLADO; }
            set
            {
                EMI_SITUACION_JURIDICA.DONDE_DESEA_TRASLADO = value;
                OnPropertyChanged("Donde");
            }
        }
        public string PorqueMotivo
        {
            get { return EMI_SITUACION_JURIDICA.MOTIVO_DESEA_TRASLADO; }
            set
            {
                EMI_SITUACION_JURIDICA.MOTIVO_DESEA_TRASLADO = value;
                OnPropertyChanged("PorqueMotivo");
            }
        }
        #endregion
        #endregion
        #region [Ingreso Anteriores CE.RE.SO]
        //public string IACereso
        //{
        //    get { return _IACereso; }
        //    set
        //    {
        //        _IACereso = value;
        //        OnPropertyChanged("IACereso");
        //    }
        //}
        //public string Delito
        //{
        //    get { return _Delito; }
        //    set
        //    {
        //        _Delito = value;
        //        OnPropertyChanged("Delito");
        //    }
        //}
        //public string PeriosoReclusion
        //{
        //    get { return _PeriosoReclusion; }
        //    set
        //    {
        //        _PeriosoReclusion = value;
        //        OnPropertyChanged("PeriosoReclusion");
        //    }
        //}
        //public string Sanciones
        //{
        //    get { return _Sanciones; }
        //    set
        //    {
        //        _Sanciones = value;
        //        OnPropertyChanged("Sanciones");
        //    }
        //}
        #endregion
        #region [Ingreso Anteriores Menores]
        #endregion
        #endregion
        #region Enabling/Disabling Description Fields

        private bool _IsCuandoEstudiosEnabled;
        public bool IsCuandoEstudiosEnabled
        {
            get { return _IsCuandoEstudiosEnabled; }
            set
            {
                _IsCuandoEstudiosEnabled = value;
                OnPropertyChanged("IsCuandoEstudiosEnabled");
                if (!value)
                {
                    Cuando = string.Empty;
                    OnPropertyChanged("Cuando");
                }
            }
        }
        private bool _IsDeseaTrasladoEnabled;
        public bool IsDeseaTrasladoEnabled
        {
            get { return _IsDeseaTrasladoEnabled; }
            set
            {
                _IsDeseaTrasladoEnabled = value;
                OnPropertyChanged("IsDeseaTrasladoEnabled");
                if (!value)
                {
                    Donde = string.Empty;
                    OnPropertyChanged("Donde");
                    PorqueMotivo = string.Empty;
                    OnPropertyChanged("PorqueMotivo");
                }
            }
        }
        #endregion

        //INGRESOS ANTERIORES CERESO
        private ObservableCollection<DELITO> lstDelitosCP;
        public ObservableCollection<DELITO> LstDelitosCP
        {
            get { return lstDelitosCP; }
            set { lstDelitosCP = value; OnPropertyChanged("LstDelitosCP"); }

        }

        private ObservableCollection<EMI_INGRESO_ANTERIOR> lstIngresosAnteriores;
        public ObservableCollection<EMI_INGRESO_ANTERIOR> LstIngresosAnteriores
        {
            get { return lstIngresosAnteriores; }
            set
            {
                lstIngresosAnteriores = value;
                OnPropertyChanged("LstIngresosAnteriores");
            }
        }

        private ObservableCollection<EMI_INGRESO_ANTERIOR> lstIngresosAnterioresMenor;
        public ObservableCollection<EMI_INGRESO_ANTERIOR> LstIngresosAnterioresMenor
        {
            get { return lstIngresosAnterioresMenor; }
            set
            {
                lstIngresosAnterioresMenor = value;
                if (lstIngresosAnterioresMenor != null)
                {
                    if (lstIngresosAnterioresMenor.Count > 0)
                        EmptyIngresosAnterioresMenores = false;
                    else
                        EmptyIngresosAnterioresMenores = true;
                }
                else
                    EmptyIngresosAnterioresMenores = true;
                OnPropertyChanged("LstIngresosAnterioresMenor");
            }
        }

        private EMI_INGRESO_ANTERIOR selectedIngresoAnterior;
        public EMI_INGRESO_ANTERIOR SelectedIngresoAnterior
        {
            get { return selectedIngresoAnterior; }
            set
            {
                selectedIngresoAnterior = value;
                OnPropertyChanged("SelectedIngresoAnterior");
                if (SelectedIngresoAnterior != null)
                {
                    EliminarItemMenu = SelectedIngresoAnterior.ID_EMI != 0 ? false : true;
                    //var isInDB = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIIngresosAnteriores().GetData(w => w.ID_EMI == emiActual.ID_EMI && w.ID_EMI_CONS == emiActual.ID_EMI_CONS && w.ID_TIPO == SelectedIngresoAnterior.ID_TIPO && w.ID_CONSEC == SelectedIngresoAnterior.ID_CONSEC);
                    //EliminarItemMenu = isInDB.Count() > 0 ? false : true;
                }
            }
        }

        private EMI_INGRESO_ANTERIOR selectedIngresoAnteriorMenor;
        public EMI_INGRESO_ANTERIOR SelectedIngresoAnteriorMenor
        {
            get { return selectedIngresoAnteriorMenor; }
            set
            {
                selectedIngresoAnteriorMenor = value;
                OnPropertyChanged("SelectedIngresoAnteriorMenor");
                if (SelectedIngresoAnteriorMenor != null)
                EliminarItemMenu = SelectedIngresoAnteriorMenor.ID_EMI != 0 ? false : true;
            }
        }

        //POP
        private EMISOR selectedEmisorIngreso;
        public EMISOR SelectedEmisorIngreso
        {
            get { return selectedEmisorIngreso; }
            set { selectedEmisorIngreso = value; OnPropertyChanged("SelectedEmisorIngreso"); }
        }

        private DELITO selectedDelitoIngreso;
        public DELITO SelectedDelitoIngreso
        {
            get { return selectedDelitoIngreso; }
            set { selectedDelitoIngreso = value; OnPropertyChanged("SelectedDelitoIngreso"); }
        }

        private string delitoDescripcion;
        public string DelitoDescripcion
        {
            get { return delitoDescripcion; }
            set { delitoDescripcion = value; OnPropertyChanged("DelitoDescripcion"); }
        }

        private string periodoReclusionIngreso;
        public string PeriodoReclusionIngreso
        {
            get { return periodoReclusionIngreso; }
            set { periodoReclusionIngreso = value; OnPropertyChanged("PeriodoReclusionIngreso"); }
        }

        private string sancionesIngreso;
        public string SancionesIngreso
        {
            get { return sancionesIngreso; }
            set { sancionesIngreso = value; OnPropertyChanged("SancionesIngreso"); }
        }


        private bool emptyIngresosAnteriores;
        public bool EmptyIngresosAnteriores
        {
            get { return emptyIngresosAnteriores; }
            set { emptyIngresosAnteriores = value; OnPropertyChanged("EmptyIngresosAnteriores"); }
        }

        private bool emptyIngresosAnterioresMenores;
        public bool EmptyIngresosAnterioresMenores
        {
            get { return emptyIngresosAnterioresMenores; }
            set { emptyIngresosAnterioresMenores = value; OnPropertyChanged("EmptyIngresosAnterioresMenores"); }
        }

        private short? tipoIngresoAnterior;
        public short? TipoIngresoAnterior
        {
            get { return tipoIngresoAnterior; }
            set { tipoIngresoAnterior = value; OnPropertyChanged("TipoIngresoAnterior"); }
        }
        #endregion


        #region TABSELECTED
        private bool tabSituacionJuridicaSelected;
        public bool TabSituacionJuridicaSelected
        {
            get { return tabSituacionJuridicaSelected; }
            set
            {
                tabSituacionJuridicaSelected = value; OnPropertyChanged("TabSituacionJuridicaSelected");
            }
        }

        private bool tabEstudioTrasladoSelected;
        public bool TabEstudioTrasladoSelected
        {
            get { return tabEstudioTrasladoSelected; }
            set { tabEstudioTrasladoSelected = value; OnPropertyChanged("TabEstudioTrasladoSelected"); }
        }

        private bool tabIngresoAnteriorSelected;
        public bool TabIngresoAnteriorSelected
        {
            get { return tabIngresoAnteriorSelected; }
            set { tabIngresoAnteriorSelected = value; OnPropertyChanged("TabIngresoAnteriorSelected"); }
        }

        private bool tabIngresoAnteriorMenorSelected;
        public bool TabIngresoAnteriorMenorSelected
        {
            get { return tabIngresoAnteriorMenorSelected; }
            set { tabIngresoAnteriorMenorSelected = value; OnPropertyChanged("TabIngresoAnteriorMenorSelected"); }
        }
        #endregion

        #region [INGRESOS ANTERIORES SISTEMA]
        private ObservableCollection<Clases.IngresoAinterior> lstIAS;
        public ObservableCollection<Clases.IngresoAinterior> LstIAS
        {
            get { return lstIAS; }
            set { lstIAS = value; OnPropertyChanged("LstIAS"); }
        }

        private bool isIngresoAntAll;
        public bool IsIngresoAntAll
        {
            get { return isIngresoAntAll; }
            set
            {
                isIngresoAntAll = value;
                OnPropertyChanged("IsIngresoAntAll");
                foreach (var ingreso in LstIAS)
                {
                    ingreso.Seleccione = value;
                }
                LstIAS = new ObservableCollection<Clases.IngresoAinterior>(LstIAS);
            }
        }

        private bool emptyIAS;
        public bool EmptyIAS
        {
            get { return emptyIAS; }
            set { emptyIAS = value; OnPropertyChanged("EmptyIAS"); }
        }

        //INGRESOS ANTERIORES MENORES
        private Visibility _ShowFingerPrint = Visibility.Hidden;
        public Visibility ShowFingerPrint
        {
            get { return _ShowFingerPrint; }
            set
            {
                _ShowFingerPrint = value;
                OnPropertyChanged("ShowFingerPrint");
            }
        }
        #endregion


        //SENTENCIA
        private int? aniosS = 0;
        public int? AniosS
        {
            get { return aniosS; }
            set { aniosS = value; OnPropertyChanged("AniosS"); }
        }

        private int? mesesS = 0;
        public int? MesesS
        {
            get { return mesesS; }
            set { mesesS = value; OnPropertyChanged("MesesS"); }
        }

        private int? diasS = 0;
        public int? DiasS
        {
            get { return diasS; }
            set { diasS = value; OnPropertyChanged("DiasS"); }
        }
        //COMPURGADOS
        private int? aniosC = 0;
        public int? AniosC
        {
            get { return aniosC; }
            set { aniosC = value; OnPropertyChanged("AniosC"); }
        }

        private int? mesesC = 0;
        public int? MesesC
        {
            get { return mesesC; }
            set { mesesC = value; OnPropertyChanged("MesesC"); }
        }

        private int? diasC = 0;
        public int? DiasC
        {
            get { return diasC; }
            set { diasC = value; OnPropertyChanged("DiasC"); }
        }

        //POR COMPURGADOS
        private int? aniosPC = 0;
        public int? AniosPC
        {
            get { return aniosPC; }
            set { aniosPC = value; OnPropertyChanged("AniosPC"); }
        }

        private int? mesesPC = 0;
        public int? MesesPC
        {
            get { return mesesPC; }
            set { mesesPC = value; OnPropertyChanged("MesesPC"); }
        }

        private int? diasPC = 0;
        public int? DiasPC
        {
            get { return diasPC; }
            set { diasPC = value; OnPropertyChanged("DiasPC"); }
        }

        private string delitos;
        public string Delitos
        {
            get { return delitos; }
            set { delitos = value; OnPropertyChanged("Delitos"); }
        }
    }
}

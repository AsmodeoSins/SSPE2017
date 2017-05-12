using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class ProgramaLibertadViewModel
    {

        #region Programa Libertad
        private ObservableCollection<PROGRAMA_LIBERTAD> lstProgramaLibertad;
        public ObservableCollection<PROGRAMA_LIBERTAD> LstProgramaLibertad
        {
            get { return lstProgramaLibertad; }
            set { 
                lstProgramaLibertad = value;
                if (value == null)
                    EmptyVisible = false;
                else
                {
                    EmptyVisible = value.Count > 0 ? false : true;
                }
                OnPropertyChanged("LstProgramaLibertad"); }
        }

        private PROGRAMA_LIBERTAD _ProgramaLibertad = new PROGRAMA_LIBERTAD();
        public PROGRAMA_LIBERTAD ProgramaLibertad
        {
            get { return _ProgramaLibertad; }
            set { 
                _ProgramaLibertad = value;
                if (value == null)
                {
                    EstatusPL = "A";
                    DescrPL = ObjetivoPL = string.Empty;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                }
                else
                {
                    EditarMenuEnabled = true;
                    #region Detalle
                    DescrPL = value.DESCR;
                    ObjetivoPL = value.OBJETIVO;
                    EstatusPL = value.ESTATUS;
                    LstActividadPrograma = new ObservableCollection<ACTIVIDAD_PROGRAMA>(value.ACTIVIDAD_PROGRAMA);
                    #endregion
                }
                OnPropertyChanged("ProgramaLibertad");
            }
        }

        private string _DescrPL;
        public string DescrPL
        {
            get { return _DescrPL; }
            set {
                if (ProgramaLibertad != null)
                    _DescrPL = ProgramaLibertad.DESCR = value;
                else
                    _DescrPL = value;
                    OnPropertyChanged("DescrPL");
            }
        }
        private string _ObjetivoPL;
        public string ObjetivoPL
        {
            get { return _ObjetivoPL; }
            set
            {
                if (ProgramaLibertad != null)
                    _ObjetivoPL = ProgramaLibertad.OBJETIVO = value;
                else
                    _ObjetivoPL = value;

                OnPropertyChanged("ObjetivoPL");
            }
        }
        private string _EstatusPL = "A";
        public string EstatusPL
        {
            get { return _EstatusPL; }
            set
            {
                if (ProgramaLibertad != null)
                {
                    if (string.IsNullOrEmpty(value))
                        _EstatusPL = ProgramaLibertad.ESTATUS = "A";
                    else
                        _EstatusPL = ProgramaLibertad.ESTATUS = value;
                }
                else
                    if (string.IsNullOrEmpty(value))
                        _EstatusPL = "A";
                    else
                        _EstatusPL = value;
                
                OnPropertyChanged("EstatusPL");
            }
        }

        #endregion

        #region Actividad Programa
        private ObservableCollection<ACTIVIDAD_PROGRAMA> lstActividadPrograma;
        public ObservableCollection<ACTIVIDAD_PROGRAMA> LstActividadPrograma
        {
            get { return lstActividadPrograma; }
            set { lstActividadPrograma = value; OnPropertyChanged("LstActividadPrograma"); }
        }

        private ACTIVIDAD_PROGRAMA _ActividadPrograma = new ACTIVIDAD_PROGRAMA();
        public ACTIVIDAD_PROGRAMA ActividadPrograma
        {
            get { return _ActividadPrograma; }
            set { 
                _ActividadPrograma = value;
                if (value == null)
                {
                    EstatusAP = "A";
                    DescrAP = string.Empty;
                }
                else
                {
                    EstatusAP = value.ESTATUS;
                    DescrAP = value.DESCR;
                }
                OnPropertyChanged("ActividadPrograma");
            }
        }

        private string _DescrAP;
        public string DescrAP
        {
            get { return _DescrAP; }
            set
            {
                if (ActividadPrograma != null)
                    _DescrAP = ActividadPrograma.DESCR = value;
                else
                    _DescrAP = value;
                    OnPropertyChanged("DescrAP");
                
            }
        }
        private string _EstatusAP = "A";
        public string EstatusAP
        {
            get { return _EstatusAP; }
            set
            {
                if (ActividadPrograma != null)
                {
                    if (string.IsNullOrEmpty(value))
                        _EstatusAP = ActividadPrograma.ESTATUS = "A";
                    else
                       _EstatusAP = ActividadPrograma.ESTATUS = value;
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                        _EstatusAP = "A";
                    else
                        _EstatusAP = value;
                }
                    OnPropertyChanged("EstatusAP");
                
            }
        }
        #endregion

        #region Buscar
        private string _busqueda;
        public string Busqueda
        {
            get { return _busqueda; }
            set { _busqueda = value; OnPropertyChanged("Busqueda"); }
        }
        #endregion

        #region Pantalla
        private bool _emptyVisible;
        public bool EmptyVisible
        {
            get { return _emptyVisible; }
            set { _emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }

        public bool bandera_editar = false;

        private string _cambio;
        public string Cambio
        {
            get { return _cambio; }
            set { _cambio = value; OnPropertyChanged("Cambio"); }
        }

        private string _catalogHeader;
        public string CatalogoHeader
        {
            get { return _catalogHeader; }
            set { _catalogHeader = value; OnPropertyChanged("CatalogoHeader"); }
        }

        private string _headerAgregar;
        public string HeaderAgregar
        {
            get { return _headerAgregar; }
            set { _headerAgregar = value; OnPropertyChanged("HeaderAgregar"); }
        }

        private int _seleccionIndice;
        public int SeleccionIndice
        {
            get { return _seleccionIndice; }
            set { _seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }

        private bool _guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return _guardarMenuEnabled; }
            set { _guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }
        #endregion

        #region Menu
        private bool _cancelarMenuEnabled;
        public bool CancelarMenuEnabled
        {
            get { return _cancelarMenuEnabled; }
            set { _cancelarMenuEnabled = value; OnPropertyChanged("CancelarMenuEnabled"); }
        }

        private bool _exportarMenuEnabled;
        public bool ExportarMenuEnabled
        {
            get { return _exportarMenuEnabled; }
            set { _exportarMenuEnabled = value; OnPropertyChanged("ExportarMenuEnabled"); }
        }

        private bool _salirMenuEnabled;
        public bool SalirMenuEnabled
        {
            get { return _salirMenuEnabled; }
            set { _salirMenuEnabled = value; OnPropertyChanged("SalirMenuEnabled"); }
        }

        private bool _ayudaMenuEnabled;
        public bool AyudaMenuEnabled
        {
            get { return _ayudaMenuEnabled; }
            set { _ayudaMenuEnabled = value; OnPropertyChanged("AyudaMenuEnabled"); }
        }

        private bool _nuevoVisible;
        public bool NuevoVisible
        {
            get { return _nuevoVisible; }
            set { _nuevoVisible = value; OnPropertyChanged("NuevoVisible"); }
        }
        #endregion

        #region [CONFIGURACION PERMISOS]
        private bool _agregarMenuEnabled;
        public bool AgregarMenuEnabled
        {
            get { return _agregarMenuEnabled; }
            set { _agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }

        private bool _editarMenuEnabled;
        public bool EditarMenuEnabled
        {
            get { return _editarMenuEnabled; }
            set { _editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }

        private bool _editarEnabled;
        public bool EditarEnabled
        {
            get { return _editarEnabled; }
            set { _editarEnabled = value; OnPropertyChanged("EditarEnabled"); }
        }

        private bool _eliminarMenuEnabled;
        public bool EliminarMenuEnabled
        {
            get { return _eliminarMenuEnabled; }
            set { _eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }

        private bool _agregarVisible;
        public bool AgregarVisible
        {
            get { return _agregarVisible; }
            set { _agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }

        private bool _editarVisible;
        public bool EditarVisible
        {
            get { return _editarVisible; }
            set { _editarVisible = value; OnPropertyChanged("EditarVisible"); }
        }

        private bool _textoHabilitado;
        public bool TextoHabilitado
        {
            get { return _textoHabilitado; }
            set { _textoHabilitado = value; OnPropertyChanged("TextoHabilitado"); }
        }

        private bool _buscarHabilitado;
        public bool BuscarHabilitado
        {
            get { return _buscarHabilitado; }
            set { _buscarHabilitado = value; OnPropertyChanged("BuscarHabilitado"); }
        }
        #endregion

    }
}
using ControlPenales.Clases;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
//using MvvmFramework;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        #region Datos Ingreso
        #region Listas
        private List<TIPO_INGRESO> listTipoIngreso;
        public List<TIPO_INGRESO> ListTipoIngreso
        {
            get { return listTipoIngreso; }
            set { listTipoIngreso = value;
            OnPropertyChanged("ListTipoIngreso"); }
        }
        private List<CLASIFICACION_JURIDICA> listClasificacionJuridica;
        public List<CLASIFICACION_JURIDICA> ListClasificacionJuridica
        {
            get { return listClasificacionJuridica; }
            set { listClasificacionJuridica = value; OnPropertyChanged("ListClasificacionJuridica"); }
        }
        private List<ESTATUS_ADMINISTRATIVO> listEstatusAdministrativo;
        public List<ESTATUS_ADMINISTRATIVO> ListEstatusAdministrativo
        {
            get { return listEstatusAdministrativo; }
            set { listEstatusAdministrativo = value; OnPropertyChanged("ListEstatusAdministrativo"); }
        }
        private List<TIPO_DELITO> listTipoDelito;
        public List<TIPO_DELITO> ListTipoDelito
        {
            get { return listTipoDelito; }
            set { listTipoDelito = value; OnPropertyChanged("ListTipoDelito"); }
        }
        private List<TIPO_AUTORIDAD_INTERNA> listTipoAutoridadInterna;
        public List<TIPO_AUTORIDAD_INTERNA> ListTipoAutoridadInterna
        {
            get { return listTipoAutoridadInterna; }
            set { listTipoAutoridadInterna = value; OnPropertyChanged("ListTipoAutoridadInterna"); }
        }
        private List<TIPO_DISPOSICION> listTipoDisposicion;
        public List<TIPO_DISPOSICION> ListTipoDisposicion
        {
            get { return listTipoDisposicion; }
            set { listTipoDisposicion = value; OnPropertyChanged("ListTipoDisposicion"); }
        }
        private List<TIPO_SEGURIDAD> listTipoSeguridad;
        public List<TIPO_SEGURIDAD> ListTipoSeguridad
        {
            get { return listTipoSeguridad; }
            set { listTipoSeguridad = value; OnPropertyChanged("ListTipoSeguridad"); }
        }
        private ObservableCollection<SECTOR> listSector;
        public ObservableCollection<SECTOR> ListSector
        {
            get { return listSector; }
            set { listSector = value; OnPropertyChanged("ListSector"); }
        }
        private ObservableCollection<CELDA> listCelda;
        public ObservableCollection<CELDA> ListCelda
        {
            get { return listCelda; }
            set { listCelda = value; OnPropertyChanged("ListCelda"); }
        }
        #endregion

        #region Selects
        private short? selectTipoAutoridadInterna;
        public short? SelectTipoAutoridadInterna
        {
            get { return selectTipoAutoridadInterna; }
            set
            {
                selectTipoAutoridadInterna = value;
                
                OnPropertyChanged("SelectTipoAutoridadInterna");
            }
        }
        private short? selectTipoDisposicion;
        public short? SelectTipoDisposicion
        {
            get { return selectTipoDisposicion; }
            set
            {
                selectTipoDisposicion = value;
                
                OnPropertyChanged("SelectTipoDisposicion");
            }
        }
        private string selectTipoSeguridad;
        public string SelectTipoSeguridad
        {
            get { return selectTipoSeguridad; }
            set
            {
                selectTipoSeguridad = value;
                
                OnPropertyChanged("SelectTipoSeguridad");
            }
        }
        private string selectCelda;
        public string SelectCelda
        {
            get { return selectCelda; }
            set
            { selectCelda = value; OnPropertyChanged("SelectCelda"); }
        }
        private bool editarIngreso;
        public bool EditarIngreso
        {
            get { return editarIngreso; }
            set
            { editarIngreso = value; OnPropertyChanged("EditarIngreso"); }
        }
        private bool editarImputado;
        public bool EditarImputado
        {
            get { return editarImputado; }
            set { editarImputado = value; OnPropertyChanged("EditarImputado"); }
        }
        #endregion

        #region [SELECTDATOSINGRESO]
        private short? selectTipoIngreso;
        public short? SelectTipoIngreso
        {
            get { return selectTipoIngreso; }
            set
            {
                selectTipoIngreso = value;
                //Cambio para cambiar la pantalla de Traslado. Salvador. 12/2/2015
                //if (value == 3)
                //    TrasladoEnabled = true;
                //else
                //    TrasladoEnabled = false;
                if (value == Parametro.TRASLADO_FOREANO_TIPO_INGRESO)
                {
                    AgregarValidacionesTraslado();
                    TrasladoEnabled = true; 
                }
                else
                {
                    RemoveValidacionesTraslado();
                    TrasladoEnabled = false; 
                }
                OnPropertyValidateChanged("SelectTipoIngreso");
            }
        }
     
        private string selectClasificacionJuridica;
        public string SelectClasificacionJuridica
        {
            get { return selectClasificacionJuridica; }
            set
            {
                selectClasificacionJuridica = value;
                OnPropertyValidateChanged("SelectClasificacionJuridica");
            }
        }
        
        private short? selectEstatusAdministrativo;
        public short? SelectEstatusAdministrativo
        {
            get { return selectEstatusAdministrativo; }
            set
            {
                selectEstatusAdministrativo = value;
                OnPropertyValidateChanged("SelectEstatusAdministrativo");
            }
        }
        
        private short selectTipoDelito;
        public short SelectTipoDelito
        {
            get { return selectTipoDelito; }
            set
            {
                selectTipoDelito = value;
                OnPropertyValidateChanged("SelectTipoDelito");
            }
        }
        #endregion

        #region [TEXTDATOSINGRESO]
        private DateTime? fechaCeresoIngreso = Fechas.GetFechaDateServer;
        public DateTime? FechaCeresoIngreso
        {
            get { return fechaCeresoIngreso; }
            set
            {
                fechaCeresoIngreso = value;
                OnPropertyValidateChanged("FechaCeresoIngreso");
            }
        }
       
        private DateTime? fechaRegistroIngreso = Fechas.GetFechaDateServer;
        public DateTime? FechaRegistroIngreso
        {
            get { return fechaRegistroIngreso; }
            set
            {
                fechaRegistroIngreso = value;
                OnPropertyValidateChanged("FechaRegistroIngreso");
            }
        }
        
        private string textNumeroOficio;
        public string TextNumeroOficio
        {
            get { return textNumeroOficio; }
            set
            {
                textNumeroOficio = value;
                OnPropertyValidateChanged("TextNumeroOficio");
            }
        }
        #endregion

        #region [CENTROS]
        private CENTRO centro;
        public CENTRO Centro
        {
            get { return centro; }
            set { centro = value; OnPropertyChanged("Centro"); }
        }

        private CAMA selectedCama;
        public CAMA SelectedCama
        {
            get { return selectedCama; }
            set
            {
                selectedCama = value;
                
                OnPropertyChanged("SelectedCama");
            }
        }
        #endregion

        #region [UBICACION]
        private List<TreeViewList> _TreeList;
        public List<TreeViewList> TreeList
        {
            get { return _TreeList; }
            set
            {
                _TreeList = value;
                OnPropertyChanged("TreeList");
            }
        }

        private TreeViewList _SelectedItem;
        public TreeViewList SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; }
        }
        #endregion

        private bool isExpanded = true;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        //DELITOS
        private ObservableCollection<INGRESO_DELITO> ingresoDelitos;
        public ObservableCollection<INGRESO_DELITO> IngresoDelitos
        {
            get { return ingresoDelitos; }
            set { ingresoDelitos = value; OnPropertyChanged("IngresoDelitos"); }
        }

        private short ingresoDelito;
        public short IngresoDelito
        {
            get { return ingresoDelito; }
            set
            {
                ingresoDelito = value;
                OnPropertyValidateChanged("IngresoDelito");
            }
        }
        
        private Visibility noControlProceso = Visibility.Hidden;
        public Visibility NoControlProceso
        {
            get { return noControlProceso; }
            set { noControlProceso = value; OnPropertyChanged("NoControlProceso"); }
        }
        
        private ObservableCollection<INGRESO> listIngresosExpediente;
        public ObservableCollection<INGRESO> ListIngresosExpediente
        {
            get { return listIngresosExpediente; }
            set { listIngresosExpediente = value; OnPropertyChanged("ListIngresosExpediente"); }
        }
        #endregion
    }
}

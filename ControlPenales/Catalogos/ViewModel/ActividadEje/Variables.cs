using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoActividadEjeViewModel 
    {
        

        private Estatus _selectedEstatus = null;
        public Estatus SelectedEstatus
        {
            get { return _selectedEstatus; }
            set { _selectedEstatus = value; OnPropertyValidateChanged("SelectedEstatus"); }
        }

        private EstatusControl _lista_Estatus = new EstatusControl();
        public EstatusControl Lista_Estatus
        {
            get { return _lista_Estatus; }
            set { _lista_Estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private ObservableCollection<ACTIVIDAD> lstActividades;
        public ObservableCollection<ACTIVIDAD> LstActividades
        {
            get { return lstActividades; }
            set { lstActividades = value; RaisePropertyChanged("LstActividades"); }
        }

        private ACTIVIDAD selectedActividad;
        public ACTIVIDAD SelectedActividad
        {
            get { return selectedActividad; }
            set { selectedActividad = value; OnPropertyValidateChanged("SelectedActividad"); }
        }

        private ObservableCollection<EJE> lstEjes;
        public ObservableCollection<EJE> LstEjes
        {
            get { return lstEjes; }
            set { lstEjes = value; RaisePropertyChanged("LstEjes"); }
        }

        private short selectedEje = -1;
        public short SelectedEje
        {
            get { return selectedEje; }
            set { selectedEje = value; OnPropertyValidateChanged ("SelectedEje"); }
        }
       
        #region Busqueda
        private short selectedEjeBusqueda = -1;
        public short SelectedEjeBusqueda
        {
            get { return selectedEjeBusqueda; }
            set { selectedEjeBusqueda = value; RaisePropertyChanged("SelectedEjeBusqueda"); }
        }
        #endregion

        #region Grid
        private ObservableCollection<ACTIVIDAD_EJE> _listItems;
        public ObservableCollection<ACTIVIDAD_EJE> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private ACTIVIDAD_EJE _selectedItem;
        public ACTIVIDAD_EJE SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (_selectedItem == null)
                {
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                }
                else
                {
                    
                    if (editarEnabled)
                        EditarMenuEnabled = true;
                    
                }
                OnPropertyChanged("SelectedItem");
            }
        }
        #endregion


        #region [CONFIGURACION PERMISOS]

        private bool _guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return _guardarMenuEnabled; }
            set { _guardarMenuEnabled = value; RaisePropertyChanged("GuardarMenuEnabled"); }
        }

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

        private bool _eliminarMenuEnabled=false;
        public bool EliminarMenuEnabled
        {
            get { return _eliminarMenuEnabled; }
            set { _eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }

        private bool editarEnabled;
        public bool EditarEnabled
        {
            get { return editarEnabled; }
            set { editarEnabled = value; RaisePropertyChanged("EditarEnabled"); }
        }
        #endregion

        #region MANEJO DE CONTROLES
        private bool agregarVisible = false;
        public bool AgregarVisible
        {
            get { return agregarVisible; }
            set { agregarVisible = value; RaisePropertyChanged("AgregarVisible"); }
        }

        private bool bandera_agregar = true;
        public bool Bandera_Agregar
        {
            get { return bandera_agregar; }
            set { bandera_agregar = value; RaisePropertyChanged("Bandera_Agregar"); }
        }
        #endregion

    }
}

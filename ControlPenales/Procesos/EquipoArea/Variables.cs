using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

namespace ControlPenales
{
    partial class EquipoAreaViewModel
    {
       
        #region Areas
        private ObservableCollection<AREA> lstAreas;
        public ObservableCollection<AREA> LstAreas
        {
            get { return lstAreas; }
            set { lstAreas = value; OnPropertyChanged("LstAreas"); }
        }

        private ObservableCollection<EQUIPO_AREA> lstEquiposArea;
        public ObservableCollection<EQUIPO_AREA> LstEquiposArea
        {
            get { return lstEquiposArea; }
            set { lstEquiposArea = value; OnPropertyChanged("LstEquiposArea"); }
        }

        private EQUIPO_AREA selectedEquipoArea;
        public EQUIPO_AREA SelectedEquipoArea
        {
            get { return selectedEquipoArea; }
            set { selectedEquipoArea = value; OnPropertyChanged("SelectedEquipoArea"); }
        }

        private short? area = -1;//Convert.ToInt16(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings["EquipoArea"].Value);
        public short? Area
        {
            get { return area; }
            set { area = value; OnPropertyValidateChanged("Area"); }
        }

        private AREA selectedArea;
        public AREA SelectedArea
        {
            get { return selectedArea; }
            set { selectedArea = value; OnPropertyChanged("SelectedArea"); }
        }

        private DateTime Hoy = Fechas.GetFechaDateServer;
        #endregion

        #region Configuracion Permisos
        private bool guardarMenuEnabled = false;
        public bool GuardarMenuEnabled
        {
            get { return guardarMenuEnabled; }
            set { guardarMenuEnabled = value;
                OnPropertyChanged("GuardarMenuEnabled"); }
        }

        private bool agregarMenuEnabled = false;
        public bool AgregarMenuEnabled
        {
            get { return agregarMenuEnabled; }
            set { agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }

        private bool eliminarMenuEnabled = false;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }

        private bool editarMenuEnabled = false;
        public bool EditarMenuEnabled
        {
            get { return editarMenuEnabled; }
            set { editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }

        private bool cancelarMenuEnabled = false;
        public bool CancelarMenuEnabled
        {
            get { return cancelarMenuEnabled; }
            set { cancelarMenuEnabled = value; OnPropertyChanged("CancelarMenuEnabled"); }
        }
        #endregion
    }
}
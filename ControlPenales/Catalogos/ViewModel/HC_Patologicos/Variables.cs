using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class Catalogo_PatologicosViewModel
    {
        #region Buscar
        private string textBuscarPatologico = string.Empty;
        public string TextBuscarPatologico
        {
            get { return textBuscarPatologico; }
            set { textBuscarPatologico = value; RaisePropertyChanged("TextBuscarPatologico"); }
        }

        private ObservableCollection<PATOLOGICO_CAT> listPatologicos=null;
        public ObservableCollection<PATOLOGICO_CAT> ListPatologicos
        {
            get { return listPatologicos; }
            set { listPatologicos = value; RaisePropertyChanged("ListPatologicos"); }
        }

        private PATOLOGICO_CAT selectedPatologicos = null;
        public PATOLOGICO_CAT SelectedPatologicos
        {
            get { return selectedPatologicos; }
            set { selectedPatologicos = value; RaisePropertyChanged("SelectedPatologicos"); }
        }

        private bool emptyVisible = false;
        public bool EmptyVisible
        {
            get { return emptyVisible; }
            set { emptyVisible = value; RaisePropertyChanged("EmptyVisible"); }
        }
        #endregion

        #region Visibilidad de Control
        private bool agregarVisible = false;
        public bool AgregarVisible
        {
            get { return agregarVisible; }
            set { agregarVisible = value; RaisePropertyChanged("AgregarVisible"); }
        }
        #endregion

        #region Datos
        private EstatusControl _lista_Estatus = new EstatusControl();
        public EstatusControl Lista_Estatus
        {
            get { return _lista_Estatus; }
            set { _lista_Estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private string _selectedEstatusValue = "S";
        public string SelectedEstatusValue
        {
            get { return _selectedEstatusValue; }
            set { _selectedEstatusValue = value; OnPropertyValidateChanged("SelectedEstatusValue"); }
        }

        private List<EXT_SECTOR_CLASIFICACION> lstSectorClasificacion = new List<EXT_SECTOR_CLASIFICACION>();
        public List<EXT_SECTOR_CLASIFICACION> LstSectorClasificacion
        {
            get { return lstSectorClasificacion; }
            set { lstSectorClasificacion = value; RaisePropertyChanged("LstSectorClasificacion"); }
        }

        private string textPatologico = string.Empty;
        public string TextPatologico
        {
            get { return textPatologico; }
            set { textPatologico = value; OnPropertyValidateChanged("TextPatologico"); }
        }

        private bool isRecuperableChecked = false;
        public bool IsRecuperableChecked
        {
            get { return isRecuperableChecked; }
            set { isRecuperableChecked = value; OnPropertyValidateChanged("IsRecuperableChecked"); }
        }
        #endregion

        #region Habilitar/Deshabilitar controles
        private bool eliminarMenuEnabled = true;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; RaisePropertyChanged("EliminarMenuEnabled"); }
        }

        private bool guardarMenuEnabled = false;
        public bool GuardarMenuEnabled
        {
            get { return guardarMenuEnabled; }
            set { guardarMenuEnabled = value; RaisePropertyChanged("GuardarMenuEnabled"); }
        }

        private bool cancelarMenuEnabled = false;
        public bool CancelarMenuEnabled
        {
            get { return cancelarMenuEnabled; }
            set { cancelarMenuEnabled = value; RaisePropertyChanged("CancelarMenuEnabled"); }
        }

        private bool agregarMenuEnabled = true;
        public bool AgregarMenuEnabled
        {
            get { return agregarMenuEnabled; }
            set { agregarMenuEnabled = value; RaisePropertyChanged("AgregarMenuEnabled"); }
        }

        private bool editarMenuEnabled = true;
        public bool EditarMenuEnabled
        {
            get { return editarMenuEnabled; }
            set { editarMenuEnabled = value; RaisePropertyChanged("EditarMenuEnabled"); }
        }

        private bool isPatologicosEnabled = true;
        public bool IsPatologicosEnabled
        {
            get { return isPatologicosEnabled; }
            set { isPatologicosEnabled = value; RaisePropertyChanged("IsPatologicosEnabled"); }
        }
        #endregion

        #region Variables Privadas
        private MODO_OPERACION modo_seleccionado;
        #endregion
    }
}

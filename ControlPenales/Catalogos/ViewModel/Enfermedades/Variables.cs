using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoEnfermedadesViewModel
    {

        #region Variables Privadas
        private MODO_OPERACION modo_seleccionado = MODO_OPERACION.INSERTAR;
        #endregion

        #region Busqueda
        private string textBuscarEnfermedad = string.Empty;
        public string TextBuscarEnfermedad
        {
            get { return textBuscarEnfermedad; }
            set { textBuscarEnfermedad = value; RaisePropertyChanged("TextBuscarEnfermedad"); }
        }

        private RangeEnabledObservableCollection<ENFERMEDAD> listEnfermedades;
        public RangeEnabledObservableCollection<ENFERMEDAD> ListEnfermedades
        {
            get { return listEnfermedades; }
            set { listEnfermedades = value; RaisePropertyChanged("ListEnfermedades"); }
        }

        private ENFERMEDAD selectedEnfermedad;
        public ENFERMEDAD SelectedEnfermedad
        {
            get { return selectedEnfermedad; }
            set { selectedEnfermedad = value; RaisePropertyChanged("Enfermedad"); }
        }

        private bool emptyVisible = false ;
        public bool EmptyVisible
        {
            get { return emptyVisible; }
            set { emptyVisible = value; RaisePropertyChanged("EmptyVisible"); }
        }

        #region Variables privadas
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        #endregion
        #endregion

        #region Datos
        private List<Tipos_Enfermedades> lstTipoEnfermedad;
        public List<Tipos_Enfermedades> LstTipoEnfermedad
        {
            get { return lstTipoEnfermedad; }
            set { lstTipoEnfermedad = value; RaisePropertyChanged("LstTipoEnfermedad"); }
        }

        private string selectedTipoEnfermedadValue = "-1";
        public string SelectedTipoEnfermedadValue
        {
            get { return selectedTipoEnfermedadValue; }
            set { selectedTipoEnfermedadValue = value; OnPropertyValidateChanged("SelectedTipoEnfermedadValue"); }
        }


        private List<EXT_SECTOR_CLASIFICACION> lstSectorClasificacion = new List<EXT_SECTOR_CLASIFICACION>();
        public List<EXT_SECTOR_CLASIFICACION> LstSectorClasificacion
        {
            get { return lstSectorClasificacion; }
            set { lstSectorClasificacion = value; RaisePropertyChanged("LstSectorClasificacion"); }
        }

        private string textLetraEnfermedad = string.Empty;
        public string TextLetraEnfermedad
        {
            get { return textLetraEnfermedad; }
            set { textLetraEnfermedad = value; OnPropertyValidateChanged("TextLetraEnfermedad"); }
        }

        private string textClaveEnfermedad = string.Empty;
        public string TextClaveEnfermedad
        {
            get { return textClaveEnfermedad; }
            set { textClaveEnfermedad = value; OnPropertyValidateChanged("TextClaveEnfermedad"); }
        }
        private string textEnfermedad = string.Empty;
        public string TextEnfermedad
        {
            get { return textEnfermedad; }
            set { textEnfermedad = value; OnPropertyValidateChanged("TextEnfermedad"); }
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

        #region Habilitar/Deshabilitar controles
        private bool eliminarMenuEnabled = false;
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

        private bool isEnfermedadesEnabled = true;
        public bool IsEnfermedadesEnabled
        {
            get { return isEnfermedadesEnabled; }
            set { isEnfermedadesEnabled = value; RaisePropertyChanged("IsEnfermedadesEnabled"); }
        }
        #endregion

    }
}

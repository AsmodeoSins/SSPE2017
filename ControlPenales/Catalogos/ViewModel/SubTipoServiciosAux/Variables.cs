using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoSubTipoServiciosAuxViewModel
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

        private string textSubTipoServAux = string.Empty;
        public string TextSubTipoServAux
        {
            get { return textSubTipoServAux; }
            set { textSubTipoServAux = value; OnPropertyValidateChanged("TextSubTipoServAux"); }
        }

        private ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT> lstTipoServAux;
        public ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT> LstTipoServAux
        {
            get { return lstTipoServAux; }
            set { lstTipoServAux = value; RaisePropertyChanged("LstTipoServAux"); }
        }
         private short selectedTipoServAux = -1;
        public short SelectedTipoServAux
        {
            get { return selectedTipoServAux; }
            set { selectedTipoServAux = value; RaisePropertyChanged("SelectedTipoServAux"); }
        }

        #region Busqueda
        private ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT> lstTipoServAuxBusqueda;
        public ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT> LstTipoServAuxBusqueda
        {
            get { return lstTipoServAuxBusqueda; }
            set { lstTipoServAuxBusqueda = value; RaisePropertyChanged("LstTipoServAuxBusqueda"); }
        }
        private short selectedTipoServAuxBusqueda = -1;
        public short SelectedTipoServAuxBusqueda
        {
            get { return selectedTipoServAuxBusqueda; }
            set { selectedTipoServAuxBusqueda = value; RaisePropertyChanged("SelectedTipoServAuxBusqueda"); }
        }
        #endregion

        #region Grid
        private ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT> _listItems;
        public ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private SUBTIPO_SERVICIO_AUX_DIAG_TRAT _selectedItem;
        public SUBTIPO_SERVICIO_AUX_DIAG_TRAT SelectedItem
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

        private bool _eliminarMenuEnabled = false;
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

        private bool emptyVisible = false;
        public bool EmptyVisible
        {
            get { return emptyVisible; }
            set { emptyVisible = value; RaisePropertyChanged("EmptyVisible"); }
        }

        #endregion
    }
}

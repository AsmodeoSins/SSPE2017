using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoTipoServiciosAuxViewModel
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

        private string textServAux = string.Empty;
        public string TextServAux
        {
            get { return textServAux; }
            set { textServAux = value; OnPropertyValidateChanged("TextServAux"); }
        }

        #region Busqueda
        private string textServAuxBusqueda = string.Empty;
        public string TextServAuxBusqueda
        {
            get { return textServAuxBusqueda; }
            set { textServAuxBusqueda = value; RaisePropertyChanged("TextServAuxBusqueda"); }
        }
        #endregion

        #region Grid
        private ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT> _listItems;
        public ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private TIPO_SERVICIO_AUX_DIAG_TRAT _selectedItem;
        public TIPO_SERVICIO_AUX_DIAG_TRAT SelectedItem
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

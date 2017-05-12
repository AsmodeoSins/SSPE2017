using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoMedicamento_CategoriasViewModel
    {
        #region Variables Privadas
        private MODO_OPERACION modo_seleccionado;
        #endregion
        #region Busqueda
        private string textBuscarMedicamento_Categoria = string.Empty;
        public string TextBuscarMedicamento_Categoria
        {
            get { return textBuscarMedicamento_Categoria; }
            set { textBuscarMedicamento_Categoria = value; RaisePropertyChanged("TextBuscarMedicamento_Categoria"); }
        }

        private ObservableCollection<PRODUCTO_CATEGORIA> listProducto_Categorias = null;
        public ObservableCollection<PRODUCTO_CATEGORIA> ListProducto_Categorias
        {
            get { return listProducto_Categorias; }
            set { listProducto_Categorias = value; RaisePropertyChanged("ListProducto_Categorias"); }
        }

        private PRODUCTO_CATEGORIA selectedProducto_Categoria;
        public PRODUCTO_CATEGORIA SelectedProducto_Categoria
        {
            get { return selectedProducto_Categoria; }
            set { selectedProducto_Categoria=value; RaisePropertyChanged("SelectedProducto_Categoria"); }
        }
        #endregion

        #region Datos
        private string textMedicamento_Categoria = string.Empty;
        public string TextMedicamento_Categoria
        {
            get { return textMedicamento_Categoria; }
            set { textMedicamento_Categoria = value; OnPropertyValidateChanged("TextMedicamento_Categoria"); }
        }

        private string textDescripcion = string.Empty;
        public string TextDescripcion
        {
            get { return textDescripcion; }
            set { textDescripcion = value; OnPropertyValidateChanged("TextDescripcion"); }
        }

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

        private bool isMedicamento_CategoriasEnabled = true;
        public bool IsMedicamento_CategoriasEnabled
        {
            get { return isMedicamento_CategoriasEnabled; }
            set { isMedicamento_CategoriasEnabled = value; RaisePropertyChanged("IsMedicamento_CategoriasEnabled"); }
        }
        #endregion
    }
}

using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoMedicamento_SubcategoriasViewModel
    {
        #region Variables Privadas
        private MODO_OPERACION modo_seleccionado;
        #endregion
        #region Busqueda
        /*
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
        }*/
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

        #region SUB
        private ObservableCollection<PRODUCTO_CATEGORIA> _ListCategoriaBusqueda;
        public ObservableCollection<PRODUCTO_CATEGORIA> ListCategoriaBusqueda
        {
            get { return _ListCategoriaBusqueda; }
            set { _ListCategoriaBusqueda = value; OnPropertyChanged("ListCategoriaBusqueda"); }
        }
        private ObservableCollection<PRODUCTO_CATEGORIA> _ListCategoria;
        public ObservableCollection<PRODUCTO_CATEGORIA> ListCategoria
        {
            get { return _ListCategoria; }
            set { _ListCategoria = value; OnPropertyChanged("ListCategoria"); }
        }
        private int _SelectCategoriaBusqueda;
        public int SelectCategoriaBusqueda
        {
            get { return _SelectCategoriaBusqueda; }
            set { _SelectCategoriaBusqueda = value; OnPropertyChanged("SelectCategoriaBusqueda"); }
        }
        private int _SelectCategoria;
        public int SelectCategoria
        {
            get { return _SelectCategoria; }
            set { _SelectCategoria = value; OnPropertyChanged("SelectCategoria"); }
        }
        private string _TextSubcategoriaBuscar;
        public string TextSubcategoriaBuscar
        {
            get { return _TextSubcategoriaBuscar; }
            set { _TextSubcategoriaBuscar = value; OnPropertyChanged("TextSubcategoriaBuscar"); }
        }
        private bool _BuscarHabilitado = true;
        public bool BuscarHabilitado
        {
            get { return _BuscarHabilitado; }
            set { _BuscarHabilitado = value; OnPropertyChanged("BuscarHabilitado"); }
        }
        private ObservableCollection<PRODUCTO_SUBCATEGORIA> _ListProducto_Subcategorias;
        public ObservableCollection<PRODUCTO_SUBCATEGORIA> ListProducto_Subcategorias
        {
            get { return _ListProducto_Subcategorias; }
            set { _ListProducto_Subcategorias = value; OnPropertyChanged("ListProducto_Subcategorias"); }
        }
        private PRODUCTO_SUBCATEGORIA _SelectedProducto_Subcategoria;
        public PRODUCTO_SUBCATEGORIA SelectedProducto_Subcategoria
        {
            get { return _SelectedProducto_Subcategoria; }
            set { _SelectedProducto_Subcategoria = value; OnPropertyChanged("SelectedProducto_Subcategoria"); }
        }
        //private static bool _FocusBlock = false;
        //public static bool FocusBlock
        //{
        //    get { return _FocusBlock; }
        //    set
        //    {
        //        _FocusBlock = value;
        //        PopUpsViewModels.RaiseStaticPropertyChanged("FocusBlock");
        //    }
        //}
        private System.Windows.Controls.TextBox tbSubcategoriaBuscar;
        private bool _EmptyVisible;
        public bool EmptyVisible
        {
            get { return _EmptyVisible; }
            set { _EmptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }
        private string _HeaderAgregar = "Agregar Nueva Subcategoria";
        public string HeaderAgregar
        {
            get { return _HeaderAgregar; }
            set { _HeaderAgregar = value; OnPropertyChanged("HeaderAgregar"); }
        }
        #endregion

    }
}

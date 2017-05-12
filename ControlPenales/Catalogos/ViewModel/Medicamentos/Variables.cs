using System.Collections.ObjectModel;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlPenales.Clases.Estatus;

namespace ControlPenales
{
    partial class CatalogoMedicamentosViewModel
    {
        #region Variables Privadas
        private MODO_OPERACION modo_seleccionado = MODO_OPERACION.INSERTAR;
        #endregion

        #region Buscar medicamentos
        private string textBuscarMedicamento = string.Empty;
        public string TextBuscarMedicamento
        {
            get { return textBuscarMedicamento; }
            set { textBuscarMedicamento = value; RaisePropertyChanged("TextBuscarMedicamento"); }
        }

        private RangeEnabledObservableCollection<PRODUCTO> listProductos;
        public RangeEnabledObservableCollection<PRODUCTO> ListProductos
        {
            get { return listProductos; }
            set { listProductos = value; RaisePropertyChanged("ListProductos"); }
        }

        private PRODUCTO selectedProducto;
        public PRODUCTO SelectedProducto
        {
            get { return selectedProducto; }
            set { selectedProducto = value; RaisePropertyChanged("SelectedProducto"); }
        }

        private bool emptyVisible = false;
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

        #region Visibilidad de Control
        private bool agregarVisible = false;
        public bool AgregarVisible
        {
            get { return agregarVisible; }
            set { agregarVisible = value; RaisePropertyChanged("AgregarVisible"); }
        }

        private bool isSubcategoriaVisible= false;
        public bool IsSubcategoriaVisible
        {
            get { return isSubcategoriaVisible; }
            set { isSubcategoriaVisible = value; RaisePropertyChanged("IsSubcategoriaVisible"); }
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
            set { _selectedEstatusValue = value; OnPropertyChanged("SelectedEstatusValue"); }
        }

        private ObservableCollection<PRODUCTO_CATEGORIA> lstProductoCategoria = null;
        public ObservableCollection<PRODUCTO_CATEGORIA> LstProductoCategoria
        {
            get { return lstProductoCategoria; }
            set { lstProductoCategoria = value; RaisePropertyChanged("LstProductoCategoria"); }
        }

        private int selectedProductoCategoriaValue = -1;
        public int SelectedProductoCategoriaValue
        {
            get { return selectedProductoCategoriaValue; }
            set { selectedProductoCategoriaValue = value; OnPropertyValidateChanged("SelectedProductoCategoriaValue"); }
        }

        private ObservableCollection<PRODUCTO_SUBCATEGORIA> lstProductoSubcategoria = null;
        public ObservableCollection<PRODUCTO_SUBCATEGORIA> LstProductoSubcategoria
        {
            get { return lstProductoSubcategoria; }
            set { lstProductoSubcategoria = value; RaisePropertyChanged("LstProductoSubcategoria"); }
        }

        private int selectedProductoSubcategoriaValue = -1;
        public int SelectedProductoSubcategoriaValue
        {
            get { return selectedProductoSubcategoriaValue; }
            set { selectedProductoSubcategoriaValue = value; OnPropertyValidateChanged("SelectedProductoSubcategoriaValue"); }
        }

        private ObservableCollection<PRODUCTO_UNIDAD_MEDIDA> lstProductoUM = null;
        public ObservableCollection<PRODUCTO_UNIDAD_MEDIDA> LstProductoUM
        {
            get { return lstProductoUM; }
            set { lstProductoUM = value; RaisePropertyChanged("LstProductoUM"); }
        }

        private int selectedProductoUMValue = -1;
        public int SelectedProductoUMValue
        {
            get { return selectedProductoUMValue; }
            set { selectedProductoUMValue = value; OnPropertyValidateChanged("SelectedProductoUMValue"); }
        }

        private ObservableCollection<FORMA_FARMACEUTICA> lstFormaFarmaceutica;
        public ObservableCollection<FORMA_FARMACEUTICA> LstFormaFarmaceutica
        {
            get { return lstFormaFarmaceutica; }
            set { lstFormaFarmaceutica = value; RaisePropertyChanged("LstFormaFarmaceutica"); }
        }

        private short selectedFormaFarmaceuticaValue = -1;
        public short SelectedFormaFarmaceuticaValue
        {
            get { return selectedFormaFarmaceuticaValue; }
            set { selectedFormaFarmaceuticaValue = value; OnPropertyValidateChanged("SelectedFormaFarmaceuticaValue"); }
        }

        private string textMedicamento = string.Empty;
        public string TextMedicamento
        {
            get { return textMedicamento; }
            set { textMedicamento = value; OnPropertyValidateChanged("TextMedicamento"); }
        }

        private string textDescripcion = string.Empty;
        public string TextDescripcion
        {
            get { return textDescripcion; }
            set { textDescripcion = value; OnPropertyValidateChanged("TextDescripcion"); }
        }

        private ObservableCollection<PRESENTACION_MEDICAMENTO> lstPresentacion_Medicamento = null;
        public ObservableCollection<PRESENTACION_MEDICAMENTO> LstPresentacion_Medicamento
        {
            get { return lstPresentacion_Medicamento; }
            set { lstPresentacion_Medicamento = value; RaisePropertyChanged("LstPresentacion_Medicamento"); }
        }

        private short selectedPresentacion_MedicamentoValue = -1;
        public short SelectedPresentacion_MedicamentoValue
        {
            get { return selectedPresentacion_MedicamentoValue; }
            set { selectedPresentacion_MedicamentoValue = value; RaisePropertyChanged("SelectedPresentacion_MedicamentoValue"); }
        }

        private ObservableCollection<PRESENTACION_MEDICAMENTO> lstPresentacion_Medicamentos_Asignadas;
        public ObservableCollection<PRESENTACION_MEDICAMENTO> LstPresentacion_Medicamentos_Asignadas
        {
            get { return lstPresentacion_Medicamentos_Asignadas; }
            set { lstPresentacion_Medicamentos_Asignadas = value; RaisePropertyChanged("LstPresentacion_Medicamentos_Asignadas"); }
        }

        private PRESENTACION_MEDICAMENTO selectedPresentacion_Medicamento_Asignadas = null;
        public PRESENTACION_MEDICAMENTO SelectedPresentacion_Medicamento_Asignadas
        {
            get { return selectedPresentacion_Medicamento_Asignadas; }
            set { selectedPresentacion_Medicamento_Asignadas = value; RaisePropertyChanged("SelectedPresentacion_Medicamento_Asignadas"); }
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

        private bool isMedicamentosEnabled = true;
        public bool IsMedicamentosEnabled
        {
            get { return isMedicamentosEnabled; }
            set { isMedicamentosEnabled = value; RaisePropertyChanged("IsMedicamentosEnabled"); }
        }
        #endregion
    }
}

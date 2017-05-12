using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ControlPenales
{
    class ProductosAdministracionViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public ProductosAdministracionViewModel() 
        {
            InsertVisible = false;
            EditVisible = false;
            BorrarVisible = false;

        }
        #endregion

        #region variables
        private bool borrarVisible;
        public bool BorrarVisible
        {
            get { return borrarVisible; }
            set { borrarVisible = value; OnPropertyChanged("BorrarVisible"); }
        }
        private bool editVisible;
        public bool EditVisible
        {
            get { return editVisible; }
            set { editVisible = value; OnPropertyChanged("EditVisible"); }
        }
        private bool insertVisible;
        public bool InsertVisible
        {
            get { return insertVisible; }
            set { insertVisible = value; OnPropertyChanged("InsertVisible"); }
        }
        public string Name
        {
            get
            {
                return "catalogo_productos_administracion";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "catalogo_nuevo":
                    //show usercontrol insert
                    InsertVisible = true;
                    EditVisible = false;
                    BorrarVisible = false;
                    break;
                case "catalogo_borrar":
                    //show are you sure
                    InsertVisible = false;
                    EditVisible = false;
                    BorrarVisible = true;
                    break;
                case "boton_guardar_insert":
                    InsertVisible = false;
                    EditVisible = false;
                    break;
                case "boton_editar_insert":
                    InsertVisible = false;
                    EditVisible = false;

                    break;
                case "boton_cancelar_insert":
                    InsertVisible = false;
                    EditVisible = false;
                    break;
                case "boton_imprimir":
                    //System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
                    //if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
                    //{
                    //    Size pageSize = new Size(Printdlg.PrintableAreaWidth, Printdlg.PrintableAreaHeight);
                    //    // sizing of the element.
                    //    dataGrid1.Measure(pageSize);
                    //    dataGrid1.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
                    //    Printdlg.PrintVisual(dataGrid1, "CERESOS");
                    //}
                    break;
            }
        }
        #endregion

        #region command
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        Usuario _selectedItem = null;
        public Usuario SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
            }
        }
        #endregion

    }
}

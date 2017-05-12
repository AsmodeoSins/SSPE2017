using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    class ActualizarCatalogosViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public ActualizarCatalogosViewModel() { }
        #endregion

        #region variables
        public string Name
        {
            get
            {
                return "catalogo_actualizar_catalogos";
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

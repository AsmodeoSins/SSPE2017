using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    class FechaEntregaAPViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public FechaEntregaAPViewModel()
        {
        }
        #endregion

        #region variables
        public string Name
        {
            get
            {
                return "fecha_entrega_alimentos_preparados";
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
                case "boton_generar_requesicion":
                    //MENSAJE DE SEGURIDAD
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
        #endregion

    }
}

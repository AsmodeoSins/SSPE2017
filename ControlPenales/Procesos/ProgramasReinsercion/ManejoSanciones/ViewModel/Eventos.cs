using System.Windows.Input;

namespace ControlPenales
{
    partial class ManejoSancionesViewModel : ViewModelBase
    {
        /* [descripcion de clase]
         * clase donde se definen todos los eventos en el modulo manejo sanciones
         * 
         * comando para carga de modulo: ControlCalificacionesLoading
         * comando para accion click: OnClick
         * 
         */

        #region [eventos]
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }
        public ICommand ControlCalificacionesLoading
        {
            get { return new DelegateCommand<ManejoSancionesView>(ManejoSancionesLoad); }
        }
        private ICommand _TabChange;
        public ICommand TabChange
        {
            get
            {
                return _TabChange ?? (_TabChange = new RelayCommand(OnTabChanged));
            }
        }
        #endregion
    }
}

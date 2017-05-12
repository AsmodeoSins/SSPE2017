using System.Windows.Input;

namespace ControlPenales
{
    public partial class ManejoEmpalmesViewModel
    {
        /* [descripcion de clase]
         * metodo que describe todos los comandos utilizados en el modulo manejo de empalmes
         * 
         * comando de carga de modulo: ManejoEmpalmeLoading
         * comando de evento click: OnClick
         * 
         */

        #region [Eventos]
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }
        public ICommand ManejoEmpalmeLoading
        {
            get { return new DelegateCommand<ManejoEmpalmesView>(ManejoEmpalmeLoad); }
        }
        #endregion
    }
}

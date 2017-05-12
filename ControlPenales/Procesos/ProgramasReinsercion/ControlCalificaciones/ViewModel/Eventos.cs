using System.Windows.Input;

namespace ControlPenales
{
    partial class ControlCalificacionesViewModel : ViewModelBase
    {
        /* [descripcion de clase]
         * clase donde se definen los eventos utilizados en el modulo control calificaciones
         * 
         * comando de carga de modulo: ControlCalificacionesLoading
         * comando de acciones click: OnClick
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
            get { return new DelegateCommand<ControlCalificacionesView>(ControlCalificacionesLoad); }
        }
        #endregion
    }
}

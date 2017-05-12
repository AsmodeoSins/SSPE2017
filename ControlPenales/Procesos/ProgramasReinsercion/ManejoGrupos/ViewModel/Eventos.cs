using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ManejoGruposViewModel
    {
        /* [descripcion de clase]
         * eventos que se usan en el modulo manejo de grupos
         * 
         * comando de carga de ventana: ManejoGruposLoading
         * comando de accion click: OnClick
         * 
         */

        #region [Commands]
        //LOADS
        public ICommand ManejoGruposLoading
        {
            get { return new DelegateCommand<ManejoGruposView>(ManejoGruposLoad); }
        }

        public ICommand CmdAbrirAgenda
        {
            get { return new DelegateCommand<object>(AbrirAgenda); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        public ICommand ValueChanged
        {
            get { return new DelegateCommand<DataGrid>(SetIsSelectedProperty); }
        }
        #endregion
    }
}

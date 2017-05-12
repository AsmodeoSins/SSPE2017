using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoTipoRelacionViewModel
    {
        //private ICommand buscarClick;
        //public ICommand BuscarClick
        //{
        //    get
        //    {
        //        return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
        //    }
        //}
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch)); }
        }

        public ICommand CatalogoSimpleLoading
        {
            get { return new DelegateCommand<CatalogoSimpleView>(TipoRelacionLoad); }
        }
    }
}
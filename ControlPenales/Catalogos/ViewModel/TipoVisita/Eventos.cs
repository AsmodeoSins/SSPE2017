using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoTipoVisitaViewModel
    {
        private ICommand _buscarClick;
        public ICommand BuscarClick
        {
            get { return _buscarClick ?? (_buscarClick = new RelayCommand(ClickEnter)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch)); }
        }

        //private ICommand _CatalogoSimpleLoading;
        public ICommand CatalogoSimpleLoading
        {
            //get {
            //    return _TipoVisitaLoading ?? (_TipoVisitaLoading = new RelayCommand(TipoVisitaLoad));
            get { return new DelegateCommand<CatalogoSimpleView>(TipoVisitaLoad); }
        }
    }
}
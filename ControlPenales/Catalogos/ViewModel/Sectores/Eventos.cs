using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoSectoresViewModel
    {
        private ICommand _buscarClick;
        public ICommand BuscarClick
        {
            get { return _buscarClick ?? (_buscarClick = new RelayCommand(ClickEnter)); }
        }

        private ICommand _cargarImagenClick;
        public ICommand CargarImagenClick
        {
            get { return _cargarImagenClick ?? (_cargarImagenClick = new RelayCommand(ElegirImagenGuardar)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch)); }
        }

        public ICommand SectoresLoading
        {
            get { return new DelegateCommand<CatalogoSectoresView>(SectoresLoad); }
        }
    }
}

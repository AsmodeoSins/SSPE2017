using System.Windows.Input;


namespace ControlPenales
{
    partial class CatalogoAreasViewModel
    {
        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }
      
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

    }
}
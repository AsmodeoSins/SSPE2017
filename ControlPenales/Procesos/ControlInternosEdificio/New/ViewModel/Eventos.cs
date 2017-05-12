using System.Windows.Input;

namespace ControlPenales
{
    partial class NewControlInternosViewModel
    {
        #region Load / Unload
        private ICommand _cmdLoad;
        public ICommand CmdLoad
        {
            get { return (_cmdLoad = _cmdLoad ?? new DelegateCommand<NewControlInternosEdificioView>(OnLoad)); }
        }

        private ICommand _cmdUnLoaded;
        public ICommand CmdUnLoaded
        {
            get { return (_cmdUnLoaded = _cmdUnLoaded ?? new DelegateCommand<NewControlInternosEdificioView>(UnLoad)); }
        }
        #endregion

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch)); }
        }

        ////EVENTO PARA CHECKBOX DE INTERNOS REQUERIDOS
        //private ICommand _checked;
        //public ICommand Checked
        //{
        //    get { return _checked ?? (_checked = new RelayCommand(InternoSeleccionado)); }
        //}

        ////EVENTO CHECKBOX PARA INTERNOS AUSENTES
        //private ICommand _checkedDelete;
        //public ICommand CheckedDelete
        //{
        //    get { return _checkedDelete ?? (_checkedDelete = new RelayCommand(EliminarInternoAusente)); }
        //}

        ////EVENTO PARA DOBLE CLIC PARA INTERNOS AUSENTES
        //private ICommand _doubleClickCommand;
        //public ICommand DoubleClickCommand
        //{
        //    get { return _doubleClickCommand ?? (_doubleClickCommand = new RelayCommand(ClickSwitch)); }
        //}

        //private ICommand _buscarInternoClick;
        //public ICommand BuscarInternoClick
        //{
        //    get { return _buscarInternoClick ?? (_buscarInternoClick = new RelayCommand(ClickEnter)); }
        //}
    }
}

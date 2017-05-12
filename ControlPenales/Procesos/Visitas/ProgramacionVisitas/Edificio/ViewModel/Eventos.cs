using System.Windows.Forms;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ProgramacionVisitaEdificioViewModel
    {
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        private ICommand _EnterClick;
        public ICommand EnterClick
        {
            get
            {
                return _EnterClick ?? (_EnterClick = new RelayCommand(FiltrarClick));
            }
        }

        public ICommand OnLoaded
        {
            get { return new DelegateCommand<ProgramacionVisitaEdificioView>(Load_Window); }
        }
        //private ICommand onLoadDataGrid;
        //public ICommand OnLoadDataGrid
        //{
        //    get
        //    {
        //        return onLoadDataGrid ?? (onLoadDataGrid = new RelayCommand(Load_Grid));
        //    }
        //}
    }
}

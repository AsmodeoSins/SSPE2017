using System.Windows.Forms;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ProgramacionVisitaApellidoViewModel
    {
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<ProgramacionVisitaApellidoView>(Load_Window); }
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

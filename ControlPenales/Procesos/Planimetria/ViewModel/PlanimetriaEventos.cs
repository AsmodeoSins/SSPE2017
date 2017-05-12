using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
namespace ControlPenales
{
    partial class PlanimetriaViewModel
    {
        #region Commands
        private ICommand mouseDoubleClickArbolCommand;
        public ICommand MouseDoubleClickArbolCommand
        {
            get
            {
                return mouseDoubleClickArbolCommand ?? (mouseDoubleClickArbolCommand = new RelayCommand(SeleccionaSector));
            }
        }
       
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(btnClick));
            }
        }

        //LOAD
        public ICommand PlanimetriaLoading
        {
            get { return new DelegateCommand<PlanimetriaView>(PlanimetriaLoad); }
        }
        #endregion
    }
}

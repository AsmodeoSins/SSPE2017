using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class BusquedaAsistenciaViewModel
    {
        #region Commands
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<LeerInternos>(OnLoad); }
        }

        private ICommand buttonMouseEnter;
        public ICommand ButtonMouseEnter
        {
            get { return buttonMouseEnter ?? (buttonMouseEnter = new RelayCommand(MouseEnterSwitch)); }
        }


        private ICommand buttonMouseLeave;
        public ICommand ButtonMouseLeave
        {
            get { return buttonMouseLeave ?? (buttonMouseLeave = new RelayCommand(MouseLeaveSwitch)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(ClickSwitch));
            }
        }

        private ICommand busquedaHuellas;
        public ICommand BusquedaHuellas
        {
            get { return busquedaHuellas ?? (busquedaHuellas = new RelayCommand(SeleccionHuella)); }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class BusquedaHuellaExcarcelaciones
    {
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(ClickSwitch));
            }
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

        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BusquedaHuellaExcarcelacionView>(OnLoad); }
        }

        private ICommand buscarNIPEnter;
        public ICommand BuscarNIPEnter
        {
            get
            {
                return buscarNIPEnter ?? (buscarNIPEnter = new RelayCommand(EnterKeyPressedID));
            }
        }
    }
}

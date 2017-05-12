using ControlPenales;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class BandejaEntradaViewModel
    {
        #region Click
        private ICommand _onClickSelect;
        public ICommand OnClickSelect
        {
            get
            {
                return _onClickSelect ?? (_onClickSelect = new RelayCommand(SelectNotificacion));
            }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(SwitchClick));
            }
        }
        #endregion

        #region Load
        public ICommand BandejaEntradaLoading
        {
            get { return new DelegateCommand<BandejaEntradaView>(BandejaEntradaLoad); }
        }
        #endregion

        #region Change Combobox
        private System.Windows.Controls.ComboBoxItem selectedItem;
        public System.Windows.Controls.ComboBoxItem SelectedItem
        {
            get { return selectedItem; }
            set { if (selectedItem != value) { selectedItem = value; setNotificaciones(); OnPropertyChanged("SelectedItem"); } }
        }

        private System.Windows.Controls.ComboBoxItem selectedItem2;
        public System.Windows.Controls.ComboBoxItem SelectedItem2
        {
            get { return selectedItem2; }
            set { if (selectedItem2 != value) { selectedItem2 = value; orderNotificaciones(); OnPropertyChanged("SelectedItem2"); } }
        }
        #endregion
    }
}

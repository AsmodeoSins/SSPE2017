﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteSituacionJuridicaViewModel
    {
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<ReporteSituacionJuridicaView>(OnLoad); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(ClickSwitch));
            }
        }
    }
}
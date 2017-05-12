﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ControlPenales
{
    class EstatusAdeudosViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public EstatusAdeudosViewModel() 
        {
        }
        #endregion

        #region variables
        public string Name
        {
            get
            {
                return "almacen_estatus_adeudo";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        #endregion
    }
}
using ControlPenales;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;


namespace ControlPenales
{
    partial class RegistroLiberadoViewModel
    {
        #region commands
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
            get { return new DelegateCommand<RegistroLiberadoView>(OnLoad); }
        }
        private ICommand buscarEmpleadoClick;
        public ICommand BuscarEmpleadoClick
        {
            get
            {
                return buscarEmpleadoClick ?? (buscarEmpleadoClick = new RelayCommand(ClickEnter));
            }
        }
        //Fotos
        public ICommand CamSettings
        {
            get { return new DelegateCommand<string>(OpenSetting); }
        }        
        public ICommand CaptureImage
        {
            get { return new DelegateCommand<System.Windows.Controls.Image>(OnTakePicture); }
        }        
        //Huellas
        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }
        #endregion
    }
}

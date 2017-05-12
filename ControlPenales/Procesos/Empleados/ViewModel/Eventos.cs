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
using System.Windows.Controls;


namespace ControlPenales
{
    partial class RegistroPersonalViewModel
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
            get { return new DelegateCommand<RegistroPersonalView>(OnLoad); }
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
        private ICommand _CargarMasPersonas;
        public ICommand CargarMasPersonas
        {
            get
            {
                return _CargarMasPersonas ?? (_CargarMasPersonas = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            if (SeguirCargandoPersonas)
                            {
                                //LstEmpleadoPop = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                                LstEmpleadoPop.InsertRange(await SegmentarPersonasBusqueda(Pagina));
                            }
                        }
                }));
            }
        }
        private ICommand _HeaderClick;
        public ICommand HeaderClick
        {
            get
            {
                return _HeaderClick ?? (_HeaderClick = new RelayCommand(HeaderSort));
            }
        }
        #endregion
    }
}

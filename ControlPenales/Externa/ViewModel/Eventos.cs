using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class PrincipalVisitaExternaViewModel
    {
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand enterClick;
        public ICommand EnterClick
        {
            get { return enterClick ?? (enterClick = new RelayCommand(ClickEnter)); }
        }
        private ICommand enterClickNuevo;
        public ICommand EnterClickNuevo
        {
            get { return enterClickNuevo ?? (enterClickNuevo = new RelayCommand(ClickEnterNuevo)); }
        }
        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }
        //public ICommand CaptureImage
        //{
        //    get { return new DelegateCommand<Image>(CapturarFoto); }
        //}
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<PrincipalVisitaExternaView>(Load_Window); }
        }
        //public ICommand FrenteDetrasCommand
        //{
        //    get { return new DelegateCommand<TomarFotoSenaParticularView>(FrenteDetrasImages); }
        //}
        /*
        public ICommand TomarFoto
        {
            get { return new DelegateCommand<object>(AbrirCamara); }
        }

        public ICommand DiaVisita
        {
            get { return new DelegateCommand<string>(SetDiaVisita); }
        }

        public ICommand CamSettings
        {
            get { return new DelegateCommand<string>(OpenSetting); }
        }

        public ICommand CaptureImage
        {
            get { return new DelegateCommand<Image>(CapturarFoto); }
        }

        public ICommand OnLoadedCapturaVisita
        {
            get { return new DelegateCommand<CapturaVisitaView>(LoadCapturaVisita); }
        }
        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }  
        */      
    }
}

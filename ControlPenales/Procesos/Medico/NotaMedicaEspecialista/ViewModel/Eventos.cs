using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class NotaMedicaEspecialistaViewModel
    {

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        public ICommand NotaMedicaLoaded
        {
            get { return new DelegateCommand<NotaMedicaView>(NotaMedicaLoad); }
        }
        public ICommand NotaMedicaUnLoaded
        {
            get { return new DelegateCommand<NotaMedicaView>(NotaMedicaUnLoad); }
        }

        #region Datos Expediente
        private System.Windows.Input.ICommand onClick;
        public System.Windows.Input.ICommand OnClick
        {
            get { return onClick ?? (onClick = new RelayCommand(clickSwitch)); }
        }        
        private System.Windows.Input.ICommand modelClick;
        public System.Windows.Input.ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(ModelEnter));
            }
        }
        private System.Windows.Input.ICommand buscarClick;
        public System.Windows.Input.ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }
        private System.Windows.Input.ICommand buscarInternoClick;
        public System.Windows.Input.ICommand BuscarInternoClick
        {
            get
            {
                return buscarInternoClick ?? (buscarInternoClick = new RelayCommand(ClickEnter));
            }
        }
        #endregion

        private System.Windows.Input.ICommand _CargarMasResultados;
        public System.Windows.Input.ICommand CargarMasResultados
        {
            get
            {
                return _CargarMasResultados ?? (_CargarMasResultados = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            if (SeguirCargando)
                                ListExpediente.InsertRange(await SegmentarResultadoBusqueda(Pagina));
                        }
                }));
            }
        }/*
        private ICommand onAppointmentClick;
        public ICommand OnAppointmentClick
        {
            get { return onAppointmentClick ?? (onAppointmentClick = new RelayCommand(AppointmentClick)); }
        }*/
        private ICommand checkClick;
        public ICommand CheckClick
        {
            get
            {
                return checkClick ?? (checkClick = new RelayCommand(OdontogramaClick));
            }
        }
        private ICommand regionClick;
        public ICommand RegionClick
        {
            get
            {
                return regionClick ?? (regionClick = new RelayCommand(RegionSwitch));
            }
        }
        private ICommand _LesionClick;
        public ICommand LesionClick
        {
            get
            {
                return _LesionClick ?? (_LesionClick = new RelayCommand(LesionSelected));
            }
        }

        #region HUELLA
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BuscarPorHuellaYNipView>(OnLoad); }
        }
        public ICommand BuscarHuellaNip
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }

        public ICommand CommandAceptar
        {
            get { return new DelegateCommand<Window>(Aceptar); }
        }

        public ICommand CommandOpem442
        {
            get { return new DelegateCommand<string>(Capture); }
        }

        public ICommand CommandContinue
        {
            get { return new DelegateCommand<string>((s) => { isKeepSearching = s == "True" ? true : false; }); }
        }
        private ICommand _Checked;
        public ICommand Checked
        {
            get
            {
                return _Checked ?? (_Checked = new DelegateCommand<object>((SelectedItem) => { DietaSelecccionada(SelectedItem); }));
            }
        }
        #endregion



        #region Agenda
        private ICommand _ClickAgenda;
        public ICommand ClickAgenda
        {
            get
            {
                return _ClickAgenda ?? (_ClickAgenda = new RelayCommand(AgendaSwitch));
            }
        }
        #endregion

    }
}

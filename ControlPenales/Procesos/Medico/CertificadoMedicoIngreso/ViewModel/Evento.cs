using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CertificadoMedicoIngresoViewModel
    {
        public ICommand CertificadoLoading
        {
            get { return new DelegateCommand<CertificadoMedicoIngresoView>(WindowLoad); }
        }
        public ICommand CertificadoUnLoading
        {
            get { return new DelegateCommand<CertificadoMedicoIngresoView>(WindowUnLoad); }
        }
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        private ICommand _EnfermedadClick;
        public ICommand EnfermedadClick
        {
            get
            {
                return _EnfermedadClick ?? (_EnfermedadClick = new RelayCommand(EnfermedadSeleccionada));
            }
        }
        private ICommand modelClick;
        public ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(ModelEnter));
            }
        }
        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }
        public ICommand SeniasFrenteLoading
        {
            get { return new DelegateCommand<SeniasFrenteView>(SeniasFrenteLoad); }
        }
        public ICommand SeniasDorsoLoading
        {
            get { return new DelegateCommand<SeniasDorsoView>(SeniasDorsoLoad); }
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
    }
}
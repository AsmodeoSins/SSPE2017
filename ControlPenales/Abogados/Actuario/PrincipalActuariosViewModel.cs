using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class PrincipalActuariosViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region variables
        public string Name
        {
            get
            {
                return "principal_actuarios";
            }
        }
        private bool padronVisible;
        public bool PadronVisible
        {
            get { return padronVisible; }
            set { padronVisible = value; OnPropertyChanged("PadronVisible"); }
        }
        private bool consultaVisible;
        public bool ConsultaVisible
        {
            get { return consultaVisible; }
            set { consultaVisible = value; OnPropertyChanged("ConsultaVisible"); }
        }
        private bool digitalizacionVisible;
        public bool DigitalizacionVisible
        {
            get { return digitalizacionVisible; }
            set { digitalizacionVisible = value; OnPropertyChanged("DigitalizacionVisible"); }
        }

        private bool huellasVisible;

        public bool HuellasVisible
        {
            get { return huellasVisible; }
            set { huellasVisible = value; OnPropertyChanged("HuellasVisible"); }
        }
        #endregion

        #region constructor
        public PrincipalActuariosViewModel() {
           
        }

        #endregion

        #region metodos
        
        void IPageViewModel.inicializa()
        {
            PadronVisible = true;
            ConsultaVisible = !PadronVisible;
            DigitalizacionVisible = !PadronVisible;
            HuellasVisible = !PadronVisible;
        }
        
        private void clickSwitch(Object obj)
        {
            switch(obj.ToString())
            {
                case "insertar_menu":
                    ConsultaVisible = false;
                    DigitalizacionVisible = false;
                    PadronVisible = true;
                    HuellasVisible = true;
                    break;
                case "buscar_menu":
                    PadronVisible = false;
                    DigitalizacionVisible = false;
                    ConsultaVisible = true;
                    HuellasVisible = true;
                    break;
                case "digitalizar":
                    PadronVisible = false;
                    ConsultaVisible = false;
                    DigitalizacionVisible = true;
                    HuellasVisible = false;
                    break;
                case "buscar_interno":
                    HuellasVisible = true;
                    break;
            }
            
        }
        #endregion

        #region command
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        #endregion

    }
}

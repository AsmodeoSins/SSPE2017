using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class PrincipalColaboradorViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region variables
        public string Name
        {
            get
            {
                return "principal_colaboradores";
            }
        }
        private bool padronVisible;
        public bool PadronVisible
        {
            get { return padronVisible; }
            set { padronVisible = value; OnPropertyChanged("PadronVisible"); }
        }
        private bool consultaAbogadoVisible;
        public bool ConsultaAbogadoVisible
        {
            get { return consultaAbogadoVisible; }
            set { consultaAbogadoVisible = value; OnPropertyChanged("ConsultaAbogadoVisible"); }
        }
        private bool consultaInternoVisible;
        public bool ConsultaInternoVisible
        {
            get { return consultaInternoVisible; }
            set { consultaInternoVisible = value; OnPropertyChanged("ConsultaInternoVisible"); }
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
        public PrincipalColaboradorViewModel()
        {
           
        }
        #endregion

        #region metodos
        
        void IPageViewModel.inicializa()
        {
            PadronVisible = true;
            ConsultaAbogadoVisible = !PadronVisible;
            ConsultaInternoVisible = !PadronVisible;
            DigitalizacionVisible = !PadronVisible;
            HuellasVisible = !PadronVisible;
        }
        
        private void clickSwitch(Object obj)
        {
            switch(obj.ToString())
            {
                case "insertar_menu":
                    ConsultaAbogadoVisible = false;
                    ConsultaInternoVisible = false;
                    DigitalizacionVisible = false;
                    PadronVisible = true;
                    HuellasVisible = true;
                    break;
                case "buscar_menu":
                    PadronVisible = false;
                    DigitalizacionVisible = false;
                    ConsultaInternoVisible = false;
                    ConsultaAbogadoVisible = true;
                    HuellasVisible = true;
                    break;
                case "buscar_abogado":
                    PadronVisible = false;
                    DigitalizacionVisible = false;
                    ConsultaInternoVisible = false;
                    ConsultaAbogadoVisible = true;
                    HuellasVisible = true;
                    break;
                case "digitalizar":
                    PadronVisible = false;
                    ConsultaInternoVisible = false;
                    ConsultaAbogadoVisible = false;
                    DigitalizacionVisible = true;
                    HuellasVisible = false;
                    break;
                case "buscar_interno":
                    PadronVisible = false;
                    DigitalizacionVisible = false;
                    ConsultaAbogadoVisible = false;
                    ConsultaInternoVisible = true;
                    HuellasVisible = true;
                    break;
                case "selecciona_buscar_abogado":
                    ConsultaAbogadoVisible = false;
                    ConsultaInternoVisible = false;
                    DigitalizacionVisible = false;
                    PadronVisible = true;
                    HuellasVisible = true;
                    break;
                case "salir_buscar_abogado":
                    ConsultaAbogadoVisible = false;
                    ConsultaInternoVisible = false;
                    DigitalizacionVisible = false;
                    PadronVisible = true;
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

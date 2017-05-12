using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class AgendaViewModel : ValidationViewModelBase, IPageViewModel
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        #region variables
        public string Name
        {
            get
            {
                return "agenda";
            }
        }

        private bool agendaVisible;
        public bool AgendaVisible
        {
            get { return agendaVisible; }
            set { agendaVisible = value; OnPropertyChanged("AgendaVisible"); }
        }

        private bool expedienteDepartamentoVisible;

        public bool ExpedienteDepartamentoVisible
        {
            get { return expedienteDepartamentoVisible; }
            set { expedienteDepartamentoVisible = value; OnPropertyChanged("ExpedienteDepartamentoVisible"); }
        }

        private bool formatoDepartamentoVisible;

        public bool FormatoDepartamentoVisible
        {
            get { return formatoDepartamentoVisible; }
            set { formatoDepartamentoVisible = value; OnPropertyChanged("FormatoDepartamentoVisible"); }
        }
        #endregion

        #region constructor
        public AgendaViewModel()
        {
            AgendaVisible = true;
            ExpedienteDepartamentoVisible = false;
            FormatoDepartamentoVisible = false;
        }

        #endregion

        #region metodos

        
        void IPageViewModel.inicializa()
        { }
        
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "expediente_menu":
                    AgendaVisible = false;
                    ExpedienteDepartamentoVisible = true;
                break;
                case "documento_menu":
                    FormatoDepartamentoVisible = true;
                break;
                case "aceptar_formato_departamento":
                    FormatoDepartamentoVisible = false;
                break;
                case "cancelar_formato_departamento":
                    FormatoDepartamentoVisible = false;
                break;
                case "menu_salir":
                    OnParameterChange("salir");
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

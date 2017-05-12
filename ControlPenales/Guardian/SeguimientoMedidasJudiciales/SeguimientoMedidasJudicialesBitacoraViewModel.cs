using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class SeguimientoMedidasJudicialesBitacoraViewModel : ValidationViewModelBase, IPageViewModel
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        #region variables
        public string Name
        {
            get
            {
                return "seguimiento_medidas_judiciales_bitacora";
            }
        }

        private bool consultaVisible;

        public bool ConsultaVisible
        {
            get { return consultaVisible; }
            set { consultaVisible = value; OnPropertyChanged("ConsultaVisible"); }
        }
        private bool expedienteVisible;

        public bool ExpedienteVisible
        {
            get { return expedienteVisible; }
            set { expedienteVisible = value; OnPropertyChanged("ExpedienteVisible"); }
        }
        #endregion

        #region constructor
        public SeguimientoMedidasJudicialesBitacoraViewModel()
        {
          
        }

        #endregion

        #region metodos

        
        void IPageViewModel.inicializa()
        {
            ConsultaVisible = true;
            ExpedienteVisible = !ConsultaVisible;
        }
        
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            { 
                case "consulta":
                    ExpedienteVisible = true;
                    ConsultaVisible = !ExpedienteVisible;
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

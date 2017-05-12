using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class InternoAgendaViewModel : ValidationViewModelBase, IPageViewModel
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        #region variables
        public string Name
        {
            get
            {
                return "interno_agenda";
            }
        }

        private DateTime fecha;

        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; FechaLetra = Fechas.fechaLetra(fecha); OnPropertyChanged("Fecha"); }
        }

        private string fechaLetra;

        public string FechaLetra
        {
            get { return fechaLetra; }
            set { fechaLetra = value; OnPropertyChanged("FechaLetra"); }
        }
        #endregion

        #region constructor
        public InternoAgendaViewModel()
        {
          
        }

        #endregion

        #region metodos

        
        void IPageViewModel.inicializa()
        {
            Fecha = Fechas.GetFechaDateServer;
        }
        
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
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

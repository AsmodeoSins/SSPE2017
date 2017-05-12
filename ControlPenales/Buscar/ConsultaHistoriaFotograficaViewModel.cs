using ControlPenales;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class ConsultaHistoriaFotograficaViewModel : ValidationViewModelBase,IPageViewModel
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        public string Name
        {
            get
            {
                return "consulta_historia_fotografica";
            }
        }

        public ConsultaHistoriaFotograficaViewModel()
        { 
        }

        void IPageViewModel.inicializa()
        { }

        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "menu_imputado_salir":
                    OnParameterChange("salir");
                    break;
                case "salir":
                    OnParameterChange("salir");
                    break;
            }
        }
        //Eventos
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
    }
}

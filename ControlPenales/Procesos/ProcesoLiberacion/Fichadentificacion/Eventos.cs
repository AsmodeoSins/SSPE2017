using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{


      partial  class ProcesoLiberacionViewModel
    {

        //private ICommand buscarClick;
        //public ICommand BuscarClick
        //{
        //    get
        //    {
        //        return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
        //    }
        //}

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }


        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

        public ICommand FichaIdentificacionLoading
        {

            get { return new DelegateCommand<FichaIdentificacionView>(FichaIdentificacionLoad); }
        }

          
    }
}

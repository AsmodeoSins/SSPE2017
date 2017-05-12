using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ControlPenales
{
    class RelacionesPersonalesPandillasViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public RelacionesPersonalesPandillasViewModel()
        {
            DatosVisible = true;
            BuscarVisible = false;
        }
        #endregion

        #region variables
        private bool datosVisible;
        public bool DatosVisible
        {
            get { return datosVisible; }
            set { datosVisible = value; OnPropertyChanged("DatosVisible"); }
        }
        private bool buscarVisible;
        public bool BuscarVisible
        {
            get { return buscarVisible; }
            set { buscarVisible = value; OnPropertyChanged("BuscarVisible"); }
        }
        public string Name
        {
            get
            {
                return "relaciones_personales_pandillas";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }

        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    DatosVisible = false;
                    BuscarVisible = true;
                    break;
                case "buscar_seleccionar":
                    DatosVisible = true;
                    BuscarVisible = false;
                    break;
                case "buscar_salir":
                    DatosVisible = true;
                    BuscarVisible = false;
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

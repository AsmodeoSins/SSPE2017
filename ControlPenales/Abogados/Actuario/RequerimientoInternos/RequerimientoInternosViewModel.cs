using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    class RequerimientoInternosViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region variables
        public string Name
        {
            get
            {
                return "requerimiento_internos";
            }
        }
        private bool requerimientoView;

        public bool RequerimientoView
        {
            get { return requerimientoView; }
            set { requerimientoView = value; OnPropertyChanged("RequerimientoView"); }
        }
        private bool busquedaView;

        public bool BusquedaView
        {
            get { return busquedaView; }
            set { busquedaView = value; OnPropertyChanged("BusquedaView"); }
        }

        #endregion

        #region constructor
        public RequerimientoInternosViewModel() {
            RequerimientoView = true;
            BusquedaView = false;
        }
        #endregion

        #region metodos
        
        void IPageViewModel.inicializa()
        { }
        
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "requerimiento":
                     BusquedaView = false;
                     RequerimientoView = true;
                     break;
                case "buscar":
                     RequerimientoView = false;
                     BusquedaView = true;
                    break;
            }
        }
        #endregion

        #region command
        private ICommand _clickPageCommand;
        public ICommand ClickPageCommand
        {
            get
            {
                return _clickPageCommand ?? (_clickPageCommand = new RelayCommand(clickSwitch));
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    class RecepcionVisitaViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public RecepcionVisitaViewModel() 
        {
            BuscarVisible = false;
            CrearNuevoExpedienteEnabled = false;
            ObservacionesSpan = 7;
            UltimaModVisible = false;
            AccesoUnicoColumna = 8;
            AccesoUnicoVisible = true;
        }
        #endregion

        #region variables
        private int observacionesSpan;
        public int ObservacionesSpan
        {
            get { return observacionesSpan; }
            set { observacionesSpan = value; OnPropertyChanged("ObservacionesSpan"); }
        }
        private int accesoUnicoColumna;
        public int AccesoUnicoColumna
        {
            get { return accesoUnicoColumna; }
            set { accesoUnicoColumna = value; OnPropertyChanged("AccesoUnicoColumna"); }
        }
        private bool accesoUnicoVisible;
        public bool AccesoUnicoVisible
        {
            get { return accesoUnicoVisible; }
            set { accesoUnicoVisible = value; OnPropertyChanged("AccesoUnicoVisible"); }
        }
        private bool ultimaModVisible;
        public bool UltimaModVisible
        {
            get { return ultimaModVisible; }
            set { ultimaModVisible = value; OnPropertyChanged("UltimaModVisible"); }
        }
        private bool crearNuevoExpedienteEnabled;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
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
                return "recepcion_visita";
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
                    BuscarVisible = true;
                    break;
                case "buscar_seleccionar":
                    BuscarVisible = false;
                    break;
                case "buscar_salir":
                    BuscarVisible = false;
                    break;
            }
        }
        #endregion

        #region commands
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

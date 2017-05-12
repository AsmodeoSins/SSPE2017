using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ControlPenales
{
    class EstudioClasificacionViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public EstudioClasificacionViewModel() 
        {
            BuscarVisible = false;
            CrearNuevoExpedienteEnabled = false;
        }
        #endregion

        #region variables
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
                return "estudio_clasificacion";
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

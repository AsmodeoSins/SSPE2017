using SSP.Servidor;
using System.Collections.ObjectModel;


namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        private VM_IMPUTADOSDATOS selectedInterconexion;
        public VM_IMPUTADOSDATOS SelectedInterconexion
        {
            get { return selectedInterconexion; }
            set { selectedInterconexion = value;
                OnPropertyChanged("SelectedInterconexion"); 
            }
        }

    }
}

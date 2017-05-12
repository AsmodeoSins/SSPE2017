
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {

      
        private DateTime? fecAIn;
        public DateTime? FecAIn
        {
            get { return fecAIn; }
            set { fecAIn = value; OnPropertyChanged("FecAIn"); }
        }
        
        private short? incidenteAIn;
        public short? IncidenteAIn
        {
            get { return incidenteAIn; }
            set { incidenteAIn = value; 
                OnPropertyChanged("IncidenteAIn"); }
        }


        private ObservableCollection<TIPO_RECURSO> lstIncidentes;
        public ObservableCollection<TIPO_RECURSO> LstIncidentes
        {
            get { return lstIncidentes; }
            set { lstIncidentes = value; OnPropertyChanged("LstIncidentes"); }
        }

        private TIPO_RECURSO selectedIncidentes;
        public TIPO_RECURSO SelectedIncidentes
        {
            get { return selectedIncidentes; }
            set { selectedIncidentes = value;
            if (value.RECURSO_RESULTADO != null)
                LstIncidenteResultado = new ObservableCollection<RECURSO_RESULTADO>(value.RECURSO_RESULTADO);
            else
                LstIncidenteResultado = new ObservableCollection<RECURSO_RESULTADO>();
                LstIncidenteResultado.Insert(0, new RECURSO_RESULTADO() { RESULTADO = string.Empty, DESCR = "SELECCIONE" });
                IncidenteResultadoAIn = string.Empty;
                OnPropertyChanged("SelectedIncidentes");
            }
        }

        private ObservableCollection<RECURSO_RESULTADO> lstIncidenteResultado;
        public ObservableCollection<RECURSO_RESULTADO> LstIncidenteResultado
        {
            get { return lstIncidenteResultado; }
            set { lstIncidenteResultado = value; OnPropertyChanged("LstIncidenteResultado"); }
        }


        private string incidenteResultadoAIn;
        public string IncidenteResultadoAIn
        {
            get { return incidenteResultadoAIn; }
            set { incidenteResultadoAIn = value; OnPropertyChanged("IncidenteResultadoAIn"); }
        }

        private decimal? diasRemisionAIn;
        public decimal? DiasRemisionAIn
        {
            get { return diasRemisionAIn; }
            set { diasRemisionAIn = value; OnPropertyChanged("DiasRemisionAIn"); }
        }
    }
}

using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        
        private ObservableCollection<PERSONALIDAD> _ListEstudiosPersonalidad;
        public ObservableCollection<PERSONALIDAD> ListEstudiosPersonalidad
        {
            get { return _ListEstudiosPersonalidad; }
            set { _ListEstudiosPersonalidad = value; OnPropertyChanged("ListEstudiosPersonalidad"); }
        }
        
        private PERSONALIDAD _SelectEstudioPersonalidad;
        public PERSONALIDAD SelectEstudioPersonalidad
        {
            get { return _SelectEstudioPersonalidad; }
            set
            {
                _SelectEstudioPersonalidad = value;
                ListDesarrollosEstudio = value != null ? new ObservableCollection<PERSONALIDAD_DETALLE>(value.PERSONALIDAD_DETALLE.OrderBy(o => o.ID_DETALLE)) : null;
                OnPropertyChanged("SelectEstudioPersonalidad");
            }
        }
        
        private ObservableCollection<PERSONALIDAD_DETALLE> _ListDesarrollosEstudio;
        public ObservableCollection<PERSONALIDAD_DETALLE> ListDesarrollosEstudio
        {
            get { return _ListDesarrollosEstudio; }
            set { _ListDesarrollosEstudio = value; OnPropertyChanged("ListDesarrollosEstudio"); }
        }
        
        private EMI_INGRESO _SelectEmi;
        public EMI_INGRESO SelectEmi
        {
            get { return _SelectEmi; }
            set { _SelectEmi = value; OnPropertyChanged("SelectEmi"); }
        }

        private ObservableCollection<EMI_INGRESO> _ListEmi;
        public ObservableCollection<EMI_INGRESO> ListEmi
        {
            get { return _ListEmi; }
            set
            {
                _ListEmi = value;
                OnPropertyChanged("ListEmi");
            }
        }
    }
}
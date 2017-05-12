using SSP.Servidor;
using System.Collections.ObjectModel;
//using MvvmFramework;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        private ObservableCollection<ALIAS> listAlias = new ObservableCollection<ALIAS>();
        public ObservableCollection<ALIAS> ListAlias
        {
            get { return listAlias; }
            set
            {
                listAlias = value;
                OnPropertyChanged("ListAlias");
            }
        }
        private ObservableCollection<APODO> listApodo = new ObservableCollection<APODO>();
        public ObservableCollection<APODO> ListApodo
        {
            get { return listApodo; }
            set
            {
                listApodo = value;
                OnPropertyChanged("ListApodo");
            }
        }
        private ObservableCollection<RELACION_PERSONAL_INTERNO> listRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>();
        public ObservableCollection<RELACION_PERSONAL_INTERNO> ListRelacionPersonalInterno
        {
            get { return listRelacionPersonalInterno; }
            set { listRelacionPersonalInterno = value; OnPropertyChanged("ListRelacionPersonalInterno"); }
        }
    }
}

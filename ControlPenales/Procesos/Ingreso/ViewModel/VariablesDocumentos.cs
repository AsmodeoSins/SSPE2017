using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
//using MvvmFramework;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        //VARIABLES
        private DateTime? fechaD;
        public DateTime? FechaD
        {
            get { return fechaD; }
            set { fechaD = value; OnPropertyChanged("FechaD"); }
        }

        private string actividadD;
        public string ActividadD
        {
            get { return actividadD; }
            set { actividadD = value; OnPropertyChanged("ActividadD"); }
        }

        //VISIBLE
        private bool visibleDocumento;
        public bool VisibleDocumento
        {
            get { return visibleDocumento; }
            set { visibleDocumento = value; OnPropertyChanged("VisibleDocumento"); }
        }

        private bool visibleDocumentoVacio;
        public bool VisibleDocumentoVacio
        {
            get { return visibleDocumentoVacio; }
            set { visibleDocumentoVacio = value; OnPropertyChanged("VisibleDocumentoVacio"); }
        }

        //LISTADOS
        private ObservableCollection<IMPUTADO_DOCUMENTO> imputadoDocumentos;
        public ObservableCollection<IMPUTADO_DOCUMENTO> ImputadoDocumentos
        {
            get { return imputadoDocumentos; }
            set { imputadoDocumentos = value; OnPropertyChanged("ImputadoDocumentos"); }
        }

        private ObservableCollection<IMPUTADO_TIPO_DOCUMENTO> tiposDocumentos;
        public ObservableCollection<IMPUTADO_TIPO_DOCUMENTO> TiposDocumentos
        {
            get { return tiposDocumentos; }
            set { tiposDocumentos = value; OnPropertyChanged("TiposDocumentos"); }
        }
        //SELECTED
        private IMPUTADO_TIPO_DOCUMENTO selectedTipoDocumento;
        public IMPUTADO_TIPO_DOCUMENTO SelectedTipoDocumento
        {
            get { return selectedTipoDocumento; }
            set { selectedTipoDocumento = value; OnPropertyChanged("SelectedTipoDocumento"); }
        }

        private IMPUTADO_DOCUMENTO selectedImputadoDocumento;
        public IMPUTADO_DOCUMENTO SelectedImputadoDocumento
        {
            get { return selectedImputadoDocumento; }
            set { selectedImputadoDocumento = value; OnPropertyChanged("SelectedImputadoDocumento"); }
        }
    }
}

using ControlPenales;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReportePadronEmpleadosViewModel
    {
        #region Panatalla
        private bool reportViewerVisible = false;
        public bool ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }
        

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        #endregion

        private ObservableCollection<TIPO_EMPLEADO> _ListTipoEmpleados;

        public ObservableCollection<TIPO_EMPLEADO> ListTipoEmpleados
        {
            get { return _ListTipoEmpleados; }
            set { _ListTipoEmpleados = value; OnPropertyChanged("ListTipoEmpleados"); }
        }
        private short? _SelectTipoEmpl = -1;

        public short? SelectTipoEmpl
        {
            get { return _SelectTipoEmpl; }
            set {
                
                _SelectTipoEmpl = value; OnPropertyChanged("SelectTipoEmpl"); 
            
            
            }
        }

        private int _SelectEstatus=1;
        
        public int SelectEstatus
        {
            get { return _SelectEstatus; }
            set { _SelectEstatus = value; OnPropertyChanged("SelectEstatus"); }
        }

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}

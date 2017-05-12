using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms.Integration;

namespace ControlPenales
{
    public partial class ReporteControlInternoEdificioViewModel
    {
        private short ordenarPor = 1;
        public short OrdenarPor
        {
            get { return ordenarPor; }
            set { ordenarPor = value; OnPropertyChanged("OrdenarPor"); }
        }

        private Visibility _reportViewerVisible = Visibility.Visible;
        public Visibility ReportViewerVisible 
        {
            get { return _reportViewerVisible; }
            set 
            {
                _reportViewerVisible = value; 
                OnPropertyChanged("ReportViewerVisible"); 
            }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set 
            {
                reporte = value; 
                OnPropertyChanged("Reporte"); 
            }
        }

        private DateTime? _fechaInicio;
        public DateTime? FechaInicio 
        {
            get { return _fechaInicio; }
            set 
            {
                _fechaInicio = value;
                Validaciones();
                OnPropertyChanged("FechaInicio");
            }
        }

        private DateTime? _fechaFin;
        public DateTime? FechaFin 
        {
            get { return _fechaFin; }
            set
            {
                _fechaFin = value;
                Validaciones();
                OnPropertyChanged("FechaFin");
            }
        }

        private WindowsFormsHost repositorio;
        public WindowsFormsHost Repositorio
        {
            get { return repositorio; }
            set { repositorio = value; OnPropertyChanged("Repositorio"); }
        }
        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}

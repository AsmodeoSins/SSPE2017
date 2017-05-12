using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteDecomisoCustodioViewModel
    {

        private string nombreCustodioBuscar;
        public string NombreCustodioBuscar
        {
            get { return nombreCustodioBuscar; }
            set { nombreCustodioBuscar = value; OnPropertyChanged("NombreCustodioBuscar"); }
        }

        private int? id_Persona;
        public int? ID_Persona
        {
            get { return id_Persona; }
            set { id_Persona = value; OnPropertyChanged("ID_Persona"); }
        }


        private string paternoCustodioBuscar;
        public string PaternoCustodioBuscar
        {
            get { return paternoCustodioBuscar; }
            set { paternoCustodioBuscar = value; OnPropertyChanged("PaternoCustodioBuscar"); }
        }

        private string maternoCustodioBuscar;
        public string MaternoCustodioBuscar
        {
            get { return maternoCustodioBuscar; }
            set { maternoCustodioBuscar = value; OnPropertyChanged("MaternoCustodioBuscar"); }
        }

        private ReporteDecomisoCustodioView ventana;
        public ReporteDecomisoCustodioView Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }


        private Visibility reportViewerVisible;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }


        private bool mostrarTodos;
        public bool MostrarTodos
        {
            get { return mostrarTodos; }
            set
            {
                mostrarTodos = value;
                BuscarEnabled = !value;
                OnPropertyChanged("MostrarTodos");
            }
        }

        private bool buscarEnabled;

        public bool BuscarEnabled
        {
            get { return buscarEnabled; }
            set { buscarEnabled = value; OnPropertyChanged("BuscarEnabled"); }
        }


        const int EMPLEADO = 1;
        const int VISITA = 3;

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}

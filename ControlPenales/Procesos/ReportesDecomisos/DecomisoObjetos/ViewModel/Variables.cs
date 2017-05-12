using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteDecomisoObjetoViewModel
    {

        private List<OBJETO_TIPO> objetoTipos;
        public List<OBJETO_TIPO> ObjetoTipos
        {
            get { return objetoTipos; }
            set { objetoTipos = value; OnPropertyChanged("ObjetoTipos"); }
        }

        private OBJETO_TIPO selectedTipo;
        public OBJETO_TIPO SelectedTipo
        {
            get { return selectedTipo; }
            set { selectedTipo = value; OnPropertyChanged("SelectedTipo"); }
        }

        private ReporteDecomisoObjetoView ventana;
        public ReporteDecomisoObjetoView Ventana
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


        private bool objetoEspecifico;
        public bool ObjetoEspecifico
        {
            get { return objetoEspecifico; }
            set { objetoEspecifico = value; todosLosObjetos = !value; OnPropertyChanged("ObjetoEspecifico"); }
        }


        private bool todosLosObjetos;
        public bool TodosLosObjetos
        {
            get { return todosLosObjetos; }
            set { todosLosObjetos = value; objetoEspecifico = !value; OnPropertyChanged("TodosLosObjetos"); }
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

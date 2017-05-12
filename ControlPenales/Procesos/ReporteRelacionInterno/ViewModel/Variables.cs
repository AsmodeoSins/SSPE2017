using ControlPenales;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteRelacionInternoViewModel
    {
        #region Filtros
        private short ordenarPor = 1;
        public short OrdenarPor
        {
            get { return ordenarPor; }
            set { ordenarPor = value; OnPropertyChanged("OrdenarPor"); }
        }

        private bool incluirFoto = false;
        public bool IncluirFoto
        {
            get { return incluirFoto; }
            set { incluirFoto = value; OnPropertyChanged("IncluirFoto"); }
        }

        private bool incluirEdad = false;
        public bool IncluirEdad
        {
            get { return incluirEdad; }
            set { incluirEdad = value; OnPropertyChanged("IncluirEdad"); }
        }

        private bool incluirNCP = false;
        public bool IncluirNCP
        {
            get { return incluirNCP; }
            set { incluirNCP = value; OnPropertyChanged("IncluirNCP"); }
        }
        #endregion

        #region Panatalla
        private Visibility reportViewerVisible = Visibility.Visible;
        public Visibility ReportViewerVisible
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


        private WindowsFormsHost repositorio;
        public WindowsFormsHost Repositorio
        {
            get { return repositorio; }
            set { repositorio = value; OnPropertyChanged("Repositorio"); }
        }
        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion

    }
}

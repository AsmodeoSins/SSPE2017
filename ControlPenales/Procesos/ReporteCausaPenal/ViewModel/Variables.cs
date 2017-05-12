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
    public partial class ReporteCausaPenalViewModel
    {
        #region Filtros
        private int? dAnios;
        public int? DAnios
        {
            get { return dAnios; }
            set { dAnios = value; OnPropertyChanged("DAnios"); }
        }

        private int? dMeses;
        public int? DMeses
        {
            get { return dMeses; }
            set { dMeses = value; OnPropertyChanged("DMeses"); }
        }
        
        private int? dDias;
        public int? DDias
        {
            get { return dDias; }
            set { dDias = value; OnPropertyChanged("DDias"); }
        }
        
        private int? hAnios;
        public int? HAnios
        {
            get { return hAnios; }
            set { hAnios = value; OnPropertyChanged("HAnios"); }
        }

        private int? hMeses;
        public int? HMeses
        {
            get { return hMeses; }
            set { hMeses = value; OnPropertyChanged("HMeses"); }
        }
     
        private int? hDias;
        public int? HDias
        {
            get { return hDias; }
            set { hDias = value; OnPropertyChanged("HDias"); }
        }
      
        private bool compurgado = true;
        public bool Compurgado
        {
            get { return compurgado; }
            set { compurgado = value; OnPropertyChanged("Compurgado"); }
        }
        
        private bool porCompurgar = true;
        public bool PorCompurgar
        {
            get { return porCompurgar; }
            set { porCompurgar = value; OnPropertyChanged("PorCompurgar"); }
        }
        
        private DateTime? fec = Fechas.GetFechaDateServer;
        public DateTime? Fec
        {
            get { return fec; }
            set { fec = value; OnPropertyChanged("Fec"); }
        }

        private bool tiempoRealCompurgacion = false;
        public bool TiempoRealCompurgacion
        {
            get { return tiempoRealCompurgacion; }
            set { tiempoRealCompurgacion = value; OnPropertyChanged("TiempoRealCompurgacion"); }
        }
      
        private int? resultado;
        public int? Resultado
        {
            get { return resultado; }
            set { resultado = value; OnPropertyChanged("Resultado"); }
        }
        #endregion

        #region Reporte
        private IEnumerable<cReporteCausaPenal> lstInternos;
        public IEnumerable<cReporteCausaPenal> LstInternos
        {
            get { return lstInternos; }
            set { lstInternos = value; OnPropertyChanged("LstInternos"); }
        }
        #endregion

        #region Panatalla
        private Visibility reportViewerVisible = Visibility.Collapsed;
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

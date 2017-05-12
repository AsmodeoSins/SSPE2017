using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteGeneralDelitoViewModel : ViewModelBase
    {
        public enum OrderByValues
        {
            Ninguno = 0,
            Numero_Expediente = 1,
            Nombre_Interno = 2,
            Fecha_Ingreso = 3,
            Ubicacion = 4,
            Clasificacion_Juridica = 5
        }
        
        #region Filtros
        ObservableCollection<Orderby> _ListaOrdenamiento;
        public ObservableCollection<Orderby> ListaOrdenamiento
        {
            get { return _ListaOrdenamiento; }
            set
            {
                _ListaOrdenamiento = value;
                OnPropertyChanged("ListaOrdenamiento");
            }
        }

        bool _IncluirFoto;
        public bool IncluirFoto
        {
            get { return _IncluirFoto; }
            set
            {
                _IncluirFoto = value;
                OnPropertyChanged("IncluirFoto");
                LimpiarReporte();
            }
        }

        bool _IncluirEdad;
        public bool IncluirEdad
        {
            get { return _IncluirEdad; }
            set
            {
                _IncluirEdad = value;
                OnPropertyChanged("IncluirEdad");
                LimpiarReporte();
            }
        }

        bool _IncluirNCP;
        public bool IncluirNCP
        {
            get { return _IncluirNCP; }
            set
            {
                _IncluirNCP = value;
                OnPropertyChanged("IncluirNCP");
                LimpiarReporte();
            }
        }

        bool _Ordenamiento = true;
        public bool Ordenamiento
        {
            get { return _Ordenamiento; }
            set
            {
                _Ordenamiento = value;
                OnPropertyChanged("Ordenamiento");
                LimpiarReporte();
            }
        }

        //OrderByValues _SelectedValue;
        //public OrderByValues SelectedValue
        //{
        //    get { return _SelectedValue; }
        //    set
        //    {
        //        _SelectedValue = value;
        //        OnPropertyChanged("SelectedValue");
        //        LimpiarReporte();
        //    }
        //}

        private short ordenarPor = 1;
        public short OrdenarPor
        {
            get { return ordenarPor; }
            set { ordenarPor = value; OnPropertyChanged("OrdenarPor"); }
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

        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }

    public class Orderby
    {
        public string Text { get; set; }
        public ControlPenales.ReporteGeneralDelitoViewModel.OrderByValues Value { get; set; }
    }
}

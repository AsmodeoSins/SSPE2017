using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class ReporteAltoImpactoViewModel
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

        private DateTime? _TextFechaInicio;

        public DateTime? TextFechaInicio
        {
            get { return _TextFechaInicio; }
            set { _TextFechaInicio = value; OnPropertyChanged("TextFechaInicio"); }
        }

        private DateTime? _TextFechaFin;

        public DateTime? TextFechaFin
        {
            get { return _TextFechaFin; }
            set { _TextFechaFin = value; OnPropertyChanged("TextFechaFin"); }
        }

        #endregion
        private ObservableCollection<FUERO> _ListFueros;

        public ObservableCollection<FUERO> ListFueros
        {
            get { return _ListFueros; }
            set { _ListFueros = value; OnPropertyChanged("ListFueros"); }
        }

        private string _SelectFuero = "SELECCIONE";

        public string SelectFuero
        {
            get { return _SelectFuero; }
            set
            {

                if (value == "SELECCIONE")
                {
                    ListTitulos = null;
                    ListGrupoDelito = null;
                }
                else
                {
                    ListTitulos = new ObservableCollection<DELITO_TITULO>(new cDelitoTitulo().ObtenerTodos().Where(w => w.ID_FUERO == value).OrderBy(o => o.DESCR));

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListTitulos.Insert(0, new DELITO_TITULO() { ID_TITULO = -1, DESCR = "SELECCIONE" });
                        SelectTitulo = -1;
                    }));
                }
                
                _SelectFuero = value; OnPropertyChanged("SelectFuero");


            }
        }
        private ObservableCollection<DELITO_TITULO> _ListTitulos;

        public ObservableCollection<DELITO_TITULO> ListTitulos
        {
            get { return _ListTitulos; }
            set { _ListTitulos = value; OnPropertyChanged("ListTitulos"); }
        }

        private short? _SelectTitulo = -1;

        public short? SelectTitulo
        {
            get { return _SelectTitulo; }
            set
            {
                if (value == -1)
                {
                    ListGrupoDelito = null;
                }
                else
                {
                    ListGrupoDelito = new ObservableCollection<DELITO_GRUPO>(new cDelitoGrupo().ObtenerTodos().Where(w => w.ID_FUERO == SelectFuero && w.ID_TITULO == value).OrderBy(o => o.DESCR));

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListGrupoDelito.Insert(0, new DELITO_GRUPO() { ID_GRUPO_DELITO = -1, DESCR = "SELECCIONE" });
                        SelectGrupoDelito = -1;
                    }));
                }
                
                _SelectTitulo = value; OnPropertyChanged("SelectTitulo");


            }
        }

        private ObservableCollection<DELITO_GRUPO> _ListGrupoDelito;

        public ObservableCollection<DELITO_GRUPO> ListGrupoDelito
        {
            get { return _ListGrupoDelito; }
            set { _ListGrupoDelito = value; OnPropertyChanged("ListGrupoDelito"); }
        }

        private short? _SelectGrupoDelito = -1;

        public short? SelectGrupoDelito
        {
            get { return _SelectGrupoDelito; }
            set
            {

                _SelectGrupoDelito = value; OnPropertyChanged("SelectGrupoDelito");


            }
        }

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}

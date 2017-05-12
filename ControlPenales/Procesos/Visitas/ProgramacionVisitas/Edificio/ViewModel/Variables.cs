using Andora.UserControlLibrary;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ProgramacionVisitaEdificioViewModel
    {
        #region LISTAS
        private ObservableCollection<VISITA_DIA> listVisitaDiaAlta;
        public ObservableCollection<VISITA_DIA> ListVisitaDiaAlta
        {
            get { return listVisitaDiaAlta; }
            set { listVisitaDiaAlta = value; OnPropertyChanged("ListVisitaDiaAlta"); }
        }
        
        private ObservableCollection<VISITA_DIA> listVisitaDia;
        public ObservableCollection<VISITA_DIA> ListVisitaDia
        {
            get { return listVisitaDia; }
            set { listVisitaDia = value; OnPropertyChanged("ListVisitaDia"); }
        }
       
        private ObservableCollection<VISITA_EDIFICIO> listVisitasPorEdificio;
        public ObservableCollection<VISITA_EDIFICIO> ListVisitasPorEdificio
        {
            get { return listVisitasPorEdificio; }
            set { listVisitasPorEdificio = value; OnPropertyChanged("ListVisitasPorEdificio"); }
        }
        
        private ObservableCollection<TIPO_VISITA> _List_Tipo;
        public ObservableCollection<TIPO_VISITA> List_Tipo
        {
            get { return _List_Tipo; }
            set
            {
                _List_Tipo = value;
                OnPropertyChanged("List_Tipo");
            }
        }
        
        private ObservableCollection<EDIFICIO> _List_Edificio;
        public ObservableCollection<EDIFICIO> List_Edificio
        {
            get { return _List_Edificio; }
            set
            {
                _List_Edificio = value;
                OnPropertyChanged("List_Edificio");
            }
        }
        
        private ObservableCollection<SECTOR> _List_Sector;
        public ObservableCollection<SECTOR> List_Sector
        {
            get { return _List_Sector; }
            set
            {
                _List_Sector = value;
                OnPropertyChanged("List_Sector");
            }
        }
        
        private ObservableCollection<CELDA> _List_Celda;
        public ObservableCollection<CELDA> List_Celda
        {
            get { return _List_Celda; }
            set
            {
                _List_Celda = value;
                OnPropertyChanged("List_Celda");
            }
        }
        
        private ObservableCollection<AREA> _ListAreas;
        public ObservableCollection<AREA> ListAreas
        {
            get { return _ListAreas; }
            set { _ListAreas = value; OnPropertyChanged("ListAreas"); }
        }
        #endregion

        #region SELECT
        private short _SelectArea;
        public short SelectArea
        {
            get { return _SelectArea; }
            set { _SelectArea = value; OnPropertyChanged("SelectArea"); }
        }
        private VISITA_EDIFICIO selectVisitaEdificio;
        public VISITA_EDIFICIO SelectVisitaEdificio
        {
            get { return selectVisitaEdificio; }
            set { selectVisitaEdificio = value; OnPropertyChanged("SelectVisitaEdificio"); }
        }

        private short selectDiaVisita = -1;//6;
        public short SelectDiaVisita
        {
            get { return selectDiaVisita; }
            set { selectDiaVisita = value; OnPropertyChanged("SelectDiaVisita"); }
        }
        
        private short? selectTipoVisita = -1;
        public short? SelectTipoVisita
        {
            get { return selectTipoVisita; }
            set { selectTipoVisita = value; OnPropertyChanged("SelectTipoVisita"); }
        }
        
        private short? selectEdificio = -1;
        public short? SelectEdificio
        {
            get { return selectEdificio; }
            set
            {
                selectEdificio = value;
                OnPropertyChanged("SelectEdificio");
                if (value == null)
                {
                    List_Sector = new ObservableCollection<SECTOR>();
                    List_Sector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
                    return;
                }
                obtenerSectores(value.Value);
            }
        }
        
        private short? selectSector = -1;
        public short? SelectSector
        {
            get { return selectSector; }
            set
            {
                selectSector = value;
                OnPropertyChanged("SelectSector");
                if (value == null)
                {
                    List_Celda = new ObservableCollection<CELDA>();
                    return;
                }
                obtenerCeldas(SelectEdificio.Value, value.Value);
            }
        }
        
        private string selectCeldaInicio = "SELECCIONE";
        public string SelectCeldaInicio
        {
            get { return selectCeldaInicio; }
            set { selectCeldaInicio = value; OnPropertyChanged("SelectCeldaInicio"); }
        }
        
        private string selectCeldaFin = "SELECCIONE";
        public string SelectCeldaFin
        {
            get { return selectCeldaFin; }
            set { selectCeldaFin = value; OnPropertyChanged("SelectCeldaFin"); }
        }
        #endregion

        private short selectFechaFiltro = -1;
        public short SelectFechaFiltro
        {
            get { return selectFechaFiltro; }
            set 
            { 
                selectFechaFiltro = value; 
                OnPropertyChanged("SelectFechaFiltro"); 
            }
        }
        private Visibility emptyVisible = Visibility.Collapsed;
        public Visibility EmptyVisible
        {
            get { return emptyVisible; }
            set { emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }
        private CalendarBlackoutDatesCollection _blackoutDates;
        public CalendarBlackoutDatesCollection BlackoutDates
        {
            get
            {
                return _blackoutDates;
            }
            set
            {
                _blackoutDates = value;
                OnPropertyChanged("BlackoutDates");
            }
        }
        public string HoraEntrada
        {
            get
            {
                return DateRangeSlider.LowValue.HasValue ? DateRangeSlider.LowValue.Value.ToString("HHmm") : Convert.ToDateTime("1/1/0001 7:00:00 AM").ToString("HHmm");
            }
        }
        public string HoraSalida
        {
            get
            {
                return DateRangeSlider.HighValue.HasValue ? DateRangeSlider.HighValue.Value.ToString("HHmm") : Convert.ToDateTime("1/1/0001 7:00:00 PM").ToString("HHmm");
            }
        }

        #region Menu
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }

        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }

        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        #region Control Tiempo
        private string horaInicio;
        public string HoraInicio
        {
            get { return horaInicio; }
            set { horaInicio = value; OnPropertyChanged("HoraInicio"); }
        }

        private string minutoInicio;
        public string MinutoInicio
        {
            get { return minutoInicio; }
            set { minutoInicio = value; OnPropertyChanged("MinutoInicio"); }
        }

        private string horaFin;
        public string HoraFin
        {
            get { return horaFin; }
            set { horaFin = value; OnPropertyChanged("HoraFin"); }
        }

        private string minutoFin;
        public string MinutoFin
        {
            get { return minutoFin; }
            set { minutoFin = value; OnPropertyChanged("MinutoFin"); }
        }
        #endregion
    }
    //public class ProgramacionVisitas
    //{
    //    public DateTime FECHA { get; set; }
    //    public TIPO_VISITA TIPO_VISITA { get; set; }
    //    public EDIFICIO EDIFICIO { get; set; }
    //    public SECTOR SECTOR { get; set; }
    //    public CELDA CELDA_INICIO { get; set; }
    //    public CELDA CELDA_FIN { get; set; }
    //}
}

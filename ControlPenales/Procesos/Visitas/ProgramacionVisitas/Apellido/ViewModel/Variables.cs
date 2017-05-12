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
    partial class ProgramacionVisitaApellidoViewModel
    {
        #region Listas
        private ObservableCollection<VISITA_APELLIDO> _ListVisitasPorApellido;
        public ObservableCollection<VISITA_APELLIDO> ListVisitasPorApellido
        {
            get { return _ListVisitasPorApellido; }
            set { _ListVisitasPorApellido = value; OnPropertyChanged("ListVisitasPorApellido"); }
        }
        private ObservableCollection<string> _ListLetras;
        public ObservableCollection<string> ListLetras
        {
            get { return _ListLetras; }
            set { _ListLetras = value; OnPropertyChanged("ListLetras"); }
        }
        private ObservableCollection<TIPO_VISITA> _ListTipoVisita;
        public ObservableCollection<TIPO_VISITA> ListTipoVisita
        {
            get { return _ListTipoVisita; }
            set { _ListTipoVisita = value; OnPropertyChanged("ListTipoVisita"); }
        }
        private RangeEnabledObservableCollection<VISITA_DIA> _ListVisitaDiaFiltro;
        public RangeEnabledObservableCollection<VISITA_DIA> ListVisitaDiaFiltro
        {
            get { return _ListVisitaDiaFiltro; }
            set { _ListVisitaDiaFiltro = value; OnPropertyChanged("ListVisitaDiaFiltro"); }
        }
        private ObservableCollection<VISITA_DIA> _ListVisitaDia;
        public ObservableCollection<VISITA_DIA> ListVisitaDia
        {
            get { return _ListVisitaDia; }
            set { _ListVisitaDia = value; OnPropertyChanged("ListVisitaDia"); }
        }
        private ObservableCollection<AREA> _ListAreas;
        public ObservableCollection<AREA> ListAreas
        {
            get { return _ListAreas; }
            set { _ListAreas = value; OnPropertyChanged("ListAreas"); }
        }
        #endregion

        #region Visibles
        private Visibility _EmptyVisible;
        public Visibility EmptyVisible
        {
            get { return _EmptyVisible; }
            set { _EmptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }
        #endregion

        #region Select
        private short _SelectArea;
        public short SelectArea
        {
            get { return _SelectArea; }
            set { _SelectArea = value; OnPropertyChanged("SelectArea"); }
        }
        private short _SelectTipoVisita;
        public short SelectTipoVisita
        {
            get { return _SelectTipoVisita; }
            set { _SelectTipoVisita = value; OnPropertyChanged("SelectTipoVisita"); }
        }
        private string _SelectLetraInicial;
        public string SelectLetraInicial
        {
            get { return _SelectLetraInicial; }
            set { _SelectLetraInicial = value; OnPropertyChanged("SelectLetraInicial"); /*OnPropertyChanged("SelectLetraFinal");*/ }
        }
        private string _SelectLetraFinal;
        public string SelectLetraFinal
        {
            get { return _SelectLetraFinal; }
            set { _SelectLetraFinal = value; OnPropertyChanged("SelectLetraFinal"); /*OnPropertyChanged("SelectLetraInicial");*/ }
        }
        private VISITA_APELLIDO _SelectVisitaApellido;
        public VISITA_APELLIDO SelectVisitaApellido
        {
            get { return _SelectVisitaApellido; }
            set { _SelectVisitaApellido = value; OnPropertyChanged("SelectVisitaApellido"); }
        }
        private short _SelectDiaVisita;
        public short SelectDiaVisita
        {
            get { return _SelectDiaVisita; }
            set { _SelectDiaVisita = value; OnPropertyChanged("SelectDiaVisita"); }
        }
        private short _SelectFechaFiltro;
        public short SelectFechaFiltro
        {
            get { return _SelectFechaFiltro; }
            set { _SelectFechaFiltro = value; OnPropertyChanged("SelectFechaFiltro"); }
        }
        #endregion

        #region Text
        //private string _HorarioVisita;
        //public string HorarioVisita
        //{
        //    get { return _HorarioVisita; }
        //    set { _HorarioVisita = value; OnPropertyChanged("HorarioVisita"); }
        //}
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
        #endregion

        #region controles
        //private DateTime? horaInicio = new DateTime().Date.AddHours(7);
        //public DateTime? HoraInicio
        //{
        //    get { return horaInicio; }
        //    set { horaInicio = value; OnPropertyChanged("HoraInicio"); }
        //}

        //private DateTime? horaFinal = new DateTime().Date.AddHours(19);
        //public DateTime? HoraFinal
        //{
        //    get { return horaFinal; }
        //    set { horaFinal = value; OnPropertyChanged("HoraFinal"); }
        //}
        #endregion

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
}

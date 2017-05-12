using Andora.UserControlLibrary;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlPenales
{
    partial class BitacoraPasesPorVerificarViewModel
    {
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        private string selectAutorizaFiltro="T";
        public string SelectAutorizaFiltro
        {
            get { return selectAutorizaFiltro; }
            set { selectAutorizaFiltro = value; OnPropertyChanged("SelectAutorizaFiltro"); }
        }
        private string textNombreFiltro;
        public string TextNombreFiltro
        {
            get { return textNombreFiltro; }
            set
            {
                textNombreFiltro = value;
                if (!string.IsNullOrEmpty(value))
                {
                    ListPases = new RangeEnabledObservableCollection<VERIFICACION_PASES>();
                    ListPases.InsertRange(ListPasesTotales.Where(w => w.NOMBRE_IMPUTADO.Contains(value) || w.NOMBRE_VISITANTE.Contains(value)));
                }
                else
                {
                    ListPases = ListPasesTotales;
                }
                OnPropertyChanged("TextNombreFiltro");
            }
        }
        private DateTime? fechaFiltroInicial;
        public DateTime? FechaFiltroInicial
        {
            get { return fechaFiltroInicial; }
            set { fechaFiltroInicial = value; OnPropertyChanged("FechaFiltroInicial"); }
        }
        private DateTime? fechaFiltroFinal;
        public DateTime? FechaFiltroFinal
        {
            get { return fechaFiltroFinal; }
            set { fechaFiltroFinal = value; OnPropertyChanged("FechaFiltroFinal"); }
        }
        private RangeEnabledObservableCollection<VERIFICACION_PASES> ListPasesTotales;
        private RangeEnabledObservableCollection<VERIFICACION_PASES> listPases;
        public RangeEnabledObservableCollection<VERIFICACION_PASES> ListPases
        {
            get { return listPases; }
            set { listPases = value; OnPropertyChanged("ListPases"); }
        }
        private ObservableCollection<PASE_CATALOGO> listTipoPase;
        public ObservableCollection<PASE_CATALOGO> ListTipoPase
        {
            get { return listTipoPase; }
            set { listTipoPase = value; OnPropertyChanged("ListTipoPase"); }
        }
        private short selectTipoPase;
        public short SelectTipoPase
        {
            get { return selectTipoPase; }
            set
            {
                selectTipoPase = value;
                OnPropertyChanged("SelectTipoPase");
            }
        }

        #region Menu
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }

        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        #endregion
    }
}

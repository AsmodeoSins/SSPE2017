using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using MvvmFramework;

namespace ControlPenales
{
    partial class EscalaRiesgoViewModel
    {
        #region Escala Riesgo
        private RangeEnabledObservableCollection<ESCALA_RIESGO> lstEscalaRiesgo;
        public RangeEnabledObservableCollection<ESCALA_RIESGO> LstEscalaRiesgo
        {
            get { return lstEscalaRiesgo; }
            set { lstEscalaRiesgo = value;
                OnPropertyChanged("LstEscalaRiesgo"); }
        }

        private Visibility emptyBuscarEscalaRiesgo = Visibility.Collapsed;
        public Visibility EmptyBuscarEscalaRiesgo
        {
            get { return emptyBuscarEscalaRiesgo; }
            set { emptyBuscarEscalaRiesgo = value; OnPropertyChanged("EmptyBuscarEscalaRiesgo"); }
        }

        private ESCALA_RIESGO selectedEscalaRiesgo;
        public ESCALA_RIESGO SelectedEscalaRiesgo
        {
            get { return selectedEscalaRiesgo; }
            set { selectedEscalaRiesgo = value; OnPropertyChanged("SelectedEscalaRiesgo"); }
        }

        private string lugar;
        public string Lugar
        {
            get { return lugar; }
            set { lugar = value; OnPropertyValidateChanged("Lugar"); }
        }

        private DateTime? fecha = Fechas.GetFechaDateServer;
        public DateTime? Fecha
        {
            get { return fecha; }
            set { fecha = value; OnPropertyValidateChanged("Fecha"); }
        }

        private string paterno;
        public string Paterno
        {
            get { return paterno; }
            set { paterno = value; OnPropertyValidateChanged("Paterno"); }
        }

        private string materno;
        public string Materno
        {
            get { return materno; }
            set { materno = value; OnPropertyValidateChanged("Materno"); }
        }

        private string nombre;
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; OnPropertyValidateChanged("Nombre"); }
        }

        private string _NUC;
        public string NUC
        {
            get { return _NUC; }
            set { _NUC = value; OnPropertyValidateChanged("NUC"); }
        }

        private short? datosFamiliares;
        public short? DatosFamiliares
        {
            get { return datosFamiliares; }
            set { datosFamiliares = value;
            if (value.HasValue)
            {
                pantalla.E1.BorderBrush = null;
            }
            OnPropertyValidateChanged("DatosFamiliares");
            }
        }

        private short? aportacionEconomica;
        public short? AportacionEconomica
        {
            get { return aportacionEconomica; }
            set { aportacionEconomica = value;
            if (value.HasValue)
            {
                pantalla.E2.BorderBrush = null;
            }
            OnPropertyValidateChanged("AportacionEconomica");
            }
        }

        private short? arraigoLocalidad;
        public short? ArraigoLocalidad
        {
            get { return arraigoLocalidad; }
            set { arraigoLocalidad = value;
            if (value.HasValue)
            {
                pantalla.E3.BorderBrush = null;
            }
            OnPropertyValidateChanged("ArraigoLocalidad");
            }
        }

        private short? residencia;
        public short? Residencia
        {
            get { return residencia; }
            set { residencia = value;
            if (value.HasValue)
            {
                pantalla.E4.BorderBrush = null;
            }
            OnPropertyValidateChanged("Residencia");
            }
        }

        private short? historiaLaboral;
        public short? HistoriaLaboral
        {
            get { return historiaLaboral; }
            set { historiaLaboral = value;
            if (value.HasValue)
            {
                pantalla.E5.BorderBrush = null;
            }
            OnPropertyValidateChanged("HistoriaLaboral");
            }
        }

        private short? consumosustancias;
        public short? Consumosustancias
        {
            get { return consumosustancias; }
            set { consumosustancias = value;
            if (value.HasValue)
            {
                pantalla.E6.BorderBrush = null;
            }
            OnPropertyValidateChanged("Consumosustancias");
            }
        }

        private short? posiblePena;
        public short? PosiblePena
        {
            get { return posiblePena; }
            set { posiblePena = value;
            if (value.HasValue)
            {
                pantalla.E7.BorderBrush = null;
            }
            OnPropertyValidateChanged("PosiblePena");
            }
        }

        private short? cumplimientoCondiciones;
        public short? CumplimientoCondiciones
        {
            get { return cumplimientoCondiciones; }
            set { cumplimientoCondiciones = value; OnPropertyValidateChanged("CumplimientoCondiciones"); }
        }

        private short? antecedentesPenales;
        public short? AntecedentesPenales
        {
            get { return antecedentesPenales; }
            set { antecedentesPenales = value;
            if (value.HasValue)
            {
                pantalla.E9.BorderBrush = null;
            }
            CalculaTotal();
            OnPropertyValidateChanged("AntecedentesPenales");
            }
        }

        private short? datosFalsos;
        public short? DatosFalsos
        {
            get { return datosFalsos; }
            set { datosFalsos = value; OnPropertyValidateChanged("DatosFalsos"); }
        }

        private bool hValue = false;
        public bool HValue
        {
            get { return hValue; }
            set { hValue = value;
            if (value)
            {
                CumplimientoCondiciones = -6;
            }
            else
            {
                CumplimientoCondiciones = 0;
            }
            CalculaTotal();
                OnPropertyValidateChanged("HValue"); }
        }

        private bool jValue = false;
        public bool JValue
        {
            get { return jValue; }
            set
            {
                jValue = value;
                if (value)
                {
                    DatosFalsos = -6;
                }
                else
                {
                    DatosFalsos = 0;
                }
                CalculaTotal();
                OnPropertyValidateChanged("JValue");
            }
        }
        #endregion

        #region Resultados
        private ObservableCollection<cEscalaRiesgoCalificacion> lstCalificaciones;
        public ObservableCollection<cEscalaRiesgoCalificacion> LstCalificaciones
        {
            get { return lstCalificaciones; }
            set { lstCalificaciones = value; OnPropertyChanged("LstCalificaciones"); }
        }

        private ObservableCollection<cEscalaRiesgoRangos> lstRangos;
        public ObservableCollection<cEscalaRiesgoRangos> LstRangos
        {
            get { return lstRangos; }
            set { lstRangos = value; OnPropertyChanged("LstRangos"); }
        }


        #endregion

        #region Menu
        private bool menuReporteEnabled = true;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        private bool menuGuardarEnabled = true;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuBuscarEnabled = true;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        
        #endregion

        #region Pantalla
        EscalaRiesgoView pantalla;
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
      
        #region Antecedentes
        private bool tieneAntecedentes = false;
        public bool TieneAntecedentes
        {
            get { return tieneAntecedentes; }
            set { tieneAntecedentes = value;
            if (value)
            {
                TieneDosATresAntecedentes = TieneMasTresAntecedentes = false;
                AntecedentesEnabled = true;
                AntecedentesPenales = -6;
            }
            else
            {
                TieneDosATresAntecedentes = TieneMasTresAntecedentes = false;
                AntecedentesEnabled = false;
                AntecedentesPenales = 0;
            }

                OnPropertyChanged("TieneAntecedentes"); }
        }

        private bool tieneDosATresAntecedentes = false;
        public bool TieneDosATresAntecedentes
        {
            get { return tieneDosATresAntecedentes; }
            set { tieneDosATresAntecedentes = value;
                if(value)
                    AntecedentesPenales = -11;
                OnPropertyChanged("TieneDosATresAntecedentes"); }
        }

        private bool tieneMasTresAntecedentes = false;
        public bool TieneMasTresAntecedentes
        {
            get { return tieneMasTresAntecedentes; }
            set { tieneMasTresAntecedentes = value; 
                if(value)
                    AntecedentesPenales = -12;
                OnPropertyChanged("TieneMasTresAntecedentes"); }
        }

        private bool antecedentesEnabled = false;
        public bool AntecedentesEnabled
        {
            get { return antecedentesEnabled; }
            set { antecedentesEnabled = value; OnPropertyChanged("AntecedentesEnabled"); }
        }
        #endregion
    }
}

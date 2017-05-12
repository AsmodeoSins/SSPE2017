using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Controlador.Catalogo.Justicia.Actividades;
using SSP.Servidor;
using ControlPenales.Clases.ControlInternos;
using DPUruNet;
using ControlPenales.Clases.ControlProgramas;
using MahApps.Metro.Controls;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    public partial class ActividadesViewModel
    {


        #region Propiedades Actividad Seleccionada
        private GRUPO_HORARIO selectedActividad;
        public GRUPO_HORARIO SelectedActividad
        {
            get { return selectedActividad; }
            set
            {
                selectedActividad = value;
                OnPropertyChanged("SelectedActividad");
            }
        }

        private string responsable;
        public string Responsable
        {
            get { return responsable; }
            set
            {
                responsable = value;
                OnPropertyChanged("Responsable");
            }
        }

        private List<InternosActividad> listaInternosActividad;
        public List<InternosActividad> ListaInternosActividad
        {
            get { return listaInternosActividad; }
            set
            {
                listaInternosActividad = value;
                OnPropertyChanged("ListaInternosActividad");
            }
        }

        private InternosActividad selectedInterno;
        public InternosActividad SelectedInterno
        {
            get { return selectedInterno; }
            set
            {
                selectedInterno = value;
                OnPropertyChanged("SelectedInterno");
            }
        }

        private byte[] fotoInternoSeleccionado;
        public byte[] FotoInternoSeleccionado
        {
            get { return fotoInternoSeleccionado; }
            set
            {
                fotoInternoSeleccionado = value;
                OnPropertyChanged("FotoInternoSeleccionado");
            }
        }
        #endregion

        #region Propiedades ListaActividades
        private DateTime fechaServer = Fechas.GetFechaDateServer;
        public DateTime FechaServer
        {
            get { return fechaServer; }
            set
            {
                fechaServer = value;
                OnPropertyChanged("FechaServer");
            }
        }

        private DateTime selectedFecha = Fechas.GetFechaDateServer;
        public DateTime SelectedFecha
        {
            get { return selectedFecha; }
            set
            {
                selectedFecha = value;
                OnPropertyChanged("SelectedFecha");
            }
        }

        private List<EQUIPO_AREA> areas;
        public List<EQUIPO_AREA> Areas
        {
            get { return areas; }
            set
            {
                areas = value;
                OnPropertyChanged("Areas");
            }
        }

        private List<GRUPO_HORARIO> listaActividades = new List<GRUPO_HORARIO>();
        public List<GRUPO_HORARIO> ListaActividades
        {
            get { return listaActividades; }
            set
            {
                listaActividades = value;
                OnPropertyChanged("ListaActividades");

                if (listaActividades.Count > 0)
                    EmptyActividadesVisible = false;
                else
                    EmptyActividadesVisible = true;
            }
        }

        private bool emptyActividadesVisible;
        public bool EmptyActividadesVisible
        {
            get { return emptyActividadesVisible; }
            set
            {
                emptyActividadesVisible = value;
                OnPropertyChanged("EmptyActividadesVisible");
            }
        }
        #endregion

        #region Propiedades Menu Enableds
        private bool menuGuardarEnabled;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set 
            { 
                menuGuardarEnabled = value;
                OnPropertyChanged("MenuGuardarEnabled");
            }
        }

        private bool menuBuscarEnabled;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set 
            { 
                menuBuscarEnabled = value;
                OnPropertyChanged("MenuBuscarEnabled");
            }
        }

        private bool menuLimpiarEnabled;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set 
            { 
                menuLimpiarEnabled = value;
                OnPropertyChanged("MenuLimpiarEnabled");
            }
        }

        private bool menuReporteEnabled;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set
            {
                menuReporteEnabled = value;
                OnPropertyChanged("MenuReporteEnabled");
            }
        }

        private bool menuAyudaEnabled;
        public bool MenuAyudaEnabled
        {
            get { return menuAyudaEnabled; }
            set 
            { 
                menuAyudaEnabled = value;
                OnPropertyChanged("MenuAyudaEnabled");
            }
        }

        private bool menuSalirEnabled;
        public bool MenuSalirEnabled
        {
            get { return menuSalirEnabled; }
            set 
            {
                menuSalirEnabled = value;
                OnPropertyChanged("MenuSalirEnabled");
            }
        }
        #endregion


    }


    #region Imputado Huella
    public class Imputado_Huella
    {
        public Fmd FMD { get; set; }
        public cHuellasImputado IMPUTADO { get; set; }
        public enumTipoBiometrico tipo_biometrico { get; set; }
    }
    #endregion
}

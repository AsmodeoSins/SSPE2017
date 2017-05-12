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
using System.Timers;
using SSP.Controlador.Catalogo.Justicia;

namespace ControlPenales
{
    public partial class ActividadesViewModel
    {
        # region Copyright Quadro – 2016
        //
        // Todos los derechos reservados. La reproducción o trasmisión en su
        // totalidad o en parte, en cualquier forma o medio electrónico, mecánico
        // o similar es prohibida sin autorización expresa y por escrito del
        // propietario de este código.
        //
        // Archivo: Variables.cs
        //
        #endregion

        #region Propiedades Actividad Seleccionada
        // 
        // Propiedades requeridas para la construcción del modal y lógica (User Control) de la actividad seleccionada
        // 1.- Responsable de la actividad seleccionada
        // 2.- Foto del interno seleccionado sobre la lista que se encuentra en la 
        // 3.- Interno seleccionado
        // 4.- Actividad seleccionada
        // 5.- Lista de los internos que se encuentran inscritos en la actividad seleccionada
        // 

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
        #endregion

        #region Propiedades ListaActividades
        /// 
        ///Propiedades para la construcción de la ventana principal del módulo
        ///1.- Propiedad que permite la visualización de la etiqueta "No hay información" en caso de que no exista ninguna actividad en la fecha acorde al "DatePicker"
        ///2.- Fecha de inicio del "DatePicker" para la selección
        ///3.- Fecha seleccionada para la visualización de actividades acorde a la misma
        ///4.- Lista de Areas asignadas al equipo
        ///5.- Lista de Actividades de acuerdo a la fecha seleccionada
        ///

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
        #endregion

        #region Propiedades Enableds Menu Principal

        /// 
        /// Propiedades para la habilitación/inhabilitación de botones
        /// 1.- Permite la habilitación/inhabilitación del botón de "Buscar" en el menú
        /// 2.- Permite la habilitación/inhabilitación del botón de "Limpiar" en el menú
        /// 3.- Permite la habilitación/inhabilitación del botón de "Ayuda" en el menú
        /// 4.- Permite la habilitación/inhabilitación del botón de "Salir" en el menú
        /// 5.- Permite la habilitación/inhabilitación del botón de "Toma de Asistencia"
        /// 

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

        private bool tomaAsistenciaEnabled;
        public bool TomaAsistenciaEnabled
        {
            get { return tomaAsistenciaEnabled; }
            set
            {
                tomaAsistenciaEnabled = value;
                OnPropertyChanged("TomaAsistenciaEnabled");
            }
        }
        #endregion

        #region Propiedades Búsqueda

        ///
        /// Propiedades que se utilizan para realizar la búsqueda de internos
        /// 1.- Contiene el parámetro del Nombre del interno
        /// 2.- Contiene el parámetro del Apellido Paterno del interno
        /// 3.- Contiene el parámetro del Apellido Materno del interno
        /// 4.- Contiene el parámetro del Año de registro del interno
        /// 5.- Contiene el parámetro del Folio (ID del Imputado) de registro del interno
        /// 6.- Habilita/Deshabilita los controles de búsqueda
        /// 7.- Habilita/Deshabilita el selector de fechas
        ///

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set
            {
                nombreBuscar = value;
                OnPropertyChanged("NombreBuscar");
            }
        }

        private string apellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return apellidoPaternoBuscar; }
            set
            {
                apellidoPaternoBuscar = value;
                OnPropertyChanged("ApellidoPaternoBuscar");
            }
        }

        private string apellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return apellidoMaternoBuscar; }
            set
            {
                apellidoMaternoBuscar = value;
                OnPropertyChanged("ApellidoMaternoBuscar");
            }
        }

        private string anioBuscar;
        public string AnioBuscar
        {
            get { return anioBuscar; }
            set
            {
                anioBuscar = value;
                OnPropertyChanged("AnioBuscar");
            }
        }

        private string folioBuscar;
        public string FolioBuscar
        {
            get { return folioBuscar; }
            set
            {
                folioBuscar = value;
                OnPropertyChanged("FolioBuscar");
            }
        }

        private bool camposBusquedaEnabled;
        public bool CamposBusquedaEnabled
        {
            get { return camposBusquedaEnabled; }
            set { camposBusquedaEnabled = value; OnPropertyChanged("CamposBusquedaEnabled"); }
        }

        private bool selectedFechaEnabled;
        public bool SelectedFechaEnabled
        {
            get { return selectedFechaEnabled; }
            set { selectedFechaEnabled = value; OnPropertyChanged("SelectedFechaEnabled"); }
        }
        #endregion

        #region Constantes
        const string TRASLADO_ACTIVO = "AC";
        const string TRASLADO_EN_PROCESO = "EP";
        const string EXCARCELACION_ACTIVA = "AC";
        const string EXCARCELACION_EN_PROCESO = "EP";
        #endregion

        #region Enumerables

        /// 
        /// Enumerables de la clase
        /// 1.- Enumerable que indica los estatus posibles de un interno hacia un programa
        /// 

        private enum enumGrupoAsistenciaEstatus
        {
            ACTIVO = 1,
            CANCELADO = 2,
            JUSTIFICADO = 3
        }
        #endregion

        #region Ventanas

        /// 
        /// Ventanas utilizadas por el módulo
        /// 1.- Ventana utilizada para el módulo de búsqueda de internos
        /// 2.- Ventana utilizada para el módulo de toma de asistencia de internos
        /// 

        private BuscarInternosProgramas windowBusquedaInternos;
        public BuscarInternosProgramas WindowBusquedaInternos
        {
            get { return windowBusquedaInternos; }
            set
            {
                windowBusquedaInternos = value;
                OnPropertyChanged("WindowBusquedaInternos");
            }
        }

        private MetroWindow windowTomaDeAsistencia;
        public MetroWindow WindowTomaDeAsistencia
        {
            get { return windowTomaDeAsistencia; }
            set
            {
                windowTomaDeAsistencia = value;
                OnPropertyChanged("WindowTomaDeAsistencia");
            }
        }
        #endregion

        #region Propiedades Permisos

        /// 
        /// Propiedades que indican las operaciones que puede realizar el usuario sobre el módulo
        /// 1.- Indica si el usuario tiene el permiso de "INSERTAR"
        /// 2.- Indica si el usuario tiene el permiso de "EDITAR"
        /// 3.- Indica si el usuario tiene el permiso de "CONSULTAR"
        /// 4.- Indica si el usuario tiene el permiso de "IMPRIMIR"
        /// 

        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set
            {
                pInsertar = value;
            }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set
            {
                pEditar = value;
                TomaAsistenciaEnabled = (SelectedFecha.Year == Fechas.GetFechaDateServer.Year &&
                                             SelectedFecha.Month == Fechas.GetFechaDateServer.Month &&
                                             SelectedFecha.Day == Fechas.GetFechaDateServer.Day);
            }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
                if (value)
                {
                    SelectedFechaEnabled = value;
                    MenuBuscarEnabled = value;
                    CamposBusquedaEnabled = value;
                }
            }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set
            {
                pImprimir = value;
            }
        }
        #endregion
    }
    #region Imputado Huella
    /// <summary>
    /// Clase utilizada para alojar la información de las huellas digitales encontradas de los internos
    /// </summary>
    public class Imputado_Huella
    {
        public Fmd FMD { get; set; }
        public cHuellasImputado IMPUTADO { get; set; }
        public enumTipoBiometrico tipo_biometrico { get; set; }
    }
    #endregion
}

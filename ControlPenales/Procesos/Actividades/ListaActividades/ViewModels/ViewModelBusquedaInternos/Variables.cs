using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases.ControlProgramas;
using DPUruNet;
using SSP.Servidor;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;


namespace ControlPenales
{
    public partial class BusquedaInternoProgramasViewModel
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

        #region Propiedades Visuales Salidas


        private Brush colorEnabled;
        public Brush ColorEnabled
        {
            get { return colorEnabled; }
            set
            {
                colorEnabled = value;
                OnPropertyChanged("ColorEnabled");
            }
        }
        #endregion

        #region Propiedades Visuales NIP
        ///
        /// Propiedades requeridas para la correcta visualización del flyout de la captura del ID del custodio, así como sus indicadores visuales y la aceptación o rechazo de dicho ID.
        /// 1.- Texto que indica si el ID enviado es válido.
        /// 2.- Indica por medio de un color si el ID enviado es válido.
        /// 3.- Color de fondo para el color del botón que borra únicamente un solo número del ID.
        /// 4.- Color de fondo para el color del botón que borra todo el ID.
        /// 5.- Texto que indica que lo que se captura es un ID y no un NIP.
        /// 6.- Indica si la captura del ID es visible.
        ///

        private string checkMark;
        public string CheckMark
        {
            get { return checkMark; }
            set
            {
                checkMark = value;
                OnPropertyChanged("CheckMark");
            }
        }

        private Brush colorAprobacionNIP;
        public Brush ColorAprobacionNIP
        {
            get { return colorAprobacionNIP; }
            set
            {
                colorAprobacionNIP = value;
                RaisePropertyChanged("ColorAprobacionNIP");
            }
        }


        private Brush fondoBackSpaceNIP;
        public Brush FondoBackSpaceNIP
        {
            get { return fondoBackSpaceNIP; }
            set
            {
                fondoBackSpaceNIP = value;
                OnPropertyChanged("FondoBackSpaceNIP");
            }
        }


        private Brush fondoLimpiarNIP;
        public Brush FondoLimpiarNIP
        {
            get { return fondoLimpiarNIP; }
            set
            {
                fondoLimpiarNIP = value;
                OnPropertyChanged("FondoLimpiarNIP");
            }
        }

        public string CampoCaptura { get; set; }

        private bool capturaNIPVisible;
        public bool CapturaNIPVisible
        {
            get { return capturaNIPVisible; }
            set
            {
                capturaNIPVisible = value;
                OnPropertyChanged("CapturaNIPVisible");
            }
        }
        #endregion

        #region Propiedades Visuales Lectura Huella
        //
        //  Propiedades requeridas para la correcta visualización de la captura de huellas de los custodios, así como el indicador de aceptación o rechazo de la huella capturada.
        //  1.- Imagen que indica el resultado de la comparación de la huella capturada.
        //  2.- Indica cuando la imagen de evaluación debe ser visible.
        //  3.- Color que indica el resultado de la comparación de la huella capturada.
        //  4.- Indica la visibilidad del control "Controls:ProgressRing", para cuando existe una comparación en proceso.
        //

        private byte[] imagenEvaluacion;
        public byte[] ImagenEvaluacion
        {
            get { return imagenEvaluacion; }
            set
            {
                imagenEvaluacion = value;
                OnPropertyChanged("ImagenEvaluacion");
            }
        }

        private bool imagenEvaluacionVisible;
        public bool ImagenEvaluacionVisible
        {
            get { return imagenEvaluacionVisible; }
            set
            {
                imagenEvaluacionVisible = value;
                OnPropertyChanged("ImagenEvaluacionVisible");
            }
        }

        private Brush colorAprobacion;
        public Brush ColorAprobacion
        {
            get { return colorAprobacion; }
            set
            {
                colorAprobacion = value;
                OnPropertyChanged("ColorAprobacion");
            }
        }

        private Visibility progressRingVisible;
        public Visibility ProgressRingVisible
        {
            get { return progressRingVisible; }
            set
            {
                progressRingVisible = value;
                OnPropertyChanged("ProgressRingVisible");
            }
        }
        #endregion

        #region Propiedades Visuales Búsqueda
        /// 
        /// Propiedades visuales requeridas para la correcta visualización de la ventana
        /// 1.- Indica si los resultados de la búsqueda no tuvo ningún resultado
        /// 2.- Indica si ha seleccionado ó no, algún interno de los resultados de la búsqueda
        /// 3.- Ventana principal de búsqueda (fines visuales)
        /// 4.- Foto de ingreso de frente del resultado seleccionado 
        /// 5.- Foto de seguimiento de frente del resultado seleccionado
        /// 6.- Interno seleccionado de la búsqueda
        /// 

        private bool emptyBusquedaVisible;
        public bool EmptyBusquedaVisible
        {
            get { return emptyBusquedaVisible; }
            set
            {
                emptyBusquedaVisible = value;
                OnPropertyChanged("EmptyBusquedaVisible");
            }
        }

        private bool emptySeleccionadosVisible;
        public bool EmptySeleccionadosVisible
        {
            get { return emptySeleccionadosVisible; }
            set
            {
                emptySeleccionadosVisible = value;
                OnPropertyChanged("EmptySeleccionadosVisible");

            }
        }


        private BuscarInternosProgramas ventana;
        public BuscarInternosProgramas Ventana
        {
            get { return ventana; }
            set
            {
                ventana = value;
                OnPropertyChanged("Ventana");
            }
        }

        private byte[] selectedImputadoFotoIngreso;
        public byte[] SelectedImputadoFotoIngreso
        {
            get { return selectedImputadoFotoIngreso; }
            set
            {
                selectedImputadoFotoIngreso = value;
                OnPropertyChanged("SelectedImputadoFotoIngreso");
            }
        }

        private byte[] selectedImputadoFotoSeguimiento;
        public byte[] SelectedImputadoFotoSeguimiento
        {
            get { return selectedImputadoFotoSeguimiento; }
            set
            {
                selectedImputadoFotoSeguimiento = value;
                OnPropertyChanged("SelectedImputadoFotoSeguimiento");
            }
        }
        #endregion

        #region Propiedades Busqueda
        // 
        // Propiedades requeridas para la búsqueda de internos en las actividades, así como las fotos de los mismos al ser seleccionados
        // 1.- Interno seleccionado de una búsqueda resultante
        // 2.- Áreas que contempla el equipo
        // 3.- Lista donde se almacenan los resultados de las búsquedas
        // 4.- Año del ingreso a buscar
        // 5.- Folio/ID del ingreso a buscar
        // 6.- Nombre del ingreso a buscar
        // 7.- Apellido Paterno del ingreso a buscar
        // 8.- Apellido Materno del ingreso a buscar
        //

        private InternosActividad selectedImputado;
        public InternosActividad SelectedImputado
        {
            get { return selectedImputado; }
            set
            {
                selectedImputado = value;
                OnPropertyChanged("SelectedImputado");
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

        private List<InternosActividad> listExpediente;
        public List<InternosActividad> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                OnPropertyChanged("ListExpediente");

                if (listExpediente.Count > 0)
                    EmptyBusquedaVisible = false;
                else
                    EmptyBusquedaVisible = true;
            }
        }


        private string anioBuscar;
        public string AnioBuscar
        {
            get { return anioBuscar; }
            set { anioBuscar = value; OnPropertyChanged("AnioBuscar"); }
        }

        private string folioBuscar;
        public string FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value; OnPropertyChanged("FolioBuscar"); }
        }

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set { nombreBuscar = value; OnPropertyChanged("NombreBuscar"); }
        }


        private string apellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return apellidoPaternoBuscar; }
            set { apellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
        }


        private string apellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return apellidoMaternoBuscar; }
            set { apellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscar"); }
        }

        private enum enumGrupoAsistenciaEstatus
        {
            ACTIVO = 1,
            JUSTIFICADO = 2,
            CANCELADO = 3
        }

        const string NOTIFICADO = "S";
        const string SIN_NOTIFICAR = "N";
        #endregion

        #region Constantes
        /// 
        /// Constantes utilizadas para lectura del código
        /// 1.- Indica el estatus de excarcelacion: AUTORIZADA
        /// 2.- Indica el estatus de excarcelacion: EN PROCESO
        /// 3.- Indica el estatus de traslado: PROGRAMADO
        /// 4.- Indica el estatus de traslado: EN PROCESO
        /// 
        const string EXCARCELACION_AUTORIZADA = "AU";
        const string EXCARCELACION_EN_PROCESO = "EP";
        const string TRASLADO_PROGRAMADO = "PR";
        const string TRASLADO_EN_PROCESO = "EP";
        const string TRASLADO_ACTIVO = "AC";
        #endregion

        #region Propiedades Escolta
        /// 
        /// Propiedades utilizadas para la escolta de internos
        /// 1.- Enumerable que indica los distintos estatus posibles de una ubicación
        /// 2.- Enumerable que indica las áreas posibles a las que un interno tiene permitida la salida en caso de una actividad no programada
        /// 3.- Enumerable que indica las áreas de trabajo requeridas por los siguientes casos:
        ///     3.1.- Si existe médico
        ///     3.2.- Si el empleado que capturó huella/ID es un custodio
        /// 4.- Enumerable que indica los mensajes de resultado posibles de comparación del ID
        /// 5.- Enumerable que indica los mensajes de resultado posibles de comparación de huella
        /// 6.- Indica el tipo de salida que el interno tiene asignada
        /// 7.- Indica si la escolta de los internos esta habilitada
        /// 8.- Nombre del custodio encontrado
        /// 9.- Apellido Paterno del custodio encontrado
        /// 10.- Apellido Materno del custodio encontrado
        /// 11.- Año del custodio encontrado
        /// 12.- ID del custodio encontrado
        /// 13.- Fecha de registro del custodio encontrado
        /// 14.- Foto del custodio encontrado
        /// 15.- Custodio encontrado
        /// 16.- Lista de internos seleccionados para
        /// 

        private enum enumEstatusUbicacion
        {
            EN_ESTANCIA = 0,
            EN_TRANSITO = 1,
            EN_ACTIVIDAD = 2
        };

        private enum enumAreaSalida
        {
            SALIDA_DEL_CENTRO = 111,
            LOCUTORIOS = 85,
            AREA_MEDICA_PB = 40
        }

        private enum enumAreaTrabajo
        {
            MEDICO_DE_GUARDIA = 12,
            COMANDANCIA = 4
        }

        public enum enumMensajeNIP
        {
            ENCONTRADO = 1,
            NO_ENCONTRADO = 2
        }

        public enum enumMensajeHuella
        {
            EN_ESPERA = 0,
            ENCONTRADO = 1,
            NO_ENCONTRADO = 2,
            FALSO_POSITIVO = 3,
            HUELLA_VACIA = 4,
            EN_PROCESO = 5,
            ASIGNADO_PREVIAMENTE = 6
        }

        private enumTipoSalida? selectedJustificacion;
        public enumTipoSalida? SelectedJustificacion
        {
            get { return selectedJustificacion; }
            set
            {
                selectedJustificacion = value;
                OnPropertyChanged("SelectedJustificacion");
            }
        }

        private bool escoltarEnabled;
        public bool EscoltarEnabled
        {
            get { return escoltarEnabled; }
            set
            {
                escoltarEnabled = value;

                if (escoltarEnabled)
                {
                    ColorEnabled = new SolidColorBrush(Colors.LightYellow);
                }

                else
                {
                    ColorEnabled = new SolidColorBrush(Colors.Gray);
                    NombreCustodio = "";
                    PaternoCustodio = "";
                    MaternoCustodio = "";
                    AnioCustodio = "";
                    IDCustodio = "";
                    FotoCustodio = new Imagenes().getImagenPerson();
                    SelectedCustodio = null;
                }


                OnPropertyChanged("EscoltarEnabled");
            }
        }

        private string nombreCustodio;
        public string NombreCustodio
        {
            get { return nombreCustodio; }
            set { nombreCustodio = value; OnPropertyChanged("NombreCustodio"); }
        }

        private string paternoCustodio;
        public string PaternoCustodio
        {
            get { return paternoCustodio; }
            set { paternoCustodio = value; OnPropertyChanged("PaternoCustodio"); }
        }

        private string maternoCustodio;
        public string MaternoCustodio
        {
            get { return maternoCustodio; }
            set { maternoCustodio = value; OnPropertyChanged("MaternoCustodio"); }
        }

        private string anioCustodio;
        public string AnioCustodio
        {
            get { return anioCustodio; }
            set
            {
                anioCustodio = value;
                OnPropertyChanged("AnioCustodio");
            }
        }

        private string idCustodio;
        public string IDCustodio
        {
            get { return idCustodio; }
            set
            {
                idCustodio = value;
                OnPropertyChanged("IDCustodio");
            }
        }

        private string fechaRegistroCustodio;
        public string FechaRegistroCustodio
        {
            get { return fechaRegistroCustodio; }
            set { fechaRegistroCustodio = value; OnPropertyChanged("FechaRegistroCustodio"); }
        }


        private byte[] fotoCustodio;
        public byte[] FotoCustodio
        {
            get { return fotoCustodio; }
            set
            {
                fotoCustodio = value;
                OnPropertyChanged("FotoCustodio");
            }
        }

        private EMPLEADO selectedCustodio;
        public EMPLEADO SelectedCustodio
        {
            get { return selectedCustodio; }
            set
            {
                selectedCustodio = value;
                OnPropertyChanged("SelectedCustodio");
            }
        }

        private List<InternosActividad> listaSeleccionados;
        public List<InternosActividad> ListaSeleccionados
        {
            get { return listaSeleccionados; }
            set
            {
                listaSeleccionados = value;
                OnPropertyChanged("ListaSeleccionados");


                if (listaSeleccionados.Count > 0)
                    EmptySeleccionadosVisible = false;
                else
                    EmptySeleccionadosVisible = true;
            }
        }
        #endregion

        #region Propiedades Lectura Huella
        ///
        /// Propiedades requeridas para la lectura y comparación de huellas
        /// 1.- Dedo seleccionado para realizar la comparación
        ///
        private enumTipoBiometrico? selectedFinger;
        public enumTipoBiometrico? SelectedFinger
        {
            get { return selectedFinger; }
            set
            {
                selectedFinger = value;
                OnPropertyChanged("SelectedFinger");
            }
        }
        #endregion

        #region Propiedades Lectura ID
        /// 
        /// Propiedades requeridas para la comparación del ID del custodio
        /// 1.- ID del custodio a buscar
        /// 
        private string nipBuscar;
        public string NIPBuscar
        {
            get { return nipBuscar; }
            set
            {
                nipBuscar = value;
                OnPropertyChanged("NIPBuscar");
            }
        }
        #endregion

        #region Custodio Huella
        public class Custodio_Huella
        {
            public Fmd FMD { get; set; }
            public cHuellasPersona PERSONA { get; set; }
            public enumTipoBiometrico tipo_biometrico { get; set; }
        }
        #endregion
    }
}

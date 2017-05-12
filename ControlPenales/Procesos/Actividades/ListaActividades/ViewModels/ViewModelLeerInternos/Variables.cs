using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales
{
    public partial class LeerInternosViewModel
    {
        #region Propiedades Visuales Asistencia
        /// <summary>
        /// Propiedades requeridas para la visualización de una asistencia autorizada/no autorizada.
        /// 1.- Foto del interno que toma lectura
        /// </summary>

        private byte[] fotoLectura;
        public byte[] FotoLectura
        {
            get { return fotoLectura; }
            set
            {
                fotoLectura = value;
                OnPropertyChanged("FotoLectura");
            }
        }
        #endregion

        #region Propiedades Asistencia
        /// <summary>
        /// Propiedades requeridas para la correcta toma de asistencia y control de la misma.
        /// 1.- Permite o niega el acceso al área dependiendo del valor del Checkbox.
        /// 2.- Interno que tomo asistencia recientemente.
        /// 3.- Lista para la visualización de la asistencia.
        /// 4.- Enumerable que indica los estatus posibles para una participación hacia un grupo en particular.
        /// </summary>

        private bool asistenciaChecked;
        public bool AsistenciaChecked
        {
            get { return asistenciaChecked; }
            set
            {
                asistenciaChecked = value;
                OnPropertyChanged("AsistenciaChecked");

                if (value)
                    SelectedAsistencia = new GRUPO_ASISTENCIA()
                    {
                        ASISTENCIA = 1,
                        EMP_APROBADO = SelectedAsistencia.EMP_APROBADO,
                        EMP_COORDINACION = SelectedAsistencia.EMP_COORDINACION,
                        EMP_FECHA = SelectedAsistencia.EMP_FECHA,
                        EMPALME = SelectedAsistencia.EMPALME,
                        ESTATUS = SelectedAsistencia.ESTATUS,
                        FEC_REGISTRO = SelectedAsistencia.FEC_REGISTRO,
                        GRUPO_ASISTENCIA_ESTATUS = SelectedAsistencia.GRUPO_ASISTENCIA_ESTATUS,
                        GRUPO_HORARIO = SelectedAsistencia.GRUPO_HORARIO,
                        GRUPO_PARTICIPANTE = SelectedAsistencia.GRUPO_PARTICIPANTE,
                        ID_ACTIVIDAD = SelectedAsistencia.ID_ACTIVIDAD,
                        ID_CENTRO = SelectedAsistencia.ID_CENTRO,
                        ID_CONSEC = SelectedAsistencia.ID_CONSEC,
                        ID_GRUPO = SelectedAsistencia.ID_GRUPO,
                        ID_GRUPO_HORARIO = SelectedAsistencia.ID_GRUPO_HORARIO,
                        ID_TIPO_PROGRAMA = SelectedAsistencia.ID_TIPO_PROGRAMA
                    };
                else
                    SelectedAsistencia = new GRUPO_ASISTENCIA()
                    {
                        ASISTENCIA = null,
                        EMP_APROBADO = SelectedAsistencia.EMP_APROBADO,
                        EMP_COORDINACION = SelectedAsistencia.EMP_COORDINACION,
                        EMP_FECHA = SelectedAsistencia.EMP_FECHA,
                        EMPALME = SelectedAsistencia.EMPALME,
                        ESTATUS = SelectedAsistencia.ESTATUS,
                        FEC_REGISTRO = SelectedAsistencia.FEC_REGISTRO,
                        GRUPO_ASISTENCIA_ESTATUS = SelectedAsistencia.GRUPO_ASISTENCIA_ESTATUS,
                        GRUPO_HORARIO = SelectedAsistencia.GRUPO_HORARIO,
                        GRUPO_PARTICIPANTE = SelectedAsistencia.GRUPO_PARTICIPANTE,
                        ID_ACTIVIDAD = SelectedAsistencia.ID_ACTIVIDAD,
                        ID_CENTRO = SelectedAsistencia.ID_CENTRO,
                        ID_CONSEC = SelectedAsistencia.ID_CONSEC,
                        ID_GRUPO = SelectedAsistencia.ID_GRUPO,
                        ID_GRUPO_HORARIO = SelectedAsistencia.ID_GRUPO_HORARIO,
                        ID_TIPO_PROGRAMA = SelectedAsistencia.ID_TIPO_PROGRAMA
                    };




            }
        }

        private GRUPO_ASISTENCIA selectedAsistencia;
        public GRUPO_ASISTENCIA SelectedAsistencia
        {
            get { return selectedAsistencia; }
            set
            {
                selectedAsistencia = value;
                OnPropertyChanged("SelectedAsistencia");
            }
        }

        private List<GRUPO_ASISTENCIA> listaAsistencias;
        public List<GRUPO_ASISTENCIA> ListaAsistencias
        {
            get { return listaAsistencias; }
            set
            {
                listaAsistencias = value;
                OnPropertyChanged("ListaAsistencias");
            }
        }

        private enum enumGrupoAsistenciaEstatus
        {
            ACTIVO = 1,
            JUSTIFICADO = 2,
            CANCELADO = 3
        }
        #endregion

        #region Propiedades Visuales Incidencia
        private bool capturaIncidenciaVisible;

        public bool CapturaIncidenciaVisible
        {
            get { return capturaIncidenciaVisible; }
            set { capturaIncidenciaVisible = value; OnPropertyChanged("CapturaIncidenciaVisible"); }
        }

        private string incidenciaNIP;
        public string IncidenciaNIP
        {
            get { return incidenciaNIP; }
            set { incidenciaNIP = value; OnPropertyChanged("IncidenciaNIP"); }
        }

        private string textoIncidencia;
        public string TextoIncidencia
        {
            get { return textoIncidencia; }
            set { textoIncidencia = value; OnPropertyChanged("TextoIncidencia"); }
        }



        public enum enumIncidenteTipo : short
        {
            NORMAL = 1
        }

        const string INCIDENTE_PENDIENTE = "P";
        #endregion


        #region Propiedades Visuales Lectura NIP

        /// <summary>
        /// Propiedades requeridas para el muestreo de mensajes de captura del NIP.
        /// 1.- Texto que indica el resultado de una comparación del NIP ingresado.
        /// 2.- Indica el color del resultado de una comparacion del NIP ingresado.
        /// 3.- Indica el color del botón de "Borrar un caractér" de la captura del NIP.
        /// 4.- Indica el color del botón de "Borrar todo el NIP" de la captura del NIP.
        /// 5.- Indica lo que se esta capturando (NIP ó ID).
        /// 6.- Habilita o deshabilita la captura del NIP.
        /// </summary>

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

        /// <summary>
        /// Propiedades requeridas para el muestreo de mensajes de captura de huellas.
        /// 1.- Indica por medio de una imagen el resultado de comparación de una huella.
        /// 2.- Indica cuándo una huella esta siendo comparada.
        /// 3.- Habilita o deshabilita la visualización de la imagen de resultado de comparación.
        /// 4.- Indica el color del resultado de una comparación de huella.
        /// 5.- Habilita o deshabilita el mensaje del lector de huellas.
        /// </summary>

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

        private bool progressRingVisible;
        public bool ProgressRingVisible
        {
            get { return progressRingVisible; }
            set
            {
                progressRingVisible = value; OnPropertyChanged("ProgressRingVisible");
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

        private bool scannerMessageVisible;
        public bool ScannerMessageVisible
        {
            get { return scannerMessageVisible; }
            set { scannerMessageVisible = value; OnPropertyChanged("ScannerMessageVisible"); }
        }
        #endregion

        #region Propiedades Búsqueda Imputado NIP

        /// <summary>
        /// Propiedades requeridas para la captura del NIP.
        /// 1.- Campo donde se captura el NIP.
        /// </summary>

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

        #region Propiedades Búsqueda Imputado Huella

        /// <summary>
        /// Propiedades requeridas para la comparación de huellas.
        /// 1.- Lista que almacena el universo de huellas sobre el cuál comparar.
        /// 2.- Áreas que contempla el equipo.
        /// 3.- Muestra si la lectura de la huella fue exitosa.
        /// 4.- Indica el tipo de biométrico a evaluar.
        /// </summary>

        private List<Imputado_Huella> huellas_Imputados;
        public List<Imputado_Huella> Huellas_Imputados
        {
            get { return huellas_Imputados; }
            set
            {
                huellas_Imputados = value;
                OnPropertyChanged("Huellas_Imputados");
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

        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }

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

        #region Propiedades Enums

        /// <summary>
        /// Propiedades Enumerables
        /// 1.- Enumerable requerido para conocer el estatus de la ubicación de un interno.
        /// </summary>
        public enum EstatusUbicacion
        {
            EN_TRANSITO = 1,
            ACTIVIDAD = 2
        }
        #endregion

        #region Resultado Busqueda Biómetrico
        public class ResultadoBusquedaBiometrico
        {
            public string Expediente { get; set; }
            public string NIP { get; set; }
            public string APaterno { get; set; }
            public string AMaterno { get; set; }
            public string Nombre { get; set; }
            public ImageSource Foto { get; set; }
            public IMPUTADO Imputado { get; set; }
            public SSP.Servidor.PERSONA Persona { get; set; }
        }
        #endregion
    }
}

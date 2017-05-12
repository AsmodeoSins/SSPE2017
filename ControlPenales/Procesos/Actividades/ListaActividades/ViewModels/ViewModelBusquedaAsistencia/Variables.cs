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
    public partial class BusquedaAsistenciaViewModel
    {

        #region Propiedades Visuales Asistencia
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

        private Visibility showFotoLectura = Visibility.Visible;
        public Visibility ShowFotoLectura
        {
            get { return showFotoLectura; }
            set
            {
                showFotoLectura = value;
                OnPropertyChanged("ShowFotoLectura");
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
        #endregion

        #region Propiedades Visuales NIP
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
        private byte[] imagen;
        public byte[] Imagen
        {
            get { return imagen; }
            set
            {
                imagen = value;
                OnPropertyChanged("Imagen");
            }
        }

        private Visibility showLine = Visibility.Visible;
        public Visibility ShowLine
        {
            get { return showLine; }
            set
            {
                showLine = value;
                OnPropertyChanged("ShowLine");
            }
        }

        private Visibility showPropertyImage = Visibility.Visible;
        public Visibility ShowPropertyImage
        {
            get { return showPropertyImage; }
            set { showPropertyImage = value; }
        }

        private bool asistenciaBiometricaSelected;
        public bool AsistenciaBiometricaSelected
        {
            get { return asistenciaBiometricaSelected; }
            set
            {
                asistenciaBiometricaSelected = value;
                OnPropertyChanged("AsistenciaBiometricaSelected");
            }
        }

        private bool asistenciaBiometricaEnabled;
        public bool AsistenciaBiometricaEnabled
        {
            get { return asistenciaBiometricaEnabled; }
            set
            {
                asistenciaBiometricaEnabled = value;
                OnPropertyChanged("AsistenciaBiometricaEnabled");
            }
        }

        private bool asistenciaNIPSelected;
        public bool AsistenciaNIPSelected
        {
            get { return asistenciaNIPSelected; }
            set
            {
                asistenciaNIPSelected = value;
                OnPropertyChanged("AsistenciaNIPSelected");
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

        private bool imagenHuellaVisible;
        public bool ImagenHuellaVisible
        {
            get { return imagenHuellaVisible; }
            set
            {
                imagenHuellaVisible = value;
                OnPropertyChanged("ImagenHuellaVisible");
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
        #endregion

        #region Propiedades Búsqueda Imputado NIP
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


        private enum enumGrupoAsistenciaEstatus
        {
            ACTIVO = 1,
            JUSTIFICADO = 2,
            CANCELADO = 3
        }
        #endregion

        #region Propiedades Enums
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

using ControlPenales.Clases.ControlProgramas;
using SSP.Servidor;
using System.Collections.Generic;
using System.Windows.Media;


namespace ControlPenales
{
    public partial class BusquedaInternoProgramasViewModel
    {

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

        #endregion

        #region Propiedades Busqueda

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


        private BuscarInternosProgramas ventanaBusqueda;
        public BuscarInternosProgramas VentanaBusqueda
        {
            get { return ventanaBusqueda; }
            set
            {
                ventanaBusqueda = value;
                OnPropertyChanged("VentanaBusqueda");
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

        private List<InternosActividad> listaImputados;
        public List<InternosActividad> ListaImputados
        {
            get { return listaImputados; }
            set
            {
                listaImputados = value;
                OnPropertyChanged("ListaImputados");
            }
        }

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

        private List<InternosActividad> listExpediente;
        public List<InternosActividad> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                OnPropertyChanged("ListExpediente");
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

        #region Propiedades Escolta
        private bool escoltarEnabled;
        public bool EscoltarEnabled
        {
            get { return escoltarEnabled; }
            set
            {
                escoltarEnabled = value;

                if (escoltarEnabled)
                {
                    EscoltarInternosEnabled = true;
                    ColorEnabled = new SolidColorBrush(Colors.LightYellow);
                }

                else
                {
                    EscoltarInternosEnabled = false;
                    ColorEnabled = new SolidColorBrush(Colors.Gray);
                    NombreCustodio = "";
                    PaternoCustodio = "";
                    MaternoCustodio = "";
                    AnioCustodio = "";
                    IDCustodio = "";
                    FotoCustodio = new Imagenes().getImagenPerson();
                }


                OnPropertyChanged("EscoltarEnabled");
            }
        }


        private bool escoltarInternosEnabled;
        public bool EscoltarInternosEnabled
        {
            get { return escoltarInternosEnabled; }
            set
            {
                escoltarInternosEnabled = value;

                OnPropertyChanged("EscoltarInternosEnabled");
            }
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
            }
        }

        public enum enumMensajeNIP
        {
            ENCONTRADO = 1,
            NO_ENCONTRADO = 2
        }

        public enum enumMensajeHuella
        {
            ENCONTRADO = 1,
            NO_ENCONTRADO = 2,
            FALSO_POSITIVO = 3
        }


        #endregion
    }
}

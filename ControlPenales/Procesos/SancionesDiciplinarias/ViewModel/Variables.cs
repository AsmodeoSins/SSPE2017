using ControlPenales;
using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
     partial class SancionesDisciplinariasViewModel : ValidationViewModelBase
     {
         #region Sanciones
         private string tituloPopUp;
        public string TituloPopUp
         {
             get { return tituloPopUp; }
             set { tituloPopUp = value; OnPropertyChanged("TituloPopUp"); }
         }

         private ObservableCollection<INCIDENTE> lstIncidente;
         public ObservableCollection<INCIDENTE> LstIncidente
         {
             get { return lstIncidente; }
             set { lstIncidente = value; OnPropertyChanged("LstIncidente"); }
         }

         private INCIDENTE selectedIncidente;
         public INCIDENTE SelectedIncidente
         {
             get { return selectedIncidente; }
             set { selectedIncidente = value;
             if (value != null)
             {
                 if (value.SANCION != null)
                 {
                     EmptySanciones = value.SANCION.Count > 0 ? false : true;
                 }
                 else
                     EmptySanciones = true;
             }
             
                 OnPropertyChanged("SelectedIncidente"); }
         }

         private ObservableCollection<INCIDENTE_TIPO> lstIncidenteTipo;
         public ObservableCollection<INCIDENTE_TIPO> LstIncidenteTipo
         {
             get { return lstIncidenteTipo; }
             set { lstIncidenteTipo = value; OnPropertyChanged("LstIncidenteTipo"); }
         }

         private INCIDENTE_TIPO selectedIncidenteTipo;
         public INCIDENTE_TIPO SelectedIncidenteTipo
         {
             get { return selectedIncidenteTipo; }
             set { selectedIncidenteTipo = value; OnPropertyChanged("SelectedIncidenteTipo"); }
         }

         private short? idIncidenteTipo;
         public short? IdIncidenteTipo
         {
             get { return idIncidenteTipo; }
             set { idIncidenteTipo = value; OnPropertyChanged("IdIncidenteTipo"); }
         }

         private DateTime? fecIncidencia;
         public DateTime? FecIncidencia
         {
             get { return fecIncidencia; }
             set { fecIncidencia = value;
                 SetValidacionesIncidente();
                 OnPropertyChanged("FecIncidencia"); }
         }

         private string motivo;
         public string Motivo
         {
             get { return motivo; }
             set { motivo = value; OnPropertyChanged("Motivo"); }
         }

         private ObservableCollection<SANCION> lstSancion;
         public ObservableCollection<SANCION> LstSancion
         {
             get { return lstSancion; }
             set { lstSancion = value; OnPropertyChanged("LstSancion"); }
         }

         private short selectedSancion;//un campo de tipo sancion para q sea seleccionado
         public short SelectedSancion
         {
             get { return selectedSancion; }
             set { selectedSancion = value; OnPropertyChanged("SelectedSancion"); }
         }

         private SANCION sancionSelected;
         public SANCION SancionSelected
         {
             get { return sancionSelected; }
             set { sancionSelected = value; OnPropertyChanged("SancionSelected"); }
         }


         private ObservableCollection<SANCION_TIPO> listTipoSanciones;
         public ObservableCollection<SANCION_TIPO> ListTipoSanciones
         {
             get { return listTipoSanciones; }
             set { listTipoSanciones = value; OnPropertyChanged("ListTipoSanciones"); }
         }

         private SANCION_TIPO selectSancion;
         public SANCION_TIPO SelectSancion
         {
             get { return selectSancion; }
             set { selectSancion = value;
             //if (value != null)
             //    IdSancionTipo = value.ID_SANCION;
             selectSancion = value;

             OnPropertyChanged("SelectSancion"); }
         }

         private DateTime _MinimunDate = Fechas.GetFechaDateServer;
         public DateTime MinimunDate
         {
             get { return _MinimunDate; }
             set
             {
                 FecInicioSancion = value;
                 OnPropertyChanged("MinimunDate");
             }
         }

         private DateTime _MaximumDate = Fechas.GetFechaDateServer.AddYears(2);
         public DateTime MaximumDate
         {
             get { return _MaximumDate; }
             set
             {
                 FecFinSancion = value;
                 OnPropertyChanged("MaximumDate");
             }
         }

         private DateTime _FechaLowerVal;
         public DateTime FechaLowerVal
         {
             get { return _FechaLowerVal; }
             set
             {
                 _FechaLowerVal = value;
                 FecInicioSancion = value;
                 OnPropertyChanged("FechaLowerVal");
             }
         }

         private DateTime _FechaUpperVal;
         public DateTime FechaUpperVal
         {
             get { return _FechaUpperVal; }
             set
             {
                 _FechaUpperVal = value;
                 FecFinSancion = value;
                 OnPropertyChanged("FechaUpperVal");
             }
         }

         private DateTime _HoraLowerVal;
         public DateTime HoraLowerVal
         {
             get { return _HoraLowerVal; }
             set
             {
                 _HoraLowerVal = value;
                 OnPropertyChanged("HoraLowerVal");
             }
         }

         private DateTime _HoraUpperVal;
         public DateTime HoraUpperVal
         {
             get { return _HoraUpperVal; }
             set
             {
                 _HoraUpperVal = value;
                 OnPropertyChanged("HoraUpperVal");
             }
         }

         private short idSancionTipo = -1;
         public short IdSancionTipo
         {
             get { return idSancionTipo; }
             set { idSancionTipo = value; OnPropertyChanged("IdSancionTipo"); }
         }

         private DateTime? fecInicioSancion;
         public DateTime? FecInicioSancion
         {
             get { return fecInicioSancion; }
             set { fecInicioSancion = value;
                 SetValidacionesSancion();
                 OnPropertyChanged("FecInicioSancion"); }
         }

         private DateTime? fecFinSancion;
         public DateTime? FecFinSancion
         {
             get { return fecFinSancion; }
             set { fecFinSancion = value;
                 SetValidacionesSancion();
                 OnPropertyChanged("FecFinSancion"); }
         }

         private bool emptyIncidente = true;
         public bool EmptyIncidente
         {
             get { return emptyIncidente; }
             set { emptyIncidente = value; OnPropertyChanged("EmptyIncidente"); }
         }

         private bool emptySanciones = true;
         public bool EmptySanciones
         {
             get { return emptySanciones; }
             set { emptySanciones = value; OnPropertyChanged("EmptySanciones"); }
         }

         private DateTime? fechaInicio;
         public DateTime? FechaInicio
         {
             get { return fechaInicio; }
             set { fechaInicio = value; OnPropertyChanged("FechaInicio"); }
         }

         private DateTime? fechaFin;
         public DateTime? FechaFin
         {
             get { return fechaFin; }
             set { fechaFin = value; OnPropertyChanged("FechaFin"); }
         }

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

         private Visibility vAgregarSancion = Visibility.Visible;
         public Visibility VAgregarSancion
         {
             get { return vAgregarSancion; }
             set { vAgregarSancion = value; OnPropertyChanged("VAgregarSancion"); }
         }

         private Visibility vEditarSancion = Visibility.Visible;
         public Visibility VEditarSancion
         {
             get { return vEditarSancion; }
             set { vEditarSancion = value; OnPropertyChanged("VEditarSancion"); }
         }

         private Visibility vEliminarSancion = Visibility.Visible;
         public Visibility VEliminarSancion
         {
             get { return vEliminarSancion; }
             set { vEliminarSancion = value; OnPropertyChanged("VEliminarSancion"); }
         }
         #endregion

         #region Buscar
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
        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set
            {
                nombreBuscar = value; OnPropertyChanged("NombreBuscar");
            }
        }
        private int? anioBuscar;
        public int? AnioBuscar
        {
            get { return anioBuscar; }
            set { anioBuscar = value; OnPropertyChanged("AnioBuscar"); }
        }
        private int? folioBuscar;
        public int? FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value; OnPropertyChanged("FolioBuscar"); }
        }
        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }
        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set
            {
                imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
            }
        }
        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set
            {
                imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
            }
        }

        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set { listExpediente = value; OnPropertyChanged("ListExpediente"); }
        }
       

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (selectExpediente.INGRESO!=null && selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = selectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    }
                    else
                        EmptyIngresoVisible = true;

                    //OBTENEMOS FOTO DE FRENTE
                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        else
                            ImagenImputado = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenImputado = new Imagenes().getImagenPerson();
                }
                else
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    EmptyIngresoVisible = true;
                }
                OnPropertyChanged("SelectExpediente");
            }
        }
        
        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                {
                    OnPropertyChanged("SelectIngreso");
                    return;
                }
                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectIngreso");
            }
        }

        private INGRESO IngresoAux;

        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value; OnPropertyChanged("SelectedIngreso"); }
        }

        private bool emptyExpedienteVisible = true;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private bool emptyIngresoVisible = true;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private int? anioD;
        public int? AnioD
        {
            get { return anioD; }
            set { anioD = value; OnPropertyChanged("AnioD"); }
        }

        private int? folioD;
        public int? FolioD
        {
            get { return folioD; }
            set { folioD = value; OnPropertyChanged("FolioD"); }
        }

        private string paternoD;
        public string PaternoD
        {
            get { return paternoD; }
            set { paternoD = value; OnPropertyChanged("PaternoD"); }
        }

        private string maternoD;
        public string MaternoD
        {
            get { return maternoD; }
            set { maternoD = value; OnPropertyChanged("MaternoD"); }
        }

        private string nombreD;
        public string NombreD
        {
            get { return nombreD; }
            set { nombreD = value; OnPropertyChanged("NombreD"); }
        }

        private short? ingresosD;
        public short? IngresosD
        {
            get { return ingresosD; }
            set { ingresosD = value; OnPropertyChanged("IngresosD"); }
        }

        private string ubicacionD;
        public string UbicacionD
        {
            get { return ubicacionD; }
            set { ubicacionD = value; OnPropertyChanged("UbicacionD"); }
        }

        private string tipoSeguridadD;
        public string TipoSeguridadD
        {
            get { return tipoSeguridadD; }
            set { tipoSeguridadD = value; OnPropertyChanged("TipoSeguridadD"); }
        }

        private DateTime? fecIngresoD;
        public DateTime? FecIngresoD
        {
            get { return fecIngresoD; }
            set { fecIngresoD = value; OnPropertyChanged("FecIngresoD"); }
        }

        private string clasificacionJuridicaD;
        public string ClasificacionJuridicaD
        {
            get { return clasificacionJuridicaD; }
            set { clasificacionJuridicaD = value; OnPropertyChanged("ClasificacionJuridicaD"); }
        }

        private string estatusD;
        public string EstatusD
        {
            get { return estatusD; }
            set { estatusD = value; OnPropertyChanged("EstatusD"); }
        }


        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
#endregion

         #region Pantalla
            private bool crearNuevoExpedienteEnabled = false;
            public bool CrearNuevoExpedienteEnabled
            {
                get { return crearNuevoExpedienteEnabled; }
                set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
            }
       #endregion

         #region Configuracion Permisos
            private bool pInsertar = false;
            public bool PInsertar
            {
                get { return pInsertar; }
                set
                {
                    pInsertar = value;
                    //if (value)
                    //    MenuGuardarEnabled = value;
                }
            }

            private bool pEditar = false;
            public bool PEditar
            {
                get { return pEditar; }
                set { pEditar = value; }
            }

            private bool pConsultar = false;
            public bool PConsultar
            {
                get { return pConsultar; }
                set
                {
                    pConsultar = value;
                    if (value)
                        MenuBuscarEnabled = value;
                }
            }

            private bool pImprimir = false;
            public bool PImprimir
            {
                get { return pImprimir; }
                set
                {
                    pImprimir = value;
                    if (value)
                        MenuReporteEnabled = value;
                }
            }
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

         #region Certificado Medico
         private int ID_CERTIFICADO_MEDICO = 0;
         #endregion
     }
}

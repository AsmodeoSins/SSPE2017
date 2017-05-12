using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    partial class ReAgendaTrasladoVirtualViewModel
    {
        #region Variables privadas
        private short?[] estatus_administrativos_inactivos = null;
        private INGRESO selectedIngreso = null;
        private MODO_REAGENDA modo_guardar_seleccionado = MODO_REAGENDA.CITA_MEDICA;
        private int otro_hospital;
        private bool isfechacitarequerida = false;
        #endregion

        #region Busqueda Citas Medicas Traslado Virtual
        private int? anioBuscarTV = null;
        public int? AnioBuscarTV
        {
            get { return anioBuscarTV; }
            set { anioBuscarTV = value; RaisePropertyChanged("AnioBuscarTV"); }
        }

        private int? folioBuscarTV = null;
        public int? FolioBuscarTV
        {
            get { return folioBuscarTV; }
            set { folioBuscarTV = value; RaisePropertyChanged("FolioBuscarTV"); }
        }

        private string nombreBuscarTV = string.Empty;
        public string NombreBuscarTV
        {
            get { return nombreBuscarTV; }
            set { nombreBuscarTV = value; RaisePropertyChanged("NombreBuscarTV"); }
        }

        private string apellidoPaternoBuscarTV = string.Empty;
        public string ApellidoPaternoBuscarTV
        {
            get { return apellidoPaternoBuscarTV; }
            set { apellidoPaternoBuscarTV = value; RaisePropertyChanged("ApellidoPaternoBuscarTV"); }
        }

        private string apellidoMaternoBuscarTV = string.Empty;
        public string ApellidoMaternoBuscarTV
        {
            get { return apellidoPaternoBuscarTV; }
            set { apellidoPaternoBuscarTV = value; RaisePropertyChanged("ApellidoMaternoBuscarTV"); }
        }

        private ObservableCollection<CENTRO> lstCentro_OrigenBuscar = null;
        public ObservableCollection<CENTRO> LstCentro_OrigenBuscar
        {
            get { return lstCentro_OrigenBuscar;  }
            set { lstCentro_OrigenBuscar = value; RaisePropertyChanged("LstCentro_OrigenBuscar"); }
        }

        private short selectedCentro_OrigenBuscarValue = -1;
        public short SelectedCentro_OrigenBuscarValue
        {
            get { return selectedCentro_OrigenBuscarValue; }
            set { selectedCentro_OrigenBuscarValue = value; RaisePropertyChanged("SelectedCentro_OrigenBuscarValue"); }
        }

        private DateTime? fechaInicialBuscarTV = DateTime.Now;
        public DateTime? FechaInicialBuscarTV
        {
            get { return fechaInicialBuscarTV; }
            set { fechaInicialBuscarTV = value; RaisePropertyChanged("FechaInicialBuscarTV"); }
        }

        private DateTime? fechaFinalBuscarTV = DateTime.Now;
        public DateTime? FechaFinalBuscarTV
        {
            get { return fechaFinalBuscarTV; }
            set { fechaFinalBuscarTV = value; RaisePropertyChanged("FechaFinalBuscarTV"); }
        }

        private bool isFechaIniBusquedaTVValida = true;
        public bool IsFechaIniBusquedaTVValida
        {
            get { return isFechaIniBusquedaTVValida; }
            set { isFechaIniBusquedaTVValida = value; RaisePropertyChanged("IsFechaIniBusquedaTVValida"); }
        }

        private ObservableCollection<TRASLADO_VIRTUAL_INGRESO> listaTVBusqueda=null;
        public ObservableCollection<TRASLADO_VIRTUAL_INGRESO> ListaTVBusqueda
        {
            get { return listaTVBusqueda; }
            set { listaTVBusqueda = value; RaisePropertyChanged("ListaTVBusqueda"); }
        }

        private TRASLADO_VIRTUAL_INGRESO selectedTVBusqueda = null;
        public TRASLADO_VIRTUAL_INGRESO SelectedTVBusqueda
        {
            get { return selectedTVBusqueda; }
            set { selectedTVBusqueda = value; RaisePropertyChanged("SelectedTVBusqueda"); }
        }
        #endregion

        #region Datos del ingreso
        private string textAnioImputado = string.Empty;
        public string TextAnioImputado
        {
            get { return textAnioImputado; }
            set { textAnioImputado = value; RaisePropertyChanged("TextAnioImputado"); }
        }

        private string textFolioImputado = string.Empty;
        public string TextFolioImputado
        {
            get { return textFolioImputado; }
            set { textFolioImputado = value; RaisePropertyChanged("TextFolioImputado"); }
        }

        private string textPaternoImputado = string.Empty;
        public string TextPaternoImputado
        {
            get { return textPaternoImputado; }
            set { textPaternoImputado = value; RaisePropertyChanged("TextPaternoImputado"); }
        }

        private string textMaternoImputado = string.Empty;
        public string TextMaternoImputado
        {
            get { return textMaternoImputado; }
            set { textMaternoImputado = value; RaisePropertyChanged("TextMaternoImputado"); }
        }

        private string textNombreImputado = string.Empty;
        public string TextNombreImputado
        {
            get { return textNombreImputado; }
            set { textNombreImputado = value; RaisePropertyChanged("TextNombreImputado"); }
        }

        private string textSexoImputado = string.Empty;
        public string TextSexoImputado
        {
            get { return textSexoImputado; }
            set { textSexoImputado = value; RaisePropertyChanged("TextSexoImputado"); }
        }

        private string textEdadImputado = string.Empty;
        public string TextEdadImputado
        {
            get { return textEdadImputado; }
            set { textEdadImputado = value; RaisePropertyChanged("TextEdadImputado"); }
        }

        private string textFechaNacImputado = string.Empty;
        public string TextFechaNacImputado
        {
            get { return textFechaNacImputado; }
            set { textFechaNacImputado = value; RaisePropertyChanged("TextFechaNacImputado"); }
        }

        private byte[] imagenIngreso = null;
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; RaisePropertyChanged("ImagenIngreso"); }
        }
        #endregion

        #region Atenciones Pendientes
        private List<cPendientesTVAreaMedica> lstPendientesTV;
        public List<cPendientesTVAreaMedica> LstPendientesTV
        {
            get { return lstPendientesTV; }
            set { lstPendientesTV = value; RaisePropertyChanged("LstPendientesTV"); }
        }

        private cPendientesTVAreaMedica selectedPendienteTV = null;
        public cPendientesTVAreaMedica SelectedPendienteTV
        {
            get { return selectedPendienteTV; }
            set { selectedPendienteTV = value; RaisePropertyChanged("SelectedPendienteTV"); }
        }
        #endregion

        #region Reagendar cita medica
        private ObservableCollection<cUsuarioExtendida> lstCitaMedicaAgendaEmpleados = null;
        public ObservableCollection<cUsuarioExtendida> LstCitaMedicaAgendaEmpleados
        {
            get { return lstCitaMedicaAgendaEmpleados; }
            set { lstCitaMedicaAgendaEmpleados = value; RaisePropertyChanged("LstCitaMedicaAgendaEmpleados"); }
        }

        private int selectedCitaAgendaEmpleadoValue;
        public int SelectedCitaAgendaEmpleadoValue
        {
            get { return selectedCitaAgendaEmpleadoValue; }
            set { selectedCitaAgendaEmpleadoValue = value; RaisePropertyChanged("SelectedCitaAgendaEmpleadoValue"); }
        }

        private ObservableCollection<AREA> lstCitaMedicaAreas = null;
        public ObservableCollection<AREA> LstCitaMedicaAreas
        {
            get { return lstCitaMedicaAreas; }
            set { lstCitaMedicaAreas = value; RaisePropertyChanged("LstCitaMedicaAreas"); }
        }

        private short selectedCitaMedicaArea = -1;
        public short SelectedCitaMedicaArea
        { 
            get { return selectedCitaMedicaArea; }
            set { selectedCitaMedicaArea = value; RaisePropertyChanged("SelectedCitaMedicaArea"); }
        }

        private bool agregarCitaMedicaAgendaFechaValid = false;
        public bool AgregarCitaMedicaAgendaFechaValid
        {
            get { return agregarCitaMedicaAgendaFechaValid; }
            set { agregarCitaMedicaAgendaFechaValid = value; RaisePropertyChanged("AgregarCitaMedicaAgendaFechaValid"); }
        }

        private DateTime? agregarCitaMedicaAgendaFecha;
        public DateTime? AgregarCitaMedicaAgendaFecha
        {
            get { return agregarCitaMedicaAgendaFecha; }
            set { agregarCitaMedicaAgendaFecha = value; RaisePropertyChanged("AgregarCitaMedicaAgendaFecha"); }
        }

        private DateTime? agregarCitaMedicaAgendaHoraI;
        public DateTime? AgregarCitaMedicaAgendaHoraI
        {
            get { return agregarCitaMedicaAgendaHoraI; }
            set
            {
                agregarCitaMedicaAgendaHoraI = value;
                if (value.HasValue)
                {
                    if (AgregarCitaMedicaAgendaHoraF.HasValue)
                    {
                        if (value <= AgregarCitaMedicaAgendaHoraF.Value)
                            AgregarCitaMedicaAgendaHorasValid = true;
                        else
                            AgregarCitaMedicaAgendaHorasValid = false;
                    }
                }
                OnPropertyChanged("AgregarCitaMedicaAgendaHoraI");
            }
        }

        private DateTime? agregarCitaMedicaAgendaHoraF;
        public DateTime? AgregarCitaMedicaAgendaHoraF
        {
            get { return agregarCitaMedicaAgendaHoraF; }
            set
            {
                agregarCitaMedicaAgendaHoraF = value;
                if (!value.HasValue)
                    AgregarCitaMedicaAgendaHorasValid = false;
                else
                    if (!AgregarCitaMedicaAgendaHoraI.HasValue)
                        AgregarCitaMedicaAgendaHorasValid = false;
                    else
                        if (value <= AgregarCitaMedicaAgendaHoraI)
                            AgregarCitaMedicaAgendaHorasValid = false;
                        else
                            AgregarCitaMedicaAgendaHorasValid = true;

                OnPropertyChanged("AgregarAgendaHoraF");
            }
        }

        private bool agregarCitaMedicaAgendaHorasValid;
        public bool AgregarCitaMedicaAgendaHorasValid
        {
            get { return agregarCitaMedicaAgendaHorasValid; }
            set { agregarCitaMedicaAgendaHorasValid = value; RaisePropertyChanged("AgregarCitaMedicaAgendaHorasValid"); }
        }
        #endregion


        #region Recrear Interconsulta
        private ObservableCollection<INTERCONSULTA_TIPO> lst_TV_Inter_Tipo=null;
        public ObservableCollection<INTERCONSULTA_TIPO> Lst_TV_Inter_Tipo
        {
            get { return lst_TV_Inter_Tipo; }
            set { lst_TV_Inter_Tipo = value; RaisePropertyChanged("Lst_TV_Inter_Tipo"); }
        }

        private short selectedTV_Inter_TipoValue = -1;
        public short SelectedTV_Inter_TipoValue
        {
            get { return selectedTV_Inter_TipoValue; }
            set { selectedTV_Inter_TipoValue = value; RaisePropertyChanged("SelectedTV_Inter_TipoValue"); }
        }

        private ObservableCollection<INTERCONSULTA_NIVEL_PRIORIDAD> lst_TV_Inter_Prioridad=null;
        public ObservableCollection<INTERCONSULTA_NIVEL_PRIORIDAD> Lst_TV_Inter_Prioridad
        {
            get { return lst_TV_Inter_Prioridad; }
            set { lst_TV_Inter_Prioridad = value; RaisePropertyChanged("Lst_TV_Inter_Prioridad"); }
        }

        private short selectedTV_Inter_PrioridadValue = -1;
        public short SelectedTV_Inter_PrioridadValue
        {
            get { return selectedTV_Inter_PrioridadValue; }
            set { selectedTV_Inter_PrioridadValue=value; RaisePropertyChanged("SelectedTV_Inter_PrioridadValue"); }
        }

        private ObservableCollection<CENTRO> lstTV_Inter_Centro = null;
        public ObservableCollection<CENTRO> LstTV_Inter_Centro
        {
            get { return lstTV_Inter_Centro; }
            set { lstTV_Inter_Centro = value; RaisePropertyChanged("LstTV_Inter_Centro"); }
        }

        private short selectedTV_Inter_CentroValue = -1;
        public short SelectedTV_Inter_CentroValue
        {
            get { return selectedTV_Inter_CentroValue; }
            set { selectedTV_Inter_CentroValue = value; RaisePropertyChanged("SelectedTV_Inter_CentroValue"); }
        }

        private ObservableCollection<HOSPITAL> lstTV_Inter_Hospitales = null;
        public ObservableCollection<HOSPITAL> LstTV_Inter_Hospitales
        {
            get { return lstTV_Inter_Hospitales; }
            set { lstTV_Inter_Hospitales = value; RaisePropertyChanged("LstTV_Inter_Hospitales"); }
        }

        private short selectedTV_Inter_HospitalValue = -1;
        public short SelectedTV_Inter_HospitalValue
        {
            get { return selectedTV_Inter_HospitalValue; }
            set { selectedTV_Inter_HospitalValue=value; RaisePropertyChanged("SelectedTV_Inter_HospitalValue"); }
        }

        private string tv_Inter_Otro_Hospital = string.Empty;
        public string TV_Inter_Otro_Hospital
        {
            get { return tv_Inter_Otro_Hospital; }
            set { tv_Inter_Otro_Hospital = value; RaisePropertyChanged("TV_Inter_Otro_Hospital"); }
        }

        private string tv_Inter_ExpHGT = string.Empty;
        public string TV_Inter_ExpHGT
        {
            get { return tv_Inter_ExpHGT; }
            set { tv_Inter_ExpHGT = value; RaisePropertyChanged("TV_Inter_ExpHGT"); }
        }

        private ObservableCollection<CITA_TIPO> lstTV_Inter_Cita_Tipo = null;
        public ObservableCollection<CITA_TIPO> LstTV_Inter_Cita_Tipo
        {
            get { return lstTV_Inter_Cita_Tipo; }
            set { lstTV_Inter_Cita_Tipo = value; RaisePropertyChanged("LstTV_Inter_Cita_Tipo"); }
        }

        private short selectedTV_Inter_Cita_TipoValue = -1;
        public short SelectedTV_Inter_Cita_TipoValue
        {
            get { return selectedTV_Inter_Cita_TipoValue; }
            set { selectedTV_Inter_Cita_TipoValue = value; RaisePropertyChanged("SelectedTV_Inter_Cita_TipoValue"); }
        }

        private bool isFechaCitaValid = false;
        public bool IsFechaCitaValid
        {
            get { return isFechaCitaValid; }
            set { isFechaCitaValid = value; RaisePropertyChanged("IsFechaCitaValid"); }
        }

        private DateTime? tv_Inter_FechaCita = null;
        public DateTime? TV_Inter_FechaCita
        {
            get { return tv_Inter_FechaCita; }
            set { tv_Inter_FechaCita = value; RaisePropertyChanged("TV_Inter_FechaCita"); }
        }

        private string tv_Inter_HR_Motivo = string.Empty;
        public string TV_Inter_HR_Motivo
        {
            get { return tv_Inter_HR_Motivo; }
            set { tv_Inter_HR_Motivo = value; RaisePropertyChanged("TV_Inter_HR_Motivo"); }
        }

        private string tv_Inter_HR_Observacion = string.Empty;
        public string TV_Inter_HR_Observacion
        {
            get { return tv_Inter_HR_Observacion; }
            set { tv_Inter_HR_Observacion = value; RaisePropertyChanged("TV_Inter_HR_Observacion"); }
        }

        private string tv_Inter_Motivo = string.Empty;
        public string TV_Inter_Motivo
        {
            get { return tv_Inter_Motivo; }
            set { tv_Inter_Motivo = value; RaisePropertyChanged("TV_Inter_Motivo"); }
        }
        #region Variables para Visibilidad
        private Visibility tv_Inter_Interna_Visible = Visibility.Collapsed;
        public Visibility TV_Inter_Interna_Visible
        {
            get { return tv_Inter_Interna_Visible; }
            set { tv_Inter_Interna_Visible = value; RaisePropertyChanged("TV_Inter_Interna_Visible"); }
        }

        private Visibility tv_Inter_Externa_Visible = Visibility.Visible;
        public Visibility TV_Inter_Externa_Visible
        {
            get { return tv_Inter_Externa_Visible; }
            set { tv_Inter_Externa_Visible = value; RaisePropertyChanged("TV_Inter_Externa_Visible"); }
        }

        private Visibility isOtroHospitalSelected = Visibility.Collapsed;
        public Visibility IsOtroHospitalSelected
        {
            get { return isOtroHospitalSelected; }
            set { isOtroHospitalSelected = value; RaisePropertyChanged("IsOtroHospitalSelected"); }
        }

        private DateTime fechaMinima = Fechas.GetFechaDateServer;
        public DateTime FechaMinima
        {
            get { return fechaMinima; }
            set { fechaMinima = value; RaisePropertyChanged("FechaMinima"); }
        }



        #endregion
        #endregion
    }
}

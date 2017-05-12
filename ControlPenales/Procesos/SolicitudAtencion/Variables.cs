using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class SolicitudAtencionViewModel : ValidationViewModelBase
    {

        #region Variables para Visualizar Controles
        private Visibility isTipoAtencionVisible=Visibility.Collapsed;
        public Visibility IsTipoAtencionVisible
        {
            get { return isTipoAtencionVisible; }
            set { isTipoAtencionVisible = value; OnPropertyValidateChanged("IsTipoAtencionVisible"); }
        }
        #endregion

        #region Solicitud Atencion

        private ATENCION_SOLICITUD selectedAtencionSolicitud;
        public ATENCION_SOLICITUD SelectedAtencionSolicitud
        {
            get { return selectedAtencionSolicitud; }
            set { selectedAtencionSolicitud = value; OnPropertyChanged("SelectedAtencionSolicitud"); }
        }
 
        private ObservableCollection<AREA> lstArea;
        public ObservableCollection<AREA> LstArea
        {
            get { return lstArea; }
            set { lstArea = value; OnPropertyChanged("LstArea"); }
        }

        private ObservableCollection<AREA_TECNICA> lstAreaTecnica;
        public ObservableCollection<AREA_TECNICA> LstAreaTecnica
        {
            get { return lstAreaTecnica; }
            set { lstAreaTecnica = value; OnPropertyChanged("LstAreaTecnica"); }
        }

        private ObservableCollection<ATENCION_TIPO> lstAtencionTipos;
        public ObservableCollection<ATENCION_TIPO> LstAtencionTipos
        {
            get { return lstAtencionTipos; }
            set { lstAtencionTipos = value; OnPropertyChanged("LstAtencionTipos"); }
        }

        private ObservableCollection<INGRESO> lstInternos;
        public ObservableCollection<INGRESO> LstInternos
        {
            get { return lstInternos; }
            set { lstInternos = value; OnPropertyValidateChanged("LstInternos"); }
        }

        private Visibility emptyInternos = Visibility.Visible;
        public Visibility EmptyInternos
        {
            get { return emptyInternos; }
            set { emptyInternos = value; OnPropertyChanged("EmptyInternos"); }
        }

        private short? sAreaTecnica = -1;
        public short? SAreaTecnica
        {
            get { return sAreaTecnica; }
            set { sAreaTecnica = value; OnPropertyValidateChanged("SAreaTecnica"); }
        }
        
        private short? sArea =-1;
        public short? SArea
        {
            get { return sArea; }
            set { sArea = value; OnPropertyValidateChanged("SArea"); }
        }

        private bool sFecValid;
        public bool SFecValid
        {
            get { return sFecValid; }
            set { sFecValid = value; OnPropertyChanged("SFecValid"); }
        }

        private string eMensajeErrorFecha = "LA FECHA ES REQUERIDA!";
        public string EMensajeErrorFecha
        {
            get { return eMensajeErrorFecha; }
            set { eMensajeErrorFecha = value; OnPropertyChanged("EMensajeErrorFecha"); }
        }

        //private DateTime? sFecha = null;
        //public DateTime? SFecha
        //{
        //    get { return sFecha; }
        //    set { sFecha = value;
        //    if (value.HasValue)
        //    {
        //            DateTime hoy = Fechas.GetFechaDateServer;
        //            if (value.Value.Date < hoy.Date.Date)
        //            {
        //                EMensajeErrorFecha = "LA FECHA DEBE SER MAYOR O IGUAL AL DIA DE HOY!";
        //                SFecValid = false;
        //            }
        //            else
        //            {
        //                SFecValid = true;
        //            }
        //    }
        //    else
        //    {
        //        EMensajeErrorFecha = "LA FECHA ES REQUERIDA!";
        //        SFecValid = false;
        //    }
        //        OnPropertyValidateChanged("SFecha"); }
        //}

        private string sActividad;
        public string SActividad
        {
            get { return sActividad; }
            set { sActividad = value; OnPropertyValidateChanged("SActividad"); }
        }
      
        private DateTime? sHora = null;
        public DateTime? SHora
        {
            get { return sHora; }
            set { sHora = value; OnPropertyValidateChanged("SHora"); }
        }

        private string sAutorizacion;
        public string SAutorizacion
        {
            get { return sAutorizacion; }
            set { sAutorizacion = value; OnPropertyValidateChanged("SAutorizacion"); }
        }

        private string sOficialTraslada;
        public string SOficialTraslada
        {
            get { return sOficialTraslada; }
            set { sOficialTraslada = value; OnPropertyValidateChanged("SOficialTraslada"); }
        }

        private short? selectedAtencion_Tipo=-1;
        public short? SelectedAtencion_Tipo
        {
            get { return selectedAtencion_Tipo; }
            set { selectedAtencion_Tipo = value; OnPropertyValidateChanged("SelectedAtencion_Tipo"); }
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
                if (value)
                    MenuGuardarEnabled = value;
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
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }

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
        #endregion

        #region Buscar Interno
        private RangeEnabledObservableCollection<INGRESO> lstIngreso;
        public RangeEnabledObservableCollection<INGRESO> LstIngreso
        {
            get { return lstIngreso; }
            set { lstIngreso = value; OnPropertyChanged("LstIngreso"); }
        }

        private int Pagina { get; set; }
        private bool SeguirCargandoIngresos { get; set; }
        private bool SeguirCargando { get; set; }

    
        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value;
            if (value != null)
            {
                if (value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenIngresoPop = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenIngresoPop = new Imagenes().getImagenPerson();
                //Ubicacion
                if (value.ID_UB_CAMA != null)
                    if (value.ID_UB_CAMA > 0)
                    {
                        UbicacionInterno = string.Format("{0}-{1}-{2}-{3}", value.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                                       value.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                                       value.CAMA.CELDA.ID_CELDA.Trim(),
                                                       value.ID_UB_CAMA);
                    }
                    else
                        UbicacionInterno = string.Empty;

            }
            else
            {
                ImagenIngresoPop = new Imagenes().getImagenPerson();
                UbicacionInterno = string.Empty;
            }
                OnPropertyChanged("SelectedIngreso"); }
        }


        #region Busqueda e Imagenes de Imputado
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

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }


        #endregion

        private short? anioI;
        public short? AnioI
        {
            get { return anioI; }
            set { anioI = value; OnPropertyChanged("AnioI"); }
        }
        
        private int? folioI;
        public int? FolioI
        {
            get { return folioI; }
            set { folioI = value; OnPropertyChanged("FolioI"); }
        }

        private string paternoI;
        public string PaternoI
        {
            get { return paternoI; }
            set { paternoI = value; OnPropertyChanged("PaternoI"); }
        }

        private string maternoI;
        public string MaternoI
        {
            get { return maternoI; }
            set { maternoI = value; OnPropertyChanged("MaternoI"); }
        }

        private string nombreI;
        public string NombreI
        {
            get { return nombreI; }
            set { nombreI = value; OnPropertyChanged("NombreI"); }
        }

        private byte[] imagenIngresoPop = new Imagenes().getImagenPerson();
        public byte[] ImagenIngresoPop
        {
            get { return imagenIngresoPop; }
            set { imagenIngresoPop = value; OnPropertyChanged("ImagenIngresoPop"); }
        }

        private bool internosEmpty = true;
        public bool InternosEmpty
        {
            get { return internosEmpty; }
            set { internosEmpty = value; OnPropertyChanged("InternosEmpty"); }
        }

        private string ubicacionInterno;
        public string UbicacionInterno
        {
            get { return ubicacionInterno; }
            set { ubicacionInterno = value; OnPropertyChanged("UbicacionInterno"); }
        }
        #endregion

        private SOCIOECONOMICO _Estudio;
        public SOCIOECONOMICO Estudio
        {
            get { return _Estudio; }
            set { _Estudio = value; OnPropertyChanged("Estudio"); }
        }

        private IMPUTADO selectedInterno;
        public IMPUTADO SelectedInterno
        {
            get { return selectedInterno; }
            set { selectedInterno = value; OnPropertyChanged("SelectedInterno"); }
        }

        private INGRESO selectIngresoLista;
        public INGRESO SelectIngresoLista
        {
            get { return selectIngresoLista; }
            set { selectIngresoLista = value; OnPropertyChanged("SelectIngresoLista"); }
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
            set
            {
                folioD = value;
                OnPropertyChanged("FolioD");
            }
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
        
        private int? ingresosD;
        public int? IngresosD
        {
            get { return ingresosD; }
            set { ingresosD = value; OnPropertyChanged("IngresosD"); }
        }
        
        private string noControlD;
        public string NoControlD
        {
            get { return noControlD; }
            set { noControlD = value; OnPropertyChanged("NoControlD"); }
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

        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set { listExpediente = value; OnPropertyChanged("ListExpediente"); }
        }

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }
        
        private bool emptyIngresoVisible;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private int indexMenu;
        public int IndexMenu
        {
            get { return indexMenu; }
            set { indexMenu = value; OnPropertyChanged("IndexMenu"); }
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
                OnPropertyChanged("SelectIngreso");
                if (selectIngreso == null)
                {
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
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
            }
        }

        private string tituloModal;
        public string TituloModal
        {
            get { return tituloModal; }
            set { tituloModal = value; OnPropertyChanged("TituloModal"); }
        }

        private short?[] estatus_inactivos=null;

        //#region Validaciones
        //private short VCitasMes = Parametro.SOLICITUD_ATENCION_POR_MES;
        //#endregion
    }
}

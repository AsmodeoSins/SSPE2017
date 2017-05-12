using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Almacenes;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using MoreLinq;
using SSP.Controlador.Catalogo.Justicia.Medico.CertificadoMedico;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace ControlPenales
{
    partial class ReporteNotasMedicasViewModel
    {
        #region Datos Expediente
        private short? anioD;
        public short? AnioD
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
        private int pagina;
        public int Pagina
        {
            get { return pagina; }
            set { pagina = value; OnPropertyChanged("Pagina"); }
        }
        private bool seguirCargando;
        public bool SeguirCargando
        {
            get { return seguirCargando; }
            set { seguirCargando = value; OnPropertyChanged("SeguirCargando"); }
        }
        #endregion

        #region Buscar
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
        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set { listExpediente = value; OnPropertyChanged("ListExpediente"); }
        }
        private bool emptyExpedienteVisible = false;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }
        private bool emptyIngresoVisible = false;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
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
                    {
                        EmptyIngresoVisible = true;
                        SelectIngreso = null;
                        ImagenIngreso = new Imagenes().getImagenPerson();
                    }

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
                    ImagenImputado = new Imagenes().getImagenPerson();
                    ImagenIngreso = new Imagenes().getImagenPerson();
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
        private INGRESO SelectedIngresoAuxiliar;
        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value; OnPropertyChanged("SelectedIngreso"); }
        }
        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }
        private string textBotonSeleccionarIngreso = "Seleccionar Ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; }
        }
        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; OnPropertyChanged("ImagenIngreso"); }
        }
        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set { imagenImputado = value; OnPropertyChanged("ImagenImputado"); }
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
            }
        }
        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set
            {
                pEditar = value;
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
                    MenuReporteEnabled = false;
            }
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

        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        #region Lesiones
        private ATENCION_MEDICA SelectAtencionMedica;
        private List<LesionesCustom> ListLesiones;
        #endregion

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }
        private Visibility _ReporteVisible = Visibility.Collapsed;
        public Visibility ReporteVisible
        {
            get { return _ReporteVisible; }
            set { _ReporteVisible = value; OnPropertyChanged("ReporteVisible"); }
        }

        #region Reportes Generales
        private bool _FiltrarEnabled;
        public bool FiltrarEnabled
        {
            get { return _FiltrarEnabled; }
            set { _FiltrarEnabled = value; OnPropertyChanged("FiltrarEnabled"); }
        }
        private Visibility _EmptyVisible;
        public Visibility EmptyVisible
        {
            get { return _EmptyVisible; }
            set { _EmptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }
        private ObservableCollection<ATENCION_TIPO> _ListTipoAtencion;
        public ObservableCollection<ATENCION_TIPO> ListTipoAtencion
        {
            get { return _ListTipoAtencion; }
            set { _ListTipoAtencion = value; OnPropertyChanged("ListTipoAtencion"); }
        }
        private ObservableCollection<ATENCION_SERVICIO> ListTipoServicioAux;
        private ObservableCollection<ATENCION_SERVICIO> _ListTipoServicio;
        public ObservableCollection<ATENCION_SERVICIO> ListTipoServicio
        {
            get { return _ListTipoServicio; }
            set { _ListTipoServicio = value; OnPropertyChanged("ListTipoServicio"); }
        }
        private ATENCION_TIPO _SelectTipoAtencion;
        public ATENCION_TIPO SelectTipoAtencion
        {
            get { return _SelectTipoAtencion; }
            set
            {
                _SelectTipoAtencion = value;
                if (value != null ? value.ID_TIPO_ATENCION != -1 : false)
                    ListTipoServicio = new ObservableCollection<ATENCION_SERVICIO>(ListTipoServicioAux.Where(w => w.ID_TIPO_ATENCION == value.ID_TIPO_ATENCION));
                else
                    ListTipoServicio = new ObservableCollection<ATENCION_SERVICIO>();
                ListTipoServicio.Insert(0, new ATENCION_SERVICIO()
                {
                    DESCR = "SELECCIONE",
                    ID_TIPO_ATENCION = -1,
                    ID_TIPO_SERVICIO = -1,
                });
                SelectTipoServicio = ListTipoServicio.First(f => f.ID_TIPO_ATENCION == -1 && f.ID_TIPO_SERVICIO == -1);
                OnPropertyChanged("SelectTipoAtencion");
            }
        }
        private List<cReporteProcsMeds> ListSubreporteProcedimientos;
        private List<cReporteInsumos> ListSubreporteInsumos;
        private ATENCION_SERVICIO _SelectTipoServicio;
        public ATENCION_SERVICIO SelectTipoServicio
        {
            get { return _SelectTipoServicio; }
            set { _SelectTipoServicio = value; OnPropertyChanged("SelectTipoServicio"); }
        }
        private ObservableCollection<ATENCION_MEDICA> _ListNotasMedicas;
        public ObservableCollection<ATENCION_MEDICA> ListNotasMedicas
        {
            get { return _ListNotasMedicas; }
            set { _ListNotasMedicas = value; OnPropertyChanged("ListNotasMedicas"); }
        }
        private List<cReporteOdontogramaSeguimiento> ListaOdontogramaSeguimiento;
        private ATENCION_MEDICA SelectNotaMedicaAuxiliar;
        private ATENCION_MEDICA _SelectNotaMedica;
        public ATENCION_MEDICA SelectNotaMedica
        {
            get { return _SelectNotaMedica; }
            set { _SelectNotaMedica = value; OnPropertyChanged("SelectNotaMedica"); }
        }
        #endregion
    }
}
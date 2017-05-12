using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ControlPenales.Clases;
using System.Windows.Media;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        private int _CantidadFichas { get; set; }

        private string _NoOficio;
        public string NoOficio
        {
            get { return _NoOficio; }
            set { _NoOficio = value; OnPropertyChanged("NoOficio"); }
        }

        private string _NombrePrograma;
        public string NombrePrograma
        {
            get { return _NombrePrograma; }
            set { _NombrePrograma = value; OnPropertyChanged("NombrePrograma"); }
        }

        #region Archivero
        public class Archivero
        {
            public string NombreArchivo { get; set; }
            public string Disponible { get; set; }
            public short TipoArchivo { get; set; }
            public bool VisibleVerDocumentoArchivero { get; set; }
        }

        private ObservableCollection<Archivero> _LstDocumentos;

        public ObservableCollection<Archivero> LstDocumentos
        {
            get { return _LstDocumentos; }
            set { _LstDocumentos = value; OnPropertyChanged("LstDocumentos"); }
        }

        private Archivero _SelectedDocumento;

        public Archivero SelectedDocumento
        {
            get { return _SelectedDocumento; }
            set
            {
                _SelectedDocumento = value;
                OnPropertyChanged("SelectedDocumento");
            }
        }

        private enum eSituacionActual
        {
            STAGE0 = 0,//APENAS SE ESTA GENERANDO LA LISTA, SE PUEDE CONSULTAR LA FICHA SIGNALETICA Y LA PARTIDA JURIDICA
            STAGE1 = 1,//YA SE GENERO LA FICHA JURIDICA Y EL OFICIO DE PETICION DE REALIZACION DE ESTUDIOS DE PERSONALIDAD
            STAGE2 = 2,//YA SE HICIERON LOS ESTUDIOS DE PERSONALIDAD, SE INCLUYE EL OFICIO QUE HACE AREAS TECNICAS HACIA JURIDICO
            STAGE3 = 3,//SE HACE EL ACTA, EL OFICIO DE REMISION DE LOS ESTUDIOS DE PERSONALIDAD CON SU DICTAMEN, EL DICTAMEN INDIVIDUAL
            STAGE4 = 4 //APARTADO ESPECIAL, CONSULTA TODOS LOS FORMATOS QUE E HALLAN GENERADO DURANTE EL PROCEDIMIENTO
        };

        /// <summary>
        /// CONDENSADO DE LOS ARCHIVOS USADOS DENTRO DEL PROCESO DE LOS ESTUDIOS DE PERSONALIDAD DE FUERO COMUN Y FUERO FEDERAL, PROCESO JURIDICO.
        /// </summary>
        private enum eDocumentoMostrado
        {
            PARTIDA_JURIDICA = 0,
            FICHA_SIGNALETICA = 1,
            FICHA_JURIDICA = 2,
            ACTA_CONSEJO_TECNICO = 3,
            DICTAMEN_INDIVIDUAL = 4,
            PERSONALIDAD_COMUN_MEDICO = 5,
            PERSONALIDAD_COMUN_PSIQ = 6,
            PERSONALIDAD_COMUN_PSICO = 7,
            PERSONALIDAD_COMUN_CRIMI = 8,
            PERSONALIDAD_COMUN_SOCIO_FAM = 9,
            PERSONALIDAD_COMUN_EDUC = 10,
            PERSONALIDAD_COMUN_CAPAC = 11,
            PERSONALIDAD_COMUN_SEGURIDAD = 12,
            ACTA_FEDERAL = 13,
            PERSONALIDAD_FEDERAL_MEDICO = 14,
            PERSONALIDAD_FEDERAL_PSICO = 15,
            PERSONALIDAD_FEDERAL_TRABAJO_SOCIAL = 16,
            PERSONALIDAD_FEDERAL_CAPAC = 17,
            PERSONALIDAD_FEDERAL_EDUCATIVAS = 18,
            PERSONALIDAD_FEDERAL_VIGILANCIA = 19,
            PERSONALIDAD_FEDERAL_CRIMINOLOGICO = 20,
            OFICIO_PETICION_REALIZACION_ESTUDIOS_PERSONALIDAD = 21,
            OFICIO_REMISION_GENERAL_DICTAMEN = 22,
            OFICIO_REMISION_APROBADO_APROBADO_MAYORIA = 23,
            TRASLADO_NACIONAL = 24,
            TRASLADO_INTERNACIONAL = 25,
            TRASLADO_ISLAS = 26,
            REMISION_CIERRE = 27,
            REMISION_DPMJ = 28
        };

        private enum eTipoSolicitudTraslado
        {//enumerador en base ala tabla PERSONALIDAD_MOTIVO
            ISLAS = 5,
            INTERNACIONAL = 4,
            NACIONAL = 3
        };

        public enum eDatosRolesProcesosPersonalidad
        {
            //ESTE ENUMERADOR ES PARA AUTO DOCUMENTAR PROCESOS DE ESTUDIOS DE PERSONALIDAD, EN BASE ALA TABLA DEPARTAMENTO_ACCESO
            COORDINACION_TECNICA = 1,
            JURIDICO = 2,
            COMANDANCIA = 4
        };

        private ObservableCollection<DelitosGrid> lstDelitosDos;

        public ObservableCollection<DelitosGrid> LstDelitosDos
        {
            get { return lstDelitosDos; }
            set { lstDelitosDos = value; OnPropertyChanged("LstDelitosDos"); }
        }

        public class DelitosGrid
        {
            public string Delito { get; set; }
            public string Proceso { get; set; }
            public string FComun { get; set; }
            public string FFederal { get; set; }
            public string Pena { get; set; }
            public string APartir { get; set; }
        };

        #endregion

        private PERSONALIDAD _selectedEstudioTerminado;
        public PERSONALIDAD SelectedEstudioTerminado
        {
            get { return _selectedEstudioTerminado; }
            set { _selectedEstudioTerminado = value; OnPropertyChanged("SelectedEstudioTerminado"); }
        }

        #region Visibles
        private Visibility _IdentificacionVisible = Visibility.Visible;
        public Visibility IdentificacionVisible
        {
            get { return _IdentificacionVisible; }
            set { _IdentificacionVisible = value; OnPropertyChanged("IdentificacionVisible"); }
        }
        
        private Visibility _JuridicoVisible = Visibility.Collapsed;
        public Visibility JuridicoVisible
        {
            get { return _JuridicoVisible; }
            set { _JuridicoVisible = value; OnPropertyChanged("JuridicoVisible"); }
        }
        
        private Visibility _AdministrativoVisible = Visibility.Collapsed;
        public Visibility AdministrativoVisible
        {
            get { return _AdministrativoVisible; }
            set { _AdministrativoVisible = value; OnPropertyChanged("AdministrativoVisible"); }
        }

        private Visibility _EstudiosVisible = Visibility.Collapsed;
        public Visibility EstudiosVisible
        {
            get { return _EstudiosVisible; }
            set { _EstudiosVisible = value; OnPropertyChanged("EstudiosVisible"); }
        }
       
        private Visibility _VisitasVisible = Visibility.Collapsed;
        public Visibility VisitasVisible
        {
            get { return _VisitasVisible; }
            set { _VisitasVisible = value; OnPropertyChanged("VisitasVisible"); }
        }
        
        private Visibility _AgendaVisible = Visibility.Collapsed;
        public Visibility AgendaVisible
        {
            get { return _AgendaVisible; }
            set { _AgendaVisible = value; OnPropertyChanged("AgendaVisible"); }
        }
       
        private Visibility _ExpedienteFisicoVisible = Visibility.Collapsed;
        public Visibility ExpedienteFisicoVisible
        {
            get { return _ExpedienteFisicoVisible; }
            set { _ExpedienteFisicoVisible = value; OnPropertyChanged("ExpedienteFisicoVisible"); }
        }

        private Visibility _VisibleDocumentosHistorico = Visibility.Collapsed;
        public Visibility VisibleDocumentosHistorico
        {
            get { return _VisibleDocumentosHistorico; }
            set { _VisibleDocumentosHistorico = value; OnPropertyChanged("VisibleDocumentosHistorico"); }
        }

        private Visibility visibleHojaDefuncion = Visibility.Collapsed;
        public Visibility VisibleHojaDefuncion
        {
            get { return visibleHojaDefuncion; }
            set { visibleHojaDefuncion = value; OnPropertyChanged("VisibleHojaDefuncion"); }
        }

        #region datos medicos
        private Visibility _VisibleDatosMedicos = Visibility.Collapsed;
        public Visibility VisibleDatosMedicos
        {
            get { return _VisibleDatosMedicos; }
            set { _VisibleDatosMedicos = value; OnPropertyChanged("VisibleDatosMedicos"); }
        }

        private Visibility _VisibleDatosKardex = Visibility.Collapsed;
        public Visibility VisibleDatosKardex
        {
            get { return _VisibleDatosKardex; }
            set { _VisibleDatosKardex = value; OnPropertyChanged("VisibleDatosKardex"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporteHCM;
        public Microsoft.Reporting.WinForms.ReportViewer ReporteHCM
        {
            get { return reporteHCM; }
            set { reporteHCM = value; OnPropertyChanged("ReporteHCM"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporteHCL;
        public Microsoft.Reporting.WinForms.ReportViewer ReporteHCL
        {
            get { return reporteHCL; }
            set { reporteHCL = value; OnPropertyChanged("ReporteHCL"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporteHD;
        public Microsoft.Reporting.WinForms.ReportViewer ReporteHD
        {
            get { return reporteHD; }
            set { reporteHD = value; OnPropertyChanged("ReporteHD"); }
        }

        private ObservableCollection<EXT_DELITOS> lstCausasPenales = null;
        public ObservableCollection<EXT_DELITOS> LstCausasPenales
        {
            get { return lstCausasPenales; }
            set { lstCausasPenales = value; RaisePropertyChanged("LstCausasPenales"); }
        }

        private ObservableCollection<NOTA_DEFUNCION> listaDecesoBusqueda = null;
        public ObservableCollection<NOTA_DEFUNCION> ListaDecesoBusqueda
        {
            get { return listaDecesoBusqueda; }
            set { listaDecesoBusqueda = value; RaisePropertyChanged("ListaInterconsultasBusqueda"); }
        }

        private ObservableCollection<CustomGridHistoricoDocumentos> lstHistoricoDocumentos;
        public ObservableCollection<CustomGridHistoricoDocumentos> LstHistoricoDocumentos
        {
            get { return lstHistoricoDocumentos; }
            set { lstHistoricoDocumentos = value; OnPropertyChanged("LstHistoricoDocumentos"); }
        }

        private CustomGridHistoricoDocumentos selectedHistoricoDocumentos;
        public CustomGridHistoricoDocumentos SelectedHistoricoDocumentos
        {
            get { return selectedHistoricoDocumentos; }
            set { selectedHistoricoDocumentos = value; OnPropertyChanged("SelectedHistoricoDocumentos"); }
        }

        //private ObservableCollection<TIPO_DOCUMENTO_HISTORICO> lstTiposDocumentosHistorico;
        //public ObservableCollection<TIPO_DOCUMENTO_HISTORICO> LstTiposDocumentosHistorico
        //{
        //    get { return lstTiposDocumentosHistorico; }
        //    set { lstTiposDocumentosHistorico = value; OnPropertyChanged("LstTiposDocumentosHistorico"); }
        //}

        private ObservableCollection<DEPARTAMENTO> lstDepartamentos;
        public ObservableCollection<DEPARTAMENTO> LstDepartamentos
        {
            get { return lstDepartamentos; }
            set { lstDepartamentos = value; OnPropertyChanged("LstDepartamentos"); }
        }

        private short? selectedDepartamento = -1;
        public short? SelectedDepartamento
        {
            get { return selectedDepartamento; }
            set { selectedDepartamento = value; OnPropertyChanged("SelectedDepartamento"); }
        }

        private short? selectedTipoDocumentoHistorico;
        public short? SelectedTipoDocumentoHistorico
        {
            get { return selectedTipoDocumentoHistorico; }
            set { selectedTipoDocumentoHistorico = value; OnPropertyChanged("SelectedTipoDocumentoHistorico"); }
        }


        private NOTA_DEFUNCION selectedDecesoBusqueda = null;
        public NOTA_DEFUNCION SelectedDecesoBusqueda
        {
            get { return selectedDecesoBusqueda; }
            set { selectedDecesoBusqueda = value; RaisePropertyChanged("SelectedDecesoBusqueda"); }
        }

        private ObservableCollection<ENFERMEDAD> _ListEnfermedades;
        public ObservableCollection<ENFERMEDAD> ListEnfermedades
        {
            get { return _ListEnfermedades; }
            set { _ListEnfermedades = value; OnPropertyChanged("ListEnfermedades"); }
        }



        private enum eTipoLiquidos
        {
            INGRESOS = 1,
            EGRESOS = 2
        };

        private enum eTipoConcentrados
        {
            MATUTINO = 1,
            VESPERTINO = 2,
            NOCTURNO = 3,
            TOTAL = 4
        };

        #region PARAMETROS
        private byte[] ParametroCuerpoDorso;
        private byte[] ParametroCuerpoFrente;
        private byte[] ParametroImagenZonaCorporal;
        #endregion

        private decimal SelectedHospitalizacion { get; set; }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_TURNO> _ListTurnosLiquidos;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_TURNO> ListTurnosLiquidos
        {
            get { return _ListTurnosLiquidos; }
            set { _ListTurnosLiquidos = value; OnPropertyChanged("ListTurnosLiquidos"); }
        }

        private System.DateTime? _FechaHojaenfermeria = Fechas.GetFechaDateServer;

        public System.DateTime? FechaHojaenfermeria
        {
            get { return _FechaHojaenfermeria; }
            set { _FechaHojaenfermeria = value; OnPropertyChanged("FechaHojaenfermeria"); }
        }

        private enum eTurnosLiqudos
        {
            MATUTUNO = 1,
            VESPERTINO = 2,
            NOCTURNO = 3
        };


        //ReportHE
        Microsoft.Reporting.WinForms.ReportViewer reporteHE;
        public Microsoft.Reporting.WinForms.ReportViewer ReporteHE
        {
            get { return reporteHE; }
            set { reporteHE = value; OnPropertyChanged("ReporteHE"); }
        }
        IList<EmpalmeParticipante> listaemp = new List<EmpalmeParticipante>();

        string _AvanceTratamiento = "0/0";
        public string AvanceTratamiento
        {
            get { return _AvanceTratamiento; }
            set
            {
                _AvanceTratamiento = value;
                OnPropertyChanged("AvanceTratamiento");
            }
        }
        string _HorasTratamiento = "0/0";
        public string HorasTratamiento
        {
            get { return _HorasTratamiento; }
            set
            {
                _HorasTratamiento = value;
                OnPropertyChanged("HorasTratamiento");
            }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporteKdx;
        public Microsoft.Reporting.WinForms.ReportViewer ReporteKdx
        {
            get { return reporteKdx; }
            set { reporteKdx = value; OnPropertyChanged("ReporteKdx"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporteHCD;
        public Microsoft.Reporting.WinForms.ReportViewer ReporteHCD
        {
            get { return reporteHCD; }
            set { reporteHCD = value; OnPropertyChanged("ReporteHCD"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporteBH;
        public Microsoft.Reporting.WinForms.ReportViewer ReporteBH
        {
            get { return reporteBH; }
            set { reporteBH = value; OnPropertyChanged("ReporteBH"); }
        }

        private List<EXT_REPORTE_BITACORA_HOSPITALIZACION_DETALLE> ds_detalle = null;
        private List<EXT_REPORTE_BITACORA_HOSPITALIZACION_ENCABEZADO> ds_encabezado = null;

        private string camasDesocupadas;
        public string CamasDesocupadas
        {
            get { return camasDesocupadas; }
            set { camasDesocupadas = value; OnPropertyChanged("CamasDesocupadas"); }
        }

        private string camasOcupadas;
        public string CamasOcupadas
        {
            get { return camasOcupadas; }
            set { camasOcupadas = value; OnPropertyChanged("CamasOcupadas"); }
        }

        const int SELECCIONE = -1;
        const string ACTIVA = "S";
        const string INACTIVA = "N";
        const string OCUPADA = "O";

        private DateTime selectedFechaHospitalizacion = Fechas.GetFechaDateServer;
        public DateTime SelectedFechaHospitalizacion
        {
            get { return selectedFechaHospitalizacion; }
            set { selectedFechaHospitalizacion = value; OnPropertyChanged("SelectedFechaHospitalizacion"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<CustomIngresos> lstCustomizadaIngresos;
        public System.Collections.ObjectModel.ObservableCollection<CustomIngresos> LstCustomizadaIngresos
        {
            get { return lstCustomizadaIngresos; }
            set { lstCustomizadaIngresos = value; OnPropertyChanged("LstCustomizadaIngresos"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT> lstTipoServAux;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT> LstTipoServAux
        {
            get { return lstTipoServAux; }
            set { lstTipoServAux = value; RaisePropertyChanged("LstTipoServAux"); }
        }

        private short selectedTipoServAux = -1;
        public short SelectedTipoServAux
        {
            get { return selectedTipoServAux; }
            set { selectedTipoServAux = value; OnPropertyValidateChanged("SelectedTipoServAux"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT> lstSubtipoServAux;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT> LstSubtipoServAux
        {
            get { return lstSubtipoServAux; }
            set { lstSubtipoServAux = value; RaisePropertyChanged("LstSubtipoServAux"); }
        }

        private short selectedSubtipoServAux = -1;
        public short SelectedSubtipoServAux
        {
            get { return selectedSubtipoServAux; }
            set { selectedSubtipoServAux = value; OnPropertyValidateChanged("SelectedSubtipoServAux"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> lstDiagnosticosPrincipal;

        public System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> LstDiagnosticosPrincipal
        {
            get { return lstDiagnosticosPrincipal; }
            set { lstDiagnosticosPrincipal = value; OnPropertyChanged("LstDiagnosticosPrincipal"); }
        }

        private int _SelectedDiagnPrincipal = -1;
        public int SelectedDiagnPrincipal
        {
            get { return _SelectedDiagnPrincipal; }
            set { _SelectedDiagnPrincipal = value; OnPropertyChanged("SelectedDiagnPrincipal"); }
        }

        private System.DateTime? _FechaInicioBusquedaResultServ = Fechas.GetFechaDateServer;

        public System.DateTime? FechaInicioBusquedaResultServ
        {
            get { return _FechaInicioBusquedaResultServ; }
            set
            {
                _FechaInicioBusquedaResultServ = value;
                if (value.HasValue)//avisa ala fecha de fin que ha cambiado
                {
                    FechaFinBusquedaResultServ = value;
                    OnPropertyChanged("FechaFinBusquedaResultServ");
                }

                OnPropertyChanged("FechaInicioBusquedaResultServ");
            }
        }

        private short _SelectedTipoServAuxEdicion = -1;

        public short SelectedTipoServAuxEdicion
        {
            get { return _SelectedTipoServAuxEdicion; }
            set { _SelectedTipoServAuxEdicion = value; OnPropertyChanged("SelectedTipoServAuxEdicion"); }
        }

        private short _SelectedSubTipoServAuxEdicion = -1;

        public short SelectedSubTipoServAuxEdicion
        {
            get { return _SelectedSubTipoServAuxEdicion; }
            set { _SelectedSubTipoServAuxEdicion = value; OnPropertyChanged("SelectedSubTipoServAuxEdicion"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT> _LstSubTipoServAuxEdicion;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT> LstSubTipoServAuxEdicion
        {
            get { return _LstSubTipoServAuxEdicion; }
            set { _LstSubTipoServAuxEdicion = value; OnPropertyChanged("LstSubTipoServAuxEdicion"); }
        }

        private int _SelectedDiagnosticoEdicion = -1;

        public int SelectedDiagnosticoEdicion
        {
            get { return _SelectedDiagnosticoEdicion; }
            set { _SelectedDiagnosticoEdicion = value; OnPropertyChanged("SelectedDiagnosticoEdicion"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> lstServAux;
        public System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> LstServAux
        {
            get { return lstServAux; }
            set { lstServAux = value; RaisePropertyChanged("LstServAux"); }
        }

                private TXTextControl.WPF.TextControl editor;
                public TXTextControl.WPF.TextControl Editor
                {
                    get { return editor; }
                    set { editor = value; OnPropertyChanged("Editor"); }
                }

        private System.DateTime? _FechaFinBusquedaResultServ = Fechas.GetFechaDateServer;
        public System.DateTime? FechaFinBusquedaResultServ
        {
            get { return _FechaFinBusquedaResultServ; }
            set { _FechaFinBusquedaResultServ = value; OnPropertyChanged("FechaFinBusquedaResultServ"); }
        }

        private short? _SelectedIngresoBusquedas;
        public short? SelectedIngresoBusquedas
        {
            get { return _SelectedIngresoBusquedas; }
            set { _SelectedIngresoBusquedas = value; OnPropertyChanged("SelectedIngresoBusquedas"); }
        }

        private bool _EmptyResultados = true;

        public bool EmptyResultados
        {
            get { return _EmptyResultados; }
            set { _EmptyResultados = value; OnPropertyChanged("EmptyResultados"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<CustomGridSinBytes> lstCustomizadaSinArchivos;//Lista que solo carga los datos SIN LOS BYTES (para ahorrar memoria)

        public System.Collections.ObjectModel.ObservableCollection<CustomGridSinBytes> LstCustomizadaSinArchivos
        {
            get { return lstCustomizadaSinArchivos; }
            set { lstCustomizadaSinArchivos = value; OnPropertyChanged("LstCustomizadaSinArchivos"); }
        }

        private CustomGridSinBytes _SeletedResultadoSinArchivo;
        public CustomGridSinBytes SeletedResultadoSinArchivo
        {
            get { return _SeletedResultadoSinArchivo; }
            set { _SeletedResultadoSinArchivo = value; OnPropertyChanged("SeletedResultadoSinArchivo"); }
        }

        public class CustomGridHistoricoDocumentos
        {
            public long IdHistorico { get; set;}
            public string Descripcion { get; set; }
            public System.DateTime? Fecha { get; set; }
            public string Departamento { get; set; }
            public string Formato { get; set; }
            public string TipoD { get; set; }
            public string Disponible { get; set; }
        }

        public class CustomIngresos
        {
            public string DescripcionIngreso { get; set; }
            public short IdIngreso { get; set; }
        }

        Microsoft.Reporting.WinForms.ReportViewer reportCM;
        public Microsoft.Reporting.WinForms.ReportViewer ReportCM
        {
            get { return reportCM; }
            set { reportCM = value; OnPropertyChanged("ReportCM"); }
        }

        private ObservableCollection<ATENCION_MEDICA> _ListNotasMedicas;
        public ObservableCollection<ATENCION_MEDICA> ListNotasMedicas
        {
            get { return _ListNotasMedicas; }
            set { _ListNotasMedicas = value; OnPropertyChanged("ListNotasMedicas"); }
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

        private bool _FiltrarEnabled;
        public bool FiltrarEnabled
        {
            get { return _FiltrarEnabled; }
            set { _FiltrarEnabled = value; OnPropertyChanged("FiltrarEnabled"); }
        }


        private ObservableCollection<ATENCION_SERVICIO> _ListTipoServicio;
        public ObservableCollection<ATENCION_SERVICIO> ListTipoServicio
        {
            get { return _ListTipoServicio; }
            set { _ListTipoServicio = value; OnPropertyChanged("ListTipoServicio"); }
        }

        private ATENCION_SERVICIO _SelectTipoServicio;
        public ATENCION_SERVICIO SelectTipoServicio
        {
            get { return _SelectTipoServicio; }
            set { _SelectTipoServicio = value; OnPropertyChanged("SelectTipoServicio"); }
        }

        private ATENCION_MEDICA SelectNotaMedicaAuxiliar;
        private ATENCION_MEDICA _SelectNotaMedica;
        public ATENCION_MEDICA SelectNotaMedica
        {
            get { return _SelectNotaMedica; }
            set { _SelectNotaMedica = value; OnPropertyChanged("SelectNotaMedica"); }
        }

        #region Lesiones
        private ATENCION_MEDICA SelectAtencionMedica;
        private List<LesionesCustom> ListLesiones;
        #endregion
        private List<cReporteOdontogramaSeguimiento> ListaOdontogramaSeguimiento;
        private List<cReporteProcsMeds> ListSubreporteProcedimientos;
        private List<cReporteInsumos> ListSubreporteInsumos;


        #endregion
        #endregion

        #region Enableds
        //private bool MenuGuardarEnabled = false;
        private bool _ReadOnlyCausaPenalSentencia = true;
        public bool ReadOnlyCausaPenalSentencia
        {
            get { return _ReadOnlyCausaPenalSentencia; }
            set { _ReadOnlyCausaPenalSentencia = value; OnPropertyChanged("ReadOnlyCausaPenalSentencia"); }
        }
        private bool _CausasPenalesIngresoEnabled;
        public bool CausasPenalesIngresoEnabled
        {
            get { return _CausasPenalesIngresoEnabled; }
            set { _CausasPenalesIngresoEnabled = value; OnPropertyChanged("CausasPenalesIngresoEnabled"); }
        }
        private bool _IdentificacionFotosHuellasEnabled;
        public bool IdentificacionFotosHuellasEnabled
        {
            get { return _IdentificacionFotosHuellasEnabled; }
            set { _IdentificacionFotosHuellasEnabled = value; OnPropertyChanged("IdentificacionFotosHuellasEnabled"); }
        }
        private bool _IdentificacionSeniasEnabled;
        public bool IdentificacionSeniasEnabled
        {
            get { return _IdentificacionSeniasEnabled; }
            set { _IdentificacionSeniasEnabled = value; OnPropertyChanged("IdentificacionSeniasEnabled"); }
        }
        private bool _IdentificacionPandillasEnabled;
        public bool IdentificacionPandillasEnabled
        {
            get { return _IdentificacionPandillasEnabled; }
            set { _IdentificacionPandillasEnabled = value; OnPropertyChanged("IdentificacionPandillasEnabled"); }
        }
        private bool _IdentificacionMediaFiliacionEnabled;
        public bool IdentificacionMediaFiliacionEnabled
        {
            get { return _IdentificacionMediaFiliacionEnabled; }
            set { _IdentificacionMediaFiliacionEnabled = value; OnPropertyChanged("IdentificacionMediaFiliacionEnabled"); }
        }
        private bool _IdentificacionApodosAliasEnabled;
        public bool IdentificacionApodosAliasEnabled
        {
            get { return _IdentificacionApodosAliasEnabled; }
            set { _IdentificacionApodosAliasEnabled = value; OnPropertyChanged("IdentificacionApodosAliasEnabled"); }
        }
        private bool _IdentificacionDatosGeneralesEnabled;
        public bool IdentificacionDatosGeneralesEnabled
        {
            get { return _IdentificacionDatosGeneralesEnabled; }
            set { _IdentificacionDatosGeneralesEnabled = value; OnPropertyChanged("IdentificacionDatosGeneralesEnabled"); }
        }
        private bool _MenuIzquierdaEnabled = true;
        public bool MenuIzquierdaEnabled
        {
            get { return _MenuIzquierdaEnabled; }
            set { _MenuIzquierdaEnabled = value; OnPropertyChanged("MenuIzquierdaEnabled"); }
        }
        #endregion

        #region [Huellas]
        private WebCam CamaraWeb;
        private WebCam _WebCam;
        WebCam WebCam
        {
            get { return _WebCam; }
            set { _WebCam = value; }
        }
        //private List<Image> _Frames;
        //public List<Image> Frames
        //{
        //    get { return _Frames; }
        //    set { _Frames = value; }
        //}
        private List<ImageSourceToSave> _ImagesToSave;
        public List<ImageSourceToSave> ImagesToSave
        {
            get { return _ImagesToSave; }
            set { _ImagesToSave = value; }
        }
        private bool _Processing = false;
        public bool Processing
        {
            get { return _Processing; }
            set
            {
                _Processing = value;
                OnPropertyChanged("Processing");
            }
        }
        IList<PlantillaBiometrico> HuellasCapturadas;

        private enumTipoBiometrico? _DD_Dedo;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set { _DD_Dedo = value; }
        }

        private Visibility _ShowPopUp = Visibility.Hidden;
        public Visibility ShowPopUp
        {
            get { return _ShowPopUp; }
            set
            {
                _ShowPopUp = value;
                OnPropertyChanged("ShowPopUp");
            }
        }

        private Visibility _ShowFingerPrint = Visibility.Hidden;
        public Visibility ShowFingerPrint
        {
            get { return _ShowFingerPrint; }
            set
            {
                _ShowFingerPrint = value;
                OnPropertyChanged("ShowFingerPrint");
            }
        }

        private Visibility _ShowLine = Visibility.Visible;
        public Visibility ShowLine
        {
            get { return _ShowLine; }
            set
            {
                _ShowLine = value;
                OnPropertyChanged("ShowLine");
            }
        }

        private Visibility _ShowOk = Visibility.Hidden;
        public Visibility ShowOk
        {
            get { return _ShowOk; }
            set
            {
                _ShowOk = value;
                OnPropertyChanged("ShowOk");
            }
        }

        private ImageSource _GuardaHuella;
        public ImageSource GuardaHuella
        {
            get { return _GuardaHuella; }
            set
            {
                _GuardaHuella = value;
                OnPropertyChanged("GuardaHuella");
            }
        }

        private Brush _PulgarDerecho;
        public Brush PulgarDerecho
        {
            get { return _PulgarDerecho; }
            set
            {
                _PulgarDerecho = value;
                RaisePropertyChanged("PulgarDerecho");
            }
        }

        private ImageSource _PulgarDerechoBMP;
        public ImageSource PulgarDerechoBMP
        {
            get { return _PulgarDerechoBMP; }
            set
            {
                _PulgarDerechoBMP = value;
                RaisePropertyChanged("PulgarDerechoBMP");
            }
        }

        private Brush _IndiceDerecho;
        public Brush IndiceDerecho
        {
            get { return _IndiceDerecho; }
            set
            {
                _IndiceDerecho = value;
                OnPropertyChanged("IndiceDerecho");
            }
        }

        private Brush _MedioDerecho;
        public Brush MedioDerecho
        {
            get { return _MedioDerecho; }
            set
            {
                _MedioDerecho = value;
                OnPropertyChanged("MedioDerecho");
            }
        }

        private Brush _AnularDerecho;
        public Brush AnularDerecho
        {
            get { return _AnularDerecho; }
            set
            {
                _AnularDerecho = value;
                OnPropertyChanged("AnularDerecho");
            }
        }

        private Brush _MeñiqueDerecho;
        public Brush MeñiqueDerecho
        {
            get { return _MeñiqueDerecho; }
            set
            {
                _MeñiqueDerecho = value;
                OnPropertyChanged("MeñiqueDerecho");
            }
        }

        private Brush _PulgarIzquierdo;
        public Brush PulgarIzquierdo
        {
            get { return _PulgarIzquierdo; }
            set
            {
                _PulgarIzquierdo = value;
                OnPropertyChanged("PulgarIzquierdo");
            }
        }

        private Brush _IndiceIzquierdo;
        public Brush IndiceIzquierdo
        {
            get { return _IndiceIzquierdo; }
            set
            {
                _IndiceIzquierdo = value;
                OnPropertyChanged("IndiceIzquierdo");
            }
        }

        private Brush _MedioIzquierdo;
        public Brush MedioIzquierdo
        {
            get { return _MedioIzquierdo; }
            set
            {
                _MedioIzquierdo = value;
                OnPropertyChanged("MedioIzquierdo");
            }
        }

        private Brush _AnularIzquierdo;
        public Brush AnularIzquierdo
        {
            get { return _AnularIzquierdo; }
            set
            {
                _AnularIzquierdo = value;
                OnPropertyChanged("AnularIzquierdo");
            }
        }

        private Brush _MeñiqueIzquierdo;
        public Brush MeñiqueIzquierdo
        {
            get { return _MeñiqueIzquierdo; }
            set
            {
                _MeñiqueIzquierdo = value;
                OnPropertyChanged("MeñiqueIzquierdo");
            }
        }
        #endregion

        private INGRESO _IngresoSeleccionado;
        public INGRESO IngresoSeleccionado
        {
            get { return _IngresoSeleccionado; }
            set { _IngresoSeleccionado = value;
            if (value != null)
            {
                if (value.IMPUTADO != null)
                {
                    Sexo = value.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO";
                    //Padres
                    if (value.IMPUTADO.IMPUTADO_PADRES != null)
                    {
                        ImputadoPadre = value.IMPUTADO.IMPUTADO_PADRES.FirstOrDefault(w => w.ID_PADRE == "P");
                        ImputadoMadre = value.IMPUTADO.IMPUTADO_PADRES.FirstOrDefault(w => w.ID_PADRE == "M");
                    }
                    else
                        ImputadoPadre = ImputadoMadre = null;

                    //Filiacion
                    if (value.IMPUTADO.IMPUTADO_FILIACION != null)
                        PopulateMediaFiliacion();
                }
                else
                { 
                    Sexo = string.Empty;
                    ImputadoPadre = ImputadoMadre = null;
                }
                
            }
            else
            { 
                Sexo = string.Empty;
                ImputadoPadre = ImputadoMadre = null;
            }

                OnPropertyChanged("IngresoSeleccionado"); }
        }

        private System.DateTime? _FechaInicioBusqueda = Fechas.GetFechaDateServer;

        public System.DateTime? FechaInicioBusqueda
        {
            get { return _FechaInicioBusqueda; }
            set { _FechaInicioBusqueda = value; OnPropertyChanged("FechaInicioBusqueda"); }
        }


        private ObservableCollection<INGRESO> _ListIngresos;
        public ObservableCollection<INGRESO> ListIngresos
        {
            get { return _ListIngresos; }
            set { _ListIngresos = value; OnPropertyChanged("ListIngresos"); }
        }

        private string sexo;
        public string Sexo
        {
            get { return sexo; }
            set { sexo = value; OnPropertyChanged("Sexo"); }
        }

        private IMPUTADO_PADRES imputadoPadre;
        public IMPUTADO_PADRES ImputadoPadre
        {
            get { return imputadoPadre; }
            set { imputadoPadre = value; OnPropertyChanged("ImputadoPadre"); }
        }

        private IMPUTADO_PADRES imputadoMadre;
        public IMPUTADO_PADRES ImputadoMadre
        {
            get { return imputadoMadre; }
            set { imputadoMadre = value; OnPropertyChanged("ImputadoMadre"); }
        }

        #region MediaFiliacion
        #region Señas Generales
        private string complexionSenias;
        public string ComplexionSenias
        {
            get { return complexionSenias; }
            set { complexionSenias = value; OnPropertyChanged("ComplexionSenias"); }
        }

        private string colorPielSenias;
        public string ColorPielSenias
        {
            get { return colorPielSenias; }
            set { colorPielSenias = value; OnPropertyChanged("ColorPielSenias"); }
        }

        private string caraSenias;
        public string CaraSenias
        {
            get { return caraSenias; }
            set { caraSenias = value; OnPropertyChanged("CaraSenias"); }
        }
        #endregion

        #region Sangre
        private string tipoSangre;
        public string TipoSangre
        {
            get { return tipoSangre; }
            set { tipoSangre = value; OnPropertyChanged("TipoSangre"); }
        }

        private string factorSangre;
        public string FactorSangre
        {
            get { return factorSangre; }
            set { factorSangre = value; OnPropertyChanged("FactorSangre"); }
        }
        #endregion

        #region Cabello
        private string cantidadCabello;
        public string CantidadCabello
        {
            get { return cantidadCabello; }
            set { cantidadCabello = value; OnPropertyChanged("CantidadCabello"); }
        }

        private string colorCabello;
        public string ColorCabello
        {
            get { return colorCabello; }
            set { colorCabello = value; OnPropertyChanged("ColorCabello"); }
        }

        private string formaCabello;
        public string FormaCabello
        {
            get { return formaCabello; }
            set { formaCabello = value; OnPropertyChanged("FormaCabello"); }
        }

        private string calvicieCabello;
        public string CalvicieCabello
        {
            get { return calvicieCabello; }
            set { calvicieCabello = value; OnPropertyChanged("CalvicieCabello"); }
        }

        private string implantacionCabello;
        public string ImplantacionCabello
        {
            get { return implantacionCabello; }
            set { implantacionCabello = value; OnPropertyChanged("ImplantacionCabello"); }
        }
        #endregion

        #region Frente
        private string alturaFrente;
        public string AlturaFrente
        {
            get { return alturaFrente; }
            set { alturaFrente = value; OnPropertyChanged("AlturaFrente"); }
        }

        private string inclinacionFrente;
        public string InclinacionFrente
        {
            get { return inclinacionFrente; }
            set { inclinacionFrente = value; OnPropertyChanged("InclinacionFrente"); }
        }

        private string anchoFrente;
        public string AnchoFrente
        {
            get { return anchoFrente; }
            set { anchoFrente = value; OnPropertyChanged("AnchoFrente"); }
        }
        #endregion

        #region Cejas
        private string direccionCejas;
        public string DireccionCejas
        {
            get { return direccionCejas;  }
            set { direccionCejas = value; OnPropertyChanged("DireccionCejas"); }
        }

        private string implantacionCejas;
        public string ImplantacionCejas
        {
            get { return implantacionCejas; }
            set { implantacionCejas = value; OnPropertyChanged("ImplantacionCejas"); }
        }

        private string formaCejas;
        public string FormaCejas
        {
            get { return formaCejas; }
            set { formaCejas = value; OnPropertyChanged("FormaCejas"); }
        }

        private string tamanioCejas;
        public string TamanioCejas
        {
            get { return tamanioCejas; }
            set { tamanioCejas = value; OnPropertyChanged("TamanioCejas"); }
        }
        #endregion

        #region Ojos
        private string colorOjos;
        public string ColorOjos
        {
            get { return colorOjos; }
            set { colorOjos = value; OnPropertyChanged("ColorOjos"); }
        }

        private string formaOjos;
        public string FormaOjos
        {
            get { return formaOjos; }
            set { formaOjos = value; OnPropertyChanged("FormaOjos"); }
        }

        private string tamanioOjos;
        public string TamanioOjos
        {
            get { return tamanioOjos; }
            set { tamanioOjos = value; OnPropertyChanged("TamanioOjos"); }
        }
        #endregion

        #region Nariz
        private string raizNariz;
        public string RaizNariz
        {
            get { return raizNariz; }
            set { raizNariz = value; OnPropertyChanged("RaizNariz"); }
        }

        private string dorsoNariz;
        public string DorsoNariz
        {
            get { return dorsoNariz; }
            set { dorsoNariz = value; OnPropertyChanged("DorsoNariz"); }
        }

        private string anchoNariz;
        public string AnchoNariz
        {
            get { return anchoNariz; }
            set { anchoNariz = value; OnPropertyChanged("AnchoNariz"); }
        }

        private string baseNariz;
        public string BaseNariz
        {
            get { return baseNariz; }
            set { baseNariz = value; OnPropertyChanged("BaseNariz"); }
        }

        private string alturaNariz;
        public string AlturaNariz
        {
            get { return alturaNariz; }
            set { alturaNariz = value; OnPropertyChanged("AlturaNariz"); }
        }
        #endregion

        #region Labios
        private string espesorLabios;
        public string EspesorLabios
        {
            get { return espesorLabios; }
            set { espesorLabios = value; OnPropertyChanged("EspesorLabios"); }
        }

        private string alturaLabios;
        public string AlturaLabios
        {
            get { return alturaLabios; }
            set { alturaLabios = value; OnPropertyChanged("AlturaLabios"); }
        }

        private string prominenciaLabios;
        public string ProminenciaLabios
        {
            get { return prominenciaLabios; }
            set { prominenciaLabios = value; OnPropertyChanged("ProminenciaLabios"); }
        }
        #endregion

        #region Boca
        private string tamanioBoca;
        public string TamanioBoca
        {
            get { return tamanioBoca; }
            set { tamanioBoca = value; OnPropertyChanged("TamanioBoca"); }
        }

        private string comisurasBoca;
        public string ComisurasBoca
        {
            get { return comisurasBoca; }
            set { comisurasBoca = value; OnPropertyChanged("ComisurasBoca"); }
        }
        #endregion

        #region Menton
        private string formaMenton;
        public string FormaMenton
        {
            get { return formaMenton; }
            set { formaMenton = value; OnPropertyChanged("FormaMenton"); }
        }

        private string tipoMenton;
        public string TipoMenton
        {
            get { return tipoMenton; }
            set { tipoMenton = value; OnPropertyChanged("TipoMenton"); }
        }

        private string inclinacionMenton;
        public string InclinacionMenton
        {
            get { return inclinacionMenton; }
            set { inclinacionMenton = value; OnPropertyChanged("InclinacionMenton"); }
        }
        #endregion

        #region Oreja
        private string formaOreja;
        public string FormaOreja
        {
            get { return formaOreja; }
            set { formaOreja = value; OnPropertyChanged("FormaOreja"); }
        }
        #endregion

        #region Helix
        private string originalHelix;
        public string OriginalHelix
        {
            get { return originalHelix; }
            set { originalHelix = value; OnPropertyChanged("OriginalHelix"); }
        }

        private string superiorHelix;
        public string SuperiorHelix
        {
            get { return superiorHelix; }
            set { superiorHelix = value; OnPropertyChanged("SuperiorHelix"); }
        }

        private string posteriorHelix;
        public string PosteriorHelix
        {
            get { return posteriorHelix; }
            set { posteriorHelix = value; OnPropertyChanged("PosteriorHelix"); }
        }

        private string adherenciaHelix;
        public string AdherenciaHelix
        {
            get { return adherenciaHelix; }
            set { adherenciaHelix = value; OnPropertyChanged("AdherenciaHelix"); }
        }
        #endregion

        #region Lobulo
        private string contornoLobulo;
        public string ContornoLobulo
        {
            get { return contornoLobulo; }
            set { contornoLobulo = value; OnPropertyChanged("ContornoLobulo"); }
        }

        private string adherenciaLobulo;
        public string AdherenciaLobulo
        {
            get { return adherenciaLobulo; }
            set { adherenciaLobulo = value; OnPropertyChanged("AdherenciaLobulo"); }
        }

        private string particularidadLobulo;
        public string ParticularidadLobulo
        {
            get { return particularidadLobulo; }
            set { particularidadLobulo = value; OnPropertyChanged("ParticularidadLobulo"); }
        }

        private string dimensionLobulo;
        public string DimensionLobulo
        {
            get { return dimensionLobulo; }
            set { dimensionLobulo = value; OnPropertyChanged("DimensionLobulo"); }
        }
        #endregion
        #endregion

        private ObservableCollection<IMPUTADO_PANDILLA> imputadoPandilla;
        public ObservableCollection<IMPUTADO_PANDILLA> ImputadoPandilla
        {
            get { return imputadoPandilla; }
            set { imputadoPandilla = value; OnPropertyChanged("ImputadoPandilla"); }
        }

        #region Datos Generales CP
        //DATOS
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

        //ARBOL INGRESOS
        private List<TreeViewList> _TreeList;
        public List<TreeViewList> TreeList
        {
            get { return _TreeList; }
            set
            {
                _TreeList = value;
                OnPropertyChanged("TreeList");
            }
        }

        //ARBOL UBICACIONES
        private List<TreeViewList> _TreeListUbicacion;
        public List<TreeViewList> TreeListUbicacion
        {
            get { return _TreeListUbicacion; }
            set
            {
                _TreeListUbicacion = value;
                OnPropertyChanged("TreeListUbicacion");
            }
        }

        //ARBOL DELITOS
        private List<TreeViewList> _TreeListDelito;
        public List<TreeViewList> TreeListDelito
        {
            get { return _TreeListDelito; }
            set
            {
                _TreeListDelito = value;
                //OnPropertyChanged("TreeListDelito");
            }
        }

        private int indexMenu;
        public int IndexMenu
        {
            get { return indexMenu; }
            set { indexMenu = value; OnPropertyChanged("IndexMenu"); }
        }

        private string tituloModal;
        public string TituloModal
        {
            get { return tituloModal; }
            set { tituloModal = value; OnPropertyChanged("TituloModal"); }
        }

        private string tituloAlias;
        public string TituloAlias
        {
            get { return tituloAlias; }
            set { tituloAlias = value; OnPropertyChanged("TituloAlias"); }
        }

        private string tituloApodo;
        public string TituloApodo
        {
            get { return tituloApodo; }
            set { tituloApodo = value; OnPropertyChanged("TituloApodo"); }
        }

        //NO ELIMINAR SI YA ESTAN GRABADOS EN BASE DE DATOS
        private bool eliminarVisible;
        public bool EliminarVisible
        {
            get { return eliminarVisible; }
            set { eliminarVisible = value; OnPropertyChanged("EliminarVisible"); }
        }

        private bool causaPenalDelitoEmpty = true;
        public bool CausaPenalDelitoEmpty
        {
            get { return causaPenalDelitoEmpty; }
            set { causaPenalDelitoEmpty = value; OnPropertyChanged("CausaPenalDelitoEmpty"); }
        }


        //LISTADO FECHAS TRASLAPE
        private ObservableCollection<EmpalmeFechas> lstFechasTraslape;
        public ObservableCollection<EmpalmeFechas> LstFechasTraslape
        {
            get { return lstFechasTraslape; }
            set { lstFechasTraslape = value; OnPropertyChanged("LstFechasTraslape"); }
        }

        private string empalmeDescr;
        public string EmpalmeDescr
        {
            get { return empalmeDescr; }
            set { empalmeDescr = value; OnPropertyChanged("EmpalmeDescr"); }
        }

        private bool enCausaPenal = false;
        public bool EnCausaPenal
        {
            get { return enCausaPenal; }
            set { enCausaPenal = value; }
        }
        #endregion

        private PdfViewer _pdfViewer;
        public PdfViewer pdfViewer
        {
            get { return _pdfViewer; }
            set { _pdfViewer = value; }
        }
        //pdfViewer3


        private enum eTiposHistoriaClinica
        {
            MEDICA = 1,
            DENTAL = 2
        };

        private PdfViewer _pdfViewer3;
        public PdfViewer pdfViewer3
        {
            get { return _pdfViewer3; }
            set { _pdfViewer3 = value; }
        }
        private PdfViewer _pdfViewerHistoriaClinicaMedica;
        public PdfViewer pdfViewerHistoriaClinicaMedica
        {
            get { return _pdfViewerHistoriaClinicaMedica; }
            set { _pdfViewerHistoriaClinicaMedica = value; }
        }

        private byte[] _DocumentoDigitalizado;
        public byte[] DocumentoDigitalizado
        {
            get { return _DocumentoDigitalizado; }
            set
            {
                _DocumentoDigitalizado = value;
                OnPropertyChanged("DocumentoDigitalizado");
            }
        }

        private byte[] _DocumentoDigitalizado2;
        public byte[] DocumentoDigitalizado2
        {
            get { return _DocumentoDigitalizado2; }
            set
            {
                _DocumentoDigitalizado2 = value;
                OnPropertyChanged("DocumentoDigitalizado2");
            }
        }

        private DateTime? _FechaInicioBusquedaHojasEnfermeria;
        public DateTime? FechaInicioBusquedaHojasEnfermeria
        {
            get { return _FechaInicioBusquedaHojasEnfermeria; }
            set { _FechaInicioBusquedaHojasEnfermeria = value; OnPropertyChanged("FechaInicioBusquedaHojasEnfermeria"); }
        }

        private DateTime? _FechaFinBusquedaHojasEnfermeria;
        public DateTime? FechaFinBusquedaHojasEnfermeria
        {
            get { return _FechaFinBusquedaHojasEnfermeria; }
            set { _FechaFinBusquedaHojasEnfermeria = value; OnPropertyChanged("FechaFinBusquedaHojasEnfermeria"); }
        }

        private DateTime? _FechaInicioBusquedaHojasLiquidos;
        public DateTime? FechaInicioBusquedaHojasLiquidos
        {
            get { return _FechaInicioBusquedaHojasLiquidos; }
            set { _FechaInicioBusquedaHojasLiquidos = value; OnPropertyChanged("FechaInicioBusquedaHojasLiquidos"); }
        }

        private DateTime? _FechaFinBusquedaHojasLiquidos;
        public DateTime? FechaFinBusquedaHojasLiquidos
        {
            get { return _FechaFinBusquedaHojasLiquidos; }
            set { _FechaFinBusquedaHojasLiquidos = value; OnPropertyChanged("FechaFinBusquedaHojasLiquidos"); }
        }

        private ObservableCollection<LIQUIDO_HOJA_CTRL> lstHojasLiquidos;
        public ObservableCollection<LIQUIDO_HOJA_CTRL> LstHojasLiquidos
        {
            get { return lstHojasLiquidos; }
            set { lstHojasLiquidos = value; OnPropertyChanged("LstHojasLiquidos"); }
        }

        private decimal? selectTurnosHCL = -1;
        public decimal? SelectTurnosHCL
        {
            get { return selectTurnosHCL; }
            set { selectTurnosHCL = value; OnPropertyChanged("SelectTurnosHCL"); }
        }
        private ObservableCollection<LIQUIDO_TURNO> listTurnosHCL;
        public ObservableCollection<LIQUIDO_TURNO> ListTurnosHCL
        {
            get { return listTurnosHCL; }
            set { listTurnosHCL = value; OnPropertyChanged("ListTurnosHCL"); }
        }

        private LIQUIDO_HOJA_CTRL seletedHojasLiquidos;
        public LIQUIDO_HOJA_CTRL SeletedHojasLiquidos
        {
            get { return seletedHojasLiquidos; }
            set { seletedHojasLiquidos = value; OnPropertyChanged("SeletedHojasLiquidos"); }
        }

        private ObservableCollection<HOJA_ENFERMERIA> lstHojasenfermeria;
        public ObservableCollection<HOJA_ENFERMERIA> LstHojasenfermeria
        {
            get { return lstHojasenfermeria; }
            set { lstHojasenfermeria = value; OnPropertyChanged("LstHojasenfermeria"); }
        }

        private Visibility visibleDatosKardexReporte = Visibility.Collapsed;
        public Visibility VisibleDatosKardexReporte
        {
            get { return visibleDatosKardexReporte; }
            set { visibleDatosKardexReporte = value; OnPropertyChanged("VisibleDatosKardexReporte"); }
        }

        private Visibility visibleReporteHistoriaClinicaMedica = Visibility.Collapsed;
        public Visibility VisibleReporteHistoriaClinicaMedica
        {
            get { return visibleReporteHistoriaClinicaMedica; }
            set { visibleReporteHistoriaClinicaMedica = value; OnPropertyChanged("VisibleReporteHistoriaClinicaMedica"); }
        }

        private Visibility visibleReporteHistoriaClinicaDental = Visibility.Collapsed;
        public Visibility VisibleReporteHistoriaClinicaDental
        {
            get { return visibleReporteHistoriaClinicaDental; }
            set { visibleReporteHistoriaClinicaDental = value; OnPropertyChanged("VisibleReporteHistoriaClinicaDental"); }
        }

        private Visibility visibleReporteBitacoraHospitalizaciones = Visibility.Collapsed;
        public Visibility VisibleReporteBitacoraHospitalizaciones
        {
            get { return visibleReporteBitacoraHospitalizaciones; }
            set { visibleReporteBitacoraHospitalizaciones = value; OnPropertyChanged("VisibleReporteBitacoraHospitalizaciones"); }
        }

        private short? _IndiceHistoriasClinicas = -1;
        public short? IndiceHistoriasClinicas
        {
            get { return _IndiceHistoriasClinicas; }
            set 
            { 
                _IndiceHistoriasClinicas = value; 
                OnPropertyChanged("IndiceHistoriasClinicas");
            }
        }

        private bool TieneHistoriaClinicaMedica { get; set; }
        private bool TieneHistoriaClinicaDental { get; set; }

        private HOJA_ENFERMERIA seletedHojaEnfermeria;
        public HOJA_ENFERMERIA SeletedHojaEnfermeria
        {
            get { return seletedHojaEnfermeria; }
            set { seletedHojaEnfermeria = value; OnPropertyChanged("SeletedHojaEnfermeria"); }
        }
        private ObservableCollection<CustomArchivosSimple> listArchivosHCD;
        public ObservableCollection<CustomArchivosSimple> ListArchivosHCD
        {
            get { return listArchivosHCD; }
            set { listArchivosHCD = value; OnPropertyChanged("ListArchivosHCD"); }
        }

        private CustomArchivosSimple selectArchivosHCD;
        public CustomArchivosSimple SelectArchivosHCD
        {
            get { return selectArchivosHCD; }
            set { selectArchivosHCD = value; OnPropertyChanged("SelectArchivosHCD"); }
        }

        private ObservableCollection<CustomArchivosSimple> listArchivosHCM;
        public ObservableCollection<CustomArchivosSimple> ListArchivosHCM
        {
            get { return listArchivosHCM; }
            set { listArchivosHCM = value; OnPropertyChanged("ListArchivosHCM"); }
        }

        private CustomArchivosSimple selectArchivosHCM;
        public CustomArchivosSimple SelectArchivosHCM
        {
            get { return selectArchivosHCM; }
            set { selectArchivosHCM = value; OnPropertyChanged("SelectArchivosHCM"); }
        }

        public class CustomArchivosSimple
        { //clase para mostrar los documentos de las historias clinicas, carga sin los archivos para mejorar el rendimiento de la busqueda
            public string TipoDocumento { get; set; }
            public string Formato { get; set; }
            public string Disponible { get; set; }
            public short Consecutivo { get; set; }
            public string Generico { get; set; }
            public decimal ConsecutivoDental { get; set; }
            public string Generico2 { get; set; }
        }

        private ObservableCollection<DocumentoExpedienteImputado> ListExpedienteFisicoAuxiliar;
        
        private ObservableCollection<DocumentoExpedienteImputado> _ListExpedienteFisico;
        public ObservableCollection<DocumentoExpedienteImputado> ListExpedienteFisico
        {
            get { return _ListExpedienteFisico; }
            set { _ListExpedienteFisico = value; OnPropertyChanged("ListExpedienteFisico"); }
        }

        private string _TextFiltroExpediente;
        public string TextFiltroExpediente
        {
            get { return _TextFiltroExpediente; }
            set
            {
                _TextFiltroExpediente = value;
                if (value != null && ListExpedienteFisicoAuxiliar.Any())
                {
                    ListExpedienteFisico = new ObservableCollection<DocumentoExpedienteImputado>(ListExpedienteFisicoAuxiliar.Where(w => w.TIPO_DOCTO.Contains(value)));
                }
                OnPropertyChanged("TextFiltroExpediente");
            }
        }

        private short? formato_documentacion_excarcelacion = null;
        public short? Formato_Documentacion_Excarcelacion
        {
            get { return formato_documentacion_excarcelacion; }
            set
            {
                formato_documentacion_excarcelacion = value;
                OnPropertyChanged("Formato_Documentacion_Excarcelacion");
            }
        }

        private TextControlView _tc2;
        public TextControlView tc2
        {
            get { return _tc2; }
            set { _tc2 = value; }
        }
    }
}

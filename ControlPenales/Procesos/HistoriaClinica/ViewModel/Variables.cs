using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using SSP.Servidor.ModelosExtendidos;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Controls;
namespace ControlPenales
{
    partial class HistoriaClinicaViewModel
    {

        private enum eFamiliares
        {
            MADRE = 1002,
            PADRE = 1001,
            CONYUGE = 1054,
            HIJOS = 1055,
            HERMANOS = 1056
        };

        private enum eTiposMedicos
        {
            DENTISTA = 33
        };

        private enum eFormatosDigitalizacion
        {
            PDF = 3,
            DOCX = 1,
            XLS = 2,
            DOC = 4,
            JPEG = 5,
            XLSX = 6,
            JPG = 15
        };

        private enum eformatosPermitidos
        {
            PDF = 3,
            JPEG = 5,
            JPG = 15
        };

        private ObservableCollection<SentenciaIngreso> lstSentenciasIngresos;
        public ObservableCollection<SentenciaIngreso> LstSentenciasIngresos
        {
            get { return lstSentenciasIngresos; }
            set { lstSentenciasIngresos = value; OnPropertyChanged("LstSentenciasIngresos"); }
        }

        #region Generales
        private bool _TabsEnabled = false;
        public bool TabsEnabled
        {
            get { return _TabsEnabled; }
            set { _TabsEnabled = value; OnPropertyChanged("TabsEnabled"); }
        }
        private short _SelectExploracionFisica;
        public short SelectExploracionFisica
        {
            get { return _SelectExploracionFisica; }
            set
            {
                _SelectExploracionFisica = value;
                TextBoxGenericoVisible = value != (short)enumExploracionFisica.GENERAL ? Visibility.Visible : Visibility.Collapsed;
                switch (value)
                {
                    case (short)enumExploracionFisica.ABDOMEN:
                        //SetValidacionesEFAbdomen();
                        //TextGenerico = TextAbdomen;
                        break;
                    case (short)enumExploracionFisica.CABEZA:
                        //SetValidacionesEFCabeza();
                        //TextGenerico = TextCabeza;
                        break;
                    case (short)enumExploracionFisica.CUELLO:
                        //SetValidacionesEFCuello();
                        //TextGenerico = TextCuello;
                        break;
                    case (short)enumExploracionFisica.EXTREMIDADES:
                        //SetValidacionesEFExtremidades();
                        //TextGenerico = TextExtremidades;
                        break;
                    case (short)enumExploracionFisica.GENERAL:
                        //SetValidacionesEFGeneral();
                        //TextGenerico = Text;
                        break;
                    case (short)enumExploracionFisica.GENITALES:
                        //SetValidacionesEFGenitales();
                        //TextGenerico = TextGenitales;
                        break;
                    case (short)enumExploracionFisica.IMPRESION_DIAGNOSTICA:
                        //SetValidacionesEFImpresionDiagnostica();
                        //TextGenerico = TextImpresionDiagnostica;
                        break;
                    case (short)enumExploracionFisica.RECTO:
                        //SetValidacionesEFRecto();
                        //TextGenerico = TextRecto;
                        break;
                    case (short)enumExploracionFisica.RESULTADOS_ANALISIS_CLINICOS:
                        //SetValidacionesEFResultadosClinicos();
                        //TextGenerico = TextResultadosAnalisisClinicos;
                        break;
                    case (short)enumExploracionFisica.RESULTADOS_ESTUDIOS_GABINETE:
                        //SetValidacionesEFResultadosGabinete();
                        //TextGenerico = TextResultadosestudiosGabinete;
                        break;
                    case (short)enumExploracionFisica.SIGNOS_VITALES:
                        //SetValidacionesEFSignosVitales();
                        //TextGenerico = TextConclusiones;
                        TextBoxGenericoVisible = Visibility.Collapsed;
                        break;
                    case (short)enumExploracionFisica.TORAX:
                        //SetValidacionesEFTorax();
                        //TextGenerico = TextTorax;
                        break;
                }
                OnPropertyChanged("SelectExploracionFisica");
            }
        }
        private short _SelectAparatosSistemas;
        public short SelectAparatosSistemas
        {
            get { return _SelectAparatosSistemas; }
            set
            {
                _SelectAparatosSistemas = value;
                switch (value)
                {
                    case (short)enumAparatosYSistemas.CARDIOVASCULAR:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASCardiovascular();
                        //TextGenerico = TextCardiovascular;
                        break;
                    case (short)enumAparatosYSistemas.DIGESTIVO:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASDigestivo();
                        //TextGenerico = TextDigestivo;
                        break;
                    case (short)enumAparatosYSistemas.ENDOCRINO:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASEndocrino();
                        //TextGenerico = TextEndocrino;
                        break;
                    case (short)enumAparatosYSistemas.GENITAL:
                        TextBoxGenericoVisible = Visibility.Visible;
                        if (SelectIngreso.IMPUTADO.SEXO == "M")
                        {
                            TextGenerico = TextGenitalHombres;
                            GenitalHombre = Visibility.Visible;
                            GenitalMujer = Visibility.Collapsed;
                        }
                        else
                        {
                            TextGenerico = TextGenitalMujeres;
                            GenitalHombre = Visibility.Collapsed;
                            GenitalMujer = Visibility.Visible;
                        }
                        // SetValidacionesASGenital();
                        break;
                    case (short)enumAparatosYSistemas.HEMATICO_LINFATICO:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASHematico();
                        //TextGenerico = TextHematicoLinfatico;
                        break;
                    case (short)enumAparatosYSistemas.MUSCULO_ESQUELETICO:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASMusculoEsqueletico();
                        //TextGenerico = TextMusculoEsqueletico;
                        break;
                    case (short)enumAparatosYSistemas.NERVIOSO:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASNervioso();
                        //TextGenerico = TextNervioso;
                        break;
                    case (short)enumAparatosYSistemas.PIEL_Y_ANEXOS:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASPielAnexos();
                        //TextGenerico = TextPielAnexos;
                        break;
                    case (short)enumAparatosYSistemas.RESPIRATORIO:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASRespiratorio();
                        //TextGenerico = TextRespiratorio;
                        break;
                    case (short)enumAparatosYSistemas.SINTOMAS_GENERALES:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASSintomasGenerales();
                        //TextGenerico = TextSintomasGenerales;
                        break;
                    case (short)enumAparatosYSistemas.TERAPEUTICA_PREVIA:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASTerapeuticaPrevia();
                        //TextGenerico = TextTerapeuticaPrevia;
                        break;
                    case (short)enumAparatosYSistemas.URINARIO:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesASUrinario();
                        //TextGenerico = TextUrinario;
                        break;
                }
                OnPropertyChanged("SelectAparatosSistemas");
            }
        }
        private short _SelectTab;
        public short SelectTab
        {
            get { return _SelectTab; }
            set
            {
                _SelectTab = value;
                //TextGenerico = string.Empty;
                switch (value)
                {
                    case (short)enumHistoriaClinica.ANTECEDENTES_NO_PATOLOGICOS:
                        //TextBoxGenericoVisible = Visibility.Collapsed;
                        break;
                    case (short)enumHistoriaClinica.ANTECEDENTES_PATOLOGICOS:
                        //TextBoxGenericoVisible = Visibility.Collapsed;
                        break;
                    case (short)enumHistoriaClinica.ANTECEDENTES_HEREDO_FAMILIARES:
                        TextBoxGenericoVisible = Visibility.Collapsed;
                        //SetValidacionesAHF();
                        break;
                    case (short)enumHistoriaClinica.CONCLUSIONES:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesConclusiones();
                        //TextGenerico = TextConclusiones;
                        break;
                    case (short)enumHistoriaClinica.CONSIDERACIONES_FINALES:
                        //TextBoxGenericoVisible = Visibility.Collapsed;
                        break;
                    case (short)enumHistoriaClinica.EXAMEN:
                        //TextBoxGenericoVisible = Visibility.Collapsed;
                        break;
                    case (short)enumHistoriaClinica.MAYORES_DE_65:
                        //TextBoxGenericoVisible = Visibility.Collapsed;
                        break;
                    case (short)enumHistoriaClinica.MUJERES:
                        //TextBoxGenericoVisible = Visibility.Collapsed;
                        break;
                    case (short)enumHistoriaClinica.PADECIMIENTO_ACTUAL:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SetValidacionesPadecimientoActual();
                        //TextGenerico = TextPadecimientoActual;
                        break;
                    case (short)enumHistoriaClinica.APARATOS_Y_SISTEMAS:
                        //TextBoxGenericoVisible = Visibility.Visible;
                        //SelectAparatosSistemas = (short)enumAparatosYSistemas.RESPIRATORIO;
                        //SetValidacionesASRespiratorio();
                        break;
                    case (short)enumHistoriaClinica.EXPLORACION_FISICA:
                        //if (SelectExploracionFisica == (short)enumExploracionFisica.GENERAL)
                        //{
                        //    TextBoxGenericoVisible = Visibility.Collapsed;
                        //    SetValidacionesEFGeneral();
                        //}
                        //else
                        //    SelectExploracionFisica = (short)enumExploracionFisica.GENERAL;
                        break;
                    case (short)enumHistoriaClinica.HEREDO_FAMILIARES_DENTAL:
                        break;
                    case (short)enumHistoriaClinica.PATOLOGICOS_DENTAL:
                        break;
                    case (short)enumHistoriaClinica.INTERROGATORIO:
                        break;
                    case (short)enumHistoriaClinica.EPLORACION_BUCODENTAL:
                        break;
                    case (short)enumHistoriaClinica.DIENTES:
                        break;
                    case (short)enumHistoriaClinica.ARTICULACION_TERMO:
                        break;
                    case (short)enumHistoriaClinica.ENCIAS:
                        break;
                    case (short)enumHistoriaClinica.BRUXISMO:
                        break;
                    case (short)enumHistoriaClinica.SIGNOS_VITALES:
                        break;
                    case (short)enumHistoriaClinica.ODONTOGRAMA_SEGUIMIENTO:
                        break;
                }
                OnPropertyChanged("SelectTab");
            }
        }
        private string _LabelTextGenerico;
        public string LabelTextGenerico
        {
            get { return _LabelTextGenerico; }
            set { _LabelTextGenerico = value; OnPropertyChanged("LabelTextGenerico"); }
        }
        private string _TextGenerico;
        public string TextGenerico
        {
            get { return _TextGenerico; }
            set
            {
                _TextGenerico = value;
                if (value != null)
                    switch (SelectTab)
                    {
                        case (short)enumHistoriaClinica.CONCLUSIONES:
                            //TextConclusiones = value;
                            break;
                        case (short)enumHistoriaClinica.PADECIMIENTO_ACTUAL:
                            //TextPadecimientoActual = value;
                            break;
                        case (short)enumHistoriaClinica.APARATOS_Y_SISTEMAS:
                            switch (SelectAparatosSistemas)
                            {
                                case (short)enumAparatosYSistemas.CARDIOVASCULAR:

                                    //TextCardiovascular = value;
                                    break;
                                case (short)enumAparatosYSistemas.DIGESTIVO:

                                    //TextDigestivo = value;
                                    break;
                                case (short)enumAparatosYSistemas.ENDOCRINO:

                                    //TextEndocrino = value;
                                    break;
                                case (short)enumAparatosYSistemas.GENITAL:

                                    if (SelectIngreso.IMPUTADO.SEXO == "M")
                                        TextGenitalHombres = value;
                                    else
                                        TextGenitalMujeres = value;
                                    break;
                                case (short)enumAparatosYSistemas.HEMATICO_LINFATICO:

                                    //TextHematicoLinfatico = value;
                                    break;
                                case (short)enumAparatosYSistemas.MUSCULO_ESQUELETICO:

                                    //TextMusculoEsqueletico = value;
                                    break;
                                case (short)enumAparatosYSistemas.NERVIOSO:

                                    //TextNervioso = value;
                                    break;
                                case (short)enumAparatosYSistemas.PIEL_Y_ANEXOS:

                                    //TextPielAnexos = value;
                                    break;
                                case (short)enumAparatosYSistemas.RESPIRATORIO:

                                    //TextRespiratorio = value;
                                    break;
                                case (short)enumAparatosYSistemas.SINTOMAS_GENERALES:

                                    //TextSintomasGenerales = value;
                                    break;
                                case (short)enumAparatosYSistemas.TERAPEUTICA_PREVIA:

                                    //TextTerapeuticaPrevia = value;
                                    break;
                                case (short)enumAparatosYSistemas.URINARIO:

                                    //TextUrinario = value;
                                    break;
                            }
                            break;
                        case (short)enumHistoriaClinica.EXPLORACION_FISICA:
                            switch (SelectExploracionFisica)
                            {
                                case (short)enumExploracionFisica.ABDOMEN:
                                    //TextAbdomen = value;
                                    break;
                                case (short)enumExploracionFisica.CABEZA:
                                    //TextCabeza = value;
                                    break;
                                case (short)enumExploracionFisica.CUELLO:
                                    //TextCuello = value;
                                    break;
                                case (short)enumExploracionFisica.EXTREMIDADES:
                                    //TextExtremidades = value;
                                    break;
                                case (short)enumExploracionFisica.GENERAL:

                                    break;
                                case (short)enumExploracionFisica.GENITALES:
                                    //TextGenitales = value;
                                    break;
                                case (short)enumExploracionFisica.IMPRESION_DIAGNOSTICA:
                                    //TextImpresionDiagnostica = value;
                                    break;
                                case (short)enumExploracionFisica.RECTO:
                                    //TextRecto = value;
                                    break;
                                case (short)enumExploracionFisica.RESULTADOS_ANALISIS_CLINICOS:
                                    //TextResultadosAnalisisClinicos = value;
                                    break;
                                case (short)enumExploracionFisica.RESULTADOS_ESTUDIOS_GABINETE:
                                    //TextResultadosestudiosGabinete = value;
                                    break;
                                case (short)enumExploracionFisica.SIGNOS_VITALES:

                                    break;
                                case (short)enumExploracionFisica.TORAX:
                                    //TextTorax = value;
                                    break;
                            }
                            break;
                    }
                OnPropertyChanged("TextGenerico");
            }
        }
        private Visibility _GenitalHombre = Visibility.Collapsed;
        public Visibility GenitalHombre
        {
            get { return _GenitalHombre; }
            set { _GenitalHombre = value; OnPropertyChanged("GenitalHombre"); }
        }
        private Visibility _GenitalMujer = Visibility.Collapsed;
        public Visibility GenitalMujer
        {
            get { return _GenitalMujer; }
            set { _GenitalMujer = value; OnPropertyChanged("GenitalMujer"); }
        }
        private Visibility _TextBoxGenericoVisible = Visibility.Collapsed;
        public Visibility TextBoxGenericoVisible
        {
            get { return _TextBoxGenericoVisible; }
            set { _TextBoxGenericoVisible = value; OnPropertyChanged("TextBoxGenericoVisible"); }
        }
        #endregion

        #region Permisos

        private bool _EsDentista = false;

        public bool EsDentista
        {
            get { return _EsDentista; }
            set { _EsDentista = value; OnPropertyChanged("EsDentista"); }
        }

        private bool pInsertar = true;
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

        private bool pEditar = true;
        public bool PEditar
        {
            get { return pEditar; }
            set
            {
                pEditar = value;
            }
        }

        private bool pConsultar = true;//se comenta para seguir con el proceso
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
                if (value)
                {
                    MenuBuscarEnabled = value;
                    BuscarImputadoHabilitado = value;
                }
            }
        }

        private bool pImprimir = true;
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

        private bool menuGuardarEnabled = true;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }

        private bool _MenuFichaEnabled = false;

        public bool MenuFichaEnabled
        {
            get { return _MenuFichaEnabled; }
            set { _MenuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }

        private bool _BuscarImputadoHabilitado = true;
        public bool BuscarImputadoHabilitado
        {
            get { return _BuscarImputadoHabilitado; }
            set { _BuscarImputadoHabilitado = value; OnPropertyChanged("BuscarImputadoHabilitado"); }
        }
        private bool menuBuscarEnabled = true;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }

        private bool menuReporteEnabled = true;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        #endregion

        #region Datos Expediente
        private byte[] _FotoIngreso = new Imagenes().getImagenPerson();
        public byte[] FotoIngreso
        {
            get { return _FotoIngreso; }
            set { _FotoIngreso = value; OnPropertyChanged("FotoIngreso"); }
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

        private int? _TextAnioImputado;
        public int? TextAnioImputado
        {
            get { return _TextAnioImputado; }
            set { _TextAnioImputado = value; OnPropertyChanged("TextAnioImputado"); }
        }
        private int? _TextFolioImputado;
        public int? TextFolioImputado
        {
            get { return _TextFolioImputado; }
            set { _TextFolioImputado = value; OnPropertyChanged("TextFolioImputado"); }
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

        private string _TextPaternoImputado;
        public string TextPaternoImputado
        {
            get { return _TextPaternoImputado; }
            set { _TextPaternoImputado = value; OnPropertyChanged("TextPaternoImputado"); }
        }
        private string _TextMaternoImputado;
        public string TextMaternoImputado
        {
            get { return _TextMaternoImputado; }
            set { _TextMaternoImputado = value; OnPropertyChanged("TextMaternoImputado"); }
        }
        private string _TextNombreImputado;
        public string TextNombreImputado
        {
            get { return _TextNombreImputado; }
            set { _TextNombreImputado = value; OnPropertyChanged("TextNombreImputado"); }
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
        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; OnPropertyChanged("ImagenIngreso"); }
        }
        private bool nombreBuscarHabilitado = true;
        public bool NombreBuscarHabilitado
        {
            get { return nombreBuscarHabilitado; }
            set { nombreBuscarHabilitado = value; OnPropertyChanged("NombreBuscarHabilitado"); }
        }
        private bool apellidoMaternoBuscarHabilitado = true;
        public bool ApellidoMaternoBuscarHabilitado
        {
            get { return apellidoMaternoBuscarHabilitado; }
            set { apellidoMaternoBuscarHabilitado = value; OnPropertyChanged("ApellidoMaternoBuscarHabilitado"); }
        }
        private bool apellidoPaternoBuscarHabilitado = true;
        public bool ApellidoPaternoBuscarHabilitado
        {
            get { return apellidoPaternoBuscarHabilitado; }
            set { apellidoPaternoBuscarHabilitado = value; OnPropertyChanged("ApellidoPaternoBuscarHabilitado"); }
        }
        private bool folioBuscarHabilitado = true;
        public bool FolioBuscarHabilitado
        {
            get { return folioBuscarHabilitado; }
            set { folioBuscarHabilitado = value; OnPropertyChanged("FolioBuscarHabilitado"); }
        }
        private bool anioBuscarHabilitado = true;
        public bool AnioBuscarHabilitado
        {
            get { return anioBuscarHabilitado; }
            set { anioBuscarHabilitado = value; OnPropertyChanged("AnioBuscarHabilitado"); }
        }
        #endregion

        #region Busqueda
        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }
        private bool crearNuevoExpedienteEnabled;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
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

        private bool _IsEnabledSignosVitales = true;

        public bool IsEnabledSignosVitales
        {
            get { return _IsEnabledSignosVitales; }
            set { _IsEnabledSignosVitales = value; OnPropertyChanged("IsEnabledSignosVitales"); }
        }

        private bool emptyExpedienteVisible;
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
        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                OnPropertyChanged("ListExpediente");
            }
        }
        private IMPUTADO SelectExpedienteAuxiliar;
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
                    //TextBotonSeleccionarIngreso = "aceptar";
                    SelectIngresoEnabled = true;
                    if (SelectIngreso != null)
                    {
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO != item)
                            {
                                TextBotonSeleccionarIngreso = "seleccionar ingreso";
                                SelectIngresoEnabled = true;
                            }
                            else
                            {
                                SelectIngresoEnabled = false;
                                break;
                            }
                        }
                    }
                    else
                        SelectIngresoEnabled = false;
                }
                else
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    EmptyIngresoVisible = true;
                }
                OnPropertyChanged("SelectExpediente");
            }
        }
        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set { aceptarBusquedaHuellaFocus = value; OnPropertyChanged("AceptarBusquedaHuellaFocus"); }
        }
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        private bool selectIngresoEnabled;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }
        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }
        private bool _BuscarReadOnly = true;
        public bool BuscarReadOnly
        {
            get { return _BuscarReadOnly; }
            set { _BuscarReadOnly = value; OnPropertyChanged("BuscarReadOnly"); }
        }
        private INGRESO SelectIngresoAuxiliar;
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
                    

                if (value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (value.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();

                TextBotonSeleccionarIngreso = "seleccionar ingreso";
                SelectIngresoEnabled = (!Parametro.ESTATUS_ADMINISTRATIVO_INACT.Contains(value.ID_ESTATUS_ADMINISTRATIVO));
                OnPropertyChanged("SelectIngreso");
            }
        }

        #endregion

        #region Historia Clinica

        #region Examen
        private short? _TextEdad;
        public short? TextEdad
        {
            get { return _TextEdad; }
            set { _TextEdad = value; OnPropertyChanged("TextEdad"); }
        }
        private string _SelectSexo;
        public string SelectSexo
        {
            get { return _SelectSexo; }
            set { _SelectSexo = value; OnPropertyChanged("SelectSexo"); }
        }
        private string _TextLugarNacimiento;
        public string TextLugarNacimiento
        {
            get { return _TextLugarNacimiento; }
            set { _TextLugarNacimiento = value; OnPropertyChanged("TextLugarNacimiento"); }
        }
        private DateTime? _SelectFechaNacimiento;
        public DateTime? SelectFechaNacimiento
        {
            get { return _SelectFechaNacimiento; }
            set { _SelectFechaNacimiento = value; OnPropertyChanged("SelectFechaNacimiento"); }
        }
        private string _TextEscolaridad;
        public string TextEscolaridad
        {
            get { return _TextEscolaridad; }
            set { _TextEscolaridad = value; OnPropertyChanged("TextEscolaridad"); }
        }
        private string _TextOcupacion;
        public string TextOcupacion
        {
            get { return _TextOcupacion; }
            set { _TextOcupacion = value; OnPropertyChanged("TextOcupacion"); }
        }
        private string _TextEstadoCivil;
        public string TextEstadoCivil
        {
            get { return _TextEstadoCivil; }
            set { _TextEstadoCivil = value; OnPropertyChanged("TextEstadoCivil"); }
        }
        private string _TextDelito;
        public string TextDelito
        {
            get { return _TextDelito; }
            set { _TextDelito = value; OnPropertyChanged("TextDelito"); }
        }
        private string _TextSentencia;
        public string TextSentencia
        {
            get { return _TextSentencia; }
            set { _TextSentencia = value; OnPropertyChanged("TextSentencia"); }
        }
        private DateTime? _TextAPartir;
        public DateTime? TextAPartir
        {
            get { return _TextAPartir; }
            set { _TextAPartir = value; OnPropertyChanged("TextAPartir"); }
        }

        private string _CentroReclusion;

        public string CentroReclusion
        {
            get { return _CentroReclusion; }
            set { _CentroReclusion = value; OnPropertyChanged("CentroReclusion"); }
        }
        private string _NombreImputado;

        public string NombreImputado
        {
            get { return _NombreImputado; }
            set { _NombreImputado = value; OnPropertyChanged("NombreImputado"); }
        }

        #endregion

        #region AHF
        private bool _ConyugeEnabled;
        public bool ConyugeEnabled
        {
            get { return _ConyugeEnabled; }
            set
            {
                _ConyugeEnabled = value;
                if (value)
                {
                    base.RemoveRule("TextEdadConyuge");
                    base.AddRule(() => TextEdadConyuge, () => TextEdadConyuge.HasValue, "EDAD CONYUGE ES REQUERIDO!");
                }
                else
                    base.RemoveRule("TextEdadConyuge");

                OnPropertyChanged("TextEdadConyuge");
                OnPropertyChanged("ConyugeEnabled");
            }
        }
        private bool _HijosEnabled;
        public bool HijosEnabled
        {
            get { return _HijosEnabled; }
            set { _HijosEnabled = value; OnPropertyChanged("HijosEnabled"); }
        }
        private bool _MadreEnabled;
        public bool MadreEnabled
        {
            get { return _MadreEnabled; }
            set
            {
                if (value)
                {
                    base.RemoveRule("TextEdadMadre");
                    base.AddRule(() => TextEdadMadre, () => TextEdadMadre.HasValue, "EDAD DE MADRE ES REQUERIDO!");
                }
                else
                    base.RemoveRule("TextEdadMadre");

                _MadreEnabled = value;
                OnPropertyChanged("MadreEnabled");
            }
        }
        private bool _PadreEnabled;
        public bool PadreEnabled
        {
            get { return _PadreEnabled; }
            set
            {
                _PadreEnabled = value;
                if (value)
                {
                    base.RemoveRule("TextEdadPadre");
                    base.AddRule(() => TextEdadPadre, () => TextEdadPadre.HasValue, "EDAD DEL PADRE ES REQUERIDO!");
                }
                else
                    base.RemoveRule("TextEdadPadre");

                OnPropertyChanged("TextEdadPadre");
                OnPropertyChanged("PadreEnabled");
            }
        }
        private string _CheckPadreVive;
        public string CheckPadreVive
        {
            get { return _CheckPadreVive; }
            set
            {
                _CheckPadreVive = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        PadreEnabled = true;
                        IsMuertoPadre = false;
                    }
                    else
                    {
                        IsMuertoPadre = true;
                        PadreEnabled = false;
                    }
                };

                OnPropertyChanged("CheckPadreVive");
                OnPropertyChanged("IsMuertoPadre");
                OnPropertyChanged("TextEdadPadre");
            }
        }
        private string _CheckPadrePadece;
        public string CheckPadrePadece
        {
            get { return _CheckPadrePadece; }
            set
            {
                _CheckPadrePadece = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                        IsEnabledPadecimientosPadre = true;
                    else
                        IsEnabledPadecimientosPadre = false;
                };

                OnPropertyChanged("CheckPadrePadece");
                OnPropertyChanged("IsEnabledPadecimientosPadre");
            }
        }
        private string _CheckMadreVive;
        public string CheckMadreVive
        {
            get { return _CheckMadreVive; }
            set
            {
                _CheckMadreVive = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        MadreEnabled = true;
                        IsMuertoMadre = false;
                    }
                    else
                    {
                        IsMuertoMadre = true;
                        MadreEnabled = false;
                    }
                }

                OnPropertyChanged("CheckMadreVive");
                OnPropertyChanged("IsMuertoMadre");
                OnPropertyChanged("TextEdadMadre");
                OnPropertyChanged("MadreEnabled");
            }
        }
        private string _CheckMadrePadece;
        public string CheckMadrePadece
        {
            get { return _CheckMadrePadece; }
            set
            {
                _CheckMadrePadece = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                        IsEnabledPadecimientosMadre = true;
                    else
                        IsEnabledPadecimientosMadre = false;
                }

                OnPropertyChanged("CheckMadrePadece");
                OnPropertyChanged("IsEnabledPadecimientosMadre");
            }
        }
        private short? _TextEdadPadre;
        public short? TextEdadPadre
        {
            get { return _TextEdadPadre; }
            set { _TextEdadPadre = value; OnPropertyChanged("TextEdadPadre"); }
        }
        private short? _TextEdadMadre;
        public short? TextEdadMadre
        {
            get { return _TextEdadMadre; }
            set { _TextEdadMadre = value; OnPropertyChanged("TextEdadMadre"); }
        }
        private short? _TextHermanosMujeres;
        public short? TextHermanosMujeres
        {
            get { return _TextHermanosMujeres; }
            set { _TextHermanosMujeres = value; OnPropertyChanged("TextHermanosMujeres"); }
        }
        private short? _TextHermanosHombres;
        public short? TextHermanosHombres
        {
            get { return _TextHermanosHombres; }
            set { _TextHermanosHombres = value; OnPropertyChanged("TextHermanosHombres"); }
        }
        private string _CheckHermanosVivos;
        public string CheckHermanosVivos
        {
            get { return _CheckHermanosVivos; }
            set
            {
                _CheckHermanosVivos = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                        IsMuertoHnos = false;
                    if (value == "N")
                        IsMuertoHnos = true;
                    if (value == "X")
                    {

                        TextHermanosMujeres = TextHermanosHombres = 0;
                        CheckHermanosSanos = IsCheckedDiabHnos = IsCheckedTBHnos = IsCheckedCAHnos = IsCheckedCardiHnos = IsCheckedEpiHnos = IsCheckedMentHnos = IsCheckedAlergiHnos = IsCheckedHipertHnos = "N";
                        CuandoMuerteHnos = null;
                        CausaMuerteHnos = string.Empty;
                        IsMuertoHnos = false;

                        base.RemoveRule("CuandoMuerteHnos");
                        base.RemoveRule("CausaMuerteHnos");
                        base.RemoveRule("TextHermanosHombres");
                        base.RemoveRule("TextHermanosMujeres");

                        OnPropertyChanged("TextHermanosMujeres");
                        OnPropertyChanged("TextHermanosHombres");
                        OnPropertyChanged("CheckHermanosSanos");
                        OnPropertyChanged("IsCheckedDiabHnos");
                        OnPropertyChanged("IsCheckedTBHnos");
                        OnPropertyChanged("IsCheckedCAHnos");
                        OnPropertyChanged("IsCheckedCardiHnos");
                        OnPropertyChanged("IsCheckedEpiHnos");
                        OnPropertyChanged("IsCheckedMentHnos");
                        OnPropertyChanged("IsCheckedAlergiHnos");
                        OnPropertyChanged("IsCheckedHipertHnos");
                        OnPropertyChanged("CuandoMuerteHnos");
                        OnPropertyChanged("CausaMuerteHnos");
                        OnPropertyChanged("CuandoMuerteHnos");
                        OnPropertyChanged("CausaMuerteHnos");
                        OnPropertyChanged("TextHermanosHombres");
                        OnPropertyChanged("TextHermanosMujeres");

                        OnPropertyChanged("CuandoMuerteHnos");
                        OnPropertyChanged("CausaMuerteHnos");
                        OnPropertyChanged("TextHermanosHombres");
                        OnPropertyChanged("TextHermanosMujeres");
                    }
                }

                OnPropertyChanged("CheckHermanosVivos");
                OnPropertyChanged("IsMuertoHnos");
                OnPropertyChanged("TextHermanosHombres");
                OnPropertyChanged("TextHermanosMujeres");
            }
        }
        private string _CheckHermanosSanos;
        public string CheckHermanosSanos
        {
            get { return _CheckHermanosSanos; }
            set
            {
                _CheckHermanosSanos = value;
                if (value == "S")
                    IsEnabledPadecimientosHnos = true;
                else
                    IsEnabledPadecimientosHnos = false;

                OnPropertyChanged("CheckHermanosSanos");
            }
        }
        private string _CheckConyugeVive;
        public string CheckConyugeVive
        {
            get { return _CheckConyugeVive; }
            set
            {
                _CheckConyugeVive = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        ConyugeEnabled = true;
                        IsMuertoCony = false;
                    }
                    if (value == "N")
                        IsMuertoCony = true;

                    if (value == "X")
                    {
                        TextEdadConyuge = 0;
                        CheckConyugePadece = IsCheckedDiabCony = IsCheckedTBCony = IsCheckedCACony = IsCheckedCardiCony = IsCheckedEpiCony = IsCheckedMentCony = IsCheckedAlergiCony = IsCheckedHipertCony = "N";
                        CuandoMuerteCony = null;
                        CausaMuerteCony = string.Empty;
                        IsMuertoCony = false;

                        base.RemoveRule("CuandoMuerteCony");
                        base.RemoveRule("CausaMuerteCony");
                        OnPropertyChanged("IsMuertoCony");
                        OnPropertyChanged("TextEdadConyuge");
                        OnPropertyChanged("CheckConyugePadece");
                        OnPropertyChanged("IsCheckedDiabCony");
                        OnPropertyChanged("IsCheckedTBCony");
                        OnPropertyChanged("IsCheckedCACony");
                        OnPropertyChanged("IsCheckedCardiCony");
                        OnPropertyChanged("IsCheckedEpiCony");
                        OnPropertyChanged("IsCheckedMentCony");
                        OnPropertyChanged("IsCheckedAlergiCony");
                        OnPropertyChanged("IsCheckedHipertCony");
                        OnPropertyChanged("CuandoMuerteCony");
                        OnPropertyChanged("CausaMuerteCony");
                        OnPropertyChanged("CuandoMuerteCony");
                        OnPropertyChanged("CausaMuerteCony");
                    }
                }

                OnPropertyChanged("CheckConyugeVive");
                OnPropertyChanged("TextEdadConyuge");
                OnPropertyChanged("IsMuertoCony");
            }
        }
        private string _CheckConyugePadece;
        public string CheckConyugePadece
        {
            get { return _CheckConyugePadece; }
            set
            {
                _CheckConyugePadece = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                        IsEnabledPadecimientosCony = true;
                    else
                        IsEnabledPadecimientosCony = false;
                };

                OnPropertyChanged("CheckConyugePadece");
            }
        }
        private short? _TextEdadConyuge;
        public short? TextEdadConyuge
        {
            get { return _TextEdadConyuge; }
            set { _TextEdadConyuge = value; OnPropertyChanged("TextEdadConyuge"); }
        }
        private string _CheckHijosVive;
        public string CheckHijosVive
        {
            get { return _CheckHijosVive; }
            set
            {
                _CheckHijosVive = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        IsMuertoHijos = false;
                        HijosEnabled = true;
                    }
                    if (value == "N")
                    {
                        IsMuertoHijos = true;
                        HijosEnabled = false;
                    }

                    if (value == "X")
                    {
                        TextEdadesHijos = 0;
                        CheckHijosPadece = IsCheckedDiabHijos = IsCheckedTBHijos = IsCheckedCAHijos = IsCheckedCardiHijos = IsCheckedEpiHijos = IsCheckedMentHijos = IsCheckedAlergiHijos = IsCheckedHipertHijos = "N";
                        CuandoMuerteHijos = null;
                        IsMuertoHijos = false;
                        CausaMuerteHijos = string.Empty;
                        base.RemoveRule("IsCheckedDiabHijos");
                        base.RemoveRule("IsCheckedTBHijos");
                        base.RemoveRule("IsCheckedCAHijos");
                        base.RemoveRule("IsCheckedCardiHijos");
                        base.RemoveRule("IsCheckedEpiHijos");
                        base.RemoveRule("IsCheckedMentHijos");
                        base.RemoveRule("IsCheckedAlergiHijos");
                        base.RemoveRule("IsCheckedHipertHijos");

                        OnPropertyChanged("IsMuertoHijos");
                        OnPropertyChanged("TextEdadesHijos");
                        OnPropertyChanged("CheckHijosPadece");
                        OnPropertyChanged("IsCheckedDiabHijos");
                        OnPropertyChanged("IsCheckedTBHijos");
                        OnPropertyChanged("IsCheckedCAHijos");
                        OnPropertyChanged("IsCheckedCardiHijos");
                        OnPropertyChanged("IsCheckedEpiHijos");
                        OnPropertyChanged("IsCheckedMentHijos");
                        OnPropertyChanged("IsCheckedAlergiHijos");
                        OnPropertyChanged("IsCheckedHipertHijos");
                        OnPropertyChanged("CuandoMuerteHijos");
                        OnPropertyChanged("CausaMuerteHijos");

                        OnPropertyChanged("IsCheckedDiabHijos");
                        OnPropertyChanged("IsCheckedTBHijos");
                        OnPropertyChanged("IsCheckedCAHijos");
                        OnPropertyChanged("IsCheckedCardiHijos");
                        OnPropertyChanged("IsCheckedEpiHijos");
                        OnPropertyChanged("IsCheckedMentHijos");
                        OnPropertyChanged("IsCheckedAlergiHijos");
                        OnPropertyChanged("IsCheckedHipertHijos");
                    }
                }

                OnPropertyChanged("CheckHijosVive");
                OnPropertyChanged("TextEdadesHijos");
                OnPropertyChanged("IsMuertoHijos");
                OnPropertyChanged("HijosEnabled");
            }
        }
        private string _CheckHijosPadece;
        public string CheckHijosPadece
        {
            get { return _CheckHijosPadece; }
            set
            {
                _CheckHijosPadece = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                        IsEnabledPadecimientosHijos = true;
                    else
                        IsEnabledPadecimientosHijos = false;
                }

                OnPropertyChanged("CheckHijosPadece");
                OnPropertyChanged("IsEnabledPadecimientosHijos");
            }
        }
        private short? _TextEdadesHijos;
        public short? TextEdadesHijos
        {
            get { return _TextEdadesHijos; }
            set { _TextEdadesHijos = value; OnPropertyChanged("TextEdadesHijos"); }
        }
        private bool _CheckDiabetes;
        public bool CheckDiabetes
        {
            get { return _CheckDiabetes; }
            set { _CheckDiabetes = value; OnPropertyChanged("CheckDiabetes"); }
        }
        private bool _CheckTB;
        public bool CheckTB
        {
            get { return _CheckTB; }
            set { _CheckTB = value; OnPropertyChanged("CheckTB"); }
        }
        private bool _CheckCA;
        public bool CheckCA
        {
            get { return _CheckCA; }
            set { _CheckCA = value; OnPropertyChanged("CheckCA"); }
        }
        private bool _CheckCardiacos;
        public bool CheckCardiacos
        {
            get { return _CheckCardiacos; }
            set { _CheckCardiacos = value; OnPropertyChanged("CheckCardiacos"); }
        }
        private bool _CheckEpilepsia;
        public bool CheckEpilepsia
        {
            get { return _CheckEpilepsia; }
            set { _CheckEpilepsia = value; OnPropertyChanged("CheckEpilepsia"); }
        }
        private bool _CheckMentales;
        public bool CheckMentales
        {
            get { return _CheckMentales; }
            set { _CheckMentales = value; OnPropertyChanged("CheckMentales"); }
        }
        private bool _CheckAlergias;
        public bool CheckAlergias
        {
            get { return _CheckAlergias; }
            set { _CheckAlergias = value; OnPropertyChanged("CheckAlergias"); }
        }
        private bool _CheckHipertensivo;
        public bool CheckHipertensivo
        {
            get { return _CheckHipertensivo; }
            set { _CheckHipertensivo = value; OnPropertyChanged("CheckHipertensivo"); }
        }


        private string _CausaMuertePadre;

        public string CausaMuertePadre
        {
            get { return _CausaMuertePadre; }
            set { _CausaMuertePadre = value; OnPropertyChanged("CausaMuertePadre"); }
        }
        private DateTime? _CuandoMuertePadre;

        public DateTime? CuandoMuertePadre
        {
            get { return _CuandoMuertePadre; }
            set { _CuandoMuertePadre = value; OnPropertyChanged("CuandoMuertePadre"); }
        }

        private DateTime _MaximaFechaMuerte = Fechas.GetFechaDateServer;

        public DateTime MaximaFechaMuerte
        {
            get { return _MaximaFechaMuerte; }
            set { _MaximaFechaMuerte = value; OnPropertyChanged("MaximaFechaMuerte"); }
        }
        private string _IsCheckedDiabPadre;

        public string IsCheckedDiabPadre
        {
            get { return _IsCheckedDiabPadre; }
            set { _IsCheckedDiabPadre = value; OnPropertyChanged("IsCheckedDiabPadre"); }
        }
        private string _IsCheckedTBPadre;

        public string IsCheckedTBPadre
        {
            get { return _IsCheckedTBPadre; }
            set { _IsCheckedTBPadre = value; OnPropertyChanged("IsCheckedTBPadre"); }
        }
        private string _IsCheckedCAPadre;

        public string IsCheckedCAPadre
        {
            get { return _IsCheckedCAPadre; }
            set { _IsCheckedCAPadre = value; OnPropertyChanged("IsCheckedCAPadre"); }
        }
        private string _IsCheckedCardiPadre;

        public string IsCheckedCardiPadre
        {
            get { return _IsCheckedCardiPadre; }
            set { _IsCheckedCardiPadre = value; OnPropertyChanged("IsCheckedCardiPadre"); }
        }
        private string _IsCheckedEpiPadre;

        public string IsCheckedEpiPadre
        {
            get { return _IsCheckedEpiPadre; }
            set { _IsCheckedEpiPadre = value; OnPropertyChanged("IsCheckedEpiPadre"); }
        }
        private string _IsCheckedMentPadre;

        public string IsCheckedMentPadre
        {
            get { return _IsCheckedMentPadre; }
            set { _IsCheckedMentPadre = value; OnPropertyChanged("IsCheckedMentPadre"); }
        }
        private string _IsCheckedAlergiPadre;

        public string IsCheckedAlergiPadre
        {
            get { return _IsCheckedAlergiPadre; }
            set { _IsCheckedAlergiPadre = value; OnPropertyChanged("IsCheckedAlergiPadre"); }
        }
        private string _IsCheckedHipertPadre;

        public string IsCheckedHipertPadre
        {
            get { return _IsCheckedHipertPadre; }
            set { _IsCheckedHipertPadre = value; OnPropertyChanged("IsCheckedHipertPadre"); }
        }




        private string _CausaMuerteMadre;

        public string CausaMuerteMadre
        {
            get { return _CausaMuerteMadre; }
            set { _CausaMuerteMadre = value; OnPropertyChanged("CausaMuerteMadre"); }
        }
        private DateTime? _CuandoMuerteMadre;

        public DateTime? CuandoMuerteMadre
        {
            get { return _CuandoMuerteMadre; }
            set { _CuandoMuerteMadre = value; OnPropertyChanged("CuandoMuerteMadre"); }
        }
        private string _IsCheckedDiabMadre;

        public string IsCheckedDiabMadre
        {
            get { return _IsCheckedDiabMadre; }
            set { _IsCheckedDiabMadre = value; OnPropertyChanged("IsCheckedDiabMadre"); }
        }
        private string _IsCheckedTBMadre;

        public string IsCheckedTBMadre
        {
            get { return _IsCheckedTBMadre; }
            set { _IsCheckedTBMadre = value; OnPropertyChanged("IsCheckedTBMadre"); }
        }
        private string _IsCheckedCAMadre;

        public string IsCheckedCAMadre
        {
            get { return _IsCheckedCAMadre; }
            set { _IsCheckedCAMadre = value; OnPropertyChanged("IsCheckedCAMadre"); }
        }
        private string _IsCheckedCardiMadre;

        public string IsCheckedCardiMadre
        {
            get { return _IsCheckedCardiMadre; }
            set { _IsCheckedCardiMadre = value; OnPropertyChanged("IsCheckedCardiMadre"); }
        }
        private string _IsCheckedEpiMadre;

        public string IsCheckedEpiMadre
        {
            get { return _IsCheckedEpiMadre; }
            set { _IsCheckedEpiMadre = value; OnPropertyChanged("IsCheckedEpiMadre"); }
        }
        private string _IsCheckedMentMadre;

        public string IsCheckedMentMadre
        {
            get { return _IsCheckedMentMadre; }
            set { _IsCheckedMentMadre = value; OnPropertyChanged("IsCheckedMentMadre"); }
        }
        private string _IsCheckedAlergiMadre;

        public string IsCheckedAlergiMadre
        {
            get { return _IsCheckedAlergiMadre; }
            set { _IsCheckedAlergiMadre = value; OnPropertyChanged("IsCheckedAlergiMadre"); }
        }
        private string _IsCheckedHipertMadre;

        public string IsCheckedHipertMadre
        {
            get { return _IsCheckedHipertMadre; }
            set { _IsCheckedHipertMadre = value; OnPropertyChanged("IsCheckedHipertMadre"); }
        }




        private string _CausaMuerteHnos;

        public string CausaMuerteHnos
        {
            get { return _CausaMuerteHnos; }
            set { _CausaMuerteHnos = value; OnPropertyChanged("CausaMuerteHnos"); }
        }
        private DateTime? _CuandoMuerteHnos;

        public DateTime? CuandoMuerteHnos
        {
            get { return _CuandoMuerteHnos; }
            set { _CuandoMuerteHnos = value; OnPropertyChanged("CuandoMuerteHnos"); }
        }
        private string _IsCheckedDiabHnos;

        public string IsCheckedDiabHnos
        {
            get { return _IsCheckedDiabHnos; }
            set { _IsCheckedDiabHnos = value; OnPropertyChanged("IsCheckedDiabHnos"); }
        }
        private string _IsCheckedTBHnos;

        public string IsCheckedTBHnos
        {
            get { return _IsCheckedTBHnos; }
            set { _IsCheckedTBHnos = value; OnPropertyChanged("IsCheckedTBHnos"); }
        }
        private string _IsCheckedCAHnos;

        public string IsCheckedCAHnos
        {
            get { return _IsCheckedCAHnos; }
            set { _IsCheckedCAHnos = value; OnPropertyChanged("IsCheckedCAHnos"); }
        }
        private string _IsCheckedCardiHnos;

        public string IsCheckedCardiHnos
        {
            get { return _IsCheckedCardiHnos; }
            set { _IsCheckedCardiHnos = value; OnPropertyChanged("IsCheckedCardiHnos"); }
        }
        private string _IsCheckedEpiHnos;

        public string IsCheckedEpiHnos
        {
            get { return _IsCheckedEpiHnos; }
            set { _IsCheckedEpiHnos = value; OnPropertyChanged("IsCheckedEpiHnos"); }
        }
        private string _IsCheckedMentHnos;

        public string IsCheckedMentHnos
        {
            get { return _IsCheckedMentHnos; }
            set { _IsCheckedMentHnos = value; OnPropertyChanged("IsCheckedMentHnos"); }
        }
        private string _IsCheckedAlergiHnos;

        public string IsCheckedAlergiHnos
        {
            get { return _IsCheckedAlergiHnos; }
            set { _IsCheckedAlergiHnos = value; OnPropertyChanged("IsCheckedAlergiHnos"); }
        }
        private string _IsCheckedHipertHnos;

        public string IsCheckedHipertHnos
        {
            get { return _IsCheckedHipertHnos; }
            set { _IsCheckedHipertHnos = value; OnPropertyChanged("IsCheckedHipertHnos"); }
        }


        private string _CausaMuerteCony;

        public string CausaMuerteCony
        {
            get { return _CausaMuerteCony; }
            set { _CausaMuerteCony = value; OnPropertyChanged("CausaMuerteCony"); }
        }
        private DateTime? _CuandoMuerteCony;

        public DateTime? CuandoMuerteCony
        {
            get { return _CuandoMuerteCony; }
            set { _CuandoMuerteCony = value; OnPropertyChanged("CuandoMuerteCony"); }
        }
        private string _IsCheckedDiabCony;

        public string IsCheckedDiabCony
        {
            get { return _IsCheckedDiabCony; }
            set { _IsCheckedDiabCony = value; OnPropertyChanged("IsCheckedDiabCony"); }
        }
        private string _IsCheckedTBCony;

        public string IsCheckedTBCony
        {
            get { return _IsCheckedTBCony; }
            set { _IsCheckedTBCony = value; OnPropertyChanged("IsCheckedTBCony"); }
        }
        private string _IsCheckedCACony;

        public string IsCheckedCACony
        {
            get { return _IsCheckedCACony; }
            set { _IsCheckedCACony = value; OnPropertyChanged("IsCheckedCACony"); }
        }
        private string _IsCheckedCardiCony;

        public string IsCheckedCardiCony
        {
            get { return _IsCheckedCardiCony; }
            set { _IsCheckedCardiCony = value; OnPropertyChanged("IsCheckedCardiCony"); }
        }
        private string _IsCheckedEpiCony;

        public string IsCheckedEpiCony
        {
            get { return _IsCheckedEpiCony; }
            set { _IsCheckedEpiCony = value; OnPropertyChanged("IsCheckedEpiCony"); }
        }
        private string _IsCheckedMentCony;

        public string IsCheckedMentCony
        {
            get { return _IsCheckedMentCony; }
            set { _IsCheckedMentCony = value; OnPropertyChanged("IsCheckedMentCony"); }
        }
        private string _IsCheckedAlergiCony;

        public string IsCheckedAlergiCony
        {
            get { return _IsCheckedAlergiCony; }
            set { _IsCheckedAlergiCony = value; OnPropertyChanged("IsCheckedAlergiCony"); }
        }
        private string _IsCheckedHipertCony;

        public string IsCheckedHipertCony
        {
            get { return _IsCheckedHipertCony; }
            set { _IsCheckedHipertCony = value; OnPropertyChanged("IsCheckedHipertCony"); }
        }



        private string _CausaMuerteHijos;

        public string CausaMuerteHijos
        {
            get { return _CausaMuerteHijos; }
            set { _CausaMuerteHijos = value; OnPropertyChanged("CausaMuerteHijos"); }
        }
        private DateTime? _CuandoMuerteHijos;

        public DateTime? CuandoMuerteHijos
        {
            get { return _CuandoMuerteHijos; }
            set { _CuandoMuerteHijos = value; OnPropertyChanged("CuandoMuerteHijos"); }
        }

        private string _IsCheckedDiabHijos;

        public string IsCheckedDiabHijos
        {
            get { return _IsCheckedDiabHijos; }
            set { _IsCheckedDiabHijos = value; OnPropertyChanged("IsCheckedDiabHijos"); }
        }
        private string _IsCheckedTBHijos;

        public string IsCheckedTBHijos
        {
            get { return _IsCheckedTBHijos; }
            set { _IsCheckedTBHijos = value; OnPropertyChanged("IsCheckedTBHijos"); }
        }
        private string _IsCheckedCAHijos;

        public string IsCheckedCAHijos
        {
            get { return _IsCheckedCAHijos; }
            set { _IsCheckedCAHijos = value; OnPropertyChanged("IsCheckedCAHijos"); }
        }
        private string _IsCheckedCardiHijos;

        public string IsCheckedCardiHijos
        {
            get { return _IsCheckedCardiHijos; }
            set { _IsCheckedCardiHijos = value; OnPropertyChanged("IsCheckedCardiHijos"); }
        }
        private string _IsCheckedEpiHijos;

        public string IsCheckedEpiHijos
        {
            get { return _IsCheckedEpiHijos; }
            set { _IsCheckedEpiHijos = value; OnPropertyChanged("IsCheckedEpiHijos"); }
        }
        private string _IsCheckedMentHijos;

        public string IsCheckedMentHijos
        {
            get { return _IsCheckedMentHijos; }
            set { _IsCheckedMentHijos = value; OnPropertyChanged("IsCheckedMentHijos"); }
        }
        private string _IsCheckedAlergiHijos;

        public string IsCheckedAlergiHijos
        {
            get { return _IsCheckedAlergiHijos; }
            set { _IsCheckedAlergiHijos = value; OnPropertyChanged("IsCheckedAlergiHijos"); }
        }
        private string _IsCheckedHipertHijos;

        public string IsCheckedHipertHijos
        {
            get { return _IsCheckedHipertHijos; }
            set { _IsCheckedHipertHijos = value; OnPropertyChanged("IsCheckedHipertHijos"); }
        }
        #endregion

        #region ANP
        private string _TextNacimientoNoPatologicos;
        public string TextNacimientoNoPatologicos
        {
            get { return _TextNacimientoNoPatologicos; }
            set { _TextNacimientoNoPatologicos = value; OnPropertyChanged("TextNacimientoNoPatologicos"); }
        }
        private string _TextAlimentacionNoPatologicos;
        public string TextAlimentacionNoPatologicos
        {
            get { return _TextAlimentacionNoPatologicos; }
            set { _TextAlimentacionNoPatologicos = value; OnPropertyChanged("TextAlimentacionNoPatologicos"); }
        }
        private string _TextHabitacionNoPatologicos;
        public string TextHabitacionNoPatologicos
        {
            get { return _TextHabitacionNoPatologicos; }
            set { _TextHabitacionNoPatologicos = value; OnPropertyChanged("TextHabitacionNoPatologicos"); }
        }
        private string _TextAlcoholismoNoPatologicos;
        public string TextAlcoholismoNoPatologicos
        {
            get { return _TextAlcoholismoNoPatologicos; }
            set
            {
                _TextAlcoholismoNoPatologicos = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        ObservacionesAlcohlismo = !string.IsNullOrEmpty(ObservacionesAlcohlismo) ? ObservacionesAlcohlismo != LeyendaNoAplica ? ObservacionesAlcohlismo : string.Empty : string.Empty;
                        base.RemoveRule("ObservacionesAlcohlismo");
                        base.AddRule(() => ObservacionesAlcohlismo, () => !string.IsNullOrEmpty(ObservacionesAlcohlismo), "OBSERVACIONES ALCOHLISMO ES REQUERIDO!");
                        IsEnabledAlcohl = true;
                    }
                    else
                    {
                        IsEnabledAlcohl = true;
                        ObservacionesAlcohlismo = string.Empty;
                        ObservacionesAlcohlismo = LeyendaNoAplica;
                        base.RemoveRule("ObservacionesAlcohlismo");
                        base.AddRule(() => ObservacionesAlcohlismo, () => !string.IsNullOrEmpty(ObservacionesAlcohlismo), "OBSERVACIONES ALCOHLISMO ES REQUERIDO!");
                    }
                };

                OnPropertyChanged("IsEnabledAlcohl");
                OnPropertyChanged("ObservacionesAlcohlismo");
                OnPropertyChanged("TextAlcoholismoNoPatologicos");
            }
        }

        private bool _IsEnabledAlcohl = false;

        public bool IsEnabledAlcohl
        {
            get { return _IsEnabledAlcohl; }
            set { _IsEnabledAlcohl = value; OnPropertyChanged("IsEnabledAlcohl"); }
        }

        private string _TextTabaquismoNoPatologicos;
        public string TextTabaquismoNoPatologicos
        {
            get { return _TextTabaquismoNoPatologicos; }
            set
            {
                _TextTabaquismoNoPatologicos = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        ObservacionesTabaquismo = !string.IsNullOrEmpty(ObservacionesTabaquismo) ? ObservacionesTabaquismo != LeyendaNoAplica ? ObservacionesTabaquismo : string.Empty : string.Empty;
                        IsEnabledTabaq = true;
                        base.RemoveRule("ObservacionesTabaquismo");
                        base.AddRule(() => ObservacionesTabaquismo, () => !string.IsNullOrEmpty(ObservacionesTabaquismo), "OBSERVACIONES TABAQUISMO ES REQUERIDO!");
                    }

                    else
                    {
                        IsEnabledTabaq = true;
                        ObservacionesTabaquismo = string.Empty;
                        ObservacionesTabaquismo = LeyendaNoAplica;
                        base.RemoveRule("ObservacionesTabaquismo");
                        base.AddRule(() => ObservacionesTabaquismo, () => !string.IsNullOrEmpty(ObservacionesTabaquismo), "OBSERVACIONES TABAQUISMO ES REQUERIDO!");
                    }
                };

                OnPropertyChanged("ObservacionesTabaquismo");
                OnPropertyChanged("IsEnabledTabaq");
                OnPropertyChanged("TextTabaquismoNoPatologicos");
            }
        }

        private bool _IsEnabledTabaq = false;

        public bool IsEnabledTabaq
        {
            get { return _IsEnabledTabaq; }
            set { _IsEnabledTabaq = value; OnPropertyChanged("IsEnabledTabaq"); }
        }

        private string _TextToxicomaniasNoPatologicos;
        public string TextToxicomaniasNoPatologicos
        {
            get { return _TextToxicomaniasNoPatologicos; }
            set
            {
                _TextToxicomaniasNoPatologicos = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        ObservacionesToxicomanias = !string.IsNullOrEmpty(ObservacionesToxicomanias) ? ObservacionesToxicomanias != LeyendaNoAplica ? ObservacionesToxicomanias : string.Empty : string.Empty;
                        IsEnabledToxicoma = true;
                        base.RemoveRule("ObservacionesToxicomanias");
                        base.AddRule(() => ObservacionesToxicomanias, () => !string.IsNullOrEmpty(ObservacionesToxicomanias), "OBSERVACIONES TOXICOMANIAS ES REQUERIDO!");
                    }

                    else
                    {
                        IsEnabledToxicoma = true;
                        ObservacionesToxicomanias = string.Empty;
                        ObservacionesToxicomanias = LeyendaNoAplica;
                        base.RemoveRule("ObservacionesToxicomanias");
                        base.AddRule(() => ObservacionesToxicomanias, () => !string.IsNullOrEmpty(ObservacionesToxicomanias), "OBSERVACIONES TOXICOMANIAS ES REQUERIDO!");
                    }
                };

                OnPropertyChanged("TextToxicomaniasNoPatologicos");
                OnPropertyChanged("ObservacionesToxicomanias");
                OnPropertyChanged("IsEnabledToxicoma");
            }
        }

        private bool _IsEnabledToxicoma = false;

        public bool IsEnabledToxicoma
        {
            get { return _IsEnabledToxicoma; }
            set { _IsEnabledToxicoma = value; OnPropertyChanged("IsEnabledToxicoma"); }
        }
        #endregion

        #region ANT_PATOLOGICOS
        private string _TextSarampion;
        public string TextSarampion
        {
            get { return _TextSarampion; }
            set { _TextSarampion = value; OnPropertyChanged("TextSarampion"); }
        }
        private string _TextViruela;
        public string TextViruela
        {
            get { return _TextViruela; }
            set { _TextViruela = value; OnPropertyChanged("TextViruela"); }
        }
        private string _TextRubeola;
        public string TextRubeola
        {
            get { return _TextRubeola; }
            set { _TextRubeola = value; OnPropertyChanged("TextRubeola"); }
        }
        private string _TextLueticos;
        public string TextLueticos
        {
            get { return _TextLueticos; }
            set { _TextLueticos = value; OnPropertyChanged("TextLueticos"); }
        }
        private string _TextFimicos;
        public string TextFimicos
        {
            get { return _TextFimicos; }
            set { _TextFimicos = value; OnPropertyChanged("TextFimicos"); }
        }
        private string _TextAlergicos;
        public string TextAlergicos
        {
            get { return _TextAlergicos; }
            set { _TextAlergicos = value; OnPropertyChanged("TextAlergicos"); }
        }
        private string _TextTraumaticos;
        public string TextTraumaticos
        {
            get { return _TextTraumaticos; }
            set { _TextTraumaticos = value; OnPropertyChanged("TextTraumaticos"); }
        }
        private string _TextQuirurgicos;
        public string TextQuirurgicos
        {
            get { return _TextQuirurgicos; }
            set { _TextQuirurgicos = value; OnPropertyChanged("TextQuirurgicos"); }
        }
        private string _TextPaludicos;
        public string TextPaludicos
        {
            get { return _TextPaludicos; }
            set { _TextPaludicos = value; OnPropertyChanged("TextPaludicos"); }
        }
        private string _TextTosferina;
        public string TextTosferina
        {
            get { return _TextTosferina; }
            set { _TextTosferina = value; OnPropertyChanged("TextTosferina"); }
        }
        private string _TextParotiditis;
        public string TextParotiditis
        {
            get { return _TextParotiditis; }
            set { _TextParotiditis = value; OnPropertyChanged("TextParotiditis"); }
        }
        private string _TextAmigdalitis;
        public string TextAmigdalitis
        {
            get { return _TextAmigdalitis; }
            set { _TextAmigdalitis = value; OnPropertyChanged("TextAmigdalitis"); }
        }
        private string _TextParasitarios;
        public string TextParasitarios
        {
            get { return _TextParasitarios; }
            set { _TextParasitarios = value; OnPropertyChanged("TextParasitarios"); }
        }
        private string _TextTransfusionales;
        public string TextTransfusionales
        {
            get { return _TextTransfusionales; }
            set { _TextTransfusionales = value; OnPropertyChanged("TextTransfusionales"); }
        }
        private string _TextIntolerancias;
        public string TextIntolerancias
        {
            get { return _TextIntolerancias; }
            set { _TextIntolerancias = value; OnPropertyChanged("TextIntolerancias"); }
        }

        #endregion

        #region MUJERES

        private DateTime? _FechaProbParto;

        public DateTime? FechaProbParto
        {
            get { return _FechaProbParto; }
            set { _FechaProbParto = value; OnPropertyChanged("FechaProbParto"); }
        }
        private bool _MujeresEnabled;
        public bool MujeresEnabled
        {
            get { return _MujeresEnabled; }
            set { _MujeresEnabled = value; OnPropertyChanged("MujeresEnabled"); }
        }
        private short? _CheckMenarquia;
        public short? CheckMenarquia
        {
            get { return _CheckMenarquia; }
            set { _CheckMenarquia = value; OnPropertyChanged("CheckMenarquia"); }
        }
        private string _TextAniosRitmo;
        public string TextAniosRitmo
        {
            get { return _TextAniosRitmo; }
            set { _TextAniosRitmo = value; OnPropertyChanged("TextAniosRitmo"); }
        }
        private short? _TextEmbarazos;
        public short? TextEmbarazos
        {
            get { return _TextEmbarazos; }
            set { _TextEmbarazos = value; OnPropertyChanged("TextEmbarazos"); }
        }
        private short? _TextPartos;
        public short? TextPartos
        {
            get { return _TextPartos; }
            set { _TextPartos = value; OnPropertyChanged("TextPartos"); }
        }
        private short? _TextAbortos;
        public short? TextAbortos
        {
            get { return _TextAbortos; }
            set { _TextAbortos = value; OnPropertyChanged("TextAbortos"); }
        }
        private short? _TextCesareas;
        public short? TextCesareas
        {
            get { return _TextCesareas; }
            set { _TextCesareas = value; OnPropertyChanged("TextCesareas"); }
        }
        private DateTime? _FechaUltimaMenstruacion;
        public DateTime? FechaUltimaMenstruacion
        {
            get { return _FechaUltimaMenstruacion; }
            set { _FechaUltimaMenstruacion = value; OnPropertyChanged("FechaUltimaMenstruacion"); }
        }
        private string _TextDeformacionesOrganicas;
        public string TextDeformacionesOrganicas
        {
            get { return _TextDeformacionesOrganicas; }
            set { _TextDeformacionesOrganicas = value; OnPropertyChanged("TextDeformacionesOrganicas"); }
        }
        private string _TextIntegridadFisica;
        public string TextIntegridadFisica
        {
            get { return _TextIntegridadFisica; }
            set { _TextIntegridadFisica = value; OnPropertyChanged("TextIntegridadFisica"); }
        }

        private enum eSINO
        {
            SI = 0,
            NO = 1
        };

        private enum eVulnerablesActivos
        {
            SI = 1
        };
        private string _IdControlPreN;
        public string IdControlPreN
        {
            get { return _IdControlPreN; }
            set
            {
                _IdControlPreN = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        base.RemoveRule("IdSelectedControlP");
                        base.RemoveRule("FechaProbParto");
                        IsEnabledControlP = false;
                        base.AddRule(() => FechaProbParto, () => FechaProbParto.HasValue, "FECHA PROBABLE DE PARTO ES REQUERIDO!");
                    }
                    if (value == "N")
                    {
                        base.RemoveRule("IdSelectedControlP");
                        base.RemoveRule("FechaProbParto");
                        IsEnabledControlP = true;
                        base.AddRule(() => IdSelectedControlP, () => IdSelectedControlP.HasValue ? IdSelectedControlP.Value != -1 : false, "CONTROL PRENATAL ES REQUERIDO!");
                    }
                };

                OnPropertyChanged("FechaProbParto");
                OnPropertyChanged("IdSelectedControlP");
                OnPropertyChanged("IsEnabledControlP");
                OnPropertyChanged("IdControlPreN");
            }
        }

        private bool _IsEnabledControlP = false;

        public bool IsEnabledControlP
        {
            get { return _IsEnabledControlP; }
            set { _IsEnabledControlP = value; OnPropertyChanged("IsEnabledControlP"); }
        }


        private ObservableCollection<CONTROL_PRENATAL> lstControlPrenatal;

        public ObservableCollection<CONTROL_PRENATAL> LstControlPrenatal
        {
            get { return lstControlPrenatal; }
            set { lstControlPrenatal = value; OnPropertyChanged("LstControlPrenatal"); }
        }

        private short? idSelectedControlP;

        public short? IdSelectedControlP
        {
            get { return idSelectedControlP; }
            set { idSelectedControlP = value; OnPropertyChanged("IdSelectedControlP"); }
        }


        #endregion

        #region EXPLORACION FISICA
        private string _TextPeso;
        public string TextPeso
        {
            get { return _TextPeso; }
            set { _TextPeso = value; OnPropertyChanged("TextPeso"); }
        }
        private string _TextEstatura;
        public string TextEstatura
        {
            get { return _TextEstatura; }
            set { _TextEstatura = value; OnPropertyChanged("TextEstatura"); }
        }
        private string _TextCabeza;
        public string TextCabeza
        {
            get { return _TextCabeza; }
            set { _TextCabeza = value; OnPropertyChanged("TextCabeza"); }
        }
        private string _TextCuello;
        public string TextCuello
        {
            get { return _TextCuello; }
            set
            {

                _TextCuello = value;
                OnPropertyChanged("TextCuello");
            }
        }
        private string _TextTorax;
        public string TextTorax
        {
            get { return _TextTorax; }
            set
            {
                _TextTorax = value;
                OnPropertyChanged("TextTorax");
            }
        }
        private string _TextAbdomen;
        public string TextAbdomen
        {
            get { return _TextAbdomen; }
            set
            {
                _TextAbdomen = value;
                OnPropertyChanged("TextAbdomen");
            }
        }
        private string _TextRecto;
        public string TextRecto
        {
            get { return _TextRecto; }
            set
            {
                _TextRecto = value;
                OnPropertyChanged("TextRecto");
            }
        }
        private string _TextGenitales;
        public string TextGenitales
        {
            get { return _TextGenitales; }
            set
            {
                _TextGenitales = value;
                OnPropertyChanged("TextGenitales");
            }
        }
        private string _TextExtremidades;
        public string TextExtremidades
        {
            get { return _TextExtremidades; }
            set
            {
                _TextExtremidades = value;
                OnPropertyChanged("TextExtremidades");
            }
        }
        private string _TextPresionArterial;
        public string TextPresionArterial
        {
            get { return _TextPresionArterial; }
            set { _TextPresionArterial = value; OnPropertyChanged("TextPresionArterial"); }
        }
        private string _TextPulso;
        public string TextPulso
        {
            get { return _TextPulso; }
            set { _TextPulso = value; OnPropertyChanged("TextPulso"); }
        }
        private string _TextRespiracion;
        public string TextRespiracion
        {
            get { return _TextRespiracion; }
            set { _TextRespiracion = value; OnPropertyChanged("TextRespiracion"); }
        }
        private string _TextTemperatura;
        public string TextTemperatura
        {
            get { return _TextTemperatura; }
            set { _TextTemperatura = value; OnPropertyChanged("TextTemperatura"); }
        }
        private string _TextResultadosAnalisisClinicos;
        public string TextResultadosAnalisisClinicos
        {
            get { return _TextResultadosAnalisisClinicos; }
            set
            {
                _TextResultadosAnalisisClinicos = value;
                OnPropertyChanged("TextResultadosAnalisisClinicos");
            }
        }
        private string _TextResultadosestudiosGabinete;
        public string TextResultadosestudiosGabinete
        {
            get { return _TextResultadosestudiosGabinete; }
            set
            {

                _TextResultadosestudiosGabinete = value;
                OnPropertyChanged("TextResultadosestudiosGabinete");
            }
        }
        private string _TextImpresionDiagnostica;
        public string TextImpresionDiagnostica
        {
            get { return _TextImpresionDiagnostica; }
            set
            {

                _TextImpresionDiagnostica = value;
                OnPropertyChanged("TextImpresionDiagnostica");
            }
        }
        #endregion

        #region Mayores 65
        private bool _CheckPresbicia;
        public bool CheckPresbicia
        {
            get { return _CheckPresbicia; }
            set { _CheckPresbicia = value; OnPropertyChanged("CheckPresbicia"); }
        }
        private bool _CheckCataratas;
        public bool CheckCataratas
        {
            get { return _CheckCataratas; }
            set { _CheckCataratas = value; OnPropertyChanged("CheckCataratas"); }
        }
        private bool _CheckPterigion;
        public bool CheckPterigion
        {
            get { return _CheckPterigion; }
            set { _CheckPterigion = value; OnPropertyChanged("CheckPterigion"); }
        }
        private bool _OtroDisminucionVisualEnabled;
        public bool OtroDisminucionVisualEnabled
        {
            get { return _OtroDisminucionVisualEnabled; }
            set { _OtroDisminucionVisualEnabled = value; OnPropertyChanged("OtroDisminucionVisualEnabled"); }
        }
        private bool _CheckOtros;
        public bool CheckOtros
        {
            get { return _CheckOtros; }
            set
            {
                _CheckOtros = value;
                OtroDisminucionVisualEnabled = value;
                OnPropertyChanged("CheckOtros");
            }
        }


        private string _TextOtroDisminucionVisual;
        public string TextOtroDisminucionVisual
        {
            get { return _TextOtroDisminucionVisual; }
            set { _TextOtroDisminucionVisual = value; OnPropertyChanged("TextOtroDisminucionVisual"); }
        }
        private Visibility _OtroDisminucionVisualVisible;
        public Visibility OtroDisminucionVisualVisible
        {
            get { return _OtroDisminucionVisualVisible; }
            set { _OtroDisminucionVisualVisible = value; OnPropertyChanged("OtroDisminucionVisualVisible"); }
        }
        private string _CheckAgudezaAuditiva;
        public string CheckAgudezaAuditiva
        {
            get { return _CheckAgudezaAuditiva; }
            set { _CheckAgudezaAuditiva = value; OnPropertyChanged("CheckAgudezaAuditiva"); }
        }
        private string _CheckOlfacion;
        public string CheckOlfacion
        {
            get { return _CheckOlfacion; }
            set { _CheckOlfacion = value; OnPropertyChanged("CheckOlfacion"); }
        }
        private string _CheckCapacidadVisomotriz;
        public string CheckCapacidadVisomotriz
        {
            get { return _CheckCapacidadVisomotriz; }
            set { _CheckCapacidadVisomotriz = value; OnPropertyChanged("CheckCapacidadVisomotriz"); }
        }
        private bool _CheckMemoriaNinguno;
        public bool CheckMemoriaNinguno
        {
            get { return _CheckMemoriaNinguno; }
            set { _CheckMemoriaNinguno = value; OnPropertyChanged("CheckMemoriaNinguno"); }
        }
        private bool _CheckMemoriaSuperficiales;
        public bool CheckMemoriaSuperficiales
        {
            get { return _CheckMemoriaSuperficiales; }
            set { _CheckMemoriaSuperficiales = value; OnPropertyChanged("CheckMemoriaSuperficiales"); }
        }
        private bool _CheckMemoriaModerados;
        public bool CheckMemoriaModerados
        {
            get { return _CheckMemoriaModerados; }
            set { _CheckMemoriaModerados = value; OnPropertyChanged("CheckMemoriaModerados"); }
        }
        private bool _CheckMemoriaGraves;
        public bool CheckMemoriaGraves
        {
            get { return _CheckMemoriaGraves; }
            set { _CheckMemoriaGraves = value; OnPropertyChanged("CheckMemoriaGraves"); }
        }
        private bool _CheckAtencionNinguno;
        public bool CheckAtencionNinguno
        {
            get { return _CheckAtencionNinguno; }
            set { _CheckAtencionNinguno = value; OnPropertyChanged("CheckAtencionNinguno"); }
        }
        private bool _CheckAtencionSuperficiales;
        public bool CheckAtencionSuperficiales
        {
            get { return _CheckAtencionSuperficiales; }
            set { _CheckAtencionSuperficiales = value; OnPropertyChanged("CheckAtencionSuperficiales"); }
        }
        private bool _CheckAtencionModerados;
        public bool CheckAtencionModerados
        {
            get { return _CheckAtencionModerados; }
            set { _CheckAtencionModerados = value; OnPropertyChanged("CheckAtencionModerados"); }
        }
        private bool _CheckAtencionGraves;
        public bool CheckAtencionGraves
        {
            get { return _CheckAtencionGraves; }
            set { _CheckAtencionGraves = value; OnPropertyChanged("CheckAtencionGraves"); }
        }
        private bool _CheckComprensionNinguno;
        public bool CheckComprensionNinguno
        {
            get { return _CheckComprensionNinguno; }
            set { _CheckComprensionNinguno = value; OnPropertyChanged("CheckComprensionNinguno"); }
        }
        private bool _CheckComprensionSuperficiales;
        public bool CheckComprensionSuperficiales
        {
            get { return _CheckComprensionSuperficiales; }
            set { _CheckComprensionSuperficiales = value; OnPropertyChanged("CheckComprensionSuperficiales"); }
        }
        private bool _CheckComprensionModerados;
        public bool CheckComprensionModerados
        {
            get { return _CheckComprensionModerados; }
            set { _CheckComprensionModerados = value; OnPropertyChanged("CheckComprensionModerados"); }
        }
        private bool _CheckComprensionGraves;
        public bool CheckComprensionGraves
        {
            get { return _CheckComprensionGraves; }
            set { _CheckComprensionGraves = value; OnPropertyChanged("CheckComprensionGraves"); }
        }
        private bool _CheckOrientacionNinguno;
        public bool CheckOrientacionNinguno
        {
            get { return _CheckOrientacionNinguno; }
            set { _CheckOrientacionNinguno = value; OnPropertyChanged("CheckOrientacionNinguno"); }
        }
        private bool _CheckOrientacionSuperficiales;
        public bool CheckOrientacionSuperficiales
        {
            get { return _CheckOrientacionSuperficiales; }
            set { _CheckOrientacionSuperficiales = value; OnPropertyChanged("CheckOrientacionSuperficiales"); }
        }
        private bool _CheckOrientacionModerados;
        public bool CheckOrientacionModerados
        {
            get { return _CheckOrientacionModerados; }
            set { _CheckOrientacionModerados = value; OnPropertyChanged("CheckOrientacionModerados"); }
        }
        private bool _CheckOrientacionGraves;
        public bool CheckOrientacionGraves
        {
            get { return _CheckOrientacionGraves; }
            set { _CheckOrientacionGraves = value; OnPropertyChanged("CheckOrientacionGraves"); }
        }
        private string _CheckDemencial;
        public string CheckDemencial
        {
            get { return _CheckDemencial; }
            set { _CheckDemencial = value; OnPropertyChanged("CheckDemencial"); }
        }
        private bool _CheckEstadoAnimoDisforia;
        public bool CheckEstadoAnimoDisforia
        {
            get { return _CheckEstadoAnimoDisforia; }
            set { _CheckEstadoAnimoDisforia = value; OnPropertyChanged("CheckEstadoAnimoDisforia"); }
        }
        private bool _CheckEstadoAnimoIncontinencia;
        public bool CheckEstadoAnimoIncontinencia
        {
            get { return _CheckEstadoAnimoIncontinencia; }
            set { _CheckEstadoAnimoIncontinencia = value; OnPropertyChanged("CheckEstadoAnimoIncontinencia"); }
        }
        private bool _CheckEstadoAnimoDepresion;
        public bool CheckEstadoAnimoDepresion
        {
            get { return _CheckEstadoAnimoDepresion; }
            set { _CheckEstadoAnimoDepresion = value; OnPropertyChanged("CheckEstadoAnimoDepresion"); }
        }
        private bool _CheckReadaptacionIntegra;
        public bool CheckReadaptacionIntegra
        {
            get { return _CheckReadaptacionIntegra; }
            set { _CheckReadaptacionIntegra = value; OnPropertyChanged("CheckReadaptacionIntegra"); }
        }
        private bool _CheckReadaptacionLimitada;
        public bool CheckReadaptacionLimitada
        {
            get { return _CheckReadaptacionLimitada; }
            set { _CheckReadaptacionLimitada = value; OnPropertyChanged("CheckReadaptacionLimitada"); }
        }
        private bool _CheckReadaptacionNula;
        public bool CheckReadaptacionNula
        {
            get { return _CheckReadaptacionNula; }
            set { _CheckReadaptacionNula = value; OnPropertyChanged("CheckReadaptacionNula"); }
        }

        private byte[] _ImagenPruebas = new Imagenes().getImagenPerson();

        public byte[] ImagenPruebas
        {
            get { return _ImagenPruebas; }
            set
            {
                _ImagenPruebas = value;
                OnPropertyChanged("ImagenPruebas");
            }
        }
        private short? _IdDisminucionVisua;

        public short? IdDisminucionVisua
        {
            get { return _IdDisminucionVisua; }
            set
            {
                _IdDisminucionVisua = value;
                if (_IdDisminucionVisua != null)
                {
                    if (value == 3)
                    {
                        base.RemoveRule("TextOtroDisminucionVisual");
                        base.AddRule(() => TextOtroDisminucionVisual, () => !string.IsNullOrEmpty(TextOtroDisminucionVisual), "OTROS ES REQUERIDO!");
                        OtroDisminucionVisualEnabled = true;
                    }
                    else
                    {
                        OtroDisminucionVisualEnabled = false;
                        base.RemoveRule("TextOtroDisminucionVisual");
                    }
                }

                OnPropertyChanged("TextOtroDisminucionVisual");
                OnPropertyChanged("IdDisminucionVisua");
            }
        }

        private short? _IdTranstornosMemoria;

        public short? IdTranstornosMemoria
        {
            get { return _IdTranstornosMemoria; }
            set { _IdTranstornosMemoria = value; OnPropertyChanged("IdTranstornosMemoria"); }
        }

        private short? _IdTranstornosAtencion;

        public short? IdTranstornosAtencion
        {
            get { return _IdTranstornosAtencion; }
            set { _IdTranstornosAtencion = value; OnPropertyChanged("IdTranstornosAtencion"); }
        }

        private short? _IdTranstornosComprension;

        public short? IdTranstornosComprension
        {
            get { return _IdTranstornosComprension; }
            set { _IdTranstornosComprension = value; OnPropertyChanged("IdTranstornosComprension"); }
        }

        private short? _IdTranstornosOrientacion;

        public short? IdTranstornosOrientacion
        {
            get { return _IdTranstornosOrientacion; }
            set { _IdTranstornosOrientacion = value; OnPropertyChanged("IdTranstornosOrientacion"); }
        }

        private short? _IdEstadoAnimo;

        public short? IdEstadoAnimo
        {
            get { return _IdEstadoAnimo; }
            set { _IdEstadoAnimo = value; OnPropertyChanged("IdEstadoAnimo"); }
        }

        private short? _IdCapacidadParticipacion;

        public short? IdCapacidadParticipacion
        {
            get { return _IdCapacidadParticipacion; }
            set { _IdCapacidadParticipacion = value; OnPropertyChanged("IdCapacidadParticipacion"); }
        }

        private short? _IdComplica;

        public short? IdComplica
        {
            get { return _IdComplica; }
            set { _IdComplica = value; OnPropertyChanged("IdComplica"); }
        }

        private short? _IdEtapaEvo;

        public short? IdEtapaEvo
        {
            get { return _IdEtapaEvo; }
            set { _IdEtapaEvo = value; OnPropertyChanged("IdEtapaEvo"); }
        }

        private short? _IdPosibRemis;

        public short? IdPosibRemis
        {
            get { return _IdPosibRemis; }
            set { _IdPosibRemis = value; OnPropertyChanged("IdPosibRemis"); }
        }

        private short? _IdCapacTrata;

        public short? IdCapacTrata
        {
            get { return _IdCapacTrata; }
            set { _IdCapacTrata = value; OnPropertyChanged("IdCapacTrata"); }
        }

        private short? _IdNivelReq;

        public short? IdNivelReq
        {
            get { return _IdNivelReq; }
            set { _IdNivelReq = value; OnPropertyChanged("IdNivelReq"); }
        }
        #endregion

        #region CONSIDERACIONES_FINALES
        private bool _CheckComplicacionesLeve;
        public bool CheckComplicacionesLeve
        {
            get { return _CheckComplicacionesLeve; }
            set { _CheckComplicacionesLeve = value; OnPropertyChanged("CheckComplicacionesLeve"); }
        }
        private bool _CheckComplicacionesModerada;
        public bool CheckComplicacionesModerada
        {
            get { return _CheckComplicacionesModerada; }
            set { _CheckComplicacionesModerada = value; OnPropertyChanged("CheckComplicacionesModerada"); }
        }
        private bool _CheckComplicacionesSevera;
        public bool CheckComplicacionesSevera
        {
            get { return _CheckComplicacionesSevera; }
            set { _CheckComplicacionesSevera = value; OnPropertyChanged("CheckComplicacionesSevera"); }
        }
        private bool _CheckEtapaEvolutivaInicial;
        public bool CheckEtapaEvolutivaInicial
        {
            get { return _CheckEtapaEvolutivaInicial; }
            set { _CheckEtapaEvolutivaInicial = value; OnPropertyChanged("CheckEtapaEvolutivaInicial"); }
        }
        private bool _CheckEtapaEvolutivaMedia;
        public bool CheckEtapaEvolutivaMedia
        {
            get { return _CheckEtapaEvolutivaMedia; }
            set { _CheckEtapaEvolutivaMedia = value; OnPropertyChanged("CheckEtapaEvolutivaMedia"); }
        }
        private bool _CheckEtapaEvolutivaTerminal;
        public bool CheckEtapaEvolutivaTerminal
        {
            get { return _CheckEtapaEvolutivaTerminal; }
            set { _CheckEtapaEvolutivaTerminal = value; OnPropertyChanged("CheckEtapaEvolutivaTerminal"); }
        }
        private bool _CheckRemisionReversible;
        public bool CheckRemisionReversible
        {
            get { return _CheckRemisionReversible; }
            set { _CheckRemisionReversible = value; OnPropertyChanged("CheckRemisionReversible"); }
        }
        private bool _CheckRemisionIrreversible;
        public bool CheckRemisionIrreversible
        {
            get { return _CheckRemisionIrreversible; }
            set { _CheckRemisionIrreversible = value; OnPropertyChanged("CheckRemisionIrreversible"); }
        }
        private bool _CheckCapacidadTratamientoSuficiente;
        public bool CheckCapacidadTratamientoSuficiente
        {
            get { return _CheckCapacidadTratamientoSuficiente; }
            set { _CheckCapacidadTratamientoSuficiente = value; OnPropertyChanged("CheckCapacidadTratamientoSuficiente"); }
        }
        private bool _CheckCapacidadTratamientoMediana;
        public bool CheckCapacidadTratamientoMediana
        {
            get { return _CheckCapacidadTratamientoMediana; }
            set { _CheckCapacidadTratamientoMediana = value; OnPropertyChanged("CheckCapacidadTratamientoMediana"); }
        }
        private bool _CheckCapacidadTratamientoEscasa;
        public bool CheckCapacidadTratamientoEscasa
        {
            get { return _CheckCapacidadTratamientoEscasa; }
            set { _CheckCapacidadTratamientoEscasa = value; OnPropertyChanged("CheckCapacidadTratamientoEscasa"); }
        }
        private bool _CheckCapacidadTratamientoNula;
        public bool CheckCapacidadTratamientoNula
        {
            get { return _CheckCapacidadTratamientoNula; }
            set { _CheckCapacidadTratamientoNula = value; OnPropertyChanged("CheckCapacidadTratamientoNula"); }
        }
        private bool _CheckAtencionNivel1;
        public bool CheckAtencionNivel1
        {
            get { return _CheckAtencionNivel1; }
            set { _CheckAtencionNivel1 = value; OnPropertyChanged("CheckAtencionNivel1"); }
        }
        private bool _CheckAtencionNivel2;
        public bool CheckAtencionNivel2
        {
            get { return _CheckAtencionNivel2; }
            set { _CheckAtencionNivel2 = value; OnPropertyChanged("CheckAtencionNivel2"); }
        }
        private bool _CheckAtencionNivel3;
        public bool CheckAtencionNivel3
        {
            get { return _CheckAtencionNivel3; }
            set { _CheckAtencionNivel3 = value; OnPropertyChanged("CheckAtencionNivel3"); }
        }
        private string _TextGradoAfectacion;
        public string TextGradoAfectacion
        {
            get { return _TextGradoAfectacion; }
            set { _TextGradoAfectacion = value; OnPropertyChanged("TextGradoAfectacion"); }
        }
        private string _TextPronostico;
        public string TextPronostico
        {
            get { return _TextPronostico; }
            set { _TextPronostico = value; OnPropertyChanged("TextPronostico"); }
        }
        #endregion

        #region Generales
        private string _TextConclusiones;
        public string TextConclusiones
        {
            get { return _TextConclusiones; }
            set { _TextConclusiones = value; OnPropertyChanged("TextConclusiones"); }
        }
        private string _TextPadecimientoActual;
        public string TextPadecimientoActual
        {
            get { return _TextPadecimientoActual; }
            set
            {
                _TextPadecimientoActual = value;
                OnPropertyChanged("TextPadecimientoActual");
            }
        }

        #region Aparatos y Sistemas
        private string _TextAparatosSistemas;
        public string TextAparatosSistemas
        {
            get { return _TextAparatosSistemas; }
            set { _TextAparatosSistemas = value; OnPropertyChanged("TextAparatosSistemas"); }
        }
        private string _TextRespiratorio;
        public string TextRespiratorio
        {
            get { return _TextRespiratorio; }
            set
            {
                _TextRespiratorio = value;
                OnPropertyChanged("TextRespiratorio");
            }
        }
        private string _TextCardiovascular;
        public string TextCardiovascular
        {
            get { return _TextCardiovascular; }
            set
            {
                _TextCardiovascular = value;
                OnPropertyChanged("TextCardiovascular");
            }
        }
        private string _TextDigestivo;
        public string TextDigestivo
        {
            get { return _TextDigestivo; }
            set
            {
                _TextDigestivo = value;
                OnPropertyChanged("TextDigestivo");
            }
        }
        private string _TextUrinario;
        public string TextUrinario
        {
            get { return _TextUrinario; }
            set
            {
                _TextUrinario = value;
                OnPropertyChanged("TextUrinario");
            }
        }
        private string _TextGenitalMujeres;
        public string TextGenitalMujeres
        {
            get { return _TextGenitalMujeres; }
            set
            {
                _TextGenitalMujeres = value;
                OnPropertyChanged("TextGenitalMujeres");
            }
        }
        private string _TextGenitalHombres;
        public string TextGenitalHombres
        {
            get { return _TextGenitalHombres; }
            set
            {
                _TextGenitalHombres = value;
                OnPropertyChanged("TextGenitalHombres");
            }
        }

        private string _TextGenital;

        public string TextGenital
        {
            get { return _TextGenital; }
            set { _TextGenital = value; OnPropertyChanged("TextGenital"); }
        }
        private string _TextEndocrino;
        public string TextEndocrino
        {
            get { return _TextEndocrino; }
            set
            {
                _TextEndocrino = value;
                OnPropertyChanged("TextEndocrino");
            }
        }

        private bool _CheckedRecuperado = false;
        public bool CheckedRecuperado
        {
            get { return _CheckedRecuperado; }
            set
            {
                _CheckedRecuperado = value;
                OnPropertyChanged("CheckedRecuperado");
            }
        }

        private string _TextMusculoEsqueletico;
        public string TextMusculoEsqueletico
        {
            get { return _TextMusculoEsqueletico; }
            set
            {
                _TextMusculoEsqueletico = value;
                OnPropertyChanged("TextMusculoEsqueletico");
            }
        }
        private string _TextHematicoLinfatico;
        public string TextHematicoLinfatico
        {
            get { return _TextHematicoLinfatico; }
            set
            {
                _TextHematicoLinfatico = value;
                OnPropertyChanged("TextHematicoLinfatico");
            }
        }
        private string _TextNervioso;
        public string TextNervioso
        {
            get { return _TextNervioso; }
            set
            {
                _TextNervioso = value;
                OnPropertyChanged("TextNervioso");
            }
        }
        private string _TextPielAnexos;
        public string TextPielAnexos
        {
            get { return _TextPielAnexos; }
            set
            {
                _TextPielAnexos = value;
                OnPropertyChanged("TextPielAnexos");
            }
        }
        private string _TextSintomasGenerales;
        public string TextSintomasGenerales
        {
            get { return _TextSintomasGenerales; }
            set
            {
                _TextSintomasGenerales = value;
                OnPropertyChanged("TextSintomasGenerales");
            }
        }
        private string _TextTerapeuticaPrevia;
        public string TextTerapeuticaPrevia
        {
            get { return _TextTerapeuticaPrevia; }
            set
            {
                _TextTerapeuticaPrevia = value;
                OnPropertyChanged("TextTerapeuticaPrevia");
            }
        }
        #endregion

        #region Exploracion Fisica
        private string _TextExploracionFisica;
        public string TextExploracionFisica
        {
            get { return _TextExploracionFisica; }
            set
            {
                _TextExploracionFisica = value;
                OnPropertyChanged("TextExploracionFisica");
            }
        }
        private string _TextEFCabeza;
        public string TextEFCabeza
        {
            get { return _TextEFCabeza; }
            set
            {
                _TextEFCabeza = value;
                OnPropertyChanged("TextEFCabeza");
            }
        }
        private string _TextEFCuello;
        public string TextEFCuello
        {
            get { return _TextEFCuello; }
            set
            {
                _TextEFCuello = value;
                OnPropertyChanged("TextEFCuello");
            }
        }
        private string _TextEFTorax;
        public string TextEFTorax
        {
            get { return _TextEFTorax; }
            set
            {
                _TextEFTorax = value;
                OnPropertyChanged("TextEFTorax");
            }
        }
        private string _TextEFAbdomen;
        public string TextEFAbdomen
        {
            get { return _TextEFAbdomen; }
            set
            {
                _TextEFAbdomen = value;
                OnPropertyChanged("TextEFAbdomen");
            }
        }
        private string _TextEFGenitales;
        public string TextEFGenitales
        {
            get { return _TextEFGenitales; }
            set
            {
                _TextEFGenitales = value;
                OnPropertyChanged("TextEFGenitales");
            }
        }
        private string _TextEFExtremidades;
        public string TextEFExtremidades
        {
            get { return _TextEFExtremidades; }
            set
            {
                _TextEFExtremidades = value;
                OnPropertyChanged("TextEFExtremidades");
            }
        }
        private string _TextEFPresionArterial;
        public string TextEFPresionArterial
        {
            get { return _TextEFPresionArterial; }
            set { _TextEFPresionArterial = value; OnPropertyChanged("TextEFPresionArterial"); }
        }
        private string _TextEFPulso;
        public string TextEFPulso
        {
            get { return _TextEFPulso; }
            set { _TextEFPulso = value; OnPropertyChanged("TextEFPulso"); }
        }
        private string _TextEFRespiracion;
        public string TextEFRespiracion
        {
            get { return _TextEFRespiracion; }
            set { _TextEFRespiracion = value; OnPropertyChanged("TextEFRespiracion"); }
        }
        private string _TextEFTemperatura;
        public string TextEFTemperatura
        {
            get { return _TextEFTemperatura; }
            set { _TextEFTemperatura = value; OnPropertyChanged("TextEFTemperatura"); }
        }
        private string _TextEFResultadoAnalisis;
        public string TextEFResultadoAnalisis
        {
            get { return _TextEFResultadoAnalisis; }
            set
            {
                _TextEFResultadoAnalisis = value;
                OnPropertyChanged("TextEFResultadoAnalisis");
            }
        }
        private string _TextEFResultadoGabinete;
        public string TextEFResultadoGabinete
        {
            get { return _TextEFResultadoGabinete; }
            set
            {
                _TextEFResultadoGabinete = value;
                OnPropertyChanged("TextEFResultadoGabinete");
            }
        }
        private string _TextEFImpresionDiagnostica;
        public string TextEFImpresionDiagnostica
        {
            get { return _TextEFImpresionDiagnostica; }
            set
            {
                _TextEFImpresionDiagnostica = value;
                OnPropertyChanged("TextEFImpresionDiagnostica");
            }
        }

        private string _ConclusionesF;

        public string ConclusionesF
        {
            get { return _ConclusionesF; }
            set { _ConclusionesF = value; OnPropertyChanged("ConclusionesF"); }
        }
        #endregion

        #endregion

        #endregion

        #region Validaciones
        private bool _IsEnabledPadecimientosPadre = false;

        public bool IsEnabledPadecimientosPadre
        {
            get { return _IsEnabledPadecimientosPadre; }
            set
            {
                _IsEnabledPadecimientosPadre = value;
                if (value)
                {
                    base.RemoveRule("IsCheckedDiabPadre");
                    base.RemoveRule("IsCheckedTBPadre");
                    base.RemoveRule("IsCheckedCAPadre");
                    base.RemoveRule("IsCheckedCardiPadre");
                    base.RemoveRule("IsCheckedEpiPadre");
                    base.RemoveRule("IsCheckedMentPadre");
                    base.RemoveRule("IsCheckedAlergiPadre");
                    base.RemoveRule("IsCheckedHipertPadre");
                    base.AddRule(() => IsCheckedDiabPadre, () => !string.IsNullOrEmpty(IsCheckedDiabPadre), "DIABETES PADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedTBPadre, () => !string.IsNullOrEmpty(IsCheckedTBPadre), "TB PADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedCAPadre, () => !string.IsNullOrEmpty(IsCheckedCAPadre), "CA PADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedCardiPadre, () => !string.IsNullOrEmpty(IsCheckedCardiPadre), "CARDIACO PADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedEpiPadre, () => !string.IsNullOrEmpty(IsCheckedEpiPadre), "EPILEPSIA PADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedMentPadre, () => !string.IsNullOrEmpty(IsCheckedMentPadre), "MENTALES PADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedAlergiPadre, () => !string.IsNullOrEmpty(IsCheckedAlergiPadre), "ALERGIAS PADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedHipertPadre, () => !string.IsNullOrEmpty(IsCheckedHipertPadre), "HIPERTENSO PADRE ES REQUERIDO!");
                }

                else
                {
                    base.RemoveRule("IsCheckedDiabPadre");
                    base.RemoveRule("IsCheckedTBPadre");
                    base.RemoveRule("IsCheckedCAPadre");
                    base.RemoveRule("IsCheckedCardiPadre");
                    base.RemoveRule("IsCheckedEpiPadre");
                    base.RemoveRule("IsCheckedMentPadre");
                    base.RemoveRule("IsCheckedAlergiPadre");
                    base.RemoveRule("IsCheckedHipertPadre");
                }


                OnPropertyChanged("IsCheckedDiabPadre");
                OnPropertyChanged("IsCheckedTBPadre");
                OnPropertyChanged("IsCheckedCAPadre");
                OnPropertyChanged("IsCheckedCardiPadre");
                OnPropertyChanged("IsCheckedEpiPadre");
                OnPropertyChanged("IsCheckedMentPadre");
                OnPropertyChanged("IsCheckedAlergiPadre");
                OnPropertyChanged("IsCheckedHipertPadre");
                OnPropertyChanged("IsEnabledPadecimientosPadre");
            }
        }
        private bool _IsEnabledPadecimientosMadre = false;

        public bool IsEnabledPadecimientosMadre
        {
            get { return _IsEnabledPadecimientosMadre; }
            set
            {
                _IsEnabledPadecimientosMadre = value;
                if (value)
                {
                    base.RemoveRule("IsCheckedDiabMadre");
                    base.RemoveRule("IsCheckedTBMadre");
                    base.RemoveRule("IsCheckedCAMadre");
                    base.RemoveRule("IsCheckedCardiMadre");
                    base.RemoveRule("IsCheckedEpiMadre");
                    base.RemoveRule("IsCheckedMentMadre");
                    base.RemoveRule("IsCheckedAlergiMadre");
                    base.RemoveRule("IsCheckedHipertMadre");
                    base.AddRule(() => IsCheckedDiabMadre, () => !string.IsNullOrEmpty(IsCheckedDiabMadre), "DIABETES MADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedTBMadre, () => !string.IsNullOrEmpty(IsCheckedTBMadre), "TB MADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedCAMadre, () => !string.IsNullOrEmpty(IsCheckedCAMadre), "CA PADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedCardiMadre, () => !string.IsNullOrEmpty(IsCheckedCardiMadre), "CARDIACO MADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedEpiMadre, () => !string.IsNullOrEmpty(IsCheckedEpiMadre), "EPILEPSIA MADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedMentMadre, () => !string.IsNullOrEmpty(IsCheckedMentMadre), "MENTALES MADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedAlergiMadre, () => !string.IsNullOrEmpty(IsCheckedAlergiMadre), "ALERGIAS MADRE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedHipertMadre, () => !string.IsNullOrEmpty(IsCheckedHipertMadre), "HIPERTENSO MADRE ES REQUERIDO!");
                }

                else
                {
                    base.RemoveRule("IsCheckedDiabMadre");
                    base.RemoveRule("IsCheckedTBMadre");
                    base.RemoveRule("IsCheckedCAMadre");
                    base.RemoveRule("IsCheckedCardiMadre");
                    base.RemoveRule("IsCheckedEpiMadre");
                    base.RemoveRule("IsCheckedMentMadre");
                    base.RemoveRule("IsCheckedAlergiMadre");
                    base.RemoveRule("IsCheckedHipertMadre");
                }


                OnPropertyChanged("IsCheckedDiabMadre");
                OnPropertyChanged("IsCheckedTBMadre");
                OnPropertyChanged("IsCheckedCAMadre");
                OnPropertyChanged("IsCheckedCardiMadre");
                OnPropertyChanged("IsCheckedEpiMadre");
                OnPropertyChanged("IsCheckedMentMadre");
                OnPropertyChanged("IsCheckedAlergiMadre");
                OnPropertyChanged("IsCheckedHipertMadre");
                OnPropertyChanged("IsEnabledPadecimientosMadre");
            }
        }
        private bool _IsEnabledPadecimientosHnos = false;

        public bool IsEnabledPadecimientosHnos
        {
            get { return _IsEnabledPadecimientosHnos; }
            set
            {
                _IsEnabledPadecimientosHnos = value;
                if (value)
                {
                    base.RemoveRule("IsCheckedDiabHnos");
                    base.RemoveRule("IsCheckedTBHnos");
                    base.RemoveRule("IsCheckedCAHnos");
                    base.RemoveRule("IsCheckedCardiHnos");
                    base.RemoveRule("IsCheckedEpiHnos");
                    base.RemoveRule("IsCheckedMentHnos");
                    base.RemoveRule("IsCheckedAlergiHnos");
                    base.RemoveRule("IsCheckedHipertHnos");
                    base.AddRule(() => IsCheckedDiabHnos, () => !string.IsNullOrEmpty(IsCheckedDiabHnos), "DIABETES HERMANOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedTBHnos, () => !string.IsNullOrEmpty(IsCheckedTBHnos), "TB HERMANOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedCAHnos, () => !string.IsNullOrEmpty(IsCheckedCAHnos), "CA HERMANOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedCardiHnos, () => !string.IsNullOrEmpty(IsCheckedCardiHnos), "CARDIACO HERMANOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedEpiHnos, () => !string.IsNullOrEmpty(IsCheckedEpiHnos), "EPILEPSIA HERMANOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedMentHnos, () => !string.IsNullOrEmpty(IsCheckedMentHnos), "MENTALES HERMANOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedAlergiHnos, () => !string.IsNullOrEmpty(IsCheckedAlergiHnos), "ALERGIAS HERMANOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedHipertHnos, () => !string.IsNullOrEmpty(IsCheckedHipertHnos), "HIPERTENSO HERMANOS ES REQUERIDO!");
                }

                else
                {
                    base.RemoveRule("IsCheckedDiabHnos");
                    base.RemoveRule("IsCheckedTBHnos");
                    base.RemoveRule("IsCheckedCAHnos");
                    base.RemoveRule("IsCheckedCardiHnos");
                    base.RemoveRule("IsCheckedEpiHnos");
                    base.RemoveRule("IsCheckedMentHnos");
                    base.RemoveRule("IsCheckedAlergiHnos");
                    base.RemoveRule("IsCheckedHipertHnos");
                }


                OnPropertyChanged("IsCheckedDiabHnos");
                OnPropertyChanged("IsCheckedTBHnos");
                OnPropertyChanged("IsCheckedCAHnos");
                OnPropertyChanged("IsCheckedCardiHnos");
                OnPropertyChanged("IsCheckedEpiHnos");
                OnPropertyChanged("IsCheckedMentHnos");
                OnPropertyChanged("IsCheckedAlergiHnos");
                OnPropertyChanged("IsCheckedHipertHnos");
                OnPropertyChanged("IsEnabledPadecimientosHnos");
            }
        }
        private bool _IsEnabledPadecimientosCony = false;

        public bool IsEnabledPadecimientosCony
        {
            get { return _IsEnabledPadecimientosCony; }
            set
            {
                _IsEnabledPadecimientosCony = value;
                if (value)
                {
                    base.RemoveRule("IsCheckedDiabCony");
                    base.RemoveRule("IsCheckedTBCony");
                    base.RemoveRule("IsCheckedCACony");
                    base.RemoveRule("IsCheckedCardiCony");
                    base.RemoveRule("IsCheckedEpiCony");
                    base.RemoveRule("IsCheckedMentCony");
                    base.RemoveRule("IsCheckedAlergiCony");
                    base.RemoveRule("IsCheckedHipertCony");
                    base.AddRule(() => IsCheckedDiabCony, () => !string.IsNullOrEmpty(IsCheckedDiabCony), "DIABETES CONYUGE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedTBCony, () => !string.IsNullOrEmpty(IsCheckedTBCony), "TB CONYUGE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedCACony, () => !string.IsNullOrEmpty(IsCheckedCACony), "CA CONYUGE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedCardiCony, () => !string.IsNullOrEmpty(IsCheckedCardiCony), "CARDIACO CONYUGE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedEpiCony, () => !string.IsNullOrEmpty(IsCheckedEpiCony), "EPILEPSIA CONYUGE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedMentCony, () => !string.IsNullOrEmpty(IsCheckedMentCony), "MENTALES CONYUGE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedAlergiCony, () => !string.IsNullOrEmpty(IsCheckedAlergiCony), "ALERGIAS CONYUGE ES REQUERIDO!");
                    base.AddRule(() => IsCheckedHipertCony, () => !string.IsNullOrEmpty(IsCheckedHipertCony), "HIPERTENSO CONYUGE ES REQUERIDO!");
                }

                else
                {
                    base.RemoveRule("IsCheckedDiabCony");
                    base.RemoveRule("IsCheckedTBCony");
                    base.RemoveRule("IsCheckedCACony");
                    base.RemoveRule("IsCheckedCardiCony");
                    base.RemoveRule("IsCheckedEpiCony");
                    base.RemoveRule("IsCheckedMentCony");
                    base.RemoveRule("IsCheckedAlergiCony");
                    base.RemoveRule("IsCheckedHipertCony");
                }


                OnPropertyChanged("IsCheckedDiabCony");
                OnPropertyChanged("IsCheckedTBCony");
                OnPropertyChanged("IsCheckedCACony");
                OnPropertyChanged("IsCheckedCardiCony");
                OnPropertyChanged("IsCheckedEpiCony");
                OnPropertyChanged("IsCheckedMentCony");
                OnPropertyChanged("IsCheckedAlergiCony");
                OnPropertyChanged("IsCheckedHipertCony");
                OnPropertyChanged("IsEnabledPadecimientosCony");
            }
        }
        private bool _IsEnabledPadecimientosHijos = false;

        public bool IsEnabledPadecimientosHijos
        {
            get { return _IsEnabledPadecimientosHijos; }
            set
            {
                _IsEnabledPadecimientosHijos = value;
                if (value)
                {
                    base.RemoveRule("IsCheckedDiabHijos");
                    base.RemoveRule("IsCheckedTBHijos");
                    base.RemoveRule("IsCheckedCAHijos");
                    base.RemoveRule("IsCheckedCardiHijos");
                    base.RemoveRule("IsCheckedEpiHijos");
                    base.RemoveRule("IsCheckedMentHijos");
                    base.RemoveRule("IsCheckedAlergiHijos");
                    base.RemoveRule("IsCheckedHipertHijos");
                    base.AddRule(() => IsCheckedDiabHijos, () => !string.IsNullOrEmpty(IsCheckedDiabHijos), "DIABETES HIJOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedTBHijos, () => !string.IsNullOrEmpty(IsCheckedTBHijos), "TB HIJOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedCAHijos, () => !string.IsNullOrEmpty(IsCheckedCAHijos), "CA HIJOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedCardiHijos, () => !string.IsNullOrEmpty(IsCheckedCardiHijos), "CARDIACO HIJOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedEpiHijos, () => !string.IsNullOrEmpty(IsCheckedEpiHijos), "EPILEPSIA HIJOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedMentHijos, () => !string.IsNullOrEmpty(IsCheckedMentHijos), "MENTALES HIJOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedAlergiHijos, () => !string.IsNullOrEmpty(IsCheckedAlergiHijos), "ALERGIAS HIJOS ES REQUERIDO!");
                    base.AddRule(() => IsCheckedHipertHijos, () => !string.IsNullOrEmpty(IsCheckedHipertHijos), "HIPERTENSO HIJOS ES REQUERIDO!");
                }

                else
                {
                    base.RemoveRule("IsCheckedDiabHijos");
                    base.RemoveRule("IsCheckedTBHijos");
                    base.RemoveRule("IsCheckedCAHijos");
                    base.RemoveRule("IsCheckedCardiHijos");
                    base.RemoveRule("IsCheckedEpiHijos");
                    base.RemoveRule("IsCheckedMentHijos");
                    base.RemoveRule("IsCheckedAlergiHijos");
                    base.RemoveRule("IsCheckedHipertHijos");
                }


                OnPropertyChanged("IsCheckedDiabHijos");
                OnPropertyChanged("IsCheckedTBHijos");
                OnPropertyChanged("IsCheckedCAHijos");
                OnPropertyChanged("IsCheckedCardiHijos");
                OnPropertyChanged("IsCheckedEpiHijos");
                OnPropertyChanged("IsCheckedMentHijos");
                OnPropertyChanged("IsCheckedAlergiHijos");
                OnPropertyChanged("IsCheckedHipertCony");
                OnPropertyChanged("IsCheckedHipertHijos");
                OnPropertyChanged("IsEnabledPadecimientosHijos");
            }
        }


        private bool _IsMuertoPadre = false;

        public bool IsMuertoPadre
        {
            get { return _IsMuertoPadre; }
            set
            {
                _IsMuertoPadre = value;
                if (value)
                {
                    base.RemoveRule("CuandoMuertePadre");
                    base.RemoveRule("CausaMuertePadre");
                    base.AddRule(() => CuandoMuertePadre, () => CuandoMuertePadre.HasValue, "FECHA DE MUERTE PADRE ES REQUERIDO!");
                    base.AddRule(() => CausaMuertePadre, () => !string.IsNullOrEmpty(CausaMuertePadre), "CAUSA MUERTE PADRE ES REQUERIDO!");
                }

                else
                {
                    base.RemoveRule("CuandoMuertePadre");
                    base.RemoveRule("CausaMuertePadre");
                }

                OnPropertyChanged("CuandoMuertePadre");
                OnPropertyChanged("CausaMuertePadre");
                OnPropertyChanged("IsMuertoPadre");
            }
        }
        private bool _IsMuertoMadre = false;

        public bool IsMuertoMadre
        {
            get { return _IsMuertoMadre; }
            set
            {
                _IsMuertoMadre = value;
                if (value)
                {
                    base.RemoveRule("CuandoMuerteMadre");
                    base.RemoveRule("CausaMuerteMadre");
                    base.AddRule(() => CuandoMuerteMadre, () => CuandoMuerteMadre.HasValue, "FECHA DE MUERTE MADRE ES REQUERIDO!");
                    base.AddRule(() => CausaMuerteMadre, () => !string.IsNullOrEmpty(CausaMuerteMadre), "CAUSA MUERTE MADRE ES REQUERIDO!");
                }

                else
                {
                    base.RemoveRule("CuandoMuerteMadre");
                    base.RemoveRule("CausaMuerteMadre");
                }

                OnPropertyChanged("CuandoMuerteMadre");
                OnPropertyChanged("CausaMuerteMadre");
                OnPropertyChanged("IsMuertoMadre");
            }
        }

        private bool _IsMuertoHnos = false;

        public bool IsMuertoHnos
        {
            get { return _IsMuertoHnos; }
            set
            {
                _IsMuertoHnos = value;
                if (value)
                {
                    base.RemoveRule("CuandoMuerteHnos");
                    base.RemoveRule("CausaMuerteHnos");
                    base.RemoveRule("TextHermanosHombres");
                    base.RemoveRule("TextHermanosMujeres");
                    base.AddRule(() => CuandoMuerteHnos, () => CuandoMuerteHnos.HasValue, "FECHA DE MUERTE HERMANOS ES REQUERIDO!");
                    base.AddRule(() => CausaMuerteHnos, () => !string.IsNullOrEmpty(CausaMuerteHnos), "CAUSA MUERTE HERMANOS ES REQUERIDO!");
                    base.AddRule(() => TextHermanosHombres, () => TextHermanosHombres.HasValue, "HERMANOS ES REQUERIDO!");
                    base.AddRule(() => TextHermanosMujeres, () => TextHermanosMujeres.HasValue, "HERMANAS ES REQUERIDO!");
                }
                else
                {
                    base.RemoveRule("CuandoMuerteHnos");
                    base.RemoveRule("CausaMuerteHnos");
                    base.RemoveRule("TextHermanosHombres");
                    base.RemoveRule("TextHermanosMujeres");
                }

                OnPropertyChanged("CuandoMuerteHnos");
                OnPropertyChanged("CausaMuerteHnos");
                OnPropertyChanged("TextHermanosHombres");
                OnPropertyChanged("TextHermanosMujeres");
                OnPropertyChanged("IsMuertoHnos");
            }
        }
        private bool _IsMuertoCony = false;

        public bool IsMuertoCony
        {
            get { return _IsMuertoCony; }
            set
            {
                _IsMuertoCony = value;
                if (value)
                {
                    base.RemoveRule("CuandoMuerteCony");
                    base.RemoveRule("CausaMuerteCony");
                    base.AddRule(() => CuandoMuerteCony, () => CuandoMuerteCony.HasValue, "FECHA DE MUERTE CONYUGE ES REQUERIDO!");
                    base.AddRule(() => CausaMuerteCony, () => !string.IsNullOrEmpty(CausaMuerteCony), "CAUSA MUERTE CONYUGE ES REQUERIDO!");
                }
                else
                {
                    base.RemoveRule("CuandoMuerteCony");
                    base.RemoveRule("CausaMuerteCony");
                }

                OnPropertyChanged("CuandoMuerteCony");
                OnPropertyChanged("CausaMuerteCony");
                OnPropertyChanged("IsMuertoCony");
            }
        }
        private bool _IsMuertoHijos = false;

        public bool IsMuertoHijos
        {
            get { return _IsMuertoHijos; }
            set
            {
                _IsMuertoHijos = value;
                if (value)
                {
                    base.RemoveRule("CuandoMuerteHijos");
                    base.RemoveRule("CausaMuerteHijos");
                    base.RemoveRule("TextEdadesHijos");
                    base.AddRule(() => CuandoMuerteHijos, () => CuandoMuerteHijos.HasValue, "FECHA DE MUERTE HIJOS ES REQUERIDO!");
                    base.AddRule(() => CausaMuerteHijos, () => !string.IsNullOrEmpty(CausaMuerteHijos), "CAUSA MUERTE HIJOS ES REQUERIDO!");
                }
                else
                {
                    base.RemoveRule("TextEdadesHijos");
                    base.RemoveRule("CuandoMuerteHijos");
                    base.RemoveRule("CausaMuerteHijos");
                    base.AddRule(() => TextEdadesHijos, () => TextEdadesHijos.HasValue, "EDAD DE LOS HIJOS ES REQUERIDO!");
                }

                OnPropertyChanged("CuandoMuerteHijos");
                OnPropertyChanged("CausaMuerteHijos");
                OnPropertyChanged("IsMuertoHijos");
                OnPropertyChanged("TextEdadesHijos");
            }
        }


        private HISTORIA_CLINICA _SelectedHistoriaC;

        public HISTORIA_CLINICA SelectedHistoriaC
        {
            get { return _SelectedHistoriaC; }
            set { _SelectedHistoriaC = value; OnPropertyChanged("SelectedHistoriaC"); }
        }

        private HISTORIA_CLINICA_DENTAL _SelctedHistoriaClinicaDental;

        public HISTORIA_CLINICA_DENTAL SelctedHistoriaClinicaDental
        {
            get { return _SelctedHistoriaClinicaDental; }
            set { _SelctedHistoriaClinicaDental = value; OnPropertyChanged("SelctedHistoriaClinicaDental"); }
        }

        private string _lblNombreArchivo;

        public string LblNombreArchivo
        {
            get { return _lblNombreArchivo; }
            set { _lblNombreArchivo = value; OnPropertyChanged("LblNombreArchivo"); }
        }

        private byte[] _ArchSelect;

        public byte[] ArchSelect
        {
            get { return _ArchSelect; }
            set
            {
                _ArchSelect = value;
                OnPropertyChanged("ArchSelect");
            }
        }

        private bool _IsEnabledTabHF = false;

        public bool IsEnabledTabHF
        {
            get { return _IsEnabledTabHF; }
            set { _IsEnabledTabHF = value; OnPropertyChanged("IsEnabledTabHF"); }
        }

        private bool _IsEnabledTabExamen = false;

        public bool IsEnabledTabExamen
        {
            get { return _IsEnabledTabExamen; }
            set { _IsEnabledTabExamen = value; OnPropertyChanged("IsEnabledTabExamen"); }
        }

        private bool _IsEnabledTabANP = false;

        public bool IsEnabledTabANP
        {
            get { return _IsEnabledTabANP; }
            set { _IsEnabledTabANP = value; OnPropertyChanged("IsEnabledTabANP"); }
        }
        private bool _IsEnabledTabAP = false;

        public bool IsEnabledTabAP
        {
            get { return _IsEnabledTabAP; }
            set { _IsEnabledTabAP = value; OnPropertyChanged("IsEnabledTabAP"); }
        }
        private bool _IsEnabledTabMujeres = false;

        public bool IsEnabledTabMujeres
        {
            get { return _IsEnabledTabMujeres; }
            set { _IsEnabledTabMujeres = value; OnPropertyChanged("IsEnabledTabMujeres"); }
        }
        private bool _IsEnabledTabPadAct = false;

        public bool IsEnabledTabPadAct
        {
            get { return _IsEnabledTabPadAct; }
            set { _IsEnabledTabPadAct = value; OnPropertyChanged("IsEnabledTabPadAct"); }
        }
        private bool _IsEnabledTabAparatosSist = false;

        public bool IsEnabledTabAparatosSist
        {
            get { return _IsEnabledTabAparatosSist; }
            set { _IsEnabledTabAparatosSist = value; OnPropertyChanged("IsEnabledTabAparatosSist"); }
        }
        private bool _IsENabledTabExplorFisica = false;

        public bool IsENabledTabExplorFisica
        {
            get { return _IsENabledTabExplorFisica; }
            set { _IsENabledTabExplorFisica = value; OnPropertyChanged("IsENabledTabExplorFisica"); }
        }
        private bool _IsEnabledTab65Mas = false;

        public bool IsEnabledTab65Mas
        {
            get { return _IsEnabledTab65Mas; }
            set { _IsEnabledTab65Mas = value; OnPropertyChanged("IsEnabledTab65Mas"); }
        }
        private bool _IsEnabledTabConsideracionesFinales = false;

        public bool IsEnabledTabConsideracionesFinales
        {
            get { return _IsEnabledTabConsideracionesFinales; }
            set { _IsEnabledTabConsideracionesFinales = value; OnPropertyChanged("IsEnabledTabConsideracionesFinales"); }
        }
        private bool _IsEnabledTabConclusiones = false;

        public bool IsEnabledTabConclusiones
        {
            get { return _IsEnabledTabConclusiones; }
            set { _IsEnabledTabConclusiones = value; OnPropertyChanged("IsEnabledTabConclusiones"); }
        }


        private bool datosIngresoVisible = false;
        public bool DatosIngresoVisible
        {
            get { return datosIngresoVisible; }
            set { datosIngresoVisible = value; OnPropertyChanged("DatosIngresoVisible"); }
        }
        private bool ingresosVisible = true;
        public bool IngresosVisible
        {
            get { return ingresosVisible; }
            set { ingresosVisible = value; OnPropertyChanged("IngresosVisible"); }
        }

        #endregion

        #region Arbol

        private List<ControlPenales.Clases.TreeViewList> _TreeList;
        public List<ControlPenales.Clases.TreeViewList> TreeList
        {
            get { return _TreeList; }
            set { _TreeList = value; OnPropertyChanged("TreeList"); }
        }

        #endregion

        #region Digitalizacion
        private ObservableCollection<HC_DOCUMENTO_TIPO> listTipoDocumento;
        public ObservableCollection<HC_DOCUMENTO_TIPO> ListTipoDocumento
        {
            get { return listTipoDocumento; }
            set { listTipoDocumento = value; OnPropertyChanged("ListTipoDocumento"); }
        }

        private ObservableCollection<HISTORIA_CLINICA_DOCUMENTO> lstDocumentos;

        public ObservableCollection<HISTORIA_CLINICA_DOCUMENTO> LstDocumentos
        {
            get { return lstDocumentos; }
            set { lstDocumentos = value; OnPropertyChanged("LstDocumentos"); }
        }

        HC_DOCUMENTO_TIPO _SelectedTipoDocumento;
        public HC_DOCUMENTO_TIPO SelectedTipoDocumento
        {
            get { return _SelectedTipoDocumento; }
            set { _SelectedTipoDocumento = value; OnPropertyChanged("SelectedTipoDocumento"); }
        }

        private DateTime? datePickCapturaDocumento = Fechas.GetFechaDateServer;
        public DateTime? DatePickCapturaDocumento
        {
            get { return datePickCapturaDocumento; }
            set { datePickCapturaDocumento = value; OnPropertyChanged("DatePickCapturaDocumento"); }
        }

        private string observacionDocumento;
        public string ObservacionDocumento
        {
            get { return observacionDocumento; }
            set { observacionDocumento = value; OnPropertyChanged("ObservacionDocumento"); }
        }

        private bool _AutoGuardado = true;
        public bool AutoGuardado
        {
            get { return _AutoGuardado; }
            set { _AutoGuardado = value; OnPropertyChanged("AutoGuardado"); }
        }

        private bool _Duplex = true;
        public bool Duplex
        {
            get { return _Duplex; }
            set
            {
                _Duplex = value;
                OnPropertyChanged("Duplex");
            }
        }

        private EscanerSources selectedSource = null;
        public EscanerSources SelectedSource
        {
            get { return selectedSource; }
            set { selectedSource = value; RaisePropertyChanged("SelectedSource"); }
        }

        private List<EscanerSources> lista_Sources = null;
        public List<EscanerSources> Lista_Sources
        {
            get { return lista_Sources; }
            set { lista_Sources = value; RaisePropertyChanged("Lista_Sources"); }
        }

        private string hojasMaximo;
        public string HojasMaximo
        {
            get { return hojasMaximo; }
            set { hojasMaximo = value; RaisePropertyChanged("HojasMaximo"); }
        }

        private bool isObservacionesEscanerEnabled = false;
        public bool IsObservacionesEscanerEnabled
        {
            get { return isObservacionesEscanerEnabled; }
            set { isObservacionesEscanerEnabled = value; OnPropertyChanged("IsObservacionesEscanerEnabled"); }
        }

        #endregion


        #region Digitalizacion

        private byte[] documento = null;
        public byte[] Documento
        {
            get { return documento; }
            set { documento = value; OnPropertyChanged("Documento"); }
        }


        private int _IdTipoDocumento;

        public int IdTipoDocumento
        {
            get { return _IdTipoDocumento; }
            set { _IdTipoDocumento = value; OnPropertyChanged("IdTipoDocumento"); }
        }

        private short? _IdTipoFormatoDocumento;

        public short? IdTipoFormatoDocumento
        {
            get { return _IdTipoFormatoDocumento; }
            set { _IdTipoFormatoDocumento = value; OnPropertyChanged("IdTipoFormatoDocumento"); }
        }

        DigitalizarDocumento escaner = new DigitalizarDocumento(Application.Current.Windows[0]);
        private byte[] documentoDigitalizado = null;
        public byte[] DocumentoDigitalizado
        {
            get { return documentoDigitalizado; }
            set { documentoDigitalizado = value; OnPropertyChanged("DocumentoDigitalizado"); }
        }

        #endregion
        #region Busqueda por Huella

        private BuscarPorHuellaYNipView HuellaWindow;
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

        private System.Windows.Media.Brush _PulgarDerecho;
        public System.Windows.Media.Brush PulgarDerecho
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

        private System.Windows.Media.Brush _IndiceDerecho;
        public System.Windows.Media.Brush IndiceDerecho
        {
            get { return _IndiceDerecho; }
            set
            {
                _IndiceDerecho = value;
                OnPropertyChanged("IndiceDerecho");
            }
        }

        private System.Windows.Media.Brush _MedioDerecho;
        public System.Windows.Media.Brush MedioDerecho
        {
            get { return _MedioDerecho; }
            set
            {
                _MedioDerecho = value;
                OnPropertyChanged("MedioDerecho");
            }
        }

        private System.Windows.Media.Brush _AnularDerecho;
        public System.Windows.Media.Brush AnularDerecho
        {
            get { return _AnularDerecho; }
            set
            {
                _AnularDerecho = value;
                OnPropertyChanged("AnularDerecho");
            }
        }

        private System.Windows.Media.Brush _MeñiqueDerecho;
        public System.Windows.Media.Brush MeñiqueDerecho
        {
            get { return _MeñiqueDerecho; }
            set
            {
                _MeñiqueDerecho = value;
                OnPropertyChanged("MeñiqueDerecho");
            }
        }

        private System.Windows.Media.Brush _PulgarIzquierdo;
        public System.Windows.Media.Brush PulgarIzquierdo
        {
            get { return _PulgarIzquierdo; }
            set
            {
                _PulgarIzquierdo = value;
                OnPropertyChanged("PulgarIzquierdo");
            }
        }

        private System.Windows.Media.Brush _IndiceIzquierdo;
        public System.Windows.Media.Brush IndiceIzquierdo
        {
            get { return _IndiceIzquierdo; }
            set
            {
                _IndiceIzquierdo = value;
                OnPropertyChanged("IndiceIzquierdo");
            }
        }

        private System.Windows.Media.Brush _MedioIzquierdo;
        public System.Windows.Media.Brush MedioIzquierdo
        {
            get { return _MedioIzquierdo; }
            set
            {
                _MedioIzquierdo = value;
                OnPropertyChanged("MedioIzquierdo");
            }
        }

        private System.Windows.Media.Brush _AnularIzquierdo;
        public System.Windows.Media.Brush AnularIzquierdo
        {
            get { return _AnularIzquierdo; }
            set
            {
                _AnularIzquierdo = value;
                OnPropertyChanged("AnularIzquierdo");
            }
        }

        private System.Windows.Media.Brush _MeñiqueIzquierdo;
        public System.Windows.Media.Brush MeñiqueIzquierdo
        {
            get { return _MeñiqueIzquierdo; }
            set
            {
                _MeñiqueIzquierdo = value;
                OnPropertyChanged("MeñiqueIzquierdo");
            }
        }


        private bool Conectado;
        public enumTipoPersona BuscarPor = enumTipoPersona.IMPUTADO;
        private Visibility _ShowCapturar = Visibility.Collapsed;
        public Visibility ShowCapturar
        {
            get { return _ShowCapturar; }
            set
            {
                _ShowCapturar = value;
                OnPropertyChanged("ShowCapturar");
            }
        }
        private Visibility _ShowContinuar = Visibility.Collapsed;
        public Visibility ShowContinuar
        {
            get { return _ShowContinuar; }
            set
            {
                _ShowContinuar = value;
                OnPropertyChanged("ShowContinuar");
            }
        }
        private Visibility _ShowLoading = Visibility.Collapsed;
        public Visibility ShowLoading
        {
            get { return _ShowLoading; }
            set
            {
                _ShowLoading = value;
                OnPropertyChanged("ShowLoading");
            }
        }
        private bool isKeepSearching { get; set; }
        private bool GuardandoHuellas { get; set; }
        private bool CancelKeepSearching { get; set; }
        private bool _GuardarHuellas { get; set; }
        public IList<PlantillaBiometrico> HuellasCapturadas { get; set; }
        private string _CabeceraBusqueda;
        public string CabeceraBusqueda
        {
            get { return _CabeceraBusqueda; }
            set
            {
                _CabeceraBusqueda = value;
                OnPropertyChanged("CabeceraBusqueda");
            }
        }
        private string _CabeceraFoto;
        public string CabeceraFoto
        {
            get { return _CabeceraFoto; }
            set
            {
                _CabeceraFoto = value;
                OnPropertyChanged("CabeceraFoto");
            }
        }
        private System.Windows.Media.Brush _ColorMessage;
        public System.Windows.Media.Brush ColorMessage
        {
            get { return _ColorMessage; }
            set
            {
                _ColorMessage = value;
                RaisePropertyChanged("ColorMessage");
            }
        }
        private enumTipoBiometrico? _DD_Dedo = enumTipoBiometrico.INDICE_DERECHO;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set
            {
                LimpiarCampos();
                _DD_Dedo = value;
                OnPropertyChanged("DD_Dedo");
            }
        }

        private IList<ResultadoBusquedaBiometrico> _ListResultado;
        public IList<ResultadoBusquedaBiometrico> ListResultado
        {
            get { return _ListResultado; }
            set
            {
                _ListResultado = value;
                var bk = SelectRegistro;
                OnPropertyChanged("ListResultado");
                if (CancelKeepSearching)
                    SelectRegistro = bk;
            }
        }
        private ResultadoBusquedaBiometrico _SelectRegistro;
        public ResultadoBusquedaBiometrico SelectRegistro
        {
            get { return _SelectRegistro; }
            set
            {
                _SelectRegistro = value;
                FotoRegistro = value == null ? new Imagenes().getImagenPerson() : new Imagenes().ConvertBitmapToByte((System.Windows.Media.Imaging.BitmapSource)value.Foto);
                OnPropertyChanged("SelectRegistro");
            }
        }
        private byte[] _FotoRegistro = new Imagenes().getImagenPerson();
        public byte[] FotoRegistro
        {
            get { return _FotoRegistro; }
            set { _FotoRegistro = value; OnPropertyChanged("FotoRegistro"); }
        }
        private string _TextNipBusqueda;
        public string TextNipBusqueda
        {
            get { return _TextNipBusqueda; }
            set { _TextNipBusqueda = value; OnPropertyChanged("TextNipBusqueda"); }
        }
        #endregion
        #region patologicos

        private bool _HabilitaImputadosDent = false;

        public bool HabilitaImputadosDent
        {
            get { return _HabilitaImputadosDent; }
            set { _HabilitaImputadosDent = value; OnPropertyChanged("HabilitaImputadosDent"); }
        }

        private ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS> lstPatologicos;

        public ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS> LstPatologicos
        {
            get { return lstPatologicos; }
            set { lstPatologicos = value; OnPropertyChanged("LstPatologicos"); }
        }


        private bool _Directo { get; set; }
        private bool _ConstanciaDoc { get; set; }
        private enum eTipoDocumentoHistoriaClinica
        {
            DIRECTO = 2,
            CONSTANCIA_DOCUMENTAL = 1
        };

        private enum eFisico
        {
            SI = 0,
            NO = 1
        };

        private ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS> lstCondensadoPatologicos;

        public ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS> LstCondensadoPatologicos
        {
            get { return lstCondensadoPatologicos; }
            set { lstCondensadoPatologicos = value; OnPropertyChanged("LstCondensadoPatologicos"); }
        }

        private HISTORIA_CLINICA_PATOLOGICOS _SelectedCondensadoPato;

        public HISTORIA_CLINICA_PATOLOGICOS SelectedCondensadoPato
        {
            get { return _SelectedCondensadoPato; }
            set
            {
                _SelectedCondensadoPato = value;
                OnPropertyChanged("SelectedCondensadoPato");
            }
        }

        private bool _IsEnabledEdicionObservaciones = false;

        public bool IsEnabledEdicionObservaciones
        {
            get { return _IsEnabledEdicionObservaciones; }
            set { _IsEnabledEdicionObservaciones = value; OnPropertyChanged("IsEnabledEdicionObservaciones"); }
        }

        private HISTORIA_CLINICA_PATOLOGICOS _SelectedPatologico;

        public HISTORIA_CLINICA_PATOLOGICOS SelectedPatologico
        {
            get { return _SelectedPatologico; }
            set { _SelectedPatologico = value; OnPropertyChanged("SelectedPatologico"); }
        }

        private ObservableCollection<PATOLOGICO_CAT> lstEnfermedades;

        public ObservableCollection<PATOLOGICO_CAT> LstEnfermedades
        {
            get { return lstEnfermedades; }
            set { lstEnfermedades = value; OnPropertyChanged("LstEnfermedades"); }
        }
        #endregion

        private string _GenitalesAMbos;

        public string GenitalesAMbos
        {
            get { return _GenitalesAMbos; }
            set { _GenitalesAMbos = value; OnPropertyChanged("GenitalesAMbos"); }
        }

        private string _ObservacionesAlcohlismo;

        public string ObservacionesAlcohlismo
        {
            get { return _ObservacionesAlcohlismo; }
            set { _ObservacionesAlcohlismo = value; OnPropertyChanged("ObservacionesAlcohlismo"); }
        }

        private string _ObservacionesTabaquismo;

        public string ObservacionesTabaquismo
        {
            get { return _ObservacionesTabaquismo; }
            set { _ObservacionesTabaquismo = value; OnPropertyChanged("ObservacionesTabaquismo"); }
        }

        private string _ObservacionesToxicomanias;

        public string ObservacionesToxicomanias
        {
            get { return _ObservacionesToxicomanias; }
            set { _ObservacionesToxicomanias = value; OnPropertyChanged("ObservacionesToxicomanias"); }
        }

        private string _TextExploracionfisica;

        public string TextExploracionfisica
        {
            get { return _TextExploracionfisica; }
            set { _TextExploracionfisica = value; OnPropertyChanged("TextExploracionfisica"); }
        }

        private string _MedicamentosActivos;

        public string MedicamentosActivos
        {
            get { return _MedicamentosActivos; }
            set { _MedicamentosActivos = value; OnPropertyChanged("MedicamentosActivos"); }
        }

        #region Documentos
        private enum eFisicoDigital
        {
            FISICO = 0,
            DIGITAL = 1
        };

        private int _TipoDocto = -1;
        public int TipoDocto
        {
            get { return _TipoDocto; }
            set { _TipoDocto = value; OnPropertyChanged("TipoDocto"); }
        }

        private string _TipoArchivo;

        public string TipoArchivo
        {
            get { return _TipoArchivo; }
            set
            {
                if (value == "F")
                {
                    IsEnabledTipoDocto = true;
                    base.RemoveRule("TipoDocto");
                    base.AddRule(() => TipoDocto, () => TipoDocto != null, "TIPO DE DOCUMENTO ES REQUERIDO!");
                }

                if (value == "D")
                {
                    IsEnabledTipoDocto = true;
                    base.RemoveRule("TipoDocto");
                    base.AddRule(() => TipoDocto, () => TipoDocto != null, "TIPO DE DOCUMENTO ES REQUERIDO!");
                }

                OnPropertyChanged("TipoDocto");
                OnPropertyChanged("IsEnabledTipoDocto");
                _TipoArchivo = value;
                OnPropertyChanged("TipoArchivo");
            }
        }

        private ObservableCollection<HISTORIA_CLINICA_DOCUMENTO> lstDocumentosActuales;

        public ObservableCollection<HISTORIA_CLINICA_DOCUMENTO> LstDocumentosActuales
        {
            get { return lstDocumentosActuales; }
            set { lstDocumentosActuales = value; OnPropertyChanged("LstDocumentosActuales"); }
        }

        private HISTORIA_CLINICA_DOCUMENTO _SelectedDocumentoActual;

        public HISTORIA_CLINICA_DOCUMENTO SelectedDocumentoActual
        {
            get { return _SelectedDocumentoActual; }
            set { _SelectedDocumentoActual = value; OnPropertyChanged("SelectedDocumentoActual"); }
        }

        private bool _IsEnabledTipoDocto = false;

        public bool IsEnabledTipoDocto
        {
            get { return _IsEnabledTipoDocto; }
            set { _IsEnabledTipoDocto = value; OnPropertyChanged("IsEnabledTipoDocto"); }
        }

        #endregion
        #region Definicion de Solo Lectura
        private bool _IsReadOnlyHerefoFamiliares = true;

        public bool IsReadOnlyHerefoFamiliares
        {
            get { return _IsReadOnlyHerefoFamiliares; }
            set
            {
                _IsReadOnlyHerefoFamiliares = value;
                OnPropertyChanged("IsReadOnlyHerefoFamiliares");
            }
        }

        private bool _IsReadOnlyDatos = true;

        public bool IsReadOnlyDatos
        {
            get { return _IsReadOnlyDatos; }
            set { _IsReadOnlyDatos = value; OnPropertyChanged("IsReadOnlyDatos"); }
        }

        private bool _IsDocumentoFisicoEnabled = true;

        public bool IsDocumentoFisicoEnabled
        {
            get { return _IsDocumentoFisicoEnabled; }
            set { _IsDocumentoFisicoEnabled = value; OnPropertyChanged("IsDocumentoFisicoEnabled"); }
        }

        private bool _DigitalizacionDirecta = true;

        public bool DigitalizacionDirecta
        {
            get { return _DigitalizacionDirecta; }
            set { _DigitalizacionDirecta = value; OnPropertyChanged("DigitalizacionDirecta"); }
        }
        #endregion

        private string _Arterial1;
        public string Arterial1
        {
            get { return _Arterial1; }
            set
            {
                _Arterial1 = value;
                if (!string.IsNullOrEmpty(value))
                    TextPresionArterial = value + "/" + Arterial2;

                OnPropertyChanged("Arterial1");
            }
        }

        private string _Arterial2;
        public string Arterial2
        {
            get { return _Arterial2; }
            set
            {
                _Arterial2 = value;
                if (!string.IsNullOrEmpty(value))
                    TextPresionArterial = Arterial1 + "/" + value;

                OnPropertyChanged("Arterial2");
            }
        }

        private string _TituloHeaderExpandirDescripcion;

        public string TituloHeaderExpandirDescripcion
        {
            get { return _TituloHeaderExpandirDescripcion; }
            set { _TituloHeaderExpandirDescripcion = value; OnPropertyChanged("TituloHeaderExpandirDescripcion"); }
        }

        private string _TextAmpliarDescripcion;
        public string TextAmpliarDescripcion
        {
            get { return _TextAmpliarDescripcion; }
            set { _TextAmpliarDescripcion = value; OnPropertyChanged("TextAmpliarDescripcion"); }
        }

        private ObservableCollection<GRUPO_VULNERABLE> lstGruposVulnerables;

        public ObservableCollection<GRUPO_VULNERABLE> LstGruposVulnerables
        {
            get { return lstGruposVulnerables; }
            set { lstGruposVulnerables = value; OnPropertyChanged("LstGruposVulnerables"); }
        }
        private GRUPO_VULNERABLE _SelectedGrupoVulnerable;

        public GRUPO_VULNERABLE SelectedGrupoVulnerable
        {
            get { return _SelectedGrupoVulnerable; }
            set { _SelectedGrupoVulnerable = value; OnPropertyChanged("SelectedGrupoVulnerable"); }
        }

        private ObservableCollection<SECTOR_CLASIFICACION> lstSectoresVulnerbles;
        public ObservableCollection<SECTOR_CLASIFICACION> LstSectoresVulnerbles
        {
            get { return lstSectoresVulnerbles; }
            set { lstSectoresVulnerbles = value; OnPropertyChanged("LstSectoresVulnerbles"); }
        }

        private SECTOR_CLASIFICACION _SelectedSectorVulnerable;

        public SECTOR_CLASIFICACION SelectedSectorVulnerable
        {
            get { return _SelectedSectorVulnerable; }
            set
            {
                _SelectedSectorVulnerable = value;
                OnPropertyChanged("SelectedSectorVulnerable");
            }
        }


        private bool _AplicaVulnerable;
        public bool AplicaVulnerable
        {
            get { return _AplicaVulnerable; }
            set
            {
                _AplicaVulnerable = value;
                OnPropertyChanged("AplicaVulnerable");
            }
        }

        private bool _HabilitaImputados = true;

        public bool HabilitaImputados
        {
            get { return _HabilitaImputados; }
            set { _HabilitaImputados = value; OnPropertyChanged("HabilitaImputados"); }
        }

        private bool _EnabledAttachDocumentos = false;
        public bool EnabledAttachDocumentos
        {
            get { return _EnabledAttachDocumentos; }
            set { _EnabledAttachDocumentos = value; OnPropertyChanged("EnabledAttachDocumentos"); }
        }

        #region DATOS DENTALES

        private List<CustomOdontograma> ListOdontograma;
        private List<CustomOdontograma> ListSeguimientoDientes;
        private string _TextTratamientoDental;
        public string TextTratamientoDental
        {
            get { return _TextTratamientoDental; }
            set { _TextTratamientoDental = value; OnPropertyChanged("TextTratamientoDental"); }
        }

        private bool _SelectedAplicaEnfermedadDental = false;
        public bool SelectedAplicaEnfermedadDental
        {
            get { return _SelectedAplicaEnfermedadDental; }
            set
            {
                _SelectedAplicaEnfermedadDental = value;
                if (value)
                {
                    IsEnabledEnfermedadDental = true;
                    SelectedAplicaTratamientoDental = false;
                    IdSelectedTratamientoDental = -1;
                }
                else
                    IsEnabledEnfermedadDental = false;

                OnPropertyChanged("IsEnabledEnfermedadDental");
                OnPropertyChanged("SelectedAplicaEnfermedadDental");
            }
        }

        private bool _SelectedAplicaTratamientoDental = false;
        public bool SelectedAplicaTratamientoDental
        {
            get { return _SelectedAplicaTratamientoDental; }
            set
            {
                _SelectedAplicaTratamientoDental = value;
                if (value)
                {
                    IsEnabledTratamientoDental = true;
                    SelectedAplicaEnfermedadDental = false;
                    IdSelectedEnfermedadDental = -1;
                }
                else
                    IsEnabledTratamientoDental = false;

                OnPropertyChanged("SelectedAplicaTratamientoDental");
                OnPropertyChanged("IsEnabledTratamientoDental");
            }
        }

        private bool _IsReadOnlyDatosHistoriaClinica = false;

        public bool IsReadOnlyDatosHistoriaClinica
        {
            get { return _IsReadOnlyDatosHistoriaClinica; }
            set { _IsReadOnlyDatosHistoriaClinica = value; OnPropertyChanged("IsReadOnlyDatosHistoriaClinica"); }
        }
        private string _CheckPadreViveDental;

        public string CheckPadreViveDental
        {
            get { return _CheckPadreViveDental; }
            set { _CheckPadreViveDental = value; OnPropertyChanged("CheckPadreViveDental"); }
        }

        private short _TextEdadPadreDental;

        public short TextEdadPadreDental
        {
            get { return _TextEdadPadreDental; }
            set { _TextEdadPadreDental = value; OnPropertyChanged("TextEdadPadreDental"); }
        }

        private string _CheckPadrePadeceDental;

        public string CheckPadrePadeceDental
        {
            get { return _CheckPadrePadeceDental; }
            set { _CheckPadrePadeceDental = value; OnPropertyChanged("CheckPadrePadeceDental"); }
        }

        private string _IsCheckedDiabPadreDental;

        public string IsCheckedDiabPadreDental
        {
            get { return _IsCheckedDiabPadreDental; }
            set { _IsCheckedDiabPadreDental = value; OnPropertyChanged("IsCheckedDiabPadreDental"); }
        }

        private string _IsCheckedTBPadreDental;

        public string IsCheckedTBPadreDental
        {
            get { return _IsCheckedTBPadreDental; }
            set { _IsCheckedTBPadreDental = value; OnPropertyChanged("IsCheckedTBPadreDental"); }
        }

        private string _IsCheckedCAPadreDental;

        public string IsCheckedCAPadreDental
        {
            get { return _IsCheckedCAPadreDental; }
            set { _IsCheckedCAPadreDental = value; OnPropertyChanged("IsCheckedCAPadreDental"); }
        }

        private string _IsCheckedCardiPadreDental;

        public string IsCheckedCardiPadreDental
        {
            get { return _IsCheckedCardiPadreDental; }
            set { _IsCheckedCardiPadreDental = value; OnPropertyChanged("IsCheckedCardiPadreDental"); }
        }

        private string _IsCheckedEpiPadreDental;

        public string IsCheckedEpiPadreDental
        {
            get { return _IsCheckedEpiPadreDental; }
            set { _IsCheckedEpiPadreDental = value; OnPropertyChanged("IsCheckedEpiPadreDental"); }
        }

        private string _IsCheckedMentPadreDental;

        public string IsCheckedMentPadreDental
        {
            get { return _IsCheckedMentPadreDental; }
            set { _IsCheckedMentPadreDental = value; OnPropertyChanged("IsCheckedMentPadreDental"); }
        }

        private string _IsCheckedAlergiPadreDental;

        public string IsCheckedAlergiPadreDental
        {
            get { return _IsCheckedAlergiPadreDental; }
            set { _IsCheckedAlergiPadreDental = value; OnPropertyChanged("IsCheckedAlergiPadreDental"); }
        }

        private string _IsCheckedHipertPadreDental;

        public string IsCheckedHipertPadreDental
        {
            get { return _IsCheckedHipertPadreDental; }
            set { _IsCheckedHipertPadreDental = value; OnPropertyChanged("IsCheckedHipertPadreDental"); }
        }

        private DateTime? _CuandoMuertePadreDental;

        public DateTime? CuandoMuertePadreDental
        {
            get { return _CuandoMuertePadreDental; }
            set { _CuandoMuertePadreDental = value; OnPropertyChanged("CuandoMuertePadreDental"); }
        }

        private string _CausaMuertePadreDental;

        public string CausaMuertePadreDental
        {
            get { return _CausaMuertePadreDental; }
            set { _CausaMuertePadreDental = value; OnPropertyChanged("CausaMuertePadreDental"); }
        }


        private string _CheckMadreViveDental;

        public string CheckMadreViveDental
        {
            get { return _CheckMadreViveDental; }
            set { _CheckMadreViveDental = value; OnPropertyChanged("CheckMadreViveDental"); }
        }

        private short _TextEdadMadreDental;

        public short TextEdadMadreDental
        {
            get { return _TextEdadMadreDental; }
            set { _TextEdadMadreDental = value; OnPropertyChanged("TextEdadMadreDental"); }
        }

        private string _CheckMadrePadeceDental;

        public string CheckMadrePadeceDental
        {
            get { return _CheckMadrePadeceDental; }
            set { _CheckMadrePadeceDental = value; OnPropertyChanged("CheckMadrePadeceDental"); }
        }

        private string _IsCheckedDiabMadreDental;

        public string IsCheckedDiabMadreDental
        {
            get { return _IsCheckedDiabMadreDental; }
            set { _IsCheckedDiabMadreDental = value; OnPropertyChanged("IsCheckedDiabMadreDental"); }
        }

        private string _ToxicomaniasDental;

        public string ToxicomaniasDental
        {
            get { return _ToxicomaniasDental; }
            set { _ToxicomaniasDental = value; OnPropertyChanged("ToxicomaniasDental"); }
        }
        private string _IsCheckedTBMadreDental;

        public string IsCheckedTBMadreDental
        {
            get { return _IsCheckedTBMadreDental; }
            set { _IsCheckedTBMadreDental = value; OnPropertyChanged("IsCheckedTBMadreDental"); }
        }

        private string _IsCheckedCAMadreDental;

        public string IsCheckedCAMadreDental
        {
            get { return _IsCheckedCAMadreDental; }
            set { _IsCheckedCAMadreDental = value; OnPropertyChanged("IsCheckedCAMadreDental"); }
        }

        private string _IsCheckedCardiMadreDental;

        public string IsCheckedCardiMadreDental
        {
            get { return _IsCheckedCardiMadreDental; }
            set { _IsCheckedCardiMadreDental = value; OnPropertyChanged("IsCheckedCardiMadreDental"); }
        }

        private string _IsCheckedEpiMadreDental;

        public string IsCheckedEpiMadreDental
        {
            get { return _IsCheckedEpiMadreDental; }
            set { _IsCheckedEpiMadreDental = value; OnPropertyChanged("IsCheckedEpiMadreDental"); }
        }

        private string _IsCheckedMentMadreDental;

        public string IsCheckedMentMadreDental
        {
            get { return _IsCheckedMentMadreDental; }
            set { _IsCheckedMentMadreDental = value; OnPropertyChanged("IsCheckedMentMadreDental"); }
        }

        private string _IsCheckedAlergiMadreDental;

        public string IsCheckedAlergiMadreDental
        {
            get { return _IsCheckedAlergiMadreDental; }
            set { _IsCheckedAlergiMadreDental = value; OnPropertyChanged("IsCheckedAlergiMadreDental"); }
        }

        private string _IsCheckedHipertMadreDental;

        public string IsCheckedHipertMadreDental
        {
            get { return _IsCheckedHipertMadreDental; }
            set { _IsCheckedHipertMadreDental = value; OnPropertyChanged("IsCheckedHipertMadreDental"); }
        }

        private DateTime? _CuandoMuerteMadreDental;

        public DateTime? CuandoMuerteMadreDental
        {
            get { return _CuandoMuerteMadreDental; }
            set { _CuandoMuerteMadreDental = value; OnPropertyChanged("CuandoMuerteMadreDental"); }
        }

        private string _CausaMuerteMadreDental;

        public string CausaMuerteMadreDental
        {
            get { return _CausaMuerteMadreDental; }
            set { _CausaMuerteMadreDental = value; OnPropertyChanged("CausaMuerteMadreDental"); }
        }


        private string _CheckHermanosVivosDental;

        public string CheckHermanosVivosDental
        {
            get { return _CheckHermanosVivosDental; }
            set { _CheckHermanosVivosDental = value; OnPropertyChanged("CheckHermanosVivosDental"); }
        }

        private short _TextHermanosMujeresDental;

        public short TextHermanosMujeresDental
        {
            get { return _TextHermanosMujeresDental; }
            set { _TextHermanosMujeresDental = value; OnPropertyChanged("TextHermanosMujeresDental"); }
        }

        private short _TextHermanosHombresDental;

        public short TextHermanosHombresDental
        {
            get { return _TextHermanosHombresDental; }
            set { _TextHermanosHombresDental = value; OnPropertyChanged("TextHermanosHombresDental"); }
        }

        private string _CheckHermanosSanosDental;

        public string CheckHermanosSanosDental
        {
            get { return _CheckHermanosSanosDental; }
            set { _CheckHermanosSanosDental = value; OnPropertyChanged("CheckHermanosSanosDental"); }
        }

        private string _IsCheckedDiabHnosDental;

        public string IsCheckedDiabHnosDental
        {
            get { return _IsCheckedDiabHnosDental; }
            set { _IsCheckedDiabHnosDental = value; OnPropertyChanged("IsCheckedDiabHnosDental"); }
        }

        private string _IsCheckedTBHnosDental;

        public string IsCheckedTBHnosDental
        {
            get { return _IsCheckedTBHnosDental; }
            set { _IsCheckedTBHnosDental = value; OnPropertyChanged("IsCheckedTBHnosDental"); }
        }

        private string _IsCheckedCAHnosDental;

        public string IsCheckedCAHnosDental
        {
            get { return _IsCheckedCAHnosDental; }
            set { _IsCheckedCAHnosDental = value; OnPropertyChanged("IsCheckedCAHnosDental"); }
        }

        private string _IsCheckedCardiHnosDental;

        public string IsCheckedCardiHnosDental
        {
            get { return _IsCheckedCardiHnosDental; }
            set { _IsCheckedCardiHnosDental = value; OnPropertyChanged("IsCheckedCardiHnosDental"); }
        }

        private string _IsCheckedEpiHnosDental;

        public string IsCheckedEpiHnosDental
        {
            get { return _IsCheckedEpiHnosDental; }
            set { _IsCheckedEpiHnosDental = value; OnPropertyChanged("IsCheckedEpiHnosDental"); }
        }

        private string _IsCheckedMentHnosDental;

        public string IsCheckedMentHnosDental
        {
            get { return _IsCheckedMentHnosDental; }
            set { _IsCheckedMentHnosDental = value; OnPropertyChanged("IsCheckedMentHnosDental"); }
        }

        private string _IsCheckedAlergiHnosDental;

        public string IsCheckedAlergiHnosDental
        {
            get { return _IsCheckedAlergiHnosDental; }
            set { _IsCheckedAlergiHnosDental = value; OnPropertyChanged("IsCheckedAlergiHnosDental"); }
        }

        private string _IsCheckedHipertHnosDental;

        public string IsCheckedHipertHnosDental
        {
            get { return _IsCheckedHipertHnosDental; }
            set { _IsCheckedHipertHnosDental = value; OnPropertyChanged("IsCheckedHipertHnosDental"); }
        }

        private DateTime? _CuandoMuerteHnosDental;

        public DateTime? CuandoMuerteHnosDental
        {
            get { return _CuandoMuerteHnosDental; }
            set { _CuandoMuerteHnosDental = value; OnPropertyChanged("CuandoMuerteHnosDental"); }
        }

        private string _CausaMuerteHnosDental;

        public string CausaMuerteHnosDental
        {
            get { return _CausaMuerteHnosDental; }
            set { _CausaMuerteHnosDental = value; OnPropertyChanged("CausaMuerteHnosDental"); }
        }

        private string _CheckConyugeViveDental;

        public string CheckConyugeViveDental
        {
            get { return _CheckConyugeViveDental; }
            set { _CheckConyugeViveDental = value; OnPropertyChanged("CheckConyugeViveDental"); }
        }

        private short _TextEdadConyugeDental;

        public short TextEdadConyugeDental
        {
            get { return _TextEdadConyugeDental; }
            set { _TextEdadConyugeDental = value; OnPropertyChanged("TextEdadConyugeDental"); }
        }

        private string _CheckConyugePadeceDental;

        public string CheckConyugePadeceDental
        {
            get { return _CheckConyugePadeceDental; }
            set { _CheckConyugePadeceDental = value; OnPropertyChanged("CheckConyugePadeceDental"); }
        }

        private string _IsCheckedDiabConyDental;

        public string IsCheckedDiabConyDental
        {
            get { return _IsCheckedDiabConyDental; }
            set { _IsCheckedDiabConyDental = value; OnPropertyChanged("IsCheckedDiabConyDental"); }
        }

        private string _IsCheckedTBConyDental;

        public string IsCheckedTBConyDental
        {
            get { return _IsCheckedTBConyDental; }
            set { _IsCheckedTBConyDental = value; OnPropertyChanged("IsCheckedTBConyDental"); }
        }

        private string _IsCheckedCAConyDental;

        public string IsCheckedCAConyDental
        {
            get { return _IsCheckedCAConyDental; }
            set { _IsCheckedCAConyDental = value; OnPropertyChanged("IsCheckedCAConyDental"); }
        }

        private string _IsCheckedCardiConyDental;

        public string IsCheckedCardiConyDental
        {
            get { return _IsCheckedCardiConyDental; }
            set { _IsCheckedCardiConyDental = value; OnPropertyChanged("IsCheckedCardiConyDental"); }
        }

        private string _IsCheckedEpiConyDental;

        public string IsCheckedEpiConyDental
        {
            get { return _IsCheckedEpiConyDental; }
            set { _IsCheckedEpiConyDental = value; OnPropertyChanged("IsCheckedEpiConyDental"); }
        }

        private string _IsCheckedMentConyDental;

        public string IsCheckedMentConyDental
        {
            get { return _IsCheckedMentConyDental; }
            set { _IsCheckedMentConyDental = value; OnPropertyChanged("IsCheckedMentConyDental"); }
        }

        private string _IsCheckedAlergiConyDental;

        public string IsCheckedAlergiConyDental
        {
            get { return _IsCheckedAlergiConyDental; }
            set { _IsCheckedAlergiConyDental = value; OnPropertyChanged("IsCheckedAlergiConyDental"); }
        }

        private string _IsCheckedHipertConyDental;

        public string IsCheckedHipertConyDental
        {
            get { return _IsCheckedHipertConyDental; }
            set { _IsCheckedHipertConyDental = value; OnPropertyChanged("IsCheckedHipertConyDental"); }
        }

        private DateTime? _CuandoMuerteConyDental;

        public DateTime? CuandoMuerteConyDental
        {
            get { return _CuandoMuerteConyDental; }
            set { _CuandoMuerteConyDental = value; OnPropertyChanged("CuandoMuerteConyDental"); }
        }

        private string _CausaMuerteConyDental;

        public string CausaMuerteConyDental
        {
            get { return _CausaMuerteConyDental; }
            set { _CausaMuerteConyDental = value; OnPropertyChanged("CausaMuerteConyDental"); }
        }

        private string _CheckHijosViveDental;

        public string CheckHijosViveDental
        {
            get { return _CheckHijosViveDental; }
            set { _CheckHijosViveDental = value; OnPropertyChanged("CheckHijosViveDental"); }
        }

        private short _TextEdadesHijosDental;

        public short TextEdadesHijosDental
        {
            get { return _TextEdadesHijosDental; }
            set { _TextEdadesHijosDental = value; OnPropertyChanged("TextEdadesHijosDental"); }
        }

        private string _CheckHijosPadeceDental;

        public string CheckHijosPadeceDental
        {
            get { return _CheckHijosPadeceDental; }
            set { _CheckHijosPadeceDental = value; OnPropertyChanged("CheckHijosPadeceDental"); }
        }

        private string _IsCheckedDiabHijosDental;

        public string IsCheckedDiabHijosDental
        {
            get { return _IsCheckedDiabHijosDental; }
            set { _IsCheckedDiabHijosDental = value; OnPropertyChanged("IsCheckedDiabHijosDental"); }
        }

        private string _IsCheckedTBHijosDental;

        public string IsCheckedTBHijosDental
        {
            get { return _IsCheckedTBHijosDental; }
            set { _IsCheckedTBHijosDental = value; OnPropertyChanged("IsCheckedTBHijosDental"); }
        }

        private string _IsCheckedCAHijosDental;

        public string IsCheckedCAHijosDental
        {
            get { return _IsCheckedCAHijosDental; }
            set { _IsCheckedCAHijosDental = value; OnPropertyChanged("IsCheckedCAHijosDental"); }
        }

        private string _IsCheckedCardiHijosDental;

        public string IsCheckedCardiHijosDental
        {
            get { return _IsCheckedCardiHijosDental; }
            set { _IsCheckedCardiHijosDental = value; OnPropertyChanged("IsCheckedCardiHijosDental"); }
        }

        private string _IsCheckedEpiHijosDental;

        public string IsCheckedEpiHijosDental
        {
            get { return _IsCheckedEpiHijosDental; }
            set { _IsCheckedEpiHijosDental = value; OnPropertyChanged("IsCheckedEpiHijosDental"); }
        }

        private string _IsCheckedMentHijosDental;

        public string IsCheckedMentHijosDental
        {
            get { return _IsCheckedMentHijosDental; }
            set { _IsCheckedMentHijosDental = value; OnPropertyChanged("IsCheckedMentHijosDental"); }
        }

        private string _IsCheckedAlergiHijosDental;

        public string IsCheckedAlergiHijosDental
        {
            get { return _IsCheckedAlergiHijosDental; }
            set { _IsCheckedAlergiHijosDental = value; OnPropertyChanged("IsCheckedAlergiHijosDental"); }
        }

        private string _IsCheckedHipertHijosDental;

        public string IsCheckedHipertHijosDental
        {
            get { return _IsCheckedHipertHijosDental; }
            set { _IsCheckedHipertHijosDental = value; OnPropertyChanged("IsCheckedHipertHijosDental"); }
        }

        private DateTime? _CuandoMuerteHijosDental;

        public DateTime? CuandoMuerteHijosDental
        {
            get { return _CuandoMuerteHijosDental; }
            set { _CuandoMuerteHijosDental = value; OnPropertyChanged("CuandoMuerteHijosDental"); }
        }

        private string _CausaMuerteHijosDental;

        public string CausaMuerteHijosDental
        {
            get { return _CausaMuerteHijosDental; }
            set { _CausaMuerteHijosDental = value; OnPropertyChanged("CausaMuerteHijosDental"); }
        }


        private ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS> lstPatosDental;
        public ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS> LstPatosDental
        {
            get { return lstPatosDental; }
            set { lstPatosDental = value; OnPropertyChanged("LstPatosDental"); }
        }


        private ObservableCollection<ENFERMEDAD> lstEnfermedadesDentales;

        public ObservableCollection<ENFERMEDAD> LstEnfermedadesDentales
        {
            get { return lstEnfermedadesDentales; }
            set { lstEnfermedadesDentales = value; OnPropertyChanged("LstEnfermedadesDentales"); }
        }

        private bool _IsEnabledEnfermedadDental;
        public bool IsEnabledEnfermedadDental
        {
            get { return _IsEnabledEnfermedadDental; }
            set { _IsEnabledEnfermedadDental = value; OnPropertyChanged("IsEnabledEnfermedadDental"); }
        }

        private bool _IsEnabledTratamientoDental;
        public bool IsEnabledTratamientoDental
        {
            get { return _IsEnabledTratamientoDental; }
            set { _IsEnabledTratamientoDental = value; OnPropertyChanged("IsEnabledTratamientoDental"); }
        }
        private ObservableCollection<DENTAL_NOMENCLATURA> lstNomenclaturasDentales;

        public ObservableCollection<DENTAL_NOMENCLATURA> LstNomenclaturasDentales
        {
            get { return lstNomenclaturasDentales; }
            set { lstNomenclaturasDentales = value; OnPropertyChanged("LstNomenclaturasDentales"); }
        }

        private List<CheckBox> ListCheckBoxOdontograma;
        private List<CheckBox> LstCheckBoxSeguimiento;
        private int? _IdSelectedEnfermedadDental;

        public int? IdSelectedEnfermedadDental
        {
            get { return _IdSelectedEnfermedadDental; }
            set { _IdSelectedEnfermedadDental = value; OnPropertyChanged("IdSelectedEnfermedadDental"); }
        }

        private ObservableCollection<DENTAL_TRATAMIENTO> lstTratamientosDentales;
        public ObservableCollection<DENTAL_TRATAMIENTO> LstTratamientosDentales
        {
            get { return lstTratamientosDentales; }
            set { lstTratamientosDentales = value; OnPropertyChanged("LstTratamientosDentales"); }
        }

        private ObservableCollection<ODONTOGRAMA_SEGUIMIENTO2> lstSeguimientoDental;

        public ObservableCollection<ODONTOGRAMA_SEGUIMIENTO2> LstSeguimientoDental
        {
            get { return lstSeguimientoDental; }
            set { lstSeguimientoDental = value; OnPropertyChanged("LstSeguimientoDental"); }
        }

        private DateTime? _FechaProbTratamientoDental;

        public DateTime? FechaProbTratamientoDental
        {
            get { return _FechaProbTratamientoDental; }
            set { _FechaProbTratamientoDental = value; OnPropertyChanged("FechaProbTratamientoDental"); }
        }

        private ODONTOGRAMA_SEGUIMIENTO2 _selectedDienteSeguimiento;

        public ODONTOGRAMA_SEGUIMIENTO2 SelectedDienteSeguimiento
        {
            get { return _selectedDienteSeguimiento; }
            set { _selectedDienteSeguimiento = value; OnPropertyChanged("SelectedDienteSeguimiento"); }
        }
        private short? _IdTratamientoSeguimiento;

        public short? IdTratamientoSeguimiento
        {
            get { return _IdTratamientoSeguimiento; }
            set { _IdTratamientoSeguimiento = value; OnPropertyChanged("IdTratamientoSeguimiento"); }
        }
        private short? _IdSelectedTratamientoDental;

        public short? IdSelectedTratamientoDental
        {
            get { return _IdSelectedTratamientoDental; }
            set { _IdSelectedTratamientoDental = value; OnPropertyChanged("IdSelectedTratamientoDental"); }
        }

        private HISTORIA_CLINICA_PATOLOGICOS _SelectedPAtoDental;

        public HISTORIA_CLINICA_PATOLOGICOS SelectedPAtoDental
        {
            get { return _SelectedPAtoDental; }
            set { _SelectedPAtoDental = value; OnPropertyChanged("SelectedPAtoDental"); }
        }

        private ObservableCollection<ODONTOGRAMA_INICIAL> lstOdontogramaInicial;

        public ObservableCollection<ODONTOGRAMA_INICIAL> LstOdontogramaInicial
        {
            get { return lstOdontogramaInicial; }
            set { lstOdontogramaInicial = value; OnPropertyChanged("LstOdontogramaInicial"); }
        }

        private ODONTOGRAMA_INICIAL _SelectedDiente1;

        public ODONTOGRAMA_INICIAL SelectedDiente1
        {
            get { return _SelectedDiente1; }
            set { _SelectedDiente1 = value; OnPropertyChanged("SelectedDiente1"); }
        }

        private ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS> lstCondensadoPatosDental;

        public ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS> LstCondensadoPatosDental
        {
            get { return lstCondensadoPatosDental; }
            set { lstCondensadoPatosDental = value; OnPropertyChanged("LstCondensadoPatosDental"); }
        }

        private HISTORIA_CLINICA_PATOLOGICOS _SelectedCondensadoPatoDental;

        public HISTORIA_CLINICA_PATOLOGICOS SelectedCondensadoPatoDental
        {
            get { return _SelectedCondensadoPatoDental; }
            set { _SelectedCondensadoPatoDental = value; OnPropertyChanged("SelectedCondensadoPatoDental"); }
        }

        private bool _IsEnabledHeredoFamiliaresDental = false;

        public bool IsEnabledHeredoFamiliaresDental
        {
            get { return _IsEnabledHeredoFamiliaresDental; }
            set { _IsEnabledHeredoFamiliaresDental = value; OnPropertyChanged("IsEnabledHeredoFamiliaresDental"); }
        }

        private bool _IsEnabledOdontoSeguimientoDental = false;

        public bool IsEnabledOdontoSeguimientoDental
        {
            get { return _IsEnabledOdontoSeguimientoDental; }
            set { _IsEnabledOdontoSeguimientoDental = value; OnPropertyChanged("IsEnabledOdontoSeguimientoDental"); }
        }
        private bool _IsEnabledPatologicosDental = false;

        public bool IsEnabledPatologicosDental
        {
            get { return _IsEnabledPatologicosDental; }
            set { _IsEnabledPatologicosDental = value; OnPropertyChanged("IsEnabledPatologicosDental"); }
        }

        private bool _IsEnabledInterrogatorioDental = false;

        public bool IsEnabledInterrogatorioDental
        {
            get { return _IsEnabledInterrogatorioDental; }
            set { _IsEnabledInterrogatorioDental = value; OnPropertyChanged("IsEnabledInterrogatorioDental"); }
        }

        private bool _IsEnabledExploracionBucoDental = false;

        public bool IsEnabledExploracionBucoDental
        {
            get { return _IsEnabledExploracionBucoDental; }
            set { _IsEnabledExploracionBucoDental = value; OnPropertyChanged("IsEnabledExploracionBucoDental"); }
        }

        private bool _IsEnabledDientesDental = false;

        public bool IsEnabledDientesDental
        {
            get { return _IsEnabledDientesDental; }
            set { _IsEnabledDientesDental = value; OnPropertyChanged("IsEnabledDientesDental"); }
        }

        private bool _IsEnabledArticulacionDental = false;

        public bool IsEnabledArticulacionDental
        {
            get { return _IsEnabledArticulacionDental; }
            set { _IsEnabledArticulacionDental = value; OnPropertyChanged("IsEnabledArticulacionDental"); }
        }

        private bool _IsEnabledEnciasDental = false;

        public bool IsEnabledEnciasDental
        {
            get { return _IsEnabledEnciasDental; }
            set { _IsEnabledEnciasDental = value; OnPropertyChanged("IsEnabledEnciasDental"); }
        }

        private bool _IsEnabledBruxismoDental = false;

        public bool IsEnabledBruxismoDental
        {
            get { return _IsEnabledBruxismoDental; }
            set { _IsEnabledBruxismoDental = value; OnPropertyChanged("IsEnabledBruxismoDental"); }
        }

        private bool _IsEnabledSignosVitalesDental = false;

        public bool IsEnabledSignosVitalesDental
        {
            get { return _IsEnabledSignosVitalesDental; }
            set { _IsEnabledSignosVitalesDental = value; OnPropertyChanged("IsEnabledSignosVitalesDental"); }
        }

        private string _ComplicacionesDespuesTratamDental;

        public string ComplicacionesDespuesTratamDental
        {
            get { return _ComplicacionesDespuesTratamDental; }
            set { _ComplicacionesDespuesTratamDental = value; OnPropertyChanged("ComplicacionesDespuesTratamDental"); }
        }

        private string _HemorragiaDespuesExtracDental;

        public string HemorragiaDespuesExtracDental
        {
            get { return _HemorragiaDespuesExtracDental; }
            set { _HemorragiaDespuesExtracDental = value; OnPropertyChanged("HemorragiaDespuesExtracDental"); }
        }

        private string _TenidoReaccionNegativaDental;

        public string TenidoReaccionNegativaDental
        {
            get { return _TenidoReaccionNegativaDental; }
            set { _TenidoReaccionNegativaDental = value; OnPropertyChanged("TenidoReaccionNegativaDental"); }
        }

        private string _TomandoAlgunMedicamento;

        public string TomandoAlgunMedicamento
        {
            get { return _TomandoAlgunMedicamento; }
            set { _TomandoAlgunMedicamento = value; OnPropertyChanged("TomandoAlgunMedicamento"); }
        }

        private string _FrecuenciaCardSignosVitalesDental;

        public string FrecuenciaCardSignosVitalesDental
        {
            get { return _FrecuenciaCardSignosVitalesDental; }
            set { _FrecuenciaCardSignosVitalesDental = value; OnPropertyChanged("FrecuenciaCardSignosVitalesDental"); }
        }
        private string _ObservacionesCansancioDental;

        public string ObservacionesCansancioDental
        {
            get { return _ObservacionesCansancioDental; }
            set { _ObservacionesCansancioDental = value; OnPropertyChanged("ObservacionesCansancioDental"); }
        }

        private string _ObservacionesChasquidosDental;

        public string ObservacionesChasquidosDental
        {
            get { return _ObservacionesChasquidosDental; }
            set { _ObservacionesChasquidosDental = value; OnPropertyChanged("ObservacionesChasquidosDental"); }
        }
        private string _ObservacionesRigidezDental;

        public string ObservacionesRigidezDental
        {
            get { return _ObservacionesRigidezDental; }
            set { _ObservacionesRigidezDental = value; OnPropertyChanged("ObservacionesRigidezDental"); }
        }
        private string _ObservacionesDolorDental;

        public string ObservacionesDolorDental
        {
            get { return _ObservacionesDolorDental; }
            set { _ObservacionesDolorDental = value; OnPropertyChanged("ObservacionesDolorDental"); }
        }

        private string _CualMedicamentoTomando;

        public string CualMedicamentoTomando
        {
            get { return _CualMedicamentoTomando; }
            set { _CualMedicamentoTomando = value; OnPropertyChanged("CualMedicamentoTomando"); }
        }

        private string _AlergicoAlgunMedicamento;

        public string AlergicoAlgunMedicamento
        {
            get { return _AlergicoAlgunMedicamento; }
            set
            {
                _AlergicoAlgunMedicamento = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        CualMedicamentoAlergico = !string.IsNullOrEmpty(CualMedicamentoAlergico) ? CualMedicamentoAlergico != LeyendaNoAplica ? CualMedicamentoAlergico : string.Empty : string.Empty;
                        base.RemoveRule("CualMedicamentoAlergico");
                        base.AddRule(() => CualMedicamentoAlergico, () => !string.IsNullOrEmpty(CualMedicamentoAlergico), "CUAL MEDICAMENTO ES REQUERIDO!");
                        IsRequeridoCualMedicamento = true;
                        OnPropertyChanged("CualMedicamentoAlergico");
                        OnPropertyChanged("IsRequeridoCualMedicamento");
                    }
                    else
                    {
                        IsRequeridoCualMedicamento = true;
                        CualMedicamentoAlergico = string.Empty;
                        CualMedicamentoAlergico = LeyendaNoAplica;
                        base.RemoveRule("CualMedicamentoAlergico");
                        base.AddRule(() => CualMedicamentoAlergico, () => !string.IsNullOrEmpty(CualMedicamentoAlergico), "CUAL MEDICAMENTO ES REQUERIDO!");
                        OnPropertyChanged("CualMedicamentoAlergico");
                        OnPropertyChanged("IsRequeridoCualMedicamento");
                    }
                }
                else
                {
                    IsRequeridoCualMedicamento = false;
                    CualMedicamentoAlergico = string.Empty;
                    base.RemoveRule("CualMedicamentoAlergico");
                    OnPropertyChanged("CualMedicamentoAlergico");
                    OnPropertyChanged("IsRequeridoCualMedicamento");
                }

                OnPropertyChanged("AlergicoAlgunMedicamento");
            }
        }


        private string _LeyendaNoAplica = "NO APLICA";
        public string LeyendaNoAplica
        {
            get { return _LeyendaNoAplica; }
            set { _LeyendaNoAplica = value; OnPropertyChanged("LeyendaNoAplica"); }
        }
        private bool _IsRequeridoCualMedicamento = false;

        public bool IsRequeridoCualMedicamento
        {
            get { return _IsRequeridoCualMedicamento; }
            set { _IsRequeridoCualMedicamento = value; OnPropertyChanged("IsRequeridoCualMedicamento"); }
        }
        private string _CualMedicamentoAlergico;

        public string CualMedicamentoAlergico
        {
            get { return _CualMedicamentoAlergico; }
            set { _CualMedicamentoAlergico = value; OnPropertyChanged("CualMedicamentoAlergico"); }
        }

        private string _EstaEmbarazadaDental;

        public string EstaEmbarazadaDental
        {
            get { return _EstaEmbarazadaDental; }
            set { _EstaEmbarazadaDental = value; OnPropertyChanged("EstaEmbarazadaDental"); }
        }

        private string _AmenazaAbortoDental;

        public string AmenazaAbortoDental
        {
            get { return _AmenazaAbortoDental; }
            set { _AmenazaAbortoDental = value; OnPropertyChanged("AmenazaAbortoDental"); }
        }

        private string _LactandoDental;

        public string LactandoDental
        {
            get { return _LactandoDental; }
            set { _LactandoDental = value; OnPropertyChanged("LactandoDental"); }
        }

        private string _AlcohlisDental;

        public string AlcohlisDental
        {
            get { return _AlcohlisDental; }
            set { _AlcohlisDental = value; OnPropertyChanged("AlcohlisDental"); }
        }

        private string _TabaqDental;

        public string TabaqDental
        {
            get { return _TabaqDental; }
            set { _TabaqDental = value; OnPropertyChanged("TabaqDental"); }
        }

        private string _OtrosPresentaAlgunaDental;

        public string OtrosPresentaAlgunaDental
        {
            get { return _OtrosPresentaAlgunaDental; }
            set { _OtrosPresentaAlgunaDental = value; OnPropertyChanged("OtrosPresentaAlgunaDental"); }
        }

        private string _OtrosHipoDental;

        public string OtrosHipoDental
        {
            get { return _OtrosHipoDental; }
            set { _OtrosHipoDental = value; OnPropertyChanged("OtrosHipoDental"); }
        }

        private string _CariesDental;

        public string CariesDental
        {
            get { return _CariesDental; }
            set { _CariesDental = value; OnPropertyChanged("CariesDental"); }
        }

        private string _EspecifiqueAnomaliadForma;

        public string EspecifiqueAnomaliadForma
        {
            get { return _EspecifiqueAnomaliadForma; }
            set { _EspecifiqueAnomaliadForma = value; OnPropertyChanged("EspecifiqueAnomaliadForma"); }
        }

        private string _EspecifiqueAnomaliadTamanio;

        public string EspecifiqueAnomaliadTamanio
        {
            get { return _EspecifiqueAnomaliadTamanio; }
            set { _EspecifiqueAnomaliadTamanio = value; OnPropertyChanged("EspecifiqueAnomaliadTamanio"); }
        }

        private string _EspecifiqueHipoplas;

        public string EspecifiqueHipoplas
        {
            get { return _EspecifiqueHipoplas; }
            set { _EspecifiqueHipoplas = value; OnPropertyChanged("EspecifiqueHipoplas"); }
        }
        private string _FluoroDental;

        public string FluoroDental
        {
            get { return _FluoroDental; }
            set { _FluoroDental = value; OnPropertyChanged("FluoroDental"); }
        }

        private string _AnomTamanio;

        public string AnomTamanio
        {
            get { return _AnomTamanio; }
            set
            {
                _AnomTamanio = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        EspecifiqueAnomaliadTamanio = !string.IsNullOrEmpty(EspecifiqueAnomaliadTamanio) ? EspecifiqueAnomaliadTamanio != LeyendaNoAplica ? EspecifiqueAnomaliadTamanio : string.Empty : string.Empty;
                        base.RemoveRule("EspecifiqueAnomaliadTamanio");
                        base.AddRule(() => EspecifiqueAnomaliadTamanio, () => !string.IsNullOrEmpty(EspecifiqueAnomaliadTamanio), "CUAL ANOMALIA DE TAMANIO ES REQUERIDO!");
                        OnPropertyChanged("EspecifiqueAnomaliadTamanio");
                    }
                    else
                    {
                        EspecifiqueAnomaliadTamanio = string.Empty;
                        EspecifiqueAnomaliadTamanio = LeyendaNoAplica;
                        base.RemoveRule("EspecifiqueAnomaliadTamanio");
                        base.AddRule(() => EspecifiqueAnomaliadTamanio, () => !string.IsNullOrEmpty(EspecifiqueAnomaliadTamanio), "CUAL ANOMALIA DE TAMANIO ES REQUERIDO!");
                        OnPropertyChanged("EspecifiqueAnomaliadTamanio");
                    }
                }
                else
                {
                    EspecifiqueAnomaliadTamanio = string.Empty;
                    base.RemoveRule("EspecifiqueAnomaliadTamanio");
                    OnPropertyChanged("EspecifiqueAnomaliadTamanio");
                }

                OnPropertyChanged("AnomTamanio");
            }
        }

        private string _AnomForma;

        public string AnomForma
        {
            get { return _AnomForma; }
            set
            {
                _AnomForma = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        EspecifiqueAnomaliadForma = !string.IsNullOrEmpty(EspecifiqueAnomaliadForma) ? EspecifiqueAnomaliadForma != LeyendaNoAplica ? EspecifiqueAnomaliadForma : string.Empty : string.Empty;
                        base.RemoveRule("EspecifiqueAnomaliadForma");
                        base.AddRule(() => EspecifiqueAnomaliadForma, () => !string.IsNullOrEmpty(EspecifiqueAnomaliadForma), "CUAL ANOMALIA DE FORMA ES REQUERIDO!");
                        OnPropertyChanged("EspecifiqueAnomaliadForma");
                    }

                    else
                    {
                        EspecifiqueAnomaliadForma = string.Empty;
                        EspecifiqueAnomaliadForma = LeyendaNoAplica;
                        base.RemoveRule("EspecifiqueAnomaliadForma");
                        base.AddRule(() => EspecifiqueAnomaliadForma, () => !string.IsNullOrEmpty(EspecifiqueAnomaliadForma), "CUAL ANOMALIA DE FORMA ES REQUERIDO!");
                        OnPropertyChanged("EspecifiqueAnomaliadForma");
                    }
                }
                else
                {
                    EspecifiqueAnomaliadForma = string.Empty;
                    base.RemoveRule("EspecifiqueAnomaliadForma");
                    OnPropertyChanged("EspecifiqueAnomaliadForma");
                }

                OnPropertyChanged("AnomForma");
            }
        }

        private string _HipoPlastDental;

        public string HipoPlastDental
        {
            get { return _HipoPlastDental; }
            set
            {
                _HipoPlastDental = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        EspecifiqueHipoplas = !string.IsNullOrEmpty(EspecifiqueHipoplas) ? EspecifiqueHipoplas != LeyendaNoAplica ? EspecifiqueHipoplas : string.Empty : string.Empty;
                        base.RemoveRule("EspecifiqueHipoplas");
                        base.AddRule(() => EspecifiqueHipoplas, () => !string.IsNullOrEmpty(EspecifiqueHipoplas), "HIPOPLASIA ES REQUERIDO!");
                        OnPropertyChanged("EspecifiqueHipoplas");
                    }

                    else
                    {
                        EspecifiqueHipoplas = string.Empty;
                        EspecifiqueHipoplas = LeyendaNoAplica;
                        base.RemoveRule("EspecifiqueHipoplas");
                        base.AddRule(() => EspecifiqueHipoplas, () => !string.IsNullOrEmpty(EspecifiqueHipoplas), "HIPOPLASIA ES REQUERIDO!");
                        OnPropertyChanged("EspecifiqueHipoplas");
                    }
                }
                else
                {
                    EspecifiqueHipoplas = string.Empty;
                    base.RemoveRule("EspecifiqueHipoplas");
                    OnPropertyChanged("EspecifiqueHipoplas");
                }

                OnPropertyChanged("HipoPlastDental");
            }
        }

        private string _DolorDental;

        public string DolorDental
        {
            get { return _DolorDental; }
            set
            {
                _DolorDental = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        ObservacionesDolorDental = !string.IsNullOrEmpty(ObservacionesDolorDental) ? ObservacionesDolorDental != LeyendaNoAplica ? ObservacionesDolorDental : string.Empty : string.Empty;
                        base.RemoveRule("ObservacionesDolorDental");
                        base.AddRule(() => ObservacionesDolorDental, () => !string.IsNullOrEmpty(ObservacionesDolorDental), "ESPECIFIQUE DOLOR ES REQUERIDO!");
                        OnPropertyChanged("ObservacionesDolorDental");
                    }
                    else
                    {
                        ObservacionesDolorDental = string.Empty;
                        ObservacionesDolorDental = LeyendaNoAplica;
                        base.RemoveRule("ObservacionesDolorDental");
                        base.AddRule(() => ObservacionesDolorDental, () => !string.IsNullOrEmpty(ObservacionesDolorDental), "ESPECIFIQUE DOLOR ES REQUERIDO!");
                        OnPropertyChanged("ObservacionesDolorDental");
                    }
                }
                else
                {
                    ObservacionesDolorDental = string.Empty;
                    base.RemoveRule("ObservacionesDolorDental");
                    OnPropertyChanged("ObservacionesDolorDental");
                }

                OnPropertyChanged("DolorDental");
            }
        }

        private string _RigidezDental;

        public string RigidezDental
        {
            get { return _RigidezDental; }
            set
            {
                _RigidezDental = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        ObservacionesRigidezDental = !string.IsNullOrEmpty(ObservacionesRigidezDental) ? ObservacionesRigidezDental != LeyendaNoAplica ? ObservacionesRigidezDental : string.Empty : string.Empty;
                        base.RemoveRule("ObservacionesRigidezDental");
                        base.AddRule(() => ObservacionesRigidezDental, () => !string.IsNullOrEmpty(ObservacionesRigidezDental), "ESPECIFIQUE RIGIDEZ ES REQUERIDO!");
                        OnPropertyChanged("ObservacionesRigidezDental");
                    }
                    else
                    {
                        ObservacionesRigidezDental = string.Empty;
                        ObservacionesRigidezDental = LeyendaNoAplica;
                        base.RemoveRule("ObservacionesRigidezDental");
                        base.AddRule(() => ObservacionesRigidezDental, () => !string.IsNullOrEmpty(ObservacionesRigidezDental), "ESPECIFIQUE RIGIDEZ ES REQUERIDO!");
                        OnPropertyChanged("ObservacionesRigidezDental");
                    }
                }
                else
                {
                    ObservacionesRigidezDental = string.Empty;
                    base.RemoveRule("ObservacionesRigidezDental");
                    OnPropertyChanged("ObservacionesRigidezDental");
                }

                OnPropertyChanged("RigidezDental");
            }
        }

        private string _ChasidosDental;

        public string ChasidosDental
        {
            get { return _ChasidosDental; }
            set
            {
                _ChasidosDental = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        ObservacionesChasquidosDental = !string.IsNullOrEmpty(ObservacionesChasquidosDental) ? ObservacionesChasquidosDental != LeyendaNoAplica ? ObservacionesChasquidosDental : string.Empty : string.Empty;
                        base.RemoveRule("ObservacionesChasquidosDental");
                        base.AddRule(() => ObservacionesChasquidosDental, () => !string.IsNullOrEmpty(ObservacionesChasquidosDental), "ESPECIFIQUE CHASQUIDOS ES REQUERIDO!");
                        OnPropertyChanged("ObservacionesChasquidosDental");
                    }
                    else
                    {
                        ObservacionesChasquidosDental = string.Empty;
                        ObservacionesChasquidosDental = LeyendaNoAplica;
                        base.RemoveRule("ObservacionesChasquidosDental");
                        base.AddRule(() => ObservacionesChasquidosDental, () => !string.IsNullOrEmpty(ObservacionesChasquidosDental), "ESPECIFIQUE CHASQUIDOS ES REQUERIDO!");
                        OnPropertyChanged("ObservacionesChasquidosDental");
                    }
                }
                else
                {
                    ObservacionesChasquidosDental = string.Empty;
                    base.RemoveRule("ObservacionesChasquidosDental");
                    OnPropertyChanged("ObservacionesChasquidosDental");
                }

                OnPropertyChanged("ChasidosDental");
            }
        }

        private string _CansancioDental;

        public string CansancioDental
        {
            get { return _CansancioDental; }
            set
            {
                _CansancioDental = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        ObservacionesCansancioDental = !string.IsNullOrEmpty(ObservacionesCansancioDental) ? ObservacionesCansancioDental != LeyendaNoAplica ? ObservacionesCansancioDental : string.Empty : string.Empty;
                        base.RemoveRule("ObservacionesCansancioDental");
                        base.AddRule(() => ObservacionesCansancioDental, () => !string.IsNullOrEmpty(ObservacionesCansancioDental), "ESPECIFIQUE CANSANCIO ES REQUERIDO!");
                        OnPropertyChanged("ObservacionesCansancioDental");
                    }
                    else
                    {
                        ObservacionesCansancioDental = string.Empty;
                        ObservacionesCansancioDental = LeyendaNoAplica;
                        base.RemoveRule("ObservacionesCansancioDental");
                        base.AddRule(() => ObservacionesCansancioDental, () => !string.IsNullOrEmpty(ObservacionesCansancioDental), "ESPECIFIQUE CANSANCIO ES REQUERIDO!");
                        OnPropertyChanged("ObservacionesCansancioDental");
                    }
                }
                else
                {
                    ObservacionesCansancioDental = string.Empty;
                    base.RemoveRule("ObservacionesCansancioDental");
                    OnPropertyChanged("ObservacionesCansancioDental");
                }

                OnPropertyChanged("CansancioDental");
            }
        }

        private bool _IsEnabledCamposMujeres = false;
        public bool IsEnabledCamposMujeres
        {
            get { return _IsEnabledCamposMujeres; }
            set { _IsEnabledCamposMujeres = value; OnPropertyChanged("IsEnabledCamposMujeres"); }
        }

        private string _EnciasColorDental;

        public string EnciasColorDental
        {
            get { return _EnciasColorDental; }
            set { _EnciasColorDental = value; OnPropertyChanged("EnciasColorDental"); }
        }

        private string _EnciasFormaDental;

        public string EnciasFormaDental
        {
            get { return _EnciasFormaDental; }
            set { _EnciasFormaDental = value; OnPropertyChanged("EnciasFormaDental"); }
        }

        private string _EnciasTexturaDental;

        public string EnciasTexturaDental
        {
            get { return _EnciasTexturaDental; }
            set { _EnciasTexturaDental = value; OnPropertyChanged("EnciasTexturaDental"); }
        }

        private bool _ActivoDetalleBruxismo = false;
        public bool ActivoDetalleBruxismo
        {
            get { return _ActivoDetalleBruxismo; }
            set { _ActivoDetalleBruxismo = value; OnPropertyChanged("ActivoDetalleBruxismo"); }
        }
        private string _BruxismoEstatus;

        public string BruxismoEstatus
        {
            get { return _BruxismoEstatus; }
            set
            {
                _BruxismoEstatus = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        ActivoDetalleBruxismo = true;
                        base.RemoveRule("AfirmativoBruxismo");
                        base.AddRule(() => AfirmativoBruxismo, () => !string.IsNullOrEmpty(AfirmativoBruxismo), "BRUXISMO ES REQUERIDO!");
                        OnPropertyChanged("AfirmativoBruxismo");
                        OnPropertyChanged("ActivoDetalleBruxismo");
                    }
                    else
                    {
                        ActivoDetalleBruxismo = false;
                        AfirmativoBruxismo = string.Empty;
                        base.RemoveRule("AfirmativoBruxismo");
                        OnPropertyChanged("AfirmativoBruxismo");
                        OnPropertyChanged("ActivoDetalleBruxismo");
                    }
                }
                OnPropertyChanged("BruxismoEstatus");
            }
        }

        private int? _IdTipoImagenDental;

        public int? IdTipoImagenDental
        {
            get { return _IdTipoImagenDental; }
            set { _IdTipoImagenDental = value; OnPropertyChanged("IdTipoImagenDental"); }
        }

        private string _AfirmativoBruxismo;

        public string AfirmativoBruxismo
        {
            get { return _AfirmativoBruxismo; }
            set { _AfirmativoBruxismo = value; OnPropertyChanged("AfirmativoBruxismo"); }
        }

        private DateTime? _FechaSignosVitalesDental;

        public DateTime? FechaSignosVitalesDental
        {
            get { return _FechaSignosVitalesDental; }
            set { _FechaSignosVitalesDental = value; OnPropertyChanged("FechaSignosVitalesDental"); }
        }

        private DateTime? _HoraSignosVitalesDental;

        public DateTime? HoraSignosVitalesDental
        {
            get { return _HoraSignosVitalesDental; }
            set { _HoraSignosVitalesDental = value; OnPropertyChanged("HoraSignosVitalesDental"); }
        }

        private string _PulsoSignosVitalesDental;

        public string PulsoSignosVitalesDental
        {
            get { return _PulsoSignosVitalesDental; }
            set { _PulsoSignosVitalesDental = value; OnPropertyChanged("PulsoSignosVitalesDental"); }
        }

        private string _TemperaturaSignosVitalesDental;

        public string TemperaturaSignosVitalesDental
        {
            get { return _TemperaturaSignosVitalesDental; }
            set { _TemperaturaSignosVitalesDental = value; OnPropertyChanged("TemperaturaSignosVitalesDental"); }
        }

        private string _FrecuenciaCardiacaSignosVitalesDental;

        public string FrecuenciaCardiacaSignosVitalesDental
        {
            get { return _FrecuenciaCardiacaSignosVitalesDental; }
            set { _FrecuenciaCardiacaSignosVitalesDental = value; OnPropertyChanged("FrecuenciaCardiacaSignosVitalesDental"); }
        }

        private string _FrecuenciaRespSignosVitalesDental;

        public string FrecuenciaRespSignosVitalesDental
        {
            get { return _FrecuenciaRespSignosVitalesDental; }
            set { _FrecuenciaRespSignosVitalesDental = value; OnPropertyChanged("FrecuenciaRespSignosVitalesDental"); }
        }

        private string _GlicemiaSignosVitalesDental;

        public string GlicemiaSignosVitalesDental
        {
            get { return _GlicemiaSignosVitalesDental; }
            set { _GlicemiaSignosVitalesDental = value; OnPropertyChanged("GlicemiaSignosVitalesDental"); }
        }

        private string _PesoSignosVitalesDental;

        public string PesoSignosVitalesDental
        {
            get { return _PesoSignosVitalesDental; }
            set { _PesoSignosVitalesDental = value; OnPropertyChanged("PesoSignosVitalesDental"); }
        }

        private string _EstaturaSignosVitalesDental;

        public string EstaturaSignosVitalesDental
        {
            get { return _EstaturaSignosVitalesDental; }
            set { _EstaturaSignosVitalesDental = value; OnPropertyChanged("EstaturaSignosVitalesDental"); }
        }

        private bool _IsEnabledOdontoInicialDental = false;

        public bool IsEnabledOdontoInicialDental
        {
            get { return _IsEnabledOdontoInicialDental; }
            set { _IsEnabledOdontoInicialDental = value; OnPropertyChanged("IsEnabledOdontoInicialDental"); }
        }

        private string _EspecifiqueLabiosDental;

        public string EspecifiqueLabiosDental
        {
            get { return _EspecifiqueLabiosDental; }
            set { _EspecifiqueLabiosDental = value; OnPropertyChanged("EspecifiqueLabiosDental"); }
        }

        private string _EspecifiqueLenguaDental;

        public string EspecifiqueLenguaDental
        {
            get { return _EspecifiqueLenguaDental; }
            set { _EspecifiqueLenguaDental = value; OnPropertyChanged("EspecifiqueLenguaDental"); }
        }

        private string _EspecifiqueMucosaDental;

        public string EspecifiqueMucosaDental
        {
            get { return _EspecifiqueMucosaDental; }
            set { _EspecifiqueMucosaDental = value; OnPropertyChanged("EspecifiqueMucosaDental"); }
        }

        private string _EspecifiqueAmigdalasDental;

        public string EspecifiqueAmigdalasDental
        {
            get { return _EspecifiqueAmigdalasDental; }
            set { _EspecifiqueAmigdalasDental = value; OnPropertyChanged("EspecifiqueAmigdalasDental"); }
        }

        private string _EspecifiquePisoBocaDental;

        public string EspecifiquePisoBocaDental
        {
            get { return _EspecifiquePisoBocaDental; }
            set { _EspecifiquePisoBocaDental = value; OnPropertyChanged("EspecifiquePisoBocaDental"); }
        }

        private string _EspecifiquePaladarDuroDental;

        public string EspecifiquePaladarDuroDental
        {
            get { return _EspecifiquePaladarDuroDental; }
            set { _EspecifiquePaladarDuroDental = value; OnPropertyChanged("EspecifiquePaladarDuroDental"); }
        }

        private string _EspecifiquePaladarBlancoDental;

        public string EspecifiquePaladarBlancoDental
        {
            get { return _EspecifiquePaladarBlancoDental; }
            set { _EspecifiquePaladarBlancoDental = value; OnPropertyChanged("EspecifiquePaladarBlancoDental"); }
        }

        private string _EspecifiqueCarrillosDental;

        public string EspecifiqueCarrillosDental
        {
            get { return _EspecifiqueCarrillosDental; }
            set { _EspecifiqueCarrillosDental = value; OnPropertyChanged("EspecifiqueCarrillosDental"); }
        }

        private string _EspecifiqueFrenillosDental;

        public string EspecifiqueFrenillosDental
        {
            get { return _EspecifiqueFrenillosDental; }
            set { _EspecifiqueFrenillosDental = value; OnPropertyChanged("EspecifiqueFrenillosDental"); }
        }

        private string _EspecifiqueOtrosDental;

        public string EspecifiqueOtrosDental
        {
            get { return _EspecifiqueOtrosDental; }
            set { _EspecifiqueOtrosDental = value; OnPropertyChanged("EspecifiqueOtrosDental"); }
        }


        private ObservableCollection<HISTORIA_CLINICA_DENTAL_DOCUME> lstDocumentosDentales;

        public ObservableCollection<HISTORIA_CLINICA_DENTAL_DOCUME> LstDocumentosDentales
        {
            get { return lstDocumentosDentales; }
            set { lstDocumentosDentales = value; OnPropertyChanged("LstDocumentosDentales"); }
        }

        private bool _IsEnabledHistoriaClinicaDental = true;
        public bool IsEnabledHistoriaClinicaDental
        {
            get { return _IsEnabledHistoriaClinicaDental; }
            set { _IsEnabledHistoriaClinicaDental = value; OnPropertyChanged("IsEnabledHistoriaClinicaDental"); }
        }

        private bool _IsEnabledSignosVitalesDentales = true;

        public bool IsEnabledSignosVitalesDentales
        {
            get { return _IsEnabledSignosVitalesDentales; }
            set { _IsEnabledSignosVitalesDentales = value; OnPropertyChanged("IsEnabledSignosVitalesDentales"); }
        }
        private bool _SubirImagenesDental = true;

        public bool SubirImagenesDental
        {
            get { return _SubirImagenesDental; }
            set { _SubirImagenesDental = value; OnPropertyChanged("SubirImagenesDental"); }
        }

        private bool _IsEnabledBotonesProcesoOdontogramas = true;

        public bool IsEnabledBotonesProcesoOdontogramas
        {
            get { return _IsEnabledBotonesProcesoOdontogramas; }
            set { _IsEnabledBotonesProcesoOdontogramas = value; OnPropertyChanged("IsEnabledBotonesProcesoOdontogramas"); }
        }

        #endregion
    }
}
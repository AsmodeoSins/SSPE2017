using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {
        #region [Conductas Parasociales]
        private EMI_USO_DROGA EMI_USO_DROGA = new EMI_USO_DROGA();
        private EMI_HPS EMI_HPS = new EMI_HPS();
        private EMI_TATUAJE EMI_TATUAJE = new EMI_TATUAJE();
        private EMI_ENFERMEDAD EMI_ENFERMEDAD = new EMI_ENFERMEDAD();
        private ObservableCollection<PANDILLA> lstPandillas;
        private ObservableCollection<MOTIVO_PROSTITUCION> lstMotivosProstituye;
        private ObservableCollection<COMPORTAMIENTO_HOMO> lstComportamientoHomo;
        private ObservableCollection<DROGA> lstDrogas;
        //private List<DROGA_FRECUENCIA> lstFrecuenciasUsoDrogas;
        private EMI_USO_DROGA _NewObjUsoDroga = new EMI_USO_DROGA();
        private EMI_USO_DROGA _SelectedUsoDroga;
        private List<EMI_USO_DROGA> lstUsosEliminar = new System.Collections.Generic.List<EMI_USO_DROGA>();
        #region [Uso de Drogas]
        #region [Uso de Drogas]
        private ObservableCollection<EMI_USO_DROGA> lstUsoDrogas;
        public ObservableCollection<EMI_USO_DROGA> LstUsoDrogas
        {
            get { return lstUsoDrogas; }
            set { lstUsoDrogas = value; OnPropertyChanged("LstUsoDrogas"); }
        }

        public short Droga
        {
            get { return EMI_USO_DROGA.ID_DROGA; }
            set
            {
                EMI_USO_DROGA.ID_DROGA = value;
                OnPropertyChanged("Droga");
            }
        }
        public short? EdadIncio
        {
            get { return EMI_USO_DROGA.EDAD_INICIO; }
            set
            {
                EMI_USO_DROGA.EDAD_INICIO = value;
                OnPropertyChanged("EdadIncio");
            }
        }
        public DateTime? FechaUltimaDosis
        {
            get { return EMI_USO_DROGA.FEC_ULTIMA_DOSIS; }
            set
            {
                EMI_USO_DROGA.FEC_ULTIMA_DOSIS = value;
                OnPropertyChanged("FechaUltimaDosis");
            }
        }
        public short? FrecuenciaUso
        {
            get { return EMI_USO_DROGA.FRECUENCIA_USO; }
            set
            {
                EMI_USO_DROGA.FRECUENCIA_USO = value;
                OnPropertyChanged("FrecuenciaUso");
            }
        }
        public string ConsumoActual
        {
            get { return EMI_USO_DROGA.CONSUMO_ACTUAL; }
            set
            {
                EMI_USO_DROGA.CONSUMO_ACTUAL = value;
                OnPropertyChanged("ConsumoActual");
            }
        }
        public short? TiempoConsumo
        {
            get { return EMI_USO_DROGA.TIEMPO_CONSUMO; }
            set
            {
                EMI_USO_DROGA.TIEMPO_CONSUMO = value;
                OnPropertyChanged("TiempoConsumo");
            }
        }
        public ObservableCollection<DROGA> LstDrogas
        {
            get { return lstDrogas; }
            set
            {
                lstDrogas = value;
                OnPropertyChanged("LstDrogas");
            }
        }

        private ObservableCollection<DROGA_FRECUENCIA> _lstFrecuenciasUsoDrogas;
        public ObservableCollection<DROGA_FRECUENCIA> LstFrecuenciasUsoDrogas
        {
            get { return _lstFrecuenciasUsoDrogas; }
            set
            {
                _lstFrecuenciasUsoDrogas = value;
                OnPropertyChanged("LstFrecuenciasUsoDrogas");
            }
        }

        public EMI_USO_DROGA SelectedUsoDroga
        {
            get { return _SelectedUsoDroga; }
            set
            {
                _SelectedUsoDroga = value;

                if (_SelectedUsoDroga != null)
                {
                    var isInDB = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIUsoDroga().GetData(w => w.ID_EMI == emiActual.ID_EMI && w.ID_EMI_CONS == emiActual.ID_EMI_CONS && w.ID_DROGA == SelectedUsoDroga.ID_DROGA);
                    EliminarItemMenu = isInDB.Count() > 0 ? false : true;
                }
                OnPropertyChanged("SelectedUsoDroga");
            }
        }
        #endregion
        #region PopUpAddEdit Uso drogas
        public EMI_USO_DROGA popUpUsoDroga
        {
            get { return _NewObjUsoDroga; }
            set
            {
                _NewObjUsoDroga = value;
                OnPropertyChanged("popUpUsoDroga");
            }
        }
        public short popUpDrogaId
        {
            get
            {
                if (popUpUsoDroga != null)
                    return popUpUsoDroga.ID_DROGA;
                else
                    return -1;
            }
            set
            {
                if (popUpUsoDroga != null)
                    popUpUsoDroga.ID_DROGA = value;
                else
                    popUpUsoDroga.ID_DROGA = -1;
                OnPropertyChanged("popUpDrogaId");

            }
        }
        public short? popUpEdadInicio
        {
            get
            {

                if (popUpUsoDroga != null)
                    return popUpUsoDroga.EDAD_INICIO;
                else
                    return null;
            }
            set
            {
                if (popUpUsoDroga != null)
                    popUpUsoDroga.EDAD_INICIO = value;
                OnPropertyChanged("popUpEdadInicio");
            }
        }
        //private DateTime? _popUpFechaUltDosis;
        public DateTime? popUpFechaUltDosis
        {
            get
            {
                return popUpUsoDroga.FEC_ULTIMA_DOSIS;
            }
            set
            {
                if (value != null)
                    popUpUsoDroga.FEC_ULTIMA_DOSIS = value;
                OnPropertyChanged("popUpFechaUltDosis");
            }
        }

        private DateTime? fechaTest;
        public DateTime? FechaTest
        {
            get { return fechaTest; }
            set { fechaTest = value; OnPropertyChanged("FechaTest"); }
        }

        //public short? popUpFrecuenciaUso
        //{
        //    get
        //    {
        //        if (popUpUsoDroga != null)
        //            return popUpUsoDroga.FRECUENCIA_USO;
        //        else
        //            return null;
        //        }
        //    set
        //    {
        //        if (popUpUsoDroga != null)
        //        popUpUsoDroga.FRECUENCIA_USO = value;
        //        OnPropertyChanged("popUpFrecuenciaUso");
        //    }
        //}

        public short? popUpFrecuenciaUso
        {
            get { return popUpUsoDroga.FRECUENCIA_USO; }
            set
            {
                if (value != null)
                    popUpUsoDroga.FRECUENCIA_USO = value;
                OnPropertyChanged("popUpFrecuenciaUso");
            }
        }

        public string popUpConsumoActual
        {
            get
            {
                if (popUpUsoDroga != null)
                    return popUpUsoDroga.CONSUMO_ACTUAL;
                else
                    return string.Empty;
            }
            set
            {
                if (popUpUsoDroga != null)
                    popUpUsoDroga.CONSUMO_ACTUAL = value;
                OnPropertyChanged("popUpConsumoActual");
            }
        }
        public short? popUpTiempoConsumo
        {
            get
            {
                if (popUpUsoDroga != null)
                    return popUpUsoDroga.TIEMPO_CONSUMO;
                else
                    return null;
            }
            set
            {
                if (popUpUsoDroga != null)
                    popUpUsoDroga.TIEMPO_CONSUMO = value;
                OnPropertyChanged("popUpTiempoConsumo");
            }
        }
        #endregion

        #region [Homosexualidad, Pandillas, Sexualidad]
        #region [Conducta Parasocial]
        public string VivioCalleOrfanato
        {
            get { return EMI_HPS.VIVIO_CALLE_ORFANATO_NINO; }
            set
            {
                EMI_HPS.VIVIO_CALLE_ORFANATO_NINO = value;
                OnPropertyChanged("VivioCalleOrfanato");
            }
        }
        public string PertenecePandilla
        {
            get { return EMI_HPS.PERTENECE_PANDILLA_ACTUAL; }
            set
            {
                EMI_HPS.PERTENECE_PANDILLA_ACTUAL = value;
                //PandillaNombre = PertenecePandilla == "S" ? -1 : new Nullable<short>();
                PandillaNombre = -1;
                PandillaEnabled = PertenecePandilla == "S";
                setValidacionesHPS();
                OnPropertyChanged("PertenecePandilla");
            }
        }

        public ObservableCollection<PANDILLA> LstPandillas
        {
            get { return lstPandillas; }
            set
            {
                lstPandillas = value;
                OnPropertyChanged("LstPandillas");
            }
        }
        public short? PandillaNombre
        {
            get { return EMI_HPS.ID_PANDILLA; }
            set
            {
                EMI_HPS.ID_PANDILLA = value;
                OnPropertyChanged("PandillaNombre");
            }
        }
        #region [Conducta]
        //public string Homosexual
        //{
        //    get { return EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL; }
        //    set
        //    {
        //        EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL = value;
        //        OnPropertyChanged("Homosexual");
        //    }
        //}


        private string nombrePandilla;

        public string NombrePandilla
        {
            get { return nombrePandilla; }
            set { nombrePandilla = value; OnPropertyChanged("NombrePandilla"); }
        }

        private string homosexual;

        public string Homosexual
        {
            get { return homosexual; }
            set
            {
                homosexual = value;
                if (homosexual != null)
                {
                    ComportamientoHomosexualEnabled = true;
                    HomosexualRol = "";

                    if (homosexual == "S") { }
                    else if (homosexual == "N")
                    {
                        ComportamientoHomosexualEnabled = false;
                        HomosexualRol = null;
                        Id_Homo = -1;
                    }
                    else
                        ComportamientoHomosexualEnabled = false;
                    HomosexualEdadIncial = 0;
                    HomosexualRol = string.Empty;
                }
                setValidacionesHPS();
                OnPropertyChanged("Homosexual");
            }
        }
        public short? HomosexualEdadIncial
        {
            get { return EMI_HPS.EDAD_INICIAL_HOMOSEXUAL; }
            set
            {
                EMI_HPS.EDAD_INICIAL_HOMOSEXUAL = value;
                OnPropertyChanged("HomosexualEdadIncial");
            }
        }
        public string HomosexualRol
        {
            get { return EMI_HPS.ROL_HOMOSEXUAL; }
            set
            {
                EMI_HPS.ROL_HOMOSEXUAL = value;
                OnPropertyChanged("HomosexualRol");
            }
        }
        public string PertenecioPandillaExterior
        {
            get { return EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR; }
            set
            {
                EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR = value;
                if (EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR != null)
                {
                    if (EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR.Equals("S"))
                    {
                        PertenecioPandillaEnabled = true;
                    }
                    else
                    {
                        PertenecioPandillaEnabled = false;
                        PandillaExteriorEdadInicial = 0;
                        PandillaExteriorMotivo = string.Empty;
                    }
                }
                setValidacionesHPS();
                OnPropertyChanged("PertenecioPandillaExterior");
            }
        }
        public short? PandillaExteriorEdadInicial
        {
            get { return EMI_HPS.EDAD_INICIAL_PANDILLAS; }
            set
            {
                EMI_HPS.EDAD_INICIAL_PANDILLAS = value;
                OnPropertyChanged("PandillaExteriorEdadInicial");
            }
        }
        public string PandillaExteriorMotivo
        {
            get { return EMI_HPS.MOTIVOS_PERTENENCIA_PANDILLAS; }
            set
            {
                EMI_HPS.MOTIVOS_PERTENENCIA_PANDILLAS = value;
                OnPropertyChanged("PandillaExteriorMotivo");
            }
        }
        public string Vagancia
        {
            get { return EMI_HPS.VAGANCIA; }
            set
            {
                EMI_HPS.VAGANCIA = value;
                if (EMI_HPS.VAGANCIA != null)
                {
                    if (EMI_HPS.VAGANCIA.Equals("S"))
                    {
                        VaganciaEnabled = true;
                    }
                    else
                    {
                        VaganciaEnabled = false;
                        VaganciaEdadIncial = 0;
                        VaganciaMotivos = string.Empty;
                    }
                }
                setValidacionesHPS();
                OnPropertyChanged("Vagancia");
            }
        }
        public short? VaganciaEdadIncial
        {
            get { return EMI_HPS.EDAD_INICIAL_VAGANCIA; }
            set
            {
                EMI_HPS.EDAD_INICIAL_VAGANCIA = value;
                OnPropertyChanged("VaganciaEdadIncial");
            }
        }
        public string VaganciaMotivos
        {
            get { return EMI_HPS.MOTIVOS_VAGANCIA; }
            set
            {
                EMI_HPS.MOTIVOS_VAGANCIA = value;
                OnPropertyChanged("VaganciaMotivos");
            }
        }
        public string Cicatrices
        {
            get { return EMI_HPS.CICATRICES; }
            set
            {
                EMI_HPS.CICATRICES = value;
                if (EMI_HPS.CICATRICES != null)
                {
                    if (EMI_HPS.CICATRICES.Equals("S"))
                    {
                        CicatricesEnabled = true;
                    }
                    else
                    {
                        CicatricesEnabled = false;
                        CicatricesEdadIncial = 0;
                        CicatricesMotivo = string.Empty;
                        CicatricesRina = false;
                    }
                }
                setValidacionesHPS();
                OnPropertyChanged("Cicatrices");
            }
        }
        public short? CicatricesEdadIncial
        {
            get { return EMI_HPS.EDAD_INICIO_CICATRICES; }
            set
            {
                EMI_HPS.EDAD_INICIO_CICATRICES = value;
                OnPropertyChanged("CicatricesEdadIncial");
            }
        }
        public string CicatricesMotivo
        {
            get { return EMI_HPS.MOTIVO_CICATRICES; }
            set
            {
                EMI_HPS.MOTIVO_CICATRICES = value;
                OnPropertyChanged("CicatricesMotivo");
            }
        }
        public bool CicatricesRina
        {
            get { return EMI_HPS.CICATRIZ_POR_RINA == "S" ? true : false; }
            set
            {
                EMI_HPS.CICATRIZ_POR_RINA = value == true ? "S" : "N";
                OnPropertyChanged("CicatricesRina");
            }
        }
        public string DesercionEscolar
        {
            get { return EMI_HPS.DESERCION_ESCOLAR; }
            set
            {
                EMI_HPS.DESERCION_ESCOLAR = value;
                if (EMI_HPS.DESERCION_ESCOLAR != null)
                {
                    if (EMI_HPS.DESERCION_ESCOLAR.Equals("S"))
                    {
                        DesercionEscolarEnabled = true;
                    }
                    else
                    {
                        DesercionEscolarEnabled = false;
                        DesercionMotivo = string.Empty;
                    }
                }
                setValidacionesHPS();
                OnPropertyChanged("DesercionEscolar");
            }
        }
        public string DesercionMotivo
        {
            get { return EMI_HPS.MOTIVO_DESERCION_ESCOLAR; }
            set
            {
                EMI_HPS.MOTIVO_DESERCION_ESCOLAR = value;
                OnPropertyChanged("DesercionMotivo");
            }
        }

        public string ReprobacionEscolar
        {
            get { return EMI_HPS.REPROBACION_ESCOLAR; }
            set
            {
                EMI_HPS.REPROBACION_ESCOLAR = value;
                if (EMI_HPS.REPROBACION_ESCOLAR != null)
                {
                    if (EMI_HPS.REPROBACION_ESCOLAR.Equals("S"))
                    {
                        ReprobacionEscolarEnabled = true;
                    }
                    else
                    {
                        ReprobacionEscolarEnabled = false;
                        ReprobacionEscolarMotivo = string.Empty;
                    }
                    ReprobacionGrado = -1;
                }
                setValidacionesHPS();
                OnPropertyChanged("ReprobacionEscolar");
            }
        }
        public string ReprobacionEscolarMotivo
        {
            get { return EMI_HPS.MOTIVO_REPROBACION_ESCOLAR; }
            set
            {
                EMI_HPS.MOTIVO_REPROBACION_ESCOLAR = value;
                OnPropertyChanged("ReprobacionEscolarMotivo");
            }
        }

        public short? ReprobacionGrado
        {
            get { return EMI_HPS.GRADO_REPROBACION_ESCOLAR; }
            set
            {
                EMI_HPS.GRADO_REPROBACION_ESCOLAR = value;
                OnPropertyChanged("ReprobacionGrado");
            }
        }
        public string ExplusionEscolar
        {
            get { return EMI_HPS.EXPULSION_ESCOLAR; }
            set
            {
                EMI_HPS.EXPULSION_ESCOLAR = value;
                if (EMI_HPS.EXPULSION_ESCOLAR != null)
                {
                    if (EMI_HPS.EXPULSION_ESCOLAR.Equals("S"))
                    {
                        ExpulsionEscolarEnabled = true;
                    }
                    else
                    {
                        ExpulsionEscolarEnabled = false;
                        ExplusionEscolarMotivo = string.Empty;
                    }
                    ExpulsionGrado = -1;
                }
                setValidacionesHPS();
                OnPropertyChanged("ExplusionEscolar");
            }
        }
        public string ExplusionEscolarMotivo
        {
            get { return EMI_HPS.MOTIVO_EXPULSION_ESCOLAR; }
            set
            {
                EMI_HPS.MOTIVO_EXPULSION_ESCOLAR = value;
                OnPropertyChanged("ExplusionEscolarMotivo");
            }
        }
        public short? ExpulsionGrado
        {
            get { return EMI_HPS.GRADO_EXPULSION_ESCOLAR; }
            set
            {
                EMI_HPS.GRADO_EXPULSION_ESCOLAR = value;
                OnPropertyChanged("ExpulsionGrado");
            }
        }

        //VARIABLES VALIDACIONES
        private bool comportamientoHomosexualEnabled;
        public bool ComportamientoHomosexualEnabled
        {
            get { return comportamientoHomosexualEnabled; }
            set { comportamientoHomosexualEnabled = value; OnPropertyChanged("ComportamientoHomosexualEnabled"); }
        }

        private bool pertenecioPandillaEnabled;
        public bool PertenecioPandillaEnabled
        {
            get { return pertenecioPandillaEnabled; }
            set { pertenecioPandillaEnabled = value; OnPropertyChanged("PertenecioPandillaEnabled"); }
        }

        private bool vaganciaEnabled;
        public bool VaganciaEnabled
        {
            get { return vaganciaEnabled; }
            set { vaganciaEnabled = value; OnPropertyChanged("VaganciaEnabled"); }
        }

        private bool cicatricesEnabled;
        public bool CicatricesEnabled
        {
            get { return cicatricesEnabled; }
            set { cicatricesEnabled = value; OnPropertyChanged("CicatricesEnabled"); }
        }

        private bool desercionEscolarEnabled;
        public bool DesercionEscolarEnabled
        {
            get { return desercionEscolarEnabled; }
            set { desercionEscolarEnabled = value; OnPropertyChanged("DesercionEscolarEnabled"); }
        }

        private bool reprobacionEscolarEnabled;
        public bool ReprobacionEscolarEnabled
        {
            get { return reprobacionEscolarEnabled; }
            set { reprobacionEscolarEnabled = value; OnPropertyChanged("ReprobacionEscolarEnabled"); }
        }

        private bool expulsionEscolarEnabled;
        public bool ExpulsionEscolarEnabled
        {
            get { return expulsionEscolarEnabled; }
            set { expulsionEscolarEnabled = value; OnPropertyChanged("ExpulsionEscolarEnabled"); }
        }
        #endregion
        #region [Pagaba por Servicio Sexual]
        public bool ConHombres
        {
            get { return EMI_HPS.PAGA_SEXUAL_HOMBRE == "S" ? true : false; }
            set
            {
                EMI_HPS.PAGA_SEXUAL_HOMBRE = value == true ? "S" : "N";
                OnPropertyChanged("ConHombres");
            }
        }
        public bool ConMujeres
        {
            get { return EMI_HPS.PAGA_SEXUAL_MUJER == "S" ? true : false; }
            set
            {
                EMI_HPS.PAGA_SEXUAL_MUJER = value == true ? "S" : "N";
                OnPropertyChanged("ConMujeres");
            }
        }
        #endregion
        #region Se Prostituia
        public bool SeProstituiaHombres
        {
            get { return EMI_HPS.PROSTITUIA_HOMBRES == "S" ? true : false; }
            set
            {
                EMI_HPS.PROSTITUIA_HOMBRES = value == true ? "S" : "N";
                OnPropertyChanged("SeProstituiaHombres");
            }

        }
        public bool SeProstituiaMujeres
        {
            get { return EMI_HPS.PROSTITUIA_MUJERES == "S" ? true : false; }
            set
            {
                EMI_HPS.PROSTITUIA_MUJERES = value == true ? "S" : "N";
                OnPropertyChanged("SeProstituiaMujeres");
            }

        }
        public short? MotivoProstituye
        {
            get { return EMI_HPS.PROSTITUYE_POR; }
            set
            {
                EMI_HPS.PROSTITUYE_POR = value;
                OnPropertyChanged("MotivoProstituye");
            }

        }
        public ObservableCollection<MOTIVO_PROSTITUCION> LstMotivosProstituye
        {
            get { return lstMotivosProstituye; }
            set
            {
                lstMotivosProstituye = value;
                OnPropertyChanged("LstMotivosProstituye");
            }

        }

        public ObservableCollection<COMPORTAMIENTO_HOMO> LstComportamientoHomo
        {
            get { return lstComportamientoHomo; }
            set
            {
                lstComportamientoHomo = value;
                OnPropertyChanged("LstComportamientoHomo");
            }
        }

        public short? Id_Homo
        {
            get { return EMI_HPS.ID_HOMO; }
            set
            {
                EMI_HPS.ID_HOMO = value;
                OnPropertyChanged("Id_Homo");
            }

        }

        #endregion
        #endregion
        #endregion

        #region [Tatuajes]
        #region [Tatuajes]
        public short? CantidadAntesIngresoAntisocial
        {
            get { return EMI_TATUAJE.ANTISOCIAL_AI == null ? 0 : EMI_TATUAJE.ANTISOCIAL_AI; }
            set
            {
                EMI_TATUAJE.ANTISOCIAL_AI = value == null ? 0 : value;
                OnPropertyChanged("CantidadAntesIngresoAntisocial");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadIntramurosAntisocial
        {
            get { return EMI_TATUAJE.ANTISOCIAL_I == null ? 0 : EMI_TATUAJE.ANTISOCIAL_I; }
            set
            {
                EMI_TATUAJE.ANTISOCIAL_I = value == null ? 0 : value;
                OnPropertyChanged("CantidadIntramurosAntisocial");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadAntesIngresoErotico
        {
            get { return EMI_TATUAJE.EROTICO_AI == null ? 0 : EMI_TATUAJE.EROTICO_AI; }
            set
            {
                EMI_TATUAJE.EROTICO_AI = value == null ? 0 : value;
                OnPropertyChanged("CantidadAntesIngresoErotico");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadIntramurosErotico
        {
            get { return EMI_TATUAJE.EROTICO_I == null ? 0 : EMI_TATUAJE.EROTICO_I; }
            set
            {
                EMI_TATUAJE.EROTICO_I = value == null ? 0 : value;
                OnPropertyChanged("CantidadIntramurosErotico");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadAntesIngresoReligioso
        {
            get { return EMI_TATUAJE.RELIGIOSO_AI == null ? 0 : EMI_TATUAJE.RELIGIOSO_AI; }
            set
            {
                EMI_TATUAJE.RELIGIOSO_AI = value == null ? 0 : value;
                OnPropertyChanged("CantidadAntesIngresoReligioso");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadIntramurosReligioso
        {
            get { return EMI_TATUAJE.RELIGIOSO_I == null ? 0 : EMI_TATUAJE.RELIGIOSO_I; }
            set
            {
                EMI_TATUAJE.RELIGIOSO_I = value == null ? 0 : value;
                OnPropertyChanged("CantidadIntramurosReligioso");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadAntesIngresoIdentificacion
        {
            get { return EMI_TATUAJE.IDENTIFICACION_AI == null ? 0 : EMI_TATUAJE.IDENTIFICACION_AI; }
            set
            {
                EMI_TATUAJE.IDENTIFICACION_AI = value == null ? 0 : value;
                OnPropertyChanged("CantidadAntesIngresoIdentificacion");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadIntramurosIdentificacion
        {
            get { return EMI_TATUAJE.IDENTIFICACION_I == null ? 0 : EMI_TATUAJE.IDENTIFICACION_I; }
            set
            {
                EMI_TATUAJE.IDENTIFICACION_I = value == null ? 0 : value;
                OnPropertyChanged("CantidadIntramurosIdentificacion");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadAntesIngresoDecorativo
        {
            get { return EMI_TATUAJE.DECORATIVO_AI == null ? 0 : EMI_TATUAJE.DECORATIVO_AI; }
            set
            {
                EMI_TATUAJE.DECORATIVO_AI = value == null ? 0 : value;
                OnPropertyChanged("CantidadAntesIngresoDecorativo");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadIntramurosDecorativo
        {
            get { return EMI_TATUAJE.DECORATIVO_I == null ? 0 : EMI_TATUAJE.DECORATIVO_I; }
            set
            {
                EMI_TATUAJE.DECORATIVO_I = value == null ? 0 : value;
                OnPropertyChanged("CantidadIntramurosDecorativo");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadAntesIngresoSentimental
        {
            get { return EMI_TATUAJE.SENTIMENTAL_AI == null ? 0 : EMI_TATUAJE.SENTIMENTAL_AI; }
            set
            {
                EMI_TATUAJE.SENTIMENTAL_AI = value == null ? 0 : value;
                OnPropertyChanged("CantidadAntesIngresoSentimental");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public short? CantidadIntramurosSentimental
        {
            get { return EMI_TATUAJE.SENTIMENTAL_I == null ? 0 : EMI_TATUAJE.SENTIMENTAL_I; }
            set
            {
                EMI_TATUAJE.SENTIMENTAL_I = value == null ? 0 : value;
                OnPropertyChanged("CantidadIntramurosSentimental");
                TatuajesTotal = ContarTatuajes();
                OnPropertyChanged("TatuajesTotal");
            }
        }
        public int? TatuajesTotal
        {
            get { return EMI_TATUAJE.TOTAL_TATUAJES; }
            set
            {
                if (value != null)
                {
                    if (value != ContarTatuajes())
                        TatuajesTotal = ContarTatuajes();
                    EMI_TATUAJE.TOTAL_TATUAJES = (short)value /*!= ContarTatuajes() ? short.Parse(ContarTatuajes().ToString()) : short.Parse(value.ToString())*/;
                    OnPropertyChanged("TatuajesTotal");
                }
            }
        }
        public string TatuajesDescripcion
        {
            get { return EMI_TATUAJE.DESCR; }
            set
            {
                EMI_TATUAJE.DESCR = value;
                OnPropertyChanged("TatuajesDescripcion");
                //TatuajesTotal = ContarTatuajes();
                //OnPropertyChanged("TatuajesTotal");
            }
        }
        #endregion
        #endregion
       
        #region [Enfermedades]
        #region [Descripcion de Enfermedades Cronicas, Degenerativas, e Infectocontagiosas]
        #region [Descripcion en Caso de Presentarlas o Tener Antecedentes]
        public string DescripcionPresentarlasAntecedentes
        {
            get { return EMI_ENFERMEDAD.DESCR_ENFERMEDAD; }
            set
            {
                EMI_ENFERMEDAD.DESCR_ENFERMEDAD = value;
                OnPropertyChanged("DescripcionPresentarlasAntecedentes");
            }
        }
        #endregion
        #region [Apariencia Fisica]
        public string AparienciaFisicaAlineado
        {
            get { return EMI_ENFERMEDAD.APFISICA_ALINADO; }
            set
            {
                EMI_ENFERMEDAD.APFISICA_ALINADO = value;
                OnPropertyChanged("AparienciaFisicaAlineado");
            }
        }
        public string AparienciaFisicaConformado
        {
            get { return EMI_ENFERMEDAD.APFISICA_CONFORMADO; }
            set
            {
                EMI_ENFERMEDAD.APFISICA_CONFORMADO = value;
                OnPropertyChanged("AparienciaFisicaConformado");
            }
        }
        public string AparienciaFisicaIntegro
        {
            get { return EMI_ENFERMEDAD.APFISICA_INTEGRO; }
            set
            {
                EMI_ENFERMEDAD.APFISICA_INTEGRO = value;
                OnPropertyChanged("AparienciaFisicaIntegro");
            }
        }
        public string AparienciaFisicaLimpio
        {
            get { return EMI_ENFERMEDAD.APFISICA_LIMPIO; }
            set
            {
                EMI_ENFERMEDAD.APFISICA_LIMPIO = value;
                OnPropertyChanged("AparienciaFisicaLimpio");
            }
        }
        #endregion
        #region [Especifique]
        public string Discapacidades
        {
            get { return EMI_ENFERMEDAD.DISCAPACIDAD; }
            set
            {
                EMI_ENFERMEDAD.DISCAPACIDAD = value;
                if (EMI_ENFERMEDAD.DISCAPACIDAD != null)
                {
                    if (base.FindRule("Discapacidades"))
                    if (EMI_ENFERMEDAD.DISCAPACIDAD.Equals("S"))
                    {
                        DiscapacidadEnabled = true;
                        base.RemoveRule("DiscapacidadesMotivo");
                        base.AddRule(() => DiscapacidadesMotivo, () => !string.IsNullOrEmpty(DiscapacidadesMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                        OnPropertyChanged("DiscapacidadesMotivo");
                    }
                    else
                    {
                        DiscapacidadEnabled = false;
                        DiscapacidadesMotivo = string.Empty;
                        base.RemoveRule("DiscapacidadesMotivo");
                        OnPropertyChanged("DiscapacidadesMotivo");
                    }

                }
                //setValidacionesEnfermedades();
                OnPropertyChanged("Discapacidades");
            }
        }
        public string DiscapacidadesMotivo
        {
            get { return EMI_ENFERMEDAD.DESCR_DISCAPACIDAD; }
            set
            {
                EMI_ENFERMEDAD.DESCR_DISCAPACIDAD = value;
                OnPropertyChanged("DiscapacidadesMotivo");
            }
        }
        public string EnfermoMental
        {
            get { return EMI_ENFERMEDAD.ENFERMO_MENTAL; }
            set
            {
                EMI_ENFERMEDAD.ENFERMO_MENTAL = value;
                if (EMI_ENFERMEDAD.ENFERMO_MENTAL != null)
                {
                    if (base.FindRule("EnfermoMental"))
                    if (EMI_ENFERMEDAD.ENFERMO_MENTAL.Equals("S"))
                    {
                        EnfermoMentalEnabled = true;
                        base.RemoveRule("EnfermoMentalMotivo");
                        base.AddRule(() => EnfermoMentalMotivo, () => !string.IsNullOrEmpty(EnfermoMentalMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                        OnPropertyChanged("EnfermoMentalMotivo");
                    }
                    else
                    {
                        EnfermoMentalEnabled = false;
                        EnfermoMentalMotivo = string.Empty;
                        base.RemoveRule("EnfermoMentalMotivo");
                        OnPropertyChanged("EnfermoMentalMotivo");
                    }
                }
                //setValidacionesEnfermedades();
                OnPropertyChanged("EnfermoMental");
            }
        }
        public string EnfermoMentalMotivo
        {
            get { return EMI_ENFERMEDAD.DESCR_ENFERMO_MENTAL; }
            set
            {
                EMI_ENFERMEDAD.DESCR_ENFERMO_MENTAL = value;
                OnPropertyChanged("EnfermoMentalMotivo");
            }
        }
        public string VIHHepatitis
        {
            get { return EMI_ENFERMEDAD.VIH_HEPATITIS; }
            set
            {
                EMI_ENFERMEDAD.VIH_HEPATITIS = value;
                if (value != null)
                {
                    if (base.FindRule("VIHHepatitis"))
                    if (value.Equals("S"))
                    {
                        EnfermedadEnabled = true;
                        base.RemoveRule("VIHHepatitisTratamientoFarmaco");
                        base.AddRule(() => VIHHepatitisTratamientoFarmaco, () => !string.IsNullOrEmpty(VIHHepatitisTratamientoFarmaco), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                        OnPropertyChanged("VIHHepatitisTratamientoFarmaco");
                        base.RemoveRule("VIHHepatitisDiagnosticoFormal");
                        base.AddRule(() => VIHHepatitisDiagnosticoFormal, () => !string.IsNullOrEmpty(VIHHepatitisDiagnosticoFormal), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                        OnPropertyChanged("VIHHepatitisDiagnosticoFormal");
                    }
                    else
                    {
                        VIHHepatitisDiagnosticoFormal = "";
                        VIHHepatitisTratamientoFarmaco = "";
                        EnfermedadEnabled = false;
                        base.RemoveRule("VIHHepatitisTratamientoFarmaco");
                        OnPropertyChanged("VIHHepatitisTratamientoFarmaco");
                        base.RemoveRule("VIHHepatitisDiagnosticoFormal");
                        OnPropertyChanged("VIHHepatitisDiagnosticoFormal");
                    }
                    

                }
                //setValidacionesEnfermedades();
                OnPropertyChanged("VIHHepatitis");
            }
        }
        private string _VIHHepatitisTratamientoFarmaco;
        public string VIHHepatitisTratamientoFarmaco
        {
            get { return _VIHHepatitisTratamientoFarmaco; }
            set
            {
                _VIHHepatitisTratamientoFarmaco = value;
                //setValidacionesEnfermedades();
                OnPropertyChanged("VIHHepatitisTratamientoFarmaco");
            }
        }
        private string _VIHHepatitisDiagnosticoFormal;
        public string VIHHepatitisDiagnosticoFormal
        {
            get { return _VIHHepatitisDiagnosticoFormal; }
            set
            {
                _VIHHepatitisDiagnosticoFormal = value;
                //setValidacionesEnfermedades();
                OnPropertyChanged("VIHHepatitisDiagnosticoFormal");
            }
        }
        #endregion
        #region [VALIDACION]
        private bool discapacidadEnabled;
        public bool DiscapacidadEnabled
        {
            get { return discapacidadEnabled; }
            set { discapacidadEnabled = value; OnPropertyChanged("DiscapacidadEnabled"); }
        }

        private bool enfermoMentalEnabled;
        public bool EnfermoMentalEnabled
        {
            get { return enfermoMentalEnabled; }
            set { enfermoMentalEnabled = value; OnPropertyChanged("EnfermoMentalEnabled"); }
        }

        private bool enfermedadEnabled;
        public bool EnfermedadEnabled
        {
            get { return enfermedadEnabled; }
            set { enfermedadEnabled = value; OnPropertyChanged("EnfermedadEnabled"); }
        }
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion

        private bool tabUsoDrogaSelected;
        public bool TabUsoDrogaSelected
        {
            get { return tabUsoDrogaSelected; }
            set { tabUsoDrogaSelected = value; OnPropertyChanged("TabUsoDrogaSelected"); }
        }

        private bool tabHPSSelected;
        public bool TabHPSSelected
        {
            get { return tabHPSSelected; }
            set { tabHPSSelected = value; OnPropertyChanged("TabHPSSelected"); }
        }

        private bool tabTatuajesSelected;
        public bool TabTatuajesSelected
        {
            get { return tabTatuajesSelected; }
            set { tabTatuajesSelected = value; OnPropertyChanged("TabTatuajesSelected"); }
        }

        private bool tabEnfermedadesSelected;
        public bool TabEnfermedadesSelected
        {
            get { return tabEnfermedadesSelected; }
            set { tabEnfermedadesSelected = value; OnPropertyChanged("TabEnfermedadesSelected"); }
        }

        private bool tabConductasParasocialesSelected;
        public bool TabConductasParasocialesSelected
        {
            get { return tabConductasParasocialesSelected; }
            set { tabConductasParasocialesSelected = value; OnPropertyChanged("TabConductasParasocialesSelected"); }
        }

        //enabled
        private bool pandillaEnabled;
        public bool PandillaEnabled
        {
            get { return pandillaEnabled; }
            set { pandillaEnabled = value; OnPropertyChanged("PandillaEnabled"); }
        }

        private Visibility comboFrontBackFotoVisible = Visibility.Collapsed;
        public Visibility ComboFrontBackFotoVisible
        {
            get { return comboFrontBackFotoVisible; }
            set { comboFrontBackFotoVisible = value; OnPropertyChanged("ComboFrontBackFotoVisible"); }
        }
    }
}
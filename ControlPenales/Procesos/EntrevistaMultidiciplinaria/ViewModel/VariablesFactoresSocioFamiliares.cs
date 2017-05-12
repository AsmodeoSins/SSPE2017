using ControlPenales.Clases;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {
        #region [Factores Socio Familiares]
        private EMI_FACTORES_SOCIO_FAMILIARES EMI_FACTORES_SOCIO_FAMILIARES = new EMI_FACTORES_SOCIO_FAMILIARES();
        private EMI_ANTECEDENTE_FAM_CON_DEL EMI_ANTECEDENTE_FAM_CON_DEL = new EMI_ANTECEDENTE_FAM_CON_DEL();
        private ObservableCollection<FRECUENCIA> lstFrecuencias;

        #region [Factores]
        #region [Factores Socio Familiares]
        public ObservableCollection<FRECUENCIA> LstFrecuencias
        {
            get { return lstFrecuencias; }
            set
            {
                lstFrecuencias = value;
                OnPropertyChanged("LstFrecuencias");
            }
        }
        public string RecibeVisitaFamiliar
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR = value;
                Frecuencia = -1;//value == "S" ? (short)0 : (short)1;
                EnabledFrecuenciaVF = value == "S" ? true : false;
                OnPropertyChanged("RecibeVisitaFamiliar");
                if (SelectIngreso != null)
                {
                    setValidacionesFactores();
                   // OnPropertyChanged();
                }
            }
        }
        public short? Frecuencia
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_VISITA_FAMILIAR; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_VISITA_FAMILIAR = value;
                OnPropertyChanged("Frecuencia");
            }
        }
        public string VisitaIntima
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA = value;
                OnPropertyChanged("VisitaIntima");
            }
        }
        public string ApoyoEconomico
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO = value;
                CantidadApoyoEconomicoEnabled = value == "S";
               // if (!CantidadApoyoEconomicoEnabled)
                CantidadApoyoEconomico = string.Empty;
                CantidadFrecuencia = -1;
                OnPropertyChanged("ApoyoEconomico");
                if (SelectIngreso != null)
                {
                    setValidacionesFactores();
                    //OnPropertyChanged();
                }
            }
        }
        public short? CantidadFrecuencia
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_APOYO_ECONOMICO; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_APOYO_ECONOMICO = value;
                OnPropertyChanged("CantidadFrecuencia");
            }
        }
        public string CantidadApoyoEconomico
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_APOYO_ECONOMICO == null ? "0" : EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_APOYO_ECONOMICO; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_APOYO_ECONOMICO = value;
                OnPropertyChanged("CantidadApoyoEconomico");
            }
        }

        #region [Situacion Actual de los Padre]
        public string VivePadre
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE = value;
                if (value.Equals("N"))
                {
                    PadresVivenJuntos = "N";
                    OnPropertyChanged("PadresVivenJuntos");
                    EdadFallecioPadreEnabled = true;
                    OnPropertyChanged("EdadFallecioPadreEnabled");
                    PadresJuntosEnabled = false;

                    //if (ViveMadre == "N")
                    //{
                    //    base.RemoveRule("PadresVivenJuntos");
                    //    OnPropertyChanged("PadresVivenJuntos");
                    //}
                }
                else
                {
                    PadresJuntosEnabled = true;
                    EdadFallecioPadreEnabled = false;
                    OnPropertyChanged("EdadFallecioPadreEnabled");
                }

                #region Validaciones
                if (value == "N")
                {
                    base.FindRule("FallecioPadre");
                    base.AddRule(() => FallecioPadre, () => FallecioPadre.HasValue, "EDAD DEL INTERNO AL FALLECER PADRE");
                    OnPropertyChanged("FallecioPadre");
                }
                else
                {
                    base.FindRule("FallecioPadre");
                    OnPropertyChanged("FallecioPadre");
                }

                #endregion

                OnPropertyChanged("VivePadre");
                
                //setValidacionesFactores();
                //OnPropertyChanged();
            }
        }
        public string ViveMadre
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE = value;
                if (value.Equals("N"))
                {
                    EdadFallecioMadreEnabled = true;
                    OnPropertyChanged("EdadFallecioMadreEnabled");
                }
                else
                {
                    EdadFallecioMadreEnabled = false;
                    OnPropertyChanged("EdadFallecioMadreEnabled");
                }
                //setValidacionesFactores();
                //if (VivePadre == "N")
                //{
                //    base.RemoveRule("PadresVivenJuntos");
                //    OnPropertyChanged("PadresVivenJuntos");
                //}

                #region Validaciones
                if (value == "N")
                {
                    base.FindRule("FallecioMadre");
                    base.AddRule(() => FallecioMadre, () => FallecioMadre.HasValue, "EDAD DEL INTERNO AL FALLECER MADRE");
                    OnPropertyChanged("FallecioMadre");
                }
                else
                {
                    base.FindRule("FallecioMadre");
                    OnPropertyChanged("FallecioMadre");
                }
                #endregion

                OnPropertyChanged("ViveMadre");
            }
        }
        public string PadresVivenJuntos
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.PADRES_JUNTOS; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.PADRES_JUNTOS = value;
                SeparacionPadresEnabled = value == "N" ? true : false;
                EdadInternoSeparacionPadres = 0;// value == "S" ? 0 : new Nullable<short>();
                MotivoSeparacion = string.Empty;

                #region Validaciones
                if (value == "N")
                {
                    base.FindRule("EdadInternoSeparacionPadres");
                    base.AddRule(() => EdadInternoSeparacionPadres, () => EdadInternoSeparacionPadres.HasValue, "EDAD DEL INTERNO AL SEPARARSE SUS PADRES");
                    OnPropertyChanged("EdadInternoSeparacionPadres");

                    base.RemoveRule("MotivoSeparacion");
                    base.AddRule(() => MotivoSeparacion, () => !string.IsNullOrEmpty(MotivoSeparacion), "MOTIVO DE SEPARACION DE LOS PADRES");
                    OnPropertyChanged("MotivoSeparacion");
                }
                else
                {
                    base.FindRule("EdadInternoSeparacionPadres");
                    OnPropertyChanged("EdadInternoSeparacionPadres");

                    base.RemoveRule("MotivoSeparacion");
                    OnPropertyChanged("MotivoSeparacion");                    
                }
                #endregion

                OnPropertyChanged("PadresVivenJuntos");
                //setValidacionesFactores();
               // OnPropertyChanged();
            }
        }
        public short? FallecioPadre
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_PADRE; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_PADRE = value;
                var x = value == null ? 0 : value;
                var y = FallecioMadre == null ? 0 : FallecioMadre;
                if (x < y)
                    EdadInternoSeparacionPadres = x;
                else
                    EdadInternoSeparacionPadres = y;
                if (SelectIngreso.IMPUTADO.NACIMIENTO_FECHA != null)
                    //if (value <= ((DateTime.Parse(new Fechas().GetFechaYHoraDateServer()) - SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value).Days / 365))
                    if (value <= ((DateTime.Now - SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value).Days / 365))
                {
                    OnPropertyChanged("FallecioPadre");
                }
                else
                    OnPropertyChanged();
            }
        }
        public short? FallecioMadre
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_MADRE; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_MADRE = value;
                var x = FallecioPadre == null ? 0 : FallecioPadre;
                var y = value == null ? 0 : value;
                if (x < y)
                    EdadInternoSeparacionPadres = x;
                else
                    EdadInternoSeparacionPadres = y;
                if (SelectIngreso.IMPUTADO.NACIMIENTO_FECHA != null)
                // if (value <= ((DateTime.Parse(new Fechas().GetFechaYHoraDateServer()) - SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value).Days / 365))
                    if (value <= ((DateTime.Now - SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value).Days / 365))
                    {
                    OnPropertyChanged("FallecioMadre");
                }
                else
                    OnPropertyChanged();
            }
        }
        public string MotivoSeparacion
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.MOTIVOS_SEPARACION; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.MOTIVOS_SEPARACION = value;
                OnPropertyChanged("MotivoSeparacion");
            }
        }
        public short? EdadInternoSeparacionPadres
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_SEPARACION; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_SEPARACION = value;
                OnPropertyChanged("EdadInternoSeparacionPadres");
            }
        }
        #region Enabling/Disabling Description Fields
        private bool _CantidadApoyoEconomicoEnabled;
        public bool CantidadApoyoEconomicoEnabled
        {
            get { return _CantidadApoyoEconomicoEnabled; }
            set { _CantidadApoyoEconomicoEnabled = value; OnPropertyChanged("CantidadApoyoEconomicoEnabled"); }
        }
        private bool _EdadFallecioPadreEnabled;
        public bool EdadFallecioPadreEnabled
        {
            get { return _EdadFallecioPadreEnabled; }
            set
            {
                _EdadFallecioPadreEnabled = value;
                //if (!value)
                //{
                //    FallecioPadre = 0;
                //    OnPropertyChanged("FallecioPadre");
                //}
                OnPropertyChanged("EdadFallecioPadreEnabled");
            }
        }
        private bool _EdadFallecioMadreEnabled;
        public bool EdadFallecioMadreEnabled
        {
            get { return _EdadFallecioMadreEnabled; }
            set
            {

                _EdadFallecioMadreEnabled = value;
                //if (!value)
                //{
                //    FallecioMadre = 0;
                //    OnPropertyChanged("FallecioMadre");
                //}
                OnPropertyChanged("EdadFallecioMadreEnabled");
            }
        }
        private bool _SeparacionPadresEnabled = false;
        public bool SeparacionPadresEnabled
        {
            get { return _SeparacionPadresEnabled; }
            set
            {
                _SeparacionPadresEnabled = value;
                OnPropertyChanged("SeparacionPadresEnabled");
            }
        }
        private bool _AbusoSexualEnabled;
        public bool AbusoSexualEnabled
        {
            get { return _AbusoSexualEnabled; }
            set
            {
                _AbusoSexualEnabled = value;
                OnPropertyChanged("AbusoSexualEnabled");
            }
        }
        private bool _MaltratoFisicoEnabled;
        public bool MaltratoFisicoEnabled
        {
            get { return _MaltratoFisicoEnabled; }
            set
            {
                _MaltratoFisicoEnabled = value;
                OnPropertyChanged("MaltratoFisicoEnabled");
            }
        }
        private bool _MaltratoEmocionalEnabled;
        public bool MaltratoEmocionalEnabled
        {
            get { return _MaltratoEmocionalEnabled; }
            set
            {
                _MaltratoEmocionalEnabled = value;
                OnPropertyChanged("MaltratoEmocionalEnabled");
            }
        }
        private bool _AbandonoFamiliarEnabled;
        public bool AbandonoFamiliarEnabled
        {
            get { return _AbandonoFamiliarEnabled; }
            set
            {
                _AbandonoFamiliarEnabled = value;
                OnPropertyChanged("AbandonoFamiliarEnabled");
            }
        }
        private bool _EdadAbusoSexualEnabled;
        public bool EdadAbusoSexualEnabled
        {
            get { return _EdadAbusoSexualEnabled; }
            set
            {
                _EdadAbusoSexualEnabled = value;
                OnPropertyChanged("EdadAbusoSexualEnabled");
                //if (!value)
                //{
                //    EdadAbuso = 0;
                //    OnPropertyChanged("EdadAbuso");
                //}

            }
        }
        private bool _EliminarItemMenu;
        public bool EliminarItemMenu
        {
            get { return _EliminarItemMenu; }
            set
            {
                _EliminarItemMenu = value;
                OnPropertyChanged("EliminarItemMenu");
            }
        }
        private bool _AddItemMenu;
        public bool AddItemMenu
        {
            get { return _AddItemMenu; }
            set
            {
                _AddItemMenu = value;
                OnPropertyChanged("AddItemMenu");
            }
        }
        #endregion
        #endregion
        #region [Niveles]
        public short? Social
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL = value;
                OnPropertyChanged("Social");
            }
        }
        public short? Cultural
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL = value;
                OnPropertyChanged("Cultural");
            }
        }

        public short? Economico
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_ECONOMICO; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_ECONOMICO = value;
                OnPropertyChanged("Economico");
            }
        }
        #endregion
        #region [Relacion de Pareja que ha Mantenido]
        public short? TotalParejas
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.TOTAL_PAREJAS; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.TOTAL_PAREJAS = value;
                OnPropertyChanged("TotalParejas");
            }
        }
        public short? CuantasUnion
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_PAREJAS_UNION; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_PAREJAS_UNION = value;
                OnPropertyChanged("CuantasUnion");
            }
        }
        #endregion
        #region [Hijos]
        public short? Hijos
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS = value;
                OnPropertyChanged("Hijos");
            }
        }
        public short? Registrados
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.HIJOS_REGISTRADOS; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.HIJOS_REGISTRADOS = value;
                OnPropertyChanged("Registrados");
            }
        }
        public short? CuantosMantieneRelacion
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_RELACION; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_RELACION = value;
                OnPropertyChanged("CuantosMantieneRelacion");
            }
        }
        public short? CuantosVisitan
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_VISITA; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_VISITA = value;
                OnPropertyChanged("CuantosVisitan");
            }
        }
        #endregion
        #region [Contactos en Caso de Necesidad]
        public string ContactoNombre
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_NOMBRE; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_NOMBRE = value;
                OnPropertyChanged("ContactoNombre");
            }
        }

        public short? ContactoParentesco
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.ID_CONTACTO_PARENTESCO; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.ID_CONTACTO_PARENTESCO = value;
                OnPropertyChanged("ContactoParentesco");
            }
        }
        public long? ContactoTelefono
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO = value;
                OnPropertyChanged("ContactoTelefono");
            }
        }

        private string textContactoTelefono;
        public string TextContactoTelefono
        {
            get { return textContactoTelefono; }
            set {
                if (!string.IsNullOrEmpty(value))
                {
                    textContactoTelefono = new Converters().MascaraTelefono(value);
                }
                else
                    textContactoTelefono = value;
                OnPropertyChanged("TextContactoTelefono"); }
        }

        #endregion
        #region []
        public string AbandonoFamiliar
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR = value;
                //HuidasHogar = -1;//value == "S" ? -1 : new Nullable<short>();
                EspecifiqueAbandonoFamiliar = -1;
                OnPropertyChanged("AbandonoFamiliar");
                AbandonoFamiliarEnabled = value == "S";
                OnPropertyChanged("AbandonoFamiliarEnabled");
                //EspecifiqueAbandonoFamiliar = value.ToString().Equals("S") ? EspecifiqueAbandonoFamiliar : string.Empty;
                setValidacionesFactores();
                OnPropertyChanged("EspecifiqueAbandonoFamiliar");
            }
        }
        public short? EspecifiqueAbandonoFamiliar
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR = value;
                OnPropertyChanged("EspecifiqueAbandonoFamiliar");
            }
        }
        public short? HuidasHogar
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.HUIDAS_HOGAR_ABANDONO_FAMILIAR; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.HUIDAS_HOGAR_ABANDONO_FAMILIAR = value;
                OnPropertyChanged("HuidasHogar");
            }
        }
        public string MaltratoEmocional
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL = value;
                //OnPropertyChanged("MaltratoEmocional");
                //MaltratoEmocionalEnabled = value == "S";
                //OnPropertyChanged("MaltratoEmocionalEnabled");
                //EspecifiqueMaltratoEmocional = value.ToString().Equals("S") ? EspecifiqueMaltratoEmocional : string.Empty;
                //OnPropertyChanged("EspecifiqueMaltratoEmocional");
                if (value == "S")
                    MaltratoEmocionalEnabled = true;
                else
                    MaltratoEmocionalEnabled = false;
                EspecifiqueMaltratoEmocional = string.Empty;
                setValidacionesFactores();
                OnPropertyChanged();
            }
        }

        private string especifiqueMaltratoEmocional;
        public string EspecifiqueMaltratoEmocional
        {
            get { return especifiqueMaltratoEmocional; }
            set { especifiqueMaltratoEmocional = value; OnPropertyChanged("EspecifiqueMaltratoEmocional"); }
                }
        //public string EspecifiqueMaltratoEmocional
        //{
        //    get { return EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL != null ? (EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL.Equals("S") ? EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL : string.Empty) : string.Empty; }
        //    set
        //    {
        //        if (EMI_FACTORES_SOCIO_FAMILIARES != null)
        //        {
        //            if (EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL != null)
        //                EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL = EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL.Equals("S") ? value : string.Empty;
        //            OnPropertyChanged("EspecifiqueMaltratoEmocional");
        //        }
        //    }
        //}
        public string MaltratoFisico
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO = value;
                OnPropertyChanged("MaltratoFisico");
                MaltratoFisicoEnabled = value == "S";
                OnPropertyChanged("MaltratoFisicoEnabled");
                EspecifiqueMaltratoFisico = value.ToString().Equals("S") ? EspecifiqueMaltratoFisico : string.Empty;
                OnPropertyChanged("EspecifiqueMaltratoFisico");
                setValidacionesFactores();
                OnPropertyChanged();
            }
        }

        private string especifiqueMaltratoFisico;
        public string EspecifiqueMaltratoFisico
        {
            get { return especifiqueMaltratoFisico; }
            set { especifiqueMaltratoFisico = value; OnPropertyChanged("EspecifiqueMaltratoFisico"); }
        }

        //public string EspecifiqueMaltratoFisico
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL))
        //            return string.Empty;
        //        else
        //        {
        //            if (EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL.Equals("S"))
        //                return EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO;
        //            else
        //                return string.Empty;
        //        }
        //        //else
        //        //{
        //        //    if (EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO == null)
        //        //    {
        //        //        return string.Empty;
        //        //    }
        //        //    else
        //        //        if (EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO.Equals("S"))
        //        //            return EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO;
        //        //        else
        //        //            return string.Empty;
        //        //}
        //        //return !string.IsNullOrEmpty(EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL) ? string.Empty : EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO.Equals("S") ? EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO : string.Empty;
        //        //return EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL != null ? (EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO.Equals("S") ? EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO : string.Empty) : string.Empty; 
        //    }
        //    set
        //    {
        //        if (EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO != null)
        //            EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO = EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO.Equals("S") ? value : string.Empty;
        //        OnPropertyChanged("EspecifiqueMaltratoFisico");
        //    }
        //}
        public string AbusoSexual
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL = value;
                OnPropertyChanged("AbusoSexual");
                EdadAbusoSexualEnabled = AbusoSexualEnabled = value == "S";
                OnPropertyChanged("AbusoSexualEnabled");
                OnPropertyChanged("EdadAbusoSexualEnabled");
                EspecifiqueAbusoSexual = value.ToString().Equals("S") ? EspecifiqueAbusoSexual : string.Empty;
                OnPropertyChanged("EspecifiqueAbusoSexual");
                setValidacionesFactores();
                OnPropertyChanged();
            }
        }
        public string EspecifiqueAbusoSexual
        {
            get
            {
                if (string.IsNullOrEmpty(EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL))
                    return string.Empty;
                else
                {
                    if (EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL.Equals("S"))
                        return EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL;
                    else
                        return string.Empty;
                }
                //return EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL != null ? (EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL.Equals("S") ? EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL : string.Empty) : string.Empty;
            }
            set
            {
                if (EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL != null)
                    EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL = EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL.Equals("S") ? value : string.Empty;
                OnPropertyChanged("EspecifiqueAbusoSexual");
            }
        }
        public short? EdadAbuso
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.EDAD_ABUSO_SEXUAL; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.EDAD_ABUSO_SEXUAL = value;
                OnPropertyChanged("EdadAbuso");
            }
        }
        public short? EdadInicioContactoSexual
        {
            get { return EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INICIO_CONTACTO_SEXUAL; }
            set
            {
                EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INICIO_CONTACTO_SEXUAL = value;
                OnPropertyChanged("EdadInicioContactoSexual");
            }
        }
        #endregion
        #endregion
        #endregion
        #region [Datos Grupo Familiar]
        #region [Indique Integrantes del su Grupo Familiar Primario, Secundario y Personas que Viven con Usted]
        private ObservableCollection<EMI_GRUPO_FAMILIAR> lstGrupoFamiliar;
        public ObservableCollection<EMI_GRUPO_FAMILIAR> LstGrupoFamiliar
        {
            get { return lstGrupoFamiliar; }
            set { lstGrupoFamiliar = value; OnPropertyChanged("LstGrupoFamiliar"); }
        }

        private EMI_GRUPO_FAMILIAR selectedGrupoFamiliar;
        public EMI_GRUPO_FAMILIAR SelectedGrupoFamiliar
        {
            get { return selectedGrupoFamiliar; }
            set
            {
                selectedGrupoFamiliar = value;
                OnPropertyChanged("SelectedGrupoFamiliar");
                if (SelectedGrupoFamiliar != null)
                {
                    var isInDB = new SSP.Controlador.Catalogo.Justicia.cEMIGrupoFamiliar().GetData(w => w.ID_EMI == emiActual.ID_EMI && w.ID_EMI_CONS == emiActual.ID_EMI_CONS && w.ID_CONS == SelectedGrupoFamiliar.ID_CONS);
                    EliminarItemMenu = isInDB.Count() > 0 ? false : true;
                }
            }
        }

        private ObservableCollection<TIPO_REFERENCIA> lstTipoReferencia;
        public ObservableCollection<TIPO_REFERENCIA> LstTipoReferencia
        {
            get { return lstTipoReferencia; }
            set { lstTipoReferencia = value; OnPropertyChanged("LstTipoReferencia"); }
        }

        private ObservableCollection<ESTADO_CIVIL> lstEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> LstEstadoCivil
        {
            get { return lstEstadoCivil; }
            set { lstEstadoCivil = value; OnPropertyChanged("LstEstadoCivil"); }
        }

        private short? grupoFamiliar;
        public short? GrupoFamiliar
        {
            get { return grupoFamiliar; }
            set { grupoFamiliar = value; OnPropertyChanged("GrupoFamiliar"); }
        }

        private string nombreGrupoFamiliar;
        public string NombreGrupoFamiliar
        {
            get { return nombreGrupoFamiliar; }
            set { nombreGrupoFamiliar = value; OnPropertyChanged("NombreGrupoFamiliar"); }
        }

        private string paternoGrupoFamiliar;
        public string PaternoGrupoFamiliar
        {
            get { return paternoGrupoFamiliar; }
            set { paternoGrupoFamiliar = value; OnPropertyChanged("PaternoGrupoFamiliar"); }
        }

        private string maternoGrupoFamiliar;
        public string MaternoGrupoFamiliar
        {
            get { return maternoGrupoFamiliar; }
            set { maternoGrupoFamiliar = value; OnPropertyChanged("MaternoGrupoFamiliar"); }
        }

        private short? edadGrupoFamiliar;
        public short? EdadGrupoFamiliar
        {
            get { return edadGrupoFamiliar; }
            set { edadGrupoFamiliar = value; OnPropertyChanged("EdadGrupoFamiliar"); }
        }

        private DateTime? fechaNacGrupoFamiliar;
        public DateTime? FechaNacGrupoFamiliar
        {
            get { return fechaNacGrupoFamiliar; }
            set { fechaNacGrupoFamiliar = value;
            if (value != null)
            {
                EdadGrupoFamiliar = new Fechas().CalculaEdad(value);
            }
                OnPropertyChanged("FechaNacGrupoFamiliar"); }
        }

        private TIPO_REFERENCIA selectedRelacionGrupoFamiliar;
        public TIPO_REFERENCIA SelectedRelacionGrupoFamiliar
        {
            get { return selectedRelacionGrupoFamiliar; }
            set { selectedRelacionGrupoFamiliar = value; OnPropertyChanged("SelectedRelacionGrupoFamiliar"); }
        }


        private string domicilioGrupoFamiliar;
        public string DomicilioGrupoFamiliar
        {
            get { return domicilioGrupoFamiliar; }
            set { domicilioGrupoFamiliar = value; OnPropertyChanged("DomicilioGrupoFamiliar"); }
        }

        private OCUPACION selectedOcupacionGrupoFamiliar;
        public OCUPACION SelectedOcupacionGrupoFamiliar
        {
            get { return selectedOcupacionGrupoFamiliar; }
            set { selectedOcupacionGrupoFamiliar = value; OnPropertyChanged("SelectedOcupacionGrupoFamiliar"); }
        }

        private ESTADO_CIVIL selectedEdoCivilGrupoFamiliar;
        public ESTADO_CIVIL SelectedEdoCivilGrupoFamiliar
        {
            get { return selectedEdoCivilGrupoFamiliar; }
            set { selectedEdoCivilGrupoFamiliar = value; OnPropertyChanged("SelectedEdoCivilGrupoFamiliar"); }
        }

        private bool? viveConElGrupoFamiliar;
        public bool? ViveConElGrupoFamiliar
        {
            get { return viveConElGrupoFamiliar; }
            set { viveConElGrupoFamiliar = value; OnPropertyChanged("ViveConElGrupoFamiliar"); }
        }

        private bool isGrupoFamiliarEmpty;
        public bool IsGrupoFamiliarEmpty
        {
            get { return isGrupoFamiliarEmpty; }
            set { isGrupoFamiliarEmpty = value; OnPropertyChanged("IsGrupoFamiliarEmpty"); }
        }

        //private ObservableCollection<emi>
        //public string DGFGrupo
        //{
        //    get { return _DGFGrupo; }
        //    set
        //    {
        //        _DGFGrupo = value;
        //        OnPropertyChanged("DGFGrupo");
        //    }
        //}
        //public string DGFPaterno
        //{
        //    get { return _DGFPaterno; }
        //    set
        //    {
        //        _DGFPaterno = value;
        //        OnPropertyChanged("DGFPaterno");
        //    }
        //}
        //public string DGFMaterno
        //{
        //    get { return _DGFMaterno; }
        //    set
        //    {
        //        _DGFMaterno = value;
        //        OnPropertyChanged("DGFMaterno");
        //    }
        //}
        //public string DGFNombre
        //{
        //    get { return _DGFNombre; }
        //    set
        //    {
        //        _DGFNombre = value;
        //        OnPropertyChanged("DGFNombre");
        //    }
        //}
        //public string DGFEdad
        //{
        //    get { return _DGFEdad; }
        //    set
        //    {
        //        _DGFEdad = value;
        //        OnPropertyChanged("DGFEdad");
        //    }
        //}

        //public string DGFRelacion
        //{
        //    get { return _DGFRelacion; }
        //    set
        //    {
        //        _DGFRelacion = value;
        //        OnPropertyChanged("DGFRelacion");
        //    }
        //}

        //public string DGFDomicilio
        //{
        //    get { return _DGFDomicilio; }
        //    set
        //    {
        //        _DGFDomicilio = value;
        //        OnPropertyChanged("DGFDomicilio");
        //    }
        //}

        //public string DGFOcupacion
        //{
        //    get { return _DGFOcupacion; }
        //    set
        //    {
        //        _DGFOcupacion = value;
        //        OnPropertyChanged("DGFOcupacion");
        //    }
        //}

        //public string DGFEstadoCivil
        //{
        //    get { return _DGFEstadoCivil; }
        //    set
        //    {
        //        _DGFEstadoCivil = value;
        //        OnPropertyChanged("DGFEstadoCivil");
        //    }
        //}

        //public string DGFViveConEl
        //{
        //    get { return _DGFViveConEl; }
        //    set
        //    {
        //        EMI_ANTECEDENTE_FAMILIAR_DROGA.vive.vive = value;
        //        OnPropertyChanged("DGFViveConEl");
        //    }
        //}
        #endregion
        #endregion
        #region [Antecedentes Grupo Familiar]
        //public short? AGFParentesco
        //{
        //    get { return EMI_ANTECEDENTE_FAM_CON_DEL.ID_PARENTESCO; }
        //    set
        //    {
        //        EMI_ANTECEDENTE_FAM_CON_DEL.ID_PARENTESCO = value;
        //        OnPropertyChanged("AGFParentesco");
        //    }
        //}
        //public short? AGFAño
        //{
        //    get { return EMI_ANTECEDENTE_FAM_CON_DEL.ANIO; }
        //    set
        //    {
        //        EMI_ANTECEDENTE_FAM_CON_DEL.ANIO = value;
        //        OnPropertyChanged("AGFAño");
        //    }
        //}
        //public short AGFCereso
        //{
        //    get { return EMI_ANTECEDENTE_FAM_CON_DEL.ID_EMI_ANTECEDENTE; }
        //    set
        //    {
        //        EMI_ANTECEDENTE_FAM_CON_DEL.ID_EMI_ANTECEDENTE = value;
        //        OnPropertyChanged("AGFCereso");
        //    }
        //}
        //public short? AGFDelito
        //{
        //    get { return EMI_ANTECEDENTE_FAM_CON_DEL.ID_DELITO; }
        //    set
        //    {
        //        EMI_ANTECEDENTE_FAM_CON_DEL.ID_DELITO = value;
        //        OnPropertyChanged("AGFDelito");
        //    }
        //}
        //public short? AGFRelacion
        //{
        //    get { return EMI_ANTECEDENTE_FAM_CON_DEL.ID_RELACION; }
        //    set
        //    {
        //        EMI_ANTECEDENTE_FAM_CON_DEL.ID_RELACION = value;
        //        OnPropertyChanged("AGFRelacion");
        //    }
        //}

        //FAMILIAR DELITO

        private ObservableCollection<INGRESO_DELITO> lstDelitos;
        public ObservableCollection<INGRESO_DELITO> LstDelitos
        {
            get { return lstDelitos; }
            set { lstDelitos = value; OnPropertyChanged("LstDelitos"); }

        }

        private ObservableCollection<TIPO_RELACION> lstTipoRelacion;
        public ObservableCollection<TIPO_RELACION> LstTipoRelacion
        {
            get { return lstTipoRelacion; }
            set { lstTipoRelacion = value; OnPropertyChanged("LstTipoRelacion"); }

        }

        private ObservableCollection<EMISOR> lstEmisor;
        public ObservableCollection<EMISOR> LstEmisor
        {
            get { return lstEmisor; }
            set { lstEmisor = value; OnPropertyChanged("LstEmisor"); }

        }

        private bool isEmptyFamiliarDelito;
        public bool IsEmptyFamiliarDelito
        {
            get { return isEmptyFamiliarDelito; }
            set { isEmptyFamiliarDelito = value; OnPropertyChanged("IsEmptyFamiliarDelito"); }
        }

        private ObservableCollection<EMI_ANTECEDENTE_FAM_CON_DEL> lstFamiliarDelito;
        public ObservableCollection<EMI_ANTECEDENTE_FAM_CON_DEL> LstFamiliarDelito
        {
            get { return lstFamiliarDelito; }
            set { lstFamiliarDelito = value; OnPropertyChanged("LstFamiliarDelito"); }
        }

        private EMI_ANTECEDENTE_FAM_CON_DEL selectedFamiliarDelito;
        public EMI_ANTECEDENTE_FAM_CON_DEL SelectedFamiliarDelito
        {
            get { return selectedFamiliarDelito; }
            set
            {
                selectedFamiliarDelito = value;
                OnPropertyChanged("SelectedFamiliarDelito");
                if (SelectedFamiliarDelito != null)
                {
                    var isInDB = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIFamConDelito().GetData(w => w.ID_EMI == emiActual.ID_EMI && w.ID_EMI_CONS == emiActual.ID_EMI_CONS && w.ID_EMI_ANTECEDENTE == SelectedFamiliarDelito.ID_EMI_ANTECEDENTE);
                    EliminarItemMenu = isInDB.Count() > 0 ? false : true;
                }
            }
        }

        private TIPO_REFERENCIA selectedParentescoFDel;
        public TIPO_REFERENCIA SelectedParentescoFDel
        {
            get { return selectedParentescoFDel; }
            set { selectedParentescoFDel = value; OnPropertyChanged("SelectedParentescoFDel"); }
        }

        private string delitoFDel;

        public string DelitoFDel
        {
            get { return delitoFDel; }
            set { delitoFDel = value; OnPropertyChanged("DelitoFDel"); }
        }

        private short? anioFDel;
        public short? AnioFDel
        {
            get { return anioFDel; }
            set     { anioFDel = value; OnPropertyChanged("AnioFDel"); }
        }

        private INGRESO_DELITO selectedDelitoFDel;
        public INGRESO_DELITO SelectedDelitoFDel
        {
            get { return selectedDelitoFDel; }
            set { selectedDelitoFDel = value; OnPropertyChanged("SelectedDelitoFDel"); }
        }

        private TIPO_RELACION selectedRelacionFDel;
        public TIPO_RELACION SelectedRelacionFDel
        {
            get { return selectedRelacionFDel; }
            set { selectedRelacionFDel = value; OnPropertyChanged("SelectedRelacionFDel"); }
        }

        private EMISOR selectedEmisorFDel;
        public EMISOR SelectedEmisorFDel
        {
            get { return selectedEmisorFDel; }
            set { selectedEmisorFDel = value; OnPropertyChanged("SelectedEmisorFDel"); }
        }

        private short selectedIdAntecedenteFDel;
        public short SelectedIdAntecedenteFDel
        {
            get { return selectedIdAntecedenteFDel; }
            set { selectedIdAntecedenteFDel = value; }
        }


        //FAMILIAR USO DROGA


        private ObservableCollection<EMI_ANTECEDENTE_FAMILIAR_DROGA> lstFamiliarDroga;
        public ObservableCollection<EMI_ANTECEDENTE_FAMILIAR_DROGA> LstFamiliarDroga
        {
            get { return lstFamiliarDroga; }
            set { lstFamiliarDroga = value; OnPropertyChanged("LstFamiliarDroga"); }
        }

        private bool isEmptyFamiliarDroga;
        public bool IsEmptyFamiliarDroga
        {
            get { return isEmptyFamiliarDroga; }
            set { isEmptyFamiliarDroga = value; OnPropertyChanged("IsEmptyFamiliarDroga"); }
        }

        private EMI_ANTECEDENTE_FAMILIAR_DROGA selectedFamiliarDroga;
        public EMI_ANTECEDENTE_FAMILIAR_DROGA SelectedFamiliarDroga
        {
            get { return selectedFamiliarDroga; }
            set
            {
                selectedFamiliarDroga = value;
                OnPropertyChanged("SelectedFamiliarDroga");
                if (SelectedFamiliarDroga != null)
                {
                    var isInDB = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIFamiliarDroga().GetData(w => w.ID_EMI == emiActual.ID_EMI && w.ID_EMI_CONS == emiActual.ID_EMI_CONS && w.ID_EMI_ANT_CONS == SelectedFamiliarDroga.ID_EMI_ANT_CONS);
                    EliminarItemMenu = isInDB.Count() > 0 ? false : true;
                }
            }
        }

        private DROGA selectedDrogaFDroga;
        public DROGA SelectedDrogaFDroga
        {
            get { return selectedDrogaFDroga; }
            set { selectedDrogaFDroga = value; OnPropertyChanged("SelectedDrogaFDroga"); }
        }

        private TIPO_REFERENCIA selectedParentescoFDroga;
        public TIPO_REFERENCIA SelectedParentescoFDroga
        {
            get { return selectedParentescoFDroga; }
            set { selectedParentescoFDroga = value; OnPropertyChanged("SelectedParentescoFDroga"); }
        }

        private short? anioFDroga;
        public short? AnioFDroga
        {
            get { return anioFDroga; }
            set { anioFDroga = value; OnPropertyChanged("AnioFDroga"); }
        }

        private TIPO_RELACION selectedRelacionFDroga;
        public TIPO_RELACION SelectedRelacionFDroga
        {
            get { return selectedRelacionFDroga; }
            set { selectedRelacionFDroga = value; OnPropertyChanged("SelectedRelacionFDroga"); }
        }

        //ABANDONO DE HOGAR
        private ObservableCollection<EMI_FACTORES_ABANDONO> lstFactoresAbandono;
        public ObservableCollection<EMI_FACTORES_ABANDONO> LstFactoresAbandono
        {
            get { return lstFactoresAbandono; }
            set { lstFactoresAbandono = value; OnPropertyChanged("LstFactoresAbandono"); }
        }

        private ObservableCollection<EMI_FACTORES_HUIDA> lstFactoresHuida;
        public ObservableCollection<EMI_FACTORES_HUIDA> LstFactoresHuida
        {
            get { return lstFactoresHuida; }
            set { lstFactoresHuida = value; OnPropertyChanged("LstFactoresHuida"); }
        }
        #endregion

        private bool enabledFrecuenciaVF;
        public bool EnabledFrecuenciaVF
        {
            get { return enabledFrecuenciaVF; }
            set { enabledFrecuenciaVF = value; OnPropertyChanged(); }
        }

        private bool padresJuntosEnabled = true;
        public bool PadresJuntosEnabled
        {
            get { return padresJuntosEnabled; }
            set { padresJuntosEnabled = value; OnPropertyChanged("PadresJuntosEnabled"); }
        }
        #endregion

        #region [GRUPO FAMILIAR DESDE PADRON DE VISITAS]
        private ObservableCollection<GrupoFamiliarPV> lstPV;
        public ObservableCollection<GrupoFamiliarPV> LstPV
        {
            get { return lstPV; }
            set { lstPV = value; OnPropertyChanged("LstPV"); }
        }

        private ObservableCollection<GrupoFam> lstGpoFam;
        public ObservableCollection<GrupoFam> LstGpoFam
        {
            get { return lstGpoFam; }
            set { lstGpoFam = value; OnPropertyChanged("LstGpoFam"); }
        }

        private bool tabFactores;
        public bool TabFactores
        {
            get { return tabFactores; }
            set { tabFactores = value; OnPropertyChanged("TabFactores"); }
        }

        private bool tabGrupoFamiliarSelected;
        public bool TabGrupoFamiliarSelected
        {
            get { return tabGrupoFamiliarSelected; }
            set { tabGrupoFamiliarSelected = value; OnPropertyChanged("TabGrupoFamiliarSelected"); }
        }

        private bool emptyPadronVisita;
        public bool EmptyPadronVisita
        {
            get { return emptyPadronVisita; }
            set { emptyPadronVisita = value; OnPropertyChanged("EmptyPadronVisita"); }
        }
        #endregion

        private bool tabFactoresSocioFamiliaresSelected;
        public bool TabFactoresSocioFamiliaresSelected
        {
            get { return tabFactoresSocioFamiliaresSelected; }
            set { tabFactoresSocioFamiliaresSelected = value; OnPropertyChanged("TabFactoresSocioFamiliaresSelected"); }
        }

        private bool tabGrupoFamiliarAntecedenteSelected;
        public bool TabGrupoFamiliarAntecedenteSelected
        {
            get { return tabGrupoFamiliarAntecedenteSelected; }
            set { tabGrupoFamiliarAntecedenteSelected = value; OnPropertyChanged("TabGrupoFamiliarAntecedenteSelected"); }
        }

        private bool tabFactorSelected;
        public bool TabFactorSelected
        {
            get { return tabFactorSelected; }
            set { tabFactorSelected = value; OnPropertyChanged("TabFactorSelected"); }
        }

        private bool isTodosGrupoFamiliarPVSelected;
        public bool IsTodosGrupoFamiliarPVSelected
        {
            get { return isTodosGrupoFamiliarPVSelected; }
            set
            {
                isTodosGrupoFamiliarPVSelected = value;
                OnPropertyChanged("IsTodosGrupoFamiliarPVSelected");
                foreach (var visita in LstPV)
                {
                    visita.Seleccionado = value;
                }
                LstPV = new ObservableCollection<GrupoFamiliarPV>(lstPV);
            }
        }
    }
}

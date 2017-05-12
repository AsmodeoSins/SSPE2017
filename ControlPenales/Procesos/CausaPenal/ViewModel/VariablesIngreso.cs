
using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        private DateTime? fecRegistroI;
        public DateTime? FecRegistroI
        {
            get { return fecRegistroI; }
            set { fecRegistroI = value;
            SetValidacionesDatosIngreso();
                OnPropertyValidateChanged("FecRegistroI"); }
        }

        private DateTime? fecCeresoI;
        public DateTime? FecCeresoI
        {
            get { return fecCeresoI; }
            set { fecCeresoI = value;
                SetValidacionesDatosIngreso();
                OnPropertyValidateChanged("FecCeresoI");
            }
        }

        private short tipoI;
        public short TipoI
        {
            get { return tipoI; }
            set { tipoI = value; OnPropertyValidateChanged("TipoI"); }
        }

        private short estatusAdministrativoI;
        public short EstatusAdministrativoI
        {
            get { return estatusAdministrativoI; }
            set { estatusAdministrativoI = value; OnPropertyValidateChanged("EstatusAdministrativoI"); }
        }

        private string clasificacionI;
        public string ClasificacionI
        {
            get { return clasificacionI; }
            set { clasificacionI = value; OnPropertyValidateChanged("ClasificacionI"); }
        }

        private int delitoI;
        public int DelitoI
        {
            get { return delitoI; }
            set { delitoI = value; OnPropertyValidateChanged("DelitoI"); }
        }

        private string noOficioI;
        public string NoOficioI
        {
            get { return noOficioI; }
            set { noOficioI = value; OnPropertyValidateChanged("NoOficioI"); }
        }

        private short autoridadInternaI;
        public short AutoridadInternaI
        {
            get { return autoridadInternaI; }
            set { autoridadInternaI = value; OnPropertyValidateChanged("AutoridadInternaI"); }
        }

        private string tipoSeguridadI;
        public string TipoSeguridadI
        {
            get { return tipoSeguridadI; }
            set { tipoSeguridadI = value; OnPropertyValidateChanged("TipoSeguridadI"); }
        }

        private short quedaDisposicionI;
        public short QuedaDisposicionI
        {
            get { return quedaDisposicionI; }
            set { quedaDisposicionI = value; OnPropertyValidateChanged("QuedaDisposicionI"); }
        }

        private string ubicacionI;
        public string UbicacionI
        {
            get { return ubicacionI; }
            set { ubicacionI = value; OnPropertyValidateChanged("UbicacionI"); }
        }

        private short? anioExpedienteI;
        public short? AnioExpedienteI
        {
            get { return anioExpedienteI; }
            set { anioExpedienteI = value; OnPropertyValidateChanged("AnioExpedienteI"); }
        }

        private string folioExpedienteI;
        public string FolioExpedienteI
        {
            get { return folioExpedienteI; }
            set { folioExpedienteI = value; OnPropertyValidateChanged("FolioExpedienteI"); }
        }

        private DateTime inicioCompurgacionI;
        public DateTime InicioCompurgacionI
        {
            get { return inicioCompurgacionI; }
            set { inicioCompurgacionI = value; OnPropertyValidateChanged("InicioCompurgacionI"); }
        }

        private int aniosPenaI;
        public int AniosPenaI
        {
            get { return aniosPenaI; }
            set { aniosPenaI = value; OnPropertyChanged("AniosPenaI"); }
        }

        private int mesesPenaI;
        public int MesesPenaI
        {
            get { return mesesPenaI; }
            set { mesesPenaI = value; OnPropertyChanged("MesesPenaI"); }
        }

        private int diasPenaI;
        public int DiasPenaI
        {
            get { return diasPenaI; }
            set { diasPenaI = value; OnPropertyChanged("DiasPenaI"); }
        }

        private int aniosCumplidoI;
        public int AniosCumplidoI
        {
            get { return aniosCumplidoI; }
            set { aniosCumplidoI = value; OnPropertyChanged("AniosCumplidoI"); }
        }

        private int mesesCumplidoI;
        public int MesesCumplidoI
        {
            get { return mesesCumplidoI; }
            set { mesesCumplidoI = value; OnPropertyChanged("MesesCumplidoI"); }
        }

        private int diasCumplidoI;
        public int DiasCumplidoI
        {
            get { return diasCumplidoI; }
            set { diasCumplidoI = value; OnPropertyChanged("DiasCumplidoI"); }
        }

        private int aniosAbonosI;
        public int AniosAbonosI
        {
            get { return aniosAbonosI; }
            set { aniosAbonosI = value; OnPropertyChanged("AniosAbonosI"); }
        }

        private int mesesAbonosI;
        public int MesesAbonosI
        {
            get { return mesesAbonosI; }
            set { mesesAbonosI = value; OnPropertyChanged("MesesAbonosI"); }
        }

        private int diasAbonosI;
        public int DiasAbonosI
        {
            get { return diasAbonosI; }
            set { diasAbonosI = value; OnPropertyChanged("DiasAbonosI"); }
        }

        private int aniosRestanteI;
        public int AniosRestanteI
        {
            get { return aniosRestanteI; }
            set { aniosRestanteI = value; OnPropertyChanged("AniosRestanteI"); }
        }

        private int mesesRestanteI;
        public int MesesRestanteI
        {
            get { return mesesRestanteI; }
            set { mesesRestanteI = value; OnPropertyChanged("MesesRestanteI"); }
        }

        private int diasRestanteI;
        public int DiasRestanteI
        {
            get { return diasRestanteI; }
            set { diasRestanteI = value; OnPropertyChanged("DiasRestanteI"); }
        }

        private string folioGobiernoI;
        public string FolioGobiernoI
        {
            get { return folioGobiernoI; }
            set { folioGobiernoI = value; OnPropertyChanged("FolioGobiernoI"); }
        }

        private short? anioGobiernoI;
        public short? AnioGobiernoI
        {
            get { return anioGobiernoI; }
            set { anioGobiernoI = value; OnPropertyChanged("AnioGobiernoI"); }
        }

        //COMBOBOX
        private ObservableCollection<TIPO_INGRESO> lstTiposIngreso;
        public ObservableCollection<TIPO_INGRESO> LstTiposIngreso
        {
            get { return lstTiposIngreso; }
            set { lstTiposIngreso = value; OnPropertyChanged("LstTiposIngreso"); }
        }

        private ObservableCollection<ESTATUS_ADMINISTRATIVO> lstEstatusAdministrativo;
        public ObservableCollection<ESTATUS_ADMINISTRATIVO> LstEstatusAdministrativo
        {
            get { return lstEstatusAdministrativo; }
            set { lstEstatusAdministrativo = value; OnPropertyChanged("LstEstatusAdministrativo"); }
        }

        private ObservableCollection<TIPO_AUTORIDAD_INTERNA> lstAutoridades;
        public ObservableCollection<TIPO_AUTORIDAD_INTERNA> LstAutoridades
        {
            get { return lstAutoridades; }
            set { lstAutoridades = value; OnPropertyChanged("LstAutoridades"); }
        }

        private ObservableCollection<TIPO_SEGURIDAD> lstTiposSeguridad;
        public ObservableCollection<TIPO_SEGURIDAD> LstTiposSeguridad
        {
            get { return lstTiposSeguridad; }
            set { lstTiposSeguridad = value; OnPropertyChanged("LstTiposSeguridad"); }
        }

        private ObservableCollection<TIPO_DISPOSICION> lstAutoridadDisposicion;
        public ObservableCollection<TIPO_DISPOSICION> LstAutoridadDisposicion
        {
            get { return lstAutoridadDisposicion; }
            set { lstAutoridadDisposicion = value; OnPropertyChanged("LstAutoridadDisposicion"); }
        }
        
        private ObservableCollection<EDIFICIO> edificios;
        public ObservableCollection<EDIFICIO> Edificios
        {
            get { return edificios; }
            set { edificios = value; OnPropertyChanged("Edificios"); }
        }
        
        private ObservableCollection<CLASIFICACION_JURIDICA> lstClasificaciones;
        public ObservableCollection<CLASIFICACION_JURIDICA> LstClasificaciones
        {
            get { return lstClasificaciones; }
            set { lstClasificaciones = value; OnPropertyChanged("LstClasificaciones"); }
        }

        private ObservableCollection<TIPO_AUTORIDAD_INTERNA> lstAutoridadesInterna;
        public ObservableCollection<TIPO_AUTORIDAD_INTERNA> LstAutoridadesInterna
        {
            get { return lstAutoridadesInterna; }
            set { lstAutoridadesInterna = value; OnPropertyChanged("LstAutoridadesInterna"); }
        }


        //LISTADO INGRESOS
        private ObservableCollection<INGRESO> ingresos;
        public ObservableCollection<INGRESO> Ingresos
        {
            get { return ingresos; }
            set { ingresos = value; OnPropertyChanged("Ingresos"); }
        }

        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set
            {
                selectedIngreso = value;
                if (selectedIngreso != null)
                {
                    TituloCausasPenales = string.Format("Causas Penales({0}) del Ingreso No.{1}", selectedIngreso.CAUSA_PENAL.Count, selectedIngreso.ID_INGRESO);
                }
                #region Imagen

                if (value.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                #endregion

                //LISTADO DE CAUSAS PENALES
                CausasPenalesIngreso = new ObservableCollection<CausaPenalIngreso>();
                if (value != null)
                {
                    if (SelectedIngreso.CAUSA_PENAL != null)
                    {
                        string fuero = string.Empty;
                        foreach (var cp in selectedIngreso.CAUSA_PENAL)
                        {
                            if (!string.IsNullOrEmpty(cp.CP_FUERO))
                            {
                                switch (cp.CP_FUERO)
                                {
                                    case "C":
                                        fuero = "COMUN";
                                        break;
                                    case "F":
                                        fuero = "FEDERAL";
                                        break;
                                    case "M":
                                        fuero = "MILITAR";
                                        break;
                                }
                            }
                            if (cp.SENTENCIA.Count > 0)
                            {
                                foreach (var s in cp.SENTENCIA)
                                {

                                    CausasPenalesIngreso.Add(new CausaPenalIngreso
                                    {
                                        CausaPenal = cp,
                                        Cp = string.Format("{0}/{1}/{2}/{3}/{4}", cp.CP_ESTADO_JUZGADO != null ? cp.CP_ESTADO_JUZGADO.ToString() : string.Empty, cp.CP_MUNICIPIO_JUZGADO != null ? cp.CP_MUNICIPIO_JUZGADO.ToString() : string.Empty, cp.CP_JUZGADO != null ? cp.CP_JUZGADO.ToString() : string.Empty, cp.CP_ANIO, cp.CP_FOLIO),
                                        FecSentencia = s.FEC_SENTENCIA,
                                        FecEjecutoria = s.FEC_EJECUTORIA,
                                        FecInicioCompurgacion = s.FEC_INICIO_COMPURGACION,
                                        JuzgadoFuero = string.Format("{0}/{1}", cp.JUZGADO != null ? cp.JUZGADO.DESCR : string.Empty, fuero)
                                    });
                                    break;
                                }
                            }
                            else
                            {
                                CausasPenalesIngreso.Add(new CausaPenalIngreso
                                      {
                                          CausaPenal = cp,
                                          Cp = string.Format("{0}/{1}/{2}/{3}/{4}", cp.CP_ESTADO_JUZGADO != null ? cp.CP_ESTADO_JUZGADO.ToString() : string.Empty, cp.CP_MUNICIPIO_JUZGADO != null ? cp.CP_MUNICIPIO_JUZGADO.ToString() : string.Empty, cp.CP_JUZGADO != null ? cp.CP_JUZGADO.ToString() : string.Empty, cp.CP_ANIO, cp.CP_FOLIO),
                                          FecSentencia = null,
                                          FecEjecutoria = null,
                                          FecInicioCompurgacion = null,
                                          JuzgadoFuero = string.Format("{0}/{1}", cp.JUZGADO != null ? cp.JUZGADO.DESCR : string.Empty, fuero)
                                      });
                            }
                        }

                    }
                }
                OnPropertyChanged("SelectedIngreso");
            }
        }

        private IMPUTADO selectedInterno;
        public IMPUTADO SelectedInterno
        {
            get { return selectedInterno; }
            set { selectedInterno = value; OnPropertyChanged("SelectedInterno"); }
        }

        private ObservableCollection<INGRESO> lstIngresosCentro;
        public ObservableCollection<INGRESO> LstIngresosCentro
        {
            get { return lstIngresosCentro; }
            set { lstIngresosCentro = value; OnPropertyChanged("LstIngresosCentro"); }
        }

        private bool tabIngresoSelected;
        public bool TabIngresoSelected
        {
            get { return tabIngresoSelected; }
            set { tabIngresoSelected = value; OnPropertyChanged("TabIngresoSelected"); }
        }

        private ObservableCollection<SentenciaIngreso> lstSentenciasIngresos;
        public ObservableCollection<SentenciaIngreso> LstSentenciasIngresos
        {
            get { return lstSentenciasIngresos; }
            set { lstSentenciasIngresos = value; OnPropertyChanged("LstSentenciasIngresos"); }
        }

        private int totalAnios;
        public int TotalAnios
        {
            get { return totalAnios; }
            set { totalAnios = value; OnPropertyChanged("TotalAnios"); }
        }

        private int totalMeses;
        public int TotalMeses
        {
            get { return totalMeses; }
            set { totalMeses = value; OnPropertyChanged("TotalMeses"); }
        }

        private int totalDias;
        public int TotalDias
        {
            get { return totalDias; }
            set { totalDias = value; OnPropertyChanged("TotalDias"); }
        }

        //UBICACION
        private bool ubicacionVisible;
        public bool UbicacionVisible
        {
            get { return ubicacionVisible; }
            set { ubicacionVisible = value; OnPropertyChanged("UbicacionVisible"); }
        }

        private CAMA selectedUbicacion;
        public CAMA SelectedUbicacion
        {
            get { return selectedUbicacion; }
            set { selectedUbicacion = value; OnPropertyChanged("SelectedUbicacion"); }
        }

        private CENTRO ubicaciones;
        public CENTRO Ubicaciones
        {
            get { return ubicaciones; }
            set { ubicaciones = value; OnPropertyChanged("Ubicaciones"); }
        }

        //DELITO INGRESO
        private ObservableCollection<INGRESO_DELITO> lstIngresoDelitos;
        public ObservableCollection<INGRESO_DELITO> LstIngresoDelitos
        {
            get { return lstIngresoDelitos; }
            set { lstIngresoDelitos = value; OnPropertyValidateChanged("LstIngresoDelitos"); }
        }

        private INGRESO_DELITO selectedIngresoDelito;
        public INGRESO_DELITO SelectedIngresoDelito
        {
            get { return selectedIngresoDelito; }
            set { selectedIngresoDelito = value; OnPropertyChanged("SelectedIngresoDelito"); }
        }

        private short ingresoDelito;
        public short IngresoDelito
        {
            get { return ingresoDelito; }
            set { ingresoDelito = value; OnPropertyChanged("IngresoDelito"); }
        }
        
        //NUC
        private string nUC;
        public string NUC
        {
            get { return nUC; }
            set { 
                nUC = value; 
                OnPropertyChanged("NUC"); }
        }
        
    }
}

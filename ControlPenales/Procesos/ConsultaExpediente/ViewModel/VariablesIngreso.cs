using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        private DateTime? fecRegistroI;
        public DateTime? FecRegistroI
        {
            get { return fecRegistroI; }
            set
            {
                fecRegistroI = value;
                //SetValidacionesDatosIngreso();
                OnPropertyChanged("FecRegistroI");
            }
        }

        private DateTime? fecCeresoI;
        public DateTime? FecCeresoI
        {
            get { return fecCeresoI; }
            set
            {
                fecCeresoI = value;
                //SetValidacionesDatosIngreso();
                OnPropertyChanged("FecCeresoI");
            }
        }

        private short tipoI;
        public short TipoI
        {
            get { return tipoI; }
            set { tipoI = value; OnPropertyChanged("TipoI"); }
        }

        private short estatusAdministrativoI;
        public short EstatusAdministrativoI
        {
            get { return estatusAdministrativoI; }
            set { estatusAdministrativoI = value; OnPropertyChanged("EstatusAdministrativoI"); }
        }

        private string clasificacionI;
        public string ClasificacionI
        {
            get { return clasificacionI; }
            set { clasificacionI = value; OnPropertyChanged("ClasificacionI"); }
        }

        private int delitoI;
        public int DelitoI
        {
            get { return delitoI; }
            set { delitoI = value; OnPropertyChanged("DelitoI"); }
        }

        private string noOficioI;
        public string NoOficioI
        {
            get { return noOficioI; }
            set { noOficioI = value; OnPropertyChanged("NoOficioI"); }
        }

        private short autoridadInternaI;
        public short AutoridadInternaI
        {
            get { return autoridadInternaI; }
            set { autoridadInternaI = value; OnPropertyChanged("AutoridadInternaI"); }
        }

        private string tipoSeguridadI;
        public string TipoSeguridadI
        {
            get { return tipoSeguridadI; }
            set { tipoSeguridadI = value; OnPropertyChanged("TipoSeguridadI"); }
        }

        private short quedaDisposicionI;
        public short QuedaDisposicionI
        {
            get { return quedaDisposicionI; }
            set { quedaDisposicionI = value; OnPropertyChanged("QuedaDisposicionI"); }
        }

        private string ubicacionI;
        public string UbicacionI
        {
            get { return ubicacionI; }
            set { ubicacionI = value; OnPropertyChanged("UbicacionI"); }
        }

        private short? anioExpedienteI;
        public short? AnioExpedienteI
        {
            get { return anioExpedienteI; }
            set { anioExpedienteI = value; OnPropertyChanged("AnioExpedienteI"); }
        }

        private string folioExpedienteI;
        public string FolioExpedienteI
        {
            get { return folioExpedienteI; }
            set { folioExpedienteI = value; OnPropertyChanged("FolioExpedienteI"); }
        }

        private DateTime inicioCompurgacionI;
        public DateTime InicioCompurgacionI
        {
            get { return inicioCompurgacionI; }
            set { inicioCompurgacionI = value; OnPropertyChanged("InicioCompurgacionI"); }
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
                if (value != null)
                {
                    TituloCausasPenales = string.Format("Causas Penales({0}) del Ingreso No.{1}", value.CAUSA_PENAL.Count, value.ID_INGRESO);
                }
                ListCausasPenalesIngreso = new ObservableCollection<CausaPenalIngreso>();
                if (value != null)
                {
                    IngresoSeleccionado = value;
                    GetDatosIngresoSeleccionado();
                    GetIngresos(value);
                    GetEstudios();
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();

                //LISTADO DE CAUSAS PENALES
                OnPropertyChanged("SelectedIngreso");
            }
        }

        private async void GetIngresos(INGRESO value)
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                #region ingreso
                if (value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenIngreso = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                /*if (IngresoSeleccionado != null)
                {
                    IngresoSeleccionado = value;
                    GetDatosIngresoSeleccionado();
                }*/
                #endregion

                #region causas penales
                if (value.CAUSA_PENAL != null)
                {
                    string fuero = string.Empty;
                    var ListCausasPenalesIngresoAux = new List<CausaPenalIngreso>();
                    foreach (var cp in value.CAUSA_PENAL)
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

                                ListCausasPenalesIngresoAux.Add(new CausaPenalIngreso
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
                            ListCausasPenalesIngresoAux.Add(new CausaPenalIngreso
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
                    if (ListCausasPenalesIngresoAux.Any())
                        ListCausasPenalesIngreso = new ObservableCollection<CausaPenalIngreso>(ListCausasPenalesIngresoAux);

                    if (ListCausasPenalesIngreso != null ? ListCausasPenalesIngreso.Count > 0 : false)
                    {
                        //SelectedCausaPenalIngreso = ListCausasPenalesIngreso.FirstOrDefault();
                        SelectedCausaPenal = ListCausasPenalesIngreso.FirstOrDefault().CausaPenal;
                    }
                }
                #endregion
            });
        }

        private IMPUTADO selectedInterno;
        public IMPUTADO SelectedInterno
        {
            get { return selectedInterno; }
            set { selectedInterno = value; OnPropertyChanged("SelectedInterno"); }
        }

        private bool tabIngresoSelected;
        public bool TabIngresoSelected
        {
            get { return tabIngresoSelected; }
            set { tabIngresoSelected = value; OnPropertyChanged("TabIngresoSelected"); }
        }

        private List<SentenciaIngreso> lstSentenciasIngresos;
        public List<SentenciaIngreso> LstSentenciasIngresos
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
            set { lstIngresoDelitos = value; OnPropertyChanged("LstIngresoDelitos"); }
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

        private string delitoDescrI;
        public string DelitoDescrI
        {
            get { return delitoDescrI; }
            set { delitoDescrI = value; OnPropertyValidateChanged("DelitoDescrI"); }
        }

        //NUC
        private string nUC;
        public string NUC
        {
            get { return nUC; }
            set
            {
                nUC = value;
                OnPropertyChanged("NUC");
            }
        }



        private ObservableCollection<JUZGADO> lstJuzgadoAmparo;
        public ObservableCollection<JUZGADO> LstJuzgadoAmparo
        {
            get { return lstJuzgadoAmparo; }
            set { lstJuzgadoAmparo = value; OnPropertyChanged("LstJuzgadoAmparo"); }
        }

        private ObservableCollection<AMPARO_INDIRECTO_SENTENCIA> lstSentenciaAI;
        public ObservableCollection<AMPARO_INDIRECTO_SENTENCIA> LstSentenciaAI
        {
            get { return lstSentenciaAI; }
            set { lstSentenciaAI = value; OnPropertyChanged("LstSentenciaAI"); }
        }

        private ObservableCollection<TIPO_RECURSO> lstTiposRecursos;
        public ObservableCollection<TIPO_RECURSO> LstTiposRecursos
        {
            get { return lstTiposRecursos; }
            set { lstTiposRecursos = value; OnPropertyChanged("LstTiposRecursos"); }
        }

        private ObservableCollection<RECURSO_RESULTADO> lstRecursoResultado;
        public ObservableCollection<RECURSO_RESULTADO> LstRecursoResultado
        {
            get { return lstRecursoResultado; }
            set { lstRecursoResultado = value; OnPropertyChanged("LstRecursoResultado"); }
        }

        private ObservableCollection<JUZGADO> lstTribunales;
        public ObservableCollection<JUZGADO> LstTribunales
        {
            get { return lstTribunales; }
            set { lstTribunales = value; OnPropertyChanged("LstTribunales"); }
        }

        private ObservableCollection<AMPARO_DIRECTO_SENTENCIA> lstSentenciaAD;
        public ObservableCollection<AMPARO_DIRECTO_SENTENCIA> LstSentenciaAD
        {
            get { return lstSentenciaAD; }
            set { lstSentenciaAD = value; OnPropertyChanged("LstSentenciaAD"); }
        }

        private ObservableCollection<TIPO_RECURSO> lstIncidentes;
        public ObservableCollection<TIPO_RECURSO> LstIncidentes
        {
            get { return lstIncidentes; }
            set { lstIncidentes = value; OnPropertyChanged("LstIncidentes"); }
        }

        private ObservableCollection<RECURSO_RESULTADO> lstIncidenteResultado;
        public ObservableCollection<RECURSO_RESULTADO> LstIncidenteResultado
        {
            get { return lstIncidenteResultado; }
            set { lstIncidenteResultado = value; OnPropertyChanged("LstIncidenteResultado"); }
        }

        private ObservableCollection<AMPARO_INCIDENTE_TIPO> lstAmparoIncidenteTipo;
        public ObservableCollection<AMPARO_INCIDENTE_TIPO> LstAmparoIncidenteTipo
        {
            get { return lstAmparoIncidenteTipo; }
            set { lstAmparoIncidenteTipo = value; OnPropertyChanged("LstAmparoIncidenteTipo"); }
        }


    }
}

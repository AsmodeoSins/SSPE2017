using SSP.Servidor;
using System.Linq;
using System.Collections.ObjectModel;
using System;

namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {
        #region [Clasificacion Criminologica]
        private EMI_CLAS_CRIMINOLOGICA EMI_CLAS_CRIMINOLOGICA = new EMI_CLAS_CRIMINOLOGICA();
        //private EMI_SANCION_DISCIPLINARIA EMI_SANCION_DISCIPLINARIA = new EMI_SANCION_DISCIPLINARIA();
        private EMI_SANCION_DISCIPLINARIA _SelectedSancion;
        private EMI_SANCION_DISCIPLINARIA _NewSancion = new EMI_SANCION_DISCIPLINARIA();
        private ObservableCollection<EMI_SANCION_DISCIPLINARIA> lstSanciones;
        private ObservableCollection<CLASIFICACION_CRIMINOLOGICA> lstClasCrim;
        private ObservableCollection<PERTENECE_CRIMEN_ORG> lstPertCrimenOrg;
        #region listas
        public ObservableCollection<EMI_SANCION_DISCIPLINARIA> LstSanciones
        {
            get { return lstSanciones; }
            set
            {
                lstSanciones = value;
                if (lstSanciones != null)
                {
                    if (lstSanciones.Count > 0)
                        SancionesEmpty = false;
                    else
                        SancionesEmpty = true;
                }
                else
                    SancionesEmpty = false;
                OnPropertyChanged("LstSanciones");
            }
        }
        public ObservableCollection<CLASIFICACION_CRIMINOLOGICA> LstClasCrim
        {
            get { return lstClasCrim; }
            set
            {
                lstClasCrim = value;
                OnPropertyChanged("LstClasCrim");
            }
        }
        public ObservableCollection<PERTENECE_CRIMEN_ORG> LstPertCrimenOrg
        {
            get { return lstPertCrimenOrg; }
            set
            {
                lstPertCrimenOrg = value;
                OnPropertyChanged("LstPertCrimenOrg");
            }
        }
        #endregion
        public EMI_SANCION_DISCIPLINARIA SelectedSancion
        {
            get { return _SelectedSancion; }
            set
            {
                _SelectedSancion = value;
                OnPropertyChanged("SelectedSancion");
                if (_SelectedSancion != null)
                {
                    var isInDB = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMISancionDisciplinaria().GetData(w => w.ID_EMI == _SelectedSancion.ID_EMI && w.ID_EMI_CONS == _SelectedSancion.ID_EMI_CONS &&
                    w.ID_SANCIONES_DISCIPLINARIAS == _SelectedSancion.ID_SANCIONES_DISCIPLINARIAS);
                    EliminarItemMenu = isInDB.Count() > 0 ? false : true;
                }
                else
                    EliminarItemMenu = false;
            }
        }
        public EMI_SANCION_DISCIPLINARIA NewSancion
        {
            get { return _NewSancion; }
            set
            {
                _NewSancion = value;
                OnPropertyChanged("NewSancion");
                //if (_NewSancion != null)
                //{
                //    var isInDB = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMISancionDisciplinaria().GetData(w => w.ID_EMI == emiActual.ID_EMI && w.ID_EMI_CONS == emiActual.ID_EMI_CONS &&
                //    w.ID_SANCIONES_DISCIPLINARIAS == SelectedSancion.ID_SANCIONES_DISCIPLINARIAS);
                //    EliminarItemMenu = isInDB.Count() > 0 ? false : true;
                //}
            }
        }
        #region [Clasificacion Criminologica por Antecedentes]
        public short? ClasificacionCriminologica
        {
            get { return EMI_CLAS_CRIMINOLOGICA.ID_CLAS; }
            set
            {
                EMI_CLAS_CRIMINOLOGICA.ID_CLAS = value;
                OnPropertyChanged("ClasificacionCriminologica");
            }
        }
        public short? PertenenciaCrimenOrganizado
        {
            get { return EMI_CLAS_CRIMINOLOGICA.ID_CRIMENO; }
            set
            {
                EMI_CLAS_CRIMINOLOGICA.ID_CRIMENO = value;
                OnPropertyChanged("PertenenciaCrimenOrganizado");
            }
        }
        #endregion
        #region [Sanciones Diciplinarias y/o Nuevos Procesos]
        public string MotivoProceso
        {
            get { 
                if(NewSancion != null)
                return NewSancion.MOTIVO_PROCESO;
                return string.Empty;
            }
            set
            {
                try
                {
                    if (NewSancion != null)
                    NewSancion.MOTIVO_PROCESO = value;
                    OnPropertyChanged("MotivoProceso");
                }
                catch (Exception) { }
            }
        }
        public short? Cantidad
        {
            get {
                if (NewSancion != null)
                    return NewSancion.CANTIDAD_PARTICIPACION;
                else
                    return null;
            }
            set
            {
                try
                {
                    if (NewSancion != null)
                    NewSancion.CANTIDAD_PARTICIPACION = value;
                    OnPropertyChanged("Cantidad");
                }
                catch (Exception ) { }
            }
        }
        public string TiempoSancionProceso
        {
            get {
                if (NewSancion != null)
                    return NewSancion.TIEMPO_CASTIGO_SANCION_PROCESO;
                else
                    return string.Empty;
            }
            set
            {
                try
                {
                    if (NewSancion != null)
                    NewSancion.TIEMPO_CASTIGO_SANCION_PROCESO = value;
                    OnPropertyChanged("TiempoSancionProceso");
                }
                catch (Exception) { }
            }
        }
        public bool NuevoProceso
        {
            get {
                if (NewSancion != null)
                    return NewSancion.NUEVO_PROCESO == "S" ? true : false;
                else
                    return false;
            }
            set
            {
                try
                {
                    if (NewSancion != null)
                    NewSancion.NUEVO_PROCESO = value == true ? "S" : "N";
                    OnPropertyChanged("NuevoProceso");
                }
                catch (Exception) { }
            }
        }
        #endregion
        #endregion

        #region [FACTORES CRIMINODIAGNOSTICO]
        private bool tabFactorCriminodiagnosticoSelected;
        public bool TabFactorCriminodiagnosticoSelected
        {
            get { return tabFactorCriminodiagnosticoSelected; }
            set { tabFactorCriminodiagnosticoSelected = value; OnPropertyChanged("TabFactorCriminodiagnosticoSelected"); }
        }

        private bool tabClasificacionCriminologicaPadreSelected;
        public bool TabClasificacionCriminologicaPadreSelected
        {
            get { return tabClasificacionCriminologicaPadreSelected; }
            set { tabClasificacionCriminologicaPadreSelected = value; OnPropertyChanged("TabClasificacionCriminologicaPadreSelected"); }
        }

        private ObservableCollection<EMI_FACTOR_NIVEL> lstFactorNivel;
        public ObservableCollection<EMI_FACTOR_NIVEL> LstFactorNivel
        {
            get { return lstFactorNivel; }
            set { lstFactorNivel = value; OnPropertyChanged("LstFactorNivel"); }
        }

        private ObservableCollection<EMI_FACTOR_RESULTADO> lstFactorResultado;
        public ObservableCollection<EMI_FACTOR_RESULTADO> LstFactorResultado
        {
            get { return lstFactorResultado; }
            set { lstFactorResultado = value; OnPropertyChanged("LstFactorResultado"); }
        }

        private ObservableCollection<EMI_FACTOR_UBICACION> lstFactorUbicacion;
        public ObservableCollection<EMI_FACTOR_UBICACION> LstFactorUbicacion
        {
            get { return lstFactorUbicacion; }
            set { lstFactorUbicacion = value; OnPropertyChanged("LstFactorUbicacion"); }
        }

        private short? egocentrismoSelected;
        public short? EgocentrismoSelected
        {
            get { return egocentrismoSelected; }
            set { egocentrismoSelected = value; OnPropertyChanged("EgocentrismoSelected"); }
        }

        private short? agresividadSelected;
        public short? AgresividadSelected
        {
            get { return agresividadSelected; }
            set { agresividadSelected = value; OnPropertyChanged("AgresividadSelected"); }
        }

        private short? indiferenciaAfectivaSelected;
        public short? IndiferenciaAfectivaSelected
        {
            get { return indiferenciaAfectivaSelected; }
            set { indiferenciaAfectivaSelected = value; OnPropertyChanged("IndiferenciaAfectivaSelected"); }
        }

        private short? labilidadAfectivaSelected;
        public short? LabilidadAfectivaSelected
        {
            get { return labilidadAfectivaSelected; }
            set { labilidadAfectivaSelected = value; OnPropertyChanged("LabilidadAfectivaSelected"); }
        }

        private short? adaptacionSocialSelected;
        public short? AdaptacionSocialSelected
        {
            get { return adaptacionSocialSelected; }
            set { adaptacionSocialSelected = value; OnPropertyChanged("AdaptacionSocialSelected"); }
        }

        private short? liderazgoSelected;
        public short? LiderazgoSelected
        {
            get { return liderazgoSelected; }
            set { liderazgoSelected = value; OnPropertyChanged("LiderazgoSelected"); }
        }
        
        private short? toleranciaFrustracionSelected;
        public short? ToleranciaFrustracionSelected
        {
            get { return toleranciaFrustracionSelected; }
            set { toleranciaFrustracionSelected = value; OnPropertyChanged("ToleranciaFrustracionSelected"); }
        }

        private short? controlImpulsosSelected;
        public short? ControlImpulsosSelected
        {
            get { return controlImpulsosSelected; }
            set { controlImpulsosSelected = value; OnPropertyChanged("ControlImpulsosSelected"); }
        }

        private short? capacidadCriminalSelected;
        public short? CapacidadCriminalSelected
        {
            get { return capacidadCriminalSelected; }
            set { capacidadCriminalSelected = value; OnPropertyChanged("CapacidadCriminalSelected"); }
        }

        private short? pronosticoIntrainstitucionalSelected;
        public short? PronosticoIntrainstitucionalSelected
        {
            get { return pronosticoIntrainstitucionalSelected; }
            set { pronosticoIntrainstitucionalSelected = value; OnPropertyChanged("PronosticoIntrainstitucionalSelected"); }
        }
        
        private short? indiceEstadoPeligrosoSelected;
        public short? IndiceEstadoPeligrosoSelected
        {
            get { return indiceEstadoPeligrosoSelected; }
            set { indiceEstadoPeligrosoSelected = value; OnPropertyChanged("IndiceEstadoPeligrosoSelected"); }
        }

        private short? ubicacionClasificacionCriminologicaSelected;
        public short? UbicacionClasificacionCriminologicaSelected
        {
            get { return ubicacionClasificacionCriminologicaSelected; }
            set { ubicacionClasificacionCriminologicaSelected = value; OnPropertyChanged("UbicacionClasificacionCriminologicaSelected"); }
        }

        private string dictamen;
        public string Dictamen
        {
            get { return dictamen; }
            set { dictamen = value; OnPropertyChanged("Dictamen"); }
        }

        private string dictamenAdd;
        public string DictamenAdd
        {
            get { return dictamenAdd; }
            set { dictamenAdd = value; OnPropertyChanged("DictamenAdd"); }
        }

        
        #endregion

        #region [TAB SELECTED]
        private bool tabClasificacionCriminologicaSelected;
        public bool TabClasificacionCriminologicaSelected
        {
            get { return tabClasificacionCriminologicaSelected; }
            set { tabClasificacionCriminologicaSelected = value; OnPropertyChanged("TabClasificacionCriminologicaSelected"); }
        }

        private bool tabFactoresCriminodiagnosticosSelected;
        public bool TabFactoresCriminodiagnosticosSelected
        {
            get { return tabFactoresCriminodiagnosticosSelected; }
            set { tabFactoresCriminodiagnosticosSelected = value; OnPropertyChanged("TabFactoresCriminodiagnosticosSelected"); }
        }
        #endregion

        private bool sancionesEmpty = true;
        public bool SancionesEmpty
        {
            get { return sancionesEmpty; }
            set { sancionesEmpty = value; OnPropertyChanged("SancionesEmpty"); }
        }
    }
}

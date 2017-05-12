using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales
{
    partial class RealizacionEstudiosViewModel
    {
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
        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        #endregion


        #region Datos Generales
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

        private bool _IsOtraTarea = false;

        public bool IsOtraTarea
        {
            get { return _IsOtraTarea; }
            set { _IsOtraTarea = value; OnPropertyChanged("IsOtraTarea"); }
        }

        private enum eAreaVacia
        {
            SIN_AREA = 0
        };

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
                        SelectIngreso = selectExpediente.INGRESO.OrderBy(o => o.FEC_INGRESO_CERESO).FirstOrDefault();
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

        private int CuantosActivos { get; set; }

        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                {
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
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

        #endregion

        private short _IdVentanaAcual;
        public short IdVentanaAcual
        {
            get { return _IdVentanaAcual; }
            set
            {
                _IdVentanaAcual = value;
                OnPropertyChanged("IdVentanaAcual");
            }
        }

        private enum eNombresInternosTabsComunes
        {
            //ESTE ENUMERADOR SIRVE PARA IDENTIFICAR  EL TAB AL QUE SE TIENE QUE REGRESAR AL USUARIO
            TabEstudioMedicoFC = 1,
            TabEstudioPsiqFC = 2,
            TabEstudioPsicFC = 3,
            TabCriminoDFC = 4,
            TabEstudioSocioFamFC = 5,
            TabEstudioEducCultDepFC = 6,
            TabEstudioCapacitacionTrabajoPenitFC = 7,
            TabSeguriddCustodiaFC = 8
        };

        private enum eNombresInternosTabsFederales
        {
            TabEstudioMedicoFF = 1,
            TabEstudioPsicoFF = 2,
            TabEstudioTrabajoSocialFF = 3,
            TabActivProductCapacitFF = 4,
            TabActEducCultDepRecCivFF = 5,
            TabVigilanciaFF = 6,
            TabEstudioCriminologico = 7
        };

        private PERSONALIDAD_FUERO_COMUN EstudioRealizado { get; set; }

        private enum eVentanasProceso
        {
            //DEFINCION DE ENUMERADOR PARA QUE SIRVA COMO DOCUMENTACION DEL METODO CON EL QUE SE VA A IMPRIMIR EL FORMATO EN BASE ALA VENTANA EN LA QUE SE ENCUENTRA EL USUARIO
            ESTUDIO_MEDICO_FUERO_COMUN = 1,
            ESTUDIO_PSICOLOGICO_FUERO_COMUN = 2,
            ESTUDIO_PSIQUIATRICO_FUERO_COMUN = 3,
            ESTUDIO_CRIMINODIAGNOSTICO_FUERO_COMUN = 4,
            ESTUDIO_SOCIOFAMILIAR_FUERO_COMUN = 5,
            ESTUDIO_EDUCATIVO_CULTURAL_DEPORTIVO_FUERO_COMUN = 6,
            ESTUDIO_CAPACITACION_TRABAJO_PENITENCIARIO_FUERO_COMUN = 7,
            ESTUDIO_SEGURIDAD_CUSTODIA_FUERO_COMUN = 8,
            CONSEJO_TENICO_INTERD_FUERO_FEDERAL = 9,
            ESTUDIO_MEDICO_FUERO_FEDERAL = 10,
            ESTUDIO_TRABAJO_SOCIAL_FUERO_FEDERAL = 11,
            ACTIV_PRODUCTIVAS_CAPACITACION_FUERO_FEDERAL = 12,
            ESTUDIO_ACTIV_EDUCAT_CULT_DEP_RECR_CIV_FUERO_FEDERAL = 13,
            VIGILANCIA_FUERO_FEDERAL = 14,
            ESTUDIO_CRIMINOLOGICOO_FUERO_FEDERAL = 15,
            ESTUDIO_PSICOLOGICO_FUERO_FEDERAL = 16
        };

        private enum eEstatudDetallePersonalidad
        {
            ACTIVO = 1,
            PENDIENTE = 2,
            TERMINADO = 3,
            CANCELADO = 4,
            ASIGNADO = 5
        };

        private enum eProcEstudiosNuevasUbicacionesP
        {
            ESTUDIO_CRIMINOLOGICO_DE_FUERO_COMUN = 1,
            ESTUDIO_DE_TRABAJO_SOCIAL_DE_FUERO_COMUN = 2,
            ESTUDIO_DE_SEGURIDAD_DE_FUERO_COMÚN = 3,
            ESTUDIO_MEDICO_DE_FUERO_COMUN = 4,
            ESTUDIO_PSICOLOGICO_DE_FUERO_COMUN = 5,
            ESTUDIO_PSIQUIATRICO_DE_FUERO_COMUN = 6,
            ESTUDIO_PEDAGOGICO_DE_FUERO_COMUN = 7,
            ESTUDIO_LABORAL_DE_FUERO_COMUN = 8,
            ESTUDIO_CRIMINOLOGICO_DE_FUERO_FEDERAL = 9,
            ESTUDIO_DE_TRABAJO_SOCIAL_DE_FUERO_FEDERAL = 10,
            ESTUDIO_DE_SEGURIDAD_DE_FUERO_FEDERAL = 11,
            ESTUDIO_MEDICO_DE_FUERO_FEDERAL = 12,
            ESTUDIO_PSIQUIATRICO_DE_FUERO_FEDERAL = 13,
            ESTUDIO_PSICOLOGICO_DE_FUERO_FEDERAL = 14,
            ESTUDIO_PEDAGOGICO_DE_FUERO_FEDERAL = 15,
            ESTUDIO_LABORAL_DE_FUERO_FEDERAL = 16
        };

        private enum eTiposAduana
        {
            VISITA = 3
        };

        private enum eFrecuencia
        {
            SEMANAL = 4,
            QUINCENAL = 3,
            MENSUAL = 2,
            ANUAL = 1,
            SIN_DATO = 0
        };

        private enum eEstatusGrupos
        {
            ACTIVO = 1,
            SUSPENDIDO = 4,
            CONCLUIDO = 5,
            CANCELADO = 2
        };

        #region Enable en TABS
        #region Fuero Comun
        private bool _TabEstudiosFueroComun = false;
        public bool TabEstudiosFueroComun
        {
            get { return _TabEstudiosFueroComun; }
            set
            {
                _TabEstudiosFueroComun = value;
                //if (value)
                //    IdVentanaAcual = new short();

                OnPropertyChanged("TabEstudiosFueroComun");
                //OnPropertyChanged("IdVentanaAcual");
            }
        }

        /// <summary>
        /// ESTE ES EL INDICADOR PARA SABER CUAL FUE EL ULTIMO ELEGIDO, PARA SABER SI NECESITO QUE TE VUELVAS A AUTENTIFICAR COMO INTERNO Y CAMBIAR LA UBICACION
        /// </summary>
        private string NombreUltimoTabElegido { get; set; }
        private bool EstaNavegandoTabs { get; set; }
        public bool PuedoGuardar { get; set; }
        private bool _valida;
        private bool _TabEstudioMedicoFC = false;
        public bool TabEstudioMedicoFC
        {
            get { return _TabEstudioMedicoFC; }
            set
            {
                _TabEstudioMedicoFC = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_COMUN;
                //else
                //    if (EnabledComun1)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_COMUN);

                OnPropertyChanged("TabEstudioMedicoFC");
                OnPropertyChanged("IdVentanaAcual");
            }
        }

        private bool _TabEstudioPsiqFC = false;
        public bool TabEstudioPsiqFC
        {
            get { return _TabEstudioPsiqFC; }
            set
            {
                _TabEstudioPsiqFC = value;

                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSIQUIATRICO_FUERO_COMUN;
                //else
                //    if (EnabledComun2)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_PSIQUIATRICO_FUERO_COMUN);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabEstudioPsiqFC");
            }
        }

        private bool _TabEstudioPsicFC = false;
        public bool TabEstudioPsicFC
        {
            get { return _TabEstudioPsicFC; }
            set
            {
                _TabEstudioPsicFC = value;

                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_COMUN;
                //else
                //    if (EnabledComun3)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_COMUN);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabEstudioPsicFC");
            }
        }

        private bool _TabCriminoDFC = false;
        public bool TabCriminoDFC
        {
            get { return _TabCriminoDFC; }
            set
            {
                _TabCriminoDFC = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CRIMINODIAGNOSTICO_FUERO_COMUN;
                //else
                //    if (EnabledComun4)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_CRIMINODIAGNOSTICO_FUERO_COMUN);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabCriminoDFC");
            }
        }

        private bool _TabEstudioSocioFamFC = false;
        public bool TabEstudioSocioFamFC
        {
            get { return _TabEstudioSocioFamFC; }
            set
            {
                _TabEstudioSocioFamFC = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_SOCIOFAMILIAR_FUERO_COMUN;
                //else
                //    if (EnabledComun5)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_SOCIOFAMILIAR_FUERO_COMUN);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabEstudioSocioFamFC");
            }
        }

        private bool _TabEstudioEducCultDepFC = false;
        public bool TabEstudioEducCultDepFC
        {
            get { return _TabEstudioEducCultDepFC; }
            set
            {
                _TabEstudioEducCultDepFC = value;

                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_EDUCATIVO_CULTURAL_DEPORTIVO_FUERO_COMUN;
                //else
                //    if (EnabledComun6)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_EDUCATIVO_CULTURAL_DEPORTIVO_FUERO_COMUN);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabEstudioEducCultDepFC");
            }
        }

        private bool _TabEstudioCapacitacionTrabajoPenitFC = false;
        public bool TabEstudioCapacitacionTrabajoPenitFC
        {
            get { return _TabEstudioCapacitacionTrabajoPenitFC; }
            set
            {
                _TabEstudioCapacitacionTrabajoPenitFC = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CAPACITACION_TRABAJO_PENITENCIARIO_FUERO_COMUN;
                //else
                //    if (EnabledComun7)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_CAPACITACION_TRABAJO_PENITENCIARIO_FUERO_COMUN);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabEstudioCapacitacionTrabajoPenitFC");
            }
        }

        private bool _TabSeguriddCustodiaFC = false;
        public bool TabSeguriddCustodiaFC
        {
            get { return _TabSeguriddCustodiaFC; }
            set
            {
                _TabSeguriddCustodiaFC = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_SEGURIDAD_CUSTODIA_FUERO_COMUN;
                //else
                //    if (EnabledComun8)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_SEGURIDAD_CUSTODIA_FUERO_COMUN);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabSeguriddCustodiaFC");
            }
        }
        #endregion
        private bool _TabFueroFederal = false;
        public bool TabFueroFederal
        {
            get { return _TabFueroFederal; }
            set
            {
                _TabFueroFederal = value;
                OnPropertyChanged("TabFueroFederal");
            }
        }

        private bool _TabConsejoTecInterdFF = false;
        public bool TabConsejoTecInterdFF
        {
            get { return _TabConsejoTecInterdFF; }
            set
            {
                _TabConsejoTecInterdFF = value;

                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.CONSEJO_TENICO_INTERD_FUERO_FEDERAL;

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabConsejoTecInterdFF");
            }
        }

        private bool _TabEstudioMedicoFF = false;
        public bool TabEstudioMedicoFF
        {
            get { return _TabEstudioMedicoFF; }
            set
            {
                _TabEstudioMedicoFF = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_FEDERAL;
                //else
                //    if (EnabledFederal1)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_FEDERAL);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabEstudioMedicoFF");
            }
        }

        private bool _TabEstudioPsicoFF = false;
        public bool TabEstudioPsicoFF
        {
            get { return _TabEstudioPsicoFF; }
            set
            {
                _TabEstudioPsicoFF = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_FEDERAL;
                //else
                //    if (EnabledFederal2)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_FEDERAL);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabEstudioPsicoFF");
            }
        }

        private bool _TabEstudioTrabajoSocialFF = false;
        public bool TabEstudioTrabajoSocialFF
        {
            get { return _TabEstudioTrabajoSocialFF; }
            set
            {
                _TabEstudioTrabajoSocialFF = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_TRABAJO_SOCIAL_FUERO_FEDERAL;
                //else
                //    if (EnabledFederal3)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_TRABAJO_SOCIAL_FUERO_FEDERAL);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabEstudioTrabajoSocialFF");
            }
        }

        private bool _TabActivProductCapacitFF = false;
        public bool TabActivProductCapacitFF
        {
            get { return _TabActivProductCapacitFF; }
            set
            {
                _TabActivProductCapacitFF = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ACTIV_PRODUCTIVAS_CAPACITACION_FUERO_FEDERAL;
                //else
                //    if (EnabledFederal4)
                //        GuardaAislado((short)eVentanasProceso.ACTIV_PRODUCTIVAS_CAPACITACION_FUERO_FEDERAL);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabActivProductCapacitFF");
            }
        }

        private bool _TabActEducCultDepRecCivFF = false;
        public bool TabActEducCultDepRecCivFF
        {
            get { return _TabActEducCultDepRecCivFF; }
            set
            {
                _TabActEducCultDepRecCivFF = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_ACTIV_EDUCAT_CULT_DEP_RECR_CIV_FUERO_FEDERAL;
                //else
                //    if (EnabledFederal5)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_ACTIV_EDUCAT_CULT_DEP_RECR_CIV_FUERO_FEDERAL);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabActEducCultDepRecCivFF");
            }
        }

        private bool _TabVigilanciaFF = false;
        public bool TabVigilanciaFF
        {
            get { return _TabVigilanciaFF; }
            set
            {
                _TabVigilanciaFF = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.VIGILANCIA_FUERO_FEDERAL;
                //else
                //    if (EnabledFederal6)
                //        GuardaAislado((short)eVentanasProceso.VIGILANCIA_FUERO_FEDERAL);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabVigilanciaFF");
            }
        }

        private bool _TabEstudioCriminologico = false;
        public bool TabEstudioCriminologico
        {
            get { return _TabEstudioCriminologico; }
            set
            {
                _TabEstudioCriminologico = value;
                if (value)
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CRIMINOLOGICOO_FUERO_FEDERAL;
                //else
                //    if (EnabledFederal7)
                //        GuardaAislado((short)eVentanasProceso.ESTUDIO_CRIMINOLOGICOO_FUERO_FEDERAL);

                OnPropertyChanged("IdVentanaAcual");
                OnPropertyChanged("TabEstudioCriminologico");
            }
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

        private bool _BuscarReadOnly;

        public bool BuscarReadOnly
        {
            get { return _BuscarReadOnly; }
            set { _BuscarReadOnly = value; OnPropertyChanged("BuscarReadOnly"); }
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

        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }

        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set { aceptarBusquedaHuellaFocus = value; OnPropertyChanged("AceptarBusquedaHuellaFocus"); }
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

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set { pInsertar = value; }
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
            set { pConsultar = value; }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set { pImprimir = value; }
        }
        #endregion


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


        private bool _EnabledComun1 = false;

        public bool EnabledComun1
        {
            get { return _EnabledComun1; }
            set { _EnabledComun1 = value; OnPropertyChanged("EnabledComun1"); }
        }

        private bool _EnabledComun2 = false;

        public bool EnabledComun2
        {
            get { return _EnabledComun2; }
            set { _EnabledComun2 = value; OnPropertyChanged("EnabledComun2"); }
        }

        private bool _EnabledComun3 = false;

        public bool EnabledComun3
        {
            get { return _EnabledComun3; }
            set { _EnabledComun3 = value; OnPropertyChanged("EnabledComun3"); }
        }

        private bool _EnabledComun4 = false;

        public bool EnabledComun4
        {
            get { return _EnabledComun4; }
            set { _EnabledComun4 = value; OnPropertyChanged("EnabledComun4"); }
        }

        private bool _EnabledComun5 = false;

        public bool EnabledComun5
        {
            get { return _EnabledComun5; }
            set { _EnabledComun5 = value; OnPropertyChanged("EnabledComun5"); }
        }

        private bool _EnabledComun6 = false;

        public bool EnabledComun6
        {
            get { return _EnabledComun6; }
            set { _EnabledComun6 = value; OnPropertyChanged("EnabledComun6"); }
        }

        private bool _EnabledComun7 = false;

        public bool EnabledComun7
        {
            get { return _EnabledComun7; }
            set { _EnabledComun7 = value; OnPropertyChanged("EnabledComun7"); }
        }

        private bool _EnabledComun8 = false;

        public bool EnabledComun8
        {
            get { return _EnabledComun8; }
            set { _EnabledComun8 = value; OnPropertyChanged("EnabledComun8"); }
        }

        private bool _EnabledFederal1 = false;

        public bool EnabledFederal1
        {
            get { return _EnabledFederal1; }
            set { _EnabledFederal1 = value; OnPropertyChanged("EnabledFederal1"); }
        }

        private bool _EnabledFederal2 = false;

        public bool EnabledFederal2
        {
            get { return _EnabledFederal2; }
            set { _EnabledFederal2 = value; OnPropertyChanged("EnabledFederal2"); }
        }

        private bool _EnabledFederal3 = false;

        public bool EnabledFederal3
        {
            get { return _EnabledFederal3; }
            set { _EnabledFederal3 = value; OnPropertyChanged("EnabledFederal3"); }
        }

        private bool _EnabledFederal4 = false;

        public bool EnabledFederal4
        {
            get { return _EnabledFederal4; }
            set { _EnabledFederal4 = value; OnPropertyChanged("EnabledFederal4"); }
        }

        private bool _EnabledFederal5 = false;

        public bool EnabledFederal5
        {
            get { return _EnabledFederal5; }
            set { _EnabledFederal5 = value; OnPropertyChanged("EnabledFederal5"); }
        }

        private bool _EnabledFederal6 = false;

        public bool EnabledFederal6
        {
            get { return _EnabledFederal6; }
            set { _EnabledFederal6 = value; OnPropertyChanged("EnabledFederal6"); }
        }

        private bool _EnabledFederal7 = false;

        public bool EnabledFederal7
        {
            get { return _EnabledFederal7; }
            set { _EnabledFederal7 = value; OnPropertyChanged("EnabledFederal7"); }
        }

        private bool _MenuFichaEnabled = false;

        public bool MenuFichaEnabled
        {
            get { return _MenuFichaEnabled; }
            set { _MenuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }

        private short _IdIndex;

        public short IdIndex
        {
            get { return _IdIndex; }
            set { _IdIndex = value; OnPropertyChanged("IdIndex"); }
        }

        private short? _SelectedTipoPNuev = -1;
        public short? SelectedTipoPNuev
        {
            get { return _SelectedTipoPNuev; }
            set { _SelectedTipoPNuev = value; OnPropertyChanged("SelectedTipoPNuev"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ACTIVIDAD> _lstActividadesNuevasEdicion;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ACTIVIDAD> LstActividadesNuevasEdicion
        {
            get { return _lstActividadesNuevasEdicion; }
            set { _lstActividadesNuevasEdicion = value; OnPropertyChanged("LstActividadesNuevasEdicion"); }
        }

        private bool _EnabledEdicionProgPsicoComun = true;

        public bool EnabledEdicionProgPsicoComun
        {
            get { return _EnabledEdicionProgPsicoComun; }
            set { _EnabledEdicionProgPsicoComun = value; OnPropertyChanged("EnabledEdicionProgPsicoComun"); }
        }

        private enum eTiposP
        {
            PROGRAMA_DESHABITUAMIENTO = 5,
            MODIFICACION_CONDUCTA = 6,
            COMPLEMENTARIO = 10,
            TALLERES_ORIENTACION = 127,
            APOYO_ESPIRITUAL = 9,
            FORTALECIMIENTO_NUCLEO_FAMILIAR = 11,
            EDUCATIVAS = 167,
            CULTURALES = 168,
            DEPORTIVAS = 169,
            CAPACITACION_LABORAL = 147,
            ACTIVIDADES_GRATIFICADAS = 149,
            ACTIVIDADES_NO_GRATIFICADAS = 148
        };

        private bool _EnablededitarCampoCapacitacionComun = true;
        public bool EnablededitarCampoCapacitacionComun
        {
            get { return _EnablededitarCampoCapacitacionComun; }
            set { _EnablededitarCampoCapacitacionComun = value; OnPropertyChanged("EnablededitarCampoCapacitacionComun"); }
        }
        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ACTIVIDAD> lstAcvidadesCongregacionesTSComun;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ACTIVIDAD> LstAcvidadesCongregacionesTSComun
        {
            get { return lstAcvidadesCongregacionesTSComun; }
            set { lstAcvidadesCongregacionesTSComun = value; OnPropertyChanged("LstAcvidadesCongregacionesTSComun"); }
        }

        private short? _SelectedCongActiv = -1;
        public short? SelectedCongActiv
        {
            get { return _SelectedCongActiv; }
            set { _SelectedCongActiv = value; OnPropertyChanged("SelectedCongActiv"); }
        }

        private bool _EnabledCongregacionSocComun = true;
        public bool EnabledCongregacionSocComun
        {
            get { return _EnabledCongregacionSocComun; }
            set { _EnabledCongregacionSocComun = value; OnPropertyChanged("EnabledCongregacionSocComun"); }
        }
        private short IdEdicionProgramasPsicologicosComunes { get; set; }
    }
}
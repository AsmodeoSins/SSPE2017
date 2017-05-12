using Andora.UserControlLibrary;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ControlPenales
{
    partial class PadronVisitasViewModel
    {
        #region Listas
        private ObservableCollection<TipoDocumento> AuxListTipoDocumento;
        ObservableCollection<TipoDocumento> _ListTipoDocumento;
        public ObservableCollection<TipoDocumento> ListTipoDocumento
        {
            get { return _ListTipoDocumento; }
            set
            {
                _ListTipoDocumento = value;
                OnPropertyChanged("ListTipoDocumento");
            }
        }
        private ObservableCollection<VISITA_AUTORIZADA> listVisitaAutorizada;
        public ObservableCollection<VISITA_AUTORIZADA> ListVisitaAutorizada
        {
            get { return listVisitaAutorizada; }
            set { listVisitaAutorizada = value; OnPropertyChanged("ListVisitaAutorizada"); }
        }
        private ObservableCollection<PASE_CATALOGO> listTipoPase;
        public ObservableCollection<PASE_CATALOGO> ListTipoPase
        {
            get { return listTipoPase; }
            set { listTipoPase = value; OnPropertyChanged("ListTipoPase"); }
        }
        private ObservableCollection<ESTATUS_VISITA> listSituacion;
        public ObservableCollection<ESTATUS_VISITA> ListSituacion
        {
            get { return listSituacion; }
            set { listSituacion = value; OnPropertyChanged("ListSituacion"); }
        }
        private ObservableCollection<TIPO_REFERENCIA> listTipoRelacion;
        public ObservableCollection<TIPO_REFERENCIA> ListTipoRelacion
        {
            get { return listTipoRelacion; }
            set { listTipoRelacion = value; OnPropertyChanged("ListTipoRelacion"); }
        }
        private ObservableCollection<TIPO_VISITANTE> listTipoVisitante;
        public ObservableCollection<TIPO_VISITANTE> ListTipoVisitante
        {
            get { return listTipoVisitante; }
            set
            {
                listTipoVisitante = value;
                OnPropertyChanged("ListTipoVisitante");
            }
        }
        private ObservableCollection<TIPO_VISITA> listTipoVisita;
        public ObservableCollection<TIPO_VISITA> ListTipoVisita
        {
            get
            {
                return listTipoVisita;
            }
            set
            {
                listTipoVisita = value;
                OnPropertyChanged("ListTipoVisita");
            }
        }
        private ObservableCollection<AREA> _ListAreaVisita;
        public ObservableCollection<AREA> ListAreaVisita
        {
            get
            {
                return _ListAreaVisita;
            }
            set
            {
                _ListAreaVisita = value;
                OnPropertyChanged("ListAreaVisita");
            }
        }
        private ObservableCollection<PAIS_NACIONALIDAD> listPais;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPais
        {
            get { return listPais; }
            set { listPais = value; OnPropertyChanged("ListPais"); }
        }
        private ObservableCollection<ENTIDAD> listEntidad;
        public ObservableCollection<ENTIDAD> ListEntidad
        {
            get { return listEntidad; }
            set { listEntidad = value; OnPropertyChanged("ListEntidad"); }
        }
        private ObservableCollection<TIPO_DISCAPACIDAD> listDiscapacidades;
        public ObservableCollection<TIPO_DISCAPACIDAD> ListDiscapacidades
        {
            get { return listDiscapacidades; }
            set { listDiscapacidades = value; OnPropertyChanged("ListDiscapacidades"); }
        }
        private List<VISITA_DIA> ListDias;

        private ObservableCollection<MUNICIPIO> ListMunicipioAsignacion;
        private ObservableCollection<MUNICIPIO> ListMunicipioPadron;
        private ObservableCollection<MUNICIPIO> ListMunicipioEntrega;
        public ObservableCollection<MUNICIPIO> ListMunicipio
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return ListMunicipioAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return ListMunicipioPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return ListMunicipioEntrega;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    ListMunicipioAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    ListMunicipioPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    ListMunicipioEntrega = value;
                OnPropertyChanged("ListMunicipio");
            }
        }
        private ObservableCollection<COLONIA> ListColoniaEntrega;
        private ObservableCollection<COLONIA> ListColoniaAsignacion;
        private ObservableCollection<COLONIA> ListColoniaPadron;
        public ObservableCollection<COLONIA> ListColonia
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return ListColoniaAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return ListColoniaPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return ListColoniaEntrega;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    ListColoniaAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    ListColoniaPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    ListColoniaEntrega = value;
                OnPropertyChanged("ListColonia");
            }
        }
        private ObservableCollection<PERSONAVISITAAUXILIAR> listVisitantesPadron;
        private ObservableCollection<PERSONAVISITAAUXILIAR> listVisitantesCredencial;
        public ObservableCollection<PERSONAVISITAAUXILIAR> ListVisitantes
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return listVisitantesAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return listVisitantesPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return listVisitantesCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    listVisitantesPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    listVisitantesCredencial = value;
                OnPropertyChanged("ListVisitantes");
            }
        }

        public ObservableCollection<PERSONAVISITAAUXILIAR> ListVisitantesImputadoAsignacion { get; set; }
        public ObservableCollection<PERSONAVISITAAUXILIAR> ListVisitantesImputadoPadron { get; set; }
        public ObservableCollection<PERSONAVISITAAUXILIAR> ListVisitantesImputado
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? ListVisitantesImputadoAsignacion : ListVisitantesImputadoPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    ListVisitantesImputadoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    ListVisitantesImputadoPadron = value;
                OnPropertyChanged("ListVisitantesImputado");
            }
        }
        private ListaVisitaAgenda _SelectVisitaProgramada;
        public ListaVisitaAgenda SelectVisitaProgramada
        {
            get { return _SelectVisitaProgramada; }
            set { _SelectVisitaProgramada = value; OnPropertyChanged("SelectVisitaProgramada"); }
        }
        private ObservableCollection<ListaVisitaAgenda> ListProgramacionVisitaAux;
        private ObservableCollection<ListaVisitaAgenda> _ListProgramacionVisita;
        public ObservableCollection<ListaVisitaAgenda> ListProgramacionVisita
        {
            get { return _ListProgramacionVisita; }
            set
            {
                _ListProgramacionVisita = value;
                OnPropertyChanged("ListProgramacionVisita");
            }
        }
        public ObservableCollection<PERSONAVISITAAUXILIAR> ListadoInternosAuxiliar { get; set; }
        public ObservableCollection<VISITANTE_INGRESO> ListadoInternos
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return _ListadoInternosAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return _ListadoInternosPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return _ListadoInternosCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    _ListadoInternosAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    _ListadoInternosPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    _ListadoInternosCredencial = value;

                OnPropertyChanged("ListadoInternos");
            }
        }
        public ObservableCollection<ACOMPANANTE> ListAcompanantes
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return ListAcompanantesAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return ListAcompanantesPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return listAcompanantesCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    ListAcompanantesAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    ListAcompanantesPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    listAcompanantesCredencial = value;
                OnPropertyChanged("ListAcompanantes");
            }
        }

        #region PADRON
        private ObservableCollection<VISITANTE_INGRESO> _ListadoInternosPadron;
        private ObservableCollection<ACOMPANANTE> ListAcompanantesPadron;
        #endregion

        #region ASIGNACION
        private ObservableCollection<PERSONAVISITAAUXILIAR> listVisitantesAsignacion;
        public ObservableCollection<PERSONAVISITAAUXILIAR> ListVisitantesAsignacion
        {
            get { return listVisitantesAsignacion; }
            set { listVisitantesAsignacion = value; OnPropertyChanged("ListVisitantesAsignacion"); }
        }
        private ObservableCollection<VISITANTE_INGRESO> _ListadoInternosAsignacion;
        private ObservableCollection<ACOMPANANTE> listAcompanantesAsignacion;
        public ObservableCollection<ACOMPANANTE> ListAcompanantesAsignacion
        {
            get { return listAcompanantesAsignacion; }
            set { listAcompanantesAsignacion = value; OnPropertyChanged("ListAcompanantesAsignacion"); }
        }
        #endregion

        #region [CREDENCIAL]
        private ObservableCollection<VISITANTE_INGRESO> _ListadoInternosCredencial;
        private ObservableCollection<ACOMPANANTE> listAcompanantesCredencial;
        #endregion
        #endregion

        #region Select
        private string SelectDiaVisita { get; set; }
        DigitalizarDocumento escaner = new DigitalizarDocumento(Application.Current.Windows[0]);
        private TabsVisita _SelectedTab = TabsVisita.ASIGNACION_DE_VISITAS;
        internal TabsVisita SelectedTab
        {
            get { return _SelectedTab; }
            set { _SelectedTab = value; }
        }
        TipoDocumento _SelectedTipoDocumento;
        public TipoDocumento SelectedTipoDocumento
        {
            get { return _SelectedTipoDocumento; }
            set
            {
                DocumentoDigitalizado = null;
                _SelectedTipoDocumento = value;
                OnPropertyChanged("SelectedTipoDocumento");
            }
        }
        public INGRESO SelectImputadoIngreso
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return SelectImputadoIngresoAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return SelectImputadoIngresoPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return SelectImputadoIngresoCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectImputadoIngresoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    SelectImputadoIngresoPadron = value;
                    if (value != null)
                        AcompananteVisible = FechaNacimiento.HasValue ? SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO ? Visibility.Visible : (((Fechas.GetFechaDateServer - FechaNacimiento.Value).TotalDays / 365) <= Parametro.MAYORIA_EDAD) ? Visibility.Visible : Visibility.Collapsed : TextEdad.HasValue ? SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO ? Visibility.Visible : TextEdad.Value <= Parametro.EDAD_MENOR_M ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                }
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    SelectImputadoIngresoCredencial = value;
                OnPropertyChanged("SelectImputadoIngreso");
            }
        }
        private INGRESO SelectImputadoIngresoPadron;
        private INGRESO SelectImputadoIngresoAsignacion;
        private INGRESO SelectImputadoIngresoCredencial;
        private VISITA_AUTORIZADA selectVisitanteAutorizado;
        public VISITA_AUTORIZADA SelectVisitanteAutorizado
        {
            get { return selectVisitanteAutorizado; }
            set { selectVisitanteAutorizado = value; OnPropertyChanged("SelectVisitanteAutorizado"); }
        }
        private short selectParentescoListaTonta;
        public short SelectParentescoListaTonta
        {
            get { return selectParentescoListaTonta; }
            set { selectParentescoListaTonta = value; OnPropertyChanged("SelectParentescoListaTonta"); }
        }
        private short selectTipoPase;
        public short SelectTipoPase
        {
            get { return selectTipoPase; }
            set
            {
                if (TipoPaseAbre)
                {
                    if (value == 3)
                    {
                        TipoPaseAbre = !(selectTipoPase == 3);
                        selectTipoPase = value;
                        OnPropertyChanged("SelectTipoPase");
                    }
                }
                else
                {
                    selectTipoPase = value;
                    OnPropertyChanged("SelectTipoPase");
                }
            }
        }
        public List<SSP.Servidor.PERSONA_BIOMETRICO> PersonaBiometrico { get; set; }
        private PERSONAVISITAAUXILIAR selectNuevoImputadoVisitante;
        public PERSONAVISITAAUXILIAR SelectNuevoImputadoVisitante
        {
            get { return selectNuevoImputadoVisitante; }
            set { selectNuevoImputadoVisitante = value; OnPropertyChanged("SelectNuevoImputadoVisitante"); }
        }
        private VISITANTE_INGRESO SelectVisitanteIngresoAsignacion;
        private VISITANTE_INGRESO SelectVisitanteIngresoPadron;
        private VISITANTE_INGRESO SelectVisitanteIngresoCredencial;
        public VISITANTE_INGRESO SelectVisitanteIngreso
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return SelectVisitanteIngresoAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return SelectVisitanteIngresoPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return SelectVisitanteIngresoCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectVisitanteIngresoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectVisitanteIngresoPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    SelectVisitanteIngresoCredencial = value;
                OnPropertyChanged("SelectVisitanteIngreso");
            }
        }
        private short? selectEstatusRelacionPadron;
        private short? selectEstatusRelacionCredencial;
        private short? selectParentescoNuevoImputado;
        public short? SelectParentescoNuevoImputado
        {
            get { return selectParentescoNuevoImputado; }
            set { selectParentescoNuevoImputado = value; OnPropertyChanged("SelectParentescoNuevoImputado"); }
        }
        public short? SelectEstatusRelacion
        {
            get
            {
                switch (SelectedTab)
                {
                    //case TabsVisita.ASIGNACION_DE_VISITAS:
                    //    return SelectVisitanteAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return selectEstatusRelacionPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return selectEstatusRelacionCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    selectEstatusRelacionPadron = value;
                    if (value == (short)enumEstatusVisita.AUTORIZADO)
                        Credencializado = true;
                    else
                        Credencializado = false;
                }
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    selectEstatusRelacionCredencial = value;


                OnPropertyChanged("SelectEstatusRelacion");
            }
        }

        private bool credencializado = false;
        public bool Credencializado
        {
            get { return credencializado; }
            set { credencializado = value; OnPropertyChanged("Credencializado"); }
        }

        public SSP.Servidor.PERSONA SelectPersonaVisitante
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectPersonaVisitanteAsignacion : SelectPersonaVisitantePadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectPersonaVisitanteAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectPersonaVisitantePadron = value;

                OnPropertyChanged("SelectPersonaVisitante");
            }
        }
        private string deshabilitarVisitaAutorizada = "Habilitar";
        public string DeshabilitarVisitaAutorizada
        {
            get { return deshabilitarVisitaAutorizada; }
            set { deshabilitarVisitaAutorizada = value; OnPropertyChanged("DeshabilitarVisitaAutorizada"); }
        }
        public PERSONAVISITAAUXILIAR SelectVisitanteInterno
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectVisitanteInternoAsignacion : SelectVisitanteInternoPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectVisitanteInternoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectVisitanteInternoPadron = value;

                ListTipoDocumento = null;
                if (value != null && value.OBJETO_PERSONA != null)
                    ContextMenuBorrarVisitanteEnabled = false;
                else
                {
                    ContextMenuBorrarVisitanteEnabled = true;
                    if (value != null)
                    {
                        if (value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.ESTATUS == 1 : false)
                            DeshabilitarVisitaAutorizada = "Habilitar";
                        if (value.OBJETO_VISITA_AUTORIZADA != null ? value.OBJETO_VISITA_AUTORIZADA.ESTATUS == 0 : false)
                            DeshabilitarVisitaAutorizada = "Deshabilitar";
                    }
                }
                OnPropertyChanged("SelectVisitanteInterno");
            }
        }
        public PERSONAVISITAAUXILIAR SelectVisitante
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return SelectVisitanteAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return SelectVisitantePadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return SelectVisitanteCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectVisitanteAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    SelectVisitantePadron = value;
                    DigitalizarDocumentosEnabled = value != null ? value.OBJETO_PERSONA != null ? true : false : false;
                    if (value != null && value.OBJETO_PERSONA != null)
                        ImagenPersona = value.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                                            value.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                                new Imagenes().getImagenPerson();

                }
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                {
                    SelectVisitanteCredencial = value;
                    if (value != null && value.OBJETO_PERSONA != null)
                        ImagenPersona = value.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                                            value.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                                new Imagenes().getImagenPerson();

                }
                OnPropertyChanged("SelectVisitante");
            }
        }
        public short? SelectTipoVisitante
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return SelectTipoVisitanteAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return SelectTipoVisitantePadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return SelectTipoVisitanteCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                base.RemoveRule("SelectDiscapacitado");
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectTipoVisitanteAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    //SelectAccesoUnico = "X";
                    SelectTipoVisitantePadron = value;
                    if (value == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO)
                    {
                        //DiscapacitadoEnabled = true;
                        //SelectDiscapacitado = "";
                        //base.AddRule(() => SelectDiscapacitado, () => !string.IsNullOrEmpty(SelectDiscapacitado), "DISCAPACIDAD ES REQUERIDO!");
                        //OnPropertyChanged("SelectDiscapacitado");
                    }
                    else if (value == Parametro.ID_TIPO_VISITANTE_FORANEO)
                    {
                        //DiscapacitadoEnabled = false;
                        //SelectAccesoUnico = string.IsNullOrEmpty(SelectAccesoUnico) ? "X" : SelectAccesoUnico == "S" || SelectAccesoUnico == "N" ? SelectAccesoUnico : "X";
                        //SetValidacionesGenerales();
                    }
                    else
                    {
                        //DiscapacitadoEnabled = false;
                        //SelectDiscapacitado = null;
                        //SelectDiscapacitado = "";
                        //SelectDiscapacidad = 0;
                    }
                    if (SelectDiscapacitado == "S")
                        if (SelectImputadoIngreso != null)
                            AcompananteVisible = FechaNacimiento.HasValue ? SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO ? Visibility.Visible : (((Fechas.GetFechaDateServer - FechaNacimiento.Value).TotalDays / 365) <= Parametro.MAYORIA_EDAD) ? Visibility.Visible : Visibility.Collapsed : TextEdad.HasValue ? SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO ? Visibility.Visible : TextEdad.Value <= Parametro.EDAD_MENOR_M ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                }
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    SelectTipoVisitanteCredencial = value;
                OnPropertyChanged("SelectTipoVisitante");
            }
        }
        public short? SelectAreaVisita
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectAreaVisitaAsignacion : SelectAreaVisitaPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectAreaVisitaAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectAreaVisitaPadron = value;

                OnPropertyChanged("SelectAreaVisita");
            }
        }
        public short? SelectTipoVisita
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectTipoVisitaAsignacion : SelectTipoVisitaPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectTipoVisitaAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectTipoVisitaPadron = value;

                OnPropertyChanged("SelectTipoVisita");
            }
        }
        public string SelectSexo
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return SelectSexoAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return SelectSexoPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return SelectSexoCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectSexoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    SelectSexoPadron = value;
                    if (SelectImputadoIngreso != null)
                        AcompananteVisible = FechaNacimiento.HasValue ? SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO ? Visibility.Visible : (((Fechas.GetFechaDateServer - FechaNacimiento.Value).TotalDays / 365) <= Parametro.MAYORIA_EDAD) ? Visibility.Visible : Visibility.Collapsed : TextEdad.HasValue ? SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO ? Visibility.Visible : TextEdad.Value <= Parametro.EDAD_MENOR_M ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                }
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    SelectSexoCredencial = value;
                OnPropertyChanged("SelectSexo");
            }
        }
        public DateTime? FechaNacimiento
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return FechaNacimientoAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return FechaNacimientoPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return FechaNacimientoCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    FechaNacimientoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    FechaNacimientoPadron = value;
                    TextEdad = value != null ? new Fechas().CalculaEdad(value) : new Nullable<short>();
                    if (SelectImputadoIngreso != null)
                        AcompananteVisible = FechaNacimiento.HasValue ? SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO ? Visibility.Visible : (((Fechas.GetFechaDateServer - FechaNacimiento.Value).TotalDays / 365) <= Parametro.MAYORIA_EDAD) ? Visibility.Visible : Visibility.Collapsed : TextEdad.HasValue ? SelectTipoVisitante == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO ? Visibility.Visible : TextEdad.Value <= Parametro.EDAD_MENOR_M ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                    if (!string.IsNullOrEmpty(TextMaterno) && !string.IsNullOrEmpty(TextPaterno) && !string.IsNullOrEmpty(TextNombre)
                        && FechaNacimiento != null && (!BanderaEditar ||
                        SelectVisitante == null ? true : SelectVisitante.OBJETO_PERSONA == null ?
                        true : string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.RFC)))
                        TextRfc = CURPRFC.CalcularRFC(TextNombre, TextPaterno, TextMaterno, FechaNacimiento.Value.ToString("yyMMdd"));

                    if (!string.IsNullOrEmpty(TextMaterno) && !string.IsNullOrEmpty(TextPaterno) && !string.IsNullOrEmpty(TextNombre)
                        && FechaNacimiento != null && (!BanderaEditar ||
                        SelectVisitante == null ? true : SelectVisitante.OBJETO_PERSONA == null ?
                        true : string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.CURP)))
                        TextCurp = CURPRFC.CalcularCURP(TextNombre, TextPaterno, TextMaterno, FechaNacimiento.Value.ToString("yyMMdd"));
                }
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                {
                    FechaNacimientoCredencial = value;
                    if (value.HasValue)
                    {
                        var hoy = Fechas.GetFechaDateServer;
                        var edad = hoy.Year - value.Value.Year;
                        if (hoy.Month < value.Value.Month || (hoy.Month == value.Value.Month && hoy.Day < value.Value.Day))
                            edad--;
                        TextEdad = (short?)edad;
                    }
                }
                OnPropertyChanged("FechaNacimiento");
            }
        }
        public short? SelectSituacion
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return SelectSituacionAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return SelectSituacionPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return SelectSituacionCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectSituacionAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectSituacionPadron = SelectVisitante != null ? SelectVisitante.OBJETO_VISITA_AUTORIZADA != null && SelectAccesoUnico ? 13 : value : value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    SelectSituacionCredencial = value;

                OnPropertyChanged("SelectSituacion");
            }
        }
        public bool SelectAccesoUnico
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectAccesoUnicoAsignacion : SelectAccesoUnicoPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectAccesoUnicoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    SelectAccesoUnicoPadron = value;
                    if (value)
                    {
                        PaseEnabled = true;
                        SelectTipoPase = (short)3;
                        //DigitalizarDocumentosEnabled = value ? SelectVisitante != null : false;
                        SetValidacionesGenerales();
                    }
                    else
                    {
                        PaseEnabled = false;
                        SelectTipoPase = 1;
                    }
                }
                OnPropertyChanged("SelectAccesoUnico");
            }
        }
        public short? SelectPais
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectPaisAsignacion : SelectPaisPadron; }
            set
            {
                //if (value == 223)
                //value = 82;
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectPaisAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectPaisPadron = value;
                if (value > 0)
                {
                    //ListEntidad = ListEntidad ?? new ObservableCollection<ENTIDAD>();
                    ListEntidad = new ObservableCollection<ENTIDAD>(ListEntidadesAuxiliares.Where(w => w.ID_PAIS_NAC == value).OrderBy(o => o.DESCR)); //(new cEntidad()).ObtenerTodos();
                }
                else
                    ListEntidad = new ObservableCollection<ENTIDAD>();
                if (value == Parametro.PAIS)//Mexico
                {
                    //SelectEntidad = 2;//Baja California
                    if (ValidarEnabled)
                    {
                        EntidadEnabled = true;
                        MunicipioEnabled = true;
                        ColoniaEnabled = true;
                    }
                }
                else if (value == -1)
                {
                    //SelectEntidad = -1;
                    if (ValidarEnabled)
                    {
                        EntidadEnabled = true;
                        MunicipioEnabled = true;
                        ColoniaEnabled = true;
                    }
                }
                else
                {
                    //SelectEntidad = 33;
                    EntidadEnabled = false;
                    MunicipioEnabled = false;
                    ColoniaEnabled = false;
                }

                OnPropertyChanged("SelectPais");
            }
        }
        public short? SelectEntidad
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectEntidadAsignacion : SelectEntidadPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectEntidadAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectEntidadPadron = value;

                if (value != null)
                {
                    if (value > 0)
                    {
                        //ListMunicipio = ListMunicipio ?? new ObservableCollection<MUNICIPIO>();
                        ListMunicipio = new ObservableCollection<MUNICIPIO>(ListMunicipiosAuxiliares.Where(w => w.ID_ENTIDAD == value).OrderBy(o => o.MUNICIPIO1));//(new cMunicipio()).ObtenerTodos(string.Empty, value));
                    }
                    else
                        ListMunicipio = new ObservableCollection<MUNICIPIO>();

                    if (ListMunicipio != null)
                    {
                        ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                        if (value == 33)
                            SelectMunicipio = 1001;
                        else
                            SelectMunicipio = -1;
                        OnPropertyChanged("SelectEntidad");
                    }
                }
            }
        }
        public short? SelectMunicipio
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectMunicipioAsignacion : SelectMunicipioPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectMunicipioAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectMunicipioPadron = value;

                if (value != null)
                {
                    if (value > 0)
                    {
                        //ListColonia = ListColonia ?? new ObservableCollection<COLONIA>();
                        ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w => w.ID_ENTIDAD == SelectEntidad && w.ID_MUNICIPIO == value).OrderBy(o => o.DESCR));//(new cColonia()).ObtenerTodos(string.Empty, value, SelectEntidad));
                    }
                    else
                        ListColonia = new ObservableCollection<COLONIA>();
                    ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                    if (value == 1001)
                        SelectColonia = 102;
                    else
                    {
                        if (ListColonia.Count == 1)
                        {
                            ListColonia.Insert(0, ListColoniasAuxiliares.First(w => w.ID_COLONIA == 102)); //new cColonia().Obtener(102).FirstOrDefault());
                            SelectColonia = 102;
                            ColoniaEnabled = false;
                        }
                        else
                        {
                            SelectColonia = -1;
                            ColoniaEnabled = true;
                        }
                    }

                    OnPropertyChanged("SelectMunicipio");

                }
            }
        }
        public int? SelectColonia
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectColoniaAsignacion : SelectColoniaPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectColoniaAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectColoniaPadron = value;

                OnPropertyChanged("SelectColonia");
            }
        }
        public string SelectDiscapacitado
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectDiscapacitadoAsignacion : SelectDiscapacitadoPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectDiscapacitadoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    SelectDiscapacitadoPadron = value;

                    if (SelectImputadoIngreso != null)
                        AcompananteVisible = value == "S" ? Visibility.Visible : Visibility.Collapsed;
                    DiscapacidadEnabled = value == "S";
                    AcompananteVisible = FechaNacimiento.HasValue ? SelectTipoVisitante == 23 ? Visibility.Visible : SelectSexo == "M" ? (((DateTime.Now - FechaNacimiento.Value).TotalDays / 365) <= 12) ? Visibility.Visible : Visibility.Collapsed : SelectSexo == "F" ? (((DateTime.Now - FechaNacimiento.Value).TotalDays / 365) <= 14) ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed : TextEdad.HasValue ? SelectTipoVisitante == 23 ? Visibility.Visible : SelectSexo == "M" ? TextEdad.Value <= 12 ? Visibility.Visible : Visibility.Collapsed : SelectSexo == "F" ? TextEdad.Value <= 14 ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed : Visibility.Collapsed;
                    SelectDiscapacidad = (SelectVisitante != null && value == "S") ? SelectVisitante.OBJETO_PERSONA != null ? SelectVisitante.OBJETO_PERSONA.ID_TIPO_DISCAPACIDAD : 0 : 0;
                }
                OnPropertyChanged("SelectDiscapacidad");
                OnPropertyChanged("SelectDiscapacitado");
            }
        }
        public short? SelectDiscapacidad
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectDiscapacidadAsignacion : SelectDiscapacidadPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectDiscapacidadAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectDiscapacidadPadron = value;

                OnPropertyChanged("SelectDiscapacidad");
            }
        }
        public short? SelectParentesco
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return SelectParentescoAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return SelectParentescoPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return SelectParentescoCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectParentescoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectParentescoPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    SelectParentescoCredencial = value;

                OnPropertyChanged("SelectParentesco");
            }
        }

        #region PADRON
        private SSP.Servidor.PERSONA SelectPersonaVisitantePadron { get; set; }
        private PERSONAVISITAAUXILIAR SelectVisitanteInternoPadron { get; set; }
        private PERSONAVISITAAUXILIAR SelectVisitantePadron { get; set; }
        private PERSONAVISITAAUXILIAR SelectVisitanteCredencial { get; set; }
        private short? SelectTipoVisitantePadron = 0;
        private short? SelectTipoVisitaPadron { get; set; }
        private short? SelectAreaVisitaPadron { get; set; }
        private string SelectSexoPadron { get; set; }
        private DateTime? FechaNacimientoPadron { get; set; }
        private short? SelectSituacionPadron { get; set; }
        private bool SelectAccesoUnicoPadron { get; set; }
        private short? SelectPaisPadron { get; set; }
        private short? SelectEntidadPadron { get; set; }
        private short? SelectMunicipioPadron { get; set; }
        private int? SelectColoniaPadron { get; set; }
        private string SelectDiscapacitadoPadron = "";
        private short? SelectDiscapacidadPadron { get; set; }
        private short? SelectParentescoPadron { get; set; }
        #endregion

        #region ASIGNACION
        private SSP.Servidor.PERSONA SelectPersonaVisitanteAsignacion { get; set; }
        private PERSONAVISITAAUXILIAR SelectVisitanteInternoAsignacion { get; set; }
        private PERSONAVISITAAUXILIAR SelectVisitanteAsignacion { get; set; }
        private short? SelectTipoVisitanteAsignacion { get; set; }
        private short? SelectTipoVisitaAsignacion { get; set; }
        private short? SelectAreaVisitaAsignacion { get; set; }
        private string SelectSexoAsignacion { get; set; }
        private DateTime? FechaNacimientoAsignacion { get; set; }
        private short? SelectSituacionAsignacion { get; set; }
        private bool SelectAccesoUnicoAsignacion { get; set; }
        private short? SelectPaisAsignacion { get; set; }
        private short? SelectEntidadAsignacion { get; set; }
        private short? SelectMunicipioAsignacion { get; set; }
        private int? SelectColoniaAsignacion { get; set; }
        private string SelectDiscapacitadoAsignacion { get; set; }
        private short? SelectDiscapacidadAsignacion { get; set; }
        private short? SelectParentescoAsignacion { get; set; }
        #endregion

        #region [CREDENCIAL]
        private string SelectSexoCredencial { get; set; }
        private DateTime? FechaNacimientoCredencial { get; set; }
        private short? SelectTipoVisitanteCredencial = 0;
        private short? SelectSituacionCredencial { get; set; }
        private short? SelectParentescoCredencial { get; set; }
        #endregion
        #endregion

        #region Text
        private string textFechaAlta = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy");
        public string TextFechaAlta
        {
            get { return textFechaAlta; }
            set { textFechaAlta = value; OnPropertyChanged("TextFechaAlta"); }
        }
        DateTime? _DatePickCapturaDocumento = Fechas.GetFechaDateServer;
        public DateTime? DatePickCapturaDocumento
        {
            get { return _DatePickCapturaDocumento; }
            set
            {
                _DatePickCapturaDocumento = value;
                OnPropertyChanged("DatePickCapturaDocumento");
            }
        }
        string _ObservacionDocumento;
        public string ObservacionDocumento
        {
            get { return _ObservacionDocumento; }
            set
            {
                _ObservacionDocumento = value;
                OnPropertyChanged("ObservacionDocumento");
            }
        }
        /*
        private DateTime _HoraEntrada;
        public DateTime HoraEntrada
        {
            get
            {
            //    if (ReiniciaHoraEntrada)
            //    {
            //        DateRangeSlider.LowValue = new DateTime(1, 1, 1, 7, 0, 0);
            //        ReiniciaHoraEntrada = false;
            //    }
            //    _HoraEntrada = DateRangeSlider.LowValue.HasValue ? DateRangeSlider.LowValue.Value.ToString("HHmm") : Convert.ToDateTime("1/1/0001 7:00:00 AM").ToString("HHmm");
                return _HoraEntrada;
            }
            set
            {
                _HoraEntrada = value;
                OnPropertyChanged("HoraEntrada");
            }
        }
        private DateTime _HoraSalida;
        public DateTime HoraSalida
        {
            get
            {
                //if (ReiniciaHoraEntrada)
                //{
                //    DateRangeSlider.HighValue = new DateTime(1, 1, 1, 19, 0, 0);
                //    ReiniciaHoraSalida = false;
                //}
                //return DateRangeSlider.HighValue.HasValue ? DateRangeSlider.HighValue.Value.ToString("HHmm") : Convert.ToDateTime("1/1/0001 7:00:00 PM").ToString("HHmm");
                return _HoraSalida;
            }
            set
            {
                _HoraSalida = value;
                OnPropertyChanged("HoraSalida");
            }
        }
        private DateTime _Minimo;
        public DateTime Minimo
        {
            get { return _Minimo; }
            set { _Minimo = value; OnPropertyChanged("Minimo"); }
        }
        private DateTime _Maximo;
        public DateTime Maximo
        {
            get { return _Maximo; }
            set { _Maximo = value; OnPropertyChanged("Maximo"); }
        }*/
        private bool ReiniciaHoraEntrada = false;
        private bool ReiniciaHoraSalida = false;
        public long? TextCodigo
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return TextCodigoAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return TextCodigoPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return TextCodigoCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextCodigoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TextCodigoPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    TextCodigoCredencial = value;

                OnPropertyChanged("TextCodigo");
            }
        }
        public string TextNombre
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return TextNombreAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return TextNombrePadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return TextNombreCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextNombreAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    TextNombrePadron = value;
                    if (!string.IsNullOrEmpty(TextMaterno) && !string.IsNullOrEmpty(TextPaterno) && !string.IsNullOrEmpty(TextNombre)
                        && FechaNacimiento != null && (!BanderaEditar ||
                        SelectVisitante == null ? true : SelectVisitante.OBJETO_PERSONA == null ?
                        true : string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.RFC)))
                        TextRfc = CURPRFC.CalcularRFC(TextNombre, TextPaterno, TextMaterno, FechaNacimiento.Value.ToString("yyMMdd"));

                    if (!string.IsNullOrEmpty(TextMaterno) && !string.IsNullOrEmpty(TextPaterno) && !string.IsNullOrEmpty(TextNombre)
                        && FechaNacimiento != null && (!BanderaEditar ||
                        SelectVisitante == null ? true : SelectVisitante.OBJETO_PERSONA == null ?
                        true : string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.CURP)))
                        TextCurp = CURPRFC.CalcularCURP(TextNombre, TextPaterno, TextMaterno, FechaNacimiento.Value.ToString("yyMMdd"));
                }
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    TextNombreCredencial = value;

                OnPropertyChanged("TextNombre");
            }
        }
        public string TextPaterno
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return TextPaternoAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return TextPaternoPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return TextPaternoCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextPaternoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    TextPaternoPadron = value;
                    if (!string.IsNullOrEmpty(TextMaterno) && !string.IsNullOrEmpty(TextPaterno) && !string.IsNullOrEmpty(TextNombre)
                        && FechaNacimiento != null && (!BanderaEditar ||
                        SelectVisitante == null ? true : SelectVisitante.OBJETO_PERSONA == null ?
                        true : string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.RFC)))
                        TextRfc = CURPRFC.CalcularRFC(TextNombre, TextPaterno, TextMaterno, FechaNacimiento.Value.ToString("yyMMdd"));

                    if (!string.IsNullOrEmpty(TextMaterno) && !string.IsNullOrEmpty(TextPaterno) && !string.IsNullOrEmpty(TextNombre)
                        && FechaNacimiento != null && (!BanderaEditar ||
                        SelectVisitante == null ? true : SelectVisitante.OBJETO_PERSONA == null ?
                        true : string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.CURP)))
                        TextCurp = CURPRFC.CalcularCURP(TextNombre, TextPaterno, TextMaterno, FechaNacimiento.Value.ToString("yyMMdd"));
                }
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    TextPaternoCredencial = value;

                #region Validaciones
                if (base.FindRule("TextNombre"))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        base.RemoveRule("TextMaterno");
                        OnPropertyChanged("TextMaterno");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(TextMaterno))
                        {
                            base.RemoveRule("TextPaterno");
                            base.AddRule(() => TextPaterno, () => !string.IsNullOrEmpty(TextPaterno), "APELLIDO PATERNO ES REQUERIDO!");

                            base.RemoveRule("TextMaterno");
                            base.AddRule(() => TextMaterno, () => !string.IsNullOrEmpty(TextMaterno), "APELLIDO MATERNO ES REQUERIDO!");
                            OnPropertyChanged("TextMaterno");
                        }
                    }
                }
                #endregion

                OnPropertyChanged("TextPaterno");
            }
        }
        public string TextMaterno
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return TextMaternoAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return TextMaternoPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return TextMaternoCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextMaternoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    TextMaternoPadron = value;
                    if (!string.IsNullOrEmpty(TextMaterno) && !string.IsNullOrEmpty(TextPaterno) && !string.IsNullOrEmpty(TextNombre)
                        && FechaNacimiento != null && (!BanderaEditar ||
                        SelectVisitante == null ? true : SelectVisitante.OBJETO_PERSONA == null ?
                        true : string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.RFC)))
                        TextRfc = CURPRFC.CalcularRFC(TextNombre, TextPaterno, TextMaterno, FechaNacimiento.Value.ToString("yyMMdd"));

                    if (!string.IsNullOrEmpty(TextMaterno) && !string.IsNullOrEmpty(TextPaterno) && !string.IsNullOrEmpty(TextNombre)
                        && FechaNacimiento != null && (!BanderaEditar ||
                        SelectVisitante == null ? SelectVisitante == null : SelectVisitante.OBJETO_PERSONA == null ?
                        SelectVisitante.OBJETO_PERSONA == null : string.IsNullOrEmpty(SelectVisitante.OBJETO_PERSONA.CURP)))
                        TextCurp = CURPRFC.CalcularCURP(TextNombre, TextPaterno, TextMaterno, FechaNacimiento.Value.ToString("yyMMdd"));
                }
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    TextMaternoCredencial = value;

                #region Validaciones
                if (base.FindRule("TextNombre"))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        base.RemoveRule("TextPaterno");
                        OnPropertyChanged("TextPaterno");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(TextPaterno))
                        {
                            base.RemoveRule("TextMaterno");
                            base.AddRule(() => TextMaterno, () => !string.IsNullOrEmpty(TextMaterno), "APELLIDO MATERNO ES REQUERIDO!");

                            base.RemoveRule("TextPaterno");
                            base.AddRule(() => TextPaterno, () => !string.IsNullOrEmpty(TextPaterno), "APELLIDO PATERNO ES REQUERIDO!");
                            OnPropertyChanged("TextPaterno");
                        }
                    }
                }
                #endregion

                OnPropertyChanged("TextMaterno");
            }
        }
        public string TextCurp
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? TextCurpAsignacion : TextCurpPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextCurpAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    TextCurpPadron = value;
                    #region Validaciones
                    if (base.FindRule("SelectDiscapacidad"))
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            //base.RemoveRule("TextCurp");
                            //base.AddRule(() => TextCurp, () => TextCurp.Length == 18, "EL CURP DEBE CONTENER 18 CARACTERES!");
                        }
                        else
                        {
                            //base.RemoveRule("TextCurp");
                        }
                    }
                    #endregion
                }
                OnPropertyChanged("TextCurp");
            }
        }
        public string TextRfc
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? TextRfcAsignacion : TextRfcPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextRfcAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    TextRfcPadron = value;
                    #region Validaciones
                    if (base.FindRule("SelectDiscapacidad"))
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            base.RemoveRule("TextRfc");
                            base.AddRule(() => TextRfc, () => TextRfc.Length == 13, "EL RFC DEBE CONTENER 13 CARACTERES!");
                        }
                        else
                        {
                            base.RemoveRule("TextRfc");
                        }
                    }
                    #endregion
                }

                OnPropertyChanged("TextRfc");
            }
        }
        public string TextTelefono
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? TextTelefonoAsignacion : TextTelefonoPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextTelefonoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TextTelefonoPadron = value;

                OnPropertyChanged("TextTelefono");
            }
        }
        public short? TextEdad
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return TextEdadAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return TextEdadPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return TextEdadCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextEdadAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TextEdadPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    TextEdadCredencial = value;

                OnPropertyChanged("TextEdad");
            }
        }
        public string TextCorreo
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? TextCorreoAsignacion : TextCorreoPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextCorreoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TextCorreoPadron = value;

                OnPropertyChanged("TextCorreo");
            }
        }
        //public string TextNip
        //{
        //    get
        //    {
        //        switch (SelectedTab)
        //        {
        //            case TabsVisita.ASIGNACION_DE_VISITAS:
        //                return TextNipAsignacion;
        //            case TabsVisita.PADRON_DE_VISITAS:
        //                return TextNipPadron;
        //            case TabsVisita.ENTREGA_CREDENCIALES:
        //                return TextNipCredencial;
        //            default:
        //                return null;
        //        }
        //    }
        //    set
        //    {
        //        if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
        //            TextNipAsignacion = value;
        //        if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
        //            TextNipPadron = value;
        //        if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
        //            TextNipCredencial = value;

        //        OnPropertyChanged("TextNip");
        //    }
        //}
        public string TextFechaUltimaModificacion
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? TextFechaUltimaModificacionAsignacion : TextFechaUltimaModificacionPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextFechaUltimaModificacionAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TextFechaUltimaModificacionPadron = value;

                OnPropertyChanged("TextFechaUltimaModificacion");
            }
        }
        public string TextCalle
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? TextCalleAsignacion : TextCallePadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextCalleAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TextCallePadron = value;

                OnPropertyChanged("TextCalle");
            }
        }
        public int? TextNumExt
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? TextNumExtAsignacion : TextNumExtPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextNumExtAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TextNumExtPadron = value;

                OnPropertyChanged("TextNumExt");
            }
        }
        public string TextNumInt
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? TextNumIntAsignacion : TextNumIntPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextNumIntAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TextNumIntPadron = value;

                OnPropertyChanged("TextNumInt");
            }
        }
        public int? TextCodigoPostal
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? TextCodigoPostalAsignacion : TextCodigoPostalPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextCodigoPostalAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TextCodigoPostalPadron = value;

                OnPropertyChanged("TextCodigoPostal");
            }
        }
        public string TextObservacion
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return TextObservacionAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return TextObservacionPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return TextObservacionCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TextObservacionAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TextObservacionPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    TextObservacionCredencial = value;

                OnPropertyChanged("TextObservacion");
            }
        }
        public string textAmpliarDescripcion;
        public string TextAmpliarDescripcion
        {
            get { return textAmpliarDescripcion; }
            set
            {
                textAmpliarDescripcion = value;
                OnPropertyChanged("TextAmpliarDescripcion");
            }
        }
        public int MaxLengthAmpliarDescripcion = 200;
        public string TextHeaderDatosInterno = "Datos del Interno Seleccionado";
        public string TituloHeaderExpandirDescripcion = "AMPLIAR EL CAMPO DE OBSERVACIONES";

        #region DatosImputado
        private string _TextVisitaDeTermino;
        public string TextVisitaDeTermino
        {
            get { return _TextVisitaDeTermino; }
            set { _TextVisitaDeTermino = value; OnPropertyChanged("TextVisitaDeTermino"); }
        }
        public string AnioD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return AnioDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return AnioDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return AnioDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    AnioDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    AnioDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    AnioDCredencial = value;

                OnPropertyChanged("AnioD");
            }
        }
        public string FolioD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return FolioDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return FolioDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return FolioDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    FolioDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    FolioDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    FolioDCredencial = value;

                OnPropertyChanged("FolioD");
            }
        }
        public int? AnioBuscar
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? AnioBuscarAsignacion : AnioBuscarPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    AnioBuscarAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    AnioBuscarPadron = value;

                OnPropertyChanged("AnioBuscar");
            }
        }
        public int? FolioBuscar
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? FolioBuscarAsignacion : FolioBuscarPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    FolioBuscarAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    FolioBuscarPadron = value;

                OnPropertyChanged("FolioBuscar");
            }
        }
        public string PaternoD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return PaternoDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return PaternoDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return PaternoDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    PaternoDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    PaternoDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    PaternoDCredencial = value;

                OnPropertyChanged("PaternoD");
            }
        }
        public string MaternoD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return MaternoDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return MaternoDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return MaternoDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    MaternoDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    MaternoDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    MaternoDCredencial = value;

                OnPropertyChanged("MaternoD");
            }
        }
        public string NombreD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return NombreDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return NombreDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return NombreDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    NombreDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    NombreDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    NombreDCredencial = value;

                OnPropertyChanged("NombreD");
            }
        }
        public string IngresosD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return IngresosDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return IngresosDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return IngresosDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    IngresosDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    IngresosDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    IngresosDCredencial = value;

                OnPropertyChanged("IngresosD");
            }
        }
        public string NoControlD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return NoControlDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return NoControlDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return NoControlDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    NoControlDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    NoControlDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    NoControlDCredencial = value;

                OnPropertyChanged("NoControlD");
            }
        }
        public string UbicacionD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return UbicacionDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return UbicacionDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return UbicacionDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    UbicacionDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    UbicacionDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    UbicacionDCredencial = value;

                OnPropertyChanged("UbicacionD");
            }
        }
        public string TipoSeguridadD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return TipoSeguridadDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return TipoSeguridadDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return TipoSeguridadDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    TipoSeguridadDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    TipoSeguridadDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    TipoSeguridadDCredencial = value;

                OnPropertyChanged("TipoSeguridadD");
            }
        }
        public string FecIngresoD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return FecIngresoDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return FecIngresoDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return FecIngresoDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    FecIngresoDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    FecIngresoDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    FecIngresoDCredencial = value;

                OnPropertyChanged("FecIngresoD");
            }
        }
        public string ClasificacionJuridicaD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return ClasificacionJuridicaDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return ClasificacionJuridicaDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return ClasificacionJuridicaDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    ClasificacionJuridicaDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    ClasificacionJuridicaDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    ClasificacionJuridicaDCredencial = value;

                OnPropertyChanged("ClasificacionJuridicaD");
            }
        }
        public string EstatusD
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return EstatusDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return EstatusDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return EstatusDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    EstatusDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    EstatusDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    EstatusDCredencial = value;

                OnPropertyChanged("EstatusD");
            }
        }
        #endregion

        #region PADRON
        private long? TextCodigoPadron { get; set; }
        private string TextNombrePadron { get; set; }
        private string TextPaternoPadron { get; set; }
        private string TextMaternoPadron { get; set; }
        private string TextCurpPadron { get; set; }
        private string TextRfcPadron { get; set; }
        private string textTelefonoPadron;
        public string TextTelefonoPadron
        {
            get
            {
                if (textTelefonoPadron == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(textTelefonoPadron);
            }
            set { textTelefonoPadron = value; }
        }
        private short? TextEdadPadron { get; set; }
        private string TextCorreoPadron { get; set; }
        private string TextNipPadron { get; set; }
        private string TextFechaUltimaModificacionPadron { get; set; }
        private string TextCallePadron { get; set; }
        private int? TextNumExtPadron { get; set; }
        private string TextNumIntPadron { get; set; }
        private int? TextCodigoPostalPadron { get; set; }
        private string TextObservacionPadron { get; set; }

        #region DatosImputado
        private string AnioDPadron { get; set; }
        private string FolioDPadron { get; set; }
        private int? AnioBuscarPadron { get; set; }
        private int? FolioBuscarPadron { get; set; }
        private string PaternoDPadron { get; set; }
        private string MaternoDPadron { get; set; }
        private string NombreDPadron { get; set; }
        private string IngresosDPadron { get; set; }
        private string NoControlDPadron { get; set; }
        private string UbicacionDPadron { get; set; }
        private string TipoSeguridadDPadron { get; set; }
        private string FecIngresoDPadron { get; set; }
        private string ClasificacionJuridicaDPadron { get; set; }
        private string EstatusDPadron { get; set; }
        #endregion

        #endregion

        #region ASIGNACION
        private long? TextCodigoAsignacion { get; set; }
        private string TextNombreAsignacion { get; set; }
        private string TextPaternoAsignacion { get; set; }
        private string TextMaternoAsignacion { get; set; }
        private string TextCurpAsignacion { get; set; }
        private string TextRfcAsignacion { get; set; }
        private string textTelefonoAsignacion;
        public string TextTelefonoAsignacion
        {
            get
            {
                if (textTelefonoAsignacion == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(textTelefonoAsignacion);
            }
            set { textTelefonoAsignacion = value; }
        }
        private short? TextEdadAsignacion { get; set; }
        private string TextCorreoAsignacion { get; set; }
        private string TextNipAsignacion { get; set; }
        private string TextFechaUltimaModificacionAsignacion { get; set; }
        private string TextCalleAsignacion { get; set; }
        private int? TextNumExtAsignacion { get; set; }
        private string TextNumIntAsignacion { get; set; }
        private int? TextCodigoPostalAsignacion { get; set; }
        private string TextObservacionAsignacion { get; set; }

        #region DatosImputado
        private string AnioDAsignacion { get; set; }
        private string FolioDAsignacion { get; set; }
        private int? AnioBuscarAsignacion { get; set; }
        private int? FolioBuscarAsignacion { get; set; }
        private string PaternoDAsignacion { get; set; }
        private string MaternoDAsignacion { get; set; }
        private string NombreDAsignacion { get; set; }
        private string IngresosDAsignacion { get; set; }
        private string NoControlDAsignacion { get; set; }
        private string UbicacionDAsignacion { get; set; }
        private string TipoSeguridadDAsignacion { get; set; }
        private string FecIngresoDAsignacion { get; set; }
        private string ClasificacionJuridicaDAsignacion { get; set; }
        private string EstatusDAsignacion { get; set; }
        #endregion

        #endregion

        #region [CREDENCIAL]
        private long? TextCodigoCredencial { get; set; }
        private string TextNombreCredencial { get; set; }
        private string TextPaternoCredencial { get; set; }
        private string TextMaternoCredencial { get; set; }
        private short? TextEdadCredencial { get; set; }
        private string TextNipCredencial { get; set; }
        private string TextObservacionCredencial { get; set; }

        private string AnioDCredencial { get; set; }
        private string FolioDCredencial { get; set; }
        private string PaternoDCredencial { get; set; }
        private string MaternoDCredencial { get; set; }
        private string NombreDCredencial { get; set; }
        private string IngresosDCredencial { get; set; }
        private string NoControlDCredencial { get; set; }
        private string UbicacionDCredencial { get; set; }
        private string TipoSeguridadDCredencial { get; set; }
        private string FecIngresoDCredencial { get; set; }
        private string ClasificacionJuridicaDCredencial { get; set; }
        private string EstatusDCredencial { get; set; }
        #endregion

        #endregion

        #region Visible
        private bool emptyBuscarListaTonta;
        public bool EmptyBuscarListaTonta
        {
            get { return emptyBuscarListaTonta; }
            set { emptyBuscarListaTonta = value; OnPropertyChanged("EmptyBuscarListaTonta"); }
        }
        private Visibility lineasGuiaFoto = Visibility.Visible;
        public Visibility LineasGuiaFoto
        {
            get { return lineasGuiaFoto; }
            set { lineasGuiaFoto = value; OnPropertyChanged("LineasGuiaFoto"); }
        }
        private Visibility datosExpedienteVisible = Visibility.Collapsed;
        public Visibility DatosExpedienteVisible
        {
            get { return datosExpedienteVisible; }
            set { datosExpedienteVisible = value; OnPropertyChanged("DatosExpedienteVisible"); }
        }
        private Visibility AcompananteVisibleAsignacion = Visibility.Collapsed;
        private Visibility AcompananteVisiblePadron = Visibility.Collapsed;
        public Visibility AcompananteVisible
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? AcompananteVisibleAsignacion : AcompananteVisiblePadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    AcompananteVisibleAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    AcompananteVisiblePadron = value;
                OnPropertyChanged("AcompananteVisible");
            }
        }
        private bool buscarAcompananteVisible = false;
        public bool BuscarAcompananteVisible
        {
            get { return buscarAcompananteVisible; }
            set { buscarAcompananteVisible = value; OnPropertyChanged("BuscarAcompananteVisible"); }
        }
        private bool listaInternoVisitaVisible = false;
        public bool ListaInternoVisitaVisible
        {
            get { return listaInternoVisitaVisible; }
            set { listaInternoVisitaVisible = value; OnPropertyChanged("ListaInternoVisitaVisible"); }
        }
        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }
        private bool emptyBuscarRelacionInternoVisible;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return emptyBuscarRelacionInternoVisible; }
            set { emptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }
        private bool _ProgramacionVisitaVisible = false;
        public bool ProgramacionVisitaVisible
        {
            get { return _ProgramacionVisitaVisible; }
            set { _ProgramacionVisitaVisible = value; OnPropertyChanged("ProgramacionVisitaVisible"); }
        }
        private bool _CapturarVisitanteVisible = false;
        public bool CapturarVisitanteVisible
        {
            get { return _CapturarVisitanteVisible; }
            set { _CapturarVisitanteVisible = value; OnPropertyChanged("CapturarVisitanteVisible"); }
        }
        private Visibility tipoDocumentoVisible = Visibility.Visible;
        public Visibility TipoDocumentoVisible
        {
            get { return tipoDocumentoVisible; }
            set { tipoDocumentoVisible = value; OnPropertyChanged("TipoDocumentoVisible"); }
        }
        private Visibility identificacionOficialVisible = Visibility.Collapsed;
        public Visibility IdentificacionOficialVisible
        {
            get { return identificacionOficialVisible; }
            set { identificacionOficialVisible = value; OnPropertyChanged("IdentificacionOficialVisible"); }
        }
        private Visibility comboFrontBackFotoVisible = Visibility.Collapsed;
        public Visibility ComboFrontBackFotoVisible
        {
            get { return comboFrontBackFotoVisible; }
            set { comboFrontBackFotoVisible = value; OnPropertyChanged("ComboFrontBackFotoVisible"); }
        }

        #region PADRON
        #endregion

        #region ASIGNACION
        #endregion

        #endregion

        #region Enable
        private bool crearNuevoVisitanteEnabled = false;
        public bool CrearNuevoVisitanteEnabled
        {
            get { return crearNuevoVisitanteEnabled; }
            set { crearNuevoVisitanteEnabled = value; OnPropertyChanged("CrearNuevoVisitanteEnabled"); }
        }
        private bool SituacionEnabledAsignacion = false;
        private bool SituacionEnabledPadron = false;
        public bool SituacionEnabled
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SituacionEnabledAsignacion : SituacionEnabledPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SituacionEnabledAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SituacionEnabledPadron = value;
                OnPropertyChanged("SituacionEnabled");
            }
        }
        private bool seleccionarVisitaExistente;
        public bool SeleccionarVisitaExistente
        {
            get { return seleccionarVisitaExistente; }
            set { seleccionarVisitaExistente = value; OnPropertyChanged("SeleccionarVisitaExistente"); }
        }
        private bool contextMenuBorrarVisitanteEnabled = false;
        public bool ContextMenuBorrarVisitanteEnabled
        {
            get { return contextMenuBorrarVisitanteEnabled; }
            set { contextMenuBorrarVisitanteEnabled = value; OnPropertyChanged("ContextMenuBorrarVisitanteEnabled"); }
        }
        private bool contextMenuNuevoVisitanteEnabled = false;
        public bool ContextMenuNuevoVisitanteEnabled
        {
            get { return contextMenuNuevoVisitanteEnabled; }
            set { contextMenuNuevoVisitanteEnabled = value; OnPropertyChanged("ContextMenuNuevoVisitanteEnabled"); }
        }
        private bool contextMenuEnabled = false;
        public bool ContextMenuEnabled
        {
            get { return contextMenuEnabled; }
            set { contextMenuEnabled = value; OnPropertyChanged("ContextMenuEnabled"); }
        }

        public bool _MenuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return _MenuGuardarEnabled; }
            set
            {
                _MenuGuardarEnabled = value;
                OnPropertyChanged("MenuGuardarEnabled");
            }
        }

        public bool ValidarEnabled
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? ValidarEnabledAsignacion : ValidarEnabledPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                {
                    ValidarEnabledAsignacion = value;
                    EntidadEnabledAsignacion = value;
                    MunicipioEnabledAsignacion = value;
                    ColoniaEnabledAsignacion = value;
                }
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                {
                    ValidarEnabledPadron = value;
                    EntidadEnabledPadron = value;
                    MunicipioEnabledPadron = value;
                    ColoniaEnabledPadron = value;
                }
                OnPropertyChanged("ValidarEnabled");
            }
        }
        public bool GeneralEnabled
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? GeneralEnabledAsignacion : GeneralEnabledPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    GeneralEnabledAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    GeneralEnabledPadron = value;
                OnPropertyChanged("GeneralEnabled");
            }
        }
        private bool codigoEnabledEntrega = false;
        public bool CodigoEnabledEntrega
        {
            get { return codigoEnabledEntrega; }
            set { codigoEnabledEntrega = value; }
        }

        public bool CodigoEnabled
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return CodigoEnabledAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return CodigoEnabledPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return CodigoEnabledEntrega;
                    default:
                        return true;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    CodigoEnabledAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    CodigoEnabledPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    CodigoEnabledEntrega = value;
                OnPropertyChanged("CodigoEnabled");
            }
        }
        private bool discapacitadoEnabled = true;
        public bool DiscapacitadoEnabled
        {
            get { return discapacitadoEnabled; }
            set { discapacitadoEnabled = value; OnPropertyChanged("DiscapacitadoEnabled"); }
        }
        private bool discapacidadEnabled = false;
        public bool DiscapacidadEnabled
        {
            get { return discapacidadEnabled; }
            set { discapacidadEnabled = value; OnPropertyChanged("DiscapacidadEnabled"); }
        }
        private bool _IsDetalleInternosEnable = false;
        public bool IsDetalleInternosEnable
        {
            get { return _IsDetalleInternosEnable; }
            set
            {
                _IsDetalleInternosEnable = value;
                OnPropertyChanged("IsDetalleInternosEnable");
            }
        }
        public bool EntidadEnabled
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? EntidadEnabledAsignacion : EntidadEnabledPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    EntidadEnabledAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    EntidadEnabledPadron = value;
                OnPropertyChanged("EntidadEnabled");
            }
        }
        public bool MunicipioEnabled
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? MunicipioEnabledAsignacion : MunicipioEnabledPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    MunicipioEnabledAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    MunicipioEnabledPadron = value;
                OnPropertyChanged("MunicipioEnabled");
            }
        }
        public bool ColoniaEnabled
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? ColoniaEnabledAsignacion : ColoniaEnabledPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    ColoniaEnabledAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    ColoniaEnabledPadron = value;
                OnPropertyChanged("ColoniaEnabled");
            }
        }
        private bool fotoHuellaEnabled = false;
        public bool FotoHuellaEnabled
        {
            get { return fotoHuellaEnabled; }
            set { fotoHuellaEnabled = value; OnPropertyChanged("FotoHuellaEnabled"); }
        }
        private bool capturarVisitanteEnabled = false;
        public bool CapturarVisitanteEnabled
        {
            get { return capturarVisitanteEnabled; }
            set { capturarVisitanteEnabled = value; OnPropertyChanged("CapturarVisitanteEnabled"); }
        }
        private bool accesoUnicoEnabled = false;
        public bool AccesoUnicoEnabled
        {
            get { return accesoUnicoEnabled; }
            set { accesoUnicoEnabled = value; OnPropertyChanged("AccesoUnicoEnabled"); }
        }
        private bool digitalizarDocumentosEnabled = false;
        public bool DigitalizarDocumentosEnabled
        {
            get { return digitalizarDocumentosEnabled; }
            set { digitalizarDocumentosEnabled = value; OnPropertyChanged("DigitalizarDocumentosEnabled"); }
        }
        private bool paseEnabled = false;
        public bool PaseEnabled
        {
            get { return paseEnabled; }
            set
            {
                paseEnabled = value;
                OnPropertyChanged("PaseEnabled");
            }
        }
        private bool programacionVisitasMenuEnabled = false;
        public bool ProgramacionVisitasMenuEnabled
        {
            get { return programacionVisitasMenuEnabled; }
            set { programacionVisitasMenuEnabled = value; OnPropertyChanged("ProgramacionVisitasMenuEnabled"); }
        }
        public bool SelectParentescoIngresoEnabled
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return SelectParentescoIngresoEnabledAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return SelectParentescoIngresoEnabledPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return false;
                    default:
                        return false;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectParentescoIngresoEnabledAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    SelectParentescoIngresoEnabledPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES) { }// SelectParentescoIngresoEnabledCredencial = value;

                OnPropertyChanged("SelectParentescoIngresoEnabled");
            }
        }
        private bool selectEstatusRelacionEnabled = false;
        public bool SelectEstatusRelacionEnabled
        {
            get { return selectEstatusRelacionEnabled; }
            set { selectEstatusRelacionEnabled = value; OnPropertyChanged("SelectEstatusRelacionEnabled"); }
        }

        #region PARDON
        private bool SelectParentescoIngresoEnabledPadron = false;
        private bool seleccionarVisitaExistentePadron;
        public bool SeleccionarVisitaExistentePadron
        {
            get { return seleccionarVisitaExistentePadron; }
            set { seleccionarVisitaExistentePadron = value; OnPropertyChanged("SeleccionarVisitaExistentePadron"); }
        }
        private bool contextMenuEnabledPadron;
        public bool ContextMenuEnabledPadron
        {
            get { return contextMenuEnabledPadron; }
            set { contextMenuEnabledPadron = value; OnPropertyChanged("ContextMenuEnabledPadron"); }
        }
        public bool ValidarEnabledPadron { get; set; }
        public bool GeneralEnabledPadron { get; set; }
        private bool codigoEnabledPadron;
        public bool CodigoEnabledPadron
        {
            get { return codigoEnabledPadron; }
            set { codigoEnabledPadron = value; OnPropertyChanged("CodigoEnabledPadron"); }
        }
        private bool discapacitadoEnabledPadron;
        public bool DiscapacitadoEnabledPadron
        {
            get { return discapacitadoEnabledPadron; }
            set { discapacitadoEnabledPadron = value; OnPropertyChanged("DiscapacitadoEnabledPadron"); }
        }
        private bool discapacidadEnabledPadron;
        public bool DiscapacidadEnabledPadron
        {
            get { return discapacidadEnabledPadron; }
            set { discapacidadEnabledPadron = value; OnPropertyChanged("DiscapacidadEnabledPadron"); }
        }
        private bool _IsDetalleInternosEnablePadron;
        public bool IsDetalleInternosEnablePadron
        {
            get { return _IsDetalleInternosEnablePadron; }
            set
            {
                _IsDetalleInternosEnablePadron = value;
                OnPropertyChanged("IsDetalleInternosEnablePadron");
            }
        }
        private bool entidadEnabledPadron;
        public bool EntidadEnabledPadron
        {
            get { return entidadEnabledPadron; }
            set { entidadEnabledPadron = value; OnPropertyChanged("EntidadEnabledPadron"); }
        }
        private bool municipioEnabledPadron;
        public bool MunicipioEnabledPadron
        {
            get { return municipioEnabledPadron; }
            set { municipioEnabledPadron = value; OnPropertyChanged("MunicipioEnabledPadron"); }
        }
        private bool coloniaEnabledPadron;
        public bool ColoniaEnabledPadron
        {
            get { return coloniaEnabledPadron; }
            set { coloniaEnabledPadron = value; OnPropertyChanged("ColoniaEnabledPadron"); }
        }
        private bool fotoHuellaEnabledPadron;
        public bool FotoHuellaEnabledPadron
        {
            get { return fotoHuellaEnabledPadron; }
            set { fotoHuellaEnabledPadron = value; OnPropertyChanged("FotoHuellaEnabledPadron"); }
        }
        #endregion

        #region ASIGNACION
        private bool SelectParentescoIngresoEnabledAsignacion = false;
        private bool seleccionarVisitaExistenteAsignacion;
        public bool SeleccionarVisitaExistenteAsignacion
        {
            get { return seleccionarVisitaExistenteAsignacion; }
            set { seleccionarVisitaExistenteAsignacion = value; OnPropertyChanged("SeleccionarVisitaExistenteAsignacion"); }
        }
        private bool contextMenuEnabledAsignacion;
        public bool ContextMenuEnabledAsignacion
        {
            get { return contextMenuEnabledAsignacion; }
            set { contextMenuEnabledAsignacion = value; OnPropertyChanged("ContextMenuEnabledAsignacion"); }
        }
        public bool ValidarEnabledAsignacion { get; set; }
        public bool GeneralEnabledAsignacion { get; set; }
        private bool codigoEnabledAsignacion;
        public bool CodigoEnabledAsignacion
        {
            get { return codigoEnabledAsignacion; }
            set { codigoEnabledAsignacion = value; OnPropertyChanged("CodigoEnabledAsignacion"); }
        }
        private bool discapacitadoEnabledAsignacion;
        public bool DiscapacitadoEnabledAsignacion
        {
            get { return discapacitadoEnabledAsignacion; }
            set { discapacitadoEnabledAsignacion = value; OnPropertyChanged("DiscapacitadoEnabledAsignacion"); }
        }
        private bool discapacidadEnabledAsignacion;
        public bool DiscapacidadEnabledAsignacion
        {
            get { return discapacidadEnabledAsignacion; }
            set { discapacidadEnabledAsignacion = value; OnPropertyChanged("DiscapacidadEnabledAsignacion"); }
        }
        private bool _IsDetalleInternosEnableAsignacion;
        public bool IsDetalleInternosEnableAsignacion
        {
            get { return _IsDetalleInternosEnableAsignacion; }
            set
            {
                _IsDetalleInternosEnableAsignacion = value;
                OnPropertyChanged("IsDetalleInternosEnableAsignacion");
            }
        }
        private bool entidadEnabledAsignacion;
        public bool EntidadEnabledAsignacion
        {
            get { return entidadEnabledAsignacion; }
            set { entidadEnabledAsignacion = value; OnPropertyChanged("EntidadEnabledAsignacion"); }
        }
        private bool municipioEnabledAsignacion;
        public bool MunicipioEnabledAsignacion
        {
            get { return municipioEnabledAsignacion; }
            set { municipioEnabledAsignacion = value; OnPropertyChanged("MunicipioEnabledAsignacion"); }
        }
        private bool coloniaEnabledAsignacion;
        public bool ColoniaEnabledAsignacion
        {
            get { return coloniaEnabledAsignacion; }
            set { coloniaEnabledAsignacion = value; OnPropertyChanged("ColoniaEnabledAsignacion"); }
        }
        private bool fotoHuellaEnabledAsignacion;
        public bool FotoHuellaEnabledAsignacion
        {
            get { return fotoHuellaEnabledAsignacion; }
            set { fotoHuellaEnabledAsignacion = value; OnPropertyChanged("FotoHuellaEnabledAsignacion"); }
        }
        #endregion


        #endregion

        #region Fotos
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
        private WebCam CamaraWeb;
        private List<ImageSourceToSave> _ImagesToSave;
        public List<ImageSourceToSave> ImagesToSave
        {
            get { return _ImagesToSave; }
            set { _ImagesToSave = value; }
        }
        //private List<ImageSourceToSave> _ImageFrontal;
        //public List<ImageSourceToSave> ImageFrontal
        //{
        //    get { return _ImageFrontal; }
        //    set { _ImageFrontal = value; }
        //}
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
        private ImageSource _FotoVisitaPadron = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
        private ImageSource _FotoVisitaCredencial = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
        public ImageSource FotoVisita
        {
            get
            {
                switch (SelectedTab)
                {
                    //case TabsVisita.ASIGNACION_DE_VISITAS:
                    //    return TextObservacionAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return _FotoVisitaPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return _FotoVisitaCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    _FotoVisitaPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    _FotoVisitaCredencial = value;
                OnPropertyChanged("FotoVisita");
            }
        }
        private ImageSource imagenVisitante = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
        public ImageSource ImagenVisitante
        {
            get { return imagenVisitante; }
            set { imagenVisitante = value; OnPropertyChanged("ImagenVisitante"); }
        }
        private byte[] imagenPersonaPadron = new Imagenes().getImagenPerson();
        private byte[] imagenPersonaCredencial = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get
            {
                switch (SelectedTab)
                {
                    //case TabsVisita.ASIGNACION_DE_VISITAS:
                    //    return TextObservacionAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return imagenPersonaPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return imagenPersonaCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    imagenPersonaPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    imagenPersonaCredencial = value;
                OnPropertyChanged("ImagenPersona");
            }
        }
        private bool FotoTomada = false;
        private bool botonTomarFotoEnabled = false;
        public bool BotonTomarFotoEnabled
        {
            get { return botonTomarFotoEnabled; }
            set { botonTomarFotoEnabled = value; OnPropertyChanged("BotonTomarFotoEnabled"); }
        }
        #endregion

        #region Huellas
        IList<PlantillaBiometrico> HuellasCapturadas;
        private Visibility _ShowPopUp = Visibility.Hidden;
        public Visibility ShowPopUp
        {
            get { return _ShowPopUp; }
            set { _ShowPopUp = value; OnPropertyChanged("ShowPopUp"); }
        }
        private Visibility _ShowPanel = Visibility.Hidden;
        public Visibility ShowPanel
        {
            get { return _ShowPanel; }
            set { _ShowPanel = value; OnPropertyChanged("ShowPanel"); }
        }
        private Visibility _ShowFingerPrint = Visibility.Hidden;
        public Visibility ShowFingerPrint
        {
            get { return _ShowFingerPrint; }
            set { _ShowFingerPrint = value; OnPropertyChanged("ShowFingerPrint"); }
        }
        private Visibility _ShowLine = Visibility.Hidden;
        public Visibility ShowLine
        {
            get { return _ShowLine; }
            set { _ShowLine = value; OnPropertyChanged("ShowLine"); }
        }
        private Visibility _ShowOk = Visibility.Hidden;
        public Visibility ShowOk
        {
            get { return _ShowOk; }
            set { _ShowOk = value; OnPropertyChanged("ShowOk"); }
        }
        private ImageSource _GuardaHuella;
        public ImageSource GuardaHuella
        {
            get { return _GuardaHuella; }
            set { _GuardaHuella = value; OnPropertyChanged("GuardaHuella"); }
        }

        #region PADRON

        //IList<PlantillaBiometrico> HuellasCapturadasPadron;

        private Visibility _ShowPopUpPadron;
        public Visibility ShowPopUpPadron
        {
            get { return _ShowPopUpPadron; }
            set { _ShowPopUpPadron = value; OnPropertyChanged("ShowPopUpPadron"); }
        }
        private Visibility _ShowPanelPadron;
        public Visibility ShowPanelPadron
        {
            get { return _ShowPanelPadron; }
            set { _ShowPanelPadron = value; OnPropertyChanged("ShowPanelPadron"); }
        }
        private Visibility _ShowFingerPrintPadron = Visibility.Hidden;
        public Visibility ShowFingerPrintPadron
        {
            get { return _ShowFingerPrintPadron; }
            set { _ShowFingerPrintPadron = value; OnPropertyChanged("ShowFingerPrintPadron"); }
        }
        private Visibility _ShowLinePadron = Visibility.Hidden;
        public Visibility ShowLinePadron
        {
            get { return _ShowLinePadron; }
            set { _ShowLinePadron = value; OnPropertyChanged("ShowLinePadron"); }
        }
        private Visibility _ShowOkPadron = Visibility.Hidden;
        public Visibility ShowOkPadron
        {
            get { return _ShowOkPadron; }
            set { _ShowOkPadron = value; OnPropertyChanged("ShowOkPadron"); }
        }
        private ImageSource _GuardaHuellaPadron;
        public ImageSource GuardaHuellaPadron
        {
            get { return _GuardaHuellaPadron; }
            set { _GuardaHuellaPadron = value; OnPropertyChanged("GuardaHuellaPadron"); }
        }
        #endregion

        #region ASIGNACION
        //IList<PlantillaBiometrico> HuellasCapturadasAsignacion;
        private Visibility _ShowPopUpAsignacion;
        public Visibility ShowPopUpAsignacion
        {
            get { return _ShowPopUpAsignacion; }
            set { _ShowPopUpAsignacion = value; OnPropertyChanged("ShowPopUpAsignacion"); }
        }
        private Visibility _ShowPanelAsignacion;
        public Visibility ShowPanelAsignacion
        {
            get { return _ShowPanelAsignacion; }
            set { _ShowPanelAsignacion = value; OnPropertyChanged("ShowPanelAsignacion"); }
        }
        private Visibility _ShowFingerPrintAsignacion;
        public Visibility ShowFingerPrintAsignacion
        {
            get { return _ShowFingerPrintAsignacion; }
            set { _ShowFingerPrintAsignacion = value; OnPropertyChanged("ShowFingerPrintAsignacion"); }
        }
        private Visibility _ShowLineAsignacion;
        public Visibility ShowLineAsignacion
        {
            get { return _ShowLineAsignacion; }
            set { _ShowLineAsignacion = value; OnPropertyChanged("ShowLineAsignacion"); }
        }
        private Visibility _ShowOkAsignacion;
        public Visibility ShowOkAsignacion
        {
            get { return _ShowOkAsignacion; }
            set { _ShowOkAsignacion = value; OnPropertyChanged("ShowOkAsignacion"); }
        }
        private ImageSource _GuardaHuellaAsignacion;
        public ImageSource GuardaHuellaAsignacion
        {
            get { return _GuardaHuellaAsignacion; }
            set { _GuardaHuellaAsignacion = value; OnPropertyChanged("GuardaHuellaAsignacion"); }
        }
        #endregion

        #endregion

        #region Tabs
        private bool padronVisitasTab;
        public bool PadronVisitasTab
        {
            get { return padronVisitasTab; }
            set
            {
                padronVisitasTab = value;
                if (value)
                {
                    base.ClearRules();
                    SelectedTab = TabsVisita.PADRON_DE_VISITAS;
                    AnioD = AnioDPadron;
                    FolioD = FolioDPadron;
                    AnioBuscar = AnioBuscarPadron;
                    FolioBuscar = FolioBuscarPadron;
                    PaternoD = PaternoDPadron;
                    MaternoD = MaternoDPadron;
                    NombreD = NombreDPadron;
                    IngresosD = IngresosDPadron;
                    NoControlD = NoControlDPadron;
                    UbicacionD = UbicacionDPadron;
                    TipoSeguridadD = TipoSeguridadDPadron;
                    FecIngresoD = FecIngresoDPadron;
                    ClasificacionJuridicaD = ClasificacionJuridicaDPadron;
                    EstatusD = EstatusDPadron;
                    ImagenIngreso = ImagenIngresoDPadron;
                    FotoVisita = _FotoVisitaPadron;

                    SelectParentescoIngresoEnabled = SelectParentescoIngresoEnabledPadron;
                    ValidarEnabled = ValidarEnabledPadron;
                    GeneralEnabled = GeneralEnabledPadron;
                    SituacionEnabled = SituacionEnabledPadron;
                    ListadoInternos = _ListadoInternosPadron;
                    ListVisitantesImputado = ListVisitantesImputadoPadron == null ? null : new ObservableCollection<PERSONAVISITAAUXILIAR>(ListVisitantesImputadoPadron);

                    SelectPersonaVisitante = SelectPersonaVisitantePadron;
                    SelectVisitanteInterno = SelectVisitanteInternoPadron;
                    SelectVisitante = SelectVisitantePadron;
                    SelectSexo = SelectSexoPadron;
                    FechaNacimiento = FechaNacimientoPadron;
                    SelectSituacion = SelectSituacionPadron;
                    SelectAccesoUnico = SelectAccesoUnicoPadron;
                    var discPadron = SelectDiscapacitadoPadron;
                    SelectTipoVisitante = SelectTipoVisitantePadron;
                    SelectDiscapacitado = discPadron;
                    SelectDiscapacidad = SelectDiscapacidadPadron;
                    SelectParentesco = SelectParentescoPadron;

                    //MenuGuardarEnabled = MenuGuardarEnabledPadron;

                    TextCodigo = TextCodigoPadron;
                    TextNombre = TextNombrePadron;
                    TextPaterno = TextPaternoPadron;
                    TextMaterno = TextMaternoPadron;
                    TextCurp = TextCurpPadron;
                    TextRfc = TextRfcPadron;
                    TextTelefono = TextTelefonoPadron;
                    TextEdad = TextEdadPadron;
                    TextCorreo = TextCorreoPadron;
                    //TextNip = TextNipPadron;
                    TextFechaUltimaModificacion = TextFechaUltimaModificacionPadron;
                    TextCalle = TextCallePadron;
                    TextNumExt = TextNumExtPadron;
                    TextNumInt = TextNumIntPadron;
                    TextCodigoPostal = TextCodigoPostalPadron;
                    TextObservacion = TextObservacionPadron;

                    var mun = SelectMunicipioPadron;
                    var col = SelectColoniaPadron;
                    SelectPais = SelectPaisPadron;
                    SelectEntidad = SelectEntidadPadron;
                    SelectMunicipio = mun;
                    SelectColonia = col;
                    SelectParentesco = SelectParentescoPadron;
                    SelectEstatusRelacion = selectEstatusRelacionPadron;

                    ListAcompanantes = ListAcompanantesPadron;
                    ImagenPersona = imagenPersonaPadron;
                    SelectVisitanteIngreso = SelectVisitanteIngresoPadron;
                    SelectImputadoIngreso = SelectImputadoIngresoPadron;
                    ListVisitantes = listVisitantesPadron;
                    CodigoEnabled = CodigoEnabledPadron;
                    if (SelectVisitanteIngresoPadron != null)
                        SetValidacionesGenerales();
                }
                OnPropertyChanged("PadronVisitasTab");
            }
        }

        private bool asignacionVisitasTab;
        public bool AsignacionVisitasTab
        {
            get { return asignacionVisitasTab; }
            set
            {
                asignacionVisitasTab = value;
                if (value)
                {
                    base.ClearRules();
                    SelectedTab = TabsVisita.ASIGNACION_DE_VISITAS;
                    AnioD = AnioDAsignacion;
                    FolioD = FolioDAsignacion;
                    AnioBuscar = AnioBuscarAsignacion;
                    FolioBuscar = FolioBuscarAsignacion;
                    PaternoD = PaternoDAsignacion;
                    MaternoD = MaternoDAsignacion;
                    NombreD = NombreDAsignacion;
                    IngresosD = IngresosDAsignacion;
                    NoControlD = NoControlDAsignacion;
                    UbicacionD = UbicacionDAsignacion;
                    TipoSeguridadD = TipoSeguridadDAsignacion;
                    FecIngresoD = FecIngresoDAsignacion;
                    ClasificacionJuridicaD = ClasificacionJuridicaDAsignacion;
                    EstatusD = EstatusDAsignacion;
                    ImagenIngreso = ImagenIngresoDAsignacion;

                    SelectParentescoIngresoEnabled = SelectParentescoIngresoEnabledAsignacion;
                    ValidarEnabled = ValidarEnabledAsignacion;
                    GeneralEnabled = GeneralEnabledAsignacion;
                    SituacionEnabled = SituacionEnabledAsignacion;
                    ListadoInternos = _ListadoInternosAsignacion;
                    ListVisitantesImputado = ListVisitantesImputadoAsignacion == null ? null : new ObservableCollection<PERSONAVISITAAUXILIAR>(ListVisitantesImputadoAsignacion);

                    SelectPersonaVisitante = SelectPersonaVisitanteAsignacion;
                    SelectVisitanteInterno = SelectVisitanteInternoAsignacion;
                    SelectVisitante = SelectVisitanteAsignacion;
                    SelectSexo = SelectSexoAsignacion;
                    FechaNacimiento = FechaNacimientoAsignacion;
                    SelectSituacion = SelectSituacionAsignacion;
                    SelectAccesoUnico = SelectAccesoUnicoAsignacion;
                    SelectTipoVisitante = SelectTipoVisitanteAsignacion;
                    SelectDiscapacitado = SelectDiscapacitadoAsignacion;
                    SelectDiscapacidad = SelectDiscapacidadAsignacion;
                    SelectParentesco = SelectParentescoAsignacion;

                    //MenuGuardarEnabled = MenuGuardarEnabledAsignacion;

                    TextCodigo = TextCodigoAsignacion;
                    TextNombre = TextNombreAsignacion;
                    TextPaterno = TextPaternoAsignacion;
                    TextMaterno = TextMaternoAsignacion;
                    TextCurp = TextCurpAsignacion;
                    TextRfc = TextRfcAsignacion;
                    TextTelefono = TextTelefonoAsignacion;
                    TextEdad = TextEdadAsignacion;
                    TextCorreo = TextCorreoAsignacion;
                    //TextNip = TextNipAsignacion;
                    TextFechaUltimaModificacion = TextFechaUltimaModificacionAsignacion;
                    TextCalle = TextCalleAsignacion;
                    TextNumExt = TextNumExtAsignacion;
                    TextNumInt = TextNumIntAsignacion;
                    TextCodigoPostal = TextCodigoPostalAsignacion;
                    TextObservacion = TextObservacionAsignacion;

                    var mun = SelectMunicipioAsignacion;
                    var col = SelectColoniaAsignacion;
                    SelectPais = SelectPaisAsignacion;
                    SelectEntidad = SelectEntidadAsignacion;
                    SelectMunicipio = mun;
                    SelectColonia = col;
                    ListVisitantes = ListVisitantesAsignacion;
                    CodigoEnabled = CodigoEnabledAsignacion;
                    if (SelectImputadoIngreso != null)
                    {

                        var autorizadas = new ObservableCollection<VISITA_AUTORIZADA>();
                        var lista = new List<PERSONAVISITAAUXILIAR>();
                        Application.Current.Dispatcher.Invoke((Action)(async delegate
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                                {
                                    #region VISITA_AUTORIZADA
                                    autorizadas = new cVisitaAutorizada().ObtenerXIngresoYNoCredencializados(SelectImputadoIngreso.ID_CENTRO, SelectImputadoIngreso.ID_ANIO, SelectImputadoIngreso.ID_IMPUTADO, SelectImputadoIngreso.ID_INGRESO);
                                    lista = autorizadas.Select(s => new PERSONAVISITAAUXILIAR
                                    {
                                        ID_CENTRO = s.ID_CENTRO,
                                        ID_ANIO = s.ID_ANIO,
                                        ID_IMPUTADO = s.ID_IMPUTADO,
                                        INGRESO = s.INGRESO,
                                        ID_VISITA = s.ID_VISITA,
                                        ID_COLONIA = s.ID_COLONIA,
                                        ID_MUNICIPIO = s.ID_MUNICIPIO,
                                        ID_ENTIDAD = s.ID_ENTIDAD,
                                        ID_PAIS = s.ID_PAIS,
                                        SEXO = s.SEXO,
                                        ID_ESTATUS_VISITA = (short)(s.ESTATUS == 0 ? Parametro.ID_ESTATUS_VISITA_REGISTRO : Parametro.ID_ESTATUS_VISITA_SUSPENDIDO),
                                        ESTATUS_VISITA_DESCR = s.ESTATUS == 0 ? "REGISTRO" : "SUSPENDIDO",
                                        INGRESO_NOMBRE = s.INGRESO.IMPUTADO.NOMBRE,
                                        INGRESO_PATERNO = s.INGRESO.IMPUTADO.PATERNO,
                                        INGRESO_MATERNO = s.INGRESO.IMPUTADO.MATERNO,
                                        ID_PARENTESCO = s.ID_PARENTESCO,
                                        PARENTESCO_DESCR = s.TIPO_REFERENCIA.DESCR,
                                        EDAD = s.EDAD,
                                        TIPO_REFERENCIA = s.TIPO_REFERENCIA,
                                        ESTATUS_VISITA = new ESTATUS_VISITA() { ID_ESTATUS_VISITA = (short)(s.ESTATUS == 0 ? Parametro.ID_ESTATUS_VISITA_REGISTRO : Parametro.ID_ESTATUS_VISITA_SUSPENDIDO), DESCR = s.ESTATUS == 0 ? "REGISTRO" : "SUSPENDIDO" },
                                        TELEFONO = s.TELEFONO,
                                        DOMICILIO = s.DOMICILIO_CALLE + " " + (s.DOMICILIO_NUM_EXT.HasValue ? s.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty) + " " + s.DOMICILIO_NUM_INT + " " + (s.DOMICILIO_CODIGO_POSTAL.HasValue ? s.DOMICILIO_CODIGO_POSTAL.Value.ToString() : string.Empty),

                                        NOMBRE = s.NOMBRE,
                                        MATERNO = s.MATERNO,
                                        PATERNO = s.PATERNO,
                                        OBJETO_VISITA_AUTORIZADA = s
                                    }).ToList();
                                    #endregion

                                    #region PERSONAS
                                    var personas = new cPersona().ObtenerPersonasVisitantesXIngreso(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                                    ///TODO: checar buscar personas
                                    lista.AddRange(personas.ToList().Select(s => new PERSONAVISITAAUXILIAR()
                                    {
                                        ID_PERSONA = s.ID_PERSONA,
                                        ID_COLONIA = s.ID_COLONIA,
                                        ID_MUNICIPIO = s.ID_MUNICIPIO,
                                        ID_ENTIDAD = s.ID_ENTIDAD,
                                        ID_PAIS = s.ID_PAIS,
                                        SEXO = s.SEXO,
                                        RFC = s.RFC,
                                        CURP = s.CURP,
                                        TELEFONO = s.TELEFONO,
                                        FECHA_NACIMIENTO = s.FEC_NACIMIENTO,
                                        EDAD = s.FEC_NACIMIENTO.HasValue ? (short)((Fechas.GetFechaDateServer - s.FEC_NACIMIENTO.Value).TotalDays / 365) : new Nullable<short>(),
                                        DOMICILIO = s.DOMICILIO_CALLE + " " + (s.DOMICILIO_NUM_EXT.HasValue ? s.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty) + " " + s.DOMICILIO_NUM_INT + " " + (s.DOMICILIO_CODIGO_POSTAL.HasValue ? s.DOMICILIO_CODIGO_POSTAL.Value.ToString() : string.Empty),
                                        NOMBRE = s.NOMBRE,
                                        MATERNO = s.MATERNO,
                                        PATERNO = s.PATERNO,
                                        OBJETO_PERSONA = s,

                                        DE_TERMINO = s.VISITANTE != null ?
                                            (s.VISITANTE.VISITANTE_INGRESO.Any() ?
                                                s.VISITANTE.VISITANTE_INGRESO.Any(f => f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_ANIO == SelectIngreso.ID_ANIO && f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_INGRESO == SelectIngreso.ID_INGRESO) ?
                                                    s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO && f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).VISITANTE_INGRESO_PASE.Any(a => a.ID_PASE == (short)enumTiposPases.DE_TERMINO ?
                                                        a.AUTORIZADO == "S" ?
                                                            a.ADUANA_INGRESO != null ?
                                                                a.ADUANA_INGRESO.Any()
                                                            : false
                                                        : false
                                                    : false)
                                                : false
                                            : false)
                                        : false,
                                        ID_ESTATUS_VISITA = s.VISITANTE != null ? s.VISITANTE.ID_ESTATUS_VISITA : new Nullable<short>(),
                                        ESTATUS_VISITA = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).ESTATUS_VISITA : null) : null,
                                        ESTATUS_VISITA_DESCR = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).ESTATUS_VISITA.DESCR : null) : null,
                                        INGRESO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO : null) : null,
                                        PARENTESCO_DESCR = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).TIPO_REFERENCIA.DESCR : "SIN INFORMACION") : null,
                                        ID_PARENTESCO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).TIPO_REFERENCIA.ID_TIPO_REFERENCIA : (short)9999) : new Nullable<short>(),
                                        TIPO_REFERENCIA = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).TIPO_REFERENCIA : new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = 9999, DESCR = "SIN INFORMACION" }) : null,
                                        INGRESO_NOMBRE = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO.IMPUTADO.NOMBRE : string.Empty) : null,
                                        INGRESO_PATERNO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO.IMPUTADO.PATERNO : string.Empty) : null,
                                        INGRESO_MATERNO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO.IMPUTADO.MATERNO : string.Empty) : null,
                                        ID_CENTRO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).ID_CENTRO : (short)0) : new Nullable<short>(),
                                        ID_ANIO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO.ID_ANIO : new Nullable<short>()) : new Nullable<short>(),
                                        ID_IMPUTADO = s.VISITANTE != null ? (s.VISITANTE.VISITANTE_INGRESO.Any() ? s.VISITANTE.VISITANTE_INGRESO.First(f => f.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && f.ID_ANIO == SelectIngreso.ID_ANIO &&
                                            f.ID_CENTRO == SelectIngreso.ID_CENTRO && f.ID_INGRESO == SelectIngreso.ID_INGRESO).INGRESO.ID_IMPUTADO : new Nullable<int>()) : new Nullable<int>(),
                                    }));
                                    #endregion
                                });
                            ListVisitantesImputado = new ObservableCollection<PERSONAVISITAAUXILIAR>(lista);
                        }));
                    }
                }
                OnPropertyChanged("AsignacionVisitasTab");
            }
        }

        private bool _CredencialesTab;
        public bool CredencialesTab
        {
            get { return _CredencialesTab; }
            set
            {
                _CredencialesTab = value;
                if (value)
                {
                    base.ClearRules();
                    SelectedTab = TabsVisita.ENTREGA_CREDENCIALES;
                    AnioD = AnioDCredencial;
                    FolioD = FolioDCredencial;
                    PaternoD = PaternoDCredencial;
                    MaternoD = MaternoDCredencial;
                    NombreD = NombreDCredencial;
                    IngresosD = IngresosDCredencial;
                    NoControlD = NoControlDCredencial;
                    UbicacionD = UbicacionDCredencial;
                    TipoSeguridadD = TipoSeguridadDCredencial;
                    FecIngresoD = FecIngresoDCredencial;
                    ClasificacionJuridicaD = ClasificacionJuridicaDCredencial;
                    EstatusD = EstatusDCredencial;

                    SelectSexo = SelectSexoCredencial;
                    FechaNacimiento = FechaNacimientoCredencial;
                    TextCodigo = TextCodigoCredencial;
                    TextNombre = TextNombreCredencial;
                    TextPaterno = TextPaternoCredencial;
                    TextMaterno = TextMaternoCredencial;
                    TextEdad = TextEdadCredencial;
                    //TextNip = TextNipCredencial;
                    TextObservacion = TextObservacionCredencial;
                    ImagenIngreso = ImagenIngresoDCredencial;
                    SelectSituacion = SelectSituacionCredencial;
                    SelectTipoVisitante = SelectTipoVisitanteCredencial;
                    FotoVisita = _FotoVisitaCredencial;
                    ListadoInternos = _ListadoInternosCredencial;

                    SelectParentesco = SelectParentescoCredencial;
                    SelectEstatusRelacion = selectEstatusRelacionCredencial;
                    ListAcompanantes = listAcompanantesCredencial;
                    ImagenPersona = imagenPersonaCredencial;
                    SelectVisitanteIngreso = SelectVisitanteIngresoCredencial;
                    SelectImputadoIngreso = SelectImputadoIngresoCredencial;
                    ListVisitantes = listVisitantesCredencial;
                    CodigoEnabled = CodigoEnabledEntrega;
                }
                OnPropertyChanged("CredencialesTab");
            }
        }
        #endregion

        #region ACOMPANANTE
        private bool emptyBuscarAcompanante;
        public bool EmptyBuscarAcompanante
        {
            get { return emptyBuscarAcompanante; }
            set { emptyBuscarAcompanante = value; OnPropertyChanged("EmptyBuscarAcompanante"); }
        }
        private string textNombreAcompanante;
        public string TextNombreAcompanante
        {
            get { return textNombreAcompanante; }
            set { textNombreAcompanante = value; OnPropertyChanged("TextNombreAcompanante"); }
        }
        private string textPaternoAcompanante;
        public string TextPaternoAcompanante
        {
            get { return textPaternoAcompanante; }
            set { textPaternoAcompanante = value; OnPropertyChanged("TextPaternoAcompanante"); }
        }
        private string textMaternoAcompanante;
        public string TextMaternoAcompanante
        {
            get { return textMaternoAcompanante; }
            set { textMaternoAcompanante = value; OnPropertyChanged("TextMaternoAcompanante"); }
        }
        private ACOMPANANTE selectAcompananteAuxiliar;
        public ACOMPANANTE SelectAcompananteAuxiliar
        {
            get { return selectAcompananteAuxiliar; }
            set { selectAcompananteAuxiliar = value; OnPropertyChanged("SelectAcompananteAuxiliar"); }
        }
        private ACOMPANANTE selectAcompanante;
        public ACOMPANANTE SelectAcompanante
        {
            get { return selectAcompanante; }
            set
            {
                selectAcompanante = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    if (SelectVisitanteIngreso != null && value != null)
                        if (SelectVisitanteIngreso.ACOMPANANTE.Where(w => w.ID_VISITANTE == value.ID_VISITANTE && w.ID_ACOMPANANTE == value.ID_ACOMPANANTE).Any())
                            BorrarMenuAcompananteVisible = Visibility.Collapsed;
                        else
                            BorrarMenuAcompananteVisible = Visibility.Visible;
                    else
                        BorrarMenuAcompananteVisible = Visibility.Collapsed;
                else
                    BorrarMenuAcompananteVisible = Visibility.Collapsed;
                OnPropertyChanged("SelectAcompanante");
            }
        }
        private Visibility borrarMenuAcompananteVisible = Visibility.Collapsed;
        public Visibility BorrarMenuAcompananteVisible
        {
            get { return borrarMenuAcompananteVisible; }
            set { borrarMenuAcompananteVisible = value; OnPropertyChanged("BorrarMenuAcompananteVisible"); }
        }
        private short selectParentescoAcompanante;
        public short SelectParentescoAcompanante
        {
            get { return selectParentescoAcompanante; }
            set { selectParentescoAcompanante = value; OnPropertyChanged("SelectParentescoAcompanante"); }
        }
        private VISITANTE_INGRESO selectBuscarAcompanante;
        public VISITANTE_INGRESO SelectBuscarAcompanante
        {
            get { return selectBuscarAcompanante; }
            set
            {
                selectBuscarAcompanante = value;
                /*if (value != null)
                {
                    SelectAcompanante = new ACOMPANANTE
                    {
                        ID_ACOMPANANTE = value.ID_PERSONA,
                        ACO_ID_ANIO = value.ID_ANIO,
                        ACO_ID_CENTRO = value.ID_CENTRO,
                        ACO_ID_IMPUTADO = value.ID_IMPUTADO,
                        ACO_ID_INGRESO = value.ID_INGRESO
                    };
                }*/
                OnPropertyChanged("SelectBuscarAcompanante");
            }
        }
        private ObservableCollection<VISITANTE_INGRESO> listBuscarAcompanantes;
        public ObservableCollection<VISITANTE_INGRESO> ListBuscarAcompanantes
        {
            get { return listBuscarAcompanantes; }
            set { listBuscarAcompanantes = value; OnPropertyChanged("ListBuscarAcompanantes"); }
        }
        #endregion

        #region CADENA DOMICILIO
        private List<ENTIDAD> ListEntidadesAuxiliares;
        private List<MUNICIPIO> ListMunicipiosAuxiliares;
        private List<COLONIA> ListColoniasAuxiliares;
        #endregion

        private GafetesView GafeteView;
        private bool NuevaVisitaAgenda = false;
        private List<string> ListLetras;
        private bool _InsertarNuevoImputadoVisita = false;
        public bool InsertarNuevoImputadoVisita
        {
            get { return _InsertarNuevoImputadoVisita; }
            set { _InsertarNuevoImputadoVisita = value; OnPropertyChanged("InsertarNuevoImputadoVisita"); }
        }
        private bool TipoPaseAbre = false;
        private bool banderaEditarAsignacion;
        private bool banderaEditarPadron;
        public bool BanderaEditar
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return banderaEditarAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return banderaEditarPadron;
                    default:
                        return true;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    banderaEditarAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    banderaEditarPadron = value;
                OnPropertyChanged("BanderaEditar");
            }
        }
        private bool _AutoGuardado = true;
        private VISITANTE_INGRESO NuevoListaTonta;
        public bool AutoGuardado
        {
            get { return _AutoGuardado; }
            set
            {
                _AutoGuardado = value;
                OnPropertyChanged("AutoGuardado");
            }
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

        public enum TabsVisita
        {
            ASIGNACION_DE_VISITAS = 0,
            PADRON_DE_VISITAS = 1,
            ENTREGA_CREDENCIALES = 2
        }

        #region Camara

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
                    MenuReporteEnabled = value;
            }
        }

        /*private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }*/

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

        #region Pantalla
        private int tabIndice;
        public int TabIndice
        {
            get { return tabIndice; }
            set
            {
                tabIndice = value;
                if (value == 1)
                {
                    if (SelectVisitante != null)
                        SetValidacionesGenerales();
                }

                OnPropertyChanged("TabIndice");
            }
        }
        #endregion

        #region Validacion Tipo Documento
        private short VisitanteOrdinario = Parametro.ID_TIPO_VISITANTE_ORDINARIO;
        private short VisitanteDiscapacidad = Parametro.ID_TIPO_VISITANTE_DISCAPACITADO;
        private short VisitanteIntima = Parametro.ID_TIPO_VISITANTE_INTIMA;
        #endregion

        #region Programacion de Visita
        private bool pVDomingo = false;
        public bool PVDomingo
        {
            get { return pVDomingo; }
            set { pVDomingo = value; OnPropertyChanged("PVDomingo"); }
        }

        private bool pVLunes = false;
        public bool PVLunes
        {
            get { return pVLunes; }
            set { pVLunes = value; OnPropertyChanged("PVLunes"); }
        }

        private bool pVMartes = false;
        public bool PVMartes
        {
            get { return pVMartes; }
            set { pVMartes = value; OnPropertyChanged("PVMartes"); }
        }

        private bool pVMiercoles = false;
        public bool PVMiercoles
        {
            get { return pVMiercoles; }
            set { pVMiercoles = value; OnPropertyChanged("PVMiercoles"); }
        }

        private bool pVJueves = false;
        public bool PVJueves
        {
            get { return pVJueves; }
            set { pVJueves = value; OnPropertyChanged("PVJueves"); }
        }

        private bool pVViernes = false;
        public bool PVViernes
        {
            get { return pVViernes; }
            set { pVViernes = value; OnPropertyChanged("PVViernes"); }
        }

        private bool pVSabado = false;
        public bool PVSabado
        {
            get { return pVSabado; }
            set { pVSabado = value; OnPropertyChanged("PVSabado"); }
        }

        private DateTime horaUpperVal = new DateTime(0001, 01, 01, 19, 0, 0);
        public DateTime HoraUpperVal
        {
            get { return horaUpperVal; }
            set { horaUpperVal = value; OnPropertyChanged("HoraUpperVal"); }
        }

        private DateTime horaLowerVal = new DateTime(0001, 01, 01, 7, 0, 0);
        public DateTime HoraLowerVal
        {
            get { return horaLowerVal; }
            set { horaLowerVal = value; OnPropertyChanged("HoraLowerVal"); }
        }

        private DateRangeSlider dRS;
        public DateRangeSlider DRS
        {
            get { return dRS; }
            set { dRS = value; OnPropertyChanged("DRS"); }
        }
        private string _SelectHoraEntrada;
        public string SelectHoraEntrada
        {
            get { return _SelectHoraEntrada; }
            set { _SelectHoraEntrada = value; OnPropertyChanged("SelectHoraEntrada"); }
        }
        private string _SelectHoraSalida;
        public string SelectHoraSalida
        {
            get { return _SelectHoraSalida; }
            set { _SelectHoraSalida = value; OnPropertyChanged("SelectHoraSalida"); }
        }
        private string _SelectMinutoEntrada;
        public string SelectMinutoEntrada
        {
            get { return _SelectMinutoEntrada; }
            set { _SelectMinutoEntrada = value; OnPropertyChanged("SelectMinutoEntrada"); }
        }
        private string _SelectMinutoSalida;
        public string SelectMinutoSalida
        {
            get { return _SelectMinutoSalida; }
            set { _SelectMinutoSalida = value; OnPropertyChanged("SelectMinutoSalida"); }
        }
        #endregion

        #region BUSQUEDA HUELLA
        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }
        private string _TextNIPCartaCompromiso;
        public string TextNIPCartaCompromiso
        {
            get { return _TextNIPCartaCompromiso; }
            set { _TextNIPCartaCompromiso = value; OnPropertyChanged("TextNIPCartaCompromiso"); }
        }
        private CartaCompromisoAceptaView CartaView;
        private bool HuellaCompromiso = false;
        private bool CartaCompromiso = false;
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
        private BuscarPorHuellaYNipView HuellaWindow;
        //public IList<PlantillaBiometrico> HuellasCapturadas { get; set; }
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
                FotoRegistro = value == null ? new Imagenes().getImagenPerson() : new Imagenes().ConvertBitmapToByte((BitmapSource)value.Foto);
                OnPropertyChanged("SelectRegistro");
            }
        }
        private byte[] _FotoRegistro = new Imagenes().getImagenPerson();
        public byte[] FotoRegistro
        {
            get { return _FotoRegistro; }
            set { _FotoRegistro = value; OnPropertyChanged("FotoRegistro"); }
        }
        private string _LabelNipCodigo = "Num. Visita";
        public string LabelNipCodigo
        {
            get { return _LabelNipCodigo; }
            set { _LabelNipCodigo = value; OnPropertyChanged("LabelNipCodigo"); }
        }
        private string _TextNipBusqueda;
        public string TextNipBusqueda
        {
            get { return _TextNipBusqueda; }
            set { _TextNipBusqueda = value; OnPropertyChanged("TextNipBusqueda"); }
        }
        #endregion

        private bool _PaseTerminoEnabled = false;
        public bool PaseTerminoEnabled
        {
            get { return _PaseTerminoEnabled; }
            set { _PaseTerminoEnabled = value; OnPropertyChanged("PaseTerminoEnabled"); }
        }
        private bool _PaseDiasEnabled = false;
        public bool PaseDiasEnabled
        {
            get { return _PaseDiasEnabled; }
            set { _PaseDiasEnabled = value; OnPropertyChanged("PaseDiasEnabled"); }
        }
        private bool _PaseExtraordinarioEnabled = false;
        public bool PaseExtraordinarioEnabled
        {
            get { return _PaseExtraordinarioEnabled; }
            set { _PaseExtraordinarioEnabled = value; OnPropertyChanged("PaseExtraordinarioEnabled"); }
        }

        #region Nota Tecnica
        private string notaTecnica;
        public string NotaTecnica
        {
            get { return notaTecnica; }
            set { notaTecnica = value; OnPropertyChanged("NotaTecnica"); }
        }
        #endregion

        #region Control de Tiempo
        private DateTime? FechaInicioRegistro;
        private DateTime? FechaFinRegistro;
        #endregion
    }

    public class InternoVisita
    {
        public string Año { get; set; }
        public string Folio { get; set; }
        public string Interno { get; set; }
        public string Ubicacion { get; set; }
        public string Situacion { get; set; }
        public string Relacion { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombre { get; set; }
    }

    public class TipoDocumento
    {
        public short ID_TIPO_VISITA { get; set; }
        public short ID_TIPO_DOCUMENTO { get; set; }
        public string DESCR { get; set; }
        public bool DIGITALIZADO { get; set; }
    }

    public class TipoDocumentoInternacion
    {
        public short ID_TIPO_DOCUMENTO { get; set; }
        public string DESCR { get; set; }
        public bool DIGITALIZADO { get; set; }
    }
}

using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using MvvmFramework;

namespace ControlPenales
{
    partial class RegistroLiberadosViewModel
    {
        #region Resolucion
        private bool rMC = false;
        public bool RMC
        {
            get { return rMC; }
            set { rMC = value; OnPropertyValidateChanged("RMC"); }
        }

        private bool rSSP = false;
        public bool RSSP
        {
            get { return rSSP; }
            set { rSSP = value; OnPropertyValidateChanged("RSSP"); }
        }

        private bool rProvp = false;
        public bool RProvp
        {
            get { return rProvp; }
            set { rProvp = value; OnPropertyValidateChanged("RProvp"); }
        }

        private string rNUC;
        public string RNUC
        {
            get { return rNUC; }
            set { rNUC = value; OnPropertyValidateChanged("RNUC"); }
        }
   
        private string rCP;
        public string RCP
        {
            get { return rCP; }
            set { rCP = value; OnPropertyValidateChanged("RCP"); }
        }
        #endregion

        #region Delitos
        private string dDelitos;
        public string DDelitos
        {
            get { return dDelitos; }
            set { dDelitos = value; OnPropertyValidateChanged("DDelitos"); }
        }

        private bool? dReclasificado = false;
        public bool? DReclasificado
        {
            get { return dReclasificado; }
            set { dReclasificado = value; OnPropertyValidateChanged("DReclasificado"); }
        }
        #endregion 

        #region Medida Judicial
        private string mMedidaJudicial;
        public string MMedidaJudicial
        {
            get { return mMedidaJudicial; }
            set { mMedidaJudicial = value; OnPropertyValidateChanged("MMedidaJudicial"); }
        }

        private string mPeridiocidad;
        public string MPeridiocidad
        {
            get { return mPeridiocidad; }
            set { mPeridiocidad = value; OnPropertyValidateChanged("MPeridiocidad"); }
        }

        private DateTime? mApartir;
        public DateTime? MApartir
        {
            get { return mApartir; }
            set { mApartir = value;
            if (value != null)
            {
                DateTime hoy;
                if (SelectMJ == null)
                    hoy = Fechas.GetFechaDateServer;
                else
                    hoy = SelectMJ.APARTIR.Value;
                if (value.Value.Date < hoy.Date.Date)
                {
                    EMensajeErrorFecha = "LA FECHA DEBE SER MAYOR AL DIA DE HOY!";
                    MApartirV = false;
                }
                else
                    MApartirV = true;
            }
            else
            {
                MApartirV = false;
                EMensajeErrorFecha = "APARTIR ES REQUERIDO!"; 
            }
                OnPropertyValidateChanged("MApartir"); }
        }

        private bool mApartirV = false;
        public bool MApartirV
        {
            get { return mApartirV; }
            set { mApartirV = value; OnPropertyChanged("MApartirV"); }
        }
        
        private string eMensajeErrorFecha = "APARTIR ES REQUERIDO!";
        public string EMensajeErrorFecha
        {
            get { return eMensajeErrorFecha; }
            set { eMensajeErrorFecha = value; OnPropertyChanged("EMensajeErrorFecha"); }
        }
      
        private string mDuracion;
        public string MDuracion
        {
            get { return mDuracion; }
            set { mDuracion = value; OnPropertyValidateChanged("MDuracion"); }
        }
        #endregion

        #region Defensor
        private bool? dPublico = true;
        public bool? DPublico
        {
            get { return dPublico; }
            set { dPublico = value;
                ValidarDefensor();
                OnPropertyValidateChanged("DPublico"); 
            }
        }

        private string dNombreDefensor;
        public string DNombreDefensor
        {
            get { return dNombreDefensor; }
            set { dNombreDefensor = value; OnPropertyValidateChanged("DNombreDefensor"); }
        }

        private string dTelefonoDefensor;
        public string DTelefonoDefensor
        {
            get { return dTelefonoDefensor; }
            set
            {
                dTelefonoDefensor = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;
                OnPropertyValidateChanged("DTelefonoDefensor"); }
        }
        #endregion

        #region Aval Moral
        private ObservableCollection<TIPO_REFERENCIA> lstTipoReferencia;
        public ObservableCollection<TIPO_REFERENCIA> LstTipoReferencia
        {
            get { return lstTipoReferencia; }
            set { lstTipoReferencia = value; OnPropertyChanged("LstTipoReferencia"); }
        }

        private string aNombre;
        public string ANombre
        {
            get { return aNombre; }
            set { aNombre = value; OnPropertyValidateChanged("ANombre"); }
        }

        private short aRelacion = -1;
        public short ARelacion
        {
            get { return aRelacion; }
            set { aRelacion = value; OnPropertyValidateChanged("ARelacion"); }
        }

        private string aTiempoConocerlo;
        public string ATiempoConocerlo
        {
            get { return aTiempoConocerlo; }
            set { aTiempoConocerlo = value; OnPropertyValidateChanged("ATiempoConocerlo"); }
        }

        private string aDocmicilio;
        public string ADocmicilio
        {
            get { return aDocmicilio; }
            set { aDocmicilio = value; OnPropertyValidateChanged("ADocmicilio"); }
        }

        private string aTelefonoMovil;
        public string ATelefonoMovil
        {
            get { return aTelefonoMovil; }
            set
            {
                aTelefonoMovil = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value; 
                  OnPropertyValidateChanged("ATelefonoMovil"); }
        }
        
        private string aTelefonoFijo;
        public string ATelefonoFijo
        {
            get { return aTelefonoFijo; }
            set
            {
                aTelefonoFijo = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value; 
                  OnPropertyValidateChanged("ATelefonoFijo"); }
        }
        #endregion

        #region Datos Generales
        private ObservableCollection<ESTADO_CIVIL> listEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> ListEstadoCivil
        {
            get { return listEstadoCivil; }
            set { listEstadoCivil = value; OnPropertyChanged("ListEstadoCivil"); }
        }

        private ObservableCollection<OCUPACION> listOcupacion;
        public ObservableCollection<OCUPACION> ListOcupacion
        {
            get { return listOcupacion; }
            set { listOcupacion = value; OnPropertyChanged("ListOcupacion"); }
        }

        private ObservableCollection<ESCOLARIDAD> listEscolaridad;
        public ObservableCollection<ESCOLARIDAD> ListEscolaridad
        {
            get { return listEscolaridad; }
            set { listEscolaridad = value; OnPropertyChanged("ListEscolaridad"); }
        }

        private ObservableCollection<PAIS_NACIONALIDAD> listPaisNacionalidad;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisNacionalidad
        {
            get { return listPaisNacionalidad; }
            set { listPaisNacionalidad = value; OnPropertyChanged("ListPaisNacionalidad"); }
        }

        private ObservableCollection<IDIOMA> lstIdioma;
        public ObservableCollection<IDIOMA> LstIdioma
        {
            get { return lstIdioma; }
            set { lstIdioma = value; OnPropertyChanged("LstIdioma"); }
        }

        private ObservableCollection<ETNIA> listEtnia;
        public ObservableCollection<ETNIA> ListEtnia
        {
            get { return listEtnia; }
            set { listEtnia = value; OnPropertyChanged("ListEtnia"); }
        }

        private ObservableCollection<DIALECTO> lstDialecto;
        public ObservableCollection<DIALECTO> LstDialecto
        {
            get { return lstDialecto; }
            set { lstDialecto = value; OnPropertyChanged("LstDialecto"); }
        }

        private ObservableCollection<RELIGION> listReligion;
        public ObservableCollection<RELIGION> ListReligion
        {
            get { return listReligion; }
            set { listReligion = value; OnPropertyChanged("ListReligion"); }
        }

        private string selectSexo = "S";
        public string SelectSexo
        {
            get { return selectSexo; }
            set
            {
                //if (value == selectSexo)
                //    return;
                selectSexo = value;
                OnPropertyValidateChanged("SelectSexo");
            }
        }
        
        private short? selectEstadoCivil = -1;
        public short? SelectEstadoCivil
        {
            get { return selectEstadoCivil; }
            set
            {
                if (value == selectEstadoCivil)
                    return;

                selectEstadoCivil = value;

                OnPropertyValidateChanged("SelectEstadoCivil");
            }
        }
        
        private short? selectOcupacion = -1;
        public short? SelectOcupacion
        {
            get { return selectOcupacion; }
            set
            {
                if (value == selectOcupacion)
                    return;

                selectOcupacion = value;

                OnPropertyValidateChanged("SelectOcupacion");
            }
        }
        
        private short? selectEscolaridad = -1;
        public short? SelectEscolaridad
        {
            get { return selectEscolaridad; }
            set
            {
                if (value == selectEscolaridad)
                    return;

                selectEscolaridad = value;

                OnPropertyValidateChanged("SelectEscolaridad");
            }
        }
        
        private short? selectReligion = -1;
        public short? SelectReligion
        {
            get { return selectReligion; }
            set
            {
                if (value == selectReligion)
                    return;

                selectReligion = value;

                OnPropertyValidateChanged("SelectReligion");
            }
        }
        
        private short? selectEtnia = -1;
        public short? SelectEtnia
        {
            get { return selectEtnia; }
            set
            {
                if (value == selectEtnia)
                    return;

                selectEtnia = value;

                OnPropertyValidateChanged("SelectEtnia");
            }
        }

        private short? selectedIdioma = 8;
        public short? SelectedIdioma
        {
            get { return selectedIdioma; }
            set { selectedIdioma = value; OnPropertyChanged("SelectedIdioma"); }
        }

        private short? selectedDialecto = -1;
        public short? SelectedDialecto
        {
            get { return selectedDialecto; }
            set { selectedDialecto = value; OnPropertyChanged("SelectedDialecto"); }
        }

        private bool requiereTraductor = false;
        public bool RequiereTraductor
        {
            get { return requiereTraductor; }
            set { requiereTraductor = value; OnPropertyChanged("RequiereTraductor"); }
        }

        private bool estaturaPesoVisible = false;
        public bool EstaturaPesoVisible
        {
            get { return estaturaPesoVisible; }
            set { estaturaPesoVisible = value; OnPropertyChanged("EstaturaPesoVisible"); }
        }

        private int _TextHorasTrabajo;

           public int TextHorasTrabajo
           {
          get { return _TextHorasTrabajo; }
           set { _TextHorasTrabajo = value; OnPropertyChanged("TextHorasTrabajo");}
              }

        private int _TextDiasTrabajo;

public int TextDiasTrabajo
{
  get { return _TextDiasTrabajo; }
  set { _TextDiasTrabajo = value; OnPropertyChanged("TextDiasTrabajo");}
     }



    

        #endregion

        #region Observaciones
        private string _TextObservaciones;

        public string TextObservaciones
        {
            get { return _TextObservaciones; }
            set { _TextObservaciones = value; OnPropertyChanged("TextObservaciones"); }
        }

        private string _ActitudGeneralEntrv;

        public string ActitudGeneralEntrv
        {
            get { return _ActitudGeneralEntrv; }
            set { _ActitudGeneralEntrv = value; OnPropertyChanged("ActitudGeneralEntrv"); }
        }

        #endregion

        #region Domicilio
        private ObservableCollection<PAIS_NACIONALIDAD> listPaisDomicilio;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisDomicilio
        {
            get { return listPaisDomicilio; }
            set { listPaisDomicilio = value; OnPropertyChanged("ListPaisDomicilio"); }
        }

        private ObservableCollection<ENTIDAD> listEntidad;
        public ObservableCollection<ENTIDAD> ListEntidad
        {
            get { return listEntidad; }
            set { listEntidad = value; OnPropertyChanged("ListEntidad"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipio;
        public ObservableCollection<MUNICIPIO> ListMunicipio
        {
            get { return listMunicipio; }
            set { listMunicipio = value; OnPropertyChanged("ListMunicipio"); }
        }

        private ObservableCollection<COLONIA> listColonia;
        public ObservableCollection<COLONIA> ListColonia
        {
            get { return listColonia; }
            set { listColonia = value; OnPropertyChanged("ListColonia"); }
        }

        private PAIS_NACIONALIDAD selectedPais;
        public PAIS_NACIONALIDAD SelectedPais
        {
            get { return selectedPais; }
            set { 
                selectedPais = value;
                ListEntidad = value != null ? new ObservableCollection<ENTIDAD>(value.ENTIDAD) :  new ObservableCollection<ENTIDAD>();
                ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                SelectEntidad = -1;
                OnPropertyChanged("SelectedPais"); }
        }

        private ENTIDAD selectedEntidad;
        public ENTIDAD SelectedEntidad
        {
            get { return selectedEntidad; }
            set { selectedEntidad = value;
                ListMunicipio = value != null ?  new ObservableCollection<MUNICIPIO>(value.MUNICIPIO) : new ObservableCollection<MUNICIPIO>();
                ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                SelectMunicipio = -1;
                OnPropertyChanged("SelectedEntidad"); }
        }

        private MUNICIPIO selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return selectedMunicipio; }
            set { selectedMunicipio = value;
                ListColonia = value != null ? new ObservableCollection<COLONIA>(value.COLONIA) : new ObservableCollection<COLONIA>();
                ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                SelectColonia = -1;
                OnPropertyChanged("SelectedMunicipio"); }
        }

        private short? selectPais = Parametro.PAIS;//82;
        public short? SelectPais
        {
            get { return selectPais; }
            set
            {
                selectPais = value;
                OnPropertyValidateChanged("SelectPais");
            }
        }
       
        private short? selectEntidad = -1;
        public short? SelectEntidad
        {
            get { return selectEntidad; }
            set
            {
                selectEntidad = value;
                OnPropertyValidateChanged("SelectEntidad");
            }
        }
        
        private short? selectMunicipio = -1;
        public short? SelectMunicipio
        {
            get { return selectMunicipio; }
            set
            {
                selectMunicipio = value;
                OnPropertyValidateChanged("SelectMunicipio");
            }
        }
        
        private int? selectColonia = -1;
        public int? SelectColonia
        {
            get { return selectColonia; }
            set
            {
                selectColonia = value;
                OnPropertyValidateChanged("SelectColonia");
            }
        }
        
        private string textCalle;
        public string TextCalle
        {
            get { return textCalle; }
            set
            {
                textCalle = value;
                OnPropertyValidateChanged("TextCalle");
            }
        }
        
        private int? textNumeroExterior;
        public int? TextNumeroExterior
        {
            get { return textNumeroExterior; }
            set
            {
                textNumeroExterior = value;
                OnPropertyValidateChanged("TextNumeroExterior");
            }
        }
        
        private string textNumeroInterior;
        public string TextNumeroInterior
        {
            get { return textNumeroInterior; }
            set
            {
                textNumeroInterior = value;
                OnPropertyValidateChanged("TextNumeroInterior");
            }
        }
        
        private string textTelefono;
        public string TextTelefono
        {
            get { return textTelefono; }
            set
            {
                textTelefono = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;
                OnPropertyValidateChanged("TextTelefono");
            }
        }
        
        private int? textCodigoPostal;
        public int? TextCodigoPostal
        {
            get { return textCodigoPostal; }
            set
            {
                textCodigoPostal = value;
                OnPropertyValidateChanged("TextCodigoPostal");
            }
        }
        
        private string textDomicilioTrabajo;
        public string TextDomicilioTrabajo
        {
            get { return textDomicilioTrabajo; }
            set
            {
                textDomicilioTrabajo = value;
                OnPropertyValidateChanged("TextDomicilioTrabajo");
            }
        }
        
        private DateTime? fechaEstado = Fechas.GetFechaDateServer;
        public DateTime? FechaEstado
        {
            get { return fechaEstado; }
            set
            {
                fechaEstado = value;
                if (value != null)
                {
                    int a, m, d = 0;
                    var hoy = Fechas.GetFechaDateServer;
                    new Fechas().DiferenciaFechas(hoy, value.Value, out a, out m, out d);
                    AniosEstado = a.ToString();
                    MesesEstado = m.ToString();
                }
                else
                {
                    AniosEstado = MesesEstado = "0";
                }
                OnPropertyValidateChanged("FechaEstado");
            }
        }
        
        private string aniosEstado = "0";
        public string AniosEstado
        {
            get { return aniosEstado; }
            set
            {
                aniosEstado = value;
                OnPropertyValidateChanged("AniosEstado");
            }
        }
        
        private string mesesEstado = "0";
        public string MesesEstado
        {
            get { return mesesEstado; }
            set
            {
                mesesEstado = value;
                OnPropertyValidateChanged("MesesEstado");
            }
        }
        #endregion

        #region Nacimiento
        
        private ObservableCollection<PAIS_NACIONALIDAD> listPaisNacimiento;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisNacimiento
        {
            get { return listPaisNacimiento; }
            set { listPaisNacimiento = value; OnPropertyChanged("ListPaisNacimiento"); }
        }

        private ObservableCollection<ENTIDAD> listEntidadNacimiento;
        public ObservableCollection<ENTIDAD> ListEntidadNacimiento
        {
            get { return listEntidadNacimiento; }
            set { listEntidadNacimiento = value; OnPropertyChanged("ListEntidadNacimiento"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipioNacimiento;
        public ObservableCollection<MUNICIPIO> ListMunicipioNacimiento
        {
            get { return listMunicipioNacimiento; }
            set { listMunicipioNacimiento = value; OnPropertyChanged("ListMunicipioNacimiento"); }
        }

        private PAIS_NACIONALIDAD selectedPaisNacimiento;
        public PAIS_NACIONALIDAD SelectedPaisNacimiento
        {
            get { return selectedPaisNacimiento; }
            set { selectedPaisNacimiento = value;
                ListEntidadNacimiento = value != null ? new ObservableCollection<ENTIDAD>(value.ENTIDAD) : new ObservableCollection<ENTIDAD>();
                ListEntidadNacimiento.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                SelectEntidadNacimiento = -1;
                OnPropertyChanged("SelectedPaisNacimiento"); }
        }

        private ENTIDAD selectedEntidadNacimiento;
        public ENTIDAD SelectedEntidadNacimiento
        {
            get { return selectedEntidadNacimiento; }
            set { selectedEntidadNacimiento = value; 
                ListMunicipioNacimiento = value != null ? new ObservableCollection<MUNICIPIO>(value.MUNICIPIO) : new ObservableCollection<MUNICIPIO>();
                ListMunicipioNacimiento.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                SelectMunicipioNacimiento = -1;
                OnPropertyChanged("SelectedEntidadNacimiento"); }
        }

        private short? selectNacionalidad = Parametro.PAIS;
        public short? SelectNacionalidad
        {
            get { return selectNacionalidad; }
            set
            {
                selectNacionalidad = value;
                OnPropertyValidateChanged("SelectNacionalidad");
            }
        }

        private short? selectPaisNacimiento = Parametro.PAIS;
        public short? SelectPaisNacimiento
        {
            get { return selectPaisNacimiento; }
            set
            {
                selectPaisNacimiento = value;
                if (value == Parametro.PAIS)
                    LugarNacimientoEnabled = false;
                else
                    LugarNacimientoEnabled = true;
                OnPropertyValidateChanged("SelectPaisNacimiento");
            }
        }
        
        private short? selectEntidadNacimiento = -1;
        public short? SelectEntidadNacimiento
        {
            get { return selectEntidadNacimiento; }
            set
            {
                selectEntidadNacimiento = value;
                OnPropertyValidateChanged("SelectEntidadNacimiento");
            }
        }
        
        private short? selectMunicipioNacimiento = -1;
        public short? SelectMunicipioNacimiento
        {
            get { return selectMunicipioNacimiento; }
            set
            {
                selectMunicipioNacimiento = value;
                OnPropertyValidateChanged("SelectMunicipioNacimiento");
            }
        }

        private DateTime? textFechaNacimiento;
        public DateTime? TextFechaNacimiento
        {
            get { return textFechaNacimiento; }
            set
            {
                textFechaNacimiento = value;
                if (value != null)
                {
                    TextEdad = new Fechas().CalculaEdad(value);
                }
                else
                    TextEdad = 0;
                OnPropertyValidateChanged("TextFechaNacimiento");
            }
        }
       
        private int textEdad;
        public int TextEdad
        {
            get { return textEdad; }
            set
            {
                textEdad = value;
                OnPropertyValidateChanged("TextEdad");
            }
        }

        private string textLugarNacimientoExtranjero;
        public string TextLugarNacimientoExtranjero
        {
            get { return textLugarNacimientoExtranjero; }
            set
            {
                textLugarNacimientoExtranjero = value;
                OnPropertyValidateChanged("TextLugarNacimientoExtranjero");
            }
        }
        #endregion

        #region Configuracion Permisos
        //private bool pInsertar = false;
        //public bool PInsertar
        //{
        //    get { return pInsertar; }
        //    set
        //    {
        //        pInsertar = value;
        //        if (value)
        //            MenuGuardarEnabled = value;
        //    }
        //}

        //private bool pEditar = false;
        //public bool PEditar
        //{
        //    get { return pEditar; }
        //    set { pEditar = value; }
        //}

        //private bool pConsultar = false;
        //public bool PConsultar
        //{
        //    get { return pConsultar; }
        //    set
        //    {
        //        pConsultar = value;
        //        //if (value)
        //           // MenuBuscarEnabled = value;
        //    }
        //}

        //private bool pImprimir = false;
        //public bool PImprimir
        //{
        //    get { return pImprimir; }
        //    set
        //    {
        //        pImprimir = value;
        //        if (value)
        //            MenuReporteEnabled = value;
        //    }
        //}
        #endregion

        #region Menu
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
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

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        
        #endregion

        #region Padre
        private ObservableCollection<PAIS_NACIONALIDAD> listPaisDomicilioPadre;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisDomicilioPadre
        {
            get { return listPaisDomicilioPadre; }
            set { listPaisDomicilioPadre = value; OnPropertyChanged("ListPaisDomicilioPadre"); }
        }

        private ObservableCollection<ENTIDAD> listEntidadDomicilioPadre;
        public ObservableCollection<ENTIDAD> ListEntidadDomicilioPadre
        {
            get { return listEntidadDomicilioPadre; }
            set { listEntidadDomicilioPadre = value; OnPropertyChanged("ListEntidadDomicilioPadre"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipioDomicilioPadre;
        public ObservableCollection<MUNICIPIO> ListMunicipioDomicilioPadre
        {
            get { return listMunicipioDomicilioPadre; }
            set { listMunicipioDomicilioPadre = value; OnPropertyChanged("ListMunicipioDomicilioPadre"); }
        }

        private ObservableCollection<COLONIA> listColoniaDomicilioPadre;
        public ObservableCollection<COLONIA> ListColoniaDomicilioPadre
        {
            get { return listColoniaDomicilioPadre; }
            set { listColoniaDomicilioPadre = value; OnPropertyChanged("ListColoniaDomicilioPadre"); }
        }

        private PAIS_NACIONALIDAD selectedPaisDomicilioPadre;
        public PAIS_NACIONALIDAD SelectedPaisDomicilioPadre
        {
            get { return selectedPaisDomicilioPadre; }
            set { selectedPaisDomicilioPadre = value;
                ListEntidadDomicilioPadre = value != null ? new ObservableCollection<ENTIDAD>(value.ENTIDAD) : new ObservableCollection<ENTIDAD>();
                ListEntidadDomicilioPadre.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                SelectEntidadDomicilioPadre = -1;
                OnPropertyChanged("SelectedPaisDomicilioPadre"); }
        }
        private ENTIDAD selectedEntidadDomicilioPadre;
        public ENTIDAD SelectedEntidadDomicilioPadre
        {
            get { return selectedEntidadDomicilioPadre; }
            set { selectedEntidadDomicilioPadre = value;
                ListMunicipioDomicilioPadre = value != null ? new ObservableCollection<MUNICIPIO>(value.MUNICIPIO) : new ObservableCollection<MUNICIPIO>();
                ListMunicipioDomicilioPadre.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                SelectMunicipioDomicilioPadre = -1;
                OnPropertyChanged("SelectedEntidadDomicilioPadre"); }
        }

        private MUNICIPIO selectedMunicipioDomicilioPadre;
        public MUNICIPIO SelectedMunicipioDomicilioPadre
        {
            get { return selectedMunicipioDomicilioPadre; }
            set { selectedMunicipioDomicilioPadre = value;
                ListColoniaDomicilioPadre = value != null ? new ObservableCollection<COLONIA>(value.COLONIA) : new ObservableCollection<COLONIA>();
                ListColoniaDomicilioPadre.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                SelectColoniaDomicilioPadre = -1;
                OnPropertyChanged("SelectedMunicipioDomicilioPadre"); }
        }
        
        private bool checkPadreFinado = false;
        public bool CheckPadreFinado
        {
            get { return checkPadreFinado; }
            set { checkPadreFinado = value;
            if (value)
            {
                DomicilioPadreEnabled = MismoDomicilioMadreEnabled = MismoDomicilioMadre =  false;
            }
            else
            {
                DomicilioPadreEnabled = MismoDomicilioMadreEnabled = true; 
            }
            ValidarDatosPadre();
                OnPropertyValidateChanged("CheckPadreFinado"); }
        }

        private string textPadrePaterno;
        public string TextPadrePaterno
        {
            get { return textPadrePaterno; }
            set
            {
                textPadrePaterno = value;
                if (!string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("TextPadreMaterno");
                    OnPropertyChanged("TextPadreMaterno");
                }
                else
                {
                    if (string.IsNullOrEmpty(TextPadreMaterno))
                    {
                        base.RemoveRule("TextPadrePaterno");
                        base.AddRule(() => TextPadrePaterno, () => !string.IsNullOrEmpty(TextPadrePaterno), "APELLIDO PATERNO ES REQUERIDO!");

                        base.RemoveRule("TextPadreMaterno");
                        base.AddRule(() => TextPadreMaterno, () => !string.IsNullOrEmpty(TextPadreMaterno), "APELLIDO MATERNO ES REQUERIDO!");
                        OnPropertyChanged("TextPadreMaterno");
                    }
                    else
                        base.RemoveRule("TextPadrePaterno");
                }
                //if (!string.IsNullOrEmpty(value))
                //{
                //    base.RemoveRule("TextMadrePaterno");
                //    OnPropertyChanged("TextMadrePaterno");
                //}
                //else
                //{
                //    if (string.IsNullOrEmpty(TextMadrePaterno))
                //    {
                //        base.RemoveRule("TextPadrePaterno");
                //        base.AddRule(() => TextPadrePaterno, () => !string.IsNullOrEmpty(TextPadrePaterno), "APELLIDO PATERNO ES REQUERIDO!");

                //        base.RemoveRule("TextMadrePaterno");
                //        base.AddRule(() => TextMadrePaterno, () => !string.IsNullOrEmpty(TextMadrePaterno), "APELLIDO MATERNO ES REQUERIDO!");
                //        OnPropertyChanged("TextMadrePaterno");
                //    }
                //}
                OnPropertyValidateChanged("TextPadrePaterno");
            }
        }

        private string textMadrePaterno;
        public string TextMadrePaterno
        {
            get { return textMadrePaterno; }
            set
            {
                textMadrePaterno = value;
                if (!string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("TextMadreMaterno");
                    OnPropertyChanged("TextMadreMaterno");
                }
                else
                {
                    if (string.IsNullOrEmpty(TextMadreMaterno))
                    {
                        base.RemoveRule("TextMadrePaterno");
                        base.AddRule(() => TextMadrePaterno, () => !string.IsNullOrEmpty(TextMadrePaterno), "APELLIDO PATERNO ES REQUERIDO!");

                        base.RemoveRule("TextMadreMaterno");
                        base.AddRule(() => TextMadreMaterno, () => !string.IsNullOrEmpty(TextMadreMaterno), "APELLIDO MATERNO ES REQUERIDO!");
                        OnPropertyChanged("TextMadreMaterno");
                    }
                    else
                        base.RemoveRule("TextMadrePaterno");
                }
                //if (!string.IsNullOrEmpty(value))
                //{
                //    base.RemoveRule("TextMadrePaterno");
                //    OnPropertyChanged("TextMadrePaterno");
                //}
                //else
                //{
                //    if (string.IsNullOrEmpty(TextMadrePaterno))
                //    {
                //        base.RemoveRule("TextMadrePaterno");
                //        base.AddRule(() => TextMadrePaterno, () => !string.IsNullOrEmpty(TextMadrePaterno), "APELLIDO MATERNO ES REQUERIDO!");

                //        base.RemoveRule("TextMadrePaterno");
                //        base.AddRule(() => TextMadrePaterno, () => !string.IsNullOrEmpty(TextMadrePaterno), "APELLIDO PATERNO ES REQUERIDO!");
                //        OnPropertyChanged("TextMadrePaterno");
                //    }
                //}
                OnPropertyValidateChanged("TextMadrePaterno");
            }
        }

        private string textPadreNombre;
        public string TextPadreNombre
        {
            get { return textPadreNombre; }
            set
            {
                textPadreNombre = value;
                OnPropertyValidateChanged("TextPadreNombre");
            }
        }
     
        private short? selectPaisDomicilioPadre =Parametro.PAIS;// 82;
        public short? SelectPaisDomicilioPadre
        {
            get { return selectPaisDomicilioPadre; }
            set
            {
                selectPaisDomicilioPadre = value;
                OnPropertyValidateChanged("SelectPaisDomicilioPadre");
            }
        }
        
        private short? selectEntidadDomicilioPadre = -1;
        public short? SelectEntidadDomicilioPadre
        {
            get { return selectEntidadDomicilioPadre; }
            set
            {
                selectEntidadDomicilioPadre = value;
                OnPropertyValidateChanged("SelectEntidadDomicilioPadre");
            }
        }
        
        private short? selectMunicipioDomicilioPadre = -1;
        public short? SelectMunicipioDomicilioPadre
        {
            get { return selectMunicipioDomicilioPadre; }
            set
            {
                selectMunicipioDomicilioPadre = value;
                OnPropertyValidateChanged("SelectMunicipioDomicilioPadre");
            }
        }
        
        private int? selectColoniaDomicilioPadre = -1;
        public int? SelectColoniaDomicilioPadre
        {
            get { return selectColoniaDomicilioPadre; }
            set
            {
                selectColoniaDomicilioPadre = value;
                OnPropertyValidateChanged("SelectColoniaDomicilioPadre");
            }
        }

        private string textCalleDomicilioPadre;
        public string TextCalleDomicilioPadre
        {
            get { return textCalleDomicilioPadre; }
            set
            {
                textCalleDomicilioPadre = value;
                OnPropertyValidateChanged("TextCalleDomicilioPadre");
            }
        }
        
        private int? textNumeroExteriorDomicilioPadre;
        public int? TextNumeroExteriorDomicilioPadre
        {
            get { return textNumeroExteriorDomicilioPadre; }
            set
            {
                textNumeroExteriorDomicilioPadre = value;
                OnPropertyValidateChanged("TextNumeroExteriorDomicilioPadre");
            }
        }
        
        private string textNumeroInteriorDomicilioPadre;
        public string TextNumeroInteriorDomicilioPadre
        {
            get { return textNumeroInteriorDomicilioPadre; }
            set
            {
                textNumeroInteriorDomicilioPadre = value;
                OnPropertyValidateChanged("TextNumeroInteriorDomicilioPadre");
            }
        }
        
        private int? textCodigoPostalDomicilioPadre;
        public int? TextCodigoPostalDomicilioPadre
        {
            get { return textCodigoPostalDomicilioPadre; }
            set
            {
                textCodigoPostalDomicilioPadre = value;
                OnPropertyValidateChanged("TextCodigoPostalDomicilioPadre");
            }
        }

        private bool domicilioPadreEnabled = true;
        public bool DomicilioPadreEnabled
        {
            get { return domicilioPadreEnabled; }
            set { domicilioPadreEnabled = value; OnPropertyChanged("DomicilioPadreEnabled"); }
        }
        #endregion

        #region Madre
        private ObservableCollection<PAIS_NACIONALIDAD> listPaisDomicilioMadre;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisDomicilioMadre
        {
            get { return listPaisDomicilioMadre; }
            set { listPaisDomicilioMadre = value; OnPropertyChanged("ListPaisDomicilioMadre"); }
        }

        private ObservableCollection<ENTIDAD> listEntidadDomicilioMadre;
        public ObservableCollection<ENTIDAD> ListEntidadDomicilioMadre
        {
            get { return listEntidadDomicilioMadre; }
            set { listEntidadDomicilioMadre = value; OnPropertyChanged("ListEntidadDomicilioMadre"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipioDomicilioMadre;
        public ObservableCollection<MUNICIPIO> ListMunicipioDomicilioMadre
        {
            get { return listMunicipioDomicilioMadre; }
            set { listMunicipioDomicilioMadre = value; OnPropertyChanged("ListMunicipioDomicilioMadre"); }
        }

        private ObservableCollection<COLONIA> listColoniaDomicilioMadre;
        public ObservableCollection<COLONIA> ListColoniaDomicilioMadre
        {
            get { return listColoniaDomicilioMadre; }
            set { listColoniaDomicilioMadre = value; OnPropertyChanged("ListColoniaDomicilioMadre"); }
        }

        private PAIS_NACIONALIDAD selectedPaisDomicilioMadre;
        public PAIS_NACIONALIDAD SelectedPaisDomicilioMadre
        {
            get { return selectedPaisDomicilioMadre; }
            set
            {
                selectedPaisDomicilioMadre = value;
                ListEntidadDomicilioMadre = value != null ? new ObservableCollection<ENTIDAD>(value.ENTIDAD) : new ObservableCollection<ENTIDAD>();
                ListEntidadDomicilioMadre.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                SelectEntidadDomicilioMadre = -1;
                OnPropertyChanged("SelectedPaisDomicilioMadre");
            }
        }
        
        private ENTIDAD selectedEntidadDomicilioMadre;
        public ENTIDAD SelectedEntidadDomicilioMadre
        {
            get { return selectedEntidadDomicilioMadre; }
            set
            {
                selectedEntidadDomicilioMadre = value;
                ListMunicipioDomicilioMadre = value != null ? new ObservableCollection<MUNICIPIO>(value.MUNICIPIO) : new ObservableCollection<MUNICIPIO>();
                ListMunicipioDomicilioMadre.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                SelectMunicipioDomicilioMadre = -1;
                OnPropertyChanged("SelectedEntidadDomicilioMadre");
            }
        }

        private MUNICIPIO selectedMunicipioDomicilioMadre;
        public MUNICIPIO SelectedMunicipioDomicilioMadre
        {
            get { return selectedMunicipioDomicilioMadre; }
            set
            {
                selectedMunicipioDomicilioMadre = value;
                ListColoniaDomicilioMadre = value != null ? new ObservableCollection<COLONIA>(value.COLONIA) : new ObservableCollection<COLONIA>();
                ListColoniaDomicilioMadre.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                SelectColoniaDomicilioMadre = -1;
                OnPropertyChanged("SelectedMunicipioDomicilioMadre");
            }
        }

        private bool checkMadreFinado = false;
        public bool CheckMadreFinado
        {
            get { return checkMadreFinado; }
            set { checkMadreFinado = value;
            if (value)
            {
                MismoDomicilioMadreEnabled = MismoDomicilioMadre = DomicilioMadreEnabled = false;
            }
            else
            {
                if(!CheckPadreFinado)
                    MismoDomicilioMadreEnabled = true;
                if (!MismoDomicilioMadre)
                    DomicilioMadreEnabled = true;
            }
            //ValidarDatosMadre();
                OnPropertyValidateChanged("CheckMadreFinado"); }
        }

        private bool mismoDomicilioMadre = false;
        public bool MismoDomicilioMadre
        {
            get { return mismoDomicilioMadre; }
            set { mismoDomicilioMadre = value;
            if (value)
                DomicilioMadreEnabled = false;
            else
            {
                if (!CheckMadreFinado)
                    DomicilioMadreEnabled = true;
            }
            //ValidarDatosMadre();
                OnPropertyValidateChanged("MismoDomicilioMadre"); }
        }

        private string textPadreMaterno;
        public string TextPadreMaterno
        {
            get { return textPadreMaterno; }
            set
            {
                textPadreMaterno = value;
                if (!string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("TextPadrePaterno");
                    OnPropertyChanged("TextPadrePaterno");
                }
                else
                {
                    if (string.IsNullOrEmpty(TextPadrePaterno))
                    {
                        base.RemoveRule("TextPadreMaterno");
                        base.AddRule(() => TextPadreMaterno, () => !string.IsNullOrEmpty(TextPadreMaterno), "APELLIDO MATERNO ES REQUERIDO!");

                        base.RemoveRule("TextPadrePaterno");
                        base.AddRule(() => TextPadrePaterno, () => !string.IsNullOrEmpty(TextPadrePaterno), "APELLIDO PATERNO ES REQUERIDO!");
                        OnPropertyChanged("TextPadrePaterno");
                    }
                    else
                        base.RemoveRule("TextPadreMaterno");
                }   
                OnPropertyValidateChanged("TextPadreMaterno");
            }
        }

        private string textMadreMaterno;
        public string TextMadreMaterno
        {
            get { return textMadreMaterno; }
            set
            {
                textMadreMaterno = value;
                if (!string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("TextMadrePaterno");
                    OnPropertyChanged("TextMadrePaterno");
                }
                else
                {
                    if (string.IsNullOrEmpty(TextMadrePaterno))
                    {
                        base.RemoveRule("TextMadreMaterno");
                        base.AddRule(() => TextMadreMaterno, () => !string.IsNullOrEmpty(TextMadreMaterno), "APELLIDO MATERNO ES REQUERIDO!");

                        base.RemoveRule("TextMadrePaterno");
                        base.AddRule(() => TextMadrePaterno, () => !string.IsNullOrEmpty(TextMadrePaterno), "APELLIDO PATERNO ES REQUERIDO!");
                        OnPropertyChanged("TextMadrePaterno");
                    }
                    else
                        base.RemoveRule("TextMadreMaterno");
                }   
                OnPropertyChanged("TextMadreMaterno");
            }
        }

        private string textMadreNombre;
        public string TextMadreNombre
        {
            get { return textMadreNombre; }
            set
            {
                textMadreNombre = value;
                OnPropertyValidateChanged("TextMadreNombre");
            }
        }

        private short? selectPaisDomicilioMadre = Parametro.PAIS;//82;
        public short? SelectPaisDomicilioMadre
        {
            get { return selectPaisDomicilioMadre; }
            set
            {
                selectPaisDomicilioMadre = value;
                OnPropertyValidateChanged("SelectPaisDomicilioMadre");
            }
        }
        
        private short? selectEntidadDomicilioMadre = -1;
        public short? SelectEntidadDomicilioMadre
        {
            get { return selectEntidadDomicilioMadre; }
            set
            {
                selectEntidadDomicilioMadre = value;
                OnPropertyValidateChanged("SelectEntidadDomicilioMadre");
                if (!finLoad)
                {
                    StaticSourcesViewModel.SourceChanged = false;
                    finLoad = true;
                }
            }
        }
        
        private short? selectMunicipioDomicilioMadre = -1;
        public short? SelectMunicipioDomicilioMadre
        {
            get { return selectMunicipioDomicilioMadre; }
            set
            {
                selectMunicipioDomicilioMadre = value;
                OnPropertyValidateChanged("SelectMunicipioDomicilioMadre");
            }
        }
        
        private int? selectColoniaDomicilioMadre = -1;
        public int? SelectColoniaDomicilioMadre
        {
            get { return selectColoniaDomicilioMadre; }
            set
            {
                selectColoniaDomicilioMadre = value;
                OnPropertyValidateChanged("SelectColoniaDomicilioMadre");
            }
        }

        private string textCalleDomicilioMadre;
        public string TextCalleDomicilioMadre
        {
            get { return textCalleDomicilioMadre; }
            set
            {
                textCalleDomicilioMadre = value;
                OnPropertyValidateChanged("TextCalleDomicilioMadre");
            }
        }
        private int? textNumeroExteriorDomicilioMadre;
        public int? TextNumeroExteriorDomicilioMadre
        {
            get { return textNumeroExteriorDomicilioMadre; }
            set
            {
                textNumeroExteriorDomicilioMadre = value;
                OnPropertyValidateChanged("TextNumeroExteriorDomicilioMadre");
            }
        }
        private string textNumeroInteriorDomicilioMadre;
        public string TextNumeroInteriorDomicilioMadre
        {
            get { return textNumeroInteriorDomicilioMadre; }
            set
            {
                textNumeroInteriorDomicilioMadre = value;
                OnPropertyValidateChanged("TextNumeroInteriorDomicilioMadre");
            }
        }
        private int? textCodigoPostalDomicilioMadre;
        public int? TextCodigoPostalDomicilioMadre
        {
            get { return textCodigoPostalDomicilioMadre; }
            set
            {
                textCodigoPostalDomicilioMadre = value;
                OnPropertyValidateChanged("TextCodigoPostalDomicilioMadre");
            }
        }

        private bool mismoDomicilioMadreEnabled = true;
        public bool MismoDomicilioMadreEnabled
        {
            get { return mismoDomicilioMadreEnabled; }
            set { mismoDomicilioMadreEnabled = value; OnPropertyChanged("MismoDomicilioMadreEnabled"); }
        }

        private bool domicilioMadreEnabled = true;
        public bool DomicilioMadreEnabled
        {
            get { return domicilioMadreEnabled; }
            set { domicilioMadreEnabled = value;
                ValidarDatosMadre();
                OnPropertyChanged("DomicilioMadreEnabled"); }
        }

        #endregion

        #region Alias
        private ObservableCollection<ALIAS> listAlias;
        public ObservableCollection<ALIAS> ListAlias
        {
            get { return listAlias; }
            set { listAlias = value; OnPropertyValidateChanged("ListAlias"); }
        }

        private ALIAS selectAlias;
        public ALIAS SelectAlias
        {
            get { return selectAlias; }
            set { selectAlias = value; OnPropertyChanged("SelectAlias"); }
        }

        private string paternoAlias;
        public string PaternoAlias
        {
            get { return paternoAlias; }
            set { paternoAlias = value;
            if (!string.IsNullOrEmpty(value))
            {
                base.RemoveRule("MaternoAlias");
                OnPropertyChanged("MaternoAlias");
            }
            else
            {
                if (string.IsNullOrEmpty(MaternoAlias))
                {
                    base.RemoveRule("PaternoAlias");
                    base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ES REQUERIDO!");

                    base.RemoveRule("MaternoAlias");
                    base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ES REQUERIDO!");
                    OnPropertyChanged("MaternoAlias");
                }
            }
                OnPropertyChanged("PaternoAlias"); }
        }

        private string maternoAlias;
        public string MaternoAlias
        {
            get { return maternoAlias; }
            set { maternoAlias = value;
            if (!string.IsNullOrEmpty(value))
            {
                base.RemoveRule("PaternoAlias");
                OnPropertyChanged("PaternoAlias");
            }
            else
            {
                if (string.IsNullOrEmpty(PaternoAlias))
                {
                    base.RemoveRule("MaternoAlias");
                    base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ES REQUERIDO!");

                    base.RemoveRule("PaternoAlias");
                    base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ES REQUERIDO!");
                    OnPropertyChanged("PaternoAlias");
                }
            }   
            OnPropertyChanged("MaternoAlias"); 
            }
        }

        private string nombreAlias;
        public string NombreAlias
        {
            get { return nombreAlias; }
            set { nombreAlias = value; OnPropertyChanged("NombreAlias"); }
        }
        #endregion

        #region Apodo
        private ObservableCollection<APODO> listApodo;
        public ObservableCollection<APODO> ListApodo
        {
            get { return listApodo; }
            set { listApodo = value; OnPropertyValidateChanged("ListApodo"); }
        }

        private APODO selectApodo;
        public APODO SelectApodo
        {
            get { return selectApodo; }
            set { selectApodo = value; OnPropertyChanged("SelectApodo"); }
        }

        private string apodo;
        public string Apodo
        {
            get { return apodo; }
            set { apodo = value; OnPropertyChanged("Apodo"); }
        }
        #endregion

        #region Buscar
        private string textBotonSeleccionarIngreso = "Seleccionar Medida Judicial";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }
        
        private bool crearNuevoExpedienteEnabled = true;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
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

        private short? anioBuscar;
        public short? AnioBuscar
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

        private string _NUCBuscar;
        public string NUCBuscar
        {
            get { return _NUCBuscar; }
            set { _NUCBuscar = value; OnPropertyChanged("NUCBuscar"); }
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

        //private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        //public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        //{
        //    get { return listExpediente; }
        //    set
        //    {
        //        listExpediente = value;
        //        OnPropertyChanged("ListExpediente");
        //    }
        //}

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (value != null)
                {
                    if (value.IMPUTADO_BIOMETRICO != null)
                    {
                        var foto = value.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                        if (foto != null)
                        {
                            if (foto.BIOMETRICO != null)
                                ImagenInterno = foto.BIOMETRICO;
                            else
                                ImagenInterno = new Imagenes().getImagenPerson();
                        }
                        else
                            ImagenInterno = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenInterno = new Imagenes().getImagenPerson();
                    NuevoProcesoEnabled = true;
                    SelectedProcesoLibertad = null;
                    SeleccionarProcesoEnabled = false;
                    
                    //Nombre 
                    AnioBuscar = value.ID_ANIO;
                    FolioBuscar = value.ID_IMPUTADO;
                    ApellidoPaternoBuscar = value.PATERNO;
                    ApellidoMaternoBuscar = value.MATERNO;
                    NombreBuscar = value.NOMBRE; 
                    
                    if (value.PROCESO_LIBERTAD != null)
                    {
                        if (value.PROCESO_LIBERTAD.Count == 0)
                        {
                            EmptyProceso = Visibility.Visible;
                        }
                        else
                            EmptyProceso = Visibility.Collapsed;
                    }
                    else
                        EmptyProceso = Visibility.Visible;
                }
                else
                {
                    NuevoProcesoEnabled = SeleccionarProcesoEnabled = false;
                    SelectedProcesoLibertad = null;
                    EmptyProceso = Visibility.Visible;
                    ImagenInterno = new Imagenes().getImagenPerson();
                }
                #region Comentado
                //NuevaMJEnabled = value != null ? true : false;
                //if (value != null)
                //{
                //    var foto = value.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                //    if (foto != null)
                //        ImagenInterno = foto.BIOMETRICO;
                //    else
                //    {
                //        if (value.INGRESO != null)
                //        {
                //            var ingreso = value.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                //            if (ingreso != null)
                //            {
                //                var fotoIngreso = ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                //                if(fotoIngreso != null)
                //                    ImagenInterno = fotoIngreso.BIOMETRICO;
                //                else
                //                    ImagenInterno = new Imagenes().getImagenPerson();
                //            }
                //            else
                //                ImagenInterno = new Imagenes().getImagenPerson();
                //        }
                //    }
                //    var l = new cLiberado().Obtener(value.ID_CENTRO, value.ID_ANIO, value.ID_IMPUTADO);
                //    if (l != null)
                //    {
                //        LstLiberadoMJ = new ObservableCollection<LIBERADO_MEDIDA_JUDICIAL>(l.LIBERADO_MEDIDA_JUDICIAL);
                //    }
                //    else
                //        LstLiberadoMJ = null;
                //}
                //else
                //{ 
                //    LstLiberadoMJ = null;
                //    ImagenInterno = new Imagenes().getImagenPerson();
                //}
                //SelectMJ = null;
                //EmptyMJVisible = LstLiberadoMJ != null ? LstLiberadoMJ.Count > 0 ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
                #endregion
                OnPropertyChanged("SelectExpediente");
            }
        }

        private PROCESO_LIBERTAD selectProceso;
        public PROCESO_LIBERTAD SelectProceso
        {
            get { return selectProceso; }
            set { selectProceso = value;
            if (value != null)
                SeleccionarProcesoEnabled = true;
            else
                SeleccionarProcesoEnabled = false;

                OnPropertyChanged("SelectProceso"); }
        }

        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set { aceptarBusquedaHuellaFocus = value; OnPropertyChanged("AceptarBusquedaHuellaFocus"); }
        }

        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                    return;

                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    OnPropertyChanged("SelectIngreso");
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();

                OnPropertyChanged("SelectIngreso");
            }
        }

        private LIBERADO_MEDIDA_JUDICIAL selectMJ;
        public LIBERADO_MEDIDA_JUDICIAL SelectMJ
        {
            get { return selectMJ; }
            set { selectMJ = value;
                //SelectMJEnabled = value != null ? true : false;
                OnPropertyChanged("SelectMJ"); }
        }

        private Visibility emptyProceso = Visibility.Collapsed;
        public Visibility EmptyProceso
        {
            get { return emptyProceso; }
            set { emptyProceso = value; OnPropertyChanged("EmptyProceso"); }
        }

        private ObservableCollection<LIBERADO_MEDIDA_JUDICIAL> lstLiberadoMJ;
        public ObservableCollection<LIBERADO_MEDIDA_JUDICIAL> LstLiberadoMJ
        {
            get { return lstLiberadoMJ; }
            set { lstLiberadoMJ = value; OnPropertyChanged("LstLiberadoMJ"); }
        }

        private byte[] imagenInterno = new Imagenes().getImagenPerson();
        public byte[] ImagenInterno
        {
            get { return imagenInterno; }
            set { imagenInterno = value; OnPropertyChanged("ImagenInterno"); }
        }

        private bool seleccionarProcesoEnabled = false;
        public bool SeleccionarProcesoEnabled
        {
            get { return seleccionarProcesoEnabled; }
            set { seleccionarProcesoEnabled = value; OnPropertyChanged("SeleccionarProcesoEnabled"); }
        }

        private bool nuevoProcesoEnabled = false;
        public bool NuevoProcesoEnabled
        {
            get { return nuevoProcesoEnabled; }
            set { nuevoProcesoEnabled = value; OnPropertyChanged("NuevoProcesoEnabled"); }
        }

        //private byte[] fotoImputado = new Imagenes().getImagenPerson();
        //public byte[] FotoImputado
        //{
        //    get { return fotoImputado; }
        //    set { fotoImputado = value; OnPropertyChanged("FotoImputado"); }
        //}
        #endregion

        #region Liberado
        private string lLugarOcupacion;
        public string LLugarOcupacion
        {
            get { return lLugarOcupacion; }
            set { lLugarOcupacion = value; OnPropertyValidateChanged("LLugarOcupacion"); }
        }
        
        private string lLugarFrecuenta;
        public string LLugarFrecuenta
        {
            get { return lLugarFrecuenta; }
            set { lLugarFrecuenta = value; OnPropertyValidateChanged("LLugarFrecuenta"); }
        }
        #endregion

        #region Camara
        private WebCam CamaraWeb;

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
        #endregion

        #region Huellas
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

        #region Interconexion
        private VM_IMPUTADOSDATOS selectedInterconexion;
        public VM_IMPUTADOSDATOS SelectedInterconexion
        {
            get { return selectedInterconexion; }
            set { selectedInterconexion = value;
            if (value != null)
            {
                PNUC = value.EXPEDIENTEID.ToString();
            }
            else
                PNUC = string.Empty;
                OnPropertyChanged("PNUC");
                OnPropertyChanged("SelectedInterconexion");
            }
        }
        private ObservableCollection<IMPUTADO_FILIACION> lstImputadoFiliacion;
        public ObservableCollection<IMPUTADO_FILIACION> LstImputadoFiliacion
        {
            get { return lstImputadoFiliacion; }
            set { lstImputadoFiliacion = value; }
        }
        #endregion
     
        #region Generales
        private string tituloTop = "Registro de Liberados";
        public string TituloTop
        {
            get { return tituloTop; }
            set { tituloTop = value; OnPropertyChanged("TituloTop"); }
        }
        
        private bool tabFotosHuellas;
        public bool TabFotosHuellas
        {
            get { return tabFotosHuellas; }
            set
            {
                tabFotosHuellas = value;
                OnPropertyChanged("TabFotosHuellas");
            }
        }
        
        private bool camposBusquedaEnabled = false;
        public bool CamposBusquedaEnabled
        {
            get { return camposBusquedaEnabled; }
            set { camposBusquedaEnabled = value; OnPropertyChanged("CamposBusquedaEnabled"); }
        }
        
        private bool tabsEnabled = false;
        public bool TabsEnabled
        {
            get { return tabsEnabled; }
            set { tabsEnabled = value; OnPropertyChanged("TabsEnabled"); }
        }
        
        private Visibility relacionesPersonalesVisible = Visibility.Collapsed;
        public Visibility RelacionesPersonalesVisible
        {
            get { return relacionesPersonalesVisible; }
            set { relacionesPersonalesVisible = value; OnPropertyChanged("RelacionesPersonalesVisible"); }
        }

        private bool Changed = false;

        private bool bHuellasEnabled = false;
        public bool BHuellasEnabled
        {
            get { return bHuellasEnabled; }
            set { bHuellasEnabled = value; OnPropertyChanged("BHuellasEnabled"); }
        }

        private bool interconexionEnabled = false;
        public bool InterconexionEnabled
        {
            get { return interconexionEnabled; }
            set { interconexionEnabled = value; OnPropertyChanged("InterconexionEnabled"); }
        }
        
        private bool finLoad = false;

        private bool lugarNacimientoEnabled = false;
        public bool LugarNacimientoEnabled
        {
            get { return lugarNacimientoEnabled; }
            set { lugarNacimientoEnabled = value; OnPropertyChanged("LugarNacimientoEnabled"); }
        }

        private bool defensorEnabled = false;
        public bool DefensorEnabled
        {
            get { return defensorEnabled; }
            set { defensorEnabled = value; OnPropertyChanged("DefensorEnabled"); }
        }

        private bool tabIdentificacion = false;
        public bool TabIdentificacion
        {
            get { return tabIdentificacion; }
            set { 
                tabIdentificacion = value;
                if (value)
                {
                    ValidacionesLiberado();
                    MenuGuardarEnabled = true;
                }
                else
                { 
                    base.ClearRules();
                    MenuGuardarEnabled = false;
                }
                OnPropertyChanged("TabIdentificacion"); }
        }

        private bool tabObservacion = false;
        public bool TabObservacion
        {
            get { return tabObservacion; }
            set { tabObservacion = value; OnPropertyChanged("TabObservacion"); }
        }
        #endregion

        #region Proceso Libertad
        private string pNUC;
        public string PNUC
        {

            get { return pNUC; }
            set { pNUC = value; OnPropertyChanged("PNUC"); }
        }
        
        private DateTime? pFecha;
        public DateTime? PFecha
        {
            get { return pFecha; }
            set { pFecha = value; OnPropertyChanged("PFecha"); }
        }

        private short? pCPAnio;
        public short? PCPAnio
        {
            get { return pCPAnio; }
            set { pCPAnio = value; OnPropertyChanged("PCPAnio"); }
        }
        
        private int? pCPFolio;
        public int? PCPFolio
        {
            get { return pCPFolio; }
            set { pCPFolio = value; OnPropertyChanged("PCPFolio"); }
        }

        private short? pTipo =-1;
        public short? PTipo
        {
            get { return pTipo; }
            set {
                var anterior = pTipo;
                pTipo = value;
            if (pTipo != -1)
            {
                if (value == 2 && anterior != -1)
                {
                    ActualizarEstatus();
                }
                else
                    TipoEnabled = true;
            }
                OnPropertyChanged("PTipo"); }
        }

      
        private bool tipoEnabled = true;
        public bool TipoEnabled
        {
            get { return tipoEnabled; }
            set { tipoEnabled = value; OnPropertyChanged("TipoEnabled"); }
        }


        private decimal? pEscalaRiesgo;
        public decimal? PEscalaRiesgo
        {
            get { return pEscalaRiesgo; }
            set { pEscalaRiesgo = value; OnPropertyChanged("PEscalaRiesgo"); }
        }

        private ObservableCollection<PROCESO_LIBERTAD> lstProcesoLibertad;
        public ObservableCollection<PROCESO_LIBERTAD> LstProcesoLibertad
        {
            get { return lstProcesoLibertad; }
            set { lstProcesoLibertad = value;
            if (value != null)
            {
                LstProcesoIsEmpty = value.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                LstProcesoIsEmpty = Visibility.Collapsed;
            }
            OnPropertyChanged("LstProcesoLibertad"); }
        }

        private PROCESO_LIBERTAD selectedProcesoLibertad;
        public PROCESO_LIBERTAD SelectedProcesoLibertad
        {
            get { return selectedProcesoLibertad; }
            set { selectedProcesoLibertad = value;
            if (value != null)
            {
                TabIdentificacionEnabled = TabMedidaEnabled = true;
                SeleccionarProcesoEnabled = true;
                //if (LstProcesoLibertad.Count == 0)
                //    LstProcesoLibertad = new ObservableCollection<PROCESO_LIBERTAD>(SelectExpediente.PROCESO_LIBERTAD);
                //LstMedidaDocumento = new ObservableCollection<MEDIDA_DOCUMENTO>(value.MEDIDA_DOCUMENTO);
                //LstMedidaDocumentoCB = new ObservableCollection<MEDIDA_DOCUMENTO>(LstMedidaDocumento);
                //LstMedidaLibertad = new ObservableCollection<MEDIDA_LIBERTAD>(value.MEDIDA_LIBERTAD);
                //SeleccionarProcesoEnabled = true;
                //Obtener();
            }
            else
            {
                TabIdentificacionEnabled = TabMedidaEnabled = false;
                LstProcesoLibertad = new ObservableCollection<PROCESO_LIBERTAD>();
                LstMedidaDocumento = new ObservableCollection<MEDIDA_DOCUMENTO>();
                LstMedidaDocumentoCB = new ObservableCollection<MEDIDA_DOCUMENTO>();
                LstMedidaLibertad = new ObservableCollection<MEDIDA_LIBERTAD>();
                SeleccionarProcesoEnabled = false; 
            }
            LstMedidaDocumentoCB.Insert(0, new MEDIDA_DOCUMENTO() { ID_DOCUMENTO = -1, TITULO = "SELECCIONE" });
            SelectedMedidaDocumento = null;

                OnPropertyChanged("SelectedProcesoLibertad"); }
        }

        private PROCESO_LIBERTAD selectedProcesoLibertadListado;
        public PROCESO_LIBERTAD SelectedProcesoLibertadListado
        {
            get { return selectedProcesoLibertadListado; }
            set
            {
                selectedProcesoLibertadListado =  value;
                OnPropertyChanged("SelectedProcesoLibertadListado");
            }
        }

        private Visibility lstProcesoIsEmpty = Visibility.Visible;
        public Visibility LstProcesoIsEmpty
        {
            get { return lstProcesoIsEmpty; }
            set { lstProcesoIsEmpty = value; OnPropertyChanged("LstProcesoIsEmpty"); }
        }
        #endregion

        #region Medida
        private ObservableCollection<MEDIDA_TIPO> lstMedidaTipo;
        public ObservableCollection<MEDIDA_TIPO> LstMedidaTipo
        {
            get { return lstMedidaTipo; }
            set { lstMedidaTipo = value; OnPropertyChanged("LstMedidaTipo"); }
        }

        private short? mDocumento = -1;
        public short? MDocumento
        {
            get { return mDocumento; }
            set { mDocumento = value; OnPropertyChanged("MDocumento"); }
        } 
        
        private MEDIDA_TIPO selectedMedidaTipo;
        public MEDIDA_TIPO SelectedMedidaTipo
        {
            get { return selectedMedidaTipo; }
            set { selectedMedidaTipo = value;
            if (value != null)
            {
                if (value.ID_TIPO_MEDIDA != -1)
                {
                    LstMedida = new ObservableCollection<MEDIDA>(value.MEDIDA);
                    LstMedida.Insert(0, new MEDIDA() { ID_MEDIDA = -1, DESCR = "SELECCIONE" });
                    MMedida = -1;
                }
                else
                {
                    LstMedida = new ObservableCollection<MEDIDA>();
                    LstMedida.Insert(0, new MEDIDA() { ID_MEDIDA = -1, DESCR = "SELECCIONE" });
                    MMedida = -1;
                }
            }
            else
            {
                LstMedida = new ObservableCollection<MEDIDA>();
                LstMedida.Insert(0, new MEDIDA() { ID_MEDIDA = -1, DESCR = "SELECCIONE" });
                MMedida = -1;
            }
                OnPropertyChanged("SelectedMedidaTipo"); }
        }

        private short? mMDocumento = -1;
        public short? MMDocumento
        {
            get { return mMDocumento; }
            set { mMDocumento = value; OnPropertyChanged("MMDocumento"); }
        }

        private short mMedidaTipo = -1;
        public short MMedidaTipo
        {
            get { return mMedidaTipo; }
            set { mMedidaTipo = value; OnPropertyChanged("MMedidaTipo"); }
        }

        private ObservableCollection<MEDIDA> lstMedida;
        public ObservableCollection<MEDIDA> LstMedida
        {
            get { return lstMedida; }
            set { lstMedida = value; OnPropertyChanged("LstMedida"); }
        }

        private short mMedida = -1;
        public short MMedida
        {
            get { return mMedida; }
            set { mMedida = value; OnPropertyChanged("MMedida"); }

        }

        private DateTime? mMFechaInicio = null;
        public DateTime? MFechaInicio
        {
            get { return mMFechaInicio; }
            set { mMFechaInicio = value; OnPropertyChanged("MFechaInicio"); }
        }

        private DateTime? mFechaFin = null;
        public DateTime? MFechaFin
        {
            get { return mFechaFin; }
            set { mFechaFin = value; OnPropertyChanged("MFechaFin"); }
        }

        private string mObservacion;
        public string MObservacion
        {
            get { return mObservacion; }
            set { mObservacion = value; OnPropertyChanged("MObservacion"); }
        }

        private ObservableCollection<MEDIDA_LIBERTAD> lstMedidaLibertad;
        public ObservableCollection<MEDIDA_LIBERTAD> LstMedidaLibertad
        {
            get { return lstMedidaLibertad; }
            set { lstMedidaLibertad = value; OnPropertyChanged("LstMedidaLibertad"); }
        }
        
        private MEDIDA_LIBERTAD selectedMedidaLibertad;
        public MEDIDA_LIBERTAD SelectedMedidaLibertad
        {
            get { return selectedMedidaLibertad; }
            set { selectedMedidaLibertad = value;
            if (value != null)
            {
                BotonesEnabled = true;
                if (value.MEDIDA_LIBERTAD_ESTATUS != null)
                {
                    if (value.MEDIDA_LIBERTAD_ESTATUS.Count > 0)
                        EstatusMedidaEmpty = Visibility.Collapsed;
                    else
                        EstatusMedidaEmpty = Visibility.Visible; 
                }
                else
                    EstatusMedidaEmpty = Visibility.Visible;

                if (value.MEDIDA_PERSONA != null)
                {
                    if (value.MEDIDA_PERSONA.Count > 0)
                        PersonasMedidaEmpty = Visibility.Collapsed;
                    else
                        PersonasMedidaEmpty = Visibility.Visible;
                }
                else
                    PersonasMedidaEmpty = Visibility.Visible;

                if (value.MEDIDA_PRESENTACION != null)
                {
                    if (value.MEDIDA_PRESENTACION.MEDIDA_PRESENTACION_DETALLE != null)
                    {
                        if (value.MEDIDA_PRESENTACION.MEDIDA_PRESENTACION_DETALLE.Count > 0)
                        {
                            PresentacionMedidaEmpty = Visibility.Collapsed;
                        }
                        else
                        {
                            PresentacionMedidaEmpty = Visibility.Visible;
                        }
                    }
                    else
                        PresentacionMedidaEmpty = Visibility.Visible;
                }
                else
                    PresentacionMedidaEmpty = Visibility.Visible;
            }
            else
            {
                EstatusMedidaEmpty = PersonasMedidaEmpty = PresentacionMedidaEmpty = Visibility.Visible;
                BotonesEnabled = false;
            }
                OnPropertyChanged("SelectedMedidaLibertad"); }
        }
        #endregion

        #region Medida Status
        private ObservableCollection<MEDIDA_ESTATUS> lstMedidaEstatus;
        public ObservableCollection<MEDIDA_ESTATUS> LstMedidaEstatus
        {
            get { return lstMedidaEstatus; }
            set { lstMedidaEstatus = value; OnPropertyChanged("LstMedidaEstatus"); }
        }

        private MEDIDA_ESTATUS selectedMedidaEstatus;
        public MEDIDA_ESTATUS SelectedMedidaEstatus
        {
            get { return selectedMedidaEstatus; }
            set { selectedMedidaEstatus = value;
            if (value != null)
            {
                if (value.ID_ESTATUS != -1)
                {
                    LstMotivo = new ObservableCollection<MEDIDA_MOTIVO>(value.MEDIDA_MOTIVO);
                    LstMotivo.Insert(0, new MEDIDA_MOTIVO() { ID_MOTIVO = -1, DESCR = "SELECCIONE" });
                    MLMotivo = -1;
                }
                else
                {
                    LstMotivo = new ObservableCollection<MEDIDA_MOTIVO>();
                    LstMotivo.Insert(0, new MEDIDA_MOTIVO() { ID_MOTIVO = -1, DESCR = "SELECCIONE" });
                    MLMotivo = -1;
                }
            }
            else
            {
                LstMotivo = new ObservableCollection<MEDIDA_MOTIVO>();
                LstMotivo.Insert(0, new MEDIDA_MOTIVO() { ID_MOTIVO = -1, DESCR = "SELECCIONE" });
                MLMotivo = -1;
            }
                OnPropertyChanged("SelectedMedidaEstatus"); }
        }

        private short mLEstatus = -1;
        public short MLEstatus
        {
            get { return mLEstatus; }
            set { mLEstatus = value; OnPropertyChanged("MLEstatus"); }
        }

        private ObservableCollection<MEDIDA_MOTIVO> lstMotivo;
        public ObservableCollection<MEDIDA_MOTIVO> LstMotivo
        {
            get { return lstMotivo; }
            set { lstMotivo = value; OnPropertyChanged("LstMotivo"); }
        }

        private short mLMotivo = -1;
        public short MLMotivo
        {
            get { return mLMotivo; }
            set { mLMotivo = value; OnPropertyChanged("MLMotivo"); }
        }
        

        private DateTime? mLFecha = null;
        public DateTime? MLFecha
        {
            get { return mLFecha; }
            set { mLFecha = value; OnPropertyChanged("MLFecha"); }
        }

        private string mLComentario;
        public string MLComentario
        {
            get { return mLComentario; }
            set { mLComentario = value; OnPropertyChanged("MLComentario"); }
        }

        private ObservableCollection<MEDIDA_LIBERTAD_ESTATUS> lstMedidaLibertadEstatus;
        public ObservableCollection<MEDIDA_LIBERTAD_ESTATUS> LstMedidaLibertadEstatus
        {
            get { return lstMedidaLibertadEstatus; }
            set { lstMedidaLibertadEstatus = value; OnPropertyChanged("LstMedidaLibertadEstatus"); }
        }

        private MEDIDA_LIBERTAD_ESTATUS selectedMedidaLibertadEstatus;
        public MEDIDA_LIBERTAD_ESTATUS SelectedMedidaLibertadEstatus
        {
            get { return selectedMedidaLibertadEstatus; }
            set { selectedMedidaLibertadEstatus = value; OnPropertyChanged("SelectedMedidaLibertadEstatus"); }
        }

        private Visibility estatusMedidaEmpty = Visibility.Visible;
        public Visibility EstatusMedidaEmpty
        {
            get { return estatusMedidaEmpty; }
            set { estatusMedidaEmpty = value; OnPropertyChanged("EstatusMedidaEmpty"); }
        }
        #endregion

        #region Medida Persona
        private string mPNombre;
        public string MPNombre
        {
            get { return mPNombre; }
            set { mPNombre = value; OnPropertyChanged("MPNombre"); }
        }

        private string mPPaterno;
        public string MPPaterno
        {
            get { return mPPaterno; }
            set { mPPaterno = value;
            if (!string.IsNullOrEmpty(value))
            {
                base.RemoveRule("MPMaterno");
                OnPropertyChanged("MPMaterno");
            }
            else
            {
                if (string.IsNullOrEmpty(MPMaterno))
                {
                    base.RemoveRule("MPPaterno");
                    base.AddRule(() => MPPaterno, () => !string.IsNullOrEmpty(MPPaterno), "APELLIDO PATERNO ES REQUERIDO!");

                    base.RemoveRule("MPMaterno");
                    base.AddRule(() => MPMaterno, () => !string.IsNullOrEmpty(MPMaterno), "APELLIDO MATERNO ES REQUERIDO!");
                    OnPropertyChanged("MPMaterno");
                }
            }
                OnPropertyChanged("MPPaterno"); }
        }

        private string mPMaterno;
        public string MPMaterno
        {
            get { return mPMaterno; }
            set { mPMaterno = value;

            if (!string.IsNullOrEmpty(value))
            {
                base.RemoveRule("MPPaterno");
                OnPropertyChanged("MPPaterno");
            }
            else
            {
                if (string.IsNullOrEmpty(MPPaterno))
                {
                    base.RemoveRule("MPMaterno");
                    base.AddRule(() => MPMaterno, () => !string.IsNullOrEmpty(MPMaterno), "APELLIDO MATERNO ES REQUERIDO!");

                    base.RemoveRule("MPPaterno");
                    base.AddRule(() => MPPaterno, () => !string.IsNullOrEmpty(MPPaterno), "APELLIDO PATERNO ES REQUERIDO!");
                    OnPropertyChanged("MPPaterno");
                }
            }   
                OnPropertyChanged("MPMaterno"); }
        }

        private string mPAlias;
        public string MPAlias
        {
            get { return mPAlias; }
            set { mPAlias = value; OnPropertyChanged("MPAlias"); }
        }

        private short mPRelacion;
        public short MPRelacion
        {
            get { return mPRelacion; }
            set { mPRelacion = value; OnPropertyChanged("MPRelacion"); }
        }

        private ObservableCollection<PARTICULARIDAD> lstParticularidad;
        public ObservableCollection<PARTICULARIDAD> LstParticularidad
        {
            get { return lstParticularidad; }
            set { lstParticularidad = value; OnPropertyChanged("LstParticularidad"); }
        }

        private short mPParticularidad = -1;
        public short MPParticularidad
        {
            get { return mPParticularidad; }
            set { 
                mPParticularidad = value; 
                OnPropertyChanged("MPParticularidad"); }
        }

        private ObservableCollection<MEDIDA_PERSONA> lstMedidaPersona;
        public ObservableCollection<MEDIDA_PERSONA> LstMedidaPersona
        {
            get { return lstMedidaPersona; }
            set { lstMedidaPersona = value; OnPropertyChanged("LstMedidaPersona"); }
        }

        private MEDIDA_PERSONA selectedMedidaPersona;
        public MEDIDA_PERSONA SelectedMedidaPersona
        {
            get { return selectedMedidaPersona; }
            set { selectedMedidaPersona = value; OnPropertyChanged("SelectedMedidaPersona"); }
        }


        private Visibility personasMedidaEmpty = Visibility.Visible;
        public Visibility PersonasMedidaEmpty
        {
            get { return personasMedidaEmpty; }
            set { personasMedidaEmpty = value; OnPropertyChanged("PersonasMedidaEmpty"); }
        }


        #endregion

        #region Medida Lugar
        private string mLCalle;
        public string MLCalle
        {
            get { return mLCalle; }
            set { mLCalle = value; OnPropertyChanged("MLCalle"); }
        }

        private short? mLNoExterior;
        public short? MLNoExterior
        {
            get { return mLNoExterior; }
            set { mLNoExterior = value; OnPropertyChanged("MLNoExterior"); }
        }

        private string mLNoInterior;
        public string MLNoInterior
        {
            get { return mLNoInterior; }
            set { mLNoInterior = value; OnPropertyChanged("MLNoInterior"); }
        }

        private string mLTelefono;
        public string MLTelefono
        {
            get { return mLTelefono; }
            set
            {
                mLTelefono = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;
                OnPropertyChanged("MLTelefono"); }
        }

        private ObservableCollection<GIRO> lstGiro;
        public ObservableCollection<GIRO> LstGiro
        {
            get { return lstGiro; }
            set { lstGiro = value; OnPropertyChanged("LstGiro"); }
        }

        private short? mLGiro;
        public short? MLGiro
        {
            get { return mLGiro; }
            set { mLGiro = value; OnPropertyChanged("MLGiro"); }
        }

        private ObservableCollection<ENTIDAD> lstEntidadML;
        public ObservableCollection<ENTIDAD> LstEntidadML
        {
            get { return lstEntidadML; }
            set { lstEntidadML = value; OnPropertyChanged("LstEntidadML"); }
        }

        private ENTIDAD selectedEntidadML;
        public ENTIDAD SelectedEntidadML
        {
            get { return selectedEntidadML; }
            set { selectedEntidadML = value;
            if (value != null)
            {
                if (value.ID_ENTIDAD != -1)
                {
                    LstMunicipioML = new ObservableCollection<MUNICIPIO>(value.MUNICIPIO);
                    LstMunicipioML.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    MLMunicipio = -1;
                }
                else
                {
                    LstMunicipioML = new ObservableCollection<MUNICIPIO>();
                    LstMunicipioML.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    MLMunicipio = -1;
                }
            }
            else
            {
                LstMunicipioML = new ObservableCollection<MUNICIPIO>();
                LstMunicipioML.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                MLMunicipio = -1;
            }
                OnPropertyChanged("SelectedEntidadML"); }
        }

        private short? mLEntidad = Parametro.ESTADO;
        public short? MLEntidad
        {
            get { return mLEntidad; }
            set { mLEntidad = value; OnPropertyChanged("MLEntidad"); }
        }

        private ObservableCollection<MUNICIPIO> lstMunicipioML;
        public ObservableCollection<MUNICIPIO> LstMunicipioML
        {
            get { return lstMunicipioML; }
            set { lstMunicipioML = value; OnPropertyChanged("LstMunicipioML"); }
        }

        private MUNICIPIO selectedMunicipioML;
        public MUNICIPIO SelectedMunicipioML
        {
            get { return selectedMunicipioML; }
            set { selectedMunicipioML = value; OnPropertyChanged("SelectedMunicipioML"); }
        }

        private short? mLMunicipio = -1;
        public short? MLMunicipio
        {
            get { return mLMunicipio; }
            set { mLMunicipio = value; OnPropertyChanged("MLMunicipio"); }
        }

        private string mLColonia;
        public string MLColonia
        {
            get { return mLColonia; }
            set { mLColonia = value; OnPropertyChanged("MLColonia"); }
        }

        private ObservableCollection<MEDIDA_LUGAR> lstMedidaLugar;
        public ObservableCollection<MEDIDA_LUGAR> LstMedidaLugar
        {
            get { return lstMedidaLugar; }
            set { lstMedidaLugar = value; OnPropertyChanged("LstMedidaLugar"); }
        }

        private MEDIDA_LUGAR selectedMedidaLugar;
        public MEDIDA_LUGAR SelectedMedidaLugar
        {
            get { return selectedMedidaLugar; }
            set { selectedMedidaLugar = value; OnPropertyChanged("LstMedidaLugar"); }
        }

        private Visibility lugarMedidaEmpty = Visibility.Collapsed;
        public Visibility LugarMedidaEmpty
        {
            get { return lugarMedidaEmpty; }
            set { lugarMedidaEmpty = value; OnPropertyChanged("LugarMedidaEmpty"); }
        }

        private string mLPertenece = "";
        public string MLPertenece
        {
            get { return mLPertenece; }
            set { mLPertenece = value; OnPropertyChanged("MLPertenece"); }
        }
        #endregion

        #region Medida Presentacion
        private ObservableCollection<LUGAR_CUMPLIMIENTO> lstLugarCumplimiento;
        public ObservableCollection<LUGAR_CUMPLIMIENTO> LstLugarCumplimiento
        {
            get { return lstLugarCumplimiento; }
            set { lstLugarCumplimiento = value; OnPropertyChanged("LstLugarCumplimiento"); }
        }

        private decimal mPRLugar;
        public decimal MPRLugar
        {
            get { return mPRLugar; }
            set { mPRLugar = value; OnPropertyChanged("MPRLugar"); }
        }

        private int mPRAsesor;
        public int MPRAsesor
        {
            get { return mPRAsesor; }
            set { mPRAsesor = value; OnPropertyChanged("MPRAsesor"); }
        }

        private string mPRObservacion;
        public string MPRObservacion
        {
            get { return mPRObservacion; }
            set { mPRObservacion = value; OnPropertyChanged("MPRObservacion"); }
        }

        private bool mPRTipo = true;
        public bool MPRTipo
        {
            get { return mPRTipo; }
            set { mPRTipo = value;
            if (value)
            {
                VProgramacion = Visibility.Visible;
                vExcepciones = Visibility.Collapsed;
            }
            else
            {
                vExcepciones = Visibility.Visible;
                VProgramacion = Visibility.Collapsed;
            }
                OnPropertyChanged("MPRTipo"); }
        }

        private Visibility vProgramacion = Visibility.Visible;
        public Visibility VProgramacion
        {
            get { return vProgramacion; }
            set { vProgramacion = value; OnPropertyChanged("VProgramacion"); }
        }

        private Visibility vExcepciones = Visibility.Collapsed;
        public Visibility VExcepciones
        {
            get { return vExcepciones; }
            set { vExcepciones = value; OnPropertyChanged("VExcepciones"); }
        }

        private short? mPRDia;
        public short? MPRDia
        {
            get { return mPRDia; }
            set { mPRDia = value; OnPropertyChanged("MPRDia"); }
        }

        private short? mPRCada;
        public short? MPRCada
        {
            get { return mPRCada; }
            set { mPRCada = value; OnPropertyChanged("MPRCada"); }
        }

        private DateTime? mPRPrimeraAsistencia;
        public DateTime? MPRPrimeraAsistencia
        {
            get { return mPRPrimeraAsistencia; }
            set { mPRPrimeraAsistencia = value;
            if (value != null)
            {
                MPRUltimaAsistencia = value.Value.Date.AddYears(1);
            }
                OnPropertyChanged("MPRPrimeraAsistencia"); }
        }

        private DateTime? mPRUltimaAsistencia;
        public DateTime? MPRUltimaAsistencia
        {
            get { return mPRUltimaAsistencia; }
            set { mPRUltimaAsistencia = value; OnPropertyChanged("MPRUltimaAsistencia"); }
        }

        private DateTime? mPRFechaAgrerar;
        public DateTime? MPRFechaAgrerar
        {
            get { return mPRFechaAgrerar; }
            set { mPRFechaAgrerar = value; OnPropertyChanged("MPRFechaAgrerar"); }
        }

        private ObservableCollection<DateTime> lstFechasProgramar;
        public ObservableCollection<DateTime> LstFechasProgramar
        {
            get { return lstFechasProgramar; }
            set { lstFechasProgramar = value; OnPropertyChanged("LstFechasProgramar"); }
        }

        private DateTime selectedFechaProgramar;
        public DateTime SelectedFechaProgramar
        {
            get { return selectedFechaProgramar; }
            set { selectedFechaProgramar = value; OnPropertyChanged("SelectedFechaProgramar"); }
        }

        private ObservableCollection<DateTime> lstFechasExcepcion;
        public ObservableCollection<DateTime> LstFechasExcepcion
        {
            get { return lstFechasExcepcion; }
            set { lstFechasExcepcion = value; OnPropertyChanged("LstFechasExcepcion"); }
        }

        private DateTime selectedFechaExcepcion;
        public DateTime SelectedFechaExcepcion
        {
            get { return selectedFechaExcepcion; }
            set { selectedFechaExcepcion = value; OnPropertyChanged("SelectedFechaExcepcion"); }
        }

        private ObservableCollection<MEDIDA_PRESENTACION> lstMedidaPresentacion;
        public ObservableCollection<MEDIDA_PRESENTACION> LstMedidaPresentacion
        {
            get { return lstMedidaPresentacion; }
            set { lstMedidaPresentacion = value; OnPropertyChanged("LstMedidaPresentacion"); }
        }

        private MEDIDA_PRESENTACION selectedMedidaPresentacion;
        public MEDIDA_PRESENTACION SelectedMedidaPresentacion
        {
            get { return selectedMedidaPresentacion; }
            set { selectedMedidaPresentacion = value; OnPropertyChanged("SelectedMedidaPresentacion"); }
        }

        private ObservableCollection<MEDIDA_PRESENTACION_DETALLE> lstMedidaPresentacionDetalle;
        public ObservableCollection<MEDIDA_PRESENTACION_DETALLE> LstMedidaPresentacionDetalle
        {
            get { return lstMedidaPresentacionDetalle; }
            set { lstMedidaPresentacionDetalle = value; OnPropertyChanged("LstMedidaPresentacionDetalle"); }
        }

        private MEDIDA_PRESENTACION_DETALLE selectedMedidaPresentacionDetalle;
        public MEDIDA_PRESENTACION_DETALLE SelectedMedidaPresentacionDetalle
        {
            get { return selectedMedidaPresentacionDetalle; }
            set { selectedMedidaPresentacionDetalle = value; OnPropertyChanged("LstMedidaPresentacionDetalle"); }
        }

        private Visibility presentacionMedidaEmpty = Visibility.Visible;
        public Visibility PresentacionMedidaEmpty
        {
            get { return presentacionMedidaEmpty; }
            set { presentacionMedidaEmpty = value; OnPropertyChanged("PresentacionMedidaEmpty"); }
        }

        private DateTime HOY = Fechas.GetFechaDateServer.Date;

        private string _MPObservacion;
        public string MPObservacion
        {
            get { return _MPObservacion; }
            set { _MPObservacion = value;
                OnPropertyChanged("MPObservacion"); }
        }

        private bool _MPHuellaEmpleado = false;
        public bool MPHuellaEmpleado
        {
            get { return _MPHuellaEmpleado; }
            set { _MPHuellaEmpleado = value; OnPropertyChanged("MPHuellaEmpleado"); }
        }
        #endregion

        #region Medida Documento
        private DateTime? mDFecha;
        public DateTime? MDFecha
        {
            get { return mDFecha; }
            set { mDFecha = value; OnPropertyChanged("MDFecha"); }
        }

        private string mDFolio;
        public string MDFolio
        {
            get { return mDFolio; }
            set { mDFolio = value; OnPropertyChanged("MDFolio"); }
        }

        private ObservableCollection<Asesor> lstAsesor;
        public ObservableCollection<Asesor> LstAsesor
        {
            get { return lstAsesor; }
            set { lstAsesor = value; OnPropertyChanged("LstAsesor"); }
        }

        private int? mDAutor = -1;
        public int? MDAutor
        {
            get { return mDAutor; }
            set { mDAutor = value; OnPropertyChanged("MDAutor"); }
        }

        private string mDTitulo;
        public string MDTitulo
        {
            get { return mDTitulo; }
            set { mDTitulo = value; OnPropertyChanged("MDTitulo"); }
        }

        private ObservableCollection<FUENTE> lstFuente;
        public ObservableCollection<FUENTE> LstFuente
        {
            get { return lstFuente; }
            set { lstFuente = value; OnPropertyChanged("LstFuente"); }
        }

        private decimal? mDFuente = -1;
        public decimal? MDFuente
        {
            get { return mDFuente; }
            set { mDFuente = value; OnPropertyChanged("MDFuente"); }
        }

        private bool mDEntrada = true;
        public bool MDEntrada
        {
            get { return mDEntrada; }
            set { mDEntrada = value;
            if (value)
            {
                LstTipoDocumentoMedidaFiltro = new ObservableCollection<TIPO_DOCUMENTO_MEDIDA>(LstTipoDocumentoMedida.Where(w => w.SENTIDO == "E"));
                LstTipoDocumentoMedidaFiltro.Insert(0, new TIPO_DOCUMENTO_MEDIDA() { ID_TIPDOCMED = -1, DESCR = "SELECCIONE" });
                MDTipoDocumento = -1;
            }
                
                OnPropertyChanged("MDEntrada"); }
        }

        private bool mDSalida = false;
        public bool MDSalida
        {
            get { return mDSalida; }
            set { mDSalida = value;
            if (value)
            {
                LstTipoDocumentoMedidaFiltro = new ObservableCollection<TIPO_DOCUMENTO_MEDIDA>(LstTipoDocumentoMedida.Where(w => w.SENTIDO == "S"));
                LstTipoDocumentoMedidaFiltro.Insert(0, new TIPO_DOCUMENTO_MEDIDA() { ID_TIPDOCMED = -1, DESCR = "SELECCIONE" });
                MDTipoDocumento = -1;
            }
                OnPropertyChanged("MDSalida"); }
        }

        private ObservableCollection<TIPO_DOCUMENTO_MEDIDA> lstTipoDocumentoMedida;
        public ObservableCollection<TIPO_DOCUMENTO_MEDIDA> LstTipoDocumentoMedida
        {
            get { return lstTipoDocumentoMedida; }
            set { lstTipoDocumentoMedida = value; OnPropertyChanged("LstTipoDocumentoMedida"); }
        }

        private ObservableCollection<TIPO_DOCUMENTO_MEDIDA> lstTipoDocumentoMedidaFiltro;
        public ObservableCollection<TIPO_DOCUMENTO_MEDIDA> LstTipoDocumentoMedidaFiltro
        {
            get { return lstTipoDocumentoMedidaFiltro; }
            set { lstTipoDocumentoMedidaFiltro = value; OnPropertyChanged("LstTipoDocumentoMedidaFiltro"); }
        }

        private decimal? mDTipoDocumento;
        public decimal? MDTipoDocumento
        {
            get { return mDTipoDocumento; }
            set { mDTipoDocumento = value; OnPropertyChanged("MDTipoDocumento"); }
        }

        private byte[] mDDocumento;
        public byte[] MDDocumento
        {
            get { return mDDocumento; }
            set { mDDocumento = value; OnPropertyChanged("MDDocumento"); }
        }

        private ObservableCollection<MEDIDA_DOCUMENTO> lstMedidaDocumento;
        public ObservableCollection<MEDIDA_DOCUMENTO> LstMedidaDocumento
        {
            get { return lstMedidaDocumento; }
            set { lstMedidaDocumento = value;
            if (value != null)
            { 
            if(value.Count > 0)
                TabMedidaSEnabled = true;
            else
                TabMedidaSEnabled = false;
            }
            else
                TabMedidaSEnabled = false;

                OnPropertyChanged("LstMedidaDocumento"); }
        }

        private ObservableCollection<MEDIDA_DOCUMENTO> lstMedidaDocumentoCB;
        public ObservableCollection<MEDIDA_DOCUMENTO> LstMedidaDocumentoCB
        {
            get { return lstMedidaDocumentoCB; }
            set { lstMedidaDocumentoCB = value; OnPropertyChanged("LstMedidaDocumentoCB"); }
        }

        //private ObservableCollection<MEDIDA_DOCUMENTO> lstMedidaDocumentoCB;
        //public ObservableCollection<MEDIDA_DOCUMENTO> LstMedidaDocumentoCB
        //{
        //    get { return lstMedidaDocumentoCB; }
        //    set { lstMedidaDocumentoCB = value; OnPropertyChanged("LstMedidaDocumentoCB"); }
        //}
    
        private MEDIDA_DOCUMENTO selectedMedidaDocumento;
        public MEDIDA_DOCUMENTO SelectedMedidaDocumento
        {
            get { return selectedMedidaDocumento; }
            set { selectedMedidaDocumento = value; 
                OnPropertyChanged("SelectedMedidaDocumento"); }
        }

        private string mDSeleccion = string.Empty;
        public string MDSeleccion
        {
            get { return mDSeleccion; }
            set { mDSeleccion = value; OnPropertyChanged("MDSeleccion"); }
        }
        #endregion

        #region Validacioes
        private short tabIndex = 0;
        public short TabIndex
        {
            get { return tabIndex; }
            set { tabIndex = value;
            if (value == 1)
            {
                
                MenuGuardarEnabled = true;
            }
            else
            {
                
                MenuGuardarEnabled = false;
            }
                OnPropertyChanged("TabIndex"); }
        }

        private bool tabIdentificacionEnabled = false;
        public bool TabIdentificacionEnabled
        {
            get { return tabIdentificacionEnabled; }
            set { tabIdentificacionEnabled = value; OnPropertyChanged("TabIdentificacionEnabled"); }
        }
        
        private bool tabMedidaEnabled = false;
        public bool TabMedidaEnabled
        {
            get { return tabMedidaEnabled; }
            set { tabMedidaEnabled = value; OnPropertyChanged("TabMedidaEnabled"); }
        }
        
        private bool tabMedidaSEnabled = false;
        public bool TabMedidaSEnabled
        {
            get { return tabMedidaSEnabled; }
            set { tabMedidaSEnabled = value; OnPropertyChanged("TabMedidaSEnabled"); }
        }
        
        private bool botonesEnabled = false;
        public bool BotonesEnabled
        {
            get { return botonesEnabled; }
            set { botonesEnabled = value; OnPropertyChanged("BotonesEnabled"); }
        }
        #endregion

        #region Busqueda Huellas
        private BuscarPorHuellaYNipMedidaView HuellaWindow;
       
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
      
        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }
        private bool LeyendoHuellas { get; set; }
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
        private string _TextNipBusqueda;
        public string TextNipBusqueda
        {
            get { return _TextNipBusqueda; }
            set { _TextNipBusqueda = value; OnPropertyChanged("TextNipBusqueda"); }
        }

        private bool _ElementosDisponibles = false;
        public bool ElementosDisponibles
        {
            get { return _ElementosDisponibles; }
            set { _ElementosDisponibles = value; OnPropertyChanged("ElementosDisponibles"); }
        }

        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion

        #region Liberados
        private RangeEnabledObservableCollection<cLiberados> lstLiberados;
        public RangeEnabledObservableCollection<cLiberados> LstLiberados
        {
            get { return lstLiberados; }
            set { lstLiberados = value; OnPropertyChanged("LstLiberados"); }
        }

        private cLiberados selectedLiberado;
        public cLiberados SelectedLiberado
        {
            get { return selectedLiberado; }
            set { selectedLiberado = value;
            if (value != null)
            {
                SelectExpediente = new cImputado().Obtener(value.ID_IMPUTADO, value.ID_ANIO, value.ID_CENTRO).FirstOrDefault();
            }
            else
            {
                SelectExpediente = null;
            }
                OnPropertyChanged("SelectedLiberado"); }
        }
        #endregion

        #region Seguimiento
        private ObservableCollection<PROCESO_LIBERTAD_SEGUIMIENTO> lstSeguimiento;
        public ObservableCollection<PROCESO_LIBERTAD_SEGUIMIENTO> LstSeguimiento
        {
            get { return lstSeguimiento; }
            set { lstSeguimiento = value; OnPropertyChanged("LstSeguimiento"); }
        }

        private Visibility lstProcesoSeguimientoIsEmpty = Visibility.Collapsed;
        public Visibility LstProcesoSeguimientoIsEmpty
        {
            get { return lstProcesoSeguimientoIsEmpty; }
            set { lstProcesoSeguimientoIsEmpty = value; OnPropertyChanged("LstProcesoSeguimientoIsEmpty"); }
        }

        private PROCESO_LIBERTAD_SEGUIMIENTO selectedSeguimiento;
        public PROCESO_LIBERTAD_SEGUIMIENTO SelectedSeguimiento
        {
            get { return selectedSeguimiento; }
            set { selectedSeguimiento = value; OnPropertyChanged("SelectedSeguimiento"); }
        }

        private DateTime? sFecha = Fechas.GetFechaDateServer;
        public DateTime? SFecha
        {
            get { return sFecha; }
            set { sFecha = value; 
                if(value != null)
                {
                    base.RemoveRule("SFecha");
                    base.AddRule(() => SFecha, () => SFecha.Value.Date <= HOY.Date, "LA FECHA DEBE SER MENOR O IGUAL AL DIA DE HOY!");
                }
                OnPropertyChanged("SFecha"); }
        }

        private string sObservacion;
        public string SObservacion
        {
            get { return sObservacion; }
            set { sObservacion = value; OnPropertyChanged("SObservacion"); }
        }
        #endregion

        #region Ficha
        private short? _CPAnio;
        public short? CPAnio
        {
            get { return _CPAnio; }
            set { _CPAnio = value; OnPropertyChanged("CPAnio"); }
        }

        private int? _CPFolio;
        public int? CPFolio
        {
            get { return _CPFolio; }
            set { _CPFolio = value; OnPropertyChanged("CPFolio"); }
        }

        private string juzgado;
        public string Juzgado
        {
            get { return juzgado; }
            set { juzgado = value; OnPropertyChanged("Juzgado"); }
        }
        private string delito;
        public string Delito
        {
            get { return delito; }
            set { delito = value; OnPropertyChanged("Delito"); }
        }

        private CAUSA_PENAL _CausaPenal;
        public CAUSA_PENAL CausaPenal
        {
            get { return _CausaPenal; }
            set { _CausaPenal = value;
            if (value == null)
                LimpiarCausaPenal();
                OnPropertyChanged("CausaPenal"); }
        }
        #endregion

        #region Filtros
        private bool porNUC = true;
        public bool PorNUC
        {
            get { return porNUC; }
            set { porNUC = value;
            if (value)
            {
                PorCP = false;
                PorNUCVisible = Visibility.Visible;
                PorCPVisible = Visibility.Collapsed;
                AnioBuscar = null;
                FolioBuscar = null;
            }
            else
            {
                PorCP = true;
                PorNUCVisible = Visibility.Collapsed;
                PorCPVisible = Visibility.Visible;
                NUCBuscar = string.Empty;
            }
                OnPropertyChanged("PorNUC"); }
        }

        private bool porCP = false;
        public bool PorCP
        {
            get { return porCP; }
            set { porCP = value; OnPropertyChanged("PorCP"); }
        }
        
        private Visibility porNUCVisible = Visibility.Visible;
        public Visibility PorNUCVisible
        {
            get { return porNUCVisible; }
            set { porNUCVisible = value; OnPropertyChanged("PorNUCVisible"); }
        }

        private Visibility porCPVisible = Visibility.Collapsed;
        public Visibility PorCPVisible
        {
            get { return porCPVisible; }
            set { porCPVisible = value; OnPropertyChanged("PorCPVisible"); }
        }

        
        #endregion

    }
}

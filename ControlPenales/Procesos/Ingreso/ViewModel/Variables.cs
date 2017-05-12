using ControlPenales.BiometricoServiceReference;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
//using MvvmFramework;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        #region Variables Privadas
        DateTime _FechaServer = Fechas.GetFechaDateServer;
        #endregion

        #region Pantalla
        public string Name
        {
            get { return "registro_ingreso_interno"; }
        }
        private IMPUTADO imputado;
        public IMPUTADO Imputado
        {
            get { return imputado; }
            set { imputado = value; OnPropertyChanged("Imputado"); }
        }
        private IMPUTADO imputadoSeleccionadoAuxiliar;
        public IMPUTADO ImputadoSeleccionadoAuxiliar
        {
            get { return imputadoSeleccionadoAuxiliar; }
            set { imputadoSeleccionadoAuxiliar = value; OnPropertyChanged("ImputadoSeleccionadoAuxiliar"); }
        }
        private INGRESO ingreso;
        public INGRESO Ingreso
        {
            get { return ingreso; }
            set
            {
                ingreso = value;
                OnPropertyChanged("Ingreso");
                LoadSelectedTreeViewItem(Ingreso, Imputado);
            }
        }
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

                if (value != null)
                    MenuReporteEnabled = true;
                else
                    MenuReporteEnabled = false;
                OnPropertyChanged("SelectIngreso");
            }
        }
        private bool tipoTatuajeEnabled;
        public bool TipoTatuajeEnabled
        {
            get { return tipoTatuajeEnabled; }
            set { tipoTatuajeEnabled = value; OnPropertyChanged("TipoTatuajeEnabled"); }
        }
        private bool banderaEntrada;
        public bool BanderaEntrada
        {
            get { return banderaEntrada; }
            set { banderaEntrada = value; }
        }
        private bool visibleIngreso = true;
        public bool VisibleIngreso
        {
            get { return visibleIngreso; }
            set { visibleIngreso = value; OnPropertyChanged("VisibleIngreso"); }
        }
        private string tituloTop = "Registro";
        public string TituloTop
        {
            get { return tituloTop; }
            set { tituloTop = value; OnPropertyChanged("TituloTop"); }
        }

        private bool tipoRegistro;
        public bool TipoRegistro
        {
            get { return tipoRegistro; }
            set { tipoRegistro = value;
            if (tipoRegistro)
                VisibleInterconexion = true;
            else
                VisibleInterconexion = false;
                OnPropertyChanged("TipoRegistro"); }
        }

        private bool visibleInterconexion;
        public bool VisibleInterconexion
        {
            get { return visibleInterconexion; }
            set { visibleInterconexion = value;
                OnPropertyChanged("VisibleInterconexion"); }
        }

        private bool visibleBuscarNUC;
        public bool VisibleBuscarNUC
        {
            get { return visibleBuscarNUC; }
            set { visibleBuscarNUC = value; OnPropertyChanged("VisibleBuscarNUC"); }
        }

        //SELECCION DE SISTEMA TRADICIONAL / NUEVOS SISTEMA 
        private bool visibleSeleccionSistema = true;
        public bool VisibleSeleccionSistema
        {
            get { return visibleSeleccionSistema; }
            set { visibleSeleccionSistema = value; OnPropertyChanged("VisibleSeleccionSistema"); }
        }
        //private bool cambiosSinGuardar;
        //public bool CambiosSinGuardar
        //{
        //    get { return cambiosSinGuardar; }
        //    set { cambiosSinGuardar = value; OnPropertyChanged("CambiosSinGuardar"); }
        //}

        #region [TAB SELECTEDS]
        private bool nuevoImputado;
        public bool NuevoImputado
        {
            get { return nuevoImputado; }
            set { nuevoImputado = value; OnPropertyChanged("NuevoImputado"); }
        }
        private bool isSelectedDatosIngreso;
        public bool IsSelectedDatosIngreso
        {
            get { return isSelectedDatosIngreso; }
            set
            {
                isSelectedDatosIngreso = value;
                OnPropertyChanged("IsSelectedDatosIngreso");
            }
        }
        private bool isSelectedIdentificacion;
        public bool IsSelectedIdentificacion
        {
            get { return isSelectedIdentificacion; }
            set
            {
                isSelectedIdentificacion = value;
                OnPropertyChanged("IsSelectedIdentificacion");
            }
        }
        private bool isSelectedTraslado;
        public bool IsSelectedTraslado
        {
            get { return isSelectedTraslado; }
            set
            {
                isSelectedTraslado = value;
                OnPropertyChanged("IsSelectedTraslado");
            }
        }
        private bool tabDatosGenerales;
        public bool TabDatosGenerales
        {
            get { return tabDatosGenerales; }
            set
            {
                tabDatosGenerales = value;
                OnPropertyChanged("TabDatosGenerales");
            }
        }
        private bool tabApodosAlias;
        public bool TabApodosAlias
        {
            get { return tabApodosAlias; }
            set
            {
                tabApodosAlias = value;
                OnPropertyChanged("TabApodosAlias");
            }
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
        private bool tabMediaFiliacion;
        public bool TabMediaFiliacion
        {
            get { return tabMediaFiliacion; }
            set
            {
                tabMediaFiliacion = value;
                OnPropertyChanged("TabMediaFiliacion");
                //getDatosMediaFiliacion();
            }
        }
        private bool tabSenasParticulares;
        public bool TabSenasParticulares
        {
            get { return tabSenasParticulares; }
            set
            {
                tabSenasParticulares = value;
                OnPropertyChanged("TabSenasParticulares");
            }
        }
        private bool tabPandillas;
        public bool TabPandillas
        {
            get { return tabPandillas; }
            set
            {
                tabPandillas = value;
                OnPropertyChanged("TabPandillas");
            }
        }
        #endregion

        #region [TAB ENABLEDS]
        private bool camposBusquedaEnabled;
        public bool CamposBusquedaEnabled
        {
            get { return camposBusquedaEnabled; }
            set { camposBusquedaEnabled = value;
                MenuBuscarEnabled = value;
            
                OnPropertyChanged("CamposBusquedaEnabled"); }
        }
        private bool estatusAdministrativoEnabled = true;
        public bool EstatusAdministrativoEnabled
        {
            get { return estatusAdministrativoEnabled; }
            set { estatusAdministrativoEnabled = value; OnPropertyChanged("EstatusAdministrativoEnabled"); }
        }
        private bool clasificacionJuridicaEnabled = true;
        public bool ClasificacionJuridicaEnabled
        {
            get { return clasificacionJuridicaEnabled; }
            set { clasificacionJuridicaEnabled = value; OnPropertyChanged("ClasificacionJuridicaEnabled"); }
        }
        private bool datosGeneralesEnabled = true;
        public bool DatosGeneralesEnabled
        {
            get { return datosGeneralesEnabled; }
            set { datosGeneralesEnabled = value; OnPropertyChanged("DatosGeneralesEnabled"); }
        }
        private bool apodosAliasEnabled = true;
        public bool ApodosAliasEnabled
        {
            get { return apodosAliasEnabled; }
            set { apodosAliasEnabled = value; OnPropertyChanged("ApodosAliasEnabled"); }
        }
        private bool fotosHuellasEnabled = true;
        public bool FotosHuellasEnabled
        {
            get { return fotosHuellasEnabled; }
            set { fotosHuellasEnabled = value; OnPropertyChanged("FotosHuellasEnabled"); }
        }
        private bool mediaFiliacionEnabled = true;
        public bool MediaFiliacionEnabled
        {
            get { return mediaFiliacionEnabled; }
            set { mediaFiliacionEnabled = value; OnPropertyChanged("MediaFiliacionEnabled"); }
        }
        private bool senasParticularesEnabled = true;
        public bool SenasParticularesEnabled
        {
            get { return senasParticularesEnabled; }
            set { senasParticularesEnabled = value; OnPropertyChanged("SenasParticularesEnabled"); }
        }
        private bool pandillasEnabled;
        public bool PandillasEnabled
        {
            get { return pandillasEnabled; }
            set { pandillasEnabled = value; OnPropertyChanged("PandillasEnabled"); }
        }
        private bool ingresoEnabled;
        public bool IngresoEnabled
        {
            get { return ingresoEnabled; }
            set
            {
                ingresoEnabled = value;
                OnPropertyChanged("IngresoEnabled");
            }
        }
        private bool selectIngresoEnabled;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }
        private bool identificacionEnabled;
        public bool IdentificacionEnabled
        {
            get { return identificacionEnabled; }
            set { identificacionEnabled = value; OnPropertyChanged("IdentificacionEnabled"); }
        }
        private bool trasladoEnabled;
        public bool TrasladoEnabled
        {
            get { return trasladoEnabled; }
            set { trasladoEnabled = value; OnPropertyChanged("TrasladoEnabled"); }
        }
        #endregion

        #region [VISIBLES]
        private bool contenedorIdentificacionVisible = true;
        public bool ContenedorIdentificacionVisible
        {
            get { return contenedorIdentificacionVisible; }
            set { contenedorIdentificacionVisible = value; OnPropertyChanged("ContenedorIdentificacionVisible"); }
        }
        private bool tabVisible;
        public bool TabVisible
        {
            get { return tabVisible; }
            set { tabVisible = value; OnPropertyChanged("TabVisible"); }
        }
        private bool visibleTomarFotoSenasParticulares;
        public bool VisibleTomarFotoSenasParticulares
        {
            get { return visibleTomarFotoSenasParticulares; }
            set { visibleTomarFotoSenasParticulares = value; OnPropertyChanged("VisibleTomarFotoSenasParticulares"); }
        }

        private Visibility isNombreCentroVisible = Visibility.Collapsed;
        public Visibility IsNombreCentroVisible
        {
            get { return isNombreCentroVisible; }
            set { isNombreCentroVisible = value; RaisePropertyChanged("IsNombreCentroVisible"); }
        }
        #endregion

        #region [Menu Enableds]
        private bool menuGuardarEnabled;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuInsertarEnabled;
        public bool MenuInsertarEnabled
        {
            get { return menuInsertarEnabled; }
            set { menuInsertarEnabled = value; OnPropertyChanged("MenuInsertarEnabled"); }
        }
        private bool menuBorrarEnabled;
        public bool MenuBorrarEnabled
        {
            get { return menuBorrarEnabled; }
            set { menuBorrarEnabled = value; OnPropertyChanged("MenuBorrarEnabled"); }
        }
        private bool menuBuscarEnabled;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        private bool menuDeshacerEnabled;
        public bool MenuDeshacerEnabled
        {
            get { return menuDeshacerEnabled; }
            set { menuDeshacerEnabled = value; OnPropertyChanged("MenuDeshacerEnabled"); }
        }
        private bool menuLimpiarEnabled;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set { menuLimpiarEnabled = value; OnPropertyChanged("MenuLimpiarEnabled"); }
        }
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        private bool menuAyudaEnabled = true;
        public bool MenuAyudaEnabled
        {
            get { return menuAyudaEnabled; }
            set { menuAyudaEnabled = value; OnPropertyChanged("MenuAyudaEnabled"); }
        }
        private bool menuSalirEnabled = true;
        public bool MenuSalirEnabled
        {
            get { return menuSalirEnabled; }
            set { menuSalirEnabled = value; OnPropertyChanged("MenuSalirEnabled"); }
        }
        private bool menuFichaEnabled;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        //
        private ObservableCollection<IMPUTADO_FILIACION> lstImputadoFiliacion;
        public ObservableCollection<IMPUTADO_FILIACION> LstImputadoFiliacion
        {
            get { return lstImputadoFiliacion; }
            set { lstImputadoFiliacion = value; OnPropertyChanged(); }
        }


        private CENTRO centroActual;
        public CENTRO CentroActual
        {
            get { return centroActual; }
            set { centroActual = value; OnPropertyChanged("CentroActual"); }
        }

        //Validaciones
        private bool bHuellasEnabled = false;
        public bool BHuellasEnabled
        {
            get { return bHuellasEnabled; }
            set { bHuellasEnabled = value; OnPropertyValidateChanged("BHuellasEnabled"); }
        }

        //private bool menuReporteEnabled = false;

        private Visibility ubicacionArbolVisibility = Visibility.Visible;
        public Visibility UbicacionArbolVisibility
        {
            get { return ubicacionArbolVisibility; }
            set { ubicacionArbolVisibility = value; OnPropertyChanged("UbicacionArbolVisibility"); }
        }
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
            set { pEditar = value; }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
                if (value)
                {
                    if (CamposBusquedaEnabled)
                        MenuBuscarEnabled = true;
                    else
                        MenuBuscarEnabled = false;
                   BHuellasEnabled = value;
                }
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
                {
                    //MenuReporteEnabled = value;
                }
            }
        }

        //Botones
        private bool enrolarHuellasEnabled = true;
        public bool EnrolarHuellasEnabled
        {
            get { return enrolarHuellasEnabled; }
            set { enrolarHuellasEnabled = value; OnPropertyChanged("EnrolarHuellasEnabled"); }
        }
        #endregion

        short pais_param = Parametro.PAIS;

        #region Tabs
        private short opcion = 0;
        public short Opcion
        {
            get { return opcion; }
            set { opcion = value; OnPropertyChanged("Opcion"); }
        }
        #endregion

        #region Huellas
        //Mano Derecha
        private bool tPulgarD = false;
        public bool TPulgarD
        {
            get { return tPulgarD; }
            set { tPulgarD = value; OnPropertyChanged("TPulgarD"); }
        }

        private bool tIndiceD = false;
        public bool TIndiceD
        {
            get { return tIndiceD; }
            set { tIndiceD = value; OnPropertyChanged("TIndiceD"); }
        }

        private bool tMedioD = false;
        public bool TMedioD
        {
            get { return tMedioD; }
            set { tMedioD = value; OnPropertyChanged("TMedioD"); }
        }

        private bool tAnularD = false;
        public bool TAnularD
        {
            get { return tAnularD; }
            set { tAnularD = value; OnPropertyChanged("TAnularD"); }
        }

        private bool tMeniqueD = false;
        public bool TMeniqueD
        {
            get { return tMeniqueD; }
            set { tMeniqueD = value; OnPropertyChanged("TMeniqueD"); }
        }

        //Mano Izquierda
        private bool tPulgarI = false;
        public bool TPulgarI
        {
            get { return tPulgarI; }
            set { tPulgarI = value; OnPropertyChanged("TPulgarI"); }
        }

        private bool tIndiceI = false;
        public bool TIndiceI
        {
            get { return tIndiceI; }
            set { tIndiceI = value; OnPropertyChanged("TIndiceI"); }
        }

        private bool tMedioI = false;
        public bool TMedioI
        {
            get { return tMedioI; }
            set { tMedioI = value; OnPropertyChanged("TMedioI"); }
        }

        private bool tAnularI = false;
        public bool TAnularI
        {
            get { return tAnularI; }
            set { tAnularI = value; OnPropertyChanged("TAnularI"); }
        }

        private bool tMeniqueI = false;
        public bool TMeniqueI
        {
            get { return tMeniqueI; }
            set { tMeniqueI = value; OnPropertyChanged("TMeniqueI"); }
        }
        #endregion 

        #region Delitos
        private DELITO selectedDelito;
        public DELITO SelectedDelito
        {
            get { return selectedDelito; }
            set
            {
                selectedDelito = value;
                if (value != null)
                    DescrDelito = value.DESCR;
                else
                    DescrDelito = string.Empty;
                OnPropertyChanged("SelectedDelito");
            }
        }

        private string descrDelito;
        public string DescrDelito
        {
            get { return descrDelito; }
            set { descrDelito = value; OnPropertyChanged("DescrDelito"); }
        }
        #endregion
    }
}

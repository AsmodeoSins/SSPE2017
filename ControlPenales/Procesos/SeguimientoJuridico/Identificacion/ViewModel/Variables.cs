using ControlPenales.Clases;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using System.Linq;
using System.Windows;
//using MvvmFramework;

namespace ControlPenales
{
    partial class JuridicoIdentificacionViewModel
    {
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
        private bool editarImputado;
        public bool EditarImputado
        {
            get { return editarImputado; }
            set { editarImputado = value; OnPropertyChanged("EditarImputado"); }
        }
        //private bool visibleDocumento;
        //public bool VisibleDocumento
        //{
        //    get { return visibleDocumento; }
        //    set { visibleDocumento = value; OnPropertyChanged("VisibleDocumento"); }
        //}
        //private bool visibleAgregarDocumento;
        //public bool VisibleAgregarDocumento
        //{
        //    get { return visibleAgregarDocumento; }
        //    set { visibleAgregarDocumento = value; OnPropertyChanged("VisibleAgregarDocumento"); }
        //}
        private string tituloTop = "Ficha Identificación";
        public string TituloTop
        {
            get { return tituloTop; }
            set { tituloTop = value; OnPropertyChanged("TituloTop"); }
        }

        //SELECCION DE SISTEMA TRADICIONAL / NUEVOS SISTEMA 
        private bool visibleSeleccionSistema;
        public bool VisibleSeleccionSistema
        {
            get { return visibleSeleccionSistema; }
            set
            {
                visibleSeleccionSistema = value;
                if (!visibleSeleccionSistema)
                    VisibleInterconexion = false;
                OnPropertyChanged("VisibleSeleccionSistema");
            }
        }

        private bool visibleInterconexion;
        public bool VisibleInterconexion
        {
            get { return visibleInterconexion; }
            set
            {
                visibleInterconexion = value;
                OnPropertyChanged("VisibleInterconexion");
            }
        }

        #region [TAB SELECTEDS]
        private bool isSelectedDatosIngreso;
        public bool IsSelectedDatosIngreso
        {
            get { return isSelectedDatosIngreso; }
            set
            {
                isSelectedDatosIngreso = value;
                OnPropertyChanged("IsSelectedDatosIngreso");
                //setValidationRules();
                //ChecarValidaciones();
            }
        }
        private bool isSelectedIdentificacion = true;
        public bool IsSelectedIdentificacion
        {
            get { return isSelectedIdentificacion; }
            set
            {
                isSelectedIdentificacion = value;
                OnPropertyChanged("IsSelectedIdentificacion");
                //setValidationRules();
                //ChecarValidaciones();
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
                //setValidationRules();
                //ChecarValidaciones();
            }
        }
        private int bandera = 0;
        private bool tabDatosGenerales;
        public bool TabDatosGenerales
        {
            get { return tabDatosGenerales; }
            set
            {
                tabDatosGenerales = value;
                OnPropertyChanged("TabDatosGenerales");
                //ChecarValidaciones();
                //setValidationRules();
                //if (value)
                //    getDatosGeneralesIdentificacion();
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
                //setValidationRules();
                //ChecarValidaciones();
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
                //setValidationRules();
                //ChecarValidaciones();
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
                //ChecarValidaciones();
                //setValidationRules();
                //if (value)
                //    getDatosMediaFiliacion();
            }
        }
        private bool tabSenasParticulares;
        public bool TabSenasParticulares
        {
            get { return tabSenasParticulares; }
            set
            {
                tabSenasParticulares = value;
                if (value && Imputado != null && (ListSenasParticulares != null || ListSenasParticulares.Count < 1))
                {
                    ListSenasParticulares = new System.Collections.ObjectModel.ObservableCollection<SENAS_PARTICULARES>(Imputado.SENAS_PARTICULARES);
                    TextNombreCompleto = Imputado.PATERNO.Trim() + " " + Imputado.MATERNO.Trim() + " " + Imputado.NOMBRE.Trim();
                    TextExpediente = Imputado.ID_ANIO + "/" + Imputado.ID_IMPUTADO;
                }
                //ListSenasParticulares = new ObservableCollection<SENAS_PARTICULARES>((new cSenasParticulares()).Obtener(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO));
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
                //setValidationRules();
                //ChecarValidaciones();
            }
        }
        #endregion

        #region [TAB ENABLEDS]
        private bool camposBusquedaEnabled = false;
        public bool CamposBusquedaEnabled
        {
            get { return camposBusquedaEnabled; }
            set { camposBusquedaEnabled = value; OnPropertyChanged("CamposBusquedaEnabled"); }
        }
        private bool estatusAdministrativoEnabled;
        public bool EstatusAdministrativoEnabled
        {
            get { return estatusAdministrativoEnabled; }
            set { estatusAdministrativoEnabled = value; OnPropertyChanged("EstatusAdministrativoEnabled"); }
        }
        private bool clasificacionJuridicaEnabled;
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
            set { ingresoEnabled = value; OnPropertyChanged("IngresoEnabled"); }
        }
        private bool identificacionEnabled = true;
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
        private bool contenedorIdentificacionVisible;
        public bool ContenedorIdentificacionVisible
        {
            get { return contenedorIdentificacionVisible; }
            set { contenedorIdentificacionVisible = value; OnPropertyChanged("ContenedorIdentificacionVisible"); }
        }
        private bool tabVisible = true;
        public bool TabVisible
        {
            get { return tabVisible; }
            set { tabVisible = value; OnPropertyChanged("TabVisible"); }
        }
        #endregion

        #region [ENABLEDS]
        private bool tipoTatuajeEnabled;
        public bool TipoTatuajeEnabled
        {
            get { return tipoTatuajeEnabled; }
            set { tipoTatuajeEnabled = value; OnPropertyChanged("TipoTatuajeEnabled"); }
        }
        private WebCam CamaraWeb;
        #endregion

        #region [Menu Enableds]
        private bool menuGuardarEnabled = false;
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
        private bool menuBuscarEnabled = false;
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
        private bool menuLimpiarEnabled = true;
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
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        #region [DOCUMENTOS]
        private bool visibleDocumento;
        public bool VisibleDocumento
        {
            get { return visibleDocumento; }
            set { visibleDocumento = value; OnPropertyChanged("VisibleDocumento"); }
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
            set
            {
                pEditar = value;
                if (!value)
                    EnrolarHuellasEnabled = value;
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
                {
                    MenuBuscarEnabled = CamposBusquedaEnabled = BHuellasEnabled = value;
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
                { /*MenuReporteEnabled =*/ MenuFichaEnabled = value; }
            }
        }

        //botones 
        private bool bHuellasEnabled = false;
        public bool BHuellasEnabled
        {
            get { return bHuellasEnabled; }
            set { bHuellasEnabled = value; OnPropertyChanged("BHuellasEnabled"); }
        }

        private bool enrolarHuellasEnabled = false;
        public bool EnrolarHuellasEnabled
        {
            get { return enrolarHuellasEnabled; }
            set { enrolarHuellasEnabled = value; OnPropertyChanged("EnrolarHuellasEnabled"); }
        }
        //Auxiliares
        private bool AliasApodoChange = false;
        private bool FotosHuellasChange = false;
        private bool PandillaChange = false;
        #endregion

        #region Tabs
        private short anterior = 0;
        public short Anterior
        {
            get { return anterior; }
            set { anterior = value; OnPropertyChanged("Anterior"); }
        }

        private short opcion = 0;
        public short Opcion
        {
            get { return opcion; }
            set
            {
                if (!base.HasErrors)
                {
                    opcion = value;
                    GuardarTabs();
                    OnPropertyChanged("Opcion");
                }
                else
                {
                    if (!BanderaEntrada)
                    {
                        opcion = value;
                        OnPropertyChanged("Opcion");
                    }
                }
            }
        }

        private bool BanderaEntrada = false;
        #endregion

        #region Temporales
        private IMPUTADO tImputado;
        private INGRESO tIngreso;
        #endregion

        #region Pantalla
        private Visibility ubicacionArbolVisibility = Visibility.Collapsed;
        public Visibility UbicacionArbolVisibility
        {
            get { return ubicacionArbolVisibility; }
            set { ubicacionArbolVisibility = value; OnPropertyChanged("UbicacionArbolVisibility"); }
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using SSP.Servidor;
using GESAL.Clases.Extendidas;
using GESAL.Models;
namespace GESAL.ViewModels
{
    public partial class RegistroTraspasosExternosViewModel
    {
        #region Propiedades
        #region Propiedades Filtro Origen

        private List<TipoBusquedaAlmacen> tiposBusqueda = new TiposBusquedaAlmacen().LISTA_TIPOBUSQUEDAALMACEN;

        public List<TipoBusquedaAlmacen> TiposBusqueda
        {
            get { return tiposBusqueda; }
        }

        private TipoBusquedaAlmacen selectedTipoBusqueda;

        private ObservableCollection<MUNICIPIO> municipiosOrigen;
        public ObservableCollection<MUNICIPIO> MunicipiosOrigen
        {
            get { return municipiosOrigen; }
            set { municipiosOrigen = value; }
        }

        private MUNICIPIO selectedMunicipioOrigen;
        public MUNICIPIO SelectedMunicipioOrigen
        {
            get { return selectedMunicipioOrigen; }
            set
            {
                selectedMunicipioOrigen = value;
                RaisePropertyChanged("SelectedMunicipioOrigen");
                if (value != null)
                    MunicipioCambioOrigen(value.ID_MUNICIPIO);
            }
        }
        private ObservableCollection<CENTRO> centrosOrigen;
        public ObservableCollection<CENTRO> CentrosOrigen
        {
            get { return centrosOrigen; }
            set { centrosOrigen = value; RaisePropertyChanged("CentrosOrigen"); }
        }

        private CENTRO selectedCentroOrigen;
        public CENTRO SelectedCentroOrigen
        {
            get { return selectedCentroOrigen; }
            set
            {
                selectedCentroOrigen = value;
                RaisePropertyChanged("SelectedCentroOrigen");
                if (value != null)
                    CentroCambioOrigen(value.ID_CENTRO);
            }
        }

        private ObservableCollection<ALMACEN> almacenesPrincipalesOrigen;
        public ObservableCollection<ALMACEN> AlmacenesPrincipalesOrigen
        {
            get { return almacenesPrincipalesOrigen; }
            set { almacenesPrincipalesOrigen = value; RaisePropertyChanged("AlmacenesPrincipalesOrigen"); }
        }

        private ALMACEN selectedAlmacenPrincipalOrigen;
        public ALMACEN SelectedAlmacenPrincipalOrigen
        {
            get { return selectedAlmacenPrincipalOrigen; }
            set 
            {
                selectedAlmacenPrincipalOrigen = value;
                RaisePropertyChanged("SelectedAlmacenPrincipalOrigen");
                if (value != null)
                    AlmacenPrincipalCambioOrigen(value.ID_ALMACEN);
            }
        }

        private ObservableCollection<ALMACEN_TIPO_CAT> almacen_Tipos_CatOrigen;
        public ObservableCollection<ALMACEN_TIPO_CAT> Almacen_Tipos_CatOrigen
        {
            get { return almacen_Tipos_CatOrigen; }
            set { almacen_Tipos_CatOrigen = value; RaisePropertyChanged("Almacen_Tipos_CatOrigen"); }
        }

        private ALMACEN_TIPO_CAT selectedAlmacen_Tipo_CatOrigen;
        public ALMACEN_TIPO_CAT SelectedAlmacen_Tipo_CatOrigen
        {
            get { return selectedAlmacen_Tipo_CatOrigen; }
            set
            {
                selectedAlmacen_Tipo_CatOrigen = value;
                RaisePropertyChanged("SelectedAlmacen_Tipo_CatOrigen");
                if (value != null)
                    Almacen_TipoCambioOrigen(value.ID_ALMACEN_TIPO);
            }
        }
        #endregion

        #region Propiedades Filtro Emisor
        
        private ObservableCollection<MUNICIPIO> municipios;
        public ObservableCollection<MUNICIPIO> Municipios
        {
            get { return municipios; }
            set { municipios = value; }
        }

        private MUNICIPIO selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return selectedMunicipio; }
            set 
            { 
                selectedMunicipio = value;
                RaisePropertyChanged("SelectedMunicipio");
                if (value != null)
                    MunicipioCambio(value.ID_MUNICIPIO);
            }
        }
        private ObservableCollection<CENTRO> centros;
        public ObservableCollection<CENTRO> Centros
        {
            get { return centros; }
            set { centros = value; RaisePropertyChanged("Centros"); }
        }

        private CENTRO selectedCentro;
        public CENTRO SelectedCentro
        {
            get { return selectedCentro; }
            set 
            {
                selectedCentro = value;
                RaisePropertyChanged("SelectedCentro");
                if (value != null)
                    CentroCambio(value.ID_CENTRO);
            }
        }

        private ObservableCollection<ALMACEN> almacenesPrincipales;
        public ObservableCollection<ALMACEN> AlmacenesPrincipales
        {
            get { return almacenesPrincipales; }
            set { almacenesPrincipales = value; RaisePropertyChanged("AlmacenesPrincipales"); }
        }

        private ALMACEN selectedAlmacenPrincipal;
        public ALMACEN SelectedAlmacenPrincipal
        {
            get { return selectedAlmacenPrincipal; }
            set { selectedAlmacenPrincipal = value; RaisePropertyChanged("SelectedAlmacenPrincipal"); }
        }

        private ObservableCollection<ALMACEN_TIPO_CAT> almacen_Tipos_Cat;
        public ObservableCollection<ALMACEN_TIPO_CAT> Almacen_Tipos_Cat
        {
            get { return almacen_Tipos_Cat; }
            set { almacen_Tipos_Cat = value; RaisePropertyChanged("Almacen_Tipos_Cat"); }
        }

        private ALMACEN_TIPO_CAT selectedAlmacen_Tipo_Cat;
        public ALMACEN_TIPO_CAT SelectedAlmacen_Tipo_Cat
        {
            get { return selectedAlmacen_Tipo_Cat; }
            set 
            { 
                selectedAlmacen_Tipo_Cat = value;
                RaisePropertyChanged("SelectedAlmacen_Tipo_Cat");
                if (value != null)
                    Almacen_TipoCambio(value.ID_ALMACEN_TIPO);
            }
        }
        #endregion

        private ObservableCollection<EXT_V_INVENTARIO_PRODUCTO> inventario;

        private ObservableCollection<EXT_V_INVENTARIO_PRODUCTO> inventarioCopia;
        public ObservableCollection<EXT_V_INVENTARIO_PRODUCTO> InventarioCopia
        {
            get { return inventarioCopia; }
            set { inventarioCopia = value; RaisePropertyChanged("InventarioCopia"); }
        }

        private EXT_V_INVENTARIO_PRODUCTO selectedInventario;
        public EXT_V_INVENTARIO_PRODUCTO SelectedInventario
        {
            get { return selectedInventario; }
            set { selectedInventario = value; RaisePropertyChanged("SelectedInventario"); }
        }
            

        private ObservableCollection<EXT_PRODUCTOS_TRASPASO> productos_Traspaso=new ObservableCollection<EXT_PRODUCTOS_TRASPASO>();
        public ObservableCollection<EXT_PRODUCTOS_TRASPASO> Productos_Traspaso
        {
            get { return productos_Traspaso; }
            set 
            {
                productos_Traspaso = value;
                if (value != null && value.Count > 0)
                    OnPropertyValidateChanged("Productos_Traspaso");
                else
                    RaisePropertyChanged("Productos_Traspaso");
            }
        }

        private EXT_PRODUCTOS_TRASPASO selectedProductos_Traspaso;
        public EXT_PRODUCTOS_TRASPASO SelectedProductos_Traspaso
        {
            get { return selectedProductos_Traspaso; }
            set { selectedProductos_Traspaso = value; RaisePropertyChanged("SelectedProductos_Traspaso"); }
        }

        private string buscarProducto;

        public string BuscarProducto
        {
            get { return buscarProducto; }
            set { buscarProducto = value; RaisePropertyChanged("BuscarProducto"); }
        }
        #endregion
    }
}

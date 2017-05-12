using MVVMShared.ViewModels;
using SSP.Controlador.Catalogo.Almacenes;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSP.Controlador.Principales.Almacenes;
using GESAL.Clases.Extendidas;
using GESAL.Clases.ExtensionesClases;
namespace GESAL.ViewModels
{
    public partial class RegistroTraspasosExternosViewModel : ValidationViewModelBase, IDataErrorInfo
    {

        public async void OnLoad(object sender)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarAlmacen_TipoCatOrigen();
                    CargarMunicipiosOrigen();
                    CargarCentrosOrigen(SelectedMunicipioOrigen.ID_MUNICIPIO);
                    CargarAlmacenesPrincipalesOrigen(SelectedAlmacen_Tipo_CatOrigen.ID_ALMACEN_TIPO, SelectedCentroOrigen.ID_CENTRO);
                    CargarAlmacen_TipoCat();
                    CargarMunicipios();
                    CargarCentros(SelectedMunicipio.ID_MUNICIPIO);
                    CargarAlmacenesPrincipales(SelectedAlmacen_Tipo_Cat.ID_ALMACEN_TIPO, SelectedCentro.ID_CENTRO);
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch(Exception ex)
            {
                _error = true;
            }
            if(_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }

        #region Metodos Filtros Origen
        private void CargarMunicipiosOrigen()
        {
            try
            {
                municipiosOrigen = new ObservableCollection<MUNICIPIO>(new cMunicipio().Obtener(2).ToList());
                var dummy = new MUNICIPIO
                {
                    MUNICIPIO1 = "SELECCIONE UNA",
                    ID_MUNICIPIO = -1
                };
                municipiosOrigen.Add(dummy);
                RaisePropertyChanged("MunicipiosOrigen");
                selectedMunicipioOrigen = dummy;
                RaisePropertyChanged("SelectedMunicipioOrigen");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los municipios. Favor de contactar al administrador");
            }
        }

        private void CargarCentrosOrigen(int id_municipio)
        {
            try
            {
                if (id_municipio != -1)
                    centrosOrigen = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos(string.Empty, 0, id_municipio).ToList());
                else
                    centrosOrigen = new ObservableCollection<CENTRO>();
                var dummy = new CENTRO
                {
                    ID_CENTRO = -1,
                    DESCR = "SELECCIONE UNA"
                };
                centrosOrigen.Add(dummy);
                RaisePropertyChanged("CentrosOrigen");
                selectedCentroOrigen = dummy;
                RaisePropertyChanged("SelectedCentroOrigen");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los centros. Favor de contactar al administrador");
            }
        }

        private void CargarAlmacenesPrincipalesOrigen(short id_almacen_tipo_cat, short id_centro)
        {
            try
            {
                if (id_almacen_tipo_cat != 1 && id_centro != -1)
                    almacenesPrincipalesOrigen = new ObservableCollection<ALMACEN>(new cAlmacen().SeleccionarAlmacenesPrincipales(id_almacen_tipo_cat, id_centro, _usuario.Almacen_Grupo, "S"));
                else
                    almacenesPrincipalesOrigen = new ObservableCollection<ALMACEN>();
                var dummy = new ALMACEN
                {
                    ID_ALMACEN = -1,
                    DESCRIPCION = "SELECCIONE UNA"
                };
                almacenesPrincipalesOrigen.Add(dummy);
                RaisePropertyChanged("AlmacenesPrincipalesOrigen");
                selectedAlmacenPrincipalOrigen = dummy;
                RaisePropertyChanged("SelectedAlmacenPrincipalOrigen");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los almacenes. Favor de contactar al administrador");
            }
        }

        private void CargarAlmacen_TipoCatOrigen()
        {
            try
            {
                almacen_Tipos_CatOrigen = new ObservableCollection<ALMACEN_TIPO_CAT>(new cAlmacen_Tipo_Cat().Seleccionar(_usuario.Almacen_Grupo, "S"));
                var dummy = new ALMACEN_TIPO_CAT
                {
                    ID_ALMACEN_TIPO = -1,
                    DESCR = "SELECCIONE UNA"
                };
                almacen_Tipos_CatOrigen.Add(dummy);
                RaisePropertyChanged("Almacen_Tipos_CatOrigen");
                selectedAlmacen_Tipo_CatOrigen = dummy;
                RaisePropertyChanged("SelectedAlmacen_Tipo_CatOrigen");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los tipos de almacen. Favor de contactar al administrador");
            }
        }

        private async void MunicipioCambioOrigen(int id_municipio)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                CargarCentrosOrigen(id_municipio);
                CargarAlmacenesPrincipalesOrigen(SelectedAlmacen_Tipo_CatOrigen.ID_ALMACEN_TIPO, selectedCentroOrigen.ID_CENTRO);
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }

        private async void CentroCambioOrigen(short id_centro)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {

                CargarAlmacenesPrincipalesOrigen(SelectedAlmacen_Tipo_CatOrigen.ID_ALMACEN_TIPO, id_centro);
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }

        private async void Almacen_TipoCambioOrigen(short id_almacen_tipo_cat)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                CargarAlmacenesPrincipalesOrigen(id_almacen_tipo_cat, SelectedCentroOrigen.ID_CENTRO);
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }

        private async void AlmacenPrincipalCambioOrigen(int id_almacen)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                CargarInventario(id_almacen);
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }

        #endregion

        #region Metodos Filtros Emisor
        private void CargarMunicipios()
        {
            try
            {
                municipios = new ObservableCollection<MUNICIPIO>(new cMunicipio().Obtener(2).ToList());
                var dummy = new MUNICIPIO {
                    MUNICIPIO1="SELECCIONE UNA",
                    ID_MUNICIPIO=-1
                };
                municipios.Add(dummy);
                RaisePropertyChanged("Municipios");
                selectedMunicipio = dummy;
                RaisePropertyChanged("SelectedMunicipio");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los municipios. Favor de contactar al administrador");
            }
        }

        private void CargarCentros(int id_municipio)
        {
            try
            {
                if (id_municipio != -1)
                    centros = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos(string.Empty, 0, id_municipio).ToList());
                else
                    centros = new ObservableCollection<CENTRO>();
                var dummy = new CENTRO {
                    ID_CENTRO=-1,
                    DESCR="SELECCIONE UNA"
                };
                centros.Add(dummy);
                RaisePropertyChanged("Centros");
                selectedCentro = dummy;
                RaisePropertyChanged("SelectedCentro");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los centros. Favor de contactar al administrador");
            }
        }

        private void CargarAlmacenesPrincipales(short id_almacen_tipo_cat, short id_centro)
        {
            try
            {
                if (id_almacen_tipo_cat != 1 && id_centro != -1)
                    almacenesPrincipales = new ObservableCollection<ALMACEN>(new cAlmacen().SeleccionarAlmacenesPrincipales(id_almacen_tipo_cat, id_centro, _usuario.Almacen_Grupo, "S"));
                else
                    almacenesPrincipales = new ObservableCollection<ALMACEN>();
                var dummy = new ALMACEN {
                    ID_ALMACEN=-1,
                    DESCRIPCION="SELECCIONE UNA"
                };
                almacenesPrincipales.Add(dummy);
                RaisePropertyChanged("AlmacenesPrincipales");
                selectedAlmacenPrincipal = dummy;
                RaisePropertyChanged("SelectedAlmacenPrincipal");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los almacenes. Favor de contactar al administrador");
            }
        }

        private void CargarAlmacen_TipoCat()
        {
            try
            {
                almacen_Tipos_Cat = new ObservableCollection<ALMACEN_TIPO_CAT>(new cAlmacen_Tipo_Cat().Seleccionar(_usuario.Almacen_Grupo, "S"));
                var dummy = new ALMACEN_TIPO_CAT {
                    ID_ALMACEN_TIPO=-1,
                    DESCR="SELECCIONE UNA"
                };
                almacen_Tipos_Cat.Add(dummy);
                RaisePropertyChanged("Almacen_Tipos_Cat");
                selectedAlmacen_Tipo_Cat = dummy;
                RaisePropertyChanged("SelectedAlmacen_Tipo_Cat");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los tipos de almacen. Favor de contactar al administrador");
            }
        }

        private async void MunicipioCambio(int id_municipio)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                CargarCentros(id_municipio);
                CargarAlmacenesPrincipales(SelectedAlmacen_Tipo_Cat.ID_ALMACEN_TIPO, selectedCentro.ID_CENTRO);
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }

        private async void CentroCambio(short id_centro)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {

                CargarAlmacenesPrincipales(SelectedAlmacen_Tipo_Cat.ID_ALMACEN_TIPO, id_centro);
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }

        private async void Almacen_TipoCambio(short id_almacen_tipo_cat)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                CargarAlmacenesPrincipales(id_almacen_tipo_cat, SelectedCentro.ID_CENTRO);
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }
        #endregion

        private void CargarInventario(int id_almacen)
        {
            try
            {
                if (id_almacen != -1)
                    inventario = new ObservableCollection<EXT_V_INVENTARIO_PRODUCTO>(new cVista_Inventario_Producto().Seleccionar(id_almacen)
                        .Select(s => new EXT_V_INVENTARIO_PRODUCTO
                        {
                            CANTIDAD = s.CANTIDAD,
                            DESCR = s.DESCR,
                            ID_ALMACEN = s.ID_ALMACEN,
                            ID_PRESENTACION = s.PRESENTACION,
                            PRESENTACION = s.PRESENTACION_DESCR,
                            ID_UNIDAD_MEDIDA = s.UNIDAD_MEDIDA,
                            UNIDAD_MEDIDA = s.UNIDAD_DESCRIP,
                            NOMBRE = s.NOMBRE,
                            Seleccionado = false
                        }));
                else
                    inventario = new ObservableCollection<EXT_V_INVENTARIO_PRODUCTO>();

                InventarioCopia = new ObservableCollection<EXT_V_INVENTARIO_PRODUCTO>(inventario.Clone());
            }
            catch(Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "hubo un error al cargar el inventario del almacen. Favor de contactar al administrador");
            }
        }

        private void OnClickSwitch(object parametro)
        {
            if (parametro!=null)
            {
                switch(parametro.ToString())
                {
                    case "buscar_producto":
                        BusquedaProducto();
                        break;
                    case "agregar_producto":
                        AgregarProducto();
                        break;
                }
            }
        }

        private void BusquedaProducto()
        {
            if (!string.IsNullOrWhiteSpace(BuscarProducto))
            {
                if (inventario!=null)
                {
                    var temp = new ObservableCollection<EXT_V_INVENTARIO_PRODUCTO>(inventario.Where(w => w.NOMBRE.Contains(BuscarProducto)));
                    InventarioCopia = new ObservableCollection<EXT_V_INVENTARIO_PRODUCTO>(temp.Clone());
                }
            }
            else
            {
                InventarioCopia = new ObservableCollection<EXT_V_INVENTARIO_PRODUCTO>(inventario.Clone());
            }
        }

        private void AgregarProducto()
        {
            if (SelectedInventario!=null && SelectedInventario.Seleccionado!=true)
            {
                var temp = new ObservableCollection<EXT_PRODUCTOS_TRASPASO>(Productos_Traspaso.Clone());
                temp.Add(new EXT_PRODUCTOS_TRASPASO {
                    CANTIDAD=SelectedInventario.CANTIDAD,
                    DESCR=SelectedInventario.DESCR,
                    ID_ALMACEN=SelectedInventario.ID_ALMACEN,
                    ID_PRESENTACION=SelectedInventario.ID_PRESENTACION,
                    ID_PRODUCTO=SelectedInventario.ID_PRODUCTO,
                    ID_UNIDAD_MEDIDA=SelectedInventario.ID_UNIDAD_MEDIDA,
                    NOMBRE=SelectedInventario.NOMBRE,
                    ORDENADO=0,
                    PRESENTACION=SelectedInventario.PRESENTACION,
                    UNIDAD_MEDIDA=SelectedInventario.UNIDAD_MEDIDA
                });
                Productos_Traspaso = new ObservableCollection<EXT_PRODUCTOS_TRASPASO>(temp.Clone());
                SelectedInventario.Seleccionado = true;
            }
        }
    }
}

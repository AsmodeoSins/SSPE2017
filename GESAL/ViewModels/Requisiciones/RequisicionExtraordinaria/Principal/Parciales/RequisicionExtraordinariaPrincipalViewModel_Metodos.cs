using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GESAL.Views;
using MVVMShared.ViewModels;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Almacenes;
using GESAL.Clases.ExtensionesClases;
using GESAL.Clases.Extendidas;
using SSP.Controlador.Principales.Almacenes;
using SSP.Servidor.ModelosExtendidos;
using System.Windows;
namespace GESAL.ViewModels
{
    public partial class RequisicionExtraordinariaPrincipalViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        public async void OnLoad(object sender)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(()=>{
                    CargarDatos(-1, -1, -1, -1);
                })
                .ContinueWith((prevTask) => {
                    StaticSourcesViewModel.SourceChanged = false;
                    selectedMunicipioValue = -1;
                    RaisePropertyChanged("SelectedMunicipioValue");
                    selectedCentroValue = -1;
                    RaisePropertyChanged("SelectedCentroValue");
                    selectedAlmacen_Tipo_CatValue = -1;
                    RaisePropertyChanged("SelectedAlmacen_Tipo_CatValue");
                    selectedAlmacenPrincipalValue = -1;
                    RaisePropertyChanged("SelectedAlmacenPrincipalValue");
                    isRequisicionHabilitado = false;
                    RaisePropertyChanged("IsRequisicionHabilitado");
                    isCantidadesRequeridasValidas = true;
                    RaisePropertyChanged("IsCantidadesRequeridasValidas");
                    isProductosRequisicionValido = true;
                    RaisePropertyChanged("IsProductosRequisicionValido");
                    salvarHabilitado = false;
                    RaisePropertyChanged("SalvarHabilitado");
                    switch (_usuario.ROL)
                    {
                        case "ADMINISTRADOR DE CENTRO":
                            IsTraspasoExternoHabilitado = false;
                            IsOCExtraordinariaHabilitado = false;
                            AgregarHabilitado = true;
                            break;
                        case "ADMINISTRADOR CENTRAL":
                            IsTraspasoExternoHabilitado = true;
                            IsOCExtraordinariaHabilitado = true;
                            AgregarHabilitado = false;
                            break;
                    }
                });
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
            else 
            {
                var _window = (RequisicionExtraordinariaPrincipalView)sender;
                _window.VW_RequisicionExtraordinaria.Loaded += VW_RequisicionExtraordinaria_Loaded;
                _window.VW_TraspasosExternos.Loaded += VW_TraspasosExternos_Loaded;
            }
        }

        private async void BusquedaPopUpSwitch(object parametro)
        {
            if (parametro!=null)
            {
                switch(parametro.ToString())
                {
                    case "seleccionarRequisicion":
                        SelectedRequisicionExtraordinaria = SelectedRequisicionExtraordinariaPop_Up;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_REQUISICION_ESTATUS);
                        switch (SelectedTabIndexTipoOperacion)
                        {
                            case 0:
                                await VaciarRequisicion(selectedRequisicionExtraordinaria);
                                setValidationRulesRequisicionExtraordinaria();
                                TituloDatosRequisicion = "Datos de la Requisición - Folio " + selectedRequisicionExtraordinaria.ID_REQUISICION.ToString();
                                EditarHabilitado = true;
                                IsRequisicionHabilitado = false;
                                IsDatosCentroRequisicionHabilitado = false;
                                SalvarHabilitado = false;
                                CancelarHabilitado = false;
                                switch (_usuario.ROL)
                                {
                                    case "ADMINISTRADOR DE CENTRO":
                                        AgregarHabilitado = true;
                                        break;
                                    case "ADMINISTRADOR CENTRAL":
                                        AgregarHabilitado = false;
                                        break;
                                }
                                
                                if (selectedRequisicionExtraordinaria.ESTATUS == "AE")
                                    ImprimirHabilitado = true;
                                else
                                    ImprimirHabilitado = false;
                                break;
                            case 1:
                                IsBusquedaProductosTraspasosVisible = true;
                                SelectedTipoBusqueda = TiposBusqueda.First(w => w.CLAVE == 0);
                                await CargarDatosRequisicionTraspaso(SelectedRequisicionExtraordinaria);
                                ValidarTraspasoExterno();
                                break;
                        }
                        
                        break;
                    case "cancelarRequisicion":
                        switch (selectedTabIndexTipoOperacion)
                        {
                            case 0:
                                setValidationRulesRequisicionExtraordinaria();
                                break;
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_REQUISICION_ESTATUS);
                        break;
                }
            }
        }

        private async void AccionMenuSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "agregar":
                        switch (SelectedTabIndexTipoOperacion)
                        {
                            case 0: //si esta en la pestaña de requisicion extraordinaria
                                await ResetFiltrosRequisicion();
                                TituloDatosRequisicion = "Datos de la Requisición";
                                IsRequisicionHabilitado = true;
                                IsDatosCentroRequisicionHabilitado = true;
                                setValidationRulesRequisicionExtraordinaria();
                                IsCantidadesRequeridasValidas = false;
                                IsProductosRequisicionValido = false;
                                selectedRequisicionExtraordinaria = null;
                                break;
                        }
                        EliminarHabilitado = false;
                        AgregarHabilitado = false;
                        CancelarHabilitado = true;
                        SalvarHabilitado = true;
                        ImprimirHabilitado = false;
                        EditarHabilitado = false;
                        break;
                    case "salvar":
                        switch (SelectedTabIndexTipoOperacion)
                        {
                            case 0: //si esta en la pestaña de requisicion extraordinaria
                                await SalvarRequisicion();
                                await ResetFiltrosRequisicion();
                                CondicionesInicialesRequisicion();
                                IsCantidadesRequeridasValidas = true;
                                IsProductosRequisicionValido = true;
                                IsRequisicionHabilitado = false;
                                IsDatosCentroRequisicionHabilitado = false;
                                selectedRequisicionExtraordinaria = null;
                                TituloDatosRequisicion = "Datos de la Requisición";
                                break;
                        }
                        SalvarHabilitado = false;
                        EditarHabilitado = false;
                        EliminarHabilitado = false;
                        ImprimirHabilitado = false;
                        base.ClearRules();
                        break;
                    case "cancelar":
                        switch (SelectedTabIndexTipoOperacion)
                        {
                            case 0: //si esta en la pestaña de requisicion extraordinaria
                                IsRequisicionHabilitado = false;
                                IsDatosCentroRequisicionHabilitado = false;
                                CancelarHabilitado = false;
                                await ResetFiltrosRequisicion();
                                CondicionesInicialesRequisicion();
                                IsCantidadesRequeridasValidas = true;
                                IsProductosRequisicionValido = true;
                                selectedRequisicionExtraordinaria = null;
                                TituloDatosRequisicion = "Datos de la Requisición";
                                break;
                        }
                        SalvarHabilitado = false;
                        EditarHabilitado = false;
                        EliminarHabilitado = false;
                        ImprimirHabilitado = false;
                        base.ClearRules();
                        break;  
                    case "buscar":
                        switch (SelectedTabIndexTipoOperacion)
                        {
                            case 0: //si esta en la pestaña de requisicion extraordinaria
                                setValidationRuleBuscarRequisicion();
                                await BusquedaRequisicion();
                                break;
                        }
                        break;
                    case "editar":
                        setValidationRulesRequisicionExtraordinaria();
                        IsRequisicionHabilitado = true;
                        IsDatosCentroRequisicionHabilitado = false;
                        CancelarHabilitado = true;
                        AgregarHabilitado = false;
                        IsCantidadesRequeridasValidas = true;
                        IsProductosRequisicionValido = true;
                        SalvarHabilitado = true;
                        EditarHabilitado = false;
                        EliminarHabilitado = true;
                        break;
                    case "eliminar":
                        switch (SelectedTabIndexTipoOperacion)
                        {
                            case 0: //si esta en la pestaña de requisicion extraordinaria
                                await CancelarRequisicion();
                                 await ResetFiltrosRequisicion();
                                CondicionesInicialesRequisicion();
                                selectedRequisicionExtraordinaria = null;
                                TituloDatosRequisicion = "Datos de la Requisición";
                                break;
                        }
                        EditarHabilitado = false;
                        EliminarHabilitado = false;
                        AgregarHabilitado = true;
                        CancelarHabilitado = false;
                        ImprimirHabilitado = false;
                        base.ClearRules();
                        break;
                    case "imprimir":
                        var reporte_datos = await CargarDatosReporteRequisicion(selectedRequisicionExtraordinaria.ID_REQUISICION);
                        if (reporte_datos!=null)
                            ReportViewer_Requisicion(reporte_datos);
                        break;
                }
            }
        }

        private void CargarRequisiciones(List<string> estatus, int? id_centro = null, string almacen_grupo = "", short? id_almacen = null)
        {
            try
            {
                requisicionesExtraordinarias = new ObservableCollection<REQUISICION_CENTRO>(new cRequisicion_Centro().SeleccionarPorEstatus(estatus, 2, id_centro, almacen_grupo, id_almacen));
                RaisePropertyChanged("RequisicionesExtraordinarias");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los productos. Favor de contactar al administrador");
            }
        }

        private async void BuscarRequisicion(string folio_requisicion,List<string> estatus, int? id_centro=null, string almacen_grupo="", short? id_almacen=null)
        {
            var _error = false;
            var _datos_reporte = new Datos_Reporte();
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarRequisiciones(estatus, id_centro, almacen_grupo, id_almacen);
                }).ContinueWith((prevTask) => {
                    if (!string.IsNullOrWhiteSpace(folio_requisicion))
                    {
                        requisicionesExtraordinarias = new ObservableCollection<REQUISICION_CENTRO>(requisicionesExtraordinarias.Where(w => w.ID_REQUISICION.ToString().Contains(folio_requisicion)));
                        RaisePropertyChanged("RequisicionesExtraordinarias");
                    }
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar la informacion de las requisiciones. Favor de contactar al administrador");
            }
            else
            {
                setValidationRuleBuscarRequisicion();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA_REQUISICION_ESTATUS);
            }
                
        }

        private void CargarDatos(short id_almacen_tipo, int id_municipio, short id_centro, short id_almacen)
        {
                switch (SelectedTabIndexTipoOperacion)
                {
                    case 0:
                        CargarAlmacen_TipoCat();
                        CargarMunicipios();
                        CargarCentros(id_municipio);
                        CargarAlmacenesPrincipales(id_almacen_tipo, id_centro);
                        CargarProductos(id_almacen);
                        break;
                    case 1:
                        CargarAlmacen_TipoCatOrigen();
                        CargarMunicipiosOrigen();
                        CargarCentrosOrigen(id_municipio);
                        CargarAlmacenesPrincipalesOrigen(id_almacen_tipo, id_centro);
                        //CargarProductosOrigen(id_almacen);
                        break;
                }
        }

        #region Requisicion Extraordinaria

        private async void VW_RequisicionExtraordinaria_Loaded(object sender, RoutedEventArgs e)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    
                    CargarDatos(-1, -1, -1, -1);

                })
            .ContinueWith((prevTask) =>
            {
                StaticSourcesViewModel.SourceChanged = false;
                selectedMunicipioValue = -1;
                RaisePropertyChanged("SelectedMunicipioValue");
                selectedCentroValue = -1;
                RaisePropertyChanged("SelectedCentroValue");
                selectedAlmacen_Tipo_CatValue = -1;
                RaisePropertyChanged("SelectedAlmacen_Tipo_CatValue");
                selectedAlmacenPrincipalValue = -1;
                RaisePropertyChanged("SelectedAlmacenPrincipalValue");
                IsRequisicionHabilitado = false;
                CondicionesInicialesRequisicion();
                isCantidadesRequeridasValidas = true;
                RaisePropertyChanged("IsCantidadesRequeridasValidas");
                isProductosRequisicionValido = true;
                RaisePropertyChanged("IsProductosRequisicionValido");
                salvarHabilitado = false;
                RaisePropertyChanged("SalvarHabilitado");                
            });

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

        private void CondicionesInicialesRequisicion()
        {
            IsRequisicionHabilitado = false;
            switch (_usuario.ROL)
            {
                case "ADMINISTRADOR DE CENTRO":
                    AgregarHabilitado = true;
                    break;
                case "ADMINISTRADOR CENTRAL":
                    AgregarHabilitado = false;
                    break;
            }
        }

        private async Task<Datos_Reporte> CargarDatosReporteRequisicion(int id_requisicion)
        {
            var _error = false;
            var _datos_reporte = new Datos_Reporte();
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                _datos_reporte.datos=await Task<List<EXT_Reporte_RequisicionExtraordinaria>>.Factory.StartNew(() =>
                {
                    return new cRequisicion_Centro_Producto().SeleccionarReporte_RequisicionExtraordinaria(id_requisicion);
                });
                _datos_reporte.encabezado = await Task<List<EXT_RequisicionExtraordinaria_Encabezado>>.Factory.StartNew(() => {
                    return new cRequisicion_Centro().SeleccionarEncabezadoReporteRequisicionExtraordinaria(id_requisicion);
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar la informacion del reporte. Favor de contactar al administrador");
                return null;
            }
            else
            {
                return _datos_reporte;
            }
        }

        private void ReportViewer_Requisicion(Datos_Reporte reporte_datos)
        {
            var _reporte = new Contenedor_ReporteView();
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FONDOOBSCURO);
            _reporte.Closed += (s, e) =>
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FONDOOBSCURO);
            };
            _reporte.Show();
            _reporte.Report.LocalReport.ReportPath = "Reportes/Requisicion/RRequisicionExtraordinaria.rdlc";
            _reporte.Report.LocalReport.DataSources.Clear();
            Microsoft.Reporting.WinForms.ReportDataSource rsd1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            rsd1.Name = "DataSet1";
            rsd1.Value = reporte_datos.datos;
            Microsoft.Reporting.WinForms.ReportDataSource rsd2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            rsd2.Name = "DataSet2";
            rsd2.Value = reporte_datos.encabezado;
            _reporte.Report.LocalReport.DataSources.Add(rsd1);
            _reporte.Report.LocalReport.DataSources.Add(rsd2);
            _reporte.Report.RefreshReport();
        }

        private void CargarProductos(short id_almacen,string producto_nombre="")
        {
            try
            {
                if (id_almacen != -1)
                    productos = new cVista_Inventario_Producto().Seleccionar(id_almacen,producto_nombre).Select(s => new EXT_V_INVENTARIO_PRODUCTO {
                        CANTIDAD=s.CANTIDAD,
                        DESCR=s.DESCR,
                        ID_ALMACEN=s.ID_ALMACEN,
                        ID_PRESENTACION=s.PRESENTACION,
                        ID_PRODUCTO=s.ID_PRODUCTO,
                        ID_UNIDAD_MEDIDA=s.UNIDAD_MEDIDA,
                        NOMBRE=s.NOMBRE,
                        PRESENTACION=s.PRESENTACION_DESCR,
                        Seleccionado=false,
                        TRANSITO=null,
                        TRASPASO=null,
                        UNIDAD_MEDIDA=s.UNIDAD_DESCRIP
                    }).ToList();
                else
                    productos = new List<EXT_V_INVENTARIO_PRODUCTO>();
                //productosCopia = (List<EXT_V_INVENTARIO_PRODUCTO>)productos.Clone();
                RaisePropertyChanged("Productos");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los productos. Favor de contactar al administrador");
            }
        }

        private void OnClickRequisicionSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "buscar_producto":
                        BusquedaProducto();
                        break;
                    case "agregar_producto":
                        AgregarProducto();
                        break;
                    case "quitar_producto":
                        QuitarProducto();
                        break;
                }
            }
        }

        private async void BusquedaProducto()
        {
            var _error=false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    if (!string.IsNullOrWhiteSpace(BuscarProducto))
                        CargarProductos(SelectedAlmacenPrincipalValue, BuscarProducto);
                    else
                        CargarProductos(SelectedAlmacenPrincipalValue);
                })
                .ContinueWith((prevTask) => {
                    MarcarProductosInventarioRequisicion();
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch(Exception ex)
            {
                _error=true;
            }
            if(_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this,"Error","Hubo un error al cargar el inventario. Favor de contactar al administrador");
            }
        }

        private void MarcarProductosInventarioRequisicion()
        {
            if (seleccionadosProductos!=null)
            {
                foreach (var item in seleccionadosProductos)
                {
                    var temp = productos.FirstOrDefault(w => w.ID_PRODUCTO == item.ID_PRODUCTO);
                    if(temp!=null)
                        temp.Seleccionado=true;
                }
                RaisePropertyChanged("Productos");
            }
        }

        private void AgregarProducto()
        {
            if (SelectedProducto != null && SelectedProducto.Seleccionado != true)
            {
                ObservableCollection<EXT_PRODUCTO_REQUISICION> temp = new ObservableCollection<EXT_PRODUCTO_REQUISICION>(); ;
                if (ProductosRequisicion != null && ProductosRequisicion.Count()>0)
                    temp = new ObservableCollection<EXT_PRODUCTO_REQUISICION>(ProductosRequisicion.Clone());
                else
                    seleccionadosProductos = new List<EXT_V_INVENTARIO_PRODUCTO>();
                temp.Add(new EXT_PRODUCTO_REQUISICION
                {
                    ID_PRESENTACION = SelectedProducto.ID_PRESENTACION,
                    ID_PRODUCTO = SelectedProducto.ID_PRODUCTO,
                    ID_UNIDAD_MEDIDA = SelectedProducto.ID_UNIDAD_MEDIDA,
                    NOMBRE = SelectedProducto.NOMBRE,
                    ORDENADO=0,
                    PRESENTACION = SelectedProducto.PRESENTACION,
                    Seleccionado=false,
                    UNIDAD_MEDIDA = SelectedProducto.UNIDAD_MEDIDA
                });
                ProductosRequisicion = new ObservableCollection<EXT_PRODUCTO_REQUISICION>(temp.Clone());
                SelectedProducto.Seleccionado = true;
                productos.First(f => f.ID_PRODUCTO == SelectedProducto.ID_PRODUCTO).Seleccionado = true;
                seleccionadosProductos.Add(productos.First(f => f.ID_PRODUCTO == SelectedProducto.ID_PRODUCTO));
                ValidarProductoRequisicion();
            }
        }

        private void QuitarProducto()
        {
            if (SelectedProductoRequisicion != null)
            {
                ObservableCollection<EXT_PRODUCTO_REQUISICION> temp = new ObservableCollection<EXT_PRODUCTO_REQUISICION>(); ;
                if (ProductosRequisicion != null)
                {
                    var item_producto = Productos.First(f => f.ID_PRODUCTO == SelectedProductoRequisicion.ID_PRODUCTO);
                    item_producto.Seleccionado = false;
                    seleccionadosProductos.Remove(seleccionadosProductos.First(w=>w.ID_PRODUCTO==item_producto.ID_PRODUCTO));
                    temp = new ObservableCollection<EXT_PRODUCTO_REQUISICION>(ProductosRequisicion.Clone());
                    temp.Remove(temp.First(f => f.ID_PRODUCTO == SelectedProductoRequisicion.ID_PRODUCTO));
                    ProductosRequisicion = new ObservableCollection<EXT_PRODUCTO_REQUISICION>(temp.Clone());
                    ValidarProductoRequisicion();
                }
                    
            }
        }

        private void ValidarProductoRequisicion()
        {
            if (ProductosRequisicion != null && ProductosRequisicion.Count > 0)
                isProductosRequisicionValido = true;
            else
                isProductosRequisicionValido = false;
            RaisePropertyChanged("IsProductosRequisicionValido");
        }

        private void ValidarCantidadesRequeridad(object parametro)
        {
            IsCantidadesRequeridasValidas = !ProductosRequisicion.Any(a=>a.IsOrdenadoValido==false);
        }

        private async Task BusquedaRequisicion()
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    switch (_usuario.ROL)
                    {
                        case "ADMINISTRADOR DE CENTRO":
                            CargarRequisiciones(new List<string>() {"GE","AC"},_usuario.CENTRO.Value);
                            break;
                        case "ADMINISTRADOR CENTRAL":
                            CargarRequisiciones(new List<string>() { "AC", "AE" });
                            break;
                    }
                    
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar las requisiciones. Favor de contactar al administrador");
            }
            else
            {
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA_REQUISICION_ESTATUS);
            }

        }

        public async Task VaciarRequisicion(REQUISICION_CENTRO _requisicion_centro)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarDatos(_requisicion_centro.ALMACEN.ID_PRODUCTO_TIPO.Value, _requisicion_centro.ALMACEN.CENTRO.ID_MUNICIPIO.Value,_requisicion_centro.ALMACEN.ID_CENTRO, 
                        _requisicion_centro.ALMACEN.ID_ALMACEN);
                    ProductosRequisicion=new ObservableCollection<EXT_PRODUCTO_REQUISICION>(_requisicion_centro.REQUISICION_CENTRO_PRODUCTO.Select(s => new EXT_PRODUCTO_REQUISICION {
                        ID_PRESENTACION=s.PRODUCTO.ID_PRESENTACION,
                        ID_PRODUCTO=s.ID_PRODUCTO,
                        ID_UNIDAD_MEDIDA=s.PRODUCTO.ID_UNIDAD_MEDIDA,
                        IsOrdenadoValido=true,
                        NOMBRE=s.PRODUCTO.NOMBRE,
                        ORDENADO=s.CANTIDAD,
                        PRESENTACION=s.PRODUCTO.PRODUCTO_PRESENTACION.DESCR,
                        Seleccionado=false,
                        UNIDAD_MEDIDA=s.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE
                    }));
                })
            .ContinueWith((prevTask) =>
            {
                selectedMunicipioValue = _requisicion_centro.ALMACEN.CENTRO.ID_MUNICIPIO.Value;
                RaisePropertyChanged("SelectedMunicipioValue");
                selectedCentroValue = _requisicion_centro.ALMACEN.ID_CENTRO;
                RaisePropertyChanged("SelectedCentroValue");
                selectedAlmacen_Tipo_CatValue = _requisicion_centro.ALMACEN.ID_PRODUCTO_TIPO.Value;
                RaisePropertyChanged("SelectedAlmacen_Tipo_CatValue");
                selectedAlmacenPrincipalValue = _requisicion_centro.ALMACEN.ID_ALMACEN;
                RaisePropertyChanged("SelectedAlmacenPrincipalValue");                                
                IsCantidadesRequeridasValidas = true;
                IsProductosRequisicionValido = true;
                SeleccionarProductos();
            });

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

        private void SeleccionarProductos()
        {
            seleccionadosProductos = new List<EXT_V_INVENTARIO_PRODUCTO>();
            foreach(var item in ProductosRequisicion)
            {
                productos.First(w => w.ID_PRODUCTO == item.ID_PRODUCTO).Seleccionado = true;
                RaisePropertyChanged("Productos");
                seleccionadosProductos.Add(productos.First(w => w.ID_PRODUCTO == item.ID_PRODUCTO));
            }
        }

        private async Task ResetFiltrosRequisicion()
        {
            await MunicipioCambio(-1)
                .ContinueWith((prevTask) =>
                {
                    SelectedMunicipioValue = -1;
                    SelectedAlmacen_Tipo_CatValue = -1;
                });
        }

        private async Task SalvarRequisicion()
        {
            var _error = false;
            int _id_requisicion = 0;
            StaticSourcesViewModel.ShowProgressLoading("Guardando la requisición", "Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var requisicion_producto = new List<REQUISICION_CENTRO_PRODUCTO>();
                    foreach (var item in ProductosRequisicion)
                        requisicion_producto.Add(new REQUISICION_CENTRO_PRODUCTO
                        {
                            CANTIDAD = Convert.ToInt32(item.ORDENADO.Value),
                            ESTATUS = "PA",
                            ID_PRODUCTO = item.ID_PRODUCTO,
                            ID_REQUISICION = selectedRequisicionExtraordinaria != null ? selectedRequisicionExtraordinaria.ID_REQUISICION : 0
                        });

                    if (selectedRequisicionExtraordinaria == null)
                    {
                        var requisicion_centro = new REQUISICION_CENTRO
                        {
                            ANIO = Convert.ToInt16(DateTime.Now.Year),
                            ESTATUS = "AC",
                            FECHA = DateTime.Now,
                            ID_ALMACEN = SelectedAlmacenPrincipalValue,
                            ID_TIPO = 2,
                            ID_USUARIO = _usuario.Username,
                            MES = Convert.ToInt16(DateTime.Now.Month),
                            REQUISICION_CENTRO_PRODUCTO = requisicion_producto,
                            ID_REQUISICION = selectedRequisicionExtraordinaria != null ? selectedRequisicionExtraordinaria.ID_REQUISICION : 0
                        };
                        _id_requisicion = new cRequisicion_Centro().Insertar(requisicion_centro);
                    }
                    else
                    {
                        switch (_usuario.ROL)
                        {
                            case "ADMINISTRADOR DE CENTRO":
                                new cRequisicion_Centro().Actualizar(requisicion_producto);
                                break;
                            case "ADMINISTRADOR CENTRAL":
                                new cRequisicion_Centro().Actualizar(requisicion_producto, "AE");
                                break;
                        }

                        _id_requisicion = selectedRequisicionExtraordinaria.ID_REQUISICION;
                    }
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Mensaje de Error", "Hubo un error al guardar la requisición");
            }
            else
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Notificación", "La requisición con FOLIO " + _id_requisicion.ToString() + " fue guardada y autorizada con exito.");
                if (_usuario.ROL == "ADMINISTRADOR CENTRAL")
                {
                    var reporte_datos = await CargarDatosReporteRequisicion(_id_requisicion);
                    ReportViewer_Requisicion(reporte_datos);
                }
            }

        }

        private async Task CancelarRequisicion()
        {
            var _error = false;
            int _id_requisicion = selectedRequisicionExtraordinaria.ID_REQUISICION;
            StaticSourcesViewModel.ShowProgressLoading("Cancelando la requisición", "Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    new cRequisicion_Centro().CancelarRequisicionCentro(_id_requisicion);
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Mensaje de Error", "Hubo un error al cancelar la requisición");
            }
            else
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Notificación", "La requisición con FOLIO " + _id_requisicion.ToString() + " fue cancelada con exito.");
            }
        }
        #region Filtros
        private void CargarMunicipios()
        {
            try
            {
                municipios = new ObservableCollection<MUNICIPIO>(new cMunicipio().Obtener(2).ToList());
                var dummy = new MUNICIPIO
                {
                    MUNICIPIO1 = "SELECCIONE UNA",
                    ID_MUNICIPIO = -1
                };
                municipios.Add(dummy);
                RaisePropertyChanged("Municipios");                
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
                var dummy = new CENTRO
                {
                    ID_CENTRO = -1,
                    DESCR = "SELECCIONE UNA"
                };
                centros.Add(dummy);
                RaisePropertyChanged("Centros");                
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
                var dummy = new ALMACEN
                {
                    ID_ALMACEN = -1,
                    DESCRIPCION = "SELECCIONE UNA"
                };
                almacenesPrincipales.Add(dummy);
                RaisePropertyChanged("AlmacenesPrincipales");                
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
                var dummy = new ALMACEN_TIPO_CAT
                {
                    ID_ALMACEN_TIPO = -1,
                    DESCR = "SELECCIONE UNA"
                };
                almacen_Tipos_Cat.Add(dummy);
                RaisePropertyChanged("Almacen_Tipos_Cat");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los tipos de almacén. Favor de contactar al administrador");
            }
        }

        private async void ValidarFiltroRequisicionCambio(object parametro)
        {
            if (parametro!=null && !string.IsNullOrWhiteSpace(parametro.ToString()))
            {
                switch (parametro.ToString())
                {
                    case "cambio_municipio":
                        if (StaticSourcesViewModel.SourceChanged)
                            if (await (_dialogCoordinator.ShowMessageAsync(this, "Advertencia", "Hay cambios sin guardar, ¿Seguro que desea cambiar de municipio sin guardar?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative)) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                            {
                                await MunicipioCambio(selectedMunicipioValue);
                                StaticSourcesViewModel.SourceChanged = false;
                            }
                            else
                            {
                                selectedMunicipioValue = selectedMunicipioOldValue;
                                RaisePropertyChanged("SelectedMunicipioValue");
                            }
                        else
                        {
                            await MunicipioCambio(selectedMunicipioValue);
                        }
                        break;
                    case "cambio_centro":
                        if (StaticSourcesViewModel.SourceChanged)
                            if (await (_dialogCoordinator.ShowMessageAsync(this, "Advertencia", "Hay cambios sin guardar, ¿Seguro que desea cambiar de centro sin guardar?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative)) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                            {
                                await CentroCambio(selectedCentroValue);
                                StaticSourcesViewModel.SourceChanged = false;
                            }
                            else
                            {
                                selectedCentroValue = selectedCentroOldValue;
                                RaisePropertyChanged("SelectedCentroValue");
                            }
                        else
                        {
                            await CentroCambio(selectedCentroValue);
                        }
                        break;
                    case "cambio_almacen_tipo":
                        if (StaticSourcesViewModel.SourceChanged)
                            if (await (_dialogCoordinator.ShowMessageAsync(this, "Advertencia", "Hay cambios sin guardar, ¿Seguro que desea cambiar de tipo de almacén sin guardar?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative)) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                            {
                                await Almacen_TipoCambio(selectedAlmacen_Tipo_CatValue);
                                StaticSourcesViewModel.SourceChanged = false;
                            }
                            else
                            {
                                selectedAlmacen_Tipo_CatValue = selectedAlmacen_Tipo_CatOldValue;
                                RaisePropertyChanged("SelectedAlmacen_Tipo_CatValue");
                            }
                        else
                        {
                            await Almacen_TipoCambio(selectedAlmacen_Tipo_CatValue);
                        }
                        break;
                    case "cambio_almacen":
                        if (StaticSourcesViewModel.SourceChanged)
                            if (await (_dialogCoordinator.ShowMessageAsync(this, "Advertencia", "Hay cambios sin guardar, ¿Seguro que desea cambiar de almacén principal sin guardar?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative)) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                            {
                                await AlmacenPrincipal_Cambio(selectedAlmacenPrincipalValue);
                                StaticSourcesViewModel.SourceChanged = false;
                            }
                            else
                            {
                                selectedAlmacenPrincipalValue = selectedAlmacenPrincipalOldValue;
                                RaisePropertyChanged("SelectedAlmacenPrincipalValue");
                            }
                        else
                        {
                            await AlmacenPrincipal_Cambio(selectedAlmacenPrincipalValue);
                        }
                        break;
                }
            }
        }

        private async Task MunicipioCambio(int id_municipio)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarCentros(id_municipio);
                    CargarAlmacenesPrincipales(selectedAlmacen_Tipo_CatValue, -1);
                    CargarProductos(-1);
                    ProductosRequisicion = new ObservableCollection<EXT_PRODUCTO_REQUISICION>();
                })
                .ContinueWith((prevTask) => {
                    SelectedCentroValue = -1;
                    SelectedAlmacenPrincipalValue = -1;
                });
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

        private async Task CentroCambio(short id_centro)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarAlmacenesPrincipales(selectedAlmacen_Tipo_CatValue, id_centro);
                    CargarProductos(-1);
                    ProductosRequisicion = new ObservableCollection<EXT_PRODUCTO_REQUISICION>();
                })
                .ContinueWith((prevTask) => {
                    selectedAlmacenPrincipalValue = -1;
                    RaisePropertyChanged("SelectedAlmacenPrincipalValue");
                });
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

        private async Task Almacen_TipoCambio(short id_almacen_tipo_cat)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarAlmacenesPrincipales(id_almacen_tipo_cat, selectedCentroValue);
                    CargarProductos(-1);
                    ProductosRequisicion = new ObservableCollection<EXT_PRODUCTO_REQUISICION>();
                }).ContinueWith((prevTask) => {
                    selectedAlmacenPrincipalValue = -1;
                    RaisePropertyChanged("SelectedAlmacenPrincipalValue");
                });
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

        private async Task AlmacenPrincipal_Cambio(short id_almacen)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() => {
                    CargarProductos(id_almacen);
                    ProductosRequisicion = new ObservableCollection<EXT_PRODUCTO_REQUISICION>();
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch(Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this,"Error", "Hubo un error al cargar los productos. Favor de contactar al administrador");
            }
        }
        #endregion
        #endregion

        #region Traspasos Externos
        private async void VW_TraspasosExternos_Loaded(object sender, RoutedEventArgs e)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    IsBusquedaProductosTraspasosVisible = false;
                    IsAlmacenOrigenVisible = false;
                    SelectedRequisicionExtraordinaria = null;
                });
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

        private void OnClickTraspasosSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "buscar_requisicion_traspaso":
                        BuscarRequisicion(BusquedaFolioRequisicionTraspaso, new List<string> {"AE"},null,_usuario.Almacen_Grupo,null);
                        break;
                    case "agregar_producto":
                        AgregarProductoTraspasoExterno();
                        break;
                    case "quitar_producto":
                        QuitarProductoTraspasoExterno();
                        break;
                    case "buscar_producto_traspaso":
                        BusquedaProductoTraspasoExterno();
                        break;
                }
            }
        }

        private async Task CargarDatosRequisicionTraspaso(REQUISICION_CENTRO _requisicion_centro)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() => {
                    CargarInventario_Productos(_requisicion_centro.REQUISICION_CENTRO_PRODUCTO.Select(s => s.ID_PRODUCTO).ToList());
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los datos de la requisición");
            }
        }

        private void CargarInventario_Productos(List<int>productos,short? id_almacen=null, string producto_nombre="")
        {
            try
            {
                if (id_almacen != -1)
                    inventario_productos = new ObservableCollection<EXT_V_INVENTARIO_PRODUCTO>(new cVista_Inventario_Producto().SeleccionarPorProductos(productos,id_almacen,1,producto_nombre)                        
                        .Where(w=>w.ID_ALMACEN!=SelectedRequisicionExtraordinaria.ID_ALMACEN)
                        .Select(s => new EXT_V_INVENTARIO_PRODUCTO
                    {
                        CANTIDAD = s.CANTIDAD,
                        DESCR = s.DESCR,
                        ID_ALMACEN = s.ID_ALMACEN,
                        ID_PRESENTACION = s.PRESENTACION,
                        ID_PRODUCTO = s.ID_PRODUCTO,
                        ID_UNIDAD_MEDIDA = s.UNIDAD_MEDIDA,
                        NOMBRE = s.NOMBRE,
                        PRESENTACION = s.PRESENTACION_DESCR,
                        Seleccionado = false,
                        TRANSITO = null,
                        TRASPASO = null,
                        UNIDAD_MEDIDA = s.UNIDAD_DESCRIP,
                        CENTRO=s.CENTRO
                    }).ToList());
                else
                    inventario_productos = new ObservableCollection<EXT_V_INVENTARIO_PRODUCTO>();
                RaisePropertyChanged("Inventario_Productos");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los productos. Favor de contactar al administrador");
            }
        }

        private void AgregarProductoTraspasoExterno()
        {
            if (SelectedInventario_Producto != null && SelectedInventario_Producto.Seleccionado != true)
            {
                var temp = new ObservableCollection<EXT_PRODUCTO_TRASPASO_EXTERNO>();
                if (ProductosTraspasoExterno != null)
                    temp = new ObservableCollection<EXT_PRODUCTO_TRASPASO_EXTERNO>(ProductosTraspasoExterno.Clone());
                else
                    seleccionadosInventario_Producto = new List<EXT_V_INVENTARIO_PRODUCTO>();
                temp.Add(new EXT_PRODUCTO_TRASPASO_EXTERNO
                {
                    ID_PRESENTACION = SelectedInventario_Producto.ID_PRESENTACION,
                    ID_PRODUCTO = SelectedInventario_Producto.ID_PRODUCTO,
                    ID_UNIDAD_MEDIDA = SelectedInventario_Producto.ID_UNIDAD_MEDIDA,
                    NOMBRE = SelectedInventario_Producto.NOMBRE,
                    TRASPASAR = 0,
                    PRESENTACION = SelectedInventario_Producto.PRESENTACION,
                    Seleccionado = false,
                    UNIDAD_MEDIDA = SelectedInventario_Producto.UNIDAD_MEDIDA,
                    CENTRO=SelectedInventario_Producto.CENTRO,
                    ALMACEN=SelectedInventario_Producto.DESCR,
                    ID_ALMACEN=SelectedInventario_Producto.ID_ALMACEN
                });
                ProductosTraspasoExterno = new ObservableCollection<EXT_PRODUCTO_TRASPASO_EXTERNO>(temp.Clone());
                SelectedInventario_Producto.Seleccionado = true;
                seleccionadosInventario_Producto.Add(SelectedInventario_Producto);
                ValidarTraspasoExterno();
            }
        }

        private void QuitarProductoTraspasoExterno()
        {
            if (SelectedProductoTraspasoExterno != null)
            {
                var temp = new ObservableCollection<EXT_PRODUCTO_TRASPASO_EXTERNO>(); ;
                if (ProductosTraspasoExterno != null)
                {
                    var item_producto = Inventario_Productos.FirstOrDefault(f => f.ID_PRODUCTO == SelectedProductoTraspasoExterno.ID_PRODUCTO);
                    if (item_producto!=null)
                        item_producto.Seleccionado = false;
                    seleccionadosInventario_Producto.Remove(seleccionadosInventario_Producto.First(w => w.ID_PRODUCTO == SelectedProductoTraspasoExterno.ID_PRODUCTO));
                    temp = new ObservableCollection<EXT_PRODUCTO_TRASPASO_EXTERNO>(ProductosTraspasoExterno.Clone());
                    temp.Remove(temp.First(f => f.ID_PRODUCTO == SelectedProductoTraspasoExterno.ID_PRODUCTO));
                    ProductosTraspasoExterno = new ObservableCollection<EXT_PRODUCTO_TRASPASO_EXTERNO>(temp.Clone());
                }
                ValidarTraspasoExterno();
            }
        }

        private async void BusquedaProductoTraspasoExterno()
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    if (!string.IsNullOrWhiteSpace(BuscarProductoTraspaso))
                        CargarInventario_Productos(SelectedRequisicionExtraordinaria.REQUISICION_CENTRO_PRODUCTO.Select(s => s.ID_PRODUCTO).ToList(),
                            selectedTipoBusqueda.CLAVE==0?null:(short?)selectedAlmacenPrincipalOrigenValue,
                            BuscarProductoTraspaso);
                    else
                        CargarInventario_Productos(SelectedRequisicionExtraordinaria.REQUISICION_CENTRO_PRODUCTO.Select(s => s.ID_PRODUCTO).ToList(),
                            selectedTipoBusqueda.CLAVE == 0 ? null : (short?)selectedAlmacenPrincipalOrigenValue); 
                })
                .ContinueWith((prevTask) =>
                {
                    MarcarProductosInventarioTraspasoExterno();
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el inventario. Favor de contactar al administrador");
            }
        }

        private void MarcarProductosInventarioTraspasoExterno()
        {
            if (seleccionadosInventario_Producto != null)
            {
                foreach (var item in seleccionadosInventario_Producto)
                {
                    var temp = inventario_productos.FirstOrDefault(w => w.ID_PRODUCTO == item.ID_PRODUCTO);
                    if (temp != null)
                        temp.Seleccionado = true;
                }
                RaisePropertyChanged("Inventario_Productos");
            }
        }

        private void ValidarTraspasoExterno(object parametro=null)
        {
            mensajeErrorTraspasoExterno = string.Empty;
            isTraspasoExternoValido = true;
            foreach (var item in SelectedRequisicionExtraordinaria.REQUISICION_CENTRO_PRODUCTO)
            {
                if (ProductosTraspasoExterno!=null)
                {
                    var encontro = ProductosTraspasoExterno.Where(w => w.ID_PRODUCTO == item.ID_PRODUCTO);
                    if (encontro.Count()==0)
                    {
                        mensajeErrorTraspasoExterno += "EL PRODUCTO " + item.PRODUCTO.NOMBRE + " ES OBLIGATORIO \n";
                        isTraspasoExternoValido = false;
                    }
                    else
                    {
                        var total = encontro.Select(s => s.TRASPASAR).Sum();
                        if (total!=item.CANTIDAD)
                        {
                            mensajeErrorTraspasoExterno += "EL PRODUCTO " + item.PRODUCTO.NOMBRE + " NO CUMPLE CON LA CANTIDAD REQUERIDA \n";
                            isTraspasoExternoValido = false;
                        }
                    }
                }
                else
                {
                    mensajeErrorTraspasoExterno += "EL PRODUCTO " + item.PRODUCTO.NOMBRE + " ES OBLIGATORIO \n";
                    isTraspasoExternoValido = false;
                }
            }
            var ind = mensajeErrorTraspasoExterno.LastIndexOf("\n");
            mensajeErrorTraspasoExterno = mensajeErrorTraspasoExterno.Remove(ind, 1);
            RaisePropertyChanged("MensajeErrorTraspasoExterno");
            RaisePropertyChanged("IsTraspasoExternoValido");
        }

        private async void ModeloTraspasoChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_tipo":
                        await TipoBusquedaCambio(selectedTipoBusqueda.CLAVE);
                        break;
                    case "cambio_municipio":
                        await MunicipioOrigenCambio(selectedMunicipioOrigenValue);
                        break;
                    case "cambio_centro":
                        await CentroOrigenCambio(selectedCentroOrigenValue);
                        break;
                    case "cambio_almacen_tipos_cat":
                        await Almacen_TipoOrigenCambio(selectedAlmacen_Tipo_CatOrigenValue);
                        break;
                    case "cambio_almacen":
                        await AlmacenPrincipalOrigen_Cambio(selectedAlmacenPrincipalOrigenValue);
                        break;
                }
            }
        }
        #region Filtros de Almacen de Origen

        

        private async Task TipoBusquedaCambio(short id_tipobusqueda)
        {
            if (id_tipobusqueda!=0)
            {
                var _error = false;
                StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        CargarDatos(-1, -1, -1, -1);
                    })
                    .ContinueWith((prevTask) =>
                    {
                        selectedMunicipioOrigenValue = -1;
                        RaisePropertyChanged("SelectedMunicipioOrigenValue");
                        selectedCentroOrigenValue = -1;
                        RaisePropertyChanged("SelectedCentroOrigenValue");
                        selectedAlmacen_Tipo_CatOrigenValue = -1;
                        RaisePropertyChanged("SelectedAlmacen_Tipo_CatOrigenValue");
                        selectedAlmacenPrincipalOrigenValue = -1;
                        RaisePropertyChanged("SelectedAlmacenPrincipalOrigenValue");
                        inventario_productos = new ObservableCollection<EXT_V_INVENTARIO_PRODUCTO>();
                        RaisePropertyChanged("Inventario_Productos");
                    });
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
                IsAlmacenOrigenVisible = true;
            }
            else
            {
                BusquedaProductoTraspasoExterno();
                IsAlmacenOrigenVisible = false;
            }
            
        }

        private void CargarMunicipiosOrigen()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate {
                    municipiosOrigen = new ObservableCollection<MUNICIPIO>(new cMunicipio().Obtener(2).ToList());
                    var dummy = new MUNICIPIO
                    {
                        MUNICIPIO1 = "SELECCIONE UNA",
                        ID_MUNICIPIO = -1
                    };
                    municipiosOrigen.Add(dummy);
                    RaisePropertyChanged("MunicipiosOrigen");
                }));
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
                Application.Current.Dispatcher.Invoke((Action)(delegate
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
                }));
                
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
                Application.Current.Dispatcher.Invoke((Action)(delegate
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
                }));
                
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
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    almacen_Tipos_CatOrigen = new ObservableCollection<ALMACEN_TIPO_CAT>(new cAlmacen_Tipo_Cat().Seleccionar(_usuario.Almacen_Grupo, "S"));
                    var dummy = new ALMACEN_TIPO_CAT
                    {
                        ID_ALMACEN_TIPO = -1,
                        DESCR = "SELECCIONE UNA"
                    };
                    almacen_Tipos_CatOrigen.Add(dummy);
                    RaisePropertyChanged("Almacen_Tipos_CatOrigen");
                }));
                
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los tipos de almacén. Favor de contactar al administrador");
            }
        }

        private async Task MunicipioOrigenCambio(int id_municipio)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarCentrosOrigen(id_municipio);
                    CargarAlmacenesPrincipalesOrigen(selectedAlmacen_Tipo_CatOrigenValue, -1);
                    CargarInventario_Productos(SelectedRequisicionExtraordinaria.REQUISICION_CENTRO_PRODUCTO.Select(s => s.ID_PRODUCTO).ToList(), selectedAlmacenPrincipalOrigenValue);
                    //ProductosRequisicion = new ObservableCollection<EXT_PRODUCTO_REQUISICION>();
                })
                .ContinueWith((prevTask) =>
                {
                    SelectedCentroOrigenValue = -1;
                    SelectedAlmacenPrincipalOrigenValue = -1;
                    MarcarProductosInventarioTraspasoExterno();
                });
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

        private async Task CentroOrigenCambio(short id_centro)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarAlmacenesPrincipalesOrigen(selectedAlmacen_Tipo_CatOrigenValue, id_centro);
                    CargarInventario_Productos(SelectedRequisicionExtraordinaria.REQUISICION_CENTRO_PRODUCTO.Select(s => s.ID_PRODUCTO).ToList(), selectedAlmacenPrincipalOrigenValue);
                    //ProductosRequisicion = new ObservableCollection<EXT_PRODUCTO_REQUISICION>();
                })
                .ContinueWith((prevTask) =>
                {
                    selectedAlmacenPrincipalOrigenValue = -1;
                    RaisePropertyChanged("SelectedAlmacenPrincipalOrigenValue");
                    MarcarProductosInventarioTraspasoExterno();
                });
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

        private async Task Almacen_TipoOrigenCambio(short id_almacen_tipo_cat)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarAlmacenesPrincipalesOrigen(id_almacen_tipo_cat, selectedCentroOrigenValue);
                    CargarInventario_Productos(SelectedRequisicionExtraordinaria.REQUISICION_CENTRO_PRODUCTO.Select(s => s.ID_PRODUCTO).ToList(), selectedAlmacenPrincipalOrigenValue);
                    //ProductosRequisicion = new ObservableCollection<EXT_PRODUCTO_REQUISICION>();
                }).ContinueWith((prevTask) =>
                {
                    selectedAlmacenPrincipalOrigenValue = -1;
                    RaisePropertyChanged("SelectedAlmacenPrincipalOrigenValue");
                    MarcarProductosInventarioTraspasoExterno();
                });
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

        private async Task AlmacenPrincipalOrigen_Cambio(short id_almacen)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarInventario_Productos(SelectedRequisicionExtraordinaria.REQUISICION_CENTRO_PRODUCTO.Select(s => s.ID_PRODUCTO).ToList(), id_almacen);
                })
                .ContinueWith((prevTask) => {
                    MarcarProductosInventarioTraspasoExterno();
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los productos. Favor de contactar al administrador");
            }
        }

        #endregion

        
        #endregion


    }
}

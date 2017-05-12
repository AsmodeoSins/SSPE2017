using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Almacenes;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    public partial class CatalogoMedicamentosViewModel:ValidationViewModelBase
    {
        #region Generales
        public async void CatalogoMedicamentosLoad(CatalogoMedicamentosView Window = null)
        {
            try
            {
                BuscarEnfermedades();
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                    CargarPresentacion_Medicamento(true);
                    CargarForma_Farmaceutica(true);
                    CargarProducto_Unidad_Medida(true);
                    CargarProductoCategorias(true);
                    CargarProductoSubcategorias(selectedProductoCategoriaValue, true);
                });
                setValidacionRule();
                StaticSourcesViewModel.SourceChanged = false;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al cargar la vista.", ex);
            }
        }

        private async void ClickSwitch(object parametro)
        {
            if (parametro!=null && !string.IsNullOrWhiteSpace(parametro.ToString()))
                switch (parametro.ToString())
                {
                    case "buscar":
                        BuscarEnfermedades();
                        break;
                    case "menu_agregar":
                        modo_seleccionado = MODO_OPERACION.INSERTAR;
                        Limpiar();
                        AgregarVisible = true;
                        _selectedEstatusValue = "S";
                        RaisePropertyChanged("SelectedEstatusValue");
                        selectedPresentacion_MedicamentoValue = -1;
                        RaisePropertyChanged("SelectedPresentacion_MedicamentoValue");
                        selectedFormaFarmaceuticaValue = -1;
                        RaisePropertyChanged("SelectedFormaFarmaceuticaValue");
                        selectedProductoUMValue = -1;
                        RaisePropertyChanged("SelectedProductoUMValue");
                        selectedProductoCategoriaValue = -1;
                        RaisePropertyChanged("SelectedProductoCategoriaValue");
                        if (lstProductoSubcategoria != null && lstProductoSubcategoria.Count > 0)
                        {
                            IsSubcategoriaVisible = true;
                            selectedProductoSubcategoriaValue = -1;
                            RaisePropertyChanged("SelectedProductoSubcategoriaValue");
                            AddRule(()=>SelectedProductoSubcategoriaValue,()=>SelectedProductoSubcategoriaValue!=-1,"SUBCATEGORIA ES REQUERIDA!");
                            RaisePropertyChanged("SelectedProductoSubcategoriaValue");
                        }
                        else
                        {
                            IsSubcategoriaVisible = false;
                            RemoveRule("SelectedProductoSubcategoriaValue");
                        }
                        GuardarMenuEnabled = true;
                        CancelarMenuEnabled = true;
                        EditarMenuEnabled = false;
                        AgregarMenuEnabled = false;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "agregar_presentación":
                        if (SelectedPresentacion_MedicamentoValue==-1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una presentación de medicamento");
                            return;
                        }
                        if(lstPresentacion_Medicamentos_Asignadas!=null && lstPresentacion_Medicamentos_Asignadas.Any(a=>a.ID_PRESENTACION_MEDICAMENTO==selectedPresentacion_MedicamentoValue))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La presentación ya se encuentra asignada al medicamento");
                            return;
                        }
                        if (lstPresentacion_Medicamentos_Asignadas == null)
                            lstPresentacion_Medicamentos_Asignadas = new ObservableCollection<PRESENTACION_MEDICAMENTO>();
                        lstPresentacion_Medicamentos_Asignadas.Add(lstPresentacion_Medicamento.FirstOrDefault(w=>w.ID_PRESENTACION_MEDICAMENTO==selectedPresentacion_MedicamentoValue));
                        RaisePropertyChanged("LstPresentacion_Medicamentos_Asignadas");
                        break;
                    case "eliminar_presentacion_medicamento_asignado":
                        if (LstPresentacion_Medicamentos_Asignadas==null || LstPresentacion_Medicamentos_Asignadas.Count==0)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "No hay presentaciones asignadas al medicamento");
                            return;
                        }
                        if (SelectedPresentacion_Medicamento_Asignadas==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una presentación de medicamento asignada");
                            return;
                        }
                        LstPresentacion_Medicamentos_Asignadas.Remove(LstPresentacion_Medicamentos_Asignadas.FirstOrDefault(f => f.ID_PRESENTACION_MEDICAMENTO == SelectedPresentacion_Medicamento_Asignadas.ID_PRESENTACION_MEDICAMENTO));
                        LstPresentacion_Medicamentos_Asignadas = new ObservableCollection<PRESENTACION_MEDICAMENTO>(LstPresentacion_Medicamentos_Asignadas); 
                        break;
                    case "menu_guardar":
                        if (HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios. \n\n" + base.Error);
                            return;
                        }
                        Guardar();
                        break;
                    case "menu_cancelar":
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "Existen cambios sin guardar,¿desea cancelar la operación?") != 1)
                                return;
                        }
                        AgregarVisible = false;
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        IsMedicamentosEnabled = true;
                        Limpiar();
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "menu_editar":
                        if (SelectedProducto == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccionar un producto para editar");
                            return;
                        }
                        Limpiar();
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            _selectedEstatusValue = selectedProducto.ACTIVO;
                            RaisePropertyChanged("SelectedEstatusValue");
                            selectedProductoUMValue = selectedProducto.ID_UNIDAD_MEDIDA.Value;
                            RaisePropertyChanged("SelectedProductoUMValue");
                            selectedFormaFarmaceuticaValue = SelectedProducto.ID_FORMA_FARM.Value;
                            RaisePropertyChanged("SelectedFormaFarmaceuticaValue");
                            selectedProductoCategoriaValue = SelectedProducto.ID_CATEGORIA.Value;
                            RaisePropertyChanged("SelectedProductoCategoriaValue");
                            CargarProductoSubcategorias(selectedProductoCategoriaValue, true);
                            if (lstProductoSubcategoria != null && lstProductoSubcategoria.Count > 0)
                            {
                                IsSubcategoriaVisible = true;
                                selectedProductoSubcategoriaValue = selectedProducto.ID_SUBCATEGORIA.Value;
                                RaisePropertyChanged("SelectedProductoSubcategoriaValue");
                                AddRule(() => SelectedProductoSubcategoriaValue, () => SelectedProductoSubcategoriaValue != -1, "SUBCATEGORIA ES REQUERIDA!");
                                RaisePropertyChanged("SelectedProductoSubcategoriaValue");
                            }
                            else
                            {
                                IsSubcategoriaVisible = false;
                                RemoveRule("SelectedProductoSubcategoriaValue");
                            }
                            textMedicamento = selectedProducto.NOMBRE.Trim();
                            RaisePropertyChanged("TextMedicamento");
                            textDescripcion = selectedProducto.DESCRIPCION.Trim();
                            RaisePropertyChanged("TextDescripcion");
                            selectedPresentacion_MedicamentoValue = -1;
                            RaisePropertyChanged("SelectedPresentacion_MedicamentoValue");
                            if (selectedProducto.PRODUCTO_PRESENTACION_MED!=null && selectedProducto.PRODUCTO_PRESENTACION_MED.Count>0)
                            {
                                lstPresentacion_Medicamentos_Asignadas = new ObservableCollection<PRESENTACION_MEDICAMENTO>(selectedProducto.PRODUCTO_PRESENTACION_MED.Select(s => s.PRESENTACION_MEDICAMENTO).ToList());
                                RaisePropertyChanged("lstPresentacion_Medicamentos_Asignadas");
                            }
                            IsMedicamentosEnabled = false;
                            AgregarVisible = true;
                            GuardarMenuEnabled = true;
                            CancelarMenuEnabled = true;
                            EditarMenuEnabled = false;
                            AgregarMenuEnabled = false;
                        });

                        modo_seleccionado = MODO_OPERACION.EDICION;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "menu_salir":
                        SalirMenu();
                        break;
                    case "menu_eliminar":
                        if (SelectedProducto == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccionar un producto para editar");
                            return;
                        }
                        modo_seleccionado = MODO_OPERACION.ELIMINAR;
                        Guardar();
                        break;
                }
        }

        

        public async static void SalirMenu()
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?");
                    if (dialogresult != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }

                var metro = Application.Current.Windows[0] as MetroWindow;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                GC.Collect();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir del módulo", ex);
            }
        }

        private void Limpiar()
        {
            
            TextDescripcion = string.Empty;
            TextMedicamento = string.Empty;
            LstPresentacion_Medicamentos_Asignadas = new ObservableCollection<PRESENTACION_MEDICAMENTO>();
        }

        private async void Guardar()
        { 
            try
            {
                if (modo_seleccionado == MODO_OPERACION.INSERTAR)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando medicamento en el catalogo", () =>{
                        var _fecha_servidor = Fechas.GetFechaDateServer;
                        var _presentaciones_asignadas = new List<PRODUCTO_PRESENTACION_MED>();
                        if (lstPresentacion_Medicamentos_Asignadas != null && lstPresentacion_Medicamentos_Asignadas.Count > 0)
                        {
                            _presentaciones_asignadas = lstPresentacion_Medicamentos_Asignadas.Select(s => new PRODUCTO_PRESENTACION_MED
                            {
                                ID_PRESENTACION_MEDICAMENTO = s.ID_PRESENTACION_MEDICAMENTO,
                                ID_USUARIO = GlobalVar.gUsr,
                                FECHA_REGISTRO = _fecha_servidor
                            }).ToList();
                        }
                        var _medicamento = new PRODUCTO
                        {
                            ACTIVO = _selectedEstatusValue,
                            DESCRIPCION = textDescripcion,
                            ID_CATEGORIA = selectedProductoCategoriaValue,
                            ID_FORMA_FARM = selectedFormaFarmaceuticaValue,
                            ID_SUBCATEGORIA = isSubcategoriaVisible ? selectedProductoSubcategoriaValue : (int?)null,
                            ID_UNIDAD_MEDIDA = selectedProductoUMValue,
                            NOMBRE = textMedicamento,
                            PRODUCTO_PRESENTACION_MED = _presentaciones_asignadas.Count > 0 ? _presentaciones_asignadas : null
                        };
                        new cMedicamento().Insertar(_medicamento);
                        return true;
                    }))
                    {
                        ListProductos = new RangeEnabledObservableCollection<PRODUCTO>();
                        ListProductos.InsertRange(await SegmentarResultadoBusqueda());
                        if (ListProductos == null || ListProductos.Count() == 0)
                            EmptyVisible = true;
                        else
                            EmptyVisible = false;
                        AgregarVisible = false;
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        IsMedicamentosEnabled = true;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "El medicamento fue guardado en el catalogo con exito");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else if (modo_seleccionado==MODO_OPERACION.EDICION)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando medicamento en el catalogo", () =>
                    {
                        var _fecha_servidor = Fechas.GetFechaDateServer;
                        var _presentaciones_asignadas = new List<PRODUCTO_PRESENTACION_MED>();
                        if (lstPresentacion_Medicamentos_Asignadas != null && lstPresentacion_Medicamentos_Asignadas.Count > 0)
                        {
                            _presentaciones_asignadas = lstPresentacion_Medicamentos_Asignadas.Select(s => new PRODUCTO_PRESENTACION_MED
                            {
                                ID_PRODUCTO=selectedProducto.ID_PRODUCTO,
                                ID_PRESENTACION_MEDICAMENTO = s.ID_PRESENTACION_MEDICAMENTO,
                                ID_USUARIO = GlobalVar.gUsr,
                                FECHA_REGISTRO = _fecha_servidor
                            }).ToList();
                        }
                        var _medicamento = new PRODUCTO
                        {
                            ID_PRODUCTO=selectedProducto.ID_PRODUCTO,
                            ACTIVO = _selectedEstatusValue,
                            DESCRIPCION = textDescripcion,
                            ID_CATEGORIA = selectedProductoCategoriaValue,
                            ID_FORMA_FARM = selectedFormaFarmaceuticaValue,
                            ID_SUBCATEGORIA = isSubcategoriaVisible ? selectedProductoSubcategoriaValue : (int?)null,
                            ID_UNIDAD_MEDIDA = selectedProductoUMValue,
                            NOMBRE = textMedicamento
                        };
                        new cMedicamento().Editar(_medicamento,_presentaciones_asignadas);
                        return true;
                    }))
                    {
                        ListProductos = new RangeEnabledObservableCollection<PRODUCTO>();
                        ListProductos.InsertRange(await SegmentarResultadoBusqueda());
                        if (ListProductos == null || ListProductos.Count() == 0)
                            EmptyVisible = true;
                        else
                            EmptyVisible = false;
                        AgregarVisible = false;
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        IsMedicamentosEnabled = true;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "El medicamento fue guardado en el catalogo con exito");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else if (modo_seleccionado==MODO_OPERACION.ELIMINAR)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando medicamento en el catalogo", () =>
                    {
                        var _medicamento = new PRODUCTO
                        {
                            ID_PRODUCTO = selectedProducto.ID_PRODUCTO,
                            ACTIVO = "N",
                            DESCRIPCION = selectedProducto.DESCRIPCION,
                            ID_CATEGORIA = selectedProducto.ID_CATEGORIA,
                            ID_FORMA_FARM = selectedProducto.ID_FORMA_FARM,
                            ID_SUBCATEGORIA = selectedProducto.ID_SUBCATEGORIA,
                            ID_UNIDAD_MEDIDA = selectedProducto.ID_UNIDAD_MEDIDA,
                            NOMBRE = selectedProducto.NOMBRE
                        };
                        new cMedicamento().Editar(_medicamento);
                        return true;

                    }))
                    {
                        ListProductos = new RangeEnabledObservableCollection<PRODUCTO>();
                        ListProductos.InsertRange(await SegmentarResultadoBusqueda());
                        if (ListProductos == null || ListProductos.Count() == 0)
                            EmptyVisible = true;
                        else
                            EmptyVisible = false;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "El medicamento fue cambiado a inactivo con exito");
                    }
                        
                }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar en el catalogo de medicamentos.", ex);
            }
        }

        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro!=null && !string.IsNullOrWhiteSpace(parametro.ToString()))
            {
                switch(parametro.ToString())
                {
                    case "cambio_categoria":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                            CargarProductoSubcategorias(selectedProductoCategoriaValue, true);
                            if (lstProductoSubcategoria != null && lstProductoSubcategoria.Count > 0)
                            {
                                IsSubcategoriaVisible = true;
                                selectedProductoSubcategoriaValue = -1;
                                RaisePropertyChanged("SelectedProductoSubcategoriaValue");
                                AddRule(() => SelectedProductoSubcategoriaValue, () => SelectedProductoSubcategoriaValue != -1, "SUBCATEGORIA ES REQUERIDA!");
                                RaisePropertyChanged("SelectedProductoSubcategoriaValue");
                            }
                            else
                            {
                                IsSubcategoriaVisible = false;
                                RemoveRule("SelectedProductoSubcategoriaValue");
                            }
                        });
                        break;
                }
            }
        }
        #endregion

        #region busqueda segmentada
        private async void BuscarEnfermedades()
        {
            try
            {
                ListProductos = new RangeEnabledObservableCollection<PRODUCTO>();
                ListProductos.InsertRange(await SegmentarResultadoBusqueda());
                if (ListProductos == null || ListProductos.Count() == 0)
                    EmptyVisible = true;
                else
                    EmptyVisible = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar las enfermedades.", ex);
            }
        }

        private async Task<List<PRODUCTO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<PRODUCTO>>(() => new ObservableCollection<PRODUCTO>(new cMedicamento().ObtenerTodosPorRango(TextBuscarMedicamento, Pagina)));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de medicamentos.", ex);
                return new List<PRODUCTO>();
            }

        }
        #endregion

        #region Cargar Catalogos
        private void CargarProductoCategorias(bool isExceptionManaged=false )
        {
            try
            {
                lstProductoCategoria = new ObservableCollection<PRODUCTO_CATEGORIA>(new cProducto_Categoria().Seleccionar("M","S"));
                lstProductoCategoria.Insert(0, new PRODUCTO_CATEGORIA {
                    ID_CATEGORIA=-1,
                    NOMBRE="SELECCIONE"
                });
                RaisePropertyChanged("LstProductoCategoria");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de categorias de producto", ex);
            }
        }

        private void CargarProductoSubcategorias(int id_categoria ,bool isExceptionManaged = false)
        {
            try
            {
                lstProductoSubcategoria = new ObservableCollection<PRODUCTO_SUBCATEGORIA>(new cProducto_SubCategoria().Seleccionar(string.Empty,id_categoria,"M", "S"));
                if (selectedProductoCategoriaValue==-1 || (lstProductoSubcategoria!=null &&  lstProductoSubcategoria.Count>0))
                    lstProductoSubcategoria.Insert(0, new PRODUCTO_SUBCATEGORIA
                    {
                        ID_SUBCATEGORIA = -1,
                        DESCR = "SELECCIONE"
                    });
                RaisePropertyChanged("LstProductoSubcategoria");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos de subcategorias de producto", ex);
            }
        }

        private void CargarProducto_Unidad_Medida(bool isExceptionManaged = false)
        {
            try
            {
                lstProductoUM = new ObservableCollection<PRODUCTO_UNIDAD_MEDIDA>(new cProducto_Unidad_Medida().Seleccionar("S"));
                lstProductoUM.Insert(0, new PRODUCTO_UNIDAD_MEDIDA
                {
                    ID_UNIDAD_MEDIDA=-1,
                    NOMBRE = "SELECCIONE"
                });
                RaisePropertyChanged("LstProductoUM");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos de unidades de medida de producto", ex);
            }
        }

        private void CargarForma_Farmaceutica(bool isExceptionManaged = false)
        {
            try
            {
                lstFormaFarmaceutica = new ObservableCollection<FORMA_FARMACEUTICA>(new cForma_Farmaceutica().Seleccionar("S"));
                lstFormaFarmaceutica.Insert(0, new FORMA_FARMACEUTICA
                {
                    ID_FORMA_FARM = -1,
                    DESCR = "SELECCIONE"
                });
                RaisePropertyChanged("LstFormaFarmaceutica");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos de unidades de medida de producto", ex);
            }
        }

        private void CargarPresentacion_Medicamento(bool isExceptionManaged = false)
        {
            try
            {
                lstPresentacion_Medicamento = new ObservableCollection<PRESENTACION_MEDICAMENTO>(new cPresentacion_Medicamento().Seleccionar("S"));
                lstPresentacion_Medicamento.Insert(0, new PRESENTACION_MEDICAMENTO
                {
                    ID_PRESENTACION_MEDICAMENTO = -1,
                    DESCRIPCION = "SELECCIONE"
                });
                RaisePropertyChanged("LstPresentacion_Medicamento");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos de unidades de medida de producto", ex);
            }
        }
        #endregion

        private enum MODO_OPERACION
        {
            INSERTAR = 1,
            EDICION = 2,
            ELIMINAR=3
        }
    }
}

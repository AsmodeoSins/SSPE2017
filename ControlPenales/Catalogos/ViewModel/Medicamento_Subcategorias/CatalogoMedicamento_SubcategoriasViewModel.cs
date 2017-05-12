using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Almacenes;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class CatalogoMedicamento_SubcategoriasViewModel : ValidationViewModelBase
    {
        #region Generales
        public async void CatalogoMedicamento_CategoriasLoad(CatalogoMedicamento_SubcategoriasView Window = null)
        {
            try
            {
                tbSubcategoriaBuscar = Window.tbSubcategoria;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    BuscarProducto_Categorias(string.Empty, true);
                    //ListCategoriaBusqueda = 
                    ListCategoria = new ObservableCollection<PRODUCTO_CATEGORIA>(new cProducto_Categoria().Seleccionar(string.Empty, "S"));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        //ListCategoriaBusqueda.Insert(0, new PRODUCTO_CATEGORIA { DESCR = "SELECCIONE", ID_CATEGORIA = -1 });
                        ListCategoria.Insert(0, new PRODUCTO_CATEGORIA { NOMBRE = "SELECCIONE", ID_CATEGORIA = -1 });
                        SelectCategoriaBusqueda = -1;
                        SelectCategoria = -1;
                    }));
                });

                StaticSourcesViewModel.SourceChanged = false;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al cargar la vista.", ex);
            }
        }

        private async void ClickSwitch(object parametro)
        {
            if (parametro != null && !string.IsNullOrWhiteSpace(parametro.ToString()))
                switch (parametro.ToString())
                {
                    case "buscar":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarProducto_Categorias(TextSubcategoriaBuscar, true);
                        });
                        await System.Threading.Tasks.TaskEx.Delay(500);
                        //FocusBlock = true;
                        tbSubcategoriaBuscar.Focus();
                        break;
                    case "menu_agregar":
                        Limpiar();
                        AgregarVisible = false;
                        GuardarMenuEnabled = true;
                        CancelarMenuEnabled = true;
                        EditarMenuEnabled = false;
                        AgregarMenuEnabled = false;
                        EliminarMenuEnabled = false;
                        AgregarVisible = true;
                        SelectCategoria = -1;
                        TextDescripcion = string.Empty;
                        setValidacionRule();
                        modo_seleccionado = MODO_OPERACION.INSERTAR;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "menu_guardar":
                        if (HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios. \n\n" + base.Error);
                            return;
                        }

                        Guardar();
                        break;
                    case "menu_editar":
                        if (SelectedProducto_Subcategoria == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccionar una subcategoria de medicamento para editar");
                            return;
                        }
                        Limpiar();
                        IsMedicamento_CategoriasEnabled = false;
                        SelectedEstatusValue = SelectedProducto_Subcategoria.ACTIVO;
                        SelectCategoria = SelectedProducto_Subcategoria.ID_CATEGORIA;
                        TextDescripcion = SelectedProducto_Subcategoria.DESCR;
                        GuardarMenuEnabled = true;
                        CancelarMenuEnabled = true;
                        EditarMenuEnabled = false;
                        AgregarMenuEnabled = false;
                        EliminarMenuEnabled = false;
                        AgregarVisible = true;
                        modo_seleccionado = MODO_OPERACION.EDICION;
                        setValidacionRule();
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "menu_cancelar":
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "Existen cambios sin guardar, ¿está seguro que desea cancelar la operación?") != 1)
                                return;
                        }
                        AgregarVisible = false;
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        EliminarMenuEnabled = true;
                        IsMedicamento_CategoriasEnabled = true;
                        Limpiar();
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "menu_eliminar":
                        if (SelectedProducto_Subcategoria == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Selecciona una subcategoria de medicamento para editar");
                            return;
                        }
                        modo_seleccionado = MODO_OPERACION.ELIMINAR;
                        Guardar();
                        break;
                    case "menu_salir":
                        SalirMenu();
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

        private async void Guardar()
        {
            try
            {
                if (modo_seleccionado == MODO_OPERACION.INSERTAR)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la subcategoria del medicamento en el catalogo", () =>
                    {
                        var _producto_categoria = new PRODUCTO_SUBCATEGORIA
                        {
                            ACTIVO = _selectedEstatusValue,
                            ID_CATEGORIA = SelectCategoria,
                            DESCR = TextDescripcion,
                        };
                        var regresa = new cProducto_SubCategoria().Insertar(_producto_categoria);
                        return regresa;
                    }))
                    {
                        AgregarVisible = false;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarProducto_Categorias(TextSubcategoriaBuscar, true);
                        });
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        EliminarMenuEnabled = true;
                        IsMedicamento_CategoriasEnabled = true;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La subcategoria del medicamento fue guardado en el catalogo con exito");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("ADVERTENCIA!", "Ocurrió un error al guardar la información.");
                    }
                }
                else if (modo_seleccionado == MODO_OPERACION.EDICION)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la subcategoria del medicamento en el catalogo", () =>
                    {
                        var _producto_subcategoria = new PRODUCTO_SUBCATEGORIA
                        {
                            ID_CATEGORIA = SelectCategoria,
                            ID_SUBCATEGORIA = SelectedProducto_Subcategoria.ID_SUBCATEGORIA,
                            ACTIVO = _selectedEstatusValue,
                            DESCR = TextDescripcion,
                        };
                        var regresa = new cProducto_SubCategoria().Actualizar(_producto_subcategoria);
                        return regresa;
                    }))
                    {
                        AgregarVisible = false;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarProducto_Categorias(TextSubcategoriaBuscar, true);
                        });
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        EliminarMenuEnabled = true;
                        IsMedicamento_CategoriasEnabled = true;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La subcategoria del medicamento fue guardado en el catalogo con exito");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("ADVERTENCIA!", "Ocurrió un error al guardar la información.");
                    }
                }
                else if (modo_seleccionado == MODO_OPERACION.ELIMINAR)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la subcategoria del medicamento en el catalogo", () =>
                    {
                        var _producto_subcategoria = new PRODUCTO_SUBCATEGORIA
                        {
                            ID_CATEGORIA = SelectedProducto_Subcategoria.ID_CATEGORIA,
                            DESCR = SelectedProducto_Subcategoria.DESCR,
                            ID_SUBCATEGORIA = SelectedProducto_Subcategoria.ID_SUBCATEGORIA,
                            ACTIVO = "N",
                        };
                        var regresa = new cProducto_SubCategoria().Actualizar(_producto_subcategoria);
                        return regresa;
                    }))
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarProducto_Categorias(TextSubcategoriaBuscar, true);
                        });
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La subcategoria del medicamento fue cambiado a inactivo con exito");
                    }

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar en el catalogo de medicamentos.", ex);
            }
        }

        private void Limpiar()
        {
            TextDescripcion = string.Empty;
            SelectCategoria = -1;
        }
        #endregion

        #region Catalogos
        private void BuscarProducto_Categorias(string buscar, bool isExceptionManaged = false)
        {
            try
            {
                var lista = new cProducto_SubCategoria().BuscarSubcategorias(buscar, SelectCategoriaBusqueda);
                ListProducto_Subcategorias = new ObservableCollection<PRODUCTO_SUBCATEGORIA>(lista.Any() ? lista.OrderBy(o => o.ID_CATEGORIA) : lista);
                EmptyVisible = !ListProducto_Subcategorias.Any();
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de subcategorias de producto", ex);
            }
        }
        #endregion

        private enum MODO_OPERACION
        {
            INSERTAR = 1,
            EDICION = 2,
            ELIMINAR = 3
        }
    }
}

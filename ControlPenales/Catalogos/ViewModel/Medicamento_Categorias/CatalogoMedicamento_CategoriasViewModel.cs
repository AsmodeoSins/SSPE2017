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
    public partial class CatalogoMedicamento_CategoriasViewModel:ValidationViewModelBase
    {
        #region Generales
        public async void CatalogoMedicamento_CategoriasLoad(CatalogoMedicamento_CategoriasView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    BuscarProducto_Categorias(string.Empty, true);
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
             if (parametro!=null && !string.IsNullOrWhiteSpace(parametro.ToString()))
                 switch (parametro.ToString())
                 {
                     case "buscar":
                         await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                         {
                             BuscarProducto_Categorias(textBuscarMedicamento_Categoria, true);
                         });
                         break;
                     case "menu_agregar":
                         Limpiar();
                         AgregarVisible = false;
                         GuardarMenuEnabled = true;
                         CancelarMenuEnabled =true;
                         EditarMenuEnabled = false;
                         AgregarMenuEnabled = false;
                         EliminarMenuEnabled = false;
                         AgregarVisible = true;
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
                         if (SelectedProducto_Categoria == null)
                         {
                             new Dialogos().ConfirmacionDialogo("Validación", "Seleccionar una categoria de medicamento para editar");
                             return;
                         }
                         Limpiar();
                         IsMedicamento_CategoriasEnabled = false;
                         await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                             _selectedEstatusValue = selectedProducto_Categoria.ACTIVO;
                             RaisePropertyChanged("SelectedEstatusValue");
                             textMedicamento_Categoria = selectedProducto_Categoria.NOMBRE;
                             RaisePropertyChanged("TextMedicamento_Categoria");
                             textDescripcion = selectedProducto_Categoria.DESCR;
                             RaisePropertyChanged("TextDescripcion");
                         });
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
                                 "Existen cambios sin guardar,¿desea cancelar la operación?") != 1)
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
                         if (SelectedProducto_Categoria == null)
                         {
                             new Dialogos().ConfirmacionDialogo("Validación", "Seleccionar una categoria de medicamento para cambiar estatus a inactivo");
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
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la categoria del medicamento en el catalogo", () =>
                    {
                        var _producto_categoria = new PRODUCTO_CATEGORIA
                        {
                            ACTIVO = _selectedEstatusValue,
                            NOMBRE=TextMedicamento_Categoria,
                            ID_PROD_GRUPO="M",
                            DESCR=TextDescripcion
                        };
                        new cProducto_Categoria().Insertar(_producto_categoria);
                        return true;
                    }))
                    {
                        AgregarVisible = false;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarProducto_Categorias(textBuscarMedicamento_Categoria, true);
                        });
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true; 
                        EliminarMenuEnabled = true;
                        IsMedicamento_CategoriasEnabled = true;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La categoria del medicamento fue guardado en el catalogo con exito");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else if (modo_seleccionado == MODO_OPERACION.EDICION)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la categoria del medicamento en el catalogo", () =>
                    {
                        var _producto_categoria = new PRODUCTO_CATEGORIA
                        {
                            ID_CATEGORIA=selectedProducto_Categoria.ID_CATEGORIA,
                            ACTIVO = _selectedEstatusValue,
                            NOMBRE = TextMedicamento_Categoria,
                            ID_PROD_GRUPO = "M",
                            DESCR = TextDescripcion
                        };
                        new cProducto_Categoria().Actualizar(_producto_categoria);
                        return true;
                    }))
                    {
                        AgregarVisible = false;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarProducto_Categorias(textBuscarMedicamento_Categoria, true);
                        });
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        EliminarMenuEnabled = true;
                        IsMedicamento_CategoriasEnabled = true;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La categoria del medicamento fue guardado en el catalogo con exito");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else if (modo_seleccionado == MODO_OPERACION.ELIMINAR)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando la categoria del medicamento en el catalogo", () =>
                    {
                        var _producto_categoria = new PRODUCTO_CATEGORIA
                        {
                            ID_CATEGORIA = selectedProducto_Categoria.ID_CATEGORIA,
                            ACTIVO = "N",
                            NOMBRE = selectedProducto_Categoria.NOMBRE,
                            ID_PROD_GRUPO = selectedProducto_Categoria.ID_PROD_GRUPO,
                            DESCR = selectedProducto_Categoria.DESCR
                        };
                        new cProducto_Categoria().Actualizar(_producto_categoria);
                        return true;
                    }))
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarProducto_Categorias(textBuscarMedicamento_Categoria, true);
                        });
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La categoria del medicamento fue cambiado a inactivo con exito");
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
            TextMedicamento_Categoria = string.Empty;
        }
        #endregion
        #region Catalogos
        private void BuscarProducto_Categorias(string categoriaBuscar,bool isExceptionManaged = false)
        {
            try
            {
                listProducto_Categorias = new ObservableCollection<PRODUCTO_CATEGORIA>(new cProducto_Categoria().Seleccionar(categoriaBuscar,"M",string.Empty));
                RaisePropertyChanged("ListProducto_Categorias");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de categorias de producto", ex);
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

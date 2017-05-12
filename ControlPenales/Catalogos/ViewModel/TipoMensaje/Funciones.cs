using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;
using System.Windows;
using System.Collections.Generic;
namespace ControlPenales
{
    partial class CatalogoTipoMensajeViewModel
    {
        private void Limpiar()
        {
            SeleccionIndice = -1;
            Prioridad = -1;
            Descripcion = Encabezado = Contenido = string.Empty;
            Color = "#FFFFFF";
            //base.ClearRules();
            //PopularEstatusCombo();
        }

        //private void PopularEstatusCombo()
        //{
        //    var ListaEstatus_ = new Clases.Estatus.EstatusControl().LISTA_ESTATUS;
        //    ListEstatus = new ObservableCollection<ControlPenales.Clases.Estatus.Estatus>(ListaEstatus_);
        //}

        private void PopulateDetalle()
        {
            if (SelectedItem != null)
            {
                Descripcion = SelectedItem.DESCR == null ? SelectedItem.DESCR : SelectedItem.DESCR.TrimEnd();
                Prioridad = SelectedItem.PRIORIDAD;
                Color = SelectedItem.COLOR;
                Encabezado = SelectedItem.ENCABEZADO == null ? SelectedItem.ENCABEZADO : SelectedItem.ENCABEZADO.TrimEnd();
                Contenido = SelectedItem.CONTENIDO == null ? SelectedItem.CONTENIDO : SelectedItem.CONTENIDO.TrimEnd();
                //PopularEstatusCombo();
                SelectedEstatus = ListEstatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();

                #region Roles
                LstMensajeRol = new ObservableCollection<MENSAJE_ROL>();
                if (SelectedItem.MENSAJE_ROL != null) {
                    foreach (var i in SelectedItem.MENSAJE_ROL)
                    {
                        LstMensajeRol.Add(new MENSAJE_ROL()
                        {
                            ID_MEN_TIPO = i.ID_MEN_TIPO,
                            ID_ROL = i.ID_ROL,
                            DESCR = i.DESCR,
                            SISTEMA_ROL = i.SISTEMA_ROL
                        });
                    }
                }
                #endregion
            }
        }

        private void PopulateListado()
        {
            ListItems = new ObservableCollection<MENSAJE_TIPO>(new cTipoMensaje().ObtenerTodos(Busqueda,string.Empty));
            EmptyVisible = ListItems.Count > 0 ? false : true;
        }

        private bool Guardar()
        {
            try
            {
                var obj = new MENSAJE_TIPO();
                obj.DESCR = Descripcion;
                obj.PRIORIDAD = Prioridad;
                obj.COLOR = Color;
                obj.ENCABEZADO = Encabezado;
                obj.CONTENIDO = Contenido;
                obj.ESTATUS = SelectedEstatus.DESCRIPCION == "ACTIVO" ? "S" : "N";
                var lRol = new List<MENSAJE_ROL>();
                if (SelectedItem == null)//INSERT
                {
                    #region Mensaje Rol
                    if (LstMensajeRol != null)
                    {
                        foreach (var x in LstMensajeRol)
                        {
                            lRol.Add(new MENSAJE_ROL() { ID_ROL = x.ID_ROL, DESCR = obj.DESCR});
                        }
                    }
                    obj.MENSAJE_ROL = lRol;
                    #endregion

                    if (new cTipoMensaje().Agregar(obj) > 0)
                        return true;
                }
                else//UPDATE
                {
                    obj.ID_MEN_TIPO = SelectedItem.ID_MEN_TIPO;
                    #region Mensaje Rol
                    if (LstMensajeRol != null)
                    {
                        foreach (var x in LstMensajeRol)
                        {
                            lRol.Add(new MENSAJE_ROL() { ID_MEN_TIPO = obj.ID_MEN_TIPO,ID_ROL = x.ID_ROL, DESCR = obj.DESCR });
                        }
                    }
                    #endregion
                    if (new cTipoMensaje().Actualizar(obj, lRol))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
                return false;
            }
        }

        private bool Eliminar()
        {
            try
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem != null)
                    {
                        if (new cTipoMensaje().Eliminar(SelectedItem))
                            return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        //LOAD
        private async void TipoMensajeLoad(CatalogoTipoMensajeView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    CatalogoHeader = "Tipo de Mensaje";
                    HeaderAgregar = "Agregar Nuevo Tipo de Mensaje";
                    //LLENAR 
                    EditarVisible = false;
                    NuevoVisible = false;
                    AgregarVisible = false;
                    GuardarMenuEnabled = false;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    SeleccionIndice = -1;
                    EmptyVisible = false;
                    ConfiguraPermisos();
                    #region Rol
                    LstRol = new ObservableCollection<SISTEMA_ROL>(new cSistemaRol().ObtenerTodos().Where(w => w.ID_ROL > 0));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstRol.Insert(0, new SISTEMA_ROL() { ID_ROL = -1, DESCR = "SELECCIONE" });
                    }));
                    #endregion
                    PopulateListado();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de mensaje.", ex);
            }
        }

        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_TIPO_MENSAJE.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
                    if (p.CONSULTAR == 1)
                    {
                        BuscarHabilitado = true;
                        TextoHabilitado = true;
                    }
                    if (p.EDITAR == 1)
                        EditarEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
    }
}

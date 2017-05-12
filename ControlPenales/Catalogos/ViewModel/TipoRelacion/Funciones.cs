using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoTipoRelacionViewModel
    {
        //private void LimpiarTipoVisita()
        //{
        //    Descripcion = string.Empty;
        //    SelectedEstatus = null;
        //}

        //private void PopularEstatusCombo()
        //{
        //    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
        //}

        private void Guardar()
        {
            try
            {
                if (Clave != 0)
                {  //Actualizar
                    obj.Actualizar(new TIPO_RELACION
                    {
                        ID_RELACION = SelectedItem.ID_RELACION,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                else
                {   //Agregar
                    obj.Insertar(new TIPO_RELACION
                    {
                        ID_RELACION = 0,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                //Mostrar Listado
                this.GetTipoRelacion();
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                Busqueda = string.Empty;
                SelectedEstatus = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        public async void TipoRelacionLoad(CatalogoSimpleView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<TIPO_RELACION>();
                    CatalogoHeader = "Tipo de Relación";
                    HeaderAgregar = "Agregar Nuevo Tipo de Relación";
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
                    /*MAXLENGTH*/
                    MaxLength = 30;
                    SeleccionIndice = -1;
                    //Obtenemos las Etnias
                    this.GetTipoRelacion();
                    setValidationRules();
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de comportamiento homosexual.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_RELACION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
        #endregion
    }
}

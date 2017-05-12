using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;
namespace ControlPenales
{
    partial class CatalogoSectorClasificacionViewModel
    {
        private void LimpiarSectorClasificacion()
        {
            SeleccionIndice = -1;
            Clave = 0;
            Descripcion = Observacion = string.Empty;
            Color = "#FFFFFF";
            ColorFont = "#000000";
            //PopularEstatusCombo();
        }

        //private void PopularEstatusCombo()
        //{
        //    var ListaEstatus_ = new Clases.Estatus.EstatusControl().LISTA_ESTATUS;
        //    ListEstatus = new ObservableCollection<ControlPenales.Clases.Estatus.Estatus>(ListaEstatus_);
        //}

        private void PopulateSectorClasificacion()
        {
            if (SelectedItem != null)
            {
                Descripcion = SelectedItem.POBLACION == null ? SelectedItem.POBLACION : SelectedItem.POBLACION.TrimEnd();
                Observacion = SelectedItem.OBSERV == null ? SelectedItem.OBSERV : SelectedItem.OBSERV.TrimEnd();
                Color = SelectedItem.COLOR;
                ColorFont = SelectedItem.COLOR_TEXTO;
                //PopularEstatusCombo();
                SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
            }
        }

        private void PopulateSectorClasificacionListado()
        {
            ListItems = new ObservableCollection<SECTOR_CLASIFICACION>(new cSectorClasificacion().ObtenerTodas(Busqueda));
            EmptyVisible = ListItems.Count > 0 ? false : true;
        }

        private bool GuardarSectorClasificacion()
        {
            try
            {
                var clasif = new SECTOR_CLASIFICACION();
                clasif.POBLACION = Descripcion;
                clasif.OBSERV = Observacion;
                clasif.COLOR = Color;
                clasif.COLOR_TEXTO = ColorFont;
                clasif.ESTATUS = SelectedEstatus.CLAVE;
                clasif.ES_GRUPO_VULNERABLE = GrupoVulnerable ? (short)1 : (short)0;
                if (SelectedItem == null)//INSERT
                {
                    if (new cSectorClasificacion().Agregar(clasif) > 0)
                        return true;
                }
                else//UPDATE
                {
                    clasif.ID_SECTOR_CLAS = SelectedItem.ID_SECTOR_CLAS;
                    if (new cSectorClasificacion().Actualizar(clasif))
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

        private bool EliminarSectorClasificacion()
        {
            try
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.SECTOR_OBSERVACION.Count == 0)
                    {
                        if (new cSectorClasificacion().Eliminar(SelectedItem))
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

        private async void SectorClasificacionLoad(CatalogoSectorClasificacionView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    CatalogoHeader = "Sector Clasificación";
                    HeaderAgregar = "Agregar Nueva Clasificación";
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
                    MaxLength = 14;
                    SeleccionIndice = -1;
                    EmptyVisible = false;
                    PopulateSectorClasificacionListado();
                    setValidationRules();
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de clasificación.", ex);
            }
        }

        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_SECTOR_CLASIFICACION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

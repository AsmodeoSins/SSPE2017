using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Controlador.Catalogo.Justicia;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Windows;
using MahApps.Metro.Controls;

namespace ControlPenales
{
    partial class ConfiguracionDepartamentoAreaTecnicaViewModel:ValidationViewModelBase
    {

        #region General
        private async void ConfiguracionDepartamentoAreaTecnicaLoad(object parametro)
        {
            ConfiguraPermisos();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                CargarAreasTecnicas(true);
            });
        }

        private async void ClickSwitch(object parametro)
        {
            if (parametro.GetType()==typeof(string) && !string.IsNullOrWhiteSpace(parametro.ToString()))
            {
                switch (parametro.ToString())
                {
                    case "editar_area":
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea cambiar de área?") != 1)
                                return;
                        }
                        StaticSourcesViewModel.SourceChanged = false;
                        if (SelectedArea!=null)
                        {
                            selectedAreaValue = SelectedArea.ID_TECNICA;
                            AreaSeleccionada = "ÁREA TÉCNICA SELECCIONADA: " + SelectedArea.DESCR;
                            IsListadoConfiguracionVisible = Visibility.Visible;
                            IsAgregarConfiguracionVisible = Visibility.Collapsed;
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                CargarDepartamentosConfigurados(selectedAreaValue, true);
                            });
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "SELECCIONAR UN ÁREA PARA EDITAR!");
                        break;
                    case "agregar_configuracion":
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea agregar una configuración nueva?") != 1)
                                return;
                        }
                        StaticSourcesViewModel.SourceChanged = false;
                        IsAgregarConfiguracionVisible = Visibility.Visible;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            CargarDepartamentos(true);
                            SetValidacionesGenerales();
                        });
                        break;
                    case "guardar_configuracion":
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                            return;
                        }
                        Guardar();
                        break;
                    case "cancelar_configuracion":
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea cancelar el proceso de agregar configuración?") != 1)
                                return;
                        }
                        StaticSourcesViewModel.SourceChanged = false;
                        IsAgregarConfiguracionVisible = Visibility.Collapsed;
                        break;
                    case "eliminar_configuracion":
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea eliminar el área?") != 1)
                                return;
                        }
                        if (await new Dialogos().ConfirmarEliminar("Mensaje", "¿Esta seguro de cancelar eliminar la configuración?") != 1)
                            return;
                        StaticSourcesViewModel.SourceChanged = false;
                        IsAgregarConfiguracionVisible = Visibility.Collapsed;
                        if (SelectedDepartamentoAreaTecnica!=null)
                            Eliminar(selectedDepartamentoAreaTecnica.ID_TECNICA, selectedDepartamentoAreaTecnica.ID_DEPARTAMENTO);
                        else
                            new Dialogos().ConfirmacionDialogo("Validación","SELECCIONAR UN DEPARTAMENTO PARA ELIMINAR!");
                        break;
                    case "salir_menu":
                        SalirMenu();
                        break;
                }
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

        private async void Eliminar(short id_area_tecnica, short id_departamento)
        {
            try
            {
                if (await StaticSourcesViewModel.OperacionesAsync<bool>("ELIMINANDO CONFIGURACIÓN", () =>
                {
                    new cDepartamentoAreaTecnica().Eliminar(id_area_tecnica, id_departamento);
                    return true;
                }))
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        CargarDepartamentosConfigurados(selectedArea.ID_TECNICA, true);
                    });
                    new Dialogos().ConfirmacionDialogo("EXITO!", "La configuración ha sido eliminada");
                    IsAgregarConfiguracionVisible = Visibility.Collapsed;
                }
            }
            catch(Exception  ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar la configuración", ex);
            }
        }

        private async void Guardar()
        {
            try
            {
                var _departamento_area_tecnica = new DEPARTAMENTO_AREA_TECNICA {
                    ID_DEPARTAMENTO=SelectedDepartamentoValue,
                    ID_TECNICA = selectedAreaValue,
                    REGISTRO_FEC=Fechas.GetFechaDateServer
                };
                if (await StaticSourcesViewModel.OperacionesAsync<bool>("GUARDANDO CONFIGURACIÓN", () => {
                    new cDepartamentoAreaTecnica().Guardar(_departamento_area_tecnica);
                    return true;
                }))
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        CargarDepartamentosConfigurados(selectedArea.ID_TECNICA, true);
                    });
                    StaticSourcesViewModel.SourceChanged = false;
                    new Dialogos().ConfirmacionDialogo("EXITO!", "La configuración ha sido guardada");
                    IsAgregarConfiguracionVisible = Visibility.Collapsed;
                }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la configuración", ex);
            }
        }

        #endregion

        #region Cargar Catalogos
        private void CargarAreasTecnicas(bool isExceptionManaged=false)
        {
            try
            {
                lstAreas = new ObservableCollection<AREA_TECNICA>(new cAreaTecnica().ObtenerTodo(string.Empty,"S"));
                RaisePropertyChanged("LstAreas");
            }
            catch(Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las áreas", ex);
                else
                    throw ex;
            }
        }

        private void CargarDepartamentosConfigurados(short id_area_tecnica,bool isExceptionManaged=false)
        {
            try
            {
                lstDepartamentoAreaTecnica = new ObservableCollection<DEPARTAMENTO_AREA_TECNICA>(new cDepartamentoAreaTecnica().ObtenerTodos(id_area_tecnica));
                RaisePropertyChanged("LstDepartamentoAreaTecnica");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los departamentos configurados", ex);
                else
                    throw ex;
            }
        }

        private void CargarDepartamentos(bool isExceptionManaged = false)
        {
            try
            {
                lstDepartamento = new ObservableCollection<DEPARTAMENTO>(new cDepartamentos().ObtenerTodos(string.Empty, "S"));
                lstDepartamento.Insert(0, new DEPARTAMENTO {
                    DESCR="SELECCIONE",
                    ID_DEPARTAMENTO=-1
                });
                RaisePropertyChanged("LstDepartamento");
                selectedDepartamentoValue = -1;
                RaisePropertyChanged("SelectedDepartamentoValue");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los departamentos", ex);
                else
                    throw ex;
            }
        }
        #endregion

        #region Permisos
        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                MenuInsertarEnabled = false;
                MenuEliminarEnabled = false;
               
                permisos = new cProcesoUsuario().Obtener(enumProcesos.CONFIGURACIONDEPARTAMENTOAREATECNICA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        MenuInsertarEnabled = true;

                    if (p.EDITAR == 1)
                    {
                        MenuEliminarEnabled = true;
                    }

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
        #endregion
    }
}

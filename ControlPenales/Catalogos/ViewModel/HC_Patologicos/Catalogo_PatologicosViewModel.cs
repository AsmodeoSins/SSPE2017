using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class Catalogo_PatologicosViewModel:ValidationViewModelBase
    {
        #region General
        private async void CatalogoPatologicosLoad(Catalogo_PatologicosView Window=null)
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                BuscarPatologicos(string.Empty, true);
                ObtenerSectores_Clasificacion(true);
            });
            setValidacionRule();
            StaticSourcesViewModel.SourceChanged = false;
        }

        private async void ClickSwitch(object parametro)
        {
            if (parametro != null && !string.IsNullOrWhiteSpace(parametro.ToString()))
                switch (parametro.ToString())
                {
                    case "buscar":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarPatologicos(textBuscarPatologico, true);
                        });
                        break;
                    case "menu_agregar":
                        Limpiar();
                        AgregarVisible = true;
                        GuardarMenuEnabled = true;
                        CancelarMenuEnabled = true;
                        EditarMenuEnabled = false;
                        EliminarMenuEnabled = false;
                        AgregarMenuEnabled = false;
                        modo_seleccionado = MODO_OPERACION.INSERTAR;
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
                        EliminarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        IsPatologicosEnabled = true;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "menu_guardar":
                        if(HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios. \n\n" + base.Error);
                            return;
                        }
                        Guardar();
                        break;
                    case "menu_editar":
                        if (SelectedPatologicos == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccionar un antecedente personal patológico para editar");
                            return;
                        }
                        Limpiar();
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            textPatologico = selectedPatologicos.DESCR;
                            RaisePropertyChanged("TextPatologico");
                            isRecuperableChecked = selectedPatologicos.PUEDE_CURARSE.HasValue && selectedPatologicos.PUEDE_CURARSE.Value==1 ? true : false;
                            RaisePropertyChanged("IsRecuperableChecked");
                            _selectedEstatusValue = selectedPatologicos.ESTATUS;
                            RaisePropertyChanged("SelectedEstatusValue");
                            IsPatologicosEnabled = false;
                            AgregarVisible = true;
                            GuardarMenuEnabled = true;
                            CancelarMenuEnabled = true;
                            EditarMenuEnabled = false;
                            AgregarMenuEnabled = false;
                            EliminarMenuEnabled = false;
                            if (selectedPatologicos.SECTOR_CLASIFICACION != null && selectedPatologicos.SECTOR_CLASIFICACION.Count > 0)
                            {
                                foreach (var item in selectedPatologicos.SECTOR_CLASIFICACION)
                                {
                                    if (lstSectorClasificacion.FirstOrDefault(w => w.SECTOR_CLASIFICACION.ID_SECTOR_CLAS == item.ID_SECTOR_CLAS) != null)
                                        lstSectorClasificacion.First(w => w.SECTOR_CLASIFICACION.ID_SECTOR_CLAS == item.ID_SECTOR_CLAS).IS_CHECKED = true;
                                }
                                lstSectorClasificacion = new List<EXT_SECTOR_CLASIFICACION>(lstSectorClasificacion);
                                RaisePropertyChanged("LstSectorClasificacion");
                            }
                        });

                        modo_seleccionado = MODO_OPERACION.EDICION;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "menu_eliminar":
                        if (SelectedPatologicos == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccionar un antecedente personal patológico para cambiar estatus a inactivo");
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

        private void BuscarPatologicos(string patologicoBuscar,bool isExceptionManaged = false)
        {
            try
            {
                listPatologicos = new ObservableCollection<PATOLOGICO_CAT>(new cPatologicoCat().ObtenerTodo(patologicoBuscar));
                RaisePropertyChanged("ListPatologicos");
                if (listPatologicos == null || listPatologicos.Count == 0)
                    EmptyVisible = true;
                else
                    EmptyVisible = false;
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de categorias de producto", ex);
            }
        }

        private void Limpiar()
        {
            TextPatologico = string.Empty;
            IsRecuperableChecked = false;
            foreach (var item in lstSectorClasificacion)
                item.IS_CHECKED = false;
            LstSectorClasificacion = new List<EXT_SECTOR_CLASIFICACION>(lstSectorClasificacion);
        }

        private async void Guardar()
        {
            try
            {
                if (modo_seleccionado == MODO_OPERACION.INSERTAR)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando el antecedente personal patológico en el catalogo", () =>
                    {
                        var _sectores_seleccionados = lstSectorClasificacion.Where(w => w.IS_CHECKED).Select(s => s.SECTOR_CLASIFICACION.ID_SECTOR_CLAS).ToList();
                        var _patologico = new PATOLOGICO_CAT
                        {
                            DESCR=textPatologico,
                            ESTATUS=_selectedEstatusValue,
                            PUEDE_CURARSE=isRecuperableChecked?(short)1:(short)0
                        };
                        new cPatologicoCat().Insertar(_patologico, _sectores_seleccionados);
                        return true;
                    }))
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarPatologicos(textBuscarPatologico, true);
                        });
                        AgregarVisible = false;
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        EliminarMenuEnabled = true;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "El antecedente personal patológico fue guardado en el catalogo con exito");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else if (modo_seleccionado == MODO_OPERACION.EDICION)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando el antecedente personal patológico en el catalogo", () =>
                    {
                        var _sectores_seleccionados = lstSectorClasificacion.Where(w => w.IS_CHECKED).Select(s => s.SECTOR_CLASIFICACION.ID_SECTOR_CLAS).ToList();
                        var _patologico = new PATOLOGICO_CAT
                        {
                            ID_PATOLOGICO = selectedPatologicos.ID_PATOLOGICO,
                            DESCR=textPatologico,
                            ESTATUS=_selectedEstatusValue,
                            PUEDE_CURARSE=isRecuperableChecked?(short)1:(short)0
                        };
                        new cPatologicoCat().Editar(_patologico, _sectores_seleccionados);
                        return true;
                    }))
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarPatologicos(textBuscarPatologico, true);
                        });
                        AgregarVisible = false;
                        GuardarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        EditarMenuEnabled = true;
                        AgregarMenuEnabled = true;
                        EliminarMenuEnabled = true;
                        IsPatologicosEnabled = true;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "El antecedente personal patológico fue editado en el catalogo con exito");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else if (modo_seleccionado == MODO_OPERACION.ELIMINAR)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando el antecedente personal patológico en el catalogo", () =>
                    {
                        var _patologico = new PATOLOGICO_CAT
                        {
                            DESCR=selectedPatologicos.DESCR,
                            ESTATUS="N",
                            ID_PATOLOGICO=selectedPatologicos.ID_PATOLOGICO,
                            PUEDE_CURARSE=selectedPatologicos.PUEDE_CURARSE
                        };
                        new cPatologicoCat().Editar(_patologico);
                        return true;
                    }))
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarPatologicos(textBuscarPatologico, true);
                        });
                        new Dialogos().ConfirmacionDialogo("EXITO!", "El antecedente personal patológico fue cambiado a inactivo con exito");
                    }

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar en el catalogo de enfermedades.", ex);
            }
        }
        #endregion
        #region Catalogos
        private void ObtenerSectores_Clasificacion(bool isExceptionManaged = false)
        {
            try
            {
                lstSectorClasificacion = new cSectorClasificacion().ObtenerGruposVulnerables().Select(s => new EXT_SECTOR_CLASIFICACION
                {
                    IS_CHECKED = false,
                    SECTOR_CLASIFICACION = s
                }).ToList();
                RaisePropertyChanged("LstSectorClasificacion");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar lso catalogos de interconsulta externa", ex);
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

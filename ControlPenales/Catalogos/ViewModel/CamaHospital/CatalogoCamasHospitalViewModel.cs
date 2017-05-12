using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ControlPenales.Clases.Estatus;
namespace ControlPenales
{
    public partial class CatalogoCamasHospitalViewModel : ValidationViewModelBase
    {
        public async void OnLoad(CatalogoCamasHospitalView Window)
        {
            Ventana = Window;
            GuardarMenuEnabled = false;
            AgregarMenuEnabled = true;
            BuscarMenuEnabled = true;
            EliminarMenuEnabled = false;
            EditarMenuEnabled = true;
            CancelarMenuEnabled = false;
            MenuLimpiarEnabled = false;
            AyudaMenuEnabled = false;
            SalirMenuEnabled = true;
            Descripcion = string.Empty;
            SelectedItem = null;
            ListaEstatus.LISTA_ESTATUS.Add(new Estatus()
            {
                CLAVE = SELECCIONAR,
                DESCRIPCION = SELECCIONAR
            });

            ListaEstatusBusqueda.LISTA_ESTATUS.Add(new Estatus()
            {
                CLAVE = SELECCIONAR,
                DESCRIPCION = SELECCIONAR
            });

            ListaEstatusBusqueda.LISTA_ESTATUS.Add(new Estatus()
            {
                CLAVE = TODOS,
                DESCRIPCION = TODOS
            });




            var aux = ListaEstatusBusqueda.LISTA_ESTATUS[0];
            ListaEstatusBusqueda.LISTA_ESTATUS[0] = ListaEstatusBusqueda.LISTA_ESTATUS[2];
            ListaEstatusBusqueda.LISTA_ESTATUS[2] = aux;
            aux = ListaEstatusBusqueda.LISTA_ESTATUS[1];
            ListaEstatusBusqueda.LISTA_ESTATUS[1] = ListaEstatusBusqueda.LISTA_ESTATUS[3];
            ListaEstatusBusqueda.LISTA_ESTATUS[3] = aux;


            aux = ListaEstatus.LISTA_ESTATUS[0];
            ListaEstatus.LISTA_ESTATUS[0] = ListaEstatus.LISTA_ESTATUS[2];
            ListaEstatus.LISTA_ESTATUS[2] = aux;


            SelectedEstatusBusqueda = ListaEstatusBusqueda.LISTA_ESTATUS.Where(w => w.CLAVE == SELECCIONAR).FirstOrDefault();
            SelectedEstatus = ListaEstatus.LISTA_ESTATUS.Where(w => w.CLAVE == SELECCIONAR).FirstOrDefault();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                try
                {
                    ListItems = new cCamaHospital().ObtenerTodos(GlobalVar.gCentro).OrderBy(o => o.ID_CENTRO).ThenBy(t => t.ID_CAMA_HOSPITAL).ToList();

                    EmptyVisible = ListItems.Count == 0;
                    AgregarVisible = false;

                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo cargar el catálogo de camas de hospital", ex);
                    }));
                }
            });
        }

        public async void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "BuscarCamas":
                    // ValidacionEstatusBusqueda();

                    //if (!base.HasErrors)
                    if (SelectedEstatusBusqueda.CLAVE != SELECCIONAR)
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {

                                ListItems = SelectedEstatusBusqueda.CLAVE == TODOS ? new cCamaHospital().ObtenerTodos(GlobalVar.gCentro).OrderBy(o => o.ID_CENTRO).ThenBy(t => t.ID_CAMA_HOSPITAL).ToList() : new cCamaHospital().ObtenerCamasHospitalEstatus(SelectedEstatusBusqueda.CLAVE, GlobalVar.gCentro).ToList();
                                EmptyVisible = ListItems.Count == 0;
                            }
                            catch (Exception ex)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo cargar el catálogo de camas de hospital", ex);
                                }));

                            }
                        });
                    break;
                case "menu_buscar":
                    ValidacionEstatusBusqueda();

                    if (!base.HasErrors)
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {

                                ListItems = SelectedEstatusBusqueda.CLAVE == TODOS ? new cCamaHospital().ObtenerTodos(GlobalVar.gCentro).OrderBy(o => o.ID_CENTRO).ThenBy(t => t.ID_CAMA_HOSPITAL).ToList() : new cCamaHospital().ObtenerCamasHospitalEstatus(SelectedEstatusBusqueda.CLAVE, GlobalVar.gCentro).ToList();
                                EmptyVisible = ListItems.Count == 0;
                            }
                            catch (Exception ex)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo cargar el catálogo de camas de hospital", ex);
                                }));

                            }
                        });

                    break;
                case "menu_guardar":
                    try
                    {

                        var guardar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        ValidacionGuardar();
                        if (!base.HasErrors)
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                var consultor = new cCamaHospital();
                                var cama_hospital = new CAMA_HOSPITAL()
                                {
                                    ID_CENTRO = GlobalVar.gCentro,
                                    DESCR = Descripcion,
                                    ID_USUARIO = GlobalVar.gUsr,
                                    REGISTRO_FEC = Fechas.GetFechaDateServer,
                                    ESTATUS = SelectedEstatus.CLAVE
                                };

                                if (SelectedItem == null)
                                {
                                    guardar = consultor.InsertarCama(GlobalVar.gCentro, cama_hospital);
                                }
                                else
                                {
                                    cama_hospital.ID_CAMA_HOSPITAL = SelectedItem.ID_CAMA_HOSPITAL;//SIN EL IDENTIFICADOR, NO SABE A QUE ELEMENTO ACTUALIZAR DENTRO DE LA BASE DE DATOS
                                    guardar = consultor.ActualizarCama(cama_hospital);
                                }
                            });
                            if (guardar)
                            {
                                await met.ShowMessageAsync("¡ÉXITO!", "Se ha guardado la información de la cama.");
                                AgregarMenuEnabled = BuscarMenuEnabled = Ventana.CatalogoCamasLista.IsEnabled = true;
                                SelectedItem = null;
                                AgregarVisible = CancelarMenuEnabled = GuardarMenuEnabled = EditarMenuEnabled = false;
                            }
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                try
                                {
                                    ListItems = new cCamaHospital().ObtenerTodos(GlobalVar.gCentro).OrderBy(o => o.ID_CENTRO).ThenBy(t => t.ID_CAMA_HOSPITAL).ToList();

                                    EmptyVisible = ListItems.Count == 0;

                                }
                                catch (Exception ex)
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo cargar el catálogo de camas de hospital", ex);
                                    }));

                                }
                            });
                        }
                        else
                        {
                            var mensaje = new StringBuilder();
                            if (string.IsNullOrEmpty(Descripcion))
                                mensaje.Append("DESCRIPCIÓN ES REQUERIDA");
                            if (SelectedEstatus.CLAVE == SELECCIONAR)
                                mensaje.Append(string.Format("{0}", string.IsNullOrEmpty(Descripcion) ? "/" : string.Empty) + "ESTATUS ES REQUERIDO");

                            await met.ShowMessageAsync("Validación", "Faltan datos por capturar: " + mensaje.ToString() + ".");
                        }

                    }
                    catch (Exception ex)
                    {

                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo guardar la información de la nueva cama.", ex);
                    }
                    break;
                case "menu_agregar":
                    ValidacionGuardar();
                    Ventana.CatalogoCamasLista.IsEnabled = false;
                    AgregarVisible = CancelarMenuEnabled = GuardarMenuEnabled = true; ;
                    AgregarMenuEnabled = BuscarMenuEnabled = EditarMenuEnabled = false;
                    Ventana.txtDescripcion.Focus();
                    Descripcion = string.Empty;
                    if (ListaEstatus.LISTA_ESTATUS.Any(a => a.CLAVE == OCUPADA))
                        ListaEstatus.LISTA_ESTATUS.Remove(ListaEstatus.LISTA_ESTATUS.Where(w => w.CLAVE == OCUPADA).FirstOrDefault());
                    SelectedEstatus = ListaEstatus.LISTA_ESTATUS.Where(w => w.CLAVE == SELECCIONAR).FirstOrDefault();
                    break;
                case "menu_editar":
                    Ventana.CatalogoCamasLista.IsEnabled = false;
                    if (SelectedItem == null)
                    {
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una cama primero.");
                    }
                    else
                    {
                        Descripcion = SelectedItem.DESCR.TrimEnd();
                        Ventana.ComboBoxEstatus.IsEnabled = SelectedItem.ESTATUS != OCUPADA;
                        AgregarVisible = CancelarMenuEnabled = GuardarMenuEnabled = true;
                        AgregarMenuEnabled = BuscarMenuEnabled = false;
                        SelectedEstatus = ListaEstatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).FirstOrDefault();
                    }
                    break;
                case "menu_cancelar":

                    AgregarMenuEnabled = BuscarMenuEnabled = Ventana.CatalogoCamasLista.IsEnabled = true;
                    EditarMenuEnabled = SelectedItem != null;
                    AgregarVisible = CancelarMenuEnabled = GuardarMenuEnabled = false;
                    break;
                case "menu_salir":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }
    }
}

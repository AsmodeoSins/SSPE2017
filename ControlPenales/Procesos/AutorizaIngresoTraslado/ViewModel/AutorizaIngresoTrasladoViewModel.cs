using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using SSP.Controlador.Principales.Compartidos;
namespace ControlPenales
{
    partial class AutorizaIngresoTrasladoViewModel : ValidationViewModelBase
    {
        #region constructor
        public AutorizaIngresoTrasladoViewModel() 
        {
        }
        #endregion
        #region metodos
        private async void AutorizaIngresoTrasladoLoad(object parametro)
        {
            try
            {
                Traslados = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EXT_TRASLADO_DETALLE>>(() => {
                    return new ObservableCollection<EXT_TRASLADO_DETALLE>(new cTrasladoDetalle().ObtenerTodosDestino(GlobalVar.gCentro, new List<string> { "AC" })
                        .Select(s => new EXT_TRASLADO_DETALLE { TRASLADO_DETALLE = s, SELECCIONADO = true }));
                });
                if (Traslados.Count() > 0)
                {
                    var permisos = new cProcesoUsuario().Obtener(enumProcesos.TRASLADOMASIVO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                    if (permisos.Any(w => w.INSERTAR == 1))
                        MenuAutorizarEnabled = true;
                    StaticSourcesViewModel.SourceChanged = true;
                }
                    
                else
                {
                    StaticSourcesViewModel.SourceChanged = false;
                    MenuAutorizarEnabled = false;
                }
                    
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los traslados pendientes", ex);
            }
        }

        private async void CambioPropiedad(object parametro)
        {
            if (parametro!=null)
            {
                switch (parametro.ToString())
                {
                    case "selectedtraslado":
                        await StaticSourcesViewModel.CargarDatosAsync<bool>(() =>
                        {
                            if (SelectedTraslado != null)
                            {
                                DT_Fecha = SelectedTraslado.TRASLADO_DETALLE.TRASLADO.TRASLADO_FEC;
                                DT_Autorizado = SelectedTraslado.TRASLADO_DETALLE.TRASLADO.AUTORIZA_TRASLADO;
                                DT_Centro_Origen = new cCentro().Obtener(SelectedTraslado.TRASLADO_DETALLE.TRASLADO.CENTRO_ORIGEN.Value).First().DESCR;
                                DT_Justificacion = SelectedTraslado.TRASLADO_DETALLE.TRASLADO.JUSTIFICACION;
                                DT_Motivo = SelectedTraslado.TRASLADO_DETALLE.TRASLADO.TRASLADO_MOTIVO.DESCR;
                                DT_Oficio_Autorizacion = SelectedTraslado.TRASLADO_DETALLE.TRASLADO.OFICIO_AUTORIZACION;
                                if (SelectedTraslado.TRASLADO_DETALLE.INGRESO.INGRESO_BIOMETRICO
                                    .Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                                    ImagenIngreso = SelectedTraslado.TRASLADO_DETALLE.INGRESO.INGRESO_BIOMETRICO
                                        .Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    ImagenIngreso = new Imagenes().getImagenPerson();
                                DatosTrasladosVisible = Visibility.Visible;
                            }
                            else
                                DatosTrasladosVisible = Visibility.Hidden;
                            return true;
                        });                
                        break;
                    case "salir_menu":
                        SalirMenu();
                        break;
                }
            }
        }

        private void ClickSwitch(object parametro)
        {
            if (parametro !=null)
            {
                switch(parametro.ToString())
                {
                    case "autorizar_menu":
                        if (Traslados.Any(w => w.SELECCIONADO == true))
                            Autorizar();
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Debe de haber por lo menos un traslado seleccionado para autorizar");
                        break;
                }
            }
        }
        private async void Autorizar()
        {
            try
            {
                var id_area_traslado = Parametro.ID_AREA_TRASLADO;
                if (await StaticSourcesViewModel.OperacionesAsync<bool>("Autorizando traslados", () =>
                {
                    var traslados_detalle = new List<TRASLADO_DETALLE>();
                    var fecha = Fechas.GetFechaDateServer;
                    foreach(var item in Traslados)
                    {
                        var _traslado = item.TRASLADO_DETALLE.TRASLADO;
                        if (item.SELECCIONADO)
                            traslados_detalle.Add(new TRASLADO_DETALLE {
                                ID_ESTATUS_ADMINISTRATIVO=item.TRASLADO_DETALLE.ID_ESTATUS_ADMINISTRATIVO,
                                ID_ANIO=item.TRASLADO_DETALLE.ID_ANIO,
                                ID_CENTRO=item.TRASLADO_DETALLE.ID_CENTRO,
                                ID_ESTATUS="FI",
                                ID_IMPUTADO=item.TRASLADO_DETALLE.ID_IMPUTADO,
                                ID_INGRESO=item.TRASLADO_DETALLE.ID_INGRESO,
                                ID_TRASLADO=item.TRASLADO_DETALLE.ID_TRASLADO
                            });
                    }
                    new cTrasladoDetalle().AutorizarTraslado(traslados_detalle,fecha,id_area_traslado,Parametro.UB_RECEPCION_TRASLADO,(short)enumAtencionTipo.CONSULTA_MEDICA,
                        (short)enumAtencionTipo.CONSULTA_DENTAL,GlobalVar.gUsr,"PE");
                    return true;
                }))
                {
                    new Dialogos().ConfirmacionDialogo("EXITO!", "Se autorizaron los traslados");
                    Traslados = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EXT_TRASLADO_DETALLE>>(() =>
                    {
                        return new ObservableCollection<EXT_TRASLADO_DETALLE>(new cTrasladoDetalle().ObtenerTodosDestino(GlobalVar.gCentro, new List<string> { "AC" })
                            .Select(s => new EXT_TRASLADO_DETALLE { TRASLADO_DETALLE = s, SELECCIONADO = true }));
                    });
                    if (Traslados.Count() > 0)
                        StaticSourcesViewModel.SourceChanged = true;
                    else
                    {
                        MenuAutorizarEnabled = false;
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    ImagenIngreso = null;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al autorizar los traslados pendientes", ex);
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
        #endregion

        
    }
}

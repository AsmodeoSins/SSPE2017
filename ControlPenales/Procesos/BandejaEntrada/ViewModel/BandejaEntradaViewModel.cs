using ControlPenales;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class BandejaEntradaViewModel : ValidationViewModelBase// : ValidationViewModelBase,IPageViewModel
    {

        #region Constructor
        public BandejaEntradaViewModel()
        {
            //Notificaciones = new List<cNotificacion>();
            //this.setNotificaciones();
        }
        #endregion

        #region Funciones
        private void setNotificaciones() {
            int x = 51,t = 26;
            if (SelectedItem != null)
            { 
                notificaciones.Clear();
                switch (SelectedItem.Tag.ToString())
                { 
                    case "1":
                        x = 25;
                        break;
                    case "2":
                        t = 1;
                        x = 25;
                        break;
                }
            }
            var hoy = Fechas.GetFechaDateServer;
            int tipo = 1;
            for (int i = 1; i < x; i++)
            {
                if (i == t)
                    tipo = 2;
                Notificaciones.Add(new cNotificacion { Id = i, Notificacion = string.Format("Notificacion de Prueba {0}", i), Fecha = hoy, Tipo = tipo });
            }

            NoNotificaciones = string.Format("{0} Notificaciones", Notificaciones.Count);
        }

        private void orderNotificaciones() {
            int op = 0;

            if (selectedItem2 != null)
                op = Convert.ToInt16(selectedItem2.Tag.ToString());
            switch (op)
            { 
                case 0:
                    Notificaciones = Notificaciones.OrderBy(x => x.Id).ToList();
                break;
                case 1:
                    Notificaciones = Notificaciones.OrderBy(x => x.Fecha).ToList();
                break;
                case 2:
                    Notificaciones = Notificaciones.OrderByDescending(x => x.Tipo).ToList();
                break;
            }
        }

        private void SelectNotificacion(Object obj)
        {
            PopAbierto = true;
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.NOTIFICACION);
        }

        private async void SwitchClick(Object obj)
        {
            switch (obj.ToString())
            {
                case "leer_notificacion":
                    if (MensajeLeido())
                    {
                        PopulateListado();
                        PopAbierto = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.NOTIFICACION);
                    }
                    break;
                case "eliminar_notificacion":
                    if (await new Dialogos().ConfirmarEliminar("Confirmacion!", "Esta seguro que desea eliminar esta notificación.") == 1)
                        if (MensajeEliminado())
                        {
                            PopulateListado();
                            PopAbierto = false;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.NOTIFICACION);
                        }
                    break;
                case "cancelar_notificacion":
                    PopAbierto = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.NOTIFICACION);
                    break;
                case "ver_documento":
                    MostrarDocumento();
                    break;
                case "leidas":
                    if (MensajesLeidos())
                    {
                        PopulateListado();
                    }
                    break;
                case "buscar":
                    PopulateListado();

                    break;
                case "genera_notificacion":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ExcarcelacionView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext =
                        new ExcarcelacionViewModel(enumVentanaOrigen_Excarcelacion.BANDEJA_ENTRADA, decimal.Parse(SelectedMensaje.UsuarioMensaje.MENSAJE.NUC),
                        SelectedMensaje.UsuarioMensaje.MENSAJE.INTER_DOCTO.DOCTO, SelectedMensaje.UsuarioMensaje.MENSAJE.INTER_DOCTO.ID_FORMATO);
                    break;
            }
        }

        public string ConfiguracionEntity()
        {

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return config.ConnectionStrings.ConnectionStrings["SSPEntidades"].ConnectionString;

        }
     
        private async void BandejaEntradaLoad(BandejaEntradaView Window = null)
        {
            try
            {
                var tmp = ConfiguracionEntity();

                //await Task.Factory.StartNew(() =>
                //{
                PopulateListado();
                //});
                aTimer = new System.Timers.Timer();
                aTimer.Elapsed += (s, e) =>
                {
                    if (SelectedVer == 1)
                    {
                        if (LstMensaje != null)
                        {
                            if (LstMensaje.Where(w => w.Seleccionado == true).Count() == 0)
                            {
                                if (!PopAbierto)
                                    PopulateListado();
                            }
                        }
                        else
                        {
                            PopulateListado();
                        }
                    }
                };
                aTimer.Interval = 600000;//se refrescara cada 5 min
                aTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la bandeja de entrada", ex);
            }
        }
        #endregion

        #region Bandeja Entrada
        private void PopulateListado()
        {
            try
            {
                LstMensaje = new ObservableCollection<Mensajes>();
                DateTime f1, f2;
                if (FechaInicio != null)
                {
                    f1 = FechaInicio.Value;
                }
                else
                    f1 = Fechas.GetFechaDateServer;

                if (FechaFin != null)
                    f2 = FechaFin.Value;
                else
                    f2 = Fechas.GetFechaDateServer;

                f1 = new DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new DateTime(f2.Year, f2.Month, f2.Day);
                TimeSpan ts = f2 - f1;
                // Difference in days.
                if (ts.Days == 0)
                {
                    f2 = f2.AddDays(1);
                }
                else if (ts.Days > VNoDias)
                {
                    new Dialogos().ConfirmacionDialogo("Notificacion!", string.Format("El rango de busqueda no puede ser mayor a {0} dias", VNoDias));
                    return;
                }

                var lista = new cUsuarioMensaje().ObtenerTodos(StaticSourcesViewModel.UsuarioLogin.Username, Buscar, f1, f2, SelectedVer, SelectedOrganizar,GlobalVar.gCentro);
                if (lista != null)
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        foreach (var um in lista)
                        {
                            LstMensaje.Add(new Mensajes() { Seleccionado = false, UsuarioMensaje = um, Documento = um.MENSAJE.ID_INTER_DOCTO != null ? true : false });
                        }
                    }));
                }
                NoNotificaciones = string.Format("{0} Notificaciones", LstMensaje.Count);

                MResultados = LstMensaje.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el listado de notificaciones", ex);
            }
        }

        private bool MensajeLeido()
        {
            try
            {
                var obj = new USUARIO_MENSAJE();
                obj.ID_USUARIO = SelectedMensaje.UsuarioMensaje.ID_USUARIO;
                obj.ID_MENSAJE = SelectedMensaje.UsuarioMensaje.ID_MENSAJE;
                obj.LECTURA_FEC = Fechas.GetFechaDateServer;
                obj.ESTATUS = 2;
                if (new cUsuarioMensaje().Actualizar(obj))
                {
                    new Dialogos().ConfirmacionDialogo("Notificación!", "La notificación ha cambiado su estatus a Leida");
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambiar el estatus de la notificación a leido", ex);
            }
            return false;
        }

        private bool MensajesLeidos()
        {
            try
            {
                var list = new List<USUARIO_MENSAJE>();
                var fechaServer = Fechas.GetFechaDateServer;
                foreach (var x in LstMensaje.Where(w => w.Seleccionado == true))
                {
                    list.Add(new USUARIO_MENSAJE()
                    {
                        ID_USUARIO = x.UsuarioMensaje.ID_USUARIO,
                        ID_MENSAJE = x.UsuarioMensaje.ID_MENSAJE,
                        LECTURA_FEC = fechaServer,
                        ESTATUS = 2
                    });
                }
                if (new cUsuarioMensaje().Actualizar(list))
                {
                    new Dialogos().ConfirmacionDialogo("Notificación!", "La(s) notificacion(es) ha(n) cambiado su estatus a Leida");
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambiar el estatus de la(s) notificacion(es) a leido", ex);
            }
            return false;

        }

        private bool MensajeEliminado()
        {
            try
            {
                var obj = new USUARIO_MENSAJE();
                obj.ID_USUARIO = SelectedMensaje.UsuarioMensaje.ID_USUARIO;
                obj.ID_MENSAJE = SelectedMensaje.UsuarioMensaje.ID_MENSAJE;
                obj.ESTATUS = 3;
                if (new cUsuarioMensaje().Actualizar(obj))
                {
                    new Dialogos().ConfirmacionDialogo("Notificación!", "La notificación ha sido Eliminada");
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar la notificación", ex);
            }
            return false;
        }

        private void MostrarDocumento()
        {
            try
            {
                if (SelectedMensaje.UsuarioMensaje.MENSAJE.INTER_DOCTO.ID_FORMATO != 1 && SelectedMensaje.UsuarioMensaje.MENSAJE.INTER_DOCTO.ID_FORMATO != 3 && SelectedMensaje.UsuarioMensaje.MENSAJE.INTER_DOCTO.ID_FORMATO != 4)
                {
                    new Dialogos().ConfirmacionDialogo("Notificación!", "Formato de archivo no válido");
                    return;
                }
                var tc = new TextControlView();
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        switch (SelectedMensaje.UsuarioMensaje.MENSAJE.INTER_DOCTO.ID_FORMATO)
                        {
                            case 1://DOCX
                                tc.editor.Load(SelectedMensaje.UsuarioMensaje.MENSAJE.INTER_DOCTO.DOCTO, TXTextControl.BinaryStreamType.WordprocessingML);
                                break;
                            case 3://PDF
                                tc.editor.Load(SelectedMensaje.UsuarioMensaje.MENSAJE.INTER_DOCTO.DOCTO, TXTextControl.BinaryStreamType.AdobePDF);
                                break;
                            case 4://DOCX
                                tc.editor.Load(SelectedMensaje.UsuarioMensaje.MENSAJE.INTER_DOCTO.DOCTO, TXTextControl.BinaryStreamType.WordprocessingML);
                                break;
                            default:
                                new Dialogos().ConfirmacionDialogo("Notificación!", "Formato de archivo no válido");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
            }
        }
        #endregion
    }

    public class cNotificacion {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private string notificacion;

        public string Notificacion
        {
            get { return notificacion; }
            set { notificacion = value; }
        }
        private DateTime fecha;

        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }

        private int tipo;

        public int Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }
    }
}

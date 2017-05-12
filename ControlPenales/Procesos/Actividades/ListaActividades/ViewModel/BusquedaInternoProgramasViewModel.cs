using ControlPenales.Clases;
using ControlPenales.Clases.ControlProgramas;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using ControlPenales.BiometricoServiceReference;
using System.Threading.Tasks;
using System.Transactions;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace ControlPenales
{
    public partial class BusquedaInternoProgramasViewModel : FingerPrintScanner
    {
        #region Constructor
        public BusquedaInternoProgramasViewModel(BuscarInternosProgramas Window, List<GRUPO_ASISTENCIA> ListaExpedientes, List<EQUIPO_AREA> Areas)
        {
            VentanaBusqueda = Window;
            NIPBuscar = "";
            var placeholder_limpiar = new Imagenes().getImagenPerson();
            SelectedImputadoFotoIngreso = placeholder_limpiar;
            SelectedImputadoFotoSeguimiento = placeholder_limpiar;
            FotoCustodio = placeholder_limpiar;
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            EscoltarEnabled = true;
            SelectedJustificacion = enumTipoSalida.TRASLADO;
            var ListaAux = new List<InternosActividad>();
            foreach (var imputado in ListaExpedientes)
            {
                ListaAux.Add(new InternosActividad()
                {
                    Actividad = imputado.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR,
                    Anio = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                    Area = (short)imputado.GRUPO_HORARIO.ID_AREA,
                    Asistencia = imputado.ASISTENCIA == 1 ? true : false,
                    Centro = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                    Expediente = string.Format("{0}/{1}", imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO, imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO),
                    IdIngreso = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_INGRESO,
                    IdImputado = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                    Id_Grupo = (short)imputado.GRUPO_PARTICIPANTE.ID_GRUPO,
                    IdConsec = (short)imputado.ID_CONSEC,
                    Materno = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO,
                    Paterno = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO,
                    Nombre = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE,
                    NIP = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NIP,
                    Ubicacion = imputado.GRUPO_HORARIO.AREA.DESCR,
                    Seleccionar = false,
                    Responsable = string.Format("{1} {2} {0}", imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.NOMBRE.TrimEnd(), imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.PATERNO.TrimEnd(), imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.MATERNO.TrimEnd())
                });

            }
            ListExpediente = ListaAux;
            ListaSeleccionados = new List<InternosActividad>();
            if (ListExpediente.Count > 0)
            {
                EmptyBusquedaVisible = false;
            }
            else
            {
                EmptyBusquedaVisible = true;
            }

            this.Areas = Areas;
        }

        #endregion


        #region Metodos Eventos
        //ON LOAD
        public void OnLoad(BuscarInternosProgramas Window)
        {


        }


        //CLICK ENTER
        public async void EnterKeyPressed(object obj)
        {
            VentanaBusqueda.Hide();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                var AnioBusqueda = 0;
                var FolioBusqueda = 0;
                Int32.TryParse(AnioBuscar, out AnioBusqueda);
                Int32.TryParse(FolioBuscar, out FolioBusqueda);
                if (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar) || AnioBusqueda != 0 || FolioBusqueda != 0)
                {

                    try
                    {

                        //SE LIMPIA BÚSQUEDA ANTERIOR
                        ListExpediente.Clear();

                        //SE INICIA NUEVA BÚSQUEDA
                        var imputados_busqueda = new List<InternosActividad>();
                        List<GRUPO_ASISTENCIA> imputados = new InternosActividad().ObtenerInternos(Areas, NombreBuscar, ApellidoPaternoBuscar, ApellidoMaternoBuscar, (short)AnioBusqueda, FolioBusqueda).Where(w =>

                            //FILTRA CON LA FECHA DEL SERVER
                            w.GRUPO_HORARIO.HORA_INICIO.Value.Day == Fechas.GetFechaDateServer.Day &&
                         w.GRUPO_HORARIO.HORA_INICIO.Value.Month == Fechas.GetFechaDateServer.Month &&
                         w.GRUPO_HORARIO.HORA_INICIO.Value.Year == Fechas.GetFechaDateServer.Year &&
                         w.GRUPO_HORARIO.HORA_INICIO.Value.Hour == Fechas.GetFechaDateServer.Hour &&

                            //FILTRA A LOS INTERNOS CON TRASLADOS EN PROCESO Ó ACTIVOS
                         (w.GRUPO_PARTICIPANTE.INGRESO.TRASLADO_DETALLE.Where(
                         wTD =>
                                   wTD.TRASLADO.TRASLADO_FEC.Year == Fechas.GetFechaDateServer.Year &&
                                   wTD.TRASLADO.TRASLADO_FEC.Month == Fechas.GetFechaDateServer.Month &&
                                   wTD.TRASLADO.TRASLADO_FEC.Day == Fechas.GetFechaDateServer.Day &&
                                   (wTD.TRASLADO.ID_ESTATUS == "EP" || wTD.TRASLADO.ID_ESTATUS == "AC")).Count() == 0) &&

                            //FILTRA A LOS INTERNOS CON EXCARCELACIONES EN PROCESO Ó ACTIVAS
                         (w.GRUPO_PARTICIPANTE.INGRESO.EXCARCELACION.Where(
                         wEXC =>
                                   wEXC.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                                   wEXC.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                                   wEXC.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day &&
                                   wEXC.ID_ESTATUS == "EP" || wEXC.ID_ESTATUS == "AC").Count() == 0)
                            ).ToList();


                        foreach (var imputado in imputados)
                        {

                            if (!ListaSeleccionados.Where(w =>
                                        w.Anio == imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                        w.Centro == imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                                        w.IdImputado == imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).Any())
                            {
                                imputados_busqueda.Add(new InternosActividad()
                                {
                                    Actividad = imputado.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR,
                                    Anio = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                                    Area = (short)imputado.GRUPO_HORARIO.ID_AREA,
                                    Asistencia = imputado.ASISTENCIA == 1 ? true : false,
                                    Centro = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                                    Expediente = string.Format("{0}/{1}", imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO, imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO),
                                    IdIngreso = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_INGRESO,
                                    IdImputado = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                                    Id_Grupo = (short)imputado.GRUPO_PARTICIPANTE.ID_GRUPO,
                                    IdConsec = (short)imputado.ID_CONSEC,
                                    Materno = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO,
                                    Paterno = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO,
                                    Nombre = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE,
                                    NIP = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NIP,
                                    Ubicacion = imputado.GRUPO_HORARIO.AREA.DESCR,
                                    Seleccionar = false,
                                    Responsable = string.Format("{1} {2} {0}", imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.NOMBRE.TrimEnd(), imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.PATERNO.TrimEnd(), imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.MATERNO.TrimEnd())
                                });
                            }
                        }
                        ListExpediente = imputados_busqueda;
                        if (ListExpediente.Count > 0)
                            EmptyBusquedaVisible = false;
                        else
                            EmptyBusquedaVisible = true;
                    }

                    catch (Exception ex)
                    {

                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda de participantes.", ex);
                    }

                }
                else
                {
                    ListExpediente = new List<InternosActividad>();
                }
            });
            VentanaBusqueda.ShowDialog();
        }

        //CLICK SWITCH - BOTONES
        public async void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "BuscarClick":
                    VentanaBusqueda.Hide();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        var AnioBusqueda = 0;
                        var FolioBusqueda = 0;
                        Int32.TryParse(AnioBuscar, out AnioBusqueda);
                        Int32.TryParse(FolioBuscar, out FolioBusqueda);
                        if (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar) || AnioBusqueda != 0 || FolioBusqueda != 0)
                        {

                            try
                            {
                                ListExpediente.Clear();
                                var imputados_busqueda = new List<InternosActividad>();
                                List<GRUPO_ASISTENCIA> imputados = new InternosActividad().ObtenerInternos(Areas, NombreBuscar, ApellidoPaternoBuscar, ApellidoMaternoBuscar, (short)AnioBusqueda, FolioBusqueda).Where(w =>

                                    //FILTRA CON LA FECHA DEL SERVER
                                    w.GRUPO_HORARIO.HORA_INICIO.Value.Day == Fechas.GetFechaDateServer.Day &&
                                 w.GRUPO_HORARIO.HORA_INICIO.Value.Month == Fechas.GetFechaDateServer.Month &&
                                 w.GRUPO_HORARIO.HORA_INICIO.Value.Year == Fechas.GetFechaDateServer.Year &&
                                 w.GRUPO_HORARIO.HORA_INICIO.Value.Hour == Fechas.GetFechaDateServer.Hour &&

                                    //FILTRA A LOS INTERNOS CON TRASLADOS EN PROCESO Ó ACTIVOS
                                 (w.GRUPO_PARTICIPANTE.INGRESO.TRASLADO_DETALLE.Where(
                                 wTD =>
                                           wTD.TRASLADO.TRASLADO_FEC.Year == Fechas.GetFechaDateServer.Year &&
                                           wTD.TRASLADO.TRASLADO_FEC.Month == Fechas.GetFechaDateServer.Month &&
                                           wTD.TRASLADO.TRASLADO_FEC.Day == Fechas.GetFechaDateServer.Day &&
                                           (wTD.TRASLADO.ID_ESTATUS == "EP" || wTD.TRASLADO.ID_ESTATUS == "AC")).Count() == 0) &&

                                    //FILTRA A LOS INTERNOS CON EXCARCELACIONES EN PROCESO Ó ACTIVAS
                                 (w.GRUPO_PARTICIPANTE.INGRESO.EXCARCELACION.Where(
                                 wEXC =>
                                           wEXC.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                                           wEXC.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                                           wEXC.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day &&
                                           wEXC.ID_ESTATUS == "EP" || wEXC.ID_ESTATUS == "AC").Count() == 0)
                                    ).ToList();


                                foreach (var imputado in imputados)
                                {
                                    if (!ListaSeleccionados.Where(w =>
                                        w.Anio == imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                        w.Centro == imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                                        w.IdImputado == imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).Any())
                                    {


                                        imputados_busqueda.Add(new InternosActividad()
                                        {
                                            Actividad = imputado.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR,
                                            Anio = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                                            Area = (short)imputado.GRUPO_HORARIO.ID_AREA,
                                            Asistencia = imputado.ASISTENCIA == 1 ? true : false,
                                            Centro = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                                            Expediente = string.Format("{0}/{1}", imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO, imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO),
                                            IdIngreso = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_INGRESO,
                                            IdImputado = (short)imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                                            Id_Grupo = (short)imputado.GRUPO_PARTICIPANTE.ID_GRUPO,
                                            IdConsec = (short)imputado.ID_CONSEC,
                                            Materno = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO,
                                            Paterno = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO,
                                            Nombre = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE,
                                            NIP = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NIP,
                                            Ubicacion = imputado.GRUPO_HORARIO.AREA.DESCR,
                                            Seleccionar = false,
                                            Responsable = string.Format("{1} {2} {0}", imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.NOMBRE.TrimEnd(), imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.PATERNO.TrimEnd(), imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.MATERNO.TrimEnd())
                                        });
                                    }
                                }
                                ListExpediente = imputados_busqueda;
                                if (ListExpediente.Count > 0)
                                    EmptyBusquedaVisible = false;
                                else
                                    EmptyBusquedaVisible = true;
                            }


                            catch (Exception ex)
                            {

                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda de participantes.", ex);
                            }

                        }
                        else
                        {
                            ListExpediente = new List<InternosActividad>();
                        }
                    });
                    VentanaBusqueda.ShowDialog();

                    break;
                case "SeleccionarImputado":
                    if (SelectedImputado != null)
                    {
                        if (SelectedImputado.FotoInterno == null)
                        {
                            try
                            {
                                var ingreso_biometrico = new cIngresoBiometrico();
                                var placeholder = new Imagenes().getImagenPerson();
                                var foto_ingreso = ingreso_biometrico.Obtener(SelectedImputado.Anio, SelectedImputado.Centro, SelectedImputado.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                var foto_seguimiento = ingreso_biometrico.Obtener(SelectedImputado.Anio, SelectedImputado.Centro, SelectedImputado.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                SelectedImputadoFotoIngreso = foto_ingreso != null ? foto_ingreso.BIOMETRICO : placeholder;
                                SelectedImputadoFotoSeguimiento = foto_seguimiento != null ? foto_seguimiento.BIOMETRICO : placeholder;
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener fotos del imputado seleccionado.", ex);

                            }

                        }
                    }
                    break;
                case "0":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "0";
                    }
                    break;
                case "1":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "1";
                    }
                    break;
                case "2":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "2";
                    }
                    break;
                case "3":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "3";
                    }
                    break;
                case "4":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "4";
                    }
                    break;
                case "5":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "5";
                    }
                    break;
                case "6":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "6";
                    }
                    break;
                case "7":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "7";
                    }
                    break;
                case "8":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "8";
                    }
                    break;
                case "9":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "9";
                    }
                    break;
                case "backspace":
                    if (NIPBuscar.Length > 0)
                    {
                        NIPBuscar = NIPBuscar.Substring(0, NIPBuscar.Length - 1);
                    }
                    break;
                case "limpiarNIP":
                    NIPBuscar = "";
                    break;
                case "onBuscarPorNIP":
                    var NIP = 0;
                    try
                    {
                        if (Int32.TryParse(NIPBuscar, out NIP))
                        {
                            SelectedCustodio = new cEmpleado().Obtener(NIP);

                            if (SelectedCustodio != null)
                            {
                                NombreCustodio = SelectedCustodio.PERSONA.NOMBRE.TrimEnd();
                                PaternoCustodio = SelectedCustodio.PERSONA.PATERNO.TrimEnd();
                                MaternoCustodio = SelectedCustodio.PERSONA.MATERNO.TrimEnd();
                                AnioCustodio = SelectedCustodio.REGISTRO_FEC.Value.Year.ToString();
                                IDCustodio = SelectedCustodio.ID_EMPLEADO.ToString();
                                var consulta_foto_custodio = SelectedCustodio.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                FotoCustodio = consulta_foto_custodio != null ? consulta_foto_custodio.BIOMETRICO : new Imagenes().getImagenPerson();
                                CambiarMensajeNIP(enumMensajeNIP.ENCONTRADO);

                            }
                            else
                            {
                                CambiarMensajeNIP(enumMensajeNIP.NO_ENCONTRADO);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        VentanaBusqueda.Hide();
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener informacion de un custodio (Error al buscar por ID).", ex);
                    }

                    break;
                case "Autorizar":
                    try
                    {
                        var ingreso_ubicacion = new cIngresoUbicacion();
                        using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                        {


                            foreach (var imputado in ListExpediente)
                            {

                            }

                            transaccion.Complete();

                        }
                    }
                    catch (Exception ex)
                    {

                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al asignar el custodio seleccionado.", ex);
                    }
                    break;
                case "SeleccionarInternos":
                    if (ListExpediente.Count > 0)
                    {
                        var seleccionados = ListExpediente.Where(w => w.Seleccionar).ToList();
                        var lista_seleccionados = seleccionados != null ? seleccionados : new List<InternosActividad>();
                        if (lista_seleccionados.Count > 0)
                        {
                            EmptySeleccionadosVisible = false;
                            ListaSeleccionados.AddRange(lista_seleccionados);
                            RaisePropertyChanged("ListaSeleccionados");
                        }
                            
                        else
                        {
                            EmptySeleccionadosVisible = true;
                            await VentanaBusqueda.ShowMessageAsync("Validación", "Debe seleccionar al menos un interno.");
                        }
                    }
                    else
                    {
                        await VentanaBusqueda.ShowMessageAsync("Validación", "No hay internos resultantes de la búsqueda. Por favor, intente más tarde ó ejecute una nueva búsqueda.");
                    }
                    break;
                case "RemoverInternos":
                    if (ListaSeleccionados.Count > 0)
                    {
                        var seleccionados = ListaSeleccionados.Where(w => w.Seleccionar).ToList();
                        var lista_seleccionados = seleccionados != null ? seleccionados : new List<InternosActividad>();
                        if (lista_seleccionados.Count > 0)
                        {
                            
                        }
                        else
                        {
                            await VentanaBusqueda.ShowMessageAsync("Validación", "Debe seleccionar al menos un interno.");
                        }
                    }
                    else
                    {
                        await VentanaBusqueda.ShowMessageAsync("Validación", "No hay internos seleccionados.");
                    }
                    break;
                case "Limpiar":
                    SelectedJustificacion = enumTipoSalida.TRASLADO;
                    NombreCustodio = "";
                    PaternoCustodio = "";
                    MaternoCustodio = "";
                    AnioCustodio = "";
                    IDCustodio = "";
                    FotoCustodio = new Imagenes().getImagenPerson();
                    break;
                case "LimpiarClick":
                    var placeholder_limpiar = new Imagenes().getImagenPerson();
                    SelectedImputadoFotoIngreso = placeholder_limpiar;
                    SelectedImputadoFotoSeguimiento = placeholder_limpiar;
                    NombreBuscar = "";
                    ApellidoPaternoBuscar = "";
                    ApellidoMaternoBuscar = "";
                    AnioBuscar = "";
                    FolioBuscar = "";
                    ListExpediente = new List<InternosActividad>();
                    EmptyBusquedaVisible = true;
                    break;
            }

        }




        private void MouseEnterSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    break;
            }
        }


        private void MouseLeaveSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
                    break;
            }
        }
        #endregion


        #region Metodos Mensajes Interfaz
        public async void CambiarMensajeNIP(enumMensajeNIP TipoMensaje)
        {
            switch (TipoMensaje)
            {
                case enumMensajeNIP.ENCONTRADO:
                    CheckMark = "\u2713 \u2713";
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Green);
                    break;
                case enumMensajeNIP.NO_ENCONTRADO:
                    CheckMark = "X";
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Red);
                    break;
            }
            await TaskEx.Delay(1500);
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);

        }


        public async void CambiarMensajeHuella(enumMensajeHuella TipoMensaje)
        {
            switch (TipoMensaje)
            {
                case enumMensajeHuella.ENCONTRADO:
                    break;
                case enumMensajeHuella.NO_ENCONTRADO:
                    break;
                case enumMensajeHuella.FALSO_POSITIVO:
                    break;
            }
        }
        #endregion

    }
}

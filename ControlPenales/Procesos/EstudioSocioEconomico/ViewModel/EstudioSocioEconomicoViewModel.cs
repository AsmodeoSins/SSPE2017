using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class EstudioSocioEconomicoViewModel : ValidationViewModelBase
    {
        public EstudioSocioEconomicoViewModel() { }
        private void LoadEstudioSocioEconomico(EstudioSocioEconomicoView Window = null)
        {
            ConfiguraPermisos();
        }

        #region Metodos

        private async void ModelEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    if (!obj.GetType().Name.Equals("String"))
                    {
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    NombreBuscar = textbox.Text;
                                    NombreD = NombreBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    ApellidoPaternoBuscar = textbox.Text;
                                    PaternoD = ApellidoPaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    ApellidoMaternoBuscar = textbox.Text;
                                    MaternoD = ApellidoMaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "FolioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        FolioBuscar = int.Parse(textbox.Text);
                                    else
                                        FolioBuscar = null;
                                    AnioBuscar = AnioD;
                                    break;
                                case "AnioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        AnioBuscar = int.Parse(textbox.Text);
                                    else
                                        AnioBuscar = null;
                                    FolioBuscar = FolioD;
                                    break;
                            }
                        }

                    }
                }
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

                if (string.IsNullOrEmpty(NombreD))
                    NombreBuscar = string.Empty;
                else
                    NombreBuscar = NombreD;

                if (string.IsNullOrEmpty(PaternoD))
                    ApellidoPaternoBuscar = string.Empty;
                else
                    ApellidoPaternoBuscar = PaternoD;

                if (string.IsNullOrEmpty(MaternoD))
                    ApellidoMaternoBuscar = string.Empty;
                else
                    ApellidoMaternoBuscar = MaternoD;

                if (AnioBuscar != null && FolioBuscar != null)
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                SelectExpediente = null;
                                SelectIngreso = null;
                                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                                return;
                            }
                        }
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                                ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        }

                        SelectExpediente = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        SelectedInterno = SelectExpediente;
                        SeleccionaIngreso();
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        return;
                    }
                    else
                    {
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                }
                else
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)//Empty row
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;

                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }


        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar_menu":
                    //ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    LimpiarBusqueda();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudioSocioEconomicoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.EstudioSocioEconomicoViewModel();
                    ConfiguraPermisos();
                    break;

                case "nueva_busqueda":
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    break;

                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;

                case "ficha_menu":
                    break;

                case "buscar_salir":
                    if (AuxIngreso != null)
                    {
                        SelectIngreso = AuxIngreso;
                        AuxIngreso = null;
                    }
                    StaticSourcesViewModel.SourceChanged = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "reporte_menu":
                    if (SelectIngreso != null)
                        ImprimirEstudio();
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }

                    break;

                case "buscar_seleccionar":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }

                    foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                    {
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                            return;
                        }
                    }

                    if (SelectIngreso.ID_UB_CENTRO.HasValue && SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        return;
                    }
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "]tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                        return;
                    }
                    SelectedInterno = SelectIngreso.IMPUTADO;
                    if (SelectIngreso != null)
                        SelectIngreso = SelectIngreso;

                    this.SeleccionaIngreso();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    StaticSourcesViewModel.SourceChanged = false;
                    break;

                case "guardar_menu":
                    if (SelectIngreso != null)
                    {
                        if (!base.HasErrors)
                        {
                            Guardar();
                            (new Dialogos()).ConfirmacionDialogo("Exito!", "Se ha guardado el estudio socioeconómico correctamente");
                            StaticSourcesViewModel.SourceChanged = false;
                        }

                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Es necesario capturar todos los campos necesarios para continuar");
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }

                    break;

                default:
                    //no action
                    break;
            }
        }

        private void ProcesaMultiSelect()
        {
            try
            {
                var SeleccionadosPrimario = new List<string>();
                var SeleccionadosSecundario = new List<string>();
                var CaracteristicasGuardadas = new cEstudioSocioEconomicoCat().GetData();

                #region Validacion Datos Primario (version preliminar de funcionalidad)
                if (IsCartonPrimarioChecked) SeleccionadosPrimario.Add("M1");
                if (IsAdobePrimarioChecked) SeleccionadosPrimario.Add("M2");
                if (IsLadrilloPrimarioChecked) SeleccionadosPrimario.Add("M3");
                if (IsBlockPrimarioChecked) SeleccionadosPrimario.Add("M4");
                if (IsMaderaPrimarioChecked) SeleccionadosPrimario.Add("M5");
                if (IsMaterialesOtrosPrimarioChecked) SeleccionadosPrimario.Add("M6");

                if (IsSalaPrimarioChecked) SeleccionadosPrimario.Add("D1");
                if (IsCocinaPrimarioChecked) SeleccionadosPrimario.Add("D2");
                if (IsComedorPrimarioChecked) SeleccionadosPrimario.Add("D3");
                if (IsRecamaraChecked) SeleccionadosPrimario.Add("D4");
                if (IsBanioChecked) SeleccionadosPrimario.Add("D5");
                if (IsDistribucionPrimariaOtrosChecked) SeleccionadosPrimario.Add("D6");

                if (IsEnergiaElectricaPrimariaChecked) SeleccionadosPrimario.Add("S1");
                if (IsAguaPrimariaChecked) SeleccionadosPrimario.Add("S2");
                if (IsDrenajePrimariaChecked) SeleccionadosPrimario.Add("S3");
                if (IsPavimentoPrimarioChecked) SeleccionadosPrimario.Add("S4");
                if (IsTelefonoPrimarioChecked) SeleccionadosPrimario.Add("S5");
                if (IsTVCableChecked) SeleccionadosPrimario.Add("S6");

                if (IsEstufaPrimarioChecked) SeleccionadosPrimario.Add("E1");
                if (IsRefrigeradorPrimarioChecked) SeleccionadosPrimario.Add("E2");
                if (IsMicroOndasPrimarioChecked) SeleccionadosPrimario.Add("E3");
                if (IsTVPrimarioChecked) SeleccionadosPrimario.Add("E4");
                if (IsLavadoraPrimarioChecked) SeleccionadosPrimario.Add("E5");
                if (IsSecadoraPrimarioChecked) SeleccionadosPrimario.Add("E6");
                if (IsComputadoraPrimarioChecked) SeleccionadosPrimario.Add("E7");
                if (IsOtrosElectrodomesticosPrimarioChecked) SeleccionadosPrimario.Add("E8");


                if (IsAutomovilPrimarioChecked) SeleccionadosPrimario.Add("T1");
                if (IsAutobusPrimarioChecked) SeleccionadosPrimario.Add("T2");
                if (IsOtrosMediosTransportePrimarioChecked) SeleccionadosPrimario.Add("T3");


                #endregion

                #region Validacion Datos Secundarios (version premilinar de funcionalidad)

                if (IsCartonSecundarioChecked) SeleccionadosSecundario.Add("M1");
                if (IsAdobeSecundarioChecked) SeleccionadosSecundario.Add("M2");
                if (IsLadrilloSecundarioChecked) SeleccionadosSecundario.Add("M3");
                if (IsBlockSecundarioChecked) SeleccionadosSecundario.Add("M4");
                if (IsMaderaSecundarioChecked) SeleccionadosSecundario.Add("M5");
                if (IsOtrosMaterialesSecundarioChecked) SeleccionadosSecundario.Add("M6");
                if (IsSalaSecundarioChecked) SeleccionadosSecundario.Add("D1");
                if (IsCocinaSecundarioChecked) SeleccionadosSecundario.Add("D2");
                if (IsComedorSecundarioChecked) SeleccionadosSecundario.Add("D3");
                if (IsRecamaraSecundarioChecked) SeleccionadosSecundario.Add("D4");
                if (IsBanioSecundarioChecked) SeleccionadosSecundario.Add("D5");
                if (IsOtrosDistribucionSecundarioChecked) SeleccionadosSecundario.Add("D6");
                if (IsEnergiaElectricaSecundariaChecked) SeleccionadosSecundario.Add("S1");
                if (IsAguaSecundarioChecked) SeleccionadosSecundario.Add("S2");
                if (IsDrenajeSecundarioChecked) SeleccionadosSecundario.Add("S3");
                if (IsPavimentoSecundarioChecked) SeleccionadosSecundario.Add("S4");
                if (IsTelefonoSecundarioChecked) SeleccionadosSecundario.Add("S5");
                if (IsTVCableSecundarioChecked) SeleccionadosSecundario.Add("S6");
                if (IsEstufaSecundarioChecked) SeleccionadosSecundario.Add("E1");
                if (IsRefrigeradorSecundarioChecked) SeleccionadosSecundario.Add("E2");
                if (IsMicroOndasSecundarioChecked) SeleccionadosSecundario.Add("E3");
                if (IsTVSecundarioChecked) SeleccionadosSecundario.Add("E4");
                if (IsLavadoraSecundarioChecked) SeleccionadosSecundario.Add("E5");
                if (IsSecadoraSecundarioChecked) SeleccionadosSecundario.Add("E6");
                if (IsComputadoraSecundariaChecked) SeleccionadosSecundario.Add("E7");
                if (IsOtrosElectrodomesticosChecked) SeleccionadosSecundario.Add("E8");
                if (IsAutomovilSecundarioChecked) SeleccionadosSecundario.Add("T1");
                if (IsAutobusSecundarioChecked) SeleccionadosSecundario.Add("T2");
                if (IsOtrosMediosTransporteChecked) SeleccionadosSecundario.Add("T3");

                if (IsGiroChecked) SeleccionadosSecundario.Add("A1");
                if (IsCuentaBChecked) SeleccionadosSecundario.Add("A2");
                if (IsDepositoChecked) SeleccionadosSecundario.Add("A3");

                #endregion

                #region Asignacion y Validacion d claves
                int Clave = new int();
                string Tipo = string.Empty;

                ListCaractPrimario = new List<SOCIOECONOMICO_CAT>();
                if (SeleccionadosPrimario.Any())
                {
                    foreach (var item in SeleccionadosPrimario)
                    {
                        int.TryParse(item.Substring(1), out Clave);
                        Tipo = item.Substring(0, 1);

                        foreach (var item2 in CaracteristicasGuardadas)
                        {
                            if (item2.ID_TIPO == Tipo && item2.ID_CLAVE == Clave)
                            {
                                ListCaractPrimario.Add(item2);
                                break;
                            }
                            else
                                continue;
                        }
                    }
                }

                else
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No ha marcado al menos una de las características del grupo primario.");
                    return;
                }

                ListCaractSecundario = new List<SOCIOECONOMICO_CAT>();
                if (SeleccionadosSecundario.Any())
                {
                    foreach (var item in SeleccionadosSecundario)
                    {
                        int.TryParse(item.Substring(1), out Clave);
                        Tipo = item.Substring(0, 1);

                        foreach (var item2 in CaracteristicasGuardadas)
                        {
                            if (item2.ID_TIPO == Tipo && item2.ID_CLAVE == Clave)
                            {
                                ListCaractSecundario.Add(item2);
                                break;
                            }
                            else
                                continue;
                        }
                    }
                }

                else
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No ha marcado al menos una de las características del grupo secundario.");
                    return;
                }


                #endregion

            }

            catch (Exception exc)
            {
                throw;
            }
        }


        private void ImprimirEstudio()
        {
            try
            {
                Estudio = new cEstudioSocioEconomico().Obtener(SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                #region Validacion y Obtencion de Datos
                if (SelectIngreso != null)
                {
                    if (Estudio != null)
                    {
                        var DatosReporte = new cCuerpoReporte();

                        var View = new ReportesView();
                        #region Iniciliza el entorno para mostrar el reporte al usuario
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        View.Owner = PopUpsViewModels.MainWindow;
                        View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };


                        View.Show();
                        #endregion

                        #region Se forma el cuerpo del reporte

                        string _NombreUsuario = string.Empty;
                        var NombreUsuario = new cUsuario().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr).FirstOrDefault();
                        if (NombreUsuario != null)
                            _NombreUsuario = NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty;

                        string _NombreCoord = string.Empty;
                        var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_TRABAJO_SOCAL).FirstOrDefault();
                        if (_UsuarioCoordinador != null)
                            _NombreCoord = _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty;



                        if (SelectIngreso.IMPUTADO != null)
                        {
                            //DatosReporte.Telefono = SelectIngreso.IMPUTADO.TELEFONO.HasValue ? SelectIngreso.IMPUTADO.TELEFONO.Value.ToString() : string.Empty;
                            DatosReporte.Telefono = SelectIngreso.TELEFONO.HasValue ? SelectIngreso.TELEFONO.Value.ToString() : string.Empty;
                            //DatosReporte.Escolaridad = SelectIngreso.IMPUTADO.ESCOLARIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.ESCOLARIDAD.DESCR) ? SelectIngreso.IMPUTADO.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                            DatosReporte.Escolaridad = SelectIngreso.ESCOLARIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? SelectIngreso.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                            //DatosReporte.Ocupacion = SelectIngreso.IMPUTADO.OCUPACION != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.OCUPACION.DESCR) ? SelectIngreso.IMPUTADO.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                            DatosReporte.Ocupacion = SelectIngreso.OCUPACION != null ? !string.IsNullOrEmpty(SelectIngreso.OCUPACION.DESCR) ? SelectIngreso.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                            DatosReporte.EdadInterno = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString();
                            DatosReporte.FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty;
                            DatosReporte.NombreInternoInicial = string.Concat("NOMBRE DEL INTERNO: ", string.Format("{0}/{1} ", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO), string.Format("{0} {1} {2}",
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty), ", UBICACIÓN ACTUAL: ", string.Format("{0}-{1}{2}-{3}",
                                                    SelectIngreso != null ? SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectIngreso != null ? SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectIngreso != null ? SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectIngreso != null ? SelectIngreso.ID_UB_CAMA.HasValue ? SelectIngreso.ID_UB_CAMA.Value.ToString() : string.Empty : string.Empty));

                            DatosReporte.NombreInterno = string.Format("{0} {1} {2}",
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);

                            //DatosReporte.Domicilio = string.Format("{0} {1} {2} {3}", SelectIngreso.IMPUTADO.DOMICILIO_CALLE, SelectIngreso.IMPUTADO.DOMICILIO_NUM_EXT.HasValue ? SelectIngreso.IMPUTADO.DOMICILIO_NUM_EXT != decimal.Zero ? SelectIngreso.IMPUTADO.DOMICILIO_NUM_EXT.ToString() : string.Empty : string.Empty,
                            //    SelectIngreso.IMPUTADO.COLONIA != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.COLONIA.DESCR) ? SelectIngreso.IMPUTADO.COLONIA.DESCR.Trim() : string.Empty : string.Empty, SelectIngreso.IMPUTADO.MUNICIPIO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1) ? SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                            DatosReporte.Domicilio = string.Format("{0} {1} {2} {3}", SelectIngreso.DOMICILIO_CALLE, SelectIngreso.DOMICILIO_NUM_EXT.HasValue ? SelectIngreso.DOMICILIO_NUM_EXT != decimal.Zero ? SelectIngreso.DOMICILIO_NUM_EXT.ToString() : string.Empty : string.Empty,
                                SelectIngreso.COLONIA != null ? !string.IsNullOrEmpty(SelectIngreso.COLONIA.DESCR) ? SelectIngreso.COLONIA.DESCR.Trim() : string.Empty : string.Empty, SelectIngreso.MUNICIPIO != null ? !string.IsNullOrEmpty(SelectIngreso.MUNICIPIO.MUNICIPIO1) ? SelectIngreso.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                        };


                        DatosReporte.NombreCentro = SelectIngreso.CENTRO != null ? !string.IsNullOrEmpty(SelectIngreso.CENTRO.DESCR) ? SelectIngreso.CENTRO.DESCR.Trim() : string.Empty : string.Empty;
                        DatosReporte.SalarioPercibia = Estudio.SALARIO.HasValue ? Estudio.SALARIO.Value.ToString("c") : string.Empty;
                        DatosReporte.Elaboro = _NombreUsuario;
                        DatosReporte.Coordinador = _NombreCoord;

                        #region Datos Grupo Primario
                        if (Estudio.SOCIOE_GPOFAMPRI != null)
                        {
                            DatosReporte.GrupoFamiliarPrimario = string.Concat("Funcional ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR) ? Estudio.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR.Equals("F") ? "X" : string.Empty : string.Empty, " )  Disfuncional ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR) ? Estudio.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR.Equals("D") ? "X" : string.Empty : string.Empty, " )");
                            DatosReporte.RelacionIntroFamiliarPrimariaGrupoPrimario = string.Concat("Adecuada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR) ? Estudio.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR.Equals("A") ? "X" : string.Empty : string.Empty, " )  Inadecuada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR) ? Estudio.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR.Equals("I") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.CuantasPersonasVivenEnHogar = Estudio.SOCIOE_GPOFAMPRI.PERSONAS_VIVEN_HOGAR.HasValue ? Estudio.SOCIOE_GPOFAMPRI.PERSONAS_VIVEN_HOGAR.Value.ToString() : string.Empty;
                            DatosReporte.CuantasPersonasLaboranGrupoPrimario = Estudio.SOCIOE_GPOFAMPRI.PERSONAS_LABORAN.HasValue ? Estudio.SOCIOE_GPOFAMPRI.PERSONAS_LABORAN.Value.ToString() : string.Empty;
                            DatosReporte.IngresoMensualFamiliarGrupoPrimario = Estudio.SOCIOE_GPOFAMPRI.INGRESO_MENSUAL.HasValue ? Estudio.SOCIOE_GPOFAMPRI.INGRESO_MENSUAL.Value.ToString("c") : string.Empty;
                            DatosReporte.EgresoMensualFamiliarGrupoPrimario = Estudio.SOCIOE_GPOFAMPRI.EGRESO_MENSUAL.HasValue ? Estudio.SOCIOE_GPOFAMPRI.EGRESO_MENSUAL.Value.ToString("c") : string.Empty;
                            DatosReporte.IntegranteAntecedentesAdiccionGrupoPrimario = string.Concat("SI ( ", Estudio.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE.HasValue ? Estudio.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE.Value == (decimal)eProcedimiento.SI ? "X" : string.Empty : string.Empty, " )  NO ( ", Estudio.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE.HasValue ? Estudio.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE.Value == (decimal)eProcedimiento.NO ? "X" : string.Empty : string.Empty, " )");
                            DatosReporte.EspecifiqueAntecedentesAdiccionGrupoPrimario = Estudio.SOCIOE_GPOFAMPRI.ANTECEDENTE;
                            DatosReporte.ZonaUbicacionViviendaGrupoPrimario = string.Concat("Urbana ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA.Equals("U") ? "X" : string.Empty : string.Empty, " )  Semi-Urbana ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA.Equals("S") ? "X" : string.Empty : string.Empty, " )  Rural ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA.Equals("R") ? "X" : string.Empty : string.Empty, " )");
                            DatosReporte.CondicionesViviendaGrupoPrimario = string.Concat("Propia ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES.Equals("P") ? "X" : string.Empty : string.Empty, " )  Rentada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES.Equals("R") ? "X" : string.Empty : string.Empty, " )  Otro ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES.Equals("O") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.NivelSocioEconomicoCulturalGrupoPrimario = string.Concat(" Alto ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL.Equals("A") ? "X" : string.Empty : string.Empty, " )  Medio ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL.Equals("M") ? "X" : string.Empty : string.Empty, " )  Bajo ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL.Equals("B") ? "X" : string.Empty : string.Empty, " ) ");

                            if (Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Any())
                            {
                                var MaterialesPrimarios = Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Where(w => w.ID_TIPO == "M" && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);//Lista de los materiales
                                DatosReporte.ViviendaMaterialesPrimario = string.Concat("Cartón ( ", MaterialesPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("CARTON")) ? "X" : string.Empty,
                                    MaterialesPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ADOBE")) ? "X" : string.Empty, " ) Ladrillo ( ", MaterialesPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("LADRILLO")) ? "X" : string.Empty,
                                    " ) Madera ( ", MaterialesPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("MADERA")) ? "X" : string.Empty,
                                    " ) Block ( ", MaterialesPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("BLOCK")) ? "X" : string.Empty, " ) Otro ( ", MaterialesPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("OTRO")) ? "X" : string.Empty, " ) ");

                                var DistribucionPrimaria = Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Where(x => x.ID_TIPO == "D" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string DristriGrupoPrimario = string.Concat("Sala ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("SALA")) ? "X" : string.Empty,
                                    " )  Cocina ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("COCINA")) ? "X" : string.Empty, " ) Comedor ( ",
                                DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("COMEDOR")) ? "X" : string.Empty, " ) Recamara ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("RECAMARA")) ? "X" : string.Empty,
                                " ) Baño ( ", DistribucionPrimaria.Any(X => X.SOCIOECONOMICO_CAT.DESCR.Contains("BAÑO")) ? "X" : string.Empty, " ) Otros ( ",
                                DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("OTROS")) ? "X" : string.Empty, " ) ");
                                DatosReporte.DistVivivendaPrimario = DristriGrupoPrimario;


                                var ServiciosPrimarios = Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Where(x => x.ID_TIPO == "S" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string ServiciosP = string.Concat("Energía  Eléctrica ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ENERGIA")) ? "X" : string.Empty, " ) Agua ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AGUA")) ? "X" : string.Empty,
                                    ")  Drenaje ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("DRENAJE")) ? "X" : string.Empty, " ) Pavimento ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("PAVIMENTO")) ? "X" : string.Empty,
                                    " ) Teléfono ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("TELEFONO")) ? "X" : string.Empty, " ) TV por Cable ( ", ServiciosPrimarios.Any(X => X.SOCIOECONOMICO_CAT.DESCR.Contains("TV POR CABLE")) ? "X" : string.Empty, " )");
                                DatosReporte.ServiciosPrimario = ServiciosP;


                                var ElectrodomesticosPrimarios = Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Where(x => x.ID_TIPO == "E" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string ElectrodomesticosP = string.Concat("Estufa ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ESTUFA")) ? "X" : string.Empty, ") Refrigerador ( ",
                                    ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("REFRIGERADOR")) ? "X" : string.Empty, " ) Horno Microondas ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("HORNO MICROONDAS")) ? "X" : string.Empty,
                                    " ) Televisión ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("TELEVISION")) ? "X" : string.Empty, " ) Lavadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("LAVADORA")) ? "X" : string.Empty,
                                    " )  Secadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("SECADORA")) ? "X" : string.Empty, " ) Computadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("COMPUTADORA")) ? "X" : string.Empty,
                                    " ) Otros ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("OTROS")) ? "X" : string.Empty, " )");
                                DatosReporte.ElectViviendaPrimario = ElectrodomesticosP;


                                var MediosTransporte = Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Where(x => x.ID_TIPO == "T" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string MediosPrimarios = string.Concat(" Automóvil ( ", MediosTransporte.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AUTOMOVIL")) ? "X" : string.Empty, " ) Autobús ( ", MediosTransporte.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AUTOBUS")) ? "X" : string.Empty,
                                    " ) Otro ( ", MediosTransporte.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("OTRO")) ? "X" : string.Empty, " ) ");
                                DatosReporte.MediosTranspPrimario = MediosPrimarios;

                                var ApoyoEconomico = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_TIPO == "A" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                DatosReporte.EspecifiqueApoyoEconomico = string.Concat(" Giro Telegráfico ( ", ApoyoEconomico.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("GIRO TELEGRAFICO")) ? "X" : string.Empty, " ) Cuenta Bancaria ( ", ApoyoEconomico.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("CUENTA BANCARIA")) ? "X" : string.Empty, " ) Deposito en la Institución ( ", ApoyoEconomico.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("DESPOSITO EN LA INSTITUCION")) ? "X" : string.Empty, " )");

                            };
                        };
                        #endregion

                        #region Datos Grupo Secundario
                        if (Estudio.SOCIOE_GPOFAMSEC != null)
                        {
                            DatosReporte.GrupoFamiliarSecundario = string.Concat("Funcional ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR) ? Estudio.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR.Equals("F") ? "X" : string.Empty : string.Empty, " )  Disfuncional ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR) ? Estudio.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR.Equals("D") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.RelacionIntroFamiliarGrupoSecundario = string.Concat("Adecuada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR) ? Estudio.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR.Equals("A") ? "X" : string.Empty : string.Empty, " )  Inadecuada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR) ? Estudio.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR.Equals("I") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.CuantasPersonasLaboranGrupoSecundario = Estudio.SOCIOE_GPOFAMSEC.PERSONAS_LABORAN.HasValue ? Estudio.SOCIOE_GPOFAMSEC.PERSONAS_LABORAN.Value.ToString() : string.Empty;
                            DatosReporte.IngresoMensualGrupoSecundario = Estudio.SOCIOE_GPOFAMSEC.INGRESO_MENSUAL.HasValue ? Estudio.SOCIOE_GPOFAMSEC.INGRESO_MENSUAL.Value.ToString("c") : string.Empty;
                            DatosReporte.EgresoMensualGrupoSecundario = Estudio.SOCIOE_GPOFAMSEC.EGRESO_MENSUAL.HasValue ? Estudio.SOCIOE_GPOFAMSEC.EGRESO_MENSUAL.Value.ToString("c") : string.Empty;
                            DatosReporte.AntecedentesAdiccionGrupoSecundario = string.Concat("SI ( ", Estudio.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE.HasValue ? Estudio.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE.Value == (decimal)eProcedimiento.SI ? "X" : string.Empty : string.Empty, " )  NO ( ", Estudio.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE.HasValue ? Estudio.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE.Value == (decimal)eProcedimiento.NO ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.EspecifiqueAdiccionAntecedentesGrupoSecundario = Estudio.SOCIOE_GPOFAMSEC.ANTECEDENTE;
                            DatosReporte.ZonaUbicacionGrupoSecundario = string.Concat("Urbana ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA.Equals("U") ? "X" : string.Empty : string.Empty, " )  Semi-Urbana ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA.Equals("S") ? "X" : string.Empty : string.Empty, " )  Rural ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA.Equals("R") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.CondicionesViviendaGrupoSecundario = string.Concat("Propia ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES.Equals("P") ? "X" : string.Empty : string.Empty, " )  Rentada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES.Equals("R") ? "X" : string.Empty : string.Empty, " )  Otro ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES.Equals("O") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.NivelSocioEconomicoCulturalGrupoSecundario = string.Concat(" Alto ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL.Equals("A") ? "X" : string.Empty : string.Empty, " )  Medio ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL.Equals("M") ? "X" : string.Empty : string.Empty, " )  Bajo ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL.Equals("B") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.RecibeVisitaFamiliar = string.Concat("SI ( ", Estudio.SOCIOE_GPOFAMSEC.RECIBE_VISITA.HasValue ? Estudio.SOCIOE_GPOFAMSEC.RECIBE_VISITA.Value == (decimal)eProcedimiento.SI ? "X" : string.Empty : string.Empty, " )  NO ( ", Estudio.SOCIOE_GPOFAMSEC.RECIBE_VISITA.HasValue ? Estudio.SOCIOE_GPOFAMSEC.RECIBE_VISITA.Value == (decimal)eProcedimiento.NO ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.DeQuien = Estudio.SOCIOE_GPOFAMSEC.VISITA;
                            DatosReporte.Frecuencia = Estudio.SOCIOE_GPOFAMSEC.FRECUENCIA;
                            DatosReporte.EnCasoDeNoRecibirVisitaEspecifique = Estudio.SOCIOE_GPOFAMSEC.MOTIVO_NO_VISITA;
                            DatosReporte.CuentaConApoyoEconomico = string.Concat("SI ( ", Estudio.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO.HasValue ? Estudio.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO.Value == (decimal)eProcedimiento.SI ? "X" : string.Empty : string.Empty, " )  NO ( ", Estudio.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO.HasValue ? Estudio.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO.Value == (decimal)eProcedimiento.NO ? "X" : string.Empty : string.Empty, " )");
                            DatosReporte.Dictamen = Estudio.DICTAMEN;
                            DatosReporte.FechaEstudio = Estudio.DICTAMEN_FEC.HasValue ? Estudio.DICTAMEN_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;

                            if (Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Any())
                            {
                                var MaterialesSecundarios = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(w => w.ID_TIPO == "M" && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);//Lista de los materiales
                                DatosReporte.VivivendamaterialesSecundario = string.Concat("Cartón ( ", MaterialesSecundarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("CARTON")) ? "X" : string.Empty, " ) ",
                                    " Adobe ( ", MaterialesSecundarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ADOBE")) ? "X" : string.Empty, " ) Ladrillo ( ", MaterialesSecundarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("LADRILLO")) ? "X" : string.Empty,
                                    " ) Madera ( ", MaterialesSecundarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("MADERA")) ? "X" : string.Empty,
                                    ") Block ( ", MaterialesSecundarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("BLOCK")) ? "X" : string.Empty, " ) Otro ( ", MaterialesSecundarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("OTRO")) ? "X" : string.Empty, " )");

                                var DistribucionPrimaria = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_TIPO == "D" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string DristriGrupoPrimario = string.Concat("Sala ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("SALA")) ? "X" : string.Empty,
                                    " )  Cocina ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("COCINA")) ? "X" : string.Empty, " ) Comedor ( ",
                                DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("COMEDOR")) ? "X" : string.Empty, " ) Recamara ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("RECAMARA")) ? "X" : string.Empty,
                                " ) Baño ( ", DistribucionPrimaria.Any(X => X.SOCIOECONOMICO_CAT.DESCR.Contains("BAÑO")) ? "X" : string.Empty, " ) Otros ( ",
                                DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("OTROS")) ? "X" : string.Empty, " ) ");
                                DatosReporte.DistViviendaSecundario = DristriGrupoPrimario;


                                var ServiciosPrimarios = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_TIPO == "S" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string ServiciosP = string.Concat("Energía  Eléctrica ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ENERGIA")) ? "X" : string.Empty, " ) Agua ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AGUA")) ? "X" : string.Empty,
                                    ")  Drenaje ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("DRENAJE")) ? "X" : string.Empty, " ) Pavimento ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("PAVIMENTO")) ? "X" : string.Empty,
                                    "Teléfono ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("TELEFONO")) ? "X" : string.Empty, " ) TV por Cable ( ", ServiciosPrimarios.Any(X => X.SOCIOECONOMICO_CAT.DESCR.Contains("TV POR CABLE")) ? "X" : string.Empty, " )");
                                DatosReporte.ServicioSecundario = ServiciosP;



                                var ElectrodomesticosPrimarios = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_TIPO == "E" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string ElectrodomesticosP = string.Concat("Estufa ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ESTUFA")) ? "X" : string.Empty, " ) Refrigerador ( ",
                                    ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("REFRIGERADOR")) ? "X" : string.Empty, " ) Horno Microondas ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("HORNO MICROONDAS")) ? "X" : string.Empty,
                                    " ) Televisión ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("TELEVISION")) ? "X" : string.Empty, " ) Lavadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("LAVADORA")) ? "X" : string.Empty,
                                    " )  Secadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("SECADORA")) ? "X" : string.Empty, " ) Computadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("COMPUTADORA")) ? "X" : string.Empty,
                                    " ) Otros ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("OTROS")) ? "X" : string.Empty, " ) ");
                                DatosReporte.ElectViviendaSecundario = ElectrodomesticosP;

                                var MediosTransporte = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_TIPO == "T" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string MediosPrimarios = string.Concat(" Automóvil ( ", MediosTransporte.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AUTOMOVIL")) ? "X" : string.Empty, " ) Autobús ( ", MediosTransporte.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AUTOBUS")) ? "X" : string.Empty,
                                    " ) Otro ( ", MediosTransporte.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("OTRO")) ? "X" : string.Empty, " ) ");
                                DatosReporte.MediosTransporteSec = MediosPrimarios;
                            };
                        };

                        #endregion

                        #endregion

                        #region Se forma el encabezado del reporte
                        var Encabezado = new cEncabezado();
                        Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                        Encabezado.TituloDos = Parametro.ENCABEZADO2;
                        Encabezado.NombreReporte = "ESTUDIO SOCIOECONÓMICO";
                        Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                        Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;

                        #endregion

                        #region Inicializacion de reporte
                        View.Report.LocalReport.ReportPath = "Reportes/rEstudioSocioEconomico.rdlc";
                        View.Report.LocalReport.DataSources.Clear();
                        #endregion

                        #region Definicion de origenes de datos

                        //datasource Uno
                        var ds1 = new List<cEncabezado>();
                        ds1.Add(Encabezado);
                        Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds1.Name = "DataSet2";
                        rds1.Value = ds1;
                        View.Report.LocalReport.DataSources.Add(rds1);


                        //datasource dos
                        var ds2 = new List<cCuerpoReporte>();
                        ds2.Add(DatosReporte);
                        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds2.Name = "DataSet1";
                        rds2.Value = ds2;
                        View.Report.LocalReport.DataSources.Add(rds2);

                        View.Report.LocalReport.DisplayName = string.Format("Estudio Socioeconómico del imputado {0} {1} {2} ",//Nombre del archivo que se va a generar con el reporte independientemente del formato que se elija
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty ,
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);

                        View.Report.ReportExport += (s, e) =>
                            {
                                try
                                {
                                    if (e != null && !e.Extension.LocalizedName.Equals("PDF"))//Solo permite pdf
                                    {
                                        e.Cancel = true;//Detiene el evento
                                        s = e = null;//Detiene el mapeo de los parametros para que no continuen en el arbol
                                        (new Dialogos()).ConfirmacionDialogo("Validación", "No tiene permitido exportar la información en este formato.");
                                    }
                                }

                                catch (Exception exc)
                                {
                                    throw exc;
                                }
                            };

                        View.Report.RefreshReport();
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Es necesario capturar todos los campos para poder imprimir el formato.");

                        #endregion
                }
                #endregion
            }

            catch (Exception exc)
            {
                throw exc;
            }

        }

        //private void DesHabilitaExtension(RenderingExtension ext) 
        //{
        //    FieldInfo info = ext.GetType().GetField("m_localizedName", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (info != null)
        //    {
        //        var rsExtension = info.GetValue(ext) as Extension;
        //        if (rsExtension != null)
        //            rsExtension.Visible = false;

        //    };

        //    return;
        //}

        #region Enumeradores para proceso
        private enum eProcedimiento
        {
            SI = 0,
            NO = 1
        };

        #endregion

        private void Guardar()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    if (Estudio == null)
                    {

                        ProcesaMultiSelect();//PROCESA LOS ELEMENTOS QUE SE HAN SELECCIONADO POR EL USUARIO

                        var NuevoEstudio = new SOCIOECONOMICO();
                        var NuevoEstudioGrupoPrimario = new SOCIOE_GPOFAMPRI();
                        var NuevoEstudioGrupoSecundario = new SOCIOE_GPOFAMSEC();
                        var CaractGrupoPrimario = new List<SOCIOE_GPOFAMPRI_CARAC>();
                        var CaractGrupoSecundario = new List<SOCIOE_GPOFAMSEC_CARAC>();


                        #region Datos Basicos
                        NuevoEstudio.DICTAMEN = DictamenDescripcion;
                        NuevoEstudio.DICTAMEN_FEC = FechaEstudio;
                        NuevoEstudio.ID_ANIO = SelectIngreso.ID_ANIO;
                        NuevoEstudio.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        NuevoEstudio.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        NuevoEstudio.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        NuevoEstudio.SALARIO = Salario;
                        #endregion
                        #region Datos del grupo primario
                        NuevoEstudioGrupoPrimario.EGRESO_MENSUAL = EgresoMensualPrimario;
                        NuevoEstudioGrupoPrimario.ANTECEDENTE = AntecedentePrimario;
                        NuevoEstudioGrupoPrimario.FAM_ANTECEDENTE = FamiliarAntecedentePrimario;
                        NuevoEstudioGrupoPrimario.GRUPO_FAMILIAR = GrupoFamiliarPrimario;
                        NuevoEstudioGrupoPrimario.ID_ANIO = SelectIngreso.ID_ANIO;
                        NuevoEstudioGrupoPrimario.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        NuevoEstudioGrupoPrimario.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        NuevoEstudioGrupoPrimario.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        NuevoEstudioGrupoPrimario.INGRESO_MENSUAL = IngresoMensualPrimario;
                        NuevoEstudioGrupoPrimario.NIVEL_SOCIO_CULTURAL = NivelSocioCulturalPrimario;
                        NuevoEstudioGrupoPrimario.PERSONAS_LABORAN = NoPersonasTrabajanPrimario;
                        NuevoEstudioGrupoPrimario.PERSONAS_VIVEN_HOGAR = NoPersonasVivenHogar;
                        NuevoEstudioGrupoPrimario.RELACION_INTRAFAMILIAR = RelacionIntroFamiliarPrimario;
                        NuevoEstudioGrupoPrimario.VIVIENDA_CONDICIONES = ViviendaCondicionesPrimario;
                        NuevoEstudioGrupoPrimario.VIVIENDA_ZONA = ViviendaZonaPrimario;

                        if (ListCaractPrimario.Any())
                        {
                            foreach (var item in ListCaractPrimario)
                            {
                                var CaractNuevo = new SOCIOE_GPOFAMPRI_CARAC();
                                CaractNuevo.ID_ANIO = SelectIngreso.ID_ANIO;
                                CaractNuevo.ID_CENTRO = SelectIngreso.ID_CENTRO;
                                CaractNuevo.ID_CLAVE = item.ID_CLAVE;
                                CaractNuevo.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                                CaractNuevo.ID_INGRESO = SelectIngreso.ID_INGRESO;
                                CaractNuevo.ID_TIPO = item.ID_TIPO;
                                CaractNuevo.REGISTRO_FEC = Fechas.GetFechaDateServer;
                                CaractGrupoPrimario.Add(CaractNuevo);
                            }
                        }

                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No ha marcado al menos una de las características del grupo primario.");
                            return;
                        }

                        if (ListCaractSecundario.Any())
                        {
                            foreach (var item in ListCaractSecundario)
                            {
                                var CaractNuevo = new SOCIOE_GPOFAMSEC_CARAC();
                                CaractNuevo.ID_ANIO = SelectIngreso.ID_ANIO;
                                CaractNuevo.ID_CENTRO = SelectIngreso.ID_CENTRO;
                                CaractNuevo.ID_CLAVE = item.ID_CLAVE;
                                CaractNuevo.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                                CaractNuevo.ID_INGRESO = SelectIngreso.ID_INGRESO;
                                CaractNuevo.ID_TIPO = item.ID_TIPO;
                                CaractNuevo.REGISTRO_FEC = Fechas.GetFechaDateServer;
                                CaractGrupoSecundario.Add(CaractNuevo);
                            }
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No ha marcado al menos una de las características del grupo secundario.");
                            return;
                        }

                        #endregion
                        #region Datos del grupo secundario
                        NuevoEstudioGrupoSecundario.ANTECEDENTE = AntecedenteSecundario;
                        NuevoEstudioGrupoSecundario.APOYO_ECONOMICO = ApoyoEconomico;
                        NuevoEstudioGrupoSecundario.EGRESO_MENSUAL = EgresoMensualSecundario;
                        NuevoEstudioGrupoSecundario.FAM_ANTECEDENTE = FamiliarAntecedenteSecundario;
                        NuevoEstudioGrupoSecundario.FRECUENCIA = Frecuencia;
                        NuevoEstudioGrupoSecundario.GRUPO_FAMILIAR = GrupoFamiliarSecundario;
                        NuevoEstudioGrupoSecundario.ID_ANIO = SelectIngreso.ID_ANIO;
                        NuevoEstudioGrupoSecundario.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        NuevoEstudioGrupoSecundario.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        NuevoEstudioGrupoSecundario.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        NuevoEstudioGrupoSecundario.INGRESO_MENSUAL = IngresoMensualSecundario;
                        NuevoEstudioGrupoSecundario.NIVEL_SOCIO_CULTURAL = NivelSocioCulturalSecundario;
                        NuevoEstudioGrupoSecundario.PERSONAS_LABORAN = PersonasLaboranSecundario;
                        NuevoEstudioGrupoSecundario.RECIBE_VISITA = RecibeVisita;
                        NuevoEstudioGrupoSecundario.RELACION_INTRAFAMILIAR = RelacionIntroFamiliarSecundario;
                        NuevoEstudioGrupoSecundario.VISITA = DeQuien;
                        NuevoEstudioGrupoSecundario.VIVIENDA_CONDICIONES = ViviendaCondicionesSecundario;
                        NuevoEstudioGrupoSecundario.VIVIENDA_ZONA = ViviendaZonaSecundario;
                        NuevoEstudioGrupoSecundario.MOTIVO_NO_VISITA = RazonNoRecibeVisita;
                        #endregion

                        new cEstudioSocioEconomico().Insertar(NuevoEstudio, NuevoEstudioGrupoPrimario, NuevoEstudioGrupoSecundario, CaractGrupoPrimario, CaractGrupoSecundario);
                        MenuGuardarEnabled = PInsertar = PEditar = false;

                    }

                    else
                    {
                        var Nuevo = new SOCIOECONOMICO();
                        Nuevo.DICTAMEN = DictamenDescripcion;
                        Nuevo.DICTAMEN_FEC = FechaEstudio;
                        Nuevo.ID_ANIO = SelectIngreso.ID_ANIO;
                        Nuevo.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        Nuevo.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        Nuevo.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        Nuevo.SALARIO = Salario;

                        var NuevoEstudioPrimario = new SOCIOE_GPOFAMPRI();
                        NuevoEstudioPrimario.EGRESO_MENSUAL = EgresoMensualPrimario;
                        NuevoEstudioPrimario.ANTECEDENTE = AntecedentePrimario;
                        NuevoEstudioPrimario.FAM_ANTECEDENTE = FamiliarAntecedentePrimario;
                        NuevoEstudioPrimario.GRUPO_FAMILIAR = GrupoFamiliarPrimario;
                        NuevoEstudioPrimario.ID_ANIO = SelectIngreso.ID_ANIO;
                        NuevoEstudioPrimario.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        NuevoEstudioPrimario.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        NuevoEstudioPrimario.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        NuevoEstudioPrimario.INGRESO_MENSUAL = IngresoMensualPrimario;
                        NuevoEstudioPrimario.NIVEL_SOCIO_CULTURAL = NivelSocioCulturalPrimario;
                        NuevoEstudioPrimario.PERSONAS_LABORAN = NoPersonasTrabajanPrimario;
                        NuevoEstudioPrimario.PERSONAS_VIVEN_HOGAR = NoPersonasVivenHogar;
                        NuevoEstudioPrimario.RELACION_INTRAFAMILIAR = RelacionIntroFamiliarPrimario;
                        NuevoEstudioPrimario.VIVIENDA_CONDICIONES = ViviendaCondicionesPrimario;
                        NuevoEstudioPrimario.VIVIENDA_ZONA = ViviendaZonaPrimario;

                        ProcesaMultiSelect();
                        var ListCaracteristicasGrupoPrimario = new List<SOCIOE_GPOFAMPRI_CARAC>();
                        if (ListCaractPrimario.Any())
                        {
                            foreach (var item in ListCaractPrimario)
                            {
                                var CaractNuevo = new SOCIOE_GPOFAMPRI_CARAC();
                                CaractNuevo.ID_ANIO = SelectIngreso.ID_ANIO;
                                CaractNuevo.ID_CENTRO = SelectIngreso.ID_CENTRO;
                                CaractNuevo.ID_CLAVE = item.ID_CLAVE;
                                CaractNuevo.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                                CaractNuevo.ID_INGRESO = SelectIngreso.ID_INGRESO;
                                CaractNuevo.ID_TIPO = item.ID_TIPO;
                                CaractNuevo.REGISTRO_FEC = Fechas.GetFechaDateServer;
                                ListCaracteristicasGrupoPrimario.Add(CaractNuevo);
                            }
                        };


                        var ListCaracteristicasGrupoSecundario = new List<SOCIOE_GPOFAMSEC_CARAC>();
                        if (ListCaractSecundario.Any())
                        {
                            foreach (var item in ListCaractSecundario)
                            {
                                var CaractNuevo = new SOCIOE_GPOFAMSEC_CARAC();
                                CaractNuevo.ID_ANIO = SelectIngreso.ID_ANIO;
                                CaractNuevo.ID_CENTRO = SelectIngreso.ID_CENTRO;
                                CaractNuevo.ID_CLAVE = item.ID_CLAVE;
                                CaractNuevo.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                                CaractNuevo.ID_INGRESO = SelectIngreso.ID_INGRESO;
                                CaractNuevo.ID_TIPO = item.ID_TIPO;
                                CaractNuevo.REGISTRO_FEC = Fechas.GetFechaDateServer;
                                ListCaracteristicasGrupoSecundario.Add(CaractNuevo);
                            }
                        }


                        var NuevoEstudioSecundario = new SOCIOE_GPOFAMSEC();
                        NuevoEstudioSecundario.ANTECEDENTE = AntecedenteSecundario;
                        NuevoEstudioSecundario.APOYO_ECONOMICO = ApoyoEconomico;
                        NuevoEstudioSecundario.EGRESO_MENSUAL = EgresoMensualSecundario;
                        NuevoEstudioSecundario.FAM_ANTECEDENTE = FamiliarAntecedenteSecundario;
                        NuevoEstudioSecundario.FRECUENCIA = Frecuencia;
                        NuevoEstudioSecundario.GRUPO_FAMILIAR = GrupoFamiliarSecundario;
                        NuevoEstudioSecundario.ID_ANIO = SelectIngreso.ID_ANIO;
                        NuevoEstudioSecundario.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        NuevoEstudioSecundario.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        NuevoEstudioSecundario.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        NuevoEstudioSecundario.INGRESO_MENSUAL = IngresoMensualSecundario;
                        NuevoEstudioSecundario.NIVEL_SOCIO_CULTURAL = NivelSocioCulturalSecundario;
                        NuevoEstudioSecundario.PERSONAS_LABORAN = PersonasLaboranSecundario;
                        NuevoEstudioSecundario.RECIBE_VISITA = RecibeVisita;
                        NuevoEstudioSecundario.RELACION_INTRAFAMILIAR = RelacionIntroFamiliarSecundario;
                        NuevoEstudioSecundario.VISITA = DeQuien;
                        NuevoEstudioSecundario.VIVIENDA_CONDICIONES = ViviendaCondicionesSecundario;
                        NuevoEstudioSecundario.VIVIENDA_ZONA = ViviendaZonaSecundario;
                        NuevoEstudioSecundario.MOTIVO_NO_VISITA = RazonNoRecibeVisita;

                        new cEstudioSocioEconomico().ActualizarEstudioTransaccion(Nuevo, NuevoEstudioPrimario, NuevoEstudioSecundario, ListCaracteristicasGrupoPrimario, ListCaracteristicasGrupoSecundario);
                    }
                }
            }

            catch (Exception exc)
            {
            }
        }


        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    var textbox = obj as TextBox;

                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "AnioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    AnioBuscar = int.Parse(textbox.Text);
                                else
                                    AnioBuscar = null;
                                break;
                            case "FolioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    FolioBuscar = int.Parse(textbox.Text);
                                else
                                    FolioBuscar = null;
                                break;
                        }
                    }
                }

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;

                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }


        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new List<IMPUTADO>();
            }
        }


        private void SeleccionaIngreso()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    AnioD = SelectIngreso.ID_ANIO;
                    FolioD = SelectIngreso.ID_IMPUTADO;
                    PaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                    MaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                    NombreD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                    IngresosD = SelectIngreso.ID_INGRESO;

                    if (SelectIngreso.CAMA != null)
                        UbicacionD = string.Format("{0}-{1}{2}-{3}",
                            SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty, SelectIngreso.ID_UB_CAMA);
                    else
                        UbicacionD = string.Empty;

                    TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.TIPO_SEGURIDAD.DESCR) ? SelectIngreso.TIPO_SEGURIDAD.DESCR.Trim() : string.Empty : string.Empty;
                    FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO;
                    ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(SelectIngreso.CLASIFICACION_JURIDICA.DESCR) ? SelectIngreso.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty;
                    EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO != null ? !string.IsNullOrEmpty(SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR) ? SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR.Trim() : string.Empty : string.Empty;
                    GetDatosEstudio();
                    ConsultaSeleccionados(Estudio);
                }

                else
                    return;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }

        private void ConsultaSeleccionados(SOCIOECONOMICO Entity)
        {
            try
            {
                if (Entity != null)
                {
                    #region Grupo Primario
                    if (Entity.SOCIOE_GPOFAMPRI != null)
                    {
                        foreach (var item in Entity.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC)
                        {
                            if (item.ID_TIPO == "M")
                            {
                                if (item.ID_CLAVE == 1) IsCartonPrimarioChecked = true;
                                if (item.ID_CLAVE == 2) IsAdobePrimarioChecked = true;
                                if (item.ID_CLAVE == 3) IsLadrilloPrimarioChecked = true;
                                if (item.ID_CLAVE == 4) IsBlockPrimarioChecked = true;
                                if (item.ID_CLAVE == 5) IsMaderaPrimarioChecked = true;
                                if (item.ID_CLAVE == 6) IsMaterialesOtrosPrimarioChecked = true;
                            }

                            if (item.ID_TIPO == "D")
                            {
                                if (item.ID_CLAVE == 1) IsSalaPrimarioChecked = true;
                                if (item.ID_CLAVE == 2) IsCocinaPrimarioChecked = true;
                                if (item.ID_CLAVE == 3) IsComedorPrimarioChecked = true;
                                if (item.ID_CLAVE == 4) IsRecamaraChecked = true;
                                if (item.ID_CLAVE == 5) IsBanioChecked = true;
                                if (item.ID_CLAVE == 6) IsDistribucionPrimariaOtrosChecked = true;
                            }

                            if (item.ID_TIPO == "S")
                            {
                                if (item.ID_CLAVE == 1) IsEnergiaElectricaPrimariaChecked = true;
                                if (item.ID_CLAVE == 2) IsAguaPrimariaChecked = true;
                                if (item.ID_CLAVE == 3) IsDrenajePrimariaChecked = true;
                                if (item.ID_CLAVE == 4) IsPavimentoPrimarioChecked = true;
                                if (item.ID_CLAVE == 5) IsTelefonoPrimarioChecked = true;
                                if (item.ID_CLAVE == 6) IsTVCableChecked = true;
                            }

                            if (item.ID_TIPO == "E")
                            {
                                if (item.ID_CLAVE == 1) IsEstufaPrimarioChecked = true;
                                if (item.ID_CLAVE == 2) IsRefrigeradorPrimarioChecked = true;
                                if (item.ID_CLAVE == 3) IsMicroOndasPrimarioChecked = true;
                                if (item.ID_CLAVE == 4) IsTVPrimarioChecked = true;
                                if (item.ID_CLAVE == 5) IsLavadoraPrimarioChecked = true;
                                if (item.ID_CLAVE == 6) IsSecadoraPrimarioChecked = true;
                                if (item.ID_CLAVE == 7) IsComputadoraPrimarioChecked = true;
                                if (item.ID_CLAVE == 8) IsOtrosElectrodomesticosPrimarioChecked = true;
                            }

                            if (item.ID_TIPO == "T")
                            {
                                if (item.ID_CLAVE == 1) IsAutomovilPrimarioChecked = true;
                                if (item.ID_CLAVE == 2) IsAutobusPrimarioChecked = true;
                                if (item.ID_CLAVE == 3) IsOtrosMediosTransportePrimarioChecked = true;
                            }
                        }
                    }

                    #endregion

                    #region Grupo Secundario
                    if (Entity.SOCIOE_GPOFAMSEC != null)
                    {
                        foreach (var item in Entity.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC)
                        {
                            if (item.ID_TIPO == "M")
                            {
                                if (item.ID_CLAVE == 1) IsCartonSecundarioChecked = true;
                                if (item.ID_CLAVE == 2) IsAdobeSecundarioChecked = true;
                                if (item.ID_CLAVE == 3) IsLadrilloSecundarioChecked = true;
                                if (item.ID_CLAVE == 4) IsBlockSecundarioChecked = true;
                                if (item.ID_CLAVE == 5) IsMaderaSecundarioChecked = true;
                                if (item.ID_CLAVE == 6) IsOtrosMaterialesSecundarioChecked = true;
                            }

                            if (item.ID_TIPO == "D")
                            {
                                if (item.ID_CLAVE == 1) IsSalaSecundarioChecked = true;
                                if (item.ID_CLAVE == 2) IsCocinaSecundarioChecked = true;
                                if (item.ID_CLAVE == 3) IsComedorSecundarioChecked = true;
                                if (item.ID_CLAVE == 4) IsRecamaraSecundarioChecked = true;
                                if (item.ID_CLAVE == 5) IsBanioSecundarioChecked = true;
                                if (item.ID_CLAVE == 6) IsOtrosDistribucionSecundarioChecked = true;
                            }

                            if (item.ID_TIPO == "S")
                            {
                                if (item.ID_CLAVE == 1) IsEnergiaElectricaSecundariaChecked = true;
                                if (item.ID_CLAVE == 2) IsAguaSecundarioChecked = true;
                                if (item.ID_CLAVE == 3) IsDrenajeSecundarioChecked = true;
                                if (item.ID_CLAVE == 4) IsPavimentoSecundarioChecked = true;
                                if (item.ID_CLAVE == 5) IsTelefonoSecundarioChecked = true;
                                if (item.ID_CLAVE == 6) IsTVCableSecundarioChecked = true;
                            }

                            if (item.ID_TIPO == "E")
                            {
                                if (item.ID_CLAVE == 1) IsEstufaSecundarioChecked = true;
                                if (item.ID_CLAVE == 2) IsRefrigeradorSecundarioChecked = true;
                                if (item.ID_CLAVE == 3) IsMicroOndasSecundarioChecked = true;
                                if (item.ID_CLAVE == 4) IsTVSecundarioChecked = true;
                                if (item.ID_CLAVE == 5) IsLavadoraSecundarioChecked = true;
                                if (item.ID_CLAVE == 6) IsSecadoraSecundarioChecked = true;
                                if (item.ID_CLAVE == 7) IsComputadoraSecundariaChecked = true;
                                if (item.ID_CLAVE == 8) IsOtrosElectrodomesticosChecked = true;
                            }

                            if (item.ID_TIPO == "T")
                            {
                                if (item.ID_CLAVE == 1) IsAutomovilSecundarioChecked = true;
                                if (item.ID_CLAVE == 2) IsAutobusSecundarioChecked = true;
                                if (item.ID_CLAVE == 3) IsOtrosMediosTransporteChecked = true;
                            }

                            if(item.ID_TIPO == "A")
                            {
                                if (item.ID_CLAVE == 1) IsGiroChecked = true;
                                if (item.ID_CLAVE == 1) IsCuentaBChecked = true;
                                if (item.ID_CLAVE == 3) IsDepositoChecked = true;
                            }
                        }
                    }


                    #endregion
                }
            }

            catch (Exception exc)
            {
            }
        }

        private void GetDatosEstudio()
        {
            var EstudioAnterior = new cEstudioSocioEconomico().Obtener(SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
            DatosGrupoFamiliarPrimarioEnabled = DatosGrupoFamiliarSecundarioEnabled = DictamenSocioEconomicoEnabled = true;

            if (EstudioAnterior == null)//No cuenta con un estudio previo
            {
                Estudio = null;
                MenuGuardarEnabled = true;
                GrupoFamiliarPrimario = GrupoFamiliarSecundario = RelacionIntroFamiliarPrimario = RelacionIntroFamiliarSecundario = ViviendaZonaPrimario =
                NivelSocioCulturalSecundario = ViviendaZonaSecundario = ViviendaCondicionesPrimario = ViviendaCondicionesSecundario = NivelSocioCulturalPrimario = DictamenDescripcion = AntecedenteSecundario = DeQuien = Frecuencia = RazonNoRecibeVisita = string.Empty;
                FamiliarAntecedentePrimario = FamiliarAntecedenteSecundario = RecibeVisita = ApoyoEconomico = -1;
                FechaEstudio = null;
                PersonasLaboranSecundario = 0;
                EgresoMensualSecundario = IngresoMensualSecundario = 0;
            }

            else
            {
                Estudio = new SOCIOECONOMICO();
                Estudio = EstudioAnterior;

                #region Datos del grupo familiar primario
                GrupoFamiliarPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR : string.Empty;
                RelacionIntroFamiliarPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR : string.Empty;
                NoPersonasVivenHogar = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.PERSONAS_VIVEN_HOGAR : new short?();
                NoPersonasTrabajanPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.PERSONAS_LABORAN : new short?();
                IngresoMensualPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.INGRESO_MENSUAL : new int?();
                EgresoMensualPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.EGRESO_MENSUAL : new int?();
                FamiliarAntecedentePrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE : new decimal?();
                AntecedentePrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.ANTECEDENTE : string.Empty;
                ViviendaZonaPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA : string.Empty;
                ViviendaCondicionesPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES : string.Empty;
                NivelSocioCulturalPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL : string.Empty;
                #endregion

                #region Datos del grupo familiar secundario
                GrupoFamiliarSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR : string.Empty;
                RelacionIntroFamiliarSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR : string.Empty;
                PersonasLaboranSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.PERSONAS_LABORAN : new short?();
                IngresoMensualSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.INGRESO_MENSUAL : new int?();
                EgresoMensualSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.EGRESO_MENSUAL : new int?();
                FamiliarAntecedenteSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE : new decimal?();
                AntecedenteSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.ANTECEDENTE : string.Empty;
                ViviendaZonaSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA : string.Empty;
                ViviendaCondicionesSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES : string.Empty;
                NivelSocioCulturalSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL : string.Empty;
                RecibeVisita = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.RECIBE_VISITA : new decimal?();
                DeQuien = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.VISITA : string.Empty;
                Frecuencia = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.FRECUENCIA : string.Empty;
                FechaEstudio = EstudioAnterior.DICTAMEN_FEC;
                RazonNoRecibeVisita = EstudioAnterior.SOCIOE_GPOFAMSEC.MOTIVO_NO_VISITA;
                ApoyoEconomico = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO : new decimal?(); 
                #endregion

                #region Dictamen
                DictamenDescripcion = EstudioAnterior.DICTAMEN;
                Salario = EstudioAnterior.SALARIO;
                #endregion
            }

            SetValidaciones();
        }

        #endregion

        #region Busqueda
        private void LimpiarBusqueda() 
        {
            AuxIngreso = SelectIngreso;
            ListExpediente = null;
            SelectExpediente = null;
            ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
            AnioBuscar = null;
            FolioBuscar = null;
            Pagina =  1;
            SeguirCargando = true;
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.ESTUDIO_SOCIOECONOMICO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            MenuGuardarEnabled = true;
                        if (p.EDITAR == 1)
                            MenuGuardarEnabled = PEditar = true;
                        if (p.CONSULTAR == 1)
                            MenuBuscarEnabled = PConsultar = true;
                        if (p.IMPRIMIR == 1)
                            MenuReporteEnabled = PImprimir = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region Cambio SelectedItem de Busqueda de Expediente
        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_expediente":
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            //MUESTRA LOS INGRESOS
                            if (SelectExpediente.INGRESO != null && SelectExpediente.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            }
                            else
                                EmptyIngresoVisible = true;

                            //OBTENEMOS FOTO DE FRENTE
                            if (SelectIngreso != null)
                            {
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    ImagenImputado = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenImputado = new Imagenes().getImagenPerson();
                        }
                        break;
                }
            }
        }
        #endregion

    }
}
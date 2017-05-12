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
    partial class CreacionListasExamenPViewModel : ValidationViewModelBase
    {
        public CreacionListasExamenPViewModel() { }
        private async void ModelEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
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
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        if (ListExpediente[0].INGRESO != null && !ListExpediente[0].INGRESO.Any())
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                            return;
                        };

                        foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                        {
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                SelectExpediente = null;
                                SelectIngreso = null;
                                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                                return;
                            };
                        };

                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        };

                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                                ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        };


                        SelectExpediente = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        SelectedInterno = SelectExpediente;
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        }

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

                    SelectExpediente = null;
                    SelectIngreso = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
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

        private string ProcesaFrecuencia(INGRESO Entity)
        {
            try
            {
                var _VisitanteIngreso = new cAduanaIngreso().GetData(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ADUANA != null && x.ADUANA.ID_TIPO_PERSONA == (short)eTiposAduana.VISITA); //new cVisitanteIngreso().ObtenerXImputado(Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO);
                if (_VisitanteIngreso.Any())//SOLO VISITAS NORMALES, NO LEGALES NI ACTUARIOS NI DEMAS
                {
                    var _VisitasPorAnio = _VisitanteIngreso.GroupBy(x => x.ENTRADA_FEC.Value.Year);//VISITAS POR AñO
                    if (_VisitasPorAnio.Any())
                    {
                        var _DetalleMeses = _VisitasPorAnio.OrderByDescending(x => x.Count()).FirstOrDefault();// TOMA EL MES CON VAS VISITAS RECIBIDAS
                        if (_DetalleMeses != null)
                        {
                            if (_DetalleMeses.Count() >= (int)eFrecuencia.SEMANAL)
                                return "SEMANAL";
                            if (_DetalleMeses.Count() == (int)eFrecuencia.QUINCENAL)
                                return "QUINCENAL";
                            if (_DetalleMeses.Count() == (int)eFrecuencia.MENSUAL)
                                return "MENSUAL";
                            if (_DetalleMeses.Count() == (int)eFrecuencia.ANUAL)
                                return "ANUAL";
                        };
                    };
                };

                return "SIN DATO";
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region TRASLA2
        private void ProcesaTraslado(PERSONALIDAD Entity, short Opcion)
        {
            try
            {
                if (Entity == null)
                    return;

                switch (Opcion)
                {   ///LIMPIA SIEMPRE LOS DATOS ANTERIORES
                    case (short)eTipoSolicitudTraslado.ISLAS:
                        DictamenTrasladoIslas = EstadoAdiccionTrasladoIslas = CriminogTrasladoIslas = DictamenTrasladoIslas = ContinTratamTrasladoIslas = TieneAnuenciaTrasladoIslas = TratamSugTrasladoIslas = IntimidacionPenaTrasladoIslas = string.Empty;
                        IdEgocentrismoTrasladoIslas = IdLabAfecTrasladoIslas = IdAgresivIntrTrasladoIslas = IdPeligroTrasladoIslas = -1;
                        FechaTrasladoIslas = null;
                        var _trasladoIslas = new cTrasladoIslasPersonalidad().GetData(c => c.ID_ESTUDIO == Entity.ID_ESTUDIO && c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_INGRESO == Entity.ID_INGRESO && c.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();
                        if (_trasladoIslas != null)
                        {
                            CriminogTrasladoIslas = _trasladoIslas.CRIMINOGENESIS;
                            IdEgocentrismoTrasladoIslas = _trasladoIslas.EGOCENTRISMO;
                            IdLabAfecTrasladoIslas = _trasladoIslas.LABILIDAD_AFECTIVA;
                            EstadoAdiccionTrasladoIslas = _trasladoIslas.EN_CASO_ADICCION;
                            IntimidacionPenaTrasladoIslas = _trasladoIslas.INTIMIDACION_PENA;
                            IdAgresivIntrTrasladoIslas = _trasladoIslas.AGRESIVIDAD_INTRAMUROS;
                            IdPeligroTrasladoIslas = _trasladoIslas.ID_PELIGROSIDAD;
                            DictamenTrasladoIslas = _trasladoIslas.DICTAMEN_TRASLADO;
                            ContinTratamTrasladoIslas = _trasladoIslas.CONTINUE_TRATAMIENTO;
                            TieneAnuenciaTrasladoIslas = _trasladoIslas.ANUENCIA_FIRMADA;
                            TratamSugTrasladoIslas = _trasladoIslas.TRATAMIENTO_SUGERIDO;
                            FechaTrasladoIslas = _trasladoIslas.ESTUDIO_FEC;
                        };

                        ValidacionesTrasladoIslasPersonalidad();
                        LoadListasTraslados();
                        break;

                    case (short)eTipoSolicitudTraslado.INTERNACIONAL:
                        NombreCentroOrigenTrasladoIntern = NombreCentroSolicitud = NombreTrasladoInternac = EdadTrasladoInternac = EdoCivilTrasladoInternac = OriginarioTrasladoInternac = string.Empty;
                        LugarResidenciaTrasladoInternac = EscolaridadTrasladoInternac = OcupacionPreviaTrasladoInternac = string.Empty;
                        LstDelitosUno = new ObservableCollection<DelitosGrid>();
                        AntecedentesPenalesTrasladoInternacional = VersionDelitoTrasladoInternacional = ClinicSanoTrasladoInternacional = string.Empty;
                        LstDelitosDos = new ObservableCollection<DelitosGrid>();
                        IndicaPadecimientoTrasladoInternacional = TratamientoActualTrasladoInternacional = CoeficIntTrasladoInternacional = string.Empty;
                        DanioCerebTrasladoInternacional = OtrosAspectosPersonalidadTrasladoInternacional = string.Empty;
                        ApoyoPadresTrasladoInternacional = ConyugeTrasladoInternacional = string.Empty;
                        LstVisitasTraslInt = new ObservableCollection<TRASLADO_INTERNACIONAL_VISITA>();
                        FrecuenciaVisitaTrasladoInternacional = NoRecibeVisitaCausasTrasladoInternacional = CartaArraigoCuentaTrasladoInternacional = string.Empty;
                        DomicilioTrasladoInternacional = AnuenciaCupoTrasladoInternacional = string.Empty;
                        FechaAnuenciaTrasladoInternacional = null;
                        NivelSocioETrasladoInternacional = EstudiaActualTrasladoInternacional = CausasNoEstudiaTrasladoInternacional = OtrosCursosCapacRecibTrasladoInternacional = string.Empty;
                        AcualmenteTrabajaInstTrasladoInternacional = SenialeOcupacionTrasladoInternacional = NegativoSenialeCausas = string.Empty;
                        TotalDiasEfectivTrasladoInternacional = 0;
                        ConductaConsidTrasladoInternacional = -1;
                        LstSancionesTrasladoInt = new ObservableCollection<TRASLADO_INTERNACIONAL_SANCION>();
                        IdPeligroTrasladoInternacional = -1;
                        AdiccionToxicosTrasladoInternacional = CasoAfirmativoCualesTrasladoInternacional = string.Empty;
                        IsPsicoTrasladoInterNacionalChecked = IsEducTrasladoInterNacionalChecked = IsLabTrasladoInterNacionalChecked = IsOtrosTrasladoInterNacionalChecked = false;
                        EspecifiqueOtroTratamientoTrasladoInterNacional = OtrosAspectosTrasladoInternacional = string.Empty;
                        var _trasladoInternacional = new cTrasladoInternacionalPersonalidad().GetData(c => c.ID_ESTUDIO == Entity.ID_ESTUDIO && c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_INGRESO == Entity.ID_INGRESO && c.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();

                        if (Entity.INGRESO != null)
                            if (Entity.INGRESO.GRUPO_PARTICIPANTE != null && Entity.INGRESO.GRUPO_PARTICIPANTE.Any())
                            {
                                var _GruposPart = Entity.INGRESO.GRUPO_PARTICIPANTE.Where(x => x.ID_TIPO_PROGRAMA == 147 || x.ID_TIPO_PROGRAMA == 148 || x.ID_TIPO_PROGRAMA == 149);
                                foreach (var item in _GruposPart)
                                    if (item.ACTIVIDAD != null)
                                        if (!string.IsNullOrEmpty(item.ACTIVIDAD.DESCR))
                                            if ((item.ACTIVIDAD.DESCR.Trim().Length + OtrosCursosCapacRecibTrasladoInternacional.Length) < 500)
                                                OtrosCursosCapacRecibTrasladoInternacional += string.Format("{0}, ", item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty);
                            };


                        var _centro = new cCentro().GetData(x => x.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();
                        if (_centro != null)
                            NombreCentroOrigenTrasladoIntern = !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty;

                        NombreTrasladoInternac = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO != null ?
                            string.Format("{0} {1} {2}", !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.NOMBRE) ? Entity.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.PATERNO) ? Entity.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.MATERNO) ? Entity.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty;

                        EdadTrasladoInternac = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO != null ?
                            new Fechas().CalculaEdad(Entity.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty : string.Empty;

                        FrecuenciaVisitaTrasladoInternacional = ProcesaFrecuencia(Entity.INGRESO);
                        //EdoCivilTrasladoInternac = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO != null ?
                        //    Entity.INGRESO.IMPUTADO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR) ? Entity.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR.Trim()
                        //    : string.Empty : string.Empty : string.Empty : string.Empty;
                        EdoCivilTrasladoInternac = Entity.INGRESO != null ?
                            Entity.INGRESO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(Entity.INGRESO.ESTADO_CIVIL.DESCR) ? Entity.INGRESO.ESTADO_CIVIL.DESCR.Trim()
                            : string.Empty : string.Empty : string.Empty;

                        if (Entity.INGRESO != null)
                            if (Entity.INGRESO.IMPUTADO != null)
                            {
                                var _EstadoNac = new cEntidad().GetData(x => x.ID_ENTIDAD == Entity.INGRESO.IMPUTADO.NACIMIENTO_ESTADO).FirstOrDefault();
                                var _MunicipioNac = _EstadoNac != null ? new cMunicipio().GetData(x => x.ID_MUNICIPIO == Entity.INGRESO.IMPUTADO.NACIMIENTO_MUNICIPIO && x.ID_ENTIDAD == _EstadoNac.ID_ENTIDAD).FirstOrDefault() : null;
                                OriginarioTrasladoInternac = string.Format("{0}, {1}", _EstadoNac != null ? !string.IsNullOrEmpty(_EstadoNac.DESCR) ? _EstadoNac.DESCR.Trim() : string.Empty : string.Empty, _MunicipioNac != null ? !string.IsNullOrEmpty(_MunicipioNac.MUNICIPIO1) ? _MunicipioNac.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                                if (Entity.INGRESO.IMPUTADO.IMPUTADO_PADRES.Any())
                                    foreach (var item in Entity.INGRESO.IMPUTADO.IMPUTADO_PADRES)
                                        LugarResidenciaTrasladoInternac = string.Format("{0} {1} ", !string.IsNullOrEmpty(item.CALLE) ? item.CALLE.Trim() : string.Empty, item.NUM_EXT.HasValue ? item.NUM_EXT.Value.ToString() : string.Empty);
                            };

                        var _causasPenales = Entity.INGRESO != null ? Entity.INGRESO.CAUSA_PENAL : null;
                        if (_causasPenales != null && _causasPenales.Any())
                            foreach (var item in _causasPenales)
                                foreach (var item2 in item.SENTENCIA)
                                    foreach (var item3 in item2.SENTENCIA_DELITO)
                                        LstDelitosUno.Add(new DelitosGrid
                                            {
                                                Delito = item3.DESCR_DELITO,
                                                APartir = item2.FEC_INICIO_COMPURGACION.HasValue ? item2.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                FComun = item3.ID_FUERO == "C" ? "X" : string.Empty,
                                                FFederal = item3.ID_FUERO == "F" ? "X" : string.Empty,
                                                Pena = string.Format("{0} , {1} , {2} ",
                                                item2.ANIOS.HasValue ? item2.ANIOS.Value > 0 ? string.Concat(item2.ANIOS.Value + " AÑOS") : "0 AÑOS" : "0 AÑOS",
                                                item2.MESES.HasValue ? item2.MESES.Value > 0 ? string.Concat(item2.MESES.Value + " MESES") : "0 MESES" : "0 MESES",
                                                item2.DIAS.HasValue ? item2.DIAS.Value > 0 ? string.Concat(item2.DIAS.Value + " DÍAS") : "0 DÍAS" : "0 DÍAS"),
                                                Proceso = string.Format("{0} / {1} ", item.CP_ANIO.HasValue ? item.CP_ANIO.Value.ToString() : string.Empty, item.CP_FOLIO.HasValue ? item.CP_FOLIO.Value.ToString() : string.Empty)
                                            });

                        var Imputado = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO : null;
                        if (Imputado != null)
                            if (Imputado.INGRESO != null && Imputado.INGRESO.Any())
                                foreach (var item in Imputado.INGRESO)
                                    foreach (var item2 in item.CAUSA_PENAL)
                                        foreach (var item3 in item2.SENTENCIA)
                                            foreach (var item4 in item3.SENTENCIA_DELITO)
                                                LstDelitosDos.Add(new DelitosGrid
                                                {
                                                    Delito = !string.IsNullOrEmpty(item4.DESCR_DELITO) ? item4.DESCR_DELITO.Trim() : string.Empty,
                                                    APartir = item4.SENTENCIA != null ? item4.SENTENCIA.FEC_INICIO_COMPURGACION.HasValue ? item4.SENTENCIA.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty,
                                                    FComun = item4.ID_FUERO == "C" ? "X" : string.Empty,
                                                    FFederal = item4.ID_FUERO == "F" ? "X" : string.Empty,
                                                    Pena = string.Format("{0} , {1} , {2} ",
                                                                            item3.ANIOS.HasValue ? item3.ANIOS.Value > 0 ? string.Concat(item3.ANIOS.Value + " AÑOS") : "0 AÑOS" : "0 AÑOS",
                                                                            item3.MESES.HasValue ? item3.MESES.Value > 0 ? string.Concat(item3.MESES.Value + " MESES") : "0 MESES" : "0 MESES",
                                                                            item3.DIAS.HasValue ? item3.DIAS.Value > 0 ? string.Concat(item3.DIAS.Value + " DÍAS") : "0 DÍAS" : "0 DÍAS"),
                                                    Proceso = string.Format("{0} / {1} ", item2.CP_ANIO.HasValue ? item2.CP_ANIO.Value.ToString() : string.Empty, item2.CP_FOLIO.HasValue ? item2.CP_FOLIO.Value.ToString() : string.Empty)
                                                });

                        if (LstDelitosDos != null && LstDelitosDos.Any())
                            AntecedentesPenalesTrasladoInternacional = "S";

                        if (_trasladoInternacional == null)
                        {
                            //EscolaridadTrasladoInternac = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO != null ?
                            //    Entity.INGRESO.IMPUTADO.ID_ESCOLARIDAD.HasValue ? !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.ESCOLARIDAD.DESCR) ? Entity.INGRESO.IMPUTADO.ESCOLARIDAD.DESCR.Trim() :
                            //    string.Empty : string.Empty : string.Empty : string.Empty;
                            EscolaridadTrasladoInternac = Entity.INGRESO != null ?
                                Entity.INGRESO.ID_ESCOLARIDAD.HasValue ? !string.IsNullOrEmpty(Entity.INGRESO.ESCOLARIDAD.DESCR) ? Entity.INGRESO.ESCOLARIDAD.DESCR.Trim() :
                                string.Empty : string.Empty : string.Empty;

                            var _EmiByIngreso = Entity.INGRESO != null ? Entity.INGRESO.EMI_INGRESO.Any() ? Entity.INGRESO.EMI_INGRESO.FirstOrDefault().EMI != null ? Entity.INGRESO.EMI_INGRESO.FirstOrDefault().EMI.EMI_ULTIMOS_EMPLEOS : null : null : null;
                            if (_EmiByIngreso != null)
                            {
                                var _UltimoEmpleo = _EmiByIngreso.FirstOrDefault(x => x.ULTIMO_EMPLEO_ANTES_DETENCION == "S");
                                if (_UltimoEmpleo != null)
                                    OcupacionPreviaTrasladoInternac = _UltimoEmpleo.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(_UltimoEmpleo.OCUPACION.DESCR) ? _UltimoEmpleo.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                            };

                            var _visitas = new cVisitaAutorizada().ObtenerXIngreso(Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO);
                            if (_visitas != null && _visitas.Any())
                                foreach (var item in _visitas)
                                {
                                    LstVisitasTraslInt.Add(new TRASLADO_INTERNACIONAL_VISITA
                                        {
                                            ID_ANIO = Entity.ID_ANIO,
                                            ID_CENTRO = Entity.ID_CENTRO,
                                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                                            ID_INGRESO = Entity.ID_INGRESO,
                                            ID_TIPO_REFERENCIA = item.ID_PARENTESCO,
                                            MATERNO = !string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Trim() : string.Empty,
                                            PATERNO = !string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Trim() : string.Empty,
                                            NOMBRE = !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Empty,
                                            TIPO_REFERENCIA = item.TIPO_REFERENCIA
                                        });
                                };


                            if (LstVisitasTraslInt != null && LstVisitasTraslInt.Any())
                            {
                                base.RemoveRule("EnabledSanoIntern");
                                base.RemoveRule("NoRecibeVisitaCausasTrasladoInternacional");// se asgura que no exista la regla, sino causaria una excepcion
                                base.AddRule(() => NoRecibeVisitaCausasTrasladoInternacional, () => !string.IsNullOrEmpty(NoRecibeVisitaCausasTrasladoInternacional), "ESPECIFIQUE RAZON NO VISITAS ES REQUERIDO!");
                                OnPropertyChanged("IndicaPadecimientoTrasladoInternacional");
                                OnPropertyChanged("EnabledSanoIntern");
                            }

                            var _sanciones = new cSancion().GetData(c => c.ID_ANIO == Entity.ID_ANIO && c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_INGRESO == Entity.ID_INGRESO && c.ID_CENTRO == Entity.ID_CENTRO);
                            if (_sanciones != null && _sanciones.Any())
                                foreach (var item in _sanciones)
                                {
                                    LstSancionesTrasladoInt.Add(new TRASLADO_INTERNACIONAL_SANCION
                                        {
                                            ID_ANIO = Entity.ID_ANIO,
                                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                                            ID_CENTRO = Entity.ID_CENTRO,
                                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                                            ID_INGRESO = Entity.ID_INGRESO,
                                            MOTIVO = item.INCIDENTE != null ? !string.IsNullOrEmpty(item.INCIDENTE.MOTIVO) ? item.INCIDENTE.MOTIVO.Trim() : string.Empty : string.Empty,
                                            SANCION = item.SANCION_TIPO != null ? !string.IsNullOrEmpty(item.SANCION_TIPO.DESCR) ? item.SANCION_TIPO.DESCR.Trim() : string.Empty : string.Empty,
                                            SANCION_FEC = item.INICIA_FEC
                                        });
                                };
                        }
                        else
                        {
                            EscolaridadTrasladoInternac = _trasladoInternacional.ESCOLARIDAD;//Revisar 
                            OcupacionPreviaTrasladoInternac = _trasladoInternacional.OCUPACION_PREVIA;//Revisar

                            if (_trasladoInternacional.TRASLADO_INTERNACIONAL_VISITA != null && _trasladoInternacional.TRASLADO_INTERNACIONAL_VISITA.Any())
                                foreach (var item in _trasladoInternacional.TRASLADO_INTERNACIONAL_VISITA)
                                    LstVisitasTraslInt.Add(item);

                            if (LstVisitasTraslInt != null && LstVisitasTraslInt.Any())
                            {
                                base.RemoveRule("EnabledSanoIntern");
                                base.RemoveRule("NoRecibeVisitaCausasTrasladoInternacional");
                                base.AddRule(() => NoRecibeVisitaCausasTrasladoInternacional, () => !string.IsNullOrEmpty(NoRecibeVisitaCausasTrasladoInternacional), "ESPECIFIQUE RAZON NO VISITAS ES REQUERIDO!");
                                OnPropertyChanged("IndicaPadecimientoTrasladoInternacional");
                                OnPropertyChanged("EnabledSanoIntern");
                            }

                            DescAgresividad = _trasladoInternacional.AGRESIVIDAD;
                            VersionDelitoTrasladoInternacional = _trasladoInternacional.VERSION_DELITO;
                            ClinicSanoTrasladoInternacional = _trasladoInternacional.CLINICAMENTE_SANO;
                            IndicaPadecimientoTrasladoInternacional = _trasladoInternacional.PADECIMIENTO;
                            TratamientoActualTrasladoInternacional = _trasladoInternacional.TRATAMIENTO_ACTUAL;
                            CoeficIntTrasladoInternacional = _trasladoInternacional.COEFICIENTE_INTELECTUAL;
                            DanioCerebTrasladoInternacional = _trasladoInternacional.DANIO_CEREBRAL;
                            OtrosAspectosPersonalidadTrasladoInternacional = _trasladoInternacional.OTROS_ASPECTOS;
                            ApoyoPadresTrasladoInternacional = _trasladoInternacional.APOYO_PADRES;
                            ConyugeTrasladoInternacional = _trasladoInternacional.CONYUGE;
                            FrecuenciaVisitaTrasladoInternacional = _trasladoInternacional.FRECUENCIA_VISITAS;
                            NoRecibeVisitaCausasTrasladoInternacional = _trasladoInternacional.CAUSA_NO_VISITAS;
                            CartaArraigoCuentaTrasladoInternacional = _trasladoInternacional.CARTA_ARRAIGO;
                            DomicilioTrasladoInternacional = _trasladoInternacional.DOMICILIO;
                            AnuenciaCupoTrasladoInternacional = _trasladoInternacional.ANUENCIA_CUPO;
                            FechaAnuenciaTrasladoInternacional = _trasladoInternacional.ANUENCIA_FEC;
                            NivelSocioETrasladoInternacional = _trasladoInternacional.NIVEL_SOCIOECONOMICO;
                            EstudiaActualTrasladoInternacional = _trasladoInternacional.ESTUDIA_ACTUALMENTE;
                            CausasNoEstudiaTrasladoInternacional = _trasladoInternacional.CAUSA_NO_ESTUDIA;
                            OtrosCursosCapacRecibTrasladoInternacional = _trasladoInternacional.OTROS_CURSOS;
                            AcualmenteTrabajaInstTrasladoInternacional = _trasladoInternacional.TRABAJA_ACTUALMENTE;
                            SenialeOcupacionTrasladoInternacional = _trasladoInternacional.OCUPACION_ACTUAL;
                            NegativoSenialeCausas = _trasladoInternacional.CAUSA_NO_TRABAJA;
                            TotalDiasEfectivTrasladoInternacional = _trasladoInternacional.DIAS_EFECTIVOS_TRABAJO;
                            ConductaConsidTrasladoInternacional = _trasladoInternacional.CONDUCTA_RECLUSION;

                            LstSancionesTrasladoInt.Clear();
                            if (_trasladoInternacional.TRASLADO_INTERNACIONAL_SANCION != null && _trasladoInternacional.TRASLADO_INTERNACIONAL_SANCION.Any())
                                foreach (var item in _trasladoInternacional.TRASLADO_INTERNACIONAL_SANCION)
                                    LstSancionesTrasladoInt.Add(item);

                            IdPeligroTrasladoInternacional = _trasladoInternacional.INDICE_PELIGROSIDAD;
                            AdiccionToxicosTrasladoInternacional = _trasladoInternacional.ADICCION_TOXICOS;
                            CasoAfirmativoCualesTrasladoInternacional = _trasladoInternacional.ADICCION_CUALES;
                            IsPsicoTrasladoInterNacionalChecked = _trasladoInternacional.CONTINUAR_PSICOLOGICO == "S" ? true : false;
                            IsEducTrasladoInterNacionalChecked = _trasladoInternacional.CONTINUAR_EDUCATIVO == "S" ? true : false;
                            IsLabTrasladoInterNacionalChecked = _trasladoInternacional.CONTINUAR_LABORAL == "S" ? true : false;
                            EspecifiqueOtroTratamientoTrasladoInterNacional = _trasladoInternacional.CONTINUAR_OTRO;
                            OtrosAspectosTrasladoInternacional = _trasladoInternacional.OTROS_ASPECTOS_OPINION;
                        }

                        ValidacionesTrasladoInternacionalPersonalidad();
                        LoadListasTraslados();
                        break;

                    case (short)eTipoSolicitudTraslado.NACIONAL:
                        var _trasladoNacional = new cTrasladoNacionalPersonalidad().GetData(c => c.ID_ESTUDIO == Entity.ID_ESTUDIO && c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_INGRESO == Entity.ID_INGRESO && c.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();
                        IdPeligrosidadTrasladoNacional = -1;
                        AdicToxTrasladoNacional = EspecifiqueToxicosTrasladoNacional = string.Empty;
                        IsPsicoTrasladoNacionalChecked = IsEducTrasladoNacionalChecked = IsLabTrasladoNacionalChecked = false;
                        EspecifiqueOtroTratamientoTrasladoNacional = EspecifiqueAspectosRelevantesTrasladoNacional = string.Empty;
                        FechaTrasladoNacional = null;

                        if (_trasladoNacional != null)
                        {
                            IdPeligrosidadTrasladoNacional = _trasladoNacional.ID_PELIGROSIDAD;
                            AdicToxTrasladoNacional = _trasladoNacional.ADICCION_TOXICOS;
                            EspecifiqueToxicosTrasladoNacional = _trasladoNacional.CUALES;
                            IsPsicoTrasladoNacionalChecked = _trasladoNacional.CONTINUAR_PSICOLOGICO == "S" ? true : false;
                            IsEducTrasladoNacionalChecked = _trasladoNacional.CONTINUAR_EDUCATIVO == "S" ? true : false;
                            IsLabTrasladoNacionalChecked = _trasladoNacional.CONTINUAR_LABORAL == "S" ? true : false;
                            EspecifiqueOtroTratamientoTrasladoNacional = _trasladoNacional.CONTINUAR_OTROS;
                            //IsOtrosTrasladoNacionalChecked = _trasladoNacional.CONTINUAR_OTROS == "S" ? true : false;
                            //EspecifiqueOtroTratamientoTrasladoNacional = _trasladoNacional.
                            EspecifiqueAspectosRelevantesTrasladoNacional = _trasladoNacional.OTROS_ASPECTOS_OPINION;
                            FechaTrasladoNacional = _trasladoNacional.ESTUDIO_FEC;
                        };

                        LoadListasTraslados();
                        ValidacionesTrasladoIslas();
                        break;

                    default:
                        //no action
                        break;
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        #endregion

        private bool GuardaTraslado(PERSONALIDAD Selected, short Opcion)
        {
            try
            {
                var _Centro = new cCentro().GetData(x => x.ID_CENTRO == (short)GlobalVar.gCentro).FirstOrDefault();
                string _dir = _Centro != null ? !string.IsNullOrEmpty(_Centro.DIRECTOR) ? _Centro.DIRECTOR.Trim() : string.Empty : string.Empty;
                string NombreUsuario = string.Empty;
                string NombreJefeJuridico = string.Empty;
                USUARIO _usuario = new cUsuario().Obtener(GlobalVar.gUsr);
                if (_usuario != null)
                {
                    var _persona = new cPersona().GetData(c => c.ID_PERSONA == _usuario.ID_PERSONA).FirstOrDefault();
                    if (_persona != null)
                        NombreUsuario = string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(_persona.NOMBRE) ? _persona.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_persona.PATERNO) ? _persona.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_persona.MATERNO) ? _persona.MATERNO.Trim() : string.Empty);
                };

                if (Opcion != decimal.Zero)
                {
                    switch (Opcion)
                    {
                        case (short)eTipoSolicitudTraslado.ISLAS:
                            var _trasldoIsla = new TRASLADO_ISLAS()
                            {
                                AGRESIVIDAD_INTRAMUROS = IdAgresivIntrTrasladoIslas,
                                ANUENCIA_FIRMADA = TieneAnuenciaTrasladoIslas,
                                CONTINUE_TRATAMIENTO = ContinTratamTrasladoIslas,
                                CRIMINOGENESIS = CriminogTrasladoIslas,
                                DICTAMEN_TRASLADO = DictamenTrasladoIslas,
                                EGOCENTRISMO = IdEgocentrismoTrasladoIslas,
                                EN_CASO_ADICCION = EstadoAdiccionTrasladoIslas,
                                ID_ANIO = Selected.ID_ANIO,
                                ID_IMPUTADO = Selected.ID_IMPUTADO,
                                ID_INGRESO = Selected.ID_INGRESO,
                                ID_PELIGROSIDAD = IdPeligroTrasladoIslas,
                                INTIMIDACION_PENA = IntimidacionPenaTrasladoIslas,
                                LABILIDAD_AFECTIVA = IdLabAfecTrasladoIslas,
                                ID_ESTUDIO = Selected.ID_ESTUDIO,
                                ESTUDIO_FEC = FechaTrasladoIslas,
                                ID_CENTRO = Selected.ID_CENTRO,
                                TRATAMIENTO_SUGERIDO = TratamSugTrasladoIslas,
                                DIRECTOR = _dir,
                                RESPONSABLE = NombreUsuario
                            };

                            if (new cTrasladoIslasPersonalidad().InsertarTrasladoIslas(_trasldoIsla))
                            {
                                new Dialogos().ConfirmacionDialogo("ÉXITO", "Se ha guardado el traslado con éxito");
                                return true;
                            }
                            else
                            {
                                new Dialogos().ConfirmacionDialogo("ERROR", "Surgió un error al ingresar el traslado a islas");
                                return false;
                            }

                            break;

                        case (short)eTipoSolicitudTraslado.NACIONAL:
                            string _NombreCoordinadorCrimin = string.Empty;
                            var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_CRIMINODIAGNOSTICO).FirstOrDefault();
                            if (_UsuarioCoordinador != null)
                                _NombreCoordinadorCrimin = _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                                    !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty;


                            var _nacional = new TRASLADO_NACIONAL()
                            {
                                ADICCION_TOXICOS = AdicToxTrasladoNacional,
                                CONTINUAR_EDUCATIVO = IsEducTrasladoNacionalChecked == true ? "S" : "N",
                                CONTINUAR_LABORAL = IsLabTrasladoNacionalChecked == true ? "S" : "N",
                                CONTINUAR_OTROS = EspecifiqueOtroTratamientoTrasladoNacional,
                                CONTINUAR_PSICOLOGICO = IsPsicoTrasladoNacionalChecked == true ? "S" : "N",
                                CUALES = EspecifiqueToxicosTrasladoNacional,
                                ESTUDIO_FEC = FechaTrasladoNacional,
                                ID_ANIO = Selected.ID_ANIO,
                                ID_CENTRO = Selected.ID_CENTRO,
                                ID_ESTUDIO = Selected.ID_ESTUDIO,
                                ID_IMPUTADO = Selected.ID_IMPUTADO,
                                ID_INGRESO = Selected.ID_INGRESO,
                                ID_PELIGROSIDAD = IdPeligrosidadTrasladoNacional,
                                DIRECTOR_CERESO = _dir,
                                ELABORO = NombreUsuario,
                                COORDINADOR_CRIMINOLOGIA = _NombreCoordinadorCrimin,
                                OTROS_ASPECTOS_OPINION = EspecifiqueAspectosRelevantesTrasladoNacional
                            };

                            if (new cTrasladoNacionalPersonalidad().InsertarTrasladoNacional(_nacional))
                            {
                                new Dialogos().ConfirmacionDialogo("ÉXITO", "Se ha guardado el traslado con éxito");
                                return true;
                            }
                            else
                            {
                                new Dialogos().ConfirmacionDialogo("ERROR", "Surgió un error al ingresar el traslado nacional");
                                return false;
                            }
                            break;

                        case (short)eTipoSolicitudTraslado.INTERNACIONAL:
                            var _internacional = new TRASLADO_INTERNACIONAL()
                            {
                                ADICCION_CUALES = CasoAfirmativoCualesTrasladoInternacional,
                                ADICCION_TOXICOS = AdiccionToxicosTrasladoInternacional,
                                AGRESIVIDAD = DescAgresividad,
                                ANUENCIA_CUPO = AnuenciaCupoTrasladoInternacional,
                                ANUENCIA_FEC = FechaAnuenciaTrasladoInternacional,
                                APOYO_PADRES = ApoyoPadresTrasladoInternacional,
                                CARTA_ARRAIGO = CartaArraigoCuentaTrasladoInternacional,
                                CAUSA_NO_ESTUDIA = CausasNoEstudiaTrasladoInternacional,
                                CAUSA_NO_TRABAJA = NegativoSenialeCausas,
                                CAUSA_NO_VISITAS = NoRecibeVisitaCausasTrasladoInternacional,
                                CLINICAMENTE_SANO = ClinicSanoTrasladoInternacional,
                                COEFICIENTE_INTELECTUAL = CoeficIntTrasladoInternacional,
                                CONDUCTA_RECLUSION = ConductaConsidTrasladoInternacional,
                                CONTINUAR_EDUCATIVO = IsEducTrasladoInterNacionalChecked == true ? "S" : "N",
                                CONTINUAR_LABORAL = IsLabTrasladoInterNacionalChecked == true ? "S" : "N",
                                CONTINUAR_OTRO = EspecifiqueOtroTratamientoTrasladoInterNacional,
                                CONTINUAR_PSICOLOGICO = IsPsicoTrasladoInterNacionalChecked == true ? "S" : "N",
                                CONYUGE = ConyugeTrasladoInternacional,
                                DANIO_CEREBRAL = DanioCerebTrasladoInternacional,
                                DESCONOCE = string.Empty,
                                DIAS_EFECTIVOS_TRABAJO = TotalDiasEfectivTrasladoInternacional,
                                DOMICILIO = DomicilioTrasladoInternacional,
                                ESTUDIA_ACTUALMENTE = EstudiaActualTrasladoInternacional,
                                ESTUDIO_FEC = Fechas.GetFechaDateServerString,
                                ESCOLARIDAD = EscolaridadTrasladoInternac,
                                FRECUENCIA_VISITAS = FrecuenciaVisitaTrasladoInternacional,
                                ID_ANIO = Selected.ID_ANIO,
                                ID_IMPUTADO = Selected.ID_IMPUTADO,
                                ID_CENTRO = Selected.ID_CENTRO,
                                ID_ESTUDIO = Selected.ID_ESTUDIO,
                                ID_INGRESO = Selected.ID_INGRESO,
                                INDICE_PELIGROSIDAD = IdPeligroTrasladoInternacional,
                                NIVEL_SOCIOECONOMICO = NivelSocioETrasladoInternacional,
                                OCUPACION_ACTUAL = SenialeOcupacionTrasladoInternacional,
                                OCUPACION_PREVIA = OcupacionPreviaTrasladoInternac,
                                OTROS_ASPECTOS = EspecifiqueOtroTratamientoTrasladoInterNacional,
                                OTROS_ASPECTOS_OPINION = OtrosAspectosTrasladoInternacional,
                                PADECIMIENTO = IndicaPadecimientoTrasladoInternacional,
                                TRABAJA_ACTUALMENTE = AcualmenteTrabajaInstTrasladoInternacional,
                                TRATAMIENTO_ACTUAL = TratamientoActualTrasladoInternacional,
                                DIRECTOR = _dir,
                                RESPONSABLE = NombreUsuario,
                                INSTITUCION = _Centro != null ? !string.IsNullOrEmpty(_Centro.DESCR) ? _Centro.DESCR.Trim() : string.Empty : string.Empty,
                                VERSION_DELITO = VersionDelitoTrasladoInternacional,
                                OTROS_CURSOS = OtrosCursosCapacRecibTrasladoInternacional
                            };

                            if (LstSancionesTrasladoInt != null && LstSancionesTrasladoInt.Any())
                                foreach (var item in LstSancionesTrasladoInt)
                                    _internacional.TRASLADO_INTERNACIONAL_SANCION.Add(item);

                            if (LstVisitasTraslInt != null && LstVisitasTraslInt.Any())
                                foreach (var item in LstVisitasTraslInt)
                                    _internacional.TRASLADO_INTERNACIONAL_VISITA.Add(item);

                            if (new cTrasladoInternacionalPersonalidad().InsertarTrasladoInternacional(_internacional))
                            {
                                new Dialogos().ConfirmacionDialogo("ÉXITO", "Se ha guardado el traslado con éxito");
                                return true;
                            }
                            else
                            {
                                new Dialogos().ConfirmacionDialogo("ERROR", "Surgió un error al ingresar el traslado internacional");
                                return false;
                            }
                            break;

                        default:
                            break;
                    }
                }
                return false;

            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void ImprimeTrasladoNacional(PERSONALIDAD Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                View.Show();

                var _centro = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                cTrasladoNacionalPersonalidadRep DatosReporte = new cTrasladoNacionalPersonalidadRep();
                var _trasladoHecho = new cTrasladoNacionalPersonalidad().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();
                if (_trasladoHecho != null)
                    DatosReporte = new cTrasladoNacionalPersonalidadRep()
                    {
                        Generico1 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_PSICOLOGICO) ? _trasladoHecho.CONTINUAR_PSICOLOGICO == "S" ? "X" : string.Empty : string.Empty,
                        Generico2 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_PSICOLOGICO) ? _trasladoHecho.CONTINUAR_PSICOLOGICO == "N" ? "X" : string.Empty : string.Empty,
                        Generico3 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_EDUCATIVO) ? _trasladoHecho.CONTINUAR_EDUCATIVO == "S" ? "X" : string.Empty : string.Empty,
                        Generico4 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_EDUCATIVO) ? _trasladoHecho.CONTINUAR_EDUCATIVO == "N" ? "X" : string.Empty : string.Empty,
                        Generico5 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_LABORAL) ? _trasladoHecho.CONTINUAR_LABORAL == "S" ? "X" : string.Empty : string.Empty,
                        Generico6 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_LABORAL) ? _trasladoHecho.CONTINUAR_LABORAL == "N" ? "X" : string.Empty : string.Empty,

                        IndicePeligrosidad = string.Format("MÍNIMA ( {0} )  MÍNIMA MEDIA ( {1} )  MEDIA ( {2} ) SUPERIOR A LA MEDIA ( {3} )  MÁXIMA ( {4} )",
                        _trasladoHecho.ID_PELIGROSIDAD.HasValue ? _trasladoHecho.ID_PELIGROSIDAD.Value == 5 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD.HasValue ? _trasladoHecho.ID_PELIGROSIDAD.Value == 4 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD.HasValue ? _trasladoHecho.ID_PELIGROSIDAD.Value == 3 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD.HasValue ? _trasladoHecho.ID_PELIGROSIDAD.Value == 2 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD.HasValue ? _trasladoHecho.ID_PELIGROSIDAD.Value == 1 ? "X" : string.Empty : string.Empty
                        ),
                        ContinuaTratamientoEduc = _trasladoHecho.CONTINUAR_EDUCATIVO == "S" ? "X" : string.Empty,
                        ContinuaTratamientoLab = _trasladoHecho.CONTINUAR_LABORAL == "S" ? "X" : string.Empty,
                        ContinuaTratamientoPsico = _trasladoHecho.CONTINUAR_PSICOLOGICO == "S" ? "X" : string.Empty,
                        CoordCrimi = _trasladoHecho.COORDINADOR_CRIMINOLOGIA,
                        Elaboro = _trasladoHecho.ELABORO,
                        FechaElaboracion = _trasladoHecho.ESTUDIO_FEC.HasValue ? _trasladoHecho.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                        OtrosAspectosRelevantes = _trasladoHecho.OTROS_ASPECTOS_OPINION,
                        CualesToxicos = _trasladoHecho.CUALES,
                        NombreDirector = string.Format("DIRECTOR DEL CERESO \" {0} \" \n {1}", _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty, _trasladoHecho.DIRECTOR_CERESO),
                        OtrosContinuar = _trasladoHecho.CONTINUAR_OTROS == "S" ? "X" : string.Empty,
                        PresentaAdiccionToxicos = string.Format("SI ( {0} ) \t NO ( {1} )", _trasladoHecho.ADICCION_TOXICOS == "S" ? "X" : string.Empty, _trasladoHecho.ADICCION_TOXICOS == "N" ? "X" : string.Empty),
                        Generico7 = string.Empty,
                        Generico8 = _trasladoHecho.CONTINUAR_OTROS
                    };

                var ds1 = new List<cTrasladoNacionalPersonalidadRep>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(DatosReporte);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                View.Report.LocalReport.ReportPath = "Reportes/rTrasladoNacional.rdlc";
                View.Report.RefreshReport();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void ImprimeTrasladoIslas(PERSONALIDAD Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                View.Show();

                cTrasladoIslasPerson DatosReporte = new cTrasladoIslasPerson();
                var _trasladoHecho = new cTrasladoIslasPersonalidad().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();
                if (_trasladoHecho != null)
                    DatosReporte = new cTrasladoIslasPerson()
                    {
                        Crimino = _trasladoHecho.CRIMINOGENESIS,
                        Egocentrismo = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _trasladoHecho.EGOCENTRISMO == 1 ? "X" : string.Empty, _trasladoHecho.EGOCENTRISMO == 2 ? "X" : string.Empty, _trasladoHecho.EGOCENTRISMO == 4 ? "X" : string.Empty),
                        LabAfec = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _trasladoHecho.LABILIDAD_AFECTIVA == 1 ? "X" : string.Empty, _trasladoHecho.LABILIDAD_AFECTIVA == 2 ? "X" : string.Empty, _trasladoHecho.LABILIDAD_AFECTIVA == 4 ? "X" : string.Empty),
                        AdiccionNarco = string.Format("REMISIÓN ( {0} )  EXTINGUIDA ( {1} )  EN ENTORNO CONTROLADA ( {2} )",
                        _trasladoHecho.EN_CASO_ADICCION == "R" ? "X" : string.Empty,
                        _trasladoHecho.EN_CASO_ADICCION == "E" ? "X" : string.Empty,
                        _trasladoHecho.EN_CASO_ADICCION == "C" ? "X" : string.Empty),
                        IntimidacionPena = string.Format("SI ( {0} )   NO ( {1} )", _trasladoHecho.INTIMIDACION_PENA == "S" ? "X" : string.Empty, _trasladoHecho.INTIMIDACION_PENA == "N" ? "X" : string.Empty),
                        AgresividadIntram = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _trasladoHecho.AGRESIVIDAD_INTRAMUROS == 1 ? "X" : string.Empty, _trasladoHecho.AGRESIVIDAD_INTRAMUROS == 2 ? "X" : string.Empty, _trasladoHecho.AGRESIVIDAD_INTRAMUROS == 4 ? "X" : string.Empty),
                        IndicePeligrosidadCrimiActual = string.Format("MÍNIMA ( {0} )  MÍNIMA-MEDIA ( {1} )  MEDIA ( {2} )  MEDIA-MÁXIMA ( {3} )  MÁXIMA ( {4} )",
                        _trasladoHecho.ID_PELIGROSIDAD == 5 ? "X" : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD == 4 ? "X" : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD == 3 ? "X" : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD == 2 ? "X" : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD == 1 ? "X" : string.Empty),
                        Continue = string.Format(" ( {0} )", _trasladoHecho.CONTINUE_TRATAMIENTO == "S" ? "X" : string.Empty),
                        AnuenciaFirmada = string.Format("SI ( {0} )   NO ( {1} )", _trasladoHecho.ANUENCIA_FIRMADA == "S" ? "X" : string.Empty, _trasladoHecho.ANUENCIA_FIRMADA == "N" ? "X" : string.Empty),
                        DictamenActual = string.Format("FAVORABLE ( {0} )  DESFAVORABLE ( {1} )", _trasladoHecho.DICTAMEN_TRASLADO == "F" ? "X" : string.Empty, _trasladoHecho.DICTAMEN_TRASLADO == "D" ? "X" : string.Empty),
                        LugarFc = _trasladoHecho.ESTUDIO_FEC.HasValue ? string.Format("A {0}", Fechas.fechaLetra(_trasladoHecho.ESTUDIO_FEC.Value, false).ToUpper()) : string.Empty,
                        DirectorCentro = _trasladoHecho.DIRECTOR,
                        Responsable = _trasladoHecho.RESPONSABLE,
                        TratamExtram = _trasladoHecho.TRATAMIENTO_SUGERIDO
                    };


                var ds1 = new List<cTrasladoIslasPerson>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(DatosReporte);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                View.Report.LocalReport.ReportPath = "Reportes/rTrasladoIslas.rdlc";
                View.Report.RefreshReport();
            }

            catch (Exception)
            {
                throw;
            }
        }

        private void ImprimeTrasladoInternacional(PERSONALIDAD Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                View.Show();

                string _Origi = string.Empty;
                string _resiPadres = string.Empty;
                if (Entity.INGRESO.IMPUTADO != null)
                {
                    var _EstadoNac = new cEntidad().GetData(x => x.ID_ENTIDAD == Entity.INGRESO.IMPUTADO.NACIMIENTO_ESTADO).FirstOrDefault();
                    var _MunicipioNac = new cMunicipio().GetData(x => x.ID_MUNICIPIO == Entity.INGRESO.IMPUTADO.NACIMIENTO_MUNICIPIO && x.ID_ENTIDAD == _EstadoNac.ID_ENTIDAD).FirstOrDefault();
                    _Origi = string.Format("{0}, {1}", _EstadoNac != null ? !string.IsNullOrEmpty(_EstadoNac.DESCR) ? _EstadoNac.DESCR.Trim() : string.Empty : string.Empty, _MunicipioNac != null ? !string.IsNullOrEmpty(_MunicipioNac.MUNICIPIO1) ? _MunicipioNac.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                    if (Entity.INGRESO.IMPUTADO.IMPUTADO_PADRES.Any())
                        foreach (var item in Entity.INGRESO.IMPUTADO.IMPUTADO_PADRES)
                            _resiPadres = string.Format("{0} {1} ", !string.IsNullOrEmpty(item.CALLE) ? item.CALLE.Trim() : string.Empty, item.NUM_EXT.HasValue ? item.NUM_EXT.Value.ToString() : string.Empty);
                };

                System.Collections.Generic.List<cDetallesJuridicosReporte> ListaDelitos = new List<cDetallesJuridicosReporte>();
                System.Collections.Generic.List<cDetallesJuridicosReporte> ListaAntecedentesPenales = new List<cDetallesJuridicosReporte>();

                var _causasPenales = Entity.INGRESO != null ? Entity.INGRESO.CAUSA_PENAL : null;
                if (_causasPenales != null && _causasPenales.Any())
                    foreach (var item in _causasPenales)
                        foreach (var item2 in item.SENTENCIA)
                            foreach (var item3 in item2.SENTENCIA_DELITO)
                                ListaDelitos.Add(new cDetallesJuridicosReporte
                                {
                                    Delito = item3.DESCR_DELITO,
                                    Apartir = item2.FEC_INICIO_COMPURGACION.HasValue ? item2.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty,
                                    FueroC = item3.ID_FUERO == "C" ? "X" : string.Empty,
                                    FueroF = item3.ID_FUERO == "F" ? "X" : string.Empty,
                                    Pena = string.Format("{0} , {1} , {2} ",
                                    item2.ANIOS.HasValue ? item2.ANIOS.Value > 0 ? string.Concat(item2.ANIOS.Value + " AÑOS") : "0 AÑOS" : "0 AÑOS",
                                    item2.MESES.HasValue ? item2.MESES.Value > 0 ? string.Concat(item2.MESES.Value + " MESES") : "0 MESES" : "0 MESES",
                                    item2.DIAS.HasValue ? item2.DIAS.Value > 0 ? string.Concat(item2.DIAS.Value + " DÍAS") : "0 DÍAS" : "0 DÍAS"),
                                    Proceso = string.Format("{0} / {1} ", item.CP_ANIO.HasValue ? item.CP_ANIO.Value.ToString() : string.Empty, item.CP_FOLIO.HasValue ? item.CP_FOLIO.Value.ToString() : string.Empty)
                                });

                var Imputado = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO : null;
                if (Imputado != null)
                    if (Imputado.INGRESO != null && Imputado.INGRESO.Any())
                        foreach (var item in Imputado.INGRESO)
                            foreach (var item2 in item.CAUSA_PENAL)
                                foreach (var item3 in item2.SENTENCIA)
                                    foreach (var item4 in item3.SENTENCIA_DELITO)
                                        ListaAntecedentesPenales.Add(new cDetallesJuridicosReporte
                                        {
                                            Delito = !string.IsNullOrEmpty(item4.DESCR_DELITO) ? item4.DESCR_DELITO.Trim() : string.Empty,
                                            Apartir = item4.SENTENCIA != null ? item4.SENTENCIA.FEC_INICIO_COMPURGACION.HasValue ? item4.SENTENCIA.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty,
                                            FueroC = item4.ID_FUERO == "C" ? "X" : string.Empty,
                                            FueroF = item4.ID_FUERO == "F" ? "X" : string.Empty,
                                            Pena = string.Format("{0} , {1} , {2} ",
                                                                    item3.ANIOS.HasValue ? item3.ANIOS.Value > 0 ? string.Concat(item3.ANIOS.Value + " AÑOS") : "0 AÑOS" : "0 AÑOS",
                                                                    item3.MESES.HasValue ? item3.MESES.Value > 0 ? string.Concat(item3.MESES.Value + " MESES") : "0 MESES" : "0 MESES",
                                                                    item3.DIAS.HasValue ? item3.DIAS.Value > 0 ? string.Concat(item3.DIAS.Value + " DÍAS") : "0 DÍAS" : "0 DÍAS"),
                                            Proceso = string.Format("{0} / {1} ", item2.CP_ANIO.HasValue ? item2.CP_ANIO.Value.ToString() : string.Empty, item2.CP_FOLIO.HasValue ? item2.CP_FOLIO.Value.ToString() : string.Empty)
                                        });

                string Antece = "N";
                if (LstDelitosDos != null && LstDelitosDos.Any())
                    Antece = "S";

                var _Centro = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                cTrasladoInterPersonalidad DatosReporte = new cTrasladoInterPersonalidad();
                List<cTrasladoIntSancionReporte> lstSanciones = new List<cTrasladoIntSancionReporte>();
                List<cTrasladoIntVisitaReporte> lstVisitas = new List<cTrasladoIntVisitaReporte>();

                var _trasladoHecho = new cTrasladoInternacionalPersonalidad().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_trasladoHecho != null)
                {
                    DatosReporte = new cTrasladoInterPersonalidad()
                    {
                        CentroOrigen = _Centro != null ? !string.IsNullOrEmpty(_Centro.DESCR) ? _Centro.DESCR.Trim() : string.Empty : string.Empty,
                        CentroD = string.Empty,
                        NombreImp = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.NOMBRE) ? Entity.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                         !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.PATERNO) ? Entity.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                         !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.MATERNO) ? Entity.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty,
                        EdadImp = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO != null ? new Fechas().CalculaEdad(Entity.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty : string.Empty,
                        //EdoCivilImp = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR) ? Entity.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                        EdoCivilImp = Entity.INGRESO != null ? Entity.INGRESO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(Entity.INGRESO.ESTADO_CIVIL.DESCR) ? Entity.INGRESO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                        OriginaImp = _Origi,
                        LugarResid = _resiPadres,
                        EscolaridadImp = _trasladoHecho.ESCOLARIDAD,
                        OcupacionPreviaImp = _trasladoHecho.OCUPACION_PREVIA,
                        AntecPenales = string.Format("NO ( {0} ) \t SI ( {1} )", string.Empty, "X"),
                        VersionDel = _trasladoHecho.VERSION_DELITO,
                        ClinicSano = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.CLINICAMENTE_SANO == "S" ? "X" : string.Empty, _trasladoHecho.CLINICAMENTE_SANO == "N" ? "X" : string.Empty),
                        CasoNegEsp = _trasladoHecho.PADECIMIENTO,
                        TratamActual = _trasladoHecho.TRATAMIENTO_ACTUAL,
                        CoeficienteInt = _trasladoHecho.COEFICIENTE_INTELECTUAL,
                        DanioOrganoCere = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.DANIO_CEREBRAL == "S" ? "X" : string.Empty, _trasladoHecho.DANIO_CEREBRAL == "N" ? "X" : string.Empty),
                        OtrosAspectos = _trasladoHecho.OTROS_ASPECTOS_OPINION,
                        ApoyoPadres = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.APOYO_PADRES == "S" ? "X" : string.Empty, _trasladoHecho.APOYO_PADRES == "N" ? "X" : string.Empty),
                        Conyuge = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.CONYUGE == "S" ? "X" : string.Empty, _trasladoHecho.CONYUGE == "N" ? "X" : string.Empty),
                        Frecuenciavisitas = _trasladoHecho.FRECUENCIA_VISITAS,
                        NoRecibVisitas = _trasladoHecho.CAUSA_NO_VISITAS,
                        CartaArraigoDom = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.CARTA_ARRAIGO == "S" ? "X" : string.Empty, _trasladoHecho.CARTA_ARRAIGO == "N" ? "X" : string.Empty),
                        Domic = _trasladoHecho.DOMICILIO,
                        AnuenciaCupo = string.Format("SE DESCONOCE ( {0} )   SI( {1} )   NO( {2} )",
                        _trasladoHecho.ANUENCIA_CUPO == "D" ? "X" : string.Empty,
                        _trasladoHecho.ANUENCIA_CUPO == "S" ? "X" : string.Empty,
                        _trasladoHecho.ANUENCIA_CUPO == "N" ? "X" : string.Empty),
                        Fecha1 = _trasladoHecho.ANUENCIA_FEC.HasValue ? _trasladoHecho.ANUENCIA_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                        NivelSocioE = _trasladoHecho.NIVEL_SOCIOECONOMICO,
                        EstudiaActu = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.ESTUDIA_ACTUALMENTE == "S" ? "X" : string.Empty, _trasladoHecho.ESTUDIA_ACTUALMENTE == "N" ? "X" : string.Empty),
                        CasoNoEstudia = _trasladoHecho.CAUSA_NO_ESTUDIA,
                        TrabajaInst = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.TRABAJA_ACTUALMENTE == "S" ? "X" : string.Empty, _trasladoHecho.TRABAJA_ACTUALMENTE == "N" ? "X" : string.Empty),
                        Ocupaciones = _trasladoHecho.OCUPACION_ACTUAL,
                        NegativoTrabaja = _trasladoHecho.CAUSA_NO_TRABAJA,
                        DiasEfectLab = _trasladoHecho.DIAS_EFECTIVOS_TRABAJO.HasValue ? _trasladoHecho.DIAS_EFECTIVOS_TRABAJO.Value.ToString() : string.Empty,
                        ConductaDentroReclusion = string.Format("MÍNIMA ( {0} )  MÍNIMA MEDIA ( {1} )  MEDIA ( {2} )  SUPERIOR A LA MEDIA ( {3} )  MÁXIMA ( {4} ) ",
                        _trasladoHecho.CONDUCTA_RECLUSION.HasValue ? _trasladoHecho.CONDUCTA_RECLUSION == 5 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.CONDUCTA_RECLUSION.HasValue ? _trasladoHecho.CONDUCTA_RECLUSION == 4 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.CONDUCTA_RECLUSION.HasValue ? _trasladoHecho.CONDUCTA_RECLUSION == 3 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.CONDUCTA_RECLUSION.HasValue ? _trasladoHecho.CONDUCTA_RECLUSION == 2 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.CONDUCTA_RECLUSION.HasValue ? _trasladoHecho.CONDUCTA_RECLUSION == 1 ? "X" : string.Empty : string.Empty),
                        EsAdictoToxicos = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.ADICCION_TOXICOS == "S" ? "X" : string.Empty, _trasladoHecho.ADICCION_TOXICOS == "N" ? "X" : string.Empty),
                        AdictoEspecifica = _trasladoHecho.ADICCION_CUALES,
                        ContinuaTratamPsico = _trasladoHecho.CONTINUAR_PSICOLOGICO == "S" ? "X" : string.Empty,
                        ContinuaTratamEduc = _trasladoHecho.CONTINUAR_EDUCATIVO == "S" ? "X" : string.Empty,
                        ContinuaTratamLaboral = _trasladoHecho.CONTINUAR_LABORAL == "S" ? "X" : string.Empty,
                        OtrosAspectosP = _trasladoHecho.OTROS_ASPECTOS,
                        NombreDirector = _trasladoHecho.DIRECTOR,
                        Responsable = _trasladoHecho.RESPONSABLE,
                        FecElaboracion = _trasladoHecho.ESTUDIO_FEC,
                        Generico = _trasladoHecho.AGRESIVIDAD,
                        Generico2 = _trasladoHecho.CONTINUAR_PSICOLOGICO == "N" ? "X" : string.Empty,
                        Generico3 = _trasladoHecho.CONTINUAR_EDUCATIVO == "N" ? "X" : string.Empty,
                        Generico4 = _trasladoHecho.CONTINUAR_LABORAL == "N" ? "X" : string.Empty,
                        Generico5 = string.Format("SI ({0})   NO ({1})", Antece == "S" ? "X" : string.Empty, Antece == "N" ? "X" : string.Empty),
                        Generico6 = _Centro != null ? !string.IsNullOrEmpty(_Centro.DESCR) ? _Centro.DESCR.Trim() : string.Empty : string.Empty,
                        Cursos = _trasladoHecho.OTROS_CURSOS,
                        Desconoc = _trasladoHecho.CONTINUAR_OTRO
                    };

                    if (_trasladoHecho.TRASLADO_INTERNACIONAL_SANCION != null && _trasladoHecho.TRASLADO_INTERNACIONAL_SANCION.Any())
                        foreach (var item in _trasladoHecho.TRASLADO_INTERNACIONAL_SANCION)
                        {
                            lstSanciones.Add(new cTrasladoIntSancionReporte()
                            {
                                Motivo = item.MOTIVO,
                                Sancion = item.SANCION,
                                SancionFecha = item.SANCION_FEC.HasValue ? item.SANCION_FEC.Value.ToString("dd/MM/yyyy") : string.Empty
                            });
                        };

                    if (_trasladoHecho.TRASLADO_INTERNACIONAL_VISITA != null && _trasladoHecho.TRASLADO_INTERNACIONAL_VISITA.Any())
                        foreach (var item in _trasladoHecho.TRASLADO_INTERNACIONAL_VISITA)
                        {
                            lstVisitas.Add(new cTrasladoIntVisitaReporte
                                {
                                    Materno = item.MATERNO,
                                    Nombre = item.NOMBRE,
                                    Parentesco = item.ID_TIPO_REFERENCIA.HasValue ? !string.IsNullOrEmpty(item.TIPO_REFERENCIA.DESCR) ? item.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                    Paterno = item.PATERNO
                                });
                        };
                };


                cEncabezado Encabezado = new cEncabezado();
                Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.NombreReporte = !string.IsNullOrEmpty(_Centro.DESCR) ? _Centro.DESCR.Trim() : string.Empty;
                Encabezado.ImagenDerecha = Parametro.LOGO_BC_ACTA_COMUN;

                var ds1 = new List<cEncabezado>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(Encabezado);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);


                var ds2 = new List<cTrasladoInterPersonalidad>();
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds2.Add(DatosReporte);
                rds2.Name = "DataSet2";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                var ds3 = new List<cTrasladoIntSancionReporte>();
                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds3 = lstSanciones;
                rds3.Name = "DataSet3";
                rds3.Value = ds3;
                View.Report.LocalReport.DataSources.Add(rds3);

                var ds4 = new List<cTrasladoIntVisitaReporte>();
                Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds4 = lstVisitas;
                rds4.Name = "DataSet4";
                rds4.Value = ds4;
                View.Report.LocalReport.DataSources.Add(rds4);


                Microsoft.Reporting.WinForms.ReportDataSource rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = ListaDelitos;
                View.Report.LocalReport.DataSources.Add(rds5);


                Microsoft.Reporting.WinForms.ReportDataSource rds6 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds6.Name = "DataSet6";
                rds6.Value = ListaAntecedentesPenales;
                View.Report.LocalReport.DataSources.Add(rds6);


                View.Report.LocalReport.ReportPath = "Reportes/rTrasladoInternacional.rdlc";
                View.Report.RefreshReport();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void BuscarEstudiosParaDictamen()
        {
            try
            {
                LstEstudiosTerminados = new ObservableCollection<PERSONALIDAD>();
                SelectedEstudioTerminado = null;

                DateTime f1, f2;
                if (FechaInicioBusquedaDictamenFinal.HasValue)
                    f1 = FechaInicioBusquedaDictamenFinal.Value;
                else
                    f1 = Fechas.GetFechaDateServer;

                if (FechaFinBusquedaDictamenFinal.HasValue)
                    f2 = FechaFinBusquedaDictamenFinal.Value;
                else
                    f2 = Fechas.GetFechaDateServer;

                f1 = new DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new DateTime(f2.Year, f2.Month, f2.Day);

                var _dato = new cEstudioPersonalidad().BuscarEstudiosDictamenFinal(f1, f2, NoOficioBusquedaDictamenFinal, GlobalVar.gCentro);
                if (_dato != null && _dato.Any())
                    foreach (var item in _dato)
                        LstEstudiosTerminados.Add(item);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscarEstudiosDictamenFinal":
                    BuscarEstudiosParaDictamen();
                    break;

                case "guardar_traslado_sol":
                    switch (_TrasladoActua)
                    {
                        case (short)eTipoSolicitudTraslado.ISLAS:
                            if (base.HasErrors)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                                }));
                                return;
                            }

                            GuardaTraslado(SelectedEstudioTerminado, (short)eTipoSolicitudTraslado.ISLAS);
                            break;

                        case (short)eTipoSolicitudTraslado.NACIONAL:
                            if (base.HasErrors)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                                }));
                                return;
                            }

                            GuardaTraslado(SelectedEstudioTerminado, (short)eTipoSolicitudTraslado.NACIONAL);
                            break;

                        case (short)eTipoSolicitudTraslado.INTERNACIONAL:
                            if (base.HasErrors)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                                }));
                                return;
                            }
                            GuardaTraslado(SelectedEstudioTerminado, (short)eTipoSolicitudTraslado.INTERNACIONAL);
                            break;

                        default:
                            break;
                    }
                    break;

                case "reporte_traslado_sol":
                    switch (_TrasladoActua)
                    {
                        case (short)eTipoSolicitudTraslado.ISLAS:
                            ImprimeTrasladoIslas(SelectedEstudioTerminado);
                            break;

                        case (short)eTipoSolicitudTraslado.NACIONAL:
                            ImprimeTrasladoNacional(SelectedEstudioTerminado);
                            break;

                        case (short)eTipoSolicitudTraslado.INTERNACIONAL:
                            ImprimeTrasladoInternacional(SelectedEstudioTerminado);
                            break;

                        default:
                            break;
                    }
                    break;

                case "regresar_traslado_sol":
                    _TrasladoActua = 0;
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudiosTerminadosView();
                    StaticSourcesViewModel.SourceChanged = false;
                    break;

                case "redactar_solicitud_traslado":
                    if (SelectedEstudioTerminado != null)
                    {
                        if (SelectedEstudioTerminado.ID_MOTIVO.HasValue)
                        {
                            switch (SelectedEstudioTerminado.ID_MOTIVO)
                            {
                                case (short)eTipoSolicitudTraslado.ISLAS:
                                    _TrasladoActua = (short)eTipoSolicitudTraslado.ISLAS;
                                    ProcesaTraslado(SelectedEstudioTerminado, (short)eTipoSolicitudTraslado.ISLAS);
                                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new TrasladoIslasPersonalidadView();

                                    break;

                                case (short)eTipoSolicitudTraslado.NACIONAL:
                                    _TrasladoActua = (short)eTipoSolicitudTraslado.NACIONAL;
                                    ProcesaTraslado(SelectedEstudioTerminado, (short)eTipoSolicitudTraslado.NACIONAL);
                                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new TrasladoNacionalPersonalidadView();

                                    break;

                                case (short)eTipoSolicitudTraslado.INTERNACIONAL:
                                    _TrasladoActua = (short)eTipoSolicitudTraslado.INTERNACIONAL;
                                    ProcesaTraslado(SelectedEstudioTerminado, (short)eTipoSolicitudTraslado.INTERNACIONAL);
                                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new TrasladoInternacionalPersonalidadView();
                                    break;

                                default:
                                    new Dialogos().ConfirmacionDialogo("Validación!", "Verifique el motivo del estudio de personalidad seleccionado.");
                                    break;
                            };
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación!", "Verifique el motivo del estudio de personalidad seleccionado.");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación!", "Seleccione un estudio de personalidad.");

                    break;

                case "buscar_menu":
                    if (ModuloActual == (short)eModuloActual.DICTAMEN_FINAL)
                        return;

                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "limpiar_menu":
                    switch (ModuloActual)
                    {
                        case (short)eModuloActual.DICTAMEN_FINAL:
                            NoOficioBusquedaDictamenFinal = string.Empty;
                            FechaInicioBusquedaDictamenFinal = FechaFinBusquedaDictamenFinal = Fechas.GetFechaDateServer;
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudiosTerminadosView();
                            break;

                        case (short)eModuloActual.CREACION_LISTAS:
                            LstEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                            IdMotivoE = _UltimoValor = -1;
                            NombrePrograma = _DatoD = NoOficio = _DatoU = string.Empty;
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CreacionListasExamenPView();
                            break;

                        default:
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CreacionListasExamenPView();
                            break;
                    };

                    break;

                case "nueva_busqueda":
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    break;

                case "agregar_area_tec":
                    SelectedAreTec = null;
                    NombreAreaMedica = OpinionAreaMedica = string.Empty;
                    IdAreaT = -1;
                    ValidacionesOpinionArea();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_OPINION_AREA_TECNICA);
                    break;

                case "editar_area_tec":
                    if (SelectedAreTec != null)
                    {
                        NombreAreaMedica = SelectedAreTec.NOMBRE;
                        OpinionAreaMedica = SelectedAreTec.OPINION;
                        IdAreaT = SelectedAreTec.ID_AREA_TECNICA;
                        ValidacionesOpinionArea();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_OPINION_AREA_TECNICA);
                    }

                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una opinión");
                        return;
                    }
                    break;

                case "cancelar_area_tec":
                    NombreAreaMedica = OpinionAreaMedica = string.Empty;
                    IdAreaT = -1;
                    SelectedAreTec = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_OPINION_AREA_TECNICA);
                    break;

                case "guardar_area_tec":
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                        return;
                    }

                    if (SelectedAreTec == null)
                    {
                        if (LstAreasTec == null)
                            LstAreasTec = new ObservableCollection<PFF_ACTA_DETERMINO>();

                        LstAreasTec.Add(
                            new PFF_ACTA_DETERMINO
                            {
                                ID_AREA_TECNICA = IdAreaT,
                                NOMBRE = NombreAreaMedica,
                                OPINION = OpinionAreaMedica,
                                AREA_TECNICA = SelArea
                            });
                    }

                    else
                    {
                        if (LstAreasTec.Remove(SelectedAreTec))
                        {
                            LstAreasTec.Add(
                            new PFF_ACTA_DETERMINO
                            {
                                ID_AREA_TECNICA = IdAreaT,
                                NOMBRE = NombreAreaMedica,
                                OPINION = OpinionAreaMedica,
                                AREA_TECNICA = SelArea
                            });
                        };
                    }

                    SelectedAreTec = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_OPINION_AREA_TECNICA);
                    break;

                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;

                case "consultar_documentos_cero":
                    if (SelectedSolicitud == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un imputado para continuar");
                        return;
                    }

                    InicializaLista((short)eSituacionActual.STAGE0);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VISUALIZAR_LISTA_DOCUMENTOS);
                    break;

                case "consultar_documentos_tres":
                    InicializaLista((short)eSituacionActual.STAGE3);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VISUALIZAR_LISTA_DOCUMENTOS);
                    break;

                case "ver_documento_personalidad":
                    if (SelectedSolicitud != null && SelectedDocumento != null)
                        MuestraFormato(SelectedDocumento, SelectedSolicitud);
                    break;

                case "ficha_menu":
                    break;

                case "cancelar_documento":
                    SelectedSolicitud = SelectedEstudioTerminado = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VISUALIZAR_LISTA_DOCUMENTOS);
                    break;

                case "buscar_salir":

                    StaticSourcesViewModel.SourceChanged = false;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "reporte_menu":
                    break;

                case "buscar_seleccionar":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }

                    foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                    {
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
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
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                        return;
                    }

                    SelectedInterno = SelectIngreso.IMPUTADO;
                    if (SelectIngreso != null)
                        SelectIngreso = SelectIngreso;

                    this.SeleccionaIngreso();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    StaticSourcesViewModel.SourceChanged = false;
                    break;

                case "consultar_documentos_personalidad":
                    if (SelectedSolicitud == null)
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un imputado para continuar");
                    else
                        ImprimirPartidaJuridica(SelectedSolicitud);
                    break;

                case "ver_documento_cero":

                    break;
                case "guardar_menu":
                    if (!PInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    if (ModuloActual == (short)eModuloActual.DICTAMEN_FINAL)
                        return;

                    if (LstEstudiosPersonalidad == null || !LstEstudiosPersonalidad.Any())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Es necesario agregar al menos un candidato a estudios y especificar el motivo de los estudios");
                        return;
                    }

                    else
                    {
                        ValidacionesCreacionLista();
                        if (base.HasErrors)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                            }));
                            return;
                        }

                        if (IdMotivoE != null && IdMotivoE.Value != -1)
                        {
                            if (!ValidaFichasJuridicas())
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Es necesario capturar la ficha jurídica a todos los imputados");
                                return;
                            }
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione el motivo del estudio");
                            return;
                        }

                        if (LstEstudiosPersonalidad.Any())
                            foreach (var item in LstEstudiosPersonalidad)
                                if (item.INGRESO != null)
                                    if (item.INGRESO.CAUSA_PENAL.Any())
                                        if (item.INGRESO.CAUSA_PENAL.Any(c => c.CP_FUERO == "F" && c.ID_ESTATUS_CP == 1))
                                            if (DiasEsu == null)
                                            {
                                                (new Dialogos()).ConfirmacionDialogo("Validación!", "Ingrese la cantidad de días a considerar");
                                                return;
                                            };

                        if (new cEstudioPersonalidad().GuardaListaCandidatos(LstEstudiosPersonalidad, IdMotivoE, NoOficio, NombrePrograma, DiasEsu, SelectedFueroCreacionLista, (short)enumMensajeTipo.PROGRAMACION_ESTUDIOS_PERSONALIDAD))
                        {

                            _CantidadFichas = LstEstudiosPersonalidad.Count();
                            ObservableCollection<PERSONALIDAD> _copia = new ObservableCollection<PERSONALIDAD>();
                            foreach (var item in LstEstudiosPersonalidad)
                            {
                                if (item.INGRESO != null)
                                    if (item.INGRESO.CAUSA_PENAL.Any(c => c.CP_FUERO == "C" && c.ID_ESTATUS_CP == 1))
                                        _copia.Add(new PERSONALIDAD
                                        {
                                            ID_ANIO = item.ID_ANIO,
                                            ID_IMPUTADO = item.ID_IMPUTADO,
                                            ID_SITUACION = item.ID_SITUACION,
                                            ID_CENTRO = item.ID_CENTRO,
                                            ID_ESTUDIO = item.ID_ESTUDIO,
                                            ID_INGRESO = item.ID_INGRESO,
                                            ID_MOTIVO = item.ID_MOTIVO,
                                            INICIO_FEC = item.INICIO_FEC,
                                            NUM_OFICIO = NoOficio,
                                            PROG_NOMBRE = NombrePrograma,
                                            RESULT_ESTUDIO = item.RESULT_ESTUDIO,
                                            SOLICITADO = item.SOLICITADO,
                                            SOLICITUD_FEC = item.SOLICITUD_FEC,
                                            TERMINO_FEC = item.TERMINO_FEC,
                                            INGRESO = item.INGRESO,
                                            PLAZO_DIAS = DiasEsu
                                        });
                            };

                            LstEstudiosPersonalidad.Clear();

                            if (_copia != null && _copia.Any())
                            {
                                if (await new Dialogos().ConfirmarEliminar("Exito!", "Se ha guardado la lista con éxito,  ¿Desea visualizar el formato de remisión?") != 1)
                                {
                                    _copia = new ObservableCollection<PERSONALIDAD>();
                                    IdMotivoE = _UltimoValor = -1;
                                    NoOficio = NombrePrograma = _DatoU = _DatoD = string.Empty;
                                    DiasEsu = new short?();
                                    StaticSourcesViewModel.SourceChanged = false;
                                    break;
                                }

                                _CantidadFichas = _copia.Count;
                                MuestraOficioEnvio(_copia);
                                _copia.Clear();
                                StaticSourcesViewModel.SourceChanged = false;
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Éxito", "Se ha guardado la lista con éxito");

                            _copia = new ObservableCollection<PERSONALIDAD>();
                            IdMotivoE = _UltimoValor = -1;
                            NoOficio = NombrePrograma = _DatoU = _DatoD = string.Empty;
                            DiasEsu = new short?();
                            StaticSourcesViewModel.SourceChanged = false;
                            return;
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error!", "Surgió un error al ingresar la lista de candidatos");
                            return;
                        }
                    }

                    break;

                #region Ficha
                case "guardar_ficha_menu":
                    ValidacionesFichaJuridica();
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));

                        return;
                    }
                    if (!TramiteLibertadAntic && !TramiteMod && !TramiteDiagn && !TramiteTr && !TramiteTraslVol)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione el tipo de trámite.");
                        return;
                    }

                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Ingresando ficha de identificación jurídica", () => GuardarFichaIndividual()))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Exito!", "Se ha registrado la ficha de identificacion exitosamente");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error!", "Surgió un error al ingresar la ficha juridica");

                    break;

                case "agregar_ficha":
                    if (SelectedSolicitud != null)
                    {
                        _UltimoValor = IdMotivoE;
                        _DatoU = NoOficio;
                        _DatoD = NombrePrograma;
                        var _validacionFueroFichaComun = SelectedSolicitud.INGRESO != null ? SelectedSolicitud.INGRESO.CAUSA_PENAL : null;
                        if (_validacionFueroFichaComun.Any())
                            if (_validacionFueroFichaComun.FirstOrDefault(c => c.ID_ESTATUS_CP == 1 && c.CP_FUERO == "C") != null)
                            {
                                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new FichaIdentificacionJuridica();
                                StaticSourcesViewModel.SourceChanged = true;
                                ProcesaDatosImputadoSeleccionado();
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Validación", "El imputado seleccionado es de fuero federal");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un imputado para continuar");

                    break;
                case "editar_ficha":
                    //no action
                    break;
                case "cancelar_ficha":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CreacionListasExamenPView();
                    break;
                case "limpiar_ficha_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new FichaIdentificacionJuridica();
                    break;
                case "regresar_ficha":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var respuesta = await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Seguro que desea salir sin guardar?");
                        if (respuesta == 1)
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CreacionListasExamenPView();
                    }
                    else
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CreacionListasExamenPView();
                    break;
                case "reporte_ficha_menu":
                    if (SelectedSolicitud != null)
                        ImprimeFichaJuridica(SelectedSolicitud);
                    break;
                #endregion

                #region Cierre y Dictamen de estudios de personalidad
                case "regresar_acta_federal":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudiosTerminadosView();
                    StaticSourcesViewModel.SourceChanged = false;
                    break;


                case "regresar_acta":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudiosTerminadosView();
                    StaticSourcesViewModel.SourceChanged = false;
                    break;

                case "agregar_acta_comun":
                    if (SelectedEstudioTerminado != null)
                    {
                        var _validacionFuero = new cCausaPenal().Obtener(SelectedEstudioTerminado.ID_CENTRO, SelectedEstudioTerminado.ID_ANIO, SelectedEstudioTerminado.ID_IMPUTADO, SelectedEstudioTerminado.ID_INGRESO);
                        var _CausaPenalActiva = _validacionFuero.FirstOrDefault(x => x.ID_ESTATUS_CP == 1);
                        if (_CausaPenalActiva != null)
                        {
                            if (_CausaPenalActiva.CP_FUERO == "C")
                            {
                                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ActaConsejoTecnicoComunView();
                                StaticSourcesViewModel.SourceChanged = false;
                                InicializaCamposActaConsejo(SelectedEstudioTerminado);
                                ValidacionesActaConsejoTecnicoComun();
                            }

                            if (_CausaPenalActiva.CP_FUERO == "F")
                            {
                                if (SelectedEstudioTerminado.PERSONALIDAD_FUERO_FEDERAL == null)
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Es necesario realizar los estudios para redactar el acta de consejo técnico de fuero federal");
                                    return;
                                };

                                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ActaConsejoTecnicoInterdisciplinarioFFView();
                                SelectedSolicitud = SelectedEstudioTerminado;
                                ActaConsejoTecnicoInterd(SelectedEstudioTerminado.INGRESO, SelectedEstudioTerminado.ID_ESTUDIO);
                                StaticSourcesViewModel.SourceChanged = false;
                            }
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un estudio para continuar");

                    break;

                //ActaFederal
                case "reporte_acta_federal":
                    ActaFederal(SelectedEstudioTerminado.INGRESO, SelectedEstudioTerminado.ID_ESTUDIO);
                    break;

                case "guardar_acta_federal":
                    ValidacionesActaFederal();
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));

                        return;
                    }

                    if (GuardarActaFederal(SelectedEstudioTerminado.INGRESO, SelectedEstudioTerminado.ID_ESTUDIO))
                        (new Dialogos()).ConfirmacionDialogo("Exito!", "Se ha registrado el acta de consejo técnico exitosamente");
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error", "Surgió un error al guardar el acta");

                    break;
                case "guardar_acta_comun":
                    ValidacionesActaConsejoTecnicoComun();
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));

                        return;
                    }
                    else
                    {
                        ACTA_CONSEJO_TECNICO _Entity = new ACTA_CONSEJO_TECNICO()
                        {
                            ACTUACION = ActuacionActaComun,
                            ACUERDO = AcuerdoActaComun,
                            AREA_LABORAL = NombreAreaLaboralActaComun,
                            CRIMINOLOGIA = NombreCriminologiaActaComun,
                            EDUCATIVO = NombreEducativoActaComun,
                            ID_ANIO = SelectedEstudioTerminado.ID_ANIO,
                            ID_CENTRO = SelectedEstudioTerminado.ID_CENTRO,
                            ID_IMPUTADO = SelectedEstudioTerminado.ID_IMPUTADO,
                            ID_INGRESO = SelectedEstudioTerminado.ID_INGRESO,
                            ID_ESTUDIO = SelectedEstudioTerminado.ID_ESTUDIO,
                            INTERNO = NombreInternoActaComun,
                            JURIDICO = NombreJuridicoActaComun,
                            LUGAR = LugarActaComun,
                            MANIFESTARON = ManifestaronActaComun,
                            MEDICO = NombreMedicoActaComun,
                            OPINION = OpinionActaComun,
                            OPINION_CRIMINOLOGIA = OpinionCrimi,
                            PSICOLOGIA = NombrePsiccoActaComun,
                            OPINION_ESCOLAR = OpinionEscolar,
                            OPINION_LABORAL = OpinionLaboral,
                            OPINION_PSICOLOGICA = OpinionPsico,
                            SECRETARIO = NombreSecretarioActaComun,
                            SEGURIDAD_CUSTODIA = NombreSeguridadActaComun,
                            OPINION_MEDICO = OpinionMedico,
                            OPINION_SEGURIDAD = OpinionSeguridad,
                            OPINION_TRABAJO_SOCIAL = OpinionTrabSocial,
                            PRESIDENTE = NombrePresidenteActaComun,
                            TRABAJO_SOCIAL = NombreTrabajoSocialActaComun
                        };

                        if (new cActaConsejoTecnicoComun().InsertarActaConsejoTecnico(_Entity))
                        {
                            if (await new Dialogos().ConfirmarEliminar("Exito!", "El acta ha sido registrada con éxito,  ¿Desea visualizar el formato de remisión?") != 1)
                                break;

                            MuestraOficioRemisionDictamenEstudioPersonalidad(SelectedEstudioTerminado);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Surgió un error al ingresar el acta ");
                    }

                    break;

                case "limpiar_acta_comun":

                    break;

                case "reporte_acta_comun":
                    if (SelectedEstudioTerminado != null)
                        MuestraFormatoActaConsejoTecnicoComun(SelectedEstudioTerminado);
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un estudio para continuar");

                    break;

                case "mostrar_oficio_remision":
                    if (SelectedEstudioTerminado != null)
                        MuestraOficioRemisionDictamenEstudioPersonalidad(SelectedEstudioTerminado);
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un registro para continuar");

                    break;

                #endregion
                default:
                    //no action
                    break;
            }
        }

        private void MuestraOficioRemisionDictamenEstudioPersonalidad(PERSONALIDAD _Entity)
        {
            try
            {
                if (_Entity == null)
                    return;

                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();
                var NombreAreasTecnicas = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 1 && x.USUARIO.EMPLEADO.ID_CENTRO == _Entity.INGRESO.ID_CENTRO).FirstOrDefault();
                cFormatoDictamenEstudiosPersonalidad DatosReporte = new cFormatoDictamenEstudiosPersonalidad();
                var _datos = new cActaConsejoTecnicoComun().GetData(x => x.ID_IMPUTADO == _Entity.ID_IMPUTADO && x.ID_INGRESO == _Entity.ID_INGRESO && x.ID_ESTUDIO == _Entity.ID_ESTUDIO && x.ID_CENTRO == _Entity.ID_CENTRO).FirstOrDefault();
                var _centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                if (_datos != null)
                {
                    DatosReporte = new cFormatoDictamenEstudiosPersonalidad()
                    {
                        Expediente = string.Format("Expediente: {0} / {1}", _Entity.ID_ANIO, _Entity.ID_IMPUTADO),
                        NombreInterno = string.Format("{0} \n ({1})",
                            _datos.INGRESO != null ? _datos.INGRESO.IMPUTADO != null ?
                                string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_datos.INGRESO.IMPUTADO.NOMBRE) ? _datos.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                             !string.IsNullOrEmpty(_datos.INGRESO.IMPUTADO.PATERNO) ? _datos.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                             !string.IsNullOrEmpty(_datos.INGRESO.IMPUTADO.MATERNO) ? _datos.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty,
                        _datos.OPINION == "S" ? "APROBADO" : "APROBADO POR MAYORÍA"),
                        NombreDirectorCERESO = _centro != null ? !string.IsNullOrEmpty(_centro.DIRECTOR) ? _centro.DIRECTOR.Trim() : string.Empty : string.Empty,
                        DirectorPenas = NombreAreasTecnicas != null ? NombreAreasTecnicas.USUARIO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                                        !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                                        !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty)
                                                         : string.Empty : string.Empty : string.Empty : string.Empty,
                        Fecha = string.Format("{0} a {1}", string.Format("{0} {1}", _centro != null ? _centro.ID_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(_centro.MUNICIPIO.MUNICIPIO1) ? _centro.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty : string.Empty,
                                                                                  _centro != null ? _centro.MUNICIPIO != null ? _centro.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(_centro.MUNICIPIO.ENTIDAD.DESCR) ? _centro.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty),
                                                                                  Fechas.fechaLetra(Fechas.GetFechaDateServer, false).ToUpper()),
                        Dictamen = string.Format("DIRECTOR DEL {0}", _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty)
                    };
                };

                cEncabezado Encabezado = new cEncabezado();
                Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                Encabezado.NombreReporte = _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                Encabezado.PieUno = Parametro.DESCR_ISO_1.Replace(";", ";\n");


                #region Inicializacion de reporte
                View.Report.LocalReport.ReportPath = "Reportes/rDictamenIndividualEstudiosPersonalidad.rdlc";
                View.Report.LocalReport.DataSources.Clear();
                #endregion

                #region Definicion d origenes de datos
                var ds1 = new List<cFormatoDictamenEstudiosPersonalidad>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(DatosReporte);
                rds1.Name = "DataSet2";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                //datasource dos
                var ds2 = new List<cEncabezado>();
                ds2.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet1";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                #endregion

                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        private void MuestraOficioRemisionDEPMJ(ObservableCollection<PERSONALIDAD> _lstEstudios)
        {
            try
            {
                if (_lstEstudios == null)
                    return;

                if (_lstEstudios.Any())
                {
                    var View = new ReportesView();
                    #region Iniciliza el entorno para mostrar el reporte al usuario
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    View.Owner = PopUpsViewModels.MainWindow;
                    View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    //View.Show();
                    #endregion

                    var _muestra = _lstEstudios.FirstOrDefault();
                    string NombreCentro = string.Empty;
                    string EntitdadActual = string.Empty;
                    var Centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (Centro != null)
                        NombreCentro = !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty;

                    var _entidad = Centro.ID_ENTIDAD.HasValue ? Centro.ID_MUNICIPIO.HasValue ? new cMunicipio().GetData().FirstOrDefault(x => x.ID_ENTIDAD == Centro.ID_ENTIDAD && x.ID_MUNICIPIO == Centro.ID_MUNICIPIO) : null : null;

                    var NombreEjecucion = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 20 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();

                    string lstAprobados = string.Empty;
                    string lstAprobadosMayoria = string.Empty;
                    short _cont = 0;
                    short _cont1 = 0;

                    foreach (var item in _lstEstudios)
                    {
                        if (item.ACTA_CONSEJO_TECNICO == null)
                            continue;

                        string _alia = string.Empty;
                        if (item.ACTA_CONSEJO_TECNICO.OPINION == "S")
                        {
                            _cont++;
                            var _aliasImputado = new cAlias().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                            if (_aliasImputado != null && _aliasImputado.Any())
                                foreach (var item2 in _aliasImputado)
                                    _alia += string.Format(" y/o {0}", string.Concat(!string.IsNullOrEmpty(item2.NOMBRE) ? item2.NOMBRE.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.PATERNO) ? item2.PATERNO.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.MATERNO) ? item2.MATERNO.Trim() : string.Empty));

                            lstAprobados += string.Format("{0}. {1} {2} \n", _cont,
                                string.Format("{0} {1} {2}",
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty),
                                _alia
                                );
                        }

                        if (item.ACTA_CONSEJO_TECNICO.OPINION == "N")
                        {
                            _cont1++;
                            var _aliasImputado = new cAlias().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                            if (_aliasImputado != null && _aliasImputado.Any())
                                foreach (var item2 in _aliasImputado)
                                    _alia += string.Format(" y/o {0}", string.Concat(!string.IsNullOrEmpty(item2.NOMBRE) ? item2.NOMBRE.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.PATERNO) ? item2.PATERNO.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.MATERNO) ? item2.MATERNO.Trim() : string.Empty));

                            lstAprobadosMayoria += string.Format("{0}. {1} {2}  \n", _cont1,
                                string.Format("{0} {1} {2}",
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty),
                                _alia
                                );
                        }
                    }


                    cFormatoRemisionDPMJ DatosReporte = new cFormatoRemisionDPMJ()
                    {
                        NoOficio = string.Format("OFICIO: {0}", _muestra.NUM_OFICIO1),
                        NombreCentro = Centro != null ? !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty : string.Empty,
                        Fecha = string.Format("{0} a {1} ", _entidad != null ? !string.IsNullOrEmpty(_entidad.MUNICIPIO1) ? _entidad.MUNICIPIO1.Trim() : string.Empty : string.Empty, Fechas.fechaLetra(Fechas.GetFechaDateServer, false)),
                        NombreDirector = NombreEjecucion != null ? NombreEjecucion.USUARIO != null ? NombreEjecucion.USUARIO.EMPLEADO != null ? NombreEjecucion.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreEjecucion.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreEjecucion.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreEjecucion.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreEjecucion.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreEjecucion.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreEjecucion.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty,
                        NombrePrograma = _muestra != null ? !string.IsNullOrEmpty(_muestra.PROG_NOMBRE) ? _muestra.PROG_NOMBRE.Trim() : string.Empty : string.Empty,
                        CantidadEstudios = _lstEstudios.Count.ToString(),
                        NombreAprobados = lstAprobados,
                        NombreAprobadosMayoria = lstAprobadosMayoria,
                        NombreDirectorCereso = Centro != null ? !string.IsNullOrEmpty(Centro.DIRECTOR) ? Centro.DIRECTOR.Trim() : string.Empty : string.Empty
                    };

                    cEncabezado Encabezado = new cEncabezado();
                    Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA DEL ESTADO DE BAJA CALIFORNIA";
                    Encabezado.TituloDos = Parametro.ENCABEZADO2;
                    Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                    Encabezado.NombreReporte = !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty;
                    Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                    Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                    Encabezado.PieUno = Parametro.DESCR_ISO_1.Replace(";", ";\n");


                    #region Inicializacion de reporte
                    View.Report.LocalReport.ReportPath = "Reportes/rFormatoRemisionEstudiosDPMJ.rdlc";
                    View.Report.LocalReport.DataSources.Clear();
                    #endregion

                    #region Definicion d origenes de datos
                    var ds1 = new List<cFormatoRemisionDPMJ>();
                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    ds1.Add(DatosReporte);
                    rds1.Name = "DataSet2";
                    rds1.Value = ds1;
                    View.Report.LocalReport.DataSources.Add(rds1);

                    //datasource dos
                    var ds2 = new List<cEncabezado>();
                    ds2.Add(Encabezado);
                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet1";
                    rds2.Value = ds2;
                    View.Report.LocalReport.DataSources.Add(rds2);

                    #endregion

                    View.Report.RefreshReport();
                    byte[] renderedBytes;

                    Microsoft.Reporting.WinForms.Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;

                    renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                    var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                    System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                    renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                    var tc = new TextControlView();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.editor.Loaded += (s, e) =>
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                    };

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Owner = PopUpsViewModels.MainWindow;
                    tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Show();

                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void MuestraOficioEnvio(ObservableCollection<PERSONALIDAD> _listaEstudios)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                string NombreCentro = string.Empty;
                string NombreMunicipio = string.Empty;
                var Centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                if (Centro != null)
                {
                    NombreCentro = !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty;
                    NombreMunicipio = string.Format("{0} {1}", Centro.ID_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(Centro.MUNICIPIO.MUNICIPIO1) ? Centro.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                        Centro.ID_MUNICIPIO.HasValue ? Centro.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(Centro.MUNICIPIO.ENTIDAD.DESCR) ? Centro.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty);
                };

                string ListaImp = string.Empty;
                short _dummy = 0;
                if (_listaEstudios != null && _listaEstudios.Any())
                    foreach (var item in _listaEstudios)
                    {
                        var _FichaAnterior = new cFichasJuridicas().GetData(x => x.ID_INGRESO == item.ID_INGRESO && x.ID_IMPUTADO == item.ID_IMPUTADO && x.ID_ANIO == item.ID_ANIO && x.ID_CENTRO == item.ID_CENTRO).FirstOrDefault();

                        string _alia = string.Empty;
                        _dummy++;
                        var _aliasImputado = new cAlias().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                        if (_aliasImputado != null && _aliasImputado.Any())
                            foreach (var item2 in _aliasImputado)
                                _alia += string.Format(" y/o {0}", string.Concat(!string.IsNullOrEmpty(item2.NOMBRE) ? item2.NOMBRE.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.PATERNO) ? item2.PATERNO.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.MATERNO) ? item2.MATERNO.Trim() : string.Empty));

                        ListaImp += _dummy + ". " + string.Format("{0} {1} {2} {3} ({4})", item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,//0
                                                        item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,//1
                                                        item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,//2
                                                        !string.IsNullOrEmpty(_alia) ? _alia.Trim() : string.Empty,//3
                                                        _FichaAnterior != null ? !string.IsNullOrEmpty(_FichaAnterior.P2_CLAS_JURID) ? _FichaAnterior.P2_CLAS_JURID.Trim() : string.Empty : string.Empty)//4 
                                                        + "\n";
                    };

                string _NombreAreasTecnicas = string.Empty;
                var NombreAreasTecnicas = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == (short)eDatosRolesProcesosPersonalidad.COORDINACION_TECNICA && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                if (NombreAreasTecnicas != null)
                    _NombreAreasTecnicas = NombreAreasTecnicas.USUARIO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                string NombreJ = string.Empty;
                var NombreJuridico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == (short)eDatosRolesProcesosPersonalidad.JURIDICO && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                if (NombreJuridico != null)
                    NombreJ = NombreJuridico.USUARIO != null ? NombreJuridico.USUARIO.EMPLEADO != null ? NombreJuridico.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;


                string NombreC = string.Empty;
                var NombreComandante = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == (short)eDatosRolesProcesosPersonalidad.COMANDANCIA && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                if (NombreComandante != null)
                    NombreC = NombreComandante.USUARIO != null ? NombreComandante.USUARIO.EMPLEADO != null ? NombreComandante.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreComandante.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreComandante.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreComandante.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreComandante.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreComandante.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreComandante.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;


                string _NombreP = string.Concat("         En seguimiento a la ruta crítica establecida para el <b>", !string.IsNullOrEmpty(NombrePrograma) ? NombrePrograma.ToUpper() : NombrePrograma, "</b> ,remito a usted ", _CantidadFichas, " Fichas de Identificación Jurídica del Fuero Común de los privados de la libertad que se relacionan, los cuales se encuentran actualmente recluidos en este Centro en calidad de sentenciados, a efecto de que se les realice el Estudio de Personalidad correspondiente:");

                var _fecha = _listaEstudios.Any() ? _listaEstudios.FirstOrDefault().SOLICITUD_FEC.HasValue ? _listaEstudios.FirstOrDefault().SOLICITUD_FEC.Value : Fechas.GetFechaDateServer : Fechas.GetFechaDateServer;
                string Fecha = string.Format("{0} A {1} ", NombreMunicipio, Fechas.fechaLetra(_fecha, false).ToUpper());
                NoOficio = _listaEstudios.Any() ? _listaEstudios.FirstOrDefault().NUM_OFICIO : string.Empty;
                #region Inicia definicion de datos para el reporte
                var DatosReporte = new cPeticionRealizacionEstudiosPersonalidad()
                    {
                        CCP1 = string.Format("c.c.p.C. {0} .- Comandante del Centro de Reinserción Social.- Para la elaboración de estudios", NombreC),
                        CCP2 = string.Format("c.c.p.C. {0} .- Comandante del Área Femenil.- Mismo fin.", NombreC),
                        Fecha = Fecha,
                        NombreCentro = NombreCentro,
                        NombreCoordinador = _NombreAreasTecnicas,
                        NombreJefeDptoJuridico = NombreJ,
                        NoOficio = string.Format("Se remiten Resúmenes Sociales Oficio número {0}", !string.IsNullOrEmpty(NoOficio) ? NoOficio.ToUpper() : NoOficio),
                        NombrePrograma = _NombreP,
                        ListaImputadosEstudios = ListaImp
                    };

                #endregion


                cEncabezado Encabezado = new cEncabezado();
                Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                Encabezado.NombreReporte = NombreCentro;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                Encabezado.PieUno = Parametro.DESCR_ISO_1.Replace(";", ";\n");

                #region Inicializacion de reporte
                View.Report.LocalReport.ReportPath = "Reportes/rPeticionRealizacionEstudiosPersonalidad.rdlc";
                View.Report.LocalReport.DataSources.Clear();
                #endregion

                #region Definicion d origenes de datos
                var ds1 = new List<cPeticionRealizacionEstudiosPersonalidad>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(DatosReporte);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                //datasource dos
                var ds2 = new List<cEncabezado>();
                ds2.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                #endregion

                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private bool ValidaFichasJuridicas()
        {
            try
            {
                bool _validacion = false;
                if (LstEstudiosPersonalidad != null && LstEstudiosPersonalidad.Any())
                    foreach (var item in LstEstudiosPersonalidad)
                    {
                        var _fue = item.INGRESO != null ? item.INGRESO.CAUSA_PENAL : null;
                        if (_fue != null)
                            if (_fue.Any())
                            {
                                if (_fue.FirstOrDefault(c => c.ID_ESTATUS_CP == 1 && c.CP_FUERO == "C") != null)
                                    _validacion = new cFichasJuridicas().ValidaFicha(item.ID_IMPUTADO, item.ID_INGRESO, item.ID_ANIO);
                                else
                                    _validacion = true;
                            };

                        if (!_validacion)
                            return false;
                    };

                return true;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private async void InicializaDictamen()
        {
            try
            {
                ModuloActual = (short)eModuloActual.DICTAMEN_FINAL;
                if (StaticSourcesViewModel.SourceChanged)
                {
                    var respuesta = await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Seguro que desea salir sin guardar?");
                    if (respuesta == 1)
                    {
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudiosTerminadosView();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else
                {
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudiosTerminadosView();
                    StaticSourcesViewModel.SourceChanged = false;
                }
            }
            catch (Exception exc)
            {

                throw;
            }
        }

        private async void InicializaCreacionListas()
        {
            try
            {
                ModuloActual = (short)eModuloActual.CREACION_LISTAS;
                if (StaticSourcesViewModel.SourceChanged)
                {
                    var respuesta = await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Seguro que desea salir sin guardar?");
                    if (respuesta == 1)
                    {
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CreacionListasExamenPView();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else
                {
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CreacionListasExamenPView();
                    StaticSourcesViewModel.SourceChanged = false;
                }
            }
            catch (Exception exc)
            {

                throw;
            }
        }

        private void ImprimeFichaJuridica(PERSONALIDAD _dato)
        {
            try
            {
                if (_dato == null)
                    return;

                var _FichaActual = new cFichasJuridicas().GetData(x => x.ID_IMPUTADO == _dato.ID_IMPUTADO && x.ID_INGRESO == _dato.ID_INGRESO).FirstOrDefault();
                if (_FichaActual != null)
                {
                    var DatosReporte = new cFichaIdentifJuridica();
                    var View = new ReportesView();
                    #region Iniciliza el entorno para mostrar el reporte al usuario
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    View.Owner = PopUpsViewModels.MainWindow;
                    View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                    View.Show();

                    #endregion

                    #region Cuerpo del reporte
                    #region Info Basica
                    var _Foto = _FichaActual.INGRESO != null ? _FichaActual.INGRESO.INGRESO_BIOMETRICO.Any() ? _FichaActual.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : null : null;
                    if (_Foto == null)
                        DatosReporte.Foto = new Imagenes().getImagenPerson();
                    else
                        DatosReporte.Foto = _FichaActual.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;

                    DatosReporte.NombreImputado = string.Format("{0} {1} {2}", _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.NOMBRE) ? _FichaActual.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                        _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.PATERNO) ? _FichaActual.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                        _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.MATERNO) ? _FichaActual.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty);

                    var _aliasByImputado = new cAlias().ObtenerTodosXImputado(_FichaActual.ID_CENTRO, _FichaActual.ID_ANIO, _FichaActual.ID_IMPUTADO);
                    if (_aliasByImputado != null && _aliasByImputado.Any())
                        foreach (var item in _aliasByImputado)
                            DatosReporte.Alias += string.Format(" y/O {0} {1} {2}", !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Trim() : string.Empty);

                    DatosReporte.Edad = string.Concat(new Fechas().CalculaEdad(_FichaActual.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString(), " AÑOS ");

                    if (_FichaActual.INGRESO != null && _FichaActual.INGRESO.ID_UB_CENTRO.HasValue)
                    {
                        var _centro = new cCentro().GetData(x => x.ID_CENTRO == _FichaActual.INGRESO.ID_UB_CENTRO).FirstOrDefault();
                        if (_centro != null)
                            DatosReporte.CeReSo = !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty;
                    }
                    else
                    {
                        var _centro = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                        if (_centro != null)
                            DatosReporte.CeReSo = !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty;
                    }

                    DatosReporte.Sentencia = _FichaActual.P2_SENTENCIA;
                    DatosReporte.Procesos = _FichaActual.P2_PROCESOS;
                    DatosReporte.Juzgados = _FichaActual.P2_JUZGADOS;
                    DatosReporte.DescrSituacionJuridi = _FichaActual.P2_CLAS_JURID;
                    DatosReporte.CausoEjecutoria = _FichaActual.P2_EJECUTORIA;
                    DatosReporte.PorcentPena = _FichaActual.P2_PENA_COMPURG;
                    DatosReporte.ProcedenteD = _FichaActual.P2_PROCEDENTE;
                    DatosReporte.FecIngreso = _FichaActual.P2_FEC_INGRESO.HasValue ? _FichaActual.P2_FEC_INGRESO.Value.ToString("dd/MM/yyyy") : string.Empty;
                    DatosReporte.Entidad = _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? _FichaActual.INGRESO.ID_ENTIDAD.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.MUNICIPIO.ENTIDAD.DESCR) ? _FichaActual.INGRESO.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty;
                    DatosReporte.Expediente = string.Format("{0} / {1}", _FichaActual.ID_ANIO.ToString(), _FichaActual.ID_IMPUTADO.ToString());
                    DatosReporte.Fecha = _FichaActual.FICHA_FEC.HasValue ? Fechas.fechaLetra(_FichaActual.FICHA_FEC.Value, false).ToUpper() : string.Empty;
                    //DatosReporte.EdoCivil = _FichaActual.INGRESO.IMPUTADO != null ? _FichaActual.INGRESO.IMPUTADO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR) ? _FichaActual.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                    DatosReporte.EdoCivil = _FichaActual.INGRESO != null ? _FichaActual.INGRESO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.ESTADO_CIVIL.DESCR) ? _FichaActual.INGRESO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                    var _EstadoNac = new cEntidad().GetData(x => x.ID_ENTIDAD == _FichaActual.INGRESO.IMPUTADO.NACIMIENTO_ESTADO).FirstOrDefault();
                    var _MunicipioNac = new cMunicipio().GetData(x => x.ID_MUNICIPIO == _FichaActual.INGRESO.IMPUTADO.NACIMIENTO_MUNICIPIO && x.ID_ENTIDAD == _FichaActual.INGRESO.IMPUTADO.NACIMIENTO_ESTADO).FirstOrDefault();
                    DatosReporte.LugarOrigen = string.Format("{0}, {1}", _EstadoNac != null ? !string.IsNullOrEmpty(_EstadoNac.DESCR) ? _EstadoNac.DESCR.Trim() : string.Empty : string.Empty, _MunicipioNac != null ? !string.IsNullOrEmpty(_MunicipioNac.MUNICIPIO1) ? _MunicipioNac.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                    DatosReporte.FecNac = _FichaActual.INGRESO.IMPUTADO.NACIMIENTO_FECHA.HasValue ? _FichaActual.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty;
                    if (_FichaActual.INGRESO != null && _FichaActual.INGRESO.CAMA != null)
                        DatosReporte.Ubicado = string.Format("{0}-{1}{2}-{3}",
                                                   _FichaActual.INGRESO.CAMA.CELDA != null ? _FichaActual.INGRESO.CAMA.CELDA.SECTOR != null ? _FichaActual.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? _FichaActual.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                   _FichaActual.INGRESO.CAMA.CELDA != null ? _FichaActual.INGRESO.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.CAMA.CELDA.SECTOR.DESCR) ? _FichaActual.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                                                   _FichaActual.INGRESO.CAMA.CELDA != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.CAMA.CELDA.ID_CELDA) ? _FichaActual.INGRESO.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty, _FichaActual.INGRESO.ID_UB_CAMA);

                    //DatosReporte.Ocupacion = _FichaActual.INGRESO.IMPUTADO.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.OCUPACION.DESCR) ? _FichaActual.INGRESO.IMPUTADO.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                    DatosReporte.Ocupacion = _FichaActual.INGRESO.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.OCUPACION.DESCR) ? _FichaActual.INGRESO.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                    DatosReporte.Nacionalidad = _FichaActual.INGRESO.IMPUTADO.ID_NACIONALIDAD.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD) ? _FichaActual.INGRESO.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD.Trim() : string.Empty : string.Empty;
                    //DatosReporte.Escolaridad = _FichaActual.INGRESO.IMPUTADO.ID_ESCOLARIDAD.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.ESCOLARIDAD.DESCR) ? _FichaActual.INGRESO.IMPUTADO.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                    DatosReporte.Escolaridad = _FichaActual.INGRESO.ID_ESCOLARIDAD.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.ESCOLARIDAD.DESCR) ? _FichaActual.INGRESO.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                    //DatosReporte.DomicilioExt = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.DOMICILIO_CALLE) ? _FichaActual.INGRESO.IMPUTADO.DOMICILIO_CALLE.Trim() : string.Empty,
                    //                                               _FichaActual.INGRESO.IMPUTADO.DOMICILIO_NUM_EXT.HasValue ? string.Concat("No. ", _FichaActual.INGRESO.IMPUTADO.DOMICILIO_NUM_EXT.Value.ToString()) : string.Empty,
                    //                                               _FichaActual.INGRESO.IMPUTADO.DOMICILIO_CODIGO_POSTAL.HasValue ? _FichaActual.INGRESO.IMPUTADO.DOMICILIO_CODIGO_POSTAL.Value.ToString() : string.Empty);
                    DatosReporte.DomicilioExt = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_FichaActual.INGRESO.DOMICILIO_CALLE) ? _FichaActual.INGRESO.DOMICILIO_CALLE.Trim() : string.Empty,
                                                                   _FichaActual.INGRESO.DOMICILIO_NUM_EXT.HasValue ? string.Concat("No. ", _FichaActual.INGRESO.DOMICILIO_NUM_EXT.Value.ToString()) : string.Empty,
                                                                   _FichaActual.INGRESO.DOMICILIO_CP.HasValue ? _FichaActual.INGRESO.DOMICILIO_CP.Value.ToString() : string.Empty);

                    DatosReporte.ProcesosPendientes = string.Format("SI ( {0} ) \t NO ( {1} ) ", _FichaActual.P3_PROCESOS_PENDIENTES == "S" ? "XX" : string.Empty, _FichaActual.P3_PROCESOS_PENDIENTES == "N" ? "XX" : string.Empty);
                    DatosReporte.FecUltimosExamenes = _FichaActual.P4_ULTIMO_EXAMEN_FEC.HasValue ? Fechas.fechaLetra(_FichaActual.P4_ULTIMO_EXAMEN_FEC.Value, false).ToUpper() : string.Empty;
                    DatosReporte.Aprob = string.Format("( {0} ) APROBADO \n ( {1} ) APLAZADO \n ( {2} ) MAYORÍA  \n ( {3} ) UNANIMIDAD ", _FichaActual.P5_RESOLUCION_APROBADO == "A" ? "XX" : string.Empty, _FichaActual.P5_RESOLUCION_APLAZADO == "B" ? "XX" : string.Empty,
                        _FichaActual.P5_RESOLUCION_MAYORIA == "A" ? "XX" : string.Empty, _FichaActual.P5_RESOLUCION_UNANIMIDAD == "B" ? "XX" : string.Empty);

                    DatosReporte.APartirD = _FichaActual.P2_PARTIR;

                    DatosReporte.TramiteDescripcion = string.Format("{0} {1} {2} {3}",
                        _FichaActual != null ? _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.NOMBRE) ? _FichaActual.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        _FichaActual != null ? _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.PATERNO) ? _FichaActual.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        _FichaActual != null ? _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.MATERNO) ? _FichaActual.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        DatosReporte.Alias);
                    DatosReporte.Criminod = _FichaActual.P6_CRIMINODINAMIA;
                    DatosReporte.Elaboro = _FichaActual.ELABORO;
                    DatosReporte.JefeJurid = _FichaActual.JEFE_DEPARTAMENTO;
                    DatosReporte.Delito = _FichaActual.P2_DELITO;
                    DatosReporte.Tr1 = _FichaActual.TRAMITE_DIAGNOSTICO == "D" ? "XX" : string.Empty;
                    DatosReporte.Tr2 = _FichaActual.P7_TRAMITE_TRASLADO == "C" ? "XX" : string.Empty;
                    DatosReporte.TR3 = _FichaActual.P7_TRAMITE_LIBERTAD == "A" ? "XX" : string.Empty;
                    DatosReporte.Tr4 = _FichaActual.P7_TRAMITE_MODIFICACION == "B" ? "XX" : string.Empty;
                    DatosReporte.Tr5 = _FichaActual.TRAMITE_TRASLADO_VOLUNTARIO == "E" ? "XX" : string.Empty;
                    DatosReporte.DetalleTram = string.Format("LIBERTAD ANTICIPADA ( {0} ) \n MODIFICACIÓN DE LA PENA ( {1} ) \n TRASLADO ( {2} )",
                        _FichaActual.P7_TRAMITE_LIBERTAD == "A" ? "XX" : string.Empty,
                        _FichaActual.P7_TRAMITE_MODIFICACION == "B" ? "XX" : string.Empty,
                        _FichaActual.P7_TRAMITE_TRASLADO == "C" ? "XX" : string.Empty);
                    DatosReporte.Oficio = !string.IsNullOrEmpty(_FichaActual.OFICIO_ESTUDIO_SOLICITADO) ? string.Format("( ESTUDIOS SOLICITADOS MEDIANTE OFICIO {0} )", _FichaActual.OFICIO_ESTUDIO_SOLICITADO) : string.Empty;
                    #endregion


                    #endregion

                    cEncabezado Encabezado = new cEncabezado();
                    Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                    Encabezado.TituloDos = Parametro.ENCABEZADO2;
                    Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                    Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;
                    Encabezado.NombreReporte = Parametro.ENCABEZADO_FUERO_COMUN;
                    Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                    Encabezado.PieUno = Parametro.DESCR_ISO_1;
                    Encabezado.PieDos = Parametro.DESCR_ISO_2;
                    Encabezado.PieTres = Parametro.DESCR_ISO_3;


                    #region Inicializacion de reporte
                    View.Report.LocalReport.ReportPath = "Reportes/rFichaIdentificacionJuridica.rdlc";
                    View.Report.LocalReport.DataSources.Clear();
                    #endregion


                    #region Definicion de origenes de datos


                    var ds1 = new List<cEncabezado>();
                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    ds1.Add(Encabezado);
                    rds1.Name = "DataSet1";
                    rds1.Value = ds1;
                    View.Report.LocalReport.DataSources.Add(rds1);

                    //datasource dos
                    var ds2 = new List<cFichaIdentifJuridica>();
                    ds2.Add(DatosReporte);
                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = ds2;
                    View.Report.LocalReport.DataSources.Add(rds2);

                    #endregion

                    View.Report.RefreshReport();
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private bool GuardarFichaIndividual()
        {
            try
            {
                string NombreUsuario = string.Empty;
                string NombreJefeJuridico = string.Empty;
                USUARIO _usuario = new cUsuario().Obtener(GlobalVar.gUsr);
                if (_usuario != null)
                {
                    var _persona = new cPersona().GetData(c => c.ID_PERSONA == _usuario.ID_PERSONA).FirstOrDefault();
                    if (_persona != null)
                        NombreUsuario = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_persona.NOMBRE) ? _persona.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_persona.PATERNO) ? _persona.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(_persona.MATERNO) ? _persona.MATERNO.Trim() : string.Empty);
                };

                string NombreJ = string.Empty;
                var NombreJuridico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 30 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                if (NombreJuridico != null)
                    NombreJ = NombreJuridico.USUARIO != null ? NombreJuridico.USUARIO.EMPLEADO != null ? NombreJuridico.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                FICHA_IDENTIFICACION_JURIDICA _ficha = new FICHA_IDENTIFICACION_JURIDICA()
                {
                    FICHA_FEC = Fechas.GetFechaDateServer,
                    ID_ANIO = SelectedSolicitud.ID_ANIO,
                    ID_CENTRO = SelectedSolicitud.ID_CENTRO,
                    ID_IMPUTADO = SelectedSolicitud.ID_IMPUTADO,
                    ID_INGRESO = SelectedSolicitud.ID_INGRESO,
                    P3_PROCESOS_PENDIENTES = ProcesosPendientes,
                    P4_ULTIMO_EXAMEN_FEC = FechaUltimoExamen,
                    P5_RESOLUCION_APLAZADO = ResolucionAprobado == "B" ? "B" : string.Empty,
                    P5_RESOLUCION_APROBADO = ResolucionAprobado == "A" ? "A" : string.Empty,
                    P5_RESOLUCION_MAYORIA = ResolucionAplazado == "A" ? "A" : string.Empty,
                    P5_RESOLUCION_UNANIMIDAD = ResolucionAplazado == "B" ? "B" : string.Empty,
                    P6_CRIMINODINAMIA = CriminoDinamia,
                    P7_TRAMITE_LIBERTAD = TramiteLibertadAntic == true ? "A" : string.Empty,
                    P7_TRAMITE_MODIFICACION = TramiteMod == true ? "B" : string.Empty,
                    P7_TRAMITE_TRASLADO = TramiteTr == true ? "C" : string.Empty,
                    TRAMITE_DIAGNOSTICO = TramiteDiagn == true ? "D" : string.Empty,
                    TRAMITE_TRASLADO_VOLUNTARIO = TramiteTraslVol == true ? "E" : string.Empty,
                    ELABORO = NombreUsuario,
                    OFICIO_ESTUDIO_SOLICITADO = NoOficioEstudio,
                    JEFE_DEPARTAMENTO = NombreJ,
                    DEPARTAMENTO_JURIDICO = NombreJ,
                    P2_CLAS_JURID = ClasifJuridFicha,
                    P2_DELITO = DelitoFicha,
                    P2_PROCESOS = ProcesosFicha,
                    P2_JUZGADOS = JuzgadoFicha,
                    P2_SENTENCIA = SentenciaFicha,
                    P2_PARTIR = APartirDeFicha,
                    P2_EJECUTORIA = CausoEjecFicha,
                    P2_FEC_INGRESO = FecIngresoFicha,
                    P2_PENA_COMPURG = PorcentPenaCompur,
                    P2_PROCEDENTE = ProcedenteDeFicha
                };

                return new cFichasJuridicas().InsertarFichaNueva(_ficha);
            }
            catch (Exception exc)
            {
                throw;
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
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }

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

                    var _fuero = SelectIngreso.CAUSA_PENAL.Any() ? SelectIngreso.CAUSA_PENAL.FirstOrDefault(c => c.ID_ESTATUS_CP == 1) : null;
                    if (_fuero != null)
                    {
                        if (!SelectedFueroCreacionLista)//fuero comun
                        {
                            if (_fuero.CP_FUERO == "F")
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", string.Format("EL IMPUTADO {0}/{1} {2} ES DE FUERO FEDERAL Y NO ES PERMITIDO AGREGAR IMPUTADOS DE FUERO FEDERAL DENTRO DE LA LISTA.",
                                    SelectIngreso.ID_ANIO,
                                    SelectIngreso.ID_IMPUTADO,
                                    string.Concat(SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty, " ",
                                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty, " ",
                                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty)
                                    ));
                                LimpiarCampos();
                                return;
                            }
                            else
                                ValidaEstudio();
                        };

                        if (SelectedFueroCreacionLista)//fuero federal
                        {
                            if (_fuero.CP_FUERO == "C")
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", string.Format("EL IMPUTADO {0}/{1} {2} ES DE FUERO COMÚN Y NO ES PERMITIDO AGREGAR IMPUTADOS DE FUERO COMÚN DENTRO DE LA LISTA.",
                                   SelectIngreso.ID_ANIO,
                                   SelectIngreso.ID_IMPUTADO,
                                   string.Concat(SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty, " ",
                                   SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty, " ",
                                   SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty)
                                   ));
                                LimpiarCampos();
                                return;
                            }
                            else
                                ValidaEstudio();
                        };
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "VERIFIQUE EL ESTATUS DE LA CAUSA PENAL DEL IMPUTADO.");
                        LimpiarCampos();
                        return;
                    };
                };
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }


        private /*async*/ void ProcesaCambioFueros()
        {
            try
            {
                if (LstEstudiosPersonalidad != null)
                    if (LstEstudiosPersonalidad.Any())
                    {
                        //if (await new Dialogos().ConfirmarEliminar("Advertencia", string.Format("Existen {0} elemento(s) en la lista ,al continuar, estos seran borrados de la lista ¿Desea continuar?",
                        //         LstEstudiosPersonalidad.Count)) != 1)
                        //{
                        //    var _Actual = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<MahApps.Metro.Controls.ToggleSwitch>(((Grid)_vista.FindName("Controles"))).FirstOrDefault();
                        //    if (_Actual != null)
                        //    {
                        //       // _Actual.IsChecked = !SelectedFueroCreacionLista;
                        //        _Actual.IsCheckedChanged += (s, e) =>
                        //            {
                        //                if (e != null)
                        //                {

                        //                }
                        //            };
                        //    }
                        //}

                        //SelectedFueroCreacionLista = !SelectedFueroCreacionLista;

                        if (!SelectedFueroCreacionLista)/// false es igual a comun, NO PERMITE FEDERALES
                        {
                            if (LstEstudiosPersonalidad.Any(x => x.INGRESO.CAUSA_PENAL.FirstOrDefault().CP_FUERO == "F"))
                            {
                                LimpiarCampos();
                                LstEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                            }
                            else
                                LimpiarCampos();
                        }
                        else
                        {///true es igual a FEDERAL, NO PERMITE COMUNES
                            if (LstEstudiosPersonalidad.Any(x => x.INGRESO.CAUSA_PENAL.FirstOrDefault().CP_FUERO == "C"))
                            {
                                LimpiarCampos();
                                LstEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                            }
                            else
                                LimpiarCampos();
                        }

                        LimpiarCampos();
                        LstEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                    }
                    else
                    {
                        LimpiarCampos();
                        LstEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                    }
                else
                {
                    LimpiarCampos();
                    LstEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private async void ValidaEstudio()
        {
            try
            {
                if (LstEstudiosPersonalidad == null)
                    LstEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();


                #region Validacion de fueros
                var Fuero = new cCausaPenal().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ESTATUS_CP == (short)eEstatusCausaPenalListas.ACTIVO).FirstOrDefault();
                if (Fuero == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de verificar el fuero del imputado");
                    LimpiarCampos();
                    return;
                }
                else
                {
                    if (string.IsNullOrEmpty(Fuero.CP_FUERO))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de verificar el fuero del imputado");
                        LimpiarCampos();
                        return;
                    };
                };

                #endregion

                DateTime FechaServer = Fechas.GetFechaDateServer;
                string _NombreUsuario = string.Empty;
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                if (NombreUsuario != null)
                    _NombreUsuario = NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty;

                var _fuero = SelectIngreso.CAUSA_PENAL.Any() ? SelectIngreso.CAUSA_PENAL.FirstOrDefault(c => c.ID_ESTATUS_CP == 1) : null;

                var UltimoEstudio = new cEstudioPersonalidad().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                if (UltimoEstudio.Any() && UltimoEstudio != null)
                {

                    if (LstEstudiosPersonalidad.FirstOrDefault(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO) != null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El imputado " + SelectIngreso.ID_ANIO + " / " + SelectIngreso.ID_IMPUTADO + " ya se encuentra dentro de la lista de candidatos");
                        LimpiarCampos();
                        return;
                    }

                    short DatosUltimoEstudio = new cEstudioPersonalidad().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_CENTRO == SelectIngreso.ID_CENTRO).Max(x => x.ID_ESTUDIO);

                    var DetalleUltimoEstudio = new cEstudioPersonalidad().GetData(y => y.ID_ESTUDIO == DatosUltimoEstudio).FirstOrDefault();
                    if (DetalleUltimoEstudio.SOLICITUD_FEC.HasValue)
                    {
                        if (DetalleUltimoEstudio.SOLICITUD_FEC.Value.Date.AddMonths(Convert.ToInt32(Parametro.LAPSO_ESTUDIOS)) >= FechaServer)//parametrizado a 6 meses
                        {
                            int a = 0, m = 0, d = 0;
                            string Detalle = new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, DetalleUltimoEstudio.SOLICITUD_FEC.Value.Date, out a, out  m, out d);
                            if (await
                                    new Dialogos().ConfirmarEliminar("Advertencia",
                                        string.Format(
                                            "Al imputado: {0} / {1} le fue programado un estudio hace {2} ,el estudio se encuentra: {3} ¿Desea continuar?",
                                            SelectIngreso.IMPUTADO != null
                                                ? SelectIngreso.IMPUTADO.ID_ANIO.ToString()
                                                : string.Empty,
                                            SelectIngreso.IMPUTADO != null
                                                ? SelectIngreso.IMPUTADO.ID_IMPUTADO.ToString()
                                                : string.Empty,
                                            !string.IsNullOrEmpty(Detalle)
                                                ? string.Format("{0}", Detalle)
                                                : "menos de un día",
                                            DetalleUltimoEstudio.ID_SITUACION.HasValue
                                                ? !string.IsNullOrEmpty(
                                                    DetalleUltimoEstudio.PERSONALIDAD_SITUACION.DESCR)
                                                    ? DetalleUltimoEstudio.PERSONALIDAD_SITUACION.DESCR.Trim()
                                                    : string.Empty
                                                : string.Empty)) != 1)
                            {
                                LimpiarCampos();
                                return;
                            }



                            LstEstudiosPersonalidad.Add(new PERSONALIDAD
                            {
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                SOLICITUD_FEC = FechaServer,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                SOLICITADO = _NombreUsuario,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                INGRESO = SelectIngreso
                            });

                            LimpiarCampos();
                        }
                        else
                        {
                            LstEstudiosPersonalidad.Add(new PERSONALIDAD
                            {
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                SOLICITUD_FEC = FechaServer,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                SOLICITADO = _NombreUsuario,
                                INGRESO = SelectIngreso
                            });

                            LimpiarCampos();
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de verificar la fecha de solicitud del estudio de personalidad");
                }

                else
                {//NO SE LE HA REALIZADO UN ESTUDIO DE PERSONALIDAD
                    if (LstEstudiosPersonalidad.FirstOrDefault(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO) != null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El imputado " + SelectIngreso.ID_ANIO + " / " + SelectIngreso.ID_IMPUTADO + " ya se encuentra dentro de la lista de candidatos");
                        LimpiarCampos();
                        return;
                    }

                    LstEstudiosPersonalidad.Add(new PERSONALIDAD
                        {
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            SOLICITUD_FEC = FechaServer,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            SOLICITADO = _NombreUsuario,
                            INGRESO = SelectIngreso
                        });

                    LimpiarCampos();
                }

                StaticSourcesViewModel.SourceChanged = true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void LimpiarCampos()
        {
            try
            {
                AnioD = FolioD = IngresosD = new int?();
                AnioBuscar = FolioBuscar = new int?();
                PaternoD = MaternoD = NombreD = UbicacionD = TipoSeguridadD = ClasificacionJuridicaD = EstatusD = ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                FecIngresoD = new DateTime?();
                SelectIngreso = null;
                SelectedInterno = null;
                ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #region Load


        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REALIZACION_LISTAS_ESTUDIOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = true;
                        if (p.EDITAR == 1)
                            PEditar = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = BuscarImputadoHabilitado = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        private async void CreacionListaPLoad(CreacionListasExamenPView obj)
        {
            try
            {
                LstBeneficios = new ObservableCollection<PERSONALIDAD_MOTIVO>(new cPersonalidadMotivo().GetData().OrderBy(x => x.DESCR));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstBeneficios.Insert(0, (new PERSONALIDAD_MOTIVO() { ID_MOTIVO = -1, DESCR = "SELECCIONE" }));
                }));

                if (_UltimoValor.HasValue && _UltimoValor.Value != -1)
                    IdMotivoE = _UltimoValor;
                else
                    IdMotivoE = -1;

                NoOficio = !string.IsNullOrEmpty(_DatoU) ? _DatoU : string.Empty;
                NombrePrograma = !string.IsNullOrEmpty(_DatoD) ? _DatoD : string.Empty;

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    //ConfiguraPermisos();
                    ValidacionesCreacionLista();
                    AnioD = FolioD = IngresosD = null;
                    PaternoD = MaternoD = NombreD = NoControlD = UbicacionD = ClasificacionJuridicaD = EstatusD = TipoSeguridadD = string.Empty;
                    FecIngresoD = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la creación de listas.", ex);
            }
        }

        private async void FichaJuridicaLoad(FichaIdentificacionJuridica Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    MenuReporteEnabled = true;
                    StaticSourcesViewModel.SourceChanged = true;
                });

                ValidacionesFichaJuridica();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la ficha jurídica", ex);
            }
        }

        private async void EstudiosTerminadosLoad(EstudiosTerminadosView Window = null)
        {
            try
            {
                MenuReporteEnabled = true;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    //ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                    CargaEstudiosTerminados();
                });
            }

            catch (Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los estudios terminados", exc);
            }
        }

        private void CargaEstudiosTerminados()
        {
            try
            {
                LstEstudiosTerminados = new ObservableCollection<PERSONALIDAD>();
                DateTime f1, f2;
                if (FechaInicioBusquedaDictamenFinal.HasValue)
                    f1 = FechaInicioBusquedaDictamenFinal.Value;
                else
                    f1 = Fechas.GetFechaDateServer;

                if (FechaFinBusquedaDictamenFinal.HasValue)
                    f2 = FechaFinBusquedaDictamenFinal.Value;
                else
                    f2 = Fechas.GetFechaDateServer;

                f1 = new DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new DateTime(f2.Year, f2.Month, f2.Day);
                LstEstudiosTerminados = new ObservableCollection<PERSONALIDAD>(new cEstudioPersonalidad().BuscarEstudiosDictamenFinal(f1, f2, NoOficioBusquedaDictamenFinal, GlobalVar.gCentro));
            }

            catch (Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los estudios terminados", exc);
            }
        }

        #endregion

        #region Populado de Datos
        private void ProcesaDatosImputadoSeleccionado()
        {
            try
            {
                if (SelectedSolicitud == null)
                    return;

                ProcesaDatosJuridicosSentencia();
                FechaUltimoExamen = null;
                ResolucionAprobado = ResolucionAplazado = string.Empty;
                var _ultimoPersonalidad = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectedSolicitud.ID_INGRESO && x.ID_IMPUTADO == SelectedSolicitud.ID_IMPUTADO);//sin importar el centro consulta si alguna vez se le hizo un estudio de personalidad
                if (_ultimoPersonalidad != null && _ultimoPersonalidad.Any())
                {
                    var UltimoP = _ultimoPersonalidad.OrderByDescending(o => o.ID_ESTUDIO).FirstOrDefault();
                    FechaUltimoExamen = UltimoP != null ? UltimoP.INICIO_FEC : new DateTime?();
                }

                FecIngresoFicha = FecNacimientoFicha = null;
                NombreFicha = AliasFicha = EdadFicha = EdoCivilFicha = LugarOrigenFicha = UbicacionFicha = OcupacionFicha = NacionalidadFicha = EscolaridadFicha = DomicilioFicha = DelitoFicha = DelitoFicha = string.Empty;
                ProcesosPendientes = CriminoDinamia = NoOficioEstudio = ClasifJuridFicha = DelitoFicha = ProcesosFicha = JuzgadoFicha = SentenciaFicha = APartirDeFicha = CausoEjecFicha = PorcentPenaCompur = ProcedenteDeFicha = string.Empty;
                if (SelectedSolicitud.INGRESO != null && SelectedSolicitud.INGRESO.IMPUTADO != null)
                {
                    var _FichaAnterior = new cFichasJuridicas().GetData(x => x.ID_INGRESO == SelectedSolicitud.ID_INGRESO && x.ID_IMPUTADO == SelectedSolicitud.ID_IMPUTADO && x.ID_ANIO == SelectedSolicitud.ID_ANIO && x.ID_CENTRO == SelectedSolicitud.ID_CENTRO).FirstOrDefault();

                    #region Info Basica
                    NombreFicha = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.IMPUTADO.NOMBRE) ? SelectedSolicitud.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.IMPUTADO.PATERNO) ? SelectedSolicitud.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.IMPUTADO.MATERNO) ? SelectedSolicitud.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty);

                    var _aliasByImputado = new cAlias().ObtenerTodosXImputado(SelectedSolicitud.ID_CENTRO, SelectedSolicitud.ID_ANIO, SelectedSolicitud.ID_IMPUTADO);
                    if (_aliasByImputado != null && _aliasByImputado.Any())
                        foreach (var item in _aliasByImputado)
                            AliasFicha += string.Format(" y/O {0} {1} {2}", !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Trim() : string.Empty);

                    EdadFicha = new Fechas().CalculaEdad(SelectedSolicitud.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString();
                    //EdoCivilFicha = SelectedSolicitud.INGRESO.IMPUTADO != null ? SelectedSolicitud.INGRESO.IMPUTADO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR) ? SelectedSolicitud.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                    EdoCivilFicha = SelectedSolicitud.INGRESO != null ? SelectedSolicitud.INGRESO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.ESTADO_CIVIL.DESCR) ? SelectedSolicitud.INGRESO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                    var _EstadoNac = new cEntidad().GetData(x => x.ID_ENTIDAD == SelectedSolicitud.INGRESO.IMPUTADO.NACIMIENTO_ESTADO).FirstOrDefault();
                    var _MunicipioNac = new cMunicipio().GetData(x => x.ID_MUNICIPIO == SelectedSolicitud.INGRESO.IMPUTADO.NACIMIENTO_MUNICIPIO && x.ID_ENTIDAD == SelectedSolicitud.INGRESO.IMPUTADO.NACIMIENTO_ESTADO).FirstOrDefault();
                    LugarOrigenFicha = string.Format("{0}, {1}", _EstadoNac != null ? !string.IsNullOrEmpty(_EstadoNac.DESCR) ? _EstadoNac.DESCR.Trim() : string.Empty : string.Empty, _MunicipioNac != null ? !string.IsNullOrEmpty(_MunicipioNac.MUNICIPIO1) ? _MunicipioNac.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                    FecNacimientoFicha = SelectedSolicitud.INGRESO.IMPUTADO.NACIMIENTO_FECHA;
                    if (SelectedSolicitud.INGRESO.CAMA != null)
                        UbicacionFicha = string.Format("{0}-{1}{2}-{3}",
                                                   SelectedSolicitud.INGRESO.CAMA.CELDA != null ? SelectedSolicitud.INGRESO.CAMA.CELDA.SECTOR != null ? SelectedSolicitud.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectedSolicitud.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                   SelectedSolicitud.INGRESO.CAMA.CELDA != null ? SelectedSolicitud.INGRESO.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.CAMA.CELDA.SECTOR.DESCR) ? SelectedSolicitud.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                                                   SelectedSolicitud.INGRESO.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.CAMA.CELDA.ID_CELDA) ? SelectedSolicitud.INGRESO.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty, SelectedSolicitud.INGRESO.ID_UB_CAMA);

                    //OcupacionFicha = SelectedSolicitud.INGRESO.IMPUTADO.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.IMPUTADO.OCUPACION.DESCR) ? SelectedSolicitud.INGRESO.IMPUTADO.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                    OcupacionFicha = SelectedSolicitud.INGRESO.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.OCUPACION.DESCR) ? SelectedSolicitud.INGRESO.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                    NacionalidadFicha = SelectedSolicitud.INGRESO.IMPUTADO.ID_NACIONALIDAD.HasValue ? !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD) ? SelectedSolicitud.INGRESO.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD.Trim() : string.Empty : string.Empty;
                    //EscolaridadFicha = SelectedSolicitud.INGRESO.IMPUTADO.ID_ESCOLARIDAD.HasValue ? !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.IMPUTADO.ESCOLARIDAD.DESCR) ? SelectedSolicitud.INGRESO.IMPUTADO.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                    EscolaridadFicha = SelectedSolicitud.INGRESO.ID_ESCOLARIDAD.HasValue ? !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.ESCOLARIDAD.DESCR) ? SelectedSolicitud.INGRESO.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                    //DomicilioFicha = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.IMPUTADO.DOMICILIO_CALLE) ? SelectedSolicitud.INGRESO.IMPUTADO.DOMICILIO_CALLE.Trim() : string.Empty,
                    //                                               SelectedSolicitud.INGRESO.IMPUTADO.DOMICILIO_NUM_EXT.HasValue ? string.Concat("No. ", SelectedSolicitud.INGRESO.IMPUTADO.DOMICILIO_NUM_EXT.Value.ToString()) : string.Empty,
                    //                                               SelectedSolicitud.INGRESO.IMPUTADO.DOMICILIO_CODIGO_POSTAL.HasValue ? SelectedSolicitud.INGRESO.IMPUTADO.DOMICILIO_CODIGO_POSTAL.Value.ToString() : string.Empty);
                    DomicilioFicha = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectedSolicitud.INGRESO.DOMICILIO_CALLE) ? SelectedSolicitud.INGRESO.DOMICILIO_CALLE.Trim() : string.Empty,
                                                                   SelectedSolicitud.INGRESO.DOMICILIO_NUM_EXT.HasValue ? string.Concat("No. ", SelectedSolicitud.INGRESO.DOMICILIO_NUM_EXT.Value.ToString()) : string.Empty,
                                                                   SelectedSolicitud.INGRESO.DOMICILIO_CP.HasValue ? SelectedSolicitud.INGRESO.DOMICILIO_CP.Value.ToString() : string.Empty);

                    var _delitos = new cCausaPenalDelito().Obtener(Convert.ToInt32(SelectedSolicitud.ID_CENTRO), Convert.ToInt32(SelectedSolicitud.ID_ANIO), Convert.ToInt32(SelectedSolicitud.ID_IMPUTADO), Convert.ToInt32(SelectedSolicitud.ID_INGRESO));
                    if (_delitos.Any())
                        foreach (var item in _delitos)
                        {
                            if (_delitos.Count > 1)
                                DelitoFicha += string.Format("{0} Y ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                            else
                                DelitoFicha += string.Format("{0} ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                        };

                    #region Causa Penal
                    if (SelectedSolicitud.INGRESO.CAUSA_PENAL != null && SelectedSolicitud.INGRESO.CAUSA_PENAL.Any())
                        foreach (var item in SelectedSolicitud.INGRESO.CAUSA_PENAL)
                        {
                            if (SelectedSolicitud.INGRESO.CAUSA_PENAL.Count > 1)
                            {
                                JuzgadoFicha += string.Format("{0} \n", item.CP_JUZGADO.HasValue ? !string.IsNullOrEmpty(item.JUZGADO.DESCR) ? item.JUZGADO.DESCR.Trim() : string.Empty : string.Empty);
                                ProcesosFicha += string.Format("{0} / {1} Y ", item.CP_ANIO.HasValue ? item.CP_ANIO.Value.ToString() : string.Empty, item.CP_FOLIO.HasValue ? item.CP_FOLIO.ToString() : string.Empty);
                            }
                            else
                            {
                                JuzgadoFicha += string.Format("{0} \n", item.CP_JUZGADO.HasValue ? !string.IsNullOrEmpty(item.JUZGADO.DESCR) ? item.JUZGADO.DESCR.Trim() : string.Empty : string.Empty);
                                ProcesosFicha += string.Format("{0} / {1}", item.CP_ANIO.HasValue ? item.CP_ANIO.Value.ToString() : string.Empty, item.CP_FOLIO.HasValue ? item.CP_FOLIO.ToString() : string.Empty);

                            }

                            if (item.SENTENCIA.Any())
                            {
                                var _ejecutoria = item.SENTENCIA.Any() ? item.SENTENCIA.FirstOrDefault(x => x.FEC_EJECUTORIA.HasValue) : null;
                                if (_ejecutoria != null)
                                    CausoEjecFicha = string.Format("{0} ", _ejecutoria.FEC_EJECUTORIA.HasValue ? Fechas.fechaLetra(_ejecutoria.FEC_EJECUTORIA.Value, false).ToUpper() : string.Empty);
                            };
                        };

                    SentenciaFicha = CalcularSentencia().ToUpper();
                    CalcularSentenciasPendiente();
                    APartirDeFicha = SelectedSolicitud.INGRESO.FEC_INGRESO_CERESO.HasValue ? Fechas.fechaLetra(SelectedSolicitud.INGRESO.FEC_INGRESO_CERESO.Value, false).ToUpper() : string.Empty;
                    FecIngresoFicha = SelectedSolicitud.INGRESO.FEC_INGRESO_CERESO;
                    #endregion
                    #endregion

                    if (_FichaAnterior != null)
                    {
                        ProcesosPendientes = _FichaAnterior.P3_PROCESOS_PENDIENTES;
                        FechaUltimoExamen = _FichaAnterior.P4_ULTIMO_EXAMEN_FEC;
                        ResolucionAprobado = _FichaAnterior.P5_RESOLUCION_APROBADO ?? string.Empty;
                        ResolucionAplazado = _FichaAnterior.P5_RESOLUCION_MAYORIA ?? string.Empty;
                        CriminoDinamia = _FichaAnterior.P6_CRIMINODINAMIA;
                        //TipoTramite = _FichaAnterior.TRAMITE_DIAGNOSTICO;
                        TramiteLibertadAntic = !string.IsNullOrEmpty(_FichaAnterior.P7_TRAMITE_LIBERTAD) ? true : false;
                        TramiteMod = !string.IsNullOrEmpty(_FichaAnterior.P7_TRAMITE_MODIFICACION) ? true : false;
                        TramiteTr = !string.IsNullOrEmpty(_FichaAnterior.P7_TRAMITE_TRASLADO) ? true : false;
                        TramiteDiagn = !string.IsNullOrEmpty(_FichaAnterior.TRAMITE_DIAGNOSTICO) ? true : false;
                        TramiteTraslVol = !string.IsNullOrEmpty(_FichaAnterior.TRAMITE_TRASLADO_VOLUNTARIO) ? true : false;
                        NoOficioEstudio = _FichaAnterior.OFICIO_ESTUDIO_SOLICITADO;
                        ClasifJuridFicha = _FichaAnterior.P2_CLAS_JURID;
                        DelitoFicha = _FichaAnterior.P2_DELITO;
                        ProcesosFicha = _FichaAnterior.P2_PROCESOS;
                        JuzgadoFicha = _FichaAnterior.P2_JUZGADOS;
                        SentenciaFicha = _FichaAnterior.P2_SENTENCIA;
                        APartirDeFicha = _FichaAnterior.P2_PARTIR;
                        CausoEjecFicha = _FichaAnterior.P2_EJECUTORIA;
                        PorcentPenaCompur = _FichaAnterior.P2_PENA_COMPURG;
                        ProcedenteDeFicha = _FichaAnterior.P2_PROCEDENTE;
                        FecIngresoFicha = _FichaAnterior.P2_FEC_INGRESO;
                    }

                    else
                    {
                        TramiteLibertadAntic = TramiteMod = TramiteTr = TramiteDiagn = TramiteTraslVol = false;
                        ProcesosPendientes = ResolucionAprobado = ResolucionAplazado = CriminoDinamia = NoOficioEstudio = ClasifJuridFicha = ProcedenteDeFicha = string.Empty;
                    }
                }

                //ValidacionesFichaJuridica();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void ProcesaDatosJuridicosSentencia()
        {
            try
            {
                var sentencia = new cSentencia().ObtenerSentenciaIngreso(
                 SelectedSolicitud.ID_CENTRO,
                 SelectedSolicitud.ID_ANIO,
                 SelectedSolicitud.ID_IMPUTADO,
                 SelectedSolicitud.ID_INGRESO,
                 Fechas.GetFechaDateServer);
                int AniosS = 0;
                int MesesS = 0;
                int DiasS = 0;
                int AniosC = 0;
                int MesesC = 0;
                int DiasC = -0;
                int dias = 0;
                int dias_cumplidos = 0;
                if (sentencia != null)
                {
                    foreach (var item in sentencia)
                    {
                        AniosS += item.S_ANIO;
                        MesesS += item.S_MES;
                        DiasS += item.S_DIA;
                        AniosC += item.C_ANIO;
                        MesesC += item.C_MES;
                        DiasC += item.C_DIA;
                        dias = ((item.S_ANIO * 365) + (item.S_MES * 30) + item.S_DIA);
                        dias_cumplidos = (item.C_ANIO * 365) + (item.C_MES * 30) + item.C_DIA;
                    };

                    //double perc = ((double)dias - (double)dias_cumplidos) / 100;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region Sentencia de Datos
        ///CALCULAR TIEMPOS DE SENTENCIA Y COMPURGACION
        private string CalcularSentencia()
        {
            try
            {
                LstSentenciasIngresos = new ObservableCollection<SentenciaIngreso>();
                if (SelectedSolicitud != null)
                {
                    if (SelectedSolicitud.INGRESO != null)
                    {
                        int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                        DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                        if (SelectedSolicitud.INGRESO.CAUSA_PENAL != null)
                        {
                            foreach (var cp in SelectedSolicitud.INGRESO.CAUSA_PENAL)
                            {
                                bool segundaInstancia = false, Incidente = false;
                                if (cp.SENTENCIA != null)
                                {
                                    if (cp.SENTENCIA.Count > 0)
                                    {

                                        #region Incidente
                                        if (cp.AMPARO_INCIDENTE != null)
                                        {
                                            var i = cp.AMPARO_INCIDENTE.Where(w => w.MODIFICA_PENA_ANIO != null && w.MODIFICA_PENA_MES != null && w.MODIFICA_PENA_DIA != null);
                                            if (i != null)
                                            {
                                                var res = i.OrderByDescending(w => w.ID_AMPARO_INCIDENTE).FirstOrDefault();// SingleOrDefault();
                                                if (res != null)
                                                {

                                                    anios = anios + (res.MODIFICA_PENA_ANIO != null ? res.MODIFICA_PENA_ANIO.Value : 0);
                                                    meses = meses + (res.MODIFICA_PENA_MES != null ? res.MODIFICA_PENA_MES.Value : 0);
                                                    dias = dias + (res.MODIFICA_PENA_DIA != null ? res.MODIFICA_PENA_DIA.Value : 0);

                                                    LstSentenciasIngresos.Add(
                                                    new SentenciaIngreso()
                                                    {
                                                        CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                        Fuero = cp.CP_FUERO,
                                                        SentenciaAnios = res.MODIFICA_PENA_ANIO != null ? res.MODIFICA_PENA_ANIO : 0,
                                                        SentenciaMeses = res.MODIFICA_PENA_MES != null ? res.MODIFICA_PENA_MES : 0,
                                                        SentenciaDias = res.MODIFICA_PENA_DIA != null ? res.MODIFICA_PENA_DIA : 0,
                                                        Instancia = "INCIDENCIA",
                                                        Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
                                                    });
                                                    Incidente = true;
                                                }
                                            }

                                            //ABONOS
                                            var dr = cp.AMPARO_INCIDENTE.Where(w => w.DIAS_REMISION != null);
                                            if (i != null)
                                            {
                                                foreach (var x in dr)
                                                {
                                                    //ABONO
                                                    dias_abono = dias_abono + (x.DIAS_REMISION != null ? (int)x.DIAS_REMISION : 0);
                                                }
                                            }
                                        }
                                        #endregion

                                        #region BUSCAMOS SI TIENE 2DA INSTANCIA
                                        if (cp.RECURSO.Count > 0)
                                        {
                                            var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
                                            if (r != null && Incidente == false)
                                            {
                                                var res = r.OrderByDescending(w => w.ID_RECURSO).FirstOrDefault();
                                                if (res != null)
                                                {
                                                    //SENTENCIA
                                                    anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
                                                    meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
                                                    dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);

                                                    LstSentenciasIngresos.Add(
                                                    new SentenciaIngreso()
                                                    {
                                                        CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                        Fuero = cp.CP_FUERO,
                                                        SentenciaAnios = res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS : 0,
                                                        SentenciaMeses = res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES : 0,
                                                        SentenciaDias = res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS : 0,
                                                        Instancia = "SEGUNDA INSTANCIA",
                                                        Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
                                                    });
                                                    segundaInstancia = true;
                                                }
                                            }
                                        }
                                        #endregion

                                        var s = cp.SENTENCIA.Where(w => w.ESTATUS == "A").FirstOrDefault();
                                        if (s != null)
                                        {
                                            if (FechaInicioCompurgacion == null)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                            else
                                                if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                    FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;

                                            //SENTENCIA
                                            if (!segundaInstancia && !Incidente)
                                            {
                                                anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
                                                meses = meses + (s.MESES != null ? s.MESES.Value : 0);
                                                dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);

                                                LstSentenciasIngresos.Add(
                                                new SentenciaIngreso()
                                                {
                                                    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                    Fuero = cp.CP_FUERO,
                                                    SentenciaAnios = s.ANIOS != null ? s.ANIOS : 0,
                                                    SentenciaMeses = s.MESES != null ? s.MESES : 0,
                                                    SentenciaDias = s.DIAS != null ? s.DIAS : 0,
                                                    Instancia = "PRIMERA INSTANCIA",
                                                    Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
                                                });
                                            }

                                            //ABONO
                                            anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
                                            meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
                                            dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
                                        }
                                    }
                                    else
                                    {
                                        LstSentenciasIngresos.Add(
                                        new SentenciaIngreso()
                                        {
                                            CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                            Fuero = cp.CP_FUERO,
                                            SentenciaAnios = null,
                                            SentenciaMeses = null,
                                            SentenciaDias = null
                                        });
                                    }
                                }
                                else
                                {
                                    LstSentenciasIngresos.Add(
                                    new SentenciaIngreso()
                                    {
                                        CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                        Fuero = cp.CP_FUERO,
                                        SentenciaAnios = null,
                                        SentenciaMeses = null,
                                        SentenciaDias = null
                                    });
                                }
                            }
                        }

                        while (dias > 29)
                        {
                            meses++;
                            dias = dias - 30;
                        }

                        while (meses > 11)
                        {
                            anios++;
                            meses = meses - 12;
                        }


                        if (FechaInicioCompurgacion != null)
                        {
                            FechaFinCompurgacion = FechaInicioCompurgacion.Value.Date;
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
                            //
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

                            int a = 0, m = 0, d = 0;
                            new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);

                            a = m = d = 0;
                        };

                        return anios + (anios == 1 ? " AÑO " : " AÑOS ") + meses + (meses == 1 ? " MES " : " MESES ") + dias + (dias == 1 ? " DÍA  " : " DÍAS ");
                    }

                    return string.Empty;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }

            return string.Empty;
        }

        private void CalcularSentenciasPendiente()
        {
            try
            {
                LstSentenciasIngresos = new ObservableCollection<SentenciaIngreso>();
                if (SelectedSolicitud != null)
                {
                    if (SelectedSolicitud.INGRESO != null)
                    {
                        int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                        DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                        if (SelectedSolicitud.INGRESO.CAUSA_PENAL != null)
                        {
                            foreach (var cp in SelectedSolicitud.INGRESO.CAUSA_PENAL)
                            {
                                bool segundaInstancia = false, Incidente = false;
                                if (cp.SENTENCIA != null)
                                {
                                    if (cp.SENTENCIA.Count > 0)
                                    {

                                        #region Incidente
                                        if (cp.AMPARO_INCIDENTE != null)
                                        {
                                            var i = cp.AMPARO_INCIDENTE.Where(w => w.MODIFICA_PENA_ANIO != null && w.MODIFICA_PENA_MES != null && w.MODIFICA_PENA_DIA != null);
                                            if (i != null)
                                            {
                                                var res = i.OrderByDescending(w => w.ID_AMPARO_INCIDENTE).FirstOrDefault();// SingleOrDefault();
                                                if (res != null)
                                                {

                                                    anios = anios + (res.MODIFICA_PENA_ANIO != null ? res.MODIFICA_PENA_ANIO.Value : 0);
                                                    meses = meses + (res.MODIFICA_PENA_MES != null ? res.MODIFICA_PENA_MES.Value : 0);
                                                    dias = dias + (res.MODIFICA_PENA_DIA != null ? res.MODIFICA_PENA_DIA.Value : 0);

                                                    LstSentenciasIngresos.Add(
                                                    new SentenciaIngreso()
                                                    {
                                                        CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                        Fuero = cp.CP_FUERO,
                                                        SentenciaAnios = res.MODIFICA_PENA_ANIO != null ? res.MODIFICA_PENA_ANIO : 0,
                                                        SentenciaMeses = res.MODIFICA_PENA_MES != null ? res.MODIFICA_PENA_MES : 0,
                                                        SentenciaDias = res.MODIFICA_PENA_DIA != null ? res.MODIFICA_PENA_DIA : 0,
                                                        Instancia = "INCIDENCIA",
                                                        Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
                                                    });
                                                    Incidente = true;
                                                }
                                            }

                                            //ABONOS
                                            var dr = cp.AMPARO_INCIDENTE.Where(w => w.DIAS_REMISION != null);
                                            if (i != null)
                                            {
                                                foreach (var x in dr)
                                                {
                                                    //ABONO
                                                    dias_abono = dias_abono + (x.DIAS_REMISION != null ? (int)x.DIAS_REMISION : 0);
                                                }
                                            }
                                        }
                                        #endregion

                                        #region BUSCAMOS SI TIENE 2DA INSTANCIA
                                        if (cp.RECURSO.Count > 0)
                                        {
                                            var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
                                            if (r != null && Incidente == false)
                                            {
                                                var res = r.OrderByDescending(w => w.ID_RECURSO).FirstOrDefault();
                                                if (res != null)
                                                {
                                                    //SENTENCIA
                                                    anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
                                                    meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
                                                    dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);

                                                    LstSentenciasIngresos.Add(
                                                    new SentenciaIngreso()
                                                    {
                                                        CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                        Fuero = cp.CP_FUERO,
                                                        SentenciaAnios = res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS : 0,
                                                        SentenciaMeses = res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES : 0,
                                                        SentenciaDias = res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS : 0,
                                                        Instancia = "SEGUNDA INSTANCIA",
                                                        Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
                                                    });
                                                    segundaInstancia = true;
                                                }
                                            }
                                        }
                                        #endregion

                                        var s = cp.SENTENCIA.FirstOrDefault(w => w.ESTATUS == "A");
                                        if (s != null)
                                        {
                                            if (FechaInicioCompurgacion == null)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                            else
                                                if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                    FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;

                                            //SENTENCIA
                                            if (!segundaInstancia && !Incidente)
                                            {
                                                anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
                                                meses = meses + (s.MESES != null ? s.MESES.Value : 0);
                                                dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);

                                                LstSentenciasIngresos.Add(
                                                new SentenciaIngreso()
                                                {
                                                    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                    Fuero = cp.CP_FUERO,
                                                    SentenciaAnios = s.ANIOS != null ? s.ANIOS : 0,
                                                    SentenciaMeses = s.MESES != null ? s.MESES : 0,
                                                    SentenciaDias = s.DIAS != null ? s.DIAS : 0,
                                                    Instancia = "PRIMERA INSTANCIA",
                                                    Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
                                                });
                                            }

                                            //ABONO
                                            anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
                                            meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
                                            dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
                                        }
                                    }
                                    else
                                    {
                                        LstSentenciasIngresos.Add(
                                        new SentenciaIngreso()
                                        {
                                            CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                            Fuero = cp.CP_FUERO,
                                            SentenciaAnios = null,
                                            SentenciaMeses = null,
                                            SentenciaDias = null
                                        });
                                    }
                                }
                                else
                                {
                                    LstSentenciasIngresos.Add(
                                    new SentenciaIngreso()
                                    {
                                        CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                        Fuero = cp.CP_FUERO,
                                        SentenciaAnios = null,
                                        SentenciaMeses = null,
                                        SentenciaDias = null
                                    });
                                }
                            }
                        }

                        while (dias > 29)
                        {
                            meses++;
                            dias = dias - 30;
                        }

                        while (meses > 11)
                        {
                            anios++;
                            meses = meses - 12;
                        }


                        if (FechaInicioCompurgacion != null)
                        {
                            FechaFinCompurgacion = FechaInicioCompurgacion.Value.Date;
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
                            //
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
                            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

                            int a = 0, m = 0, d = 0;
                            new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                            int _AniosCumplidoI = a;
                            int _MesesCumplidoI = m;
                            int _DiasCumplidoI = d;

                            //estos son los datos de las sentencias
                            var _Sentencia = LstSentenciasIngresos != null ? LstSentenciasIngresos.FirstOrDefault() : null;
                            int _AniosRestanteI = _Sentencia != null ? _Sentencia.SentenciaAnios ?? new int() : new int();
                            int _MesesRestanteI = _Sentencia != null ? _Sentencia.SentenciaMeses ?? new int() : new int();
                            int _DiasRestanteI = _Sentencia != null ? _Sentencia.SentenciaDias ?? new int() : new int();

                            if ((_AniosCumplidoI + _MesesCumplidoI + _DiasCumplidoI) != new int())
                                if ((_AniosRestanteI + _MesesRestanteI + _DiasRestanteI) != new int())
                                {
                                    decimal diasCumpli2 = (_AniosCumplidoI != decimal.Zero ? _AniosCumplidoI * 365 : 0) +
                                                          (_MesesCumplidoI != decimal.Zero ? _MesesCumplidoI * 30 : 0) +
                                                          _DiasCumplidoI;
                                    decimal diasRestantes2 = (_AniosRestanteI != decimal.Zero ? _AniosRestanteI * 365 : 0) +
                                                             (_MesesRestanteI != decimal.Zero ? _MesesRestanteI * 30 : 0) +
                                                             _DiasRestanteI; //sentencia
                                    decimal Result = (diasCumpli2 / diasRestantes2) * 100;
                                    PorcentPenaCompur = string.Format("{0:##.##} %", Result);
                                }
                                else
                                    PorcentPenaCompur = "0 %";
                        };
                    }

                    return;
                }

                return;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
        }

        #endregion


        private void ImprimirPartidaJuridica(PERSONALIDAD _dato)
        {
            if (_dato == null)
                return;

            var _CausasPenales = SelectedSolicitud.INGRESO.CAUSA_PENAL.Any() ? SelectedSolicitud.INGRESO.CAUSA_PENAL.FirstOrDefault(c => c.ID_ESTATUS_CP == 1) : null;//SOLO CAUSAS PENALES ACTIVAS
            if (_CausasPenales != null)
            {
                try
                {
                    var centro = new cCentro().Obtener(SelectedSolicitud.ID_CENTRO).FirstOrDefault();
                    SENTENCIA sentencia;
                    if (_CausasPenales.SENTENCIA == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "La causa penal no cuenta con una sentencia");
                        return;
                    }
                    else
                    {
                        sentencia = _CausasPenales.SENTENCIA.FirstOrDefault(w => w.ESTATUS == "A");
                        if (sentencia == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La causa penal no cuenta con una sentencia");
                            return;
                        }
                    }

                    var diccionario = new Dictionary<string, string>();
                    diccionario.Add("centro", _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.CENTRO != null ? !string.IsNullOrEmpty(_CausasPenales.INGRESO.CENTRO.DESCR) ? _CausasPenales.INGRESO.CENTRO.DESCR.Trim() : string.Empty : string.Empty : string.Empty);
                    diccionario.Add("expediente", string.Format("{0}/{1}", _CausasPenales.ID_ANIO, _CausasPenales.ID_IMPUTADO));
                    diccionario.Add("ciudad", _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.CENTRO != null ? _CausasPenales.INGRESO.CENTRO.MUNICIPIO != null ? !string.IsNullOrEmpty(_CausasPenales.INGRESO.CENTRO.MUNICIPIO.MUNICIPIO1) ? _CausasPenales.INGRESO.CENTRO.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty : string.Empty : string.Empty);
                    diccionario.Add("fecha_letra", Fechas.fechaLetra(Fechas.GetFechaDateServer, false));
                    diccionario.Add("dirigido", centro != null ? !string.IsNullOrEmpty(centro.DIRECTOR) ? centro.DIRECTOR.Trim() : string.Empty : string.Empty);
                    string nombres = string.Empty;
                    nombres = string.Format("{0} {1} {2}", _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_CausasPenales.INGRESO.IMPUTADO.NOMBRE) ? _CausasPenales.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                        _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_CausasPenales.INGRESO.IMPUTADO.PATERNO) ? _CausasPenales.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                        _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_CausasPenales.INGRESO.IMPUTADO.MATERNO) ? _CausasPenales.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty);

                    if (_CausasPenales.INGRESO != null)
                        if (_CausasPenales.INGRESO.IMPUTADO != null)
                            if (_CausasPenales.INGRESO.IMPUTADO.ALIAS != null && _CausasPenales.INGRESO.IMPUTADO.ALIAS.Any())
                                foreach (var a in _CausasPenales.INGRESO.IMPUTADO.ALIAS)
                                    nombres = nombres + string.Format("(O)\n{0} {1} {2}", !string.IsNullOrEmpty(a.NOMBRE) ? a.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(a.PATERNO) ? a.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.MATERNO) ? a.MATERNO.Trim() : string.Empty);

                    diccionario.Add("interno", nombres);
                    diccionario.Add("causa_penal", string.Format("{0}/{1}", _CausasPenales.CP_ANIO, _CausasPenales.CP_FOLIO));
                    diccionario.Add("juzgado", _CausasPenales.JUZGADO.DESCR.Trim());
                    string delitos = string.Empty;
                    if (_CausasPenales.CAUSA_PENAL_DELITO != null)
                        foreach (var d in _CausasPenales.CAUSA_PENAL_DELITO)
                        {
                            if (!string.IsNullOrEmpty(delitos))
                                delitos = delitos + ",";
                            delitos = delitos + d.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(d.MODALIDAD_DELITO.DELITO.DESCR) ? d.MODALIDAD_DELITO.DELITO.DESCR.Trim() : string.Empty : string.Empty;
                        }

                    diccionario.Add("delito", delitos);
                    diccionario.Add("fecha_traslado", " ");
                    diccionario.Add("cereso_procedencia", " ");
                    diccionario.Add("ingreso_origen", _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.FEC_INGRESO_CERESO.HasValue ? Fechas.fechaLetra(_CausasPenales.INGRESO.FEC_INGRESO_CERESO.Value, false) : string.Empty : string.Empty);

                    if (_CausasPenales.SENTENCIA != null)
                    {
                        if (sentencia != null)
                        {
                            string primera;
                            primera = sentencia.FEC_EJECUTORIA.HasValue ? Fechas.fechaLetra(sentencia.FEC_EJECUTORIA.Value, false) : string.Empty + ",";
                            if (sentencia.ANIOS != null && sentencia.ANIOS > 0)
                                primera = primera + string.Format(" {0} Años", sentencia.ANIOS);
                            if (sentencia.MESES != null && sentencia.MESES > 0)
                                primera = primera + string.Format(" {0} Meses", sentencia.MESES);
                            if (sentencia.DIAS != null && sentencia.DIAS > 0)
                                primera = primera + string.Format(" {0} Dias", sentencia.DIAS);
                            diccionario.Add("sentencia_primera_instancia", primera);
                            diccionario.Add("reparacion_danio", !string.IsNullOrEmpty(sentencia.REPARACION_DANIO) ? sentencia.REPARACION_DANIO.Trim() : " ");
                            //MULTAS
                            diccionario.Add("multa_primera", !string.IsNullOrEmpty(sentencia.MULTA) ? sentencia.MULTA.Trim() : " ");
                            diccionario.Add("reparacion_primera", !string.IsNullOrEmpty(sentencia.REPARACION_DANIO) ? sentencia.REPARACION_DANIO.Trim() : " ");
                            diccionario.Add("sustitucion_primera", !string.IsNullOrEmpty(sentencia.SUSTITUCION_PENA) ? sentencia.SUSTITUCION_PENA.Trim() : " ");
                            diccionario.Add("suspencion_primera", !string.IsNullOrEmpty(sentencia.SUSPENSION_CONDICIONAL) ? sentencia.SUSPENSION_CONDICIONAL.Trim() : " ");
                            //ABONOS
                            string abonos = string.Empty;
                            if (sentencia.ANIOS_ABONADOS != null && sentencia.ANIOS_ABONADOS > 0)
                                abonos = string.Format("{0} Años ", sentencia.ANIOS_ABONADOS);
                            if (sentencia.MESES_ABONADOS != null && sentencia.MESES_ABONADOS > 0)
                                abonos = abonos + string.Format("{0} Meses ", sentencia.MESES_ABONADOS);
                            if (sentencia.DIAS_ABONADOS != null && sentencia.DIAS_ABONADOS > 0)
                                abonos = abonos + string.Format("{0} Dias ", sentencia.DIAS_ABONADOS);
                            if (!string.IsNullOrEmpty(abonos))
                                diccionario.Add("abonos", abonos);
                            else
                                diccionario.Add("abonos", " ");
                        }
                        else
                        {
                            diccionario.Add("sentencia_primera_instancia", " ");
                            //MULTAS
                            diccionario.Add("multa_primera", " ");
                            diccionario.Add("reparacion_primera", " ");
                            diccionario.Add("sustitucion_primera", " ");
                            diccionario.Add("suspencion_primera", " ");
                            //ABONOS
                            diccionario.Add("abonos", " ");
                        }
                    }
                    else
                    {
                        diccionario.Add("sentencia_primera_instancia", " ");
                        //MULTAS
                        diccionario.Add("multa_primera", " ");
                        diccionario.Add("reparacion_primera", " ");
                        diccionario.Add("sustitucion_primera", " ");
                        diccionario.Add("suspencion_primera", " ");
                        //ABONOS
                        diccionario.Add("abonos", " ");
                    }


                    diccionario.Add("beneficio_ley", "No");

                    if (_CausasPenales.RECURSO != null)
                    {
                        var recurso = _CausasPenales.RECURSO.FirstOrDefault(w => w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2");
                        if (recurso != null)
                        {
                            string segunda;
                            segunda = recurso.FEC_RECURSO.HasValue ? Fechas.fechaLetra(recurso.FEC_RECURSO.Value, false) : string.Empty + ",";
                            if (recurso.SENTENCIA_ANIOS != null && recurso.SENTENCIA_ANIOS > 0)
                                segunda = segunda + string.Format(" {0} Años", recurso.SENTENCIA_ANIOS);
                            if (recurso.SENTENCIA_MESES != null && recurso.SENTENCIA_MESES > 0)
                                segunda = segunda + string.Format(" {0} Meses", recurso.SENTENCIA_MESES);
                            if (recurso.SENTENCIA_DIAS != null && recurso.SENTENCIA_DIAS > 0)
                                segunda = segunda + string.Format(" {0} Dias", recurso.SENTENCIA_DIAS);
                            diccionario.Add("sentencia_segunda_instancia", segunda);
                            //MULTA
                            diccionario.Add("multa_segunda", !string.IsNullOrEmpty(recurso.MULTA) ? recurso.MULTA.Trim() : " ");
                            diccionario.Add("reparacion_segunda", !string.IsNullOrEmpty(recurso.REPARACION_DANIO) ? recurso.REPARACION_DANIO.Trim() : " ");
                            diccionario.Add("sustitucion_segunda", !string.IsNullOrEmpty(recurso.SUSTITUCION_PENA) ? recurso.SUSTITUCION_PENA.Trim() : " ");
                            diccionario.Add("suspencion_segunda", !string.IsNullOrEmpty(recurso.MULTA_CONDICIONAL) ? recurso.MULTA_CONDICIONAL.Trim() : " ");
                        }
                        else
                        {
                            diccionario.Add("sentencia_segunda_instancia", " ");
                            diccionario.Add("multa_segunda", " ");
                            diccionario.Add("reparacion_segunda", " ");
                            diccionario.Add("sustitucion_segunda", " ");
                            diccionario.Add("suspencion_segunda", " ");
                        }
                    }
                    else
                    {
                        diccionario.Add("sentencia_segunda_instancia", " ");
                        diccionario.Add("multa_segunda", " ");
                        diccionario.Add("reparacion_segunda", " ");
                        diccionario.Add("sustitucion_segunda", " ");
                        diccionario.Add("suspencion_segunda", " ");
                    }

                    //INCIDENTE MODIFICA SENTENCIA
                    if (_CausasPenales.AMPARO_INCIDENTE != null)
                    {
                        var incidente = _CausasPenales.AMPARO_INCIDENTE.FirstOrDefault(w => w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M");
                        if (incidente != null)
                        {
                            string adecuacion = string.Empty;
                            if (incidente.MODIFICA_PENA_ANIO > 0)
                                adecuacion = string.Format("{0} Años ", incidente.MODIFICA_PENA_ANIO);
                            if (incidente.MODIFICA_PENA_MES > 0)
                                adecuacion = adecuacion + string.Format("{0} Meses ", incidente.MODIFICA_PENA_MES);
                            if (incidente.MODIFICA_PENA_DIA > 0)
                                adecuacion = adecuacion + string.Format("{0} Dias ", incidente.MODIFICA_PENA_DIA);

                            diccionario.Add("incidente_adecuacion_pena", adecuacion);
                        }
                        else
                            diccionario.Add("incidente_adecuacion_pena", " ");
                    }
                    else
                        diccionario.Add("incidente_adecuacion_pena", " ");

                    diccionario.Add("director_centro", !string.IsNullOrEmpty(centro.DIRECTOR) ? centro.DIRECTOR.Trim() : string.Empty);
                    diccionario.Add("elaboro", StaticSourcesViewModel.UsuarioLogin.Nombre);

                    var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.PARTIDA_JURIDICA); //File.ReadAllBytes(@"C:\libertades\PJ.doc");
                    if (documento == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No se encontró la plantilla del documento");
                        return;
                    }

                    var bytes = new cWord().FillFields(documento.DOCUMENTO, diccionario);
                    if (bytes == null)
                        return;

                    var tc = new TextControlView();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.editor.Loaded += (s, e) =>
                    {
                        try
                        {
                            switch (documento.ID_FORMATO)
                            {
                                case (int)enumFormatoDocumento.DOCX:
                                    tc.editor.Load(bytes, TXTextControl.BinaryStreamType.WordprocessingML);
                                    break;
                                case (int)enumFormatoDocumento.PDF:
                                    tc.editor.Load(bytes, TXTextControl.BinaryStreamType.AdobePDF);
                                    break;
                                case (int)enumFormatoDocumento.DOC:
                                    tc.editor.Load(bytes, TXTextControl.BinaryStreamType.MSWord);
                                    break;
                                default:
                                    new Dialogos().ConfirmacionDialogo("Validación", string.Format("El formato {0} del documento no es valido", documento.FORMATO_DOCUMENTO.DESCR));
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                        }
                    };

                    tc.Owner = PopUpsViewModels.MainWindow;
                    tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Show();
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir la partida jurídica.", ex);
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una causa penal.");
        }

        #region ACTA FEDERAL

        private void ActaConsejoTecnicoInterd(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    Delito = string.Empty;
                    var ActaAnterior = new cActaConsejoTecnicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_INGRESO == _ing.ID_INGRESO).FirstOrDefault();
                    NombreImputadoFF = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_ing.IMPUTADO.NOMBRE) ? _ing.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_ing.IMPUTADO.PATERNO) ? _ing.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(_ing.IMPUTADO.MATERNO) ? _ing.IMPUTADO.MATERNO.Trim() : string.Empty);
                    Sentencia = CalcularSentencia().ToUpper();
                    var _entidad = new cEntidad().GetData(x => x.ID_ENTIDAD == Parametro.ESTADO).FirstOrDefault();
                    if (_entidad != null)
                        EstadoActual = string.Format("{0}",
                            !string.IsNullOrEmpty(_entidad.DESCR) ? _entidad.DESCR.Trim() : string.Empty);
                    else
                        EstadoActual = string.Empty;

                    var CausaPenal = new cCausaPenal().GetData(x => x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO).FirstOrDefault();
                    var _estudioP = new cEstudioPersonalidad().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_ANIO == _ing.ID_ANIO & x.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (CausaPenal != null)
                    {
                        var _delitos = new cCausaPenalDelito().Obtener(Convert.ToInt32(_ing.ID_CENTRO), Convert.ToInt32(_ing.ID_ANIO), Convert.ToInt32(_ing.ID_IMPUTADO), Convert.ToInt32(_ing.ID_INGRESO));
                        if (_delitos.Any())
                            foreach (var item in _delitos)
                            {
                                if (_delitos.Count > 1)
                                    Delito += string.Format("{0} Y ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                                else
                                    Delito += string.Format("{0} ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                            };

                        var _Sentencia = new cSentencia().GetData(c => c.ID_INGRESO == _ing.ID_INGRESO && c.ID_IMPUTADO == _ing.ID_IMPUTADO && c.ID_CAUSA_PENAL == CausaPenal.ID_CAUSA_PENAL).FirstOrDefault();
                        if (_Sentencia != null)
                        {
                            var InicioComp = new cSentencia().GetData(x => x.ID_SENTENCIA == _Sentencia.ID_SENTENCIA && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_INGRESO == _ing.ID_INGRESO).FirstOrDefault();
                            if (InicioComp != null)
                                APartirDe = InicioComp.FEC_INICIO_COMPURGACION.HasValue ? InicioComp.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty;
                        }
                        else
                            APartirDe = string.Empty;
                    }

                    if (ActaAnterior == null)
                    {
                        LstAreasTec = new ObservableCollection<PFF_ACTA_DETERMINO>();
                        var _Centro = new cCentro().GetData(x => x.ID_CENTRO == (short)GlobalVar.gCentro).FirstOrDefault();
                        EnSesionDeFecha = Fechas.GetFechaDateServer;
                        ExpedienteImputadoFF = string.Format("{0} / {1}", _ing.IMPUTADO.ID_ANIO, _ing.IMPUTADO.ID_IMPUTADO);
                        LugarActa = LugarPiePagina();
                        IdCentro = GlobalVar.gCentro;
                        DirectorCentro = _Centro != null ? !string.IsNullOrEmpty(_Centro.DIRECTOR) ? _Centro.DIRECTOR.Trim() : string.Empty : string.Empty;
                        FechaActa = Fechas.GetFechaDateServer;
                        TramiteDescripcion = _estudioP != null ? _estudioP.ID_MOTIVO.HasValue ? !string.IsNullOrEmpty(_estudioP.PERSONALIDAD_MOTIVO.DESCR) ? _estudioP.PERSONALIDAD_MOTIVO.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        VotosR = ActuacionR = LugarActaFF = string.Empty;
                        LoadListasFederales();
                    }

                    else
                    {
                        EnSesionDeFecha = ActaAnterior.SESION_FEC;
                        ExpedienteImputadoFF = ActaAnterior.EXPEDIENTE;
                        IdCentro = ActaAnterior.CEN_ID_CENTRO;
                        DirectorCentro = ActaAnterior.DIRECTOR;
                        FechaActa = ActaAnterior.FECHA;
                        TramiteDescripcion = ActaAnterior.TRAMITE;
                        VotosR = ActaAnterior.APROBADO_POR;
                        LugarActa = ActaAnterior.LUGAR;
                        ActuacionR = ActaAnterior.APROBADO_APLAZADO;
                        LstAreasTec = new ObservableCollection<PFF_ACTA_DETERMINO>();
                        if (ActaAnterior.PFF_ACTA_DETERMINO != null && ActaAnterior.PFF_ACTA_DETERMINO.Any())
                            foreach (var item in ActaAnterior.PFF_ACTA_DETERMINO)
                                LstAreasTec.Add(item);

                        LoadListasFederales();
                    }

                    ValidacionesActaFederal();
                }
            }

            catch (Exception exc)
            {
                throw;
            }
        }

        private string LugarPiePagina()
        {
            try
            {
                string _LugarDefinitivo = string.Empty;
                string EstadoSist, EdoNuevo = string.Empty;
                var EstadoActual = new cEntidad().GetData(x => x.ID_ENTIDAD == Parametro.ESTADO).FirstOrDefault();
                var _Municipio = new cMunicipio().GetData(x => x.ID_MUNICIPIO == 2 && x.ID_ENTIDAD == 2).FirstOrDefault();
                EstadoSist = EstadoActual != null ? string.Concat(_Municipio != null ? !string.IsNullOrEmpty(_Municipio.MUNICIPIO1) ? _Municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty, " ", !string.IsNullOrEmpty(EstadoActual.DESCR) ? EstadoActual.DESCR.Trim() : string.Empty) : string.Empty;
                _LugarDefinitivo = string.Format("{0} A {1} ", EstadoSist, Fechas.GetFechaDateServer.ToString(" dd , MMMM , yyyy", System.Globalization.CultureInfo.CurrentCulture).ToUpper());
                if (!string.IsNullOrEmpty(_LugarDefinitivo))
                    EdoNuevo = _LugarDefinitivo.Replace(" , ", " DE ");
                else
                    EdoNuevo = _LugarDefinitivo;

                _LugarDefinitivo = EdoNuevo;
                return _LugarDefinitivo;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private bool GuardarActaFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                string _NombreDir = string.Empty;
                var NombreDirector = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                if (NombreDirector != null)
                    _NombreDir = string.Format("{0}", !string.IsNullOrEmpty(NombreDirector.DIRECTOR) ? NombreDirector.DIRECTOR.Trim() : string.Empty);

                var _Respuesta = new PFF_ACTA_CONSEJO_TECNICO();
                _Respuesta.ID_ANIO = _ing.ID_ANIO;
                _Respuesta.ID_CENTRO = _ing.ID_CENTRO;
                _Respuesta.ID_IMPUTADO = _ing.ID_IMPUTADO;
                _Respuesta.ID_INGRESO = _ing.ID_INGRESO;
                _Respuesta.APROBADO_APLAZADO = ActuacionR;
                _Respuesta.APROBADO_POR = VotosR;
                _Respuesta.CEN_ID_CENTRO = IdCentro;
                _Respuesta.DIRECTOR = DirectorCentro;
                _Respuesta.EXPEDIENTE = ExpedienteImputadoFF;
                _Respuesta.FECHA = FechaActa;
                _Respuesta.LUGAR = EstadoActual;
                _Respuesta.SUSCRITO_DIRECTOR_CRS = DirectorCentro;
                _Respuesta.TRAMITE = TramiteDescripcion;
                _Respuesta.DIRECTOR = _NombreDir;
                _Respuesta.SESION_FEC = EnSesionDeFecha;
                _Respuesta.LUGAR = LugarActa;
                _Respuesta.PFF_ACTA_DETERMINO.Clear();
                if (LstAreasTec != null && LstAreasTec.Any())
                    foreach (var item in LstAreasTec)
                        _Respuesta.PFF_ACTA_DETERMINO.Add(item);

                return new cActaFederal().GuardarActaFederal(_Respuesta, _IdEstudio);
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        private void LoadListasTraslados()
        {
            try
            {
                if (LstPeligrosidadTraslados == null)
                {
                    LstPeligrosidadTraslados = new ObservableCollection<PFC_V_PELIGROSIDAD>(new cPeligrosidad().ObtenerTodos(string.Empty));
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstPeligrosidadTraslados.Insert(0, new PFC_V_PELIGROSIDAD() { ID_PELIGROSIDAD = -1, DESCR = "SELECCIONE" });
                    }));
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private void LoadListasFederales()
        {
            try
            {
                if (LstOcupaciones == null)
                {
                    LstOcupaciones = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos(string.Empty));
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstOcupaciones.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                    }));
                }

                if (LstDialectos == null)
                {
                    LstDialectos = new ObservableCollection<DIALECTO>(new cDialecto().ObtenerTodos());
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstDialectos.Insert(0, new DIALECTO() { ID_DIALECTO = -1, DESCR = "SELECCIONE" });
                    }));
                }

                if (LstPaises == null)
                {
                    LstPaises = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos());
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstPaises.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
                    }));
                }

                if (LstEntidades == null)
                {
                    LstEntidades = new ObservableCollection<ENTIDAD>();
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                    }));
                }

                if (LstMunicipios == null)
                {
                    LstMunicipios = new ObservableCollection<MUNICIPIO>();
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    }));
                }

                if (LstCentros == null)
                {
                    LstCentros = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos(string.Empty));
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstCentros.Insert(0, (new CENTRO() { ID_CENTRO = -1, DESCR = "SELECCIONE" }));
                    }));
                }

                if (LstAreas == null)
                {
                    LstAreas = new ObservableCollection<AREA_TECNICA>(new cAreaTecnica().ObtenerTodo());
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstAreas.Insert(0, (new AREA_TECNICA() { ID_TECNICA = -1, DESCR = "SELECCIONE" }));
                    }));
                }

                if (LstEstadoCivil == null)
                {
                    LstEstadoCivil = new ObservableCollection<ESTADO_CIVIL>(new cEstadoCivil().ObtenerTodos(string.Empty));
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstEstadoCivil.Insert(0, (new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" }));
                    }));
                }

                if (LstParentescos == null)
                {
                    LstParentescos = new ObservableCollection<TIPO_REFERENCIA>(new cTipoReferencia().ObtenerTodos());
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstParentescos.Insert(0, (new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" }));
                    }));
                }

                if (ListEstadoCivil == null)
                {
                    ListEstadoCivil = new ObservableCollection<ESTADO_CIVIL>(new cEstadoCivil().ObtenerTodos());
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListEstadoCivil.Insert(0, (new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" }));
                    }));
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion
        #region ACTA DE CONSEJO TECNICO
        private void InicializaCamposActaConsejo(PERSONALIDAD _Estudio)
        {
            try
            {
                if (_Estudio == null)
                    return;

                var _ActaAnterior = new cActaConsejoTecnicoComun().GetData(x => x.ID_IMPUTADO == _Estudio.ID_IMPUTADO && x.ID_INGRESO == _Estudio.ID_INGRESO && x.ID_CENTRO == _Estudio.ID_CENTRO && x.ID_ESTUDIO == _Estudio.ID_ESTUDIO && x.ID_ANIO == _Estudio.ID_ANIO).FirstOrDefault();

                LugarActaComun = NombrePresidenteActaComun = NombreSecretarioActaComun = NombreJuridicoActaComun = NombreMedicoActaComun = NombrePsiccoActaComun = NombreCriminologiaActaComun = NombreTrabajoSocialActaComun = NombreEducativoActaComun = NombreAreaLaboralActaComun = NombreSeguridadActaComun =
                OpinionMedico = OpinionActaComun = OpinionPsico = OpinionCrimi = OpinionTrabSocial = OpinionEscolar = OpinionLaboral = OpinionSeguridad = NombreInternoActaComun = ManifestaronActaComun = ActuacionActaComun = AcuerdoActaComun = string.Empty;

                if (_ActaAnterior == null)
                {
                    var _CentroActual = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (_CentroActual != null)
                        NombrePresidenteActaComun = !string.IsNullOrEmpty(_CentroActual.DIRECTOR) ? _CentroActual.DIRECTOR.Trim() : string.Empty;

                    DEPARTAMENTO_ACCESO _SubDirector = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 23 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (_SubDirector != null)
                        NombreSecretarioActaComun = _SubDirector.USUARIO != null ? _SubDirector.USUARIO.EMPLEADO != null ? _SubDirector.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_SubDirector.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _SubDirector.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_SubDirector.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _SubDirector.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_SubDirector.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _SubDirector.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                    DEPARTAMENTO_ACCESO _CoordinadorJuridico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 30 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (_CoordinadorJuridico != null)
                        NombreJuridicoActaComun = _CoordinadorJuridico.USUARIO != null ? _CoordinadorJuridico.USUARIO.EMPLEADO != null ? _CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                    DEPARTAMENTO_ACCESO _CoordinadorMedico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 5 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (_CoordinadorMedico != null)
                        NombreMedicoActaComun = _CoordinadorMedico.USUARIO != null ? _CoordinadorMedico.USUARIO.EMPLEADO != null ? _CoordinadorMedico.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_CoordinadorMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _CoordinadorMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_CoordinadorMedico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _CoordinadorMedico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_CoordinadorMedico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _CoordinadorMedico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                    DEPARTAMENTO_ACCESO NombreCoordinadorPsicologico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 6 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (NombreCoordinadorPsicologico != null)
                        NombrePsiccoActaComun = NombreCoordinadorPsicologico.USUARIO != null ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO != null ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                    DEPARTAMENTO_ACCESO NombreCoordinadorCrimi = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 14 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (NombreCoordinadorCrimi != null)
                        NombreCriminologiaActaComun = NombreCoordinadorCrimi.USUARIO != null ? NombreCoordinadorCrimi.USUARIO.EMPLEADO != null ? NombreCoordinadorCrimi.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordinadorCrimi.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordinadorCrimi.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorCrimi.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordinadorCrimi.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorCrimi.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordinadorCrimi.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                    DEPARTAMENTO_ACCESO NombreCoordinadorTS = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 3 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (NombreCoordinadorTS != null)
                        NombreTrabajoSocialActaComun = NombreCoordinadorTS.USUARIO != null ? NombreCoordinadorTS.USUARIO.EMPLEADO != null ? NombreCoordinadorTS.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordinadorTS.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordinadorTS.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorTS.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordinadorTS.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorTS.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordinadorTS.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                    DEPARTAMENTO_ACCESO NombreCoordinadorEducaivo = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 13 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (NombreCoordinadorEducaivo != null)
                        NombreEducativoActaComun = NombreCoordinadorEducaivo.USUARIO != null ? NombreCoordinadorEducaivo.USUARIO.EMPLEADO != null ? NombreCoordinadorEducaivo.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordinadorEducaivo.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordinadorEducaivo.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorEducaivo.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordinadorEducaivo.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorEducaivo.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordinadorEducaivo.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                    DEPARTAMENTO_ACCESO NombreCoordProgramas = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 21 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (NombreCoordProgramas != null)
                        NombreAreaLaboralActaComun = NombreCoordProgramas.USUARIO != null ? NombreCoordProgramas.USUARIO.EMPLEADO != null ? NombreCoordProgramas.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordProgramas.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordProgramas.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordProgramas.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordProgramas.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordProgramas.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordProgramas.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;


                    DEPARTAMENTO_ACCESO NombreSeguridad = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 4 && x.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (NombreSeguridad != null)
                        NombreSeguridadActaComun = NombreSeguridad.USUARIO != null ? NombreSeguridad.USUARIO.EMPLEADO != null ? NombreSeguridad.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreSeguridad.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreSeguridad.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreSeguridad.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreSeguridad.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreSeguridad.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreSeguridad.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                    DateTime Fecha = Fechas.GetFechaDateServer;
                    LugarActaComun = string.Format("{0} ESTADO DE {1} SIENDO LAS {2} HORAS DEL DÍA {3}", _CentroActual.ID_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(_CentroActual.MUNICIPIO.MUNICIPIO1) ? _CentroActual.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                        _CentroActual.ID_MUNICIPIO.HasValue ? _CentroActual.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(_CentroActual.MUNICIPIO.ENTIDAD.DESCR) ? _CentroActual.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                        Fecha.ToString("HH:mm"), Fechas.fechaLetra(Fecha, false)).ToUpper();

                    NombreInternoActaComun = _Estudio.INGRESO != null ? _Estudio.INGRESO.IMPUTADO != null ?
                        string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_Estudio.INGRESO.IMPUTADO.NOMBRE) ? _Estudio.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_Estudio.INGRESO.IMPUTADO.PATERNO) ? _Estudio.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_Estudio.INGRESO.IMPUTADO.MATERNO) ? _Estudio.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty;

                    var _aliasImputado = new cAlias().ObtenerTodosXImputado(_Estudio.ID_CENTRO, _Estudio.ID_ANIO, _Estudio.ID_IMPUTADO);
                    if (_aliasImputado != null && _aliasImputado.Any())
                        foreach (var item2 in _aliasImputado)
                            NombreInternoActaComun += string.Format(" y/o {0}", string.Concat(!string.IsNullOrEmpty(item2.NOMBRE) ? item2.NOMBRE.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.PATERNO) ? item2.PATERNO.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.MATERNO) ? item2.MATERNO.Trim() : string.Empty));
                }

                else
                {
                    LugarActaComun = _ActaAnterior.LUGAR;
                    NombrePresidenteActaComun = _ActaAnterior.PRESIDENTE;
                    NombreSecretarioActaComun = _ActaAnterior.SECRETARIO;
                    NombreJuridicoActaComun = _ActaAnterior.JURIDICO;
                    NombreMedicoActaComun = _ActaAnterior.MEDICO;
                    NombrePsiccoActaComun = _ActaAnterior.PSICOLOGIA;
                    NombreCriminologiaActaComun = _ActaAnterior.CRIMINOLOGIA;
                    NombreTrabajoSocialActaComun = _ActaAnterior.TRABAJO_SOCIAL;
                    NombreEducativoActaComun = _ActaAnterior.EDUCATIVO;
                    NombreAreaLaboralActaComun = _ActaAnterior.AREA_LABORAL;
                    NombreSeguridadActaComun = _ActaAnterior.SEGURIDAD_CUSTODIA;
                    OpinionMedico = _ActaAnterior.OPINION_MEDICO;
                    OpinionPsico = _ActaAnterior.OPINION_PSICOLOGICA;
                    OpinionCrimi = _ActaAnterior.OPINION_CRIMINOLOGIA;
                    OpinionTrabSocial = _ActaAnterior.OPINION_TRABAJO_SOCIAL;
                    OpinionEscolar = _ActaAnterior.OPINION_ESCOLAR;
                    OpinionLaboral = _ActaAnterior.OPINION_LABORAL;
                    OpinionSeguridad = _ActaAnterior.OPINION_SEGURIDAD;
                    NombreInternoActaComun = _ActaAnterior.INTERNO;
                    ManifestaronActaComun = _ActaAnterior.MANIFESTARON;
                    ActuacionActaComun = _ActaAnterior.ACTUACION;
                    AcuerdoActaComun = _ActaAnterior.ACUERDO;
                    OpinionActaComun = _ActaAnterior.OPINION;
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void MuestraFormatoActaConsejoTecnicoComun(PERSONALIDAD _Entity)
        {
            try
            {
                if (_Entity == null)
                    return;

                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                cActaConsejoTecnicoComunReporte DatosReporte = new cActaConsejoTecnicoComunReporte();
                var _datos = new cActaConsejoTecnicoComun().GetData(x => x.ID_IMPUTADO == _Entity.ID_IMPUTADO && x.ID_INGRESO == _Entity.ID_INGRESO && x.ID_ESTUDIO == _Entity.ID_ESTUDIO && x.ID_CENTRO == _Entity.ID_CENTRO).FirstOrDefault();
                var _centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                string _alia = string.Empty;
                var _aliasByImputado = new cAlias().ObtenerTodosXImputado(_Entity.ID_CENTRO, _Entity.ID_ANIO, _Entity.ID_IMPUTADO);
                if (_aliasByImputado != null && _aliasByImputado.Any())
                    foreach (var item in _aliasByImputado)
                        _alia += string.Format(" y/O {0} {1} {2}", !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Trim() : string.Empty);

                if (_datos != null)
                {
                    string OpinionCompuesta = string.Empty;
                    if (_datos.OPINION == "S")
                        OpinionCompuesta = string.Format("OBTENIENDO ASÍ UNA OPINIÓN FAVORABLE DE REINSERCIÓN SOCIAL, POR LO QUE ES CONVENIENTE RECOMENDARLO PARA EL OTORGAMIENTO DE ALGÚN BENEFICIO DE LIBERTAD ANTICIPADA. \n POR LO ANTERIOR, EL SUB DIRECTOR JURÍDICO, EN SU CALIDAD DE SECRETARIO DEL CONSEJO TÉCNICO INTERDISCIPLINARIO, HACE CONSTAR QUE LA PRESENTE ACTUACIÓN FUE DE APROBAR Y EN EL MISMO ACTO GIRA SUS ÓRDENES PARA QUE SE REALICEN LOS TRÁMITES CORRESPONDIENTES PARA QUE EL PRESENTE ACUERDO SE REMITA A LA DIRECCIÓN DE EJECUCIÓN DE PENAS Y MEDIDAS JUDICIALES, ASÍ COMO LOS ESTUDIOS TÉCNICOS DE PERSONALIDAD, PARTIDA INTEGRA DE ANTECEDENTES PENALES DEL INTERNO {0} CON NÚMERO DE EXPEDIENTE {1}",
                           string.Format("{0} {1}", _datos.INTERNO, _alia), string.Format("{0} / {1}", _datos.ID_ANIO, _datos.ID_IMPUTADO));
                    else
                        OpinionCompuesta = string.Format("OBTENIENDO ASÍ UNA OPINIÓN FAVORABLE POR MAYORÍA DE LOS INTEGRANTES DE ESTE ÓRGANO COLEGIADO, YA QUE DENTRO DEL ABANICO DE OPORTUNIDADES QUE EN RECLUSIÓN SE OFRECE A LOS INTERNOS CONFORME AL MANDATO CONSTITUCIONAL, EL SENTENCIADO HA PARTICIPADO ACTIVA Y POSITIVAMENTE EN LOS DIVERSOS PROGRAMAS DE REINSERCIÓN SOCIAL ASÍ MISMO JURÍDICAMENTE ESTÁ EN LOS PORCENTAJES DE TIEMPO QUE MARCAN LAS LEYES RESPECTIVAS PARA QUE UN SENTENCIADO PUEDA TENER ACCESO A SOLICITAR UN BENEFICIO DE LIBERTAD ANTICIPADA, AUNADO A LO ANTERIOR CUENTA CON DÍAS LABORADOS QUE SOPORTE LO ANTERIOR, POR LO QUE ESTE ÓRGANO COLEGIADO CONSIDERA Y DETERMINA POR MAYORÍA DE LAS ÁREAS RECOMENDARLO PARA EL OTORGAMIENTO DE ALGÚN BENEFICIO DE LIBERTAD ANTICIPADA CORRESPONDIENTE. \n POR LO ANTERIOR, EL SUB DIRECTOR JURÍDICO, EN SU CALIDAD DE SECRETARIO DEL CONSEJO TÉCNICO INTERDISCIPLINARIO, HACE CONSTAR QUE LA PRESENTE ACTUACIÓN FUE DE APROBAR Y EN EL MISMO ACTO GIRA SUS ÓRDENES PARA QUE SE REALICEN LOS TRÁMITES CORRESPONDIENTES PARA QUE EL PRESENTE ACUERDO SE REMITA A LA DIRECCIÓN DE EJECUCIÓN DE PENAS Y MEDIDAS JUDICIALES, ASÍ COMO LOS ESTUDIOS TÉCNICOS DE PERSONALIDAD, PARTIDA INTEGRA DE ANTECEDENTES PENALES DEL INTERNO {0} CON NÚMERO DE EXPEDIENTE {1}",
                           string.Format("{0} {1}", _datos.INTERNO, _alia), string.Format("{0} / {1}", _datos.ID_ANIO, _datos.ID_IMPUTADO));

                    DatosReporte = new cActaConsejoTecnicoComunReporte()
                    {
                        Lugar = _datos.LUGAR,
                        Interno = string.Format("{0} {1}", _datos.INTERNO, _alia),
                        Presidente = _datos.PRESIDENTE,
                        Secretario = _datos.SECRETARIO,
                        Juridico = _datos.JURIDICO,
                        Medico = _datos.MEDICO,
                        Psicologia = _datos.PSICOLOGIA,
                        Criminologia = _datos.CRIMINOLOGIA,
                        TrabajoSocial = _datos.TRABAJO_SOCIAL,
                        Educativo = _datos.EDUCATIVO,
                        Laboral = _datos.AREA_LABORAL,
                        SeguridadCustodia = _datos.SEGURIDAD_CUSTODIA,
                        Actuacion = _datos.ACTUACION,
                        Acuerdo = _datos.ACUERDO,
                        OpinionMedico = _datos.OPINION_MEDICO == "S" ? "APROBADO" : "APLAZADO",
                        OpinionPsicologia = _datos.OPINION_PSICOLOGICA == "S" ? "APROBADO" : "APLAZADO",
                        OpinionTrabajoSocial = _datos.OPINION_TRABAJO_SOCIAL == "S" ? "APROBADO" : "APLAZADO",
                        OpinionSeguridad = _datos.OPINION_SEGURIDAD == "S" ? "APROBADO" : "APLAZADO",
                        OpinionLaboral = _datos.OPINION_LABORAL == "S" ? "APROBADO" : "APLAZADO",
                        OpinionEscolar = _datos.OPINION_ESCOLAR == "S" ? "APROBADO" : "APLAZADO",
                        OpinionCriminologia = _datos.OPINION_CRIMINOLOGIA == "S" ? "APROBADO" : "APLAZADO",
                        Manifestaron = _datos.MANIFESTARON,
                        Opinion = OpinionCompuesta
                    };
                };


                cEncabezado Encabezado = new cEncabezado
                {
                    TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA",
                    TituloDos = Parametro.ENCABEZADO2,
                    ImagenIzquierda = Parametro.LOGO_BC_ACTA_COMUN,
                    NombreReporte = Parametro.ENCABEZADO_FUERO_COMUN
                };

                #region Inicializacion de reporte
                View.Report.LocalReport.ReportPath = "Reportes/rActaConsejoTecnicoFueroComun.rdlc";
                View.Report.LocalReport.DataSources.Clear();
                #endregion

                #region Definicion d origenes de datos
                var ds1 = new List<cActaConsejoTecnicoComunReporte>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(DatosReporte);
                rds1.Name = "DataSet2";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                //datasource dos
                var ds2 = new List<cEncabezado>();
                ds2.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource
                {
                    Name = "DataSet1",
                    Value = ds2
                };
                View.Report.LocalReport.DataSources.Add(rds2);

                #endregion

                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion


        #region Archivero
        private void InicializaLista(short? Actual)
        {
            try
            {
                if (Actual.HasValue)
                {
                    LstDocumentos = new ObservableCollection<Archivero>();
                    switch (Actual)
                    {
                        case (short)eSituacionActual.STAGE0:
                            if (SelectedSolicitud != null)
                            {
                                LstDocumentos.Add(new Archivero() { Disponible = "SI", NombreArchivo = "PARTIDA JURÍDICA", TipoArchivo = (short)eDocumentoMostrado.PARTIDA_JURIDICA, VisibleVerDocumentoArchivero = true });
                                LstDocumentos.Add(new Archivero() { Disponible = "SI", NombreArchivo = "FICHA SIGNALETICA", TipoArchivo = (short)eDocumentoMostrado.FICHA_SIGNALETICA, VisibleVerDocumentoArchivero = true });

                                var _validaFuero = new cCausaPenal().GetData(x => x.ID_IMPUTADO == SelectedSolicitud.ID_IMPUTADO && x.ID_INGRESO == SelectedSolicitud.ID_INGRESO && x.ID_ANIO == SelectedSolicitud.ID_ANIO && x.ID_CENTRO == SelectedSolicitud.ID_CENTRO);

                                if (_validaFuero.Any())
                                    if (_validaFuero.FirstOrDefault(x => x.ID_ESTATUS_CP == 1 && x.CP_FUERO == "F") != null)
                                        return;

                                var _tieneFicha = new cFichasJuridicas().GetData(c => c.ID_IMPUTADO == SelectedSolicitud.ID_IMPUTADO && c.ID_INGRESO == SelectedSolicitud.ID_INGRESO && c.ID_ANIO == SelectedSolicitud.ID_ANIO && c.ID_CENTRO == SelectedSolicitud.ID_CENTRO).FirstOrDefault();

                                LstDocumentos.Add(new Archivero() { Disponible = _tieneFicha != null ? "SI" : "NO", NombreArchivo = "FICHA DE IDENTIFICACIÓN JURÍDICA", TipoArchivo = (short)eDocumentoMostrado.FICHA_JURIDICA, VisibleVerDocumentoArchivero = _tieneFicha != null ? true : false });
                            }

                            break;

                        case (short)eSituacionActual.STAGE1:
                            //NO CASE
                            break;

                        case (short)eSituacionActual.STAGE2:
                            //NO CASE
                            break;

                        case (short)eSituacionActual.STAGE3:
                            if (SelectedEstudioTerminado != null)
                            {
                                SelectedSolicitud = SelectedEstudioTerminado;
                                LstDocumentos.Add(new Archivero() { Disponible = "SI", NombreArchivo = "PARTIDA JURÍDICA", TipoArchivo = (short)eDocumentoMostrado.PARTIDA_JURIDICA, VisibleVerDocumentoArchivero = true });
                                LstDocumentos.Add(new Archivero() { Disponible = "SI", NombreArchivo = "FICHA SIGNALETICA", TipoArchivo = (short)eDocumentoMostrado.FICHA_SIGNALETICA, VisibleVerDocumentoArchivero = true });

                                ObservableCollection<PERSONALIDAD> lstDummy = new ObservableCollection<PERSONALIDAD>();
                                //MuestraOficioEnvio
                                short _IdFuero = 0;
                                var _EstudioPersonalidad = new cEstudioPersonalidad().GetData(c => c.ID_ESTUDIO == SelectedEstudioTerminado.ID_ESTUDIO && c.ID_IMPUTADO == SelectedEstudioTerminado.ID_IMPUTADO && c.ID_INGRESO == SelectedEstudioTerminado.ID_INGRESO && c.ID_ANIO == SelectedEstudioTerminado.ID_ANIO && c.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).FirstOrDefault();

                                if (_EstudioPersonalidad != null)
                                {
                                    if (_EstudioPersonalidad.INGRESO != null)
                                        if (_EstudioPersonalidad.INGRESO.CAUSA_PENAL != null && _EstudioPersonalidad.INGRESO.CAUSA_PENAL.Any())
                                        {
                                            ///AISLADOS DOCUMENTOS RELACIONADOS ALA REMISION DEL PROGRAMA Y JURIDICOS INDEPENDIENTES AL DESARROLLO DE LOS ESTUDIOS
                                            var _detallesCausaPenal = _EstudioPersonalidad.INGRESO.CAUSA_PENAL.FirstOrDefault(c => c.ID_ESTATUS_CP == 1 && c.CP_FUERO == "C");
                                            if (_detallesCausaPenal != null)
                                            {
                                                var _tieneFicha = new cFichasJuridicas().GetData(c => c.ID_IMPUTADO == SelectedEstudioTerminado.ID_IMPUTADO && c.ID_INGRESO == SelectedEstudioTerminado.ID_INGRESO && c.ID_ANIO == SelectedEstudioTerminado.ID_ANIO && c.ID_CENTRO == SelectedEstudioTerminado.ID_CENTRO).FirstOrDefault();

                                                LstDocumentos.Add(new Archivero() { Disponible = _tieneFicha != null ? "SI" : "NO", NombreArchivo = "FICHA DE IDENTIFICACIÓN JURÍDICA", TipoArchivo = (short)eDocumentoMostrado.FICHA_JURIDICA, VisibleVerDocumentoArchivero = _tieneFicha != null ? true : false });

                                                var CondensadoEstudiosProgramadosGeneral = new cEstudioPersonalidad().GetData(x => x.NUM_OFICIO.Trim() == _EstudioPersonalidad.NUM_OFICIO.Trim() && x.PROG_NOMBRE.Trim() == _EstudioPersonalidad.PROG_NOMBRE.Trim() && x.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro);
                                                if (CondensadoEstudiosProgramadosGeneral != null && CondensadoEstudiosProgramadosGeneral.Any())
                                                    foreach (var item in CondensadoEstudiosProgramadosGeneral)
                                                        lstDummy.Add(item);

                                                LstDocumentos.Add(new Archivero() { Disponible = lstDummy.Any() ? "SI" : "NO", NombreArchivo = "FORMATO DE PETICIÓN DE REALIZACIÓN DE ESTUDIOS DE PERSONALIDAD", TipoArchivo = (short)eDocumentoMostrado.OFICIO_PETICION_REALIZACION_ESTUDIOS_PERSONALIDAD, VisibleVerDocumentoArchivero = lstDummy.Any() ? true : false });

                                                LstDocumentos.Add(new Archivero() { Disponible = lstDummy.Any() ? "SI" : "NO", NombreArchivo = "FORMATO DE REMISIÓN DE CIERRE DE ESTUDIOS DE PERSONALIDAD", TipoArchivo = (short)eDocumentoMostrado.REMISION_CIERRE, VisibleVerDocumentoArchivero = lstDummy.Any() ? true : false });

                                                var detallesBrigada = new cEstudioPersonalidad().GetData(x => x.NUM_OFICIO.Trim() == SelectedEstudioTerminado.NUM_OFICIO && x.SOLICITUD_FEC == SelectedEstudioTerminado.SOLICITUD_FEC && x.PROG_NOMBRE.Trim() == SelectedEstudioTerminado.PROG_NOMBRE && x.ID_SITUACION != 4);//consulta los estudios que fueron hechos con respecto a esta brigada

                                                if (detallesBrigada.Any())
                                                {
                                                    bool _procede = false;
                                                    if (detallesBrigada.Any(x => x.ACTA_CONSEJO_TECNICO == null))
                                                        _procede = false;
                                                    else
                                                        _procede = true;

                                                    LstDocumentos.Add(new Archivero() { Disponible = _procede ? "SI" : "NO", NombreArchivo = "FORMATO DE REMISIÓN DE CIERRE DE ESTUDIOS DE PERSONALIDAD DEPMJ", TipoArchivo = (short)eDocumentoMostrado.REMISION_DPMJ, VisibleVerDocumentoArchivero = _procede ? true : false });
                                                }
                                                else
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "FORMATO DE REMISIÓN DE CIERRE DE ESTUDIOS DE PERSONALIDAD DEPMJ", TipoArchivo = (short)eDocumentoMostrado.REMISION_DPMJ, VisibleVerDocumentoArchivero = false });

                                                var _Comunes = new cEstudioPersonalidadFueroComun().GetData(x => x.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO && x.ID_INGRESO == _EstudioPersonalidad.ID_INGRESO && x.ID_IMPUTADO == _EstudioPersonalidad.ID_IMPUTADO && x.PERSONALIDAD.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                                                if (_Comunes != null)
                                                {
                                                    _IdFuero = 1;

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_II_MEDICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO MÉDICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_MEDICO, VisibleVerDocumentoArchivero = _Comunes.PFC_II_MEDICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_III_PSIQUIATRICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO PSIQUIÁTRICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSIQ, VisibleVerDocumentoArchivero = _Comunes.PFC_III_PSIQUIATRICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_IV_PSICOLOGICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO PSICOLÓGICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSICO, VisibleVerDocumentoArchivero = _Comunes.PFC_IV_PSICOLOGICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_IX_SEGURIDAD != null ? "SI" : "NO", NombreArchivo = "INFORME DEL ÁREA DE SEGURIDAD Y CUSTODIA DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SEGURIDAD, VisibleVerDocumentoArchivero = _Comunes.PFC_IX_SEGURIDAD != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_V_CRIMINODIAGNOSTICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO CRIMINODIAGNÓSTICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CRIMI, VisibleVerDocumentoArchivero = _Comunes.PFC_V_CRIMINODIAGNOSTICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_VI_SOCIO_FAMILIAR != null ? "SI" : "NO", NombreArchivo = "ESTUDIO SOCIO-FAMILIAR DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SOCIO_FAM, VisibleVerDocumentoArchivero = _Comunes.PFC_VI_SOCIO_FAMILIAR != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_VII_EDUCATIVO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO EDUCATIVO, CULTURAL Y DEPORTIVO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_EDUC, VisibleVerDocumentoArchivero = _Comunes.PFC_VII_EDUCATIVO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_VIII_TRABAJO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO SOBRE CAPACITACIÓN Y TRABAJO PENITENCIARIO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CAPAC, VisibleVerDocumentoArchivero = _Comunes.PFC_VIII_TRABAJO != null ? true : false });
                                                }
                                                else
                                                {
                                                    _IdFuero = 1;

                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO MÉDICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_MEDICO, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO PSIQUIÁTRICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSIQ, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO PSICOLÓGICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSICO, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "INFORME DEL ÁREA DE SEGURIDAD Y CUSTODIA DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SEGURIDAD, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO CRIMINODIAGNÓSTICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CRIMI, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO SOCIO-FAMILIAR DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SOCIO_FAM, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO EDUCATIVO, CULTURAL Y DEPORTIVO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_EDUC, VisibleVerDocumentoArchivero = false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO SOBRE CAPACITACIÓN Y TRABAJO PENITENCIARIO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CAPAC, VisibleVerDocumentoArchivero = false });
                                                }
                                            }
                                            else
                                            {
                                                var _Federales = new cPersonalidadFueroFederal().GetData(x => x.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO && x.ID_INGRESO == _EstudioPersonalidad.ID_INGRESO && x.ID_IMPUTADO == _EstudioPersonalidad.ID_IMPUTADO && x.PERSONALIDAD.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                                                if (_Federales != null)
                                                {
                                                    _IdFuero = 2;
                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_ACTA_CONSEJO_TECNICO != null ? "SI" : "NO", NombreArchivo = "ACTA DE CONSEJO TÉCNICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.ACTA_FEDERAL, VisibleVerDocumentoArchivero = _Federales.PFF_ACTA_CONSEJO_TECNICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_ACTIVIDAD != null ? "SI" : "NO", NombreArchivo = "INFORME DE LAS ACTIVIDADES EDUCATIVAS, CULTURALES, DEPORTIVAS, RECREATIVAS Y CÍVICAS DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_EDUCATIVAS, VisibleVerDocumentoArchivero = _Federales.PFF_ACTIVIDAD != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_CAPACITACION != null ? "SI" : "NO", NombreArchivo = "INFORME DE LAS ACTIVIDADES PRODUCTIVAS DE CAPACITACIÓN DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CAPAC, VisibleVerDocumentoArchivero = _Federales.PFF_CAPACITACION != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_CRIMINOLOGICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO CRIMINOLÓGICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CRIMINOLOGICO, VisibleVerDocumentoArchivero = _Federales.PFF_CRIMINOLOGICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_ESTUDIO_MEDICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO MÉDICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_MEDICO, VisibleVerDocumentoArchivero = _Federales.PFF_ESTUDIO_MEDICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_ESTUDIO_PSICOLOGICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO PSICOLÓGICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_PSICO, VisibleVerDocumentoArchivero = _Federales.PFF_ESTUDIO_PSICOLOGICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_TRABAJO_SOCIAL != null ? "SI" : "NO", NombreArchivo = "ESTUDIO DE TRABAJO SOCIAL DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_TRABAJO_SOCIAL, VisibleVerDocumentoArchivero = _Federales.PFF_TRABAJO_SOCIAL != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_VIGILANCIA != null ? "SI" : "NO", NombreArchivo = "INFORME DE LA SECCIÓN DE VIGILANCIA DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_VIGILANCIA, VisibleVerDocumentoArchivero = _Federales.PFF_VIGILANCIA != null ? true : false });
                                                }
                                                else
                                                {
                                                    if (_IdFuero != 1)
                                                    {
                                                        _IdFuero = 2;
                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ACTA DE CONSEJO TÉCNICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.ACTA_FEDERAL, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "INFORME DE LAS ACTIVIDADES EDUCATIVAS, CULTURALES, DEPORTIVAS, RECREATIVAS Y CÍVICAS DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_EDUCATIVAS, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "INFORME DE LAS ACTIVIDADES PRODUCTIVAS DE CAPACITACIÓN DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CAPAC, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO CRIMINOLÓGICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CRIMINOLOGICO, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO MÉDICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_MEDICO, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO PSICOLÓGICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_PSICO, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO DE TRABAJO SOCIAL DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_TRABAJO_SOCIAL, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "INFORME DE LA SECCIÓN DE VIGILANCIA DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_VIGILANCIA, VisibleVerDocumentoArchivero = false });
                                                    }
                                                }
                                            }
                                        };

                                    if (_IdFuero == 1)
                                    {
                                        var _ActaComun = new cActaConsejoTecnicoComun().GetData(x => x.ID_IMPUTADO == SelectedEstudioTerminado.ID_IMPUTADO && x.ID_INGRESO == SelectedEstudioTerminado.ID_INGRESO && x.ID_ESTUDIO == SelectedEstudioTerminado.ID_ESTUDIO && x.PERSONALIDAD.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).FirstOrDefault();

                                        LstDocumentos.Add(new Archivero() { Disponible = _ActaComun != null ? "SI" : "NO", NombreArchivo = "FORMATO DE REMISIÓN DE ESTUDIOS DE PERSONALIDAD ", TipoArchivo = (short)eDocumentoMostrado.DICTAMEN_INDIVIDUAL, VisibleVerDocumentoArchivero = _ActaComun != null ? true : false });

                                        LstDocumentos.Add(new Archivero() { Disponible = _ActaComun != null ? "SI" : "NO", NombreArchivo = "ACTA DE CONSEJO TÉCNICO INTERDISCIPLINARIO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.ACTA_CONSEJO_TECNICO, VisibleVerDocumentoArchivero = _ActaComun != null ? true : false });
                                    };

                                    if (_EstudioPersonalidad.ID_MOTIVO == (short)eTipoSolicitudTraslado.ISLAS)
                                    {
                                        var _validacionTrasladoIslas = new cTrasladoIslasPersonalidad().GetData(c => c.ID_IMPUTADO == _EstudioPersonalidad.ID_IMPUTADO && c.ID_INGRESO == _EstudioPersonalidad.ID_INGRESO && c.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO && c.PERSONALIDAD.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).FirstOrDefault();

                                        LstDocumentos.Add(new Archivero() { Disponible = _validacionTrasladoIslas != null ? "SI" : "NO", NombreArchivo = " FORMATO DE SOLICITUD DE TRASLADO A ISLAS ", TipoArchivo = (short)eDocumentoMostrado.TRASLADO_ISLAS, VisibleVerDocumentoArchivero = _validacionTrasladoIslas != null ? true : false });
                                    };

                                    if (_EstudioPersonalidad.ID_MOTIVO == (short)eTipoSolicitudTraslado.INTERNACIONAL)
                                    {
                                        var _validacionTrasladoInternacional = new cTrasladoInternacionalPersonalidad().GetData(c => c.ID_IMPUTADO == _EstudioPersonalidad.ID_IMPUTADO && c.ID_INGRESO == _EstudioPersonalidad.ID_INGRESO && c.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO && c.PERSONALIDAD.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).FirstOrDefault();

                                        LstDocumentos.Add(new Archivero() { Disponible = _validacionTrasladoInternacional != null ? "SI" : "NO", NombreArchivo = " FORMATO DE SOLICITUD DE TRASLADO INTERNACIONAL ", TipoArchivo = (short)eDocumentoMostrado.TRASLADO_INTERNACIONAL, VisibleVerDocumentoArchivero = _validacionTrasladoInternacional != null ? true : false });
                                    };

                                    if (_EstudioPersonalidad.ID_MOTIVO == (short)eTipoSolicitudTraslado.NACIONAL)
                                    {
                                        var _validacionTrasladoNacional = new cTrasladoNacionalPersonalidad().GetData(c => c.ID_IMPUTADO == _EstudioPersonalidad.ID_IMPUTADO && c.ID_INGRESO == _EstudioPersonalidad.ID_INGRESO && c.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO && c.PERSONALIDAD.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).FirstOrDefault();

                                        LstDocumentos.Add(new Archivero() { Disponible = _validacionTrasladoNacional != null ? "SI" : "NO", NombreArchivo = " FORMATO DE SOLICITUD DE TRASLADO NACIONAL ", TipoArchivo = (short)eDocumentoMostrado.TRASLADO_NACIONAL, VisibleVerDocumentoArchivero = _validacionTrasladoNacional != null ? true : false });
                                    };
                                }
                            };
                            break;

                        default:
                            //no action
                            break;
                    };
                };
            }

            catch (Exception exc)
            {
                throw exc;
            }

            return;
        }

        private void MuestraFormato(Archivero _Documento, PERSONALIDAD _ActualPersonalidad)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    if (_Documento == null || _ActualPersonalidad == null) return;
                    switch (_Documento.TipoArchivo)
                    {
                        case (short)eDocumentoMostrado.PARTIDA_JURIDICA:
                            ImprimirPartidaJuridica(_ActualPersonalidad);
                            break;
                        case (short)eDocumentoMostrado.FICHA_SIGNALETICA:
                            if (_ActualPersonalidad.INGRESO != null)
                                ImprimeFicha(_ActualPersonalidad.INGRESO);
                            break;
                        case (short)eDocumentoMostrado.FICHA_JURIDICA:
                            ImprimeFichaJuridica(_ActualPersonalidad);
                            break;
                        case (short)eDocumentoMostrado.ACTA_CONSEJO_TECNICO:
                            MuestraFormatoActaConsejoTecnicoComun(_ActualPersonalidad);
                            break;
                        case (short)eDocumentoMostrado.DICTAMEN_INDIVIDUAL:
                            MuestraOficioRemisionDictamenEstudioPersonalidad(_ActualPersonalidad);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_MEDICO:
                            if (_ActualPersonalidad.INGRESO != null)
                                MedicoComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSIQ:
                            PsiquiatricoComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSICO:
                            PsicologicoComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CRIMI:
                            CriminologicoComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SOCIO_FAM:
                            SocioFamiliarComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_EDUC:
                            EducativoComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CAPAC:
                            CapacitacionComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SEGURIDAD:
                            SeguridadComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.ACTA_FEDERAL:
                            ActaFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_MEDICO:
                            MedicoFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_PSICO:
                            PsicologicoFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_TRABAJO_SOCIAL:
                            SocioFamiliarFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CAPAC:
                            CapacitacionFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_EDUCATIVAS:
                            EducativasFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_VIGILANCIA:
                            VigilanciaFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CRIMINOLOGICO:
                            CriminologicoFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.OFICIO_PETICION_REALIZACION_ESTUDIOS_PERSONALIDAD:
                            ObservableCollection<PERSONALIDAD> lstDummy = new ObservableCollection<PERSONALIDAD>();
                            var CondensadoEstudiosProgramadosGeneral = new cEstudioPersonalidad().GetData(x => x.NUM_OFICIO == _ActualPersonalidad.NUM_OFICIO && x.PROG_NOMBRE == _ActualPersonalidad.PROG_NOMBRE && x.INGRESO != null && x.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro);
                            _CantidadFichas = CondensadoEstudiosProgramadosGeneral.Any() ? CondensadoEstudiosProgramadosGeneral.Count() : 0;
                            NombrePrograma = CondensadoEstudiosProgramadosGeneral.Any() ? CondensadoEstudiosProgramadosGeneral.FirstOrDefault().PROG_NOMBRE : string.Empty;
                            if (CondensadoEstudiosProgramadosGeneral != null && CondensadoEstudiosProgramadosGeneral.Any())
                                foreach (var item in CondensadoEstudiosProgramadosGeneral)
                                    lstDummy.Add(item);

                            MuestraOficioEnvio(lstDummy);
                            break;

                        case (short)eDocumentoMostrado.REMISION_DPMJ:
                            ObservableCollection<PERSONALIDAD> lstEstudios = new ObservableCollection<PERSONALIDAD>();
                            var Condensado = new cEstudioPersonalidad().GetData(x => x.NUM_OFICIO.Trim() == _ActualPersonalidad.NUM_OFICIO && x.PROG_NOMBRE.Trim() == _ActualPersonalidad.PROG_NOMBRE && x.ID_SITUACION != 4);

                            if (Condensado != null && Condensado.Any())
                                foreach (var item in Condensado)
                                    lstEstudios.Add(item);

                            MuestraOficioRemisionDEPMJ(lstEstudios);
                            break;

                        case (short)eDocumentoMostrado.REMISION_CIERRE:
                            MuestraFormatoCierreEstudiosPersonalidadArchivero(_ActualPersonalidad);
                            break;

                        case (short)eDocumentoMostrado.TRASLADO_NACIONAL:
                            ImprimeTrasladoNacional(_ActualPersonalidad);
                            break;

                        case (short)eDocumentoMostrado.TRASLADO_ISLAS:
                            ImprimeTrasladoIslas(_ActualPersonalidad);
                            break;

                        case (short)eDocumentoMostrado.TRASLADO_INTERNACIONAL:
                            ImprimeTrasladoInternacional(_ActualPersonalidad);
                            break;

                        default:
                            break;
                    };
                });
            }
            catch (Exception exc)
            {
                throw;
            }

            return;
        }

        private void ImprimeFicha(INGRESO _dato)
        {
            try
            {
                if (_dato != null)
                {
                    var vw = new ReporteView(_dato);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    vw.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    vw.Owner = PopUpsViewModels.MainWindow;
                    vw.Show();
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar un ingreso.");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar datos para imprimir la ficha de identificacion.", ex);
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
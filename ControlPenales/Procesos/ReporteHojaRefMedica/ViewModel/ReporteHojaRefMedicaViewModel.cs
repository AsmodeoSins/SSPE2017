using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPenales
{
    public partial class ReporteHojaRefMedicaViewModel:ValidationViewModelBase
    {
        #region Generales
        private async void ReporteHojaRefMedicaOnLoading(ReporteHojaRefMedicaView window)
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                CargarAtencionTipoBuscar(true);
                selectedAtencion_TipoBuscarValue = -1;
                RaisePropertyChanged("SelectedAtencion_TipoBuscarValue");
            });
        }

        private async void ClickSwitch(object parametro)
        {
            if (parametro != null && !string.IsNullOrWhiteSpace(parametro.ToString()))
                switch (parametro.ToString())
                {
                    case "filtro_interconsultas":
                        if (!IsFechaIniBusquedaSolValida)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La fecha de inicio tiene que ser menor a la fecha fin!");
                            return;
                        }
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarSolicitudesInterconsulta(selectedAtencion_TipoBuscarValue == -1 ? (short?)null : selectedAtencion_TipoBuscarValue, anioBuscarInter, folioBuscarInter,
                                nombreBuscarInter, apellidoPaternoBuscarInter, apellidoMaternoBuscarInter, null,
                                fechaInicialBuscarInter, fechaFinalBuscarInter, true);
                        });
                        break;
                    case "seleccionar_interconsulta":
                        if (selectedInterconsultaBusqueda==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debe de seleccionar una solicitud de interconsulta");
                            return;
                        }
                        await CargarReporte();
                        if (documento == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la plantilla del documento");
                            return;
                        }

                        var tc = new TextControlView();
                        tc.Closed += (s, e) =>
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        };
                        tc.editor.Loaded += (s, e) =>
                        {
                            var d = new cWord().FillFieldsDocx(documento.DOCUMENTO, diccionario_reporte);
                            //DOCX
                            tc.editor.EditMode = TXTextControl.EditMode.ReadOnly;
                            TXTextControl.LoadSettings _settings = new TXTextControl.LoadSettings();
                            tc.editor.Load(d, TXTextControl.BinaryStreamType.WordprocessingML, _settings);
                            //aqui se mueve el margen del documento, el txttextcontrol maneja como unidad de medida 1/96 de la pulgada, y piensa que el word le esta mandando 40ums solamente, menos de la pulgada.
                            tc.editor.PageMargins.Left = 100;
                            tc.editor.PageMargins.Right = 100;
                        };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Show();
                        
                        break;
                }
        }

        private void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_fecha_inicio_busqueda_sol":
                    if (!FechaInicialBuscarInter.HasValue || !FechaFinalBuscarInter.HasValue || FechaFinalBuscarInter >= FechaInicialBuscarInter)
                        IsFechaIniBusquedaSolValida = true;
                    else
                        IsFechaIniBusquedaSolValida = false;
                    break;
            }
        }

        private async Task CargarReporte()
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                var _reporte = new cHojaReferenciaMedica();
                StringBuilder _str_builder = new StringBuilder();

                if (selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.Any())
                {
                    foreach (var item in Enum.GetValues(typeof(eFamiliares)).Cast<eFamiliares>())
                    {
                        var _familiar = new cTipoReferencia().Obtener((short)item).FirstOrDefault();
                        if (_familiar != null)
                        {
                            if (_str_builder.Length != 0)
                                _str_builder.Append(", ");
                            _str_builder.Append(new cTipoReferencia().Obtener((short)item).FirstOrDefault().DESCR.Trim());
                            _str_builder.Append(":");
                            if (selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().HISTORIA_CLINICA_FAMILIAR.Any(a => a.ID_TIPO_REFERENCIA == _familiar.ID_TIPO_REFERENCIA))
                            {
                                var _antecedente_heredo_fam = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().HISTORIA_CLINICA_FAMILIAR.First(a => a.ID_TIPO_REFERENCIA == _familiar.ID_TIPO_REFERENCIA);
                                if ((!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_ALERGIAS) && _antecedente_heredo_fam.AHF_ALERGIAS != "N")
                                    || (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_CA) && _antecedente_heredo_fam.AHF_CA != "N")
                                    || (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_CARDIACOS) && _antecedente_heredo_fam.AHF_CARDIACOS != "N")
                                    || (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_DIABETES) && _antecedente_heredo_fam.AHF_DIABETES != "N")
                                    || (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_HIPERTENSIVO) && _antecedente_heredo_fam.AHF_HIPERTENSIVO != "N")
                                    || (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_MENTALES) && _antecedente_heredo_fam.AHF_MENTALES != "N")
                                    || (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_TB) && _antecedente_heredo_fam.AHF_TB != "N")
                                    || (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_EPILEPSIA) && _antecedente_heredo_fam.AHF_EPILEPSIA != "N"))
                                {
                                    var _subsecuente = false;
                                    if (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_ALERGIAS) && _antecedente_heredo_fam.AHF_ALERGIAS != "N")
                                    {
                                        _str_builder.Append("ALERGIAS");
                                        _subsecuente = true;
                                    }
                                    if (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_CA) && _antecedente_heredo_fam.AHF_CA != "N")
                                    {
                                        if (_subsecuente)
                                            _str_builder.Append(", ");
                                        _str_builder.Append("CA");
                                        _subsecuente = true;
                                    }
                                    if (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_CARDIACOS) && _antecedente_heredo_fam.AHF_CARDIACOS != "N")
                                    {
                                        if (_subsecuente)
                                            _str_builder.Append(", ");
                                        _str_builder.Append("CARDIACOS");
                                        _subsecuente = true;
                                    }
                                    if (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_DIABETES) && _antecedente_heredo_fam.AHF_DIABETES != "N")
                                    {
                                        if (_subsecuente)
                                            _str_builder.Append(", ");
                                        _str_builder.Append("DIABETES");
                                        _subsecuente = true;
                                    }
                                    if (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_HIPERTENSIVO) && _antecedente_heredo_fam.AHF_HIPERTENSIVO != "N")
                                    {
                                        if (_subsecuente)
                                            _str_builder.Append(", ");
                                        _str_builder.Append("HIPERTENSIVO");
                                        _subsecuente = true;
                                    }
                                    if (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_MENTALES) && _antecedente_heredo_fam.AHF_MENTALES != "N")
                                    {
                                        if (_subsecuente)
                                            _str_builder.Append(", ");
                                        _str_builder.Append("MENTALES");
                                        _subsecuente = true;
                                    }
                                    if (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_TB) && _antecedente_heredo_fam.AHF_TB != "N")
                                    {
                                        if (_subsecuente)
                                            _str_builder.Append(", ");
                                        _str_builder.Append("T.B.");
                                        _subsecuente = true;
                                    }
                                    if (!string.IsNullOrWhiteSpace(_antecedente_heredo_fam.AHF_EPILEPSIA) && _antecedente_heredo_fam.AHF_EPILEPSIA != "N")
                                    {
                                        if (_subsecuente)
                                            _str_builder.Append(", ");
                                        _str_builder.Append("EPILEPSIA");
                                        _subsecuente = true;
                                    }
                                }
                                else
                                    _str_builder.Append("NINGUNO");
                            }
                            else
                            {
                                _str_builder.Append("NINGUNO");
                            }
                        }
                    }
                    _reporte.ANT_HEREDO_FAMILIARES = _str_builder.ToString();
                    _str_builder.Clear();
                    if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_NACIMIENTO)
                        || !string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_ALIMENTACION)
                        || !string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_HABITACION)
                        || (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TABAQUISMO) && selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TABAQUISMO == "S")
                        || (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_ALCOHOLISMO) && selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_ALCOHOLISMO == "S")
                        || (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TOXICOMANIAS) && selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TOXICOMANIAS == "S"))
                    {
                        if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_NACIMIENTO))
                            _str_builder.Append(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_NACIMIENTO);
                        if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_ALIMENTACION))
                        {
                            if (_str_builder.Length != 0)
                                _str_builder.Append(", ");
                            _str_builder.Append(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_ALIMENTACION);
                        }
                        if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_HABITACION))
                        {
                            if (_str_builder.Length != 0)
                                _str_builder.Append(", ");
                            _str_builder.Append(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_HABITACION);
                        }
                        if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TABAQUISMO) && selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TABAQUISMO == "S")
                        {
                            if (_str_builder.Length != 0)
                                _str_builder.Append(", ");
                            _str_builder.Append("TABAQUISMO: SÍ");
                            if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TABAQUISMO_OBSERV))
                            {
                                _str_builder.Append(", ");
                                _str_builder.Append(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TABAQUISMO_OBSERV);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_ALCOHOLISMO) && selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_ALCOHOLISMO == "S")
                        {
                            if (_str_builder.Length != 0)
                                _str_builder.Append(", ");
                            _str_builder.Append("ALCOHOLISMO: SÍ");
                            if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_ALCOHOLISMO_OBSERV))
                            {
                                _str_builder.Append(", ");
                                _str_builder.Append(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_ALCOHOLISMO_OBSERV);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TOXICOMANIAS) && selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TOXICOMANIAS == "S")
                        {
                            if (_str_builder.Length != 0)
                                _str_builder.Append(", ");
                            _str_builder.Append("TOXICOMANÍAS: SÍ");
                            if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TOXICOMANIAS))
                            {
                                _str_builder.Append(", ");
                                _str_builder.Append(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().APNP_TOXICOMANIAS);
                            }
                        }
                    }
                    else
                        _str_builder.Append("NINGUNO");
                    _reporte.ANT_PERSONALES_NO_PAT = _str_builder.ToString();
                    _str_builder.Clear();
                    if (selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().HISTORIA_CLINICA_PATOLOGICOS.Any())
                    {
                        foreach (var item in selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.HISTORIA_CLINICA.First().HISTORIA_CLINICA_PATOLOGICOS)
                        {
                            if (_str_builder.Length > 0)
                                _str_builder.Append(", ");
                            _str_builder.Append(item.PATOLOGICO_CAT.DESCR);
                            if (!string.IsNullOrWhiteSpace(item.OBSERVACIONES))
                            {
                                _str_builder.Append(", ");
                                _str_builder.Append(item.OBSERVACIONES);
                            }
                        }
                    }
                    else
                    {
                        _str_builder.Append("NINGUNO");
                    }
                    _reporte.ANT_PERSONALES_PAT = _str_builder.ToString();
                }
                else
                {
                    _reporte.ANT_HEREDO_FAMILIARES = "DESCONOCIDOS";
                    _reporte.ANT_PERSONALES_NO_PAT = "DESCONOCIDOS";
                    _reporte.ANT_PERSONALES_PAT = "DESCONOCIDOS";

                }
                if (selectedInterconsultaBusqueda.ID_ESPECIALIDAD != null)
                    _reporte.CAUSA_SOLICITUD = selectedInterconsultaBusqueda.ESPECIALIDAD.DESCR;
                else
                {
                    _str_builder.Clear();
                    if (selectedInterconsultaBusqueda.SERVICIO_AUX_INTERCONSULTA.Any())
                        foreach (var item in selectedInterconsultaBusqueda.SERVICIO_AUX_INTERCONSULTA)
                        {
                            if (_str_builder.Length > 0)
                                _str_builder.Append(", ");
                            _str_builder.Append(item.SERVICIO_AUX_DIAG_TRAT.DESCR);
                        }
                    _reporte.CAUSA_SOLICITUD = _str_builder.ToString();
                }
                _reporte.CIUDAD = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CENTRO.DESCR.Trim();
                _reporte.DESTINO = selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA != null ? selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.Any() ? !string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO) ?
                    selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO.Trim() : selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR.Trim() : "" : "";
                _reporte.EDAD = new Fechas().CalculaEdad(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA);
                _reporte.ESTADO = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CENTRO.MUNICIPIO.ENTIDAD.DESCR;
                _reporte.FECHA_NAC = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy");
                _reporte.FECHA_REGISTRO_LETRA = Fechas.fechaLetra(selectedInterconsultaBusqueda.REGISTRO_FEC, false);
                _reporte.FOLIO = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_ANIO.ToString() + "/" + selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_IMPUTADO.ToString();
                _reporte.FREC_CARDIACA = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC;
                _reporte.FREC_RESPIRATORIA = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA;
                _reporte.IMPRESION_DIAGNOSTICA = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.PRONOSTICO.DESCR;
                _str_builder.Clear();
                _str_builder.Append(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim());
                if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO))
                {
                    _str_builder.Append(" ");
                    _str_builder.Append(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim());
                }
                if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO))
                {
                    _str_builder.Append(" ");
                    _str_builder.Append(selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim());
                }
                _reporte.NOMBRE = _str_builder.ToString();
                _reporte.OBSERVACIONES = selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA != null ? selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.Any() ? selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.First().OBSERV : string.Empty : string.Empty;
                _reporte.PESO = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO;
                _reporte.PRIMERA = selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA != null ? selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.Any() ?selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.First().ID_TIPO_CITA == (short)enumCita_Tipo.PRIMERA ? "X" : "" : "" : "";
                _reporte.SUBSECUENTE = selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA != null ? selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.Any() ? selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.First().ID_TIPO_CITA == (short)enumCita_Tipo.SUBSECUENTE ? "X" : "" : "" : "";
                if (selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any())
                {
                    _str_builder.Clear();
                    foreach (var item in selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD)
                    {
                        if (_str_builder.Length > 0)
                            _str_builder.Append(", ");
                        _str_builder.Append(item.ENFERMEDAD.NOMBRE.Trim());
                    }
                    _reporte.RESUMEN_CLINICO = _str_builder.ToString();
                }
                _reporte.SEXO = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO";
                _reporte.TENSION_ARTERIAL = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL.Trim();
                _reporte.TIPO_ATENCION = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ATENCION_SERVICIO.ATENCION_TIPO.DESCR;
                _reporte.CENTRO = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CENTRO.DESCR.Trim();
                _reporte.EXP_HGT = selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA != null ? selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.Any() ? selectedInterconsultaBusqueda.HOJA_REFERENCIA_MEDICA.First().EXP_HGT : "" : "";
                _reporte.TALLA = selectedInterconsultaBusqueda.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TALLA;
                _str_builder.Clear();
                _str_builder.Append(selectedInterconsultaBusqueda.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim());
                if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim()))
                {
                    _str_builder.Append(" ");
                    _str_builder.Append(selectedInterconsultaBusqueda.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim());
                }
                if (!string.IsNullOrWhiteSpace(selectedInterconsultaBusqueda.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim()))
                {
                    _str_builder.Append(" ");
                    _str_builder.Append(selectedInterconsultaBusqueda.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim());
                }
                _reporte.NOMBRE_MEDICO = _str_builder.ToString();
                diccionario_reporte = new Dictionary<string, string>();
                diccionario_reporte.Add("<<CENTRO>>", _reporte.CENTRO);
                diccionario_reporte.Add("<<NOMBRE>>", _reporte.NOMBRE);
                diccionario_reporte.Add("<<EDAD>>", _reporte.EDAD.ToString());
                diccionario_reporte.Add("<<FOLIO>>", _reporte.FOLIO);
                diccionario_reporte.Add("<<SEXO>>", _reporte.SEXO);
                diccionario_reporte.Add("<<FECHA_NAC>>", _reporte.FECHA_NAC);
                diccionario_reporte.Add("<<DESTINO>>", _reporte.DESTINO);
                diccionario_reporte.Add("<<TIPO_ATENCION>>", _reporte.TIPO_ATENCION);
                diccionario_reporte.Add("<<EXP_HGT>>", _reporte.EXP_HGT);
                diccionario_reporte.Add("<<CAUSA_SOLICITUD>>", _reporte.CAUSA_SOLICITUD);
                diccionario_reporte.Add("<<ANT_HEREDO_FAMILIARES>>", _reporte.ANT_HEREDO_FAMILIARES);
                diccionario_reporte.Add("<<ANT_PERSONALES_NO_PAT>>", _reporte.ANT_PERSONALES_NO_PAT);
                diccionario_reporte.Add("<<ANT_PERSONALES_PAT>>", _reporte.ANT_PERSONALES_PAT);
                diccionario_reporte.Add("<<PRIMERA>>", _reporte.PRIMERA);
                diccionario_reporte.Add("<<SUBSECUENTE>>", _reporte.SUBSECUENTE);
                diccionario_reporte.Add("<<TA>>", _reporte.TENSION_ARTERIAL);
                diccionario_reporte.Add("<<FC>>", _reporte.FREC_CARDIACA);
                diccionario_reporte.Add("<<FR>>", _reporte.FREC_RESPIRATORIA);
                diccionario_reporte.Add("<<PESO>>", _reporte.PESO);
                diccionario_reporte.Add("<<TALLA>>", _reporte.TALLA);
                diccionario_reporte.Add("<<RESUMEN_CLINICO>>", _reporte.RESUMEN_CLINICO);
                diccionario_reporte.Add("<<IMPRESION_DIAGNOSTICA>>", _reporte.IMPRESION_DIAGNOSTICA);
                diccionario_reporte.Add("<<OBSERVACIONES>>", _reporte.OBSERVACIONES);
                diccionario_reporte.Add("<<CIUDAD>>", _reporte.CIUDAD);
                diccionario_reporte.Add("<<ESTADO>>", _reporte.ESTADO);
                diccionario_reporte.Add("<<FECHA_REGISTRO_LETRA>>", _reporte.FECHA_REGISTRO_LETRA);
                diccionario_reporte.Add("<<NOMBRE_MEDICO>>", _reporte.NOMBRE_MEDICO);

                documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.HOJA_REFERENCIA_MEDICA);
            });

        }

        #endregion

        #region catalogos
        private void CargarAtencionTipoBuscar(bool isExceptionManaged = false)
        {
            try
            {
                lstAtencion_TipoBuscar = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                lstAtencion_TipoBuscar.Add(new ATENCION_TIPO
                {
                    DESCR = "SELECCIONE UNO",
                    ID_TIPO_ATENCION = -1
                });
                RaisePropertyChanged("LstAtencion_TipoBuscar");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error el cargar tipos de interconsulta", ex);
            }
        }

        #endregion

        #region Buscar
        private void BuscarSolicitudesInterconsulta(short? tipo_atencion = null, short? anio_imputado = null, int? folio_imputado = null, string nombre = "", string paterno = "", string materno = "", short? tipo_interconsulta = null,
            DateTime? fecha_inicio = null, DateTime? fecha_final = null, bool isExceptionManaged = false)
        {
            try
            {
                listaInterconsultasBusqueda = new ObservableCollection<INTERCONSULTA_SOLICITUD>(new cInterconsulta_Solicitud().Buscar(GlobalVar.gCentro,new List<string> { "N", "S" }, tipo_atencion, anio_imputado, folio_imputado,
                    nombre, paterno, materno, tipo_interconsulta, fecha_inicio, fecha_final));
                RaisePropertyChanged("ListaInterconsultasBusqueda");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar las solicitudes de interconsulta", ex);
            }
        }


        #endregion

        private enum eFamiliares
        {
            MADRE = 1002,
            PADRE = 1001,
            CONYUGE = 1054,
            HIJOS = 1055,
            HERMANOS = 1056
        };
    }
}

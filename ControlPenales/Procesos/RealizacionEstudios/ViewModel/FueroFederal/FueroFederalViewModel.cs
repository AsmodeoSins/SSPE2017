using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class RealizacionEstudiosViewModel
    {

        #region Llenado de datos de reportes de fuero federal
        private Microsoft.Reporting.WinForms.ReportDataSource DatosActaConsejoTecnicoInteridsciplinarioFueroFederal(PERSONALIDAD Entity)
        {
            try
            {
                string _Del = string.Empty;
                string PartirDe = string.Empty;
                var CausaPenal = new cCausaPenal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO).FirstOrDefault();
                if (CausaPenal != null)
                {
                    var _Sentencia = new cSentencia().GetData(c => c.ID_INGRESO == SelectIngreso.ID_INGRESO && c.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && c.ID_CAUSA_PENAL == CausaPenal.ID_CAUSA_PENAL).FirstOrDefault();
                    if (_Sentencia != null)
                    {
                        var _SentenciaDelito = new cSentenciaDelito().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_SENTENCIA == _Sentencia.ID_SENTENCIA);
                        if (_SentenciaDelito != null && _SentenciaDelito.Any())
                            foreach (var item in _SentenciaDelito)
                                _Del += string.Format("{0}, ", !string.IsNullOrEmpty(item.DESCR_DELITO) ? item.DESCR_DELITO.Trim() : string.Empty);

                        var InicioComp = new cSentencia().GetData(x => x.ID_SENTENCIA == _Sentencia.ID_SENTENCIA && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault();
                        if (InicioComp != null)
                            PartirDe = InicioComp.FEC_INICIO_COMPURGACION.HasValue ? InicioComp.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty;
                    };
                };

                var _Datos = new List<cRealizacionEstudios>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                CamposBase.NombreInterno = string.Format("{0} {1} {2}", SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);

                var _DatosActHecha = new cActaConsejoTecnicoFueroFederal().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO).FirstOrDefault();
                if (_DatosActHecha != null)
                {
                    CamposBase.ExpInterno = _DatosActHecha.EXPEDIENTE;
                    CamposBase.NombreInterno = string.Format("{0} {1} {2}", SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);
                    CamposBase.DelitoInterno = _Del;
                    CamposBase.SentenciaInterno = CalcularSentencia().ToUpper();
                    CamposBase.APartirDe = PartirDe;
                    CamposBase.EnSesionFecha = _DatosActHecha.SESION_FEC.HasValue ? _DatosActHecha.SESION_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;

                    var _Edo = new cEntidad().GetData(x => x.ID_ENTIDAD == Parametro.ESTADO).FirstOrDefault();
                    if (_Edo != null)
                        CamposBase.Estado = !string.IsNullOrEmpty(_Edo.DESCR) ? _Edo.DESCR.Trim() : string.Empty;

                    CamposBase.NombbreCentro = _DatosActHecha.CENTRO != null ? !string.IsNullOrEmpty(_DatosActHecha.CENTRO.DESCR) ? _DatosActHecha.CENTRO.DESCR.Trim() : string.Empty : string.Empty;
                    CamposBase.ActuacionTexto = _DatosActHecha.APROBADO_APLAZADO == "S" ? "APROBADO" : "APLAZADO";
                    CamposBase.VotosTexto = _DatosActHecha.APROBADO_POR == "M" ? "MAYORIA" : "UNANIMIDAD";
                    CamposBase.LugarDesc = _DatosActHecha.LUGAR;
                    CamposBase.TramiteTexto = !string.IsNullOrEmpty(_DatosActHecha.TRAMITE) ? _DatosActHecha.TRAMITE.Trim() : string.Empty;
                    CamposBase.DirectorCRS = !string.IsNullOrEmpty(_DatosActHecha.DIRECTOR) ? _DatosActHecha.DIRECTOR.Trim() : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource DatosAreasTecnicasActaInterd(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cActaDeterminoRealizacionEstudiosPersonalidad>();
                var _DatosCapacitacion = new cActaDeterminoFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cActaDeterminoRealizacionEstudiosPersonalidad CamposBase = new cActaDeterminoRealizacionEstudiosPersonalidad();
                if (_DatosCapacitacion != null)
                    if (_DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            CamposBase = new cActaDeterminoRealizacionEstudiosPersonalidad()
                            {
                                Nombre = item.NOMBRE,
                                NombreArea = item.AREA_TECNICA != null ? !string.IsNullOrEmpty(item.AREA_TECNICA.DESCR) ? item.AREA_TECNICA.DESCR.Trim() : string.Empty : string.Empty,
                                Opinion = !string.IsNullOrEmpty(item.OPINION) ? item.OPINION == "F" ? "FAVORABLE" : "DESFAVORABLE" : string.Empty
                            };

                            _Datos.Add(CamposBase);
                        };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;

            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoMedicoFueroFederal(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioMedicoFederal = new cEstudioMedicoFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioMedicoFederal != null)
                    {
                        CamposBase.NombreInterno = !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.NOMBRE) ? _DatosEstudioMedicoFederal.NOMBRE.Trim() : string.Empty;
                        CamposBase.AliasInterno = !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.ALIAS) ? _DatosEstudioMedicoFederal.ALIAS.Trim() : string.Empty;
                        CamposBase.EdadInterno = _DatosEstudioMedicoFederal.EDAD.HasValue ? _DatosEstudioMedicoFederal.EDAD.Value.ToString() : string.Empty;
                        if (!string.IsNullOrEmpty(_DatosEstudioMedicoFederal.EDO_CIVIL))
                        {
                            short _EstInterno = short.Parse(_DatosEstudioMedicoFederal.EDO_CIVIL);
                            var _EdoCivil = new cEstadoCivil().GetData(x => x.ID_ESTADO_CIVIL == _EstInterno).FirstOrDefault();
                            if (_EdoCivil != null)
                                CamposBase.EstadoCivilInterno = !string.IsNullOrEmpty(_EdoCivil.DESCR) ? _EdoCivil.DESCR.Trim() : string.Empty;
                        };

                        CamposBase.OriginarioDeInterno = _DatosEstudioMedicoFederal.ORIGINARIO_DE;
                        CamposBase.OcupacionAnteriorInterno = _DatosEstudioMedicoFederal.OCUPACION_ANT.HasValue ? !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.OCUPACION.DESCR) ? _DatosEstudioMedicoFederal.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.OcupacionActualInterno = _DatosEstudioMedicoFederal.OCUPACION_ACT.HasValue ? !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.OCUPACION1.DESCR) ? _DatosEstudioMedicoFederal.OCUPACION1.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.DelitoInterno = _DatosEstudioMedicoFederal.DELITO;
                        CamposBase.SentenciaInterno = _DatosEstudioMedicoFederal.SENTENCIA;
                        CamposBase.AntecedentesHeredoFamFederal = _DatosEstudioMedicoFederal.ANTE_HEREDO_FAM;
                        CamposBase.AntecedentesPersonalesNoPatFederal = _DatosEstudioMedicoFederal.ANTE_PERSONAL_NO_PATOLOGICOS;
                        CamposBase.AntecedentesPatoFederal = _DatosEstudioMedicoFederal.ANTE_PATOLOGICOS;
                        CamposBase.PadecimientoActual = _DatosEstudioMedicoFederal.PADECIMIENTO_ACTUAL;
                        CamposBase.InterrogAparatosSistFederal = _DatosEstudioMedicoFederal.INTERROGATORIO_APARATOS;
                        CamposBase.ExploracionFisicaCabezCuello = _DatosEstudioMedicoFederal.EXP_FIS_CABEZA_CUELLO;
                        CamposBase.Torax = _DatosEstudioMedicoFederal.EXP_FIS_TORAX;
                        CamposBase.Abdomen = _DatosEstudioMedicoFederal.EXP_FIS_ABDOMEN;
                        CamposBase.OrganosGenit = _DatosEstudioMedicoFederal.EXP_FIS_GENITALES;
                        CamposBase.Extremidades = _DatosEstudioMedicoFederal.EXP_FIS_EXTREMIDADES;
                        CamposBase.TensionArterial = _DatosEstudioMedicoFederal.TA;
                        CamposBase.Teperatura = _DatosEstudioMedicoFederal.TEMPERATURA;
                        CamposBase.Pulso = _DatosEstudioMedicoFederal.PULSO;
                        CamposBase.Respiracion = _DatosEstudioMedicoFederal.RESPIRACION;
                        CamposBase.Estatura = _DatosEstudioMedicoFederal.ESTATURA;
                        CamposBase.DescripcionTatuajesCicatrRecAntiguasMalformacionesFederal = _DatosEstudioMedicoFederal.TATUAJES;
                        CamposBase.Diagnostico = _DatosEstudioMedicoFederal.DIAGNOSTICO;
                        CamposBase.TerpeuticaImpl = _DatosEstudioMedicoFederal.RESULTADOS_OBTENIDOS;
                        CamposBase.Conclusion = _DatosEstudioMedicoFederal.CONCLUSION;
                        CamposBase.DirectorCRS = _DatosEstudioMedicoFederal.DIRECTOR_CENTRO;
                        CamposBase.LugarDesc = _DatosEstudioMedicoFederal.LUGAR;
                        CamposBase.MatricesRavenTexto = _DatosEstudioMedicoFederal.MEDICO;
                        CamposBase.TextoGenerico1 = string.Format("( {0} )", _DatosEstudioMedicoFederal.ASIST_FARMACODEPENDENCIA == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico2 = string.Format("( {0} )", _DatosEstudioMedicoFederal.ASIST_AA == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico3 = string.Format("( {0} )", _DatosEstudioMedicoFederal.ASIST_OTROS == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico4 = _DatosEstudioMedicoFederal.ASIST_OTROS_ESPECIF;
                        CamposBase.TextoGenerico5 = _DatosEstudioMedicoFederal.TA;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosToxicosMedicoFueroFederal(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleToxicos = new List<cAntecedentesConsumoToxicosMedicoFederal>();
                var Datos = new cSustanciaToxicaFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                if (Datos != null && Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleToxicos.Add(new cAntecedentesConsumoToxicosMedicoFederal
                        {
                            Cantidad = item.CANTIDAD.ToString(),
                            EdadInicio = item.EDAD_INICIO.HasValue ? item.EDAD_INICIO.Value.ToString() : string.Empty,
                            Periodicidad = item.PERIODICIDAD,
                            Tipo = item.DROGA != null ? !string.IsNullOrEmpty(item.DROGA.DESCR) ? item.DROGA.DESCR.Trim() : string.Empty : string.Empty
                        });
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoPsicologicoFueroFederal(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioMedicoFederal = new cEstudioPsicologicoFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioMedicoFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioMedicoFederal.NOMBRE;
                        CamposBase.AliasInterno = _DatosEstudioMedicoFederal.SOBRENOMBRE;
                        CamposBase.EdadInterno = _DatosEstudioMedicoFederal.EDAD.HasValue ? _DatosEstudioMedicoFederal.EDAD.ToString() : string.Empty;
                        CamposBase.DelitoInterno = _DatosEstudioMedicoFederal.DELITO;
                        CamposBase.ActitudTomadaAntesEntrevista = string.Concat("ACTITUD TOMADA ANTE LA ENTREVISTA: ", _DatosEstudioMedicoFederal.ACTITUD);
                        CamposBase.ExamenMentalFF = string.Concat("EXAMEN MENTAL: ", _DatosEstudioMedicoFederal.EXAMEN_MENTAL);
                        CamposBase.PruebasAplicadas = string.Concat("PRUEBAS APLICADAS: ", _DatosEstudioMedicoFederal.PRUEBAS_APLICADAS);
                        CamposBase.NivelInt = _DatosEstudioMedicoFederal.NIVEL_INTELECTUAL;
                        CamposBase.CI = _DatosEstudioMedicoFederal.CI;
                        CamposBase.IndiceLesionOrganica = _DatosEstudioMedicoFederal.INDICE_LESION_ORGANICA;
                        CamposBase.DinamicaPersonalidadIngreso = _DatosEstudioMedicoFederal.DINAM_PERSON_INGRESO;
                        CamposBase.DinamicaPersonalidadActual = _DatosEstudioMedicoFederal.DINAM_PERSON_ACTUAL;
                        CamposBase.ResultadosTratamientoProp = _DatosEstudioMedicoFederal.RESULT_TRATAMIENTO;
                        CamposBase.RequiereTratExtraMurosTexto = string.Format("REQUERIMIENTOS DE CONTINUACIÓN DE TRATAMIENTO: SI ( {0} )  NO ( {1} )", _DatosEstudioMedicoFederal.REQ_CONT_TRATAMIENTO == "S" ? "X" : string.Empty, _DatosEstudioMedicoFederal.REQ_CONT_TRATAMIENTO == "N" ? "X" : string.Empty);
                        CamposBase.Abdomen = _DatosEstudioMedicoFederal.INTERNO;
                        CamposBase.Actitud = _DatosEstudioMedicoFederal.EXTERNO;
                        CamposBase.EspecifiqueExtraM = string.Format("ESPECIFIQUE: {0}", _DatosEstudioMedicoFederal.ESPECIFIQUE);
                        CamposBase.DirectorCRS = _DatosEstudioMedicoFederal.DIRECTOR_DENTRO;
                        CamposBase.LugarDesc = _DatosEstudioMedicoFederal.LUGAR;
                        CamposBase.Pronostico = string.Format("PRONÓSTICO DE REINTEGRACIÓN SOCIAL: {0}", _DatosEstudioMedicoFederal.PRONOSTICO_REINTEGRACION);
                        CamposBase.OpinionSobreOtorgamientoBeneficio = string.Format("OPINIÓN SOBRE EL OTORGAMIENTO DEL BENEFICIO: {0}", _DatosEstudioMedicoFederal.OPINION);
                        CamposBase.MatricesRavenTexto = _DatosEstudioMedicoFederal.PSICOLOGO;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        #region Reporte de Trabajo Social
        private Microsoft.Reporting.WinForms.ReportDataSource DatosGrupoFamiliarPrimarioFueroFederal(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleToxicos = new List<cGrupoFamiliarPrimarioDatos>();
                var Datos = new cGrupoFamiliarFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.ID_GRUPO_FAMILIAR == (short)eTipopGrupoTrabajoSocial.PRIMARIO);
                if (Datos != null && Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleToxicos.Add(new cGrupoFamiliarPrimarioDatos
                        {
                            Edad = item.EDAD,
                            EdoCivil = item.ESTADO_CIVIL,
                            Nombre = item.NOMBRE,
                            Ocupacion = item.OCUPACION,
                            PArentesco = item.PARENTESCO
                        });
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosGrupoFamiliarSecundarioFueroFederal(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleToxicos = new List<cGrupoFamiliarPrimarioDatos>();
                var Datos = new cGrupoFamiliarFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.ID_GRUPO_FAMILIAR == (short)eTipopGrupoTrabajoSocial.SECUNDARIO);
                if (Datos != null && Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleToxicos.Add(new cGrupoFamiliarPrimarioDatos
                        {
                            Edad = item.EDAD,
                            EdoCivil = item.ESTADO_CIVIL,
                            Nombre = item.NOMBRE,
                            Ocupacion = item.OCUPACION,
                            PArentesco = item.PARENTESCO
                        });
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoTSFueroFederal(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cEstudioTrabajoSocialFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.DialectoInterno = _DatosEstudioTSFederal.DIALECTO.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.DIALECTO1.DESCR) ? _DatosEstudioTSFederal.DIALECTO1.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.LugarFecNacInterno = string.Format("{0} {1}",
                                !string.IsNullOrEmpty(_DatosEstudioTSFederal.LUGAR_NAC) ? _DatosEstudioTSFederal.LUGAR_NAC.Trim() : string.Empty,
                                _DatosEstudioTSFederal.FECHA_NAC.HasValue ? _DatosEstudioTSFederal.FECHA_NAC.Value.ToString("dd/MM/yyyy") : string.Empty);
                        CamposBase.EscolaridadAnteriorIngreso = _DatosEstudioTSFederal.ESCOLARIDAD_CENTRO.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.ESCOLARIDAD1.DESCR) ? _DatosEstudioTSFederal.ESCOLARIDAD1.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.EscolaridadAct = _DatosEstudioTSFederal.ESCOLARIDAD_ACTUAL.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.ESCOLARIDAD.DESCR) ? _DatosEstudioTSFederal.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                        if (_DatosEstudioTSFederal.EDO_CIVIL.HasValue)
                        {
                            var _edoCivil = new cEstadoCivil().GetData(c => c.ID_ESTADO_CIVIL == _DatosEstudioTSFederal.EDO_CIVIL.Value).FirstOrDefault();
                            if (_edoCivil != null)
                                CamposBase.EstadoCivilInterno = !string.IsNullOrEmpty(_edoCivil.DESCR) ? _edoCivil.DESCR.Trim() : string.Empty;
                        };

                        CamposBase.OcupacionAnteriorInterno = _DatosEstudioTSFederal.TRABAJO_DESEMP_ANTES;
                        CamposBase.DomicilioInterno = _DatosEstudioTSFederal.DOMICILIO;
                        CamposBase.CaractZona = string.Format("URBANA ( {0} )  SUB - URBANA ( {1} )  RURAL ( {2} )  CRIMINÓGENA (Existencia de bandas o pandillas, sobrepoblación, prostíbulos, cantinas, billares, etc.) ( {3} )",
                            _DatosEstudioTSFederal.ECO_FP_ZONA == "U" ? "X" : string.Empty, _DatosEstudioTSFederal.ECO_FP_ZONA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.ECO_FP_ZONA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.ECO_FP_ZONA == "N" ? "X" : string.Empty);
                        CamposBase.ResponsManutencionHogar = string.Format("RESPONSABLE (S) DE LA MANUTENCIÓN DEL HOGAR: {0}", _DatosEstudioTSFederal.ECO_FP_RESPONSABLE);
                        CamposBase.TotalIngresosMensuales = _DatosEstudioTSFederal.ECO_FP_TOTAL_INGRESOS_MEN.HasValue ? _DatosEstudioTSFederal.ECO_FP_TOTAL_INGRESOS_MEN.Value.ToString("c") : string.Empty;
                        CamposBase.TotalEgresosMensuales = _DatosEstudioTSFederal.ECO_FP_TOTAL_EGRESOS_MEN.HasValue ? _DatosEstudioTSFederal.ECO_FP_TOTAL_EGRESOS_MEN.Value.ToString("c") : string.Empty;
                        CamposBase.ActualmenteInternoCooperaEcon = _DatosEstudioTSFederal.ECO_FP_COOPERA_ACTUALMENTE;
                        CamposBase.TieneFondoAhorro = _DatosEstudioTSFederal.ECO_FP_FONDOS_AHORRO;
                        CamposBase.GrupoFamPrimarioTexto = string.Format(" GRUPO FAMILIAR: FUNCIONAL ( {0} ) DISFUNCIONAL ( {1} )", _DatosEstudioTSFederal.CARACT_FP_GRUPO == "F" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_GRUPO == "D" ? "X" : string.Empty);
                        CamposBase.RelacionesInterfamiliaresTexto = string.Format("RELACIONES INTERFAMILIARES: ADECUADAS ( {0} ) INADECUADAS ( {1} )", _DatosEstudioTSFederal.CARACT_FP_RELAC_INTERFAM == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_RELAC_INTERFAM == "I" ? "X" : string.Empty);
                        CamposBase.HuboViolenciaIntrafamTexto = string.Format("{0}", _DatosEstudioTSFederal.CARACT_FP_VIOLENCIA_FAM == "S" ? "SI" : "NO");
                        CamposBase.EspecificarViolenciaIntrafam = _DatosEstudioTSFederal.CARACT_FP_VIOLENCIA_FAM_ESPEFI;
                        CamposBase.NivelSocioEconomicoCultPrimario = string.Format("NIVEL SOCIO-ECONÓMICO Y CULTURAL ALTO ( {0} ) MEDIO ( {1} ) BAJO ( {2} )", _DatosEstudioTSFederal.CARACT_FP_NIVEL_SOCIO_CULTURAL == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_NIVEL_SOCIO_CULTURAL == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_NIVEL_SOCIO_CULTURAL == "B" ? "X" : string.Empty);
                        CamposBase.AlgunIntegTieneAntecedPenales = string.Format("ALGÚN INTEGRANTE DE LA FAMILIA TIENE ANTECEDENTES PENALES O ADICCIÓN A ALGÚN ESTUPEFACIENTE O CUALQUIER TIPO DE TÓXICOS {0} ESPECIFIQUE {1}", _DatosEstudioTSFederal.CARACT_FP_ANTECE_PENALES_ADIC == "S" ? "SI" : "NO", _DatosEstudioTSFederal.CARACT_FP_ANTECEDENTES_PENALES);
                        CamposBase.ConceptoTieneFamInterno = string.Format("CONCEPTO QUE TIENE LA FAMILIA DEL INTERNO: {0} ", _DatosEstudioTSFederal.CARACT_FP_CONCEPTO);
                        CamposBase.HijosUnionesAnt = _DatosEstudioTSFederal.CARACT_FS_HIJOS_ANT;
                        CamposBase.GrupoFamSec = string.Format("FUNCIONAL ( {0} )  DISFUNCIONAL ( {1} ) ", _DatosEstudioTSFederal.CARACT_FS_GRUPO == "F" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_GRUPO == "D" ? "X" : string.Empty);
                        CamposBase.RelacionesInterfamSecundario = string.Format("ADECUADAS ( {0} ) INADECUADAS ( {1} )", _DatosEstudioTSFederal.CARACT_FS_RELACIONES_INTERFAM == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_RELACIONES_INTERFAM == "I" ? "X" : string.Empty);
                        CamposBase.ViolenciaIntraFamGrupoSecundario = _DatosEstudioTSFederal.CARACT_FS_VIOLENCIA_INTRAFAM == "S" ? "SI" : "NO";
                        CamposBase.EspecificViolenciaGrupoSecundario = _DatosEstudioTSFederal.CARACT_FS_VIOLENCIA_INTRAFAM_E;
                        CamposBase.NivelSocioEconomicoCulturalGrupoSecundario = string.Format("ALTO ( {0} ) MEDIO ( {1} ) BAJO ( {2} )", _DatosEstudioTSFederal.CARACT_FS_NIVEL_SOCIO_CULTURAL == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_NIVEL_SOCIO_CULTURAL == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_NIVEL_SOCIO_CULTURAL == "B" ? "X" : string.Empty);

                        CamposBase.NumHabitacionesTotal = string.Format("NÚMERO DE HABITACIONES EN TOTAL (SALA, COMEDOR, COCINA, RECAMARAS, BAÑO, CUARTO DE SERVICIO, ETC.): {0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_NUM_HABITACIO.HasValue ? _DatosEstudioTSFederal.CARACT_FS_VIVIEN_NUM_HABITACIO.Value.ToString() : string.Empty);
                        CamposBase.DescripcionVivienda = string.Format("CÓMO ES SU VIVIENDA (DESCRIPCIÓN, MATERIALES DE LOS QUE ESTÁ CONSTRUIDA): {0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_DESCRIPCION);
                        CamposBase.TransporteCercaVivienda = string.Format("EL TRANSPORTE ESTÁ CERCA DE SU VIVIENDA O TIENE QUE CAMINAR PARA TOMARLO {0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_TRANSPORTE);
                        CamposBase.EnseresMobiliario = string.Format("MOBILIARIO Y ENSERES DOMÉSTICOS {0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_MOBILIARIO);
                        CamposBase.CaractZonaGrupoSec = string.Format("URBANA ( {0} )  SUB - URBANA ( {1} )  RURAL ( {2} )  CRIMINÓGENA (Existencia de bandas o pandillas, sobrepoblación, prostíbulos, cantinas, billares, etc.) ( {3} )",
                            _DatosEstudioTSFederal.CARACT_FS_ZONA == "U" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_ZONA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_ZONA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_ZONA == "N" ? "X" : string.Empty);
                        CamposBase.RelacionMedioExterno = string.Format("RELACIÓN CON MEDIO EXTERNO {0}", _DatosEstudioTSFederal.CARACT_FS_RELACION_MEDIO_EXT);


                        CamposBase.AlgunMiembroPresentaProbConductaPara = string.Format("ALGÚN MIEMBRO DE LA FAMILIA PRESENTA PROBLEMAS DE CONDUCTA PARA Ó ANTISOCIAL: {0} ", _DatosEstudioTSFederal.CARACT_FS_PROBLEMAS_CONDUCTA == "S" ? "SI" : "NO");
                        CamposBase.DescrConducta = string.Format("ESPECIFIQUE: {0}", _DatosEstudioTSFederal.CARACT_FS_PROBLEMAS_CONDUCTA_E);
                        CamposBase.NoPersonasVividoManeraEstable = _DatosEstudioTSFederal.NUM_PAREJAS_ESTABLE;
                        CamposBase.TrabajoAntesReclusion = _DatosEstudioTSFederal.TRABAJO_DESEMP_ANTES;
                        CamposBase.TiempoLaborar = _DatosEstudioTSFederal.TIEMPO_LABORAR;
                        CamposBase.SueldoPercibidoGrupoSecundario = _DatosEstudioTSFederal.SUELDO_PERCIBIDO.HasValue ? _DatosEstudioTSFederal.SUELDO_PERCIBIDO.Value.ToString("c") : string.Empty;
                        CamposBase.OtrasAportacionesDeLaFamilia = string.Format("APARTE DEL INTERNO, SEÑALE OTRAS APORTACIONES ECONÓMICAS DE LA FAMILIA, QUIEN LAS REALIZA Y A CUÁNTO ASCIENDEN {0}", _DatosEstudioTSFederal.APORTACIONES_FAM);
                        CamposBase.DistribucionGastoFamiliar = string.Format("DISTRIBUCIÓN DEL GASTO FAMILIAR: {0}", _DatosEstudioTSFederal.DISTRIBUCION_GASTO_FAM);
                        CamposBase.AlimentacionFamiliar = string.Format("LA ALIMENTACIÓN FAMILIAR EN QUE CONSISTE: {0}", _DatosEstudioTSFederal.ALIMENTACION_FAM);
                        CamposBase.ServiciosCuenta = string.Format("CON QUE SERVICIOS PÚBLICOS CUENTA (LUZ, AGUA, DRENAJE, ETC.): {0}", _DatosEstudioTSFederal.SERVICIOS_PUBLICOS);
                        CamposBase.CuentaOfertaTrabajoTexto = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.OFERTA_TRABAJO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.OFERTA_TRABAJO == "N" ? "X" : string.Empty);
                        CamposBase.EspecifiqueOfertaG = string.Format("EN QUÉ CONSISTE: {0}", _DatosEstudioTSFederal.OFERTA_TRABAJO_CONSISTE);
                        CamposBase.CuentaApoyoFamiliaAlgunaPersona = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.APOYO_FAM_OTROS == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.APOYO_FAM_OTROS == "N" ? "X" : string.Empty);
                        CamposBase.RecibeVisitaFam = string.Format("RECIBE VISITAS DE FAMILIARES \t SI ( {0} )  NO ( {1} ) \n RADICAN EN EL ESTADO: SI ( {2} )  NO ( {3} )",
                            _DatosEstudioTSFederal.VISITA_FAMILIARES == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.VISITA_FAMILIARES == "N" ? "X" : string.Empty,
                            _DatosEstudioTSFederal.RADICAN_ESTADO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.RADICAN_ESTADO == "N" ? "X" : string.Empty);

                        //CamposBase.ParentescoD = _DatosEstudioTSFederal.VISTA_PARENTESCO.HasValue ? _DatosEstudioTSFederal.tipo;
                        if (_DatosEstudioTSFederal.VISTA_PARENTESCO.HasValue)
                        {
                            var _parent = new cTipoReferencia().GetData(c => c.ID_TIPO_REFERENCIA == _DatosEstudioTSFederal.VISTA_PARENTESCO).FirstOrDefault();
                            if (_parent != null)
                                CamposBase.ParentescoD = !string.IsNullOrEmpty(_parent.DESCR) ? _parent.DESCR.Trim() : string.Empty;
                        }
                        else
                            CamposBase.ParentescoD = string.Empty;

                        CamposBase.FrecuenciaG = _DatosEstudioTSFederal.VISITA_FRECUENCIA;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                        CamposBase.VisitadoOtrasPersonas = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.VISITAS_OTROS == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.VISITAS_OTROS == "S" ? "X" : string.Empty);
                        CamposBase.QuienesVisitasTexto = string.Format("QUIENES {0}", _DatosEstudioTSFederal.VISITA_OTROS_QUIIEN);
                        CamposBase.CuentaAvalMoralTexto = string.Format("AVAL MORAL (Nombre): {0})", _DatosEstudioTSFederal.AVAL_MORAL);
                        if (!string.IsNullOrEmpty(_DatosEstudioTSFederal.AVAL_MORAL_PARENTESCO))
                        {
                            short _id = short.Parse(_DatosEstudioTSFederal.AVAL_MORAL_PARENTESCO);
                            var _parent = new cTipoReferencia().GetData(c => c.ID_TIPO_REFERENCIA == _id).FirstOrDefault();
                            if (_parent != null)
                                CamposBase.TextoGenerico1 = CamposBase.TextoGenerico9 = !string.IsNullOrEmpty(_parent.DESCR) ? _parent.DESCR.Trim() : string.Empty;
                        };

                        CamposBase.ConQuienViviraAlSerExternado = string.Format("CON QUIEN VA A VIVIR AL SER EXTERNADO: {0}", _DatosEstudioTSFederal.EXTERNADO_VIVIR_NOMBRE);
                        CamposBase.OpinionInternamiento = string.Format("CUÁL ES SU OPINIÓN ACERCA DE SU INTERNAMIENTO {0}", _DatosEstudioTSFederal.OPINION_INTERNAMIENTO);
                        CamposBase.DeQueFormaInfluenciaEstarPrision = string.Format("DE QUÉ MANERA LE HA INFLUENCIADO SU ESTANCIA EN PRISIÓN {0}", _DatosEstudioTSFederal.INFLUENCIADO_ESTANCIA_PRISION);
                        CamposBase.Diagnostico = string.Format("DIAGNÓSTICO  SOCIAL Y PRONÓSTICO DE EXTERNACIÓN {0}", _DatosEstudioTSFederal.DIAG_SOCIAL_PRONOS);
                        CamposBase.OpinionSobreOtorgamientoBeneficio = string.Format("ANOTE SU OPINIÓN SOBRE LA CONCESIÓN DE BENEFICIOS AL INTERNO EN ESTUDIO {0}", _DatosEstudioTSFederal.OPINION_CONCESION_BENEFICIOS);
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.MatricesRavenTexto = _DatosEstudioTSFederal.TRABAJADORA_SOCIAL;

                        CamposBase.TextoGenerico2 = _DatosEstudioTSFederal.EXTERNADO_CALLE;
                        CamposBase.TextoGenerico3 = _DatosEstudioTSFederal.EXTERNADO_NUMERO;
                        CamposBase.TextoGenerico4 = _DatosEstudioTSFederal.EXTERNADO_COLONIA.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.COLONIA.DESCR) ? _DatosEstudioTSFederal.COLONIA.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.TextoGenerico5 = _DatosEstudioTSFederal.EXTERNADO_CP;
                        CamposBase.TextoGenerico6 = _DatosEstudioTSFederal.EXTERNADO_COLONIA.HasValue ? _DatosEstudioTSFederal.COLONIA.MUNICIPIO != null ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.COLONIA.MUNICIPIO.MUNICIPIO1) ? _DatosEstudioTSFederal.COLONIA.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty : string.Empty;
                        CamposBase.TextoGenerico7 = _DatosEstudioTSFederal.EXTERNADO_COLONIA.HasValue ? _DatosEstudioTSFederal.COLONIA.MUNICIPIO != null ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.COLONIA.MUNICIPIO.MUNICIPIO1) ? _DatosEstudioTSFederal.COLONIA.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty : string.Empty;
                        CamposBase.TextoGenerico8 = _DatosEstudioTSFederal.EXTERNADO_COLONIA.HasValue ? _DatosEstudioTSFederal.COLONIA.MUNICIPIO != null ? _DatosEstudioTSFederal.COLONIA.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.COLONIA.MUNICIPIO.ENTIDAD.DESCR) ? _DatosEstudioTSFederal.COLONIA.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoActividadesProductCapacFueroFederal(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cCapacitacionFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.NoD = string.Format("{0}-{1}{2}-{3}",
                            SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.ID_UB_CAMA);
                        CamposBase.OficioActivDesempenadaAntesReclucion = _DatosEstudioTSFederal.OFICIO_ANTES_RECLUSION.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.OCUPACION.DESCR) ? _DatosEstudioTSFederal.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.SueldoPercibidoGrupoSecundario = _DatosEstudioTSFederal.SALARIO_DEVENGABA_DETENCION.HasValue ? _DatosEstudioTSFederal.SALARIO_DEVENGABA_DETENCION.Value.ToString("c") : string.Empty;
                        CamposBase.ActividadProductivaActualDentroCentro = _DatosEstudioTSFederal.ACTIVIDAD_PRODUC_ACTUAL;
                        CamposBase.AtiendeIndicacionesSuperiores = _DatosEstudioTSFederal.ATIENDE_INDICACIONES == "S" ? "SI" : "NO";
                        CamposBase.SatisfaceActividad = _DatosEstudioTSFederal.SATISFACE_ACTIVIDAD == "S" ? "SI" : "NO";
                        CamposBase.DescuidadoCumplimientoLabores = _DatosEstudioTSFederal.DESCUIDADO_LABORES == "S" ? "SI" : "NO";
                        CamposBase.MotivosTiempoInterrupcionActiv = string.Format("MOTIVOS Y TIEMPO DE LAS INTERRUPCIONES EN LA ACTIVIDADE {0}", _DatosEstudioTSFederal.MOTIVO_TIEMPO_INTERRUP_ACT);
                        CamposBase.RecibioConstancia = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.RECIBIO_CONSTANCIA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.RECIBIO_CONSTANCIA == "N" ? "X" : string.Empty);
                        CamposBase.EspecifiqueAsistenciaGruposFederal = string.Format("EN CASO DE NO HABER ASISTIDO A CURSOS, ESPECIFIQUE EL MOTIVO: {0}", _DatosEstudioTSFederal.NO_CURSOS_MOTIVO);
                        CamposBase.CambiadoActividades = string.Format("¿HA CAMBIADO DE ACTIVIDAD? {0} \t ¿POR QUÉ? {1} ", _DatosEstudioTSFederal.CAMBIO_ACTIVIDAD == "S" ? "SI" : "NO", _DatosEstudioTSFederal.CAMBIO_ACTIVIDAD_POR_QUE);
                        CamposBase.HaProgresadoOficio = !string.IsNullOrEmpty(_DatosEstudioTSFederal.HA_PROGRESADO_OFICIO) ? _DatosEstudioTSFederal.HA_PROGRESADO_OFICIO == "S" ? "SI" : "NO" : "NO";
                        CamposBase.ActitudesHaciaDesempenoActivProduct = string.Format("ACTITUDES HACIA EL DESEMPEÑO DE ACTIVIDADES PRODUCTIVAS: {0}", _DatosEstudioTSFederal.ACTITUDES_DESEMPENO_ACT);
                        CamposBase.CuentaFondoAhorroTexto = string.Format("SI ( {0} )\t NO ( {1} )", _DatosEstudioTSFederal.FONDO_AHORRO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.FONDO_AHORRO == "N" ? "X" : string.Empty);
                        CamposBase.CompensacionRecibeActualmen = _DatosEstudioTSFederal.FONDO_AHORRO_COMPESACION_ACTUA;
                        CamposBase.TotalDiasLaboradosEfect = _DatosEstudioTSFederal.A_TOTAL_DIAS_LABORADOS.HasValue ? _DatosEstudioTSFederal.A_TOTAL_DIAS_LABORADOS.Value.ToString() : string.Empty;
                        CamposBase.DiasOtrosCentros = _DatosEstudioTSFederal.B_DIAS_LABORADOS_OTROS_CERESOS.HasValue ? _DatosEstudioTSFederal.B_DIAS_LABORADOS_OTROS_CERESOS.Value.ToString() : string.Empty;
                        CamposBase.DiasTotalLaborados = _DatosEstudioTSFederal.TOTAL_A_B.HasValue ? _DatosEstudioTSFederal.TOTAL_A_B.Value.ToString() : string.Empty;
                        CamposBase.Conclusion = _DatosEstudioTSFederal.CONCLUSIONES;
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosCursosCapacitacionFueroFederal(PERSONALIDAD Entity)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cCursoCapacitacionFueroFederal>();
                var Datos = new cCapacitacionCursoFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                if (Datos != null && Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleToxicos.Add(new cCursoCapacitacionFueroFederal
                            {
                                Curso = item.CURSO,
                                FechaFin = item.FECHA_TERMINO.HasValue ? item.FECHA_TERMINO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                FechaInicio = item.FECHA_INICIO.HasValue ? item.FECHA_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty
                            });
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet4";
            }

            catch (Exception ex)
            {
            }

            return _respuesta;
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosDiasLaboradosCapacitacionFueroFederal(PERSONALIDAD Entity)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cDiasLaboradosFueroFed>();
                var Datos = new cDiasLaboradosFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                if (Datos != null && Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                            {
                                Mes = MesString(item.MES),
                                Anio = item.ANIO.ToString(),
                                DiasLab = item.DIAS_TRABAJADOS.ToString()
                            });
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
            }
            catch (Exception ex)
            {
            }

            return _respuesta;
        }


        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoInformeActivEducCultFueroFederal(PERSONALIDAD Entity)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cActividadesFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.EscolaridadAnteriorIngreso = _DatosEstudioTSFederal.ESCOLARIDAD_MOMENTO.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.ESCOLARIDAD.DESCR) ? _DatosEstudioTSFederal.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.EstudiosHaRealizadoInternamiento = _DatosEstudioTSFederal.ESTUDIOS_EN_INTERNAMIENTO;
                        CamposBase.EstudiosCursaActualmente = _DatosEstudioTSFederal.ESTUDIOS_ACTUALES;
                        CamposBase.AsisteEscuelaVoluntPuntualidadAsist = string.Format("SI ( {0} ) \t NO ( {1} )   EN CASO NEGATIVO ¿PORQUE? {2}", _DatosEstudioTSFederal.ASISTE_PUNTUAL == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.ASISTE_PUNTUAL == "N" ? "X" : string.Empty, _DatosEstudioTSFederal.ASISTE_PUNTUAL_NO_POR_QUE);
                        CamposBase.AvanceRendimientoAcademico = string.Format("¿CUÁL ES SU AVANCE Y RENDIMIENTO ACADÉMICO {0}", _DatosEstudioTSFederal.AVANCE_RENDIMIENTO_ACADEMINCO);
                        CamposBase.HaSidoPromovido = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.PROMOVIDO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.PROMOVIDO == "N" ? "X" : string.Empty);
                        CamposBase.Abdomen = string.Format("( {0} ) ", _DatosEstudioTSFederal.ALFABE_PRIMARIA == "S" ? "X" : string.Empty);
                        CamposBase.Actitud = string.Format("( {0} ) ", _DatosEstudioTSFederal.PRIMARIA_SECU == "S" ? "X" : string.Empty);
                        CamposBase.ActitudesHaciaDesempenoActivProduct = string.Format("( {0} ) ", _DatosEstudioTSFederal.SECU_BACHILLER == "S" ? "X" : string.Empty);
                        CamposBase.ActitudTomadaAntesEntrevista = string.Format("( {0} ) ", _DatosEstudioTSFederal.BACHILLER_UNI == "S" ? "X" : string.Empty);
                        CamposBase.ActividadProductivaActualDentroCentro = string.Format("( {0} ) ", _DatosEstudioTSFederal.OTRA_ENSENANZA == "S" ? "X" : string.Empty);
                        CamposBase.EspecifiqueOtrasPromociones = _DatosEstudioTSFederal.ESPECIFIQUE;
                        CamposBase.EnsenanzaRecibe = _DatosEstudioTSFederal.OTRA_ENSENANZA;
                        CamposBase.HaImpartidoEnsenanza = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA == "N" ? "X" : string.Empty);
                        CamposBase.DeQueTipoEnsenanza = _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA_TIPO;
                        CamposBase.CuantoTiempoEnsenanzaImpartida = _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA_TIEMPO;
                        CamposBase.Conclusion = _DatosEstudioTSFederal.CONCLUSIONES;
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                };
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return _respuesta;

        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosParticipacionesFueroFederal(PERSONALIDAD Entity)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cParticipacionFueroFederal>();
                var Datos = new cActividadParticipacionFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                if (Datos != null && Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleToxicos.Add(new cParticipacionFueroFederal
                            {
                                AnioD = item.FECHA_1.HasValue ? item.FECHA_1.Value.Year.ToString() : string.Empty,
                                AnioE = item.FECHA_2.HasValue ? item.FECHA_2.Value.Year.ToString() : string.Empty,
                                Programa = item.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.TIPO_PROGRAMA.NOMBRE) ? item.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty,
                                Particip = string.Format("SI ( {0} )  NO ( {1} )", item.PARTICIPACION == "S" ? "X" : string.Empty, item.PARTICIPACION == "N" ? "X" : string.Empty),
                                FecInicio = item.FECHA_1.HasValue ? item.FECHA_1.Value.ToString("dd/MM/yyyy") : string.Empty,
                                FecFin = item.FECHA_2.HasValue ? item.FECHA_2.Value.ToString("dd/MM/yyyy") : string.Empty,
                                MesD = item.FECHA_1.HasValue ? MesString(short.Parse(item.FECHA_1.Value.Month.ToString())) : string.Empty,
                                MesE = item.FECHA_2.HasValue ? MesString(short.Parse(item.FECHA_2.Value.Month.ToString())) : string.Empty,
                                DiaD = item.FECHA_1.HasValue ? DiaStringByDayOfWeek((int)(item.FECHA_1.Value.DayOfWeek)) : string.Empty,
                                DiaE = item.FECHA_2.HasValue ? DiaStringByDayOfWeek((int)(item.FECHA_2.Value.DayOfWeek)) : string.Empty
                            });
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
            }
            catch (Exception ex)
            {
            }

            return _respuesta;
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoInformeVigilanciaFueroFederal(PERSONALIDAD Entity)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cVigilanciaFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.FechaIngreso = _DatosEstudioTSFederal.FECHA_INGRESO.HasValue ? _DatosEstudioTSFederal.FECHA_INGRESO.Value.ToString("dd/MM/yyyy") : string.Empty;
                        CamposBase.CeresoProcede = _DatosEstudioTSFederal.CENTRO_DONDE_PROCEDE;
                        CamposBase.ConductaObservoEnElMismo = string.Format("EXCELENTE ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ( {3} )", _DatosEstudioTSFederal.CONDUCTA == "E" ? "X" : string.Empty
                            , _DatosEstudioTSFederal.CONDUCTA == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA == "M" ? "X" : string.Empty);
                        CamposBase.MotivoTraslado = _DatosEstudioTSFederal.MOTIVO_TRASLADO;
                        CamposBase.ConductaSuperioresTexto = string.Format("EXCELENTE ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ( {3} )", _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "E" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "B" ? "X" : string.Empty
                            , _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "M" ? "X" : string.Empty);
                        CamposBase.RelacionCompanieros = _DatosEstudioTSFederal.RELACION_COMPANEROS;
                        CamposBase.DescrConducta = _DatosEstudioTSFederal.DESCRIPCION_CONDUCTA;
                        CamposBase.HigienePersonalTexto = string.Format("BUENA ( {0} ) REGULAR ( {1} ) MALA ( {2} )", _DatosEstudioTSFederal.HIGIENE_PERSONAL == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_PERSONAL == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_PERSONAL == "M" ? "X" : string.Empty);
                        CamposBase.HigieneCeldaTexto = string.Format("BUENA ( {0} ) REGULAR ( {1} ) MALA ( {2} )", _DatosEstudioTSFederal.HIGIENE_CELDA == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_CELDA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_CELDA == "M" ? "X" : string.Empty);
                        CamposBase.RecibeVisitasTexto = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.VISITA_RECIBE == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.VISITA_RECIBE == "N" ? "X" : string.Empty);
                        CamposBase.FrecuenciaG = string.Format("FRECUENCIA: {0}", _DatosEstudioTSFederal.VISITA_FRECUENCIA);
                        CamposBase.DeQuienesVisita = string.Format(" DE QUIENES: {0}", _DatosEstudioTSFederal.VISITA_QUIENES);
                        CamposBase.ConductaFamilia = string.Format("BUENA ( {0} ) REGULAR ( {1} ) MALA ( {2} )", _DatosEstudioTSFederal.CONDUCTA_FAMILIA == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_FAMILIA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_FAMILIA == "M" ? "X" : string.Empty);
                        CamposBase.EstimulosBuenaCond = _DatosEstudioTSFederal.ESTIMULOS_BUENA_CONDUCTA;
                        CamposBase.ClasificConsudtaGral = string.Format("EXCELENTE ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ( {3} )", _DatosEstudioTSFederal.CONDUCTA_GENERAL == "E" ? "X" : string.Empty
                            , _DatosEstudioTSFederal.CONDUCTA_GENERAL == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_GENERAL == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_GENERAL == "M" ? "X" : string.Empty);
                        CamposBase.Conclusion = string.Format("CONCLUSIONES: {0}", _DatosEstudioTSFederal.CONCLUSIONES);
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCorrectivosVigiFueroFederal(PERSONALIDAD Entity)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cCorrectivosVigilanciaFF>();
                var Datos = new cCorrectivoFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                if (Datos != null && Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleToxicos.Add(new cCorrectivosVigilanciaFF
                            {
                                Anio = item.FECHA.HasValue ? item.FECHA.Value.Year.ToString() : string.Empty,
                                Motivo = item.MOTIVO,
                                ResolucionH = item.RESOLUCION,
                                Mes = item.FECHA.HasValue ? MesString(short.Parse(item.FECHA.Value.Month.ToString())) : string.Empty,
                                Dia = item.FECHA.HasValue ? DiaStringByDayOfWeek((int)(item.FECHA.Value.DayOfWeek)) : string.Empty
                            });
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return _respuesta;
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoCriminologicoFueroFederal(PERSONALIDAD Entity)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cCriminologicoFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.AliasInterno = _DatosEstudioTSFederal.SOBRENOMBRE;
                        CamposBase.VersionDelitoSegunInterno = _DatosEstudioTSFederal.P1_VERSION_INTERNO;
                        CamposBase.CaractPersonalidadRelDel = _DatosEstudioTSFederal.P2_PERSONALIDAD;
                        CamposBase.ReqValoracionVictim = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.P3_VALORACION == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.P3_VALORACION == "N" ? "X" : string.Empty);
                        CamposBase.AntecedentesParasoc = _DatosEstudioTSFederal.ANTECEDENTES_PARA_ANTI_SOCIALE;
                        CamposBase.ClasificCriminologTexto = string.Format("PRIMODELINCUENTE: {0} \n REINCIDENTE ESPECÍFICO: {1} \t HABITUAL: {2} \n REINCIDENTE GENÉRICO: {3} \t PROFESIONAL: {4}",
                            _DatosEstudioTSFederal.P5_PRIMODELINCUENTE == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.P5_ESPECIFICO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.P5_HABITUAL == "S" ? "X" : string.Empty,
                            _DatosEstudioTSFederal.P5_GENERICO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.P5_PROFESIONAL == "S" ? "X" : string.Empty);
                        CamposBase.Criminogenesis = _DatosEstudioTSFederal.P6_CRIMINOGENESIS;
                        CamposBase.EgocentrismoTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_EGOCENTRISMO == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_EGOCENTRISMO == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_EGOCENTRISMO == "B" ? "X" : string.Empty);
                        CamposBase.LabilidadAfectivaTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_LABILIDAD == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_LABILIDAD == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_LABILIDAD == "B" ? "X" : string.Empty);
                        CamposBase.AgresividadTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_AGRESIVIDAD == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_AGRESIVIDAD == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_AGRESIVIDAD == "B" ? "X" : string.Empty);
                        CamposBase.IndiferenciaAfectTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_INDIFERENCIA == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_INDIFERENCIA == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_INDIFERENCIA == "B" ? "X" : string.Empty);
                        CamposBase.ResultTratamInstitucional = _DatosEstudioTSFederal.P8_RESULTADO_TRATAMIENTO;
                        CamposBase.EstadoPeligrosidadActual = string.Format("MINIMO ( {0} )  MÍNIMO MEDIO ( {1} )  MEDIO ( {2} )  MEDIO ALTO ( {3} )  ALTO ( {4} )", _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("1") ? "X" : string.Empty
                            , _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("2") ? "X" : string.Empty, _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("3") ? "X" : string.Empty, _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("4") ? "X" : string.Empty
                            , _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("5") ? "X" : string.Empty);
                        CamposBase.OpinionSobreOtorgamientoBeneficio = _DatosEstudioTSFederal.P10_OPINION;
                        CamposBase.ReqContinuacionTratTexto = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.P10_CONTINUAR_TRATAMIENTO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.P10_CONTINUAR_TRATAMIENTO == "N" ? "X" : string.Empty);
                        CamposBase.Abdomen = string.Concat("EN CASO AFIRMATIVO ESPECIFICAR: ", _DatosEstudioTSFederal.P10_CONTINUAR_SI_ESPECIFICAR);
                        CamposBase.Actitud = string.Concat("EN CASO NEGATIVO ESPECIFICAR: ", _DatosEstudioTSFederal.P10_CONTINUAR_NO_ESPECIFICAR);
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.MatricesRavenTexto = _DatosEstudioTSFederal.CRIMINOLOGO;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                        CamposBase.ProbabilidadReincidencia = string.Format("ALTO( {0} )  MEDIO ({1})  BAJO ( {2} )",
                            _DatosEstudioTSFederal.P9_PRONOSTICO == "A" ? "X" : string.Empty,
                            _DatosEstudioTSFederal.P9_PRONOSTICO == "M" ? "X" : string.Empty,
                            _DatosEstudioTSFederal.P9_PRONOSTICO == "B" ? "X" : string.Empty);
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Datos Ventanas Fuero Federal
        private void ActaConsejoTecnicoInterd()
        {
            try
            {
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var ActaAnterior = new cActaConsejoTecnicoFueroFederal().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault();
                    NombreImputadoFF = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                    Sentencia = CalcularSentencia().ToUpper();
                    var _estado = new cEntidad().GetData(x => x.ID_ENTIDAD == Parametro.ESTADO).FirstOrDefault();
                    EstadoActual = _estado != null ? !string.IsNullOrEmpty(_estado.DESCR) ? _estado.DESCR.Trim() : string.Empty : string.Empty;
                    var CausaPenal = new cCausaPenal().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO).FirstOrDefault();
                    if (CausaPenal != null)
                    {
                        var _Sentencia = new cSentencia().GetData(c => c.ID_INGRESO == SelectIngreso.ID_INGRESO && c.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && c.ID_CAUSA_PENAL == CausaPenal.ID_CAUSA_PENAL).FirstOrDefault();
                        if (_Sentencia != null)
                        {
                            var _SentenciaDelito = new cSentenciaDelito().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_SENTENCIA == _Sentencia.ID_SENTENCIA);
                            if (_SentenciaDelito != null && _SentenciaDelito.Any())
                                foreach (var item in _SentenciaDelito)
                                    Delito += string.Format("{0}, ", !string.IsNullOrEmpty(item.DESCR_DELITO) ? item.DESCR_DELITO.Trim() : string.Empty);

                            var InicioComp = new cSentencia().GetData(x => x.ID_SENTENCIA == _Sentencia.ID_SENTENCIA && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault();
                            if (InicioComp != null)
                                APartirDe = InicioComp.FEC_INICIO_COMPURGACION.HasValue ? InicioComp.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty;
                        }

                        else
                            Delito = APartirDe = string.Empty;
                    }

                    if (ActaAnterior == null)
                    {
                        LstAreasTec = new ObservableCollection<PFF_ACTA_DETERMINO>();
                        var _Centro = new cCentro().GetData(x => x.ID_CENTRO == (short)GlobalVar.gCentro).FirstOrDefault();
                        EnSesionDeFecha = Fechas.GetFechaDateServer;
                        ExpedienteImputadoFF = string.Format("{0} / {1}", SelectIngreso.IMPUTADO.ID_ANIO, SelectIngreso.IMPUTADO.ID_IMPUTADO);
                        LugarActa = LugarPiePagina();
                        IdCentro = GlobalVar.gCentro;
                        DirectorCentro = _Centro != null ? !string.IsNullOrEmpty(_Centro.DIRECTOR) ? _Centro.DIRECTOR.Trim() : string.Empty : string.Empty;
                        FechaActa = Fechas.GetFechaDateServer;
                        TramiteDescripcion = VotosR = ActuacionR = LugarActaFF = string.Empty;
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
                }
            }

            catch (Exception exc)
            {
                throw;
            }
        }
        private void EstudioMedicoFueroFederal(PERSONALIDAD Entity)
        {
            try
            {
                var _Respuesta = new PFF_ESTUDIO_MEDICO();

                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var _EstudioMedicoFederal = new cEstudioMedicoFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                    if (_EstudioMedicoFederal == null)
                    {
                        DescripcionTatuajesCicatrRecientes = AliasImputadoMedicoFederal = DescripcionDelitoMedicoFederal = AntecedentesHeredoFam = EspecifiqueOtraDependencia = AntecedenterPersonalesNoPato = string.Empty;
                        LoadListasFederales();
                        NombreImputadoMedicoFederal = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
!string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                        var _Alias = new cAlias().ObtenerTodosXImputado(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO);
                        if (_Alias != null && _Alias.Any())
                            foreach (var item in _Alias)
                            {
                                string _Nombre = string.Empty;
                                _Nombre = string.Format("{0} {1} {2}",
                                        !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Trim() : string.Empty);

                                if ((AliasImputadoMedicoFederal.Length + _Nombre.Length) < 100)
                                    AliasImputadoMedicoFederal += string.Format("{0}, ", _Nombre);
                            };

                        EdadImputadoMedicoFederal = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA);
                        EstatusCivilImputado = SelectIngreso.ID_ESTADO_CIVIL.HasValue ? SelectIngreso.ID_ESTADO_CIVIL.Value.ToString() : string.Empty;
                        var _EstadoNac = new cEntidad().GetData(x => x.ID_ENTIDAD == SelectIngreso.IMPUTADO.NACIMIENTO_ESTADO).FirstOrDefault();
                        var _MunicipioNac = new cMunicipio().GetData(x => x.ID_MUNICIPIO == SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO && x.ID_ENTIDAD == _EstadoNac.ID_ENTIDAD).FirstOrDefault();
                        OriginarioImputado = string.Format("{0}, {1}",
                            _MunicipioNac != null ? !string.IsNullOrEmpty(_MunicipioNac.MUNICIPIO1) ? _MunicipioNac.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                            _EstadoNac != null ? !string.IsNullOrEmpty(_EstadoNac.DESCR) ? _EstadoNac.DESCR.Trim() : string.Empty : string.Empty);

                        OcupacionAnteriorImputado = SelectIngreso.ID_OCUPACION.HasValue ? SelectIngreso.ID_OCUPACION : -1;

                        var CausaPenal = new cCausaPenal().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO).FirstOrDefault();
                        if (CausaPenal != null)
                        {
                            var _Sentencia = new cSentencia().GetData(c => c.ID_INGRESO == SelectIngreso.ID_INGRESO && c.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && c.ID_CAUSA_PENAL == CausaPenal.ID_CAUSA_PENAL).FirstOrDefault();
                            if (_Sentencia != null)
                            {
                                var _SentenciaDelito = new cSentenciaDelito().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_SENTENCIA == _Sentencia.ID_SENTENCIA);
                                if (_SentenciaDelito != null && _SentenciaDelito.Any())
                                    foreach (var item in _SentenciaDelito)
                                        DescripcionDelitoMedicoFederal += string.Format("{0}, ", !string.IsNullOrEmpty(item.DESCR_DELITO) ? item.DESCR_DELITO.Trim() : string.Empty);
                            };
                        };

                        AntecedentesPersonalesNoPatologicos = AntecedentesHeredoFam = string.Empty;
                        SentenciaDelito = CalcularSentencia().ToUpper();
                        var _HistoriaClinicaByImputado = new cHistoriaClinica().GetData(x => x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_CENTRO == GlobalVar.gCentro && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO);
                        if (_HistoriaClinicaByImputado.Any())
                        {
                            var _Historia = _HistoriaClinicaByImputado.FirstOrDefault();
                            if (_Historia.HISTORIA_CLINICA_FAMILIAR != null && _Historia.HISTORIA_CLINICA_FAMILIAR.Any())
                            {
                                foreach (var item in _Historia.HISTORIA_CLINICA_FAMILIAR)
                                {
                                    string _Caracteres = string.Empty;
                                    _Caracteres = string.Format("FAMILIAR: {0}, {1} ALERGIAS, {2} CA, {3} CARDIACOS, {4} DIABETES, {5} EPILEPSIA, {6} HIPERTENSO, {7} MENTALES, {8} TB \n",
                                        item.ID_TIPO_REFERENCIA.HasValue ? !string.IsNullOrEmpty(item.TIPO_REFERENCIA.DESCR) ? item.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                        item.AHF_ALERGIAS == "S" ? "SI" : "NO", item.AHF_CA == "S" ? "SI" : "NO", item.AHF_CARDIACOS == "S" ? "SI" : "NO", item.AHF_DIABETES == "S" ? "SI" : "NO",
                                        item.AHF_EPILEPSIA == "S" ? "SI" : "NO", item.AHF_HIPERTENSIVO == "S" ? "SI" : "NO", item.AHF_MENTALES == "S" ? "SI" : "NO", item.AHF_TB == "S" ? "SI" : "NO"
                                        );

                                    if (AntecedentesHeredoFam.Length + _Caracteres.Length < 500)
                                        AntecedentesHeredoFam += _Caracteres;
                                };

                                string NoPatos = string.Format("{0} ALCOHOLISMO,  ALIMENTACION: {1}, HABITACION: {2}, NACIMIENTO: {3}, {4} TABAQUISMO, {5} TOXICOMANIAS",
                                    _Historia.APNP_ALCOHOLISMO == "S" ? "SI" : "NO", _Historia.APNP_ALIMENTACION, _Historia.APNP_HABITACION, _Historia.APNP_NACIMIENTO, _Historia.APNP_TABAQUISMO == "S" ? "SI" : "NO", _Historia.APNP_TOXICOMANIAS == "S" ? "SI" : "NO");

                                if (AntecedenterPersonalesNoPato.Length + NoPatos.Length < 500)
                                    AntecedenterPersonalesNoPato += NoPatos;
                            };

                            if (_Historia.HISTORIA_CLINICA_PATOLOGICOS != null && _Historia.HISTORIA_CLINICA_PATOLOGICOS.Any())
                            {
                                DescripcionAntecedentesPatoMedicoFederal = string.Empty;
                                foreach (var item in _Historia.HISTORIA_CLINICA_PATOLOGICOS)
                                {
                                    if (item.PATOLOGICO_CAT != null)
                                        if (!string.IsNullOrEmpty(item.PATOLOGICO_CAT.DESCR))
                                        {
                                            if (DescripcionAntecedentesPatoMedicoFederal.Contains(item.PATOLOGICO_CAT.DESCR))
                                                continue;

                                            if ((DescripcionAntecedentesPatoMedicoFederal.Length + item.PATOLOGICO_CAT.DESCR.Trim().Length) < 500)
                                                DescripcionAntecedentesPatoMedicoFederal += string.Format("{0}, \n ", item.PATOLOGICO_CAT.DESCR.Trim());
                                        };
                                };
                            };
                        };

                        LstSustToxicas = new ObservableCollection<PFF_SUSTANCIA_TOXICA>();
                        var _EmiIngreso = SelectIngreso.EMI_INGRESO;
                        if (_EmiIngreso != null && _EmiIngreso.Any())
                            foreach (var item in _EmiIngreso)
                                if (item.EMI != null)
                                    if (item.EMI.EMI_USO_DROGA.Any())
                                    {
                                        foreach (var item2 in item.EMI.EMI_USO_DROGA)
                                        {
                                            var ToxicaNueva = new PFF_SUSTANCIA_TOXICA()
                                            {
                                                ID_DROGA = item2.ID_DROGA,
                                                ID_ANIO = SelectIngreso.ID_ANIO,
                                                DROGA = item2.DROGA,
                                                PERIODICIDAD = item2.DROGA_FRECUENCIA != null ? !string.IsNullOrEmpty(item2.DROGA_FRECUENCIA.DESCR) ? item2.DROGA_FRECUENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                                EDAD_INICIO = item2.EDAD_INICIO,
                                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                                ID_INGRESO = SelectIngreso.ID_INGRESO
                                            };

                                            LstSustToxicas.Add(ToxicaNueva);
                                        }
                                    }

                        #region Programas
                        ListGruposSocioFamComun = new ObservableCollection<PFC_VI_GRUPO>();
                        ListFortalecimientoSocioFamComun = new ObservableCollection<PFC_VI_GRUPO>();
                        var GruposIngreso = SelectIngreso.GRUPO_PARTICIPANTE;
                        if (GruposIngreso != null && GruposIngreso.Any())
                        {
                            IsOtrosGruChecked = true;
                            var _gruposRefinados = GruposIngreso.Where(c => c.ESTATUS == 2 || c.ESTATUS == 5);
                            foreach (var item in _gruposRefinados)
                            {
                                if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_DESHABITUAMIENTO && item.ID_ACTIVIDAD == (short)eTipoActividad.ALCOHLICOS_ANONIMOS)
                                    IsAlcohlicosAnonChecked = true;

                                if (item.ID_TIPO_PROGRAMA == (short)eGrupos.RECONSTRUCCION_PERSONAL && item.ID_ACTIVIDAD == (short)eTipoActividad.DESINTOXICACION)
                                    IsFarmacoDepenChecked = true;

                                if (!EspecifiqueOtraDependencia.Contains(item.ACTIVIDAD.DESCR) && (item.ACTIVIDAD.DESCR.Length + EspecifiqueOtraDependencia.Length) <= (int)eMaxLengthCampos.QUINIENTOS)
                                    EspecifiqueOtraDependencia += string.Format("{0} \n ", item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty);
                            };
                        };

                        #endregion

                        #region Senias particulares
                        IQueryable<SENAS_PARTICULARES> Senias = SelectIngreso.IMPUTADO.SENAS_PARTICULARES.AsQueryable();
                        if (Senias.Any() && Senias != null)
                            foreach (var item in Senias)
                            {
                                if (DescripcionTatuajesCicatrRecientes == null)
                                    DescripcionTatuajesCicatrRecientes = string.Empty;

                                if ((DescripcionTatuajesCicatrRecientes.Length + item.SIGNIFICADO.Length) < (int)eMaxLengthCampos.QUINIENTOS)
                                    DescripcionTatuajesCicatrRecientes += string.Format("{0} \n", !string.IsNullOrEmpty(item.SIGNIFICADO) ? item.SIGNIFICADO.Trim() : string.Empty);
                            };
                        #endregion
                        LugarMedico = LugarPiePagina();
                        OcupacionActualImputado = -1;
                        DescripcionPadecMedicoFederal = InterrogatorioAparatosSist = ExploracionFisicaCabezaCuello = Torax = Abdomen = OrganosGenit = Extemidades =
                        TensionFederalUno = TensionFederalDos = TemperaturaFed = PulsoFed = RespiracionFed = EstaturaFed = Diagnostico = TerapeuticaImplementadaYResult = Conclusion = string.Empty;
                        FechaEstudioMedicoFederal = Fechas.GetFechaDateServer;
                    }

                    else
                    {
                        EdadImputadoMedicoFederal = _EstudioMedicoFederal.EDAD;
                        EstatusCivilImputado = _EstudioMedicoFederal.EDO_CIVIL;
                        OriginarioImputado = _EstudioMedicoFederal.ORIGINARIO_DE;
                        OcupacionAnteriorImputado = _EstudioMedicoFederal.OCUPACION_ANT.HasValue ? _EstudioMedicoFederal.OCUPACION_ANT : -1;
                        DescripcionDelitoMedicoFederal = _EstudioMedicoFederal.DELITO;
                        SentenciaDelito = _EstudioMedicoFederal.SENTENCIA;
                        AntecedentesHeredoFam = _EstudioMedicoFederal.ANTE_HEREDO_FAM;
                        AntecedenterPersonalesNoPato = _EstudioMedicoFederal.ANTE_PERSONAL_NO_PATOLOGICOS;
                        DescripcionAntecedentesPatoMedicoFederal = _EstudioMedicoFederal.ANTE_PATOLOGICOS;
                        OcupacionActualImputado = _EstudioMedicoFederal.OCUPACION_ACT.HasValue ? _EstudioMedicoFederal.OCUPACION_ACT : -1;
                        DescripcionPadecMedicoFederal = _EstudioMedicoFederal.PADECIMIENTO_ACTUAL;
                        InterrogatorioAparatosSist = _EstudioMedicoFederal.INTERROGATORIO_APARATOS;
                        ExploracionFisicaCabezaCuello = _EstudioMedicoFederal.EXP_FIS_CABEZA_CUELLO;
                        Torax = _EstudioMedicoFederal.EXP_FIS_TORAX;
                        Abdomen = _EstudioMedicoFederal.EXP_FIS_ABDOMEN;
                        OrganosGenit = _EstudioMedicoFederal.EXP_FIS_GENITALES;
                        Extemidades = _EstudioMedicoFederal.EXP_FIS_EXTREMIDADES;
                        if (!string.IsNullOrEmpty(_EstudioMedicoFederal.TA) ? _EstudioMedicoFederal.TA.Trim().Contains("/") : false)
                        {
                            string dato1 = _EstudioMedicoFederal.TA.Split('/')[0];
                            string dato2 = _EstudioMedicoFederal.TA.Split('/')[1];
                            TensionFederalUno = dato1.Trim();
                            TensionFederalDos = dato2.Trim();
                        }

                        DescripcionTatuajesCicatrRecientes = _EstudioMedicoFederal.TATUAJES;
                        TemperaturaFed = _EstudioMedicoFederal.TEMPERATURA;
                        PulsoFed = _EstudioMedicoFederal.PULSO;
                        RespiracionFed = _EstudioMedicoFederal.RESPIRACION;
                        EstaturaFed = _EstudioMedicoFederal.ESTATURA;
                        LugarMedico = _EstudioMedicoFederal.LUGAR;
                        Diagnostico = _EstudioMedicoFederal.DIAGNOSTICO;
                        TerapeuticaImplementadaYResult = _EstudioMedicoFederal.RESULTADOS_OBTENIDOS;
                        Concentracion = _EstudioMedicoFederal.CONCLUSION;
                        FechaEstudioMedicoFederal = _EstudioMedicoFederal.FECHA;
                        AliasImputadoMedicoFederal = _EstudioMedicoFederal.ALIAS;
                        Conclusion = _EstudioMedicoFederal.CONCLUSION;
                        NombreImputadoMedicoFederal = _EstudioMedicoFederal.NOMBRE;
                        IsFarmacoDepenChecked = !string.IsNullOrEmpty(_EstudioMedicoFederal.ASIST_FARMACODEPENDENCIA) ? _EstudioMedicoFederal.ASIST_FARMACODEPENDENCIA == "S" ? true : false : false;
                        IsAlcohlicosAnonChecked = !string.IsNullOrEmpty(_EstudioMedicoFederal.ASIST_AA) ? _EstudioMedicoFederal.ASIST_AA == "S" ? true : false : false;
                        IsOtrosGruChecked = !string.IsNullOrEmpty(_EstudioMedicoFederal.ASIST_OTROS) ? _EstudioMedicoFederal.ASIST_OTROS == "S" ? true : false : false;
                        EspecifiqueOtraDependencia = _EstudioMedicoFederal.ASIST_OTROS_ESPECIF;
                        LstSustToxicas = new ObservableCollection<PFF_SUSTANCIA_TOXICA>();

                        if (_EstudioMedicoFederal.PFF_SUSTANCIA_TOXICA != null && _EstudioMedicoFederal.PFF_SUSTANCIA_TOXICA.Any())
                            foreach (var item in _EstudioMedicoFederal.PFF_SUSTANCIA_TOXICA)
                                LstSustToxicas.Add(item);
                    }
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private void EstudioPsicologicoFueroFederalDatos(PERSONALIDAD Entity)
        {
            try
            {
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    SobreNombFF = string.Empty;
                    var _EstudioPsicologicoRealizado = new cEstudioPsicologicoFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                    if (_EstudioPsicologicoRealizado == null)
                    {
                        NombreImpFF = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                        var _Alias = new cAlias().ObtenerTodosXImputado(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO);
                        if (_Alias != null && _Alias.Any())
                            foreach (var item in _Alias)
                            {
                                if (string.IsNullOrEmpty(SobreNombFF))
                                    SobreNombFF = string.Empty;

                                decimal CantidadCaracteres = (!string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Length : decimal.Zero) + (!string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Length : decimal.Zero) + (!string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Length : decimal.Zero);
                                if ((SobreNombFF.Length + CantidadCaracteres <= 100))
                                    SobreNombFF += string.Format("{0} {1} {2} ,", !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Trim() : string.Empty);
                            };

                        EdadImpFF = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA);

                        var CausaPenal = new cCausaPenal().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO).FirstOrDefault();
                        if (CausaPenal != null)
                        {
                            var _Sentencia = new cSentencia().GetData(c => c.ID_INGRESO == SelectIngreso.ID_INGRESO && c.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && c.ID_CAUSA_PENAL == CausaPenal.ID_CAUSA_PENAL).FirstOrDefault();
                            if (_Sentencia != null)
                            {
                                var _SentenciaDelito = new cSentenciaDelito().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_SENTENCIA == _Sentencia.ID_SENTENCIA);
                                if (_SentenciaDelito != null && _SentenciaDelito.Any())
                                    foreach (var item in _SentenciaDelito)
                                        DelifoFFl += string.Format("{0}, ", !string.IsNullOrEmpty(item.DESCR_DELITO) ? item.DESCR_DELITO.Trim() : string.Empty);
                            };
                        };

                        ActitudTomadaEntrev = ExMentFF = LugarPsicoF = PruebasAplic = NivelIntelec = CoeficienteInt = IndiceLesionOrg = DinamicaPersonIngreso = DinamicaPersActual = ResultTratPropArea = InternoFF = ExternoFF = EspecifiqueContTrat = PronosticoReinsercionSocial = OpinionSobreBeneficio = IdRequiereContTratam = string.Empty;
                        FecEstudioPsicoFF = Fechas.GetFechaDateServer;
                        LugarPsico = LugarPiePagina();
                    }

                    else
                    {
                        NombreImpFF = _EstudioPsicologicoRealizado.NOMBRE;
                        SobreNombFF = _EstudioPsicologicoRealizado.SOBRENOMBRE;
                        EdadImpFF = _EstudioPsicologicoRealizado.EDAD;
                        DelifoFFl = _EstudioPsicologicoRealizado.DELITO;
                        ActitudTomadaEntrev = _EstudioPsicologicoRealizado.ACTITUD;
                        ExMentFF = _EstudioPsicologicoRealizado.EXAMEN_MENTAL;
                        PruebasAplic = _EstudioPsicologicoRealizado.PRUEBAS_APLICADAS;
                        NivelIntelec = _EstudioPsicologicoRealizado.NIVEL_INTELECTUAL;
                        CoeficienteInt = _EstudioPsicologicoRealizado.CI;
                        IndiceLesionOrg = _EstudioPsicologicoRealizado.INDICE_LESION_ORGANICA;
                        DinamicaPersonIngreso = _EstudioPsicologicoRealizado.DINAM_PERSON_INGRESO;
                        DinamicaPersActual = _EstudioPsicologicoRealizado.DINAM_PERSON_ACTUAL;
                        ResultTratPropArea = _EstudioPsicologicoRealizado.RESULT_TRATAMIENTO;
                        InternoFF = _EstudioPsicologicoRealizado.INTERNO;
                        ExternoFF = _EstudioPsicologicoRealizado.EXTERNO;
                        EspecifiqueContTrat = _EstudioPsicologicoRealizado.ESPECIFIQUE;
                        PronosticoReinsercionSocial = _EstudioPsicologicoRealizado.PRONOSTICO_REINTEGRACION;
                        OpinionSobreBeneficio = _EstudioPsicologicoRealizado.OPINION;
                        IdRequiereContTratam = !string.IsNullOrEmpty(_EstudioPsicologicoRealizado.REQ_CONT_TRATAMIENTO) ? _EstudioPsicologicoRealizado.REQ_CONT_TRATAMIENTO : string.Empty;
                        FecEstudioPsicoFF = _EstudioPsicologicoRealizado.FECHA;
                        LugarPsico = _EstudioPsicologicoRealizado.LUGAR;
                    }
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private void InformeActividadesCapacitacionFueroFederalInfo(PERSONALIDAD Entity)
        {
            try
            {
                PrepararListas();
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var _InformeAnterior = new cCapacitacionFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                    if (_InformeAnterior == null)
                    {
                        NombreInternoCpaFF = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                        SeccionInterno = string.Format("{0}-{1}{2}-{3}",
                            SelectIngreso.CAMA != null && SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA != null && SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA != null && SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty,
                            SelectIngreso.ID_UB_CAMA);

                        ActivEnCentroActual = string.Empty;
                        OficioActivDesempenadaAntesR = -1;
                        var _EmiByIngreso = SelectIngreso.EMI_INGRESO.Any() ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI != null ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI.EMI_ULTIMOS_EMPLEOS : null : null;
                        if (_EmiByIngreso != null)
                        {
                            var _UltimoEmpleo = _EmiByIngreso.FirstOrDefault(x => x.ULTIMO_EMPLEO_ANTES_DETENCION == "S");
                            if (_UltimoEmpleo != null)
                                OficioActivDesempenadaAntesR = _UltimoEmpleo.ID_OCUPACION;
                        };

                        LstCursosCapacitacionFederal = new ObservableCollection<PFF_CAPACITACION_CURSO>();
                        var GruposIngreso = SelectIngreso.GRUPO_PARTICIPANTE;
                        if (GruposIngreso != null && GruposIngreso.Any())
                        {
                            foreach (var item in GruposIngreso)
                                if (item.GRUPO != null)
                                    if (!ActivEnCentroActual.Contains(item.GRUPO.DESCR.Trim()))
                                        ActivEnCentroActual += string.Format("{0}, ", !string.IsNullOrEmpty(item.GRUPO.DESCR) ? item.GRUPO.DESCR.Trim() : string.Empty);

                            var _Refinados = GruposIngreso.Where(c => c.ESTATUS == 2 || c.ESTATUS == 5);
                            foreach (var item in _Refinados)
                            {
                                if (item.GRUPO != null)
                                {
                                    PFF_CAPACITACION_CURSO _curso = new PFF_CAPACITACION_CURSO()
                                    {
                                        CURSO = !string.IsNullOrEmpty(item.GRUPO.DESCR) ? item.GRUPO.DESCR.Trim() : string.Empty,
                                        FECHA_INICIO = item.GRUPO.FEC_INICIO,
                                        FECHA_TERMINO = item.GRUPO.FEC_FIN,
                                        ID_ANIO = SelectIngreso.ID_ANIO,
                                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                        ID_INGRESO = SelectIngreso.ID_INGRESO
                                    };

                                    LstCursosCapacitacionFederal.Add(_curso);
                                };
                            };
                        };

                        IdAtiendeIndicacionesSup = MotivosTiempoInterrupcionesActividad = LugarActaFF = EspecifiqueNoCursos = ExpecifiqueCambioAct = ActitudesHaciaDesempenioActivProd = ConclusionesActivProdCapac = IdProgresoOficio = IdCuentaFondoA = IdSatisfaceActiv = IdDescuidadoCumplimientoLab = IdRecibioConstancia = IdCambiadoActiv = EspecifiqueCompensacion = string.Empty;
                        SalarioPercib = 0;
                        DiasLaboradosEfectivos = 0;
                        DiasOtrosCentros = 0;
                        DiasTotalAB = 0;
                        FecCapacitFF = Fechas.GetFechaDateServer;
                        LugarProd = LugarPiePagina();
                        LstDiasLaborados = new ObservableCollection<PFF_DIAS_LABORADO>();
                    }

                    else
                    {
                        DiasLaboradosEfectivos = 0;
                        NombreInternoCpaFF = _InformeAnterior.NOMBRE;
                        SeccionInterno = _InformeAnterior.SECCION;
                        DiasTotalAB = _InformeAnterior.TOTAL_A_B;
                        DiasOtrosCentros = _InformeAnterior.B_DIAS_LABORADOS_OTROS_CERESOS;
                        OficioActivDesempenadaAntesR = _InformeAnterior.OFICIO_ANTES_RECLUSION.HasValue ? _InformeAnterior.OFICIO_ANTES_RECLUSION != -1 ? _InformeAnterior.OFICIO_ANTES_RECLUSION : -1 : -1;
                        IdAtiendeIndicacionesSup = !string.IsNullOrEmpty(_InformeAnterior.ATIENDE_INDICACIONES) ? _InformeAnterior.ATIENDE_INDICACIONES : string.Empty;
                        IdSatisfaceActiv = !string.IsNullOrEmpty(_InformeAnterior.SATISFACE_ACTIVIDAD) ? _InformeAnterior.SATISFACE_ACTIVIDAD : string.Empty;
                        IdDescuidadoCumplimientoLab = !string.IsNullOrEmpty(_InformeAnterior.DESCUIDADO_LABORES) ? _InformeAnterior.DESCUIDADO_LABORES : string.Empty;
                        IdRecibioConstancia = !string.IsNullOrEmpty(_InformeAnterior.RECIBIO_CONSTANCIA) ? _InformeAnterior.RECIBIO_CONSTANCIA : string.Empty;
                        IdCambiadoActiv = !string.IsNullOrEmpty(_InformeAnterior.CAMBIO_ACTIVIDAD) ? _InformeAnterior.CAMBIO_ACTIVIDAD : string.Empty;
                        IdProgresoOficio = !string.IsNullOrEmpty(_InformeAnterior.HA_PROGRESADO_OFICIO) ? _InformeAnterior.HA_PROGRESADO_OFICIO : string.Empty;
                        IdCuentaFondoA = !string.IsNullOrEmpty(_InformeAnterior.FONDO_AHORRO) ? _InformeAnterior.FONDO_AHORRO : string.Empty;
                        MotivosTiempoInterrupcionesActividad = _InformeAnterior.MOTIVO_TIEMPO_INTERRUP_ACT;
                        EspecifiqueNoCursos = _InformeAnterior.NO_CURSOS_MOTIVO;
                        ExpecifiqueCambioAct = _InformeAnterior.CAMBIO_ACTIVIDAD_POR_QUE;
                        ActitudesHaciaDesempenioActivProd = _InformeAnterior.ACTITUDES_DESEMPENO_ACT;
                        ConclusionesActivProdCapac = _InformeAnterior.CONCLUSIONES;
                        LugarActaFF = _InformeAnterior.LUGAR;
                        EspecifiqueCompensacion = _InformeAnterior.FONDO_AHORRO_COMPESACION_ACTUA;
                        ActivEnCentroActual = _InformeAnterior.ACTIVIDAD_PRODUC_ACTUAL;
                        LugarProd = _InformeAnterior.LUGAR;
                        FecCapacitFF = _InformeAnterior.FECHA;
                        SalarioPercib = _InformeAnterior.SALARIO_DEVENGABA_DETENCION;
                        LstDiasLaborados = new ObservableCollection<PFF_DIAS_LABORADO>();
                        LstCursosCapacitacionFederal = new ObservableCollection<PFF_CAPACITACION_CURSO>();

                        if (_InformeAnterior.PFF_DIAS_LABORADO != null && _InformeAnterior.PFF_DIAS_LABORADO.Any())
                            foreach (var item in _InformeAnterior.PFF_DIAS_LABORADO)
                            {
                                LstDiasLaborados.Add(item);
                                DiasLaboradosEfectivos = short.Parse((DiasLaboradosEfectivos + item.DIAS_TRABAJADOS).ToString());
                            };

                        DiasTotalAB = DiasLaboradosEfectivos + _InformeAnterior.B_DIAS_LABORADOS_OTROS_CERESOS;
                        if (_InformeAnterior.PFF_CAPACITACION_CURSO != null && _InformeAnterior.PFF_CAPACITACION_CURSO.Any())
                            foreach (var item in _InformeAnterior.PFF_CAPACITACION_CURSO)
                                LstCursosCapacitacionFederal.Add(item);
                    }
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private void InformeActivFF(PERSONALIDAD Entity)
        {
            try
            {
                LoadListasFederales();
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var InformeActividadesAnterior = new cActividadesFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                    if (InformeActividadesAnterior == null)
                    {
                        LstActividadPart = new ObservableCollection<PFF_ACTIVIDAD_PARTICIPACION>();
                        NombreImpInfActivFF = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                        EstudiosCursaActualmente = DetalleOtrosProgAss = EstudiosRealizadosInternamiento = string.Empty;

                        var GruposIngreso = SelectIngreso.GRUPO_PARTICIPANTE;
                        if (GruposIngreso != null && GruposIngreso.Any())
                        {
                            var _Refinados = GruposIngreso.Where(c => c.ESTATUS == (short)eEstatusGrupos.CONCLUIDO);//ESTATUS DE TERMINADO
                            foreach (var item in _Refinados)
                                if (item.ACTIVIDAD != null)
                                    if (!string.IsNullOrEmpty(item.ACTIVIDAD.DESCR))
                                        if ((EstudiosRealizadosInternamiento.Length + item.ACTIVIDAD.DESCR.Trim().Length) < 500)
                                            EstudiosRealizadosInternamiento += string.Format("{0}, ", item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty);

                            var ProgramasActivos = GruposIngreso.Where(x => x.ESTATUS == (short)eEstatusGrupos.ACTIVO);
                            if (ProgramasActivos.Any())
                                foreach (var item in ProgramasActivos)
                                    if (item.ACTIVIDAD != null)
                                        if (!string.IsNullOrEmpty(item.ACTIVIDAD.DESCR))
                                            if ((EstudiosCursaActualmente.Length + item.ACTIVIDAD.DESCR.Trim().Length) < 500)
                                                EstudiosCursaActualmente += string.Format("{0}, ", item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty);

                            foreach (var item in GruposIngreso)
                            {
                                if (LstActividadPart.Any(x => x.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA))
                                    continue;

                                if (item.ACTIVIDAD != null && item.ACTIVIDAD.TIPO_PROGRAMA != null && item.ACTIVIDAD.TIPO_PROGRAMA.ID_DEPARTAMENTO == 13 || item.ID_TIPO_PROGRAMA == (short)eTipoActividad.APOYO_ESPIRITUAL_Y_RELIGIOSO || item.ID_TIPO_PROGRAMA == 168 || item.ID_TIPO_PROGRAMA == 169)
                                    LstActividadPart.Add(new PFF_ACTIVIDAD_PARTICIPACION
                                        {
                                            ID_ANIO = SelectIngreso.ID_ANIO,
                                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                            ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                                            PARTICIPACION = "S",
                                            TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null,
                                            FECHA_1 = item.FEC_REGISTRO,
                                            FECHA_2 = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().FEC_REGISTRO : new DateTime?() : new DateTime?(),
                                            OTRO_ESPECIFICAR = "_"
                                        });

                                else
                                {
                                    if (DetalleOtrosProgAss.Length + (item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim().Length : 0 : 0) <= 500)
                                        DetalleOtrosProgAss += string.Format("{0}, ", item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty);
                                }
                            }
                        };

                        TipoEnsenanza = ConclusionAcadem = HaSidoPromovido = CuantoTiempo = QueOtraEnsenanaRecibe = EspecifiqueOtroAcademico = EspecifiqueNoAsisteEscuelaVoluntariamente = AvanceYRendimientoAcademico = string.Empty;
                        IdAsisteEscuelaVountariamente = HaImpartidoAlgunaEnsenanza = string.Empty;
                        IsAlfabAPrimariaChecked = IsPrimaASecChecked = IsSecABacChecked = IsBacAUnivChecked = IsOtroAcademChecked = false;
                        FecEstudioInformeActivFF = Fechas.GetFechaDateServer;
                        EscolaridadMomentoDetencion = SelectIngreso.ID_ESCOLARIDAD.HasValue ? SelectIngreso.ID_ESCOLARIDAD : -1;
                        LugarEduc = LugarPiePagina();
                    }

                    else
                    {
                        DetalleOtrosProgAss = InformeActividadesAnterior.OTROS_PROGRAMAS;
                        NombreImpInfActivFF = InformeActividadesAnterior.NOMBRE;
                        EscolaridadMomentoDetencion = InformeActividadesAnterior.ESCOLARIDAD_MOMENTO.HasValue ? InformeActividadesAnterior.ESCOLARIDAD_MOMENTO : -1;
                        TipoEnsenanza = InformeActividadesAnterior.IMPARTIDO_ENSENANZA_TIPO;
                        ConclusionAcadem = InformeActividadesAnterior.CONCLUSIONES;
                        CuantoTiempo = InformeActividadesAnterior.IMPARTIDO_ENSENANZA_TIEMPO;
                        QueOtraEnsenanaRecibe = InformeActividadesAnterior.OTRA_ENSENANZA;
                        EspecifiqueOtroAcademico = InformeActividadesAnterior.ESPECIFIQUE;
                        IdAsisteEscuelaVountariamente = !string.IsNullOrEmpty(InformeActividadesAnterior.ASISTE_PUNTUAL) ? InformeActividadesAnterior.ASISTE_PUNTUAL : string.Empty;
                        HaImpartidoAlgunaEnsenanza = !string.IsNullOrEmpty(InformeActividadesAnterior.IMPARTIDO_ENSENANZA) ? InformeActividadesAnterior.IMPARTIDO_ENSENANZA : string.Empty;
                        EspecifiqueNoAsisteEscuelaVoluntariamente = InformeActividadesAnterior.ASISTE_PUNTUAL_NO_POR_QUE;
                        AvanceYRendimientoAcademico = InformeActividadesAnterior.AVANCE_RENDIMIENTO_ACADEMINCO;
                        HaSidoPromovido = !string.IsNullOrEmpty(InformeActividadesAnterior.PROMOVIDO) ? InformeActividadesAnterior.PROMOVIDO : string.Empty;
                        IsAlfabAPrimariaChecked = !string.IsNullOrEmpty(InformeActividadesAnterior.ALFABE_PRIMARIA) ? InformeActividadesAnterior.ALFABE_PRIMARIA == "N" ? InformeActividadesAnterior.ALFABE_PRIMARIA == "S" : true : false;
                        IsPrimaASecChecked = !string.IsNullOrEmpty(InformeActividadesAnterior.PRIMARIA_SECU) ? InformeActividadesAnterior.PRIMARIA_SECU == "N" ? InformeActividadesAnterior.PRIMARIA_SECU == "S" : true : false;
                        IsSecABacChecked = !string.IsNullOrEmpty(InformeActividadesAnterior.SECU_BACHILLER) ? InformeActividadesAnterior.SECU_BACHILLER == "N" ? InformeActividadesAnterior.SECU_BACHILLER == "S" : true : false;
                        IsBacAUnivChecked = !string.IsNullOrEmpty(InformeActividadesAnterior.BACHILLER_UNI) ? InformeActividadesAnterior.BACHILLER_UNI == "N" ? InformeActividadesAnterior.BACHILLER_UNI == "S" : true : false;
                        IsOtroAcademChecked = !string.IsNullOrEmpty(InformeActividadesAnterior.OTRO) ? InformeActividadesAnterior.OTRO == "N" ? InformeActividadesAnterior.OTRO == "S" : true : false;
                        FecEstudioInformeActivFF = InformeActividadesAnterior.FECHA;
                        LugarEduc = InformeActividadesAnterior.LUGAR;
                        EstudiosRealizadosInternamiento = InformeActividadesAnterior.ESTUDIOS_EN_INTERNAMIENTO;
                        EstudiosCursaActualmente = InformeActividadesAnterior.ESTUDIOS_ACTUALES;
                        LstActividadPart = new ObservableCollection<PFF_ACTIVIDAD_PARTICIPACION>();

                        if (InformeActividadesAnterior.PFF_ACTIVIDAD_PARTICIPACION != null && InformeActividadesAnterior.PFF_ACTIVIDAD_PARTICIPACION.Any())
                            foreach (var item in InformeActividadesAnterior.PFF_ACTIVIDAD_PARTICIPACION)
                                LstActividadPart.Add(item);
                    }
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private void VigilanciaDatosFF(PERSONALIDAD Entity)
        {
            try
            {
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var InformeVigilanciaAnt = new cVigilanciaFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                    if (InformeVigilanciaAnt == null)
                    {
                        MotivoTraslado = RelacionCompaneros = DescripcionConducta = ConclusionesGrales = EstimulosBuenaConducta = QuienesRecibeVisita = RecibeVisitaFrecuencia = IdRecibeVisita = IdHigienePersonal = IdHigieneEnCelda = IdClasificConductaGral = IdConducta = IdConductaSuperiores = IdConductaConFam = string.Empty;
                        FecVigiFF = Fechas.GetFechaDateServer;

                        NombreImpVigilanciaFF = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                        FecIngresoImputado = SelectIngreso.FEC_INGRESO_CERESO;
                        var CentroActual = new cCentro().GetData(x => x.ID_CENTRO == SelectIngreso.ID_UB_CENTRO).FirstOrDefault();
                        if (CentroActual != null)
                            NombreCentroProcede = !string.IsNullOrEmpty(CentroActual.DESCR) ? CentroActual.DESCR.Trim() : string.Empty;

                        int algo = string.Empty.Length;
                        if (SelectIngreso.ADUANA_INGRESO.Any())
                            if (SelectIngreso.ADUANA_INGRESO.Any(x => x.ADUANA != null && x.ADUANA.ID_TIPO_PERSONA == 3))
                            {
                                IdRecibeVisita = "S";
                                var _Visitas = SelectIngreso.ADUANA_INGRESO.Where(x => x.ADUANA != null && x.ADUANA.ID_TIPO_PERSONA == 3);
                                if (_Visitas != null && _Visitas.Any())
                                    foreach (var item in _Visitas)
                                        if (item.ADUANA != null)
                                            if (item.ADUANA.PERSONA != null)
                                                if (((!string.IsNullOrEmpty(item.ADUANA.PERSONA.NOMBRE) ? item.ADUANA.PERSONA.NOMBRE.Trim() : string.Empty).Length +
                                                   (!string.IsNullOrEmpty(item.ADUANA.PERSONA.PATERNO) ? item.ADUANA.PERSONA.PATERNO.Trim() : string.Empty).Length +
                                                   (!string.IsNullOrEmpty(item.ADUANA.PERSONA.MATERNO) ? item.ADUANA.PERSONA.MATERNO.Trim() : string.Empty).Length) + QuienesRecibeVisita.Length < 500)
                                                    QuienesRecibeVisita += string.Format("{0} {1} {2}, ",
                                                        !string.IsNullOrEmpty(item.ADUANA.PERSONA.NOMBRE) ? item.ADUANA.PERSONA.NOMBRE.Trim() : string.Empty,
                                                        !string.IsNullOrEmpty(item.ADUANA.PERSONA.PATERNO) ? item.ADUANA.PERSONA.PATERNO.Trim() : string.Empty,
                                                        !string.IsNullOrEmpty(item.ADUANA.PERSONA.MATERNO) ? item.ADUANA.PERSONA.MATERNO.Trim() : string.Empty);
                            }

                            else
                                IdRecibeVisita = "N";
                        else
                            IdRecibeVisita = "N";

                        RecibeVisitaFrecuencia = ProcesaFrecuencia(SelectIngreso);

                        short _Conse = 1;
                        LugarVigi = LugarPiePagina();
                        LstSancFF = new ObservableCollection<SANCION>();
                        LstCorrectivosFF = new ObservableCollection<PFF_CORRECTIVO>();
                        var DetalleSanciones = new cSancion().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.INCIDENTE != null && x.INICIA_FEC.Value.Year == Fechas.GetFechaDateServer.Year).OrderByDescending(c => c.INICIA_FEC);
                        if (DetalleSanciones != null && DetalleSanciones.Any())
                            foreach (var item in DetalleSanciones)
                            {
                                var _NuevoCorrectivo = new PFF_CORRECTIVO()
                                {
                                    ID_CONSEC = _Conse,
                                    FECHA = item.INICIA_FEC,
                                    ID_ANIO = item.ID_ANIO,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_INGRESO = item.ID_INGRESO,
                                    RESOLUCION = item.SANCION_TIPO != null ? !string.IsNullOrEmpty(item.SANCION_TIPO.DESCR) ? item.SANCION_TIPO.DESCR.Trim() : string.Empty : string.Empty,
                                    MOTIVO = item.INCIDENTE != null ? !string.IsNullOrEmpty(item.INCIDENTE.MOTIVO) ? item.INCIDENTE.MOTIVO.Trim() : string.Empty : string.Empty
                                };

                                LstCorrectivosFF.Add(_NuevoCorrectivo);
                                _Conse++;
                            };
                    }

                    else
                    {
                        NombreImpVigilanciaFF = InformeVigilanciaAnt.NOMBRE;
                        FecIngresoImputado = InformeVigilanciaAnt.FECHA_INGRESO;
                        NombreCentroProcede = InformeVigilanciaAnt.CENTRO_DONDE_PROCEDE;
                        IdHigienePersonal = !string.IsNullOrEmpty(InformeVigilanciaAnt.HIGIENE_PERSONAL) ? InformeVigilanciaAnt.HIGIENE_PERSONAL : string.Empty;
                        IdHigieneEnCelda = !string.IsNullOrEmpty(InformeVigilanciaAnt.HIGIENE_CELDA) ? InformeVigilanciaAnt.HIGIENE_CELDA : string.Empty;
                        IdClasificConductaGral = !string.IsNullOrEmpty(InformeVigilanciaAnt.CONDUCTA_GENERAL) ? InformeVigilanciaAnt.CONDUCTA_GENERAL : string.Empty;
                        IdConducta = !string.IsNullOrEmpty(InformeVigilanciaAnt.CONDUCTA) ? InformeVigilanciaAnt.CONDUCTA : string.Empty;
                        IdConductaSuperiores = !string.IsNullOrEmpty(InformeVigilanciaAnt.CONDUCTA_SUPERIORES) ? InformeVigilanciaAnt.CONDUCTA_SUPERIORES : string.Empty;
                        IdConductaConFam = !string.IsNullOrEmpty(InformeVigilanciaAnt.CONDUCTA_FAMILIA) ? InformeVigilanciaAnt.CONDUCTA_FAMILIA : string.Empty;
                        MotivoTraslado = InformeVigilanciaAnt.MOTIVO_TRASLADO;
                        RelacionCompaneros = InformeVigilanciaAnt.RELACION_COMPANEROS;
                        DescripcionConducta = InformeVigilanciaAnt.DESCRIPCION_CONDUCTA;
                        ConclusionesGrales = InformeVigilanciaAnt.CONCLUSIONES;
                        EstimulosBuenaConducta = InformeVigilanciaAnt.ESTIMULOS_BUENA_CONDUCTA;
                        QuienesRecibeVisita = InformeVigilanciaAnt.VISITA_QUIENES;
                        RecibeVisitaFrecuencia = InformeVigilanciaAnt.VISITA_FRECUENCIA;
                        IdRecibeVisita = !string.IsNullOrEmpty(InformeVigilanciaAnt.VISITA_RECIBE) ? InformeVigilanciaAnt.VISITA_RECIBE : string.Empty;
                        FecVigiFF = InformeVigilanciaAnt.FECHA;
                        LugarVigi = InformeVigilanciaAnt.LUGAR;
                        LstCorrectivosFF = new ObservableCollection<PFF_CORRECTIVO>();

                        if (InformeVigilanciaAnt.PFF_CORRECTIVO != null && InformeVigilanciaAnt.PFF_CORRECTIVO.Any())
                            foreach (var item in InformeVigilanciaAnt.PFF_CORRECTIVO)
                                LstCorrectivosFF.Add(item);
                    }
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private void CriminoDiagnosticoInfoFF(PERSONALIDAD Entity)
        {
            if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
            {
                var CriminodFederalAnterior = new cCriminologicoFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (CriminodFederalAnterior == null)
                {
                    VersionDelitoCriminFF = CaractPersonalesRelacionadasDelito = ResultadoTratamientoInst = CriminogenesisCrimFF = AntecedentesParaSocialesAntisociales = string.Empty;
                    IdPronReinciFF = ReqTrataExtraMurosCriminFF = OpinionSobreConBeneficio = AfirmaEspecifFF = NegatEspecifFF = Egocentrismo = IdRequiereValoracionCrimin = IdEstadoPeligrosidad = string.Empty;
                    LabAfectiva = Agresividad = IndAfectiva = SobreN = string.Empty;
                    FecCriminFF = Fechas.GetFechaDateServer;
                    LugarCrimi = LugarPiePagina();
                    PrimoCheckedFederal = EspecificoCheckedFederal = GenericoCheckedFederal = HabitualCheckedFederal = ProfesionalCheckedFederal = false;

                    NombreImpCriminFF = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                    var _Apodos = new cApodo().ObtenerTodosXImputado(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO);
                    if (_Apodos.Any())
                        foreach (var item in _Apodos)
                            if (!string.IsNullOrEmpty(item.APODO1))
                            {
                                if (SobreN.Length + item.APODO1.Length > 99)
                                    continue;

                                SobreN += string.Format("{0} ,", item.APODO1.Trim());
                            };

                    if (SelectIngreso.EMI_INGRESO.Any())
                    {
                        var _UltimoEMI = SelectIngreso.EMI_INGRESO.OrderByDescending(x => x.ID_EMI_CONS).FirstOrDefault();
                        if (_UltimoEMI != null)
                            if (_UltimoEMI.EMI != null)
                            {
                                if (_UltimoEMI.EMI.EMI_SITUACION_JURIDICA != null)
                                    if (!string.IsNullOrEmpty(_UltimoEMI.EMI.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO))
                                        if (_UltimoEMI.EMI.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO.Length > 1000)
                                            VersionDelitoCriminFF = _UltimoEMI.EMI.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO.Substring(0, 999);
                                        else
                                            VersionDelitoCriminFF = _UltimoEMI.EMI.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO;

                                if (_UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO != null)
                                {
                                    Egocentrismo = _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EGOCENTRISMO == 3 ? "A" : _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EGOCENTRISMO == 2 ? "M" : _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EGOCENTRISMO == 1 ? "B" : string.Empty;
                                    LabAfectiva = _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.LABILIDAD_AFECTIVA == 3 ? "A" : _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.LABILIDAD_AFECTIVA == 2 ? "M" : _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.LABILIDAD_AFECTIVA == 1 ? "B" : string.Empty;
                                    Agresividad = _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.AGRESIVIDAD == 3 ? "A" : _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.AGRESIVIDAD == 2 ? "M" : _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.AGRESIVIDAD == 1 ? "B" : string.Empty;
                                    IndAfectiva = _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.INDIFERENCIA_AFECTIVA == 3 ? "A" : _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.INDIFERENCIA_AFECTIVA == 2 ? "M" : _UltimoEMI.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.INDIFERENCIA_AFECTIVA == 1 ? "B" : string.Empty;
                                };
                            };
                    };
                }

                else
                {
                    NombreImpCriminFF = CriminodFederalAnterior.NOMBRE;
                    LabAfectiva = !string.IsNullOrEmpty(CriminodFederalAnterior.P7_LABILIDAD) ? CriminodFederalAnterior.P7_LABILIDAD : string.Empty;
                    Agresividad = !string.IsNullOrEmpty(CriminodFederalAnterior.P7_AGRESIVIDAD) ? CriminodFederalAnterior.P7_AGRESIVIDAD : string.Empty;
                    IndAfectiva = !string.IsNullOrEmpty(CriminodFederalAnterior.P7_INDIFERENCIA) ? CriminodFederalAnterior.P7_INDIFERENCIA : string.Empty;
                    CriminogenesisCrimFF = CriminodFederalAnterior.P6_CRIMINOGENESIS;
                    ResultadoTratamientoInst = CriminodFederalAnterior.P8_RESULTADO_TRATAMIENTO;
                    SobreN = CriminodFederalAnterior.SOBRENOMBRE;
                    VersionDelitoCriminFF = CriminodFederalAnterior.P1_VERSION_INTERNO;
                    CaractPersonalesRelacionadasDelito = CriminodFederalAnterior.P2_PERSONALIDAD;
                    AntecedentesParaSocialesAntisociales = CriminodFederalAnterior.ANTECEDENTES_PARA_ANTI_SOCIALE;
                    Egocentrismo = !string.IsNullOrEmpty(CriminodFederalAnterior.P7_EGOCENTRISMO) ? CriminodFederalAnterior.P7_EGOCENTRISMO : string.Empty;
                    IdRequiereValoracionCrimin = !string.IsNullOrEmpty(CriminodFederalAnterior.P3_VALORACION) ? CriminodFederalAnterior.P3_VALORACION : string.Empty;
                    IdEstadoPeligrosidad = !string.IsNullOrEmpty(CriminodFederalAnterior.P8_ESTADO_PELIGRO) ? CriminodFederalAnterior.P8_ESTADO_PELIGRO : string.Empty;
                    IdPronReinciFF = !string.IsNullOrEmpty(CriminodFederalAnterior.P9_PRONOSTICO) ? CriminodFederalAnterior.P9_PRONOSTICO : string.Empty;
                    ReqTrataExtraMurosCriminFF = !string.IsNullOrEmpty(CriminodFederalAnterior.P10_CONTINUAR_TRATAMIENTO) ? CriminodFederalAnterior.P10_CONTINUAR_TRATAMIENTO : string.Empty;
                    OpinionSobreConBeneficio = CriminodFederalAnterior.P10_OPINION;
                    AfirmaEspecifFF = CriminodFederalAnterior.P10_CONTINUAR_SI_ESPECIFICAR;
                    NegatEspecifFF = CriminodFederalAnterior.P10_CONTINUAR_NO_ESPECIFICAR;
                    LugarCrimi = CriminodFederalAnterior.LUGAR;
                    FecCriminFF = CriminodFederalAnterior.FECHA;
                    PrimoCheckedFederal = CriminodFederalAnterior.P5_PRIMODELINCUENTE == "S";
                    EspecificoCheckedFederal = CriminodFederalAnterior.P5_ESPECIFICO == "S";
                    GenericoCheckedFederal = CriminodFederalAnterior.P5_GENERICO == "S";
                    HabitualCheckedFederal = CriminodFederalAnterior.P5_HABITUAL == "S";
                    ProfesionalCheckedFederal = CriminodFederalAnterior.P5_PROFESIONAL == "S";
                }
            }
        }
        private void SocioFamiliarFueroFederalInfo(PERSONALIDAD Entity)
        {
            try
            {
                LoadListasFederales();
                if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                {
                    var _EstudioSocioFamAnterior = new cEstudioTrabajoSocialFueroFederal().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                    if (_EstudioSocioFamAnterior == null)
                    {
                        OcupacionAntesIngreso = -1;
                        short CountHabitaciones = 0;
                        NombbreImptSocioFF = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                                          !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                                          !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                        var _Dialecto = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.ID_DIALECTO : null;
                        if (_Dialecto != null)
                            Dialecto = _Dialecto;
                        else
                            Dialecto = -1;

                        DescripcionServiciosCuenta = LugarFechaNac = IdCaracZonaSec = EspecifiqueProblemasConductaAntisocial = string.Empty;
                        if (SelectExpediente.NACIMIENTO_ESTADO != null)
                            if (SelectExpediente.NACIMIENTO_MUNICIPIO != null)
                            {
                                var municipio = new cMunicipio().Obtener(SelectExpediente.NACIMIENTO_ESTADO.Value, SelectExpediente.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                                LugarFechaNac = SelectIngreso.IMPUTADO != null ? string.Format("{0} ", SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? municipio != null && !string.IsNullOrEmpty(municipio.MUNICIPIO1) ? municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty) : string.Empty;
                            }

                        FechaNacimientoTSFederal = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA : new DateTime?();
                        //EscolaridadIngreso = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.ID_ESCOLARIDAD : -1;
                        EscolaridadIngreso = SelectIngreso != null ? SelectIngreso.ID_ESCOLARIDAD.HasValue ? SelectIngreso.ID_ESCOLARIDAD : -1 : -1;
                        //EdoCivilTrabajoSocFF = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.ESTADO_CIVIL.DESCR) ? SelectIngreso.IMPUTADO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        EdoCivilTrabajoSocFF = SelectIngreso != null ? SelectIngreso.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectIngreso.ESTADO_CIVIL.DESCR) ? SelectIngreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        //var _Edo = new cEstadoCivil().GetData(x => x.ID_ESTADO_CIVIL == SelectIngreso.IMPUTADO.ID_ESTADO_CIVIL).FirstOrDefault();
                        //if (_Edo != null)
                        //    IdEdoCivilTSFF = _Edo.ID_ESTADO_CIVIL;
                        IdEdoCivilTSFF = SelectIngreso != null ? SelectIngreso.ESTADO_CIVIL != null ? SelectIngreso.ID_ESTADO_CIVIL : -1 : -1;
                        OcupacionAntesIngreso = SelectIngreso.ID_OCUPACION;
                        var _EmiByIngreso = SelectIngreso.EMI_INGRESO.Any() ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI != null ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI.EMI_ULTIMOS_EMPLEOS : null : null;
                        if (_EmiByIngreso != null)
                        {
                            var _UltimoEmpleo = _EmiByIngreso.FirstOrDefault(x => x.ULTIMO_EMPLEO_ANTES_DETENCION == "S");
                            if (_UltimoEmpleo != null)
                                OcupacionAntesIngreso = _UltimoEmpleo.ID_OCUPACION;
                        };

                        IdMiembroProblemasConductaAntisocial = string.Empty;
                        CantidadHijosUnionesAnteriores = SelectIngreso.EMI_INGRESO.Any() ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI != null ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI.EMI_FACTORES_SOCIO_FAMILIARES != null ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI.EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS.ToString() : string.Empty : string.Empty : string.Empty;
                        //Domicilio = SelectIngreso.IMPUTADO != null ? string.Format("{0} {1} ", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.DOMICILIO_CALLE) ? SelectIngreso.IMPUTADO.DOMICILIO_CALLE.Trim() : string.Empty, SelectIngreso.IMPUTADO.DOMICILIO_NUM_EXT.HasValue ? SelectIngreso.IMPUTADO.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty) : string.Empty;
                        Domicilio = SelectIngreso != null ? string.Format("{0} {1} ", !string.IsNullOrEmpty(SelectIngreso.DOMICILIO_CALLE) ? SelectIngreso.DOMICILIO_CALLE.Trim() : string.Empty, SelectIngreso.DOMICILIO_NUM_EXT.HasValue ? SelectIngreso.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty) : string.Empty;

                        IdAlgunIntegranteCuentaAntecedentesAdiccion = EspecifiqueAdiccion = IdCaractZona = IdRelacionInterFamSec = IdGrupoFamiliarSecundario = string.Empty;
                        var EstudioSocioFamImputado = new cEstudioSocioEconomico().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault();
                        if (EstudioSocioFamImputado != null)
                        {
                            if (EstudioSocioFamImputado.SOCIOE_GPOFAMPRI != null)
                            {
                                IdCaractZona = EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA == "U" ? "U" : EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA == "S" ? "S" : EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA == "R" ? "R" : string.Empty;
                                ResponsableManutHogar = string.Empty;//Este dato NO EXISTE en el estudio socioeconomico
                                TotalIngresosMensuales = EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.INGRESO_MENSUAL;
                                TotalEgresosMensuales = EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.EGRESO_MENSUAL;
                                IdGrupoFamiliarPrim = EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR;
                                IdRelacionesInterf = EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR;
                                TieneFondosAhorro = IdHuboViolenciaIntro = IdActualmenteCooperaEconoConFamilia = EspecifiqueViolenciaIntro = string.Empty;
                                IdNivelSocioEconoPrim = EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL;
                                IdAlgunIntegranteCuentaAntecedentesAdiccion = EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE == 0 ? "S" : "N";
                                EspecifiqueAdiccion = EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE == 0 ? EstudioSocioFamImputado.SOCIOE_GPOFAMPRI.ANTECEDENTE : string.Empty;
                            };

                            if (EstudioSocioFamImputado.SOCIOE_GPOFAMSEC != null)
                            {
                                IdNivelSocioEconCulturalSec = EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL;
                                IdGrupoFamiliarSecundario = EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR;
                                IdRelacionInterFamSec = EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR;
                                IdCaracZonaSec = EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA == "U" ? "U" : EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA == "S" ? "S" : EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA == "R" ? "R" : string.Empty;
                                IdMiembroProblemasConductaAntisocial = EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE == decimal.Zero ? "S" : "N";
                                EspecifiqueProblemasConductaAntisocial = EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE == decimal.Zero ? EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.ANTECEDENTE : string.Empty;
                                var _DetalleEstudioSecundario = EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC;
                                EnseresMobiliarioDomestico = string.Empty;
                                if (_DetalleEstudioSecundario != null && _DetalleEstudioSecundario.Any())
                                    foreach (var item in _DetalleEstudioSecundario.AsQueryable())
                                    {
                                        if (item.ID_TIPO.Contains("D"))//Detalle de caracteristicas del estudio socioeconomico en el nucleo secundario
                                            CountHabitaciones++;

                                        if (item.ID_TIPO.Contains("E"))
                                        {
                                            if (!string.IsNullOrEmpty(EnseresMobiliarioDomestico))
                                            {
                                                if (EnseresMobiliarioDomestico.Contains(item.SOCIOECONOMICO_CAT.DESCR))
                                                    continue;
                                            }

                                            EnseresMobiliarioDomestico += string.Format("{0}, ", item.SOCIOECONOMICO_CAT != null ? !string.IsNullOrEmpty(item.SOCIOECONOMICO_CAT.DESCR) ? item.SOCIOECONOMICO_CAT.DESCR.Trim() : string.Empty : string.Empty);
                                        }

                                        if (item.ID_TIPO.Contains("S"))
                                            if (item.SOCIOECONOMICO_CAT != null)
                                                if (!string.IsNullOrEmpty(item.SOCIOECONOMICO_CAT.DESCR))
                                                    if (DescripcionServiciosCuenta.Length + item.SOCIOECONOMICO_CAT.DESCR.Trim().Length < 500)
                                                        DescripcionServiciosCuenta += string.Format("{0},", item.SOCIOECONOMICO_CAT.DESCR.Trim());

                                        if (item.ID_TIPO.Contains("M"))
                                            DescripcionVivienda += string.Format("{0}, ", item.SOCIOECONOMICO_CAT != null ? !string.IsNullOrEmpty(item.SOCIOECONOMICO_CAT.DESCR) ? item.SOCIOECONOMICO_CAT.DESCR.Trim() : string.Empty : string.Empty);
                                    };

                                CantidadHabitacionesTotal = CountHabitaciones;
                                IdCaracZona = EstudioSocioFamImputado.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA;
                            }
                        }

                        else
                        {
                            IdCaractZona = ResponsableManutHogar = IdNivelSocioEconoPrim = IdGrupoFamiliarPrim = IdRelacionesInterf = TieneFondosAhorro = IdHuboViolenciaIntro = IdActualmenteCooperaEconoConFamilia = EspecifiqueViolenciaIntro = string.Empty;
                            IdNivelSocioEconCulturalSec = EnseresMobiliarioDomestico = DescripcionVivienda = IdCaracZona = string.Empty;
                        }

                        IdRecibeVisitasFamiliares = string.Empty;
                        var _VisitanteIngreso = new cAduanaIngreso().GetData(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ADUANA != null && x.ADUANA.ID_TIPO_PERSONA == (short)eTiposAduana.VISITA);
                        if (_VisitanteIngreso.Any())
                            IdRecibeVisitasFamiliares = "S";

                        TiempoLaborar = string.Empty;
                        var _EmiByIngreso2 = SelectIngreso.EMI_INGRESO.Any() ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI != null ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI.EMI_ULTIMOS_EMPLEOS : null : null;
                        if (_EmiByIngreso2 != null)
                        {
                            var _UltimoEmpleo = _EmiByIngreso2.FirstOrDefault(x => x.ULTIMO_EMPLEO_ANTES_DETENCION == "S");
                            if (_UltimoEmpleo != null)
                            {
                                TrabajoDesempeniadoAntesReclusion += string.Format("{0} ", _UltimoEmpleo.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(_UltimoEmpleo.OCUPACION.DESCR) ? _UltimoEmpleo.OCUPACION.DESCR.Trim() : string.Empty : string.Empty);
                                TiempoLaborar = string.Format("{0}", !string.IsNullOrEmpty(_UltimoEmpleo.DURACION) ? _UltimoEmpleo.DURACION.Trim() : string.Empty);
                            };
                        };

                        LstGrupoFam = new ObservableCollection<PFF_GRUPO_FAMILIAR>();
                        LstGrupoFamSecu = new ObservableCollection<PFF_GRUPO_FAMILIAR>();

                        var GrupoFamPrimarioEMI = SelectIngreso.EMI_INGRESO.Any() ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI != null ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI.EMI_GRUPO_FAMILIAR : null : null;
                        if (GrupoFamPrimarioEMI != null && GrupoFamPrimarioEMI.Any())
                        {
                            short _Conse = 1;
                            foreach (var item in GrupoFamPrimarioEMI)
                            {
                                if (item.GRUPO == 1)
                                {
                                    var Grupo = new PFF_GRUPO_FAMILIAR()
                                    {
                                        EDAD = item.EDAD.HasValue ? item.EDAD.Value.ToString() : string.Empty,
                                        ESTADO_CIVIL = item.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(item.ESTADO_CIVIL.DESCR) ? item.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty,
                                        ID_CONSEC = _Conse,
                                        ID_GRUPO_FAMILIAR = (short)eTipopGrupoTrabajoSocial.PRIMARIO,
                                        NOMBRE = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Trim() : string.Empty),
                                        OCUPACION = item.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(item.OCUPACION.DESCR) ? item.OCUPACION.DESCR.Trim() : string.Empty : string.Empty,
                                        PARENTESCO = item.ID_RELACION.HasValue ? !string.IsNullOrEmpty(item.TIPO_REFERENCIA.DESCR) ? item.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                    };

                                    LstGrupoFam.Add(Grupo);
                                    continue;
                                }

                                if (item.GRUPO == 2)
                                {
                                    var Grupo = new PFF_GRUPO_FAMILIAR()
                                    {
                                        EDAD = item.EDAD.HasValue ? item.EDAD.Value.ToString() : string.Empty,
                                        ESTADO_CIVIL = item.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(item.ESTADO_CIVIL.DESCR) ? item.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty,
                                        ID_CONSEC = _Conse,
                                        ID_GRUPO_FAMILIAR = (short)eTipopGrupoTrabajoSocial.SECUNDARIO,
                                        NOMBRE = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Trim() : string.Empty),
                                        OCUPACION = item.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(item.OCUPACION.DESCR) ? item.OCUPACION.DESCR.Trim() : string.Empty : string.Empty,
                                        PARENTESCO = item.ID_RELACION.HasValue ? !string.IsNullOrEmpty(item.TIPO_REFERENCIA.DESCR) ? item.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                    };

                                    LstGrupoFamSecu.Add(Grupo);
                                    continue;
                                }
                            }
                        }

                        FrecuenciaVisitas = ProcesaFrecuencia(SelectIngreso);
                        LugarTS = LugarPiePagina();
                        SueldoPercibido = 0;
                        EPais = Parametro.PAIS;//82;
                        EColonia = -1;
                        IdParentescoQuienViviraSerExternado = IdParentescoVisTSFF = EMunicipio = EscolaridadActual = IdParentAvalTSFF = -1;
                        IdEntidadFedQuienViviraSerExternado = IdCiudadQuienViviraSerExternado = IdMunicipioQuienViviraSerExternado = IdColoniaQuienViviraSerExternado = 0;
                        IdRadicanEstado = NombreCalleExternado = IdCuentaOfertaTrabajo = IsVisitadoPorOtrasPersonas = IdCuentaApoyoFamiliaOtraPersona = NoQuienViviraSerExternado = CodPosQuienViviraSerExternado = string.Empty;
                        NumeroParejasVividoManeraEstable = IDHuboViolenciaIntraFam = OpinionConcesionBeneficio = DiagnosticoPronosticoExternacion = DeQueManeraLeInfluenciaEstanciaPrision = OpicionAcercaInternamiento = CalleQuienViviraSerExternado = ConQuienViviraSerExternado = NombreAvalMoral = QuienesVisitanOtrasPersonas = ConsisteOfertaFF = DescripcionAlimentacionFamiliar = DescripcionGastoFamiliar = DsitribucionGastoFamiliar = DescripcionAportacionesEconomicasQuienCuanto = RelacionMedioExterno = ConceptoTieneFamiliaInterno = TransporteCercaVivienda = EspecifiqueViolenciaIntraFam = string.Empty;
                        FecEstudioSocioFF = Fechas.GetFechaDateServer;
                    }

                    else
                    {
                        NombbreImptSocioFF = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.NOMBRE) ? _EstudioSocioFamAnterior.NOMBRE.Trim() : string.Empty;
                        Dialecto = _EstudioSocioFamAnterior.DIALECTO.HasValue ? _EstudioSocioFamAnterior.DIALECTO : -1;
                        LugarFechaNac = _EstudioSocioFamAnterior.LUGAR_NAC;
                        EscolaridadIngreso = _EstudioSocioFamAnterior.ESCOLARIDAD_CENTRO.HasValue ? _EstudioSocioFamAnterior.ESCOLARIDAD_CENTRO : -1;
                        IdEdoCivilTSFF = _EstudioSocioFamAnterior.EDO_CIVIL.HasValue ? _EstudioSocioFamAnterior.EDO_CIVIL : -1;
                        EscolaridadActual = _EstudioSocioFamAnterior.ESCOLARIDAD_ACTUAL.HasValue ? _EstudioSocioFamAnterior.ESCOLARIDAD_ACTUAL : -1;
                        OcupacionAntesIngreso = _EstudioSocioFamAnterior.OCUPACION_ANT.HasValue ? _EstudioSocioFamAnterior.OCUPACION_ANT : -1;
                        Domicilio = _EstudioSocioFamAnterior.DOMICILIO;
                        IdCaractZona = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.ECO_FP_ZONA) ? _EstudioSocioFamAnterior.ECO_FP_ZONA : string.Empty;
                        IdCaracZonaSec = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FS_ZONA) ? _EstudioSocioFamAnterior.CARACT_FS_ZONA : string.Empty;
                        ResponsableManutHogar = _EstudioSocioFamAnterior.ECO_FP_RESPONSABLE;
                        TotalIngresosMensuales = _EstudioSocioFamAnterior.ECO_FP_TOTAL_INGRESOS_MEN;
                        TotalEgresosMensuales = _EstudioSocioFamAnterior.ECO_FP_TOTAL_EGRESOS_MEN;
                        IdActualmenteCooperaEconoConFamilia = _EstudioSocioFamAnterior.ECO_FP_COOPERA_ACTUALMENTE;
                        TieneFondosAhorro = _EstudioSocioFamAnterior.ECO_FP_FONDOS_AHORRO;
                        FechaNacimientoTSFederal = _EstudioSocioFamAnterior.FECHA_NAC;
                        IdGrupoFamiliarPrim = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FP_GRUPO) ? _EstudioSocioFamAnterior.CARACT_FP_GRUPO : string.Empty;
                        IdRelacionesInterf = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FP_RELAC_INTERFAM) ? _EstudioSocioFamAnterior.CARACT_FP_RELAC_INTERFAM : string.Empty;
                        IdHuboViolenciaIntro = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FP_VIOLENCIA_FAM) ? _EstudioSocioFamAnterior.CARACT_FP_VIOLENCIA_FAM : string.Empty;
                        EspecifiqueViolenciaIntro = _EstudioSocioFamAnterior.CARACT_FP_VIOLENCIA_FAM_ESPEFI;
                        IdNivelSocioEconoPrim = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FP_NIVEL_SOCIO_CULTURAL) ? _EstudioSocioFamAnterior.CARACT_FP_NIVEL_SOCIO_CULTURAL : string.Empty;
                        IdAlgunIntegranteCuentaAntecedentesAdiccion = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FP_ANTECE_PENALES_ADIC) ? _EstudioSocioFamAnterior.CARACT_FP_ANTECE_PENALES_ADIC : string.Empty;
                        EspecifiqueAdiccion = _EstudioSocioFamAnterior.CARACT_FP_ANTECEDENTES_PENALES;
                        ConceptoTieneFamiliaInterno = _EstudioSocioFamAnterior.CARACT_FP_CONCEPTO;
                        CantidadHijosUnionesAnteriores = _EstudioSocioFamAnterior.CARACT_FS_HIJOS_ANT;
                        IdGrupoFamiliarSecundario = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FS_GRUPO) ? _EstudioSocioFamAnterior.CARACT_FS_GRUPO : string.Empty;
                        IdRelacionInterFamSec = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FS_RELACIONES_INTERFAM) ? _EstudioSocioFamAnterior.CARACT_FS_RELACIONES_INTERFAM : string.Empty;
                        IDHuboViolenciaIntraFam = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FS_VIOLENCIA_INTRAFAM) ? _EstudioSocioFamAnterior.CARACT_FS_VIOLENCIA_INTRAFAM : string.Empty;
                        EspecifiqueViolenciaIntraFam = _EstudioSocioFamAnterior.CARACT_FS_VIOLENCIA_INTRAFAM_E;
                        IdNivelSocioEconCulturalSec = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FS_NIVEL_SOCIO_CULTURAL) ? _EstudioSocioFamAnterior.CARACT_FS_NIVEL_SOCIO_CULTURAL : string.Empty;
                        CantidadHabitacionesTotal = _EstudioSocioFamAnterior.CARACT_FS_VIVIEN_NUM_HABITACIO;
                        DescripcionVivienda = _EstudioSocioFamAnterior.CARACT_FS_VIVIEN_DESCRIPCION;
                        TransporteCercaVivienda = _EstudioSocioFamAnterior.CARACT_FS_VIVIEN_TRANSPORTE;
                        EnseresMobiliarioDomestico = _EstudioSocioFamAnterior.CARACT_FS_VIVIEN_MOBILIARIO;
                        IdCaracZona = _EstudioSocioFamAnterior.CARACT_FS_ZONA;
                        RelacionMedioExterno = _EstudioSocioFamAnterior.CARACT_FS_RELACION_MEDIO_EXT;
                        IdMiembroProblemasConductaAntisocial = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.CARACT_FS_PROBLEMAS_CONDUCTA) ? _EstudioSocioFamAnterior.CARACT_FS_PROBLEMAS_CONDUCTA : string.Empty;
                        EspecifiqueProblemasConductaAntisocial = _EstudioSocioFamAnterior.CARACT_FS_PROBLEMAS_CONDUCTA_E;
                        NumeroParejasVividoManeraEstable = _EstudioSocioFamAnterior.NUM_PAREJAS_ESTABLE;
                        TrabajoDesempeniadoAntesReclusion = _EstudioSocioFamAnterior.TRABAJO_DESEMP_ANTES;
                        TiempoLaborar = _EstudioSocioFamAnterior.TIEMPO_LABORAR;
                        SueldoPercibido = _EstudioSocioFamAnterior.SUELDO_PERCIBIDO;
                        DescripcionAportacionesEconomicasQuienCuanto = _EstudioSocioFamAnterior.APORTACIONES_FAM;
                        DsitribucionGastoFamiliar = _EstudioSocioFamAnterior.DISTRIBUCION_GASTO_FAM;
                        DescripcionAlimentacionFamiliar = _EstudioSocioFamAnterior.ALIMENTACION_FAM;
                        DescripcionServiciosCuenta = _EstudioSocioFamAnterior.SERVICIOS_PUBLICOS;
                        IdCuentaOfertaTrabajo = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.OFERTA_TRABAJO) ? _EstudioSocioFamAnterior.OFERTA_TRABAJO : string.Empty;
                        ConsisteOfertaFF = _EstudioSocioFamAnterior.OFERTA_TRABAJO_CONSISTE;
                        IdCuentaApoyoFamiliaOtraPersona = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.APOYO_FAM_OTROS) ? _EstudioSocioFamAnterior.APOYO_FAM_OTROS : string.Empty;
                        IdRecibeVisitasFamiliares = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.VISITA_FAMILIARES) ? _EstudioSocioFamAnterior.VISITA_FAMILIARES : string.Empty;
                        EEstado = _EstudioSocioFamAnterior.EXTERNADO_ENTIDAD;
                        ConQuienViviraSerExternado = _EstudioSocioFamAnterior.EXTERNADO_VIVIR_NOMBRE;
                        NombreCalleExternado = _EstudioSocioFamAnterior.EXTERNADO_CALLE;
                        NoQuienViviraSerExternado = _EstudioSocioFamAnterior.EXTERNADO_NUMERO;
                        CodPosQuienViviraSerExternado = _EstudioSocioFamAnterior.EXTERNADO_CP;
                        IdParentAvalTSFF = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.AVAL_MORAL_PARENTESCO) ? short.Parse(_EstudioSocioFamAnterior.AVAL_MORAL_PARENTESCO) : short.Parse((-1).ToString());
                        EMunicipio = _EstudioSocioFamAnterior.EXTERNADO_MUNICIPIO.HasValue ? _EstudioSocioFamAnterior.EXTERNADO_MUNICIPIO != decimal.Zero ? _EstudioSocioFamAnterior.EXTERNADO_MUNICIPIO : -1 : -1;
                        EColonia = _EstudioSocioFamAnterior.EXTERNADO_COLONIA != decimal.Zero ? _EstudioSocioFamAnterior.EXTERNADO_COLONIA : -1;
                        EPais = _EstudioSocioFamAnterior.COLONIA != null ? _EstudioSocioFamAnterior.COLONIA.MUNICIPIO != null ? _EstudioSocioFamAnterior.COLONIA.MUNICIPIO.ENTIDAD != null ? _EstudioSocioFamAnterior.COLONIA.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD != null ? _EstudioSocioFamAnterior.COLONIA.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.ID_PAIS_NAC : short.Parse((82).ToString()) : short.Parse((82).ToString()) : short.Parse((82).ToString()) : short.Parse((82).ToString());
                        IdParentescoQuienViviraSerExternado = _EstudioSocioFamAnterior.EXTERNADO_PARENTESCO.HasValue ? _EstudioSocioFamAnterior.EXTERNADO_PARENTESCO != -1 ? _EstudioSocioFamAnterior.EXTERNADO_PARENTESCO : -1 : -1;
                        IdRadicanEstado = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.RADICAN_ESTADO) ? _EstudioSocioFamAnterior.RADICAN_ESTADO : string.Empty;
                        IdParentescoVisTSFF = _EstudioSocioFamAnterior.VISTA_PARENTESCO.HasValue ? _EstudioSocioFamAnterior.VISTA_PARENTESCO != -1 ? _EstudioSocioFamAnterior.VISTA_PARENTESCO : -1 : -1;
                        FrecuenciaVisitas = _EstudioSocioFamAnterior.VISITA_FRECUENCIA;
                        IsVisitadoPorOtrasPersonas = !string.IsNullOrEmpty(_EstudioSocioFamAnterior.VISITAS_OTROS) ? _EstudioSocioFamAnterior.VISITAS_OTROS : string.Empty;
                        QuienesVisitanOtrasPersonas = _EstudioSocioFamAnterior.VISITA_OTROS_QUIIEN;
                        NombreAvalMoral = _EstudioSocioFamAnterior.AVAL_MORAL;
                        IdParentescoAval = _EstudioSocioFamAnterior.AVAL_MORAL_PARENTESCO;
                        OpicionAcercaInternamiento = _EstudioSocioFamAnterior.OPINION_INTERNAMIENTO;
                        DeQueManeraLeInfluenciaEstanciaPrision = _EstudioSocioFamAnterior.INFLUENCIADO_ESTANCIA_PRISION;
                        DiagnosticoPronosticoExternacion = _EstudioSocioFamAnterior.DIAG_SOCIAL_PRONOS;
                        OpinionConcesionBeneficio = _EstudioSocioFamAnterior.OPINION_CONCESION_BENEFICIOS;
                        if (_EstudioSocioFamAnterior.EXTERNADO_ENTIDAD.HasValue)
                        {
                            var _detallesEntidad = new cEntidad().GetData(x => x.ID_ENTIDAD == _EstudioSocioFamAnterior.EXTERNADO_ENTIDAD).FirstOrDefault();
                            SelectedPais = _detallesEntidad != null ? _detallesEntidad.PAIS_NACIONALIDAD != null ? _detallesEntidad.PAIS_NACIONALIDAD : null : null;
                        };

                        if (_EstudioSocioFamAnterior.EXTERNADO_ENTIDAD.HasValue)
                            SelectedEstado = new cEntidad().GetData(x => x.ID_ENTIDAD == _EstudioSocioFamAnterior.EXTERNADO_ENTIDAD).FirstOrDefault();

                        if (_EstudioSocioFamAnterior.EXTERNADO_MUNICIPIO.HasValue)
                            SelectedMunicipio = new cMunicipio().GetData(x => x.ID_MUNICIPIO == _EstudioSocioFamAnterior.EXTERNADO_MUNICIPIO).FirstOrDefault();

                        LugarTS = _EstudioSocioFamAnterior.LUGAR;
                        FecEstudioSocioFF = _EstudioSocioFamAnterior.FECHA;
                        LstGrupoFam = new ObservableCollection<PFF_GRUPO_FAMILIAR>();
                        LstGrupoFamSecu = new ObservableCollection<PFF_GRUPO_FAMILIAR>();

                        if (_EstudioSocioFamAnterior.PFF_GRUPO_FAMILIAR != null && _EstudioSocioFamAnterior.PFF_GRUPO_FAMILIAR.Any())
                        {
                            foreach (var item in _EstudioSocioFamAnterior.PFF_GRUPO_FAMILIAR.AsQueryable())
                            {
                                if (item.ID_GRUPO_FAMILIAR == (short)eTipopGrupoTrabajoSocial.PRIMARIO)
                                {
                                    LstGrupoFam.Add(item);
                                    continue;//Ya lo ubique, no es necesario continuar recorriendo la lista de elementos
                                }

                                if (item.ID_GRUPO_FAMILIAR == (short)eTipopGrupoTrabajoSocial.SECUNDARIO)
                                {
                                    LstGrupoFamSecu.Add(item);
                                    continue;
                                };
                            };
                        };
                    }

                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion
        #region Guardado Fuero Federal
        private PFF_ACTA_CONSEJO_TECNICO ActaConsejoFueroFederal()
        {
            try
            {
                var NombreDirector = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                var _Respuesta = new PFF_ACTA_CONSEJO_TECNICO()
                {
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    APROBADO_APLAZADO = ActuacionR,
                    APROBADO_POR = VotosR,
                    CEN_ID_CENTRO = IdCentro,
                    EXPEDIENTE = ExpedienteImputadoFF,
                    FECHA = FechaActa,
                    SUSCRITO_DIRECTOR_CRS = NombreDirector != null ? string.Format("{0}", !string.IsNullOrEmpty(NombreDirector.DIRECTOR) ? NombreDirector.DIRECTOR.Trim() : string.Empty) : string.Empty,
                    TRAMITE = TramiteDescripcion,
                    DIRECTOR = _NombreDir,
                    SESION_FEC = EnSesionDeFecha,
                    LUGAR = LugarActa,
                    PRESENTE_ACTUACION = string.Empty
                };

                if (LstAreasTec != null && LstAreasTec.Any())
                    foreach (var item in LstAreasTec)
                        _Respuesta.PFF_ACTA_DETERMINO.Add(item);

                return _Respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }
        private PFF_ESTUDIO_MEDICO GuardadoEstudioMedicoFederal()
        {
            try
            {
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                var NombreDirector = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();

                var _Respuesta = new PFF_ESTUDIO_MEDICO()
                {
                    ALIAS = AliasImputadoMedicoFederal,
                    ANTE_HEREDO_FAM = AntecedentesHeredoFam,
                    ANTE_PATOLOGICOS = DescripcionAntecedentesPatoMedicoFederal,
                    ANTE_PERSONAL_NO_PATOLOGICOS = AntecedenterPersonalesNoPato,
                    CONCLUSION = Conclusion,
                    DELITO = DescripcionDelitoMedicoFederal,
                    DIAGNOSTICO = Diagnostico,
                    EDAD = EdadImputadoMedicoFederal,
                    EDO_CIVIL = !string.IsNullOrEmpty(EstatusCivilImputado) ? EstatusCivilImputado : string.Empty,
                    ESTATURA = EstaturaFed,
                    EXP_FIS_ABDOMEN = Abdomen,
                    EXP_FIS_CABEZA_CUELLO = ExploracionFisicaCabezaCuello,
                    EXP_FIS_EXTREMIDADES = Extemidades,
                    EXP_FIS_GENITALES = OrganosGenit,
                    EXP_FIS_TORAX = Torax,
                    FECHA = FechaEstudioMedicoFederal,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    INTERROGATORIO_APARATOS = InterrogatorioAparatosSist,
                    NOMBRE = NombreImputadoMedicoFederal,
                    OCUPACION_ACT = OcupacionActualImputado,
                    OCUPACION_ANT = OcupacionAnteriorImputado,
                    ORIGINARIO_DE = OriginarioImputado,
                    PADECIMIENTO_ACTUAL = DescripcionPadecMedicoFederal,
                    PULSO = PulsoFed,
                    RESPIRACION = RespiracionFed,
                    RESULTADOS_OBTENIDOS = TerapeuticaImplementadaYResult,
                    SENTENCIA = SentenciaDelito,
                    TA = string.Format("{0} / {1}", TensionFederalUno, TensionFederalDos),
                    TEMPERATURA = TemperaturaFed,
                    MEDICO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                                     !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                                     !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty)
                    : string.Empty : string.Empty : string.Empty,
                    DIRECTOR_CENTRO = NombreDirector != null ? string.Format("{0}", !string.IsNullOrEmpty(NombreDirector.DIRECTOR) ? NombreDirector.DIRECTOR.Trim() : string.Empty) : string.Empty,
                    LUGAR = LugarMedico,
                    TATUAJES = DescripcionTatuajesCicatrRecientes,
                    ASIST_AA = IsAlcohlicosAnonChecked == true ? "S" : "N",
                    ASIST_FARMACODEPENDENCIA = IsFarmacoDepenChecked == true ? "S" : "N",
                    ASIST_OTROS = IsOtrosGruChecked == true ? "S" : "N",
                    ASIST_OTROS_ESPECIF = EspecifiqueOtraDependencia
                };

                if (LstSustToxicas != null && LstSustToxicas.Any())
                    foreach (var item in LstSustToxicas)
                        _Respuesta.PFF_SUSTANCIA_TOXICA.Add(item);

                return _Respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        private PFF_ESTUDIO_PSICOLOGICO GuardadoPsicologicoFueroFederal()
        {
            try
            {
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                var NombreDirector = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                var _Respuesta = new PFF_ESTUDIO_PSICOLOGICO()
                {
                    ACTITUD = ActitudTomadaEntrev,
                    CI = CoeficienteInt,
                    DELITO = DelifoFFl,
                    DINAM_PERSON_ACTUAL = DinamicaPersActual,
                    DINAM_PERSON_INGRESO = DinamicaPersonIngreso,
                    DIRECTOR_DENTRO = NombreDirector != null ? string.Format("{0}", !string.IsNullOrEmpty(NombreDirector.DIRECTOR) ? NombreDirector.DIRECTOR.Trim() : string.Empty) : string.Empty,
                    EDAD = EdadImpFF,
                    ESPECIFIQUE = EspecifiqueContTrat,
                    EXAMEN_MENTAL = ExMentFF,
                    EXTERNO = ExternoFF,
                    FECHA = FecEstudioPsicoFF,
                    INDICE_LESION_ORGANICA = IndiceLesionOrg,
                    INTERNO = InternoFF,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    NIVEL_INTELECTUAL = NivelIntelec,
                    NOMBRE = NombreImpFF,
                    OPINION = OpinionSobreBeneficio,
                    PRONOSTICO_REINTEGRACION = PronosticoReinsercionSocial,
                    PRUEBAS_APLICADAS = PruebasAplic,
                    PSICOLOGO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                                     !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                                     !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty)
                                                     : string.Empty : string.Empty : string.Empty,
                    REQ_CONT_TRATAMIENTO = IdRequiereContTratam,
                    RESULT_TRATAMIENTO = ResultTratPropArea,
                    SOBRENOMBRE = SobreNombFF,
                    LUGAR = LugarPsico
                };

                return _Respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        private PFF_TRABAJO_SOCIAL GuardadoTrabajoSocialFueroFederal()
        {
            try
            {
                var NombreDirector = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                var _Respuesta = new PFF_TRABAJO_SOCIAL()
                {
                    ALIMENTACION_FAM = DescripcionAlimentacionFamiliar,
                    APORTACIONES_FAM = DescripcionAportacionesEconomicasQuienCuanto,
                    APOYO_FAM_OTROS = IdCuentaApoyoFamiliaOtraPersona,
                    CARACT_FP_ANTECE_PENALES_ADIC = IdAlgunIntegranteCuentaAntecedentesAdiccion,
                    CARACT_FP_ANTECEDENTES_PENALES = EspecifiqueAdiccion,
                    CARACT_FP_CONCEPTO = ConceptoTieneFamiliaInterno,
                    CARACT_FP_GRUPO = IdGrupoFamiliarPrim,
                    CARACT_FP_NIVEL_SOCIO_CULTURAL = IdNivelSocioEconoPrim,
                    CARACT_FP_RELAC_INTERFAM = IdRelacionesInterf,
                    CARACT_FP_VIOLENCIA_FAM = IdHuboViolenciaIntro,
                    CARACT_FP_VIOLENCIA_FAM_ESPEFI = EspecifiqueViolenciaIntro,
                    CARACT_FS_GRUPO = IdGrupoFamiliarSecundario,
                    CARACT_FS_HIJOS_ANT = CantidadHijosUnionesAnteriores,
                    CARACT_FS_NIVEL_SOCIO_CULTURAL = IdNivelSocioEconCulturalSec,
                    CARACT_FS_PROBLEMAS_CONDUCTA = IdMiembroProblemasConductaAntisocial,
                    CARACT_FS_PROBLEMAS_CONDUCTA_E = EspecifiqueProblemasConductaAntisocial,
                    CARACT_FS_RELACION_MEDIO_EXT = RelacionMedioExterno,
                    CARACT_FS_RELACIONES_INTERFAM = IdRelacionInterFamSec,
                    CARACT_FS_VIOLENCIA_INTRAFAM = IDHuboViolenciaIntraFam,
                    CARACT_FS_VIOLENCIA_INTRAFAM_E = EspecifiqueViolenciaIntraFam,
                    CARACT_FS_VIVIEN_DESCRIPCION = DescripcionVivienda,
                    CARACT_FS_VIVIEN_MOBILIARIO = EnseresMobiliarioDomestico,
                    CARACT_FS_VIVIEN_NUM_HABITACIO = CantidadHabitacionesTotal,
                    CARACT_FS_VIVIEN_TRANSPORTE = TransporteCercaVivienda,
                    CARACT_FS_ZONA = IdCaracZonaSec,
                    DIAG_SOCIAL_PRONOS = DiagnosticoPronosticoExternacion,
                    DIALECTO = Dialecto,
                    DIRECTOR_CENTRO = NombreDirector != null ? !string.IsNullOrEmpty(NombreDirector.DIRECTOR) ? NombreDirector.DIRECTOR.Trim() : string.Empty : string.Empty,
                    DISTRIBUCION_GASTO_FAM = DsitribucionGastoFamiliar,
                    DOMICILIO = Domicilio,
                    ECO_FP_COOPERA_ACTUALMENTE = IdActualmenteCooperaEconoConFamilia,
                    ECO_FP_FONDOS_AHORRO = TieneFondosAhorro,
                    ECO_FP_RESPONSABLE = ResponsableManutHogar,
                    ECO_FP_TOTAL_EGRESOS_MEN = TotalEgresosMensuales,
                    ECO_FP_TOTAL_INGRESOS_MEN = TotalIngresosMensuales,
                    ECO_FP_ZONA = IdCaractZona,
                    EDO_CIVIL = IdEdoCivilTSFF,
                    ESCOLARIDAD_ACTUAL = EscolaridadActual,
                    ESCOLARIDAD_CENTRO = EscolaridadIngreso,
                    EXTERNADO_VIVIR_NOMBRE = ConQuienViviraSerExternado,
                    EXTERNADO_CALLE = NombreCalleExternado,
                    EXTERNADO_CIUDAD = IdCiudadQuienViviraSerExternado,
                    EXTERNADO_CP = CodPosQuienViviraSerExternado,
                    EXTERNADO_NUMERO = NoQuienViviraSerExternado,
                    EXTERNADO_PARENTESCO = IdParentescoQuienViviraSerExternado,
                    FECHA = FecEstudioSocioFF,
                    FECHA_NAC = FechaNacimientoTSFederal,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    INFLUENCIADO_ESTANCIA_PRISION = DeQueManeraLeInfluenciaEstanciaPrision,
                    LUGAR = LugarTS,
                    LUGAR_NAC = LugarFechaNac,
                    NOMBRE = NombbreImptSocioFF,
                    NUM_PAREJAS_ESTABLE = NumeroParejasVividoManeraEstable,
                    OCUPACION_ANT = OcupacionAntesIngreso,
                    OFERTA_TRABAJO = IdCuentaOfertaTrabajo,
                    OFERTA_TRABAJO_CONSISTE = ConsisteOfertaFF,
                    OPINION_CONCESION_BENEFICIOS = OpinionConcesionBeneficio,
                    OPINION_INTERNAMIENTO = OpicionAcercaInternamiento,
                    RADICAN_ESTADO = IdRadicanEstado,
                    SERVICIOS_PUBLICOS = DescripcionServiciosCuenta,
                    SUELDO_PERCIBIDO = SueldoPercibido,
                    TIEMPO_LABORAR = TiempoLaborar,
                    VISITA_FRECUENCIA = FrecuenciaVisitas,
                    EXTERNADO_COLONIA = EColonia,
                    EXTERNADO_MUNICIPIO = EMunicipio,
                    EXTERNADO_ENTIDAD = EEstado,
                    AVAL_MORAL_PARENTESCO = IdParentAvalTSFF.HasValue ? IdParentAvalTSFF.Value.ToString() : string.Empty,
                    AVAL_MORAL = NombreAvalMoral,
                    TRABAJADORA_SOCIAL = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                                     !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                                     !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty)
                                                     : string.Empty : string.Empty : string.Empty,
                    TRABAJO_DESEMP_ANTES = TrabajoDesempeniadoAntesReclusion,
                    VISITA_FAMILIARES = IdRecibeVisitasFamiliares,
                    VISITA_OTROS_QUIIEN = QuienesVisitanOtrasPersonas,
                    VISITAS_OTROS = IsVisitadoPorOtrasPersonas,
                    VISTA_PARENTESCO = IdParentescoVisTSFF
                };

                if (LstGrupoFam != null && LstGrupoFam.Any())
                    foreach (var item in LstGrupoFam)
                        _Respuesta.PFF_GRUPO_FAMILIAR.Add(item);

                if (LstGrupoFamSecu != null && LstGrupoFamSecu.Any())
                    foreach (var item in LstGrupoFamSecu)
                        _Respuesta.PFF_GRUPO_FAMILIAR.Add(item);

                return _Respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        private PFF_CAPACITACION GuardadoCapacitacionFueroFederal()
        {
            try
            {
                var NombreDirector = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_PROGRAMAS).FirstOrDefault();//PROGRAMAS PRODUCTIVOS
                var _Respuesta = new PFF_CAPACITACION()
                {
                    A_TOTAL_DIAS_LABORADOS = DiasLaboradosEfectivos,
                    ACTITUDES_DESEMPENO_ACT = ActitudesHaciaDesempenioActivProd,
                    ACTIVIDAD_PRODUC_ACTUAL = ActivEnCentroActual,
                    ATIENDE_INDICACIONES = IdAtiendeIndicacionesSup,
                    B_DIAS_LABORADOS_OTROS_CERESOS = DiasOtrosCentros,
                    CAMBIO_ACTIVIDAD = IdCambiadoActiv,
                    CAMBIO_ACTIVIDAD_POR_QUE = ExpecifiqueCambioAct,
                    CONCLUSIONES = ConclusionesActivProdCapac,
                    DESCUIDADO_LABORES = IdDescuidadoCumplimientoLab,
                    DIRECTOR_CENTRO = NombreDirector != null ? string.Format("{0}", !string.IsNullOrEmpty(NombreDirector.DIRECTOR) ? NombreDirector.DIRECTOR.Trim() : string.Empty) : string.Empty,
                    FECHA = FecCapacitFF,
                    FONDO_AHORRO = IdCuentaFondoA,
                    FONDO_AHORRO_COMPESACION_ACTUA = EspecifiqueCompensacion,
                    HA_PROGRESADO_OFICIO = IdProgresoOficio,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    MOTIVO_TIEMPO_INTERRUP_ACT = MotivosTiempoInterrupcionesActividad,
                    NO_CURSOS_MOTIVO = EspecifiqueNoCursos,
                    NOMBRE = NombreInternoCpaFF,
                    OFICIO_ANTES_RECLUSION = OficioActivDesempenadaAntesR,
                    RECIBIO_CONSTANCIA = IdRecibioConstancia,
                    SALARIO_DEVENGABA_DETENCION = SalarioPercib,
                    SATISFACE_ACTIVIDAD = IdSatisfaceActiv,
                    SECCION = SeccionInterno,
                    TOTAL_A_B = DiasLaboradosEfectivos + DiasOtrosCentros,
                    LUGAR = LugarProd,
                    JEFE_SECC_INDUSTRIAL = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty
                };

                if (LstCursosCapacitacionFederal != null && LstCursosCapacitacionFederal.Any())
                    foreach (var item in LstCursosCapacitacionFederal)
                        _Respuesta.PFF_CAPACITACION_CURSO.Add(item);

                if (LstDiasLaborados != null && LstDiasLaborados.Any())
                    foreach (var item in LstDiasLaborados)
                        _Respuesta.PFF_DIAS_LABORADO.Add(item);

                return _Respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        private PFF_ACTIVIDAD GuardadoActividadesEducFueroFederal()
        {
            try
            {
                var NombreDirector = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_EDUCATIVO).FirstOrDefault();

                var _Respuesta = new PFF_ACTIVIDAD()
                {
                    ALFABE_PRIMARIA = IsAlfabAPrimariaChecked == true ? "S" : "N",
                    ASISTE_PUNTUAL = !string.IsNullOrEmpty(IdAsisteEscuelaVountariamente) ? IdAsisteEscuelaVountariamente : string.Empty,
                    ASISTE_PUNTUAL_NO_POR_QUE = EspecifiqueNoAsisteEscuelaVoluntariamente,
                    AVANCE_RENDIMIENTO_ACADEMINCO = AvanceYRendimientoAcademico,
                    BACHILLER_UNI = IsBacAUnivChecked == true ? "S" : "N",
                    CONCLUSIONES = ConclusionAcadem,
                    DIRECTOR_CENTRO = NombreDirector != null ? string.Format("{0}", !string.IsNullOrEmpty(NombreDirector.DIRECTOR) ? NombreDirector.DIRECTOR.Trim() : string.Empty) : string.Empty,
                    ESCOLARIDAD_MOMENTO = EscolaridadMomentoDetencion,
                    ESPECIFIQUE = EspecifiqueOtroAcademico,
                    ESTUDIOS_ACTUALES = EstudiosCursaActualmente,
                    ESTUDIOS_EN_INTERNAMIENTO = EstudiosRealizadosInternamiento,
                    FECHA = FecEstudioInformeActivFF,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    IMPARTIDO_ENSENANZA = HaImpartidoAlgunaEnsenanza,
                    IMPARTIDO_ENSENANZA_TIEMPO = CuantoTiempo,
                    IMPARTIDO_ENSENANZA_TIPO = TipoEnsenanza,
                    NOMBRE = NombreImpInfActivFF,
                    OTRO = IsOtroAcademChecked ? "S" : "N",
                    PRIMARIA_SECU = IsPrimaASecChecked ? "S" : "N",
                    PROMOVIDO = HaSidoPromovido,
                    SECU_BACHILLER = IsSecABacChecked ? "S" : "N",
                    LUGAR = LugarEduc,
                    OTRA_ENSENANZA = QueOtraEnsenanaRecibe,
                    OTROS_PROGRAMAS = DetalleOtrosProgAss,
                    JEFE_SECC_EDUCATIVA = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty
                };

                if (LstActividadPart != null && LstActividadPart.Any())
                    foreach (var item in LstActividadPart)
                        _Respuesta.PFF_ACTIVIDAD_PARTICIPACION.Add(item);

                return _Respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        private PFF_VIGILANCIA GuardadoVigilanciaFueroFederal()
        {
            try
            {
                var NombreDirector = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_COMANDANCIA).FirstOrDefault();
                var _Respuesta = new PFF_VIGILANCIA()
                {
                    CENTRO_DONDE_PROCEDE = NombreCentroProcede,
                    CONCLUSIONES = ConclusionesGrales,
                    CONDUCTA = IdConducta,
                    CONDUCTA_FAMILIA = IdConductaConFam,
                    CONDUCTA_GENERAL = IdClasificConductaGral,
                    CONDUCTA_SUPERIORES = IdConductaSuperiores,
                    DESCRIPCION_CONDUCTA = DescripcionConducta,
                    DIRECTOR_CENTRO = NombreDirector != null ? string.Format("{0}", !string.IsNullOrEmpty(NombreDirector.DIRECTOR) ? NombreDirector.DIRECTOR.Trim() : string.Empty) : string.Empty,
                    ESTIMULOS_BUENA_CONDUCTA = EstimulosBuenaConducta,
                    FECHA = FecVigiFF,
                    FECHA_INGRESO = FecIngresoImputado,
                    HIGIENE_CELDA = IdHigieneEnCelda,
                    HIGIENE_PERSONAL = IdHigienePersonal,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    MOTIVO_TRASLADO = MotivoTraslado,
                    NOMBRE = NombreImpVigilanciaFF,
                    RELACION_COMPANEROS = RelacionCompaneros,
                    LUGAR = LugarVigi,
                    VISITA_RECIBE = IdRecibeVisita,
                    JEFE_VIGILANCIA = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty,
                    VISITA_FRECUENCIA = RecibeVisitaFrecuencia,
                    VISITA_QUIENES = QuienesRecibeVisita
                };

                if (LstCorrectivosFF != null && LstCorrectivosFF.Any())
                    foreach (var item in LstCorrectivosFF)
                        _Respuesta.PFF_CORRECTIVO.Add(item);

                return _Respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private PFF_CRIMINOLOGICO GuardadoCriminologicoFueroFederal()
        {
            try
            {
                string _NombreEduc = string.Empty;
                var NombreDirector = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                var _Respuesta = new PFF_CRIMINOLOGICO()
                {
                    ANTECEDENTES_PARA_ANTI_SOCIALE = AntecedentesParaSocialesAntisociales,
                    CRIMINOLOGO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty)
                        : string.Empty : string.Empty : string.Empty,
                    DIRECTOR_CENTRO = NombreDirector != null ? string.Format("{0}", !string.IsNullOrEmpty(NombreDirector.DIRECTOR) ? NombreDirector.DIRECTOR.Trim() : string.Empty) : string.Empty,
                    FECHA = FecCriminFF,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    NOMBRE = NombreImpCriminFF,
                    P1_VERSION_INTERNO = VersionDelitoCriminFF,
                    P10_CONTINUAR_NO_ESPECIFICAR = NegatEspecifFF,
                    P10_CONTINUAR_SI_ESPECIFICAR = AfirmaEspecifFF,
                    P10_OPINION = OpinionSobreConBeneficio,
                    P2_PERSONALIDAD = CaractPersonalesRelacionadasDelito,
                    P3_VALORACION = IdRequiereValoracionCrimin,
                    P6_CRIMINOGENESIS = CriminogenesisCrimFF,
                    P7_AGRESIVIDAD = Agresividad,
                    P7_EGOCENTRISMO = Egocentrismo,
                    P7_INDIFERENCIA = IndAfectiva,
                    P7_LABILIDAD = LabAfectiva,
                    P10_CONTINUAR_TRATAMIENTO = ReqTrataExtraMurosCriminFF,
                    P8_ESTADO_PELIGRO = IdEstadoPeligrosidad,
                    P8_RESULTADO_TRATAMIENTO = ResultadoTratamientoInst,
                    P9_PRONOSTICO = IdPronReinciFF,
                    SOBRENOMBRE = SobreN,
                    P5_PRIMODELINCUENTE = PrimoCheckedFederal ? "S" : string.Empty,
                    P5_ESPECIFICO = EspecificoCheckedFederal ? "S" : string.Empty,
                    P5_GENERICO = GenericoCheckedFederal ? "S" : string.Empty,
                    P5_HABITUAL = HabitualCheckedFederal ? "S" : string.Empty,
                    P5_PROFESIONAL = ProfesionalCheckedFederal ? "S" : string.Empty,
                    LUGAR = LugarCrimi
                };

                return _Respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion



        #region Complementarios y listas federales

        private void ConsultaVisitasPadron()
        {
            try
            {
                LstPV = new ObservableCollection<Clases.GrupoFamiliarPV>();
                if (SelectIngreso.VISITA_AUTORIZADA != null)
                {
                    foreach (var p in SelectIngreso.VISITA_AUTORIZADA)
                    {
                        LstPV.Add(
                            new Clases.GrupoFamiliarPV()
                            {
                                Seleccionado = false,
                                Nombre = p.NOMBRE,
                                Paterno = p.PATERNO,
                                Materno = p.MATERNO,
                                IdReferencia = p.ID_PARENTESCO,
                                TipoReferencia = p.TIPO_REFERENCIA,
                                IdGrupo = 1,
                                ViveConEl = false,
                                ID_ANIO = p.ID_ANIO,
                                ID_CENTRO = p.ID_CENTRO,
                                ID_IMPUTADO = p.ID_IMPUTADO,
                                ID_INGRESO = p.ID_INGRESO,
                                ID_VISITA = p.ID_VISITA,
                                Edad = p.EDAD,
                                IdOcupacion = -1,
                                IdEstadoCivil = -1,
                                Domicilio = string.Empty,
                                FNacimiento = null
                            });
                    };
                };

                if (LstPV.Count > 0)
                    EmptyPadronVisita = false;
                else
                    EmptyPadronVisita = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar el padron de visitas", ex);
            }
        }

        private string MesString(short Mes)
        {
            try
            {
                string[] meses = { null, "DICIEMBRE", "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE" };
                int d = int.Parse(Mes.ToString());
                return meses[d];
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private string DiaString(short Dia)
        {
            try
            {
                string[] semana = { "DOMINGO", "LUNES", "MARTES", "MIERCOLES", "JUEVES", "VIERNES", "SABADO" };
                int d = int.Parse(Dia.ToString());
                return semana[d];
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private string DiaStringByDayOfWeek(int Dia)
        {
            try
            {
                string[] semana = { "DOMINGO", "LUNES", "MARTES", "MIERCOLES", "JUEVES", "VIERNES", "SABADO" };
                return semana[Dia];
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        private string CalcularSentencia()
        {
            try
            {
                LstSentenciasIngresos = new ObservableCollection<SentenciaIngreso>();
                if (SelectIngreso != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (SelectIngreso.CAUSA_PENAL != null)
                    {
                        foreach (var cp in SelectIngreso.CAUSA_PENAL)
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

                    return anios + (anios == 1 ? " AÑO " : " AÑOS ") + meses + (meses == 1 ? " MES " : " MESES ") + dias + (dias == 1 ? " DÍA  " : " DÍAS ");
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }

            return string.Empty;
        }
        //public string CalcularSentencia(ICollection<CAUSA_PENAL> CausaPenal)
        //{
        //    try
        //    {
        //        if (CausaPenal != null)
        //        {
        //            int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
        //            DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
        //            if (CausaPenal != null)
        //            {
        //                foreach (var cp in CausaPenal)
        //                {
        //                    var segundaInstancia = false;
        //                    if (cp.SENTENCIA != null)
        //                    {
        //                        if (cp.SENTENCIA.Count > 0)
        //                        {
        //                            if (cp.RECURSO.Count > 0)
        //                            {
        //                                var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
        //                                if (r != null)
        //                                {
        //                                    var res = r.FirstOrDefault();
        //                                    if (res != null)
        //                                    {
        //                                        //SENTENCIA
        //                                        anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
        //                                        meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
        //                                        dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);
        //                                        segundaInstancia = true;
        //                                    }
        //                                }
        //                            }
        //                            var s = cp.SENTENCIA.FirstOrDefault();
        //                            if (s != null)
        //                            {
        //                                if (FechaInicioCompurgacion == null)
        //                                    FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
        //                                else
        //                                    if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
        //                                        FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;

        //                                //SENTENCIA
        //                                if (!segundaInstancia)
        //                                {
        //                                    anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
        //                                    meses = meses + (s.MESES != null ? s.MESES.Value : 0);
        //                                    dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);
        //                                }

        //                                //ABONO
        //                                anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
        //                                meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
        //                                dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            while (dias > 29)
        //            {
        //                meses++;
        //                dias = dias - 30;
        //            }
        //            while (meses > 11)
        //            {
        //                anios++;
        //                meses = meses - 12;
        //            }

        //            if (FechaInicioCompurgacion != null)
        //            {
        //                FechaFinCompurgacion = FechaInicioCompurgacion;
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
        //                //
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

        //                int a = 0, m = 0, d = 0;
        //                new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
        //                a = m = d = 0;
        //                new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);

        //                return (a + (a == 1 ? " Año " : " Años ") + m + (m == 1 ? " Mes " : " Meses ") + d + (d == 1 ? " Día " : " Días ")).ToUpper();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
        //    }
        //    return string.Empty;
        //}
        private void LoadListasFederales()
        {
            try
            {
                if (LstOcupaciones == null)
                {
                    LstOcupaciones = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos());
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

                if (LstEscolaridadesGrupoFamTSFF == null)
                {
                    LstEscolaridadesGrupoFamTSFF = new ObservableCollection<ESCOLARIDAD>(new cEscolaridad().ObtenerTodos(string.Empty));
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstEscolaridadesGrupoFamTSFF.Insert(0, (new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" }));
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

                if (ListProgramas == null)
                {
                    ListProgramas = new ObservableCollection<TIPO_PROGRAMA>(new cTipoPrograma().ObtenerTodos());
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListProgramas.Insert(0, (new TIPO_PROGRAMA() { ID_TIPO_PROGRAMA = -1, DESCR = "SELECCIONE" }));
                    }));
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
                string EstadoSist, EdoNuevo = string.Empty;
                var EstadoActual = new cEntidad().GetData(x => x.ID_ENTIDAD == Parametro.ESTADO).FirstOrDefault();
                var _Municipio = new cMunicipio().GetData(x => x.ID_MUNICIPIO == 2 && x.ID_ENTIDAD == 2).FirstOrDefault();
                EstadoSist = EstadoActual != null ? string.Concat(_Municipio != null ? !string.IsNullOrEmpty(_Municipio.MUNICIPIO1) ? _Municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty, " ", !string.IsNullOrEmpty(EstadoActual.DESCR) ? EstadoActual.DESCR.Trim() : string.Empty) : string.Empty;
                LugarMedico = string.Format("{0} A {1} ", EstadoSist, Fechas.GetFechaDateServer.ToString(" dd , MMMM , yyyy", System.Globalization.CultureInfo.CurrentCulture).ToUpper());
                if (!string.IsNullOrEmpty(LugarMedico))
                    EdoNuevo = LugarMedico.Replace(" , ", " DE ");
                else
                    EdoNuevo = LugarMedico;

                LugarMedico = EdoNuevo;
                return LugarMedico;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion
    }
}
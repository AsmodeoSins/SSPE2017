using System.Linq;
using MoreLinq;
using ControlPenales.BiometricoServiceReference;
namespace ControlPenales
{
    public partial class HojaEnfermeriaViewModel : ValidationViewModelBase
    {
        public HojaEnfermeriaViewModel() { }

        private void LoadHojaEnfermeria(HojaEnfermeriaView Window = null)
        {
            InicializaListas();
            ConfiguraPermisos();
        }

        private void LoadSolucionesWindow(SeccionSolucionesHEView Window = null)
        {
            try
            {
                if (Window == null)
                    return;

                AutoCompleteReceta = Window.AutoCompleteReceta;
                AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", Window.AutoCompleteReceta) as System.Windows.Controls.ListBox;
                AutoCompleteReceta.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(MouseUpReceta);
                AutoCompleteReceta.KeyDown += KeyDownReceta;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ConfiguraPermisos()
        {
            try
            {
                var _detalleUsuarioActual = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr).FirstOrDefault();
                if (_detalleUsuarioActual != null)
                {
                    #region PERMISOS MEDICOS
                    var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.HOJA_ENFERMERIA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                    foreach (var p in permisos)
                    {
                        if (p.CONSULTAR == 1)
                            PConsultar = MenuBuscarEnabled = BuscarImputadoHabilitado = true;
                        if (p.IMPRIMIR == 1)
                            PImprimir = MenuFichaEnabled = MenuReporteEnabled = true;
                        if (p.EDITAR == 1)
                            MenuGuardarEnabled = true;
                    };

                    #endregion
                };
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }

        private async System.Threading.Tasks.Task<System.Collections.Generic.List<SSP.Servidor.IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new System.Collections.Generic.List<SSP.Servidor.IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.IMPUTADO>>(() => new SSP.Controlador.Catalogo.Justicia.cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }

            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new System.Collections.Generic.List<SSP.Servidor.IMPUTADO>();
            }
        }

        private void ImprimeHojaEnfermeria()
        {
            try
            {
                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                    return;
                };

                ParametroCuerpoDorso = Parametro.CUERPO_DORSO;
                ParametroCuerpoFrente = Parametro.CUERPO_FRENTE;
                ParametroImagenZonaCorporal = Parametro.IMAGEN_ZONA_CORPORAL;

                ReportesView View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Show();

                var _validacionHospitalizado = new SSP.Controlador.Catalogo.Justicia.Medico.cHospitalizacion().ObtenerHospitalizacionesPorIngreso(SelectIngreso);
                var _UnicaH = _validacionHospitalizado != null ? _validacionHospitalizado.Any() ? _validacionHospitalizado.FirstOrDefault(x => x.ID_HOSPITA == SelectedHospitalizacion) : null : null;
                var _Enfermedades = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD : null : null;
                string _DatosDiagnotico = string.Empty;
                if (_Enfermedades != null && _Enfermedades.Any())
                    foreach (var item in _Enfermedades)
                        _DatosDiagnotico += string.Format("{0}, ", item.ENFERMEDAD != null ? !string.IsNullOrEmpty(item.ENFERMEDAD.NOMBRE) ? item.ENFERMEDAD.NOMBRE.Trim() : string.Empty : string.Empty);

                string DiagnosticoSinComa = !string.IsNullOrEmpty(_DatosDiagnotico) ? _DatosDiagnotico.TrimEnd(',') : string.Empty;
                cHojaEnfermeriaEncabezadoResporte _DatosCabezera = new cHojaEnfermeriaEncabezadoResporte()
                {
                    Cama = _UnicaH != null ? _UnicaH.CAMA_HOSPITAL != null ? !string.IsNullOrEmpty(_UnicaH.CAMA_HOSPITAL.DESCR) ? _UnicaH.CAMA_HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                    Diagnostico = DiagnosticoSinComa,
                    Edad = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? new Fechas().CalculaEdad(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Exp = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? string.Format("{0} / {1}", _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO, _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO) : string.Empty : string.Empty : string.Empty,
                    NombrePaciente = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ?
                        string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty)
                                 : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    FechaNacimiento = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA.HasValue ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Sexo = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.SEXO == "M" ? "MASCULINO" : _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.SEXO == "F" ? "FEMENINO" : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Peso = _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                    Fecha = Fechas.GetFechaDateServerString,
                    FechaIngreso = _UnicaH.INGRESO_FEC.HasValue ? _UnicaH.INGRESO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                };

                System.Collections.Generic.List<cMedicamentosHojaEnfermeriaReporte> listaMedicamentosHojaEnfermeria = new System.Collections.Generic.List<cMedicamentosHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cHojaEnfermeriaSignosVitalesReporte> lstSignosVitales = new System.Collections.Generic.List<cHojaEnfermeriaSignosVitalesReporte>();
                System.Collections.Generic.List<cNotasEnfermeriaHojaEnfermeriaReporte> lstNotasEnfermeria = new System.Collections.Generic.List<cNotasEnfermeriaHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cSolucionesHojaEnfermeriaReporte> lstSolucionesHojaEnfermeria = new System.Collections.Generic.List<cSolucionesHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte> lstIngresosHojaEnfermeria = new System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte> lstIngresosHojaEnfermeriaFinal = new System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte> lstEgresosHojaEnfermeriaFinal = new System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cReporteCertificadoNuevoIngreso> listaUlceras = new System.Collections.Generic.List<cReporteCertificadoNuevoIngreso>();
                System.Collections.Generic.List<cReporteCertificadoNuevoIngreso> listaL = new System.Collections.Generic.List<cReporteCertificadoNuevoIngreso>();
                System.Collections.Generic.List<LesionesCustom> Lesiones = new System.Collections.Generic.List<LesionesCustom>();
                System.Collections.Generic.List<cCateteresReporteHojaEnfermeria> lstCateteresReporte = new System.Collections.Generic.List<cCateteresReporteHojaEnfermeria>();
                System.Collections.Generic.List<cSondaReporteHojaEnfermeria> lstSondasReporte = new System.Collections.Generic.List<cSondaReporteHojaEnfermeria>();
                System.Collections.Generic.List<cRayosXHojaenfermeriaReporte> lstRayosXReporte = new System.Collections.Generic.List<cRayosXHojaenfermeriaReporte>();
                System.Collections.Generic.List<cLaboratoriosHojaenfermeriaReporte> listLaboratoriosReporte = new System.Collections.Generic.List<cLaboratoriosHojaenfermeriaReporte>();

                var _TurnosDisponibles = new SSP.Controlador.Catalogo.Justicia.cLiquidoTurno().GetData(x => x.ESTATUS == "S");
                var _DatosHoja = new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeria().GetData(x => x.FECHA_REGISTRO.Value.Year == FechaHojaenfermeria.Value.Year && x.FECHA_REGISTRO.Value.Month == FechaHojaenfermeria.Value.Month && x.FECHA_REGISTRO.Value.Day == FechaHojaenfermeria.Value.Day && x.ID_HOSPITA == SelectedHospitalizacion);
                if (_DatosHoja != null && _DatosHoja.Any())
                {
                    foreach (var item in _DatosHoja)
                    {
                        if (item.HOJA_ENFERMERIA_LECTURA != null && item.HOJA_ENFERMERIA_LECTURA.Any())
                            foreach (var item2 in item.HOJA_ENFERMERIA_LECTURA)
                                lstSignosVitales.Add(new cHojaEnfermeriaSignosVitalesReporte
                                {
                                    CambioPosicion = item2.CAMBIO_POSICION == "S" ? "SI" : item2.CAMBIO_POSICION == "N" ? "NO" : string.Empty,
                                    FC = !string.IsNullOrEmpty(item2.PC) ? item2.PC.Trim() : string.Empty,
                                    FR = !string.IsNullOrEmpty(item2.PR) ? item2.PR.Trim() : string.Empty,
                                    SAO = !string.IsNullOrEmpty(item2.SAO) ? item2.SAO.Trim() : string.Empty,
                                    NEB = !string.IsNullOrEmpty(item2.NEB) ? item2.NEB.Trim() : string.Empty,
                                    RiesgoEscara = item2.CAMBIO_ESCARAS == "S" ? "SI" : item2.CAMBIO_ESCARAS == "N" ? "NO" : string.Empty,
                                    Dextr = !string.IsNullOrEmpty(item2.DEXTROXTIX) ? item2.DEXTROXTIX.Trim() : string.Empty,
                                    PVC = !string.IsNullOrEmpty(item2.PVC) ? item2.PVC.Trim() : string.Empty,
                                    TA = !string.IsNullOrEmpty(item2.TA) ? item2.TA.Trim() : string.Empty,
                                    RiesgoCaidas = item2.RIESGO_CAIDAS == "S" ? "SI" : item2.RIESGO_CAIDAS == "N" ? "NO" : string.Empty,
                                    TAMedia = !string.IsNullOrEmpty(item2.TA_MEDIA) ? item2.TA_MEDIA.Trim() : string.Empty,
                                    Temp = !string.IsNullOrEmpty(item2.TEMP) ? item2.TEMP.Trim() : string.Empty,
                                    Generico = item2.FECHA_LECTURA.HasValue ? item2.FECHA_LECTURA.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty
                                });

                        if (item.HOJA_ENFERMERIA_ULCERA != null && item.HOJA_ENFERMERIA_ULCERA.Any())
                            foreach (var item2 in item.HOJA_ENFERMERIA_ULCERA)
                            {
                                Lesiones.Add(new LesionesCustom
                                    {
                                        DESCR = item2.DESC,
                                        REGION = item2.ANATOMIA_TOPOGRAFICA
                                    });
                            };

                        if (!string.IsNullOrEmpty(item.RX))
                            lstRayosXReporte.Add(new cRayosXHojaenfermeriaReporte
                                {
                                    Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                    RayosX = item.RX
                                });

                        if (!string.IsNullOrEmpty(item.LABORATORIO))
                            listLaboratoriosReporte.Add(new cLaboratoriosHojaenfermeriaReporte
                                {
                                    Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                    Laboratorio = item.LABORATORIO
                                });

                        if (item.HOJA_ENFERMERIA_CATETER != null && item.HOJA_ENFERMERIA_CATETER.Any())
                            foreach (var item2 in item.HOJA_ENFERMERIA_CATETER)
                            {
                                lstCateteresReporte.Add(new cCateteresReporteHojaEnfermeria
                                    {
                                        FechaInstalado = item2.INSTALACION_FEC.HasValue ? item2.INSTALACION_FEC.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                        Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                        FechaRetirado = item2.REGISTRO_FEC.HasValue ? item2.REGISTRO_FEC.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                        Retirado = item2.RETIRO == "S" ? "SI" : item2.RETIRO == "N" ? "NO" : string.Empty,
                                        TipoCatater = item2.CATETER_TIPO != null ? !string.IsNullOrEmpty(item2.CATETER_TIPO.DESCR) ? item2.CATETER_TIPO.DESCR.Trim() : string.Empty : string.Empty
                                    });
                            };

                        if (item.HOJA_ENFERMERIA_SONDA_GASOGAS != null && item.HOJA_ENFERMERIA_SONDA_GASOGAS.Any())
                            foreach (var item2 in item.HOJA_ENFERMERIA_SONDA_GASOGAS)
                            {
                                lstSondasReporte.Add(new cSondaReporteHojaEnfermeria
                                    {
                                        FechaInstalacion = item2.INSTALACION_FEC.HasValue ? item2.INSTALACION_FEC.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                        Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                        Retirado = item2.RETIRO == "S" ? "SI" : item2.RETIRO == "N" ? "NO" : string.Empty
                                    });
                            };

                        if (item.HOJA_CONTROL_ENFERMERIA != null && item.HOJA_CONTROL_ENFERMERIA.Any())
                            foreach (var item2 in item.HOJA_CONTROL_ENFERMERIA)
                            {
                                lstIngresosHojaEnfermeria.Add(new cIngresosHojaEnfermeriaReporte
                                    {
                                        Nombre = item2.LIQUIDO != null ? !string.IsNullOrEmpty(item2.LIQUIDO.DESCR) ? item2.LIQUIDO.DESCR.Trim() : string.Empty : string.Empty,
                                        Horas = item2.ID_LIQ,
                                        Generico3 = item2.CANT.HasValue ? item2.CANT.Value.ToString() : string.Empty,
                                        Hora = item2.LIQUIDO_HORA != null ? !string.IsNullOrEmpty(item2.LIQUIDO_HORA.DESCR) ? item2.LIQUIDO_HORA.DESCR.Trim() : string.Empty : string.Empty,
                                        Generico = item2.LIQUIDO != null ? item2.LIQUIDO.ID_LIQTIPO : string.Empty,
                                        IdTurn = item2.ID_LIQHORA.HasValue ? item2.LIQUIDO_HORA.ID_LIQTURNO : new decimal?()
                                    });
                            };

                        if (!string.IsNullOrEmpty(item.OBSERVACION))
                            lstNotasEnfermeria.Add(new cNotasEnfermeriaHojaEnfermeriaReporte
                                {
                                    Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                    Nota = !string.IsNullOrEmpty(item.OBSERVACION) ? item.OBSERVACION.Trim() : string.Empty
                                });

                        int _Consecutivo = new int();
                        if (_TurnosDisponibles.Any())
                            foreach (var item2 in _TurnosDisponibles)
                                if (item.HOJA_ENFERMERIA_SOLUCION != null && item.HOJA_ENFERMERIA_SOLUCION.Any())
                                {
                                    var SolucionesTurno = item.HOJA_ENFERMERIA_SOLUCION.Where(x => x.ID_LIQTURNO_INICIO == item2.ID_LIQTURNO);
                                    foreach (var item3 in SolucionesTurno)
                                    {
                                        _Consecutivo++;
                                        lstSolucionesHojaEnfermeria.Add(new cSolucionesHojaEnfermeriaReporte
                                        {
                                            Nombre = item3.ID_PRODUCTO.HasValue ? !string.IsNullOrEmpty(item3.PRODUCTO.NOMBRE) ? item3.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty,
                                            Generico = item3.ID_LIQTURNO_INICIO.HasValue ? !string.IsNullOrEmpty(item3.LIQUIDO_TURNO.DESCR) ? item3.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                            Consecutivo = _Consecutivo,
                                            Generico2 = item3.TERMINO == "S" ? "SI" : item3.TERMINO == "N" ? "NO" : string.Empty
                                        });
                                    };
                                };

                        if (item.HOJA_ENFERMERIA_MEDICAMENTO != null && item.HOJA_ENFERMERIA_MEDICAMENTO.Any())
                        {
                            var _refinados = item.HOJA_ENFERMERIA_MEDICAMENTO.OrderBy(x => x.FECHA_SUMINISTRO);
                            foreach (var item2 in _refinados)
                                listaMedicamentosHojaEnfermeria.Add(new cMedicamentosHojaEnfermeriaReporte
                                    {
                                        Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                        Cantidad = item2.CANTIDAD.HasValue ? item2.CANTIDAD.Value.ToString() : string.Empty,
                                        Cena = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.CENA == 1 ? "SI" : item2.RECETA_MEDICA_DETALLE.CENA == 2 ? "NO" : string.Empty : string.Empty,
                                        Comida = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.COMIDA == 1 ? "SI" : item2.RECETA_MEDICA_DETALLE.COMIDA == 2 ? "NO" : string.Empty : string.Empty,
                                        Desayuno = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.DESAYUNO == 1 ? "SI" : item2.RECETA_MEDICA_DETALLE.DESAYUNO == 2 ? "NO" : string.Empty : string.Empty,
                                        DuracionTratam = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.DURACION.HasValue ? item2.RECETA_MEDICA_DETALLE.DURACION.Value.ToString() : string.Empty : string.Empty,
                                        FecSuministro = item2.FECHA_SUMINISTRO.HasValue ? item2.FECHA_SUMINISTRO.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                        FechaReceto = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.RECETA_MEDICA != null ? item2.RECETA_MEDICA_DETALLE.RECETA_MEDICA.RECETA_FEC.HasValue ? item2.RECETA_MEDICA_DETALLE.RECETA_MEDICA.RECETA_FEC.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty : string.Empty,
                                        UnidadMedida = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.PRODUCTO != null ? item2.RECETA_MEDICA_DETALLE.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? !string.IsNullOrEmpty(item2.RECETA_MEDICA_DETALLE.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.DESCR) ? item2.RECETA_MEDICA_DETALLE.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                        PresentacionMedicamento = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.ID_PRESENTACION_MEDICAMENTO.HasValue ? !string.IsNullOrEmpty(item2.RECETA_MEDICA_DETALLE.PRESENTACION_MEDICAMENTO.DESCRIPCION) ? item2.RECETA_MEDICA_DETALLE.PRESENTACION_MEDICAMENTO.DESCRIPCION.Trim() : string.Empty : string.Empty : string.Empty,
                                        Nombre = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.PRODUCTO != null ? !string.IsNullOrEmpty(item2.RECETA_MEDICA_DETALLE.PRODUCTO.NOMBRE) ? item2.RECETA_MEDICA_DETALLE.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty
                                    });
                        };
                    };

                    //AQUI SE COMPARA CON RESPECTO A LOS TURNOS QUE NO SE CAPTURARON PARA AGREGARLOS AL FORMATO DEL REPORTE Y VISUALIZAR LOS 3 TURNOS
                    int _Cons = 1;
                    if (_TurnosDisponibles != null && _TurnosDisponibles.Any())
                        foreach (var item in _TurnosDisponibles)
                        {
                            if (!string.IsNullOrEmpty(item.DESCR))
                            {
                                if (lstNotasEnfermeria != null && lstNotasEnfermeria.Any())
                                    if (!lstNotasEnfermeria.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        lstNotasEnfermeria.Add(new cNotasEnfermeriaHojaEnfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });
                                    else { }
                                else///NUNCA HA EXISTIDO, HAY QUE AGREGARLO AL ESQUELETO DEL REPORTE
                                    lstNotasEnfermeria.Add(new cNotasEnfermeriaHojaEnfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });

                                if (lstCateteresReporte != null && lstCateteresReporte.Any())
                                    if (!lstCateteresReporte.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        lstCateteresReporte.Add(new cCateteresReporteHojaEnfermeria { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });
                                    else { /*no action */}
                                else
                                    lstCateteresReporte.Add(new cCateteresReporteHojaEnfermeria { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });

                                if (lstRayosXReporte != null && lstRayosXReporte.Any())
                                    if (!lstRayosXReporte.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        lstRayosXReporte.Add(new cRayosXHojaenfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });
                                    else { }
                                else
                                    lstRayosXReporte.Add(new cRayosXHojaenfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });

                                if (listLaboratoriosReporte != null && listLaboratoriosReporte.Any())
                                    if (!listLaboratoriosReporte.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        listLaboratoriosReporte.Add(new cLaboratoriosHojaenfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });
                                    else { }
                                else
                                    listLaboratoriosReporte.Add(new cLaboratoriosHojaenfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });

                                if (lstSondasReporte != null && lstSondasReporte.Any())
                                    if (!lstSondasReporte.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        lstSondasReporte.Add(new cSondaReporteHojaEnfermeria { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });
                                    else { }
                                else
                                    lstSondasReporte.Add(new cSondaReporteHojaEnfermeria { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });

                                if (lstSolucionesHojaEnfermeria != null && lstSolucionesHojaEnfermeria.Any())
                                    if (!lstSolucionesHojaEnfermeria.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                    {
                                        lstSolucionesHojaEnfermeria.Add(new cSolucionesHojaEnfermeriaReporte
                                        {
                                            Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                                            //Consecutivo = _Cons
                                        });
                                    }
                                    else { }
                                else
                                    lstSolucionesHojaEnfermeria.Add(new cSolucionesHojaEnfermeriaReporte
                                    {
                                        Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                                        //Consecutivo = _Cons
                                    });

                                if (listaMedicamentosHojaEnfermeria != null && listaMedicamentosHojaEnfermeria.Any())
                                    if (!listaMedicamentosHojaEnfermeria.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        listaMedicamentosHojaEnfermeria.Add(new cMedicamentosHojaEnfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty/*, Nombre = string.Empty, Cantidad = string.Empty*/ });
                                    else { }
                                else
                                    listaMedicamentosHojaEnfermeria.Add(new cMedicamentosHojaEnfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty/*, Nombre = string.Empty, Cantidad = string.Empty*/ });
                            };
                            _Cons++;
                        };
                }
                else
                {
                    //SI ENTRA AQUI ES PORQUE NO TIENE UNA HOJA DE ENFERMERIA REGISTRADA, CREA EL ESQUELETO BASE DEL REPORTE PARA DEFINIR LOS TURNOS
                    int _Cons = 1;
                    if (_TurnosDisponibles != null && _TurnosDisponibles.Any())
                        foreach (var item in _TurnosDisponibles)
                        {
                            lstSolucionesHojaEnfermeria.Add(new cSolucionesHojaEnfermeriaReporte
                            {
                                Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                            });

                            listaMedicamentosHojaEnfermeria.Add(new cMedicamentosHojaEnfermeriaReporte
                                {
                                    Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                                    //Nombre = string.Empty,
                                    //Cantidad = string.Empty
                                });

                            lstNotasEnfermeria.Add(new cNotasEnfermeriaHojaEnfermeriaReporte
                                {
                                    Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                                });

                            lstCateteresReporte.Add(new cCateteresReporteHojaEnfermeria
                                {
                                    Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                                });

                            lstRayosXReporte.Add(new cRayosXHojaenfermeriaReporte
                            {
                                Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                            });

                            listLaboratoriosReporte.Add(new cLaboratoriosHojaenfermeriaReporte
                            {
                                Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                            });

                            lstSondasReporte.Add(new cSondaReporteHojaEnfermeria
                                {
                                    Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                                });

                            _Cons++;
                        };
                }

                var _HorasDisponibles = new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S", y => y.ID_LIQHORA.ToString());//ESTAS SON LAS HORAS QUE TENGO PARA TRABAJAR
                var _LiquidosIngreso = new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "1" && x.ID_LIQ_FORMATO == 2);//ESTOS SON LOS LIQUIDOS QUE TNGO PARA TRABAJAR
                var _LiquidosEgreso = new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "2" && x.ID_LIQ_FORMATO == 2);
                var IngresosEncontrados = lstIngresosHojaEnfermeria != null ? lstIngresosHojaEnfermeria.Any() ? lstIngresosHojaEnfermeria.Where(x => x.Generico == "1") : null : null;
                var EgresosEncontrados = lstIngresosHojaEnfermeria != null ? lstIngresosHojaEnfermeria.Any() ? lstIngresosHojaEnfermeria.Where(x => x.Generico == "2") : null : null;
                #region Balances de liquidos de ingreso y egreso
                decimal? BalanceM, BalanceV, BalanceN = new decimal?();
                decimal? BalanceEM, BalanceEV, BalanceEN = new decimal?();
                BalanceM = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.MATUTUNO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.MATUTUNO).ESTATUS == "N" ? IngresosEncontrados != null ? IngresosEncontrados.Any() ? IngresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.MATUTUNO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                BalanceV = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.VESPERTINO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.VESPERTINO).ESTATUS == "N" ? IngresosEncontrados != null ? IngresosEncontrados.Any() ? IngresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.VESPERTINO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                BalanceN = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.NOCTURNO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.NOCTURNO).ESTATUS == "N" ? IngresosEncontrados != null ? IngresosEncontrados.Any() ? IngresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.NOCTURNO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                BalanceEM = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.MATUTUNO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.MATUTUNO).ESTATUS == "N" ? EgresosEncontrados != null ? EgresosEncontrados.Any() ? EgresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.MATUTUNO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                BalanceEV = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.VESPERTINO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.VESPERTINO).ESTATUS == "N" ? EgresosEncontrados != null ? EgresosEncontrados.Any() ? EgresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.VESPERTINO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                BalanceEN = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.NOCTURNO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.NOCTURNO).ESTATUS == "N" ? EgresosEncontrados != null ? EgresosEncontrados.Any() ? EgresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.NOCTURNO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                _DatosCabezera.Generico = BalanceM.HasValue ? BalanceM.Value.ToString() : string.Empty;
                _DatosCabezera.Generico2 = BalanceV.HasValue ? BalanceV.Value.ToString() : string.Empty;
                _DatosCabezera.Generico3 = BalanceN.HasValue ? BalanceN.Value.ToString() : string.Empty;
                _DatosCabezera.Generico4 = BalanceEM.HasValue ? BalanceEM.Value.ToString() : string.Empty;
                _DatosCabezera.Generico5 = BalanceEV.HasValue ? BalanceEV.Value.ToString() : string.Empty;
                _DatosCabezera.Generico6 = BalanceEN.HasValue ? BalanceEN.Value.ToString() : string.Empty;
                #endregion

                if (_HorasDisponibles != null && _HorasDisponibles.Any())
                {
                    string Gen = string.Empty;
                    if (_LiquidosIngreso.Any())
                        foreach (var item in _LiquidosIngreso)
                            Gen += string.Format("{0}\n", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty);//SIRVE COMO BASE DE LA MATRIZ

                    string Da = string.Empty;
                    if (_LiquidosEgreso.Any())
                        foreach (var item in _LiquidosEgreso)
                            Da += string.Format("{0}\n", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty);//SIRVE COMO BASE DE LA MATRIZ

                    foreach (var item in _HorasDisponibles)
                    {
                        #region INGRESOS
                        string Dat = string.Empty;
                        if (_LiquidosIngreso != null && _LiquidosIngreso.Any())
                        {
                            foreach (var item2 in _LiquidosIngreso)
                            {
                                if (IngresosEncontrados != null && IngresosEncontrados.Any())
                                {
                                    if (IngresosEncontrados.Any(x => x.Horas == item2.ID_LIQ && x.Hora == item.DESCR))
                                        Dat += string.Format("{0} \n", IngresosEncontrados.FirstOrDefault(x => x.Horas == item2.ID_LIQ).Generico3);
                                    else
                                        Dat += string.Format("{0} \n", "*");
                                }
                                else
                                    Dat += string.Format("{0} \n", "*");
                            };
                        };

                        lstIngresosHojaEnfermeriaFinal.Add(new cIngresosHojaEnfermeriaReporte
                        {
                            Generico3 = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                            Generico2 = !string.IsNullOrEmpty(Gen) ? Gen.TrimEnd('\n') : string.Empty,
                            Hora = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                            Generico = !string.IsNullOrEmpty(Dat) ? Dat.TrimEnd('\n') : string.Empty
                        });

                        #endregion
                        #region EGRESOS
                        string Egr = string.Empty;
                        if (_LiquidosEgreso != null && _LiquidosEgreso.Any())
                        {
                            foreach (var item3 in _LiquidosEgreso)
                            {
                                if (EgresosEncontrados != null && EgresosEncontrados.Any())
                                {
                                    if (EgresosEncontrados.Any(x => x.Horas == item3.ID_LIQ && x.Hora == item.DESCR))
                                        Egr += string.Format("{0} \n", EgresosEncontrados.FirstOrDefault(x => x.Horas == item3.ID_LIQ).Generico3);
                                    else
                                        Egr += string.Format("{0} \n", "*");
                                }
                                else
                                    Egr += string.Format("{0} \n", "*");
                            };
                        };

                        lstEgresosHojaEnfermeriaFinal.Add(new cIngresosHojaEnfermeriaReporte
                            {
                                Generico3 = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                                Generico2 = !string.IsNullOrEmpty(Da) ? Da.TrimEnd('\n') : string.Empty,
                                Hora = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                                Generico = !string.IsNullOrEmpty(Egr) ? Egr.TrimEnd('\n') : string.Empty
                            });
                        #endregion
                    };
                };

                if (Lesiones != null && Lesiones.Any())
                {
                    foreach (var item in Lesiones)
                    {
                        listaL.Add(new cReporteCertificadoNuevoIngreso
                            {
                                Cedula = item.REGION != null ? !string.IsNullOrEmpty(item.REGION.DESCR) ? item.REGION.DESCR.Trim() : string.Empty : string.Empty,
                                NombreMedico = item.DESCR
                            });
                    };

                    listaUlceras.Add(new cReporteCertificadoNuevoIngreso
                        {
                            Dorso = ParametroCuerpoDorso,
                            Frente = ParametroCuerpoFrente,
                            Check = ParametroImagenZonaCorporal,
                            F_FRONTAL = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FRONTAL) : true,
                            D_ANTEBRAZO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_DERECHO) : true,
                            D_ANTEBRAZO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_IZQUIERDO) : true,
                            D_BRAZO_POSTERIOR_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_DERECHO) : true,
                            D_BRAZO_POSTERIOR_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_IZQUIERDO) : true,
                            D_CALCANEO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_DERECHO) : true,
                            D_CALCANEO_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_IZQUIERDA) : true,
                            D_CODO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_DERECHO) : true,
                            D_CODO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_IZQUIERDO) : true,
                            D_COSTILLAS_COSTADO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_DERECHO) : true,
                            D_COSTILLAS_COSTADO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_IZQUIERDO) : true,
                            D_DORSAL_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_DERECHA) : true,
                            D_DORSAL_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_IZQUIERDA) : true,
                            D_ESCAPULAR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_DERECHA) : true,
                            D_ESCAPULAR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_IZQUIERDA) : true,
                            D_FALANGES_POSTERIORES_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_DERECHA) : true,
                            D_FALANGES_POSTERIORES_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_IZQUIERDA) : true,
                            D_GLUTEA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_DERECHA) : true,
                            D_GLUTEA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_IZQUIERDA) : true,
                            D_LUMBAR_RENAL_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_LUMBAR_RENAL_DERECHA) : true,
                            D_LUMBAR_RENAL_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                            D_METATARZIANA_PLANTA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                            D_METATARZIANA_PLANTA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_IZQUIERDA) : true,
                            D_MUÑECA_POSTERIOR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_DERECHA) : true,
                            D_MUÑECA_POSTERIOR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_IZQUIERDA) : true,
                            D_MUSLO_POSTERIOR_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_DERECHO) : true,
                            D_MUSLO_POSTERIOR_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_IZQUIERDO) : true,
                            D_OCCIPITAL_NUCA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OCCIPITAL_NUCA) : true,
                            D_OREJA_POSTERIOR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_DERECHA) : true,
                            D_OREJA_POSTERIOR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_IZQUIERDA) : true,
                            D_PANTORRILLA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_DERECHA) : true,
                            D_PANTORRILLA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_IZQUIERDA) : true,
                            D_POPLITEA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_DERECHA) : true,
                            D_POPLITEA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_IZQUIERDA) : true,
                            D_POSTERIOR_CABEZA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CABEZA) : true,
                            D_POSTERIOR_CUELLO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CUELLO) : true,
                            D_POSTERIOR_ENTREPIERNA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_DERECHA) : true,
                            D_POSTERIOR_ENTREPIERNA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_IZQUIERDA) : true,
                            D_POSTERIOR_HOMBRO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_DERECHO) : true,
                            D_POSTERIOR_HOMBRO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_IZQUIERDO) : true,
                            D_SACRA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_SACRA) : true,
                            D_TOBILLO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_DERECHO) : true,
                            D_TOBILLO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_IZQUIERDO) : true,
                            D_TORACICA_DORSAL = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TORACICA_DORSAL) : true,
                            D_VERTEBRAL_CERVICAL = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_CERVICAL) : true,
                            D_VERTEBRAL_LUMBAR = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_LUMBAR) : true,
                            D_VERTEBRAL_TORACICA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_TORACICA) : true,
                            F_ANTEBRAZO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_DERECHO) : true,
                            F_ANTEBRAZO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_IZQUIERDO) : true,
                            F_ANTERIOR_CUELLO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTERIOR_CUELLO) : true,
                            F_MUÑECA_ANTERIOR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_DERECHA) : true,
                            F_MUÑECA_ANTERIOR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_IZQUIERDA) : true,
                            F_AXILA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_DERECHA) : true,
                            F_AXILA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_IZQUIERDA) : true,
                            F_BAJO_VIENTRE_HIPOGASTRIO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BAJO_VIENTRE_HIPOGASTRIO) : true,
                            F_BRAZO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_DERECHO) : true,
                            F_BRAZO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_IZQUIERDO) : true,
                            F_CARA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                            F_CARA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CARA_IZQUIERDA) : true,
                            F_CLAVICULAR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                            F_CLAVICULAR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_IZQUIERDA) : true,
                            F_CODO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_DERECHO) : true,
                            F_CODO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_IZQUIERDO) : true,
                            F_ENTREPIERNA_ANTERIOR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_DERECHA) : true,
                            F_ENTREPIERNA_ANTERIOR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_IZQUIERDA) : true,
                            F_EPIGASTRIO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_EPIGASTRIO) : true,
                            F_ESCROTO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESCROTO) : true,
                            F_ESPINILLA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_DERECHA) : true,
                            F_ESPINILLA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_IZQUIERDA) : true,
                            F_FALANGES_MANO_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_DERECHA) : true,
                            F_FALANGES_MANO_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_IZQUIERDA) : true,
                            F_FALANGES_PIE_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_DERECHO) : true,
                            F_FALANGES_PIE_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_IZQUIERDO) : true,
                            F_HIPOCONDRIA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_DERECHA) : true,
                            F_HIPOCONDRIA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_IZQUIERDA) : true,
                            F_HOMBRO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_DERECHO) : true,
                            F_HOMBRO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_IZQUIERDO) : true,
                            F_INGUINAL_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_DERECHA) : true,
                            F_INGUINAL_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_IZQUIERDA) : true,
                            F_LATERAL_CABEZA_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_DERECHO) : true,
                            F_LATERAL_CABEZA_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_IZQUIERDO) : true,
                            F_MANDIBULA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MANDIBULA) : true,
                            F_METACARPIANOS_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_DERECHA) : true,
                            F_METACARPIANOS_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_IZQUIERDA) : true,
                            F_METATARZIANA_DORSAL_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_DERECHO) : true,
                            F_METATARZIANA_DORSAL_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_IZQUIERDO) : true,
                            F_MUSLO_ANTERIOR_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_DERECHO) : true,
                            F_MUSLO_ANTERIOR_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_IZQUIERDO) : true,
                            F_NARIZ = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_NARIZ) : true,
                            F_ORBITAL_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_DERECHO) : true,
                            F_ORBITAL_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_IZQUIERDO) : true,
                            F_OREJA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_DERECHA) : true,
                            F_OREJA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_IZQUIERDA) : true,
                            F_PENE_VAGINA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PENE_VAGINA) : true,
                            F_PEZON_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_DERECHO) : true,
                            F_PEZON_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_IZQUIERDO) : true,
                            F_RODILLA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_DERECHA) : true,
                            F_RODILLA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_IZQUIERDA) : true,
                            F_TOBILLO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_DERECHO) : true,
                            F_TOBILLO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_IZQUIERDO) : true,
                            F_TORAX_CENTRAL = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_CENTRAL) : true,
                            F_TORAX_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_DERECHO) : true,
                            F_TORAX_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_IZQUIERDO) : true,
                            F_UMBILICAL_MESOGASTIO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_UMBILICAL_MESOGASTIO) : true,
                            F_VACIO_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_DERECHA) : true,
                            F_VACIO_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_IZQUIERDA) : true,
                            F_VERTICE_CABEZA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VERTICE_CABEZA) : true
                        });
                }
                else
                {
                    #region Ulcera Vacia
                    listaUlceras.Add(new cReporteCertificadoNuevoIngreso
                        {
                            Dorso = ParametroCuerpoDorso,
                            Frente = ParametroCuerpoFrente,
                            Check = ParametroImagenZonaCorporal,
                            F_FRONTAL = true,
                            D_ANTEBRAZO_DERECHO = true,
                            D_ANTEBRAZO_IZQUIERDO = true,
                            D_BRAZO_POSTERIOR_DERECHO = true,
                            D_BRAZO_POSTERIOR_IZQUIERDO = true,
                            D_CALCANEO_DERECHO = true,
                            D_CALCANEO_IZQUIERDA = true,
                            D_CODO_DERECHO = true,
                            D_CODO_IZQUIERDO = true,
                            D_COSTILLAS_COSTADO_DERECHO = true,
                            D_COSTILLAS_COSTADO_IZQUIERDO = true,
                            D_DORSAL_DERECHA = true,
                            D_DORSAL_IZQUIERDA = true,
                            D_ESCAPULAR_DERECHA = true,
                            D_ESCAPULAR_IZQUIERDA = true,
                            D_FALANGES_POSTERIORES_DERECHA = true,
                            D_FALANGES_POSTERIORES_IZQUIERDA = true,
                            D_GLUTEA_DERECHA = true,
                            D_GLUTEA_IZQUIERDA = true,
                            D_LUMBAR_RENAL_DERECHA = true,
                            D_LUMBAR_RENAL_IZQUIERDA = true,
                            D_METATARZIANA_PLANTA_DERECHA = true,
                            D_METATARZIANA_PLANTA_IZQUIERDA = true,
                            D_MUÑECA_POSTERIOR_DERECHA = true,
                            D_MUÑECA_POSTERIOR_IZQUIERDA = true,
                            D_MUSLO_POSTERIOR_DERECHO = true,
                            D_MUSLO_POSTERIOR_IZQUIERDO = true,
                            D_OCCIPITAL_NUCA = true,
                            D_OREJA_POSTERIOR_DERECHA = true,
                            D_OREJA_POSTERIOR_IZQUIERDA = true,
                            D_PANTORRILLA_DERECHA = true,
                            D_PANTORRILLA_IZQUIERDA = true,
                            D_POPLITEA_DERECHA = true,
                            D_POPLITEA_IZQUIERDA = true,
                            D_POSTERIOR_CABEZA = true,
                            D_POSTERIOR_CUELLO = true,
                            D_POSTERIOR_ENTREPIERNA_DERECHA = true,
                            D_POSTERIOR_ENTREPIERNA_IZQUIERDA = true,
                            D_POSTERIOR_HOMBRO_DERECHO = true,
                            D_POSTERIOR_HOMBRO_IZQUIERDO = true,
                            D_SACRA = true,
                            D_TOBILLO_DERECHO = true,
                            D_TOBILLO_IZQUIERDO = true,
                            D_TORACICA_DORSAL = true,
                            D_VERTEBRAL_CERVICAL = true,
                            D_VERTEBRAL_LUMBAR = true,
                            D_VERTEBRAL_TORACICA = true,
                            F_ANTEBRAZO_DERECHO = true,
                            F_ANTEBRAZO_IZQUIERDO = true,
                            F_ANTERIOR_CUELLO = true,
                            F_MUÑECA_ANTERIOR_DERECHA = true,
                            F_MUÑECA_ANTERIOR_IZQUIERDA = true,
                            F_AXILA_DERECHA = true,
                            F_AXILA_IZQUIERDA = true,
                            F_BAJO_VIENTRE_HIPOGASTRIO = true,
                            F_BRAZO_DERECHO = true,
                            F_BRAZO_IZQUIERDO = true,
                            F_CARA_DERECHA = true,
                            F_CARA_IZQUIERDA = true,
                            F_CLAVICULAR_DERECHA = true,
                            F_CLAVICULAR_IZQUIERDA = true,
                            F_CODO_DERECHO = true,
                            F_CODO_IZQUIERDO = true,
                            F_ENTREPIERNA_ANTERIOR_DERECHA = true,
                            F_ENTREPIERNA_ANTERIOR_IZQUIERDA = true,
                            F_EPIGASTRIO = true,
                            F_ESCROTO = true,
                            F_ESPINILLA_DERECHA = true,
                            F_ESPINILLA_IZQUIERDA = true,
                            F_FALANGES_MANO_DERECHA = true,
                            F_FALANGES_MANO_IZQUIERDA = true,
                            F_FALANGES_PIE_DERECHO = true,
                            F_FALANGES_PIE_IZQUIERDO = true,
                            F_HIPOCONDRIA_DERECHA = true,
                            F_HIPOCONDRIA_IZQUIERDA = true,
                            F_HOMBRO_DERECHO = true,
                            F_HOMBRO_IZQUIERDO = true,
                            F_INGUINAL_DERECHA = true,
                            F_INGUINAL_IZQUIERDA = true,
                            F_LATERAL_CABEZA_DERECHO = true,
                            F_LATERAL_CABEZA_IZQUIERDO = true,
                            F_MANDIBULA = true,
                            F_METACARPIANOS_DERECHA = true,
                            F_METACARPIANOS_IZQUIERDA = true,
                            F_METATARZIANA_DORSAL_DERECHO = true,
                            F_METATARZIANA_DORSAL_IZQUIERDO = true,
                            F_MUSLO_ANTERIOR_DERECHO = true,
                            F_MUSLO_ANTERIOR_IZQUIERDO = true,
                            F_NARIZ = true,
                            F_ORBITAL_DERECHO = true,
                            F_ORBITAL_IZQUIERDO = true,
                            F_OREJA_DERECHA = true,
                            F_OREJA_IZQUIERDA = true,
                            F_PENE_VAGINA = true,
                            F_PEZON_DERECHO = true,
                            F_PEZON_IZQUIERDO = true,
                            F_RODILLA_DERECHA = true,
                            F_RODILLA_IZQUIERDA = true,
                            F_TOBILLO_DERECHO = true,
                            F_TOBILLO_IZQUIERDO = true,
                            F_TORAX_CENTRAL = true,
                            F_TORAX_DERECHO = true,
                            F_TORAX_IZQUIERDO = true,
                            F_UMBILICAL_MESOGASTIO = true,
                            F_VACIO_DERECHA = true,
                            F_VACIO_IZQUIERDA = true,
                            F_VERTICE_CABEZA = true
                        });

                    #endregion
                }

                var _DetalleCentroActual = new SSP.Controlador.Catalogo.Justicia.cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                View.Report.LocalReport.ReportPath = "Reportes/rHojaEnfemeria.rdlc";
                View.Report.LocalReport.DataSources.Clear();

                #region Encabezado
                var Encabezado = new cEncabezado();
                Encabezado.TituloDos = _DetalleCentroActual != null ? !string.IsNullOrEmpty(_DetalleCentroActual.DESCR) ? _DetalleCentroActual.DESCR.Trim() : string.Empty : string.Empty;
                Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                Encabezado.ImagenFondo = Parametro.LOGO_ESTADO;
                Encabezado.PieDos = string.Format("{0} {1} {2} {3} / {4}",
                    SelectIngreso != null ? SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                    SelectIngreso != null ? SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                    SelectIngreso != null ? SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                    SelectIngreso.ID_ANIO,
                    SelectIngreso.ID_IMPUTADO);
                #endregion

                if (lstSolucionesHojaEnfermeria != null && lstSolucionesHojaEnfermeria.Any())
                    if (!lstSolucionesHojaEnfermeria.TrueForAll(x => string.IsNullOrEmpty(x.Nombre)))
                        foreach (var item in lstSolucionesHojaEnfermeria)
                            if (item.Consecutivo == new int?())
                                item.Consecutivo = 1;//se usa un consecutivo como comodin, evita generar espacios vacios por la agrupacion de los elementos de la matriz

                var ds1 = new System.Collections.Generic.List<cEncabezado>();
                ds1.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                var ds2 = new System.Collections.Generic.List<cHojaEnfermeriaEncabezadoResporte>();
                ds2.Add(_DatosCabezera);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = lstSignosVitales != null ? lstSignosVitales.Any() ? lstSignosVitales.OrderBy(x => x.Generico).ToList() : new System.Collections.Generic.List<cHojaEnfermeriaSignosVitalesReporte>() : new System.Collections.Generic.List<cHojaEnfermeriaSignosVitalesReporte>();
                View.Report.LocalReport.DataSources.Add(rds3);

                Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = lstSolucionesHojaEnfermeria;
                View.Report.LocalReport.DataSources.Add(rds4);

                Microsoft.Reporting.WinForms.ReportDataSource rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = lstIngresosHojaEnfermeriaFinal;
                View.Report.LocalReport.DataSources.Add(rds5);

                Microsoft.Reporting.WinForms.ReportDataSource rds6 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds6.Name = "DataSet6";
                rds6.Value = lstEgresosHojaEnfermeriaFinal;
                View.Report.LocalReport.DataSources.Add(rds6);

                Microsoft.Reporting.WinForms.ReportDataSource rds7 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds7.Name = "DataSet7";
                rds7.Value = listaMedicamentosHojaEnfermeria;
                View.Report.LocalReport.DataSources.Add(rds7);

                Microsoft.Reporting.WinForms.ReportDataSource rds8 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds8.Name = "DataSet8";
                rds8.Value = lstNotasEnfermeria;
                View.Report.LocalReport.DataSources.Add(rds8);

                Microsoft.Reporting.WinForms.ReportDataSource rds9 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds9.Name = "DataSet9";
                rds9.Value = listaUlceras;
                View.Report.LocalReport.DataSources.Add(rds9);

                Microsoft.Reporting.WinForms.ReportDataSource rds10 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds10.Name = "DataSet10";
                rds10.Value = listaL;
                View.Report.LocalReport.DataSources.Add(rds10);

                Microsoft.Reporting.WinForms.ReportDataSource rds11 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds11.Name = "DataSet11";
                rds11.Value = lstCateteresReporte;
                View.Report.LocalReport.DataSources.Add(rds11);

                Microsoft.Reporting.WinForms.ReportDataSource rds12 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds12.Name = "DataSet12";
                rds12.Value = lstSondasReporte;
                View.Report.LocalReport.DataSources.Add(rds12);

                Microsoft.Reporting.WinForms.ReportDataSource rds13 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds13.Name = "DataSet13";
                rds13.Value = lstRayosXReporte;
                View.Report.LocalReport.DataSources.Add(rds13);

                Microsoft.Reporting.WinForms.ReportDataSource rds14 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds14.Name = "DataSet14";
                rds14.Value = listLaboratoriosReporte;
                View.Report.LocalReport.DataSources.Add(rds14);

                View.Report.ReportExport += (s, e) =>
                {
                    try
                    {
                        if (e != null && !e.Extension.LocalizedName.Equals("PDF"))//Solo permite pdf
                        {
                            e.Cancel = true;//Detiene el evento
                            s = e = null;//Detiene el mapeo de los parametros para que no continuen en el arbol
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No esta permitido exportar la información en este formato.");
                        }
                    }

                    catch (System.Exception exc)
                    {
                        throw exc;
                    }
                };

                View.Report.RefreshReport();
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private async void clickSwitch(System.Object obj)
        {
            switch (obj.ToString())
            {
                case "nueva_busqueda":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = new int?();
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_visible":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    AnioBuscar = FolioBuscar = null;
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    SelectExpediente = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new HojaEnfermeriaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.HojaEnfermeriaViewModel();
                    break;

                case "buscar_menu":
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    SelectExpediente = new SSP.Servidor.IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un ingreso vigente");
                        return;
                    }

                    foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                            return;
                        }

                    if (SelectIngreso.ID_UB_CENTRO.HasValue && SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        return;
                    }

                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-Parametro.TOLERANCIA_TRASLADO_EDIFICIO) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                        return;
                    }

                    SelectedInterno = SelectIngreso.IMPUTADO;
                    if (SelectIngreso != null)
                        SelectIngreso = SelectIngreso;

                    var _validacionHospitalizado = new SSP.Controlador.Catalogo.Justicia.Medico.cHospitalizacion().ObtenerHospitalizacionesPorIngreso(SelectIngreso);
                    if (!_validacionHospitalizado.Any())
                    {
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no está hospitalizado.");
                        return;
                    }

                    LimpiaCopmpletaHojaEnfermeria();
                    SeleccionaIngreso();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "guardar_menu":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    if (SelectedTurnoLiquidos == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un turno.");
                        break;
                    };

                    if (SelectedTurnoLiquidos == -1)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un turno.");
                        break;
                    };

                    if (ListRecetas != null && ListRecetas.Any())
                        if (ListRecetas.Any(x => !x.PRESENTACION.HasValue))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una presentación para las soluciones.");
                            break;
                        };

                    int result = 0;
                    result = await (new Dialogos()).ConfirmacionDosBotonesCustom("Hoja de Enfermería", "Una vez cerrada, serán computados los balances y esta no podrá ser editada ¿Desea cerrar la hoja de enfermería?", "SI", 1, "NO", 2);
                    Opcion = result;
                    switch (result)
                    {
                        case 1:
                            Opcion = 1;
                            break;

                        case 2:
                            Opcion = 2;
                            break;
                    }

                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Ingresando hoja de enfermería", () => GuardaHojaenfermeria()))
                    {
                        new Dialogos().ConfirmacionDialogo("Exito", "Se ha registrado la hoja de enfermería exitosamente.");
                        ProcesaDatosHojaEnfermeria();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "Surgió un error al ingresar la hoja de enfermería.");

                    break;

                case "reporte_menu":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    ImprimeHojaEnfermeria();
                    break;

                #region SIGNOS VITALES
                case "guardar_menu_signos_vitales":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    if (FechaHoraCaptura == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe capturar la hora.");
                        break;
                    };

                    if (SelectedTurnoLiquidos == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un turno.");
                        break;
                    };

                    if (SelectedTurnoLiquidos == -1)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un turno.");
                        break;
                    };

                    if (string.IsNullOrEmpty(FrecuenciaCardiacaHE) && string.IsNullOrEmpty(FrecuenciaRespiratoriaHE) && string.IsNullOrEmpty(Arterial1) && string.IsNullOrEmpty(Arterial2) && string.IsNullOrEmpty(TensionArtMediaHE) && string.IsNullOrEmpty(TempHE) && string.IsNullOrEmpty(Sa02HE) && string.IsNullOrEmpty(DextrHE) && string.IsNullOrEmpty(NebHE) && string.IsNullOrEmpty(PVCHE) && string.IsNullOrEmpty(CambioPosHE) && string.IsNullOrEmpty(RiesgoEscHE) && string.IsNullOrEmpty(RiesgoCaiHE))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe capturar al menos un registro en la sección de signos vitales y otras lecturas generales.");
                        break;
                    };

                    if (!string.IsNullOrEmpty(Arterial1) || !string.IsNullOrEmpty(Arterial2))
                        if (string.IsNullOrEmpty(Arterial1) || string.IsNullOrEmpty(Arterial2))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario capturar la presión sistólica y presión diastólica en la tensión arterial.");
                            break;
                        };

                    AgregarSignosVitalesHojaEnfermeria();
                    break;

                case "limpiar_menu_signos_vitales":
                    LimpiarSignosVitales();
                    break;

                case "eliminar_signos_vitales":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    }

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    if (SelectedSignosVitalesHojaEnfermeria == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un registro de signos vitales.");
                        break;
                    }

                    if (LstSignosVitalesHojaEnfermeria.Remove(SelectedSignosVitalesHojaEnfermeria))
                    {
                        new Dialogos().ConfirmacionDialogo("Exito", "Se ha eliminado de la lista el registro seleccionado.");
                        break;
                    }
                    break;
                #endregion

                #region SOLUCIONES
                case "eliminar_medicamento_hoja_enfermeria":
                    if (SelectReceta != null ? SelectReceta.PRODUCTO == null : true) return;
                    if (ListRecetas.Any(a => a.PRODUCTO.ID_PRODUCTO == SelectReceta.PRODUCTO.ID_PRODUCTO))
                        ListRecetas.Remove(ListRecetas.First(a => a.PRODUCTO.ID_PRODUCTO == SelectReceta.PRODUCTO.ID_PRODUCTO));
                    break;
                #endregion
                #region INGRESOS Y EGRESO
                case "guardar_menu_ingresos_egresos":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    }

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    if (PosicionActual != decimal.Zero)
                        switch (PosicionActual)
                        {
                            case (short)ePosicionActualHojaEnfermeria.INGRESOS:
                                AgregrLiquido((short)ePosicionActualHojaEnfermeria.INGRESOS);
                                break;

                            case (short)ePosicionActualHojaEnfermeria.EGRESOS:
                                AgregrLiquido((short)ePosicionActualHojaEnfermeria.EGRESOS);
                                break;

                            default:
                                //no action
                                break;
                        };
                    break;

                case "eliminar_ingreso_he":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    }

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    if (SelectedLiquidoIngresoHE == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un registro.");
                        break;
                    }

                    if (LstLiquidosIngresoHojaEnfermeria.Remove(SelectedLiquidoIngresoHE))
                        new Dialogos().ConfirmacionDialogo("Exito", "Se ha eliminado de la lista el registro seleccionado.");

                    break;

                case "limpiar_menu_ingresos_egresos":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    if (PosicionActual != decimal.Zero)
                        switch (PosicionActual)
                        {
                            case (short)ePosicionActualHojaEnfermeria.INGRESOS:
                                LimpiaIgresosEgresos((short)ePosicionActualHojaEnfermeria.INGRESOS);
                                break;

                            case (short)ePosicionActualHojaEnfermeria.EGRESOS:
                                LimpiaIgresosEgresos((short)ePosicionActualHojaEnfermeria.EGRESOS);
                                break;

                            default:
                                //no action
                                break;
                        };
                    break;
                #endregion

                #region Nota de Enfermeria
                case "agregar_anotacion_hoja_enfermeria":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    if (string.IsNullOrEmpty(AnotacionNuevaHojaEnfermeria))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe ingresar texto en el apartado de Anotaciones para complementar la nota de enfermería.");
                        break;
                    };

                    if (string.IsNullOrEmpty(NotaEnfermeriaExistente))
                        NotaEnfermeriaExistente = string.Empty;

                    if ((NotaEnfermeriaExistente.Length + AnotacionNuevaHojaEnfermeria.Length) < 1000)
                        NotaEnfermeriaExistente += string.Format(" {0} \n", AnotacionNuevaHojaEnfermeria);
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Al agregar esta anotación, supera el máximo permitido");
                        break;
                    }

                    AnotacionNuevaHojaEnfermeria = string.Empty;
                    break;

                case "cargar_notas_turno_ant":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    ConsulltaNotasEnfermeriaTurnoAnterior();
                    break;

                case "limpiar_nota_enf":
                    TextoAntiguaNotaEnfermeria = string.Empty;
                    break;
                #endregion

                #region LESION
                case "eliminar_lesion":
                    try
                    {
                        if (SelectLesionEliminar == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Debe seleccionar una lesión.");
                            return;
                        };

                        if (!ListLesiones.Any())
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Ocurrió un error con la región seleccionada.");
                            return;
                        };

                        if (ListLesiones.Remove(SelectLesionEliminar))
                        {
                            new Dialogos().ConfirmacionDialogo("Exito", "La ulcera ha sido eliminada con exito.");
                            return;
                        };
                    }
                    catch (System.Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar la ulcera.", ex);
                    }
                    break;
                case "limpiar_lesion":
                    try
                    {
                        BotonLesionEnabled = false;
                        TextDescripcionLesion = string.Empty;
                        var radioButons = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<System.Windows.Controls.RadioButton>(((HojaEnfermeriaView)((System.Windows.Controls.ContentControl)System.Windows.Application.Current.Windows[0].FindName("contentControl")).Content)).ToList();
                        foreach (var item in radioButons)
                            item.IsChecked = false;

                        SelectLesion = null;
                        SelectRegion = new short?();
                    }
                    catch (System.Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar los campos de la ulcera.", ex);
                    }
                    break;
                case "agregar_lesion":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso.");
                        break;
                    };

                    if (string.IsNullOrEmpty(TextDescripcionLesion))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe ingresar una descripción de la ulcera.");
                        break;
                    }

                    if (SelectRegion == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar la región donde se encuentra la ulcera.");
                        break;
                    };

                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        try
                        {
                            ListLesiones = ListLesiones ?? new System.Collections.ObjectModel.ObservableCollection<LesionesCustom>();
                            BotonLesionEnabled = false;
                            System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                ListLesiones.Add(new LesionesCustom { DESCR = TextDescripcionLesion, REGION = new SSP.Controlador.Catalogo.Justicia.cAnatomiaTopografica().Obtener((int)SelectRegion) });
                                var radios = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<System.Windows.Controls.RadioButton>(((HojaEnfermeriaView)((System.Windows.Controls.ContentControl)System.Windows.Application.Current.Windows[0].FindName("contentControl")).Content)).ToList();
                                foreach (var item in radios)
                                    item.IsChecked = false;
                                SelectLesion = null;
                                SelectRegion = new short?();
                            }));
                            TextDescripcionLesion = string.Empty;
                        }
                        catch (System.Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar los datos de la lesion.", ex);
                        }
                    });

                    break;
                #endregion

                case "buscar_datos_hoja_enfermeria_vieja":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    if (FechaHojaenfermeria == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una fecha");
                        break;
                    };

                    if (SelectedTurnoLiquidos == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un turno.");
                        break;
                    };

                    if (SelectedTurnoLiquidos == -1)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un turno.");
                        break;
                    };

                    ProcesaDatosHojaEnfermeria();
                    VisiblePrincipal = System.Windows.Visibility.Visible;
                    OnPropertyChanged("VisiblePrincipal");
                    break;

                case "guardar_menu_medicamento":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };


                    if (SelectedMedicCustom == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un medicamento.");
                        break;
                    };

                    if (FechaSuministroMedicamentoEditar == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una fecha de suministro del medicamento.");
                        break;
                    };

                    if (LstCustmoMedi == null)
                        LstCustmoMedi = new System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas>();

                    cCustomMedicamentosNotas temporal = SelectedMedicCustom;
                    if (LstCustmoMedi.Remove(SelectedMedicCustom))
                        LstCustmoMedi.Add(new cCustomMedicamentosNotas
                            {
                                Cantidad = temporal.Cantidad,
                                Cena = temporal.Cena,
                                Comida = temporal.Comida,
                                Desayuno = temporal.Desayuno,
                                Duracion = temporal.Duracion,
                                Id = temporal.Id,
                                FechaReceto = temporal.FechaReceto,
                                IdFolio = temporal.IdFolio,
                                IdAtencionMedica = temporal.IdAtencionMedica,
                                IdPResentacionProducto = temporal.IdPResentacionProducto,
                                IdProducto = temporal.IdProducto,
                                NombreMedicamento = temporal.NombreMedicamento,
                                Obsertvaciones = temporal.Obsertvaciones,
                                Presentacion = temporal.Presentacion,
                                UltimaFecha = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy HH:mm"),
                                UnidadMedida = temporal.UnidadMedida,
                                FechaSuministro = FechaSuministroMedicamentoEditar
                            });

                    FechaSuministroMedicamentoEditar = null;
                    temporal = SelectedMedicCustom = null;
                    LimpiaCamposMedicamento();
                    break;

                case "limpiar_menu_medicamento":
                    SelectedMedicCustom = null;
                    LimpiaCamposMedicamento();
                    break;

                case "registrar_medicamento":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    }

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    if (SelectedMedicCustom == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un registro.");
                        break;
                    }

                    CargaMedicamentoEdicion(SelectedMedicCustom);
                    break;

                #region CATETER
                case "guardar_menu_cateter":
                    if (VisualizandoCateteres)
                        return;

                    #region Validaciones
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    if (base.HasErrors)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                        return;
                    };

                    if (SelectedTipoCataterHE == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un tipo de catéter.");
                        break;
                    };

                    if (SelectedTipoCataterHE == -1)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un tipo de catéter.");
                        break;
                    };

                    if (FechaInstalacionCatHE == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe especificar la fecha de instalación del catéter que está tratando de ingresar.");
                        break;
                    };

                    if (string.IsNullOrEmpty(SelectedRetiroCateterHE))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe especificar si retiro o no el catéter que está tratando de ingresar.");
                        break;
                    };

                    #endregion
                    if (LstCateteres == null)
                        LstCateteres = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_CATETER>();

                    if (SelectedRetiroCateterHE == "S")
                    {
                        if (!IsFechaIniBusquedaValida)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La fecha de instalación del catéter debe que ser menor a la fecha de retiro del catéter!");
                            return;
                        };

                        int xy = await (new Dialogos()).ConfirmacionDosBotonesCustom("Hoja de Enfermería", "Una vez retirado, este catéter no podrá ser editado ¿Desea continuar?", "SI", 1, "NO", 2);
                        switch (xy)
                        {
                            case 1:
                                if (LstCateteres.Any(x => x.ID_CATETER == SelectedTipoCataterHE))
                                {
                                    var CataterExistente = LstCateteres.FirstOrDefault(x => x.ID_CATETER == SelectedTipoCataterHE && x.ID_HOJAENFCAT == (SelectedHojaCatater != null ? SelectedHojaCatater.ID_HOJAENFCAT : ConsecutivoInterno));
                                    if (CataterExistente != null)
                                    {
                                        if (LstCateteres.Remove(SelectedHojaCatater))
                                        {
                                            LstCateteres.Add(new SSP.Servidor.HOJA_ENFERMERIA_CATETER
                                            {
                                                DATOS_INFECCION = DatosIngeccionCateterHE,
                                                ID_CATETER = SelectedTipoCataterHE,
                                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                                ID_HOSPITA = SelectedHospitalizacion,
                                                ID_USUARIO = GlobalVar.gUsr,
                                                INSTALACION_FEC = FechaInstalacionCatHE,
                                                RETIRO = SelectedRetiroCateterHE,
                                                REGISTRO_FEC = Fechas.GetFechaDateServer,
                                                MOTIVO_RETIRO = MotivoRetiroCateterHE,
                                                VENCIMIENTO_FEC = FechavencimientoCatHE,
                                                FECHA_RETIRO = FechaRetiroCateterHE,
                                                ID_REGISTRO_INICIAL = -1,
                                                CATETER_TIPO = SelectedTipoC,
                                                ID_HOJAENFCAT = ConsecutivoInterno
                                            });

                                            ConsecutivoInterno++;
                                            LimpiaDatosCateter();
                                            EstaEditando = false;//REINICIA A SU VALOR ORIGINAL
                                            return;
                                        };
                                    }
                                    else
                                    {
                                        LstCateteres.Add(new SSP.Servidor.HOJA_ENFERMERIA_CATETER
                                        {
                                            DATOS_INFECCION = DatosIngeccionCateterHE,
                                            ID_CATETER = SelectedTipoCataterHE,
                                            ID_CENTRO_UBI = GlobalVar.gCentro,
                                            ID_HOSPITA = SelectedHospitalizacion,
                                            ID_USUARIO = GlobalVar.gUsr,
                                            INSTALACION_FEC = FechaInstalacionCatHE,
                                            RETIRO = SelectedRetiroCateterHE,
                                            REGISTRO_FEC = Fechas.GetFechaDateServer,
                                            MOTIVO_RETIRO = MotivoRetiroCateterHE,
                                            VENCIMIENTO_FEC = FechavencimientoCatHE,
                                            ID_REGISTRO_INICIAL = -1,
                                            FECHA_RETIRO = FechaRetiroCateterHE,
                                            CATETER_TIPO = SelectedTipoC,
                                            ID_HOJAENFCAT = ConsecutivoInterno
                                        });

                                        ConsecutivoInterno++;
                                        LimpiaDatosCateter();
                                        EstaEditando = false;//REINICIA A SU VALOR ORIGINAL
                                    }
                                }//INCIDENCIA_ATENCION_CITA
                                else
                                {
                                    LstCateteres.Add(new SSP.Servidor.HOJA_ENFERMERIA_CATETER
                                    {
                                        DATOS_INFECCION = DatosIngeccionCateterHE,
                                        ID_CATETER = SelectedTipoCataterHE,
                                        ID_CENTRO_UBI = GlobalVar.gCentro,
                                        ID_HOSPITA = SelectedHospitalizacion,
                                        ID_USUARIO = GlobalVar.gUsr,
                                        INSTALACION_FEC = FechaInstalacionCatHE,
                                        RETIRO = SelectedRetiroCateterHE,
                                        REGISTRO_FEC = Fechas.GetFechaDateServer,
                                        MOTIVO_RETIRO = MotivoRetiroCateterHE,
                                        VENCIMIENTO_FEC = FechavencimientoCatHE,
                                        FECHA_RETIRO = FechaRetiroCateterHE,
                                        CATETER_TIPO = SelectedTipoC,
                                        ID_REGISTRO_INICIAL = -1,
                                        ID_HOJAENFCAT = ConsecutivoInterno
                                    });

                                    ConsecutivoInterno++;
                                }

                                LimpiaDatosCateter();
                                SelectedHojaCatater = null;
                                EstaEditando = false;//REINICIA A SU VALOR ORIGINAL
                                return;

                            case 2:
                                return;
                        }
                    };

                    if (EstaEditando)
                    {
                        if (LstCateteres.Any(x => x.ID_CATETER == SelectedTipoCataterHE && x.ID_HOJAENFCAT == SelectedHojaCatater.ID_HOJAENFCAT))
                        {
                            var CateterExistente = LstCateteres.FirstOrDefault(x => x.ID_CATETER == SelectedTipoCataterHE && x.ID_HOJAENFCAT == SelectedHojaCatater.ID_HOJAENFCAT);
                            if (CateterExistente != null)
                            {
                                var x = SelectedHojaCatater;
                                //CateterTemp = CataterExistente;
                                CateterTemp = new SSP.Servidor.HOJA_ENFERMERIA_CATETER()
                                {
                                    DATOS_INFECCION = DatosIngeccionCateterHE,
                                    ID_CATETER = SelectedTipoCataterHE,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    ID_HOSPITA = SelectedHospitalizacion,
                                    ID_USUARIO = GlobalVar.gUsr,
                                    INSTALACION_FEC = FechaInstalacionCatHE,
                                    RETIRO = SelectedRetiroCateterHE,
                                    REGISTRO_FEC = Fechas.GetFechaDateServer,
                                    MOTIVO_RETIRO = MotivoRetiroCateterHE,
                                    VENCIMIENTO_FEC = FechavencimientoCatHE,
                                    FECHA_RETIRO = FechaRetiroCateterHE,
                                    CATETER_TIPO = SelectedTipoC
                                };

                                AgregaIncidenciaCateter();
                                EstaEditando = false;//REINICIA A SU VALOR ORIGINAL
                                return;
                            }
                        }
                    }
                    else
                    {
                        LstCateteres.Add(new SSP.Servidor.HOJA_ENFERMERIA_CATETER
                            {
                                DATOS_INFECCION = DatosIngeccionCateterHE,
                                ID_CATETER = SelectedTipoCataterHE,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_HOSPITA = SelectedHospitalizacion,
                                ID_USUARIO = GlobalVar.gUsr,
                                ID_REGISTRO_INICIAL = -1,
                                INSTALACION_FEC = FechaInstalacionCatHE,
                                RETIRO = SelectedRetiroCateterHE,
                                REGISTRO_FEC = Fechas.GetFechaDateServer,
                                MOTIVO_RETIRO = MotivoRetiroCateterHE,
                                VENCIMIENTO_FEC = FechavencimientoCatHE,
                                FECHA_RETIRO = FechaRetiroCateterHE,
                                CATETER_TIPO = SelectedTipoC,
                                ID_HOJAENFCAT = ConsecutivoInterno
                            });

                        ConsecutivoInterno++;
                        EstaEditando = false;
                    }

                    SelectedHojaCatater = null;
                    LimpiaDatosCateter();
                    break;

                case "limpiar_menu_cateter":
                    LimpiaDatosCateter();
                    break;

                case "editar_cateter":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    if (SelectedHojaCatater == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un registro a editar.");
                        break;
                    };

                    if (SelectedHojaCatater.RETIRO == "S")
                    {
                        NombreLeyendaMenuCateter = "Agregar";//REGRESA LA LEYENDA A SU MODO INICIAL PARA NO CONFUNDIR AL USUARIO
                        OnPropertyChanged("NombreLeyendaMenuCateter");
                        new Dialogos().ConfirmacionDialogo("Validación", "Un catéter retirado no puede ser editado.");
                        break;
                    };

                    EstaEditando = true;
                    VisualizandoCateteres = false;//NO ESTOY VIENDO,LO VOY A EDITAR
                    SelectedTipoCataterHE = SelectedHojaCatater.ID_CATETER;
                    FechaInstalacionCatHE = SelectedHojaCatater.INSTALACION_FEC;
                    FechavencimientoCatHE = SelectedHojaCatater.VENCIMIENTO_FEC;
                    FechaRetiroCateterHE = SelectedHojaCatater.FECHA_RETIRO;
                    SelectedRetiroCateterHE = SelectedHojaCatater.RETIRO ?? string.Empty;
                    DatosIngeccionCateterHE = SelectedHojaCatater.DATOS_INFECCION;
                    MotivoRetiroCateterHE = SelectedHojaCatater.MOTIVO_RETIRO;
                    EnabledPermiteQuitarCataterHe = false;
                    EnabledQuitarCateter = true;
                    break;

                case "retirar_cateter":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    if (SelectedHojaCatater == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un registro a editar.");
                        break;
                    };

                    if (SelectedHojaCatater.RETIRO == "S")
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El catéter seleccionado ya fue retirado.");
                        break;
                    };

                    VisualizandoCateteres = false;//NO ESTOY VIENDO, VOY A EDITAR
                    EnabledPermiteQuitarCataterHe = true;
                    OnPropertyChanged("EnabledPermiteQuitarCataterHe");
                    EstaEditando = true;
                    SelectedTipoCataterHE = SelectedHojaCatater.ID_CATETER;
                    FechaInstalacionCatHE = SelectedHojaCatater.INSTALACION_FEC;
                    FechavencimientoCatHE = SelectedHojaCatater.VENCIMIENTO_FEC;
                    FechaRetiroCateterHE = SelectedHojaCatater.FECHA_RETIRO;
                    SelectedRetiroCateterHE = SelectedHojaCatater.RETIRO ?? string.Empty;
                    DatosIngeccionCateterHE = SelectedHojaCatater.DATOS_INFECCION;
                    MotivoRetiroCateterHE = SelectedHojaCatater.MOTIVO_RETIRO;
                    EnabledQuitarCateter = false;
                    break;
                #endregion
                #region SONDA
                case "guardar_menu_sondas":
                    if (VisualizandoSondas)
                        return;

                    #region Validaciones
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    if (FechaInstalacionSondaHE == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ingrese fecha de instalación de la sonda.");
                        break;
                    };

                    if (string.IsNullOrEmpty(SelectedRetiroSondaNHE))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe especificar si retiro o no la sonda.");
                        break;
                    };

                    #endregion
                    if (LstSondas == null)
                        LstSondas = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS>();

                    if (SelectedRetiroSondaNHE == "S")
                    {
                        if (FechaRetiroSondaHE == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Ingrese fecha de retiro de la sonda.");
                            return;
                        }

                        if (!IsFechaSondaValida)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La fecha de instalación de la sonda debe que ser menor a la fecha de retiro de la sonda!");
                            return;
                        }

                        int r = await (new Dialogos()).ConfirmacionDosBotonesCustom("Hoja de Enfermería", "Una vez retirada, esta sonda no podrá ser editada ¿Desea continuar?", "SI", 1, "NO", 2);
                        switch (r)
                        {
                            case 1:
                                var SondaExistente = LstSondas.FirstOrDefault(x => x.ID_HOJAENF == ConsecutivoSondas);
                                if (LstSondas.Remove(SelectedSonda))
                                {
                                    LstSondas.Add(new SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS
                                        {
                                            ID_CENTRO_UBI = GlobalVar.gCentro,
                                            ID_HOSPITA = SelectedHospitalizacion,
                                            ID_USUARIO = GlobalVar.gUsr,
                                            RETIRO = SelectedRetiroSondaNHE,
                                            INSTALACION_FEC = FechaInstalacionSondaHE,
                                            OBSERVACION = ObservacionesSondaN,
                                            REGISTRO_FEC = Fechas.GetFechaDateServer,
                                            FECHA_RETIRO = FechaRetiroSondaHE,
                                        });

                                    ConsecutivoSondas++;
                                    LimpiaDatosSondas();
                                }
                                else
                                {
                                    LstSondas.Add(new SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS
                                    {
                                        ID_CENTRO_UBI = GlobalVar.gCentro,
                                        ID_HOSPITA = SelectedHospitalizacion,
                                        ID_USUARIO = GlobalVar.gUsr,
                                        RETIRO = SelectedRetiroSondaNHE,
                                        INSTALACION_FEC = FechaInstalacionSondaHE,
                                        OBSERVACION = ObservacionesSondaN,
                                        REGISTRO_FEC = Fechas.GetFechaDateServer,
                                        FECHA_RETIRO = FechaRetiroSondaHE,
                                        ID_HOJAENF = ConsecutivoSondas
                                    });

                                    ConsecutivoSondas++;
                                    LimpiaDatosSondas();
                                }

                                SelectedSonda = null;
                                return;

                            case 2:
                                return;
                        };
                    };

                    if (EstaEditandoSondas)
                    {
                        if (LstSondas.Any(x => x.ID_HOJAENF == SelectedSonda.ID_HOJAENF))
                        {
                            var SondaExistente = LstSondas.FirstOrDefault(x => x.ID_HOJAENF == SelectedSonda.ID_HOJAENF);
                            if (SondaExistente != null)
                            {
                                var x = SelectedSonda;
                                //CateterTemp = CataterExistente;
                                SondaTemp = new SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS()
                                {
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    ID_HOSPITA = SelectedHospitalizacion,
                                    ID_USUARIO = GlobalVar.gUsr,
                                    RETIRO = SelectedRetiroSondaNHE,
                                    INSTALACION_FEC = FechaInstalacionSondaHE,
                                    OBSERVACION = ObservacionesSondaN,
                                    REGISTRO_FEC = Fechas.GetFechaDateServer,
                                    FECHA_RETIRO = FechaRetiroSondaHE,
                                    ID_HOJAENF = SelectedSonda.ID_HOJAENF
                                };

                                AgregaIncidenciaSonda();
                                EstaEditandoSondas = false;//REINICIA A SU VALOR ORIGINAL
                                return;
                            }
                        }
                    }
                    else//NO EXISTIA, HAY QUE AGREGARLA DE NUEVO
                    {
                        LstSondas.Add(new SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS
                        {
                            ID_CENTRO_UBI = GlobalVar.gCentro,
                            ID_HOSPITA = SelectedHospitalizacion,
                            ID_USUARIO = GlobalVar.gUsr,
                            RETIRO = SelectedRetiroSondaNHE,
                            INSTALACION_FEC = FechaInstalacionSondaHE,
                            OBSERVACION = ObservacionesSondaN,
                            REGISTRO_FEC = Fechas.GetFechaDateServer,
                            FECHA_RETIRO = FechaRetiroSondaHE,
                            ID_HOJAENF = ConsecutivoSondas
                        });

                        ConsecutivoSondas++;
                        EstaEditandoSondas = false;
                    }

                    LimpiaDatosSondas();
                    SelectedSonda = null;
                    break;

                case "limpiar_menu_sondas":
                    LimpiaDatosSondas();
                    break;

                case "editar_sonda":
                    if (SelectedSonda == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione una sonda a editar");
                        break;
                    };

                    if (SelectedSonda.RETIRO == "S")
                    {
                        NombreLeyendaMenuSondas = "Agregar";
                        OnPropertyChanged("NombreLeyendaMenuSondas");

                        new Dialogos().ConfirmacionDialogo("Validación", "Una sonda ya retirada no puede ser editada");
                        break;
                    };

                    VisualizandoSondas = false;//NO ESTOY VIENDO, VOY A EDITAR
                    EnabledretirarSondasHE = false;
                    FechaInstalacionSondaHE = SelectedSonda.INSTALACION_FEC;
                    SelectedRetiroSondaNHE = SelectedSonda.RETIRO;
                    ObservacionesSondaN = SelectedSonda.OBSERVACION;
                    EnabledEdicionSonda = true;
                    EstaEditandoSondas = true;
                    break;

                case "retiro_sonda":
                    if (SelectedSonda == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione una sonda a editar");
                        break;
                    };

                    if (SelectedSonda.RETIRO == "S")
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "La sonda seleccionada ya fue retirada");
                        break;
                    };

                    VisualizandoSondas = false;//NO ESTOY VIENDO, VOY A EDITAR
                    EnabledEdicionSonda = false;
                    EnabledretirarSondasHE = true;
                    FechaInstalacionSondaHE = SelectedSonda.INSTALACION_FEC;
                    SelectedRetiroSondaNHE = SelectedSonda.RETIRO;
                    ObservacionesSondaN = SelectedSonda.OBSERVACION;
                    break;
                #endregion
                #region RAYOS X
                case "cargar_rayos_turno_ant":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    ConsultaRayosXTurnoAnterior();
                    break;
                case "agregar_rayos_hoja_enfermeria":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    if (string.IsNullOrEmpty(NuevoRayosXAgregarHEView))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ingrese los Rayos X a agregar");
                        break;
                    };

                    if (!string.IsNullOrEmpty(RayosXExistenteHE))
                    {
                        if ((RayosXExistenteHE.Length + NuevoRayosXAgregarHEView.Length) > 1000)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Al agregar estos Rayos X supera el máximo permitido");
                            break;
                        };
                    }
                    else
                        if (NuevoRayosXAgregarHEView.Length > 1000)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Al agregar estos Rayos X supera el máximo permitido");
                            break;
                        }

                    RayosXExistenteHE += string.Format("{0} \n", NuevoRayosXAgregarHEView);
                    NuevoRayosXAgregarHEView = string.Empty;
                    break;
                #endregion
                #region LABORATORIOS
                case "cargar_lab_turno_ant":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    ConsultaLaboratorioTurnoAnterior();
                    break;
                case "agregar_lab_hoja_enfermeria":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    };

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    };

                    if (string.IsNullOrEmpty(NuevoLaboratorioAgregarHEView))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ingrese el laboratorio a agregar");
                        break;
                    };

                    if (!string.IsNullOrEmpty(LaboratorioExistenteHE))
                    {
                        if ((LaboratorioExistenteHE.Length + NuevoLaboratorioAgregarHEView.Length) > 1000)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Al agregar este laboratorio supera el máximo permitido");
                            break;
                        };
                    }
                    else
                        if (NuevoLaboratorioAgregarHEView.Length > 1000)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Al agregar este laboratorio supera el máximo permitido");
                            break;
                        };

                    LaboratorioExistenteHE += string.Format("{0} \n", NuevoLaboratorioAgregarHEView);
                    NuevoLaboratorioAgregarHEView = string.Empty;
                    break;
                #endregion
                #region INCIDENCIAS
                case "agregar_incidente":
                    if (LstIncidenciasCateterHE == null)
                        LstIncidenciasCateterHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENCIAS_CATETER>();

                    if (LstIncidenciassonda == null)
                        LstIncidenciassonda = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENCIAS_SONDA_GAS>();

                    if (string.IsNullOrEmpty(Observacion))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ingrese las observaciones de la incidencia");
                        return;
                    }

                    if (SelectedIncidenteMotivoValue == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccion el motivo de la incidencia");
                        return;
                    }

                    if (SelectedIncidenteMotivoValue == -1)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccion el motivo de la incidencia");
                        return;
                    }

                    if (PosicionActual == (short)ePosicionActualHojaEnfermeria.CATETER)
                    {
                        var Incidencia = new SSP.Servidor.INCIDENCIAS_CATETER()
                        {
                            FECHA_REGISTRO = Fechas.GetFechaDateServer,
                            ID_ACMOTIVO = SelectedIncidenteMotivoValue,
                            ID_CENTRO_UBI = GlobalVar.gCentro,
                            ID_USUARIO = GlobalVar.gUsr,
                            OBSERVACIONES = Observacion,
                            ID_INC_CATETER = (int)SelectedHojaCatater.ID_HOJAENFCAT
                        };

                        LstIncidenciasCateterHE.Add(Incidencia);
                        var DatosT = SelectedHojaCatater;
                        if (LstCateteres.Remove(SelectedHojaCatater))
                        {
                            LstCateteres.Add(new SSP.Servidor.HOJA_ENFERMERIA_CATETER
                            {
                                DATOS_INFECCION = DatosIngeccionCateterHE,
                                ID_CATETER = SelectedTipoCataterHE,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_HOSPITA = SelectedHospitalizacion,
                                ID_USUARIO = GlobalVar.gUsr,
                                INSTALACION_FEC = FechaInstalacionCatHE,
                                RETIRO = SelectedRetiroCateterHE,
                                REGISTRO_FEC = Fechas.GetFechaDateServer,
                                MOTIVO_RETIRO = MotivoRetiroCateterHE,
                                VENCIMIENTO_FEC = FechavencimientoCatHE,
                                ID_REGISTRO_INICIAL = -1,
                                FECHA_RETIRO = FechaRetiroCateterHE,
                                CATETER_TIPO = SelectedTipoC,
                                ID_HOJAENFCAT = DatosT.ID_HOJAENFCAT
                            });

                            LimpiaDatosCateter();
                        };
                    }

                    if (PosicionActual == (short)ePosicionActualHojaEnfermeria.SONDAS)
                    {
                        var Incidencia = new SSP.Servidor.INCIDENCIAS_SONDA_GAS()
                        {
                            FECHA_REGISTRO = Fechas.GetFechaDateServer,
                            ID_ACMOTIVO = SelectedIncidenteMotivoValue,
                            ID_CENTRO_UBI = GlobalVar.gCentro,
                            ID_USUARIO = GlobalVar.gUsr,
                            OBSERVACIONES = Observacion,
                            ID_INC_SONDA_GAS = (int)SelectedSonda.ID_HOJAENF
                        };

                        LstIncidenciassonda.Add(Incidencia);
                        var DatosT = SelectedSonda;
                        if (LstSondas.Remove(SelectedSonda))
                        {
                            LstSondas.Add(new SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS
                            {
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_HOSPITA = SelectedHospitalizacion,
                                ID_USUARIO = GlobalVar.gUsr,
                                RETIRO = SelectedRetiroSondaNHE,
                                INSTALACION_FEC = FechaInstalacionSondaHE,
                                OBSERVACION = ObservacionesSondaN,
                                REGISTRO_FEC = Fechas.GetFechaDateServer,
                                FECHA_RETIRO = FechaRetiroSondaHE,
                                ID_HOJAENF = DatosT.ID_HOJAENF,
                            });

                            LimpiaDatosSondas();
                        };
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
                    break;
                case "cancelar_incidente":
                    EstaEditando = false;

                    if (PosicionActual == (short)ePosicionActualHojaEnfermeria.CATETER)
                        LimpiaDatosCateter();

                    if (PosicionActual == (short)ePosicionActualHojaEnfermeria.SONDAS)
                        LimpiaDatosSondas();

                    base.RemoveRule("Observacion");
                    base.RemoveRule("SelectedIncidenteMotivoValue");
                    OnPropertyChanged("Observacion");
                    OnPropertyChanged("SelectedIncidenteMotivoValue");
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
                    break;
                #endregion
            }
        }

        private void AgregaIncidenciaCateter()
        {
            try
            {
                base.RemoveRule("Observacion");
                base.RemoveRule("SelectedIncidenteMotivoValue");
                OnPropertyChanged("Observacion");
                OnPropertyChanged("SelectedIncidenteMotivoValue");
                LstIncidenteMotivo = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA_IN_MOTIVO>(new SSP.Controlador.Catalogo.Justicia.cAtencion_Cita_In_Motivo().GetData(x => x.ESTATUS == "S", y => y.DESCR));
                LstIncidenteMotivo.Insert(0, new SSP.Servidor.ATENCION_CITA_IN_MOTIVO { DESCR = "SELECCIONE", ID_ACMOTIVO = -1 });
                RaisePropertyChanged("LstIncidenteMotivo");
                SelectedIncidenteMotivoValue = -1;
                Observacion = string.Empty;
                ValidacionesIncidencias();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void AgregaIncidenciaSonda()
        {
            try
            {
                base.RemoveRule("Observacion");
                base.RemoveRule("SelectedIncidenteMotivoValue");
                OnPropertyChanged("Observacion");
                OnPropertyChanged("SelectedIncidenteMotivoValue");
                LstIncidenteMotivo = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA_IN_MOTIVO>(new SSP.Controlador.Catalogo.Justicia.cAtencion_Cita_In_Motivo().GetData(x => x.ESTATUS == "S", y => y.DESCR));
                LstIncidenteMotivo.Insert(0, new SSP.Servidor.ATENCION_CITA_IN_MOTIVO { DESCR = "SELECCIONE", ID_ACMOTIVO = -1 });
                RaisePropertyChanged("LstIncidenteMotivo");
                SelectedIncidenteMotivoValue = -1;
                Observacion = string.Empty;
                ValidacionesIncidencias();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void IngresaIncidencia()
        {
            try
            {
                if (CateterTemp == null)
                    return;

                //CateterTemp

            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void LimpiaDatosSondas()
        {
            try
            {
                VisualizandoSondas = false;
                FechaInstalacionSondaHE = FechaRetiroSondaHE = null;
                SelectedRetiroSondaNHE = ObservacionesSondaN = string.Empty;
                SelectedSonda = null;
                EnabledEdicionSonda = true;
                EnabledretirarSondasHE = true;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private async void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_fecha_inicio_cateter":
                    if (!FechaInstalacionCatHE.HasValue || !FechaRetiroCateterHE.HasValue || FechaRetiroCateterHE >= FechaInstalacionCatHE)
                        IsFechaIniBusquedaValida = true;
                    else
                        IsFechaIniBusquedaValida = false;
                    break;
                case "cambio_fecha_inicio_sonda":
                    if (!FechaInstalacionSondaHE.HasValue || !FechaRetiroSondaHE.HasValue || FechaRetiroSondaHE >= FechaInstalacionSondaHE)
                        IsFechaSondaValida = true;
                    else
                        IsFechaSondaValida = false;
                    break;
                #region Cambio SelectedItem de Busqueda de Expediente
                case "cambio_expediente":
                    if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            selectExpediente = new SSP.Controlador.Catalogo.Justicia.cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
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
                #endregion
            };
        }

        private void LimpiaDatosCateter()
        {
            try
            {
                VisualizandoCateteres = false;
                SelectedTipoCataterHE = -1;
                FechaInstalacionCatHE = FechavencimientoCatHE = FechaRetiroCateterHE = null;
                SelectedRetiroCateterHE = DatosIngeccionCateterHE = MotivoRetiroCateterHE = string.Empty;
                SelectedHojaCatater = null;
                EnabledRetirosCateterHE = false;
                EnabledPermiteQuitarCataterHe = true;
                EnabledQuitarCateter = true;
                NombreLeyendaMenuCateter = "Agregar";
                OnPropertyChanged("NombreLeyendaMenuCateter");
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void LimpiaCopmpletaHojaEnfermeria()
        {
            try
            {
                #region limpia los datos principales del interno
                AnioD = FolioD = new int?();
                PaternoD = MaternoD = NombreD = SexoImp = EdadImp = DiagnosticoImp = PesoImp = CamaImp = CentroImp = string.Empty;
                FechaNacimientoImputado = FechaIngresoHospitalizacion = new System.DateTime?();
                #endregion
                #region limpia los signos vitales
                LimpiarSignosVitales();
                LstSignosVitalesHojaEnfermeria = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_LECTURA>();
                #endregion
                #region limpia las soluciones
                ListRecetas = new System.Collections.ObjectModel.ObservableCollection<RecetaMedica>();
                #endregion
                #region limpia los ingresos y egresos
                LimpiaIgresosEgresos((short)ePosicionActualHojaEnfermeria.INGRESOS);
                LimpiaIgresosEgresos((short)ePosicionActualHojaEnfermeria.EGRESOS);
                #endregion
                #region limpia los medicamentos
                LimpiaCamposMedicamento();
                LstCustmoMedi = new System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas>();
                #endregion
                #region limpia las notas de enfermeria
                AnotacionNuevaHojaEnfermeria = NotaEnfermeriaExistente = TextoAntiguaNotaEnfermeria = string.Empty;
                #endregion
                #region limpia las ulceras y lesiones
                TextDescripcionLesion = string.Empty;
                ListLesiones = new System.Collections.ObjectModel.ObservableCollection<LesionesCustom>();
                #endregion
                #region Cateteres
                FechaInstalacionCatHE = FechavencimientoCatHE = FechaRetiroCateterHE = null;
                SelectedRetiroCateterHE = DatosIngeccionCateterHE = MotivoRetiroCateterHE = string.Empty;
                EstaEditando = false;
                #endregion
                #region Sonda Gast
                FechaInstalacionSondaHE = null;
                SelectedRetiroSondaNHE = ObservacionesSondaN = string.Empty;
                #endregion
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void CargaMedicamentoEdicion(cCustomMedicamentosNotas Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                LimpiaCamposMedicamento();
                NombreMedicamentoEditar = Entity.NombreMedicamento;
                UnidadMedidaMedicamentoEditar = Entity.UnidadMedida;
                NombrePresentacionMedicaMedicamentoEditar = Entity.Presentacion;
                CantidadMedicamentoEditar = Entity.Cantidad.HasValue ? Entity.Cantidad.Value.ToString() : string.Empty;
                FechaSuministroMedicamentoEditar = Fechas.GetFechaDateServer;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void LimpiaCamposMedicamento()
        {
            try
            {
                NombreMedicamentoEditar = UnidadMedidaMedicamentoEditar = CantidadMedicamentoEditar = NombrePresentacionMedicaMedicamentoEditar = string.Empty;
                FechaSuministroMedicamentoEditar = null;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ConsulltaNotasEnfermeriaTurnoAnterior()
        {
            try
            {
                TextoAntiguaNotaEnfermeria = string.Empty;
                var _DatosHojas = new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeria().GetData(x => x.ID_HOSPITA == SelectedHospitalizacion);
                if (_DatosHojas.Any())
                {
                    var _UltimaHojaAntes = _DatosHojas.OrderByDescending(x => x.ID_HOJAENF);
                    if (_UltimaHojaAntes != null && _UltimaHojaAntes.Any())
                    {
                        var Ultim = _UltimaHojaAntes.FirstOrDefault(z => z.ID_HOJAENF != SelectedHojaId);
                        if (Ultim != null)
                            TextoAntiguaNotaEnfermeria = !string.IsNullOrEmpty(Ultim.OBSERVACION) ? Ultim.OBSERVACION.Trim() : string.Empty;
                        else
                            if (SelectedHojaId == decimal.Zero)
                                TextoAntiguaNotaEnfermeria = !string.IsNullOrEmpty(Ultim.OBSERVACION) ? Ultim.OBSERVACION.Trim() : string.Empty;
                    };
                };
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ConsultaLaboratorioTurnoAnterior()
        {
            try
            {
                TextoAntiguaLaboratorio = string.Empty;
                var _DatosHojas = new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeria().GetData(x => x.ID_HOSPITA == SelectedHospitalizacion);
                if (_DatosHojas.Any())
                {
                    var _UltimaHojaAntes = _DatosHojas.OrderByDescending(x => x.ID_HOJAENF);
                    if (_UltimaHojaAntes != null && _UltimaHojaAntes.Any())
                    {
                        var Ultim = _UltimaHojaAntes.FirstOrDefault(z => z.ID_HOJAENF != SelectedHojaId);
                        if (Ultim != null)
                            TextoAntiguaLaboratorio = !string.IsNullOrEmpty(Ultim.LABORATORIO) ? Ultim.LABORATORIO.Trim() : string.Empty;
                        else
                            if (SelectedHojaId == decimal.Zero)
                                TextoAntiguaLaboratorio = !string.IsNullOrEmpty(Ultim.LABORATORIO) ? Ultim.LABORATORIO.Trim() : string.Empty;
                    };
                };
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ConsultaRayosXTurnoAnterior()
        {
            try
            {
                TextoAntiguaRayosX = string.Empty;
                var _DatosHojas = new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeria().GetData(x => x.ID_HOSPITA == SelectedHospitalizacion);
                if (_DatosHojas.Any())
                {
                    var _UltimaHojaAntes = _DatosHojas.OrderByDescending(x => x.ID_HOJAENF);
                    if (_UltimaHojaAntes != null && _UltimaHojaAntes.Any())
                    {
                        var Ultim = _UltimaHojaAntes.FirstOrDefault(z => z.ID_HOJAENF != SelectedHojaId);
                        if (Ultim != null)
                            TextoAntiguaRayosX = !string.IsNullOrEmpty(Ultim.RX) ? Ultim.RX.Trim() : string.Empty;
                        else
                            if (SelectedHojaId == decimal.Zero)///NO EXISTE UNA HOJA COMO TAL
                                TextoAntiguaRayosX = !string.IsNullOrEmpty(Ultim.RX) ? Ultim.RX.Trim() : string.Empty;
                    };
                };
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ConsultaMedicamentosNotasMedicas()
        {
            try
            {
                if (SelectExpediente == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                    return;
                };

                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                    return;
                };

                LstCustmoMedi = new System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas>();
                var DetallesMedicam = new SSP.Controlador.Catalogo.Justicia.cRecetaMedicaDetalle().Obtener_Historial_Hospitalizacion(SelectedHospitalizacion, GlobalVar.gCentro);
                var _MedicamentosNotaMedicaInicial = new SSP.Controlador.Catalogo.Justicia.cRecetaMedicaDetalle().Obtener_Historial_Hospitalizacion(SelectedHospitalizacion, GlobalVar.gCentro);
                if (_MedicamentosNotaMedicaInicial != null && _MedicamentosNotaMedicaInicial.Any())
                    foreach (var item in _MedicamentosNotaMedicaInicial)
                        LstCustmoMedi.Add(new cCustomMedicamentosNotas
                        {
                            Cantidad = item.DOSIS,
                            Cena = item.CENA == 1 ? "SI" : item.CENA == 2 ? "NO" : string.Empty,
                            Comida = item.COMIDA == 1 ? "SI" : item.COMIDA == 2 ? "NO" : string.Empty,
                            Desayuno = item.DESAYUNO == 1 ? "SI" : item.DESAYUNO == 2 ? "NO" : string.Empty,
                            Duracion = item.DURACION,
                            IdPResentacionProducto = item.ID_PRESENTACION_MEDICAMENTO,
                            IdProducto = item.ID_PRODUCTO,
                            NombreMedicamento = item.PRODUCTO != null ? !string.IsNullOrEmpty(item.PRODUCTO.NOMBRE) ? item.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty,
                            Obsertvaciones = item.OBSERV,
                            FechaReceto = item.RECETA_MEDICA != null ? item.RECETA_MEDICA.RECETA_FEC : new System.DateTime?(),
                            FechaSuministro = null,
                            Id = null,
                            UltimaFecha = null,
                            IdFolio = item.ID_FOLIO,
                            IdAtencionMedica = item.ID_ATENCION_MEDICA,
                            Presentacion = item.PRESENTACION_MEDICAMENTO != null ? !string.IsNullOrEmpty(item.PRESENTACION_MEDICAMENTO.DESCRIPCION) ? item.PRESENTACION_MEDICAMENTO.DESCRIPCION.Trim() : string.Empty : string.Empty,
                            UnidadMedida = item.PRODUCTO != null ? item.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? !string.IsNullOrEmpty(item.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE) ? item.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty
                        });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private void LimpiaCamposDetalle()
        {
            try
            {
                EspecifiqueOtroLiquido = string.Empty;//SECCION DE INGRESOS
                NombreMedicamentoEditar = UnidadMedidaMedicamentoEditar = NombrePresentacionMedicaMedicamentoEditar = CantidadMedicamentoEditar = string.Empty;
                FechaMaximaRegistroMedicamentos = Fechas.GetFechaDateServer;
                TextoAntiguaNotaEnfermeria = NotaEnfermeriaExistente = TextDescripcionLesion = string.Empty;
                TextoAntiguaLaboratorio = string.Empty;
                TextoAntiguaRayosX = string.Empty;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ProcesaDatosHojaEnfermeria()
        {
            try
            {
                LimpiaCamposDetalle();//SE ASEGURA QUE SE LIMPIEN LOS CAMPOS DE DETALLES DE LAS CONSULTAS A TURNOS PREVIOS PARA ELIMINAR DATOS RESIDUALES

                SelectedHojaId = decimal.Zero;
                LstLiquidosHorasEgresoHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA>(new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S" && x.ID_LIQTURNO == SelectedTurnoLiquidos));
                if (LstLiquidosHorasEgresoHE != null && LstLiquidosHorasEgresoHE.Any())
                    LstLiquidosHorasEgresoHE.ToList().ForEach(x => x.DESCR = string.Format("{0}:00", x.DESCR));

                LstLiquidosHorasEgresoHE.Insert(0, new SSP.Servidor.LIQUIDO_HORA { DESCR = "SELECCIONE", ID_LIQHORA = -1 });
                RaisePropertyChanged("LstLiquidosHorasEgresoHE");


                LstLiquidosHorasHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA>(new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S" && x.ID_LIQTURNO == SelectedTurnoLiquidos));
                if (LstLiquidosHorasHE != null && LstLiquidosHorasHE.Any())
                    LstLiquidosHorasHE.ToList().ForEach(x => x.DESCR = string.Format("{0}:00", x.DESCR));

                LstLiquidosHorasHE.Insert(0, new SSP.Servidor.LIQUIDO_HORA { DESCR = "SELECCIONE", ID_LIQHORA = -1 });
                RaisePropertyChanged("LstLiquidosHorasHE");

                SelectedHoraEgreso = -1;
                SelectedHoraIngresosHE = -1;

                OnPropertyChanged("SelectedHoraEgreso");
                OnPropertyChanged("SelectedHoraIngresosHE");

                LstSignosVitalesHojaEnfermeria = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_LECTURA>();
                ListRecetas = new System.Collections.ObjectModel.ObservableCollection<RecetaMedica>();
                LstLiquidosIngresoHojaEnfermeria = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_CONTROL_ENFERMERIA>();
                LstLiquidosEgresoHojaEnfermeria = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_CONTROL_ENFERMERIA>();
                LstMedicamentosHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_MEDICAMENTO>();
                LstUlcerasHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_ULCERA>();
                LstCustmoMedi = new System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas>();
                ListLesiones = new System.Collections.ObjectModel.ObservableCollection<LesionesCustom>();
                LstCateteres = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_CATETER>();
                LstSondas = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS>();

                #region Limpia las incidencias para que pueda generar mas
                LstIncidenciasCateterHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENCIAS_CATETER>();
                LstIncidenciassonda = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENCIAS_SONDA_GAS>();
                #endregion
                OnPropertyChanged("LstSondas");
                OnPropertyChanged("LstSignosVitalesHojaEnfermeria");
                OnPropertyChanged("ListRecetas");
                OnPropertyChanged("LstCateteres");
                OnPropertyChanged("LstLiquidosIngresoHojaEnfermeria");
                OnPropertyChanged("LstLiquidosEgresoHojaEnfermeria");
                OnPropertyChanged("LstMedicamentosHE");
                OnPropertyChanged("LstUlcerasHE");
                OnPropertyChanged("LstCustmoMedi");
                OnPropertyChanged("ListLesiones");

                NotaEnfermeriaExistente = RayosXExistenteHE = LaboratorioExistenteHE = string.Empty;//SE ASEGURA DE QUE ESTEN LIMPIOS LOS HISTORICOS PARA EL TURNO NUEVO O EXISTENTE
                var _DatosHoja = new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeria().GetData(x => x.FECHA_REGISTRO.Value.Year == FechaHojaenfermeria.Value.Year && x.FECHA_REGISTRO.Value.Month == FechaHojaenfermeria.Value.Month && x.FECHA_REGISTRO.Value.Day == FechaHojaenfermeria.Value.Day && x.ID_LIQTURNO == SelectedTurnoLiquidos && x.ID_HOSPITA == SelectedHospitalizacion).FirstOrDefault();
                if (_DatosHoja != null)
                {
                    //ConsultaMedicamentosNotasMedicas();
                    SelectedHojaId = _DatosHoja.ID_HOJAENF;
                    LaboratorioExistenteHE = _DatosHoja.LABORATORIO;
                    RayosXExistenteHE = _DatosHoja.RX;
                    if (_DatosHoja.HOJA_ENFERMERIA_LECTURA != null && _DatosHoja.HOJA_ENFERMERIA_LECTURA.Any())
                        foreach (var item in _DatosHoja.HOJA_ENFERMERIA_LECTURA)
                            LstSignosVitalesHojaEnfermeria.Add(item);

                    var _DetallesLiquidosHojasEnfermeria = new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeriaSolucion().ObtenerSolucionesTurnos(FechaHojaenfermeria, SelectedHospitalizacion);
                    var listaR = _DetallesLiquidosHojasEnfermeria != null ? _DetallesLiquidosHojasEnfermeria.Any() ? _DetallesLiquidosHojasEnfermeria.DistinctBy(x => x.ID_PRODUCTO) : null : null;
                    if (listaR != null && listaR.Any())
                    {
                        foreach (var item in listaR)
                            if (ListRecetas.Any(x => x.PRODUCTO == item.PRODUCTO))
                                continue;
                            else
                            {
                                var ValidaExistencia = new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeriaSolucion().GetData(x => x.ID_PRODUCTO == item.ID_PRODUCTO && x.ID_HOJAENF == _DatosHoja.ID_HOJAENF && x.ID_HOSPITA == SelectedHospitalizacion);
                                if (ValidaExistencia != null && ValidaExistencia.Any())
                                    if (ValidaExistencia.Count(x => x.TERMINO == "S") == ValidaExistencia.Count(y => y.TERMINO == "N"))//coincide la cantidad de entradas y salidas para este producto en cualquier turno
                                        continue;

                                ListRecetas.Add(new RecetaMedica
                                {
                                    PRODUCTO = item.PRODUCTO,
                                    HORA_NOCHE = false,
                                    PRESENTACION = item.PRESENTACION_MEDICAMENTO != null ? item.PRESENTACION_MEDICAMENTO.ID_PRESENTACION_MEDICAMENTO : new short?()
                                });
                            }
                    };

                    if (_DatosHoja.HOJA_CONTROL_ENFERMERIA != null && _DatosHoja.HOJA_CONTROL_ENFERMERIA.Any())
                        foreach (var item in _DatosHoja.HOJA_CONTROL_ENFERMERIA)
                            if (item.LIQUIDO != null)
                                if (item.LIQUIDO.ID_LIQTIPO == "1")
                                    LstLiquidosIngresoHojaEnfermeria.Add(item);
                                else
                                    if (item.LIQUIDO.ID_LIQTIPO == "2")
                                        LstLiquidosEgresoHojaEnfermeria.Add(item);

                    if (_DatosHoja.HOJA_ENFERMERIA_CATETER != null && _DatosHoja.HOJA_ENFERMERIA_CATETER.Any())
                    {
                        var CatVivos = _DatosHoja.HOJA_ENFERMERIA_CATETER;
                        if (CatVivos != null && CatVivos.Any())
                            foreach (var item in CatVivos)
                                LstCateteres.Add(new SSP.Servidor.HOJA_ENFERMERIA_CATETER
                                    {
                                        DATOS_INFECCION = item.DATOS_INFECCION,
                                        ID_CATETER = item.ID_CATETER,
                                        ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                        ID_HOJAENF = item.ID_HOJAENF,
                                        ID_HOJAENFCAT = item.ID_HOJAENFCAT,
                                        ID_HOSPITA = item.ID_HOSPITA,
                                        FECHA_RETIRO = item.FECHA_RETIRO,
                                        ID_REGISTRO_INICIAL = item.ID_REGISTRO_INICIAL,
                                        ID_USUARIO = item.ID_USUARIO,
                                        MOTIVO_RETIRO = item.MOTIVO_RETIRO,
                                        INSTALACION_FEC = item.INSTALACION_FEC,
                                        REGISTRO_FEC = item.REGISTRO_FEC,
                                        RETIRO = item.RETIRO,
                                        CATETER_TIPO = item.CATETER_TIPO,
                                        VENCIMIENTO_FEC = item.VENCIMIENTO_FEC
                                    });
                    }

                    if (_DatosHoja.HOJA_ENFERMERIA_SONDA_GASOGAS != null && _DatosHoja.HOJA_ENFERMERIA_SONDA_GASOGAS.Any())
                    {
                        var SondasVivas = _DatosHoja.HOJA_ENFERMERIA_SONDA_GASOGAS;
                        if (SondasVivas != null && SondasVivas.Any())
                            foreach (var item in SondasVivas)
                            {
                                LstSondas.Add(new SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS
                                    {
                                        ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                        ID_HOJAENF = item.ID_HOJAENF,
                                        ID_HOSPITA = item.ID_HOSPITA,
                                        ID_REGISTRO_ORIGINAL = item.ID_REGISTRO_ORIGINAL,
                                        ID_SONDAGAS = item.ID_SONDAGAS,
                                        ID_USUARIO = item.ID_USUARIO,
                                        INSTALACION_FEC = item.INSTALACION_FEC,
                                        OBSERVACION = item.OBSERVACION,
                                        REGISTRO_FEC = item.REGISTRO_FEC,
                                        RETIRO = item.RETIRO,
                                        FECHA_RETIRO = FechaRetiroSondaHE
                                    });
                            };
                    }

                    System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas> listaTempMedicamentos = new System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas>();
                    var _DetallesHospitalizacion = new SSP.Controlador.Catalogo.Justicia.Medico.cHospitalizacion().GetData(x => x.ID_HOSPITA == SelectedHospitalizacion).FirstOrDefault();
                    var _MedicamentosNotaMedicaInicial = new SSP.Controlador.Catalogo.Justicia.cRecetaMedicaDetalle().Obtener_Historial_Hospitalizacion(SelectedHospitalizacion, GlobalVar.gCentro);
                    if (_MedicamentosNotaMedicaInicial != null && _MedicamentosNotaMedicaInicial.Any())
                        foreach (var item in _MedicamentosNotaMedicaInicial)
                            listaTempMedicamentos.Add(new cCustomMedicamentosNotas
                            {
                                Cantidad = item.DOSIS,
                                Cena = item.CENA == 1 ? "SI" : item.CENA == 2 ? "NO" : string.Empty,
                                Comida = item.COMIDA == 1 ? "SI" : item.COMIDA == 2 ? "NO" : string.Empty,
                                Desayuno = item.DESAYUNO == 1 ? "SI" : item.DESAYUNO == 2 ? "NO" : string.Empty,
                                Duracion = item.DURACION,
                                IdPResentacionProducto = item.ID_PRESENTACION_MEDICAMENTO,
                                IdProducto = item.ID_PRODUCTO,
                                NombreMedicamento = item.PRODUCTO != null ? !string.IsNullOrEmpty(item.PRODUCTO.NOMBRE) ? item.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty,
                                Obsertvaciones = item.OBSERV,
                                Id = null,
                                FechaReceto = item.RECETA_MEDICA != null ? item.RECETA_MEDICA.RECETA_FEC : new System.DateTime?(),
                                FechaSuministro = null,
                                UltimaFecha = null,
                                IdFolio = item.ID_FOLIO,
                                IdAtencionMedica = item.ID_ATENCION_MEDICA,
                                Presentacion = item.PRESENTACION_MEDICAMENTO != null ? !string.IsNullOrEmpty(item.PRESENTACION_MEDICAMENTO.DESCRIPCION) ? item.PRESENTACION_MEDICAMENTO.DESCRIPCION.Trim() : string.Empty : string.Empty,
                                UnidadMedida = item.PRODUCTO != null ? item.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? !string.IsNullOrEmpty(item.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE) ? item.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty
                            });

                    var _MedicamentosGuardados = _DatosHoja.HOJA_ENFERMERIA_MEDICAMENTO;
                    foreach (var item in listaTempMedicamentos)
                    {
                        SSP.Servidor.HOJA_ENFERMERIA_MEDICAMENTO MedicamentoGuardado = _MedicamentosGuardados.Any() ? _MedicamentosGuardados.FirstOrDefault(x => x.ID_PRODUCTO == item.IdProducto) : null;
                        if (MedicamentoGuardado != null)
                        {
                            if (LstCustmoMedi.Any(x => x.Id == MedicamentoGuardado.ID_HOJA_ENFMED))
                                continue;//EN EL QUE ESTOY YA EXISTE DENTRO DE LA LISTA, OMITELO

                            LstCustmoMedi.Add(new cCustomMedicamentosNotas
                            {
                                Cantidad = MedicamentoGuardado.CANTIDAD,
                                Cena = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.CENA == 1 ? "SI" : MedicamentoGuardado.RECETA_MEDICA_DETALLE.CENA == 2 ? "NO" : string.Empty : string.Empty,
                                Comida = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.COMIDA == 1 ? "SI" : MedicamentoGuardado.RECETA_MEDICA_DETALLE.COMIDA == 2 ? "NO" : string.Empty : string.Empty,
                                Desayuno = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.DESAYUNO == 1 ? "SI" : MedicamentoGuardado.RECETA_MEDICA_DETALLE.DESAYUNO == 2 ? "NO" : string.Empty : string.Empty,
                                Duracion = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.DURACION : new short?(),
                                IdPResentacionProducto = MedicamentoGuardado.ID_PRESENTACION_MEDICAMENTO,
                                Id = MedicamentoGuardado.ID_HOJA_ENFMED,
                                IdProducto = MedicamentoGuardado.ID_PRODUCTO,
                                NombreMedicamento = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO != null ? !string.IsNullOrEmpty(MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO.NOMBRE) ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                Obsertvaciones = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? !string.IsNullOrEmpty(MedicamentoGuardado.RECETA_MEDICA_DETALLE.OBSERV) ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.OBSERV.Trim() : string.Empty : string.Empty,
                                FechaReceto = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.RECETA_MEDICA != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.RECETA_MEDICA.RECETA_FEC : new System.DateTime?() : new System.DateTime?(),
                                FechaSuministro = MedicamentoGuardado.FECHA_SUMINISTRO,
                                UltimaFecha = MedicamentoGuardado.FECHA_REGISTRO.HasValue ? MedicamentoGuardado.FECHA_REGISTRO.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                IdFolio = MedicamentoGuardado.ID_FOLIO,
                                IdAtencionMedica = MedicamentoGuardado.ID_ATENCION_MEDICA,
                                Presentacion = MedicamentoGuardado.PRESENTACION_MEDICAMENTO != null ? !string.IsNullOrEmpty(MedicamentoGuardado.PRESENTACION_MEDICAMENTO.DESCRIPCION) ? MedicamentoGuardado.PRESENTACION_MEDICAMENTO.DESCRIPCION.Trim() : string.Empty : string.Empty,
                                UnidadMedida = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? !string.IsNullOrEmpty(MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE) ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty
                            });
                            continue;
                        }
                        else
                        {
                            LstCustmoMedi.Add(new cCustomMedicamentosNotas
                            {
                                Cantidad = item.Cantidad,
                                Cena = item.Cena,
                                Comida = item.Comida,
                                Desayuno = item.Desayuno,
                                Duracion = item.Duracion,
                                FechaReceto = item.FechaReceto,
                                FechaSuministro = null,
                                Id = null,
                                IdAtencionMedica = item.IdAtencionMedica,
                                IdFolio = item.IdFolio,
                                IdPResentacionProducto = item.IdPResentacionProducto,
                                IdProducto = item.IdProducto,
                                NombreMedicamento = item.NombreMedicamento,
                                Obsertvaciones = item.Obsertvaciones,
                                Presentacion = item.Presentacion,
                                UltimaFecha = string.Empty,
                                UnidadMedida = item.UnidadMedida
                            });
                        }
                    }

                    if (_DatosHoja.HOJA_ENFERMERIA_ULCERA != null && _DatosHoja.HOJA_ENFERMERIA_ULCERA.Any())
                        foreach (var item in _DatosHoja.HOJA_ENFERMERIA_ULCERA)
                            ListLesiones.Add(new LesionesCustom
                                {
                                    DESCR = item.DESC,
                                    REGION = item.ANATOMIA_TOPOGRAFICA
                                });

                    NotaEnfermeriaExistente = _DatosHoja.OBSERVACION;

                    if (_DatosHoja.ESTATUS == "N")
                        EnabledHojas = MenuGuardarEnabled = false;///ESTA INACTIVA, NO ES POSIBLE CONTINUAR TRABAJANDO SOBRE ELLA
                    else
                        if (_DatosHoja.ESTATUS == "S")
                        {
                            //ESTA ACTIVA, HAY QUE VALIDAR QUE TENGA LOS PERMISOS
                            EnabledHojas = true;///ESTA ACTIVA, ES POSIBLE CONTINUAR TRABAJANDO SOBRE ELLA
                            ConfiguraPermisos();
                        };
                }
                else
                {
                    EnabledHojas = true;///ESTA ACTIVA, ES POSIBLE CONTINUAR TRABAJANDO SOBRE ELLA, VERIFICA LOS PERMISOS QUE TIENE SOBRE EL MODULO
                    ConfiguraPermisos();

                    SSP.Servidor.HOJA_ENFERMERIA _DatosUltimaHoja = new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeria().ObtenerUltimaHojaEnfermeria(FechaHojaenfermeria, SelectedHospitalizacion, SelectIngreso).OrderByDescending(y => y.ID_HOJAENF).FirstOrDefault();
                    if (_DatosUltimaHoja != null)
                    {
                        //ESTOS SON LOS DATOS DE LA ULTIMA HOJA QUE SE LE GENERO A ESTE INTERNO
                        var _DetallesLiquidosHojasEnfermeria = new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeriaSolucion().ObtenerSolucionesTurnos(FechaHojaenfermeria, SelectedHospitalizacion);
                        var listaR = _DetallesLiquidosHojasEnfermeria != null ? _DetallesLiquidosHojasEnfermeria.Any() ? _DetallesLiquidosHojasEnfermeria.DistinctBy(x => x.ID_PRODUCTO) : null : null;
                        if (listaR != null && listaR.Any())
                        {
                            foreach (var item in listaR)
                            {
                                if (ListRecetas.Any(x => x.PRODUCTO == item.PRODUCTO))
                                    continue;
                                else
                                    ListRecetas.Add(new RecetaMedica
                                    {
                                        PRODUCTO = item.PRODUCTO,
                                        HORA_NOCHE = false,
                                        PRESENTACION = item.PRESENTACION_MEDICAMENTO != null ? item.PRESENTACION_MEDICAMENTO.ID_PRESENTACION_MEDICAMENTO : new short?()
                                    });
                            };
                        };

                        #region datos de las sondas y cateteres
                        if (_DatosUltimaHoja.HOJA_ENFERMERIA_CATETER != null && _DatosUltimaHoja.HOJA_ENFERMERIA_CATETER.Any())
                        {
                            var CatNoFinalizados = _DatosUltimaHoja.HOJA_ENFERMERIA_CATETER;
                            if (CatNoFinalizados != null && CatNoFinalizados.Any())
                                foreach (var item in CatNoFinalizados)
                                {
                                    LstCateteres.Add(new SSP.Servidor.HOJA_ENFERMERIA_CATETER
                                        {
                                            DATOS_INFECCION = item.DATOS_INFECCION,
                                            ID_CATETER = item.ID_CATETER,
                                            ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                            ID_HOJAENF = item.ID_HOJAENF,
                                            ID_HOJAENFCAT = item.ID_HOJAENFCAT,
                                            ID_HOSPITA = item.ID_HOSPITA,
                                            ID_REGISTRO_INICIAL = item.ID_REGISTRO_INICIAL,
                                            ID_USUARIO = item.ID_USUARIO,
                                            INSTALACION_FEC = item.INSTALACION_FEC,
                                            MOTIVO_RETIRO = item.MOTIVO_RETIRO,
                                            REGISTRO_FEC = item.REGISTRO_FEC,
                                            CATETER_TIPO = item.CATETER_TIPO,
                                            RETIRO = item.RETIRO,
                                            FECHA_RETIRO = item.FECHA_RETIRO,
                                            VENCIMIENTO_FEC = item.VENCIMIENTO_FEC
                                        });
                                };
                        };

                        if (_DatosUltimaHoja.HOJA_ENFERMERIA_SONDA_GASOGAS != null && _DatosUltimaHoja.HOJA_ENFERMERIA_SONDA_GASOGAS.Any())
                        {
                            var SondasVivas = _DatosUltimaHoja.HOJA_ENFERMERIA_SONDA_GASOGAS;
                            if (SondasVivas != null && SondasVivas.Any())
                                foreach (var item in SondasVivas)
                                    LstSondas.Add(new SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS
                                        {
                                            ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                            ID_HOJAENF = item.ID_HOJAENF,
                                            ID_HOSPITA = item.ID_HOSPITA,
                                            ID_REGISTRO_ORIGINAL = item.ID_REGISTRO_ORIGINAL,
                                            ID_SONDAGAS = item.ID_SONDAGAS,
                                            ID_USUARIO = item.ID_USUARIO,
                                            FECHA_RETIRO = FechaRetiroSondaHE,
                                            INSTALACION_FEC = item.INSTALACION_FEC,
                                            OBSERVACION = item.OBSERVACION,
                                            RETIRO = item.RETIRO,
                                            REGISTRO_FEC = item.REGISTRO_FEC
                                        });
                        };

                        #endregion
                        System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas> listaTempMedicamentos = new System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas>();
                        var _MedicamentosNotaMedicaInicial = new SSP.Controlador.Catalogo.Justicia.cRecetaMedicaDetalle().Obtener_Historial_Hospitalizacion(SelectedHospitalizacion, GlobalVar.gCentro);
                        if (_MedicamentosNotaMedicaInicial != null && _MedicamentosNotaMedicaInicial.Any())
                            foreach (var item in _MedicamentosNotaMedicaInicial)
                                listaTempMedicamentos.Add(new cCustomMedicamentosNotas
                                {
                                    Cantidad = item.DOSIS,
                                    Cena = item.CENA == 1 ? "SI" : item.CENA == 2 ? "NO" : string.Empty,
                                    Comida = item.COMIDA == 1 ? "SI" : item.COMIDA == 2 ? "NO" : string.Empty,
                                    Desayuno = item.DESAYUNO == 1 ? "SI" : item.DESAYUNO == 2 ? "NO" : string.Empty,
                                    Duracion = item.DURACION,
                                    IdPResentacionProducto = item.ID_PRESENTACION_MEDICAMENTO,
                                    IdProducto = item.ID_PRODUCTO,
                                    NombreMedicamento = item.PRODUCTO != null ? !string.IsNullOrEmpty(item.PRODUCTO.NOMBRE) ? item.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty,
                                    Obsertvaciones = item.OBSERV,
                                    FechaReceto = item.RECETA_MEDICA != null ? item.RECETA_MEDICA.RECETA_FEC : new System.DateTime?(),
                                    FechaSuministro = null,
                                    UltimaFecha = null,
                                    IdFolio = item.ID_FOLIO,
                                    IdAtencionMedica = item.ID_ATENCION_MEDICA,
                                    Presentacion = item.PRESENTACION_MEDICAMENTO != null ? !string.IsNullOrEmpty(item.PRESENTACION_MEDICAMENTO.DESCRIPCION) ? item.PRESENTACION_MEDICAMENTO.DESCRIPCION.Trim() : string.Empty : string.Empty,
                                    UnidadMedida = item.PRODUCTO != null ? item.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? !string.IsNullOrEmpty(item.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE) ? item.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty
                                });

                        var _MedicamentosGuardados = _DatosUltimaHoja.HOJA_ENFERMERIA_MEDICAMENTO;
                        if (listaTempMedicamentos != null && listaTempMedicamentos.Any())
                            foreach (var item in listaTempMedicamentos)
                            {
                                SSP.Servidor.HOJA_ENFERMERIA_MEDICAMENTO MedicamentoGuardado = _MedicamentosGuardados.Any() ? _MedicamentosGuardados.FirstOrDefault(x => x.ID_PRODUCTO == item.IdProducto) : null;
                                if (MedicamentoGuardado != null)
                                {
                                    if (LstCustmoMedi.Any(x => x.Id == MedicamentoGuardado.ID_HOJA_ENFMED))
                                        continue;

                                    LstCustmoMedi.Add(new cCustomMedicamentosNotas
                                        {
                                            Cantidad = MedicamentoGuardado.CANTIDAD,
                                            Cena = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.CENA == 1 ? "SI" : MedicamentoGuardado.RECETA_MEDICA_DETALLE.CENA == 2 ? "NO" : string.Empty : string.Empty,
                                            Comida = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.COMIDA == 1 ? "SI" : MedicamentoGuardado.RECETA_MEDICA_DETALLE.COMIDA == 2 ? "NO" : string.Empty : string.Empty,
                                            Desayuno = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.DESAYUNO == 1 ? "SI" : MedicamentoGuardado.RECETA_MEDICA_DETALLE.DESAYUNO == 2 ? "NO" : string.Empty : string.Empty,
                                            Duracion = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.DURACION : new short?(),
                                            IdPResentacionProducto = MedicamentoGuardado.ID_PRESENTACION_MEDICAMENTO,
                                            IdProducto = MedicamentoGuardado.ID_PRODUCTO,
                                            NombreMedicamento = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO != null ? !string.IsNullOrEmpty(MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO.NOMBRE) ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                            Obsertvaciones = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? !string.IsNullOrEmpty(MedicamentoGuardado.RECETA_MEDICA_DETALLE.OBSERV) ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.OBSERV.Trim() : string.Empty : string.Empty,
                                            FechaReceto = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.RECETA_MEDICA != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.RECETA_MEDICA.RECETA_FEC : new System.DateTime?() : new System.DateTime?(),
                                            FechaSuministro = MedicamentoGuardado.FECHA_SUMINISTRO,
                                            UltimaFecha = MedicamentoGuardado.FECHA_REGISTRO.HasValue ? MedicamentoGuardado.FECHA_REGISTRO.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                            IdFolio = MedicamentoGuardado.ID_FOLIO,
                                            IdAtencionMedica = MedicamentoGuardado.ID_ATENCION_MEDICA,
                                            Presentacion = MedicamentoGuardado.PRESENTACION_MEDICAMENTO != null ? !string.IsNullOrEmpty(MedicamentoGuardado.PRESENTACION_MEDICAMENTO.DESCRIPCION) ? MedicamentoGuardado.PRESENTACION_MEDICAMENTO.DESCRIPCION.Trim() : string.Empty : string.Empty,
                                            UnidadMedida = MedicamentoGuardado.RECETA_MEDICA_DETALLE != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO != null ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? !string.IsNullOrEmpty(MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE) ? MedicamentoGuardado.RECETA_MEDICA_DETALLE.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty
                                        });
                                    continue;
                                }
                                else
                                {
                                    LstCustmoMedi.Add(new cCustomMedicamentosNotas
                                        {
                                            Cantidad = item.Cantidad,
                                            Cena = item.Cena,
                                            Comida = item.Comida,
                                            Desayuno = item.Desayuno,
                                            Duracion = item.Duracion,
                                            FechaReceto = item.FechaReceto,
                                            FechaSuministro = null,
                                            Id = null,
                                            IdAtencionMedica = item.IdAtencionMedica,
                                            IdFolio = item.IdFolio,
                                            IdPResentacionProducto = item.IdPResentacionProducto,
                                            IdProducto = item.IdProducto,
                                            NombreMedicamento = item.NombreMedicamento,
                                            Obsertvaciones = item.Obsertvaciones,
                                            Presentacion = item.Presentacion,
                                            UltimaFecha = string.Empty,
                                            UnidadMedida = item.UnidadMedida
                                        });
                                }
                            };
                    }
                    else
                    {
                        //SI ENTRA AQUI ES PORQUE NUNCA SE LE HA GENERADO AL MENOS UNA NOTA DE ENFERMERIA A ESTE INTERNO
                        ConsultaMedicamentosNotasMedicas();
                    }
                }

                var DetallesMedicam = new SSP.Controlador.Catalogo.Justicia.cRecetaMedicaDetalle().Obtener_Historial_Hospitalizacion(SelectedHospitalizacion, GlobalVar.gCentro);

            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void RegionSwitch(System.Object obj)
        {
            try
            {
                SelectRegion = short.Parse(obj.ToString().Split('-')[0]);
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la region del cuerpo.", ex);
            }
        }

        private void LesionSelected(System.Object obj)
        {
            try
            {
                if (!(obj is LesionesCustom)) return;
                SelectLesion = (LesionesCustom)obj;
                TextDescripcionLesion = SelectLesion.DESCR;
                SelectRegion = SelectLesion.REGION.ID_REGION;
                if (SelectLesion.REGION.LADO == "D")
                    foreach (var item in ListRadioButonsDorso)
                        if (item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : true)
                        {
                            item.IsChecked = true;
                            return;
                        }
                if (SelectLesion.REGION.LADO == "F")
                    foreach (var item in ListRadioButonsFrente)
                        if (item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : true)
                        {
                            item.IsChecked = true;
                            return;
                        }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la lesion.", ex);
            }
        }

        private void AgregrLiquido(short Opcion)
        {
            try
            {
                if (Opcion != decimal.Zero)
                    switch (Opcion)
                    {
                        case (short)ePosicionActualHojaEnfermeria.INGRESOS:
                            if (SelectedLiquidoIngreso == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un líquido para agregar a la lista.");
                                return;
                            };

                            if (SelectedLiquidoIngreso == -1)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un líquido para agregar a la lista.");
                                return;
                            };

                            if (base.HasErrors)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                                return;
                            };

                            if (LstLiquidosIngresoHojaEnfermeria == null)
                                LstLiquidosIngresoHojaEnfermeria = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_CONTROL_ENFERMERIA>();

                            if (LstLiquidosIngresoHojaEnfermeria.Any(x => x.ID_LIQ == SelectedLiquidoIngreso))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El líquido seleccionado ya se encuentra en la lista.");
                                return;
                            };

                            LstLiquidosIngresoHojaEnfermeria.Add(new SSP.Servidor.HOJA_CONTROL_ENFERMERIA
                                {
                                    CANT = CantidadLiqIngresoHE,
                                    FECHA_REGISTRO = Fechas.GetFechaDateServer,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    ID_LIQ = SelectedLiquidoIngreso,
                                    ID_USUARIO = GlobalVar.gUsr,
                                    ID_LIQHORA = SelectedHoraIngresosHE,
                                    OTRO_LIQUIDO = EspecifiqueOtroLiquido,
                                    ID_HOSPITA = SelectedHospitalizacion,
                                    LIQUIDO = SelectedLiquidoIngresoMostrar,
                                    LIQUIDO_HORA = SelectedLiquidoHoraMostrar
                                });

                            LimpiaIgresosEgresos((short)ePosicionActualHojaEnfermeria.INGRESOS);
                            break;

                        case (short)ePosicionActualHojaEnfermeria.EGRESOS:
                            if (SelectedLiquidoEgreso == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un líquido para agregar a la lista.");
                                return;
                            };

                            if (SelectedLiquidoEgreso == -1)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un líquido para agregar a la lista.");
                                return;
                            };

                            if (base.HasErrors)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                                return;
                            };

                            if (LstLiquidosEgresoHojaEnfermeria == null)
                                LstLiquidosEgresoHojaEnfermeria = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_CONTROL_ENFERMERIA>();

                            if (LstLiquidosEgresoHojaEnfermeria.Any(x => x.ID_LIQ == SelectedLiquidoEgreso))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El líquido seleccionado ya se encuentra en la lista.");
                                return;
                            };

                            LstLiquidosEgresoHojaEnfermeria.Add(new SSP.Servidor.HOJA_CONTROL_ENFERMERIA
                                {
                                    ID_LIQ = SelectedLiquidoEgreso,
                                    CANT = CantidadLiqHEEgresos,
                                    FECHA_REGISTRO = Fechas.GetFechaDateServer,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    ID_HOSPITA = SelectedHospitalizacion,
                                    ID_USUARIO = GlobalVar.gUsr,
                                    OTRO_LIQUIDO = string.Empty,
                                    ID_LIQHORA = SelectedHoraEgreso,
                                    LIQUIDO = SelectedLiquidoEgresoMostrar,
                                    LIQUIDO_HORA = SelectedHoraLiquidoEgresoMostrar
                                });

                            LimpiaIgresosEgresos((short)ePosicionActualHojaEnfermeria.EGRESOS);
                            break;
                        default:
                            break;
                    };
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void AgregarSignosVitalesHojaEnfermeria()
        {
            try
            {
                if (LstSignosVitalesHojaEnfermeria == null)
                    LstSignosVitalesHojaEnfermeria = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_LECTURA>();

                LstSignosVitalesHojaEnfermeria.Add(new SSP.Servidor.HOJA_ENFERMERIA_LECTURA
                    {
                        PC = FrecuenciaCardiacaHE,
                        PR = FrecuenciaRespiratoriaHE,
                        TA = !string.IsNullOrEmpty(Arterial1) ? !string.IsNullOrEmpty(Arterial2) ? string.Format("{0} / {1}", Arterial1, Arterial2) : string.Empty : string.Empty,
                        TA_MEDIA = TensionArtMediaHE,
                        TEMP = TempHE,
                        SAO = Sa02HE,
                        DEXTROXTIX = DextrHE,
                        NEB = NebHE,
                        PVC = PVCHE,
                        CAMBIO_POSICION = CambioPosHE,
                        CAMBIO_ESCARAS = RiesgoEscHE,
                        RIESGO_CAIDAS = RiesgoCaiHE,
                        FECHA_REGISTRO = Fechas.GetFechaDateServer,
                        ID_CENTRO_UBI = GlobalVar.gCentro,
                        ID_USUARIO = GlobalVar.gUsr,
                        ID_HOSPITA = SelectedHospitalizacion,
                        FECHA_LECTURA = FechaHoraCaptura
                    });

                LimpiarSignosVitales();
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ProcesaHorasTurno(decimal? _dato)
        {
            try
            {
                if (_dato == null)
                    return;

                var HorasPorTurno = new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ID_LIQTURNO == _dato);
                if (HorasPorTurno.Any())
                {
                    FechaHoraCaptura = null;
                    int HMinima = 0;
                    int HMaxima = 0;
                    var MaximaHora = HorasPorTurno.OrderByDescending(x => x.ID_LIQHORA).FirstOrDefault();
                    var MinimaHora = HorasPorTurno.FirstOrDefault();
                    if (MaximaHora != null)
                        HMaxima = int.Parse(MaximaHora.DESCR);
                    if (MinimaHora != null)
                        HMinima = int.Parse(MinimaHora.DESCR);

                    FechaMaximaHE = FechaMinimaHE = Fechas.GetFechaDateServer;
                    System.DateTime _fechaS = Fechas.GetFechaDateServer;
                    FechaMaximaHE = new System.DateTime(_fechaS.Year, _fechaS.Month, _fechaS.Day, HMaxima, 0, 0);
                    FechaMinimaHE = new System.DateTime(_fechaS.Year, _fechaS.Month, _fechaS.Day, HMinima == 24 ? 0 : HMinima, 0, 0);
                    OnPropertyChanged("FechaMinimaHE");
                    OnPropertyChanged("FechaMaximaHE");
                };
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private bool GuardaHojaenfermeria()
        {
            try
            {
                System.DateTime _fechaR = Fechas.GetFechaDateServer;
                var _HojaEnfermeria = new SSP.Servidor.HOJA_ENFERMERIA()
                {
                    CONCENTRADO_INGRESO = Opcion == 1 ? LstLiquidosIngresoHojaEnfermeria != null ? LstLiquidosIngresoHojaEnfermeria.Any() ? LstLiquidosIngresoHojaEnfermeria.Sum(x => x.CANT) : new short?() : new short?() : new short?(),
                    CONCENTRADO_EGRESO = Opcion == 1 ? LstLiquidosEgresoHojaEnfermeria != null ? LstLiquidosEgresoHojaEnfermeria.Any() ? LstLiquidosEgresoHojaEnfermeria.Sum(x => x.CANT) : new short?() : new short?() : new short?(),
                    ESTATUS = Opcion == 1 ? "N" : "S",
                    FECHA_HOJA = FechaHojaenfermeria,
                    FECHA_REGISTRO = _fechaR,
                    ID_CENTRO_UBI = GlobalVar.gCentro,
                    ID_HOSPITA = SelectedHospitalizacion,
                    ID_LIQTURNO = SelectedTurnoLiquidos,
                    ID_USUARIO = GlobalVar.gUsr,
                    RX = RayosXExistenteHE,
                    LABORATORIO = LaboratorioExistenteHE,
                    OBSERVACION = NotaEnfermeriaExistente,
                    ID_HOJAENF = SelectedHojaId//SI NO EXISTE, SE VA EN CERO Y ASI LO CREA LA BASE DE DATOS, SINO SOLO LO EDITA
                };

                if (LstSignosVitalesHojaEnfermeria != null && LstSignosVitalesHojaEnfermeria.Any())
                    foreach (var item in LstSignosVitalesHojaEnfermeria)
                        _HojaEnfermeria.HOJA_ENFERMERIA_LECTURA.Add(new SSP.Servidor.HOJA_ENFERMERIA_LECTURA
                            {
                                CAMBIO_ESCARAS = item.CAMBIO_ESCARAS,
                                CAMBIO_POSICION = item.CAMBIO_POSICION,
                                SAO = item.SAO,
                                FECHA_REGISTRO = _fechaR,
                                FECHA_LECTURA = item.FECHA_LECTURA,
                                ID_HOSPITA = SelectedHospitalizacion,
                                DEXTROXTIX = item.DEXTROXTIX,
                                ID_USUARIO = GlobalVar.gUsr,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                TA = item.TA,
                                RIESGO_CAIDAS = item.RIESGO_CAIDAS,
                                TEMP = item.TEMP,
                                TA_MEDIA = item.TA_MEDIA,
                                PVC = item.PVC,
                                PR = item.PR,
                                PC = item.PC,
                                NEB = item.NEB
                            });

                if (LstLiquidosIngresoHojaEnfermeria != null && LstLiquidosIngresoHojaEnfermeria.Any())
                    foreach (var item in LstLiquidosIngresoHojaEnfermeria)
                        _HojaEnfermeria.HOJA_CONTROL_ENFERMERIA.Add(new SSP.Servidor.HOJA_CONTROL_ENFERMERIA
                            {
                                CANT = item.CANT,
                                FECHA_REGISTRO = _fechaR,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_HOSPITA = SelectedHospitalizacion,
                                ID_USUARIO = GlobalVar.gUsr,
                                OTRO_LIQUIDO = item.OTRO_LIQUIDO,
                                ID_LIQHORA = item.ID_LIQHORA,
                                ID_LIQ = item.ID_LIQ
                            });

                if (LstLiquidosEgresoHojaEnfermeria != null && LstLiquidosEgresoHojaEnfermeria.Any())
                    foreach (var item in LstLiquidosEgresoHojaEnfermeria)
                        _HojaEnfermeria.HOJA_CONTROL_ENFERMERIA.Add(new SSP.Servidor.HOJA_CONTROL_ENFERMERIA
                            {
                                CANT = item.CANT,
                                FECHA_REGISTRO = _fechaR,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_HOSPITA = SelectedHospitalizacion,
                                ID_USUARIO = GlobalVar.gUsr,
                                OTRO_LIQUIDO = item.OTRO_LIQUIDO,
                                ID_LIQHORA = item.ID_LIQHORA,
                                ID_LIQ = item.ID_LIQ
                            });

                if (ListRecetas != null && ListRecetas.Any())
                    foreach (var item in ListRecetas)
                        _HojaEnfermeria.HOJA_ENFERMERIA_SOLUCION.Add(new SSP.Servidor.HOJA_ENFERMERIA_SOLUCION
                            {
                                FECHA_REGISTRO = _fechaR,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_HOSPITA = SelectedHospitalizacion,
                                ID_PRODUCTO = item.PRODUCTO != null ? item.PRODUCTO.ID_PRODUCTO : new int?(),
                                ID_PRESENTACION_MEDICAMENTO = item.PRESENTACION,
                                TERMINO = item.HORA_NOCHE == true ? "S" : "N",
                                ID_LIQTURNO_INICIO = SelectedTurnoLiquidos
                            });

                if (LstCustmoMedi != null && LstCustmoMedi.Any())
                {
                    var MedicamentosBuenos = LstCustmoMedi.Where(x => x.FechaReceto.HasValue && x.FechaSuministro.HasValue);
                    foreach (var item in MedicamentosBuenos)
                        _HojaEnfermeria.HOJA_ENFERMERIA_MEDICAMENTO.Add(new SSP.Servidor.HOJA_ENFERMERIA_MEDICAMENTO
                        {
                            CANTIDAD = item.Cantidad,
                            FECHA_REGISTRO = Fechas.GetFechaDateServer,
                            FECHA_SUMINISTRO = item.FechaSuministro,
                            ID_CENTRO_UBI = GlobalVar.gCentro,
                            ID_PRESENTACION_MEDICAMENTO = item.IdPResentacionProducto,
                            ID_PRODUCTO = item.IdProducto,
                            ID_USUARIO = GlobalVar.gUsr,
                            ID_ATENCION_MEDICA = item.IdAtencionMedica,
                            ID_FOLIO = item.IdFolio,
                            ID_HOSPITA = SelectedHospitalizacion
                        });
                };

                if (ListLesiones != null && ListLesiones.Any())
                    foreach (var item in ListLesiones)
                        _HojaEnfermeria.HOJA_ENFERMERIA_ULCERA.Add(new SSP.Servidor.HOJA_ENFERMERIA_ULCERA
                            {
                                DESC = item.DESCR,
                                FECHA_REGISTRO = Fechas.GetFechaDateServer,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_REGION = item.REGION != null ? item.REGION.ID_REGION : new short?(),
                                ID_USUARIO = GlobalVar.gUsr,
                                ID_HOSPITA = SelectedHospitalizacion
                            });

                if (LstCateteres != null && LstCateteres.Any())
                {
                    var detallesCatateresEnTurno = LstCateteres.Where(x => x.ID_REGISTRO_INICIAL == -1);//TRABAJA SOBRE LAS QUE FUERON MODIFICADAS POR ESTE USUARIO
                    if (detallesCatateresEnTurno != null && detallesCatateresEnTurno.Any())
                        foreach (var item in detallesCatateresEnTurno)
                        {
                            var IncidentesC = LstIncidenciasCateterHE != null ? LstIncidenciasCateterHE.Any() ? LstIncidenciasCateterHE.Where(x => x.ID_INC_CATETER == item.ID_HOJAENFCAT).ToList() : null : null;
                            _HojaEnfermeria.HOJA_ENFERMERIA_CATETER.Add(new SSP.Servidor.HOJA_ENFERMERIA_CATETER
                            {
                                DATOS_INFECCION = item.DATOS_INFECCION,
                                ID_CATETER = item.ID_CATETER,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_HOSPITA = SelectedHospitalizacion,
                                ID_USUARIO = GlobalVar.gUsr,
                                INSTALACION_FEC = item.INSTALACION_FEC,
                                FECHA_RETIRO = item.FECHA_RETIRO,
                                VENCIMIENTO_FEC = item.VENCIMIENTO_FEC,
                                RETIRO = item.RETIRO,
                                CATETER_TIPO = item.CATETER_TIPO,
                                REGISTRO_FEC = Fechas.GetFechaDateServer,
                                MOTIVO_RETIRO = item.MOTIVO_RETIRO,
                                INCIDENCIAS_CATETER = IncidentesC
                            });
                        };
                };


                if (LstSondas != null && LstSondas.Any())
                    foreach (var item in LstSondas)
                    {
                        var IncidenciasS = LstIncidenciassonda != null ? LstIncidenciassonda.Any() ? LstIncidenciassonda.Where(x => x.ID_INC_SONDA_GAS == item.ID_HOJAENF).ToList() : null : null;
                        _HojaEnfermeria.HOJA_ENFERMERIA_SONDA_GASOGAS.Add(new SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS
                        {
                            ID_CENTRO_UBI = GlobalVar.gCentro,
                            ID_HOSPITA = SelectedHospitalizacion,
                            ID_USUARIO = GlobalVar.gUsr,
                            INSTALACION_FEC = item.INSTALACION_FEC,
                            OBSERVACION = item.OBSERVACION,
                            RETIRO = item.RETIRO,
                            FECHA_RETIRO = FechaRetiroSondaHE,
                            REGISTRO_FEC = Fechas.GetFechaDateServer,
                            INCIDENCIAS_SONDA_GAS = IncidenciasS
                        });
                    };

                return new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeria().GuardaHojaEnfermeria(_HojaEnfermeria);
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void LimpiaIgresosEgresos(short Opcion)
        {
            try
            {
                switch (Opcion)
                {
                    case (short)ePosicionActualHojaEnfermeria.INGRESOS:
                        LstLiquidosIngresoHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO>(new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "1" && x.ID_LIQ_FORMATO == (short)eTiposFormatos.HOJA_ENFERMERIA));
                        LstLiquidosIngresoHE.Insert(0, new SSP.Servidor.LIQUIDO { ID_LIQ = -1, DESCR = "SELECCIONE" });
                        LstLiquidosHorasHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA>(new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S" && x.ID_LIQTURNO == SelectedTurnoLiquidos));
                        if (LstLiquidosHorasHE != null && LstLiquidosHorasHE.Any())
                            LstLiquidosHorasHE.ToList().ForEach(x => x.DESCR = string.Format("{0}:00", x.DESCR));

                        LstLiquidosHorasHE.Insert(0, new SSP.Servidor.LIQUIDO_HORA { DESCR = "SELECCIONE", ID_LIQHORA = -1 });
                        RaisePropertyChanged("LstLiquidosHorasHE");
                        RaisePropertyChanged("LstLiquidosIngresoHE");
                        SelectedHoraIngresosHE = -1;
                        SelectedLiquidoIngreso = -1;
                        OnPropertyChanged("SelectedLiquidoIngreso");
                        OnPropertyChanged("SelectedHoraIngresosHE");
                        EnabledOtrosIngresosHE = false;
                        CantidadLiqIngresoHE = null;
                        EspecifiqueOtroLiquido = string.Empty;
                        break;

                    case (short)ePosicionActualHojaEnfermeria.EGRESOS:
                        LstLiquidosEgresoHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO>(new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "2" && x.ID_LIQ_FORMATO == (short)eTiposFormatos.HOJA_ENFERMERIA));
                        LstLiquidosEgresoHE.Insert(0, new SSP.Servidor.LIQUIDO { ID_LIQ = -1, DESCR = "SELECCIONE" });
                        RaisePropertyChanged("LstLiquidosEgresoHE");

                        LstLiquidosHorasEgresoHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA>(new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S" && x.ID_LIQTURNO == SelectedTurnoLiquidos));
                        if (LstLiquidosHorasEgresoHE != null && LstLiquidosHorasEgresoHE.Any())
                            LstLiquidosHorasEgresoHE.ToList().ForEach(x => x.DESCR = string.Format("{0}:00", x.DESCR));

                        LstLiquidosHorasEgresoHE.Insert(0, new SSP.Servidor.LIQUIDO_HORA { DESCR = "SELECCIONE", ID_LIQHORA = -1 });
                        RaisePropertyChanged("LstLiquidosHorasEgresoHE");
                        SelectedHoraEgreso = -1;
                        SelectedLiquidoEgreso = -1;
                        OnPropertyChanged("SelectedLiquidoEgreso");
                        OnPropertyChanged("SelectedHoraEgreso");

                        CantidadLiqHEEgresos = null;
                        OnPropertyChanged("CantidadLiqHEEgresos");
                        break;

                    default:
                        //no action
                        break;
                }
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void LimpiarSignosVitales()
        {
            try
            {
                FrecuenciaCardiacaHE = FrecuenciaRespiratoriaHE = Arterial1 = Arterial2 = TensionArtMediaHE = TempHE = Sa02HE = DextrHE = NebHE = PVCHE = CambioPosHE = RiesgoEscHE = RiesgoCaiHE = string.Empty;
                FechaHoraCaptura = null;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private async void ModelEnter(System.Object obj)
        {
            try
            {
                if (obj != null)
                    if (!obj.GetType().Name.Equals("String"))
                    {
                        var textbox = obj as System.Windows.Controls.TextBox;
                        if (textbox != null)
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

                ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();

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
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
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

                        var _validacionHospitalizado = new SSP.Controlador.Catalogo.Justicia.Medico.cHospitalizacion().ObtenerHospitalizacionesPorIngreso(SelectIngreso);
                        if (!_validacionHospitalizado.Any())
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no esta hospitalizado.");
                            return;
                        }

                        LimpiaCopmpletaHojaEnfermeria();
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
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)//Empty row
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;

                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private void SeleccionaIngreso()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    var _CentroActual = new SSP.Controlador.Catalogo.Justicia.cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    CentroImp = _CentroActual != null ? !string.IsNullOrEmpty(_CentroActual.DESCR) ? _CentroActual.DESCR.Trim() : string.Empty : string.Empty;
                    AnioD = SelectIngreso.ID_ANIO;
                    FolioD = SelectIngreso.ID_IMPUTADO;
                    PaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                    MaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                    NombreD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                    SexoImp = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.SEXO) ? SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : SelectIngreso.IMPUTADO.SEXO == "F" ? "FEMENINO" : string.Empty : string.Empty : string.Empty;
                    EdadImp = SelectIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty;
                    FechaNacimientoImputado = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA : new System.DateTime?();
                    var _validacionHospitalizado = new SSP.Controlador.Catalogo.Justicia.Medico.cHospitalizacion().ObtenerHospitalizacionesPorIngreso(SelectIngreso);
                    if (_validacionHospitalizado != null && _validacionHospitalizado.Any())
                    {
                        var _UnicaHospitalizacion = _validacionHospitalizado.FirstOrDefault();
                        SelectedHospitalizacion = _UnicaHospitalizacion.ID_HOSPITA;
                        var _Enfermedades = _UnicaHospitalizacion.NOTA_MEDICA != null ? _UnicaHospitalizacion.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD : null;
                        if (_Enfermedades != null && _Enfermedades.Any())
                            foreach (var item in _Enfermedades)
                                DiagnosticoImp += string.Format("{0}, ", item.ENFERMEDAD != null ? !string.IsNullOrEmpty(item.ENFERMEDAD.NOMBRE) ? item.ENFERMEDAD.NOMBRE.Trim() : string.Empty : string.Empty);

                        DiagnosticoImp = !string.IsNullOrEmpty(DiagnosticoImp) ? DiagnosticoImp.TrimEnd(',') : string.Empty;
                        OnPropertyChanged("DiagnosticoImp");

                        CamaImp = _UnicaHospitalizacion.CAMA_HOSPITAL != null ? !string.IsNullOrEmpty(_UnicaHospitalizacion.CAMA_HOSPITAL.DESCR) ? _UnicaHospitalizacion.CAMA_HOSPITAL.DESCR.Trim() : string.Empty : string.Empty;
                        PesoImp = _UnicaHospitalizacion.NOTA_MEDICA != null ? _UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO) ? _UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty;
                        FechaIngresoHospitalizacion = _UnicaHospitalizacion.INGRESO_FEC;
                    };

                    VisiblePasoDos = System.Windows.Visibility.Visible;
                    //VisiblePrincipal = System.Windows.Visibility.Visible; //AUN NO MUESTRES LA PESTAñA PRINCIPAL, FALTA SELECCIONE LA FECHA Y EL TURNO
                    InicializaEntornoCaptura();
                };
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }

        private async void ClickEnter(System.Object obj)
        {
            try
            {
                if (obj != null)
                {
                    var textbox = obj as System.Windows.Controls.TextBox;
                    if (textbox != null)
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

                ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;

                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }

            return;
        }

        private void InicializaEntornoCaptura()
        {
            try
            {
                #region SIGNOS VITALES
                FrecuenciaCardiacaHE = string.Empty;
                FrecuenciaRespiratoriaHE = string.Empty;
                Arterial1 = string.Empty;
                Arterial2 = string.Empty;
                TensionArtMediaHE = string.Empty;
                TempHE = string.Empty;
                Sa02HE = string.Empty;
                DextrHE = string.Empty;
                NebHE = string.Empty;
                PVCHE = string.Empty;
                CambioPosHE = string.Empty;
                RiesgoEscHE = string.Empty;
                RiesgoCaiHE = string.Empty;
                #endregion

                #region LISTAS DE INGRESOS Y EGRESOS
                LstLiquidosIngresoHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO>(new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "1" && x.ID_LIQ_FORMATO == (short)eTiposFormatos.HOJA_ENFERMERIA));
                LstLiquidosEgresoHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO>(new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "2" && x.ID_LIQ_FORMATO == (short)eTiposFormatos.HOJA_ENFERMERIA));
                LstLiquidosHorasHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA>(new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S"));
                LstLiquidosHorasEgresoHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA>(new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S"));
                LstLiquidosHorasHE.Insert(0, new SSP.Servidor.LIQUIDO_HORA { DESCR = "SELECCIONE", ID_LIQHORA = -1 });
                LstLiquidosIngresoHE.Insert(0, new SSP.Servidor.LIQUIDO { ID_LIQ = -1, DESCR = "SELECCIONE" });
                LstLiquidosEgresoHE.Insert(0, new SSP.Servidor.LIQUIDO { ID_LIQ = -1, DESCR = "SELECCIONE" });
                LstLiquidosHorasEgresoHE.Insert(0, new SSP.Servidor.LIQUIDO_HORA { DESCR = "SELECCIONE", ID_LIQHORA = -1 });

                RaisePropertyChanged("LstLiquidosEgresoHE");
                RaisePropertyChanged("LstLiquidosIngresoHE");
                RaisePropertyChanged("LstLiquidosHorasHE");
                RaisePropertyChanged("LstLiquidosHorasEgresoHE");

                SelectedLiquidoEgreso = SelectedLiquidoIngreso = SelectedHoraIngresosHE = SelectedHoraEgreso = -1;

                OnPropertyChanged("SelectedLiquidoIngreso");
                OnPropertyChanged("SelectedHoraIngresosHE");
                OnPropertyChanged("SelectedHoraEgreso");
                OnPropertyChanged("SelectedLiquidoEgreso");
                #endregion

                #region CATETER
                LstTiposCatater = new System.Collections.Generic.List<SSP.Servidor.CATETER_TIPO>(new SSP.Controlador.Catalogo.Justicia.cCateterTipo().GetData(x => x.ESTATUS == "S"));
                LstTiposCatater.Insert(0, new SSP.Servidor.CATETER_TIPO { DESCR = "SELECCIONE", ID_CATETER = -1 });
                SelectedTipoCataterHE = -1;
                SelectedRetiroCateterHE = DatosIngeccionCateterHE = MotivoRetiroCateterHE = string.Empty;
                OnPropertyChanged("SelectedRetiroCateterHE");
                FechaInstalacionCatHE = FechavencimientoCatHE = FechaRetiroCateterHE = null;
                EnabledPermiteQuitarCataterHe = true;
                #endregion

                #region SONDAS
                FechaInstalacionSondaHE = null;
                SelectedRetiroSondaNHE = ObservacionesSondaN = string.Empty;
                LstSondas = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS>();
                #endregion

                LstSignosVitalesHojaEnfermeria = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_LECTURA>();
                ListRecetas = new System.Collections.ObjectModel.ObservableCollection<RecetaMedica>();
                LstLiquidosIngresoHojaEnfermeria = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_CONTROL_ENFERMERIA>();
                LstLiquidosEgresoHojaEnfermeria = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_CONTROL_ENFERMERIA>();
                LstCustmoMedi = new System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas>();

                OnPropertyChanged("LstTiposCatater");
                OnPropertyChanged("LstSignosVitalesHojaEnfermeria");
                OnPropertyChanged("LstSondas");
                OnPropertyChanged("LstLiquidosIngresoHojaEnfermeria");
                OnPropertyChanged("LstLiquidosEgresoHojaEnfermeria");
                OnPropertyChanged("LstCustmoMedi");
                OnPropertyChanged("ListRecetas");
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void InicializaListas()
        {
            try
            {
                ListTurnosLiquidos = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_TURNO>(new SSP.Controlador.Catalogo.Justicia.cLiquidoTurno().GetData(x => x.ESTATUS == "S", y => y.DESCR));
                ListTurnosLiquidos.Insert(0, new SSP.Servidor.LIQUIDO_TURNO { DESCR = "SELECCIONE", ID_LIQTURNO = -1 });
                RaisePropertyChanged("ListTurnosLiquidos");
                SelectedTurnoLiquidos = -1;
                OnPropertyChanged("SelectedTurnoLiquidos");

                LstIncidenciassonda = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENCIAS_SONDA_GAS>();
                lstIncidenciasCateterHE = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENCIAS_CATETER>();
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void SeniasFrenteLoad(SeniasFrenteView Window = null)
        {
            try
            {
                if (Window == null)
                    return;

                if (ListRadioButonsFrente != null ? ListRadioButonsFrente.Any() : true)
                    return;

                ListRadioButonsFrente = new System.Collections.Generic.List<System.Windows.Controls.RadioButton>();
                ListRadioButonsFrente = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<System.Windows.Controls.RadioButton>(((System.Windows.Controls.Grid)Window.FindName("GridFrente"))).ToList();
                if (SelectLesion != null)
                    foreach (var item in ListRadioButonsFrente)
                        item.IsChecked = item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : true;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos iniciales de las lesiones.", ex);
            }
        }

        private void SeniasDorsoLoad(SeniasDorsoView Window = null)
        {
            try
            {
                if (Window == null)
                    return;

                if (ListRadioButonsDorso != null ? ListRadioButonsDorso.Any() : true)
                    return;

                ListRadioButonsDorso = new System.Collections.Generic.List<System.Windows.Controls.RadioButton>();
                ListRadioButonsDorso = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<System.Windows.Controls.RadioButton>(((System.Windows.Controls.Grid)Window.FindName("GridDorso"))).ToList();
                if (SelectLesion != null)
                    foreach (var item in ListRadioButonsDorso)
                        item.IsChecked = item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : true;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos iniciales de las lesiones.", ex);
            }
        }

        private void VisualizarInformacionCateterSeleccionado(SSP.Servidor.HOJA_ENFERMERIA_CATETER Entity)
        {
            try
            {
                VisualizandoCateteres = true;
                EnabledQuitarCateter = EnabledRetirosCateterHE = EnabledPermiteQuitarCataterHe = false;
                SelectedTipoCataterHE = Entity.ID_CATETER;
                FechaInstalacionCatHE = Entity.INSTALACION_FEC;
                FechavencimientoCatHE = Entity.VENCIMIENTO_FEC;
                SelectedRetiroCateterHE = Entity.RETIRO;
                FechaRetiroCateterHE = Entity.FECHA_RETIRO;
                DatosIngeccionCateterHE = Entity.DATOS_INFECCION;
                MotivoRetiroCateterHE = Entity.MOTIVO_RETIRO;
                EnabledRetirosCateterHE = false;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del catéter.", ex);
            }
        }

        private void VisualizarInformacionSondaSeleccionada(SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS Entity)
        {
            try
            {
                VisualizandoSondas = true;
                EnabledEdicionSonda = EnabledretirarSondasHE = IsEnabledFechaSonda = false;
                FechaInstalacionSondaHE = Entity.INSTALACION_FEC;
                SelectedRetiroSondaNHE = Entity.RETIRO;
                FechaRetiroSondaHE = Entity.FECHA_RETIRO;
                ObservacionesSondaN = Entity.OBSERVACION;
                IsEnabledFechaSonda = false;
                EnabledretirarSondasHE = false;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }


    }
}
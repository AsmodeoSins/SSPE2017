using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class HistoriaClinicaViewModel
    {
        private HistoriaClinicaView HistoriaClinicaView;
        private OdontogramaInicialHistoriaClinicaDentalView _WindowOdontogramaInicial;
        private OdontogramaSeguimientoDental _WindowSeguimientoOdontograma;
        private async void LoadHistoriaClinicDental(HistoriaClinicaView Window)
        {
            try
            {
                HistoriaClinicaView = Window;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ConfiguraPermisos();
                    PrepararListas();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de la historia clínica dental.", ex);
            }
        }

        private void InicializaOdontogramaInicialDental(OdontogramaInicialHistoriaClinicaDentalView Window = null)
        {
            try
            {
                if (Window == null)
                    return;

                if (!EsDentista)
                    return;

                _WindowOdontogramaInicial = Window;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void InicilizaOdontogramaSeguimientoDental(OdontogramaSeguimientoDental Window = null)
        {
            try
            {
                if (Window == null)
                    return;

                if (!EsDentista)
                    return;

                _WindowSeguimientoOdontograma = Window;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void GuardaDientesSeguimiento()
        {
            try
            {
                if (ListSeguimientoDientes == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione las áreas a considerar");
                    return;
                }

                if (ListSeguimientoDientes.Count == 0)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione al menos un área a considerar");
                    return;
                }

                if (IdTratamientoSeguimiento == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione el tratamiento a considerar");
                    return;
                }

                if (IdTratamientoSeguimiento == -1)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione el tratamiento a considerar");
                    return;
                }

                if (FechaProbTratamientoDental == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Ingrese una fecha posible de tratamiento");
                    return;
                }

                if (LstSeguimientoDental == null)
                    LstSeguimientoDental = new ObservableCollection<ODONTOGRAMA_SEGUIMIENTO2>();


                ListSeguimientoDientes = new List<CustomOdontograma>();
                IdTratamientoSeguimiento = -1;
                FechaProbTratamientoDental = Fechas.GetFechaDateServer;
                ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)_WindowSeguimientoOdontograma.FindName("BigGrid"))).ToList();
                if (ListCheckBoxOdontograma.Any())
                    foreach (var item in ListCheckBoxOdontograma)
                        item.IsChecked = false;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private void GuardaDatosDientes()
        {
            try
            {
                if (ListOdontograma == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione las áreas a considerar");
                    return;
                }

                if (ListOdontograma.Count == 0)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione al menos un área a considerar");
                    return;
                }

                if (!SelectedAplicaEnfermedadDental && !SelectedAplicaTratamientoDental)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione la enfermedad o el tratamiento a considerar");
                    return;
                }


                if (SelectedAplicaEnfermedadDental)
                    if (IdSelectedEnfermedadDental == null || IdSelectedEnfermedadDental == -1)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione la enfermedad a considerar");
                        return;
                    }

                if (SelectedAplicaTratamientoDental)
                    if (IdSelectedTratamientoDental == null || IdSelectedTratamientoDental == -1)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione el tratamiento a considerar");
                        return;
                    }

                if (LstOdontogramaInicial == null)
                    LstOdontogramaInicial = new ObservableCollection<ODONTOGRAMA_INICIAL>();

                foreach (var item in ListOdontograma)
                {
                    string _detalleTrat = string.Empty;
                    DateTime _FechaActual = Fechas.GetFechaDateServer;
                    LstOdontogramaInicial.Add(new ODONTOGRAMA_INICIAL
                        {
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_ENFERMEDAD = IdSelectedEnfermedadDental,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            REGISTRO_FEC = _FechaActual,
                            ID_NOMENCLATURA = IdSelectedTratamientoDental,
                            ID_TIPO_ODO = item.ID_POSICION,
                            ID_POSICION = item.ID_DIENTE
                        });
                };

                ListOdontograma = new List<CustomOdontograma>();
                SelectedAplicaEnfermedadDental = SelectedAplicaTratamientoDental = false;
                IdSelectedTratamientoDental = -1;
                IdSelectedEnfermedadDental = -1;
                ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)_WindowOdontogramaInicial.FindName("GridOdontograma"))).ToList();
                if (ListCheckBoxOdontograma.Any())
                    foreach (var item in ListCheckBoxOdontograma)
                        item.IsChecked = false;

            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void LimpiarDientesIniciales()
        {
            try
            {
                ListOdontograma = new List<CustomOdontograma>();
                SelectedAplicaEnfermedadDental = SelectedAplicaTratamientoDental = false;
                IdSelectedTratamientoDental = -1;
                IdSelectedEnfermedadDental = -1;
                ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)_WindowOdontogramaInicial.FindName("GridOdontograma"))).ToList();
                if (ListCheckBoxOdontograma.Any())
                    foreach (var item in ListCheckBoxOdontograma)
                        item.IsChecked = false;
            }

            catch (Exception exc)
            {
                throw;
            }
        }

        private void LimpiarDientesSeguimiento()
        {
            try
            {
                ListSeguimientoDientes = new List<CustomOdontograma>();
                IdTratamientoSeguimiento = -1;
                FechaProbTratamientoDental = Fechas.GetFechaDateServer;
                ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)_WindowSeguimientoOdontograma.FindName("BigGrid"))).ToList();
                if (ListCheckBoxOdontograma.Any())
                    foreach (var item in ListCheckBoxOdontograma)
                        item.IsChecked = false;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void MarcaOdontogramaPosicion(Object obj = null)
        {
            try
            {
                if (obj != null)
                {
                    if (obj is ODONTOGRAMA_INICIAL)
                    {
                        var _Odonto = (ODONTOGRAMA_INICIAL)obj;

                        //Primero limpia para que no se sobre escriban los dientes que se muestran al usuario
                        ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)_WindowOdontogramaInicial.FindName("GridOdontograma"))).ToList();
                        if (ListCheckBoxOdontograma.Any())
                            foreach (var item in ListCheckBoxOdontograma)
                                item.IsChecked = false;

                        //Despues indica el diente que se selecciono
                        ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)_WindowOdontogramaInicial.FindName("GridOdontograma"))).ToList();
                        if (ListCheckBoxOdontograma.Any())
                        {
                            var _elegido = ListCheckBoxOdontograma.FirstOrDefault(x => ((object[])x.CommandParameter)[1].ToString() == string.Format("{0}_{1}", _Odonto.ID_POSICION, _Odonto.ID_TIPO_ODO));
                            if (_elegido != null)
                                _elegido.IsChecked = true;
                        };
                    }

                    if (obj is ODONTOGRAMA_SEGUIMIENTO2)
                    {
                        var _OdontoSeguimiento = (ODONTOGRAMA_SEGUIMIENTO2)obj;

                        //Primero limpia para que no se sobre escriban los dientes que se muestran al usuario
                        ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)_WindowSeguimientoOdontograma.FindName("BigGrid"))).ToList();
                        if (ListCheckBoxOdontograma.Any())
                            foreach (var item in ListCheckBoxOdontograma)
                                item.IsChecked = false;

                        //Despues indica el diente que se selecciono
                        ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)_WindowSeguimientoOdontograma.FindName("BigGrid"))).ToList();
                        if (ListCheckBoxOdontograma.Any())
                        {
                            var _elegido = ListCheckBoxOdontograma.FirstOrDefault(x => ((object[])x.CommandParameter)[1].ToString() == string.Format("{0}_{1}", _OdontoSeguimiento.ID_POSICION, _OdontoSeguimiento.ID_TIPO_ODO));
                            if (_elegido != null)
                                _elegido.IsChecked = true;
                        };
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void InicializaDatosHistoriaClinicaDental()
        {
            try
            {
                InicializaListasDentales();

                if (SelectIngreso == null)
                    return;

                var _detalleHistoriaClinicaDental = new cHistoriaClinicaDental().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.INGRESO.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                LstOdontogramaInicial = new ObservableCollection<ODONTOGRAMA_INICIAL>();
                LstSeguimientoDental = new ObservableCollection<ODONTOGRAMA_SEGUIMIENTO2>();
                var _detalleHistoriaClinica = new cHistoriaClinica().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_CENTRO == SelectIngreso.ID_CENTRO).FirstOrDefault();

                LstPatosDental = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>();
                LstCondensadoPatosDental = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>();
                LstDocumentosDentales = new ObservableCollection<HISTORIA_CLINICA_DENTAL_DOCUME>();

                #region HISTORIA CLINICA
                if (_detalleHistoriaClinica != null)
                {
                    #region FAMILIARES
                    var _familiares = _detalleHistoriaClinica.HISTORIA_CLINICA_FAMILIAR;
                    if (_familiares != null)
                    {
                        foreach (var item in _familiares)
                        {
                            #region PADRE
                            if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.PADRE)
                            {
                                CheckPadreViveDental = item.AHF_VIVE ?? string.Empty;
                                TextEdadPadreDental = item.AHF_EDAD.HasValue ? item.AHF_EDAD.Value : new short();
                                CheckPadrePadeceDental = item.AHF_SANO ?? string.Empty;
                                IsCheckedDiabPadreDental = item.AHF_DIABETES ?? string.Empty;
                                IsCheckedTBPadreDental = item.AHF_TB ?? string.Empty;
                                IsCheckedCAPadreDental = item.AHF_CA ?? string.Empty;
                                IsCheckedCardiPadreDental = item.AHF_CARDIACOS ?? string.Empty;
                                IsCheckedEpiPadreDental = item.AHF_EPILEPSIA ?? string.Empty;
                                IsCheckedMentPadreDental = item.AHF_MENTALES ?? string.Empty;
                                IsCheckedAlergiPadreDental = item.AHF_ALERGIAS ?? string.Empty;
                                IsCheckedHipertPadreDental = item.AHF_HIPERTENSIVO ?? string.Empty;
                                CuandoMuertePadreDental = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                CausaMuertePadreDental = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                continue;
                            };
                            #endregion
                            #region MADRE
                            if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.MADRE)
                            {
                                CheckMadreViveDental = item.AHF_VIVE ?? string.Empty;
                                TextEdadMadreDental = item.AHF_EDAD.HasValue ? item.AHF_EDAD.Value : new short();
                                CheckMadrePadeceDental = item.AHF_SANO ?? string.Empty;
                                IsCheckedDiabMadreDental = item.AHF_DIABETES ?? string.Empty;
                                IsCheckedTBMadreDental = item.AHF_TB ?? string.Empty;
                                IsCheckedCAMadreDental = item.AHF_CA ?? string.Empty;
                                IsCheckedCardiMadreDental = item.AHF_CARDIACOS ?? string.Empty;
                                IsCheckedEpiMadreDental = item.AHF_EPILEPSIA ?? string.Empty;
                                IsCheckedMentMadreDental = item.AHF_MENTALES ?? string.Empty;
                                IsCheckedAlergiMadreDental = item.AHF_ALERGIAS ?? string.Empty;
                                IsCheckedHipertMadreDental = item.AHF_HIPERTENSIVO ?? string.Empty;
                                CuandoMuerteMadreDental = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                CausaMuerteMadreDental = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                continue;
                            };
                            #endregion
                            #region HERMANOS
                            if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.HERMANOS)
                            {
                                CheckHermanosVivosDental = item.AHF_VIVE ?? string.Empty;
                                TextHermanosMujeresDental = _detalleHistoriaClinica.AHF_HERMANOS_F.HasValue ? _detalleHistoriaClinica.AHF_HERMANOS_F.Value : new short();
                                TextHermanosHombresDental = _detalleHistoriaClinica.AHF_HERMANOS_M.HasValue ? _detalleHistoriaClinica.AHF_HERMANOS_M.Value : new short();
                                CheckHermanosSanosDental = item.AHF_SANO ?? string.Empty;
                                IsCheckedDiabHnosDental = item.AHF_DIABETES ?? string.Empty;
                                IsCheckedTBHnosDental = item.AHF_TB ?? string.Empty;
                                IsCheckedCAHnosDental = item.AHF_CA ?? string.Empty;
                                IsCheckedCardiHnosDental = item.AHF_CARDIACOS ?? string.Empty;
                                IsCheckedEpiHnosDental = item.AHF_EPILEPSIA ?? string.Empty;
                                IsCheckedMentHnosDental = item.AHF_MENTALES ?? string.Empty;
                                IsCheckedAlergiHnosDental = item.AHF_ALERGIAS ?? string.Empty;
                                IsCheckedHipertHnosDental = item.AHF_HIPERTENSIVO ?? string.Empty;
                                CuandoMuerteHnosDental = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                CausaMuerteHnosDental = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                continue;
                            };
                            #endregion
                            #region CONYUGE
                            if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.CONYUGE)
                            {
                                CheckConyugeViveDental = item.AHF_VIVE ?? string.Empty;
                                TextEdadConyugeDental = item.AHF_EDAD.HasValue ? item.AHF_EDAD.Value : new short();
                                CheckConyugePadeceDental = item.AHF_SANO ?? string.Empty;
                                IsCheckedDiabConyDental = item.AHF_DIABETES ?? string.Empty;
                                IsCheckedTBConyDental = item.AHF_TB ?? string.Empty;
                                IsCheckedCAConyDental = item.AHF_CA ?? string.Empty;
                                IsCheckedCardiConyDental = item.AHF_CARDIACOS ?? string.Empty;
                                IsCheckedEpiConyDental = item.AHF_EPILEPSIA ?? string.Empty;
                                IsCheckedMentConyDental = item.AHF_MENTALES ?? string.Empty;
                                IsCheckedAlergiConyDental = item.AHF_ALERGIAS ?? string.Empty;
                                IsCheckedHipertConyDental = item.AHF_HIPERTENSIVO ?? string.Empty;
                                CuandoMuerteConyDental = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                CausaMuerteConyDental = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                continue;
                            };
                            #endregion
                            #region HIJOS
                            if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.HIJOS)
                            {
                                CheckHijosViveDental = item.AHF_VIVE ?? string.Empty;
                                TextEdadesHijosDental = item.AHF_EDAD.HasValue ? item.AHF_EDAD.Value : new short();
                                IsCheckedDiabHijosDental = item.AHF_DIABETES ?? string.Empty;
                                IsCheckedTBHijosDental = item.AHF_TB ?? string.Empty;
                                IsCheckedCAHijosDental = item.AHF_CA ?? string.Empty;
                                IsCheckedCardiHijosDental = item.AHF_CARDIACOS ?? string.Empty;
                                IsCheckedEpiHijosDental = item.AHF_EPILEPSIA ?? string.Empty;
                                IsCheckedMentHijosDental = item.AHF_MENTALES ?? string.Empty;
                                IsCheckedAlergiHijosDental = item.AHF_ALERGIAS ?? string.Empty;
                                IsCheckedHipertHijosDental = item.AHF_HIPERTENSIVO ?? string.Empty;
                                CuandoMuerteHijosDental = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                CausaMuerteHijosDental = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                CheckHijosPadeceDental = item.AHF_SANO ?? string.Empty;
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        CheckPadreViveDental = CheckPadrePadeceDental = IsCheckedDiabPadreDental = IsCheckedTBPadreDental = IsCheckedCAPadreDental = IsCheckedCardiPadreDental = IsCheckedEpiPadreDental = IsCheckedMentPadreDental = IsCheckedAlergiPadreDental = IsCheckedHipertPadreDental = CausaMuertePadreDental = CheckMadreViveDental = CheckMadrePadeceDental = IsCheckedDiabMadreDental = IsCheckedTBMadreDental = IsCheckedCAMadreDental = IsCheckedCardiMadreDental = IsCheckedEpiMadreDental = IsCheckedMentMadreDental = IsCheckedAlergiMadreDental = IsCheckedHipertMadreDental = CausaMuerteMadreDental = CheckHermanosVivosDental = CheckHermanosSanosDental = IsCheckedDiabHnosDental = IsCheckedTBHnosDental = IsCheckedCAHnosDental = IsCheckedCardiHnosDental = IsCheckedEpiHnosDental = IsCheckedMentHnosDental = IsCheckedAlergiHnosDental = IsCheckedHipertHnosDental = CausaMuerteHnosDental = CheckConyugeViveDental = CheckConyugePadeceDental = IsCheckedDiabConyDental = IsCheckedTBConyDental = IsCheckedCAConyDental = IsCheckedCardiConyDental = IsCheckedEpiConyDental = IsCheckedMentConyDental = IsCheckedAlergiConyDental = IsCheckedHipertConyDental = CausaMuerteConyDental = CheckHijosViveDental = IsCheckedDiabHijosDental = IsCheckedTBHijosDental = IsCheckedCAHijosDental = IsCheckedCardiHijosDental = IsCheckedEpiHijosDental = IsCheckedMentHijosDental = IsCheckedAlergiHijosDental = IsCheckedHipertHijosDental = CausaMuerteHijosDental = string.Empty;
                        CuandoMuerteHijosDental = CuandoMuerteConyDental = CuandoMuerteHnosDental = CuandoMuerteMadreDental = CuandoMuertePadreDental = new DateTime?();
                        TextEdadesHijosDental = TextEdadConyugeDental = TextHermanosMujeresDental = TextHermanosHombresDental = TextEdadMadreDental = TextEdadPadreDental = new short();
                    }

                    #endregion

                    TomandoAlgunMedicamento = !string.IsNullOrEmpty(_detalleHistoriaClinica.APP_MEDICAMENTOS_ACTIVOS) ? "S" : "N";
                    CualMedicamentoTomando = !string.IsNullOrEmpty(_detalleHistoriaClinica.APP_MEDICAMENTOS_ACTIVOS) ? _detalleHistoriaClinica.APP_MEDICAMENTOS_ACTIVOS : string.Empty;

                    AlcohlisDental = _detalleHistoriaClinica.APNP_ALCOHOLISMO == "S" ? "S" : "N";
                    TabaqDental = _detalleHistoriaClinica.APNP_TABAQUISMO == "S" ? "S" : "N";
                    ToxicomaniasDental = _detalleHistoriaClinica.APNP_TOXICOMANIAS == "S" ? "S" : "N";

                    #region MUJERES
                    var detalleGineco = _detalleHistoriaClinica.HISTORIA_CLINICA_GINECO_OBSTRE;
                    if (detalleGineco.Any())
                    {
                        var _Ultimo = detalleGineco.OrderByDescending(x => x.ID_GINECO).FirstOrDefault();
                        EstaEmbarazadaDental = _Ultimo.CONTROL_PRENATAL == "S" ? "S" : "N";
                    }
                    else
                        EstaEmbarazadaDental = "N";

                    if (SelectIngreso != null)
                        if (SelectIngreso.IMPUTADO != null)
                            if (SelectIngreso.IMPUTADO.SEXO == "F")
                            {
                                IsEnabledCamposMujeres = true;
                                ValidacionesCamposMujeresDental();
                            }
                            else
                            {
                                LimpiaValidacionesCamposMujeres();
                                IsEnabledCamposMujeres = false;
                            };

                    #endregion

                    var _patologicosRegistrados = _detalleHistoriaClinica.HISTORIA_CLINICA_PATOLOGICOS;
                    if (_patologicosRegistrados != null && _patologicosRegistrados.Any())
                        foreach (var item in _patologicosRegistrados)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                LstCondensadoPatosDental.Add(new HISTORIA_CLINICA_PATOLOGICOS
                                {
                                    FUENTE = item.FUENTE,
                                    ID_ANIO = item.ID_ANIO,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_CONSEC = item.ID_CONSEC,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_NOPATOLOGICO = item.ID_NOPATOLOGICO,
                                    ID_PATOLOGICO = item.ID_PATOLOGICO,
                                    MOMENTO_DETECCION = item.MOMENTO_DETECCION,
                                    OBSERVACIONES = item.OBSERVACIONES,
                                    REGISTRO_FEC = item.REGISTRO_FEC,
                                    PATOLOGICO_CAT = item.PATOLOGICO_CAT,
                                    RECUPERADO = !string.IsNullOrEmpty(item.RECUPERADO) ? item.RECUPERADO == "S" ? "True" : "False" : "False"
                                });
                            }));
                        };
                }
                else
                {
                    CheckPadreViveDental = CheckPadrePadeceDental = IsCheckedDiabPadreDental = IsCheckedTBPadreDental = IsCheckedCAPadreDental = IsCheckedCardiPadreDental = IsCheckedEpiPadreDental = IsCheckedMentPadreDental = IsCheckedAlergiPadreDental = IsCheckedHipertPadreDental = CausaMuertePadreDental = CheckMadreViveDental = CheckMadrePadeceDental = IsCheckedDiabMadreDental = IsCheckedTBMadreDental = IsCheckedCAMadreDental = IsCheckedCardiMadreDental = IsCheckedEpiMadreDental = IsCheckedMentMadreDental = IsCheckedAlergiMadreDental = IsCheckedHipertMadreDental = CausaMuerteMadreDental = CheckHermanosVivosDental = CheckHermanosSanosDental = IsCheckedDiabHnosDental = IsCheckedTBHnosDental = IsCheckedCAHnosDental = IsCheckedCardiHnosDental = IsCheckedEpiHnosDental = IsCheckedMentHnosDental = IsCheckedAlergiHnosDental = IsCheckedHipertHnosDental = CausaMuerteHnosDental = CheckConyugeViveDental = CheckConyugePadeceDental = IsCheckedDiabConyDental = IsCheckedTBConyDental = IsCheckedCAConyDental = IsCheckedCardiConyDental = IsCheckedEpiConyDental = IsCheckedMentConyDental = IsCheckedAlergiConyDental = IsCheckedHipertConyDental = CausaMuerteConyDental = CheckHijosViveDental = IsCheckedDiabHijosDental = IsCheckedTBHijosDental = IsCheckedCAHijosDental = IsCheckedCardiHijosDental = IsCheckedEpiHijosDental = IsCheckedMentHijosDental = IsCheckedAlergiHijosDental = IsCheckedHipertHijosDental = CausaMuerteHijosDental = string.Empty;
                    CuandoMuerteHijosDental = CuandoMuerteConyDental = CuandoMuerteHnosDental = CuandoMuerteMadreDental = CuandoMuertePadreDental = new DateTime?();
                    TextEdadesHijosDental = TextEdadConyugeDental = TextHermanosMujeresDental = TextHermanosHombresDental = TextEdadMadreDental = TextEdadPadreDental = new short();
                    TomandoAlgunMedicamento = CualMedicamentoTomando = AlcohlisDental = TabaqDental = ToxicomaniasDental = EstaEmbarazadaDental = AmenazaAbortoDental = FluoroDental = LactandoDental = AlergicoAlgunMedicamento = CualMedicamentoAlergico = string.Empty;
                }

                #endregion

                if (_detalleHistoriaClinicaDental != null)
                {
                    CualMedicamentoAlergico = _detalleHistoriaClinicaDental.ALERGICO_MEDICAMENTO_CUAL;
                    EspecifiqueLabiosDental = _detalleHistoriaClinicaDental.EXP_BUC_LABIOS;
                    EspecifiqueLenguaDental = _detalleHistoriaClinicaDental.EXP_BUC_LENGUA;
                    EspecifiqueMucosaDental = _detalleHistoriaClinicaDental.EXP_BUC_MUCOSA_NASAL;
                    EspecifiqueAmigdalasDental = _detalleHistoriaClinicaDental.EXP_BUC_AMIGDALAS;
                    EspecifiquePisoBocaDental = _detalleHistoriaClinicaDental.EXP_BUC_PISO_BOCA;
                    EspecifiquePaladarDuroDental = _detalleHistoriaClinicaDental.EXP_BUC_PALADAR_DURO;
                    EspecifiquePaladarBlancoDental = _detalleHistoriaClinicaDental.EXP_BUC_PALADAR_BLANCO;
                    EspecifiqueCarrillosDental = _detalleHistoriaClinicaDental.EXP_BUC_CARRILLOS;
                    EspecifiqueFrenillosDental = _detalleHistoriaClinicaDental.EXP_BUC_FRENILLOS;
                    EspecifiqueOtrosDental = _detalleHistoriaClinicaDental.EXP_BUC_OTROS;
                    ComplicacionesDespuesTratamDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.COMPLICACIONES) ? _detalleHistoriaClinicaDental.COMPLICACIONES : string.Empty;
                    HemorragiaDespuesExtracDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.HEMORRAGIA) ? _detalleHistoriaClinicaDental.HEMORRAGIA : string.Empty;
                    TenidoReaccionNegativaDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.REACCION_ANESTESICO) ? _detalleHistoriaClinicaDental.REACCION_ANESTESICO : string.Empty;
                    AlergicoAlgunMedicamento = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.ALERGICO_MEDICAMENTO) ? _detalleHistoriaClinicaDental.ALERGICO_MEDICAMENTO : string.Empty;
                    AmenazaAbortoDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.AMENAZA_ABORTO) ? _detalleHistoriaClinicaDental.AMENAZA_ABORTO : string.Empty;
                    LactandoDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.LACTANDO) ? _detalleHistoriaClinicaDental.LACTANDO : string.Empty;
                    CariesDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.DIENTES_CARIES) ? _detalleHistoriaClinicaDental.DIENTES_CARIES : string.Empty;
                    FluoroDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.DIENTES_FLUOROSIS) ? _detalleHistoriaClinicaDental.DIENTES_FLUOROSIS : string.Empty;
                    AnomForma = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.DIENTES_ANOM_FORMA) ? _detalleHistoriaClinicaDental.DIENTES_ANOM_FORMA : string.Empty;
                    EspecifiqueAnomaliadForma = _detalleHistoriaClinicaDental.DIENTES_ANOM_FORMA_OBS;
                    AnomTamanio = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.DIENTES_ANOM_TAMANO) ? _detalleHistoriaClinicaDental.DIENTES_ANOM_TAMANO : string.Empty;
                    EspecifiqueAnomaliadTamanio = _detalleHistoriaClinicaDental.DIENTES_ANOM_TAMANO_OBS;
                    HipoPlastDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.DIENTES_HIPOPLASIA) ? _detalleHistoriaClinicaDental.DIENTES_HIPOPLASIA : string.Empty;
                    EspecifiqueHipoplas = _detalleHistoriaClinicaDental.DIENTES_HIPOPLASIA_OBS;
                    OtrosHipoDental = _detalleHistoriaClinicaDental.DIENTES_OTROS;
                    DolorDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.ART_TEMP_DOLOR) ? _detalleHistoriaClinicaDental.ART_TEMP_DOLOR : string.Empty;
                    ObservacionesCansancioDental = _detalleHistoriaClinicaDental.ART_TEMP_CANSANCIO_OBS;
                    CansancioDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.ART_TEMP_CANSANCIO) ? _detalleHistoriaClinicaDental.ART_TEMP_CANSANCIO : string.Empty;
                    ObservacionesChasquidosDental = _detalleHistoriaClinicaDental.ART_TEMP_CHASQUIDOS_OBS;
                    ChasidosDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.ART_TEMP_CHASQUIDOS) ? _detalleHistoriaClinicaDental.ART_TEMP_CHASQUIDOS : string.Empty;
                    ObservacionesRigidezDental = _detalleHistoriaClinicaDental.ART_TEMP_RIGIDEZ_OBS;
                    RigidezDental = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.ART_TEMP_RIGIDEZ) ? _detalleHistoriaClinicaDental.ART_TEMP_RIGIDEZ : string.Empty;
                    ObservacionesDolorDental = _detalleHistoriaClinicaDental.ART_TEMP_DOLOR_OBS;
                    EnciasColorDental = _detalleHistoriaClinicaDental.ENCIAS_COLORACION;
                    EnciasFormaDental = _detalleHistoriaClinicaDental.ENCIAS_FORMA;
                    EnciasTexturaDental = _detalleHistoriaClinicaDental.ENCIAS_TEXTURA;
                    BruxismoEstatus = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.BRUXISMO) ? _detalleHistoriaClinicaDental.BRUXISMO : string.Empty;
                    AfirmativoBruxismo = !string.IsNullOrEmpty(_detalleHistoriaClinicaDental.BRUXISMO_DOLOR) ? _detalleHistoriaClinicaDental.BRUXISMO_DOLOR : string.Empty;
                    PesoSignosVitalesDental = _detalleHistoriaClinicaDental.PESO;
                    GlicemiaSignosVitalesDental = _detalleHistoriaClinicaDental.GLICEMIA;
                    EstaturaSignosVitalesDental = _detalleHistoriaClinicaDental.ESTATURA;
                    FrecuenciaRespSignosVitalesDental = _detalleHistoriaClinicaDental.FRECUENCIA_RESPIRA;
                    FrecuenciaCardSignosVitalesDental = _detalleHistoriaClinicaDental.FRECUENCIA_CARDIAC;
                    TemperaturaSignosVitalesDental = _detalleHistoriaClinicaDental.TEMPERATURA;
                    if (!string.IsNullOrEmpty(_detalleHistoriaClinicaDental.TENSION_ARTERIAL) ? _detalleHistoriaClinicaDental.TENSION_ARTERIAL.Trim().Contains("/") : false)
                    {
                        string dato1 = _detalleHistoriaClinicaDental.TENSION_ARTERIAL.Split('/')[0];
                        string dato2 = _detalleHistoriaClinicaDental.TENSION_ARTERIAL.Split('/')[1];
                        Arterial1 = !string.IsNullOrEmpty(dato1) ? dato1.Trim() : string.Empty;
                        Arterial2 = !string.IsNullOrEmpty(dato2) ? dato2.Trim() : string.Empty;
                    };

                    if (_detalleHistoriaClinicaDental.ODONTOGRAMA_INICIAL.Any())
                        foreach (var item in _detalleHistoriaClinicaDental.ODONTOGRAMA_INICIAL)
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                LstOdontogramaInicial.Add(new ODONTOGRAMA_INICIAL
                                {
                                    ID_ANIO = item.ID_ANIO,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_CONSEC = item.ID_CONSEC,
                                    ID_CONSECUTIVO = item.ID_CONSECUTIVO,
                                    ID_ENFERMEDAD = item.ID_ENFERMEDAD.HasValue ? item.ID_ENFERMEDAD != -1 ? item.ID_ENFERMEDAD : -1 : -1,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_NOMENCLATURA = item.ID_NOMENCLATURA.HasValue ? item.ID_NOMENCLATURA != -1 ? item.ID_NOMENCLATURA : -1 : -1,
                                    ID_POSICION = item.ID_POSICION,
                                    ID_TIPO_ODO = item.ID_TIPO_ODO,
                                    REGISTRO_FEC = item.REGISTRO_FEC
                                });
                            }));



                    if (_detalleHistoriaClinicaDental.HISTORIA_CLINICA_DENTAL_DOCUME.Any())
                        foreach (var item in _detalleHistoriaClinicaDental.HISTORIA_CLINICA_DENTAL_DOCUME)
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                LstDocumentosDentales.Add(new HISTORIA_CLINICA_DENTAL_DOCUME
                                    {
                                        DOCUMENTO = item.DOCUMENTO,
                                        ID_ANIO = item.ID_ANIO,
                                        ID_CENTRO = item.ID_CENTRO,
                                        ID_CONSEC = item.ID_CONSEC,
                                        ID_DOCTO = item.ID_DOCTO,
                                        ID_HCDDOCTO = item.ID_HCDDOCTO,
                                        ID_IMPUTADO = item.ID_IMPUTADO,
                                        ID_INGRESO = item.ID_INGRESO
                                    });
                            }));

                    if (_detalleHistoriaClinicaDental.ESTATUS == "T")
                        IsEnabledHistoriaClinicaDental = IsEnabledBotonesProcesoOdontogramas = MenuGuardarEnabled = SubirImagenesDental = false;//deshabilita la posiblidad de guardar de nuevo

                    else
                        if (string.IsNullOrEmpty(_detalleHistoriaClinicaDental.PESO) || string.IsNullOrEmpty(_detalleHistoriaClinicaDental.TENSION_ARTERIAL) || string.IsNullOrEmpty(_detalleHistoriaClinicaDental.ESTATURA) ||
                            string.IsNullOrEmpty(_detalleHistoriaClinicaDental.FRECUENCIA_CARDIAC) || string.IsNullOrEmpty(_detalleHistoriaClinicaDental.FRECUENCIA_RESPIRA) || string.IsNullOrEmpty(_detalleHistoriaClinicaDental.GLICEMIA)
                            || string.IsNullOrEmpty(_detalleHistoriaClinicaDental.TEMPERATURA))
                            IsEnabledSignosVitalesDentales = true;//al menos un signo vital esta vacio, permitele capturar de nuevo
                        else
                            IsEnabledSignosVitalesDentales = false;
                }
                else
                {
                    var _validaHistoriaDentalAnterior = new cHistoriaClinicaDental().GetData(x => x.ID_INGRESO != SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && (x.ESTATUS == "I" || x.ESTATUS == "T")).FirstOrDefault();

                    if (_validaHistoriaDentalAnterior != null)
                    {
                        CualMedicamentoAlergico = _validaHistoriaDentalAnterior.ALERGICO_MEDICAMENTO_CUAL;
                        EspecifiqueLabiosDental = _validaHistoriaDentalAnterior.EXP_BUC_LABIOS;
                        EspecifiqueLenguaDental = _validaHistoriaDentalAnterior.EXP_BUC_LENGUA;
                        EspecifiqueMucosaDental = _validaHistoriaDentalAnterior.EXP_BUC_MUCOSA_NASAL;
                        EspecifiqueAmigdalasDental = _validaHistoriaDentalAnterior.EXP_BUC_AMIGDALAS;
                        EspecifiquePisoBocaDental = _validaHistoriaDentalAnterior.EXP_BUC_PISO_BOCA;
                        EspecifiquePaladarDuroDental = _validaHistoriaDentalAnterior.EXP_BUC_PALADAR_DURO;
                        EspecifiquePaladarBlancoDental = _validaHistoriaDentalAnterior.EXP_BUC_PALADAR_BLANCO;
                        EspecifiqueCarrillosDental = _validaHistoriaDentalAnterior.EXP_BUC_CARRILLOS;
                        EspecifiqueFrenillosDental = _validaHistoriaDentalAnterior.EXP_BUC_FRENILLOS;
                        EspecifiqueOtrosDental = _validaHistoriaDentalAnterior.EXP_BUC_OTROS;
                        ComplicacionesDespuesTratamDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.COMPLICACIONES) ? _validaHistoriaDentalAnterior.COMPLICACIONES : string.Empty;
                        HemorragiaDespuesExtracDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.HEMORRAGIA) ? _validaHistoriaDentalAnterior.HEMORRAGIA : string.Empty;
                        TenidoReaccionNegativaDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.REACCION_ANESTESICO) ? _validaHistoriaDentalAnterior.REACCION_ANESTESICO : string.Empty;
                        AlergicoAlgunMedicamento = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.ALERGICO_MEDICAMENTO) ? _validaHistoriaDentalAnterior.ALERGICO_MEDICAMENTO : string.Empty;
                        AmenazaAbortoDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.AMENAZA_ABORTO) ? _validaHistoriaDentalAnterior.AMENAZA_ABORTO : string.Empty;
                        LactandoDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.LACTANDO) ? _validaHistoriaDentalAnterior.LACTANDO : string.Empty;
                        CariesDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.DIENTES_CARIES) ? _validaHistoriaDentalAnterior.DIENTES_CARIES : string.Empty;
                        FluoroDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.DIENTES_FLUOROSIS) ? _validaHistoriaDentalAnterior.DIENTES_FLUOROSIS : string.Empty;
                        AnomForma = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.DIENTES_ANOM_FORMA) ? _validaHistoriaDentalAnterior.DIENTES_ANOM_FORMA : string.Empty;
                        EspecifiqueAnomaliadForma = _validaHistoriaDentalAnterior.DIENTES_ANOM_FORMA_OBS;
                        AnomTamanio = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.DIENTES_ANOM_TAMANO) ? _validaHistoriaDentalAnterior.DIENTES_ANOM_TAMANO : string.Empty;
                        EspecifiqueAnomaliadTamanio = _validaHistoriaDentalAnterior.DIENTES_ANOM_TAMANO_OBS;
                        HipoPlastDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.DIENTES_HIPOPLASIA) ? _validaHistoriaDentalAnterior.DIENTES_HIPOPLASIA : string.Empty;
                        EspecifiqueHipoplas = _validaHistoriaDentalAnterior.DIENTES_HIPOPLASIA_OBS;
                        OtrosHipoDental = _validaHistoriaDentalAnterior.DIENTES_OTROS;
                        DolorDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.ART_TEMP_DOLOR) ? _validaHistoriaDentalAnterior.ART_TEMP_DOLOR : string.Empty;
                        ObservacionesCansancioDental = _validaHistoriaDentalAnterior.ART_TEMP_CANSANCIO_OBS;
                        CansancioDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.ART_TEMP_CANSANCIO) ? _validaHistoriaDentalAnterior.ART_TEMP_CANSANCIO : string.Empty;
                        ObservacionesChasquidosDental = _validaHistoriaDentalAnterior.ART_TEMP_CHASQUIDOS_OBS;
                        ChasidosDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.ART_TEMP_CHASQUIDOS) ? _validaHistoriaDentalAnterior.ART_TEMP_CHASQUIDOS : string.Empty;
                        ObservacionesRigidezDental = _validaHistoriaDentalAnterior.ART_TEMP_RIGIDEZ_OBS;
                        RigidezDental = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.ART_TEMP_RIGIDEZ) ? _validaHistoriaDentalAnterior.ART_TEMP_RIGIDEZ : string.Empty;
                        ObservacionesDolorDental = _validaHistoriaDentalAnterior.ART_TEMP_DOLOR_OBS;
                        EnciasColorDental = _validaHistoriaDentalAnterior.ENCIAS_COLORACION;
                        EnciasFormaDental = _validaHistoriaDentalAnterior.ENCIAS_FORMA;
                        EnciasTexturaDental = _validaHistoriaDentalAnterior.ENCIAS_TEXTURA;
                        BruxismoEstatus = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.BRUXISMO) ? _validaHistoriaDentalAnterior.BRUXISMO : string.Empty;
                        AfirmativoBruxismo = !string.IsNullOrEmpty(_validaHistoriaDentalAnterior.BRUXISMO_DOLOR) ? _validaHistoriaDentalAnterior.BRUXISMO_DOLOR : string.Empty;
                        PesoSignosVitalesDental = _validaHistoriaDentalAnterior.PESO;
                        GlicemiaSignosVitalesDental = _validaHistoriaDentalAnterior.GLICEMIA;
                        EstaturaSignosVitalesDental = _validaHistoriaDentalAnterior.ESTATURA;
                        FrecuenciaRespSignosVitalesDental = _validaHistoriaDentalAnterior.FRECUENCIA_RESPIRA;
                        FrecuenciaCardSignosVitalesDental = _validaHistoriaDentalAnterior.FRECUENCIA_CARDIAC;
                        TemperaturaSignosVitalesDental = _validaHistoriaDentalAnterior.TEMPERATURA;
                        if (!string.IsNullOrEmpty(_validaHistoriaDentalAnterior.TENSION_ARTERIAL) ? _validaHistoriaDentalAnterior.TENSION_ARTERIAL.Trim().Contains("/") : false)
                        {
                            string dato1 = _validaHistoriaDentalAnterior.TENSION_ARTERIAL.Split('/')[0];
                            string dato2 = _validaHistoriaDentalAnterior.TENSION_ARTERIAL.Split('/')[1];
                            Arterial1 = !string.IsNullOrEmpty(dato1) ? dato1.Trim() : string.Empty;
                            Arterial2 = !string.IsNullOrEmpty(dato2) ? dato2.Trim() : string.Empty;
                        };

                        if (_validaHistoriaDentalAnterior.ODONTOGRAMA_INICIAL.Any())
                            foreach (var item in _validaHistoriaDentalAnterior.ODONTOGRAMA_INICIAL)
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    LstOdontogramaInicial.Add(new ODONTOGRAMA_INICIAL
                                    {
                                        ID_ANIO = item.ID_ANIO,
                                        ID_CENTRO = item.ID_CENTRO,
                                        ID_CONSEC = item.ID_CONSEC,
                                        ID_CONSECUTIVO = item.ID_CONSECUTIVO,
                                        ID_ENFERMEDAD = item.ID_ENFERMEDAD.HasValue ? item.ID_ENFERMEDAD != -1 ? item.ID_ENFERMEDAD : -1 : -1,
                                        ID_IMPUTADO = item.ID_IMPUTADO,
                                        ID_INGRESO = item.ID_INGRESO,
                                        ID_NOMENCLATURA = item.ID_NOMENCLATURA.HasValue ? item.ID_NOMENCLATURA != -1 ? item.ID_NOMENCLATURA : -1 : -1,
                                        ID_POSICION = item.ID_POSICION,
                                        ID_TIPO_ODO = item.ID_TIPO_ODO,
                                        REGISTRO_FEC = item.REGISTRO_FEC
                                    });
                                }));

                        if (_validaHistoriaDentalAnterior.HISTORIA_CLINICA_DENTAL_DOCUME.Any())
                            foreach (var item in _validaHistoriaDentalAnterior.HISTORIA_CLINICA_DENTAL_DOCUME)
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    LstDocumentosDentales.Add(new HISTORIA_CLINICA_DENTAL_DOCUME
                                    {
                                        DOCUMENTO = item.DOCUMENTO,
                                        ID_ANIO = item.ID_ANIO,
                                        ID_CENTRO = item.ID_CENTRO,
                                        ID_CONSEC = item.ID_CONSEC,
                                        ID_DOCTO = item.ID_DOCTO,
                                        ID_HCDDOCTO = item.ID_HCDDOCTO,
                                        ID_IMPUTADO = item.ID_IMPUTADO,
                                        ID_INGRESO = item.ID_INGRESO
                                    });
                                }));

                        IsEnabledHistoriaClinicaDental = IsEnabledBotonesProcesoOdontogramas = MenuGuardarEnabled = SubirImagenesDental = true;//deshabilita la posiblidad de guardar de nuevo

                        if (string.IsNullOrEmpty(_validaHistoriaDentalAnterior.PESO) || string.IsNullOrEmpty(_validaHistoriaDentalAnterior.TENSION_ARTERIAL) || string.IsNullOrEmpty(_validaHistoriaDentalAnterior.ESTATURA) ||
                            string.IsNullOrEmpty(_validaHistoriaDentalAnterior.FRECUENCIA_CARDIAC) || string.IsNullOrEmpty(_validaHistoriaDentalAnterior.FRECUENCIA_RESPIRA) || string.IsNullOrEmpty(_validaHistoriaDentalAnterior.GLICEMIA)
                            || string.IsNullOrEmpty(_validaHistoriaDentalAnterior.TEMPERATURA))
                            IsEnabledSignosVitalesDentales = true;//al menos un signo vital esta vacio, permitele capturar de nuevo
                        else
                            IsEnabledSignosVitalesDentales = false;

                        return;
                    }
                    else
                    {
                        ComplicacionesDespuesTratamDental = HemorragiaDespuesExtracDental = TenidoReaccionNegativaDental = CariesDental = FluoroDental = AnomForma = EspecifiqueAnomaliadForma = AnomTamanio = EspecifiqueAnomaliadTamanio = HipoPlastDental = EspecifiqueHipoplas = OtrosHipoDental = DolorDental = ObservacionesCansancioDental = CansancioDental = ObservacionesChasquidosDental = ChasidosDental = ObservacionesRigidezDental = RigidezDental = ObservacionesDolorDental = EnciasColorDental = EnciasFormaDental = EnciasTexturaDental = BruxismoEstatus = AfirmativoBruxismo = PesoSignosVitalesDental = GlicemiaSignosVitalesDental = FrecuenciaRespSignosVitalesDental = FrecuenciaCardSignosVitalesDental = TemperaturaSignosVitalesDental = AmenazaAbortoDental = AlergicoAlgunMedicamento = EstaturaSignosVitalesDental = LactandoDental = Arterial1 = Arterial2 = string.Empty;
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private bool GuardarHistoriaClinicaDental()
        {
            try
            {
                var usr = new cUsuario().ObtenerTodos(GlobalVar.gUsr);

                var _hoy = Fechas.GetFechaDateServer;
                var user = usr.FirstOrDefault();
                var _HistoriaClinicaDental = new HISTORIA_CLINICA_DENTAL()
                {
                    ALERGICO_MEDICAMENTO = AlergicoAlgunMedicamento,
                    ALERGICO_MEDICAMENTO_CUAL = CualMedicamentoAlergico,
                    AMENAZA_ABORTO = AmenazaAbortoDental,
                    LACTANDO = LactandoDental,
                    ART_TEMP_DOLOR = DolorDental,
                    ART_TEMP_DOLOR_OBS = ObservacionesDolorDental,
                    ART_TEMP_RIGIDEZ = RigidezDental,
                    ART_TEMP_RIGIDEZ_OBS = ObservacionesRigidezDental,
                    ART_TEMP_CHASQUIDOS = ChasidosDental,
                    ART_TEMP_CHASQUIDOS_OBS = ObservacionesChasquidosDental,
                    ART_TEMP_CANSANCIO = CansancioDental,
                    ART_TEMP_CANSANCIO_OBS = ObservacionesCansancioDental,
                    BRUXISMO = BruxismoEstatus,
                    BRUXISMO_DOLOR = AfirmativoBruxismo,
                    DIENTES_CARIES = CariesDental,
                    DIENTES_FLUOROSIS = FluoroDental,
                    DIENTES_ANOM_FORMA = AnomForma,
                    DIENTES_ANOM_FORMA_OBS = EspecifiqueAnomaliadForma,
                    DIENTES_ANOM_TAMANO = AnomTamanio,
                    DIENTES_ANOM_TAMANO_OBS = EspecifiqueAnomaliadTamanio,
                    DIENTES_HIPOPLASIA = HipoPlastDental,
                    DIENTES_HIPOPLASIA_OBS = EspecifiqueHipoplas,
                    DIENTES_OTROS = OtrosHipoDental,
                    ENCIAS_COLORACION = EnciasColorDental,
                    ENCIAS_FORMA = EnciasFormaDental,
                    ENCIAS_TEXTURA = EnciasTexturaDental,
                    EXP_BUC_LABIOS = EspecifiqueLabiosDental,
                    EXP_BUC_LENGUA = EspecifiqueLenguaDental,
                    USUARIO_ENFERMERA = string.Empty,
                    ESTATUS = "T",
                    EXP_BUC_MUCOSA_NASAL = EspecifiqueMucosaDental,
                    EXP_BUC_AMIGDALAS = EspecifiqueAmigdalasDental,
                    EXP_BUC_PISO_BOCA = EspecifiquePisoBocaDental,
                    EXP_BUC_PALADAR_DURO = EspecifiquePaladarDuroDental,
                    EXP_BUC_PALADAR_BLANCO = EspecifiquePaladarBlancoDental,
                    EXP_BUC_CARRILLOS = EspecifiqueCarrillosDental,
                    EXP_BUC_FRENILLOS = EspecifiqueFrenillosDental,
                    EXP_BUC_OTROS = EspecifiqueOtrosDental,
                    TEMPERATURA = TemperaturaSignosVitalesDental,
                    TENSION_ARTERIAL = string.Format("{0} / {1}", Arterial1, Arterial2),
                    FRECUENCIA_CARDIAC = FrecuenciaCardSignosVitalesDental,
                    FRECUENCIA_RESPIRA = FrecuenciaRespSignosVitalesDental,
                    GLICEMIA = GlicemiaSignosVitalesDental,
                    PESO = PesoSignosVitalesDental,
                    ESTATURA = EstaturaSignosVitalesDental,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    ID_USUARIO = user.ID_USUARIO,
                    REGISTRO_FEC = _hoy,
                    ID_CONSEC = 0,//SE INICIALIZA UN CAMPO SOLO PARA COMPLEMENTAR EL OBJETO, ES IRRELEVANTE AL GUARDAR O EDITAR
                    COMPLICACIONES = ComplicacionesDespuesTratamDental,
                    HEMORRAGIA = HemorragiaDespuesExtracDental,
                    REACCION_ANESTESICO = TenidoReaccionNegativaDental
                };

                _HistoriaClinicaDental.ODONTOGRAMA_INICIAL.Clear();

                _HistoriaClinicaDental.HISTORIA_CLINICA_DENTAL_DOCUME.Clear();

                if (LstOdontogramaInicial != null && LstOdontogramaInicial.Any())
                    foreach (var item in LstOdontogramaInicial)
                        _HistoriaClinicaDental.ODONTOGRAMA_INICIAL.Add(item);


                if (LstDocumentosDentales != null && LstDocumentosDentales.Any())
                    foreach (var item in LstDocumentosDentales)
                        _HistoriaClinicaDental.HISTORIA_CLINICA_DENTAL_DOCUME.Add(item);

                if (new cHistoriaClinicaDental().InsertarHistoriaClinicaDental(_HistoriaClinicaDental))
                {
                    IsEnabledHistoriaClinicaDental = IsEnabledBotonesProcesoOdontogramas = MenuGuardarEnabled = SubirImagenesDental = false;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void ImprimeHistoriaClinicaDental()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    var detalleDental = new cHistoriaClinicaDental().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO).FirstOrDefault();

                    if (detalleDental == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación!", "Es necesario haber guardado la historia clínica dental para continuar");
                        return;
                    }

                    cHistoriaClinicaDentalReporte DatosReporte = new cHistoriaClinicaDentalReporte();
                    ReportesView View = new ReportesView();
                    List<cDientesReporte> lstDetalleDientes = new List<cDientesReporte>();
                    List<cDientesReporte> lstSeguimientos = new List<cDientesReporte>();
                    var _centro = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (detalleDental != null)
                    {
                        #region Iniciliza el entorno para mostrar el reporte al usuario
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        View.Owner = PopUpsViewModels.MainWindow;
                        View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                        View.Show();
                        #endregion
                        string dientes = Convert.ToBase64String(new Imagenes().getImagen("miniDiente.png"));
                        string _htmlDiente = "<html><table><tr><td>";
                        for (int i = 0; i < 16; i++)
                            _htmlDiente += "<image id='ImagenDiente' style='width:20px;height:50px;' src = \" data:image/png;base64, " + dientes + " alt=\"diente" + i + "\" /></td><td>";

                        _htmlDiente += "</td></tr></table></html>";

                        DatosReporte.Generico18 = CaptureWebPageBytesP(_htmlDiente, 400, 100);
                        string _Padece = string.Empty;
                        var HistoriaClinicaMedica = new cHistoriaClinica().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO).FirstOrDefault();
                        if (HistoriaClinicaMedica != null)
                        {
                            var patos = HistoriaClinicaMedica.HISTORIA_CLINICA_PATOLOGICOS;
                            if (patos != null && patos.Any())
                            {
                                #region Definicion de arreglo para el condensado
                                if (patos != null && patos.Any())
                                {
                                    _Padece += "<html><table><tr><td><font face=\"Arial\" size=1>";

                                    short _dummy = 0;//sirve para determinar la cantidad de patologicos por linea
                                    foreach (var item in patos)
                                    {
                                        _dummy++;
                                        if (_dummy == 3)
                                        {
                                            _Padece += (item.PATOLOGICO_CAT != null ? !string.IsNullOrEmpty(item.PATOLOGICO_CAT.DESCR) ? item.PATOLOGICO_CAT.DESCR.Trim() : string.Empty : string.Empty) + (": ") + ("<strong>") + (item.RECUPERADO == "S" ? "SI" : "NO") + ("</strong>") + "</font></td></tr><tr><td><font face=\"Arial\" size=1>";
                                            _dummy = 0;
                                        }
                                        else
                                            _Padece += (item.PATOLOGICO_CAT != null ? !string.IsNullOrEmpty(item.PATOLOGICO_CAT.DESCR) ? item.PATOLOGICO_CAT.DESCR.Trim() : string.Empty : string.Empty) + (": ") + ("<strong>") + (item.RECUPERADO == "S" ? "SI" : "NO") + ("</strong>") + "</font></td><td><font face=\"Arial\" size=1>";
                                    };

                                    _Padece += "</tr></table></html>";
                                };

                                #endregion
                            };

                            #region Complementarios
                            DatosReporte.Generico11 = string.Format("{0} {1} {2}",
                                detalleDental.INGRESO != null ? detalleDental.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(detalleDental.INGRESO.IMPUTADO.NOMBRE) ? detalleDental.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                detalleDental.INGRESO != null ? detalleDental.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(detalleDental.INGRESO.IMPUTADO.PATERNO) ? detalleDental.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                detalleDental.INGRESO != null ? detalleDental.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(detalleDental.INGRESO.IMPUTADO.MATERNO) ? detalleDental.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty);

                            DatosReporte.Generico12 = string.Format("{0} {1} {2}",
                                detalleDental.USUARIO != null ? detalleDental.USUARIO.EMPLEADO != null ? detalleDental.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(detalleDental.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? detalleDental.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                detalleDental.USUARIO != null ? detalleDental.USUARIO.EMPLEADO != null ? detalleDental.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(detalleDental.USUARIO.EMPLEADO.PERSONA.PATERNO) ? detalleDental.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                                                detalleDental.USUARIO != null ? detalleDental.USUARIO.EMPLEADO != null ? detalleDental.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(detalleDental.USUARIO.EMPLEADO.PERSONA.MATERNO) ? detalleDental.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty);

                            #endregion

                            DatosReporte.ImagenPatologisoDentales = CaptureWebPageBytesP(_Padece, 400, 100);
                            DatosReporte.Alcohlismo = string.Format("SI [ {0} ]   NO [ {1} ]", HistoriaClinicaMedica.APNP_ALCOHOLISMO == "S" ? "X" : string.Empty, HistoriaClinicaMedica.APNP_ALCOHOLISMO == "N" ? "X" : string.Empty);
                            DatosReporte.Tabaquismo = string.Format("SI [ {0} ]   NO [ {1} ]", HistoriaClinicaMedica.APNP_TABAQUISMO == "S" ? "X" : string.Empty, HistoriaClinicaMedica.APNP_TABAQUISMO == "N" ? "X" : string.Empty);
                            DatosReporte.Toxicomanias = string.Format("SI [ {0} ]   NO [ {1} ]", HistoriaClinicaMedica.APNP_TOXICOMANIAS == "S" ? "X" : string.Empty, HistoriaClinicaMedica.APNP_TOXICOMANIAS == "N" ? "X" : string.Empty);
                            DatosReporte.TomandoMedicamento = string.Format("SI [ {0} ]   NO [ {1} ]", !string.IsNullOrEmpty(HistoriaClinicaMedica.APP_MEDICAMENTOS_ACTIVOS) ? "X" : string.Empty, string.IsNullOrEmpty(HistoriaClinicaMedica.APP_MEDICAMENTOS_ACTIVOS) ? "X" : string.Empty);
                            DatosReporte.CualMedicamentoToma = HistoriaClinicaMedica.APP_MEDICAMENTOS_ACTIVOS;
                            DatosReporte.EstaEmbarazada = string.Format("SI [ {0} ]   NO [ {1} ]", HistoriaClinicaMedica.HISTORIA_CLINICA_GINECO_OBSTRE != null ? HistoriaClinicaMedica.HISTORIA_CLINICA_GINECO_OBSTRE.Any(x => x.EMBARAZO > 0) == true ? "X" : string.Empty : string.Empty, HistoriaClinicaMedica.HISTORIA_CLINICA_GINECO_OBSTRE != null ? HistoriaClinicaMedica.HISTORIA_CLINICA_GINECO_OBSTRE.Any(x => x.EMBARAZO == 0) == true ? "X" : string.Empty : string.Empty);

                            var _familiares = HistoriaClinicaMedica.HISTORIA_CLINICA_FAMILIAR;
                            if (_familiares != null && _familiares.Any())
                            {
                                DatosReporte.Generico2 = _familiares.Any(x => x.AHF_ALERGIAS == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico3 = _familiares.Any(x => x.AHF_CA == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico4 = _familiares.Any(x => x.AHF_CARDIACOS == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico5 = _familiares.Any(x => x.AHF_DIABETES == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico6 = _familiares.Any(x => x.AHF_EPILEPSIA == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico7 = _familiares.Any(x => x.AHF_HIPERTENSIVO == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico8 = _familiares.Any(x => x.AHF_MENTALES == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico9 = _familiares.Any(x => x.AHF_TB == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                            };
                        }
                        else
                            DatosReporte.Alcohlismo = DatosReporte.Tabaquismo = DatosReporte.Toxicomanias = DatosReporte.TomandoMedicamento = DatosReporte.EstaEmbarazada = DatosReporte.Generico2 = DatosReporte.Generico3 = DatosReporte.Generico4 = DatosReporte.Generico5 = DatosReporte.Generico6 = DatosReporte.Generico7 = DatosReporte.Generico8 = DatosReporte.Generico9 = "SI [  ]   NO [  ]";

                        DatosReporte.Generico10 = string.Empty;//dato limpio a proposito
                        DatosReporte.Generico = string.Format("HOMBRE [ {0} ]   MUJER [ {1} ]", SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.SEXO == "M" ? "X" : string.Empty : string.Empty, SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.SEXO == "F" ? "X" : string.Empty : string.Empty);
                        DatosReporte.NombrePaciente = string.Format("{0} {1} {2}",
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);
                        DatosReporte.NoExpediente = string.Format("{0} / {1}", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO);
                        DatosReporte.Hora = detalleDental.REGISTRO_FEC.HasValue ? detalleDental.REGISTRO_FEC.Value.ToString("HH : mm") : string.Empty;
                        DatosReporte.AlergicoMedicamento = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.ALERGICO_MEDICAMENTO == "S" ? "X" : string.Empty, detalleDental.ALERGICO_MEDICAMENTO == "N" ? "X" : string.Empty);
                        DatosReporte.ReaccionNegativa = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.REACCION_ANESTESICO == "S" ? "X" : string.Empty, detalleDental.REACCION_ANESTESICO == "N" ? "X" : string.Empty);
                        DatosReporte.CualMedicamentoAlergico = detalleDental.ALERGICO_MEDICAMENTO_CUAL;
                        DatosReporte.AmenazaAborto = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.AMENAZA_ABORTO == "S" ? "X" : string.Empty, detalleDental.AMENAZA_ABORTO == "N" ? "X" : string.Empty);
                        DatosReporte.Amigdalas = detalleDental.EXP_BUC_AMIGDALAS;
                        DatosReporte.AnomaliasForma = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.DIENTES_ANOM_FORMA == "S" ? "X" : string.Empty, detalleDental.DIENTES_ANOM_FORMA == "N" ? "X" : string.Empty);
                        DatosReporte.AnomaliasTamanio = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.DIENTES_ANOM_TAMANO == "S" ? "X" : string.Empty, detalleDental.DIENTES_ANOM_TAMANO == "N" ? "X" : string.Empty);
                        DatosReporte.Bruxismo = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.BRUXISMO == "S" ? "X" : string.Empty, detalleDental.BRUXISMO == "N" ? "X" : string.Empty);
                        DatosReporte.CansancioMusculosCaraCuello = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.ART_TEMP_CANSANCIO == "S" ? "X" : string.Empty, detalleDental.ART_TEMP_CANSANCIO == "N" ? "X" : string.Empty);
                        DatosReporte.Caries = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.DIENTES_CARIES == "S" ? "X" : string.Empty, detalleDental.DIENTES_CARIES == "N" ? "X" : string.Empty);
                        DatosReporte.Carrillos = detalleDental.EXP_BUC_CARRILLOS;
                        DatosReporte.Chasquidos = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.ART_TEMP_CHASQUIDOS == "S" ? "X" : string.Empty, detalleDental.ART_TEMP_CHASQUIDOS == "N" ? "X" : string.Empty);
                        DatosReporte.Color = detalleDental.ENCIAS_COLORACION;
                        DatosReporte.Complicaciones = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.COMPLICACIONES == "S" ? "X" : string.Empty, detalleDental.COMPLICACIONES == "N" ? "X" : string.Empty);
                        DatosReporte.Dolor = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.ART_TEMP_DOLOR == "S" ? "X" : string.Empty, detalleDental.ART_TEMP_DOLOR == "N" ? "X" : string.Empty);
                        DatosReporte.DolorEspecifique = detalleDental.ART_TEMP_DOLOR_OBS;
                        DatosReporte.Edad = SelectIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty;
                        DatosReporte.EspecifiqueBruxismo = string.Format("DOLOR [ {0} ]   ASINTOMÁTICO [ {1} ]", detalleDental.BRUXISMO_DOLOR == "S" ? "X" : string.Empty, detalleDental.BRUXISMO_DOLOR == "N" ? "X" : string.Empty);
                        DatosReporte.EspecifiqueCansancioMusculosCaraCuello = detalleDental.ART_TEMP_CANSANCIO_OBS;
                        DatosReporte.EspecifiqueChasquidos = detalleDental.ART_TEMP_CHASQUIDOS_OBS;
                        DatosReporte.EspecifiqueRigidezMuscMand = detalleDental.ART_TEMP_RIGIDEZ_OBS;
                        DatosReporte.Estatura = detalleDental.ESTATURA;
                        DatosReporte.Fecha = detalleDental.REGISTRO_FEC.HasValue ? detalleDental.REGISTRO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                        DatosReporte.Fluorosis = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.DIENTES_FLUOROSIS == "S" ? "X" : string.Empty, detalleDental.DIENTES_FLUOROSIS == "N" ? "X" : string.Empty);
                        DatosReporte.Forma = detalleDental.ENCIAS_FORMA;
                        DatosReporte.FrecuenciaCard = detalleDental.FRECUENCIA_CARDIAC;
                        DatosReporte.FrecuenciaResp = detalleDental.FRECUENCIA_RESPIRA;
                        DatosReporte.Frenillos = detalleDental.EXP_BUC_FRENILLOS;
                        DatosReporte.Glicemia = detalleDental.GLICEMIA;
                        DatosReporte.Hemorragia = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.HEMORRAGIA == "S" ? "X" : string.Empty, detalleDental.HEMORRAGIA == "N" ? "X" : string.Empty);
                        DatosReporte.Hipoplasias = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.DIENTES_HIPOPLASIA == "S" ? "X" : string.Empty, detalleDental.DIENTES_HIPOPLASIA == "N" ? "X" : string.Empty);
                        DatosReporte.Labios = detalleDental.EXP_BUC_LABIOS;
                        DatosReporte.MucosaNasal = detalleDental.EXP_BUC_MUCOSA_NASAL;
                        DatosReporte.Lengua = detalleDental.EXP_BUC_LENGUA;
                        DatosReporte.PisoBoca = detalleDental.EXP_BUC_PISO_BOCA;
                        DatosReporte.PaladarDuro = detalleDental.EXP_BUC_PALADAR_DURO;
                        DatosReporte.PaladarBlanco = detalleDental.EXP_BUC_PALADAR_BLANCO;
                        DatosReporte.OtrosExploracionBucod = detalleDental.EXP_BUC_OTROS;
                        DatosReporte.OtrosDientes = detalleDental.DIENTES_OTROS;
                        DatosReporte.RigidezMusculsMand = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.ART_TEMP_RIGIDEZ == "S" ? "X" : string.Empty, detalleDental.ART_TEMP_RIGIDEZ == "N" ? "X" : string.Empty);
                        DatosReporte.Textura = detalleDental.ENCIAS_TEXTURA;
                        DatosReporte.TensionArt = detalleDental.TENSION_ARTERIAL;
                        DatosReporte.Temperatura = detalleDental.TEMPERATURA;
                        DatosReporte.Peso = detalleDental.PESO;
                        DatosReporte.Lactando = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.LACTANDO == "S" ? "X" : string.Empty, detalleDental.LACTANDO == "N" ? "X" : string.Empty);
                    };

                    var _dientes = detalleDental.ODONTOGRAMA_INICIAL;
                    if (_dientes != null && _dientes.Any())
                    {
                        foreach (var item in _dientes)
                        {
                            var _Imagen = new Imagenes().getImagen("imageNotFound.jpg");
                            string _padec = string.Empty;
                            if (item.ID_ENFERMEDAD.HasValue)
                            {
                                _padec = !string.IsNullOrEmpty(item.ENFERMEDAD.NOMBRE) ? item.ENFERMEDAD.NOMBRE.Trim() : string.Empty;
                                var _ImagenExistente = new cOdontogramaSimbologias().GetData(x => x.ID_ENFERMEDAD == item.ID_ENFERMEDAD).FirstOrDefault();
                                if (_ImagenExistente != null)
                                    _Imagen = _ImagenExistente.IMAGEN;
                            };

                            if (item.ID_NOMENCLATURA.HasValue)
                            {
                                _padec = !string.IsNullOrEmpty(item.DENTAL_NOMENCLATURA.DESCR) ? item.DENTAL_NOMENCLATURA.DESCR.Trim() : string.Empty;
                                var _ImagenExistente = new cOdontogramaSimbologias().GetData(x => x.ID_NOMENCLATURA == item.ID_NOMENCLATURA).FirstOrDefault();
                                if (_ImagenExistente != null)
                                    _Imagen = _ImagenExistente.IMAGEN;
                            };

                            lstDetalleDientes.Add(new cDientesReporte
                                {
                                    DienteNombre = string.Format("{0} EN {1} DEL {2} . POSICIÓN {3}",
                                    _padec,
                                    item.ODONTOGRAMA_TIPO != null ? !string.IsNullOrEmpty(item.ODONTOGRAMA_TIPO.DESCR) ? item.ODONTOGRAMA_TIPO.DESCR.Trim() : string.Empty : string.Empty,
                                    item.ODONTOGRAMA != null ? !string.IsNullOrEmpty(item.ODONTOGRAMA.DESCR) ? item.ODONTOGRAMA.DESCR.Trim() : string.Empty : string.Empty,
                                    item.ODONTOGRAMA != null ? item.ODONTOGRAMA.ID_POSICION.ToString() : string.Empty),
                                    ImagenDiente = _Imagen
                                });
                        };
                    };

                    //FALTA CAMBIO POR ODONTOGRAMA2

                    //var _dientesSeguimiento = detalleDental.ODONTOGRAMA_SEGUIMIENTO2;
                    //if (_dientesSeguimiento != null && _dientesSeguimiento.Any())
                    //{
                    //    var _Imagen = new Imagenes().getImagen("imageNotFound.jpg");
                    //    foreach (var item in _dientesSeguimiento)
                    //    {
                    //        lstSeguimientos.Add(new cDientesReporte
                    //            {
                    //                DienteNombre = item.PROGRAMACION_FEC.HasValue ? item.PROGRAMACION_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                    //                Padece = string.Format("{0} EN {1} DEL {2} . POSICIÓN {3}",
                    //                    item.ID_TRATA.HasValue ? !string.IsNullOrEmpty(item.DENTAL_TRATAMIENTO.DESCR) ? item.DENTAL_TRATAMIENTO.DESCR.Trim() : string.Empty : string.Empty,
                    //                    item.ODONTOGRAMA_TIPO != null ? !string.IsNullOrEmpty(item.ODONTOGRAMA_TIPO.DESCR) ? item.ODONTOGRAMA_TIPO.DESCR.Trim() : string.Empty : string.Empty,
                    //                    item.ODONTOGRAMA != null ? !string.IsNullOrEmpty(item.ODONTOGRAMA.DESCR) ? item.ODONTOGRAMA.DESCR.Trim() : string.Empty : string.Empty,
                    //                    item.ODONTOGRAMA != null ? item.ODONTOGRAMA.ID_POSICION.ToString() : string.Empty),
                    //                ImagenDiente = _Imagen
                    //            });
                    //    };
                    //};
                    cEncabezado Encabezado = new cEncabezado();
                    Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                    Encabezado.TituloDos = Parametro.ENCABEZADO2;
                    Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                    Encabezado.NombreReporte = _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty;
                    Encabezado.ImagenFondo = Parametro.LOGO_BC_ACTA_COMUN;
                    Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                    #region Inicializacion de reporte
                    View.Report.LocalReport.ReportPath = "Reportes/rHistoriaClinicaDental.rdlc";
                    View.Report.LocalReport.DataSources.Clear();
                    #endregion

                    #region Definicion d origenes de datos
                    var ds1 = new List<cHistoriaClinicaDentalReporte>();
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

                    Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds3.Name = "DataSet3";
                    rds3.Value = lstDetalleDientes;
                    View.Report.LocalReport.DataSources.Add(rds3);

                    Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds4.Name = "DataSet4";
                    rds4.Value = lstSeguimientos;
                    View.Report.LocalReport.DataSources.Add(rds4);
                    #endregion

                    View.Report.RefreshReport();
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void InicializaListasDentales()
        {
            try
            {
                LstEnfermedadesDentales = new ObservableCollection<ENFERMEDAD>(new cEnfermedades().GetData(x => x.TIPO == "D"));
                LstNomenclaturasDentales = new ObservableCollection<DENTAL_NOMENCLATURA>(new cDentalNomenclatura().GetData());
                LstTratamientosDentales = new ObservableCollection<DENTAL_TRATAMIENTO>(new cTratamientoDental().GetData());
                ListTipoDocumento = new ObservableCollection<HC_DOCUMENTO_TIPO>(new cTipoDocumentosHistoriaClinica().GetData(x => x.ESTATUS == "S", y => y.DESCR));

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstEnfermedadesDentales.Insert(0, (new ENFERMEDAD() { ID_ENFERMEDAD = -1, NOMBRE = "SELECCIONE" }));
                    LstNomenclaturasDentales.Insert(0, (new DENTAL_NOMENCLATURA() { ID_NOMENCLATURA = -1, DESCR = "SELECCIONE" }));
                    LstTratamientosDentales.Insert(0, (new DENTAL_TRATAMIENTO() { ID_TRATA = -1, DESCR = "SELECCIONE" }));
                    ListTipoDocumento.Insert(0, (new HC_DOCUMENTO_TIPO() { ID_DOCTO = -1, DESCR = "SELECCIONE" }));
                }));


                IdTipoImagenDental = -1;
                IdSelectedEnfermedadDental = -1;
                IdSelectedTratamientoDental = -1;
                IdTratamientoSeguimiento = -1;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void OdontogramaClick(Object obj)
        {
            try
            {
                if (obj == null) return;
                if (!(obj is object[])) return;
                if (!(((object[])obj)[0] is CheckBox)) return;
                if (!(((object[])obj)[1] is string)) return;
                var checkbox = (CheckBox)((object[])obj)[0];
                var PosicionDiente = ((object[])obj)[1].ToString();
                ListOdontograma = ListOdontograma ?? new List<CustomOdontograma>();
                if (checkbox.IsChecked.HasValue ? checkbox.IsChecked.Value : false)
                {
                    ListOdontograma.Add(new CustomOdontograma
                    {
                        ID_DIENTE = short.Parse(PosicionDiente.Split('_')[0]),
                        ID_POSICION = short.Parse(PosicionDiente.Split('_')[1]),
                    });
                }
                else
                    if (ListOdontograma.Any(f => f.ID_POSICION == short.Parse(PosicionDiente.Split('_')[1]) && f.ID_DIENTE == short.Parse(PosicionDiente.Split('_')[0])))
                        ListOdontograma.Remove(ListOdontograma.First(f => f.ID_POSICION == short.Parse(PosicionDiente.Split('_')[1]) && f.ID_DIENTE == short.Parse(PosicionDiente.Split('_')[0])));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la region del diente.", ex);
            }
        }

        private void OdontogramaClickSeg(Object obj)
        {
            try
            {
                if (obj == null) return;
                if (!(obj is object[])) return;
                if (!(((object[])obj)[0] is CheckBox)) return;
                if (!(((object[])obj)[1] is string)) return;
                var checkbox = (CheckBox)((object[])obj)[0];
                var PosicionDiente = ((object[])obj)[1].ToString();
                ListSeguimientoDientes = ListSeguimientoDientes ?? new List<CustomOdontograma>();
                if (checkbox.IsChecked.HasValue ? checkbox.IsChecked.Value : false)
                {
                    ListSeguimientoDientes.Add(new CustomOdontograma
                    {
                        ID_DIENTE = short.Parse(PosicionDiente.Split('_')[0]),
                        ID_POSICION = short.Parse(PosicionDiente.Split('_')[1]),
                    });
                }
                else
                    if (ListSeguimientoDientes.Any(f => f.ID_POSICION == short.Parse(PosicionDiente.Split('_')[1]) && f.ID_DIENTE == short.Parse(PosicionDiente.Split('_')[0])))
                        ListSeguimientoDientes.Remove(ListSeguimientoDientes.First(f => f.ID_POSICION == short.Parse(PosicionDiente.Split('_')[1]) && f.ID_DIENTE == short.Parse(PosicionDiente.Split('_')[0])));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la region del diente.", ex);
            }
        }
    }
}
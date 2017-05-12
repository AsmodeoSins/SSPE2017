using System;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Collections.Generic;
using ControlPenales.Clases;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using SSP.Controlador.Catalogo.Justicia;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        private void LimpiarSentencia() {
            try
            {
                FecS = FecEjecutoriaS = FecInicioCompurgacionS = null;
                AniosS = MesesS = DiasS = AniosAbonadosS = MesesAbonadosS = DiasAbonadosS = null;

                MultaSi = MultaNo = ReparacionSi = ReparacionNo = SustitucionSi = SustitucionNo = false;

                MultaS = ReparacionDanioS = SustitucionPenaS = SustitucionPenaS = SuspensionCondicionalS = ObservacionS = MotivoCancelacionAntecedenteS = string.Empty;

                GradoAutoriaS = GradoParticipacionS = -1;

                LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar sentencia", ex);
            }
    }

        private bool GuardarSentencia()
        {
            try
            {
                if (!base.HasErrors)
                {
                    var sen = new SENTENCIA();
                    sen.ID_CENTRO = SelectedCausaPenal.ID_CENTRO;
                    sen.ID_ANIO = SelectedCausaPenal.ID_ANIO;
                    sen.ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO;
                    sen.ID_INGRESO = SelectedCausaPenal.ID_INGRESO;
                    sen.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;
                    sen.FEC_SENTENCIA = FecS;
                    sen.FEC_EJECUTORIA = FecEjecutoriaS;
                    sen.FEC_INICIO_COMPURGACION = FecInicioCompurgacionS;
                    sen.ANIOS = AniosS;
                    sen.MESES = MesesS;
                    sen.DIAS = DiasS;
                    sen.MULTA = MultaS;
                    if (MultaSi)
                        sen.MULTA_PAGADA = "S";
                    else
                        sen.MULTA_PAGADA = "N";
                    sen.REPARACION_DANIO = ReparacionDanioS;
                    if (ReparacionSi)
                        sen.REPARACION_DANIO_PAGADA = "S";
                    else
                        sen.REPARACION_DANIO_PAGADA = "N";
                    sen.SUSTITUCION_PENA = SustitucionPenaS;
                    if (SustitucionSi)
                        sen.SUSTITUCION_PENA_PAGADA = "S";
                    else
                        sen.SUSTITUCION_PENA_PAGADA = "N";
                    sen.SUSPENSION_CONDICIONAL = SuspensionCondicionalS;
                    sen.OBSERVACION = ObservacionS;
                    sen.MOTIVO_CANCELACION_ANTECEDENTE = MotivoCancelacionAntecedenteS;
                    sen.ID_GRADO_AUTORIA = GradoAutoriaS;
                    sen.ID_GRADO_PARTICIPACION = GradoParticipacionS;
                    sen.ANIOS_ABONADOS = AniosAbonadosS;
                    sen.MESES_ABONADOS = MesesAbonadosS;
                    sen.DIAS_ABONADOS = DiasAbonadosS;
                    sen.FEC_REAL_COMPURGACION = FecRealCompurgacionS;

                     if (SelectedSentencia == null)//INSERT
                    {
                        sen.ESTATUS = "A";
                        //Delitos
                        var delitos = new List<SENTENCIA_DELITO>(LstSentenciaDelitos == null ? null : LstSentenciaDelitos.Select((w, i) => new SENTENCIA_DELITO() { 
                            ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                            ID_ANIO = SelectedCausaPenal.ID_ANIO,
                            ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                            ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                            ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                            ID_SENTENCIA = 0,
                            ID_DELITO = w.ID_DELITO,
                            ID_FUERO = w.ID_FUERO,
                            ID_MODALIDAD = w.ID_MODALIDAD,
                            ID_TIPO_DELITO = w.ID_TIPO_DELITO,
                            CANTIDAD = w.CANTIDAD,
                            OBJETO = w.OBJETO,
                            DESCR_DELITO = w.DESCR_DELITO,
                            ID_CONS = Convert.ToInt16(i + 1)}));
                        sen.SENTENCIA_DELITO = delitos;
                        sen.ID_SENTENCIA = new cSentencia().Insertar(sen);
                        if (sen.ID_SENTENCIA > 0)
                        {
                            //if (GuardarDelitoSentencia(sen.ID_SENTENCIA))
                                return true;
                            //else
                            //    return false;
                        }
                        else
                            return false;

                    }
                    else //UPDATE
                    {
                        sen.ESTATUS = SelectedSentencia.ESTATUS;
                        sen.ID_SENTENCIA = SelectedSentencia.ID_SENTENCIA;
                        var delitos = new List<SENTENCIA_DELITO>(LstSentenciaDelitos == null ? null : LstSentenciaDelitos.Select((w, i) => new SENTENCIA_DELITO()
                        {
                            ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                            ID_ANIO = SelectedCausaPenal.ID_ANIO,
                            ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                            ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                            ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                            ID_SENTENCIA = SelectedSentencia.ID_SENTENCIA,
                            ID_DELITO = w.ID_DELITO,
                            ID_FUERO = w.ID_FUERO,
                            ID_MODALIDAD = w.ID_MODALIDAD,
                            ID_TIPO_DELITO = w.ID_TIPO_DELITO,
                            CANTIDAD = w.CANTIDAD,
                            OBJETO = w.OBJETO,
                            DESCR_DELITO = w.DESCR_DELITO,
                            ID_CONS = Convert.ToInt16(i + 1)
                        }));
                        if ((new cSentencia()).Actualizar(sen,delitos))
                        {
                            if (GuardarDelitoSentencia(sen.ID_SENTENCIA))
                                return true;
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar sentencia", ex);
            }
            return false;
        }

        private void PopulateSentencia() 
        {
            try
            {
                var s = SelectedSentencia;
                FecS = SelectedSentencia.FEC_SENTENCIA;
                FecEjecutoriaS = SelectedSentencia.FEC_EJECUTORIA;
                FecInicioCompurgacionS = SelectedSentencia.FEC_INICIO_COMPURGACION;
                AniosS = SelectedSentencia.ANIOS;
                MesesS = SelectedSentencia.MESES;
                DiasS = SelectedSentencia.DIAS;
                MultaS = SelectedSentencia.MULTA;
                if (!string.IsNullOrEmpty(SelectedSentencia.MULTA_PAGADA))
                {
                    if (SelectedSentencia.MULTA_PAGADA.Equals("S"))
                        MultaSi = true;
                    else
                        MultaNo = true;
                }
                else
                    MultaNo = true;
                
                ReparacionDanioS = SelectedSentencia.REPARACION_DANIO;

                if (!string.IsNullOrEmpty(SelectedSentencia.REPARACION_DANIO_PAGADA))
                {
                    if (SelectedSentencia.REPARACION_DANIO_PAGADA.Equals("S"))
                        ReparacionSi = true;
                    else
                        ReparacionNo = true;
                }
                else
                    ReparacionNo = true;

                
                SustitucionPenaS = SelectedSentencia.SUSTITUCION_PENA;
                if (!string.IsNullOrEmpty(SelectedSentencia.SUSTITUCION_PENA_PAGADA))
                {
                    if (SelectedSentencia.SUSTITUCION_PENA_PAGADA.Equals("S"))
                        SustitucionSi = true;
                    else
                        SustitucionNo = true;
                }
                else
                    SustitucionNo = true;
                
                SuspensionCondicionalS = SelectedSentencia.SUSPENSION_CONDICIONAL;
                ObservacionS = SelectedSentencia.OBSERVACION;
                MotivoCancelacionAntecedenteS = SelectedSentencia.MOTIVO_CANCELACION_ANTECEDENTE;
                GradoAutoriaS = SelectedSentencia.ID_GRADO_AUTORIA;
                GradoParticipacionS = SelectedSentencia.ID_GRADO_PARTICIPACION;
                AniosAbonadosS = SelectedSentencia.ANIOS_ABONADOS;
                MesesAbonadosS = SelectedSentencia.MESES_ABONADOS;
                DiasAbonadosS = SelectedSentencia.DIAS_ABONADOS;
                FecRealCompurgacionS = SelectedSentencia.FEC_REAL_COMPURGACION;

                //DELITOS
                PopulateDelitoSentencia();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer sentencia", ex);
            }
        }
    }
}

using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cSentencias 
    {
        ///CALCULAR TIEMPOS DE SENTENCIA Y COMPURGACION
        private void CalcularSentencia(INGRESO SelectedIngreso)
        {
            //try
            //{
            //    //LstSentenciasIngresos = new List<SentenciaIngreso>();
            //    if (SelectedIngreso != null)
            //    {
            //        int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
            //        DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
            //        if (SelectedIngreso.CAUSA_PENAL != null)
            //        {
            //            foreach (var cp in SelectedIngreso.CAUSA_PENAL)
            //            {
            //                var segundaInstancia = false;
            //                if (cp.SENTENCIA != null)
            //                {
            //                    if (cp.SENTENCIA.Count > 0)
            //                    {
            //                        //BUSCAMOS SI TIENE 2DA INSTANCIA
            //                        if (cp.RECURSO.Count > 0)
            //                        {
            //                            var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
            //                            if (r != null)
            //                            {
            //                                var res = r.SingleOrDefault();
            //                                if (res != null)
            //                                {
            //                                    //SENTENCIA
            //                                    anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
            //                                    meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
            //                                    dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);

            //                                    //LstSentenciasIngresos.Add(
            //                                    //new SentenciaIngreso()
            //                                    //{
            //                                    //    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
            //                                    //    Fuero = cp.CP_FUERO,
            //                                    //    SentenciaAnios = res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS : 0,
            //                                    //    SentenciaMeses = res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES : 0,
            //                                    //    SentenciaDias = res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS : 0,
            //                                    //    Instancia = "SEGUNDA INSTANCIA",
            //                                    //    Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
            //                                    //});
            //                                    segundaInstancia = true;
            //                                }
            //                            }
            //                        }

            //                        var s = cp.SENTENCIA.SingleOrDefault();
            //                        if (s != null)
            //                        {
            //                            if (FechaInicioCompurgacion == null)
            //                            {
            //                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
            //                            }
            //                            else
            //                            {
            //                                if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
            //                                    FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
            //                            }

            //                            //SENTENCIA
            //                            if (!segundaInstancia)
            //                            {
            //                                anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
            //                                meses = meses + (s.MESES != null ? s.MESES.Value : 0);
            //                                dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);

            //                                //LstSentenciasIngresos.Add(
            //                                //new SentenciaIngreso()
            //                                //{
            //                                //    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
            //                                //    Fuero = cp.CP_FUERO,
            //                                //    SentenciaAnios = s.ANIOS != null ? s.ANIOS : 0,
            //                                //    SentenciaMeses = s.MESES != null ? s.MESES : 0,
            //                                //    SentenciaDias = s.DIAS != null ? s.DIAS : 0,
            //                                //    Instancia = "PRIMERA INSTANCIA",
            //                                //    Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
            //                                //});
            //                            }

            //                            //ABONO
            //                            anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
            //                            meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
            //                            dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);


            //                        }
            //                    }
            //                    else
            //                    {
            //                        //LstSentenciasIngresos.Add(
            //                        //new SentenciaIngreso()
            //                        //{
            //                        //    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
            //                        //    Fuero = cp.CP_FUERO,
            //                        //    SentenciaAnios = null,
            //                        //    SentenciaMeses = null,
            //                        //    SentenciaDias = null
            //                        //});
            //                    }
            //                }
            //                else
            //                {
            //                    //LstSentenciasIngresos.Add(
            //                    //new SentenciaIngreso()
            //                    //{
            //                    //    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
            //                    //    Fuero = cp.CP_FUERO,
            //                    //    SentenciaAnios = null,
            //                    //    SentenciaMeses = null,
            //                    //    SentenciaDias = null
            //                    //});
            //                }
            //            }
            //        }

            //        while (dias > 29)
            //        {
            //            meses++;
            //            dias = dias - 30;
            //        }
            //        while (meses > 11)
            //        {
            //            anios++;
            //            meses = meses - 12;
            //        }

            //        TotalAnios = AniosPenaI = anios;
            //        TotalMeses = MesesPenaI = meses;
            //        TotalDias = DiasPenaI = dias;

            //        AniosAbonosI = anios_abono;
            //        MesesAbonosI = meses_abono;
            //        DiasAbonosI = dias_abono;

            //        if (FechaInicioCompurgacion != null)
            //        {
            //            FechaFinCompurgacion = FechaInicioCompurgacion;
            //            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
            //            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
            //            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
            //            //
            //            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
            //            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
            //            FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

            //            int a = 0, m = 0, d = 0;
            //            new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
            //            AniosCumplidoI = a;
            //            MesesCumplidoI = m;
            //            DiasCumplidoI = d;
            //            a = m = d = 0;
            //            new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);
            //            AniosRestanteI = a;
            //            MesesRestanteI = m;
            //            DiasRestanteI = d;
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            //}
        }
    }
}

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.EstudioMI;
namespace ControlPenales
{
    partial class EMILiberadoViewModel : ValidationViewModelBase
    {
        private void PopulateSentenciaDelito()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (SelectIngreso.CAUSA_PENAL != null)
                    {
                        foreach (var cp in SelectIngreso.CAUSA_PENAL)
                        {
                            bool segundaInstancia = false;
                            if (cp.SENTENCIA != null)
                            {
                                if (cp.SENTENCIA.Count > 0)
                                {
                                    if (cp.RECURSO != null)
                                    { 
                                        var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
                                        if (r != null)
                                        {
                                            if (r.Count() > 0)
                                            {
                                                var res = r.FirstOrDefault();
                                                //SENTENCIA
                                                anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
                                                meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
                                                dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);
                                                segundaInstancia = true;
                                            }
                                        }
                                    }

                                    var s = cp.SENTENCIA.FirstOrDefault();
                                    if (s != null)
                                    {
                                        if (s.SENTENCIA_DELITO != null)
                                        {
                                            foreach (var del in s.SENTENCIA_DELITO)
                                            {
                                                if (!string.IsNullOrEmpty(Delitos))
                                                    Delitos = string.Format("{0},", Delitos);
                                                Delitos = string.Format("{0}{1}", Delitos, del.DESCR_DELITO);
                                            }

                                        }

                                        if (FechaInicioCompurgacion == null)
                                        {
                                            FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        }
                                        else
                                        {
                                            if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        }

                                        //SENTENCIA
                                        if (segundaInstancia)
                                        {
                                            anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
                                            meses = meses + (s.MESES != null ? s.MESES.Value : 0);
                                            dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);
                                        }
                                        //ABONO
                                        anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
                                        meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
                                        dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);

                                    }
                                }
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

                    AniosS = anios;
                    MesesS = meses;
                    DiasS = dias;

                    FechaFinCompurgacion = FechaInicioCompurgacion;
                    if (FechaFinCompurgacion != null)
                    {
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
                        //
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

                        int a = 0, m = 0, d = 0;
                        var fecha = Fechas.GetFechaDateServer;

                        new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                        AniosC = a;
                        MesesC = m;
                        DiasC = d;
                        a = m = d = 0;

                        new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);
                        AniosPC = a;
                        MesesPC = m;
                        DiasPC = d;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer sentencia delito", ex);
            }
        }
    }
}

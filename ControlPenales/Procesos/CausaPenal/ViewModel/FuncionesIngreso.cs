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
        private bool GuardarIngreso()
        {
            try
            {
                var ingreso = new INGRESO();
                ingreso.ID_CENTRO = SelectedIngreso.ID_CENTRO;
                ingreso.ID_ANIO = SelectedIngreso.ID_ANIO;
                ingreso.ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO;
                ingreso.ID_INGRESO = SelectedIngreso.ID_INGRESO;
                ingreso.FEC_REGISTRO = FecRegistroI;
                ingreso.FEC_INGRESO_CERESO = FecCeresoI;
                ingreso.ID_TIPO_INGRESO = TipoI;
                ingreso.ID_CLASIFICACION_JURIDICA = ClasificacionI;
                ingreso.ID_ESTATUS_ADMINISTRATIVO = EstatusAdministrativoI;
                ingreso.DOCINTERNACION_NUM_OFICIO = NoOficioI;
                ingreso.ID_AUTORIDAD_INTERNA = AutoridadInternaI;
                ingreso.ID_TIPO_SEGURIDAD = TipoSeguridadI;
                ingreso.ID_DISPOSICION = QuedaDisposicionI;
                if (SelectedUbicacion != null)
                {
                    ingreso.ID_UB_CENTRO = SelectedUbicacion.ID_CENTRO;
                    ingreso.ID_UB_EDIFICIO = SelectedUbicacion.ID_EDIFICIO;
                    ingreso.ID_UB_SECTOR = SelectedUbicacion.ID_SECTOR;
                    ingreso.ID_UB_CELDA = SelectedUbicacion.ID_CELDA;
                    ingreso.ID_UB_CAMA = SelectedUbicacion.ID_CAMA;
                }
                else
                {
                    ingreso.ID_UB_CENTRO = SelectedIngreso.ID_UB_CENTRO;
                    ingreso.ID_UB_EDIFICIO = SelectedIngreso.ID_UB_EDIFICIO;
                    ingreso.ID_UB_SECTOR = SelectedIngreso.ID_UB_SECTOR;
                    ingreso.ID_UB_CELDA = SelectedIngreso.ID_UB_CELDA;
                    ingreso.ID_UB_CAMA = SelectedIngreso.ID_UB_CAMA;
                }
                ingreso.ANIO_GOBIERNO = AnioGobiernoI;
                ingreso.FOLIO_GOBIERNO = FolioGobiernoI;
                //DELITO
                ingreso.ID_INGRESO_DELITO = IngresoDelito;

                //INTERCONEXION
                ingreso.NUC = SelectedIngreso.NUC;
                ingreso.ID_PERSONA_PG = SelectedIngreso.ID_PERSONA_PG;

                if ((new cIngreso()).Actualizar(ingreso))
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar ingreso", ex);
            }
            return false;
        }

        private void PopulateIngreso() {
            try
            {
                //DATOS GENERALES
                AnioD = SelectIngreso.ID_ANIO;
                FolioD = SelectIngreso.ID_IMPUTADO;
                PaternoD = SelectIngreso.IMPUTADO.PATERNO;
                MaternoD = SelectIngreso.IMPUTADO.MATERNO;
                NombreD = SelectIngreso.IMPUTADO.NOMBRE;
                IngresosD = SelectIngreso.ID_INGRESO;
                //NoControlD = SelectIngreso
                if (SelectIngreso.CAMA != null)
                {
                    UbicacionD = UbicacionI = string.Format("{0}-{1}{2}-{3}",
                                               SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Replace(" ", string.Empty),
                                               SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Replace(" ", string.Empty),
                                               SelectIngreso.CAMA.CELDA.ID_CELDA.Replace(" ", string.Empty),
                                               SelectIngreso.ID_UB_CAMA);
                }
                else
                {
                    UbicacionD = UbicacionI = string.Empty;
                }
                TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD.DESCR;
                FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO.Value;
                ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA.DESCR;
                if (SelectIngreso.ESTATUS_ADMINISTRATIVO != null)
                    EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                else
                    EstatusD = string.Empty;
                //DATOS INGRESO
                FecRegistroI = SelectIngreso.FEC_REGISTRO.Value;
                FecCeresoI = SelectIngreso.FEC_INGRESO_CERESO.Value;
                TipoI = SelectIngreso.ID_TIPO_INGRESO.Value;
                EstatusAdministrativoI = SelectIngreso.ID_ESTATUS_ADMINISTRATIVO.Value;
                ClasificacionI = SelectIngreso.ID_CLASIFICACION_JURIDICA;
                NoOficioI = SelectIngreso.DOCINTERNACION_NUM_OFICIO;
                AutoridadInternaI = SelectIngreso.ID_AUTORIDAD_INTERNA.Value;
                TipoSeguridadI = SelectIngreso.ID_TIPO_SEGURIDAD;
                QuedaDisposicionI = SelectIngreso.ID_DISPOSICION.Value;
                //DELITO
                if (SelectIngreso.ID_INGRESO_DELITO != null)
                {
                    if (SelectIngreso.ID_INGRESO_DELITO == 0)
                        IngresoDelito = -1;
                    else
                        IngresoDelito = SelectIngreso.ID_INGRESO_DELITO.Value;
                }

                //EXPEDIENTE GOBIERNO
                FolioGobiernoI = SelectIngreso.FOLIO_GOBIERNO;
                AnioGobiernoI = SelectIngreso.ANIO_GOBIERNO;
                //
                CalcularSentencia();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ingreso", ex);
            }
        }


        ///CALCULAR TIEMPOS DE SENTENCIA Y COMPURGACION
        private void CalcularSentencia()
        {
            try
            {
                //LstSentenciasIngresos = new List<SentenciaIngreso>();
                LstSentenciasIngresos = new ObservableCollection<SentenciaIngreso>();
                if (SelectedIngreso != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (SelectedIngreso.CAUSA_PENAL != null)
                    {
                        foreach (var cp in SelectedIngreso.CAUSA_PENAL)
                        {
                            bool segundaInstancia = false,Incidente = false;
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
                                        {
                                            FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                            //FechaInicioCompurgacion = s.FEC_REAL_COMPURGACION;
                                        }
                                        else
                                        {
                                            if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                            //if (FechaInicioCompurgacion > s.FEC_REAL_COMPURGACION)
                                            //    FechaInicioCompurgacion = s.FEC_REAL_COMPURGACION;
                                        }

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

                    TotalAnios = AniosPenaI = anios;
                    TotalMeses = MesesPenaI = meses;
                    TotalDias = DiasPenaI = dias;

                    AniosAbonosI = anios_abono;
                    MesesAbonosI = meses_abono;
                    DiasAbonosI = dias_abono;

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
                        //new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                        new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                        AniosCumplidoI = a;
                        MesesCumplidoI = m;
                        DiasCumplidoI = d;
                        a = m = d = 0;
                        new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);
                        AniosRestanteI = a;
                        MesesRestanteI = m;
                        DiasRestanteI = d;
                    }

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
        }
    }
}

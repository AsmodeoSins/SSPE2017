using LinqKit;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cGrupoAsistencia : EntityManagerServer<GRUPO_ASISTENCIA>
    {
        public void GenerarEmpalmes(IEnumerable<ListaEmpalmesInterno> collection)
        {
            foreach (var item in collection)
            {
                var ListaHorarioEmpalmados = new List<GRUPO_HORARIO>();
                foreach (var itemGP in new cGrupoParticipante().GetData().Where(w => w.ID_CENTRO == item.EntityGrupoParticipante.ID_CENTRO && w.ING_ID_CENTRO == item.EntityGrupoParticipante.ING_ID_CENTRO && w.ING_ID_ANIO == item.EntityGrupoParticipante.ING_ID_ANIO && w.ING_ID_IMPUTADO == item.EntityGrupoParticipante.ING_ID_IMPUTADO && w.ING_ID_INGRESO == item.EntityGrupoParticipante.ING_ID_INGRESO).Select(s => s.GRUPO.GRUPO_HORARIO).Where(w => w.Any()))
                    foreach (var itemGH in item.ListGrupoHorario)
                        ListaHorarioEmpalmados.AddRange(itemGP.Where(w => itemGH.HORA_INICIO >= w.HORA_INICIO && itemGH.HORA_TERMINO <= w.HORA_TERMINO));

                var EMPALME_SEQ = GetSequence<int>("EMPALME_SEQ");
                foreach (var itemGA in ListaHorarioEmpalmados)
                {
                    var idcons = new cGrupoParticipante().GetData().Where(wh =>
                        wh.ID_CENTRO == itemGA.ID_CENTRO &&
                        wh.ID_TIPO_PROGRAMA == itemGA.ID_TIPO_PROGRAMA &&
                        wh.ID_ACTIVIDAD == itemGA.ID_ACTIVIDAD &&
                        wh.ID_GRUPO == itemGA.ID_GRUPO &&
                        wh.ING_ID_CENTRO == item.EntityGrupoParticipante.ING_ID_CENTRO &&
                        wh.ING_ID_ANIO == item.EntityGrupoParticipante.ING_ID_ANIO &&
                        wh.ING_ID_IMPUTADO == item.EntityGrupoParticipante.ING_ID_IMPUTADO &&
                        wh.ING_ID_INGRESO == item.EntityGrupoParticipante.ING_ID_INGRESO)
                        .FirstOrDefault().ID_CONSEC;

                    var entityGA = GetData().Where(w => w.ID_CENTRO == itemGA.ID_CENTRO && w.ID_TIPO_PROGRAMA == itemGA.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == itemGA.ID_ACTIVIDAD && w.ID_GRUPO == itemGA.ID_GRUPO && w.ID_CONSEC == idcons && w.ID_GRUPO_HORARIO == itemGA.ID_GRUPO_HORARIO).FirstOrDefault();
                    if (entityGA == null)
                        continue;
                    entityGA.EMPALME = EMPALME_SEQ;
                    entityGA.EMP_COORDINACION = 1;

                    Update(new GRUPO_ASISTENCIA()
                    {
                        ASISTENCIA = entityGA.ASISTENCIA,
                        EMP_APROBADO = entityGA.EMP_APROBADO,
                        EMP_COORDINACION = entityGA.EMP_COORDINACION,
                        EMP_FECHA = entityGA.EMP_FECHA,
                        EMPALME = entityGA.EMPALME,
                        ESTATUS = entityGA.ESTATUS,
                        FEC_REGISTRO = entityGA.FEC_REGISTRO,
                        ID_ACTIVIDAD = entityGA.ID_ACTIVIDAD,
                        ID_CENTRO = entityGA.ID_CENTRO,
                        ID_CONSEC = entityGA.ID_CONSEC,
                        ID_GRUPO = entityGA.ID_GRUPO,
                        ID_GRUPO_HORARIO = entityGA.ID_GRUPO_HORARIO,
                        ID_TIPO_PROGRAMA = entityGA.ID_TIPO_PROGRAMA
                    });
                }
            }
        }

        public void EliminarEmpalme(GRUPO_ASISTENCIA entity)
        {
            var listasistencia = GetData().Where(w => w.ESTATUS == 1 && w.ID_CENTRO == entity.ID_CENTRO && w.ID_TIPO_PROGRAMA == entity.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == entity.ID_ACTIVIDAD && w.ID_GRUPO == entity.ID_GRUPO && w.ID_CONSEC == entity.ID_CONSEC).ToList();

            foreach (var item in listasistencia)
            {
                item.GRUPO_ASISTENCIA_ESTATUS = null;
                item.GRUPO_HORARIO = null;
                item.GRUPO_PARTICIPANTE = null;

                Delete(item);

                if (GetData(w => w.ESTATUS == 1 && w.EMPALME != 0 && w.EMPALME == item.EMPALME).Count() <= 1)
                {
                    foreach (var item2 in GetData(w => w.ESTATUS == 1 && w.EMPALME != 0 && w.EMPALME == item.EMPALME))
                    {
                        Update(new GRUPO_ASISTENCIA()
                        {
                            ASISTENCIA = item2.ASISTENCIA,
                            EMP_APROBADO = null,
                            EMP_COORDINACION = 0,
                            EMP_FECHA = null,
                            EMPALME = 0,
                            ESTATUS = item2.ESTATUS,
                            FEC_REGISTRO = item2.FEC_REGISTRO,
                            ID_ACTIVIDAD = item2.ID_ACTIVIDAD,
                            ID_CENTRO = item2.ID_CENTRO,
                            ID_CONSEC = item2.ID_CONSEC,
                            ID_GRUPO = item2.ID_GRUPO,
                            ID_GRUPO_HORARIO = item2.ID_GRUPO_HORARIO,
                            ID_TIPO_PROGRAMA = item2.ID_TIPO_PROGRAMA
                        });
                    }
                }
            }
        }

        public IQueryable<GRUPO_ASISTENCIA> ObtenerInternosActividad(short ID_GRUPO_HORARIO, short ID_GRUPO)
        {
            try
            {
                return GetData(g =>
                    
                    g.ID_GRUPO_HORARIO == ID_GRUPO_HORARIO &&
                    g.ID_GRUPO == ID_GRUPO &&
                    g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 4 &&
                    g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 5 &&
                    g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 6 &&
                    g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 7 &&
                    g.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO.HasValue &&
                    g.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO.Value == GlobalVariables.gCentro);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<GRUPO_ASISTENCIA> ObtenerInternos(string Nombre = "", String Paterno = "", String Materno = "", short Id_Anio = 0, int Id_Imputado = 0, List<EQUIPO_AREA> Areas = null)
        {
            var predicate = PredicateBuilder.True<GRUPO_ASISTENCIA>();
            var predicateAreas = PredicateBuilder.False<GRUPO_ASISTENCIA>();

            predicate = predicate.And(a =>
                a.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 4 &&
                a.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 5 &&
                a.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 6 &&
                a.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 7);
            if (!string.IsNullOrEmpty(Nombre))
                predicate = predicate.And(a => a.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE.Contains(Nombre));
            if (!string.IsNullOrEmpty(Paterno))
                predicate = predicate.And(a => a.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO.Contains(Paterno));
            if (!string.IsNullOrEmpty(Materno))
                predicate = predicate.And(a => a.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO.Contains(Materno));
            if (Id_Anio != 0)
                predicate = predicate.And(a => a.GRUPO_PARTICIPANTE.ING_ID_ANIO == Id_Anio);
            if (Id_Imputado != 0)
                predicate = predicate.And(a => a.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == Id_Imputado);

            predicate = predicate.And(a =>
                a.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO.HasValue &&
                a.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO.Value == GlobalVariables.gCentro);
            if (Areas != null)
            {
                foreach (var Area in Areas)
                {
                    predicateAreas = predicateAreas.Or(o => o.GRUPO_HORARIO.ID_AREA == Area.ID_AREA);
                }
                predicate = predicate.And(predicateAreas.Expand());
            }

            return GetData(predicate.Expand());
        }


        public bool Actualizar(GRUPO_ASISTENCIA entidad)
        {
            try
            {
                Update(entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        public bool MarcarAsistencia(INGRESO_UBICACION ingreso_ubicacion, GRUPO_ASISTENCIA actividad, List<GRUPO_ASISTENCIA> actividades_no_elegidas)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    ingreso_ubicacion.ID_CONSEC = new cIngresoUbicacion().ObtenerConsecutivo<int>((int)ingreso_ubicacion.ID_CENTRO, (int)ingreso_ubicacion.ID_ANIO, ingreso_ubicacion.ID_IMPUTADO, (int)ingreso_ubicacion.ID_INGRESO);
                    Context.INGRESO_UBICACION.Add(ingreso_ubicacion);
                    Context.GRUPO_ASISTENCIA.Attach(actividad);
                    Context.Entry(actividad).Property(w => w.ASISTENCIA).IsModified = true;
                    foreach (var participacion_no_elegida in actividades_no_elegidas)
                    {

                        var participacion = Context.GRUPO_ASISTENCIA.Where(w =>
                            w.ID_CENTRO == participacion_no_elegida.ID_CENTRO &&
                            w.ID_TIPO_PROGRAMA == participacion_no_elegida.ID_TIPO_PROGRAMA &&
                            w.ID_ACTIVIDAD == participacion_no_elegida.ID_ACTIVIDAD &&
                            w.ID_GRUPO == participacion_no_elegida.ID_GRUPO &&
                            w.ID_GRUPO_HORARIO == participacion_no_elegida.ID_GRUPO_HORARIO &&
                            w.ID_CONSEC == participacion_no_elegida.ID_CONSEC).FirstOrDefault();
                        if (participacion != null)
                        {
                            participacion.ESTATUS = 3;
                            Context.Entry(participacion).State = EntityState.Modified;
                        }
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public bool QuitarAsistencia(INGRESO_UBICACION ingreso_ubicacion, GRUPO_ASISTENCIA actividad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var Ubicacion = Context.INGRESO_UBICACION.Where(w =>
                    w.ID_CENTRO == ingreso_ubicacion.ID_CENTRO &&
                    w.ID_ANIO == ingreso_ubicacion.ID_ANIO &&
                    w.ID_IMPUTADO == ingreso_ubicacion.ID_IMPUTADO &&
                    w.ID_INGRESO == ingreso_ubicacion.ID_INGRESO &&
                    w.ID_CONSEC == ingreso_ubicacion.ID_CONSEC).FirstOrDefault();
                    if (Ubicacion != null)
                    {
                        Context.INGRESO_UBICACION.Remove(Ubicacion);
                        Context.GRUPO_ASISTENCIA.Attach(actividad);
                        Context.Entry(actividad).Property(w => w.ASISTENCIA).IsModified = true;
                        var FechaServer = GetFechaServerDate();
                        var participaciones_justificadas = GetData(g =>
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Year == FechaServer.Year &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Month == FechaServer.Month &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Day == FechaServer.Day &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == FechaServer.Hour &&
                            g.GRUPO_PARTICIPANTE.ING_ID_ANIO == ingreso_ubicacion.ID_ANIO &&
                            g.GRUPO_PARTICIPANTE.ING_ID_CENTRO == ingreso_ubicacion.ID_CENTRO &&
                            g.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == ingreso_ubicacion.ID_IMPUTADO &&
                            g.GRUPO_PARTICIPANTE.ING_ID_INGRESO == ingreso_ubicacion.ID_INGRESO &&
                            ((g.EMP_COORDINACION == 2 && g.EMP_APROBADO == 0) || (g.EMP_COORDINACION == 1 && g.EMP_APROBADO == null)) &&
                            g.ESTATUS == 3).ToList();


                        foreach (var participacion_justificada in participaciones_justificadas)
                        {
                            participacion_justificada.ESTATUS = 1;
                            Context.GRUPO_ASISTENCIA.Attach(participacion_justificada);
                            Context.Entry(participacion_justificada).Property(x => x.ESTATUS).IsModified = true;
                        }
                        Context.SaveChanges();
                        transaccion.Complete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }


        }


        public IQueryable<GRUPO_ASISTENCIA> ObtenerParticionesImputadoAreas(short ID_CENTRO, short ID_ANIO, int ID_IMPUTADO, DateTime Fecha, List<EQUIPO_AREA> Areas, short ID_UB_CENTRO)
        {
            try
            {
                var predicate = PredicateBuilder.False<GRUPO_ASISTENCIA>();
                foreach (var Area in Areas)
                {
                    predicate = predicate.Or(p => p.GRUPO_HORARIO.ID_AREA == Area.ID_AREA);
                }
                return GetData(g =>
                                g.ESTATUS != 2 &&
                                g.GRUPO_PARTICIPANTE.ING_ID_CENTRO == ID_CENTRO &&
                                g.GRUPO_PARTICIPANTE.ING_ID_ANIO == ID_ANIO &&
                                g.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == ID_IMPUTADO &&
                                g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 4 &&
                                g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 5 &&
                                g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 6 &&
                                g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 7 &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Year == Fecha.Year &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Month == Fecha.Month &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Day == Fecha.Day &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == Fecha.Hour).AsExpandable().Where(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<GRUPO_ASISTENCIA> ObtenerParticipantesAreas(DateTime Fecha, List<EQUIPO_AREA> Areas, short ID_UB_CENTRO)
        {
            try
            {
                var predicate = PredicateBuilder.False<GRUPO_ASISTENCIA>();
                foreach (var Area in Areas)
                {
                    predicate = predicate.Or(p => p.GRUPO_HORARIO.ID_AREA == Area.ID_AREA);
                }
                return GetData(g =>
                                g.ESTATUS != 2 &&
                                g.ASISTENCIA == null &&
                                g.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO == ID_UB_CENTRO &&
                                g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 4 &&
                                g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 5 &&
                                g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 6 &&
                                g.GRUPO_PARTICIPANTE.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 7 &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Year == Fecha.Year &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Month == Fecha.Month &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Day == Fecha.Day &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == Fecha.Hour).AsExpandable().Where(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="Participaciones"></param>
        ///// <returns></returns>
        //public bool JustificarAsistencias(List<GRUPO_ASISTENCIA> Participaciones, INGRESO_UBICACION Ingreso_Ubicacion = null, GRUPO_ASISTENCIA Participacion_Elegida = null)
        //{
        //    try
        //    {
        //        using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
        //        {
        //            foreach (var participacion in Participaciones)
        //            {
        //                participacion.ESTATUS = 3;
        //                Context.Entry(participacion).State = EntityState.Modified;
        //            }
        //            if (Participacion_Elegida != null && Ingreso_Ubicacion != null)
        //            {
        //                MarcarAsistencia(new INGRESO_UBICACION()
        //                {
        //                    ID_CENTRO = Ingreso_Ubicacion.ID_CENTRO,
        //                    ID_ANIO = Ingreso_Ubicacion.ID_ANIO,
        //                    ID_IMPUTADO = Ingreso_Ubicacion.ID_IMPUTADO,
        //                    ID_INGRESO = Ingreso_Ubicacion.ID_INGRESO,
        //                    ID_AREA = Ingreso_Ubicacion.ID_AREA,
        //                    MOVIMIENTO_FEC = Ingreso_Ubicacion.MOVIMIENTO_FEC,
        //                    ACTIVIDAD = Ingreso_Ubicacion.ACTIVIDAD,
        //                    ESTATUS = 2
        //                }, new GRUPO_ASISTENCIA()
        //                {
        //                    ID_CENTRO = Participacion_Elegida.ID_CENTRO,
        //                    ID_ACTIVIDAD = Participacion_Elegida.ID_ACTIVIDAD,
        //                    ID_TIPO_PROGRAMA = Participacion_Elegida.ID_TIPO_PROGRAMA,
        //                    ID_GRUPO = Participacion_Elegida.ID_GRUPO,
        //                    ID_CONSEC = Participacion_Elegida.ID_CONSEC,
        //                    ID_GRUPO_HORARIO = Participacion_Elegida.ID_GRUPO_HORARIO,
        //                    ASISTENCIA = 1,
        //                    EMPALME = Participacion_Elegida.EMPALME,
        //                    EMP_COORDINACION = Participacion_Elegida.EMP_COORDINACION,
        //                    EMP_APROBADO = Participacion_Elegida.EMP_APROBADO,
        //                    EMP_FECHA = Participacion_Elegida.EMP_FECHA,
        //                    FEC_REGISTRO = Participacion_Elegida.FEC_REGISTRO,
        //                    ESTATUS = Participacion_Elegida.ESTATUS
        //                });
        //            }
        //            Context.SaveChanges();
        //            transaccion.Complete();
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new ApplicationException(ex.Message);
        //    }
        //}

        public bool ActualizarAsistencia(GRUPO_ASISTENCIA Entity)
        {
            try
            {
                Context.GRUPO_ASISTENCIA.Attach(Entity);
                Context.Entry(Entity).Property(w => w.ASISTENCIA).IsModified = true;
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }






        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CONSEC == Id).FirstOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }

    public class ListaEmpalmesInterno
    {
        public GRUPO_PARTICIPANTE EntityGrupoParticipante { get; set; }
        public List<GRUPO_HORARIO> ListGrupoHorario { get; set; }
    }
}

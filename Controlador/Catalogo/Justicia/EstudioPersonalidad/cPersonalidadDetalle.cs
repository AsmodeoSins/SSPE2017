using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonalidadDetalle : EntityManagerServer<PERSONALIDAD_DETALLE>
    {
        public cPersonalidadDetalle() { }

        public IQueryable<PERSONALIDAD_DETALLE> ObtenerTodosDetalle(int Imputado, short Ingreso, int Estudio)
        {
            try
            {
                var predicate = PredicateBuilder.True<PERSONALIDAD_DETALLE>();
                if (Imputado != decimal.Zero)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);

                if (Ingreso != decimal.Zero)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);

                if (Estudio != decimal.Zero)
                    predicate = predicate.And(w => w.ID_ESTUDIO == Estudio);

                return GetData(predicate.Expand()).OrderBy(x => x.ID_ESTUDIO);
            }

            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<PERSONALIDAD_DETALLE> ObtenerInternosProgramados(System.DateTime? FechaInicio, System.DateTime? FechaFin, short IdCentro)
        {
            try
            {
                var predicado = PredicateBuilder.True<PERSONALIDAD_DETALLE>();

                //definicion de predicado base
                predicado = predicado.And(x => x.PERSONALIDAD != null);
                predicado = predicado.And(x => x.PERSONALIDAD.ID_SITUACION == 5);//valida el estatus del estudio principal para no generar registros basura dentro de control de internos
                predicado = predicado.And(x => x.ID_ESTATUS == 1 || x.ID_ESTATUS == 5);//considera los estatus que estan activos o asignados
                predicado = predicado.And(x => x.ID_AREA != null && x.ID_CENTRO == IdCentro);

                //definicion de predicado complementado con paramteros
                if (FechaInicio.HasValue)
                    predicado = predicado.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.INICIO_FEC) >= FechaInicio);

                if (FechaFin.HasValue)
                    predicado = predicado.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.TERMINO_FEC) <= FechaFin);

                return GetData(predicado.Expand());
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }


        public bool Insertar(PERSONALIDAD_DETALLE Entity)
        {
            try
            {
                if (Entity == null)
                    return false;

                Entity.ID_DETALLE = GetSequence<short>("PERSONALIDAD_DETALLE_SEQ");
                if (Insert(Entity))
                    return true;

                return false;
            }

            catch (Exception exc)
            {
                throw;
            }
        }


        public bool Actualizar(PERSONALIDAD_DETALLE Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;
            }

            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

            return false;
        }

        public bool ActualizaDetalleEstudioPersonalidad(PERSONALIDAD_DETALLE Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var actual = Context.PERSONALIDAD_DETALLE.Where(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_DETALLE == Entity.ID_DETALLE && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();

                    if (actual != null)
                    {
                        actual.INICIO_FEC = Entity.INICIO_FEC;
                        actual.TERMINO_FEC = Entity.TERMINO_FEC;
                        actual.ID_ESTATUS = Entity.ID_ESTATUS;
                        actual.ID_AREA = Entity.ID_AREA;
                        Context.Entry(actual).State = System.Data.EntityState.Modified;
                        Context.SaveChanges();
                        transaccion.Complete();
                        return true;
                    };
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ActualizaFechasEstudiosHijos(List<PERSONALIDAD_DETALLE> Entity)
        {
            try
            {
                if (Entity == null)
                    return false;

                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    foreach (var item in Entity)
                    {
                        if (item.ID_ANIO == null && item.ID_ESTUDIO == null && item.ID_CENTRO == null && item.ID_DETALLE == null && item.ID_IMPUTADO == null)
                            continue;

                       var actual = Context.PERSONALIDAD_DETALLE.Where(x => x.ID_ANIO == item.ID_ANIO && x.ID_DETALLE == item.ID_DETALLE && x.ID_ESTUDIO == item.ID_ESTUDIO && x.ID_IMPUTADO == item.ID_IMPUTADO && x.ID_INGRESO == item.ID_INGRESO && x.ID_CENTRO == item.ID_CENTRO).FirstOrDefault();
                       if (actual != null)
                       {
                           actual.INICIO_FEC = item.INICIO_FEC;
                           actual.TERMINO_FEC = item.TERMINO_FEC;
                           actual.ID_ESTATUS = item.ID_ESTATUS;
                           actual.ID_AREA = item.ID_AREA;
                           Context.Entry(actual).State = System.Data.EntityState.Modified;
                       };
                    };

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception exc)
            {
                return false;
            }

            return false;
        }

        public bool ValidaDisponibleImputadoArea(INGRESO _Ingreso, short? _IdArea,DateTime? _fechaInicio, DateTime? _FechaFin)
        {
            try
            {
                var _ExcarcelacionesImputado = new cExcarcelacion().ObtenerImputadoExcarcelaciones(_Ingreso.ID_CENTRO, _Ingreso.ID_ANIO, _Ingreso.ID_IMPUTADO, _Ingreso.ID_INGRESO);
                var _CitasMedicas = new cAtencionCita().ObtenerTodoPorImputado(_Ingreso.ID_UB_CENTRO.Value, _Ingreso.ID_CENTRO, _Ingreso.ID_ANIO, _Ingreso.ID_IMPUTADO, _Ingreso.ID_INGRESO, _fechaInicio.Value);

                if (_ExcarcelacionesImputado.Any())
                    foreach (var item in _ExcarcelacionesImputado)
                        if (item.ID_ESTATUS == "AC")//EXCARCELACION VIVA, NO SE CONOCE SU REGRESO
                            if (item.PROGRAMADO_FEC.HasValue)
                                if (_fechaInicio.HasValue)
                                    if (_FechaFin.HasValue)
                                        if (_fechaInicio >= item.PROGRAMADO_FEC)
                                            return false;
                                        else
                                            if (_FechaFin >= item.PROGRAMADO_FEC)
                                                return false;

                                            else
                                                if (item.ID_ESTATUS != "CO" && item.ID_ESTATUS != "CA")
                                                    if (item.PROGRAMADO_FEC.HasValue)
                                                        if (_fechaInicio.HasValue)
                                                            if (_FechaFin.HasValue)//SI ES FALSO, TIENE UNA EXCARCELACION PENDIENTE O EN CURSO
                                                                if (_fechaInicio <= item.PROGRAMADO_FEC)
                                                                    return false;

                if (_CitasMedicas.Any())
                    foreach (var item in _CitasMedicas)
                        if (item.ESTATUS != "S" && item.ESTATUS != "C")
                            if (item.CITA_FECHA_HORA.HasValue)
                                if (_fechaInicio.HasValue)
                                    if (_FechaFin.HasValue)//SI ES FALSO, TIENE UNA CITA MEDICA PROGRAMADA 
                                        if (item.CITA_FECHA_HORA.Value.Year == _fechaInicio.Value.Year && item.CITA_FECHA_HORA.Value.Month == _fechaInicio.Value.Month && item.CITA_FECHA_HORA.Value.Day == _fechaInicio.Value.Day && item.CITA_FECHA_HORA.Value.Hour == _fechaInicio.Value.Hour)
                                                return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
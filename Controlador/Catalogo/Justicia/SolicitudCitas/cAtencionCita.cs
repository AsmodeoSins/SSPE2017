using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;
using System.Data.Objects;
using System.Transactions;
using System.Collections.Generic;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAtencionCita : EntityManagerServer<ATENCION_CITA>
    {
        #region Constructor
        public cAtencionCita() { }
        #endregion
        
        #region Obtener
        /// <summary>
        /// Obtiene la agenda de un centro por a partir de una fecha establecida de un imputado.
        /// </summary>
        /// <param name="ubicacion_centro">ID del centro donde se hace la consulta</param>
        /// <param name="AreaTecnica">ID del area tecnica a consultar</param>
        /// <param name="FechaInicio">Fecha a partir de la cual se va a hacer la consulta de la agenda</param>
        /// <param name="rol_enfemero">ID del rol del enfermero en el sistema</param>
        /// <param name="AtencionTipo">ID del tipo de atención en caso de ser necesario</param>
        /// <param name="usuario">ID del usuario al que le corresponden las citas</param>
        /// <returns>Objeto IQueryable &lt;AtencionCita&gt;</returns>
        public IQueryable<ATENCION_CITA> ObtenerTodoPorImputadoCitasEnfermero(short ubicacion_centro, short id_centro, short id_anio, int id_imputado, short id_ingreso,
            short rol_enfemero, DateTime? FechaInicio=null, short? AtencionTipo = null, string usuario = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                if (!string.IsNullOrWhiteSpace(usuario))
                    predicate = predicate.And(w => w.PERSONA.EMPLEADO.USUARIO.Any(a => a.ID_USUARIO.Trim() == usuario));
                else
                    predicate = predicate.And(w => w.PERSONA.EMPLEADO.USUARIO.Any(a => a.USUARIO_ROL.Any(a2 => a2.ID_ROL == rol_enfemero)));
                if (AtencionTipo.HasValue)
                    predicate = predicate.And(w => w.ID_TIPO_ATENCION == AtencionTipo);
                if (FechaInicio.HasValue )
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.CITA_FECHA_HORA) >= EntityFunctions.TruncateTime(FechaInicio.Value));
                predicate = predicate.And(w => w.ESTATUS == "N");
                predicate = predicate.And(w => w.CITA_HORA_TERMINA.HasValue);
                predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == ubicacion_centro);
                predicate = predicate.And(w => w.INGRESO.ID_CENTRO == id_centro && w.INGRESO.ID_ANIO == id_anio && w.INGRESO.ID_IMPUTADO == id_imputado && w.INGRESO.ID_INGRESO == id_ingreso);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene la agenda de un centro por area tecnica a partir de una fecha establecida de un imputado.
        /// </summary>
        /// <param name="ubicacion_centro">ID del centro donde se hace la consulta</param>
        /// <param name="AreaTecnica">ID del area tecnica a consultar</param>
        /// <param name="FechaInicio">Fecha a partir de la cual se va a hacer la consulta de la agenda</param>
        /// <param name="AtencionTipo">ID del tipo de atención en caso de ser necesario</param>
        /// <param name="usuario">ID del usuario al que le corresponden las citas</param>
        /// <returns>Objeto IQueryable &lt;AtencionCita&gt;</returns>
        public IQueryable<ATENCION_CITA> ObtenerTodoPorImputado(short ubicacion_centro,short id_centro, short id_anio, int id_imputado, short id_ingreso,
            DateTime FechaInicio,  short? AreaTecnica = null, short? AtencionTipo = null, string usuario="")
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                if (AreaTecnica.HasValue)
                    predicate = predicate.And(w => w.ATENCION_SOLICITUD.ID_TECNICA == AreaTecnica);
                if (AtencionTipo.HasValue)
                    predicate = predicate.And(w => w.ID_TIPO_ATENCION == AtencionTipo);
                if (!string.IsNullOrWhiteSpace(usuario))
                    predicate = predicate.And(w => w.PERSONA.EMPLEADO.USUARIO.Any(a=>a.ID_USUARIO.Trim()==usuario));
                predicate = predicate.And(w => EntityFunctions.TruncateTime(w.CITA_FECHA_HORA) >= EntityFunctions.TruncateTime(FechaInicio));
                predicate = predicate.And(w => w.CITA_HORA_TERMINA.HasValue);
                predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == ubicacion_centro);
                predicate = predicate.And(w => w.INGRESO.ID_CENTRO == id_centro && w.INGRESO.ID_ANIO == id_anio && w.INGRESO.ID_IMPUTADO == id_imputado && w.INGRESO.ID_INGRESO == id_ingreso);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<ATENCION_CITA> ObtenerTodo()
        {
            try
            {
                return GetData();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene grupos horarios activos
        /// </summary>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaTermino"></param>
        /// <param name="ID_CENTRO"></param>
        /// <returns></returns>
        public IQueryable<ATENCION_CITA> ObtenerActivos(short Centro, DateTime? fechaInicio, DateTime? fechaTermino)
        {
            var predicate = PredicateBuilder.True<ATENCION_CITA>();
            if (Centro != null)
                predicate = predicate.And(w => w.ID_CENTRO == Centro);
            if (fechaInicio != null)
                predicate = predicate.And(w => EntityFunctions.TruncateTime(w.CITA_FECHA_HORA) >= fechaInicio);
            if (fechaTermino != null)
                predicate = predicate.And(w => EntityFunctions.TruncateTime(w.CITA_HORA_TERMINA) <= fechaTermino);
            return GetData(predicate.Expand());
        }

        /// <summary>
        /// Obtiene la agenda de un centro por usuario a partir de una fecha establecida por un tipo de atencion medica.
        /// </summary>
        /// <param name="ubicacion_centro">ID del centro donde se hace la consulta</param>
        /// <param name="usuario">usuario a consultar</param>
        /// <param name="id_tipo_atencion">ID del tipo de atención en caso de ser necesario</param>
        /// <param name="estatus">Lista de estatus posibles para la busqueda</param>
        /// <param name="estatus_administrativos_inactivos">Arreglo de estatus administrativos inactivos para ingresos</param>
        /// <returns>Objeto IQueryable &lt;AtencionCita&gt;</returns>
        public IQueryable<ATENCION_CITA> ObtenerPorUsuarioyTipoAtencionMedica(short ubicacion_centro, string usuario, short id_tipo_atencion, short?[] estatus_administrativos_inactivos, List<string> estatus = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                var predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                if (string.IsNullOrWhiteSpace(usuario))
                    throw new Exception("Error en ObtenerPorUsuarioDesdeFecha: El usuario no puede ser nulo o vacio");
                predicate = predicate.And(w => w.CITA_HORA_TERMINA.HasValue
                    && w.INGRESO.ID_UB_CENTRO == ubicacion_centro && w.PERSONA.EMPLEADO.USUARIO.Any(a=>a.ID_USUARIO.Trim() == usuario.Trim()) && w.ID_TIPO_ATENCION == id_tipo_atencion);
                if (estatus != null && estatus.Count > 0)
                {
                    foreach (var item in estatus)
                    {
                        predicateOR = predicateOR.Or(w => w.ESTATUS == item);
                    }
                    predicate = predicate.And(predicateOR.Expand());
                }
                if (estatus_administrativos_inactivos != null || estatus_administrativos_inactivos.Count() > 0)
                {
                    //predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                    foreach (var item in estatus_administrativos_inactivos)
                        if (item.HasValue)
                            predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item.Value);
                    //predicate = predicate.And(predicateOR.Expand());
                }
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene la agenda de un centro por usuario a partir de una fecha establecida por un tipo de atencion medica.
        /// </summary>
        /// <param name="ubicacion_centro">ID del centro donde se hace la consulta</param>
        /// <param name="usuario">usuario a consultar</param>
        /// <param name="FechaInicio">Fecha a partir de la cual se va a hacer la consulta de la agenda</param>
        /// <param name="id_tipo_atencion">ID del tipo de atención en caso de ser necesario</param>
        /// <param name="estatus">Lista de estatus posibles para la busqueda</param>
        /// <param name="estatus_administrativos_inactivos">Arreglo de estatus administrativos inactivos para ingresos</param>
        /// <returns>Objeto IQueryable &lt;AtencionCita&gt;</returns>
        public IQueryable<ATENCION_CITA> ObtenerPorUsuarioDesdeFechayTipoAtencionMedica(short ubicacion_centro, string usuario, DateTime FechaInicio, short id_tipo_atencion,
            short?[] estatus_administrativos_inactivos, List<string> estatus=null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                var predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                if (string.IsNullOrWhiteSpace(usuario))
                    throw new Exception("Error en ObtenerPorUsuarioDesdeFecha: El usuario no puede ser nulo o vacio");
                predicate = predicate.And(w => EntityFunctions.TruncateTime(w.CITA_FECHA_HORA) >= EntityFunctions.TruncateTime(FechaInicio) && w.CITA_HORA_TERMINA.HasValue
                    && w.INGRESO.ID_UB_CENTRO == ubicacion_centro && w.PERSONA.EMPLEADO.USUARIO.Any(a=>a.ID_USUARIO.Trim() == usuario.Trim()) && w.ID_TIPO_ATENCION == id_tipo_atencion);
                if (estatus!=null && estatus.Count>0)
                {
                    foreach(var item in estatus)
                    {
                        predicateOR = predicateOR.Or(w => w.ESTATUS == item);
                    }
                    predicate = predicate.And(predicateOR.Expand());
                }
                if (estatus_administrativos_inactivos != null || estatus_administrativos_inactivos.Count() > 0)
                {
                    //predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                    foreach (var item in estatus_administrativos_inactivos)
                        if (item.HasValue)
                            predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item.Value);
                    //predicate = predicate.And(predicateOR.Expand());
                }
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// Obtiene la agenda de un centro por usuario a partir de una fecha establecida.
        /// </summary>
        /// <param name="ubicacion_centro">ID del centro donde se hace la consulta</param>
        /// <param name="usuario">usuario a consultar</param>
        /// <param name="FechaInicio">Fecha a partir de la cual se va a hacer la consulta de la agenda</param>
        /// <returns>Objeto IQueryable &lt;AtencionCita&gt;</returns>
        public IQueryable<ATENCION_CITA> ObtenerPorInternoDesdeFecha(short ubicacion_centro, int interno)
        {
            try
            {
                //if (string.IsNullOrWhiteSpace(interno))
                //    throw new Exception("Error en ObtenerPorUsuarioDesdeFecha: El usuario no puede ser nulo o vacio");
                return GetData(w => w.INGRESO.ID_UB_CENTRO == ubicacion_centro && w.ID_IMPUTADO == interno);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }





        /// <summary>
        /// Obtiene la agenda de un centro por usuario a partir de una fecha establecida.
        /// </summary>
        /// <param name="ubicacion_centro">ID del centro donde se hace la consulta</param>
        /// <param name="usuario">usuario a consultar</param>
        /// <param name="estatus_administrativos_inactivos">Arreglo de estatus administrativos inactivos para ingresos</param>
        /// <param name="FechaInicio">Fecha a partir de la cual se va a hacer la consulta de la agenda</param>
        /// <returns>Objeto IQueryable &lt;AtencionCita&gt;</returns>
        public IQueryable<ATENCION_CITA> ObtenerPorUsuarioDesdeFecha(short ubicacion_centro, string usuario, DateTime FechaInicio, short?[] estatus_administrativos_inactivos)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario))
                    throw new Exception("Error en ObtenerPorUsuarioDesdeFecha: El usuario no puede ser nulo o vacio");
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                var predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                predicate = predicate.And(w => EntityFunctions.TruncateTime(w.CITA_FECHA_HORA) >= EntityFunctions.TruncateTime(FechaInicio) && w.CITA_HORA_TERMINA.HasValue
                    && w.INGRESO.ID_UB_CENTRO == ubicacion_centro && w.ID_CENTRO_UBI == ubicacion_centro && w.PERSONA.EMPLEADO.USUARIO.Any(a => a.ID_USUARIO.Trim() == usuario.Trim()));
                if (estatus_administrativos_inactivos != null || estatus_administrativos_inactivos.Count() > 0)
                {
                    //predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                    foreach (var item in estatus_administrativos_inactivos)
                        if (item.HasValue)
                            predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item.Value);
                    //predicate = predicate.And(predicateOR.Expand());
                }
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene la agenda de un centro por area tecnica a partir de una fecha establecida.
        /// </summary>
        /// <param name="ubicacion_centro">ID del centro donde se hace la consulta</param>
        /// <param name="AreaTecnica">ID del area tecnica a consultar</param>
        /// <param name="FechaInicio">Fecha a partir de la cual se va a hacer la consulta de la agenda</param>
        /// <param name="AtencionTipo">ID del tipo de atención en caso de ser necesario</param>
        /// <returns>Objeto IQueryable &lt;AtencionCita&gt;</returns>
        //public IQueryable<ATENCION_CITA> ObtenerTodoDesdeFecha(short ubicacion_centro,DateTime FechaInicio, short? AreaTecnica = null, short? AtencionTipo=null)
        //{
        //    try
        //    {
        //        var predicate = PredicateBuilder.True<ATENCION_CITA>();
        //        if (AreaTecnica.HasValue)
        //            predicate = predicate.And(w => w.ATENCION_SOLICITUD.ID_TECNICA == AreaTecnica);
        //        if (AtencionTipo.HasValue)
        //            predicate = predicate.And(w => w.ID_TIPO_ATENCION == AtencionTipo);
        //        predicate = predicate.And(w=>EntityFunctions.TruncateTime(w.CITA_FECHA_HORA)>=EntityFunctions.TruncateTime(FechaInicio));
        //        predicate = predicate.And(w => w.CITA_HORA_TERMINA.HasValue);
        //        predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == ubicacion_centro);

        //        return GetData(predicate.Expand());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message);
        //    }
        //}
        /// <summary>
        /// Obtiene la agenda de un centro por area tecnica.
        /// </summary>
        /// <param name="ubicacion_centro">ID del centro donde se hace la consulta</param>
        /// <param name="AreaTecnica">ID del area tecnica a consultar</param>
        /// <param name="Hoy">Fecha de la cual se va a hacer la consulta de la agenda</param>
        /// <param name="AtencionTipo">ID del tipo de atención en caso de ser necesario</param>
        /// <param name="estatus_administrativos_inactivos">Arreglo de estatus administrativos inactivos para ingresos</param>
        /// <returns>Objeto IQueryable &lt;AtencionCita&gt;</returns>
        public IQueryable<ATENCION_CITA> ObtenerTodo(short ubicacion_centro, short?[] estatus_administrativos_inactivos, short? AreaTecnica = null, short? AtencionTipo = null, DateTime? Hoy = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                var predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                if (AreaTecnica.HasValue)
                    predicate = predicate.And(w => w.ATENCION_SOLICITUD.ID_TECNICA == AreaTecnica);
                if (AtencionTipo.HasValue)
                    predicate = predicate.And(w => w.ID_TIPO_ATENCION == AtencionTipo);
                if (Hoy.HasValue)
                    predicate = predicate.And(w => w.CITA_FECHA_HORA.HasValue ?
                        (w.CITA_FECHA_HORA.Value.Day == Hoy.Value.Day && w.CITA_FECHA_HORA.Value.Month == Hoy.Value.Month && w.CITA_FECHA_HORA.Value.Year == Hoy.Value.Year)
                            : false);
                predicate = predicate.And(w => w.CITA_HORA_TERMINA.HasValue);
                predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == ubicacion_centro);
                predicate = predicate.And(w => w.ID_CENTRO_UBI == ubicacion_centro);
                if (estatus_administrativos_inactivos != null || estatus_administrativos_inactivos.Count() > 0)
                {
                    //predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                    foreach (var item in estatus_administrativos_inactivos)
                        if (item.HasValue)
                            predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item.Value);
                    //predicate = predicate.And(predicateOR.Expand());
                }
                var x = GetData(predicate.Expand());
                return x;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        //public IQueryable<ATENCION_CITA> ObtenerCitasPorAreaTecnica(int IdTecnica, DateTime FechaHoy)
        //{
        //    try
        //    {
        //        /*EntityFunctions.TruncateTime(w.CITA_FECHA_HORA.Value) == EntityFunctions.TruncateTime(FechaHoy) &&*/ 
        //        return GetData(w => w.ATENCION_SOLICITUD.ID_TECNICA == IdTecnica && w.ESTATUS == "N");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message);
        //    }
        //}
        /// <summary>
        /// Metodo para obtener las citas por usuarios
        /// </summary>
        /// <param name="IdUsuario">ID del Usuario</param>
        /// <param name="estatus_administrativos_inactivos">Arreglo de estatus administrativos inactivos para ingresos</param>
        /// <returns>IQueryable de tipo ATENCION_CITA</returns>
        public IQueryable<ATENCION_CITA> ObtenerCitasPorUsuario(short?[] estatus_administrativos_inactivos, string IdUsuario = "",DateTime? FechaHoy = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                var predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                if (!string.IsNullOrEmpty(IdUsuario))
                    predicate = predicate.And(w =>w.PERSONA.EMPLEADO.USUARIO.Any(a=>a.ID_USUARIO.Trim() == IdUsuario));
                if(FechaHoy.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.CITA_FECHA_HORA.Value) == EntityFunctions.TruncateTime(FechaHoy));
                predicate = predicate.And(w => w.ESTATUS == "N");
                if (estatus_administrativos_inactivos != null || estatus_administrativos_inactivos.Count() > 0)
                {
                    //predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                    foreach (var item in estatus_administrativos_inactivos)
                        if (item.HasValue)
                            predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item.Value);
                    //predicate = predicate.And(predicateOR.Expand());
                }
                return GetData(predicate.Expand());
                //return GetData(w => w.ID_USUARIO.Trim() == IdUsuario && w.ESTATUS == "N");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Metodo para obtener las citas por usuarios
        /// </summary>
        /// <param name="usuario">ID del Usuario</param>
        /// <param name="estatus_administrativos_inactivos">Arreglo de estatus administrativos inactivos para ingresos</param>
        /// <param name="estatus">Lista de estatus posibles para la busqueda</param>
        /// <param name="ubicacion_centro">Centro sobre el cual se va a realizar la consulta</param>
        /// <returns>IQueryable de tipo ATENCION_CITA</returns>
        public IQueryable<ATENCION_CITA> ObtenerCitasPorUsuario(short?[] estatus_administrativos_inactivos, List<string> estatus, short ubicacion_centro, string usuario)
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                var predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                if (string.IsNullOrWhiteSpace(usuario))
                    throw new Exception("Error en ObtenerPorUsuarioDesdeFecha: El usuario no puede ser nulo o vacio");
                predicate = predicate.And(w => w.CITA_HORA_TERMINA.HasValue && w.INGRESO.ID_UB_CENTRO == ubicacion_centro 
                    && w.PERSONA.EMPLEADO.USUARIO.Any(a => a.ID_USUARIO.Trim() == usuario.Trim()));
                if (estatus != null && estatus.Count > 0)
                {
                    foreach (var item in estatus)
                        predicateOR = predicateOR.Or(w => w.ESTATUS == item);
                    predicate = predicate.And(predicateOR.Expand());
                }
                if (estatus_administrativos_inactivos != null || estatus_administrativos_inactivos.Count() > 0)
                {
                    //predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                    foreach (var item in estatus_administrativos_inactivos)
                        if (item.HasValue)
                            predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item.Value);
                    //predicate = predicate.And(predicateOR.Expand());
                }
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public ATENCION_CITA Obtener(int id, short centro)
        {
            try
            {
                return GetData(w => w.ID_CITA == id && w.ID_CENTRO_UBI == centro).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Determina si una cita se sobrepone sobre otra cita ya agendada.
        /// </summary>
        /// <param name="id_cita">ID de la cita a comparar</param>
        /// <param name="tipo_atencion">ID del tipo de atencion medica</param>
        /// <param name="fechaInicio">Fecha de inicio de la cita</param>
        /// <param name="fechaFin">Fecha final de la cita</param>
        /// <param name="estatus_administrativos_inactivos">Arreglo de estatus administrativos inactivos para ingresos</param>
        /// <returns>bool</returns>
        public bool IsOverlapCita(short tipo_atencion,DateTime fechaInicio, DateTime fechaFin,short?[] estatus_administrativos_inactivos, int? id_cita=null )
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                var predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                predicate = predicate.And(w => w.ID_CITA != id_cita && w.ID_TIPO_ATENCION == tipo_atencion && w.CITA_FECHA_HORA <= fechaFin && EntityFunctions.AddMinutes(w.CITA_HORA_TERMINA, -1) >= fechaInicio);
                if (estatus_administrativos_inactivos != null || estatus_administrativos_inactivos.Count() > 0)
                {
                    //predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                    foreach (var item in estatus_administrativos_inactivos)
                        if (item.HasValue)
                            predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item.Value);
                    //predicate = predicate.And(predicateOR.Expand());
                }
                return GetData(predicate.Expand()).Any();
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }

        }

        /// <summary>
        /// Determina si una cita se sobrepone sobre otra cita ya agendada en el area medica o dental.
        /// </summary>
        /// <param name="id_cita">ID de la cita a comparar</param>
        /// <param name="id_responsable">Id del responsable de la cita</param>
        /// <param name="fechaInicio">Fecha de inicio de la cita</param>
        /// <param name="fechaFin">Fecha final de la cita</param>
        /// <param name="estatus_administrativos_inactivos">Arreglo de estatus administrativos inactivos para ingresos</param>
        /// <returns>bool</returns>
        public bool IsOverlapCitaporResponsable(int id_responsable, DateTime fechaInicio, DateTime fechaFin, short?[] estatus_administrativos_inactivos, int? id_cita = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                var predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                predicate = predicate.And(w => w.ID_CITA != id_cita && w.ID_RESPONSABLE.HasValue && w.ID_RESPONSABLE.Value==id_responsable && w.CITA_FECHA_HORA <= fechaFin && EntityFunctions.AddMinutes(w.CITA_HORA_TERMINA, -1) >= fechaInicio);
                if (estatus_administrativos_inactivos != null || estatus_administrativos_inactivos.Count() > 0)
                {
                    //predicateOR = PredicateBuilder.False<ATENCION_CITA>();
                    foreach (var item in estatus_administrativos_inactivos)
                        if (item.HasValue)
                            predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item.Value);
                    //predicate = predicate.And(predicateOR.Expand());
                }
                return GetData(predicate.Expand()).Any();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }

        }

        /// <summary>
        /// Determina si el imputado ya tiene otra cita en otra area.
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio de la cita</param>
        /// <param name="fechaFin">Fecha Final de la cita</param>
        /// <param name="id_anio">id_anio del folio del ingreso</param>
        /// <param name="id_centro">id_centro del folio del ingreso</param>
        /// <param name="id_imputado">id_imputado del folio del ingreso</param>
        /// <param name="id_ingreso">id_ingreso del folio del ingreso</param>
        /// <param name="id_cita">id de la cita de la llave primaria</param>
        /// <param name="id_centro_ubi">id del centro de la llave primaria</param>
        /// <returns>Objeto AtencionCita</returns>
        public ATENCION_CITA ObtieneCitaOtraArea(short id_centro, short id_anio, int id_imputado, short id_ingreso, DateTime fechaInicio, DateTime fechaFin, int? id_cita=null, short? id_centro_ubi=null)
        {
            try
            {
                //revisar
                var predicate = PredicateBuilder.True<ATENCION_CITA>();
                predicate = predicate.And(w => w.CITA_FECHA_HORA <= fechaFin && EntityFunctions.AddMinutes(w.CITA_HORA_TERMINA, -1) >= fechaInicio
                    && w.INGRESO.ID_CENTRO == id_centro && w.INGRESO.ID_ANIO == id_anio && w.ID_IMPUTADO == id_imputado && w.ID_INGRESO == id_ingreso);
                if (id_cita.HasValue && id_centro_ubi.HasValue)
                    predicate = predicate.And(w => w.ID_CITA != id_cita.Value && w.ID_CENTRO_UBI==id_centro_ubi.Value);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }

        }

        #endregion
        
        #region Insercion
        /// <summary>
        /// Método de inserción
        /// </summary>
        /// <param name="Entidad">Entidad a guardar en la tabla</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public int Agregar(ATENCION_CITA Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entidad.ID_CITA = GetIDProceso<int>("ATENCION_CITA","ID_CITA",string.Format("ID_CENTRO_UBI={0}",Entidad.ID_CENTRO_UBI));
                    Context.ATENCION_CITA.Add(Entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                    //if (Insert(Entidad))
                    return Entidad.ID_CITA;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return 0;
        }
        #endregion
        
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad de ATENCION_CITA con incidencia.
        /// </summary>
        /// <param name="Entidad">Entidad a actualizar.</param>
        /// <param name="_incidencia">Entidad incidencia a agregar</param>
        /// <param name="fecha_servidor">Fecha del servidor</param>
        /// <param name="id_mensaje_tipo">ID del tipo de mensaje que se envia como notificacion</param>
        /// <returns>bool</returns>
        public bool Actualizar(ATENCION_CITA Entidad, DateTime fecha_servidor, ATENCION_CITA_INCIDENCIA _incidencia, short id_mensaje_tipo)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    if (_incidencia!=null)
                    {
                        _incidencia.ID_CITA = Entidad.ID_CITA;
                        var consec = GetIDProceso<short>("ATENCION_CITA_INCIDENCIA", "ID_CONSEC", string.Format("ID_CITA = {0} AND ID_ACMOTIVO = {1} AND ID_CENTRO_UBI={2}", _incidencia.ID_CITA, _incidencia.ID_ACMOTIVO, _incidencia.ID_CENTRO_UBI));
                        _incidencia.ID_CONSEC = consec;
                        Context.ATENCION_CITA_INCIDENCIA.Add(_incidencia);
                        var _mensaje_tipo = Context.MENSAJE_TIPO.First(w => w.ID_MEN_TIPO == id_mensaje_tipo);
                        var _ingreso = Context.INGRESO.First(w => w.ID_ANIO == Entidad.ID_ANIO && w.ID_CENTRO == Entidad.ID_CENTRO && w.ID_IMPUTADO == Entidad.ID_IMPUTADO && w.ID_INGRESO == Entidad.ID_INGRESO);
                        var _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_ingreso.IMPUTADO.NOMBRE) ? _ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(_ingreso.IMPUTADO.PATERNO) ? _ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(_ingreso.IMPUTADO.MATERNO) ? _ingreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                        var _motivo_incidencia = Context.ATENCION_CITA_IN_MOTIVO.First(w => w.ID_ACMOTIVO == _incidencia.ID_ACMOTIVO);
                        var _usuario = Context.USUARIO.First(w => w.ID_USUARIO.Trim() == _incidencia.ID_USUARIO);
                        var _nombre_completo_usuario = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_usuario.EMPLEADO.PERSONA.NOMBRE) ? _usuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(_usuario.EMPLEADO.PERSONA.PATERNO) ? _usuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(_usuario.EMPLEADO.PERSONA.MATERNO) ? _usuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty);
                        var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");

                        var _mensaje = new MENSAJE
                        {
                            CONTENIDO = _mensaje_tipo.CONTENIDO + _nombre_completo + " CON FOLIO " + string.Format("{0}/{1}", _ingreso.ID_ANIO, _ingreso.ID_IMPUTADO) + ".\n"
                            + "LA CAUSA DE LA INCIDENCIA ES POR EL MOTIVO " + _motivo_incidencia.DESCR + " CON LA SIGUIENTE OBSERVACION " + _incidencia.OBSERV + " REALIZADA " +
                            "POR " + _nombre_completo_usuario,
                            ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                            ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = fecha_servidor
                        };
                        Context.MENSAJE.Add(_mensaje);
                    }
                    Context.Entry(Entidad).State = EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualizar.</param>
        /// <returns>bool</returns>
        public bool Actualizar(ATENCION_CITA Entidad)
        {
            try
            {
                if (Update(Entidad))
                    return true;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Metodo utilizado para cambiar el dia y hora de una cita ya programada
        /// </summary>
        /// <param name="Entidad">Objeto de Tipo ATENCION_CITA</param>
        /// <returns>true = correcto, false = error</returns>
        public bool ReagendarCita(ATENCION_CITA Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.ATENCION_CITA.Attach(Entidad);
                    Context.Entry(Entidad).Property(x => x.CITA_FECHA_HORA).IsModified = true;
                    Context.Entry(Entidad).Property(x => x.CITA_HORA_TERMINA).IsModified = true;
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
        #endregion
        
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Eliminar(ATENCION_CITA Entidad)
        {
            try
            {
                if (Delete(Entidad))
                    return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        #endregion
    }
}

using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Data.Objects;
using System.Linq;
using LinqKit;
using System.Collections.Generic;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cGrupoHorario : EntityManagerServer<GRUPO_HORARIO>
    {
        public T ObtenerConsecutivo<T>(short IdCentro, short IdTipoPrograma, short IdActividad, short IdGrupo) where T : struct
        {
            var max = GetData().Where(w => w.ID_CENTRO == IdCentro && w.ID_TIPO_PROGRAMA == IdTipoPrograma && w.ID_ACTIVIDAD == IdActividad && w.ID_GRUPO == IdGrupo).Max(m => (short?)(new { m.ID_CENTRO, m.ID_TIPO_PROGRAMA, m.ID_ACTIVIDAD, m.ID_GRUPO, m.ID_GRUPO_HORARIO }.ID_GRUPO_HORARIO));
            return (T)Convert.ChangeType(max.HasValue ? ++max : 1, typeof(T));
        }
        ///// <summary>
        ///// metodo que se conecta a la base de datos para insertar un registro nuevo
        ///// </summary>
        ///// <param name="Entity">objeto de tipo "GRUPO_HORARIO" con valores a insertar</param>
        public GRUPO_HORARIO Insertar(GRUPO_HORARIO Entity)
        {
            try
            {
                Entity.ID_GRUPO_HORARIO = ObtenerConsecutivo<short>(Entity.ID_CENTRO, Entity.ID_TIPO_PROGRAMA, Entity.ID_ACTIVIDAD, Entity.ID_GRUPO);
                Insert(Entity);
                return Entity;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool InsertarEdicion(GRUPO_HORARIO Entity)
        {
            try
            {
                Entity.ID_GRUPO_HORARIO = ObtenerConsecutivo<short>(Entity.ID_CENTRO, Entity.ID_TIPO_PROGRAMA, Entity.ID_ACTIVIDAD, Entity.ID_GRUPO);
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<GRUPO_HORARIO> Obtener(DateTime fechaInicio, DateTime fechaTermino, int Id_Grupo, short ID_CENTRO)
        {
            return GetData().Where(g => EntityFunctions.TruncateTime(g.HORA_INICIO) >= EntityFunctions.TruncateTime(fechaInicio)
                && EntityFunctions.TruncateTime(g.HORA_TERMINO) <= EntityFunctions.TruncateTime(fechaTermino)
                && g.ESTATUS == 1 && g.GRUPO.GRUPO_ESTATUS.ID_ESTATUS_GRUPO == 1 && g.ID_CENTRO == ID_CENTRO && g.ID_GRUPO == Id_Grupo);
        }

        public IQueryable<GRUPO_HORARIO> ObtenerActividades(List<EQUIPO_AREA> Areas, DateTime Fecha)
        {
            try
            {
                var predicate = PredicateBuilder.False<GRUPO_HORARIO>();
                if (Areas != null && Areas.Any())
                    foreach (var Area in Areas)
                        predicate = predicate.Or(p => p.ID_AREA == Area.ID_AREA);

                return GetData(
                    g =>
                        g.HORA_INICIO.Value.Year == Fecha.Year &&
                        g.HORA_INICIO.Value.Month == Fecha.Month &&
                        g.HORA_INICIO.Value.Day == Fecha.Day).AsExpandable().Where(predicate.Expand());
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
        public IQueryable<GRUPO_HORARIO> ObtenerActivos(DateTime? fechaInicio, DateTime? fechaTermino, short ID_CENTRO)
        {
            return GetData().Where(g => EntityFunctions.TruncateTime(g.HORA_INICIO) >= EntityFunctions.TruncateTime(fechaInicio)
                && EntityFunctions.TruncateTime(g.HORA_TERMINO) <= EntityFunctions.TruncateTime(fechaTermino)
                && g.ESTATUS == 1 && g.GRUPO.GRUPO_ESTATUS.ID_ESTATUS_GRUPO == 1 && g.ID_CENTRO == ID_CENTRO);
        }
    }
}

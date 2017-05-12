using LinqKit;
using SSP.Controlador.Catalogo.Justicia.Actividades;
using SSP.Modelo;
using SSP.Servidor;
using System.Collections.Generic;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cActividad : EntityManagerServer<ACTIVIDAD>
    {
        public IQueryable<ACTIVIDAD> ObtenerTodos(string buscar = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<ACTIVIDAD>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                return GetData(predicate.Expand()).OrderBy(w => w.DESCR);
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }

        public List<ACTIVIDAD> Obtener(int Id)
        {
            var Resultado = new List<ACTIVIDAD>();
            try
            {
                Resultado = GetData().Where(w => w.ID_ACTIVIDAD == Id).ToList();
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
            return Resultado;
        }

        public void Insertar(ACTIVIDAD Entity)
        {
            try
            {
                Entity.ID_ACTIVIDAD = GetSequence<short>("ACTIVIDADES_SEQ");
                Insert(Entity);
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }


        public void Actualizar(ACTIVIDAD Entity)
        {
            try
            {
                Update(Entity);
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new System.ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new System.ApplicationException(ex.Message);
            }
        }

        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ACTIVIDAD == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new System.ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new System.ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public List<V_AGENDA> ObtenerActividades()
        {
            //return GetEjecutaQueries<V_AGENDA>("select distinct id,descripcion,area,inicio,final "
            //+ "from ssp.v_agenda "
            //+ "where inicio >= SYSDATE "
            //+ "order by inicio");


            return GetEjecutaQueries<V_AGENDA>("select distinct id,tipo,descripcion,area,inicio,final,id_centro, id_area "
            + "from ssp.v_agenda order by id asc");
        }

        public List<V_AGENDA> ObtenerTodosInternos()
        {
            return GetEjecutaQueries<V_AGENDA>("select * from ssp.v_agenda");
        }

    }
}
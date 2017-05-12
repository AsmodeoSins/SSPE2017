using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cActividadEje : EntityManagerServer<ACTIVIDAD_EJE>
    {
        public IQueryable<ACTIVIDAD_EJE> ObtenerTodos(short? id_eje=null, short? id_tipo_programa=null, short? id_actividad=null,string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<ACTIVIDAD_EJE>();
                if (id_eje.HasValue)
                    predicate = predicate.And(w => w.ID_EJE == id_eje.Value);
                if (id_tipo_programa.HasValue)
                    predicate = predicate.And(w=>w.ID_TIPO_PROGRAMA==id_tipo_programa.Value);
                if (id_actividad.HasValue)
                    predicate = predicate.And(w => w.ID_ACTIVIDAD == id_actividad.Value);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        public void Insertar(ACTIVIDAD_EJE entidad)
        {
            try
            {
                Insert(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void Actualizar(ACTIVIDAD_EJE entidad)
        {
            try
            {
                Update(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}

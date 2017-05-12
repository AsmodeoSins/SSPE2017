using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using LinqKit;

namespace SSP.Controlador.Catalogo.Almacenes
{
    public class cProducto_Presentacion:EntityManagerServer<PRODUCTO_PRESENTACION>
    {
        public bool Insertar(PRODUCTO_PRESENTACION _entidad)
        {
            try
            {
                _entidad.ID_PRESENTACION = GetSequence<short>("PRODUCTO_PRESENTACION_SEQ");
                Insert(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Actualizar(PRODUCTO_PRESENTACION _entidad)
        {
            try
            {
                Update(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Eliminar(PRODUCTO_PRESENTACION _entidad)
        {
            try
            {
                Delete(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO_PRESENTACION> Seleccionar(string estatus ="")
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO_PRESENTACION>();
                //if (!string.IsNullOrWhiteSpace(estatus))
                   // predicate = predicate.And(w => w.ACTIVO == estatus);
                return GetData(predicate.Expand());//, o => o.DESCR);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO_PRESENTACION> Seleccionar(string busqueda, string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO_PRESENTACION>();
                if (string.IsNullOrEmpty(busqueda.TrimEnd()))
                    return Seleccionar(estatus);
                //if(!string.IsNullOrWhiteSpace(estatus))
                //    predicate = predicate.And(w => w.ACTIVO == estatus);
                //predicate = predicate.And(w => w.DESCR.Contains(busqueda));
                return GetData(predicate.Expand());//, o => o.DESCR);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}

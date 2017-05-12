using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;
namespace SSP.Controlador.Catalogo.Almacenes
{
    public class cProducto_Unidad_Medida:EntityManagerServer<PRODUCTO_UNIDAD_MEDIDA>
    {
        public bool Insertar(PRODUCTO_UNIDAD_MEDIDA _entidad)
        {
            try
            {
                _entidad.ID_UNIDAD_MEDIDA = GetSequence<int>("PRODUCTO_UNIDAD_MEDIDA_SEQ");
                Insert(_entidad);
                return true;
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Actualizar(PRODUCTO_UNIDAD_MEDIDA _entidad)
        {
            try
            {
                Update(_entidad);
                return true;
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Eliminar(PRODUCTO_UNIDAD_MEDIDA _entidad)
        {
            try
            {
                Delete(_entidad);
                return true;
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO_UNIDAD_MEDIDA> Seleccionar(string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO_UNIDAD_MEDIDA>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                return GetData(predicate.Expand(), o => o.NOMBRE);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO_UNIDAD_MEDIDA> Seleccionar(string busqueda, string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO_UNIDAD_MEDIDA>();
                if (string.IsNullOrWhiteSpace(busqueda.TrimEnd()))
                    return Seleccionar(estatus);
                if(!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                predicate=predicate.And(w => w.NOMBRE.Contains(busqueda));
                return GetData(predicate.Expand(), o => o.NOMBRE);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

       
    }
}

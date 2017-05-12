using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using SSP.Servidor.ModelosExtendidos;
using LinqKit;

namespace SSP.Controlador.Catalogo.Almacenes
{
    public class cAlmacen:EntityManagerServer<ALMACEN>
    {
        public bool Insertar(ALMACEN _entidad)
        {
            try
            {
                _entidad.ID_ALMACEN = GetSequence<short>("ALMACEN_SEQ");
                Insert(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Actualizar(ALMACEN _entidad)
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

        public bool Eliminar(ALMACEN _entidad)
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

        public IQueryable<ALMACEN> Seleccionar(string estatus,string almacen_grupo="")
        {
            try
            {
                var predicate = PredicateBuilder.True<ALMACEN>();
                if(!string.IsNullOrWhiteSpace(almacen_grupo))
                    predicate=predicate.And(w=>w.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO==almacen_grupo);
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                return  GetData(predicate.Expand(), o => o.DESCRIPCION);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<ALMACEN> Seleccionar(string busqueda, short id_centro = -1, string almacen_grupo = null, string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<ALMACEN>();
                if (string.IsNullOrEmpty(busqueda.TrimEnd()) && id_centro==-1)
                    return Seleccionar(estatus,almacen_grupo );
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (!string.IsNullOrEmpty(busqueda.TrimEnd()))
                    predicate = predicate.And(w => w.DESCRIPCION.Contains(busqueda));
                if (id_centro != -1)
                    predicate = predicate.And(w => w.ID_CENTRO == id_centro);
                if (!string.IsNullOrWhiteSpace(almacen_grupo))
                    predicate = predicate.And(w => w.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == almacen_grupo);
                return GetData(predicate.Expand(), o => o.DESCRIPCION);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<ALMACEN> SeleccionarAlmacenesPrincipales(short id_almacen_tipo_cat, short id_centro = -1, string almacen_grupo = null, string estatus = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<ALMACEN>();
               
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (id_centro != -1)
                    predicate = predicate.And(w => w.ID_CENTRO == id_centro);
                if (!string.IsNullOrWhiteSpace(almacen_grupo))
                    predicate = predicate.And(w => w.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == almacen_grupo);
                predicate = predicate.And(w => w.ALMACEN_TIPO_CAT.ID_ALMACEN_TIPO == id_almacen_tipo_cat);
                predicate = predicate.And(w => !w.ALMACEN_SUPERIOR.HasValue);
                return GetData(predicate.Expand(), o => o.DESCRIPCION);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}

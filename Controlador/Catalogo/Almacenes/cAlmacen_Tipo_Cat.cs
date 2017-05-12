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
    public class cAlmacen_Tipo_Cat:EntityManagerServer<ALMACEN_TIPO_CAT>
    {
        public bool Insertar(ALMACEN_TIPO_CAT _entidad)
        {
            try
            {
                _entidad.ID_ALMACEN_TIPO = GetSequence<short>("PRODUCTO_TIPO_CAT_SEQ");
                Insert(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Actualizar(ALMACEN_TIPO_CAT _entidad)
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

        public bool Eliminar(ALMACEN_TIPO_CAT _entidad)
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

        public IQueryable<ALMACEN_TIPO_CAT> Seleccionar(string almacen_grupo = "", string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<ALMACEN_TIPO_CAT>();

                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (!string.IsNullOrEmpty(almacen_grupo))
                    predicate = predicate.And(w => w.ID_ALMACEN_GRUPO == almacen_grupo);
                return GetData(predicate.Expand(), o => o.DESCR);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<ALMACEN_TIPO_CAT> Seleccionar(string busqueda, string almacen_grupo = "", string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<ALMACEN_TIPO_CAT>();
                if (string.IsNullOrEmpty(busqueda.TrimEnd()))
                    return Seleccionar(almacen_grupo, estatus);
                if(!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (!string.IsNullOrEmpty(almacen_grupo))
                    predicate = predicate.And(w => w.ID_ALMACEN_GRUPO == almacen_grupo);
                predicate = predicate.And(w => w.DESCR.Contains(busqueda));
                return GetData(predicate.Expand(), o => o.DESCR);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<EXT_Almacen_Tipo_Cat> Seleccionar_EXT(string almacen_grupo="", string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<ALMACEN_TIPO_CAT>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (!string.IsNullOrEmpty(almacen_grupo))
                    predicate = predicate.And(w => w.ID_ALMACEN_GRUPO == almacen_grupo);
                return GetData(predicate.Expand(), o => o.DESCR)
                    .Select(s => new EXT_Almacen_Tipo_Cat()
                    {
                        ID_ALMACEN_TIPO = s.ID_ALMACEN_TIPO,
                        DESCR = s.DESCR,
                        ACTIVO = s.ACTIVO,
                        IS_SELECTED = false,
                        ID_ALMACEN_GRUPO=s.ID_ALMACEN_GRUPO
                    });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<EXT_Almacen_Tipo_Cat> Seleccionar_EXT(string busqueda,string almacen_grupo="", string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<ALMACEN_TIPO_CAT>();
                if (string.IsNullOrEmpty(busqueda.TrimEnd()))
                    return Seleccionar_EXT(almacen_grupo, estatus);
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (!string.IsNullOrEmpty(almacen_grupo))
                    predicate = predicate.And(w => w.ID_ALMACEN_GRUPO == almacen_grupo);
                predicate = predicate.And(w => w.DESCR.Contains(busqueda));
                return GetData(predicate.Expand(), o => o.DESCR)
                    .Select(s => new EXT_Almacen_Tipo_Cat()
                    {
                        ID_ALMACEN_TIPO = s.ID_ALMACEN_TIPO,
                        DESCR = s.DESCR,
                        ACTIVO = s.ACTIVO,
                        IS_SELECTED = false,
                        ID_ALMACEN_GRUPO=s.ID_ALMACEN_GRUPO
                    });

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}

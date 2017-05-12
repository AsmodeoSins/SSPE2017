using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using SSP.Servidor.ModelosExtendidos;
using LinqKit;
namespace SSP.Controlador.Catalogo.Almacenes
{
    public class cAlmacen_Grupo:EntityManagerServer<ALMACEN_GRUPO>
    {
        public void Insertar(ALMACEN_GRUPO entidad)
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

        public void Actualizar(ALMACEN_GRUPO entidad)
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

        public IQueryable<ALMACEN_GRUPO> SeleccionarTodos(string busqueda,string id_almacen_grupo="",string estatus = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<ALMACEN_GRUPO>();
                if (!string.IsNullOrWhiteSpace(id_almacen_grupo))
                    predicate = predicate.And(w => w.ID_ALMACEN_GRUPO == id_almacen_grupo);
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (!string.IsNullOrWhiteSpace(busqueda))
                    predicate = predicate.And(w => w.DESCR.Contains(busqueda));
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<ALMACEN_GRUPO> Seleccionar(string id = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<ALMACEN_GRUPO>();
                if (!string.IsNullOrEmpty(id))
                    predicate = predicate.And(w => w.ID_ALMACEN_GRUPO == id);
                return GetData(predicate);
            }catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

    }
}

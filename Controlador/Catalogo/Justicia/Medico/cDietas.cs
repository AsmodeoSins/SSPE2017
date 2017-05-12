using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cDietas : EntityManagerServer<DIETA>
    {
        public cDietas() { }

        public IQueryable<DIETA> ObtenerTodo()
        {
            try
            {
                return GetData();
            }

            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public IQueryable<DIETA> ObtenerXBusqueda(string buscar)
        {
            try
            {
                return GetData().Where(w => w.DESCR.Contains(buscar));
            }

            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public IQueryable<DIETA> ObtenerTodosActivos()
        {
            try
            {
                return GetData().Where(w => w.ESTATUS == "S");
            }

            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public IQueryable<DIETA> ObtenerBuscar(string buscar)
        {
            try
            {
                return GetData().Where(w => string.IsNullOrEmpty(w.DESCR) ? w.DESCR.Contains(buscar) : false);
            }

            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public DIETA ObtenerXID(short id)
        {
            try
            {
                return GetData().First(f => f.ID_DIETA == id);
            }

            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }
    }
}

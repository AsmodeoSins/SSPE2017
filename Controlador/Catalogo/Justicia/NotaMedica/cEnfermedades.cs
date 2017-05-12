using System.Linq;
using System.Transactions;
using SSP.Servidor;
using SSP.Modelo;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEnfermedades : EntityManagerServer<ENFERMEDAD>
    {
        public cEnfermedades() { }


        public ENFERMEDAD ObtenerEnfermedadXID(int id)
        {
            try
            {
                return GetData().Where(x => x.ID_ENFERMEDAD == id).SingleOrDefault();
            }

            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public IQueryable<ENFERMEDAD> ObtenerEnfermedadXLetra(string letra)
        {
            try
            {
                return GetData().Where(x => x.LETRA.Contains(letra));
            }

            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public IQueryable<ENFERMEDAD> ObtenerEnfermedadXClave(string clave)
        {
            try
            {
                return GetData().Where(x => x.CLAVE.Contains(clave));
            }

            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public IQueryable<ENFERMEDAD> ObtenerEnfermedadXBusqueda(string buscar)
        {
            try
            {
                return GetData().Where(x => x.CLAVE.Contains(buscar) || x.LETRA.Contains(buscar) || x.NOMBRE.Contains(buscar));
            }

            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public bool Insertar(ENFERMEDAD Entity)
        {
            try
            {
                if (Insert(Entity))
                    return true;

                return false;
            }

            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(ENFERMEDAD Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;

                return false;
            }

            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }
    }
}

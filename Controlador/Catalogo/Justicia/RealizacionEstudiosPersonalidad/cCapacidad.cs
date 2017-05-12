using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cCapacidad : EntityManagerServer<PFC_V_CAPACIDAD>
    {
        public cCapacidad() { }

        public IQueryable<PFC_V_CAPACIDAD> ObtenerTodos(string buscar)
        {
            try
            {
                getDbSet();
                if (string.IsNullOrEmpty(buscar))
                    return GetData().OrderBy(x => x.DESCR);

                return GetData(x => x.DESCR.Contains(buscar));
            }
            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }
    }
}

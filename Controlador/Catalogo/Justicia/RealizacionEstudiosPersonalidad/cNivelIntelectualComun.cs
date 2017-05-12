using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cNivelIntelectualComun : EntityManagerServer<PFC_IV_NIVEL_INTELECTUAL>
    {
        public cNivelIntelectualComun() { }

        public IQueryable<PFC_IV_NIVEL_INTELECTUAL> ObtenerTodos(string buscar)
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

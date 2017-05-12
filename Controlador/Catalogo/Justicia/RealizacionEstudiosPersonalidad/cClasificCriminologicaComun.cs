using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cClasificCriminologicaComun : EntityManagerServer<PFC_V_CRIMINOLOGICA>
    {
        public cClasificCriminologicaComun() { }

        public IQueryable<PFC_V_CRIMINOLOGICA> ObtenerTodos(string buscar)
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

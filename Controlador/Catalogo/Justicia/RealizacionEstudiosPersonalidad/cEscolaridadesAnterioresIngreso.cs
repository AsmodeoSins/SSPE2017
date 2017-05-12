using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEscolaridadesAnterioresIngreso : EntityManagerServer<PFC_VII_ESCOLARIDAD_ANTERIOR>
    {
        public cEscolaridadesAnterioresIngreso() { }

        public IQueryable<PFC_VII_ESCOLARIDAD_ANTERIOR> ObtenerTodos(string buscar)
        {
            try
            {
                getDbSet();
                if (string.IsNullOrEmpty(buscar))
                    return GetData();

                return GetData(x => x.INTERES.Contains(buscar));
            }
            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }
    }
}

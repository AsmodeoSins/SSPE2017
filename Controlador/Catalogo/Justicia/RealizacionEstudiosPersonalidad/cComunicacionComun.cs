using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cComunicacionComun : EntityManagerServer<PFC_VI_COMUNICACION>
    {
        public cComunicacionComun() { }

        public IQueryable<PFC_VI_COMUNICACION> ObtenerTodos(int IdImputado , short IdIngreso)
        {
            try
            {
                return GetData(x => x.ID_IMPUTADO == IdImputado && x.ID_INGRESO == IdIngreso);
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }
    }
}
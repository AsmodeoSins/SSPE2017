using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEstudioCapacitTrabajoPenitenciarioComun : EntityManagerServer<PFC_VIII_TRABAJO>
    {
        public cEstudioCapacitTrabajoPenitenciarioComun() { }

        public PFC_VIII_TRABAJO Obtener(int IdImputado , short IdIngreso) 
        {
            try
            {
                return GetData(x => x.ID_IMPUTADO == IdImputado && x.ID_INGRESO == IdIngreso).SingleOrDefault();
            }

            catch (System.Exception exc)
            {
                throw;
            }
        }
    }
}

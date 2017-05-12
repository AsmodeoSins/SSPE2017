using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEstudioPersonalidadFueroComun : EntityManagerServer<PERSONALIDAD_FUERO_COMUN>
    {
        public cEstudioPersonalidadFueroComun() { }

        public PERSONALIDAD_FUERO_COMUN Obtener(int IdImputado , int IdIngreso) 
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
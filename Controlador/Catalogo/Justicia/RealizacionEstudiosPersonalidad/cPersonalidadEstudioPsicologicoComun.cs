using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonalidadEstudioPsicologicoComun : EntityManagerServer<PFC_IV_PSICOLOGICO>
    {
        public cPersonalidadEstudioPsicologicoComun() { }

        public PFC_IV_PSICOLOGICO Obtener(int IdImputado, short IdIngreso) 
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

using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonalidadEstudioPsiquiatricoComun : EntityManagerServer<PFC_III_PSIQUIATRICO>
    {
        public cPersonalidadEstudioPsiquiatricoComun() { }

        public PFC_III_PSIQUIATRICO Obtener(int IdImputado, int IdIngreso) 
        {
            try
            {
                return GetData(x => x.ID_INGRESO == IdIngreso && x.ID_IMPUTADO == IdImputado).SingleOrDefault();
            }

            catch (System.Exception exc)
            {
                throw;
            }
        }
    }
}

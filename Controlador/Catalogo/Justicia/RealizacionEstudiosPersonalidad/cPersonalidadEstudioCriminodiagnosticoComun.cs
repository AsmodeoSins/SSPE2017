using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonalidadEstudioCriminodiagnosticoComun : EntityManagerServer<PFC_V_CRIMINODIAGNOSTICO>
    {
        public cPersonalidadEstudioCriminodiagnosticoComun() { }

        public PFC_V_CRIMINODIAGNOSTICO Obtener(int IdImputado, short IdIngreso)
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
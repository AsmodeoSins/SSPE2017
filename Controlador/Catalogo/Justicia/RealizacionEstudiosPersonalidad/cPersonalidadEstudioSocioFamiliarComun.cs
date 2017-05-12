using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonalidadEstudioSocioFamiliarComun : EntityManagerServer<PFC_VI_SOCIO_FAMILIAR>
    {
        public cPersonalidadEstudioSocioFamiliarComun() { }

        public PFC_VI_SOCIO_FAMILIAR Obtener(int IdImputado, short IdIngreso)
        {
            try
            {
                return GetData(c => c.ID_IMPUTADO == IdImputado && c.ID_INGRESO == IdIngreso).SingleOrDefault();
            }

            catch (System.Exception exc)
            {
                throw;
            }
        }
    }
}

using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonalidadEstudioEducativoCultDepComun : EntityManagerServer<PFC_VII_EDUCATIVO>
    {
        public cPersonalidadEstudioEducativoCultDepComun() { }
        public PFC_VII_EDUCATIVO Obtener( int IdImputado , short IdIngreso) 
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

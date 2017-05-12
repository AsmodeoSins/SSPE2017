using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cInformeAreaSeguridadCustodiaComun : EntityManagerServer<PFC_IX_SEGURIDAD>
    {
        public cInformeAreaSeguridadCustodiaComun() { }

        public PFC_IX_SEGURIDAD Obtener(short IdIngreso, int IdImputado)
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

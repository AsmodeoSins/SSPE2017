using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cGrupoSocioFamiliarComun : EntityManagerServer<PFC_VI_GRUPO>
    {
        public cGrupoSocioFamiliarComun() { }

        public IQueryable<PFC_VI_GRUPO> ObtenerTodos(int IdImputado, short IdIngreso,short IdEstudio) 
        {
            try
            {
                return GetData(x => x.ID_IMPUTADO == IdImputado && x.ID_INGRESO == IdIngreso && x.ID_ESTUDIO == IdEstudio).AsQueryable();
            }

            catch (System.Exception exc)
            {
                throw;
            }
        }
    }
}

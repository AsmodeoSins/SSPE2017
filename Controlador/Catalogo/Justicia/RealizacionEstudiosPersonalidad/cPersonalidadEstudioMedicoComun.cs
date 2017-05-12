using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonalidadEstudioMedicoComun : EntityManagerServer<PFC_II_MEDICO>
    {
        public cPersonalidadEstudioMedicoComun() { }

        /// <summary>
        /// Obtiene el estudio medico de fuero comun que se la halla aplicado al imputado
        /// </summary>
        /// <param name="IdIngreso"></param>
        /// <param name="IdImputado"></param>
        /// <returns></returns>
        public PFC_II_MEDICO Obtener(int IdIngreso, int IdImputado)
        {
            try
            {
                return GetData(x => x.ID_INGRESO == IdIngreso && x.ID_IMPUTADO == IdImputado).SingleOrDefault();
            }

            catch (System.Exception)
            {
                throw;
            }
        }
    }
}

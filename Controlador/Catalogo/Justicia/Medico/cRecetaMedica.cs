using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cRecetaMedica : EntityManagerServer<RECETA_MEDICA>
    {
        public cRecetaMedica() { }

        public IQueryable<RECETA_MEDICA> ObtenerXAtencionMedica(int AtencionMedica, short centro)
        {
            try
            {
                return GetData().Where(w => w.ID_ATENCION_MEDICA == AtencionMedica && w.ID_CENTRO_UBI == centro);
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }
    }
}
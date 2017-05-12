using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqKit;
using SSP.Modelo;
using SSP.Servidor;

namespace SSP.Controlador.Principales.Compartidos
{
    public class cParametroClave : EntityManagerServer<PARAMETRO_CLAVE>
    {
        public IQueryable<PARAMETRO_CLAVE> Obtener()
        {
            
            return GetData();

        }

        public bool Insertar(PARAMETRO_CLAVE entity)
        {

            return Insert(entity);
        }
        public bool Editar(PARAMETRO_CLAVE entity)
        {

            return Update(entity);
        }

    }
}

using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using LinqKit;
using System.Data;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cParametros : EntityManagerServer<PARAMETRO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cParametros()
        { }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <
        /// name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public PARAMETRO Obtener(string id_clave,short Centro)
        {
            try
            {
                var res = GetData().FirstOrDefault(w => w.ID_CLAVE.Trim() == id_clave && w.ID_CENTRO == Centro);
                if (res != null)
                    return res;
                else
                return GetData().FirstOrDefault(w => w.ID_CLAVE.Trim() == id_clave && w.ID_CENTRO == 0);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
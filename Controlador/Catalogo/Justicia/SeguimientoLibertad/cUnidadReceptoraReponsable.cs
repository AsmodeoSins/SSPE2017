using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using System.Transactions;
using LinqKit;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cUnidadReceptoraReponsable : EntityManagerServer<UNIDAD_RECEPTORA_RESPONSABLE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cUnidadReceptoraReponsable()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<UNIDAD_RECEPTORA_RESPONSABLE> ObtenerTodos(int UnidadReceptora)
        {
            try
            {

                return GetData().Where(w => w.ID_UNIDAD_RECEPTORA == UnidadReceptora).OrderBy(w => new { w.PATERNO,w.MATERNO,w.NOMBRE });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        
    }
}
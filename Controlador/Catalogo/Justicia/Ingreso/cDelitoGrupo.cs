using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia.Ingreso
{
    public class cDelitoGrupo : EntityManagerServer<DELITO_GRUPO>
    {

         /// <summary>
        /// constructor de la clase
        /// </summary>
        public cDelitoGrupo()
        { }

          /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DELITO"</returns>
        public IQueryable<DELITO_GRUPO> ObtenerTodos(string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<DELITO_GRUPO>();
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);

                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "DELITO"</returns>
        public IQueryable<DELITO_GRUPO> Obtener(string Fuero, int Titulo)
        {
            try
            {
                return GetData().Where(w => w.ID_FUERO == Fuero && w.ID_TITULO == Titulo);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}

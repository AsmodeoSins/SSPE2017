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


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cImputadoDocumento : EntityManagerServer<IMPUTADO_DOCUMENTO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cImputadoDocumento()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "IMPUTADO_DOCUMENTO"</returns>
        public ObservableCollection<IMPUTADO_DOCUMENTO> ObtenerTodos()
        {
            ObservableCollection<IMPUTADO_DOCUMENTO> imputado_documento;
            var Resultado = new List<IMPUTADO_DOCUMENTO>();
            try
            {
                Resultado = GetData().ToList();
                imputado_documento = new ObservableCollection<IMPUTADO_DOCUMENTO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return imputado_documento;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "IMPUTADO_DOCUMENTO"</returns>
        public IMPUTADO_DOCUMENTO Obtener(int? Centro= null,int? Anio = null,int? Imputado = null,int? Ingreso = null,int? TipoDocumento = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<IMPUTADO_DOCUMENTO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if(Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso.HasValue)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (TipoDocumento.HasValue)
                    predicate = predicate.And(w => w.ID_IM_TIPO_DOCTO == TipoDocumento);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "IMPUTADO_DOCUMENTO" con valores a insertar</param>
        public bool Insertar(IMPUTADO_DOCUMENTO Entity)
        {
            try
            {
                return Insert(Entity);
            }
            catch (Exception ex)
            {
               throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "IMPUTADO_DOCUMENTO" con valores a actualizar</param>
        public bool Actualizar(IMPUTADO_DOCUMENTO Entity)
        {
            try
            {
                Update(Entity);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                //if (ex.Message.Contains("part of the object's key information"))
                //    throw new ApplicationException("La llave principal no se puede cambiar");
                //else
                //    throw new ApplicationException(ex.Message);
            }
            
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Centro, int Anio, int Imputado, int TipoDocumento)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_IM_TIPO_DOCTO == TipoDocumento).ToList();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
        
    }
}
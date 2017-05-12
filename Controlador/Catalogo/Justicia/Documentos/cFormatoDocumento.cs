using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
//prueba\\

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cFormatoDocumento : EntityManagerServer<FORMATO_DOCUMENTO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cFormatoDocumento()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FORMATO_DOCUMENTO"</returns>
        public ObservableCollection<FORMATO_DOCUMENTO> ObtenerTodos()
        {
            ObservableCollection<FORMATO_DOCUMENTO> formato_documento;
            var Resultado = new List<FORMATO_DOCUMENTO>();
            try
            {
                Resultado = GetData().ToList();
                formato_documento = new ObservableCollection<FORMATO_DOCUMENTO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return formato_documento;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "FORMATO_DOCUMENTO"</returns>
        public List<FORMATO_DOCUMENTO> Obtener(int Id)
        {
            var Resultado = new List<FORMATO_DOCUMENTO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_FORMATO == Id).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "FORMATO_DOCUMENTO" con valores a insertar</param>
        public int Insertar(FORMATO_DOCUMENTO Entity)
        {
            try
            {
                //Entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");
                Insert(Entity);
                return  Entity.ID_FORMATO;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "FORMATO_DOCUMENTO" con valores a actualizar</param>
        public bool Actualizar(FORMATO_DOCUMENTO Entity)
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
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_FORMATO == Id).ToList();
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
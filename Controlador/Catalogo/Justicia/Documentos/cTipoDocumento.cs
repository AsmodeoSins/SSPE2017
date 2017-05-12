using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTipoDocumento : EntityManagerServer<TIPO_DOCUMENTO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTipoDocumento()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "TIPO_DOCUMENTO"</returns>
        public IQueryable<TIPO_DOCUMENTO> ObtenerTodos()
        {
            //ObservableCollection<TIPO_DOCUMENTO> tipo_documento;
            //var Resultado = new List<TIPO_DOCUMENTO>();
            try
            {
                return GetData().OrderBy(w => w.ID_TIPO_DOCUMENTO);//.ToList();
                //tipo_documento = new ObservableCollection<TIPO_DOCUMENTO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return tipo_documento;
        }
        public List<TIPO_DOCUMENTO> ObtenerAbogadoIngreso(short tipoVisita)
        {
            //ObservableCollection<TIPO_DOCUMENTO> tipo_documento;
            //var Resultado = new List<TIPO_DOCUMENTO>();
            try
            {
                return GetData().Where(w => w.ID_TIPO_VISITA == 3).OrderBy(w => w.ID_TIPO_DOCUMENTO).ToList();
                //tipo_documento = new ObservableCollection<TIPO_DOCUMENTO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return tipo_documento;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "TIPO_DOCUMENTO"</returns>
        public List<TIPO_DOCUMENTO> Obtener(short tipoDocumento, short tipoVisita)
        {
            var Resultado = new List<TIPO_DOCUMENTO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_TIPO_DOCUMENTO == tipoDocumento && w.ID_TIPO_VISITA == tipoVisita).ToList();
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
        /// <param name="Entity">objeto de tipo "TIPO_DOCUMENTO" con valores a insertar</param>
        public bool Insertar(TIPO_DOCUMENTO Entity)
        {
            try
            {
                //Entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");
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
        /// <param name="Entity">objeto de tipo "TIPO_DOCUMENTO" con valores a actualizar</param>
        public bool Actualizar(TIPO_DOCUMENTO Entity)
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
        public bool Eliminar(int Id, int Id2)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_TIPO_DOCUMENTO == Id && w.ID_TIPO_VISITA == Id2).ToList();
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
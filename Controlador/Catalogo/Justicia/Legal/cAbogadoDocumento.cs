using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAbogadoDocumento : EntityManagerServer<ABOGADO_DOCUMENTO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAbogadoDocumento()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VisitaDocumento"</returns>
        public ObservableCollection<ABOGADO_DOCUMENTO> ObtenerTodos(short centro, short tipo_visita, int abogado, short tipo_docto = 0)
        {
            ObservableCollection<ABOGADO_DOCUMENTO> VisitaDocumentos;
            var Resultado = new List<ABOGADO_DOCUMENTO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_TIPO_VISITA == tipo_visita && w.ID_ABOGADO == abogado &&
                    (tipo_docto == 0 ? true : w.ID_TIPO_DOCUMENTO == tipo_docto)).ToList();
                VisitaDocumentos = new ObservableCollection<ABOGADO_DOCUMENTO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VisitaDocumentos;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<ABOGADO_DOCUMENTO> Obtener(ABOGADO_DOCUMENTO Id)
        {
            var Resultado = new List<ABOGADO_DOCUMENTO>();
            try
            {
                Resultado = GetData().Where(w => w == Id).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        ///// <summary>
        ///// metodo que se conecta a la base de datos para insertar un registro nuevo
        ///// </summary>
        ///// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(ABOGADO_DOCUMENTO Entity)
        {
            try
            {
                //Entity.ID_VisitaDocumento = GetSequence<short>("VisitaDocumento_SEQ");
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(ABOGADO_DOCUMENTO Entity)
        {
            try
            {
                return Update(Entity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(ABOGADO_DOCUMENTO Id)
        {
            try
            {
                var Entity = GetData().Where(w => w == Id).SingleOrDefault();
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
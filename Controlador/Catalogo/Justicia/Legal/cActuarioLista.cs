using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cActuarioLista : EntityManagerServer<ACTUARIO_LISTA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cActuarioLista()
        { }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public ACTUARIO_LISTA ObtenerXLista(int idLista)
        {
            var Resultado = new ACTUARIO_LISTA();
            try
            {
                Resultado = GetData().Where(w => w.ID_LISTA == idLista).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public IQueryable<ACTUARIO_LISTA> ObtenerXAbogado(int abogado)
        {
            var Resultado = new List<ACTUARIO_LISTA>().AsQueryable();
            try
            {
                Resultado = GetData().Where(w => w.ID_ABOGADO == abogado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public IQueryable<ACTUARIO_LISTA> ObtenerXAbogadoYFecha(int abogado, DateTime hoy)
        {
            var Resultado = new List<ACTUARIO_LISTA>().AsQueryable();
            try
            {
                Resultado = GetData().Where(w => w.ID_ABOGADO == abogado && (w.CAPTURA_FEC.HasValue ?
                    (w.CAPTURA_FEC.Value.Day == hoy.Day && w.CAPTURA_FEC.Value.Month == hoy.Month && w.CAPTURA_FEC.Value.Year == hoy.Year) : false));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public ACTUARIO_LISTA ObtenerXAbogadoYLista(int abogado, int idLista)
        {
            var Resultado = new ACTUARIO_LISTA();
            try
            {
                Resultado = GetData().Where(w => w.ID_LISTA == idLista && w.ID_ABOGADO == abogado).FirstOrDefault();
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(ACTUARIO_LISTA Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                Entity.ID_LISTA = GetSequence<short>("ACTUARIO_LISTA_SEQ");
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
        public void Actualizar(ACTUARIO_LISTA Entity)
        {
            try
            {
                Update(Entity);
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
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_LISTA == Id).SingleOrDefault();
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
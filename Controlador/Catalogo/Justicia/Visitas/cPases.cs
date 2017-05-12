using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPases : EntityManagerServer<PASE_CATALOGO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cPases()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "PASE_CATALOGO"</returns>
        public ObservableCollection<PASE_CATALOGO> ObtenerTodos(string buscar = "")
        {
            ObservableCollection<PASE_CATALOGO> etnias;
            var Resultado = new List<PASE_CATALOGO>();
            try
            {
                if (string.IsNullOrEmpty(buscar))
                    Resultado = GetData().ToList();
                else
                    Resultado = GetData().Where(w => w.DESCR.Contains(buscar)).ToList();
                etnias = new ObservableCollection<PASE_CATALOGO>(Resultado);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return etnias;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<PASE_CATALOGO> Obtener(int Id)
        {
            var Resultado = new List<PASE_CATALOGO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_PASE == Id).ToList();
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
        public void Insertar(PASE_CATALOGO Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_TIPO_VISITA = GetSequence<short>("TIPO_VISITA_SEQ");
                Insert(Entity);
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
        public void Actualizar(PASE_CATALOGO Entity)
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
                var Entity = GetData().Where(w => w.ID_PASE == Id).SingleOrDefault();
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
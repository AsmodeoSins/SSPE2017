using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cVisitante : EntityManagerServer<VISITANTE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cVisitante()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VISITANTE"</returns>
        public ObservableCollection<VISITANTE> ObtenerTodos(string nombre = "", string paterno = "", string materno = "")
        {
            ObservableCollection<VISITANTE> visitantes;
            var Resultado = new List<VISITANTE>();
            try
            {
                if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(paterno) && !string.IsNullOrEmpty(materno))
                    Resultado = GetData().Where(w => w.PERSONA.NOMBRE.Contains(nombre) && w.PERSONA.PATERNO.Contains(paterno) && w.PERSONA.MATERNO.Contains(materno)).ToList();
                visitantes = new ObservableCollection<VISITANTE>(Resultado);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return visitantes;
        }

        public IQueryable<VISITANTE> ObtenerTodos(long? Id = null,string nombre = "", string paterno = "", string materno = "",int? Pagina = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<VISITANTE>();
                if (Id != null)
                    predicate = predicate.And(w => w.ID_PERSONA == Id);
                if (!string.IsNullOrEmpty(paterno))
                    predicate = predicate.And(w => w.PERSONA.PATERNO.Contains(paterno));
                if (!string.IsNullOrEmpty(materno))
                    predicate = predicate.And(w => w.PERSONA.MATERNO.Contains(materno));
                if (!string.IsNullOrEmpty(nombre))
                    predicate = predicate.And(w => w.PERSONA.NOMBRE.Contains(nombre));
                if(Pagina.HasValue)
                    return GetData(predicate.Expand()).OrderBy(w => w.ID_PERSONA).Take((Pagina.Value * 30)).Skip((Pagina.Value == 1 ? 0 : ((Pagina.Value * 30) - 30)));
                else
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
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<VISITANTE> Obtener(int Id)
        {
            var Resultado = new List<VISITANTE>();
            try
            {
                Resultado = GetData().Where(w => w.ID_PERSONA == Id).ToList();
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
        public bool Insertar(VISITANTE Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_VISITANTE = GetSequence<short>("VISITANTE_SEQ");
                Entity.FEC_ALTA = DateTime.Parse(GetFechaServer());
                Entity.ULTIMA_MODIFICACION = DateTime.Parse(GetFechaServer());
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
        public bool Actualizar(VISITANTE Entity)
        {
            try
            {
                //Entity.ULTIMA_MODIFICACION = DateTime.Parse(GetFechaServer());
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
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_PERSONA == Id).SingleOrDefault();
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
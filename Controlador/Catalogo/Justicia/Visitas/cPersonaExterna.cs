using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonaExterna : EntityManagerServer<PERSONA_EXTERNO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cPersonaExterna()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "PERSONA_EXTERNO"</returns>
        public IQueryable<PERSONA_EXTERNO> ObtenerTodosPersonas(short centro = 0, int persona = 0)
        {
            //ObservableCollection<PERSONA_EXTERNO> personaExterno;
            //var Resultado = new List<PERSONA_EXTERNO>();
            var predicate = PredicateBuilder.True<PERSONA_EXTERNO>();
            try
            {
                if (persona != 0)
                    predicate = predicate.And(a => a.ID_PERSONA == persona);
                predicate = predicate.And(a => a.ID_CENTRO == centro);
                return GetData(predicate.Expand());
                //personaExterno = new ObservableCollection<PERSONA_EXTERNO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return personaExterno;
        }

        public IQueryable<PERSONA_EXTERNO> ObtenerTodos(short? centro = null, int? persona = null, string Paterno = "", string Materno = "", string Nombre = "",int? Pagina = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PERSONA_EXTERNO>();
                if (centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == centro);
                if (persona != null)
                    predicate = predicate.And(w => w.ID_PERSONA == persona);
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.PERSONA.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.PERSONA.MATERNO.Contains(Materno));
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.PERSONA.NOMBRE.Contains(Nombre));
                if(Pagina.HasValue)
                    return GetData(predicate.Expand()).OrderBy(o => o.ID_PERSONA).Take((Pagina.Value * 30)).Skip((Pagina.Value == 1 ? 0 : ((Pagina.Value * 30) - 30))); 
                else
                    return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<PERSONA_EXTERNO> ObtenerTodosActivos(short? centro = null, string persona = "", string Paterno = "", string Materno = "", string Nombre = "", int? Pagina = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PERSONA_EXTERNO>();
                predicate = predicate.And(x => x.ESTATUS == "S");

                if (centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == centro);

                if (!string.IsNullOrEmpty(persona))
                {
                    decimal _Persona = decimal.Parse(persona);
                    predicate = predicate.And(w => w.ID_PERSONA == _Persona);
                };

                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.PERSONA.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.PERSONA.MATERNO.Contains(Materno));
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.PERSONA.NOMBRE.Contains(Nombre));
                if (Pagina.HasValue)
                    return GetData(predicate.Expand()).OrderBy(o => o.ID_PERSONA).Take((Pagina.Value * 30)).Skip((Pagina.Value == 1 ? 0 : ((Pagina.Value * 30) - 30)));
                else
                    return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(PERSONA_EXTERNO Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_AREA = GetSequence<short>("AREA_VISITA_SEQ");
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool Insertar(List<PERSONA_EXTERNO> Entity, short centro = 0, int persona = 0)
        {
            try
            {
                if (Eliminar(centro, persona))
                    return Insert(Entity);
                return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "PERSONA_EXTERNO" con valores a actualizar</param>
        public bool Actualizar(PERSONA_EXTERNO Entity)
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
        public bool Eliminar(short centro = 0, int persona = 0)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == centro && w.ID_PERSONA == persona).ToList();
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
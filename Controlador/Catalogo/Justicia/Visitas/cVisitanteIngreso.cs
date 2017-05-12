using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cVisitanteIngreso : EntityManagerServer<VISITANTE_INGRESO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cVisitanteIngreso()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VISITANTE_INGRESO"</returns>
        public ObservableCollection<VISITANTE_INGRESO> ObtenerTodos(string nombre = "", string paterno = "", string materno = "")
        {
            ObservableCollection<VISITANTE_INGRESO> visitantes;
            var Resultado = new List<VISITANTE_INGRESO>();
            try
            {
                if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(paterno) && !string.IsNullOrEmpty(materno))
                    Resultado = GetData().Where(w => w.VISITANTE.PERSONA.NOMBRE.Contains(nombre) && w.VISITANTE.PERSONA.PATERNO.Contains(paterno) && w.VISITANTE.PERSONA.MATERNO.Contains(materno)).ToList();
                visitantes = new ObservableCollection<VISITANTE_INGRESO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return visitantes;
        }


        public IQueryable<VISITANTE_INGRESO> ObtenerTodos(DateTime? FechaInicio = null, DateTime? FechaFin = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<VISITANTE_INGRESO>();
                if (FechaInicio.HasValue)
                    predicate = predicate.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.FEC_ALTA) >= FechaInicio);
                if (FechaFin.HasValue)
                    predicate = predicate.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.FEC_ALTA) <= FechaFin);
                short[] tipo = { 14, 15, 22, 23 };
                predicate = predicate.And(w => tipo.Contains(w.ID_TIPO_VISITANTE.Value));
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<VISITANTE_INGRESO> ObtenerTodosFecha()
        {
            try
            {
                return GetData();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<VISITANTE_INGRESO> ObtenerTodosIngresos()
        {
            try
            {
                var REGISTRO = 11;
                var EN_REVISION = 12;
                var AUTORIZADO = 13;
                return GetData(g =>
                    g.ID_ESTATUS_VISITA == REGISTRO ||
                    g.ID_ESTATUS_VISITA == EN_REVISION ||
                    g.ID_ESTATUS_VISITA == AUTORIZADO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<VISITANTE_INGRESO> ObtenerVisitantesIngreso(short ID_CENTRO, short ID_ANIO, int ID_IMPUTADO, short ID_INGRESO)
        {
            try
            {
                var REGISTRO = 11;
                var EN_REVISION = 12;
                var AUTORIZADO = 13;
                return GetData(w =>
                    w.ID_CENTRO == ID_CENTRO &&
                    w.ID_ANIO == ID_ANIO &&
                    w.ID_IMPUTADO == ID_IMPUTADO &&
                    w.ID_INGRESO == ID_INGRESO &&
                    (w.ID_ESTATUS_VISITA == REGISTRO ||
                    w.ID_ESTATUS_VISITA == EN_REVISION ||
                    w.ID_ESTATUS_VISITA == AUTORIZADO));
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
        public List<VISITANTE_INGRESO> Obtener(int Id)
        {
            var Resultado = new List<VISITANTE_INGRESO>();
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

        public List<VISITANTE_INGRESO> ObtenerXImputado(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            var Resultado = new List<VISITANTE_INGRESO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        public ObservableCollection<VISITANTE_INGRESO> ObtenerXImputadoYPersona(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0, int persona = 0)
        {
            var Resultado = new ObservableCollection<VISITANTE_INGRESO>();
            try
            {
                Resultado = new ObservableCollection<VISITANTE_INGRESO>(GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso && w.ID_PERSONA == persona));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        public List<VISITANTE_INGRESO> ObtenerXPersona(int persona = 0)
        {
            var Resultado = new List<VISITANTE_INGRESO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_PERSONA == persona).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        public IQueryable<VISITANTE_INGRESO> ObtenerVisitanteIngreso(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0, long persona = 0)
        {

            try
            {
                return GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso && w.ID_PERSONA == persona);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<VISITANTE_INGRESO> ObtenerXImputadoYAcompanante(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0, long persona = 0)
        {

            try
            {
                //Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado &&
                //                               w.ID_INGRESO == ingreso && w.ID_ACOMPANANTE == persona).ToList();
                //RELACIONES
                //ACOMPANANTE = ACOMPANANTE
                //VISITANTE = ACOMPANANTE1
                return GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso
                    && w.ACOMPANANTE.Where(a => a.ID_ACOMPANANTE == persona).Any());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /*public List<VISITANTE_INGRESO> ObtenerXImputadoConAcompanante(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0, int acompanante = 0)
        {
            var Resultado = new List<VISITANTE_INGRESO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado &&
                                                 w.ID_INGRESO == ingreso && w.ID_ACOMPANANTE == acompanante).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }*/

        ///// <summary>
        ///// metodo que se conecta a la base de datos para insertar un registro nuevo
        ///// </summary>
        ///// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(VISITANTE_INGRESO Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_VISITANTE = GetSequence<short>("VISITANTE_SEQ");
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool Insertar(short? Centro, short? Anio, int? Imputado, short Ingreso, int? Persona, List<VISITANTE_INGRESO> list)
        {
            try
            {
                Eliminar(Centro, Anio, Imputado, Ingreso, Persona);
                if (list.Count > 0)
                    return Insert(list);
                else
                    return true;
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
        public bool Actualizar(VISITANTE_INGRESO Entity)
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
        public bool ActualizarLista(List<VISITANTE_INGRESO> Entity)
        {
            try
            {
                var band = true;
                var resul = true;
                foreach (var item in Entity)
                {
                    band = GetData().Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO && w.ID_PERSONA == item.ID_PERSONA).Any();
                    if (band)
                        resul = Update(item);
                    else
                    {
                        //item.NIP = new cPersona().GetSequence<int>("NIP_SEQ");
                        resul = Insert(item);
                    }
                }
                return resul;
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
        public bool Eliminar(short? Centro, short? Anio, int? Imputado, short Ingreso, int? Persona)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_PERSONA == Persona && w.ID_INGRESO == Ingreso && w.ID_IMPUTADO == Imputado &&
                                             w.ID_ANIO == Anio && w.ID_CENTRO == Centro).ToList();
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
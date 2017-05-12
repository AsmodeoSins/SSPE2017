using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAbogado : EntityManagerServer<ABOGADO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAbogado()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "PERSONA"</returns>
        public IQueryable<ABOGADO> ObtenerTodos(string nombre = "", string paterno = "", string materno = "", long? codigo = 0,short? estatus = null,string titulo = "",bool OrderNumero = false)
        {
            
            try
            {
                var predicate = PredicateBuilder.True<ABOGADO>();
                if (codigo != null && codigo != 0)
                    predicate = predicate.And(w => w.ID_ABOGADO == codigo);
                else
                {
                    if (!string.IsNullOrEmpty(nombre))
                        predicate = predicate.And(w => w.PERSONA.NOMBRE.Contains(nombre));
                    if (!string.IsNullOrEmpty(paterno))
                        predicate = predicate.And(w => w.PERSONA.PATERNO.Contains(paterno));
                    if (!string.IsNullOrEmpty(materno))
                        predicate = predicate.And(w => w.PERSONA.MATERNO.Contains(materno));
                    if(estatus != null)
                        predicate = predicate.And(w => w.ID_ESTATUS_VISITA == estatus);
                    if(!string.IsNullOrEmpty(titulo))
                        predicate = predicate.And(w => w.ID_ABOGADO_TITULO == titulo);
                }
                if(!OrderNumero)
                    return GetData(predicate.Expand()).OrderBy(o => o.PERSONA.ID_TIPO_PERSONA);
                else
                    return GetData(predicate.Expand()).OrderBy(o => o.PERSONA.ID_PERSONA);
                #region Comentado
                //if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(paterno) && string.IsNullOrEmpty(materno) && codigo == null) { }
                //else
                //    if (codigo == null || codigo == 0)
                //        Resultado = GetData().Where(w => w.PERSONA.NOMBRE.Contains(nombre) && w.PERSONA.PATERNO.Contains(paterno) && w.PERSONA.MATERNO.Contains(materno)).OrderBy(o => o.PERSONA.ID_TIPO_PERSONA).ToList();
                //    else
                //        Resultado = GetData().Where(w => w.ID_ABOGADO == codigo).OrderBy(o => o.PERSONA.ID_TIPO_PERSONA).ToList();
                //personas = new ObservableCollection<ABOGADO>(Resultado);
                #endregion
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return personas;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<ABOGADO> Obtener(long Id)
        {
            var Resultado = new List<ABOGADO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_ABOGADO == Id).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public IQueryable<ABOGADO> ObtenerColaboradoresXTitular(int titular)
        {
            var Resultado = new List<ABOGADO>().AsQueryable();
            try
            {
                Resultado = GetData().Where(w => w.ABOGADO_TITULAR == titular);
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
        public bool Insertar(ABOGADO Entity)
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(ABOGADO Entity)
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
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ABOGADO == Id).SingleOrDefault();
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

        /// <summary>
        /// metodo que se conecta a la base de datos para obtener el padron de abogados asignados de un centro
        /// </summary>
        /// <param name="id_centro">llave del centro actual</param>
        /// <param name="estatus_visita_activo">valor del id para el estatus de visita autorizado</param>
        /// <returns>IQueryable&lt;ABOGADO&gt;</returns>
        public IQueryable<ABOGADO> AbogadosAsignados(short id_centro, short estatus_visita_autorizado )
        {
            try
            {
                return GetData(w => (w.ID_ABOGADO_TITULO == "C" || w.ID_ABOGADO_TITULO == "T") && w.ABOGADO_INGRESO.Any(a => a.ID_ESTATUS_VISITA == estatus_visita_autorizado && a.INGRESO.ID_UB_CENTRO == id_centro
                    && a.INGRESO.ID_ESTATUS_ADMINISTRATIVO!=4 && a.INGRESO.ID_ESTATUS_ADMINISTRATIVO!=5 && a.INGRESO.ID_ESTATUS_ADMINISTRATIVO!=6 && a.INGRESO.ID_ESTATUS_ADMINISTRATIVO!=7));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
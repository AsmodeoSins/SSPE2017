using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEdificio : EntityManagerServer<EDIFICIO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cEdificio()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ETNIA"</returns>
        public IQueryable<EDIFICIO> ObtenerTodos(string buscar = "", int municipio = 0, int centro = 0, string estatus = "S")
        {
            try
            {
                var predicado = PredicateBuilder.True<EDIFICIO>();

                if (!string.IsNullOrEmpty(buscar))
                    predicado = predicado.And(a => a.DESCR.Contains(buscar));
                if(municipio != 0)
                    predicado = predicado.And(a => a.CENTRO.ID_MUNICIPIO == municipio);
                if(centro != 0)
                    predicado = predicado.And(a => a.ID_CENTRO == centro);
                if(estatus == "S" || estatus == "N")
                    predicado = predicado.And(a => a.ESTATUS == estatus);

                return GetData().AsExpandable().Where(predicado).OrderBy(w => w.DESCR);
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
        public EDIFICIO Obtener(short? edificio, short centro)
        {
            var Resultado = new EDIFICIO();
            try
            {
                Resultado = GetData().Where(w => w.ID_EDIFICIO == edificio && w.ID_CENTRO == centro).FirstOrDefault();
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
        public void Insertar(EDIFICIO Entity)
        {
            try
            {
                Entity.ID_EDIFICIO = (short)(GetData().Where(w => w.ID_CENTRO == Entity.ID_CENTRO).Any() ?
                    GetData().Where(w => w.ID_CENTRO == Entity.ID_CENTRO).OrderByDescending(o => o.ID_EDIFICIO).FirstOrDefault().ID_EDIFICIO + 1 : 1);
                //Entity.ID_EDIFICIO = GetSequence<short>("EDIFICIO_SEQ");
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
        public void Actualizar(EDIFICIO Entity)
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
                var Entity = GetData().Where(w => w.ID_EDIFICIO == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    //if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                    //    throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                    throw new ApplicationException("Error: No se puede eliminar Tipo Filiacion: Tiene dependencias.");

                }
            }
            return false;
        }
    }
}
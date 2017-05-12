using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cVisitaDia : EntityManagerServer<VISITA_DIA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cVisitaDia()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VISITA_EDIFICIO"</returns>
        public IQueryable<VISITA_DIA> ObtenerTodos()
        {
            var Resultado = new List<VISITA_DIA>().AsQueryable();
            try
            {
                Resultado = GetData();
                //dias = new ObservableCollection<VISITA_DIA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public VISITA_DIA ObtenerTodos(short dia)
        {
            VISITA_DIA dias;
            var Resultado = new List<VISITA_DIA>();
            try
            {
                dias = GetData().Where(w => w.ID_DIA == dia).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return dias;
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(VISITA_DIA Entity)
        {
            try
            {
                Entity.ID_DIA = (short)(GetData().Where(w => w.ID_DIA == Entity.ID_DIA).FirstOrDefault().ID_DIA + 1);
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_AREA = GetSequence<short>("AREA_VISITA_SEQ");
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
        public bool Actualizar(VISITA_DIA Entity)
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
        public bool Eliminar(short id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_DIA == id).FirstOrDefault();
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
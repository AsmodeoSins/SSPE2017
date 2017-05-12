using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cVisitaAgenda : EntityManagerServer<VISITA_AGENDA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cVisitaAgenda()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<VISITA_AGENDA> ObtenerTodos(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            var Resultado = new List<VISITA_AGENDA>().AsQueryable();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public IQueryable<VISITA_AGENDA> ObtenerTodosActivos(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            var Resultado = new List<VISITA_AGENDA>().AsQueryable();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso && w.ESTATUS == "0");
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
        public void Insertar(VISITA_AGENDA Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_AREA = GetSequence<short>("AREA_VISITA_SEQ");
                Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool Insertar(List<VISITA_AGENDA> Entity, short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            try
            {
                if (Eliminar(centro, anio, imputado, ingreso))
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(VISITA_AGENDA Entity)
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
        public bool InsertarLista(List<VISITA_AGENDA> Entity, short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            try
            {
                var band = true;
                var resul = true;
                foreach (var item in Entity)
                {
                    band = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso &&
                        w.ID_DIA == item.ID_DIA && w.ID_TIPO_VISITA == item.ID_TIPO_VISITA).Any();
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
        public bool Eliminar(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso).ToList();
                if (Entity != null ? Entity.Count > 0 : false)
                    return Delete(Entity);
                else
                    return true;
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
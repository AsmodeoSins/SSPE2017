using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cVisitaExterna : EntityManagerServer<VISITA_EXTERNA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cVisitaExterna()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VISITA_EXTERNA"</returns>
        public ObservableCollection<VISITA_EXTERNA> ObtenerXFecha(DateTime fecha)
        {
            ObservableCollection<VISITA_EXTERNA> etnias;
            var Resultado = new List<VISITA_EXTERNA>();
            try
            {
                Resultado = GetData().Where(w => w.FEC_ENTRADA >= fecha && w.FEC_SALIDA <= fecha)
                    .OrderBy(o => o.FEC_ENTRADA).ThenBy(t => t.FEC_SALIDA).ToList();
                etnias = new ObservableCollection<VISITA_EXTERNA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return etnias;
        }
        public ObservableCollection<VISITA_EXTERNA> ObtenerXFechaYCentro(DateTime fecha, short centro = 0)
        {
            ObservableCollection<VISITA_EXTERNA> etnias;
            var Resultado = new List<VISITA_EXTERNA>();
            try
            {
                var f2 = DateTime.Parse(fecha.AddDays(1).ToString());
                Resultado = GetData().Where(w => w.FEC_ENTRADA >= fecha && w.FEC_SALIDA == null ? true : w.FEC_SALIDA < f2 && w.ID_CENTRO == centro)
                    .OrderBy(o => o.FEC_ENTRADA).ThenBy(t => t.FEC_SALIDA).ToList();
                etnias = new ObservableCollection<VISITA_EXTERNA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return etnias;
        }
        public ObservableCollection<VISITA_EXTERNA> ObtenerXFechaCentroYPersona(DateTime fecha, short centro = 0, int persona = 0)
        {
            ObservableCollection<VISITA_EXTERNA> etnias;
            var Resultado = new List<VISITA_EXTERNA>();
            try
            {
                Resultado = GetData().Where(w => w.FEC_ENTRADA >= fecha && w.FEC_SALIDA <= fecha && w.ID_CENTRO == centro && w.ID_PERSONA == persona)
                    .OrderBy(o => o.FEC_ENTRADA).ThenBy(t => t.FEC_SALIDA).ToList();
                etnias = new ObservableCollection<VISITA_EXTERNA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return etnias;
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(VISITA_EXTERNA Entity)
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
        public bool Insertar(List<VISITA_EXTERNA> Entity, short centro = 0, int persona = 0)
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public void Actualizar(VISITA_EXTERNA Entity)
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
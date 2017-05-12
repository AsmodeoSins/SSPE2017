using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAcompanante : EntityManagerServer<ACOMPANANTE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAcompanante()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ESTATUS_VISITA"</returns>
        public ObservableCollection<ACOMPANANTE> ObtenerTodos()
        {
            ObservableCollection<ACOMPANANTE> acompanantes;
            var Resultado = new List<ACOMPANANTE>();
            try
            {
                Resultado = GetData().ToList();
                acompanantes = new ObservableCollection<ACOMPANANTE>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return acompanantes;
        }
        public ObservableCollection<ACOMPANANTE> ObtenerXAcompanantesYVisitante(short centroV, short anioV, int imputadoV, short ingresoV, int personaV,
                                                                                short centroA, short anioA, int imputadoA, short ingresoA, int personaA)
        {
            ObservableCollection<ACOMPANANTE> acompanantes;
            var Resultado = new List<ACOMPANANTE>();
            try
            {
                Resultado = GetData().Where(w => w.VIS_ID_CENTRO == centroV && w.VIS_ID_ANIO == anioV && w.VIS_ID_IMPUTADO == imputadoV
                                                 && w.VIS_ID_INGRESO == ingresoV && w.ID_VISITANTE == personaV &&
                                                 w.ACO_ID_CENTRO == centroA && w.ACO_ID_ANIO == anioA && w.ACO_ID_IMPUTADO == imputadoA
                                                 && w.ACO_ID_INGRESO == ingresoA && w.ID_ACOMPANANTE == personaA).ToList();
                acompanantes = new ObservableCollection<ACOMPANANTE>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return acompanantes;
        }
        public IQueryable<ACOMPANANTE> ObtenerAcompanantesXVisitante(short centro, short anio, int imputado, short ingreso, int persona)
        {
            try
            {
                return GetData().Where(w => w.VIS_ID_CENTRO == centro && w.VIS_ID_ANIO == anio && w.VIS_ID_IMPUTADO == imputado
                                                 && w.VIS_ID_INGRESO == ingreso && w.ID_VISITANTE == persona);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public ObservableCollection<ACOMPANANTE> ObtenerVisitantesXAcompanante(short centro, short anio, int imputado, short ingreso, int persona)
        {
            ObservableCollection<ACOMPANANTE> acompanantes;
            var Resultado = new List<ACOMPANANTE>();
            try
            {
                Resultado = GetData().Where(w => w.ACO_ID_CENTRO == centro && w.ACO_ID_ANIO == anio && w.ACO_ID_IMPUTADO == imputado
                                                 && w.ACO_ID_INGRESO == ingreso && w.ID_ACOMPANANTE == persona).ToList();
                acompanantes = new ObservableCollection<ACOMPANANTE>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return acompanantes;
        }
        public ObservableCollection<ACOMPANANTE> ObtenerXAcompanante(int Id)
        {
            var Resultado = new ObservableCollection<ACOMPANANTE>();
            try
            {
                Resultado = new ObservableCollection<ACOMPANANTE>(GetData().Where(w => w.ID_ACOMPANANTE == Id).ToList());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public ObservableCollection<ACOMPANANTE> ObtenerXAcompanado(int Id)
        {
            var Resultado = new ObservableCollection<ACOMPANANTE>();
            try
            {
                Resultado = new ObservableCollection<ACOMPANANTE>(GetData().Where(w => w.ID_VISITANTE == Id).ToList());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<ACOMPANANTE> Obtener(int Id)
        {
            var Resultado = new List<ACOMPANANTE>();
            try
            {
                Resultado = GetData().Where(w => w.ID_ACOMPANANTE == Id).ToList();
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
        public bool Insertar(ACOMPANANTE Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_ESTATUS_VISITA = GetSequence<short>("ESTATUS_VISITA_SEQ");
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool InsertarLista(List<ACOMPANANTE> Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_ESTATUS_VISITA = GetSequence<short>("ESTATUS_VISITA_SEQ");
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
        public bool Actualizar(ACOMPANANTE Entity)
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

        public bool ActualizarListas(List<ACOMPANANTE> Entity)
        {
            try
            {
                var band = true;
                var resul = true;
                foreach (var item in Entity)
                {
                    band = GetData().Where(w => w.VIS_ID_CENTRO == item.VIS_ID_CENTRO && 
                                                w.VIS_ID_ANIO == item.VIS_ID_ANIO && 
                                                w.VIS_ID_IMPUTADO == item.VIS_ID_IMPUTADO && 
                                                w.VIS_ID_INGRESO == item.VIS_ID_INGRESO && 
                                                w.ID_VISITANTE == item.ID_VISITANTE &&
                                                w.ACO_ID_CENTRO == item.ACO_ID_CENTRO &&
                                                w.ACO_ID_ANIO == item.ACO_ID_ANIO &&
                                                w.ACO_ID_IMPUTADO == item.ACO_ID_IMPUTADO &&
                                                w.ACO_ID_INGRESO == item.ACO_ID_INGRESO && 
                                                w.ID_ACOMPANANTE == item.ID_ACOMPANANTE).Any();
                    if (band)
                    {
                        item.FEC_REGISTRO = DateTime.Parse(GetFechaServer());
                        resul = Update(item);
                    }
                    else
                        resul = Insert(item);
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
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ACOMPANANTE == Id).SingleOrDefault();
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
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAbogadoCausaPenal : EntityManagerServer<ABOGADO_CAUSA_PENAL>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAbogadoCausaPenal()
        { }
        /// <summary>
        /// metodo que se conecta a la base de datos para obtener el padron de ingresos asignados a abogados de un centro
        /// </summary>
        /// <param name="id_centro">llave del centro actual</param>
        /// <param name="estatus_visita_activo">valor del id para el estatus de visita autorizado</param>
        /// <returns>IQueryable&lt;ABOGADO&gt;</returns>
        public IQueryable<ABOGADO_CAUSA_PENAL> ObtenerAsignados(short id_centro, short estatus_visita_autorizado)
        {
            try
            {
                return GetData(w =>(w.ABOGADO_INGRESO.ABOGADO.ID_ABOGADO_TITULO=="T" || w.ABOGADO_INGRESO.ABOGADO.ID_ABOGADO_TITULO=="C") &&  w.ABOGADO_INGRESO.ID_ESTATUS_VISITA == estatus_visita_autorizado && w.ABOGADO_INGRESO.INGRESO.ID_UB_CENTRO == id_centro
                    && w.ABOGADO_INGRESO.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 4 && w.ABOGADO_INGRESO.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 5 && w.ABOGADO_INGRESO.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 6 && w.ABOGADO_INGRESO.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 7);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VisitaDocumento"</returns>
        public ObservableCollection<ABOGADO_CAUSA_PENAL> ObtenerTodos(short centro, short anio, int imputado, short ingreso, int causa_penal, int abogado)
        {
            ObservableCollection<ABOGADO_CAUSA_PENAL> VisitaDocumentos;
            var Resultado = new List<ABOGADO_CAUSA_PENAL>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso
                    && w.ID_CAUSA_PENAL == causa_penal && w.ID_ABOGADO == abogado).ToList();
                VisitaDocumentos = new ObservableCollection<ABOGADO_CAUSA_PENAL>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VisitaDocumentos;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<ABOGADO_CAUSA_PENAL> ObtenerXAbogado(int Id)
        {
            var Resultado = new List<ABOGADO_CAUSA_PENAL>();
            try
            {
                //Resultado = GetData().Where(w => w.ID_OFICIO_ASIGNACION == Id).FirstOrDefault();
                Resultado = GetData().Where(w => w.ID_ABOGADO == Id).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public List<ABOGADO_CAUSA_PENAL> ObtenerXAbogadoTitular(int Id)
        {
            var Resultado = new List<ABOGADO_CAUSA_PENAL>();
            try
            {
                //Resultado = GetData().Where(w => w.ID_OFICIO_ASIGNACION == Id).FirstOrDefault();
                Resultado = GetData().Where(w => w.ABOGADO_INGRESO.ABOGADO.ABOGADO_TITULAR == Id).ToList();
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
        public bool Insertar(List<ABOGADO_CAUSA_PENAL> Entity)
        {
            try
            {
                Entity = Entity.Select(s => new ABOGADO_CAUSA_PENAL
                {
                    ID_ABOGADO = s.ID_ABOGADO,
                    ID_ABOGADO_TITULO = s.ID_ABOGADO_TITULO,
                    ID_ANIO = s.ID_ANIO,
                    ID_CAUSA_PENAL = s.ID_CAUSA_PENAL,
                    ID_CENTRO = s.ID_CENTRO,
                    ID_IMPUTADO = s.ID_IMPUTADO,
                    ID_INGRESO = s.ID_INGRESO,
                    CAPTURA_FEC = s.CAPTURA_FEC,
                    ID_ESTATUS_VISITA = s.ID_ESTATUS_VISITA
                }).ToList();
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool Insertar(ABOGADO_CAUSA_PENAL Entity)
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
        public bool CancelarAbogados(int abogado, short centro, short anio, int imputado, short ingreso, short causaPenal, short estatusCancelar, short estatusAutorizado)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso &&
                    w.ID_CAUSA_PENAL == causaPenal && w.ID_ESTATUS_VISITA == estatusAutorizado && w.ABOGADO_INGRESO.ABOGADO.ABOGADO_TITULAR == abogado).ToList().Select(s => new ABOGADO_CAUSA_PENAL
                    {
                        CAPTURA_FEC = s.CAPTURA_FEC,
                        ID_ABOGADO = s.ID_ABOGADO,
                        ID_ABOGADO_TITULO = s.ID_ABOGADO_TITULO,
                        ID_CENTRO = s.ID_CENTRO,
                        ID_ANIO = s.ID_ANIO,
                        ID_IMPUTADO = s.ID_IMPUTADO,
                        ID_INGRESO = s.ID_INGRESO,
                        ID_CAUSA_PENAL = s.ID_CAUSA_PENAL,
                        ID_ESTATUS_VISITA = estatusCancelar
                    });
                return Update(Entity.ToList());
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }
        public bool Actualizar(ABOGADO_CAUSA_PENAL Entity)
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
        public bool Actualizar(List<ABOGADO_CAUSA_PENAL> Entity)
        {
            try
            {
                Entity = Entity.Select(s => new ABOGADO_CAUSA_PENAL
                {
                    ID_ABOGADO = s.ID_ABOGADO,
                    ID_ABOGADO_TITULO = s.ID_ABOGADO_TITULO,
                    ID_ANIO = s.ID_ANIO,
                    ID_CAUSA_PENAL = s.ID_CAUSA_PENAL,
                    ID_CENTRO = s.ID_CENTRO,
                    ID_IMPUTADO = s.ID_IMPUTADO,
                    ID_INGRESO = s.ID_INGRESO,
                    CAPTURA_FEC = s.CAPTURA_FEC,
                    ID_ESTATUS_VISITA = s.ID_ESTATUS_VISITA
                }).ToList();
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
        public bool ActualizarLista(List<ABOGADO_CAUSA_PENAL> Entity)
        {
            try
            {
                var band = true;
                var resul = true;
                foreach (var item in Entity)
                {
                    band = GetData().Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_CAUSA_PENAL == item.ID_CAUSA_PENAL &&
                        w.ID_INGRESO == item.ID_INGRESO && w.ID_ABOGADO == item.ID_ABOGADO && w.ID_ABOGADO_TITULO == item.ID_ABOGADO_TITULO).Any();
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
        public bool Eliminar(ABOGADO_CAUSA_PENAL Id)
        {
            try
            {
                var Entity = GetData().Where(w => w == Id).SingleOrDefault();
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
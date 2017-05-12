using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using LinqKit;
using SSP.Servidor.ModelosExtendidos;
using System.Data.Objects;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cMensaje : EntityManagerServer<MENSAJE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cMensaje()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "APODO"</returns>
       public IQueryable<MENSAJE> ObtenerTodos()
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

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "APODO"</returns>
        public IQueryable<MENSAJE> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_MENSAJE == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ALIAS" con valores a insertar</param>
        public bool Insertar(MENSAJE Entity)
        {
            try
            {
                //CREAR SECUENCIA
                Entity.ID_MENSAJE = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                if (Insert(Entity))
                    return true;
                else
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
        /// <param name="Entity">objeto de tipo "APODO" con valores a actualizar</param>
        public void Actualizar(MENSAJE Entity)
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
                var ListEntity = GetData().Where(w => w.ID_MENSAJE == Id);//.SingleOrDefault();
                foreach (var entity in ListEntity)
                {
                    Delete(entity);
                }
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
            return false;
        }


        public IQueryable<DOCUMENTO_SISTEMA> SeleccionarMensajes(short id_anio, short id_centro, int id_imputado, short id_ingreso,
            short tipoMensaje,string nuc="", string causapenal="" ,DateTime? fechaInicio=null, DateTime? fechaFinal=null)
        {

            var _res = (from n in this.Context.NUC
                        from m in this.Context.MENSAJE
                        from cp in this.Context.CAUSA_PENAL
                        where
                        m.ID_MEN_TIPO==tipoMensaje && n.ID_ANIO==id_anio && n.ID_CENTRO==id_centro && n.ID_IMPUTADO==id_imputado && n.ID_INGRESO==id_ingreso &&
                        n.ID_NUC == m.NUC && cp.ID_CENTRO == n.ID_CENTRO && cp.ID_ANIO == n.ID_ANIO && cp.ID_IMPUTADO == n.ID_IMPUTADO && cp.ID_INGRESO == n.ID_INGRESO && cp.ID_CAUSA_PENAL == n.ID_CAUSA_PENAL
                        select new DOCUMENTO_SISTEMA {
                            CP_ANIO=cp.CP_ANIO,
                            CP_FOLIO=cp.CP_FOLIO,
                            DOCUMENTO=m.INTER_DOCTO.DOCTO,
                            FORMATO_DOCUMENTO=m.INTER_DOCTO.ID_FORMATO,
                            MENSAJE=m.CONTENIDO,
                            NUC=m.NUC.Trim(),
                            REGISTRO_FEC=m.REGISTRO_FEC,
                            CAUSA_PENAL=cp
                        });
            if (!string.IsNullOrWhiteSpace(nuc) || (!string.IsNullOrWhiteSpace(causapenal) && causapenal.Length > 5 && causapenal.Length<=10) || fechaInicio.HasValue || fechaFinal.HasValue)
                {
                var predicate = PredicateBuilder.True<DOCUMENTO_SISTEMA>();
                if (!string.IsNullOrWhiteSpace(nuc))
                    predicate = predicate.And(w => w.NUC == nuc);
                if (!string.IsNullOrWhiteSpace(causapenal) && causapenal.Length >5 && causapenal.Length<=10)
                    {
                    int _anio = 0;
                    int _folio = 0;
                    _anio = int.Parse(causapenal.Substring(0, 4));
                    _folio = int.Parse(causapenal.Substring(5, causapenal.Length-5));
                    predicate = predicate.And(w => w.CP_ANIO == _anio && w.CP_FOLIO == _folio);
                }
                if (fechaInicio.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) >= fechaInicio.Value);
                if (fechaFinal.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= fechaFinal.Value);
                return _res.Where(predicate.Expand());
            
            }
            else
            {
                return _res;
            }
        }
    }
}
using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPasesVisitanteIngreso : EntityManagerServer<VISITANTE_INGRESO_PASE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cPasesVisitanteIngreso()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VISITANTE_INGRESO_PASE"</returns>
        public ObservableCollection<VISITANTE_INGRESO_PASE> ObtenerTodos()
        {
            var VIP = new ObservableCollection<VISITANTE_INGRESO_PASE>();
            try
            {
                VIP = new ObservableCollection<VISITANTE_INGRESO_PASE>(GetData().ToList());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VIP;
        }
        public ObservableCollection<VISITANTE_INGRESO_PASE> ObtenerPasesXAutorizarXCentro(short centro)
        {
            var VIP = new ObservableCollection<VISITANTE_INGRESO_PASE>();
            try
            {
                VIP = new ObservableCollection<VISITANTE_INGRESO_PASE>(GetData().Where(w => w.ID_CENTRO == centro /*&& w.AUTORIZADO == null*/).ToList());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VIP;
        }
        public ObservableCollection<VISITANTE_INGRESO_PASE> ObtenerPasesXAutorizarXCentroYDocumento(short tipo_documento, short centro, int Pagina = 1)
        {
            var VIP = new ObservableCollection<VISITANTE_INGRESO_PASE>();
            try
            {
                VIP = new ObservableCollection<VISITANTE_INGRESO_PASE>(GetData().Where(w => w.ID_CENTRO == centro &&
                    tipo_documento == 0 ? true : tipo_documento > 0 ? w.ID_PASE == tipo_documento : false /*&& w.AUTORIZADO == null*/)
                    .OrderBy(o => o.FECHA_ALTA).Take((Pagina * 20)).Skip((Pagina == 1 ? 0 : ((Pagina * 20) - 20))).ToList());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VIP;
        }
        public ObservableCollection<VISITANTE_INGRESO_PASE> ObtenerPasesXAutorizarXCentroDocumentoYFecha(short tipo_documento,
            short centro, DateTime? fechaInicial, DateTime? fechaFinal, string autoriza, int Pagina = 1)
        {
            var VIP = new List<VISITANTE_INGRESO_PASE>().AsQueryable();
            try
            {
                var fechaF = fechaFinal.HasValue ? fechaFinal.Value.AddDays(1) : new Nullable<DateTime>();
                if (fechaInicial.HasValue)
                {
                    if (fechaF.HasValue)
                    {
                        VIP = GetData().Where(w => w.ID_CENTRO == centro &&
                            w.FECHA_ALTA >= fechaInicial.Value && w.FECHA_ALTA <= fechaF.Value &&
                            (tipo_documento == 0 ? tipo_documento == 0 : tipo_documento > 0 ? w.ID_PASE == tipo_documento : tipo_documento < 0) &&
                            (autoriza == "T" ? w.AUTORIZADO != autoriza || w.AUTORIZADO == null : autoriza == "null" ? w.AUTORIZADO == null : w.AUTORIZADO == autoriza));
                    }
                    else
                    {
                        VIP = GetData().Where(w => w.ID_CENTRO == centro &&
                            w.FECHA_ALTA >= fechaInicial.Value &&
                            (tipo_documento == 0 ? tipo_documento == 0 : tipo_documento > 0 ? w.ID_PASE == tipo_documento : tipo_documento < 0) &&
                            (autoriza == "T" ? w.AUTORIZADO != autoriza || w.AUTORIZADO == null : autoriza == "null" ? w.AUTORIZADO == null : w.AUTORIZADO == autoriza));
                    }
                }
                else
                {
                    if (fechaF.HasValue)
                    {
                        VIP = GetData().Where(w => w.ID_CENTRO == centro &&
                           w.FECHA_ALTA <= fechaF.Value &&
                            (tipo_documento == 0 ? tipo_documento == 0 : tipo_documento > 0 ? w.ID_PASE == tipo_documento : tipo_documento < 0) &&
                            (autoriza == "T" ? w.AUTORIZADO != autoriza || w.AUTORIZADO == null : autoriza == "null" ? w.AUTORIZADO == null : w.AUTORIZADO == autoriza));
                    }
                    else
                    {
                        VIP = GetData().Where(w => w.ID_CENTRO == centro &&
                            (tipo_documento == 0 ? tipo_documento == 0 : tipo_documento > 0 ? w.ID_PASE == tipo_documento : tipo_documento < 0) &&
                            (autoriza == "T" ? w.AUTORIZADO != autoriza || w.AUTORIZADO == null : autoriza == "null" ? w.AUTORIZADO == null : w.AUTORIZADO == autoriza));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return new ObservableCollection<VISITANTE_INGRESO_PASE>(VIP.OrderByDescending(o => o.FECHA_ALTA).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 20) - 20))).ToList());
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<VISITANTE_INGRESO_PASE> ObtenerXPersona(int Id)
        {
            var Resultado = new List<VISITANTE_INGRESO_PASE>();
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
        public IQueryable<VISITANTE_INGRESO_PASE> ObtenerXPersonaEIngreso(short centro, short anio, int imputado, short ingreso, int persona)
        {
            try
            {
                return GetData().Where(w => w.ID_PERSONA == persona && w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado &&
                    w.ID_INGRESO == ingreso);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public int ObtenerSiguienteConsecutivo(short centro, short anio, int imputado, short ingreso, int persona)
        {
            var Resultado = 0;
            try
            {
                Resultado = GetData().Where(w => w.ID_PERSONA == persona && w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado &&
                    w.ID_INGRESO == ingreso).Count();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public List<VISITANTE_INGRESO_PASE> ObtenerXPase(int Id)
        {
            var Resultado = new List<VISITANTE_INGRESO_PASE>();
            try
            {
                Resultado = GetData().Where(w => w.ID_PASE == Id).ToList();
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
        public bool Insertar(VISITANTE_INGRESO_PASE Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_TIPO_VISITA = GetSequence<short>("TIPO_VISITA_SEQ");
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
        public bool Actualizar(VISITANTE_INGRESO_PASE Entity)
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
        public bool ActualizarLista(List<VISITANTE_INGRESO_PASE> Entity)
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
        public bool Eliminar(VISITANTE_INGRESO_PASE Entity)
        {
            try
            {
                //var Entity = GetData().Where(w => w.ID_PASE == Id).SingleOrDefault();
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
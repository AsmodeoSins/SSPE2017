using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAbogadoIngreso : EntityManagerServer<ABOGADO_INGRESO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAbogadoIngreso()
        { }
        /// <summary>
        /// metodo que se conecta a la base de datos para obtener el padron de ingresos asignados a abogados de un centro
        /// </summary>
        /// <param name="id_centro">llave del centro actual</param>
        /// <param name="estatus_visita_activo">valor del id para el estatus de visita autorizado</param>
        /// <returns>IQueryable&lt;ABOGADO_INGRESO&gt;</returns>
        public IQueryable<ABOGADO_INGRESO> ObtenerAsignados(short id_centro, short estatus_visita_autorizado)
        {
            try
            {
                return GetData(w => w.ID_ESTATUS_VISITA == estatus_visita_autorizado && (w.ABOGADO.ID_ABOGADO_TITULO == "C" || w.ABOGADO.ID_ABOGADO_TITULO == "T") & w.INGRESO.ID_UB_CENTRO == id_centro
                && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 4 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 5 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 6 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 7);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        public IQueryable<ABOGADO_INGRESO> ObtenerAbogadosAsignados(short ID_CENTRO, short ID_ANIO, int ID_IMPUTADO, short ID_INGRESO)
        {
            try
            {
                return GetData(g =>
                    g.ID_CENTRO == ID_CENTRO &&
                    g.ID_ANIO == ID_ANIO &&
                    g.ID_IMPUTADO == ID_IMPUTADO &&
                    g.ID_INGRESO == ID_INGRESO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VisitaDocumento"</returns>
        public ObservableCollection<ABOGADO_INGRESO> ObtenerTodos(short centro, short anio, int imputado, short ingreso, int abogado)
        {
            ObservableCollection<ABOGADO_INGRESO> VisitaDocumentos;
            var Resultado = new List<ABOGADO_INGRESO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso
                    && w.ID_ABOGADO == abogado).ToList();
                VisitaDocumentos = new ObservableCollection<ABOGADO_INGRESO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VisitaDocumentos;
        }


        public ObservableCollection<ABOGADO_INGRESO> ObtenerXAbogadoIngreso(short centro, short anio, int imputado, short ingreso, int abogado, string titulo)
        {
            ObservableCollection<ABOGADO_INGRESO> VisitaDocumentos;
            var Resultado = new List<ABOGADO_INGRESO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso
                    && w.ID_ABOGADO == abogado && w.ID_ABOGADO_TITULO == titulo).ToList();
                VisitaDocumentos = new ObservableCollection<ABOGADO_INGRESO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VisitaDocumentos;
        }
        public ObservableCollection<ABOGADO_INGRESO> ObtenerXIngreso(short centro, short anio, int imputado, short ingreso)
        {
            ObservableCollection<ABOGADO_INGRESO> VisitaDocumentos;
            var Resultado = new List<ABOGADO_INGRESO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso).ToList();
                VisitaDocumentos = new ObservableCollection<ABOGADO_INGRESO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VisitaDocumentos;
        }
        public IQueryable<ABOGADO_INGRESO> ObtenerAdministrativosXIngreso(short? centro = null, short? anio = null, int? imputado = null, short? ingreso = null, int? abogado = null, string titulo = "")
        {
            //var VisitaDocumentos = new ObservableCollection<ABOGADO_INGRESO>();
            //var Resultado = new List<ABOGADO_INGRESO>();
            try
            {
                var predicate = PredicateBuilder.True<ABOGADO_INGRESO>();
                if (centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == centro);
                if (anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == anio);
                if (imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == imputado);
                if (ingreso.HasValue)
                    predicate = predicate.And(w => w.ID_INGRESO == ingreso);
                if (abogado.HasValue)
                    predicate = predicate.And(w => w.ID_ABOGADO == abogado);
                if (!string.IsNullOrEmpty(titulo))
                    predicate = predicate.And(w => w.ID_ABOGADO == abogado);
                predicate = predicate.And(w => w.ID_ESTATUS_VISITA == 13 && w.ABOGADO_ING_DOCTO.Count != 0);
                return GetData(predicate.Expand());
                //return GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso &&
                //    w.ID_ABOGADO != abogado && w.ID_ABOGADO_TITULO == titulo && w.ID_ESTATUS_VISITA == 13 && w.ABOGADO_ING_DOCTO.Count != 0);
                //VisitaDocumentos = new ObservableCollection<ABOGADO_INGRESO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return VisitaDocumentos;
        }
        public ObservableCollection<IMPUTADO> ObtenerImputadosXAbogadoIngreso(string paterno = "", string materno = "", string nombre = "", int? anio = new Nullable<int>(), int? folio = new Nullable<int>(), int abogado = 0)
        {
            var Resultado = new ObservableCollection<IMPUTADO>();
            try
            {
                if (abogado == 0) { }
                else if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(paterno) && string.IsNullOrEmpty(materno) && !anio.HasValue && !folio.HasValue) { }
                else
                {
                    Resultado = new ObservableCollection<IMPUTADO>(GetData().Where(w => w.ID_ABOGADO == abogado &&
                         w.INGRESO.IMPUTADO.NOMBRE.Contains(nombre) && w.INGRESO.IMPUTADO.PATERNO.Contains(paterno) && w.INGRESO.IMPUTADO.MATERNO.Contains(materno) &&
                         (anio.HasValue ? w.INGRESO.IMPUTADO.ID_ANIO == anio : false) &&
                         (folio.HasValue ? w.INGRESO.IMPUTADO.ID_IMPUTADO == folio : false)
                        ).Select(s => s.INGRESO.IMPUTADO).ToList());
                }
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
        public List<ABOGADO_INGRESO> ObtenerXAbogado(int Id)
        {
            var Resultado = new List<ABOGADO_INGRESO>();
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
        public List<ABOGADO_INGRESO> ObtenerXAbogadoYTitulo(int Id, string titulo)
        {
            var Resultado = new List<ABOGADO_INGRESO>();
            try
            {
                //Resultado = GetData().Where(w => w.ID_OFICIO_ASIGNACION == Id).FirstOrDefault();
                Resultado = GetData().Where(w => w.ID_ABOGADO == Id && w.ID_ABOGADO_TITULO == titulo).ToList();
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
        public bool Insertar(List<ABOGADO_INGRESO> Entity)
        {
            try
            {
                Entity = Entity.Select(s => new ABOGADO_INGRESO
                {
                    ID_ABOGADO = s.ID_ABOGADO,
                    ID_ANIO = s.ID_ANIO,
                    ID_CENTRO = s.ID_CENTRO,
                    ID_IMPUTADO = s.ID_IMPUTADO,
                    ID_INGRESO = s.ID_INGRESO,
                    CAPTURA_FEC = s.CAPTURA_FEC,
                    ID_ESTATUS_VISITA = s.ID_ESTATUS_VISITA,
                    OBSERV = s.OBSERV
                }).ToList();
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool Insertar(ABOGADO_INGRESO Entity)
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
        public bool Actualizar(ABOGADO_INGRESO Entity)
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
        public bool CancelarXAbogado(int abogado, short estatus, string titulo)
        {
            try
            {
                foreach (var item in ObtenerXAbogadoYTitulo(abogado, titulo))
                {
                    Update(new ABOGADO_INGRESO
                    {
                        ADMINISTRATIVO = item.ADMINISTRATIVO,
                        CAPTURA_FEC = item.CAPTURA_FEC,
                        ID_ABOGADO = item.ID_ABOGADO,
                        ID_ABOGADO_TITULO = item.ID_ABOGADO_TITULO,
                        ID_ANIO = item.ID_ANIO,
                        ID_CENTRO = item.ID_CENTRO,
                        ID_ESTATUS_VISITA = estatus,
                        ID_IMPUTADO = item.ID_IMPUTADO,
                        ID_INGRESO = item.ID_INGRESO,
                        OBSERV = item.OBSERV
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }
        public bool CancelarAdministrativosXIngreso(INGRESO ingreso, ABOGADO abogado)
        {
            try
            {
                foreach (var item in ObtenerAdministrativosXIngreso(ingreso.ID_CENTRO, ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO, abogado.ID_ABOGADO, abogado.ID_ABOGADO_TITULO))
                {
                    Update(new ABOGADO_INGRESO
                    {
                        CAPTURA_FEC = item.CAPTURA_FEC,
                        ID_ABOGADO = item.ID_ABOGADO,
                        ID_ABOGADO_TITULO = item.ID_ABOGADO_TITULO,
                        ID_ANIO = item.ID_ANIO,
                        ID_CENTRO = item.ID_CENTRO,
                        ID_ESTATUS_VISITA = item.ID_ESTATUS_VISITA,
                        ID_IMPUTADO = item.ID_IMPUTADO,
                        ID_INGRESO = item.ID_INGRESO,
                        OBSERV = item.OBSERV,
                        ADMINISTRATIVO = "N"
                    });
                }
                return true;
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
        public bool Eliminar(ABOGADO_INGRESO Id)
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
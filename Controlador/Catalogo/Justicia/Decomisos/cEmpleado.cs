using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using LinqKit;
using System.Transactions;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEmpleado : EntityManagerServer<EMPLEADO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cEmpleado()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<EMPLEADO> ObtenerTodos(short? Centro = null, int? NIP = null, string Paterno = "", string Materno = "", string Nombre = "", bool EsOficial = false,int? Pagina = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<EMPLEADO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (NIP.HasValue)
                    predicate = predicate.And(w => w.ID_EMPLEADO == NIP);
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.PERSONA.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.PERSONA.MATERNO.Contains(Materno));
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.PERSONA.NOMBRE.Contains(Nombre));
                if (EsOficial)
                    predicate = predicate.And(w => w.ID_TIPO_EMPLEADO >= 3 && w.ID_TIPO_EMPLEADO <= 6);
                if(Pagina.HasValue)
                    return GetData(predicate.Expand()).OrderBy(o => o.ID_EMPLEADO).Take((Pagina.Value * 30)).Skip((Pagina.Value == 1 ? 0 : ((Pagina.Value * 30) - 30)));
                else
                    return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<EMPLEADO> ObtenerTodosReporte(short? Centro = null,short? TipoEmpleado = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<EMPLEADO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if(TipoEmpleado.HasValue)
                    if(TipoEmpleado.Value != -1)
                        predicate = predicate.And(w => w.ID_TIPO_EMPLEADO == TipoEmpleado);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<EMPLEADO> ObtenerTodosLosCustodios()
        {
            try
            {
                return GetData(g =>
                    g.ID_DEPARTAMENTO == 4);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <param name="Centro">Id del centro para el filtrado de empleados</param>
        /// <param name="estatus">Estatus para el filtrado de empleados</param>
        /// <param name="id_rol">rol del usuario para el filtrado de empleados</param>
        /// <returns>IQueryable&lt;EMPLEADO&gt;</returns>
        public IQueryable<EMPLEADO> ObtenerTodosEmpleados(short? Centro = null, string estatus="", short? id_rol=null)
        {
            try
            {
                var predicate = PredicateBuilder.True<EMPLEADO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                if (id_rol.HasValue)
                    predicate=predicate.And(w=>w.USUARIO.Any(a=>a.USUARIO_ROL.Any(w2=>w2.ID_ROL==id_rol.Value)));
                return GetData(predicate.Expand());
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
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public EMPLEADO Obtener(long Id)
        {
            try
            {
                return GetData().Where(w => w.ID_EMPLEADO == Id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public EMPLEADO ObtenerEmpleadoPorDepartamento(long Id, short Id_Departamento)
        {
            try
            {
                return GetData().Where(w => w.ID_EMPLEADO == Id && w.ID_DEPARTAMENTO == Id_Departamento).SingleOrDefault();
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
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public IQueryable<EMPLEADO> ObtenerEmpleadoSinUsuario(short? Centro = null, string Paterno = "", string Materno = "", string Nombre = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<EMPLEADO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.PERSONA.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.PERSONA.MATERNO.Contains(Materno));
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.PERSONA.NOMBRE.Contains(Nombre));
                //predicate = predicate.And(w => w.USUARIO == null);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a insertar</param>
        public bool Insertar(EMPLEADO Entity)
        {
            try
            {
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
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public bool Actualizar(EMPLEADO Entity)
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
                var ListEntity = GetData().Where(w => w.ID_EMPLEADO == Id);
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

        /// <summary>
        /// metodo para deshabilitar empleados
        /// </summary>
        /// <param name="id_empleado">id del empleado a deshabilitar</param>
        /// <returns></returns>
        public void DeshabilitarEmpleado(int id_empleado)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _empleado = Context.EMPLEADO.FirstOrDefault(w => w.ID_EMPLEADO == id_empleado); //id_persona es id_empleado
                    if (_empleado == null)
                        throw new Exception("EL EMPLEADO NO EXISTE");
                    foreach (var usr_item in _empleado.USUARIO)
                    {
                        foreach (var depto_acceso_item in usr_item.DEPARTAMENTO_ACCESO)
                        {
                            Context.Entry(depto_acceso_item).State = EntityState.Deleted;
                        }
                        usr_item.ESTATUS = "N";
                        Context.Entry(usr_item).Property(x => x.ESTATUS).IsModified = true;
                    }
                    _empleado.ESTATUS = "N";
                    Context.Entry(_empleado).Property(x => x.ESTATUS).IsModified = true;
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
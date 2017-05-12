using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTecnicaArea : EntityManagerServer<AREA_TECNICA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTecnicaArea()
        { }

        /// <summary>
        /// Obtiene si un usuario es un coordinador tecnico de un area
        /// </summary>
        /// <param name="id_area_tecnica">Llave del area técnica</param>
        /// <param name="usuario">Usuario</param>
        /// <returns>bool</returns>
        /// 
        
        public bool IsUsuarioCoordinadorAreaTecnica(short id_area_tecnica, string usuario)
        {
            try
            {
                //return Context.DEPARTAMENTO_ACCESO.Any(w=>w.ID_USUARIO.Trim()==usuario.Trim() /*&& w.DEPARTAMENTO.ID_TECNICA==id_area_tecnica*/);
                return Context.DEPARTAMENTO_ACCESO.Any(w => w.ID_USUARIO.Trim() == usuario.Trim() && w.DEPARTAMENTO.DEPARTAMENTO_AREA_TECNICA.Any(a=>a.ID_TECNICA==id_area_tecnica));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene todas las areas tecnicas relacionadas con DEPARTAMENTO
        /// </summary>
        /// <param name="id_departamento">Llave del departamento</param>
        /// <returns>IQueryable &lt;AREA_TECNICA&gt;</returns>
        /// 

        public IQueryable<AREA_TECNICA> ObtenerporDepartamento()
        {
            try
            {
               // Context.DEPARTAMENTO.Where(w => w.ID_TECNICA != null).Select(s => s.AREA_TECNICA);
                return Context.DEPARTAMENTO_AREA_TECNICA.Select(s => s.AREA_TECNICA);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        /// 
        public IQueryable<AREA_TECNICA> ObtenerTodos(string buscar = "")
        {
            try
            {
                getDbSet();
                if (string.IsNullOrEmpty(buscar))
                    return GetData();
                else
                    return GetData().Where(w => w.DESCR.Contains(buscar));
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
        public List<AREA_TECNICA> Obtener(int Id)
        {
             var resultado = new List<AREA_TECNICA>();
            try
            {
                resultado = GetData().Where(w => w.ID_TECNICA == Id).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return resultado;
        }
           
        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a insertar</param>

        public void Insertar(AREA_TECNICA Entity)
        {
            try
            {
                Entity.ID_TECNICA = GetSequence<short>("AREA_TECNICA_SEQ");
                Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public bool Actualizar(AREA_TECNICA Entity)
        {
            try
            {
                return (Update(Entity));
                   
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
                var ListEntity = GetData().Where(w => w.ID_TECNICA == Id);
                if (ListEntity != null)
                {
                    foreach (var entity in ListEntity)
                    {
                        if (Delete(entity))
                            return true;
                    }
                    return false;
                }
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


        /// <summary>
        /// Obtiene todas las areas tecnicas relacionadas un rol
        /// </summary>
        /// <param name="id_rol">Llave del rol del usuario</param>
        /// <returns>IQueryable &lt;AREA_TECNICA&gt;</returns>
        /// 

        public IQueryable<AREA_TECNICA> ObtenerporUsuario(string usuario)
        {
            try
            {
                
                return Context.DEPARTAMENTO_AREA_TECNICA.Where(w => w.DEPARTAMENTO.DEPARTAMENTO_ACCESO.Any(a => a.ID_USUARIO.Trim() == usuario.Trim())).Select(s => s.AREA_TECNICA);
                //Context.DEPARTAMENTO.Where(w => w.DEPARTAMENTO_ACCESO!=null /*&& w.DEPARTAMENTO_ACCESO.ID_USUARIO.Trim()==usuario.Trim()*/ && w.ID_DEPARTAMENTO!=null).Select(s => s.AREA_TECNICA);                
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cMunicipio : EntityManagerServer<MUNICIPIO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cMunicipio()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "PAIS_NACIONALIDAD"</returns>
        public IQueryable<MUNICIPIO> ObtenerTodos(string buscar = "")
        {
            try
            {
                if (string.IsNullOrEmpty(buscar))
                    return GetData().OrderBy(o => o.ENTIDAD.DESCR);
                else
                    return GetData().Where(w => w.MUNICIPIO1.Contains(buscar));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
   
        public IQueryable<MUNICIPIO> ObtenerTodos(string buscar = "", short? estado = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(buscar))
                {
                    if (estado == 0)
                    {
                        return GetData().OrderBy(o => o.ID_ENTIDAD);
                    }
                    else
                    {
                        return GetData().Where(w => w.ID_ENTIDAD == estado).OrderBy(o => o.ID_ENTIDAD);
                    }
                }
                else
                {
                    if (estado == 0)
                    {
                       return GetData().Where(w => w.MUNICIPIO1.Contains(buscar)).OrderBy(o => o.ID_ENTIDAD);
                    }
                    else
                    {
                        return GetData().Where(w => w.ID_ENTIDAD == estado && w.MUNICIPIO1.Contains(buscar)).OrderBy(o => o.ID_ENTIDAD);
                    }
                }
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
        /// <returns>objeto de tipo "MUNICIPIO"</returns>
        public IQueryable<MUNICIPIO> Obtener(int Estado, int Municipio = 0)
        {
            var Resultado = new List<MUNICIPIO>();
            try
            {
                if(Municipio > 0)
                    return GetData().Where(w => w.ID_ENTIDAD == Estado && w.ID_MUNICIPIO == Municipio);
                else
                    return GetData().Where(w => w.ID_ENTIDAD == Estado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public void Insertar(MUNICIPIO Entity)
        {
            try
            {
                Entity.ID_MUNICIPIO = GetIDCatalogo<short>("MUNICIPIO");
                Insert(Entity);
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
        public void Actualizar(MUNICIPIO Entity)
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
                var Entity = GetData().Where(w => w.ID_MUNICIPIO == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                //if (ex.InnerException != null)
                //{
                //    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                //        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                //}

                if(ex.Message.Contains("child record found")){
                    return false;
                }
            }
            return false;
             
        }
        
    }
}
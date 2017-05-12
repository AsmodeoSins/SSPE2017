using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Justicia.EstudioMI;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cComportamientoHomo : EntityManagerServer<COMPORTAMIENTO_HOMO>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public cComportamientoHomo() { }

        /// <summary>
        /// Obtener todos los registros de una tabla
        /// </summary>
        /// <param name="buscar">Parametro en string opcional para reducir los datos</param>
        /// <returns></returns>
        public IQueryable<COMPORTAMIENTO_HOMO> ObtenerTodos(string buscar = "")
        {
            var Resultado = new List<COMPORTAMIENTO_HOMO>();
            try
            {
                getDbSet();
                if (string.IsNullOrEmpty(buscar))
                    return GetData().OrderBy(x => x.ID_HOMO);
                else
                    return GetData().Where(w => w.DESCR.Contains(buscar));
            }

            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Regresa un registro en especifico
        /// </summary>
        /// <param name="Id">Id del registro a buscar</param>
        /// <returns></returns>
        public COMPORTAMIENTO_HOMO Obtener(int Id)
        {
            return GetData().FirstOrDefault(w => w.ID_HOMO == Id);
        }

        /// <summary>
        /// Metodo que ingresa un registro nuevo
        /// </summary>
        /// <param name="Entity">Objeto con el registro a insertar</param>
        public void Insertar(COMPORTAMIENTO_HOMO Entity)
        {
            try
            {
                Entity.ID_HOMO = GetSequence<short>("COMPORTAMIENTO_HOMO_SEQ");
                Insert(Entity);
            }

            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }


        /// <summary>
        /// metodo que actualiza un registro
        /// </summary>
        /// <param name="Entity">Objeto con informacion del registro a actualizar</param>
        public void Actualizar(COMPORTAMIENTO_HOMO Entity)
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
        /// metodo que elimina un registro
        /// </summary>
        /// <param name="Id">Id del registro a aliminar</param>
        /// <returns></returns>
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_HOMO == Id).SingleOrDefault();
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using System.Transactions;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSectorClasificacionCelda : EntityManagerServer<SECTOR_OBSERVACION_CELDA>
    {
        #region Constructor
        public cSectorClasificacionCelda() { }
        #endregion
        #region Obtener
        public IQueryable<SECTOR_OBSERVACION_CELDA> ObtenerTodas(int Id)
        {
            
                return GetData().Where(w => w.ID_SECTOR_OBS == Id);
        }






        //public ObservableCollection<SECTOR_OBSERVACION> ObtenerXCentro(short idCentro)
        //{
        //    var celdas = new cCelda().GetData(w => w.ID_CENTRO == idCentro);
        //}
        #endregion
        #region Insertar
        /// <summary>
        /// Inserta en la tabla la entidad enviada.
        /// </summary>
        /// <param name="entidad">Objeto que se desea insertar en la tabla</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Agregar(SECTOR_OBSERVACION_CELDA entidad)
        {
            if (Insert(entidad))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Inserta en la tabla las entidades enviadas.
        /// </summary>
        /// <param name="entidades">Lista de objetos a insertar en la tabla</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Agregar(List<SECTOR_OBSERVACION_CELDA> entidades)
        {
            if (entidades.Count == 0)
                return true;
            else
                if (Insert(entidades))
                    return true;//"Informaci\u00F3n registrada correctamente.";
                else
                    return false;//"No se ha podido guardar la informaci\u00F3n.";
        }

        /// Inserta en la tabla las entidades enviadas.
        /// </summary>
        /// <param name="entidades">Lista de objetos a insertar en la tabla</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Insertar(int Id,List<SECTOR_OBSERVACION_CELDA> entidades)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var celdas = Context.SECTOR_OBSERVACION_CELDA.Where(w => w.ID_SECTOR_OBS == Id);
                    if (celdas != null)
                    {
                        foreach (var c in celdas)
                            Context.SECTOR_OBSERVACION_CELDA.Remove(c);
                    }
                    foreach (var c in entidades)
                    {
                        Context.SECTOR_OBSERVACION_CELDA.Add(c);
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
                //Eliminar(Id);
                //if (entidades.Count == 0)
                //    return true;
                //else
                //    if (Insert(entidades))
                //        return true;//"Informaci\u00F3n registrada correctamente.";
                //    else
                //        return false;//"No se ha podido guardar la informaci\u00F3n.";
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(SECTOR_OBSERVACION_CELDA Entidad)
        {
            try
            {
                if (Update(Entidad))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidades">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(List<SECTOR_OBSERVACION_CELDA> Entidades)
        {
            try
            {
                if (Update(Entidades))
                    return true;//"Informaci\u00F3n actualizada correctamente.";
                else
                    return false;// "No se pudo actualizar la informaci\u00F3n, intente de nuevo.";
            }
            catch (Exception ex)
            {
                return false;// "Ocurri\u00F3 un error y no se pudo actualziar la información.";
            }
        }
        #endregion
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Eliminar(SECTOR_OBSERVACION_CELDA Entidad)
        {
            try
            {
                if (Delete(Entidad))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
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
                var ListEntity = GetData().Where(w => w.ID_SECTOR_OBS == Id);
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
        #endregion
    }
}

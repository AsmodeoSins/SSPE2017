using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cObservacionPlanimetria : EntityManagerServer<SECTOR_OBSERVACION>
    {
        #region Constructor
        public cObservacionPlanimetria() { }
        #endregion
        #region Obtención
        public ObservableCollection<SECTOR_OBSERVACION> ObtenerTodas()
        {
            return new ObservableCollection<SECTOR_OBSERVACION>(GetData());
        }
        //public ObservableCollection<SECTOR_OBSERVACION> ObtenerXCentro(short idCentro)
        //{
        //    var celdas = new cCelda().GetData(w => w.ID_CENTRO == idCentro);
        //}
        #endregion
        #region Inserción
        /// <summary>
        /// Inserta en la tabla la entidad enviada.
        /// </summary>
        /// <param name="entidad">Objeto que se desea insertar en la tabla</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public int Agregar(SECTOR_OBSERVACION entidad)
        {
            entidad.ID_SECTOR_OBS = GetIDProceso<short>("SECTOR_OBSERVACION", "ID_SECTOR_OBS", "1=1");
            if (Insert(entidad))
                //return "Informaci\u00F3n registrada correctamente.";
                return entidad.ID_SECTOR_OBS;
            else
                //return "No se ha podido guardar la informaci\u00F3n.";
                return 0;
        }
        /// <summary>
        /// Inserta en la tabla las entidades enviadas.
        /// </summary>
        /// <param name="entidades">Lista de objetos a insertar en la tabla</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Agregar(List<SECTOR_OBSERVACION> entidades)
        {
            if (Insert(entidades))
                return "Informaci\u00F3n registrada correctamente.";
            else
                return "No se ha podido guardar la informaci\u00F3n.";
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(SECTOR_OBSERVACION Entidad)
        {
            try
            {
                if (Update(Entidad))
                    return true;//"Informaci\u00F3n actualizada correctamente.";
                else
                    return false;// "No se pudo actualizar la informaci\u00F3n, intente de nuevo.";
            }
            catch (Exception ex)
            {
                return false;//"Ocurri\u00F3 un error y no se pudo actualizar la información.";
            }
        }
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidades">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Actualizar(List<SECTOR_OBSERVACION> Entidades)
        {
            try
            {
                if (Update(Entidades))
                    return "Informaci\u00F3n actualizada correctamente.";
                else
                    return "No se pudo actualizar la informaci\u00F3n, intente de nuevo.";
            }
            catch (Exception ex)
            {
                return "Ocurri\u00F3 un error y no se pudo actualziar la información.";
            }
        }
        #endregion
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Eliminar(SECTOR_OBSERVACION Entidad)
        {
            try
            {
                if (Delete(Entidad))
                    return "Informaci\u00F3n eliminada correctamente";
                else
                    return "No se pudo eliminar la informaci\u00F3n, intente de nuevo.";
            }
            catch (Exception ex)
            {
                return "Ocurri\u00F3 un error y no se pudo eliminar la información.";
            }
        }

        public bool Eliminar(int Id)
        {
            try
            {
                new cSectorClasificacionCelda().Eliminar(Id);
                var ListEntity = GetData().Where(w => w.ID_SECTOR_OBS == Id);
                foreach (var entity in ListEntity)
                {
                    Delete(entity);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
       
      

        ///// <summary>
        ///// Método que elimina una entidad de la BD.
        ///// </summary>
        ///// <param name="Entidad">Entidad a eliminar.</param>
        ///// <returns>Cadena de texto con el resultado de la operación.</returns>
        //public string Eliminar(int idEmi, int idCons)
        //{
        //    try
        //    {
        //        if (Delete(GetData(w => w.ID_EMI == idEmi && w.ID_EMI_CONS == idCons).ToList()))
        //            return "Informaci\u00F3n eliminada correctamente";
        //        else
        //            return "No se pudo eliminar la informaci\u00F3n, intente de nuevo.";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Ocurri\u00F3 un error: " + ex.Message;
        //    }
        //}
        #endregion
    }
}

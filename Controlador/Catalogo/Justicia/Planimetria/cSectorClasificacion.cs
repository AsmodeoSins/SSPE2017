using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSectorClasificacion : EntityManagerServer<SECTOR_CLASIFICACION>
    {
        #region Constructor
        public cSectorClasificacion() { }
        #endregion
        #region Obtener
        public IQueryable<SECTOR_CLASIFICACION> ObtenerGruposVulnerables(string estatus="S")
        {
            try
            {
                var predicate = PredicateBuilder.True<SECTOR_CLASIFICACION>();
                predicate = predicate.And(w=>w.ES_GRUPO_VULNERABLE.HasValue && w.ES_GRUPO_VULNERABLE==1);
                if (!string.IsNullOrWhiteSpace(estatus))
                {
                    predicate = predicate.And(w=>w.ESTATUS=="S");
                }
                return GetData(predicate.Expand());
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        public IQueryable<SECTOR_CLASIFICACION> ObtenerTodas(string Buscar = "")
        {
            if(!string.IsNullOrEmpty(Buscar))
                return GetData().Where(w => w.POBLACION.Contains(Buscar));
            else
                return GetData();
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
        public int Agregar(SECTOR_CLASIFICACION entidad)
        {
            entidad.ID_SECTOR_CLAS = GetIDProceso<short>("SECTOR_CLASIFICACION", "ID_SECTOR_CLAS", "1=1");
            if (Insert(entidad))
                return entidad.ID_SECTOR_CLAS;
            else
                return 0;
        }
        /// <summary>
        /// Inserta en la tabla las entidades enviadas.
        /// </summary>
        /// <param name="entidades">Lista de objetos a insertar en la tabla</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Agregar(List<SECTOR_CLASIFICACION> entidades)
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
        public bool Actualizar(SECTOR_CLASIFICACION Entidad)
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
        public string Actualizar(List<SECTOR_CLASIFICACION> Entidades)
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
        public bool Eliminar(SECTOR_CLASIFICACION Entidad)
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


        #endregion
    }
}

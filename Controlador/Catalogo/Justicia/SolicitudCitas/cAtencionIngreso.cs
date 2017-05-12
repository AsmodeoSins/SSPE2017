using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;
using System.Collections.Generic;
using System.Data.Objects;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAtencionIngreso : EntityManagerServer<ATENCION_INGRESO>
    {
        #region Constructor
        public cAtencionIngreso() { }
        #endregion
        #region Obtener
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CentroUser">Cnetro del Usuario</param>
        /// <param name="FechaInicio">filtro Fecha Inicio</param>
        /// <param name="FechaFin">Fecha Fin</param>
        /// <param name="Estatus">Estatus Seleccionado</param>
        /// <returns></returns>
        public IQueryable<ATENCION_INGRESO> ObtenerSolicitudAtencionEstatus(short? CentroUser,DateTime? FechaInicio, DateTime? FechaFin,short? Estatus=null)
        {
            var predicate = PredicateBuilder.True<ATENCION_INGRESO>();
            //TODOS LOS ESTATUS cuando Estatus.Value es igual a 3
         
           predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == CentroUser &&EntityFunctions.TruncateTime( w.REGISTRO_FEC)>= EntityFunctions.TruncateTime( FechaInicio)&& EntityFunctions.TruncateTime( w.REGISTRO_FEC)<= EntityFunctions.TruncateTime( FechaFin));
            switch (Estatus.Value)
            {
                case 2://<---------------------------ATENDIDA ----------------------->
                    predicate = predicate.And(w => w.ESTATUS == 1);
                    return GetData(predicate.Expand());
                
                case 3://<--------------------------NO ATENDIDA -------------------->
                    predicate = predicate.And(w => w.ESTATUS != 1);
                    return GetData(predicate.Expand());
              
              
                case 1://--------------------------SOLICITADAS----------------------------------------->
                    return GetData(predicate.Expand());
                  

            }
            
            //<------------------------------------TODOS
            return  GetData(predicate.Expand());
        }
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <param name="SoloActivos">Si la consulta va a ser solo sobre los registros activos</param>
        /// <param name="Anio">ID_ANIO del imputado</param>
        /// <param name="Areas">IDs de areas donde se va a realizar la busqueda</param>
        /// <param name="Centro">ID_CENTRO del imputado</param>
        /// <param name="Centro_Ubicacion">ID del centro que esta realizando la consulta</param>
        /// <param name="Estatus">Estatus del registro de ATENCION_INGRESO</param>
        /// <param name="Fecha">Fecha a partir de la cual se hace la consulta cuando se buscan solo agendadas</param>
        /// <param name="FechaHoy">Fecha actual</param>
        /// <param name="Folio">ID_IMPUTADO del imputado</param>
        /// <param name="Materno">Apellido materno del imputado</param>
        /// <param name="Nombre">Nombre del imputado</param>
        /// <param name="Pagina">Pagina de la consulta</param>
        /// <param name="Paterno">Apellidp paterno del imputado</param>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<ATENCION_INGRESO> ObtenerTodoEnAreas(short Centro_Ubicacion, bool SoloActivos = true, short? Centro = null, short? Anio = null, int? Folio = null, string Nombre = "", string Paterno = "", string Materno = "", List<short> Areas = null, DateTime? Fecha = null, short? Estatus = null, DateTime? FechaHoy = null, int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_INGRESO>();
                var predicateOR = PredicateBuilder.False<ATENCION_INGRESO>();
                predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == Centro_Ubicacion);
                predicate = predicate.And(w => w.ID_CENTRO_UBI == Centro_Ubicacion);
                if (SoloActivos)
                    predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 4 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 5 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 6 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 7);
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ATENCION_SOLICITUD.ID_CENTRO == Centro);
                if (Fecha.HasValue) 
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ATENCION_SOLICITUD.SOLICITUD_FEC) == EntityFunctions.TruncateTime(Fecha));
                if (Areas != null)
                    foreach (var item in Areas)
                        predicateOR = predicateOR.Or(w => w.ATENCION_SOLICITUD.ID_TECNICA == item);
                    predicate = predicate.And(predicateOR.Expand());
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Folio.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Folio);
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.NOMBRE.Contains(Nombre));
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.MATERNO.Contains(Materno));
                if (Estatus.HasValue)
                {
                    predicate = predicate.And(w => w.ESTATUS == Estatus);
                    if (Estatus == 1)/*agendadas*/
                    {
                        if (!Fecha.HasValue)
                        {
                            var hoy = FechaHoy.Value.Date;
                            predicate = predicate.And(w => w.ATENCION_SOLICITUD.SOLICITUD_FEC >= hoy);
                        }
                    }
                }

                return GetData(predicate.Expand()).OrderByDescending(w => new { w.ATENCION_SOLICITUD.SOLICITUD_FEC, w.ID_ANIO, w.ID_IMPUTADO }).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30))); ;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <param name="SoloActivos">Si la consulta va a ser solo sobre los registros activos</param>
        /// <param name="Anio">ID_ANIO del imputado</param>
        /// <param name="Area">ID del area tecnica</param>
        /// <param name="Centro">ID_CENTRO del imputado</param>
        /// <param name="Centro_Ubicacion">ID del centro que esta realizando la consulta</param>
        /// <param name="Estatus">Estatus del registro de ATENCION_INGRESO</param>
        /// <param name="Fecha">Fecha a partir de la cual se hace la consulta cuando se buscan solo agendadas</param>
        /// <param name="FechaHoy">Fecha actual</param>
        /// <param name="Folio">ID_IMPUTADO del imputado</param>
        /// <param name="Materno">Apellido materno del imputado</param>
        /// <param name="Nombre">Nombre del imputado</param>
        /// <param name="Pagina">Pagina de la consulta</param>
        /// <param name="Paterno">Apellidp paterno del imputado</param>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<ATENCION_INGRESO> ObtenerTodo(short Centro_Ubicacion,bool SoloActivos=true,short? Centro = null, short? Anio = null, int? Folio = null, string Nombre = "", string Paterno = "", string Materno = "", short? Area = null, DateTime? Fecha = null,short? Estatus = null,DateTime? FechaHoy = null, int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_INGRESO>();
                predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == Centro_Ubicacion);
                if (SoloActivos)
                    predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 4 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 5 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 6 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 7);
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ATENCION_SOLICITUD.ID_CENTRO == Centro);
                if (Fecha.HasValue)
                    predicate = predicate.And(w => w.ATENCION_SOLICITUD.SOLICITUD_FEC == Fecha);
                if (Area.HasValue)
                    predicate = predicate.And(w => w.ATENCION_SOLICITUD.ID_TECNICA == Area);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Folio.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Folio);
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.NOMBRE.Contains(Nombre));
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.MATERNO.Contains(Materno));
                if (Estatus.HasValue)
                { 
                    predicate = predicate.And(w => w.ESTATUS == Estatus);
                    if (Estatus == 1)/*agendadas*/
                    {
                        if (!Fecha.HasValue)
                        {
                            var hoy = FechaHoy.Value.Date;
                            predicate = predicate.And(w => w.ATENCION_SOLICITUD.SOLICITUD_FEC >= hoy); 
                        }
                    }
                }
                
                return GetData(predicate.Expand()).OrderByDescending(w => new { w.ATENCION_SOLICITUD.SOLICITUD_FEC,w.ID_ANIO, w.ID_IMPUTADO }).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public ATENCION_INGRESO Obtener(int id)
        {
            try
            {
                return GetData(w=>w.ID_ATENCION == id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el numero de solicitudes en el mes actual.
        /// </summary>
        /// <returns>Numero de solicitudes</returns>
        public int ObtenerSolicitudesPorMes(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null,int? AnioBuscar = null, int? MesBuscar = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_INGRESO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Anio);
                if (Ingreso.HasValue)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (AnioBuscar.HasValue)
                    predicate = predicate.And(w => w.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.Year == AnioBuscar);
                if (MesBuscar.HasValue)
                    predicate = predicate.And(w => w.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.Month == MesBuscar);
                return GetData().Count(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }        
        
        #endregion
        #region Insercion
        /// <summary>
        /// Método de inserción
        /// </summary>
        /// <param name="Entidad">Entidad a guardar en la tabla</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public bool Agregar(ATENCION_INGRESO Entidad)
        {
            try
            {

                if (Insert(Entidad))
                    return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(ATENCION_INGRESO Entidad)
        {
            try
            {
                if (Update(Entidad))
                    return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }
        #endregion
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Eliminar(ATENCION_INGRESO Entidad)
        {
            try
            {
                if (Delete(Entidad))
                    return true;
            }
            catch (Exception ex)
            {
                
            }
            return false;
        }
        #endregion
    }
}

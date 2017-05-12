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
    public class cCatalogoEquipos: EntityManagerServer<CATALOGO_EQUIPOS>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cCatalogoEquipos()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>Listado Catalogo de Equipos</returns>
        public IQueryable<CATALOGO_EQUIPOS> ObtenerTodos(string Buscar, short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<CATALOGO_EQUIPOS>();
                if (!string.IsNullOrEmpty(Buscar))
                    predicate = predicate.And(w => w.IP.Contains(Buscar) || w.MAC_ADDRESS.Contains(Buscar) || w.DESCRIPCION.Contains(Buscar));
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                return  GetData(predicate.Expand()).OrderBy(w => w.TIPO_EQUIPO);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>obtiene un Equipos</returns>
        public CATALOGO_EQUIPOS Obtener(string IP, string MacAddress)
        {
            try
            {
                var predicate = PredicateBuilder.True<CATALOGO_EQUIPOS>();
                if (!string.IsNullOrEmpty(IP))
                    predicate = predicate.And(w => w.IP == IP);
                if (!string.IsNullOrEmpty(MacAddress))
                    predicate = predicate.And(w => w.MAC_ADDRESS == MacAddress);
                return GetData(predicate.Expand()).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que valida que una mac e ip esten autorizados
        /// <param name="IPs">Arreglo de IPs del dispositivo a verificar</param>
        /// <param name="MacAddress">MacAddress del dispositivo a verificar</param>
        /// </summary>
        /// <returns>CATALOGO_EQUIPOS</returns>
        public CATALOGO_EQUIPOS ValidarEquipoMAC(string[] IPs, string MacAddress)
        {
            try
            {
                var predicate = PredicateBuilder.True<CATALOGO_EQUIPOS>();
                var predicateOR = PredicateBuilder.False<CATALOGO_EQUIPOS>();
                if (IPs != null && IPs.Count() > 0)
                    foreach (var ip in IPs)
                        predicateOR = predicateOR.Or(w => w.IP == ip);
                predicate = predicate.And(predicateOR.Expand());
                if (!string.IsNullOrWhiteSpace(MacAddress))
                    predicate = predicate.And(w => w.MAC_ADDRESS == MacAddress);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que valida que un disco duro este autorizado
        /// <param name="serial">Serial del disco duro a verificar</param>
        /// <param name="IP">IP del dispositivo a verificar</param>
        /// <param name="MacAddress">MacAddress del dispositivo a verificar</param>
        /// </summary>
        /// <returns>bool</returns>
        public bool ValidarHD(string serial, string IP, string MacAddress)
        {
            try
            {
                var predicate = PredicateBuilder.True<CATALOGO_EQUIPOS>();

                if (string.IsNullOrWhiteSpace(MacAddress) || string.IsNullOrWhiteSpace(IP) || string.IsNullOrWhiteSpace(serial))
                    return false;
                return Context.CATALOGO_EQUIPOS.Any(w=>w.SERIE_VOLUM==serial.Trim() && w.IP==IP && w.MAC_ADDRESS==MacAddress);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


     
        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "CATALOGO_EQUIPOS" con valores a insertar</param>
        public bool Insertar(CATALOGO_EQUIPOS Entity)
        {
            try
            {

                if (Insert(Entity))
                    return true;
            }
            catch (Exception ex)
            {                
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

      
        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "CATALOGO_EQUIPOS" con valores a actualizar</param>
        public bool Actualizar(CATALOGO_EQUIPOS Entity,List<EQUIPO_AREA> areas)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    #region comentado
                    //var equipo_original = Context.CATALOGO_EQUIPOS.First(w => w.IP == Entity.IP && w.MAC_ADDRESS == Entity.MAC_ADDRESS);
                    //if (equipo_original.EQUIPO_AREA!=null && equipo_original.EQUIPO_AREA.Count()>0)
                    //{
                    //    var _copia_equipo_area_original = new EQUIPO_AREA[equipo_original.EQUIPO_AREA.Count()];
                    //    equipo_original.EQUIPO_AREA.CopyTo(_copia_equipo_area_original, 0);
                    //    foreach (var item in _copia_equipo_area_original)
                    //    {
                    //        if (!Entity.EQUIPO_AREA.Any(w => w.IP == item.IP && w.MAC_ADDRESS == item.MAC_ADDRESS && w.ID_AREA==item.ID_AREA))
                    //            equipo_original.EQUIPO_AREA.Remove(equipo_original.EQUIPO_AREA.FirstOrDefault(w=>w.ID_AREA==item.ID_AREA && 
                    //                w.IP==item.IP && w.MAC_ADDRESS==item.MAC_ADDRESS));
                    //    }
                    //    Context.SaveChanges();
                    //}
                    //if (Entity.EQUIPO_AREA != null && Entity.EQUIPO_AREA.Count() > 0)
                    //{
                    //    foreach (var item in Entity.EQUIPO_AREA)
                    //    {
                    //        if (!equipo_original.EQUIPO_AREA.Any(w => w.IP == item.IP && w.MAC_ADDRESS == item.MAC_ADDRESS && w.ID_AREA == item.ID_AREA))
                    //            equipo_original.EQUIPO_AREA.Add(new EQUIPO_AREA
                    //            {
                    //                ID_AREA = item.ID_AREA,
                    //                IP = item.IP,
                    //                MAC_ADDRESS = item.MAC_ADDRESS,
                    //                REGISTRO_FEC = item.REGISTRO_FEC
                    //            });
                    //    }
                    //    Context.SaveChanges();
                    //}
                    #endregion
                    Context.Entry(Entity).State = EntityState.Modified;
                    #region Equipos Areas
                    var lista = Context.EQUIPO_AREA.Where(w => w.IP == Entity.IP && w.MAC_ADDRESS == Entity.MAC_ADDRESS);
                    if (lista != null)
                        foreach (var item in lista)
                            Context.Entry(item).State = EntityState.Deleted;
                    foreach (var ea in areas)
                        Context.EQUIPO_AREA.Add(ea);
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;

                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(CATALOGO_EQUIPOS Entity)
        {
            try
            {
                return Delete(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        //public IEnumerable<CATALOGO_EQUIPOS> ObtenerTodos(object Buscar, short p)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
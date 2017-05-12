using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEnfermedad:EntityManagerServer<ENFERMEDAD>
    {
        /// <summary>
        /// Obtiene todas las enfermedades en paginacion
        /// </summary>
        /// <param name="enfermedad">Nombre parcial o completo de la enfermedad para la consulta</param>
        /// <param name="Pagina">Numero de la pagina para la consulta</param>
        /// <returns>IQueryable&lt;ENFERMEDAD&gt;</returns>
        public IQueryable<ENFERMEDAD> ObtenerTodosPorRango(string enfermedad="", int Pagina=1)
        {
            try
            {
                var _predicate = PredicateBuilder.True<ENFERMEDAD>();
                if (!string.IsNullOrWhiteSpace(enfermedad))
                {
                    _predicate = _predicate.And(w=>w.NOMBRE.Contains(enfermedad));
                }
                return GetData(_predicate.Expand())
                    .OrderBy(o=>o.NOMBRE)
                    .Take((Pagina * 30))
                    .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
        /// <summary>
        /// Inserta una enfermedades
        /// </summary>
        /// <param name="entidad">Entidad de enfermedad a insertar</param>
        /// <param name="gpos_vulnerables">Lista de grupos vulnerables al cual pertenece esta enfermedad</param>
        public void Insertar(ENFERMEDAD entidad, List<short> gpos_vulnerables=null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _id = GetIDProceso<int>("ENFERMEDAD", "ID_ENFERMEDAD", "1=1");
                    entidad.ID_ENFERMEDAD = _id;
                    if (gpos_vulnerables!=null && gpos_vulnerables.Count>0)
                    {
                        foreach (var item in gpos_vulnerables)
                        {
                            entidad.SECTOR_CLASIFICACION.Add(Context.SECTOR_CLASIFICACION.First(w => w.ID_SECTOR_CLAS == item));
                        }
                    }
                    Context.ENFERMEDAD.Add(entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
        /// <summary>
        /// Actualiza la informacion de una enfermedad
        /// </summary>
        /// <param name="entidad">Entidad de enfermedad a actualizar</param>
        /// <param name="gpos_vulnerables">Listado de grupos vulnerables al cual pertenece la enfermedad</param>
        public void Editar(ENFERMEDAD entidad, List<short> gpos_vulnerables=null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _enfermedad = Context.ENFERMEDAD.First(w => w.ID_ENFERMEDAD == entidad.ID_ENFERMEDAD);
                    _enfermedad.SECTOR_CLASIFICACION.Clear();
                    Context.SaveChanges();
                    if (gpos_vulnerables != null && gpos_vulnerables.Count > 0)
                    {
                        foreach (var item in gpos_vulnerables)
                        {
                            _enfermedad.SECTOR_CLASIFICACION.Add(Context.SECTOR_CLASIFICACION.First(w => w.ID_SECTOR_CLAS == item));
                        }
                    }
                    _enfermedad.LETRA = entidad.LETRA;
                    _enfermedad.NOMBRE = entidad.NOMBRE;
                    _enfermedad.CLAVE = entidad.CLAVE;
                    _enfermedad.TIPO = entidad.TIPO;
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}

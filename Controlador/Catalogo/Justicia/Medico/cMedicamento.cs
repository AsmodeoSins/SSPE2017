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
    public class cMedicamento:EntityManagerServer<PRODUCTO>
    {
        /// <summary>
        /// Obtiene todos los medicamentos en paginacion
        /// </summary>
        /// <param name="enfermedad">Nombre parcial o completo del medicamento para la consulta</param>
        /// <param name="Pagina">Numero de la pagina para la consulta</param>
        /// <returns>IQueryable&lt;MEDICAMENTO&gt;</returns>
        public IQueryable<PRODUCTO> ObtenerTodosPorRango(string producto = "", int Pagina = 1)
        {
            try
            {
                var _predicate = PredicateBuilder.True<PRODUCTO>();
                _predicate = _predicate.And(w=>w.PRODUCTO_CATEGORIA.ID_PROD_GRUPO=="M");
                if (!string.IsNullOrWhiteSpace(producto))
                {
                    _predicate = _predicate.And(w => w.NOMBRE.Contains(producto));
                }
                return GetData(_predicate.Expand())
                    .OrderBy(o => o.NOMBRE)
                    .Take((Pagina * 30))
                    .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
        /// <summary>
        /// Insertar un medicamento en el catalogo
        /// </summary>
        /// <param name="entidad">Entidad a insertar en el catalogo</param>
        public void Insertar(PRODUCTO entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _id = GetIDProceso<int>("PRODUCTO", "ID_PRODUCTO", "1=1");
                    entidad.ID_PRODUCTO = _id;
                    if (entidad.PRODUCTO_PRESENTACION_MED!=null && entidad.PRODUCTO_PRESENTACION_MED.Count>0)
                    {
                        foreach (var item in entidad.PRODUCTO_PRESENTACION_MED)
                        {
                            item.ID_PRODUCTO = _id;
                        }
                    }
                    Context.PRODUCTO.Add(entidad);
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
        /// Actualiza la informacion de un medicamento
        /// </summary>
        /// <param name="entidad">Entidad de medicamento</param>
        public void Editar(PRODUCTO entidad, List<PRODUCTO_PRESENTACION_MED> _presentaciones_asignadas=null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.PRODUCTO.Attach(entidad);
                    Context.Entry(entidad).State = EntityState.Modified;
                    Context.SaveChanges();
                    if (_presentaciones_asignadas!=null && _presentaciones_asignadas.Count>0)
                    {
                        var _producto_presentacion_med = Context.PRODUCTO_PRESENTACION_MED.Where(w => w.ID_PRODUCTO == entidad.ID_PRODUCTO).ToList();
                        foreach (var item in _producto_presentacion_med)
                            Context.PRODUCTO_PRESENTACION_MED.Remove(item);
                        foreach (var item in _presentaciones_asignadas)
                            Context.PRODUCTO_PRESENTACION_MED.Add(item);
                        Context.SaveChanges();
                    }
                    
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

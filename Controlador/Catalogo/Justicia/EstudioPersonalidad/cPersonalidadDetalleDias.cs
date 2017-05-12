using System.Linq;
using LinqKit;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonalidadDetalleDias : SSP.Modelo.EntityManagerServer<SSP.Servidor.PERSONALIDAD_DETALLE_DIAS>
    {
        public cPersonalidadDetalleDias() { }
        public bool GuardaFechasProgramacionEstudiosPersonalidad(System.Collections.Generic.List<SSP.Servidor.PERSONALIDAD_DETALLE_DIAS> _Fechas, SSP.Servidor.PERSONALIDAD_DETALLE _EstudioDetalle)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var DetalleActual = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioDetalle.ID_ESTUDIO && x.ID_DETALLE == _EstudioDetalle.ID_DETALLE && x.ID_IMPUTADO == _EstudioDetalle.ID_IMPUTADO && x.ID_INGRESO == _EstudioDetalle.ID_INGRESO && x.ID_ANIO == _EstudioDetalle.ID_ANIO);
                    if (DetalleActual != null)
                    {
                        DetalleActual.INICIO_FEC = _EstudioDetalle.INICIO_FEC;
                        DetalleActual.TERMINO_FEC = _EstudioDetalle.TERMINO_FEC;
                        if (DetalleActual.ID_ESTATUS == 2)
                            DetalleActual.ID_ESTATUS = 5;

                        var _DetallesProgramacionAnterior = Context.PERSONALIDAD_DETALLE_DIAS.Where(c => c.ID_ANIO == _EstudioDetalle.ID_ANIO && c.ID_INGRESO == _EstudioDetalle.ID_INGRESO && c.ID_IMPUTADO == _EstudioDetalle.ID_IMPUTADO && c.ID_ESTUDIO == _EstudioDetalle.ID_ESTUDIO && c.ID_DETALLE == _EstudioDetalle.ID_DETALLE);
                        if (_DetallesProgramacionAnterior.Any())
                            foreach (var item in _DetallesProgramacionAnterior)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        decimal _ConsecutivoPersonalidadDias = GetIDProceso<decimal>("PERSONALIDAD_DETALLE_DIAS", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4} AND ID_DETALLE = {5}", _EstudioDetalle.ID_CENTRO, _EstudioDetalle.ID_ANIO, _EstudioDetalle.ID_IMPUTADO, _EstudioDetalle.ID_INGRESO, _EstudioDetalle.ID_ESTUDIO, _EstudioDetalle.ID_DETALLE));
                        if (_Fechas != null && _Fechas.Any())
                            foreach (var item in _Fechas)
                            {
                                var _NuevoDia = new SSP.Servidor.PERSONALIDAD_DETALLE_DIAS()
                                {
                                    FECHA_FINAL = item.FECHA_FINAL,
                                    FECHA_INICIO = item.FECHA_INICIO,
                                    ID_ANIO = DetalleActual.ID_ANIO,
                                    ID_AREA = item.ID_AREA,
                                    ID_CENTRO = DetalleActual.ID_CENTRO,
                                    ID_DETALLE = DetalleActual.ID_DETALLE,
                                    ID_ESTUDIO = DetalleActual.ID_ESTUDIO,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_CONSEC = _ConsecutivoPersonalidadDias
                                };

                                Context.PERSONALIDAD_DETALLE_DIAS.Add(_NuevoDia);
                                _ConsecutivoPersonalidadDias++;
                            };

                        Context.SaveChanges();
                        transaccion.Complete();
                        return true;
                    };
                }

                return false;
            }
            catch (System.Exception exc)
            {
                return false;
            }
        }

        public System.Linq.IQueryable<SSP.Servidor.PERSONALIDAD_DETALLE_DIAS> ValidaFechas(SSP.Servidor.PERSONALIDAD Entity, System.DateTime? FechaValida)
        {
            try
            {
               var predicate = PredicateBuilder.True<SSP.Servidor.PERSONALIDAD_DETALLE_DIAS>();
               predicate = predicate.And(x => x.ID_ESTUDIO == Entity.ID_ESTUDIO);
               predicate = predicate.And(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ANIO == Entity.ID_ANIO);
               if (FechaValida.HasValue)
                   predicate = predicate.And(x => x.FECHA_INICIO.Value.Year == FechaValida.Value.Year && x.FECHA_INICIO.Value.Month == FechaValida.Value.Month && x.FECHA_INICIO.Value.Day == FechaValida.Value.Day);
              
                return GetData(predicate.Expand());
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

    }
}
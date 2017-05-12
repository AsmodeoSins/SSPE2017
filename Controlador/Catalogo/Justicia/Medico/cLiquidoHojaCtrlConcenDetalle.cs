using LinqKit;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cLiquidoHojaCtrlConcenDetalle : SSP.Modelo.EntityManagerServer<SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE>
    {
        public cLiquidoHojaCtrlConcenDetalle() { }

        public System.Linq.IQueryable<SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE> ObtenerDetallesConcentrado(decimal? _Hospitalizacion, short? _Centro, System.DateTime? _Fecha, decimal? _Turno)
        {
            try
            {
                var predicate = PredicateBuilder.True<SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE>();
                if (_Hospitalizacion.HasValue)
                    predicate = predicate.And(x => x.ID_HOSPITA == _Hospitalizacion);

                if (_Centro.HasValue)
                    predicate = predicate.And(x => x.ID_CENTRO_UBI == _Centro);

                if (_Fecha.HasValue)
                    predicate = predicate.And(x => x.LIQUIDO_HOJA_CTRL.FECHA.Value.Year == _Fecha.Value.Year && x.LIQUIDO_HOJA_CTRL.FECHA.Value.Month == _Fecha.Value.Month && x.LIQUIDO_HOJA_CTRL.FECHA.Value.Day == _Fecha.Value.Day);

                if (_Turno.HasValue)
                    predicate = predicate.And(x => x.LIQUIDO_HOJA_CTRL != null && x.LIQUIDO_HOJA_CTRL.LIQUIDO_HORA != null && x.LIQUIDO_HOJA_CTRL.LIQUIDO_HORA.ID_LIQTURNO == _Turno);

                return GetData(predicate.Expand());
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
    }
}
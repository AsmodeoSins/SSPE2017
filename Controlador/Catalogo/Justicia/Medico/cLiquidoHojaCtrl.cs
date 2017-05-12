using System.Linq;
using LinqKit;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cLiquidoHojaCtrl : SSP.Modelo.EntityManagerServer<SSP.Servidor.LIQUIDO_HOJA_CTRL>
    {
        public cLiquidoHojaCtrl() { }

        public bool IngresarControlLiquidos(SSP.Servidor.LIQUIDO_HOJA_CTRL Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    decimal ConsecutivoHojaControlLiquidos = GetIDProceso<decimal>("LIQUIDO_HOJA_CTRL", "ID_LIQHOJACTRL", "1=1");
                    var _ControlLiquidos = new SSP.Servidor.LIQUIDO_HOJA_CTRL()
                    {
                        FECHA = Entity.FECHA,
                        FECHA_REGISTRO = Entity.FECHA_REGISTRO,
                        FRECUENCIA_CARDIACA = Entity.FRECUENCIA_CARDIACA,
                        FRECUENCIA_RESPIRATORIA = Entity.FRECUENCIA_RESPIRATORIA,
                        GLUCEMIA = Entity.GLUCEMIA,
                        ID_CENTRO_UBI = Entity.ID_CENTRO_UBI,
                        ID_HOSPITA = Entity.ID_HOSPITA,
                        ID_USUARIO__REGISTRO = Entity.ID_USUARIO__REGISTRO,
                        ID_LIQHOJACTRL = ConsecutivoHojaControlLiquidos,
                        ID_LIQHORA = Entity.ID_LIQHORA,
                        TEMPERATURA = Entity.TEMPERATURA,
                        TENSION_ARTERIAL = Entity.TENSION_ARTERIAL
                    };

                    if (Entity.LIQUIDO_HOJA_CTRL_DETALLE != null && Entity.LIQUIDO_HOJA_CTRL_DETALLE.Any())
                        foreach (var item in Entity.LIQUIDO_HOJA_CTRL_DETALLE)
                            _ControlLiquidos.LIQUIDO_HOJA_CTRL_DETALLE.Add(new SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE
                                {
                                    CANT = item.CANT,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_LIQ = item.ID_LIQ,
                                    ID_LIQHOJACTRL = ConsecutivoHojaControlLiquidos
                                });

                    Context.LIQUIDO_HOJA_CTRL.Add(_ControlLiquidos);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public System.Linq.IQueryable<SSP.Servidor.LIQUIDO_HOJA_CTRL> ObtenerHojaLiquidos(decimal? _Hospitalizacion, System.DateTime? _Fecha)
        {
            try
            {
                var predicate = PredicateBuilder.True<SSP.Servidor.LIQUIDO_HOJA_CTRL>();
                if (_Hospitalizacion.HasValue)
                    predicate = predicate.And(x => x.ID_HOSPITA == _Hospitalizacion);

                if (_Fecha.HasValue)
                {
                    System.DateTime _fechaConsulta = new System.DateTime(_Fecha.Value.Year, _Fecha.Value.Month, _Fecha.Value.Day, 0, 0, 0);
                    predicate = predicate.And(x => x.FECHA.Value.Year == _fechaConsulta.Year && x.FECHA.Value.Month == _fechaConsulta.Month &&
                        x.FECHA.Value.Day == _fechaConsulta.Day);
                }

                return GetData(predicate.Expand());
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public System.Linq.IQueryable<SSP.Servidor.LIQUIDO_HOJA_CTRL> ObtenerHojaLiquidos(decimal? _Hospitalizado, System.DateTime? _Fecha, decimal? _Hora)
        {
            try
            {
                var predicate = PredicateBuilder.True<SSP.Servidor.LIQUIDO_HOJA_CTRL>();
                if (_Hospitalizado.HasValue)
                    predicate = predicate.And(x => x.ID_HOSPITA == _Hospitalizado);

                if (_Fecha.HasValue && _Hora.HasValue)
                {
                    int Hor = System.Convert.ToInt32(_Hora.Value);
                    System.DateTime _fechaConsulta = new System.DateTime(_Fecha.Value.Year, _Fecha.Value.Month, _Fecha.Value.Day, 0, 0, 0);
                    predicate = predicate.And(x => x.FECHA.Value.Year == _fechaConsulta.Year && x.FECHA.Value.Month == _fechaConsulta.Month &&
                        x.FECHA.Value.Day == _fechaConsulta.Day && x.ID_LIQHORA == _Hora);
                };

                return GetData(predicate.Expand());
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public IQueryable<SSP.Servidor.LIQUIDO_HOJA_CTRL> ObtenerHojasLiquidosBusqueda(System.DateTime? FechaInicio, System.DateTime? FechaFin, SSP.Servidor.INGRESO Ingreso, decimal? Turno)
        {
            try
            {
                var predicado = PredicateBuilder.True<SSP.Servidor.LIQUIDO_HOJA_CTRL>();

                predicado = predicado.And(x => x.HOSPITALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO == Ingreso.ID_INGRESO && x.HOSPITALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO == Ingreso.ID_IMPUTADO && x.HOSPITALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO == Ingreso.ID_ANIO);

                if (FechaInicio.HasValue)
                    predicado = predicado.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.FECHA_REGISTRO) >= System.Data.Objects.EntityFunctions.TruncateTime(FechaInicio));
                if (FechaFin.HasValue)
                    predicado = predicado.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.FECHA_REGISTRO) <= System.Data.Objects.EntityFunctions.TruncateTime(FechaFin));

                if (Turno.HasValue)
                    predicado = predicado.And(x => x.LIQUIDO_HORA.ID_LIQTURNO == Turno);

                return GetData(predicado.Expand());
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }
    }
}
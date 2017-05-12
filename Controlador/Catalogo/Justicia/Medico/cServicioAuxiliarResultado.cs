using LinqKit;
using System.Data.Objects;
using System.Linq;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cServicioAuxiliarResultado : SSP.Modelo.EntityManagerServer<SSP.Servidor.SERVICIO_AUXILIAR_RESULTADO>
    {
        public bool AgregarArchivo(SSP.Servidor.SERVICIO_AUXILIAR_RESULTADO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    decimal ConsecutivoServAuxiliar = GetIDProceso<decimal>("SERVICIO_AUXILIAR_RESULTADO", "ID_SA_RESULTADO", "1=1");

                    var _Archivo = new SSP.Servidor.SERVICIO_AUXILIAR_RESULTADO()
                    {
                        CAMPO_BLOB = Entity.CAMPO_BLOB,
                        INGRESO = Entity.INGRESO,
                        ID_ANIO = Entity.ID_ANIO,
                        ID_CENTRO = Entity.ID_CENTRO,
                        ID_IMPUTADO = Entity.ID_IMPUTADO,
                        ID_SA_RESULTADO = ConsecutivoServAuxiliar,
                        ID_USUARIO = Entity.ID_USUARIO,
                        ID_FORMATO = Entity.ID_FORMATO,
                        REGISTRO_FEC = Entity.REGISTRO_FEC,
                        NOMBRE_ARCHIVO = Entity.NOMBRE_ARCHIVO,
                        ID_SERV_AUX = Entity.ID_SERV_AUX,
                        ID_INGRESO = Entity.ID_INGRESO
                    };

                    Context.SERVICIO_AUXILIAR_RESULTADO.Add(_Archivo);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (System.Exception exc)
            {
                return false;
            }
        }

        public System.Linq.IQueryable<SSP.Servidor.SERVICIO_AUXILIAR_RESULTADO> BuscarResultados(System.DateTime? _FechaInicio, System.DateTime? _FechaFin, short? TipoServAux, short? _SubTipoServAux, int? _Diagnostico, SSP.Servidor.INGRESO Entity, short? IngresoBusqueda)
        {
            try
            {
                var predicado = PredicateBuilder.True<SSP.Servidor.SERVICIO_AUXILIAR_RESULTADO>();
                predicado = predicado.And(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_ANIO == Entity.ID_ANIO);

                if (_FechaInicio.HasValue)
                    predicado = predicado.And(v => EntityFunctions.TruncateTime(v.REGISTRO_FEC) >= _FechaInicio);

                if (_FechaFin.HasValue)
                    predicado = predicado.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= _FechaFin);

                if (_Diagnostico.HasValue)
                    if (_Diagnostico != -1)
                        predicado = predicado.And(x => x.ID_SERV_AUX == _Diagnostico);

                if (_SubTipoServAux.HasValue)
                    if (_SubTipoServAux != -1)
                        predicado = predicado.And(x => x.SERVICIO_AUX_DIAG_TRAT.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.ID_SUBTIPO_SADT == _SubTipoServAux);

                if (TipoServAux.HasValue)
                    if (TipoServAux != -1)
                        predicado = predicado.And(x => x.SERVICIO_AUX_DIAG_TRAT.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.TIPO_SERVICIO_AUX_DIAG_TRAT.ID_TIPO_SADT == TipoServAux);

                if (IngresoBusqueda.HasValue)
                    if (IngresoBusqueda != -1)
                        predicado = predicado.And(x => x.ID_INGRESO == IngresoBusqueda);

                return GetData(predicado.Expand()).OrderByDescending(x => x.REGISTRO_FEC);
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public System.Linq.IQueryable<SSP.Servidor.SERVICIO_AUXILIAR_RESULTADO> BuscarResultadosNotaMedica(System.DateTime? _FechaInicio, System.DateTime? _FechaFin, short? TipoServAux, short? _SubTipoServAux,
            int? _Diagnostico, SSP.Servidor.INGRESO Entity, short? IngresoBusqueda)
        {
            try
            {
                var predicado = PredicateBuilder.True<SSP.Servidor.SERVICIO_AUXILIAR_RESULTADO>();
                predicado = predicado.And(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_IMPUTADO == Entity.ID_IMPUTADO);

                if (_FechaInicio.HasValue)
                    predicado = predicado.And(v => EntityFunctions.TruncateTime(v.REGISTRO_FEC) >= _FechaInicio);

                if (_FechaFin.HasValue)
                    predicado = predicado.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= _FechaFin);

                if (_Diagnostico.HasValue)
                    if (_Diagnostico != -1)
                        predicado = predicado.And(x => x.ID_SERV_AUX == _Diagnostico);
                    else
                        predicado = predicado.And(x => x.ID_SERV_AUX > 1);

                if (IngresoBusqueda.HasValue)
                    if (IngresoBusqueda != -1)
                        predicado = predicado.And(x => x.ID_INGRESO == IngresoBusqueda);
                var nuevo = GetData(g => (g.ID_ANIO == Entity.ID_ANIO && g.ID_IMPUTADO == Entity.ID_IMPUTADO && g.ID_INGRESO == Entity.ID_INGRESO) ?
                    EntityFunctions.TruncateTime(g.REGISTRO_FEC) >= _FechaInicio ?
                        EntityFunctions.TruncateTime(g.REGISTRO_FEC) <= _FechaFin ?
                            TipoServAux.HasValue ?
                                (TipoServAux > 0 ?
                                    (g.SERVICIO_AUX_DIAG_TRAT != null ?
                                        g.SERVICIO_AUX_DIAG_TRAT.SUBTIPO_SERVICIO_AUX_DIAG_TRAT != null ?
                                            g.SERVICIO_AUX_DIAG_TRAT.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.ID_TIPO_SADT.HasValue ?
                                                g.SERVICIO_AUX_DIAG_TRAT.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.ID_TIPO_SADT.Value == TipoServAux
                                            : false
                                        : false
                                    : false)
                                : true) ?
                                    _SubTipoServAux.HasValue ?
                                        (_SubTipoServAux > 0 ?
                                            g.SERVICIO_AUX_DIAG_TRAT.ID_SUBTIPO_SADT == _SubTipoServAux.Value
                                        : true) ?
                                            _Diagnostico.HasValue ?
                                                _Diagnostico > 0 ?
                                                    g.ID_SERV_AUX == _Diagnostico.Value
                                                : true
                                            : false
                                        : false
                                    : false
                                : false
                            : false
                        : false
                    : false
                : false);
                //var viejo = GetData(predicado.Expand());
                return nuevo;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
    }
}
using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTrasladoIslasPersonalidad : EntityManagerServer<TRASLADO_ISLAS>
    {
        public bool InsertarTrasladoIslas(TRASLADO_ISLAS Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _trasladoIslas = Context.TRASLADO_ISLAS.FirstOrDefault(c => c.ID_ESTUDIO == Entity.ID_ESTUDIO && c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_INGRESO == Entity.ID_INGRESO && c.ID_ANIO == Entity.ID_ANIO);
                    if (_trasladoIslas == null)
                    {
                        var _trasla = new TRASLADO_ISLAS()
                        {
                            AGRESIVIDAD_INTRAMUROS = Entity.AGRESIVIDAD_INTRAMUROS,
                            ANUENCIA_FIRMADA = Entity.ANUENCIA_FIRMADA,
                            CONTINUE_TRATAMIENTO = Entity.CONTINUE_TRATAMIENTO,
                            DICTAMEN_TRASLADO = Entity.DICTAMEN_TRASLADO,
                            EN_CASO_ADICCION = Entity.EN_CASO_ADICCION,
                            ID_ANIO = Entity.ID_ANIO,
                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                            ID_PELIGROSIDAD = Entity.ID_PELIGROSIDAD,
                            INTIMIDACION_PENA = Entity.INTIMIDACION_PENA,
                            EGOCENTRISMO = Entity.EGOCENTRISMO,
                            ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                            CRIMINOGENESIS = Entity.CRIMINOGENESIS,
                            DIRECTOR = Entity.DIRECTOR,
                            ID_CENTRO = Entity.ID_CENTRO,
                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                            ID_INGRESO = Entity.ID_INGRESO,
                            LABILIDAD_AFECTIVA = Entity.LABILIDAD_AFECTIVA,
                            RESPONSABLE = Entity.RESPONSABLE,
                            TRATAMIENTO_SUGERIDO = Entity.TRATAMIENTO_SUGERIDO
                        };

                        Context.TRASLADO_ISLAS.Add(_trasla);
                    }
                    else
                    {
                        _trasladoIslas.AGRESIVIDAD_INTRAMUROS = Entity.AGRESIVIDAD_INTRAMUROS;
                        _trasladoIslas.ANUENCIA_FIRMADA = Entity.ANUENCIA_FIRMADA;
                        _trasladoIslas.CONTINUE_TRATAMIENTO = Entity.CONTINUE_TRATAMIENTO;
                        _trasladoIslas.CRIMINOGENESIS = Entity.CRIMINOGENESIS;
                        _trasladoIslas.DICTAMEN_TRASLADO = Entity.DICTAMEN_TRASLADO;
                        _trasladoIslas.DIRECTOR = Entity.DIRECTOR;
                        _trasladoIslas.EGOCENTRISMO = Entity.EGOCENTRISMO;
                        _trasladoIslas.EN_CASO_ADICCION = Entity.EN_CASO_ADICCION;
                        _trasladoIslas.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                        _trasladoIslas.ID_PELIGROSIDAD = Entity.ID_PELIGROSIDAD;
                        _trasladoIslas.INTIMIDACION_PENA = Entity.INTIMIDACION_PENA;
                        _trasladoIslas.LABILIDAD_AFECTIVA = Entity.LABILIDAD_AFECTIVA;
                        _trasladoIslas.RESPONSABLE = Entity.RESPONSABLE;
                        _trasladoIslas.TRATAMIENTO_SUGERIDO = Entity.TRATAMIENTO_SUGERIDO;
                        Context.Entry(_trasladoIslas).State = System.Data.EntityState.Modified;
                    }

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
    }
}
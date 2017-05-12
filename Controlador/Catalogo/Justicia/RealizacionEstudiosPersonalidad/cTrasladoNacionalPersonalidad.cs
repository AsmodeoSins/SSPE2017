using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTrasladoNacionalPersonalidad : SSP.Modelo.EntityManagerServer<SSP.Servidor.TRASLADO_NACIONAL>
    {
        public bool InsertarTrasladoNacional(SSP.Servidor.TRASLADO_NACIONAL Entity)
        {
            using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                try
                {
                    var _trasla = Context.TRASLADO_NACIONAL.FirstOrDefault(c => c.ID_ESTUDIO == Entity.ID_ESTUDIO && c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_INGRESO == Entity.ID_INGRESO && c.ID_ANIO == Entity.ID_ANIO);
                    if (_trasla == null)
                    {
                        var _tr = new SSP.Servidor.TRASLADO_NACIONAL()
                        {
                            ADICCION_TOXICOS = Entity.ADICCION_TOXICOS,
                            CONTINUAR_EDUCATIVO = Entity.CONTINUAR_EDUCATIVO,
                            CONTINUAR_LABORAL = Entity.CONTINUAR_LABORAL,
                            CONTINUAR_OTROS = Entity.CONTINUAR_OTROS,
                            CONTINUAR_PSICOLOGICO = Entity.CONTINUAR_PSICOLOGICO,
                            COORDINADOR_CRIMINOLOGIA = Entity.COORDINADOR_CRIMINOLOGIA,
                            CUALES = Entity.CUALES,
                            ELABORO = Entity.ELABORO,
                            DIRECTOR_CERESO = Entity.DIRECTOR_CERESO,
                            ID_ANIO = Entity.ID_ANIO,
                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                            ID_PELIGROSIDAD = Entity.ID_PELIGROSIDAD.HasValue ? Entity.ID_PELIGROSIDAD.Value != -1 ? Entity.ID_PELIGROSIDAD : null : null,
                            ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                            ID_CENTRO = Entity.ID_CENTRO,
                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                            ID_INGRESO = Entity.ID_INGRESO,
                            OTROS_ASPECTOS_OPINION = Entity.OTROS_ASPECTOS_OPINION
                        };

                        Context.TRASLADO_NACIONAL.Add(_tr);
                    }

                    else
                    {
                        _trasla.ADICCION_TOXICOS = Entity.ADICCION_TOXICOS;
                        _trasla.CONTINUAR_EDUCATIVO = Entity.CONTINUAR_EDUCATIVO;
                        _trasla.CONTINUAR_LABORAL = Entity.CONTINUAR_LABORAL;
                        _trasla.CONTINUAR_OTROS = Entity.CONTINUAR_OTROS;
                        _trasla.CONTINUAR_PSICOLOGICO = Entity.CONTINUAR_PSICOLOGICO;
                        _trasla.COORDINADOR_CRIMINOLOGIA = Entity.COORDINADOR_CRIMINOLOGIA;
                        _trasla.CUALES = Entity.CUALES;
                        _trasla.DIRECTOR_CERESO = Entity.DIRECTOR_CERESO;
                        _trasla.ELABORO = Entity.ELABORO;
                        _trasla.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                        _trasla.ID_ANIO = Entity.ID_ANIO;
                        _trasla.ID_CENTRO = Entity.ID_CENTRO;
                        _trasla.ID_ESTUDIO = Entity.ID_ESTUDIO;
                        _trasla.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        _trasla.ID_INGRESO = Entity.ID_INGRESO;
                        _trasla.ID_PELIGROSIDAD = Entity.ID_PELIGROSIDAD.HasValue ? Entity.ID_PELIGROSIDAD.Value != -1 ? Entity.ID_PELIGROSIDAD : null : null;
                        _trasla.OTROS_ASPECTOS_OPINION = Entity.OTROS_ASPECTOS_OPINION;
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
        }
    }
}
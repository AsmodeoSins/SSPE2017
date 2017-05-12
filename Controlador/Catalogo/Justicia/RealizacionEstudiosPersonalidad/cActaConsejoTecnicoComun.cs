using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cActaConsejoTecnicoComun : EntityManagerServer<ACTA_CONSEJO_TECNICO>
    {
        public cActaConsejoTecnicoComun() { }

        public bool InsertarActaConsejoTecnico(ACTA_CONSEJO_TECNICO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _Acta = Context.ACTA_CONSEJO_TECNICO.Where(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();
                    if (_Acta == null)
                    {
                        var _ActaComun = new ACTA_CONSEJO_TECNICO()
                        {
                            ACTUACION = Entity.ACTUACION,
                            ACUERDO = Entity.ACUERDO,
                            AREA_LABORAL = Entity.AREA_LABORAL,
                            EDUCATIVO = Entity.EDUCATIVO,
                            ID_CENTRO = Entity.ID_CENTRO,
                            ID_INGRESO = Entity.ID_INGRESO,
                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                            ID_ANIO = Entity.ID_ANIO,
                            CRIMINOLOGIA = Entity.CRIMINOLOGIA,
                            JURIDICO = Entity.JURIDICO,
                            INTERNO = Entity.INTERNO,
                            LUGAR = Entity.LUGAR,
                            MANIFESTARON = Entity.MANIFESTARON,
                            MEDICO = Entity.MEDICO,
                            OPINION = Entity.OPINION,
                            OPINION_CRIMINOLOGIA = Entity.OPINION_CRIMINOLOGIA,
                            OPINION_ESCOLAR = Entity.OPINION_ESCOLAR,
                            OPINION_LABORAL = Entity.OPINION_LABORAL,
                            OPINION_MEDICO = Entity.OPINION_MEDICO,
                            OPINION_PSICOLOGICA = Entity.OPINION_PSICOLOGICA,
                            OPINION_SEGURIDAD = Entity.OPINION_SEGURIDAD,
                            OPINION_TRABAJO_SOCIAL = Entity.OPINION_TRABAJO_SOCIAL,
                            PRESIDENTE = Entity.PRESIDENTE,
                            PSICOLOGIA = Entity.PSICOLOGIA,
                            SECRETARIO = Entity.SECRETARIO,
                            SEGURIDAD_CUSTODIA = Entity.SEGURIDAD_CUSTODIA,
                            TRABAJO_SOCIAL = Entity.TRABAJO_SOCIAL
                        };

                        Context.ACTA_CONSEJO_TECNICO.Add(_ActaComun);
                    }
                    else
                    {
                        _Acta.ACTUACION = Entity.ACTUACION;
                        _Acta.ACUERDO = Entity.ACUERDO;
                        _Acta.AREA_LABORAL = Entity.AREA_LABORAL;
                        _Acta.CRIMINOLOGIA = Entity.CRIMINOLOGIA;
                        _Acta.EDUCATIVO = Entity.EDUCATIVO;
                        _Acta.ID_ANIO = Entity.ID_ANIO;
                        _Acta.ID_CENTRO = Entity.ID_CENTRO;
                        _Acta.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        _Acta.ID_INGRESO = Entity.ID_INGRESO;
                        _Acta.INTERNO = Entity.INTERNO;
                        _Acta.JURIDICO = Entity.JURIDICO;
                        _Acta.LUGAR = Entity.LUGAR;
                        _Acta.MANIFESTARON = Entity.MANIFESTARON;
                        _Acta.MEDICO = Entity.MEDICO;
                        _Acta.OPINION = Entity.OPINION;
                        _Acta.OPINION_CRIMINOLOGIA = Entity.OPINION_CRIMINOLOGIA;
                        _Acta.OPINION_ESCOLAR = Entity.OPINION_ESCOLAR;
                        _Acta.OPINION_LABORAL = Entity.OPINION_LABORAL;
                        _Acta.OPINION_MEDICO = Entity.OPINION_MEDICO;
                        _Acta.OPINION_PSICOLOGICA = Entity.OPINION_PSICOLOGICA;
                        _Acta.OPINION_SEGURIDAD = Entity.OPINION_SEGURIDAD;
                        _Acta.OPINION_TRABAJO_SOCIAL = Entity.OPINION_TRABAJO_SOCIAL;
                        _Acta.PRESIDENTE = Entity.PRESIDENTE;
                        _Acta.PSICOLOGIA = Entity.PSICOLOGIA;
                        _Acta.SECRETARIO = Entity.SECRETARIO;
                        _Acta.SEGURIDAD_CUSTODIA = Entity.SEGURIDAD_CUSTODIA;
                        _Acta.TRABAJO_SOCIAL = Entity.TRABAJO_SOCIAL;
                        Context.Entry(_Acta).State = System.Data.EntityState.Modified;
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                };
            }
            catch (System.Exception exc)
            {
                return false;
            }

            return false;
        }
    }
}
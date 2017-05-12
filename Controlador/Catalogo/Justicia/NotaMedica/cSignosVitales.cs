using System.Linq;
using System.Transactions;
using SSP.Servidor;
using SSP.Modelo;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSignosVitales : EntityManagerServer<NOTA_SIGNOS_VITALES>
    {
        public cSignosVitales() { }

        public NOTA_SIGNOS_VITALES ObtenerSignosVitales(short centro, short anio, int imputado, short ingreso)
        {
            try
            {
                return GetData().First(x => x.ATENCION_MEDICA.ID_ANIO == anio && x.ATENCION_MEDICA.ID_CENTRO == centro && x.ATENCION_MEDICA.ID_INGRESO == ingreso && x.ATENCION_MEDICA.ID_IMPUTADO == imputado &&
                    x.ATENCION_MEDICA.ID_TIPO_ATENCION == (short)eTipoAtencionMedica.CONSULTA_MEDICA);
            }

            catch (System.Exception exc)
            {
                throw;
            }
        }

        public IQueryable<NOTA_SIGNOS_VITALES> ObtenerMasRecienteXImputado(short centro, short anio, int imputado, short ingreso)
        {
            try
            {
                return GetData(x => x.ATENCION_MEDICA.ID_ANIO == anio && x.ATENCION_MEDICA.ID_CENTRO == centro && x.ATENCION_MEDICA.ID_INGRESO == ingreso && x.ATENCION_MEDICA.ID_IMPUTADO == imputado)
                    .OrderByDescending(o => o.ATENCION_MEDICA.ATENCION_FEC);
            }

            catch (System.Exception exc)
            {
                throw;
            }
        }
        ///SE CREA ENUMERADOR TEMPORAL PARA SEGUIR CON EL PROCESO, LOS DATOS AQUI LISTADOS SON PRODUCTO DEL DOCUMENTO DE ESPECIFICACION, EN LA PAG 219
        private enum eTipoAtencionMedica
        {
            CONSULTA_MEDICA = 1,
            SERVICIOS_DE_URGENCIAS = 2,
            CONSULTA_DENTAL = 3,
            ESTUDIOS_LABORATORIO_O_RAYOS_X = 4
        }

        public bool GuardarSignosVitales(NOTA_SIGNOS_VITALES Entity, short Anio, int IdImputado, short? IdCentro, short IdIngreso)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var AtencionMedica = new ATENCION_MEDICA
                    {
                        ID_TIPO_ATENCION = (short)eTipoAtencionMedica.CONSULTA_MEDICA,
                        ID_ANIO = Anio,
                        ID_IMPUTADO = IdImputado,
                        ID_CENTRO = IdCentro,
                        ID_INGRESO = IdIngreso,
                        ID_ATENCION_MEDICA = GetSequence<short>("ATENCION_MEDICA_SEQ")
                    };

                    var SignoVital = new NOTA_SIGNOS_VITALES
                    {
                        FRECUENCIA_CARDIAC = Entity.FRECUENCIA_CARDIAC,
                        FRECUENCIA_RESPIRA = Entity.FRECUENCIA_RESPIRA,
                        ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                        ID_RESPONSABLE = Entity.ID_RESPONSABLE,
                        OBSERVACIONES = Entity.OBSERVACIONES,
                        PESO = Entity.PESO,
                        TALLA = Entity.TALLA,
                        TEMPERATURA = Entity.TEMPERATURA,
                        TENSION_ARTERIAL = Entity.TENSION_ARTERIAL
                    };

                    Context.ATENCION_MEDICA.Add(AtencionMedica);
                    Context.NOTA_SIGNOS_VITALES.Add(SignoVital);

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }

            catch (System.Exception)
            {
                throw;
            }

            return false;
        }

        public bool EditarSignosVitales(NOTA_SIGNOS_VITALES Entity)
        {
            try
            {
                return Update(Entity);
            }

            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }
    }
}

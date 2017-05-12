using LinqKit;
using System.Linq;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSolicitudInterconsultaInterna : SSP.Modelo.EntityManagerServer<SSP.Servidor.SOL_INTERCONSULTA_INTERNA>
    {
        public cSolicitudInterconsultaInterna() { }

        public System.Linq.IQueryable<SSP.Servidor.SOL_INTERCONSULTA_INTERNA> BuscarSolicitudes(System.Collections.Generic.List<string> estatus, short? tipo_atencion = null, short? anio_imputado = null, int? folio_imputado = null,
            string nombre = "", string paterno = "", string materno = "",short? tipo_interconsulta = null, System.DateTime? fecha_inicio = null, System.DateTime? fecha_final = null) 
        {
            try
            {
                var predicate = PredicateBuilder.True<SSP.Servidor.SOL_INTERCONSULTA_INTERNA>();
                predicate = predicate.And(x => x.INTERCONSULTA_SOLICITUD != null);
                predicate = predicate.And(x => x.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA != null);
                predicate = predicate.And(x => x.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null);

                //var predicate = PredicateBuilder.True<SSP.Servidor.INTERCONSULTA_SOLICITUD>();
                var predicateOr = PredicateBuilder.False<SSP.Servidor.INTERCONSULTA_SOLICITUD>();
                //foreach (var item in estatus)
                //    predicateOr = predicateOr.Or(w => w.ESTATUS == item);
                //if (estatus != null && estatus.Count > 0)
                //    predicate = predicate.And(predicateOr.Expand());
                if (tipo_atencion.HasValue)
                    predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_TIPO_ATENCION == tipo_atencion.Value);
                if (tipo_interconsulta.HasValue)
                {
                    if (tipo_interconsulta != -1)
                    {
                        predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.ID_INTER == tipo_interconsulta);
                        if (fecha_inicio.HasValue || fecha_final.HasValue)
                            if (fecha_inicio.HasValue)
                                if (tipo_interconsulta == 1)
                                    predicate = predicate.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.REGISTRO_FEC) >= System.Data.Objects.EntityFunctions.TruncateTime(fecha_inicio));
                                else
                                    predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any(a => System.Data.Objects.EntityFunctions.TruncateTime(a.FECHA_CITA) >= System.Data.Objects.EntityFunctions.TruncateTime(fecha_inicio)));
                            else
                                if (tipo_interconsulta == 1)
                                    predicate = predicate.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= System.Data.Objects.EntityFunctions.TruncateTime(fecha_final));
                                else
                                    predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any(a => System.Data.Objects.EntityFunctions.TruncateTime(a.FECHA_CITA) <= System.Data.Objects.EntityFunctions.TruncateTime(fecha_final)));
                    }
                }
                else
                {
                    if (fecha_inicio.HasValue)
                        predicate = predicate.And(w => (System.Data.Objects.EntityFunctions.TruncateTime(w.REGISTRO_FEC) >= System.Data.Objects.EntityFunctions.TruncateTime(fecha_inicio) && !w.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any()) ||
                            (w.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any(a => System.Data.Objects.EntityFunctions.TruncateTime(a.FECHA_CITA) >= System.Data.Objects.EntityFunctions.TruncateTime(fecha_inicio))));
                    if (fecha_final.HasValue)
                        predicate = predicate.And(w => (System.Data.Objects.EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= System.Data.Objects.EntityFunctions.TruncateTime(fecha_final) && !w.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any())
                            || w.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any(a => System.Data.Objects.EntityFunctions.TruncateTime(a.FECHA_CITA) <= System.Data.Objects.EntityFunctions.TruncateTime(fecha_final)));
                }

                if (anio_imputado.HasValue && folio_imputado.HasValue)
                {
                    predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_ANIO == anio_imputado.Value && w.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_IMPUTADO == folio_imputado.Value);
                }

                if (!string.IsNullOrWhiteSpace(nombre))
                    predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Contains(nombre));
                if (!string.IsNullOrWhiteSpace(paterno))
                    predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Contains(paterno));
                if (!string.IsNullOrWhiteSpace(materno))
                    predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Contains(materno));

                return GetData(predicate.Expand());
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
    }
}
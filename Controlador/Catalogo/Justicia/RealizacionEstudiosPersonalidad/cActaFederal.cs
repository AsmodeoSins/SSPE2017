using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cActaFederal : EntityManagerServer<PFF_ACTA_CONSEJO_TECNICO>
    {
        public bool GuardarActaFederal(PFF_ACTA_CONSEJO_TECNICO Acta, short _IdEstudio)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (Acta != null)
                    {
                        var _ActaActual = Context.PFF_ACTA_CONSEJO_TECNICO.FirstOrDefault(x => x.ID_ESTUDIO == _IdEstudio && x.ID_IMPUTADO == Acta.ID_IMPUTADO && x.ID_INGRESO == Acta.ID_INGRESO);
                        if (_ActaActual == null)
                        {
                            var NvaActa = new PFF_ACTA_CONSEJO_TECNICO()
                            {
                                APROBADO_APLAZADO = Acta.APROBADO_APLAZADO,
                                APROBADO_POR = Acta.APROBADO_POR,
                                FECHA = Acta.FECHA,
                                ID_ANIO = Acta.ID_ANIO,
                                ID_IMPUTADO = Acta.ID_IMPUTADO,
                                LUGAR = Acta.LUGAR,
                                ID_ESTUDIO = _IdEstudio,
                                ID_INGRESO = Acta.ID_INGRESO,
                                ID_CENTRO = Acta.ID_CENTRO,
                                EXPEDIENTE = Acta.EXPEDIENTE,
                                DIRECTOR = Acta.DIRECTOR,
                                CEN_ID_CENTRO = Acta.CEN_ID_CENTRO,
                                PRESENTE_ACTUACION = Acta.PRESENTE_ACTUACION,
                                SESION_FEC = Acta.SESION_FEC,
                                SUSCRITO_DIRECTOR_CRS = Acta.SUSCRITO_DIRECTOR_CRS,
                                TRAMITE = Acta.TRAMITE
                            };

                            if (Acta.PFF_ACTA_DETERMINO != null && Acta.PFF_ACTA_DETERMINO.Any())
                                foreach (var item in Acta.PFF_ACTA_DETERMINO)
                                {
                                    var NvaDetermino = new PFF_ACTA_DETERMINO()
                                    {
                                        ID_ANIO = NvaActa.ID_ANIO,
                                        ID_AREA_TECNICA = item.ID_AREA_TECNICA,
                                        ID_CENTRO = NvaActa.ID_CENTRO,
                                        ID_ESTUDIO = NvaActa.ID_ESTUDIO,
                                        ID_IMPUTADO = NvaActa.ID_IMPUTADO,
                                        ID_INGRESO = NvaActa.ID_INGRESO,
                                        OPINION = item.OPINION,
                                        NOMBRE = item.NOMBRE
                                    };

                                    NvaActa.PFF_ACTA_DETERMINO.Add(NvaDetermino);
                                };


                            Context.PFF_ACTA_CONSEJO_TECNICO.Add(NvaActa);
                        }
                        else
                        {
                            var _condensadoAreas = Context.PFF_ACTA_DETERMINO.Where(x => x.ID_ESTUDIO == _ActaActual.ID_ESTUDIO && x.ID_IMPUTADO == _ActaActual.ID_IMPUTADO && x.ID_INGRESO == _ActaActual.ID_INGRESO && x.ID_CENTRO == _ActaActual.ID_CENTRO && x.ID_ANIO == _ActaActual.ID_ANIO);
                            _ActaActual.APROBADO_APLAZADO = Acta.APROBADO_APLAZADO;
                            _ActaActual.APROBADO_POR = Acta.APROBADO_POR;
                            _ActaActual.CEN_ID_CENTRO = Acta.CEN_ID_CENTRO;
                            _ActaActual.DIRECTOR = Acta.DIRECTOR;
                            _ActaActual.EXPEDIENTE = Acta.EXPEDIENTE;
                            _ActaActual.FECHA = Acta.FECHA;
                            _ActaActual.ID_ANIO = Acta.ID_ANIO;
                            _ActaActual.ID_CENTRO = Acta.ID_CENTRO;
                            _ActaActual.ID_ESTUDIO = _IdEstudio;
                            _ActaActual.ID_IMPUTADO = Acta.ID_IMPUTADO;
                            _ActaActual.ID_INGRESO = Acta.ID_INGRESO;
                            _ActaActual.LUGAR = Acta.LUGAR;
                            _ActaActual.PRESENTE_ACTUACION = Acta.PRESENTE_ACTUACION;
                            _ActaActual.SESION_FEC = Acta.SESION_FEC;
                            _ActaActual.SUSCRITO_DIRECTOR_CRS = Acta.SUSCRITO_DIRECTOR_CRS;
                            _ActaActual.TRAMITE = Acta.TRAMITE;
                            Context.Entry(_ActaActual).State = System.Data.EntityState.Modified;

                            if (_condensadoAreas != null && _condensadoAreas.Any())
                                foreach (var item in _condensadoAreas)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            if(Acta.PFF_ACTA_DETERMINO != null && Acta.PFF_ACTA_DETERMINO.Any())
                                foreach (var item in Acta.PFF_ACTA_DETERMINO)
                                {
                                    var NvaDetermino = new PFF_ACTA_DETERMINO()
                                    {
                                        ID_ANIO = item.ID_ANIO,
                                        ID_AREA_TECNICA = item.ID_AREA_TECNICA,
                                        ID_CENTRO = item.ID_CENTRO,
                                        ID_ESTUDIO = item.ID_ESTUDIO,
                                        ID_IMPUTADO = item.ID_IMPUTADO,
                                        ID_INGRESO = item.ID_INGRESO,
                                        OPINION = item.OPINION,
                                        NOMBRE = item.NOMBRE
                                    };

                                    _ActaActual.PFF_ACTA_DETERMINO.Add(NvaDetermino);
                                };
                        }
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
    }
}
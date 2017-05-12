using SSP.Servidor;
using SSP.Modelo;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTrasladoInternacionalPersonalidad : EntityManagerServer<TRASLADO_INTERNACIONAL>
    {
        public bool InsertarTrasladoInternacional(TRASLADO_INTERNACIONAL Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _trasladoInter = Context.TRASLADO_INTERNACIONAL.FirstOrDefault(c => c.ID_INGRESO == Entity.ID_INGRESO && c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_ESTUDIO == Entity.ID_ESTUDIO && c.ID_ANIO == Entity.ID_ANIO);
                    if (_trasladoInter == null)
                    {
                        var _internacional = new TRASLADO_INTERNACIONAL()
                        {
                            ADICCION_CUALES = Entity.ADICCION_CUALES,
                            ADICCION_TOXICOS = Entity.ADICCION_TOXICOS,
                            AGRESIVIDAD = Entity.AGRESIVIDAD,
                            ANUENCIA_CUPO = Entity.ANUENCIA_CUPO,
                            ANUENCIA_FEC = Entity.ANUENCIA_FEC,
                            APOYO_PADRES = Entity.APOYO_PADRES,
                            CARTA_ARRAIGO = Entity.CARTA_ARRAIGO,
                            CAUSA_NO_ESTUDIA = Entity.CAUSA_NO_ESTUDIA,
                            CAUSA_NO_TRABAJA = Entity.CAUSA_NO_TRABAJA,
                            CAUSA_NO_VISITAS = Entity.CAUSA_NO_VISITAS,
                            CLINICAMENTE_SANO = Entity.CLINICAMENTE_SANO,
                            COEFICIENTE_INTELECTUAL = Entity.COEFICIENTE_INTELECTUAL,
                            CONDUCTA_RECLUSION = Entity.CONDUCTA_RECLUSION,
                            CONTINUAR_EDUCATIVO = Entity.CONTINUAR_EDUCATIVO,
                            CONTINUAR_LABORAL = Entity.CONTINUAR_LABORAL,
                            CONTINUAR_OTRO = Entity.CONTINUAR_OTRO,
                            CONTINUAR_PSICOLOGICO = Entity.CONTINUAR_PSICOLOGICO,
                            DANIO_CEREBRAL = Entity.DANIO_CEREBRAL,
                            DIAS_EFECTIVOS_TRABAJO = Entity.DIAS_EFECTIVOS_TRABAJO,
                            ESCOLARIDAD = Entity.ESCOLARIDAD,
                            ESTUDIA_ACTUALMENTE = Entity.ESTUDIA_ACTUALMENTE,
                            FRECUENCIA_VISITAS = Entity.FRECUENCIA_VISITAS,
                            ID_ANIO = Entity.ID_ANIO,
                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                            INDICE_PELIGROSIDAD = Entity.INDICE_PELIGROSIDAD,
                            DESCONOCE = Entity.DESCONOCE,
                            OCUPACION_ACTUAL = Entity.OCUPACION_ACTUAL,
                            ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                            ID_INGRESO = Entity.ID_INGRESO,
                            INSTITUCION = Entity.INSTITUCION,
                            NIVEL_SOCIOECONOMICO = Entity.NIVEL_SOCIOECONOMICO,
                            OTROS_ASPECTOS = Entity.OTROS_ASPECTOS,
                            OTROS_ASPECTOS_OPINION = Entity.OTROS_ASPECTOS_OPINION,
                            OTROS_CURSOS = Entity.OTROS_CURSOS,
                            CONYUGE = Entity.CONYUGE,
                            DIRECTOR = Entity.DIRECTOR,
                            DOMICILIO = Entity.DOMICILIO,
                            PADECIMIENTO = Entity.PADECIMIENTO,
                            OCUPACION_PREVIA = Entity.OCUPACION_PREVIA,
                            ID_CENTRO = Entity.ID_CENTRO,
                            RESPONSABLE = Entity.RESPONSABLE,
                            TRABAJA_ACTUALMENTE = Entity.TRABAJA_ACTUALMENTE,
                            TRATAMIENTO_ACTUAL = Entity.TRATAMIENTO_ACTUAL,
                            VERSION_DELITO = Entity.VERSION_DELITO
                        };

                        var _consecutivoTras = GetIDProceso<short>("TRASLADO_INTERNACIONAL_SANCION", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                        if (Entity.TRASLADO_INTERNACIONAL_SANCION != null && Entity.TRASLADO_INTERNACIONAL_SANCION.Any())
                            foreach (var item in Entity.TRASLADO_INTERNACIONAL_SANCION)
                            {
                                var _sancion = new TRASLADO_INTERNACIONAL_SANCION()
                                {
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_CONSEC = _consecutivoTras,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    SANCION = item.SANCION,
                                    SANCION_FEC = item.SANCION_FEC,
                                    MOTIVO = item.MOTIVO
                                };

                                _internacional.TRASLADO_INTERNACIONAL_SANCION.Add(_sancion);
                                _consecutivoTras++;
                            };

                        var _consecutivoVisit = GetIDProceso<short>("TRASLADO_INTERNACIONAL_VISITA", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                        if (Entity.TRASLADO_INTERNACIONAL_VISITA != null && Entity.TRASLADO_INTERNACIONAL_VISITA.Any())
                            foreach (var item in Entity.TRASLADO_INTERNACIONAL_VISITA)
                            {
                                var _visita = new TRASLADO_INTERNACIONAL_VISITA()
                                {
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_TIPO_REFERENCIA = item.ID_TIPO_REFERENCIA,
                                    MATERNO = item.MATERNO,
                                    PATERNO = item.PATERNO,
                                    ID_CONSEC = _consecutivoVisit,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    NOMBRE = item.NOMBRE
                                };

                                _internacional.TRASLADO_INTERNACIONAL_VISITA.Add(_visita);
                                _consecutivoVisit++;
                            };


                        Context.TRASLADO_INTERNACIONAL.Add(_internacional);
                    }

                    else
                    {

                        var _sanciones = Context.TRASLADO_INTERNACIONAL_SANCION.Where(c => c.ID_INGRESO == Entity.ID_INGRESO && c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_INGRESO == Entity.ID_INGRESO && c.ID_ANIO == Entity.ID_ANIO);
                        var _visitas = Context.TRASLADO_INTERNACIONAL_VISITA.Where(c => c.ID_INGRESO == Entity.ID_INGRESO && c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_INGRESO == Entity.ID_INGRESO && c.ID_ANIO == Entity.ID_ANIO);
                        _trasladoInter.ADICCION_CUALES = Entity.ADICCION_CUALES;
                        _trasladoInter.ADICCION_TOXICOS = Entity.ADICCION_TOXICOS;
                        _trasladoInter.AGRESIVIDAD = Entity.AGRESIVIDAD;
                        _trasladoInter.ANUENCIA_CUPO = Entity.ANUENCIA_CUPO;
                        _trasladoInter.ANUENCIA_FEC = Entity.ANUENCIA_FEC;
                        _trasladoInter.APOYO_PADRES = Entity.APOYO_PADRES;
                        _trasladoInter.CARTA_ARRAIGO = Entity.CARTA_ARRAIGO;
                        _trasladoInter.CAUSA_NO_ESTUDIA = Entity.CAUSA_NO_ESTUDIA;
                        _trasladoInter.CAUSA_NO_TRABAJA = Entity.CAUSA_NO_TRABAJA;
                        _trasladoInter.CAUSA_NO_VISITAS = Entity.CAUSA_NO_VISITAS;
                        _trasladoInter.CLINICAMENTE_SANO = Entity.CLINICAMENTE_SANO;
                        _trasladoInter.COEFICIENTE_INTELECTUAL = Entity.COEFICIENTE_INTELECTUAL;
                        _trasladoInter.CONDUCTA_RECLUSION = Entity.CONDUCTA_RECLUSION;
                        _trasladoInter.CONTINUAR_EDUCATIVO = Entity.CONTINUAR_EDUCATIVO;
                        _trasladoInter.CONTINUAR_LABORAL = Entity.CONTINUAR_LABORAL;
                        _trasladoInter.CONTINUAR_OTRO = Entity.CONTINUAR_OTRO;
                        _trasladoInter.CONTINUAR_PSICOLOGICO = Entity.CONTINUAR_PSICOLOGICO;
                        _trasladoInter.CONYUGE = Entity.CONYUGE;
                        _trasladoInter.DANIO_CEREBRAL = Entity.DANIO_CEREBRAL;
                        _trasladoInter.DESCONOCE = Entity.DESCONOCE;
                        _trasladoInter.DIAS_EFECTIVOS_TRABAJO = Entity.DIAS_EFECTIVOS_TRABAJO;
                        _trasladoInter.DIRECTOR = Entity.DIRECTOR;
                        _trasladoInter.DOMICILIO = Entity.DOMICILIO;
                        _trasladoInter.ESCOLARIDAD = Entity.ESCOLARIDAD;
                        _trasladoInter.ESTUDIA_ACTUALMENTE = Entity.ESTUDIA_ACTUALMENTE;
                        _trasladoInter.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                        _trasladoInter.FRECUENCIA_VISITAS = Entity.FRECUENCIA_VISITAS;
                        _trasladoInter.ID_ANIO = Entity.ID_ANIO;
                        _trasladoInter.ID_CENTRO = Entity.ID_CENTRO;
                        _trasladoInter.ID_ESTUDIO = Entity.ID_ESTUDIO;
                        _trasladoInter.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        _trasladoInter.ID_INGRESO = Entity.ID_INGRESO;
                        _trasladoInter.INDICE_PELIGROSIDAD = Entity.INDICE_PELIGROSIDAD;
                        _trasladoInter.INSTITUCION = Entity.INSTITUCION;
                        _trasladoInter.NIVEL_SOCIOECONOMICO = Entity.NIVEL_SOCIOECONOMICO;
                        _trasladoInter.OCUPACION_ACTUAL = Entity.OCUPACION_ACTUAL;
                        _trasladoInter.OCUPACION_PREVIA = Entity.OCUPACION_PREVIA;
                        _trasladoInter.OTROS_ASPECTOS = Entity.OTROS_ASPECTOS;
                        _trasladoInter.OTROS_ASPECTOS_OPINION = Entity.OTROS_ASPECTOS_OPINION;
                        _trasladoInter.OTROS_CURSOS = Entity.OTROS_CURSOS;
                        _trasladoInter.PADECIMIENTO = Entity.PADECIMIENTO;
                        _trasladoInter.RESPONSABLE = Entity.RESPONSABLE;
                        _trasladoInter.TRABAJA_ACTUALMENTE = Entity.TRABAJA_ACTUALMENTE;
                        _trasladoInter.TRATAMIENTO_ACTUAL = Entity.TRATAMIENTO_ACTUAL;
                        _trasladoInter.VERSION_DELITO = Entity.VERSION_DELITO;
                        Context.Entry(_trasladoInter).State = System.Data.EntityState.Modified;

                        if (_sanciones != null && _sanciones.Any())
                            foreach (var item in _sanciones)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;


                        if (_visitas != null && _visitas.Any())
                            foreach (var item in _visitas)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        var _consecutivoTras = GetIDProceso<short>("TRASLADO_INTERNACIONAL_SANCION", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                        if (Entity.TRASLADO_INTERNACIONAL_SANCION != null && Entity.TRASLADO_INTERNACIONAL_SANCION.Any())
                            foreach (var item in Entity.TRASLADO_INTERNACIONAL_SANCION)
                            {
                                var _sancion = new TRASLADO_INTERNACIONAL_SANCION()
                                {
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    SANCION = item.SANCION,
                                    SANCION_FEC = item.SANCION_FEC,
                                    MOTIVO = item.MOTIVO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_CONSEC = _consecutivoTras,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_INGRESO = Entity.ID_INGRESO
                                };

                                Context.TRASLADO_INTERNACIONAL_SANCION.Add(_sancion);
                                _consecutivoTras++;
                            };

                        var _consecutivoVisit = GetIDProceso<short>("TRASLADO_INTERNACIONAL_VISITA", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                        if (Entity.TRASLADO_INTERNACIONAL_VISITA != null && Entity.TRASLADO_INTERNACIONAL_VISITA.Any())
                            foreach (var item in Entity.TRASLADO_INTERNACIONAL_VISITA)
                            {
                                var _visita = new TRASLADO_INTERNACIONAL_VISITA()
                                {
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_TIPO_REFERENCIA = item.ID_TIPO_REFERENCIA,
                                    MATERNO = item.MATERNO,
                                    PATERNO = item.PATERNO,
                                    ID_CONSEC = _consecutivoVisit,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    NOMBRE = item.NOMBRE
                                };

                                Context.TRASLADO_INTERNACIONAL_VISITA.Add(_visita);
                                _consecutivoVisit++;
                            };
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
        }
    }
}
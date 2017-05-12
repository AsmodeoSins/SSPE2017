using SSP.Servidor;
using System.Linq;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cFichasJuridicas : SSP.Modelo.EntityManagerServer<FICHA_IDENTIFICACION_JURIDICA>
    {
        public cFichasJuridicas() { }
        public bool ValidaFicha(int IdImputado = 0, short IdIngreso = 0,short IdAnio = 0) 
        {
            if (GetData(x => x.ID_IMPUTADO == IdImputado && x.ID_INGRESO == IdIngreso && x.ID_ANIO == IdAnio).Any())
                return true;
            else
                return false;
        }

        public bool InsertarFichaNueva(FICHA_IDENTIFICACION_JURIDICA Entity) 
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _Existe = Context.FICHA_IDENTIFICACION_JURIDICA.Where(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();
                    if (_Existe == null)
                    {
                        var NuevaFicha = new FICHA_IDENTIFICACION_JURIDICA()
                        {
                            DEPARTAMENTO_JURIDICO = Entity.DEPARTAMENTO_JURIDICO,
                            ELABORO = Entity.ELABORO,
                            FICHA_FEC = Entity.FICHA_FEC,
                            ID_ANIO = Entity.ID_ANIO,
                            ID_CENTRO = Entity.ID_CENTRO,
                            ID_INGRESO = Entity.ID_INGRESO,
                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                            JEFE_DEPARTAMENTO = Entity.JEFE_DEPARTAMENTO,
                            OFICIO_ESTUDIO_SOLICITADO = Entity.OFICIO_ESTUDIO_SOLICITADO,
                            P3_PROCESOS_PENDIENTES = Entity.P3_PROCESOS_PENDIENTES,
                            P4_ULTIMO_EXAMEN_FEC = Entity.P4_ULTIMO_EXAMEN_FEC,
                            P5_RESOLUCION_APLAZADO = Entity.P5_RESOLUCION_APLAZADO,
                            P5_RESOLUCION_APROBADO = Entity.P5_RESOLUCION_APROBADO,
                            P5_RESOLUCION_MAYORIA = Entity.P5_RESOLUCION_MAYORIA,
                            P5_RESOLUCION_UNANIMIDAD = Entity.P5_RESOLUCION_UNANIMIDAD,
                            P6_CRIMINODINAMIA = Entity.P6_CRIMINODINAMIA,
                            P7_TRAMITE_LIBERTAD = Entity.P7_TRAMITE_LIBERTAD,
                            P7_TRAMITE_MODIFICACION = Entity.P7_TRAMITE_MODIFICACION,
                            P7_TRAMITE_TRASLADO = Entity.P7_TRAMITE_TRASLADO,
                            TRAMITE_DIAGNOSTICO = Entity.TRAMITE_DIAGNOSTICO,
                            TRAMITE_TRASLADO_VOLUNTARIO = Entity.TRAMITE_TRASLADO_VOLUNTARIO,
                            P2_CLAS_JURID = Entity.P2_CLAS_JURID,
                            P2_DELITO = Entity.P2_DELITO,
                            P2_JUZGADOS = Entity.P2_JUZGADOS,
                            P2_EJECUTORIA = Entity.P2_EJECUTORIA,
                            P2_FEC_INGRESO = Entity.P2_FEC_INGRESO,
                            P2_PROCEDENTE = Entity.P2_PROCEDENTE,
                            P2_PARTIR = Entity.P2_PARTIR,
                            P2_PENA_COMPURG = Entity.P2_PENA_COMPURG,
                            P2_PROCESOS = Entity.P2_PROCESOS,
                            P2_SENTENCIA = Entity.P2_SENTENCIA
                        };

                        Context.FICHA_IDENTIFICACION_JURIDICA.Add(NuevaFicha);
                    }
                    else
                    {
                        _Existe.DEPARTAMENTO_JURIDICO = Entity.DEPARTAMENTO_JURIDICO;
                        _Existe.ELABORO = Entity.ELABORO;
                        _Existe.FICHA_FEC = Entity.FICHA_FEC;
                        _Existe.ID_ANIO = Entity.ID_ANIO;
                        _Existe.ID_CENTRO = Entity.ID_CENTRO;
                        _Existe.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        _Existe.ID_INGRESO = Entity.ID_INGRESO;
                        _Existe.JEFE_DEPARTAMENTO = Entity.JEFE_DEPARTAMENTO;
                        _Existe.OFICIO_ESTUDIO_SOLICITADO = Entity.OFICIO_ESTUDIO_SOLICITADO;
                        _Existe.P2_CLAS_JURID = Entity.P2_CLAS_JURID;
                        _Existe.P2_DELITO = Entity.P2_DELITO;
                        _Existe.P2_EJECUTORIA = Entity.P2_EJECUTORIA;
                        _Existe.P2_FEC_INGRESO = Entity.P2_FEC_INGRESO;
                        _Existe.P2_JUZGADOS = Entity.P2_JUZGADOS;
                        _Existe.P2_PARTIR = Entity.P2_PARTIR;
                        _Existe.P2_PENA_COMPURG = Entity.P2_PENA_COMPURG;
                        _Existe.P2_PROCEDENTE = Entity.P2_PROCEDENTE;
                        _Existe.P2_PROCESOS = Entity.P2_PROCESOS;
                        _Existe.P2_SENTENCIA = Entity.P2_SENTENCIA;
                        _Existe.P3_PROCESOS_PENDIENTES = Entity.P3_PROCESOS_PENDIENTES;
                        _Existe.P4_ULTIMO_EXAMEN_FEC = Entity.P4_ULTIMO_EXAMEN_FEC;
                        _Existe.P5_RESOLUCION_APLAZADO = Entity.P5_RESOLUCION_APLAZADO;
                        _Existe.P5_RESOLUCION_APROBADO = Entity.P5_RESOLUCION_APROBADO;
                        _Existe.P5_RESOLUCION_MAYORIA = Entity.P5_RESOLUCION_MAYORIA;
                        _Existe.P5_RESOLUCION_UNANIMIDAD = Entity.P5_RESOLUCION_UNANIMIDAD;
                        _Existe.P6_CRIMINODINAMIA = Entity.P6_CRIMINODINAMIA;
                        _Existe.P7_TRAMITE_LIBERTAD = Entity.P7_TRAMITE_LIBERTAD;
                        _Existe.P7_TRAMITE_MODIFICACION = Entity.P7_TRAMITE_MODIFICACION;
                        _Existe.P7_TRAMITE_TRASLADO = Entity.P7_TRAMITE_TRASLADO;
                        _Existe.TRAMITE_DIAGNOSTICO = Entity.TRAMITE_DIAGNOSTICO;
                        _Existe.TRAMITE_TRASLADO_VOLUNTARIO = Entity.TRAMITE_TRASLADO_VOLUNTARIO;
                        Context.Entry(_Existe).State = System.Data.EntityState.Modified;
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

            return false;
        }
    }
}
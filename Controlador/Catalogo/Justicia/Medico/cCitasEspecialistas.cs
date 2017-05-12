using System.Linq;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cCitasEspecialistas : SSP.Modelo.EntityManagerServer<SSP.Servidor.ESPECIALISTA_CITA>
    {
        public cCitasEspecialistas() { }

        public bool ActualizarEspecialistaCita(SSP.Servidor.ESPECIALISTA_CITA Entity) 
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _DetallesEspecislistaCita = Context.ESPECIALISTA_CITA.FirstOrDefault(x => x.ID_CITA == Entity.ID_CITA && x.ID_SOLICITUD == Entity.ID_SOLICITUD && x.ID_CENTRO_UBI == Entity.ID_CENTRO_UBI);
                    var _DetallesCita = Context.ATENCION_CITA.FirstOrDefault(x => x.ID_CITA == Entity.ID_CITA);
                    if (_DetallesEspecislistaCita != null)
                    {
                        _DetallesEspecislistaCita.ID_ESPECIALISTA = Entity.ID_ESPECIALISTA;
                        _DetallesEspecislistaCita.REGISTRO_FEC = Entity.REGISTRO_FEC;
                        _DetallesEspecislistaCita.ID_USUARIO = Entity.ID_USUARIO;
                        Context.Entry(_DetallesEspecislistaCita).State = System.Data.EntityState.Modified;
                    };

                    if(_DetallesCita != null)
                    {
                        _DetallesCita.CITA_FECHA_HORA = Entity.ATENCION_CITA.CITA_FECHA_HORA;
                        _DetallesCita.CITA_HORA_TERMINA = Entity.ATENCION_CITA.CITA_HORA_TERMINA;
                        _DetallesCita.ESTATUS = Entity.ATENCION_CITA.ESTATUS;
                        _DetallesCita.ID_RESPONSABLE = Entity.ATENCION_CITA.ID_RESPONSABLE;
                        _DetallesCita.ID_USUARIO = Entity.ATENCION_CITA.ID_USUARIO;
                        Context.Entry(_DetallesCita).State = System.Data.EntityState.Modified;
                    };

                    if (Entity.SOL_INTERCONSULTA_INTERNA != null)
                    {
                        var _DetallesInterconsulta = Entity.SOL_INTERCONSULTA_INTERNA;
                        var _InterconsultaCorresp = Context.SOL_INTERCONSULTA_INTERNA.FirstOrDefault(x => x.ID_SOLICITUD == _DetallesInterconsulta.ID_SOLICITUD);
                        if (_InterconsultaCorresp != null)
                        {
                            var _SolicitudInterconsulta = _InterconsultaCorresp.INTERCONSULTA_SOLICITUD;
                            if (_SolicitudInterconsulta != null)
                                _SolicitudInterconsulta.ESTATUS = "N";

                            Context.Entry(_SolicitudInterconsulta).State = System.Data.EntityState.Modified;
                        };
                    };

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

        public bool InsertarCita(SSP.Servidor.ESPECIALISTA_CITA Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _AtencionCita = new SSP.Servidor.ATENCION_CITA()
                    {
                        CITA_FECHA_HORA = Entity.ATENCION_CITA.CITA_FECHA_HORA,
                        CITA_HORA_TERMINA = Entity.ATENCION_CITA.CITA_HORA_TERMINA,
                        ESTATUS = Entity.ATENCION_CITA.ESTATUS,
                        ID_ANIO = Entity.ATENCION_CITA.ID_ANIO,
                        ID_AREA = Entity.ATENCION_CITA.ID_AREA,
                        ID_ATENCION = Entity.ATENCION_CITA.ID_ATENCION,
                        ID_ATENCION_MEDICA = Entity.ATENCION_CITA.ID_ATENCION_MEDICA,
                        ID_CENTRO = Entity.ATENCION_CITA.ID_CENTRO,
                        ID_CITA = GetIDProceso<short>("ATENCION_CITA", "ID_CITA", "ID_CENTRO_UBI = " + Entity.ID_CENTRO_UBI),
                        ID_IMPUTADO = Entity.ATENCION_CITA.ID_IMPUTADO,
                        ID_INGRESO = Entity.ATENCION_CITA.ID_INGRESO,
                        ID_RESPONSABLE = Entity.ATENCION_CITA.ID_RESPONSABLE,
                        ID_TIPO_ATENCION = Entity.ATENCION_CITA.ID_TIPO_ATENCION,
                        ID_TIPO_SERVICIO = Entity.ATENCION_CITA.ID_TIPO_SERVICIO,
                        ID_USUARIO = Entity.ATENCION_CITA.ID_USUARIO,
                        ID_CENTRO_UBI = Entity.ID_CENTRO_UBI
                    };

                    var _AtencionEspecialista = new SSP.Servidor.ESPECIALISTA_CITA()
                    {
                        ID_CITA = _AtencionCita.ID_CITA,
                        ID_ESPECIALIDAD = Entity.ID_ESPECIALIDAD,
                        ID_ESPECIALISTA = Entity.ID_ESPECIALISTA,
                        ID_SOLICITUD = Entity.ID_SOLICITUD,
                        ID_USUARIO = Entity.ID_USUARIO,
                        REGISTRO_FEC = Entity.REGISTRO_FEC,
                        ID_CENTRO_UBI = Entity.ID_CENTRO_UBI
                    };

                    if (Entity.SOL_INTERCONSULTA_INTERNA != null)
                    {
                        var _DetallesInterconsulta = Entity.SOL_INTERCONSULTA_INTERNA;
                        var _InterconsultaCorresp = Context.SOL_INTERCONSULTA_INTERNA.FirstOrDefault(x => x.ID_SOLICITUD == _DetallesInterconsulta.ID_SOLICITUD);
                        if (_InterconsultaCorresp != null)
                        {
                            var _SolicitudInterconsulta = _InterconsultaCorresp.INTERCONSULTA_SOLICITUD;
                            if (_SolicitudInterconsulta != null)
                                _SolicitudInterconsulta.ESTATUS = "N";

                            Context.Entry(_SolicitudInterconsulta).State = System.Data.EntityState.Modified;
                        };
                    };

                    Context.ESPECIALISTA_CITA.Add(_AtencionEspecialista);
                    Context.ATENCION_CITA.Add(_AtencionCita);
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
    }
}
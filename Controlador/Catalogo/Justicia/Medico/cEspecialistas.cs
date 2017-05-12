using System.Linq;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEspecialistas : SSP.Modelo.EntityManagerServer<SSP.Servidor.ESPECIALISTA>
    {
        public cEspecialistas() { }

        public bool GuardarEspecialista(SSP.Servidor.ESPECIALISTA Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var ConsecutivoEspecialista = GetIDProceso<short>("ESPECIALISTA", "ID_ESPECIALISTA", string.Format("ID_CENTRO_UBI={0}", Entity.ID_CENTRO_UBI));
                    var _NuevaEspecialidad = new SSP.Servidor.ESPECIALISTA()
                    {
                        ESTATUS = Entity.ESTATUS,
                        ID_ESPECIALIDAD = Entity.ID_ESPECIALIDAD,
                        ID_PERSONA = Entity.ID_PERSONA,
                        ID_USUARIO = Entity.ID_USUARIO,
                        REGISTRO_FEC = Entity.REGISTRO_FEC,
                        ESPECIALISTA_MATERNO = Entity.ESPECIALISTA_MATERNO,
                        ESPECIALISTA_NOMBRE = Entity.ESPECIALISTA_NOMBRE,
                        ESPECIALISTA_PATERNO = Entity.ESPECIALISTA_PATERNO,
                        ID_CENTRO_UBI = Entity.ID_CENTRO_UBI,
                        ID_ESPECIALISTA = ConsecutivoEspecialista
                    };

                    Context.ESPECIALISTA.Add(_NuevaEspecialidad);
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

        public bool ActualizarEspecialista(SSP.Servidor.ESPECIALISTA Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EspecialistaElegido = Context.ESPECIALISTA.Where(x => x.ID_ESPECIALISTA == Entity.ID_ESPECIALISTA).FirstOrDefault();
                    if (_EspecialistaElegido != null)
                    {
                        _EspecialistaElegido.ESTATUS = Entity.ESTATUS;
                        _EspecialistaElegido.ESPECIALISTA_MATERNO = Entity.ESPECIALISTA_MATERNO;
                        _EspecialistaElegido.ESPECIALISTA_NOMBRE = Entity.ESPECIALISTA_NOMBRE;
                        _EspecialistaElegido.ESPECIALISTA_PATERNO = Entity.ESPECIALISTA_PATERNO;
                        Context.Entry(_EspecialistaElegido).State = System.Data.EntityState.Modified;
                        Context.SaveChanges();
                        transaccion.Complete();
                    };

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
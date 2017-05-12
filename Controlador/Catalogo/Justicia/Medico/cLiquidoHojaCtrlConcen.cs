namespace SSP.Controlador.Catalogo.Justicia
{
    public class cLiquidoHojaCtrlConcen : SSP.Modelo.EntityManagerServer<SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN>
    {
        public cLiquidoHojaCtrlConcen() { }

        public bool GuardarConcentrado(SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                        decimal _ConsecutivoLiquidoConcentrado = GetIDProceso<decimal>("LIQUIDO_HOJA_CTRL_CONCEN", "ID_LIQHOJACON", "1=1");
                        var _ConcentradoNuevo = new SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN()
                        {
                            BALANCE = Entity.BALANCE,
                            CONCENTRADO_FEC = Entity.CONCENTRADO_FEC,
                            ENTRADA = Entity.ENTRADA,
                            ID_CENTRO_UBI = Entity.ID_CENTRO_UBI,
                            ID_CONCENTIPO = Entity.ID_CONCENTIPO,
                            ID_HOSPITA = Entity.ID_HOSPITA,
                            REGISTRO_FEC = Entity.REGISTRO_FEC,
                            REGISTRO_USUARIO = Entity.REGISTRO_USUARIO,
                            SALIDA = Entity.SALIDA,
                            ID_LIQHOJACON = _ConsecutivoLiquidoConcentrado
                        };

                        Context.LIQUIDO_HOJA_CTRL_CONCEN.Add(_ConcentradoNuevo);
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
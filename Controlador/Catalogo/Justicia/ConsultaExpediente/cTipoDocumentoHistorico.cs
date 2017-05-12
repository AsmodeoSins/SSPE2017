using System.Linq;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTipoDocumentoHistorico : SSP.Modelo.EntityManagerServer<SSP.Servidor.TIPO_DOCUMENTO_HISTORICO>
    {
        public cTipoDocumentoHistorico() { }

        public System.Linq.IQueryable<SSP.Servidor.TIPO_DOCUMENTO_HISTORICO> ObtenerTodos()
        {
            try
            {
                return GetData(x => x.ESTATUS == "S").OrderBy(x => x.DESCR);
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
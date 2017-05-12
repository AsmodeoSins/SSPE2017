using System.Linq;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cHistoricoDocumento : SSP.Modelo.EntityManagerServer<SSP.Servidor.HISTORICO_DOCUMENTOS>
    {
        public cHistoricoDocumento() { }

        public System.Linq.IQueryable<SSP.Servidor.HISTORICO_DOCUMENTOS> ObtenerTodosByDepartamento(short IdAnio, short? IdCentro, int IdImputado, short? IdDepartamento)
        {
            try
            {
                return GetData(x => x.ID_ANIO == IdAnio && x.ID_CENTRO == IdCentro.Value && x.ID_IMPUTADO == IdImputado && x.ID_DEPARTAMENTO == IdDepartamento.Value).OrderBy(x => x.FECHA);
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
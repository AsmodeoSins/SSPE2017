using LinqKit;
using System.Linq;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cHojaEnfermeriaSolucion :  SSP.Modelo.EntityManagerServer<SSP.Servidor.HOJA_ENFERMERIA_SOLUCION>
    {
        public cHojaEnfermeriaSolucion() { }

        public IQueryable<SSP.Servidor.HOJA_ENFERMERIA_SOLUCION> ObtenerSolucionesTurnos(System.DateTime? _fecha, decimal? Hospitalizacion)
        {
            try
            {
                var predicado = PredicateBuilder.True<SSP.Servidor.HOJA_ENFERMERIA_SOLUCION>();
                //if (_fecha.HasValue)
                //    predicado = predicado.And(x => x.HOJA_ENFERMERIA.FECHA_HOJA.Value.Year == _fecha.Value.Year && x.HOJA_ENFERMERIA.FECHA_HOJA.Value.Month == _fecha.Value.Month && x.HOJA_ENFERMERIA.FECHA_HOJA.Value.Day == _fecha.Value.Day);
                if (Hospitalizacion.HasValue)
                    predicado = predicado.And(x => x.HOJA_ENFERMERIA.ID_HOSPITA == Hospitalizacion);

                predicado = predicado.And(x => x.TERMINO == "N");
                return GetData(predicado.Expand());
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
    }
}
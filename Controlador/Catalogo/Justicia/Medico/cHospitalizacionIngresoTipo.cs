using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqKit;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cHospitalizacionIngresoTipo : EntityManagerServer<HOSPITALIZACION_INGRESO_TIPO>
    {
        public IQueryable<HOSPITALIZACION_INGRESO_TIPO> ObtenerTipos()
        {
            try
            {
                return GetData();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}

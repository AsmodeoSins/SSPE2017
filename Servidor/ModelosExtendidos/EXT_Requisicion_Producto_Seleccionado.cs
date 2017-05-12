using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_Requisicion_Producto_Seleccionado
    {
        public int ID_REQUISICION { get; set; }
        public int ID_PRODUCTO { get; set; }
        public int? CANTIDAD { get; set; }
        public string NOMBRE_PRODUCTO { get; set; }
        public List<EXT_Requisicion_Centro_Producto> DETALLE_CENTRO_PRODUCTO { get; set; }
        private decimal? precio_unitario;
        public decimal? PRECIO_UNITARIO
        {
            get { return precio_unitario; }
            set
            {
                try
                {
                    if (value.HasValue && value.Value > Convert.ToDecimal(9999999999.99))
                        precio_unitario = Convert.ToDecimal(9999999999.99);
                    else
                        precio_unitario = value;
                }
                catch(Exception ex)
                {

                }
            }
        }
    }
}

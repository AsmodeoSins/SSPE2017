using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GESAL.Clases.Extendidas
{
    public class EXT_ProductoFiltrable : ValidationViewModelBase,ICloneable
    {
        public int ID_PRODUCTO { get; set; }
        public Nullable<int> ID_UNIDAD_MEDIDA { get; set; }
        public Nullable<int> ID_CATEGORIA { get; set; }
        public Nullable<short> ID_PRESENTACION { get; set; }
        public string NOMBRE { get; set; }
        public string DESCRIPCION { get; set; }
        public Nullable<decimal> PRECIO_COMPRA { get; set; }
        public Nullable<decimal> PRECIO_VENTA { get; set; }
        public string CODIGO_BARRAS { get; set; }
        public string PERIODO_COMPRA { get; set; }
        public Nullable<int> CANTIDAD_MINIMA { get; set; }
        public Nullable<int> CANTIDAD_MAXIMA { get; set; }
        public string GENERACION_CODIGO { get; set; }
        public string ACTIVO { get; set; }
        public Nullable<short> PROD_PORCENTAJE { get; set; }
        public Nullable<decimal> PROD_VENTA_REAL { get; set; }
        public Nullable<decimal> UTILIDAD { get; set; }
        public string ES_UT { get; set; }
        public string PRESENTACION { get; set; }
        public string ES_IVA { get; set; }
        public string ES_RED { get; set; }
        public string ES_AO { get; set; }
        public string ES_DT { get; set; }
        public Nullable<int> ID_SUBCATEGORIA { get; set; }
        public byte[] IMAGEN { get; set; } 

        public string CATEGORIA { get; set; }
        public string SUBCATEGORIA { get; set; }
        public string UNIDAD_MEDIDA { get; set; }

        private bool seleccionado = false;
        public bool Seleccionado
        {
            get
            {
                return seleccionado;
            }
            set
            {
                seleccionado = value;
                RaisePropertyChanged("Seleccionado");
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

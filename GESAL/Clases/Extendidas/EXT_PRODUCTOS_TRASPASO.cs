using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GESAL.Clases.Extendidas
{
    public class EXT_PRODUCTOS_TRASPASO : ValidationViewModelBase, IDataErrorInfo, ICloneable
    {
        public short ID_ALMACEN { get; set; }
        public string DESCR { get; set; }
        public int ID_PRODUCTO { get; set; }
        public string NOMBRE { get; set; }
        public decimal? ID_UNIDAD_MEDIDA { get; set; }
        public string UNIDAD_MEDIDA { get; set; }
        public decimal? ID_PRESENTACION { get; set; }
        public string PRESENTACION { get; set; }
        public Nullable<decimal> CANTIDAD { get; set; }
        private Nullable<decimal> ordenado;
        public Nullable<decimal> ORDENADO
        {
            get { return ordenado; }
            set
            {
                if (value.HasValue && value.Value > CANTIDAD)
                    ordenado = CANTIDAD;
                else
                {
                    ordenado = value;
                    if (value.HasValue)
                        OnPropertyValidateChanged("ORDENADO");
                    else
                        RaisePropertyChanged("ORDENADO");
                }
            }
        }
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

        public EXT_PRODUCTOS_TRASPASO()
        {
            setValidationRules();
        }

        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => ORDENADO, () => (ORDENADO>0 && ORDENADO<=CANTIDAD), "ORDENADO ES OBLIGATORIO!");
            RaisePropertyChanged("ORDENADO");
        }

        public void HabilitarValidacion()
        {
            setValidationRules();
        }

        public void InhabilitarValidacion()
        {
            base.ClearRules();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

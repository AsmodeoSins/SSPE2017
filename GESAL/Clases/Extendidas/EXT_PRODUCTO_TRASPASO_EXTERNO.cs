using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GESAL.Clases.Extendidas
{
    public class EXT_PRODUCTO_TRASPASO_EXTERNO : ValidationViewModelBase, ICloneable
    {
        public string CENTRO { get; set; }
        public int ID_ALMACEN { get; set; }
        public string ALMACEN { get; set; }
        public int ID_PRODUCTO { get; set; }
        public string NOMBRE { get; set; }
        public decimal? ID_UNIDAD_MEDIDA { get; set; }
        public string UNIDAD_MEDIDA { get; set; }
        public decimal? ID_PRESENTACION { get; set; }
        public string PRESENTACION { get; set; }
        private decimal? traspasar;
        public decimal? TRASPASAR
        {
            get { return traspasar; }
            set
            {
                traspasar = value;
                OnPropertyValidateChanged("TRASPASAR");
                setValidationRules();
            }
        }
        private bool isOrdenadoValido = false;
        public bool IsOrdenadoValido
        {
            get { return isOrdenadoValido; }
            set { isOrdenadoValido = value; RaisePropertyChanged("IsOrdenadoValido"); }
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

        public EXT_PRODUCTO_TRASPASO_EXTERNO()
        {
            setValidationRules();
        }

        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => TRASPASAR, () => (TRASPASAR>0 ), "TRASPASAR ES OBLIGATORIO!");
            RaisePropertyChanged("TRASPASAR");
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

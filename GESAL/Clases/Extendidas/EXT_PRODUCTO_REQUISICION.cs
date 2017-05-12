using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GESAL.Clases.Extendidas
{
    public class EXT_PRODUCTO_REQUISICION: ValidationViewModelBase, ICloneable
    {
        public int ID_PRODUCTO { get; set; }
        public string NOMBRE { get; set; }
        public decimal? ID_UNIDAD_MEDIDA { get; set; }
        public string UNIDAD_MEDIDA { get; set; }
        public decimal? ID_PRESENTACION { get; set; }
        public string PRESENTACION { get; set; }
        private decimal? ordenado;
        public decimal? ORDENADO
        {
            get { return ordenado; }
            set
            {
                ordenado = value;
                if (value.HasValue && value>0)
                {
                    OnPropertyValidateChanged("ORDENADO");
                    IsOrdenadoValido = true;
                }
                else
                {
                    RaisePropertyChanged("ORDENADO");
                    IsOrdenadoValido = false;
                }
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

        

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

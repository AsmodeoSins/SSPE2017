
using SSP.Servidor;
using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class RecetaMedica : ValidationViewModelBase
    {
        public SSP.Servidor.PRODUCTO PRODUCTO { get; set; }
        private decimal? cantidad;
        public decimal? CANTIDAD
        {
            get { return cantidad; }
            set
            {
                cantidad = value;
                RaisePropertyChanged("CANTIDAD");
            }
        }
        private short? duracion;
        public short? DURACION
        {
            get { return duracion; }
            set
            {
                duracion = value;
                RaisePropertyChanged("DURACION");
            }
        }
        private bool habilitar;
        public bool HABILITAR
        {
            get { return habilitar; }
            set
            {
                habilitar = value;
                RaisePropertyChanged("HABILITAR");
            }
        }
        private bool _ELIMINADO;
        public bool ELIMINADO
        {
            get { return _ELIMINADO; }
            set
            {
                _ELIMINADO = value;
                RaisePropertyChanged("ELIMINADO");
            }
        }
        private bool maniana;
        public bool HORA_MANANA
        {
            get { return maniana; }
            set
            {
                maniana = value;
                RaisePropertyChanged("HORA_MANANA");
                RaisePropertyChanged("HORA_TARDE");
                RaisePropertyChanged("HORA_NOCHE");
            }
        }
        private bool tarde;
        public bool HORA_TARDE
        {
            get { return tarde; }
            set
            {
                tarde = value;
                RaisePropertyChanged("HORA_MANANA");
                RaisePropertyChanged("HORA_TARDE");
                RaisePropertyChanged("HORA_NOCHE");
            }
        }
        private bool noche;
        public bool HORA_NOCHE
        {
            get { return noche; }
            set
            {
                noche = value;
                RaisePropertyChanged("HORA_MANANA");
                RaisePropertyChanged("HORA_TARDE");
                RaisePropertyChanged("HORA_NOCHE");
            }
        }
        public RECETA_MEDICA_DETALLE RECETA_MEDICA_DETALLE { get; set; }
        public int MEDIDA { get; set; }
        private short? presentacion;
        public short? PRESENTACION
        {
            get { return presentacion; }
            set
            {
                presentacion = value;
                RaisePropertyChanged("PRESENTACION");
            }
        }
        public string OBSERVACIONES { get; set; }
        public RecetaMedica()
        {
            setValidationRules();
        }
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => CANTIDAD, () => CANTIDAD.HasValue ? CANTIDAD.Value > 0 : false, "CANTIDAD ES OBLIGATORIA!");
            base.AddRule(() => DURACION, () => DURACION.HasValue ? DURACION.Value > 0 : false, "DURACION ES OBLIGATORIA!");
            base.AddRule(() => HORA_MANANA, () => HORA_MANANA || HORA_NOCHE || HORA_TARDE, "HORA ES OBLIGATORIA!");
            base.AddRule(() => HORA_NOCHE, () => HORA_MANANA || HORA_NOCHE || HORA_TARDE, "HORA ES OBLIGATORIA!");
            base.AddRule(() => HORA_TARDE, () => HORA_MANANA || HORA_NOCHE || HORA_TARDE, "HORA ES OBLIGATORIA!");
            base.AddRule(() => PRESENTACION, () => PRESENTACION.HasValue ? 
                PRESENTACION.Value > 0 ? 
                    true 
                : PRODUCTO != null ?
                    PRODUCTO.PRODUCTO_PRESENTACION_MED.Count <= 0 
                : true
            : PRODUCTO != null ?
                PRODUCTO.PRODUCTO_PRESENTACION_MED.Count <= 0
            : true, "PRESENTACION ES OBLIGATORIA!");
            RaisePropertyChanged("CANTIDAD");
            RaisePropertyChanged("DURACION");
            RaisePropertyChanged("HORA_MANANA");
            RaisePropertyChanged("HORA_NOCHE");
            RaisePropertyChanged("HORA_TARDE");
            RaisePropertyChanged("PRESENTACION");
        }
    }
}

using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using GESAL.Clases.Enums;

namespace GESAL.Clases.Extendidas
{
    public class EXT_RECEPCION_PRODUCTOS : ValidationViewModelBase, IDataErrorInfo, ICloneable
    {
        public int ID_PRODUCTO { get; set; }
        public string NOMBRE { get; set; }
        public string DESCRIPCION { get; set; }
        public string UNIDAD_MEDIDA { get; set; }
        public decimal ORDENADO { get; set; }
        private decimal recibido;
        public decimal RECIBIDO
        {
            get { return recibido; }
            set {
                if (value <= ORDENADO)
                    recibido = value;
                else
                    recibido = ORDENADO;
                RESTANTE = ORDENADO - recibido;
                RaisePropertyChanged("RECIBIDO");
               
            }
        }

        private decimal recibidoOld;
        public decimal RECIBIDOOLD
        {
            get { return recibidoOld; }
            set { recibidoOld = value; }
        }
        private decimal restante;
        public decimal RESTANTE
        {
            get { return restante; }
            set { restante = value; RaisePropertyChanged("RESTANTE"); }
        }
       
        private bool isENABLED_RECIBIDO=true;
        public bool ISENABLED_RECIBIDO
        {
            get { return isENABLED_RECIBIDO; }
            set { isENABLED_RECIBIDO = value; RaisePropertyChanged("ISENABLED_RECIBIDO"); }
        }

        private List<EXT_RECEPCION_PRODUCTO_DETALLE> recepcion_Producto_Detalle = null;
        public List<EXT_RECEPCION_PRODUCTO_DETALLE> RECEPCION_PRODUCTO_DETALLE
        {
            get { return recepcion_Producto_Detalle; }
            set { recepcion_Producto_Detalle = value; RaisePropertyChanged("RECEPCION_PRODUCTO_DETALLE"); }
        }

        private bool is_Checked=false;
        public bool IS_CHECKED
        {
            get { return is_Checked; }
            set {
                is_Checked = value;
                RaisePropertyChanged("IS_CHECKED");
                if (value == true)
                {
                    recibido = 0;
                    ISENABLED_RECIBIDO = false;
                    RECEPCION_PRODUCTO_DETALLE = new List<EXT_RECEPCION_PRODUCTO_DETALLE>();
                    ISCMDBLCLICK_HABILITADO = false;
                }
                else
                {
                    ISENABLED_RECIBIDO = true;
                    ISCMDBLCLICK_HABILITADO = true;
                }

                RaisePropertyChanged("RECIBIDO"); //es necesario para regla de validacion en juego con recibido
            }
        }

        private bool isCMDDBLCLICK_HABILITADO = true;
        public bool ISCMDBLCLICK_HABILITADO
        {
            get { return isCMDDBLCLICK_HABILITADO; }
            set { isCMDDBLCLICK_HABILITADO = value; RaisePropertyChanged("ISCMDBLCLICK_HABILITADO"); }
        }

        private INCIDENCIA_TIPO incidencia_tipo;
        public INCIDENCIA_TIPO INCIDENCIA_TIPO
        {
            get { return incidencia_tipo; }
            set { incidencia_tipo = value; RaisePropertyChanged("INCIDENCIA_TIPO"); }
        }

        private string incidencia_Observaciones = string.Empty;
        public string INCIDENCIA_OBSERVACIONES
        {
            get { return incidencia_Observaciones; }
            set { incidencia_Observaciones = value; RaisePropertyChanged("INCIDENCIA_OBSERVACIONES"); }
        }

        private DateTime? fechaRecalendarizacion = null;
        public DateTime? FechaRecalendarizacion
        {
            get { return fechaRecalendarizacion; }
            set { fechaRecalendarizacion = value; RaisePropertyChanged("FechaRecalendarizacion"); }
        }

        public EXT_RECEPCION_PRODUCTOS()
        {
            setValidationRules();
        }

        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => RECIBIDO, () => ((IS_CHECKED) ||(RECIBIDO>0 && RECIBIDO<=ORDENADO)), "RECIBIDO ES OBLIGATORIO!");
            RaisePropertyChanged("RECIBIDO");

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

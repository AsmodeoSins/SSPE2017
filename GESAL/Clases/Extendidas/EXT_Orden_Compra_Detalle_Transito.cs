using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.ViewModels;
using SSP.Servidor;
using System.ComponentModel;
using System.Windows;
namespace GESAL.Clases.Extendidas
{
    public class EXT_Orden_Compra_Detalle_Transito : ValidationViewModelBase
    {
        private int id_calendarizacion_entrega = 0;
        public int ID_CALENDARIZACION_ENTREGA
        {
            get { return id_calendarizacion_entrega; }
            set { id_calendarizacion_entrega = value; }
        }
        public int ID_ORDEN_COMPRA { get; set; }
        public short ID_ORDEN_COMPRA_DET { get; set; }
        public int ID_PRODUCTO { get; set; }
        public Nullable<int> CANTIDAD_ORDEN { get; set; }
        public Nullable<decimal> PRECIO_COMPRA { get; set; }
        public Nullable<decimal> CANTIDAD_ENTREGADA { get; set; }
        public Nullable<decimal> CANTIDAD_ENTREGADA_ENTRADA { get; set; }
        public Nullable<decimal> DIFERENCIA { get; set; }
        public Nullable<short> ID_ALMACEN { get; set; }
        public Nullable<decimal> CANTIDAD_TRANSITO { get; set; }

        public bool IS_SELECTED { get; set; }
        private decimal? programado = 0;
        public decimal? PROGRAMADO {
            get { return programado; }
            set { 
                programado = value;
                if (value != null && value > 0 && (value <= DIFERENCIA || (PROGRAMADO_ORIGINAL.HasValue && value<=PROGRAMADO_ORIGINAL.Value )))
                    IS_PROGRAMADOVALID = true;
                else
                    IS_PROGRAMADOVALID = false;
            }
        }
        private decimal? programado_Original = 0;
        public decimal? PROGRAMADO_ORIGINAL
        {
            get { return programado_Original; }
            set { programado_Original = value;}
        }
        private bool is_ProgramadoValid = false;
        public bool IS_PROGRAMADOVALID
        {
            get{ return is_ProgramadoValid;}
            set { is_ProgramadoValid = value;
            RaisePropertyChanged("IS_PROGRAMADOVALID");
            }
            }

        public int ID_PROVEEDOR { get; set; }
        public string PROVEEDOR_NOMBRE { get; set; }
        public string PRODUCTO_NOMBRE { get; set; }

        public int NUM_ORDEN { get; set; }

        private bool isEditable = true;

        public bool IsEditable
        {
            get { return isEditable; }
            set { 
                isEditable = value;
                RaisePropertyChanged("IsEditable");
                if (value)
                    IsEntregadoVisible = Visibility.Collapsed;
                else
                    IsEntregadoVisible = Visibility.Visible;
            }
        }

        private Visibility isEntregadoVisible = Visibility.Collapsed;
        public Visibility IsEntregadoVisible
        {
            get { return isEntregadoVisible; }
            set { isEntregadoVisible = value; RaisePropertyChanged("IsEntregadoVisible"); }
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

        private string estatus = "PR"; //defaul en PROGRAMADO
        public string Estatus 
        {
            get { return estatus; }
            set { estatus = value; RaisePropertyChanged("Estatus"); }
        }


    }
}

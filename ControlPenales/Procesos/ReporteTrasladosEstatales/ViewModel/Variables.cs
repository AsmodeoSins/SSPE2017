using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    partial class ReporteTrasladosEstatalesViewModel
    {

        private List<EXT_REPORTE_TRASLADO_DETALLE> ds_detalle;
        private List<EXT_REPORTE_TRASLADO_ENCABEZADO> ds_encabezado;

        private DateTime? fechaInicio = null;
        public DateTime? FechaInicio
        {
            get { return fechaInicio; }
            set 
            {
 
                fechaInicio = value;
                RaisePropertyChanged("FechaInicio");
                if (value.HasValue)
                {
                    IsFechainicioValid = true;
                    if (!fechaFin.HasValue || (fechaFin.HasValue && fechaFin.Value >= value))
                        IsFechafinValid = true;
                    else
                        IsFechafinValid = false;

                }
                else
                {
                    IsFechainicioValid = false;
                    IsFechafinValid = true;
                }
                    
            }
        }

        private DateTime? fechaFin = null;
        public DateTime? FechaFin
        {
            get { return fechaFin; }
            set
            {
                fechaFin = value;
                RaisePropertyChanged("FechaFin");
                if (!value.HasValue || !FechaInicio.HasValue || (FechaInicio.HasValue && value.HasValue && value.Value >= FechaInicio.Value))
                    IsFechafinValid = true;
                else
                    IsFechafinValid = false;

            }
        }

        private List<Tipo_Traslado> lstTipoTraslado = null;

        public List<Tipo_Traslado> LstTipoTraslado
        {
            get { return lstTipoTraslado; }
        }

        private string selectedTipoTrasladoValue="-1";
        public string SelectedTipoTrasladoValue
        {
            get { return selectedTipoTrasladoValue; }
            set { selectedTipoTrasladoValue = value; RaisePropertyChanged("SelectedTipoTrasladoValue"); }
        }

        private bool isIngresoChecked = false;
        public bool IsIngresoChecked
        {
            get { return isIngresoChecked; }
            set { isIngresoChecked = value; RaisePropertyChanged("IsIngresoChecked"); IsTipoMovimientoValid = true; }
        }

        private bool isEgresoChecked = false;
        public bool IsEgresoChecked
        {
            get { return isEgresoChecked; }
            set { isEgresoChecked = value; RaisePropertyChanged("IsEgresoChecked"); IsTipoMovimientoValid = true; }
        }

        private ReportViewer _reporte;
        public ReportViewer Reporte
        {
            get { return _reporte; }
            set { _reporte = value; RaisePropertyChanged("Reporte"); }
        }

        private bool isFechainicioValid=false;

        public bool IsFechainicioValid
        {
            get { return isFechainicioValid; }
            set { isFechainicioValid = value; RaisePropertyChanged("IsFechainicioValid"); }
        }

        private bool isFechafinValid = true;
        public bool IsFechafinValid
        {            
            get { return isFechafinValid; }
            set { isFechafinValid = value; RaisePropertyChanged("IsFechafinValid"); }
        }

        private bool isTipoMovimientoValid = false;
        public bool IsTipoMovimientoValid
        {
            get { return isTipoMovimientoValid; }
            set { isTipoMovimientoValid = value; RaisePropertyChanged("IsTipoMovimientoValid"); }
        }

        private Visibility reportViewerVisible = Visibility.Visible;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion

 
    }
}

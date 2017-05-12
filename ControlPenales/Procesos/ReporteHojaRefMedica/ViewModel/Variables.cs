using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class ReporteHojaRefMedicaViewModel
    {
        private Dictionary<string, string> diccionario_reporte = null;
        private IMPUTADO_TIPO_DOCUMENTO documento=null;
        #region Busqueda de Solicitudes de Interconsulta
        private short? anioBuscarInter = null;
        public short? AnioBuscarInter
        {
            get { return anioBuscarInter; }
            set { anioBuscarInter = value; RaisePropertyChanged("AnioBuscarInter"); }
        }

        private short? folioBuscarInter = null;
        public short? FolioBuscarInter
        {
            get { return folioBuscarInter; }
            set { folioBuscarInter = value; RaisePropertyChanged("FolioBuscarInter"); }
        }

        private string nombreBuscarInter = string.Empty;
        public string NombreBuscarInter
        {
            get { return nombreBuscarInter; }
            set { nombreBuscarInter = value; RaisePropertyChanged("NombreBuscarInter"); }
        }

        private string apellidoPaternoBuscarInter = string.Empty;
        public string ApellidoPaternoBuscarInter
        {
            get { return apellidoPaternoBuscarInter; }
            set { apellidoPaternoBuscarInter = value; RaisePropertyChanged("ApellidoPaternoBuscarInter"); }
        }

        private string apellidoMaternoBuscarInter = string.Empty;
        public string ApellidoMaternoBuscarInter
        {
            get { return apellidoMaternoBuscarInter; }
            set { apellidoMaternoBuscarInter = value; RaisePropertyChanged("ApellidoMaternoBuscarInter"); }
        }

        private DateTime? fechaInicialBuscarInter = null;
        public DateTime? FechaInicialBuscarInter
        {
            get { return fechaInicialBuscarInter; }
            set { fechaInicialBuscarInter = value; RaisePropertyChanged("FechaInicialBuscarInter"); }
        }

        private DateTime? fechaFinalBuscarInter = null;
        public DateTime? FechaFinalBuscarInter
        {
            get { return fechaFinalBuscarInter; }
            set { fechaFinalBuscarInter = value; RaisePropertyChanged("FechaFinalBuscarInter"); }
        }

        private ObservableCollection<INTERCONSULTA_SOLICITUD> listaInterconsultasBusqueda;
        public ObservableCollection<INTERCONSULTA_SOLICITUD> ListaInterconsultasBusqueda
        {
            get { return listaInterconsultasBusqueda; }
            set { listaInterconsultasBusqueda = value; RaisePropertyChanged("ListaInterconsultasBusqueda"); }
        }

        private INTERCONSULTA_SOLICITUD selectedInterconsultaBusqueda = null;
        public INTERCONSULTA_SOLICITUD SelectedInterconsultaBusqueda
        {
            get { return selectedInterconsultaBusqueda; }
            set { selectedInterconsultaBusqueda = value; RaisePropertyChanged("SelectedInterconsultaBusqueda"); }
        }

        private ObservableCollection<ATENCION_TIPO> lstAtencion_TipoBuscar = null;
        public ObservableCollection<ATENCION_TIPO> LstAtencion_TipoBuscar
        {
            get { return lstAtencion_TipoBuscar; }
            set { lstAtencion_TipoBuscar = value; RaisePropertyChanged("LstAtencion_TipoBuscar"); }
        }

        private short selectedAtencion_TipoBuscarValue = -1;
        public short SelectedAtencion_TipoBuscarValue
        {
            get { return selectedAtencion_TipoBuscarValue; }
            set { selectedAtencion_TipoBuscarValue = value; RaisePropertyChanged("SelectedAtencion_TipoBuscarValue"); }
        }

        #region Validaciones Busqueda Solicitudes
        private bool isFechaIniBusquedaSolValida = true;
        public bool IsFechaIniBusquedaSolValida
        {
            get { return isFechaIniBusquedaSolValida; }
            set { isFechaIniBusquedaSolValida = value; RaisePropertyChanged("IsFechaIniBusquedaSolValida"); }
        }
        #endregion
        #endregion
    }
}

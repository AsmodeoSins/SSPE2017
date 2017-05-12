using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class ReporteTarjetaInformativaDecesoViewModel
    {
        #region Busqueda de Solicitudes de Interconsulta
        private short? anioBuscarDeceso = null;
        public short? AnioBuscarDeceso
        {
            get { return anioBuscarDeceso; }
            set { anioBuscarDeceso = value; RaisePropertyChanged("AnioBuscarDeceso"); }
        }

        private short? folioBuscarDeceso = null;
        public short? FolioBuscarDeceso
        {
            get { return folioBuscarDeceso; }
            set { folioBuscarDeceso = value; RaisePropertyChanged("FolioBuscarDeceso"); }
        }

        private string nombreBuscarDeceso = string.Empty;
        public string NombreBuscarDeceso
        {
            get { return nombreBuscarDeceso; }
            set { nombreBuscarDeceso = value; RaisePropertyChanged("NombreBuscarDeceso"); }
        }

        private string apellidoPaternoBuscarDeceso = string.Empty;
        public string ApellidoPaternoBuscarDeceso
        {
            get { return apellidoPaternoBuscarDeceso; }
            set { apellidoPaternoBuscarDeceso = value; RaisePropertyChanged("ApellidoPaternoBuscarDeceso"); }
        }

        private string apellidoMaternoBuscarDeceso = string.Empty;
        public string ApellidoMaternoBuscarDeceso
        {
            get { return apellidoMaternoBuscarDeceso; }
            set { apellidoMaternoBuscarDeceso = value; RaisePropertyChanged("ApellidoMaternoBuscarDeceso"); }
        }

        private DateTime? fechaInicialBuscarDeceso = null;
        public DateTime? FechaInicialBuscarDeceso
        {
            get { return fechaInicialBuscarDeceso; }
            set { fechaInicialBuscarDeceso = value; RaisePropertyChanged("FechaInicialBuscarDeceso"); }
        }

        private DateTime? fechaFinalBuscarDeceso = null;
        public DateTime? FechaFinalBuscarDeceso
        {
            get { return fechaFinalBuscarDeceso; }
            set { fechaFinalBuscarDeceso = value; RaisePropertyChanged("FechaFinalBuscarDeceso"); }
        }

        private ObservableCollection<NOTA_DEFUNCION> listaDecesoBusqueda=null;
        public ObservableCollection<NOTA_DEFUNCION> ListaDecesoBusqueda
        {
            get { return listaDecesoBusqueda; }
            set { listaDecesoBusqueda = value; RaisePropertyChanged("ListaInterconsultasBusqueda"); }
        }

        private NOTA_DEFUNCION selectedDecesoBusqueda = null;
        public NOTA_DEFUNCION SelectedDecesoBusqueda
        {
            get { return selectedDecesoBusqueda; }
            set { selectedDecesoBusqueda = value; RaisePropertyChanged("SelectedDecesoBusqueda"); }
        }

        #region Validaciones Busqueda Solicitudes
        private bool isFechaIniBusquedaDecesoValida = true;
        public bool IsFechaIniBusquedaDecesoValida
        {
            get { return isFechaIniBusquedaDecesoValida; }
            set { isFechaIniBusquedaDecesoValida = value; RaisePropertyChanged("IsFechaIniBusquedaDecesoValida"); }
        }
        #endregion
        #endregion
    }
}

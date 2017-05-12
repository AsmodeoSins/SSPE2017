using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    partial class AutorizaIngresoTrasladoViewModel
    {
        #region Variables
        private ObservableCollection<EXT_TRASLADO_DETALLE> traslados;
        public ObservableCollection<EXT_TRASLADO_DETALLE> Traslados
        {
            get { return traslados; }
            set { traslados = value; RaisePropertyChanged("Traslados"); }
        }
        private EXT_TRASLADO_DETALLE selectedTraslado;
        public EXT_TRASLADO_DETALLE SelectedTraslado
        {
            get { return selectedTraslado; }
            set { selectedTraslado = value; RaisePropertyChanged("SelectedTraslado"); }
        }

        #region Variables de Controles
        private Visibility datosTrasladosVisible=Visibility.Hidden;
        public Visibility DatosTrasladosVisible
        {
            get { return datosTrasladosVisible; }
            set { datosTrasladosVisible = value; RaisePropertyChanged("DatosTrasladosVisible"); }
        }

        private bool menuAutorizarEnabled = false;
        public bool MenuAutorizarEnabled
        {
            get { return menuAutorizarEnabled; }
            set { menuAutorizarEnabled = value; RaisePropertyChanged("MenuAutorizarEnabled"); }
        }
        #endregion

        #region Datos Ingreso
        private byte[] imagenIngreso;
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; RaisePropertyChanged("ImagenIngreso"); }
        }
        #endregion

        #region Datos Traslados
        private DateTime? dt_Fecha;
        public DateTime? DT_Fecha
        {
            get { return dt_Fecha; }
            set { dt_Fecha = value; RaisePropertyChanged("DT_Fecha"); }
        }

        private string dt_Motivo;
        public string DT_Motivo
        {
            get { return dt_Motivo; }
            set { dt_Motivo = value; RaisePropertyChanged("DT_Motivo"); }
        }

        private string dt_Justificacion;
        public string DT_Justificacion
        {
            get { return dt_Justificacion; }
            set { dt_Justificacion = value; RaisePropertyChanged("DT_Justificacion"); }
        }

        private string dt_Centro_Origen;
        public string DT_Centro_Origen
        { 
            get { return dt_Centro_Origen; }
            set { dt_Centro_Origen = value; RaisePropertyChanged("DT_Centro_Origen"); }
        }

        private string dt_Oficio_Autorizacion;
        public string DT_Oficio_Autorizacion
        {
            get { return dt_Oficio_Autorizacion; }
            set { dt_Oficio_Autorizacion = value; RaisePropertyChanged("DT_Oficio_Autorizacion"); }
        }

        private string dt_Autorizado;
        public string DT_Autorizado
        {
            get { return dt_Autorizado; }
            set { dt_Autorizado = value; RaisePropertyChanged("DT_Autorizado"); }
        }
        #endregion
        #endregion
    }
}

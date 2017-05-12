
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {

     
        private DateTime? fecDocumentoAD;
        public DateTime? FecDocumentoAD
        {
            get { return fecDocumentoAD; }
            set { fecDocumentoAD = value; OnPropertyChanged("FecDocumentoAD"); }
        }
        
        private DateTime? fecNotificacionAD;
        public DateTime? FecNotificacionAD
        {
            get { return fecNotificacionAD; }
            set { fecNotificacionAD = value; OnPropertyChanged("FecNotificacionAD"); }
        }

        private DateTime? fecSuspencionSentenciaAD;
        public DateTime? FecSuspencionSentenciaAD
        {
            get { return fecSuspencionSentenciaAD; }
            set { fecSuspencionSentenciaAD = value; OnPropertyChanged("FecSuspencionSentenciaAD"); }
        }

        private string noOficioAD;
        public string NoOficioAD
        {
            get { return noOficioAD; }
            set { noOficioAD = value; OnPropertyChanged("NoOficioAD"); }
        }

        private string noAD;
        public string NoAD
        {
            get { return noAD; }
            set { noAD = value; OnPropertyChanged("NoAD"); }
        }


        private short? autoridadInformaAD;
        public short? AutoridadInformaAD
        {
            get { return autoridadInformaAD; }
            set { autoridadInformaAD = value; OnPropertyChanged("AutoridadInformaAD"); }
        }

        private DateTime? fecSentenciaAmparoAD;
        public DateTime? FecSentenciaAmparoAD
        {
            get { return fecSentenciaAmparoAD; }
            set { fecSentenciaAmparoAD = value; OnPropertyChanged("FecSentenciaAmparoAD"); }
        }


        private ObservableCollection<AMPARO_DIRECTO_SENTENCIA> lstSentenciaAD;
        public ObservableCollection<AMPARO_DIRECTO_SENTENCIA> LstSentenciaAD
        {
            get { return lstSentenciaAD; }
            set { lstSentenciaAD = value; OnPropertyChanged("LstSentenciaAD"); }
        }

        private short? resultadoSentenciaAD;
        public short? ResultadoSentenciaAD
        {
            get { return resultadoSentenciaAD; }
            set { resultadoSentenciaAD = value; OnPropertyChanged("ResultadoSentenciaAD"); }
        }

        private short? autoridadPronunciaSentenciaAmparo;
        public short? AutoridadPronunciaSentenciaAmparo
        {
            get { return autoridadPronunciaSentenciaAmparo; }
            set { autoridadPronunciaSentenciaAmparo = value; OnPropertyChanged("AutoridadPronunciaSentenciaAmparo"); }
        }
    }
}

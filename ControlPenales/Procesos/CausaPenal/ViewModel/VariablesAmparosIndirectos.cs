
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        private DateTime? fecDocumentoAI;
        public DateTime? FecDocumentoAI
        {
            get { return fecDocumentoAI; }
            set { fecDocumentoAI = value; OnPropertyChanged("FecDocumentoAI"); }
        }

        private DateTime? fecNotificacionAI;
        public DateTime? FecNotificacionAI
        {
            get { return fecNotificacionAI; }
            set { fecNotificacionAI = value; OnPropertyChanged("FecNotificacionAI"); }
        }

        private DateTime? fecSuspencionAccionAI;
        public DateTime? FecSuspencionAccionAI
        {
            get { return fecSuspencionAccionAI; }
            set { fecSuspencionAccionAI = value; OnPropertyChanged("FecSuspencionAccionAI"); }
        }

        private string noOficioAI;
        public string NoOficioAI
        {
            get { return noOficioAI; }
            set { noOficioAI = value; OnPropertyChanged("NoOficioAI"); }
        }

        private string noAI;
        public string NoAI
        {
            get { return noAI; }
            set { noAI = value; OnPropertyChanged("NoAI"); }
        }

        //AUTORIDAD INTERNA
        private short? selectedJuzgadoAI;
        public short? SelectedJuzgadoAI
        {
            get { return selectedJuzgadoAI; }
            set { selectedJuzgadoAI = value; OnPropertyChanged("SelectedJuzgadoAI"); }
        }

        private DateTime? fecSentenciaAI;
        public DateTime? FecSentenciaAI
        {
            get { return fecSentenciaAI; }
            set { fecSentenciaAI = value; OnPropertyChanged("FecSentenciaAI"); }
        }

        //RESOLUCION DE AMPARO
        private short? selectedSentenciaAI;
        public short? SelectedSentenciaAI
        {
            get { return selectedSentenciaAI; }
            set { selectedSentenciaAI = value; OnPropertyChanged("SelectedSentenciaAI"); }
        }

        private DateTime? fecEjecutoriaAI;
        public DateTime? FecEjecutoriaAI
        {
            get { return fecEjecutoriaAI; }
            set { fecEjecutoriaAI = value; OnPropertyChanged("FecEjecutoriaAI"); }
        }

        private DateTime? fecRevisionSentenciaAI;
        public DateTime? FecRevisionSentenciaAI
        {
            get { return fecRevisionSentenciaAI; }
            set { fecRevisionSentenciaAI = value; OnPropertyChanged("FecRevisionSentenciaAI"); }
        }

        //TIPO AMPARO INDIRECTO
        //private AMPARO_INDIRECTO_TIPO selectedTipoAmparoIndirecto;
        //public AMPARO_INDIRECTO_TIPO SelectedTipoAmparoIndirecto
        //{
        //    get { return selectedTipoAmparoIndirecto; }
        //    set { selectedTipoAmparoIndirecto = value; OnPropertyChanged("SelectedTipoAmparoIndirecto"); }
        //}
        
    }
}

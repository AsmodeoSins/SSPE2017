using SSP.Servidor;
using System;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        private SENTENCIA selectedSentencia;
        public SENTENCIA SelectedSentencia
        {
            get { return selectedSentencia; }
            set { selectedSentencia = value; OnPropertyChanged("SelectedSentencia"); }
        }

        private bool fecSValid = false;
        public bool FecSValid
        {
            get { return fecSValid; }
            set { fecSValid = value; OnPropertyChanged("FecSValid"); }
        }

        private bool fecEjecutoriaSValid = false;
        public bool FecEjecutoriaSValid
        {
            get { return fecEjecutoriaSValid; }
            set { fecEjecutoriaSValid = value; OnPropertyChanged("FecEjecutoriaSValid"); }
        }

        private bool fecInicioCompurgacionSValid = false;
        public bool FecInicioCompurgacionSValid
        {
            get { return fecInicioCompurgacionSValid; }
            set { fecInicioCompurgacionSValid = value; OnPropertyChanged("FecInicioCompurgacionSValid"); }
        }

        private bool fecRealCompurgacionSValid = false;
        public bool FecRealCompurgacionSValid
        {
            get { return fecRealCompurgacionSValid; }
            set { fecRealCompurgacionSValid = value; OnPropertyChanged("FecRealCompurgacionSValid"); }
        }

        private DateTime? fecS;
        public DateTime? FecS
        {
            get { return fecS; }
            set
            {
                fecS = value;
                if (value.HasValue)
                    FecSValid = true;
                else
                    FecSValid = false;
                OnPropertyChanged("FecS");
            }
        }

        private DateTime? fecEjecutoriaS;
        public DateTime? FecEjecutoriaS
        {
            get { return fecEjecutoriaS; }
            set
            {
                fecEjecutoriaS = value;
                if (value.HasValue)
                    FecEjecutoriaSValid = true;
                else
                    FecEjecutoriaSValid = false;

                OnPropertyChanged("FecEjecutoriaS");
            }
        }

        private DateTime? fecInicioCompurgacionS;
        public DateTime? FecInicioCompurgacionS
        {
            get { return fecInicioCompurgacionS; }
            set
            {
                fecInicioCompurgacionS = value;
                if (value != null)
                { }// ValidacionEmpalmeFechas();
                if (value.HasValue)
                    FecInicioCompurgacionSValid = true;
                else
                    FecInicioCompurgacionSValid = false;
                OnPropertyChanged("FecInicioCompurgacionS");
            }
        }

        private short? aniosS;
        public short? AniosS
        {
            get { return aniosS; }
            set
            {
                aniosS = value;
                //SetValidacionesSentencia();
                OnPropertyChanged("AniosS");
            }
        }

        private short? mesesS;
        public short? MesesS
        {
            get { return mesesS; }
            set
            {
                mesesS = value;
                //SetValidacionesSentencia();
                OnPropertyChanged("MesesS");
            }
        }

        private short? diasS;
        public short? DiasS
        {
            get { return diasS; }
            set
            {
                diasS = value;
                //SetValidacionesSentencia();
                OnPropertyChanged("DiasS");
            }
        }

        private string multaS;
        public string MultaS
        {
            get { return multaS; }
            set { multaS = value; OnPropertyChanged("MultaS"); }
        }

        private bool multaPagadaS;
        public bool MultaPagadaS
        {
            get { return multaPagadaS; }
            set { multaPagadaS = value; OnPropertyChanged("MultaPagadaS"); }
        }

        private string reparacionDanioS;
        public string ReparacionDanioS
        {
            get { return reparacionDanioS; }
            set { reparacionDanioS = value; OnPropertyChanged("ReparacionDanioS"); }
        }

        private bool reparacionDanioPagadaS;
        public bool ReparacionDanioPagadaS
        {
            get { return reparacionDanioPagadaS; }
            set { reparacionDanioPagadaS = value; OnPropertyChanged("ReparacionDanioPagadaS"); }
        }

        private string sustitucionPenaS;
        public string SustitucionPenaS
        {
            get { return sustitucionPenaS; }
            set { sustitucionPenaS = value; OnPropertyChanged("SustitucionPenaS"); }
        }

        private bool sustitucionPenaPagadaS;
        public bool SustitucionPenaPagadaS
        {
            get { return sustitucionPenaPagadaS; }
            set { sustitucionPenaPagadaS = value; OnPropertyChanged("SustitucionPenaPagadaS"); }
        }

        private string suspensionCondicionalS;
        public string SuspensionCondicionalS
        {
            get { return suspensionCondicionalS; }
            set { suspensionCondicionalS = value; OnPropertyChanged("SuspensionCondicionalS"); }
        }

        private string observacionS;
        public string ObservacionS
        {
            get { return observacionS; }
            set { observacionS = value; OnPropertyChanged("ObservacionS"); }
        }

        private string motivoCancelacionAntecedenteS;
        public string MotivoCancelacionAntecedenteS
        {
            get { return motivoCancelacionAntecedenteS; }
            set { motivoCancelacionAntecedenteS = value; OnPropertyChanged("MotivoCancelacionAntecedenteS"); }
        }

        private short? gradoParticipacionS;
        public short? GradoParticipacionS
        {
            get { return gradoParticipacionS; }
            set { gradoParticipacionS = value; OnPropertyChanged("GradoParticipacionS"); }
        }

        private short? gradoAutoriaS;
        public short? GradoAutoriaS
        {
            get { return gradoAutoriaS; }
            set { gradoAutoriaS = value; OnPropertyChanged("GradoAutoriaS"); }
        }

        private short? aniosAbonadosS;
        public short? AniosAbonadosS
        {
            get { return aniosAbonadosS; }
            set { aniosAbonadosS = value; OnPropertyChanged("AniosAbonadosS"); }
        }

        private short? mesesAbonadosS;
        public short? MesesAbonadosS
        {
            get { return mesesAbonadosS; }
            set { mesesAbonadosS = value; OnPropertyChanged("MesesAbonadosS"); }
        }

        private short? diasAbonadosS;
        public short? DiasAbonadosS
        {
            get { return diasAbonadosS; }
            set { diasAbonadosS = value; OnPropertyChanged("DiasAbonadosS"); }
        }

        private bool tabSentenciaSelected;
        public bool TabSentenciaSelected
        {
            get { return tabSentenciaSelected; }
            set { tabSentenciaSelected = value; OnPropertyChanged("TabSentenciaSelected"); }
        }

        private bool tabCoparticipeSelected;
        public bool TabCoparticipeSelected
        {
            get { return tabCoparticipeSelected; }
            set { tabCoparticipeSelected = value; OnPropertyChanged("TabCoparticipeSelected"); }
        }

        private DateTime? fecRealCompurgacionS;
        public DateTime? FecRealCompurgacionS
        {
            get { return fecRealCompurgacionS; }
            set
            {
                fecRealCompurgacionS = value;
                if (value.HasValue)
                    FecRealCompurgacionSValid = true;
                else
                    FecRealCompurgacionSValid = false;
                OnPropertyChanged("FecRealCompurgacionS");
            }
        }

        private SENTENCIA sentencia;
        public SENTENCIA Sentencia
        {
            get { return sentencia; }
            set { sentencia = value; OnPropertyChanged("Sentencia"); }
        }

        //SENTENCIA DELITO
        private ObservableCollection<SENTENCIA_DELITO> lstSentenciaDelitos;
        public ObservableCollection<SENTENCIA_DELITO> LstSentenciaDelitos
        {
            get { return lstSentenciaDelitos; }
            set { lstSentenciaDelitos = value; OnPropertyChanged("LstSentenciaDelitos"); }
        }

        private SENTENCIA_DELITO selectedSentenciaDelito;
        public SENTENCIA_DELITO SelectedSentenciaDelito
        {
            get { return selectedSentenciaDelito; }
            set
            {
                selectedSentenciaDelito = value;

                DTitulo = DGrupo = DDelito = DModalidad = DDetalle = DArticulo = DFuero = string.Empty;
                if (value != null)
                {
                    //POPULA EL DETALLE DE DELITO
                    DTitulo = value.MODALIDAD_DELITO.DELITO.DELITO_GRUPO.DELITO_TITULO.DESCR;
                    DGrupo = value.MODALIDAD_DELITO.DELITO.DELITO_GRUPO.DESCR;
                    DDelito = value.MODALIDAD_DELITO.DELITO.DESCR;
                    DModalidad = value.MODALIDAD_DELITO.DESCR;
                    DDetalle = value.MODALIDAD_DELITO.DELITO.DETALLE;
                    DArticulo = value.OBJETO;
                    DFuero = value.ID_FUERO;

                    //EliminarSentenciaDelito = value.SENTENCIA == null ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                { }// EliminarSentenciaDelito = Visibility.Collapsed;
                OnPropertyChanged("SelectedSentenciaDelito");
            }
        }

        private ObservableCollection<GRADO_AUTORIA> lstGradosAutoria;
        public ObservableCollection<GRADO_AUTORIA> LstGradosAutoria
        {
            get { return lstGradosAutoria; }
            set { lstGradosAutoria = value; OnPropertyChanged("LstGradosAutoria"); }
        }

        private ObservableCollection<GRADO_PARTICIPACION> lstGradosParticipacion;
        public ObservableCollection<GRADO_PARTICIPACION> LstGradosParticipacion
        {
            get { return lstGradosParticipacion; }
            set { lstGradosParticipacion = value; OnPropertyChanged("LstGradosParticipacion"); }
        }

        private bool sentenciaDelitoEmpty = true;
        public bool SentenciaDelitoEmpty
        {
            get { return sentenciaDelitoEmpty; }
            set { sentenciaDelitoEmpty = value; OnPropertyChanged("SentenciaDelitoEmpty"); }
        }

        //RADIOBUTTONS
        private bool multaSi;
        public bool MultaSi
        {
            get { return multaSi; }
            set { multaSi = value; OnPropertyChanged("MultaSi"); }
        }


        private string _Delitos;
        public string Delitos
        {
            get { return _Delitos; }
            set { _Delitos = value; OnPropertyChanged("Delitos"); }
        }

        private bool multaNo;
        public bool MultaNo
        {
            get { return multaNo; }
            set { multaNo = value; OnPropertyChanged("MultaNo"); }
        }

        private bool reparacionSi;
        public bool ReparacionSi
        {
            get { return reparacionSi; }
            set { reparacionSi = value; OnPropertyChanged("ReparacionSi"); }
        }

        private bool reparacionNo;
        public bool ReparacionNo
        {
            get { return reparacionNo; }
            set { reparacionNo = value; OnPropertyChanged("ReparacionNo"); }
        }

        private bool sustitucionSi;
        public bool SustitucionSi
        {
            get { return sustitucionSi; }
            set { sustitucionSi = value; OnPropertyChanged("SustitucionSi"); }
        }

        private bool sustitucionNo;
        public bool SustitucionNo
        {
            get { return sustitucionNo; }
            set { sustitucionNo = value; OnPropertyChanged("SustitucionNo"); }
        }

    }
}


using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {

        private short? tipoR;
        public short? TipoR
        {
            get { return tipoR; }
            set { tipoR = value; OnPropertyChanged("TipoR"); }
        }

        private short? tribunalR;
        public short? TribunalR
        {
            get { return tribunalR; }
            set { tribunalR = value; OnPropertyChanged("TribunalR"); }
        }

        private string fueroR;
        public string FueroR
        {
            get { return fueroR; }
            set { fueroR = value; OnPropertyChanged("FueroR"); }
        }

        private int? resultadoR;
        public int? ResultadoR
        {
            get { return resultadoR; }
            set { resultadoR = value; OnPropertyChanged("ResultadoR"); }
        }

        private DateTime? fecR;
        public DateTime? FecR
        {
            get { return fecR; }
            set { fecR = value;
            SetValidacionesRecurso();
                OnPropertyChanged("FecR"); }
        }

        private string tocaPenalR;
        public string TocaPenalR
        {
            get { return tocaPenalR; }
            set { tocaPenalR = value; OnPropertyChanged("TocaPenalR"); }
        }

        private string noOficioR;
        public string NoOficioR
        {
            get { return noOficioR; }
            set { noOficioR = value; OnPropertyChanged("NoOficioR"); }
        }

        private string resolucionR;
        public string ResolucionR
        {
            get { return resolucionR; }
            set { resolucionR = value; OnPropertyChanged("ResolucionR"); }
        }

        private DateTime? fecResolucionR;
        public DateTime? FecResolucionR
        {
            get { return fecResolucionR; }
            set { fecResolucionR = value;
            SetValidacionesRecurso();
                OnPropertyChanged("FecResolucionR"); }
        }

        private string multaR;
        public string MultaR
        {
            get { return multaR; }
            set { multaR = value; OnPropertyChanged("MultaR"); }
        }

        private string reparacionDanioR;
        public string ReparacionDanioR
        {
            get { return reparacionDanioR; }
            set { reparacionDanioR = value; OnPropertyChanged("ReparacionDanioR"); }
        }

        private string sustitucionPenaR;
        public string SustitucionPenaR
        {
            get { return sustitucionPenaR; }
            set { sustitucionPenaR = value; OnPropertyChanged("SustitucionPenaR"); }
        }

        private string multaCondicionalR;
        public string MultaCondicionalR
        {
            get { return multaCondicionalR; }
            set { multaCondicionalR = value; OnPropertyChanged("MultaCondicionalR"); }
        }

        private short? aniosR;
        public short? AniosR
        {
            get { return aniosR; }
            set { aniosR = value; OnPropertyChanged("AniosR"); }
        }

        private short? mesesR;
        public short? MesesR
        {
            get { return mesesR; }
            set { mesesR = value; OnPropertyChanged("MesesR"); }
        }

        private short? diasR;
        public short? DiasR
        {
            get { return diasR; }
            set { diasR = value; OnPropertyChanged("DiasR"); }
        }

        private bool tabRecursoSelected;
        public bool TabRecursoSelected
        {
            get { return tabRecursoSelected; }
            set { tabRecursoSelected = value; OnPropertyChanged("TabRecursoSelected"); }
        }

        private bool tabRecursosSelected;
        public bool TabRecursosSelected
        {
            get { return tabRecursosSelected; }
            set { tabRecursosSelected = value; OnPropertyChanged("TabRecursosSelected"); }
        }

     
        //private TIPO_RECURSO selectedTipoRecurso;
        //public TIPO_RECURSO SelectedTipoRecurso
        //{
        //    get { return selectedTipoRecurso; }
        //    set
        //    {
        //        selectedTipoRecurso = value;
        //        if (SelectedTipoRecurso != null)
        //        {
        //            LstRecursoResultado = new ObservableCollection<RECURSO_RESULTADO>(SelectedTipoRecurso.RECURSO_RESULTADO);
        //        }
        //        else
        //            LstRecursoResultado = new ObservableCollection<RECURSO_RESULTADO>();
        //        LstRecursoResultado.Insert(0, new RECURSO_RESULTADO() { ID_TIPO_RECURSO = -1, RESULTADO = string.Empty, DESCR = "SELECCIONE" });
        //        IdRecusrsoResultado = -1;
        //        RecursoResultado = string.Empty;
        //        SelectedRecursoResultado = LstRecursoResultado.Where(w => w.ID_TIPO_RECURSO == IdRecusrsoResultado && string.IsNullOrEmpty(w.RESULTADO)).FirstOrDefault();
        //    }
        //}

        //AMPAROS
        private bool tabAmparoDirectoListaSelected = false;
        public bool TabAmparoDirectoListaSelected
        {
            get { return tabAmparoDirectoListaSelected; }
            set { tabAmparoDirectoListaSelected = value; OnPropertyChanged("TabAmparoDirectoListaSelected"); }
        }

        private bool tabAmparoDirectoSelected = false;
        public bool TabAmparoDirectoSelected
        {
            get { return tabAmparoDirectoSelected; }
            set { tabAmparoDirectoSelected = value; OnPropertyChanged("TabAmparoDirectoSelected"); }
        }

        private bool tabAmparoIndirectoListaSelected = false;
        public bool TabAmparoIndirectoListaSelected
        {
            get { return tabAmparoIndirectoListaSelected; }
            set { tabAmparoIndirectoListaSelected = value; OnPropertyChanged("TabAmparoIndirectoListaSelected"); }
        }

        private bool tabAmparoIndirectoSelected = false;
        public bool TabAmparoIndirectoSelected
        {
            get { return tabAmparoIndirectoSelected; }
            set { tabAmparoIndirectoSelected = value; OnPropertyChanged("TabAmparoIndirectoSelected"); }
        }
        //INCIDENTES
        private bool tabAmparoIncidenteListaSelected = false;
        public bool TabAmparoIncidenteListaSelected
        {
            get { return tabAmparoIncidenteListaSelected; }
            set { tabAmparoIncidenteListaSelected = value; OnPropertyChanged("TabAmparoIncidenteListaSelected"); }
        }

        private bool tabAmparoIncidenteSelected = false;
        public bool TabAmparoIncidenteSelected
        {
            get { return tabAmparoIncidenteSelected; }
            set { tabAmparoIncidenteSelected = value; OnPropertyChanged("TabAmparoIncidenteSelected"); }
        }
        //JUZGADO
        private ObservableCollection<JUZGADO> lstTribunales;
        public ObservableCollection<JUZGADO> LstTribunales
        {
            get { return lstTribunales; }
            set { lstTribunales = value; OnPropertyChanged("LstTribunales"); }
        }

       

       

       

        //DELITO
        private ObservableCollection<RECURSO_DELITO> lstrecursoDelitos;
        public ObservableCollection<RECURSO_DELITO> LstRecursoDelitos
        {
            get { return lstrecursoDelitos; }
            set { lstrecursoDelitos = value; OnPropertyChanged("LstRecursoDelitos"); }
        }

        private RECURSO_DELITO selectedRecursoDelito;
        public RECURSO_DELITO SelectedRecursoDelito
        {
            get { return selectedRecursoDelito; }
            set { selectedRecursoDelito = value; OnPropertyChanged("SelectedRecursoDelito"); }
        }

        //LISTADO RECURSOS
     
        private bool recursoDelitoEmpty = false;
        public bool RecursoDelitoEmpty
        {
            get { return recursoDelitoEmpty; }
            set { recursoDelitoEmpty = value; OnPropertyChanged("RecursoDelitoEmpty"); }
        }

        //
        private bool habilitaMulta = false;
        public bool HabilitaMulta
        {
            get { return habilitaMulta; }
            set { habilitaMulta = value; OnPropertyChanged("HabilitaMulta"); }
        }

        private bool habilitaSentencia = false;
        public bool HabilitaSentencia
        {
            get { return habilitaSentencia; }
            set { habilitaSentencia = value; OnPropertyChanged("HabilitaSentencia"); }
        }

        private enum TipoRecurso
        {
            SentenciaPrimeraInstancia,
            DeterminacionSegundaInstancia,
            AmparoApelacionSegundaInstancia,
        };

        private short? idRecusrsoResultado;
        public short? IdRecusrsoResultado
        {
            get { return idRecusrsoResultado; }
            set { idRecusrsoResultado = value; OnPropertyChanged("IdRecusrsoResultado"); }
        }

        private string recursoResultado;
        public string RecursoResultado
        {
            get { return recursoResultado; }
            set { recursoResultado = value; OnPropertyChanged("RecursoResultado"); }
        }
    }
}

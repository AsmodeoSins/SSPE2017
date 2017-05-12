using SSP.Servidor;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        private string modalidadD;
        public string ModalidadD
        {
            get { return modalidadD; }
            set { modalidadD = value; OnPropertyChanged("ModalidadD"); }
        }

        private string textHelper;
        public string TextHelper
        {
            get { return textHelper; }
            set 
            { 
                textHelper = value;
                OnPropertyChanged("TextHelper");
            }
        }

        private string delitoD;
        public string DelitoD
        {
            get { return delitoD; }
            set { delitoD = value; OnPropertyChanged("DelitoD"); }
        }

        private string graveD;
        public string GraveD
        {
            get { return graveD; }
            set { graveD = value; OnPropertyChanged("GraveD"); }
        }

        private short tipoD;
        public short TipoD
        {
            get { return tipoD; }
            set { tipoD = value; OnPropertyChanged("TipoD"); }
        }

        private string cantidadD;
        public string CantidadD
        {
            get { return cantidadD; }
            set { cantidadD = value; OnPropertyChanged("CantidadD"); }
        }

        private string objetoD;
        public string ObjetoD
        {
            get { return objetoD; }
            set { objetoD = value; OnPropertyChanged("ObjetoD"); }
        }

        private MODALIDAD_DELITO selectedDelito;
        public MODALIDAD_DELITO SelectedDelito
        {
            get { return selectedDelito; }
            set { 
                selectedDelito = value; 
                OnPropertyChanged("SelectedDelito"); 
            }
        }


        private ObservableCollection<TIPO_DELITO> lstTiposDelitos;
        public ObservableCollection<TIPO_DELITO> LstTiposDelitos
        {
            get { return lstTiposDelitos; }
            set { lstTiposDelitos = value; OnPropertyChanged("LstTiposDelitos"); }
        }

        private TIPO_DELITO selectedTipoDelito;
        public TIPO_DELITO SelectedTipoDelito
        {
            get { return selectedTipoDelito; }
            set { selectedTipoDelito = value; OnPropertyChanged("SelectedTipoDelito"); }
        }

        private ObservableCollection<DELITO> lstDelitos;
        public ObservableCollection<DELITO> LstDelitos
        {
            get { return lstDelitos; }
            set { lstDelitos = value; OnPropertyChanged("LstDelitos"); }
        }

        private ObservableCollection<DELITO_TITULO> lstDelitoTitulo;
        public ObservableCollection<DELITO_TITULO> LstDelitoTitulo
        {
            get { return lstDelitoTitulo; }
            set { lstDelitoTitulo = value;
                OnPropertyChanged("LstDelitoTitulo"); }
        }
        
        //CAUSA PENAL DELITO
        private ObservableCollection<CAUSA_PENAL_DELITO> lstCausaPenalDelitos;
        public ObservableCollection<CAUSA_PENAL_DELITO> LstCausaPenalDelitos
        {
            get { return lstCausaPenalDelitos; }
            set { lstCausaPenalDelitos = value; OnPropertyChanged("LstCausaPenalDelitos"); }
        }


        private MODALIDAD_DELITO selectedDelitoArbol;
        public MODALIDAD_DELITO SelectedDelitoArbol
        {
            get { return selectedDelitoArbol; }
            set { selectedDelitoArbol = value; OnPropertyChanged("SelectedDelitoArbol"); }
        }

        private CAUSA_PENAL_DELITO selectedCausaPenalDelito;
        public CAUSA_PENAL_DELITO SelectedCausaPenalDelito
        {
            get { return selectedCausaPenalDelito; }
            set { selectedCausaPenalDelito = value;
            DTitulo = DGrupo = DDelito = DModalidad = DDetalle = DArticulo = DFuero = string.Empty;
            if (value != null)
            {
                //if (value.ID_CENTRO == 0 && value.ID_ANIO == 0 && value.ID_IMPUTADO == 0)
                //    EliminarVisible = true;
                //else
                //    EliminarVisible = false;
                //POPULA EL DETALLE DE DELITO
                DTitulo =  value.MODALIDAD_DELITO.DELITO.DELITO_GRUPO.DELITO_TITULO.DESCR;
                DGrupo = value.MODALIDAD_DELITO.DELITO.DELITO_GRUPO.DESCR;
                DDelito = value.MODALIDAD_DELITO.DELITO.DESCR;
                DModalidad = value.MODALIDAD_DELITO.DESCR;
                DDetalle = value.MODALIDAD_DELITO.DELITO.DETALLE;
                DArticulo = value.OBJETO;
                DFuero = value.ID_FUERO;

            }
            OnPropertyChanged("SelectedCausaPenalDelito"); }
        }

        private bool editarCPDelito;
        public bool EditarCPDelito
        {
            get { return editarCPDelito; }
            set { editarCPDelito = value; OnPropertyChanged("EditarCPDelito"); }
        }

        //VISIBLE
        private bool delitoVisible;
        public bool DelitoVisible
        {
            get { return delitoVisible; }
            set { delitoVisible = value; OnPropertyChanged("DelitoVisible"); }
        }

        //ARBOL
        private bool panelSelectedDelito;
        public bool PanelSelectedDelito
        {
            get { return panelSelectedDelito; }
            set { panelSelectedDelito = value; OnPropertyChanged("PanelSelectedDelito"); }
        }

        //EXPANDER
        private bool delitoExpander;
        public bool DelitoExpander
        {
            get { return delitoExpander; }
            set 
            {
                delitoExpander = value;
                //if (value)
                //    LimpiarValidacionesAgregarDelito();
                //else
                //    SetValidacionesAgregarDelito();

                OnPropertyChanged("DelitoExpander");
            }
        }

        private bool tabDelitoSelected;
        public bool TabDelitoSelected
        {
            get { return tabDelitoSelected; }
            set { tabDelitoSelected = value; OnPropertyChanged("TabDelitoSelected"); }
        }

        private bool tabDelitosSelected;
        public bool TabDelitosSelected
        {
            get { return tabDelitosSelected; }
            set { tabDelitosSelected = value; OnPropertyChanged("TabDelitosSelected"); }
        }

        private int bandDelito;
        public int BandDelito
        {
            get { return bandDelito; }
            set { bandDelito = value; OnPropertyChanged("BandDelito"); }
        }

        //DETALLE DELITO
        private string dTitulo;

        public string DTitulo
        {
            get { return dTitulo; }
            set { dTitulo = value; OnPropertyChanged("DTitulo"); }
        }
        private string dGrupo;

        public string DGrupo
        {
            get { return dGrupo; }
            set { dGrupo = value; OnPropertyChanged("DGrupo"); }
        }
        private string dDelito;

        public string DDelito
        {
            get { return dDelito; }
            set { dDelito = value; OnPropertyChanged("DDelito"); }
        }
        private string dModalidad;

        public string DModalidad
        {
            get { return dModalidad; }
            set { dModalidad = value; OnPropertyChanged("DModalidad"); }
        }
        private string dDetalle;

        public string DDetalle
        {
            get { return dDetalle; }
            set { dDetalle = value; OnPropertyChanged("DDetalle"); }
        }
        private string dArticulo;

        public string DArticulo
        {
            get { return dArticulo; }
            set { dArticulo = value; OnPropertyChanged("DArticulo"); }
        }
        private string dFuero;

        public string DFuero
        {
            get { return dFuero; }
            set { dFuero = value; OnPropertyChanged("DFuero"); }
        }

        //COPA DE DELITO EN POPUP
        private bool delitoCopia;
        public bool DelitoCopia
        {
            get { return delitoCopia; }
            set { delitoCopia = value; OnPropertyChanged("DelitoCopia"); }
        }

        private string textoDelitoCopia;
        public string TextoDelitoCopia
        {
            get { return textoDelitoCopia; }
            set { textoDelitoCopia = value; OnPropertyChanged("TextoDelitoCopia"); }
        }

        private bool delitoCopiaVisible;
        public bool DelitoCopiaVisible
        {
            get { return delitoCopiaVisible; }
            set { delitoCopiaVisible = value; OnPropertyChanged("DelitoCopiaVisible"); }
        }

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set { pInsertar = value; }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set { pEditar = value; }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
                if (value)
                    MenuBuscarEnabled = value;
            }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set
            {
                pImprimir = value;
                if (value)
                    MenuReporteEnabled = value;
            }
        }
        #endregion
        #region Menu

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        #endregion
   
    }
}

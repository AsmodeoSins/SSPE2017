

using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Windows;
namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        #region [COPARTICIPE]
        private string paternoCoparticipe;
        public string PaternoCoparticipe
        {
            get { return paternoCoparticipe; }
            set { paternoCoparticipe = value;
            if (!string.IsNullOrEmpty(value))
            {
                base.RemoveRule("MaternoCoparticipe");
                OnPropertyChanged("MaternoCoparticipe");
            }
            else
            {
                if (string.IsNullOrEmpty(MaternoCoparticipe))
                {
                    base.RemoveRule("PaternoCoparticipe");
                    base.AddRule(() => PaternoCoparticipe, () => !string.IsNullOrEmpty(PaternoCoparticipe), "APELLIDO PATERNO ES REQUERIDO!");

                    base.RemoveRule("MaternoCoparticipe");
                    base.AddRule(() => MaternoCoparticipe, () => !string.IsNullOrEmpty(MaternoCoparticipe), "APELLIDO MATERNO ES REQUERIDO!");
                    OnPropertyChanged("MaternoCoparticipe");
                }
            }
                OnPropertyChanged("PaternoCoparticipe"); }
        }

        private string maternoCoparticipe;
        public string MaternoCoparticipe
        {
            get { return maternoCoparticipe; }
            set { maternoCoparticipe = value;
            if (!string.IsNullOrEmpty(value))
            {
                base.RemoveRule("PaternoCoparticipe");
                OnPropertyChanged("PaternoCoparticipe");
            }
            else
            {
                if (string.IsNullOrEmpty(PaternoCoparticipe))
                {
                    base.RemoveRule("MaternoCoparticipe");
                    base.AddRule(() => MaternoCoparticipe, () => !string.IsNullOrEmpty(MaternoCoparticipe), "APELLIDO MATERNO ES REQUERIDO!");

                    base.RemoveRule("PaternoCoparticipe");
                    base.AddRule(() => PaternoCoparticipe, () => !string.IsNullOrEmpty(PaternoCoparticipe), "APELLIDO PATERNO ES REQUERIDO!");
                    OnPropertyChanged("PaternoCoparticipe");
                }
            }   
                OnPropertyChanged("MaternoCoparticipe"); }
        }

        private string nombreCoparticipe;
        public string NombreCoparticipe
        {
            get { return nombreCoparticipe; }
            set { nombreCoparticipe = value; OnPropertyChanged("NombreCoparticipe"); }
        }

        private ObservableCollection<COPARTICIPE> lstCoparticipe;
        public ObservableCollection<COPARTICIPE> LstCoparticipe
        {
            get { return lstCoparticipe; }
            set { lstCoparticipe = value; OnPropertyValidateChanged("LstCoparticipe"); }
        }

        private COPARTICIPE selectedCoparticipe;
        public COPARTICIPE SelectedCoparticipe
        {
            get { return selectedCoparticipe; }
            set
            {
                selectedCoparticipe = value;
                if (value != null)
                {
                    EliminarCoparticipeOpcion = value.CAUSA_PENAL == null ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                    EliminarCoparticipeOpcion = Visibility.Collapsed;

                OnPropertyChanged("SelectedCoparticipe");

                if (selectedCoparticipeIndex != -1 && LstCoparticipe.Count > 0)
                {
                    LstAlias = new ObservableCollection<COPARTICIPE_ALIAS>(LstCoparticipe[selectedCoparticipeIndex].COPARTICIPE_ALIAS);
                    LstApodo = new ObservableCollection<COPARTICIPE_APODO>(LstCoparticipe[selectedCoparticipeIndex].COPARTICIPE_APODO);
                }
            }
        }

        private int selectedCoparticipeIndex = 0;
        public int SelectedCoparticipeIndex
        {
            get { return selectedCoparticipeIndex; }
            set
            {
                selectedCoparticipeIndex = value;
                OnPropertyChanged("SelectedCoparticipeIndex");
            }
        }

        #endregion

        #region [ALIAS]
        //private string paternoAlias;
        //public string PaternoAlias
        //{
        //    get { return paternoAlias; }
        //    set { paternoAlias = value; OnPropertyChanged("PaternoAlias"); }
        //}

        //private string maternoAlias;
        //public string MaternoAlias
        //{
        //    get { return maternoAlias; }
        //    set { maternoAlias = value; OnPropertyChanged("MaternoAlias"); }
        //}
        private string paternoAlias;
        public string PaternoAlias
        {
            get { return paternoAlias; }
            set
            {
                paternoAlias = value;
                if (!string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("MaternoAlias");
                    OnPropertyChanged("MaternoAlias");
                }
                else
                {
                    if (string.IsNullOrEmpty(MaternoAlias))
                    {
                        base.RemoveRule("PaternoAlias");
                        base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ES REQUERIDO!");

                        base.RemoveRule("MaternoAlias");
                        base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ES REQUERIDO!");
                        OnPropertyChanged("MaternoAlias");
                    }
                }
                OnPropertyChanged("PaternoAlias");
            }
        }

        private string maternoAlias;
        public string MaternoAlias
        {
            get { return maternoAlias; }
            set
            {
                maternoAlias = value;
                if (!string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("PaternoAlias");
                    OnPropertyChanged("PaternoAlias");
                }
                else
                {
                    if (string.IsNullOrEmpty(PaternoAlias))
                    {
                        base.RemoveRule("MaternoAlias");
                        base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ES REQUERIDO!");

                        base.RemoveRule("PaternoAlias");
                        base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ES REQUERIDO!");
                        OnPropertyChanged("PaternoAlias");
                    }
                }
                OnPropertyChanged("MaternoAlias");
            }
        }


        private string nombreAlias;
        public string NombreAlias
        {
            get { return nombreAlias; }
            set { nombreAlias = value; OnPropertyChanged("NombreAlias"); }
        }

        private ObservableCollection<COPARTICIPE_ALIAS> lstAlias;
        public ObservableCollection<COPARTICIPE_ALIAS> LstAlias
        {
            get { return lstAlias; }
            set { lstAlias = value; OnPropertyValidateChanged("LstAlias"); }
        }

        private COPARTICIPE_ALIAS selectedAlias;
        public COPARTICIPE_ALIAS SelectedAlias
        {
            get { return selectedAlias; }
            set
            {
                selectedAlias = value;
                if (value != null)
                {
                    EliminarAliasOpcion = value.COPARTICIPE.CAUSA_PENAL == null ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                    EliminarAliasOpcion = Visibility.Collapsed;
                OnPropertyChanged("SelectedAlias");
            }
        }
        #endregion

        #region [APODO]
        private string apodo;
        public string Apodo
        {
            get { return apodo; }
            set { apodo = value; OnPropertyChanged("Apodo"); }
        }

        private ObservableCollection<COPARTICIPE_APODO> lstApodo;
        public ObservableCollection<COPARTICIPE_APODO> LstApodo
        {
            get { return lstApodo; }
            set { lstApodo = value; OnPropertyValidateChanged("LstApodo"); }
        }

        private COPARTICIPE_APODO selectedApodo;
        public COPARTICIPE_APODO SelectedApodo
        {
            get { return selectedApodo; }
            set
            {
                selectedApodo = value;
                if (value != null)
                {
                    EliminarApodoOpcion = value.COPARTICIPE != null ? value.COPARTICIPE.CAUSA_PENAL == null ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                }
                else
                    EliminarApodoOpcion = Visibility.Collapsed;
                OnPropertyChanged("SelectedApodo");
            }

        }
        #endregion

        //private ObservableCollection<COPARTICIPE> coparticipes;
        //public ObservableCollection<COPARTICIPE> Coparticipes
        //{
        //    get { return coparticipes; }
        //    set { coparticipes = value; OnPropertyChanged("Coparticipes"); }
        //}

        //private ObservableCollection<COPARTICIPE_ALIAS> coparticipesAlias;
        //public ObservableCollection<COPARTICIPE_ALIAS> CoparticipesAlias
        //{
        //    get { return coparticipesAlias; }
        //    set { coparticipesAlias = value; OnPropertyChanged("CoparticipesAlias"); }
        //}

        //private ObservableCollection<COPARTICIPE_APODO> coparticipeApodo;
        //public ObservableCollection<COPARTICIPE_APODO> CoparticipeApodo
        //{
        //    get { return coparticipeApodo; }
        //    set { coparticipeApodo = value; OnPropertyChanged("CoparticipeApodo"); }
        //}

        ////BANDERA EDITAR COPARTICIPES
        //private bool editarCoparticipeBandera;
        //public bool EditarCoparticipeBandera
        //{
        //    get { return editarCoparticipeBandera; }
        //    set { editarCoparticipeBandera = value; }
        //}

        //private bool editarCoparticipeAliasBandera;
        //public bool EditarCoparticipeAliasBandera
        //{
        //    get { return editarCoparticipeAliasBandera; }
        //    set { editarCoparticipeAliasBandera = value; }
        //}

        //private bool editarCoparticipeApodoBandera;
        //public bool EditarCoparticipeApodoBandera
        //{
        //    get { return editarCoparticipeApodoBandera; }
        //    set { editarCoparticipeApodoBandera = value; }
        //}
    }
}

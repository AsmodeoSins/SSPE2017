using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    class AltaMedicamentoViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public AltaMedicamentoViewModel()
        {
            CategoriaChecked = false;
            AnoChecked = false;
            NombreChecked = false;
            AnoEnabled = false;
            CategoriaEnabled = false;
            NombreEnabled = false;
        }
        #endregion

        #region variables

        #region Enableds
        private bool anoEnabled;
        public bool AnoEnabled
        {
            get { return anoEnabled; }
            set { anoEnabled = value; OnPropertyChanged("AnoEnabled"); }
        }
        private bool categoriaEnabled;
        public bool CategoriaEnabled
        {
            get { return categoriaEnabled; }
            set { categoriaEnabled = value; OnPropertyChanged("CategoriaEnabled"); }
        }
        private bool nombreEnabled;
        public bool NombreEnabled
        {
            get { return nombreEnabled; }
            set { nombreEnabled = value; OnPropertyChanged("NombreEnabled"); }
        }
        #endregion

        #region Checkeds
        private bool anoChecked;
        public bool AnoChecked
        {
            get { return anoChecked; }
            set 
            {
                if (anoChecked == false)
                    AnoEnabled = true;
                else
                    AnoEnabled = false;

                anoChecked = value; 
                OnPropertyChanged("AnoChecked");
            }
        }
        private bool categoriaChecked;
        public bool CategoriaChecked
        {
            get { return categoriaChecked; }
            set
            {
                if (categoriaChecked == false)
                    CategoriaEnabled = true;
                else
                    CategoriaEnabled = false;

                categoriaChecked = value;
                OnPropertyChanged("CategoriaChecked");
            }
        }
        private bool nombreChecked;
        public bool NombreChecked
        {
            get { return nombreChecked; }
            set
            {
                if (nombreChecked == false)
                    NombreEnabled = true;
                else
                    NombreEnabled = false;

                nombreChecked = value;
                OnPropertyChanged("NombreChecked");
            }
        }
        #endregion

        public string Name
        {
            get
            {
                return "alta_medicamento";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar_visible":
                    break;
            }
        }
        #endregion

        #region command
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
            }
        }
        #endregion
    }
}

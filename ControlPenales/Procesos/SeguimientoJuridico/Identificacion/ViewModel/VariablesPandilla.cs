using SSP.Servidor;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class JuridicoIdentificacionViewModel
    {
        #region Pandillas
        private ObservableCollection<PANDILLA> pandilla;
        public ObservableCollection<PANDILLA> Pandilla
        {
            get { return pandilla; }
            set { pandilla = value; 
                OnPropertyChanged("Pandilla"); }
        }
        
        private ObservableCollection<IMPUTADO_PANDILLA> imputadoPandilla;
        public ObservableCollection<IMPUTADO_PANDILLA> ImputadoPandilla
        {
            get { return imputadoPandilla; }
            set { imputadoPandilla = value; 
                OnPropertyValidateChanged("ImputadoPandilla"); }
        }
        
        private short selectedPandillaValue=-1;
        public short SelectedPandillaValue
        {
            get { return selectedPandillaValue; }
            set { selectedPandillaValue = value; OnPropertyChanged("SelectedPandillaValue"); }
        }
        
        private PANDILLA selectedPandillaItem;
        public PANDILLA SelectedPandillaItem
        {
            get { return selectedPandillaItem; }
            set { selectedPandillaItem = value; OnPropertyChanged("SelectedPandillaItem"); }
        }
        
        private string notaPandilla;
        public string NotaPandilla
        {
            get { return notaPandilla; }
            set { notaPandilla = value; OnPropertyChanged("NotaPandilla"); }
        }
        
        private IMPUTADO_PANDILLA selectedImputadoPandilla;
        public IMPUTADO_PANDILLA SelectedImputadoPandilla
        {
            get { return selectedImputadoPandilla; }
            set { selectedImputadoPandilla = value; OnPropertyChanged("SelectedImputadoPandilla"); }
        }
        #endregion
    }
}

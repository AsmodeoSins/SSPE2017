using SSP.Servidor;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        private ObservableCollection<PANDILLA> pandilla;
        public ObservableCollection<PANDILLA> Pandilla
        {
            get { return pandilla; }
            set { pandilla = value; OnPropertyChanged("Pandilla"); }
        }

        private ObservableCollection<IMPUTADO_PANDILLA> imputadoPandilla;
        public ObservableCollection<IMPUTADO_PANDILLA> ImputadoPandilla
        {
            get { return imputadoPandilla; }
            set { imputadoPandilla = value; OnPropertyChanged("ImputadoPandilla"); }
        }

        private short selectedPandillaValue;
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
    }
}

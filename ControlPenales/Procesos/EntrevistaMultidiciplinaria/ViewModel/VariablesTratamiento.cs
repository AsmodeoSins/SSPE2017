using System.Windows;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {

        #region Tratamiento Historial
        private Visibility visibleHistorial = Visibility.Collapsed;
        public Visibility VisibleHistorial
        {
            get { return visibleHistorial; }
            set { visibleHistorial = value; OnPropertyChanged("VisibleHistorial"); }
        }

        private bool IsHistorico = false;

        private Visibility visibleCargarHistorico = Visibility.Collapsed;
        public Visibility VisibleCargarHistorico
        {
            get { return visibleCargarHistorico; }
            set { visibleCargarHistorico = value; OnPropertyChanged("VisibleCargarHistorico"); }
        }
        #endregion

    }
}

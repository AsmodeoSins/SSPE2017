using System.Windows.Input;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class ConsultaUnificadaAdminViewModel
    {
        #region Click
        private ICommand _buscarClick;
        public ICommand BuscarClick
        {
            get { return _buscarClick ?? (_buscarClick = new RelayCommand(ClickEnter)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch)); }
        }
        #endregion

        #region Scanner
        public ICommand startScanning
        {
            get { return new DelegateCommand<PdfViewer>(Scan); }
        }

        public ICommand OpenDocument
        {
            get { return new DelegateCommand<PdfViewer>(AbrirDocumento); }
        }
        #endregion

        #region Load
        public ICommand Loading
        {
            get { return new DelegateCommand<ConsultaUnificadaAdminView>(PageLoad); }
        }
        #endregion
    }
}

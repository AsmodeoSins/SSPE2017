using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class ExcarcelacionViewModel
    {

        #region General
        public ICommand CmdExcarcelacionOnLoad
        {
            get { return new DelegateCommand<ExcarcelacionView>(ExcarcelacionOnLoad); }
        }

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(ModelChangedSwitch)); }
        }

        private ICommand _CargarMasResultados;
        public ICommand CargarMasResultados
        {
            get
            {
                return _CargarMasResultados ?? (_CargarMasResultados = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            if (SeguirCargando)
                                ListExpediente.InsertRange(await SegmentarResultadoBusqueda(Pagina));
                        }
                }));
            }
        }
        #endregion
        #region Datos Expediente
        private ICommand modelClick;
        public ICommand ModelClick
        {
            get { return modelClick ?? (modelClick = new RelayCommand(ClickBuscar)); }
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get { return onClick ?? (onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get { return buscarClick ?? (buscarClick = new RelayCommand(ClickBuscarInterno)); }
        }

        #endregion

        #region Digitalizacion
        public ICommand startScanning
        {
            get { return new DelegateCommand<PdfViewer>(Scan); }
        }
        public ICommand OpenDocument
        {
            get { return new DelegateCommand<PdfViewer>(AbrirDocumento); }
        }
        #endregion
    }
}

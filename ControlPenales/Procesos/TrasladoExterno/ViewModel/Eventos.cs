using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class TrasladoExternoViewModel
    {
        public ICommand TrasladoExternoOnLoading
        {
            get { return new DelegateCommand<TrasladoExternoView>(TrasladoExternoLoad); }
        }

        #region Datos Expediente
        private ICommand modelClick;
        public ICommand ModelClick
        {
            get { return modelClick ?? (modelClick = new RelayCommand(ClickBuscar)); }
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get { return onClick??(onClick=new RelayCommand(clickSwitch)); }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get { return buscarClick ?? (buscarClick = new RelayCommand(ClickBuscarInterno)); }
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

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(CambioModel)); }
        }

        #endregion
    }
}

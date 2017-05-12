using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ReporteNotasMedicasViewModel
    {
        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        public ICommand OnLoaded
        {
            get { return new DelegateCommand<ReporteNotasMedicasView>(WindowLoad); }
        }
        private System.Windows.Input.ICommand onClick;
        public System.Windows.Input.ICommand OnClick
        {
            get { return onClick ?? (onClick = new RelayCommand(clickSwitch)); }
        }        
        
        private System.Windows.Input.ICommand modelClick;
        public System.Windows.Input.ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(EnterExpediente));
            }
        }
        private System.Windows.Input.ICommand buscarClick;
        public System.Windows.Input.ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(EnterBuscar));
            }
        }
        private System.Windows.Input.ICommand _CargarMasResultados;
        public System.Windows.Input.ICommand CargarMasResultados
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
    }
}

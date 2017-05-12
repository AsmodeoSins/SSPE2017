using ControlPenales;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }


        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
            }
        }

        /*PRUEBA BUSCAR ARBOL DELITO*/
        private ICommand onClickBuscarDelito;
        public ICommand OnClickBuscarDelito
        {
            get
            {
                return onClickBuscarDelito ?? (onClickBuscarDelito = new RelayCommand(BuscarArbolDelito));
            }
        }

        private void BuscarArbolDelito(Object obj)
        {
            var arbol = (TreeView)obj;
            //foreach (TreeViewItem n in arbol.Items)
            //{
            //    var x = n;
            //}
            //Predicate<Object> predicate = FindPoints;
            //arbol.Items.Filter(predicate);
            var node = "HOMICIDIO";
            bool done = false;
            ItemCollection ic = arbol.Items;

            while (!done)
            {
                bool found = false;

                foreach (TreeViewItem tvi in ic)
                {
                    if (node.StartsWith(tvi.Tag.ToString()))
                    {
                        found = true;
                        tvi.IsExpanded = true;
                        ic = tvi.Items;
                        if (node == tvi.Tag.ToString()) done = true;
                        break;
                    }
                }

                done = (found == false && done == false);
            }

        }

        private static bool FindPoints(Object obj)
        {
            return false;
        }

        private ICommand mouseDoubleClickCommand;
        public ICommand MouseDoubleClickCommand
        {
            get
            {
                return mouseDoubleClickCommand ?? (mouseDoubleClickCommand = new RelayCommand(SeleccionaIngreso));
            }
        }

        private ICommand menuMouseClick;
        public ICommand MenuMouseClick
        {
            get
            {
                return menuMouseClick ?? (menuMouseClick = new RelayCommand(MenuClick));
            }
        }

        //private ICommand mouseDoubleClickArbolCommand;
        //public ICommand MouseDoubleClickArbolCommand
        //{
        //    get
        //    {
        //        return mouseDoubleClickArbolCommand ?? (mouseDoubleClickArbolCommand = new RelayCommand(SeleccionaDelito));
        //    }
        //}

        public ICommand MouseDoubleClickArbolCommand
        {
            get { return new DelegateCommand<TreeView>(SeleccionaDelito); }
        }

        private ICommand mouseDoubleClickArbolIngresoCommand;
        public ICommand MouseDoubleClickArbolIngresoCommand
        {
            get
            {
                return mouseDoubleClickArbolIngresoCommand ?? (mouseDoubleClickArbolIngresoCommand = new RelayCommand(SeleccionaIngresoArbol));
            }
        }

        private ICommand mouseDoubleClickArbolUbicacionCommand;
        public ICommand MouseDoubleClickArbolUbicacionCommand
        {
            get
            {
                return mouseDoubleClickArbolUbicacionCommand ?? (mouseDoubleClickArbolUbicacionCommand = new RelayCommand(SeleccionaUbicacionArbol));
            }
        }
        

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }
        
        private ICommand modelClick;
        public ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(ModelEnter));
            }
        }

        private ICommand onClickCoparticipe;
        public ICommand OnClickCoparticipe
        {
            get
            {
                return onClickCoparticipe ?? (onClickCoparticipe = new RelayCommand(ClickEnterCoparticipe));
            }
        }

        private ICommand onClickAlias;
        public ICommand OnClickAlias
        {
            get
            {
                return onClickAlias ?? (onClickAlias = new RelayCommand(ClickEnterCoparticipeAlias));
            }
        }

        private ICommand anClickApodo;
        public ICommand OnClickApodo
        {
            get
            {
                return anClickApodo ?? (anClickApodo = new RelayCommand(ClickEnterCoparticipeApodo));
            }
        }
    
        #region [LOADS]
        public ICommand CausaPenalLoading
        {
            get { return new DelegateCommand<CausaPenalView>(CausaPenalLoad); }
        }
        
        public ICommand IngresoCausaPenalLoading
        {
            get { return new DelegateCommand<CausasPenalesIngresoCausaPenalView>(IngresoCausaPenalLoad); }
        }

        public ICommand CausaPenalCoparticipeLoading
        {
            get { return new DelegateCommand<CausasPenalesCoparticipeView>(CausaPenalCoparticipeLoad); }
        }
        
        public ICommand IngresoLoading
        {
            get { return new DelegateCommand<CausasPenalesIngresoView>(IngresoLoad); }
        }

        public ICommand RecursoLoading
        {
            get { return new DelegateCommand<CausasPenalesRecursoView>(RecursoLoad); }
        }
        
        public ICommand CausaPenalSentenciaLoading
        {
            get { return new DelegateCommand<CausasPenalesSentenciaView>(SentenciaLoad); }
        }

        public ICommand RecursosListadoLoading
        {
            get { return new DelegateCommand<CausasPenalesRDIView>(RecursosListadoLoad); }
        }

        public ICommand CausaPenalDelitosLoading
        {
            get { return new DelegateCommand<CausasPenalesDelitosView>(CausaPenalDelitosLoad); }
        }

        public ICommand IngresosCausaPenalLoading
        {
            get { return new DelegateCommand<InsertarDelitoView>(IngresosCausaPenalLoad); }
        }
        #endregion

        #region [UNLOAD]
        public ICommand CausaPenalSentenciaUnloading
        {
            get { return new DelegateCommand<CausasPenalesSentenciaView>(SentenciaUnload); }
        }
        #endregion

        #region Grid
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

        //NUC
        #region INTERCONEXION
        public ICommand BuscarNUCInterconexion
        {
            get { return new DelegateCommand<string>(OnBuscarNUCInterconexion); }
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

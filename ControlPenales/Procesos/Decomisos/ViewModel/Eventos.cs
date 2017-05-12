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
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;
using SSP.Servidor;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel
    {
        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        #region Click
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
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

        private ICommand buscarDecomisoClick;
        public ICommand BuscarDecomisoClick
        {
            get
            {
                return buscarDecomisoClick ?? (buscarDecomisoClick = new RelayCommand(ClickDecomisoEnter));
            }
        }

        private ICommand _HeaderClick;
        public ICommand HeaderClick
        {
            get
            {
                return _HeaderClick ?? (_HeaderClick = new RelayCommand(HeaderSort));
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

        private ICommand _EnterClick;
        public ICommand EnterClick
        {
            get { return _EnterClick ?? (_EnterClick = new RelayCommand(EnterPersonas)); }
        }
        #endregion

        #region Load
        public ICommand RegistroDecomisoLoaded
        {
            get { return new DelegateCommand<RegistroDecomisoView>(RegistroDecomisoLoad); }
        }
        #endregion

        #region Cargar Personas
        private ICommand _CargarMasPersonas;
        public ICommand CargarMasPersonas
        {
            get
            {
                return _CargarMasPersonas ?? (_CargarMasPersonas = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            if (SeguirCargandoPersonas)
                            {
                                if (IndexTab == 3)
                                {
                                    ListPersonas.InsertRange(await SegmentarAbogadoBusqueda());                                    
                                }
                                else
                                    LstEmpleadoPop.InsertRange(await SegmentarPersonasBusqueda(Pagina));
                            }
                        }
                }));
            }
        }
        #endregion

        #region Oficial Cargo
        private ICommand buscarOficialClick;
        public ICommand BuscarOficialClick
        {
            get
            {
                return buscarOficialClick ?? (buscarOficialClick = new RelayCommand(ClickOficialEnter));
            }
        }
        #endregion

        #region BuscarInterno
        //private ICommand buscarInternoClick;
        //public ICommand BuscarInternoClick
        //{
        //    get
        //    {
        //        return buscarInternoClick ?? (buscarInternoClick = new RelayCommand(ClickEnterInterno));
        //    }
        //}
        //CargarMasResultadosImputado
        //private ICommand _CargarMasResultadosInterno;
        //public ICommand CargarMasResultadosInterno
        //{
        //    get
        //    {
        //        return _CargarMasResultadosInterno ?? (_CargarMasResultadosInterno = new RelayCommand(async (e) =>
        //        {
        //            if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
        //                if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
        //                {
        //                    if (SeguirCargandoIngreso)
        //                    {
        //                        LstIngreso.InsertRange(await SegmentarResultadoBusquedaIngreso(PaginaIngreso));
        //                    }
        //                }
        //        }));
        //    }
        //}
        #endregion

        #region BuscarVisita
        private ICommand buscarVisitaClick;
        public ICommand BuscarVisitaClick
        {
            get
            {
                return buscarVisitaClick ?? (buscarVisitaClick = new RelayCommand(ClickVisitaEnter));
            }
        }
        #endregion

        #region BuscarEmpleado
        private ICommand buscarEmpleadoClick;
        public ICommand BuscarEmpleadoClick
        {
            get
            {
                return buscarEmpleadoClick ?? (buscarEmpleadoClick = new RelayCommand(ClickEmpleadoEnter));
            }
        }
        #endregion

        #region BuscarProveedores
        private ICommand buscarExternoClick;
        public ICommand BuscarExternoClick
        {
            get
            {
                return buscarExternoClick ?? (buscarExternoClick = new RelayCommand(ClickProveedorEnter));
            }
        }
        #endregion

        #region Foto
        public ICommand CaptureImage
        {
            get { return new DelegateCommand<Image>(OnTakePicture); }
        }
        #endregion

        #region Scroll
        private ICommand _CargarMasDecomisos;
        public ICommand CargarMasDecomisos
        {
            get
            {
                return _CargarMasDecomisos ?? (_CargarMasDecomisos = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            if (SeguirCargandoDecomisos)
                            {
                                LstDecomisos.InsertRange(await SegmentarDecomisoBusqueda(PaginaDecomiso));
                            }
                        }
                }));
            }
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
                                ListExpediente.InsertRange(await SegmentarResultadoBusqueda(PaginaInterno));
                        }
                }));
            }
        }

        private ICommand _CargarMasVisitantes;
        public ICommand CargarMasVisitantes
        {
            get
            {
                return _CargarMasVisitantes ?? (_CargarMasVisitantes = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            LstVisitantePop.InsertRange(await SegmentarVisitanteBusqueda(PaginaVisitante));
                        }
                }));
            }
        }

        private ICommand _CargarMasExternos;
        public ICommand CargarMasExternos
        {
            get
            {
                return _CargarMasExternos ?? (_CargarMasExternos = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            LstExternoPop.InsertRange(await SegmentarExternoBusqueda(PaginaExterno));
                        }
                }));
            }
        }

        private ICommand _CargarMasEmpleado;
        public ICommand CargarMasEmpleado
        {
            get
            {
                return _CargarMasEmpleado ?? (_CargarMasEmpleado = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            LstOficialPop.InsertRange(await SegmentarEmpleadoBusqueda(PaginaEmpleado));
                        }
                }));
            }
        }
        #endregion
    }
}

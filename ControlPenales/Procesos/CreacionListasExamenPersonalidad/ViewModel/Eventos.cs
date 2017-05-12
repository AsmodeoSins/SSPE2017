using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CreacionListasExamenPViewModel
    {
        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        #region Busqueda
        private ICommand modelClick;
        public ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(ModelEnter));
            }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
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
        #endregion

        public ICommand CreacionListasPLoad
        {
            get { return new DelegateCommand<CreacionListasExamenPView>(CreacionListaPLoad); }
        }

        private ICommand _onClickSelect;
        public ICommand OnClickSelect
        {
            get
            {
                return _onClickSelect ?? (_onClickSelect = new RelayCommand(clickSwitch));
            }
        }

        public ICommand FichaJuridicaLoading
        {
            get { return new DelegateCommand<FichaIdentificacionJuridica>(FichaJuridicaLoad); }
        }

        public ICommand EstudiosTerminadosLoading
        {
            get { return new DelegateCommand<EstudiosTerminadosView>(EstudiosTerminadosLoad); }
        }


        #region Scroll
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
    }
}
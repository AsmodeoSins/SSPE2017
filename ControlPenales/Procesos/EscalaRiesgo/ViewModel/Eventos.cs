using ControlPenales.Clases;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ControlPenales
{
    partial class EscalaRiesgoViewModel
    {
        //Click
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand enterClick;
        public ICommand EnterClick
        {
            get
            {
                return enterClick ?? (enterClick = new RelayCommand(ClickEnter));
            }
        }

        private ICommand _checkRadio;
        public ICommand checkRadio
        {
            get
            {
                return _checkRadio ?? (_checkRadio = new RelayCommand(Radio));
            }
        }
       //Load
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<EscalaRiesgoView>(WindowLoad); }
        }

        //scroll
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
                                LstEscalaRiesgo.InsertRange(await SegmentarResultadoBusqueda(Pagina));
                            if (LstEscalaRiesgo.Count > 0)
                                EmptyBuscarEscalaRiesgo = Visibility.Collapsed;
                            else
                                EmptyBuscarEscalaRiesgo = Visibility.Visible;
                        }
                }));
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class VisitaDomiciliariaViewModel
    {
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

        private ICommand addFotografiaClick;
        public ICommand AddFotografiaClick
        {
            get
            {
                return addFotografiaClick ?? (addFotografiaClick = new RelayCommand(BuscarFotografia));
            }
        }

        private ICommand modelClick;
        public ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(ClickEnter));
            }
        }

        public ICommand VisitaDomiciliariaLoading
        {
            get { return new DelegateCommand<VisitaDomiciliariaView>(VisitaDomiciliariaLoad); }
        }

        public ICommand AnexosLoading
        {
            get { return new DelegateCommand<AnexosView>(AnexosLoad); }
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
                                //ListExpediente.InsertRange(await SegmentarResultadoBusqueda(Pagina));
                                lstLiberados.InsertRange(await SegmentarResultadoBusquedaLiberados(Pagina));
                        }
                }));
            }
        }

        #region [Huellas Digitales]
        private ICommand _Open442;
        public ICommand Open442
        {
            get { return _Open442 ?? (_Open442 = new RelayCommand(ShowIdentification)); }
        }

        public ICommand OnClickOk
        {
            get { return new DelegateCommand<Image>(OkClick); }
        }

        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }
        #endregion
    }
}

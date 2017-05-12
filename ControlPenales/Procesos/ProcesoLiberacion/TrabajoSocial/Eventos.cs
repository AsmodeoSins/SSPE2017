using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class TrabajoSocialViewModel
    {
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        //private ICommand buscarClick;
        //public ICommand BuscarClick
        //{
        //    get
        //    {
        //        return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
        //    }
        //}

        //private ICommand modelClick;
        //public ICommand ModelClick
        //{
        //    get
        //    {
        //        return modelClick ?? (modelClick = new RelayCommand(ClickEnter));
        //    }
        //}

        public ICommand TrabajoSocialLoading
        {
            get { return new DelegateCommand<TrabajoSocialView>(TrabajoSocialLoad); }
        }
        //public ICommand SituacionActualLoad
        //private ICommand _onClick;
        //public ICommand OnClick
        //{
        //    get
        //    {
        //        return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
        //    }
        //}
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
                return modelClick ?? (modelClick = new RelayCommand(ClickEnter));
            }
        }
        private ICommand eventKey;
        public ICommand EventKey

        {
            get
            {
                return eventKey ?? (eventKey = new RelayCommand(BuscarKey));
            }
        }
        

        //public ICommand TrabajoSocialLoading
        //{
        //    get { return new DelegateCommand<TrabajoSocialView>(TrabajoSocialLoad); }
        //}
        public ICommand EntrevistaInicialLoad
        {
            get { return new DelegateCommand<EntrevistaInicial>(EntrevistaInicialLoading); }
        }

        public ICommand EstudioSocioEconomicoLoad
        {
            get { return new DelegateCommand<EstudioSocioEconomicoTSocial>(EstudioSocioEconomicoLoading); }
        }


        public ICommand SituacionActualLoad
        {
            get { return new DelegateCommand<SituacionActualView>(SituacionActualLoading); }
        }

        public ICommand EstructuraDinamicaFamiliarLoad
        {
            get { return new DelegateCommand<EstructuraDinamicaFamiliarView>(EstrucruraDinamicaFamiliarLoading); }
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
        
        //public ICommand SituacionActualLoad  
        //{
        //    get { return new DelegateCommand<TrabajoSocialView>(SituacionActualWindow); }
        //}
        //public ICommand EstudioSocioEconomicoLoad
        //{
        //    get { return new DelegateCommand<TrabajoSocialView>(EstudioSocioEconomicoWindow); }
        //}
        //public ICommand EstructuraDinamicaFamiliarLoad
        //{
        //    get { return new DelegateCommand<TrabajoSocialView>(EstructuraDinamicaFamiliarWindow); }
        //}
        //public ICommand EntrevistaInicialLoad
        //{
        //    get { return new DelegateCommand<TrabajoSocialView>(EntrevistaInicialWindow); }
        //}



        //public ICommand EstudioSocioEconomicoLoad      
        //{
        //    get { return new DelegateCommand<TrabajoSocialView>(EstudioSocioEconomicoWindow); }
        //}
        //public ICommand EstructuraDinamicaFamiliarLoad    
        //{
        //    get { return new DelegateCommand<TrabajoSocialView>(EstructuraDinamicaFamiliarWindow); }
        //}
        //public ICommand EntrevistaInicialLoad   
        //{
        //    get { return new DelegateCommand<TrabajoSocialView>(EntrevistaInicialWindow); }
        //}



        //private ICommand buscarClick;
        //public ICommand BuscarClick
        //{
        //    get
        //    {
        //        return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
        //    }
        //}

        //private ICommand modelClick;
        //public ICommand ModelClick
        //{
        //    get
        //    {
        //        return modelClick ?? (modelClick = new RelayCommand(ClickEnter));
        //    }
        //}

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

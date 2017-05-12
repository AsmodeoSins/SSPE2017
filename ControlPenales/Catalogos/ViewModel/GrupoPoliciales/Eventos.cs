using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoGruposPolicialesViewModel
    {
        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get { return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch)); }
        }

        //private ICommand _CatalogoSimpleLoading;
        public ICommand CatalogoSimpleLoading
        {
            //get {
            //    return _TipoVisitaLoading ?? (_TipoVisitaLoading = new RelayCommand(TipoVisitaLoad));
            get { return new DelegateCommand<CatalogoSimpleView>(GrupoPolicialLoad); }
        }
    }
}

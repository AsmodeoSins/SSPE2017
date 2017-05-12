using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
//using MvvmFramework;

namespace ControlPenales
{
    partial class EstatusAdministrativoViewModel
    {
        #region [ALIAS]
        private ObservableCollection<ALIAS> listAlias = new ObservableCollection<ALIAS>();
        public ObservableCollection<ALIAS> ListAlias
        {
            get { return listAlias; }
            set { listAlias = value; OnPropertyChanged("ListAlias"); }
        }

        private ALIAS selectAlias;
        public ALIAS SelectAlias
        {
            get { return selectAlias; }
            set { selectAlias = value; OnPropertyChanged("SelectAlias"); }
        }

        private bool visibleAlias;
        public bool VisibleAlias
        {
            get { return visibleAlias; }
            set { visibleAlias = value; OnPropertyChanged("VisibleAlias"); }
        }

        //private string paternoAlias;
        //public string PaternoAlias
        //{
        //    get { return paternoAlias; }
        //    set { paternoAlias = value; OnPropertyChanged("PaternoAlias"); }
        //}

        //private string maternoAlias;
        //public string MaternoAlias
        //{
        //    get { return maternoAlias; }
        //    set { maternoAlias = value; OnPropertyChanged("MaternoAlias"); }
        //}
        private string paternoAlias;
        public string PaternoAlias
        {
            get { return paternoAlias; }
            set
            {
                paternoAlias = value;
                if (!string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("MaternoAlias");
                    OnPropertyChanged("MaternoAlias");
                }
                else
                {
                    if (string.IsNullOrEmpty(MaternoAlias))
                    {
                        base.RemoveRule("PaternoAlias");
                        base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ES REQUERIDO!");

                        base.RemoveRule("MaternoAlias");
                        base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ES REQUERIDO!");
                        OnPropertyChanged("MaternoAlias");
                    }
                }
                OnPropertyChanged("PaternoAlias");
            }
        }

        private string maternoAlias;
        public string MaternoAlias
        {
            get { return maternoAlias; }
            set
            {
                maternoAlias = value;
                if (!string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("PaternoAlias");
                    OnPropertyChanged("PaternoAlias");
                }
                else
                {
                    if (string.IsNullOrEmpty(PaternoAlias))
                    {
                        base.RemoveRule("MaternoAlias");
                        base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ES REQUERIDO!");

                        base.RemoveRule("PaternoAlias");
                        base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ES REQUERIDO!");
                        OnPropertyChanged("PaternoAlias");
                    }
                }
                OnPropertyChanged("MaternoAlias");
            }
        }

        private string nombreAlias;
        public string NombreAlias
        {
            get { return nombreAlias; }
            set { nombreAlias = value; OnPropertyChanged("NombreAlias"); }
        }

        private int indexAlias;
        public int IndexAlias
        {
            get { return indexAlias; }
            set { indexAlias = value; OnPropertyChanged("IndexAlias"); }
        }

        private string tituloAlias;
        public string TituloAlias
        {
            get { return tituloAlias; }
            set { tituloAlias = value; OnPropertyChanged("TituloAlias"); }
        }

        private int OpcionGuardarAlias = 0;
        #endregion

        #region [APODO]
        private ObservableCollection<APODO> listApodo = new ObservableCollection<APODO>();
        public ObservableCollection<APODO> ListApodo
        {
            get { return listApodo; }
            set { listApodo = value; OnPropertyChanged("ListApodo"); }
        }

        private APODO selectApodo;
        public APODO SelectApodo
        {
            get { return selectApodo; }
            set { selectApodo = value; OnPropertyChanged("SelectApodo"); }
        }

        private string apodo;
        public string Apodo
        {
            get { return apodo; }
            set { apodo = value; OnPropertyChanged("Apodo");}
        }

        private int indexApodo;
        public int IndexApodo
        {
            get { return indexApodo; }
            set { indexApodo = value; OnPropertyChanged("IndexApodo"); }
        }

        private string tituloApodo;
        public string TituloApodo
        {
            get { return tituloApodo; }
            set { tituloApodo = value; OnPropertyChanged("TituloApodo"); }
        }

        private int OpcionGuardarApodo = 0;
        #endregion

        #region [RELACION INTERNO]
        private ObservableCollection<RELACION_PERSONAL_INTERNO> listRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>();
        public ObservableCollection<RELACION_PERSONAL_INTERNO> ListRelacionPersonalInterno
        {
            get { return listRelacionPersonalInterno; }
            set { listRelacionPersonalInterno = value; OnPropertyValidateChanged("ListRelacionPersonalInterno"); }
        }

        private RELACION_PERSONAL_INTERNO selectRelacionPersonalInterno;
        public RELACION_PERSONAL_INTERNO SelectRelacionPersonalInterno
        {
            get { return selectRelacionPersonalInterno; }
            set { selectRelacionPersonalInterno = value; OnPropertyValidateChanged("SelectRelacionPersonalInterno"); }
        }

        private RangeEnabledObservableCollection<INGRESO> listBuscarRelacionInterno;
        public RangeEnabledObservableCollection<INGRESO> ListBuscarRelacionInterno
        {
            get { return listBuscarRelacionInterno; }
            set { listBuscarRelacionInterno = value; OnPropertyValidateChanged("ListBuscarRelacionInterno"); }
        }

        private bool RISeguirCargando = true;
        private int RIPagina = 0;

        private INGRESO selectBuscarRelacionInterno;
        public INGRESO SelectBuscarRelacionInterno
        {
            get { return selectBuscarRelacionInterno; }
            set { selectBuscarRelacionInterno = value; OnPropertyChanged("SelectBuscarRelacionInterno"); }
        }

        private string paternoBuscarRelacionInterno;
        public string PaternoBuscarRelacionInterno
        {
            get { return paternoBuscarRelacionInterno; }
            set { paternoBuscarRelacionInterno = value; OnPropertyChanged("PaternoBuscarRelacionInterno"); }
        }

        private string maternoBuscarRelacionInterno;
        public string MaternoBuscarRelacionInterno
        {
            get { return maternoBuscarRelacionInterno; }
            set { maternoBuscarRelacionInterno = value; OnPropertyChanged("MaternoBuscarRelacionInterno"); }
        }

        private string nombreBuscarRelacionInterno;
        public string NombreBuscarRelacionInterno
        {
            get { return nombreBuscarRelacionInterno; }
            set { nombreBuscarRelacionInterno = value; OnPropertyChanged("NombreBuscarRelacionInterno"); }
        }

        private bool emptyBuscarRelacionInternoVisible;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return emptyBuscarRelacionInternoVisible; }
            set { emptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }

        private bool visibleRelacionInterno;
        public bool VisibleRelacionInterno
        {
            get { return visibleRelacionInterno; }
            set { visibleRelacionInterno = value; OnPropertyChanged("VisibleRelacionInterno"); }
        }

        private string paternoRelacionInterno;
        public string PaternoRelacionInterno
        {
            get { return paternoRelacionInterno; }
            set { paternoRelacionInterno = value; OnPropertyChanged("PaternoRelacionInterno"); }
        }

        private string maternoRelacionInterno;
        public string MaternoRelacionInterno
        {
            get { return maternoRelacionInterno; }
            set { maternoRelacionInterno = value; OnPropertyChanged("MaternoRelacionInterno"); }
        }

        private string nombreRelacionInterno;
        public string NombreRelacionInterno
        {
            get { return nombreRelacionInterno; }
            set { nombreRelacionInterno = value; OnPropertyChanged("NombreRelacionInterno"); }
        }

        private string notaRelacionInterno;
        public string NotaRelacionInterno
        {
            get { return notaRelacionInterno; }
            set { notaRelacionInterno = value; OnPropertyChanged("NotaRelacionInterno"); }
        }

        private int indexRelacionInterno;
        public int IndexRelacionInterno
        {
            get { return indexRelacionInterno; }
            set { indexRelacionInterno = value; OnPropertyChanged("IndexAlias"); }
        }

        private string tituloRelacionInterno;
        public string TituloRelacionInterno
        {
            get { return tituloRelacionInterno; }
            set { tituloRelacionInterno = value; OnPropertyChanged("TituloRelacionInterno"); }
        }

        private int OpcionGuardarRelacionInterno = 0;
        #endregion
    }
}

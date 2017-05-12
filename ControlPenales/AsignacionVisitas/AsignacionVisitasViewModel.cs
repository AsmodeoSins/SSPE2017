using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    public class AsignacionVisitasViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region variables
        public string Name
        {
            get
            {
                return "mantenimiento_visita";
            }
        }
        private bool listaVisitanteVisible;

        public bool ListaVisitanteVisible
        {
            get { return listaVisitanteVisible; }
            set { listaVisitanteVisible = value; OnPropertyChanged("ListaVisitanteVisible"); }
        }
        private bool capturarVisitanteVisible;

        public bool CapturarVisitanteVisible
        {
            get { return capturarVisitanteVisible; }
            set { capturarVisitanteVisible = value; OnPropertyChanged("CapturarVisitanteVisible"); }
        }

        private bool programacionVisitaVisible;

        public bool ProgramacionVisitaVisible
        {
            get { return programacionVisitaVisible; }
            set { programacionVisitaVisible = value; OnPropertyChanged("ProgramacionVisitaVisible"); }
        }


        private List<Visitante> visitante;

        public List<Visitante> Visitante
        {
            get { return visitante; }
            set { visitante = value; }
        }
        #endregion

        #region constructor
        public AsignacionVisitasViewModel()
        {
            CapturarVisitanteVisible = false;
            ProgramacionVisitaVisible = false;
            ListaVisitanteVisible = true;

            Visitante = new List<Visitante>();
            Visitante.Add(new Visitante { Id_visitante = 1, Nombre_visitante = "Hugo Perez", Padron = false });
            Visitante.Add(new Visitante { Id_visitante = 2, Nombre_visitante = "Paco Perez", Padron = false });
            Visitante.Add(new Visitante { Id_visitante = 3, Nombre_visitante = "Luis Perez", Padron = false });
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "insertar_menucontext":
                    ListaVisitanteVisible = false;
                    CapturarVisitanteVisible = true;
                    break;
                case "borar_menucontext":
                    ListaVisitanteVisible = false;
                    CapturarVisitanteVisible = true;
                    break;
                case "insertar_menu":
                    CapturarVisitanteVisible = false;
                    ListaVisitanteVisible = true;
                    break;
                case "guardar_menu":
                    CapturarVisitanteVisible = false;
                    ListaVisitanteVisible = true;
                    break;
            }
            
        }
        private void treeClick(Object obj) {
            var tipo = obj.GetType();
            if (tipo.Name.Equals("Visitante"))
            {
                ProgramacionVisitaVisible = true;
                CapturarVisitanteVisible = true;
                ListaVisitanteVisible = false; 
            }
            else
            {
                ProgramacionVisitaVisible = false;
                CapturarVisitanteVisible = false;
                ListaVisitanteVisible = true; 
            }
        }
        #endregion

        #region command
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand _treeViewClick;
        public ICommand TreeViewClick
        {
            get
            {
                return _treeViewClick ?? (_treeViewClick = new RelayCommand(treeClick));
            }
        }
        #endregion

    }
}

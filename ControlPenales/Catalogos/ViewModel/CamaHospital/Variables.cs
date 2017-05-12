using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace ControlPenales
{
    public partial class CatalogoCamasHospitalViewModel
    {
        private CatalogoCamasHospitalView ventana;
        public CatalogoCamasHospitalView Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }

        private Estatus selectedEstatus;
        public Estatus SelectedEstatus
        {
            get { return selectedEstatus; }
            set { selectedEstatus = value; OnPropertyChanged("SelectedEstatus"); }
        }

        private EstatusControl listaEstatus = new EstatusControl();
        public EstatusControl ListaEstatus
        {
            get { return listaEstatus; }
            set { listaEstatus = value; OnPropertyChanged("ListaEstatus"); }
        }

        private Estatus selectedEstatusBusqueda;
        public Estatus SelectedEstatusBusqueda
        {
            get { return selectedEstatusBusqueda; }
            set { selectedEstatusBusqueda = value; OnPropertyChanged("SelectedEstatusBusqueda"); }
        }

        private EstatusControl listaEstatusBusqueda = new EstatusControl();
        public EstatusControl ListaEstatusBusqueda
        {
            get { return listaEstatusBusqueda; }
            set { listaEstatusBusqueda = value; OnPropertyChanged("ListaEstatusBusqueda"); }
        }

        private List<CAMA_HOSPITAL> listItems;
        public List<CAMA_HOSPITAL> ListItems
        {
            get { return listItems; }
            set { listItems = value; OnPropertyChanged("ListItems"); }
        }

        private CAMA_HOSPITAL selectedItem;
        public CAMA_HOSPITAL SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if (selectedItem == null)
                {
                    EditarMenuEnabled = false;
                }
                else
                {
                    if (selectedItem.ESTATUS == OCUPADA)
                    {
                        ListaEstatus.LISTA_ESTATUS.Add(new Estatus()
                        {
                            CLAVE = OCUPADA,
                            DESCRIPCION = "OCUPADA"
                        });
                    }
                    else
                    {
                        if (ListaEstatus.LISTA_ESTATUS.Any(a => a.CLAVE == OCUPADA))
                            ListaEstatus.LISTA_ESTATUS.Remove(ListaEstatus.LISTA_ESTATUS.Where(w => w.CLAVE == OCUPADA).FirstOrDefault());
                    }

                    EditarMenuEnabled = true;
                }
                OnPropertyChanged("SelectedItem");
            }
        }

        private bool emptyVisible;
        public bool EmptyVisible
        {
            get { return emptyVisible; }
            set { emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }

        private bool agregarVisible;
        public bool AgregarVisible
        {
            get { return agregarVisible; }
            set { agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }

        private bool guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return guardarMenuEnabled; }
            set { guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }

        private bool agregarMenuEnabled;
        public bool AgregarMenuEnabled
        {
            get { return agregarMenuEnabled; }
            set { agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }

        private bool buscarMenuEnabled;
        public bool BuscarMenuEnabled
        {
            get { return buscarMenuEnabled; }
            set { buscarMenuEnabled = value; OnPropertyChanged("BuscarMenuEnabled"); }
        }

        private bool eliminarMenuEnabled;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }

        private bool editarMenuEnabled;
        public bool EditarMenuEnabled
        {
            get { return editarMenuEnabled; }
            set { editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }

        private bool cancelarMenuEnabled;
        public bool CancelarMenuEnabled
        {
            get { return cancelarMenuEnabled; }
            set { cancelarMenuEnabled = value; OnPropertyChanged("CancelarMenuEnabled"); }
        }

        private bool menuLimpiarEnabled;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set { menuLimpiarEnabled = value; OnPropertyChanged("MenuLimpiarEnabled"); }
        }

        private bool ayudaMenuEnabled;
        public bool AyudaMenuEnabled
        {
            get { return ayudaMenuEnabled; }
            set { ayudaMenuEnabled = value; OnPropertyChanged("AyudaMenuEnabled"); }
        }

        private bool salirMenuEnabled;
        public bool SalirMenuEnabled
        {
            get { return salirMenuEnabled; }
            set { salirMenuEnabled = value; OnPropertyChanged("SalirMenuEnabled"); }
        }

        private string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; OnPropertyChanged("Descripcion"); }
        }

        const string ACTIVA = "S";
        const string INACTIVA = "N";
        const string OCUPADA = "O";
        const string SELECCIONAR = "SELECCIONAR";
        const string TODOS = "TODOS";
    }
}

using ControlPenales;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class BuscarViewModel : ValidationViewModelBase,IPageViewModel
    {

        private expediente _expediente;

        public string Name
        {
            get
            {
                return "buscar";
            }
        }
        public expediente Expediente
        {
            get { return _expediente; }
           // set { _expediente = value; DisplayedImage = @"/ControlPenales;component/Imagen/" + Expediente.imagen; ; OnPropertyChanged("Expediente"); }
        }

      
        private ObservableCollection<expediente> _listaExpedientes;

        public ObservableCollection<expediente> ListaExpedientes
        {
            get { return _listaExpedientes; }
            set { _listaExpedientes = value; OnPropertyChanged("ListaExpedientes"); }
        }

        private string _displayerImage;
        public string DisplayedImage
        {
            get { return _displayerImage; }
            set { _displayerImage = value; OnPropertyChanged("DisplayedImage"); }
        }

        public BuscarViewModel() { 
           
        }

        private void getAllExpedientes()
        {
            
        }

        void IPageViewModel.inicializa()
        { }

        //Eventos
        private DelegateCommand selectCommand;
        public ICommand SelectCommand
        {
            get
            {
                if (selectCommand == null)
                    selectCommand = new DelegateCommand(new Action(SelectExecuted),
                        new Func<bool>(SelectCanExecute));
                return selectCommand;
            }
        }

        public bool SelectCanExecute()
        {
           return true;
        }
        public void SelectExecuted()
        {
            
        }

    }
}

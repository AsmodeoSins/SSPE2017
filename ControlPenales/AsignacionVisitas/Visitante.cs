using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class Visitante : ValidationViewModelBase 
    {
        private int id_visitante;

        public int Id_visitante
        {
            get { return id_visitante; }
            set { id_visitante = value; OnPropertyChanged("Id_visitante"); }
        }
        private string nombre_visitante;

        public string Nombre_visitante
        {
            get { return nombre_visitante; }
            set { nombre_visitante = value; OnPropertyChanged("Nombre_visitante"); }
        }

        private bool padron;

        public bool Padron
        {
            get { return padron; }
            set { padron = value; OnPropertyChanged("Padron"); }
        }

        public Visitante() { }

    }

    public class PadronVisita : ValidationViewModelBase
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged("Id"); }
        }
        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; OnPropertyChanged("Nombre"); }
        }

        private bool padron;

        public bool Padron
        {
            get { return padron; }
            set { padron = value; OnPropertyChanged("Padron"); }
        }

        private List<Visitante> visitantes;

        public List<Visitante> Visitantes
        {
            get {return visitantes; 
            }
            set { visitantes = value; OnPropertyChanged("Visitantes"); }
        }

        public PadronVisita() {
            this.Id = 1;
            this.nombre = "Padron";
            this.Padron = false;
            this.Visitantes = new List<Visitante>();
            Visitantes.Add(new Visitante { Id_visitante = 1, Nombre_visitante = "Hugo Perez", Padron = false });
            Visitantes.Add(new Visitante { Id_visitante = 2, Nombre_visitante = "Paco Perez", Padron = false });
            Visitantes.Add(new Visitante { Id_visitante = 3, Nombre_visitante = "Luis Perez", Padron = false });
        }
    }

}

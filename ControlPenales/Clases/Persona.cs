using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class Persona : ValidationViewModelBase
    {
        public Persona() { }

        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; OnPropertyChanged("Nombre"); }
        }
        private string paterno;

        public string Paterno
        {
            get { return paterno; }
            set { paterno = value; OnPropertyChanged("Paterno"); }
        }
        private string materno;

        public string Materno
        {
            get { return materno; }
            set { materno = value; OnPropertyChanged("Materno"); }
        }
        private string estatura;
        public string Estatura
        {
            get { return estatura; }
            set { estatura = value; OnPropertyChanged("Estatura"); }
        }
        private string edad;
        public string Edad
        {
            get { return edad; }
            set { edad = value; OnPropertyChanged("Edad"); }
        }
        private string sexo;
        public string Sexo
        {
            get { return sexo; }
            set { sexo = value; OnPropertyChanged("Sexo"); }
        }
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
    }
}

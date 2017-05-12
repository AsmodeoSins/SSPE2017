using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class Usuario : ValidationViewModelBase
    {
        public Usuario() { }

        private string _Username;
        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                OnPropertyChanged("Username");
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        private DateTime? vigenciaPassword;
        public DateTime? VigenciaPassword
        {
            get { return vigenciaPassword; }
            set { vigenciaPassword = value; OnPropertyChanged("VigenciaPassword"); }
        }

        private string nombre;
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; OnPropertyChanged("Nombre"); }
        }

        private short centro;
        public short Centro
        {
            get { return centro; }
            set { centro = value; OnPropertyChanged("Centro"); }
        }

        private string centroNombre;
        public string CentroNombre
        {
            get { return centroNombre; }
            set { centroNombre = value; OnPropertyChanged("CentroNombre"); }
        }

        public int Id_Empleado { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.ViewModels;

namespace GESAL.Models
{
    public class Usuario : ValidationViewModelBase
    {
        public Usuario() { }

        private string username;

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
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

        private string nombreCompleto;

        public string NombreCompleto
        {
            get { return nombreCompleto; }
            set { nombreCompleto = value; OnPropertyChanged("NombreCompleto"); }
        }

        private string almacen_Grupo;

        public string Almacen_Grupo
        {
            get { return almacen_Grupo; }
            set { almacen_Grupo = value; OnPropertyChanged("Almacen_Grupo"); }
        }



        public string ROL { get; set; }
        public int? CENTRO { get; set; }

    }
}

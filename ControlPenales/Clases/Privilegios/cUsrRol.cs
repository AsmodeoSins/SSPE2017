using Microsoft.Office.Interop.Word;
using SSP.Servidor;
using System;
using System.Collections.Generic;


namespace ControlPenales
{
    public class cUsrRol : ViewModelBase
    {
        public cUsrRol() { }

        private bool seleccion = false;
        public bool Seleccion
        {
            get { return seleccion; }
            set { seleccion = value; OnPropertyChanged("Seleccion"); }
        }

        private bool bD = false;
        public bool BD
        {
            get { return bD; }
            set { bD = value; OnPropertyChanged("BD"); }
        }

        private USUARIO_ROL usuarioRol;
        public USUARIO_ROL UsuarioRol
        {
            get { return usuarioRol; }
            set { usuarioRol = value; OnPropertyChanged("UsuarioRol"); }
        }

        public bool PermisosEnabled { set; get; }
        //public bool InsertarEnabled { set; get; }
        //public bool EditarEnabled { set; get; }
        //public bool ConsultarEnabled { set; get; }
        //public bool EnabledImprimir { set; get; }
    }
}

using Microsoft.Office.Interop.Word;
using SSP.Servidor;
using System;
using System.Collections.Generic;


namespace ControlPenales
{
    public class cProcesoUsr : ViewModelBase
    {
        public cProcesoUsr() { }

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

        private string proceso;
        public string Proceso
        {
            get { return proceso; }
            set { proceso = value; OnPropertyChanged("Proceso"); }
        }

        private string rol;
        public string Rol
        {
            get { return rol; }
            set { rol = value; OnPropertyChanged("Rol"); }
        }

        private short centro;
        public short Centro
        {
            get { return centro; }
            set { centro = value; OnPropertyChanged("Centro"); }
        }
        private PROCESO_USUARIO procesoUsuario;
        public PROCESO_USUARIO ProcesoUsuario
        {
            get { return procesoUsuario; }
            set { procesoUsuario = value; OnPropertyChanged("ProcesoUsuario"); }
        }
    }
}

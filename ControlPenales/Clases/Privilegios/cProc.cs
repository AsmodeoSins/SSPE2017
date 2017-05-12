using Microsoft.Office.Interop.Word;
using SSP.Servidor;
using System;
using System.Collections.Generic;


namespace ControlPenales
{
    public class cProc : ViewModelBase
    {
        public cProc() { }

        private bool seleccion = false;
        public bool Seleccion
        {
            get { return seleccion; }
            set { seleccion = value; OnPropertyChanged("Seleccion"); }
        }

        private PROCESO proceso;
        public PROCESO Proceso
        {
            get { return proceso; }
            set { proceso = value; OnPropertyChanged("Proceso"); }
        }

        private short insertar = 0;
        public short Insertar
        {
            get { return insertar; }
            set { insertar = value; OnPropertyChanged("Insertar"); }
        }

        private short editar = 0;
        public short Editar
        {
            get { return editar; }
            set { editar = value; OnPropertyChanged("Editar"); }
        }

        private short consultar = 1;
        public short Consultar
        {
            get { return consultar; }
            set { consultar = value; OnPropertyChanged("Consultar"); }
        }

        private short imprimir = 0;
        public short Imprimir
        {
            get { return imprimir; }
            set { imprimir = value; OnPropertyChanged("Imprimir"); }
        }
    }
}

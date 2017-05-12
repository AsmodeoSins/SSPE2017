using Microsoft.Office.Interop.Word;
using SSP.Servidor;
using System;
using System.Collections.Generic;

namespace ControlPenales
{
    public class cPermisosRol : ViewModelBase
    {
        public cPermisosRol() { }

        private bool _seleccion = false;
        public bool Seleccion
        {
            get { return _seleccion; }
            set
            {
                _seleccion = value;
                if (_seleccion == false)
                {
                    InsertarHabilitado = false;
                    EditarHabilitado = false;
                    ConsultarHabilitado = false;
                    ImprimirHabilitado = false;
                }
                if (_seleccion)
                {
                    InsertarHabilitado = true;
                    EditarHabilitado = true;
                    ConsultarHabilitado = true;
                    ImprimirHabilitado = true;
                }
                OnPropertyChanged("Seleccion");
            }
        }

        private bool bD = false;
        public bool BD
        {
            get { return bD; }
            set { bD = value; OnPropertyChanged("BD"); }
        }

        private PROCESO_ROL procesoRol;
        public PROCESO_ROL ProcesoRol
        {
            get { return procesoRol; }
            set { procesoRol = value; OnPropertyChanged("ProcesoRol"); }
        }

        private bool _insertarHabilitado = true;
        public bool InsertarHabilitado
        {
            get { return _insertarHabilitado; }
            set { _insertarHabilitado = value; OnPropertyChanged("InsertarHabilitado"); }
        }

        private bool _editarHabilitado = true;
        public bool EditarHabilitado
        {
            get { return _editarHabilitado; }
            set { _editarHabilitado = value; OnPropertyChanged("EditarHabilitado"); }
        }

        private bool _consultarHabilitado = true;
        public bool ConsultarHabilitado
        {
            get { return _consultarHabilitado; }
            set { _consultarHabilitado = value; OnPropertyChanged("ConsultarHabilitado"); }
        }

        private bool _imprimirHabilitado = true;
        public bool ImprimirHabilitado
        {
            get { return _imprimirHabilitado; }
            set { _imprimirHabilitado = value; OnPropertyChanged("ImprimirHabilitado"); }
        }
    }
}

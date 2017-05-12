using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class SolicitudesAtencionPorEstatusViewModel
    {

        private ObservableCollection<Estatus> _LstEstatus;


        public ObservableCollection<Estatus> LstEstatus
        {
            get { return _LstEstatus; }
            set { _LstEstatus = value; OnPropertyChanged("LstEstatus"); }
        }
        private short? _SelectEstatus=-1;


        public short? SelectEstatus
        {
            get { return _SelectEstatus; }
            set { _SelectEstatus = value; OnPropertyChanged("SelectEstatus"); }
        }

        private Estatus _SelectedEstatus;   

        public Estatus SelectedEstatus
        {
            get { return _SelectedEstatus; }
            set { _SelectedEstatus = value; OnPropertyChanged("SelectedEstatus"); }
        }

        private DateTime? _FFechaInicio= Fechas.GetFechaDateServer;


        public DateTime? FFechaInicio
        {
            get { return _FFechaInicio; }
            set { _FFechaInicio = value; OnPropertyChanged("FFechaInicio"); }
        }

        private DateTime? _FFechaFin = Fechas.GetFechaDateServer;

        public DateTime? FFechaFin
        {
            get { return _FFechaFin; }
            set { _FFechaFin = value; OnPropertyChanged("FFechaFin"); }
        }

        public class Estatus
        {
            private short _Id_estatus;
            public short ID_ESTATUS
            {
                get { return _Id_estatus; }
                set { _Id_estatus = value; }
            }
            private string _DESCR;

            public string DESCR
            {
                get { return _DESCR; }
                set { _DESCR = value; }
            }


        }


    }
}

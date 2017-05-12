using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases
{
    class GrupoFamiliarPV : ValidationViewModelBase
    {
        //public bool Seleccionado { get; set; }
        //public string Nombre { get; set; }
        //public string Paterno { get; set; }
        //public string Materno { get; set; }
        //public short? IdReferencia { get; set; }
        //public TIPO_REFERENCIA TipoReferencia { get; set; }
        //public short? IdGrupo { get; set; }
        //public bool? ViveConEl { get; set; }
        //public PERSONA Persona { get; set; }
        //private short? idOcupacion;
        //public short? IdOcupacion
        //{
        //    get { return idOcupacion; }
        //    set { idOcupacion = value; OnPropertyChanged("IdOcupacion"); }
        //}
        //public OCUPACION Ocupacion { get; set; }
        //private short? idEstadoCivil;
        //public short? IdEstadoCivil
        //{
        //    get { return idEstadoCivil; }
        //    set { idEstadoCivil = value; OnPropertyChanged("IdEstadoCivil"); }
        //}
        //public ESTADO_CIVIL EstadoCivil { get; set; }

        public bool Seleccionado { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public short? IdReferencia { get; set; }
        public short? Edad { get; set; }
        public TIPO_REFERENCIA TipoReferencia { get; set; }
        public short? IdGrupo { get; set; }
        public bool? ViveConEl { get; set; }
        private short? idOcupacion;
        public short? IdOcupacion
        {
            get { return idOcupacion; }
            set { idOcupacion = value; OnPropertyChanged("IdOcupacion"); }
        }
        public OCUPACION Ocupacion { get; set; }
        private short? idEstadoCivil;
        public short? IdEstadoCivil
        {
            get { return idEstadoCivil; }
            set { idEstadoCivil = value; OnPropertyChanged("IdEstadoCivil"); }
        }
        public ESTADO_CIVIL EstadoCivil { get; set; }
        public short? ID_CENTRO { get; set; }
        public short? ID_ANIO { get; set; }
        public long? ID_IMPUTADO { get; set; }
        public short? ID_INGRESO { get; set; }
        public long? ID_VISITA { get; set; }
        public DateTime? FNacimiento { get; set; }
        public string Domicilio { get; set; }

    }
}

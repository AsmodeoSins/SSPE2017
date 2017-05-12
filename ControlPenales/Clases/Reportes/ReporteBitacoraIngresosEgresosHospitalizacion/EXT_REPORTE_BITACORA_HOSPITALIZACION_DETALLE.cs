using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class EXT_REPORTE_BITACORA_HOSPITALIZACION_DETALLE
    {
        public string NOCAMA { get; set; }
        private int id_imputado;
        public int ID_IMPUTADO
        {
            get { return id_imputado; }
            set { id_imputado = value; }
        }

        private short id_anio;
        public short ID_ANIO
        {
            get { return id_anio; }
            set { id_anio = value; }
        }
        public string FOLIO {
            get { return id_anio.ToString() + "/" + id_imputado.ToString(); }
        }

        private string nombre;
        public string NOMBRE
        {
            get { return nombre; }
            set { nombre = value;}
        }

        private string paterno;
        public string PATERNO
        {
            get { return paterno; }
            set { paterno = value;}
        }
        private string materno;
        public string MATERNO
        {
            get { return materno; }
            set { materno = value; }
        }

        public string NOMBRE_COMPLETO
        {
            get 
            {
                StringBuilder _strbuilder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(PATERNO))
                    _strbuilder.Append(PATERNO.Trim()).Append(" ");
                if(!string.IsNullOrWhiteSpace(MATERNO))
                    _strbuilder.Append(MATERNO.Trim()).Append(" ");
                _strbuilder.Append(NOMBRE.Trim());
                return _strbuilder.ToString();
            }
        }

        private DateTime? fechaNac;
        public DateTime? FechaNac 
        {
            get { return fechaNac; }
            set { fechaNac = value; }
        }
        public int? EDAD
        {
            get { return fechaNac.HasValue?new Fechas().CalculaEdad(fechaNac.Value):(int?)null; }
        }
        private decimal? hospitalizacion_tipo;
        public decimal? HOSPITALIZACION_TIPO
        {
            get { return hospitalizacion_tipo; }
            set { hospitalizacion_tipo = value; }
        }

        public DateTime? FECHA_INGRESO { get; set; }
        public string EXTERNA
        {
            get { return hospitalizacion_tipo.HasValue && hospitalizacion_tipo.Value == (decimal)enumHospitalizacion_Ingreso_Tipo.EXTERNA ? "X" : string.Empty; }
        }
        public string INTERNA
        {
            get { return hospitalizacion_tipo.HasValue && hospitalizacion_tipo.Value == (decimal)enumHospitalizacion_Ingreso_Tipo.URGENCIA ? "X" : string.Empty; }
        }
        public string ESPECIALIDAD
        {
            get { return hospitalizacion_tipo.HasValue && hospitalizacion_tipo.Value == (decimal)enumHospitalizacion_Ingreso_Tipo.ESPECIALIDAD ? "X" : string.Empty; }
        }

        private string nombre_medico_alta;
        public string NOMBRE_MEDICO_ALTA
        {
            get { return nombre_medico_alta; }
            set { nombre_medico_alta = value; }
        }

        private string paterno_medico_alta;
        public string PATERNO_MEDICO_ALTA
        {
            get { return paterno_medico_alta; }
            set { paterno_medico_alta = value; }
        }

        private string materno_medico_alta;
        public string MATERNO_MEDICO_ALTA
        {
            get { return materno_medico_alta; }
            set { materno_medico_alta = value; }
        }

        public string MEDICO_ALTA
        {
            

            get {
                StringBuilder _strbuilder=new StringBuilder();
                if (!string.IsNullOrWhiteSpace(paterno_medico_alta))
                    _strbuilder.Append(paterno_medico_alta.Trim()).Append(" ");
                if(!string.IsNullOrWhiteSpace(materno_medico_alta))
                    _strbuilder.Append(materno_medico_alta.Trim()).Append(" ");
                if(!string.IsNullOrWhiteSpace(nombre_medico_alta))
                    _strbuilder.Append(nombre_medico_alta.Trim()).Append(" ");
                return ( _strbuilder.ToString()); 
            }
        }

        private string nombre_medico_ingreso;
        public string NOMBRE_MEDICO_INGRESO
        {
            get { return nombre_medico_ingreso; }
            set { nombre_medico_ingreso = value; }
        }

        private string paterno_medico_ingreso;
        public string PATERNO_MEDICO_INGRESO
        {
            get { return paterno_medico_ingreso; }
            set { paterno_medico_ingreso = value; }
        }

        private string materno_medico_ingreso;
        public string MATERNO_MEDICO_INGRESO
        {
            get { return materno_medico_ingreso; }
            set { materno_medico_ingreso = value; }
        }

        public string MEDICO_INGRESO {
            get 
            {
                StringBuilder _strbuilder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(paterno_medico_ingreso))
                    _strbuilder.Append(paterno_medico_ingreso.Trim()).Append(" ");
                if (!string.IsNullOrWhiteSpace(materno_medico_ingreso))
                    _strbuilder.Append(materno_medico_ingreso.Trim()).Append(" ");
                if (!string.IsNullOrWhiteSpace(nombre_medico_ingreso))
                    _strbuilder.Append(nombre_medico_ingreso.Trim()).Append(" ");
                return _strbuilder.ToString();
            }
        }

        public DateTime? FECHA_ALTA { get; set; }
    }
}

using SSP.Servidor;
using System;
using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cReporteRelacionInternos
    {
        public cReporteRelacionInternos() { }

        public byte[] Foto { get; set; }
        public string Expediente { get; set; }
        public string Nombre { get; set; }
        public string Edad { get; set; }
        public string Clasificacion { get; set; }
        public string Fuero { get; set; }
        public string Ubicacion { get; set; }
        public string NCP { get; set; }
    }

    public class cReporteRelacionInterno
    {
        public int ID_CENTRO { get; set; }
        public int ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public DateTime? NACIMIENTO_FECHA { get; set; }
        public string CLASIFICACION_JURIDICA { get; set; }
        public string FUERO { get; set; }
        public string EDIFICIO { get; set; }
        public string SECTOR { get; set; }
        public string CELDA { get; set; }
        public string CAMA { get; set; }
        public byte?[] FOTO { get; set; }
        public int CENTRO_ACTUAL { get; set; }
    }


    public class cReporteRelacionInternosDetalle
    {
        public cReporteRelacionInternosDetalle() { }


        private INGRESO_BIOMETRICO _IB;
        public INGRESO_BIOMETRICO IB
        {
            get { return _IB; }
            set { _IB = value;
            if (value != null)
            {
                Foto = value.BIOMETRICO;
            }
            else
                Foto = new Imagenes().getImagenPerson();
            }
        }

        public DateTime Hoy { get; set; }
        public byte[] Foto { get; set; }
        public int Anio { get; set; }
        public int Imputado { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        private DateTime? _FecNacimiento;
        public DateTime? FecNacimiento
        {
            get { return _FecNacimiento; }
            set { _FecNacimiento = value;
                if (value != null)
                {
                    Edad = new Fechas().CalculaEdad(value, Hoy);
                }
            }
        }
        public int Edad { get; set; }
        public string Clasificacion { get; set; }
        private CAUSA_PENAL _CP;
        public CAUSA_PENAL CP
        {
            get { return _CP; }
            set {
                _CP = value;
                if (value != null)
                {
                    if (value.ID_ESTATUS_CP == 1)
                        Fuero = value.CP_FUERO == "C" ? "COMUN" : "FEDERAL";
                    else
                        Fuero = "DESCONOCIDO";
                }
                else
                    Fuero = "DESCONOCIDO";
            }
        }
        public string Fuero { get; set; }
        
        public string Ubicacion { get; set; }
        public string NCP { get; set; }

        #region Ubicacion
        public int IdCentro { get; set; }
        public int IdEdificio { get; set; }
        public int IdSector { get; set; }

        private EDIFICIO edificio;
        public EDIFICIO Edificio
        {
            get { return edificio; }
            set { edificio = value; InternoUbicacion(); 
            }
        }

        private SECTOR sector;
        public SECTOR Sector
        {
            get { return sector; }
            set { sector = value; InternoUbicacion(); 
            }
        }

        private CELDA celda;
        public CELDA Celda
        {
            get { return celda; }
            set { celda = value; InternoUbicacion(); 
            }
        }

        //private CAMA cama;
        //public CAMA Cama
        //{
        //    get { return cama; }
        //    set
        //    {
        //        cama = value; InternoUbicacion();
        //    }
        //}
        public CAMA Cama { get; set; }
        #endregion


        #region Ubicacion
        public string UEdificio { get; set; }
        public string USector { get; set; }
        public string UCelda { get; set; }
        public int? UCama { get; set; }
        #endregion


        private void InternoUbicacion() {
            if (Edificio != null && Sector != null && Celda != null && Cama != null)
            {
                Ubicacion = string.Format("{0}-{1}-{2}-{3}",
                                             Edificio.DESCR.Trim(),
                                             Sector.DESCR.Trim(),
                                             Celda.ID_CELDA.Trim(),
                                             Cama.ID_CAMA);
            }
        }
    }

    public class cReporteRelacionInternosMostrar {
        public int MostrarEdad { get; set; }
        public int MostrarFoto { get; set; }
    }

    public class cReporteEdificio {
        public int IdCentro { get; set; }
        public int IdEdificio { get; set; }
        public string Descr { get; set; }
    }

    public class cReporteSector
    {
        public int IdCentro { get; set; }
        public int IdEdificio { get; set; }
        public int IdSector { get; set; }
        public string Descr { get; set; }
    }
}

using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cReporteDecomiso
    {
        public cReporteDecomiso() { }

        public string Centro {set; get;}
        public string Folio {set; get;}
        public byte[] LogoBC {set;get;}
        public string MunicipioFecha{set; get;}
        public string DirigidoA { set; get; }
        public string DirigidoPuesto { set; get; }
        public string Remitente{set; get;}
        public string RemitentePuesto { set; get; }

        #region Atencion
        public string AtencionNombre { set; get; }
        public string AtencionPuesto { set; get; }
        #endregion

        #region Datos Decomiso
        public string DecomisoFecha { set; get; }
        public string DecomisoHora { set; get; }
        public string DecmosioTurno { set; get; }
        public string DecomisoGpoTactico { set; get; }
        public string DecomisoLugar { set; get; }
        public string DecomisoResumen { set; get; }
        #endregion
    }
}

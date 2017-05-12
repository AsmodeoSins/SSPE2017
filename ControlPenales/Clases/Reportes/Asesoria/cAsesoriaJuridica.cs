using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cAsesoriaJuridica
    {
        public cAsesoriaJuridica() { }
        public string LugarFecha { set; get; }
        public string Interno { set; get; }
        public string Familiar { set; get; }
        public string Parentesco { set; get; }
        public string Ubicacion { set; get; }
        public string DefensorSI { set; get; }
        public string DefensorNO { set; get; }
        public string DefensorPublico { set; get; }
        public string DefensorParticular { set; get; }
        public string Expediente { set; get; }
        public string CausasPenales { set; get; }
        public string Juzgados { set; get; }
        public string Delitos { set; get; }
        public string FueroFederal { set; get; }
        public string FueroComun { set; get; }
        public string FueroAmbos { set; get; }
        public string Indiciado { set; get; }
        public string Imputado { set; get; }
        public string Procesado { set; get; }
        public string Sentenciado { set; get; }
        public string Apelacion { set; get; }
        public string Amparo { set; get; }
        public string Sentencia { set; get; }
        public string SentenciaAnios { set; get; }
        public string SentenciaMeses { set; get; }
        public string SentenciaDias { set; get; }
        public string MultaApartir { set; get; }
        public string ReclusionAnios { set; get; }
        public string ReclusionMeses { set; get; }
        public string ReclusionDias { set; get; }
        public string EPSI { set; get; }
        public string EPNO { set; get; }
        public string EPDesconoce { set; get; }
        public string FechaElaboracion { set; get; }
        public string TSLibertad { set; get; }
        public string TSTraslado { set; get; }
        public string TSOtro { set; get; }
        public string AsesoriaObservaciones { set; get; }
        public string NombreAsesor { set; get; }
        public string NombreRecibeAsesoria { set; get; }
    }
}

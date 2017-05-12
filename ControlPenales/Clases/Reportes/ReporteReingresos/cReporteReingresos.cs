namespace ControlPenales
{
    public class cReporteReingresos
    {
        public string Fuero { get; set; }
        public string Delito { get; set; }
        public int Femenino { get; set; }
        public int Masculino { get; set; }
        public int GrandTotal { get; set; }
        public int Total { get; set; }
    }

    public class cReporteReingreso
    {
        public string FUERO { get; set; }
        public long ID_DELITO { get; set; }
        public string DELITO { get; set; }
        public int TOTAL_M { get; set; }
        public int TOTAL_F { get; set; }
        public int TOTAL { get; set; }
    }
}
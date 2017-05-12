namespace ControlPenales
{
    public class cIngresosHojaEnfermeriaReporte
    {
        public string Nombre { get; set; } 
        public string Hora { get; set; }
        public string Generico { get; set; }
        public string Generico2 { get; set; }
        public string Generico3 { get; set; }
        public decimal BalanceIngresos { get; set; }
        public decimal? Horas { get; set; }
        public decimal? IdTurn { get; set; }
    }
}
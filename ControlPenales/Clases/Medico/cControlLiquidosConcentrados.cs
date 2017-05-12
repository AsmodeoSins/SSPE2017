namespace ControlPenales
{
    public class cControlLiquidosConcentrados
    {
        //TURNO MATUTINO
        public decimal? EntradaTM { get; set; }
        public decimal? SalidasTM { get; set; }
        public decimal? BalanceTM { get; set; }
        public string EnfermeroTM { get; set; }

        //TURNO VESPERTINO
        public decimal? EntradaTV { get; set; }
        public decimal? SalidasTV { get; set; }
        public decimal? BalanceTV { get; set; }
        public string EnfermeroTV { get; set; }

        //TURNO NOCTURNO
        public decimal? EntradaTN { get; set; }
        public decimal? SalidasTN { get; set; }
        public decimal? BalanceTN { get; set; }
        public string EnfermeroTN { get; set; }

        //TOTAL
        public decimal? EntradasTTL { get; set; }
        public decimal? SalidasTTL { get; set; }
        public decimal? BalanceTTL { get; set; }
        public string EnfermeroTTL { get; set; }
    }
}
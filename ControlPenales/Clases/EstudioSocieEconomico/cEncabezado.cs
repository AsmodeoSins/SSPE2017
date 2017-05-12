namespace ControlPenales
{
    public class cEncabezado
    {
        public string TituloUno { get; set; }
        public string TituloDos { get; set; }
        public string NombreReporte { get; set; }

        public byte[] ImagenIzquierda { get; set; }
        public byte[] ImagenFondo { get; set; }
        public byte[] ImagenDerecha { get; set; }
        public string NoImputado { get; set; }

        public string PieUno { get; set; }
        public string PieDos { get; set; }
        public string PieTres { get; set; }
    }
}
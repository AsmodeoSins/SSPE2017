using SSP.Servidor;
using System;

namespace ControlPenales
{
    public class DietaMedica
    {
        public DIETA DIETA { get; set; }
        private bool _ELEGIDO;
        public int? ID_ATENCION_MEDICA;
        public short? ID_CENTRO_UBI;
        public DateTime? FECHA;
        public bool ELEGIDO
        {
            get { return _ELEGIDO; }
            set
            {
                _ELEGIDO = value;
            }
        }
    }
}

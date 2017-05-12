using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cPadronEstatalAbogados
    {
        public string Numero { set; get; }
        public string Nombre { set; get; }
        public string Tipo { set; get; }
        public string IFE { set; get; }
        public string RFC { set; get; }
        public string Cedula { set; get; }
        public string NIP { set; get; }
        public string Correo { set; get; }
        public string Celular { set; get; }
        public string FechaAlta { set; get; }
        public byte[] Foto { set; get; }
        public List<cPadronEstatalAbogadosColaborador> Colaboradores { set; get; }
        public string Colaborador2 { set; get; }
    }
}

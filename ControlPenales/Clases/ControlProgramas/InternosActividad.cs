using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases.ControlProgramas
{
    public class InternosActividad
    {
        public string Expediente { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Ubicacion { get; set; }
        public int Area { get; set; }
        public string Actividad { get; set; }
        public int IdImputado { get; set; }
        public short Anio { get; set; }
        public short Centro { get; set; }
        public int Id_Grupo { get; set; }
        public short IdIngreso { get; set; }
        public int IdConsec { get; set; }
        public string NIP { get; set; }
        public bool TrasladoInterno { get; set; }
        public bool ExcarcelacionInterno { get; set; }
        public bool VisitaLegalInterno { get; set; }
        public bool VisitaActuarioInterno { get; set; }
        public byte[] FotoInterno { get; set; }
        public bool Asistencia { get; set; }
        public bool Justificacion { get; set; }
        public string Responsable { get; set; }
        public bool Seleccionable { get; set; }
        public bool Seleccionar { get; set; }
    }
}

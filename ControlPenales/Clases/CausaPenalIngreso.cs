using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class CausaPenalIngreso
    {
        
        private CAUSA_PENAL causaPenal;
        public CAUSA_PENAL CausaPenal
        {
            get { return causaPenal; }
            set { causaPenal = value; }
        }

        private string cp;
        public string Cp
        {
            get { return cp; }
            set { cp = value; }
        }

        private DateTime? fecInicioCompurgacion;
        public DateTime? FecInicioCompurgacion
        {
            get { return fecInicioCompurgacion; }
            set { fecInicioCompurgacion = value; }
        }


        private DateTime? fecSentencia;
        public DateTime? FecSentencia
        {
            get { return fecSentencia; }
            set { fecSentencia = value; }
        }

        private DateTime? fecEjecutoria;
        public DateTime? FecEjecutoria
        {
            get { return fecEjecutoria; }
            set { fecEjecutoria = value; }
        }

        private string juzgadoFuero;
        public string JuzgadoFuero
        {
            get { return juzgadoFuero; }
            set { juzgadoFuero = value; }
        }
    }
}

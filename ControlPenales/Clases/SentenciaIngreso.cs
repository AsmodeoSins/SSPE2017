using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class SentenciaIngreso
    {
        
        private string causaPenal;
        public string CausaPenal
        {
            get { return causaPenal; }
            set { causaPenal = value; }
        }

        private short? sentenciaAnios;
        public short? SentenciaAnios
        {
            get { return sentenciaAnios; }
            set { sentenciaAnios = value; }
        }

        private short? sentenciaMeses;
        public short? SentenciaMeses
        {
            get { return sentenciaMeses; }
            set { sentenciaMeses = value; }
        }

        private short? sentenciaDias;
        public short? SentenciaDias
        {
            get { return sentenciaDias; }
            set { sentenciaDias = value; }
        }

        private string fuero;
        public string Fuero
        {
            get { return fuero; }
            set { fuero = value; }
        }

        private string instancia;
        public string Instancia
        {
            get { return instancia; }
            set { instancia = value; }
        }

        private string estatus;
        public string Estatus
        {
            get { return estatus; }
            set { estatus = value; }
        }
    }
}

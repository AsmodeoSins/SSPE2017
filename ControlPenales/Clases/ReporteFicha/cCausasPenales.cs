using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class cCausasPenales
    {
        private string causaPenal;
        public string CausaPenal
        {
            get { return causaPenal; }
            set { causaPenal = value; }
        }

        private string abreviatura;
        public string Abreviatura
        {
            get { return abreviatura; }
            set { abreviatura = value; }
        }

        private string juzgado;
        public string Juzgado
        {
            get { return juzgado; }
            set { juzgado = value; }
        }

        private string consignado;
        public string Consignado
        {
            get { return consignado; }
            set { consignado = value; }
        }
        private string delitos;
        public string Delitos
        {
            get { return delitos; }
            set { delitos = value; }
        }
    }
}

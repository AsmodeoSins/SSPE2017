using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class cTatuajesEMI
    {
        //ANTES DE ENTRAR
        public string AntisocialAE { set; get; }
        public string EroticoAE { set; get; }
        public string ReligiosolAE { set; get; }
        public string IdentificacionAE { set; get; }
        public string DecorativoAE { set; get; }
        public string SentimentalAE { set; get; }
        //INTRAMUROS
        public string AntisocialI { set; get; }
        public string EroticoI { set; get; }
        public string ReligiosolI { set; get; }
        public string IdentificacionI { set; get; }
        public string DecorativoI { set; get; }
        public string SentimentalI { set; get; }
        //TOTAL DE TATUAJES
        public string TotalTatuajes { set; get; }
        //DESCRIPCION
        public string DescripcionTatuajes { set; get; }
        //Visible
        public bool TatuajesVisible { set; get; }
    }
}

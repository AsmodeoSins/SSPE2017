using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cAmparoInd
    {
        private short? id;
        public short? Id
        {
            get { return id; }
            set { id = value; }
        }

        private string descr;
        public string Descr
        {
            get { return descr; }
            set { descr = value; }
        }

        private short? resultado;
        public short? Resultado
        {
            get { return resultado; }
            set { resultado = value; }
        }
    }
}

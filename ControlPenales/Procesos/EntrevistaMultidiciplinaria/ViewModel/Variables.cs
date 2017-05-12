using SSP.Servidor;
using SSP.Modelo;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;

namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {
        #region EMI
        private EMI selectedEMI;
        public EMI SelectedEMI
        {
            get { return selectedEMI; }
            set { selectedEMI = value; OnPropertyChanged("SelectedEMI"); }
        }
        #endregion

        #region Temporales
        private IMPUTADO tImputado;
        private INGRESO tIngreso;
        #endregion

        #region Mejoras Menu
        private short indexPadre;
        public short IndexPadre
        {
            get { return indexPadre; }
            set { indexPadre = value; OnPropertyChanged("IndexPadre"); }
        }

        private short _IndexHijo1;
        public short IndexHijo1
        {
            get { return _IndexHijo1; }
            set { _IndexHijo1 = value; OnPropertyChanged("IndexHijo1"); }
        }

        private short _IndexHijo2;
        public short IndexHijo2
        {
            get { return _IndexHijo2; }
            set { _IndexHijo2 = value; OnPropertyChanged("IndexHijo2"); }
        }

        private short _IndexHijo3;
        public short IndexHijo3
        {
            get { return _IndexHijo3; }
            set { _IndexHijo3 = value; OnPropertyChanged("IndexHijo3"); }
        }

        private short _IndexHijo4;
        public short IndexHijo4
        {
            get { return _IndexHijo4; }
            set { _IndexHijo4 = value; OnPropertyChanged("IndexHijo4"); }
        }
        #endregion

    }

   
}
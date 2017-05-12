using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class VisitasLegalesViewModel
    {

        #region Propiedades Ventana
        private VisitasLegalesView ventana;
        public VisitasLegalesView Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }
        #endregion

        #region Propiedades Visitas
        private List<InternoVisitaAbogado> listaInternoAbogado;
        public List<InternoVisitaAbogado> ListaInternoAbogado
        {
            get { return listaInternoAbogado; }
            set { listaInternoAbogado = value; OnPropertyChanged("ListaInternoAbogado"); }
        }


        private enum enumUbicacion
        {
            ESTANCIA = 0,
            EN_TRANSITO = 1,
            ACTIVIDAD = 2
        }

        private enum enumArea
        {
            LOCUTORIOS = 85
        }
        #endregion
    }
}

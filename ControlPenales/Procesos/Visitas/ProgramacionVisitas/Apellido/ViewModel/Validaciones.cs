using System;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ProgramacionVisitaApellidoViewModel
    {
        void SetValidaciones()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => SelectDiaVisita, () => SelectDiaVisita > 0, "DIA ES REQUERIDO!");
                base.AddRule(() => SelectLetraInicial, () => !string.IsNullOrEmpty(SelectLetraInicial), "LETRA INICIAL ES REQUERIDA!");
                base.AddRule(() => SelectLetraFinal, () => !string.IsNullOrEmpty(SelectLetraFinal) && !ChecarLetras(), "LETRA FINAL NO DEBE ESTAR VACIA Y DEBE SER SUCESOR A LA INICIAL!");
                base.AddRule(() => SelectTipoVisita, () => SelectTipoVisita > 0, "TIPO DE VISITA ES REQUERIDA!");
                base.AddRule(() => SelectArea, () => SelectArea > 0, "AREA ES REQUERIDA!");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo paso...", "Ocurrió un error al cargar validaciones.", ex);
            }
        }
        private bool ChecarLetras()
        {
            return ListLetras.IndexOf(SelectLetraFinal) < ListLetras.IndexOf(SelectLetraInicial);
        }
    }
}

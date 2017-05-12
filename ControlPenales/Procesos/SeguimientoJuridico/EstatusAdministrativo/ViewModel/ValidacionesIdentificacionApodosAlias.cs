using System.ComponentModel;

namespace ControlPenales
{
    partial class EstatusAdministrativoViewModel
    {
        void setValidacionesIdentificacionApodos()
        {
            base.ClearRules();
            base.AddRule(() => Apodo, () => !string.IsNullOrEmpty(Apodo), "APODO ES REQUERIDO!");
        }
        void setValidacionesIdentificacionAlias()
        {
            base.ClearRules();
            base.AddRule(() => NombreAlias, () => !string.IsNullOrEmpty(NombreAlias), "NOMBRE ALIAS ES REQUERIDO!");
            base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ALIAS ES REQUERIDO!");
            base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ALIAS ES REQUERIDO!");
        }
        void setValidacionesIdentificacionRelacionesPersonales()
        {
            base.ClearRules();
            //base.AddRule(() => Apodo, () => Apodo != "", "APODO ES REQUERIDO!");
        }
    }
}
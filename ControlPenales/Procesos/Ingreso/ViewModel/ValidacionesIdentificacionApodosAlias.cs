using System.ComponentModel;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        void setValidacionesIdentificacionApodos()
        {
            base.ClearRules();
            base.AddRule(() => Apodo, () => Apodo != "", "APODO ES REQUERIDO!");
        }
        void setValidacionesIdentificacionAlias()
        {
            base.ClearRules();
            base.AddRule(() => NombreAlias, () => NombreAlias != "", "NOMBRE ALIAS ES REQUERIDO!");
            base.AddRule(() => PaternoAlias, () => PaternoAlias != "", "APELLIDO PATERNO ALIAS ES REQUERIDO!");
            base.AddRule(() => MaternoAlias, () => MaternoAlias != "", "APELLIDO MATERNO ALIAS ES REQUERIDO!");
        }
        void setValidacionesIdentificacionRelacionesPersonales()
        {
            base.ClearRules();
            //base.AddRule(() => Apodo, () => Apodo != "", "APODO ES REQUERIDO!");
        }
    }
}
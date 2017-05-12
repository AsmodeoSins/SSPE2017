using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoMedicamentosViewModel
    {
        private void setValidacionRule()
        {
            ClearRules();
            AddRule(() => SelectedProductoCategoriaValue, () => SelectedProductoCategoriaValue!=-1,"CATEGORIA DE PRODUCTO ES REQUERIDO!");
            RaisePropertyChanged("SelectedProductoCategoriaValue");
            AddRule(() => SelectedProductoUMValue, () => SelectedProductoUMValue!=-1,"UNIDAD DE MEDIDA DEL PRODUCTO ES REQUERIDA!");
            RaisePropertyChanged("SelectedProductoUMValue");
            AddRule(() => SelectedFormaFarmaceuticaValue, () => SelectedFormaFarmaceuticaValue != -1, "FORMA FARMACEUTICA ES REQUERIDA!");
            RaisePropertyChanged("SelectedFormaFarmaceuticaValue");
            AddRule(()=> TextMedicamento,()=>!string.IsNullOrWhiteSpace(TextMedicamento),"MEDICAMENTO ES REQUERIDO!");
            RaisePropertyChanged("TextMedicamento");
            AddRule(()=>TextDescripcion,()=>!string.IsNullOrWhiteSpace(TextDescripcion),"DESCRIPCION ES REQUERIDO!");
            RaisePropertyChanged("TextDescripcion");
        }
    }
}

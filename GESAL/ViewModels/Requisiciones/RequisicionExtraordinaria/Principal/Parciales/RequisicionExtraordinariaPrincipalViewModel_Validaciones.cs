using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GESAL.ViewModels
{
    public partial class RequisicionExtraordinariaPrincipalViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        public void setValidationRulesRequisicionExtraordinaria()
        {
            base.ClearRules();
            base.AddRule(() => ProductosRequisicion, () => IsProductosRequisicionValido, "CAPTURA POR LO MENOS UN PRODUCTO A REQUERIR!");
            base.AddRule(() => IsCantidadesRequeridasValidas, () => IsCantidadesRequeridasValidas,"TODAS LAS CANTIDADES REQUERIDAS TIENEN QUE SER MAYOR A 0");
            base.AddRule(()=>SelectedAlmacen_Tipo_CatValue,()=>SelectedAlmacen_Tipo_CatValue!=-1,"TIPO DE ALMACEN ES OBLIGATORIO!");
            base.AddRule(() => SelectedAlmacenPrincipalValue, () => SelectedAlmacenPrincipalValue != -1, "ALMACEN ES OBLIGATORIO!");
            base.AddRule(() => SelectedCentroValue, () => SelectedCentroValue != -1, "CENTRO ES OBLIGATORIO!");
            base.AddRule(() => SelectedMunicipioValue, () => SelectedMunicipioValue != -1, "MUNICIPIO ES OBLIGATORIO!");
            RaisePropertyChanged("ProductosRequisicion");
            RaisePropertyChanged("IsCantidadesRequeridasValidas");
            RaisePropertyChanged("SelectedAlmacen_Tipo_CatValue");
            RaisePropertyChanged("SelectedAlmacenPrincipalValue");
            RaisePropertyChanged("SelectedCentroValue");
            RaisePropertyChanged("SelectedMunicipioValue");
        }

        public void setValidationRuleBuscarRequisicion()
        {
            base.ClearRules();
            base.AddRule(() => SelectedRequisicionExtraordinariaPop_Up, () => SelectedRequisicionExtraordinariaPop_Up != null, "SELECCIONA UNA REQUISICION");
            RaisePropertyChanged("SelectedRequisicionExtraordinariaPop_Up");
        }
    }
}

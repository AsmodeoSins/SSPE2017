using System;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {
        void setValidacionesHPS()
        {
            try
            {
                base.ClearRules();
                #region [Homosexualidad, Pandillas, Sexualidad]

                #region [Conducta Parasocial]

                base.AddRule(() => VivioCalleOrfanato, () => !string.IsNullOrEmpty(VivioCalleOrfanato), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("VivioCalleOrfanato");
                base.AddRule(() => PertenecePandilla, () => !string.IsNullOrEmpty(PertenecePandilla), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("PertenecePandilla");
                if (!string.IsNullOrEmpty(PertenecePandilla))
                {
                    if (PertenecePandilla.Equals("S"))
                        base.AddRule(() => NombrePandilla, () => !string.IsNullOrEmpty(NombrePandilla), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                 
                    if (PertenecePandilla.Equals("N"))
                        NombrePandilla = string.Empty;
                }
                OnPropertyChanged("NombrePandilla");
                #region [Conducta]

                base.AddRule(() => Homosexual, () => !string.IsNullOrEmpty(Homosexual), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("Homosexual");
                if (Homosexual != null)
                    if (Homosexual.Equals("S"))
                    {
                        base.AddRule(() => HomosexualEdadIncial, () => HomosexualEdadIncial <= EdadInterno, "LA EDAD ES MAYOR A LA ACTUAL");
                        //base.AddRule(() => HomosexualEdadIncial, () => (HomosexualEdadIncial != null ? HomosexualEdadIncial : 0) < EdadInterno, "LA EDAD DEL INTERNO ES MAYOR A LA ACTUAL");
                        base.AddRule(() => HomosexualRol, () => !string.IsNullOrEmpty(HomosexualRol), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                        base.AddRule(() => Id_Homo, () => Id_Homo != null ? Id_Homo != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                    }

                OnPropertyChanged("HomosexualEdadIncial");
                OnPropertyChanged("Id_Homo");
                OnPropertyChanged("HomosexualRol");

                base.AddRule(() => PertenecioPandillaExterior, () => !string.IsNullOrEmpty(PertenecioPandillaExterior), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("PertenecioPandillaExterior");

                if (PertenecioPandillaExterior != null)
                    if (PertenecioPandillaExterior.Equals("S"))
                    {
                        base.AddRule(() => PandillaExteriorEdadInicial, () => PandillaExteriorEdadInicial <= EdadInterno, "LA EDAD ES MAYOR A LA ACTUAL");
                        //base.AddRule(() => PandillaExteriorEdadInicial, () => (PandillaExteriorEdadInicial != null ? PandillaExteriorEdadInicial : 0) < EdadInterno, "LA EDAD DEL INTERNO ES MAYOR A LA ACTUAL");
                        base.AddRule(() => PandillaExteriorMotivo, () => !string.IsNullOrEmpty(PandillaExteriorMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                    }
                OnPropertyChanged("PandillaExteriorEdadInicial");
                OnPropertyChanged("PandillaExteriorMotivo");

                base.AddRule(() => Vagancia, () => !string.IsNullOrEmpty(Vagancia), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("Vagancia");

                if (Vagancia != null)
                    if (Vagancia.Equals("S"))
                    {
                        base.AddRule(() => VaganciaEdadIncial, () => VaganciaEdadIncial <= EdadInterno, "LA EDAD ES MAYOR A LA ACTUAL");
                        //base.AddRule(() => VaganciaEdadIncial, () => (VaganciaEdadIncial != null ? VaganciaEdadIncial : 0) < EdadInterno, "LA EDAD DEL INTERNO ES MAYOR A LA ACTUAL");
                        base.AddRule(() => VaganciaMotivos, () => !string.IsNullOrEmpty(VaganciaMotivos), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                    }
                OnPropertyChanged("VaganciaEdadIncial");
                OnPropertyChanged("VaganciaMotivos");

                base.AddRule(() => Cicatrices, () => !string.IsNullOrEmpty(Cicatrices), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("Cicatrices");

                if (Cicatrices != null)
                    if (Cicatrices.Equals("S"))
                    {
                        base.AddRule(() => CicatricesEdadIncial, () => CicatricesEdadIncial <= EdadInterno, "LA EDAD ES MAYOR A LA ACTUAL");
                        //base.AddRule(() => CicatricesEdadIncial, () => (CicatricesEdadIncial != null ? CicatricesEdadIncial : 0) < EdadInterno, "LA EDAD DEL INTERNO ES MAYOR A LA ACTUAL");
                        base.AddRule(() => CicatricesMotivo, () => !string.IsNullOrEmpty(CicatricesMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                        //base.AddRule(() => CicatricesRina, () => !string.IsNullOrEmpty(CicatricesRina.ToString()), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                    }
                OnPropertyChanged("CicatricesEdadIncial");
                OnPropertyChanged("CicatricesMotivo");

                base.AddRule(() => DesercionEscolar, () => !string.IsNullOrEmpty(DesercionEscolar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("DesercionEscolar");

                if (DesercionEscolar != null)
                    if (DesercionEscolar.Equals("S"))
                        base.AddRule(() => DesercionMotivo, () => !string.IsNullOrEmpty(DesercionMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("DesercionMotivo");


                base.AddRule(() => ReprobacionEscolar, () => !string.IsNullOrEmpty(ReprobacionEscolar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("ReprobacionEscolar");

                if (ReprobacionEscolar != null)
                    if (ReprobacionEscolar.Equals("S"))
                    {//Comentada validacionen base a requerimiento
                        base.AddRule(() => ReprobacionEscolarMotivo, () => !string.IsNullOrEmpty(ReprobacionEscolarMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                        //base.AddRule(() => ReprobacionGrado, () => ReprobacionGrado.HasValue ? ReprobacionGrado.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                    }
                OnPropertyChanged("ReprobacionEscolarMotivo");
                //OnPropertyChanged("ReprobacionGrado");

                base.AddRule(() => ExplusionEscolar, () => !string.IsNullOrEmpty(ExplusionEscolar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("ExplusionEscolar");

                if (ExplusionEscolar != null)
                    if (ExplusionEscolar.Equals("S"))
                    {//Comentada validacionen base a requerimiento
                        base.AddRule(() => ExplusionEscolarMotivo, () => !string.IsNullOrEmpty(ExplusionEscolarMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                        //base.AddRule(() => ExpulsionGrado, () => ExpulsionGrado.HasValue ? ExpulsionGrado.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                    }
                OnPropertyChanged("ExplusionEscolarMotivo");
                //OnPropertyChanged("ExpulsionGrado");

                #endregion

                #region [Pagaba por Servicio Sexual]
                // base.AddRule(() => ConHombres, () => !string.IsNullOrEmpty(ConHombres.ToString()), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                // base.AddRule(() => ConMujeres, () => !string.IsNullOrEmpty(ConMujeres.ToString()), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                #endregion
                #endregion
                #endregion
                //OnPropertyChanged();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en HPS", ex);
            }
        }
    }
}

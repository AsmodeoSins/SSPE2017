
using System;
namespace ControlPenales
{
    partial class JuridicoIdentificacionViewModel
    {
        void setValidacionesIdentificacionMediaFiliacion()
        {
            try
            {
                base.ClearRules();

                #region SeniasGenerales
                base.AddRule(() => SelectComplexion, () => SelectComplexion >= 0, "COMPLEXION ES REQUERIDA!");
                base.AddRule(() => SelectColorPiel, () => SelectColorPiel >= 0, "COLOR DE PIEL ES REQUERIDO!");
                base.AddRule(() => SelectCara, () => SelectCara >= 0, "TIPO DE CARA ES REQUERIDO!");
                #endregion

                #region Sangre
                base.AddRule(() => SelectTipoSangre, () => SelectTipoSangre >= 0, "TIPO DE SANGRE ES REQUERIDO!");
                base.AddRule(() => SelectFactorSangre, () => SelectFactorSangre >= 0, "FACTOR DE SANGRE ES REQUERIDO!");
                #endregion

                #region Cabello
                base.AddRule(() => SelectCantidadCabello, () => SelectCantidadCabello >= 0, "CANTIDAD DE CABELLO ES REQUERIDO!");
                base.AddRule(() => SelectColorCabello, () => SelectColorCabello >= 0, "COLOR DE CABELLO ES REQUERIDO!");
                base.AddRule(() => SelectFormaCabello, () => SelectFormaCabello >= 0, "FORMA DE CABELLO ES REQUERIDO!");
                base.AddRule(() => SelectCalvicieCabello, () => SelectCalvicieCabello >= 0, "CALVICIE ES REQUERIDO!");
                base.AddRule(() => SelectImplantacionCabello, () => SelectImplantacionCabello >= 0, "IMPLANTACION DE CABELLO ES REQUERIDO!");
                #endregion

                #region Frente
                base.AddRule(() => SelectAlturaFrente, () => SelectAlturaFrente >= 0, "ALTURA DE LA FRENTE ES REQUERIDA!");
                base.AddRule(() => SelectInclinacionFrente, () => SelectInclinacionFrente >= 0, "INCLINACION DE LA FRENTE ES REQUERIDA!");
                base.AddRule(() => SelectAnchoFrente, () => SelectAnchoFrente >= 0, "ANCHO DE LA FRENTE ES REQUERIDO!");
                #endregion

                #region Cejas
                base.AddRule(() => SelectDireccionCeja, () => SelectDireccionCeja >= 0, "DIRECCION DE LAS CEJAS ES REQUERIDO!");
                base.AddRule(() => SelectImplantacionCeja, () => SelectImplantacionCeja >= 0, "IMPLANTACION EN LAS CEJAS ES REQUERIDO!");
                base.AddRule(() => SelectFormaCeja, () => SelectFormaCeja >= 0, "FORMA DE LAS CEJAS ES REQUERIDO!");
                base.AddRule(() => SelectTamanioCeja, () => SelectTamanioCeja >= 0, "TAMAÑO DE LAS CEJAS ES REQUERIDO!");
                #endregion

                #region Ojos
                base.AddRule(() => SelectColorOjos, () => SelectColorOjos >= 0, "COLOR DE OJOS ES REQUERIDO!");
                base.AddRule(() => SelectFormaOjos, () => SelectFormaOjos >= 0, "FORMA DE OJOS ES REQUERIDO!");
                base.AddRule(() => SelectTamanioOjos, () => SelectTamanioOjos >= 0, "TAMAÑO DE OJOS ES REQUERIDO!");
                #endregion

                #region Nariz
                base.AddRule(() => SelectRaizNariz, () => SelectRaizNariz >= 0, "RAIZ DE LA NARIZ ES REQUERIDA!");
                base.AddRule(() => SelectDorsoNariz, () => SelectDorsoNariz >= 0, "DORSO DE LA NARIZ ES REQUERIDO!");
                base.AddRule(() => SelectAnchoNariz, () => SelectAnchoNariz >= 0, "ANCHO DE LA NARIZ ES REQUERIDO!");
                base.AddRule(() => SelectBaseNariz, () => SelectBaseNariz >= 0, "BASE DE LA NARIZ ES REQUERIDA!");
                base.AddRule(() => SelectAlturaNariz, () => SelectAlturaNariz >= 0, "ALTURA DE LA NARIZ ES REQUERIDO!");
                #endregion

                #region Labios
                base.AddRule(() => SelectEspesorLabio, () => SelectEspesorLabio >= 0, "ESPESOR DE LOS LABIOS ES REQUERIDO!");
                base.AddRule(() => SelectAlturaLabio, () => SelectAlturaLabio >= 0, "ALTURA DE LOS LABIOS ES REQUERIDA!");
                base.AddRule(() => SelectProminenciaLabio, () => SelectProminenciaLabio >= 0, "PROMINENCIA DE LOS LABIOS ES REQUERIDO!");
                #endregion

                #region Boca
                base.AddRule(() => SelectTamanioBoca, () => SelectTamanioBoca >= 0, "TAMAÑO DE LA BOCA ES REQUERIDO!");
                base.AddRule(() => SelectComisuraBoca, () => SelectComisuraBoca >= 0, "COMISURA DE LA BOCA ES REQUERIDA!");
                #endregion

                #region Labios
                base.AddRule(() => SelectFormaMenton, () => SelectFormaMenton >= 0, "FORMA DEL MENTON ES REQUERIDA!");
                base.AddRule(() => SelectTipoMenton, () => SelectTipoMenton >= 0, "TIPO DEL MENTON ES REQUERIDO!");
                base.AddRule(() => SelectInclinacionMenton, () => SelectInclinacionMenton >= 0, "INCLINACION DEL MENTON ES REQUERIDO!");
                #endregion

                #region OrejaDerecha
                base.AddRule(() => SelectFormaOrejaDerecha, () => SelectFormaOrejaDerecha >= 0, "FORMA DE OREJA DERECHA ES REQUERIDO!");
                #region Helix
                base.AddRule(() => SelectHelixOriginalOrejaDerecha, () => SelectHelixOriginalOrejaDerecha >= 0, "HELIX ORIGINAL DERECHO ES REQUERIDO!");
                base.AddRule(() => SelectHelixSuperiorOrejaDerecha, () => SelectHelixSuperiorOrejaDerecha >= 0, "HELIX SUPERIOR DERECHO ES REQUERIDO!");
                base.AddRule(() => SelectHelixPosteriorOrejaDerecha, () => SelectHelixPosteriorOrejaDerecha >= 0, "HELIX POSTERIOR DERECHO ES REQUERIDO!");
                base.AddRule(() => SelectHelixAdherenciaOrejaDerecha, () => SelectHelixAdherenciaOrejaDerecha >= 0, "ADHERENCIA DEL HELIX DERECHO ES REQUERIDO!");
                #endregion

                #region Lobulo
                base.AddRule(() => SelectLobuloContornoOrejaDerecha, () => SelectLobuloContornoOrejaDerecha >= 0, "CONTORNO DEL LOBULO DERECHO ES REQUERIDO!");
                base.AddRule(() => SelectLobuloAdherenciaOrejaDerecha, () => SelectLobuloAdherenciaOrejaDerecha >= 0, "ADHERENCIA DEL LOBULO DERECHO ES REQUERIDO!");
                base.AddRule(() => SelectLobuloParticularidadOrejaDerecha, () => SelectLobuloParticularidadOrejaDerecha >= 0, "PARTICULARIDAD DEL LOBULO DERECHO ES REQUERIDO!");
                base.AddRule(() => SelectLobuloDimensionOrejaDerecha, () => SelectLobuloDimensionOrejaDerecha >= 0, "DIMENSION DEL LOBULO DERECHO ES REQUERIDO!");
                #endregion

                #endregion

                #region OrejaIzquierda
                //base.AddRule(() => SelectFormaOrejaIzquierda, () => SelectFormaOrejaIzquierda > 0, "FORMA DE OREJA IZQUIERDA ES REQUERIDA!");

                //#region Helix
                //base.AddRule(() => SelectHelixOriginalOrejaIzquierda, () => SelectHelixOriginalOrejaIzquierda > 0, "HELIX ORIGINAL IZQUIERDO ES REQUERIDO!");
                //base.AddRule(() => SelectHelixSuperiorOrejaIzquierda, () => SelectHelixSuperiorOrejaIzquierda > 0, "HELIX SUPERIOR IZQUIERDO ES REQUERIDO!");
                //base.AddRule(() => SelectHelixPosteriorOrejaIzquierda, () => SelectHelixPosteriorOrejaIzquierda > 0, "HELIX POSTERIOR IZQUIERDO ES REQUERIDO!");
                //base.AddRule(() => SelectHelixAdherenciaOrejaIzquierda, () => SelectHelixAdherenciaOrejaIzquierda > 0, "ADHERENCIA DEL HELIX IZQUIERDO ES REQUERIDO!");
                //#endregion

                //#region Lobulo
                //base.AddRule(() => SelectLobuloContornoOrejaIzquierda, () => SelectLobuloContornoOrejaIzquierda > 0, "CONTORNO DEL LOBULO IZQUIERDO ES REQUERIDO!");
                //base.AddRule(() => SelectLobuloAdherenciaOrejaIzquierda, () => SelectLobuloAdherenciaOrejaIzquierda > 0, "ADHERENCIA DEL LOBULO IZQUIERDO ES REQUERIDO!");
                //base.AddRule(() => SelectLobuloParticularidadOrejaIzquierda, () => SelectLobuloParticularidadOrejaIzquierda > 0, "PARTICULARIDAD DEL LOBULO IZQUIERDO ES REQUERIDO!");
                //base.AddRule(() => SelectLobuloDimensionOrejaIzquierda, () => SelectLobuloDimensionOrejaIzquierda > 0, "DIMENSION DEL LOBULO IZQUIERDO ES REQUERIDO!");
                //#endregion
                #endregion


                OnPropertyChanged("SelectComplexion");
                OnPropertyChanged("SelectColorPiel");
                OnPropertyChanged("SelectCara");

                OnPropertyChanged("SelectTipoSangre");
                OnPropertyChanged("SelectFactorSangre");

                OnPropertyChanged("SelectCantidadCabello");
                OnPropertyChanged("SelectColorCabello");
                OnPropertyChanged("SelectFormaCabello");
                OnPropertyChanged("SelectCalvicieCabello");
                OnPropertyChanged("SelectImplantacionCabello");

                OnPropertyChanged("SelectAlturaFrente");
                OnPropertyChanged("SelectInclinacionFrente");
                OnPropertyChanged("SelectAnchoFrente");

                OnPropertyChanged("SelectDireccionCeja");
                OnPropertyChanged("SelectImplantacionCeja");
                OnPropertyChanged("SelectFormaCeja");
                OnPropertyChanged("SelectTamanioCeja");

                OnPropertyChanged("SelectColorOjos");
                OnPropertyChanged("SelectFormaOjos");
                OnPropertyChanged("SelectTamanioOjos");

                OnPropertyChanged("SelectRaizNariz");
                OnPropertyChanged("SelectDorsoNariz");
                OnPropertyChanged("SelectAnchoNariz");
                OnPropertyChanged("SelectBaseNariz");
                OnPropertyChanged("SelectAlturaNariz");

                OnPropertyChanged("SelectEspesorLabio");
                OnPropertyChanged("SelectAlturaLabio");
                OnPropertyChanged("SelectProminenciaLabio");

                OnPropertyChanged("SelectTamanioBoca");
                OnPropertyChanged("SelectComisuraBoca");

                OnPropertyChanged("SelectFormaMenton");
                OnPropertyChanged("SelectTipoMenton");
                OnPropertyChanged("SelectInclinacionMenton");

                OnPropertyChanged("SelectFormaOrejaDerecha");

                OnPropertyChanged("SelectHelixOriginalOrejaDerecha");
                OnPropertyChanged("SelectHelixSuperiorOrejaDerecha");
                OnPropertyChanged("SelectHelixPosteriorOrejaDerecha");
                OnPropertyChanged("SelectHelixAdherenciaOrejaDerecha");

                OnPropertyChanged("SelectLobuloContornoOrejaDerecha");
                OnPropertyChanged("SelectLobuloAdherenciaOrejaDerecha");
                OnPropertyChanged("SelectLobuloParticularidadOrejaDerecha");
                OnPropertyChanged("SelectLobuloDimensionOrejaDerecha");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear validaciones.", ex);
            }
        }
    }
}
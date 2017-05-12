using System;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Collections.Generic;
using ControlPenales.Clases;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        #region [COPARTICIPE]
        private void AgregarCoparticipe()
        {
            try
            {
                if (SelectedCoparticipe == null)//AGREGAR
                {
                    LstCoparticipe.Add(new COPARTICIPE()
                    {
                        PATERNO = PaternoCoparticipe,
                        MATERNO = MaternoCoparticipe,
                        NOMBRE = NombreCoparticipe,
                    });
                }
                else//EDITAR
                {
                    SelectedCoparticipe.PATERNO = PaternoCoparticipe;
                    SelectedCoparticipe.MATERNO = MaternoCoparticipe;
                    SelectedCoparticipe.NOMBRE = NombreCoparticipe;
                    LstCoparticipe = new ObservableCollection<COPARTICIPE>(LstCoparticipe);
                }
                LimpiarCoparticipe();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar coparticipe", ex);
            }
        }

        private void EliminarCoparticipe()
        {
            try
            {
                if (SelectedCoparticipe != null)
                {
                    LstCoparticipe.Remove(SelectedCoparticipe);
                    LimpiarCoparticipe();
                    LstAlias = new ObservableCollection<COPARTICIPE_ALIAS>();
                    LstApodo = new ObservableCollection<COPARTICIPE_APODO>();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar coparticipe", ex);
            }
        }

        private void PopulateCoparticipe()
        {
            try
            {
                if (SelectedCoparticipe != null)
                {
                    PaternoCoparticipe = SelectedCoparticipe.PATERNO;
                    MaternoCoparticipe = SelectedCoparticipe.MATERNO;
                    NombreCoparticipe = SelectedCoparticipe.NOMBRE;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer coparticipe", ex);
            }
        }

        private void LimpiarCoparticipe()
        {
            PaternoCoparticipe = MaternoCoparticipe = NombreCoparticipe = string.Empty;
            SelectedCoparticipe = null;
        }
        #endregion

        #region [COPARTICIPE ALIAS]
        private void AgregarAlias()
        {
            try
            {
                if (LstAlias == null)
                    LstAlias = new ObservableCollection<COPARTICIPE_ALIAS>();
                if (SelectedCoparticipe != null)
                {
                    if (SelectedAlias == null)//AGREGAR
                    {
                        LstCoparticipe[SelectedCoparticipeIndex].COPARTICIPE_ALIAS.Add(new COPARTICIPE_ALIAS()
                        {
                            PATERNO = PaternoAlias,
                            MATERNO = MaternoAlias,
                            NOMBRE = NombreAlias,
                            ID_COPARTICIPE = SelectedCoparticipe.ID_COPARTICIPE,
                            COPARTICIPE = SelectedCoparticipe
                        });
                    }
                    else//EDITAR
                    {
                        SelectedAlias.PATERNO = PaternoAlias;
                        SelectedAlias.MATERNO = MaternoAlias;
                        SelectedAlias.NOMBRE = NombreAlias;

                    }
                    LstAlias = new ObservableCollection<COPARTICIPE_ALIAS>(LstCoparticipe[SelectedCoparticipeIndex].COPARTICIPE_ALIAS);
                    LimpiarCoparticipeAlias();
                }
                else
                     new Dialogos().ConfirmacionDialogo("NOTIFICACIÓN!", "Debe seleccionar un coparticipe!");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar alias", ex);
            }
        }

        private void EliminarAlias()
        {
            try
            {
                if (SelectedAlias != null)
                {
                    LstCoparticipe[SelectedCoparticipeIndex].COPARTICIPE_ALIAS.Remove(SelectedAlias);
                    LstAlias = new ObservableCollection<COPARTICIPE_ALIAS>(LstCoparticipe[SelectedCoparticipeIndex].COPARTICIPE_ALIAS);
                    //LstAlias.Remove(SelectedAlias);
                    LimpiarCoparticipeAlias();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar alias", ex);
            }
        }

        private void PopulateAlias()
        {
            try
            {
                if (SelectedAlias != null)
                {
                    PaternoAlias = SelectedAlias.PATERNO;
                    MaternoAlias = SelectedAlias.MATERNO;
                    NombreAlias = SelectedAlias.NOMBRE;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer alias", ex);
            }
        }

        private void LimpiarCoparticipeAlias()
        {
            try
            {
                PaternoAlias = MaternoAlias = NombreAlias = string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar coparticipe alias", ex);
            }
        }
        #endregion

        #region [COPARTICIPE APODO]
        private void AgregarApodo()
        {
            try
            {
               
                if (SelectedCoparticipe != null)
                {
                    if (SelectedApodo == null)//AGREGAR
                    {
                        LstCoparticipe[SelectedCoparticipeIndex].COPARTICIPE_APODO.Add(new COPARTICIPE_APODO()
                        {
                            APODO = Apodo,
                            //ID_COPARTICIPE = SelectedCoparticipe.ID_COPARTICIPE,
                            //COPARTICIPE = SelectedCoparticipe
                        });
                    }
                    else//EDITAR
                    {
                        SelectedApodo.APODO = Apodo;
                    }
                    LstApodo = new ObservableCollection<COPARTICIPE_APODO>(LstCoparticipe[SelectedCoparticipeIndex].COPARTICIPE_APODO);
                    //PopulateAlias();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar apodo", ex);
            }
        }

        private void EliminarApodo()
        {
            try
            {
                if (SelectedApodo != null)
                {
                    LstCoparticipe[SelectedCoparticipeIndex].COPARTICIPE_APODO.Remove(SelectedApodo);
                    LstApodo = new ObservableCollection<COPARTICIPE_APODO>(LstCoparticipe[SelectedCoparticipeIndex].COPARTICIPE_APODO);
                    LimpiarComparticipeApodo();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar apodo", ex);
            }
        }

        private void PopulateApodo()
        {
            try
            {
                if (SelectedApodo != null)
                    Apodo = SelectedApodo.APODO;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer apodo", ex);
            }
        }

        private void LimpiarComparticipeApodo()
        {
            try
            {
                Apodo = string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar comparticipe apodo", ex);
            }
        }
        #endregion
    }
}

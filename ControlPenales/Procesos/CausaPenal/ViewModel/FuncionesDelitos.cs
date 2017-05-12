using System;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Collections.Generic;
using ControlPenales.Clases;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using SSP.Controlador.Catalogo.Justicia;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        #region [DELITO CAUSA PENAL]
        private bool GuardarDelitoCausaPenal(short ID) {
            try
            {
                if (LstCausaPenalDelitos != null)
                {
                    var cpDelitos = new List<CAUSA_PENAL_DELITO>();
                    short index = 1;
                    foreach (var d in LstCausaPenalDelitos)
                    {
                        cpDelitos.Add(new CAUSA_PENAL_DELITO
                        {
                            ID_CENTRO = SelectedIngreso.ID_CENTRO,
                            ID_ANIO = SelectedIngreso.ID_ANIO,
                            ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectedIngreso.ID_INGRESO,
                            ID_CAUSA_PENAL = ID,
                            ID_DELITO = d.ID_DELITO,
                            ID_FUERO = d.ID_FUERO,
                            ID_MODALIDAD = d.ID_MODALIDAD,
                            ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                            CANTIDAD = d.CANTIDAD,
                            OBJETO = d.OBJETO,
                            DESCR_DELITO = d.DESCR_DELITO,
                            ID_CONS = index
                        });
                        index++;
                    }

                    //if ((new cCausaPenalDelito()).Insertar(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO, ID, cpDelitos))
                    //{
                        return true;
                    //}
                    //else
                    //    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar delito causa penal", ex);
                return false;
            }
        }

        private void PopulateDelitoCausaPenal()
        {
            try
            {
                LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>(new cCausaPenalDelito().ObtenerTodos(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL));
                if (LstCausaPenalDelitos.Count > 0)
                    CausaPenalDelitoEmpty = false;
                else
                    CausaPenalDelitoEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer delito causa penal", ex);
            }
        }

        private bool AgregarDelitoCausaPenal() 
        {
            try
            {
                if (LstCausaPenalDelitos == null)
                    LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>();
                if (SelectedCausaPenalDelito == null)//INSERT
                {
                    if (LstCausaPenalDelitos.Any(w => w.ID_DELITO == SelectedDelitoArbol.ID_DELITO))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El delito ya se encuentra registrado.");
                        return false;
                    }

                    LstCausaPenalDelitos.Add(new CAUSA_PENAL_DELITO
                    {
                        ID_CAUSA_PENAL = (SelectedCausaPenal != null ? SelectedCausaPenal.ID_CAUSA_PENAL : (short)0),
                        ID_DELITO = SelectedDelitoArbol.ID_DELITO,
                        ID_FUERO = SelectedDelitoArbol.ID_FUERO,
                        ID_MODALIDAD = SelectedDelitoArbol.ID_MODALIDAD,
                        ID_TIPO_DELITO = TipoD,
                        CANTIDAD = CantidadD,
                        OBJETO = ObjetoD,
                        MODALIDAD_DELITO = SelectedDelitoArbol,
                        TIPO_DELITO = SelectedTipoDelito,
                        DESCR_DELITO = ModalidadD,
                    });
                }
                else//UPDATE
                {
                    if (SelectedCausaPenalDelito.ID_DELITO != SelectedDelitoArbol.ID_DELITO)
                        if (LstCausaPenalDelitos.Any(w => w.ID_DELITO ==  SelectedDelitoArbol.ID_DELITO))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El delito ya se encuentra registrado.");
                            return false;
                        }
                    SelectedCausaPenalDelito.ID_DELITO = SelectedDelitoArbol.ID_DELITO;
                    SelectedCausaPenalDelito.ID_FUERO = SelectedDelitoArbol.ID_FUERO;
                    SelectedCausaPenalDelito.ID_MODALIDAD = SelectedDelitoArbol.ID_MODALIDAD;
                    SelectedCausaPenalDelito.ID_TIPO_DELITO = TipoD;
                    SelectedCausaPenalDelito.CANTIDAD = CantidadD;
                    SelectedCausaPenalDelito.OBJETO = ObjetoD;
                    SelectedCausaPenalDelito.MODALIDAD_DELITO = SelectedDelitoArbol;
                    SelectedCausaPenalDelito.TIPO_DELITO = SelectedTipoDelito;
                    SelectedCausaPenalDelito.DESCR_DELITO = ModalidadD;
                    LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>(LstCausaPenalDelitos);
                }
                if (LstCausaPenalDelitos.Count > 0)
                    CausaPenalDelitoEmpty = false;
                else
                    CausaPenalDelitoEmpty = true;
                
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error agregar delito causa penal", ex);
            }
            return false;
        }

        private void EliminarDelitoCausaPenal()
        {
            try
            {
                if (SelectedCausaPenalDelito != null)
                {
                    if (LstCausaPenalDelitos != null)
                    {
                        LstCausaPenalDelitos.Remove(SelectedCausaPenalDelito);
                        if (LstCausaPenalDelitos.Count > 0)
                            CausaPenalDelitoEmpty = false;
                        else
                            CausaPenalDelitoEmpty = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error eliminar delito causa penal", ex);
            }
        }
        #endregion

        #region [DELITO SENTENCIA]
        private bool GuardarDelitoSentencia(short ID)
        {
            try
            {
                if (LstSentenciaDelitos != null)
                {
                    List<SENTENCIA_DELITO> sDelitos = new List<SENTENCIA_DELITO>();
                    short index = 1;
                    foreach (var d in LstSentenciaDelitos)
                    {
                        sDelitos.Add(new SENTENCIA_DELITO
                        {
                            ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                            ID_ANIO = SelectedCausaPenal.ID_ANIO,
                            ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                            ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                            ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                            ID_SENTENCIA = ID,
                            ID_DELITO = d.ID_DELITO,
                            ID_FUERO = d.ID_FUERO,
                            ID_MODALIDAD = d.ID_MODALIDAD,
                            ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                            CANTIDAD = d.CANTIDAD,
                            OBJETO = d.OBJETO,
                            DESCR_DELITO = d.DESCR_DELITO,
                            ID_CONS = index
                        });
                        index++;
                    }
                    if (new cSentenciaDelito().Insertar(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL, ID, sDelitos))
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error guardar delito sentencia", ex);
                return false;
            }
        }

        private void PopulateDelitoSentencia()
        {
            try
            {
                if (SelectedSentencia != null)
                    LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>(new cSentenciaDelito().ObtenerTodos(SelectedSentencia.ID_CENTRO, SelectedSentencia.ID_ANIO, SelectedSentencia.ID_IMPUTADO, SelectedSentencia.ID_INGRESO, SelectedSentencia.ID_CAUSA_PENAL, SelectedSentencia.ID_SENTENCIA));
                else
                {
                    var delCP = new cCausaPenalDelito().ObtenerTodos(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL);
                    if (delCP != null)
                    {
                        if (LstSentenciaDelitos == null)
                            LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                        foreach (var d in delCP)
                        {
                            LstSentenciaDelitos.Add(
                                new SENTENCIA_DELITO()
                                {
                                    ID_DELITO = d.ID_DELITO,
                                    ID_FUERO = d.ID_FUERO,
                                    ID_MODALIDAD = d.ID_MODALIDAD,
                                    ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                                    DESCR_DELITO = d.DESCR_DELITO,
                                    CANTIDAD = d.CANTIDAD,
                                    OBJETO = d.OBJETO,
                                    MODALIDAD_DELITO = d.MODALIDAD_DELITO,
                                    TIPO_DELITO = d.TIPO_DELITO
                                });
                        }
                    }
                }
                if (LstSentenciaDelitos.Count > 0)
                    SentenciaDelitoEmpty = false;
                else
                    SentenciaDelitoEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer delito sentencia", ex);
            }
        }

        private bool AgregarDelitoSentencia()
        {
            try
            {
                if (LstSentenciaDelitos == null)
                    LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                if (SelectedSentenciaDelito == null)//INSERT
                {
                    if (LstSentenciaDelitos.Any(w => w.ID_DELITO == SelectedDelitoArbol.ID_DELITO))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El delito ya se encuentra registrado");
                        return false;
                    }

                    LstSentenciaDelitos.Add(new SENTENCIA_DELITO
                    {
                        ID_DELITO = SelectedDelitoArbol.ID_DELITO,
                        ID_FUERO = SelectedDelitoArbol.ID_FUERO,
                        ID_MODALIDAD = SelectedDelitoArbol.ID_MODALIDAD,
                        ID_TIPO_DELITO = TipoD,
                        CANTIDAD = CantidadD,
                        OBJETO = ObjetoD,
                        MODALIDAD_DELITO = SelectedDelitoArbol,
                        TIPO_DELITO = SelectedTipoDelito,
                        DESCR_DELITO = ModalidadD
                    });
                }
                else //UPDATE
                {
                    if (SelectedDelitoArbol != null)
                    {
                        if(SelectedSentenciaDelito.ID_DELITO != SelectedDelitoArbol.ID_DELITO)
                            if (LstSentenciaDelitos.Any(w => w.ID_DELITO == SelectedDelitoArbol.ID_DELITO))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El delito ya se encuentra registrado");
                                return false;
                            }

                        SelectedSentenciaDelito.ID_DELITO = SelectedDelitoArbol.ID_DELITO;
                        SelectedSentenciaDelito.ID_FUERO = SelectedDelitoArbol.ID_FUERO;
                        SelectedSentenciaDelito.ID_MODALIDAD = SelectedDelitoArbol.ID_MODALIDAD;
                        SelectedSentenciaDelito.MODALIDAD_DELITO = SelectedDelitoArbol;
                    }
                    SelectedSentenciaDelito.ID_TIPO_DELITO = TipoD;
                    SelectedSentenciaDelito.CANTIDAD = CantidadD;
                    SelectedSentenciaDelito.OBJETO = ObjetoD;
                    SelectedSentenciaDelito.TIPO_DELITO = SelectedTipoDelito;
                    SelectedSentenciaDelito.DESCR_DELITO = ModalidadD;
                    LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>(LstSentenciaDelitos);
                }
                if (LstSentenciaDelitos.Count > 0)
                    SentenciaDelitoEmpty = false;
                else
                    SentenciaDelitoEmpty = true;

                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar delito sentencia", ex);
            }
            return false;
        }

        private void EliminarDelitoSentencia()
        {
            try
            {
                if (SelectedSentenciaDelito != null)
                {
                    if (LstSentenciaDelitos != null)
                    {
                        LstSentenciaDelitos.Remove(SelectedSentenciaDelito);
                        if (LstSentenciaDelitos.Count > 0)
                            SentenciaDelitoEmpty = false;
                        else
                            SentenciaDelitoEmpty = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar delito sentencia", ex);
            }
        }
        #endregion

        #region [DELITO RECURSO]

        private void SearchByName(string Partial)
        {
            try
            {
                if (string.IsNullOrEmpty(Partial))
                    return;
                else
                    if (Partial.Length < 3)
                        return;

                LstDelitoTitulo.Clear();//Limpia la lista para que se vaya formando en base a los filtros que ingrese el usuario
                LstDelitos.Clear();

                LstDelitos = new ObservableCollection<DELITO>(new cDelito().ObtenerTodos().Where(x => x.DESCR.Contains(Partial) || x.DELITO_GRUPO.DESCR.Contains(Partial) || x.DELITO_GRUPO.DELITO_TITULO.DESCR.Contains(Partial)));
                
                //consulta los delitos para crear una lista inicial.
                foreach (var item in LstDelitos)
                {
                    if (item.DELITO_GRUPO != null && item.DELITO_GRUPO.DELITO_TITULO != null)
                        if (!LstDelitoTitulo.AsQueryable().Contains(item.DELITO_GRUPO.DELITO_TITULO))
                            LstDelitoTitulo.Add(item.DELITO_GRUPO.DELITO_TITULO);
                        else
                            continue;
                    else
                        continue;
                };

                //Una vez formada la lista, se refinan los resultados una vez mas para hacerla mas precisa y refina los grupos :D
                if (LstDelitoTitulo.AsQueryable().Any())
                {
                    foreach(var item in LstDelitoTitulo.ToList().AsQueryable())
                    {
                        if (item.DELITO_GRUPO.AsQueryable().Any())
                        {
                            foreach(var item2 in item.DELITO_GRUPO.ToList().AsQueryable())//N GRUPOS POR CADA MODALIDD
                            {
                                if (item2.DELITO.AsQueryable().Any())//N DELITOS POR CADA GRUPO
                                    foreach(var item3 in item2.DELITO.ToList().AsQueryable())
                                    {
                                        if (item3.DESCR.Contains(Partial))//el delito si coincide, se agrega ala lista
                                            continue;

                                        item2.DELITO.Remove(item3);//el delito no coincide
                                    };

                                if (!item2.DESCR.Contains(Partial))
                                    item.DELITO_GRUPO.Remove(item2);//el nombre del grupo no coincide con el parametro ingresado por el usuario

                                else
                                    continue;
                            };
                        };

                        if (!item.DESCR.Contains(Partial) && item.DELITO_GRUPO.Count == 0)
                            LstDelitoTitulo.Remove(item);
                    };
                };
            }

            catch (Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener el detalle de los delitos", exc);
                return;
            }

            return;
        }

        private bool GuardarDelitoRecurso(short ID)
        {
            try
            {
                if (LstRecursoDelitos != null)
                {
                    var del = new List<RECURSO_DELITO>();
                    short index = 1;
                    foreach (var d in LstRecursoDelitos)
                    {
                        del.Add(new RECURSO_DELITO
                        {
                            ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                            ID_ANIO = SelectedCausaPenal.ID_ANIO,
                            ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                            ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                            ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                            ID_RECURSO = ID,
                            ID_DELITO = d.ID_DELITO,
                            ID_FUERO = d.ID_FUERO,
                            ID_MODALIDAD = d.ID_MODALIDAD,
                            ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                            CANTIDAD = d.CANTIDAD,
                            OBJETO = d.OBJETO,
                            DESCR_DELITO = d.DESCR_DELITO,
                            ID_CONS = index
                        });
                        index++;
                    }
                    if ((new cRecursoDelito()).Insertar(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL, ID, del))
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar delito recurso", ex);
                return false;
            }
        }

        private void PopulateDelitoRecusrso()
        {
            try
            {
                if (SelectedRecurso != null)
                {
                    LstRecursoDelitos = new ObservableCollection<RECURSO_DELITO>(new cRecursoDelito().ObtenerTodos(SelectedRecurso.ID_CENTRO, SelectedRecurso.ID_ANIO, SelectedRecurso.ID_IMPUTADO, SelectedRecurso.ID_INGRESO, SelectedRecurso.ID_CAUSA_PENAL, SelectedRecurso.ID_RECURSO));
                }
                else
                {
                    if (SelectedSentencia != null)
                    {
                        var delSen = new cSentenciaDelito().ObtenerTodos(SelectedSentencia.ID_CENTRO, SelectedSentencia.ID_ANIO, SelectedSentencia.ID_IMPUTADO, SelectedSentencia.ID_INGRESO, SelectedSentencia.ID_CAUSA_PENAL, SelectedSentencia.ID_SENTENCIA);
                        if (delSen != null)
                        {
                            if (LstRecursoDelitos == null)
                                LstRecursoDelitos = new ObservableCollection<RECURSO_DELITO>();
                            foreach (var d in delSen)
                            {
                                LstRecursoDelitos.Add(
                                    new RECURSO_DELITO()
                                    {
                                        ID_DELITO = d.ID_DELITO,
                                        ID_FUERO = d.ID_FUERO,
                                        ID_MODALIDAD = d.ID_MODALIDAD,
                                        ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                                        DESCR_DELITO = d.DESCR_DELITO,
                                        CANTIDAD = d.CANTIDAD,
                                        OBJETO = d.OBJETO,
                                        MODALIDAD_DELITO = d.MODALIDAD_DELITO,
                                        TIPO_DELITO = d.TIPO_DELITO
                                    });
                            }
                        }
                    }
                }
                if (LstRecursoDelitos.Count > 0)
                    RecursoDelitoEmpty = false;
                else
                    RecursoDelitoEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer delito recurso", ex);
            }
        }

        private bool AgregarDelitoRecurso()
        {
            try
            {
                if (LstRecursoDelitos == null)
                LstRecursoDelitos = new ObservableCollection<RECURSO_DELITO>(); 
                if (SelectedRecursoDelito == null)//INSERT
                {
                    if (LstRecursoDelitos.Any(w => w.ID_DELITO == SelectedDelitoArbol.ID_DELITO))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El delito ya se encuentra registrado."); 
                        return false;
                    }

                    LstRecursoDelitos.Add(new RECURSO_DELITO
                    {
                        ID_DELITO = SelectedDelitoArbol.ID_DELITO,
                        ID_FUERO = SelectedDelitoArbol.ID_FUERO,
                        ID_MODALIDAD = SelectedDelitoArbol.ID_MODALIDAD,
                        ID_TIPO_DELITO = TipoD,
                        CANTIDAD = CantidadD,
                        OBJETO = ObjetoD,
                        MODALIDAD_DELITO = SelectedDelitoArbol,
                        TIPO_DELITO = SelectedTipoDelito,
                        DESCR_DELITO = ModalidadD
                    });
                }
                else//DELETE
                {
                    if (SelectedRecursoDelito.ID_DELITO != SelectedDelitoArbol.ID_DELITO)
                        if (LstRecursoDelitos.Any(w => w.ID_DELITO == SelectedDelitoArbol.ID_DELITO))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El delito ya se encuentra registrado."); 
                            return false;
                        }
                    SelectedRecursoDelito.ID_DELITO = SelectedDelitoArbol.ID_DELITO;
                    SelectedRecursoDelito.ID_FUERO = SelectedDelitoArbol.ID_FUERO;
                    SelectedRecursoDelito.ID_MODALIDAD = SelectedDelitoArbol.ID_MODALIDAD;
                    SelectedRecursoDelito.ID_TIPO_DELITO = TipoD;
                    SelectedRecursoDelito.CANTIDAD = CantidadD;
                    SelectedRecursoDelito.OBJETO = ObjetoD;
                    SelectedRecursoDelito.MODALIDAD_DELITO = SelectedDelitoArbol;
                    SelectedRecursoDelito.TIPO_DELITO = SelectedTipoDelito;
                    SelectedRecursoDelito.DESCR_DELITO = ModalidadD;
                    LstRecursoDelitos = new ObservableCollection<RECURSO_DELITO>(LstRecursoDelitos);
                }
                if (LstRecursoDelitos.Count > 0)
                    RecursoDelitoEmpty = false;
                else
                    RecursoDelitoEmpty = true;

                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar delito recurso", ex);
            }
            return false;
        }

        private void EliminarDelitoRecurso()
        {
            try
            {
                if (SelectedRecursoDelito != null)
                {
                    if (LstRecursoDelitos != null)
                    {
                        LstRecursoDelitos.Remove(SelectedRecursoDelito);
                        if (LstRecursoDelitos.Count > 0)
                            RecursoDelitoEmpty = false;
                        else
                            RecursoDelitoEmpty = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar delito recurso", ex);
            }
        }
        #endregion

        #region [GENERALES]
        private bool GuardarDelitos() 
        {
            try
            {
                var delitosCP = new List<CAUSA_PENAL_DELITO>(LstCausaPenalDelitos.Select((w, i) => new CAUSA_PENAL_DELITO() { 
                            ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                            ID_ANIO = SelectedCausaPenal.ID_ANIO,
                            ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                            ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                            ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                            ID_DELITO = w.ID_DELITO,
                            ID_FUERO = w.ID_FUERO,
                            ID_MODALIDAD = w.ID_MODALIDAD,
                            ID_TIPO_DELITO = w.ID_TIPO_DELITO,
                            CANTIDAD = w.CANTIDAD,
                            OBJETO = w.OBJETO,
                            DESCR_DELITO = w.DESCR_DELITO,
                            ID_CONS = Convert.ToInt16(i + 1)
                }));
                
                var delitosS = new List<SENTENCIA_DELITO>(LstSentenciaDelitos.Select((w, i) => new SENTENCIA_DELITO()
                {
                    ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                    ID_ANIO = SelectedCausaPenal.ID_ANIO,
                    ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                    ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                    ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                    ID_SENTENCIA = SelectedCausaPenal.ID_CAUSA_PENAL,
                    ID_DELITO = w.ID_DELITO,
                    ID_FUERO = w.ID_FUERO,
                    ID_MODALIDAD = w.ID_MODALIDAD,
                    ID_TIPO_DELITO = w.ID_TIPO_DELITO,
                    CANTIDAD = w.CANTIDAD,
                    OBJETO = w.OBJETO,
                    DESCR_DELITO = w.DESCR_DELITO,
                    ID_CONS = Convert.ToInt16(i + 1)
                }));

                short id_sentencia = 0;
                var sentencia = SelectedCausaPenal.SENTENCIA.Where(w => w.ESTATUS == "A").SingleOrDefault();
                if (sentencia != null)
                {
                    id_sentencia = sentencia.ID_SENTENCIA;
                }
                if ((new cCausaPenalDelito()).Insertar(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL, id_sentencia, delitosCP, delitosS))
                {
                    return true;
                }
                else
                    return false;

                //if (GuardarDelitoCausaPenal(SelectedCausaPenal.ID_CAUSA_PENAL))
                //{
                //    if (SelectedCausaPenal.SENTENCIA != null)
                //    {
                //        if (SelectedCausaPenal.SENTENCIA.Count > 0)
                //        {
                //            SelectedSentencia = SelectedCausaPenal.SENTENCIA.FirstOrDefault();
                //            if (SelectedSentencia != null)
                //            {
                //                if (GuardarDelitoSentencia(SelectedSentencia.ID_SENTENCIA))
                //                    return true;
                //            }
                //        }
                //        else
                //            return true;
                //    }
                //    else
                //        return true;
                //}
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar delito", ex);
            }
            return false;
        }
        #endregion
    }
}

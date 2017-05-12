using System;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Collections.Generic;
using ControlPenales.Clases;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using SSP.Controlador.Catalogo.Justicia;
using System.Windows.Controls;
using System.Windows;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {                                                                                       
        #region [ARBOL INGRESOS]
        private void ViewModelArbol()
        {
            var si = SelectedIngreso;
            var scp = SelectedCausaPenal;

            if (SelectedIngreso != null)
                if (SelectedIngreso.IMPUTADO != null)
                    SelectExpediente = new cImputado().Obtener(SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_CENTRO).FirstOrDefault();
            if (SelectExpediente != null)
            {
                _TreeList = new List<TreeViewList>();
                var ItemTreeRaiz = new TreeViewList();
                var ItemTreeIngreso = new List<TreeViewList>();
                foreach (var ingreso in SelectExpediente.INGRESO.Where(w => w.ID_UB_CENTRO == GlobalVar.gCentro && w.ID_INGRESO == SelectedIngreso.ID_INGRESO))
                {
                    var ItemTreeAmparoIndirecto = new List<TreeViewList>();
                    //CAUSA PENAL
                    var ItemTreeCausaPenal = new List<TreeViewList>();
                    foreach (var cp in ingreso.CAUSA_PENAL)
                    {
                        var ItemTreeCausaPenalDelito = new List<TreeViewList>();
                        var ItemTreeSentenciaDelito = new List<TreeViewList>();
                        var ItemTreeRecursos = new List<TreeViewList>();
                        var ItemTreeAmparo = new List<TreeViewList>();
                        var ItemTreeAmparoIncidente = new List<TreeViewList>();
                        var ItemTreeMultiple = new List<TreeViewList>();

                        //CAUSA PENAL DELITO
                        foreach (var cpd in cp.CAUSA_PENAL_DELITO)
                        {
                            ItemTreeCausaPenalDelito.Add(new TreeViewList
                            {
                                IsRoot = false,
                                IsCheck = false,
                                Text = cpd.MODALIDAD_DELITO.DELITO.DESCR,
                                Value = cpd
                            });
                        }

                        //SENTENCIA DELITO
                        if (cp.SENTENCIA != null)
                        {
                            ItemTreeSentenciaDelito = new List<TreeViewList>();
                            foreach (var s in cp.SENTENCIA)
                            {
                                foreach (var sd in s.SENTENCIA_DELITO)
                                {
                                    ItemTreeSentenciaDelito.Add(new TreeViewList
                                    {
                                        IsRoot = false,
                                        IsCheck = false,
                                        Text = sd.MODALIDAD_DELITO.DELITO.DESCR,
                                        Value = sd
                                    });
                                }
                            }
                        }

                        //RECURSOS
                        if (cp.RECURSO != null)
                        {
                            ItemTreeRecursos = new List<TreeViewList>();
                            var rec = cp.RECURSO.OrderBy(w => w.ID_RECURSO);
                            foreach (var r in rec)
                            {
                                ItemTreeRecursos.Add(new TreeViewList
                                {
                                    IsRoot = false,
                                    IsCheck = false,
                                    Text = string.Format("{0}/{1}", r.ID_RECURSO, r.TIPO_RECURSO.DESCR),
                                    Value = r
                                });
                            }
                        }

                        //AMPAROS DIRECTOS
                        if (cp.AMPARO_DIRECTO != null)
                        {
                            ItemTreeAmparo = new List<TreeViewList>();
                            var ad = cp.AMPARO_DIRECTO.OrderBy(w => w.ID_AMPARO_DIRECTO);
                            foreach (var x in ad)
                            {
                                if (!string.IsNullOrEmpty(x.AMPARO_NUM))
                                {
                                    ItemTreeAmparo.Add(new TreeViewList
                                    {
                                        IsRoot = false,
                                        IsCheck = false,
                                        Text = string.Format("No.Amparo:{0}", x.AMPARO_NUM),
                                        Value = x
                                    });
                                }
                                else
                                {
                                    ItemTreeAmparo.Add(new TreeViewList
                                    {
                                        IsRoot = false,
                                        IsCheck = false,
                                        Text = string.Format("No.Oficio:{0}", x.OFICIO_NUM),
                                        Value = x
                                    });
                                }
                            }
                        }

                        //AMPAROS INDIRECTOS
                        if (cp.AMPARO_INDIRECTO != null)
                        {
                            if (ItemTreeAmparo == null)
                                ItemTreeAmparo = new List<TreeViewList>();
                            var ai = cp.AMPARO_INDIRECTO.OrderBy(w => w.ID_AMPARO_INDIRECTO);
                            foreach (var x in ai)
                            {
                                if (!string.IsNullOrEmpty(x.AMPARO_NUM))
                                {
                                    ItemTreeAmparo.Add(new TreeViewList
                                    {
                                        IsRoot = false,
                                        IsCheck = false,
                                        Text = string.Format("No.Amparo:{0}", x.AMPARO_NUM),
                                        Value = x
                                    });
                                }
                                else
                                {
                                    ItemTreeAmparo.Add(new TreeViewList
                                    {
                                        IsRoot = false,
                                        IsCheck = false,
                                        Text = string.Format("No.Oficio:{0}", x.OFICIO_NUM),
                                        Value = x
                                    });
                                }
                            }
                        }

                        //INCIDENTES
                        if (cp.AMPARO_INCIDENTE != null)
                        {
                            ItemTreeAmparoIncidente = new List<TreeViewList>();
                            var ai = cp.AMPARO_INCIDENTE.OrderBy(w => w.ID_AMPARO_INCIDENTE);
                            foreach (var y in ai)
                            {
                                ItemTreeAmparoIncidente.Add(new TreeViewList
                                {
                                    IsRoot = false,
                                    IsCheck = false,
                                    Text = string.Format(y.AMPARO_INCIDENTE_TIPO.DESCR),
                                    Value = y
                                });
                            }
                        }

                        //DELITOS Y RECURSOS
                        ItemTreeMultiple.Add(new TreeViewList
                        {
                            IsRoot = false,
                            IsCheck = false,
                            Text = "Delitos Causa Penal",
                            Node = ItemTreeCausaPenalDelito,
                            Value = cp
                        });

                        ItemTreeMultiple.Add(new TreeViewList
                        {
                            IsRoot = false,
                            IsCheck = false,
                            Text = "Delitos Sentencia",
                            Node = ItemTreeSentenciaDelito,
                            Value = cp
                        });

                        //RECURSOS
                        ItemTreeMultiple.Add(new TreeViewList
                        {
                            IsRoot = false,
                            IsCheck = false,
                            Text = string.Format("Recursos: {0}", ItemTreeRecursos.Count),
                            Node = ItemTreeRecursos,
                            Value = cp
                        });

                        //INCIDENTES
                        ItemTreeMultiple.Add(new TreeViewList
                        {
                            IsRoot = false,
                            IsCheck = false,
                            Text = string.Format("Incidentes: {0}", ItemTreeAmparoIncidente.Count),
                            Node = ItemTreeAmparoIncidente,
                            Value = cp
                        });

                        //AMPAROS DIRECTO
                        ItemTreeMultiple.Add(new TreeViewList
                        {
                            IsRoot = false,
                            IsCheck = false,
                            Text = string.Format("Amparos: {0}", ItemTreeAmparo.Count),
                            Node = ItemTreeAmparo,
                            Value = cp
                        });

                        //CAUSA PENAL
                        ItemTreeCausaPenal.Add(new TreeViewList
                        {
                            IsRoot = false,
                            IsCheck = false,
                            Text = string.Format("{0}/{1} {2}", cp.CP_ANIO, cp.CP_FOLIO, cp.CAUSA_PENAL_ESTATUS.DESCR),
                            Value = cp,
                            Node = ItemTreeMultiple
                        });
                    }

                    //AMPAROS INDIRECTOS
                    if (ingreso.AMPARO_INDIRECTO != null)
                    {
                        ItemTreeAmparoIndirecto = new List<TreeViewList>();
                        var ai = ingreso.AMPARO_INDIRECTO.Where(w => w.ID_CAUSA_PENAL == null).OrderBy(w => w.ID_AMPARO_INDIRECTO);
                        foreach (var x in ai)
                        {
                            if (!string.IsNullOrEmpty(x.AMPARO_NUM))
                            {
                                ItemTreeAmparoIndirecto.Add(new TreeViewList
                                {
                                    IsRoot = false,
                                    IsCheck = false,
                                    Text = string.Format("No.Amparo:{0}", x.AMPARO_NUM),
                                    Value = x
                                });
                            }
                            else
                            {
                                ItemTreeAmparoIndirecto.Add(new TreeViewList
                                {
                                    IsRoot = false,
                                    IsCheck = false,
                                    Text = string.Format("No.Oficio:{0}", x.OFICIO_NUM),
                                    Value = x
                                });
                            }
                        }
                    }

                    ItemTreeCausaPenal.Add(new TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = string.Format("Amparos Indirectos: {0}", ItemTreeAmparoIndirecto.Count),
                        Node = ItemTreeAmparoIndirecto,
                        Value = ingreso
                    });

                    //INGRESO
                    List<TreeViewList> nodeX = null;
                    if (ItemTreeCausaPenal != null)
                        if (ItemTreeCausaPenal.Count > 0)
                        {
                            nodeX = ItemTreeCausaPenal;
                        }

                    ItemTreeIngreso.Add(new TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = string.Format("Ingreso:{0} Fecha:{1}", ingreso.ID_INGRESO, ingreso.FEC_REGISTRO.Value.ToString("dd/MM/yyyy")),
                        Value = ingreso,
                        Node = nodeX,
                        IsNodeExpanded = true

                    });
                }

                //NODO RAIZ
                ItemTreeRaiz.IsRoot = true;
                ItemTreeRaiz.IsCheck = false;
                ItemTreeRaiz.IsNodeExpanded = true;
                ItemTreeRaiz.Text = "INGRESOS";
                ItemTreeRaiz.Node = ItemTreeIngreso;
                TreeList.Add(ItemTreeRaiz);
                OnPropertyChanged("TreeList");

                if(si != null)
                    SelectedIngreso = SelectExpediente.INGRESO.Where(w => w.ID_INGRESO == si.ID_INGRESO).FirstOrDefault();
                if (scp != null)
                    SelectedCausaPenal = SelectedIngreso.CAUSA_PENAL.Where(w => w.ID_CAUSA_PENAL == scp.ID_CAUSA_PENAL).FirstOrDefault();

            }
        }
        
        private async void SeleccionaIngresoArbol(Object obj)
        {
            //validamos el cambio de opcion
            if (StaticSourcesViewModel.SourceChanged)
            { 
                 var respuesta = await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Seguro que desea salir sin guardar?");
                 if (respuesta == 0)
                     return;
            }
           
            if (obj == null)
                return;
            var arbol = (TreeView)obj;
            var x = (TreeViewList)arbol.SelectedValue;

            if (x == null)
                return;

            if (x.IsRoot)//es la raiz
            {
                IngresosVisible = true;
                DatosIngresoVisible = false;

            }
            else
            {
                if (x.Text.Equals("Delitos Causa Penal") || x.Text.Equals("Delitos Sentencia"))
                {
                    SelectedCausaPenal = (CAUSA_PENAL)x.Value;
                    SelectedIngreso = SelectedCausaPenal.INGRESO;
                    if (SelectedIngreso != null)
                    {
                        MostrarOpcion = SelectedIngreso.ID_ESTATUS_ADMINISTRATIVO != 4 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                        MostrarOpcion = Visibility.Collapsed;
                    if (SelectedCausaPenal != null)
                    {
                        //CAUSA PENAL
                        var scp = SelectedCausaPenal;
                        PopulateDelitoCausaPenal();
                        SelectedCausaPenal = scp;
                        SelectedIngreso = SelectedCausaPenal.INGRESO;
                        if (SelectedCausaPenal.SENTENCIA != null)
                        {
                            if (SelectedCausaPenal.SENTENCIA.Count > 0)
                            {
                                SelectedSentencia = SelectedCausaPenal.SENTENCIA.FirstOrDefault();
                                //Sentencia
                                PopulateDelitoSentencia();
                            }
                            else
                                SelectedSentencia = null;

                        }

                    }
                 
                    TabDelitosSelected = true;
                    IngresosVisible = false;
                    DatosIngresoVisible = true;
                    //TABS
                    DelitosTab = true;
                    CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = RecursosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = AmparoIncidenteTab = false;
                    StaticSourcesViewModel.SourceChanged = false;
                }
                else if (x.Text.StartsWith("Recursos:"))//("Partida Antecedentes"))
                {
                    SelectedCausaPenal = (CAUSA_PENAL)x.Value;
                    SelectedIngreso = SelectedCausaPenal.INGRESO;
                    if (SelectedIngreso != null)
                    {
                        MostrarOpcion = SelectedIngreso.ID_ESTATUS_ADMINISTRATIVO != 4 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                        MostrarOpcion = Visibility.Collapsed;
                    if (SelectedCausaPenal != null)
                    {
                        SelectedIngreso = SelectedCausaPenal.INGRESO;
                        if (SelectedCausaPenal.SENTENCIA != null)
                        {
                            if (SelectedCausaPenal.SENTENCIA.Count > 0)
                                SelectedSentencia = SelectedCausaPenal.SENTENCIA.FirstOrDefault();
                        }
                        else
                            SelectedSentencia = null;

                    }
                    SelectedRecurso = null;
                    TabRecursosSelected = true;
                    IngresosVisible = false;
                    DatosIngresoVisible = true;
                    //TABS
                    RecursosTab = true;
                    CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = AmparoIncidenteTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = false;
                    //if (SelectedCausaPenal != null)
                    //    PopulateRecursoListado();
                    ObtenerTodoRecurso();
                    StaticSourcesViewModel.SourceChanged = false;
                }
                else if (x.Text.StartsWith("Amparos:"))
                {
                    SelectedCausaPenal = (CAUSA_PENAL)x.Value;
                    if (SelectedIngreso != null)
                    {
                        MostrarOpcion = SelectedIngreso.ID_ESTATUS_ADMINISTRATIVO != 4 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                        MostrarOpcion = Visibility.Collapsed;
                    SelectedIngreso = SelectedCausaPenal.INGRESO;
                    TabAmparoDirectoListaSelected = true;
                    IngresosVisible = false;
                    DatosIngresoVisible = true;
                    AmparoDirectoListaTab = true;
                    RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoIndirectoTab = AmparoIncidenteListaTab = AmparoIncidenteTab =  false;
                    if (SelectedCausaPenal != null)
                    {   
                        ObtenerTodoAmparoIndirecto(2);
                        ObtenerTodoAmparoDirecto();
                    }
                    StaticSourcesViewModel.SourceChanged = false;
                }
                else if (x.Text.StartsWith("Amparos Indirectos"))
                {
                    SelectedIngreso = (INGRESO)x.Value;
                    if (SelectedIngreso != null)
                    {
                        MostrarOpcion = SelectedIngreso.ID_ESTATUS_ADMINISTRATIVO != 4 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                        MostrarOpcion = Visibility.Collapsed;

                    TabAmparoIndirectoListaSelected = true;
                    IngresosVisible = false;
                    DatosIngresoVisible = true;
                    AmparoIndirectoListaTab = true;
                    RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoDirectoListaTab = AmparoDirectoTab = AmparoIndirectoTab = AmparoIncidenteListaTab = AmparoIncidenteTab = false;
                    if (SelectedIngreso != null)
                    {
                        ObtenerTodoAmparoIndirecto(1);
                    }
                    StaticSourcesViewModel.SourceChanged = false;
                }
                else if (x.Text.StartsWith("Incidentes"))
                {
                    SelectedCausaPenal = (CAUSA_PENAL)x.Value;
                    SelectedIngreso = SelectedCausaPenal.INGRESO;
                    if (SelectedIngreso != null)
                    {
                        MostrarOpcion = SelectedIngreso.ID_ESTATUS_ADMINISTRATIVO != 4 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                        MostrarOpcion = Visibility.Collapsed;
                    TabAmparoIncidenteListaSelected = true;
                    IngresosVisible = false;
                    DatosIngresoVisible = true;
                    AmparoIncidenteListaTab = true;
                    RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoIndirectoTab = AmparoIncidenteTab = AmparoDirectoListaTab = false;
                    if (SelectedCausaPenal != null)
                        PopulateAmparoIncidenteListado();
                    StaticSourcesViewModel.SourceChanged = false;
                }
                else
                {
                    switch ((x.Value.GetType()).BaseType.Name)
                    {
                        case "INGRESO":
                            TabIngresoSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            SelectedIngreso = SelectIngreso = (INGRESO)x.Value;
                            short[] Estatus = { 1, 2, 3, 8 };
                            if (!Estatus.Contains(SelectedIngreso.ID_ESTATUS_ADMINISTRATIVO.Value))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no se encuentra activo la información sera solo de consulta");
                                MenuGuardarEnabled = false;
                            }
                            else
                                MenuGuardarEnabled = true;
                            //TABS
                            IngresoTab = true;
                            CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = DelitosTab = DelitoTab = RecursoTab = RecursosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = AmparoIncidenteTab = false;
                            if (SelectedIngreso != null)
                                PopulateIngreso();
                            StaticSourcesViewModel.SourceChanged = false;
                            break;
                        case "CAUSA_PENAL":
                            TabCausaPenalSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            //TABS
                            CausaPenalTab = CoparticipeTab = SentenciaTab = true;
                            IngresoTab = RdiTab = DelitosTab = DelitoTab = RecursoTab = RecursosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = AmparoIncidenteTab = false;
                            SelectedCausaPenal = ((CAUSA_PENAL)x.Value);
                            if (SelectedCausaPenal.INGRESO != null)
                            {
                                SelectedIngreso = SelectedCausaPenal.INGRESO;
                                MostrarOpcion = SelectedIngreso.ID_ESTATUS_ADMINISTRATIVO != 4 ? Visibility.Visible : Visibility.Collapsed;
                            }
                            else
                                MostrarOpcion = Visibility.Collapsed;
                            if (SelectedCausaPenal != null)
                            {
                                PopulateCausaPenal();
                            }
                            GuardarBandera = true;

                            StaticSourcesViewModel.SourceChanged = false;
                            break;
                        case "CAUSA_PENAL_DELITO":
                            SetValidacionesCausaPenal();
                            TabDelitoSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            //TABS
                            DelitoTab = true;
                            CausaPenalTab = CoparticipeTab = SentenciaTab = IngresoTab = RdiTab = DelitosTab = RecursoTab = RecursosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = AmparoIncidenteTab = false;
                            SelectedCausaPenalDelito = ((CAUSA_PENAL_DELITO)x.Value);
                            StaticSourcesViewModel.SourceChanged = false;
                            break;
                        case "SENTENCIA_DELITO":
                            TabDelitoSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            //TABS
                            DelitoTab = true;
                            CausaPenalTab = CoparticipeTab = SentenciaTab = IngresoTab = RdiTab = DelitosTab = RecursoTab = RecursosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = AmparoIncidenteTab = false;
                            SelectedSentenciaDelito = ((SENTENCIA_DELITO)x.Value);
                            StaticSourcesViewModel.SourceChanged = false;
                            break;
                        case "RECURSO":
                            SelectedRecurso = (RECURSO)x.Value;
                            SelectedCausaPenal = SelectedRecurso.CAUSA_PENAL;
                            SelectedIngreso = SelectedCausaPenal.INGRESO;
                            TabRecursoSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            //TABS
                            RecursoTab = true;
                            CausaPenalTab = CoparticipeTab = SentenciaTab = IngresoTab = RdiTab = DelitosTab = DelitoTab = RecursosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = AmparoIncidenteTab = false;

                            SetValidacionesRecurso();
                            LimpiarRecurso();
                            if (SelectedRecurso != null)
                                ObtenerRecurso();
                            StaticSourcesViewModel.SourceChanged = false;
                            break;
                        case "AMPARO_DIRECTO":
                            SelectedAmparoDirecto = (AMPARO_DIRECTO)x.Value;
                            if (SelectedAmparoDirecto.CAUSA_PENAL != null)
                                SelectedCausaPenal = SelectedAmparoDirecto.CAUSA_PENAL;
                            TabAmparoDirectoSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            AmparoDirectoTab = true;
                            RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = AmparoIncidenteTab = false;

                            SetValidacionesAmparoDirecto();
                            LimpiarAmparoDirecto();
                            if (SelectedAmparoDirecto != null)
                                ObtenerAmparoDirecto();
                            StaticSourcesViewModel.SourceChanged = false;
                            break;
                        case "AMPARO_INDIRECTO":
                            SelectedAmparoIndirecto = (AMPARO_INDIRECTO)x.Value;
                            SelectedIngreso = SelectedAmparoIndirecto.INGRESO;
                            if (SelectedAmparoIndirecto.CAUSA_PENAL != null)
                                SelectedCausaPenal = SelectedAmparoIndirecto.CAUSA_PENAL;
                            //PopulateAmparoIndirecto();
                            TabAmparoIndirectoSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            AmparoIndirectoTab = true;
                            RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoDirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = AmparoIncidenteTab = false;

                            SetValidacionesAmparoIndirecto();
                            LimpiarAmparoIndirecto();
                            if (SelectedAmparoIndirecto != null)
                            {
                                ObtenerTipoAmparoIndirecto(SelectedAmparoIndirecto.ID_CAUSA_PENAL != null ? "S" : "N");
                                ObtenerAmparoIndirecto();
                            }
                            StaticSourcesViewModel.SourceChanged = false;
                            break;
                        case "AMPARO_INCIDENTE":
                            SelectedAmparoIncidente = (AMPARO_INCIDENTE)x.Value;
                            if (SelectedAmparoIncidente.CAUSA_PENAL != null)
                            {
                                SelectedCausaPenal = SelectedAmparoIncidente.CAUSA_PENAL;
                                SelectedIngreso = SelectedCausaPenal.INGRESO;
                            }
                            TabAmparoIncidenteSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            AmparoIncidenteTab = true;
                            RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = false;

                            SetValidacionesIncidentes();
                            LimpiarIncidente();
                            if (SelectedAmparoIncidente != null)
                                ObtenerIncidente();
                            StaticSourcesViewModel.SourceChanged = false;
                            break;
                    }
                }
            }
        }
        #endregion

        #region[ARBOL UBICACION]
        private void ViewModelArbolUbicacion()
        {
            #region [COMENTADO]
            //string celda = string.Empty;
            //int edificio, sector, cE, cS, cC;
            //edificio = sector = 0;
            //cE = cS = cC = -1;
            //Ubicaciones = new CENTRO();
            //var camas = new List<CAMA>();
            //if (SelectedCausaPenal != null)
            //    camas = (new cCama()).GetData().OrderBy(o => new { o.ID_CENTRO, o.CELDA.SECTOR.EDIFICIO.ID_EDIFICIO, o.CELDA.SECTOR.ID_SECTOR, o.CELDA.ID_CELDA, o.ID_CAMA }).Where(w => w.ID_CENTRO == SelectedCausaPenal.ID_CENTRO && w.ESTATUS == (short)1).ToList();
            //else
            //    camas = (new cCama()).GetData().OrderBy(o => new { o.ID_CENTRO, o.CELDA.SECTOR.EDIFICIO.ID_EDIFICIO, o.CELDA.SECTOR.ID_SECTOR, o.CELDA.ID_CELDA, o.ID_CAMA }).Where(w => w.ID_CENTRO == SelectedIngreso.ID_CENTRO && w.ESTATUS == (short)1).ToList();
            //if (camas != null)
            //{
            //    Ubicaciones = camas[0].CELDA.SECTOR.EDIFICIO.CENTRO;
            //    Ubicaciones.EDIFICIO.Clear();
            //    var listaCamas = new List<CAMA>();
            //    var listaCeldas = new List<CELDA>();
            //    var listaSectores = new List<SECTOR>();
            //    var listaEdificios = new List<EDIFICIO>();
            //    foreach (var cama in camas)
            //    {
            //        //edificios
            //        if (cama.ID_EDIFICIO != edificio)
            //        {
            //            //listaSectores = new List<SECTOR>();
            //            listaEdificios.Add(cama.CELDA.SECTOR.EDIFICIO);
            //            edificio = cama.CELDA.SECTOR.EDIFICIO.ID_EDIFICIO;
            //            cE++;
            //        }
            //        if (cama.ID_EDIFICIO == edificio && cama.ID_SECTOR != sector)
            //        {
            //            //listaCeldas = new List<CELDA>();
            //            listaSectores.Add(cama.CELDA.SECTOR);
            //            sector = cama.CELDA.SECTOR.ID_SECTOR;
            //            cS++;
            //        }
            //        if (cama.ID_EDIFICIO == edificio && cama.ID_SECTOR == sector && !cama.ID_CELDA.Equals(celda))
            //        {
            //            //                        listaCamas = new List<CAMA>();
            //            listaCeldas.Add(cama.CELDA);
            //            celda = cama.ID_CELDA;
            //            cC++;
            //        }

            //        listaCeldas[cC].CAMAs.Add(cama);
            //        listaSectores[cS].CELDAs.Add(listaCeldas[cC]);
            //        listaEdificios[cE].SECTORs.Add(listaSectores[cS]);
            //    }
            //    Ubicaciones.EDIFICIO = listaEdificios;
            //}
            #endregion
            var u = (new cCentro()).Obtener(4).FirstOrDefault();
            if (u != null)
            {
                if (u.EDIFICIO != null)
                    foreach (var e in u.EDIFICIO)
                    {
                        foreach (var s in e.SECTOR)
                        {
                            foreach (var c in s.CELDA)
                            {
                                var iCama = new List<CAMA>();
                                foreach (var ca in c.CAMA)
                                {
                                    if (ca.ESTATUS == "S")
                                        iCama.Add(ca);
                                }

                                c.CAMA = iCama.OrderBy(w => w.ID_CAMA).ToList();
                            }
                        }
                    }
                Ubicaciones = u;
            }
        }

        private void SeleccionaUbicacionArbol(Object obj)
        {
            try
            {
                var arbol = (TreeView)obj;
                var x = arbol.SelectedItem;
                var t = x.GetType();
                SelectedDelito = null;
                if (t.BaseType.Name.ToString().Equals("CAMA"))
                {
                    SelectedUbicacion = (CAMA)x;
                    UbicacionI = string.Format("{0}-{1}{2}-{3}", SelectedUbicacion.CELDA.SECTOR.EDIFICIO.DESCR, SelectedUbicacion.CELDA.SECTOR.DESCR, SelectedUbicacion.CELDA.ID_CELDA, SelectedUbicacion.ID_CAMA);
                    UbicacionI = UbicacionI.Replace(" ", string.Empty);
                    UbicacionD = UbicacionI;
                    UbicacionVisible = false;
                }
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONA_UBICACION);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ubicación en árbol", ex);
            }
        }
        #endregion
    }
}

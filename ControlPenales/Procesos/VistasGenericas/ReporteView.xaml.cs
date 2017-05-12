using ControlPenales.BiometricoServiceReference;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Office.Interop.Word;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TXTextControl;
using Wsq2Bmp;

namespace ControlPenales
{
    /// <summary>
    /// Interaction logic for EditorView.xaml
    /// </summary>
    public partial class ReporteView : MahApps.Metro.Controls.MetroWindow
    {
        private INGRESO ing;
        public ReporteView(INGRESO obj = null)
        {
            this.ing = obj;
            InitializeComponent();
            ReporteViewer.Load += ReportViewer_Load;
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            try 
            { 
            ReporteViewer.ShowExportButton = false;
            if (ing != null)
            {
                ing = (new cIngreso()).Obtener(ing.ID_CENTRO, ing.ID_ANIO, ing.ID_IMPUTADO, ing.ID_INGRESO);
                if (ing != null)
                {
                    var f = new cFicha();
                    //HEADER
                    var imp = ing.IMPUTADO;
                    f.Expediente = string.Format("{0}/{1}", imp.ID_ANIO, imp.ID_IMPUTADO);
                    f.FolioGobierno = string.Format("{0}/{1}", ing.ANIO_GOBIERNO, ing.FOLIO_GOBIERNO);
                    f.NoIngreso = ing.ID_INGRESO.ToString();
                    f.FecIngreso = ing.FEC_INGRESO_CERESO.HasValue ? ing.FEC_INGRESO_CERESO.Value.ToString("dd/MM/yyyy") : string.Empty;
                    f.HoraIngreso = ing.FEC_INGRESO_CERESO.HasValue ? ing.FEC_INGRESO_CERESO.Value.ToString("HH:mm:ss") : string.Empty;
                    f.Nombre = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(imp.NOMBRE) ? imp.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(imp.PATERNO) ? imp.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(imp.MATERNO) ? imp.MATERNO.Trim() : string.Empty);

                    f.Alias = string.Empty;
                    if (imp.ALIAS != null)
                    {
                        foreach (var i in imp.ALIAS)
                        {
                            if (!string.IsNullOrEmpty(f.Alias))
                                f.Alias = string.Format("{0}, ", f.Alias);
                            f.Alias = string.Format("{0} {1} {2} {3}", f.Alias, !string.IsNullOrEmpty(i.NOMBRE) ? i.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(i.PATERNO) ? i.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(i.MATERNO) ? i.MATERNO.Trim() : string.Empty);
                        }
                    }
                    f.Apodo = string.Empty;
                    if (f.Apodo != null)
                    {
                        foreach (var a in imp.APODO)
                        {
                            if (!string.IsNullOrEmpty(f.Apodo))
                                f.Apodo = string.Format("{0}, ", f.Apodo);
                            f.Apodo = string.Format("{0}{1}", f.Apodo, a.APODO1);
                        }
                    }
                    f.Estatus = ing.ESTATUS_ADMINISTRATIVO != null ? !string.IsNullOrEmpty(ing.ESTATUS_ADMINISTRATIVO.DESCR) ? ing.ESTATUS_ADMINISTRATIVO.DESCR.Trim() : string.Empty : string.Empty;
                    f.TipoIngreso = ing.TIPO_INGRESO != null ? !string.IsNullOrEmpty(ing.TIPO_INGRESO.DESCR) ? ing.TIPO_INGRESO.DESCR.Trim() : string.Empty : string.Empty;
                    f.NoOficioInternacion = ing.DOCINTERNACION_NUM_OFICIO;
                    f.Autoridad = ing.TIPO_AUTORIDAD_INTERNA != null ? !string.IsNullOrEmpty(ing.TIPO_AUTORIDAD_INTERNA.DESCR) ? ing.TIPO_AUTORIDAD_INTERNA.DESCR.Trim() : string.Empty : string.Empty;
                    f.ADisposicion = ing.TIPO_DISPOSICION != null ? !string.IsNullOrEmpty(ing.TIPO_DISPOSICION.DESCR) ? ing.TIPO_DISPOSICION.DESCR.Trim() : string.Empty : string.Empty;
                    //DATOS GENERALES
                    f.Padre = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(imp.NOMBRE_PADRE) ? imp.NOMBRE_PADRE.Trim() : string.Empty, !string.IsNullOrEmpty(imp.PATERNO_PADRE) ? imp.PATERNO_PADRE.Trim() : string.Empty, !string.IsNullOrEmpty(imp.MATERNO_PADRE) ? imp.MATERNO_PADRE.Trim() : string.Empty);
                    f.Madre = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(imp.NOMBRE_MADRE) ? imp.NOMBRE_MADRE.Trim() : string.Empty, !string.IsNullOrEmpty(imp.PATERNO_MADRE) ? imp.PATERNO_MADRE.Trim() : string.Empty, !string.IsNullOrEmpty(imp.MATERNO_MADRE) ? imp.MATERNO_MADRE.Trim() : string.Empty);
                    var originario = new List<MUNICIPIO>((new cMunicipio()).Obtener(imp.NACIMIENTO_ESTADO.Value, imp.NACIMIENTO_MUNICIPIO.Value));
                    f.Originario = string.Empty;
                    if (originario != null)
                    {
                        if (originario.Count > 0)
                        {
                            var o = originario[0];
                            f.Originario = string.Format("{0},{1},{2}", originario[0].MUNICIPIO1, originario[0].ENTIDAD.DESCR, originario[0].ENTIDAD.PAIS_NACIONALIDAD.PAIS);
                        }
                    }

                    f.Domicilio = ing.DOMICILIO_CALLE;
                    f.Numero = ing.DOMICILIO_NUM_EXT.HasValue ? ing.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty;
                    f.Colonia = ing.COLONIA != null ? !string.IsNullOrEmpty(ing.COLONIA.DESCR) ? ing.COLONIA.DESCR.Trim() : string.Empty : string.Empty;
                    f.Ciudad = ing.MUNICIPIO != null ? !string.IsNullOrEmpty(ing.MUNICIPIO.MUNICIPIO1) ? ing.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty;
                    f.FecNacimiento = imp.NACIMIENTO_FECHA.HasValue ? imp.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty;
                    var calcula = new Fechas();
                    f.Edad = calcula.CalculaEdad(imp.NACIMIENTO_FECHA).ToString();
                    f.Sexo = imp.SEXO == "M" ? "MASCULINO" : "FEMENINO";
                    //f.EdoCivil = imp.ESTADO_CIVIL != null ? !string.IsNullOrEmpty(imp.ESTADO_CIVIL.DESCR) ? imp.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty;
                    //f.Religion = imp.RELIGION != null ? !string.IsNullOrEmpty(imp.RELIGION.DESCR) ? imp.RELIGION.DESCR.Trim() : string.Empty : string.Empty;
                    //f.Ocupacion = imp.OCUPACION != null ? !string.IsNullOrEmpty(imp.OCUPACION.DESCR) ? imp.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                    //f.GradoMaxEstudio = imp.ESCOLARIDAD != null ? !string.IsNullOrEmpty(imp.ESCOLARIDAD.DESCR) ? imp.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                    f.EdoCivil = ing.ESTADO_CIVIL != null ? !string.IsNullOrEmpty(ing.ESTADO_CIVIL.DESCR) ? ing.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty;
                    f.Religion = ing.RELIGION != null ? !string.IsNullOrEmpty(ing.RELIGION.DESCR) ? ing.RELIGION.DESCR.Trim() : string.Empty : string.Empty;
                    f.Ocupacion = ing.OCUPACION != null ? !string.IsNullOrEmpty(ing.OCUPACION.DESCR) ? ing.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                    f.GradoMaxEstudio = ing.ESCOLARIDAD != null ? !string.IsNullOrEmpty(ing.ESCOLARIDAD.DESCR) ? ing.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                    f.LugarNAcExtranjero = imp.NACIMIENTO_LUGAR;





                    //MEDIA FILIACION
                    f.Estatura = ing.ESTATURA != null ? ing.ESTATURA.HasValue ? ing.ESTATURA.Value.ToString() : string.Empty : string.Empty;
                    f.Peso = ing.PESO != null ? ing.PESO.HasValue ? ing.PESO.Value.ToString() : string.Empty : string.Empty;

                    if (imp.IMPUTADO_FILIACION != null)
                    {
                        foreach (var x in imp.IMPUTADO_FILIACION)
                        {
                            switch (x.ID_MEDIA_FILIACION)
                            {
                                case 1://NARIZ RAIZ
                                    f.NarizRaiz = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 4://NARIZ ANCHO
                                    f.NarizAncho = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 9://CABELLO COLOR
                                    f.CabelloColor = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 11://CABELLO FORMA
                                    f.CabelloForma = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 16://OJOS COLOR
                                    f.OjosColor = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 17://OJOS FORMA
                                    f.OjosForma = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 18://OJOS TAMANIO
                                    f.OjosTamano = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 19://BOCA TAMANIO
                                    f.BocaTamano = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 20://BOCA COMISURAS
                                    f.BocaComisuras = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 21://LABIOS ESPESOR
                                    f.LabiosEspesor = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 24://MENTON TIPO
                                    f.MentonTipo = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 25://MENTON FORMA
                                    f.MentonForma = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 26://MENTON INCLINACION
                                    f.MentonInclinacion = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 27://FRENTE ALTURA
                                    f.FrenteAlta = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 28://FRENTE INCLINACION
                                    f.FrenteInclinada = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 29://FRENTE ANCHO
                                    f.FrenteAncha = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 30://COLOR DE PIEL
                                    f.ColorPiel = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 32://LABIO ALTURA
                                    f.LabiosAltura = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 33://LABIO PROMINENCIA
                                    f.LabiosPromedio = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                                case 39://COMPLEAXION
                                    f.Complexion = x.TIPO_FILIACION != null ? !string.IsNullOrEmpty(x.TIPO_FILIACION.DESCR) ? x.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                    break;
                            }
                        }
                    }
                    var ficha = new List<cFicha>();
                    ficha.Add(f);
                    //CAUSAS PENALES
                    var causasPenales = new List<cCausasPenales>();
                    if (ing.CAUSA_PENAL != null)
                    {
                        foreach (var x in ing.CAUSA_PENAL)
                        {
                            var cp = new cCausasPenales();
                            cp.CausaPenal = string.Format("{0}/{1}{2}", x.CP_ANIO, x.CP_FOLIO, string.IsNullOrEmpty(x.CP_BIS) ? string.Empty : "-" + x.CP_BIS);
                            cp.Abreviatura = string.Empty;
                            if (x.JUZGADO != null)
                                cp.Juzgado = x.JUZGADO.DESCR;
                            cp.Consignado = x.AP_FEC_CONSIGNACION != null ? x.AP_FEC_CONSIGNACION.Value.ToString("dd/MM/yyyy") : string.Empty;
                            cp.Delitos = string.Empty;//"* HOMICIDIO SIMPLE \n* PRIVACION DE LIBERTAR PERSONAL \n* DELITOS EN MATERIA DE INHUMACION Y EXHUMACION \n* RESPONSABILIDAD PROFESIONAL Y TECNICA";
                            if (x.CAUSA_PENAL_DELITO != null)
                            {
                                foreach (var d in x.CAUSA_PENAL_DELITO)
                                {
                                    if (string.IsNullOrEmpty(cp.Delitos))
                                        cp.Delitos = string.Format("{0}\n", d.TIPO_DELITO != null ? !string.IsNullOrEmpty(d.TIPO_DELITO.DESCR) ? d.TIPO_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                                    cp.Delitos = string.Format("{0}* {1}", cp.Delitos, d.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(d.MODALIDAD_DELITO.DESCR) ? d.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                                }
                            }
                            causasPenales.Add(cp);
                        }
                    }

                    //SENIAS PARTICULARES
                    var seniasParticulares = new List<cSeniasParticulares>();
                    if (imp.SENAS_PARTICULARES != null)
                    {
                        foreach (var x in imp.SENAS_PARTICULARES)
                        {
                            var sp = new cSeniasParticulares();
                            sp.Significado = x.SIGNIFICADO;
                            seniasParticulares.Add(sp);
                        }
                    }
                    //BIOMETRICO (FOTOS)
                    var imputadoBiometricos = new List<cBiometricos>();
                    var ib = new cBiometricos();
                    ib.FotoIzquerda = ib.FotoCentro = ib.FotoDerecha = new Imagenes().getImagenPerson();//System.IO.File.ReadAllBytes("../../Imagen/placeholder_person.gif");
                    if (ing.INGRESO_BIOMETRICO.Where(w => w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) != null)
                    {
                        foreach (var b in ing.INGRESO_BIOMETRICO)
                        {
                            switch (b.ID_TIPO_BIOMETRICO)
                            {
                                case (int)enumTipoBiometrico.FOTO_IZQ_SEGUIMIENTO:
                                    ib.FotoIzquerda = b.BIOMETRICO;
                                    break;
                                case (int)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO:
                                    ib.FotoCentro = b.BIOMETRICO;
                                    break;
                                case (int)enumTipoBiometrico.FOTO_DER_SEGUIMIENTO:
                                    ib.FotoDerecha = b.BIOMETRICO;
                                    break;
                            }
                        }
                    }


                    //HUELLAS
                    if (imp.IMPUTADO_BIOMETRICO != null)
                    {
                        foreach (var b in imp.IMPUTADO_BIOMETRICO.Where(w => w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ))
                        {
                            if (b.BIOMETRICO.Length > 0)
                            {
                                switch (b.ID_TIPO_BIOMETRICO)
                                {
                                    case (int)enumTipoBiometrico.PULGAR_IZQUIERDO:
                                        ib.PulgarIzquierda = getHuella(b.BIOMETRICO);
                                        break;
                                    case (int)enumTipoBiometrico.INDICE_IZQUIERDO:
                                        ib.IndiceIzquierda = getHuella(b.BIOMETRICO);
                                        break;
                                    case (int)enumTipoBiometrico.MEDIO_IZQUIERDO:
                                        ib.MedioIzquierda = getHuella(b.BIOMETRICO);
                                        break;
                                    case (int)enumTipoBiometrico.ANULAR_IZQUIERDO:
                                        ib.AnularIzquierda = getHuella(b.BIOMETRICO);
                                        break;
                                    case (int)enumTipoBiometrico.MENIQUE_IZQUIERDO:
                                        ib.MeniqueIzquierda = getHuella(b.BIOMETRICO);
                                        break;
                                    case (int)enumTipoBiometrico.PULGAR_DERECHO:
                                        ib.PulgarDerecha = getHuella(b.BIOMETRICO);
                                        break;
                                    case (int)enumTipoBiometrico.INDICE_DERECHO:
                                        ib.IndiceDerecha = getHuella(b.BIOMETRICO);
                                        break;
                                    case (int)enumTipoBiometrico.MEDIO_DERECHO:
                                        ib.MedioDerecha = getHuella(b.BIOMETRICO);
                                        break;
                                    case (int)enumTipoBiometrico.ANULAR_DERECHO:
                                        ib.AnularDerecha = getHuella(b.BIOMETRICO);
                                        break;
                                    case (int)enumTipoBiometrico.MENIQUE_DERECHO:
                                        ib.MeniqueDerecha = getHuella(b.BIOMETRICO);
                                        break;
                                }
                            }
                        }
                    }

                    //GENERALES IDIOMA
                    var gi = new List<cIdiomaGenerales>();
                    gi.Add(
                        new cIdiomaGenerales()
                        {
                            Nacionalidad = ing.IMPUTADO != null ? ing.IMPUTADO.PAIS_NACIONALIDAD != null ? !string.IsNullOrEmpty(ing.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD) ? ing.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD.Trim() : string.Empty : string.Empty : string.Empty,
                            Idioma = ing.IMPUTADO != null ? ing.IMPUTADO.IDIOMA != null ? !string.IsNullOrEmpty(ing.IMPUTADO.IDIOMA.DESCR) ? ing.IMPUTADO.IDIOMA.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                            Etnia = ing.IMPUTADO != null ? ing.IMPUTADO.ETNIA != null ? !string.IsNullOrEmpty(ing.IMPUTADO.ETNIA.DESCR) ? ing.IMPUTADO.ETNIA.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                            Dialecto = ing.IMPUTADO != null ? ing.IMPUTADO.DIALECTO != null ? !string.IsNullOrEmpty(ing.IMPUTADO.DIALECTO.DESCR) ? ing.IMPUTADO.DIALECTO.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                            RequiereTraductor = ing.IMPUTADO != null ? !string.IsNullOrEmpty(ing.IMPUTADO.TRADUCTOR) ? ing.IMPUTADO.TRADUCTOR == "S" ? "SI" : "NO" : "NO" : "NO"
                        });


                    //ib.PulgarIzquierda = System.IO.File.ReadAllBytes(@"C:\Git\seguridadpublica\ControlPenales\Imagen\huella.jpg");
                    //ib.IndiceIzquierda = System.IO.File.ReadAllBytes(@"C:\Git\seguridadpublica\ControlPenales\Imagen\huella.jpg");
                    //ib.MedioIzquierda = ib.IndiceIzquierda;
                    //ib.AnularIzquierda = ib.IndiceIzquierda;
                    //ib.MeniqueIzquierda = ib.IndiceIzquierda;
                    //ib.PulgarDerecha = ib.IndiceIzquierda;
                    //ib.IndiceDerecha = ib.IndiceIzquierda;
                    //ib.MedioDerecha = ib.IndiceIzquierda;
                    //ib.AnularDerecha = ib.IndiceIzquierda;
                    //ib.MeniqueDerecha = ib.IndiceIzquierda;
                    imputadoBiometricos.Add(ib);

                    var centro = new cCentro().Obtener(GlobalVar.gCentro).SingleOrDefault();
                    var reporte = new List<cReporte>();
                    reporte.Add(new cReporte()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1,
                        Encabezado2 = Parametro.ENCABEZADO2,
                        Encabezado3 = centro.DESCR.Trim().ToUpper(),
                        Encabezado4 = "Ficha",
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2,
                    });

                    var firmas = new List<cFirmas>();
                    firmas.Add(new cFirmas()
                    {
                        NombreUsuario = StaticSourcesViewModel.UsuarioLogin.Nombre,
                        NombreSubdirector = Parametro.SUBDIRECTOR_CENTRO.Trim()
                    });

                    ReporteViewer.LocalReport.ReportPath = "Reportes/rFicha.rdlc";
                    ReporteViewer.LocalReport.DataSources.Clear();
                    //FICHA
                    Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds.Name = "DataSet1";
                    rds.Value = ficha;
                    ReporteViewer.LocalReport.DataSources.Add(rds);
                    //CAUSA PENAL
                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = causasPenales;
                    ReporteViewer.LocalReport.DataSources.Add(rds2);
                    //SENIAS PARTICULARES
                    Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds3.Name = "DataSet3";
                    rds3.Value = seniasParticulares;
                    ReporteViewer.LocalReport.DataSources.Add(rds3);
                    //BIOMETRICO
                    Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds4.Name = "DataSet4";
                    rds4.Value = imputadoBiometricos;
                    ReporteViewer.LocalReport.DataSources.Add(rds4);
                    ReporteViewer.RefreshReport();

                    //IDIOMA
                    Microsoft.Reporting.WinForms.ReportDataSource rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds5.Name = "DataSet5";
                    rds5.Value = gi;
                    ReporteViewer.LocalReport.DataSources.Add(rds5);

                    //Reporte
                    Microsoft.Reporting.WinForms.ReportDataSource rds6 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds6.Name = "DataSet6";
                    rds6.Value = reporte;
                    ReporteViewer.LocalReport.DataSources.Add(rds6);

                    //Firmas
                    Microsoft.Reporting.WinForms.ReportDataSource rds7 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds7.Name = "DataSet7";
                    rds7.Value = firmas;
                    ReporteViewer.LocalReport.DataSources.Add(rds7);

                    ReporteViewer.RefreshReport();
                }
            }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte de la ficha.", ex);
            }
        }

        private byte[] getHuella(byte[] wsq)
        {
            byte[] byteArray = new byte[0];
            MemoryStream ms = new MemoryStream(wsq);
            byte[] data = new byte[ms.Length];
            ms.Read(data, 0, data.Length);
            WsqDecoder decoder = new WsqDecoder();
            Bitmap bmp = decoder.Decode(data);
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();
                byteArray = stream.ToArray();
            }
            return byteArray;
        }
    }
}

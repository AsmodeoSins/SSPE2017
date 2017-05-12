using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class cFicha
    {
        private string expediente;

        public string Expediente
        {
            get { return expediente; }
            set { expediente = value; }
        }
        private string folioGobierno;

        public string FolioGobierno
        {
            get { return folioGobierno; }
            set { folioGobierno = value; }
        }
        private string noIngreso;

        public string NoIngreso
        {
            get { return noIngreso; }
            set { noIngreso = value; }
        }
        private string fecIngreso;

        public string FecIngreso
        {
            get { return fecIngreso; }
            set { fecIngreso = value; }
        }
        private string horaIngreso;

        public string HoraIngreso
        {
            get { return horaIngreso; }
            set { horaIngreso = value; }
        }
        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        private string alias;

        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }
        private string apodo;

        public string Apodo
        {
            get { return apodo; }
            set { apodo = value; }
        }
        private string estatus;

        public string Estatus
        {
            get { return estatus; }
            set { estatus = value; }
        }
        private string tipoIngreso;

        public string TipoIngreso
        {
            get { return tipoIngreso; }
            set { tipoIngreso = value; }
        }
        private string noOficioInternacion;

        public string NoOficioInternacion
        {
            get { return noOficioInternacion; }
            set { noOficioInternacion = value; }
        }
        private string autoridad;

        public string Autoridad
        {
            get { return autoridad; }
            set { autoridad = value; }
        }
        private string aDisposicion;

        public string ADisposicion
        {
            get { return aDisposicion; }
            set { aDisposicion = value; }
        }
        //DATOS GENERALES
        private string padre;

        public string Padre
        {
            get { return padre; }
            set { padre = value; }
        }
        private string madre;

        public string Madre
        {
            get { return madre; }
            set { madre = value; }
        }
        private string originario;

        public string Originario
        {
            get { return originario; }
            set { originario = value; }
        }
        private string domicilio;

        public string Domicilio
        {
            get { return domicilio; }
            set { domicilio = value; }
        }
        private string numero;

        public string Numero
        {
            get { return numero; }
            set { numero = value; }
        }
        private string colonia;

        public string Colonia
        {
            get { return colonia; }
            set { colonia = value; }
        }
        private string ciudad;

        public string Ciudad
        {
            get { return ciudad; }
            set { ciudad = value; }
        }
        private string fecNacimiento;

        public string FecNacimiento
        {
            get { return fecNacimiento; }
            set { fecNacimiento = value; }
        }
        private string edad;

        public string Edad
        {
            get { return edad; }
            set { edad = value; }
        }
        private string sexo;

        public string Sexo
        {
            get { return sexo; }
            set { sexo = value; }
        }
        private string edoCivil;

        public string EdoCivil
        {
            get { return edoCivil; }
            set { edoCivil = value; }
        }
        private string religion;

        public string Religion
        {
            get { return religion; }
            set { religion = value; }
        }
        private string ocupacion;

        public string Ocupacion
        {
            get { return ocupacion; }
            set { ocupacion = value; }
        }
        private string gradoMaxEstudio;

        public string GradoMaxEstudio
        {
            get { return gradoMaxEstudio; }
            set { gradoMaxEstudio = value; }
        }
        private string lugarNAcExtranjero;

        public string LugarNAcExtranjero
        {
            get { return lugarNAcExtranjero; }
            set { lugarNAcExtranjero = value; }
        }
        //MEDIA FILIACION
        private string estatura;

        public string Estatura
        {
            get { return estatura; }
            set { estatura = value; }
        }
        private string peso;

        public string Peso
        {
            get { return peso; }
            set { peso = value; }
        }
        private string complexion;

        public string Complexion
        {
            get { return complexion; }
            set { complexion = value; }
        }
        private string colorPiel;

        public string ColorPiel
        {
            get { return colorPiel; }
            set { colorPiel = value; }
        }
        private string cabelloColor;

        public string CabelloColor
        {
            get { return cabelloColor; }
            set { cabelloColor = value; }
        }
        private string cabelloForma;

        public string CabelloForma
        {
            get { return cabelloForma; }
            set { cabelloForma = value; }
        }
        private string frenteAlta;

        public string FrenteAlta
        {
            get { return frenteAlta; }
            set { frenteAlta = value; }
        }
        private string frenteInclinada;

        public string FrenteInclinada
        {
            get { return frenteInclinada; }
            set { frenteInclinada = value; }
        }
        private string frenteAncha;

        public string FrenteAncha
        {
            get { return frenteAncha; }
            set { frenteAncha = value; }
        }
        private string ojosColor;

        public string OjosColor
        {
            get { return ojosColor; }
            set { ojosColor = value; }
        }
        private string ojosForma;

        public string OjosForma
        {
            get { return ojosForma; }
            set { ojosForma = value; }
        }
        private string ojosTamano;

        public string OjosTamano
        {
            get { return ojosTamano; }
            set { ojosTamano = value; }
        }
        private string narizRaiz;

        public string NarizRaiz
        {
            get { return narizRaiz; }
            set { narizRaiz = value; }
        }
        private string narizAncho;

        public string NarizAncho
        {
            get { return narizAncho; }
            set { narizAncho = value; }
        }
        private string bocaTamano;

        public string BocaTamano
        {
            get { return bocaTamano; }
            set { bocaTamano = value; }
        }
        private string bocaComisuras;

        public string BocaComisuras
        {
            get { return bocaComisuras; }
            set { bocaComisuras = value; }
        }
        private string labiosEspesor;

        public string LabiosEspesor
        {
            get { return labiosEspesor; }
            set { labiosEspesor = value; }
        }
        private string labiosAltura;

        public string LabiosAltura
        {
            get { return labiosAltura; }
            set { labiosAltura = value; }
        }
        private string labiosPromedio;

        public string LabiosPromedio
        {
            get { return labiosPromedio; }
            set { labiosPromedio = value; }
        }
        private string mentonTipo;

        public string MentonTipo
        {
            get { return mentonTipo; }
            set { mentonTipo = value; }
        }
        private string mentonForma;

        public string MentonForma
        {
            get { return mentonForma; }
            set { mentonForma = value; }
        }
        private string mentonInclinacion;

        public string MentonInclinacion
        {
            get { return mentonInclinacion; }
            set { mentonInclinacion = value; }
        }
    }
}
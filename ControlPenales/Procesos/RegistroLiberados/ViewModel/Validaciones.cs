using System.ComponentModel;

namespace ControlPenales
{
    partial class RegistroLiberadosViewModel
    {
        void ValidacionesLiberado()
        {
            base.ClearRules();

            #region Medidas Judiciales
            //base.AddRule(() => RNUC, () => !string.IsNullOrEmpty(RNUC), "NUC ES REQUERIDO!");
            //base.AddRule(() => RCP, () => !string.IsNullOrEmpty(RCP), "CAUSA PENAL ES REQUERIDA!");

            //base.AddRule(() => DDelitos, () => !string.IsNullOrEmpty(DDelitos), "DELITO ES REQUERIDO!");

            //base.AddRule(() => MMedidaJudicial, () => !string.IsNullOrEmpty(MMedidaJudicial), "MEDIDA JUDICIAL ES REQUERIDA!");
            //base.AddRule(() => MPeridiocidad, () => !string.IsNullOrEmpty(MPeridiocidad), "PERIDIOCIDAD ES REQUERIDA!");
            ////base.AddRule(() => MApartirV, () => MApartirV, EMensajeErrorFecha);
            //base.AddRule(() => MApartir, () => MApartir.HasValue, "FECHA A PARTIR ES REQUERIDA!");
            
            //base.AddRule(() => MDuracion, () => !string.IsNullOrEmpty(MDuracion), "DURACIÓN ES REQUERIDA!");

            //if (!DPublico.Value)
            //{ 
            //    base.AddRule(() => DNombreDefensor, () => !string.IsNullOrEmpty(DNombreDefensor), "NOMBRE DE DEFENSOR ES REQUERIDO!");
            //    base.AddRule(() => DTelefonoDefensor, () => !string.IsNullOrEmpty(DTelefonoDefensor), "TELÉFONO DE DEFENSOR ES REQUERIDO!");
            //}

            //base.AddRule(() => ANombre, () => !string.IsNullOrEmpty(ANombre), "NOMBRE ES REQUERIDO!");
            //base.AddRule(() => ARelacion, () => ARelacion != -1, "RELACIÓN ES REQUERIDA!");
            //base.AddRule(() => ATiempoConocerlo, () => !string.IsNullOrEmpty(ATiempoConocerlo), "TIPO DE CONOCERLO ES REQUERIDO!");
            //base.AddRule(() => ADocmicilio, () => !string.IsNullOrEmpty(ADocmicilio), "DOMICILIO ES REQUERIDO!");
            //base.AddRule(() => ATelefonoMovil, () => !string.IsNullOrEmpty(ATelefonoMovil), "TELÉFONO MÓVIL ES REQUERIDO!");
            //base.AddRule(() => ATelefonoFijo, () => !string.IsNullOrEmpty(ATelefonoFijo), "TELÉFONO FIJO ES REQUERIDO!");

            //OnPropertyChanged("RNUC");
            //OnPropertyChanged("RCP");
            //OnPropertyChanged("DDelitos");
            //OnPropertyChanged("MMedidaJudicial");
            //OnPropertyChanged("MPeridiocidad");
            //OnPropertyChanged("MApartirV");
            //OnPropertyChanged("MDuracion");
            //OnPropertyChanged("DNombreDefensor");
            //OnPropertyChanged("DTelefonoDefensor");
            //OnPropertyChanged("ANombre");
            //OnPropertyChanged("ARelacion");
            //OnPropertyChanged("ATiempoConocerlo");
            //OnPropertyChanged("ADocmicilio");
            //OnPropertyChanged("ATelefonoMovil");
            //OnPropertyChanged("ATelefonoFijo");
            #endregion

            #region Proceso en Libertad
            //base.AddRule(() => PNUC, () => !string.IsNullOrEmpty(PNUC), "NUC ES REQUERIDO!");
            //OnPropertyChanged("PNUC");
            #endregion

            #region DATOS_GENERALES
            base.AddRule(() => SelectSexo, () => (SelectSexo == "F" || SelectSexo == "M"), "SEXO ES REQUERIDO!");
            base.AddRule(() => SelectEstadoCivil, () => SelectEstadoCivil.Value != -1, "ESTADO CIVIL ES REQUERIDO!");
            base.AddRule(() => SelectOcupacion, () => SelectOcupacion.HasValue ? SelectOcupacion.Value >= 1 : false, "OCUPACION ES REQUERIDA!");
            base.AddRule(() => SelectEscolaridad, () => SelectEscolaridad.HasValue ? SelectEscolaridad >= 1 : false, "ESCOLARIDAD ES REQUERIDA!");
            base.AddRule(() => SelectNacionalidad, () => SelectNacionalidad.HasValue ? SelectNacionalidad.Value >= 1 : false, "NACIONALIDAD ES REQUERIDA!");
            base.AddRule(() => SelectReligion, () => SelectReligion.HasValue ? SelectReligion.Value >= 1 : false, "RELIGION ES REQUERIDA!");
            base.AddRule(() => SelectEtnia, () => SelectEtnia.HasValue ? SelectEtnia.Value >= 1 : false, "ETNIA ES REQUERIDA!");
            base.AddRule(() => SelectedIdioma, () => SelectedIdioma.HasValue ? SelectedIdioma.Value >= 1 : false, "IDIOMA ES REQUERIDO!");
            base.AddRule(() => SelectedDialecto, () => SelectedDialecto.HasValue ? SelectedDialecto.Value >= 1 : false, "DIALECTO ES REQUERIDO!");
            #endregion
          
            #region DOMICILIO
            base.AddRule(() => SelectPais, () => SelectPais.Value != -1, "PAÍS ES REQUERIDO!");
            base.AddRule(() => SelectEntidad, () => SelectEntidad.Value != -1, "ENTIDAD ES REQUERIDA!");
            base.AddRule(() => SelectMunicipio, () => SelectMunicipio.Value != -1, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => SelectColonia, () => SelectColonia.Value != -1, "COLONIA ES REQUERIDA!");
            base.AddRule(() => TextCalle, () => !string.IsNullOrEmpty(TextCalle), "CALLE ES REQUERIDA!");
            base.AddRule(() => TextNumeroExterior, () => TextNumeroExterior.HasValue ? TextNumeroExterior.Value > 0 : false, "NUMERO EXTERIOR ES REQUERIDO!");
            base.AddRule(() => AniosEstado, () => !string.IsNullOrEmpty(AniosEstado), "AÑOS EN EL ESTADO ES REQUERIDO!");
            base.AddRule(() => MesesEstado, () => !string.IsNullOrEmpty(MesesEstado), "MESES EN EL ESTADO ES REQUERIDO!");
            base.AddRule(() => TextTelefono, () => TextTelefono != null ? TextTelefono.Length >= 14 : false, "TELÉFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
            base.AddRule(() => TextCodigoPostal, () => TextCodigoPostal.HasValue ? TextCodigoPostal.Value >= 1 : false, "CÓDIGO POSTAL ES REQUERIDO!");
            #endregion
          
            #region NACIMIENTO
            base.AddRule(() => SelectPaisNacimiento, () => SelectPaisNacimiento.Value != -1, "PAÍS DE NACIMIENTO ES REQUERIDO!");
            base.AddRule(() => SelectEntidadNacimiento, () => SelectEntidadNacimiento.Value != - 1, "ENTIDAD DE NACIMIENTO ES REQUERIDO!");
            base.AddRule(() => SelectMunicipioNacimiento, () => SelectMunicipioNacimiento.Value != -1, "MUNICIPIO DE NACIMIENTO ES REQUERIDO!");
            base.AddRule(() => TextFechaNacimiento, () => TextFechaNacimiento != null ? new Fechas().CalculaEdad(TextFechaNacimiento) >= 18 ? true : false : false, "FECHA DE NACIMIENTO ES REQUERIDA,LA EDAD DEBE SER MAYOR O IGUAL A 18 AÑOS!");
            #endregion
          
            #region PADRE
            base.AddRule(() => TextPadrePaterno, () => !string.IsNullOrEmpty(TextPadrePaterno), "APELLIDO PATERNO DEL PADRE ES REQUERIDO!");
            base.AddRule(() => TextPadreMaterno, () => !string.IsNullOrEmpty(TextPadreMaterno), "APELLIDO MATERNO DEL PADRE ES REQUERIDO!");
            base.AddRule(() => TextPadreNombre, () => !string.IsNullOrEmpty(TextPadreNombre), "NOMBRE(S) DEL PADRE ES REQUERIDO!");
            ValidarDatosPadre();
            #endregion
           
            #region MADRE
            base.AddRule(() => TextMadrePaterno, () => !string.IsNullOrEmpty(TextMadrePaterno), "APELLIDO PATERNO DE LA MADRE ES REQUERIDO!");
            base.AddRule(() => TextMadreMaterno, () => !string.IsNullOrEmpty(TextMadreMaterno), "APELLIDO MATERNO DE LA MADRE ES REQUERIDO!");
            base.AddRule(() => TextMadreNombre, () => !string.IsNullOrEmpty(TextMadreNombre), "NOMBRE(S) DE LA MADRE ES REQUERIDO!");
            ValidarDatosMadre();
            #endregion

            OnPropertyChanged("SelectSexo");
            OnPropertyChanged("SelectEstadoCivil");
            OnPropertyChanged("SelectOcupacion");
            OnPropertyChanged("SelectEscolaridad");
            OnPropertyChanged("SelectNacionalidad");
            OnPropertyChanged("SelectReligion");
            OnPropertyChanged("SelectEtnia");
            OnPropertyChanged("SelectedIdioma");
            OnPropertyChanged("SelectedDialecto");
            OnPropertyChanged("SelectPais");
            OnPropertyChanged("SelectEntidad");
            OnPropertyChanged("SelectMunicipio");
            OnPropertyChanged("SelectColonia");
            OnPropertyChanged("TextCalle");
            OnPropertyChanged("TextNumeroExterior");
            OnPropertyChanged("AniosEstado");
            OnPropertyChanged("MesesEstado");
            OnPropertyChanged("TextTelefono");
            OnPropertyChanged("TextCodigoPostal");
            OnPropertyChanged("SelectPaisNacimiento");
            OnPropertyChanged("SelectEntidadNacimiento");
            OnPropertyChanged("SelectMunicipioNacimiento");
            OnPropertyChanged("TextFechaNacimiento");
            OnPropertyChanged("TextPadrePaterno");
            OnPropertyChanged("TextPadreMaterno");
            OnPropertyChanged("TextPadreNombre");
            OnPropertyChanged("TextMadrePaterno");
            OnPropertyChanged("TextMadreMaterno");
            OnPropertyChanged("TextMadreNombre");
        }

        void ValidarDatosPadre()
        {
            if (CheckPadreFinado)
            {
                MismoDomicilioMadreEnabled = MismoDomicilioMadre = false;
                base.RemoveRule("SelectPaisDomicilioPadre");
                base.RemoveRule("SelectEntidadDomicilioPadre");
                base.RemoveRule("SelectMunicipioDomicilioPadre");
                base.RemoveRule("SelectColoniaDomicilioPadre");
                base.RemoveRule("TextCalleDomicilioPadre");
                base.RemoveRule("TextNumeroExteriorDomicilioPadre");
                base.RemoveRule("TextCodigoPostalDomicilioPadre");
            }
            else
            {
                MismoDomicilioMadreEnabled = true;
                base.RemoveRule("SelectPaisDomicilioPadre");
                base.RemoveRule("SelectEntidadDomicilioPadre");
                base.RemoveRule("SelectMunicipioDomicilioPadre");
                base.RemoveRule("SelectColoniaDomicilioPadre");
                base.RemoveRule("TextCalleDomicilioPadre");
                base.RemoveRule("TextNumeroExteriorDomicilioPadre");
                base.RemoveRule("TextCodigoPostalDomicilioPadre");
                base.AddRule(() => SelectPaisDomicilioPadre, () => SelectPaisDomicilioPadre.Value != -1, "PAÍS ES REQUERIDO!");
                base.AddRule(() => SelectEntidadDomicilioPadre, () => SelectEntidadDomicilioPadre.Value != -1, "ENTIDAD ES REQUERIDA!");
                base.AddRule(() => SelectMunicipioDomicilioPadre, () => SelectMunicipioDomicilioPadre.Value != -1, "MUNICIPIO ES REQUERIDO!");
                base.AddRule(() => SelectColoniaDomicilioPadre, () => SelectColoniaDomicilioPadre.Value != -1, "COLONIA ES REQUERIDA!");
                base.AddRule(() => TextCalleDomicilioPadre, () => !string.IsNullOrEmpty(TextCalleDomicilioPadre), "CALLE ES REQUERIDA!");
                base.AddRule(() => TextNumeroExteriorDomicilioPadre, () => TextNumeroExteriorDomicilioPadre.HasValue, "NUMERO EXTERIOR ES REQUERIDO!");
                base.AddRule(() => TextCodigoPostalDomicilioPadre, () => TextCodigoPostalDomicilioPadre.HasValue, "CÓDIGO POSTAL ES REQUERIDO!");
            }
            OnPropertyChanged("SelectPaisDomicilioPadre");
            OnPropertyChanged("SelectEntidadDomicilioPadre");
            OnPropertyChanged("SelectMunicipioDomicilioPadre");
            OnPropertyChanged("SelectColoniaDomicilioPadre");
            OnPropertyChanged("TextCalleDomicilioPadre");
            OnPropertyChanged("TextNumeroExteriorDomicilioPadre");
            OnPropertyChanged("TextCodigoPostalDomicilioPadre");
        }

        void ValidarDatosMadre()
        {
            if (CheckMadreFinado || MismoDomicilioMadre)
            {
                base.RemoveRule("SelectPaisDomicilioMadre");
                base.RemoveRule("SelectEntidadDomicilioMadre");
                base.RemoveRule("SelectMunicipioDomicilioMadre");
                base.RemoveRule("SelectColoniaDomicilioMadre");
                base.RemoveRule("TextCalleDomicilioMadre");
                base.RemoveRule("TextNumeroExteriorDomicilioMadre");
                base.RemoveRule("TextCodigoPostalDomicilioMadre");
            }
            else
            {
                base.RemoveRule("SelectPaisDomicilioMadre");
                base.RemoveRule("SelectEntidadDomicilioMadre");
                base.RemoveRule("SelectMunicipioDomicilioMadre");
                base.RemoveRule("SelectColoniaDomicilioMadre");
                base.RemoveRule("TextCalleDomicilioMadre");
                base.RemoveRule("TextNumeroExteriorDomicilioMadre");
                base.RemoveRule("TextCodigoPostalDomicilioMadre");
                base.AddRule(() => SelectPaisDomicilioMadre, () => SelectPaisDomicilioMadre.Value != -1, "PAÍS ES REQUERIDO!");
                base.AddRule(() => SelectEntidadDomicilioMadre, () => SelectEntidadDomicilioMadre.Value != -1, "ENTIDAD ES REQUERIDA!");
                base.AddRule(() => SelectMunicipioDomicilioMadre, () => SelectMunicipioDomicilioMadre.Value != -1, "MUNICIPIO ES REQUERIDO!");
                base.AddRule(() => SelectColoniaDomicilioMadre, () => SelectColoniaDomicilioMadre.Value != null, "COLONIA ES REQUERIDA!");
                base.AddRule(() => TextCalleDomicilioMadre, () => !string.IsNullOrEmpty(TextCalleDomicilioMadre), "CALLE ES REQUERIDA!");
                base.AddRule(() => TextNumeroExteriorDomicilioMadre, () => TextNumeroExteriorDomicilioMadre.HasValue, "NUMERO EXTERIOR ES REQUERIDO!");
                base.AddRule(() => TextCodigoPostalDomicilioMadre, () => TextCodigoPostalDomicilioMadre.HasValue, "CÓDIGO POSTAL ES REQUERIDO!");
            }

            OnPropertyChanged("SelectPaisDomicilioMadre");
            OnPropertyChanged("SelectEntidadDomicilioMadre");
            OnPropertyChanged("SelectMunicipioDomicilioMadre");
            OnPropertyChanged("SelectColoniaDomicilioMadre");
            OnPropertyChanged("TextCalleDomicilioMadre");
            OnPropertyChanged("TextNumeroExteriorDomicilioMadre");
            OnPropertyChanged("TextCodigoPostalDomicilioMadre");
        }

        void ValidarDefensor()
        {
            if (!DPublico.Value)
            {
                DefensorEnabled = true;
                base.AddRule(() => DNombreDefensor, () => !string.IsNullOrEmpty(DNombreDefensor), "NOMBRE DE DEFENSOR ES REQUERIDO!");
                base.AddRule(() => DTelefonoDefensor, () => !string.IsNullOrEmpty(DTelefonoDefensor), "TELÉFONO DE DEFENSOR ES REQUERIDO!");
            }
            else
            {
                DefensorEnabled = false;
                DNombreDefensor = DTelefonoDefensor = string.Empty;
                base.RemoveRule("DNombreDefensor");
                base.RemoveRule("DTelefonoDefensor");
            }
            OnPropertyChanged("DNombreDefensor");
            OnPropertyChanged("DTelefonoDefensor");
        }

        void ValidarAlias()
        {
            base.ClearRules();
            base.AddRule(() => NombreAlias, () => !string.IsNullOrEmpty(NombreAlias), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ES REQUERIDO!");
            base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ES REQUERIDO!");
            OnPropertyChanged("NombreAlias");
            OnPropertyChanged("PaternoAlias");
            OnPropertyChanged("MaternoAlias");
        }

        void ValidarApodo() 
        {
            base.ClearRules();
            base.AddRule(() => Apodo, () => !string.IsNullOrEmpty(Apodo), "APODO ES REQUERIDO!");
            OnPropertyChanged("Apodo");
        }

        #region Proceso Libertad
        private void ValidarProcesoLibertad()
        {
            base.ClearRules();
            //base.AddRule(() => PNUC, () => !string.IsNullOrEmpty(PNUC), "NUC ES REQUERIDO!");
            base.AddRule(() => PFecha, () => PFecha != null, "NUC ES REQUERIDO!");
            base.AddRule(() => PTipo, () => PTipo != -1, "TIPO ES REQUERIDO!");

            //OnPropertyChanged("PNUC");
            OnPropertyChanged("PFecha");
            OnPropertyChanged("PTipo");
        }
        #endregion

        #region Medida
        void ValidarMedida()
        {
            base.ClearRules();
            base.AddRule(() => MDocumento, () => MDocumento != -1, "DOCUMENTO ES REQUERIDO!");
            base.AddRule(() => MMedidaTipo, () => MMedidaTipo != -1, "TIPO DE MEDIDA ES REQUERIDA!");
            base.AddRule(() => MMedida, () => MMedida != -1, "MEDIDA ES REQUERIDA!");
            base.AddRule(() => MFechaInicio, () => MFechaInicio.HasValue, "MEDIDA ES REQUERIDA!");
            base.AddRule(() => MFechaFin, () => MFechaFin.HasValue, "MEDIDA ES REQUERIDA!");
            OnPropertyChanged("MDocumento");
            OnPropertyChanged("MMedidaTipo");
            OnPropertyChanged("MMedida");
            OnPropertyChanged("MFechaInicio");
            OnPropertyChanged("MFechaFin");
        }
        #endregion 

        #region Medida Estatus
        void ValidarMedidaEstatus() 
        {
            base.ClearRules();
            base.AddRule(() => MLEstatus, () => MLEstatus != -1, "ESTATUS ES REQUERIDO!");
            base.AddRule(() => MLMotivo, () => MLMotivo != -1, "MOTIVO ES REQUERIDO!");
            base.AddRule(() => MLFecha, () => MLFecha.HasValue, "FECHA ES REQUERIDO!");
            OnPropertyChanged("MLEstatus");
            OnPropertyChanged("MLMotivo");
            OnPropertyChanged("MLFecha");
        }
        #endregion

        #region Medida Persona
        void ValidarMedidaPersona()
        {
            base.ClearRules();
            base.AddRule(() => MPNombre, () => !string.IsNullOrEmpty(MPNombre), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => MPPaterno, () => !string.IsNullOrEmpty(MPPaterno), "APELLIDO PATERNO ES REQUERIDO!");
            base.AddRule(() => MPMaterno, () => !string.IsNullOrEmpty(MPMaterno), "APELLIDO MATERNO ES REQUERIDO!");
            //base.AddRule(() => MPAlias, () => !string.IsNullOrEmpty(MPAlias), "ALIAS ES REQUERIDO!");
            base.AddRule(() => MPRelacion, () => MPRelacion != -1, "RELACION ES REQUERIDA!");
            base.AddRule(() => MPParticularidad, () => MPParticularidad != -1, "PARTICULARIDAD ES REQUERIDA!");
            OnPropertyChanged("MPNombre");
            OnPropertyChanged("MPPaterno");
            OnPropertyChanged("MPMaterno");
            //OnPropertyChanged("MPAlias");
            OnPropertyChanged("MPRelacion");
            OnPropertyChanged("MPParticularidad");
        }
        #endregion

        #region Medida Lugar
        void ValidarMedidaLugar()
        {
            base.ClearRules();
            base.AddRule(() => MLPertenece, () => !string.IsNullOrEmpty(MLPertenece), "PERTENECE A ES REQUERIDO!");
            base.AddRule(() => MLCalle, () => !string.IsNullOrEmpty(MLCalle), "CALLE ES REQUERIDA!");
            base.AddRule(() => MLCalle, () => !string.IsNullOrEmpty(MLCalle), "CALLE ES REQUERIDA!");
            base.AddRule(() => MLNoExterior, () => MLNoExterior.HasValue, "NÚMERO EXTERIOR ES REQUERIDA!");
            //base.AddRule(() => MLTelefono, () => !string.IsNullOrEmpty(MLTelefono), "TELÉFONO EXTERIOR ES REQUERIDO!");
            base.AddRule(() => MLGiro, () => MLGiro != -1, "GIRO EXTERIOR ES REQUERIDO!");
            base.AddRule(() => MLEntidad, () => MLEntidad != -1, "ESTADO ES REQUERIDO!");
            base.AddRule(() => MLMunicipio, () => MLMunicipio != -1, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => MLColonia, () => !string.IsNullOrEmpty(MLColonia), "COLONIA ES REQUERIDA!");
            OnPropertyChanged("MLCalle");
            OnPropertyChanged("MLNoExterior");
            //OnPropertyChanged("MLTelefono");
            OnPropertyChanged("MLGiro");
            OnPropertyChanged("MLEntidad");
            OnPropertyChanged("MLMunicipio");
            OnPropertyChanged("MLColonia");
         }
        #endregion

        #region Medida Presentacion
        void ValidarMedidaPresentacion()
        {
            base.ClearRules();
            base.AddRule(() => MPRLugar, () => MPRLugar != -1, "LUGAR ES REQUERIDO!");
            base.AddRule(() => MPRAsesor, () => MPRAsesor != -1, "ASESOR ES REQUERIDO!");
            OnPropertyChanged("MPRLugar");
            OnPropertyChanged("MPRAsesor");
        }
        #endregion

        #region Documento
        void ValidarDocumento() 
        {
            base.ClearRules();
            base.AddRule(() => MDFecha, () => MDFecha.HasValue, "FECHA ES REQUERIDA!");
            base.AddRule(() => MDFolio, () => !string.IsNullOrEmpty(MDFolio), "FOLIO ES REQUERIDO!");
            base.AddRule(() => MDAutor, () => MDAutor != -1, "AUTOR ES REQUERIDO!");
            base.AddRule(() => MDTitulo, () => !string.IsNullOrEmpty(MDTitulo), "TITULO ES REQUERIDO!");
            base.AddRule(() => MDFuente, () => MDFuente != -1, "FUENTE ES REQUERIDO!");
            base.AddRule(() => MDTipoDocumento, () => MDTipoDocumento != -1, "TIPO DE DOCUMENTO ES REQUERIDO!");
            base.AddRule(() => MDSeleccion, () => !string.IsNullOrEmpty(MDSeleccion), "DOCUMENTO ES REQUERIDO!");
            
            OnPropertyChanged("MDFecha");
            OnPropertyChanged("MDFolio");
            OnPropertyChanged("MDAutor");
            OnPropertyChanged("MDTitulo");
            OnPropertyChanged("MDFuente");
            OnPropertyChanged("MDTipoDocumento");
            OnPropertyChanged("MDSeleccion");
        }
        #endregion

        #region Seguimiento
        void ValidarSeguimiento()
        {
            base.ClearRules();
            base.AddRule(() => SFecha, () => SFecha.HasValue, "FECHA ES REQUERIDO!");
            base.AddRule(() => SObservacion, () => !string.IsNullOrEmpty(SObservacion), "FECHA ES REQUERIDO!");
            OnPropertyChanged("SFecha");
            OnPropertyChanged("SObservacion");
        }
        #endregion
    }
}
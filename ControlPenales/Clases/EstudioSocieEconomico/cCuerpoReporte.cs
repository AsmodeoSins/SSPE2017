using System.Linq;
namespace ControlPenales
{
    public class cCuerpoReporte
    {
        #region Datos Generales
        public string NombreInternoInicial { get; set; }
        public string NombreInterno { get; set; }
        public string NombreCentro { get; set; }
        public string FechaNacimiento { get; set; }
        public string EdadInterno { get; set; }
        public string Ocupacion { get; set; }
        public string SalarioPercibia { get; set; }
        public string Escolaridad { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        #endregion
        #region Caract. del grupo familiar primario
        public string GrupoFamiliarPrimario { get; set; }
        public string RelacionIntroFamiliarPrimariaGrupoPrimario { get; set; }
        public string CuantasPersonasVivenEnHogar { get; set; }
        public string CuantasPersonasLaboranGrupoPrimario { get; set; }
        public string IngresoMensualFamiliarGrupoPrimario { get; set; }
        public string EgresoMensualFamiliarGrupoPrimario { get; set; }
        public string IntegranteAntecedentesAdiccionGrupoPrimario { get; set; }
        public string EspecifiqueAntecedentesAdiccionGrupoPrimario { get; set; }
        public string ZonaUbicacionViviendaGrupoPrimario { get; set; }
        public string CondicionesViviendaGrupoPrimario { get; set; }
        public string MaterialesConstruccionViviendaGrupoPrimario { get; set; }
        public IQueryable<string> ListDistribucionViviendaGrupoPrimario { get; set; }
        public IQueryable<string> ListServiciosViviendaGrupoPrimario { get; set; }
        public IQueryable<string> ListElectrodomesticosGrupoPrimario { get; set; }
        public IQueryable<string> ListMediosTransporteGrupoPrimario { get; set; }
        public string NivelSocioEconomicoCulturalGrupoPrimario { get; set; }
        #endregion
        #region Caract. del grupo familiar secundario
        public string GrupoFamiliarSecundario { get; set; }
        public string RelacionIntroFamiliarGrupoSecundario { get; set; }
        public string CuantasPersonasLaboranGrupoSecundario { get; set; }
        public string IngresoMensualGrupoSecundario { get; set; }
        public string EgresoMensualGrupoSecundario { get; set; }
        public string AntecedentesAdiccionGrupoSecundario { get; set; }
        public string EspecifiqueAdiccionAntecedentesGrupoSecundario { get; set; }
        public string ZonaUbicacionGrupoSecundario { get; set; }
        public string CondicionesViviendaGrupoSecundario { get; set; }
        public string MaterialesConstruccionViviendaGrupSecundario { get; set; }
        public IQueryable<string> DistribucionViviendaGRupoSecundario { get; set; }
        public IQueryable<string> ServiciosGRupoSecundario { get; set; }
        public IQueryable<string> ElectrodomesticosGrupoSecundario { get; set; }
        public IQueryable<string> MediosTransporteGrupoSecundario { get; set; }
        public string NivelSocioEconomicoCulturalGrupoSecundario { get; set; }
        public string RecibeVisitaFamiliar { get; set; }
        public string DeQuien { get; set; }
        public string Frecuencia { get; set; }
        public string EnCasoDeNoRecibirVisitaEspecifique { get; set; }
        public string CuentaConApoyoEconomico { get; set; }
        public string EspecifiqueApoyoEconomico { get; set; }
        #endregion
        #region Dictamen SocioEconomico
        public string Dictamen { get; set; }
        public string FechaEstudio { get; set; }
        #endregion

        public string DistVivivendaPrimario { get;  set; }
        public string DistViviendaSecundario { get; set; }
        public string ServiciosPrimario { get; set; }
        public string ServicioSecundario { get; set; }
        public string ViviendaMaterialesPrimario { get; set; }
        public string VivivendamaterialesSecundario { get; set; }
        public string ElectViviendaPrimario { get; set; }
        public string ElectViviendaSecundario { get; set; }
        public string MediosTranspPrimario { get; set; }
        public string MediosTransporteSec { get; set; }
        #region FichaIdentificaciom
        public string HorariosDiasTrabajo { get; set; } 
        public string ActitudGeneralEntrevistado { get; set; }
        public string Delito { get; set; }
        public string MedidaCautelar { get; set; }
        public string Observaciones{ get; set; }
        public string Sexo { get; set; }

        public string LugarNac{ get; set; }

        public string Apodo { get; set; }
        public string Alias{ get; set; }

        public string EstadoCivil { get; set; }
        public string SituacionJuridica { get; set; }



        #endregion

        #region Complementarios
        public string Elaboro { get; set; }
        public string Coordinador { get; set; }
        #endregion

    }
}

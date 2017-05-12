using MoreLinq;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LinqKit;
using System.Transactions;
using System.Data;
using System.Text;
using System.Data.Objects;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cImputado : EntityManagerServer<IMPUTADO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cImputado()
        { }


        public IQueryable<IMPUTADO> ObtieneImputado(string Busqueda = "")
        {
            try
            {
                getDbSet();
                if (string.IsNullOrEmpty(Busqueda))
                    return GetData().OrderBy(x => x.ID_IMPUTADO).ThenBy(o => o.PATERNO);
                else
                    return GetData().Where(x => x.NOMBRE.Contains(Busqueda) || x.PATERNO.Contains(Busqueda)).OrderBy(x => x.ID_IMPUTADO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IMPUTADO ObtenerPorNIP(string NIP)
        {
            try
            {

                return GetData(g =>
                    g.NIP.TrimEnd() == NIP).
                    FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<IMPUTADO> ObtenerXNip(string nip = "")
        {
            try
            {
                if (string.IsNullOrEmpty(nip)) return null;
                getDbSet();
                //var _nip = int.Parse(nip);
                return GetData().Where(x => !string.IsNullOrEmpty(x.NIP) ? x.NIP.Contains(nip) : false).OrderBy(x => x.ID_IMPUTADO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public ObservableCollection<IMPUTADO> ObtenerTodosCentro(short ubi_centro, short?[] estatus_inactivos, string ApellidoPaternoBuscar = "", string ApellidoMaternoBuscar = "", string NombreBuscar = "", int? Anio = 0, int? Folio = 0, int Pagina = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) && string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) && string.IsNullOrWhiteSpace(NombreBuscar) && !Anio.HasValue && !Folio.HasValue)
                    return new ObservableCollection<IMPUTADO>();

                NombreBuscar = NombreBuscar ?? string.Empty;
                ApellidoPaternoBuscar = ApellidoPaternoBuscar ?? string.Empty;
                ApellidoMaternoBuscar = ApellidoMaternoBuscar ?? string.Empty;

                if (((Anio.HasValue && Folio.HasValue) == false) && (ApellidoPaternoBuscar.Trim().Length < 2 && ApellidoMaternoBuscar.Trim().Length < 2 && NombreBuscar.Trim().Length < 2))
                    return new ObservableCollection<IMPUTADO>();

                var PredicadoAND1 = PredicateBuilder.True<IMPUTADO>();
                var PredicadoAND2 = PredicateBuilder.True<ALIAS>();
                var PredicadoAND3 = PredicateBuilder.True<APODO>();

                if (!string.IsNullOrWhiteSpace(NombreBuscar.Trim()))
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.NOMBRE.Contains(NombreBuscar.Trim()));
                    PredicadoAND2 = PredicadoAND2.And(w => w.NOMBRE.Contains(NombreBuscar.Trim()));
                    PredicadoAND3 = PredicadoAND3.And(w => w.APODO1.Contains(NombreBuscar.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(ApellidoPaternoBuscar.Trim()))
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.PATERNO.Contains(ApellidoPaternoBuscar.Trim()));
                    PredicadoAND2 = PredicadoAND2.And(w => w.PATERNO.Contains(ApellidoPaternoBuscar.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(ApellidoMaternoBuscar.Trim()))
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.MATERNO.Contains(ApellidoMaternoBuscar.Trim()));
                    PredicadoAND2 = PredicadoAND2.And(w => w.MATERNO.Contains(ApellidoMaternoBuscar.Trim()));
                }

                if (Anio.HasValue)
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.ID_ANIO == Anio.Value);
                    PredicadoAND2 = PredicadoAND2.And(w => w.ID_ANIO == Anio.Value);
                    PredicadoAND3 = PredicadoAND3.And(w => w.ID_ANIO == Anio.Value);
                }

                if (Folio.HasValue)
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.ID_IMPUTADO == Folio.Value);
                    PredicadoAND2 = PredicadoAND2.And(w => w.ID_IMPUTADO == Folio.Value);
                    PredicadoAND3 = PredicadoAND3.And(w => w.ID_IMPUTADO == Folio.Value);
                }

                var PredicadoOR1 = PredicateBuilder.False<IMPUTADO>();
                var PredicadoOR2 = PredicateBuilder.False<ALIAS>();
                var PredicadoOR3 = PredicateBuilder.False<APODO>();

                foreach (var item in  estatus_inactivos.Where(w=>w.HasValue))
                {
                    PredicadoOR1 = PredicadoOR1.Or(w=>w.INGRESO.Any(a=>a.ID_UB_CENTRO == ubi_centro && a.ID_ESTATUS_ADMINISTRATIVO!=item.Value));
                    PredicadoOR2 = PredicadoOR2.Or(w => w.IMPUTADO.INGRESO.Any(a => a.ID_UB_CENTRO == ubi_centro && a.ID_ESTATUS_ADMINISTRATIVO != item.Value));
                    PredicadoOR3 = PredicadoOR3.Or(w => w.IMPUTADO.INGRESO.Any(a => a.ID_UB_CENTRO == ubi_centro && a.ID_ESTATUS_ADMINISTRATIVO != item.Value));
                }

                PredicadoAND1 = PredicadoAND1.And(PredicadoOR1.Expand());
                PredicadoAND2 = PredicadoAND2.And(PredicadoOR2.Expand());
                PredicadoAND3 = PredicadoAND3.And(PredicadoOR3.Expand());


                var query=Context.IMPUTADO.Where(PredicadoAND1.Expand()).AsEnumerable().Select(s => s).Union(Context.Aliases.Where(PredicadoAND2.Expand()).AsEnumerable().Select(s => new IMPUTADO
                    {
                        NOMBRE = (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrWhiteSpace(NombreBuscar) ? s.NOMBRE.Trim() : NombreBuscar) ?
                        s.IMPUTADO.NOMBRE.Trim()
                        :
                        s.IMPUTADO.NOMBRE.Trim() + (string.IsNullOrWhiteSpace(NombreBuscar.Trim()) ? "" : " (" + s.NOMBRE.Trim() + ")"),
                        PATERNO = (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim().Equals(string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) ? (s.PATERNO = s.PATERNO ?? "").Trim() : ApellidoPaternoBuscar.Trim())
                        ?
                        (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim()
                        :
                        (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim() + (string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) ? string.Empty : " (" + (s.PATERNO = s.PATERNO ?? "").Trim() + ")"),
                        MATERNO = (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim().Equals(string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) ? (s.MATERNO = s.MATERNO ?? "").Trim() : ApellidoMaternoBuscar.Trim())
                        ?
                        (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim()
                        :
                        (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim() + (string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) ? string.Empty : " (" + (s.MATERNO = s.MATERNO ?? "").Trim() + ")"),
                        ID_ANIO = s.ID_ANIO,
                        ID_CENTRO = s.ID_CENTRO,
                        ID_IMPUTADO = s.ID_IMPUTADO,
                        CENTRO = s.IMPUTADO.CENTRO
                    }));
                if (!string.IsNullOrWhiteSpace(NombreBuscar) && (string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) && string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) && !Anio.HasValue && !Folio.HasValue))
                { 
                    query=query.Union(
                        Context.APODOes.Where(PredicadoAND3.Expand()).AsEnumerable().Select(s => new IMPUTADO
                        {
                            NOMBRE = (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrWhiteSpace(NombreBuscar) ? (s.APODO1 = s.APODO1 ?? "").Trim() : NombreBuscar.Trim())
                            ?
                            (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
                            :
                            (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim() + (string.IsNullOrWhiteSpace(NombreBuscar) ? string.Empty : " (" + (s.APODO1 = s.APODO1 ?? "").Trim() + ")"),
                            PATERNO = "",
                            MATERNO = "",
                            ID_ANIO = s.ID_ANIO,
                            ID_CENTRO = s.ID_CENTRO,
                            ID_IMPUTADO = s.ID_IMPUTADO,
                            CENTRO = s.IMPUTADO.CENTRO
                        }));
                }
                return new ObservableCollection<IMPUTADO>((query)
                .DistinctBy(d => new { d.ID_ANIO, d.ID_CENTRO, d.ID_IMPUTADO })
                    .OrderBy(o => o.PATERNO).ThenBy(o => o.MATERNO).ThenBy(o => o.NOMBRE)
                    .Take((Pagina * 30))
                    .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30))));
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        //public ObservableCollection<IMPUTADO> ObtenerTodosCentro(string ApellidoPaternoBuscar = "", string ApellidoMaternoBuscar = "", string NombreBuscar = "", int? Anio = 0, int? Folio = 0, int Pagina = 1)
        //{
        //    if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !Anio.HasValue && !Folio.HasValue)
        //        return new ObservableCollection<IMPUTADO>();

        //    NombreBuscar = NombreBuscar ?? string.Empty;
        //    ApellidoPaternoBuscar = ApellidoPaternoBuscar ?? string.Empty;
        //    ApellidoMaternoBuscar = ApellidoMaternoBuscar ?? string.Empty;

        //    if (((Anio.HasValue && Folio.HasValue) == false) && (ApellidoPaternoBuscar.Trim().Length < 2 && ApellidoMaternoBuscar.Trim().Length < 2 && NombreBuscar.Trim().Length < 2))
        //        return new ObservableCollection<IMPUTADO>();

        //    var PredicadoAND = PredicateBuilder.True<V_BUSCA_NOMBRE>();

        //    if (!string.IsNullOrEmpty(NombreBuscar.Trim()))
        //        PredicadoAND = PredicadoAND.And(w => w.AL_NOMBRE.Contains(NombreBuscar.Trim()) || w.NOMBRE.Contains(NombreBuscar.Trim()) || w.APODO.Contains(NombreBuscar.Trim()));
        //    if (!string.IsNullOrEmpty(ApellidoPaternoBuscar.Trim()))
        //        PredicadoAND = PredicadoAND.And(w => w.AL_PATERNO.Contains(ApellidoPaternoBuscar.Trim()) || w.PATERNO.Contains(ApellidoPaternoBuscar.Trim()));
        //    if (!string.IsNullOrEmpty(ApellidoMaternoBuscar.Trim()))
        //        PredicadoAND = PredicadoAND.And(w => w.AL_MATERNO.Contains(ApellidoMaternoBuscar.Trim()) || w.MATERNO.Contains(ApellidoMaternoBuscar.Trim()));

        //    if (Anio.HasValue)
        //        PredicadoAND = PredicadoAND.And(w => w.ID_ANIO == Anio);
        //    if (Folio.HasValue)
        //        PredicadoAND = PredicadoAND.And(w => w.ID_IMPUTADO == Folio);

        //    #region Solo mostrar centro actual
        //    PredicadoAND = PredicadoAND.And(w => Context.INGRESO.Count(x => x.ID_CENTRO == w.ID_CENTRO && x.ID_ANIO == w.ID_ANIO && x.ID_IMPUTADO == w.ID_IMPUTADO && x.ID_UB_CENTRO.HasValue && x.ID_UB_CENTRO == GlobalVariables.gCentro) > 0);
        //    #endregion

        //    #region [nuevo]
        //    var union = Context.V_BUSCA_NOMBRE
        //        .AsExpandable()
        //        .Where(PredicadoAND)
        //        .OrderBy(o => new { o.ID_CENTRO, o.ID_ANIO, o.ID_IMPUTADO })
        //        .Distinct()
        //        .GroupBy(g => new { g.ID_CENTRO, g.ID_ANIO, g.ID_IMPUTADO })
        //        .Select(s => s.FirstOrDefault())
        //        .Distinct()
        //        .OrderBy(o => new { o.PATERNO, o.MATERNO, o.NOMBRE })
        //        .Take((Pagina * 30))
        //        .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)))
        //        .Select(s => new
        //        {
        //            IMPUTADO = Context.IMPUTADO.Where(w => w.ID_CENTRO == s.ID_CENTRO && w.ID_ANIO == s.ID_ANIO && w.ID_IMPUTADO == s.ID_IMPUTADO).FirstOrDefault(),
        //            APODO = Context.IMPUTADO.Where(w => w.ID_CENTRO == s.ID_CENTRO && w.ID_ANIO == s.ID_ANIO && w.ID_IMPUTADO == s.ID_IMPUTADO).FirstOrDefault().APODO.Where(w => w.APODO1.Trim().Contains(NombreBuscar.Trim())).FirstOrDefault(),
        //            ALIAS = Context.IMPUTADO.Where(w => w.ID_CENTRO == s.ID_CENTRO && w.ID_ANIO == s.ID_ANIO && w.ID_IMPUTADO == s.ID_IMPUTADO).FirstOrDefault().ALIAS.Where(w => w.NOMBRE.Trim().Contains(NombreBuscar.Trim()) || w.MATERNO.Trim().Contains(ApellidoMaternoBuscar.Trim()) || w.PATERNO.Trim().Contains(ApellidoPaternoBuscar.Trim())).FirstOrDefault()
        //        })
        //        .AsEnumerable()
        //        .Select(s => new IMPUTADO
        //        {
        //            NOMBRE =
        //                    (s.ALIAS == null && s.APODO == null)
        //                        ?
        //                        (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
        //                        :
        //                        s.ALIAS != null
        //                            ?
        //                            (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrEmpty(NombreBuscar) ? s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE.Trim() : NombreBuscar.Trim())
        //                                ?
        //                                (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
        //                                :
        //                                (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim() + (string.IsNullOrEmpty(NombreBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE = s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE ?? "").Trim() + ")")
        //                            :
        //                            s.APODO != null
        //                                ?
        //                                (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrEmpty(NombreBuscar) ? s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE.Trim() : NombreBuscar.Trim())
        //                                    ?
        //                                    (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
        //                                    :
        //                                    (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim() + (string.IsNullOrEmpty(NombreBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.APODO.Where(w => w == s.APODO).FirstOrDefault().APODO1 = s.IMPUTADO.APODO.Where(w => w == s.APODO).FirstOrDefault().APODO1 ?? "").Trim() + ")")
        //                                :
        //                                string.Empty,

        //            PATERNO =
        //                        s.ALIAS == null
        //                            ?
        //                            (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim()
        //                            :
        //                            s.ALIAS != null
        //                                ?
        //                                (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim().Equals(string.IsNullOrEmpty(ApellidoPaternoBuscar.Trim()) ? s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().PATERNO.Trim() : ApellidoPaternoBuscar.Trim())
        //                                    ?
        //                                    (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim()
        //                                    :
        //                                    (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim() + (string.IsNullOrEmpty(ApellidoPaternoBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().PATERNO = s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().PATERNO ?? "").Trim() + ")")
        //                                :
        //                                string.Empty,

        //            MATERNO =
        //                        s.ALIAS == null
        //                            ?
        //                            (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim()
        //                            :
        //                            s.ALIAS != null
        //                                ?
        //                                (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim().Equals(string.IsNullOrEmpty(ApellidoMaternoBuscar.Trim()) ? (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().MATERNO ?? "").Trim() : ApellidoMaternoBuscar.Trim())  //Arregla bug con el apellido materno del alias
        //                                    ?
        //                                    (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim()
        //                                    :
        //                                    (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim() + (string.IsNullOrEmpty(ApellidoMaternoBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().MATERNO = s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().MATERNO ?? "").Trim() + ")")
        //                                :
        //                                string.Empty,

        //            CENTRO = s.IMPUTADO.CENTRO,
        //            ID_ANIO = s.IMPUTADO.ID_ANIO,
        //            ID_IMPUTADO = s.IMPUTADO.ID_IMPUTADO,

        //            INGRESO = s.IMPUTADO.INGRESO,
        //            ALIAS = s.IMPUTADO.ALIAS,
        //            APODO = s.IMPUTADO.APODO,
        //            BI_FECALTA = s.IMPUTADO.BI_FECALTA,
        //            BI_USER = s.IMPUTADO.BI_USER,
        //            //COLONIA = s.IMPUTADO.COLONIA,
        //            CURP = s.IMPUTADO.CURP,
        //            DIALECTO = s.IMPUTADO.DIALECTO,
        //            //DOMICILIO_CALLE = s.IMPUTADO.DOMICILIO_CALLE,
        //            //DOMICILIO_CODIGO_POSTAL = s.IMPUTADO.DOMICILIO_CODIGO_POSTAL,
        //            //DOMICILIO_NUM_EXT = s.IMPUTADO.DOMICILIO_NUM_EXT,
        //            //DOMICILIO_NUM_INT = s.IMPUTADO.DOMICILIO_NUM_INT,
        //            //DOMICILIO_TRABAJO = s.IMPUTADO.DOMICILIO_TRABAJO,
        //            //ENTIDAD = s.IMPUTADO.ENTIDAD,
        //            //ESCOLARIDAD = s.IMPUTADO.ESCOLARIDAD,
        //            //ESTADO_CIVIL = s.IMPUTADO.ESTADO_CIVIL,
        //            //ESTATURA = s.IMPUTADO.ESTATURA,
        //            ETNIA = s.IMPUTADO.ETNIA,
        //            FAMILIAR_RESPONSABLE = s.IMPUTADO.FAMILIAR_RESPONSABLE,
        //            ID_CENTRO = s.IMPUTADO.ID_CENTRO,
        //            //ID_COLONIA = s.IMPUTADO.ID_COLONIA,
        //            ID_DIALECTO = s.IMPUTADO.ID_DIALECTO,
        //            //ID_ENTIDAD = s.IMPUTADO.ID_ENTIDAD,
        //            //ID_ESCOLARIDAD = s.IMPUTADO.ID_ESCOLARIDAD,
        //            //ID_ESTADO_CIVIL = s.IMPUTADO.ID_ESTADO_CIVIL,
        //            ID_ETNIA = s.IMPUTADO.ID_ETNIA,
        //            ID_IDIOMA = s.IMPUTADO.ID_IDIOMA,
        //            //ID_MUNICIPIO = s.IMPUTADO.ID_MUNICIPIO,
        //            ID_NACIONALIDAD = s.IMPUTADO.ID_NACIONALIDAD,
        //            //ID_OCUPACION = s.IMPUTADO.ID_OCUPACION,
        //            //ID_PAIS = s.IMPUTADO.ID_PAIS,
        //            //ID_RELIGION = s.IMPUTADO.ID_RELIGION,
        //            //ID_TIPO_DISCAPACIDAD = s.IMPUTADO.ID_TIPO_DISCAPACIDAD,
        //            IDIOMA = s.IMPUTADO.IDIOMA,
        //            IFE = s.IMPUTADO.IFE,
        //            IMPUTADO_BIOMETRICO = s.IMPUTADO.IMPUTADO_BIOMETRICO,
        //            //IMPUTADO_DOCUMENTO = s.IMPUTADO.IMPUTADO_DOCUMENTO,
        //            IMPUTADO_FILIACION = s.IMPUTADO.IMPUTADO_FILIACION,
        //            IMPUTADO_FORMATO = s.IMPUTADO.IMPUTADO_FORMATO,
        //            IMPUTADO_PADRES = s.IMPUTADO.IMPUTADO_PADRES,
        //            IMPUTADO_PANDILLA = s.IMPUTADO.IMPUTADO_PANDILLA,
        //            //LUGAR_RESIDENCIA = s.IMPUTADO.LUGAR_RESIDENCIA,
        //            //MADRE_FINADO = s.IMPUTADO.MADRE_FINADO,
        //            MATERNO_MADRE = s.IMPUTADO.MATERNO_MADRE,
        //            MATERNO_PADRE = s.IMPUTADO.MATERNO_PADRE,
        //            //MUNICIPIO = s.IMPUTADO.MUNICIPIO,
        //            NACIMIENTO_ESTADO = s.IMPUTADO.NACIMIENTO_ESTADO,
        //            NACIMIENTO_FECHA = s.IMPUTADO.NACIMIENTO_FECHA,
        //            NACIMIENTO_LUGAR = s.IMPUTADO.NACIMIENTO_LUGAR,
        //            NACIMIENTO_MUNICIPIO = s.IMPUTADO.NACIMIENTO_MUNICIPIO,
        //            NACIMIENTO_PAIS = s.IMPUTADO.NACIMIENTO_PAIS,
        //            NIP = s.IMPUTADO.NIP,
        //            NOMBRE_MADRE = s.IMPUTADO.NOMBRE_MADRE,
        //            NOMBRE_PADRE = s.IMPUTADO.NOMBRE_PADRE,
        //            //NUMERO_IDENTIFICACION = s.IMPUTADO.NUMERO_IDENTIFICACION,
        //            //OCUPACION = s.IMPUTADO.OCUPACION,
        //            //PADRE_FINADO = s.IMPUTADO.PADRE_FINADO,
        //            PAIS_NACIONALIDAD = s.IMPUTADO.PAIS_NACIONALIDAD,
        //            //PAIS_NACIONALIDAD1 = s.IMPUTADO.PAIS_NACIONALIDAD1,
        //            PATERNO_MADRE = s.IMPUTADO.PATERNO_MADRE,
        //            PATERNO_PADRE = s.IMPUTADO.PATERNO_PADRE,
        //            //PESO = s.IMPUTADO.PESO,
        //            RELACION_PERSONAL_INTERNO = s.IMPUTADO.RELACION_PERSONAL_INTERNO,
        //            //RELIGION = s.IMPUTADO.RELIGION,
        //            //RESIDENCIA_ANIOS = s.IMPUTADO.RESIDENCIA_ANIOS,
        //            //RESIDENCIA_MESES = s.IMPUTADO.RESIDENCIA_MESES,
        //            RFC = s.IMPUTADO.RFC,
        //            SENAS_PARTICULARES = s.IMPUTADO.SENAS_PARTICULARES,
        //            SEXO = s.IMPUTADO.SEXO,
        //            SMATERNO = s.IMPUTADO.SMATERNO,
        //            SNOMBRE = s.IMPUTADO.SNOMBRE,
        //            SPATERNO = s.IMPUTADO.SPATERNO,
        //            TABAJADOR_CERESO = s.IMPUTADO.TABAJADOR_CERESO,
        //            //TELEFONO = s.IMPUTADO.TELEFONO,
        //            TELORIGINAL = s.IMPUTADO.TELORIGINAL,
        //            //TIPO_DISCAPACIDAD = s.IMPUTADO.TIPO_DISCAPACIDAD,
        //            TRADUCTOR = s.IMPUTADO.TRADUCTOR,
        //            PRS_ENTREVISTA_INICIAL = s.IMPUTADO.PRS_ENTREVISTA_INICIAL,
        //            EMI_IMPUTADO = s.IMPUTADO.EMI_IMPUTADO,
        //            LIBERADO = s.IMPUTADO.LIBERADO,
        //            PRS_FICHA_IDEN = s.IMPUTADO.PRS_FICHA_IDEN,
        //            PRS_REPORTE_PSICOLOGICO = s.IMPUTADO.PRS_REPORTE_PSICOLOGICO,
        //            PRS_VISITA_DOMICILIARIA = s.IMPUTADO.PRS_VISITA_DOMICILIARIA

        //        })
        //        .DistinctBy(d => new { d.ID_CENTRO, d.ID_ANIO, d.ID_IMPUTADO })
        //        .ToList();
        //    #endregion
        //    return new ObservableCollection<IMPUTADO>(union); //null;//
        //}

        public ObservableCollection<IMPUTADO> ObtenerTodos(string ApellidoPaternoBuscar = "", string ApellidoMaternoBuscar = "", string NombreBuscar = "", int? Anio = 0, int? Folio = 0, int Pagina = 1)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) && string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) && string.IsNullOrWhiteSpace(NombreBuscar) && !Anio.HasValue && !Folio.HasValue)
                    return new ObservableCollection<IMPUTADO>();

                NombreBuscar = NombreBuscar ?? string.Empty;
                ApellidoPaternoBuscar = ApellidoPaternoBuscar ?? string.Empty;
                ApellidoMaternoBuscar = ApellidoMaternoBuscar ?? string.Empty;

                if (((Anio.HasValue && Folio.HasValue) == false) && (ApellidoPaternoBuscar.Trim().Length < 2 && ApellidoMaternoBuscar.Trim().Length < 2 && NombreBuscar.Trim().Length < 2))
                    return new ObservableCollection<IMPUTADO>();

                var PredicadoAND1 = PredicateBuilder.True<IMPUTADO>();
                var PredicadoAND2 = PredicateBuilder.True<ALIAS>();
                var PredicadoAND3 = PredicateBuilder.True<APODO>();

                if (!string.IsNullOrWhiteSpace(NombreBuscar.Trim()))
                {
                    PredicadoAND1 = PredicadoAND1.And(w=>w.NOMBRE.Contains(NombreBuscar.Trim()));
                    PredicadoAND2 = PredicadoAND2.And(w=>w.NOMBRE.Contains(NombreBuscar.Trim()));
                    PredicadoAND3 = PredicadoAND3.And(w=>w.APODO1.Contains(NombreBuscar.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(ApellidoPaternoBuscar.Trim()))
                {
                    PredicadoAND1 = PredicadoAND1.And(w=>w.PATERNO.Contains(ApellidoPaternoBuscar.Trim()));
                    PredicadoAND2 = PredicadoAND2.And(w=>w.PATERNO.Contains(ApellidoPaternoBuscar.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(ApellidoMaternoBuscar.Trim()))
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.MATERNO.Contains(ApellidoMaternoBuscar.Trim()));
                    PredicadoAND2 = PredicadoAND2.And(w=>w.MATERNO.Contains(ApellidoMaternoBuscar.Trim()));
                }
                
                if (Anio.HasValue)
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.ID_ANIO == Anio.Value);
                    PredicadoAND2 = PredicadoAND2.And(w => w.ID_ANIO == Anio.Value);
                    PredicadoAND3 = PredicadoAND3.And(w=>w.ID_ANIO==Anio.Value);
                }

                if (Folio.HasValue)
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.ID_IMPUTADO == Folio.Value);
                    PredicadoAND2 = PredicadoAND2.And(w=>w.ID_IMPUTADO==Folio.Value);
                    PredicadoAND3 = PredicadoAND3.And(w=>w.ID_IMPUTADO==Folio.Value);
                }

                var query = Context.IMPUTADO.Where(PredicadoAND1.Expand()).AsEnumerable().Select(s => s).Union(Context.Aliases.Where(PredicadoAND2.Expand()).AsEnumerable().Select(s => new IMPUTADO
                {
                    NOMBRE = (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrWhiteSpace(NombreBuscar) ? s.NOMBRE.Trim() : NombreBuscar) ?
                    s.IMPUTADO.NOMBRE.Trim()
                    :
                    s.IMPUTADO.NOMBRE.Trim() + (string.IsNullOrWhiteSpace(NombreBuscar.Trim()) ? "" : " (" + s.NOMBRE.Trim() + ")"),
                    PATERNO = (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim().Equals(string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) ? (s.PATERNO = s.PATERNO ?? "").Trim() : ApellidoPaternoBuscar.Trim())
                    ?
                    (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim()
                    :
                    (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim() + (string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) ? string.Empty : " (" + (s.PATERNO = s.PATERNO ?? "").Trim() + ")"),
                    MATERNO = (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim().Equals(string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) ? (s.MATERNO = s.MATERNO ?? "").Trim() : ApellidoMaternoBuscar.Trim())
                    ?
                    (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim()
                    :
                    (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim() + (string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) ? string.Empty : " (" + (s.MATERNO = s.MATERNO ?? "").Trim() + ")"),
                    ID_ANIO = s.ID_ANIO,
                    ID_CENTRO = s.ID_CENTRO,
                    ID_IMPUTADO = s.ID_IMPUTADO,
                    CENTRO = s.IMPUTADO.CENTRO
                }));
                if (!string.IsNullOrWhiteSpace(NombreBuscar) && (string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) && string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) && !Anio.HasValue && !Folio.HasValue))
                {
                    query = query.Union(
                        Context.APODOes.Where(PredicadoAND3.Expand()).AsEnumerable().Select(s => new IMPUTADO
                        {
                            NOMBRE = (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrWhiteSpace(NombreBuscar) ? (s.APODO1 = s.APODO1 ?? "").Trim() : NombreBuscar.Trim())
                            ?
                            (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
                            :
                            (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim() + (string.IsNullOrWhiteSpace(NombreBuscar) ? string.Empty : " (" + (s.APODO1 = s.APODO1 ?? "").Trim() + ")"),
                            PATERNO = "",
                            MATERNO = "",
                            ID_ANIO = s.ID_ANIO,
                            ID_CENTRO = s.ID_CENTRO,
                            ID_IMPUTADO = s.ID_IMPUTADO,
                            CENTRO = s.IMPUTADO.CENTRO
                        }));
                }

                return new ObservableCollection<IMPUTADO>((query)
                .DistinctBy(d => new {d.ID_ANIO,d.ID_CENTRO,d.ID_IMPUTADO })
                    .OrderBy(o => o.PATERNO).ThenBy(o => o.MATERNO).ThenBy(o => o.NOMBRE)
                    .Take((Pagina * 30))
                    .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)))
                );
                
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        //public ObservableCollection<IMPUTADO> ObtenerTodos(short ubi_centro, string ApellidoPaternoBuscar = "", string ApellidoMaternoBuscar = "", string NombreBuscar = "", int? Anio = 0, int? Folio = 0, int Pagina = 1, string nuc = "")
        //{
        //    try
        //    {
        //        //Context.Configuration.LazyLoadingEnabled = false;
        //        if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !Anio.HasValue && !Folio.HasValue)
        //            return new ObservableCollection<IMPUTADO>();

        //        NombreBuscar = NombreBuscar ?? string.Empty;
        //        ApellidoPaternoBuscar = ApellidoPaternoBuscar ?? string.Empty;
        //        ApellidoMaternoBuscar = ApellidoMaternoBuscar ?? string.Empty;

        //        if (((Anio.HasValue && Folio.HasValue) == false) && (ApellidoPaternoBuscar.Trim().Length < 2 && ApellidoMaternoBuscar.Trim().Length < 2 && NombreBuscar.Trim().Length < 2))
        //            return new ObservableCollection<IMPUTADO>();

        //        var PredicadoAND = PredicateBuilder.True<V_BUSCA_NOMBRE>();

        //        if (!string.IsNullOrEmpty(NombreBuscar.Trim()))
        //            PredicadoAND = PredicadoAND.And(w => w.AL_NOMBRE.Contains(NombreBuscar.Trim()) || w.NOMBRE.Contains(NombreBuscar.Trim()) || w.APODO.Contains(NombreBuscar.Trim()));
        //        if (!string.IsNullOrEmpty(ApellidoPaternoBuscar.Trim()))
        //            PredicadoAND = PredicadoAND.And(w => w.AL_PATERNO.Contains(ApellidoPaternoBuscar.Trim()) || w.PATERNO.Contains(ApellidoPaternoBuscar.Trim()));
        //        if (!string.IsNullOrEmpty(ApellidoMaternoBuscar.Trim()))
        //            PredicadoAND = PredicadoAND.And(w => w.AL_MATERNO.Contains(ApellidoMaternoBuscar.Trim()) || w.MATERNO.Contains(ApellidoMaternoBuscar.Trim()));

        //        if (Anio.HasValue)
        //            PredicadoAND = PredicadoAND.And(w => w.ID_ANIO == Anio);
        //        if (Folio.HasValue)
        //            PredicadoAND = PredicadoAND.And(w => w.ID_IMPUTADO == Folio);


        //        #region Solo mostrar centro actual
        //        PredicadoAND = PredicadoAND.And(w => Context.INGRESO.Count(x => x.ID_CENTRO == w.ID_CENTRO && x.ID_ANIO == w.ID_ANIO && x.ID_IMPUTADO == w.ID_IMPUTADO && x.ID_UB_CENTRO.HasValue && x.ID_UB_CENTRO == ubi_centro) > 0);
        //        #endregion

        //        #region [nuevo]




        //        var union = Context.V_BUSCA_NOMBRE
        //            .AsExpandable()
        //            .Where(PredicadoAND)
        //            .OrderBy(o => new { o.ID_CENTRO, o.ID_ANIO, o.ID_IMPUTADO })
        //            .Distinct()
        //            .GroupBy(g => new { g.ID_CENTRO, g.ID_ANIO, g.ID_IMPUTADO })
        //            .Select(s => s.FirstOrDefault())
        //            .Distinct()
        //            .OrderBy(o => new { o.PATERNO, o.MATERNO, o.NOMBRE })
        //            .Take((Pagina * 30))
        //            .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)))
        //            .Select(s => new
        //            {
        //                IMPUTADO = Context.IMPUTADO.Where(w => w.ID_CENTRO == s.ID_CENTRO && w.ID_ANIO == s.ID_ANIO && w.ID_IMPUTADO == s.ID_IMPUTADO).FirstOrDefault(),
        //                APODO = Context.IMPUTADO.Where(w => w.ID_CENTRO == s.ID_CENTRO && w.ID_ANIO == s.ID_ANIO && w.ID_IMPUTADO == s.ID_IMPUTADO).FirstOrDefault().APODO.Where(w => w.APODO1.Trim().Contains(NombreBuscar.Trim())).FirstOrDefault(),
        //                ALIAS = Context.IMPUTADO.Where(w => w.ID_CENTRO == s.ID_CENTRO && w.ID_ANIO == s.ID_ANIO && w.ID_IMPUTADO == s.ID_IMPUTADO).FirstOrDefault().ALIAS.Where(w => w.NOMBRE.Trim().Contains(NombreBuscar.Trim()) || w.MATERNO.Trim().Contains(ApellidoMaternoBuscar.Trim()) || w.PATERNO.Trim().Contains(ApellidoPaternoBuscar.Trim())).FirstOrDefault()
        //            })
        //            .AsEnumerable()
        //            .Select(s => new IMPUTADO
        //            {
        //                NOMBRE =
        //                        (s.ALIAS == null && s.APODO == null)
        //                            ?
        //                            (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
        //                            :
        //                            s.ALIAS != null
        //                                ?
        //                                (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrEmpty(NombreBuscar) ? s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE.Trim() : NombreBuscar.Trim())
        //                                    ?
        //                                    (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
        //                                    :
        //                                    (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim() + (string.IsNullOrEmpty(NombreBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE = s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE ?? "").Trim() + ")")
        //                                :
        //                                s.APODO != null
        //                                    ?
        //                                    (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrEmpty(NombreBuscar) ? s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE.Trim() : NombreBuscar.Trim())
        //                                        ?
        //                                        (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
        //                                        :
        //                                        (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim() + (string.IsNullOrEmpty(NombreBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.APODO.Where(w => w == s.APODO).FirstOrDefault().APODO1 = s.IMPUTADO.APODO.Where(w => w == s.APODO).FirstOrDefault().APODO1 ?? "").Trim() + ")")
        //                                    :
        //                                    string.Empty,

        //                PATERNO =
        //                            s.ALIAS == null
        //                                ?
        //                                (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim()
        //                                :
        //                                s.ALIAS != null
        //                                    ?
        //                                    (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim().Equals(string.IsNullOrEmpty(ApellidoPaternoBuscar.Trim()) ? s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().PATERNO.Trim() : ApellidoPaternoBuscar.Trim())
        //                                        ?
        //                                        (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim()
        //                                        :
        //                                        (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim() + (string.IsNullOrEmpty(ApellidoPaternoBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().PATERNO = s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().PATERNO ?? "").Trim() + ")")
        //                                    :
        //                                    string.Empty,

        //                MATERNO =
        //                            s.ALIAS == null
        //                                ?
        //                                (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim()
        //                                :
        //                                s.ALIAS != null
        //                                    ?
        //                                    (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim().Equals(string.IsNullOrEmpty(ApellidoMaternoBuscar.Trim()) ? (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().MATERNO ?? "").Trim() : ApellidoMaternoBuscar.Trim())  //Arregla bug con el apellido materno del alias
        //                                        ?
        //                                        (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim()
        //                                        :
        //                                        (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim() + (string.IsNullOrEmpty(ApellidoMaternoBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().MATERNO = s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().MATERNO ?? "").Trim() + ")")
        //                                    :
        //                                    string.Empty,

        //                CENTRO = s.IMPUTADO.CENTRO,
        //                ID_ANIO = s.IMPUTADO.ID_ANIO,
        //                ID_IMPUTADO = s.IMPUTADO.ID_IMPUTADO,

        //                INGRESO = s.IMPUTADO.INGRESO,
        //                ALIAS = s.IMPUTADO.ALIAS,
        //                APODO = s.IMPUTADO.APODO,
        //                BI_FECALTA = s.IMPUTADO.BI_FECALTA,
        //                BI_USER = s.IMPUTADO.BI_USER,
        //                //COLONIA = s.IMPUTADO.COLONIA,
        //                CURP = s.IMPUTADO.CURP,
        //                DIALECTO = s.IMPUTADO.DIALECTO,
        //                //DOMICILIO_CALLE = s.IMPUTADO.DOMICILIO_CALLE,
        //                //DOMICILIO_CODIGO_POSTAL = s.IMPUTADO.DOMICILIO_CODIGO_POSTAL,
        //                //DOMICILIO_NUM_EXT = s.IMPUTADO.DOMICILIO_NUM_EXT,
        //                //DOMICILIO_NUM_INT = s.IMPUTADO.DOMICILIO_NUM_INT,
        //                //DOMICILIO_TRABAJO = s.IMPUTADO.DOMICILIO_TRABAJO,
        //                //ENTIDAD = s.IMPUTADO.ENTIDAD,
        //                //ESCOLARIDAD = s.IMPUTADO.ESCOLARIDAD,
        //                //ESTADO_CIVIL = s.IMPUTADO.ESTADO_CIVIL,
        //                //ESTATURA = s.IMPUTADO.ESTATURA,
        //                ETNIA = s.IMPUTADO.ETNIA,
        //                FAMILIAR_RESPONSABLE = s.IMPUTADO.FAMILIAR_RESPONSABLE,
        //                ID_CENTRO = s.IMPUTADO.ID_CENTRO,
        //                //ID_COLONIA = s.IMPUTADO.ID_COLONIA,
        //                ID_DIALECTO = s.IMPUTADO.ID_DIALECTO,
        //                //ID_ENTIDAD = s.IMPUTADO.ID_ENTIDAD,
        //                //ID_ESCOLARIDAD = s.IMPUTADO.ID_ESCOLARIDAD,
        //                //ID_ESTADO_CIVIL = s.IMPUTADO.ID_ESTADO_CIVIL,
        //                ID_ETNIA = s.IMPUTADO.ID_ETNIA,
        //                ID_IDIOMA = s.IMPUTADO.ID_IDIOMA,
        //                //ID_MUNICIPIO = s.IMPUTADO.ID_MUNICIPIO,
        //                ID_NACIONALIDAD = s.IMPUTADO.ID_NACIONALIDAD,
        //                //ID_OCUPACION = s.IMPUTADO.ID_OCUPACION,
        //                //ID_PAIS = s.IMPUTADO.ID_PAIS,
        //                //ID_RELIGION = s.IMPUTADO.ID_RELIGION,
        //                //ID_TIPO_DISCAPACIDAD = s.IMPUTADO.ID_TIPO_DISCAPACIDAD,
        //                IDIOMA = s.IMPUTADO.IDIOMA,
        //                IFE = s.IMPUTADO.IFE,
        //                IMPUTADO_BIOMETRICO = s.IMPUTADO.IMPUTADO_BIOMETRICO,
        //                //IMPUTADO_DOCUMENTO = s.IMPUTADO.IMPUTADO_DOCUMENTO,
        //                IMPUTADO_FILIACION = s.IMPUTADO.IMPUTADO_FILIACION,
        //                IMPUTADO_FORMATO = s.IMPUTADO.IMPUTADO_FORMATO,
        //                IMPUTADO_PADRES = s.IMPUTADO.IMPUTADO_PADRES,
        //                IMPUTADO_PANDILLA = s.IMPUTADO.IMPUTADO_PANDILLA,
        //                //LUGAR_RESIDENCIA = s.IMPUTADO.LUGAR_RESIDENCIA,
        //                //MADRE_FINADO = s.IMPUTADO.MADRE_FINADO,
        //                MATERNO_MADRE = s.IMPUTADO.MATERNO_MADRE,
        //                MATERNO_PADRE = s.IMPUTADO.MATERNO_PADRE,
        //                //MUNICIPIO = s.IMPUTADO.MUNICIPIO,
        //                NACIMIENTO_ESTADO = s.IMPUTADO.NACIMIENTO_ESTADO,
        //                NACIMIENTO_FECHA = s.IMPUTADO.NACIMIENTO_FECHA,
        //                NACIMIENTO_LUGAR = s.IMPUTADO.NACIMIENTO_LUGAR,
        //                NACIMIENTO_MUNICIPIO = s.IMPUTADO.NACIMIENTO_MUNICIPIO,
        //                NACIMIENTO_PAIS = s.IMPUTADO.NACIMIENTO_PAIS,
        //                NIP = s.IMPUTADO.NIP,
        //                NOMBRE_MADRE = s.IMPUTADO.NOMBRE_MADRE,
        //                NOMBRE_PADRE = s.IMPUTADO.NOMBRE_PADRE,
        //                //NUMERO_IDENTIFICACION = s.IMPUTADO.NUMERO_IDENTIFICACION,
        //                //OCUPACION = s.IMPUTADO.OCUPACION,
        //                //PADRE_FINADO = s.IMPUTADO.PADRE_FINADO,
        //                PAIS_NACIONALIDAD = s.IMPUTADO.PAIS_NACIONALIDAD,
        //                //PAIS_NACIONALIDAD1 = s.IMPUTADO.PAIS_NACIONALIDAD1,
        //                PATERNO_MADRE = s.IMPUTADO.PATERNO_MADRE,
        //                PATERNO_PADRE = s.IMPUTADO.PATERNO_PADRE,
        //                //PESO = s.IMPUTADO.PESO,
        //                RELACION_PERSONAL_INTERNO = s.IMPUTADO.RELACION_PERSONAL_INTERNO,
        //                //RELIGION = s.IMPUTADO.RELIGION,
        //                //RESIDENCIA_ANIOS = s.IMPUTADO.RESIDENCIA_ANIOS,
        //                //RESIDENCIA_MESES = s.IMPUTADO.RESIDENCIA_MESES,
        //                RFC = s.IMPUTADO.RFC,
        //                SENAS_PARTICULARES = s.IMPUTADO.SENAS_PARTICULARES,
        //                SEXO = s.IMPUTADO.SEXO,
        //                SMATERNO = s.IMPUTADO.SMATERNO,
        //                SNOMBRE = s.IMPUTADO.SNOMBRE,
        //                SPATERNO = s.IMPUTADO.SPATERNO,
        //                TABAJADOR_CERESO = s.IMPUTADO.TABAJADOR_CERESO,
        //                //TELEFONO = s.IMPUTADO.TELEFONO,
        //                TELORIGINAL = s.IMPUTADO.TELORIGINAL,
        //                //TIPO_DISCAPACIDAD = s.IMPUTADO.TIPO_DISCAPACIDAD,
        //                TRADUCTOR = s.IMPUTADO.TRADUCTOR,
        //                PRS_ENTREVISTA_INICIAL = s.IMPUTADO.PRS_ENTREVISTA_INICIAL,
        //                EMI_IMPUTADO = s.IMPUTADO.EMI_IMPUTADO,
        //                LIBERADO = s.IMPUTADO.LIBERADO,
        //                PRS_FICHA_IDEN = s.IMPUTADO.PRS_FICHA_IDEN,
        //                PRS_REPORTE_PSICOLOGICO = s.IMPUTADO.PRS_REPORTE_PSICOLOGICO,
        //                PRS_VISITA_DOMICILIARIA = s.IMPUTADO.PRS_VISITA_DOMICILIARIA,
        //                PROCESO_LIBERTAD = s.IMPUTADO.PROCESO_LIBERTAD,

        //            })
        //            .DistinctBy(d => new { d.ID_CENTRO, d.ID_ANIO, d.ID_IMPUTADO })
        //            .ToList()
        //            ;
        //        #endregion
        //        return new ObservableCollection<IMPUTADO>(union); //null;//
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message);
        //    }
        //   }


        
        
        public ObservableCollection<IMPUTADO> ObtenerImputadosXAbogadoIngreso(string ApellidoPaternoBuscar = "", string ApellidoMaternoBuscar = "", string NombreBuscar = "", int? Anio = 0, int? Folio = 0, int Pagina = 1, PERSONA abogado = null)
        {
            try
            {
                NombreBuscar = NombreBuscar ?? string.Empty;
                ApellidoPaternoBuscar = ApellidoPaternoBuscar ?? string.Empty;
                ApellidoMaternoBuscar = ApellidoMaternoBuscar ?? string.Empty;

                if (((Anio.HasValue && Folio.HasValue) == false) && (ApellidoPaternoBuscar.Trim().Length < 2 && ApellidoMaternoBuscar.Trim().Length < 2 && NombreBuscar.Trim().Length < 2))
                    return new ObservableCollection<IMPUTADO>();
                var PredicadoAND1 = PredicateBuilder.True<IMPUTADO>();
                var PredicadoAND2 = PredicateBuilder.True<ALIAS>();
                var PredicadoAND3 = PredicateBuilder.True<APODO>();

                if (!string.IsNullOrWhiteSpace(NombreBuscar.Trim()))
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.NOMBRE.Contains(NombreBuscar.Trim()));
                    PredicadoAND2 = PredicadoAND2.And(w => w.NOMBRE.Contains(NombreBuscar.Trim()));
                    PredicadoAND3 = PredicadoAND3.And(w => w.APODO1.Contains(NombreBuscar.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(ApellidoPaternoBuscar.Trim()))
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.PATERNO.Contains(ApellidoPaternoBuscar.Trim()));
                    PredicadoAND2 = PredicadoAND2.And(w => w.PATERNO.Contains(ApellidoPaternoBuscar.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(ApellidoMaternoBuscar.Trim()))
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.MATERNO.Contains(ApellidoMaternoBuscar.Trim()));
                    PredicadoAND2 = PredicadoAND2.And(w => w.MATERNO.Contains(ApellidoMaternoBuscar.Trim()));
                }

                if (Anio.HasValue)
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.ID_ANIO == Anio.Value);
                    PredicadoAND2 = PredicadoAND2.And(w => w.ID_ANIO == Anio.Value);
                    PredicadoAND3 = PredicadoAND3.And(w => w.ID_ANIO == Anio.Value);
                }

                if (Folio.HasValue)
                {
                    PredicadoAND1 = PredicadoAND1.And(w => w.ID_IMPUTADO == Folio.Value);
                    PredicadoAND2 = PredicadoAND2.And(w => w.ID_IMPUTADO == Folio.Value);
                    PredicadoAND3 = PredicadoAND3.And(w => w.ID_IMPUTADO == Folio.Value);
                }

                

                PredicadoAND1 = PredicadoAND1.And(w=>w.INGRESO.Any(a1=>a1.ABOGADO_INGRESO.Any(a2=>a2.ID_ABOGADO==abogado.ABOGADO.ID_ABOGADO && a2.ID_ABOGADO_TITULO==abogado.ABOGADO.ID_ABOGADO_TITULO)));
                PredicadoAND2 = PredicadoAND2.And(w => w.IMPUTADO.INGRESO.Any(a1 => a1.ABOGADO_INGRESO.Any(a2 => a2.ID_ABOGADO == abogado.ABOGADO.ID_ABOGADO && a2.ID_ABOGADO_TITULO == abogado.ABOGADO.ID_ABOGADO_TITULO)));
                PredicadoAND3 = PredicadoAND3.And(w => w.IMPUTADO.INGRESO.Any(a1 => a1.ABOGADO_INGRESO.Any(a2 => a2.ID_ABOGADO == abogado.ABOGADO.ID_ABOGADO && a2.ID_ABOGADO_TITULO == abogado.ABOGADO.ID_ABOGADO_TITULO)));

                var query = Context.IMPUTADO.Where(PredicadoAND1.Expand()).AsEnumerable().Select(s => s).Union(Context.Aliases.Where(PredicadoAND2.Expand()).AsEnumerable().Select(s => new IMPUTADO
                {
                    NOMBRE = (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrWhiteSpace(NombreBuscar) ? s.NOMBRE.Trim() : NombreBuscar) ?
                    s.IMPUTADO.NOMBRE.Trim()
                    :
                    s.IMPUTADO.NOMBRE.Trim() + (string.IsNullOrWhiteSpace(NombreBuscar.Trim()) ? "" : " (" + s.NOMBRE.Trim() + ")"),
                    PATERNO = (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim().Equals(string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) ? (s.PATERNO = s.PATERNO ?? "").Trim() : ApellidoPaternoBuscar.Trim())
                    ?
                    (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim()
                    :
                    (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim() + (string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) ? string.Empty : " (" + (s.PATERNO = s.PATERNO ?? "").Trim() + ")"),
                    MATERNO = (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim().Equals(string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) ? (s.MATERNO = s.MATERNO ?? "").Trim() : ApellidoMaternoBuscar.Trim())
                    ?
                    (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim()
                    :
                    (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim() + (string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) ? string.Empty : " (" + (s.MATERNO = s.MATERNO ?? "").Trim() + ")"),
                    ID_ANIO = s.ID_ANIO,
                    ID_CENTRO = s.ID_CENTRO,
                    ID_IMPUTADO = s.ID_IMPUTADO,
                    CENTRO = s.IMPUTADO.CENTRO
                }));
                if (!string.IsNullOrWhiteSpace(NombreBuscar) && (string.IsNullOrWhiteSpace(ApellidoPaternoBuscar) && string.IsNullOrWhiteSpace(ApellidoMaternoBuscar) && !Anio.HasValue && !Folio.HasValue))
                {
                    query = query.Union(
                        Context.APODOes.Where(PredicadoAND3.Expand()).AsEnumerable().Select(s => new IMPUTADO
                        {
                            NOMBRE = (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrWhiteSpace(NombreBuscar) ? (s.APODO1 = s.APODO1 ?? "").Trim() : NombreBuscar.Trim())
                            ?
                            (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
                            :
                            (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim() + (string.IsNullOrWhiteSpace(NombreBuscar) ? string.Empty : " (" + (s.APODO1 = s.APODO1 ?? "").Trim() + ")"),
                            PATERNO = "",
                            MATERNO = "",
                            ID_ANIO = s.ID_ANIO,
                            ID_CENTRO = s.ID_CENTRO,
                            ID_IMPUTADO = s.ID_IMPUTADO,
                            CENTRO = s.IMPUTADO.CENTRO
                        }));
                }

                return new ObservableCollection<IMPUTADO>((query)
                .DistinctBy(d => new { d.ID_ANIO, d.ID_CENTRO, d.ID_IMPUTADO })
                    .OrderBy(o => o.PATERNO).ThenBy(o => o.MATERNO).ThenBy(o => o.NOMBRE)
                    .Take((Pagina * 30))
                    .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)))
                );
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        //public ObservableCollection<IMPUTADO> ObtenerImputadosXAbogadoIngreso(string ApellidoPaternoBuscar = "", string ApellidoMaternoBuscar = "", string NombreBuscar = "", int? Anio = 0, int? Folio = 0, int Pagina = 1, PERSONA abogado = null)
        //{
        //    if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !Anio.HasValue && !Folio.HasValue)
        //        return new ObservableCollection<IMPUTADO>();

        //    NombreBuscar = NombreBuscar ?? string.Empty;
        //    ApellidoPaternoBuscar = ApellidoPaternoBuscar ?? string.Empty;
        //    ApellidoMaternoBuscar = ApellidoMaternoBuscar ?? string.Empty;

        //    if (((Anio.HasValue && Folio.HasValue) == false) && (ApellidoPaternoBuscar.Trim().Length < 2 && ApellidoMaternoBuscar.Trim().Length < 2 && NombreBuscar.Trim().Length < 2))
        //        return new ObservableCollection<IMPUTADO>();

        //    var PredicadoAND = PredicateBuilder.True<unionimputado>();

        //    //PredicadoAND = PredicadoAND.And(w => w.ABOGADO_INGRESO.Where(whe => whe.ABOGADO_INGRESO.Where(wh => wh.ID_ABOGADO == abogado).Any()).Any());
        //    if (!string.IsNullOrEmpty(NombreBuscar.Trim()))
        //        PredicadoAND = PredicadoAND.And(w => w.NOMBREAPODO.Contains(NombreBuscar.Trim()));
        //    if (!string.IsNullOrEmpty(ApellidoPaternoBuscar.Trim()))
        //        PredicadoAND = PredicadoAND.And(w => w.PATERNO.Contains(ApellidoPaternoBuscar.Trim()));
        //    if (!string.IsNullOrEmpty(ApellidoMaternoBuscar.Trim()))
        //        PredicadoAND = PredicadoAND.And(w => w.MATERNO.Contains(ApellidoMaternoBuscar.Trim()));

        //    if (Anio.HasValue)
        //        PredicadoAND = PredicadoAND.And(w => w.AÑO == Anio);
        //    if (Folio.HasValue)
        //        PredicadoAND = PredicadoAND.And(w => w.FOLIO == Folio);

        //    var context = new SSPEntidades();
        //    context.Conexion();
        //    var union = context.IMPUTADO.Select(s => new unionimputado
        //    {
        //        CENTRO = s.ID_CENTRO,
        //        AÑO = s.ID_ANIO,
        //        FOLIO = s.ID_IMPUTADO,
        //        NOMBREAPODO = s.NOMBRE.Trim(),
        //        PATERNO = s.PATERNO.Trim(),
        //        MATERNO = s.MATERNO.Trim(),
        //        //ABOGADO_INGRESO =s.INGRESO
        //    })
        //    .Union(context.Aliases.Select(s => new unionimputado
        //    {
        //        CENTRO = s.ID_CENTRO,
        //        AÑO = s.ID_ANIO,
        //        FOLIO = s.ID_IMPUTADO,
        //        NOMBREAPODO = s.NOMBRE.Trim(),
        //        PATERNO = s.PATERNO.Trim(),
        //        MATERNO = s.MATERNO.Trim(),
        //        //ABOGADO_INGRESO =s.IMPUTADO.INGRESO
        //    }))
        //    .Union(context.APODOes.Select(s => new unionimputado
        //    {
        //        CENTRO = s.ID_CENTRO,
        //        AÑO = s.ID_ANIO,
        //        FOLIO = s.ID_IMPUTADO,
        //        NOMBREAPODO = s.APODO1.Trim(),
        //        PATERNO = "",
        //        MATERNO = "",
        //        //ABOGADO_INGRESO =s.IMPUTADO.INGRESO
        //    }))
        //    .AsExpandable()
        //    .Where(PredicadoAND)
        //    .OrderBy(o => new { o.PATERNO, o.MATERNO, o.NOMBREAPODO })
        //    .Take((Pagina * 30))
        //    .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)))
        //    .Select(s => new
        //    {
        //        IMPUTADO = context.IMPUTADO.Where(w => w.ID_CENTRO == s.CENTRO && w.ID_ANIO == s.AÑO && w.ID_IMPUTADO == s.FOLIO).FirstOrDefault(),
        //        APODO = context.IMPUTADO.Where(w => w.ID_CENTRO == s.CENTRO && w.ID_ANIO == s.AÑO && w.ID_IMPUTADO == s.FOLIO).FirstOrDefault().APODO.Where(w => w.APODO1.Trim().Equals(s.NOMBREAPODO)).FirstOrDefault(),
        //        ALIAS = context.IMPUTADO.Where(w => w.ID_CENTRO == s.CENTRO && w.ID_ANIO == s.AÑO && w.ID_IMPUTADO == s.FOLIO).FirstOrDefault().ALIAS.Where(w => w.NOMBRE.Trim().Equals(s.NOMBREAPODO) && w.MATERNO.Trim().Equals(s.MATERNO) && w.PATERNO.Trim().Equals(s.PATERNO)).FirstOrDefault()
        //    })
        //    .Where(w => w.IMPUTADO.INGRESO.Where(wh => wh.ABOGADO_INGRESO.Where(whe => whe.ID_ABOGADO == abogado.ABOGADO.ID_ABOGADO && whe.ID_ABOGADO_TITULO == abogado.ABOGADO.ID_ABOGADO_TITULO).Any()).Any())
        //    .AsEnumerable()
        //    .DistinctBy(d => new { d.IMPUTADO.ID_CENTRO, d.IMPUTADO.ID_ANIO, d.IMPUTADO.ID_IMPUTADO })
        //    .Select(s => new IMPUTADO
        //    {
        //        NOMBRE =
        //        (s.ALIAS == null && s.APODO == null)
        //            ?
        //            (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
        //            :
        //            s.ALIAS != null
        //                ?
        //                (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrEmpty(NombreBuscar) ? s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE.Trim() : NombreBuscar.Trim())
        //                    ?
        //                    (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
        //                    :
        //                    (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim() + (string.IsNullOrEmpty(NombreBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE = s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE ?? "").Trim() + ")")
        //                :
        //                s.APODO != null
        //                    ?
        //                    (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim().Equals(string.IsNullOrEmpty(NombreBuscar) ? s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().NOMBRE.Trim() : NombreBuscar.Trim())
        //                        ?
        //                        (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim()
        //                        :
        //                        (s.IMPUTADO.NOMBRE = s.IMPUTADO.NOMBRE ?? "").Trim() + (string.IsNullOrEmpty(NombreBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.APODO.Where(w => w == s.APODO).FirstOrDefault().APODO1 = s.IMPUTADO.APODO.Where(w => w == s.APODO).FirstOrDefault().APODO1 ?? "").Trim() + ")")
        //                    :
        //                    string.Empty,

        //        PATERNO =
        //        s.ALIAS == null
        //            ?
        //            (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim()
        //            :
        //            s.ALIAS != null
        //                ?
        //                (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim().Equals(string.IsNullOrEmpty(ApellidoPaternoBuscar.Trim()) ? s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().PATERNO.Trim() : ApellidoPaternoBuscar.Trim())
        //                    ?
        //                    (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim()
        //                    :
        //                    (s.IMPUTADO.PATERNO = s.IMPUTADO.PATERNO ?? "").Trim() + (string.IsNullOrEmpty(ApellidoPaternoBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().PATERNO = s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().PATERNO ?? "").Trim() + ")")
        //                :
        //                string.Empty,

        //        MATERNO =
        //        s.ALIAS == null
        //            ?
        //            (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim()
        //            :
        //            s.ALIAS != null
        //                ?
        //                (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim().Equals(string.IsNullOrEmpty(ApellidoMaternoBuscar.Trim()) ? (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().MATERNO ?? "").Trim() : ApellidoMaternoBuscar.Trim())
        //                    ?
        //                    (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim()
        //                    :
        //                    (s.IMPUTADO.MATERNO = s.IMPUTADO.MATERNO ?? "").Trim() + (string.IsNullOrEmpty(ApellidoMaternoBuscar.Trim()) ? string.Empty : " (" + (s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().MATERNO = s.IMPUTADO.ALIAS.Where(w => w == s.ALIAS).FirstOrDefault().MATERNO ?? "").Trim() + ")")
        //                :
        //                string.Empty,

        //        CENTRO = s.IMPUTADO.CENTRO,
        //        ID_ANIO = s.IMPUTADO.ID_ANIO,
        //        ID_IMPUTADO = s.IMPUTADO.ID_IMPUTADO,

        //        INGRESO = s.IMPUTADO.INGRESO,
        //        ALIAS = s.IMPUTADO.ALIAS,
        //        APODO = s.IMPUTADO.APODO,
        //        BI_FECALTA = s.IMPUTADO.BI_FECALTA,
        //        BI_USER = s.IMPUTADO.BI_USER,
        //        //COLONIA = s.IMPUTADO.COLONIA,
        //        CURP = s.IMPUTADO.CURP,
        //        DIALECTO = s.IMPUTADO.DIALECTO,
        //        //DOMICILIO_CALLE = s.IMPUTADO.DOMICILIO_CALLE,
        //        //DOMICILIO_CODIGO_POSTAL = s.IMPUTADO.DOMICILIO_CODIGO_POSTAL,
        //        //DOMICILIO_NUM_EXT = s.IMPUTADO.DOMICILIO_NUM_EXT,
        //        //DOMICILIO_NUM_INT = s.IMPUTADO.DOMICILIO_NUM_INT,
        //        //DOMICILIO_TRABAJO = s.IMPUTADO.DOMICILIO_TRABAJO,
        //        //ENTIDAD = s.IMPUTADO.ENTIDAD,
        //        //ESCOLARIDAD = s.IMPUTADO.ESCOLARIDAD,
        //        //ESTADO_CIVIL = s.IMPUTADO.ESTADO_CIVIL,
        //        //ESTATURA = s.IMPUTADO.ESTATURA,
        //        ETNIA = s.IMPUTADO.ETNIA,
        //        FAMILIAR_RESPONSABLE = s.IMPUTADO.FAMILIAR_RESPONSABLE,
        //        ID_CENTRO = s.IMPUTADO.ID_CENTRO,
        //        //ID_COLONIA = s.IMPUTADO.ID_COLONIA,
        //        ID_DIALECTO = s.IMPUTADO.ID_DIALECTO,
        //        //ID_ENTIDAD = s.IMPUTADO.ID_ENTIDAD,
        //        //ID_ESCOLARIDAD = s.IMPUTADO.ID_ESCOLARIDAD,
        //        //ID_ESTADO_CIVIL = s.IMPUTADO.ID_ESTADO_CIVIL,
        //        ID_ETNIA = s.IMPUTADO.ID_ETNIA,
        //        ID_IDIOMA = s.IMPUTADO.ID_IDIOMA,
        //        //ID_MUNICIPIO = s.IMPUTADO.ID_MUNICIPIO,
        //        ID_NACIONALIDAD = s.IMPUTADO.ID_NACIONALIDAD,
        //        //ID_OCUPACION = s.IMPUTADO.ID_OCUPACION,
        //        //ID_PAIS = s.IMPUTADO.ID_PAIS,
        //        //ID_RELIGION = s.IMPUTADO.ID_RELIGION,
        //        //ID_TIPO_DISCAPACIDAD = s.IMPUTADO.ID_TIPO_DISCAPACIDAD,
        //        IDIOMA = s.IMPUTADO.IDIOMA,
        //        IFE = s.IMPUTADO.IFE,
        //        IMPUTADO_BIOMETRICO = s.IMPUTADO.IMPUTADO_BIOMETRICO,
        //        //IMPUTADO_DOCUMENTO = s.IMPUTADO.IMPUTADO_DOCUMENTO,
        //        IMPUTADO_FILIACION = s.IMPUTADO.IMPUTADO_FILIACION,
        //        IMPUTADO_FORMATO = s.IMPUTADO.IMPUTADO_FORMATO,
        //        IMPUTADO_PADRES = s.IMPUTADO.IMPUTADO_PADRES,
        //        IMPUTADO_PANDILLA = s.IMPUTADO.IMPUTADO_PANDILLA,
        //        //LUGAR_RESIDENCIA = s.IMPUTADO.LUGAR_RESIDENCIA,
        //        //MADRE_FINADO = s.IMPUTADO.MADRE_FINADO,
        //        MATERNO_MADRE = s.IMPUTADO.MATERNO_MADRE,
        //        MATERNO_PADRE = s.IMPUTADO.MATERNO_PADRE,
        //        //MUNICIPIO = s.IMPUTADO.MUNICIPIO,
        //        NACIMIENTO_ESTADO = s.IMPUTADO.NACIMIENTO_ESTADO,
        //        NACIMIENTO_FECHA = s.IMPUTADO.NACIMIENTO_FECHA,
        //        NACIMIENTO_LUGAR = s.IMPUTADO.NACIMIENTO_LUGAR,
        //        NACIMIENTO_MUNICIPIO = s.IMPUTADO.NACIMIENTO_MUNICIPIO,
        //        NACIMIENTO_PAIS = s.IMPUTADO.NACIMIENTO_PAIS,
        //        NIP = s.IMPUTADO.NIP,
        //        NOMBRE_MADRE = s.IMPUTADO.NOMBRE_MADRE,
        //        NOMBRE_PADRE = s.IMPUTADO.NOMBRE_PADRE,
        //        //NUMERO_IDENTIFICACION = s.IMPUTADO.NUMERO_IDENTIFICACION,
        //        //OCUPACION = s.IMPUTADO.OCUPACION,
        //        //PADRE_FINADO = s.IMPUTADO.PADRE_FINADO,
        //        PAIS_NACIONALIDAD = s.IMPUTADO.PAIS_NACIONALIDAD,
        //        //PAIS_NACIONALIDAD1 = s.IMPUTADO.PAIS_NACIONALIDAD1,
        //        PATERNO_MADRE = s.IMPUTADO.PATERNO_MADRE,
        //        PATERNO_PADRE = s.IMPUTADO.PATERNO_PADRE,
        //        //PESO = s.IMPUTADO.PESO,
        //        RELACION_PERSONAL_INTERNO = s.IMPUTADO.RELACION_PERSONAL_INTERNO,
        //        //RELIGION = s.IMPUTADO.RELIGION,
        //        //RESIDENCIA_ANIOS = s.IMPUTADO.RESIDENCIA_ANIOS,
        //        //RESIDENCIA_MESES = s.IMPUTADO.RESIDENCIA_MESES,
        //        RFC = s.IMPUTADO.RFC,
        //        SENAS_PARTICULARES = s.IMPUTADO.SENAS_PARTICULARES,
        //        SEXO = s.IMPUTADO.SEXO,
        //        SMATERNO = s.IMPUTADO.SMATERNO,
        //        SNOMBRE = s.IMPUTADO.SNOMBRE,
        //        SPATERNO = s.IMPUTADO.SPATERNO,
        //        TABAJADOR_CERESO = s.IMPUTADO.TABAJADOR_CERESO,
        //        //TELEFONO = s.IMPUTADO.TELEFONO,
        //        TELORIGINAL = s.IMPUTADO.TELORIGINAL,
        //        //TIPO_DISCAPACIDAD = s.IMPUTADO.TIPO_DISCAPACIDAD,
        //        TRADUCTOR = s.IMPUTADO.TRADUCTOR

        //    })
        //    .ToList();

        //    return new ObservableCollection<IMPUTADO>(union);
        //}


        public ObservableCollection<IMPUTADO> ObtenerImputadosActivosAnio(string ApellidoPaternoBuscar = "", string ApellidoMaternoBuscar = "", string NombreBuscar = "", int Anio = 0, short estatusAdmvo = 4)
        {
            ObservableCollection<IMPUTADO> imputados = new ObservableCollection<IMPUTADO>();
            //BUSCA EN LA TABLA DE IMPUTADOS
            var Resultado = new List<IMPUTADO>();
            Resultado = GetData().Where(x =>
                ((x.PATERNO.Contains(ApellidoPaternoBuscar) && x.MATERNO.Contains(ApellidoMaternoBuscar) && x.NOMBRE.Contains(NombreBuscar)) ||
                (x.PATERNO.Contains(ApellidoMaternoBuscar) && x.MATERNO.Contains(ApellidoPaternoBuscar) && x.NOMBRE.Contains(NombreBuscar)) ||
                (x.ALIAS.Where(w => (w.PATERNO.Contains(ApellidoPaternoBuscar) && w.MATERNO.Contains(ApellidoMaternoBuscar) && w.NOMBRE.Contains(NombreBuscar))).Count() > 0) ||
                (x.APODO.Where(w => w.APODO1.Equals(NombreBuscar)).Count() > 0)) && x.INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != estatusAdmvo).Any() && x.ID_ANIO == Anio
                ).ToList();
            if (Resultado.Count > 0)
                imputados = new ObservableCollection<IMPUTADO>(Resultado.DistinctBy(w => new { w.ID_CENTRO, w.ID_ANIO, w.ID_IMPUTADO }).ToList());
            else
                imputados = new ObservableCollection<IMPUTADO>(Resultado);
            return imputados;
        }
        public ObservableCollection<IMPUTADO> ObtenerImputadosActivosFolio(string ApellidoPaternoBuscar = "", string ApellidoMaternoBuscar = "", string NombreBuscar = "", int Folio = 0, short estatusAdmvo = 4)
        {
            ObservableCollection<IMPUTADO> imputados = new ObservableCollection<IMPUTADO>();
            //BUSCA EN LA TABLA DE IMPUTADOS
            var Resultado = new List<IMPUTADO>();
            Resultado = GetData().Where(x =>
                ((x.PATERNO.Contains(ApellidoPaternoBuscar) && x.MATERNO.Contains(ApellidoMaternoBuscar) && x.NOMBRE.Contains(NombreBuscar)) ||
                (x.PATERNO.Contains(ApellidoMaternoBuscar) && x.MATERNO.Contains(ApellidoPaternoBuscar) && x.NOMBRE.Contains(NombreBuscar)) ||
                (x.ALIAS.Where(w => (w.PATERNO.Contains(ApellidoPaternoBuscar) && w.MATERNO.Contains(ApellidoMaternoBuscar) && w.NOMBRE.Contains(NombreBuscar))).Count() > 0) ||
                (x.APODO.Where(w => w.APODO1.Equals(NombreBuscar)).Count() > 0)) && x.INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != estatusAdmvo).Any() && x.ID_IMPUTADO == Folio
                ).ToList();
            if (Resultado.Count > 0)
                imputados = new ObservableCollection<IMPUTADO>(Resultado.DistinctBy(w => new { w.ID_CENTRO, w.ID_ANIO, w.ID_IMPUTADO }).ToList());
            else
                imputados = new ObservableCollection<IMPUTADO>(Resultado);
            return imputados;
        }
       
        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "IMPUTADO"</returns>
        public IQueryable<IMPUTADO> Obtener(int Id, int Anio, int Centro)
        {
            try
            {
                return GetData().Where(w => w.ID_IMPUTADO == Id && w.ID_ANIO == Anio && w.ID_CENTRO == Centro);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        //public List<IMPUTADO> Obtener(int Id, int Anio, int Centro)
        //{
        //    var Resultado = new List<IMPUTADO>();
        //    try
        //    {
        //        Resultado = GetData().Where(w => w.ID_IMPUTADO == Id && w.ID_ANIO == Anio && w.ID_CENTRO == Centro).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message);
        //    }
        //    return Resultado;
        //}

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "IMPUTADO" con valores a insertar</param>
        public int Insertar(IMPUTADO Entity)
        {
            try
            {
                Entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");
                Insert(Entity);
                return Entity.ID_IMPUTADO;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "IMPUTADO" con valores a insertar</param>
        /// <param name="cama">objeto de tipo "CAMA" con valores a insertar</param>
        /// <param name="fecha_servidor">objeto de tipo "CAMA" con valores a insertar</param>
        public void InsertarIngresoExisteImputado(IMPUTADO entity, CAMA cama, short id_area_sala_cabos, DateTime fecha_servidor)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    foreach (var item in entity.IMPUTADO_PADRES)
                    {
                        if (Context.IMPUTADO_PADRES.Any(a => a.ID_ANIO == item.ID_ANIO && a.ID_CENTRO == item.ID_CENTRO && a.ID_IMPUTADO == item.ID_IMPUTADO && a.ID_PADRE == item.ID_PADRE))
                            Context.Entry(item).State = EntityState.Modified;
                        else
                            Context.IMPUTADO_PADRES.Add(item);
                    }
                    Context.IMPUTADO_PADRES = null;


                    var ultimo_id_apodo = Context.APODOes.Where(w => w.ID_ANIO == entity.ID_ANIO && w.ID_CENTRO == entity.ID_CENTRO && w.ID_IMPUTADO == entity.ID_IMPUTADO).Select(s => s.ID_APODO).DefaultIfEmpty<short>(0).Max();
                    foreach (var item in entity.APODO)
                    {
                        item.ID_APODO = (short)(ultimo_id_apodo + item.ID_APODO);
                        Context.APODOes.Add(item);
                    }
                    entity.APODO = null;

                    var ultimo_id_alias = Context.Aliases.Where(w => w.ID_ANIO == entity.ID_ANIO && w.ID_CENTRO == entity.ID_CENTRO && w.ID_IMPUTADO == entity.ID_IMPUTADO).Select(s => s.ID_ALIAS).DefaultIfEmpty<short>(0).Max();
                    foreach (var item in entity.ALIAS)
                    {
                        item.ID_ALIAS = (short)(item.ID_ALIAS + ultimo_id_alias);
                        Context.Aliases.Add(item);
                    }
                    entity.ALIAS = null;
                    Context.SaveChanges();
                    foreach (var item in entity.RELACION_PERSONAL_INTERNO)
                        Context.RELACION_PERSONAL_INTERNO.Add(item);
                    Context.SaveChanges();

                    var huellas = Context.IMPUTADO_BIOMETRICO.Where(w => w.ID_CENTRO == entity.ID_CENTRO && w.ID_ANIO == entity.ID_ANIO && w.ID_IMPUTADO == entity.ID_IMPUTADO);
                    if (huellas != null)
                    {
                        foreach (var h in huellas)
                        {
                            Context.IMPUTADO_BIOMETRICO.Remove(h);
                        }
                    }
                    foreach (var item in entity.IMPUTADO_BIOMETRICO)
                    {
                        Context.IMPUTADO_BIOMETRICO.Add(item);//Entry(item).State = EntityState.Modified;
                    }
                    entity.IMPUTADO_BIOMETRICO = null;
                    Context.SaveChanges();
                    var ingreso = entity.INGRESO.Where(w => w.ID_INGRESO == 0).First();
                    ingreso.ID_INGRESO = (short)(Context.INGRESO.Where(w => w.ID_CENTRO == entity.ID_CENTRO && w.ID_IMPUTADO == entity.ID_IMPUTADO && w.ID_ANIO == entity.ID_ANIO).Select(s => s.ID_INGRESO).Max() + 1);
                    var traslado_detalle = ingreso.TRASLADO_DETALLE.FirstOrDefault();
                    if (traslado_detalle != null)
                    {
                        var id_traslado = GetSequence<short>("TRASLADO_SEQ");
                        traslado_detalle.ID_INGRESO = ingreso.ID_INGRESO;
                        traslado_detalle.ID_TRASLADO = id_traslado;
                        traslado_detalle.TRASLADO.ID_TRASLADO = id_traslado;
                    }
                    foreach (var item in ingreso.INGRESO_BIOMETRICO)
                    {
                        item.ID_INGRESO = ingreso.ID_INGRESO;
                        Context.INGRESO_BIOMETRICO.Add(item);
                    }
                    Context.INGRESO.Add(ingreso);
                    Context.Entry(entity).State = EntityState.Modified;
                    Context.Entry(cama).State = EntityState.Modified;
                    var consec_ingreso_ubicacion = GetIDProceso<int>("INGRESO_UBICACION", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", ingreso.ID_CENTRO,
                    ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO));
                    Context.INGRESO_UBICACION.Add(new INGRESO_UBICACION
                    {
                        ESTATUS = 2,
                        ID_ANIO = ingreso.ID_ANIO,
                        ID_AREA = id_area_sala_cabos,
                        ID_CENTRO = ingreso.ID_CENTRO,
                        ID_CONSEC = consec_ingreso_ubicacion,
                        ID_IMPUTADO = ingreso.ID_IMPUTADO,
                        ID_INGRESO = ingreso.ID_INGRESO,
                        MOVIMIENTO_FEC = fecha_servidor
                    });
                    Context.SaveChanges();
                    transaccion.Complete();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "IMPUTADO" con valores a insertar</param>
        /// <param name="cama">objeto de tipo "CAMA" con valores a insertar</param>
        /// <param name="fecha_servidor">objeto de tipo "CAMA" con valores a insertar</param>
        public int InsertarIngresoNuevoImputado(IMPUTADO entity, CAMA cama, short id_area_sala_cabos, DateTime fecha_servidor)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");

                    string centro = entity.ID_CENTRO.ToString().PadLeft(2, '0');
                    string imputado = entity.ID_IMPUTADO.ToString().PadLeft(6, '0');
                    entity.NIP = string.Format("1{0}{1}{2}", centro, entity.ID_ANIO, imputado);
                    //entity.NIP = string.Format("1{0:x2}{1}{0:x6}", entity.ID_CENTRO, entity.ID_ANIO, entity.ID_IMPUTADO);
                    foreach (var item in entity.IMPUTADO_FILIACION)
                        item.ID_IMPUTADO = entity.ID_IMPUTADO;
                    foreach (var item in entity.IMPUTADO_PADRES)
                        item.ID_IMPUTADO = entity.ID_IMPUTADO;
                    foreach (var item in entity.APODO)
                        item.ID_IMPUTADO = entity.ID_IMPUTADO;
                    foreach (var item in entity.ALIAS)
                        item.ID_IMPUTADO = entity.ID_IMPUTADO;
                    foreach (var item in entity.RELACION_PERSONAL_INTERNO)
                        item.ID_IMPUTADO = entity.ID_IMPUTADO;
                    foreach (var item in entity.IMPUTADO_BIOMETRICO)
                        item.ID_IMPUTADO = entity.ID_IMPUTADO;
                    var ingreso = entity.INGRESO.First();
                    ingreso.ID_IMPUTADO = entity.ID_IMPUTADO;
                    ingreso.ID_INGRESO = 1;
                    var traslado_detalle = ingreso.TRASLADO_DETALLE.FirstOrDefault();
                    if (traslado_detalle != null)
                    {
                        var id_traslado = GetSequence<short>("TRASLADO_SEQ");
                        traslado_detalle.ID_IMPUTADO = entity.ID_IMPUTADO;
                        traslado_detalle.ID_INGRESO = ingreso.ID_INGRESO;
                        traslado_detalle.ID_TRASLADO = id_traslado;
                        traslado_detalle.TRASLADO.ID_TRASLADO = id_traslado;
                    }
                    foreach (var item in ingreso.INGRESO_BIOMETRICO)
                    {
                        item.ID_IMPUTADO = entity.ID_IMPUTADO;
                        item.ID_INGRESO = ingreso.ID_INGRESO;
                    }
                    Context.IMPUTADO.Add(entity);
                    Context.Entry(cama).State = EntityState.Modified;
                    var consec_ingreso_ubicacion = GetIDProceso<int>("INGRESO_UBICACION", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", ingreso.ID_CENTRO,
                    ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO));
                    Context.INGRESO_UBICACION.Add(new INGRESO_UBICACION
                    {
                        ESTATUS = 2,
                        ID_ANIO = ingreso.ID_ANIO,
                        ID_AREA = id_area_sala_cabos,
                        ID_CENTRO = ingreso.ID_CENTRO,
                        ID_CONSEC = consec_ingreso_ubicacion,
                        ID_IMPUTADO = ingreso.ID_IMPUTADO,
                        ID_INGRESO = ingreso.ID_INGRESO,
                        MOVIMIENTO_FEC = fecha_servidor
                    });
                    Context.SaveChanges();
                    transaccion.Complete();
                }


                return entity.ID_IMPUTADO;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var ve in dbEx.EntityValidationErrors)
                    foreach (var e in ve.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Error", e.PropertyName, e.ErrorMessage);
                return 0;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));

            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "IMPUTADO" con valores a actualizar</param>
        public bool Actualizar(IMPUTADO Entity)
        {
            try
            {
                Update(Entity);
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro con la informacion de medidas judiciales
        /// </summary>
        /// <param name="Entity">objeto de tipo "IMPUTADO" con valores a actualizar</param>
        public bool ActualizarLiberado(IMPUTADO Entity, List<IMPUTADO_PADRES> Padres, List<ALIAS> Alias, List<APODO> Apodos, List<IMPUTADO_BIOMETRICO> Biometrico, LIBERADO Liberado, LIBERADO_MEDIDA_JUDICIAL MedidaJudicial)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = EntityState.Modified;
                    #region Padres
                    var padres = Context.IMPUTADO_PADRES.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO);
                    if (padres != null)
                    {
                        foreach (var p in padres)
                        {
                            Context.Entry(p).State = EntityState.Deleted;
                        }
                    }
                    foreach (var p in Padres)
                    {
                        p.ID_CENTRO = Entity.ID_CENTRO;
                        p.ID_ANIO = Entity.ID_ANIO;
                        p.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        Context.IMPUTADO_PADRES.Add(p);
                    }
                    #endregion
                    #region Alias
                    var alias = Context.Aliases.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO);
                    if (alias != null)
                    {
                        foreach (var a in alias)
                        {
                            Context.Entry(a).State = EntityState.Deleted;
                        }
                    }
                    foreach (var a in Alias)
                    {
                        a.ID_CENTRO = Entity.ID_CENTRO;
                        a.ID_ANIO = Entity.ID_ANIO;
                        a.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        Context.Aliases.Add(a);
                    }
                    #endregion
                    #region Apodos
                    var apodos = Context.APODOes.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO);
                    if (apodos != null)
                    {
                        foreach (var a in apodos)
                        {
                            Context.Entry(a).State = EntityState.Deleted;
                        }
                    }
                    foreach (var a in Apodos)
                    {
                        a.ID_CENTRO = Entity.ID_CENTRO;
                        a.ID_ANIO = Entity.ID_ANIO;
                        a.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        Context.APODOes.Add(a);
                    }
                    #endregion
                    #region Biometrico
                    if (Biometrico.Count > 0)
                    {
                        var biometrico = Context.IMPUTADO_BIOMETRICO.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO);
                        if (biometrico != null)
                        {
                            foreach (var b in biometrico)
                            {
                                Context.Entry(b).State = EntityState.Deleted;
                            }
                        }
                        foreach (var b in Biometrico)
                        {
                            b.ID_CENTRO = Entity.ID_CENTRO;
                            b.ID_ANIO = Entity.ID_ANIO;
                            b.ID_IMPUTADO = Entity.ID_IMPUTADO;
                            Context.IMPUTADO_BIOMETRICO.Add(b);
                        }
                    }
                    #endregion
                    #region Liberado
                    if (Liberado != null)
                    {
                        var liberado = Context.LIBERADO.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO).SingleOrDefault();
                        if (liberado != null)
                        {
                            //Liberado.ID_CENTRO = Entity.ID_CENTRO;
                            //Liberado.ID_ANIO = Entity.ID_ANIO;
                            //Liberado.ID_IMPUTADO = Entity.ID_IMPUTADO;
                            liberado.ID_OCUPACION = Liberado.ID_OCUPACION;
                            liberado.OCUPACION_LUGAR = Liberado.OCUPACION_LUGAR;
                            liberado.LUGAR_FRECUENTA = Liberado.LUGAR_FRECUENTA;
                            liberado.REGISTRO_FEC = Liberado.REGISTRO_FEC;
                            liberado.PAM_NOMBRE = Liberado.PAM_NOMBRE;
                            liberado.PAM_DOMICILIO = Liberado.PAM_DOMICILIO;
                            liberado.PAM_TEL_CELULAR = Liberado.PAM_TEL_CELULAR;
                            liberado.PAM_TEL_FIJO = Liberado.PAM_TEL_FIJO;
                            liberado.PAM_TIEMPO_CONOCE = Liberado.PAM_TIEMPO_CONOCE;
                            liberado.PAM_ID_TIPO_REFERENCIA = Liberado.PAM_ID_TIPO_REFERENCIA;
                            liberado.ACTITUD_GENERAL = Liberado.ACTITUD_GENERAL;
                            liberado.OBSERVACION = Liberado.OBSERVACION;
                            Context.Entry(liberado).State = EntityState.Modified;


                            if (MedidaJudicial.ID_ANIO == 0 && MedidaJudicial.ID_IMPUTADO == 0) //Nueva Medida Judicial
                            {
                                var mj = Liberado.LIBERADO_MEDIDA_JUDICIAL;
                                var i = GetIDProceso<short>("LIBERADO_MEDIDA_JUDICIAL", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO));
                                MedidaJudicial.ID_CENTRO = Entity.ID_CENTRO;
                                MedidaJudicial.ID_ANIO = Entity.ID_ANIO;
                                MedidaJudicial.ID_IMPUTADO = Entity.ID_IMPUTADO;
                                MedidaJudicial.ID_CONSEC = i;
                                Context.LIBERADO_MEDIDA_JUDICIAL.Add(MedidaJudicial);
                                //foreach (var m in mj)
                                //{
                                //    m.ID_CENTRO = Entity.ID_CENTRO;
                                //    m.ID_ANIO = Entity.ID_ANIO;
                                //    m.ID_IMPUTADO = Entity.ID_IMPUTADO;
                                //    m.ID_CONSEC = i;
                                //    Context.LIBERADO_MEDIDA_JUDICIAL.Add(m);
                                //    i++;
                                //}
                            }
                            else
                            {
                                MedidaJudicial.LIBERADO = null;
                                Context.Entry(MedidaJudicial).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            var mj = Liberado.LIBERADO_MEDIDA_JUDICIAL;
                            Liberado.ID_CENTRO = Entity.ID_CENTRO;
                            Liberado.ID_ANIO = Entity.ID_ANIO;
                            Liberado.ID_IMPUTADO = Entity.ID_IMPUTADO;
                            Context.LIBERADO.Add(Liberado);
                            var i = GetIDProceso<short>("LIBERADO_MEDIDA_JUDICIAL", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO));
                            MedidaJudicial.ID_CENTRO = Entity.ID_CENTRO;
                            MedidaJudicial.ID_ANIO = Entity.ID_ANIO;
                            MedidaJudicial.ID_IMPUTADO = Entity.ID_IMPUTADO;
                            MedidaJudicial.ID_CONSEC = i;
                            Context.LIBERADO_MEDIDA_JUDICIAL.Add(MedidaJudicial);
                            //foreach (var m in mj)
                            //{
                            //    m.ID_CENTRO = Entity.ID_CENTRO;
                            //    m.ID_ANIO = Entity.ID_ANIO;
                            //    m.ID_IMPUTADO = Entity.ID_IMPUTADO;
                            //    m.ID_CONSEC = i;
                            //    Context.LIBERADO_MEDIDA_JUDICIAL.Add(m);
                            //    i++;
                            //}
                        }
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_IMPUTADO == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void ActualizarImputadoAliasApodosRelaciones(IMPUTADO imputado)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var ultimo_id_apodo = Context.APODOes.Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO).Select(s => s.ID_APODO).DefaultIfEmpty<short>(0).Max();
                    foreach (var item in imputado.APODO)
                    {
                        item.ID_APODO = (short)(ultimo_id_apodo + item.ID_APODO);
                        Context.APODOes.Add(item);
                    }
                    var ultimo_id_alias = Context.Aliases.Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO).Select(s => s.ID_ALIAS).DefaultIfEmpty<short>(0).Max();
                    foreach (var item in imputado.ALIAS)
                    {
                        item.ID_ALIAS = (short)(item.ID_ALIAS + ultimo_id_alias);
                        Context.Aliases.Add(item);
                    }

                    foreach (var item in imputado.RELACION_PERSONAL_INTERNO)
                        Context.RELACION_PERSONAL_INTERNO.Add(item);
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void ActualizarImputadoHuellasyFotos(IMPUTADO imputado,List<int> ListaToma)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (Context.IMPUTADO_BIOMETRICO.Any(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO))
                    {
                        if (imputado.IMPUTADO_BIOMETRICO.Count > 0)
                        {
                            foreach (var item in imputado.IMPUTADO_BIOMETRICO)
                            {
                                Context.Entry(item).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            var biometricos = Context.IMPUTADO_BIOMETRICO.Where(w => w.ID_CENTRO == imputado.ID_CENTRO && w.ID_ANIO == imputado.ID_ANIO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && w.ID_FORMATO == 9 && (w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9));
                            if (biometricos != null)
                            {
                                foreach (var b in biometricos)
                                {
                                    if (ListaToma.Contains(b.ID_TIPO_BIOMETRICO))
                                        b.TOMA = "S";
                                    else
                                        b.TOMA = "N";
                                    Context.Entry(b).State = EntityState.Modified;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in imputado.IMPUTADO_BIOMETRICO)
                        {
                            Context.IMPUTADO_BIOMETRICO.Add(item);
                        }
                    }
                    var _id_ingreso = imputado.INGRESO.First().ID_INGRESO;
                    if (Context.INGRESO_BIOMETRICO.Any(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && w.ID_INGRESO == _id_ingreso))
                    {
                        foreach (var item in imputado.INGRESO.First().INGRESO_BIOMETRICO)
                        {
                            Context.Entry(item).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        foreach (var item in imputado.INGRESO.First().INGRESO_BIOMETRICO)
                        {
                            Context.INGRESO_BIOMETRICO.Add(item);
                        }
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void ActualizarImputadoDatosGeneralesyPadres(IMPUTADO imputado,INGRESO ingreso)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    foreach (var item in imputado.IMPUTADO_PADRES)
                    {
                        if (Context.IMPUTADO_PADRES.Any(a => a.ID_ANIO == item.ID_ANIO && a.ID_CENTRO == item.ID_CENTRO && a.ID_IMPUTADO == item.ID_IMPUTADO && a.ID_PADRE == item.ID_PADRE))
                            Context.Entry(item).State = EntityState.Modified;
                        else
                            Context.IMPUTADO_PADRES.Add(item);
                    }

                    #region Ingreso
                    if (ingreso != null)
                    {
                        Context.INGRESO.Attach(ingreso);
                        Context.Entry(ingreso).Property(x => x.ID_ESTADO_CIVIL).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_OCUPACION).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_ESCOLARIDAD).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_RELIGION).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.DOMICILIO_CALLE).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.DOMICILIO_NUM_EXT).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.DOMICILIO_NUM_INT).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_COLONIA).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_MUNICIPIO).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_ENTIDAD).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_PAIS).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.DOMICILIO_CP).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.RESIDENCIA_ANIOS).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.RESIDENCIAS_MESES).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.TELEFONO).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.DOMICILIO_TRABAJO).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.MADRE_FINADO).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.PADRE_FINADO).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ESTATURA).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.PESO).IsModified = true;
                    }
                    #endregion

                    Context.Entry(imputado).State = EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool ActualizarImputadoDatosGeneralesyPadres(IMPUTADO imputado, List<IMPUTADO_PADRES> padres,INGRESO ingreso)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(imputado).State = EntityState.Modified;
                    
                    #region Padres
                    var Padres = Context.IMPUTADO_PADRES.Where(w => w.ID_CENTRO == imputado.ID_CENTRO && w.ID_ANIO == imputado.ID_ANIO && w.ID_IMPUTADO == imputado.ID_IMPUTADO);
                    if (Padres != null)
                    {
                        foreach (var p in Padres)
                        {
                            Context.Entry(p).State = EntityState.Deleted;
                        }
                    }
                    foreach (var p in padres)
                    {
                        Context.IMPUTADO_PADRES.Add(p);
                    }
                    #endregion

                    #region Ingreso
                    if (ingreso != null)
                    {
                        Context.INGRESO.Attach(ingreso);
                        Context.Entry(ingreso).Property(x => x.ID_ESTADO_CIVIL).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_OCUPACION).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_ESCOLARIDAD).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_RELIGION).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.DOMICILIO_CALLE).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.DOMICILIO_NUM_EXT).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.DOMICILIO_NUM_INT).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_COLONIA).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_MUNICIPIO).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_ENTIDAD).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ID_PAIS).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.DOMICILIO_CP).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.RESIDENCIA_ANIOS).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.RESIDENCIAS_MESES).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.TELEFONO).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.DOMICILIO_TRABAJO).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.MADRE_FINADO).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.PADRE_FINADO).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.ESTATURA).IsModified = true;
                        Context.Entry(ingreso).Property(x => x.PESO).IsModified = true;
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
            return false;
        }

        public bool ActualizarImputadoAliasApodosRelaciones(IMPUTADO imputado, List<ALIAS> alias, List<APODO> apodos, List<RELACION_PERSONAL_INTERNO> relaciones)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    #region Alias
                    var Alias = Context.Aliases.Where(w => w.ID_CENTRO == imputado.ID_CENTRO && w.ID_ANIO == imputado.ID_ANIO && w.ID_IMPUTADO == imputado.ID_IMPUTADO);
                    if (Alias != null)
                    {
                        foreach (var a in Alias)
                        {
                            Context.Entry(a).State = EntityState.Deleted;
                        }
                    }
                    foreach (var a in alias)
                    {
                        Context.Aliases.Add(a);
                    }
                    #endregion

                    #region Apodos
                    var Apodos = Context.APODOes.Where(w => w.ID_CENTRO == imputado.ID_CENTRO && w.ID_ANIO == imputado.ID_ANIO && w.ID_IMPUTADO == imputado.ID_IMPUTADO);
                    if (Apodos != null)
                    {
                        foreach (var a in Apodos)
                        {
                            Context.Entry(a).State = EntityState.Deleted;
                        }
                    }
                    foreach (var a in apodos)
                    {
                        Context.APODOes.Add(a);
                    }
                    #endregion

                    #region Relaciones
                    var Relaciones = Context.RELACION_PERSONAL_INTERNO.Where(w => w.ID_CENTRO == imputado.ID_CENTRO && w.ID_ANIO == imputado.ID_ANIO && w.ID_IMPUTADO == imputado.ID_IMPUTADO);
                    if (Relaciones != null)
                    {
                        foreach (var o in Relaciones)
                        {
                            Context.Entry(o).State = EntityState.Deleted;
                        }
                    }
                    foreach (var o in relaciones)
                    {
                        Context.RELACION_PERSONAL_INTERNO.Add(o);
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
            return false;
        }


        public IEnumerable<cLiberados> ObtenerLiberados(string NUC = "",short? CPAnio = null,int? CPFolio = null, string Nombre = "", string Paterno = "", string Materno = "",int Pagina = 1)
        {
            try
            {
                if(!string.IsNullOrEmpty(NUC))
                NUC = NUC.Replace("'", string.Empty);
                if (!string.IsNullOrEmpty(Nombre))
                Nombre = Nombre.Replace("'", string.Empty);
                if (!string.IsNullOrEmpty(Paterno))
                Paterno = Paterno.Replace("'", string.Empty);
                if (!string.IsNullOrEmpty(Materno))
                Materno = Materno.Replace("'", string.Empty);
                string filtros = string.Empty;
                string query = "SELECT * FROM ( " +
                "SELECT " +
                "IMP.ID_CENTRO,IMP.ID_ANIO,IMP.ID_IMPUTADO, " +
                "C.DESCR AS CENTRO, " + 
                "TRIM(IMP.PATERNO) PATERNO,TRIM(IMP.MATERNO) MATERNO,TRIM(IMP.NOMBRE) NOMBRE " +
                "FROM SSP.IMPUTADO IMP " +
                "INNER JOIN SSP.CENTRO C ON IMP.ID_CENTRO = C.ID_CENTRO "+
                "LEFT JOIN SSP.ALIAS A ON IMP.ID_CENTRO = A.ID_CENTRO AND IMP.ID_ANIO = A.ID_ANIO AND IMP.ID_IMPUTADO = A.ID_IMPUTADO " +
                "LEFT JOIN SSP.APODO AP ON IMP.ID_CENTRO = AP.ID_CENTRO AND IMP.ID_ANIO = AP.ID_ANIO AND IMP.ID_IMPUTADO = AP.ID_IMPUTADO " +
                "LEFT JOIN SSP.PROCESO_LIBERTAD PL ON IMP.ID_CENTRO = PL.ID_CENTRO AND IMP.ID_ANIO = PL.ID_ANIO AND IMP.ID_IMPUTADO = PL.ID_IMPUTADO " +
                "WHERE ";
                //NOMBRE
                if (!string.IsNullOrEmpty(Nombre))
                {
                    //filtros = filtros + string.Format(" IMP.NOMBRE LIKE '%{0}%' ", Nombre);
                    //filtros = filtros + string.Format(" OR A.NOMBRE LIKE '%{0}%' ", Nombre);
                    //filtros = filtros + string.Format(" OR AP.APODO LIKE '%{0}%' ", Nombre);
                    filtros = filtros + string.Format(" (IMP.NOMBRE LIKE '%{0}%' OR A.NOMBRE LIKE '%{0}%' OR AP.APODO LIKE '%{0}%') ", Nombre);
                }
                //PATERNO
                if (!string.IsNullOrEmpty(Paterno))
                {
                    if (!string.IsNullOrEmpty(filtros))
                        filtros = filtros + " AND ";
                    //filtros = filtros + string.Format(" (IMP.PATERNO LIKE '%{0}%' OR IMP.PATERNO LIKE '%{0}%') ", Paterno);
                    //filtros = filtros + string.Format(" OR (A.PATERNO LIKE '%{0}%' OR A.PATERNO LIKE '%{0}%') ", Paterno);
                    filtros = filtros + string.Format(" ((IMP.PATERNO LIKE '%{0}%') OR (A.PATERNO LIKE '%{0}%'))", Paterno);
                }
                //MATERNO
                if (!string.IsNullOrEmpty(Materno))
                {
                    if (!string.IsNullOrEmpty(filtros))
                        filtros = filtros + " AND ";
                    //filtros = filtros + string.Format(" (IMP.MATERNO LIKE '%{0}%' OR IMP.MATERNO LIKE '%{0}%')", Materno);
                    //filtros = filtros + string.Format(" OR (IMP.MATERNO LIKE '%{0}%' OR IMP.MATERNO LIKE '%{0}%')", Materno);
                    filtros = filtros + string.Format(" ((IMP.MATERNO LIKE '%{0}%') OR (IMP.MATERNO LIKE '%{0}%'))", Materno);
                }
                //NUC
                if (!string.IsNullOrEmpty(NUC))
                {
                    if (!string.IsNullOrEmpty(filtros))
                        filtros = filtros + " AND ";
                    filtros = filtros +  string.Format("PL.NUC LIKE '%{0}%'",NUC);
                }
                //Causa Penal Anio
                if (CPAnio.HasValue)
                {
                    if (!string.IsNullOrEmpty(filtros))
                        filtros = filtros + " AND ";
                    filtros = filtros + string.Format("PL.CP_ANIO = {0}", CPAnio);
                }
                //Causa Penal Anio
                if (CPFolio.HasValue)
                {
                    if (!string.IsNullOrEmpty(filtros))
                        filtros = filtros + " AND ";
                    filtros = filtros + string.Format("PL.CP_FOLIO = {0}", CPFolio);
                }
                query = query + filtros;
                query = query + " GROUP BY IMP.ID_CENTRO,IMP.ID_ANIO,IMP.ID_IMPUTADO,C.DESCR, " +
                "IMP.PATERNO,IMP.MATERNO,IMP.NOMBRE "+
                ") T, "+
                String.Format("TABLE (SSP.FUNC_ALIAS_APODO(T.ID_CENTRO,T.ID_ANIO,T.ID_IMPUTADO,'{0}','{1}','{2}')) F",Paterno,Materno,Nombre);
                return Context.Database.SqlQuery<cLiberados>(query);
                
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IEnumerable<cTerceraEdad> ObtenerInternosTerceraEdad(short Centro)
        {
            try
            {
                var query = new StringBuilder();
                query.Append("SELECT ");
                query.Append("T.ORDEN, ");
                query.Append("T.RANGO_EDAD, ");
                query.Append("COUNT(CASE WHEN T.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO = 'C' AND T.SEXO = 'M' AND N.ID_NUC IS NOT NULL THEN 1 END) AS M_IMPUTADO_COMUN, ");
                query.Append("COUNT(CASE WHEN T.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO = 'C' AND T.SEXO = 'M' AND N.ID_NUC IS NULL THEN 1 END) AS M_INDICIADO_COMUN, ");
                query.Append("COUNT(CASE WHEN T.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO = 'C' AND T.SEXO = 'M' THEN 1 END) AS M_PROCESADO_COMUN, ");
                query.Append("COUNT(CASE WHEN T.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO = 'F' AND T.SEXO = 'F' THEN 1 END) AS F_PROCESADO_FEDERAL, ");
                query.Append("COUNT(CASE WHEN T.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO = 'F' AND T.SEXO = 'M' THEN 1 END) AS M_PROCESADO_FEDERAL, ");
                query.Append("COUNT(CASE WHEN T.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO IS NULL AND T.SEXO = 'M' THEN 1 END) AS M_PROCESADO_SIN_FUERO, ");
                query.Append("COUNT(CASE WHEN T.ID_CLASIFICACION_JURIDICA = '3' AND CP.CP_FUERO = 'C' AND T.SEXO = 'M' THEN 1 END) AS M_SENTENCIADO_COMUN, ");
                query.Append("COUNT(CASE WHEN T.ID_CLASIFICACION_JURIDICA = '3' AND CP.CP_FUERO = 'F' AND T.SEXO = 'F' THEN 1 END) AS F_SENTENCIADO_FEDERAL, ");
                query.Append("COUNT(CASE WHEN T.ID_CLASIFICACION_JURIDICA = '3' AND CP.CP_FUERO = 'F' AND T.SEXO = 'M' THEN 1 END) AS M_SENTENCIADO_FEDERAL ");
                query.Append("FROM ");
                query.Append("( ");
                query.Append("SELECT  ");
                query.Append("CASE  ");
                query.Append("WHEN TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) >= 30 AND TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) <= 39 THEN '30 A 39'  ");
                query.Append("WHEN TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) >= 40 AND TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) <= 49 THEN '40 A 49'  ");
                query.Append("WHEN TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) >= 50 AND TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) <= 59 THEN '50 A 49'  ");
                query.Append("WHEN TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) >= 60 THEN 'MAYOR DE 60'  ");
                query.Append("END RANGO_EDAD, ");
                query.Append("CASE ");
                query.Append("WHEN TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) >= 30 AND TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) <= 39 THEN 1  ");
                query.Append("WHEN TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) >= 40 AND TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) <= 49 THEN 2  ");
                query.Append("WHEN TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) >= 50 AND TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) <= 59 THEN 3  ");
                query.Append("WHEN TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) >= 60 THEN 4  ");
                query.Append("END ORDEN ");
                query.Append(",IM.SEXO,I.* ");
                query.Append("FROM SSP.INGRESO I ");
                query.Append("INNER JOIN SSP.IMPUTADO IM ON I.ID_CENTRO = IM.ID_CENTRO AND I.ID_ANIO = IM.ID_ANIO AND I.ID_IMPUTADO = IM.ID_IMPUTADO ");
                query.AppendFormat("WHERE I.ID_UB_CENTRO = {0} AND I.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) AND TRUNC(MONTHS_BETWEEN(SYSDATE,IM.NACIMIENTO_FECHA)/12) >= 30 ",Centro);
                query.Append(") T ");
                query.Append("LEFT JOIN SSP.CAUSA_PENAL CP ON T.ID_CENTRO = CP.ID_CENTRO AND T.ID_ANIO = CP.ID_ANIO AND T.ID_IMPUTADO = CP.ID_IMPUTADO AND T.ID_INGRESO = CP.ID_INGRESO AND CP.ID_ESTATUS_CP = 1 ");
                query.Append("LEFT JOIN SSP.NUC N ON CP.ID_CENTRO = N.ID_CENTRO AND CP.ID_ANIO = N.ID_ANIO AND CP.ID_IMPUTADO = N.ID_IMPUTADO AND CP.ID_INGRESO = N.ID_INGRESO AND CP.ID_CAUSA_PENAL = N.ID_CAUSA_PENAL ");
                query.Append("GROUP BY T.ORDEN,T.RANGO_EDAD ");
                query.Append("ORDER BY T.ORDEN ");
                return Context.Database.SqlQuery<cTerceraEdad>(query.ToString());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }




    }

    public class unionimputado
    {
        public short CENTRO { get; set; }
        public short AÑO { get; set; }
        public int FOLIO { get; set; }
        public string NOMBREAPODO { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        //public ICollection<INGRESO> ABOGADO_INGRESO { get; set; }
    }

    public class cLiberados 
    {
        public short ID_CENTRO { get; set; }
        public int ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public string CENTRO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string APODO_NOMBRE { get; set; }
        public string PATERNO_A { get; set; }
        public string MATERNO_A { get; set; }
    }

    public class cTerceraEdad
    {
        public short ORDEN { get; set; }
        public string RANGO_EDAD { get; set; }
        public short M_IMPUTADO_COMUN { get; set; }
        public short M_INDICIADO_COMUN { get; set; }
        public short M_PROCESADO_COMUN { get; set; }
        public short F_PROCESADO_FEDERAL { get; set; }
        public short M_PROCESADO_FEDERAL { get; set; }
        public short M_PROCESADO_SIN_FUERO { get; set; }
        public short M_SENTENCIADO_COMUN { get; set; }
        public short F_SENTENCIADO_FEDERAL { get; set; }
        public short M_SENTENCIADO_FEDERAL { get; set; }
    }
}
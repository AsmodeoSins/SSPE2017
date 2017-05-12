using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;
using System.Transactions;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersona : EntityManagerServer<PERSONA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cPersona()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "PERSONA"</returns>
        public IQueryable<PERSONA> ObtenerTodos(string nombre = "", string paterno = "", string materno = "", int? codigo = null, int Pagina = 1)
        {
            //var Resultado = new List<PERSONA>().AsQueryable();
            try
            {
                var por_codigo = false;
                var predicate = PredicateBuilder.True<PERSONA>();
                //if (codigo.HasValue)
                //    if (codigo != 0)
                //    { 
                //        predicate = predicate.And(w => w.ID_PERSONA == codigo); 
                //    }
                if (codigo.HasValue)
                    if (codigo != 0)
                    {
                        por_codigo = true;
                        predicate = predicate.And(w => w.ID_PERSONA == codigo);
                    }
                if (!por_codigo)
                {
                    if (!string.IsNullOrEmpty(nombre))
                        predicate = predicate.And(w => w.NOMBRE.Contains(nombre));
                    if (!string.IsNullOrEmpty(paterno))
                        predicate = predicate.And(w => w.PATERNO.Contains(paterno));
                    if (!string.IsNullOrEmpty(materno))
                        predicate = predicate.And(w => w.MATERNO.Contains(materno));
                }
                return GetData(predicate.Expand()).OrderBy(o => o.ID_TIPO_PERSONA).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
                //if (nombre == null)
                //    nombre = string.Empty;
                //if (paterno == null)
                //    paterno = string.Empty;
                //if (materno == null)
                //    materno = string.Empty;

                //if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(paterno) && string.IsNullOrEmpty(materno) && codigo == null) { }
                //else
                //    if (codigo == null || codigo == 0)
                //    {
                //        if ((string.IsNullOrEmpty(nombre) ? true : string.IsNullOrEmpty(nombre.Trim())) &&
                //            (string.IsNullOrEmpty(paterno) ? true : string.IsNullOrEmpty(paterno.Trim())) &&
                //            (string.IsNullOrEmpty(materno) ? true : string.IsNullOrEmpty(materno.Trim())))
                //            return Resultado;
                //        if (!(paterno.Trim().Length < 2 && materno.Trim().Length < 2 && nombre.Trim().Length < 2))
                //            Resultado = GetData().Where(w => w.NOMBRE.Contains(nombre) && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno)).OrderBy(o => o.ID_TIPO_PERSONA)
                //                            .Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
                //    }
                //    else
                //        Resultado = GetData().Where(w => w.ID_PERSONA == codigo).OrderBy(o => o.ID_TIPO_PERSONA);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return Resultado;
        }
        public IQueryable<PERSONA> ObtenerTodosXEmpleados(string nombre = "", string paterno = "", string materno = "", int? codigo = null, int Pagina = 1, int centro = 0)
        {
            //var Resultado = new List<PERSONA>().AsQueryable();
            try
            {
                var predicate = PredicateBuilder.True<PERSONA>();
                if (!string.IsNullOrEmpty(nombre))
                    predicate = predicate.And(w => w.NOMBRE.Contains(nombre));
                if (!string.IsNullOrEmpty(paterno))
                    predicate = predicate.And(w => w.PATERNO.Contains(paterno));
                if (!string.IsNullOrEmpty(materno))
                    predicate = predicate.And(w => w.MATERNO.Contains(materno));
                if (codigo.HasValue)
                    if (codigo != 0)
                        predicate = predicate.And(w => w.ID_PERSONA == codigo);
                return GetData(predicate.Expand()).Where(w => w.EMPLEADO != null ? (centro == 0 ? true : w.EMPLEADO.ID_CENTRO == centro) : false)
                    .OrderByDescending(o => o.EMPLEADO != null).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return Resultado;
        }

        /// <summary>
        /// Obtiene los abogados registrados en el sistema
        /// </summary>
        /// <param name="nombre">Nombre del Abogado</param>
        /// <param name="paterno">Apellido paterno del abogado</param>
        /// <param name="materno">Apellido materno del abogadoi</param>
        /// <param name="Pagina">Pagina</param>
        /// <returns>Lista de personas de tipo abogado</returns>
        public IQueryable<PERSONA> ObtenerAbogados(string nombre = "", string paterno = "", string materno = "", int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<PERSONA>();
                if (!string.IsNullOrEmpty(nombre))
                    predicate = predicate.And(w => w.NOMBRE.Contains(nombre));
                if (!string.IsNullOrEmpty(paterno))
                    predicate = predicate.And(w => w.PATERNO.Contains(paterno));
                if (!string.IsNullOrEmpty(materno))
                    predicate = predicate.And(w => w.MATERNO.Contains(materno));
                //ABOGADO
                predicate = predicate.And(w => w.ABOGADO != null);
                return GetData(predicate.Expand()).OrderBy(o => o.ID_PERSONA).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<PERSONA> ObtenerAbogadosXTipoTitular(string nombre = "", string paterno = "", string materno = "", int? codigo = 0, string abogadoTitular = "", int Pagina = 1)
        {
            var Resultado = new List<PERSONA>().AsQueryable();
            try
            {
                if (nombre == null)
                    nombre = string.Empty;
                if (paterno == null)
                    paterno = string.Empty;
                if (materno == null)
                    materno = string.Empty;
                if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(paterno) && string.IsNullOrEmpty(materno) && (codigo == null || codigo == 0))
                    return Resultado;
                else
                    if (codigo == null || codigo == 0)
                    {
                        if ((string.IsNullOrEmpty(nombre) ? true : string.IsNullOrEmpty(nombre.Trim())) &&
                            (string.IsNullOrEmpty(paterno) ? true : string.IsNullOrEmpty(paterno.Trim())) &&
                            (string.IsNullOrEmpty(materno) ? true : string.IsNullOrEmpty(materno.Trim())))
                            return Resultado;
                        if (!(paterno.Trim().Length < 2 && materno.Trim().Length < 2 && nombre.Trim().Length < 2))
                            Resultado = GetData().Where(w => (w.NOMBRE.Contains(nombre) && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno)) &&
                                     w.ABOGADO != null && (w.ABOGADO != null ? w.ABOGADO.ID_ABOGADO_TITULO == abogadoTitular : false)).OrderBy(o => o.ID_PERSONA)
                                     .Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
                    }
                    else
                        Resultado = GetData().Where(w => (w.ABOGADO != null ? w.ID_PERSONA == codigo : false) &&
                            (string.IsNullOrEmpty(abogadoTitular) ? true : w.ABOGADO.ID_ABOGADO_TITULO == abogadoTitular)).OrderBy(o => o.ID_PERSONA);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public IQueryable<PERSONA> ObtenerXNombreYNIP(string nombre = "", string paterno = "", string materno = "", int? codigo = 0)
        {
            try
            {
                var predicate = PredicateBuilder.True<PERSONA>();
                if (codigo > 0)
                {
                    predicate = predicate.And(w => w.ID_PERSONA == codigo);
                }
                else
                {
                    if (!string.IsNullOrEmpty(nombre))
                        predicate = predicate.And(w => w.NOMBRE.Contains(nombre));
                    if (!string.IsNullOrEmpty(paterno))
                        predicate = predicate.And(w => w.PATERNO.Contains(paterno));
                    if (!string.IsNullOrEmpty(materno))
                        predicate = predicate.And(w => w.MATERNO.Contains(materno));
                }
                return GetData(predicate.Expand()).OrderBy(o => o.ID_TIPO_PERSONA);

                //if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(paterno) && string.IsNullOrEmpty(materno) && codigo == null) { }
                //else
                //    if (codigo == null || codigo == 0)
                //        Resultado = GetData().Where(w => w.NOMBRE.Contains(nombre) && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno)).OrderBy(o => o.ID_TIPO_PERSONA);
                //    else
                //    {
                //        Resultado = GetData().Where(w => w.PERSONA_NIP.Where(wh => wh.NIP == codigo).Any() && w.NOMBRE.Contains(nombre)
                //            && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno)).OrderBy(o => o.ID_TIPO_PERSONA);
                //    }
                ////personas = new ObservableCollection<PERSONA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return Resultado;
        }
        public IQueryable<PERSONA> ObtenerXNombreYNIP_Externos(string nombre = "", string paterno = "", string materno = "", int? codigo = 0)
        {
            //var Resultado = new List<PERSONA>().AsQueryable();
            try
            {
                var predicate = PredicateBuilder.True<PERSONA>();
                if (!string.IsNullOrEmpty(nombre))
                    predicate = predicate.And(w => w.NOMBRE.Contains(nombre));
                if (!string.IsNullOrEmpty(paterno))
                    predicate = predicate.And(w => w.PATERNO.Contains(paterno));
                if (!string.IsNullOrEmpty(materno))
                    predicate = predicate.And(w => w.MATERNO.Contains(materno));
                predicate = predicate.And(w => w.PERSONA_EXTERNO != null);
                return GetData(predicate.Expand());
                //Modificacion de modelo, PENDIENTE
                //if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(paterno) && string.IsNullOrEmpty(materno) && codigo == null) { }
                //else
                //    if (codigo == null || codigo == 0)
                //        Resultado = GetData().Where(w => w.NOMBRE.Contains(nombre) && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno) &&
                //            w.PERSONA_EXTERNO.Any()).OrderBy(o => o.ID_TIPO_PERSONA);
                //    else
                //    {
                //        Resultado = GetData().Where(w => w.PERSONA_NIP.Where(wh => wh.NIP == codigo).Any() && w.NOMBRE.Contains(nombre)
                //            && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno) && w.PERSONA_EXTERNO.Any()).OrderBy(o => o.ID_TIPO_PERSONA);
                //    }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
           // return Resultado;
        }
        public IQueryable<PERSONA> ObtenerPersonasVisitantes(string nombre = "", string paterno = "", string materno = "", long? codigo = null, int Pagina = 1)
        {
           try
           {
               var predicate = PredicateBuilder.True<PERSONA>();
               if (!string.IsNullOrEmpty(nombre))
                   predicate = predicate.And(w => w.NOMBRE.Contains(nombre));
               if (!string.IsNullOrEmpty(paterno))
                   predicate = predicate.And(w => w.PATERNO.Contains(paterno));
               if (!string.IsNullOrEmpty(materno))
                   predicate = predicate.And(w => w.MATERNO.Contains(materno));
               if (codigo.HasValue)
               { 
                    if(codigo > 0)
                        predicate = predicate.And(w => w.ID_PERSONA == codigo);
               }
               predicate = predicate.And(w => w.VISITANTE != null);
               return GetData(predicate.Expand()).OrderBy(o => o.ID_PERSONA).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));

               //if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(paterno) && string.IsNullOrEmpty(materno) && codigo == null) { }
                //else
                //    if (codigo == null || codigo == 0)
                //    {
                //        Resultado = GetData().Where(w => w.NOMBRE.Contains(nombre) && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno)
                //               && w.VISITANTE != null);
                //    }
                //    else
                //        Resultado = GetData().Where(w => w.ID_PERSONA == codigo && w.VISITANTE != null);
                //personas = new ObservableCollection<PERSONA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return Resultado;
        }
        public IQueryable<PERSONA> ObtenerPersonaXID(int codigo)
        {
            var Resultado = new List<PERSONA>().AsQueryable();
            try
            {
                Resultado = GetData().Where(w => w.ID_PERSONA == codigo);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public IQueryable<PERSONA> ObtenerPersonasVisitantesXIngreso(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            var Resultado = new List<PERSONA>().AsQueryable();
            try
            {
                if (centro > 0 && anio > 0 && imputado > 0 && ingreso > 0)
                    Resultado = GetData().Where(w => w.VISITANTE.VISITANTE_INGRESO.Where(wh => wh.ID_CENTRO == centro && wh.ID_ANIO == anio
                        && wh.ID_IMPUTADO == imputado && wh.ID_INGRESO == ingreso).Any());
                //Resultado = GetData().Where(w => w.VISITANTE.VISITANTE_INGRESO.Where(wh => wh.INGRESO.ID_CENTRO == centro && wh.INGRESO.ID_ANIO == anio && wh.INGRESO.ID_IMPUTADO == imputado && wh.INGRESO.ID_INGRESO == ingreso).Any()).ToList();
                //personas = new ObservableCollection<PERSONA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public IQueryable<PERSONA> ObtenerPersonasVisitantesBusquedaXInterno(string nombre = "", string paterno = "", string materno = "", int? codigo = 0, short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            var Resultado = new List<PERSONA>().AsQueryable();
            try
            {
                if (codigo == null || codigo == 0)
                    if (centro > 0 && anio > 0 && imputado > 0 && ingreso > 0)
                        Resultado = GetData().Where(w => w.NOMBRE.Contains(nombre) && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno)
                            && w.VISITANTE.VISITANTE_INGRESO.Where(wh => wh.ID_CENTRO == centro && wh.ID_ANIO == anio
                                && wh.ID_IMPUTADO == imputado && wh.ID_INGRESO == ingreso).Any());
                //Resultado = GetData().Where(w => w.NOMBRE.Contains(nombre) && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno) && w.VISITANTE.VISITANTE_INGRESO.Where(wh => wh.INGRESO.ID_CENTRO == centro && wh.INGRESO.ID_ANIO == anio && wh.INGRESO.ID_IMPUTADO == imputado && w.id_ingreso == ingreso).Any()).ToList();
                //personas = new ObservableCollection<PERSONA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        public IQueryable<PERSONA> ObtenerEmpleadosSinUsuario(string nombre = "", string paterno = "", string materno = "", short? Centro = null)
        {
            try
            {
                //Empleado
                var predicate = PredicateBuilder.True<PERSONA>();
                if (!string.IsNullOrEmpty(nombre))
                    predicate = predicate.And(w => w.NOMBRE.Contains(nombre));
                if (!string.IsNullOrEmpty(paterno))
                    predicate = predicate.And(w => w.PATERNO.Contains(paterno));
                if (!string.IsNullOrEmpty(materno))
                    predicate = predicate.And(w => w.MATERNO.Contains(materno));
                predicate = predicate.And(w => w.EMPLEADO != null);
                //Visitante Externo
                if (Centro != null)
                {
                    predicate = predicate.And(w => w.EMPLEADO.ID_CENTRO == Centro);
                    //predicate = predicate.And(w => w.PERSONA_EXTERNO.Where(y => y.ID_CENTRO == Centro).Count() > 0);
                }
                predicate = predicate.And(w => w.EMPLEADO.USUARIO.Count == 0);
                var empleados = GetData(predicate.Expand());

                //Visitante Externo
                var predicateVE = PredicateBuilder.True<PERSONA>();
                if (!string.IsNullOrEmpty(nombre))
                    predicateVE = predicateVE.And(w => w.NOMBRE.Contains(nombre));
                if (!string.IsNullOrEmpty(paterno))
                    predicateVE = predicateVE.And(w => w.PATERNO.Contains(paterno));
                if (!string.IsNullOrEmpty(materno))
                    predicateVE = predicateVE.And(w => w.MATERNO.Contains(materno));
                //Modificacion de modelo, PENDIENTE
                //if (Centro != null)
                //    predicateVE = predicateVE.And(w => w.PERSONA_EXTERNO.Where(y => y.ID_CENTRO == Centro).Count() > 0);
                predicateVE = predicateVE.And(w => w.EMPLEADO.USUARIO.Count == 0);
                return GetData(predicateVE.Expand()).Concat(empleados).Distinct();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }
        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public IQueryable<PERSONA> Obtener(long Id)
        {
            try
            {
                return GetData().Where(w => w.ID_PERSONA == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "PERSONA"</returns>
        public PERSONA ObtenerPersona(long Id)
        {
            try
            {
                return GetData().Where(w => w.ID_PERSONA == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(PERSONA Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                Entity.ID_PERSONA = int.Parse(DateTime.Parse(GetFechaServer()).Year + "" + GetSequence<int>("ID_PERSONA_SEQ"));
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        #region TRANSACCIONES
        public bool InsertarVisitaTransaccion(PERSONA persona, VISITANTE visitante, List<VISITANTE_INGRESO> VisitantesIngresos, List<ACOMPANANTE> acompanantes, VISITA_AUTORIZADA VisitaAutorizada, List<PERSONA_BIOMETRICO> PersonaFotos,
            List<PERSONA_BIOMETRICO> PersonaHuellas/*, PERSONA_NIP personaNip*/, short FotoFrenteRegistro, VISITANTE_INGRESO_PASE PaseIngreso, List<VISITANTE_INGRESO_PASE> PasesIngresos)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
                }))
                {
                    if (Context.PERSONA.Any(w => w.ID_PERSONA == persona.ID_PERSONA))
                    {
                        Context.Entry(persona).State = EntityState.Modified;
                        if (Context.VISITANTE.Any(w => w.ID_PERSONA == visitante.ID_PERSONA))
                            Context.Entry(visitante).State = EntityState.Modified;
                        else
                            Context.VISITANTE.Add(visitante);
                    }
                    else
                    {
                        Context.PERSONA.Add(persona);
                        Context.VISITANTE.Add(visitante);
                    }
                    if (VisitantesIngresos.Count > 0)
                    {
                        var hoy = GetFechaServerDate();
                        var lvi = Context.VISITANTE_INGRESO.Where(w => w.ID_PERSONA == persona.ID_PERSONA);
                        foreach (var item in VisitantesIngresos)
                        {
                            var x = lvi.FirstOrDefault(w => w.ID_PERSONA == item.ID_PERSONA && w.ID_CENTRO == item.ID_CENTRO &&
                                w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO);
                            if (x != null)
                            {
                                if (item.ID_ESTATUS_VISITA != x.ID_ESTATUS_VISITA)
                                {
                                    if (item.ID_ESTATUS_VISITA == 13)
                                        item.FIN_REGISTRO = hoy;
                                }
                                else
                                { 
                                    item.INICIO_REGISTRO = x.INICIO_REGISTRO;
                                    item.FIN_REGISTRO = x.FIN_REGISTRO;
                                }
                                Context.Entry(x).CurrentValues.SetValues(item);
                                //Context.Entry(item).State = EntityState.Modified; 
                            }
                            else
                            {
                                Context.VISITANTE_INGRESO.Add(item);
                            }
                        }
                        //foreach (var item in VisitantesIngresos)
                        //{
                        //    if (Context.VISITANTE_INGRESO.Any(w => w.ID_PERSONA == item.ID_PERSONA && w.ID_CENTRO == item.ID_CENTRO &&
                        //        w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO))
                        //    {
                        //        Context.Entry(item).State = EntityState.Modified; 
                        //    }
                        //    else
                        //    {
                        //        Context.VISITANTE_INGRESO.Add(item);
                        //        /*Context.VISITANTE_INGRESO_PASE.Add(new VISITANTE_INGRESO_PASE
                        //        {
                        //            ID_ANIO = item.ID_ANIO,
                        //            ID_CENTRO = item.ID_CENTRO,
                        //            ID_IMPUTADO = item.ID_IMPUTADO,
                        //            ID_INGRESO = item.ID_INGRESO,
                        //            ID_PERSONA = item.ID_PERSONA,
                        //            ID_PASE = 1,
                        //            FECHA_ALTA = item.FEC_ALTA,
                        //            ID_CONSEC = 1
                        //        });*/
                        //    }
                        //}
                    }
                    //if (personaNip != null ? personaNip.ID_PERSONA > 0 : false)
                    //    Context.PERSONA_NIP.Add(personaNip);
                    if (acompanantes.Count > 0)
                    {
                        foreach (var item in acompanantes)
                        {
                            if (Context.ACOMPANANTE.Any(w => w.ID_ACOMPANANTE == item.ID_ACOMPANANTE && w.ID_VISITANTE == item.ID_VISITANTE &&
                                w.ACO_ID_ANIO == item.ACO_ID_ANIO && w.ACO_ID_IMPUTADO == item.ACO_ID_IMPUTADO && w.ACO_ID_INGRESO == item.ACO_ID_INGRESO &&
                                w.ACO_ID_CENTRO == item.ACO_ID_CENTRO && w.VIS_ID_ANIO == item.VIS_ID_ANIO && w.VIS_ID_IMPUTADO == item.VIS_ID_IMPUTADO &&
                                w.VIS_ID_INGRESO == item.VIS_ID_INGRESO && w.VIS_ID_CENTRO == item.VIS_ID_CENTRO))
                                Context.Entry(item).State = EntityState.Modified;
                            else
                                Context.ACOMPANANTE.Add(item);
                        }
                    }
                    if (VisitaAutorizada != null)
                    {
                        if (VisitaAutorizada.ID_VISITA > 0)
                            Context.Entry(VisitaAutorizada).State = EntityState.Modified;
                        else
                        {
                            if (VisitaAutorizada.ID_CENTRO > 0 && VisitaAutorizada.ID_ANIO > 0 && VisitaAutorizada.ID_IMPUTADO > 0 && VisitaAutorizada.ID_INGRESO > 0)
                            {
                                var visitas = Context.VISITA_AUTORIZADA.Where(w => w.ID_PERSONA == persona.ID_PERSONA && VisitaAutorizada.ID_CENTRO == w.ID_CENTRO && w.ID_ANIO == VisitaAutorizada.ID_ANIO &&
                                   w.ID_IMPUTADO == VisitaAutorizada.ID_IMPUTADO && w.ID_INGRESO == VisitaAutorizada.ID_INGRESO);
                                if (!visitas.Any())
                                {
                                    VisitaAutorizada.ID_VISITA = (short)(visitas.OrderByDescending(o => o.ID_VISITA).Select(s => s.ID_VISITA).FirstOrDefault() + 1);
                                    Context.VISITA_AUTORIZADA.Add(VisitaAutorizada);
                                }
                            }
                        }
                    }
                    if (PersonaFotos.Count > 0)
                        if (Context.PERSONA_BIOMETRICO.Any(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO == FotoFrenteRegistro))
                            foreach (var item in PersonaFotos)
                                Context.Entry(item).State = EntityState.Modified;
                        else
                            foreach (var item in PersonaFotos)
                                Context.PERSONA_BIOMETRICO.Add(item);
                    if (PersonaHuellas.Count > 0)
                    {
                        var Deletes = Context.PERSONA_BIOMETRICO.Where(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO < 11).ToList();
                        if (Deletes.Count > 0)
                            foreach (var item in Deletes)
                                Context.PERSONA_BIOMETRICO.Remove(item);
                        foreach (var item in PersonaHuellas)
                            Context.PERSONA_BIOMETRICO.Add(item);
                    }
                    if (PasesIngresos.Count > 0)
                    {
                        Context.VISITANTE_INGRESO_PASE.Add(PaseIngreso);
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool InsertarVisitaExternaTransaccion(PERSONA persona, PERSONA_NIP personaNIP, bool NipExiste, PERSONA_EXTERNO personExterna, List<PERSONA_BIOMETRICO> personaFotos, List<PERSONA_BIOMETRICO> personaHuellas,
            short centro, short VisitaExterna, short FotoFrenteRegistro)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
                }))
                {
                    if (Context.PERSONA.Any(a => a.ID_PERSONA == persona.ID_PERSONA))
                    {
                        Context.Entry(persona).State = EntityState.Modified;
                        if (!NipExiste)
                            Context.PERSONA_NIP.Add(personaNIP);

                        if (Context.PERSONA_EXTERNO.Any(w => w.ID_CENTRO == centro && w.ID_PERSONA == persona.ID_PERSONA))
                            Context.Entry(personExterna).State = EntityState.Modified;
                        else
                            Context.PERSONA_EXTERNO.Add(personExterna);

                    }
                    else
                    {
                        Context.PERSONA.Add(persona);
                        if (personaNIP != null ? personaNIP.ID_PERSONA > 0 : false)
                        {
                            if (Context.PERSONA_NIP.Any(w => w.ID_CENTRO == centro && w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_VISITA == VisitaExterna))
                                Context.Entry(personaNIP).State = EntityState.Modified;
                            else
                                Context.PERSONA_NIP.Add(personaNIP);
                        }
                        if (personExterna != null ? personExterna.ID_PERSONA > 0 : false)
                        {
                            if (Context.PERSONA_EXTERNO.Any(w => w.ID_CENTRO == centro && w.ID_PERSONA == persona.ID_PERSONA))
                                Context.Entry(personExterna).State = EntityState.Modified;
                            else
                                Context.PERSONA_EXTERNO.Add(personExterna);
                        }
                    }
                    if (personaFotos.Count > 0)
                        if (Context.PERSONA_BIOMETRICO.Any(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO == FotoFrenteRegistro))
                            foreach (var item in personaFotos)
                                Context.Entry(item).State = EntityState.Modified;
                        else
                            foreach (var item in personaFotos)
                                Context.PERSONA_BIOMETRICO.Add(item);

                    if (personaHuellas.Count > 0)
                    {
                        var Deletes = Context.PERSONA_BIOMETRICO.Where(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO < 11).ToList();
                        if (Deletes.Count > 0)
                            foreach (var item in Deletes)
                                Context.PERSONA_BIOMETRICO.Remove(item);
                        foreach (var item in personaHuellas)
                            Context.PERSONA_BIOMETRICO.Add(item);
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool InsertarVisitaAutorizadaTransaccion(VISITA_AUTORIZADA VisitaAutorizada, bool Editable, List<VISITA_AGENDA> VisitaAgenda, PERSONA persona = null, VISITANTE visitante = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
                }))
                {

                    if (VisitaAutorizada.ID_VISITA > 0)
                    {
                        if (Editable)
                            Context.Entry(VisitaAutorizada).State = EntityState.Modified;
                        else
                            Context.VISITA_AUTORIZADA.Add(VisitaAutorizada);
                    }
                    foreach (var item in VisitaAgenda)
                    {
                        if (Context.VISITA_AGENDA.Any(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO &&
                            w.ID_INGRESO == item.ID_INGRESO && w.ID_TIPO_VISITA == item.ID_TIPO_VISITA && w.ID_DIA == item.ID_DIA))
                            Context.Entry(item).State = EntityState.Modified;
                        else
                            Context.VISITA_AGENDA.Add(item);
                    }
                    if (persona != null)
                    {
                        Context.Entry(persona).State = EntityState.Modified;
                        if (visitante != null ? visitante.ID_PERSONA > 0 : false)
                            Context.Entry(visitante).State = EntityState.Modified;
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool InsertarAbogadoTransaccion(PERSONA persona, bool Editable, ABOGADO abogado, bool NuevoAbogado, bool NuevoTitular, string titulo, short estatusCancelado,
            List<PERSONA_NIP> personaNip, List<PERSONA_BIOMETRICO> personaFotos, List<PERSONA_BIOMETRICO> personaHuellas, short FotoFrenteRegistro)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
                }))
                {
                    //persona.ID_PERSONA = int.Parse(DateTime.Parse(GetFechaServer()).Year + "" + GetSequence<int>("ID_PERSONA_SEQ"));
                    if (Editable)
                    {
                        persona.ABOGADO = null;
                        Context.Entry(persona).State = EntityState.Modified;
                        if (NuevoAbogado)
                        {
                            Context.ABOGADO.Add(abogado);
                            if (personaNip.Count > 0)
                                foreach (var item in personaNip)
                                    Context.PERSONA_NIP.Add(item);
                            if (NuevoTitular)
                            {
                                var AbogadoIngresos = Context.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO == persona.ID_PERSONA && w.ID_ABOGADO_TITULO == titulo).Select(s => new ABOGADO_INGRESO
                                {
                                    CAPTURA_FEC = s.CAPTURA_FEC,
                                    ID_ABOGADO = s.ID_ABOGADO,
                                    ID_ABOGADO_TITULO = s.ID_ABOGADO_TITULO,
                                    ID_ANIO = s.ID_ANIO,
                                    ID_CENTRO = s.ID_CENTRO,
                                    ID_ESTATUS_VISITA = estatusCancelado,
                                    ID_IMPUTADO = s.ID_IMPUTADO,
                                    ID_INGRESO = s.ID_INGRESO,
                                    OBSERV = s.OBSERV
                                }).ToList();
                                foreach (var item in AbogadoIngresos)
                                    Context.Entry(item).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            abogado.PERSONA = null;
                            Context.Entry(abogado).State = EntityState.Modified;
                        }

                        if (personaFotos.Count > 0)
                            if (Context.PERSONA_BIOMETRICO.Any(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO == FotoFrenteRegistro))
                                foreach (var item in personaFotos)
                                    Context.Entry(item).State = EntityState.Modified;
                            else
                                foreach (var item in personaFotos)
                                    Context.PERSONA_BIOMETRICO.Add(item);
                        if (personaHuellas.Count > 0)
                        {
                            var Deletes = Context.PERSONA_BIOMETRICO.Where(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO < 11).ToList();
                            if (Deletes.Count > 0)
                                foreach (var item in Deletes)
                                    Context.PERSONA_BIOMETRICO.Remove(item);
                            foreach (var item in personaHuellas)
                                Context.PERSONA_BIOMETRICO.Add(item);
                        }
                    }
                    else
                    {
                        Context.PERSONA.Add(persona);
                        Context.ABOGADO.Add(abogado);
                        if (personaNip.Count > 0)
                            foreach (var item in personaNip)
                                Context.PERSONA_NIP.Add(item);
                        if (personaFotos.Count > 0)
                            if (Context.PERSONA_BIOMETRICO.Any(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO == FotoFrenteRegistro))
                                foreach (var item in personaFotos)
                                    Context.Entry(item).State = EntityState.Modified;
                            else
                                foreach (var item in personaFotos)
                                    Context.PERSONA_BIOMETRICO.Add(item);
                        if (personaHuellas.Count > 0)
                            foreach (var item in personaHuellas)
                                Context.PERSONA_BIOMETRICO.Add(item);
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool InsertarColaboradorTransaccion(PERSONA persona, bool Editable, ABOGADO abogado, bool NuevoAbogado, bool NuevoTitular, string titulo, short estatusCancelado, List<PERSONA_NIP> personaNip, List<PERSONA_BIOMETRICO> personaFotos, List<PERSONA_BIOMETRICO> personaHuellas, short FotoFrenteRegistro, bool EditarTitular)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //persona.ID_PERSONA = int.Parse(DateTime.Parse(GetFechaServer()).Year + "" + GetSequence<int>("ID_PERSONA_SEQ"));
                    if (Editable)
                    {
                        Context.Entry(persona).State = EntityState.Modified;
                        if (NuevoAbogado)
                        {
                            if (personaNip.Count > 0)
                                foreach (var item in personaNip)
                                    Context.PERSONA_NIP.Add(item);
                            if (NuevoTitular)
                            {
                                var AbogadoIngresos = Context.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO == persona.ID_PERSONA && w.ID_ABOGADO_TITULO == titulo).Select(s => new ABOGADO_INGRESO
                                {
                                    CAPTURA_FEC = s.CAPTURA_FEC,
                                    ID_ABOGADO = s.ID_ABOGADO,
                                    ID_ABOGADO_TITULO = s.ID_ABOGADO_TITULO,
                                    ID_ANIO = s.ID_ANIO,
                                    ID_CENTRO = s.ID_CENTRO,
                                    ID_ESTATUS_VISITA = estatusCancelado,
                                    ID_IMPUTADO = s.ID_IMPUTADO,
                                    ID_INGRESO = s.ID_INGRESO,
                                    OBSERV = s.OBSERV
                                }).ToList();
                                foreach (var item in AbogadoIngresos)
                                    Context.Entry(item).State = EntityState.Modified;
                            }
                        }
                        else
                            Context.Entry(abogado).State = EntityState.Modified;

                        if (personaFotos.Count > 0)
                            if (Context.PERSONA_BIOMETRICO.Any(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO == FotoFrenteRegistro))
                                foreach (var item in personaFotos)
                                    Context.Entry(item).State = EntityState.Modified;
                            else
                                foreach (var item in personaFotos)
                                    Context.PERSONA_BIOMETRICO.Add(item);
                        if (personaHuellas.Count > 0)
                        {
                            var Deletes = Context.PERSONA_BIOMETRICO.Where(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO < 11).ToList();
                            if (Deletes.Count > 0)
                                foreach (var item in Deletes)
                                    Context.PERSONA_BIOMETRICO.Remove(item);
                            foreach (var item in personaHuellas)
                                Context.PERSONA_BIOMETRICO.Add(item);
                        }
                    }
                    else
                    {
                        Context.PERSONA.Add(persona);
                        Context.ABOGADO.Add(abogado);
                        if (personaNip.Count > 0)
                            foreach (var item in personaNip)
                                Context.PERSONA_NIP.Add(item);
                        if (personaFotos.Count > 0)
                            if (Context.PERSONA_BIOMETRICO.Any(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO == FotoFrenteRegistro))
                                foreach (var item in personaFotos)
                                    Context.Entry(item).State = EntityState.Modified;
                            else
                                foreach (var item in personaFotos)
                                    Context.PERSONA_BIOMETRICO.Add(item);
                        if (personaHuellas.Count > 0)
                            foreach (var item in personaHuellas)
                                Context.PERSONA_BIOMETRICO.Add(item);
                    }
                    if (EditarTitular)
                    {
                        foreach (var item in Context.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO == abogado.ID_ABOGADO && w.ID_ABOGADO_TITULO == titulo))
                            Context.Entry(new ABOGADO_INGRESO
                            {
                                ADMINISTRATIVO = item.ADMINISTRATIVO,
                                CAPTURA_FEC = item.CAPTURA_FEC,
                                ID_ABOGADO = item.ID_ABOGADO,
                                ID_ABOGADO_TITULO = item.ID_ABOGADO_TITULO,
                                ID_ANIO = item.ID_ANIO,
                                ID_CENTRO = item.ID_CENTRO,
                                ID_ESTATUS_VISITA = estatusCancelado,
                                ID_IMPUTADO = item.ID_IMPUTADO,
                                ID_INGRESO = item.ID_INGRESO,
                                OBSERV = item.OBSERV,
                            }).State = EntityState.Modified;
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool InsertarAbogadoAsignacionTransaccion(ABOGADO_INGRESO abogadoIngreso, bool Editable, List<ABOGADO_CAUSA_PENAL> abogadoCausaPenal)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (Editable)
                        Context.Entry(abogadoIngreso).State = EntityState.Modified;
                    else
                        Context.ABOGADO_INGRESO.Add(abogadoIngreso);
                    Context.SaveChanges();
                    foreach (var item in abogadoCausaPenal)
                    {
                        if (Context.ABOGADO_CAUSA_PENAL.Where(aboCP =>
                            aboCP.ID_CENTRO == item.ID_CENTRO &&
                            aboCP.ID_ABOGADO_TITULO == item.ID_ABOGADO_TITULO &&
                            aboCP.ID_CAUSA_PENAL == item.ID_CAUSA_PENAL &&
                            aboCP.ID_ANIO == item.ID_ANIO &&
                            aboCP.ID_IMPUTADO == item.ID_IMPUTADO &&
                            aboCP.ID_INGRESO == item.ID_INGRESO &&
                            aboCP.ID_ABOGADO == item.ID_ABOGADO &&
                            aboCP.ID_ABOGADO_TITULAR == item.ID_ABOGADO_TITULAR).Any())
                            Context.Entry(item).State = EntityState.Modified;
                        else
                            Context.ABOGADO_CAUSA_PENAL.Add(item);
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public bool ActualizarAbogadoAsignacionTransaccion(ABOGADO_INGRESO AbogadoIngreso, List<ABOGADO_CAUSA_PENAL> AbogadoCausaPenal)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    #region Abogado Interno
                    //anterior titular
                    var old_titular = Context.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO != AbogadoIngreso.ID_ABOGADO && w.ID_CENTRO == AbogadoIngreso.ID_CENTRO && w.ID_ANIO == AbogadoIngreso.ID_ANIO &&
                        w.ID_IMPUTADO == AbogadoIngreso.ID_IMPUTADO && w.ID_INGRESO == AbogadoIngreso.ID_INGRESO);
                    if (old_titular != null)
                    {
                        foreach (var ot in old_titular)
                        {
                            if (ot.ID_ESTATUS_VISITA != 14 && ot.ID_ESTATUS_VISITA != 15)
                                ot.ID_ESTATUS_VISITA = 14;
                            Context.Entry(ot).State = EntityState.Modified;
                        }
                    }

                    var abogado_interno = Context.ABOGADO_INGRESO.Where(w =>
                        w.ID_ABOGADO == AbogadoIngreso.ID_ABOGADO &&
                        w.ID_CENTRO == AbogadoIngreso.ID_CENTRO &&
                        w.ID_ANIO == AbogadoIngreso.ID_ANIO &&
                        w.ID_IMPUTADO == AbogadoIngreso.ID_IMPUTADO &&
                        w.ID_INGRESO == AbogadoIngreso.ID_INGRESO &&
                        w.ID_ABOGADO_TITULO == AbogadoIngreso.ID_ABOGADO_TITULO &&
                        w.ID_ABOGADO_TITULAR == AbogadoIngreso.ID_ABOGADO_TITULAR).FirstOrDefault();
                    if (abogado_interno != null)//update
                    {
                        abogado_interno.OBSERV = AbogadoIngreso.OBSERV;
                        abogado_interno.ID_ESTATUS_VISITA = AbogadoIngreso.ID_ESTATUS_VISITA; //13;//Autorizada
                        Context.Entry(abogado_interno).State = EntityState.Modified;
                    }
                    else//insert
                    {
                        Context.ABOGADO_INGRESO.Add(AbogadoIngreso);
                    }
                    #endregion

                    #region Abogado Causa Penal
                    foreach (var obj in AbogadoCausaPenal)
                    {
                        var cp = Context.ABOGADO_CAUSA_PENAL.Where(w => w.ID_CENTRO == obj.ID_CENTRO && w.ID_ANIO == obj.ID_ANIO && w.ID_IMPUTADO == obj.ID_IMPUTADO && w.ID_CAUSA_PENAL == obj.ID_CAUSA_PENAL /*&& w.ID_ABOGADO_TITULO == "T"*/);
                        if (cp != null)//Update
                        {
                            bool editar = false;
                            foreach (var o in cp)
                            {
                                if (o.ID_ABOGADO == obj.ID_ABOGADO)//Update
                                {
                                    editar = true;
                                    o.ID_ESTATUS_VISITA = o.ID_ESTATUS_VISITA;//13;//Autorizada
                                    Context.Entry(o).State = EntityState.Modified;
                                }
                                else//Insert
                                {
                                    o.ID_ESTATUS_VISITA = 14;//Cancelado
                                    Context.Entry(o).State = EntityState.Modified;
                                }
                            }
                            if (!editar)
                                Context.ABOGADO_CAUSA_PENAL.Add(obj);
                        }
                        else//Insert
                        {
                            Context.ABOGADO_CAUSA_PENAL.Add(obj);
                        }
                    }
                    #endregion

                    #region Verifica si cancela
                    if (AbogadoIngreso.ID_ESTATUS_VISITA == 14)
                    {
                        var abogados_ingreso = Context.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO == AbogadoIngreso.ID_ABOGADO && w.ID_CENTRO == AbogadoIngreso.ID_CENTRO && w.ID_ANIO == AbogadoIngreso.ID_ANIO && w.ID_IMPUTADO == AbogadoIngreso.ID_IMPUTADO && w.ID_INGRESO == AbogadoIngreso.ID_INGRESO && w.ID_ESTATUS_VISITA == 13);
                        if (abogados_ingreso != null)
                        {
                            foreach (var ai in abogados_ingreso)
                            {
                                ai.ID_ESTATUS_VISITA = 14;//Cancelado
                                Context.Entry(ai).State = EntityState.Modified;
                            }
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
        }

        #endregion

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(PERSONA Entity, out PERSONA ReturnEntity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                Entity.ID_PERSONA = GetSequence<short>("PERSONA_SEQ");
                return Insert(Entity, out ReturnEntity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public int InsertarP(PERSONA Entity, int Anio)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_PERSONA = int.Parse(Anio + "" + GetSequence<int>("ID_PERSONA_SEQ"));
                    Context.PERSONA.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_PERSONA;
                }
                //Entity.ID_PERSONA = int.Parse(DateTime.Parse(GetFechaServer()).Year + "" + GetSequence<int>("ID_PERSONA_SEQ"));
                //if (Insert(Entity))
                //    return Entity.ID_PERSONA;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }
        public int InsertarEmpleado(PERSONA persona, int Anio, bool editar, bool nuevo_empleado, EMPLEADO empleado = null, PERSONA_NIP persona_nip = null, PERSONA_BIOMETRICO foto = null, List<PERSONA_BIOMETRICO> huellas = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (!editar)
                    {
                        persona.ID_PERSONA = int.Parse(Anio + "" + GetSequence<int>("ID_PERSONA_SEQ"));
                        Context.PERSONA.Add(persona);
                        empleado.ID_EMPLEADO = persona.ID_PERSONA;
                        Context.EMPLEADO.Add(empleado);
                        persona_nip.ID_PERSONA = persona.ID_PERSONA;
                        Context.PERSONA_NIP.Add(persona_nip);
                    }
                    else
                    {
                        Context.Entry(persona).State = EntityState.Modified;
                        if (nuevo_empleado)
                        {
                            if (empleado != null)
                                Context.EMPLEADO.Add(empleado);
                            if (persona_nip != null)
                                Context.PERSONA_NIP.Add(persona_nip);
                        }
                        else
                        {
                            if (empleado != null)
                                Context.Entry(empleado).State = EntityState.Modified;
                            if (persona_nip != null)
                                Context.Entry(persona_nip).State = EntityState.Modified;
                        }
                    }
                    #region Foto
                    if (foto != null && foto.BIOMETRICO != null && foto.BIOMETRICO.Count() > 0)
                    {
                        foto.ID_PERSONA = persona.ID_PERSONA;
                        if (Context.PERSONA_BIOMETRICO.Where(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO == 102).Count() > 0)
                            Context.Entry(foto).State = EntityState.Modified;
                        else
                            Context.PERSONA_BIOMETRICO.Add(foto);
                    }
                    #endregion
                    #region Huellas
                    if (huellas != null)
                    {
                        if (huellas.Count > 0)
                        {
                            foreach (var h in Context.PERSONA_BIOMETRICO.Where(w => w.ID_PERSONA == persona.ID_PERSONA && w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9))
                            {
                                h.ID_PERSONA = persona.ID_PERSONA;
                                Context.Entry(h).State = EntityState.Deleted;
                            }
                            foreach (var h in huellas)
                            {
                                h.ID_PERSONA = persona.ID_PERSONA;
                                Context.PERSONA_BIOMETRICO.Add(h);
                            }
                        }
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return persona.ID_PERSONA;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }
        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(PERSONA Entity)
        {
            try
            {
                return Update(Entity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(PERSONA Entity, PERSONA_BIOMETRICO Foto, List<PERSONA_BIOMETRICO> Huellas)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = EntityState.Modified;
                    #region Foto
                    if (Foto != null)
                    {
                        var foto = Context.PERSONA_BIOMETRICO.Where(w => w.ID_PERSONA == Entity.ID_PERSONA && w.ID_TIPO_BIOMETRICO == 102).Count();//FOTO_FRENTE_REGISTRO
                        if (foto > 0)
                            Context.Entry(Foto).State = EntityState.Modified;
                        else
                            Context.PERSONA_BIOMETRICO.Add(Foto);
                    }
                    #endregion
                    #region Huellas
                    if (Huellas != null)
                    {
                        if (Huellas != null)
                        {
                            var huellas = Context.PERSONA_BIOMETRICO.Where(w => w.ID_PERSONA == Entity.ID_PERSONA && w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9);
                            foreach (var h in huellas)
                            {
                                Context.Entry(h).State = EntityState.Deleted;
                            }
                        }
                        foreach (var h in Huellas)
                        {
                            Context.PERSONA_BIOMETRICO.Add(h);
                        }
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //return Update(Entity);
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
                var Entity = GetData().Where(w => w.ID_PERSONA == Id).SingleOrDefault();
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


        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool InsertarPersona(PERSONA Persona)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.PERSONA.Add(Persona);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un visitante externo
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool ActualizarVisitaExterno(PERSONA Persona, List<PERSONA_BIOMETRICO> PersonaFotos, PERSONA_EXTERNO PersonaExterno)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Persona).State = EntityState.Modified;
                    var pe = Context.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == PersonaExterno.ID_CENTRO && w.ID_PERSONA == PersonaExterno.ID_PERSONA);
                    if (pe != null)
                    {
                        foreach (var x in pe)
                            Context.Entry(x).State = EntityState.Deleted;
                    }

                    Context.PERSONA_EXTERNO.Add(PersonaExterno);
                    var biometricos = Context.PERSONA_BIOMETRICO.Where(w => w.ID_PERSONA == Persona.ID_PERSONA);
                    if (biometricos != null)
                    {
                        foreach (var b in biometricos)
                        {
                            Context.Entry(b).State = EntityState.Deleted;
                        }
                    }
                    if (PersonaFotos != null)
                    {
                        foreach (var b in PersonaFotos)
                        {
                            Context.PERSONA_BIOMETRICO.Add(b);
                        }
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool ActualizarAbogado(PERSONA Persona, List<PERSONA_BIOMETRICO> PersonaFotos, ABOGADO Abogado, int IdTitular = 0, DateTime? Fecha = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //var ab = Context.ABOGADO.Where(w => w.ID_ABOGADO == Persona.ID_PERSONA).FirstOrDefault();
                    //if (ab != null)
                    //{
                    //    if (ab.ID_ABOGADO_TITULO != Abogado.ID_ABOGADO_TITULO)
                    //        Abogado.CREDENCIALIZADO = "N";
                    //}

                    Context.Entry(Persona).State = EntityState.Modified;
                    #region Biometrico
                    var biometrico = Context.PERSONA_BIOMETRICO.Where(w => w.ID_PERSONA == Persona.ID_PERSONA && (w.ID_TIPO_BIOMETRICO == 102 || (w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9)));
                    if (biometrico != null)
                    {
                        foreach (var obj in biometrico)
                        {
                            Context.Entry(obj).State = EntityState.Deleted;
                        }
                    }
                    foreach (var obj in PersonaFotos)
                    {
                        Context.PERSONA_BIOMETRICO.Add(obj);
                    }
                    #endregion

                    #region Abogado
                    var abogado = Context.ABOGADO.Count(w => w.ID_ABOGADO == Persona.ID_PERSONA);
                    if (abogado > 0)
                        Context.Entry(Abogado).State = EntityState.Modified;
                    else
                        Context.ABOGADO.Add(Abogado);
                    #endregion

                    #region Titular
                    if (IdTitular != 0)
                    {
                        var titular = new ABOGADO_TITULAR();
                        titular.ID_ABOGADO = Persona.ID_PERSONA;
                        titular.ID_ABOGADO_TITULAR = IdTitular;
                        titular.ESTATUS = "S";
                        titular.MOVIMIENTO_FEC = Fecha;
                        Context.ABOGADO_TITULAR.Add(titular);
                    }
                    #endregion

                    #region Cambio a Abogado Titular
                    if (Abogado.ID_ABOGADO_TITULO == "T")
                    {
                        var abogadosTitulares = Context.ABOGADO_TITULAR.Where(w => w.ID_ABOGADO == Persona.ID_PERSONA);
                        if (abogadosTitulares != null)
                        {
                            foreach (var at in abogadosTitulares)
                            {
                                var abogadosIngresos = Context.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO == at.ID_ABOGADO && w.ID_ABOGADO_TITULAR == at.ID_ABOGADO_TITULAR);
                                if (abogadosIngresos != null)
                                {
                                    foreach (var ai in abogadosIngresos)
                                    {
                                        ai.ID_ESTATUS_VISITA = 14;
                                        Context.Entry(ai).State = EntityState.Modified;

                                        if (ai.ABOGADO_CAUSA_PENAL != null)
                                        {
                                            foreach (var acp in ai.ABOGADO_CAUSA_PENAL)
                                            {
                                                acp.ID_ESTATUS_VISITA = 14;
                                                Context.Entry(acp).State = EntityState.Modified;
                                            }
                                        }
                                    }
                                }
                                at.ESTATUS = "N";
                                Context.Entry(at).State = EntityState.Modified;
                            }
                        }
                    }
                    #endregion

                    #region Cambio de titular a actuario
                    if (Abogado.ID_ABOGADO_TITULO == "A") //si es actuario
                    {
                        var abogados = Context.ABOGADO_TITULAR.Where(w => w.ID_ABOGADO_TITULAR == Persona.ID_PERSONA && w.ESTATUS == "S");
                        if (abogados != null)
                        {
                            foreach (var a in abogados)
                            {
                                var colaborador = a.ABOGADO;
                                if (colaborador != null)
                                {
                                    colaborador.ID_ESTATUS_VISITA = 14;
                                    Context.ABOGADO.Attach(colaborador);
                                    Context.Entry(colaborador).Property(x => x.ID_ESTATUS_VISITA).IsModified = true;
                                }

                                if (a.ABOGADO.ABOGADO_INGRESO != null)
                                {
                                    foreach (var i in a.ABOGADO.ABOGADO_INGRESO)
                                    {
                                        if (i.ABOGADO_CAUSA_PENAL != null)
                                        {
                                            foreach (var cp in i.ABOGADO_CAUSA_PENAL)
                                            {
                                                cp.ID_ESTATUS_VISITA = 14;//cancelado
                                                Context.Entry(cp).State = EntityState.Modified;
                                            }
                                        }
                                        i.ID_ESTATUS_VISITA = 14;//cancelado
                                        Context.Entry(i).State = EntityState.Modified;
                                    }
                                }
                                a.ESTATUS = "N";
                                Context.Entry(a).State = EntityState.Modified;

                            }
                        }

                        var colab = Context.ABOGADO_TITULAR.Where(w => w.ID_ABOGADO == Persona.ID_PERSONA && w.ESTATUS == "S");
                        if (colab != null)
                        {
                            foreach (var c in colab)
                            {
                                c.ESTATUS = "N";
                            }
                        }

                        var aingresos = Context.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO == Persona.ID_PERSONA && w.ID_ESTATUS_VISITA == 13);
                        if (aingresos != null)
                        {
                            foreach (var ai in aingresos)
                            {
                                ai.ID_ESTATUS_VISITA = 14;
                                Context.Entry(ai).State = EntityState.Modified;

                                if (ai.ABOGADO_CAUSA_PENAL != null)
                                {
                                    foreach (var acp in ai.ABOGADO_CAUSA_PENAL)
                                    {
                                        acp.ID_ESTATUS_VISITA = 14;
                                        Context.Entry(acp).State = EntityState.Modified;
                                    }
                                }
                            }
                        }

                        #region Comentado
                        //var abogados = Context.ABOGADO.Where(w => w.ABOGADO_TITULAR == Persona.ID_PERSONA);
                        //if (abogados != null)
                        //{
                        //    foreach (var a in abogados)
                        //    {
                        //        if (a.ABOGADO_INGRESO != null)
                        //        {
                        //            foreach (var i in a.ABOGADO_INGRESO)
                        //            {
                        //                if (i.ABOGADO_CAUSA_PENAL != null)
                        //                {
                        //                    foreach (var cp in i.ABOGADO_CAUSA_PENAL)
                        //                    {
                        //                        cp.ID_ESTATUS_VISITA = 14;//cancelado
                        //                        Context.Entry(cp).State = EntityState.Modified; 
                        //                    }
                        //                }
                        //                i.ID_ESTATUS_VISITA = 14;//cancelado
                        //                Context.Entry(i).State = EntityState.Modified; 
                        //            }
                        //        }
                        //        a.ID_ESTATUS_VISITA = 14;//cancelado
                        //        a.ULTIMA_MOD_FEC = Abogado.ULTIMA_MOD_FEC;
                        //    }
                        //}
                        #endregion
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
        }


    }
}
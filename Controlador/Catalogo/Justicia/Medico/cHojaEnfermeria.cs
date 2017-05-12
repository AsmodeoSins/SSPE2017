using System.Linq;
using LinqKit;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cHojaEnfermeria : SSP.Modelo.EntityManagerServer<SSP.Servidor.HOJA_ENFERMERIA>
    {
        public cHojaEnfermeria() { }

        public bool GuardaHojaEnfermeria(SSP.Servidor.HOJA_ENFERMERIA Entity)
        {
            try
            {
                if (Entity == null)
                    return false;

                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _HojaEnfExist = Context.HOJA_ENFERMERIA.FirstOrDefault(x => x.ID_HOJAENF == Entity.ID_HOJAENF);
                    if (_HojaEnfExist == null)
                    {
                        decimal _ConsecutivoHE = GetIDProceso<decimal>("HOJA_ENFERMERIA", "ID_HOJAENF", "1=1");
                        SSP.Servidor.HOJA_ENFERMERIA _HojaNueva = new Servidor.HOJA_ENFERMERIA()
                        {
                            CONCENTRADO_EGRESO = Entity.CONCENTRADO_EGRESO,
                            CONCENTRADO_INGRESO = Entity.CONCENTRADO_INGRESO,
                            ESTATUS = Entity.ESTATUS,
                            FECHA_REGISTRO = Entity.FECHA_REGISTRO,
                            FECHA_HOJA = Entity.FECHA_HOJA,
                            ID_CENTRO_UBI = Entity.ID_CENTRO_UBI,
                            ID_HOJAENF = _ConsecutivoHE,
                            ID_HOSPITA = Entity.ID_HOSPITA,
                            ID_LIQTURNO = Entity.ID_LIQTURNO,
                            ID_USUARIO = Entity.ID_USUARIO,
                            OBSERVACION = Entity.OBSERVACION,
                            LABORATORIO = Entity.LABORATORIO,
                            RX = Entity.RX
                        };

                        decimal _ConsecutivoHELectura = GetIDProceso<decimal>("HOJA_ENFERMERIA_LECTURA", "ID_HOJAENFLEC", "1=1");
                        if (Entity.HOJA_ENFERMERIA_LECTURA != null && Entity.HOJA_ENFERMERIA_LECTURA.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_LECTURA)
                            {
                                SSP.Servidor.HOJA_ENFERMERIA_LECTURA _Lectura = new SSP.Servidor.HOJA_ENFERMERIA_LECTURA()
                                    {
                                        CAMBIO_ESCARAS = item.CAMBIO_ESCARAS,
                                        CAMBIO_POSICION = item.CAMBIO_POSICION,
                                        FECHA_LECTURA = item.FECHA_LECTURA,
                                        DEXTROXTIX = item.DEXTROXTIX,
                                        FECHA_REGISTRO = item.FECHA_REGISTRO,
                                        ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                        ID_HOJAENF = _HojaNueva.ID_HOJAENF,
                                        ID_HOJAENFLEC = _ConsecutivoHELectura,
                                        ID_HOSPITA = item.ID_HOSPITA,
                                        SAO = item.SAO,
                                        ID_USUARIO = item.ID_USUARIO,
                                        RIESGO_CAIDAS = item.RIESGO_CAIDAS,
                                        TEMP = item.TEMP,
                                        NEB = item.NEB,
                                        PC = item.PC,
                                        PR = item.PR,
                                        PVC = item.PVC,
                                        TA = item.TA,
                                        TA_MEDIA = item.TA_MEDIA
                                    };

                                Context.HOJA_ENFERMERIA_LECTURA.Add(_Lectura);
                                _ConsecutivoHELectura++;
                            };

                        decimal _ConsecutivoHEControl = GetIDProceso<decimal>("HOJA_CONTROL_ENFERMERIA", "ID_HOJACTRLEN", "1=1");
                        if (Entity.HOJA_CONTROL_ENFERMERIA != null && Entity.HOJA_CONTROL_ENFERMERIA.Any())
                            foreach (var item in Entity.HOJA_CONTROL_ENFERMERIA)
                            {
                                SSP.Servidor.HOJA_CONTROL_ENFERMERIA _Control = new Servidor.HOJA_CONTROL_ENFERMERIA()
                                {
                                    CANT = item.CANT,
                                    FECHA_REGISTRO = item.FECHA_REGISTRO,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_USUARIO = item.ID_USUARIO,
                                    OTRO_LIQUIDO = item.OTRO_LIQUIDO,
                                    ID_LIQHORA = item.ID_LIQHORA,
                                    ID_LIQ = item.ID_LIQ,
                                    ID_HOJAENF = _HojaNueva.ID_HOJAENF,
                                    ID_HOJACTRLEN = _ConsecutivoHEControl,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI
                                };

                                Context.HOJA_CONTROL_ENFERMERIA.Add(_Control);
                                _ConsecutivoHEControl++;
                            };

                        decimal _ConsecutivoHESoluciones = GetIDProceso<decimal>("HOJA_ENFERMERIA_SOLUCION", "ID_HOJAENFSOL", "1=1");
                        if (Entity.HOJA_ENFERMERIA_SOLUCION != null && Entity.HOJA_ENFERMERIA_SOLUCION.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_SOLUCION)
                            {
                                SSP.Servidor.HOJA_ENFERMERIA_SOLUCION _Solucion = new Servidor.HOJA_ENFERMERIA_SOLUCION()
                                {
                                    FECHA_REGISTRO = item.FECHA_REGISTRO,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJAENF = _HojaNueva.ID_HOJAENF,
                                    ID_HOJAENFSOL = _ConsecutivoHESoluciones,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_PRESENTACION_MEDICAMENTO = item.ID_PRESENTACION_MEDICAMENTO,
                                    ID_LIQTURNO_INICIO = Entity.ID_LIQTURNO,
                                    ID_PRODUCTO = item.ID_PRODUCTO,
                                    TERMINO = item.TERMINO
                                };

                                Context.HOJA_ENFERMERIA_SOLUCION.Add(_Solucion);
                                _ConsecutivoHESoluciones++;
                            };

                        decimal _ConsecutivoHEMedicamentos = GetIDProceso<decimal>("HOJA_ENFERMERIA_MEDICAMENTO", "ID_HOJA_ENFMED", "1=1");
                        if (Entity.HOJA_ENFERMERIA_MEDICAMENTO != null && Entity.HOJA_ENFERMERIA_MEDICAMENTO.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_MEDICAMENTO)
                            {
                                SSP.Servidor.HOJA_ENFERMERIA_MEDICAMENTO _Medica = new Servidor.HOJA_ENFERMERIA_MEDICAMENTO()
                                {
                                    CANTIDAD = item.CANTIDAD,
                                    FECHA_REGISTRO = item.FECHA_REGISTRO,
                                    FECHA_SUMINISTRO = item.FECHA_SUMINISTRO,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJA_ENFMED = _ConsecutivoHEMedicamentos,
                                    ID_HOJAENF = _HojaNueva.ID_HOJAENF,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_PRESENTACION_MEDICAMENTO = item.ID_PRESENTACION_MEDICAMENTO,
                                    ID_PRODUCTO = item.ID_PRODUCTO,
                                    ID_FOLIO = item.ID_FOLIO,
                                    ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                    ID_USUARIO = item.ID_USUARIO
                                };

                                Context.HOJA_ENFERMERIA_MEDICAMENTO.Add(_Medica);
                                _ConsecutivoHEMedicamentos++;
                            };

                        decimal _ConsecutivoHEUlceras = GetIDProceso<decimal>("HOJA_ENFERMERIA_ULCERA", "ID_HOJA_ULCERA", "1=1");
                        if (Entity.HOJA_ENFERMERIA_ULCERA != null && Entity.HOJA_ENFERMERIA_ULCERA.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_ULCERA)
                            {
                                SSP.Servidor.HOJA_ENFERMERIA_ULCERA _Ulcera = new Servidor.HOJA_ENFERMERIA_ULCERA()
                                {
                                    DESC = item.DESC,
                                    FECHA_REGISTRO = item.FECHA_REGISTRO,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJA_ULCERA = _ConsecutivoHEUlceras,
                                    ID_HOJAENF = _HojaNueva.ID_HOJAENF,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_REGION = item.ID_REGION,
                                    ID_USUARIO = item.ID_USUARIO
                                };

                                Context.HOJA_ENFERMERIA_ULCERA.Add(_Ulcera);
                                _ConsecutivoHEUlceras++;
                            };

                        decimal _ConsecutivoHECateter = GetIDProceso<decimal>("HOJA_ENFERMERIA_CATETER", "ID_HOJAENFCAT", "1=1");
                        if (Entity.HOJA_ENFERMERIA_CATETER != null && Entity.HOJA_ENFERMERIA_CATETER.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_CATETER)
                            {
                                SSP.Servidor.HOJA_ENFERMERIA_CATETER _Cateter = new Servidor.HOJA_ENFERMERIA_CATETER()
                                {
                                    DATOS_INFECCION = item.DATOS_INFECCION,
                                    ID_CATETER = item.ID_CATETER,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJAENF = _HojaNueva.ID_HOJAENF,
                                    ID_HOJAENFCAT = _ConsecutivoHECateter,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_USUARIO = item.ID_USUARIO,
                                    INSTALACION_FEC = item.INSTALACION_FEC,
                                    MOTIVO_RETIRO = item.MOTIVO_RETIRO,
                                    REGISTRO_FEC = item.REGISTRO_FEC,
                                    RETIRO = item.RETIRO,
                                    VENCIMIENTO_FEC = item.VENCIMIENTO_FEC,
                                    FECHA_RETIRO = item.FECHA_RETIRO,
                                    ID_REGISTRO_INICIAL = _ConsecutivoHECateter
                                };

                                int _ConsecutivoHECateterIncidencia = GetIDProceso<int>("INCIDENCIAS_CATETER", "ID_INC_CATETER", "1=1");
                                if (item.INCIDENCIAS_CATETER != null && item.INCIDENCIAS_CATETER.Any())
                                    foreach (var item2 in item.INCIDENCIAS_CATETER)
                                    {
                                        SSP.Servidor.INCIDENCIAS_CATETER Inc = new Servidor.INCIDENCIAS_CATETER()
                                        {
                                            FECHA_REGISTRO = item2.FECHA_REGISTRO,
                                            ID_ACMOTIVO = item2.ID_ACMOTIVO,
                                            ID_CENTRO_UBI = item2.ID_CENTRO_UBI,
                                            ID_HOJAENFCAT = _ConsecutivoHECateter,
                                            ID_INC_CATETER = _ConsecutivoHECateterIncidencia,
                                            ID_USUARIO = item2.ID_USUARIO,
                                            OBSERVACIONES = item2.OBSERVACIONES
                                        };

                                        _Cateter.INCIDENCIAS_CATETER.Add(Inc);
                                        _ConsecutivoHECateterIncidencia++;
                                    };

                                Context.HOJA_ENFERMERIA_CATETER.Add(_Cateter);
                                _ConsecutivoHECateter++;
                            };

                        decimal _ConsecutivoHESonda = GetIDProceso<decimal>("HOJA_ENFERMERIA_SONDA_GASOGAS", "ID_SONDAGAS", "1=1");
                        if (Entity.HOJA_ENFERMERIA_SONDA_GASOGAS != null && Entity.HOJA_ENFERMERIA_SONDA_GASOGAS.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_SONDA_GASOGAS)
                            {
                                SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS _Sonda = new Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS()
                                {
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJAENF = _HojaNueva.ID_HOJAENF,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_SONDAGAS = _ConsecutivoHESonda,
                                    ID_USUARIO = item.ID_USUARIO,
                                    INSTALACION_FEC = item.INSTALACION_FEC,
                                    OBSERVACION = item.OBSERVACION,
                                    RETIRO = item.RETIRO,
                                    FECHA_RETIRO = item.FECHA_RETIRO,
                                    ID_REGISTRO_ORIGINAL = _ConsecutivoHESonda,
                                    REGISTRO_FEC = item.REGISTRO_FEC
                                };

                                int _ConsecutivoHESondaIncidencia = GetIDProceso<int>("INCIDENCIAS_SONDA_GAS", "ID_INC_SONDA_GAS", "1=1");
                                if (item.INCIDENCIAS_SONDA_GAS != null && item.INCIDENCIAS_SONDA_GAS.Any())
                                    foreach (var item2 in item.INCIDENCIAS_SONDA_GAS)
                                    {
                                        SSP.Servidor.INCIDENCIAS_SONDA_GAS Son = new Servidor.INCIDENCIAS_SONDA_GAS()
                                        {
                                            FECHA_REGISTRO = item2.FECHA_REGISTRO,
                                            ID_ACMOTIVO = item2.ID_ACMOTIVO,
                                            ID_CENTRO_UBI = item2.ID_CENTRO_UBI,
                                            ID_INC_SONDA_GAS = _ConsecutivoHESondaIncidencia,
                                            ID_SONDAGAS = _ConsecutivoHESonda,
                                            ID_USUARIO = item2.ID_USUARIO,
                                            OBSERVACIONES = item2.OBSERVACIONES
                                        };

                                        _Sonda.INCIDENCIAS_SONDA_GAS.Add(Son);
                                        _ConsecutivoHESondaIncidencia++;
                                    };

                                Context.HOJA_ENFERMERIA_SONDA_GASOGAS.Add(_Sonda);
                                _ConsecutivoHESonda++;
                            }

                        Context.HOJA_ENFERMERIA.Add(_HojaNueva);
                    }
                    else
                    {
                        #region Edicion de la hoja de enfermeria
                        _HojaEnfExist.CONCENTRADO_EGRESO = Entity.CONCENTRADO_EGRESO;
                        _HojaEnfExist.CONCENTRADO_INGRESO = Entity.CONCENTRADO_INGRESO;
                        _HojaEnfExist.ESTATUS = Entity.ESTATUS;
                        _HojaEnfExist.FECHA_HOJA = Entity.FECHA_HOJA;
                        _HojaEnfExist.OBSERVACION = Entity.OBSERVACION;
                        _HojaEnfExist.LABORATORIO = Entity.LABORATORIO;
                        _HojaEnfExist.RX = Entity.RX;
                        #endregion
                        #region Edicion de la lectura de signos vitales
                        var _LecturasViejas = Context.HOJA_ENFERMERIA_LECTURA.Where(x => x.ID_HOJAENF == _HojaEnfExist.ID_HOJAENF);
                        if (_LecturasViejas.Any())
                            foreach (var item in _LecturasViejas)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        decimal _ConsecutivoHELectura = GetIDProceso<decimal>("HOJA_ENFERMERIA_LECTURA", "ID_HOJAENFLEC", "1=1");
                        if (Entity.HOJA_ENFERMERIA_LECTURA != null && Entity.HOJA_ENFERMERIA_LECTURA.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_LECTURA)
                            {
                                SSP.Servidor.HOJA_ENFERMERIA_LECTURA _Lectura = new SSP.Servidor.HOJA_ENFERMERIA_LECTURA()
                                {
                                    CAMBIO_ESCARAS = item.CAMBIO_ESCARAS,
                                    CAMBIO_POSICION = item.CAMBIO_POSICION,
                                    FECHA_LECTURA = item.FECHA_LECTURA,
                                    DEXTROXTIX = item.DEXTROXTIX,
                                    FECHA_REGISTRO = item.FECHA_REGISTRO,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJAENF = _HojaEnfExist.ID_HOJAENF,
                                    ID_HOJAENFLEC = _ConsecutivoHELectura,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    SAO = item.SAO,
                                    ID_USUARIO = item.ID_USUARIO,
                                    RIESGO_CAIDAS = item.RIESGO_CAIDAS,
                                    TEMP = item.TEMP,
                                    NEB = item.NEB,
                                    PC = item.PC,
                                    PR = item.PR,
                                    PVC = item.PVC,
                                    TA = item.TA,
                                    TA_MEDIA = item.TA_MEDIA
                                };

                                Context.HOJA_ENFERMERIA_LECTURA.Add(_Lectura);
                                _ConsecutivoHELectura++;
                            };
                        #endregion
                        #region Edicion de ingresos - egresos
                        var _LiquidosViejos = Context.HOJA_CONTROL_ENFERMERIA.Where(x => x.ID_HOJAENF == _HojaEnfExist.ID_HOJAENF);
                        if (_LiquidosViejos != null && _LiquidosViejos.Any())
                            foreach (var item in _LiquidosViejos)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        decimal _ConsecutivoHEControl = GetIDProceso<decimal>("HOJA_CONTROL_ENFERMERIA", "ID_HOJACTRLEN", "1=1");
                        if (Entity.HOJA_CONTROL_ENFERMERIA != null && Entity.HOJA_CONTROL_ENFERMERIA.Any())
                            foreach (var item in Entity.HOJA_CONTROL_ENFERMERIA)
                            {
                                SSP.Servidor.HOJA_CONTROL_ENFERMERIA _Control = new Servidor.HOJA_CONTROL_ENFERMERIA()
                                {
                                    CANT = item.CANT,
                                    FECHA_REGISTRO = item.FECHA_REGISTRO,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_USUARIO = item.ID_USUARIO,
                                    OTRO_LIQUIDO = item.OTRO_LIQUIDO,
                                    ID_LIQHORA = item.ID_LIQHORA,
                                    ID_LIQ = item.ID_LIQ,
                                    ID_HOJAENF = _HojaEnfExist.ID_HOJAENF,
                                    ID_HOJACTRLEN = _ConsecutivoHEControl,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI
                                };

                                Context.HOJA_CONTROL_ENFERMERIA.Add(_Control);
                                _ConsecutivoHEControl++;
                            };
                        #endregion
                        #region Edicion de Soluciones
                        decimal _ConsecutivoHESoluciones = GetIDProceso<decimal>("HOJA_ENFERMERIA_SOLUCION", "ID_HOJAENFSOL", "1=1");
                        var SolucionesYaGuardadas = Context.HOJA_ENFERMERIA_SOLUCION.Where(x => x.ID_CENTRO_UBI == Entity.ID_CENTRO_UBI && x.ID_HOSPITA == Entity.ID_HOSPITA && x.HOJA_ENFERMERIA != null && x.HOJA_ENFERMERIA.ID_LIQTURNO == Entity.ID_LIQTURNO && x.TERMINO == "N");
                        if (Entity.HOJA_ENFERMERIA_SOLUCION != null && Entity.HOJA_ENFERMERIA_SOLUCION.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_SOLUCION)
                            {
                                if (SolucionesYaGuardadas.Any())//NO HA CAMBIADO, NO ES NECESARIO EL AGREGARLA DE NUEVO
                                    if (SolucionesYaGuardadas.Any(x => x.ID_PRODUCTO == item.ID_PRODUCTO && x.TERMINO == item.TERMINO && x.ID_LIQTURNO_INICIO == item.ID_LIQTURNO_INICIO))
                                        continue;

                                SSP.Servidor.HOJA_ENFERMERIA_SOLUCION _Solucion = new Servidor.HOJA_ENFERMERIA_SOLUCION()
                                {
                                    FECHA_REGISTRO = item.FECHA_REGISTRO,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJAENF = _HojaEnfExist.ID_HOJAENF,
                                    ID_HOJAENFSOL = _ConsecutivoHESoluciones,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_PRESENTACION_MEDICAMENTO = item.ID_PRESENTACION_MEDICAMENTO,
                                    ID_LIQTURNO_INICIO = _HojaEnfExist != null ? _HojaEnfExist.ID_LIQTURNO : new decimal?(),
                                    ID_PRODUCTO = item.ID_PRODUCTO,
                                    TERMINO = item.TERMINO
                                };

                                Context.HOJA_ENFERMERIA_SOLUCION.Add(_Solucion);
                                _ConsecutivoHESoluciones++;
                            };
                        #endregion
                        #region Medicamento y Ulceras
                        //var MedicamentosViejos = Context.HOJA_ENFERMERIA_MEDICAMENTO.Where(x => x.ID_HOJAENF == _HojaEnfExist.ID_HOJAENF);
                        //if (MedicamentosViejos.Any())
                        //    foreach (var item in MedicamentosViejos)
                        //        Context.Entry(item).State = System.Data.EntityState.Deleted;

                        decimal _ConsecutivoHEMedicamentos = GetIDProceso<decimal>("HOJA_ENFERMERIA_MEDICAMENTO", "ID_HOJA_ENFMED", "1=1");
                        if (Entity.HOJA_ENFERMERIA_MEDICAMENTO != null && Entity.HOJA_ENFERMERIA_MEDICAMENTO.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_MEDICAMENTO)
                            {
                                SSP.Servidor.HOJA_ENFERMERIA_MEDICAMENTO _Medica = new Servidor.HOJA_ENFERMERIA_MEDICAMENTO()
                                {
                                    CANTIDAD = item.CANTIDAD,
                                    FECHA_REGISTRO = item.FECHA_REGISTRO,
                                    FECHA_SUMINISTRO = item.FECHA_SUMINISTRO,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJA_ENFMED = _ConsecutivoHEMedicamentos,
                                    ID_HOJAENF = _HojaEnfExist.ID_HOJAENF,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_FOLIO = item.ID_FOLIO,
                                    ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                    ID_PRESENTACION_MEDICAMENTO = item.ID_PRESENTACION_MEDICAMENTO,
                                    ID_PRODUCTO = item.ID_PRODUCTO,
                                    ID_USUARIO = item.ID_USUARIO
                                };

                                Context.HOJA_ENFERMERIA_MEDICAMENTO.Add(_Medica);
                                _ConsecutivoHEMedicamentos++;
                            };

                        var UlcerasViejas = Context.HOJA_ENFERMERIA_ULCERA.Where(x => x.ID_HOJAENF == _HojaEnfExist.ID_HOJAENF);
                        if (UlcerasViejas != null && UlcerasViejas.Any())
                            foreach (var item in UlcerasViejas)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        decimal _ConsecutivoHEUlceras = GetIDProceso<decimal>("HOJA_ENFERMERIA_ULCERA", "ID_HOJA_ULCERA", "1=1");
                        if (Entity.HOJA_ENFERMERIA_ULCERA != null && Entity.HOJA_ENFERMERIA_ULCERA.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_ULCERA)
                            {
                                SSP.Servidor.HOJA_ENFERMERIA_ULCERA _Ulcera = new Servidor.HOJA_ENFERMERIA_ULCERA()
                                {
                                    DESC = item.DESC,
                                    FECHA_REGISTRO = item.FECHA_REGISTRO,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJA_ULCERA = _ConsecutivoHEUlceras,
                                    ID_HOJAENF = _HojaEnfExist.ID_HOJAENF,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_REGION = item.ID_REGION,
                                    ID_USUARIO = item.ID_USUARIO
                                };

                                Context.HOJA_ENFERMERIA_ULCERA.Add(_Ulcera);
                                _ConsecutivoHEUlceras++;
                            };
                        #endregion
                        #region Edicion de Cateter
                        decimal _ConsecutivoHECateter = GetIDProceso<decimal>("HOJA_ENFERMERIA_CATETER", "ID_HOJAENFCAT", "1=1");
                        int _ConsecutivoHECateterIncidencia = GetIDProceso<int>("INCIDENCIAS_CATETER", "ID_INC_CATETER", "1=1");
                        var CateteresExistentes = Context.HOJA_ENFERMERIA_CATETER.Where(x => x.ID_HOJAENF == _HojaEnfExist.ID_HOJAENF);//ESTOS SON LOS CATATERES QUE YA TENGO INSTALADOS EN ESTA HOJA DE ENFERMERIA
                        if (Entity.HOJA_ENFERMERIA_CATETER != null && Entity.HOJA_ENFERMERIA_CATETER.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_CATETER)
                            {
                                if (CateteresExistentes.Any())///NO LO HE MOVIDO
                                    if (CateteresExistentes.Any(x => x.ID_CATETER == item.ID_CATETER))
                                    {
                                        var DetallesCatater = CateteresExistentes.FirstOrDefault(x => x.ID_CATETER == item.ID_CATETER && x.ID_REGISTRO_INICIAL == item.ID_REGISTRO_INICIAL);
                                        if (DetallesCatater != null)
                                        {
                                            if (DetallesCatater.RETIRO == item.RETIRO)//sigue igual
                                            {
                                                DetallesCatater.DATOS_INFECCION = item.DATOS_INFECCION;
                                                DetallesCatater.FECHA_RETIRO = item.FECHA_RETIRO;
                                                DetallesCatater.MOTIVO_RETIRO = item.MOTIVO_RETIRO;
                                                DetallesCatater.VENCIMIENTO_FEC = item.VENCIMIENTO_FEC;
                                                Context.Entry(DetallesCatater).State = System.Data.EntityState.Modified;
                                                if (item.INCIDENCIAS_CATETER != null && item.INCIDENCIAS_CATETER.Any())
                                                    foreach (var item2 in item.INCIDENCIAS_CATETER)
                                                    {
                                                        var Incss = new SSP.Servidor.INCIDENCIAS_CATETER()
                                                        {
                                                            FECHA_REGISTRO = item2.FECHA_REGISTRO,
                                                            ID_ACMOTIVO = item2.ID_ACMOTIVO,
                                                            ID_CENTRO_UBI = item2.ID_CENTRO_UBI,
                                                            ID_HOJAENFCAT = DetallesCatater.ID_HOJAENFCAT,
                                                            OBSERVACIONES = item2.OBSERVACIONES,
                                                            ID_USUARIO = item2.ID_USUARIO,
                                                            ID_INC_CATETER = _ConsecutivoHECateterIncidencia
                                                        };

                                                        DetallesCatater.INCIDENCIAS_CATETER.Add(Incss);
                                                        _ConsecutivoHECateterIncidencia++;
                                                    }
                                                continue;
                                            }
                                            else
                                            {
                                                SSP.Servidor.HOJA_ENFERMERIA_CATETER _Cat = new Servidor.HOJA_ENFERMERIA_CATETER()
                                                {
                                                    DATOS_INFECCION = item.DATOS_INFECCION,
                                                    ID_CATETER = item.ID_CATETER,
                                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                                    ID_HOJAENF = _HojaEnfExist.ID_HOJAENF,
                                                    ID_HOJAENFCAT = _ConsecutivoHECateter,
                                                    ID_HOSPITA = item.ID_HOSPITA,
                                                    FECHA_RETIRO = item.FECHA_RETIRO,
                                                    ID_USUARIO = item.ID_USUARIO,
                                                    INSTALACION_FEC = item.INSTALACION_FEC,
                                                    MOTIVO_RETIRO = item.MOTIVO_RETIRO,
                                                    REGISTRO_FEC = item.REGISTRO_FEC,
                                                    RETIRO = item.RETIRO,
                                                    VENCIMIENTO_FEC = item.VENCIMIENTO_FEC,
                                                    ID_REGISTRO_INICIAL = _ConsecutivoHECateter
                                                };

                                                if (item.INCIDENCIAS_CATETER != null && item.INCIDENCIAS_CATETER.Any())
                                                    foreach (var item2 in item.INCIDENCIAS_CATETER)
                                                    {
                                                        var Incss = new SSP.Servidor.INCIDENCIAS_CATETER()
                                                        {
                                                            FECHA_REGISTRO = item2.FECHA_REGISTRO,
                                                            ID_ACMOTIVO = item2.ID_ACMOTIVO,
                                                            ID_CENTRO_UBI = item2.ID_CENTRO_UBI,
                                                            ID_HOJAENFCAT = _ConsecutivoHECateter,
                                                            OBSERVACIONES = item2.OBSERVACIONES,
                                                            ID_USUARIO = item2.ID_USUARIO,
                                                            ID_INC_CATETER = _ConsecutivoHECateterIncidencia
                                                        };

                                                        _Cat.INCIDENCIAS_CATETER.Add(Incss);
                                                        _ConsecutivoHECateterIncidencia++;
                                                    };

                                                Context.HOJA_ENFERMERIA_CATETER.Add(_Cat);
                                                _ConsecutivoHECateter++;
                                                continue;
                                            }
                                        }
                                    }

                                SSP.Servidor.HOJA_ENFERMERIA_CATETER _Cateter = new Servidor.HOJA_ENFERMERIA_CATETER()
                                {
                                    DATOS_INFECCION = item.DATOS_INFECCION,
                                    ID_CATETER = item.ID_CATETER,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJAENF = _HojaEnfExist.ID_HOJAENF,
                                    ID_HOJAENFCAT = _ConsecutivoHECateter,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_USUARIO = item.ID_USUARIO,
                                    FECHA_RETIRO = item.FECHA_RETIRO,
                                    INSTALACION_FEC = item.INSTALACION_FEC,
                                    MOTIVO_RETIRO = item.MOTIVO_RETIRO,
                                    REGISTRO_FEC = item.REGISTRO_FEC,
                                    RETIRO = item.RETIRO,
                                    VENCIMIENTO_FEC = item.VENCIMIENTO_FEC,
                                    ID_REGISTRO_INICIAL = _ConsecutivoHECateter
                                };

                                if (item.INCIDENCIAS_CATETER != null && item.INCIDENCIAS_CATETER.Any())
                                    foreach (var item2 in item.INCIDENCIAS_CATETER)
                                    {
                                        var Incss = new SSP.Servidor.INCIDENCIAS_CATETER()
                                        {
                                            FECHA_REGISTRO = item2.FECHA_REGISTRO,
                                            ID_ACMOTIVO = item2.ID_ACMOTIVO,
                                            ID_CENTRO_UBI = item2.ID_CENTRO_UBI,
                                            ID_HOJAENFCAT = item.ID_HOJAENFCAT,
                                            OBSERVACIONES = item2.OBSERVACIONES,
                                            ID_USUARIO = item2.ID_USUARIO,
                                            ID_INC_CATETER = _ConsecutivoHECateterIncidencia
                                        };

                                        _Cateter.INCIDENCIAS_CATETER.Add(Incss);
                                        _ConsecutivoHECateterIncidencia++;
                                    };

                                Context.HOJA_ENFERMERIA_CATETER.Add(_Cateter);
                                _ConsecutivoHECateter++;
                            }
                        #endregion
                        #region Edicion de Sondas
                        decimal _ConsecutivoHESonda = GetIDProceso<decimal>("HOJA_ENFERMERIA_SONDA_GASOGAS", "ID_SONDAGAS", "1=1");
                        int _ConsecutivoHESondaIncidencia = GetIDProceso<int>("INCIDENCIAS_SONDA_GAS", "ID_INC_SONDA_GAS", "1=1");
                        var SondasExistentes = Context.HOJA_ENFERMERIA_SONDA_GASOGAS.Where(x => x.ID_HOJAENF == _HojaEnfExist.ID_HOJAENF && x.ID_HOSPITA == _HojaEnfExist.ID_HOSPITA);
                        if (Entity.HOJA_ENFERMERIA_SONDA_GASOGAS != null && Entity.HOJA_ENFERMERIA_SONDA_GASOGAS.Any())
                            foreach (var item in Entity.HOJA_ENFERMERIA_SONDA_GASOGAS)
                            {
                                if (SondasExistentes.Any())
                                    if (SondasExistentes.Any(x => x.INSTALACION_FEC == item.INSTALACION_FEC))
                                    {
                                        var SondaEx = SondasExistentes.FirstOrDefault(x => x.INSTALACION_FEC == item.INSTALACION_FEC);
                                        if (SondaEx.RETIRO == item.RETIRO)
                                        {
                                            SondaEx.FECHA_RETIRO = item.FECHA_RETIRO;
                                            SondaEx.OBSERVACION = SondaEx.OBSERVACION;
                                            Context.Entry(SondaEx).State = System.Data.EntityState.Modified;
                                            if (item.INCIDENCIAS_SONDA_GAS != null && item.INCIDENCIAS_SONDA_GAS.Any())
                                            {
                                                foreach (var item2 in item.INCIDENCIAS_SONDA_GAS)
                                                {
                                                    var IncSonda = new SSP.Servidor.INCIDENCIAS_SONDA_GAS()
                                                    {
                                                        FECHA_REGISTRO = item2.FECHA_REGISTRO,
                                                        ID_ACMOTIVO = item2.ID_ACMOTIVO,
                                                        ID_CENTRO_UBI = item2.ID_CENTRO_UBI,
                                                        ID_INC_SONDA_GAS = _ConsecutivoHESondaIncidencia,
                                                        ID_SONDAGAS = SondaEx.ID_SONDAGAS,
                                                        ID_USUARIO = item2.ID_USUARIO,
                                                        OBSERVACIONES = item2.OBSERVACIONES
                                                    };

                                                    SondaEx.INCIDENCIAS_SONDA_GAS.Add(IncSonda);
                                                    _ConsecutivoHESondaIncidencia++;
                                                }
                                            }
                                            continue;
                                        }
                                        else
                                        {
                                            SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS _Sond = new Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS()
                                            {
                                                ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                                ID_HOJAENF = _HojaEnfExist.ID_HOJAENF,
                                                ID_HOSPITA = item.ID_HOSPITA,
                                                ID_SONDAGAS = _ConsecutivoHESonda,
                                                ID_USUARIO = item.ID_USUARIO,
                                                FECHA_RETIRO = item.FECHA_RETIRO,
                                                ID_REGISTRO_ORIGINAL = _ConsecutivoHESonda,
                                                INSTALACION_FEC = item.INSTALACION_FEC,
                                                OBSERVACION = item.OBSERVACION,
                                                RETIRO = item.RETIRO,
                                                REGISTRO_FEC = item.REGISTRO_FEC
                                            };

                                            if (item.INCIDENCIAS_SONDA_GAS != null && item.INCIDENCIAS_SONDA_GAS.Any())
                                            {
                                                foreach (var item2 in item.INCIDENCIAS_SONDA_GAS)
                                                {
                                                    var IncSonda = new SSP.Servidor.INCIDENCIAS_SONDA_GAS()
                                                     {
                                                         FECHA_REGISTRO = item2.FECHA_REGISTRO,
                                                         ID_ACMOTIVO = item2.ID_ACMOTIVO,
                                                         ID_CENTRO_UBI = item2.ID_CENTRO_UBI,
                                                         ID_INC_SONDA_GAS = _ConsecutivoHESondaIncidencia,
                                                         ID_SONDAGAS = _Sond.ID_SONDAGAS,
                                                         ID_USUARIO = item2.ID_USUARIO,
                                                         OBSERVACIONES = item2.OBSERVACIONES
                                                     };

                                                    _Sond.INCIDENCIAS_SONDA_GAS.Add(IncSonda);
                                                    _ConsecutivoHESondaIncidencia++;
                                                };
                                            };

                                            Context.HOJA_ENFERMERIA_SONDA_GASOGAS.Add(_Sond);
                                            _ConsecutivoHESonda++;
                                            continue;
                                        }
                                    }

                                SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS _Sonda = new Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS()
                                {
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_HOJAENF = _HojaEnfExist.ID_HOJAENF,
                                    ID_HOSPITA = item.ID_HOSPITA,
                                    ID_SONDAGAS = _ConsecutivoHESonda,
                                    ID_USUARIO = item.ID_USUARIO,
                                    INSTALACION_FEC = item.INSTALACION_FEC,
                                    OBSERVACION = item.OBSERVACION,
                                    FECHA_RETIRO = item.FECHA_RETIRO,
                                    ID_REGISTRO_ORIGINAL = _ConsecutivoHESonda,
                                    RETIRO = item.RETIRO,
                                    REGISTRO_FEC = item.REGISTRO_FEC
                                };

                                if (item.INCIDENCIAS_SONDA_GAS != null && item.INCIDENCIAS_SONDA_GAS.Any())
                                {
                                    foreach (var item2 in item.INCIDENCIAS_SONDA_GAS)
                                    {
                                        var IncSonda = new SSP.Servidor.INCIDENCIAS_SONDA_GAS()
                                        {
                                            FECHA_REGISTRO = item2.FECHA_REGISTRO,
                                            ID_ACMOTIVO = item2.ID_ACMOTIVO,
                                            ID_CENTRO_UBI = item2.ID_CENTRO_UBI,
                                            ID_INC_SONDA_GAS = _ConsecutivoHESondaIncidencia,
                                            ID_SONDAGAS = _Sonda.ID_SONDAGAS,
                                            ID_USUARIO = item2.ID_USUARIO,
                                            OBSERVACIONES = item2.OBSERVACIONES
                                        };

                                        _Sonda.INCIDENCIAS_SONDA_GAS.Add(IncSonda);
                                        _ConsecutivoHESondaIncidencia++;
                                    };
                                };

                                Context.HOJA_ENFERMERIA_SONDA_GASOGAS.Add(_Sonda);
                                _ConsecutivoHESonda++;
                            };

                        #endregion
                        Context.Entry(_HojaEnfExist).State = System.Data.EntityState.Modified;
                    };

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (System.Exception exc)
            {
                return false;
            }
        }

        public IQueryable<SSP.Servidor.HOJA_ENFERMERIA> ObtenerUltimaHojaEnfermeria(System.DateTime? Fecha, decimal? IdHospitalizacion, SSP.Servidor.INGRESO Ingreso)
        {
            try
            {
                var predicado = PredicateBuilder.True<SSP.Servidor.HOJA_ENFERMERIA>();
                //if (Fecha.HasValue)
                //    predicado = predicado.And(x => x.FECHA_REGISTRO.Value.Year == Fecha.Value.Year && x.FECHA_REGISTRO.Value.Month == Fecha.Value.Month && x.FECHA_REGISTRO.Value.Day == Fecha.Value.Day);
                if (IdHospitalizacion.HasValue)
                    predicado = predicado.And(x => x.ID_HOSPITA == IdHospitalizacion);

                predicado = predicado.And(x => x.HOSPITALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO == Ingreso.ID_INGRESO && x.HOSPITALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO == Ingreso.ID_IMPUTADO && x.HOSPITALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO == Ingreso.ID_ANIO);
                return GetData(predicado.Expand());
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public IQueryable<SSP.Servidor.HOJA_ENFERMERIA> ObtenerHojasEnfermeriaBusqueda(System.DateTime? FechaInicio, System.DateTime? FechaFin, SSP.Servidor.INGRESO Ingreso)
        {
            try
            {
                 var predicado = PredicateBuilder.True<SSP.Servidor.HOJA_ENFERMERIA>();

                 predicado = predicado.And(x => x.HOSPITALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO == Ingreso.ID_INGRESO && x.HOSPITALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO == Ingreso.ID_IMPUTADO && x.HOSPITALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO == Ingreso.ID_ANIO);

                 if (FechaInicio.HasValue)
                     predicado = predicado.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.FECHA_REGISTRO) >= System.Data.Objects.EntityFunctions.TruncateTime(FechaInicio));
                 if (FechaFin.HasValue)
                     predicado = predicado.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.FECHA_REGISTRO) <= System.Data.Objects.EntityFunctions.TruncateTime(FechaFin));

                 return GetData(predicado.Expand());
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }
    }
}
using DPUruNet;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using WCF_BiometricoService.Helpers;
using WCF_BiometricoService.Modelo;
using WCF_BiometricoService.Modelo.Entidades;
using WCF_BiometricoService.Modelo.Rule;
using LinqKit;
using System;
namespace WCF_BiometricoService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BiometricoService : IBiometricoService
    {
        #region [Utilerias]
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public byte[] DescargarWSQ(short ID_CENTRO, short ID_ANIO, int ID_IMPUTADO, enumTipoBiometrico Dedo, enumTipoFormato Formato)
        {
            try
            {
                var rleImputado = new MainRule<IMPUTADO_BIOMETRICO>();
                var ByteWSQ = rleImputado.Listado().Where(w => w.ID_CENTRO == ID_CENTRO && w.ID_ANIO == ID_ANIO && w.ID_IMPUTADO == ID_IMPUTADO && w.ID_TIPO_BIOMETRICO == (short)Dedo && w.ID_FORMATO == (short)Formato).SingleOrDefault().BIOMETRICO;
                if (ByteWSQ == null)
                    return null;
                return ByteWSQ;
            }
            catch
            {
                return null;
            }
        }

        //public byte[] DescargarBMP(short ID_CENTRO, short ID_ANIO, int ID_IMPUTADO, enumTipoBiometrico Dedo)
        //{
        //    try
        //    {
        //        var rleImputado = new MainRule<IMPUTADO_BIOMETRICO>();
        //        var ByteBMP = rleImputado.Listado().Where(w => w.ID_CENTRO == ID_CENTRO && w.ID_ANIO == ID_ANIO && w.ID_IMPUTADO == ID_IMPUTADO && w.ID_TIPO_BIOMETRICO == (short)Dedo && w.ID_FORMATO == (short)Formato).SingleOrDefault().BIOMETRICO;
        //        if (ByteBMP == null)
        //            return null;
        //        return ByteBMP;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        #endregion

        #region [IMPUTADO]
        public bool InsertarHuellaImputado(IMPUTADO_BIOMETRICO Data)
        {
            try
            {
                var rleImputado = new MainRule<IMPUTADO_BIOMETRICO>();
                rleImputado.Insertar(Data);
                return true;
            }
            catch
            {
                RollBackImputado(Data);
                return false;
            }
        }

        public bool InsertarHuellasImputado(IMPUTADO_BIOMETRICO[] Data)
        {
            try
            {
                var rleImputado = new MainRule<IMPUTADO_BIOMETRICO>();
                rleImputado.Insertar(Data);
                return true;
            }
            catch
            {
                RollBackImputado(Data.FirstOrDefault());
                return false;
            }
        }

        public bool ActualizarHuellasImputado(IMPUTADO_BIOMETRICO[] Data)
        {
            try
            {
                var rleImputado = new MainRule<IMPUTADO_BIOMETRICO>();
                rleImputado.Actualizar(Data);
                return true;
            }
            catch
            {
                RollBackImputado(Data.FirstOrDefault());
                return false;
            }
        }

        public bool RollBackImputado(IMPUTADO_BIOMETRICO Data)
        {
            try
            {
                if (Data == null)
                    return false;

                var rleImputado = new MainRule<IMPUTADO_BIOMETRICO>();
                rleImputado.Eliminar(rleImputado.Listado().Where(w => w.ID_ANIO == Data.ID_ANIO && w.ID_CENTRO == Data.ID_CENTRO && w.ID_IMPUTADO == Data.ID_IMPUTADO).ToList());
                return true;
            }
            catch { return false; }
        }

        public bool ExistenHuellasImputado(ComparationRequest[] Huellas)
        {
            if (Huellas == null)
                return false;

            if (!Huellas.Any())
                return false;

            var list = new MainRule<IMPUTADO_BIOMETRICO>().Listado().Where(w => w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 && w.BIOMETRICO != null).AsEnumerable().Select(s => new
                    {
                        IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data
                    })
                    .ToList();


            foreach (var item in Huellas)
            {
                if (item.BIOMETRICO.Length <= 0)
                    continue;

                var identify = true;

                var doIdentify = Comparison.Identify(Importer.ImportFmd(item.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, list.Where(w => w.FMD != null).Select(s => s.FMD), (0x7fffffff / 100000), 10);

                if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    identify = false;

                if (doIdentify.Indexes.Count() > 0)
                    identify = true;
                else
                    identify = false;

                if (!identify)
                    return false;
            }
            return true;
        }

        public CompareResponseImputado CompararHuellaImputado(ComparationRequest DataCompare)
        {
            var resultset = new CompareResponseImputado();
            try
            {
                var identify = true;
                var result = new List<cHuellasImputado>();
                var list = new MainRule<IMPUTADO_BIOMETRICO>().Listado().Where(w => w.ID_TIPO_BIOMETRICO == (short)DataCompare.ID_TIPO_BIOMETRICO && w.ID_FORMATO == (short)DataCompare.ID_TIPO_FORMATO && w.CALIDAD > 0 && w.BIOMETRICO != null).AsEnumerable().Select(s => new
                    {
                        IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data
                    })
                    .Where(w => w.FMD != null)
                    .ToList();

                var doIdentify = Comparison.Identify(Importer.ImportFmd(DataCompare.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, list.Select(s => s.FMD), (0x7fffffff / 100000), 10);

                if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    identify = false;
                if (doIdentify.Indexes.Count() > 0)
                {
                    foreach (var item in doIdentify.Indexes.ToList())
                        result.Add(list[item.FirstOrDefault()].IMPUTADO);
                    identify = true;
                }
                else
                    identify = false;

                resultset.Identify = identify;
                resultset.Result = result;
            }
            catch (Exception ex)
            {
                resultset.Identify = false;
                resultset.Result = new List<cHuellasImputado>();
                resultset.MensajeError = ex.InnerException.Message;
                return resultset;
            }
            return resultset;
        }

        public CompareResponseImputado CompararHuellaImputadoPorUbicacion(ComparationRequest DataCompare)
        {
            var resultset = new CompareResponseImputado();
            try
            {
                var identify = true;
                var result = new List<cHuellasImputado>();
                var predicate = PredicateBuilder.True<IMPUTADO_BIOMETRICO>();
                //predicate = predicate.And(w => w.ID_TIPO_BIOMETRICO == (short)DataCompare.ID_TIPO_BIOMETRICO && w.ID_FORMATO == (short)DataCompare.ID_TIPO_FORMATO && w.CALIDAD > 0);
                //predicate = predicate.And(w => (w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9) && w.ID_FORMATO == (short)DataCompare.ID_TIPO_FORMATO && w.CALIDAD > 0 && w.BIOMETRICO != null);
                predicate = predicate.And(w => (w.TOMA == "S") && w.ID_FORMATO == (short)DataCompare.ID_TIPO_FORMATO && w.CALIDAD > 0 && w.BIOMETRICO != null);
                if (DataCompare.ID_CENTRO.HasValue && DataCompare.ID_EDIFICIO.HasValue && DataCompare.ID_SECTOR.HasValue)
                    predicate=predicate.And(w => w.IMPUTADO.INGRESO.Any(a=>a.ID_ESTATUS_ADMINISTRATIVO!=5 && a.ID_ESTATUS_ADMINISTRATIVO !=6
                    && a.ID_ESTATUS_ADMINISTRATIVO!=7 && a.ID_ESTATUS_ADMINISTRATIVO!=4 && a.ID_UB_CENTRO==DataCompare.ID_CENTRO && a.ID_UB_EDIFICIO==DataCompare.ID_EDIFICIO && a.ID_UB_SECTOR==DataCompare.ID_SECTOR));
                else if (DataCompare.ID_CENTRO.HasValue && DataCompare.ID_EDIFICIO.HasValue)
                    predicate = predicate.And(w => w.IMPUTADO.INGRESO.Any(a => a.ID_ESTATUS_ADMINISTRATIVO != 5 && a.ID_ESTATUS_ADMINISTRATIVO != 6
                    && a.ID_ESTATUS_ADMINISTRATIVO != 7 && a.ID_ESTATUS_ADMINISTRATIVO != 4 && a.ID_UB_CENTRO == DataCompare.ID_CENTRO && a.ID_UB_EDIFICIO == DataCompare.ID_EDIFICIO));
                else if (DataCompare.ID_CENTRO.HasValue)
                    predicate = predicate.And(w => w.IMPUTADO.INGRESO.Any(a => a.ID_ESTATUS_ADMINISTRATIVO != 5 && a.ID_ESTATUS_ADMINISTRATIVO != 6
                    && a.ID_ESTATUS_ADMINISTRATIVO != 7 && a.ID_ESTATUS_ADMINISTRATIVO != 4 && a.ID_UB_CENTRO == DataCompare.ID_CENTRO));

                var list = new MainRule<IMPUTADO_BIOMETRICO>().Listado().AsExpandable().Where(predicate).AsEnumerable().Select(s => new
                    {
                        IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data
                    })
                    .Where(w => w.FMD != null)
                    .ToList();

                var doIdentify = Comparison.Identify(Importer.ImportFmd(DataCompare.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, list.Select(s => s.FMD), (0x7fffffff / 100000), 10);

                if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    identify = false;
                if (doIdentify.Indexes != null)
                {
                    if (doIdentify.Indexes.Count() > 0)
                    {
                        foreach (var item in doIdentify.Indexes.ToList())
                            result.Add(list[item.FirstOrDefault()].IMPUTADO);
                        identify = true;
                    }
                    else
                        identify = false;
                }
                else
                    identify = false;
                resultset.Identify = identify;
                resultset.Result = result;
             
            }
            catch(Exception ex)
            {
                resultset.Identify = false;
                resultset.Result = new List<cHuellasImputado>();
                resultset.MensajeError = ex.InnerException.Message;
                return resultset;
            }
            return resultset;
        }

        public bool Conexion()
        {
            return new MainRule<IMPUTADO_BIOMETRICO>().Conexion();
        }

        #endregion

        #region [PERSONA]
        public bool InsertarHuellaPersona(PERSONA_BIOMETRICO Data)
        {
            try
            {
                var rlePersona = new MainRule<PERSONA_BIOMETRICO>();
                rlePersona.Insertar(Data);
                return true;
            }
            catch
            {
                RollBackPersona(Data);
                return false;
            }
        }

        public bool InsertarHuellasPersona(PERSONA_BIOMETRICO[] Data)
        {
            try
            {
                var rlePersona = new MainRule<PERSONA_BIOMETRICO>();
                rlePersona.Insertar(Data);
                return true;
            }
            catch
            {
                RollBackPersona(Data.FirstOrDefault());
                return false;
            }
        }

        public bool ActualizarHuellasPersona(PERSONA_BIOMETRICO[] Data)
        {
            try
            {
                var rlePersona = new MainRule<PERSONA_BIOMETRICO>();
                rlePersona.Actualizar(Data);
                return true;
            }
            catch
            {
                RollBackPersona(Data.FirstOrDefault());
                return false;
            }
        }

        public bool RollBackPersona(PERSONA_BIOMETRICO Data)
        {
            try
            {
                if (Data == null)
                    return false;

                var rlePersona = new MainRule<PERSONA_BIOMETRICO>();
                rlePersona.Eliminar(rlePersona.Listado().Where(w => w.ID_PERSONA == Data.ID_PERSONA).ToList());
                return true;
            }
            catch { return false; }
        }

        public CompareResponsePersona CompararHuellaPersona(ComparationRequest DataCompare)
        {
            var resultset = new CompareResponsePersona();
            try
            {
                var identify = true;
                var result = new List<cHuellasPersona>();
                var list = new MainRule<PERSONA_BIOMETRICO>().Listado()
                    .Where(w => w.ID_TIPO_BIOMETRICO == (short)DataCompare.ID_TIPO_BIOMETRICO && (DataCompare.ID_TIPO_PERSONA.HasValue ? w.PERSONA.ID_TIPO_PERSONA == (short?)DataCompare.ID_TIPO_PERSONA : w.PERSONA.ID_TIPO_PERSONA != null) && w.ID_FORMATO == (short)DataCompare.ID_TIPO_FORMATO)
                    .AsEnumerable()
                    .Select(s => new
                    {
                        PERSONA = new cHuellasPersona { ID_PERSONA = s.ID_PERSONA, NORIGINAL = s.NORIGINAL },
                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data
                    })
                    .ToList();
                var _lista_filtrada = list.Where(w => w.FMD != null);
                var doIdentify = Comparison.Identify(Importer.ImportFmd(DataCompare.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, _lista_filtrada.Select(s => s.FMD), (0x7fffffff / 100000), 10);

                if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    identify = false;
                if (doIdentify.Indexes.Count() > 0)
                {
                    foreach (var item in doIdentify.Indexes.ToList())
                        result.Add(_lista_filtrada.ElementAt(item.FirstOrDefault()).PERSONA);
                    identify = true;
                }
                else
                    identify = false;

                resultset.Identify = identify;
                resultset.Result = result;
            }
            catch
            {
                resultset.Identify = false;
                resultset.Result = new List<cHuellasPersona>();

                return resultset;
            }
            return resultset;
        }
        #endregion

        //#region [PRUEBA DE ESTRESS]
        //#region [IMPUTADO]
        //public bool PRUEBAInsertarHuellaImputado(IMPUTADO_BIOMETRICO2 Data)
        //{
        //    try
        //    {
        //        var rleImputado = new MainRule<IMPUTADO_BIOMETRICO2>();
        //        rleImputado.Insertar(Data);
        //        return true;
        //    }
        //    catch
        //    {
        //        PRUEBARollBackImputado(Data);
        //        return false;
        //    }
        //}

        //public bool PRUEBAInsertarHuellasImputado(IMPUTADO_BIOMETRICO2[] Data)
        //{
        //    try
        //    {
        //        var rleImputado = new MainRule<IMPUTADO_BIOMETRICO2>();
        //        rleImputado.Insertar(Data);
        //        return true;
        //    }
        //    catch
        //    {
        //        PRUEBARollBackImputado(Data.FirstOrDefault());
        //        return false;
        //    }
        //}

        //public bool PRUEBAActualizarHuellasImputado(IMPUTADO_BIOMETRICO2[] Data)
        //{
        //    try
        //    {
        //        var rleImputado = new MainRule<IMPUTADO_BIOMETRICO2>();
        //        rleImputado.Actualizar(Data);
        //        return true;
        //    }
        //    catch
        //    {
        //        PRUEBARollBackImputado(Data.FirstOrDefault());
        //        return false;
        //    }
        //}

        //public bool PRUEBARollBackImputado(IMPUTADO_BIOMETRICO2 Data)
        //{
        //    try
        //    {
        //        if (Data == null)
        //            return false;

        //        var rleImputado = new MainRule<IMPUTADO_BIOMETRICO2>();
        //        rleImputado.Eliminar(rleImputado.Listado().Where(w => w.ID_ANIO == Data.ID_ANIO && w.ID_CENTRO == Data.ID_CENTRO && w.ID_IMPUTADO == Data.ID_IMPUTADO).ToList());
        //        return true;
        //    }
        //    catch { return false; }
        //}

        //public bool PRUEBAExistenHuellasImputado(ComparationRequest[] Huellas)
        //{
        //    if (Huellas == null)
        //        return false;

        //    if (!Huellas.Any())
        //        return false;

        //    var list = new MainRule<IMPUTADO_BIOMETRICO2>().Listado().Where(w => w.ID_TIPO_BIOMETRICO >= 11 && w.ID_TIPO_BIOMETRICO <= 20 && w.CALIDAD > 0 && w.BIOMETRICO != null).AsEnumerable().Select(s => new
        //    {
        //        IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
        //        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data
        //    })
        //            .ToList();


        //    foreach (var item in Huellas)
        //    {
        //        if (item.BIOMETRICO.Length <= 0)
        //            continue;

        //        var identify = true;

        //        var doIdentify = Comparison.Identify(Importer.ImportFmd(item.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, list.Where(w => w.FMD != null).Select(s => s.FMD), (0x7fffffff / 100000), 10);

        //        if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
        //            identify = false;

        //        if (doIdentify.Indexes.Count() > 0)
        //            identify = true;
        //        else
        //            identify = false;

        //        if (!identify)
        //            return false;
        //    }
        //    return true;
        //}

        //public CompareResponseImputado PRUEBACompararHuellaImputado(ComparationRequest DataCompare)
        //{
        //    var resultset = new CompareResponseImputado();
        //    try
        //    {
        //        var identify = true;
        //        var result = new List<cHuellasImputado>();
        //        var list = new MainRule<IMPUTADO_BIOMETRICO2>().Listado().Where(w => w.ID_TIPO_BIOMETRICO == (short)DataCompare.ID_TIPO_BIOMETRICO && w.CALIDAD > 0).AsEnumerable().Select(s => new
        //        {
        //            IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
        //            FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data
        //        })
        //            .ToList();

        //        var doIdentify = Comparison.Identify(Importer.ImportFmd(DataCompare.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, list.Where(w => w.FMD != null).Select(s => s.FMD), (0x7fffffff / 100000), 10);

        //        if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
        //            identify = false;
        //        if (doIdentify.Indexes.Count() > 0)
        //        {
        //            foreach (var item in doIdentify.Indexes.ToList())
        //                result.Add(list[item.FirstOrDefault()].IMPUTADO);
        //            identify = true;
        //        }
        //        else
        //            identify = false;

        //        resultset.Identify = identify;
        //        resultset.Result = result;
        //    }
        //    catch
        //    {
        //        resultset.Identify = false;
        //        resultset.Result = new List<cHuellasImputado>();

        //        return resultset;
        //    }
        //    return resultset;
        //}
        //#endregion
        //#endregion
    }
}

using System.ServiceModel;
using WCF_BiometricoService.Helpers;
using WCF_BiometricoService.Modelo;
using WCF_BiometricoService.Modelo.Entidades;

namespace WCF_BiometricoService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBiometricoService" in both code and config file together.
    [ServiceContract]
    public interface IBiometricoService
    {
        #region [Utilerias]
        [OperationContract]
        string GetData(int value);
        [OperationContract]
        byte[] DescargarWSQ(short ID_CENTRO, short ID_ANIO, int ID_IMPUTADO, enumTipoBiometrico Dedo, enumTipoFormato Formato);
        //[OperationContract]
        //byte[] DescargarBMP(short ID_CENTRO, short ID_ANIO, int ID_IMPUTADO, enumTipoBiometrico Dedo);
        #endregion

        #region [IMPUTADO]
        [OperationContract]
        bool InsertarHuellaImputado(IMPUTADO_BIOMETRICO Data);
        [OperationContract]
        bool InsertarHuellasImputado(IMPUTADO_BIOMETRICO[] Data);
        [OperationContract]
        bool ActualizarHuellasImputado(IMPUTADO_BIOMETRICO[] Data);
        [OperationContract]
        bool RollBackImputado(IMPUTADO_BIOMETRICO Data);
        [OperationContract]
        bool ExistenHuellasImputado(ComparationRequest[] Huellas);
        [OperationContract]
        CompareResponseImputado CompararHuellaImputado(ComparationRequest DataCompare);
        [OperationContract]
        CompareResponseImputado CompararHuellaImputadoPorUbicacion(ComparationRequest DataCompare);
        [OperationContract]
        bool Conexion();
        #endregion

        #region [PERSONA]
        [OperationContract]
        bool InsertarHuellaPersona(PERSONA_BIOMETRICO Data);
        [OperationContract]
        bool InsertarHuellasPersona(PERSONA_BIOMETRICO[] Data);
        [OperationContract]
        bool ActualizarHuellasPersona(PERSONA_BIOMETRICO[] Data);
        [OperationContract]
        bool RollBackPersona(PERSONA_BIOMETRICO Data);
        [OperationContract]
        CompareResponsePersona CompararHuellaPersona(ComparationRequest DataCompare);
        #endregion

        //#region [PRUEBA DE ESTRESS]
        //#region [IMPUTADO]
        //[OperationContract]
        //bool PRUEBAInsertarHuellaImputado(IMPUTADO_BIOMETRICO2 Data);
        //[OperationContract]
        //bool PRUEBAInsertarHuellasImputado(IMPUTADO_BIOMETRICO2[] Data);
        //[OperationContract]
        //bool PRUEBAActualizarHuellasImputado(IMPUTADO_BIOMETRICO2[] Data);
        //[OperationContract]
        //bool PRUEBARollBackImputado(IMPUTADO_BIOMETRICO2 Data);
        //[OperationContract]
        //bool PRUEBAExistenHuellasImputado(ComparationRequest[] Huellas);
        //[OperationContract]
        //CompareResponseImputado PRUEBACompararHuellaImputado(ComparationRequest DataCompare);
        //#endregion
        //#endregion
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.

}

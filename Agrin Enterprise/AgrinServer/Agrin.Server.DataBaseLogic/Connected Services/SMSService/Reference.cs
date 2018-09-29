using SignalGo.Shared.DataTypes;
using System.Threading.Tasks;
using SignalGo.Shared.Models;
using System;
using SMSService.ServerServices;
using SMSService.HttpServices;
using SMSService.ClientServices;

namespace SMSService.ServerServices
{
}

namespace SMSService.StreamServices
{
}

namespace SMSService.OneWayServices
{
    [ServiceContract("smssenderonewayservice",ServiceType.OneWayService, InstanceType.SingleInstance)]
    public class SMSSenderController
    {
        public static SMSSenderController Current { get; set; }
        string _signalGoServerAddress = "";
        int _signalGoPortNumber = 0;
        public SMSSenderController(string signalGoServerAddress, int signalGoPortNumber)
        {
            _signalGoServerAddress = signalGoServerAddress;
            _signalGoPortNumber = signalGoPortNumber;
        }
         public Atitec.Models.MessageContract SendSMS(string userName, string password, string message, string toNumber)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Atitec.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "smssenderonewayservice", "SendSMS", new SignalGo.Shared.Models.ParameterInfo() {  Name = "userName", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(userName) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "password", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(password) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "message", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(message) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "toNumber", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(toNumber) });
        }
         public Task<Atitec.Models.MessageContract> SendSMSAsync(string userName, string password, string message, string toNumber)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethodAsync<Atitec.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "smssenderonewayservice", "SendSMS", new SignalGo.Shared.Models.ParameterInfo() {  Name = "userName", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(userName) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "password", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(password) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "message", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(message) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "toNumber", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(toNumber) });
        }
         public Atitec.Models.MessageContract SendSMSGroup(string userName, string password, string message, System.String[] toNumbers)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethod<Atitec.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "smssenderonewayservice", "SendSMSGroup", new SignalGo.Shared.Models.ParameterInfo() {  Name = "userName", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(userName) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "password", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(password) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "message", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(message) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "toNumbers", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(toNumbers) });
        }
         public Task<Atitec.Models.MessageContract> SendSMSGroupAsync(string userName, string password, string message, System.String[] toNumbers)
        {
                return SignalGo.Client.ClientProvider.SendOneWayMethodAsync<Atitec.Models.MessageContract>(_signalGoServerAddress, _signalGoPortNumber, "smssenderonewayservice", "SendSMSGroup", new SignalGo.Shared.Models.ParameterInfo() {  Name = "userName", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(userName) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "password", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(password) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "message", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(message) }, new SignalGo.Shared.Models.ParameterInfo() {  Name = "toNumbers", Value = SignalGo.Client.ClientSerializationHelper.SerializeObject(toNumbers) });
        }
    }
}

namespace SMSService.HttpServices
{
    public class SMSSenderController
    {
        Atitec.Models.MessageContract SendSMS(string userName, string password, string message, string toNumber)
        {
                throw new NotSupportedException();
        }
        Task<Atitec.Models.MessageContract> SendSMSAsync(string userName, string password, string message, string toNumber)
        {
                return System.Threading.Tasks.Task<Atitec.Models.MessageContract>.Factory.StartNew(() => throw new NotSupportedException());
        }
        Atitec.Models.MessageContract SendSMSGroup(string userName, string password, string message, System.String[] toNumbers)
        {
                throw new NotSupportedException();
        }
        Task<Atitec.Models.MessageContract> SendSMSGroupAsync(string userName, string password, string message, System.String[] toNumbers)
        {
                return System.Threading.Tasks.Task<Atitec.Models.MessageContract>.Factory.StartNew(() => throw new NotSupportedException());
        }
    }
}

namespace Atitec.Models
{
    public class MessageContract : SignalGo.Shared.Models.NotifyPropertyChangedBase
    {
        private object _Data;
        public object Data
        {
                get
                {
                        return _Data;
                }
                set
                {
                        _Data = value;
                        OnPropertyChanged(nameof(Data));
                }
        }

        private bool _IsSuccess;
        public bool IsSuccess
        {
                get
                {
                        return _IsSuccess;
                }
                set
                {
                        _IsSuccess = value;
                        OnPropertyChanged(nameof(IsSuccess));
                }
        }

        private string _Message;
        public string Message
        {
                get
                {
                        return _Message;
                }
                set
                {
                        _Message = value;
                        OnPropertyChanged(nameof(Message));
                }
        }

        private string _StackTrace;
        public string StackTrace
        {
                get
                {
                        return _StackTrace;
                }
                set
                {
                        _StackTrace = value;
                        OnPropertyChanged(nameof(StackTrace));
                }
        }

        private Atitec.Models.ErrorMessage _ErrorCode;
        public Atitec.Models.ErrorMessage ErrorCode
        {
                get
                {
                        return _ErrorCode;
                }
                set
                {
                        _ErrorCode = value;
                        OnPropertyChanged(nameof(ErrorCode));
                }
        }


    }

}

namespace SMSService.ClientServices
{
}

namespace Atitec.Models
{
    public enum ErrorMessage : int
    {
        None = 0,
        ContactNotFound = 1,
        DriverIdIsZero = 2,
        DriverNotFound = 3,
        CodeExistOnDataBase = 4,
        TablesToExportIsNullOrEmpty = 5,
        FileNotFound = 6,
        NotResponse = 7,
        ServiceNotFound = 8,
        TransportNotFound = 9,
        DriverIsNull = 10,
        PaymentIsNullOrZero = 11,
        PaymentNotFound = 12,
        IdsIsNull = 13,
        ContactInfoIsNull = 14,
        ContactNameIsEmpty = 15,
        ContactCodeIsNotTrue = 16,
        PhonesNotSet = 17,
        NumberNotSetForContactPhone = 18,
        PhoneExistOnDataBase = 19,
        DriverInfoIsNull = 20,
        DriverInfoIsNotForYourTransport = 21,
        UserNameExistOnDataBase = 22,
        PlaqueIsNull = 23,
        ServiceInfoIsNull = 24,
        TransportIsOffline = 25,
        RewardOffIdNotFound = 26,
        RewardInfoNotFound = 27,
        RewardIdNotFound = 28,
        PhoneNotFound = 29,
        KeyNotTrue = 30,
        ServiceIsNotSent = 31,
        YouCannotSetYourPhoneInYourReagentNumber = 32,
        ReagentNumberIsSetedOnce = 33,
        UsernameOrPasswordIncorrect = 34,
        IsNotConfirm = 35,
        TransportInfoIsNull = 36,
        TransportPhonesNotSet = 37,
        OldPasswordIncorrect = 38,
        DriverIsNotOnline = 39,
        DriverIsSent = 40,
        ReasonIsNullOrEmpty = 41,
        TransportServiceIsNotForYourService = 42,
        TransportSessionNotFound = 43,
        PleaseFillAllData = 44,
        NoCharge = 45,
        PassengerNotFound = 46,
        SMSNotFound = 47,
        WrongData = 48,
        SourceAddressNotFound = 49,
        DestAddressNotFound = 50,
        CodeNotExistOnDataBase = 51,
        AccessDenied = 52,
        IdNotFoundInDataBase = 53,
        SessionAccessDenied = 54,
        ServiceDiscountIdNotFound = 55,
        CodeIsUsed = 56,
        ServiceDiscountIsNotSetForYourId = 57,
        ServiceDiscountIsExpire = 58,
        ErrorToGetCharge = 59,
        ThisMethodCannotCallDoubleTime = 60,
        DataOverFlow = 61,
        CannotCallInThisTime = 62,
        DataIdIsWrongForYouAndYouHaveNotAccessToAnotherUserData = 63,
        OneOrMoreProcessIsWorking = 64,
        DuplicateCall = 65,
        ExpireDateTime = 66,
        TokenNotFound = 67,
        InternalServiceError = 68,
        APIError = 69,
        ContextIsNullOrDispose = 70,
        ListIsEmpty = 71,
        FormatIsNotValid = 72,
        OneOrMoreFieldIsNull = 73,
        ObjectIsNull = 74,
        PropertyLengthIsOverFlow = 75,
        DefaultDataNotFound = 76,
        PropertyIdNotFound = 77,
        ApplicationMustUpdateToNewVersion = 78,
        ClientIpIsNotValid = 79,
        CreditIsNotEnough = 80,
        LoopDetected = 81,
        NotSupportYet = 82,
    }

}


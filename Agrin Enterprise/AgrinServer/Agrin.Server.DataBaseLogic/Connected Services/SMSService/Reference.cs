using SignalGo.Shared.DataTypes;
using System.Threading.Tasks;
using SignalGo.Shared.Models;
using System;
using SMSService.ServerServices;
using SMSService.HttpServices;
using SMSService.ClientServices;
using Agrin.Server.Models;

namespace SMSService.ServerServices
{
}

namespace SMSService.StreamServices
{
}

namespace SMSService.OneWayServices
{
    [ServiceContract("SMSSender", ServiceType.OneWayService, InstanceType.SingleInstance)]
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
        public int CalculateCostOfSMS(int costPerSMS, int charCountPerSms, string message)
        {
            throw new NotImplementedException();
        }
        public Task<int> CalculateCostOfSMSAsync(int costPerSMS, int charCountPerSms, string message)
        {
            throw new NotImplementedException();
        }
        public MessageContract SendSMS(string userName, string password, string message, string toNumber)
        {
            throw new NotImplementedException();
        }
        public Task<MessageContract> SendSMSAsync(string userName, string password, string message, string toNumber)
        {
            throw new NotImplementedException();
        }
        public MessageContract SendSMS(string userName, string password, string message, System.String[] toNumbers)
        {
            throw new NotImplementedException();
        }
        public Task<MessageContract> SendSMSAsync(string userName, string password, string message, System.String[] toNumbers)
        {
            throw new NotImplementedException();
        }
    }
}

namespace SMSService.HttpServices
{
}

namespace SMSService.ClientServices
{
}


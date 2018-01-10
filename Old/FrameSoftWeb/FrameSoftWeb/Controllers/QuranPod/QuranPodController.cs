using Agrin.Log;
using Newtonsoft.Json;
using SignalGo.Client;
using SignalGo.Shared;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FrameSoftWeb.Controllers.QuranPod
{
    public class QuranPodMessageContract
    {
        public object Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public int ErrorCode { get; set; }
    }

    [ServiceContract("QuranPodService")]
    public interface IQuranPod
    {
        QuranPodMessageContract AddPaymentLog(string tran_id, string order_id, string amount, string refcode, string status);
    }
    public class QuranPodPaymentModel
    {
        public string TranId { get; set; }
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string RefCode { get; set; }
        public string Status { get; set; }
    }

    public static class QuranPodSendToSignalGo
    {
        static ClientProvider provider = new ClientProvider();
        static IQuranPod service = null;
        static IQuranPod Reconnect()
        {
            while (!provider.IsConnected)
            {
                try
                {
                    provider.Dispose();
                    provider = new ClientProvider();
                    provider.Connect("http://82.102.13.99:1879/QuranPodService");
                    service = provider.RegisterClientServiceInterface<IQuranPod>();
                    break;
                }
                catch (Exception ex)
                {
                    AutoLogger.LogError(ex, "QuranPodSendToSignalGo Reconnect");
                }
                Thread.Sleep(2000);
            }
            return service;
        }

        public static void SendToLog(QuranPodPaymentModel data)
        {
            AsyncActions.Run(() =>
            {
                var service = QuranPodSendToSignalGo.Reconnect();
                service.AddPaymentLog(data.TranId, data.OrderId, data.Amount, data.RefCode, data.Status);
            });
        }
    }

    public class QuranPodController : Controller
    {
        public ActionResult Payment(string tran_id, string order_id, string amount, string refcode, string status)
        {
            var data = new QuranPodPaymentModel() { Amount = amount, OrderId = order_id, RefCode = refcode, Status = status, TranId = tran_id };
            QuranPodSendToSignalGo.SendToLog(data);
            return Content(JsonConvert.SerializeObject(data));
        }
    }
}

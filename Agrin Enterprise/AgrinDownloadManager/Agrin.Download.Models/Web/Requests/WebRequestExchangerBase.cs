using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Agrin.Download.Web.Requests
{
    public enum RequestExchangerType
    {
        NetFrameworkWebRequest
    }

    public abstract class WebRequestExchangerBase : IDisposable
    {
        static WebRequestExchangerBase()
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                //ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            }
            catch (Exception ex)
            {
            }
        }
        static bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        continue;
                    }
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                        break;
                    }
                }
            }
            return isOk;
        }
        public abstract Stream ResponseStream { get; set; }
        
        public static WebRequestExchangerBase Create(RequestExchangerType type)
        {
            if (type == RequestExchangerType.NetFrameworkWebRequest)
                return new WebRequestExchanger();
            throw new Exception("WebRequestExchangerBase type not support");
        }

        public long ContentLength { get; set; } = -2;
        public Uri ResponseUri { get; set; }
        public WebHeaderCollection RequestHeaders { get; set; }
        public WebHeaderCollection ResponseHeaders { get; set; }

        public abstract void CreateRequest(string address, string authentication, int timeOut, IWebProxy proxy, CookieContainer cookieContainer,
            int connectionLimit, WebHeaderCollection customHeaders);

        public abstract void SetExpect100Continue(bool value);
        public abstract void AddRange(long range);
        public abstract void GetResponse();
        public abstract void GetResponseStream();

        public abstract void Dispose();

    }
}

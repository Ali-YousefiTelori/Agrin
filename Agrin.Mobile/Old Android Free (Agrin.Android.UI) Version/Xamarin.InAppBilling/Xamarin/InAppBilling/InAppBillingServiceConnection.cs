using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Com.Android.Vending.Billing;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using Xamarin.InAppBilling.Utilities;

namespace Xamarin.InAppBilling
{


    public class InAppBillingServiceConnection : Java.Lang.Object, IServiceConnection, IJavaObject, IDisposable
    {
        private Activity _activity;
        private string _publicKey;
        private const string Tag = "Iab Helper";

        public event OnConnectedDelegate OnConnected;

        public event OnDisconnectedDelegate OnDisconnected;

        public event OnInAppBillingErrorDelegate OnInAppBillingError;

        public InAppBillingServiceConnection(Activity activity, string publicKey)
        {
            this._activity = activity;
            this.PublicKey = publicKey;
        }

        public void Connect()
        {
            Intent intent = new Intent("ir.cafebazaar.pardakht.InAppBillingService.BIND");
            intent.SetPackage("com.farsitel.bazaar");
            IList<ResolveInfo> list = this._activity.PackageManager.QueryIntentServices(intent, 0);
            if (list == null)
            {
                this.RaiseOnInAppBillingError(InAppBillingErrorType.BillingNotSupported, "Unable to bind with com.android.vending.billing.InAppBillingService API.");
                this.Connected = false;
            }
            else if (list.Count != 0)
            {
                this._activity.BindService(intent, this, Bind.AutoCreate);
            }
            else
            {
                this.RaiseOnInAppBillingError(InAppBillingErrorType.BillingNotSupported, "Unable to access the com.android.vending service.");
                this.Connected = false;
            }
        }

        public void Disconnect()
        {
            this._activity.UnbindService(this);
            this.Connected = false;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Logger.Debug("Billing service connected.", new object[0]);
            this.Service = IInAppBillingServiceStub.AsInterface(service);
            string packageName = this._activity.PackageName;
            try
            {
                if (this.Service.IsBillingSupported(3, packageName, "inapp") != 0)
                {
                    object[] args = new object[] { packageName };
                    Logger.Debug("In-app billing version 3 NOT supported for {0}", args);
                    this.RaiseOnInAppBillingError(InAppBillingErrorType.BillingNotSupported, string.Format("In-app billing version 3 NOT supported for {0}", packageName));
                    this.Connected = false;
                }
                else
                {
                    object[] objArray2 = new object[] { packageName };
                    Logger.Debug("In-app billing version 3 supported for {0}", objArray2);
                    int num = this.Service.IsBillingSupported(3, packageName, "subs");
                    if (num == 0)
                    {
                        Logger.Debug("Subscriptions AVAILABLE.", new object[0]);
                        this.Connected = true;
                        this.RaiseOnConnected();
                    }
                    else
                    {
                        object[] objArray3 = new object[] { num };
                        Logger.Debug("Subscriptions NOT AVAILABLE. Response: {0}", objArray3);
                        this.RaiseOnInAppBillingError(InAppBillingErrorType.SubscriptionsNotSupported, string.Format("Subscriptions NOT AVAILABLE. Response: {0}", num));
                        this.Connected = false;
                    }
                }
            }
            catch (System.Exception exception)
            {
                Logger.Debug(exception.ToString(), new object[0]);
                this.RaiseOnInAppBillingError(InAppBillingErrorType.UnknownError, exception.ToString());
                this.Connected = false;
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            this.Connected = false;
            this.Service = null;
            this.BillingHandler = null;
            this.RaiseOnDisconnected();
        }

        protected void RaiseOnConnected()
        {
            if (this.Connected)
            {
                this.BillingHandler = new InAppBillingHandler(this._activity, this.Service, this.PublicKey);
                if (this.OnConnected != null)
                {
                    this.OnConnected();
                }
            }
        }

        protected virtual void RaiseOnDisconnected()
        {
            if (this.OnDisconnected != null)
            {
                this.OnDisconnected();
            }
        }

        protected virtual void RaiseOnInAppBillingError(InAppBillingErrorType error, string message)
        {
            if (this.OnInAppBillingError != null)
            {
                this.OnInAppBillingError(error, message);
            }
        }

        public InAppBillingHandler BillingHandler { get; private set; }

        public bool Connected { get; private set; }

        public string PublicKey
        {
            get
            {
                return Crypto.Decrypt(this._publicKey, this._activity.PackageName);
            }
            set
            {
                this._publicKey = Crypto.Encrypt(value, this._activity.PackageName);
            }
        }

        public IInAppBillingService Service { get; private set; }

        public delegate void OnConnectedDelegate();

        public delegate void OnDisconnectedDelegate();

        public delegate void OnInAppBillingErrorDelegate(InAppBillingErrorType error, string message);
    }
}


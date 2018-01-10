using Android.OS;
using Android.Runtime;
using Java.Lang;
using System;

namespace Com.Android.Vending.Billing
{

    public abstract class IInAppBillingServiceStub : Binder, IInterface, IInAppBillingService, IJavaObject, IDisposable
    {
        private const string descriptor = "com.android.vending.billing.IInAppBillingService";
        internal const int TransactionConsumePurchase = 5;
        internal const int TransactionGetBuyIntent = 3;
        internal const int TransactionGetPurchases = 4;
        internal const int TransactionGetSkuDetails = 2;
        internal const int TransactionIsBillingSupported = 1;

        public IInAppBillingServiceStub()
        {
            this.AttachInterface(this, "com.android.vending.billing.IInAppBillingService");
        }

        public IBinder AsBinder()
        {
            return this;
        }

        public static IInAppBillingService AsInterface(IBinder obj)
        {
            if (obj == null)
            {
                return null;
            }
            IInterface interface2 = obj.QueryLocalInterface("com.android.vending.billing.IInAppBillingService");
            if ((interface2 != null) && (interface2 is IInAppBillingService))
            {
                return (IInAppBillingService) interface2;
            }
            return new Proxy(obj);
        }

        public abstract int ConsumePurchase(int apiVersion, string packageName, string purchaseToken);
        public abstract Bundle GetBuyIntent(int apiVersion, string packageName, string sku, string type, string developerPayload);
        public abstract Bundle GetPurchases(int apiVersion, string packageName, string type, string continuationToken);
        public abstract Bundle GetSkuDetails(int apiVersion, string packageName, string type, Bundle skusBundle);
        public abstract int IsBillingSupported(int apiVersion, string packageName, string type);
        protected override bool OnTransact(int code, Parcel data, Parcel reply, int flags)
        {
            int num3;
            string str3;
            string str4;
            switch (code)
            {
                case 1:
                {
                    data.EnforceInterface("com.android.vending.billing.IInAppBillingService");
                    int apiVersion = 0;
                    apiVersion = data.ReadInt();
                    string packageName = null;
                    packageName = data.ReadString();
                    string type = null;
                    type = data.ReadString();
                    int num2 = this.IsBillingSupported(apiVersion, packageName, type);
                    reply.WriteNoException();
                    reply.WriteInt(num2);
                    data.WriteInt(apiVersion);
                    data.WriteString(packageName);
                    data.WriteString(type);
                    return true;
                }
                case 2:
                {
                    data.EnforceInterface("com.android.vending.billing.IInAppBillingService");
                    num3 = 0;
                    num3 = data.ReadInt();
                    str3 = null;
                    str3 = data.ReadString();
                    str4 = null;
                    str4 = data.ReadString();
                    Bundle skusBundle = null;
                    skusBundle = (data.ReadInt() == 0) ? null : ((Bundle) Bundle.Creator.CreateFromParcel(data));
                    Bundle bundle2 = this.GetSkuDetails(num3, str3, str4, skusBundle);
                    reply.WriteNoException();
                    if (bundle2 == null)
                    {
                        reply.WriteInt(0);
                        break;
                    }
                    reply.WriteInt(1);
                    bundle2.WriteToParcel(reply, ParcelableWriteFlags.ReturnValue);
                    break;
                }
                case 3:
                {
                    data.EnforceInterface("com.android.vending.billing.IInAppBillingService");
                    int num4 = 0;
                    num4 = data.ReadInt();
                    string str5 = null;
                    str5 = data.ReadString();
                    string sku = null;
                    sku = data.ReadString();
                    string str7 = null;
                    str7 = data.ReadString();
                    string developerPayload = null;
                    developerPayload = data.ReadString();
                    Bundle bundle3 = this.GetBuyIntent(num4, str5, sku, str7, developerPayload);
                    reply.WriteNoException();
                    if (bundle3 == null)
                    {
                        reply.WriteInt(0);
                    }
                    else
                    {
                        reply.WriteInt(1);
                        bundle3.WriteToParcel(reply, ParcelableWriteFlags.ReturnValue);
                    }
                    data.WriteInt(num4);
                    data.WriteString(str5);
                    data.WriteString(sku);
                    data.WriteString(str7);
                    data.WriteString(developerPayload);
                    return true;
                }
                case 4:
                {
                    data.EnforceInterface("com.android.vending.billing.IInAppBillingService");
                    int num5 = 0;
                    num5 = data.ReadInt();
                    string str9 = null;
                    str9 = data.ReadString();
                    string str10 = null;
                    str10 = data.ReadString();
                    string continuationToken = null;
                    continuationToken = data.ReadString();
                    Bundle bundle4 = this.GetPurchases(num5, str9, str10, continuationToken);
                    reply.WriteNoException();
                    if (bundle4 == null)
                    {
                        reply.WriteInt(0);
                    }
                    else
                    {
                        reply.WriteInt(1);
                        bundle4.WriteToParcel(reply, ParcelableWriteFlags.ReturnValue);
                    }
                    data.WriteInt(num5);
                    data.WriteString(str9);
                    data.WriteString(str10);
                    data.WriteString(continuationToken);
                    return true;
                }
                case 5:
                {
                    data.EnforceInterface("com.android.vending.billing.IInAppBillingService");
                    int num6 = 0;
                    num6 = data.ReadInt();
                    string str12 = null;
                    str12 = data.ReadString();
                    string purchaseToken = null;
                    purchaseToken = data.ReadString();
                    int num7 = this.ConsumePurchase(num6, str12, purchaseToken);
                    reply.WriteNoException();
                    reply.WriteInt(num7);
                    data.WriteInt(num6);
                    data.WriteString(str12);
                    data.WriteString(purchaseToken);
                    return true;
                }
                case 0x5f4e5446:
                    reply.WriteString("com.android.vending.billing.IInAppBillingService");
                    return true;

                default:
                    return base.OnTransact(code, data, reply, flags);
            }
            data.WriteInt(num3);
            data.WriteString(str3);
            data.WriteString(str4);
            return true;
        }

        public class Proxy : Java.Lang.Object, IInAppBillingService, IInterface, IJavaObject, IDisposable
        {
            private IBinder remote;

            public Proxy(IBinder remote)
            {
                this.remote = remote;
            }

            public IBinder AsBinder()
            {
                return this.remote;
            }

            public int ConsumePurchase(int apiVersion, string packageName, string purchaseToken)
            {
                Parcel parcel = Parcel.Obtain();
                Parcel parcel2 = Parcel.Obtain();
                int num = 0;
                try
                {
                    parcel.WriteInterfaceToken("com.android.vending.billing.IInAppBillingService");
                    parcel.WriteInt(apiVersion);
                    parcel.WriteString(packageName);
                    parcel.WriteString(purchaseToken);
                    this.remote.Transact(5, parcel, parcel2, 0);
                    parcel2.ReadException();
                    num = parcel2.ReadInt();
                }
                finally
                {
                    parcel2.Recycle();
                    parcel.Recycle();
                }
                return num;
            }

            public Bundle GetBuyIntent(int apiVersion, string packageName, string sku, string type, string developerPayload)
            {
                Parcel parcel = Parcel.Obtain();
                Parcel parcel2 = Parcel.Obtain();
                Bundle bundle = null;
                try
                {
                    parcel.WriteInterfaceToken("com.android.vending.billing.IInAppBillingService");
                    parcel.WriteInt(apiVersion);
                    parcel.WriteString(packageName);
                    parcel.WriteString(sku);
                    parcel.WriteString(type);
                    parcel.WriteString(developerPayload);
                    this.remote.Transact(3, parcel, parcel2, 0);
                    parcel2.ReadException();
                    bundle = (parcel2.ReadInt() == 0) ? null : ((Bundle) Bundle.Creator.CreateFromParcel(parcel2));
                }
                finally
                {
                    parcel2.Recycle();
                    parcel.Recycle();
                }
                return bundle;
            }

            public string GetInterfaceDescriptor()
            {
                return "com.android.vending.billing.IInAppBillingService";
            }

            public Bundle GetPurchases(int apiVersion, string packageName, string type, string continuationToken)
            {
                Parcel parcel = Parcel.Obtain();
                Parcel parcel2 = Parcel.Obtain();
                Bundle bundle = null;
                try
                {
                    parcel.WriteInterfaceToken("com.android.vending.billing.IInAppBillingService");
                    parcel.WriteInt(apiVersion);
                    parcel.WriteString(packageName);
                    parcel.WriteString(type);
                    parcel.WriteString(continuationToken);
                    this.remote.Transact(4, parcel, parcel2, 0);
                    parcel2.ReadException();
                    bundle = (parcel2.ReadInt() == 0) ? null : ((Bundle) Bundle.Creator.CreateFromParcel(parcel2));
                }
                finally
                {
                    parcel2.Recycle();
                    parcel.Recycle();
                }
                return bundle;
            }

            public Bundle GetSkuDetails(int apiVersion, string packageName, string type, Bundle skusBundle)
            {
                Parcel parcel = Parcel.Obtain();
                Parcel parcel2 = Parcel.Obtain();
                Bundle bundle = null;
                try
                {
                    parcel.WriteInterfaceToken("com.android.vending.billing.IInAppBillingService");
                    parcel.WriteInt(apiVersion);
                    parcel.WriteString(packageName);
                    parcel.WriteString(type);
                    if (skusBundle != null)
                    {
                        parcel.WriteInt(1);
                        skusBundle.WriteToParcel(parcel, 0);
                    }
                    else
                    {
                        parcel.WriteInt(0);
                    }
                    this.remote.Transact(2, parcel, parcel2, 0);
                    parcel2.ReadException();
                    bundle = (parcel2.ReadInt() == 0) ? null : ((Bundle) Bundle.Creator.CreateFromParcel(parcel2));
                }
                finally
                {
                    parcel2.Recycle();
                    parcel.Recycle();
                }
                return bundle;
            }

            public int IsBillingSupported(int apiVersion, string packageName, string type)
            {
                Parcel parcel = Parcel.Obtain();
                Parcel parcel2 = Parcel.Obtain();
                int num = 0;
                try
                {
                    parcel.WriteInterfaceToken("com.android.vending.billing.IInAppBillingService");
                    parcel.WriteInt(apiVersion);
                    parcel.WriteString(packageName);
                    parcel.WriteString(type);
                    this.remote.Transact(1, parcel, parcel2, 0);
                    parcel2.ReadException();
                    num = parcel2.ReadInt();
                }
                finally
                {
                    parcel2.Recycle();
                    parcel.Recycle();
                }
                return num;
            }
        }
    }
}


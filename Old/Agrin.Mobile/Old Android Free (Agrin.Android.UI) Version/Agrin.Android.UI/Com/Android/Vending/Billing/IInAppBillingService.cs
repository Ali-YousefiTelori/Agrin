using Android.OS;
using Android.Runtime;
using System;

namespace Com.Android.Vending.Billing
{
    public interface IInAppBillingService : IInterface, IJavaObject, IDisposable
    {
        int ConsumePurchase(int apiVersion, string packageName, string purchaseToken);
        Bundle GetBuyIntent(int apiVersion, string packageName, string sku, string type, string developerPayload);
        Bundle GetPurchases(int apiVersion, string packageName, string type, string continuationToken);
        Bundle GetSkuDetails(int apiVersion, string packageName, string type, Bundle skusBundle);
        int IsBillingSupported(int apiVersion, string packageName, string type);
    }
}


namespace Xamarin.InAppBilling
{
    using Android.App;
    using Android.Content;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IInAppBillingHandler
    {
        void BuyProduct(Product product);
        void BuyProduct(string sku, string itemType, string payload);
        bool ConsumePurchase(string token);
        bool ConsumePurchase(Purchase purchase);
        IList<Purchase> GetPurchases(string itemType);
        void HandleActivityResult(int requestCode, Result resultCode, Intent data);
        Task<IList<Product>> QueryInventoryAsync(IList<string> skuList, string itemType);
    }
}


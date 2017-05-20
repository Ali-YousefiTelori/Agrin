// Type: Xamarin.InAppBilling.InAppBillingHandler
// Assembly: Xamarin.InAppBilling, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 312DE16D-65DB-4E32-94A1-D49884001984
// Assembly location: D:\Projects\xamarin.inappbilling-2.2\xamarin.inappbilling-2.2\lib\android\Xamarin.InAppBilling.dll

using Android.App;
using Android.Content;
using Android.OS;
using Com.Android.Vending.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xamarin.InAppBilling.Utilities;

namespace Xamarin.InAppBilling
{



    /// <summary>
    /// The <see cref="T:Xamarin.InAppBilling.InAppBillingHandler"/> is a helper class that handles communication with the
    ///             Google Play services to provide support for getting a list of available products, buying a product, consuming a
    ///             product, and getting a list of already owned products.
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public class InAppBillingHandler : IInAppBillingHandler
    {
        [CompilerGenerated]
        private static Func<string, Product> f__mgcache0;

        [CompilerGenerated]
        private sealed class c__AnonStorey0
        {
            // Fields
            internal InAppBillingHandler This;
            internal string itemType;
            internal IList<string> skuList;

            // Methods
            internal IList<Product> m__0()
            {
                try
                {
                    Bundle skusBundle = new Bundle();
                    skusBundle.PutStringArrayList("ITEM_ID_LIST", this.skuList);
                    Bundle skuDetails = this.This._billingService.GetSkuDetails(3, this.This._activity.PackageName, this.itemType, skusBundle);
                    int @int = skuDetails.GetInt("RESPONSE_CODE");
                    if (@int == 0)
                    {
                        IList<string> stringArrayList = skuDetails.GetStringArrayList("DETAILS_LIST");
                        if (InAppBillingHandler.f__mgcache0 == null)
                        {
                            InAppBillingHandler.f__mgcache0 = new Func<string, Product>(DeserializeObject<Product>);
                        }
                        return ((stringArrayList != null) ? (Enumerable.Select<string, Product>(stringArrayList, InAppBillingHandler.f__mgcache0).ToList<Product>()) : null);
                    }
                    this.This.RaiseQueryInventoryError(@int, skuDetails);
                    return null;
                }
                catch (Exception exception)
                {
                    this.This.RaiseInAppBillingProcessingError(string.Format("Error Available Inventory: {0}", exception.ToString()));
                    return null;
                }
            }
        }

        public static T DeserializeObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
        private const int PurchaseRequestCode = 1001;
        private Activity _activity;
        private string _payload;
        private IInAppBillingService _billingService;
        private string _publicKey;

        /// <summary>
        /// Gets or sets the Google Play Service public key used for In-App Billing
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The public key.
        /// </value>
        /// 
        /// <remarks>
        /// NOTE: The key will be encrypted when it is stored in memory.
        /// </remarks>
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

        /// <summary>
        /// Raised where there is an error getting previously purchased products from the Google Play Services.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public event InAppBillingHandler.OnGetProductsErrorDelegate OnGetProductsError;

        /// <summary>
        /// Occurs when there is an error querying inventory from Google Play Services.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public event InAppBillingHandler.QueryInventoryErrorDelegate QueryInventoryError;

        /// <summary>
        /// Occurs when the user attempts to buy a product and there is an error.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public event InAppBillingHandler.BuyProductErrorDelegate BuyProductError;

        /// <summary>
        /// Occurs when there is an in app billing procesing error.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public event InAppBillingHandler.InAppBillingProcessingErrorDelegate InAppBillingProcesingError;

        /// <summary>
        /// Raised when Google Play Services returns an invalid bundle from previously purchased items
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public event InAppBillingHandler.OnInvalidOwnedItemsBundleReturnedDelegate OnInvalidOwnedItemsBundleReturned;

        /// <summary>
        /// Occurs when the is an error on a product purchase attempt.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public event InAppBillingHandler.OnProductPurchaseErrorDelegate OnProductPurchasedError;

        /// <summary>
        /// Occurs when a previously purchased product fails to validate.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public event InAppBillingHandler.OnPurchaseFailedValidationDelegate OnPurchaseFailedValidation;

        /// <summary>
        /// Occurs after a product has been successfully purchased Google Play.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// This event is fired after a <c>OnProductPurchased</c> which is raised when the user successfully
        ///             logs an intent to purchase with Google Play.
        /// </remarks>
        public event InAppBillingHandler.OnProductPurchasedDelegate OnProductPurchased;

        /// <summary>
        /// Occurs when there is an error consuming a product.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public event InAppBillingHandler.OnPurchaseConsumedErrorDelegate OnPurchaseConsumedError;

        /// <summary>
        /// Occurs when on product consumed.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public event InAppBillingHandler.OnPurchaseConsumedDelegate OnPurchaseConsumed;

        /// <summary>
        /// Occurs when on user canceled.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public event InAppBillingHandler.OnUserCanceledDelegate OnUserCanceled;

        public InAppBillingHandler(Activity activity, IInAppBillingService billingService, string publicKey)
        {
            this._billingService = billingService;
            this._activity = activity;
            this.PublicKey = publicKey;
        }

        /// <param name="skuList">Sku list.</param><param name="itemType">The <see cref="!:Xamarin.Android.InAppBilling.ItemType"/> of product being queried.</param>
        /// <summary>
        /// Queries the inventory asynchronously and returns a list of <see cref="!:Xamarin.Android.InAppBilling.Product"/>s matching
        ///             the given list of SKU numbers.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// List of <see cref="!:Xamarin.Android.InAppBilling.Product"/>s matching the given list of SKUs.
        /// </returns>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public Task<IList<Product>> QueryInventoryAsync(IList<string> skuList, string itemType)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: reference to a compiler-generated method
            return Task.Factory.StartNew<IList<Product>>(new Func<IList<Product>>(new InAppBillingHandler.c__AnonStorey0()
            {
                skuList = skuList,
                itemType = itemType,
                This = this
            }.m__0));
        }

        /// <param name="product">The <see cref="!:Xamarin.Android.InAppBilling.Product"/> representing the item the users wants to
        ///             purchase.</param>
        /// <summary>
        /// Buys the given <see cref="!:Xamarin.Android.InAppBilling.Product"/>
        /// </summary>
        /// 
        /// <remarks>
        /// This method automatically generates a unique GUID and attaches it as the developer payload for this purchase.
        /// 
        /// </remarks>
        public void BuyProduct(Product product)
        {
            this._payload = Guid.NewGuid().ToString();
            this.BuyProduct(product.ProductId, product.Type, this._payload);
        }

        /// <param name="product">The <see cref="!:Xamarin.Android.InAppBilling.Product"/> representing the item the users wants to
        ///             purchase.</param><param name="payload">The developer payload to attach to the purchase.</param>
        /// <summary>
        /// Buys the given <see cref="!:Xamarin.Android.InAppBilling.Product"/> and attaches the given developer payload to the
        ///             purchase.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public void BuyProduct(Product product, string payload)
        {
            this.BuyProduct(product.ProductId, product.Type, payload);
        }

        /// <param name="sku">The SKU of the item to purchase.</param><param name="itemType">The type of the item to purchase.</param><param name="payload">The developer payload to attach to the purchase.</param>
        /// <summary>
        /// Buys a product based on the given product SKU and Item Type attaching the given payload
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public void BuyProduct(string sku, string itemType, string payload)
        {
            try
            {
                Bundle buyIntent = this._billingService.GetBuyIntent(3, this._activity.PackageName, sku, itemType, payload);
                int @int = buyIntent.GetInt("RESPONSE_CODE");
                if (@int == 0)
                {
                    PendingIntent pendingIntent = buyIntent.GetParcelable("BUY_INTENT") as PendingIntent;
                    if (pendingIntent == null)
                        return;
                    this._activity.StartIntentSenderForResult(pendingIntent.IntentSender, 1001, new Intent(), (ActivityFlags)0, (ActivityFlags)0, 0);
                }
                else
                    this.RaiseBuyProductError(@int, sku);
            }
            catch (Exception ex)
            {
                this.RaiseInAppBillingProcessingError(string.Format("Error Buy Product: {0}", (object)((object)ex).ToString()));
            }
        }

        /// <param name="purchase">The purchase receipt of the item to consume.</param>
        /// <summary>
        /// Consumes the purchased item.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// <c>true</c> if the purchase is successfully consumed else returns <c>false</c>.
        /// </returns>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public bool ConsumePurchase(Purchase purchase)
        {
            if (purchase == null)
                throw new ArgumentNullException("Purchase receipt is null");
            else
                return this.ConsumePurchase(purchase.PurchaseToken);
        }

        /// <param name="token">The purchase token of the purchase to consume.</param>
        /// <summary>
        /// Consumes the purchased item
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// <c>true</c> if the purchase is successfully consumed else returns <c>false</c>.
        /// </returns>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public bool ConsumePurchase(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Purchase token is null");
            }
            try
            {
                int responseCode = this._billingService.ConsumePurchase(3, this._activity.PackageName, token);
                object[] args = new object[] { token, responseCode };
                Logger.Info("Consuming purchase '{0}', response: {1}", args);
                if (responseCode == 0)
                {
                    this.RaiseOnPurchaseConsumed(token);
                    return true;
                }
                object[] objArray2 = new object[] { token, responseCode };
                Logger.Error("Unable to consume '{0}', response: {1}", objArray2);
                this.RaiseOnPurchaseConsumedError(responseCode, token);
                return false;
            }
            catch (Exception exception)
            {
                this.RaiseInAppBillingProcessingError(string.Format("Error Consume Purchase: {0}", exception.ToString()));
                return false;
            }
        }

        /// <param name="itemType">Item type (product or subs)</param>
        /// <summary>
        /// Gets a list of all products of a given item type purchased by the current user.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A list of <see cref="T:Xamarin.InAppBilling.Product"/>s purchased by the current user.
        /// </returns>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public IList<Purchase> GetPurchases(string itemType)
        {
            string continuationToken = string.Empty;
            List<Purchase> list = new List<Purchase>();
            do
            {
                Bundle bundle;
                IList<string> stringArrayList1;
                IList<string> stringArrayList2;
                IList<string> stringArrayList3;
                try
                {
                    string pname = this._activity.PackageName;
                    var support = this._billingService.IsBillingSupported(3, pname, itemType);
                    bundle = !(continuationToken == string.Empty) ? this._billingService.GetPurchases(3, pname, itemType, continuationToken) : this._billingService.GetPurchases(3, pname, itemType, (string)null);
                    if (bundle == null)
                    {
                        this.RaiseInAppBillingProcessingError("No items returned from Google Play Services.");
                        return (IList<Purchase>)null;
                    }
                    else
                    {
                        int @int = bundle.GetInt("RESPONSE_CODE");
                        if (@int == 0)
                        {
                            if (!InAppBillingHandler.ValidOwnedItems(bundle))
                            {
                                Logger.Debug("Invalid purchases", new object[0]);
                                this.RaiseOnInvalidOwnedItemsBundleReturned(bundle);
                                return (IList<Purchase>)list;
                            }
                            else
                            {
                                stringArrayList1 = bundle.GetStringArrayList("INAPP_PURCHASE_ITEM_LIST");
                                stringArrayList2 = bundle.GetStringArrayList("INAPP_PURCHASE_DATA_LIST");
                                stringArrayList3 = bundle.GetStringArrayList("INAPP_DATA_SIGNATURE_LIST");
                                if (stringArrayList1 != null && stringArrayList2 != null)
                                {
                                    if (stringArrayList3 != null)
                                        goto label_11;
                                }
                                this.RaiseInAppBillingProcessingError(string.Format("Invalid owned items bundle returned by Google Play Services: {0}", (object)bundle.ToString()));
                                return (IList<Purchase>)null;
                            }
                        }
                        else
                        {
                            this.RaiseOnGetProductsError(@int, bundle);
                            return (IList<Purchase>)null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.RaiseInAppBillingProcessingError(string.Format("Error retrieving previous purchases: {0}", (object)((object)ex).ToString()));
                    return (IList<Purchase>)null;
                }
            label_11:
                for (int index1 = 0; index1 < stringArrayList1.Count; ++index1)
                {
                    string str1 = stringArrayList2[index1];
                    string str2 = stringArrayList3[index1];
                    Purchase purchase;
                    try
                    {
                        purchase = DeserializeObject<Purchase>(str1);
                    }
                    catch (Exception ex)
                    {
                        string format = "GetPurchases Error {0}: Unable to deserialize purchase '{1}'.\n Setting Purchase.DeveloperPayload with info returned from Google.";
                        object[] objArray = new object[2];
                        int index2 = 0;
                        string str3 = ((object)ex).ToString();
                        objArray[index2] = (object)str3;
                        int index3 = 1;
                        string str4 = str1;
                        objArray[index3] = (object)str4;
                        Logger.Error(format, objArray);
                        purchase = new Purchase();
                        purchase.DeveloperPayload = str1;
                    }
                    try
                    {
                        if (purchase.ProductId.Contains("android.test."))
                            list.Add(purchase);
                        else if (Security.VerifyPurchase(this.PublicKey, str1, str2))
                            list.Add(purchase);
                        else
                            this.RaiseOnPurchaseFailedValidation(purchase, str1, str2);
                    }
                    catch (Exception ex)
                    {
                        this.RaiseInAppBillingProcessingError(string.Format("Error validating previous purchase {0}: {1}", (object)purchase.ProductId, (object)((object)ex).ToString()));
                        return (IList<Purchase>)null;
                    }
                }
                try
                {
                    continuationToken = bundle.GetString("INAPP_CONTINUATION_TOKEN");
                    string format = "Continuation Token: {0}";
                    object[] objArray = new object[1];
                    int index = 0;
                    string str = continuationToken;
                    objArray[index] = (object)str;
                    Logger.Debug(format, objArray);
                }
                catch
                {
                    continuationToken = string.Empty;
                }
            }
            while (!string.IsNullOrWhiteSpace(continuationToken));
            return (IList<Purchase>)list;
        }

        private static bool ValidOwnedItems(Bundle purchased)
        {
            if (purchased.ContainsKey("INAPP_PURCHASE_ITEM_LIST") && purchased.ContainsKey("INAPP_PURCHASE_DATA_LIST"))
                return purchased.ContainsKey("INAPP_DATA_SIGNATURE_LIST");
            else
                return false;
        }

        /// <param name="requestCode">Request code.</param><param name="resultCode">Result code.</param><param name="data">Data.</param>
        /// <summary>
        /// Handles the activity result.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public void HandleActivityResult(int requestCode, Result resultCode, Intent data)
        {
            string str1 = string.Empty;
            string str2 = string.Empty;
            if (requestCode != 1001)
                return;
            if (data == null)
                return;
            int reponseCodeFromIntent;
            string stringExtra1;
            string stringExtra2;
            try
            {
                reponseCodeFromIntent = Xamarin.InAppBilling.Utilities.Extensions.GetReponseCodeFromIntent(data);
                switch (reponseCodeFromIntent)
                {
                    case 0:
                        stringExtra1 = data.GetStringExtra("INAPP_PURCHASE_DATA");
                        stringExtra2 = data.GetStringExtra("INAPP_DATA_SIGNATURE");
                        break;
                    case 1:
                        this.RaiseOnUserCanceled();
                        return;
                    default:
                        this.RaiseBuyProductError(reponseCodeFromIntent, "unknown");
                        return;
                }
            }
            catch (Exception ex)
            {
                this.RaiseInAppBillingProcessingError(string.Format("Error Decoding Returned Packet Information: {0}", (object)((object)ex).ToString()));
                return;
            }
            Purchase purchase;
            try
            {
                purchase = DeserializeObject<Purchase>(stringExtra1);
            }
            catch (Exception ex)
            {
                string format = "Completed Purchase Error {0}: Unable to deserialize purchase '{1}'.\nSetting Purchase.DeveloperPayload with info returned from Google.";
                object[] objArray = new object[2];
                int index1 = 0;
                string str3 = ((object)ex).ToString();
                objArray[index1] = (object)str3;
                int index2 = 1;
                string str4 = stringExtra1;
                objArray[index2] = (object)str4;
                Logger.Error(format, objArray);
                this.RaiseInAppBillingProcessingError(string.Format("Unable to deserialize purchase: {0}\nError: {1}", (object)stringExtra1, (object)((object)ex).ToString()));
                this.RaiseOnPurchaseFailedValidation(new Purchase()
                {
                    DeveloperPayload = stringExtra1
                }, stringExtra1, stringExtra2);
                return;
            }
            try
            {
                if (purchase.ProductId.Contains("android.test."))
                    this.RaiseOnProductPurchased(reponseCodeFromIntent, purchase, stringExtra1, stringExtra2);
                else if (Security.VerifyPurchase(this.PublicKey, stringExtra1, stringExtra2))
                    this.RaiseOnProductPurchased(reponseCodeFromIntent, purchase, stringExtra1, stringExtra2);
                else
                    this.RaiseOnPurchaseFailedValidation(purchase, stringExtra1, stringExtra2);
            }
            catch (Exception ex)
            {
                this.RaiseInAppBillingProcessingError(string.Format("Error Decoding Returned Packet Information: {0}", (object)((object)ex).ToString()));
            }
        }

        internal void RaiseOnGetProductsError(int responseCode, Bundle ownedItems)
        {
            if (this.OnGetProductsError == null)
                return;
            this.OnGetProductsError(responseCode, ownedItems);
        }

        internal void RaiseQueryInventoryError(int responseCode, Bundle skuDetails)
        {
            if (this.QueryInventoryError == null)
                return;
            this.QueryInventoryError(responseCode, skuDetails);
        }

        internal void RaiseBuyProductError(int responseCode, string sku)
        {
            if (this.BuyProductError == null)
                return;
            this.BuyProductError(responseCode, sku);
        }

        internal void RaiseInAppBillingProcessingError(string message)
        {
            if (this.InAppBillingProcesingError == null)
                return;
            this.InAppBillingProcesingError(message);
        }

        internal void RaiseOnInvalidOwnedItemsBundleReturned(Bundle ownedItems)
        {
            if (this.OnInvalidOwnedItemsBundleReturned == null)
                return;
            this.OnInvalidOwnedItemsBundleReturned(ownedItems);
        }

        internal void RaiseOnProductPurchasedError(int responseCode, string sku)
        {
            if (this.OnProductPurchasedError == null)
                return;
            this.OnProductPurchasedError(responseCode, sku);
        }

        internal void RaiseOnPurchaseFailedValidation(Purchase purchase, string purchaseData, string purchaseSignature)
        {
            if (this.OnPurchaseFailedValidation == null)
                return;
            this.OnPurchaseFailedValidation(purchase, purchaseData, purchaseSignature);
        }

        internal void RaiseOnProductPurchased(int response, Purchase purchase, string purchaseData, string purchaseSignature)
        {
            if (this.OnProductPurchased == null)
                return;
            this.OnProductPurchased(response, purchase, purchaseData, purchaseSignature);
        }

        internal void RaiseOnPurchaseConsumedError(int responseCode, string token)
        {
            if (this.OnPurchaseConsumedError == null)
                return;
            this.OnPurchaseConsumedError(responseCode, token);
        }

        internal void RaiseOnPurchaseConsumed(string token)
        {
            if (this.OnPurchaseConsumed == null)
                return;
            this.OnPurchaseConsumed(token);
        }

        internal void RaiseOnUserCanceled()
        {
            if (this.OnUserCanceled == null)
                return;
            this.OnUserCanceled();
        }

        public delegate void OnGetProductsErrorDelegate(int responseCode, Bundle ownedItems);

        public delegate void QueryInventoryErrorDelegate(int responseCode, Bundle skuDetails);

        public delegate void BuyProductErrorDelegate(int responseCode, string sku);

        public delegate void InAppBillingProcessingErrorDelegate(string message);

        public delegate void OnInvalidOwnedItemsBundleReturnedDelegate(Bundle ownedItems);

        public delegate void OnProductPurchaseErrorDelegate(int responseCode, string sku);

        public delegate void OnPurchaseFailedValidationDelegate(Purchase purchase, string purchaseData, string purchaseSignature);

        public delegate void OnProductPurchasedDelegate(int response, Purchase purchase, string purchaseData, string purchaseSignature);

        public delegate void OnPurchaseConsumedErrorDelegate(int responseCode, string token);

        public delegate void OnPurchaseConsumedDelegate(string token);

        public delegate void OnUserCanceledDelegate();
    }
}

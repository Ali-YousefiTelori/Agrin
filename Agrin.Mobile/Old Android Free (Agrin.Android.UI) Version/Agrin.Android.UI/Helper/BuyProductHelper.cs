using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.InAppBilling;
using System.Threading.Tasks;
using Agrin.MonoAndroid.UI.Activities;
using Agrin.Log;

namespace Agrin.MonoAndroid.UI.Helper
{
    public class BuyProductHelper
    {
        public Activity CurrentActivity { get; set; }

        public Action<int, Result> ChangedItems { get; set; }
        public Action<Purchase> BuyItemAction { get; set; }
        public Action ConnectComeplete { get; set; }
        #region Private Variables
        private IList<Product> _products;
        public IList<Product> Products
        {
            get { return _products; }
            set { _products = value; }
        }

        public InAppBillingServiceConnection _serviceConnection;
        #endregion

        /// <summary>
        /// Starts the current <c>Activity</c>
        /// </summary>
        /// <param name="bundle">Bundle.</param>
        public BuyProductHelper(Activity currentActivity)
        {
            CurrentActivity = currentActivity;
        }

        public bool IsError { get; set; }
        bool started = false;
        public bool StartConnetion()
        {
            try
            {
                IsError = false;
                if (started)
                    return true;
                Products = new List<Product>();
                // Attempt to attach to the Google Play Service
                StartSetup();

                // Initialize the list of available items
                started = true;
                return true;
            }
            catch (Java.Lang.Exception eee)
            {
                AutoLogger.LogText("BuyProductHelper StartConnetion " + eee.Message);
            }
            catch (Exception eee)
            {
                AutoLogger.LogError(eee, "BuyProductHelper StartConnetion ");
            }
            return false;
        }

        public void BuyProduct(Product product)
        {
            _serviceConnection.BillingHandler.BuyProduct(product);
        }

        /// <summary>
        /// Perform any final cleanup before an activity is destroyed.
        /// </summary>CurrentActivity
        public void Disconnect()
        {
            // Are we attached to the Google Play Service?
            if (_serviceConnection != null)
            {
                CurrentActivity = null;
                ChangedItems = null;
                // Yes, disconnect
                _serviceConnection.Disconnect();
            }
        }


        #region Private Methods
        /// <summary>
        /// Loads the purchased items.
        /// </summary>
        public IList<Purchase> GetPurchases()
        {
            // Ask the open connection's billing handler to get any purchases
            return _serviceConnection.BillingHandler.GetPurchases(ItemType.Product);
        }

        ///// <summary>
        ///// Updates the purchased items.
        ///// </summary>
        //private void UpdatePurchasedItems()
        //{
        //    // Ask the open connection's billing handler to get any purchases
        //    var purchases = _serviceConnection.BillingHandler.GetPurchases(ItemType.Product);

        //    // Is there a data adapter for purchases?
        //    if (_purchasesAdapter != null)
        //    {
        //        _purchasesAdapter.Items.Clear();
        //        // Yes, add new items to adapter
        //        foreach (var item in purchases)
        //        {
        //            _purchasesAdapter.Items.Add(item);
        //        }

        //        // Ask the adapter to display the new items
        //        _purchasesAdapter.NotifyDataSetChanged();
        //    }
        //}

        /// <summary>
        /// Connects to the Google Play Service and gets a list of products that are available
        /// for purchase.
        /// </summary>
        /// <returns>The inventory.</returns>
        private async Task GetInventory(Action<bool> complete)
        {
            // Ask the open connection's billing handler to return a list of avilable products for the 
            // given list of items.
            // NOTE: We are asking for the Reserved Test Product IDs that allow you to test In-App
            // Billing without actually making a purchase.,"buystorage20gb"
            Products = await _serviceConnection.BillingHandler.QueryInventoryAsync(new List<string> {
                "buystorage100mb","buystorage500mb",
                "buystorage1gb","buystorage2gb","buystorage5gb","buystorage10gb"
            }, ItemType.Product);

            // Were any products returned?
            if (Products == null)
            {
                // No, abort
                complete(false);
                return;
            }
            //if (Products.Count > 0)
            //{
            //    var removeItem = Products.Where(x => x.ProductId == "buystorage20gb");
            //    if (removeItem.FirstOrDefault() != null)
            //    {
            //        Products.Remove(removeItem.FirstOrDefault());
            //        Products.Add(removeItem.FirstOrDefault());
            //    }
            //}
            complete(true);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the setup of this Android application by connection to the Google Play Service
        /// to handle In-App purchases.
        /// </summary>
        public void StartSetup()
        {
            // A Licensing and In-App Billing public key is required before an app can communicate with
            // Google Play, however you DON'T want to store the key in plain text with the application.
            // The Unify command provides a simply way to obfuscate the key by breaking it into two or
            // or more parts, specifying the order to reassemlbe those parts and optionally providing
            // a set of key/value pairs to replace in the final string. 
            string hash = "MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwDGzGKBdeNbI3vZumpcBb0ivCwUrGQYj5UEd3nOZ06aViRtZg97NIgC//wyzUbOKeRRWI4lIItNncMlZV4D9tb0XbLqe19YC/Oo65HH+45FoEcMXnut5XscJsbfaJvlMQkkD/vav2XQOjTp0T5+r92HSk1dL0HTkcQmrfabASC5eyqabxUIrCcA/KCEiqEa9WLqY05udwLWSoGew6ndg/dVmwUWOEY+J12OtKc4+uMCAwEAAQ==";
            //var base64EncodedBytes = System.Convert.FromBase64String(hash);
            //var sssss = System.Text.Encoding.ASCII.GetString(base64EncodedBytes);
            //string value = Security.Unify(
            //    new string[] { hash },
            //    new int[] {});

            // Create a new connection to the Google Play Service
            _serviceConnection = new InAppBillingServiceConnection(CurrentActivity, hash);
            _serviceConnection.OnConnected += () =>
            {
                // Attach to the various error handlers to report issues
                _serviceConnection.BillingHandler.OnGetProductsError += (int responseCode, Bundle ownedItems) =>
                {
                    IsError = true;
                    AutoLogger.LogText("BuyProductHelper: Error getting products " + responseCode);
                    //Console.WriteLine("Error getting products");
                };

                _serviceConnection.BillingHandler.OnInvalidOwnedItemsBundleReturned += (Bundle ownedItems) =>
                {
                    AutoLogger.LogText("BuyProductHelper: Invalid owned items bundle returned ");
                    //Console.WriteLine("Invalid owned items bundle returned");
                };

                _serviceConnection.BillingHandler.OnProductPurchasedError += (int responseCode, string sku) =>
                {
                    IsError = true;
                    AutoLogger.LogText("BuyProductHelper: Error purchasing item " + responseCode + " " + sku);
                    //Console.WriteLine("Error purchasing item {0}", sku);
                };

                _serviceConnection.BillingHandler.OnPurchaseConsumedError += (int responseCode, string token) =>
                {
                    IsError = true;
                    AutoLogger.LogText("BuyProductHelper: Error consuming previous purchase " + responseCode + " " + token);
                    //Console.WriteLine("Error consuming previous purchase");
                };

                _serviceConnection.BillingHandler.InAppBillingProcesingError += (message) =>
                {
                    IsError = true;
                    AutoLogger.LogText("BuyProductHelper: In app billing processing error message:" + message);
                    //Console.WriteLine("In app billing processing error {0}", message);
                };

                _serviceConnection.BillingHandler.OnProductPurchased += (int response, Purchase purchase, string purchaseData, string purchaseSignature) =>
                {
                    if (BuyItemAction != null)
                        BuyItemAction(purchase);
                };

                _serviceConnection.BillingHandler.OnProductPurchasedError += (int responseCode, string sku) =>
                {
                    IsError = true;
                    AutoLogger.LogText("BuyProductHelper: OnProductPurchasedError:" + responseCode + " " + sku);
                };
                // Load inventory or available products
                var task = GetInventory((isOK) =>
                {
                    if (ConnectComeplete != null)
                        ConnectComeplete();
                });

            };
            _serviceConnection.OnDisconnected += _serviceConnection_OnDisconnected;
            _serviceConnection.OnInAppBillingError += _serviceConnection_OnInAppBillingError;

            //Intent intent = new Intent("ir.cafebazaar.pardakht.InAppBillingService.BIND");
            //intent.SetPackage("com.farsitel.bazaar");
            //IList<ResolveInfo> list = this.PackageManager.QueryIntentServices(intent, 0);
            //if (list == null)
            //{
            //    //this.RaiseOnInAppBillingError(InAppBillingErrorType.BillingNotSupported, "Unable to bind with com.android.vending.billing.InAppBillingService API.");
            //    //this.Connected = false;
            //}
            //else if (list.Count != 0)
            //{
            //   var bind= this.BindService(intent, _serviceConnection, Bind.AutoCreate);
            //}
            //else
            //{
            //    //this.RaiseOnInAppBillingError(InAppBillingErrorType.BillingNotSupported, "Unable to access the com.android.vending service.");
            //    //this.Connected = false;
            //}

            // Attempt to connect to the service
            _serviceConnection.Connect();

        }

        void _serviceConnection_OnInAppBillingError(InAppBillingErrorType error, string message)
        {

        }

        void _serviceConnection_OnDisconnected()
        {

        }

        #endregion

        #region User Interaction Routines

        ///// <summary>
        ///// Handle the user consuming a previously purchased item
        ///// </summary>
        ///// <param name="parent">Parent.</param>
        ///// <param name="view">View.</param>
        ///// <param name="position">Position.</param>
        ///// <param name="id">Identifier.</param>
        //public void OnItemClick(AdapterView parent, Android.Views.View view, int position, long id)
        //{
        //    // Access item being clicked on
        //    string productid = ((TextView)view).Text;
        //    var purchases = _purchasesAdapter.Items;
        //    var purchasedItem = purchases.FirstOrDefault(p => p.ProductId == productid);

        //    // Was anyting selected?
        //    if (purchasedItem != null)
        //    {
        //        // Yes, attempt to consume the given product
        //        bool result = ;

        //        // Was the product consumed?
        //        if (result)
        //        {
        //            // Yes, update interface
        //            _purchasesAdapter.Items.Remove(purchasedItem);
        //            _purchasesAdapter.NotifyDataSetChanged();
        //        }
        //    }
        //}

        public bool ConsumePurchase(Purchase purchasedItem)
        {
            return _serviceConnection.BillingHandler.ConsumePurchase(purchasedItem);
        }

        #endregion
    }
}
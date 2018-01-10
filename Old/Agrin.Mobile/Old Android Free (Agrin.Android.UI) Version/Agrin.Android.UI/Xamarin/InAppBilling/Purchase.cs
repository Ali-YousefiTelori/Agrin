// Type: Xamarin.InAppBilling.Purchase
// Assembly: Xamarin.InAppBilling, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 312DE16D-65DB-4E32-94A1-D49884001984
// Assembly location: D:\Projects\xamarin.inappbilling-2.2\xamarin.inappbilling-2.2\lib\android\Xamarin.InAppBilling.dll

using System;

namespace Xamarin.InAppBilling
{
    /// <summary>
    /// Holds all information about a product purchased from Google Play for the current user.
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public class Purchase
    {
        /// <summary>
        /// Gets or sets the name of the package.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The name of the package.
        /// </value>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public string PackageName { get; set; }

        /// <summary>
        /// Gets or sets the order identifier.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The order identifier.
        /// </value>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public string OrderId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The product identifier.
        /// </value>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the developer payload.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The developer payload.
        /// </value>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public string DeveloperPayload { get; set; }

        /// <summary>
        /// Gets or sets the purchase time.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The purchase time.
        /// </value>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public long PurchaseTime { get; set; }

        /// <summary>
        /// Gets or sets the state of the purchase.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The state of the purchase.
        /// </value>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public int PurchaseState { get; set; }

        /// <summary>
        /// Gets or sets the purchase token.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The purchase token.
        /// </value>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public string PurchaseToken { get; set; }

        /// <summary>
        /// To be added.
        /// </summary>
        /// 
        /// <returns>
        /// To be added.
        /// </returns>
        /// 
        /// <remarks>
        /// To be added.
        /// </remarks>
        public override string ToString()
        {
            object[] args = new object[] { this.PackageName, this.OrderId, this.ProductId, this.DeveloperPayload, this.PurchaseTime, this.PurchaseState, this.PurchaseToken };
            return string.Format("[Purchase: PackageName={0}, OrderId={1}, ProductId={2}, DeveloperPayload={3}, PurchaseTime={4}, PurchaseState={5}, PurchaseToken={6}]", args);

        }
    }
}

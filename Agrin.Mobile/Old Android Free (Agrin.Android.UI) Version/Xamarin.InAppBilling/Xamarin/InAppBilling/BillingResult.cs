namespace Xamarin.InAppBilling
{
    using System;

    public static class BillingResult
    {
        public const int BillingUnavailable = 3;
        public const int DeveloperError = 5;
        public const int Error = 6;
        public const int ItemAlreadyOwned = 7;
        public const int ItemNotOwned = 8;
        public const int ItemUnavailable = 4;
        public const int OK = 0;
        public const int ServiceUnavailable = 2;
        public const int UserCancelled = 1;
    }
}


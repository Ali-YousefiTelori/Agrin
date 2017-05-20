using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Xamarin.InAppBilling
{

    public class Product
    {
        public override string ToString()
        {
            object[] args = new object[] { this.Title, this.Price, this.Price_Amount_Micros, this.Price_Currency_Code, this.Type, this.Description, this.ProductId };
            return string.Format("[Product: Title={0}, Price={1}, Price_Amount_Micros={2}, Price_Currency_Code={3}, Type={4}, Description={5}, ProductId={6}]", args);
        }

        public string Description { get; set; }

        public string Price { get; set; }

        public string Price_Amount_Micros { get; set; }

        public string Price_Currency_Code { get; set; }

        public string ProductId { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }
    }
}


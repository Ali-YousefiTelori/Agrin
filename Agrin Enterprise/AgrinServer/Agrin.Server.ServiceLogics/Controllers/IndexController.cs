using SignalGo.Server.DataTypes;
using SignalGo.Server.Models;
using SignalGo.Shared.DataTypes;
using SignalGo.Shared.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.Controllers
{
    [ServiceContract("", ServiceType.HttpService, InstanceType.SingleInstance)]
    public class IndexController
    {
        [HomePage]
        public string OpenPage()
        {
            OperationContext.Current.HttpClient.Status = System.Net.HttpStatusCode.MovedPermanently;
            OperationContext.Current.HttpClient.ResponseHeaders.Add("Location", new string[] { "http://framework.blogfa.com" });
            OperationContext.Current.HttpClient.ResponseHeaders.Add("Content-Type", new string[] { "text/html" });

            return @"<html>
<head>
<title>Moved</title>
</head>
<body>
<h1>Moved</h1>
<p>This page has moved to <a href=""http://framework.blogfa.com"">http://framework.blogfa.com</a>.</p>
</body>
</html>";
        }
        //[HomePage]
        //public FileActionResult OpenPage(string address, string name, List<string> values)
        //{
        //    try
        //    {
        //        string baseAddress = @"C:\Users\ASUS\source\repos\WeCanDoPanel\WeCanDoPanel\bin\Debug\Output\";
        //        address = address.Replace("/", "\\");
        //        if (string.IsNullOrEmpty(address))
        //        {
        //            OperationContext.Current.HttpClient.ResponseHeaders.Add("Content-Type", MimeTypes.MimeTypeMap.GetMimeType(".html"));
        //            return new FileActionResult(Path.Combine(baseAddress, "index.html"));
        //        }
        //        else
        //        {
        //            OperationContext.Current.HttpClient.ResponseHeaders.Add("Content-Type", MimeTypes.MimeTypeMap.GetMimeType(Path.GetExtension(name)));
        //            return new FileActionResult(Path.Combine(baseAddress, address, name));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
    }
}

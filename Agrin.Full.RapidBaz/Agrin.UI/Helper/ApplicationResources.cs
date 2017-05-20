using System.Windows;

namespace Agrin.UI.Helper
{
    public static class ApplicationResources
    {
        public static string GetAppResource(object key, bool nullable = false)
        {
            var obj = Application.Current.Resources[key];
            if (obj != null)
                return obj.ToString();
            if (nullable)
                return "";
            return "Not Found";
        }

        public static object GetAppResourceObject(object key)
        {
            return Application.Current.Resources[key];
        }

        public static Style GetAppResourceStyle(object key)
        {
            var obj = Application.Current.Resources[key];
            if (obj is Style)
                return (Style)obj;
            return null;
        }
    }
}

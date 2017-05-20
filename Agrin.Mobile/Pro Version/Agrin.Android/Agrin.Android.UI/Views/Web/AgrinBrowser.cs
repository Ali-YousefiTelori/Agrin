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
using Android.Webkit;
using System.Threading.Tasks;
using System.Threading;
using Agrin.Helpers;
using Android.Util;
using Java.Lang;

namespace Agrin.Views.Web
{
    public class CustomWebViewClient : WebViewClient
    {
        public Action<string> UrlLoadingAction { get; set; }

        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            UrlLoadingAction?.Invoke(url);
            return base.ShouldOverrideUrlLoading(view, url);
        }

        public override void OnLoadResource(WebView view, string url)
        {
            base.OnLoadResource(view, url);
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            return base.ShouldInterceptRequest(view, request);
        }
    }

    public class AgrinBrowser : IDisposable
    {
        Activity CurrentActivity { get; set; }

        LinearLayout _mainLayout;
        LinearLayout _actionToolBoxLayout;
        LinearLayout _actionTopToolBoxLayout;
        WebView currentWebView = null;
        EditText txt_URL = null;
        int _templateResourceId;
        View _CustomView;
        Button btnGoogle = null, btnGo = null, btnBack = null;
        public AgrinBrowser(Activity context, int templateResourceId, LinearLayout mainLayout, LinearLayout actionToolBoxLayout, LinearLayout actionTopToolBoxLayout)
        {
            CurrentActivity = context;
            _mainLayout = mainLayout;
            _templateResourceId = templateResourceId;
            _CustomView = CurrentActivity.LayoutInflater.Inflate(Resource.Layout.AgrinBrowserLayout, mainLayout, false);
            currentWebView = _CustomView.FindViewById<WebView>(Resource.AgrinBrowserLayout.mainWebView);
            InitWebView(currentWebView);
            btnGo = _CustomView.FindViewById<Button>(Resource.AgrinBrowserLayout.btnGo);
            btnGo.Click += BtnGo_Click;
            btnGoogle = _CustomView.FindViewById<Button>(Resource.AgrinBrowserLayout.btnGoogle);
            btnGoogle.Click += BtnGoogle_Click;
            btnBack = _CustomView.FindViewById<Button>(Resource.AgrinBrowserLayout.btnBack);
            btnBack.Click += BtnBack_Click;
            txt_URL = _CustomView.FindViewById<EditText>(Resource.AgrinBrowserLayout.txt_URL);
            txt_URL.Hint = "آدرس را وارد کنید.";
            _actionToolBoxLayout = actionToolBoxLayout;
            _actionTopToolBoxLayout = actionTopToolBoxLayout;
            ViewsUtility.SetTextLanguage(CurrentActivity, _CustomView, new List<int>() { Resource.AgrinBrowserLayout.btnGo, Resource.AgrinBrowserLayout.btnGoogle , Resource.AgrinBrowserLayout.btnBack });
            
            currentWebView.Download += CurrentWebView_Download;
            var wc = new CustomWebViewClient();
            wc.UrlLoadingAction = (url) =>
            {
                txt_URL.Text = url;
            };
            currentWebView.SetWebViewClient(wc);
            InitializeView();
        }

        public void InitWebView(WebView webView)
        {
            webView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);

            try
            {
                if ((int)Build.VERSION.SdkInt >= 19)
                    webView.SetLayerType(LayerType.Hardware, null);
                else
                    webView.SetLayerType(LayerType.Software, null);
            }
            catch (System.Exception ex)
            {

            }
            
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            var ws = webView.Settings;
            webView.Settings.AllowFileAccess = true;
            if ((int)Build.VERSION.SdkInt >= 5)
            {
                try
                {
                    var m1 = webView.Settings.Class.GetMethod("setDomStorageEnabled", new Class[] { Java.Lang.Boolean.Type });
                    m1.Invoke(ws, Java.Lang.Boolean.True);

                    var m2 = ws.Class.GetMethod("setDatabaseEnabled", new Class[] { Java.Lang.Boolean.Type });
                    m2.Invoke(ws, Java.Lang.Boolean.True);

                    var m3 = webView.Settings.Class.GetMethod("setDatabasePath", new Class[] { new Java.Lang.String().Class });
                    m3.Invoke(ws, "/data/data/" + CurrentActivity.PackageName + "/databases/");

                    var m4 = webView.Settings.Class.GetMethod("setAppCacheMaxSize", new Class[] { Long.Type });
                    m4.Invoke(ws, 1024 * 1024 * 8);

                    var m5 = webView.Settings.Class.GetMethod("setAppCachePath", new Class[] { new Java.Lang.String().Class });
                    m5.Invoke(ws, "/data/data/" + CurrentActivity.PackageName + "/cache/");

                    var m6 = webView.Settings.Class.GetMethod("setAppCacheEnabled", new Class[] { Java.Lang.Boolean.Type });
                    m6.Invoke(ws, Java.Lang.Boolean.True);

                    //Log.d(TAG, "Enabled HTML5-Features");
                }
                catch (NoSuchMethodException e)
                {
                    //Log.e(TAG, "Reflection fail", e);
                }
                catch (Java.Lang.Reflect.InvocationTargetException e)
                {
                    //Log.e(TAG, "Reflection fail", e);
                }
                catch (IllegalAccessException e)
                {
                    // Log.e(TAG, "Reflection fail", e);
                }
                catch (System.Exception ex)
                {

                }
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            if (currentWebView.CanGoBack())
                currentWebView.GoBack();
        }

        private void BtnGoogle_Click(object sender, EventArgs e)
        {
            btnGoogle.Enabled = false;
            currentWebView.LoadUrl("https://google.com");
            Task.Factory.StartNew(() =>
            {
                CurrentActivity.RunOnUI(() =>
                {
                    System.Threading.Thread.Sleep(1500);
                    btnGoogle.Enabled = true;
                });
            });
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            string url = txt_URL.Text;
            if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                url = "http://" + url;
            currentWebView.LoadUrl(url);
            Task.Factory.StartNew(() =>
            {
                CurrentActivity.RunOnUI(() =>
                {
                    System.Threading.Thread.Sleep(1500);
                    btnGo.Enabled = true;
                });
            });
        }

        private void CurrentWebView_Download(object sender, DownloadEventArgs e)
        {
            Intent intent = new Intent(CurrentActivity, typeof(BrowseActivity));
            intent.PutExtra(Intent.ExtraText, e.Url);
            intent.SetFlags(ActivityFlags.NewTask);
            InitializeApplication.CurrentContext.StartActivity(intent);
        }

        public void InitializeView()
        {
            if (_actionToolBoxLayout.ChildCount > 0)
                _actionToolBoxLayout.RemoveAllViews();
            _actionToolBoxLayout.Visibility = ViewStates.Gone;

            if (_actionTopToolBoxLayout.ChildCount > 0)
                _actionTopToolBoxLayout.RemoveAllViews();
            _actionTopToolBoxLayout.Visibility = ViewStates.Gone;

            if (_mainLayout.ChildCount > 0)
                _mainLayout.RemoveAllViews();
            _mainLayout.AddView(_CustomView);
        }

        public void Dispose()
        {

        }
    }
}
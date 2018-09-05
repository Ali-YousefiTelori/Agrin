using Agrin.IO.Helper;
using Agrin.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Agrin.Windows.UI.Views.WindowLayouts.Asuda
{
    /// <summary>
    /// Interaction logic for BasketReceiverPinnedWindow.xaml
    /// </summary>
    public partial class BasketReceiverPinnedWindow : Window
    {
        public BasketReceiverPinnedWindow()
        {
            InitializeComponent();
        }

        private void btnUnPin_Click(object sender, RoutedEventArgs e)
        {
            BasketReceiverWindow.IsPinned = false;
        }

        bool _IsShow = false;

        public bool IsShow
        {
            get
            {
                return _IsShow;
            }
            set
            {
                _IsShow = value;
                if (value)
                    BasketReceiverWindow.ShowDataWindowPinned();
                else
                    BasketReceiverWindow.HideDataWindowPinned();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsShow = !IsShow;
        }


        private void Window_DragOver(object sender, DragEventArgs e)
        {
            DragOverData(e);
            //var result = GenerateData(e.Data);
            //if (result.Items.Count == 0)
            //    e.Effects = DragDropEffects.None;
            //else
            //    e.Effects = DragDropEffects.All;
            //e.Handled = true;

        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            DragEnterData(e);
        }

        public static MultipeWebResponseData GenerateData(IDataObject data)
        {
            MultipeWebResponseData result = new MultipeWebResponseData();
            try
            {
                var formats = data.GetFormats();
                Dictionary<string, object> values = new Dictionary<string, object>();
                foreach (var item in formats)
                {
                    try
                    {
                        if (item == "DragContext" || item == "FileContents")
                            continue;
                        var value = data.GetData(item);
                        if (value.GetType() == typeof(string))
                            values.Add(item, value);
                    }
                    catch
                    {

                    }
                }
                List<string> items = new List<string>();
                Action<string> addLink = (link) =>
                {
                    if (items.Contains(link))
                        return;
                    items.Add(link);
                };
                Action<IEnumerable<string>> addLinks = (links) =>
                {
                    foreach (var item in links)
                    {
                        addLink(item);
                    }
                };
                string referenceUri = null;
                foreach (var item in values)
                {
                    string value = (string)item.Value;
                    if (string.IsNullOrEmpty(value))
                        continue;
                    if (value.Contains("SourceURL="))
                    {
                        referenceUri = Agrin.IO.Strings.Text.GetStartTextToEndline(value, "SourceURL=");
                    }
                    else if (value.Contains("SourceURL:"))
                    {
                        referenceUri = Agrin.IO.Strings.Text.GetStartTextToEndline(value, "SourceURL:");
                        if (items.Contains(referenceUri))
                            referenceUri = null;
                    }
                    var links = Agrin.IO.Strings.HtmlPage.ExtractLinksFromHtmlTwo(value);
                    addLinks(links);
                }
                result.Reference = referenceUri;
                foreach (var item in items)
                {
                    if (item == referenceUri)
                        continue;
                    var res = new WebResponseData() { Uri = Agrin.IO.Strings.Decodings.FullDecodeString(item), FileName = Agrin.IO.FileStatic.GetLinksFileName(item) };
                    if (!string.IsNullOrEmpty(res.FileName))
                        res.Extension = MPath.GetFileExtention(res.FileName);
                    result.Items.Add(res);
                }
            }
            catch (Exception ex)
            {
                Log.AutoLogger.LogError(ex, "Drag GenerateData");
            }
            return result;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            DropData(e.Data);
        }

        public static void DropData(IDataObject data)
        {
            var result = GenerateData(data);
            if (result.Items.Count > 0)
            {
                ProxyMonitor.MultipeResponseCompleteAction?.Invoke(result);
            }
        }

        static DragDropEffects effect = DragDropEffects.None;
        public static void DragOverData(DragEventArgs e)
        {
            e.Effects = effect;
            e.Handled = true;
        }

        public static void DragEnterData(DragEventArgs e)
        {
            var result = GenerateData(e.Data);
            if (result.Items.Count == 0)
                e.Effects = effect = DragDropEffects.None;
            else
                e.Effects = effect = DragDropEffects.All;
            e.Handled = true;
        }
    }
}

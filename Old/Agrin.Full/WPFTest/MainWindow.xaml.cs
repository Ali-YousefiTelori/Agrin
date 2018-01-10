using System;
using System.Collections.Generic;
using System.Windows;

namespace WPFTest
{
    public class Test
    {
        [Newtonsoft.Json.JsonProperty("media type and subtype(s)")]
        public string mimeType { get; set; }
        [Newtonsoft.Json.JsonProperty("suffixes applicable")]
        public string Extension { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var text = Convert.ToBase64String(System.IO.File.ReadAllBytes(@"D:\BaseProjects\OLD\Agrin 2013\Design\Windows\Agrin Browser Page Html\index.html"));
            //try
            //{
            //    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Test>>(System.IO.File.ReadAllText("d:\\ali.json"));
            //    StringBuilder texts = new StringBuilder();

            //    foreach (var item in data)
            //    {
            //        if (string.IsNullOrEmpty(item.mimeType) || string.IsNullOrEmpty(item.Extension))
            //        {
            //            continue;
            //        }
            //        item.mimeType = item.mimeType.ToLower();
            //        item.Extension = item.Extension.ToLower();
            //        if (items.ContainsKey(item.mimeType))
            //        {
            //            if (items[item.mimeType].Contains(item.Extension))
            //            {

            //            }
            //            items[item.mimeType].Add(item.Extension);
            //        }
            //        else
            //        {
            //            items.Add(item.mimeType, new List<string>());
            //            items[item.mimeType].Add(item.Extension);
            //        }
            //    }
            //    foreach (var item in items)
            //    {
            //        texts.Append("{\"" + item.Key + "\", new string[] {");
            //        foreach (var val in item.Value)
            //        {
            //            texts.Append("\"" + val + "\",");
            //        }
            //        var old = texts.ToString();
            //        old = old.TrimEnd(',');
            //        texts.Clear();
            //        texts.Append(old);
            //        texts.Append("}");
            //        texts.Append("},");
            //        texts.AppendLine();
            //    }
            //    var text = texts.ToString();
            //}
            //catch
            //{

            //}
        }

        Dictionary<string, List<string>> items = new Dictionary<string, List<string>>() { { "ali", new List<string>() { "a", "b" } } };
    }
}

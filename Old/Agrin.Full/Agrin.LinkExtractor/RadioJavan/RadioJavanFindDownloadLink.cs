using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.LinkExtractor.RadioJavan
{
    public class RadioJavanQualityInfo
    {
        public string Link { get; set; }
        public string Quality { get; set; }
    }

    public class RadioJavanLinkInfo
    {
        public RadioJavanLinkInfo()
        {
            Qualities = new List<RadioJavanQualityInfo>();
        }

        public string Title { get; set; }
        public List<RadioJavanQualityInfo> Qualities { get; set; }
    }

    public static class RadioJavanFindDownloadLink
    {
        public static bool IsRadioJavanLink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                if (uri.Host.ToLower() == "www.radiojavan.com" || uri.Host.ToLower().Contains("radiojavan.com"))
                    return true;
            }
            else
                return false;
            return false;
        }

        public static RadioJavanLinkInfo FindLinkFromSite(string link, IWebProxy proxy,bool changedFromMobile=false)
        {
            var _request = (HttpWebRequest)WebRequest.Create(link);
            _request.Timeout = 60000;
            _request.AllowAutoRedirect = true;
            _request.Proxy = proxy;
            _request.ServicePoint.ConnectionLimit = int.MaxValue;

            _request.KeepAlive = true;
            _request.Headers.Add("Accept-Language", "en-US");
            _request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";

            using (var reader = new System.IO.StreamReader(_request.GetResponse().GetResponseStream()))
            {
                var data = reader.ReadToEnd();
                //get mp3 file
                var linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "id=\"download\"", "Download");
                if (string.IsNullOrEmpty(linkString))
                    linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "download_album", "Download");
                if (string.IsNullOrEmpty(linkString))
                {
                    //get mp3 mobile
                    var linkValue = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "currentMP3Url", ";");
                    if (!string.IsNullOrEmpty(linkValue))
                    {
                        linkValue = "https://rjmediamusic.com/media/" + Agrin.IO.Strings.Text.GetTextBetweenTwoValue(linkValue, "'", "'") + ".mp3";

                        return new RadioJavanLinkInfo() { Title = System.IO.Path.GetFileNameWithoutExtension(linkValue), Qualities = new List<RadioJavanQualityInfo>() { new RadioJavanQualityInfo() { Link = linkValue, Quality = "music" } } };
                    }
                }
                linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(linkString, "javascript", "target");
                linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(linkString, "link=\"", "\"");
                var title = System.IO.Path.GetFileNameWithoutExtension(linkString);
                if (!string.IsNullOrEmpty(linkString))
                {
                    return new RadioJavanLinkInfo() { Title = title, Qualities = new List<RadioJavanQualityInfo>() { new RadioJavanQualityInfo() { Link = linkString, Quality = "music" } } };
                }
                //get video file
                else
                {

                    linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "RJ.videoPermlink", "script");
                }
                //get mobile video file
                if (string.IsNullOrEmpty(linkString) && link.ToLower().Contains("/mobile") && link.ToLower().Contains("/video"))
                {
                    linkString = "https://www.radiojavan.com/videos/video/" + System.IO.Path.GetFileName(link);
                    if (!changedFromMobile)
                        return FindLinkFromSite(linkString, proxy, true);
                }
                else
                    title = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(linkString, "'", "'");

                return new RadioJavanLinkInfo() { Title = title, Qualities = GetQualities(linkString) };// Link = Agrin.IO.Strings.HtmlPage.ExtractFirstLinkFromHtml(linkString, 1)
            }
        }

        static List<RadioJavanQualityInfo> GetQualities(string text)
        {
            List<RadioJavanQualityInfo> items = new List<RadioJavanQualityInfo>();
            var video480p = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(text, "video480p", ";");
            video480p = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(video480p, "'", "'");
            if (!string.IsNullOrEmpty(video480p))
            {
                items.Add(new RadioJavanQualityInfo() { Link = "https://rjmediamusic.com/media/music_video" + video480p, Quality = "480p" });
            }

            var video720p = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(text, "video720p", ";");
            video720p = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(video720p, "'", "'");
            if (!string.IsNullOrEmpty(video720p))
            {
                items.Add(new RadioJavanQualityInfo() { Link = "https://rjmediamusic.com/media/music_video" + video720p, Quality = "720p" });
            }

            var video4k = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(text, "video4k", ";");
            video4k = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(video4k, "'", "'");
            if (!string.IsNullOrEmpty(video4k))
            {
                items.Add(new RadioJavanQualityInfo() { Link = "https://rjmediamusic.com/media/music_video" + video4k, Quality = "4k" });
            }

            var video1080p = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(text, "video1080p", ";");
            video1080p = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(video1080p, "'", "'");
            if (!string.IsNullOrEmpty(video1080p))
            {
                items.Add(new RadioJavanQualityInfo() { Link = "https://rjmediamusic.com/media/music_video" + video1080p, Quality = "1080p" });
            }

            return items;
        }

        public static RadioJavanQualityInfo GetQualityByText(IEnumerable<RadioJavanQualityInfo> items, string qualityText)
        {
            foreach (var item in items)
            {
                if (item.Quality == qualityText)
                    return item;
            }
            return null;
        }
    }
}

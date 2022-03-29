using Agrin.IO.Helper;
using Agrin.Log;
using Agrin.Network.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Network
{
    public static class ProxyMonitor
    {
        static string GetDownloadPage()
        {
            return System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String("PGh0bWw+DQogICAgPGhlYWQ+DQogICAgICAgIDxtZXRhIGh0dHAtZXF1aXY9IkNvbnRlbnQtVHlwZSIgY29udGVudD0idGV4dC9odG1sOyBjaGFyc2V0PVVURi04IiAvPg0KICAgICAgICA8dGl0bGU+cHNkdG93ZWJBZ3JpbkJyb3dzZXJQYWdlLnBzZDwvdGl0bGU+DQoJCTxzY3JpcHQ+DQogIGZ1bmN0aW9uIEV4aXQoKSB7DQoJCXdpbmRvdy5vcGVuKCdsb2NhdGlvbicsICdfc2VsZicsICcnKTsNCgkJd2luZG93LmNsb3NlKCk7DQogICB9DQo8L3NjcmlwdD4NCiAgICAgICAgPHN0eWxlPg0KCQkgYm9keSB7DQogICAgbWFyZ2luOiAwOw0KICAgIHBhZGRpbmc6IDA7DQogfQ0KIA0KICNMYXllcjAgDQp7IA0KICAgICBsZWZ0OiAwcHg7IA0KICAgICB0b3A6IDBweDsgDQogICAgIHBvc2l0aW9uOiBhYnNvbHV0ZTsgDQogICAgIHdpZHRoOiAxMDAlOw0KICAgICBoZWlnaHQ6IDEwMCU7DQoJIGJhY2tncm91bmQtY29sb3I6ICNlZmVmZWY7DQp9IA0KDQogI1JvdW5kZWRSZWN0YW5nbGUxIA0KeyANCiAgICAgcG9zaXRpb246IGFic29sdXRlOw0KCSBsZWZ0OiA1MCU7DQoJIHRvcDogNTAlOw0KCSBtYXJnaW4tbGVmdDogLTI2N3B4Ow0KCSBtYXJnaW4tdG9wOiAtMTY3cHg7DQogICAgIHdpZHRoOiA1MzRweDsNCiAgICAgaGVpZ2h0OiAzMzRweDsNCgkgYm9yZGVyLXN0eWxlOiBzb2xpZDsNCgkgYm9yZGVyLXdpZHRoOiAxcHg7DQoJIGJvcmRlci1jb2xvcjogIzVlNWU1ZTsNCgkgYm9yZGVyLXJhZGl1czogMTBweDsNCgkgYmFja2dyb3VuZC1jb2xvcjogIzk5YjhjYzsNCn0gDQojaWNvbmRpdiANCnsgDQoJIHBvc2l0aW9uOiBhYnNvbHV0ZTsNCgkgbGVmdDogNTAlOw0KCSB0b3A6IDUwJTsNCgkgbWFyZ2luLWxlZnQ6IDIxMHB4Ow0KCSBtYXJnaW4tdG9wOiAtMTUwcHg7DQogICAgIHdpZHRoOiA1MHB4Ow0KICAgICBoZWlnaHQ6IDUwcHg7DQp9IA0KI3RleHREaXYgDQp7IA0KCSBwb3NpdGlvbjogYWJzb2x1dGU7DQoJIGxlZnQ6IDUwJTsNCgkgdG9wOiA1MCU7DQoJIG1hcmdpbi1sZWZ0OiAtMjQwcHg7DQoJIG1hcmdpbi10b3A6IC0xNDBweDsNCiAgICAgd2lkdGg6IDQ0MHB4Ow0KICAgICBoZWlnaHQ6IDUwcHg7DQoJIGRpcmVjdGlvbjogcnRsOw0KCSB0ZXh0LWFsaWduOiBqdXN0aWZ5Ow0KCSBmb250LWZhbWlseTogJ3RhaG9tYSc7DQoJIGZvbnQtc2l6ZTogMTBwdDsNCn0gDQojUmVjdGFuZ2xlMSB7DQogIGJvcmRlci1zdHlsZTogc29saWQ7DQogIGJvcmRlci13aWR0aDogMXB4Ow0KICBib3JkZXItY29sb3I6ICMyMjIyMjI7DQogIGJvcmRlci1yYWRpdXM6IDVweDsNCiAgYmFja2dyb3VuZC1jb2xvcjogIzRhN2NiOTsNCiAgcG9zaXRpb246IGFic29sdXRlOw0KICBsZWZ0OiAxNXB4Ow0KICBib3R0b206IDE1cHg7DQogIHdpZHRoOiA5MHB4Ow0KICBoZWlnaHQ6IDM0cHg7DQp9DQojY2xvc2VidXR0b24gew0KCWZvbnQtc2l6ZTogMTJweDsNCglmb250LWZhbWlseTogIlRhaG9tYSI7DQoJY29sb3I6IHJnYigyNTUsIDI1NSwgMjU1KTsNCglwb3NpdGlvbjogYWJzb2x1dGU7DQoJdGV4dC1hbGlnbjogY2VudGVyOw0KCXdpZHRoOiAxMDAlOw0KCWhlaWdodDogMTAwJTsNCgl0b3A6IDdweDsNCn0NCg0KCQk8L3N0eWxlPg0KICAgIDwvaGVhZD4NCiAgICA8Ym9keT4NCiAgICAgICAgPGRpdiBpZD0iYmFja2dyb3VuZCI+DQoJCQkNCiAgICAgICAgICAgIDxkaXYgaWQ9IkxheWVyMCI+PC9kaXY+DQogICAgICAgICAgICA8ZGl2IGlkPSJSb3VuZGVkUmVjdGFuZ2xlMSI+DQoJCQkgIDxkaXYgaWQ9IlJlY3RhbmdsZTEiIG9uY2xpY2s9IkV4aXQoKSI+DQoJCQkgIDxkaXYgaWQ9ImNsb3NlYnV0dG9uIj4NCgkJCSAg2KjYs9iq2YYNCgkJCSAgPC9kaXY+DQoJCQkgIDwvZGl2Pg0KCQkJPC9kaXY+DQo8ZGl2IGlkPSJyZWN0MiI+DQoJCQkgIDxkaXYgaWQ9Imljb25kaXYiPg0KCQkJCTxzdmcgd2lkdGg9IjUwIiBoZWlnaHQ9IjUwIiB2aWV3Qm94PSIwIDAgNjAwIDYwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4NCgkJCQkgPGc+DQoJCQkJICA8cGF0aCBmaWxsPSIjYjk0YTRhIiBkPSIgTSAyMzAuNTQgMjQuODEgQyAyNjguNDggMjAuNzAgMzA3LjQ1IDI1Ljg1IDM0Mi44NCA0MC4yMiBDIDM3My41OCA1Mi41NiA0MDEuNjEgNzEuNTkgNDI0LjQwIDk1LjYzIEMgNDYxLjM0IDEzNC4xMyA0ODQuMzIgMTg1LjczIDQ4Ny45NSAyMzguOTkgQyA0OTEuMzYgMjg0LjQzIDQ4MS4wOSAzMzAuODMgNDU4LjUwIDM3MC40NCBDIDQ0MC4yNSA0MDIuNzcgNDE0LjA5IDQzMC42MCAzODIuOTggNDUwLjg2IEMgMzQ3LjE2IDQ3NC4zNiAzMDQuNjggNDg3LjU3IDI2MS44NSA0ODguNDggQyAyMTIuNTEgNDg5LjkyIDE2Mi44OCA0NzQuOTcgMTIyLjU2IDQ0Ni40OCBDIDk1LjA5IDQyNy4yNSA3MS44MyA0MDEuOTkgNTQuOTcgMzcyLjk5IEMgMzcuNDAgMzQyLjkwIDI2Ljc0IDMwOC44MCAyNC4xNCAyNzQuMDQgQyAyMS4wOSAyMzQuOTMgMjguMDAgMTk1LjAyIDQ0LjQzIDE1OS4zNyBDIDcwLjMwIDEwMi4xMSAxMjAuNDkgNTYuNTAgMTc5Ljk0IDM2LjE3IEMgMTk2LjMxIDMwLjUwIDIxMy4zMiAyNi43MCAyMzAuNTQgMjQuODEgTSAyNjkuNDUgMTEzLjU5IEMgMjU5LjUxIDExNS4zOSAyNTAuNTUgMTIxLjcwIDI0NS4zNiAxMzAuMzQgQyAyNDAuOTcgMTM3LjU2IDI0MC4wMSAxNDYuNjAgMjQyLjE5IDE1NC43MCBDIDI0NC40OSAxNjMuMjMgMjUwLjgwIDE3MC4yNiAyNTguMzQgMTc0LjYyIEMgMjY2LjEzIDE3OS4xNCAyNzUuNjAgMTgwLjE4IDI4NC4zNyAxNzguNTcgQyAyOTMuNjQgMTc2LjgwIDMwMS45NSAxNzEuMDIgMzA3LjE3IDE2My4yMSBDIDMxMi4yMCAxNTUuNjcgMzEzLjMyIDE0NS44OCAzMTAuODggMTM3LjI0IEMgMzA4Ljc0IDEyOS42NyAzMDMuNDAgMTIzLjM1IDI5Ny4wMSAxMTguOTMgQyAyODkuMDQgMTEzLjUwIDI3OC44NiAxMTEuODggMjY5LjQ1IDExMy41OSBNIDIyNi4wMSAyMTEuODAgQyAyMTMuNjIgMjE0LjY0IDIwMS44MSAyMTkuNDcgMTg5Ljk5IDIyNC4wNSBDIDE4OC42OCAyMjkuNDcgMTg3LjM0IDIzNC44OCAxODYuMDQgMjQwLjMxIEMgMTk0LjEwIDIzNy41MiAyMDIuMzkgMjM0LjUzIDIxMS4wNyAyMzUuMTkgQyAyMTYuNTYgMjM1LjQ5IDIyMy40NiAyMzYuMjYgMjI2LjI4IDI0MS43NCBDIDIyOS42MCAyNDguNjcgMjI4LjU4IDI1Ni43MyAyMjcuNDYgMjY0LjA4IEMgMjIzLjgxIDI4Mi4zNCAyMTcuOTUgMzAwLjA1IDIxMy4wOCAzMTguMDEgQyAyMDguNzEgMzMyLjY3IDIwNC43OSAzNDcuNzYgMjA1LjI1IDM2My4xOCBDIDIwNS42NCAzNzQuMzAgMjExLjkwIDM4NC44NiAyMjEuMjcgMzkwLjc4IEMgMjMyLjM3IDM5Ny45NyAyNDYuMjMgMzk5LjA3IDI1OS4wOSAzOTcuODUgQyAyNjUuNzQgMzk3LjU5IDI3Mi4yOSAzOTYuMDUgMjc4LjQ5IDM5My42OCBDIDI4Ny4xNCAzOTAuMzkgMjk1LjgzIDM4Ny4xOSAzMDQuNDcgMzgzLjg3IEMgMzA1Ljc1IDM3OC40NSAzMDcuMTAgMzczLjA1IDMwOC4zOSAzNjcuNjMgQyAyOTkuOTEgMzcwLjY2IDI5MS4wNiAzNzMuNjIgMjgxLjkyIDM3Mi42OSBDIDI3Ni41MSAzNzIuMjUgMjY5Ljk0IDM3MS4wOCAyNjcuMjggMzY1LjcwIEMgMjYzLjY4IDM1OC4zMSAyNjUuNzMgMzQ5LjkyIDI2Ni42NyAzNDIuMTcgQyAyNzAuNDcgMzIzLjU0IDI3Ni40NiAzMDUuNDUgMjgxLjQwIDI4Ny4xMCBDIDI4My43NSAyNzguNTEgMjg2LjYzIDI2OS45OCAyODcuNTAgMjYxLjA2IEMgMjg4LjMxIDI1Mi4yMSAyODkuNTMgMjQyLjk3IDI4Ni40NSAyMzQuNDAgQyAyODIuNzkgMjIzLjM0IDI3Mi45OSAyMTUuMDUgMjYxLjkwIDIxMS45OCBDIDI1MC4yNSAyMDguODEgMjM3LjY5IDIwOC42MyAyMjYuMDEgMjExLjgwIFoiIC8+DQoJCQkJICA8L2c+DQoJCQkJPC9zdmc+DQoJCQkgIDwvZGl2Pg0KCQkJICAgPGRpdiBpZD0idGV4dERpdiI+DQoJCQkJ2YTbjNmG2qkg2LTZhdinINio2Ycg2K/Yp9mG2YTZiNivINmF2YbbjNis2LEg2KLar9ix24zZhiDYrNmH2Kog2K/Yp9mG2YTZiNivINin2LHYs9in2YQg2LTYr9iMINiv2LEg2LXZiNix2KrbjCDZhtmF24wg2K7ZiNin2YfbjNivINmE24zZhtqpINi02YXYpyDYqtmI2LPYtyDYotqv2LHbjNmGINiv2LHbjNin2YHYqiDYtNmI2K8g2YjYp9ix2K8g2KjYrti0INiq2YbYuNuM2YXYp9iqINmG2LHZhSDYp9mB2LLYp9ixINi02YjbjNivINuM2Kcg2KfYsiDYt9ix24zZgiDZvtmG2KzYsdmHINuMINmH2YjYtNmF2YbYryDYotqv2LHbjNmGINix2YjbjCDYtdmB2K3ZhyDbjCDYrtmI2K8g2KLZhiDYsdinINiu2KfZhdmI2LQg2Ygg2LrbjNixINmB2LnYp9mEINqp2YbbjNivLg0KCQkJICA8L2Rpdj4NCgkJCTwvZGl2Pg0KCQkJDQogICAgICAgIDwvZGl2Pg0KIDwvYm9keT4NCiA8L2h0bWw+"));
        }

        public static Action<WebResponseData> ResponseCompleteAction { get; set; }
        public static Action<MultipeWebResponseData> MultipeResponseCompleteAction { get; set; }

        public static void Start()
        {
            //if (Fiddler.FiddlerApplication.IsStarted())
            //    return;
            //Fiddler.FiddlerApplication.ResponseHeadersAvailable -= FiddlerApplication_AfterSessionComplete;
            //Fiddler.FiddlerApplication.ResponseHeadersAvailable += FiddlerApplication_AfterSessionComplete;

            //Fiddler.FiddlerApplication.BeforeResponse -= FiddlerApplication_BeforeResponse;
            //Fiddler.FiddlerApplication.BeforeResponse += FiddlerApplication_BeforeResponse;
            //Fiddler.FiddlerApplication.BeforeRequest -= FiddlerApplication_BeforeRequest;
            //Fiddler.FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;

            //Fiddler.FiddlerApplication.BeforeReturningError -= FiddlerApplication_BeforeReturningError;
            //Fiddler.FiddlerApplication.BeforeReturningError += FiddlerApplication_BeforeReturningError;

            //Fiddler.FiddlerApplication.Startup(6699, Fiddler.FiddlerCoreStartupFlags.MonitorAllConnections | Fiddler.FiddlerCoreStartupFlags.RegisterAsSystemProxy);
        }

        //private static void FiddlerApplication_BeforeReturningError(Fiddler.Session oSession)
        //{

        //}

        public static void Stop()
        {
            //Fiddler.FiddlerApplication.Shutdown();
        }

        //static void FiddlerApplication_BeforeRequest(Fiddler.Session oSession)
        //{
        //    oSession.bBufferResponse = true;
        //}

        //static void FiddlerApplication_BeforeResponse(Fiddler.Session session)
        //{
        //    GenerateSession(session);
        //}

        //static void FiddlerApplication_AfterSessionComplete(Fiddler.Session session)
        //{
        //    GenerateSession(session);
        //}

        //static void GenerateSession(Fiddler.Session session)
        //{
        //    try
        //    {
        //        if (session.ResponseHeaders.Exists("AgrinResponseToDownload"))
        //        {
        //            session.ResponseHeaders.RemoveAll();
        //            session.utilDecodeResponse();
        //            session.utilSetResponseBody(GetDownloadPage());
        //            session.ResponseHeaders["AgrinResponseToDownload"] = "ok";
        //            return;
        //        }
        //        var process = System.Diagnostics.Process.GetProcessById(session.LocalProcessID);
        //        if (process == null || string.IsNullOrEmpty(process.ProcessName))
        //            return;
        //        if (NetworkProxySettings.Current == null || !NetworkProxySettings.ExistProcesses(process.ProcessName))
        //            return;

        //        string fileName = "";
        //        string extension = "";
        //        if (session.ResponseHeaders.Exists("content-disposition"))
        //        {
        //            var disp = Agrin.IO.Strings.Decodings.UrlDecode(session.ResponseHeaders["content-disposition"]);
        //            try
        //            {
        //                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition(disp);
        //                fileName = cd.FileName;
        //            }
        //            catch
        //            {
        //                if (!string.IsNullOrEmpty(disp) && disp.Contains("filename="))
        //                {
        //                    fileName = disp.Substring(disp.IndexOf("filename=") + 9).Replace("\"", "");
        //                }
        //            }
        //        }

        //        if (string.IsNullOrEmpty(fileName))
        //            fileName = System.IO.Path.GetFileName(session.fullUrl);
        //        if (fileName.Length > 1 && fileName.Contains("?"))
        //            fileName = fileName.Substring(0, fileName.IndexOf("?"));
        //        var fileNameExt = "";
        //        if (!string.IsNullOrEmpty(fileName))
        //            fileNameExt = MPath.GetFileExtention(fileName);
        //        if (session.ResponseHeaders.Exists("content-type"))
        //        {
        //            var extentions = MimeTypeHelper.GetExtension(session.ResponseHeaders["content-type"]);
        //            if (extentions != null)
        //            {
        //                if (extentions.Length == 1)
        //                {
        //                    extension = extentions.First();
        //                }
        //                else
        //                {
        //                    if (!string.IsNullOrEmpty(fileName))
        //                    {
        //                        var fileExt = MPath.GetFileExtention(fileName).ToLower();
        //                        if (extentions.Contains(fileExt) || MimeTypeHelper.ExistExtensionOnDB(fileExt))
        //                            extension = fileExt;
        //                        else
        //                            extension = extentions.First();
        //                    }
        //                }
        //            }
        //        }
        //        if (string.IsNullOrEmpty(extension) && !string.IsNullOrEmpty(fileName))
        //            extension = MPath.GetFileExtention(fileName);
        //        if (!string.IsNullOrEmpty(fileName))
        //        {
        //            fileName = Agrin.IO.Strings.Decodings.FullDecodeString(fileName);
        //        }

        //        if (!NetworkProxySettings.IsSupportExtension(extension))
        //        {
        //            if (NetworkProxySettings.IsSupportExtension(fileNameExt))
        //            {
        //                extension = fileNameExt;
        //            }
        //            else
        //            {

        //                return;
        //            }
        //        }

        //        WebResponseData data = new WebResponseData();
        //        data.Uri = Agrin.IO.Strings.Decodings.UrlDecode(session.fullUrl);
        //        data.FileName = fileName;
        //        data.Extension = extension;
        //        try
        //        {
        //            foreach (var header in session.RequestHeaders)
        //            {
        //                data.Headers.Add(header.Name, header.Value);
        //            }
        //        }
        //        catch(Exception e)
        //        {
        //            AutoLogger.LogError(e, "GenerateSession 1");
        //        }
        //        session.ResponseHeaders.RemoveAll();
        //        session.utilDecodeResponse();
        //        var text = GetDownloadPage();
        //        session.utilSetResponseBody(text);
        //        session.ResponseHeaders["AgrinResponseToDownload"] = "ok";
        //        ResponseCompleteAction?.Invoke(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        AutoLogger.LogError(ex, "GenerateSession 2");
        //    }
        //}
    }
}

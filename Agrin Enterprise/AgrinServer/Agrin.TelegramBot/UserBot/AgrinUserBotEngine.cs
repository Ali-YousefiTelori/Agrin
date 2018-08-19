using HeyRed.Mime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TeleSharp.TL.Upload;
using TLSharp.Core;

namespace Agrin.TelegramBot.UserBot
{
    public class StreamObject
    {
        public AutoResetEvent ResetEvent { get; set; }
        public TelegramStream Value { get; set; }
        public string FileName { get; set; }
    }
    public static class AgrinUserBotEngine
    {
        public static bool IsExist(int messageId)
        {
            return FileStreamActions.TryGetValue(messageId, out StreamObject streamObject);
        }

        public static StreamObject GetStream(int messageId)
        {
            if (FileStreamActions.ContainsKey(messageId))
                throw new Exception("key exist on database!");
            AutoResetEvent resetEvent = new AutoResetEvent(true);
            resetEvent.Reset();
            Console.WriteLine($"add key {messageId}");

            FileStreamActions.TryAdd(messageId, new StreamObject() { ResetEvent = resetEvent });
            resetEvent.WaitOne();
            if (FileStreamActions.TryGetValue(messageId, out StreamObject streamObject))
                return streamObject;
            throw new Exception("key of stream not found");
        }

        public static void SetStream(int messageId, TelegramStream telegramStream)
        {
            Console.WriteLine($"SetStream called {messageId}");
            if (FileStreamActions.TryGetValue(messageId, out StreamObject streamObject))
            {
                Console.WriteLine($"SetStream success {messageId}");
                streamObject.Value = telegramStream;
                streamObject.ResetEvent.Set();
                streamObject.ResetEvent.Reset();
            }
        }

        public static void Remove(int messageId)
        {
            var result = FileStreamActions.TryRemove(messageId, out StreamObject streamObject);
            Console.WriteLine($"remove key {messageId} {result}");
        }

        static ConcurrentDictionary<int, StreamObject> FileStreamActions { get; set; } = new ConcurrentDictionary<int, StreamObject>();

        public static async void Run(Action finish)
        {
            Console.WriteLine("starting client...");
            TryAgain:
            try
            {
                int apiId = 273181;
                string apiHash = "37ab91cb27cb8617fd2368d7ade4fce1";
                Console.WriteLine("connecting client...");
                //var client = new TelegramClient(apiId, apiHash, null, "alitest", ProxyTcpClient.NewWebProxy);
                var client = new TelegramClient(apiId, apiHash, null, "alitest");//, ProxyTcpClient.NewWebProxy);
                await client.ConnectAsync();
                Console.WriteLine("client connected!");
                if (!client.IsUserAuthorized())
                {
                    var hashKey = await client.SendCodeRequestAsync("989379078100");
                    Console.WriteLine("client set sms code to login");
                    var code = Console.ReadLine();
                    TLUser myuser = await client.MakeAuthAsync("989379078100", hashKey, code);
                }
                Console.WriteLine("client check IsUserAuthorized");
                var isok = client.IsUserAuthorized();
                if (!isok)
                    throw new Exception("client user need login as session!");
                Console.WriteLine("client connect ok!");

                while (true)
                {
                    Thread.Sleep(5000);
                    Console.WriteLine("try get lock...");

                    try
                    {
                        TelegramStream.lockobj.Wait();
                        Console.WriteLine("try get dialog...");
                        var dialogs = await client.GetUserDialogsAsync() as TLDialogs;
                        Console.WriteLine("Check Unread Message...");

                        foreach (var dia in dialogs.Dialogs.Where(x => x.Peer is TLPeerUser && x.UnreadCount > 0))
                        {
                            Console.WriteLine("foreach unread messages...");
                            var peer = dia.Peer as TLPeerUser;
                            var chat = dialogs.Messages.OfType<TLMessage>().FirstOrDefault();
                            if (chat == null)
                                continue;
                            var user = dialogs.Users.OfType<TLUser>().FirstOrDefault(x => x.Id == ((TLPeerUser)dia.Peer).UserId);
                            var target = new TLInputPeerUser() { UserId = ((TLPeerUser)dia.Peer).UserId, AccessHash = user.AccessHash.GetValueOrDefault() };
                            var hist = await client.GetHistoryAsync(target, 0, -1, dia.UnreadCount) as TLMessagesSlice;
                            if (hist == null)
                                continue;
                            Console.WriteLine("THIS IS:" + chat.Id + " WITH " + dia.UnreadCount + " UNREAD MESSAGES");
                            foreach (var m in hist.Messages.OfType<TLMessage>())
                            {
                                if (m.Media is TLMessageMediaDocument documentMessage)
                                {
                                    if (documentMessage.Document is TLDocument document)
                                    {
                                        if (!FileStreamActions.TryGetValue(m.FromId.GetValueOrDefault(), out StreamObject streamObject))
                                            continue;
                                        var streamForRead = new TelegramStream(client, new TLInputDocumentFileLocation()
                                        {
                                            AccessHash = document.AccessHash,
                                            Id = document.Id,
                                            Version = document.Version
                                        }, document.Size);
                                        streamObject.FileName = document.Attributes.OfType<TLDocumentAttributeFilename>().Select(x => x.FileName).DefaultIfEmpty("").FirstOrDefault();
                                        SetStream(m.FromId.GetValueOrDefault(), streamForRead);
                                    }
                                    else
                                    {
                                        Console.WriteLine("not support :" + m.Media.GetType().FullName);
                                    }
                                }
                                else if (m.Media is TLMessageMediaPhoto photoMessage)
                                {
                                    if (photoMessage.Photo is TLPhoto photo)
                                    {
                                        TLPhotoSize maxSizePhoto = photo.Sizes.OfType<TLPhotoSize>().Aggregate((x, y) => x.Size > y.Size ? x : y);
                                        var location = (TLFileLocation)maxSizePhoto.Location;

                                        if (!FileStreamActions.TryGetValue(m.FromId.GetValueOrDefault(), out StreamObject streamObject))
                                            continue;
                                        var streamForRead = new TelegramStream(client, new TLInputFileLocation()
                                        {
                                            LocalId = location.LocalId,
                                            Secret = location.Secret,
                                            VolumeId = location.VolumeId
                                        }, maxSizePhoto.Size);
                                        streamObject.FileName = "photo" + MimeTypesMap.GetExtension(maxSizePhoto.Type);
                                        SetStream(m.FromId.GetValueOrDefault(), streamForRead);
                                    }
                                    else
                                    {
                                        Console.WriteLine("not support :" + m.Media.GetType().FullName);
                                    }
                                }
                                else if (m.Media != null)
                                {
                                    Console.WriteLine("not support :" + m.Media.GetType().FullName);
                                }
                                else
                                {
                                    Console.WriteLine("skip message :" + m.GetType().FullName);
                                }
                                Console.WriteLine("message readed :" + m.GetType().FullName + " media : " + (m.Media == null ? "null" : m.Media.GetType().FullName) + " " + FileStreamActions.ContainsKey(m.FromId.GetValueOrDefault()));

                                var markAsRead = new TeleSharp.TL.Messages.TLRequestReadHistory()
                                {
                                    Peer = target
                                };
                                var readed = await client.SendRequestAsync<TLAffectedMessages>(markAsRead);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"client excetion to read something! {ex.Message}");
                        finish();
                        break;
                    }
                    finally
                    {
                        TelegramStream.lockobj.Release();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("client connect exception!");
                Thread.Sleep(1000);
                goto TryAgain;
            }
        }
    }

    public class ProxyTcpClient
    {
        public TcpClient GetClient(string targetHost, int targetPort, string httpProxyHost, int httpProxyPort, string proxyUserName, string proxyPassword)
        {
            const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance;
            Uri proxyUri = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttp,
                Host = httpProxyHost,
                Port = httpProxyPort
            }.Uri;
            Uri targetUri = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttp,
                Host = targetHost,
                Port = targetPort
            }.Uri;

            WebProxy webProxy = new WebProxy(proxyUri, true);
            webProxy.Credentials = new NetworkCredential(proxyUserName, proxyPassword);
            WebRequest request = WebRequest.Create(targetUri);
            request.Proxy = webProxy;
            request.Method = "CONNECT";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            Type responseType = responseStream.GetType();
            PropertyInfo connectionProperty = responseType.GetProperty("Connection", Flags);
            var connection = connectionProperty.GetValue(responseStream, null);
            Type connectionType = connection.GetType();
            PropertyInfo networkStreamProperty = connectionType.GetProperty("NetworkStream", Flags);
            NetworkStream networkStream = (NetworkStream)networkStreamProperty.GetValue(connection, null);
            Type nsType = networkStream.GetType();
            PropertyInfo socketProperty = nsType.GetProperty("Socket", Flags);
            Socket socket = (Socket)socketProperty.GetValue(networkStream, null);

            return new TcpClient { Client = socket };
        }

        public static TcpClient NewWebProxy(string ip, int port)
        {
            string proxyServer = "dev.atitec.ir";
            int proxyPort = 808;
            string proxyUsername = "atitec";
            string proxyPassword = "atitec123";
            ProxyTcpClient client = new ProxyTcpClient();
            return client.GetClient(ip, port, proxyServer, proxyPort, proxyUsername, proxyPassword);
        }

        public static WebProxy ManualProxy(string host, int port, string userName, string password)
        {
            WebProxy webProxy = new WebProxy(host, port);
            webProxy.Credentials = new NetworkCredential(userName, password);
            //request.Proxy = webProxy;
            return webProxy;
        }
    }
}

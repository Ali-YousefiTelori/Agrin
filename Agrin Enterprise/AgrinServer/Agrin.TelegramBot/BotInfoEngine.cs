using Agrin.StorageServer.ServiceLogics.StorageManager;
using Agrin.TelegramBot.UserBot;
using AgrinMainServer.OneWayServices;
using HeyRed.Mime;
using SignalGo.Shared;
using SignalGo.Shared.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace Agrin.TelegramBot
{
    public abstract class BotInfoEngineBase
    {  /// <summary>
       /// کلید ای پی آی ربات
       /// </summary>
        internal string ApiKey { get; set; }
        /// <summary>
        /// کلاینت بات مورد نظر تا سیستم به آن وصل شود و پیام ها را دریافت کند
        /// این کلاینت پیام های کاربران که به ربات فرستاده میشود را شنود میکنند
        /// </summary>
        internal TelegramBotClient CurrentBotClient { get; set; }
        /// <summary>
        /// زمانی که سیستم متصل شد
        /// </summary>
        internal DateTime ConnectedDate = DateTime.Now;

        /// <summary>
        /// سازنده ی کلاس که کلید ای پی آی را برای اتصال نیاز  دارد
        /// </summary>
        /// <param name="apiKey"></param>
        public BotInfoEngineBase(string apiKey)
        {
            ApiKey = apiKey;
        }

        /// <summary>
        /// شروع اتصال ربات
        /// </summary>
        public async void Start()
        {
            //CurrentBotClient = new TelegramBotClient(ApiKey, WebRequest.DefaultWebProxy);
            CurrentBotClient = new TelegramBotClient(ApiKey);
            {
                //Timeout = new TimeSpan(0, 0, 5)
            };
            ConnectedDate = DateTime.Now;
            CurrentBotClient.StartReceiving();
            CurrentBotClient.OnMessage += TelegramBotClient_OnMessage;

            User me = await CurrentBotClient.GetMeAsync().ConfigureAwait(false);
            Console.WriteLine($"Bot {me.FirstName} {me.LastName} Connected!");
        }

        int mainUserId = 615366842;
        public virtual void TelegramBotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            Console.WriteLine("receive message!");

            int agrinUserId = 0;
            int tlmessageId = 0;
            AsyncActions.Run(() =>
            {
                Console.WriteLine("calculate message!");

                var text = string.IsNullOrEmpty(e.Message.Text) ? "" : e.Message.Text;
                var command = text.ToLower();
                if (command == "/start")
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("با سلام، به ربات آپلود منیجر آگرین خوش امدید، این ربات به شما کمک میکند تا از فایل های تلگرام یا هر سایت دیگر لینک مستقیم ساخته و با سرعت بالا دانلود کنید.");
                    builder.AppendLine("برای ثبت نام /Register را امتحان کنید، با اولین ثبت نام یک گیگابایت هدیه دریافت کنید");
                    builder.AppendLine("برای پشتیبانی آنلاین به گروه https://t.me/joinchat/CVPIBj5xvvgZOPBEO-YfTA مراجعه کنید");
                    builder.AppendLine("بهتر است برای سهولت در دانلود از دانلود منیجر آگرین برای دانلود فایل ها استفاده کنید، برای دریافت دانلود منیجر آگرین به کانال @AgrinDM مراجعه کنید.");
                    builder.AppendLine("برای دریافت لینک مستقیم فقط لینک یا فایل را در ربات شیر کنید");
                    builder.AppendLine("اوقات خوشی را برای شما آرزو میکنیم");
                    builder.AppendLine("دستورات کلی:");
                    builder.AppendLine("شروع /Start");
                    builder.AppendLine("ثبت نام /Register");
                    builder.AppendLine("مشاهده میزان شارژ شما /Charge");
                    SendTextMessageForce(e.Message.Chat.Id, builder.ToString());
                    return;
                }
                var userId = e.Message.From.Id;
                if (e.Message.Contact != null && e.Message.Contact.UserId != e.Message.From.Id)
                {
                    SendTextMessageForce(e.Message.Chat.Id, "لطفا فقط اطلاعات کاربری خود را به اشتراک بگذارید، با به اشتراک گذاشتن اطلاعات کاربری دیگران نمی توانید در سیستم ثبت نام کنید");
                }
                else
                {
                    var userInfo = StorageAuthenticationService.Current.GetUserByTelegramUserId(userId).Data;
                    if (command == "/register")
                    {
                        if (userInfo == null)
                        {
                            SendTextMessageForce(e.Message.Chat.Id, "لطفا کاربری تلگرام خود را با ما در اشتراک بگذارید تا ثبت نام شما تکمیل شود، ما به اطلاعات شما جهت جلوگیری از ثبت نام های تکراری و اضافه کردن قابلیت های جدید در اینده نیاز خواهیم داشت.هیچگونه تبلیغ یا سوء استفاده از شماره شما انجام نخواهد گرفت.");
                        }
                        else
                        {
                            SendTextMessageForce(e.Message.Chat.Id, "شما قبلا ثبت نام کردید.برای مشاهده شارژ حساب روی /Charge کلیک کنید یا تایپ کنید");
                        }
                    }
                    else if (e.Message.Contact != null)
                    {
                        if (userInfo == null)
                        {
                            userInfo = StorageAuthenticationService.Current.GetUserByUserName(e.Message.Contact.PhoneNumber).Data;
                            if (userInfo == null)
                            {
                                userInfo = new Server.DataBase.Models.UserInfo()
                                {
                                    TelegramUserId = userId,
                                    UserName = e.Message.Contact.PhoneNumber,
                                    Name = e.Message.Contact.FirstName,
                                    Family = e.Message.Contact.LastName,
                                    RoamUploadSize = 1024 * 1024 * 1024
                                };
                                userInfo = StorageAuthenticationService.Current.AddUser(userInfo).Data;
                                if (userInfo == null)
                                    throw new Exception("cannot register user!");
                            }
                            else
                            {
                                StorageAuthenticationService.Current.ChangeUserTelegramId(userInfo.Id, userId);
                            }
                            SendTextMessageForce(e.Message.Chat.Id, "ثبت نام شما با موفقیت انجام شد.برای مشاهده شارژ حساب روی /Charge کلیک کنید یا تایپ کنید");
                        }
                        else
                        {
                            SendTextMessageForce(e.Message.Chat.Id, "شما قبلا ثبت نام کردید.برای مشاهده شارژ حساب روی /Charge کلیک کنید یا تایپ کنید");
                        }
                    }
                    else if (userInfo == null && userId != mainUserId)
                    {
                        SendTextMessageForce(e.Message.Chat.Id, "اطلاعات شما در سیستم یافت نشد لطفا با کلیک روی /Register در سیستم ثبت نام کنید یا تایپ کنید");
                    }
                    else
                    {
                        long fileSize = 0;
                        Stream streamToRead = null;
                        string filePath = "";
                        bool canShowEmptyError = true;
                        if (command == "/charge")
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.AppendLine("میزان حساب شما:");
                            builder.AppendLine($"{userInfo.RoamUploadSize} بایت");
                            builder.AppendLine($"{userInfo.RoamUploadSize / 1024 } کیلوبایت");
                            builder.AppendLine($"{userInfo.RoamUploadSize / 1024 / 1024} مگابایت");
                            builder.AppendLine($"{userInfo.RoamUploadSize / 1024 / 1024 / 1024} گیگابایت");
                            builder.AppendLine("می باشد");
                            SendTextMessageForce(e.Message.Chat.Id, builder.ToString());
                            canShowEmptyError = false;
                        }
                        else if (e.Message.Voice != null)
                        {
                            if (AgrinUserBotEngine.IsExist(e.Message.From.Id))
                            {
                                SendTextMessageForce(e.Message.Chat.Id, "شما یک فایل در حال دانلود دارید لطفا منتظر بمانید تا دانلود اتمام شود سپس مجددا سعی کنید");
                                return;
                            }
                            Console.WriteLine("forward message!");
                            var message = ForceForward(e.Message.Chat.Id, mainUserId, e.Message.MessageId);
                            Console.WriteLine("wait for result message!");
                            tlmessageId = message.From.Id;
                            var result = AgrinUserBotEngine.GetStream(message.From.Id);
                            Console.WriteLine("result success!");
                            streamToRead = result.Value;
                            fileSize = result.Value.Length;
                            filePath = result.FileName;
                            //var file = GetFileForce(e.Message.Voice.FileId);
                            //streamToRead = GetFileStreamForce(file.FilePath);
                            //fileSize = teleStream.Length;
                            //filePath = file.FilePath;

                        }
                        else if (e.Message.Video != null)
                        {
                            if (AgrinUserBotEngine.IsExist(e.Message.From.Id))
                            {
                                SendTextMessageForce(e.Message.Chat.Id, "شما یک فایل در حال دانلود دارید لطفا منتظر بمانید تا دانلود اتمام شود سپس مجددا سعی کنید");
                                return;
                            }
                            Console.WriteLine("forward message!");
                            var message = ForceForward(e.Message.Chat.Id, mainUserId, e.Message.MessageId);
                            Console.WriteLine("wait for result message!");
                            tlmessageId = message.From.Id;
                            var result = AgrinUserBotEngine.GetStream(message.From.Id);
                            Console.WriteLine("result success!");
                            streamToRead = result.Value;
                            fileSize = result.Value.Length;
                            filePath = result.FileName;
                            //var file = GetFileForce(e.Message.Video.FileId);
                            //streamToRead = GetFileStreamForce(file.FilePath);
                            //filePath = file.FilePath;
                        }
                        else if (e.Message.Document != null)
                        {
                            if (AgrinUserBotEngine.IsExist(e.Message.From.Id))
                            {
                                SendTextMessageForce(e.Message.Chat.Id, "شما یک فایل در حال دانلود دارید لطفا منتظر بمانید تا دانلود اتمام شود سپس مجددا سعی کنید");
                                return;
                            }
                            Console.WriteLine("forward message!");
                            var message = ForceForward(e.Message.Chat.Id, mainUserId, e.Message.MessageId);
                            Console.WriteLine("wait for result message!");
                            tlmessageId = message.From.Id;
                            var result = AgrinUserBotEngine.GetStream(message.From.Id);
                            Console.WriteLine("result success!");
                            streamToRead = result.Value;
                            fileSize = result.Value.Length;
                            filePath = result.FileName;
                            //var file = GetFileForce(e.Message.Document.FileId);
                            //streamToRead = GetFileStreamForce(file.FilePath);
                            //fileSize = file.FileSize;
                            //filePath = file.FilePath;
                        }
                        else if (e.Message.Audio != null)
                        {
                            if (AgrinUserBotEngine.IsExist(e.Message.From.Id))
                            {
                                SendTextMessageForce(e.Message.Chat.Id, "شما یک فایل در حال دانلود دارید لطفا منتظر بمانید تا دانلود اتمام شود سپس مجددا سعی کنید");
                                return;
                            }
                            Console.WriteLine("forward message!");
                            var message = ForceForward(e.Message.Chat.Id, mainUserId, e.Message.MessageId);
                            Console.WriteLine("wait for result message!");
                            tlmessageId = message.From.Id;
                            var result = AgrinUserBotEngine.GetStream(message.From.Id);
                            Console.WriteLine("result success!");
                            streamToRead = result.Value;
                            fileSize = result.Value.Length;
                            filePath = result.FileName;
                            //var file = GetFileForce(e.Message.Audio.FileId);
                            //streamToRead = GetFileStreamForce(file.FilePath);
                            //fileSize = file.FileSize;
                            //filePath = file.FilePath;
                        }

                        //int BistMB = 1024 * 1024 * 20;
                        Uri.TryCreate(text, UriKind.Absolute, out Uri uri);

                        if (streamToRead != null || uri != null)
                        {
                            agrinUserId = userInfo.Id;
                            if (!LinkUploadManager.TryAddUserFileDownloading(userInfo.Id))
                            {
                                SendTextMessageForce(e.Message.Chat.Id, "شما یک فایل در حال دانلود دارید لطفا منتظر بمانید تا دانلود اتمام شود سپس مجددا سعی کنید");
                            }
                            else
                            {

                                if (uri != null)
                                {
                                    try
                                    {
                                        var _request = WebRequest.Create(uri);
                                        _request.Proxy = null;
                                        bool isFTP = false;
                                        if (uri.OriginalString.ToLower().StartsWith("ftp://"))
                                            isFTP = true;
                                        if (!isFTP)
                                        {
                                            ((HttpWebRequest)_request).CookieContainer = new CookieContainer();
                                            (((HttpWebRequest)_request).CookieContainer).Add(new Cookie("allow", "yes", "", _request.RequestUri.Host));
                                        }

                                        InitializeCustomHeaders(_request);
                                        _request.Timeout = 15000;
                                        SetAllowAutoRedirect(_request, true);

                                        SetConnectionLimit(_request, 100);

                                        var response = _request.GetResponse();
                                        if (response.ContentLength <= 0)
                                        {
                                            SendTextMessageForce(e.Message.Chat.Id, "حجم لینک نامشخص است نمی توانیم فایل را دانلود کنیم");
                                            return;
                                        }
                                        fileSize = response.ContentLength;
                                        streamToRead = response.GetResponseStream();
                                        filePath = GetFileName(uri.OriginalString, response.Headers);
                                    }
                                    //catch (WebException ex2)
                                    //{
                                    //    var reader = new StreamReader(ex2.Response.GetResponseStream());
                                    //    var text54 = reader.ReadToEnd();
                                    //}
                                    catch (Exception ex)
                                    {
                                        LinkUploadManager.TryRemoveUserFileDownloading(userInfo.Id);
                                        StringBuilder builder = new StringBuilder();
                                        builder.AppendLine("بررسی لینک با خطا مواجه شده است:");
                                        builder.AppendLine(ex.ToString());
                                        SendTextMessageForce(e.Message.Chat.Id, builder.ToString());
                                        return;
                                    }
                                }
                                //if (userInfo.RoamUploadSize < fileSize && fileSize > BistMB)
                                //{
                                //    LinkUploadManager.TryRemoveUserFileDownloading(userInfo.Id);
                                //    SendTextMessageForce(e.Message.Chat.Id, "شارژ حساب شما به اتمام رسیده است، تنها لینک های زیر 20 مگابایت رایگان می باشند");
                                //}
                                //else
                                //{
                                try
                                {
                                    SendTextMessageWait(e.Message.Chat.Id, "فایل در حال آپلود می باشد.");
                                }
                                catch (TimeoutException)
                                {

                                }
                                byte lastProgerss = 0;
                                long chatId = e.Message.Chat.Id;
                                AsyncActions.Run(() =>
                                {
                                    bool isSendingMessage = false;
                                    object lockobj = new object();

                                    LinkUploadManager.UploadStream(userInfo.Id, streamToRead, text, filePath, fileSize, UltraStreamGo.StreamIdentifier.GetRandomString(5), (progress) =>
                                    {
                                        if ((progress < lastProgerss + 10 && progress < 95 && progress > 5) || lastProgerss == progress)
                                            return;
                                        lastProgerss = progress;
                                        lock (lockobj)
                                        {
                                            if (isSendingMessage)
                                                return;
                                            isSendingMessage = true;
                                        }
                                        AsyncActions.Run(async () =>
                                        {
                                            await SendTextMessageWait(chatId, $"فایل در حال آپلود {progress}% تکمیل شده.");
                                            lock (lockobj)
                                            {
                                                isSendingMessage = false;
                                            }
                                        }, (ex) =>
                                        {
                                            AutoLogger.Default.LogError(ex, "edit messsage");
                                            lock (lockobj)
                                            {
                                                isSendingMessage = false;
                                            }
                                        });
                                    }, (isComplete, fileInfo) =>
                                    {
                                        AgrinUserBotEngine.Remove(tlmessageId);
                                        if (isComplete)
                                        {
                                            try
                                            {
                                                SendTextMessageWait(chatId, "فایل شما با موفقیت آپلود شد.");
                                            }
                                            catch (Exception)
                                            {

                                            }   
                                            StringBuilder builder = new StringBuilder();
                                            builder.AppendLine("آدرس لینک:");
                                            builder.AppendLine($"http://agrindownloadmanager.ir:1397/Download/DownloadFile?{fileInfo.Id}&{fileInfo.Password}");
                                            builder.AppendLine("این لینک حداکثر بعد از دو روز پاک خواهد شد");

                                            SendTextMessageForce(e.Message.Chat.Id, builder.ToString());
                                        }
                                        else
                                        {
                                            SendTextMessageForce(chatId, "خطا در آپلود فایل رخ داده است");
                                        }
                                    });

                                }, (ex) =>
                                {
                                    AgrinUserBotEngine.Remove(tlmessageId);
                                    AutoLogger.Default.LogError(ex, "upload error");
                                    SendTextMessageForce(e.Message.Chat.Id, "خطا در پردازش اطلاعات رخ داده است. برای پشتیبانی به گروه https://t.me/joinchat/CVPIBj5xvvgZOPBEO-YfTA مراجعه کرده و اشکالات را به ما اطلاع دهید.");
                                });
                                // }
                            }
                        }
                        else if (canShowEmptyError)
                        {
                            SendTextMessageForce(e.Message.Chat.Id, "هیچ فایل یا لینکی یافت نشد، لطفا فایل های خود را به ربات فروارد کنید یا لینک خود را در ربات به اشتراک بگذارید.");
                        }
                    }

                }
            }, (ex) =>
            {
                LinkUploadManager.TryRemoveUserFileDownloading(agrinUserId);
                AgrinUserBotEngine.Remove(tlmessageId);
                if (ex.InnerException != null && ex.InnerException.Message == "Bad Request: file is too big")
                {
                    SendTextMessageForce(e.Message.Chat.Id, "در حال حاضر ربات تلگرام قابلیت دانلود فایل های بیشتر از حجم 20 مگابایت از خود تلگرام را ندارند، این محدودیت از خود سرور تلگرام می باشد و ما در حال بررسی هستیم تا راهی پیدا کنیم، ولی فعلا نمی توانیم این لینک را دریافت کنیم");
                }
                else
                {
                    AutoLogger.Default.LogError(ex, "bot error");
                    SendTextMessageForce(e.Message.Chat.Id, "خطا در پردازش اطلاعات رخ داده است. برای پشتیبانی به گروه https://t.me/joinchat/CVPIBj5xvvgZOPBEO-YfTA مراجعه کرده و اشکالات را به ما اطلاع دهید.");
                }
            });
        }

        /// <summary>
        /// set connection limit of we request
        /// </summary>
        /// <param name="_request"></param>
        /// <param name="value"></param>
        public void SetConnectionLimit(WebRequest _request, int value)
        {
            if (_request is HttpWebRequest)
                ((HttpWebRequest)_request).ServicePoint.ConnectionLimit = value;
            else if (_request is FtpWebRequest)
                ((FtpWebRequest)_request).ServicePoint.ConnectionLimit = value;
        }

        public void SetAllowAutoRedirect(WebRequest _request, bool value)
        {
            if (_request is HttpWebRequest)
                ((HttpWebRequest)_request).AllowAutoRedirect = value;
        }
        public void InitializeCustomHeaders(WebRequest _request)
        {
            if (!(_request is HttpWebRequest))
                return;
            HttpWebRequest request = (HttpWebRequest)_request;
            request.KeepAlive = true;
            // request.Connection = "keep-alive";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            request.UserAgent = "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:60.0) Gecko/20100101 Firefox/60.0";
        }
        /// <summary>
        /// set cookie for request
        /// </summary>
        /// <param name="_request"></param>
        /// <param name="value"></param>
        public void SetCookieContainer(WebRequest _request, CookieContainer value)
        {
            if (_request is HttpWebRequest)
                ((HttpWebRequest)_request).CookieContainer = value;
        }

        public void SendTextMessageForce(long chatId, string message)
        {
            TryAgain:
            try
            {
                 CurrentBotClient.SendTextMessageAsync(chatId, message).GetAwaiter().GetResult();
            }
            catch (TimeoutException)
            {
                Thread.Sleep(5000);
                goto TryAgain;
            }
        }

        public Stream GetFileStreamForce(string filePath)
        {
            TryAgain:
            try
            {
                return CurrentBotClient.GetFileStreamAsync(filePath).Result;
            }
            catch (TimeoutException)
            {
                Thread.Sleep(1000);
                goto TryAgain;
            }
        }

        public Telegram.Bot.Types.File GetFileForce(string fileId)
        {
            TryAgain:
            try
            {
                return CurrentBotClient.GetFileAsync(fileId).Result;
            }
            catch (TimeoutException)
            {
                Thread.Sleep(1000);
                goto TryAgain;
            }
        }

        public Message ForceForward(long fromChatId, long toChatId, int messageId)
        {
            TryAgain:
            try
            {
                return CurrentBotClient.ForwardMessageAsync(new ChatId(toChatId), new ChatId(fromChatId), messageId).Result;
            }
            catch (TimeoutException)
            {
                Thread.Sleep(1000);
                goto TryAgain;
            }
        }

        public Task<Message> SendTextMessageWait(long chatId, string message)
        {
            return CurrentBotClient.SendTextMessageAsync(chatId, message);
        }

        public static string GetFileName(string uri, WebHeaderCollection webHeaderCollection)
        {
            string fileName = "";
            string contentType = "";
            if (webHeaderCollection["content-disposition"] != null)
            {
                //try
                //{
                //    ContentDisposition content = new ContentDisposition(webHeaderCollection["content-disposition"]);
                //    fileName = content.FileName;
                //}
                //catch (Exception ex)
                //{

                //}

                fileName = WebUtility.UrlDecode(GetFileNameFromContentDisposition(webHeaderCollection["content-disposition"]));

            }
            if (webHeaderCollection["content-type"] != null)
            {
                contentType = webHeaderCollection["content-type"];
            }

            if (String.IsNullOrEmpty(fileName) || (!String.IsNullOrEmpty(fileName) && (fileName.ToLower().EndsWith("html") || fileName.ToLower().EndsWith("htm"))))
            {
                string getfileName = GetLinksFileName(uri);
                if (Path.HasExtension(getfileName))
                {
                    fileName = getfileName;
                }
            }
            if (!String.IsNullOrEmpty(contentType))
            {
                string ext = MimeTypesMap.GetExtension(contentType);
                if (string.IsNullOrEmpty(ext) || !fileName.ToLower().EndsWith(ext.ToLower()))
                {
                    var extNew = GetFileExtention(fileName);
                    if (extNew != null)
                        extNew = extNew.ToLower();
                    if (extNew == ".htm" || extNew == ".html")
                    {
                        var newFileName = Path.GetFileNameWithoutExtension(fileName) + extNew;
                        fileName = newFileName;
                    }
                }
            }

            return GetFileNameValidChar(fileName);
        }

        public static string GetLinksFileName(string newFileName)
        {
            if (string.IsNullOrEmpty(newFileName))
                return newFileName;
            string decode = WebUtility.HtmlDecode(newFileName.Trim().Trim(new char[] { '/', '\\' }));
            string fileName = null;
            if (Uri.TryCreate(decode, UriKind.Absolute, out Uri uri))
            {
                fileName = WebUtility.HtmlDecode(System.IO.Path.GetFileName(uri.AbsolutePath));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = WebUtility.HtmlDecode(System.IO.Path.GetFileName(decode));
                    if (fileName.Contains("="))
                    {
                        int l = fileName.LastIndexOf('=');
                        if (l < fileName.Length - 1)
                        {
                            string name = fileName.Substring(l + 1, fileName.Length - l - 1);
                            if (!string.IsNullOrEmpty(name))
                                fileName = name;
                        }

                    }
                }
            }
            else
                fileName = Path.GetFileName(GetFileNameValidChar(decode));

            foreach (var item in Path.GetInvalidPathChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            foreach (var item in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            if (string.IsNullOrEmpty(fileName))
                return "notName.html";
            else if (string.IsNullOrEmpty(GetFileExtention(fileName)))
                return fileName.Trim(new char[] { '"' }) + ".html";
            return fileName.Trim(new char[] { '"' });
        }

        public static string GetFileExtention(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                return GetFileNameValidChar(Path.GetExtension(uri.AbsolutePath));
            }
            return GetFileNameValidChar(Path.GetExtension(url));
        }

        public static string GetFileNameFromContentDisposition(string contentDisposition)
        {
            try
            {
                contentDisposition = contentDisposition.Replace("utf-8", "").Replace("UTF-8", "").Replace("'", "");
                contentDisposition = GetFileNameValidChar(contentDisposition);
                if (contentDisposition == "attachment" || contentDisposition == "attachment;")
                    return "";
                return (new System.Net.Mime.ContentDisposition(contentDisposition)).FileName.Trim(new char[] { '"' });
            }
            catch (Exception c)
            {
                string txt = contentDisposition;
                try
                {
                    txt = contentDisposition.Substring(contentDisposition.IndexOf("filename=") + 9);
                    if (txt.IndexOf("\"") == 0)
                    {
                        txt = txt.Remove(0, 1);
                        txt = txt.Substring(0, txt.IndexOf("\""));
                    }
                    else if (txt.Contains(";"))
                    {
                        txt = txt.Substring(0, txt.IndexOf(";"));
                    }
                }
                catch (Exception e)
                {
                }


                txt = GetFileNameValidChar(txt);
                return txt.Trim(new char[] { '"' });
            }
        }

        public static string GetFileNameValidChar(string fileName)
        {
            foreach (var item in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            return fileName;
        }
        //public async void SendSticker(long chatId, Sticker message)
        //{
        //    var sended = await CurrentBotClient.SendStickerAsync(chatId, new InputOnlineFile(message.FileId));
        //}

        //public async void SendPhoto(long chatId, PhotoSize message, string caption)
        //{
        //    var sended = await CurrentBotClient.SendPhotoAsync(chatId, new FileToSend(message.FileId), caption);
        //}

        //public async void SendVoice(long chatId, Voice audio, string caption)
        //{
        //    var sended = await CurrentBotClient.SendVoiceAsync(chatId, new FileToSend(audio.FileId), caption);
        //}

        //public async void SendAudio(long chatId, Audio audio, string caption)
        //{
        //    var sended = await CurrentBotClient.SendAudioAsync(chatId, new FileToSend(audio.FileId), caption, audio.Duration, audio.Performer, audio.Title);
        //}

        //public async void SendVideo(long chatId, Video video, string caption)
        //{
        //    int.TryParse(video.Width, out int width);
        //    int.TryParse(video.Height, out int height);
        //    var sended = await CurrentBotClient.SendVideoAsync(chatId, new FileToSend(video.FileId), video.Duration, width, height, caption);
        //}

        //public async void SendVideoNote(long chatId, VideoNote video, string caption)
        //{
        //    var sended = await CurrentBotClient.SendVideoNoteAsync(chatId, new FileToSend(video.FileId), video.Duration, video.Length);
        //}

        //public async void SendDocument(long chatId, Document document, string caption)
        //{
        //    var sended = await CurrentBotClient.SendDocumentAsync(chatId, new FileToSend(document.FileId), caption);
        //}
        //public async void SendSticker(long chatId, Sticker message)
        //{
        //    var sended = await CurrentBotClient.SendPhotoAsync(chatId, message);
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.LinkExtractor.RapidBaz
{
    public static class RapidBazFindDownloadLink
    {
        static List<string> SupporHosts = new List<string>() { "rapidshare.com", "rapidshare.net", "megaupload.com", "mega.co.nz", "filefactory.com", "depositfiles.com", "easy-share.com", "crocko.com", "gigasize.com", "adrive.com", "4shared.com", "netload.in", "uploaded.to", "uploaded.net", "ul.to", "storage.to", "kickload.com", "uploadbox.com", "badongo.com", "uploading.com", "mediafire.com", "megashares.com", "letitbit.net", "vip-file.com", "zshare.net", "hotfile.com", "gigapeta.com", "ugotfile.com", "filesmonster.com", "megashare.com", "bigandfree.com", "megaftp.com", "ifile.it", "2shared.com", "filesonic.com", "sharingmatrix.com", "freakshare.net", "freakshare.com", "sendspace.com", "turbobit.net", "filebox.com", "oron.com", "appscene.org", "x7.to", "extabit.com", "shareflare.net", "sharecash.org", "fileserve.com", "turboshare.com", "sharejunky.com", "movieshare.in", "speedyshare.com", "unibytes.com", "filestrack.com", "furk.net", "fileape.com", "bitshare.com", "duckload.com", "usershare.net", "davvas.com", "sockshare.com", "putlocker.com", "firedrive.com", "filerio.com", "uploadstation.com", "hulkshare.com", "wupload.com", "enterupload.com", "filepost.com", "filejungle.com", "uploadking.com", "uploadhere.com", "bulletupload.com/		", "fiberupload.com/		", "grupload.com/		", "bayfiles.com/		", "bayfiles.net/		", "rg.to/		", "rapidgator.net/		", "jumbofiles.com/		", "sharebeast.com/		", "slingfile.com/		", "uploadbaz.com/		", "filereactor.com/		", "hipfile.com/		", "share-online.biz/		", "ryushare.com/		", "azushare.net/		", "sharpfile.com", "filedownloads.org", "novafile.com", "lumfile.com", "lumfile.se", "lumfile.eu", "rodfile.com", "ultramegabit.com", "uptobox.com", "expressleech.com", "cloudzer.net", "clz.to", "fileparadox.in", "uploadboy.com", "uploadboy.ir", "uploadscenter.com", "dailymotion.com", "dai.ly", "rarefile.net", "keep2share.cc", "k2s.cc", "keep2s.cc", "uploadhero.co", "uploadhero.com", "oteupload.com", "fileom.com", "dizzcloud.com", "secureupload.eu", "1fichier.com", "terafile.co", "easybytez.com", "junocloud.me", "rioupload.com", "filepup.net", "asfile.com", "oboom.com", "datafile.com", "nitroflare.com", "vip.farskids", "farskids.com", "vip.filmtory", "dl.farskids", "dl2.farskids", "dl3.farskids", "vip-ir.com", "tinyez.tv", "uploadin.net", "xerver.co", "uploadable.ch", "srvdl.com" };
        public static bool IsRapidBazLink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                if (uri.Host.ToLower().Contains("rapidbaz.com"))
                    return true;
            }
            else
                return false;
            return false;
        }

        public static bool SupportThisLink(string address)
        {
            if (string.IsNullOrEmpty(address))
                return false;
            address = address.ToLower();
            Uri uri = null;
            if (Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out uri))
            {
                if (SupporHosts.Contains(uri.Host.ToLower()))
                    return true;
            }
            return false;
        }

        public static string GetErrorTextByErrorNumber(string errorCode)
        {
            switch(errorCode)
            {
                case "4041":
                    {
                        return @"متاسفانه شناسه مستعار و دامنه مرتبط با فایل شما توسط سیستم مردود شناخته شده است
لطفا به داخل سیستم بازگردید، لینک خود را دوباره بازیابی کنید و نسبت به دانلود آن اقدام کنید";
                    }
                case "4042":
                    {
                        return @"به دلیل انتقال نا موفق و یا ایراد سیستم فایل مربوطه در سیستم موجود نیست و حجمی بابت آن کسر نشده است
لطفا به داخل سیستم بازگردید، لینک خود را دوباره بازیابی کنید و نسبت به دانلود آن اقدام کنید";
                    }
                case "4043":
                    {
                        return @"اعتبار لینک شما در سرورهای ما به اتمام رسیده است و فایل مربوطه از روی سرورها حذف شده است
لطفا به داخل سیستم بازگردید، در قسمت فایلهای درخواست با استفاده از فلش آبی دوباره آن را درخواست دهید";
                    }
                case "4044":
                    {
                        return @"متاسفانه شناسه مستعار و دامنه مرتبط با فایل شما توسط سیستم مردود شناخته شده است
لطفا به داخل سیستم بازگردید، در قسمت فایلهای درخواست با استفاده از فلش آبی دوباره آن را درخواست دهید";
                    }
                case "4045":
                    {
                        return @"شما به عنوان کاربر رایگان،‌ تنها می توانید ۲ دانلود همزمان داشته باشید و در حال حاضر سیستم بیش از این تعداد را با مشخصات شما شناسایی کرده است
لطفا نسبت به تهیه اشتراک رپیدباز اقدام نمایید و یا در فواصلی دیگر و بعد از اتمام دانلودتان مجددا سعی کنید";
                    }
                case "4046":
                    {
                        return @"شما به عنوان کاربر رایگان،‌ تنها می توانید ۲ دانلود همزمان داشته باشید و در حال حاضر سیستم بیش از این تعداد را با مشخصات شما شناسایی کرده است
لطفا نسبت به تهیه اشتراک رپیدباز اقدام نمایید و یا در فواصلی دیگر و بعد از اتمام دانلودتان مجددا سعی کنید";
                    }
                case "4047":
                    {
                        return @"به دلیل نامشخصی سرور میزبان فایل این لینک را پیدا نکرده است
لطفا با پشتیبانی تماس گرفته و آدرس فایل مربوطه را جهت بررسی برایشان ارسال کنید";
                    }
                case "4048":
                    {
                        return @"لینک مستقیم ساخته شده حذف و غیرفعال شده است
لطفا با مالک فایل تماس گرفته و از وی بخواهید لینک مستقیم جدید با قرار دادن فایل در «پوشه مستقیم» ایجاد کرده و به شما بدهد";
                    }
                case "401":
                    {
                        return @"دسترسی به صفحه درخواستی بدلیل آنکه شما مشترک رپیدباز نیستید و یا سیستم شما را نشناخته است محدود شده است
با ورود به سیستم با اشتراک رپیدباز خود می توانید مجددا سعی کنید. در صورتیکه فکر می کنید سیستم مشکلی دارد با پشتیبانی رپیدباز تماس بگیرید";
                    }
                case "402":
                    {
                        return @"دسترسی به صفحه درخواستی بدلیل آنکه سرویس حرفه ای رپیدباز شما فعال نمی باشد محدود شده است
با تهیه سرویس حرفه ای می توانید به محتویات درخواستی دسترسی پیدا کنید.  در صورتیکه فکر می کنید سیستم مشکلی دارد با پشتیبانی رپیدباز تماس بگیرید";
                    }
                case "403":
                    {
                        return @"دسترسی به فایل درخواستی به دلیل عدم تطابق آن با قوانین سیستم بسته شده است
در صورتیکه فکر می کنید خطایی در سیستم رخ داده است لطفا با پشتیبانی تماس بگیرید";
                    }
                case "4031":
                    {
                        return @"دسترسی به فایل درخواستی توسط مالک فایل محدود شده است
لطفا با کسی که از آن لینک فایل را دریافت کرده اید تماس گرفته و اطلاعات بیشتری در زمینه چگونگی دسترسی به آن بخواهید";
                    }
                case "4032":
                    {
                        return @"بدلیل عدم وارد کردن کلمه عبور صحیح دسترسی به فایل درخواستی توسط مالک محدود شده است
لطفا با مالک فایل تماس گرفته و اطلاعات بیشتری در زمینه چگونگی دسترسی دریافت کنید سپس از طریق جعبه زیر آن را دریافت کنید";
                    }
                case "4033":
                    {
                        return @"دسترسی به فایل درخواستی توسط مالک با استفاده از لینک کدگذاری شده فایل محدود شده است
لطفا مطمئن شوید که از همان کامپیوتری که درخواست دانلود داده اید در حال دانلود هستید و  در صورتیکه مشکل هنوز پابرجاست با سایت مالک فایل مشکل را مطرح کنید";
                    }
                case "4034":
                    {
                        return @"مدت زمان دانلود فایل که توسط مالک فایل از طریق لینک کدگذاری شده مشخص شده است اتمام یافته
لطفا مطمئن شوید در زمان مشخص شده لینک را دانلود می کنید و در صورتیکه مشکل هنوز پابرجاست با سایت مالک فایل مشکل را مطرح کنید";
                    }
                case "404":
                    {
                        return @"فایل شما در سرور موجود نمی باشد
پیام خطای سرور زمانی رخ می دهد که درخواست شما برای سرور خوانا نباشد
لطفا لینک خود را چک کنید و دوباره لینک خود را از منبع خود دریافت کنید. 
اگر مطمئن هستید که لینک شما مشکلی ندارد لطفا لینک خود را ذخیره کرده و بعدا امتحان کنید. 
ممکن است در حال حاضر به خاطر فشار بالا سرور قادر به پاسخگویی به درخواست شما نباشد. ";
                    }
                case "410":
                    {
                        return @"تایید هویت شما جهت دانلود پرمیوم میسر نبود
جهت دانلود پرمیوم شما باید تنظیمات مربوطه را در دانلود منیجر خود انجام دهید.
لطفا جهت دریافت تنظیمات مربوطه برای دانلود منیجر خود به وبلاگ مراجعه کنید. 
در صورتیکه شما مشترک رپیدباز نیستید به صفحه اصلی سیستم مراجعه کرده و نسبت به تهیه اشتراک اقدام نمایید.
ممکن است در حال حاضر به خاطر فشار بالا سرور قادر به پاسخگویی به درخواست شما نباشد.";
                    }
            }
            return "";
        }
    }
}

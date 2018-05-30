using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.TelegramBot
{
    public static class BotsManager
    {
        /// <summary>
        /// ربات های متصل به سیستم
        /// </summary>
        internal static List<BotInfoEngineBase> Bots { get; set; } = new List<BotInfoEngineBase>();

        /// <summary>
        /// شروع یک ربات
        /// </summary>
        /// <param name="apiKey">کلید ای پی آی ربات</param>
        public static void StartBot<T>(string apiKey)
        {
            var bot = (BotInfoEngineBase)Activator.CreateInstance(typeof(T),new object[] { apiKey });
            bot.Start();
            Bots.Add(bot);
        }

        //internal static BotInfoEngineBase FindBotByUserId(int userId)
        //{
        //    var botApiKey = BotInfoHelper.GetBotApiKeyByUserId(userId);
        //    foreach (var bot in Bots.ToList())
        //    {
        //        if (bot.ApiKey == botApiKey)
        //            return bot;
        //    }
        //    return null;
        //}

        //internal static BotInfoEngine FindBotByTelegramUserId(int telegramUserId)
        //{
        //    return FindBotByUserId(UserInfoHelper.GetUserIdByTelegramUserId(telegramUserId));
        //}
    }
}

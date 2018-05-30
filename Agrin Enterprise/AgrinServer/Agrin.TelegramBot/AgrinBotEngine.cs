using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace Agrin.TelegramBot
{
    public class AgrinBotEngine : BotInfoEngineBase
    {
        public AgrinBotEngine(string apiKey) : base(apiKey)
        {

        }

        public override void TelegramBotClient_OnMessage(object sender, MessageEventArgs e)
        {
            base.TelegramBotClient_OnMessage(sender, e);
        }
    }
}

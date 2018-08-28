using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using TradeSharp.Util;

namespace TradeSharp.Client
{
    public class TelegaSignalBot
    {
        public bool IsOnline => chats.Count > 0;

        private List<long> chats = new List<long>();
        private TelegramBotClient telegramClient = new TelegramBotClient(TelegaBotSettings.ApiKey);
        private const string InitCommand = "init";

        public TelegaSignalBot() { }

        public async Task CheckUpdates()
        {
            var updates = await telegramClient.GetUpdatesAsync();
            if (updates?.Length > 0)
            {
                var updateChatIds =
                    updates.Select(x => x.Message.Chat.Id).Distinct().ToArray();
                await InitializationInChat(updateChatIds);
            }
        }

        public async Task PublishTradeSignal(string imagePath, string message)
        {
            foreach (var chatId in chats)
            {
                try
                {
                    using (var stream = new MemoryStream(File.ReadAllBytes(imagePath)))
                    {
                        InputOnlineFile telegaImg = new InputOnlineFile(stream);
                        telegaImg.FileName = "SharpSignal";

                        await telegramClient.SendChatActionAsync(chatId, ChatAction.UploadPhoto);
                        await telegramClient.SendPhotoAsync(chatId, telegaImg, message);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Ошибка публикации сигнала в чат телеги", e);
                    //TODO logging "Send signal error: " + e.GetBaseException().Message
                }
            }
        }


        private async Task InitializationInChat(long[] updateChatIds)
        {
            foreach (var chatId in updateChatIds)
            {
                var responceText = string.Empty;
                if (!chats.Contains(chatId))
                {
                    chats.Add(chatId);
                    responceText = "Ok. I'm ready to work.";
                    await telegramClient.SendTextMessageAsync(chatId, responceText);
                }
            }
        }
    }
}
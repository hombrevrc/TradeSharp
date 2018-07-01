using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSharp.Util;

namespace TradeSharp.Server.BL
{
    /// <summary>
    /// Реализация, читающая настройки из App.config
    /// </summary>
    public class SmsSenderSettings : ISmsSenderSettings
    {
        /// <summary>
        /// Уникальные идентификаторы PlatformUser
        /// </summary>
        public List<int> RecipientIds { get; set; }
        public string ApiId { get; set; }
        public bool IsPaused { get; set; }
        public int SendHour { get; set; }

        public SmsSenderSettings()
        {
            RecipientIds = new List<int>();
        }

        public bool UpdateSettings()
        {
            RecipientIds.Clear();
            SendHour = AppConfig.GetIntParam("SmsSender.SendHour", 0);
            IsPaused = AppConfig.GetBooleanParam("SmsSender.IsPaused", true);
            ApiId = AppConfig.GetStringParam("SmsSender.ApiId", string.Empty);
            var recipientIdSource = AppConfig.GetStringParam("SmsSender.RecipientIds", string.Empty);

            if (string.IsNullOrEmpty(ApiId) || string.IsNullOrEmpty(recipientIdSource))
                return false;

            var recipientIdSourceArray = recipientIdSource.Split(',');

            foreach (var recipientIdString in recipientIdSourceArray)
            {
                int recipientId = -1;
                if (int.TryParse(recipientIdString, out recipientId))
                    RecipientIds.Add(recipientId);
            }

            return true;
        }
    }
}
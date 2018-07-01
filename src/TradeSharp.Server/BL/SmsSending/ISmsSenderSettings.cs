using System.Collections.Generic;

namespace TradeSharp.Server.BL
{
    interface ISmsSenderSettings
    {
        /// <summary>
        /// Уникальные идентификаторы PlatformUser
        /// </summary>
        List<int> RecipientIds { get; set; }
        string ApiId { get; set; }
        bool IsPaused { get; set; }
        int SendHour { get; set; }
    }
}
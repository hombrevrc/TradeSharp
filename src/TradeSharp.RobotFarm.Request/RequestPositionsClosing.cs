using System.Collections.Generic;

namespace TradeSharp.RobotFarm.Request
{
    [JsonResponseType(typeof(JsonResponsePositionsClosing))]
    public class RequestPositionsClosing : JsonRequest
    {
        public List<Position> positions = new List<Position>();

        public RequestPositionsClosing()
        {
            RequestType = JsonRequestType.PositionsClosing;
        }

        public RequestPositionsClosing(long requestId)
            : this()
        {
            RequestId = requestId;
        }
    }
}

using System.Collections.Generic;

namespace TradeSharp.RobotFarm.Request
{
    [JsonResponseType(typeof(JsonResponsePositionsModifying))]
    public class RequestPositionsModifying : JsonRequest
    {
        public List<Position> positions = new List<Position>();

        public RequestPositionsModifying()
        {
            RequestType = JsonRequestType.PositionsModifying;
        }

        public RequestPositionsModifying(long requestId)
            : this()
        {
            RequestId = requestId;
        }
    }
}

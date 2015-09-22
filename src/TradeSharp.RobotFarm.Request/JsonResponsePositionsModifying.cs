namespace TradeSharp.RobotFarm.Request
{
    public class JsonResponsePositionsModifying : JsonResponse
    {
        public int CountModified { get; set; }

        public int CountFail { get; set; }
    }
}

using System.Collections.Generic;

namespace Network.NetRequests
{
    public enum RequestType
    {
        GET,
        POST,
        None
    }

    public enum GameStatus
    {
        Waiting = 0,
        Playing = 1,
        End = 2
    }

    public enum ChessType
    {
        Black = 0,
        White = 1,
        Unknown = 2
    }

    public static class NetRequests
    {
        public static readonly Dictionary<RequestType, string> RequestTypeKVPairs = new()
        {
            {RequestType.GET, "GET"},
            {RequestType.POST, "POST"}
        };
    }
}

namespace GHC.VideoServer.Model
{
    public class EndPointToCacheServerConnection
    {
        public int ID { get; set; }
        public int CacheServerID { get; set; }
        public int LatencyInMilliSecondsFromCacheToEndpoint { get; set; }
        public EndPoint EndPoint { get; set; }
    }
}
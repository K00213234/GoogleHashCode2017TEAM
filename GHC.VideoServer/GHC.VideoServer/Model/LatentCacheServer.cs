namespace GHC.VideoServer.Model
{
    public class LatentCacheServer
    {
        private int _latencyInMilliseconds;
        private CacheServer _cacheServer;
        public LatentCacheServer(CacheServer cacheServer, int latencyInMilliSeconds)
        {
            _cacheServer = cacheServer;
            _latencyInMilliseconds = latencyInMilliSeconds;
        }
        public CacheServer Cache {get { return _cacheServer; } }
        public int LatencyInMilliSeconds { get { return _latencyInMilliseconds; } }
        public int ConsumedSpace { get { return _cacheServer.ConsumedSpace(); } }

        public int MaxMB { get { return _cacheServer.MaxMB; } }

        public int ID { get { return _cacheServer.ID; } }

        public double CalculateCachingScore(RequestDescription request)
        {
            double numberOfRequests = (double)request.NumberOfReqeusts;
            double videoSize = (double)request.Video.VideoSizeInMb;
            double latency = (double)LatencyInMilliSeconds;

            return (numberOfRequests * videoSize) / latency;
        }

        public AddToCacheResult AddRequestToCache(RequestDescription request, double cachingScore)
        {
            if (request.Video.VideoSizeInMb > (MaxMB - ConsumedSpace))
            {
                return AddToCacheResult.NotEnoughFreeSpace;
            }

            if (Cache.VideoCache.ContainsKey(request.VideoID))
            {
                Cache.VideoCache[request.VideoID].CacheScore += cachingScore;
            }
            else
            {
                Cache.VideoCache.Add(request.VideoID, new CachedVideoRequest
                {
                    Video = request.Video,
                    VideoID = request.VideoID,
                    CacheScore = cachingScore
                });
            }
            
            return AddToCacheResult.Added;
        }
    }
}

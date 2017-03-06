namespace GHC.VideoServer.Model
{
    public class LatentCacheServer
    {
        private readonly CacheServer _cacheServer;
        public LatentCacheServer(CacheServer cacheServer, int latencyInMilliSeconds)
        {
            _cacheServer = cacheServer;
            LatencyInMilliSeconds = latencyInMilliSeconds;
        }
        public CacheServer Cache => _cacheServer;
        public int LatencyInMilliSeconds { get; }
        public int ConsumedSpace => _cacheServer.ConsumedSpace();

        public int MaxMb => _cacheServer.MaxMB;

        public int ID { get { return _cacheServer.ID; } }

        public double CalculateCachingScore(RequestDescription request)
        {
            double numberOfRequests = (double)request.NumberOfReqeusts;// / 1000;
            double videoSize = (double)request.Video.VideoSizeInMb;// / 1000;
            double latency = (double)LatencyInMilliSeconds;// / 1000;

            return (numberOfRequests / videoSize) / latency;
        }

        public AddToCacheResult AddRequestToCache(RequestDescription request, double cachingScore)
        {
            if (request.Video.VideoSizeInMb > (MaxMb - ConsumedSpace))
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


        public AddToCacheResult AddRequestToInfiniteCache(RequestDescription request, double cachingScore)
        {          
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
                    CacheScore = cachingScore,
                    OriginalRequest =  request
                });
                request.IsCached = true;
            }

            return AddToCacheResult.Added;
        }
    }
}

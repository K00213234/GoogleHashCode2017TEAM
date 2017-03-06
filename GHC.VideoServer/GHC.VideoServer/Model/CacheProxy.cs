using System.Collections.Generic;
using System.Linq;

namespace GHC.VideoServer.Model
{
    public class CacheProxy
    {
        private readonly List<LatentCacheServer> _latentCaches;
        public CacheProxy(List<LatentCacheServer> latentCaches)
        {
            _latentCaches = latentCaches.OrderByDescending(c => c.LatencyInMilliSeconds).ToList();
        }

        public void AddRequest(RequestDescription request)
        {
            bool isAlreadyInCache = false;                      

            for (int i = 0; i < _latentCaches.Count; i++)
            {
                var latentCache = _latentCaches[i];
                if (latentCache.Cache.VideoCache.ContainsKey(request.VideoID))
                {
                    isAlreadyInCache = true;
                    break;
                }
            }
            if (!isAlreadyInCache)
            {
                AddToAnyCache(request);
            }
        }

        private void AddToAnyCache(RequestDescription request)
        {            
            for (int i = 0; i < _latentCaches.Count; i++)
            {
                var latentCache = _latentCaches[i];
                var outcome = latentCache.AddRequestToCache(request, latentCache.CalculateCachingScore(request));

                if (outcome == AddToCacheResult.Added)
                {
                    return;
                }               
            }
        }        
    }
}

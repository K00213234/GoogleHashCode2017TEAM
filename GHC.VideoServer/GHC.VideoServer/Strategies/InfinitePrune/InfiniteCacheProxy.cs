using System.Collections.Generic;
using System.Linq;
using GHC.VideoServer.Model;

namespace GHC.VideoServer.Strategies.InfinitePrune
{
    public class InfiniteCacheProxy
    {
        private readonly List<LatentCacheServer> _latentCaches;
        public InfiniteCacheProxy(List<LatentCacheServer> latentCaches)
        {
            _latentCaches = latentCaches.OrderByDescending(c => c.LatencyInMilliSeconds).ToList();
        }

        public void AddRequest(RequestDescription request)
        {
            AddToAnyCache(request);
        }

        private void AddToAnyCache(RequestDescription request)
        {            
            var isInCache = false;

            if (request.IsCached)
            {
                return;
            }

            foreach (var cacheServer in _latentCaches)
            {
                if (cacheServer.Cache.VideoCache.ContainsKey(request.VideoID))
                {                                        
                    request.IsCached = true;
                    isInCache = true;
                }
            }

            if (isInCache)
                return;

            for (int i = 0; i < _latentCaches.Count; i++)
            {
                var latentCache = _latentCaches[i];
                var outcome = latentCache.AddRequestToInfiniteCache(request, latentCache.CalculateCachingScore(request));
                if (outcome == AddToCacheResult.Added)
                {
                    return; // no need to add to another cache
                }
            }           
        }       
    }
}

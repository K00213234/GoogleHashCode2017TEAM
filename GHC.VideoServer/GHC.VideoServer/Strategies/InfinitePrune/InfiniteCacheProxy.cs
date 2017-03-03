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
            //bool isAlreadyInCache = false;
            //for (int i = 0; i < _latentCaches.Count; i++)
            //{   
            //    var latentCache = _latentCaches[i];

            //    if (latentCache.Cache.VideoCache.ContainsKey(request.VideoID))
            //    {
            //        latentCache.Cache.VideoCache[request.VideoID].CacheScore += latentCache.CalculateCachingScore(request);
            //        isAlreadyInCache = true;
            //    }
            //}
            //if (!isAlreadyInCache)
            //{
                
            //}
        }

        private void AddToAnyCache(RequestDescription request)
        {            
            for (int i = 0; i < _latentCaches.Count; i++)
            {
                var latentCache = _latentCaches[i];
                var outcome = latentCache.AddRequestToInfiniteCache(request, latentCache.CalculateCachingScore(request));
                if (outcome == AddToCacheResult.Added)
                {
                    //return; // no need to add to another cache
                }
            }           
        }       
    }
}

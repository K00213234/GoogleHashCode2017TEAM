using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHC.VideoServer.Model
{
    public class CacheProxy
    {
        private List<LatentCacheServer> _latentCaches;
        public CacheProxy(List<LatentCacheServer> latentCaches)
        {
            _latentCaches = latentCaches.OrderByDescending(c => c.LatencyInMilliSeconds).ToList();
        }

        public void AddRequest(RequestDescription request)
        {            
            for (int i = 0; i < _latentCaches.Count; i++)
            {
                bool isAlreadyInCache = false;
                var latentCache = _latentCaches[i];

                if (latentCache.Cache.VideoCache.ContainsKey(request.VideoID))
                {
                    latentCache.Cache.VideoCache[request.VideoID].CacheScore += latentCache.CalculateCachingScore(request);
                    isAlreadyInCache = true;
                }

                if (isAlreadyInCache)
                    continue;

                var cachingScore = latentCache.CalculateCachingScore(request);             
                var outcome = latentCache.AddRequestToCache(request, cachingScore);
            }
        }

        //private AddToCacheResult NotEnoughFreeSpaceTryAndReplaceALowerScoringItem(LatentCacheServer cache, RequestDescription request)
        //{
        //    var score = cache.CalculateCachingScore(request);
        //    var caches = cache.Cache.VideoCache.OrderBy(x => x.CacheScore).ToList();

        //    bool isBetterScore = false;
        //    int i;
        //    for (i = 0; i < caches.Count; i++)
        //    {
        //        var cachedItem = caches[i];
        //        if (cachedItem.Video.VideoSizeInMb >= request.Video.VideoSizeInMb && cachedItem.CacheScore < score)
        //        {
        //            isBetterScore = true;
        //            break;
        //        }
        //    }

        //    if (isBetterScore)
        //    {
        //        caches.RemoveAt(i);
        //        cache.AddRequestToCache(request, score);
        //        return AddToCacheResult.Added;
        //    }
        //    return AddToCacheResult.NotAdded;
        //}
    }
}

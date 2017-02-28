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
            _latentCaches = latentCaches;
        }

        public void AddRequest(RequestDescription request)
        {
            foreach (var latentCache in _latentCaches)
            {
                var cachingScore = latentCache.CalculateCachingScore(request);
                var outcome = latentCache.AddRequestToCache(request, cachingScore);
                if(outcome == AddToCacheResult.Added || outcome == AddToCacheResult.AlreadyInCache)
                {
                    return;
                }
                
                if(outcome == AddToCacheResult.NotEnoughFreeSpace)
                {
                    outcome = NotEnoughFreeSpaceTryAndReplaceALowerScoringItem(latentCache, request);

                    if(outcome != AddToCacheResult.Added)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private AddToCacheResult NotEnoughFreeSpaceTryAndReplaceALowerScoringItem(LatentCacheServer cache, RequestDescription request)
        {
            var score = cache.CalculateCachingScore(request);
            var caches = cache.Cache.Cache.OrderBy(x => x.CacheScore).ToList();
           
            bool isBetterScore = false;
            int i;
            for(i = 0; i < caches.Count; i++)
            {
                var cachedItem = caches[i];
                if (cachedItem.Video.VideoSizeInMb >= request.Video.VideoSizeInMb && cachedItem.CacheScore < score)
                {
                    isBetterScore = true;
                    break;
                }
            }

            if (isBetterScore)
            {
                caches.RemoveAt(i);
                cache.AddRequestToCache(request, score);
                return AddToCacheResult.Added;
            }
            return AddToCacheResult.NotAdded;
        }
    }
}

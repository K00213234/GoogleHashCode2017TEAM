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
            bool isAlreadyInCache = false;

            for (int i = 0; i < _latentCaches.Count; i++)
            {
                var latentCache = _latentCaches[i];
                foreach (var video in latentCache.Cache.VideoCache)
                {
                    if (video.VideoID == request.VideoID)
                    {
                        //this video is already cached on a reachable cache server, this has double the benefit so up the score (make it harder to remove)
                        video.CacheScore += latentCache.CalculateCachingScore(request);
                    }
                }
            }

            if (!isAlreadyInCache)
            {
                foreach (var latentCache in _latentCaches)
                {
                    var cachingScore = latentCache.CalculateCachingScore(request);
                    var outcome = latentCache.AddRequestToCache(request, cachingScore);
                    if (outcome == AddToCacheResult.Added)
                    {
                        return;
                    }

                    if (outcome == AddToCacheResult.NotEnoughFreeSpace)
                    {
                        outcome = NotEnoughFreeSpaceTryAndReplaceALowerScoringItem(latentCache, request);

                        if (outcome == AddToCacheResult.Added)
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private AddToCacheResult NotEnoughFreeSpaceTryAndReplaceALowerScoringItem(LatentCacheServer cache, RequestDescription request)
        {
            var score = cache.CalculateCachingScore(request);
            var caches = cache.Cache.VideoCache.OrderBy(x => x.CacheScore).ToList();

            bool isBetterScore = false;
            int i;
            for (i = 0; i < caches.Count; i++)
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

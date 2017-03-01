using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

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
                if (latentCache.Cache.VideoCache.ContainsKey(request.VideoID))
                {
                    latentCache.Cache.VideoCache[request.VideoID].CacheScore += latentCache.CalculateCachingScore(request);
                    isAlreadyInCache = true;
                }
            }
            if (!isAlreadyInCache)
            {
                AddToAnyCache(request);
            }
        }

        private void AddToAnyCache(RequestDescription request)
        {
            var isEnoughSpace = true;
            for (int i = 0; i < _latentCaches.Count; i++)
            {
                var latentCache = _latentCaches[i];
                var outcome = latentCache.AddRequestToCache(request, latentCache.CalculateCachingScore(request));

                if (outcome == AddToCacheResult.Added)
                {
                    return;
                }
                if (outcome == AddToCacheResult.NotEnoughFreeSpace)
                {
                    isEnoughSpace = false;
                }
            }

            //got to here ...  its not been added
            //is this a better pick than another video
            if (isEnoughSpace)
            {
                return;
            }

            for (int i = 0; i < _latentCaches.Count; i++)
            {
                var latentCache = _latentCaches[i];
                bool breakout = false;
                foreach(var cachedItem in latentCache.Cache.VideoCache.Values)
                {
                    if (cachedItem.Video.VideoSizeInMb > request.Video.VideoSizeInMb && latentCache.CalculateCachingScore(request) > cachedItem.CacheScore)
                    {
                        latentCache.Cache.VideoCache.Remove(cachedItem.VideoID); //modifying inside loop :(
                        latentCache.Cache.VideoCache.Add(request.VideoID, new CachedVideoRequest()
                        {
                            CacheScore = latentCache.CalculateCachingScore(request),
                            Video = request.Video,
                            VideoID =  request.VideoID
                        });

                        breakout = true;
                        break;
                    }
                }
                if (breakout)
                    break;
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

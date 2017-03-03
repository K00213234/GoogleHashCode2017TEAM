using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using GHC.VideoServer.Model;

namespace GHC.VideoServer.Strategies.InfinitePrune
{
    
    public class InfiniteCachePruneCostBasedStrategy
    {
        public Context Run(Context context)
        {
            FillCaches(context);
            Prune(context);
            //TODO ND: Prune
            return context;
        }                
        private void FillCaches(Context context)
        {
            foreach (var request in context.Requests)
            {
                var latentCaches = Utility.GetConnectedCachesInOrderOfLatency(context, request.Value.EndPoint).ToList();
                var cacheProxy = new InfiniteCacheProxy(latentCaches);
                cacheProxy.AddRequest(request.Value);
            }
        }

        private void Prune(Context context)
        {
            //foreach (var cache in context.CacheServers)
            //{
            //    PruneCachesToAverage(cache.Value);                
            //}

            foreach (var cacheServer in context.CacheServers)
            {
                PruneLowestScoringItems(cacheServer.Value);
            }
        }

        private void PruneLowestScoringItems(CacheServer cacheServer)
        {
            var cache = cacheServer.VideoCache.Values.OrderByDescending(v => v.CacheScore).ToList();
            var consumedSpace = cacheServer.ConsumedSpace();
            for (int i = cache.Count - 1; i >= 0; i--)
            {
                if (cacheServer.MaxMB < consumedSpace)
                {
                    consumedSpace -= cache[i].Video.VideoSizeInMb;
                    cacheServer.VideoCache.Remove(cache[i].VideoID);
                }

                if (consumedSpace <= cacheServer.MaxMB)
                {
                    return;
                }
            }
        }


        private void PruneCachesToAverage(CacheServer cacheServer, double recommendedScaling = 0)
        {
            var averageScore = CalculateAverageCachingScore(cacheServer);
            var totalSpace = cacheServer.MaxMB;
            var consumedSpace = cacheServer.ConsumedSpace();
            var scaling = (recommendedScaling <= 0.1 ?  0.1 :recommendedScaling);
            var pruneToPercentage =   averageScore * scaling; //prune the top scoring items
            bool somethingWasRemoved = false;
            if (totalSpace >= consumedSpace)
            {
                return;
            }

            var videosToRemove = new List<CachedVideoRequest>();

            foreach (var video in cacheServer.VideoCache.Values.OrderBy(v => v.CacheScore))
            {
                if(video.CacheScore <= pruneToPercentage)
                {
                    var videoToRemove = cacheServer.VideoCache[video.VideoID];
                    videosToRemove.Add(videoToRemove);
                    consumedSpace -= videoToRemove.Video.VideoSizeInMb;
                    if (totalSpace >= consumedSpace)
                    {
                        break;
                    }
                }   
            }

            foreach (var videoToRemove in videosToRemove)
            {
                cacheServer.VideoCache.Remove(videoToRemove.VideoID);
                somethingWasRemoved = true;
            }

            if (totalSpace < consumedSpace)
            {
                PruneCachesToAverage(cacheServer, (somethingWasRemoved) ? scaling : scaling + 0.1);
            }
        }    

        private double CalculateAverageCachingScore(CacheServer cache)
        {
            var sum = 0.0;

            foreach (var video in cache.VideoCache.Values)
            {
                sum += video.CacheScore;
            }

            return sum / cache.VideoCache.Count;
        }

    }
}

using System.Collections.Generic;
using System.Linq;
using GHC.VideoServer.Model;

namespace GHC.VideoServer.Strategies.InfinitePrune
{
    
    public class InfiniteCachePruneCostBasedStrategy
    {
        public Context Run(Context context)
        {
            FillCaches(context);
            Prune(context);            
            return context;
        }                
        private void FillCaches(Context context)
        {
            foreach (var request in context.Requests)
            {
                var latentCaches = Utility.GetConnectedCachesInOrderOfLatency(context, request.Value.EndPoint);
                if (latentCaches.Count == 0)
                {
                    continue;
                }
                var cacheProxy = new InfiniteCacheProxy(latentCaches);
                cacheProxy.AddRequest(request.Value);
            }
        }

        private void Prune(Context context)
        {
            foreach (var cacheServer in context.CacheServers)
            {
                PruneLowestScoringItems(cacheServer.Value, context);
            }
        }

        private void PruneLowestScoringItems(CacheServer cacheServer,Context context)
        {
            var cachedVideoRequests = cacheServer.VideoCache.Values.OrderBy(v => v.CacheScore).ToList();
            var consumedSpace = cacheServer.ConsumedSpace();
            var removedVideos = new List<CachedVideoRequest>();
                       
            for (int i = 0; i < cachedVideoRequests.Count; i++)
            {   
                if (cacheServer.MaxMB < consumedSpace)
                {
                    consumedSpace -= cachedVideoRequests[i].Video.VideoSizeInMb;
                    removedVideos.Add(cachedVideoRequests[i]);
                    cachedVideoRequests[i].OriginalRequest.IsCached = false;
                    cacheServer.VideoCache.Remove(cachedVideoRequests[i].VideoID);                   
                }

                if (consumedSpace <= cacheServer.MaxMB)
                {
                    break;
                }
            }
            
            //add back in as many high scoring requests as possible
            foreach (var removedVideo in removedVideos.OrderByDescending(r => r.CacheScore))
            {
                if (removedVideo.Video.VideoSizeInMb + consumedSpace > cacheServer.MaxMB) continue;
                consumedSpace += removedVideo.Video.VideoSizeInMb;
                cacheServer.VideoCache[removedVideo.VideoID] = removedVideo;
            }
        }   

    }
}

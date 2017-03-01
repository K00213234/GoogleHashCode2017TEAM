using System.Data.Odbc;
using GHC.VideoServer.Model;

namespace GHC.VideoServer.Strategies.CostBased
{
    public class PlugUnusedSpaceStrategy
    {
        public void Run(Context context)
        {            
           //chew up any space left
            foreach (var request in context.Requests)
            {
                var caches = context.EndPointToCacheServer[request.Value.EndPointID];

                foreach (var cache in caches)
                {
                    var cacheServer = context.CacheServers[cache.CacheServerID];

                    if (cacheServer.VideoCache.ContainsKey(request.Value.VideoID))
                    {
                        continue;
                    }
                    if (cacheServer.MaxMB - cacheServer.ConsumedSpace() > request.Value.Video.VideoSizeInMb)
                    { 
                        cacheServer.VideoCache.Add(request.Value.VideoID,  new CachedVideoRequest()
                        {
                            VideoID = request.Value.VideoID,
                            Video =  request.Value.Video
                        });
                    }
                }   
            }                          
        } 
    }
}


//var caches = context.CacheServers.OrderBy(c => c.Value.CalculateCacheScore()).ToArray();
//var cacheCount = caches.Length;

//            for (int counter = 0; counter<cacheCount; counter++)
//            {
//                var cache = context.CacheServers[counter];

//                foreach (var request in context.Requests)
//                {
//                    if (cache.VideoCache.ContainsKey(request.Value.VideoID))
//                    {
//                        continue;
//                    }

//                    var endPointToCacheServerConnection = context.EndPointToCacheServer[request.Value.EndPointID].FirstOrDefault(x => x.CacheServerID == cache.ID);

//                    if (endPointToCacheServerConnection == null)
//                    {
//                        continue; // no connections from the endpoint to the cache
//                    }

//                    var latentCache = new LatentCacheServer(cache, endPointToCacheServerConnection.LatencyInMilliSecondsFromCacheToEndpoint);
//var requestScore = latentCache.CalculateCachingScore(request.Value);
//var replacements = latentCache.Cache.VideoCache.Values.Where(p => p.CacheScore <= requestScore && p.Video.VideoSizeInMb > request.Value.Video.VideoSizeInMb).OrderBy(h => h.CacheScore);

//                    if (replacements.Any())
//                    {
//                        var videoToReplace = replacements.First();
//cache.VideoCache.Remove(videoToReplace.VideoID);
//                        cache.VideoCache.Add(request.Value.VideoID, new CachedVideoRequest
//                        {
//                            CacheScore = requestScore,
//                            VideoID = request.Value.VideoID,
//                            Video = request.Value.Video
//                        });
//                    }
//                }
//            }                 
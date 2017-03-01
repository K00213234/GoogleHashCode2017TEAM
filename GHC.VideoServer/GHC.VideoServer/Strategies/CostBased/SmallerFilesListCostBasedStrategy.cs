using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHC.VideoServer.Model;

namespace GHC.VideoServer.Strategies.CostBased
{
    public class SmallerFilesListCostBasedStrategy
    {
        public void Run(Context context)
        {            
            var caches = context.CacheServers.OrderBy(c => c.Value.CalculateCacheScore()).ToArray();
            var cacheCount = caches.Length;

            for (int counter = 0; counter < cacheCount; counter++)
            {
                var lowestScoringCache = context.CacheServers[counter];

                //var connections = context.CacheServerToEndPoint[lowestScoringCache.ID];

                var smallestRequestsFirst = context.Requests.Where(r => r.Value.EndPoint.Connections.Any(c => c.CacheServerID == lowestScoringCache.ID)).OrderBy(r => r.Value.Video.VideoSizeInMb).ToArray();

                foreach (var request in smallestRequestsFirst)
                {
                    if (lowestScoringCache.VideoCache.ContainsKey(request.Value.VideoID))
                    {
                        continue;
                    }
                   
                    //var c = request.Value.EndPoint.Connections.Where(t => t.CacheServerID == lowestScoringCache.ID).First();


                    var c = context.EndPointToCacheServer[request.Value.EndPointID].First( x => x.CacheServerID == lowestScoringCache.ID);
                    //var d = context.CacheServerToEndPoint[lowestScoringCache.ID];
                    //var d = context.CacheServerToEndPoint[lowestScoringCache.ID].Values.First();
                    //var e = context.EndPointToCacheServer[lowestScoringCache.ID];

                    var latentCache = new LatentCacheServer(lowestScoringCache, c.LatencyInMilliSecondsFromCacheToEndpoint);
                    var requestScore = latentCache.CalculateCachingScore(request.Value);
                    var replacements = latentCache.Cache.VideoCache.Values.Where(p => p.CacheScore <= requestScore && p.Video.VideoSizeInMb > request.Value.Video.VideoSizeInMb).OrderBy(h => h.CacheScore);

                    if (replacements.Any())
                    {
                        var videoToReplace = replacements.First();
                        lowestScoringCache.VideoCache.Remove(videoToReplace.VideoID);
                        lowestScoringCache.VideoCache.Add(request.Value.VideoID, new CachedVideoRequest
                        {
                            CacheScore = requestScore,
                            VideoID = request.Value.VideoID,
                            Video = request.Value.Video
                        });
                    }
                }
            }                                               
        } 
    }
}

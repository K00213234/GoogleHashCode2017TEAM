using System.Data.Odbc;
using System.Linq;
using System.Net.Http.Headers;
using GHC.VideoServer.Model;

namespace GHC.VideoServer.Strategies.CostBased
{
    public class PlugUnusedSpaceStrategy
    {
        public void Run(Context context)
        {            
           //chew up any space left
            foreach (var request in context.Requests.OrderBy(r => r.Value.Video.VideoSizeInMb)) // smallest gets us the most videos in, probably not a great choice, its all trade offs
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
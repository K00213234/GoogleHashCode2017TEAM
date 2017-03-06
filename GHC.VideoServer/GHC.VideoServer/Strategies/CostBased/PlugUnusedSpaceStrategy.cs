using GHC.VideoServer.Model;

namespace GHC.VideoServer.Strategies.CostBased
{
    public class PlugUnusedSpaceStrategy
    {
        public void Run(Context context)
        {            
              ChewUpSpace(context);
        }

        private void ChewUpSpace(Context context)
        {
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
                        cacheServer.VideoCache.Add(request.Value.VideoID, new CachedVideoRequest()
                        {
                            VideoID = request.Value.VideoID,
                            Video = request.Value.Video
                        });
                    }
                }
            }
        } 
    }
}            
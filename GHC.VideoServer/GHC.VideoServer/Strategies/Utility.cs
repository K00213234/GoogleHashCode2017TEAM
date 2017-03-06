using System.Collections.Generic;
using GHC.VideoServer.Model;

namespace GHC.VideoServer.Strategies
{
    public class Utility
    {
        public static List<LatentCacheServer> GetConnectedCachesInOrderOfLatency(Context context, EndPoint endpoint)
        {
            var result = new List<LatentCacheServer>();
            if (!context.EndPointToCacheServer.ContainsKey(endpoint.EndPointID))
                return new List<LatentCacheServer>();
            var connections = context.EndPointToCacheServer[endpoint.EndPointID];

            foreach (var connection in connections)
            {
                if (!context.CacheServers.ContainsKey(connection.CacheServerID)) continue;
                var cache = context.CacheServers[connection.CacheServerID];
                result.Add(new LatentCacheServer(cache, connection.LatencyInMilliSecondsFromCacheToEndpoint));
            }

            return result;
        }     
    }
}

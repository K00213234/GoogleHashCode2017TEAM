using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHC.VideoServer.Model;

namespace GHC.VideoServer.Strategies
{
    public class Utility
    {
        public static IEnumerable<LatentCacheServer> GetConnectedCachesInOrderOfLatency(Context context, EndPoint endpoint)
        {
            List<LatentCacheServer> result = new List<LatentCacheServer>();

            var connections = context.EndPointToCacheServer[endpoint.EndPointID];

            foreach (var connection in connections)
            {
                var cache = context.CacheServers[connection.CacheServerID];//.First(x => x.Value.ID == connection.CacheServerID);
                result.Add(new LatentCacheServer(cache, connection.LatencyInMilliSecondsFromCacheToEndpoint));
            }

            return result;//.OrderBy(x => x.LatencyInMilliSeconds);
        }

        public static CacheServer GetLowestScoringCacheServer(Context context)
        {
            int lowestScore = (int) context.CacheServers[0].CalculateCacheScore();
            CacheServer lowestScoringCache = context.CacheServers[0];

            foreach (var cache in context.CacheServers)
            {
                var currentScore = (int)cache.Value.CalculateCacheScore();
                if (currentScore < lowestScore)
                {
                    lowestScore = currentScore;
                    lowestScoringCache = cache.Value;
                }
            }
            return lowestScoringCache;
        }
    }
}

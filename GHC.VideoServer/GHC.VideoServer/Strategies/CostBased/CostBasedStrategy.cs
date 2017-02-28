using GHC.VideoServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GHC.VideoServer.Strategies.CostBased
{
    
    public class CostBasedStrategy
    {
        Context _context;

        public Context Run(Context context)
        {
            FillCaches(context);
            UseAnyEmptySpace(context);
            //UseAnyEmptySpace(context);
            //UseAnyEmptySpace(context);
            return context;
        }

        private void UseAnyEmptySpace(Context context)
        {
            foreach (var request in context.Requests)
            {
                var latentCaches = GetConnectedCachesInOrderOfLatency(context, request.EndPoint).ToList();
                var cacheProxy = new CacheProxy(latentCaches);
                cacheProxy.AddRequest(request);
            }
        }

        private void FillCaches(Context context)
        {
            foreach (var request in context.Requests)
            {
                var latentCaches = GetConnectedCachesInOrderOfLatency(context, request.EndPoint).ToList();
                var cacheProxy = new CacheProxy(latentCaches);
                cacheProxy.AddRequest(request);
            }
        }
        private IEnumerable<LatentCacheServer> GetConnectedCachesInOrderOfLatency(Context context, EndPoint endpoint)
        {
            List<LatentCacheServer> result = new List<LatentCacheServer>();
            
            foreach(var connection in endpoint.Connections)
            {
                var cache = context.CacheServers.First(x => x.ID == connection.CacheServerID);
                result.Add(new LatentCacheServer(cache, connection.LatencyInMilliSecondsFromCacheToEndpoint));
            }

            return result.OrderBy(x => x.LatencyInMilliSeconds);
        }
        private void CalculateDuplicateEntries(Context context)
        {
             //[video][endpoint]    
            int [][] entries = new int[context.Videos.Count() ][];
            var size = context.Requests.Count();
            for(int i = 0; i < entries.Length; i++)
            {
                entries[i] = new int[size];
            }

            foreach (var request in context.Requests)
            {
                entries[request.VideoID][request.EndPointID]++;
                if(entries[request.VideoID][request.EndPointID] > 1)
                {
                    Console.WriteLine($"duplicate {entries[request.VideoID][request.EndPointID]}: {request.VideoID} - {request.EndPointID} - {request.NumberOfReqeusts}");
                }
            }
        }
    }
}

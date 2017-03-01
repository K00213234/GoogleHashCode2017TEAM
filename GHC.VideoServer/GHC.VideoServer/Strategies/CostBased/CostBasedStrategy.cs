using GHC.VideoServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GHC.VideoServer.Strategies.CostBased
{
    
    public class CostBasedStrategy
    {
        public Context Run(Context context)
        {
            FillCaches(context);
            //CalculateAverageScore(context);


            //UseAnyEmptySpace(context);
            //PruneInfiniteCache(context);
            //UseAnyEmptySpace(context);
            //UseAnyEmptySpace(context);
            return context;
        }

        //private int CalculateAverageScore(Context context)
        //{
        //    int counter = 1;
        //    int sum = 0;
        //    foreach (var cache in context.CacheServers)
        //    {
        //        counter++;
        //        sum += (int) cache.Value.CalculateCacheScore();
        //    }
        //    Console.WriteLine($"Average Score: {sum/counter}");
        //    return sum / counter;
        //} 
                
        private void FillCaches(Context context)
        {
            foreach (var request in context.Requests)
            {
                var latentCaches = Utility.GetConnectedCachesInOrderOfLatency(context, request.Value.EndPoint).ToList();
                var cacheProxy = new CacheProxy(latentCaches);
                cacheProxy.AddRequest(request.Value);
            }
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
                entries[request.Value.VideoID][request.Value.EndPointID]++;
                if(entries[request.Value.VideoID][request.Value.EndPointID] > 1)
                {
                    Console.WriteLine($"duplicate {entries[request.Value.VideoID][request.Value.EndPointID]}: {request.Value.VideoID} - {request.Value.EndPointID} - {request.Value.NumberOfReqeusts}");
                }
            }
        }
    }
}

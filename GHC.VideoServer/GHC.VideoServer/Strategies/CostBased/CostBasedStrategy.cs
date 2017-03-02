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
            return context;
        }                
        private void FillCaches(Context context)
        {
            foreach (var request in context.Requests)
            {
                var latentCaches = Utility.GetConnectedCachesInOrderOfLatency(context, request.Value.EndPoint).ToList();                
                var cacheProxy = new CacheProxy(latentCaches);
                cacheProxy.AddRequest(request.Value);
            }
        }      
    }
}

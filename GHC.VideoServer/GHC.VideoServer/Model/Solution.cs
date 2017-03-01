using System;
using System.Linq;

namespace GHC.VideoServer.Model
{
    public class Solution
    {
        public Context Context { get; set; }
        
        public override string ToString()
        {
            string output = Context.CacheServers.Count + Environment.NewLine;
            foreach(var item in Context.CacheServers)
            {
                output += item.Value.ID;

                foreach (var video in item.Value.VideoCache.Values.OrderBy(x => x.VideoID))
                {
                    output += " " + video.VideoID;
                }

                output += Environment.NewLine;
            }
            return output;
        }
    }
}
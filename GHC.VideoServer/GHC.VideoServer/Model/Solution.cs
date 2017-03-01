using System;

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

                foreach (var video in item.Value.VideoCache.Values)
                {
                    output += " " + video.VideoID;
                }

                output += Environment.NewLine;
            }
            return output;
        }

    }
}
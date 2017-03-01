using System;

namespace GHC.VideoServer.Model
{
    public class Solution
    {
        public Context context { get; set; }


        public override String ToString()
        {
            String output = this.context.CacheServers.Count + Environment.NewLine;
            foreach(CacheServer item in this.context.CacheServers)
            {
                output += item.ID;
                for(int index = 0; index < item.VideoCache.Count; index++)
                {
                    output += " " + item.VideoCache[index].VideoID;
                }
                output += Environment.NewLine;
            }
            return output;
        }

    }
}
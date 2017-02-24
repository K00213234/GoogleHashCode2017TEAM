using System;

namespace GHC.VideoServer.Model
{
    public class Solution
    {
        public Context context { get; set; }


        public override String ToString()
        {
            String output = this.context.CacheServerList.Count + Environment.NewLine;
            foreach(CacheServer item in this.context.CacheServerList)
            {
                output += item.ID;
                for(int index = 0; index < item.VideoList.Count; index++)
                {
                    output += " " + item.VideoList[index].VideoID;
                }
                output += Environment.NewLine;
            }
            return output;
        }

    }
}
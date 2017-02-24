using System.Collections.Generic;

namespace GHC.VideoServer.Model
{
    public class CacheServer
    {
        public int ID { get; set; }
        public List<VideoRequest> VideoList = new List<VideoRequest>();
        public int MaxMB { get; set; }

        public int ConsumedSpace()
        {
            int sum = 0;
            foreach(var video in VideoList)
            {
                sum += video.Video.VideoSizeInMB;
            }

            return sum;
        }
    }
}
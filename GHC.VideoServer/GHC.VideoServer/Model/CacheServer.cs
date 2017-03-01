using System.Collections.Generic;

namespace GHC.VideoServer.Model
{
    public class CacheServer
    {
        public int ID { get; set; }
        public Dictionary<int, CachedVideoRequest> VideoCache = new Dictionary<int, CachedVideoRequest>();
        public int MaxMB { get; set; }

        public int ConsumedSpace()
        {
            int sum = 0;
            foreach(var video in VideoCache)
            {
                sum += video.Value.Video.VideoSizeInMb;
            }

            return sum;
        }

        public double CalculateCacheScore()
        {
            var result = 0.0;
            foreach(var cachedItem in VideoCache)
            {
                result += cachedItem.Value.CacheScore;
            }
            return result;
        }
    }
}
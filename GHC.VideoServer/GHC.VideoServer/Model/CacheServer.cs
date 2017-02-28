using System.Collections.Generic;

namespace GHC.VideoServer.Model
{
    public class CacheServer
    {
        public int ID { get; set; }
        public List<CachedVideoRequest> Cache = new List<CachedVideoRequest>();
        public int MaxMB { get; set; }

        public int ConsumedSpace()
        {
            int sum = 0;
            foreach(var video in Cache)
            {
                sum += video.Video.VideoSizeInMb;
            }

            return sum;
        }

        public double CalculateCacheScore()
        {
            var result = 0.0;
            foreach(var cachedItem in Cache)
            {
                result += cachedItem.CacheScore;
            }
            return result;
        }
    }
}
namespace GHC.VideoServer.Model
{
    public class CachedVideoRequest
    {
        public int VideoID { get; set; }
        public Video Video { get; set; }

        public double CacheScore { get; set;}
        public int RequestTotal { get; set; }

        public RequestDescription OriginalRequest { get; set; }
    }
}
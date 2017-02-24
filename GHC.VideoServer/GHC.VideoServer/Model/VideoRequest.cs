namespace GHC.VideoServer.Model
{
    public class VideoRequest
    {
        public int VideoID { get; set; }
        public Video Video { get; set; }
        public int RequestTotal { get; set; }
    }
}
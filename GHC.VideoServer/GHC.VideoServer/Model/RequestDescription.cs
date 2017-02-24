namespace GHC.VideoServer.Model
{
    public class RequestDescription
    {
        public int VideoID { get; set; }

        public Video Video { get; set; }

        public int EndPointID { get; set; }

        public int NumberOfReqeusts { get; set; }

        public EndPoint EndPoint { get; set; }
    }
}
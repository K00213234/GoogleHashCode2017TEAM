using System.Collections.Generic;

namespace GHC.VideoServer.Model
{
	public class Context
	{
		public List<Video> Videos { get; set; }

        public List<EndPoint> EndPointList { get; set; }

        public List<RequestDescription> RequestDescriptionList { get; set; }

        public FileDescriptor FileDescriptor { get; set; }

        public List<CacheServer> CacheServerList { get; set; }
	}
}
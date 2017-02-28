using System.Collections.Generic;

namespace GHC.VideoServer.Model
{
	public class Context
	{
		public List<Video> Videos { get; set; }

        public List<EndPoint> EndPoints { get; set; }

        public List<RequestDescription> Requests { get; set; }

        public FileDescriptor FileDescriptor { get; set; }

        public List<CacheServer> CacheServers { get; set; }
	}
}
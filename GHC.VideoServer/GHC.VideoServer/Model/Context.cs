using System.Collections.Generic;

namespace GHC.VideoServer.Model
{
	public class Context
	{
		public List<Video> Videos { get; set; }

        public Dictionary<int, EndPoint> EndPoints { get; set; }

        /// <summary>
        /// key is EndPointID
        /// </summary>
        public Dictionary<int, List<EndPointToCacheServerConnection>> CacheServerToEndPoint { get; set; }

        /// <summary>
        /// key is CacheServerID  -> endpointID, cacheinfo
        /// </summary>
        public Dictionary<int, List<EndPointToCacheServerConnection>> EndPointToCacheServer  { get; set; }

        public Dictionary<int, RequestDescription> Requests { get; set; }

        public FileDescriptor FileDescriptor { get; set; }

        public Dictionary<int, CacheServer> CacheServers { get; set; }

        public Dictionary<int, CacheServer> UnOptimizedCacheServers { get; set; }

        public Dictionary<int, RequestDescription> RequestsToRedistribute { get; set; }
        
	}
}
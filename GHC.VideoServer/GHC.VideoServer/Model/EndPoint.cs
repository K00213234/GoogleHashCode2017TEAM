using System.Collections.Generic;

namespace GHC.VideoServer.Model
{
    public class EndPoint
    {
        public int EndPointID { get; set; }
        public int LatencyInMiliSecondsFromDataCenter { get; set; }
        public int NumberOfConnectedCacheServers { get; set; }

        public int Hit { get; set; }
        public int Miss { get; set; }

        public List<EndPointToCacheServerConnection> Connections { get { return this.connections; } }
        public List<EndPointToCacheServerConnection> connections = new List<EndPointToCacheServerConnection>();
    }
}
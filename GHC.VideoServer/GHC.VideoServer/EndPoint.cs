using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GHC.VideoServer
{
    public class EndPoint
    {
        public int EndPointID { get; set; }
        public int LatencyInMiliSecondsFromDataCenter { get; set; }
        public int NumberOfConnectedCacheServers { get; set; }

        public List<EndPointToCacheServerConnection> Connections { get { return this.connections; } }
        public List<EndPointToCacheServerConnection> connections = new List<EndPointToCacheServerConnection>();

    }
}
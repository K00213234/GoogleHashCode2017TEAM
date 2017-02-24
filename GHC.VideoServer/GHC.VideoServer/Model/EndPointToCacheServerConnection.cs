using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GHC.VideoServer.Model
{
    public class EndPointToCacheServerConnection
    {
        public int CacheServerID { get; set; }
        public int LatencyInMilliSecondsFromCacheToEndpoint { get; set; }
        public EndPoint EndPoint { get; set; }        
    }
}
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GHC.VideoServer.Model
{
    public class FileDescriptor
    {
        public int VideoCount { get; set; }
        public int EndpointCount { get; set; }
        public int RequestDescriptorCount { get; set; }
        public int CacheServersCount { get; set; }
        public int CacheServersCapacityMB { get; set; }
    }
}
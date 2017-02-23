using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GHC.VideoServer
{
    public class RequestDescription
    {
        //public int 
        public int RequestDescriptionID { get; set; }
        public int RequestEndPointIDOrigin { get; set; }

        public int NumberOfRequests { get; set; }
    }
}
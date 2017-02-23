using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GHC.VideoServer
{
    public class RequestDescription
    {
        public int VideoID { get; set; }
        
        public int EndPointID { get; set; }

        public int NumberOfReqeusts { get; set; }        
    }
}
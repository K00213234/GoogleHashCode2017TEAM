using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHC.VideoServer.Model
{
    public enum AddToCacheResult
    {
        Added,
        NotAdded,
        NotEnoughFreeSpace,
        AlreadyInCache
    }
}

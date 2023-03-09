using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace testing.Models
{
    [Keyless]
    public class cCounters
    {

        public int numClients { get; set; }
        public int impData { get; set; }
        public int expData { get; set; }
        public int extClients { get; set; }

        public cCounters()
        { }
    }
}

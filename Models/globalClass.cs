using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace testing.Models
{
    [Keyless]
    public class globalClass
    {
        public bool isPass { get; set; }

        public globalClass()
        {
            isPass = false;
        }
    }
}

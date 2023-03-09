using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using testing.Models;

namespace testing.Data
{
    public class testingContext : DbContext
    {
        public testingContext (DbContextOptions<testingContext> options)
            : base(options)
        {
        }

        public DbSet<testing.Models.Contacts> Contacts { get; set; }
        public DbSet<testing.Models.cCounters> cCounters { get; set; }
        public DbSet<testing.Models.cCounters> UsersLog { get; set; }
        public DbSet<testing.Models.UsersLog> UsersLog_1 { get; set; }

        public DbSet<testing.Models.exportData> exportData { get; set; }

        public DbSet<testing.Models.globalClass> globalClass { get; set; }
    }
}

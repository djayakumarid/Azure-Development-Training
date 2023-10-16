using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jd1_webapp_sql.Models;

namespace jd1_webapp_sql.Data
{
    public class jd1_webapp_sqlContext : DbContext
    {
        public jd1_webapp_sqlContext (DbContextOptions<jd1_webapp_sqlContext> options)
            : base(options)
        {
        }

        public DbSet<jd1_webapp_sql.Models.Employee> Employee { get; set; } = default!;
    }
}

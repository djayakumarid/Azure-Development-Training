using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.SharedLibrary;

namespace EmployeeManagement.SharedLibrary
{
    public class EmployeeDataContext : DbContext
    {
        public EmployeeDataContext (DbContextOptions<EmployeeDataContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeManagement.SharedLibrary.Employee> Employees { get; set; } = default!;
    }
}

using Microsoft.EntityFrameworkCore;
using VerlofApp.Models;

namespace VerlofApp.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
    }
}

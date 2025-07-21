using Microsoft.EntityFrameworkCore;

namespace Timer.API.Models
{
    public class TimerDbContext : DbContext
    {
        public TimerDbContext(DbContextOptions<TimerDbContext> options) : base(options) { }
        
        public DbSet<Timer> Timers { get; set; }
    }
}

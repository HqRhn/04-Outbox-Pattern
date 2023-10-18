using Microsoft.EntityFrameworkCore;

namespace OutboxPattern
{
    public class OrderRepository : DbContext
    {
        public virtual DbSet<Order> Orders { get; protected set; }

        public virtual DbSet<OutboxMessage> OutboxMessages { get; protected set; }
        public OrderRepository(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);          
        }
    }
}

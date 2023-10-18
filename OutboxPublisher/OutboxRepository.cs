using Microsoft.EntityFrameworkCore;

namespace OutboxPublisher
{
    public class OutboxRepository : DbContext
    {
        public virtual DbSet<OutboxMessage> OutboxMessages { get; protected set; }
        public OutboxRepository(DbContextOptions options) : base(options)
        {

        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace MultiCarrier.Database;

public class MultiCarrierContext : DbContext
{
    public MultiCarrierContext(
        DbContextOptions<MultiCarrierContext> options) : base(options)
    {
    }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)  
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.ApplyConfiguration(new RecordConfiguration());
    }
}
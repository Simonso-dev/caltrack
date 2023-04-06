using Microsoft.EntityFrameworkCore;

namespace caltrack.Data;

public class CaltrackContext : DbContext {
    public CaltrackContext(DbContextOptions options) : base(options) {
    }
    public DbSet<CaloriesModel> Calories { get; set;}
    public DbSet<User> Users { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // one to many relationship between user and calories
        modelBuilder.Entity<CaloriesModel>()
            .HasOne(c => c.User)
            .WithMany(u => u.CaloriesList)
            .HasForeignKey(c => c.UserId);

    }
}
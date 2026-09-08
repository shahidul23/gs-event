using System;
using GSEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace GSEvent.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
    { }
    public DbSet<Category> Categories {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasKey(c => c.CategoryId);
        base.OnModelCreating(modelBuilder);
    }

    
}

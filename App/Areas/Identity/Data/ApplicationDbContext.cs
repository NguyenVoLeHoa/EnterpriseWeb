using App.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using App.Web.Entities;

namespace App.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.

        builder.Entity<User>().ToTable("User");
        builder.Entity<IdentityRole>().ToTable("Role");
        //TODO: Create relationship for department with another table



        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
    }

    internal Task CreateAsync(Department newDepartment, string departmentName)
    {
        throw new NotImplementedException();
    }

    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Firstname).HasMaxLength(255);
            builder.Property(u => u.Lastname).HasMaxLength(255);
        }
    }


    public DbSet<Event>? Events { get; set; }
    public DbSet<Post>? Posts { get; set; }

    public DbSet<Department>? Departments { get; set; }
    public DbSet<UserActionLog>? UserActionLogs { get; set; }
    public DbSet<User>? Users { get; set; }

    public DbSet<Comment>? Comments { get; set; }

    public DbSet<App.Entities.Category>? Category { get; set; }

    //TODO: Add-Migration for department

}

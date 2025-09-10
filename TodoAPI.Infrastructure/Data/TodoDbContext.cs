// TodoAPI.Infrastructure/Data/TodoDbContext.cs
using Microsoft.EntityFrameworkCore;
using TodoAPI.Core.Entities;

namespace TodoAPI.Infrastructure.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TaskItem Configuration
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(t => t.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(t => t.Status)
                    .HasConversion<string>();

                entity.Property(t => t.Priority)
                    .HasConversion<string>();

                entity.Property(t => t.CreatedAt)
                    .IsRequired();

                entity.Property(t => t.UpdatedAt)
                    .IsRequired();

                // Relationships
                entity.HasOne(t => t.User)
                    .WithMany(u => u.Tasks)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Category)
                    .WithMany(c => c.Tasks)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Indexes for performance
                entity.HasIndex(t => t.UserId);
                entity.HasIndex(t => t.CategoryId);
                entity.HasIndex(t => t.Status);
                entity.HasIndex(t => t.Priority);
                entity.HasIndex(t => t.CreatedAt);
            });

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(254);

                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(u => u.CreatedAt)
                    .IsRequired();

                entity.Property(u => u.UpdatedAt)
                    .IsRequired();

                // Unique email constraint
                entity.HasIndex(u => u.Email)
                    .IsUnique();
            });

            // Category Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                entity.Property(c => c.CreatedAt)
                    .IsRequired();

                // Unique name constraint
                entity.HasIndex(c => c.Name)
                    .IsUnique();
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            var personalCategoryId = new Guid("11111111-1111-1111-1111-111111111111");
            var workCategoryId = new Guid("22222222-2222-2222-2222-222222222222");

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = personalCategoryId,
                    Name = "Personal",
                    Description = "Personal tasks and activities",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Category
                {
                    Id = workCategoryId,
                    Name = "Work",
                    Description = "Work-related tasks and projects",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is TaskItem taskItem)
                {
                    if (entry.State == EntityState.Added)
                    {
                        taskItem.CreatedAt = DateTime.UtcNow;
                    }
                    taskItem.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is User user)
                {
                    if (entry.State == EntityState.Added)
                    {
                        user.CreatedAt = DateTime.UtcNow;
                    }
                    user.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is Category category && entry.State == EntityState.Added)
                {
                    category.CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
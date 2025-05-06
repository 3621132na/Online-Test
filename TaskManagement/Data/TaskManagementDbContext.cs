using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Data;

public partial class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<task> tasks { get; set; }

    public virtual DbSet<user> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<task>(entity =>
        {
            entity.HasKey(e => e.id).HasName("tasks_pkey");

            entity.HasIndex(e => e.duedate, "idx_tasks_duedate");

            entity.HasIndex(e => e.status, "idx_tasks_status");

            entity.HasIndex(e => e.userid, "idx_tasks_userid");
            entity.Property(e => e.duedate)
                .HasColumnType("date");
            entity.Property(e => e.createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.status).HasMaxLength(20);
            entity.Property(e => e.title).HasMaxLength(100);
            entity.Property(e => e.updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.user).WithMany(p => p.tasks)
                .HasForeignKey(d => d.userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tasks_userid_fkey");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.HasIndex(e => e.email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.username, "users_username_key").IsUnique();

            entity.Property(e => e.createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.passwordhash).HasMaxLength(255);
            entity.Property(e => e.username).HasMaxLength(50);
            entity.Property(e => e.role)
                .HasMaxLength(20)
                .HasDefaultValue("Regular")
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

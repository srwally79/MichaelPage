using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Log> Logs => Set<Log>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(user => user.Id).HasName("PK_Usuario");
            entity.Property(user => user.Id).HasColumnName("Usuario_Id").HasDefaultValueSql("NEWID()");
            entity.Property(user => user.Name).HasColumnName("Usuario_Nombre").HasMaxLength(255).IsRequired();
            entity.Property(user => user.Email).HasColumnName("Usuario_Email").HasMaxLength(255).IsRequired();
            entity.Property(user => user.PasswordHash).HasColumnName("Usuario_Password_Hash").HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Estado");
            entity.HasKey(status => status.Id).HasName("PK_Estado");
            entity.Property(status => status.Id).HasColumnName("Estado_Id").HasDefaultValueSql("NEWID()");
            entity.Property(status => status.Name).HasColumnName("Estado_Nombre").HasMaxLength(30).IsRequired();
            entity.HasIndex(status => status.Name).IsUnique().HasDatabaseName("UQ_Estado_Nombre");
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tarea");
            entity.HasKey(task => task.Id).HasName("PK_Tarea");
            entity.Property(task => task.Id).HasColumnName("Tarea_Id").HasDefaultValueSql("NEWID()");
            entity.Property(task => task.Title).HasColumnName("Tarea_Titulo").HasMaxLength(255).IsRequired();
            entity.Property(task => task.StatusId).HasColumnName("Tarea_Estado_Id").IsRequired();
            entity.Property(task => task.UserId).HasColumnName("Tarea_Usuario_id").IsRequired();
            entity.Property(task => task.AdditionalInfo).HasColumnName("Tarea_Informacion").HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(task => task.CreatedAt).HasColumnName("Tarea_Fecha_Creacion").HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            entity.Property(task => task.StartedAt).HasColumnName("Tarea_Fecha_Inicio").HasColumnType("datetime");
            entity.Property(task => task.FinishedAt).HasColumnName("Tarea_Fecha_Fin").HasColumnType("datetime");
            entity.HasOne(task => task.Status).WithMany(status => status.Tasks).HasForeignKey(task => task.StatusId).HasConstraintName("FK_Tarea_Estado");
            entity.HasOne(task => task.User).WithMany(user => user.Tasks).HasForeignKey(task => task.UserId).HasConstraintName("FK_Tarea_Usuario");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log");
            entity.HasKey(log => log.Id).HasName("PK_Log");
            entity.Property(log => log.Id).HasColumnName("Log_Id").HasDefaultValueSql("NEWID()");
            entity.Property(log => log.TaskId).HasColumnName("Log_Tarea_id").IsRequired();
            entity.Property(log => log.CreatedAt).HasColumnName("Log_Fecha").HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            entity.Property(log => log.Description).HasColumnName("Log_Descripcion").HasColumnType("varchar(max)");
            entity.HasOne(log => log.Task).WithMany(task => task.Logs).HasForeignKey(log => log.TaskId).HasConstraintName("FK_Log_Tarea");
        });
    }
}

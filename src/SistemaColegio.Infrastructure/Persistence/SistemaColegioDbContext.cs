using Microsoft.EntityFrameworkCore;
using SistemaColegio.Domain.Entities;
using SistemaColegio.Domain.Entities.Identity;

namespace SistemaColegio.Infrastructure.Persistence
{
    public class SistemaColegioDbContext : DbContext
    {
        public SistemaColegioDbContext(DbContextOptions<SistemaColegioDbContext> options) : base(options)
        {
        }
        
        public DbSet<Student> Estudiantes { get; set; }
        public DbSet<Professor> Profesores { get; set; }
        public DbSet<Subject> Materias { get; set; }
        public DbSet<Registration> Registros { get; set; }
        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuración de entidades
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Estudiantes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StudentId).IsRequired().HasMaxLength(20);
                entity.Ignore(e => e.Nombre);
                
                entity.HasMany(e => e.Registrations)
                      .WithOne(r => r.Estudiante)
                      .HasForeignKey(r => r.EstudianteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<Professor>(entity =>
            {
                entity.ToTable("Profesores");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Specialty).IsRequired().HasMaxLength(100);
                entity.Ignore(e => e.Department);
                entity.Ignore(e => e.Nombre);
            });
            
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Materias");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Credits).IsRequired();
                
                entity.HasOne(s => s.Professor)
                      .WithMany(p => p.Subjects)
                      .HasForeignKey(s => s.ProfessorId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
            
            modelBuilder.Entity<Registration>(entity =>
            {
                entity.ToTable("Registros");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaRegistro).IsRequired();
                
                // Ignore the compatibility properties
                entity.Ignore(e => e.StudentId);
                entity.Ignore(e => e.SubjectId);
                entity.Ignore(e => e.RegistrationDate);
                
                entity.HasOne(r => r.Estudiante)
                      .WithMany(s => s.Registrations)
                      .HasForeignKey(r => r.EstudianteId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(r => r.Materia)
                      .WithMany()
                      .HasForeignKey(r => r.MateriaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CreatedAt).IsRequired();
                
                entity.HasOne(u => u.Student)
                      .WithOne()
                      .HasForeignKey<User>(u => u.StudentId)
                      .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(u => u.Professor)
                      .WithOne()
                      .HasForeignKey<User>(u => u.ProfessorId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
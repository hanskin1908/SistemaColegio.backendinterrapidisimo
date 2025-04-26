using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace SistemaColegio.Infrastructure.Persistence.DesignTime
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SistemaColegioDbContext>
    {
        public SistemaColegioDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine("Creating DbContext...");
            
            // Usar una cadena de conexión hardcodeada para el tiempo de diseño
            var connectionString = "Data Source=HANS\\HANSPC;Initial Catalog=sistemaNotas;TrustServerCertificate=True;User ID=sa;Password=mariana0505;";

            Console.WriteLine($"Using connection string: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<SistemaColegioDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new SistemaColegioDbContext(optionsBuilder.Options);
        }
    }
}
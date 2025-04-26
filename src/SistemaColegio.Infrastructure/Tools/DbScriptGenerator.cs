using Microsoft.EntityFrameworkCore;
using SistemaColegio.Infrastructure.Persistence;
using SistemaColegio.Infrastructure.Persistence.DesignTime;
using System;
using System.IO;

namespace SistemaColegio.Infrastructure.Tools
{
    public class DbScriptGenerator
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Generating database script...");

            try
            {
                // Crear una instancia del contexto de base de datos
                var factory = new DesignTimeDbContextFactory();
                using var context = factory.CreateDbContext(args);

                // Generar el script SQL
                var script = context.Database.GenerateCreateScript();

                // Guardar el script en un archivo
                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "database_script.sql");
                File.WriteAllText(scriptPath, script);

                Console.WriteLine($"Script generated successfully at: {scriptPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating script: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
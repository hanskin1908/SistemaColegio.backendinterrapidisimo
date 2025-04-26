using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaColegio.Domain.Entities;
using SistemaColegio.Domain.Interfaces;

namespace SistemaColegio.Infrastructure.Persistence
{
    public class EstudianteRepositorio : RepositorioBase<Student>, IEstudianteRepositorio
    {
        public EstudianteRepositorio(SistemaColegioDbContext contexto) : base(contexto)
        {
        }
        
        public async Task<IReadOnlyList<Student>> ObtenerEstudiantesConRegistrosAsync()
        {
            return await _contexto.Estudiantes
                .Include(e => e.Registrations)
                    .ThenInclude(r => r.Materia)
                .ToListAsync();
        }
        
        public async Task<Student> ObtenerEstudiantePorMatriculaAsync(string matricula)
        {
            return await _contexto.Estudiantes
                .Include(e => e.Registrations)
                    .ThenInclude(r => r.Materia)
                .FirstOrDefaultAsync(e => e.StudentId == matricula);
        }
        
        // Sobrescribimos el método ObtenerPorIdAsync para incluir las relaciones
        public override async Task<Student> ObtenerPorIdAsync(int id)
        {
            return await _contexto.Estudiantes
                .Include(e => e.Registrations)
                    .ThenInclude(r => r.Materia)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        // Nuevo método: obtener estudiantes por materia
        public async Task<IReadOnlyList<Student>> ObtenerEstudiantesPorMateriaIdAsync(int materiaId)
        {
            return await _contexto.Estudiantes
                .Where(e => e.Registrations.Any(r => r.MateriaId == materiaId))
                .Include(e => e.Registrations)
                .ToListAsync();
        }

    }
}
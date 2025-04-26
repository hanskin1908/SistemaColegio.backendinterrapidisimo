using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaColegio.Domain.Entities;

namespace SistemaColegio.Domain.Interfaces
{
    public interface IEstudianteRepositorio : IRepositorio<Student>
    {
        Task<IReadOnlyList<Student>> ObtenerEstudiantesConRegistrosAsync();
        Task<Student> ObtenerEstudiantePorMatriculaAsync(string matricula);
        // Nuevo método para obtener estudiantes por materia
        Task<IReadOnlyList<Student>> ObtenerEstudiantesPorMateriaIdAsync(int materiaId);
    }
}
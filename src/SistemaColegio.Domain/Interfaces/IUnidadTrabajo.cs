using System;
using System.Threading.Tasks;

namespace SistemaColegio.Domain.Interfaces
{
    public interface IUnidadTrabajo : IDisposable
    {
        IEstudianteRepositorio Estudiantes { get; }
        IProfesorRepositorio Profesores { get; }
        IMateriaRepositorio Materias { get; }
        IRegistroRepositorio Registros { get; }
        
        Task<int> CompletarAsync();
        Task<bool> GuardarCambiosAsync();
    }
}
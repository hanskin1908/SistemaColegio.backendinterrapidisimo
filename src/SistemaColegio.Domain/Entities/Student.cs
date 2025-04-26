using System.Collections.Generic;

namespace SistemaColegio.Domain.Entities
{
    public class Student : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        
        // Propiedad adicional para compatibilidad
        public string Nombre => Name;
        
        // Relaciones
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}
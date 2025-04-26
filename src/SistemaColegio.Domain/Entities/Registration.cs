using System;

namespace SistemaColegio.Domain.Entities
{
    public class Registration : BaseEntity
    {
        public int EstudianteId { get; set; }
        public Student Estudiante { get; set; }
        
        public int MateriaId { get; set; }
        public Subject Materia { get; set; }
        
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        
        // Propiedades adicionales para compatibilidad con lectura/escritura
        public int StudentId { get => EstudianteId; set => EstudianteId = value; }
        public int SubjectId { get => MateriaId; set => MateriaId = value; }
        public DateTime RegistrationDate { get => FechaRegistro; set => FechaRegistro = value; }
    }
}
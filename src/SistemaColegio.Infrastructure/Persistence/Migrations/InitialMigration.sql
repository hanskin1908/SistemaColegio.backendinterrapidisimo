-- Crear tablas

-- Tabla de Estudiantes
CREATE TABLE IF NOT EXISTS Students (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    StudentId NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_Students_Email UNIQUE (Email),
    CONSTRAINT UQ_Students_StudentId UNIQUE (StudentId)
);

-- Tabla de Profesores
CREATE TABLE IF NOT EXISTS Professors (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_Professors_Email UNIQUE (Email)
);

-- Tabla de Materias
CREATE TABLE IF NOT EXISTS Subjects (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Code NVARCHAR(20) NOT NULL,
    Credits INT NOT NULL,
    ProfessorId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_Subjects_Code UNIQUE (Code),
    CONSTRAINT FK_Subjects_Professors FOREIGN KEY (ProfessorId) REFERENCES Professors(Id) ON DELETE SET NULL
);

-- Tabla de Registros (Inscripciones)
CREATE TABLE IF NOT EXISTS Registrations (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StudentId INT NOT NULL,
    SubjectId INT NOT NULL,
    RegistrationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Registrations_Students FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Registrations_Subjects FOREIGN KEY (SubjectId) REFERENCES Subjects(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Registrations_Student_Subject UNIQUE (StudentId, SubjectId)
);

-- Insertar datos de ejemplo

-- Insertar Estudiantes
INSERT INTO Students (Name, Email, StudentId)
VALUES 
('Juan Pérez', 'juan.perez@ejemplo.com', 'EST-001'),
('María García', 'maria.garcia@ejemplo.com', 'EST-002'),
('Carlos Rodríguez', 'carlos.rodriguez@ejemplo.com', 'EST-003'),
('Ana Martínez', 'ana.martinez@ejemplo.com', 'EST-004'),
('Luis Sánchez', 'luis.sanchez@ejemplo.com', 'EST-005');

-- Insertar Profesores
INSERT INTO Professors (Name, Email, Department)
VALUES 
('Roberto Gómez', 'roberto.gomez@ejemplo.com', 'Matemáticas'),
('Laura Torres', 'laura.torres@ejemplo.com', 'Ciencias'),
('Miguel Fernández', 'miguel.fernandez@ejemplo.com', 'Historia'),
('Carmen López', 'carmen.lopez@ejemplo.com', 'Literatura'),
('Javier Díaz', 'javier.diaz@ejemplo.com', 'Informática');

-- Insertar Materias
INSERT INTO Subjects (Name, Code, Credits, ProfessorId)
VALUES 
('Cálculo I', 'MAT-101', 4, 1),
('Física Básica', 'FIS-101', 4, 2),
('Historia Universal', 'HIS-101', 3, 3),
('Literatura Contemporánea', 'LIT-101', 3, 4),
('Programación I', 'INF-101', 5, 5),
('Álgebra Lineal', 'MAT-102', 4, 1),
('Química General', 'QUI-101', 4, 2);

-- Insertar Registros
INSERT INTO Registrations (StudentId, SubjectId, RegistrationDate)
VALUES 
(1, 1, GETDATE()),
(1, 3, GETDATE()),
(1, 5, GETDATE()),
(2, 2, GETDATE()),
(2, 4, GETDATE()),
(3, 1, GETDATE()),
(3, 2, GETDATE()),
(3, 5, GETDATE()),
(4, 3, GETDATE()),
(4, 4, GETDATE()),
(5, 5, GETDATE()),
(5, 6, GETDATE()),
(5, 7, GETDATE());
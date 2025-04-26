-- Script de migraciu00f3n inicial corregido para evitar problemas con columnas faltantes

-- Crear tablas con sintaxis correcta para SQL Server

-- Tabla de Estudiantes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
BEGIN
    CREATE TABLE Students (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        StudentId NVARCHAR(20) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT UQ_Students_Email UNIQUE (Email),
        CONSTRAINT UQ_Students_StudentId UNIQUE (StudentId)
    );
    PRINT 'Tabla Students creada';
END
ELSE
BEGIN
    PRINT 'La tabla Students ya existe';
END

-- Tabla de Profesores
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Professors')
BEGIN
    CREATE TABLE Professors (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        Department NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT UQ_Professors_Email UNIQUE (Email)
    );
    PRINT 'Tabla Professors creada';
END
ELSE
BEGIN
    PRINT 'La tabla Professors ya existe';
END

-- Tabla de Materias
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Subjects')
BEGIN
    CREATE TABLE Subjects (
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
    PRINT 'Tabla Subjects creada';
END
ELSE
BEGIN
    PRINT 'La tabla Subjects ya existe';
END

-- Tabla de Registros (Inscripciones) con columnas en inglu00e9s y espau00f1ol
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Registrations')
BEGIN
    CREATE TABLE Registrations (
        Id INT PRIMARY KEY IDENTITY(1,1),
        -- Columnas originales en inglu00e9s
        StudentId INT NOT NULL,
        SubjectId INT NOT NULL,
        SubjectId1 INT NULL, -- Columna adicional que causaba error
        RegistrationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        -- Columnas equivalentes en espau00f1ol
        EstudianteId INT NOT NULL,
        MateriaId INT NOT NULL,
        FechaRegistro DATETIME2 NOT NULL DEFAULT GETDATE(),
        -- Otras columnas
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        -- Restricciones
        CONSTRAINT FK_Registrations_Students FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Registrations_Subjects FOREIGN KEY (SubjectId) REFERENCES Subjects(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_Registrations_Student_Subject UNIQUE (StudentId, SubjectId)
    );
    PRINT 'Tabla Registrations creada';
END
ELSE
BEGIN
    PRINT 'La tabla Registrations ya existe';
END

-- Crear trigger para mantener sincronizadas las columnas duplicadas
GO
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Registrations_SyncColumns')
    DROP TRIGGER TR_Registrations_SyncColumns;
GO

CREATE TRIGGER TR_Registrations_SyncColumns
ON Registrations
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Sincronizar StudentId y EstudianteId
    UPDATE Registrations
    SET StudentId = i.EstudianteId
    FROM Registrations r
    INNER JOIN inserted i ON r.Id = i.Id
    WHERE r.StudentId <> i.EstudianteId;
    
    UPDATE Registrations
    SET EstudianteId = i.StudentId
    FROM Registrations r
    INNER JOIN inserted i ON r.Id = i.Id
    WHERE r.EstudianteId <> i.StudentId;
    
    -- Sincronizar SubjectId y MateriaId
    UPDATE Registrations
    SET SubjectId = i.MateriaId
    FROM Registrations r
    INNER JOIN inserted i ON r.Id = i.Id
    WHERE r.SubjectId <> i.MateriaId;
    
    UPDATE Registrations
    SET MateriaId = i.SubjectId
    FROM Registrations r
    INNER JOIN inserted i ON r.Id = i.Id
    WHERE r.MateriaId <> i.SubjectId;
    
    -- Sincronizar SubjectId1 con SubjectId
    UPDATE Registrations
    SET SubjectId1 = i.SubjectId
    FROM Registrations r
    INNER JOIN inserted i ON r.Id = i.Id
    WHERE (r.SubjectId1 <> i.SubjectId OR r.SubjectId1 IS NULL) AND i.SubjectId IS NOT NULL;
    
    -- Sincronizar RegistrationDate y FechaRegistro
    UPDATE Registrations
    SET RegistrationDate = i.FechaRegistro
    FROM Registrations r
    INNER JOIN inserted i ON r.Id = i.Id
    WHERE r.RegistrationDate <> i.FechaRegistro;
    
    UPDATE Registrations
    SET FechaRegistro = i.RegistrationDate
    FROM Registrations r
    INNER JOIN inserted i ON r.Id = i.Id
    WHERE r.FechaRegistro <> i.RegistrationDate;
END;
GO

-- Insertar datos de ejemplo solo si las tablas estu00e1n vacu00edas

-- Insertar Estudiantes
IF NOT EXISTS (SELECT TOP 1 * FROM Students)
BEGIN
    INSERT INTO Students (Name, Email, StudentId)
    VALUES 
    ('Juan Pu00e9rez', 'juan.perez@ejemplo.com', 'EST-001'),
    ('Maru00eda Garcu00eda', 'maria.garcia@ejemplo.com', 'EST-002'),
    ('Carlos Rodru00edguez', 'carlos.rodriguez@ejemplo.com', 'EST-003'),
    ('Ana Martu00ednez', 'ana.martinez@ejemplo.com', 'EST-004'),
    ('Luis Su00e1nchez', 'luis.sanchez@ejemplo.com', 'EST-005');
    PRINT 'Datos insertados en Students';
END

-- Insertar Profesores
IF NOT EXISTS (SELECT TOP 1 * FROM Professors)
BEGIN
    INSERT INTO Professors (Name, Email, Department)
    VALUES 
    ('Roberto Gu00f3mez', 'roberto.gomez@ejemplo.com', 'Matemu00e1ticas'),
    ('Laura Torres', 'laura.torres@ejemplo.com', 'Ciencias'),
    ('Miguel Fernu00e1ndez', 'miguel.fernandez@ejemplo.com', 'Historia'),
    ('Carmen Lu00f3pez', 'carmen.lopez@ejemplo.com', 'Literatura'),
    ('Javier Du00edaz', 'javier.diaz@ejemplo.com', 'Informu00e1tica');
    PRINT 'Datos insertados en Professors';
END

-- Insertar Materias
IF NOT EXISTS (SELECT TOP 1 * FROM Subjects)
BEGIN
    INSERT INTO Subjects (Name, Code, Credits, ProfessorId)
    VALUES 
    ('Cu00e1lculo I', 'MAT-101', 4, 1),
    ('Fu00edsica Bu00e1sica', 'FIS-101', 4, 2),
    ('Historia Universal', 'HIS-101', 3, 3),
    ('Literatura Contemporu00e1nea', 'LIT-101', 3, 4),
    ('Programaciu00f3n I', 'INF-101', 5, 5),
    ('u00c1lgebra Lineal', 'MAT-102', 4, 1),
    ('Quu00edmica General', 'QUI-101', 4, 2);
    PRINT 'Datos insertados en Subjects';
END

-- Insertar Registros (con columnas en inglu00e9s y espau00f1ol sincronizadas)
IF NOT EXISTS (SELECT TOP 1 * FROM Registrations)
BEGIN
    INSERT INTO Registrations (StudentId, SubjectId, RegistrationDate, EstudianteId, MateriaId, FechaRegistro, SubjectId1)
    VALUES 
    (1, 1, GETDATE(), 1, 1, GETDATE(), 1),
    (1, 3, GETDATE(), 1, 3, GETDATE(), 3),
    (1, 5, GETDATE(), 1, 5, GETDATE(), 5),
    (2, 2, GETDATE(), 2, 2, GETDATE(), 2),
    (2, 4, GETDATE(), 2, 4, GETDATE(), 4),
    (3, 1, GETDATE(), 3, 1, GETDATE(), 1),
    (3, 2, GETDATE(), 3, 2, GETDATE(), 2),
    (3, 5, GETDATE(), 3, 5, GETDATE(), 5),
    (4, 3, GETDATE(), 4, 3, GETDATE(), 3),
    (4, 4, GETDATE(), 4, 4, GETDATE(), 4),
    (5, 5, GETDATE(), 5, 5, GETDATE(), 5),
    (5, 6, GETDATE(), 5, 6, GETDATE(), 6),
    (5, 7, GETDATE(), 5, 7, GETDATE(), 7);
    PRINT 'Datos insertados en Registrations';
END

-- Script para actualizar la tabla si ya existe
GO
-- Verificar si la tabla Registrations ya existe y necesita modificaciones
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Registrations')
BEGIN
    DECLARE @sql NVARCHAR(MAX) = '';
    
    -- Verificar columna EstudianteId
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Registrations' AND COLUMN_NAME = 'EstudianteId')
    BEGIN
        SET @sql = @sql + 'ALTER TABLE Registrations ADD EstudianteId INT NOT NULL DEFAULT 0; ';
    END
    
    -- Verificar columna MateriaId
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Registrations' AND COLUMN_NAME = 'MateriaId')
    BEGIN
        SET @sql = @sql + 'ALTER TABLE Registrations ADD MateriaId INT NOT NULL DEFAULT 0; ';
    END
    
    -- Verificar columna FechaRegistro
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Registrations' AND COLUMN_NAME = 'FechaRegistro')
    BEGIN
        SET @sql = @sql + 'ALTER TABLE Registrations ADD FechaRegistro DATETIME2 NOT NULL DEFAULT GETDATE(); ';
    END
    
    -- Verificar columna SubjectId1
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Registrations' AND COLUMN_NAME = 'SubjectId1')
    BEGIN
        SET @sql = @sql + 'ALTER TABLE Registrations ADD SubjectId1 INT NULL; ';
    END
    
    -- Ejecutar las alteraciones si hay alguna
    IF @sql <> ''
    BEGIN
        EXEC sp_executesql @sql;
        PRINT 'Columnas faltantes agregadas a la tabla Registrations';
        
        -- Sincronizar datos entre columnas equivalentes
        UPDATE Registrations SET EstudianteId = StudentId;
        UPDATE Registrations SET MateriaId = SubjectId;
        UPDATE Registrations SET FechaRegistro = RegistrationDate;
        UPDATE Registrations SET SubjectId1 = SubjectId;
        PRINT 'Datos sincronizados entre columnas equivalentes';
    END
END

PRINT 'Script de migraciu00f3n completado exitosamente';
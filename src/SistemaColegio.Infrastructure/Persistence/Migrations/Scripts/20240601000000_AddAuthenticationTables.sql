-- Migration: AddAuthenticationTables
-- Description: Adds authentication tables and updates existing tables to work with the authentication system

-- Create Users table
CREATE TABLE IF NOT EXISTS Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'student',
    StudentId INT NULL,
    ProfessorId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastLogin DATETIME2 NULL,
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT FK_Users_Students FOREIGN KEY (StudentId) REFERENCES Estudiantes(Id) ON DELETE SET NULL,
    CONSTRAINT FK_Users_Professors FOREIGN KEY (ProfessorId) REFERENCES Profesores(Id) ON DELETE SET NULL
);

-- Update Students table to make StudentId nullable (to match frontend model)
ALTER TABLE Estudiantes
ALTER COLUMN StudentId NVARCHAR(20) NULL;

-- Create index on StudentId and ProfessorId in Users table
CREATE INDEX IX_Users_StudentId ON Users(StudentId);
CREATE INDEX IX_Users_ProfessorId ON Users(ProfessorId);

-- Insert admin user
INSERT INTO Users (Email, PasswordHash, Name, Role)
VALUES ('admin@example.com', '$2a$11$iqJSHD.BGr0E2IxQwYgJmeP3NvhPrXAeLSaGCj6IR/XU5QtjVu5Tm', 'Administrator', 'admin');
-- Note: Password hash is for 'password123'

-- Create stored procedure for user registration
CREATE OR ALTER PROCEDURE sp_RegisterUser
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(MAX),
    @Name NVARCHAR(100),
    @Role NVARCHAR(20),
    @StudentId INT = NULL,
    @ProfessorId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if email already exists
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
    BEGIN
        THROW 50000, 'Email already exists', 1;
        RETURN;
    END
    
    -- Insert new user
    INSERT INTO Users (Email, PasswordHash, Name, Role, StudentId, ProfessorId, CreatedAt)
    VALUES (@Email, @PasswordHash, @Name, @Role, @StudentId, @ProfessorId, GETDATE());
    
    -- Return the new user ID
    SELECT SCOPE_IDENTITY() AS UserId;
END;
GO

-- Create stored procedure for user login
CREATE OR ALTER PROCEDURE sp_LoginUser
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get user with password hash for verification
    SELECT 
        u.Id,
        u.Email,
        u.PasswordHash,
        u.Name,
        u.Role,
        u.StudentId,
        u.ProfessorId
    FROM 
        Users u
    WHERE 
        u.Email = @Email;
    
    -- Update last login time
    IF @@ROWCOUNT > 0
    BEGIN
        UPDATE Users
        SET LastLogin = GETDATE(),
            UpdatedAt = GETDATE()
        WHERE Email = @Email;
    END
END;
GO

-- Create stored procedure to get user profile
CREATE OR ALTER PROCEDURE sp_GetUserProfile
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.Id,
        u.Email,
        u.Name,
        u.Role,
        u.StudentId,
        u.ProfessorId,
        s.StudentId AS StudentIdentifier,
        p.Department
    FROM 
        Users u
    LEFT JOIN 
        Estudiantes s ON u.StudentId = s.Id
    LEFT JOIN 
        Profesores p ON u.ProfessorId = p.Id
    WHERE 
        u.Id = @UserId;
END;
GO
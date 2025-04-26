using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SistemaColegio.Infrastructure.Persistence.Migrations
{
    public partial class AddAuthenticationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Read and execute the SQL script
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations", "Scripts", "20240601000000_AddAuthenticationTables.sql");
            if (File.Exists(sqlFile))
            {
                migrationBuilder.Sql(File.ReadAllText(sqlFile));
            }
            else
            {
                // Fallback if file not found - define the migration directly
                // Create Users table
                migrationBuilder.CreateTable(
                    name: "Users",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                        PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                        Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "student"),
                        StudentId = table.Column<int>(type: "int", nullable: true),
                        ProfessorId = table.Column<int>(type: "int", nullable: true),
                        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                        LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Users", x => x.Id);
                        table.UniqueConstraint("UQ_Users_Email", x => x.Email);
                        table.ForeignKey(
                            name: "FK_Users_Students",
                            column: x => x.StudentId,
                            principalTable: "Estudiantes",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.SetNull);
                        table.ForeignKey(
                            name: "FK_Users_Professors",
                            column: x => x.ProfessorId,
                            principalTable: "Profesores",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.SetNull);
                    });

                // Update Students table to make StudentId nullable
                migrationBuilder.AlterColumn<string>(
                    name: "StudentId",
                    table: "Estudiantes",
                    type: "nvarchar(20)",
                    maxLength: 20,
                    nullable: true,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(20)",
                    oldMaxLength: 20);

                // Create indexes
                migrationBuilder.CreateIndex(
                    name: "IX_Users_StudentId",
                    table: "Users",
                    column: "StudentId");

                migrationBuilder.CreateIndex(
                    name: "IX_Users_ProfessorId",
                    table: "Users",
                    column: "ProfessorId");

                // Insert admin user
                migrationBuilder.InsertData(
                    table: "Users",
                    columns: new[] { "Email", "PasswordHash", "Name", "Role" },
                    values: new object[] { "admin@example.com", "$2a$11$iqJSHD.BGr0E2IxQwYgJmeP3NvhPrXAeLSaGCj6IR/XU5QtjVu5Tm", "Administrator", "admin" });

                // Create stored procedures
                migrationBuilder.Sql(@"
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
                ");

                migrationBuilder.Sql(@"
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
                ");

                migrationBuilder.Sql(@"
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
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop stored procedures
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetUserProfile");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_LoginUser");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_RegisterUser");

            // Drop Users table
            migrationBuilder.DropTable(name: "Users");

            // Revert StudentId column to not nullable
            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Estudiantes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
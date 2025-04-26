# Sistema Colegio - Backend v2

Este proyecto implementa el backend para un sistema de gestión de colegio utilizando una arquitectura DDD (Domain-Driven Design) y principios de Clean Code.

## Estructura del Proyecto

El proyecto está organizado en las siguientes capas:

- **SistemaColegio.Domain**: Contiene las entidades del dominio, interfaces de repositorio y lógica de dominio.
- **SistemaColegio.Application**: Contiene los servicios de aplicación, DTOs, interfaces y lógica de negocio.
- **SistemaColegio.Infrastructure**: Contiene la implementación de repositorios, configuración de base de datos y servicios externos.
- **SistemaColegio.API**: Contiene los controladores API, configuración de middleware y punto de entrada de la aplicación.

## Requisitos Previos

- .NET 6.0 SDK o superior
- SQL Server (o SQL Server Express)
- Visual Studio 2022 o Visual Studio Code

## Configuración de la Base de Datos

1. Actualiza la cadena de conexión en `src/SistemaColegio.API/appsettings.json` según tu entorno.
2. Ejecuta las migraciones para crear la base de datos:

```bash
cd src/SistemaColegio.API
dotnet ef database update
```

Alternativamente, puedes ejecutar el script SQL ubicado en `src/SistemaColegio.Infrastructure/Persistence/Migrations/InitialMigration.sql` directamente en tu servidor SQL.

## Ejecución del Proyecto

1. Restaura los paquetes NuGet:

```bash
dotnet restore
```

2. Compila el proyecto:

```bash
dotnet build
```

3. Ejecuta la API:

```bash
cd src/SistemaColegio.API
dotnet run
```

La API estará disponible en `https://localhost:5001` y `http://localhost:5000`.

## Endpoints API

### Estudiantes

- `GET /api/estudiantes` - Obtener todos los estudiantes
- `GET /api/estudiantes/{id}` - Obtener estudiante por ID
- `GET /api/estudiantes/matricula/{matricula}` - Obtener estudiante por matrícula
- `POST /api/estudiantes` - Crear nuevo estudiante
- `PUT /api/estudiantes/{id}` - Actualizar estudiante
- `DELETE /api/estudiantes/{id}` - Eliminar estudiante

### Profesores

- `GET /api/profesores` - Obtener todos los profesores
- `GET /api/profesores/{id}` - Obtener profesor por ID
- `GET /api/profesores/email/{email}` - Obtener profesor por email
- `POST /api/profesores` - Crear nuevo profesor
- `PUT /api/profesores/{id}` - Actualizar profesor
- `DELETE /api/profesores/{id}` - Eliminar profesor

### Materias

- `GET /api/materias` - Obtener todas las materias
- `GET /api/materias/{id}` - Obtener materia por ID
- `GET /api/materias/profesor/{profesorId}` - Obtener materias por profesor
- `GET /api/materias/codigo/{codigo}` - Obtener materia por código
- `POST /api/materias` - Crear nueva materia
- `PUT /api/materias/{id}` - Actualizar materia
- `DELETE /api/materias/{id}` - Eliminar materia

### Registros

- `GET /api/registros` - Obtener todos los registros
- `GET /api/registros/{id}` - Obtener registro por ID
- `GET /api/registros/estudiante/{estudianteId}` - Obtener registros por estudiante
- `GET /api/registros/materia/{materiaId}` - Obtener registros por materia
- `POST /api/registros` - Crear nuevo registro
- `PUT /api/registros/{id}` - Actualizar registro
- `DELETE /api/registros/{id}` - Eliminar registro

## Autenticación

La API utiliza autenticación JWT. Para obtener un token, utiliza el endpoint:

- `POST /api/auth/login` - Iniciar sesión y obtener token

Luego, incluye el token en el encabezado de autorización para las solicitudes protegidas:

```
Authorization: Bearer {tu_token}
```

## Contribución

1. Clona el repositorio
2. Crea una rama para tu funcionalidad (`git checkout -b feature/nueva-funcionalidad`)
3. Realiza tus cambios y haz commit (`git commit -am 'Agrega nueva funcionalidad'`)
4. Sube tus cambios (`git push origin feature/nueva-funcionalidad`)
5. Crea un Pull Request
# PruebaTecnica OnOffSolucionesDigitales - Backend

API RESTful desarrollada con .NET 9 para una aplicación de gestión de tareas (To-Do List) con autenticación JWT. Este proyecto demuestra la implementación de Clean Architecture, buenas prácticas de desarrollo y patrones de diseño modernos.

## 📋 Tabla de Contenidos

- [Características](#características)
- [Arquitectura](#arquitectura)
- [Tecnologías](#tecnologías)
- [Prerrequisitos](#prerrequisitos)
- [Instalación](#instalación)
- [Configuración de Base de Datos](#configuración-de-base-de-datos)
- [Ejecución](#ejecución)
- [Pruebas](#pruebas)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [API Endpoints](#api-endpoints)
- [Frontend](#frontend)

## ✨ Características

- **Autenticación JWT**: Sistema seguro de autenticación sin estado
- **CRUD Completo**: Gestión completa de tareas (crear, leer, actualizar, eliminar)
- **Dashboard**: Métricas de tareas.
- **Validación de Datos**: Validación automática mediante Data Annotations
- **Manejo de Errores**: Sistema robusto de control de excepciones y logging
- **Testing**: Suite de pruebas unitarias con xUnit
- **Documentación API**: Swagger/OpenAPI integrado

## 🏗️ Arquitectura

El proyecto implementa **Clean Architecture** organizada en múltiples capas:

```
PTOnOffToDoListBackEnd/
├── ApplicationCore/        # Lógica de negocio y entidades
│   ├── Entities/
│   ├── Interfaces/
|   ├── Models/
├── Infrastructure/         # Acceso a datos y servicios externos
│   ├── Data/
│   └── Services/
└── WebApiPTBackOnOff/     # Capa de presentación (API)
|   ├── Controllers/
|   └── Utils/
├── WebApiPTBackOnOff.Shared/   # Capa de compartidos para las demas capas
│   ├── Utils/
├── OnOffXUnitTesting/  # Capa de Pruebas
```

### Decisiones de Arquitectura

| Aspecto | Implementación | Justificación |
|---------|----------------|---------------|
| **Estructura** | Clean Architecture (Multicapa) | Separación de responsabilidades, facilita testing y mantenimiento |
| **Autenticación** | JWT (JSON Web Tokens) | Estándar de la industria para APIs RESTful sin estado |
| **ORM** | Entity Framework Core | Abstracción de base de datos con patrón Repository |
| **Logging** | Sistema centralizado en `/utils/log` | Trazabilidad eficiente y debugging |
| **Testing** | xUnit | Framework robusto para pruebas unitarias en .NET |

## 🗄️ Base de Datos

**SQL Server** con arquitectura basada en esquemas:

- **Esquema `Auth`**: Gestión de usuarios y autenticación
- **Esquema `Tasks`**: Gestión de tareas

### Características de la BD

- Restricciones `UNIQUE` en nombres de usuario
- Relación 1:N con `ON DELETE CASCADE`
- Auditoría automática con `dtDateTimeRegister`
- Claves primarias auto-incrementales (`IDENTITY`)

## 🛠️ Tecnologías

- **.NET 9 SDK**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server** (2019 o superior)
- **JWT Authentication**
- **xUnit** (Testing)
- **Swagger/OpenAPI**

## 📦 Prerrequisitos

Antes de comenzar, asegúrate de tener instalado:

1. **Visual Studio 2022** (recomendado) o Visual Studio Code
2. **.NET 9 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download/dotnet/9.0)
3. **SQL Server** (2019 o superior)
4. **SQL Server Management Studio (SSMS)** - Versión 18 o superior

## 🚀 Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/JuanMaldonado95/PTOnOffToDoListBackEnd.git
cd PTOnOffToDoListBackEnd
```

### 2. Restaurar Dependencias

```bash
dotnet restore
```

### 3. Compilar el Proyecto

```bash
dotnet build
```

## 💾 Configuración de Base de Datos

### Paso 1: Crear la Base de Datos

Ejecuta el siguiente script en **SQL Server Management Studio**:

```sql
-- Crear base de datos
IF NOT EXISTS (SELECT name FROM master.sys.databases WHERE name = N'DBPTOnOff')
BEGIN
    CREATE DATABASE [DBPTOnOff];
END
GO

USE [DBPTOnOff];
GO

-- Crear esquemas
IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'Auth')
    EXEC('CREATE SCHEMA Auth');
GO

IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'Tasks')
    EXEC('CREATE SCHEMA Tasks');
GO

-- Tabla de usuarios
IF OBJECT_ID('Auth.tblUser', 'U') IS NOT NULL 
    DROP TABLE Auth.tblUser; 

CREATE TABLE Auth.tblUser (
    iIDUser INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    tUserName NVARCHAR(256) NOT NULL UNIQUE,
    tPasswordHash NVARCHAR(32) NOT NULL, 
    dtDateTimeRegister DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Tabla de tareas
IF OBJECT_ID('Tasks.tblTask', 'U') IS NOT NULL 
    DROP TABLE Tasks.tblTask; 

CREATE TABLE Tasks.tblTask (
    iIDTask INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    iIDUser INT NOT NULL, 
    tTitle NVARCHAR(512) NOT NULL,
    bIsCompleted BIT NOT NULL DEFAULT 0,
    dtDateTimeRegister DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Task_User 
        FOREIGN KEY (iIDUser) 
        REFERENCES Auth.tblUser(iIDUser) 
        ON DELETE CASCADE
);
GO
```

### Paso 2: Insertar Usuario de Prueba

```sql
USE [DBPTOnOff]
GO

INSERT INTO Auth.tblUser (tUserName, tPasswordHash)
VALUES ('user@test.com', 'E10ADC3949BA59ABBE56E057F20F883E');
GO
```

**Credenciales de acceso:**
- **Usuario**: `user@test.com`
- **Contraseña**: `123456`

### Paso 3: Configurar Cadena de Conexión

Edita el archivo `appsettings.json` en el proyecto `WebApiPTBackOnOff`:

```json
{
  "ConnectionStrings": {
    "SQLServerConnection": "Server=TU_SERVIDOR;Database=DBPTOnOff;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

**Reemplaza `TU_SERVIDOR`** con el nombre de tu instancia de SQL Server (ej. `localhost`, `.\SQLEXPRESS`, etc.)

## ▶️ Ejecución

### Opción 1: Desde Visual Studio Code

1. Abre la solución `PTOnOffToDoListBackEnd.sln`
2. Establece `WebApiPTBackOnOff` como proyecto de inicio
3. Presiona **F5** o haz clic en el botón **Run**

### Opción 2: Desde la Terminal

```bash
cd WebApiPTBackOnOff
dotnet run
```

La API estará disponible en: **https://localhost:44363**

### Documentación Swagger

Una vez iniciada la aplicación, accede a la documentación interactiva:

```
https://localhost:44363/swagger
```

## 🧪 Pruebas

El proyecto incluye pruebas unitarias con **xUnit** que cubren controladores y servicios.

### Ejecutar todas las pruebas

```bash
dotnet test
```

### Ejecutar pruebas con cobertura detallada

```bash
dotnet test --verbosity detailed
```

### Desde Visual Studio Code

1. Menú **Pruebas** > **Explorador de Pruebas**
2. Haz clic en **Ejecutar todas las pruebas**

## 🔌 API Endpoints

### Autenticación

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/auth/login` | Autenticación de usuario |

### Tareas

| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| GET | `/api/tasks` | Listar todas las tareas del usuario | ✅ |
| POST | `/api/tasks` | Crear nueva tarea | ✅ |
| PUT | `/api/tasks/{id}` | Actualizar tarea | ✅ |
| DELETE | `/api/tasks/{id}` | Eliminar tarea | ✅ |
| GET | `/api/tasks/dashboard` | Obtener métricas | ✅ |

### Ejemplo de Request

```bash
# Login
curl -X POST https://localhost:44363/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"user@test.com","password":"123456"}'

# Crear tarea (requiere token JWT)
curl -X POST https://localhost:44363/api/tasks \
  -H "Authorization: Bearer {tu_token}" \
  -H "Content-Type: application/json" \
  -d '{"title":"Mi primera tarea"}'
```

## 💻 Frontend

Este proyecto tiene un frontend complementario desarrollado en Angular:

**Repositorio**: [PTOnOffToDoListFrontEnd](https://github.com/JuanMaldonado95/PTOnOffToDoListFrontEnd)

**Nota importante**: Asegúrate de que el backend esté corriendo en `https://localhost:44363` antes de iniciar el frontend.

## 📝 Notas Adicionales

- El hash de la contraseña en la base de datos utiliza **MD5** (para propósitos de demostración). En producción se recomienda usar **bcrypt** o **Argon2**.
- El token JWT tiene una expiración configurable en `appsettings.json`
- Los logs se almacenan en la carpeta `/Utils/log`


## 📄 Licencia

Este proyecto fue desarrollado como parte de una prueba técnica para OnOff Soluciones Digitales.

## 👤 Autor

**Juan Maldonado**
- GitHub: [@JuanMaldonado95](https://github.com/JuanMaldonado95)

---

**¿Necesitas ayuda?** Si encuentras algún problema durante la instalación o ejecución, por favor abre un issue en el repositorio.

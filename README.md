# PruebaTecnica OnOffSolucionesDigitales 

Este proyecto es la solución desarrollada para la prueba técnica .net, cuyo objetivo principal es **evaluar las habilidades técnicas en Angular y .NET 9**, incluyendo arquitectura, gestión de estados, optimización e integración de APIs.

El proyecto implementa una aplicación completa de **Lista de Tareas (To-Do List)** que cumple con los siguientes requisitos funcionales:

* **Autenticación:** Implementación de un flujo de Inicio de Sesión (Login) con autenticación basada en JWT a través de la API de .NET 9.
* **Gestión de Tareas (CRUD):** Funcionalidad completa para ver, agregar, editar, eliminar y marcar tareas como completadas/pendientes.
* **Dashboard:** Visualización de métricas clave (total, completadas, pendientes).
* **Notificaciones:** Retroalimentación al usuario mediante mensajes de éxito o error.

________________________________________________________________________________________________________________________________________________________________________

### Arquitectura del Backend (.NET 9)

| Requisito Técnico | Decisión Implementada | Justificación |
| :--- | :--- | :--- |
| **Estructura Base** | Arquitectura de Múltiples Capas (Clean Architecture). Proyectos de clase: | La lógica de negocio reside en ApplicationCore, el acceso a datos en Infrastructure, y la capa de presentación/API en WebAPI. Facilita el mantenimiento, testing y la escalabilidad. |
| **Autenticación** | **JSON Web Tokens (JWT)** | Estándar de la industria para autenticación sin estado (stateless), ideal para APIs RESTful. El token se genera al hacer login y se valida en cada request. |
| **Acceso a Datos** | **Entity Framework Core** | Utilizado con un patrón de Repositorio. Esto abstrae la lógica de la base de datos y facilita el testing (mediante la inyección de repositorios simulados). |
| **Validación de Datos** | **Data Annotations** Carpeta de modelos para la validación automática del modelo en los Controladores. | Garantiza que los datos recibidos por los *endpoints* cumplan con las reglas de negocio antes de procesarse. |
| **Manejo de Errores** | Control Explícito de Errores (try-catch y retorno de códigos HTTP específicos). | Se implementa try-catch en la lógica de negocio para capturar errores de manera granular y asegurar que el backend responda con códigos de estado HTTP semánticos (ej. 400 Bad Request, 404 Not Found, 500 Internal Server Error) y mensajes claros. |
| Sistema de Logs | Archivo utils/log dedicado | Centraliza el manejo de errores (logging) y mensajes de depuración, permitiendo una trazabilidad eficiente y desacoplando la presentación del manejo de excepciones.

________________________________________________________________________________________________________________________________________________________________________

###  SQL Server con SQL Server Management Studio 21

| Requisito Técnico | Decisión Implementada | Justificación |
| :--- | :--- | :--- |
| **Organización Lógica** | Uso de Esquemas (Auth y Tasks) | Facilita la separación de la información sensible (Autenticación) de la lógica de negocio (Tareas), mejorando la seguridad y la modularidad.
| **Unicidad de Cuentas** | Restricción UNIQUE en Auth.tblUser.tUserName | Garantiza que no existan nombres de usuario duplicados, manteniendo la integridad del subsistema de autenticación.
| **Integridad Referencial** | Relación 1:M con ON DELETE CASCADE | Asegura que al eliminar un registro principal (Usuario), todos los registros dependientes (Tareas) se eliminen automáticamente.
| **Auditoría Básica** | Columna dtDateTimeRegister con DEFAULT GETDATE() | Permite registrar automáticamente la fecha y hora de inserción de cualquier fila sin requerir lógica en el Backend.
| **Identificadores** | Uso de IDENTITY(1,1) | Implementación de claves primarias autoincrementables estándar para un manejo simple y eficiente de las filas.

________________________________________________________________________________________________________________________________________________________________________

### Prerrequisitos Tecnicos

Lista de las principales tecnologías y versiones.

* **Editor de Código:** Se recomienda Visual Studio 2022 para BackEnd.

* **Servidor SQL Server:** Con las credenciales necesarias para conexión.

* **SDK de .NET 9:** Necesario para compilar y ejecutar el backend.
    * [Descargar .NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (Enlace de ejemplo, el enlace final debe ser verificado al momento del lanzamiento oficial de .NET 9).

________________________________________________________________________________________________________________________________________________________________________

### Creación de la Base de Datos

Pasos para la Configuración Inicial de la Base de Datos

Para que la aplicación funcione correctamente, es necesario preparar la base de datos y registrar un usuario inicial. Sigue estos pasos en este orden:

1. Crear la Base de Datos y Tablas: Ejecuta el 'SCRIPT DE CREACIÓN DE BASE DE DATOS SQL' script completo que te pongo mas abajo en SQL Server Management Studio.
   
3. Registrar el Usuario Inicial: Una vez creada la base de datos, ejecuta este script 'SCRIPT PARA CREAR EL REGISTRO EN LA TABLA' que te adjunto mas abajo:
    - Este usuario es el unico de momento que permite acceso.
      
        usuario: user@test.com
        Contraseña: 123456
________________________________________________________________________________________________________________________________________________________________________

-- SCRIPT DE CREACIÓN DE BASE DE DATOS SQL 

IF NOT EXISTS (SELECT name FROM master.sys.databases WHERE name = N'DBPTOnOff')
BEGIN
    CREATE DATABASE [DBPTOnOff];
END
GO

USE [DBPTOnOff];
GO

IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'Auth')
    EXEC('CREATE SCHEMA Auth');
GO

IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'Tasks')
    EXEC('CREATE SCHEMA Tasks');
GO

IF OBJECT_ID('Auth.tblUser', 'U') IS NOT NULL 
    DROP TABLE Auth.tblUser; 

CREATE TABLE Auth.tblUser (
    iIDUser INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    tUserName NVARCHAR(256) NOT NULL UNIQUE,
    tPasswordHash NVARCHAR(32) NOT NULL, 
    dtDateTimeRegister DATETIME NOT NULL DEFAULT GETDATE()
);
GO

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

________________________________________________________________________________________________________________________________________________________________________

-- SCRIPT PARA CREAR EL REGISTRO EN LA TABLA

use [DBPTOnOff]

INSERT INTO Auth.tblUser (tUserName, tPasswordHash)
VALUES ('user@test.com', 'E10ADC3949BA59ABBE56E057F20F883E');

________________________________________________________________________________________________________________________________________________________________________

### Ejecución del Backend

1.  **Restaurar Dependencias y Compilar:** (Desde el directorio raíz de la solución `.sln`):
    ```bash
    dotnet restore
    dotnet build
    ```
    
2. Configurar la cadena de conexión en `appsettings.json`, reemplazando el nombre del servidor por el de tu instancia de SQL Server:
  "ConnectionStrings": {
    "SQLServerConnection": "Server=JUANMALDONADO;Database=DBPTOnOff;Integrated Security=True;TrustServerCertificate=True"
   }
   
4. **Ejecutar la API:** (Navegue a la carpeta del proyecto `WebApiPTBackOnOff` o use el comando a nivel de solución):
    ```bash
    dotnet run --project WebApiPTBackOnOff
    ```
    *La API estará disponible en **`https://localhost:44363`** (o el puerto configurado). Puede acceder a la documentación de Swagger en esta dirección.*

5. Guardar La dirección del puerto es importante para la ejecución de los servicios del Front en esta versión local, [https://github.com/JuanMaldonado95/PTOnOffToDoListFrontEnd]

________________________________________________________________________________________________________________________________________________________________________

##  Cómo Ejecutar las Pruebas Unitarias realizadas en xUnit

Se han escrito pruebas unitarias (con xUnit de testing de .NET) para al menos un controlador y un servicio, garantizando la cobertura de la lógica de negocio.

1.  **Navegar al Directorio de Solución** (donde se encuentra el archivo `.sln`).
2.  **Ejecutar Pruebas Unitarias:** : Para la ejución puedes navegar al menú superior: Pruebas (Test) > Explorador de Pruebas (Test Explorer) y ejecutar todas las pruebas o las que desee ó ejecutar el siguiente comando:
    ```bash
    dotnet test
    ```

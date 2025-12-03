# 🗂️ Sistema de Gestión de Tareas – TP Final Desarrollo de Sistemas

Aplicación web para la gestión de tareas tipo Kanban, desarrollada utilizando ASP.NET Core MVC, Minimal API, Dapper y SQL Server.

## 📌 Descripción

Este sistema permite administrar tableros, columnas y tareas mediante una interfaz Web MVC y una API REST integrada.
Incluye soporte para drag & drop, CRUD completo y una capa de persistencia basada en Dapper.

## ✨ Características

## 🏠 Tableros y Columnas

Visualización de tableros con sus columnas correspondientes.

Estructura estilo Kanban.

Organización clara y adaptable.

## 📝 Gestión de Tareas

Crear nuevas tareas dentro de una columna.

Editar título, descripción, tipo, prioridad y tiempo estimado.

Eliminar tareas con confirmación.

Ver detalles completos.

Timestamps automáticos: CreatedAt y UpdatedAt.

## 🔀 Drag & Drop

Mover tareas entre columnas con actualización automática.

Integración directa con un endpoint REST.

## 🔌 API REST Integrada

Endpoints CRUD para tareas.

Endpoint especial para mover tareas.

Documentación con Swagger.

## 🛠️ Persistencia Dapper

Consultas SQL eficientes y optimizadas.

DAO separado en la capa DapperData.

## 🛠️ Tecnologías Utilizadas

Backend: ASP.NET Core 8.0 MVC + Minimal API

Persistencia: Dapper (capa independiente en /DapperData)

Frontend: Razor, Bootstrap 5, JavaScript

Base de Datos: SQL Server

Arquitectura: MVC + DAO

Testing: xUnit en /DapperData.Tests

Documentación de API: Swagger

## 📁 Estructura del Proyecto
TPFINALDESARROLLODESISTEMAS/
├── DapperData/             # Capa de acceso a datos (DAO, consultas Dapper)
├── DapperData.Tests/       # Pruebas unitarias de la capa de persistencia
├── MVC/                    # Aplicación web MVC (Controladores, Vistas, wwwroot)
├── Script/                 # Scripts SQL para generar la base de datos
├── TPFinalAPI/             # API REST (Minimal APIs, DTOs)
└── TPFinalDesarrolloDeSistemas.sln

## ⚙️ Configuración

## 1️⃣ Base de Datos

Ejecución de scripts en el orden recomendado:

Script/
├── ddl.sql
├── install.sql
└── storedProcedure.sql

## 2️⃣ Cadena de Conexión

En appsettings.json (MVC o API):

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TareasDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

## 3️⃣ Ejecutar la Aplicación
🖥️ Aplicación MVC
cd MVC
dotnet restore
dotnet build
dotnet run

## 🌐 API
cd TPFinalAPI
dotnet restore
dotnet build
dotnet run

## 🧩 Detalles Técnicos
Controladores (MVC)

TablerosController

ColumnasController

TareasController

HomeController (landing)

Capa Dapper (/DapperData)

Interfaz IDao

Implementación DaoDappers

Métodos:

ObtenerTareas()

ObtenerTareaPorId()

CrearTarea()

ActualizarTarea()

EliminarTarea()

MoverTarea()

Vistas (MVC)

Diseño con Bootstrap 5

Listas Kanban

Formularios de validación

Modales de confirmación

## 🚀 Próximas Mejoras

Sistema de login y roles

Notificaciones en tiempo real

Panel de estadísticas

API más completa (tableros / columnas)

Exportación de datos

## 🤝 Contribución

Crear rama:

git checkout -b feature/NuevaFuncionalidad


Commit:

git commit -m "Agrega nueva funcionalidad"


Push:

git push origin feature/NuevaFuncionalidad


Abrir Pull Request

## 📜 Licencia

Proyecto bajo licencia MIT.

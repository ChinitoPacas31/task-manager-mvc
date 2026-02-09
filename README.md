# Task Manager Pro

Sistema moderno de gestión de tareas con arquitectura MVC.

## Arquitectura

```
├── backend/                    # API REST con C# ASP.NET Core
│   └── TaskManager.API/
│       ├── Controllers/        # Controladores MVC
│       ├── Models/             # Modelos de dominio
│       ├── DTOs/               # Data Transfer Objects
│       ├── Services/           # Lógica de negocio
│       ├── Repositories/       # Acceso a datos
│       └── Data/               # Contexto MongoDB
│
└── frontend/                   # Aplicación Next.js
    └── src/
        ├── app/                # Páginas (App Router)
        ├── components/         # Componentes React
        ├── services/           # Servicios API
        ├── store/              # Estado global (Zustand)
        └── types/              # Tipos TypeScript
```

## Tecnologías

### Backend
- **C# ASP.NET Core 8.0** - Framework web
- **MongoDB** - Base de datos NoSQL
- **JWT** - Autenticación
- **BCrypt** - Hash de contraseñas

### Frontend
- **Next.js 14** - Framework React
- **TypeScript** - Tipado estático
- **Tailwind CSS** - Estilos
- **Zustand** - Estado global
- **Recharts** - Gráficos
- **Lucide React** - Iconos

## Funcionalidades

- ✅ Autenticación de usuarios (Login/Registro)
- ✅ Gestión de Tareas (CRUD)
- ✅ Gestión de Proyectos
- ✅ Sistema de Comentarios
- ✅ Historial de actividades
- ✅ Notificaciones
- ✅ Búsqueda de tareas
- ✅ Reportes y estadísticas
- ✅ Dashboard con métricas

## Requisitos

- .NET 8.0 SDK
- Node.js 18+
- MongoDB 6.0+

## Instalación

### 1. Base de Datos

Instala y ejecuta MongoDB:

```bash
# Con Docker
docker run -d --name mongodb -p 27017:27017 mongo:6

# O instala MongoDB localmente
```

### 2. Backend

```bash
cd backend/TaskManager.API

# Restaurar paquetes
dotnet restore

# Ejecutar
dotnet run
```

El API estará disponible en `http://localhost:5000`

### 3. Frontend

```bash
cd frontend

# Instalar dependencias
npm install

# Crear archivo de entorno
cp .env.example .env.local

# Ejecutar en desarrollo
npm run dev
```

La aplicación estará disponible en `http://localhost:3000`

## Endpoints API

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrar usuario

### Tareas
- `GET /api/tasks` - Listar tareas (con filtros y paginación)
- `GET /api/tasks/{id}` - Obtener tarea
- `POST /api/tasks` - Crear tarea
- `PUT /api/tasks/{id}` - Actualizar tarea
- `DELETE /api/tasks/{id}` - Eliminar tarea
- `GET /api/tasks/search?q={query}` - Buscar tareas

### Proyectos
- `GET /api/projects` - Listar proyectos
- `GET /api/projects/{id}` - Obtener proyecto
- `POST /api/projects` - Crear proyecto
- `PUT /api/projects/{id}` - Actualizar proyecto
- `DELETE /api/projects/{id}` - Eliminar proyecto

### Comentarios
- `GET /api/comments/task/{taskId}` - Comentarios de tarea
- `POST /api/comments` - Crear comentario
- `PUT /api/comments/{id}` - Actualizar comentario
- `DELETE /api/comments/{id}` - Eliminar comentario

### Notificaciones
- `GET /api/notifications` - Listar notificaciones
- `PUT /api/notifications/{id}/read` - Marcar como leída
- `PUT /api/notifications/read-all` - Marcar todas como leídas

### Reportes
- `GET /api/reports/dashboard` - Estadísticas del dashboard
- `GET /api/reports/productivity` - Reporte de productividad
- `GET /api/reports/activity` - Actividad reciente

## Estructura de Base de Datos (MongoDB)

### Colecciones
- `users` - Usuarios del sistema
- `tasks` - Tareas
- `projects` - Proyectos
- `comments` - Comentarios
- `notifications` - Notificaciones
- `taskHistories` - Historial de cambios

## Licencia

MIT License

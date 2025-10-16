# Prueba Técnica - .NET 8

Este proyecto es una API desarrollada en .NET 8 que incluye autenticación JWT, validaciones con FluentValidation, y acceso a datos mediante Entity Framework Core.

---

## **Requisitos Previos**

Antes de ejecutar el proyecto, asegúrate de tener instalados los siguientes requisitos:

- **.NET SDK 8.0**: [Descargar .NET SDK](https://dotnet.microsoft.com/download)
- **SQL Server**: Un servidor SQL Server para la base de datos.
- **Git**: Para clonar el repositorio.

---

## **Configuración del Proyecto**

### 1. Clonar el Repositorio
```bash
# Clonar el repositorio desde GitHub
git clone https://github.com/Jose-Dev02/Prueba-Tecnica-NET-8-.git

# Navegar al directorio del proyecto
cd Prueba-Tecnica-NET-8-
```

### 2. Configurar la Base de Datos
Edita el archivo `appsettings.json` para configurar la cadena de conexión a tu base de datos SQL Server:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
}
```

### 3. Aplicar Migraciones
Ejecuta los siguientes comandos para aplicar las migraciones y crear la base de datos:
```bash
# Restaurar dependencias
dotnet restore

# Aplicar migraciones
dotnet ef database update
```

---

## **Ejecutar el Proyecto**

1. Ejecuta el siguiente comando para iniciar la API:
```bash
dotnet run
```

2. Accede a la API mediante Swagger UI:
```
https://localhost:5001/swagger
```

---

## **Autenticación JWT**

El proyecto utiliza JWT para la autenticación. Configura las claves en el archivo `appsettings.json`:
```json
"JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY",
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE"
}
```

---

## **Comandos Útiles**

### Crear una Nueva Migración
```bash
dotnet ef migrations add MigrationName
```

### Actualizar la Base de Datos
```bash
dotnet ef database update
```

---

## **Licencia**

Este proyecto está bajo la licencia MIT. Consulta el archivo `LICENSE` para más detalles.

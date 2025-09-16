# 📋 Todo API

![.NET](https://img.shields.io/badge/.NET-9.0-blue?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-Latest-green?style=flat-square&logo=csharp)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core%209.0-orange?style=flat-square)
![JWT](https://img.shields.io/badge/JWT-Authentication-yellow?style=flat-square)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI%203.0-brightgreen?style=flat-square)

A modern, secure RESTful API for task management built with ASP.NET Core, featuring JWT authentication, comprehensive CRUD operations, and clean architecture principles.

## 🌟 Features

### 🔐 **Authentication & Security**
- JWT token-based authentication
- BCrypt password hashing
- Role-based access control
- Secure user registration and login

### 📊 **Task Management**
- Complete CRUD operations for tasks
- Task categorization system
- Priority levels (Low, Medium, High, Critical)
- Status tracking (Todo, InProgress, Done)
- User-specific task isolation

### 📚 **Category Management**
- Organize tasks into categories
- Hierarchical task organization
- Category-based task filtering

### 🔍 **Advanced Features**
- Comprehensive task filtering and search
- Task statistics and analytics
- Pagination support
- RESTful API design
- Comprehensive error handling

### 📖 **Documentation**
- Interactive Swagger UI
- OpenAPI 3.0 specification
- Detailed endpoint documentation
- Authentication examples

## 🏗️ Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
TodoAPI/
├── 📁 TodoAPI.Core/          # Domain entities, enums, interfaces
├── 📁 TodoAPI.Application/   # Business logic, DTOs, services
├── 📁 TodoAPI.Infrastructure/ # Data access, repositories, EF Core
├── 📁 TodoAPI.API/          # Controllers, middleware, configuration
└── 📁 docs/                 # Additional documentation
```

### 🧱 **Core Components**

- **Domain Layer** (`Core`): Entities, enums, repository interfaces
- **Application Layer** (`Application`): DTOs, mapping profiles, services
- **Infrastructure Layer** (`Infrastructure`): EF Core, repositories, data access
- **API Layer** (`API`): Controllers, authentication, Swagger configuration

## 🚀 Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (or SQL Server)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/TodoAPI.git
   cd TodoAPI
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update database connection** (if needed)
   
   Edit `TodoAPI.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TodoApiDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
     }
   }
   ```

4. **Run database migrations**
   ```bash
   cd TodoAPI.API
   dotnet ef database update
   ```

5. **Start the application**
   ```bash
   dotnet run --project TodoAPI.API
   ```

6. **Open Swagger UI**
   
   Navigate to: `https://localhost:7189` or `http://localhost:5050`

## 📚 API Documentation

### 🔗 **Base URLs**
- **HTTPS**: `https://localhost:7189`
- **HTTP**: `http://localhost:5050`
- **Swagger UI**: `https://localhost:7189/` (root)

### 🔐 **Authentication Endpoints**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Register new user |
| `POST` | `/api/auth/login` | User login |
| `POST` | `/api/auth/refresh` | Refresh JWT token |
| `POST` | `/api/auth/logout` | User logout |
| `GET` | `/api/auth/me` | Get current user info |

### 📋 **Task Endpoints**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/api/tasks` | Get user's tasks with filtering | ✅ |
| `GET` | `/api/tasks/{id}` | Get specific task | ✅ |
| `POST` | `/api/tasks` | Create new task | ✅ |
| `PUT` | `/api/tasks/{id}` | Update task | ✅ |
| `PATCH` | `/api/tasks/{id}/status` | Update task status only | ✅ |
| `DELETE` | `/api/tasks/{id}` | Delete task | ✅ |
| `GET` | `/api/tasks/stats` | Get task statistics | ✅ |

### 🏷️ **Category Endpoints**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/api/categories` | Get all categories | ❌ |
| `GET` | `/api/categories/{id}` | Get specific category | ❌ |
| `POST` | `/api/categories` | Create new category | ❌ |
| `PUT` | `/api/categories/{id}` | Update category | ❌ |
| `DELETE` | `/api/categories/{id}` | Delete category | ❌ |

### 🧪 **Testing Endpoints**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/test/health` | Health check |
| `GET` | `/api/test/categories` | Test categories |
| `POST` | `/api/test/test-user` | Create test user |
| `POST` | `/api/jwttest/generate-token` | Generate test JWT |

## 🔧 Configuration

### JWT Settings

Configure JWT in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-that-is-at-least-32-characters-long",
    "Issuer": "TodoAPI",
    "Audience": "TodoAPI-Users",
    "ExpiryInMinutes": 60,
    "RefreshTokenExpiryInDays": 7
  }
}
```

### Database Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TodoApiDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

## 📊 Data Models

### User
```json
{
  "id": "guid",
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

### Task
```json
{
  "id": "guid",
  "title": "string",
  "description": "string",
  "status": "Todo | InProgress | Done",
  "priority": "Low | Medium | High | Critical",
  "userId": "guid",
  "categoryId": "guid?",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

### Category
```json
{
  "id": "guid",
  "name": "string",
  "description": "string?",
  "createdAt": "datetime",
  "taskCount": "integer"
}
```

## 🔒 Authentication

The API uses JWT Bearer token authentication. Include the token in the Authorization header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Registration Example

```bash
curl -X POST "https://localhost:7189/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "SecurePassword123",
    "confirmPassword": "SecurePassword123"
  }'
```

### Login Example

```bash
curl -X POST "https://localhost:7189/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePassword123"
  }'
```

## 💾 Database

The API uses Entity Framework Core with SQL Server LocalDB by default. The database includes:

- **Users** table with authentication data
- **Tasks** table with task information
- **Categories** table for task organization
- Proper foreign key relationships and indexes

### Initial Data

The database is seeded with default categories:
- **Personal** - Personal tasks and activities
- **Work** - Work-related tasks and projects

## 🧪 Testing

### Health Check

```bash
curl -X GET "https://localhost:7189/api/test/health"
```

### Create Test Data

1. Create a test user:
   ```bash
   curl -X POST "https://localhost:7189/api/test/test-user"
   ```

2. Generate JWT token:
   ```bash
   curl -X POST "https://localhost:7189/api/jwttest/generate-token"
   ```

## 🚀 Deployment

### Docker Support

The project is Docker-ready. Build and run with Docker:

```bash
# Build image
docker build -t todoapi .

# Run container
docker run -p 5000:80 todoapi
```

### Production Considerations

1. **Security**: Change JWT secret key for production
2. **Database**: Configure production database connection
3. **HTTPS**: Ensure HTTPS is properly configured
4. **Logging**: Configure structured logging
5. **Environment**: Set `ASPNETCORE_ENVIRONMENT=Production`

## 🛠️ Development

### Prerequisites for Development

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- SQL Server LocalDB
- Git

### Development Commands

```bash
# Build solution
dotnet build

# Run tests (when implemented)
dotnet test

# Run application in development mode
dotnet run --project TodoAPI.API --environment Development

# Create new migration
dotnet ef migrations add MigrationName --project TodoAPI.Infrastructure --startup-project TodoAPI.API

# Update database
dotnet ef database update --project TodoAPI.Infrastructure --startup-project TodoAPI.API
```

## 📦 Dependencies

### Core Packages
- **Microsoft.AspNetCore.App** (9.0) - ASP.NET Core framework
- **Microsoft.EntityFrameworkCore.SqlServer** (9.0.9) - Database provider
- **Microsoft.AspNetCore.Authentication.JwtBearer** (9.0.4) - JWT authentication

### Additional Packages
- **AutoMapper** (15.0.1) - Object mapping
- **BCrypt.Net-Next** (4.0.3) - Password hashing
- **Swashbuckle.AspNetCore** (9.0.4) - Swagger documentation
- **System.IdentityModel.Tokens.Jwt** (8.14.0) - JWT handling

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🎯 Roadmap

- [ ] Unit and integration tests
- [ ] Real-time notifications with SignalR
- [ ] Advanced task filtering and search
- [ ] Task due dates and reminders
- [ ] File attachments for tasks
- [ ] Task comments and collaboration
- [ ] Advanced reporting and analytics
- [ ] Mobile app integration
- [ ] Third-party integrations (Calendar, Email)

## 📞 Support

For support and questions:

- 📧 Email: support@todoapi.com
- 🐛 Issues: [GitHub Issues](https://github.com/yourusername/TodoAPI/issues)
- 📖 Documentation: Available in Swagger UI

---

**Built with ❤️ using ASP.NET Core and Clean Architecture principles**
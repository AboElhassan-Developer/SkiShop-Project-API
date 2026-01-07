# SkiShop API

A modern E-Commerce API built with **.NET Core 8** and **C#**, implementing clean architecture patterns and best practices.

## 🎯 Overview

SkiShop API is the backend service for a complete E-Commerce platform. It provides robust endpoints for product management, user authentication, shopping basket operations, order processing, and Stripe payment integration.

## 🏗️ Architecture

Multi-project solution with clean architecture:
- **Core** - Business logic & domain entities
- **Infrastructure** - Data access & external services
- **SkiShop.API** - API controllers & presentation layer

## 🚀 Key Features

### Authentication & Authorization
- ASP.NET Identity for user management
- Role-based access control
- Secure registration and login

### Data Access Patterns
- Repository Pattern for abstraction
- Unit of Work Pattern for transactions
- Specification Pattern for flexible queries
- Multiple DbContext instances

### E-Commerce Features
- Product catalog with filtering, sorting & pagination
- Shopping basket management with Redis persistence
- Order processing and creation
- Stripe payment integration with 3D Secure

### Performance
- Redis cache for shopping basket
- Efficient query optimization
- Reduced database load

### Real-Time Communication
- SignalR for real-time notifications
- Live order status updates

## 🛠️ Technologies

| Component | Technology |
|-----------|-----------|
| **Framework** | .NET Core 8 |
| **Language** | C# |
| **Database** | SQL Server + Entity Framework Core |
| **Authentication** | ASP.NET Identity |
| **Caching** | Redis |
| **Payments** | Stripe API |
| **Real-Time** | SignalR |
| **Containerization** | Docker & Docker Compose |

## 📋 Prerequisites

- .NET Core 8 SDK
- SQL Server
- Redis server
- Stripe account

## ⚙️ Installation

### 1. Clone Repository
```bash
git clone https://github.com/AboElhassan-Developer/SkiShop-Project-API.git
cd SkiShop-Project-API
```

### 2. Restore & Configure
```bash
dotnet restore
```

Update connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=SkiShop;Trusted_Connection=true;"
  },
  "Stripe": {
    "PublishableKey": "your_key",
    "SecretKey": "your_key"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### 3. Database Setup
```bash
dotnet ef database update
```

### 4. Run
```bash
dotnet run
```

API available at `https://localhost:5001`

## 🐳 Docker

```bash
docker-compose up --build
```

## 📚 API Endpoints

```
Authentication:
POST   /api/auth/register
POST   /api/auth/login

Products:
GET    /api/products
GET    /api/products/{id}

Basket:
GET    /api/basket
POST   /api/basket/add
DELETE /api/basket/remove/{id}

Orders:
GET    /api/orders
POST   /api/orders/create
GET    /api/orders/{id}

Payments:
POST   /api/payments/process
```

## 🧪 Design Patterns

- **Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management
- **Specification Pattern** - Reusable query logic

## 🔒 Security

- Input validation & sanitization
- Authorization on protected endpoints
- CORS configuration
- Secure password hashing
- SQL injection prevention

## 📄 License

MIT License

---

**Happy coding! 🚀**
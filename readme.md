# RESTful-API

This project is a **RESTful API** built with **.NET 9** using **JWT authentication**.  
It is a learning project to practice authentication, authorization, and building a
clean backend structure with Docker, including advanced features like
comprehensive exception handling, and caching.

---

## Table of Contents
- [Features](#features)
- [Technologies](#technologies)
- [Architecture](#architecture)
- [Structure](#structure)
- [Authentication](#authentification)
- [Exception Handling](#exception-handling)
- [Setup & Run](#setup--run)

---

## Features
- **CRUD operations** for user entities
- **User registration & login** with JWT authentication
- **Protected endpoints** with `[Authorize]` attribute
- **Two separate databases**:
    - **MySQL** → Business data (users, entities)
    - **PostgreSQL** → Authentication data (user credentials)
- **Advanced exception handling** with structured error responses
- **Memory caching** for improved performance
- **CORS configuration** for different environments
- **Swagger UI** with JWT support
- **Docker setup** for MySQL & PostgreSQL

---

## Technologies
Following technologies were used:

- .NET SDK 9
- Docker
- Rider IDE
- MySQL
- Postgres

## Architecture

The application follows a layered architecture:

- **Controllers** → Handle HTTP requests (AuthController, UserController)
- **Services** → Business logic (authentication, user handling, CRUD operations)
- **Entities & DTOs** → Data models for persistence and API communication
- **DbContexts** → EF Core contexts (MySQL for business data, PostgreSQL for auth data)
- **Middleware** → Global error handling, logging
- **Authentication** → JWT-based authentication and claims
- **Exception Handling** → Structured error responses
- **Caching** → Memory caching for performance optimization

## Structure
```
/Controllers -> API endpoints (AuthController, UserController, etc.)
/Auth -> JWT services, auth logic
/Context -> EF Core DbContexts (MySQL + PostgreSQL)
/Entity -> Entities & DTOs
/Middleware -> Global exception handling
/Services -> Business logic services
```

## Authentification

**Register**
```http
POST /auth/register
Content-Type: application/json

{
  "username": "Noah",
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Login**
```http
POST /auth/login
Content-Type: application/json

{
  "username": "Noah",
  "password": "Password123!"
}
```

**Response:**
```json
{
  "token": "<JWT_TOKEN>",
  "expiresAtUtc": "2025-09-20T10:00:00Z"
}
```

**Protected endpoint**
```http
GET /auth/me
Authorization: Bearer <JWT_TOKEN>
```

**Response:**
```json
{
  "user": "Noah",
  "claims": [
    { "type": "nameidentifier", "value": "2" },
    { "type": "name", "value": "Noah" },
    { "type": "email", "value": "user@example.com" },
    { "type": "role", "value": "User" }
  ]
}
```

## Exception Handling
The API provides structured error responses for different scenarios:
### Validation Errors (400 Bad Request)
```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "details": [
    "Name: Name is required",
    "Email: Invalid email format",
    "Age: Age must be between 1 and 150"
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Not Found (404)
```json
{
  "statusCode": 404,
  "message": "User with ID '999' was not found",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Setup & Run
Requirements

- .NET 9 SDK

- Docker & Docker Compose

Run locally
```bash
# Start databases
docker-compose up -d

# Run backend
dotnet run --project RESTful
```
The API will be available at:

Swagger UI: http://localhost:5287/swagger

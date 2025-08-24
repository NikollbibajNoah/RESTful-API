# RESTful-API

This project is a **RESTful API** built with **.NET 9** using **JWT authentication**.  
It is a learning project to practice authentication, authorization, and building a
clean backend structure with Docker.

---

## Table of Contents
- [Features](#features)
- [Technologies](#technologies)
- [Architecture](#architecture)
- [Structure](#structure)
- [Authentication](#authentification)
- [Setup & Run](#setup--run)

---

## Features
- CRUD operations
- User registration & login with JWT
- Protected endpoints with `[Authorize]`
- Two separate databases:
    - **MySQL** → business data (e.g. users / entities)
    - **PostgreSQL** → authentication (user credentials)
- Swagger UI with JWT support
- Exception handling middleware
- Docker setup for MySQL & PostgreSQL

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
    { "type": "role", "value": "User" },
    ...
  ]
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
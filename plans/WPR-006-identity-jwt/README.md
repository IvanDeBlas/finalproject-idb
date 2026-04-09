# Plan: WPR-006 - Identity JWT Authentication

**Fecha:** 2026-02-12
**Tipo:** Infraestructura (backend only)
**Estado actual:** ~70% implementado, falta Login + Roles

---

## Resumen

Completar el sistema de autenticacion minimo con ASP.NET Core Identity + JWT para WePlay Rises.

### Lo que YA existe
- `POST /api/auth/register` (RegisterCommand + Handler + Validator)
- JwtTokenGenerator (genera tokens sin roles)
- UserAccessContext (IdentityDbContext)
- ServiceResponseMessageType constants

### Lo que FALTA implementar
1. **LoginCommand** - `POST /api/auth/login` con email/password
2. **GetCurrentUserQuery** - `GET /api/auth/me` con [Authorize]
3. **Roles** - Constantes (Artista, Fan, Admin) + seed en DB
4. **JWT con roles** - Incluir role claims en token
5. **Migracion Identity** - Crear tablas AspNetUsers, AspNetRoles, etc.

---

## Archivos del Plan

### Backend (5 archivos)

| Archivo | Descripcion |
|---------|-------------|
| [`api-contracts.md`](backend/api-contracts.md) | DTOs request/response, validaciones, error codes, OpenAPI docs |
| [`hexagonal-architecture.md`](backend/hexagonal-architecture.md) | Domain + Infrastructure: archivos nuevos/modificar, diagramas, decisiones |
| [`cqrs-plan.md`](backend/cqrs-plan.md) | Application layer: LoginCommand, GetCurrentUserQuery, Validators, Handlers |
| [`newman-tests.md`](backend/newman-tests.md) | Estrategia de tests de integracion: 18 requests, 30+ casos de prueba |
| [`postman-collection.json`](backend/postman-collection.json) | Coleccion Postman ejecutable con Newman |

---

## Archivos a Crear (3)

| Archivo | Capa |
|---------|------|
| `UserAccess.Domain/Constants/Roles.cs` | Domain |
| `UserAccess.Application/Features/Auth/Commands/LoginCommand.cs` | Application |
| `UserAccess.Application/Features/Auth/Validators/LoginCommandValidator.cs` | Application |

## Archivos a Modificar (5)

| Archivo | Cambio |
|---------|--------|
| `ServiceResponseMessageType.cs` | +3 constantes Auth |
| `IJwtTokenGenerator.cs` | Agregar parametro `roles` |
| `JwtTokenGenerator.cs` | Incluir role claims |
| `RegisterCommand.cs` | Asignar rol Fan + pasar roles a token |
| `AuthController.cs` | +2 endpoints (login, me) |

## WebApi

| Archivo | Cambio |
|---------|--------|
| `Program.cs` | Seed de roles al iniciar app |

---

## Tiempo Estimado

| Tarea | Tiempo |
|-------|--------|
| Domain (constantes) | 5 min |
| Application (LoginCommand, Validator) | 15 min |
| Infrastructure (JwtTokenGenerator) | 10 min |
| WebApi (Controller, Program.cs) | 10 min |
| Migracion + seed | 10 min |
| Testing manual | 15 min |
| **Total** | **~65 min** |

---

## Decisiones Clave

1. **Reusar RegisterResponseDto para Login** - MVP, mismo shape de respuesta
2. **SignInManager para validar password** - Mejor practica vs PasswordHasher
3. **Seed roles en Program.cs** - Simple para MVP (no migracion)
4. **Sin refresh token** - Token 24h, re-login manual
5. **Sin confirmacion email** - Flujo rapido para MVP

---

## Siguiente Paso

```bash
/implement WPR-006-identity-jwt --target backend
```

O implementar manualmente siguiendo el orden:
1. Domain (Roles.cs, ServiceResponseMessageType)
2. Application (IJwtTokenGenerator, LoginCommand, Validator)
3. Infrastructure (JwtTokenGenerator)
4. WebApi (AuthController, Program.cs)
5. Migration + test

# WePlay Rises

Plataforma de crowdfunding musical para grupos noveles.

---

## Indice

1. [Ficha del Proyecto](#1-ficha-del-proyecto)
2. [Descripcion del Producto](#2-descripcion-del-producto)
3. [Arquitectura del Sistema](#3-arquitectura-del-sistema)
4. [Modelo de Datos](#4-modelo-de-datos)
5. [Especificacion de la API](#5-especificacion-de-la-api)
6. [Historias de Usuario](#6-historias-de-usuario)
7. [Tickets de Trabajo](#7-tickets-de-trabajo)
8. [Pull Requests](#8-pull-requests)
9. [Registro de Tiempo](#9-registro-de-tiempo)

---

## 1. Ficha del Proyecto

| Campo | Valor |
|-------|-------|
| **Nombre** | WePlay Rises |
| **Descripcion** | Plataforma de crowdfunding y crowdsourcing para grupos musicales noveles |
| **Autor** | Ivan de Blas |
| **Curso** | AI4Devs - LIDR |
| **Fecha inicio** | Enero 2026 |
| **Tiempo total** | 30 horas |
| **Deadline** | Fin de curso LIDR |
| **Enfoque MVP** | Flujo E2E Crowdfunding |
| **URL Produccion** | _Pendiente de deploy_ |
| **Repositorio** | Este repositorio |

### Stack Tecnologico

| Capa | Tecnologia |
|------|------------|
| Backend | .NET 8, ASP.NET Core, EF Core, CQRS, MediatR, FluentValidation |
| Landing (web) | Vite, React 18, TypeScript, Tailwind CSS, shadcn/ui |
| Dashboard (admin) | Next.js 14, React 18, TypeScript, Tailwind CSS, shadcn/ui |
| Shared | TypeScript, Zod schemas, utils compartidos |
| Base de datos | SQL Server (LocalDB dev, Azure SQL prod) |
| Auth | JWT + ASP.NET Core Identity |
| Infraestructura | Azure App Service, Azure Static Web Apps |
| CI/CD | Azure DevOps Pipelines |

### Restricciones del Proyecto

- **Tiempo total disponible**: 30 horas
- **Prioridad**: MVP funcional sobre perfeccion
- **Enfoque**: Flujo E2E > Tests basicos > Deploy Azure
- **Pospuesto**: Optimizaciones, features secundarias

---

## 2. Descripcion del Producto

### Vision

WePlay Rises es una plataforma de crowdfunding musical que integra **tres dimensiones** en un mismo ecosistema:

1. **Crowdfunding**: Los fans financian proyectos musicales a cambio de recompensas exclusivas
2. **Crowdsourcing**: Los artistas publican necesidades profesionales (productor, disenador, ingeniero de mezcla) y los profesionales envian propuestas
3. **Crowdpromotion**: Los promotores ayudan a difundir campanas a cambio de comisiones por cada backer referido

Lo innovador es que **un mismo proyecto puede combinar las tres dimensiones**, y el usuario lo ve de forma integrada en una sola interfaz.

### Problema que Resuelve

Los grupos musicales emergentes enfrentan tres grandes barreras:
- **Falta de financiacion** para producir y promocionar su musica
- **Dificultad para encontrar profesionales** asequibles
- **Alcance limitado** para llegar a nuevos fans

### Propuesta de Valor

Una plataforma integral donde artistas pueden financiar, producir y promocionar sus proyectos musicales con el apoyo de su comunidad. Un mismo proyecto artistico actua como hub que conecta las tres dimensiones.

### Usuarios Objetivo

| Persona | Descripcion | Necesidad Principal |
|---------|-------------|---------------------|
| **Artista** | Banda o solista emergente | Financiar y producir su musica |
| **Fan** | Seguidor de musica independiente | Apoyar a sus artistas favoritos |
| **Profesional** | Freelancer del sector musical | Encontrar proyectos y enviar propuestas |
| **Promotor** | Influencer o fan activo | Difundir campanas a cambio de comisiones |

### Funcionalidades Implementadas

#### Crowdfunding (nucleo)
| Feature | Estado |
|---------|--------|
| Registro y login de usuarios (JWT + Identity) | Completada |
| Crear perfil de artista | Completada |
| Crear campania de crowdfunding (wizard 4 pasos) | Completada |
| Definir recompensas con stock | Completada |
| Publicar campania | Completada |
| Ver listado de campanias activas con filtros y busqueda | Completada |
| Ver detalle de campania con progreso, tabs y badges | Completada |
| Hacer backing con seleccion de recompensa | Completada |
| Dashboard de artista con metricas | Completada |
| Comentarios de backers y updates de artistas | Completada |
| Stretch Goals | Completada |

#### Crowdsourcing
| Feature | Estado |
|---------|--------|
| Templates y guia para artistas noveles | Completada |
| Gestionar necesidades profesionales | Completada |
| Explorar y enviar propuestas | Completada |
| Acuerdos de trabajo y entregables | Completada |
| Mensajeria entre artista y profesional | Completada |
| Valoraciones | Completada |

#### Crowdpromotion
| Feature | Estado |
|---------|--------|
| Perfil de promotor | Completada |
| Programas de promocion | Completada |
| Inscripcion a programas y tareas | Completada |
| Tracking y metricas de promocion | Completada |
| Wallet y comisiones | Completada |

---

## 3. Arquitectura del Sistema

### Diagrama de Alto Nivel

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              CLIENTES                                    │
├─────────────────┬─────────────────────────────┬─────────────────────────┤
│                 │                             │                         │
│   Landing Web   │      Dashboard Admin        │      API REST           │
│  (Vite+React)   │      (Next.js 14)           │      (Swagger)          │
│   :3000         │        :3001                │                         │
│                 │                             │                         │
└────────┬────────┴──────────────┬──────────────┴────────────┬────────────┘
         │                       │                           │
         │         HTTPS/JWT     │                           │
         └───────────────────────┼───────────────────────────┘
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                         BACKEND .NET 8                                   │
│                    (Monolito Modular + CQRS)                            │
│                          :7001                                           │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐  │
│  │    Core      │  │  UserAccess  │  │ Crowdfunding │  │  WebApi     │  │
│  │   (Maestras) │  │  (Artista)   │  │  (Campania)  │  │ (Auth+DI)   │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  └─────────────┘  │
│                                                                          │
│  ┌──────────────────────────────────────────────────────────────────┐   │
│  │                      BuildingBlocks                               │   │
│  │  Kernel │ Abstractions │ EntityFramework │ Caching │ StronglyIds │   │
│  └──────────────────────────────────────────────────────────────────┘   │
│                                                                          │
└───────────────────────────────────┬─────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                         SQL SERVER                                       │
│                  (LocalDB dev / Azure SQL prod)                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### Estructura del Proyecto

```
WePlay_Rises/
├── src/
│   ├── api/                           # Backend .NET 8
│   │   ├── BuildingBlocks/            # Infraestructura compartida
│   │   │   ├── Abstractions/          # IRepository, MasterTableBase
│   │   │   ├── Caching/               # RequestCacheService
│   │   │   ├── EntityFramework/       # CoreDbContext, RepositoryBase
│   │   │   ├── Kernel/                # ServiceResponse<T>
│   │   │   └── StronglyTypedIds/      # ArtistaId, CampaniaCrowdfundingId
│   │   │
│   │   ├── Modules/
│   │   │   ├── Core/                  # Maestras (estados, monedas, tipos)
│   │   │   │   ├── Core.Domain/
│   │   │   │   ├── Core.Application/
│   │   │   │   ├── Core.Infra/
│   │   │   │   └── Core.WebApi/
│   │   │   │
│   │   │   ├── UserAccess/            # Artista, FanProfile
│   │   │   │   ├── UserAccess.Domain/
│   │   │   │   ├── UserAccess.Application/
│   │   │   │   ├── UserAccess.Infra/
│   │   │   │   └── UserAccess.WebApi/
│   │   │   │
│   │   │   ├── Crowdfunding/          # Campania, Reward, Backing
│   │   │   │   ├── Crowdfunding.Domain/
│   │   │   │   ├── Crowdfunding.Application/
│   │   │   │   ├── Crowdfunding.Infra/
│   │   │   │   └── Crowdfunding.WebApi/
│   │   │   │
│   │   │   ├── Crowdsourcing/         # Necesidades, Propuestas, Acuerdos
│   │   │   └── Crowdpromotion/        # Programas, Inscripciones, Wallet
│   │   │
│   │   └── WebApi/                    # Composition root
│   │       ├── Auth/                  # JWT + Identity
│   │       ├── Program.cs
│   │       └── appsettings.json
│   │
│   ├── shared/                        # Codigo compartido Frontend
│   │   ├── types/                     # TypeScript types
│   │   ├── schemas/                   # Zod validation
│   │   ├── constants/                 # QUERY_KEYS
│   │   └── utils/                     # cn(), formatCurrency()
│   │
│   ├── web/                           # Landing publica (Vite + React)
│   │   └── src/
│   │       ├── app/
│   │       ├── components/
│   │       ├── features/              # Arquitectura hexagonal
│   │       └── hooks/
│   │
│   └── admin/                         # Dashboard artista (Next.js 14)
│       └── src/
│           ├── app/                   # App Router
│           ├── components/
│           ├── services/
│           └── hooks/
│
├── docs/
│   ├── architecture/adrs/             # Decisiones arquitectonicas
│   ├── product/                       # User Stories
│   └── specs/                         # Especificaciones detalladas
│
├── tasks/
│   ├── backlog.md                     # Tareas detalladas
│   └── time-log.md                    # Registro de tiempo
│
├── .claude/                           # Sistema de tooling Claude
│   ├── rules/                         # Reglas contextuales
│   ├── commands/                      # Comandos slash
│   └── templates/                     # Templates de codigo
│
├── CLAUDE.md                          # Instrucciones para Claude
└── README.md                          # Este archivo
```

### Decisiones Arquitectonicas (ADRs)

| ADR | Titulo | Estado | Justificacion |
|-----|--------|--------|---------------|
| ADR-001 | DDD + Arquitectura Hexagonal | Aceptada | Manejo estructurado de complejidad del dominio |
| ADR-002 | CQRS con MediatR | Aceptada | Separacion clara Commands/Queries |
| ADR-003 | Modular Monolith | Aceptada | MVP sin overhead de microservicios |
| ADR-004 | JWT + Identity | Aceptada | Auth stateless compatible con SPA |
| ADR-005 | Strongly Typed IDs | Aceptada | Type safety: ArtistaId ≠ CampaniaId |
| ADR-006 | Caching Strategy | Aceptada | Evitar queries duplicadas por request |

> Detalle completo en `docs/architecture/adrs/`

### Arquitectura por Capas (Backend)

Cada modulo sigue la estructura DDD + Hexagonal:

```
Modulo/
├── Domain/              # Nucleo - Entidades, Value Objects
│   ├── Model/           # Entidades de dominio
│   ├── Constants/       # ServiceResponseMessageType
│   └── Interfaces/      # Contratos de repositorios
│
├── Application/         # Casos de uso - CQRS
│   ├── Features/
│   │   └── {Entidad}/
│   │       ├── Commands/   # Create, Update, Delete
│   │       ├── Queries/    # GetById, GetAll
│   │       ├── Validators/ # FluentValidation
│   │       └── Dtos/       # Data Transfer Objects
│   ├── Mapping/         # AutoMapper Profiles
│   └── Interfaces/      # Service interfaces
│
├── Infra/               # Adaptadores externos
│   ├── Context/         # DbContext EF Core
│   ├── Repositories/    # Implementacion repos
│   ├── Services/        # Implementacion services
│   └── DependencyInjection.cs
│
└── WebApi/              # Adaptador HTTP
    └── Controllers/     # Endpoints REST
```

### Flujo de Datos (Request -> Response)

```
[HTTP Request]
      │
      ▼
[Controller] ─────────────────────────────────────────┐
      │                                               │
      ▼                                               │
[MediatR.Send(Command/Query)]                         │
      │                                               │
      ▼                                               │
[Handler]                                             │
      │                                               │
      ├─► [Validator] ─► ValidationError? ──────────► │ ServiceResponse
      │                                               │  (con errores)
      ▼                                               │
[Service] ─────────────────────────────────────────┐  │
      │                                            │  │
      ▼                                            │  │
[RequestCache] ─► Hit? ─► Return cached            │  │
      │                                            │  │
      ▼ Miss                                       │  │
[Repository]                                       │  │
      │                                            │  │
      ▼                                            │  │
[DbContext] ────► SQL Server                       │  │
      │                                            │  │
      ▼                                            │  │
[Entity] ◄─────────────────────────────────────────┘  │
      │                                               │
      ▼                                               │
[AutoMapper] ─► DTO                                   │
      │                                               │
      ▼                                               │
[ServiceResponse<DTO>] ◄──────────────────────────────┘
      │
      ▼
[HTTP Response 200/400/404]
```

---

## 4. Modelo de Datos

### Diagrama ER Completo

```
┌─────────────────────┐
│   IdentityUser      │  (ASP.NET Core Identity)
│─────────────────────│
│ Id (string/Guid)    │
│ Email               │
│ PasswordHash        │
│ NombreCompleto      │
└──────────┬──────────┘
           │
           ├────────────────────┬──────────────────────┐
           │                    │                      │
           ▼                    ▼                      ▼
    ┌──────────────┐   ┌───────────────┐   ┌──────────────────┐
    │   Artista    │   │  FanProfile   │   │PerfilProfesional │
    │──────────────│   │───────────────│   │──────────────────│
    │ Id (STID)    │   │ Id (STID)     │   │ Id (STID)        │
    │ UserId (FK)  │   │ UserId (FK)   │   │ UserId (FK)      │
    │ NombreArtist │   │ NombreCompleto│   │ Especialidades   │
    │ Descripcion  │   │ PaisResidencia│   │ Portfolio        │
    │ Pais, Ciudad │   │ Ciudad        │   │ (Post-MVP)       │
    │ URLs Sociales│   │               │   │                  │
    └──────┬───────┘   └───────┬───────┘   └──────────────────┘
           │                   │
           │                   │
           ▼                   │
┌─────────────────────────┐    │
│  CampaniaCrowdfunding   │◄───┘ (via Backing)
│─────────────────────────│
│ Id (STID)               │
│ ArtistaId (FK)          │
│ Titulo                  │
│ Descripcion             │
│ ImporteObjetivo         │
│ ImportePledgedActual    │
│ EstadoId (FK)           │
│ MonedaId (FK)           │
│ FechaInicio, FechaFin   │
└────────────┬────────────┘
             │
    ┌────────┼────────┐
    │        │        │
    ▼        │        ▼
┌────────────────┐    │    ┌─────────────────────┐
│ CampaniaCrowd. │    │    │ CampaniaCrowd.      │
│    Reward      │    │    │    Update           │
│────────────────│    │    │─────────────────────│
│ Id (STID)      │    │    │ Id (STID)           │
│ CampaniaId(FK) │    │    │ CampaniaId (FK)     │
│ Nombre         │    │    │ Titulo              │
│ ImporteMinimo  │    │    │ Contenido           │
│ CantidadMaxima │    │    │ FechaPublicacion    │
│ TiempoEntrega  │    │    └─────────────────────┘
└───────┬────────┘    │
        │             │
        ▼             │
┌────────────────────────────┐
│   PedidoCrowdfunding       │ (Backing)
│────────────────────────────│
│ Id (STID)                  │
│ CampaniaId (FK)            │
│ UserId (FK, nullable)      │
│ RewardId (FK, nullable)    │
│ ImporteTotal               │
│ EsAnonimo                  │
│ Mensaje                    │
└────────────┬───────────────┘
             │
             ▼
┌────────────────────────────┐
│   AportacionCrowdfunding   │
│────────────────────────────│
│ Id (STID)                  │
│ PedidoId (FK)              │
│ Monto                      │
│ MetodoPagoId (FK)          │
│ EstadoId (FK)              │
└────────────────────────────┘

MAESTRAS (Tablas de Referencia):
├── MaestraEstadoCampaniaCrowd (borrador, activa, finalizada, cancelada)
├── MaestraEstadoPedidoCrowd (pendiente, confirmado, entregado, cancelado)
├── MaestraMoneda (EUR, USD, GBP)
├── MaestraTipoFinanciacion (all-or-nothing, keep-it-all)
├── MaestraTipoReward (digital, fisico, experiencia)
└── MaestraMetodoPago (tarjeta, transferencia, paypal)
```

### Entidades Principales

| Entidad | Modulo | Descripcion | Campos Clave |
|---------|--------|-------------|--------------|
| **Artista** | UserAccess | Perfil de banda/solista | UserId, NombreArtistico, Descripcion, Pais, URLs |
| **FanProfile** | UserAccess | Perfil de fan/backer | UserId, NombreCompleto, PaisResidencia |
| **CampaniaCrowdfunding** | Crowdfunding | Campania de financiacion | ArtistaId, Titulo, ImporteObjetivo, Estado, Fechas |
| **CampaniaCrowdfundingReward** | Crowdfunding | Niveles de recompensa | CampaniaId, Nombre, ImporteMinimo, CantidadMaxima |
| **PedidoCrowdfunding** | Crowdfunding | Backing de un fan | CampaniaId, UserId, RewardId, ImporteTotal |
| **AportacionCrowdfunding** | Crowdfunding | Registro de pago | PedidoId, Monto, MetodoPagoId, Estado |

### Modelo de Datos Detallado

#### Artista (UserAccess)
```csharp
public class Artista
{
    public ArtistaId Id { get; set; }
    public string UserIdPropietario { get; set; }  // FK a Identity
    public string NombreArtistico { get; set; }    // max 200
    public string? Descripcion { get; set; }       // max 2000
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }
    public string? UrlSitioWeb { get; set; }
    public string? UrlInstagram { get; set; }
    public string? UrlYouTube { get; set; }
    public string? UrlSpotify { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }

    // Navigation
    public ICollection<CampaniaCrowdfunding> Campanias { get; set; }
}
```

#### CampaniaCrowdfunding (Crowdfunding)
```csharp
public class CampaniaCrowdfunding
{
    public CampaniaCrowdfundingId Id { get; set; }
    public ArtistaId ArtistaId { get; set; }
    public string Titulo { get; set; }             // max 255
    public string? Subtitulo { get; set; }
    public string Descripcion { get; set; }        // rich text
    public decimal ImporteObjetivo { get; set; }
    public decimal ImportePledgedActual { get; set; } = 0;
    public int MonedaId { get; set; }              // FK Maestra
    public int TipoFinanciacionId { get; set; }    // FK Maestra
    public int EstadoCampaniaId { get; set; }      // FK Maestra
    public string? ImagenPrincipalUrl { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public bool Borrado { get; set; } = false;     // soft delete
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }

    // Navigation
    public Artista Artista { get; set; }
    public ICollection<CampaniaCrowdfundingReward> Rewards { get; set; }
    public ICollection<PedidoCrowdfunding> Pedidos { get; set; }
}
```

#### Reward (Crowdfunding)
```csharp
public class CampaniaCrowdfundingReward
{
    public CampaniaCrowdfundingRewardId Id { get; set; }
    public CampaniaCrowdfundingId CampaniaId { get; set; }
    public string Nombre { get; set; }              // max 100
    public string Descripcion { get; set; }         // max 1000
    public decimal ImporteMinimo { get; set; }
    public int? CantidadMaxima { get; set; }        // null = ilimitado
    public int CantidadVendida { get; set; } = 0;
    public string? TiempoEntregaEstimado { get; set; }
    public int Orden { get; set; }
    public bool EsActivo { get; set; } = true;

    // Computed
    public int? CantidadDisponible => CantidadMaxima.HasValue
        ? CantidadMaxima.Value - CantidadVendida
        : null;
}
```

#### Backing/Pedido (Crowdfunding)
```csharp
public class PedidoCrowdfunding
{
    public PedidoCrowdfundingId Id { get; set; }
    public CampaniaCrowdfundingId CampaniaId { get; set; }
    public string? UserId { get; set; }             // null = anonimo
    public CampaniaCrowdfundingRewardId? RewardId { get; set; }
    public int EstadoPedidoId { get; set; }
    public decimal ImporteTotal { get; set; }
    public string? Mensaje { get; set; }            // max 500
    public bool EsAnonimo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

### Estados de Campania

```
┌─────────────┐
│  BORRADOR   │ ◄── Creacion inicial
└──────┬──────┘
       │ [publicar]
       ▼
┌─────────────┐
│  PUBLICADA  │ ◄── Visible, esperando fecha inicio
└──────┬──────┘
       │ [fecha inicio]
       ▼
┌─────────────┐
│   EN_CURSO  │ ◄── Aceptando backings
└──────┬──────┘
       │ [fecha fin]
       │
       ├────────────────┬────────────────┐
       │                │                │
       ▼                ▼                ▼
┌──────────────┐ ┌──────────────┐ ┌────────────┐
│ COMPLETADA   │ │ COMPLETADA   │ │ CANCELADA  │
│    EXITO     │ │    FALLO     │ │            │
└──────────────┘ └──────────────┘ └────────────┘
 (meta alcanzada) (meta no alcanzada) (artista cancela)
```

---

## 5. Especificacion de la API

### Formato de Respuesta Estandar

Todos los endpoints retornan `ServiceResponse<T>`:

```json
{
  "data": { /* payload */ },
  "messages": [
    {
      "message": "Mensaje descriptivo",
      "errorCode": "0000",
      "httpStatusCode": 200,
      "propertyName": null
    }
  ]
}
```

**Rangos de ErrorCodes:**
| Rango | Categoria | Ejemplo |
|-------|-----------|---------|
| 0000-0999 | Success | 0000=Success, 0001=Created |
| 1000-1999 | Validation | 1001=Required, 1002=MaxLength |
| 2000-2999 | Not Found | 2002=Artista_NotFound |
| 3000-3999 | Auth | 3001=Unauthorized |
| 4000-4999 | Business Rules | 4008=DuplicateName |
| 5000-5999 | Internal | 5000=UnexpectedError |

### Endpoints de Autenticacion

| Metodo | Endpoint | Descripcion | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Registro de usuario | No |
| POST | `/api/auth/login` | Login (devuelve JWT) | No |

#### POST /api/auth/register
```json
// Request
{
  "email": "banda@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}

// Response 201 Created
{
  "data": {
    "userId": "guid",
    "email": "banda@example.com",
    "token": "eyJhbGciOiJIUzI1NiIs..."
  },
  "messages": [{ "message": "Usuario registrado", "errorCode": "0001" }]
}

// Errores: 400 Validacion, 409 Email duplicado
```

### Endpoints de Artistas

| Metodo | Endpoint | Descripcion | Auth |
|--------|----------|-------------|------|
| GET | `/api/artistas` | Listar artistas | No |
| GET | `/api/artistas/{id}` | Detalle de artista | No |
| POST | `/api/artistas` | Crear perfil artista | Si |
| PUT | `/api/artistas/{id}` | Actualizar perfil | Si |
| GET | `/api/artistas/by-user/{userId}` | Artista por user | Si |

#### POST /api/artistas
```json
// Request (Header: Authorization: Bearer {token})
{
  "nombreArtistico": "Los Rockeros",
  "descripcion": "Banda de rock alternativo de Madrid",
  "pais": "Espana",
  "ciudad": "Madrid",
  "imagenUrl": "https://..."
}

// Response 201 Created
{
  "data": {
    "id": "guid",
    "nombreArtistico": "Los Rockeros",
    "userId": "guid"
  },
  "messages": [...]
}
```

### Endpoints de Campanias

| Metodo | Endpoint | Descripcion | Auth |
|--------|----------|-------------|------|
| GET | `/api/campanias` | Listar campanias publicas | No |
| GET | `/api/campanias/{id}` | Detalle de campania | No |
| POST | `/api/campanias` | Crear campania | Si |
| PUT | `/api/campanias/{id}` | Actualizar campania | Si |
| POST | `/api/campanias/{id}/publicar` | Publicar campania | Si |
| GET | `/api/campanias/mis-campanias` | Mis campanias (artista) | Si |

#### POST /api/campanias
```json
// Request
{
  "titulo": "Primer Album de Los Rockeros",
  "subtitulo": "Ayudanos a grabar nuestro sueno",
  "descripcion": "<p>Somos una banda de Madrid...</p>",
  "importeObjetivo": 5000.00,
  "monedaId": 1,
  "tipoFinanciacionId": 1,
  "fechaFin": "2026-03-21",
  "imagenPrincipalUrl": "https://...",
  "videoPrincipalUrl": "https://youtube.com/..."
}

// Response 201 Created
{
  "data": {
    "id": "guid",
    "titulo": "Primer Album de Los Rockeros",
    "estadoCampaniaId": 1,
    "estadoCampaniaNombre": "Borrador"
  }
}
```

### Endpoints de Rewards

| Metodo | Endpoint | Descripcion | Auth |
|--------|----------|-------------|------|
| GET | `/api/campanias/{id}/rewards` | Listar rewards | No |
| POST | `/api/campanias/{id}/rewards` | Crear reward | Si |
| PUT | `/api/rewards/{id}` | Actualizar reward | Si |
| DELETE | `/api/rewards/{id}` | Eliminar reward | Si |

#### POST /api/campanias/{id}/rewards
```json
// Request
{
  "nombre": "CD Firmado",
  "descripcion": "CD fisico del album firmado por todos los miembros",
  "importeMinimo": 25.00,
  "cantidadMaxima": 100,
  "tiempoEntregaEstimado": "Marzo 2026"
}

// Response 201 Created
{
  "data": {
    "id": "guid",
    "nombre": "CD Firmado",
    "importeMinimo": 25.00,
    "cantidadDisponible": 100
  }
}
```

### Endpoints de Backings

| Metodo | Endpoint | Descripcion | Auth |
|--------|----------|-------------|------|
| POST | `/api/campanias/{id}/backings` | Hacer backing | Opcional |
| GET | `/api/campanias/{id}/backings` | Listar backings (artista) | Si |
| GET | `/api/backings/mis-backings` | Mis backings (fan) | Si |

#### POST /api/campanias/{id}/backings
```json
// Request
{
  "rewardId": "guid",   // opcional, null = sin recompensa
  "monto": 25.00,
  "mensaje": "Exitos con el album!",
  "esAnonimo": false
}

// Response 201 Created
{
  "data": {
    "id": "guid",
    "campaniaTitulo": "Primer Album de Los Rockeros",
    "rewardNombre": "CD Firmado",
    "monto": 25.00
  }
}

// Errores: 400 Monto invalido, 409 Sin stock, 422 Campania no activa
```

### Endpoints de Dashboard

| Metodo | Endpoint | Descripcion | Auth |
|--------|----------|-------------|------|
| GET | `/api/dashboard/resumen` | Resumen general artista | Si |
| GET | `/api/campanias/{id}/stats` | Estadisticas campania | Si |

---

## 6. Historias de Usuario

### US-01: Registro de Artista

| Campo | Valor |
|-------|-------|
| **ID** | US-01 |
| **Prioridad** | Alta |
| **Sprint** | 1 |
| **Tarea** | WPR-010 |

**Como** artista **quiero** registrarme y crear mi perfil **para** presentar mi proyecto musical.

#### Flujo Principal
```
Usuario accede a /auth/register
    ↓
Completa formulario: email, password, confirmar password
    ↓
Sistema crea cuenta Identity + JWT
    ↓
Redirige a /artista/perfil/crear
    ↓
Completa formulario: nombre artistico, descripcion, pais, ciudad, imagen
    ↓
Sistema crea perfil Artista vinculado al User
    ↓
Redirige a /dashboard
```

#### Criterios de Aceptacion
| ID | Criterio |
|----|----------|
| AC-01-1 | Email unico en el sistema |
| AC-01-2 | Password minimo 8 caracteres |
| AC-01-3 | Nombre artistico obligatorio (max 200) |
| AC-01-4 | Imagen es URL valida (opcional) |
| AC-01-5 | Perfil visible en /artistas/{id} |

---

### US-02: Crear Campania

| Campo | Valor |
|-------|-------|
| **ID** | US-02 |
| **Prioridad** | Alta |
| **Sprint** | 1 |
| **Tarea** | WPR-011 |

**Como** artista **quiero** crear una campania de crowdfunding **para** financiar mi proyecto.

#### Flujo Principal (Wizard 4 pasos)
```
Paso 1: Info basica (titulo, subtitulo, descripcion)
    ↓
Paso 2: Financiacion (meta, moneda, tipo)
    ↓
Paso 3: Fechas (inicio opcional, fin obligatorio)
    ↓
Paso 4: Media (imagen, video)
    ↓
Preview → Guardar como BORRADOR o Publicar
```

#### Criterios de Aceptacion
| ID | Criterio |
|----|----------|
| AC-02-1 | Campania se crea en estado BORRADOR |
| AC-02-2 | Meta > 0 |
| AC-02-3 | Fecha fin >= 7 dias desde hoy |
| AC-02-4 | Fecha fin <= 60 dias desde hoy |
| AC-02-5 | Solo el artista dueno puede editar |
| AC-02-6 | Al publicar, estado cambia a PUBLICADA |

---

### US-03: Definir Recompensas

| Campo | Valor |
|-------|-------|
| **ID** | US-03 |
| **Prioridad** | Alta |
| **Sprint** | 1 |
| **Tarea** | WPR-012 |

**Como** artista **quiero** anadir recompensas a mi campania **para** incentivar las aportaciones.

#### Criterios de Aceptacion
| ID | Criterio |
|----|----------|
| AC-03-1 | Minimo 1 recompensa recomendado para publicar |
| AC-03-2 | Monto minimo > 0 |
| AC-03-3 | Stock se decrementa al hacer backing |
| AC-03-4 | No se puede hacer backing si stock = 0 |
| AC-03-5 | Recompensas ordenables |
| AC-03-6 | No eliminar reward con backings |

#### Ejemplos de Rewards Tipicos
| Nivel | Nombre | Monto | Stock |
|-------|--------|-------|-------|
| 1 | Agradecimiento Digital | 5 EUR | Ilimitado |
| 2 | Descarga Digital Album | 10 EUR | Ilimitado |
| 3 | CD Fisico | 20 EUR | 200 |
| 4 | CD Firmado | 35 EUR | 100 |
| 5 | Vinilo Edicion Limitada | 50 EUR | 50 |
| 6 | Meet & Greet | 150 EUR | 20 |
| 7 | Concierto Privado | 500 EUR | 5 |

---

### US-04: Hacer Backing

| Campo | Valor |
|-------|-------|
| **ID** | US-04 |
| **Prioridad** | Alta |
| **Sprint** | 1 |
| **Tarea** | WPR-013 |

**Como** fan **quiero** apoyar una campania **para** ayudar al artista y obtener recompensas.

#### Flujo Principal
```
Fan accede a /campanias
    ↓
Ve listado con: imagen, titulo, progreso, dias restantes
    ↓
Click en campania → detalle completo
    ↓
Selecciona reward (o "Sin recompensa")
    ↓
Indica monto >= minimo
    ↓
Opcional: mensaje, marcar como anonimo
    ↓
Click "Apoyar" → Sistema crea Backing
    ↓
Actualiza ImportePledgedActual y stock
    ↓
Fan ve confirmacion
```

#### Criterios de Aceptacion
| ID | Criterio |
|----|----------|
| AC-04-1 | Solo campanias PUBLICADA/EN_CURSO visibles |
| AC-04-2 | Monto >= ImporteMinimo del reward |
| AC-04-3 | Progreso se actualiza en tiempo real |
| AC-04-4 | Stock se decrementa |
| AC-04-5 | Backers anonimos no muestran nombre |
| AC-04-6 | Sin recompensa permite cualquier monto > 0 |

---

### US-05: Dashboard Artista

| Campo | Valor |
|-------|-------|
| **ID** | US-05 |
| **Prioridad** | Alta |
| **Sprint** | 1 |
| **Tarea** | WPR-014 |

**Como** artista **quiero** ver el progreso de mi campania **para** conocer las metricas.

#### Vista Principal
```
┌──────────────────────────────────────────────────┐
│  RESUMEN GENERAL                                  │
│  ┌────────┐  ┌────────┐  ┌────────┐              │
│  │ €3,500 │  │   45   │  │   2    │              │
│  │ Total  │  │Backers │  │Activas │              │
│  └────────┘  └────────┘  └────────┘              │
│                                                   │
│  MIS CAMPANIAS                [+ Nueva]          │
│  ┌─────────────────────────────────────────────┐ │
│  │ Album Los Rockeros                          │ │
│  │ [========>          ] 25% - 45 dias         │ │
│  │ €1,250 / €5,000 - 15 backers                │ │
│  └─────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────┘
```

#### Criterios de Aceptacion
| ID | Criterio |
|----|----------|
| AC-05-1 | Solo ve sus propias campanias |
| AC-05-2 | Metricas actualizadas en tiempo real |
| AC-05-3 | Lista backers muestra los mas recientes |
| AC-05-4 | Backers anonimos muestran "Anonimo" |
| AC-05-5 | Porcentaje calculado correctamente |
| AC-05-6 | Dias restantes se actualizan |

---

## 7. Tickets de Trabajo

### Fase 1: Fundamentos (8h estimadas)

| ID | Titulo | Estado | Prioridad | Estimado |
|----|--------|--------|-----------|----------|
| WPR-001 | Definir modelo de datos MVP | ✅ Completada | Alta | 2h |
| WPR-002 | Copiar BuildingBlocks de miUrba | ✅ Completada | Alta | 2h |
| WPR-003 | Crear estructura de modulos MVP | ✅ Completada | Alta | 2h |
| WPR-004 | Configurar DbContext y migrations | ✅ Completada | Alta | 1h |
| WPR-005 | Setup proyecto React + Templates | ✅ Completada | Alta | 1h |
| WPR-006 | Implementar Identity minimo (JWT) | ✅ Completada | Alta | 1.5h |
| WPR-006a | Configurar User Secrets | ✅ Completada | Alta | 0.25h |

### Fase 2: Features MVP - Crowdfunding (12h estimadas)

| ID | Titulo | Estado | Prioridad | Estimado |
|----|--------|--------|-----------|----------|
| WPR-010 | US-01 - Registro de Artista | ✅ Completada | Alta | 2h |
| WPR-011 | US-02 - Crear Campania | ✅ Completada | Alta | 3h |
| WPR-012 | US-03 - Definir Recompensas | ✅ Completada | Alta | 2h |
| WPR-013 | US-04 - Hacer Backing | ✅ Completada | Alta | 2h |
| WPR-014 | US-05 - Dashboard Artista | ✅ Completada | Alta | 2h |

### Fase 3: Testing y Deploy (10h estimadas)

| ID | Titulo | Estado | Prioridad | Estimado |
|----|--------|--------|-----------|----------|
| WPR-020 | Tests unitarios backend | ✅ Completada | Media | 3h |
| WPR-021 | Tests de integracion | ✅ Completada | Media | 2h |
| WPR-022 | Deploy Azure | ✅ Completada | Media | 3h |
| WPR-023 | Documentacion final | ✅ Completada | Media | 2h |

### Fase 4: Crowdsourcing y Crowdpromotion

| ID | Titulo | Estado | Prioridad |
|----|--------|--------|-----------|
| US-CS-01 | Templates y guia para artistas | ✅ Completada | Alta |
| US-CS-02 | Gestionar necesidades | ✅ Completada | Alta |
| US-CS-03 | Explorar y enviar propuestas | ✅ Completada | Alta |
| US-CS-04 | Acuerdos y entregables | ✅ Completada | Media |
| US-CS-05 | Mensajeria | ✅ Completada | Media |
| US-CS-06 | Valoraciones | ✅ Completada | Media |
| US-CP-01 | Perfil promotor | ✅ Completada | Alta |
| US-CP-02 | Programas de promocion | ✅ Completada | Alta |
| US-CP-03 | Inscripcion y tareas | ✅ Completada | Alta |
| US-CP-04 | Tracking y metricas | ✅ Completada | Media |
| US-CP-05 | Wallet y comisiones | ✅ Completada | Media |

> Detalle completo en `tasks/backlog.md`

---

## 8. Pull Requests

El desarrollo se realizo en rama `master` con commits atomicos. Para la entrega se creo la rama `entrega/idb-final`.

---

## 9. Registro de Tiempo

### Resumen

| Metrica | Valor |
|---------|-------|
| **Total invertido** | ~30 horas |
| **Presupuesto total** | 30 horas |
| **Porcentaje usado** | 100% |

```
[#########################] 100% (~30/30h)
```

### Por Categoria

| Categoria | Horas aprox | % del Total |
|-----------|-------------|-------------|
| Planning y Arquitectura | 4.0 | 13% |
| Setup e Infraestructura | 4.0 | 13% |
| Backend (Crowdfunding) | 5.0 | 17% |
| Backend (Crowdsourcing + Crowdpromotion) | 4.0 | 13% |
| Frontend Landing | 5.0 | 17% |
| Frontend Admin | 3.0 | 10% |
| Testing | 2.0 | 7% |
| Deploy y Docker | 1.5 | 5% |
| Documentacion y Seed | 1.5 | 5% |

> Detalle completo en `tasks/time-log.md`

---

## Como Ejecutar

### Opcion recomendada: Docker Compose

Levanta todo el stack (SQL Server + API + Web + Admin) con un solo comando:

```bash
docker compose up -d
```

Servicios disponibles:

| Servicio | URL | Descripcion |
|----------|-----|-------------|
| Landing (web) | http://localhost:3000 | Vite + React, campanias publicas |
| Admin (dashboard) | http://localhost:3001 | Next.js 14, dashboard de artista |
| API Swagger | http://localhost:5001/swagger | Documentacion interactiva de la API |
| SQL Server | localhost:1433 | sa / WePlayRises2024! |

Para detener:
```bash
docker compose down       # Detener servicios
docker compose down -v    # Detener y borrar datos de BD
```

### Seed de datos de demo

Tras levantar Docker, ejecutar el script de seed para poblar la base de datos:

```bash
sqlcmd -S localhost,1433 -U sa -P "WePlayRises2024!" -d WePlayRises -i scripts/seed-complete.sql
```

### Credenciales de prueba

#### Artistas (password: `WePlay2026!`)

| Email | Artista | Dimensiones activas |
|-------|---------|---------------------|
| vetusta@weplay-test.com | Vetusta Morla | Funding + Sourcing + Promo |
| badbunny@weplay-test.com | Bad Bunny | Funding + Sourcing + Promo |
| rosalia@weplay-test.com | Rosalia | Funding + Sourcing |
| tangana@weplay-test.com | C. Tangana | Funding |

> Vetusta Morla tambien tiene rol Admin.

#### Fans (password: `Test123!`)

| Email | Nombre | Perfil Profesional | Perfil Promotor |
|-------|--------|--------------------|-----------------|
| maria.garcia@weplay-test.com | Maria Garcia | No | No |
| carlos.lopez@weplay-test.com | Carlos Lopez | Productor Musical | Si |
| emma.wilson@weplay-test.com | Emma Wilson | Ingeniera de Mezcla | Si |

### Opcion alternativa: Desarrollo local

#### Requisitos
- .NET 8 SDK
- Node.js 20+
- SQL Server (LocalDB)

#### Backend
```bash
cd src/api
dotnet user-secrets set "Jwt:Key" "WePlayRises_DevKey_SuperSecret_MinLength32Chars!" --project WebApi/WePlayRises.WebApi.csproj
dotnet restore && dotnet build WePlayRises.sln
dotnet run --project WebApi/WebApi.csproj
# API en https://localhost:5001/swagger
```

#### Frontend - Landing
```bash
cd src/web && npm install && npm run dev
# http://localhost:3000
```

#### Frontend - Admin
```bash
cd src/admin && npm install && npm run dev
# http://localhost:3001
```

---

## Como Ejecutar Tests

### Backend (xUnit)
```bash
cd src/api && dotnet test
```

### Frontend - Landing (Vitest)
```bash
cd src/web && npm run test:run
```

### Frontend - Landing E2E (Playwright)
```bash
cd src/web && npx playwright test e2e/smoke/ --project=chromium
```

### Frontend - Admin (Vitest)
```bash
cd src/admin && npm run test:run
```

### Frontend - Admin E2E (Playwright)
```bash
cd src/admin && npx playwright test --project=chromium
```

> Nota: Los tests E2E requieren Docker corriendo con datos seeded.

---

## Documentacion Adicional

| Documento | Ubicacion | Proposito |
|-----------|-----------|----------|
| ADRs | `docs/architecture/adrs/` | Decisiones arquitectonicas |
| User Stories | `docs/product/` | Especificaciones de features |
| Specs detalladas | `docs/specs/` | Contracts, UI/UX, implementation |
| Demo y video | `docs/20260324_demo-plataforma-weplay-rises.md` | Guion de demo y datos |
| Tareas | `tasks/` | Backlog, time-log, estados |
| Prompts IA | `prompts.md` | Metodologia de desarrollo con Claude Code |

---

## Licencia

Proyecto academico - Curso AI4Devs LIDR

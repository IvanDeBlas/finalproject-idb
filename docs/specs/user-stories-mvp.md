# WePlay Rises - Especificaciones de Historias de Usuario MVP

> **Version:** 1.0
> **Fecha:** 2026-01-21
> **Alcance:** Solo Crowdfunding (MVP)

---

## US-01: Registro de Artista

### Descripcion
**Como** artista, **quiero** registrarme y crear mi perfil **para** presentar mi proyecto musical.

### Actores
- Artista (usuario no autenticado → autenticado)

### Flujo Principal
1. Usuario accede a `/auth/register`
2. Completa formulario: email, password, confirmar password
3. Sistema crea cuenta en Identity
4. Usuario es redirigido a `/artista/perfil/crear`
5. Completa formulario: nombre artistico, descripcion, pais, ciudad, imagen URL
6. Sistema crea perfil de Artista vinculado al User
7. Usuario es redirigido a `/dashboard`

### Flujos Alternativos
- **1a.** Email ya existe → Mostrar error, sugerir login
- **5a.** Usuario omite perfil → Puede crearlo despues desde dashboard

### Criterios de Aceptacion
| ID | Criterio | Prueba |
|----|----------|--------|
| AC-01-1 | Email unico en el sistema | Intentar registrar email duplicado |
| AC-01-2 | Password minimo 8 caracteres | Validar en frontend y backend |
| AC-01-3 | Nombre artistico obligatorio | Form no envia sin nombre |
| AC-01-4 | Imagen es URL valida (opcional) | Acepta URLs o vacio |
| AC-01-5 | Perfil visible en `/artistas/{id}` | Navegar a perfil publico |

### API Endpoints

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
  "userId": "guid",
  "email": "banda@example.com",
  "token": "jwt..."
}
```

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
  "id": "guid",
  "nombreArtistico": "Los Rockeros",
  "userId": "guid"
}
```

### Modelo de Datos
```csharp
public class Artista
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;  // FK a Identity.User
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
```

### Validaciones (FluentValidation)
```csharp
RuleFor(x => x.NombreArtistico)
    .NotEmpty().WithMessage("El nombre artistico es obligatorio")
    .MaximumLength(200).WithMessage("Maximo 200 caracteres");

RuleFor(x => x.Descripcion)
    .MaximumLength(2000).WithMessage("Maximo 2000 caracteres");

RuleFor(x => x.ImagenUrl)
    .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ImagenUrl))
    .WithMessage("Debe ser una URL valida");
```

---

## US-02: Crear Campania de Crowdfunding

### Descripcion
**Como** artista, **quiero** crear una campania de crowdfunding **para** financiar mi proyecto musical.

### Actores
- Artista (autenticado, con perfil)

### Precondiciones
- Usuario tiene cuenta
- Usuario tiene perfil de Artista creado

### Flujo Principal
1. Artista accede a `/dashboard/campanias/nueva`
2. Completa Paso 1: Informacion basica
   - Titulo (obligatorio)
   - Subtitulo (opcional)
   - Descripcion (obligatorio, rich text)
3. Completa Paso 2: Financiacion
   - Meta financiera (obligatorio, > 0)
   - Moneda (EUR por defecto)
   - Tipo financiacion: "Todo o nada" / "Flexible"
4. Completa Paso 3: Fechas
   - Fecha inicio (opcional, default: al publicar)
   - Fecha fin (obligatorio, minimo 7 dias, maximo 60 dias)
5. Completa Paso 4: Media
   - Imagen principal (URL)
   - Video principal (URL YouTube/Vimeo, opcional)
6. Sistema crea campania en estado BORRADOR
7. Artista ve preview
8. Artista puede publicar o guardar para despues

### Flujos Alternativos
- **6a.** Guardar como borrador → Campania queda editable
- **8a.** Publicar → Estado cambia a PUBLICADA, fechas se activan

### Criterios de Aceptacion
| ID | Criterio | Prueba |
|----|----------|--------|
| AC-02-1 | Campania se crea en BORRADOR | Verificar estado en DB |
| AC-02-2 | Meta > 0 | Validar monto positivo |
| AC-02-3 | Fecha fin >= 7 dias desde hoy | Validar calendario |
| AC-02-4 | Fecha fin <= 60 dias desde hoy | Validar calendario |
| AC-02-5 | Solo el artista dueno puede editar | Intentar editar con otro user |
| AC-02-6 | Al publicar, estado cambia a PUBLICADA | Click publicar, verificar estado |

### API Endpoints

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
  "fechaInicio": null,
  "fechaFin": "2026-03-21",
  "imagenPrincipalUrl": "https://...",
  "videoPrincipalUrl": "https://youtube.com/..."
}

// Response 201 Created
{
  "id": "guid",
  "titulo": "Primer Album de Los Rockeros",
  "estadoCampaniaId": 1,
  "estadoCampaniaNombre": "Borrador"
}
```

#### POST /api/campanias/{id}/publicar
```json
// Response 200 OK
{
  "id": "guid",
  "estadoCampaniaId": 2,
  "estadoCampaniaNombre": "Publicada",
  "fechaPublicacion": "2026-01-21T10:30:00Z"
}
```

### Modelo de Datos
```csharp
public class Campania
{
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string Descripcion { get; set; } = null!;
    public decimal ImporteObjetivo { get; set; }
    public decimal ImportePledgedActual { get; set; } = 0;
    public int MonedaId { get; set; } = 1;  // EUR
    public int TipoFinanciacionId { get; set; }
    public int EstadoCampaniaId { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }

    // Navigation
    public Artista Artista { get; set; } = null!;
    public ICollection<Reward> Rewards { get; set; } = new List<Reward>();
    public ICollection<Backing> Backings { get; set; } = new List<Backing>();
}

public enum EstadoCampania
{
    Borrador = 1,
    Publicada = 2,
    EnCurso = 3,
    CompletadaExito = 4,
    CompletadaFallo = 5,
    Cancelada = 6
}

public enum TipoFinanciacion
{
    TodoONada = 1,
    Flexible = 2
}
```

---

## US-03: Definir Recompensas

### Descripcion
**Como** artista, **quiero** anadir recompensas a mi campania **para** incentivar las aportaciones.

### Actores
- Artista (dueno de la campania)

### Precondiciones
- Campania existe (cualquier estado)

### Flujo Principal
1. Artista accede a edicion de campania
2. Navega a seccion "Recompensas"
3. Click "Anadir recompensa"
4. Completa formulario:
   - Nombre (obligatorio)
   - Descripcion (obligatorio)
   - Monto minimo (obligatorio, > 0)
   - Stock limitado (checkbox)
   - Cantidad maxima (si stock limitado)
   - Tiempo estimado de entrega (texto)
5. Guarda recompensa
6. Puede reordenar recompensas (drag & drop)
7. Puede editar/eliminar recompensas

### Criterios de Aceptacion
| ID | Criterio | Prueba |
|----|----------|--------|
| AC-03-1 | Minimo 1 recompensa para publicar | Intentar publicar sin rewards |
| AC-03-2 | Monto minimo > 0 | Validar campo |
| AC-03-3 | Stock se decrementa al hacer backing | Crear backing, verificar stock |
| AC-03-4 | No se puede hacer backing si stock = 0 | Intentar backing sin stock |
| AC-03-5 | Recompensas ordenables | Verificar campo Orden |

### API Endpoints

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
  "id": "guid",
  "nombre": "CD Firmado",
  "importeMinimo": 25.00,
  "cantidadDisponible": 100,
  "orden": 1
}
```

### Modelo de Datos
```csharp
public class Reward
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal ImporteMinimo { get; set; }
    public int? CantidadMaxima { get; set; }  // null = ilimitado
    public int CantidadVendida { get; set; } = 0;
    public string? TiempoEntregaEstimado { get; set; }
    public int Orden { get; set; }
    public bool EsActivo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }

    // Computed
    public int? CantidadDisponible => CantidadMaxima.HasValue
        ? CantidadMaxima.Value - CantidadVendida
        : null;

    // Navigation
    public Campania Campania { get; set; } = null!;
}
```

---

## US-04: Hacer Backing (Apoyar Campania)

### Descripcion
**Como** fan, **quiero** apoyar una campania **para** ayudar al artista y obtener recompensas.

### Actores
- Fan (puede ser anonimo o autenticado)
- Sistema

### Precondiciones
- Campania esta PUBLICADA o EN_CURSO
- Fecha actual <= FechaFin de campania

### Flujo Principal
1. Fan accede a `/campanias`
2. Ve listado de campanias activas
3. Click en una campania
4. Ve detalle con:
   - Descripcion, video, imagenes
   - Barra de progreso (recaudado vs meta)
   - Lista de recompensas
   - Numero de backers
   - Dias restantes
5. Selecciona recompensa (o "Sin recompensa")
6. Indica monto (>= monto minimo de reward)
7. Opcional: mensaje para el artista
8. Opcional: hacer anonimo
9. Click "Apoyar"
10. Sistema crea Backing
11. Sistema actualiza ImportePledgedActual
12. Sistema decrementa stock si aplica
13. Fan ve confirmacion

### Criterios de Aceptacion
| ID | Criterio | Prueba |
|----|----------|--------|
| AC-04-1 | Solo campanias activas visibles | Listar solo PUBLICADA/EN_CURSO |
| AC-04-2 | Monto >= ImporteMinimo del reward | Validar monto |
| AC-04-3 | Progreso se actualiza en tiempo real | Verificar ImportePledgedActual |
| AC-04-4 | Stock se decrementa | Verificar CantidadVendida |
| AC-04-5 | Backers anonimos no muestran nombre | Verificar en lista publica |

### API Endpoints

#### GET /api/campanias
```json
// Response 200 OK
{
  "items": [
    {
      "id": "guid",
      "titulo": "Primer Album de Los Rockeros",
      "artistaNombre": "Los Rockeros",
      "importeObjetivo": 5000.00,
      "importeRecaudado": 1250.00,
      "porcentaje": 25,
      "numBackers": 15,
      "diasRestantes": 45,
      "imagenUrl": "https://..."
    }
  ],
  "totalCount": 10,
  "page": 1,
  "pageSize": 12
}
```

#### POST /api/campanias/{id}/backings
```json
// Request
{
  "rewardId": "guid",  // opcional, null = sin recompensa
  "monto": 25.00,
  "mensaje": "Exitos con el album!",
  "esAnonimo": false
}

// Response 201 Created
{
  "id": "guid",
  "campaniaTitulo": "Primer Album de Los Rockeros",
  "rewardNombre": "CD Firmado",
  "monto": 25.00,
  "fechaCreacion": "2026-01-21T10:30:00Z"
}
```

### Modelo de Datos
```csharp
public class Backing
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public string? UserId { get; set; }  // null si anonimo
    public Guid? RewardId { get; set; }
    public decimal Monto { get; set; }
    public string? Mensaje { get; set; }
    public bool EsAnonimo { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Navigation
    public Campania Campania { get; set; } = null!;
    public Reward? Reward { get; set; }
}
```

---

## US-05: Dashboard de Artista

### Descripcion
**Como** artista, **quiero** ver el progreso de mi campania **para** conocer las metricas.

### Actores
- Artista (autenticado, con campanias)

### Flujo Principal
1. Artista accede a `/dashboard`
2. Ve resumen de sus campanias:
   - Card por campania activa
   - Total recaudado vs meta
   - Porcentaje de progreso
   - Numero de backers
   - Dias restantes
3. Click en campania para ver detalle
4. Ve lista de backings recientes
5. Ve grafico de progreso en el tiempo (opcional MVP)
6. Puede ver lista completa de backers
7. Puede exportar lista de backers (post-MVP)

### Criterios de Aceptacion
| ID | Criterio | Prueba |
|----|----------|--------|
| AC-05-1 | Solo ve sus propias campanias | Login con otro user |
| AC-05-2 | Metricas actualizadas en tiempo real | Crear backing, verificar dashboard |
| AC-05-3 | Lista backers muestra los mas recientes | Ordenar por fecha desc |
| AC-05-4 | Backers anonimos muestran "Anonimo" | Verificar nombre |

### API Endpoints

#### GET /api/campanias/mis-campanias
```json
// Response 200 OK
{
  "items": [
    {
      "id": "guid",
      "titulo": "Primer Album de Los Rockeros",
      "estado": "EnCurso",
      "importeObjetivo": 5000.00,
      "importeRecaudado": 1250.00,
      "porcentaje": 25,
      "numBackers": 15,
      "diasRestantes": 45,
      "fechaFin": "2026-03-21"
    }
  ]
}
```

#### GET /api/campanias/{id}/backings
```json
// Response 200 OK
{
  "items": [
    {
      "id": "guid",
      "nombreBacker": "Juan Garcia",  // o "Anonimo"
      "rewardNombre": "CD Firmado",
      "monto": 25.00,
      "mensaje": "Exitos!",
      "fechaCreacion": "2026-01-21T10:30:00Z"
    }
  ],
  "totalCount": 15,
  "totalRecaudado": 1250.00
}
```

---

## Diagrama de Estados de Campania

```
                    +-------------+
                    |   BORRADOR  |
                    +------+------+
                           |
                    [publicar]
                           |
                           v
                    +-------------+
                    |  PUBLICADA  |
                    +------+------+
                           |
                    [fecha inicio]
                           |
                           v
                    +-------------+
                    |   EN_CURSO  |
                    +------+------+
                           |
                    [fecha fin]
                           |
           +---------------+---------------+
           |                               |
    [meta alcanzada]              [meta NO alcanzada]
           |                               |
           v                               v
   +---------------+             +-----------------+
   |COMPLETADA_EXITO|             |COMPLETADA_FALLO|
   +---------------+             +-----------------+


   En cualquier momento antes de COMPLETADA_*:
   +-------------+
   |  CANCELADA  | <-- [artista cancela]
   +-------------+
```

---

## Proximos Pasos

1. **Implementar WPR-001**: Crear entidades Domain basadas en estas specs
2. **Implementar WPR-002**: Copiar BuildingBlocks
3. **Implementar WPR-010**: US-01 completa (Backend + Frontend)
4. Continuar con US-02, US-03, US-04, US-05

**Cada historia es implementable de forma independiente** siguiendo el patron CQRS del MonolitoModular.

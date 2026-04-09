# Contratos: Templates y Guia para Artistas Noveles

> **Feature:** cs-templates-guia (US-CS-01)
> **Última actualización:** 2026-02-15

Este documento define los contratos entre proyectos. **Cualquier cambio aquí debe reflejarse en todos los proyectos afectados.**

---

## 📡 Endpoints API

### GET /api/crowdsourcing/templates

**Descripción:** Lista las plantillas de proyecto activas disponibles para artistas. Retorna resumen con rango de precios y cantidad de necesidades.

**Autorización:** ✅ Bearer JWT (Artista autenticado)

**Request Body:** N/A (GET request)

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "nombre": "Producción de EP",
        "descripcion": "Plantilla completa para producir un EP de 4-6 canciones con calidad profesional",
        "icono": "🎵",
        "orden": 1,
        "precioMinTotal": 3000.00,
        "precioMaxTotal": 8000.00,
        "moneda": 1,
        "cantidadNecesidades": 8,
        "fases": ["Preproducción", "Grabación", "Mezcla y Master", "Promoción"]
      },
      {
        "id": "7cb95f12-9301-4872-a1de-3f984e22bbc1",
        "nombre": "Producción de Álbum Completo",
        "descripcion": "Plantilla para producir un álbum de 10-12 canciones con todos los servicios profesionales",
        "icono": "💿",
        "orden": 2,
        "precioMinTotal": 8000.00,
        "precioMaxTotal": 25000.00,
        "moneda": 1,
        "cantidadNecesidades": 12,
        "fases": ["Preproducción", "Grabación", "Mezcla y Master", "Diseño y Producción", "Promoción"]
      },
      {
        "id": "9ab12c45-7821-4963-c2ef-1d873f55aac3",
        "nombre": "Producción de Sencillo",
        "descripcion": "Plantilla ágil para producir 1-2 canciones y lanzarlas rápidamente",
        "icono": "🎤",
        "orden": 3,
        "precioMinTotal": 1000.00,
        "precioMaxTotal": 3500.00,
        "moneda": 1,
        "cantidadNecesidades": 5,
        "fases": ["Grabación", "Mezcla y Master", "Promoción"]
      }
    ]
  },
  "messages": [
    {
      "message": "Plantillas obtenidas exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 500 | 5000 | Error inesperado al obtener plantillas | Excepción no controlada |

---

### GET /api/crowdsourcing/templates/{id}

**Descripción:** Obtiene el detalle completo de una plantilla incluyendo todas sus necesidades profesionales organizadas por fase.

**Autorización:** ✅ Bearer JWT (Artista autenticado)

**Path Parameters:**
- `id` (Guid) - ID de la plantilla

**Response 200 OK:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombre": "Producción de EP",
    "descripcion": "Plantilla completa para producir un EP de 4-6 canciones con calidad profesional",
    "icono": "🎵",
    "orden": 1,
    "necesidades": [
      {
        "id": "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d",
        "fase": "Preproducción",
        "titulo": "Productor Musical",
        "descripcion": "Desarrollo de arreglos, estructura y dirección musical del proyecto",
        "rolProfesional": {
          "id": 1,
          "nombre": "Productor Musical",
          "descripcion": "Profesional que guía el proceso creativo y técnico de la grabación",
          "categoriaRol": 1,
          "modalidadCobro": "Por proyecto"
        },
        "precioMinOrientativo": 500.00,
        "precioMaxOrientativo": 1500.00,
        "moneda": 1,
        "prioridad": "Alta",
        "orden": 1
      },
      {
        "id": "2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e",
        "fase": "Grabación",
        "titulo": "Ingeniero de Grabación",
        "descripcion": "Grabación profesional de todos los instrumentos y voces",
        "rolProfesional": {
          "id": 2,
          "nombre": "Ingeniero de Grabación",
          "descripcion": "Técnico especializado en captura de audio de alta calidad",
          "categoriaRol": 2,
          "modalidadCobro": "Por día"
        },
        "precioMinOrientativo": 800.00,
        "precioMaxOrientativo": 2000.00,
        "moneda": 1,
        "prioridad": "Alta",
        "orden": 2
      },
      {
        "id": "3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f",
        "fase": "Mezcla y Master",
        "titulo": "Ingeniero de Mezcla",
        "descripcion": "Mezcla profesional de las 4-6 canciones del EP",
        "rolProfesional": {
          "id": 3,
          "nombre": "Ingeniero de Mezcla",
          "descripcion": "Especialista en balance y procesamiento de audio multipista",
          "categoriaRol": 2,
          "modalidadCobro": "Por canción"
        },
        "precioMinOrientativo": 600.00,
        "precioMaxOrientativo": 1800.00,
        "moneda": 1,
        "prioridad": "Alta",
        "orden": 3
      },
      {
        "id": "4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f9a",
        "fase": "Mezcla y Master",
        "titulo": "Ingeniero de Mastering",
        "descripcion": "Masterización final para distribución en plataformas digitales",
        "rolProfesional": {
          "id": 4,
          "nombre": "Ingeniero de Mastering",
          "descripcion": "Experto en optimización final de audio para medios",
          "categoriaRol": 2,
          "modalidadCobro": "Por canción"
        },
        "precioMinOrientativo": 300.00,
        "precioMaxOrientativo": 800.00,
        "moneda": 1,
        "prioridad": "Alta",
        "orden": 4
      },
      {
        "id": "5e6f7a8b-9c0d-1e2f-3a4b-5c6d7e8f9a0b",
        "fase": "Promoción",
        "titulo": "Diseñador Gráfico",
        "descripcion": "Diseño de portada del EP y material promocional",
        "rolProfesional": {
          "id": 10,
          "nombre": "Diseñador Gráfico",
          "descripcion": "Profesional de diseño visual y branding musical",
          "categoriaRol": 3,
          "modalidadCobro": "Por proyecto"
        },
        "precioMinOrientativo": 200.00,
        "precioMaxOrientativo": 800.00,
        "moneda": 1,
        "prioridad": "Media",
        "orden": 5
      },
      {
        "id": "6f7a8b9c-0d1e-2f3a-4b5c-6d7e8f9a0b1c",
        "fase": "Promoción",
        "titulo": "Community Manager",
        "descripcion": "Gestión de redes sociales durante 3 meses post-lanzamiento",
        "rolProfesional": {
          "id": 12,
          "nombre": "Community Manager",
          "descripcion": "Experto en gestión de comunidades y marketing digital",
          "categoriaRol": 4,
          "modalidadCobro": "Por mes"
        },
        "precioMinOrientativo": 300.00,
        "precioMaxOrientativo": 900.00,
        "moneda": 1,
        "prioridad": "Media",
        "orden": 6
      },
      {
        "id": "7a8b9c0d-1e2f-3a4b-5c6d-7e8f9a0b1c2d",
        "fase": "Promoción",
        "titulo": "Fotógrafo Musical",
        "descripcion": "Sesión de fotos promocionales del proyecto",
        "rolProfesional": {
          "id": 11,
          "nombre": "Fotógrafo Musical",
          "descripcion": "Fotógrafo especializado en artistas y proyectos musicales",
          "categoriaRol": 3,
          "modalidadCobro": "Por sesión"
        },
        "precioMinOrientativo": 200.00,
        "precioMaxOrientativo": 600.00,
        "moneda": 1,
        "prioridad": "Baja",
        "orden": 7
      },
      {
        "id": "8b9c0d1e-2f3a-4b5c-6d7e-8f9a0b1c2d3e",
        "fase": "Promoción",
        "titulo": "Productor de Video Musical",
        "descripcion": "Producción de 1-2 videoclips para el EP",
        "rolProfesional": {
          "id": 13,
          "nombre": "Productor de Video Musical",
          "descripcion": "Realizador audiovisual especializado en música",
          "categoriaRol": 3,
          "modalidadCobro": "Por video"
        },
        "precioMinOrientativo": 800.00,
        "precioMaxOrientativo": 3000.00,
        "moneda": 1,
        "prioridad": "Baja",
        "orden": 8
      }
    ],
    "resumen": {
      "precioMinTotal": 3700.00,
      "precioMaxTotal": 11400.00,
      "moneda": 1,
      "cantidadNecesidadesAlta": 4,
      "cantidadNecesidadesMedia": 2,
      "cantidadNecesidadesBaja": 2
    }
  },
  "messages": [
    {
      "message": "Plantilla obtenida exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El ID es obligatorio | ID vacío o nulo |
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 404 | 2006 | Plantilla no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado al obtener plantilla | Excepción no controlada |

---

### POST /api/crowdsourcing/templates/{id}/generar

**Descripción:** Genera necesidades de crowdsourcing para un proyecto artístico a partir de una plantilla seleccionada. El artista puede personalizar presupuestos para cada necesidad.

**Autorización:** ✅ Bearer JWT (Artista autenticado, debe ser dueño del proyecto)

**Path Parameters:**
- `id` (Guid) - ID de la plantilla

**Request Body:**
```json
{
  "proyectoArtisticoId": "9f8e7d6c-5b4a-3c2d-1e0f-9a8b7c6d5e4f",
  "necesidadesSeleccionadas": [
    {
      "plantillaNecesidadId": "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d",
      "presupuestoMin": 800.00,
      "presupuestoMax": 1200.00,
      "monedaId": 1
    },
    {
      "plantillaNecesidadId": "2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e",
      "presupuestoMin": 1000.00,
      "presupuestoMax": 1800.00,
      "monedaId": 1
    },
    {
      "plantillaNecesidadId": "3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f",
      "presupuestoMin": 800.00,
      "presupuestoMax": 1500.00,
      "monedaId": 1
    },
    {
      "plantillaNecesidadId": "4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f9a",
      "presupuestoMin": 400.00,
      "presupuestoMax": 700.00,
      "monedaId": 1
    },
    {
      "plantillaNecesidadId": "5e6f7a8b-9c0d-1e2f-3a4b-5c6d7e8f9a0b",
      "presupuestoMin": 300.00,
      "presupuestoMax": 600.00,
      "monedaId": 1
    }
  ]
}
```

**Response 201 Created:**
```json
{
  "data": {
    "necesidadesCreadas": 5,
    "necesidadIds": [
      "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
      "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
      "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f",
      "d4e5f6a7-b8c9-0d1e-2f3a-4b5c6d7e8f9a",
      "e5f6a7b8-c9d0-1e2f-3a4b-5c6d7e8f9a0b"
    ],
    "presupuestoTotalMin": 3300.00,
    "presupuestoTotalMax": 5800.00,
    "moneda": 1
  },
  "messages": [
    {
      "message": "Necesidades generadas exitosamente a partir de la plantilla",
      "errorCode": "0001"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El ID del proyecto artístico es obligatorio | proyectoArtisticoId vacío |
| 400 | 1001 | Debe seleccionar al menos una necesidad | necesidadesSeleccionadas vacío |
| 400 | 1009 | El presupuesto mínimo no puede ser negativo | presupuestoMin < 0 |
| 400 | 1009 | El presupuesto máximo debe ser mayor o igual al mínimo | presupuestoMax < presupuestoMin |
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 403 | 3002 | No tienes permiso para modificar este proyecto | UserId del token != dueño del proyecto |
| 404 | 2006 | Plantilla no encontrada | ID plantilla no existe |
| 404 | 2007 | Proyecto artístico no encontrado | proyectoArtisticoId no existe |
| 404 | 2008 | Una o más necesidades de plantilla no encontradas | plantillaNecesidadId inválido |
| 500 | 5000 | Error inesperado al generar necesidades | Excepción no controlada |

---

### GET /api/crowdsourcing/maestras/roles-profesionales

**Descripción:** Obtiene el catálogo completo de roles profesionales disponibles con su categoría y modalidad de cobro.

**Autorización:** ✅ Bearer JWT (Usuario autenticado)

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": 1,
        "nombre": "Productor Musical",
        "descripcion": "Profesional que guía el proceso creativo y técnico de la grabación",
        "categoriaRol": {
          "id": 1,
          "nombre": "Producción Musical",
          "icono": "🎛️",
          "orden": 1
        },
        "modalidadCobro": "Por proyecto",
        "activo": true
      },
      {
        "id": 2,
        "nombre": "Ingeniero de Grabación",
        "descripcion": "Técnico especializado en captura de audio de alta calidad",
        "categoriaRol": {
          "id": 2,
          "nombre": "Ingeniería de Audio",
          "icono": "🎚️",
          "orden": 2
        },
        "modalidadCobro": "Por día",
        "activo": true
      },
      {
        "id": 3,
        "nombre": "Ingeniero de Mezcla",
        "descripcion": "Especialista en balance y procesamiento de audio multipista",
        "categoriaRol": {
          "id": 2,
          "nombre": "Ingeniería de Audio",
          "icono": "🎚️",
          "orden": 2
        },
        "modalidadCobro": "Por canción",
        "activo": true
      },
      {
        "id": 4,
        "nombre": "Ingeniero de Mastering",
        "descripcion": "Experto en optimización final de audio para medios",
        "categoriaRol": {
          "id": 2,
          "nombre": "Ingeniería de Audio",
          "icono": "🎚️",
          "orden": 2
        },
        "modalidadCobro": "Por canción",
        "activo": true
      },
      {
        "id": 10,
        "nombre": "Diseñador Gráfico",
        "descripcion": "Profesional de diseño visual y branding musical",
        "categoriaRol": {
          "id": 3,
          "nombre": "Diseño y Creatividad",
          "icono": "🎨",
          "orden": 3
        },
        "modalidadCobro": "Por proyecto",
        "activo": true
      },
      {
        "id": 11,
        "nombre": "Fotógrafo Musical",
        "descripcion": "Fotógrafo especializado en artistas y proyectos musicales",
        "categoriaRol": {
          "id": 3,
          "nombre": "Diseño y Creatividad",
          "icono": "🎨",
          "orden": 3
        },
        "modalidadCobro": "Por sesión",
        "activo": true
      },
      {
        "id": 12,
        "nombre": "Community Manager",
        "descripcion": "Experto en gestión de comunidades y marketing digital",
        "categoriaRol": {
          "id": 4,
          "nombre": "Marketing y Promoción",
          "icono": "📢",
          "orden": 4
        },
        "modalidadCobro": "Por mes",
        "activo": true
      },
      {
        "id": 13,
        "nombre": "Productor de Video Musical",
        "descripcion": "Realizador audiovisual especializado en música",
        "categoriaRol": {
          "id": 3,
          "nombre": "Diseño y Creatividad",
          "icono": "🎨",
          "orden": 3
        },
        "modalidadCobro": "Por video",
        "activo": true
      }
    ]
  },
  "messages": [
    {
      "message": "Roles profesionales obtenidos exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 500 | 5000 | Error inesperado al obtener roles | Excepción no controlada |

---

### GET /api/crowdsourcing/maestras/categorias-rol

**Descripción:** Obtiene las categorías de roles profesionales para filtrado y organización.

**Autorización:** ✅ Bearer JWT (Usuario autenticado)

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": 1,
        "nombre": "Producción Musical",
        "icono": "🎛️",
        "orden": 1
      },
      {
        "id": 2,
        "nombre": "Ingeniería de Audio",
        "icono": "🎚️",
        "orden": 2
      },
      {
        "id": 3,
        "nombre": "Diseño y Creatividad",
        "icono": "🎨",
        "orden": 3
      },
      {
        "id": 4,
        "nombre": "Marketing y Promoción",
        "icono": "📢",
        "orden": 4
      }
    ]
  },
  "messages": [
    {
      "message": "Categorías obtenidas exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 500 | 5000 | Error inesperado al obtener categorías | Excepción no controlada |

---

## 🔐 Autorización

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `GET /api/crowdsourcing/templates` | ✅ | Artista | UserId del token |
| `GET /api/crowdsourcing/templates/{id}` | ✅ | Artista | UserId del token |
| `POST /api/crowdsourcing/templates/{id}/generar` | ✅ | Artista | Valida ownership del proyecto |
| `GET /api/crowdsourcing/maestras/roles-profesionales` | ✅ | - | Usuario autenticado |
| `GET /api/crowdsourcing/maestras/categorias-rol` | ✅ | - | Usuario autenticado |

### Claims JWT Requeridos

```json
{
  "sub": "userId (GUID del usuario)",
  "email": "artista@example.com",
  "role": "Artista",
  "exp": 1739823600,
  "iat": 1739737200
}
```

**Configuración JWT:**
- **Algoritmo:** HS256
- **Expiración:** 24 horas
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

### Protección de Rutas Frontend

| Ruta (Admin Dashboard) | Auth | Redirect | Notas |
|------------------------|------|----------|-------|
| `/dashboard/crowdsourcing/templates` | ✅ | `/auth/login` | Lista plantillas |
| `/dashboard/crowdsourcing/templates/:id` | ✅ | `/auth/login` | Detalle plantilla |
| `/dashboard/crowdsourcing/wizard/:projectId` | ✅ | `/auth/login` | Wizard de generación |

---

## 📦 DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/PlantillaProyectoListDto.cs
public class PlantillaProyectoListDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public decimal PrecioMinTotal { get; set; }
    public decimal PrecioMaxTotal { get; set; }
    public int Moneda { get; set; }
    public int CantidadNecesidades { get; set; }
    public List<string> Fases { get; set; } = new();
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/PlantillaProyectoDto.cs
public class PlantillaProyectoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public List<PlantillaProyectoNecesidadDto> Necesidades { get; set; } = new();
    public PlantillaResumenDto Resumen { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/PlantillaProyectoNecesidadDto.cs
public class PlantillaProyectoNecesidadDto
{
    public Guid Id { get; set; }
    public string Fase { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public RolProfesionalDto RolProfesional { get; set; } = null!;
    public decimal? PrecioMinOrientativo { get; set; }
    public decimal? PrecioMaxOrientativo { get; set; }
    public int Moneda { get; set; }
    public string Prioridad { get; set; } = null!; // "Alta", "Media", "Baja"
    public int Orden { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/PlantillaResumenDto.cs
public class PlantillaResumenDto
{
    public decimal PrecioMinTotal { get; set; }
    public decimal PrecioMaxTotal { get; set; }
    public int Moneda { get; set; }
    public int CantidadNecesidadesAlta { get; set; }
    public int CantidadNecesidadesMedia { get; set; }
    public int CantidadNecesidadesBaja { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/RolProfesionalDto.cs
public class RolProfesionalDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int CategoriaRolId { get; set; }
    public string? ModalidadCobro { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/RolProfesionalConCategoriaDto.cs
public class RolProfesionalConCategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public CategoriaRolDto CategoriaRol { get; set; } = null!;
    public string? ModalidadCobro { get; set; }
    public bool Activo { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CategoriaRolDto.cs
public class CategoriaRolDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Icono { get; set; }
    public int Orden { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/GenerarNecesidadesResultDto.cs
public class GenerarNecesidadesResultDto
{
    public int NecesidadesCreadas { get; set; }
    public List<Guid> NecesidadIds { get; set; } = new();
    public decimal PresupuestoTotalMin { get; set; }
    public decimal PresupuestoTotalMax { get; set; }
    public int Moneda { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Features/Templates/Commands/GenerarNecesidadesDesdeTemplateCommand.cs
public class NecesidadSeleccionadaDto
{
    public Guid PlantillaNecesidadId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int MonedaId { get; set; }
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/crowdsourcing.ts

export interface PlantillaProyectoList {
  id: string;
  nombre: string;
  descripcion?: string;
  icono?: string;
  orden: number;
  precioMinTotal: number;
  precioMaxTotal: number;
  moneda: number;
  cantidadNecesidades: number;
  fases: string[];
}

export interface PlantillaProyecto {
  id: string;
  nombre: string;
  descripcion?: string;
  icono?: string;
  orden: number;
  necesidades: PlantillaProyectoNecesidad[];
  resumen: PlantillaResumen;
}

export interface PlantillaProyectoNecesidad {
  id: string;
  fase: string;
  titulo: string;
  descripcion?: string;
  rolProfesional: RolProfesional;
  precioMinOrientativo?: number;
  precioMaxOrientativo?: number;
  moneda: number;
  prioridad: PrioridadNecesidad;
  orden: number;
}

export interface PlantillaResumen {
  precioMinTotal: number;
  precioMaxTotal: number;
  moneda: number;
  cantidadNecesidadesAlta: number;
  cantidadNecesidadesMedia: number;
  cantidadNecesidadesBaja: number;
}

export interface RolProfesional {
  id: number;
  nombre: string;
  descripcion?: string;
  categoriaRolId: number;
  modalidadCobro?: string;
}

export interface RolProfesionalConCategoria {
  id: number;
  nombre: string;
  descripcion?: string;
  categoriaRol: CategoriaRol;
  modalidadCobro?: string;
  activo: boolean;
}

export interface CategoriaRol {
  id: number;
  nombre: string;
  icono?: string;
  orden: number;
}

export interface GenerarNecesidadesRequest {
  proyectoArtisticoId: string;
  necesidadesSeleccionadas: NecesidadSeleccionada[];
}

export interface NecesidadSeleccionada {
  plantillaNecesidadId: string;
  presupuestoMin?: number;
  presupuestoMax?: number;
  monedaId: number;
}

export interface GenerarNecesidadesResult {
  necesidadesCreadas: number;
  necesidadIds: string[];
  presupuestoTotalMin: number;
  presupuestoTotalMax: number;
  moneda: number;
}

export type PrioridadNecesidad = "Alta" | "Media" | "Baja";
```

---

## ✅ Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| proyectoArtisticoId | requerido | `.NotEmpty().WithErrorCode(Validation_Required)` | `.min(1, 'El proyecto es obligatorio')` |
| necesidadesSeleccionadas | requerido, min 1 | `.NotEmpty().WithErrorCode(Validation_Required)` | `.array().min(1, 'Debe seleccionar al menos una necesidad')` |
| plantillaNecesidadId | requerido | `.NotEmpty().WithErrorCode(Validation_Required)` | `.min(1, 'La necesidad es obligatoria')` |
| presupuestoMin | >= 0 si presente | `.GreaterThanOrEqualTo(0).When(x => x.PresupuestoMin.HasValue).WithErrorCode(Validation_InvalidRange)` | `.nonnegative('No puede ser negativo').optional()` |
| presupuestoMax | >= presupuestoMin | `.GreaterThanOrEqualTo(x => x.PresupuestoMin ?? 0).When(x => x.PresupuestoMax.HasValue).WithErrorCode(Validation_InvalidRange)` | `refine(presupuestoMax >= presupuestoMin)` |
| monedaId | requerido, > 0 | `.GreaterThan(0).WithErrorCode(Validation_Required)` | `.positive('La moneda es obligatoria')` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/crowdsourcing.schema.ts
import { z } from 'zod';

export const necesidadSeleccionadaSchema = z.object({
  plantillaNecesidadId: z
    .string()
    .min(1, 'La necesidad es obligatoria'),
  presupuestoMin: z
    .number()
    .nonnegative('El presupuesto mínimo no puede ser negativo')
    .optional(),
  presupuestoMax: z
    .number()
    .nonnegative('El presupuesto máximo no puede ser negativo')
    .optional(),
  monedaId: z
    .number()
    .positive('La moneda es obligatoria'),
}).refine(
  (data) => {
    if (data.presupuestoMin !== undefined && data.presupuestoMax !== undefined) {
      return data.presupuestoMax >= data.presupuestoMin;
    }
    return true;
  },
  {
    message: 'El presupuesto máximo debe ser mayor o igual al mínimo',
    path: ['presupuestoMax'],
  }
);

export const generarNecesidadesSchema = z.object({
  proyectoArtisticoId: z
    .string()
    .min(1, 'El proyecto artístico es obligatorio'),
  necesidadesSeleccionadas: z
    .array(necesidadSeleccionadaSchema)
    .min(1, 'Debe seleccionar al menos una necesidad'),
});

export type GenerarNecesidadesFormData = z.infer<typeof generarNecesidadesSchema>;
export type NecesidadSeleccionadaFormData = z.infer<typeof necesidadSeleccionadaSchema>;
```

---

## 🔗 Constantes Compartidas

### Adiciones a src/shared/constants/index.ts

```typescript
// Query Keys additions
export const QUERY_KEYS = {
  // ... existing keys

  // Crowdsourcing Templates
  crowdsourcing: {
    templates: {
      all: ['crowdsourcing', 'templates'] as const,
      byId: (id: string) => ['crowdsourcing', 'templates', id] as const,
    },
    maestras: {
      rolesProfesionales: ['crowdsourcing', 'maestras', 'roles'] as const,
      categoriasRol: ['crowdsourcing', 'maestras', 'categorias'] as const,
    },
  },
};

// API Routes additions
export const API_ROUTES = {
  // ... existing routes

  crowdsourcing: {
    templates: {
      base: '/api/crowdsourcing/templates',
      byId: (id: string) => `/api/crowdsourcing/templates/${id}`,
      generar: (id: string) => `/api/crowdsourcing/templates/${id}/generar`,
    },
    maestras: {
      rolesProfesionales: '/api/crowdsourcing/maestras/roles-profesionales',
      categoriasRol: '/api/crowdsourcing/maestras/categorias-rol',
    },
  },
};

// App Routes additions
export const APP_ROUTES = {
  // ... existing routes

  dashboard: {
    // ... existing dashboard routes
    crowdsourcing: {
      templates: '/dashboard/crowdsourcing/templates',
      templateDetail: (id: string) => `/dashboard/crowdsourcing/templates/${id}`,
      wizard: (projectId: string) => `/dashboard/crowdsourcing/wizard/${projectId}`,
    },
  },
};

// New constants for Crowdsourcing

// Prioridad de Necesidad (aligned with backend string values)
export const PRIORIDAD_NECESIDAD = {
  ALTA: 'Alta',
  MEDIA: 'Media',
  BAJA: 'Baja',
} as const;

export const PRIORIDAD_NECESIDAD_LABELS: Record<string, string> = {
  Alta: 'Alta prioridad',
  Media: 'Prioridad media',
  Baja: 'Prioridad baja',
};

export const PRIORIDAD_NECESIDAD_COLORS: Record<string, string> = {
  Alta: 'red',
  Media: 'yellow',
  Baja: 'blue',
};

// Modalidades de Cobro (catálogo de valores posibles)
export const MODALIDAD_COBRO = {
  POR_PROYECTO: 'Por proyecto',
  POR_DIA: 'Por día',
  POR_HORA: 'Por hora',
  POR_CANCION: 'Por canción',
  POR_MES: 'Por mes',
  POR_SESION: 'Por sesión',
  POR_VIDEO: 'Por video',
} as const;

// Fases comunes de proyectos musicales
export const FASES_PROYECTO = [
  'Preproducción',
  'Grabación',
  'Mezcla y Master',
  'Diseño y Producción',
  'Promoción',
  'Distribución',
] as const;
```

---

## 📊 Mapeo de Errores a UI

### Adiciones a src/shared/utils/error-messages.ts

```typescript
export const ERROR_MESSAGES: Record<string, string> = {
  // ... existing error messages

  // Crowdsourcing Template errors
  TEMPLATE_NOT_FOUND: 'La plantilla seleccionada no existe',
  PROYECTO_ARTISTICO_NOT_FOUND: 'El proyecto artístico no fue encontrado',
  PLANTILLA_NECESIDAD_NOT_FOUND: 'Una o más necesidades de la plantilla no fueron encontradas',
  PROYECTO_NO_PERTENECE_ARTISTA: 'No tienes permiso para modificar este proyecto',

  // Validation errors for crowdsourcing
  VALIDATION_PRESUPUESTO_MIN_NEGATIVO: 'El presupuesto mínimo no puede ser negativo',
  VALIDATION_PRESUPUESTO_MAX_MENOR_MIN: 'El presupuesto máximo debe ser mayor o igual al mínimo',
  VALIDATION_NECESIDADES_REQUERIDAS: 'Debe seleccionar al menos una necesidad',
  VALIDATION_PROYECTO_REQUERIDO: 'Debe seleccionar un proyecto artístico',
};
```

---

## 🔔 Eventos SignalR

**No aplica para esta feature.** La generación de necesidades desde templates es un flujo síncrono sin necesidad de notificaciones en tiempo real.

---

## 📝 Notas de Implementación

### Backend

1. **Módulo:** Crear nuevo módulo `Crowdsourcing` con estructura estándar (Domain, Application, Infra, WebApi)

2. **Entidades clave:**
   - `PlantillaProyecto` - Plantillas de proyectos (EP, Álbum, Sencillo)
   - `PlantillaProyectoNecesidad` - Necesidades profesionales por plantilla
   - `MaestraRolProfesional` - Catálogo de roles (int ID)
   - `MaestraCategoriaRol` - Categorías de roles (int ID)
   - `NecesidadCrowdsourcing` - Necesidades generadas para ProyectoArtistico (ya existe en el sistema)

3. **Servicios:**
   - `IPlantillaProyectoService` - CRUD y lógica de plantillas
   - `IRolProfesionalService` - Gestión de catálogos maestros
   - `INecesidadCrowdsourcingService` - Generación de necesidades desde template

4. **Validaciones críticas:**
   - En `GenerarNecesidadesDesdeTemplateValidator`: Validar que el artista sea dueño del proyecto usando `IProyectoArtisticoService` con caching
   - Validar que todas las `plantillaNecesidadId` existan en la plantilla seleccionada
   - Validar rangos de presupuesto (min >= 0, max >= min)

5. **Lógica de generación:**
   - Query plantilla con Include de necesidades y roles
   - Crear una `NecesidadCrowdsourcing` por cada item seleccionado
   - Copiar datos de `PlantillaProyectoNecesidad` a `NecesidadCrowdsourcing`
   - Usar presupuestos personalizados del request o valores orientativos de la plantilla
   - Retornar resumen con IDs generados y totales

6. **Constants:**
   - Crear `ServiceResponseMessageType` con códigos 2006-2008 para NotFound específicos de crowdsourcing
   - Reutilizar códigos existentes para validaciones estándar (1001, 1009, 3001, 3002, 5000)

### Frontend

1. **Nuevos archivos:**
   - `src/shared/types/crowdsourcing.ts` - Tipos TypeScript
   - `src/shared/schemas/crowdsourcing.schema.ts` - Validaciones Zod
   - `src/admin/src/features/crowdsourcing/` - Feature completa con componentes del wizard

2. **Componentes clave del wizard:**
   - `TemplateSelectionStep.tsx` - Paso 1: Selección de plantilla con cards
   - `NeedCustomizationStep.tsx` - Paso 2: Personalización de presupuestos por necesidad
   - `SummaryConfirmationStep.tsx` - Paso 3: Resumen y confirmación
   - `WizardProgress.tsx` - Indicador de progreso multi-paso

3. **Hooks personalizados:**
   - `useTemplates()` - Query para listar plantillas
   - `useTemplateDetail(id)` - Query para detalle de plantilla
   - `useGenerarNecesidades()` - Mutation para generar necesidades
   - `useRolesProfesionales()` - Query para catálogo de roles
   - `useCategoriaRol()` - Query para categorías

4. **State management:**
   - Usar Zustand o Context para manejar estado del wizard (plantilla seleccionada, necesidades, presupuestos)
   - Persistir en localStorage para recuperar progreso si el usuario sale

5. **UX considerations:**
   - Mostrar precios orientativos vs. presupuestos personalizados claramente diferenciados
   - Filtrado de necesidades por prioridad (Alta, Media, Baja)
   - Agrupación visual por fase (Preproducción, Grabación, etc.)
   - Resumen en tiempo real del presupuesto total mientras personaliza
   - Validación en cada paso antes de permitir avanzar

6. **Integración con ProyectoArtistico:**
   - El wizard se inicia desde la vista de detalle de un ProyectoArtistico
   - Tras completar el wizard, redirigir a la lista de necesidades del proyecto generadas

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores
- [x] Autorización por endpoint
- [x] Claims JWT documentados
- [x] DTOs C# completos
- [x] Types TypeScript equivalentes
- [x] Schemas Zod con mismas reglas
- [x] Constantes compartidas (query keys, API routes, app routes)
- [x] Nuevas constantes de dominio (PRIORIDAD_NECESIDAD, MODALIDAD_COBRO, FASES_PROYECTO)
- [x] Mapeo de errores a mensajes UI
- [x] Notas de implementación detalladas

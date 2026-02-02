# Feature: Registro de Artista

> **ID:** registro-artista
> **User Story:** US-01
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** 1

---

## Descripción

Permitir que artistas se registren en la plataforma y creen su perfil para poder presentar proyectos musicales de crowdfunding.

---

## User Story

**Como** artista
**Quiero** registrarme y crear mi perfil
**Para** presentar mi proyecto musical en la plataforma

---

## Flujo Principal

```
1. Usuario accede a /auth/register (Landing)
2. Completa formulario de registro (email, password)
3. Sistema crea cuenta en Identity (Backend)
4. Usuario es redirigido a /artista/perfil/crear (Admin)
5. Completa formulario de perfil artista
6. Sistema crea perfil de Artista (Backend)
7. Usuario es redirigido a /dashboard (Admin)
```

---

## Criterios de Aceptación

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-01-1 | Email único en el sistema | Backend |
| AC-01-2 | Password mínimo 8 caracteres | Backend + Frontend |
| AC-01-3 | Nombre artístico obligatorio | Backend + Frontend |
| AC-01-4 | Imagen es URL válida (opcional) | Backend + Frontend |
| AC-01-5 | Perfil visible en `/artistas/{id}` | Landing + Backend |
| AC-01-6 | Después del registro, usuario tiene JWT válido | Backend |
| AC-01-7 | Dashboard accesible solo con JWT | Admin + Backend |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Endpoints Auth + Artista, Identity, JWT | ALTO |
| **Landing** | Form registro, página perfil público | MEDIO |
| **Admin** | Form crear perfil, dashboard | ALTO |
| **Shared** | Types, schemas, constantes | MEDIO |

---

## Dependencias

### Técnicas
- WPR-006: Identity + JWT debe estar implementado

### De otras features
- Ninguna (feature inicial)

---

## Referencia

- User Story completa: [docs/product/US-01-registro-artista.md](../../product/US-01-registro-artista.md)

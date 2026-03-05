# Coleccion Postman - cp-inscripcion-programa (US-CP-03)

> **Coleccion de pruebas de integracion 100% auto-inclusiva para validar el lifecycle completo de inscripciones de promotores en programas.**

---

## Resumen Rapido

| Metrica | Valor |
|---------|-------|
| **Requests** | 18 |
| **Folders** | 6 |
| **Assertions** | ~65 |
| **Endpoints cubiertos** | 8/8 (100%) |
| **Usuarios necesarios** | 2 (pre-existentes) |
| **Tiempo de ejecucion** | 15-20 segundos |
| **Status** | ✓ Listo para usar |

---

## Archivos Incluidos

```
plans/cp-inscripcion-programa/backend/
├── postman-collection.json       ★ Coleccion Postman v2.1 ejecutable
├── QUICK_START.md               Guia rapida de uso (START HERE)
├── COLLECTION_SUMMARY.md        Analisis detallado de la coleccion
├── FLOW_DIAGRAM.md              Diagramas de flujo y estados
├── EXECUTION_GUIDE.md           Instrucciones para CI/CD
├── INDEX.md                     Indice y navegacion
└── README.md                    Este archivo
```

**Comienza por**: [`QUICK_START.md`](./QUICK_START.md)

---

## Que Testea Esta Coleccion

### Happy Path (Flujo Principal)
```
Promotor Explorar → Solicitar Inscripcion → Artista Aprueba → Codigo Generado
```

1. **GET /programas/explorar** - Catalogo paginado de programas
2. **POST /programas/{id}/inscripcion** - Solicitar inscripcion (estado: Pendiente)
3. **GET /promotor/mis-programas** - Listar inscripciones como promotor
4. **GET /programas/{id}/inscripciones** - Listar inscripciones como artista
5. **PATCH /aprobar** - Aprobar + generar codigo referido
6. **GET /promotor/mis-programas** (verificar) - Validar codigo generado

### Validaciones (400 Bad Request)
- Inscripcion duplicada
- Programa no existe

### Acciones Artista
- Rechazar inscripcion (eliminacion fisica)
- Bloquear promotor

### Errores de Autenticacion
- 401 Unauthorized (sin token)
- 403 Forbidden (no es propietario)

### Not Found (404)
- Inscripcion no existe
- Programa no existe

---

## Requisitos Minimos

### Backend
- .NET 8 + EF Core ejecutandose
- URL: `http://localhost:5001` (configurable)
- BD con usuarios de prueba (ver abajo)

### Base de Datos
Usuarios pre-existentes en Docker DB:
- **usuario1@mail.com** / **123456** (Promotor/Fan, tiene Artista)
- **api-test@mail.com** / **123456** (Artista/Fan, tiene Artista)

### Herramientas
- **Postman** (para GUI) o **Newman CLI** (para automation)
- **Node.js + npm** (si usas Newman)

### Datos de Prueba
- Al menos 1 programa activo debe existir en `PromoPrograma` tabla

---

## Como Usar

### Opcion 1: Postman GUI (Recomendado para desarrollo)

```bash
# 1. Abrir Postman
# 2. File → Import
# 3. Seleccionar: postman-collection.json
# 4. En la coleccion, hacer click en "Run"
# 5. Ejecutar carpetas en orden:
#    - _Setup (primero, genera tokens)
#    - Inscripcion - Happy Path (flujo principal)
#    - Resto de carpetas segun se necesite
```

### Opcion 2: Newman CLI (Recomendado para CI/CD)

```bash
# Instalacion
npm install -g newman newman-reporter-htmlextra

# Ejecucion basica
newman run plans/cp-inscripcion-programa/backend/postman-collection.json

# Con reporte HTML
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/results.html

# Solo happy path (rapido)
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "_Setup,Inscripcion - Happy Path"
```

---

## Estados de Inscripcion

```
Pendiente → Aprobado → DadoDeBaja
         ↓
       Bloqueado (no puede re-solicitar)

También: Rechazado (ELIMINADO fisicamente)
```

| Estado | Condiciones | Transiciones |
|--------|-------------|--------------|
| **Pendiente** | EsAprobado=F, EsBloqueado=F | → Aprobar, Rechazar, Bloquear |
| **Aprobado** | EsAprobado=T, CodigoReferido!=null | → DarDeBaja, Bloquear |
| **DadoDeBaja** | FechaBaja!=null | (terminal) |
| **Bloqueado** | EsBloqueado=T | (terminal, sin re-solicitud) |
| **Rechazado** | (ELIMINADO) | (terminal) |

---

## Variables de Coleccion

Todas se cargan **automaticamente** durante ejecucion:

| Variable | Ejemplo | Generada en |
|----------|---------|-------------|
| `baseUrl` | `http://localhost:5001` | Manual (default) |
| `promotorToken` | `eyJhbGc...` | _Setup 01 |
| `artistaToken` | `eyJhbGc...` | _Setup 02 |
| `programaId` | `uuid-xxx` | Happy Path 01 |
| `inscripcionId` | `uuid-yyy` | Happy Path 02 |
| `codigoReferido` | `ABC123-XyZ9w` | Happy Path 05 |
| (+ 9 mas) | | Varias |

---

## Errores Validados

| ErrorCode | HTTP | Descripcion |
|-----------|------|-------------|
| 1001 | 400 | Campo requerido |
| 2019 | 404 | Programa no encontrado |
| 2020 | 404 | Inscripcion no encontrada |
| 4021 | 400 | Inscripcion ya existe |
| 4022 | 403/400 | Promotor bloqueado |
| 4026 | 403 | No es propietario |

---

## Flujo de Ejecucion

```
START
  ↓
_Setup (OBLIGATORIO)
  ├─ Login Promotor → promotorToken
  └─ Login Artista → artistaToken
  ↓
Happy Path (RECOMENDADO)
  ├─ Explorar programas
  ├─ Solicitar inscripcion
  ├─ Listar inscripciones (ambos roles)
  └─ Aprobar + generar codigo
  ↓
[OPCIONAL] Otras validaciones
  ├─ Validaciones
  ├─ Acciones Artista
  ├─ Bloqueo y Baja
  ├─ Auth Errors
  └─ Not Found
  ↓
END
```

**Tiempo total**: ~20s (solo happy path: ~8s)

---

## Troubleshooting

### "Status is 401" en cualquier request
**Problema**: _Setup no se ejecuto o tokens expiraron.
**Solucion**: Ejecutar _Setup nuevamente.

### "items is empty" en Happy Path 01
**Problema**: No hay programas activos en BD.
**Solucion**: Crear programa via Swagger `/api/crowdpromotion/programas`

### "connection refused"
**Problema**: Backend no esta corriendo.
**Solucion**: `dotnet run --project src/api/WebApi`

### "Database connection failed"
**Problema**: BD no esta iniciada.
**Solucion**: `docker compose up -d` (si usando Docker)

---

## Documentacion Completa

Para mas detalles, ver:

| Documento | Proposito |
|-----------|-----------|
| [QUICK_START.md](./QUICK_START.md) | **Guia rapida** - Comienzo aqui |
| [COLLECTION_SUMMARY.md](./COLLECTION_SUMMARY.md) | Analisis detallado de metricas |
| [FLOW_DIAGRAM.md](./FLOW_DIAGRAM.md) | Diagramas de flujo y estados |
| [EXECUTION_GUIDE.md](./EXECUTION_GUIDE.md) | Instrucciones para CI/CD |
| [INDEX.md](./INDEX.md) | Indice y navegacion completa |
| [api-contracts.md](./api-contracts.md) | Contratos API detallados |

---

## Integracion en CI/CD

### GitHub Actions
```yaml
- uses: actions/setup-node@v3
  with:
    node-version: '18'

- run: npm install -g newman newman-reporter-htmlextra

- run: |
    newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
      --reporters cli,htmlextra \
      --reporter-htmlextra-export reports/results.html \
      --bail
```

### CLI Simple (Pre-commit Hook)
```bash
#!/bin/bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "_Setup,Inscripcion - Happy Path" \
  --bail && echo "✓ Tests passed"
```

Ver [`EXECUTION_GUIDE.md`](./EXECUTION_GUIDE.md) para mas detalles.

---

## Copiar/Pegar Comandos

```bash
# Desarrollo local (GUI)
open plans/cp-inscripcion-programa/backend/postman-collection.json

# CLI rapido
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters cli

# Con reporte HTML
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/results.html

# CI/CD strict (falla si hay error)
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --bail --reporters json --reporter-json-export results.json

# Solo happy path (rapido para pre-commit)
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "_Setup,Inscripcion - Happy Path"
```

---

## Metricas Finales

```
Total Folders:      6
Total Requests:     18
Total Assertions:   ~65
Total Variables:    15

Coverage:
  ✓ Endpoints:      8/8 (100%)
  ✓ Estados:        4/4 (100%)
  ✓ Roles:          2/2 (100%)
  ✓ Errores:        6/8 (75%)

Usuarios:           2 (pre-existentes)
Tiempo ejecucion:   15-20 segundos
Status:             READY TO USE ✓
```

---

## FAQ

**P: Donde obtengo los usuarios?**
A: Pre-existentes en Docker DB. Ver `QUICK_START.md`.

**P: Puedo ejecutar multiples veces?**
A: Si, cada vez se reutilizan los mismos usuarios.

**P: Como cambio la base URL?**
A: En la coleccion edita la variable `baseUrl` o: `--env-var baseUrl=...` en Newman.

**P: Que pasa si falla un test?**
A: Lee `QUICK_START.md` seccion "Troubleshooting".

**P: Como genero reportes HTML?**
A: `newman run ... --reporters htmlextra --reporter-htmlextra-export reports/test.html`

---

## Soporte

- **Contratos API**: Ver [`api-contracts.md`](./api-contracts.md)
- **Comandos Newman**: Ver [`EXECUTION_GUIDE.md`](./EXECUTION_GUIDE.md)
- **Diagramas**: Ver [`FLOW_DIAGRAM.md`](./FLOW_DIAGRAM.md)
- **Guia rapida**: Ver [`QUICK_START.md`](./QUICK_START.md)

---

## Proximos Pasos

1. ✓ Lee [`QUICK_START.md`](./QUICK_START.md)
2. ✓ Asegura backend en `http://localhost:5001`
3. ✓ Importa `postman-collection.json` en Postman
4. ✓ Ejecuta carpeta `_Setup` primero
5. ✓ Ejecuta `Inscripcion - Happy Path`
6. ✓ Revisa reportes si hay fallos

---

## Changelog

**v1.0** (2026-02-25)
- Coleccion inicial generada
- 18 requests, 6 folders
- Happy path + validaciones + errores
- Documentacion completa

---

## Autor

Generado por sistema de tooling automático de WePlay Rises.

**Fecha**: 2026-02-25
**Feature**: cp-inscripcion-programa (US-CP-03)
**Status**: ✓ Listo para usar

---

**[Comienza aqui →](./QUICK_START.md)**

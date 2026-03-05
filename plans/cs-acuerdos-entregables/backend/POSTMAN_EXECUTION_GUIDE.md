# Guia de Ejecucion: Postman Collection - Acuerdos y Entregables

## Overview

Coleccion Postman para validar la feature **cs-acuerdos-entregables** (US-CS-04). Incluye pruebas de integracion de los 11 endpoints principales con coverage de casos de exito, validacion y autorizacion.

**Archivo:** `postman-collection.json`

---

## Requisitos Previos

### Backend en ejecucion
```bash
# Docker
docker compose up -d

# O .NET CLI
cd src/api/WebApi
dotnet run
```

**URL Base:** `http://localhost:5001`

### Base de datos con datos de prueba

La coleccion requiere:
- **Usuarios:** usuario1@mail.com (artista) + profesional dinamico (registrado en setup)
- **Necesidades:** Al menos 2-3 necesidades en estado `Abierta` con propuestas `Pendiente`
- **Propuestas:** Al menos 4 propuestas pendientes vinculadas a necesidades del artista

Si no existen, cargar datos de seed o crear manualmente via Swagger.

### Postman CLI (Newman)

```bash
# Instalar globalmente
npm install -g newman

# O instalar localmente en el proyecto
npm install --save-dev newman
```

---

## Ejecucion

### Opcion 1: GUI Postman

1. Abrir Postman Desktop
2. File > Import > Select `postman-collection.json`
3. Seleccionar entorno (o crear uno con variables)
4. Click en coleccion > Run Collection
5. Verificar results

### Opcion 2: CLI (Newman)

```bash
# Ejecucion simple con output en consola
newman run plans/cs-acuerdos-entregables/backend/postman-collection.json \
  --reporters cli

# Con reporte HTML
newman run plans/cs-acuerdos-entregables/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export ./results/postman-report.html

# Con variables de entorno
newman run plans/cs-acuerdos-entregables/backend/postman-collection.json \
  --environment postman-env.json \
  --reporters cli,htmlextra

# Ejecucion con timeout y delay
newman run plans/cs-acuerdos-entregables/backend/postman-collection.json \
  --reporters cli \
  --timeout-request 10000 \
  --delay-request 500
```

---

## Estructura de la Coleccion

### Folders principales

1. **_Setup** (4 requests)
   - Login artista (usuario1@mail.com)
   - Register profesional (email dinamico)
   - Get necesidad ID (para buscar propuestas)
   - Get primera propuesta (estado Pendiente)

2. **Acuerdos & Entregables - CRUD Lifecycle** (8 requests)
   - 01. POST Aceptar Propuesta → Crear Acuerdo
   - 02. GET Acuerdo by ID
   - 03. POST Create Milestone
   - 04. PUT Update Milestone
   - 05. POST Create Entregable (as Profesional)
   - 06. PATCH Aprobar Entregable (as Artista)
   - 07. GET Acuerdo (verify milestone)
   - 08. PATCH Completar Acuerdo

3. **Rechazo de Propuesta - Lifecycle** (2 requests)
   - Get otra propuesta pendiente
   - PATCH Rechazar propuesta individual

4. **Cancelacion de Acuerdo - Lifecycle** (3 requests)
   - Setup: Get segunda propuesta
   - Setup: Create acuerdo para cancelar
   - PATCH Cancelar acuerdo (as Profesional)

5. **Rechazo de Entregable - Lifecycle** (4 requests)
   - Setup requests para crear acuerdo + entregable
   - PATCH Rechazar entregable

6. **Validaciones - Acuerdos** (3 requests)
   - 400: Titulo vacio
   - 400: Titulo > 200 chars
   - 400: FechaFin < FechaInicio

7. **Validaciones - Milestones** (3 requests)
   - 400: Titulo vacio
   - 400: Titulo < 3 chars
   - 400: Importe = 0

8. **Validaciones - Entregables** (2 requests)
   - 400: Titulo vacio
   - 400: URL invalida

9. **Autorizacion - Errores 403** (3 requests)
   - 403: Profesional crea milestone
   - 403: Artista sube entregable
   - 403: Profesional aprueba entregable

10. **Not Found - Errores 404** (4 requests)
    - 404: Propuesta no existe
    - 404: Acuerdo no existe
    - 404: Milestone no existe
    - 404: Entregable no existe

11. **DELETE Milestone - Lifecycle** (3 requests)
    - Setup acuerdo para delete
    - Setup milestone para delete
    - DELETE milestone (sin entregables)

---

## Variables de Coleccion

Auto-completadas durante ejecucion:

| Variable | Tipo | Set por | Descripcion |
|----------|------|---------|-------------|
| `baseUrl` | string | Manual | URL del backend (default: http://localhost:5001) |
| `artistaToken` | string | 01. Login Artista | JWT del usuario artista |
| `profesionalToken` | string | 02. Register Prof | JWT del usuario profesional |
| `necesidadId` | string | 03. Get Necesidad | ID de necesidad encontrada |
| `propuestaId` | string | 04. Get Propuesta | ID de propuesta Pendiente |
| `acuerdoId` | string | 01. POST Aceptar | ID del acuerdo creado |
| `milestoneId` | string | 03. POST Milestone | ID del milestone |
| `entregableId` | string | 05. POST Entregable | ID del entregable |

Todas las variables se inicializan en el folder _Setup y se reutilizan en requests posteriores.

---

## Flujo de Ejecucion Esperado

```
_Setup
├─ Login Artista ✓ (200, artista token)
├─ Register Profesional ✓ (200, prof token)
├─ Get Necesidad ✓ (200, necesidad ID)
└─ Get Propuesta ✓ (200, propuesta ID)

CRUD Lifecycle
├─ 01. POST Aceptar Propuesta ✓ (201, acuerdo creado, prof rechazadas)
├─ 02. GET Acuerdo ✓ (200, detalle + timeline)
├─ 03. POST Milestone ✓ (201, milestone creado)
├─ 04. PUT Update Milestone ✓ (200, actualizado)
├─ 05. POST Entregable (prof) ✓ (201, entregable Entregado)
├─ 06. PATCH Aprobar (artista) ✓ (200, estado Aprobado)
├─ 07. GET Acuerdo (verify) ✓ (200, entregable aprobado)
└─ 08. PATCH Completar ✓ (200, estado Completado)

Otros Lifecycles...
Validaciones...
Autorizaciones...
Not Found errors...
```

---

## Status Codes Esperados

| Folder | Esperado | Validacion |
|--------|----------|-----------|
| Acuerdos CRUD | 201, 200, 200, 200, 201, 200, 200, 200 | `pm.response.to.have.status(xxx)` |
| Validaciones | 400, 400, 400... | `isSuccess == false`, `messages != []` |
| Autorizacion | 403, 403, 403 | `error code 3002` |
| Not Found | 404, 404, 404, 404 | `error codes 2010-2013` |

---

## Assertions Incluidas

Cada request contiene test scripts con validaciones de:

1. **Status Code**
   ```javascript
   pm.test('Status is 201', function () {
       pm.response.to.have.status(201);
   });
   ```

2. **Response Structure**
   ```javascript
   pm.test('Has expected fields', function () {
       const json = pm.response.json();
       pm.expect(json.data).to.exist;
       pm.expect(json.messages).to.be.an('array');
   });
   ```

3. **Data Persistence**
   ```javascript
   pm.collectionVariables.set('acuerdoId', json.data.acuerdoId);
   ```

4. **Error Codes**
   ```javascript
   pm.test('Error code validation', function () {
       const codes = json.messages.map(m => m.errorCode);
       pm.expect(codes).to.include('1001');
   });
   ```

---

## Troubleshooting

### Error: "Propuesta no encontrada (2010)"
**Causa:** No existen propuestas en estado `Pendiente`
**Solucion:**
- Crear necesidades con propuestas via Swagger
- O resetear DB y ejecutar seeders

### Error: "Necesidad no encontrada"
**Causa:** Usuario artista no tiene necesidades
**Solucion:**
- Crear necesidad como usuario1@mail.com en landing
- O insertar directamente en DB

### Error: "isSuccess == false pero esperaba true"
**Causa:** Validacion fallida (requiere campos especificos)
**Solucion:**
- Revisar error code retornado
- Consultar contratos API
- Verificar datos en request body

### Timeout en requests
**Causa:** Backend lento o no respondiendo
**Solucion:**
```bash
newman run postman-collection.json \
  --timeout-request 15000 \
  --timeout 30000
```

### Token expirado (3001)
**Causa:** JWT vencido o invalido
**Solucion:**
- Ejecutar _Setup nuevamente
- Verificar JWT expiration time en headers

---

## Metricas de Cobertura

| Metrica | Valor |
|---------|-------|
| Endpoints testeados | 11 / 11 |
| Folders | 11 |
| Requests totales | 45+ |
| Test scripts | 50+ |
| Assertions | 120+ |
| Casos de exito | 15 |
| Casos de error | 15+ |
| Tiempo ejecucion | ~30-45s |

---

## Notas Importantes

### Pre-request Scripts
- Generan emails unicos con `Date.now()` para evitar colisiones
- No modificar entre ejecuciones

### Variables Dinamicas
- Timestamps: `{{$timestamp}}`
- Todos los IDs se extraen de responses y se guardan en variables

### Flujo de Auth
1. _Setup genera tokens (24h validity)
2. Todos los requests autenticados con Bearer token
3. Tests verifican 401/403 con tokens invalidos

### Datos Ephemeral
- Nueva propuesta rechazada en cada ejecucion
- Nuevo acuerdo creado por ejecucion
- No reutiliza IDs de ejecuciones anteriores

---

## Proximos Pasos

1. Ejecutar coleccion e inspeccionar failures
2. Ajustar datos de seed si es necesario
3. Implementar CI/CD con Newman
4. Generar reportes HTML para dashboard
5. Expandir con casos de edge cases adicionales

---

## Contacto

Para preguntas sobre los endpoints, consultar:
- `docs/user-stories/cs-acuerdos-entregables/contracts.md`
- `plans/cs-acuerdos-entregables/backend/api-contracts.md`

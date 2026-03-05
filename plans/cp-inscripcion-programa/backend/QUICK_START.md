# Coleccion Postman - cp-inscripcion-programa (US-CP-03)

## Resumen Ejecutivo

Coleccion de pruebas de integracion **100% auto-inclusiva** para validar el lifecycle completo de inscripciones de promotores en programas de promocion:

- **Exploracion** de catalogo de programas publicos
- **Solicitud** de inscripcion en un programa
- **Aprobacion** con generacion de codigo referido y URL de tracking
- **Rechazo, Bloqueo y Baja** de promotores
- **Listados** paginados (promotor + artista)
- **Validaciones** de errores (duplicidad, no activo, no propietario)

---

## Metricas de la Coleccion

| Metrica | Valor |
|---------|-------|
| **Folders** | 6 |
| **Requests** | 18 |
| **Assertions** | ~65 |
| **Usuarios necesarios** | 2 (promotor + artista) |
| **Setup automatico** | Si (login ambos usuarios) |
| **Variables coleccion** | 15 |
| **Tiempo ejecucion** | ~15-20 segundos |

---

## Estructura de la Coleccion

### _Setup (2 requests)
1. **Login Promotor** (usuario1@mail.com / 123456)
2. **Login Artista** (api-test@mail.com / 123456)

Ambos generan tokens JWT que se reutilizan en todos los requests.

### Inscripcion - Happy Path (6 requests)
1. **GET Explorar Programas** - Catalogo paginado (como promotor)
2. **POST Solicitar Inscripcion** - Crea inscripcion pendiente
3. **GET Mis Inscripciones** - Listar como promotor (verifica estado Pendiente)
4. **GET Inscripciones del Programa** - Listar como artista (verifica estado Pendiente)
5. **PATCH Aprobar Inscripcion** - Aprueba + genera codigo + URL
6. **GET Mis Inscripciones Verificar** - Valida que aparece como Aprobado con codigo

### Inscripcion - Validaciones (2 requests)
- **POST 400** - Intenta duplicar inscripcion en mismo programa
- **POST 404** - Intenta inscribirse en programa inexistente

### Inscripcion - Acciones Artista (3 requests)
1. **POST Solicitar Nueva** - Crea inscripcion para rechazar
2. **PATCH Rechazar** - Eliminacion fisica del registro
3. **GET Verificar Rechazada** - Confirma que no aparece en lista

### Inscripcion - Bloqueo y Baja (2 requests)
1. **PATCH Bloquear** - Bloquea la inscripcion aprobada anterior
2. **GET Verificar Bloqueada** - Valida estado = "Bloqueado"

### Inscripcion - Errores de Autenticacion (2 requests)
- **GET 401** - Sin token en explorar programas
- **PATCH 403** - Promotor intenta aprobar (no es propietario)

### Inscripcion - Not Found (2 requests)
- **PATCH 404** - Intenta aprobar inscripcion inexistente
- **GET 404** - Intenta listar inscripciones de programa inexistente

---

## Prerequisitos

### 1. Backend ejecutandose
```bash
dotnet run --project src/api/WebApi
```

Debe estar disponible en: `http://localhost:5001`

### 2. Base de datos con usuarios de prueba
Usuarios creados en Docker DB:
- **usuario1@mail.com** / **123456** (Fan, tiene Artista "Usuario 1 Music Actualizado")
- **api-test@mail.com** / **123456** (Fan, tiene Artista "API Test Artist")

### 3. Programas activos existentes
Los programas para las pruebas deben existir en BD. Si no hay programas activos:
- La coleccion falla en "01. GET Explorar Programas"
- Crear un programa via Swagger o manualmente en BD

### 4. Newman instalado (para ejecucion CLI)
```bash
npm install -g newman
npm install -g newman-reporter-htmlextra
```

---

## Como Ejecutar

### Opcion A: Postman GUI

1. **Abrir Postman**
2. **Importar coleccion**
   - File > Import
   - Seleccionar: `plans/cp-inscripcion-programa/backend/postman-collection.json`
3. **Verificar variables**
   - En la coleccion, editar "baseUrl" si es necesario (default: `http://localhost:5001`)
4. **Ejecutar carpetas en orden**
   - **_Setup** (debe ejecutarse primero, genera tokens)
   - **Inscripcion - Happy Path** (flujo completo)
   - Luego las otras carpetas segun se necesite

---

### Opcion B: Newman CLI (Automatizado)

#### Ejecucion completa (todos los folders)
```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --environment <(echo '{"id":"","values":[{"key":"baseUrl","value":"http://localhost:5001"}]}') \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/cp-inscripcion-programa.html
```

#### Ejecucion solo happy path
```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "_Setup,Inscripcion - Happy Path" \
  --reporters cli,htmlextra
```

#### Ejecucion solo validaciones
```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "Inscripcion - Validaciones" \
  --reporters cli
```

---

## Flujo de Ejecucion Esperado

```
START
  ↓
_Setup
  ├─ Login Promotor ✓ (tokens.promotorToken)
  └─ Login Artista ✓ (tokens.artistaToken)
  ↓
Inscripcion - Happy Path
  ├─ 01. GET Explorar Programas ✓ (programaId = primer programa)
  ├─ 02. POST Solicitar Inscripcion ✓ (inscripcionId guardado, estado = Pendiente)
  ├─ 03. GET Mis Inscripciones ✓ (promotor ve inscripcion en estado Pendiente)
  ├─ 04. GET Inscripciones del Programa ✓ (artista ve inscripcion en estado Pendiente)
  ├─ 05. PATCH Aprobar Inscripcion ✓ (codigoReferido generado, estado = Aprobado)
  └─ 06. GET Mis Inscripciones Verificar ✓ (promotor ve codigo referido)
  ↓
[OPCIONAL] Otras validaciones
  └─ Inscripcion - Validaciones / Acciones Artista / Bloqueo / Etc.
  ↓
FINISH
```

---

## Estados de Inscripcion

La maquina de estados se valida en varios requests:

| Transicion | Endpoint | Status | Estado Nuevo | Verificacion |
|------------|----------|--------|--------------|--------------|
| Create | POST Solicitar | 201 | Pendiente | esAprobado=false, esBloqueado=false |
| Aprobar | PATCH Aprobar | 200 | Aprobado | esAprobado=true, codigoReferido existe |
| Rechazar | PATCH Rechazar | 200 | ELIMINADO | Desaparece de lista |
| Bloquear | PATCH Bloquear | 200 | Bloqueado | esBloqueado=true, esAprobado=false |
| Dar de baja | PATCH Baja | 200 | DadoDeBaja | fechaBaja != null |

---

## Codigos de Error Esperados

| ErrorCode | HTTP | Descripcion | Test |
|-----------|------|-------------|------|
| 1001 | 400 | Campo requerido vacio | Validacion |
| 2015 | 404 | Promotor no encontrado | Happy Path |
| 2016 | 404 | Artista no encontrado | Happy Path |
| 2019 | 404 | Programa no encontrado | Not Found |
| 2020 | 404 | Inscripcion no encontrada | Not Found |
| 4021 | 400 | Inscripcion ya existe | Validaciones |
| 4022 | 403 | Promotor bloqueado | Validaciones |
| 4023 | 400 | Promotor inactivo | Validaciones |
| 4024 | 400 | Programa inactivo | Validaciones |
| 4025 | 400 | Estado invalido para operacion | Bloqueo |
| 4026 | 403 | No es propietario del programa | Auth Errors |

---

## Variables de Coleccion

Todas las variables se cargan **automaticamente** durante la ejecucion:

| Variable | Generado en | Usado en |
|----------|-------------|----------|
| `baseUrl` | Manual | Todos los requests |
| `promotorEmail` | _Setup 01 | Login Promotor |
| `promotorPassword` | _Setup 01 | Login Promotor |
| `promotorToken` | _Setup 01 (JWT) | Todos los requests como promotor |
| `artistaEmail` | _Setup 02 | Login Artista |
| `artistaPassword` | _Setup 02 | Login Artista |
| `artistaToken` | _Setup 02 (JWT) | Todos los requests como artista |
| `programaId` | Happy Path 01 | Solicitar / Listar / Aprobar |
| `programaTitulo` | Happy Path 01 | Info |
| `inscripcionId` | Happy Path 02 | Listar / Aprobar / Bloquear |
| `inscripcionId2` | Acciones 01 | Rechazar |
| `codigoReferido` | Happy Path 05 | Info |
| `urlTracking` | Happy Path 05 (opcional) | Info |
| `miEstado` | Happy Path 01 | Info |
| `inscripcionBloqueada` | Bloqueo 01 | Info |

---

## Notas Importantes

1. **Orden de ejecucion**: La carpeta `_Setup` **DEBE** ejecutarse primero para generar tokens. Sin esto, todos los requests fallaran con 401.

2. **Base de datos**: Los usuarios ya existen. No hace falta crear cuentas nuevas.

3. **Programas**: Si el resultado de "01. GET Explorar Programas" devuelve lista vacia, crear un programa activo en BD o via Swagger antes de continuar.

4. **Idempotencia**: Cada ejecucion usa los mismos usuarios de prueba. Para ejecutar multiples veces limpiar la BD o dejar que los datos se acumulen (esto es OK para testing).

5. **JWT Key**: El backend usa clave `WePlayRisesDockerSecretKey123456789`. Los tokens tienen duracion estandar (normalmente 1 hora).

6. **Assertions**: Cada request incluye 2-5 pm.test() con validaciones de status code, estructura de respuesta e IDs guardados en variables.

---

## Troubleshooting

### Error: "Status is 401"
**Causa**: _Setup no se ejecuto o tokens expiraron.
**Solucion**: Ejecutar _Setup nuevamente.

### Error: "items is empty" en Happy Path 01
**Causa**: No hay programas activos en BD.
**Solucion**: Crear programa via Swagger `/api/crowdpromotion/programas` o manualmente en BD.

### Error: "inscripcionId is undefined" en Happy Path 05
**Causa**: El POST en paso 02 fallo (posible inscripcion duplicada).
**Solucion**: Limpiar BD o cambiar usuario de prueba.

### Newman no se instala
```bash
npm install -g newman newman-reporter-htmlextra
# O con yarn
yarn global add newman newman-reporter-htmlextra
```

### Reports no se generan
Crear carpeta `reports/`:
```bash
mkdir -p reports
```

---

## Reportes HTML

Despues de ejecutar con Newman + htmlextra:
```bash
open reports/cp-inscripcion-programa.html  # macOS
start reports\cp-inscripcion-programa.html # Windows
```

El reporte incluye:
- Resumen de ejecucion (total requests, assertions, tiempo)
- Detalles de cada request (status, response time, assertions)
- Errores si los hay (con response body completo)
- Timeline grafico

---

## Comandos Rapidos

```bash
# Ejecucion basica
newman run plans/cp-inscripcion-programa/backend/postman-collection.json

# Con reporte HTML
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters htmlextra --reporter-htmlextra-export reports/test.html

# CLI bonita + HTML
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters cli,htmlextra --reporter-htmlextra-export reports/test.html

# Solo ciertos folders
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "_Setup,Inscripcion - Happy Path"

# Con variables de entorno externas
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --environment env-staging.json

# Con timeout personalizado
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --timeout 5000
```

---

## Documentacion Completa

Ver: `plans/cp-inscripcion-programa/backend/api-contracts.md` para detalles de:
- DTOs de request/response
- Validadores
- Logica de negocio en handlers
- Endpoints completos
- Errores esperados

---

## Autor

Generado automaticamente por el sistema de tooling de WePlay Rises.
Fecha: 2026-02-25
Feature: cp-inscripcion-programa (US-CP-03)

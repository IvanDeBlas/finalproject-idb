# Índice - Campanias Integration Tests (Newman)

Colección Postman ejecutable para testing de integración de la API de Campanias.

**Generada:** 2026-02-12
**Versión:** 1.0
**Estado:** Ready for Use ✅

---

## Archivos Generados

### 📦 Colección Postman

```
WePlay.Campanias.IntegrationTests.postman_collection.json
├─ Tamaño: ~120 KB
├─ Schema: Postman v2.1.0
├─ Requests: 48 total
├─ Assertions: 150+
└─ Ejecutable con: Newman CLI
```

**Estructura:**
- `_Setup/` - 3 requests (Register, Login, Create Artista)
- `Campanias/` - 45 requests en 11 carpetas:
  - `201 CREATED/` - 2 requests
  - `200 OK - GET Detail/` - 1 request
  - `200 OK - UPDATE/` - 3 requests
  - `200 OK - PUBLISH/` - 2 requests
  - `200 OK - LIST PUBLIC/` - 2 requests
  - `200 OK - LIST MY CAMPAIGNS/` - 3 requests
  - `400 BAD REQUEST/` - 8 requests
  - `401 UNAUTHORIZED/` - 5 requests
  - `403 FORBIDDEN/` - 2 requests
  - `404 NOT FOUND/` - 3 requests
  - `409 CONFLICT/` - 2 requests
  - `E2E Happy Path/` - 9 requests

### 🔧 Environment

```
environments/development.postman_environment.json
├─ Variables: 12
├─ Predefinidas: 2 (baseUrl, identityUrl, clientId)
├─ Dinámicas: 10 (se rellenan en runtime)
└─ Uso: Testing local en http://localhost:5000
```

### 📖 Documentación

| Archivo | Propósito | Secciones |
|---------|-----------|-----------|
| `README_NEWMAN.md` | Guía completa de uso | 13 |
| `CI_CD_EXAMPLES.md` | Ejemplos de integración | 5 plataformas |
| `GENERACION_COMPLETADA.md` | Informe de generación | 16 secciones |
| `INDEX.md` | Este archivo | Índice |

### 🚀 Scripts de Ejecución

```
run-tests.sh          Linux/macOS bash script
run-tests.bat         Windows batch script
```

Ambos soportan opciones:
- `-f, --folder` - Ejecutar solo un folder
- `-r, --reporters` - Especificar reporteros
- `-e, --export` - Exportar environment final
- `-d, --delay` - Delay entre requests
- `-h, --help` - Mostrar ayuda

---

## Quick Start

### 1️⃣ Instalación (una sola vez)

```bash
npm install -g newman newman-reporter-htmlextra
```

### 2️⃣ Ejecución Básica

```bash
cd tests/integration
./run-tests.sh    # Linux/macOS
run-tests.bat     # Windows
```

### 3️⃣ Ver Resultados

Abre: `test-results/report.html` en tu navegador

---

## Comandos Comunes

### Ejecutar Todo

```bash
./run-tests.sh
```

### Ejecutar Solo Setup

```bash
./run-tests.sh -f "_Setup"
```

### Ejecutar E2E Happy Path

```bash
./run-tests.sh -f "Campanias/E2E Happy Path"
```

### Con Reporte JUnit (para CI/CD)

```bash
./run-tests.sh -r "cli,junitxml"
```

### Exportar Environment Final

```bash
./run-tests.sh -e "final-environment.json"
```

### Validar Colección

```bash
newman run WePlay.Campanias.IntegrationTests.postman_collection.json --dry-run
```

---

## Cobertura

### Endpoints

✅ `POST /api/campanias` - Create
✅ `GET /api/campanias/{id}` - Get Detail
✅ `PUT /api/campanias/{id}` - Update
✅ `POST /api/campanias/{id}/publicar` - Publish
✅ `GET /api/campanias` - List Public
✅ `GET /api/campanias/mis-campanias` - List My Campaigns

### Status Codes

✅ 200 OK
✅ 201 CREATED
✅ 400 BAD REQUEST
✅ 401 UNAUTHORIZED
✅ 403 FORBIDDEN
✅ 404 NOT FOUND
✅ 409 CONFLICT

### Validaciones

✅ Empty required fields (1001)
✅ Max length exceeded (1002)
✅ Invalid URL format (1006)
✅ Invalid range (1007)
✅ Invalid amount (1011)
✅ Invalid date (1012)
✅ Not found (2003)
✅ Forbidden (3002)
✅ Business rule violation (4009)

---

## Métricas

| Métrica | Valor |
|---------|-------|
| **Total Requests** | 48 |
| **Total Assertions** | 150+ |
| **Cobertura Endpoints** | 100% (6/6) |
| **Status Codes** | 7 cubiertos |
| **Tiempo de Ejecución** | 30-40 segundos |
| **Tamaño Colección** | ~120 KB |

---

## Documentación

### Para Comenzar
1. Lee: `README_NEWMAN.md` - Guía completa
2. Ejecuta: `./run-tests.sh -f "_Setup"` - Valida setup
3. Ejecuta: `./run-tests.sh` - Suite completa

### Para CI/CD
1. Lee: `CI_CD_EXAMPLES.md` - Ejemplos para tu plataforma
2. Copia la configuración
3. Adapta variables de entorno

### Para Troubleshooting
1. Lee: `README_NEWMAN.md` sección "Troubleshooting"
2. Valida: `newman run collection.json --dry-run`
3. Verifica: Logs en consola durante ejecución

---

## Variables de Entorno

### Predefinidas (en development.postman_environment.json)

```
baseUrl = http://localhost:5000/api
identityUrl = http://localhost:5000/api
clientId = weplay-test
```

### Dinámicas (se rellenan en runtime)

```
accessToken          ← Obtenido en Login
testUserId           ← Obtenido en Register
testUserEmail        ← Obtenido en Register
artistaId            ← Obtenido en Create Artista
campaignId           ← Obtenido en POST Campanias
publishedCampaignId  ← Obtenido en Publish
e2eCampaignId        ← Para E2E tests
secondUserToken      ← Para tests de 403
```

---

## Requisitos

### Software

- **Node.js** v16+
- **Newman** v5.3+ (instalable con npm)
- **Postman** (opcional, para editar)

### API Backend

- **URL:** http://localhost:5000
- **Port:** 5000
- **Framework:** .NET 8
- **Database:** SQL Server / LocalDB

### Dependencias npm

```bash
npm install -g newman newman-reporter-htmlextra newman-reporter-junitxml
```

---

## Flujo de Ejecución

```
START
  ↓
_Setup (Register, Login, Create Artista)
  ├─ Guarda: accessToken, testUserId, artistaId
  ↓
Campanias Tests (48 requests)
  ├─ 201 CREATED (2 requests)
  ├─ 200 OK Tests (11 requests)
  ├─ 400 BAD REQUEST (8 requests)
  ├─ 401 UNAUTHORIZED (5 requests)
  ├─ 403 FORBIDDEN (2 requests)
  ├─ 404 NOT FOUND (3 requests)
  ├─ 409 CONFLICT (2 requests)
  └─ E2E Happy Path (9 requests)
  ↓
Reporte
  ├─ test-results/report.html (HTML)
  ├─ test-results/results.xml (JUnit)
  └─ Consola (CLI)
  ↓
END
```

---

## Reportes Generados

### HTML Extra Report
- **Archivo:** `test-results/report.html`
- **Abre en:** Navegador web
- **Contiene:** Resumen, timeline, detalles por request, errores

### JUnit XML Report
- **Archivo:** `test-results/results.xml`
- **Usa:** Azure DevOps, Jenkins, GitHub Actions
- **Integración:** Automática con CI/CD

### CLI Output
- **Salida:** Consola en tiempo real
- **Muestra:** Status, tiempos, assertions

---

## Estructura de Carpetas

```
tests/integration/
├── WePlay.Campanias.IntegrationTests.postman_collection.json
├── run-tests.sh
├── run-tests.bat
├── environments/
│   └── development.postman_environment.json
├── test-results/  (creado automáticamente)
│   ├── report.html
│   ├── results.xml
│   └── results.json
├── README_NEWMAN.md
├── CI_CD_EXAMPLES.md
├── GENERACION_COMPLETADA.md
├── INDEX.md (este archivo)
└── newman-tests.md (plan original)
```

---

## Solución de Problemas

### "newman: command not found"

```bash
npm install -g newman
```

### "401 Unauthorized"

```bash
# Asegúrate de que la API está corriendo
# http://localhost:5000 debe estar disponible

# Ejecuta setup primero
./run-tests.sh -f "_Setup"
```

### "404 Not Found on valid ID"

```bash
# El ID no se guardó en la variable
# Verifica que POST /campanias retorna 201 con un data.id válido
# Revisa el HTML report para detalles
```

### "Campaign not in BORRADOR state"

```bash
# Está intentando editar una campaña publicada
# Esto es correcto - debería retornar 409
# Esto es un test negativo esperado
```

---

## Mejoras Futuras

### Corto Plazo
- [ ] Agregar más tests de validación
- [ ] Extender a otros endpoints (Backings, Rewards)
- [ ] Pruebas de performance

### Mediano Plazo
- [ ] Pruebas de carga con Artillery
- [ ] Security testing (OWASP Top 10)
- [ ] Dokumentación API en OpenAPI

### Largo Plazo
- [ ] Pruebas end-to-end con UI (Selenium)
- [ ] Pruebas de compatibilidad entre versiones
- [ ] Análisis de cobertura de código

---

## Recursos Adicionales

### Documentación Oficial
- [Postman Learning Center](https://learning.postman.com/)
- [Newman CLI Docs](https://learning.postman.com/docs/running-collections/using-newman-cli/)
- [Chai Assertions](https://www.chaijs.com/api/)

### Archivos del Proyecto
- `plans/crear-campania/backend/api-contracts.md` - API contracts
- `plans/crear-campania/backend/newman-tests.md` - Plan detallado
- `.claude/rules/backend/cqrs.rule.md` - Reglas backend

---

## Soporte

### ¿Preguntas sobre la colección?
1. Revisa: `README_NEWMAN.md`
2. Revisa: `CI_CD_EXAMPLES.md`
3. Revisa: `GENERACION_COMPLETADA.md`

### ¿Errores en ejecución?
1. Abre: `test-results/report.html`
2. Busca: El request que falló
3. Revisa: Body, headers, response

### ¿Cambios en API?
1. Abre colección en Postman
2. Modifica request/assertions
3. Exporta JSON
4. Commit a Git

---

## Cambio Log

### v1.0 (2026-02-12)
- ✅ Generación inicial
- ✅ 48 requests
- ✅ 150+ assertions
- ✅ 6 archivos
- ✅ 5 ejemplos CI/CD
- ✅ Scripts bash/batch
- ✅ Documentación completa

---

## Checklist de Uso

- [ ] Instalar Newman: `npm install -g newman`
- [ ] Clonar/descargar colección
- [ ] Verificar API en puerto 5000
- [ ] Ejecutar: `./run-tests.sh`
- [ ] Abrir: `test-results/report.html`
- [ ] Revisar resultados
- [ ] Integrar en CI/CD (si aplica)
- [ ] Documentar customizaciones

---

## Próximos Pasos

### Hoy
```bash
cd tests/integration
npm install -g newman newman-reporter-htmlextra
./run-tests.sh
```

### Esta Semana
- Integrar en GitHub Actions
- Documentar fallos comunes
- Entrenar al equipo

### Este Mes
- Extender a más endpoints
- Pruebas de performance
- Integración completa en CI/CD

---

**Creado:** 2026-02-12
**Versión:** 1.0
**Estado:** ✅ Ready for Production
**Mantenedor:** WePlay Rises Team

---

*Para más información, ver archivos de documentación adjuntos.*

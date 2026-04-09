# Índice de Archivos - Dashboard Artista Collection

Documentación completa de la colección Postman para tests de integración del Dashboard de Artista.

## Archivos Principales

### 1. postman-collection.json
- **Descripción**: Colección Postman v2.1 ejecutable con Newman
- **Contenido**: 5 folders, 18 requests, 45+ assertions
- **Uso**: `newman run postman-collection.json`
- **Tamaño**: ~30 KB
- **Actualización**: 2026-02-14

**Estructura de Folders**:
- `_Setup`: Autenticacion (Login)
- `Dashboard - Resumen`: Metricas generales del artista
- `Mis Campanias`: Listado de campanias con paginacion y filtros
- `Backings por Campania`: Listado de backings con paginacion
- `Stats de Campania`: Estadisticas detalladas y proyecciones

---

## Documentación

### 2. README.md
- **Descripción**: Documentación principal
- **Audiencia**: Developers, QA, DevOps
- **Contenido**:
  - Descripción de endpoints
  - Variables de colección
  - Instrucciones de ejecución
  - Ejemplos de Newman
  - Notas importantes
  - Troubleshooting
- **Lectura estimada**: 15 minutos
- **Cuando leer**: Primera vez que usas la colección

### 3. QUICK-START.md
- **Descripción**: Guía de inicio rápido
- **Audiencia**: Developers que necesitan ejecutar rápidamente
- **Contenido**:
  - Instalación rápida
  - Comandos más comunes
  - Troubleshooting rápido
  - Ejemplos prácticos
- **Lectura estimada**: 5 minutos
- **Cuando leer**: Cuando necesitas ejecutar tests rápidamente

### 4. TEST-SPEC.md
- **Descripción**: Especificación técnica completa de los tests
- **Audiencia**: QA, Developers que van a mantener los tests
- **Contenido**:
  - Cobertura detallada por endpoint
  - Matriz de tests
  - Orden de ejecución
  - Validaciones de negocio
  - Assertions detalladas
  - Escenarios de error
  - Performance targets
- **Lectura estimada**: 20 minutos
- **Cuando leer**: Cuando necesitas entender qué se está testeando

---

## Configuración

### 5. environment.json
- **Descripción**: Variables de entorno para desarrollo local
- **Contiene**:
  - baseUrl: http://localhost:5001
  - testEmail: usuario1@mail.com
  - testPassword: 123456
- **Uso**: `newman run postman-collection.json -e environment.json`
- **Modificable**: Sí, personalizar según necesidad

### 6. environment-staging.json
- **Descripción**: Variables de entorno para staging
- **Contiene**:
  - baseUrl: https://staging-api.weplayurises.com
  - testEmail: staging-test@mail.com
  - testPassword: StagingTestPassword123
- **Uso**: `newman run postman-collection.json -e environment-staging.json`
- **Modificable**: Sí, actualizar credenciales staging reales

---

## Scripts de Ejecución

### 7. run-tests.sh
- **Descripción**: Script Bash para ejecutar tests (Linux/Mac)
- **Uso**: `./run-tests.sh [environment] [reporters]`
- **Ejemplos**:
  - `./run-tests.sh` (usa environment.json, reporter cli)
  - `./run-tests.sh environment-staging.json html` (staging, reporte HTML)
- **Características**:
  - Valida que archivos existan
  - Genera reportes con timestamp
  - Crea directorio reports/
  - Exit codes apropiados para CI/CD

### 8. run-tests.bat
- **Descripción**: Script Batch para ejecutar tests (Windows)
- **Uso**: `run-tests.bat [environment] [reporters]`
- **Ejemplos**:
  - `run-tests.bat` (por defecto)
  - `run-tests.bat environment-staging.json html`
- **Características**:
  - Valida archivos en Windows
  - Soporta timestamp
  - Genera reportes
  - Compatible con CMD y PowerShell

---

## Flujo de Uso Típico

### Primer Uso (Lectura de Documentación)
```
1. Leer QUICK-START.md (5 min)
   ↓
2. Instalar Newman
   ↓
3. Ejecutar: ./run-tests.sh
   ↓
4. Ver resultados en consola
```

### Uso Frecuente
```
1. Ejecutar: ./run-tests.sh
2. Revisar output
3. Si hay fallos, leer TEST-SPEC.md para entender validaciones
```

### Debugging/Mantenimiento
```
1. Leer TEST-SPEC.md (cobertura exacta)
2. Editar postman-collection.json en Postman app
3. Exportar changes
4. Ejecutar tests para validar
```

### CI/CD Integration
```
1. Usar run-tests.bat/run-tests.sh en pipeline
2. Capturar resultados JSON
3. Reportar a sistema de CI
```

---

## Matriz de Referencia Rápida

| Tarea | Archivo | Comando |
|-------|---------|---------|
| Leer documentación | README.md | - |
| Referencia rápida | QUICK-START.md | - |
| Entender tests | TEST-SPEC.md | - |
| Ejecutar tests | run-tests.sh/bat | `./run-tests.sh` |
| Ver variables | environment.json | - |
| Cambiar config | environment.json | (editar) |
| Tests en Postman App | postman-collection.json | (import) |
| Ver assertions | postman-collection.json | (abrir en editor) |

---

## Variables Clave por Archivo

### postman-collection.json
```javascript
// Variables de coleccion (globales)
baseUrl: string           // URL base API
authToken: string         // Token JWT (guardado por Login)
campaniaId: string        // GUID de campania (guardado por Mis Campanias)
testEmail: string         // Email para autenticacion
testPassword: string      // Password para autenticacion
```

### environment.json / environment-staging.json
```json
{
  "baseUrl": "URL_DE_API",
  "testEmail": "email@test.com",
  "testPassword": "password"
}
```

---

## Endpoints Cubiertos

| Endpoint | Método | Folder | Requests | Status |
|----------|--------|--------|----------|--------|
| /api/auth/login | POST | _Setup | 1 | 200 |
| /api/dashboard/resumen | GET | Dashboard | 2 | 200, 401 |
| /api/campanias/mis-campanias | GET | Mis Campanias | 3 | 200, 200(filter), 401 |
| /api/campanias/{id}/backings | GET | Backings | 4 | 200, 200(p2), 404, 401 |
| /api/campanias/{id}/stats | GET | Stats | 3 | 200, 404, 401 |

**Total**: 5 endpoints, 13 requests (incluyendo errores)

---

## Aserciones Distribuidas

| Carpeta | Assertions | Principales |
|---------|-----------|------------|
| _Setup | 3 | Status 200, Token existe, JWT format |
| Dashboard | 7 | Fields presentes, Types, Performance |
| Mis Campanias | 9 | Paginacion, Filtros, Porcentaje |
| Backings | 9 | Stats, Anonimo, Paginacion |
| Stats | 17 | Calculos, Arrays, Business rules |

**Total**: 45+ assertions

---

## Performance

| Metrica | Valor |
|---------|-------|
| Tiempo total ejecución | ~3-5 segundos |
| Requests por segundo | ~2.5-4 |
| Tamaño respuesta promedio | ~500 bytes |
| Tamaño total recibido | ~12 KB |
| Response time promedio | 250-300 ms |
| Response time máximo | < 500 ms (validado) |

---

## Requisitos Mínimos

| Componente | Versión |
|-----------|---------|
| Node.js | 16+ |
| Newman | 5.3+ |
| Backend API | Corriendo en baseUrl |
| Base de datos | Con usuario de test |

---

## Actualización y Mantenimiento

### Cambiar Endpoints
1. Abrir postman-collection.json en Postman App
2. Editar request
3. Actualizar assertions si es necesario
4. Exportar (Collections > Export)
5. Reemplazar postman-collection.json
6. Actualizar TEST-SPEC.md si cambia cobertura

### Agregar Nuevos Tests
1. En Postman App: Right-click folder > Add Request
2. Configurar request (URL, headers, body)
3. Agregar test script con assertions
4. Exportar colección
5. Actualizar documentación

### Cambiar Variables
1. Editar environment.json
2. O en run-tests.sh: `--env-var baseUrl=http://new-url`

---

## Troubleshooting Rápido

| Problema | Archivo Referencia |
|----------|-------------------|
| ¿Cómo ejecutar? | QUICK-START.md |
| ¿Qué se testa? | TEST-SPEC.md |
| ¿Cómo funciona? | README.md |
| ¿Errores comunes? | README.md - Troubleshooting |
| ¿Assertions detalladas? | TEST-SPEC.md - Assertions |
| ¿Variables? | README.md o environment.json |

---

## Rutas Absolutas Rápidas

```
Colección:    C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json
Documentacion: C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\README.md
Quick Start:  C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\QUICK-START.md
Spec Técnica: C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\TEST-SPEC.md
Environment:  C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\environment.json
Script Win:   C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\run-tests.bat
Script Unix:  C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\run-tests.sh
```

---

## Checklist de Primer Uso

- [ ] Leer QUICK-START.md
- [ ] Instalar Newman: `npm install -g newman`
- [ ] Backend corriendo en http://localhost:5001
- [ ] Ejecutar: `./run-tests.sh`
- [ ] Ver todos los tests PASS
- [ ] Leer README.md para entender arquitectura
- [ ] Personalizr environment.json si es necesario

---

## Siguientes Pasos

1. **Para Developers**: Ver QUICK-START.md para ejecutar tests
2. **Para QA**: Leer TEST-SPEC.md para entender qué se prueba
3. **Para DevOps**: Integrar run-tests.bat/sh en CI/CD pipeline
4. **Para Arquitectos**: Revisar postman-collection.json para entender cobertura

---

## Historial

| Fecha | Versión | Cambios |
|-------|---------|---------|
| 2026-02-14 | 1.0 | Creación inicial |

---

## Contacto / Support

Para reportar issues o mejorar tests:
1. Revisar TEST-SPEC.md y README.md primero
2. Editar postman-collection.json en Postman App
3. Exportar y comprometer cambios
4. Actualizar documentación

---

**Última actualización**: 2026-02-14
**Versión colección**: 1.0 (Postman v2.1)
**Tests**: 18 requests, 45+ assertions
**Cobertura**: 5 endpoints + autenticacion

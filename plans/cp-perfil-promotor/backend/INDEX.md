# Índice de Archivos: Colección Postman cp-perfil-promotor

**Fecha:** 2026-02-25
**Feature:** US-CP-01 - Perfil de Promotor
**Módulo:** Crowdpromotion

---

## Archivos Entregados

### 1. postman-collection.json

**Ubicación:** `plans/cp-perfil-promotor/backend/postman-collection.json`

**Descripción:** Colección Postman v2.1 completa, ejecutable y auto-inclusiva.

**Contenido:**
- 5 Folders (Setup + 4 Feature folders)
- 20 Requests con pre/test scripts
- 9 Variables de colección
- ~64 Assertions
- Cobertura: 4 endpoints, 9 error codes

**Usar para:**
- Importar en Postman UI
- Ejecutar con Newman CLI
- Documentar APIs
- Testing de integración

**Tamaño:** ~80 KB (minificado)

**Validación:** ✓ JSON v2.1 válido, parseado correctamente

---

### 2. QUICK_START.md

**Ubicación:** `plans/cp-perfil-promotor/backend/QUICK_START.md`

**Descripción:** Guía de 2-3 minutos para ejecutar la colección.

**Contenido:**
- Instrucciones de 30 segundos
- Comandos copy-paste
- Tabla de variables
- Output esperado
- Troubleshooting mínimo

**Usar para:**
- Primera vez usando colección
- Ejecutar rápidamente
- Referencia rápida de comandos

**Público:** Developers, QA, DevOps

**Tiempo de lectura:** 2-3 minutos

---

### 3. POSTMAN_COLLECTION_GUIDE.md

**Ubicación:** `plans/cp-perfil-promotor/backend/POSTMAN_COLLECTION_GUIDE.md`

**Descripción:** Guía detallada de la colección con patrones y troubleshooting.

**Contenido:**
- Descripción de endpoints (4 totales)
- Estructura de folders
- Variables de colección (9 totales)
- Instrucciones Postman UI + Newman
- Flujo CRUD completo (6 pasos)
- Casos de error (13 tests)
- Patrones usados (pre-request, test scripts)
- Troubleshooting completo
- Referencias técnicas

**Usar para:**
- Entender qué testa cada request
- Aprender patrones Postman
- Resolver problemas
- Ejecutar folios específicas
- Implementar cambios

**Público:** Backend developers, QA engineers

**Tiempo de lectura:** 10-15 minutos

---

### 4. COLLECTION_EXECUTION_SUMMARY.md

**Ubicación:** `plans/cp-perfil-promotor/backend/COLLECTION_EXECUTION_SUMMARY.md`

**Descripción:** Resumen ejecutivo con estadísticas, cobertura y patrones.

**Contenido:**
- Cobertura de endpoints (4/4 = 100%)
- Estructura de validaciones
- Flujo de datos detallado
- Estadísticas (requests, assertions, codes)
- Casos cubiertos por folder
- Validaciones por request
- Status codes cubiertos (5/5)
- Error codes cubiertos (9/9)
- Ejecución paso a paso
- Pre-requisitos
- Output esperado Newman

**Usar para:**
- Revisar cobertura
- Validar completitud
- Documentar metrics
- Reportes a stakeholders
- Planificar CI/CD

**Público:** Tech leads, project managers, QA leads

**Tiempo de lectura:** 15-20 minutos

---

### 5. DELIVERABLE_SUMMARY.md

**Ubicación:** `plans/cp-perfil-promotor/backend/DELIVERABLE_SUMMARY.md`

**Descripción:** Checklist formal de entrega y validación.

**Contenido:**
- Lista de archivos entregados
- Validación JSON
- Estructura de folders
- Variables de colección
- Cobertura por endpoint (POST, GET, PUT, PATCH)
- Error codes cubiertos (success, validation, notfound, auth, business)
- CRUD lifecycle completo
- Pre/test scripts
- Estadísticas finales (20 requests, 64 assertions)
- Instrucciones de uso
- Requisitos cumplidos (checklist)
- Guías incluidas
- Siguientes pasos
- Notas técnicas
- Validación de contratos
- Calidad & testing
- Checklist final
- Sign-off

**Usar para:**
- Validar entrega
- Documentación formal
- Handoff a equipo
- Tracking de requisitos
- Histórico de cambios

**Público:** Tech lead, product manager, stakeholders

**Tiempo de lectura:** 20-25 minutos

---

### 6. INDEX.md (Este archivo)

**Ubicación:** `plans/cp-perfil-promotor/backend/INDEX.md`

**Descripción:** Índice y guía de navegación para todos los archivos.

**Contenido:**
- Lista de 6 archivos generados
- Descripción de cada uno
- Ubicación exacta
- Público recomendado
- Tiempo de lectura
- Cómo usar

**Usar para:**
- Navegar entre documentos
- Saber qué leer primero
- Referencia rápida

**Público:** Todos

**Tiempo de lectura:** 3-5 minutos

---

## Cómo Navegar

### Si tienes 2 minutos...

👉 Lee: **QUICK_START.md**
- Ejecuta la colección rápido
- Verifica que funciona
- Done

---

### Si tienes 15 minutos...

👉 Lee en orden:
1. **QUICK_START.md** (2 min) - Contexto rápido
2. **POSTMAN_COLLECTION_GUIDE.md** (13 min) - Detalles completos

---

### Si tienes 30 minutos...

👉 Lee todo:
1. **QUICK_START.md** (2 min)
2. **POSTMAN_COLLECTION_GUIDE.md** (13 min)
3. **COLLECTION_EXECUTION_SUMMARY.md** (10 min)
4. **DELIVERABLE_SUMMARY.md** (5 min)

---

### Si necesitas validar entrega...

👉 Lee:
1. **INDEX.md** (este) (3 min) - Visión general
2. **DELIVERABLE_SUMMARY.md** (25 min) - Checklist de entrega
3. **COLLECTION_EXECUTION_SUMMARY.md** (15 min) - Estadísticas de cobertura

---

### Si necesitas resolver un problema...

👉 Lee:
1. **QUICK_START.md** - Troubleshooting rápido
2. **POSTMAN_COLLECTION_GUIDE.md** - Sección Troubleshooting
3. **postman-collection.json** - Ver script específico

---

## Matriz de Públicos

| Rol | Primero | Luego | Referencia |
|-----|---------|-------|-----------|
| **Developer Backend** | QUICK_START | POSTMAN_GUIDE | postman-collection.json |
| **QA Engineer** | QUICK_START | POSTMAN_GUIDE | COLLECTION_SUMMARY |
| **DevOps** | QUICK_START | COLLECTION_SUMMARY | postman-collection.json |
| **Tech Lead** | DELIVERABLE_SUMMARY | COLLECTION_SUMMARY | POSTMAN_GUIDE |
| **Product Manager** | DELIVERABLE_SUMMARY | COLLECTION_SUMMARY | INDEX |
| **Stakeholder** | DELIVERABLE_SUMMARY | INDEX | — |

---

## Tabla Rápida de Contenidos

| Documento | Tema Principal | Páginas | Tiempo |
|-----------|-----------------|---------|--------|
| INDEX.md | Navegación | 2-3 | 3 min |
| QUICK_START.md | Ejecutar | 2-3 | 2 min |
| POSTMAN_GUIDE.md | Detalle técnico | 8-10 | 15 min |
| COLLECTION_SUMMARY.md | Estadísticas | 6-8 | 20 min |
| DELIVERABLE_SUMMARY.md | Checklist | 10-12 | 25 min |
| postman-collection.json | Ejecución | 550+ líneas | N/A |

**Total Documentación:** ~27-35 páginas (equivalente)
**Total Lectura:** ~65-70 minutos (si lees todo)
**Colección Completa:** 20 requests, 64 assertions, 4 endpoints

---

## Estructura de Directorios

```
plans/cp-perfil-promotor/backend/
├── postman-collection.json                 [EJECUTABLE]
├── QUICK_START.md                          [2 min]
├── POSTMAN_COLLECTION_GUIDE.md             [15 min]
├── COLLECTION_EXECUTION_SUMMARY.md         [20 min]
├── DELIVERABLE_SUMMARY.md                  [25 min]
└── INDEX.md                                [3 min - este]
```

---

## Checklist: Qué Leer según tu Tarea

### Importar colección en Postman
- [ ] QUICK_START.md (Paso a paso)
- [ ] postman-collection.json (Importar)

### Ejecutar tests manualmente
- [ ] QUICK_START.md (Setup)
- [ ] POSTMAN_COLLECTION_GUIDE.md (Si algo falla)

### Ejecutar en pipeline CI/CD
- [ ] QUICK_START.md (Comandos)
- [ ] COLLECTION_EXECUTION_SUMMARY.md (Métricas)

### Revisar cobertura
- [ ] DELIVERABLE_SUMMARY.md (Checklist)
- [ ] COLLECTION_EXECUTION_SUMMARY.md (Detalles)

### Entender patrones Postman
- [ ] POSTMAN_COLLECTION_GUIDE.md (Sección Patrones)
- [ ] postman-collection.json (Ver scripts)

### Documentar para equipo
- [ ] DELIVERABLE_SUMMARY.md (Entrega)
- [ ] COLLECTION_EXECUTION_SUMMARY.md (Estadísticas)

### Resolver problema
- [ ] QUICK_START.md (Quick fix)
- [ ] POSTMAN_COLLECTION_GUIDE.md (Troubleshooting)
- [ ] postman-collection.json (Debug request)

---

## Referencias Cruzadas

### postman-collection.json

**Referenciado en:**
- QUICK_START.md (cómo importar)
- POSTMAN_COLLECTION_GUIDE.md (estructura)
- COLLECTION_EXECUTION_SUMMARY.md (estadísticas)
- DELIVERABLE_SUMMARY.md (validación)

---

### QUICK_START.md

**Referencia a:**
- POSTMAN_COLLECTION_GUIDE.md (detalles)
- postman-collection.json (importar)

---

### POSTMAN_COLLECTION_GUIDE.md

**Referencia a:**
- postman-collection.json (específicos requests)
- QUICK_START.md (para rápido)
- docs/user-stories/cp-perfil-promotor/contracts.md (contratos)

---

### COLLECTION_EXECUTION_SUMMARY.md

**Referencia a:**
- postman-collection.json (estadísticas)
- DELIVERABLE_SUMMARY.md (validación)
- POSTMAN_COLLECTION_GUIDE.md (patrones)

---

### DELIVERABLE_SUMMARY.md

**Referencia a:**
- postman-collection.json (archivo)
- POSTMAN_COLLECTION_GUIDE.md (guía)
- COLLECTION_EXECUTION_SUMMARY.md (metrics)

---

## Cambios Futuros

Si necesitas **actualizar la colección**:

1. **Modifica:** postman-collection.json
2. **Actualiza:** POSTMAN_COLLECTION_GUIDE.md (si cambias estructura)
3. **Actualiza:** COLLECTION_EXECUTION_SUMMARY.md (si cambias métricas)
4. **Actualiza:** QUICK_START.md (si cambias instrucciones)
5. **Actualiza:** DELIVERABLE_SUMMARY.md (si cambias requisitos)

---

## Versionado

**Colección:** v1.0
**Documentación:** v1.0
**Fecha:** 2026-02-25
**Feature:** US-CP-01
**Módulo:** Crowdpromotion

---

## Soporte & Contacto

**Documentación:** Este archivo + QUICK_START.md
**Errores:** Ver POSTMAN_COLLECTION_GUIDE.md → Troubleshooting
**Cambios:** Modificar postman-collection.json
**Preguntas:** Ver DELIVERABLE_SUMMARY.md → Notas Técnicas

---

## Quick Links

| Enlace | Archivo | Propósito |
|--------|---------|----------|
| Ejecutar ahora | QUICK_START.md | Setup inmediato |
| Detalles técnicos | POSTMAN_COLLECTION_GUIDE.md | Entender qué testa |
| Cobertura | COLLECTION_EXECUTION_SUMMARY.md | Validar completitud |
| Checklist | DELIVERABLE_SUMMARY.md | Validar entrega |
| Código | postman-collection.json | JSON ejecutable |

---

**Fin del Índice**

Para empezar: 👉 **QUICK_START.md**

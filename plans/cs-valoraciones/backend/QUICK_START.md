# Quick Start: Valoraciones Integration Tests

**Feature:** cs-valoraciones (US-CS-06)
**Colección:** `plans/cs-valoraciones/backend/postman-collection.json`

---

## 30 Segundos de Setup

### Prerequisitos
- Backend corriendo en `http://localhost:5001`
- BD con usuario `usuario1@mail.com` / `123456` (estándar del proyecto)
- Newman instalado: `npm install -g newman newman-reporter-htmlextra`

### Ejecutar Tests
```bash
# Navegar a raíz del proyecto
cd C:\Repos\WePlay_Rises

# Ejecutar todas las pruebas
newman run plans/cs-valoraciones/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/valoraciones-test-report.html
```

**Resultado:** Terminal muestra resumen de tests. HTML report en `reports/valoraciones-test-report.html`.

---

## Pasos Individuales (Postman UI)

### 1. Importar Colección
```
File → Import → Seleccionar plans/cs-valoraciones/backend/postman-collection.json
```

### 2. Establecer Base URL
```
Collections → Variables → baseUrl = "http://localhost:5001"
```

### 3. Ejecutar _Setup
```
Click derecho en "_Setup" → Run Collection
```
Esperar a que complete (8 requests). Verifica que todos son 200/201.

### 4. Ejecutar Happy Path
```
Click derecho en "Valoraciones - Happy Path" → Run Collection
```
3 requests que validan el flujo completo.

### 5. Ejecutar Tests de Error
```
Click derecho en "Valoraciones - Validation Errors" → Run Collection
Click derecho en "Valoraciones - Auth Errors" → Run Collection
Click derecho en "Valoraciones - Not Found" → Run Collection
Click derecho en "Valoraciones - Business Rules" → Run Collection
```

---

## Comando Rápido (Con Script)

Si tienes permisos de ejecución:

```bash
# Hacer script ejecutable
chmod +x plans/cs-valoraciones/backend/run-tests.sh

# Ejecutar todas las pruebas
./plans/cs-valoraciones/backend/run-tests.sh all

# Ejecutar solo setup + happy path
./plans/cs-valoraciones/backend/run-tests.sh happy

# Ejecutar solo tests de validación
./plans/cs-valoraciones/backend/run-tests.sh validation
```

---

## Validaciones Clave

La colección verifica:

### ✓ Endpoints Correctos
- `POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones` → 201
- `GET /api/crowdsourcing/usuarios/{userId}/valoraciones` → 200

### ✓ Errores de Validación
- Puntuación obligatoria (1001)
- Puntuación entre 1-5 (1009)
- Comentario max 1000 caracteres (1002)

### ✓ Reglas de Negocio
- Acuerdo debe estar Completado (4014)
- Solo una valoración por usuario/acuerdo (4017)

### ✓ Autenticación
- Token requerido (401)
- Token inválido (401)

### ✓ Recursos
- Acuerdo no encontrado (404 - 2011)
- Usuario no encontrado (404 - 2000)

### ✓ Estructura de Datos
- Response ServiceResponse con `data` y `messages`
- Resumen con media, total, distribución
- Listado paginado con ordenamiento DESC

---

## Troubleshooting Rápido

| Problema | Solución |
|----------|----------|
| Setup falla en Login Artista | Verificar que `usuario1@mail.com` existe en BD |
| Backend no responde | Confirmar que http://localhost:5001 está activo |
| "Acuerdo no encontrado" después de setup | Paso 8 del setup debe ejecutarse y completarse |
| Newman: "command not found" | Instalar: `npm install -g newman newman-reporter-htmlextra` |
| Tests de validación fallan | Backend puede estar sin la constante `1009` implementada |

---

## Estructura de Carpetas

```
plans/cs-valoraciones/backend/
├── postman-collection.json          # Colección ejecutable
├── POSTMAN_COLLECTION_README.md     # Documentación detallada
├── QUICK_START.md                   # Este archivo
├── run-tests.sh                     # Script de ejecución
└── api-contracts.md                 # Especificación técnica

reports/
└── valoraciones_*.html              # Reports de ejecución
```

---

## Tiempo Estimado de Ejecución

| Modo | Tiempo |
|------|--------|
| Setup solo | ~10-15 seg |
| Setup + Happy Path | ~15-20 seg |
| Setup + Happy Path + Validation | ~25-30 seg |
| Todos los tests | ~45-60 seg |

---

## Logs Útiles

### Ver request/response en Postman
```
Collections → Valoraciones - Happy Path → 01. POST Create Valoracion
→ Click "Send" → Tab "Body" (response) y "Tests" (scripts)
```

### Ver logs en Newman
```bash
newman run ... --reporters cli
# Muestra request/response de cada test
```

### Generar reporte JSON para parsing
```bash
newman run ... --reporter-json-export reports/results.json
# Para procesar resultados programáticamente
```

---

## Siguiente Paso

Una vez que todos los tests pasen:

1. **Backend:** Implementar la feature según `plans/cs-valoraciones/backend/api-contracts.md`
2. **Frontend:** Implementar UI según `docs/user-stories/cs-valoraciones/feature-spec.md`
3. **E2E Tests:** Agregar tests Cypress en `tests/e2e/`
4. **Documentación:** Actualizar README del proyecto

---

## Preguntas Frecuentes

**P: ¿Necesito fixtures de BD?**
R: No. El _Setup genera todo automáticamente. Solo necesita `usuario1@mail.com` pre-existente.

**P: ¿Puedo ejecutar tests en paralelo?**
R: No recomendado. Newman ejecuta secuencialmente. Ejecutar múltiples instancias creará conflictos de email únicos.

**P: ¿Qué variables puedo cambiar?**
R: En Postman UI, edita `Collections → cs-valoraciones → Variables`. Variables dinámicas (email profesional, timestamps) se generan automáticamente.

**P: ¿Los tests son destructivos?**
R: No. Solo crean datos de prueba nuevos. El usuario `usuario1@mail.com` no es modificado, solo usado para login.

---

**Última actualización:** 2026-02-21
**Autor:** Claude Agent - WePlay Rises Integration Testing

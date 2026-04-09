# Quick Start - Dashboard Artista Tests

Guía rápida para ejecutar los tests de integración.

## Instalación Rápida

### 1. Instalar Newman (si no lo tienes)
```bash
npm install -g newman
```

### 2. Verificar instalación
```bash
newman --version
```

## Ejecución Rápida

### Opción 1: Desde la línea de comandos (Linux/Mac)
```bash
cd /C/Repos/WePlay_Rises/plans/dashboard-artista/backend
chmod +x run-tests.sh
./run-tests.sh
```

### Opción 2: Windows (Batch)
```cmd
cd C:\Repos\WePlay_Rises\plans\dashboard-artista\backend
run-tests.bat
```

### Opción 3: Newman directo
```bash
newman run C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json
```

## Ejecución con Opciones

### Ejecutar solo una carpeta
```bash
# Solo Setup
newman run postman-collection.json --folder "_Setup"

# Solo Dashboard
newman run postman-collection.json --folder "Dashboard - Resumen"

# Solo Backings
newman run postman-collection.json --folder "Backings por Campania"
```

### Generar reporte HTML
```bash
newman run postman-collection.json \
    --reporters html \
    --reporter-html-export test-report.html
```

### Con timeout aumentado
```bash
newman run postman-collection.json --timeout 10000
```

### Ambiente staging
```bash
newman run postman-collection.json \
    --environment environment-staging.json
```

### Cambiar URL base sin crear environment
```bash
newman run postman-collection.json \
    --env-var baseUrl=http://staging:5001
```

### Múltiples iteraciones
```bash
newman run postman-collection.json --iterations 5
```

### Sin parar en errores (útil para ver todos los fallos)
```bash
newman run postman-collection.json --bail off
```

## Uso en Postman App

### 1. Importar la colección
- Abrir Postman
- Click en "Import" (esquina superior izquierda)
- Seleccionar archivo: `postman-collection.json`
- Click "Import"

### 2. Configurar variables
- En la colección, hacer click en "..." -> "Edit"
- Ir a tab "Variables"
- Configurar `baseUrl`, `testEmail`, `testPassword`

### 3. Ejecutar colección
- Hacer click en "..." (junto al nombre de colección)
- Seleccionar "Run collection"
- Click en botón "Run Dashboard Artista..."

### 4. Ver resultados
- Resultados se muestran en ventana de ejecución
- Cada request muestra status y tiempo de respuesta
- Assertions fallidas se marcan en rojo

## Troubleshooting Rápido

| Problema | Solución |
|----------|----------|
| Command not found: newman | Instalar: `npm install -g newman` |
| Connection refused | Verificar backend en http://localhost:5001 |
| 401 Unauthorized | Ejecutar primero "_Setup" para obtener token |
| 404 Not Found | Usar campaniaId válido (se guarda auto desde Mis Campanias) |
| ECONNREFUSED | Backend no está corriendo |
| Timeout | Aumentar timeout: `--timeout 10000` |

## Verificación Rápida

### ¿Backend está corriendo?
```bash
curl http://localhost:5001/swagger
```
Si retorna HTML, el backend está vivo.

### ¿Credentials válidas?
```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"usuario1@mail.com","password":"123456"}'
```
Si retorna un token, las credenciales son válidas.

### ¿Estructura JSON correcta?
```bash
newman run postman-collection.json --reporters json > results.json
cat results.json | jq '.run.stats'
```

## Ejemplos de Comandos Completos

### Desarrollo local - Simple
```bash
newman run C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json
```

### Desarrollo local - Con reporte
```bash
newman run C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json \
    --reporters cli,html \
    --reporter-html-export report.html
```

### Staging - Con credenciales
```bash
newman run C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json \
    --environment C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\environment-staging.json
```

### CI/CD - Modo estricto
```bash
newman run C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json \
    --reporters cli,json \
    --reporter-json-export test-results.json \
    --bail on \
    --timeout-request 5000
```

### Producción - Con retry
```bash
newman run C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json \
    --environment C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\environment-staging.json \
    --reporters cli \
    --delay-request 500 \
    --timeout-request 10000
```

## Interpretar Resultados

### Output Simple (CLI)
```
┌─────────────────────────────┬──────────┬──────────┐
│                             │ executed │ failed   │
├─────────────────────────────┼──────────┼──────────┤
│ iterations                  │ 1        │ 0        │
├─────────────────────────────┼──────────┼──────────┤
│ requests                    │ 13       │ 0        │
├─────────────────────────────┼──────────┼──────────┤
│ test-scripts                │ 13       │ 0        │
├─────────────────────────────┼──────────┼──────────┤
│ prerequest-scripts          │ 0        │ 0        │
├─────────────────────────────┼──────────┼──────────┤
│ assertions                  │ 45       │ 0        │
├─────────────────────────────┼──────────┼──────────┤
│ total run duration: 3s      │          │          │
├─────────────────────────────┼──────────┼──────────┤
│ total data received: 12.5kB │          │          │
├─────────────────────────────┼──────────┼──────────┤
│ average response time: 250ms │          │          │
└─────────────────────────────┴──────────┴──────────┘
```

### Significados
- **executed**: Total requests ejecutados
- **failed**: Requests que fallaron
- **assertions**: Total validaciones (test scripts)
- **total run duration**: Tiempo total de ejecución
- **average response time**: Promedio de latencia

## Guardar en Excel/CSV

```bash
# JSON output
newman run postman-collection.json --reporters json > results.json

# Procesar con jq para CSV
jq -r '.run.executions[] | [.request.name, .response.code, .response.responseTime] | @csv' results.json > results.csv
```

## Ejecutar Periodicamente (Linux/Mac)

### Cada 30 minutos
```bash
*/30 * * * * cd /C/Repos/WePlay_Rises/plans/dashboard-artista/backend && ./run-tests.sh > /tmp/dashboard-tests.log 2>&1
```

### Cada día a las 9:00 AM
```bash
0 9 * * * cd /C/Repos/WePlay_Rises/plans/dashboard-artista/backend && ./run-tests.sh > /tmp/dashboard-tests.log 2>&1
```

## Variables Avanzadas

### Cambiar múltiples variables
```bash
newman run postman-collection.json \
    --env-var baseUrl=http://staging:5001 \
    --env-var testEmail=staging@test.com \
    --env-var testPassword=StagingPass123
```

### Usar archivo de variables JSON
```bash
cat > vars.json << EOF
{
  "baseUrl": "http://staging:5001",
  "testEmail": "test@example.com",
  "testPassword": "TestPass123"
}
EOF

newman run postman-collection.json \
    -e environment.json \
    --env-var-file vars.json
```

## Testing en Paralelo (experimental)

Ejecutar varias carpetas en paralelo:
```bash
newman run postman-collection.json --folder "Dashboard - Resumen" &
newman run postman-collection.json --folder "Backings por Campania" &
wait
```

## Monitoreo de Performance

### Identificar requests lentos
```bash
newman run postman-collection.json --reporters json | \
    jq '.run.executions[] | select(.response.responseTime > 300) | {name: .request.name, time: .response.responseTime}'
```

### Response time máximo
```bash
newman run postman-collection.json --reporters json | \
    jq '[.run.executions[].response.responseTime] | max'
```

## Debugging

### Ver headers de request
```bash
newman run postman-collection.json \
    --reporters cli \
    --verbose
```

### Ver request y response completos
```bash
newman run postman-collection.json \
    --reporters cli \
    --verbose \
    --export-environment debug-env.json
```

## Pasos Siguientes

1. **Leer TEST-SPEC.md**: Especificación técnica detallada
2. **Leer README.md**: Documentación completa
3. **Modificar environment.json**: Configurar para tu entorno
4. **Ejecutar regularmente**: Integrar con CI/CD

## Soporte

Si encontras problemas:

1. Verificar README.md sección "Troubleshooting"
2. Revisar TEST-SPEC.md para validaciones exactas
3. Ver logs con `--verbose`
4. Exportar variables de debugging: `--export-environment debug.json`

# Dashboard Artista - Colección Postman

Colección JSON ejecutable para tests de integración del Dashboard de Artista (WePlay Rises MVP).

## Contenido

La colección incluye 4 carpetas principales:

### 1. _Setup
- **Login - Get Auth Token**: Autentica con email/password y guarda el token JWT en `authToken`

### 2. Dashboard - Resumen
- **200 - GET Resumen**: Obtiene metricas generales del artista (total recaudado, backers, campanias activas)
- **401 - Sin Autenticacion**: Verifica que el endpoint requiere autenticacion

### 3. Mis Campanias
- **200 - GET Mis Campanias (Pagina 1)**: Lista campanias del artista con paginacion
- **200 - GET Mis Campanias (Con filtro estado)**: Filtra campanias por estado (ej: EnCurso)
- **401 - Sin Autenticacion**: Verifica requerimiento de autenticacion

### 4. Backings por Campania
- **200 - GET Backings (Pagina 1)**: Lista backings de una campania (paginado, 20 items)
- **200 - GET Backings (Pagina 2)**: Verifica paginacion funciona correctamente
- **404 - Campania No Existe**: Verifica manejo de campanias inexistentes
- **401 - Sin Autenticacion**: Verifica requerimiento de autenticacion

### 5. Stats de Campania
- **200 - GET Stats**: Obtiene estadisticas completas (metricas, rewards, progreso por dia)
- **404 - Campania No Existe**: Verifica manejo de errores
- **401 - Sin Autenticacion**: Verifica requerimiento de autenticacion

## Variables

| Variable | Valor Default | Descripcion |
|----------|---------------|-------------|
| `baseUrl` | http://localhost:5001 | URL base de la API |
| `authToken` | (vacio) | Token JWT guardado por Login |
| `campaniaId` | (vacio) | ID de campania obtenido de Mis Campanias |
| `testEmail` | usuario1@mail.com | Usuario de test |
| `testPassword` | 123456 | Contraseña de test |

## Ejecucion

### Requisitos
- Node.js 16+
- Newman: `npm install -g newman`
- Backend corriendo en http://localhost:5001

### Ejecutar la coleccion completa
```bash
newman run C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json
```

### Ejecutar con environment personalizado
```bash
newman run C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json \
    --environment environment.json
```

### Ejecutar carpeta especifica
```bash
# Solo tests de autenticacion
newman run C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json \
    --folder "_Setup"

# Solo tests de resumen
newman run C:\Repos\WePlay_Rises\plans\dashboard-artista\backend\postman-collection.json \
    --folder "Dashboard - Resumen"
```

### Opciones utiles
```bash
# Con reporte HTML
newman run postman-collection.json --reporters html --reporter-html-export report.html

# Con ambiente y variables de entorno
newman run postman-collection.json -e environment.json --env-var baseUrl=http://staging:5001

# Iteraciones multiples
newman run postman-collection.json --iterations 5

# Sin parar en errores
newman run postman-collection.json --bail off
```

## Tests Incluidos

Cada request tiene assertions que validan:

- **Status Code**: Verifica el codigo HTTP esperado (200, 401, 404)
- **Response Time**: Validacion de performance (< 500ms)
- **Schema**: Estructura JSON con campos obligatorios
- **Tipos**: Validacion de tipos de datos (number, string, array, boolean)
- **Valores**: Rangos y validaciones de negocio (porcentaje 0-100, etc)
- **Business Rules**: Calculos correctos (porcentaje, backing promedio, etc)

Total: Aproximadamente 45+ assertions distribuidas en 18 requests.

## Notas Importantes

1. **Autenticacion**: El request "_Setup > Login" debe ejecutarse primero. Guarda automaticamente el token en `authToken`.

2. **CampaniaId**: Se obtiene automaticamente del primer request de "Mis Campanias". Si necesitas usar una campania especifica, edita manualmente la variable.

3. **Usuarios de Test** (Docker):
   - usuario1@mail.com / 123456 (Fan + Artista)
   - api-test@mail.com / 123456 (Fan + Artista)

4. **Variables de Entorno**: Para cambiar baseUrl o credenciales, usa la interfaz de Postman o crea un archivo environment.json:

```json
{
  "id": "dev-env",
  "name": "Development",
  "values": [
    {
      "key": "baseUrl",
      "value": "http://localhost:5001",
      "enabled": true
    },
    {
      "key": "testEmail",
      "value": "usuario1@mail.com",
      "enabled": true
    },
    {
      "key": "testPassword",
      "value": "123456",
      "enabled": true
    }
  ]
}
```

## Casos de Uso

### Validar que endpoints estan funcionando
```bash
newman run postman-collection.json --reporters cli
```

### Generar reporte detallado
```bash
newman run postman-collection.json \
    --reporters html,json \
    --reporter-html-export test-report.html \
    --reporter-json-export test-results.json
```

### CI/CD (GitHub Actions)
```yaml
- name: Run API Tests
  run: |
    newman run ./plans/dashboard-artista/backend/postman-collection.json \
      --environment ./tests/environments/dev.json \
      --reporters cli,json \
      --reporter-json-export results.json \
      --bail on
```

## Estructura de Response Esperada

### GET /api/dashboard/resumen (200 OK)
```json
{
  "totalRecaudado": 3500.00,
  "totalBackers": 45,
  "campaniasActivas": 2,
  "campaniasCompletadas": 1,
  "moneda": "EUR"
}
```

### GET /api/campanias/mis-campanias (200 OK)
```json
{
  "items": [
    {
      "id": "guid",
      "titulo": "Primer Album de Los Rockeros",
      "imagenUrl": "https://...",
      "estado": "EnCurso",
      "estadoNombre": "En Curso",
      "importeObjetivo": 5000.00,
      "importeRecaudado": 1250.00,
      "porcentaje": 25,
      "numBackers": 15,
      "diasRestantes": 45,
      "fechaFin": "2026-03-21",
      "fechaCreacion": "2026-01-15"
    }
  ],
  "totalCount": 3
}
```

### GET /api/campanias/{id}/backings (200 OK)
```json
{
  "items": [
    {
      "id": "guid",
      "nombreBacker": "Juan Garcia",
      "emailBacker": "juan@...",
      "rewardNombre": "CD Firmado",
      "monto": 25.00,
      "mensaje": "Exitos!",
      "esAnonimo": false,
      "fechaCreacion": "2026-01-21T10:30:00Z",
      "fechaRelativa": "hace 2 horas"
    }
  ],
  "totalCount": 15,
  "totalRecaudado": 1250.00,
  "stats": {
    "backingPromedio": 83.33,
    "rewardMasPopular": "CD Firmado",
    "ultimoBacking": "2026-01-21T10:30:00Z"
  }
}
```

### GET /api/campanias/{id}/stats (200 OK)
```json
{
  "importeObjetivo": 5000.00,
  "importeRecaudado": 1250.00,
  "porcentaje": 25,
  "numBackers": 15,
  "backingPromedio": 83.33,
  "diasRestantes": 45,
  "diasTranscurridos": 6,
  "proyeccionFinal": 3750.00,
  "rewardStats": [
    {
      "rewardNombre": "CD Firmado",
      "cantidad": 10,
      "total": 350.00,
      "porcentaje": 28
    }
  ],
  "progressoPorDia": [
    { "fecha": "2026-01-15", "total": 150.00 }
  ]
}
```

## Troubleshooting

### Error: "Could not find token in response"
- Verificar que POST /api/auth/login retorna un campo `accessToken`
- Revisar credenciales en variables

### Error: 401 Unauthorized
- El token puede haber expirado
- Ejecutar nuevamente "_Setup > Login" para obtener nuevo token

### Error: 404 Not Found en Backings/Stats
- La campania no existe
- Ejecutar primero "Mis Campanias" para obtener un `campaniaId` valido
- La variable `campaniaId` se guarda automaticamente

### Timeouts o lentitud
- Verificar que backend esta corriendo en puerto 5001
- Revisar conexion de red
- Aumentar timeout en Newman: `--timeout 10000`

## Metricas

- **Folders**: 5 (_Setup, Dashboard, Mis Campanias, Backings, Stats)
- **Requests**: 18 (3 setup, 2 resumen, 3 mis-campanias, 5 backings, 5 stats)
- **Assertions**: 45+ (status, types, structure, business rules)
- **Tiempo esperado**: < 5 segundos (ambiente local)

## Referencias

- [Especificacion Tecnica](/docs/product/US-05-dashboard-artista.md)
- [Newman Documentation](https://learning.postman.com/docs/running-collections/using-newman-cli/)
- [Postman Collection Schema](https://schema.getpostman.com/json/collection/v2.1.0/collection.json)

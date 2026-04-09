# Estado de Infraestructura: CI/CD, Testing y Secretos

**Fecha**: 2026-01-26
**Proyecto**: WePlay Rises
**Proposito**: Auditoria del estado actual de CI/CD, tests y gestion de secretos

---

## Resumen Ejecutivo

| Area | Estado | Prioridad |
|------|--------|-----------|
| CI/CD | No implementado | Baja (post-MVP) |
| Tests Unitarios | No implementado | Media |
| Tests Integracion | No implementado | Media |
| Tests E2E | No implementado | Baja |
| Connection Strings | Configurado (LocalDB) | OK para dev |
| Azure Key Vault | No implementado | Alta para prod |

**Contexto**: Proyecto MVP con 30 horas disponibles. Prioridad definida: Flujo E2E funcional > Tests basicos > Deploy Azure.

---

## 1. CI/CD

### 1.1 Estado Actual

| Sistema | Archivo | Estado |
|---------|---------|--------|
| GitHub Actions | `.github/workflows/*.yml` | No existe |
| Azure DevOps | `azure-pipelines.yml` | No existe |
| GitLab CI | `.gitlab-ci.yml` | No existe |
| Jenkins | `Jenkinsfile` | No existe |
| Docker | `Dockerfile`, `docker-compose.yml` | No existe |

### 1.2 Arquitectura Planificada

Segun `README.md` (lineas 39-40):

- **Backend**: Azure App Service
- **Frontend Landing**: Azure Static Web Apps
- **Frontend Admin**: Azure Static Web Apps
- **CI/CD**: Azure DevOps Pipelines
- **Base de Datos**: Azure SQL (produccion)

**Estado**: "Pendiente de deploy"

### 1.3 Scripts de Build Disponibles

**Frontend Landing** (`src/web/package.json`):
```json
{
  "scripts": {
    "dev": "vite",
    "build": "tsc -b && vite build",
    "lint": "eslint ."
  }
}
```

**Frontend Admin** (`src/admin/package.json`):
```json
{
  "scripts": {
    "dev": "next dev -p 3001",
    "build": "next build",
    "start": "next start -p 3001",
    "lint": "next lint"
  }
}
```

**Backend** (manual):
```bash
cd src/api
dotnet restore
dotnet build WePlayRises.sln
dotnet run --project WebApi/WebApi.csproj
```

### 1.4 Acciones Requeridas para CI/CD

1. Crear `azure-pipelines.yml` con stages: Build, Test, Deploy
2. Crear `Dockerfile` para backend .NET 8
3. Crear `docker-compose.yml` para ambiente local
4. Configurar Azure App Service y Static Web Apps
5. Configurar variables de entorno en Azure DevOps

---

## 2. Tests Unitarios

### 2.1 Estado Actual

| Componente | Estado | Ubicacion Esperada |
|------------|--------|-------------------|
| Proyectos .NET (*.Tests.csproj) | No existe | `tests/` |
| Archivos test backend (*Tests.cs) | No existe | `tests/Unit/` |
| Archivos test frontend (*.test.ts) | No existe | `src/*/__tests__/` |
| Configuracion Vitest | No existe | `vitest.config.ts` |
| Configuracion xUnit | No existe | `xunit.runner.json` |

### 2.2 Dependencias Faltantes

**Backend (.NET)**:
- xUnit
- xUnit.runner.visualstudio
- Moq
- FluentAssertions
- Microsoft.NET.Test.Sdk

**Frontend (TypeScript)**:
- vitest
- @testing-library/react
- @testing-library/jest-dom
- @testing-library/user-event
- msw (Mock Service Worker)

### 2.3 Documentacion Existente

**Archivo**: `.claude/rules/testing/unit-tests.rule.md`

**Objetivos definidos**:
- Cobertura minima: 80%
- Tiempo ejecucion: < 60 segundos

**Patron Backend (xUnit)**:
```csharp
[Fact]
public async Task MetodoAProbar_Escenario_ResultadoEsperado()
{
    // Arrange
    // Act
    // Assert
}
```

**Patron Frontend (Vitest)**:
```tsx
describe('ComponentName', () => {
  it('should render correctly', () => {
    render(<Component />);
    expect(screen.getByText('text')).toBeInTheDocument();
  });
});
```

### 2.4 Estructura de Directorios Planificada

```
tests/
├── WePlayRises.Api.Tests/           # Tests de integracion API
│   └── WePlayRises.Api.Tests.csproj
├── WePlayRises.Crowdfunding.Tests/  # Tests unitarios modulo
│   └── WePlayRises.Crowdfunding.Tests.csproj
└── WePlayRises.UserAccess.Tests/    # Tests unitarios modulo
    └── WePlayRises.UserAccess.Tests.csproj

src/web/src/features/{feature}/__tests__/
├── {Component}.test.tsx
├── use{Hook}.test.ts
└── {service}.service.test.ts
```

---

## 3. Pruebas de Integracion

### 3.1 Estado Actual

| Componente | Estado |
|------------|--------|
| Colecciones Postman | No existe |
| Newman runner | No configurado |
| Tests HTTP backend | No existe |
| Fixtures de datos | No existe |

### 3.2 Infraestructura Disponible

**Scripts SQL para datos de prueba**:
- `docs/database/00_WePlayRises_Identity_AND_CrowdFundinf.sql` (59 KB)
- `docs/database/01_WePlayRises_Crowdsourcing.sql` (15 KB)
- `docs/database/02_WePlayRises_Crowdpromotion.sql` (14 KB)
- `docs/database/03_WePlayRises_Poblacion_Maestras.sql` (27 KB)

**API disponible para testing**:
- URL: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`

### 3.3 Documentacion SDLC

**Archivo**: `docs/analisis/20260122_sdlc-tooling-orquestado.md`

Contiene estrategia completa de testing (lineas 245-337):
- Piramide de tests (Unit > Integration > E2E)
- Patrones de test backend con xUnit y Moq
- Patrones de test frontend con Vitest
- Scripts PowerShell de orquestacion (planificados)

---

## 4. Pruebas E2E

### 4.1 Estado Actual

| Componente | Estado |
|------------|--------|
| Playwright | No configurado |
| Cypress | No configurado |
| Directorio e2e/ | No existe |
| Page Objects | No existe |

### 4.2 Estructura Planificada

```
e2e/
├── tests/
│   ├── auth/
│   │   ├── login.spec.ts
│   │   └── register.spec.ts
│   └── campanias/
│       ├── crear-campania.spec.ts
│       └── ver-campania.spec.ts
├── fixtures/
│   └── test-data.json
├── pages/
│   ├── LoginPage.ts
│   └── CampaniaPage.ts
└── playwright.config.ts
```

---

## 5. Connection Strings y Base de Datos

### 5.1 Configuracion Actual

**Archivo**: `src/api/WebApi/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=WePlayRises;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

| Ambiente | Base de Datos | Estado |
|----------|---------------|--------|
| Desarrollo | SQL Server LocalDB | Configurado |
| Produccion | Azure SQL | No configurado |

### 5.2 Uso en Codigo

**Archivo**: `src/api/WebApi/Program.cs` (lineas 56-57)

```csharp
builder.Services.AddDbContext<UserAccessContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### 5.3 Migraciones EF Core

```bash
# Crear migracion
dotnet ef migrations add NombreMigracion --project WebApi -c WePlayRisesDbContext

# Aplicar migraciones
dotnet ef database update --project WebApi
```

---

## 6. Gestion de Secretos

### 6.1 Estado Actual de Azure Key Vault

| Componente | Estado |
|------------|--------|
| Azure.Identity | No instalado |
| Azure.Security.KeyVault.Secrets | No instalado |
| Configuracion Key Vault | No existe |
| Managed Identity | No configurado |

**Conclusion**: Azure Key Vault NO esta implementado.

### 6.2 Configuracion JWT Actual

**Archivo**: `src/api/WebApi/appsettings.json`
```json
{
  "Jwt": {
    "Key": "REPLACE_WITH_SECURE_KEY_IN_PRODUCTION",
    "Issuer": "WePlayRises",
    "Audience": "WePlayRisesUsers",
    "ExpirationMinutes": 60
  }
}
```

**Archivo**: `src/api/WebApi/appsettings.Development.json`
```json
{
  "Jwt": {
    "Key": "WePlayRises_DevKey_SuperSecret_MinLength32Chars!",
    "Issuer": "WePlayRises",
    "Audience": "WePlayRisesUsers",
    "ExpirationMinutes": 120
  }
}
```

**Problema**: JWT Key hardcodeada en archivo versionado.

### 6.3 Uso en Codigo

**Archivo**: `src/api/WebApi/Program.cs` (lineas 72-93)

```csharp
var jwtKey = builder.Configuration["Jwt:Key"] ?? "WePlayRisesDefaultSecretKey123456789";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "WePlayRises";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "WePlayRisesUsers";
```

### 6.4 Variables de Entorno Frontend

**Landing** (`src/web/.env.example`):
```
VITE_API_URL=/api
```

**Admin** (`src/admin/.env.example`):
```
NEXT_PUBLIC_API_URL=https://localhost:7001/api
```

### 6.5 Gitignore para Secretos

**Archivo**: `.gitignore`
```
.env
.env.local
appsettings.Development.json
.credentials.local.json
```

**Problema detectado**: `appsettings.Development.json` esta listado en .gitignore pero actualmente existe en el repositorio.

### 6.6 Recomendaciones para Produccion

1. **Implementar Azure Key Vault**:
   ```bash
   dotnet add package Azure.Identity
   dotnet add package Azure.Security.KeyVault.Secrets
   ```

2. **Configurar en Program.cs**:
   ```csharp
   if (builder.Environment.IsProduction())
   {
       var keyVaultUri = new Uri($"https://{vaultName}.vault.azure.net/");
       builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
   }
   ```

3. **Usar User Secrets para desarrollo**:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "mi-clave-secreta-local"
   ```

4. **Configurar Managed Identity en Azure App Service**

5. **Remover `appsettings.Development.json` del repositorio**

---

## 7. Matriz de Riesgos

| Riesgo | Impacto | Probabilidad | Mitigacion |
|--------|---------|--------------|------------|
| JWT Key expuesta en repo | Alto | Media | Implementar Key Vault |
| Sin tests antes de deploy | Alto | Alta | Crear tests minimos |
| Sin CI/CD automatizado | Medio | Alta | Pipeline basico |
| Connection string hardcoded | Medio | Baja | Variables de entorno |

---

## 8. Plan de Accion Sugerido

### Fase 1: Seguridad Inmediata (Pre-produccion)
- [ ] Remover `appsettings.Development.json` del repo
- [ ] Configurar User Secrets para desarrollo local
- [ ] Rotar JWT Key

### Fase 2: Testing Minimo (MVP)
- [ ] Crear proyecto `WePlayRises.Api.Tests`
- [ ] Tests unitarios para servicios criticos
- [ ] Configurar Vitest en frontend

### Fase 3: CI/CD Basico (Post-MVP)
- [ ] Crear `azure-pipelines.yml`
- [ ] Configurar Azure App Service
- [ ] Implementar Azure Key Vault

### Fase 4: Testing Completo (Futuro)
- [ ] Colecciones Postman/Newman
- [ ] Tests E2E con Playwright
- [ ] Cobertura 80%+

---

## Referencias

- `CLAUDE.md` - Instrucciones del proyecto
- `.claude/rules/testing/unit-tests.rule.md` - Reglas de testing
- `docs/analisis/20260122_sdlc-tooling-orquestado.md` - Estrategia SDLC completa
- `README.md` - Arquitectura planificada

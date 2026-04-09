# Plan Final Sprint: Horas Restantes (13h)

- **Fecha**: 2026-02-14
- **Autor**: Ivan + Claude Code
- **Contexto**: MVP WePlay Rises, curso LIDR (AI4Devs)
- **Presupuesto total**: 30 horas | **Consumido**: 17h (57%) | **Restante**: 13h

---

## Resumen Ejecutivo

El MVP de CrowdFunding esta **100% funcional** (5 User Stories completadas, tests unitarios, tests de integracion Newman, Docker Compose funcionando). Quedan 13 horas para maximizar el impacto del proyecto antes de la entrega del curso.

### Distribucion propuesta

| Bloque | Horas | Prioridad | Justificacion |
|--------|-------|-----------|---------------|
| WPR-022: Deploy Azure | 3h | Obligatoria | Requisito de entrega LIDR |
| Polish visual CrowdFunding | 2h | Alta | Impacto directo en demo/video |
| CampaniaCrowdfundingUpdate | 1.5h | Alta | Feature CrowdFunding pendiente con alto impacto demo |
| CrowdSourcing MVP | 4.5h | Media-Alta | Demuestra vision completa plataforma |
| WPR-023: Docs + Video demo | 2h | Obligatoria | Requisito de entrega LIDR |
| **TOTAL** | **13h** | | |

> **Nota:** Se eliminó el buffer de 1h para incorporar CampaniaCrowdfundingUpdate (1.5h) y se redujo CrowdSourcing de 5h a 4.5h. El riesgo es aceptable dado que el Domain + DbContext de CrowdSourcing ya están implementados.

---

## 1. WPR-022: Deploy Azure (3h)

### Objetivo

Desplegar el stack completo en Azure para que sea accesible via URL publica. El profesor y evaluadores deben poder navegar la plataforma sin instalar nada.

### Arquitectura de Deploy

```
                    ┌─────────────────────┐
                    │   Azure DNS / URL   │
                    └──────────┬──────────┘
                               │
              ┌────────────────┼────────────────┐
              │                │                │
    ┌─────────▼──────┐ ┌──────▼────────┐ ┌──────▼───────┐
    │  Static Web App│ │ Static Web App│ │  App Service │
    │  (Landing)     │ │ (Admin)       │ │  (API .NET)  │
    │  React/Vite    │ │ Next.js SSG   │ │  Linux B1    │
    │  Puerto 3000   │ │ Puerto 3001   │ │  Puerto 8080 │
    └────────────────┘ └───────────────┘ └──────┬───────┘
                                                │
                                         ┌──────▼───────┐
                                         │  Azure SQL   │
                                         │  Basic (5DTU)│
                                         │  WePlayRises │
                                         └──────────────┘
```

### Paso a paso detallado

#### 1.1 Azure SQL Database (30 min)

**Recursos a crear:**
- Resource Group: `rg-weplay-rises`
- SQL Server: `sql-weplay-rises` (region: West Europe)
- Database: `weplay-rises-db` (tier: Basic 5 DTU, ~5 EUR/mes)

**Comandos Azure CLI:**
```bash
# Login
az login

# Resource Group
az group create --name rg-weplay-rises --location westeurope

# SQL Server
az sql server create \
  --name sql-weplay-rises \
  --resource-group rg-weplay-rises \
  --location westeurope \
  --admin-user weplayadmin \
  --admin-password "<GenerarPasswordSeguro>"

# Firewall: permitir Azure services
az sql server firewall-rule create \
  --resource-group rg-weplay-rises \
  --server sql-weplay-rises \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Firewall: permitir IP local (para migraciones)
az sql server firewall-rule create \
  --resource-group rg-weplay-rises \
  --server sql-weplay-rises \
  --name AllowLocalDev \
  --start-ip-address <MI_IP> \
  --end-ip-address <MI_IP>

# Database
az sql db create \
  --resource-group rg-weplay-rises \
  --server sql-weplay-rises \
  --name weplay-rises-db \
  --service-objective Basic \
  --backup-storage-redundancy Local
```

**Ejecutar migraciones contra Azure SQL:**
```bash
cd src/api

# Connection string Azure SQL
export CONN="Server=sql-weplay-rises.database.windows.net;Database=weplay-rises-db;User Id=weplayadmin;Password=<Password>;Encrypt=True;TrustServerCertificate=False;"

# Aplicar migraciones de los 3 contextos
dotnet ef database update --project WebApi -c UserAccessContext -- --ConnectionStrings:DefaultConnection "$CONN"
dotnet ef database update --project WebApi -c CrowdfundingContext -- --ConnectionStrings:DefaultConnection "$CONN"
dotnet ef database update --project WebApi -c CoreContext -- --ConnectionStrings:DefaultConnection "$CONN"
```

**Seed de datos maestros:**
- Los seeds ya estan en el `OnModelCreating` (MaestraEstadoCampania, MaestraTipoFinanciacion, MaestraMoneda)
- Verificar que los roles (Fan, Artista, Admin) se crean en startup via `Program.cs`

**Datos de demo para la presentacion:**
- Crear 2-3 artistas con perfiles completos
- Crear 3-5 campanias en distintos estados (borrador, publicada, completada)
- Crear rewards variados para cada campania
- Crear 10-15 backings para mostrar actividad

#### 1.2 Azure App Service - API (45 min)

**Recurso a crear:**
- App Service Plan: `asp-weplay-rises` (tier: B1 Linux, ~13 EUR/mes)
- Web App: `app-weplay-rises-api`

**Comandos:**
```bash
# App Service Plan (Linux, B1)
az appservice plan create \
  --name asp-weplay-rises \
  --resource-group rg-weplay-rises \
  --sku B1 \
  --is-linux

# Web App (.NET 8)
az webapp create \
  --name app-weplay-rises-api \
  --resource-group rg-weplay-rises \
  --plan asp-weplay-rises \
  --runtime "DOTNETCORE:8.0"

# Configurar variables de entorno
az webapp config appsettings set \
  --name app-weplay-rises-api \
  --resource-group rg-weplay-rises \
  --settings \
    ConnectionStrings__DefaultConnection="Server=sql-weplay-rises.database.windows.net;Database=weplay-rises-db;User Id=weplayadmin;Password=<Password>;Encrypt=True;" \
    Jwt__Key="<GenerarClaveJWT_MinLength32_Produccion>" \
    Jwt__Issuer="WePlayRises" \
    Jwt__Audience="WePlayRisesUsers" \
    ASPNETCORE_ENVIRONMENT="Production" \
    ApplyMigrations="true"

# CORS para los frontends
az webapp cors add \
  --name app-weplay-rises-api \
  --resource-group rg-weplay-rises \
  --allowed-origins \
    "https://weplay-rises-landing.azurestaticapps.net" \
    "https://weplay-rises-admin.azurestaticapps.net"
```

**Deploy via ZIP (mas rapido que CI/CD para MVP):**
```bash
cd src/api
dotnet publish WebApi/WePlayRises.WebApi.csproj -c Release -o ./publish
cd publish && zip -r ../deploy.zip .
az webapp deploy \
  --name app-weplay-rises-api \
  --resource-group rg-weplay-rises \
  --src-path ../deploy.zip \
  --type zip
```

**Verificar:**
- `https://app-weplay-rises-api.azurewebsites.net/swagger` debe cargar
- `POST /api/auth/register` debe funcionar
- `GET /api/campanias` debe retornar datos

#### 1.3 Azure Static Web Apps - Landing (30 min)

**Recurso:**
- Static Web App: `swa-weplay-rises-landing` (tier: Free)

**Opcion A: Deploy manual (recomendada para MVP)**
```bash
cd src/web

# Configurar API URL para produccion
echo "VITE_API_URL=https://app-weplay-rises-api.azurewebsites.net/api" > .env.production

# Build
npm run build

# Deploy con SWA CLI
npx @azure/static-web-apps-cli deploy ./dist \
  --deployment-token <TOKEN_AZURE>
```

**Opcion B: GitHub Actions (si hay tiempo)**
```yaml
# .github/workflows/deploy-landing.yml
name: Deploy Landing
on:
  push:
    branches: [master]
    paths: ['src/web/**']
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: 20 }
      - run: cd src/web && npm ci && npm run build
        env:
          VITE_API_URL: ${{ secrets.API_URL }}
      - uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.SWA_TOKEN }}
          app_location: "src/web/dist"
          skip_app_build: true
```

**Configurar routing SPA:**
```json
// src/web/staticwebapp.config.json
{
  "navigationFallback": {
    "rewrite": "/index.html",
    "exclude": ["/assets/*", "/*.ico"]
  }
}
```

#### 1.4 Azure Static Web Apps - Admin (30 min)

**Recurso:**
- Static Web App: `swa-weplay-rises-admin` (tier: Free)

**Importante para Next.js en SWA:**
Next.js 14 con App Router necesita exportacion estatica para SWA (sin SSR):

```javascript
// next.config.js - agregar output: 'export'
const nextConfig = {
  output: 'export',
  distDir: 'out',
  env: {
    NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL
  }
}
```

```bash
cd src/admin

# Configurar API URL
echo "NEXT_PUBLIC_API_URL=https://app-weplay-rises-api.azurewebsites.net/api" > .env.production

# Build estatico
npm run build

# Deploy
npx @azure/static-web-apps-cli deploy ./out \
  --deployment-token <TOKEN_AZURE>
```

**Nota:** Si hay rutas dinamicas (`[id]`), asegurar que `generateStaticParams()` esta configurado o usar client-side routing.

#### 1.5 Verificacion E2E en Azure (30 min)

**Checklist de verificacion:**

- [ ] Landing carga en `https://swa-weplay-rises-landing.azurestaticapps.net`
- [ ] Admin carga en `https://swa-weplay-rises-admin.azurestaticapps.net`
- [ ] API Swagger accesible en `https://app-weplay-rises-api.azurewebsites.net/swagger`
- [ ] Registro de usuario funciona (Landing + Admin)
- [ ] Login funciona y retorna JWT valido
- [ ] Listar campanias publicas (sin auth) funciona
- [ ] Crear perfil artista (con auth) funciona
- [ ] Crear campania desde Admin funciona
- [ ] Hacer backing desde Landing funciona
- [ ] CORS correctamente configurado (no errores en consola)
- [ ] HTTPS funciona en todos los servicios

**Datos de demo sembrados:**
- [ ] 2+ artistas con perfil completo (imagen, bio, genero)
- [ ] 3+ campanias publicadas con imagenes reales (usar Unsplash music)
- [ ] Rewards variados (desde 5 EUR hasta 500 EUR)
- [ ] 10+ backings distribuidos entre campanias
- [ ] 1 campania al 80%+ de su meta (para mostrar progreso casi completo)
- [ ] 1 campania recien creada (para mostrar flujo desde cero)

### Costes estimados Azure (mes)

| Servicio | Tier | Coste/mes |
|----------|------|-----------|
| Azure SQL | Basic (5 DTU) | ~5 EUR |
| App Service | B1 Linux | ~13 EUR |
| Static Web App Landing | Free | 0 EUR |
| Static Web App Admin | Free | 0 EUR |
| **TOTAL** | | **~18 EUR** |

---

## 2. Polish Visual CrowdFunding (2h)

### Objetivo

Elevar la calidad visual del frontend para que la demo/video sea impactante. No se trata de reescribir componentes, sino de mejoras quirurgicas con alto ROI visual.

### Estado actual del frontend

**Landing (Vite + React):**
- HomePage: Hero basico (texto + 2 botones), stats hardcodeadas, grid de campanias
- CampaniaCard: Funcional pero basica (imagen/placeholder, badge, progress bar, stats)
- ExplorarPage: Grid de campanias sin filtros
- CampaniaDetailPage: Detalle con rewards, backings, tabs

**Admin (Next.js):**
- Dashboard: Cards metricas, lista campanias, backings recientes
- Wizard creacion campania: 4 pasos funcionales
- Gestion rewards/backings: CRUD completo

### 2.1 Landing Hero Mejorado (30 min)

**Actual**: Gradiente simple `from-background to-muted`, texto centrado, 2 botones.

**Objetivo**: Hero inmersivo con identidad musical.

**Mejoras concretas:**
- Gradiente mas vibrante con tematica musical: `from-[#0f0a2e] via-[#1a0a3e] to-[#0d0d2b]`
- Agregar patron decorativo SVG/CSS de notas musicales o ondas de audio (background-image con opacity baja)
- Texto principal con gradiente de color en palabras clave: "Impulsa la **musica** que amas" donde "musica" tiene gradiente pink-to-purple
- Agregar animacion sutil de fade-in al cargar (usando CSS `@keyframes` o Tailwind `animate-`)
- Botones: primario con gradiente `from-pink-500 to-purple-600` (coherente con progress bars), outline con hover que rellena
- Agregar un tercer CTA sutil: "Como funciona?" con scroll a seccion inferior
- Agregar partículas musicales o efecto de ondas sonoras como fondo (CSS puro con `radial-gradient` animado)

**Seccion Stats:**
- Reemplazar datos hardcodeados (100+, 5000+, 500K) por datos reales del API usando un endpoint `GET /api/stats/platform`
- Si no da tiempo el endpoint: al menos que los numeros tengan animacion de "count up" al entrar en viewport (usando `IntersectionObserver`)
- Agregar iconos mas expresivos o emojis musicales

**Seccion "Como funciona" (nueva, 15 min):**
```
1. Descubre -> 2. Apoya -> 3. Disfruta
   Icono        Icono       Icono
   Texto        Texto       Texto
```
3 columnas con iconos (lucide), titulo y descripcion corta. Seccion horizontal con fondo alternado.

### 2.2 CampaniaCard Mejorada (30 min)

**Actual**: Card dark con imagen, badge, progress bar, monto y backers count.

**Mejoras concretas:**
- **Imagenes placeholder mejoradas**: En vez de mostrar solo la inicial del titulo cuando no hay imagen, usar un gradiente aleatorio basado en el ID + icono musical (Music, Headphones, Mic, Guitar de lucide). Generar color del gradiente con hash del ID para consistencia.
- **Badge de estado animado**: Los estados "Publicada" y "Finalizada" con pulse sutil en el badge para campanas activas
- **Hover effect mejorado**: Ademas del border-purple, agregar sombra glow `shadow-lg shadow-purple-500/20` en hover
- **Barra de progreso animada**: Usar `transition-all duration-1000` con un delay incremental basado en el indice del card (efecto cascada al cargar la lista)
- **Mostrar artista**: Agregar linea con avatar miniatura + nombre del artista debajo del titulo (ya se tiene `artistaNombreArtistico` en el DTO)
- **"Dias restantes" mas visual**: Reemplazar texto plano por badge con color (verde > 30 dias, amarillo 7-30, rojo < 7, gris finalizada)
- **Skeleton mejorado**: El `CampaniaListSkeleton` actual funciona; agregar shimmer effect (gradient animado de izquierda a derecha)

### 2.3 Pagina de Detalle de Campania (30 min)

**Mejoras concretas:**
- **Hero de campania**: Imagen a ancho completo (16:9) con overlay gradiente oscuro y titulo superpuesto (estilo Kickstarter)
- **Sidebar sticky**: Info de backing (meta, progreso, botones) sticky en desktop al hacer scroll por la descripcion
- **Rewards mejorados**: Cada RewardPublicCard con efecto de "seleccionado" mas claro, borde animado al elegirlo
- **Pestana de actualizaciones**: Aunque este vacia, agregarla con un "Proximamente" para dar sensacion de plataforma completa
- **Compartir en redes**: Boton "Compartir" con icons de Twitter/X, WhatsApp, copiar link (no requiere backend, solo construir URLs con `window.location`)

### 2.4 Pagina Explorar con Filtros (20 min)

**Actual**: Solo grid de campanias sin filtros.

**Mejoras concretas:**
- Barra de busqueda en la parte superior (filtrado client-side por titulo)
- Filtros por estado: Tabs o pills "Todas | Activas | Completadas"
- Ordenar por: "Mas recientes | Mas financiadas | Cerca de la meta"
- Todo esto es filtrado/ordenacion client-side sobre los datos ya cargados (sin cambios en backend)

### 2.5 CampaniaCrowdfundingUpdate - Actualizaciones de Campaña (1.5h)

> **Referencia:** Auditoría completa en `20260214_auditoria-crowdfunding-domain.md`

#### Contexto

La entidad `CampaniaCrowdfundingUpdate` ya existe en el Domain Model con Fluent API configurado en `CrowdfundingContext`, pero no tiene implementación CQRS ni frontend. Las actualizaciones de progreso son una funcionalidad esencial en cualquier plataforma de crowdfunding (Kickstarter, Indiegogo, GoFundMe): los artistas publican posts sobre el avance de su proyecto y los fans los leen en la página de detalle.

#### Entidad Domain (ya existe)

```csharp
// Domain/Model/CampaniaCrowdfundingUpdate.cs
public class CampaniaCrowdfundingUpdate
{
    public Guid Id { get; set; }
    public CampaniaCrowdfundingId CampaniaId { get; set; }
    public string Titulo { get; set; } = null!;
    public string Contenido { get; set; } = null!;
    public bool EsPublico { get; set; }          // Visible para todos
    public bool SoloBackers { get; set; }         // Solo visible para quienes apoyaron
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public virtual CampaniaCrowdfunding Campania { get; set; } = null!;
}
```

#### 2.5.1 Backend CQRS (45 min)

**Archivos a crear:**

```
Modules/Crowdfunding/
  WePlayRises.Crowdfunding.Application/
    Dtos/
      CampaniaUpdateDto.cs                          # DTO para respuesta
    Features/CampaniaUpdates/
      Commands/
        CreateCampaniaUpdateCommand.cs              # Command + Handler
      Queries/
        GetUpdatesByCampaniaQuery.cs                # Query + Handler
      Validators/
        CreateCampaniaUpdateValidator.cs            # FluentValidation
    Mapping/
      CampaniaUpdateProfile.cs                      # AutoMapper
    Interfaces/Services/
      ICampaniaUpdateService.cs                     # Interface de servicio
  WePlayRises.Crowdfunding.Infra/
    Repositories/
      CampaniaUpdateRepository.cs                   # Repository EF Core
    Services/
      CampaniaUpdateService.cs                      # Service con RequestCache
  WePlayRises.Crowdfunding.WebApi/
    Controllers/
      CampaniaUpdatesController.cs                  # REST endpoints
```

**DTOs:**
```csharp
public class CampaniaUpdateDto
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public string Titulo { get; set; } = null!;
    public string Contenido { get; set; } = null!;
    public bool EsPublico { get; set; }
    public bool SoloBackers { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

**Endpoints API:**

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| `POST` | `/api/campanias/{campaniaId}/updates` | Authorize (artista dueño) | Crear actualización |
| `GET` | `/api/campanias/{campaniaId}/updates` | AllowAnonymous | Listar actualizaciones públicas |

**Command (Create):**
- Valida que la campaña existe y el usuario es el artista dueño
- Valida Titulo (requerido, max 200) y Contenido (requerido, max 5000)
- EsPublico default true, SoloBackers default false
- FechaCreacion = DateTime.UtcNow

**Query (GetByCampania):**
- Si el usuario es el artista dueño: retorna todas las actualizaciones
- Si el usuario es backer: retorna EsPublico=true y SoloBackers=true
- Si anónimo/fan sin backing: retorna solo EsPublico=true
- Ordenar por FechaCreacion descendente

**Validator:**
```csharp
RuleFor(x => x.Titulo)
    .NotEmpty().WithMessage("El título es obligatorio")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required)
    .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

RuleFor(x => x.Contenido)
    .NotEmpty().WithMessage("El contenido es obligatorio")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required)
    .MaximumLength(5000).WithMessage("El contenido no puede exceder 5000 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

RuleFor(x => x.CampaniaId)
    .NotEmpty().WithMessage("La campaña es obligatoria")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required);
```

**Registrar en DI:**
- Agregar `ICampaniaUpdateService` / `CampaniaUpdateService` en `DependencyInjection.cs`
- Agregar `ICampaniaUpdateRepository` / `CampaniaUpdateRepository`

#### 2.5.2 Frontend Landing (20 min)

**En CampaniaDetailPage (pestaña "Actualizaciones"):**

La página de detalle ya tiene sistema de tabs. Agregar tab "Actualizaciones":

```tsx
// features/campanias/presentation/components/CampaniaUpdates.tsx
interface CampaniaUpdatesProps {
  campaniaId: string;
}

export const CampaniaUpdates: FC<CampaniaUpdatesProps> = ({ campaniaId }) => {
  const { data: updates, isLoading } = useQuery({
    queryKey: ['campania-updates', campaniaId],
    queryFn: () => campaniaService.getUpdates(campaniaId),
  });

  if (isLoading) return <UpdatesSkeleton />;
  if (!updates?.length) return <EmptyUpdates />;

  return (
    <div className="space-y-6">
      {updates.map((update) => (
        <CampaniaUpdateCard key={update.id} update={update} />
      ))}
    </div>
  );
};
```

**CampaniaUpdateCard:**
```tsx
<Card>
  <CardHeader>
    <div className="flex items-center justify-between">
      <CardTitle className="text-lg">{update.titulo}</CardTitle>
      {update.soloBackers && (
        <Badge variant="secondary">Solo backers</Badge>
      )}
    </div>
    <p className="text-sm text-muted-foreground">
      {formatRelativeTime(update.fechaCreacion)}
    </p>
  </CardHeader>
  <CardContent>
    <p className="text-sm whitespace-pre-wrap">{update.contenido}</p>
  </CardContent>
</Card>
```

**Empty state:**
```tsx
<div className="text-center py-12 text-muted-foreground">
  <FileText className="w-12 h-12 mx-auto mb-4 opacity-50" />
  <p>No hay actualizaciones todavía</p>
  <p className="text-sm">El artista publicará novedades sobre esta campaña</p>
</div>
```

#### 2.5.3 Frontend Admin (15 min)

**En el dashboard del artista, dentro del detalle de campaña:**

Agregar sección "Publicar actualización" con formulario:

```tsx
// admin/src/components/campania/CampaniaUpdateForm.tsx
<form onSubmit={handleSubmit}>
  <div className="space-y-4">
    <Input
      label="Título"
      placeholder="Ej: ¡Acabamos de grabar la batería!"
      {...register('titulo')}
    />
    <Textarea
      label="Contenido"
      placeholder="Comparte las novedades de tu campaña..."
      rows={6}
      {...register('contenido')}
    />
    <div className="flex gap-4">
      <Checkbox label="Público" {...register('esPublico')} defaultChecked />
      <Checkbox label="Solo backers" {...register('soloBackers')} />
    </div>
    <Button type="submit">Publicar actualización</Button>
  </div>
</form>
```

**Lista de actualizaciones del artista:**
- Mostrar todas las actualizaciones de la campaña (incluidas privadas)
- Badge visual para "Solo backers" vs "Público"
- No es necesario editar/eliminar en MVP

#### 2.5.4 Shared Types (5 min)

```typescript
// src/shared/types/campania-update.ts
export interface CampaniaUpdateDto {
  id: string;
  campaniaId: string;
  titulo: string;
  contenido: string;
  esPublico: boolean;
  soloBackers: boolean;
  fechaCreacion: string;
}

export interface CreateCampaniaUpdateRequest {
  titulo: string;
  contenido: string;
  esPublico: boolean;
  soloBackers: boolean;
}
```

#### 2.5.5 Datos de Demo

Crear 2-3 actualizaciones para las campañas de demo:
- "¡Acabamos de entrar al estudio!" (público)
- "Preview exclusivo del primer single" (solo backers)
- "Confirmamos fecha de lanzamiento" (público)

Esto enriquece la demo y muestra que las campañas tienen actividad real.

#### Impacto en Demo

Con esta implementación, el flujo de demo se enriquece:
1. Fan ve campaña → pestaña "Actualizaciones" muestra posts del artista
2. Artista en Admin → publica actualización desde dashboard
3. Fan backer ve contenido exclusivo "Solo backers"
4. Evaluador percibe plataforma viva con comunicación artista-fan

---

### 2.6 Responsive y Micro-interacciones (10 min)

**Quick wins:**
- Verificar que el grid de campanias usa 1 columna en movil, 2 en tablet, 3 en desktop (ya parece estar con `grid-cols-1 md:grid-cols-2 lg:grid-cols-3`)
- Agregar `scroll-smooth` al HTML root
- Toast de confirmacion al hacer backing con animacion (ya deberia existir con sonner/toast)
- Loading button states en formularios (spinner dentro del boton mientras se envia)

---

## 3. CrowdSourcing MVP (5h)

### Vision

CrowdSourcing es el segundo pilar de WePlay Rises: permite a artistas **buscar colaboradores** (productores, diseñadores, videografos, etc.) y a profesionales **ofrecer sus servicios**.

El MVP no necesita implementar todo el flujo (Necesidad -> Propuesta -> Acuerdo -> Milestone -> Entregable -> Valoracion). Con implementar **2 User Stories** basicas es suficiente para demostrar que la plataforma va mas alla del crowdfunding.

### Modelo de datos existente (Domain)

Ya estan creadas 8 entidades con sus relaciones y DbContext configurado:

| Entidad | Campos clave | Relaciones |
|---------|-------------|------------|
| `NecesidadCrowdsourcing` | Titulo, Descripcion, TipoNecesidad, PresupuestoMin/Max, Ubicacion, FechaLimite | -> Propuestas, -> Acuerdos |
| `PropuestaCrowdsourcing` | MensajePropuesta, PrecioPropuesto, DiasEstimados, Estado | -> Necesidad |
| `AcuerdoCrowdsourcing` | ImporteTotalPactado, FechaInicio/Fin, Estado | -> Necesidad, -> Propuesta, -> Milestones |
| `AcuerdoCrowdsourcingMilestone` | Titulo, ImporteParcial, FechaLimite | -> Acuerdo |
| `AcuerdoCrowdsourcingEntregable` | Titulo, UrlRecurso, Estado | -> Acuerdo, -> Milestone |
| `ConversacionCrowdsourcing` | Asunto, UserIdArtista, UserIdProveedor | -> Mensajes |
| `MensajeCrowdsourcing` | Contenido, UrlAdjunto, FechaLeido | -> Conversacion |
| `ValoracionCrowdsourcing` | Puntuacion (1-5), Comentario | -> Acuerdo |

**Infraestructura ya lista:**
- `CrowdsourcingContext` con Fluent API completo
- StronglyTypedIds configurados (NecesidadCrowdsourcingId, PropuestaCrowdsourcingId, AcuerdoCrowdsourcingId)
- Indices y relaciones definidos
- Proyectos .csproj (Domain, Application, Infra, WebApi) creados y en .sln

### User Stories propuestas

#### US-CS01: Publicar Necesidad de Colaboracion (2.5h)

**Historia:** Como artista, quiero publicar una necesidad de servicio (produccion, diseno, video, etc.) para encontrar profesionales que me ayuden con mi proyecto.

**Backend CQRS (1.5h):**

```
Modules/Crowdsourcing/
  WePlayRises.Crowdsourcing.Domain/
    Constants/
      ServiceResponseMessageType.cs       # Constantes (codigos 1xxx, 2xxx, 4xxx)
      TipoNecesidad.cs                    # Enum: Produccion, Diseno, Video, Fotografia, Mastering, Otro
      EstadoNecesidad.cs                  # Enum: Borrador, Publicada, EnNegociacion, Cerrada, Cancelada
      ModalidadTrabajo.cs                 # Enum: Remoto, Presencial, Hibrido
  WePlayRises.Crowdsourcing.Application/
    Dtos/
      NecesidadDto.cs                     # DTO completo para detalle
      NecesidadListDto.cs                 # DTO resumido para listados
    Features/Necesidad/
      Commands/
        CreateNecesidadCommand.cs         # Command + Handler (POST)
        UpdateNecesidadCommand.cs         # Command + Handler (PUT)
        PublishNecesidadCommand.cs        # Command + Handler (POST /publicar)
      Queries/
        GetNecesidadByIdQuery.cs          # Query + Handler (GET /{id})
        GetAllNecesidadesQuery.cs         # Query + Handler (GET, publicas)
        GetMisNecesidadesQuery.cs         # Query + Handler (GET /mis-necesidades)
      Validators/
        CreateNecesidadValidator.cs       # FluentValidation
        UpdateNecesidadValidator.cs
    Mapping/
      NecesidadProfile.cs                # AutoMapper
    Interfaces/Services/
      INecesidadService.cs
  WePlayRises.Crowdsourcing.Infra/
    Repositories/
      NecesidadRepository.cs
    Services/
      NecesidadService.cs                # Con RequestCache
  WePlayRises.Crowdsourcing.WebApi/
    Controllers/
      NecesidadesController.cs           # REST endpoints
```

**Endpoints API:**

| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| `POST` | `/api/necesidades` | Artista | Crear necesidad (borrador) |
| `GET` | `/api/necesidades` | Publico | Listar necesidades publicadas |
| `GET` | `/api/necesidades/{id}` | Publico | Detalle de necesidad |
| `GET` | `/api/necesidades/mis-necesidades` | Artista | Mis necesidades |
| `PUT` | `/api/necesidades/{id}` | Artista | Editar necesidad |
| `POST` | `/api/necesidades/{id}/publicar` | Artista | Publicar necesidad |

**Frontend - Landing (0.5h):**

Pagina publica `src/web/src/features/crowdsourcing/`:
- `ExplorarNecesidadesPage.tsx`: Grid de necesidades publicadas (cards)
- `NecesidadDetailPage.tsx`: Detalle con info del artista, requisitos, presupuesto
- `NecesidadCard.tsx`: Card con titulo, tipo (badge), presupuesto, modalidad, dias restantes
- Ruta: `/necesidades` y `/necesidades/{id}`

**Frontend - Admin (0.5h):**

Dashboard artista `src/admin/src/app/(dashboard)/necesidades/`:
- `page.tsx`: Lista de mis necesidades (similar a mis-campanias)
- `nueva/page.tsx`: Formulario creacion (titulo, descripcion, tipo, presupuesto, modalidad, ubicacion, fecha limite)
- Componentes: `NecesidadStatusBadge`, `NecesidadListCard`

#### US-CS02: Enviar Propuesta a Necesidad (2.5h)

**Historia:** Como profesional/fan, quiero enviar una propuesta a una necesidad publicada para ofrecer mis servicios al artista.

**Backend CQRS (1.5h):**

```
Modules/Crowdsourcing/
  WePlayRises.Crowdsourcing.Application/
    Dtos/
      PropuestaDto.cs
      PropuestaListDto.cs
    Features/Propuesta/
      Commands/
        CreatePropuestaCommand.cs         # Command + Handler
        UpdatePropuestaCommand.cs         # Editar propuesta pendiente
      Queries/
        GetPropuestasByNecesidadQuery.cs  # Listar propuestas de una necesidad
        GetMisPropuestasQuery.cs          # Mis propuestas enviadas
      Validators/
        CreatePropuestaValidator.cs
    Mapping/
      PropuestaProfile.cs
    Interfaces/Services/
      IPropuestaService.cs
  WePlayRises.Crowdsourcing.Infra/
    Repositories/
      PropuestaRepository.cs
    Services/
      PropuestaService.cs
  WePlayRises.Crowdsourcing.WebApi/
    Controllers/
      PropuestasController.cs
```

**Endpoints API:**

| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| `POST` | `/api/necesidades/{id}/propuestas` | User | Enviar propuesta |
| `GET` | `/api/necesidades/{id}/propuestas` | Artista* | Ver propuestas recibidas |
| `GET` | `/api/propuestas/mis-propuestas` | User | Mis propuestas enviadas |
| `PUT` | `/api/propuestas/{id}` | User | Editar mi propuesta |

*Solo el artista dueno de la necesidad puede ver las propuestas.

**Frontend - Landing (0.5h):**

En la pagina de detalle de necesidad:
- Formulario de propuesta (mensaje, precio propuesto, dias estimados)
- Dialog/Modal para enviar propuesta
- Lista de "mis propuestas" en el perfil del usuario

**Frontend - Admin (0.5h):**

Dashboard artista:
- En detalle de necesidad: lista de propuestas recibidas
- Card de propuesta: nombre proveedor, mensaje, precio, dias estimados
- Botones de accion: Aceptar/Rechazar (placeholder - no implementar Acuerdo completo)

### Shared Types (pre-requisito, 15 min)

```typescript
// src/shared/types/crowdsourcing.ts
export interface NecesidadDto {
  id: string;
  artistaId: string;
  artistaNombreArtistico?: string;
  titulo: string;
  descripcion?: string;
  tipoNecesidadId: number;
  estadoNecesidadId: number;
  modalidadTrabajoId: number;
  presupuestoMin?: number;
  presupuestoMax?: number;
  monedaId?: number;
  ubicacionCiudad?: string;
  ubicacionPais?: string;
  fechaLimitePropuestas?: string;
  propuestasCount: number;
  fechaCreacion: string;
}

export interface PropuestaDto {
  id: string;
  necesidadId: string;
  userId: string;
  userEmail?: string;
  mensajePropuesta?: string;
  precioPropuesto: number;
  monedaId?: number;
  diasEstimados?: number;
  estadoPropuestaId: number;
  fechaCreacion: string;
}

// Constantes
export const TIPO_NECESIDAD = {
  1: 'Produccion Musical',
  2: 'Diseno Grafico',
  3: 'Videografia',
  4: 'Fotografia',
  5: 'Mastering',
  6: 'Mixing',
  7: 'Marketing',
  8: 'Otro'
} as const;

export const ESTADO_NECESIDAD = {
  1: 'Borrador',
  2: 'Publicada',
  3: 'En Negociacion',
  4: 'Cerrada',
  5: 'Cancelada'
} as const;

export const MODALIDAD_TRABAJO = {
  1: 'Remoto',
  2: 'Presencial',
  3: 'Hibrido'
} as const;
```

### Migraciones DB

Se necesita una migracion para crear las tablas de Crowdsourcing en la base de datos (las entidades y el Context ya existen, pero no se han generado migrations aun):

```bash
cd src/api
dotnet ef migrations add InitCrowdsourcing -c CrowdsourcingContext --project Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra
dotnet ef database update -c CrowdsourcingContext
```

**Importante:** Verificar si necesitan datos maestros (MaestraTipoNecesidad, MaestraEstadoNecesidad, MaestraModalidadTrabajo) en seeds.

### Integracion en la Landing

**Menu de navegacion actualizado:**
```
Logo | Explorar Campanias | Explorar Necesidades | [Login]
```

**Homepage: nueva seccion debajo de campanias destacadas:**
```
"Buscan Colaboradores"
Necesidades recientes (3 cards con tipo, titulo, presupuesto, artista)
[Ver todas las necesidades ->]
```

### Impacto en la Demo

Con estas 2 US implementadas, la demo puede mostrar:
1. Artista crea campania de crowdfunding (ya funciona)
2. Artista publica necesidad "Busco productor para mi album" (nuevo)
3. Profesional navega necesidades y envia propuesta (nuevo)
4. Artista ve propuestas recibidas en su dashboard (nuevo)

Esto demuestra que WePlay Rises no es solo crowdfunding, sino una **plataforma integral** para el ecosistema musical.

---

## 4. WPR-023: Documentacion Final + Video Demo (2h)

### 4.1 README.md Profesional (30 min)

Reescribir el README actual para que funcione como **carta de presentacion** del proyecto.

**Estructura propuesta:**

```markdown
# WePlay Rises 🎵

> Plataforma integral de crowdfunding y colaboracion musical.
> MVP desarrollado para el curso LIDR (AI4Devs).

## Demo en Vivo
- Landing: https://weplay-rises-landing.azurestaticapps.net
- Admin: https://weplay-rises-admin.azurestaticapps.net
- API: https://app-weplay-rises-api.azurewebsites.net/swagger

## Screenshots
[4-6 screenshots embebidos: Hero, Explorar campanias, Detalle campania, Dashboard artista, Crear campania wizard, Necesidades]

## Arquitectura
[Diagrama ASCII o imagen del stack]

## Stack Tecnologico
[Tabla con Backend, Frontend Landing, Frontend Admin, DB, Auth, Deploy]

## Modulos
| Modulo | Estado | Descripcion |
|--------|--------|-------------|
| CrowdFunding | ✅ Completo | Campanias, rewards, backings |
| CrowdSourcing | 🚧 MVP | Necesidades, propuestas |
| CrowdPromotion | 📋 Modelado | Domain entities listos |

## Como ejecutar localmente
### Prerequisitos
### Docker (recomendado)
### Desarrollo manual

## Estructura del proyecto
[Arbol de carpetas simplificado]

## API Endpoints
[Tabla con los endpoints principales por modulo]

## Tests
[Como ejecutar tests, cobertura]

## Proceso de desarrollo
[Breve mencion de AI-assisted development con Claude Code]
[Enlace a prompts.md]

## Creditos y Licencia
```

### 4.2 Swagger/OpenAPI (15 min)

**Configurar correctamente Swagger para produccion:**
- Agrupar endpoints por modulo/controller con `[ApiExplorerSettings(GroupName = "...")]`
- Agregar descripciones XML a los endpoints principales
- Incluir ejemplo de request/response en los DTOs con `[SwaggerSchema]`
- Configurar autenticacion JWT en Swagger UI (boton "Authorize")
- Verificar que el boton "Try it out" funciona contra la API de Azure

**XML Comments minimos a agregar:**
```csharp
/// <summary>
/// Listar campanias publicadas
/// </summary>
/// <returns>Lista de campanias activas</returns>
/// <response code="200">Lista de campanias</response>
[HttpGet]
public async Task<ActionResult> GetAll(...)
```

### 4.3 Capturas de Pantalla (15 min)

Capturar 6-8 screenshots de alta calidad para README y presentacion:

1. **Landing Hero** - Primera impresion de la plataforma
2. **Explorar Campanias** - Grid con 3-4 campanias con datos reales
3. **Detalle Campania** - Con imagen, progreso al 70%, rewards visibles
4. **Hacer Backing** - Modal de backing con reward seleccionado
5. **Dashboard Artista** - Metricas, campanias, backings recientes
6. **Wizard Campania** - Paso 1 (info basica) del formulario
7. **Explorar Necesidades** - Grid de necesidades (CrowdSourcing)
8. **Swagger API** - Documentacion de endpoints

**Herramientas:**
- Usar Playwright `page.screenshot()` para consistencia (ya esta configurado)
- Alternativamente: Extension de Chrome "Full Page Screen Capture"
- Resolucion: 1280x720 para screenshots de pagina completa

### 4.4 Video Demo (45 min)

**Herramienta recomendada:** OBS Studio (gratis) o Loom (rapido)

**Duracion objetivo:** 3-5 minutos

**Guion del video:**

```
00:00 - 00:20  INTRO
  "WePlay Rises es una plataforma integral para el ecosistema musical..."
  Mostrar landing hero

00:20 - 01:00  FLUJO FAN
  - Navegar campanias en la landing
  - Entrar a detalle de una campania
  - Ver rewards disponibles
  - Hacer un backing (seleccionar reward, confirmar)
  - Ver confirmacion

01:00 - 02:00  FLUJO ARTISTA - CROWDFUNDING
  - Login como artista en Admin
  - Ver dashboard con metricas
  - Crear nueva campania (wizard: info, historia, rewards, review)
  - Publicar campania
  - Ver que aparece en la landing

02:00 - 02:45  FLUJO ARTISTA - CROWDSOURCING
  - Crear necesidad "Busco productor musical"
  - Publicar necesidad
  - Ver que aparece en landing (seccion necesidades)
  - Mostrar propuestas recibidas (si hay)

02:45 - 03:30  ARQUITECTURA TECNICA
  - Mostrar brevemente Swagger (API documentada)
  - Mencionar: .NET 8 + CQRS, React 18, Next.js 14
  - Mostrar Docker Compose corriendo
  - Mencionar tests (unitarios + Newman)

03:30 - 04:00  CIERRE
  - Resumen de modulos (Funding completo, Sourcing MVP, Promotion modelado)
  - "Desarrollado con asistencia de IA (Claude Code)"
  - URLs de demo en pantalla
```

**Tips para el video:**
- Tener datos de demo pre-cargados (no crear desde cero en el video)
- Usar zoom en areas importantes (DevTools cerradas)
- Velocidad: no demasiado rapido, dar tiempo al espectador
- Grabar la pantalla sin webcam para simplificar
- Musica de fondo sutil (libre de derechos)

### 4.5 prompts.md (15 min)

Documentar los prompts y comandos mas relevantes usados con Claude Code:

```markdown
# Prompts y Comandos IA Usados

## Comandos Slash Personalizados
| Comando | Descripcion | Uso |
|---------|-------------|-----|
| /us-to-spec | User Story -> Specs | Genero feature-spec, contracts, ui-ux |
| /plan | Specs -> Planes detallados | Backend (CQRS) + Frontend (Landing + Admin) |
| /implement | Planes -> Codigo + Tests | Scaffolding completo de features |

## Ejemplos de Prompts Efectivos
[3-5 ejemplos reales de prompts que generaron buen resultado]

## Flujo de Trabajo AI-Assisted
[Diagrama: Idea -> /us-to-spec -> /plan -> /implement -> /done]
```

---

## 5. Ideas "Wow" de Alto Impacto / Bajo Esfuerzo

Estas ideas son **opcionales** y se implementan solo si sobra tiempo dentro del buffer o si alguna fase principal termina antes de lo estimado. Estan ordenadas por ratio impacto/esfuerzo.

### 5.1 Simulacion de Pago con UI Stripe-like (1.5h) - ALTO IMPACTO

**Que es:** Un modal de "pago" que simula la experiencia Stripe sin procesamiento real. El backing ya se registra en DB; esto solo mejora la UX del paso final.

**Por que impresiona:** Hace que la plataforma parezca lista para produccion. Ningun evaluador espera pago real en un MVP, pero la UI de pago eleva la percepcion de profesionalismo.

**Implementacion:**
```
BackingFlow actual: Seleccionar Reward -> Confirmar monto -> POST /backings -> Confirmacion
BackingFlow mejorado: Seleccionar Reward -> Confirmar monto -> [NUEVO: Formulario "pago"] -> POST /backings -> Confirmacion animada
```

**Componentes:**
- `PaymentSimulationModal.tsx`: Card de pago con campos visuales (no funcionales)
  - Numero de tarjeta: `4242 4242 4242 4242` (pre-llenado como "test")
  - Fecha: `12/28`
  - CVC: `***`
  - Boton "Pagar XX EUR" con loading spinner de 1.5 segundos (simula procesamiento)
  - Mensaje "Pago simulado - Entorno de desarrollo" visible
  - Iconos de Visa/Mastercard/Amex (SVGs de lucide o custom)
- `PaymentConfirmation.tsx`: Animacion de check verde, confetti sutil (CSS), resumen del backing

**Detalle visual:**
```tsx
// Estructura del modal de pago
<Dialog>
  <div className="bg-white rounded-lg p-6 max-w-md">
    <div className="flex items-center gap-2 mb-4">
      <CreditCard className="w-5 h-5" />
      <span className="font-semibold">Confirmar pago</span>
    </div>

    {/* Resumen del backing */}
    <div className="bg-gray-50 rounded-md p-4 mb-6">
      <p className="font-medium">{reward.nombre}</p>
      <p className="text-2xl font-bold">{monto} EUR</p>
      <p className="text-sm text-gray-500">Campana: {campania.titulo}</p>
    </div>

    {/* Formulario visual (decorativo) */}
    <div className="space-y-4">
      <Input value="4242 4242 4242 4242" disabled label="Numero de tarjeta" />
      <div className="grid grid-cols-2 gap-4">
        <Input value="12/28" disabled label="Fecha" />
        <Input value="***" disabled label="CVC" />
      </div>
    </div>

    {/* Aviso sandbox */}
    <div className="mt-4 text-xs text-amber-600 bg-amber-50 p-2 rounded">
      Entorno sandbox - No se realizara ningun cargo real
    </div>

    {/* Boton de pago */}
    <Button className="w-full mt-4 bg-gradient-to-r from-pink-500 to-purple-600">
      {isProcessing ? <Spinner /> : `Pagar ${monto} EUR`}
    </Button>
  </div>
</Dialog>
```

### 5.2 Busqueda y Filtros en Campanias (1h) - ALTO IMPACTO

**Que es:** Barra de busqueda + filtros en la pagina de explorar campanias. Todo client-side, sin cambios en backend.

**Por que impresiona:** Transforma una lista plana en una experiencia de descubrimiento real.

**Implementacion:**
```tsx
// ExplorarPage.tsx mejorada
<div className="space-y-6">
  {/* Barra de busqueda */}
  <div className="relative">
    <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
    <Input
      placeholder="Buscar campanias..."
      value={search}
      onChange={(e) => setSearch(e.target.value)}
      className="pl-10"
    />
  </div>

  {/* Filtros */}
  <div className="flex gap-2 flex-wrap">
    <Button variant={filter === 'all' ? 'default' : 'outline'} onClick={() => setFilter('all')}>
      Todas
    </Button>
    <Button variant={filter === 'active' ? 'default' : 'outline'} onClick={() => setFilter('active')}>
      Activas
    </Button>
    <Button variant={filter === 'completed' ? 'default' : 'outline'} onClick={() => setFilter('completed')}>
      Completadas
    </Button>
  </div>

  {/* Ordenacion */}
  <Select value={sort} onValueChange={setSort}>
    <SelectTrigger className="w-48">
      <SelectValue placeholder="Ordenar por" />
    </SelectTrigger>
    <SelectContent>
      <SelectItem value="recent">Mas recientes</SelectItem>
      <SelectItem value="funded">Mas financiadas</SelectItem>
      <SelectItem value="nearGoal">Cerca de la meta</SelectItem>
      <SelectItem value="endingSoon">Finalizan pronto</SelectItem>
    </SelectContent>
  </Select>

  {/* Grid filtrado */}
  <CampaniaList campanias={filteredCampanias} isLoading={isLoading} />

  {/* Empty state si no hay resultados */}
  {filteredCampanias.length === 0 && !isLoading && (
    <EmptyState
      icon={<SearchX />}
      title="Sin resultados"
      description="Intenta con otros terminos de busqueda"
    />
  )}
</div>
```

### 5.3 Pagina "Como Funciona" (30 min) - MEDIO IMPACTO

**Que es:** Seccion en la landing que explica los 3 pilares de la plataforma con graficos.

**Por que impresiona:** Da contexto inmediato al evaluador. Demuestra vision de producto.

**Implementacion:**

Seccion en HomePage (o pagina dedicada `/como-funciona`):

```
┌────────────────────────────────────────────────────────────────┐
│                    Como funciona WePlay Rises                  │
│                                                                │
│   ┌──────────┐       ┌──────────┐       ┌──────────┐           │
│   │    💰   │       │    🤝    │       │    📢    │          │  
│   │  FUNDING │ ───►  │ SOURCING │ ───►  │ PROMOTION│           │
│   │          │       │          │       │          │           │
│   │ Financia │       │ Colabora │       │ Promueve │           │
│   │ el album │       │ con pros │       │ al mundo │           │
│   └──────────┘       └──────────┘       └──────────┘           │
│                                                                │
│   Para Artistas:                                               │
│   1. Crea tu campania de crowdfunding                          │
│   2. Encuentra colaboradores (productores, designers...)       │
│   3. Lanza tu programa de promocion con influencers            │
│                                                                │
│   Para Fans:                                                   │
│   1. Apoya proyectos musicales que te inspiran                 │
│   2. Obtiene recompensas exclusivas                            │
│   3. Participa como promotor y gana comisiones                 │
└────────────────────────────────────────────────────────────────┘
```

**Componente:**
```tsx
const pillars = [
  {
    icon: <Banknote className="w-12 h-12" />,
    title: "CrowdFunding",
    subtitle: "Financia la musica",
    description: "Los fans apoyan campanias de artistas para financiar albums, giras y videoclips. Elige rewards exclusivos.",
    status: "Disponible",
    color: "from-pink-500 to-purple-600"
  },
  {
    icon: <Handshake className="w-12 h-12" />,
    title: "CrowdSourcing",
    subtitle: "Colabora con talento",
    description: "Artistas publican necesidades (produccion, diseno, video) y profesionales envian propuestas.",
    status: "Nuevo",
    color: "from-blue-500 to-cyan-600"
  },
  {
    icon: <Megaphone className="w-12 h-12" />,
    title: "CrowdPromotion",
    subtitle: "Amplifica el alcance",
    description: "Programas de promocion donde fans e influencers difunden campanias a cambio de comisiones.",
    status: "Proximamente",
    color: "from-amber-500 to-orange-600"
  }
];
```

### 5.4 Compartir en Redes Sociales (20 min) - MEDIO IMPACTO

**Que es:** Botones para compartir una campania en Twitter/X, WhatsApp, y copiar enlace.

**Por que impresiona:** Funcionalidad esperada en cualquier plataforma de crowdfunding.

**Implementacion (sin backend):**
```tsx
function ShareButtons({ campania }: { campania: CampaniaDto }) {
  const url = window.location.href;
  const text = `Apoya "${campania.titulo}" en WePlay Rises!`;

  const shareLinks = {
    twitter: `https://twitter.com/intent/tweet?text=${encodeURIComponent(text)}&url=${encodeURIComponent(url)}`,
    whatsapp: `https://wa.me/?text=${encodeURIComponent(text + ' ' + url)}`,
    linkedin: `https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(url)}`,
  };

  const copyToClipboard = async () => {
    await navigator.clipboard.writeText(url);
    toast.success("Enlace copiado!");
  };

  return (
    <div className="flex gap-2">
      <Button variant="outline" size="icon" asChild>
        <a href={shareLinks.twitter} target="_blank"><Twitter /></a>
      </Button>
      <Button variant="outline" size="icon" asChild>
        <a href={shareLinks.whatsapp} target="_blank"><MessageCircle /></a>
      </Button>
      <Button variant="outline" size="icon" onClick={copyToClipboard}>
        <Link2 />
      </Button>
    </div>
  );
}
```

### 5.5 Notificaciones In-App (45 min) - MEDIO IMPACTO

**Que es:** Sistema basico de notificaciones en el header (icono campana + badge con count + dropdown con items).

**Por que impresiona:** Da sensacion de plataforma viva con actividad real.

**Implementacion (client-side, sin backend real):**

Las "notificaciones" se generan a partir de datos existentes:
- "Nuevo backing de {user} en tu campania {titulo}" (de backings recientes)
- "Tu campania {titulo} alcanzo el {X}% de su meta" (calcular de stats)
- "Nueva propuesta recibida para {necesidad}" (si CrowdSourcing esta listo)

```tsx
// hooks/useNotifications.ts
export function useNotifications() {
  const { data: backings } = useRecentBackings();

  const notifications = useMemo(() => {
    return (backings ?? []).slice(0, 5).map(b => ({
      id: b.id,
      type: 'backing' as const,
      message: `Nuevo backing de ${b.userEmail} - ${formatCurrency(b.monto)}`,
      timestamp: b.fechaCreacion,
      read: false,
    }));
  }, [backings]);

  return { notifications, unreadCount: notifications.filter(n => !n.read).length };
}
```

```tsx
// components/NotificationBell.tsx
<Popover>
  <PopoverTrigger>
    <div className="relative">
      <Bell className="w-5 h-5" />
      {unreadCount > 0 && (
        <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full w-4 h-4 flex items-center justify-center">
          {unreadCount}
        </span>
      )}
    </div>
  </PopoverTrigger>
  <PopoverContent className="w-80">
    <div className="space-y-2">
      {notifications.map(n => (
        <div key={n.id} className="flex gap-3 p-2 rounded hover:bg-muted">
          <BellRing className="w-4 h-4 mt-1 text-primary shrink-0" />
          <div>
            <p className="text-sm">{n.message}</p>
            <p className="text-xs text-muted-foreground">{formatRelativeTime(n.timestamp)}</p>
          </div>
        </div>
      ))}
    </div>
  </PopoverContent>
</Popover>
```

### 5.6 Endpoint de Stats de Plataforma (20 min) - BAJO IMPACTO

**Que es:** `GET /api/stats/platform` que retorna totales reales para el hero de la landing.

**Por que impresiona:** Reemplaza los numeros hardcodeados por datos reales en tiempo real.

**Implementacion:**
```csharp
// GET /api/stats/platform (publico, sin auth)
public class GetPlatformStatsQuery : IRequest<ServiceResponse<PlatformStatsDto>> { }

public class PlatformStatsDto
{
    public int TotalCampanias { get; set; }
    public int TotalBackers { get; set; }
    public decimal TotalRecaudado { get; set; }
    public int TotalArtistas { get; set; }
}
```

**En el frontend:**
```tsx
const { data: stats } = useQuery({
  queryKey: ['platform-stats'],
  queryFn: () => api.get('/stats/platform'),
  staleTime: 60000, // Cachear 1 minuto
});

// En el hero:
<div className="text-3xl font-bold">{stats?.totalCampanias ?? '...'}</div>
```

### 5.7 Dark/Light Mode Toggle (20 min) - BAJO IMPACTO FUNCIONAL, ALTO VISUAL

**Que es:** Toggle para cambiar entre tema oscuro (actual) y claro.

**Nota:** El diseño actual ya es dark-first. Si se implementa, usar las CSS variables de shadcn/ui que ya soportan temas via clase `dark` en `<html>`.

**Implementacion:**
- Ya disponible en shadcn/ui themes system
- Solo agregar toggle button en header
- Persistir preferencia en localStorage

### Matriz Resumen Ideas "Wow"

| # | Idea | Esfuerzo | Impacto Visual | Impacto Demo | Recomendacion |
|---|------|----------|---------------|-------------|---------------|
| 5.1 | Simulacion Pago | 1.5h | Muy Alto | Muy Alto | Implementar si hay tiempo |
| 5.2 | Busqueda/Filtros | 1h | Alto | Alto | Implementar si hay tiempo |
| 5.3 | Como Funciona | 30min | Alto | Muy Alto | Implementar (bajo esfuerzo) |
| 5.4 | Compartir Redes | 20min | Medio | Medio | Implementar (trivial) |
| 5.5 | Notificaciones | 45min | Alto | Alto | Implementar si hay tiempo |
| 5.6 | Stats Reales | 20min | Medio | Medio | Implementar (bajo esfuerzo) |
| 5.7 | Dark/Light Mode | 20min | Medio | Bajo | Posponer |

**Recomendacion de priorizacion si hay buffer:**
1. 5.3 Como Funciona (30min) - Mejor ratio impacto/esfuerzo
2. 5.4 Compartir Redes (20min) - Trivial y esperado
3. 5.2 Busqueda/Filtros (1h) - Transforma la experiencia
4. 5.1 Simulacion Pago (1.5h) - Impresionante para demo

---

## 6. Cronograma Sugerido

### Sesion 1: Deploy Azure (3h)
```
[00:00 - 00:30] Azure SQL: Crear server, DB, firewall, migraciones
[00:30 - 01:15] App Service: API .NET 8, variables, deploy ZIP
[01:15 - 01:45] Static Web App: Landing (build + deploy)
[01:45 - 02:15] Static Web App: Admin (build + deploy)
[02:15 - 02:45] Verificacion E2E en Azure
[02:45 - 03:00] Cargar datos de demo
```

### Sesion 2: Polish Visual + CampaniaUpdates (3.5h)
```
[00:00 - 00:30] Hero landing mejorado + seccion "Como funciona"
[00:30 - 01:00] CampaniaCard mejorada + skeleton shimmer
[01:00 - 01:30] Detalle campania (hero + sidebar sticky) + compartir redes
[01:30 - 02:00] Explorar con filtros client-side
[02:00 - 02:45] CampaniaUpdates Backend: CQRS (Command, Query, Validator, Service, Repository, Controller)
[02:45 - 03:05] CampaniaUpdates Frontend Landing: Tab "Actualizaciones" + CampaniaUpdateCard
[03:05 - 03:20] CampaniaUpdates Frontend Admin: Formulario + lista de updates
[03:20 - 03:30] CampaniaUpdates: Shared types + datos de demo
```

### Sesion 3: CrowdSourcing MVP (4.5h)
```
[00:00 - 00:15] Shared types + migraciones DB
[00:15 - 01:15] Backend US-CS01: Necesidades (CQRS completo)
[01:15 - 01:45] Frontend US-CS01: Landing + Admin (necesidades)
[01:45 - 02:45] Backend US-CS02: Propuestas (CQRS completo)
[02:45 - 03:15] Frontend US-CS02: Landing + Admin (propuestas)
[03:15 - 04:00] Integracion: navegacion, homepage, seeds
[04:00 - 04:30] Tests basicos + verificacion E2E
```

### Sesion 4: Docs + Video (2h)
```
[00:00 - 00:30] README.md profesional + screenshots
[00:30 - 00:45] Swagger anotaciones
[00:45 - 01:00] prompts.md
[01:00 - 01:45] Grabar video demo
[01:45 - 02:00] Revision final, re-deploy si hay cambios
```

---

## 7. Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|-------------|---------|------------|
| Next.js SSG no funciona con rutas dinamicas en SWA | Media | Alto | Usar client-side routing o cambiar a output `standalone` con App Service |
| Migraciones de CrowdSourcing fallan por FKs cruzadas | Baja | Medio | Las entidades no tienen FKs a UserAccess (usan string UserId), deberia funcionar |
| CORS mal configurado en Azure | Alta | Medio | Testear inmediatamente despues de deploy, es un fix de 2 minutos |
| 4.5h insuficientes para CrowdSourcing completo | Media | Medio | Priorizar US-CS01 (necesidades). Si no da tiempo US-CS02, mostrar solo la parte de publicar necesidades |
| Sin buffer de imprevistos | Media | Bajo | Se eliminó el buffer de 1h para incluir CampaniaUpdates. Si hay retrasos, recortar ideas "Wow" opcionales |
| Video demo tarda mas de lo esperado | Media | Bajo | Tener guion pre-escrito, datos pre-cargados, ensayar 1 vez antes de grabar |

---

## 8. Definition of Done (Entrega LIDR)

### Obligatorio
- [ ] Plataforma accesible via URLs publicas Azure
- [ ] README con screenshots y URLs
- [ ] Video demo de 3-5 minutos
- [ ] Swagger documentado y accesible
- [ ] Codigo en GitHub (repositorio publico o compartido con evaluadores)

### Deseable
- [ ] CampaniaCrowdfundingUpdate implementado (CQRS + frontend Landing + Admin)
- [ ] CrowdSourcing MVP funcionando (al menos necesidades)
- [ ] UI pulida con filtros y micro-interacciones
- [ ] Tests ejecutables (unitarios + Newman)
- [ ] prompts.md documentado

### Nice-to-have
- [ ] Simulacion de pago
- [ ] Notificaciones in-app
- [ ] Seccion "Como funciona"
- [ ] Dark/Light mode

---

*Documento generado el 2026-02-14 como plan de sprint final para WePlay Rises.*

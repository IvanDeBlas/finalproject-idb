# Azure Deploy Guide - WePlay Rises

## Arquitectura

```
                    Internet
                       |
        +--------------+--------------+
        |              |              |
   SWA Free       App Service B1  App Service B1
   (Landing)      (API .NET 8)   (Admin Next.js)
   Vite SPA       ZIP Deploy     ZIP Deploy
        |              |              |
        +--------------+--------------+
                       |
                 Azure SQL Basic
                   (5 DTU)
```

**Costo estimado: ~18 EUR/mes** (SQL 5 EUR + App Service B1 13 EUR + SWA Free 0)

---

## Pre-requisitos

```bash
# Login en Azure
az login

# Verificar suscripcion activa
az account show
```

---

## Paso 1: Crear recursos Azure (15 min)

### 1.1 Resource Group

```bash
az group create --name rg-weplay-rises --location westeurope
```

### 1.2 SQL Server + Database

```bash
# SQL Server
az sql server create \
  --name sql-weplay-rises \
  --resource-group rg-weplay-rises \
  --location westeurope \
  --admin-user weplayadmin \
  --admin-password "<PASSWORD_SEGURO>"

# Firewall: permitir Azure Services
az sql server firewall-rule create \
  --resource-group rg-weplay-rises \
  --server sql-weplay-rises \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Firewall: permitir IP local (para migraciones manuales)
# Obtener tu IP: curl ifconfig.me
az sql server firewall-rule create \
  --resource-group rg-weplay-rises \
  --server sql-weplay-rises \
  --name AllowLocalDev \
  --start-ip-address <MI_IP> \
  --end-ip-address <MI_IP>

# Database (Basic tier, ~5 EUR/mes)
az sql db create \
  --resource-group rg-weplay-rises \
  --server sql-weplay-rises \
  --name weplay-rises-db \
  --service-objective Basic \
  --backup-storage-redundancy Local
```

### 1.3 App Service Plan (Linux B1)

```bash
az appservice plan create \
  --name asp-weplay-rises \
  --resource-group rg-weplay-rises \
  --sku B1 \
  --is-linux
```

---

## Paso 2: Deploy API .NET (30 min)

### 2.1 Crear Web App

```bash
az webapp create \
  --name app-weplay-rises-api \
  --resource-group rg-weplay-rises \
  --plan asp-weplay-rises \
  --runtime "DOTNETCORE:8.0"
```

### 2.2 Configurar App Settings

```bash
az webapp config appsettings set \
  --name app-weplay-rises-api \
  --resource-group rg-weplay-rises \
  --settings \
    ConnectionStrings__DefaultConnection="Server=sql-weplay-rises.database.windows.net;Database=weplay-rises-db;User Id=weplayadmin;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=False;" \
    Jwt__Key="<GENERAR_JWT_KEY_PRODUCCION_MIN_32_CHARS>" \
    Jwt__Issuer="WePlayRises" \
    Jwt__Audience="WePlayRisesUsers" \
    ASPNETCORE_ENVIRONMENT="Production" \
    ApplyMigrations="true"
```

> **Nota**: Las URLs de CORS se configuran en el Paso 5 una vez conocidas las URLs de los frontends.

### 2.3 Build y Deploy

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

### 2.4 Verificar

- Abrir: `https://app-weplay-rises-api.azurewebsites.net/swagger`
- Las migraciones se aplican automaticamente en startup
- Los roles (Fan, Artista, Admin) se crean automaticamente

---

## Paso 3: Deploy Landing - SWA (20 min)

### 3.1 Crear Static Web App

```bash
az staticwebapp create \
  --name swa-weplay-rises-landing \
  --resource-group rg-weplay-rises \
  --location westeurope \
  --sku Free
```

### 3.2 Obtener deployment token

```bash
az staticwebapp secrets list \
  --name swa-weplay-rises-landing \
  --resource-group rg-weplay-rises
```

### 3.3 Build y Deploy

```bash
cd src/web
npm run build
npx @azure/static-web-apps-cli deploy ./dist \
  --deployment-token <TOKEN>
```

### 3.4 Verificar

- Abrir la URL proporcionada por SWA (ej: `https://swa-weplay-rises-landing.azurestaticapps.net`)
- Verificar que las rutas SPA funcionan (navegar a `/campanias`, recargar pagina)

---

## Paso 4: Deploy Admin - App Service Node.js (20 min)

### 4.1 Crear Web App (mismo plan B1, sin costo adicional)

```bash
az webapp create \
  --name app-weplay-rises-admin \
  --resource-group rg-weplay-rises \
  --plan asp-weplay-rises \
  --runtime "NODE:20-lts"
```

### 4.2 Configurar App Settings

```bash
az webapp config appsettings set \
  --name app-weplay-rises-admin \
  --resource-group rg-weplay-rises \
  --settings \
    INTERNAL_API_URL="https://app-weplay-rises-api.azurewebsites.net" \
    NEXT_PUBLIC_API_URL="/api" \
    NODE_ENV="production"
```

### 4.3 Configurar startup command

```bash
az webapp config set \
  --name app-weplay-rises-admin \
  --resource-group rg-weplay-rises \
  --startup-file "node_modules/.bin/next start -p 8080"
```

### 4.4 Build y Deploy

```bash
cd src/admin
npm run build

# Crear ZIP con archivos necesarios
zip -r deploy-admin.zip .next package.json next.config.mjs node_modules src/

az webapp deploy \
  --name app-weplay-rises-admin \
  --resource-group rg-weplay-rises \
  --src-path deploy-admin.zip \
  --type zip
```

### 4.5 Verificar

- Abrir: `https://app-weplay-rises-admin.azurewebsites.net`
- Verificar login y rutas dinamicas (`/campanias/[id]`)

---

## Paso 5: Actualizar CORS con URLs reales (5 min)

Una vez desplegados los frontends:

```bash
az webapp config appsettings set \
  --name app-weplay-rises-api \
  --resource-group rg-weplay-rises \
  --settings \
    Cors__AllowedOrigins__0="https://swa-weplay-rises-landing.azurestaticapps.net" \
    Cors__AllowedOrigins__1="https://app-weplay-rises-admin.azurewebsites.net" \
    Cors__AllowedOrigins__2="http://localhost:3000" \
    Cors__AllowedOrigins__3="http://localhost:3001"
```

> **Importante**: Reiniciar la API despues de actualizar CORS:
> ```bash
> az webapp restart --name app-weplay-rises-api --resource-group rg-weplay-rises
> ```

---

## Paso 6: Verificacion E2E (15 min)

### Checklist

- [ ] Landing carga en URL SWA
- [ ] Admin carga en URL App Service
- [ ] Swagger accesible en API URL `/swagger`
- [ ] Registro de usuario funciona (Landing)
- [ ] Login funciona y retorna JWT
- [ ] Listar campanias publicas (sin auth)
- [ ] Crear perfil artista (con auth)
- [ ] CORS sin errores en consola del browser
- [ ] HTTPS funciona en todos los servicios

---

## Paso 7: Cargar datos de demo (10 min)

Via Swagger (`/swagger`), crear:
- 2-3 artistas con perfiles completos
- 3-5 campanias en distintos estados
- Rewards variados para cada campania
- 10-15 backings

---

## URLs Finales

| Servicio | URL |
|----------|-----|
| API + Swagger | https://app-weplay-rises-api.azurewebsites.net/swagger |
| Landing | https://swa-weplay-rises-landing.azurestaticapps.net |
| Admin | https://app-weplay-rises-admin.azurewebsites.net |

---

## Troubleshooting

### Ver logs de la API
```bash
az webapp log tail --name app-weplay-rises-api --resource-group rg-weplay-rises
```

### Ver logs del Admin
```bash
az webapp log tail --name app-weplay-rises-admin --resource-group rg-weplay-rises
```

### Restart servicios
```bash
az webapp restart --name app-weplay-rises-api --resource-group rg-weplay-rises
az webapp restart --name app-weplay-rises-admin --resource-group rg-weplay-rises
```

### Eliminar todo (si se necesita recrear)
```bash
az group delete --name rg-weplay-rises --yes --no-wait
```

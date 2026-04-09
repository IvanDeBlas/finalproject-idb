# Analisis: Docker y Kubernetes para desarrollo local

- > **Fecha:** 2026-02-12
- > **Contexto:** MVP WePlay Rises, 30h disponibles, 1 desarrollador
- > **Conclusion:** Docker si (pospuesto), Kubernetes no

---

## Situacion actual

| App | Tecnologia | Puerto | Como se levanta hoy |
|-----|-----------|--------|---------------------|
| Backend API | .NET 8 | https://localhost:7001 | `dotnet run` |
| Landing | Vite + React 18 | http://localhost:3000 | `npm run dev` |
| Admin | Next.js 14 | http://localhost:3001 | `npm run dev` |
| Base de datos | SQL Server LocalDB | (localdb)\MSSQLLocalDB | Automatico con Windows |

Se levantan en 3 terminales separadas. Funciona sin problemas en el entorno actual.

---

## Docker Compose

### Ventajas

- Un solo `docker compose up` levanta todo el stack
- SQL Server real en contenedor (no LocalDB)
- Entorno reproducible para cualquier maquina
- Util si alguien mas evalua el proyecto (profesor LIDR)
- Elimina problemas de "en mi maquina funciona"

### Desventajas para este proyecto

- Tiempo de setup estimado: **3-4 horas** (Dockerfiles + compose + networking + volumes)
- Un solo desarrollador, el entorno local ya funciona
- Horas limitadas, MVP incompleto (WPR-011 a WPR-014 pendientes)
- Cada ajuste de config Docker es tiempo que no va al MVP
- Hot reload mas lento que desarrollo nativo

### Stack propuesto (si se implementa)

```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports: ["1433:1433"]

  api:
    build: ./src/api
    ports: ["7001:7001"]
    depends_on: [sqlserver]

  web:
    build: ./src/web
    ports: ["3000:3000"]
    depends_on: [api]

  admin:
    build: ./src/admin
    ports: ["3001:3001"]
    depends_on: [api]
```

### Veredicto

**Util pero no prioritario.** Implementar solo si sobra tiempo despues de completar las historias de usuario MVP.

---

## Kubernetes local

### Opciones evaluadas

| Herramienta | Complejidad | RAM minima |
|-------------|------------|------------|
| Minikube | Media | 4 GB |
| Kind | Media | 2 GB |
| Docker Desktop K8s | Baja | 4 GB |
| k3d | Media | 1 GB |

### Por que NO tiene sentido

| Factor | WePlay Rises | Donde K8s aporta valor |
|--------|-------------|------------------------|
| Escala | 1 instancia por app | Decenas/cientos de pods |
| Equipo | 1 desarrollador | Equipos con microservicios |
| Arquitectura | Monolito modular | Microservicios distribuidos |
| Servicios | 3 procesos + 1 DB | Service mesh, ingress, RBAC, secrets |
| Tiempo setup | ~0 (ya funciona) | 6-8 horas para manifests y config |
| Mantenimiento | Nulo | Continuo (updates, monitoring) |

### Complejidad innecesaria que agregaria

- Manifests YAML (Deployment, Service, Ingress) por cada app
- ConfigMaps y Secrets para variables de entorno
- Persistent Volumes para SQL Server
- Ingress controller para routing
- Debugging mas complejo (logs distribuidos, port-forwarding)

### Veredicto

**No recomendado.** La complejidad supera ampliamente el beneficio para un MVP con 1 desarrollador y 3 servicios.

---

## Recomendacion final

### Ahora (prioridad)

Mantener el setup actual con 3 terminales. Terminar el MVP:
- WPR-011: Crear Campania
- WPR-012: Definir Recompensas
- WPR-013: Hacer Backing
- WPR-014: Dashboard Artista

### Si sobra tiempo

Crear un `docker-compose.yml` basico para demo. Valor: levantar todo con un comando para la presentacion del curso LIDR.

### Para produccion

Usar el plan existente de Azure (WPR-022):
- Azure App Service para API
- Azure Static Web Apps para frontends
- Azure SQL Database

Esto es mas pragmatico y alineado con el stack del proyecto que cualquier solucion basada en K8s.

---

## Decision

| Opcion | Decision | Cuando |
|--------|----------|--------|
| Docker Compose | **Si, pospuesto** | Despues de completar Fase 2 del MVP |
| Kubernetes local | **No** | No aplica para este proyecto |
| Azure (WPR-022) | **Si, planificado** | Fase 3 del backlog |

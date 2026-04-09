# Resumen de Uso de IA en WePlay Rises

- **Proyecto**: WePlay Rises - Plataforma de crowdfunding musical
- **Herramienta**: Claude Code (CLI de Anthropic)
- **Modelo**: Claude Opus 4.6 (1M context)
- **Periodo**: Enero - Marzo 2026

---

## Sistema de Desarrollo con IA

Para este proyecto se desarrollo un **sistema agentico personalizado** sobre Claude Code que automatiza y estandariza el flujo de desarrollo. Este sistema no se incluye en el entregable por considerarse propiedad intelectual del autor.

### Componentes del Sistema (descripcion general)

| Componente | Funcion |
|------------|---------|
| **Comandos** | Acciones invocables que ejecutan tareas complejas de forma atomica (scaffolding, planificacion, implementacion) |
| **Agentes especializados** | Subprocesos con contexto especifico para dominios concretos (backend .NET, frontend React, contratos API, testing, UI/UX) |
| **Rules contextuales** | Reglas que se cargan automaticamente segun el tipo de archivo editado, asegurando consistencia en patrones y convenciones |
| **Templates** | Plantillas de codigo que los agentes utilizan para generar artefactos siguiendo la arquitectura del proyecto |
| **Memorias** | Preferencias persistentes del proyecto que informan las decisiones de generacion |
| **MCP Servers** | Integraciones con Playwright (validacion visual), Figma (referencia de diseno), Context7 (documentacion actualizada) |

### Flujo de Trabajo Agentico

```
Usuario invoca comando --> Sistema selecciona agente(s) -->
Agentes consultan rules y templates --> Generacion coordinada -->
Validacion contra convenciones --> Verificacion visual (Playwright MCP) --> Output al usuario
```

Este sistema permite que una instruccion de alto nivel como *"implementa el modulo de campanias"* se descomponga automaticamente en tareas de dominio, infraestructura, aplicacion y presentacion, ejecutadas por agentes especializados en paralelo o secuencia segun dependencias.

---

## Prompts Representativos por Categoria

Los siguientes ejemplos muestran interacciones tipicas. En la practica, muchas de estas se abstraen mediante el sistema de comandos, pero se presentan en forma de prompt para ilustrar el tipo de instrucciones procesadas.

### 1. Vision y Concepto Inicial

**Contexto**: Definir el alcance del MVP y la diferenciacion del producto.

> "Quiero que revises los archivos del modelo de datos y las instrucciones del proyecto LIDR. Tengo tres entregas y un proyecto en React, API y base de datos que realizar. Se trata de una aplicacion de crowdsourcing y crowdfunding para grupos noveles. Quiero usar algo parecido al workspace y a CodePilot para crear en 30 horas el modelo de datos, una API en .NET y un frontal en React."

**Resultado**: Documento de debate inicial con arquitectura, modelo de datos simplificado (de 60 tablas a 5 entidades core para MVP), y plan de entregas por fases.

**Ajustes humanos**: Se simplifico el modelo inicial y se priorizo el flujo E2E de crowdfunding sobre features secundarias.

---

### 2. Diseno de Arquitectura

**Contexto**: Definir la estructura modular del backend siguiendo CQRS.

> "Necesito estructurar el modulo Crowdfunding con Commands y Queries separados. La entidad principal es Campania con campos: Titulo, Descripcion, ImporteObjetivo, FechaInicio, FechaFin, Estado. Debe seguir el patron donde Handler y Command van en el mismo archivo y siempre retornan ServiceResponse<T>."

**Aplicacion**: El sistema activo el agente de arquitectura backend, que consulto las rules de CQRS y genero la estructura completa de carpetas y archivos base.

---

### 3. Desarrollo de Features (Crowdfunding)

**Contexto**: Implementar endpoint de creacion de campania.

> "Implementa CreateCampaniaCommand con su Handler. Debe validar que el titulo no este vacio, que el importe objetivo sea mayor a 100, y que la fecha fin sea posterior a fecha inicio."

**Aplicacion**: El agente de CQRS genero Command, Handler, Validator y Profile de AutoMapper utilizando templates preconfigurados que garantizan adherencia a las convenciones (ServiceResponse, inyeccion de dependencias, logging estructurado).

---

### 4. Frontend con React

**Contexto**: Crear componentes para listar campanias.

> "Crea un componente CampaniaCard usando shadcn/ui Card. Debe mostrar titulo, descripcion truncada, barra de progreso del funding, y boton de ver mas."

**Aplicacion**: El agente frontend aplico las rules de React/TypeScript y genero el componente con props tipadas, usando los componentes de shadcn/ui y Tailwind segun las convenciones del proyecto.

---

### 5. Concepto Multi-Dimension (Crowdsourcing + Crowdpromotion)

**Contexto**: Evolucion del MVP para integrar las tres dimensiones en un mismo proyecto.

> "Cualquier proyecto puede ser crowdfunding, crowdsourcing y crowdpromotion. Un mismo proyecto artistico combina las tres dimensiones. Quiero que cada card de campania muestre badges de que dimensiones tiene activas."

**Aplicacion**: Se diseno el concepto de ProyectoArtistico como hub central que conecta las tres dimensiones. Los agentes generaron:
- Backend: CrowdFlagsService cross-module para consultar dimensiones activas
- Frontend: Badges visuales en cards, tabs contextuales en detalle de campania
- Shared: Tipos TypeScript compartidos entre Landing y Admin

**Ajustes humanos**: Decision estrategica de integrar todo en una interfaz unificada en lugar de tres secciones separadas.

---

### 6. Seed de Datos Completo

**Contexto**: Poblar la base de datos con datos realistas para demo y evaluacion.

> "Crea un script SQL completo para popularizar toda la base de datos. Quiero 8 artistas reales (Vetusta Morla, Bad Bunny, Rosalia...), 5 fans, campanias con rewards, backings, necesidades, propuestas, promotores. Datos creibles para una demo de 3 minutos."

**Aplicacion**: Claude genero un script SQL de ~120KB con datos coherentes entre si: artistas con campanias que tienen las dimensiones apropiadas, fans con perfiles profesionales y de promotor, backings con mensajes realistas, y relaciones de crowdsourcing y crowdpromotion completas.

---

### 7. Depuracion

**Contexto**: Error de validacion no se mostraba en frontend.

> "El formulario de crear campania no muestra los errores de validacion del backend. El endpoint retorna 200 con Messages pero el frontend no los procesa."

**Aplicacion**: Claude analizo el flujo completo (hook, servicio, componente) e identifico que faltaba extraer los mensajes del ServiceResponse. Propuso la correccion manteniendo el patron establecido.

---

### 8. Mejora y Refactoring

**Contexto**: Optimizar queries de base de datos.

> "El listado de campanias hace N+1 queries para cargar el artista. Optimiza la consulta."

**Aplicacion**: Consultando las rules de Entity Framework, Claude modifico el repository para usar eager loading y sugirio proyeccion directa al DTO para minimizar transferencia de datos.

---

### 9. Iteracion Visual con Playwright MCP

**Contexto**: Validar cambios de UI en tiempo real sin salir del terminal.

> "Ejecuta la app en localhost. Quiero navegar la landing y verificar que los badges de tipo se ven correctamente en las cards de campania."

> "Las pestanas no se ven bien. Deja solo el icono cuando no estan seleccionadas y muestra el texto solo en la pestana activa."

**Aplicacion**: Claude uso el MCP de Playwright para navegar la aplicacion, capturar screenshots, identificar problemas visuales y aplicar correcciones de CSS/componentes en el mismo ciclo. Esto permitio iteraciones de UI sin cambiar de contexto.

---

### 10. Testing

**Contexto**: Crear tests unitarios para los servicios.

> "Genera tests unitarios para CampaniaService cubriendo crear, obtener existente, y obtener inexistente."

**Aplicacion**: El agente de testing genero la clase siguiendo el patron AAA (Arrange-Act-Assert) con xUnit y Moq, respetando la estructura de archivos y nombrado definidos en las rules de testing del proyecto.

---

### 11. Docker y Deploy

**Contexto**: Dockerizar toda la plataforma para entrega reproducible.

> "Necesito que toda la plataforma funcione con un solo docker compose up. SQL Server, API, Landing y Admin."

**Aplicacion**: Claude genero el docker-compose.yml con los 4 servicios, Dockerfiles para cada proyecto, y la configuracion de health checks y dependencias entre servicios.

---

## Herramientas de IA Utilizadas

| Herramienta | Uso | Porcentaje estimado |
|-------------|-----|---------------------|
| **Claude Code (Opus 4.6)** | Desarrollo principal: arquitectura, codigo, testing, debugging | 90% |
| **MCP Playwright** | Validacion visual automatizada de la UI | 5% |
| **MCP Figma** | Referencia de diseno y componentes | 3% |
| **MCP Context7** | Consulta de documentacion actualizada de librerias | 2% |

---

## Beneficios del Sistema Agentico

- **Consistencia**: Todo el codigo generado sigue las mismas convenciones sin repetir instrucciones
- **Velocidad**: Tareas complejas (modulo completo con CQRS, tests, frontend) se ejecutan con comandos simples
- **Calidad**: Las rules actuan como guardrails que previenen desviaciones de arquitectura
- **Escalabilidad**: Nuevos patrones se incorporan una vez y aplican a todo el proyecto
- **Verificacion visual**: Playwright MCP permite validar UI sin salir del flujo de desarrollo

---

## Metricas del Proyecto

| Metrica | Valor |
|---------|-------|
| Horas totales de desarrollo | ~30h |
| Modulos backend | 4 (UserAccess, Crowdfunding, Crowdsourcing, Crowdpromotion) |
| Endpoints API | 40+ |
| Componentes React | 60+ |
| Tests unitarios backend | 50+ |
| Tests frontend (Vitest + Playwright) | 30+ |
| Entidades de dominio | 20+ |
| Script de seed | ~120KB SQL |

---

## Nota Final

El sistema de tooling representa una inversion significativa en configuracion y diseno que permite multiplicar la productividad del desarrollador. Se menciona su existencia para contextualizar el flujo de trabajo, pero los archivos de configuracion, comandos personalizados, templates y definiciones de agentes **no forman parte del entregable** por tratarse de metodologia propietaria aplicable a otros proyectos.

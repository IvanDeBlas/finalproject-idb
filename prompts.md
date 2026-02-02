# Resumen de Uso de IA en WePlay Rises

**Proyecto**: WePlay Rises - Plataforma de crowdfunding musical
**Herramienta**: Claude Code (CLI de Anthropic)
**Modelo**: Claude Opus 4.5
**Fecha**: 2026-02-02

---

## Sistema de Desarrollo con IA

Para este proyecto se desarrolló un **sistema agéntico personalizado** sobre Claude Code que automatiza y estandariza el flujo de desarrollo. Este sistema no se incluye en el entregable por considerarse propiedad intelectual del autor.

### Componentes del Sistema (descripción general)

| Componente | Función |
|------------|---------|
| **Comandos** | Acciones invocables que ejecutan tareas complejas de forma atómica (scaffolding, planificación, implementación) |
| **Agentes especializados** | Subprocesos con contexto específico para dominios concretos (backend .NET, frontend React, contratos API, testing) |
| **Rules contextuales** | Reglas que se cargan automáticamente según el tipo de archivo editado, asegurando consistencia en patrones y convenciones |
| **Templates** | Plantillas de código que los agentes utilizan para generar artefactos siguiendo la arquitectura del proyecto |
| **Memorias** | Preferencias persistentes del proyecto que informan las decisiones de generación |

### Flujo de Trabajo Agéntico

```
Usuario invoca comando → Sistema selecciona agente(s) →
Agentes consultan rules y templates → Generación coordinada →
Validación contra convenciones → Output al usuario
```

Este sistema permite que una instrucción de alto nivel como *"implementa el módulo de campañas"* se descomponga automáticamente en tareas de dominio, infraestructura, aplicación y presentación, ejecutadas por agentes especializados en paralelo o secuencia según dependencias.

---

## Prompts Representativos por Categoría

Los siguientes ejemplos muestran interacciones típicas. En la práctica, muchas de estas se abstraen mediante el sistema de comandos, pero se presentan en forma de prompt para ilustrar el tipo de instrucciones procesadas.

### 1. Diseño de Arquitectura

**Contexto**: Definir la estructura modular del backend siguiendo CQRS.

> "Necesito estructurar el módulo Crowdfunding con Commands y Queries separados. La entidad principal es Campania con campos: Titulo, Descripcion, ImporteObjetivo, FechaInicio, FechaFin, Estado. Debe seguir el patrón donde Handler y Command van en el mismo archivo y siempre retornan ServiceResponse<T>."

**Aplicación**: El sistema activó el agente de arquitectura backend, que consultó las rules de CQRS y generó la estructura completa de carpetas y archivos base.

---

### 2. Desarrollo de Features

**Contexto**: Implementar endpoint de creación de campaña.

> "Implementa CreateCampaniaCommand con su Handler. Debe validar que el título no esté vacío, que el importe objetivo sea mayor a 100, y que la fecha fin sea posterior a fecha inicio."

**Aplicación**: El agente de CQRS generó Command, Handler, Validator y Profile de AutoMapper utilizando templates preconfigurados que garantizan adherencia a las convenciones (ServiceResponse, inyección de dependencias, logging estructurado).

---

### 3. Frontend con React

**Contexto**: Crear componentes para listar campañas.

> "Crea un componente CampaniaCard usando shadcn/ui Card. Debe mostrar título, descripción truncada, barra de progreso del funding, y botón de ver más."

**Aplicación**: El agente frontend aplicó las rules de React/TypeScript y generó el componente con props tipadas, usando los componentes de shadcn/ui y Tailwind según las convenciones del proyecto.

---

### 4. Depuración

**Contexto**: Error de validación no se mostraba en frontend.

> "El formulario de crear campaña no muestra los errores de validación del backend. El endpoint retorna 200 con Messages pero el frontend no los procesa."

**Aplicación**: Claude analizó el flujo completo (hook, servicio, componente) e identificó que faltaba extraer los mensajes del ServiceResponse. Propuso la corrección manteniendo el patrón establecido.

---

### 5. Mejora y Refactoring

**Contexto**: Optimizar queries de base de datos.

> "El listado de campañas hace N+1 queries para cargar el artista. Optimiza la consulta."

**Aplicación**: Consultando las rules de Entity Framework, Claude modificó el repository para usar eager loading y sugirió proyección directa al DTO para minimizar transferencia de datos.

---

### 6. Testing

**Contexto**: Crear tests unitarios para el servicio.

> "Genera tests unitarios para CampaniaService cubriendo crear, obtener existente, y obtener inexistente."

**Aplicación**: El agente de testing generó la clase siguiendo el patrón AAA con la estructura de archivos y nombrado definidos en las rules de testing del proyecto.

---

## Beneficios del Sistema Agéntico

- **Consistencia**: Todo el código generado sigue las mismas convenciones sin repetir instrucciones
- **Velocidad**: Tareas complejas se ejecutan con comandos simples
- **Calidad**: Las rules actúan como guardrails que previenen desviaciones de arquitectura
- **Escalabilidad**: Nuevos patrones se incorporan una vez y aplican a todo el proyecto

---

## Nota Final

El sistema de tooling representa una inversión significativa en configuración y diseño que permite multiplicar la productividad del desarrollador. Se menciona su existencia para contextualizar el flujo de trabajo, pero los archivos de configuración, comandos personalizados, templates y definiciones de agentes **no forman parte del entregable** por tratarse de metodología propietaria aplicable a otros proyectos.

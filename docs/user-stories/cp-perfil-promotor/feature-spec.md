# Feature: Perfil de Promotor

> **ID:** cp-perfil-promotor
> **User Story:** US-CP-01
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** TBD

---

## Descripcion

Esta feature permite a un fan registrado crear y gestionar su perfil de promotor dentro del modulo Crowdpromotion. El promotor es el actor central de este modulo: un usuario que se compromete a difundir campanas de artistas a cambio de comisiones por referidos y backing generado. El flujo de registro recoge los datos publicos del promotor (nombre, tipo, redes sociales) y crea automaticamente una wallet en EUR para acumular sus ganancias.

El perfil de promotor es independiente del perfil de artista: un mismo usuario puede ser artista y promotor simultaneamente. Su creacion es el punto de entrada al modulo Crowdpromotion y habilitador critico para el resto de las funcionalidades del modulo, como la postulacion a programas de promocion (US-CP-02) y el seguimiento de comisiones (US-CP-04). Sin un perfil de promotor activo, el fan no puede participar en ningun programa de promocion.

La desactivacion del perfil es logica (EsActivo = false) y tiene efecto cascada sobre los programas activos en los que participa el promotor. Esto garantiza que los artistas no cuenten con promotores inactivos en sus programas, manteniendo la integridad operativa del modulo.

---

## User Story

**Como** fan registrado en la plataforma
**Quiero** crear mi perfil de promotor indicando mi nombre publico, redes sociales y tipo de promotor
**Para** poder postularme a programas de promocion de artistas y ganar comisiones difundiendo sus campanas

---

## Flujo Principal

```
1. Fan autenticado accede a /promotor/registro en la landing
2. Sistema comprueba si el usuario ya tiene perfil de promotor activo
3. Si ya existe → Redirigir a /promotor/dashboard (FA-01)
4. Si no existe → Mostrar formulario de registro de promotor
5. Fan completa los campos: nombre publico, tipo de promotor (obligatorios) y datos opcionales (email de contacto, URL sitio web, redes sociales)
6. Sistema valida los datos en frontend (schema Zod)
7. Fan envia el formulario
8. Backend valida los datos (FluentValidation) y comprueba unicidad de UserId
9. Backend crea el registro Promotor (EsActivo = true) vinculado al UserId
10. Backend crea automaticamente PromotorWallet en EUR asociada al nuevo promotor
11. Backend devuelve el perfil creado
12. Frontend muestra toast: "Perfil de promotor creado correctamente"
13. Sistema redirige al fan a /promotor/dashboard
```

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Fan ya tiene perfil de promotor (activo o inactivo) | Redirigir directamente a /promotor/dashboard sin mostrar formulario |
| FA-02 | Fan no tiene FanProfile vinculado | Crear perfil de promotor sin FanProfileId; no es error bloqueante |
| FA-03 | Ninguna red social indicada | Permitir el registro; mostrar aviso informativo recomendando agregar al menos una red para mejorar la visibilidad |
| FA-04 | URL de red social con formato invalido | Validacion Zod bloquea el envio del formulario y muestra el error en el campo correspondiente |
| FA-05 | Error de red al guardar | Mostrar toast de error, mantener los datos del formulario para reintentar |

---

## Flujo Secundario: Editar Perfil

```
1. Promotor autenticado accede a /promotor/perfil en la landing (o admin)
2. Sistema carga el perfil actual y pre-rellena el formulario
3. Promotor modifica los campos permitidos (nombre publico, email de contacto, URLs de redes)
4. El campo Tipo de Promotor se muestra como solo lectura (no editable)
5. Sistema valida los datos en frontend
6. Promotor envia el formulario
7. Backend actualiza el registro y registra FechaActualizacion
8. Frontend muestra toast: "Perfil actualizado correctamente"
```

## Flujo Secundario: Desactivar Perfil

```
1. Promotor accede a la seccion de configuracion de su perfil
2. Promotor hace clic en "Desactivar cuenta de promotor"
3. Si el promotor tiene programas activos → sistema muestra dialogo de confirmacion informando cuantos programas se veran afectados
4. Si no tiene programas activos → dialogo de confirmacion simple
5. Promotor confirma la desactivacion
6. Backend establece EsActivo = false y ejecuta baja automatica de todos los programas activos
7. Frontend muestra toast: "Perfil de promotor desactivado"
8. Sistema redirige al usuario a la pagina principal de la landing
```

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-CP01-1 | Un fan autenticado puede crear su perfil de promotor completando nombre publico y tipo de promotor; el registro se almacena en BD con los datos correctos | Backend |
| AC-CP01-2 | El sistema vincula automaticamente el UserId del usuario autenticado al perfil creado; si existe FanProfile para ese UserId se vincula tambien el FanProfileId | Backend |
| AC-CP01-3 | El perfil se crea con EsActivo = true | Backend |
| AC-CP01-4 | Se crea automaticamente una PromotorWallet en EUR al crear el perfil; la wallet queda asociada al PromotorId recien creado | Backend |
| AC-CP01-5 | No se puede crear un segundo perfil de promotor para el mismo UserId; el intento retorna un error de negocio descriptivo | Backend |
| AC-CP01-6 | Las URLs de redes sociales (Instagram, TikTok, YouTube, Twitter/X, sitio web) se validan con formato URL correcto tanto en frontend (Zod) como en backend (FluentValidation); una URL invalida impide el guardado | Backend + Landing + Shared |
| AC-CP01-7 | El formulario de registro carga la lista de tipos de promotor desde la tabla Maestra_TipoPromotor y la muestra en un selector | Backend + Landing |
| AC-CP01-8 | El promotor puede editar su perfil (nombre publico, email de contacto, URLs de redes); tras guardar, FechaActualizacion se actualiza en BD | Backend + Landing |
| AC-CP01-9 | El campo Tipo de Promotor no es editable una vez creado el perfil | Landing + Admin |
| AC-CP01-10 | Al desactivar el perfil, todos los programas de promocion activos del promotor quedan con estado de baja automatica | Backend |
| AC-CP01-11 | El formulario de registro y edicion del perfil usa schemas Zod definidos en shared y reutilizados en landing y admin | Shared + Landing + Admin |
| AC-CP01-12 | Un usuario sin sesion activa que accede a /promotor/registro es redirigido al login | Landing |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar endpoints POST, GET, PUT y PATCH del modulo Crowdpromotion para la entidad Promotor. Crear el registro Promotor con vinculacion a UserId y FanProfileId opcional. Crear automaticamente la PromotorWallet en EUR. Ejecutar baja de programas activos al desactivar. Exponer endpoint GET de tipos de promotor (maestras). Validar unicidad de UserId. Validar formatos de URL. | ALTO |
| **Landing** | Implementar pagina /promotor/registro con formulario de alta de promotor. Implementar pagina /promotor/perfil con formulario de edicion. Mostrar dialogo de confirmacion en la desactivacion. Gestionar redirecciones segun estado del perfil. Consumir endpoint de maestras para el selector de tipos. | ALTO |
| **Admin** | Implementar vista de perfil de promotor en el dashboard administrativo si el usuario tambien es promotor. Mostrar estado del perfil (activo/inactivo) y acceso rapido a la edicion. | MEDIO |
| **Shared** | Definir tipos TypeScript para Promotor, PromotorWallet, MaestraTipoPromotor, CreatePromotorDto y UpdatePromotorDto. Definir schemas Zod para los formularios de registro y edicion del perfil, incluyendo validacion de formato URL para todos los campos de redes sociales. | ALTO |

---

## Dependencias

### Tecnicas

- ASP.NET Core Identity activo y configurado con JWT: el UserId del token se usa como clave de vinculacion al crear el perfil
- Tabla Maestra_TipoPromotor con datos seed cargados antes de la primera ejecucion (ver datos seed en la US)
- Modulo Crowdpromotion creado con su DbContext, entidades y migraciones aplicadas

### De otras features

- Ninguna en el modulo Crowdpromotion. Esta feature es el punto de entrada y prerequisito para:
  - US-CP-02 (Explorar y Postularse a Programas): requiere perfil de promotor activo
  - US-CP-03 (Gestion de Programas por el Artista): lee promotores activos
  - US-CP-04 (Seguimiento de Comisiones): lee el PromotorId y la PromotorWallet

---

## Notas Tecnicas

### Cambio de Modelo de Dominio Requerido

La entidad `Promotor` existente en el codebase NO incluye los campos `EmailContacto` y `UrlSitioWeb` que la User Story define como campos del formulario. Antes de implementar los endpoints es necesario:

1. Agregar las propiedades a la entidad `Promotor`:
   - `public string? EmailContacto { get; set; }` (max 200)
   - `public string? UrlSitioWeb { get; set; }` (max 300)
2. Agregar la configuracion Fluent API en la clase de configuracion de EF Core para ambas columnas
3. Generar y aplicar una nueva migracion de EF Core

Este cambio no es breaking: ambas columnas son nullable y no afectan registros existentes.

### Otras Notas

- La desactivacion del perfil es logica (EsActivo = false); el registro nunca se elimina de BD
- Un usuario puede ser artista y promotor simultaneamente; ambos perfiles son independientes
- La wallet se crea en el mismo Command (transaccion) que el Promotor para garantizar consistencia
- El handler CQRS de creacion debe seguir el patron del proyecto: Command + Handler en mismo archivo, inyectar IPromotorService (no DbContext)
- Validaciones de formato de URL deben aplicarse en frontend (Zod) y backend (FluentValidation) de forma independiente

---

## Referencia

- User Story completa: [docs/product/US-CP-01-perfil-promotor.md](../../product/US-CP-01-perfil-promotor.md)

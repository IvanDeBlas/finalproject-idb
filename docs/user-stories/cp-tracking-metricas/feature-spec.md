# Feature: Tracking de Eventos, Conversiones y Dashboard de Metricas

> **ID:** cp-tracking-metricas
> **User Story:** US-CP-05
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** TBD

---

## Descripcion

Esta feature implementa el motor de medicion del modulo Crowdpromotion. Cuando un fan navega a traves del enlace referido de un promotor, el sistema captura automaticamente los parametros UTM y el codigo referido presentes en la URL, los persiste en sesion, y registra una cadena de eventos: Click al llegar, PageView al visitar la campana, Signup si el fan se registra, y Backing (conversion) si el fan realiza una aportacion. Cada evento queda almacenado en la entidad `PromoEvento` con trazabilidad completa: programa, promotor, campana, UTMs, URL de origen y referer HTTP. Al registrar un Backing referido, el sistema calcula la comision del promotor segun la configuracion del programa (porcentaje, importe fijo o el mayor de ambos) y acredita la transaccion en la wallet del promotor de forma sincrona.

La segunda parte de la feature expone los datos agregados mediante dos dashboards diferenciados. El artista propietario de un programa puede ver KPIs globales (clicks, page views, signups, conversiones, valor generado, comisiones, tasa de conversion) filtrables por rango de fechas y un ranking de promotores ordenado por conversiones con su grafico temporal de actividad. El promotor autenticado puede ver sus propias metricas individuales por programa (clicks propios, conversiones propias, valor generado y comision acumulada) junto con el historial de eventos recientes y su enlace referido para copiar rapidamente.

Sin este tracking el modulo Crowdpromotion no tiene valor economico: no se puede saber que promotor genero que conversion, ni calcular correctamente las comisiones, ni dar al artista visibilidad sobre el rendimiento de su programa. Esta feature es el cierre del loop de incentivos iniciado en US-CP-03 (inscripcion y aprobacion de promotores).

---

## User Story

**Como** artista que tiene un programa de promocion activo (y como promotor inscrito)
**Quiero** que el sistema registre automaticamente los eventos generados por los enlaces de los promotores (clicks, visitas, signups, backings) y ver un dashboard con metricas de rendimiento
**Para** medir el impacto real de la promocion y saber que promotores generan mas resultados

---

## Actores

| Actor | Descripcion |
|-------|-------------|
| Sistema | Registra eventos automaticamente al detectar codigo referido o parametros UTM en la URL de la Landing |
| Fan / Visitante | Genera eventos al navegar a traves de enlaces de promotores; puede ser anonimo o autenticado |
| Promotor | Ve sus metricas individuales por programa y copia su enlace referido |
| Artista | Ve las metricas globales de su programa y el ranking de promotores |

---

## Precondiciones

- Existen programas activos (EsActivo = true) con al menos un promotor aprobado que tiene un CodigoReferido asignado
- La tabla maestra `Maestra_TipoEventoPromo` tiene los datos seed cargados (Click, PageView, Signup, Backing, Share)
- El promotor tiene una `PromotorWallet` activa o el sistema puede crearla al registrar la primera comision

---

## Flujo Principal: Tracking Automatico de Eventos

```
1. Fan hace click en el enlace de un promotor (contiene ?ref=CODIGO&utm_source=weplay&utm_medium=referral&utm_campaign=PROGRAMA)
2. Landing detecta los parametros UTM y el codigo referido en la URL al cargar cualquier pagina
3. Frontend llama al endpoint POST /api/crowdpromotion/tracking/evento con tipoEventoPromoId = Click
4. Backend valida el codigo referido: lo resuelve a un PromoProgramaPromotor activo y aprobado
5. Backend verifica rate limiting: si la misma IP + codigoReferido ya registro un Click en los ultimos 5 minutos, retorna 429 y no crea un nuevo evento
6. Si pasa la validacion, Backend crea un PromoEvento con todos los campos de trazabilidad (UTMs, urlOrigen, urlReferer, programaId, promotorId, campaniaCrowdfundingId si aplica)
7. Frontend persiste el codigoReferido en cookie o sessionStorage para atribuir eventos posteriores dentro de la misma sesion
8. Fan navega a la pagina de la campana; Landing llama al endpoint con tipoEventoPromoId = PageView
9. Fan decide registrarse; al completar el registro exitoso, Landing llama al endpoint con tipoEventoPromoId = Signup y el userIdAfectado
10. Fan realiza un backing; el modulo de Crowdfunding llama internamente al endpoint POST /api/crowdpromotion/tracking/conversion con los datos del backing
11. Backend crea el PromoEvento tipo Backing con ValorMonetario = importe del backing
12. Backend calcula la comision segun la configuracion del programa (porcentaje, fija o la mayor de ambas)
13. Backend crea una PromotorWalletTransaccion de tipo Credito en estado Pendiente y acredita en la wallet del promotor
14. Backend retorna la comisionCalculada y el walletTransaccionId en la respuesta
```

---

## Flujo Secundario: Dashboard del Artista

```
1. Artista autenticado accede al detalle de un programa en el Admin
2. Artista selecciona la pestana "Metricas"
3. Sistema muestra los KPIs globales del programa: total clicks, page views, signups, conversiones (backings), valor total generado, tasa de conversion y comisiones totales
4. Artista puede ajustar el rango de fechas (fechaDesde, fechaHasta) mediante un selector de periodo
5. Al cambiar el periodo, los KPIs y el ranking se recalculan filtrando por el rango seleccionado
6. Sistema muestra el ranking de promotores ordenado por numero de conversiones descendente: nombre del promotor, tipo, clicks, conversiones, valor generado y comision acumulada
7. Sistema muestra el grafico temporal con eventos por dia (clicks, page views, signups, conversiones)
```

---

## Flujo Secundario: Dashboard del Promotor

```
1. Promotor autenticado accede a su seccion de metricas en la Landing
2. Sistema muestra un selector de programa (si el promotor esta en varios programas)
3. Promotor selecciona un programa
4. Sistema muestra sus KPIs individuales: mis clicks, mis conversiones, mi valor generado, mi comision acumulada y mi tasa de conversion
5. Sistema muestra el listado de eventos recientes: tipo de evento, valor monetario (si aplica), comision generada (si aplica) y fecha
6. Sistema muestra el enlace referido del promotor con un boton de copiar al portapapeles
```

---

## Flujo Secundario: Calculo de Comisiones al Registrar Conversion

```
1. Backend recibe el evento tipo Backing (conversion) con ValorMonetario
2. Backend obtiene el PromoPrograma para recuperar ImporteComisionPorcentaje, ImporteComisionFija y el tipo de calculo
3. Si tipo = Porcentaje: Comision = ValorMonetario * ImporteComisionPorcentaje / 100
4. Si tipo = Fija: Comision = ImporteComisionFija
5. Si tipo = Ambas: Comision = max(calculo porcentaje, ImporteComisionFija)
6. Backend crea PromotorWalletTransaccion con TipoTransaccion = Credito, Estado = Pendiente, Importe = Comision calculada
7. Backend incrementa SaldoActual en PromotorWallet
8. La operacion de BD es sincrona (no cola asincrona en MVP)
```

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Codigo referido invalido o no encontrado en BD | Registrar el evento sin vinculo a promotor (tracking anonimo); no calcular comision |
| FA-02 | Programa inactivo pero el codigo referido aun circula en redes sociales | Registrar el PromoEvento pero no calcular ni acreditar comision |
| FA-03 | Promotor dado de baja o bloqueado en el programa | Registrar el PromoEvento pero no calcular ni acreditar comision |
| FA-04 | Mismo IP + codigoReferido ya registro un Click en los ultimos 5 minutos | Retornar 429 Too Many Requests; no crear un nuevo PromoEvento de tipo Click |
| FA-05 | Fan llega con UTMs pero sin parametro ref | Registrar el evento con los campos UTM pero sin vinculo a promotor; PromoProgramaPromotorId queda nulo |
| FA-06 | Fan llega sin ninguno de los parametros de tracking | No llamar al endpoint de tracking; no registrar evento |
| FA-07 | Backing cancelado despues de registrar la conversion | No revertir automaticamente en MVP; ajuste manual fuera de scope |
| FA-08 | Wallet del promotor no existe al acreditar comision | Backend crea la wallet automaticamente para la moneda del programa antes de acreditar |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-CP05-1 | Al visitar una URL con parametro `ref` valido, el frontend llama al endpoint de tracking y se crea un PromoEvento de tipo Click con programaId, promotorId, urlOrigen y UTMs correctamente persistidos en BD | Backend + Landing |
| AC-CP05-2 | El codigoReferido detectado en la URL se persiste en cookie o sessionStorage; visitas posteriores dentro de la misma sesion sin parametro `ref` en la URL siguen atribuyendo eventos al mismo promotor | Landing |
| AC-CP05-3 | Al navegar a la pagina de detalle de una campana con codigo referido activo en sesion, se registra un PromoEvento de tipo PageView con CampaniaCrowdfundingId correctamente asignado | Backend + Landing |
| AC-CP05-4 | Al completar el registro de un nuevo usuario con codigo referido activo, se registra un PromoEvento de tipo Signup con UserIdAfectado correctamente asignado | Backend + Landing |
| AC-CP05-5 | Al procesar un backing con codigo referido activo, se registra un PromoEvento de tipo Backing con ValorMonetario = importe del backing y AportacionCrowdfundingId correctamente asignado | Backend |
| AC-CP05-6 | Se calcula la comision segun el tipo configurado en el programa (porcentaje, fija o la mayor de ambas) y se crea una PromotorWalletTransaccion de tipo Credito con el importe calculado; el SaldoActual del PromotorWallet se incrementa en la misma operacion | Backend |
| AC-CP05-7 | Si la misma IP + codigoReferido ya registro un Click en los ultimos 5 minutos, el endpoint retorna 429 Too Many Requests y no crea un PromoEvento adicional de tipo Click | Backend |
| AC-CP05-8 | Los eventos de un programa con EsActivo = false se registran correctamente en BD pero no generan calculo de comision ni PromotorWalletTransaccion | Backend |
| AC-CP05-9 | El artista propietario del programa puede ver el dashboard de metricas con los KPIs: totalClicks, totalPageViews, totalSignups, totalConversiones, valorTotalGenerado, tasaConversion y comisionesTotales | Backend + Admin |
| AC-CP05-10 | El dashboard del artista soporta filtrado por rango de fechas (fechaDesde y fechaHasta); al cambiar el rango los KPIs y el ranking se recalculan correctamente | Backend + Admin |
| AC-CP05-11 | El dashboard del artista muestra el ranking de promotores con: nombre del promotor, tipo de promotor, clicks generados, conversiones, valor generado y comision acumulada, ordenado por conversiones descendente | Backend + Admin |
| AC-CP05-12 | El promotor autenticado puede ver su dashboard individual con los KPIs: misClicks, misConversiones, miValorGenerado, miComisionAcumulada y miTasaConversion para el programa seleccionado | Backend + Landing |
| AC-CP05-13 | El dashboard del promotor muestra el historial de eventos recientes con tipo, valor monetario (si aplica), comision generada (si aplica) y fechaEvento | Backend + Landing |
| AC-CP05-14 | El promotor puede ver y copiar su enlace referido completo con los parametros UTM desde su dashboard | Landing |
| AC-CP05-15 | Los tipos TypeScript para PromoEventoDto, MetricasProgramaDto, RankingPromotorDto, MetricasPromotorDto y los schemas Zod para RegistrarEventoRequest estan definidos en shared y son reutilizados por Landing y Admin | Shared |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar los cuatro endpoints CQRS del modulo Crowdpromotion: POST tracking/evento (publico, con rate limiting), POST tracking/conversion (interno, llamado desde Crowdfunding), GET programas/{id}/metricas (artista autenticado), GET promotor/metricas (promotor autenticado). Implementar la logica de resolucion del codigo referido, el rate limiting por IP+ref (cache en memoria), el calculo de comisiones y la acreditacion sincrona en wallet. Implementar las queries agregadas con GROUP BY sobre PromoEvento con filtros por fecha para los dashboards. | ALTO |
| **Landing** | Implementar el interceptor de URL que detecta los parametros ref y UTM al cargar la app (App.tsx o router), persiste el codigoReferido en cookie/sessionStorage y llama al endpoint de tracking segun el tipo de evento. Implementar el dashboard del promotor en /promotor/metricas con selector de programa, KPIs individuales, historial de eventos y boton de copiar enlace referido. | ALTO |
| **Admin** | Implementar la pestana "Metricas" en el detalle del programa del artista: KPIs globales con selector de rango de fechas, ranking de promotores en tabla ordenable y grafico temporal de eventos por dia. | ALTO |
| **Shared** | Definir tipos TypeScript: PromoEventoDto, RegistrarEventoRequest, RegistrarConversionRequest, MetricasProgramaDto (con kpis, rankingPromotores, eventosPorDia), MetricasPromotorDto (con kpis y eventosRecientes), RankingPromotorItemDto. Definir schemas Zod para RegistrarEventoRequest. Agregar QUERY_KEYS para metricas-programa y metricas-promotor. | MEDIO |

---

## Reglas de Negocio

| Regla | Descripcion |
|-------|-------------|
| RN-01 | El tracking de eventos es publico: el fan puede ser anonimo; el endpoint POST tracking/evento no requiere autenticacion |
| RN-02 | Un evento con codigo referido invalido o de un promotor no aprobado se registra sin vinculo a promotor (PromoProgramaPromotorId nulo); no genera comision |
| RN-03 | Rate limiting para eventos de tipo Click: maximo 1 evento por combinacion de IP + codigoReferido en una ventana deslizante de 5 minutos |
| RN-04 | El calculo de comision solo se ejecuta si el programa esta activo (EsActivo = true) y el promotor esta aprobado (EsAprobado = true) y no bloqueado (EsBloqueado = false) en el momento del evento Backing |
| RN-05 | La acreditacion de comision en wallet es sincrona al registrar la conversion; si falla la creacion de PromotorWalletTransaccion, la conversion no se registra exitosamente (operacion atomica) |
| RN-06 | Los eventos de tipo Backing incluyen obligatoriamente ValorMonetario y AportacionCrowdfundingId para trazabilidad de la conversion |
| RN-07 | El promotor solo puede ver sus propias metricas; no puede ver las metricas de otros promotores del mismo programa |
| RN-08 | El artista solo puede ver las metricas de sus propios programas; no puede ver las de programas de otros artistas |
| RN-09 | Los PromoEvento son inmutables una vez creados; no se eliminan fisicamente para mantener trazabilidad de auditoria |

---

## Requisitos No Funcionales

| ID | Requisito |
|----|-----------|
| RNF-01 | El endpoint POST tracking/evento debe responder en menos de 200ms bajo carga normal para no impactar la experiencia de navegacion del fan |
| RNF-02 | El rate limiting debe implementarse con cache en memoria (o Redis si disponible) con clave IP + codigoReferido y TTL de 5 minutos; no debe requerir acceso a BD para la verificacion |
| RNF-03 | Las queries de dashboard usan GROUP BY sobre PromoEvento; la BD debe tener indices en las columnas PromoPrograma_Id, PromoPrograma_Promotor_Id, TipoEventoPromo_Id y FechaEvento para garantizar tiempos de respuesta aceptables |
| RNF-04 | El endpoint GET programas/{id}/metricas debe responder en menos de 800ms para programas con hasta 10.000 eventos registrados |
| RNF-05 | La acreditacion de comision al registrar una conversion es una operacion transaccional: crear PromoEvento + crear PromotorWalletTransaccion + actualizar PromotorWallet deben ejecutarse en una sola transaccion de BD; si cualquier paso falla, todos los cambios deben revertirse |
| RNF-06 | El endpoint POST tracking/evento no requiere autenticacion JWT (es publico) pero debe aplicar rate limiting por IP para prevenir abuso |
| RNF-07 | Los dashboards deben mostrar datos actualizados; no se usa cache de consulta en MVP (lectura directa de BD en cada request) |

---

## Dependencias

### Tecnicas

- ASP.NET Core Identity con JWT activo: el ArtistaId y PromotorId se extraen del claim del token en los endpoints autenticados de dashboard
- Entidad `PromoEvento` definida en el dominio Crowdpromotion con todos los campos de trazabilidad (UTMs, FKs a programa, promotor, campana, backing)
- Tabla maestra `Maestra_TipoEventoPromo` con datos seed cargados para los cinco tipos de evento (Click, PageView, Signup, Backing, Share)
- Entidad `PromotorWallet` y `PromotorWalletTransaccion` existentes en el dominio para acreditar comisiones
- Cache en memoria (IMemoryCache de ASP.NET Core) disponible para el rate limiting de clicks; no requiere Redis en MVP
- Indices de BD en PromoEvento sobre (PromoPrograma_Id, FechaEvento) y (PromoPrograma_Promotor_Id, FechaEvento) para performance de queries agregadas

### De otras features

- **cp-perfil-promotor (US-CP-01):** provee la entidad `Promotor` con NombrePublico y el PromotorId necesario para vincular eventos y wallets
- **cp-programas-promocion (US-CP-02):** provee la entidad `PromoPrograma` con EsActivo, ImporteComisionPorcentaje, ImporteComisionFija y CodigoTrackingBase; sin programas configurados no hay tracking que registrar
- **cp-inscripcion-programa (US-CP-03):** provee la entidad `PromoProgramaPromotor` con CodigoReferido, EsAprobado y EsBloqueado; esta feature es un prerequisito directo porque sin inscripcion aprobada y codigo referido asignado no existe el enlace que el promotor comparte

---

## Notas de Alcance MVP

### En scope

- Registro automatico de eventos tipo Click, PageView, Signup y Backing via endpoint publico
- Persistencia del codigo referido en sesion del usuario (cookie o sessionStorage)
- Rate limiting en memoria para eventos de tipo Click (1 por 5 min por IP + codigoReferido)
- Calculo sincrono de comision al registrar Backing; acreditacion inmediata en wallet del promotor
- Dashboard del artista con KPIs agregados, ranking de promotores y filtro por rango de fechas
- Dashboard del promotor con KPIs individuales, historial de eventos recientes y copia de enlace referido

### Diferido (fuera de MVP)

- Evento de tipo Share via boton de la app (definido en la US pero de baja prioridad)
- Reversion automatica de comision al cancelar un backing (ajuste manual en MVP segun FA-07)
- Alertas o notificaciones al artista cuando un promotor genera una conversion
- Exportacion de datos de metricas a CSV o Excel
- Rate limiting distribuido con Redis (cache en memoria es suficiente para MVP de un solo nodo)
- Grafico temporal en el dashboard del artista (puede simplificarse a tabla de datos por dia en MVP si hay restricciones de tiempo)

---

## Referencia

- User Story completa: [docs/product/US-CP-05-tracking-metricas.md](../../product/US-CP-05-tracking-metricas.md)
- Feature spec cp-inscripcion-programa: [docs/user-stories/cp-inscripcion-programa/feature-spec.md](../cp-inscripcion-programa/feature-spec.md)
- Feature spec cp-programas-promocion: [docs/user-stories/cp-programas-promocion/feature-spec.md](../cp-programas-promocion/feature-spec.md)
- Feature spec cp-perfil-promotor: [docs/user-stories/cp-perfil-promotor/feature-spec.md](../cp-perfil-promotor/feature-spec.md)
- Feature spec cp-tareas-promocion: [docs/user-stories/cp-tareas-promocion/feature-spec.md](../cp-tareas-promocion/feature-spec.md)

# Plan CQRS: cp-tracking-metricas

**Fecha:** 2026-03-02
**Modulo:** Crowdpromotion
**Feature:** cp-tracking-metricas (US-CP-05)
**Basado en:** `api-contracts.md`, `hexagonal-architecture.md`, `feature-spec.md`, `contracts.md`
**Depende de:** US-CP-03 (cp-inscripcion-programa)

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Archivo | Request | Response |
|-----------|------|---------|---------|----------|
| Registrar evento publico (Click, PageView, Signup, Share) | Command | `RegistrarEventoCommand.cs` | `RegistrarEventoCommand` | `ServiceResponse<RegistrarEventoResponseDto>` |
| Registrar conversion (Backing) + acreditar comision en wallet | Command | `RegistrarConversionCommand.cs` | `RegistrarConversionCommand` | `ServiceResponse<RegistrarConversionResponseDto>` |
| Obtener metricas del programa (dashboard artista) | Query | `GetProgramaMetricasQuery.cs` | `GetProgramaMetricasQuery` | `ServiceResponse<ProgramaMetricasResponseDto>` |
| Obtener metricas del promotor (dashboard promotor) | Query | `GetPromotorMetricasQuery.cs` | `GetPromotorMetricasQuery` | `ServiceResponse<PromotorMetricasResponseDto>` |

---

## 2. Commands

### 2.1 RegistrarEventoCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Commands/RegistrarEventoCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Auth:** Publica (`[AllowAnonymous]`). Si el usuario tiene JWT valido, `UserIdAfectado` e `IpOrigen` se inyectan desde el Controller; no vienen del body.

#### Command Properties

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `CodigoReferido` | `string?` | No | Codigo referido del promotor. Max 50 chars. Null si el fan llego sin parametro `ref`. |
| `TipoEventoPromoId` | `int` | Si | 1=Click, 2=PageView, 3=Signup, 5=Share. El valor 4 (Backing) es invalido en este endpoint. |
| `CampaniaCrowdfundingId` | `Guid?` | No | FK a CampaniaCrowdfunding. Presente en eventos PageView. |
| `UrlOrigen` | `string?` | No | URL completa desde donde llego el click. Max 2048 chars. |
| `UrlReferer` | `string?` | No | HTTP Referer header del request del fan. Max 2048 chars. |
| `UtmSource` | `string?` | No | UTM source. Siempre "weplay" cuando lo envia el frontend. Max 100 chars. |
| `UtmMedium` | `string?` | No | UTM medium. Siempre "referral". Max 100 chars. |
| `UtmCampaign` | `string?` | No | UTM campaign. Coincide con CodigoTrackingBase del programa. Max 100 chars. |
| `IpOrigen` | `string?` | No | Inyectado por el Controller desde `HttpContext.Connection.RemoteIpAddress`. Nunca viene del body. |
| `UserIdAfectado` | `string?` | No | Inyectado por el Controller desde el claim `sub` del JWT (si existe). Null si request anonimo. |

**Implementa:** `IRequest<ServiceResponse<RegistrarEventoResponseDto>>`

#### Handler: RegistrarEventoCommandHandler

**Dependencias:**

| Dependencia | Interface | Descripcion |
|-------------|-----------|-------------|
| `_promoEventoService` | `IPromoEventoService` | Resolución de CodigoReferido + persistencia del PromoEvento |
| `_rateLimitingService` | `IRateLimitingService` | Verificacion y registro del rate limit para clicks |
| `_mapper` | `IMapper` | Mapeo de Command -> PromoEvento (entity) |
| `_validator` | `IValidator<RegistrarEventoCommand>` | Validacion del command |
| `_logger` | `ILogger<RegistrarEventoCommandHandler>` | Logging de errores |

**Constructor:** Usar `?? throw new ArgumentNullException` para TODAS las dependencias.

**Flujo del Handle:**

```
1. VALIDACION
   await _validator.ValidateAsync(request, ct)
   Si !IsValid -> return ServiceResponse con validationResult.GetServiceResponseMessages()

2. RATE LIMITING (solo si TipoEventoPromoId == 1 / Click)
   bool esRateLimited = await _rateLimitingService.IsClickRateLimitedAsync(request.IpOrigen, request.CodigoReferido, ct)
   Si esRateLimited -> return ServiceResponse con ErrorCode = ServiceResponseMessageType.BusinessRule_RateLimitExcedido
   NOTA: El Controller interpreta este code y retorna HTTP 429

3. MAPPING Command -> Entity
   var entity = _mapper.Map<PromoEvento>(request)
   entity.FechaCreacion = DateTime.UtcNow

4. RESOLUCION DE CODIGO REFERIDO (dentro de PromoEventoService)
   El service ResolverPromotorPorCodigoReferidoAsync resuelve ProgramaId + PromotorId + PromoProgramaPromotorId
   Si CodigoReferido es invalido/no activo: ProgramaId y PromotorId quedan null (FA-01 - tracking anonimo)

5. PERSISTENCIA
   var eventoCreado = await _promoEventoService.RegistrarEventoAsync(entity, ct)
   NOTA: El service internamente llama ResolverPromotorPorCodigoReferidoAsync antes de insertar,
         asigna ProgramaId, PromotorId, PromoProgramaPromotorId y llama SaveChangesAsync

6. REGISTRO RATE LIMIT POST-INSERT (solo si TipoEventoPromoId == 1 / Click)
   await _rateLimitingService.RegisterClickAsync(request.IpOrigen, request.CodigoReferido, ct)
   NOTA: Solo llamar DESPUES del insert exitoso para no registrar el rate limit si el insert fallo

7. MAPEO Response
   var responseDto = new RegistrarEventoResponseDto { EventoId = eventoCreado.Id, Registrado = true }

8. RESPUESTA EXITOSA
   return ServiceResponse<RegistrarEventoResponseDto> con:
     Data = responseDto
     Messages = [{ Message = "Evento registrado", ErrorCode = ServiceResponseMessageType.Created }]

9. TRY-CATCH
   catch (Exception ex):
     _logger.LogError(ex, "Error registrando PromoEvento tipo {TipoEventoPromoId}", request.TipoEventoPromoId)
     return ServiceResponse con ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
```

**Nota critica sobre el 429:** El `ServiceResponse` retorna con `ErrorCode = ServiceResponseMessageType.BusinessRule_RateLimitExcedido ("4032")`. El Controller es responsable de detectar este error code en el response y retornar `StatusCode(429, result)` en lugar del 201/400 estandar. El Handler no conoce los status HTTP; solo retorna el ServiceResponse correcto.

---

### 2.2 RegistrarConversionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Commands/RegistrarConversionCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Auth:** Uso interno (llamado desde el handler de Crowdfunding via inyeccion directa de `IPromoEventoService` en MVP, no via HTTP entre modulos). En MVP el endpoint existe pero puede no ser invocado via HTTP.

**Atomicidad CRITICA:** PromoEvento + PromotorWalletTransaccion + actualizacion PromotorWallet en una sola transaccion de BD. Si cualquier paso falla, todos los cambios revierten (RNF-05).

#### Command Properties

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `CodigoReferido` | `string` | Si | Codigo referido del promotor. Max 50 chars. Obligatorio (el backing siempre tiene referido en sesion). |
| `CampaniaCrowdfundingId` | `Guid` | Si | FK a CampaniaCrowdfunding de la aportacion. |
| `AportacionCrowdfundingId` | `Guid` | Si | FK a AportacionCrowdfunding para trazabilidad completa. |
| `ValorMonetario` | `decimal` | Si | Importe del backing. Debe ser > 0. |
| `MonedaId` | `int` | Si | FK a MaestraMoneda. Debe ser > 0. |
| `UserIdAfectado` | `string` | Si | UserId del fan que realizo el backing. |

**Implementa:** `IRequest<ServiceResponse<RegistrarConversionResponseDto>>`

#### Handler: RegistrarConversionCommandHandler

**Dependencias:**

| Dependencia | Interface | Descripcion |
|-------------|-----------|-------------|
| `_promoEventoService` | `IPromoEventoService` | Operacion atomica: insert PromoEvento + calculo comision + acreditacion wallet |
| `_mapper` | `IMapper` | Mapeo de Command -> PromoEvento (entity) con tipo Backing = 4 fijo |
| `_validator` | `IValidator<RegistrarConversionCommand>` | Validacion del command |
| `_logger` | `ILogger<RegistrarConversionCommandHandler>` | Logging de errores |

**Constructor:** Usar `?? throw new ArgumentNullException` para TODAS las dependencias.

**Flujo del Handle:**

```
1. VALIDACION
   await _validator.ValidateAsync(request, ct)
   Si !IsValid -> return ServiceResponse con validationResult.GetServiceResponseMessages()

2. MAPPING Command -> Entity
   var entity = _mapper.Map<PromoEvento>(request)
   entity.TipoEventoId = 4  (Backing, fijo - no viene del command)
   entity.FechaCreacion = DateTime.UtcNow
   NOTA: ProgramaId, PromotorId, PromoProgramaPromotorId se resuelven dentro del Service
         a partir del CodigoReferido

3. OPERACION ATOMICA (delegada al Service)
   var result = await _promoEventoService.RegistrarConversionAsync(entity, ct)

   Dentro del Service (operacion atomica en transaccion de BD):
     a. Resolver CodigoReferido -> PromoProgramaPromotor (EsAprobado=true, EsBloqueado=false, FechaBaja=null)
     b. Si no resuelve: dejar ProgramaId/PromotorId null (FA-01); registrar evento sin comision
     c. Insertar PromoEvento via _eventoRepository.AddAsync(entity, ct)
     d. Si inscripcion resuelta AND programa.EsActivo AND inscripcion.EsAprobado AND !inscripcion.EsBloqueado:
        - Calcular comision segun tipo del programa:
            Solo porcentaje: entity.ImporteAsociado.Value * programa.ImporteComisionPorcentaje / 100
            Solo fija:       programa.ImporteComisionFija
            Ambas (ambas tienen valor > 0): MAX(calculo porcentaje, calculo fija)
            Ninguna con valor: comision = 0
        - Buscar PromotorWallet por promotorId + MonedaId del programa
        - Si wallet no existe: crear nueva PromotorWallet (FA-08)
        - Crear PromotorWalletTransaccion:
            EstadoTransaccionId = 1 (Pendiente)
            TipoTransaccion = Credito
            Importe = comision calculada
            Concepto = "Comision backing referido"
            ReferenciaExterna = evento.Id.ToString()  (para vincular a PromoEvento)
        - wallet.SaldoPendiente += comision
        - wallet.TotalGanado += comision
        - ComisionAcreditada = true
     e. Si no aplica comision (FA-02, FA-03, FA-01): ComisionAcreditada = false, ComisionCalculada = 0
     f. SaveChangesAsync() y CommitAsync() dentro del Service
     g. Retornar RegistrarConversionResult { EventoId, ComisionCalculada, WalletTransaccionId, ComisionAcreditada }

4. MAPEO Response
   var responseDto = new RegistrarConversionResponseDto
   {
       EventoId = result.EventoId,
       ComisionCalculada = result.ComisionCalculada,
       MonedaNombre = result.ComisionAcreditada ? result.MonedaNombre : null,
       WalletTransaccionId = result.WalletTransaccionId,
       ComisionAcreditada = result.ComisionAcreditada
   }

5. RESPUESTA EXITOSA
   return ServiceResponse<RegistrarConversionResponseDto> con:
     Data = responseDto
     Messages = [{ Message = "Conversion registrada", ErrorCode = ServiceResponseMessageType.Created }]

6. TRY-CATCH
   catch (Exception ex):
     _logger.LogError(ex, "Error registrando conversion. CodigoReferido: {CodigoReferido}, ValorMonetario: {ValorMonetario}",
                      request.CodigoReferido, request.ValorMonetario)
     return ServiceResponse con ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
     NOTA: Si el Service hace rollback de la transaccion, el Handler retorna el error sin estado inconsistente en BD
```

---

## 3. Queries

### 3.1 GetProgramaMetricasQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Queries/GetProgramaMetricasQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

**Auth:** Bearer JWT. Solo el artista propietario del programa puede acceder.

#### Query Properties

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `ProgramaId` | `Guid` | Si | ID del programa de promocion (path param, inyectado por el Controller). |
| `UserId` | `string` | Si | Inyectado por el Controller desde el claim `sub` del JWT. Nunca viene como query param. |
| `FechaDesde` | `DateOnly?` | No | Fecha inicio del periodo. Default: min(programa.FechaInicio, hoy - 90 dias). |
| `FechaHasta` | `DateOnly?` | No | Fecha fin del periodo. Default: hoy (UTC). |

**Implementa:** `IRequest<ServiceResponse<ProgramaMetricasResponseDto>>`

#### Handler: GetProgramaMetricasQueryHandler

**Dependencias:**

| Dependencia | Interface | Descripcion |
|-------------|-----------|-------------|
| `_promoEventoService` | `IPromoEventoService` | Obtener metricas (KPIs + ranking + serie temporal) y resolver Artista/Programa |
| `_validator` | `IValidator<GetProgramaMetricasQuery>` | Validacion del query |
| `_logger` | `ILogger<GetProgramaMetricasQueryHandler>` | Logging de errores |

**NOTA:** Este handler NO inyecta `IMapper` porque los DTOs de response son construidos directamente en el Service desde queries agregadas (GROUP BY), no desde mapeo de entidades. Esta es la excepcion justificada a la Regla 4 documentada en `api-contracts.md` Seccion 7.1.

**Constructor:** Usar `?? throw new ArgumentNullException` para TODAS las dependencias.

**Flujo del Handle:**

```
1. VALIDACION (formato)
   await _validator.ValidateAsync(request, ct)
   Si !IsValid -> return ServiceResponse con validationResult.GetServiceResponseMessages()

2. RESOLVER ARTISTA (validacion de negocio, requiere DB - no va al Validator)
   var artista = await _promoEventoService.GetArtistaByUserIdAsync(request.UserId, ct)
   Si artista == null:
     return ServiceResponse con ErrorCode = ServiceResponseMessageType.NotFound_Artista
     Message = "No tienes un perfil de artista"

3. VERIFICAR PROGRAMA (validacion de negocio, requiere DB - no va al Validator)
   var programa = await _promoEventoService.GetProgramaByIdAsync(request.ProgramaId, ct)
   Si programa == null:
     return ServiceResponse con ErrorCode = ServiceResponseMessageType.NotFound_PromoPrograma
     Message = "El programa de promocion no existe"

4. VERIFICAR PROPIEDAD (regla de negocio - no va al Validator)
   Si programa.ArtistaId != artista.Id:
     return ServiceResponse con ErrorCode = ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma
     Message = "No tienes permiso para ver las metricas de este programa"

5. CALCULAR DEFAULTS DE FECHAS (logica de negocio en el Handler)
   fechaDesde = request.FechaDesde ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-90))
   fechaHasta = request.FechaHasta ?? DateOnly.FromDateTime(DateTime.UtcNow)
   NOTA: Si FechaDesde > fechaHasta (con defaults calculados), el Validator ya lo captura en forma de string.
         Con defaults siempre es coherente (hoy-90 <= hoy).

6. OBTENER METRICAS (agregadas por el Service, retorna DTO directamente - excepcion justificada)
   var metricasDto = await _promoEventoService.GetProgramaMetricasAsync(
       request.ProgramaId, fechaDesde, fechaHasta, ct)

7. RESPUESTA EXITOSA
   return ServiceResponse<ProgramaMetricasResponseDto> con:
     Data = metricasDto
     Messages = [{ Message = "Metricas obtenidas", ErrorCode = ServiceResponseMessageType.Success }]

8. TRY-CATCH
   catch (Exception ex):
     _logger.LogError(ex, "Error obteniendo metricas del programa {ProgramaId} para usuario {UserId}",
                      request.ProgramaId, request.UserId)
     return ServiceResponse con ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
```

---

### 3.2 GetPromotorMetricasQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Queries/GetPromotorMetricasQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

**Auth:** Bearer JWT. El promotor solo puede ver sus propias metricas (RN-07).

#### Query Properties

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `UserId` | `string` | Si | Inyectado por el Controller desde el claim `sub` del JWT. Nunca viene como query param. |
| `ProgramaId` | `Guid?` | No | Filtrar por programa especifico. Sin valor: metricas de todos los programas del promotor. |
| `FechaDesde` | `DateOnly?` | No | Fecha inicio del periodo. Default: hoy - 90 dias. |
| `FechaHasta` | `DateOnly?` | No | Fecha fin del periodo. Default: hoy (UTC). |

**Implementa:** `IRequest<ServiceResponse<PromotorMetricasResponseDto>>`

#### Handler: GetPromotorMetricasQueryHandler

**Dependencias:**

| Dependencia | Interface | Descripcion |
|-------------|-----------|-------------|
| `_promoEventoService` | `IPromoEventoService` | Obtener metricas individuales del promotor y resolver PromotorId |
| `_validator` | `IValidator<GetPromotorMetricasQuery>` | Validacion del query |
| `_logger` | `ILogger<GetPromotorMetricasQueryHandler>` | Logging de errores |

**NOTA:** Este handler NO inyecta `IMapper` por la misma razon que el handler anterior (agregaciones desde el Service).

**Constructor:** Usar `?? throw new ArgumentNullException` para TODAS las dependencias.

**Flujo del Handle:**

```
1. VALIDACION (formato)
   await _validator.ValidateAsync(request, ct)
   Si !IsValid -> return ServiceResponse con validationResult.GetServiceResponseMessages()

2. RESOLVER PROMOTOR (validacion de negocio, requiere DB - no va al Validator)
   var promotor = await _promoEventoService.GetPromotorByUserIdAsync(request.UserId, ct)
   Si promotor == null:
     return ServiceResponse con ErrorCode = ServiceResponseMessageType.NotFound_Promotor
     Message = "No tienes un perfil de promotor"

3. CALCULAR DEFAULTS DE FECHAS (logica de negocio en el Handler)
   fechaDesde = request.FechaDesde ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-90))
   fechaHasta = request.FechaHasta ?? DateOnly.FromDateTime(DateTime.UtcNow)

4. OBTENER METRICAS (agregadas por el Service, retorna DTO directamente - excepcion justificada)
   var metricasDto = await _promoEventoService.GetPromotorMetricasAsync(
       promotor.Id, request.ProgramaId, fechaDesde, fechaHasta, ct)

5. RESPUESTA EXITOSA
   return ServiceResponse<PromotorMetricasResponseDto> con:
     Data = metricasDto
     Messages = [{ Message = "Metricas obtenidas", ErrorCode = ServiceResponseMessageType.Success }]

6. TRY-CATCH
   catch (Exception ex):
     _logger.LogError(ex, "Error obteniendo metricas del promotor para usuario {UserId}", request.UserId)
     return ServiceResponse con ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
```

---

## 4. Validators

**Ubicacion de todos los validators:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Validators/`

**Import critico en todos los validators:**
```csharp
using WePlayRises.Crowdpromotion.Domain.Constants;
```

**Patron a seguir en todos los validators:**
- `WithMessage("...")` Y `WithErrorCode(ServiceResponseMessageType.X)` en CADA regla
- NO usar strings literales como ErrorCode: usar la constante tipada
- Validaciones que requieren acceso a BD van en el Handler (no en el Validator)

---

### 4.1 RegistrarEventoCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Validators/RegistrarEventoCommandValidator.cs`

**No inyecta servicios.** Todas las reglas son de formato/estructura pura.

| Campo | Regla FluentValidation | Mensaje | ErrorCode (Constante) |
|-------|------------------------|---------|----------------------|
| `TipoEventoPromoId` | `GreaterThan(0)` | "El tipo de evento es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `TipoEventoPromoId` | `InclusiveBetween(1, 5)` | "Tipo de evento invalido. Valores validos: 1 (Click), 2 (PageView), 3 (Signup), 4 (Backing), 5 (Share)" | `ServiceResponseMessageType.Validation_TipoEventoInvalido` ("1033") |
| `TipoEventoPromoId` | `NotEqual(4)` | "El tipo de evento Backing no se acepta en este endpoint. Use el endpoint de conversion" | `ServiceResponseMessageType.Validation_BackingNoPermitido` ("1034") |
| `CodigoReferido` | `MaximumLength(50)` cuando `!string.IsNullOrEmpty(x.CodigoReferido)` | "El codigo referido no puede superar los 50 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |
| `UrlOrigen` | `MaximumLength(2048)` cuando `!string.IsNullOrEmpty(x.UrlOrigen)` | "La URL de origen no puede superar los 2048 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |
| `UrlReferer` | `MaximumLength(2048)` cuando `!string.IsNullOrEmpty(x.UrlReferer)` | "La URL del referer no puede superar los 2048 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |
| `UtmSource` | `MaximumLength(100)` cuando `!string.IsNullOrEmpty(x.UtmSource)` | "El utm_source no puede superar los 100 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |
| `UtmMedium` | `MaximumLength(100)` cuando `!string.IsNullOrEmpty(x.UtmMedium)` | "El utm_medium no puede superar los 100 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |
| `UtmCampaign` | `MaximumLength(100)` cuando `!string.IsNullOrEmpty(x.UtmCampaign)` | "El utm_campaign no puede superar los 100 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |

**Notas de validacion:**
- `IpOrigen` y `UserIdAfectado` son inyectados por el Controller y no se validan en el Validator (pueden ser null legitimos).
- `CampaniaCrowdfundingId` es `Guid?`; si se envia un GUID con formato invalido, el model binding lo rechaza antes de llegar al Validator.
- La verificacion de rate limiting (429) NO va en el Validator, sino en el Handler via `IRateLimitingService`.
- Las reglas de `TipoEventoPromoId` tienen un orden importante: primero `GreaterThan(0)` para validar que no es cero, luego `InclusiveBetween(1,5)` para el rango, luego `NotEqual(4)` para excluir Backing. FluentValidation ejecuta todas las reglas de un campo salvo que se use `CascadeMode.Stop`.
- Para el `CodigoReferido`, `UrlOrigen`, `UrlReferer`, `UtmSource`, `UtmMedium`, `UtmCampaign` se usa `When(x => !string.IsNullOrEmpty(x.Campo))` para no validar si el campo es null/empty (son opcionales).

---

### 4.2 RegistrarConversionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Validators/RegistrarConversionCommandValidator.cs`

**No inyecta servicios.** Todas las reglas son de formato/estructura pura.

| Campo | Regla FluentValidation | Mensaje | ErrorCode (Constante) |
|-------|------------------------|---------|----------------------|
| `CodigoReferido` | `NotEmpty()` | "El codigo referido es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `CodigoReferido` | `MaximumLength(50)` | "El codigo referido no puede superar los 50 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |
| `CampaniaCrowdfundingId` | `NotEmpty()` | "La campana es obligatoria" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `AportacionCrowdfundingId` | `NotEmpty()` | "La aportacion es obligatoria" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `ValorMonetario` | `GreaterThan(0)` | "El valor monetario debe ser mayor que cero" | `ServiceResponseMessageType.Validation_ValorMonetarioInvalido` ("1035") |
| `MonedaId` | `GreaterThan(0)` | "La moneda es obligatoria" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `UserIdAfectado` | `NotEmpty()` | "El usuario que realizo el backing es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |

---

### 4.3 GetProgramaMetricasQueryValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Validators/GetProgramaMetricasQueryValidator.cs`

**No inyecta servicios.** Las validaciones de negocio (existencia del artista, propiedad del programa) van en el Handler.

| Campo | Regla FluentValidation | Mensaje | ErrorCode (Constante) |
|-------|------------------------|---------|----------------------|
| `ProgramaId` | `NotEmpty()` | "El identificador del programa es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `UserId` | `NotEmpty()` | "El usuario no pudo ser identificado" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `FechaDesde` | `.Must((query, fechaDesde) => fechaDesde == null \|\| query.FechaHasta == null \|\| fechaDesde <= query.FechaHasta)` cuando `FechaDesde != null` | "La fecha de inicio no puede ser posterior a la fecha fin" | `ServiceResponseMessageType.Validation_FechaRangoInvalido` ("1036") |

**Nota sobre la validacion de fechas:** La regla cross-field se aplica sobre el campo `FechaDesde` usando un `Must` que accede a toda la query via el parametro `(query, fechaDesde)`. La condicion evalua `fechaDesde <= query.FechaHasta` solo cuando ambas fechas tienen valor.

---

### 4.4 GetPromotorMetricasQueryValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Validators/GetPromotorMetricasQueryValidator.cs`

**No inyecta servicios.** La validacion de existencia del Promotor va en el Handler.

| Campo | Regla FluentValidation | Mensaje | ErrorCode (Constante) |
|-------|------------------------|---------|----------------------|
| `UserId` | `NotEmpty()` | "El usuario no pudo ser identificado" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `FechaDesde` | `.Must((query, fechaDesde) => fechaDesde == null \|\| query.FechaHasta == null \|\| fechaDesde <= query.FechaHasta)` cuando `FechaDesde != null` | "La fecha de inicio no puede ser posterior a la fecha fin" | `ServiceResponseMessageType.Validation_FechaRangoInvalido` ("1036") |

---

## 5. DTOs de Response

Todos los DTOs de response se definen como archivos separados en la carpeta `Dtos/`.

**Carpeta:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/`

### 5.1 RegistrarEventoResponseDto

**Archivo:** `RegistrarEventoResponseDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `EventoId` | `Guid` | ID del PromoEvento creado. Presente siempre que el insert sea exitoso. |
| `Registrado` | `bool` | Siempre `true` cuando se retorna 201. |

### 5.2 RegistrarConversionResponseDto

**Archivo:** `RegistrarConversionResponseDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `EventoId` | `Guid` | ID del PromoEvento tipo Backing creado. |
| `ComisionCalculada` | `decimal` | Importe de la comision calculada. Cero si no aplica comision. |
| `MonedaNombre` | `string?` | Nombre de la moneda. Null si `ComisionAcreditada = false`. |
| `WalletTransaccionId` | `Guid?` | ID de la PromotorWalletTransaccion creada. Null si no se genero comision. |
| `ComisionAcreditada` | `bool` | `true` solo cuando se creo PromotorWalletTransaccion y se actualizo el saldo. |

### 5.3 ProgramaMetricasKpisDto

**Archivo:** `ProgramaMetricasKpisDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `TotalClicks` | `int` | COUNT(TipoEventoPromoId=1 en el periodo filtrado) |
| `TotalPageViews` | `int` | COUNT(TipoEventoPromoId=2) |
| `TotalSignups` | `int` | COUNT(TipoEventoPromoId=3) |
| `TotalConversiones` | `int` | COUNT(TipoEventoPromoId=4) |
| `ValorTotalGenerado` | `decimal` | SUM(ImporteAsociado WHERE TipoEventoPromoId=4). Cero si sin conversiones. |
| `MonedaNombre` | `string?` | Resuelto desde PromoPrograma.MonedaId. Null si sin conversiones. |
| `TasaConversion` | `decimal` | TotalConversiones / TotalClicks * 100. Cero si TotalClicks = 0. Redondeado a 2 decimales. |
| `ComisionesTotales` | `decimal` | SUM de PromotorWalletTransaccion.Importe vinculadas al programa en el periodo. |

### 5.4 RankingPromotorItemDto

**Archivo:** `RankingPromotorItemDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `PromotorId` | `Guid` | ID del Promotor. |
| `PromotorNombre` | `string` | Promotor.NombrePublico. |
| `TipoPromotorNombre` | `string?` | Resuelto desde MaestraTipoPromotor. Null si no se puede resolver. |
| `Clicks` | `int` | COUNT eventos tipo Click del promotor en el periodo. |
| `PageViews` | `int` | COUNT eventos tipo PageView. |
| `Signups` | `int` | COUNT eventos tipo Signup. |
| `Conversiones` | `int` | COUNT eventos tipo Backing. Columna de ordenacion DESC. |
| `ValorGenerado` | `decimal` | SUM(ImporteAsociado) de eventos tipo Backing del promotor. |
| `ComisionAcumulada` | `decimal` | SUM de PromotorWalletTransaccion.Importe del promotor en el programa y periodo. |

### 5.5 EventosPorDiaItemDto

**Archivo:** `EventosPorDiaItemDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Fecha` | `string` | Fecha en formato `YYYY-MM-DD`. |
| `Clicks` | `int` | COUNT eventos tipo Click en ese dia. |
| `PageViews` | `int` | COUNT eventos tipo PageView. |
| `Signups` | `int` | COUNT eventos tipo Signup. |
| `Conversiones` | `int` | COUNT eventos tipo Backing. |

### 5.6 ProgramaMetricasResponseDto

**Archivo:** `ProgramaMetricasResponseDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `ProgramaId` | `Guid` | ID del programa consultado. |
| `ProgramaTitulo` | `string` | Titulo del programa. |
| `FechaDesde` | `string` | Fecha inicio del periodo aplicado en formato `YYYY-MM-DD`. |
| `FechaHasta` | `string` | Fecha fin del periodo aplicado en formato `YYYY-MM-DD`. |
| `Kpis` | `ProgramaMetricasKpisDto` | KPIs agregados del programa en el periodo. |
| `RankingPromotores` | `IReadOnlyList<RankingPromotorItemDto>` | Ranking ordenado por Conversiones DESC. Solo promotores con >= 1 evento. |
| `EventosPorDia` | `IReadOnlyList<EventosPorDiaItemDto>` | Serie temporal. Solo dias con actividad. |

### 5.7 PromotorMetricasKpisDto

**Archivo:** `PromotorMetricasKpisDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `MisClicks` | `int` | COUNT(TipoEventoPromoId=1) del promotor en el periodo. |
| `MisPageViews` | `int` | COUNT(TipoEventoPromoId=2). |
| `MisSignups` | `int` | COUNT(TipoEventoPromoId=3). |
| `MisConversiones` | `int` | COUNT(TipoEventoPromoId=4). |
| `MiValorGenerado` | `decimal` | SUM(ImporteAsociado) de conversiones propias en el periodo. |
| `MiComisionAcumulada` | `decimal` | SUM de PromotorWalletTransaccion.Importe del promotor en el periodo. |
| `MonedaNombre` | `string?` | Moneda de la comision. Null si sin conversiones. |
| `MiTasaConversion` | `decimal` | MisConversiones / MisClicks * 100. Cero si MisClicks = 0. Redondeado a 2 decimales. |

### 5.8 EventoRecienteDto

**Archivo:** `EventoRecienteDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del PromoEvento. |
| `TipoEventoId` | `int` | Siempre 4 (Backing) en esta lista. Se incluye para consistencia del tipo. |
| `TipoEventoNombre` | `string` | Resuelto desde Maestra_TipoEventoPromo. Siempre "Backing" en esta lista. |
| `ValorMonetario` | `decimal` | Importe del backing asociado (ImporteAsociado de PromoEvento). |
| `ComisionGenerada` | `decimal?` | Comision acreditada para este evento. Null si no se genero comision. |
| `MonedaNombre` | `string?` | Nombre de la moneda. Resuelto desde MaestraMoneda. |
| `FechaEvento` | `DateTime` | Timestamp del evento en UTC. Serializa como ISO 8601. |

### 5.9 PromotorMetricasResponseDto

**Archivo:** `PromotorMetricasResponseDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `PromotorId` | `Guid` | ID del promotor autenticado. |
| `PromotorNombre` | `string` | Promotor.NombrePublico. |
| `ProgramaId` | `Guid?` | ID del programa filtrado. Null si la consulta no filtra por programa. |
| `ProgramaTitulo` | `string?` | Titulo del programa filtrado. Null si sin filtro de programa. |
| `FechaDesde` | `string` | Fecha inicio del periodo aplicado en formato `YYYY-MM-DD`. |
| `FechaHasta` | `string` | Fecha fin del periodo aplicado en formato `YYYY-MM-DD`. |
| `Kpis` | `PromotorMetricasKpisDto` | KPIs individuales del promotor. |
| `EventosRecientes` | `IReadOnlyList<EventoRecienteDto>` | Ultimos 20 eventos tipo Backing. Ordenados por FechaEvento DESC. |

---

## 6. AutoMapper Profile

### 6.1 PromoEventoProfile

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/PromoEventoProfile.cs`

**Descripcion:** Solo define mappings de Command -> Entity para la creacion del PromoEvento. Los DTOs de response de metricas (ProgramaMetricasResponseDto, PromotorMetricasResponseDto y sus componentes) NO usan AutoMapper porque se construyen directamente en el Service desde queries agregadas (GROUP BY).

#### Mapping 1: RegistrarEventoCommand -> PromoEvento

| Source Property | Destination Property | Tipo de Mapeo | Notas |
|-----------------|----------------------|---------------|-------|
| `TipoEventoPromoId` | `TipoEventoId` | `MapFrom(src => src.TipoEventoPromoId)` | Renombrado: contrato usa TipoEventoPromoId, entidad usa TipoEventoId |
| `CampaniaCrowdfundingId` | `CampaniaCrowdfundingId` | Automatico (mismo nombre) | Nullable Guid |
| `CodigoReferido` | `CodigoReferido` | Automatico (mismo nombre) | Nullable string |
| `IpOrigen` | `IpOrigen` | Automatico (mismo nombre) | Nullable string |
| `UrlOrigen` | `UrlOrigen` | Automatico (mismo nombre) | Nuevo campo en entidad |
| `UrlReferer` | `UrlReferer` | Automatico (mismo nombre) | Nuevo campo en entidad |
| `UtmSource` | `UtmSource` | Automatico (mismo nombre) | Nuevo campo en entidad |
| `UtmMedium` | `UtmMedium` | Automatico (mismo nombre) | Nuevo campo en entidad |
| `UtmCampaign` | `UtmCampaign` | Automatico (mismo nombre) | Nuevo campo en entidad |
| `UserIdAfectado` | `UserIdAfectado` | Automatico (mismo nombre) | Nuevo campo en entidad |
| N/A | `Id` | `Ignore()` | Guid generado por EF Core |
| N/A | `ProgramaId` | `Ignore()` | Resuelto en el Service via CodigoReferido |
| N/A | `PromotorId` | `Ignore()` | Resuelto en el Service via CodigoReferido |
| N/A | `PromoProgramaPromotorId` | `Ignore()` | Resuelto en el Service via CodigoReferido |
| N/A | `FechaCreacion` | `Ignore()` | Asignado en el Handler/Service como DateTime.UtcNow |
| N/A | `MonedaId` | `Ignore()` | Null para eventos no-Backing |
| N/A | `ImporteAsociado` | `Ignore()` | Null para eventos no-Backing |
| N/A | `AportacionCrowdfundingId` | `Ignore()` | Solo tipo Backing |
| N/A | `Programa` | `Ignore()` | Navigation property |
| N/A | `Promotor` | `Ignore()` | Navigation property |

#### Mapping 2: RegistrarConversionCommand -> PromoEvento

| Source Property | Destination Property | Tipo de Mapeo | Notas |
|-----------------|----------------------|---------------|-------|
| `ValorMonetario` | `ImporteAsociado` | `MapFrom(src => src.ValorMonetario)` | Renombrado: contrato usa ValorMonetario, entidad usa ImporteAsociado |
| `MonedaId` | `MonedaId` | Automatico (mismo nombre) | FK a MaestraMoneda |
| `CampaniaCrowdfundingId` | `CampaniaCrowdfundingId` | Automatico (mismo nombre) | Guid no-nullable |
| `AportacionCrowdfundingId` | `AportacionCrowdfundingId` | Automatico (mismo nombre) | Guid no-nullable |
| `CodigoReferido` | `CodigoReferido` | Automatico (mismo nombre) | String obligatorio |
| `UserIdAfectado` | `UserIdAfectado` | Automatico (mismo nombre) | UserId del fan |
| N/A | `TipoEventoId` | `Ignore()` | Fijado en 4 (Backing) en el Handler/Service |
| N/A | `Id` | `Ignore()` | Guid generado por EF Core |
| N/A | `ProgramaId` | `Ignore()` | Resuelto en el Service via CodigoReferido |
| N/A | `PromotorId` | `Ignore()` | Resuelto en el Service via CodigoReferido |
| N/A | `PromoProgramaPromotorId` | `Ignore()` | Resuelto en el Service via CodigoReferido |
| N/A | `FechaCreacion` | `Ignore()` | Asignado en el Service |
| N/A | `IpOrigen` | `Ignore()` | No aplica para conversiones internas |
| N/A | `UserAgentOrigen` | `Ignore()` | No aplica para conversiones internas |
| N/A | `UrlOrigen` | `Ignore()` | No aplica para conversiones internas |
| N/A | `UrlReferer` | `Ignore()` | No aplica para conversiones internas |
| N/A | `UtmSource` | `Ignore()` | No aplica para conversiones internas |
| N/A | `UtmMedium` | `Ignore()` | No aplica para conversiones internas |
| N/A | `UtmCampaign` | `Ignore()` | No aplica para conversiones internas |
| N/A | `Programa` | `Ignore()` | Navigation property |
| N/A | `Promotor` | `Ignore()` | Navigation property |

---

## 7. Estructura de Archivos a Crear

```
src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/
├── Features/
│   └── Tracking/
│       ├── Commands/
│       │   ├── RegistrarEventoCommand.cs         NUEVO: Command + Handler en el mismo archivo
│       │   └── RegistrarConversionCommand.cs     NUEVO: Command + Handler en el mismo archivo
│       ├── Queries/
│       │   ├── GetProgramaMetricasQuery.cs       NUEVO: Query + Handler en el mismo archivo
│       │   └── GetPromotorMetricasQuery.cs       NUEVO: Query + Handler en el mismo archivo
│       └── Validators/
│           ├── RegistrarEventoCommandValidator.cs          NUEVO
│           ├── RegistrarConversionCommandValidator.cs      NUEVO
│           ├── GetProgramaMetricasQueryValidator.cs        NUEVO
│           └── GetPromotorMetricasQueryValidator.cs        NUEVO
├── Dtos/
│   ├── RegistrarEventoResponseDto.cs             NUEVO
│   ├── RegistrarConversionResponseDto.cs         NUEVO
│   ├── ProgramaMetricasKpisDto.cs                NUEVO
│   ├── RankingPromotorItemDto.cs                 NUEVO
│   ├── EventosPorDiaItemDto.cs                   NUEVO
│   ├── ProgramaMetricasResponseDto.cs            NUEVO
│   ├── PromotorMetricasKpisDto.cs                NUEVO
│   ├── EventoRecienteDto.cs                      NUEVO
│   └── PromotorMetricasResponseDto.cs            NUEVO
└── Mapping/
    └── PromoEventoProfile.cs                     NUEVO: 2 mappings Command->Entity
```

---

## 8. Patrones Importantes

### 8.1 Separacion de Responsabilidades Handler vs Service

```
Handler es responsable de:
- Validacion de formato (via Validator)
- Validaciones de negocio que requieren DB (resolver Artista, Promotor, verificar propiedad del Programa)
- Calcular defaults de fechas (logica de negocio pura, sin acceso a datos)
- Verificar rate limiting (via IRateLimitingService, antes de la operacion principal)
- Registrar el rate limit POST-insert exitoso (solo si TipoEventoPromoId == Click)
- Mapping del resultado a ServiceResponse<T>
- Try-catch con logging

Service es responsable de:
- Resolver CodigoReferido a PromoProgramaPromotor (consulta a BD)
- Insertar PromoEvento (para eventos simples)
- Operacion atomica completa para conversiones (BEGIN TRANSACTION / COMMIT / ROLLBACK)
- Calcular comision segun tipo del programa (porcentaje, fija, max de ambas)
- Crear/buscar PromotorWallet del promotor
- Crear PromotorWalletTransaccion
- Actualizar PromotorWallet.SaldoPendiente y TotalGanado
- Queries agregadas (GROUP BY) para metricas del artista
- Queries de metricas individuales del promotor
- Enriquecer rankings con NombrePublico y TipoPromotorNombre del Promotor
```

### 8.2 Excepcion Justificada a la Regla 4 (Services retornan Entidades, NO DTOs)

Los metodos `GetProgramaMetricasAsync` y `GetPromotorMetricasAsync` de `IPromoEventoService` retornan DTOs directamente:

- `GetProgramaMetricasAsync` -> `ProgramaMetricasResponseDto`
- `GetPromotorMetricasAsync` -> `PromotorMetricasResponseDto`

**Justificacion:** Los datos son calculados mediante queries agregadas (GROUP BY, COUNT, SUM), no son entidades del dominio. No existe una entidad de dominio "MetricasPrograma" que mapear. Retornar tipos intermedios forzaria una doble iteracion sobre colecciones ya calculadas sin beneficio arquitectonico. Los Handlers siguen siendo los responsables del `ServiceResponse<T>` wrapping.

### 8.3 Flujo de 429 Too Many Requests

El Handler retorna `ServiceResponse<RegistrarEventoResponseDto>` con `ErrorCode = ServiceResponseMessageType.BusinessRule_RateLimitExcedido ("4032")` en el cuerpo del mensaje. El Controller (`TrackingController.RegistrarEvento`) detecta este ErrorCode en la respuesta e invoca `StatusCode(429, result)` en lugar del `Created(201)` estandar. Esta responsabilidad de traduccion HTTP/codigo queda en el Controller, no en el Handler.

### 8.4 Atomicidad de RegistrarConversionAsync

La atomicidad (RNF-05) es responsabilidad del `PromoEventoService`, no del Handler. El Service usa `await using var transaction = await _context.Database.BeginTransactionAsync(ct)` y coordina:
1. AddAsync(PromoEvento)
2. Resolver comision y crear PromotorWalletTransaccion
3. Update PromotorWallet.SaldoPendiente
4. SaveChangesAsync()
5. CommitAsync()

Si cualquier paso falla, el Service hace `RollbackAsync()` y relanza la excepcion. El Handler captura la excepcion en su try-catch y retorna `Internal_UnexpectedError`.

### 8.5 ServiceResponse con Constants (NO strings literales)

```
// CORRECTO - Constants de dominio
ServiceResponseMessageType.Created                            ("0001")
ServiceResponseMessageType.Success                            ("0000")
ServiceResponseMessageType.Validation_Required                ("1001")
ServiceResponseMessageType.Validation_MaxLength               ("1002")
ServiceResponseMessageType.Validation_TipoEventoInvalido      ("1033")  // NUEVO
ServiceResponseMessageType.Validation_BackingNoPermitido       ("1034")  // NUEVO
ServiceResponseMessageType.Validation_ValorMonetarioInvalido   ("1035")  // NUEVO
ServiceResponseMessageType.Validation_FechaRangoInvalido       ("1036")  // NUEVO
ServiceResponseMessageType.NotFound_Promotor                  ("2015")  // existente
ServiceResponseMessageType.NotFound_Artista                   ("2016")  // existente
ServiceResponseMessageType.NotFound_PromoPrograma             ("2019")  // existente
ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma ("4026") // existente
ServiceResponseMessageType.BusinessRule_RateLimitExcedido      ("4032")  // NUEVO
ServiceResponseMessageType.Internal_UnexpectedError            ("5000")  // existente

// INCORRECTO - Strings literales NUNCA
ServiceResponseMessageType.Created -> "0001"  // NO HACER
```

---

## 9. Interfaces de Servicio Requeridas por los Handlers

Los Handlers inyectan `IPromoEventoService` e `IRateLimitingService`. Ambas interfaces viven en la capa Application.

### 9.1 IPromoEventoService (Application/Interfaces/Services/)

Los Handlers usan los siguientes metodos de esta interface:

| Metodo | Usado por | Descripcion |
|--------|-----------|-------------|
| `RegistrarEventoAsync(PromoEvento evento, CancellationToken ct)` | RegistrarEventoCommandHandler | Inserta PromoEvento resolviendo CodigoReferido internamente |
| `RegistrarConversionAsync(PromoEvento evento, CancellationToken ct)` | RegistrarConversionCommandHandler | Operacion atomica: insert + comision + wallet |
| `GetArtistaByUserIdAsync(string userId, CancellationToken ct)` | GetProgramaMetricasQueryHandler | Resolver UserId -> Artista |
| `GetProgramaByIdAsync(Guid programaId, CancellationToken ct)` | GetProgramaMetricasQueryHandler | Verificar existencia y propiedad del programa |
| `GetProgramaMetricasAsync(Guid programaId, DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken ct)` | GetProgramaMetricasQueryHandler | Metricas agregadas del programa; retorna DTO directamente |
| `GetPromotorByUserIdAsync(string userId, CancellationToken ct)` | GetPromotorMetricasQueryHandler | Resolver UserId -> Promotor |
| `GetPromotorMetricasAsync(Guid promotorId, Guid? programaId, DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken ct)` | GetPromotorMetricasQueryHandler | Metricas individuales del promotor; retorna DTO directamente |

### 9.2 IRateLimitingService (Application/Interfaces/Services/)

| Metodo | Usado por | Descripcion |
|--------|-----------|-------------|
| `IsClickRateLimitedAsync(string ip, string? codigoReferido, CancellationToken ct)` | RegistrarEventoCommandHandler (step 2) | Verifica si ya registro un Click en los ultimos 5 min |
| `RegisterClickAsync(string ip, string? codigoReferido, CancellationToken ct)` | RegistrarEventoCommandHandler (step 6) | Registra la clave en cache post-insert exitoso |

---

## 10. Tabla de Errores por Handler

### RegistrarEventoCommandHandler

| Paso | Condicion | ErrorCode | Constante | HTTP (Controller) |
|------|-----------|-----------|-----------|-------------------|
| 1 - Validacion | TipoEventoPromoId == 0 | `"1001"` | `Validation_Required` | 400 |
| 1 - Validacion | TipoEventoPromoId fuera de 1-5 | `"1033"` | `Validation_TipoEventoInvalido` | 400 |
| 1 - Validacion | TipoEventoPromoId == 4 | `"1034"` | `Validation_BackingNoPermitido` | 400 |
| 1 - Validacion | CodigoReferido > 50 chars | `"1002"` | `Validation_MaxLength` | 400 |
| 1 - Validacion | UrlOrigen > 2048 chars | `"1002"` | `Validation_MaxLength` | 400 |
| 2 - Rate limit | IP + ref ya registrado en 5 min | `"4032"` | `BusinessRule_RateLimitExcedido` | 429 |
| 5 - Persistencia | Excepcion no controlada | `"5000"` | `Internal_UnexpectedError` | 500 |
| Exito | Evento insertado | `"0001"` | `Created` | 201 |

### RegistrarConversionCommandHandler

| Paso | Condicion | ErrorCode | Constante | HTTP (Controller) |
|------|-----------|-----------|-----------|-------------------|
| 1 - Validacion | CodigoReferido vacio | `"1001"` | `Validation_Required` | 400 |
| 1 - Validacion | ValorMonetario <= 0 | `"1035"` | `Validation_ValorMonetarioInvalido` | 400 |
| 1 - Validacion | CampaniaCrowdfundingId vacio | `"1001"` | `Validation_Required` | 400 |
| 1 - Validacion | AportacionCrowdfundingId vacio | `"1001"` | `Validation_Required` | 400 |
| 3 - Atomica | Excepcion / rollback | `"5000"` | `Internal_UnexpectedError` | 500 |
| Exito | Conversion registrada | `"0001"` | `Created` | 201 |

### GetProgramaMetricasQueryHandler

| Paso | Condicion | ErrorCode | Constante | HTTP (Controller) |
|------|-----------|-----------|-----------|-------------------|
| 1 - Validacion | ProgramaId vacio | `"1001"` | `Validation_Required` | 400 |
| 1 - Validacion | UserId vacio | `"1001"` | `Validation_Required` | 400 |
| 1 - Validacion | FechaDesde > FechaHasta | `"1036"` | `Validation_FechaRangoInvalido` | 400 |
| 2 - Artista | No existe Artista con UserId | `"2016"` | `NotFound_Artista` | 404 |
| 3 - Programa | No existe PromoPrograma | `"2019"` | `NotFound_PromoPrograma` | 404 |
| 4 - Propiedad | Artista no es propietario | `"4026"` | `BusinessRule_NoEsPropietarioPrograma` | 403 |
| 5 - Metricas | Excepcion no controlada | `"5000"` | `Internal_UnexpectedError` | 500 |
| Exito | Metricas calculadas | `"0000"` | `Success` | 200 |

### GetPromotorMetricasQueryHandler

| Paso | Condicion | ErrorCode | Constante | HTTP (Controller) |
|------|-----------|-----------|-----------|-------------------|
| 1 - Validacion | UserId vacio | `"1001"` | `Validation_Required` | 400 |
| 1 - Validacion | FechaDesde > FechaHasta | `"1036"` | `Validation_FechaRangoInvalido` | 400 |
| 2 - Promotor | No existe Promotor con UserId | `"2015"` | `NotFound_Promotor` | 404 |
| 4 - Metricas | Excepcion no controlada | `"5000"` | `Internal_UnexpectedError` | 500 |
| Exito | Metricas calculadas | `"0000"` | `Success` | 200 |

---

## 11. Contexto de las Nuevas Constantes en ServiceResponseMessageType

Los Handlers y Validators de esta feature usan 5 constantes nuevas que deben agregarse al archivo:

**Archivo a modificar:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

```
// Tracking - US-CP-05 (Validacion 1000-1999)
Validation_TipoEventoInvalido      = "1033"
Validation_BackingNoPermitido      = "1034"
Validation_ValorMonetarioInvalido  = "1035"
Validation_FechaRangoInvalido      = "1036"

// Tracking Rate Limiting - US-CP-05 (Business Rules 4000-4999)
BusinessRule_RateLimitExcedido     = "4032"
```

Estas constantes ya estan documentadas en `hexagonal-architecture.md` Seccion 3.3 y en `api-contracts.md` Seccion 5. El implementador debe agregarlas ANTES de implementar los Validators y Handlers para que el compilador valide el uso correcto.

---

## 12. Checklist de Implementacion

### Commands

- [ ] `RegistrarEventoCommand.cs` creado con Command + Handler en el MISMO archivo
- [ ] `RegistrarEventoCommandHandler` constructor con `?? throw` para TODAS las dependencias (4 dependencias)
- [ ] Handler verifica rate limit ANTES de la persistencia (solo para TipoEventoPromoId == 1)
- [ ] Handler registra el rate limit DESPUES del insert exitoso (solo para TipoEventoPromoId == 1)
- [ ] Handler NO inyecta DbContext ni IPromoEventoRepository directamente
- [ ] `RegistrarConversionCommand.cs` creado con Command + Handler en el MISMO archivo
- [ ] `RegistrarConversionCommandHandler` constructor con `?? throw` para TODAS las dependencias (3 dependencias)
- [ ] Handler delega la atomicidad completamente al `IPromoEventoService.RegistrarConversionAsync`
- [ ] Handler captura excepcion en try-catch y retorna `Internal_UnexpectedError` sin estado inconsistente

### Queries

- [ ] `GetProgramaMetricasQuery.cs` creado con Query + Handler en el MISMO archivo
- [ ] `GetProgramaMetricasQueryHandler` constructor con `?? throw` para TODAS las dependencias (3 dependencias - NO IMapper)
- [ ] Handler resuelve Artista, verifica Programa y verifica propiedad ANTES de llamar al Service
- [ ] Handler calcula defaults de fechas si FechaDesde o FechaHasta son null
- [ ] `GetPromotorMetricasQuery.cs` creado con Query + Handler en el MISMO archivo
- [ ] `GetPromotorMetricasQueryHandler` constructor con `?? throw` para TODAS las dependencias (3 dependencias - NO IMapper)
- [ ] Handler resuelve Promotor ANTES de llamar al Service

### Validators

- [ ] Todos los validators usan `using WePlayRises.Crowdpromotion.Domain.Constants;`
- [ ] Todas las reglas tienen `WithMessage("...")` Y `WithErrorCode(ServiceResponseMessageType.X)` (NO strings literales)
- [ ] `RegistrarEventoCommandValidator` tiene las 3 reglas de `TipoEventoPromoId` en orden correcto
- [ ] `RegistrarEventoCommandValidator` usa `When(x => !string.IsNullOrEmpty(x.Campo))` para campos URL/UTM opcionales
- [ ] `GetProgramaMetricasQueryValidator` y `GetPromotorMetricasQueryValidator` usan `.Must((query, fecha) => ...)` para validacion cross-field de fechas
- [ ] Ningún Validator inyecta DbContext ni accede a BD

### DTOs

- [ ] 9 archivos de DTO creados en la carpeta `Dtos/`
- [ ] `ProgramaMetricasResponseDto` tiene `IReadOnlyList<T>` para listas (no `List<T>`)
- [ ] `PromotorMetricasResponseDto` tiene `ProgramaId: Guid?` y `ProgramaTitulo: string?` (nullable, no required)
- [ ] `EventoRecienteDto` tiene `ComisionGenerada: decimal?` (nullable - puede no haber comision)

### AutoMapper Profile

- [ ] `PromoEventoProfile.cs` creado con 2 mappings: RegistrarEventoCommand->PromoEvento y RegistrarConversionCommand->PromoEvento
- [ ] `ForMember(dest => dest.TipoEventoId, opt => opt.MapFrom(src => src.TipoEventoPromoId))` en mapping de RegistrarEventoCommand
- [ ] `ForMember(dest => dest.ImporteAsociado, opt => opt.MapFrom(src => src.ValorMonetario))` en mapping de RegistrarConversionCommand
- [ ] `ForMember(dest => dest.TipoEventoId, opt => opt.Ignore())` en mapping de RegistrarConversionCommand (fijado en Service a 4)
- [ ] Todos los campos no mapeados (ProgramaId, PromotorId, FechaCreacion, navigation properties) tienen `Ignore()`
- [ ] Los DTOs de metricas agregadas NO tienen mapping AutoMapper (construidos en el Service)

### Constantes

- [ ] 5 nuevas constantes agregadas a `ServiceResponseMessageType.cs` ANTES de implementar Validators y Handlers
- [ ] Verificar que `NotFound_Promotor ("2015")`, `NotFound_Artista ("2016")`, `NotFound_PromoPrograma ("2019")` y `BusinessRule_NoEsPropietarioPrograma ("4026")` existen en el archivo

### Registro DI

- [ ] `IValidator<RegistrarEventoCommand>` registrado (FluentValidation lo hace via `AddFluentValidation` o `AddValidatorsFromAssembly`)
- [ ] `IValidator<RegistrarConversionCommand>` registrado
- [ ] `IValidator<GetProgramaMetricasQuery>` registrado
- [ ] `IValidator<GetPromotorMetricasQuery>` registrado
- [ ] `PromoEventoProfile` registrado en AutoMapper via `AddProfile<PromoEventoProfile>()` o `AddAutoMapper(assembly)`
- [ ] MediatR reconoce los 4 Handlers via `AddMediatR(assembly)` del modulo Crowdpromotion

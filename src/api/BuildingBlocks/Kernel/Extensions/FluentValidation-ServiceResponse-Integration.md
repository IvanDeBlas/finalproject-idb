# Integración FluentValidation con ServiceResponse

## Resumen

Se ha mejorado la integración entre FluentValidation y nuestro sistema `ServiceResponse` para proporcionar información detallada de errores tanto para la UI como para debugging interno.

## Estructura Mejorada de ServiceResponseMessage

### Propiedades

| Propiedad | Tipo | Descripción | Ejemplo |
|-----------|------|-------------|---------|
| `Message` | `string` | Mensaje amigable para mostrar al usuario en la UI | "Id is required" |
| `Type` | `string` | Tipo de mensaje para determinar el código HTTP | "Error", "Warning", "Info", "Forbidden" |
| `ErrorCode` | `string?` | Código de error interno específico de FluentValidation | "NotEmptyValidator", "AsyncPredicateValidator" |
| `PropertyName` | `string?` | Campo que generó el error | "Id", "Name", "Email" |

### Ejemplo de Respuesta JSON

```json
{
  "data": {
    "data": null,
    "messages": [
      {
        "message": "Id is required",
        "type": "Error",
        "errorCode": "NotEmptyValidator",
        "propertyName": "Id"
      },
      {
        "message": "Instalacion doesn't exist with id: 0.",
        "type": "Error",
        "errorCode": "AsyncPredicateValidator",
        "propertyName": "Id"
      }
    ]
  },
  "messages": null
}
```

## Mapeo de Códigos HTTP

El sistema `BaseLoggerController.ProcessServiceResponse()` ahora analiza correctamente los tipos de mensaje:

- **Type = "Error"** → `400 BadRequest`
- **Type = "Forbidden"** → `403 Forbidden`
- **Type = "Warning"** → `200 OK` (con warnings)
- **Type = "Info"** → `200 OK`

## Beneficios

### Para la UI
- **Mensajes amigables**: Campo `Message` contiene texto legible para el usuario
- **Campos específicos**: Campo `PropertyName` permite destacar campos con errores
- **Códigos HTTP apropiados**: Respuestas con código de estado correcto

### Para Debugging
- **Trazabilidad**: Campo `ErrorCode` identifica exactamente qué validador falló
- **Análisis técnico**: Facilita la localización de problemas específicos
- **Logging mejorado**: Los logs contienen información detallada del error

## Ejemplos de Uso

### Validación Simple
```csharp
// ValidationResult de FluentValidation
var validationResult = await validator.ValidateAsync(request);
if (!validationResult.IsValid)
{
    return new ServiceResponse<T>
    {
        Messages = validationResult.GetServiceResponseMessages()
    };
}
```

### Respuesta Generada
```json
{
  "message": "Name is required",
  "type": "Error",
  "errorCode": "NotEmptyValidator",
  "propertyName": "Name"
}
```

### Validación Personalizada
```csharp
// En un validator personalizado
RuleFor(x => x.Id)
    .MustAsync(async (id, cancellation) => await ExistsAsync(id))
    .WithMessage("Entity doesn't exist with id: {PropertyValue}");
```

### Respuesta Generada
```json
{
  "message": "Entity doesn't exist with id: 123",
  "type": "Error", 
  "errorCode": "AsyncPredicateValidator",
  "propertyName": "Id"
}
```

## Comportamiento Anterior vs Nuevo

### Antes
- ❌ Códigos HTTP siempre 200 OK para errores de validación
- ❌ Solo nombres técnicos de validadores en `Type`
- ❌ Información limitada para debugging

### Después
- ✅ Códigos HTTP correctos (400 BadRequest para errores)
- ✅ Tipo "Error" estándar + código específico en `ErrorCode`
- ✅ Información completa: mensaje, tipo, código y campo
- ✅ Compatible con UI y debugging interno

## Migración

No se requiere cambios en el código existente. La mejora es **totalmente retrocompatible**.

Los endpoints que ya usan:
```csharp
return await HandleRequestAsync(async () => 
    await _mediator.Send(new Query(), cancellationToken));
```

Automáticamente se beneficiarán de las mejoras sin modificaciones.

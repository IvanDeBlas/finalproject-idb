# ADR-002: CQRS con MediatR para Separacion de Comandos y Queries

## Metadata
- **Estado**: Aceptada
- **Fecha**: 2026-01-21
- **Relacionado con**: ADR-001
- **Basado en**: miUrba ADR-002

---

## Contexto

Necesitamos organizar la logica de aplicacion de forma que:
- Separe operaciones de lectura (queries) de escritura (commands)
- Mantenga handlers desacoplados del resto del sistema
- Facilite testing unitario
- Permita aplicar cross-cutting concerns (logging, validation)

---

## Decision

Implementar **CQRS (Command Query Responsibility Segregation)** usando **MediatR** como mediator pattern.

**Convencion importante:** Command/Query, Handler y Result van en el **mismo archivo**.

**Estructura:**

```
Application/Features/
+-- Campanias/
|   +-- Commands/
|   |   +-- CreateCampania.cs        # Command + Handler + Result
|   +-- Queries/
|   |   +-- GetCampaniaById.cs       # Query + Handler + Result
|   +-- Validators/
|       +-- CreateCampaniaValidator.cs
```

---

## Ejemplo de Implementacion

**CreateCampania.cs (Command + Handler en mismo archivo):**

```csharp
// Command
public record CreateCampaniaCommand(
    Guid ArtistaId,
    string Titulo,
    string Descripcion,
    decimal ImporteObjetivo
) : IRequest<CreateCampaniaResult>;

// Result
public record CreateCampaniaResult(Guid CampaniaId, string Titulo);

// Handler
public class CreateCampaniaHandler : IRequestHandler<CreateCampaniaCommand, CreateCampaniaResult>
{
    private readonly ICampaniaService _campaniaService;

    public CreateCampaniaHandler(ICampaniaService campaniaService)
    {
        _campaniaService = campaniaService;
    }

    public async Task<CreateCampaniaResult> Handle(
        CreateCampaniaCommand request,
        CancellationToken cancellationToken)
    {
        var campania = await _campaniaService.CreateAsync(
            request.ArtistaId,
            request.Titulo,
            request.Descripcion,
            request.ImporteObjetivo,
            cancellationToken);

        return new CreateCampaniaResult(campania.Id, campania.Titulo);
    }
}
```

**Regla critica:** Handler NUNCA inyecta DbContext directamente.
- Handler -> Service -> Repository -> DbContext

---

## Justificacion

- CQRS permite optimizar lecturas y escrituras de forma independiente
- MediatR proporciona desacoplamiento excelente
- Facilita vertical slice architecture (features autocontenidos)
- Consistente con patron de miUrba

---

## Alternativas Consideradas

### Alternativa 1: CQRS sin MediatR (manual)
- **Descripcion**: Implementar mediator pattern manualmente
- **Razon para no elegirla**: Boilerplate excesivo

### Alternativa 2: Controllers con logica directa
- **Descripcion**: Logica de negocio directamente en controllers
- **Razon para no elegirla**: Testabilidad pobre

### Alternativa 3: Traditional Service Layer
- **Descripcion**: Servicios con metodos CRUD
- **Razon para no elegirla**: Menos estructurado, dificil aplicar validaciones

---

## Consecuencias

### Positivas
- Controllers delgados (solo envian commands/queries)
- Handlers faciles de testear aisladamente
- Codigo organizado por features, no por capas tecnicas
- Validators con FluentValidation integrados

### Negativas
- Dependencia de libreria externa (MediatR)
- Mas archivos (un handler por operacion)
- Abstraccion adicional puede confundir inicialmente

### Riesgos
- **Overuse de MediatR** para cosas triviales -> Usar solo para casos de uso reales
- **Handlers muy grandes** -> Extraer logica a domain services

---

## Notas de Implementacion

**Registro en Program.cs:**

```csharp
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

**Validators siempre con .WithMessage() Y .WithErrorCode():**

```csharp
public class CreateCampaniaValidator : AbstractValidator<CreateCampaniaCommand>
{
    public CreateCampaniaValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El titulo es obligatorio")
            .WithErrorCode("CAMPANIA_TITULO_REQUIRED");

        RuleFor(x => x.ImporteObjetivo)
            .GreaterThan(0)
            .WithMessage("El importe objetivo debe ser mayor a 0")
            .WithErrorCode("CAMPANIA_IMPORTE_INVALID");
    }
}
```

---

## Referencias

- [CQRS Pattern - Martin Fowler](https://martinfowler.com/bliki/CQRS.html)
- [MediatR GitHub](https://github.com/jbogard/MediatR)
- [Vertical Slice Architecture - Jimmy Bogard](https://jimmybogard.com/vertical-slice-architecture/)

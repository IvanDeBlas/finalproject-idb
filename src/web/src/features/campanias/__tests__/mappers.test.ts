import { describe, it, expect } from "vitest"
import {
    mapCampaniaDtoToDomain,
    mapCampaniaListItemDtoToDomain,
    mapRewardDtoToDomain,
    mapCreateCampaniaToDto,
    mapUpdateCampaniaToDto,
} from "../infrastructure/mappers"
import type { CampaniaDto, CampaniaListItemDto, RewardApiDto } from "../infrastructure/dtos"
import type { CreateCampaniaData, UpdateCampaniaData } from "../domain/types"

function buildCampaniaDto(overrides: Partial<CampaniaDto> = {}): CampaniaDto {
    return {
        id: "camp-001",
        artistaId: "art-001",
        titulo: "Mi Primera Campania",
        descripcion: "Descripcion de la campania",
        importeObjetivo: 5000,
        importePledgedActual: 1250,
        estadoCampaniaId: 2,
        monedaId: 1,
        tipoFinanciacionId: 1,
        permiteAportacionesAnonimas: false,
        permitePropinas: true,
        backersCount: 15,
        fechaCreacion: "2026-01-01T00:00:00Z",
        fechaFin: "2026-06-01T00:00:00Z",
        ...overrides,
    }
}

function buildCampaniaListItemDto(overrides: Partial<CampaniaListItemDto> = {}): CampaniaListItemDto {
    return {
        id: "camp-001",
        artistaId: "art-001",
        titulo: "Campania en Lista",
        importeObjetivo: 3000,
        importePledgedActual: 500,
        estadoCampaniaId: 1,
        monedaId: 1,
        backersCount: 5,
        fechaCreacion: "2026-02-01T00:00:00Z",
        ...overrides,
    }
}

function buildRewardApiDto(overrides: Partial<RewardApiDto> = {}): RewardApiDto {
    return {
        id: "reward-001",
        campaniaId: "camp-001",
        nombre: "Reward Basico",
        importeMinimo: 10,
        cantidadReclamada: 3,
        ...overrides,
    }
}

describe("mapCampaniaDtoToDomain", () => {
    it("maps basic fields correctly", () => {
        // Arrange
        const dto = buildCampaniaDto()

        // Act
        const result = mapCampaniaDtoToDomain(dto)

        // Assert
        expect(result.id).toBe("camp-001")
        expect(result.artistaId).toBe("art-001")
        expect(result.titulo).toBe("Mi Primera Campania")
        expect(result.descripcion).toBe("Descripcion de la campania")
        expect(result.importeObjetivo).toBe(5000)
        expect(result.importePledgedActual).toBe(1250)
        expect(result.estadoCampaniaId).toBe(2)
        expect(result.permiteAportacionesAnonimas).toBe(false)
        expect(result.permitePropinas).toBe(true)
        expect(result.backersCount).toBe(15)
    })

    it("sets default monedaId to 1 when missing", () => {
        // Arrange
        const dto = buildCampaniaDto({ monedaId: undefined as unknown as number })

        // Act
        const result = mapCampaniaDtoToDomain(dto)

        // Assert
        expect(result.monedaId).toBe(1)
    })

    it("sets default importePledgedActual to 0 when missing", () => {
        // Arrange
        const dto = buildCampaniaDto({ importePledgedActual: undefined as unknown as number })

        // Act
        const result = mapCampaniaDtoToDomain(dto)

        // Assert
        expect(result.importePledgedActual).toBe(0)
    })

    it("maps legacy imagenUrl to imagenPrincipalUrl", () => {
        // Arrange
        const dto = buildCampaniaDto({
            imagenPrincipalUrl: undefined,
            imagenUrl: "https://example.com/legacy-image.jpg",
        })

        // Act
        const result = mapCampaniaDtoToDomain(dto)

        // Assert
        expect(result.imagenPrincipalUrl).toBe("https://example.com/legacy-image.jpg")
        expect(result.imagenUrl).toBe("https://example.com/legacy-image.jpg")
    })

    it("prefers imagenPrincipalUrl over legacy imagenUrl", () => {
        // Arrange
        const dto = buildCampaniaDto({
            imagenPrincipalUrl: "https://example.com/new-image.jpg",
            imagenUrl: "https://example.com/legacy-image.jpg",
        })

        // Act
        const result = mapCampaniaDtoToDomain(dto)

        // Assert
        expect(result.imagenPrincipalUrl).toBe("https://example.com/new-image.jpg")
    })

    it("creates Date objects for fechaInicioDate and fechaFinDate", () => {
        // Arrange
        const dto = buildCampaniaDto({
            fechaInicio: "2026-03-01T00:00:00Z",
            fechaFin: "2026-06-01T00:00:00Z",
        })

        // Act
        const result = mapCampaniaDtoToDomain(dto)

        // Assert
        expect(result.fechaInicioDate).toBeInstanceOf(Date)
        expect(result.fechaFinDate).toBeInstanceOf(Date)
        expect(result.fechaInicioDate.toISOString()).toBe("2026-03-01T00:00:00.000Z")
        expect(result.fechaFinDate.toISOString()).toBe("2026-06-01T00:00:00.000Z")
    })

    it("creates createdAt and updatedAt as Date objects", () => {
        // Arrange
        const dto = buildCampaniaDto({
            fechaCreacion: "2026-01-15T10:30:00Z",
            fechaActualizacion: "2026-02-01T14:00:00Z",
        })

        // Act
        const result = mapCampaniaDtoToDomain(dto)

        // Assert
        expect(result.createdAt).toBeInstanceOf(Date)
        expect(result.updatedAt).toBeInstanceOf(Date)
    })

    it("maps estadoCampaniaId to estado string", () => {
        // Arrange
        const dto = buildCampaniaDto({ estadoCampaniaId: 2, estado: undefined })

        // Act
        const result = mapCampaniaDtoToDomain(dto)

        // Assert
        expect(result.estado).toBe("activa")
    })

    it("maps default estadoCampaniaId (borrador) when missing", () => {
        // Arrange
        const dto = buildCampaniaDto({
            estadoCampaniaId: undefined as unknown as number,
            estado: undefined,
        })

        // Act
        const result = mapCampaniaDtoToDomain(dto)

        // Assert
        expect(result.estadoCampaniaId).toBe(1)
        expect(result.estado).toBe("borrador")
    })
})

describe("mapCampaniaListItemDtoToDomain", () => {
    it("maps basic fields correctly", () => {
        // Arrange
        const dto = buildCampaniaListItemDto()

        // Act
        const result = mapCampaniaListItemDtoToDomain(dto)

        // Assert
        expect(result.id).toBe("camp-001")
        expect(result.artistaId).toBe("art-001")
        expect(result.titulo).toBe("Campania en Lista")
        expect(result.importeObjetivo).toBe(3000)
        expect(result.importePledgedActual).toBe(500)
        expect(result.estadoCampaniaId).toBe(1)
        expect(result.backersCount).toBe(5)
    })

    it("provides default values for optional fields", () => {
        // Arrange
        const dto = buildCampaniaListItemDto({
            descripcionCorta: undefined,
            descripcion: undefined,
            importePledgedActual: undefined as unknown as number,
            backersCount: undefined as unknown as number,
            monedaId: undefined as unknown as number,
        })

        // Act
        const result = mapCampaniaListItemDtoToDomain(dto)

        // Assert
        expect(result.descripcionCorta).toBe("")
        expect(result.descripcion).toBe("")
        expect(result.importePledgedActual).toBe(0)
        expect(result.backersCount).toBe(0)
        expect(result.monedaId).toBe(1)
    })

    it("maps legacy imagenUrl when imagenPrincipalUrl is missing", () => {
        // Arrange
        const dto = buildCampaniaListItemDto({
            imagenPrincipalUrl: undefined,
            imagenUrl: "https://example.com/old.jpg",
        })

        // Act
        const result = mapCampaniaListItemDtoToDomain(dto)

        // Assert
        expect(result.imagenPrincipalUrl).toBe("https://example.com/old.jpg")
        expect(result.imagenUrl).toBe("https://example.com/old.jpg")
    })

    it("uses descripcionCorta over descripcion", () => {
        // Arrange
        const dto = buildCampaniaListItemDto({
            descripcionCorta: "Corta",
            descripcion: "Larga descripcion",
        })

        // Act
        const result = mapCampaniaListItemDtoToDomain(dto)

        // Assert
        expect(result.descripcionCorta).toBe("Corta")
        expect(result.descripcion).toBe("Corta")
    })
})

describe("mapRewardDtoToDomain", () => {
    it("maps basic reward fields correctly", () => {
        // Arrange
        const dto = buildRewardApiDto()

        // Act
        const result = mapRewardDtoToDomain(dto)

        // Assert
        expect(result.id).toBe("reward-001")
        expect(result.campaniaId).toBe("camp-001")
        expect(result.nombre).toBe("Reward Basico")
        expect(result.importeMinimo).toBe(10)
        expect(result.cantidadReclamada).toBe(3)
    })

    it("maps optional fields when present", () => {
        // Arrange
        const dto = buildRewardApiDto({
            descripcion: "Una descripcion del reward",
            cantidadDisponible: 100,
            fechaEntregaEstimada: "2026-12-01",
        })

        // Act
        const result = mapRewardDtoToDomain(dto)

        // Assert
        expect(result.descripcion).toBe("Una descripcion del reward")
        expect(result.cantidadDisponible).toBe(100)
        expect(result.fechaEntregaEstimada).toBe("2026-12-01")
    })

    it("falls back to stockLimitado for cantidadDisponible", () => {
        // Arrange
        const dto = buildRewardApiDto({
            cantidadDisponible: undefined,
            stockLimitado: 50,
        })

        // Act
        const result = mapRewardDtoToDomain(dto)

        // Assert
        expect(result.cantidadDisponible).toBe(50)
    })

    it("defaults cantidadReclamada to 0 when missing", () => {
        // Arrange
        const dto = buildRewardApiDto({
            cantidadReclamada: undefined as unknown as number,
        })

        // Act
        const result = mapRewardDtoToDomain(dto)

        // Assert
        expect(result.cantidadReclamada).toBe(0)
    })
})

describe("mapCreateCampaniaToDto", () => {
    it("converts Date to ISO string for fechaFin", () => {
        // Arrange
        const data: CreateCampaniaData = {
            titulo: "Nueva Campania",
            descripcion: "Descripcion nueva",
            importeObjetivo: 10000,
            fechaFin: new Date("2026-12-31T23:59:59Z"),
        }

        // Act
        const result = mapCreateCampaniaToDto(data)

        // Assert
        expect(result.fechaFin).toBe("2026-12-31T23:59:59.000Z")
    })

    it("includes all required fields", () => {
        // Arrange
        const data: CreateCampaniaData = {
            titulo: "Campania Completa",
            descripcion: "Con todos los campos",
            importeObjetivo: 5000,
            fechaFin: new Date("2026-06-01T00:00:00Z"),
            imagenUrl: "https://example.com/img.jpg",
        }

        // Act
        const result = mapCreateCampaniaToDto(data)

        // Assert
        expect(result.titulo).toBe("Campania Completa")
        expect(result.descripcion).toBe("Con todos los campos")
        expect(result.importeObjetivo).toBe(5000)
        expect(result.fechaFin).toBe("2026-06-01T00:00:00.000Z")
        expect(result.imagenUrl).toBe("https://example.com/img.jpg")
    })

    it("omits imagenUrl when not provided", () => {
        // Arrange
        const data: CreateCampaniaData = {
            titulo: "Sin Imagen",
            descripcion: "Sin imagen url",
            importeObjetivo: 2000,
            fechaFin: new Date("2026-09-01T00:00:00Z"),
        }

        // Act
        const result = mapCreateCampaniaToDto(data)

        // Assert
        expect(result.imagenUrl).toBeUndefined()
    })
})

describe("mapUpdateCampaniaToDto", () => {
    it("handles partial data with only title provided", () => {
        // Arrange
        const data: UpdateCampaniaData = {
            titulo: "Titulo Actualizado",
        }

        // Act
        const result = mapUpdateCampaniaToDto(data)

        // Assert
        expect(result.titulo).toBe("Titulo Actualizado")
        expect(result.descripcion).toBeUndefined()
        expect(result.importeObjetivo).toBeUndefined()
        expect(result.fechaFin).toBeUndefined()
        expect(result.imagenUrl).toBeUndefined()
    })

    it("converts fechaFin Date to ISO string when present", () => {
        // Arrange
        const data: UpdateCampaniaData = {
            titulo: "Actualizada",
            fechaFin: new Date("2026-12-31T00:00:00Z"),
        }

        // Act
        const result = mapUpdateCampaniaToDto(data)

        // Assert
        expect(result.fechaFin).toBe("2026-12-31T00:00:00.000Z")
    })

    it("maps all provided fields correctly", () => {
        // Arrange
        const data: UpdateCampaniaData = {
            titulo: "Titulo Nuevo",
            descripcion: "Descripcion Nueva",
            importeObjetivo: 8000,
            fechaFin: new Date("2027-01-15T00:00:00Z"),
            imagenUrl: "https://example.com/updated.jpg",
        }

        // Act
        const result = mapUpdateCampaniaToDto(data)

        // Assert
        expect(result.titulo).toBe("Titulo Nuevo")
        expect(result.descripcion).toBe("Descripcion Nueva")
        expect(result.importeObjetivo).toBe(8000)
        expect(result.fechaFin).toBe("2027-01-15T00:00:00.000Z")
        expect(result.imagenUrl).toBe("https://example.com/updated.jpg")
    })

    it("does not convert fechaFin when it is undefined", () => {
        // Arrange
        const data: UpdateCampaniaData = {
            descripcion: "Solo descripcion",
        }

        // Act
        const result = mapUpdateCampaniaToDto(data)

        // Assert
        expect(result.fechaFin).toBeUndefined()
    })
})

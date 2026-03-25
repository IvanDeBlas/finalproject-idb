import { describe, it, expect, vi, beforeEach } from "vitest"
import { artistaService } from "../infrastructure/artista.service"

// Mock the api-client
const mockApiFetch = vi.fn()
vi.mock("@/lib/api-client", () => ({
    apiFetch: (...args: unknown[]) => mockApiFetch(...args),
}))

const mockArtistaDto = {
    id: "artista-001",
    userId: "user-123",
    nombreArtistico: "Test Artist",
    descripcion: "Rock band",
    pais: "Espana",
    ciudad: "Madrid",
    imagenUrl: "https://example.com/photo.jpg",
    generoMusical: "Rock",
    createdAt: "2026-01-15T10:00:00Z",
    updatedAt: "2026-02-20T15:30:00Z",
}

describe("ArtistaService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe("getById", () => {
        it("calls GET /artistas/:id", async () => {
            mockApiFetch.mockResolvedValue({
                data: mockArtistaDto,
                messages: [],
            })

            await artistaService.getById("artista-001")

            expect(mockApiFetch).toHaveBeenCalledWith("/artistas/artista-001")
        })

        it("maps DTO dates to Date objects", async () => {
            mockApiFetch.mockResolvedValue({
                data: mockArtistaDto,
                messages: [],
            })

            const result = await artistaService.getById("artista-001")

            expect(result.id).toBe("artista-001")
            expect(result.nombreArtistico).toBe("Test Artist")
            expect(result.createdAt).toBeInstanceOf(Date)
            expect(result.updatedAt).toBeInstanceOf(Date)
        })

        it("propagates API errors", async () => {
            mockApiFetch.mockRejectedValue(new Error("Not found"))

            await expect(
                artistaService.getById("non-existent")
            ).rejects.toThrow("Not found")
        })
    })

    describe("getMyProfile", () => {
        it("calls GET /artistas/me", async () => {
            mockApiFetch.mockResolvedValue({
                data: mockArtistaDto,
                messages: [],
            })

            await artistaService.getMyProfile()

            expect(mockApiFetch).toHaveBeenCalledWith("/artistas/me")
        })

        it("maps DTO to domain model", async () => {
            mockApiFetch.mockResolvedValue({
                data: mockArtistaDto,
                messages: [],
            })

            const result = await artistaService.getMyProfile()

            expect(result).not.toBeNull()
            expect(result?.nombreArtistico).toBe("Test Artist")
            expect(result?.createdAt).toBeInstanceOf(Date)
        })

        it("returns null on error", async () => {
            mockApiFetch.mockRejectedValue(new Error("Unauthorized"))

            const result = await artistaService.getMyProfile()

            expect(result).toBeNull()
        })
    })

    describe("create", () => {
        it("calls POST /artistas with data", async () => {
            mockApiFetch.mockResolvedValue({
                data: mockArtistaDto,
                messages: [],
            })

            const createData = {
                nombreArtistico: "New Artist",
                descripcion: "A new artist",
                generoMusical: "Pop",
            }

            await artistaService.create(createData)

            expect(mockApiFetch).toHaveBeenCalledWith("/artistas", {
                method: "POST",
                data: createData,
            })
        })

        it("returns mapped domain model", async () => {
            mockApiFetch.mockResolvedValue({
                data: mockArtistaDto,
                messages: [],
            })

            const result = await artistaService.create({
                nombreArtistico: "Test",
            })

            expect(result.id).toBe("artista-001")
            expect(result.createdAt).toBeInstanceOf(Date)
        })

        it("propagates API errors", async () => {
            mockApiFetch.mockRejectedValue(new Error("Validation error"))

            await expect(
                artistaService.create({ nombreArtistico: "" })
            ).rejects.toThrow("Validation error")
        })
    })

    describe("update", () => {
        it("calls PUT /artistas/:id with data", async () => {
            mockApiFetch.mockResolvedValue({
                data: { ...mockArtistaDto, nombreArtistico: "Updated" },
                messages: [],
            })

            const updateData = { nombreArtistico: "Updated" }

            await artistaService.update("artista-001", updateData)

            expect(mockApiFetch).toHaveBeenCalledWith("/artistas/artista-001", {
                method: "PUT",
                data: updateData,
            })
        })

        it("returns updated domain model", async () => {
            mockApiFetch.mockResolvedValue({
                data: { ...mockArtistaDto, nombreArtistico: "Updated Name" },
                messages: [],
            })

            const result = await artistaService.update("artista-001", {
                nombreArtistico: "Updated Name",
            })

            expect(result.nombreArtistico).toBe("Updated Name")
            expect(result.createdAt).toBeInstanceOf(Date)
        })

        it("propagates API errors", async () => {
            mockApiFetch.mockRejectedValue(new Error("Not found"))

            await expect(
                artistaService.update("bad-id", { nombreArtistico: "Test" })
            ).rejects.toThrow("Not found")
        })
    })
})

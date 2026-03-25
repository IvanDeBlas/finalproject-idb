import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@/test-utils"
import PerfilPage from "../page"

const mockMutateAsync = vi.fn()

vi.mock("@/hooks/use-artista", () => ({
    useMyArtistProfile: vi.fn(),
    useCreateArtista: () => ({
        mutateAsync: mockMutateAsync,
        isPending: false,
    }),
    useUpdateArtista: () => ({
        mutateAsync: mockMutateAsync,
        isPending: false,
    }),
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

vi.mock("@/components/artistas/artista-form", () => ({
    ArtistaForm: ({
        onSubmit,
        isSubmitting,
        defaultValues,
    }: {
        onSubmit: (data: Record<string, string>) => void
        isSubmitting: boolean
        defaultValues?: Record<string, string>
    }) => (
        <div data-testid="artista-form">
            <span data-testid="is-submitting">
                {isSubmitting.toString()}
            </span>
            {defaultValues && (
                <span data-testid="default-values">
                    {JSON.stringify(defaultValues)}
                </span>
            )}
            <button
                data-testid="submit-btn"
                onClick={() =>
                    onSubmit({ nombreArtistico: "Test Artist" })
                }
            >
                Submit
            </button>
        </div>
    ),
}))

const { useMyArtistProfile } = await import("@/hooks/use-artista")
const { toast } = await import("sonner")

describe("PerfilPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("shows loading skeleton when data is loading", () => {
        vi.mocked(useMyArtistProfile).mockReturnValue({
            data: undefined,
            isLoading: true,
        } as ReturnType<typeof useMyArtistProfile>)

        render(<PerfilPage />)

        // Skeleton renders animated placeholder elements, no form
        expect(screen.queryByTestId("artista-form")).not.toBeInTheDocument()
    })

    it("shows 'Crear Perfil de Artista' when no artista profile exists", () => {
        vi.mocked(useMyArtistProfile).mockReturnValue({
            data: undefined,
            isLoading: false,
        } as ReturnType<typeof useMyArtistProfile>)

        render(<PerfilPage />)

        expect(
            screen.getByText("Crear Perfil de Artista")
        ).toBeInTheDocument()
        expect(
            screen.getByText(
                "Completa tu perfil para poder crear campanias"
            )
        ).toBeInTheDocument()
    })

    it("shows 'Editar Perfil' when artista profile exists", () => {
        vi.mocked(useMyArtistProfile).mockReturnValue({
            data: {
                id: "artista-1",
                nombreArtistico: "Test Artist",
                descripcion: "Desc",
                imagenUrl: "http://img.com/pic.jpg",
                generoMusical: "Rock",
            },
            isLoading: false,
        } as ReturnType<typeof useMyArtistProfile>)

        render(<PerfilPage />)

        expect(screen.getByText("Editar Perfil")).toBeInTheDocument()
        expect(
            screen.getByText(
                "Actualiza la informacion de tu perfil artistico"
            )
        ).toBeInTheDocument()
    })

    it("passes defaultValues to ArtistaForm when artista exists", () => {
        vi.mocked(useMyArtistProfile).mockReturnValue({
            data: {
                id: "artista-1",
                nombreArtistico: "Test Artist",
                descripcion: "A description",
                imagenUrl: "http://img.com/pic.jpg",
                generoMusical: "Rock",
            },
            isLoading: false,
        } as ReturnType<typeof useMyArtistProfile>)

        render(<PerfilPage />)

        const defaultValues = screen.getByTestId("default-values")
        const parsed = JSON.parse(defaultValues.textContent || "{}")
        expect(parsed.nombreArtistico).toBe("Test Artist")
        expect(parsed.descripcion).toBe("A description")
        expect(parsed.generoMusical).toBe("Rock")
    })

    it("calls createMutation when submitting without existing artista", async () => {
        vi.mocked(useMyArtistProfile).mockReturnValue({
            data: undefined,
            isLoading: false,
        } as ReturnType<typeof useMyArtistProfile>)

        mockMutateAsync.mockResolvedValue({ id: "new-artista" })

        render(<PerfilPage />)

        const submitBtn = screen.getByTestId("submit-btn")
        submitBtn.click()

        await waitFor(() => {
            expect(mockMutateAsync).toHaveBeenCalledWith({
                nombreArtistico: "Test Artist",
            })
        })

        expect(toast.success).toHaveBeenCalledWith("Perfil creado")
    })

    it("calls updateMutation when submitting with existing artista", async () => {
        vi.mocked(useMyArtistProfile).mockReturnValue({
            data: {
                id: "artista-1",
                nombreArtistico: "Existing",
                descripcion: "",
                imagenUrl: "",
                generoMusical: "",
            },
            isLoading: false,
        } as ReturnType<typeof useMyArtistProfile>)

        mockMutateAsync.mockResolvedValue({ id: "artista-1" })

        render(<PerfilPage />)

        const submitBtn = screen.getByTestId("submit-btn")
        submitBtn.click()

        await waitFor(() => {
            expect(mockMutateAsync).toHaveBeenCalledWith({
                id: "artista-1",
                data: { nombreArtistico: "Test Artist" },
            })
        })

        expect(toast.success).toHaveBeenCalledWith("Perfil actualizado")
    })

    it("shows error toast when mutation fails", async () => {
        vi.mocked(useMyArtistProfile).mockReturnValue({
            data: undefined,
            isLoading: false,
        } as ReturnType<typeof useMyArtistProfile>)

        mockMutateAsync.mockRejectedValue(new Error("Server error"))

        render(<PerfilPage />)

        const submitBtn = screen.getByTestId("submit-btn")
        submitBtn.click()

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalledWith(
                "Error al guardar el perfil"
            )
        })
    })
})

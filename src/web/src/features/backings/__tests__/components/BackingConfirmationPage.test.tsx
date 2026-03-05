import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter, Route, Routes } from "react-router-dom"
import BackingConfirmationPage from "../../presentation/pages/BackingConfirmationPage"

// Mock useCampania hook
vi.mock("@/features/campanias/application/useCampanias", () => ({
    useCampania: vi.fn(() => ({
        data: {
            id: "550e8400-e29b-41d4-a716-446655440000",
            titulo: "Mi Album Debut",
        },
        isLoading: false,
        error: null,
    })),
}))

// Mock auth store
vi.mock("@/store/auth-store", () => ({
    useAuthStore: vi.fn(() => ({
        user: { email: "test@mail.com" },
    })),
}))

function renderPage(path = "/campanias/550e8400-e29b-41d4-a716-446655440000/confirmacion?backingId=abc123") {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false } },
    })

    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter initialEntries={[path]}>
                <Routes>
                    <Route path="/campanias/:id/confirmacion" element={<BackingConfirmationPage />} />
                </Routes>
            </MemoryRouter>
        </QueryClientProvider>
    )
}

describe("BackingConfirmationPage", () => {
    it("renders success message", () => {
        renderPage()

        expect(screen.getByText(/Gracias por tu apoyo/i)).toBeInTheDocument()
    })

    it("shows campania title", () => {
        renderPage()

        expect(screen.getAllByText(/Mi Album Debut/).length).toBeGreaterThan(0)
    })

    it("displays backing ID from search params", () => {
        renderPage()

        expect(screen.getByText("abc123")).toBeInTheDocument()
    })

    it("shows email confirmation message", () => {
        renderPage()

        expect(screen.getByText(/test@mail.com/)).toBeInTheDocument()
    })

    it("shows 'Ver Campana' link", () => {
        renderPage()

        const link = screen.getByRole("link", { name: "Ver Campana" })
        expect(link).toBeInTheDocument()
        expect(link).toHaveAttribute("href", "/campanias/550e8400-e29b-41d4-a716-446655440000")
    })

    it("shows 'Explorar Mas Campanas' button", () => {
        renderPage()

        expect(screen.getByRole("button", { name: "Explorar Mas Campanas" })).toBeInTheDocument()
    })

    it("shows next steps section", () => {
        renderPage()

        expect(screen.getByText(/Que sigue/i)).toBeInTheDocument()
        expect(screen.getByText(/procesara tu recompensa/i)).toBeInTheDocument()
    })

    it("shows date in summary", () => {
        renderPage()

        expect(screen.getByText("Fecha:")).toBeInTheDocument()
    })
})

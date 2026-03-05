import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { CodigoReferidoBlock } from "../../presentation/components/CodigoReferidoBlock"
import { UrlTrackingBlock } from "../../presentation/components/UrlTrackingBlock"
import { TareaItem } from "../../presentation/components/TareaItem"
import { mockMiPrograma_Aprobado, mockTarea } from "../../__mocks__/inscripcion.mock"

vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))

import { toast } from "sonner"

// Setup global clipboard mock for the entire file
const writeTextMock = vi.fn(() => Promise.resolve())

beforeEach(() => {
    writeTextMock.mockClear()
    vi.mocked(toast.success).mockClear()
    vi.mocked(toast.error).mockClear()
})

// Apply clipboard mock once, before all tests
Object.defineProperty(navigator, "clipboard", {
    value: { writeText: writeTextMock },
    writable: true,
    configurable: true,
})

describe("CodigoReferidoBlock", () => {
    it("renders codigoReferido", () => {
        render(<CodigoReferidoBlock codigo={mockMiPrograma_Aprobado.codigoReferido!} />)

        expect(screen.getByText("album-2026-x7k9m")).toBeInTheDocument()
    })

    it("copies codigoReferido to clipboard on button click", async () => {
        const user = userEvent.setup()
        render(<CodigoReferidoBlock codigo="album-2026-x7k9m" />)

        await user.click(screen.getByRole("button", { name: /Copiar codigo referido/i }))

        await waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith("Codigo referido copiado al portapapeles")
        })
    })

    it("shows copy success feedback after copy", async () => {
        const user = userEvent.setup()
        render(<CodigoReferidoBlock codigo="album-2026-x7k9m" />)

        await user.click(screen.getByRole("button", { name: /Copiar codigo referido/i }))

        expect(screen.getByRole("button", { name: /Codigo copiado/i })).toBeInTheDocument()
    })
})

describe("UrlTrackingBlock", () => {
    it("renders urlTrackingPersonalizada", () => {
        render(<UrlTrackingBlock url={mockMiPrograma_Aprobado.urlTrackingPersonalizada!} />)

        expect(screen.getByText(/utm_source=weplay/i)).toBeInTheDocument()
    })

    it("copies urlTracking to clipboard on button click", async () => {
        const user = userEvent.setup()
        render(<UrlTrackingBlock url="https://weplay.com/test" />)

        await user.click(screen.getByRole("button", { name: /Copiar URL de tracking/i }))

        await waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith("URL de tracking copiada al portapapeles")
        })
    })

    it("shows copy success feedback after copy", async () => {
        const user = userEvent.setup()
        render(<UrlTrackingBlock url="https://weplay.com/test" />)

        await user.click(screen.getByRole("button", { name: /Copiar URL de tracking/i }))

        expect(screen.getByRole("button", { name: /URL copiada/i })).toBeInTheDocument()
    })
})

describe("TareaItem", () => {
    it("renders tarea titulo", () => {
        render(<TareaItem tarea={mockTarea} />)

        expect(screen.getByText("Comparte en Instagram Stories")).toBeInTheDocument()
    })

    it("renders tarea tipo badge", () => {
        render(<TareaItem tarea={mockTarea} />)

        expect(screen.getByText("Share")).toBeInTheDocument()
    })

    it("renders recompensa amount", () => {
        render(<TareaItem tarea={mockTarea} />)

        expect(screen.getByText(/5\.00 EUR/)).toBeInTheDocument()
    })

    it("renders repetibilidad info", () => {
        render(<TareaItem tarea={mockTarea} />)

        expect(screen.getByText(/Repetible x10/)).toBeInTheDocument()
    })

    it("renders No repetible when esRepetible is false", () => {
        const tareaNoRepetible = { ...mockTarea, esRepetible: false }
        render(<TareaItem tarea={tareaNoRepetible} />)

        expect(screen.getByText(/No repetible/)).toBeInTheDocument()
    })
})

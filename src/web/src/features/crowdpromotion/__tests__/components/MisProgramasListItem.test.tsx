import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { InscripcionItem } from "../../presentation/components/InscripcionItem"
import {
    mockMiPrograma_Aprobado,
    mockMiPrograma_Pendiente,
    mockMiPrograma_Bloqueado,
    mockMiPrograma_DadoDeBaja,
} from "../../__mocks__/inscripcion.mock"

vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))

describe("InscripcionItem", () => {
    beforeEach(() => {
        Object.defineProperty(navigator, "clipboard", {
            value: { writeText: vi.fn().mockResolvedValue(undefined) },
            writable: true,
            configurable: true,
        })
    })

    it("renders programa titulo and artista", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_Aprobado} />)

        expect(screen.getByText("Promociona mi nuevo album")).toBeInTheDocument()
        expect(screen.getByText("Luna Nova")).toBeInTheDocument()
    })

    it("renders estado badge correcto for Aprobado", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_Aprobado} />)

        expect(screen.getByText("Aprobado")).toBeInTheDocument()
    })

    it("renders estado badge correcto for Pendiente", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_Pendiente} />)

        expect(screen.getByText("Pendiente de aprobacion")).toBeInTheDocument()
    })

    it("renders estado badge correcto for Bloqueado", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_Bloqueado} />)

        expect(screen.getByText("Bloqueado")).toBeInTheDocument()
    })

    it("renders estado badge correcto for DadoDeBaja", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_DadoDeBaja} />)

        expect(screen.getByText("Dado de baja")).toBeInTheDocument()
    })

    it("shows codigoReferido for aprobado inscripcion", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_Aprobado} />)

        expect(screen.getByText("album-2026-x7k9m")).toBeInTheDocument()
    })

    it("does not show codigoReferido for pendiente inscripcion", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_Pendiente} />)

        expect(screen.queryByText("album-2026-x7k9m")).not.toBeInTheDocument()
    })

    it("shows comision info for aprobado", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_Aprobado} />)

        expect(screen.getByText(/10%/)).toBeInTheDocument()
    })

    it("shows pending message for pendiente", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_Pendiente} />)

        expect(screen.getByText(/Esperando aprobacion del artista/i)).toBeInTheDocument()
    })

    it("shows blocked message for bloqueado", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_Bloqueado} />)

        expect(screen.getByText(/Tu acceso a este programa ha sido bloqueado/i)).toBeInTheDocument()
    })

    it("shows baja message for DadoDeBaja", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_DadoDeBaja} />)

        expect(screen.getByText(/Tu inscripcion fue dada de baja/i)).toBeInTheDocument()
    })

    it("shows expand button with tareas for aprobado", async () => {
        const user = userEvent.setup()
        render(<InscripcionItem inscripcion={mockMiPrograma_Aprobado} />)

        const expandButton = screen.getByRole("button", { name: /Ver tareas y estadisticas/i })
        expect(expandButton).toBeInTheDocument()

        await user.click(expandButton)

        expect(screen.getByText("Comparte en Instagram Stories")).toBeInTheDocument()
    })

    it("renders expanded by default when defaultExpanded is true", () => {
        render(<InscripcionItem inscripcion={mockMiPrograma_Aprobado} defaultExpanded={true} />)

        expect(screen.getByText("Comparte en Instagram Stories")).toBeInTheDocument()
        expect(screen.getByRole("button", { name: /Ocultar detalles/i })).toBeInTheDocument()
    })
})

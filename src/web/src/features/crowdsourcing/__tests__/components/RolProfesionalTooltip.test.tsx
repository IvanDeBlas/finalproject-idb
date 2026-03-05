import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { RolProfesionalTooltip } from "../../presentation/components/RolProfesionalTooltip"
import { TooltipProvider } from "@/components/ui/tooltip"
import type { RolProfesional } from "../../domain"

function renderWithTooltip(ui: React.ReactElement) {
    return render(<TooltipProvider>{ui}</TooltipProvider>)
}

const mockRol: RolProfesional = {
    id: 1,
    nombre: "Productor Musical",
    descripcion: "Profesional que guia el proceso creativo y tecnico",
    categoriaRolId: 1,
    modalidadCobro: "Por proyecto",
}

describe("RolProfesionalTooltip", () => {
    it("renders info button with aria-label", () => {
        renderWithTooltip(<RolProfesionalTooltip rol={mockRol} />)

        expect(
            screen.getByRole("button", {
                name: /informacion sobre Productor Musical/i,
            })
        ).toBeInTheDocument()
    })

    it("renders nothing when rol has no descripcion", () => {
        const rolSinDesc: RolProfesional = {
            id: 2,
            nombre: "Test",
            descripcion: "",
            categoriaRolId: 1,
            modalidadCobro: "Por hora",
        }

        const { container } = renderWithTooltip(
            <RolProfesionalTooltip rol={rolSinDesc} />
        )

        expect(container.firstChild).toBeNull()
    })

    it("has accessible button element", () => {
        renderWithTooltip(<RolProfesionalTooltip rol={mockRol} />)

        const button = screen.getByRole("button")
        expect(button).toHaveAttribute(
            "aria-label",
            "Informacion sobre Productor Musical"
        )
    })
})

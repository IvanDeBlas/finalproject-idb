import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { CampaniaStatusBadge } from "../CampaniaStatusBadge"

describe("CampaniaStatusBadge", () => {
    it("renders correct label for Borrador (1)", () => {
        render(<CampaniaStatusBadge estadoId={1} />)

        expect(screen.getByText("Borrador")).toBeInTheDocument()
    })

    it("renders correct label for Publicada (2)", () => {
        render(<CampaniaStatusBadge estadoId={2} />)

        expect(screen.getByText("Publicada")).toBeInTheDocument()
    })

    it("renders correct label for Finalizada (3)", () => {
        render(<CampaniaStatusBadge estadoId={3} />)

        expect(screen.getByText("Finalizada")).toBeInTheDocument()
    })

    it("renders correct label for Cancelada (4)", () => {
        render(<CampaniaStatusBadge estadoId={4} />)

        expect(screen.getByText("Cancelada")).toBeInTheDocument()
    })

    it("renders Desconocido for unknown estado", () => {
        render(<CampaniaStatusBadge estadoId={99} />)

        expect(screen.getByText("Desconocido")).toBeInTheDocument()
    })

    it("has correct aria-label", () => {
        render(<CampaniaStatusBadge estadoId={2} />)

        expect(
            screen.getByRole("status", {
                name: "Estado de campania: Publicada",
            })
        ).toBeInTheDocument()
    })
})

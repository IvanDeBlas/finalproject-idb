import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { BrowserRouter } from "react-router-dom"
import { EmptyStatePropuestas } from "../../presentation/components/EmptyStatePropuestas"

function renderWithRouter(ui: React.ReactElement) {
    return render(<BrowserRouter>{ui}</BrowserRouter>)
}

describe("EmptyStatePropuestas", () => {
    it("renders 'Sin propuestas enviadas' when no filter active", () => {
        renderWithRouter(
            <EmptyStatePropuestas estadoFiltroActivo={null} />
        )

        expect(screen.getByText("Sin propuestas enviadas")).toBeInTheDocument()
        expect(
            screen.getByText("Aun no has enviado propuestas a ningun artista")
        ).toBeInTheDocument()
    })

    it("renders explorar link when no filter active", () => {
        renderWithRouter(
            <EmptyStatePropuestas estadoFiltroActivo={null} />
        )

        expect(screen.getByText("Explorar necesidades")).toBeInTheDocument()
    })

    it("renders 'Sin propuestas Pendiente' when filter active", () => {
        renderWithRouter(
            <EmptyStatePropuestas estadoFiltroActivo={1} estadoNombre="Pendiente" />
        )

        expect(screen.getByText(/Sin propuestas/)).toBeInTheDocument()
        expect(
            screen.getByText("No tienes propuestas en este estado")
        ).toBeInTheDocument()
    })

    it("has role=status for accessibility", () => {
        renderWithRouter(
            <EmptyStatePropuestas estadoFiltroActivo={null} />
        )

        expect(screen.getByRole("status")).toBeInTheDocument()
    })
})

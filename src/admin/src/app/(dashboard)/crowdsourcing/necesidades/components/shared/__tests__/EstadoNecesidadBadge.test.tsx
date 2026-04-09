import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { EstadoNecesidadBadge } from "../EstadoNecesidadBadge"
import { ESTADO_NECESIDAD } from "@shared/constants"

describe("EstadoNecesidadBadge", () => {
    it("renders Abierta with green color", () => {
        const { container } = render(
            <EstadoNecesidadBadge estadoId={ESTADO_NECESIDAD.ABIERTA} />
        )

        expect(screen.getByText("Abierta")).toBeInTheDocument()
        const badge = container.querySelector("[class*='green']")
        expect(badge).toBeInTheDocument()
    })

    it("renders En Progreso with blue color", () => {
        const { container } = render(
            <EstadoNecesidadBadge estadoId={ESTADO_NECESIDAD.EN_PROGRESO} />
        )

        expect(screen.getByText("En Progreso")).toBeInTheDocument()
        const badge = container.querySelector("[class*='blue']")
        expect(badge).toBeInTheDocument()
    })

    it("renders Cerrada with gray color", () => {
        const { container } = render(
            <EstadoNecesidadBadge estadoId={ESTADO_NECESIDAD.CERRADA} />
        )

        expect(screen.getByText("Cerrada")).toBeInTheDocument()
        const badge = container.querySelector("[class*='gray']")
        expect(badge).toBeInTheDocument()
    })

    it("renders Cancelada with red color", () => {
        const { container } = render(
            <EstadoNecesidadBadge estadoId={ESTADO_NECESIDAD.CANCELADA} />
        )

        expect(screen.getByText("Cancelada")).toBeInTheDocument()
        const badge = container.querySelector("[class*='red']")
        expect(badge).toBeInTheDocument()
    })

    it("includes aria-label for accessibility", () => {
        render(<EstadoNecesidadBadge estadoId={ESTADO_NECESIDAD.ABIERTA} />)

        expect(screen.getByLabelText("Estado: Abierta")).toBeInTheDocument()
    })

    it("renders Desconocido for unknown estadoId", () => {
        render(<EstadoNecesidadBadge estadoId={999} />)

        expect(screen.getByText("Desconocido")).toBeInTheDocument()
    })
})

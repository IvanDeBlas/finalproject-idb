import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { EventoTipoBadge } from "../../metricas/presentation/components/EventoTipoBadge"
import type { TipoEventoPromo } from "@shared/types/crowdpromotion"

describe("EventoTipoBadge", () => {
    it('renders "Click" for tipoEventoId=1', () => {
        render(<EventoTipoBadge tipoEventoId={1 as TipoEventoPromo} />)
        expect(screen.getByText("Click")).toBeInTheDocument()
    })

    it('renders "Vista" for tipoEventoId=2', () => {
        render(<EventoTipoBadge tipoEventoId={2 as TipoEventoPromo} />)
        expect(screen.getByText("Vista")).toBeInTheDocument()
    })

    it('renders "Registro" for tipoEventoId=3', () => {
        render(<EventoTipoBadge tipoEventoId={3 as TipoEventoPromo} />)
        expect(screen.getByText("Registro")).toBeInTheDocument()
    })

    it('renders "Backing" for tipoEventoId=4', () => {
        render(<EventoTipoBadge tipoEventoId={4 as TipoEventoPromo} />)
        expect(screen.getByText("Backing")).toBeInTheDocument()
    })

    it('renders "Share" for tipoEventoId=5', () => {
        render(<EventoTipoBadge tipoEventoId={5 as TipoEventoPromo} />)
        expect(screen.getByText("Share")).toBeInTheDocument()
    })

    it("applies blue styling for Click (tipo 1)", () => {
        const { container } = render(<EventoTipoBadge tipoEventoId={1 as TipoEventoPromo} />)
        const badge = container.querySelector("[class*='blue']")
        expect(badge).not.toBeNull()
    })

    it("applies slate styling for PageView (tipo 2)", () => {
        const { container } = render(<EventoTipoBadge tipoEventoId={2 as TipoEventoPromo} />)
        const badge = container.querySelector("[class*='slate']")
        expect(badge).not.toBeNull()
    })

    it("applies green styling for Signup (tipo 3)", () => {
        const { container } = render(<EventoTipoBadge tipoEventoId={3 as TipoEventoPromo} />)
        const badge = container.querySelector("[class*='green']")
        expect(badge).not.toBeNull()
    })

    it("applies purple styling for Backing (tipo 4)", () => {
        const { container } = render(<EventoTipoBadge tipoEventoId={4 as TipoEventoPromo} />)
        const badge = container.querySelector("[class*='purple']")
        expect(badge).not.toBeNull()
    })

    it("applies amber styling for Share (tipo 5)", () => {
        const { container } = render(<EventoTipoBadge tipoEventoId={5 as TipoEventoPromo} />)
        const badge = container.querySelector("[class*='amber']")
        expect(badge).not.toBeNull()
    })
})

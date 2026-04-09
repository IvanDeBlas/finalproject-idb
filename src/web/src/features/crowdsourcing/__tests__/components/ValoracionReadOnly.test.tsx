import { render, screen } from "@testing-library/react"
import { describe, it, expect } from "vitest"
import { ValoracionReadOnly } from "../../presentation/components/ValoracionReadOnly"
import { mockValoracionCreada } from "../../__mocks__/valoracion.mock"

describe("ValoracionReadOnly", () => {
    it('renders card with title "Tu valoracion"', () => {
        render(<ValoracionReadOnly valoracion={mockValoracionCreada} />)
        expect(screen.getByText("Tu valoracion")).toBeInTheDocument()
    })

    it("renders StarDisplay with correct puntuacion", () => {
        render(<ValoracionReadOnly valoracion={mockValoracionCreada} />)
        // StarDisplay renders with role="img"
        const starDisplay = screen.getByRole("img")
        expect(starDisplay).toHaveAttribute(
            "aria-label",
            "Puntuacion: 5 de 5 estrellas"
        )
    })

    it("renders comment text when comentario exists", () => {
        render(<ValoracionReadOnly valoracion={mockValoracionCreada} />)
        expect(
            screen.getByText(
                /Excelente trabajo, muy profesional y puntual/
            )
        ).toBeInTheDocument()
    })

    it("does not render comment section when comentario is undefined", () => {
        render(
            <ValoracionReadOnly
                valoracion={{ ...mockValoracionCreada, comentario: undefined }}
            />
        )
        expect(
            screen.queryByText(
                /Excelente trabajo, muy profesional y puntual/
            )
        ).not.toBeInTheDocument()
    })

    it("renders formatted creation date", () => {
        render(<ValoracionReadOnly valoracion={mockValoracionCreada} />)
        // "2026-03-16T10:00:00Z" -> "16 mar 2026" (es-ES)
        expect(screen.getByText(/16/)).toBeInTheDocument()
        expect(screen.getByText(/2026/)).toBeInTheDocument()
    })

    it("does not render any input or button (read-only)", () => {
        render(<ValoracionReadOnly valoracion={mockValoracionCreada} />)
        expect(screen.queryByRole("button")).not.toBeInTheDocument()
        expect(screen.queryByRole("textbox")).not.toBeInTheDocument()
    })

    it("renders check icon indicating completion", () => {
        const { container } = render(
            <ValoracionReadOnly valoracion={mockValoracionCreada} />
        )
        // CheckCircle2 is rendered as an SVG
        const svgs = container.querySelectorAll("svg")
        expect(svgs.length).toBeGreaterThanOrEqual(1)
    })

    it("has green border styling indicating completed state", () => {
        const { container } = render(
            <ValoracionReadOnly valoracion={mockValoracionCreada} />
        )
        const card = container.firstChild as HTMLElement
        expect(card.className).toContain("border-[#10b981]")
    })
})

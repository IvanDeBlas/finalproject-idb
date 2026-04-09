import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { TemplateCard } from "../../presentation/components/TemplateCard"
import { mockTemplatesList } from "../../__mocks__/crowdsourcing.mock"

const mockTemplate = mockTemplatesList[0]

describe("TemplateCard", () => {
    it("renders template name", () => {
        render(<TemplateCard template={mockTemplate} onSelect={vi.fn()} />)

        expect(screen.getByText("Produccion de EP")).toBeInTheDocument()
    })

    it("renders template description", () => {
        render(<TemplateCard template={mockTemplate} onSelect={vi.fn()} />)

        expect(
            screen.getByText("Plantilla completa para producir un EP de 4-6 canciones")
        ).toBeInTheDocument()
    })

    it("displays necesidades count", () => {
        render(<TemplateCard template={mockTemplate} onSelect={vi.fn()} />)

        expect(screen.getByText("8 necesidades")).toBeInTheDocument()
    })

    it("displays price range", () => {
        render(<TemplateCard template={mockTemplate} onSelect={vi.fn()} />)

        // formatCurrency may render with or without thousand separators depending on locale
        expect(screen.getByText(/3[.,]?000/)).toBeInTheDocument()
        expect(screen.getByText(/8[.,]?000/)).toBeInTheDocument()
    })

    it("calls onSelect with template id on button click", () => {
        const handleSelect = vi.fn()
        render(<TemplateCard template={mockTemplate} onSelect={handleSelect} />)

        fireEvent.click(screen.getByText("Seleccionar"))
        expect(handleSelect).toHaveBeenCalledWith("tpl-001")
    })

    it("calls onSelect with template id on card click", () => {
        const handleSelect = vi.fn()
        render(<TemplateCard template={mockTemplate} onSelect={handleSelect} />)

        fireEvent.click(screen.getByRole("button", { name: /Template: Produccion de EP/i }))
        expect(handleSelect).toHaveBeenCalledWith("tpl-001")
    })

    it("calls onSelect on Enter key press", () => {
        const handleSelect = vi.fn()
        render(<TemplateCard template={mockTemplate} onSelect={handleSelect} />)

        const card = screen.getByRole("button", { name: /Template: Produccion de EP/i })
        fireEvent.keyDown(card, { key: "Enter" })
        expect(handleSelect).toHaveBeenCalledWith("tpl-001")
    })

    it("has correct aria-label with template info", () => {
        render(<TemplateCard template={mockTemplate} onSelect={vi.fn()} />)

        const card = screen.getByRole("button", { name: /Template: Produccion de EP/i })
        expect(card).toHaveAttribute("aria-label")
        expect(card.getAttribute("aria-label")).toContain("8 necesidades")
    })
})

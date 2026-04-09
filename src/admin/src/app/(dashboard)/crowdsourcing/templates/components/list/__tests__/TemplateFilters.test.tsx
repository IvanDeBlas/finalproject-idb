import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { TemplateFilters } from "../TemplateFilters"

describe("TemplateFilters", () => {
    const defaultProps = {
        searchQuery: "",
        onSearchChange: vi.fn(),
        statusFilter: "all",
        onStatusChange: vi.fn(),
    }

    it("renders search input", () => {
        render(<TemplateFilters {...defaultProps} />)

        expect(
            screen.getByPlaceholderText("Buscar templates...")
        ).toBeInTheDocument()
    })

    it("search input triggers onSearchChange", async () => {
        const user = userEvent.setup()
        render(<TemplateFilters {...defaultProps} />)

        const input = screen.getByPlaceholderText("Buscar templates...")
        await user.type(input, "EP")

        expect(defaultProps.onSearchChange).toHaveBeenCalled()
    })

    it("displays current search value", () => {
        render(<TemplateFilters {...defaultProps} searchQuery="test query" />)

        const input = screen.getByPlaceholderText("Buscar templates...")
        expect(input).toHaveValue("test query")
    })

    it("has search aria label", () => {
        render(<TemplateFilters {...defaultProps} />)

        expect(
            screen.getByLabelText("Buscar templates por nombre")
        ).toBeInTheDocument()
    })
})

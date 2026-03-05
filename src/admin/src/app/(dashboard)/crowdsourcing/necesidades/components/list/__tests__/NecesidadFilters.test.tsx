import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@/test-utils"
import { NecesidadFilters } from "../NecesidadFilters"

describe("NecesidadFilters", () => {
    const defaultProps = {
        estadoFilter: "all",
        onEstadoChange: vi.fn(),
        searchQuery: "",
        onSearchChange: vi.fn(),
    }

    it("renders search input", () => {
        render(<NecesidadFilters {...defaultProps} />)

        expect(
            screen.getByPlaceholderText(/buscar por titulo/i)
        ).toBeInTheDocument()
    })

    it("calls onSearchChange when typing in search input", () => {
        const handleSearch = vi.fn()
        render(
            <NecesidadFilters {...defaultProps} onSearchChange={handleSearch} />
        )

        const input = screen.getByPlaceholderText(/buscar por titulo/i)
        fireEvent.change(input, { target: { value: "mezcla" } })

        expect(handleSearch).toHaveBeenCalledWith("mezcla")
    })

    it("renders with current search value", () => {
        render(
            <NecesidadFilters {...defaultProps} searchQuery="mezcla" />
        )

        expect(screen.getByPlaceholderText(/buscar por titulo/i)).toHaveValue("mezcla")
    })
})

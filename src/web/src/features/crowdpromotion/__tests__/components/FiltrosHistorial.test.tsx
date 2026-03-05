import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { FiltrosHistorial } from "../../wallet/presentation/components/FiltrosHistorial"
import type { WalletTransaccionesFilters } from "@shared/types/crowdpromotion"

describe("FiltrosHistorial", () => {
    const defaultFilters: WalletTransaccionesFilters = {
        page: 1,
        pageSize: 10,
    }
    let onChange: ReturnType<typeof vi.fn>

    beforeEach(() => {
        onChange = vi.fn()
    })

    it("renders tipo and estado selects", () => {
        render(<FiltrosHistorial filters={defaultFilters} onChange={onChange} />)

        expect(screen.getByText("Todos")).toBeDefined()
        expect(screen.getByText("Todos los estados")).toBeDefined()
    })

    it("renders date inputs", () => {
        render(<FiltrosHistorial filters={defaultFilters} onChange={onChange} />)

        const dateInputs = screen.getAllByDisplayValue("")
        expect(dateInputs.length).toBeGreaterThanOrEqual(2)
    })

    it("does not show clear button when no filters active", () => {
        render(<FiltrosHistorial filters={defaultFilters} onChange={onChange} />)

        expect(screen.queryByText(/limpiar filtros/i)).toBeNull()
    })

    it("shows clear button when filters are active", () => {
        const filtersWithActive: WalletTransaccionesFilters = {
            ...defaultFilters,
            esCredito: true,
        }
        render(<FiltrosHistorial filters={filtersWithActive} onChange={onChange} />)

        expect(screen.getByText(/limpiar filtros/i)).toBeDefined()
    })

    it("calls onChange with cleared filters on clear click", async () => {
        const user = userEvent.setup()
        const filtersWithActive: WalletTransaccionesFilters = {
            ...defaultFilters,
            esCredito: true,
        }
        render(<FiltrosHistorial filters={filtersWithActive} onChange={onChange} />)

        await user.click(screen.getByText(/limpiar filtros/i))

        expect(onChange).toHaveBeenCalledWith({ page: 1, pageSize: 10 })
    })
})

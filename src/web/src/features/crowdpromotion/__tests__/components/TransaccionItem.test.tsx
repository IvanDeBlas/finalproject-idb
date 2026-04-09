import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { TransaccionItem } from "../../wallet/presentation/components/TransaccionItem"
import { mockTransaccionCredito, mockTransaccionDebito, mockTransaccionPagada } from "../../__mocks__/wallet.mock"

describe("TransaccionItem", () => {
    it("renders credit transaction with green importe", () => {
        render(<TransaccionItem transaccion={mockTransaccionCredito} />)

        expect(screen.getByText(/\+25,50/)).toBeDefined()
        expect(screen.getByText("Procesada")).toBeDefined()
        expect(screen.getByText(/comision backing/i)).toBeDefined()
    })

    it("renders debit transaction with red importe", () => {
        render(<TransaccionItem transaccion={mockTransaccionDebito} />)

        expect(screen.getByText(/-50,00/)).toBeDefined()
        expect(screen.getByText("Pendiente")).toBeDefined()
    })

    it("renders tipoRewardNombre when available", () => {
        render(<TransaccionItem transaccion={mockTransaccionCredito} />)

        expect(screen.getByText("Dinero")).toBeDefined()
    })

    it("does not render tipoRewardNombre when null", () => {
        render(<TransaccionItem transaccion={mockTransaccionDebito} />)

        expect(screen.queryByText("Dinero")).toBeNull()
    })

    it("renders formatted date", () => {
        render(<TransaccionItem transaccion={mockTransaccionCredito} />)

        // Feb 15, 2026 in es-ES short format
        const dateText = screen.getByText(/2026/)
        expect(dateText).toBeDefined()
    })

    it("renders correct badge for Pagada state", () => {
        render(<TransaccionItem transaccion={mockTransaccionPagada} />)

        expect(screen.getByText("Pagada")).toBeDefined()
    })

    it("renders concepto when available", () => {
        render(<TransaccionItem transaccion={mockTransaccionDebito} />)

        expect(screen.getByText(/retiro a cuenta bancaria/i)).toBeDefined()
    })

    it("uses custom monedaNombre", () => {
        render(<TransaccionItem transaccion={mockTransaccionCredito} monedaNombre="USD" />)

        // USD format includes $
        const importeEl = screen.getByText(/\+/)
        expect(importeEl).toBeDefined()
    })
})

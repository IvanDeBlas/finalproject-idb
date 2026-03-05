import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { DesactivarPromoProgramaDialog } from "../DesactivarPromoProgramaDialog"
import { promoProgramaService } from "@/services/promo-programa.service"
import { mockDesactivadoResult } from "@/__mocks__/promo-programa.mock"

vi.mock("@/services/promo-programa.service", () => ({
    promoProgramaService: {
        getMisProgramas: vi.fn(),
        getById: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        desactivar: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

describe("DesactivarPromoProgramaDialog", () => {
    const defaultProps = {
        open: true,
        onOpenChange: vi.fn(),
        programaId: "programa-1",
        tituloPrograma: "Mi Programa Test",
    }

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders dialog title", () => {
        render(<DesactivarPromoProgramaDialog {...defaultProps} />)
        expect(screen.getByText("Desactivar programa")).toBeInTheDocument()
    })

    it("shows program name in description", () => {
        render(<DesactivarPromoProgramaDialog {...defaultProps} />)
        expect(screen.getByText(/Mi Programa Test/)).toBeInTheDocument()
    })

    it("shows Cancelar and Desactivar buttons", () => {
        render(<DesactivarPromoProgramaDialog {...defaultProps} />)
        expect(screen.getByText("Cancelar")).toBeInTheDocument()
        expect(screen.getByText("Desactivar")).toBeInTheDocument()
    })

    it("calls desactivar service when confirmed", async () => {
        vi.mocked(promoProgramaService.desactivar).mockResolvedValue(mockDesactivadoResult)
        const user = userEvent.setup()
        render(<DesactivarPromoProgramaDialog {...defaultProps} />)

        await user.click(screen.getByText("Desactivar"))

        expect(promoProgramaService.desactivar).toHaveBeenCalledWith("programa-1")
    })

    it("does not render when open is false", () => {
        render(<DesactivarPromoProgramaDialog {...defaultProps} open={false} />)
        expect(screen.queryByText("Desactivar programa")).not.toBeInTheDocument()
    })
})

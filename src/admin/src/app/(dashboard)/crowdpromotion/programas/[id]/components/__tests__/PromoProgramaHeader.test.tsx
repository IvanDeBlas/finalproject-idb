import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { PromoProgramaHeader } from "../PromoProgramaHeader"
import { mockProgramaDetail, mockProgramaDetailInactivo } from "@/__mocks__/promo-programa.mock"

vi.mock("next/navigation", () => ({
    useRouter: () => ({
        push: vi.fn(),
    }),
}))

describe("PromoProgramaHeader", () => {
    const onDesactivar = vi.fn()

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders program title", () => {
        render(<PromoProgramaHeader programa={mockProgramaDetail} onDesactivar={onDesactivar} />)
        expect(screen.getByText("Campana de Referidos Q1")).toBeInTheDocument()
    })

    it("renders tipo badge", () => {
        render(<PromoProgramaHeader programa={mockProgramaDetail} onDesactivar={onDesactivar} />)
        expect(screen.getByText("Referral")).toBeInTheDocument()
    })

    it("renders campania title", () => {
        render(<PromoProgramaHeader programa={mockProgramaDetail} onDesactivar={onDesactivar} />)
        expect(screen.getByText("Mi Album Debut")).toBeInTheDocument()
    })

    it("shows Desactivar button for active program", () => {
        render(<PromoProgramaHeader programa={mockProgramaDetail} onDesactivar={onDesactivar} />)
        expect(screen.getByText("Desactivar")).toBeInTheDocument()
    })

    it("hides Desactivar button for inactive program", () => {
        render(<PromoProgramaHeader programa={mockProgramaDetailInactivo} onDesactivar={onDesactivar} />)
        expect(screen.queryByText("Desactivar")).not.toBeInTheDocument()
    })

    it("calls onDesactivar when button clicked", async () => {
        const user = userEvent.setup()
        render(<PromoProgramaHeader programa={mockProgramaDetail} onDesactivar={onDesactivar} />)
        await user.click(screen.getByText("Desactivar"))
        expect(onDesactivar).toHaveBeenCalledTimes(1)
    })

    it("always shows Editar button", () => {
        render(<PromoProgramaHeader programa={mockProgramaDetail} onDesactivar={onDesactivar} />)
        expect(screen.getByText("Editar")).toBeInTheDocument()
    })
})

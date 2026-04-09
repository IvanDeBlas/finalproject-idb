import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import { PromoProgramaCard } from "../PromoProgramaCard"
import { mockProgramaListItem, mockProgramaListItemInactivo } from "@/__mocks__/promo-programa.mock"

vi.mock("next/navigation", () => ({
    useRouter: () => ({
        push: vi.fn(),
    }),
}))

describe("PromoProgramaCard", () => {
    const onDesactivar = vi.fn()

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders program title", () => {
        render(<PromoProgramaCard programa={mockProgramaListItem} onDesactivar={onDesactivar} />)
        expect(screen.getByText("Campana de Referidos Q1")).toBeInTheDocument()
    })

    it("renders tipo badge", () => {
        render(<PromoProgramaCard programa={mockProgramaListItem} onDesactivar={onDesactivar} />)
        expect(screen.getByText("Referral")).toBeInTheDocument()
    })

    it("renders ACTIVO badge for active program", () => {
        render(<PromoProgramaCard programa={mockProgramaListItem} onDesactivar={onDesactivar} />)
        expect(screen.getByText("ACTIVO")).toBeInTheDocument()
    })

    it("renders INACTIVO badge for inactive program", () => {
        render(<PromoProgramaCard programa={mockProgramaListItemInactivo} onDesactivar={onDesactivar} />)
        expect(screen.getByText("INACTIVO")).toBeInTheDocument()
    })

    it("renders promotores count", () => {
        render(<PromoProgramaCard programa={mockProgramaListItem} onDesactivar={onDesactivar} />)
        expect(screen.getByText("5 promotores")).toBeInTheDocument()
    })

    it("renders tareas count", () => {
        render(<PromoProgramaCard programa={mockProgramaListItem} onDesactivar={onDesactivar} />)
        expect(screen.getByText("2 tareas")).toBeInTheDocument()
    })

    it("renders campania title when present", () => {
        render(<PromoProgramaCard programa={mockProgramaListItem} onDesactivar={onDesactivar} />)
        expect(screen.getByText("Mi Album Debut")).toBeInTheDocument()
    })

    it("renders commission info", () => {
        render(<PromoProgramaCard programa={mockProgramaListItem} onDesactivar={onDesactivar} />)
        expect(screen.getByText(/Comision: 10%/)).toBeInTheDocument()
    })
})

import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import { TemplateTable } from "../TemplateTable"
import type { PlantillaProyectoList } from "@shared/types"

const mockTemplates: PlantillaProyectoList[] = [
    {
        id: "t-1",
        nombre: "Produccion de EP",
        descripcion: "Plantilla para EP",
        icono: "music",
        orden: 1,
        precioMinTotal: 3700,
        precioMaxTotal: 11400,
        moneda: 1,
        cantidadNecesidades: 8,
        fases: ["Preproduccion", "Grabacion"],
    },
    {
        id: "t-2",
        nombre: "Album Completo",
        descripcion: "Plantilla para album",
        icono: "disc",
        orden: 2,
        precioMinTotal: 8000,
        precioMaxTotal: 25000,
        moneda: 1,
        cantidadNecesidades: 12,
        fases: ["Preproduccion", "Grabacion", "Mezcla y Master"],
    },
]

const defaultProps = {
    templates: mockTemplates,
    onEdit: vi.fn(),
    onView: vi.fn(),
    onToggleStatus: vi.fn(),
    isLoading: false,
}

describe("TemplateTable", () => {
    it("renders table with templates", () => {
        render(<TemplateTable {...defaultProps} />)

        expect(screen.getByText("Produccion de EP")).toBeInTheDocument()
        expect(screen.getByText("Album Completo")).toBeInTheDocument()
    })

    it("displays template name and stats", () => {
        render(<TemplateTable {...defaultProps} />)

        expect(screen.getByText("Produccion de EP")).toBeInTheDocument()
        expect(screen.getByText("8")).toBeInTheDocument()
        expect(screen.getByText("12")).toBeInTheDocument()
    })

    it("renders column headers", () => {
        render(<TemplateTable {...defaultProps} />)

        expect(screen.getByText("Nombre")).toBeInTheDocument()
        expect(screen.getByText("Neces.")).toBeInTheDocument()
        expect(screen.getByText("Estado")).toBeInTheDocument()
    })

    it("shows empty state when no data", () => {
        render(<TemplateTable {...defaultProps} templates={[]} />)

        expect(screen.getByText("No hay templates disponibles")).toBeInTheDocument()
    })

    it("shows skeleton when loading", () => {
        const { container } = render(
            <TemplateTable {...defaultProps} templates={[]} isLoading={true} />
        )

        const skeletons = container.querySelectorAll("[class*='animate-pulse']")
        expect(skeletons.length).toBeGreaterThan(0)
    })

    it("renders status badges", () => {
        render(<TemplateTable {...defaultProps} />)

        const activeBadges = screen.getAllByText("Activo")
        expect(activeBadges.length).toBe(2)
    })
})

import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { TemplateForm } from "../TemplateForm"
import type { PlantillaProyecto } from "@shared/types"

const mockRoles = [
    {
        id: 1,
        nombre: "Productor Musical",
        descripcion: "Guia el proceso creativo",
        categoriaRol: { id: 1, nombre: "Produccion", icono: "music", orden: 1 },
        modalidadCobro: "Por proyecto",
        activo: true,
    },
    {
        id: 2,
        nombre: "Ingeniero de Mezcla",
        descripcion: "Mezcla de audio",
        categoriaRol: { id: 2, nombre: "Ingenieria", icono: "headphones", orden: 2 },
        modalidadCobro: "Por cancion",
        activo: true,
    },
]

const mockTemplate: PlantillaProyecto = {
    id: "t-1",
    nombre: "EP Template",
    descripcion: "Template para EP",
    icono: "music",
    orden: 1,
    necesidades: [
        {
            id: "n-1",
            fase: "Preproduccion",
            titulo: "Arreglos",
            descripcion: "Arreglos musicales",
            rolProfesional: {
                id: 1,
                nombre: "Productor Musical",
                descripcion: "Guia el proceso",
                categoriaRolId: 1,
                modalidadCobro: "Por proyecto",
            },
            precioMinOrientativo: 500,
            precioMaxOrientativo: 1500,
            moneda: 1,
            prioridad: "Alta",
            orden: 0,
        },
    ],
    resumen: {
        precioMinTotal: 500,
        precioMaxTotal: 1500,
        moneda: 1,
        cantidadNecesidadesAlta: 1,
        cantidadNecesidadesMedia: 0,
        cantidadNecesidadesBaja: 0,
    },
}

const defaultProps = {
    mode: "create" as const,
    rolesProfesionales: mockRoles,
    onSubmit: vi.fn(),
    onCancel: vi.fn(),
}

describe("TemplateForm", () => {
    it("renders form sections", () => {
        render(<TemplateForm {...defaultProps} />)

        expect(screen.getByText("Datos Generales")).toBeInTheDocument()
        expect(screen.getByText(/Necesidades/)).toBeInTheDocument()
    })

    it("renders create mode buttons", () => {
        render(<TemplateForm {...defaultProps} />)

        expect(screen.getByRole("button", { name: "Crear template" })).toBeInTheDocument()
        expect(screen.getByRole("button", { name: "Cancelar" })).toBeInTheDocument()
    })

    it("renders edit mode button text", () => {
        render(<TemplateForm {...defaultProps} mode="edit" initialData={mockTemplate} />)

        expect(screen.getByRole("button", { name: "Guardar cambios" })).toBeInTheDocument()
    })

    it("pre-fills form in edit mode", () => {
        render(<TemplateForm {...defaultProps} mode="edit" initialData={mockTemplate} />)

        expect(screen.getByDisplayValue("EP Template")).toBeInTheDocument()
        expect(screen.getByDisplayValue("Template para EP")).toBeInTheDocument()
    })

    it("calls onCancel when cancel button clicked", async () => {
        const user = userEvent.setup()
        render(<TemplateForm {...defaultProps} />)

        await user.click(screen.getByRole("button", { name: "Cancelar" }))

        expect(defaultProps.onCancel).toHaveBeenCalledOnce()
    })

    it("disables buttons when isPending", () => {
        render(<TemplateForm {...defaultProps} isPending={true} />)

        expect(screen.getByRole("button", { name: /Guardando/i })).toBeDisabled()
        expect(screen.getByRole("button", { name: "Cancelar" })).toBeDisabled()
    })

    it("shows loading text when isPending", () => {
        render(<TemplateForm {...defaultProps} isPending={true} />)

        expect(screen.getByText("Guardando...")).toBeInTheDocument()
    })

    it("renders nombre input field", () => {
        render(<TemplateForm {...defaultProps} />)

        expect(screen.getByLabelText("Nombre *")).toBeInTheDocument()
    })

    it("renders add necesidad button", () => {
        render(<TemplateForm {...defaultProps} />)

        expect(screen.getByRole("button", { name: /Agregar necesidad/i })).toBeInTheDocument()
    })

    it("shows empty necesidades message in create mode", () => {
        render(<TemplateForm {...defaultProps} />)

        expect(screen.getByText("No hay necesidades agregadas")).toBeInTheDocument()
    })

    it("pre-fills necesidades in edit mode", () => {
        render(<TemplateForm {...defaultProps} mode="edit" initialData={mockTemplate} />)

        expect(screen.getByDisplayValue("Arreglos")).toBeInTheDocument()
    })

    it("adds new necesidad when clicking add button", async () => {
        const user = userEvent.setup()
        render(<TemplateForm {...defaultProps} />)

        await user.click(screen.getByRole("button", { name: /Agregar necesidad/i }))

        expect(screen.queryByText("No hay necesidades agregadas")).not.toBeInTheDocument()
        expect(screen.getByText("Necesidades (1)")).toBeInTheDocument()
    })
})

import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { useForm, FormProvider } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { NecesidadFormItem } from "../NecesidadFormItem"
import {
    createTemplateSchema,
    type CreateTemplateFormData,
} from "@/lib/validations/template.schema"
import type { ReactNode } from "react"

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

function FormWrapper({
    children,
    defaultValues,
}: {
    children: ReactNode
    defaultValues?: Partial<CreateTemplateFormData>
}) {
    const form = useForm<CreateTemplateFormData>({
        resolver: zodResolver(createTemplateSchema),
        defaultValues: {
            nombre: "Test",
            icono: "music",
            orden: 0,
            activo: true,
            necesidades: [
                {
                    fase: "Preproduccion",
                    titulo: "Productor",
                    descripcion: "",
                    rolProfesionalId: 1,
                    precioMinOrientativo: 500,
                    precioMaxOrientativo: 1500,
                    prioridad: "Alta",
                    orden: 0,
                },
            ],
            ...defaultValues,
        },
    })

    return <FormProvider {...form}>{children}</FormProvider>
}

describe("NecesidadFormItem", () => {
    const defaultProps = {
        index: 0,
        rolesProfesionales: mockRoles,
        onDelete: vi.fn(),
    }

    it("renders all form labels", () => {
        render(
            <FormWrapper>
                <NecesidadFormItem {...defaultProps} />
            </FormWrapper>
        )

        expect(screen.getByText("Fase *")).toBeInTheDocument()
        expect(screen.getByText("Titulo *")).toBeInTheDocument()
        expect(screen.getByText("Rol Profesional *")).toBeInTheDocument()
        expect(screen.getByText("Presupuesto Orientativo")).toBeInTheDocument()
        expect(screen.getByText("Prioridad *")).toBeInTheDocument()
    })

    it("renders titulo input with pre-filled value", () => {
        render(
            <FormWrapper>
                <NecesidadFormItem {...defaultProps} />
            </FormWrapper>
        )

        expect(screen.getByDisplayValue("Productor")).toBeInTheDocument()
    })

    it("renders delete button with aria label", () => {
        render(
            <FormWrapper>
                <NecesidadFormItem {...defaultProps} />
            </FormWrapper>
        )

        expect(
            screen.getByRole("button", { name: "Eliminar necesidad" })
        ).toBeInTheDocument()
    })

    it("calls onDelete when delete button is clicked", async () => {
        const user = userEvent.setup()
        const onDelete = vi.fn()

        render(
            <FormWrapper>
                <NecesidadFormItem {...defaultProps} onDelete={onDelete} />
            </FormWrapper>
        )

        await user.click(screen.getByRole("button", { name: "Eliminar necesidad" }))

        expect(onDelete).toHaveBeenCalledOnce()
    })

    it("renders presupuesto min and max inputs", () => {
        render(
            <FormWrapper>
                <NecesidadFormItem {...defaultProps} />
            </FormWrapper>
        )

        expect(screen.getByPlaceholderText("Min")).toBeInTheDocument()
        expect(screen.getByPlaceholderText("Max")).toBeInTheDocument()
        expect(screen.getByText("EUR")).toBeInTheDocument()
    })

    it("renders pre-filled budget values", () => {
        render(
            <FormWrapper>
                <NecesidadFormItem {...defaultProps} />
            </FormWrapper>
        )

        expect(screen.getByDisplayValue("500")).toBeInTheDocument()
        expect(screen.getByDisplayValue("1500")).toBeInTheDocument()
    })
})

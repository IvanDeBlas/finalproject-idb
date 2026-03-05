import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { TareaPendienteCard } from "../TareaPendienteCard"
import {
    mockTareaPendienteItem,
    mockTareaPendienteItemSinComentario,
} from "@/__mocks__/cp-tareas-promocion.mock"

describe("TareaPendienteCard", () => {
    it("renders promotor name and tarea name", () => {
        render(
            <TareaPendienteCard
                item={mockTareaPendienteItem}
                onValidar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        expect(screen.getByText("Maria Lopez")).toBeInTheDocument()
        expect(screen.getByText(/Comparte en Instagram Stories/)).toBeInTheDocument()
    })

    it("renders proof URL as external link", () => {
        render(
            <TareaPendienteCard
                item={mockTareaPendienteItem}
                onValidar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        const link = screen.getByRole("link")
        expect(link).toHaveAttribute("href", "https://instagram.com/stories/maria_promo_abc123")
        expect(link).toHaveAttribute("target", "_blank")
        expect(link).toHaveAttribute("rel", expect.stringContaining("noopener"))
    })

    it("renders promotor comment when present", () => {
        render(
            <TareaPendienteCard
                item={mockTareaPendienteItem}
                onValidar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        expect(
            screen.getByText("Comparto mi story con la campana tal como se indico")
        ).toBeInTheDocument()
    })

    it("shows sin comentario when comment is absent", () => {
        render(
            <TareaPendienteCard
                item={mockTareaPendienteItemSinComentario}
                onValidar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        expect(screen.getByText("(sin comentario)")).toBeInTheDocument()
    })

    it("renders vecesCompletada as ordinal", () => {
        render(
            <TareaPendienteCard
                item={mockTareaPendienteItemSinComentario}
                onValidar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        expect(screen.getByText(/3ra vez/)).toBeInTheDocument()
    })

    it("calls onValidar with item when Validar clicked", async () => {
        const onValidar = vi.fn()
        const user = userEvent.setup()

        render(
            <TareaPendienteCard
                item={mockTareaPendienteItem}
                onValidar={onValidar}
                onRechazar={vi.fn()}
            />
        )

        await user.click(screen.getByRole("button", { name: /Validar/i }))
        expect(onValidar).toHaveBeenCalledWith(mockTareaPendienteItem)
    })

    it("calls onRechazar with item when Rechazar clicked", async () => {
        const onRechazar = vi.fn()
        const user = userEvent.setup()

        render(
            <TareaPendienteCard
                item={mockTareaPendienteItem}
                onValidar={vi.fn()}
                onRechazar={onRechazar}
            />
        )

        await user.click(screen.getByRole("button", { name: /Rechazar/i }))
        expect(onRechazar).toHaveBeenCalledWith(mockTareaPendienteItem)
    })
})

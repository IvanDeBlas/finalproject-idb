import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { EmptyState } from "../EmptyState"

describe("EmptyState", () => {
    it("renders title and description", () => {
        render(
            <EmptyState
                icon={<span data-testid="icon">icon</span>}
                title="No hay campanias"
                description="Crea tu primera campania para empezar"
            />
        )

        expect(screen.getByText("No hay campanias")).toBeInTheDocument()
        expect(
            screen.getByText("Crea tu primera campania para empezar")
        ).toBeInTheDocument()
    })

    it("renders icon", () => {
        render(
            <EmptyState
                icon={<span data-testid="test-icon">icon</span>}
                title="Titulo"
                description="Descripcion"
            />
        )

        expect(screen.getByTestId("test-icon")).toBeInTheDocument()
    })

    it("renders action button when actionLabel and onAction provided", () => {
        const handleAction = vi.fn()

        render(
            <EmptyState
                icon={<span>icon</span>}
                title="Titulo"
                description="Descripcion"
                actionLabel="Crear campania"
                onAction={handleAction}
            />
        )

        expect(
            screen.getByRole("button", { name: "Crear campania" })
        ).toBeInTheDocument()
    })

    it("does not render button when actionLabel missing", () => {
        render(
            <EmptyState
                icon={<span>icon</span>}
                title="Titulo"
                description="Descripcion"
                onAction={() => {}}
            />
        )

        expect(screen.queryByRole("button")).not.toBeInTheDocument()
    })

    it("calls onAction when button clicked", async () => {
        const handleAction = vi.fn()
        const user = userEvent.setup()

        render(
            <EmptyState
                icon={<span>icon</span>}
                title="Titulo"
                description="Descripcion"
                actionLabel="Crear campania"
                onAction={handleAction}
            />
        )

        await user.click(
            screen.getByRole("button", { name: "Crear campania" })
        )

        expect(handleAction).toHaveBeenCalledOnce()
    })
})

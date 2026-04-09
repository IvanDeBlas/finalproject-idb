import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { PromoProgramaWizardStepper } from "../PromoProgramaWizardStepper"

describe("PromoProgramaWizardStepper", () => {
    it("renders all 4 step labels", () => {
        render(
            <PromoProgramaWizardStepper
                pasoActual={1}
                pasosCompletados={[]}
            />
        )
        expect(screen.getByText("Datos basicos")).toBeInTheDocument()
        expect(screen.getByText("Comisiones")).toBeInTheDocument()
        expect(screen.getByText("Definir tareas")).toBeInTheDocument()
        expect(screen.getByText("Revisar")).toBeInTheDocument()
    })

    it("marks active step with aria-current", () => {
        render(
            <PromoProgramaWizardStepper
                pasoActual={2}
                pasosCompletados={[1]}
            />
        )
        const activeButton = screen.getByRole("button", { current: "step" })
        expect(activeButton).toBeInTheDocument()
    })

    it("calls onPasoClick when clicking completed step", async () => {
        const user = userEvent.setup()
        const onPasoClick = vi.fn()
        render(
            <PromoProgramaWizardStepper
                pasoActual={3}
                pasosCompletados={[1, 2]}
                onPasoClick={onPasoClick}
            />
        )

        await user.click(screen.getByText("Datos basicos"))
        expect(onPasoClick).toHaveBeenCalledWith(1)
    })

    it("does not call onPasoClick when clicking uncompleted step", async () => {
        const user = userEvent.setup()
        const onPasoClick = vi.fn()
        render(
            <PromoProgramaWizardStepper
                pasoActual={1}
                pasosCompletados={[]}
                onPasoClick={onPasoClick}
            />
        )

        await user.click(screen.getByText("Comisiones"))
        expect(onPasoClick).not.toHaveBeenCalled()
    })

    it("shows step numbers for uncompleted steps", () => {
        render(
            <PromoProgramaWizardStepper
                pasoActual={1}
                pasosCompletados={[]}
            />
        )
        expect(screen.getByText("1")).toBeInTheDocument()
        expect(screen.getByText("2")).toBeInTheDocument()
        expect(screen.getByText("3")).toBeInTheDocument()
        expect(screen.getByText("4")).toBeInTheDocument()
    })
})

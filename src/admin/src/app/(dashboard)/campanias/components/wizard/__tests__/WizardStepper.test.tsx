import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { WizardStepper } from "../WizardStepper"

describe("WizardStepper", () => {
    const defaultProps = {
        currentStep: 1,
        totalSteps: 4,
        completedSteps: [] as number[],
    }

    it("renders all 4 step labels", () => {
        render(<WizardStepper {...defaultProps} />)

        // Labels appear in both desktop and mobile views, so use getAllByText
        expect(screen.getAllByText("Informacion Basica").length).toBeGreaterThanOrEqual(1)
        expect(screen.getAllByText("Historia").length).toBeGreaterThanOrEqual(1)
        expect(screen.getAllByText("Recompensas").length).toBeGreaterThanOrEqual(1)
        expect(screen.getAllByText("Revision").length).toBeGreaterThanOrEqual(1)
    })

    it("highlights current step with aria-current='step'", () => {
        render(<WizardStepper {...defaultProps} currentStep={2} />)

        const buttons = screen.getAllByRole("button")
        const step2Button = buttons.find(
            (btn) => btn.getAttribute("aria-current") === "step"
        )

        expect(step2Button).toBeDefined()
        expect(step2Button).toHaveTextContent("2")
    })

    it("shows check icon for completed steps", () => {
        const { container } = render(
            <WizardStepper
                {...defaultProps}
                currentStep={3}
                completedSteps={[1, 2]}
            />
        )

        // Completed steps (1 and 2) should have SVG check icons instead of numbers
        // The desktop stepper buttons for completed non-active steps show <Check> icon
        const desktopStepper = container.querySelector(".hidden.md\\:flex")
        const svgs = desktopStepper?.querySelectorAll("svg")

        // Steps 1 and 2 are completed and not active, so they show check icons
        expect(svgs?.length).toBeGreaterThanOrEqual(2)
    })

    it("calls onStepClick when completed step clicked", async () => {
        const onStepClick = vi.fn()
        const user = userEvent.setup()

        render(
            <WizardStepper
                {...defaultProps}
                currentStep={3}
                completedSteps={[1, 2]}
                onStepClick={onStepClick}
            />
        )

        // Step 1 is completed; its button should be clickable
        const buttons = screen.getAllByRole("button")
        // Find the button that contains text "1" (completed step, but since
        // step 1 is completed and not active, it shows a check icon, not "1")
        // We need to find enabled buttons that are not the current step
        const enabledButtons = buttons.filter(
            (btn) => !btn.hasAttribute("disabled")
        )
        // Click the first enabled completed step button
        await user.click(enabledButtons[0])

        expect(onStepClick).toHaveBeenCalledWith(1)
    })

    it("does not call onStepClick for uncompleted steps", async () => {
        const onStepClick = vi.fn()
        const user = userEvent.setup()

        render(
            <WizardStepper
                {...defaultProps}
                currentStep={1}
                completedSteps={[]}
                onStepClick={onStepClick}
            />
        )

        // Step 4 is not completed, its button should be disabled
        const buttons = screen.getAllByRole("button")
        // All non-completed, non-active steps are disabled
        const disabledButtons = buttons.filter((btn) =>
            btn.hasAttribute("disabled")
        )
        expect(disabledButtons.length).toBeGreaterThan(0)

        // Clicking a disabled button should not trigger onStepClick
        await user.click(disabledButtons[0])

        expect(onStepClick).not.toHaveBeenCalled()
    })

    it("shows mobile step indicator ('Paso X de Y')", () => {
        render(
            <WizardStepper
                {...defaultProps}
                currentStep={2}
                totalSteps={4}
            />
        )

        // Text is split across child nodes, so use a matcher function
        expect(
            screen.getByText((_, element) => {
                return element?.textContent === "Paso 2 de 4"
            })
        ).toBeInTheDocument()
    })
})

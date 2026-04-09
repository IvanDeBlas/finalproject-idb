import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import AuthLayout from "../layout"

describe("AuthLayout", () => {
    it("renders children content", () => {
        render(
            <AuthLayout>
                <div data-testid="child">Child Content</div>
            </AuthLayout>
        )

        expect(screen.getByTestId("child")).toBeInTheDocument()
        expect(screen.getByText("Child Content")).toBeInTheDocument()
    })

    it("centers content in the viewport", () => {
        const { container } = render(
            <AuthLayout>
                <div>Content</div>
            </AuthLayout>
        )

        const wrapper = container.firstElementChild
        expect(wrapper).toHaveClass("min-h-screen", "flex", "items-center", "justify-center")
    })
})

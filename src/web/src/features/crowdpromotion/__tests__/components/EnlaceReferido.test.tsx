import { describe, it, expect, vi, beforeEach, afterEach } from "vitest"
import { render, screen, waitFor, act } from "@testing-library/react"
import userEvent from "@testing-library/user-event"

vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))

// Mock the hook directly for reliable clipboard testing
vi.mock("../../metricas/application/hooks/useCopyToClipboard", () => {
    const useCopyToClipboard = vi.fn()
    return { useCopyToClipboard }
})

import { toast } from "sonner"
import { useCopyToClipboard } from "../../metricas/application/hooks/useCopyToClipboard"
import { EnlaceReferido } from "../../metricas/presentation/components/EnlaceReferido"

const testUrl = "https://weplay.com/campanias/xxx?ref=album-2026-x7k9m"

describe("EnlaceReferido", () => {
    let mockCopy: ReturnType<typeof vi.fn>

    beforeEach(() => {
        vi.clearAllMocks()
        mockCopy = vi.fn()
        vi.mocked(useCopyToClipboard).mockReturnValue({
            copied: false,
            copy: mockCopy,
        })
    })

    afterEach(() => {
        vi.useRealTimers()
    })

    it("renders the input with the correct URL", () => {
        render(<EnlaceReferido url={testUrl} />)
        const input = screen.getByLabelText("Tu enlace de promocion") as HTMLInputElement
        expect(input.value).toBe(testUrl)
    })

    it('shows "Copiar" button in initial state', () => {
        render(<EnlaceReferido url={testUrl} />)
        expect(screen.getByText("Copiar")).toBeInTheDocument()
    })

    it("calls copy function when button is clicked", async () => {
        const user = userEvent.setup()
        render(<EnlaceReferido url={testUrl} />)

        await user.click(screen.getByRole("button", { name: /copiar/i }))

        expect(mockCopy).toHaveBeenCalledTimes(1)
    })

    it('changes button text to "Copiado!" after successful copy', () => {
        vi.mocked(useCopyToClipboard).mockReturnValue({
            copied: true,
            copy: mockCopy,
        })
        render(<EnlaceReferido url={testUrl} />)

        expect(screen.getByText("Copiado!")).toBeInTheDocument()
    })

    it("passes the URL to useCopyToClipboard", () => {
        render(<EnlaceReferido url={testUrl} />)

        expect(useCopyToClipboard).toHaveBeenCalledWith(testUrl, 2000)
    })

    it('has dynamic aria-label "Enlace copiado" when copied', () => {
        vi.mocked(useCopyToClipboard).mockReturnValue({
            copied: true,
            copy: mockCopy,
        })
        render(<EnlaceReferido url={testUrl} />)

        expect(screen.getByLabelText("Enlace copiado")).toBeInTheDocument()
    })

    it("disables button when copied", () => {
        vi.mocked(useCopyToClipboard).mockReturnValue({
            copied: true,
            copy: mockCopy,
        })
        render(<EnlaceReferido url={testUrl} />)

        expect(screen.getByLabelText("Enlace copiado")).toBeDisabled()
    })

    it("has readonly input", () => {
        render(<EnlaceReferido url={testUrl} />)
        const input = screen.getByLabelText("Tu enlace de promocion") as HTMLInputElement
        expect(input.readOnly).toBe(true)
    })
})

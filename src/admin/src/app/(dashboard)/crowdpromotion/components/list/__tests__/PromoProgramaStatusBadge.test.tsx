import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { PromoProgramaStatusBadge } from "../PromoProgramaStatusBadge"

describe("PromoProgramaStatusBadge", () => {
    it("renders ACTIVO for active program", () => {
        render(<PromoProgramaStatusBadge esActivo={true} />)
        expect(screen.getByText("ACTIVO")).toBeInTheDocument()
    })

    it("renders INACTIVO for inactive program", () => {
        render(<PromoProgramaStatusBadge esActivo={false} />)
        expect(screen.getByText("INACTIVO")).toBeInTheDocument()
    })

    it("applies green styles for active", () => {
        render(<PromoProgramaStatusBadge esActivo={true} />)
        const badge = screen.getByText("ACTIVO")
        expect(badge.className).toContain("emerald")
    })

    it("applies gray styles for inactive", () => {
        render(<PromoProgramaStatusBadge esActivo={false} />)
        const badge = screen.getByText("INACTIVO")
        expect(badge.className).toContain("zinc")
    })
})

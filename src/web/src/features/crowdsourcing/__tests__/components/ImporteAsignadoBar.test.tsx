import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { ImporteAsignadoBar } from "../../presentation/components/ImporteAsignadoBar"

describe("ImporteAsignadoBar", () => {
    it("renders text with importe asignado and total", () => {
        render(
            <ImporteAsignadoBar
                importeAsignado={270}
                importeTotal={450}
                porcentaje={60}
                monedaNombre="EUR"
            />
        )

        expect(screen.getByText(/270,00/)).toBeInTheDocument()
        expect(screen.getByText(/450,00/)).toBeInTheDocument()
        expect(screen.getByText(/EUR/)).toBeInTheDocument()
        expect(screen.getByText(/60%/)).toBeInTheDocument()
    })

    it("renders progressbar with correct aria-valuenow", () => {
        render(
            <ImporteAsignadoBar
                importeAsignado={270}
                importeTotal={450}
                porcentaje={60}
                monedaNombre="EUR"
            />
        )

        const progressbar = screen.getByRole("progressbar")
        expect(progressbar).toHaveAttribute("aria-valuenow", "60")
    })

    it("renders correctly when importeAsignado is 0", () => {
        render(
            <ImporteAsignadoBar
                importeAsignado={0}
                importeTotal={450}
                porcentaje={0}
                monedaNombre="EUR"
            />
        )

        const progressbar = screen.getByRole("progressbar")
        expect(progressbar).toHaveAttribute("aria-valuenow", "0")
        expect(screen.getByText(/0%/)).toBeInTheDocument()
    })

    it("renders correctly when 100% assigned", () => {
        render(
            <ImporteAsignadoBar
                importeAsignado={450}
                importeTotal={450}
                porcentaje={100}
                monedaNombre="EUR"
            />
        )

        const progressbar = screen.getByRole("progressbar")
        expect(progressbar).toHaveAttribute("aria-valuenow", "100")
        expect(screen.getByText(/100%/)).toBeInTheDocument()
    })

    it("updates display when importeNuevo is provided", () => {
        render(
            <ImporteAsignadoBar
                importeAsignado={270}
                importeTotal={450}
                porcentaje={60}
                monedaNombre="EUR"
                importeNuevo={90}
            />
        )

        // 270 + 90 = 360 => 80%
        expect(screen.getByText(/360,00/)).toBeInTheDocument()
        const progressbar = screen.getByRole("progressbar")
        expect(progressbar).toHaveAttribute("aria-valuenow", "80")
    })

    it("shows error text when sum exceeds total", () => {
        render(
            <ImporteAsignadoBar
                importeAsignado={400}
                importeTotal={450}
                porcentaje={89}
                monedaNombre="EUR"
                importeNuevo={100}
            />
        )

        // 400 + 100 = 500 > 450
        expect(
            screen.getByText("El importe asignado supera el total pactado")
        ).toBeInTheDocument()
    })
})

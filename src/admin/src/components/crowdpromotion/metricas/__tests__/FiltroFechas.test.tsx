import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { FiltroFechas } from "../FiltroFechas"

const defaultProps = {
    fechaDesde: "",
    fechaHasta: "",
    onFechaDesdeChange: vi.fn(),
    onFechaHastaChange: vi.fn(),
    onAplicar: vi.fn(),
    onLimpiar: vi.fn(),
    tieneFiltroPeriodo: false,
    isLoading: false,
}

describe("FiltroFechas", () => {
    it("renders both date inputs", () => {
        render(<FiltroFechas {...defaultProps} />)
        expect(screen.getByLabelText("Fecha de inicio")).toBeInTheDocument()
        expect(screen.getByLabelText("Fecha de fin")).toBeInTheDocument()
    })

    it("renders Aplicar button enabled by default", () => {
        render(<FiltroFechas {...defaultProps} />)
        const btn = screen.getByRole("button", { name: /Aplicar/i })
        expect(btn).toBeInTheDocument()
        expect(btn).not.toBeDisabled()
    })

    it("hides Limpiar button when tieneFiltroPeriodo is false", () => {
        render(<FiltroFechas {...defaultProps} />)
        expect(screen.queryByRole("button", { name: /Limpiar/i })).not.toBeInTheDocument()
    })

    it("shows Limpiar button when tieneFiltroPeriodo is true", () => {
        render(<FiltroFechas {...defaultProps} tieneFiltroPeriodo={true} />)
        expect(screen.getByRole("button", { name: /Limpiar/i })).toBeInTheDocument()
    })

    it("shows active filter badge when tieneFiltroPeriodo is true", () => {
        render(
            <FiltroFechas
                {...defaultProps}
                tieneFiltroPeriodo={true}
                fechaDesde="2026-03-01"
                fechaHasta="2026-03-31"
            />
        )
        expect(screen.getByText(/Filtrando:/)).toBeInTheDocument()
    })

    it("calls onAplicar when Aplicar is clicked", async () => {
        const onAplicar = vi.fn()
        render(<FiltroFechas {...defaultProps} onAplicar={onAplicar} />)

        await userEvent.click(screen.getByRole("button", { name: /Aplicar/i }))

        expect(onAplicar).toHaveBeenCalledTimes(1)
    })

    it("calls onLimpiar when Limpiar button is clicked", async () => {
        const onLimpiar = vi.fn()
        render(
            <FiltroFechas
                {...defaultProps}
                tieneFiltroPeriodo={true}
                onLimpiar={onLimpiar}
            />
        )

        await userEvent.click(screen.getByRole("button", { name: /Limpiar/i }))

        expect(onLimpiar).toHaveBeenCalledTimes(1)
    })

    it("calls onLimpiar when X in badge is clicked", async () => {
        const onLimpiar = vi.fn()
        render(
            <FiltroFechas
                {...defaultProps}
                tieneFiltroPeriodo={true}
                onLimpiar={onLimpiar}
            />
        )

        await userEvent.click(screen.getByLabelText("Quitar filtro de fechas"))

        expect(onLimpiar).toHaveBeenCalledTimes(1)
    })

    it("shows validation error when fechaDesde > fechaHasta", () => {
        render(
            <FiltroFechas
                {...defaultProps}
                fechaDesde="2026-03-31"
                fechaHasta="2026-03-01"
            />
        )

        expect(
            screen.getByText(/La fecha inicio no puede ser posterior a la fecha fin/)
        ).toBeInTheDocument()
        expect(screen.getByRole("button", { name: /Aplicar/i })).toBeDisabled()
    })

    it("disables Aplicar button when isLoading is true", () => {
        render(<FiltroFechas {...defaultProps} isLoading={true} />)
        expect(screen.getByRole("button", { name: /Aplicar/i })).toBeDisabled()
    })

    it("shows Loader2 spinner in Aplicar when isLoading", () => {
        const { container } = render(<FiltroFechas {...defaultProps} isLoading={true} />)
        const spinner = container.querySelector(".animate-spin")
        expect(spinner).toBeInTheDocument()
    })

    it("calls onFechaDesdeChange on input change", async () => {
        const onFechaDesdeChange = vi.fn()
        render(
            <FiltroFechas {...defaultProps} onFechaDesdeChange={onFechaDesdeChange} />
        )

        const input = screen.getByLabelText("Fecha de inicio")
        await userEvent.type(input, "2026-03-01")

        expect(onFechaDesdeChange).toHaveBeenCalled()
    })
})

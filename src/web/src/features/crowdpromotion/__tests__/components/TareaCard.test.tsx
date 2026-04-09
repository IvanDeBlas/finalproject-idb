import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { TareaCard } from "../../tareas/presentation/components/TareaCard"
import {
    mockTarea_Pendiente,
    mockTarea_Completada_NoRepetible,
    mockTarea_Validada_NoRepetible,
    mockTarea_Rechazada,
    mockTarea_RepetibleLimiteAlcanzado,
    mockTarea_RepetibleConCupo,
    mockTarea_SoloPuntos,
} from "../../__mocks__/tareas.mock"

const defaultProps = {
    onCompletar: vi.fn(),
    isProgramaActivo: true,
    isSubmitting: false,
}

describe("TareaCard", () => {
    it("renders the task name", () => {
        render(
            <TareaCard tarea={mockTarea_Pendiente} {...defaultProps} />
        )

        expect(
            screen.getByText("Comparte en Instagram Stories")
        ).toBeInTheDocument()
    })

    it("renders event type as a badge", () => {
        render(
            <TareaCard tarea={mockTarea_Pendiente} {...defaultProps} />
        )

        expect(screen.getByText("Share")).toBeInTheDocument()
    })

    it("renders monetary reward amount", () => {
        render(
            <TareaCard tarea={mockTarea_Pendiente} {...defaultProps} />
        )

        expect(screen.getByText(/5.*EUR/)).toBeInTheDocument()
    })

    it("renders points reward when no monetary amount", () => {
        render(
            <TareaCard tarea={mockTarea_SoloPuntos} {...defaultProps} />
        )

        expect(screen.getByText(/100 puntos/)).toBeInTheDocument()
    })

    it("renders Repetible badge when esRepetible is true", () => {
        render(
            <TareaCard tarea={mockTarea_Pendiente} {...defaultProps} />
        )

        expect(screen.getByText("Repetible")).toBeInTheDocument()
    })

    it("does not render Repetible badge when esRepetible is false", () => {
        render(
            <TareaCard
                tarea={mockTarea_Completada_NoRepetible}
                {...defaultProps}
            />
        )

        expect(screen.queryByText("Repetible")).not.toBeInTheDocument()
    })

    it("renders Completar button when miEstado is undefined", () => {
        render(
            <TareaCard tarea={mockTarea_Pendiente} {...defaultProps} />
        )

        expect(
            screen.getByRole("button", { name: /Completar tarea/i })
        ).toBeInTheDocument()
    })

    it("does not render action button when non-repeatable is Completada", () => {
        render(
            <TareaCard
                tarea={mockTarea_Completada_NoRepetible}
                {...defaultProps}
            />
        )

        expect(
            screen.queryByRole("button", { name: /Completar/i })
        ).not.toBeInTheDocument()
        expect(screen.getByText("Ver prueba enviada")).toBeInTheDocument()
    })

    it("does not render action button when non-repeatable is Validada", () => {
        render(
            <TareaCard
                tarea={mockTarea_Validada_NoRepetible}
                {...defaultProps}
            />
        )

        expect(
            screen.queryByRole("button", { name: /Completar/i })
        ).not.toBeInTheDocument()
    })

    it("shows max repetitions message when limit reached", () => {
        render(
            <TareaCard
                tarea={mockTarea_RepetibleLimiteAlcanzado}
                {...defaultProps}
            />
        )

        expect(
            screen.getByText(/Maximo de repeticiones alcanzado/)
        ).toBeInTheDocument()
    })

    it("renders Completar de nuevo button for repeatable with available quota", () => {
        render(
            <TareaCard
                tarea={mockTarea_RepetibleConCupo}
                {...defaultProps}
            />
        )

        expect(
            screen.getByRole("button", { name: /Completar de nuevo/i })
        ).toBeInTheDocument()
    })

    it("shows rejection reason when estadoTareaId is 4", () => {
        render(
            <TareaCard tarea={mockTarea_Rechazada} {...defaultProps} />
        )

        expect(
            screen.getByText("La URL no muestra el contenido requerido")
        ).toBeInTheDocument()
        expect(screen.getByText("Motivo del rechazo:")).toBeInTheDocument()
    })

    it("renders re-send button when task is rejected", () => {
        render(
            <TareaCard tarea={mockTarea_Rechazada} {...defaultProps} />
        )

        expect(
            screen.getByRole("button", {
                name: /Re-enviar con nueva prueba/i,
            })
        ).toBeInTheDocument()
    })

    it("calls onCompletar with tarea and esReenvio=false on Completar click", async () => {
        const handleCompletar = vi.fn()
        render(
            <TareaCard
                tarea={mockTarea_Pendiente}
                onCompletar={handleCompletar}
                isProgramaActivo={true}
            />
        )

        const user = userEvent.setup()
        await user.click(
            screen.getByRole("button", { name: /Completar tarea/i })
        )

        expect(handleCompletar).toHaveBeenCalledWith(
            mockTarea_Pendiente,
            false
        )
    })

    it("calls onCompletar with esReenvio=true on re-send click", async () => {
        const handleCompletar = vi.fn()
        render(
            <TareaCard
                tarea={mockTarea_Rechazada}
                onCompletar={handleCompletar}
                isProgramaActivo={true}
            />
        )

        const user = userEvent.setup()
        await user.click(
            screen.getByRole("button", {
                name: /Re-enviar con nueva prueba/i,
            })
        )

        expect(handleCompletar).toHaveBeenCalledWith(
            mockTarea_Rechazada,
            true
        )
    })

    it("shows Programa inactivo when isProgramaActivo is false", () => {
        render(
            <TareaCard
                tarea={mockTarea_Pendiente}
                onCompletar={vi.fn()}
                isProgramaActivo={false}
            />
        )

        expect(screen.getByText("Programa inactivo")).toBeInTheDocument()
        expect(
            screen.queryByRole("button", { name: /Completar/i })
        ).not.toBeInTheDocument()
    })
})

import { render, screen } from "@/test-utils"
import { PromotorProfileCard } from "../PromotorProfileCard"
import { mockPromotor, mockPromotorMinimo, mockPromotorInactivo } from "@/__mocks__/promotor.mock"

const defaultProps = {
    promotor: mockPromotor,
    onEditClick: vi.fn(),
    onDesactivarClick: vi.fn(),
}

describe("PromotorProfileCard", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders promotor nombre publico", () => {
        render(<PromotorProfileCard {...defaultProps} />)

        expect(screen.getByText("DJ Promo Star")).toBeInTheDocument()
    })

    it("renders tipo promotor label", () => {
        render(<PromotorProfileCard {...defaultProps} />)

        expect(screen.getByText("Influencer")).toBeInTheDocument()
    })

    it("shows active badge when esActivo is true", () => {
        render(<PromotorProfileCard {...defaultProps} />)

        expect(screen.getByText("Activo")).toBeInTheDocument()
    })

    it("shows inactive badge when esActivo is false", () => {
        render(
            <PromotorProfileCard
                {...defaultProps}
                promotor={mockPromotorInactivo}
            />
        )

        expect(screen.getByText("Inactivo")).toBeInTheDocument()
    })

    it("renders programas activos KPI card", () => {
        render(<PromotorProfileCard {...defaultProps} />)

        expect(screen.getByText("Programas Activos")).toBeInTheDocument()
        expect(screen.getByText("3")).toBeInTheDocument()
    })

    it("renders comisiones ganadas KPI card", () => {
        render(<PromotorProfileCard {...defaultProps} />)

        expect(screen.getByText("Comisiones Ganadas")).toBeInTheDocument()
    })

    it("renders social links when available", () => {
        render(<PromotorProfileCard {...defaultProps} />)

        expect(screen.getByLabelText("Instagram")).toBeInTheDocument()
        expect(screen.getByLabelText("YouTube")).toBeInTheDocument()
    })

    it("does not render social links section when none provided", () => {
        render(
            <PromotorProfileCard
                {...defaultProps}
                promotor={mockPromotorMinimo}
            />
        )

        expect(screen.queryByLabelText("Instagram")).not.toBeInTheDocument()
    })

    it("hides desactivar button when promotor is inactive", () => {
        render(
            <PromotorProfileCard
                {...defaultProps}
                promotor={mockPromotorInactivo}
            />
        )

        expect(screen.queryByText("Desactivar")).not.toBeInTheDocument()
    })
})

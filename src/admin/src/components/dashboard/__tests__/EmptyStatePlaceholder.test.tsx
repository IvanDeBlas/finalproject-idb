import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { EmptyStatePlaceholder } from "../EmptyStatePlaceholder"
import { Music } from "lucide-react"

describe("EmptyStatePlaceholder", () => {
    it("renders title", () => {
        render(
            <EmptyStatePlaceholder
                icon={Music}
                title="No tienes campanias aun"
                description="Crea tu primera campania"
            />
        )

        expect(
            screen.getByText("No tienes campanias aun")
        ).toBeInTheDocument()
    })

    it("renders description", () => {
        render(
            <EmptyStatePlaceholder
                icon={Music}
                title="No tienes campanias aun"
                description="Crea tu primera campania"
            />
        )

        expect(
            screen.getByText("Crea tu primera campania")
        ).toBeInTheDocument()
    })

    it("renders action button when action provided", () => {
        render(
            <EmptyStatePlaceholder
                icon={Music}
                title="No tienes campanias"
                description="Desc"
                action={{ label: "Crear campania", href: "/nueva" }}
            />
        )

        const link = screen.getByRole("link", { name: "Crear campania" })
        expect(link).toBeInTheDocument()
        expect(link).toHaveAttribute("href", "/nueva")
    })

    it("does not render action when not provided", () => {
        render(
            <EmptyStatePlaceholder
                icon={Music}
                title="No tienes campanias"
                description="Desc"
            />
        )

        expect(screen.queryByRole("link")).not.toBeInTheDocument()
    })

    it("renders icon", () => {
        const { container } = render(
            <EmptyStatePlaceholder
                icon={Music}
                title="Title"
                description="Desc"
            />
        )

        const svg = container.querySelector("svg")
        expect(svg).toBeInTheDocument()
    })
})

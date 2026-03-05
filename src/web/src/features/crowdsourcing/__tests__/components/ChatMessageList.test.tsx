import { describe, it, expect, vi, beforeAll } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { ChatMessageList } from "../../presentation/components/ChatMessageList"

beforeAll(() => {
    Element.prototype.scrollIntoView = vi.fn()
})
import {
    mockMensajeAjeno,
    mockMensajePropio,
    mockMensajePropioSinAdjunto,
} from "../../__mocks__/mensajeria.mock"
import type { Mensaje } from "../../domain"

const mensajes: Mensaje[] = [
    mockMensajeAjeno,
    mockMensajePropio,
    mockMensajePropioSinAdjunto,
]

describe("ChatMessageList", () => {
    it("shows skeleton during initial loading", () => {
        const { container } = render(
            <ChatMessageList
                mensajes={[]}
                isLoading={true}
                hasMore={false}
                isLoadingMore={false}
                onLoadMore={vi.fn()}
            />
        )

        // Loading state renders Skeleton components
        const skeletons = container.querySelectorAll("[class*='animate-pulse'], [data-slot='skeleton']")
        expect(skeletons.length).toBeGreaterThan(0)
    })

    it("renders messages when loaded", () => {
        render(
            <ChatMessageList
                mensajes={mensajes}
                isLoading={false}
                hasMore={false}
                isLoadingMore={false}
                onLoadMore={vi.fn()}
            />
        )

        expect(
            screen.getByText(/me interesa tu propuesta/)
        ).toBeInTheDocument()
        expect(
            screen.getByText(/He trabajado con bandas/)
        ).toBeInTheDocument()
    })

    it("shows empty state when no messages", () => {
        render(
            <ChatMessageList
                mensajes={[]}
                isLoading={false}
                hasMore={false}
                isLoadingMore={false}
                onLoadMore={vi.fn()}
            />
        )

        expect(
            screen.getByText(/Aun no hay mensajes/)
        ).toBeInTheDocument()
    })

    it("shows load more button when hasMore is true", () => {
        render(
            <ChatMessageList
                mensajes={mensajes}
                isLoading={false}
                hasMore={true}
                isLoadingMore={false}
                onLoadMore={vi.fn()}
            />
        )

        expect(
            screen.getByText("Cargar mensajes anteriores")
        ).toBeInTheDocument()
    })

    it("hides load more button when hasMore is false", () => {
        render(
            <ChatMessageList
                mensajes={mensajes}
                isLoading={false}
                hasMore={false}
                isLoadingMore={false}
                onLoadMore={vi.fn()}
            />
        )

        expect(
            screen.queryByText("Cargar mensajes anteriores")
        ).not.toBeInTheDocument()
    })

    it("calls onLoadMore when load more button is clicked", () => {
        const onLoadMore = vi.fn()
        render(
            <ChatMessageList
                mensajes={mensajes}
                isLoading={false}
                hasMore={true}
                isLoadingMore={false}
                onLoadMore={onLoadMore}
            />
        )

        fireEvent.click(screen.getByText("Cargar mensajes anteriores"))
        expect(onLoadMore).toHaveBeenCalledTimes(1)
    })

    it("renders date separators between different days", () => {
        // mockMensajeAjeno: 2026-03-01, mockMensajePropioSinAdjunto: 2026-03-02
        render(
            <ChatMessageList
                mensajes={mensajes}
                isLoading={false}
                hasMore={false}
                isLoadingMore={false}
                onLoadMore={vi.fn()}
            />
        )

        // Date separators contain month text from es-ES locale
        // March 2026 dates should produce date separator text
        const dateTexts = screen.getAllByText(/marzo/i)
        expect(dateTexts.length).toBeGreaterThanOrEqual(1)
    })
})

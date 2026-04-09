import { render, screen, act } from "@testing-library/react"
import { describe, it, expect, vi, afterEach } from "vitest"
import { RatingHistogram } from "../../presentation/components/RatingHistogram"
import {
    mockResumenConValoraciones,
    mockResumenSinValoraciones,
} from "../../__mocks__/valoracion.mock"

vi.useFakeTimers()

afterEach(() => {
    vi.clearAllTimers()
})

function renderAndAnimate(
    distribucion: Record<number, number>,
    total: number
) {
    const result = render(
        <RatingHistogram distribucion={distribucion} total={total} />
    )
    act(() => {
        vi.advanceTimersByTime(100)
    })
    return result
}

describe("RatingHistogram", () => {
    it("renders 5 rows (one per star level)", () => {
        const { container } = renderAndAnimate(
            mockResumenConValoraciones.distribucion,
            mockResumenConValoraciones.totalValoraciones
        )
        // Level labels are in the .w-3 spans
        const levelLabels = container.querySelectorAll(
            "span.w-3"
        )
        expect(levelLabels).toHaveLength(5)
        const texts = Array.from(levelLabels).map((el) => el.textContent)
        expect(texts).toEqual(["5", "4", "3", "2", "1"])
    })

    it("renders star level labels 5 to 1", () => {
        const { container } = renderAndAnimate(
            mockResumenConValoraciones.distribucion,
            mockResumenConValoraciones.totalValoraciones
        )
        const levelLabels = container.querySelectorAll(
            "span.w-3"
        )
        const texts = Array.from(levelLabels).map((el) => el.textContent)
        expect(texts).toEqual(["5", "4", "3", "2", "1"])
    })

    it("renders count for each level", () => {
        const { container } = renderAndAnimate(
            mockResumenConValoraciones.distribucion,
            mockResumenConValoraciones.totalValoraciones
        )
        // Counts are in the .w-4 spans
        const countSpans = container.querySelectorAll("span.w-4")
        const counts = Array.from(countSpans).map((el) => el.textContent)
        // Distribution: 5->7, 4->3, 3->1, 2->1, 1->0
        expect(counts).toEqual(["7", "3", "1", "1", "0"])
    })

    it("renders 0 for star level with no ratings", () => {
        const { container } = renderAndAnimate(
            mockResumenConValoraciones.distribucion,
            mockResumenConValoraciones.totalValoraciones
        )
        const countSpans = container.querySelectorAll("span.w-4")
        const counts = Array.from(countSpans).map((el) => el.textContent)
        // Level 1 has 0 ratings (last count)
        expect(counts[4]).toBe("0")
    })

    it("bar width is proportional to count relative to total", () => {
        const { container } = renderAndAnimate(
            { 5: 7, 4: 3, 3: 1, 2: 1, 1: 0 },
            12
        )
        const bars = container.querySelectorAll("[role='presentation']")
        expect(bars).toHaveLength(5)
        // Level 5: 7/12 = 58%
        expect((bars[0] as HTMLElement).style.width).toBe("58%")
        // Level 4: 3/12 = 25%
        expect((bars[1] as HTMLElement).style.width).toBe("25%")
        // Level 3: 1/12 = 8%
        expect((bars[2] as HTMLElement).style.width).toBe("8%")
        // Level 1: 0/12 = 0%
        expect((bars[4] as HTMLElement).style.width).toBe("0%")
    })

    it("all bars have 0% width when total is 0", () => {
        const { container } = renderAndAnimate(
            mockResumenSinValoraciones.distribucion,
            0
        )
        const bars = container.querySelectorAll("[role='presentation']")
        bars.forEach((bar) => {
            expect((bar as HTMLElement).style.width).toBe("0%")
        })
    })

    it("renders correctly with mockResumenConValoraciones", () => {
        const { container } = renderAndAnimate(
            mockResumenConValoraciones.distribucion,
            mockResumenConValoraciones.totalValoraciones
        )
        const countSpans = container.querySelectorAll("span.w-4")
        expect(countSpans[0].textContent).toBe("7") // 5-star count
        expect(countSpans[1].textContent).toBe("3") // 4-star count
    })

    it("renders correctly with mockResumenSinValoraciones", () => {
        const { container } = renderAndAnimate(
            mockResumenSinValoraciones.distribucion,
            mockResumenSinValoraciones.totalValoraciones
        )
        const countSpans = container.querySelectorAll("span.w-4")
        Array.from(countSpans).forEach((span) => {
            expect(span.textContent).toBe("0")
        })
    })
})

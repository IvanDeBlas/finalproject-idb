import { describe, it, expect } from "vitest"
import {
    mapTipoEventoPromoToLabel,
    mapTipoEventoPromoToBadgeClass,
} from "@shared/utils/mappers"
import { formatTasaConversion } from "@shared/utils/format"

describe("mapTipoEventoPromoToLabel", () => {
    it('returns "Click" for type 1', () => {
        expect(mapTipoEventoPromoToLabel(1)).toBe("Click")
    })

    it('returns "Vista" for type 2', () => {
        expect(mapTipoEventoPromoToLabel(2)).toBe("Vista")
    })

    it('returns "Registro" for type 3', () => {
        expect(mapTipoEventoPromoToLabel(3)).toBe("Registro")
    })

    it('returns "Backing" for type 4', () => {
        expect(mapTipoEventoPromoToLabel(4)).toBe("Backing")
    })

    it('returns "Share" for type 5', () => {
        expect(mapTipoEventoPromoToLabel(5)).toBe("Share")
    })

    it('returns "Evento" for unknown type', () => {
        expect(mapTipoEventoPromoToLabel(99)).toBe("Evento")
    })
})

describe("mapTipoEventoPromoToBadgeClass", () => {
    it("contains 'blue' for Click (type 1)", () => {
        expect(mapTipoEventoPromoToBadgeClass(1)).toContain("blue")
    })

    it("contains 'slate' for PageView (type 2)", () => {
        expect(mapTipoEventoPromoToBadgeClass(2)).toContain("slate")
    })

    it("contains 'green' for Signup (type 3)", () => {
        expect(mapTipoEventoPromoToBadgeClass(3)).toContain("green")
    })

    it("contains 'purple' for Backing (type 4)", () => {
        expect(mapTipoEventoPromoToBadgeClass(4)).toContain("purple")
    })

    it("contains 'amber' for Share (type 5)", () => {
        expect(mapTipoEventoPromoToBadgeClass(5)).toContain("amber")
    })

    it("returns fallback slate class for unknown type", () => {
        expect(mapTipoEventoPromoToBadgeClass(99)).toContain("slate")
    })
})

describe("formatTasaConversion", () => {
    it('formats 1.11 as "1.11%"', () => {
        expect(formatTasaConversion(1.11)).toBe("1.11%")
    })

    it('formats 0 as "0.00%"', () => {
        expect(formatTasaConversion(0)).toBe("0.00%")
    })

    it('formats 100 as "100.00%"', () => {
        expect(formatTasaConversion(100)).toBe("100.00%")
    })

    it('formats falsy value as "0.00%"', () => {
        expect(formatTasaConversion(0)).toBe("0.00%")
    })
})

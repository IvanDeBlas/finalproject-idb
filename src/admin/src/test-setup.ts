import "@testing-library/jest-dom/vitest"

// Polyfill ResizeObserver for Radix UI components in jsdom
class ResizeObserverMock {
    observe() {}
    unobserve() {}
    disconnect() {}
}
globalThis.ResizeObserver = ResizeObserverMock as unknown as typeof ResizeObserver

// Polyfill matchMedia for Radix UI / Tailwind responsive utilities
Object.defineProperty(window, "matchMedia", {
    writable: true,
    value: (query: string) => ({
        matches: false,
        media: query,
        onchange: null,
        addListener: () => {},
        removeListener: () => {},
        addEventListener: () => {},
        removeEventListener: () => {},
        dispatchEvent: () => false,
    }),
})

// Polyfill HTMLElement.scrollIntoView for Radix Dialog
Element.prototype.scrollIntoView = () => {}

// Polyfill hasPointerCapture/setPointerCapture/releasePointerCapture
Element.prototype.hasPointerCapture = () => false
Element.prototype.setPointerCapture = () => {}
Element.prototype.releasePointerCapture = () => {}

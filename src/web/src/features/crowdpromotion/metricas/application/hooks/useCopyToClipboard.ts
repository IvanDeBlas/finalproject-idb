import { useState, useCallback, useRef, useEffect } from "react"
import { toast } from "sonner"

export function useCopyToClipboard(text: string, resetAfterMs: number = 2000) {
    const [copied, setCopied] = useState(false)
    const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null)

    useEffect(() => {
        return () => {
            if (timeoutRef.current) clearTimeout(timeoutRef.current)
        }
    }, [])

    const copy = useCallback(async () => {
        try {
            await navigator.clipboard.writeText(text)
            setCopied(true)
            timeoutRef.current = setTimeout(() => setCopied(false), resetAfterMs)
        } catch {
            try {
                const textarea = document.createElement("textarea")
                textarea.value = text
                textarea.style.position = "fixed"
                textarea.style.left = "-9999px"
                document.body.appendChild(textarea)
                textarea.select()
                document.execCommand("copy")
                document.body.removeChild(textarea)
                setCopied(true)
                timeoutRef.current = setTimeout(() => setCopied(false), resetAfterMs)
            } catch {
                toast.error("No se pudo copiar. Copia el enlace manualmente.")
            }
        }
    }, [text, resetAfterMs])

    return { copied, copy }
}

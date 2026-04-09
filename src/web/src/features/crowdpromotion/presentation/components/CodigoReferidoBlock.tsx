import { type FC, useState, useCallback } from "react"
import { Button } from "@/components/ui/button"
import { Copy, Check } from "lucide-react"
import { toast } from "sonner"

interface CodigoReferidoBlockProps {
    codigo: string
}

export const CodigoReferidoBlock: FC<CodigoReferidoBlockProps> = ({ codigo }) => {
    const [copied, setCopied] = useState(false)

    const handleCopy = useCallback(async () => {
        try {
            await navigator.clipboard.writeText(codigo)
            setCopied(true)
            toast.success("Codigo referido copiado al portapapeles")
            setTimeout(() => setCopied(false), 1500)
        } catch {
            toast.error("No se pudo copiar el codigo")
        }
    }, [codigo])

    return (
        <div>
            <p className="text-xs font-medium text-[#94a3b8] mb-1.5">
                Codigo referido:
            </p>
            <div className="flex items-center gap-2">
                <div
                    className="flex-1 flex items-center bg-[#0f0f1f] border border-[#a855f7]/30 rounded-lg px-3 h-10 font-mono text-sm text-[#a855f7] overflow-hidden"
                    role="textbox"
                    aria-readonly="true"
                    aria-label="Codigo referido"
                >
                    <span className="truncate">{codigo}</span>
                </div>
                <Button
                    variant="ghost"
                    size="icon"
                    className="h-10 w-10 p-0 text-[#64748b] hover:text-[#a855f7] hover:bg-[#a855f7]/10 shrink-0"
                    onClick={handleCopy}
                    aria-label={copied ? "Codigo copiado" : "Copiar codigo referido"}
                >
                    {copied
                        ? <Check className="w-4 h-4 text-green-400" aria-hidden="true" />
                        : <Copy className="w-4 h-4" aria-hidden="true" />
                    }
                </Button>
            </div>
        </div>
    )
}

import { Copy, Check } from "lucide-react"
import { Card } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { useCopyToClipboard } from "../../application/hooks/useCopyToClipboard"

interface EnlaceReferidoProps {
    url: string
}

export function EnlaceReferido({ url }: EnlaceReferidoProps) {
    const { copied, copy } = useCopyToClipboard(url, 2000)

    return (
        <Card className="bg-[#151525] border-[#334155] p-5 mb-6">
            <p className="text-sm font-medium text-[#94a3b8] mb-3">
                Mi enlace de promocion
            </p>
            <div className="flex items-center gap-2">
                <Input
                    readOnly
                    value={url}
                    aria-label="Tu enlace de promocion"
                    aria-readonly="true"
                    className="bg-[#0f0f1f] border-[#334155] text-[#94a3b8] text-sm flex-1 font-mono cursor-default focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e] truncate sm:truncate-none"
                />
                <Button
                    variant="outline"
                    size="sm"
                    onClick={copy}
                    aria-label={copied ? "Enlace copiado" : "Copiar enlace de promocion"}
                    className={
                        copied
                            ? "shrink-0 border-green-800/50 text-[#10b981]"
                            : "shrink-0 border-[#334155] text-[#94a3b8] hover:border-[#a855f7] hover:text-[#a855f7] focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
                    }
                    disabled={copied}
                >
                    {copied ? (
                        <>
                            <Check className="w-4 h-4 mr-1.5" />
                            Copiado!
                        </>
                    ) : (
                        <>
                            <Copy className="w-4 h-4 mr-1.5" />
                            Copiar
                        </>
                    )}
                </Button>
            </div>
        </Card>
    )
}
